namespace StorageApi.DTOs
{
    public class StatsDto
    {
        public int TotalProducts { get; set; }
        public long TotalInventoryValue { get; set; }
        public double AveragePrice { get; set; }
    }
}