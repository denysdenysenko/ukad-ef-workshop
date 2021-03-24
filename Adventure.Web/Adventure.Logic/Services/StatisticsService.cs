using Adventure.Logic.Models;
using Adventure.Model.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Adventure.Logic.Services
{
    public class StatisticsService
    {
        private static List<ProductModelProductDescription> _productDescriptionCache;
        private readonly AdventureDBContext _dbContext;

        public StatisticsService(AdventureDBContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<BestSalesPeopleModel>> GetBestSalesPeople(int top)
        {
            var customers = await _dbContext.Customers.ToListAsync();
            Dictionary<string, decimal> salesPeople = new Dictionary<string, decimal>();
            foreach (var customer in customers)
            {
                if (!salesPeople.ContainsKey(customer.SalesPerson))
                {
                    salesPeople[customer.SalesPerson] = 0;
                }
                foreach (var salesOrder in customer.SalesOrderHeaders)
                {
                    foreach (var detail in salesOrder.SalesOrderDetails)
                    {
                        salesPeople[customer.SalesPerson] += detail.LineTotal;
                    }
                }
            }

            return salesPeople.OrderByDescending(so => so.Value)
                              .Take(top)
                              .Select(so => new BestSalesPeopleModel
                              {
                                  SalesPerson = so.Key,
                                  TotalSold = so.Value,
                              })
                              .ToList();
        }

        public async Task CacheProductModelProductDescription(int top)
        {
            _productDescriptionCache = await _dbContext.ProductModelProductDescriptions
                                                       //.AsNoTracking()
                                                       .Take(top)
                                                       .ToListAsync();
        }
    }
}
