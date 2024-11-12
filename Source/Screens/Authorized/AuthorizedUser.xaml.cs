using TaskListMaui.Source.Screens.Home;

namespace TaskListMaui.Source.Screens.Authorized;

public partial class AuthorizedUser : ContentPage
{
	public AuthorizedUser()
	{
		InitializeComponent();
	}

    private void Button_Clicked(object sender, EventArgs e)
    {
				
		Navigation.PushModalAsync(new LoginPage());
        //await Navigation.PopModalAsync();
    }
}