using Microsoft.AspNetCore.Mvc;

namespace HRApplication.Models
{
    public class Experience
    {
        public string? organization { get; set; }
        public string? jobTitle { get; set; }
        public double? salary { get; set; }
        public DateTime from { get; set; }

        [Remote(action: "CheckDate", controller: "Home", AdditionalFields = "from" , ErrorMessage = "Experience end date must be after start date.")]
        public DateTime To { get; set; }
        public string? reasonOfLeaving { get; set; }
    
    }
}
