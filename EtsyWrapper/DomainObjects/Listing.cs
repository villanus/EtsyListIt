using System.Collections.Generic;
using System.Runtime.Serialization;

namespace EtsyWrapper.DomainObjects
{
    [DataContract]
    public class Listing
    {
        [DataMember(Name = "listing_id")]
        public string ID { get; set; }
        [DataMember(Name = "category_id")]
        public string CategoryID;
        [DataMember(Name = "title")]
        public string Title { get; set; }
        [DataMember(Name = "description")]
        public string Description { get; set; }

        [DataMember(Name = "state")]
        public string State { get; set; }

        [DataMember(Name = "quantity")]
        public string Quantity { get; set; }
        [DataMember(Name = "price")]
        public decimal Price { get; set; }
        [DataMember(Name = "is_supply")]
        public bool IsSupply { get; set; }
        [DataMember(Name = "when_made")]
        public string WhenMade { get; set; }
        [DataMember(Name = "who_made")]
        public string WhoMade { get; set; }
        [DataMember(Name = "is_digital")]
        public bool IsDigital { get; set; }
        [DataMember(Name = "shipping_template_id")]
        public string ShippingTemplateID { get; set; }
        [DataMember(Name = "is_customizable")]
        public bool IsCustomizable { get; set; }
        [DataMember(Name = "tags")]
        public string[] Tags { get; set; }
        [DataMember(Name = "image_ids")]
        public List<string> ImageIds { get; set; }

        public ListingImage[] Images { get; set; }
        public DigitalFile[] DigitalFiles { get; set; }
        public bool Activate { get; set; }
    }
}