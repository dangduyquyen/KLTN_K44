﻿namespace SV20T1080048.Web.Models
{
    public class CartItem
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; } = "";
        public string Unit { get; set; } = "";
        public int Quantity { get; set; }
        public string Photo { get; set; }
        public decimal Price { get; set; }
        public decimal Total => Quantity * Price;
    }
}
