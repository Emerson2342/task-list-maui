using SkiaSharp;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Reflection;
using TaskListMaui.Source.Domain.Main.DTOs.TaskDTOs;
using TaskListMaui.Source.Domain.Main.Entities;
using TaskListMaui.Source.Domain.Main.UseCase.ResponseCase;
using static System.Net.WebRequestMethods;

namespace TaskListMaui.Source.Screens.Tasks;

public partial class EditTask : ContentPage
{
    private readonly string _token;
    private readonly string Ip = Configuration.IpAddress;

    private readonly string _taskId;
    private Guid UserId { get; set; }

    private RequestTask? TaskEdited { get; set; }
    private string PhotoTask = string.Empty;

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
            ABDTaskPhoto.Source = ImageSource.FromStream(() =>
                    new MemoryStream(Convert.FromBase64String(result.Task.PhotoTask)));

        }
    }

    private async void TakePhoto(object sender, EventArgs e)
    {
        if (MediaPicker.Default.IsCaptureSupported)
        {
            FileResult? photo = await MediaPicker.Default.CapturePhotoAsync();

            if (photo != null)
            {
               
                using Stream sourceStream = await photo.OpenReadAsync();

                SKBitmap bitmap = SKBitmap.Decode(sourceStream);

                int newWidth = 300;
                int newHeight = 500;

                SKBitmap resizedBitmap = bitmap.Resize(new SKImageInfo(newWidth, newHeight), SKFilterQuality.Low);

                using (MemoryStream memoryStream = new MemoryStream())
                {
                    SKImage image = SKImage.FromBitmap(resizedBitmap);
                    SKData encoded = image.Encode(SKEncodedImageFormat.Jpeg, 60);
                    encoded.SaveTo(memoryStream);
                    
                    byte[] bytes = memoryStream.ToArray();

                    string base64Image = Convert.ToBase64String(bytes);

                    PhotoTask = base64Image;
                    //await DisplayAlert("Imagem", base64Photo.Substring(0, 100) + "...", "Fechar");
                }
            }
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
                PhotoTask = PhotoTask
            };

            //await DisplayAlert("Imagem", TaskEdited.PhotoTask.Substring(0, 100) + "...", "Fechar");
            //return;


            HttpClientHandler handler = new()
            {
                ServerCertificateCustomValidationCallback = (message, cert, chain, sslPolicyErrors) => true
            };
            HttpClient http = new(handler);

            http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _token);

            var url = $"https://{Ip}/task/edit";

            var request = await http.PostAsJsonAsync(url, TaskEdited);

            var result = await request.Content.ReadFromJsonAsync<Response>();

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