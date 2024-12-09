
using Microsoft.AspNetCore.Http;
using SkiaSharp;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Reflection;
using System.Runtime.CompilerServices;
using TaskListMaui.Source.Domain.Main.DTOs.TaskDTOs;
using TaskListMaui.Source.Domain.Main.UseCase.ResponseCase;
using static TaskListMaui.Source.Domain.Main.Services.PhotoHelper;

namespace TaskListMaui.Source.Screens.Tasks;

public partial class NewTask : ContentPage
{
    private readonly string _userId;
    private readonly string _token;

    private Stream? PhotoFile;
    private SKEncodedOrigin encodedOrigin;


    private readonly string Ip = Configuration.IpAddress;

    public NewTask(string userId, string token)
    {
        _userId = userId;
        _token = token;
        InitializeComponent();
        TitleTask.Text = string.Empty;
        Description.Text = string.Empty;
    }

    private async void CloseModal(object sender, EventArgs e)
    {
        await Navigation.PopModalAsync();
    }

    public async void TakePhoto(object sender, EventArgs e)
    {
        if (MediaPicker.Default.IsCaptureSupported)
        {
            FileResult? photo = await MediaPicker.Default.CapturePhotoAsync();

            if (photo != null)
            {
                string localFilePath = Path.Combine(FileSystem.CacheDirectory, photo.FileName);

                using (Stream sourceStream = await photo.OpenReadAsync())
                using (FileStream localFileStream = File.OpenWrite(localFilePath))
                {
                    await sourceStream.CopyToAsync(localFileStream);
                }

                using (Stream imageStream = File.OpenRead(localFilePath))
                {

                    SKEncodedOrigin origin;
                    SKBitmap bitmap = LoadBitmapWithOrigin(imageStream, out origin);

                    if (bitmap != null)
                    {

                        int newWidth = bitmap.Width / 5;
                        int newHeight = bitmap.Height / 5;
                        SKBitmap resizedBitmap = bitmap.Resize(new SKImageInfo(newWidth, newHeight), SKFilterQuality.Medium);
                        
                        SKBitmap orientedBitmap = AutoOrient(resizedBitmap, origin);

                        string newFilePath = Path.Combine(FileSystem.CacheDirectory, "resized_" + photo.FileName);
                        using (var newFileStream = new FileStream(newFilePath, FileMode.Create, FileAccess.Write))
                        {

                            using (var image = SKImage.FromBitmap(orientedBitmap))
                            using (var data = image.Encode())
                            {
                                data.SaveTo(newFileStream);
                            }
                        }
                        PhotoFile = new FileStream(newFilePath, FileMode.Open, FileAccess.Read);
                        ABDTaskPhoto.Source = ImageSource.FromFile(newFilePath);
                    }

                }
            }
        }
    }    

    private async void PickFromGalery(object sender, EventArgs e)
    {
        try
        {
            FileResult? photo = await MediaPicker.Default.PickPhotoAsync();

            if (photo != null)
            {

                string localFilePath = Path.Combine(FileSystem.CacheDirectory, photo.FileName);

                using (Stream sourceStream = await photo.OpenReadAsync())
                {

                    using (FileStream localFileStream = File.OpenWrite(localFilePath))
                    {
                        await sourceStream.CopyToAsync(localFileStream);
                    }
                }
                PhotoFile = new FileStream(localFilePath, FileMode.Open);

            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Erro", $"Erro ao pegar foto da galeria: {ex.Message}", "Fechar");
        }
    }

    private async void OnCreateTask(object sender, EventArgs e)
    {
        try
        {
            Loading.IsRunning = true;
            btnCreate.IsEnabled = false;

            var title = TitleTask.Text;
            var description = Description.Text;
            var startTime = StartDate.Date;
            var deadline = Deadline.Date;

            if (_userId == null)
            {
                await DisplayAlert("Erro", "Erro ao criar Tarefa", "Fechar");
                return;
            }

            RequestTask newTask = new()
            {
                UserId = Guid.Parse(_userId),
                Title = title,
                Description = description,
                StartTime = DateOnly.FromDateTime(startTime),
                Deadline = DateOnly.FromDateTime(deadline)
            };

            var formData = new MultipartFormDataContent
            {
               { new StringContent(newTask.UserId.ToString()), "UserId" },
                { new StringContent(newTask.Title), "Title" },
                { new StringContent(newTask.Description), "Description" },
                { new StringContent(newTask.StartTime.ToString("yyyy-MM-dd")), "StartTime" },
                { new StringContent(newTask.Deadline.ToString("yyyy-MM-dd")), "Deadline" }

            };

            if (PhotoFile != null)
                formData.Add(new StreamContent(PhotoFile), "photoFile", "photo.jpg");


            HttpClientHandler handler = new()
            {
                ServerCertificateCustomValidationCallback = (message, cert, chain, sslPolicyErrors) => true
            };
            HttpClient http = new(handler);

            http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _token);

            var url = $"https://{Ip}/task/create";

            var response = await http.PostAsync(url, formData);

            var result = await response.Content.ReadFromJsonAsync<Response>();

            if (result == null)
            {
                await DisplayAlert("Mensagem", $"Erro ao criar tarefa", "Fechar");
                return;
            }

            if (result.IsSuccess)
            {
                await DisplayAlert("Mensagem", $"{result.Message}", "Fechar");
                await Navigation.PopModalAsync();
            }
            await DisplayAlert("Erro", $"{result.Message}", "Fechar");

        }
        catch (Exception ex)
        {

            await DisplayAlert("Erro", $"Erro ao criar tarefa. {ex.Message} - {ex.InnerException}", "Fechar");
        }
        finally
        {
            Loading.IsRunning = false;
            btnCreate.IsEnabled = true;
        }

    }
}