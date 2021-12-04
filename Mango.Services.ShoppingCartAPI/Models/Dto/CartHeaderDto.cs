namespace Mango.Services.ShoppingCartAPI.Models.Dto;

using System.ComponentModel.DataAnnotations;

public class CartHeaderDto
{
    [Key]
    public int CartHeaderId { get; set; }
    public string UserId { get; set; }
    public string CouponCode { get; set; }
}