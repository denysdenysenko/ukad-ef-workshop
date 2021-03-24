# Entity Framework performance diagnostic and tuning

## Before you get started
- It's recommended to watch [udemy course](https://www.udemy.com/course/entity-framework-core-tutorial) to grasp basic concepts.
- **Adventure.Web** project is created using ASP.NET Core Web APP (MVC) project template.
- **Adventure.Model** project is a .NET Class library. DB context is generated using `Scaffold-DbContext` comand out of a sample AdventureWorks Azure DB. For more information check [scaffolding tutorial](https://www.entityframeworktutorial.net/efcore/create-model-for-existing-database-in-ef-core.aspx).
- **Adventure.Logicc** project is a project representing aggregation summaries logic.
- Some additional project tuning included: removal of hardcoded connection strging from DB Context itself, adding connection string logic to `Startup.cs`, enabling lazy loading (to match EF 6).

## Workshop

### Setup
Clone the project. 
If not yet configured, install git tools. Open folder for the solution and execute `git clone https://github.com/denysdenysenko/ukad-ef-workshop`. 
Update connection string to point to Adventure Works sample DB. It can be your own DB installation or common installation provided by the host. If time permits, DB can be created during the workshop.

### Garbage collector
Notice hardcoded static collection `_bestSalesStub` consisting of 999 POCOs in [HomeController.cs](Adventure.Web/Adventure.Web/Controllers/HomeController.cs).
Start the project by pressing F5.
Go to **Diagnistic Tools**, **Memory Usage**, press **Take Snapshot**. Revisit memory objects, find 999 stub items.

### Tracking vs NoTracking
1. While keeping the project running go to https://localhost:44382/home/cache?top=500, after that take another memory snapshot.
Revisit memory objects, find 500 cached objects and supporting objects. https://i.imgur.com/YOHtZd2.png
2. Stop the project.
3. Go to [StatisticsService.cs](Adventure.Web/Adventure.Logic/Services/StatisticsService.cs), uncomment `.AsNoTracking()` line in `CacheProductModelProductDescription` method. 
4. Start the project and repeat step 1, notice reduced number of memory objects. https://i.imgur.com/uBgswb7.png


### SQL Profiler and Database log feed

### Include versus lazy loading
Check memory snapshot, check queries.

### Non-tracking queries
Check memory snapshot.

### Custom objects versus include
Check memory snapshot, check queries.
