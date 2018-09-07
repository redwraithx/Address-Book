
namespace AddressBook
{
    class Contact
    {
        public int ID
        {
            get;
            set;
        }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Address { get; set; }

        public Contact()
        {
        }
        
        public Contact(int id, string firstName, string lastName, string address)
        {
            ID = id;
            FirstName = firstName;
            LastName = lastName;
            Address = address;
        }
    }
}
