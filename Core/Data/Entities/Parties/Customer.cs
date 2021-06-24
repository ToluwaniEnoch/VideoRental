namespace Api.Data.Entities.Parties
{
    public record Customer(string TIN, string RcNumber, string AccountNumber, string Email, string PhoneNumber, string CompanyName, string Address) : BaseEntity();
    

}
