using AquariumForum_2.Data;
using AquariumForum_2.Models;

namespace AquariumForum_2.ViewModels
{
    public class UserProfileViewModel
    {
        public ApplicationUser User { get; set; }
        public List<Discussion> Discussions { get; set; }
    }
}
