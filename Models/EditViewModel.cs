using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace IdentityApp.Models
{
	public class EditViewModel
	{
		public string? Id { get; set; }
		public string? FullName { get; set; }

		[EmailAddress]
		public string? Email { get; set; } 

		[DataType(DataType.Password)]
		public string? Password { get; set; }

		[DataType(DataType.Password)]
		[Compare(nameof(Password),ErrorMessage = "Parola eşleşmiyor.")] // ["Password"]
		public string? ConfirmPassword { get; set; }
        public IList<string>? SelectedRoles { get; set; }
    }
}
