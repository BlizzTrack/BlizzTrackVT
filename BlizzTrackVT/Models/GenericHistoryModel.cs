namespace BlizzTrackVT.Models
{
    public class GenericHistoryModel<T>
    {
        public T Current { get; set; }
        public T Previous { get; set; }
        public T Latest { get; set; }
    }
}
