﻿using StoreManager.Abstract.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoreManager.Client.Cart
{
    internal class MyCart : StoreCartInteraction
    {
        public MyCart(ClientStoreInteraction client) : base(client) { }
    }
}