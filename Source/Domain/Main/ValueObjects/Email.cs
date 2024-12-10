
namespace TaskListMaui.Source.Domain.Main.ValueObjects
{
    public class Email
    {
        public string Address { get; set; } = string.Empty;
        public Email(string address)
        {
            Address = address;
        }
    }


}
