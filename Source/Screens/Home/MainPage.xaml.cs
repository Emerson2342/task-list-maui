
using System.Net.Http.Json;
using TaskListMaui.Source.Domain.Main.DTOs.UserDTOs;
using TaskListMaui.Source.Domain.Main.Entities;
using TaskListMaui.Source.Domain.Main.UseCase.ResponseCase;
using TaskListMaui.Source.Screens.Task;
using TaskListMaui.Source.Screens.User;

namespace TaskListMaui.Source.Screens.Home;

public partial class MainPage : ContentPage
{

    public MainPage()
    {    
        InitializeComponent();   
        

    }

    private void HandlerPassword(object sender, EventArgs e)
    {
    }

    private async void SignIn_Clicked(object sender, EventArgs e)
    {
        HttpClientHandler handler = new HttpClientHandler
        {
            ServerCertificateCustomValidationCallback = (message, cert, chain, sslPolicyErrors) => true
        };
        HttpClient http = new (handler);

        var email = Email.Text;
        var password = Password.Text;

        RequestLogin model = new()
        {
            Email = email,
            Password = password
        };

        try
        {
            var request = await http.PostAsJsonAsync("https://localhost:7103/login", model);
            var response = await request.Content.ReadFromJsonAsync<Response>();
            if (response == null || response?.User == null || response.IsSuccess == false) {
              await DisplayAlert("Erro ao logar", $"{response?.Message}", "Fechar");
            }
            else
            {
                await Navigation.PushAsync(new TaskList(response.User.Token, response.User.Id.ToString()));
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Erro ao logar", $"{ex.Message}", "Fechar");
        }

    }
    private async void SignUp_Clicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new NewUser());
    }
}

