using TaskListMaui.Source.Screens.Home;

namespace TaskListMaui.Source.Screens.User;

public partial class ConfirmEmail : ContentPage
{
	public ConfirmEmail()
	{
		InitializeComponent();

	}

    private async void Button_Clicked(object sender, EventArgs e)
    {
        await Navigation.PopModalAsync();

    }

}