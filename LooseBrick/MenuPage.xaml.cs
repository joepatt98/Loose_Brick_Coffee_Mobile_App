using Square;
using Square.Exceptions;
using Square.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using Xamarin.Forms;
//https://docs.microsoft.com/en-us/xamarin/xamarin-forms/app-fundamentals/navigation/hierarchical#passing-data-when-navigating
//https://developer.squareup.com/reference/square/catalog-api/upsert-catalog-object
//https://developer.squareup.com/docs/catalog-api/build-with-catalog

namespace LooseBrick
{
    public partial class MenuPage : ContentPage
    {
        SquareClient client = new SquareClient.Builder().Environment(Square.Environment.Sandbox)
                                                            .AccessToken("EAAAECVdu1OyHExv8tNbUM6rJyxZYl9FEsQoDZPGMXAaD2obRyJF051mzm_equSx")
                                                            .Build();
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
            StackLayout stack = mainstack;
            FlexLayout flex = flexLayout;
            ScrollView scroll = scrollView;
            
            var objectTypes = new List<string>();
            objectTypes.Add("ITEM");

            var aPrefixQuery = new CatalogQueryPrefix.Builder(attributeName: "name", attributePrefix: "A").Build();
            var aQuery = new CatalogQuery.Builder().PrefixQuery(aPrefixQuery).Build();
            var aBody = new SearchCatalogObjectsRequest.Builder().ObjectTypes(objectTypes).Query(aQuery).Limit(100).Build();
            
            var bPrefixQuery = new CatalogQueryPrefix.Builder(attributeName: "name", attributePrefix: "B").Build();
            var bQuery = new CatalogQuery.Builder().PrefixQuery(bPrefixQuery).Build();
            var bBody = new SearchCatalogObjectsRequest.Builder().ObjectTypes(objectTypes).Query(bQuery).Limit(100).Build();

            /*var blendedPrefixQuery = new CatalogQueryPrefix.Builder(attributeName: "name", attributePrefix: "blended").Build();
            var blendedQuery = new CatalogQuery.Builder().PrefixQuery(blendedPrefixQuery).Build();
            var blendedBody = new SearchCatalogObjectsRequest.Builder().ObjectTypes(objectTypes).Query(blendedQuery).Limit(100).Build();*/

            try
            {
                var aResult = await client.CatalogApi.SearchCatalogObjectsAsync(body: aBody);
                IList<CatalogObject> aObjects = aResult.Objects;
                IEnumerator<CatalogObject> aEnum = aObjects.GetEnumerator();
                aEnum.MoveNext();

                var bResult = await client.CatalogApi.SearchCatalogObjectsAsync(body: bBody);
                IList<CatalogObject> bObjects = bResult.Objects;
                IEnumerator<CatalogObject> bEnum = bObjects.GetEnumerator();
                bEnum.MoveNext();

                /*var blendedResult = await client.CatalogApi.SearchCatalogObjectsAsync(body: blendedBody);
                IList<CatalogObject> blendedObjects = blendedResult.Objects;
                IEnumerator<CatalogObject> blendedEnum = blendedObjects.GetEnumerator();
                blendedEnum.MoveNext();*/
                
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
                /*foreach (CatalogObject blended in blendedObjects)
                {
                    Frame blendedFrame = CreateFrame(blended);
                    flex.Children.Add(blendedFrame);
                }*/
                
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
            // do variation stuff in here
            // can do an if for variations
            // or for each variation create two labels for name and price
            Style button = cartButton;
            Style header = headerLabel;
            Style info = infoLabel;
            Style frameS = frameStyle;
            Style regLabel = regularLabel;
            IList<Label> labels = new List<Label>();

            FlexLayout flexLayout = new FlexLayout
            {
                Direction = FlexDirection.Column,
                AlignContent = FlexAlignContent.Center,
                Children =
                {
                    new Label
                    {
                        Text = obj.ItemData.Name,
                        Style = header
                    }
                }
            };
            String price = "";
            if (obj.ItemData.Variations.Count != 0)
            {
                IList<CatalogObject> variations = obj.ItemData.Variations;
                IEnumerator<CatalogObject> varEnum = variations.GetEnumerator();
                varEnum.MoveNext();

                foreach (CatalogObject variation in variations)
                {
                    price = variation.ItemVariationData.PriceMoney.Amount.ToString();
                    String varName = variation.ItemVariationData.Name;
                    String newPrice = price.Insert(1, ".");
                    CheckBox check = new CheckBox
                    {
                        Color = Color.White,
                        StyleId = price + "," + varName
                    };
                    Label varLabel = new Label
                    {
                        Text = varName + ": $" + newPrice,
                        Style = info
                    };
                    flexLayout.Children.Add(check);
                    flexLayout.Children.Add(varLabel);
                }
            }
            flexLayout.Children.Add(new Label {Style=regLabel});
            Button button1 = new Button
            {
                Style = button,
                StyleId = obj.Id +  "," + obj.ItemData.Name + "," + price
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
            //FlexLayout.SetGrow(growLabel, 1);
            return frame;
        }
        async void Button_Clicked(object sender, EventArgs e)
        {
            //string key = Guid.NewGuid().ToString();
            Button button = (Button)sender;
            
            String id = button.StyleId;
            String[] itemInfo = id.Split(",", 3);
            finalPrice += int.Parse(itemInfo[2]);
            var basePriceMoney = new Money.Builder()
                                .Amount(int.Parse(itemInfo[2]))
                                .Currency("USD")
                                .Build();

            var orderLineItem = new OrderLineItem.Builder(quantity: "1")
                                .CatalogObjectId(itemInfo[0])
                                .Name(itemInfo[1])
                                .BasePriceMoney(basePriceMoney)
                                .Build();

            lineItems.Add(orderLineItem);
        }
        async void Checkout_Button_Clicked(object sender, EventArgs e)
        {
            string key = Guid.NewGuid().ToString();

            var pickupDetails = new OrderFulfillmentPickupDetails.Builder()
              .ExpiresAt("2021-03-30T20:21:54.59Z")
              //.AutoCompleteDuration("P0DT1H0S")
              .ScheduleType("SCHEDULED")
              .PickupAt("2021-03-30T19:21:54.59Z")
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
              .CustomerId("customer_test")
              .State("OPEN")
              .Fulfillments(fulfillments)
              .Taxes(taxes)
              .Build();

            var order_body = new CreateOrderRequest.Builder()
              .Order(order)
              .LocationId(locationID)
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
                //.Autocomplete(true)
                .CustomerId("customer_test")
                .LocationId(locationID)
                .ReferenceId(key)
                .Note("test")
                .Build();

            try
            {
                var order_result = await client.OrdersApi.CreateOrderAsync(body: order_body);
                var pay_result = await client.PaymentsApi.CreatePaymentAsync(body: pay_body);
            }
            catch (ApiException ex)
            {
                Debug.WriteLine("Failed to make the request");
                Debug.WriteLine($"Response Code: {ex.ResponseCode}");
                Debug.WriteLine($"Exception: {ex}");
            }
        }

        // This works but only for one item 
        async void retrieve(SquareClient client)
        {
            var objectIds = new List<string>();
            //objectIds.Add("AJCF2IUQWYJBC7DTNSJKXK6O");
            objectIds.Add("PJMCEBHHUS3OKDB6PYUHLCPP");

            var body = new BatchRetrieveCatalogObjectsRequest.Builder(objectIds: objectIds)
              .IncludeRelatedObjects(true)
              .Build();

            try
            {
                var result = await client.CatalogApi.BatchRetrieveCatalogObjectsAsync(body: body);
                Console.WriteLine("result");
            }
            catch (ApiException e)
            {
                Console.WriteLine("Failure in retrieve");
                Console.WriteLine("Failed to make the request");
                Console.WriteLine($"Response Code: {e.ResponseCode}");
                Console.WriteLine($"Exception: {e.Message}");
            }
        }
        // This works but only for one item right now
        async void delete(SquareClient client)
        {
            var objectIds = new List<string>();
            objectIds.Add("AJCF2IUQWYJBC7DTNSJKXK6O");

            var body = new BatchDeleteCatalogObjectsRequest.Builder()
              .ObjectIds(objectIds)
              .Build();

            try
            {
                var result = await client.CatalogApi.BatchDeleteCatalogObjectsAsync(body: body);
                IEnumerator<string> ids = result.DeletedObjectIds.GetEnumerator();
                ids.MoveNext();
                Console.WriteLine(ids.Current.ToString());
            }
            catch (ApiException e)
            {
                Console.WriteLine("Failure in delete");
                Console.WriteLine("Failed to make the request");
                Console.WriteLine($"Response Code: {e.ResponseCode}");
                Console.WriteLine($"Exception: {e.Message}");
            }
        }
    }
}