
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
    private Stream? PhotoFile;

    public EditTask(string token, string idTask)
    {
        InitializeComponent();
       
        _token = token;
        _taskId = idTask;

        if (PhotoFile != null)
        {
            BorderPhoto.StrokeThickness = 2;
        }
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
                string localFilePath = Path.Combine(FileSystem.CacheDirectory, photo.FileName);

                using (Stream sourceStream = await photo.OpenReadAsync())
                using (FileStream localFileStream = System.IO.File.OpenWrite(localFilePath))
                {
                    await sourceStream.CopyToAsync(localFileStream);
                }

                PhotoFile = new FileStream(localFilePath, FileMode.Open);
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