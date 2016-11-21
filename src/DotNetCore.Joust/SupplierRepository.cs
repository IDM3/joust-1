using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace DotNetCore.Joust
{
    public class SupplierRepository
    {
        //list of all suppliers
        public List<Supplier> SupplierFile {get; set;}

        public SupplierRepository(string directoryOfSupplierFiles)
        {
            DirectoryInfo directory = new DirectoryInfo(directoryOfSupplierFiles);
            SupplierFile = new List<Supplier>();
            foreach(FileInfo file in directory.EnumerateFiles("*.csv"))
            {
                Supplier currentSupplier = new Supplier(file.DirectoryName + Path.DirectorySeparatorChar + file.Name);
                Func<Supplier, bool> isSameSupplier = (supplier) => currentSupplier.Name == supplier.Name;
                if(SupplierFile.Any(isSameSupplier))
                {
                    Supplier oldSupplier = SupplierFile.First(isSameSupplier);
                    if(oldSupplier.DateRecieved < currentSupplier.DateRecieved)
                    {
                        SupplierFile.Remove(oldSupplier);
                        SupplierFile.Add(currentSupplier);
                    }
                    else
                    {
                        //do nothing current supplier on file
                    }
                }
                else
                {
                    SupplierFile.Add(currentSupplier);
                }

            }
            
        }

        public List<Carpet> SearchInventory(Func<Carpet, bool> searchCriteria)
        {
            List<Carpet> carpetInventory = new List<Carpet>();
            foreach(Supplier sup in SupplierFile)
            {
                IEnumerable<Carpet> validInventory = sup.Inventory.Where(searchCriteria);
                if(validInventory != null && validInventory.Any())
                {
                    carpetInventory.AddRange(validInventory);
                }                
            }
            return carpetInventory;
        }
    }
}