using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.Data.Entity;

namespace FitBitMVC.Models
{
    public class FitbitUser
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int UniqueID { get; set; }
        public string FitbitID { get; set; }
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
        public string Email { get; set; }

        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        public string FullName { get { return LastName + ", " + FirstName; } }

        [DataType(DataType.Date)]
        //[DisplayFormat(DataFormatString = "{0:yyyy-MM-dd", ApplyFormatInEditMode = true)]
        //[DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime TimeStamp { get; set; }
        //[Timestamp]
        //public byte[] TimeStamp { get; set; }

        public virtual IList<Group> Groups { get; set; }

        public FitbitUser()
        {
            TimeStamp = DateTime.Now;
        }
        
    }

    public class FitbitContext : DbContext
    {
        public DbSet<FitbitUser> FitbitUsers { get; set; }
        public DbSet<Group> Groups { get; set; }
        public DbSet<File>  Files { get; set; }
    }
}