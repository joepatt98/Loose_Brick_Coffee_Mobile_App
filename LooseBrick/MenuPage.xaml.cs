using Square;
using Square.Exceptions;
using Square.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using Xamarin.Forms;
//https://developer.squareup.com/reference/square/catalog-api/upsert-catalog-object
//https://developer.squareup.com/docs/catalog-api/build-with-catalog

/* Creation of LooseBrick App Menu Page
 * Main Steps/Functions:
 * 1. Grab menu items from Square Dashboard Item Library
 * 2. Create display for each menu item
 * 3. Functionality for "Add to cart" and "Checkout" buttons
 */
namespace LooseBrick
{
    public partial class MenuPage : ContentPage
    {
        // This sets up the connection to your Square Sandbox dashboard
        // Change the access token to your own
        SquareClient client = new SquareClient.Builder().Environment(Square.Environment.Sandbox)
                                                            .AccessToken("EAAAECVdu1OyHExv8tNbUM6rJyxZYl9FEsQoDZPGMXAaD2obRyJF051mzm_equSx")
                                                            .Build();
        // Change the location ID to your own
        String locationID = "L03JRP068FQD0";
        IList<OrderLineItem> lineItems = new List<OrderLineItem>();
        int finalPrice = 0;

        public MenuPage()
        {
            InitializeComponent();
            retreiveItems();
        }

        async void retreiveItems()
        {

            // Grabs the mainstack, flex layout, and scroll view from MenuPage.xaml (lines 56-58)
            StackLayout stack = mainstack;
            FlexLayout flex = flexLayout;
            ScrollView scroll = scrollView;
            
            // Grabbing Item object types (menu items)
            var objectTypes = new List<string>();
            objectTypes.Add("ITEM");

            // Creates request for menu items by first letter
            var aPrefixQuery = new CatalogQueryPrefix.Builder(attributeName: "name", attributePrefix: "A").Build();
            var aQuery = new CatalogQuery.Builder().PrefixQuery(aPrefixQuery).Build();
            var aBody = new SearchCatalogObjectsRequest.Builder().ObjectTypes(objectTypes).Query(aQuery).Limit(100).Build();
            
            var bPrefixQuery = new CatalogQueryPrefix.Builder(attributeName: "name", attributePrefix: "B").Build();
            var bQuery = new CatalogQuery.Builder().PrefixQuery(bPrefixQuery).Build();
            var bBody = new SearchCatalogObjectsRequest.Builder().ObjectTypes(objectTypes).Query(bQuery).Limit(100).Build();

            try
            {
                // Grabs menu item objects from Catalog requests to Square
                var aResult = await client.CatalogApi.SearchCatalogObjectsAsync(body: aBody);
                IList<CatalogObject> aObjects = aResult.Objects;
                IEnumerator<CatalogObject> aEnum = aObjects.GetEnumerator();
                aEnum.MoveNext();

                var bResult = await client.CatalogApi.SearchCatalogObjectsAsync(body: bBody);
                IList<CatalogObject> bObjects = bResult.Objects;
                IEnumerator<CatalogObject> bEnum = bObjects.GetEnumerator();
                bEnum.MoveNext();
                
                // Calls method to create a frame (small display of item info) for each menu item object
                // After frame is create it is added to the flex layout
                foreach (CatalogObject aObject in aObjects)
                {
                    Frame aFrame = CreateFrame(aObject);
                    flex.Children.Add(aFrame);
                }
                foreach (CatalogObject bObject in bObjects)
                {
                    Frame bFrame = CreateFrame(bObject);
                    flex.Children.Add(bFrame);
                }
                
            }
            catch (ApiException e)
            {
                Console.WriteLine("Failed to make the request");
                Console.WriteLine($"Response Code: {e.ResponseCode}");
                Console.WriteLine($"Exception: {e.Message}");
            }
        }

        public Frame CreateFrame(CatalogObject obj)
        {
            /* This method creates the display of the menu item's information
             * It accepts one menu item object (catalog object)
             */

            // Grabs the styles (color, text, fontsize, etc.) of certain .xaml types from MenuPage.xaml (lines 6-55)
            Style button = cartButton;
            Style header = headerLabel;
            Style info = infoLabel;
            Style frameS = frameStyle;
            Style regLabel = regularLabel; // this label has a flexlayout grow property so the children will fill the whole frame

            IList<Label> labels = new List<Label>();

            FlexLayout flexLayout = new FlexLayout
            {
                Direction = FlexDirection.Column,
                AlignContent = FlexAlignContent.Center
            };

            StackLayout bigStack = new StackLayout
            {
                Orientation = StackOrientation.Vertical,
                VerticalOptions = LayoutOptions.CenterAndExpand,
                Children =
                {
                    new Label
                    {
                        Text = obj.ItemData.Name, //menu item name
                        Style = header
                    }
                }
            };
            String price = "";
            // A variation is like regular, medium, large
            if (obj.ItemData.Variations.Count != 0)
            {
                IList<CatalogObject> variations = obj.ItemData.Variations;
                IEnumerator<CatalogObject> varEnum = variations.GetEnumerator();
                varEnum.MoveNext();

                /* NOT IMPLEMENTED YET
                 * To create functionality for the checkboxes to know which variation the user wants -
                 * use lists of checkboxes and labels first, then loop through the lists to create the display
                 */ 
                IList<CheckBox> checkBoxList = new List<CheckBox>();
                IList<Label> varLabelList = new List<Label>();

                foreach (CatalogObject variation in variations)
                {
                    /* For each variation of the menu item:
                     * Create a Checkbox and label
                     * Add the checkbox and label to the horizontal stack layout created below
                     * Lastly, add the horizontal stack to the vertical stack (bigStack)
                     */
                    StackLayout varStack = new StackLayout
                    {
                        Orientation = StackOrientation.Horizontal
                    };
                    price = variation.ItemVariationData.PriceMoney.Amount.ToString();
                    String varName = variation.ItemVariationData.Name;
                    String newPrice = price.Insert(1, ".");
                    CheckBox check = new CheckBox
                    {
                        Color = Color.Black,
                        StyleId = price + "," + varName + "," + variation.ItemVariationData.ItemId, 
                        // ^^ have price, variation name, and variation id for checkboxes style id
                        // this allows the checkbox to be associated with the variation name, id, and price
                    };
                    checkBoxList.Add(check);
                    Label varLabel = new Label
                    {
                        Text = varName + ": $" + newPrice, // displays the variation name and price
                        Style = info
                    };
                    varLabelList.Add(varLabel);
                    varStack.Children.Add(check);
                    varStack.Children.Add(varLabel);
                    bigStack.Children.Add(varStack);
                }
            }
            flexLayout.Children.Add(bigStack);
            flexLayout.Children.Add(new Label {Style=regLabel});
            // Add to cart button
            Button button1 = new Button
            {
                Style = button,
                StyleId = obj.Id +  "," + obj.ItemData.Name + "," + price 
                // ^^ associate this add to cart button with the items id, name, and price
            };
            button1.Clicked += Button_Clicked;
            flexLayout.Children.Add(button1);

            Frame frame = new Frame
            {
                WidthRequest = 390,
                HeightRequest = 200,
                Style = frameS,
                Content = flexLayout
            };
            return frame;
        }
        async void Button_Clicked(object sender, EventArgs e)
        {
            /* When the add to cart button is clicked:
             * Create a new order line item and add it to the line items list initialized in the beginning
             */
            Button button = (Button)sender;
            
            String id = button.StyleId;
            String[] itemInfo = id.Split(",", 3);
            finalPrice += int.Parse(itemInfo[2]);
            var basePriceMoney = new Money.Builder()
                                .Amount(int.Parse(itemInfo[2]))
                                .Currency("USD")
                                .Build();

            var orderLineItem = new OrderLineItem.Builder(quantity: "1")
                                //.CatalogObjectId(itemInfo[0])
                                .CatalogObjectId("ETLHFZN4QHYQGBQZ3DBITO6X")
                                //.Name(itemInfo[1])
                                .BasePriceMoney(basePriceMoney)
                                .Build();
            Console.WriteLine(orderLineItem.Name);
            lineItems.Add(orderLineItem);
            Console.WriteLine(lineItems.Count);
        }
        async void Checkout_Button_Clicked(object sender, EventArgs e)
        {
            /* When the checkout button is clicked:
             * Create a Square order request with all the line item information
             * Create a Square payment request
             * If successful, the Checkout button text will display "Success"
             */
            string key = Guid.NewGuid().ToString();
            Console.WriteLine(lineItems.Count);

            var recipient = new OrderFulfillmentRecipient.Builder()
              .DisplayName("Joe Patterson")
              .Build();

            var pickupDetails = new OrderFulfillmentPickupDetails.Builder()
              .Recipient(recipient)
              .ExpiresAt("2021-04-27T20:21:54.59Z")
              .AutoCompleteDuration("P0DT1H0S")
              .ScheduleType("SCHEDULED")
              .PickupAt("2021-04-27T19:21:54.59Z")
              .Note("Pour over coffee")
              .Build();

            var orderFulfillment = new OrderFulfillment.Builder()
              .Type("PICKUP")
              .State("PROPOSED")
              .PickupDetails(pickupDetails)
              .Build();

            var fulfillments = new List<OrderFulfillment>();
            fulfillments.Add(orderFulfillment);

            var orderLineItemTax = new OrderLineItemTax.Builder()
                                    .Uid("state-sales-tax")
                                    .Name("State Sales Tax")
                                    .Percentage("9")
                                    .Scope("ORDER")
                                    .Build();

            var taxes = new List<OrderLineItemTax>();
            taxes.Add(orderLineItemTax);

            var order = new Order.Builder(locationId: locationID)
              .LineItems(lineItems)
              .ReferenceId("Test")
              .CustomerId("customer_test")
              .State("OPEN")
              .Fulfillments(fulfillments)
              //.Taxes(taxes)
              .Build();

            var order_body = new CreateOrderRequest.Builder()
              .Order(order)
              //.LocationId(locationID)
              .IdempotencyKey(key)
              .Build();

            var finalPriceMoney = new Money.Builder()
                                    //.Amount(finalPrice)
                                    .Amount(500L)
                                    .Currency("USD")
                                    .Build();

            var pay_body = new CreatePaymentRequest.Builder(
                    sourceId: "cnon:card-nonce-ok",
                    idempotencyKey: key,
                    amountMoney: finalPriceMoney)
                .Autocomplete(true)
                .CustomerId("customer_test")
                .LocationId(locationID)
                .ReferenceId(key)
                .Note("test")
                .Build();

            try
            {
                var order_result = await client.OrdersApi.CreateOrderAsync(body: order_body);
                var pay_result = await client.PaymentsApi.CreatePaymentAsync(body: pay_body);
                (sender as Button).Text = "Success";
            }
            catch (ApiException ex)
            {
                (sender as Button).Text = "Failure";
                Debug.WriteLine("Failed to make the request");
                Debug.WriteLine($"Response Code: {ex.ResponseCode}");
                Debug.WriteLine($"Exception: {ex}");
            }
        }
    }
}