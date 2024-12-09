
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Reflection;
using TaskListMaui.Source.Domain.Main.DTOs.TaskDTOs;
using TaskListMaui.Source.Domain.Main.Entities;
using TaskListMaui.Source.Domain.Main.UseCase.ResponseCase;
using static System.Net.WebRequestMethods;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using Image = SixLabors.ImageSharp.Image;
using SixLabors.ImageSharp.Formats.Jpeg;
using SkiaSharp;
using System.IO;
using File = System.IO.File;
using SixLabors.ImageSharp.Processing.Processors;
using static TaskListMaui.Source.Domain.Main.Services.PhotoHelper;


namespace TaskListMaui.Source.Screens.Tasks;

public partial class EditTask : ContentPage
{
    private readonly string _token;
    private readonly string Ip = Configuration.IpAddress;

    private readonly string _taskId;
    private Guid UserId { get; set; }

    private RequestTask? TaskEdited { get; set; }
    private Stream? PhotoFile;
    private SKEncodedOrigin encodedOrigin;

    public EditTask(string token, string idTask)
    {
        InitializeComponent();

        _token = token;
        _taskId = idTask;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        HttpClientHandler handler = new()
        {
            ServerCertificateCustomValidationCallback = (message, cert, chain, sslPolicyErrors) => true
        };
        HttpClient http = new(handler);

        http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _token);

        var url = $"https://{Ip}/task/task/{_taskId}";

        var request = await http.GetAsync(url);
        var result = await request.Content.ReadFromJsonAsync<Response>();

        if (result == null)
        {
            await DisplayAlert("Erro", $"Erro ao buscar dados!", "Fechar");
            return;
        }
        if (result.Task != null)
        {
            ABTitle.Text = result.Task.Title;
            ABDescription.Text = result.Task.Description;
            ABStartTime.Date = result.Task.StartTime.ToDateTime(TimeOnly.MinValue);
            ABDeadline.Date = result.Task.Deadline.ToDateTime(TimeOnly.MinValue);
            UserId = result.Task.UserId;
        }
        try
        {
            Loading.IsRunning = true;
            var responseImage = await http.GetAsync($"https://{Configuration.IpAddress}/task/get-photo/{_taskId}");


            var image = await responseImage.Content.ReadAsByteArrayAsync();

            var stream = new MemoryStream(image);
            ABDTaskPhoto.Source = ImageSource.FromStream(() => stream);
        }
        catch (Exception ex)
        {
            await DisplayAlert("Erro", $"Falha ao carregar imagem. {ex.Message} - {ex.InnerException}", "Fechar");
        }
        finally
        {
            Loading.IsRunning = false;
        }
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

                    using (FileStream localFileStream = System.IO.File.OpenWrite(localFilePath))
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

    private async void EditTask_Clicked(object sender, EventArgs e)
    {
        try
        {
            Loading.IsRunning = true;
            btnEditTask.IsEnabled = false;

            TaskEdited = new()
            {
                Id = Guid.Parse(_taskId),
                Title = ABTitle.Text,
                Description = ABDescription.Text,
                StartTime = DateOnly.FromDateTime(ABStartTime.Date),
                Deadline = DateOnly.FromDateTime(ABDeadline.Date),
                UserId = UserId,
            };

            var formData = new MultipartFormDataContent
            {
               { new StringContent(TaskEdited.UserId.ToString()), "UserId" },
               { new StringContent(TaskEdited.Id.ToString()), "id" },
                { new StringContent(TaskEdited.Title), "Title" },
                { new StringContent(TaskEdited.Description), "Description" },
                { new StringContent(TaskEdited.StartTime.ToString("yyyy-MM-dd")), "StartTime" },
                { new StringContent(TaskEdited.Deadline.ToString("yyyy-MM-dd")), "Deadline" }

            };

            if (PhotoFile != null)
                formData.Add(new StreamContent(PhotoFile), "photoFile", "photo.jpg");

            //await DisplayAlert("Imagem", TaskEdited.PhotoTask.Substring(0, 100) + "...", "Fechar");
            //return;


            HttpClientHandler handler = new()
            {
                ServerCertificateCustomValidationCallback = (message, cert, chain, sslPolicyErrors) => true
            };
            HttpClient http = new(handler);

            http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _token);

            var url = $"https://{Ip}/task/edit";

            var response = await http.PostAsync(url, formData);

            var result = await response.Content.ReadFromJsonAsync<Response>();

            if (result == null)
            {
                await DisplayAlert("Erro", "Erro ao ler os dados JSON", "Fechar");
                return;
            }

            if (!result.IsSuccess)
            {
                await DisplayAlert("Erro", $"{result.Message}", "Fechar");
                return;
            }
            await DisplayAlert("Editada", $"Tarefa editada com sucesso!", "Fechar");

            await Navigation.PopModalAsync();

        }
        catch (Exception ex)
        {
            await DisplayAlert("Erro", $"{ex.Message} - {ex.InnerException}", "Fechar");
        }
        finally
        {
            Loading.IsRunning = false;
            btnEditTask.IsEnabled = true;
        }
    }

    private async void Back_Clicked(object sender, EventArgs e)
    {
        await Navigation.PopModalAsync();
    }

}