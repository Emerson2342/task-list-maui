using Microsoft.Extensions.Logging;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Xml.XPath;
using TaskListMaui.Source.Domain.Main.Entities;
using TaskListMaui.Source.Domain.Main.Services;
using TaskListMaui.Source.Domain.Main.UseCase.ResponseCase;
using TaskListMaui.Source.Screens.Home;
using TaskListMaui.Source.Screens.User;

namespace TaskListMaui.Source.Screens.Tasks;

public partial class TaskList : ContentPage, INotifyPropertyChanged
{
    private TaskEntity? _selectedTask;
    private bool _isSwapping = false;

    private ObservableCollection<TaskEntity> _tasksList { get; set; } = new();

    public ObservableCollection<TaskEntity> TasksList
    {
        get => _tasksList;
        set
        {
            _tasksList = value;
            OnPropertyChanged();
        }
    }
    public new event PropertyChangedEventHandler? PropertyChanged;

    public TaskList()
    {
  
        InitializeComponent();
        BindingContext = this;
       
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

        if (string.IsNullOrEmpty(AuthenticationService.Token))
        {
            Navigation.PopModalAsync();
            return;
        }
        
        LoadTasks();       

    }

    private async void LoadTasks()
    {
       
        HttpClientHandler handler = new()
        {
            ServerCertificateCustomValidationCallback = (message, cert, chain, sslPolicyErrors) => true
        };
        HttpClient http = new(handler);

        http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", await AuthenticationService.GetToken());

        var request = await http.GetAsync($"https://{Configuration.IpAddress}/task/list/{await AuthenticationService.GetUserId()}");

        if (request.IsSuccessStatusCode)
        {
            var result = await request.Content.ReadFromJsonAsync<Response>();
            if (result == null)
            {
                await DisplayAlert("Erro", "Erro ao ler dos dados", "Fechar");
                return;
            }
            TasksList = new ObservableCollection<TaskEntity>(result.TaskList);

            if (result.TaskList == null)
                await DisplayAlert("Erro", "Erro ao ler dos dados", "Fechar");
            return;
        }
    }

    private void StartSwipe(object sender, EventArgs e)
    {
        _isSwapping = true;
    }

    private void EndSwipe(object sender, EventArgs e)
    {
        _isSwapping = false;
    }

    private void SelectTask(object sender, SelectionChangedEventArgs e)
    {
        var task = e.CurrentSelection[0] as TaskEntity;

        if (task == null)
        {
            DisplayAlert("Erro", "Erro ao selecionar tarefa", "Fechar");
            return;
        }

        _selectedTask = task;

        if (!_isSwapping)
            ShowTask_Clicked(sender, e);
          _isSwapping = false;
    }

    protected override void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    private async void Back_Clicked(object sender, EventArgs e)
    {
        AuthenticationService.RemoveToken();
        await Navigation.PopModalAsync();
    }

    private async void ModalAddTask(object sender, EventArgs e)
    {
        System.Diagnostics.Debug.Assert(await AuthenticationService.GetToken() != null);

        await Navigation.PushModalAsync(new NewTask(await AuthenticationService.GetUserId(),
            await AuthenticationService.GetToken()));
    }

    private async void Delete_Clicked(object sender, EventArgs e)
    {
        if (_selectedTask == null)
        {
            await DisplayAlert("Atenção", "Favor selecionar uma tarefa", "Fechar");
            return;
        }

        await Navigation.PushModalAsync(new DeleteTask(await AuthenticationService.GetToken(), _selectedTask.Id, _selectedTask.Title));
    }

    private async void ShowTask_Clicked(object sender, EventArgs e)
    {
        await Navigation.PushModalAsync(new ShowTask(_selectedTask));
    }
    private async void Edit_Clicked(object sender, EventArgs e)
    {
        if (_selectedTask == null)
        {
            await DisplayAlert("Atenção", "Favor selecionar uma tarefa", "Fechar");
            return;
        }
        await Navigation.PushModalAsync(new EditTask(await AuthenticationService.GetToken(), _selectedTask.Id.ToString()));

    }

    private async void ChangePassword_Clicked(object sender, EventArgs e)
    {
        await Navigation.PushModalAsync(new ChangePassword(await AuthenticationService.GetToken()));
    }
}