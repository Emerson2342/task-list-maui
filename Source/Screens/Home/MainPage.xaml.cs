namespace TaskListMaui.Source.Screens.Home;

public partial class MainPage : ContentPage
{

    public MainPage()
    {
        //InitializeComponent();
        InitializeComponent();
    }

    private void HandlerPassword(object sender, EventArgs e)
    {
        DisplayAlert("Mensage", "Botão foi clicado", "Fechar!");
    }
}

