using System.Net.Http.Headers;
using System.Net.Http.Json;
using TaskListMaui.Source.Domain.Main.DTOs.UserDTOs;
using TaskListMaui.Source.Domain.Main.UseCase.ResponseCase;

namespace TaskListMaui.Source.Screens.User;

public partial class ChangePassword : ContentPage
{

    private readonly string _token;
    private readonly string Ip = Configuration.IpAddress;

    public RequestNewPassword NewPassword { get; set; } = new();
    public ChangePassword(string token)
	{
		InitializeComponent();
        ABNewPassword.Text = string.Empty;

        _token = token;
    }

    private async void Back_Clicked(object sender, EventArgs e)
    {
        await Navigation.PopModalAsync();
    }

    private async void ChangePassword_Clicked(object sender, EventArgs e)
    {

        try
        {
            NewPassword.NewPassword = ABNewPassword.Text;

            HttpClientHandler handler = new()
            {
                ServerCertificateCustomValidationCallback = (message, cert, chain, sslPolicyErrors) => true
            };
            HttpClient http = new(handler);

            http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _token);

            var url = $"https://{Ip}/user/change-password-in";

            var response = await http.PostAsJsonAsync(url, NewPassword);

            var result = await response.Content.ReadFromJsonAsync<Response>();

            if (result != null) {

                if (result.IsSuccess) {
                    await DisplayAlert("", $"{result.Message}", "Fechar");
                    await Navigation.PopModalAsync();
                    return;
                }
                await DisplayAlert("", $"{result.Message}", "Fechar");


            }

        }
        catch (Exception ex) {

           await DisplayAlert("Erro", $"{ex.Message} - {ex.InnerException}", "Fechar");
        }
        




    }
}