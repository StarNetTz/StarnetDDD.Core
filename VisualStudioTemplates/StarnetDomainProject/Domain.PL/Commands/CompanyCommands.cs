namespace $safeprojectname$.Commands
{
    public class RegisterCompany : Command
    {
        public string Name { get; set; }
        public Address Address { get; set; }
        public string VATId { get; set; }
    }
}