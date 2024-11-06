using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using TaskListMaui.Source.Domain.Main.Entities;
using TaskListMaui.Source.Domain.Main.UseCase.ResponseCase;
using TaskListMaui.Source.Screens.Home;

namespace TaskListMaui.Source.Screens.Tasks;

public partial class TaskList : ContentPage, INotifyPropertyChanged
{
    private readonly string _token;
    private readonly string _userId;
    private readonly string _idTask;

    private string message = string.Empty;

    public ObservableCollection<TaskEntity> _tasksList { get; set; } =new();
    public ObservableCollection<TaskEntity> TasksList { 
        get => _tasksList;
        set
        {
            _tasksList = value;
            OnPropertyChanged();
        } 
    }
    
    public event PropertyChangedEventHandler PropertyChanged;
        public TaskList(string token, string userId)
	{
        InitializeComponent();

        _token = token;
        _userId = userId;
        _idTask = IdTask.Text;

        BindingContext = this;        
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        HttpClientHandler handler = new ()
        {
            ServerCertificateCustomValidationCallback = (message, cert, chain, sslPolicyErrors) => true
        };
        HttpClient http = new(handler);

        http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _token);

        var request = await http.GetAsync($"https://192.168.10.10:7103/task/list/{_userId}");

        if (request.IsSuccessStatusCode) { 
        var result = await request.Content.ReadFromJsonAsync<Response>();
            if (result == null)
            {
                message = "Erro ao ler os dados";
                return;
            }

            if (result.TaskList == null)
                return;



            TasksList = new ObservableCollection<TaskEntity>(result.TaskList);
            message = result.Message;

        }

     }
    protected override void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    private async void Back_Clicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new MainPage());
    }

    private async void ModalAddTask(object sender, EventArgs e)
    {
        await Navigation.PushModalAsync(new NewTask(_userId, _token));
    }

    private async void Delete_Clicked(object sender, EventArgs e)
    {
        await Navigation.PushModalAsync(new DeleteTask(_token, _idTask));
    }
}