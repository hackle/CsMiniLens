namespace CharpLensTest {    
    public class Address 
    {
        public string Street { get; set; }
        public int Number { get; set; }
    }
    public class CityAddress : Address {
        public string City;
    }
    public class Person
    {
        public Address Address { get; set; }
        public string Name { get; set; }
    }
}