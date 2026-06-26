using FarmHelpAPI.Models;

namespace FarmHelpAPI.Data
{
    public static class DbInitializer
    {
        public static void Initialize(AppDbContext context)
        {
            context.Database.EnsureCreated();

            // Seed admin user
            if (!context.Users.Any(u => u.Role == "admin"))
            {
                var admin = new User
                {
                    Username = "admin",
                    Password = BCrypt.Net.BCrypt.HashPassword("admin123"),
                    Role = "admin",
                    Phone = "13800138000",
                    Address = "北京市朝阳区",
                    CreatedAt = DateTime.Now
                };
                context.Users.Add(admin);
                context.SaveChanges();
            }

            // Seed farmer users (with address)
            if (!context.Users.Any(u => u.Role == "farmer"))
            {
                var farmers = new List<User>
                {
                    new User { Username = "张农户", Password = BCrypt.Net.BCrypt.HashPassword("farmer123"), Role = "farmer", Phone = "13900139001", Address = "山东省济南市历城区王舍人镇张庄", CreatedAt = DateTime.Now },
                    new User { Username = "李农户", Password = BCrypt.Net.BCrypt.HashPassword("farmer123"), Role = "farmer", Phone = "13900139002", Address = "黑龙江省五常市龙凤山镇", CreatedAt = DateTime.Now },
                    new User { Username = "王农户", Password = BCrypt.Net.BCrypt.HashPassword("farmer123"), Role = "farmer", Phone = "13900139003", Address = "四川省成都市郫都区安德镇", CreatedAt = DateTime.Now }
                };
                context.Users.AddRange(farmers);
                context.SaveChanges();
            }

            // Seed consumer users (with address)
            if (!context.Users.Any(u => u.Role == "consumer"))
            {
                var consumers = new List<User>
                {
                    new User { Username = "消费者小刘", Password = BCrypt.Net.BCrypt.HashPassword("consumer123"), Role = "consumer", Phone = "13700137001", Address = "北京市海淀区中关村", CreatedAt = DateTime.Now },
                    new User { Username = "消费者小王", Password = BCrypt.Net.BCrypt.HashPassword("consumer123"), Role = "consumer", Phone = "13700137002", Address = "上海市浦东新区", CreatedAt = DateTime.Now }
                };
                context.Users.AddRange(consumers);
                context.SaveChanges();
            }

            // Get user IDs for seeding
            var farmerUsers = context.Users.Where(u => u.Role == "farmer").ToList();
            var consumerUsers = context.Users.Where(u => u.Role == "consumer").ToList();

            // Track whether we're seeding fresh products (to avoid FK issues with dependent data)
            var seededProducts = false;
            var seededProductList = new List<Product>();

            // Seed sample products (with origin and location)
            if (!context.Products.Any())
            {
                seededProducts = true;
                var products = new List<Product>
                {
                    new Product { Name = "有机西红柿", Category = "蔬菜水果", Price = 8.5m, Stock = 100, Description = "新鲜采摘的有机西红柿，无农药残留，口感酸甜可口", Images = "https://via.placeholder.com/200", Origin = "山东济南", Location = "济南市历城区王舍人镇", Status = "approved", FarmerId = farmerUsers.Count > 0 ? farmerUsers[0].Id : 1, CreatedAt = DateTime.Now },
                    new Product { Name = "农家土鸡蛋", Category = "肉类禽蛋", Price = 18.0m, Stock = 500, Description = "散养土鸡产蛋，蛋黄饱满，营养丰富", Images = "https://via.placeholder.com/200", Origin = "山东济南", Location = "济南市历城区王舍人镇", Status = "approved", FarmerId = farmerUsers.Count > 0 ? farmerUsers[0].Id : 1, CreatedAt = DateTime.Now },
                    new Product { Name = "五常大米", Category = "粮油米面", Price = 45.0m, Stock = 200, Description = "黑龙江五常优质大米，米粒饱满，香气浓郁", Images = "https://via.placeholder.com/200", Origin = "黑龙江五常", Location = "五常市龙凤山镇", Status = "approved", FarmerId = farmerUsers.Count > 1 ? farmerUsers[1].Id : 2, CreatedAt = DateTime.Now },
                    new Product { Name = "新鲜香菇", Category = "蔬菜水果", Price = 12.0m, Stock = 80, Description = "人工培育的新鲜香菇，肉质肥厚，味道鲜美", Images = "https://via.placeholder.com/200", Origin = "黑龙江五常", Location = "五常市龙凤山镇", Status = "approved", FarmerId = farmerUsers.Count > 1 ? farmerUsers[1].Id : 2, CreatedAt = DateTime.Now },
                    new Product { Name = "农家猪肉", Category = "肉类禽蛋", Price = 35.0m, Stock = 150, Description = "散养黑猪肉，肉质鲜嫩，肥瘦相间", Images = "https://via.placeholder.com/200", Origin = "四川成都", Location = "成都市郫都区安德镇", Status = "approved", FarmerId = farmerUsers.Count > 2 ? farmerUsers[2].Id : 3, CreatedAt = DateTime.Now },
                    new Product { Name = "山茶油", Category = "粮油米面", Price = 128.0m, Stock = 60, Description = "纯天然山茶籽油，营养健康，适合烹饪", Images = "https://via.placeholder.com/200", Origin = "四川成都", Location = "成都市郫都区安德镇", Status = "approved", FarmerId = farmerUsers.Count > 2 ? farmerUsers[2].Id : 3, CreatedAt = DateTime.Now }
                };
                context.Products.AddRange(products);
                context.SaveChanges();
                seededProductList = products;
            }

            // Get actual product IDs (either just seeded or existing) for dependent seed data
            var seedProductIds = seededProducts
                ? seededProductList.Select(p => p.Id).ToList()
                : context.Products.Select(p => p.Id).ToList();

            // Seed sample reviews (only if we have products to reference)
            if (!context.Reviews.Any() && seedProductIds.Count >= 4 && consumerUsers.Count > 0)
            {
                var reviews = new List<Review>
                {
                    new Review { ProductId = seedProductIds[0], UserId = consumerUsers[0].Id, Rating = 5, Content = "产品品质很好，口感不错！", CreatedAt = DateTime.Now },
                    new Review { ProductId = seedProductIds[1], UserId = consumerUsers[0].Id, Rating = 5, Content = "质量很好，值得购买。", CreatedAt = DateTime.Now },
                    new Review { ProductId = seedProductIds[2], UserId = consumerUsers[0].Id, Rating = 4, Content = "很香，煮饭特别好吃。", CreatedAt = DateTime.Now },
                    new Review { ProductId = seedProductIds[3], UserId = consumerUsers[0].Id, Rating = 5, Content = "很新鲜，肉质鲜嫩！", CreatedAt = DateTime.Now }
                };
                context.Reviews.AddRange(reviews);
                context.SaveChanges();
            }

            // Seed sample purchase demands (with contact info)
            if (!context.Purchases.Any())
            {
                var purchases = new List<Purchase>
                {
                    new Purchase { BuyerId = consumerUsers.Count > 0 ? consumerUsers[0].Id : 5, ProductType = "蔬菜水果", Quantity = 500, Budget = 5000, Deadline = DateTime.Now.AddDays(30), ContactName = "小刘", ContactPhone = "13700137001", Status = "active", CreatedAt = DateTime.Now },
                    new Purchase { BuyerId = consumerUsers.Count > 1 ? consumerUsers[1].Id : 6, ProductType = "粮油米面", Quantity = 1000, Budget = 8000, Deadline = DateTime.Now.AddDays(15), ContactName = "小王", ContactPhone = "13700137002", Status = "active", CreatedAt = DateTime.Now }
                };
                context.Purchases.AddRange(purchases);
                context.SaveChanges();
            }

            // Seed sample traces (only if we have products and traces don't exist yet)
            if (!context.Traces.Any() && seedProductIds.Count > 0)
            {
                var traces = new List<Trace>
                {
                    new Trace { ProductId = seedProductIds[0], NodeType = "planting", Location = "山东省济南市", Time = DateTime.Now.AddMonths(-3), Description = "开始种植", Operator = "张农户" },
                    new Trace { ProductId = seedProductIds[0], NodeType = "processing", Location = "山东省济南市", Time = DateTime.Now.AddMonths(-1), Description = "采摘包装", Operator = "张农户" },
                    new Trace { ProductId = seedProductIds[0], NodeType = "logistics", Location = "山东省济南市", Time = DateTime.Now.AddDays(-5), Description = "发货", Operator = "顺丰快递" }
                };
                context.Traces.AddRange(traces);
                context.SaveChanges();
            }

            // Seed sample trace codes (only if we have at least 2 products and trace codes don't exist yet)
            if (!context.TraceCodes.Any() && seedProductIds.Count >= 2)
            {
                var traceCodes = new List<TraceCode>
                {
                    new TraceCode { Code = "SN0001", ProductId = seedProductIds[0], CreatedAt = DateTime.Now },
                    new TraceCode { Code = "SN0002", ProductId = seedProductIds[1], CreatedAt = DateTime.Now }
                };
                context.TraceCodes.AddRange(traceCodes);
                context.SaveChanges();
            }
        }
    }
}
