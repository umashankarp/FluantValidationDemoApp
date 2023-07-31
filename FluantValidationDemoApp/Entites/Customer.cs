﻿namespace FluantValidationDemoApp.Entites
{
    public class Customer
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int Age { get; set; }
        public string PhoneNumber { get; set; }=string.Empty;
        public bool IsAdult { get; set; }       
        public Address? Address { get; set; }
    }
}
