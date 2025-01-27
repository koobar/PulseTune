namespace PulseTune
{
    internal struct DateOnly
    {
        public uint Year;
        public uint Month;
        public uint Day;

        public DateOnly(uint year, uint month, uint day)
        {
            this.Year = year;
            this.Month = month;
            this.Day = day;
        }

        public override string ToString()
        {
            return $"{this.Year}/{this.Month.ToString("00")}/{this.Day.ToString("00")}";
        }
    }
}
