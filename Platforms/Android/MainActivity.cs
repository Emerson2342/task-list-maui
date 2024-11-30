using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Content.Res;
using Android.OS;
using AndroidX.AppCompat.App;
using TaskListMaui.Source.Screens.User;

namespace TaskListMaui;

[Activity(
    Theme = "@style/Maui.SplashTheme",
    MainLauncher = true,
    LaunchMode = LaunchMode.SingleTop,
    ConfigurationChanges = ConfigChanges.ScreenSize |
    ConfigChanges.Orientation |
    ConfigChanges.UiMode |
    ConfigChanges.ScreenLayout|
    ConfigChanges.SmallestScreenSize|
     ConfigChanges.KeyboardHidden |
    ConfigChanges.Density)]

[IntentFilter(new string[] { Intent.ActionView },
    Categories = new[] { Intent.CategoryDefault, Intent.CategoryBrowsable },
    DataScheme = "https",
    DataHost = "teste.com",
    DataPath = "/email",
    DataPathPrefix = "/email",
    AutoVerify = true)]
public class MainActivity : MauiAppCompatActivity
{
    protected override void OnCreate(Bundle? savedInstanceState)
    {
        base.OnCreate(savedInstanceState);
        var url = Intent?.DataString;
        if (!string.IsNullOrEmpty(url))
        {
            Microsoft.Maui.Controls.Application.Current?.SendOnAppLinkRequestReceived(new Uri(url));
        }
    }


}
