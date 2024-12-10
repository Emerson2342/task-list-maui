
using TaskListMaui.Source.Screens.User;

namespace TaskListMaui;

public partial class App : Application
{
    public App()
    {
        InitializeComponent();

        MainPage = new AppShell();
    }

    protected override async void OnAppLinkRequestReceived(Uri uri)
    {
        base.OnAppLinkRequestReceived(uri);

        // Show an alert to test that the app link was received.
        await Dispatcher.DispatchAsync(async () =>
        {
            await Windows[0].Page!.Navigation.PushModalAsync(new ConfirmEmail());
            
        });
    }
}
