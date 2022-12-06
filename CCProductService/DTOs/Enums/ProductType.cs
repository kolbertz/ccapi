using Swashbuckle.AspNetCore.Annotations;

namespace CCProductService.DTOs.Enums
{
    [SwaggerSchema(Description = "0 = DefaultProduct / null / Standard Artikel\n1 = MenuProduct / Speiseplan Artikel mit täglich wechselnden Texten und Bildern\n2 = Deposit / Pfand\n3 = Discount / Subventionsartikel")]
    public enum ProductType
    {
        DefaultProduct = 0x00,

        MenuProduct = 0x01,

        Deposit = 0x02,

        Discount = 0x03
    }
}
