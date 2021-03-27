using Square;
using Square.Exceptions;
using Square.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using Xamarin.Forms;

namespace LooseBrick
{
    public partial class CartPage : ContentPage
    {

        public CartPage()
        {
            InitializeComponent();

        }

        async void OnButtonClicked(object sender, EventArgs args)
        {
            // Generates a new key (idempotency) each time the button is clicked.
            string key = Guid.NewGuid().ToString();

            // This is the Access Token for the Square Account being used to communicate
            // with the APIs in the application.
            string access_token = "EAAAECVdu1OyHExv8tNbUM6rJyxZYl9FEsQoDZPGMXAaD2obRyJF051mzm_equSx";

            // This is the Location ID for the Square Account being used to communicate
            // with the APIs in the application.
            string location_id = "L03JRP068FQD0";

            SquareClient client = new SquareClient.Builder()
                .Environment(Square.Environment.Sandbox)
                .AccessToken(access_token)
                .Build();

            var basePriceMoney = new Money.Builder()
              .Amount(500L)
              .Currency("USD")
              .Build();

            var orderLineItem = new OrderLineItem.Builder(quantity: "1")
              .Name("Small Coffee")
              .BasePriceMoney(basePriceMoney)
              .Build();

            var lineItems = new List<OrderLineItem>();
            lineItems.Add(orderLineItem);

            var recipient = new OrderFulfillmentRecipient.Builder()
              .DisplayName("Joe Patterson")
              .Build();

            var pickupDetails = new OrderFulfillmentPickupDetails.Builder()
              .Recipient(recipient)
              .ExpiresAt("2021-02-28T20:21:54.59Z")
              //.AutoCompleteDuration("P0DT1H0S")
              .ScheduleType("SCHEDULED")
              .PickupAt("2021-02-28T19:21:54.59Z")
              .Note("Pour over coffee")
              .Build();

            var orderFulfillment = new OrderFulfillment.Builder()
              .Type("PICKUP")
              .State("PROPOSED")
              .PickupDetails(pickupDetails)
              .Build();

            var fulfillments = new List<OrderFulfillment>();
            fulfillments.Add(orderFulfillment);

            var order = new Order.Builder(locationId: location_id)
              .LineItems(lineItems)
              .CustomerId("customer_test")
              .State("OPEN")
              .Fulfillments(fulfillments)
              .Build();

            var order_body = new CreateOrderRequest.Builder()
              .Order(order)
              .LocationId(location_id)
              .IdempotencyKey(key)
              .Build();

            var pay_body = new CreatePaymentRequest.Builder(
                    sourceId: "cnon:card-nonce-ok",
                    idempotencyKey: key,
                    amountMoney: basePriceMoney)
                .Autocomplete(true)
                .CustomerId("customer_test")
                .LocationId(location_id)
                .ReferenceId(key)
                .Note("test")
                .Build();

            try
            {
                var order_result = await client.OrdersApi.CreateOrderAsync(body: order_body);
                var pay_result = await client.PaymentsApi.CreatePaymentAsync(body: pay_body);

                //Debug
                (sender as Button).Text = "Success";
            }

            catch (ApiException e)
            {
                Debug.WriteLine("Failed to make the request");
                Debug.WriteLine($"Response Code: {e.ResponseCode}");
                Debug.WriteLine($"Exception: {e}");

                //Debug
                (sender as Button).Text = "Failure";
            }



        }



    }
}
