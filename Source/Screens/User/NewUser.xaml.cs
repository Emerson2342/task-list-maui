using System.Net.Http.Json;
using TaskListMaui.Source.Domain.Main.DTOs.UserDTOs;
using TaskListMaui.Source.Domain.Main.UseCase.ResponseCase;

namespace TaskListMaui.Source.Screens.User;

public partial class NewUser : ContentPage
{
    public NewUser()
    {
        InitializeComponent();
        double screenWidth = DeviceDisplay.MainDisplayInfo.Width / DeviceDisplay.MainDisplayInfo.Density;
        double screenHeight = DeviceDisplay.MainDisplayInfo.Height / DeviceDisplay.MainDisplayInfo.Density;
        ModalStack.WidthRequest = screenWidth * 0.8;
        ModalStack.HeightRequest = screenHeight * 0.4;

        Row0.Height = ModalStack.HeightRequest * 0.6;
        Row1.Height = ModalStack.HeightRequest * 0.4;

        WidthRow0.WidthRequest = ModalStack.WidthRequest;
        WidthRow1.WidthRequest = ModalStack.WidthRequest;

        NewName.WidthRequest = ModalStack.WidthRequest * 0.8;
        NewEmail.WidthRequest = ModalStack.WidthRequest * 0.8;
        NewPassword.WidthRequest = ModalStack.WidthRequest * 0.8;

        btnCreate.WidthRequest = ModalStack.WidthRequest * 0.4;
        btnClose.WidthRequest = ModalStack.WidthRequest * 0.4;

        BorderClose.Stroke = btnCreate.BackgroundColor;
        btnClose.TextColor = btnCreate.BackgroundColor;

        NewName.Text = string.Empty;
        NewEmail.Text = string.Empty;
        NewPassword.Text = string.Empty;

    }

    private async void CloseModal(object sender, EventArgs e)
    {
        await Navigation.PopModalAsync();
    }


    private async void OnCreateUser(object sender, EventArgs e)
    {
        
        try
        {
            Loading.IsRunning = true;
            btnCreate.IsEnabled = false;
            HttpClientHandler handler = new()
            {
                ServerCertificateCustomValidationCallback = (message, cert, chain, sslPolicyErrors) => true
            };
            var http = new HttpClient(handler);

            var name = NewName.Text;
            var email = NewEmail.Text;
            var password = NewPassword.Text;

            RequestNewUser newUser = new()
            {
                Name = name,
                Email = email,
                Password = password,
            };
            var request = await http.PostAsJsonAsync("https://192.168.10.10:7103/user/create-maui", newUser);

            var result = await request.Content.ReadFromJsonAsync<Response>();
            if (result != null)
            {
                if (result.IsSuccess)
                {
                    await DisplayAlert("Parabéns", $"{result.Message}", "Fechar");
                    await Navigation.PopModalAsync();
                }

                if (!result.IsSuccess)
                    await DisplayAlert("Erro", $"{result.Message}", "Fechar");
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Erro", $"Erro ao criar usuário. {ex.Message}", "Fechar");

        }
        finally
        {
            Loading.IsRunning = false;
            btnCreate.IsEnabled = true;
        }
    }
}