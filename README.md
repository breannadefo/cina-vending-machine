# cina-vending-machine

## Goal
This project was made using C# and WPF. It is a vending machine app. The reason it was created is because for the final project in one of my classes, we had to make a WPF project in pairs. Out of all the options we were provided with, my partner and I decided that we liked the idea of creating a vending machine the most.

## Key Features

### Adding and Removing items from the cart:
The app displays a selection of items. Each item has a slider to select how many of that item you want to add or remove from your cart. Below the slider, there are two buttons: 'Add' and 'Remove'. These actually add/remove the items.   
   
### Greyed out items:
When the items are displayed, some of them might be greyed out. This means that the particular item is out of stock, so it can't be added to the cart.   
   
### Checkout:
When you add everything you want to the cart, there is a 'Proceed to Checkout' button at the bottom of the screen, which will open a payment screen.   
   
### Receipt:
Once you have paid for the items in your cart, you will get a receipt that will pop up on the screen.
   
All transactions get saved to a file to keep track of the history, so once you've accepted your receipt, another popup will appear to tell you this, as well as giving you the option to open said file.   
