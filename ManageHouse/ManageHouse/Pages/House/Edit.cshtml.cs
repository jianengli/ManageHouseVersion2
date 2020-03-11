using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ManageHouse.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System.Net.Http;

namespace ManageHouse.Pages.House
{
    public class EditModel : PageModel
    {
        IHouseRepository _houseRepository;
        public List<int> stageList { get; set; }
        public List<Uri> imageList;
        //public List<List<Uri>> urlList = new List<List<Uri>>();
        public EditModel(IHouseRepository houseRepository)
        {
            _houseRepository = houseRepository;
            imageList = new List<Uri>();
        }

        [BindProperty]
        public Entity.House house { get; set; }

        public void OnGet(int id)
        {
 
            house = _houseRepository.GetHouse(id);
            stageList = _houseRepository.GetStage(house);
        }
        
        public IActionResult OnPost()
        {
            var data = house;
            if (ModelState.IsValid)
            {
                var count = _houseRepository.Update(data);
                if (count > 0)
                {
                    return RedirectToPage("/House/HouseList");
                }
            }
            return Page();
        }

        public IActionResult OnPostManage(int stagenumber,int id)
        {
            //System.Diagnostics.Debug.WriteLine($"xxxxxxxxxxxx");
            house = _houseRepository.GetHouse(id);
            //OnGet(id);
            CloudStorageAccount account = CloudStorageAccount.Parse(Constants.BlobKey);
                var blobClient = account.CreateCloudBlobClient();
                var imageContainer = blobClient.GetContainerReference(house.Name.Trim());
                var result = imageContainer.ListBlobsSegmentedAsync("", true, BlobListingDetails.None, 500, null, null, null).GetAwaiter().GetResult();
                var list = result.Results.Where(r => (r.Uri.ToString().Contains(".jpg") && r.Uri.ToString().Contains("stage" + stagenumber)) || (r.Uri.ToString().Contains(".png") && r.Uri.ToString().Contains("stage" + stagenumber))).ToList();
                foreach (var item in list)
                {
                System.Diagnostics.Debug.WriteLine($"Downloading {item.Uri}");
                    //var data = client.GetByteArrayAsync(item.Uri).GetAwaiter().GetResult();
                    imageList.Add(item.Uri);
                }
            return Page();
        }

    }
}
