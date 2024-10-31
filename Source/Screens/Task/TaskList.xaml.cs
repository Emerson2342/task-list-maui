using System.Net.Http.Headers;

namespace TaskListMaui.Source.Screens.Task;

public partial class TaskList : ContentPage
{
    private readonly string _token;
    private readonly string _userId;

	public TaskList(string token, string userId)
	{
		InitializeComponent();
        _token = token;
        _userId = userId;

        DisplayAlert("Token", $"Token: {token}\nUserId: {userId}","Fechar");
	}

    private async void Mostrar_Clicked(object sender, EventArgs e)
    {
        HttpClientHandler handler = new HttpClientHandler
        {
            ServerCertificateCustomValidationCallback = (message, cert, chain, sslPolicyErrors) => true
        };
        HttpClient http = new(handler);

        http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _token);

        var request = await http.GetAsync($"https://localhost:7103/task/list/{_userId}");


    }
}