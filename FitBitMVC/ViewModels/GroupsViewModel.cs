using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FitBitMVC.Models;

namespace FitBitMVC.ViewModels
{
    public class GroupsViewModel
    {
        private readonly List<Group> _groups;

        [Display(Name = "Add to group")]
        public int SelectedGroup { get; set; }
        public IEnumerable<SelectListItem> Groups => new SelectList(_groups, "ID", "Name");

        public GroupsViewModel()
        {
            FitbitContext db = new FitbitContext();
            _groups = db.Groups.ToList();
        }

        public GroupsViewModel(FitbitContext db)
        {
            _groups = db.Groups.ToList();
        }

    }

}