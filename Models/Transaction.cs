using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace bankaccount.Models
{
    public class Transaction
    {
        [Key]
        public int TransactionId {get;set;}
        [Required]
        public decimal Amount {get;set;}
        public DateTime created_at {get;set;} = DateTime.Now;
        public int UserId {get;set;}
        [ForeignKey("UserId")]
        public User Handler {get;set;}

    }
}