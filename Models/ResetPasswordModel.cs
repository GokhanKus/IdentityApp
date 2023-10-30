using System.ComponentModel.DataAnnotations;

namespace IdentityApp.Models
{
	public class ResetPasswordModel
	{
		[Required]
		public string Token { get; set; } = string.Empty;

		//[Required]
		[DataType(DataType.EmailAddress)]
		public string Email { get; set; } = string.Empty;

		[Required]
		[DataType(DataType.Password)]
		public string Password { get; set; } = string.Empty;

		[Required]
		[DataType(DataType.Password)]
		[Compare(nameof(Password), ErrorMessage = "Parola eşleşmiyor.")] // ["Password"]
		public string ConfirmPassword { get; set; } = string.Empty;
	}
}
