using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ManageHouse.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;

namespace ManageHouse.Pages.House
{
    public class IndexModel : PageModel
    {
        IHouseRepository _houseRepository;
        public IndexModel(IHouseRepository houseRepository)
        {
            _houseRepository = houseRepository;
        }

        [BindProperty]
        public List<Entity.House> houseList { get; set; }

        [BindProperty]
        public Entity.House house { get; set; }

        [TempData]
        public string Message { get; set; }
        public void OnGet()
        {
            houseList = _houseRepository.GetList();
        }

        public IActionResult OnPostDelete(int id)
        {
            house = _houseRepository.GetHouse(id);
            string containerName = house.Name.Trim();
            var count = _houseRepository.Delete(id);
            CloudStorageAccount account = CloudStorageAccount.Parse(Constants.BlobKey);
            var blobClient = account.CreateCloudBlobClient();
            DeleteSampleContainerAsync(blobClient, containerName);
            if (count > 0)
            {
                Message = "House Deleted Successfully !";
                return RedirectToPage("/House/Index");
            }
            return Page();

        }

        private static async Task DeleteSampleContainerAsync(CloudBlobClient blobClient, string containerName)
        {
            CloudBlobContainer container = blobClient.GetContainerReference(containerName);
            try
            {
                // Delete the specified container and handle the exception.
                await container.DeleteAsync();
            }
            catch (StorageException e)
            {
                Console.WriteLine("HTTP error code {0}: {1}",
                                    e.RequestInformation.HttpStatusCode,
                                    e.RequestInformation.ErrorCode);
                Console.WriteLine(e.Message);
                Console.ReadLine();
            }
        }
    }
}
