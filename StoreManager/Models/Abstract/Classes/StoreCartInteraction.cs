using StoreManager.DB_classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoreManager.Models.Abstract.Classes
{
    public abstract class StoreCartInteraction
    {
        public int orderNumber { get; set; }
        public struct OrderItem
        {
            public Product product { get; set; }
            public int quantity { get; set; }
        }

        public bool isCreating = false;
        private List<OrderItem> _orderItems = new List<OrderItem>();
        public List<OrderItem> orderItems 
        {
            get { return _orderItems; }
            private set { _orderItems = value; }
        }
        public AllUsersInteractions client;
        protected StoreCartInteraction(AllUsersInteractions client)
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

        public void AddOrUpdateItem(Product product, int quantity)
        {
            if (orderItems.Where(item => item.product.ProductID == product.ProductID).Count() < 1)
            {
                AddItem(product, quantity);
            }
            else
            {
                UpdateItem(product, quantity);
            }
        }

        private void AddItem(Product product, int quantity)
        {
            orderItems.Add(new OrderItem
            {
                product = product,
                quantity = quantity
            });
        }

        private void UpdateItem(Product product, int quantity)
        {
            OrderItem itemToUpdate = orderItems.Find(item => item.product.ProductID == product.ProductID);
            orderItems.Remove(itemToUpdate);
            itemToUpdate.quantity += quantity;
            if (itemToUpdate.quantity <= 0)
                return;
            orderItems.Add(itemToUpdate);
        }

        public void RemoveItem(Product product)
        {
            OrderItem itemToRemove = orderItems.Find(item => item.product.ProductID == product.ProductID);
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
                totalPrice += item.product.Price * item.quantity;
            }
            return totalPrice;
        }
        public bool CreateOrder(bool isBankCard, string BankCardNumber)
        {
            isCreating = client.CreateOrder(orderNumber, orderItems, GetTotalPrice(), isBankCard, BankCardNumber);
            return isCreating;
        }
    }
}
