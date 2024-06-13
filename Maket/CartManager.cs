using System.Collections.Generic;
using System.Linq;
using Maket.Models;

namespace Maket
{
    public class CartManager
    {
        private List<CartItem> cartItems;

        public CartManager()
        {
            cartItems = new List<CartItem>();
        }

        public void AddToCart(CartItem item)
        {
            cartItems.Add(item);
        }

        public void RemoveFromCart(CartItem item)
        {
            cartItems.Remove(item);
        }

        public decimal GetTotalCost()
        {
            return cartItems.Sum(item => item.ItemPrice * item.Quantity);
        }

        public List<CartItem> GetCartItems()
        {
            return cartItems;
        }

        public List<CartItem> GetCartItemsByClientId(int clientId)
        {
            return cartItems.Where(item => item.ClientID == clientId).ToList();
        }

        public void CompletePurchase(int clientId)
        {
            // Logic to complete the purchase for the specific client
            cartItems.RemoveAll(item => item.ClientID == clientId);
        }

        // Добавлено
        public void AddFoodOrDrinkToCart(CartItem cartItem)
        {
            cartItems.Add(cartItem);
        }
    }
}
