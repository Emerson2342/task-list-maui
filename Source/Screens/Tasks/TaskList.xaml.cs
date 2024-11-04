using System.Net.Http.Headers;
using System.Net.Http.Json;
using TaskListMaui.Source.Domain.Main.Entities;
using TaskListMaui.Source.Domain.Main.UseCase.ResponseCase;

namespace TaskListMaui.Source.Screens.Tasks;

public partial class TaskList : ContentPage
{
    private readonly string _token;
    private readonly string _userId;

    private string message = string.Empty;

    public List<TaskEntity> TasksList { get; set; } =new();

    public TaskList(string token, string userId)
	{
		InitializeComponent();

        _token = token;
        _userId = userId;

        BindingContext = this;


        DisplayAlert("Token", $"Token: {token}\nUserId: {userId}","Fechar");
        LoadTasksList();


    }

    private async void LoadTasksList()
    {
        HttpClientHandler handler = new ()
        {
            ServerCertificateCustomValidationCallback = (message, cert, chain, sslPolicyErrors) => true
        };
        HttpClient http = new(handler);

        http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _token);

        var request = await http.GetAsync($"https://192.168.10.10:7103/task/list/{_userId}");

        if (request.IsSuccessStatusCode) { 
        var result = await request.Content.ReadFromJsonAsync<Response>();
            if (result == null)
            {
                message = "Erro ao ler os dados";
                return;
            }

            if (result.TaskList == null)
                return;

            TasksList = result.TaskList;
            message = result.Message;

        }
        await DisplayAlert("Banco de Dados", $"{TasksList[0].Description}", "Fechar");


    }




}