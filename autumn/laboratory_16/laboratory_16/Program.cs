using System;
using System.Collections.Generic;
using System.Linq;

namespace AirlineSystem
{
    public enum UserRole
    {
        Guest,
        User,
        Dispatcher,
        Administrator
    }

    public enum CrewRole
    {
        Pilot,
        CoPilot,
        Navigator,
        RadioOperator,
        FlightAttendant
    }

    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public UserRole Role { get; set; }

        private static int _nextId = 1;

        public User(string username, string password, string email, UserRole role)
        {
            Id = _nextId++;
            Username = username;
            Password = password;
            Email = email;
            Role = role;
        }

        public bool Login(string username, string password)
        {
            return Username == username && Password == password;
        }

        public void Logout()
        {
            Console.WriteLine($"User {Username} logged out");
        }
    }

    public class Flight
    {
        public int FlightId { get; set; }
        public string FlightNumber { get; set; }
        public string DepartureAirport { get; set; }
        public string ArrivalAirport { get; set; }
        public DateTime DepartureTime { get; set; }
        public DateTime ArrivalTime { get; set; }
        public string AircraftType { get; set; }
        public Crew AssignedCrew { get; set; }

        private static int _nextFlightId = 1;

        public Flight(string flightNumber, string departure, string arrival,
                     DateTime depTime, DateTime arrTime, string aircraft)
        {
            FlightId = _nextFlightId++;
            FlightNumber = flightNumber;
            DepartureAirport = departure;
            ArrivalAirport = arrival;
            DepartureTime = depTime;
            ArrivalTime = arrTime;
            AircraftType = aircraft;
        }

        public void UpdateFlightInfo(string departure, string arrival,
                                    DateTime depTime, DateTime arrTime)
        {
            DepartureAirport = departure;
            ArrivalAirport = arrival;
            DepartureTime = depTime;
            ArrivalTime = arrTime;
            Console.WriteLine($"Flight {FlightNumber} updated");
        }

        public void DisplayFlightInfo()
        {
            Console.WriteLine($"Flight {FlightNumber}: {DepartureAirport} -> {ArrivalAirport}");
            Console.WriteLine($"Departure: {DepartureTime}, Arrival: {ArrivalTime}");
            Console.WriteLine($"Aircraft: {AircraftType}");
        }
    }

    public class CrewMember
    {
        public int EmployeeId { get; set; }
        public string Name { get; set; }
        public CrewRole Role { get; set; }
        public string LicenseNumber { get; set; }
        public List<string> Certifications { get; set; }

        private static int _nextEmployeeId = 1;

        public CrewMember(string name, CrewRole role, string license)
        {
            EmployeeId = _nextEmployeeId++;
            Name = name;
            Role = role;
            LicenseNumber = license;
            Certifications = new List<string>();
        }

        public void AddCertification(string certification)
        {
            Certifications.Add(certification);
            Console.WriteLine($"Certification '{certification}' added to {Name}");
        }

        public void DisplayInfo()
        {
            Console.WriteLine($"{Name} - {Role} (License: {LicenseNumber})");
        }
    }

    public class Crew
    {
        public int CrewId { get; set; }
        public List<CrewMember> Members { get; set; }
        public int FlightId { get; set; }

        private static int _nextCrewId = 1;

        public Crew()
        {
            CrewId = _nextCrewId++;
            Members = new List<CrewMember>();
        }

        public void AddMember(CrewMember member)
        {
            Members.Add(member);
            Console.WriteLine($"Added {member.Name} to crew {CrewId}");
        }

        public void RemoveMember(CrewMember member)
        {
            Members.Remove(member);
            Console.WriteLine($"Removed {member.Name} from crew {CrewId}");
        }

        public bool ValidateCrewComposition()
        {
            var pilots = Members.Count(m => m.Role == CrewRole.Pilot || m.Role == CrewRole.CoPilot);
            var flightAttendants = Members.Count(m => m.Role == CrewRole.FlightAttendant);

            bool isValid = pilots >= 2 && flightAttendants >= 1;
            Console.WriteLine($"Crew composition validation: {isValid}");
            return isValid;
        }

        public void DisplayCrew()
        {
            Console.WriteLine($"Crew {CrewId} for Flight {FlightId}:");
            foreach (var member in Members)
            {
                Console.WriteLine($"  - {member.Name} ({member.Role})");
            }
        }
    }

    public class AuthenticationService
    {
        private List<User> _users;
        public User CurrentUser { get; private set; }

        public AuthenticationService()
        {
            _users = new List<User>();
            // Добавляем тестовых пользователей
            _users.Add(new User("admin", "admin123", "admin@airline.com", UserRole.Administrator));
            _users.Add(new User("dispatcher", "dispatch123", "dispatch@airline.com", UserRole.Dispatcher));
            _users.Add(new User("user", "user123", "user@airline.com", UserRole.User));
        }

        public User Login(string username, string password)
        {
            var user = _users.FirstOrDefault(u => u.Username == username && u.Password == password);
            if (user != null)
            {
                CurrentUser = user;
                Console.WriteLine($"User {username} successfully logged in");
            }
            else
            {
                Console.WriteLine("Login failed: invalid credentials");
            }
            return user;
        }

        public void Logout()
        {
            if (CurrentUser != null)
            {
                Console.WriteLine($"User {CurrentUser.Username} logged out");
                CurrentUser = null;
            }
        }

        public bool Register(User user)
        {
            if (_users.Any(u => u.Username == user.Username))
            {
                Console.WriteLine("Registration failed: username already exists");
                return false;
            }

            _users.Add(user);
            Console.WriteLine($"User {user.Username} successfully registered");
            return true;
        }

        public bool HasPermission(UserRole requiredRole)
        {
            return CurrentUser != null && CurrentUser.Role >= requiredRole;
        }
    }

    public class FlightService
    {
        private List<Flight> _flights;

        public FlightService()
        {
            _flights = new List<Flight>();
        }

        public void AddFlight(Flight flight)
        {
            _flights.Add(flight);
            Console.WriteLine($"Flight {flight.FlightNumber} added to system");
        }

        public bool RemoveFlight(int flightId)
        {
            var flight = _flights.FirstOrDefault(f => f.FlightId == flightId);
            if (flight != null)
            {
                _flights.Remove(flight);
                Console.WriteLine($"Flight {flight.FlightNumber} removed from system");
                return true;
            }
            Console.WriteLine($"Flight with ID {flightId} not found");
            return false;
        }

        public Flight GetFlight(int flightId)
        {
            return _flights.FirstOrDefault(f => f.FlightId == flightId);
        }

        public List<Flight> GetAllFlights()
        {
            return _flights;
        }

        public void UpdateFlight(Flight updatedFlight)
        {
            var existingFlight = _flights.FirstOrDefault(f => f.FlightId == updatedFlight.FlightId);
            if (existingFlight != null)
            {
                existingFlight.UpdateFlightInfo(
                    updatedFlight.DepartureAirport,
                    updatedFlight.ArrivalAirport,
                    updatedFlight.DepartureTime,
                    updatedFlight.ArrivalTime
                );
            }
            else
            {
                Console.WriteLine($"Flight with ID {updatedFlight.FlightId} not found");
            }
        }

        public void DisplayAllFlights()
        {
            Console.WriteLine("\n=== ALL FLIGHTS ===");
            foreach (var flight in _flights)
            {
                flight.DisplayFlightInfo();
                Console.WriteLine("---");
            }
        }
    }

    public class CrewService
    {
        private List<CrewMember> _availableCrewMembers;
        private List<Crew> _crews;

        public CrewService()
        {
            _availableCrewMembers = new List<CrewMember>();
            _crews = new List<Crew>();

            // Добавляем тестовых членов экипажа
            AddCrewMember(new CrewMember("John Smith", CrewRole.Pilot, "P12345"));
            AddCrewMember(new CrewMember("Maria Garcia", CrewRole.CoPilot, "CP67890"));
            AddCrewMember(new CrewMember("Robert Johnson", CrewRole.Navigator, "N54321"));
            AddCrewMember(new CrewMember("Lisa Chen", CrewRole.RadioOperator, "RO98765"));
            AddCrewMember(new CrewMember("Anna Kowalski", CrewRole.FlightAttendant, "FA11111"));
            AddCrewMember(new CrewMember("David Brown", CrewRole.FlightAttendant, "FA22222"));
        }

        public void AddCrewMember(CrewMember member)
        {
            _availableCrewMembers.Add(member);
            Console.WriteLine($"Crew member {member.Name} added to available pool");
        }

        public Crew CreateCrewForFlight(int flightId)
        {
            var crew = new Crew { FlightId = flightId };
            _crews.Add(crew);
            Console.WriteLine($"New crew created for flight {flightId} (Crew ID: {crew.CrewId})");
            return crew;
        }

        public List<CrewMember> GetAvailableCrewMembers()
        {
            return _availableCrewMembers;
        }

        public CrewMember GetCrewMember(int employeeId)
        {
            return _availableCrewMembers.FirstOrDefault(m => m.EmployeeId == employeeId);
        }

        public bool AssignCrewToFlight(int crewId, int flightId, FlightService flightService)
        {
            var crew = _crews.FirstOrDefault(c => c.CrewId == crewId);
            var flight = flightService.GetFlight(flightId);

            if (crew != null && flight != null)
            {
                if (crew.ValidateCrewComposition())
                {
                    flight.AssignedCrew = crew;
                    Console.WriteLine($"Crew {crewId} successfully assigned to flight {flightId}");
                    return true;
                }
                else
                {
                    Console.WriteLine("Cannot assign crew: invalid crew composition");
                    return false;
                }
            }

            Console.WriteLine("Cannot assign crew: crew or flight not found");
            return false;
        }

        public void DisplayAvailableCrew()
        {
            Console.WriteLine("\n=== AVAILABLE CREW MEMBERS ===");
            foreach (var member in _availableCrewMembers)
            {
                member.DisplayInfo();
            }
        }

        public void DisplayAllCrews()
        {
            Console.WriteLine("\n=== ALL CREWS ===");
            foreach (var crew in _crews)
            {
                crew.DisplayCrew();
            }
        }
    }

    public interface IFlightManager
    {
        void AddFlight(Flight flight);
        bool RemoveFlight(int flightId);
        List<Flight> GetAllFlights();
        void UpdateFlight(Flight flight);
    }

    public interface ICrewManager
    {
        Crew CreateCrewForFlight(int flightId);
        void AddCrewMember(CrewMember member);
        bool AssignCrewToFlight(int crewId, int flightId);
        List<CrewMember> GetAvailableCrewMembers();
    }

    public class AirlineSystem
    {
        private AuthenticationService _authService;
        private FlightService _flightService;
        private CrewService _crewService;

        public AirlineSystem()
        {
            _authService = new AuthenticationService();
            _flightService = new FlightService();
            _crewService = new CrewService();
        }

        public void Run()
        {
            Console.WriteLine("=== AIRLINE MANAGEMENT SYSTEM ===");

            // Демонстрация функциональности
            DemoAuthentication();
            DemoFlightManagement();
            DemoCrewManagement();
            DemoCompleteWorkflow();
        }

        private void DemoAuthentication()
        {
            Console.WriteLine("\n--- AUTHENTICATION DEMO ---");

            // Вход в систему
            _authService.Login("admin", "admin123");

            // Попытка входа с неверными данными
            _authService.Login("wrong", "password");

            // Регистрация нового пользователя
            var newUser = new User("newuser", "pass123", "new@email.com", UserRole.User);
            _authService.Register(newUser);

            // Выход из системы
            _authService.Logout();
        }

        private void DemoFlightManagement()
        {
            Console.WriteLine("\n--- FLIGHT MANAGEMENT DEMO ---");

            // Вход как администратор
            _authService.Login("admin", "admin123");

            if (_authService.HasPermission(UserRole.Administrator))
            {
                // Создание рейсов
                var flight1 = new Flight("SU100", "Moscow (SVO)", "New York (JFK)",
                    DateTime.Now.AddDays(1), DateTime.Now.AddDays(1).AddHours(10), "Boeing 777");

                var flight2 = new Flight("SU200", "Moscow (SVO)", "London (LHR)",
                    DateTime.Now.AddDays(2), DateTime.Now.AddDays(2).AddHours(4), "Airbus A320");

                _flightService.AddFlight(flight1);
                _flightService.AddFlight(flight2);

                // Просмотр всех рейсов
                _flightService.DisplayAllFlights();

                // Обновление рейса
                flight1.UpdateFlightInfo("Moscow (SVO)", "Paris (CDG)",
                    DateTime.Now.AddDays(1).AddHours(2), DateTime.Now.AddDays(1).AddHours(6));

                _flightService.DisplayAllFlights();
            }

            _authService.Logout();
        }

        private void DemoCrewManagement()
        {
            Console.WriteLine("\n--- CREW MANAGEMENT DEMO ---");

            // Вход как диспетчер
            _authService.Login("dispatcher", "dispatch123");

            if (_authService.HasPermission(UserRole.Dispatcher))
            {
                // Просмотр доступных членов экипажа
                _crewService.DisplayAvailableCrew();

                // Создание бригады для рейса
                var crew = _crewService.CreateCrewForFlight(1);

                // Добавление членов в бригаду
                var pilot = _crewService.GetCrewMember(1); // John Smith - Pilot
                var coPilot = _crewService.GetCrewMember(2); // Maria Garcia - CoPilot
                var attendant = _crewService.GetCrewMember(5); // Anna Kowalski - Flight Attendant

                if (pilot != null) crew.AddMember(pilot);
                if (coPilot != null) crew.AddMember(coPilot);
                if (attendant != null) crew.AddMember(attendant);

                // Проверка состава бригады
                crew.ValidateCrewComposition();
                crew.DisplayCrew();

                _crewService.DisplayAllCrews();
            }

            _authService.Logout();
        }

        private void DemoCompleteWorkflow()
        {
            Console.WriteLine("\n--- COMPLETE WORKFLOW DEMO ---");

            // Администратор создает рейс
            _authService.Login("admin", "admin123");
            var newFlight = new Flight("SU300", "Moscow (SVO)", "Tokyo (NRT)",
                DateTime.Now.AddDays(3), DateTime.Now.AddDays(3).AddHours(9), "Boeing 787");
            _flightService.AddFlight(newFlight);
            _authService.Logout();

            // Диспетчер формирует бригаду
            _authService.Login("dispatcher", "dispatch123");

            var crew = _crewService.CreateCrewForFlight(newFlight.FlightId);

            // Добавляем полный состав
            var members = _crewService.GetAvailableCrewMembers();
            var selectedMembers = members.Take(5).ToList(); // Берем первых 5 доступных членов

            foreach (var member in selectedMembers)
            {
                crew.AddMember(member);
            }

            crew.ValidateCrewComposition();

            // Назначаем бригаду на рейс
            _crewService.AssignCrewToFlight(crew.CrewId, newFlight.FlightId, _flightService);

            // Показываем финальный результат
            Console.WriteLine("\n=== FINAL RESULT ===");
            newFlight.DisplayFlightInfo();
            if (newFlight.AssignedCrew != null)
            {
                newFlight.AssignedCrew.DisplayCrew();
            }

            _authService.Logout();
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            var airlineSystem = new AirlineSystem();
            airlineSystem.Run();

            Console.WriteLine("\nPress any key to exit...");
            Console.ReadKey();
        }
    }
}