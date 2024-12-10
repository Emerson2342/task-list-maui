using TaskListMaui.Source.Domain.Main.UseCase.ResponseCase;
using System.Net.Http.Json;
using TaskListMaui.Source.Domain.Main.DTOs.UserDTOs;

namespace TaskListMaui.Source.Screens.User;

public partial class ForgotPassword : ContentPage
{
    private readonly string Ip = Configuration.IpAddress;

    private RequestEmail EmailUser { get; set; } = new();
    public ForgotPassword()
    {
        InitializeComponent();
        double screenWidth = DeviceDisplay.MainDisplayInfo.Width / DeviceDisplay.MainDisplayInfo.Density;
        double screenHeight = DeviceDisplay.MainDisplayInfo.Height / DeviceDisplay.MainDisplayInfo.Density;
        ModalStack.WidthRequest = screenWidth * 0.8;
        ModalStack.HeightRequest = screenHeight * 0.3;

        UserEmail.WidthRequest = ModalStack.WidthRequest * 0.8;

        btnClose.WidthRequest = ModalStack.WidthRequest * 0.4;
        btnSendEmail.WidthRequest = ModalStack.WidthRequest * 0.4;

        BorderClose.Stroke = btnSendEmail.BackgroundColor;
        btnSendEmail.BorderColor = btnSendEmail.BackgroundColor;
        btnClose.TextColor = btnSendEmail.BackgroundColor;

       
    }

    private async void OnSendEmail_Clicked(object sender, EventArgs e)
    {
        try
        {
            Loading.IsRunning = true;
            btnSendEmail.IsEnabled = false;
            if (string.IsNullOrEmpty(UserEmail.Text))
            {
                await DisplayAlert("Erro", "Favor preencher o campo do EMAIL", "Fechar");
                return;
            }


            EmailUser.Email = UserEmail.Text.ToLower();
            HttpClientHandler handler = new()
            {
                ServerCertificateCustomValidationCallback = (message, cert, chain, sslPolicyErrors) => true
            };
            HttpClient http = new(handler);


            var response = await http.PostAsJsonAsync($"https://{Ip}/user/reset-password-out", EmailUser);

            var result = await response.Content.ReadFromJsonAsync<Response>();
            if (result != null)
            {
                if(result.IsSuccess)
                    {
                    await DisplayAlert("", $"{result.Message}", "Fechar");
                    await Navigation.PopModalAsync();
                    return;
                }
                else
                {
                    await DisplayAlert("", $"{result.Message}", "Fechar");
                }
                
            }
            await DisplayAlert("Erro", $"Erro ao enviar email!", "Fechar");
        }
        catch (Exception ex)
        {            
            await DisplayAlert("Erro", $"Erro ao enviar email. {ex.Message} - {ex.InnerException}", "Fechar");
        }
        finally
        {
            Loading.IsRunning = false;
            btnSendEmail.IsEnabled = true;
        }





    }


    private async void CloseModal(object sender, EventArgs e)
    {
        await Navigation.PopModalAsync();
    }
}