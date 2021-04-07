# Entity Framework performance diagnostic and tuning

## Before you get started
- It's recommended to watch [udemy course](https://www.udemy.com/course/entity-framework-core-tutorial) to grasp basic concepts.
- Make sure you have Azure Data Studio witn SQL Server Profiler extension installed. Alternatively you can use SQL Server Profiler that comes with on premise SQL Server installation, to install it you can re-run SQL Server installer package and make sure you have `Management Tools - Complete` option selected.
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
3. Go to [StatisticsService.cs](Adventure.Web/Adventure.Logic/Services/StatisticsService.cs), insert `.AsNoTracking()` line in `CacheProductModelProductDescription` method. 
4. Start the project and repeat step 1, notice reduced number of memory objects. https://i.imgur.com/uBgswb7.png.
5. Stop the project.


### Database log feed
1. Remove `_bestSalesStub` variable declaration at the top of [HomeController.cs](Adventure.Web/Adventure.Web/Controllers/HomeController.cs).
2. Replace `_bestSalesStub` assignment within `Index` action with actual statistics service call `vm.BestSalesPeople = await _statisticsService.GetBestSalesPeopleAsync(10);`.
3. Set breakpoint at the beginning of `GetBestSalesPeopleAsync` method in [StatisticsService.cs](Adventure.Web/Adventure.Logic/Services/StatisticsService.cs) and start the project again.
4. Once you hit breakpoint, open the output window, clear it's content, press F10 and examine the newly added records. Stop the project.
5. Go to the [Startup.cs](Adventure.Web/Adventure.Web/Startup.cs) file, find a line with `AdventureDBContext` configuration and add `.LogTo(_=> Debug.WriteLine(_), LogLevel.Information)` to the configuration method chain, add missing `using` statements. Start the project again and repeat the previous step.
6. Examine the info message with actual SQL query performed. Press F10 a few more times while keeping output window open to see how lazy loading triggers new SQL statements to appear.
7. Remove the breakpoint. Stop debugging.

### SQL Profiler
1. Launch SQL Server Profiler. Start a new trace, enter connection details. Please notice that elevated DB permissions might be required in order to collect traces.
2. In trace properties, under `Use the template:` select `TSQL_Duration`. Press Run.
3. Revisit Trace Windows, notice enormous amount of SQL statements listed.
4. Notice that SQL Server Profiler can be used in many more cases, such as: deadlocks diagnostics, transactions durations, numbers of physical reads, statistics etc.
5. Clear trace window.

### Include versus lazy loading
1. Start the project and collect memory dump. Notice memory objects number and heap size. 
2. Open [AdventureDBContext.cs](Adventure.Web/Adventure.Model/Models/AdventureDBContext.cs) file, find `OnConfiguring` method and comment out of remove `UseLazyLoadingProxies` configuration.
3. Open [StatisticsService.cs](Adventure.Web/Adventure.Logic/Services/StatisticsService.cs) file, find `GetBestSalesPeopleAsync` method. Modify customers selection logic, extend it with additional include statements `.Include("SalesOrderHeaders.SalesOrderDetails")`. Optionally mark query as non-tracking.
4. Start the project again. Collect memory dump again and notice reduced number of objects created as well as reduced heap size.
5. Get back to SQL Server Profiler, notice a single SQL statement added to the traces window. 
6. Copy SQL statement and start SQL Server Management Studio, connect to the same DB, create a new query, execute copied statement.
7. Scroll through results, notice excessive customer data duplicated multiple times in order for Include statements to work correctly. Find customer `Metropolitan Bicycle Supply` and revisit duplicated data in `Customer` table columns.

### Custom objects versus include
1. Start the application, open developers tool, network tab. Hit refresh a few times, see how long does it take to load the document.
2. Stop the application, navigate to [HomeController.cs](Adventure.Web/Adventure.Web/Controllers/HomeController.cs) file, find `Index` action and replace `GetBestSalesPeopleAsync` method call with `GetBestSalesPeopleAggregateAsync` method call. 
3. Start project again and collect memory dump, notice reduced heap size and number of objects created.
4. Go to SQL Server Profiler and compare new SQL statement with the previous one, execute it in SQL Server Management studio to se how it affects total amount of data to be transferred between SQL Server and the app.
5. Open Dev Tools, hit refresh a few times and see how the update affected overall document load time.