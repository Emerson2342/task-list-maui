using System.Net.Http.Headers;
using static System.Net.WebRequestMethods;
using System.Reflection;
using TaskListMaui.Source.Domain.Main.DTOs.TaskDTOs;
using System.Net.Http.Json;

namespace TaskListMaui.Source.Screens.Tasks;

public partial class DeleteTask : ContentPage
{
    private readonly string _token;
    private readonly Guid _taskId;

    private readonly string Ip = Configuration.IpAddress;

    public DeleteTask(string token, Guid taskId, string title)
	{
		InitializeComponent();
        _token = token;
        _taskId = taskId;

        ABC.Text = title;
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

            taskToDelete.Id = _taskId;

            var request = await http.PostAsJsonAsync($"https://{Ip}/task/delete", taskToDelete);

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