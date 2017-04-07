using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace SampleClient.Models
{
    public class ClientViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage ="Client Id is required")]
        public string ClientId { get; set; }

        [Required(ErrorMessage = "Client Name is required")]
        public string ClientName { get; set; }

        [Required(ErrorMessage = "Scopes are required")]
        public string Scopes { get; set; }

        [Required(ErrorMessage = "Redirect Uri is required")]
        public string RedirectUris { get; set; }

        [Required(ErrorMessage = "Post Redirect Uri is required")]
        public string PostRedirectUris { get; set; }
    }
}