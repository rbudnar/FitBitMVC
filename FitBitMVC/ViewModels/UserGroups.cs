using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FitBitMVC.Models;

namespace FitBitMVC.ViewModels
{
    public class UserGroupViewModel
    {
        public FitbitUser FBUser { get; set; }
        public Group FbGroup { get; set; } //for editing single groups
        public List<Group> GroupsInList { get; set; }
        public List<Group> GroupsNotInList { get; set; }
        public int[] GroupsIn { get; set; }
        public int[] GroupsNotIn { get; set; }


        public UserGroupViewModel()
        {
            FBUser = new FitbitUser();
            FbGroup = new Group();
        }
    }
}