﻿namespace Mango.Services.ShoppingCartAPI.Models.Dto;

using System.ComponentModel.DataAnnotations.Schema;

public class CartDetailsDto
{
    public int CartDetailsId { get; set; }
    public int CartHeaderId { get; set; }
    public virtual CartHeader CartHeader { get; set; }
    public int ProductId { get; set; }
    public virtual Product Product { get; set; }
    public int Count { get; set; }
}