using System;
using System.Collections.Generic;
using System.Text;

namespace PIIIProject
{
    public class Product
    {
        #region backing fields

        private string _productCode,_name;
        private decimal _price;
        private int _stock;

        #endregion

        #region constructors

        //constructor in case the stock wasn't immediately provided
        public Product(string code, string name, decimal price)
        {
            ProductCode = code;
            Name = name;
            Price = price;
            Stock = 0;
        }

        //construstor for if the stock was provided
        public Product(string code, string name, decimal price, int stock) :this(code, name, price)
        {
            Stock = stock;
        }

        #endregion

        #region properties

        //this has a public getter to access the product code, and a private setter so only methods within the class can set it
        public string ProductCode
        {
            get { return _productCode; }
            private set
            {
                int codeLength = 5;
                //this makes sure the code is the proper format
                if (value.Length > codeLength)
                    throw new ArgumentException("Invalid product code; too long.", "ProductCode");

                _productCode = value;
            }
        }

        //public getter so anyone can access the name, but private setter so only the class can set the name
        public string Name
        {
            get { return _name; }
            private set
            {
                _name = value;
            }
        }

        //public getter so everyone can get the product's price, but only the class can set it
        public decimal Price
        {
            get { return _price; }
            private set
            {
                //making sure the price is positive
                if (value < 0)
                    throw new ArgumentException("Price of product has to be greater than zero.", "Price");

                _price = value;
            }
        }

        // private setter so that outsiders cannot change it, they would have to go through the public add and remove methods
        public int Stock
        {
            get { return _stock; }
            private set
            {
                //making sure the price isn't negative
                if (value < 0)
                    throw new ArgumentException("Stock cannot go below zero.", "Stock");

                _stock = value;
            }
        }

        #endregion

        #region member methods

        // this method takes in how much of a stock they have to add. It adds this number to the stock count and returns the new stock amount.
        public int AddStock(int newStock)
        {
            //making sure the stock isn't going to be lowered
            if (newStock < 0)
                throw new ArgumentException("Cannot add negative stock.", "AddStock");

            Stock = Stock + newStock;

            return Stock;
        }

        //this method takes in how much stock needs to be removed. It removes the amount from the current stock and returns the new stock amount.
        public int RemoveStock(int stockToRemove)
        {
            //making sure the stock won't be increased
            if (stockToRemove < 0)
                throw new ArgumentException("Cannot remove negative stock.", "RemoveStock");

            //making sure the stock doesn't go into negatives
            if (Stock - stockToRemove < 0)
            {
                throw new ArgumentException("Cannot remove more than what's currently in stock.", "RemoveStock");
            }
            else
            {
                Stock = Stock - stockToRemove;
            }

            return Stock;
        }

        #endregion

        #region overriden methods

        //making the output show the product information
        public override string ToString()
        {
            return $"Code: {ProductCode} - Name: {Name} - Price: {Price} - Stock: {Stock}";
        }

        #endregion

    }
}
