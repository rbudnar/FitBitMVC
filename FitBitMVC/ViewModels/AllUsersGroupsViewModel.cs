using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FitBitMVC.Models;

namespace FitBitMVC.ViewModels
{
    public class AllUsersGroupsViewModel
    {
        //public List<FitbitUser> FBUsers { get; set; }
        //public List<Group> Groups { get; set; }
        //public List<int> SelectedUsers { get; set; } 
        public List<GroupForDeletionViewModel> ListOfAllGroups { get; set; }
    }

    public class GroupForDeletionViewModel
    {
        public int UniqueID { get; set; }
        public List<UserInGroupViewModel> Users { get; set; }
        public string GroupName { get; set; }
    }

    public class UserInGroupViewModel
    {
        public int UniqueID { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public bool IsSelected { get; set; }
    }

}