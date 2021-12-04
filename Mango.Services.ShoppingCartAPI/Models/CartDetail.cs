﻿namespace Mango.Services.ShoppingCartAPI.Models;

using System.ComponentModel.DataAnnotations.Schema;

public class CartDetail
{
    public int CartDetailsId { get; set; }
    public int CartHeaderId { get; set; }
    [ForeignKey("CartHeaderId")]
    public virtual CartHeader CartHeader { get; set; }
    public int ProductId { get; set; }
    [ForeignKey("ProductId")]
    public virtual Product Product { get; set; }
    public int Count { get; set; }
}