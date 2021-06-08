using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Myoutlet.ge.Models;

namespace Myoutlet.ge.ViewModels
{
    public class ViewModel
    {
        public List<Category>Categories { get; set; }
        public Category Category { get; set; }
        public Description description { get; set; }
        public List<Kind> Kinds { get; set; }
        public List<Description> Descriptions { get; set; }
        public List<Gallery> Galleries { get; set; }
        public List<Partner> Partners { get; set; }
        public Partner Partner { get; set; }
        public List<Product> Products { get; set; }
        public List<Campaign> Campaigns { get; set; }
        public List<Add> Adds { get; set; }
        public List<Product> Recents { get; set; }
        public List<Gallery> RecentImgs { get; set; }
        public List<Product> HotDiscounts { get; set; }
        public List<int> DiscountsList { get; set; }
        public List<Gallery> HotImgs { get; set; }
        public List<Product> WeeklyDiscounts { get; set; }
        public List<Gallery> WeeklyImgs { get; set; }
        public List<Partner> ValuablePartners { get; set; }
        public FbLink FbLink { get; set; }
        public InstaLink InstaLink { get; set; }
        public YoutubeLink YoutubeLink { get; set; }
        public Product Product { get; set; }
        public List<Product> SimilarProducts { get; set; }
        public About info { get; set; }
        public List<Comment> Comments { get; set; }
        public List<callRequest> Requests { get; set; }

        public List<string> Details { get; set; }
        public string Total { get; set; }
    }
}