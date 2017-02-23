using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;


namespace Rgen.UAT.UATToolServiceLayer.Models
{
    public class UserRegModel
    {
        [Key]
        public int? UserId { get; set; }
        // All the other properties.


        [Display(Name = "FirstName")]
        [RegularExpression(@"^[a-zA-Z'.\s]{1,40}$", ErrorMessage = "Special Characters not allowed")]
        public string FirstName { get; set; }


        [Display(Name = "LastName")]
        [RegularExpression(@"^[a-zA-Z'.\s]{1,40}$", ErrorMessage = "Special Characters not allowed")]
        public string LastName { get; set; }


        [DataType(DataType.EmailAddress)]
        [Display(Name = "Email")]
        [Required(ErrorMessage = "Please provide Email", AllowEmptyStrings = false)]
        [RegularExpression(@"^([0-9a-zA-Z]([\+\-_\.][0-9a-zA-Z]+)*)+@(([0-9a-zA-Z][-\w]*[0-9a-zA-Z]*\.)+[a-zA-Z0-9]{2,3})$", ErrorMessage = "Please provide valid Email id")]
        public string Email { get; set; }


        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        [Required(ErrorMessage = "Please provide  password", AllowEmptyStrings = false)]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Password must be 2 char long.")]
        public string Password { get; set; }


        [DataType(DataType.Password)]
        [Display(Name = "Confirm Password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        [Required(ErrorMessage = "Please provide confirmation password", AllowEmptyStrings = false)]
        public string ConfirmPassword { get; set; }



        [Display(Name = "DOB")]
        [DataType(DataType.Date)]
        public DateTime DOB { get; set; }


        [Display(Name = "Gender")]
        public string Gender { get; set; }

        [Display(Name = "MobileNo")]
        [StringLength(13, MinimumLength = 10)]
        public string MobileNo { get; set; }


        [Display(Name = "Country")]
        public string Country { get; set; }
    }
}
