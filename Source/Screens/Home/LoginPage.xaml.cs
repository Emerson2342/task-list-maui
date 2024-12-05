
using Microsoft.Extensions.Logging;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Json;
using TaskListMaui.Source.Domain.Main.DTOs.UserDTOs;
using TaskListMaui.Source.Domain.Main.Services;
using TaskListMaui.Source.Domain.Main.UseCase.ResponseCase;
using TaskListMaui.Source.Screens.Tasks;
using TaskListMaui.Source.Screens.User;

namespace TaskListMaui.Source.Screens.Home;

public partial class LoginPage : ContentPage
{

    private readonly string Ip = Configuration.IpAddress;

    public LoginPage()
    {
        
        InitializeComponent();

        Loading.IsVisible = false;

        double screenWidth = DeviceDisplay.MainDisplayInfo.Width / DeviceDisplay.MainDisplayInfo.Density;
        double screenHeight = DeviceDisplay.MainDisplayInfo.Height / DeviceDisplay.MainDisplayInfo.Density;

        Row0.Height = screenHeight * 0.4;
        Row1.Height = screenWidth * 0.4;
        Row2.Height = screenHeight * 0.2;

        LoginEmail.Text = "lyncoln_erc@hotmail.com";
        LoginPassword.Text = "123456";
    }

    private async void HandlerPassword(object sender, EventArgs e)
    {
        await Navigation.PushModalAsync(new ForgotPassword());
    }

    private async void SignIn_Clicked(object sender, EventArgs e)
    {
        HttpClientHandler handler = new HttpClientHandler
        {
            ServerCertificateCustomValidationCallback = (message, cert, chain, sslPolicyErrors) => true
        };
        HttpClient http = new(handler);

        var email = LoginEmail.Text;
        var password = LoginPassword.Text;

        RequestLogin model = new()
        {
            Email = email,
            Password = password
        };

        try
        {
            Loading.IsVisible = true;

            var request = await http.PostAsJsonAsync($"https://{Ip}/login", model);
            var response = await request.Content.ReadFromJsonAsync<Response>();
            if (response == null || response?.User == null || response.IsSuccess == false)
            {
                await DisplayAlert("Erro ao logar", $"{response?.Message}", "Fechar");
            }
            else
            {
                await AuthenticationService.SaveToken(response.User.Token);
                await Navigation.PushModalAsync(new TaskList());
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Erro ao logar", $"{ex.Message} - {ex.InnerException}", "Fechar");
        }
        finally
        {
            Loading.IsVisible = false;
        }

    }
    private async void SignUp_Clicked(object sender, EventArgs e)
    {
        await Navigation.PushModalAsync(new NewUser());
    }
}

