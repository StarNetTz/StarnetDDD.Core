namespace $safeprojectname$.Events
{
    public class OrganizationRegistered : Event
    {
        public string Name { get; set; }
        public Address Address { get; set; }
    }
}