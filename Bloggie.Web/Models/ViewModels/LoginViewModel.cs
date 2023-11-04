using System.ComponentModel.DataAnnotations;
using System.Security.Principal;

namespace Bloggie.Web.Models.ViewModels
{
	public class LoginViewModel
	{
		[Required]
		public string Username { get; set; }

		[Required]
		[MinLength(6, ErrorMessage = "password has to be at least 6 characters")]
		public string Password { get; set; }

		public string? ReturnUrl { get; set; }
	}
}
