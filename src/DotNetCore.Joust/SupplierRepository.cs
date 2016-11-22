using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace DotNetCore.Joust
{
    /// <summary>
    /// Repository of all latest suppliers
    /// </summary>
    public class SupplierRepository
    {
        /// <summary>
        /// list of of latested <see cref="Supplier"/> 
        /// </summary>
        public List<Supplier> SupplierFile {get; set;}

        /// <summary>
        /// Creatse supppliery repository based on directory containing csv files for suppliers
        /// </summary>
        /// <param name="directoryOfSupplierFiles">directory of suppliers</param>
        public SupplierRepository(string directoryOfSupplierFiles)
        {
            //get directory info so we can find file info
            DirectoryInfo directory = new DirectoryInfo(directoryOfSupplierFiles);
            SupplierFile = new List<Supplier>();
            //go through each *.csv file in directory
            foreach(FileInfo file in directory.EnumerateFiles("*.csv"))
            {
                //create new supplier from file
                Supplier currentSupplier = new Supplier(file.DirectoryName + Path.DirectorySeparatorChar + file.Name);
                //function to check if suppliers have same name and are thus same supplier
                Func<Supplier, bool> isSameSupplier = (supplier) => currentSupplier.Name == supplier.Name;
                //does the respository arleady have this suppler on file
                if(SupplierFile.Any(isSameSupplier))
                {
                    //if so get the old one on file
                    Supplier oldSupplier = SupplierFile.First(isSameSupplier);
                    //is the current supplier more recent than the one we had on file
                    if(oldSupplier.DateRecieved < currentSupplier.DateRecieved)
                    {
                        //if so remove the old one
                        SupplierFile.Remove(oldSupplier);
                        //add the new one
                        SupplierFile.Add(currentSupplier);
                    }
                    else
                    {
                        //do nothing the old supplier on file is the latest file copy
                    }
                }
                else
                {
                    //else add supplier to file
                    SupplierFile.Add(currentSupplier);
                }

            }
            
        }

        /// <summary>
        /// Search all suppliers <see cref="Carpet"/> inventory for something matching search criteria 
        /// </summary>
        /// <param name="searchCriteria">if this returns true the <see cref="Carpet"/> will be returned in result</param>
        /// <returns>list of <see cref="Carpet"/> matching search criteria, if none match it will be empty</returns>
        public List<Carpet> SearchInventory(Func<Carpet, bool> searchCriteria)
        {
            //start initial list of matching carpets as empty
            List<Carpet> carpetInventory = new List<Carpet>();
            //go through each supplier
            foreach(Supplier sup in SupplierFile)
            {
                //find all inventory of supplier that matches criteria
                IEnumerable<Carpet> validInventory = sup.Inventory.Where(searchCriteria);
                //do any exist
                if(validInventory != null && validInventory.Any())
                {
                    //if so add inventory to list
                    carpetInventory.AddRange(validInventory);
                }                
                else
                {
                    //else no invtory to add
                }
            }
            //returns empty list or list containing matching carpets
            return carpetInventory;
        }
    }
}