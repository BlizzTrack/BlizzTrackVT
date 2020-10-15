namespace BlizzTrackVT.Models
{
    public class PartialConfigDataModel<T>
    {
        public T Current { get; set; }
        public T Previous { get; set; }
    }
}
