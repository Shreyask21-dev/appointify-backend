using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ConsultantDashboard.Core.DTOs
{
    public class PaymentResponseDTO
    {
        [JsonPropertyName("razorpay_order_id")]
        public string? OrderId { get; set; }

        [JsonPropertyName("razorpay_payment_id")]
        public string? PaymentId { get; set; }

        [JsonPropertyName("razorpay_signature")]
        public string? Signature { get; set; }
    }

}
