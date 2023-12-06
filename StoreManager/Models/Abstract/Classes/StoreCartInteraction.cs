using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoreManager.Models.Abstract.Classes
{
    public abstract class StoreCartInteraction
    {
        private int orderNumber { get; set; }
        public struct OrderItem
        {
            public int orderNumber { get; set; }
            public int productId { get; set; }
            public string productName { get; set; }
            public string categoryName { get; set; }
            public int price { get; set; }
            public int quantity { get; set; }
        }
        public bool isCreating = false;
        private List<OrderItem> _orderItems = new List<OrderItem>();
        public List<OrderItem> orderItems 
        {
            get { return _orderItems; }
            private set { _orderItems = value; }
        }
        private ClientStoreInteraction client;
        protected StoreCartInteraction(ClientStoreInteraction client)
        {
            this.client = client;
            this.orderNumber = client.GetNewRandomOrderNumber();
        }

        public void CreateNewCart()
        {
            this.orderNumber = client.GetNewRandomOrderNumber();
            orderItems = new List<OrderItem>();
            this.isCreating = false;
        }

        public void AddOrUpdateItem(int productId, string productName, int price, string categoryName, int quantity)
        {
            if (orderItems.Where(item => item.productId == productId).Count() < 1)
            {
                AddItem(productId, productName, categoryName, price, quantity);
            }
            else
            {
                UpdateItem(productId, quantity);
            }
        }

        private void AddItem(int productId, string productName, string categoryName, int price, int quantity)
        {
            orderItems.Add(new OrderItem
            {
                orderNumber = this.orderNumber,
                productId = productId,
                productName = productName,
                categoryName = categoryName,
                price = price,
                quantity = quantity
            });
        }

        private void UpdateItem(int productId, int quantity)
        {
            OrderItem itemToUpdate = orderItems.Find(item => item.productId == productId);
            orderItems.Remove(itemToUpdate);
            itemToUpdate.quantity += quantity;
            if (itemToUpdate.quantity <= 0)
                return;
            orderItems.Add(itemToUpdate);
        }

        public void RemoveItem(int productId)
        {
            OrderItem itemToRemove = orderItems.Find(item => item.productId == productId);
            if (!itemToRemove.Equals(null))
            {
                orderItems.Remove(itemToRemove);
            }
        }
        public int GetTotalPrice()
        {
            int totalPrice = 0;
            foreach(var item in this.orderItems)
            {
                totalPrice += item.price * item.quantity;
            }
            return totalPrice;
        }
        public void CreateOrder(bool isBankCard, string BankCardNumber)
        {
            client.CreateOrder(orderNumber, orderItems, GetTotalPrice(), isBankCard, BankCardNumber);
            isCreating = true;
        }
    }
}
