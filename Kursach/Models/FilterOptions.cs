namespace Kursach.Models
{
    public class FilterOptions
    {
        public int[] ChosenTypes { get; set; }
        public int Action { get; set; }
        public int RoomsNum { get; set; }
        public string Address { get; set; }
        public FilterOptions() { }

        public FilterOptions(int[] chosenTypes, int action, int roomsNum, string address)
        {
            ChosenTypes = chosenTypes;
            Action = action;
            RoomsNum = roomsNum;
            Address = address;
        }
    }
}
