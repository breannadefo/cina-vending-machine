using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;
using PIIIProject.Models;

namespace PIIIProject
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        #region global and constant variables

        //global variables
        private static List<Product> _products = new List<Product>();
        private static List<int> _sliderValues = new List<int>();
        private static ShoppingCart _cart;
        
        //constants
        const decimal DEFAULT_COST = 2.99M;
        const int DEFAULT_SLIDER_VALUE = 0;
        const int PRODUCT_CODE_LENGTH = 5;
        const string STOCK_FILE_PATH = "../../../DataFiles/StockData.txt";

        #endregion

        #region window set up

        public MainWindow()
        {
            //gets the products from the file/default values
            PopulateProducts();

            //initializes the window
            InitializeComponent();
            
            //sets up the sliders
            ResetSliders();

            //creates the shopping cart
            _cart = new ShoppingCart(_products);
            
        }

        #endregion

        #region reading from file

        //this method reads a data file and populates the list of products based on the information in the file
        //if the file doesn't exists, it populates the list with products that are all out of stock
        public static void PopulateProducts()
        {
            bool problemWithFile = false;

            //checks if the stock data file exists
            if (File.Exists(STOCK_FILE_PATH))
            {
                StreamReader stream = null;
                string line, productCode, name;
                int count = 0, index, stock;
                decimal price;
                string[] productParts;

                //try catch so the program doesn't crash if something goes wrong while reading from the file
                try
                {
                    //opens a link to the file
                    stream = new StreamReader(STOCK_FILE_PATH);

                    //goes through every line of the file
                    while ((line = stream.ReadLine()) != null)
                    {
                        //checks which line it is at
                        if (count == 0)
                        {
                            //the first line is just setting up the order of the data
                            //nothing actually happens with this line
                            count++;
                        }
                        else
                        {
                            //separating the file line
                            index = 0;
                            productParts = line.Split(',');

                            //getting the product code
                            productCode = productParts[index];
                            index++;

                            //getting the product name
                            name = productParts[index];
                            index++;

                            //making sure the price is a valid number
                            if (!decimal.TryParse(productParts[index], out price))
                            {
                                //sets the price to a default price if file price was invalid
                                price = DEFAULT_COST;
                            }
                            index++;

                            //making sure the stock is a valid number
                            if (int.TryParse(productParts[index], out stock) && stock >= 0)
                            {
                                //adds stock right away if stock is valid
                                _products.Add(new Product(productCode, name, price, stock));
                            }
                            else
                            {
                                //creates object with default stock from the class
                                _products.Add(new Product(productCode, name, price));
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    //the problem pops up to show the user
                    MessageBox.Show(e.Message, "Problem", MessageBoxButton.OK, MessageBoxImage.Error);

                    //lets method know the product values will have to be set to the default values
                    problemWithFile = true;
                }
                finally
                {
                    //closes the stream if it is open
                    if (stream != null)
                        stream.Close();
                }
            }
            else
            {
                //lets method know the product values will have to be set to the default values
                problemWithFile = true;
            }

            if (problemWithFile)
            {
                //creates a new list to make sure there is no product duplication
                _products = new List<Product>();
                
                //adds the products with no stock if the file doesn't exist or there was a problem while reading it
                _products.Add(new Product("DRK01", "Water", 1.99M));
                _products.Add(new Product("DRK02", "Coca Cola", 3.99M));
                _products.Add(new Product("DRK03", "Ice Tea", 2.99M));
                _products.Add(new Product("CHP01", "Takis", 3.99M));
                _products.Add(new Product("CHP02", "Doritos Classic", 3.99M));
                _products.Add(new Product("CHP01", "Lays Classic", 1.99M));
                _products.Add(new Product("CHO01", "KitKat", 1.99M));
                _products.Add(new Product("CHO02", "Twixx", 1.99M));
                _products.Add(new Product("CHO03", "Kinder Buano", 1.99M));
                _products.Add(new Product("BAR01", "Natural Valley", 0.99M));
                _products.Add(new Product("BAR02", "Nutri Grain", 1.45M));
                _products.Add(new Product("BAR03", "Kashi", 0.99M));
            }
        }

        #endregion

        #region sliders

        //this sets/resets all the slider values and the values saved in the list to their default
        private void ResetSliders()
        {
            //checks if the slider values list has already been initialized or not
            if (_sliderValues.Count < _products.Count)
            {
                //initializes the slider values list and sets all the values to the default value
                for (int i = 0; i < _products.Count; i++)
                {
                    _sliderValues.Add(DEFAULT_SLIDER_VALUE);

                    //this calls a method to display the slider value on the screen
                    ShowSliderValues(_sliderValues[i], _products[i].ProductCode);
                }
                    
            }
            else
            {
                //sets all the slider values to the default since the list is already initialized
                for (int j = 0; j < _sliderValues.Count; j++)
                {
                    _sliderValues[j] = DEFAULT_SLIDER_VALUE;

                    //this calls a method to display the slider value on the screen
                    ShowSliderValues(_sliderValues[j], _products[j].ProductCode);
                }

            }

            //resetting all the slider values
            DRK01Sld.Value = DEFAULT_SLIDER_VALUE;
            DRK02Sld.Value = DEFAULT_SLIDER_VALUE;
            DRK03Sld.Value = DEFAULT_SLIDER_VALUE;
            CHP01Sld.Value = DEFAULT_SLIDER_VALUE;
            CHP02Sld.Value = DEFAULT_SLIDER_VALUE;
            CHP03Sld.Value = DEFAULT_SLIDER_VALUE;
            CHO01Sld.Value = DEFAULT_SLIDER_VALUE;
            CHO02Sld.Value = DEFAULT_SLIDER_VALUE;
            CHO03Sld.Value = DEFAULT_SLIDER_VALUE;
            BAR01Sld.Value = DEFAULT_SLIDER_VALUE;
            BAR02Sld.Value = DEFAULT_SLIDER_VALUE;
            BAR03Sld.Value = DEFAULT_SLIDER_VALUE;

            //this calls a method that will change the background of an item depending on its stock
            CheckForNoStock();
        }

        //this method updates the slider list so that the current value for each slider is accessible when a button is clicked
        private void SliderChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            //makes sure the sender object is actually a slider
            Slider slide = sender as Slider;
            if (slide is null)
                MessageBox.Show("There has been a fatal error, please try again.");

            //gets the product code to identify the slider that was clicked
            string productCode = GetProductCode(slide.Name);

            //gets the index of the product this slider is linked to
            int index = GetListIndex(productCode);

            //saves the slider value
            _sliderValues[index] = (int)slide.Value;

            //calls a method to updates the slider amount that shows on the screen
            ShowSliderValues(_sliderValues[index], productCode);
        }

        //this shows a slider amount on the screen depending on which product the slider is for
        private void ShowSliderValues(int sliderValue, string productCode)
        {
            //switch case finds the textblock that needs to be updated based on the provided product code
            switch (productCode)
            {
                case "DRK01":
                    DRK01Show.Text = sliderValue.ToString();
                    break;
                case "DRK02":
                    DRK02Show.Text = sliderValue.ToString();
                    break;
                case "DRK03":
                    DRK03Show.Text = sliderValue.ToString();
                    break;
                case "CHP01":
                    CHP01Show.Text = sliderValue.ToString();
                    break;
                case "CHP02":
                    CHP02Show.Text = sliderValue.ToString();
                    break;
                case "CHP03":
                    CHP03Show.Text = sliderValue.ToString();
                    break;
                case "CHO01":
                    CHO01Show.Text = sliderValue.ToString();
                    break;
                case "CHO02":
                    CHO02Show.Text = sliderValue.ToString();
                    break;
                case "CHO03":
                    CHO03Show.Text = sliderValue.ToString();
                    break;
                case "BAR01":
                    BAR01Show.Text = sliderValue.ToString();
                    break;
                case "BAR02":
                    BAR02Show.Text = sliderValue.ToString();
                    break;
                case "BAR03":
                    BAR03Show.Text = sliderValue.ToString();
                    break;
                default:
                    break;
            }
        }

        #endregion

        #region button clicks

        //it gets the slider value for that product and adds the amount specified to the shopping cart
        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            //makes sure the sender object is actually a button
            Button add = sender as Button;
            if (sender is null)
                MessageBox.Show("There has been a fatal error, please try again.");

            //this gets the product code, which is the first 5 letters of the button name
            string productCode = GetProductCode(add.Name);

            //this gets the index of the product from the list of products
            int index = GetListIndex(productCode);
            
            //gets the quantity from the slider value list, which is parallel to the products list
            int quantity = _sliderValues[index];
            
            //try catch so that the program doesn't crash if an exception is thrown
            try
            {
                //adds the item to the cart however may times was specified
                _cart.AddItemToCart(productCode, quantity);

                //updates the on screen shopping cart
                ShoppingInfo.Text = _cart.ToString();

                //calls a method to change item background colour based on item's stock
                CheckForNoStock();
            }
            catch (Exception error)
            {
                //lets the user know a problem has occured
                MessageBox.Show(error.Message, "Problem", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            
        }

        //this method gets the slider value for that product and removes that amount specified from the shopping cart
        private void btnRemove_Click(object sender, RoutedEventArgs e)
        {
            //makes sure the sender object is actually a button
            Button add = sender as Button;
            if (sender is null)
                MessageBox.Show("There has been a fatal error, please try again.");

            //this gets the product code, which is the first 5 letters of the button name
            string productCode = GetProductCode(add.Name);

            //this gets the index of the product from the list of products
            int index = GetListIndex(productCode);

            //gets the quantity from the slider value list, which is parallel to the products list
            int quantity = _sliderValues[index];

            //try catch so that the program doesn't crash if an exception is thrown
            try
            {
                //removes the number of items from the cart
                _cart.RemoveItemFromCart(productCode, quantity);

                //updates the on screen shopping cart
                ShoppingInfo.Text = _cart.ToString();

                //calls a method that changes item backgrounds depending on the item's stock
                CheckForNoStock();
            }
            catch (Exception error)
            {
                //lets the user know a problem has occured
                MessageBox.Show(error.Message, "Problem", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            
        }

        //this method opens the checkout page
        private void btnProceedCheckount_Click(object sender, RoutedEventArgs e)
        {
            //making sure there are items in the shopping cart to buy
            if (_cart.TotalPrice() > 0)
            {
                //creates a new instance of the checkout page
                PaymentDetailsWindow newPage = new PaymentDetailsWindow(_cart, STOCK_FILE_PATH, _products);
                
                //uses show dialog so the user can't do anything with the first window before they're done with checking out
                newPage.ShowDialog();
            }

            //resets the sliders and the on screen shopping cart since it isn't needed when checking out and so that it will be ready for the
            //next transaction
            ResetSliders();
            ShoppingInfo.Text = "";
        }

        #endregion

        #region getting information

        //this method gets the product code from a wpf feature's name
        //returns the productCode
        public string GetProductCode(string originalString)
        {
            string productCode = "";

            //goes through the first 5 chars of the name to get the code
            for (int i = 0; i < PRODUCT_CODE_LENGTH; i++)
            {
                productCode += originalString[i];
            }

            return productCode;
        }


        //this method gets the index of product with the provided product code
        //since there are a lot of parallel lists going on, this prevents code repetition
        //it either returns the index of the product, or -1 if the product was never found
        public int GetListIndex(string productCode)
        {
            int index = 0;

            //goes through each product to see if the codes match to find the index
            foreach (Product item in _products)
            {
                if (item.ProductCode == productCode)
                    return index;

                index++;
            }

            return -1;
        }

        #endregion

        #region changing background colour

        //this method checks for which items have 0 stock and calls a second method to actually change the colour
        private void CheckForNoStock()
        {
            SolidColorBrush gray = new SolidColorBrush(Colors.LightSlateGray);
            SolidColorBrush regular = new SolidColorBrush(Colors.Transparent);

            bool enabledState;

            //goes through each product to check if their stock is zero or not
            foreach (Product item in _products)
            {
                //checks if there is no stock left
                if (item.Stock == 0)
                {
                    //this lets the next method know that the add button should not be enabled
                    enabledState = false;

                    //this calls a method to change a products background to gray and disable their add button
                    ChangeItemBackground(item.ProductCode, gray, enabledState);
                }
                else
                {
                    //this lets the next method know that the add button should be enabled
                    enabledState = true;

                    //this calls a method to change a products background to its regular colour and to enable the add button
                    ChangeItemBackground(item.ProductCode, regular, enabledState);
                }
            }
        }

        //this mainly just a switch case to turn the background colour to gray
        //currently a work in progress since for some reason it refuses to accept that any of the StackPanel names are actually StackPanels
        private void ChangeItemBackground(string productCode, SolidColorBrush colour, bool state)
        {
            //can change the exact shade of gray, just using it for now
            int indexOfProduct = GetListIndex(productCode);

            //this updates the specified products stock value, changes the background colour, and enables/disables the products add button
            switch (productCode)
            {
                
                case "DRK01":
                    DRK01Stack.Background = colour;
                    DRK01BtnAdd.IsEnabled = state;
                    DRK01Stock.Text = _products[indexOfProduct].Stock.ToString();
                    break;
                case "DRK02":
                    DRK02Stack.Background = colour;
                    DRK02BtnAdd.IsEnabled = state;
                    DRK02Stock.Text = _products[indexOfProduct].Stock.ToString();
                    break;
                case "DRK03":
                    DRK03Stack.Background = colour;
                    DRK03BtnAdd.IsEnabled = state;
                    DRK03Stock.Text = _products[indexOfProduct].Stock.ToString();
                    break;
                case "CHP01":
                    CHP01Stack.Background = colour;
                    CHP01BtnAdd.IsEnabled = state;
                    CHP01Stock.Text = _products[indexOfProduct].Stock.ToString();
                    break;
                case "CHP02":
                    CHP02Stack.Background = colour;
                    CHP02BtnAdd.IsEnabled = state;
                    CHP02Stock.Text = _products[indexOfProduct].Stock.ToString();
                    break;
                case "CHP03":
                    CHP03Stack.Background = colour;
                    CHP03BtnAdd.IsEnabled = state;
                    CHP03Stock.Text = _products[indexOfProduct].Stock.ToString();
                    break;
                case "CHO01":
                    CHO01Stack.Background = colour;
                    CHO01BtnAdd.IsEnabled = state;
                    CHO01Stock.Text = _products[indexOfProduct].Stock.ToString();
                    break;
                case "CHO02":
                    CHO02Stack.Background = colour;
                    CHO02BtnAdd.IsEnabled = state;
                    CHO02Stock.Text = _products[indexOfProduct].Stock.ToString();
                    break;
                case "CHO03":
                    CHO03Stack.Background = colour;
                    CHO03BtnAdd.IsEnabled = state;
                    CHO03Stock.Text = _products[indexOfProduct].Stock.ToString();
                    break;
                case "BAR01":
                    BAR01Stack.Background = colour;
                    BAR01BtnAdd.IsEnabled = state;
                    BAR01Stock.Text = _products[indexOfProduct].Stock.ToString();
                    break;
                case "BAR02":
                    BAR02Stack.Background = colour;
                    BAR02BtnAdd.IsEnabled = state;
                    BAR02Stock.Text = _products[indexOfProduct].Stock.ToString();
                    break;
                case "BAR03":
                    BAR03Stack.Background = colour;
                    BAR03BtnAdd.IsEnabled = state;
                    BAR03Stock.Text = _products[indexOfProduct].Stock.ToString();
                    break;
                default:
                    break;
            }
            
        }

        #endregion

    }
}
