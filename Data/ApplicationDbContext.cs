namespace CinemaDashboard.Data
{
    // Inherits IdentityDbContext so Identity tables are created alongside app tables
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<Category>      Categories    { get; set; }
        public DbSet<Cinema>        Cinemas       { get; set; }
        public DbSet<Actor>         Actors        { get; set; }
        public DbSet<Movie>         Movies        { get; set; }
        public DbSet<MovieSubImage> MovieSubImages { get; set; }
        public DbSet<MovieActor>    MovieActors   { get; set; }
        public DbSet<Booking>       Bookings      { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder); // must call base for Identity tables

            modelBuilder.Entity<Category>().HasData(
                new Category { Id = 1, Name = "Action",  Description = "High energy action movies",       Status = true  },
                new Category { Id = 2, Name = "Drama",   Description = "Emotional and story-driven",      Status = true  },
                new Category { Id = 3, Name = "Comedy",  Description = "Light-hearted and funny",         Status = true  },
                new Category { Id = 4, Name = "Horror",  Description = "Scary and thrilling movies",      Status = false },
                new Category { Id = 5, Name = "Sci-Fi",  Description = "Science fiction and futurism",    Status = true  }
            );

            modelBuilder.Entity<Cinema>().HasData(
                new Cinema { Id = 1, Name = "Grand Cinema Cairo",      Description = "Biggest cinema in Cairo",       Img = "1.jpg", Status = true  },
                new Cinema { Id = 2, Name = "Stars Cinema Alex",       Description = "Located in Alexandria",         Img = "2.jpg", Status = true  },
                new Cinema { Id = 3, Name = "Nova Screens Giza",       Description = "Modern screens in Giza",        Img = "3.jpg", Status = true  },
                new Cinema { Id = 4, Name = "CinePlex Maadi",          Description = "Family-friendly cinema",        Img = "4.jpg", Status = false },
                new Cinema { Id = 5, Name = "Royal Cinema Heliopolis", Description = "Premium experience",            Img = "5.jpg", Status = true  }
            );

            modelBuilder.Entity<Actor>().HasData(
                new Actor { Id = 1, Name = "Ahmed Ezz",          Bio = "Famous Egyptian action actor",     Img = "1.jpg", Status = true  },
                new Actor { Id = 2, Name = "Mona Zaki",          Bio = "Renowned Egyptian drama actress",  Img = "2.jpg", Status = true  },
                new Actor { Id = 3, Name = "Karim Abdel Aziz",   Bio = "Versatile Egyptian movie star",    Img = "3.jpg", Status = true  },
                new Actor { Id = 4, Name = "Hend Sabry",         Bio = "Award-winning actress",            Img = "4.jpg", Status = true  },
                new Actor { Id = 5, Name = "Ahmed Helmy",        Bio = "Top Egyptian comedy actor",        Img = "5.jpg", Status = true  },
                new Actor { Id = 6, Name = "Nelly Karim",        Bio = "Acclaimed drama actress",          Img = "6.jpg", Status = false }
            );

            modelBuilder.Entity<Movie>().HasData(
                new Movie { Id = 1,  Name = "Desert Storm",      Description = "Epic action in the desert",    Price = 120, Status = true,  DateTime = new DateTime(2025,6,15),  MainImg = "1.jpg",  CategoryId = 1, CinemaId = 1 },
                new Movie { Id = 2,  Name = "Broken Hearts",     Description = "A love story gone wrong",      Price = 95,  Status = true,  DateTime = new DateTime(2025,7,20),  MainImg = "2.jpg",  CategoryId = 2, CinemaId = 2 },
                new Movie { Id = 3,  Name = "Laugh Factory",     Description = "Non-stop comedy",              Price = 80,  Status = true,  DateTime = new DateTime(2025,8,1),   MainImg = "3.jpg",  CategoryId = 3, CinemaId = 3 },
                new Movie { Id = 4,  Name = "Dark Shadows",      Description = "Horror thriller",              Price = 100, Status = false, DateTime = new DateTime(2025,9,10),  MainImg = "4.jpg",  CategoryId = 4, CinemaId = 4 },
                new Movie { Id = 5,  Name = "Galaxy Quest",      Description = "Sci-fi adventure",             Price = 150, Status = true,  DateTime = new DateTime(2025,10,5),  MainImg = "5.jpg",  CategoryId = 5, CinemaId = 5 },
                new Movie { Id = 6,  Name = "City of Fire",      Description = "Action in the streets",        Price = 110, Status = true,  DateTime = new DateTime(2025,11,12), MainImg = "6.jpg",  CategoryId = 1, CinemaId = 1 },
                new Movie { Id = 7,  Name = "Silent Tears",      Description = "Emotional family drama",       Price = 90,  Status = true,  DateTime = new DateTime(2025,12,3),  MainImg = "7.jpg",  CategoryId = 2, CinemaId = 2 },
                new Movie { Id = 8,  Name = "Just Kidding",      Description = "Family comedy for all ages",   Price = 75,  Status = true,  DateTime = new DateTime(2026,1,18),  MainImg = "8.jpg",  CategoryId = 3, CinemaId = 3 },
                new Movie { Id = 9,  Name = "The Awakening",     Description = "Supernatural horror",          Price = 105, Status = false, DateTime = new DateTime(2026,2,14),  MainImg = "9.jpg",  CategoryId = 4, CinemaId = 4 },
                new Movie { Id = 10, Name = "Beyond the Stars",  Description = "Space exploration epic",       Price = 160, Status = true,  DateTime = new DateTime(2026,3,22),  MainImg = "10.jpg", CategoryId = 5, CinemaId = 5 }
            );

            modelBuilder.Entity<MovieActor>().HasData(
                new MovieActor { MovieId = 1, ActorId = 1 }, new MovieActor { MovieId = 1, ActorId = 3 },
                new MovieActor { MovieId = 2, ActorId = 2 }, new MovieActor { MovieId = 2, ActorId = 4 },
                new MovieActor { MovieId = 3, ActorId = 5 }, new MovieActor { MovieId = 4, ActorId = 3 },
                new MovieActor { MovieId = 5, ActorId = 1 }, new MovieActor { MovieId = 6, ActorId = 1 },
                new MovieActor { MovieId = 7, ActorId = 2 }, new MovieActor { MovieId = 8, ActorId = 5 },
                new MovieActor { MovieId = 9, ActorId = 6 }, new MovieActor { MovieId = 10, ActorId = 3 }
            );

            modelBuilder.Entity<Booking>().HasData(
                new Booking { Id = 1, CustomerName = "Ahmed Ali",    CustomerPhone = "01012345678", MovieId = 1, Seats = 2, BookedAt = new DateTime(2025,6,10)  },
                new Booking { Id = 2, CustomerName = "Sara Mohamed", CustomerPhone = "01198765432", MovieId = 2, Seats = 1, BookedAt = new DateTime(2025,7,15)  },
                new Booking { Id = 3, CustomerName = "Khaled Hassan",CustomerPhone = "01234567890", MovieId = 3, Seats = 3, BookedAt = new DateTime(2025,8,1)   },
                new Booking { Id = 4, CustomerName = "Ahmed Ali",    CustomerPhone = "01012345678", MovieId = 5, Seats = 2, BookedAt = new DateTime(2025,10,2)  },
                new Booking { Id = 5, CustomerName = "Nour Tarek",   CustomerPhone = "01556677889", MovieId = 6, Seats = 4, BookedAt = new DateTime(2025,11,10) }
            );
        }
    }
}
