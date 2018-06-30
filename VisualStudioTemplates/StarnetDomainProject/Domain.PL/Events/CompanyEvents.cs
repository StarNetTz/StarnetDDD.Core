namespace $safeprojectname$.Events
{
    public class CompanyRegistered : Event
    {
        public string Name { get; set; }
        public Address Address { get; set; }
        public string VATId { get; set; }
    }
}
