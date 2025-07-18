namespace HOMMS.Domain.Dtos
{
    public class RevenueDto
    {
        public int Order { get; set; }
        public int? Total { get; set; }
        public int? QuantityFood { get; set; }
        public List<ChartOrderDto> ChartOrders { get; set; }

    }

    public class ChartOrderDto
    {


        public string  Date { get; set; }
        public int? OrderCount { get; set; }
        public int? TotalAmount { get; set; }
        public int? QuantityFood { get; set; }


    }
}
