using System;
using System.ComponentModel.DataAnnotations;

namespace bankaccount.Models
{
    public class Login_User
    {
        [Key]
        public int id {get;set;}
        [Required]
        [EmailAddress]
        public string email{get;set;}
        [Required]
        [DataType(DataType.Password)]
        public string password{get;set;}

    }
}