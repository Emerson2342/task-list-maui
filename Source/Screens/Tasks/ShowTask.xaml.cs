using System.Net.Http.Headers;
using TaskListMaui.Source.Domain.Main.Entities;
using TaskListMaui.Source.Domain.Main.Services;

namespace TaskListMaui.Source.Screens.Tasks;

public partial class ShowTask : ContentPage
{
   
    private readonly TaskEntity _selectedTask;

    public ShowTask(TaskEntity selectedTask)
	{
		InitializeComponent();
		_selectedTask = selectedTask;

        AbTitle.Text = _selectedTask.Title;
		AbDescription.Text = _selectedTask.Description;
		AbStartTime.Text = _selectedTask.StartTime.ToShortDateString();
		AbEndTime.Text = _selectedTask.Deadline.ToShortDateString();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        LoadImage();

    }

    private async void LoadImage()
    {

        HttpClientHandler handler = new()
        {
            ServerCertificateCustomValidationCallback = (message, cert, chain, sslPolicyErrors) => true
        };
        HttpClient http = new(handler);

        http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", await AuthenticationService.GetToken());

        var response = await http.GetAsync($"https://{Configuration.IpAddress}/task/get-photo/{_selectedTask.Id}");

        var image = await response.Content.ReadAsByteArrayAsync();

        var stream = new MemoryStream(image);      
        AbPhotoTask.Source = ImageSource.FromStream(() => stream);
       
    }



    private async void Back_Clicked(object sender, EventArgs e)
	{
		await Navigation.PopModalAsync();
	}

	private async void Share_Clicked(object sender, EventArgs e)
	{
		await ShareText(_selectedTask.Title, _selectedTask.Description);

	}

	public static async Task ShareText(string title, string description)
	{
		await Share.Default.RequestAsync(new ShareTextRequest
		{
			Title = "Compartilhar Tarefa",
			Text = title + " " + description
		});
	}
}