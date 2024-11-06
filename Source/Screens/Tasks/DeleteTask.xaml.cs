using System.Net.Http.Headers;
using static System.Net.WebRequestMethods;
using System.Reflection;
using TaskListMaui.Source.Domain.Main.DTOs.TaskDTOs;
using System.Net.Http.Json;

namespace TaskListMaui.Source.Screens.Tasks;

public partial class DeleteTask : ContentPage
{
    private readonly string _token;
    private readonly string _taskId;

	public DeleteTask(string token, string taskId)
	{
		InitializeComponent();
	}

    private async void Back_Clicked(object sender, EventArgs e)
    {
        await Navigation.PopModalAsync();
    }

    private async void Delete_Clicked(object sender, EventArgs e)
    {
        try
        {
            HttpClientHandler handler = new()
            {
                ServerCertificateCustomValidationCallback = (message, cert, chain, sslPolicyErrors) => true
            };
            HttpClient http = new(handler);

            http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _token);

            RequestTask taskToDelete = new();

            taskToDelete.Id = Guid.Parse(_taskId);

            var request = await http.PostAsJsonAsync("https://192.168.10.10:7103/task/delete", taskToDelete);

            if (request == null)
            {
                await DisplayAlert("Erro", "Erro ao ler os dados", "Fechar");
                return;
            }
            await Navigation.PopModalAsync();
        }
        catch (Exception ex) {
            await DisplayAlert("Erro", $"{ex.Message} - {ex.InnerException}", "Fechar");
        
        }

    }
}