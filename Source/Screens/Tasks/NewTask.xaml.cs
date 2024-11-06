using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Runtime.CompilerServices;
using TaskListMaui.Source.Domain.Main.DTOs.TaskDTOs;
using TaskListMaui.Source.Domain.Main.UseCase.ResponseCase;

namespace TaskListMaui.Source.Screens.Tasks;

public partial class NewTask : ContentPage
{
    private readonly string _userId;
    private readonly string _token;

	public NewTask(string userId, string token)
	{
        _userId = userId;
        _token = token;
		InitializeComponent();
        
    }

    private async void CloseModal(object sender, EventArgs e)
    {
        await Navigation.PopModalAsync();
    }

    private  async void OnCreateTask(object sender, EventArgs e)
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
                Deadline = DateOnly.FromDateTime(deadline),
            };


            HttpClientHandler handler = new()
            {
                ServerCertificateCustomValidationCallback = (message, cert, chain, sslPolicyErrors) => true
            };
            HttpClient http = new(handler);

            http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _token);

            var url = "https://192.168.10.10:7103/task/create";


            var response = await http.PostAsJsonAsync(url, newTask);

            var result = await response.Content.ReadFromJsonAsync<Response>();



            await DisplayAlert("Mensagem", $"{result?.Message}", "Fechar");
            await Navigation.PopModalAsync();
        }
        catch (Exception ex)
        {

            await DisplayAlert("Erro", $"Erro ao criar tarefa. {ex.Message} - {ex.InnerException}", "Fechar");
        }
        finally {
            Loading.IsRunning = false;
            btnCreate.IsEnabled = true;
        }
        
    }
}