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
    public class AddModel : PageModel
    {
        IHouseRepository _houseRepository;
        public AddModel(IHouseRepository houseRepository)
        {
            _houseRepository = houseRepository;
        }

        [BindProperty]
        public Entity.House house { get; set; }

        [TempData]
        public string Message { get; set; }
        public IActionResult OnGet()
        {
            return Page();
        }
        public IActionResult OnPost()
        {
            if (ModelState.IsValid)
            {
                var count = _houseRepository.Add(house);
                string containerName = house.Name.Trim();

                CloudStorageAccount account = CloudStorageAccount.Parse(Constants.BlobKey);
                var blobClient = account.CreateCloudBlobClient();
                CreateSampleContainerAsync(blobClient, containerName);

                if (count > 0)
                {
                    Message = "New House Added Successfully !";
                    return RedirectToPage("/House/HouseList");
                }
            }

            return Page();
        }

        private static async Task<CloudBlobContainer> CreateSampleContainerAsync(CloudBlobClient blobClient, String containerName)
        {

            CloudBlobContainer container = blobClient.GetContainerReference(containerName);

            try
            {
                // Create the container if it does not already exist.
                bool result = await container.CreateIfNotExistsAsync();
                SetPublicContainerPermissions(container);
                if (result == true)
                {
                    Console.WriteLine("Created container {0}", container.Name);
                }
            }
            catch (StorageException e)
            {
                Console.WriteLine("HTTP error code {0}: {1}",
                                    e.RequestInformation.HttpStatusCode,
                                    e.RequestInformation.ErrorCode);
                Console.WriteLine(e.Message);
            }

            return container;
        }

        private static async Task SetPublicContainerPermissions(CloudBlobContainer container)
        {
            BlobContainerPermissions permissions = await container.GetPermissionsAsync();
            permissions.PublicAccess = BlobContainerPublicAccessType.Container;
            await container.SetPermissionsAsync(permissions);

            Console.WriteLine("Container {0} - permissions set to {1}", container.Name, permissions.PublicAccess);
        }
    }
}
