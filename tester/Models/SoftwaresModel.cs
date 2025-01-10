namespace tester.Models
{
    public class SoftwaresModel
    {
        public string sprache { get; set; }
        public List<Pakete> inhalt { get; set; }
    }

    public class Pakete
    {
        public string name { get; set; }
        public string path { get; set; }
    }
}
