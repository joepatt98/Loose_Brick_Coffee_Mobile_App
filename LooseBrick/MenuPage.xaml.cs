using System;
using System.Collections.Generic;
using System.Diagnostics;
using Xamarin.Forms;
using Square;
using Square.Models;
using Square.Exceptions;
using System.Collections;
//https://docs.microsoft.com/en-us/xamarin/xamarin-forms/app-fundamentals/navigation/hierarchical#passing-data-when-navigating
//https://developer.squareup.com/reference/square/catalog-api/upsert-catalog-object
//https://developer.squareup.com/docs/catalog-api/build-with-catalog

namespace LooseBrick
{
    public partial class MenuPage : ContentPage
    {
        public MenuPage()
        {
            InitializeComponent();
        }

        async void Button_Clicked(object sender, EventArgs e)
        {
            string key = Guid.NewGuid().ToString();

            SquareClient client = new SquareClient.Builder()
                .Environment(Square.Environment.Sandbox)
                .AccessToken("EAAAECVdu1OyHExv8tNbUM6rJyxZYl9FEsQoDZPGMXAaD2obRyJF051mzm_equSx")
                .Build();

            // works for one specific item
            //delete(client);

            // Also don't need a locationID for this
            /*IList<string> location = new List<string>();
            location.Add("L03JRP068FQD0");*/

            var taxIds = new List<string>();
            taxIds.Add("#SalesTax");

            var priceMoney = new Money.Builder()
              .Amount(150L)
              .Currency("USD")
              .Build();

            // Creates a variation of Tea - Tea in mug
            var itemVariationData = new CatalogItemVariation.Builder()
              .ItemId("#Tea")
              .Name("Mug")
              .PricingType("FIXED_PRICING")
              .PriceMoney(priceMoney)
              .Build();

            var catalogObject1 = new CatalogObject.Builder(type: "ITEM_VARIATION", id: "#Tea_Mug")//.PresentAtLocationIds(location)
              .PresentAtAllLocations(true)
              .ItemVariationData(itemVariationData)
              .Build();

            var variations = new List<CatalogObject>();
            variations.Add(catalogObject1);

            //Creates the main tea item
            var itemData = new CatalogItem.Builder()
              .Name("Tea")
              .Description("Hot Leaf Juice")
              .CategoryId("#Beverages")
              .TaxIds(taxIds)
              .Variations(variations)
              .Build();

            var catalogObject = new CatalogObject.Builder(type: "ITEM", id: "#Tea")//.PresentAtLocationIds(location)
              .PresentAtAllLocations(true)
              .ItemData(itemData)
              .Build();

            var taxIds1 = new List<string>();
            taxIds1.Add("#SalesTax");

            var priceMoney1 = new Money.Builder()
              .Amount(250L)
              .Currency("USD")
              .Build();

            // Creates a regular coffee variation
            var itemVariationData1 = new CatalogItemVariation.Builder()
              .ItemId("#Coffee")
              .Name("Regular")
              .PricingType("FIXED_PRICING")
              .PriceMoney(priceMoney1)
              .Build();

            var catalogObject3 = new CatalogObject.Builder(type: "ITEM_VARIATION", id: "#Coffee_Regular")//.PresentAtLocationIds(location)
              .PresentAtAllLocations(true)
              .ItemVariationData(itemVariationData1)
              .Build();

            var priceMoney2 = new Money.Builder()
              .Amount(350L)
              .Currency("USD")
              .Build();

            // Creates a large coffee variation
            var itemVariationData2 = new CatalogItemVariation.Builder()
              .ItemId("#Coffee")
              .Name("Large")
              .PricingType("FIXED_PRICING")
              .PriceMoney(priceMoney2)
              .Build();

            var catalogObject4 = new CatalogObject.Builder(type: "ITEM_VARIATION", id: "#Coffee_Large")//.PresentAtLocationIds(location)
              .PresentAtAllLocations(true)
              .ItemVariationData(itemVariationData2)
              .Build();

            var variations1 = new List<CatalogObject>();
            variations1.Add(catalogObject3);
            variations1.Add(catalogObject4);

            // Creates main coffee item
            var itemData1 = new CatalogItem.Builder()
              .Name("Coffee")
              .Description("Hot Bean Juice")
              .CategoryId("#Beverages")
              .TaxIds(taxIds1)
              .Variations(variations1)
              .Build();

            var catalogObject2 = new CatalogObject.Builder(type: "ITEM", id: "#Coffee")//.PresentAtLocationIds(location)
              .PresentAtAllLocations(true)
              .ItemData(itemData1)
              .Build();

            // Creates new category - beverages
            var categoryData = new CatalogCategory.Builder()
              .Name("Beverages")
              .Build();

            var catalogObject5 = new CatalogObject.Builder(type: "CATEGORY", id: "#Beverages")//.PresentAtLocationIds(location)
              .PresentAtAllLocations(true)
              .CategoryData(categoryData)
              .Build();

            // Creates a tax catalog
            var taxData = new CatalogTax.Builder()
              .Name("Sales Tax")
              .CalculationPhase("TAX_SUBTOTAL_PHASE")
              .InclusionType("ADDITIVE")
              .Percentage("5.0")
              .AppliesToCustomAmounts(true)
              .Enabled(true)
              .Build();

            var catalogObject6 = new CatalogObject.Builder(type: "TAX", id: "#SalesTax")//.PresentAtLocationIds(location)
              .PresentAtAllLocations(true)
              .TaxData(taxData)
              .Build();

            // Makes list of tea, coffee, beverage category, and tax
            var objects = new List<CatalogObject>();
            objects.Add(catalogObject);
            objects.Add(catalogObject2);
            objects.Add(catalogObject5);
            objects.Add(catalogObject6);

            var catalogObjectBatch = new CatalogObjectBatch.Builder(objects: objects)
              .Build();

            var batches = new List<CatalogObjectBatch>();
            batches.Add(catalogObjectBatch);

            // Creates request body
            var body = new BatchUpsertCatalogObjectsRequest.Builder(idempotencyKey: key, batches: batches)
              .Build();

            try
            {
                // Sends request to square client - dashboard
                var result = await client.CatalogApi.BatchUpsertCatalogObjectsAsync(body: body);
                (sender as Button).Text = "Added";

                /*Still working on this but it retrieves and outputs Teas(?) object id
                 need to store the object ids in order to retrieve or delete items
                IEnumerator<CatalogIdMapping> enumerator = result.IdMappings.GetEnumerator();
                enumerator.MoveNext();
                Console.WriteLine(enumerator.Current.ObjectId);*/

                // Retrieve is not needed for demo
                //retrieve(client);
            }
            catch (ApiException ex)
            {
                Console.WriteLine("Failure in upsert");
                Console.WriteLine("Failed to make the request");
                Console.WriteLine($"Response Code: {ex.ResponseCode}");
                Console.WriteLine($"Exception: {ex.Message}");
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