namespace tester.Models
{
    public class ProgModel
    {
        public double height { get; set; }
        public double width { get; set; }
        public string color { get; set; }
        public string emptyarguments { get; set; }
        public List<points> inputs { get; set; }
        public List<items> selectableitems { get; set; }
        public bool istextinput { get; set; } = false;
        public List<points> output { get; set; }
    }

    public class points
    {
        public int argumentindex { get; set; }
        public position position { get; set; }
    }

    public class position
    {
        public double x { get; set; }
        public double y { get; set; }
    }

    public class items
    {
        public List<string> type { get; set; }
    }
}
