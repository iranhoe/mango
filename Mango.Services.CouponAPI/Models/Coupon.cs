namespace Mango.Services.CouponAPI.Models;

using System.ComponentModel.DataAnnotations;

public class Coupon
{
    [Key] 
    public int CouponId { get; set; }
    public string CouponCode { get; set; }
    public double DiscountAmount { get; set; }
}