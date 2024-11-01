namespace TaskListMaui.Source.Screens.User;

public partial class NewUser : ContentPage
{
	public NewUser()
	{
		InitializeComponent();
		double screenWidth = DeviceDisplay.MainDisplayInfo.Width / DeviceDisplay.MainDisplayInfo.Density;
        double screenHeight = DeviceDisplay.MainDisplayInfo.Height / DeviceDisplay.MainDisplayInfo.Density;
        ModalStack.WidthRequest = screenWidth * 0.8;
        ModalStack.HeightRequest = screenHeight * 0.5;
    }

	private async void CloseModal(object sender, EventArgs e)
	{
		await Navigation.PopModalAsync();
	}
}