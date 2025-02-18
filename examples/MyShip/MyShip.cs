﻿using System;
using System.IO;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using PitneyBowes.Developer.ShippingApi;
using PitneyBowes.Developer.ShippingApi.Model;
using PitneyBowes.Developer.ShippingApi.Fluent;

namespace MyShip
{
    class Program
    {
        static void Main(string[] args)
        {
            var sandbox = new Session() { EndPoint = "https://api-sandbox.pitneybowes.com", Requester = new ShippingApiHttpRequest() };

            var configs = new Dictionary<string, string>
                {
                    { "ApiKey", "YOUR_API_KEY" },
                    { "ApiSecret", "YOUR_API_SECRET" },
                    { "RatePlan", "YOUR_RATE_PLAN" },
                    { "ShipperID", "YOUR_SHIPPER_ID" },
                    { "DeveloperID", "YOUR_DEVELOPER_ID" }
                };
            var configurationBuilder = new ConfigurationBuilder();
            configurationBuilder
                .AddInMemoryCollection(configs)
                .AddJsonFile(Globals.GetConfigPath("shippingapisettings.json") , optional: true, reloadOnChange: true);
        
            sandbox.GetConfigItem = (c) => configurationBuilder.Build()[c];
            Model.RegisterSerializationTypes(sandbox.SerializationRegistry);
            Globals.DefaultSession = sandbox;

            var shipment = (Shipment)ShipmentFluent<Shipment>.Create()
                .ToAddress((Address)AddressFluent<Address>.Create()
                    .AddressLines("643 Greenway Rd")
                    .PostalCode("28607")
                    .CountryCode("US")
                    .Verify())
               .FromAddress((Address)AddressFluent<Address>.Create()
                    .Company("Pitney Bowes Inc")
                    .AddressLines("27 Waterview Drive")
                    .CityTown("Shelton").StateProvince("CT").PostalCode("06484")
                    .CountryCode("US")
                    )
               .Parcel((Parcel)ParcelFluent<Parcel>.Create()
                    .Dimension(12, 0.25M, 9)
                    .Weight(3m, UnitOfWeight.OZ))
               .Rates(RatesArrayFluent<Rates>.Create()
                    .USPSPriority<Rates, Parameter>()
                    .InductionPostalCode("06484")
                    )
               .Documents((List<IDocument>)DocumentsArrayFluent<Document>.Create()
                    .ShippingLabel(ContentType.BASE64, Size.DOC_4X6, FileFormat.PNG))
               .ShipmentOptions(ShipmentOptionsArrayFluent<ShipmentOptions>.Create()
                    .ShipperId(sandbox.GetConfigItem("ShipperID"))
                    .MinimalAddressvalidation()
                    )
               .TransactionId(Guid.NewGuid().ToString().Substring(15));

            shipment.IncludeDeliveryCommitment = true;
            var label = Api.CreateShipment(shipment).GetAwaiter().GetResult();
            if (label.Success)
            {
                var sw = new StreamWriter("label.pdf");
                foreach (var d in label.APIResponse.Documents)
                {
                    Api.WriteToStream(d, sw.BaseStream).GetAwaiter().GetResult();
                }
            }
        }
    }
}

