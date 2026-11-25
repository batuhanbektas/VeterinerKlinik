namespace VetClinic.Models;

// rol adlarını her yerde elle string yazmamak için sabitler
// [Authorize(Roles = Roles.Admin)] şeklinde kullanacağım
public static class Roles
{
    public const string Admin = "Admin";
    public const string Veteriner = "Veteriner";
    public const string Uye = "Uye"; // türkçe karakter koymadım, rol adında sorun çıkmasın

    public static readonly string[] All = { Admin, Veteriner, Uye };
}
