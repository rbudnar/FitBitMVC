using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FitBitMVC.Models
{
    public class Group //: IEqualityComparer<Group>
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public bool Enabled { get; set; }

        public virtual ICollection<FitbitUser> Users { get; set; }
    }

}
