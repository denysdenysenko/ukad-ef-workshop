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

        public async Task<List<BestSalesPeopleModel>> GetBestSalesPeopleAsync(int top)
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

        public async Task<List<BestSalesPeopleModel>> GetBestSalesPeopleAggregateAsync(int top)
        {
            List<BestSalesPeopleModel> bestSalesPeople = await (from customer in _dbContext.Customers
                                                                from header in customer.SalesOrderHeaders.DefaultIfEmpty()
                                                                from detail in header.SalesOrderDetails.DefaultIfEmpty()
                                                                group new { customer, detail } by customer.SalesPerson into g
                                                                select new BestSalesPeopleModel
                                                                {
                                                                    SalesPerson = g.Key,
                                                                    TotalSold = g.Sum(i => i.detail.LineTotal),
                                                                })
                                                                .OrderByDescending(i => i.TotalSold)
                                                                .Take(top)
                                                                .ToListAsync();

            return bestSalesPeople;
        }

        public async Task CacheProductModelProductDescription(int top)
        {
            _productDescriptionCache = await _dbContext.ProductModelProductDescriptions
                                                       .Take(top)
                                                       .ToListAsync();
        }
    }
}
