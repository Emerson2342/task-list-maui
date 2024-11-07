using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using TaskListMaui.Source.Domain.Main.Entities;
using TaskListMaui.Source.Domain.Main.UseCase.ResponseCase;
using TaskListMaui.Source.Screens.Home;
using TaskListMaui.Source.Screens.User;

namespace TaskListMaui.Source.Screens.Tasks;

public partial class TaskList : ContentPage, INotifyPropertyChanged
{
    private readonly string _token;
    private readonly string _userId;
    private TaskEntity? _selectedTask;


    private bool isSwapping = false;


    private readonly string Ip = Configuration.IpAddress;

    public ObservableCollection<TaskEntity> _tasksList { get; set; } = new();
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
    public TaskList(string token, string userId)
    {
        InitializeComponent();


        _token = token;
        _userId = userId;

        BindingContext = this;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        HttpClientHandler handler = new()
        {
            ServerCertificateCustomValidationCallback = (message, cert, chain, sslPolicyErrors) => true
        };
        HttpClient http = new(handler);

        http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _token);

        var request = await http.GetAsync($"https://{Ip}/task/list/{_userId}");

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

        isSwapping = true;
    }

    private void EndSwipe(object sender, EventArgs e)
    {
        isSwapping = false;
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

       // if (!isSwapping)
            //ShowSelectedTask();
        //await Task.Delay(500);
       // isSwapping = false;
    }

    protected override void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    private async void Back_Clicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new LoginPage());
    }

    private async void ModalAddTask(object sender, EventArgs e)
    {
        await Navigation.PushModalAsync(new NewTask(_userId, _token));
    }

    private async void Delete_Clicked(object sender, EventArgs e)
    {
        if (_selectedTask == null)
        {
            await DisplayAlert("Aten��o", "Favor selecionar uma tarefa", "Fechar");
            return;
        }

        await Navigation.PushModalAsync(new DeleteTask(_token, _selectedTask.Id, _selectedTask.Title));
    }

    private async void Edit_Clicked(object sender, EventArgs e)
    {
        if (_selectedTask == null)
        {
            await DisplayAlert("Aten��o", "Favor selecionar uma tarefa", "Fechar");
            return;
        }

        await Navigation.PushModalAsync(new EditTask(_token, _selectedTask.Id.ToString()));

    }

    private async void ChangePassword_Clicked(object sender, EventArgs e)
    {
        await Navigation.PushModalAsync(new ChangePassword(_token));
    }
}