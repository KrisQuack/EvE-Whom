namespace EveConnectionFinder.Models
{
    public class ESICorp
    {
        public int alliance_id { get; set; }

        public int ceo_id { get; set; }

        public int creator_id { get; set; }

        public string date_founded { get; set; }

        public string description { get; set; }

        public int faction_id { get; set; }

        public int home_station_id { get; set; }

        public int member_count { get; set; }

        public string name { get; set; }

        public long shares { get; set; }

        public decimal tax_rate { get; set; }

        public string ticker { get; set; }

        public string url { get; set; }

        public bool war_eligible { get; set; }
    }
}