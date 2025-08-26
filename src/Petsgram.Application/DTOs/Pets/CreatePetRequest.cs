namespace Petsgram.Application.DTOs.Pets;

public class CreatePetRequest
{
    public string PetName { get; set; } = string.Empty;
    public string PetType { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string CurrencyCode { get; set; } = "USD";
}