namespace FluantValidationDemoApp.Entites
{
    public class Address
    {
        public int Id { get; set; }
        public string Line1 { get; set; } = string.Empty;
        public string Line2 { get; set; } =  string.Empty;
        public string Town { get; set; } = string.Empty;
        public string Postcode { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;
        public string State { get; set; } = string.Empty;
    }
}
