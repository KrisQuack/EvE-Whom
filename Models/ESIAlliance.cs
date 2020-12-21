namespace EveConnectionFinder.Models
{
    public class ESIAlliance
    {
        public int creator_corporation_id { get; set; }

        public int creator_id { get; set; }

        public string date_founded { get; set; }

        public int executor_corporation_id { get; set; }

        public int faction_id { get; set; }

        public string name { get; set; }

        public string ticker { get; set; }
    }
}