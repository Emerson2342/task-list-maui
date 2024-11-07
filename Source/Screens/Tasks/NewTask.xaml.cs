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

            var url = $"https://{Ip}/task/create";


            var response = await http.PostAsJsonAsync(url, newTask);

            var result = await response.Content.ReadFromJsonAsync<Response>();

            if(result == null)
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
        finally {
            Loading.IsRunning = false;
            btnCreate.IsEnabled = true;
        }
        
    }
}