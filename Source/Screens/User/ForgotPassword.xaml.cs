namespace TaskListMaui.Source.Screens.User;

public partial class ForgotPassword : ContentPage
{
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

    private async void CloseModal(object sender, EventArgs e)
    {
        await Navigation.PopModalAsync();
    }
}