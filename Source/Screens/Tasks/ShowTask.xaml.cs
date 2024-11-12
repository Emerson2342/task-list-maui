using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.Marshalling;
using TaskListMaui.Source.Domain.Main.Entities;

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
            Text = description,
            Title = title
        });
    }
}