namespace Mango.Services.ShoppingCartAPI.Models;

using System.ComponentModel.DataAnnotations;

public class CartHeader
{
    [Key]
    public int CartHeaderId { get; set; }
    public string UserId { get; set; }
    public string CouponCode { get; set; }
}