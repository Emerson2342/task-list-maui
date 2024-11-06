using System.Net.Http.Headers;

namespace TaskListMaui.Source.Screens.Tasks;

public partial class EditTask : ContentPage
{
    private readonly string _token;
    private readonly string _taskId;


	public EditTask(string token, string taskId)
	{
        _token = token;
        _taskId = taskId;

		InitializeComponent();
	}

    private async void Back_Clicked(object sender, EventArgs e)
    {
		await Navigation.PopModalAsync();
    }

   
}