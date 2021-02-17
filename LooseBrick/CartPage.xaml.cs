using System.Collections.Generic;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Square;
using Square.Models;
using Square.Exceptions;
using System;
using System.Diagnostics;

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
            string key = Guid.NewGuid().ToString();

            SquareClient client = new SquareClient.Builder()
                .Environment(Square.Environment.Sandbox)
                .AccessToken("EAAAEEGUegliN33KdnaRMfEGKbSzgz723KNZ3IzrMA6cIZ1CdPJ-rS3Li7PJhyAD")
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
              .ExpiresAt("2021-02-18T20:21:54.59Z")
              //.AutoCompleteDuration("P0DT1H0S")
              .ScheduleType("SCHEDULED")
              .PickupAt("2021-02-18T19:21:54.59Z")
              .Note("Pour over coffee")
              .Build();

            var orderFulfillment = new OrderFulfillment.Builder()
              .Type("PICKUP")
              .State("PROPOSED")
              .PickupDetails(pickupDetails)
              .Build();

            var fulfillments = new List<OrderFulfillment>();
            fulfillments.Add(orderFulfillment);

            var order = new Order.Builder(locationId: "LTBXEM50E3Q47")
              .LineItems(lineItems)
              .CustomerId("customer_test")
              .State("OPEN")
              .Fulfillments(fulfillments)
              //.Id(key)
              .Build();

            var order_body = new CreateOrderRequest.Builder()
              .Order(order)
              .LocationId("LTBXEM50E3Q47")
              .IdempotencyKey(key)
              .Build();

            var pay_body = new CreatePaymentRequest.Builder(
                    sourceId: "cnon:card-nonce-ok",
                    idempotencyKey: key,
                    amountMoney: basePriceMoney)
                .Autocomplete(true)
                .CustomerId("customer_test")
                .LocationId("LTBXEM50E3Q47")
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
