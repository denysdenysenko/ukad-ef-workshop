# Entity Framework performance diagnostic and tuning

## Before you get started
- It's recommended to watch [udemy course](https://www.udemy.com/course/entity-framework-core-tutorial) to grasp basic concepts.
- Make sure you have SQL Server Profiler installed. Re-run SQL Server installer package otherwise and make sure you have `Management Tools - Complete` option selected.
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
4. Start the project and repeat step 1, notice reduced number of memory objects. https://i.imgur.com/uBgswb7.png.
5. Stop the project.


### Database log feed
1. Remove `_bestSalesStub` variable declaration at the top of [HomeController.cs](Adventure.Web/Adventure.Web/Controllers/HomeController.cs).
2. Replace `_bestSalesStub` assignment within `Index` action with actual statistics service call `vm.BestSalesPeople = await _statisticsService.GetBestSalesPeople(10);`.
3. Set breakpoint at the beginning of `GetBestSalesPeople` method in [StatisticsService.cs](Adventure.Web/Adventure.Logic/Services/StatisticsService.cs) and start the project again.
4. Once you hit breakpoint, open the output window, clear it's content, press F10 and examine the newly added records. Stop the project.
5. Go to the [Startup.cs](Adventure.Web/Adventure.Web/Startup.cs) file, find a line with `AdventureDBContext` configuration and add `.LogTo(_=> Debug.WriteLine(_), LogLevel.Information)` to the configuration method chain, add missing `using` statements. Start the project again and repeat the previous step.
6. Examine the info message with actual SQL query performed. Press F10 a few more times while keeping output window open to see how lazy loading triggers new SQL statements to appear.

### SQL Profiler
1. Launch SQL Server Profiler.
2. 

### Include versus lazy loading
Check memory snapshot, check queries.

### Non-tracking queries
Check memory snapshot.

### Custom objects versus include
Check memory snapshot, check queries.
