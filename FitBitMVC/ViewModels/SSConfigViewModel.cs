using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FitBitMVC.ViewModels
{
    public class SSConfigViewModel
    {
        public IEnumerable<SelectListItem> ColumnNames { get; set; }

        [Display(Name = "Email Domain Column")]
        public string EmailDomain { get; set; }
        [Display(Name = "Append Email Domain?")]
        public bool AppendEmailDomain { get; set; }

        [Display(Name = "First Name Column")]
        public int FirstNameColumn { get; set; }
        [Display(Name = "Last Name Column")]
        public int LastNameColumn { get; set; }
        [Display(Name = "Email Address Column")]
        public int EmailAddress { get; set; }

        public string GroupName { get; set; }
        public int GroupID { get; set; }

        public int FileID { get; set; }

        public SSConfigViewModel()
        {
            FirstNameColumn = 2;
            LastNameColumn = 1;
            EmailAddress = 3;
            AppendEmailDomain = true;
            EmailDomain = "@my.unt.edu";
        }
    }
}