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
//using System.Windows.Shapes;
using System.IO;
using PIIIProject.Models;
using System.Diagnostics;

namespace PIIIProject
{
    /// <summary>
    /// Interaction logic for PaymentDetailsWindow.xaml
    /// </summary>
    public partial class PaymentDetailsWindow : Window
    {
        #region global and contant variables

        //global variables
        ShoppingCart _cart;
        private static string _filePath;
        private static List<Product> _products;

        //constant variables
        const int MINIMUM_CREDIT_SPENDING = 5;
        const string TRANSACTIONS_FILE_PATH = "../../../DataFiles/Transactions.txt";

        #endregion

        #region window set up

        public PaymentDetailsWindow(ShoppingCart cart, string filePath, List<Product> products)
        {
            //global variables are set by the values that are passed through the constructor
            _cart = cart;
            _filePath = filePath;
            _products = products;

            //initializes the window
            InitializeComponent();
            
            //makes the total price show up on screen
            ShowTotal.Text = _cart.TotalPrice().ToString();
        }

        #endregion

        #region button click

        //this method is called when the user decides to checkout. It figures out their method of payment and prints a receipt
        private void btnCheckout_Click(object sender, RoutedEventArgs e)
        {
            //this is used to indicate whether or not the checkout was successful and all the data should be exported and reset
            bool canContinue = true;

            //makes sure the sender object is actually a button
            Button checkout = sender as Button;
            if (checkout is null)
                MessageBox.Show("There has been a fatal error, please try again.");

            //creates the stringbuilder that will create the receipt
            StringBuilder receipt = new StringBuilder();

            //adds the items breakdown and the total cost to the receipt
            receipt.AppendLine(_cart.ToString());
            receipt.AppendLine($"Total Cost: ${_cart.TotalPrice()}\n");

            //checks which method of payment the user decided to go with
            if (rdbCash.IsChecked == true)
            {
                //since user chose cash, it calls a method to create the rest of the receipt that deals with cash
                CheckoutWithCash(receipt, ref canContinue);
            }
            else if (rdbCredit.IsChecked == true)
            {
                //since user chose credit/debit, it calls a method to create the rest of the receipt that deals with credit/debit
                CheckoutWithCredit(receipt, ref canContinue);
            }
            else
            {
                //this is what happens if the user did not specify their method of payment

                //keeps track that the transaction was unsuccessful
                canContinue = false;

                //lets the user know what the problem was and that they need to choose a payment type
                MessageBox.Show("You must select Cash or Credit/Debit", "Problem", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            //only runs if the transaction went through
            if (canContinue)
            {
                //sets up the message for the user
                string messageForUser = "Transaction saved to history file.\nWould you like to open the file?";
                
                //updates the stock file so that it hold correct information
                UpdateFile();
                
                //this adds the transaction to a history file. if it is successful, it tells the user so
                if (UpdateTransactions(receipt.ToString()))
                {
                    //lets the user know the transaction was saved to a file
                    //asks them if they want to open said file
                    //this checks for the user's answer
                    if (MessageBox.Show(messageForUser, "Transaction saved.", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                    {
                        //opens the file

                        //got the code to open the file from assignment 5 instructions in OneDrive
                        Process.Start(new ProcessStartInfo
                        {
                            FileName = Path.GetFullPath(TRANSACTIONS_FILE_PATH),
                            UseShellExecute = true
                        });
                    }
                }
                
                //this unchecks both of the checkboxes
                rdbCash.IsChecked = rdbCredit.IsChecked = false;

                //this empties the cart now that the transaction has gone through
                _cart.ResetCart();

                //this empties the text box where the user specifies the bill they are using to pay
                InputCash.Text = string.Empty;

            }
        }

        #endregion

        #region cash methods

        //this method creates the receipt for when the user chooses to pay with cash
        private void CheckoutWithCash(StringBuilder receipt, ref bool canContinue)
        {
            int[] possibleBills = new int[] { 5, 10, 20, 50, 100 };
            int billAmount;
            decimal change;
            bool validBillAmount = false;

            //adds the payment type to the receipt
            receipt.AppendLine("Payment Choice: Cash");

            //checks that the user wrote a number in the provided text box
            if (string.IsNullOrEmpty(InputCash.Text) || !int.TryParse(InputCash.Text, out billAmount))
            {
                //saves that the transaction has failed
                canContinue = false;

                //lets the user know what their problem was
                MessageBox.Show("You need to put in a bill amount when choosing cash.", "Problem", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else
            {
                //checks if the inputted number is a valid bill number
                foreach (int number in possibleBills)
                    if (billAmount == number)
                    {
                        //sets this to true if the number is valid
                        validBillAmount = true;

                        //exits the foreach early since the number is confirmed to be valid
                        break;
                    }

                //checks that the bill amount is valid and that it is more than the total cost
                if (validBillAmount && billAmount >= _cart.TotalPrice())
                {
                    //calculates the change
                    change = (decimal)billAmount - _cart.TotalPrice();

                    //adds the bill amount and the change to the receipt
                    receipt.AppendLine($"Amount received: ${billAmount}");
                    receipt.AppendLine($"Change: ${change}");

                    //adds the breakdown to the receipt
                    receipt.AppendLine("\nBreakdown: \n");

                    //checks if there is change to give
                    if (change > 0)
                    {
                        //calls a method to calculate how the user will receive their change
                        CalculateChangeBreakdown(receipt, change);
                    }
                    else
                    {
                        //adds that there is no change to be received to the receipt
                        receipt.AppendLine("No change received.");
                    }

                    //adds a final thank you message to the receipt
                    receipt.AppendLine("\nThank you for shopping at CINA vending machine.\nCome again soon!");

                    //actually prints the receipt
                    MessageBox.Show(receipt.ToString(), "Receipt", MessageBoxButton.OK);

                }
                else
                {
                    //saves that the transaction has failed
                    canContinue = false;

                    //lets the user know what the problem was
                    MessageBox.Show("Bill amount must be a valid bill number (5, 10, 20, 50, or 100) \nAND it must be more than the cost.", "Problem", MessageBoxButton.OK, MessageBoxImage.Error);
                }

            }
        }

        //this method gets the breakdown of the user's change and adds it to the receipt string builder
        private void CalculateChangeBreakdown(StringBuilder sb, decimal change)
        {
            //list of all the possible bill/coin amounts
            List<decimal> billCoinAmounts = new List<decimal> { 100, 50, 20, 10, 5, 2, 1, 0.25M, 0.10M, 0.05M };
            
            //keeps track of how many of each bill/coin needs to be given to customer
            int amountOfBills = 0;

            //goes through every value in list to calculate how many of each bill/coin the user gets
            foreach (decimal amount in billCoinAmounts)
            {
                //loops so that the user can receive multiple of the same bill/coin
                while (change >= amount)
                {
                    //removes the bill/coin amount from the total
                    change -= amount;

                    //increases how of this bill/coin is necessary
                    amountOfBills++;
                }

                //checks if there is any of this bill/coin to return to customer
                if (amountOfBills > 0)
                {
                    //adds the information for a bill/coin to the receipt
                    sb.AppendLine($"{amountOfBills} ${amount} bill(s)/coin(s)");

                    //resets the bill/coin counter
                    amountOfBills = 0;
                }
            }
        }

        #endregion

        #region credit/debit methods

        //this method creates the receipt for when the user chooses to pay with credit/debit
        private void CheckoutWithCredit(StringBuilder receipt, ref bool canContinue)
        {
            //checks that the total price is more than the minimum spending for using credit/debit
            if (_cart.TotalPrice() > MINIMUM_CREDIT_SPENDING)
            {
                //adds the payment type and a thank you message to the receipt
                receipt.AppendLine("Payment Choice: Credit/Debit");
                receipt.AppendLine();
                receipt.AppendLine("Thank you for shopping at CINA vending machine.\nCome again soon!");

                //shows the user the receipt
                MessageBox.Show(receipt.ToString(), "Receipt", MessageBoxButton.OK);
            }
            else
            {
                //saves that the transaction has failed
                canContinue = false;

                //lets the user know what the problem was
                MessageBox.Show($"Your total must be above ${MINIMUM_CREDIT_SPENDING} to use credit/debit.", "Problem", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        #endregion

        #region reading/writing to files

        //this method updates the stock data file with the new product stocks
        public static void UpdateFile()
        {
            StreamWriter stream = null;
            
            //try catch so that the program doesn't crash if something goes wrong while writing to the file
            try
            {
                //opens the file
                stream = new StreamWriter(_filePath);

                StringBuilder sb = new StringBuilder();

                //adds the first line, which is the indicator of how every line should look like
                sb.AppendLine("productCode,name,price,stock");

                //goes through each product
                foreach (Product item in _products)
                {
                    //adds all the product information to the list to keep everything up to date
                    sb.AppendLine($"{item.ProductCode},{item.Name},{item.Price},{item.Stock}");
                }

                //overwites the file with the new, up-to-date information
                stream.Write(sb.ToString());
            }
            catch (Exception e)
            {
                //lets the user know there was a problem while writing to the file
                MessageBox.Show(e.Message, "Problem", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                //checks if the stream is open, and closes it if it is
                if (stream != null)
                    stream.Close();
            }
        }

        //this method takes in a receipt and then adds that receipt to a file that keeps all the previous receipts
        public static bool UpdateTransactions(string newReceipt)
        {
            StringBuilder sb = new StringBuilder();
            bool isSuccessful = true;

            //checks if the transaction history file already exists
            if (File.Exists(TRANSACTIONS_FILE_PATH))
            {
                //try catch so the program doesn't crash if there is a problem
                try
                {
                    //gets all the lines from the file
                    string[] text = File.ReadAllLines(TRANSACTIONS_FILE_PATH);

                    //adds each line to the stringbuilder
                    foreach (string line in text)
                        sb.AppendLine(line);
                }
                catch (Exception e)
                {
                    //lets the user know there was a problem while reading the file and that some data might be lost because of it
                    MessageBox.Show("An error occured when reading the previous transactions. That data will unfortunately be lost.", "Problem", MessageBoxButton.OK, MessageBoxImage.Error);
                }

            }

            StreamWriter writeStream = null;

            //try catch so the program doesn't crash if there is a problem while writing to the file
            try
            {
                //opens the file
                writeStream = new StreamWriter(TRANSACTIONS_FILE_PATH);

                //checks if the string builder is empty
                if (!string.IsNullOrEmpty(sb.ToString()))
                {
                    //adds an extra line if its not empty to help with spacing in the text
                    sb.AppendLine();
                }

                //adds the receipt to the string builder
                sb.AppendLine(newReceipt);

                //adds a separator to help with differenciating the different receipts
                sb.AppendLine("-------------------------------------------------\n");

                //actually writes thr string builder content to the file
                writeStream.Write(sb.ToString());
            }
            catch (Exception e)
            {
                //saves that adding the receipt to the file didn't work
                isSuccessful = false;

                //lets the user know something went wrong and that some data might be lost
                MessageBox.Show("An error occured when adding the transaction to the file. Some data may be lost.", "Problem", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                //closes the stream if the stream is still open
                if (writeStream != null)
                    writeStream.Close();
            }

            //returns whether saving the transaction to a file was successful or not
            return isSuccessful;
        }

        #endregion

    }
}
