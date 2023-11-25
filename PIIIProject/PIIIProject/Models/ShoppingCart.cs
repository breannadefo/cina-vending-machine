using System;
using System.Collections.Generic;
using System.Text;

namespace PIIIProject.Models
{
    public class ShoppingCart
    {
        #region backing fields

        private const int MINIMUM = 0;
        
        private List<Product> _products;    //this is so that we have access to the products
        private List<int> _quantities = new List<int>();      //this is to keep track of how many of each product are being bought

        #endregion

        #region constructors

        //need a list of products to create a shopping cart
        public ShoppingCart(List<Product> products)
        {
            Products = products;
            InitializeQuantities(); //this is what sets the quantities list
        }

        #endregion

        #region properties

        //anyone can get the product list, but only class methods can set the list
        public List<Product> Products
        {
            get { return _products; }
            private set { _products = value; }
        }


        //only a getter since the setting happens in a method
        public List<int> Quantities
        {
            get { return _quantities; }
        }

        #endregion

        #region member methods

        //this initializes the quantity list and sets the quantity value for each product to zero
        private void InitializeQuantities()
        {
            foreach (Product item in _products)
            {
                //sets the quantity values to zero for each product
                _quantities.Add(MINIMUM);
            }
        }

        //this increases an item's quantity based on the product code that was sent in
        public void AddItemToCart(string productCode, int quantity)
        {
            bool doesntExist = true;

            //makes sure the quantity is a positive number
            if (quantity < MINIMUM)
                throw new ArgumentException("Cannot add a negative quantity.", "AddItemToCart");
            
            
            for (int i = 0; i < _products.Count; i++)
            {
                //this is here to make sure the product code is a valid code
                if (productCode == _products[i].ProductCode)
                {
                    doesntExist = false;

                    //checks that the quantity of the item they want to buy isn't more than what is in stock for that item
                    if ((_quantities[i] + quantity) > _products[i].Stock)
                    {
                        throw new ArgumentException($"Cannot buy an amount greater than the item's current stock ({_products[i].Stock}).", "AddItemToCart");
                    }
                    else
                    {
                        //adds the quantity to the current quantity for this product
                        _quantities[i] += quantity;

                        //lowers the item's stock so that the stock values stay correct
                        _products[i].RemoveStock(quantity);
                    }
                }
            }

            if (doesntExist)
                throw new ArgumentException("Product code does not match an existing product.", "AddItemToCart");
                        
        }

        //this decreases an item's quantity based on the product code that was sent in
        public void RemoveItemFromCart(string productCode, int quantity)
        {
            bool doesntExist = true;

            //makes sure the quantity is a positive number
            if (quantity < MINIMUM)
                throw new ArgumentException("Cannot remove a negative quantity.", "RemoveItemFromCart");


            for (int i = 0; i < _products.Count; i++)
            {
                //this is here to make sure the product code is a valid code
                if (productCode == _products[i].ProductCode)
                {
                    doesntExist = false;

                    //makes sure they aren't trying to remove more than what they have in their cart
                    if ((_quantities[i] - quantity) < MINIMUM)
                    {
                        throw new ArgumentException("Can't remove more than what is in the cart.", "RemoveItemFromCart");
                    }
                    else
                    {
                        //adds the quantity to the current quantity for this product
                        _quantities[i] -= quantity;

                        //adds back the stock that was removed when the item was placed in the cart, to make sure the stock value stays correct
                        _products[i].AddStock(quantity);
                    }
                }
            }

            if (doesntExist)
                throw new ArgumentException("Product code does not match an existing product.", "AddItemToCart");

        }

        //this method returns the price of a specific item based on how many of that item are in the cart.
        //it either returns the cost for all of that item, or -1 if the item doesn't exist
        public decimal ItemTotalPrice(string productCode)
        {            
            for (int i = 0; i < _products.Count; i++)
            {
                //this finds the product to get the price of the item
                if (productCode == _products[i].ProductCode)
                {
                    //calculates how much all of a product in the cart will cost
                    return _quantities[i] * _products[i].Price;
                }
            }

            return -1;
        }

        //this method calculates and returns the total price of all the items in the cart
        public decimal TotalPrice()
        {
            decimal totalPrice = MINIMUM;
            int index = MINIMUM;

            //goes through each item and calculates the price of those items and adds it to the final price
            foreach (Product item in _products)
            {
                totalPrice += item.Price * _quantities[index];
                index++;
            }

            return totalPrice;
        }

        //the products and product codes would be the same no matter what cart there is, so we just have to reset
        //the _quantities list back to having zero for eveything
        public void ResetCart()
        {
            //goes through every inex of the list and sets it to zero
            for (int i = 0; i < _quantities.Count; i++)
                _quantities[i] = MINIMUM;
        }

        #endregion

        #region overriden methods

        //this adds all the products that have at least one item in the cart to a string
        // it structures the information as quantity, product name, total price for that item
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            int index = 0;

            foreach (Product item in _products)
            {
                //only adds the products that are in the cart
                if (_quantities[index] > 0)
                {
                    sb.AppendLine($"{_quantities[index]} {item.Name} --- {ItemTotalPrice(item.ProductCode)}");
                }

                index++;
            }

            return sb.ToString();
        }

        #endregion

    }
}
