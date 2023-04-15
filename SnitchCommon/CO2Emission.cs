namespace SnitchCommon
{
    public class CO2Emission
    {
        public double Total { get; set; }
        public double Concrete { get; set; }
        public double Steel { get; set; }

        public void CollectCO2(CO2Emission other)
        {
            Total += other.Total;
            Concrete += other.Concrete;
            Steel += other.Steel;
        }
    }
}
