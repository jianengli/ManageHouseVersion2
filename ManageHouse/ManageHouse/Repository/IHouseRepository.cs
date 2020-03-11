using ManageHouse.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ManageHouse.Repository
{

    public interface IHouseRepository 
    {
        int Add(House house); 

        List<House> GetList();  // NOTE: (kbg) Would have just named it list

        House GetHouse(int id); // NOTE: (kbg) Would have just named it Get or Fetch

        int Update(House house); 

        int Delete(int id); 

        List<int> GetStage(House house); 
    }
}
