using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Text.Encodings.Web;
using System.Text.Unicode;
using System.Linq;

namespace DataQualityChecker
{
    public class Anime
    {
        public string Title { get; set; }
        public int Episodes { get; set; }
        public int Rating { get; set; }
    }

    class Program
    {
        static List<Anime> database = new List<Anime>();
        static string fileName = "anime_data.json";

        static void Main(string[] args)
        {
            LoadFromFile();
            bool programRunning = true;

            while (programRunning)
            {
                Console.Clear();
                Console.WriteLine("=== SYSTEM ZARZĄDZANIA ANIME 2.0 ===");
                Console.WriteLine($"W bazie: {database.Count} tytułów");
                Console.WriteLine("1. Dodaj nowe anime");
                Console.WriteLine("2. Pokaż listę / Ranking");
                Console.WriteLine("3. Edytuj wpis");
                Console.WriteLine("4. Usuń wpis");
                Console.WriteLine("5. Wyszukaj wpis");
                Console.WriteLine("6. Wyjdź");
                Console.Write("\nWybierz opcję: ");

                string choice = Console.ReadLine();
                switch (choice)
                {
                    case "1": AddNewAnime(); break;
                    case "2": ShowDatabase(); break;
                    case "3": EditAnime(); break;
                    case "4": DeleteAnime(); break;
                    case "5": SearchAnime(); break;
                    case "6": programRunning = false; break;
                }
            }
        }

        // Business Logic Methods

        static void ShowDatabase()
        {
            Console.WriteLine("\n--- TWOJA LISTA ---");
            if (database.Count == 0) Console.WriteLine("Baza jest pusta.");
            else
            {
                for (int i = 0; i < database.Count; i++)
                {
                    Console.WriteLine($"{i + 1}. [{database[i].Rating}/10] {database[i].Title} ({database[i].Episodes} odc.)");
                }
            }
            Console.WriteLine("\nNaciśnij klawisz...");
            Console.ReadKey();
        }

        static void EditAnime()
        {
            if (database.Count == 0) return;

            ShowDatabase(); 
            Console.Write("\nPodaj numer do edycji: ");

            if (int.TryParse(Console.ReadLine(), out int index) && index > 0 && index <= database.Count)
            {
                var anime = database[index - 1];
                Console.WriteLine($"--- Edytujesz: {anime.Title} ---");

             
                Console.Write("Nowy tytuł (enter = bez zmian): ");
                string newTitle = Console.ReadLine();
                if (!string.IsNullOrWhiteSpace(newTitle)) anime.Title = newTitle;

                
                while (true)
                {
                    Console.Write($"Nowe odcinki (obecnie: {anime.Episodes}, 0 = bez zmian): ");
                    string input = Console.ReadLine();
                    if (input == "0" || string.IsNullOrEmpty(input)) break;
                    if (int.TryParse(input, out int newEps) && newEps > 0) { anime.Episodes = newEps; break; }
                    Console.WriteLine("BŁĄD: Podaj poprawną liczbę!");
                }

              
                while (true)
                {
                    Console.Write($"Nowa ocena (obecnie: {anime.Rating}/10, 0 = bez zmian): ");
                    string input = Console.ReadLine();
                    if (input == "0" || string.IsNullOrEmpty(input)) break;
                    if (int.TryParse(input, out int newRate) && newRate >= 1 && newRate <= 10) { anime.Rating = newRate; break; }
                    Console.WriteLine("BŁĄD: Ocena musi być od 1 do 10!");
                }

                SaveToFile();
                Console.WriteLine("Zaktualizowano pomyślnie!");
            }
            else Console.WriteLine("Nieprawidłowy numer!");
            Console.ReadKey();
        }

        static void DeleteAnime()
        {
            if (database.Count == 0) return;

            Console.WriteLine("\nPodaj numer anime do USUNIĘCIA:");
            if (int.TryParse(Console.ReadLine(), out int index) && index > 0 && index <= database.Count)
            {
                Console.WriteLine($"Czy na pewno chcesz usunąć {database[index - 1].Title}? (t/n)");
                if (Console.ReadLine().ToLower() == "t")
                {
                    database.RemoveAt(index - 1);
                    SaveToFile();
                    Console.WriteLine("Usunięto pomyślnie.");
                }
            }
            Console.ReadKey();
        }

        static void SearchAnime()
        {
            Console.Write("\nPodaj fragment nazwy do wyszukania: ");
            string query = Console.ReadLine().ToLower();

            Console.WriteLine($"\n--- WYNIKI WYSZUKIWANIA ---");
            bool found = false;

            for (int i = 0; i < database.Count; i++)
            {
               
                if (database[i].Title.ToLower().Contains(query))
                {
                  
                    Console.WriteLine($"{i + 1}. [{database[i].Rating}/10] {database[i].Title}");
                    found = true;
                }
            }

            if (!found)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("Nic nie znaleziono o takiej nazwie.");
                Console.ResetColor();
            }

            Console.WriteLine("\nNaciśnij dowolny klawisz...");
            Console.ReadKey();
        }

        // Data Persistence

        static void SaveToFile()
        {
            try
            {
                var options = new JsonSerializerOptions
                {
                    Encoder = JavaScriptEncoder.Create(UnicodeRanges.All),
                    WriteIndented = true
                };
                string jsonString = JsonSerializer.Serialize(database, options);
                File.WriteAllText(fileName, jsonString);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"BŁĄD ZAPISU: Nie udało się zapisać danych na dysku! {ex.Message}");
                Console.ReadKey();
            }
        }

        static void LoadFromFile()
        {
            try
            {
                if (File.Exists(fileName))
                {
                    string jsonString = File.ReadAllText(fileName);
                    database = JsonSerializer.Deserialize<List<Anime>>(jsonString) ?? new List<Anime>();
                }
            }
            catch (JsonException ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"BŁĄD KRYTYCZNY: Plik danych jest uszkodzony! ({ex.Message})");
                Console.ResetColor();
                database = new List<Anime>();
                Console.ReadKey();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Nieoczekiwany błąd podczas wczytywania: {ex.Message}");
                Console.ReadKey();
            }
        }

        static void AddNewAnime()
        {
            Console.WriteLine("\n--- DODAWANIE NOWEGO TYTUŁU ---");

         
            string title = "";
            while (string.IsNullOrWhiteSpace(title))
            {
                Console.Write("Podaj tytuł: ");
                title = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(title)) Console.WriteLine("BŁĄD: Tytuł nie może być pusty!");
            }

          
            int episodes;
            while (true)
            {
                Console.Write("Liczba odcinków: ");
                if (int.TryParse(Console.ReadLine(), out episodes) && episodes > 0) break;
                Console.WriteLine("BŁĄD: Podaj liczbę większą od 0!");
            }

           
            int rating;
            while (true)
            {
                Console.Write("Ocena (1-10): ");
                if (int.TryParse(Console.ReadLine(), out rating) && rating >= 1 && rating <= 10) break;

                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("BŁĄD: Ocena musi być liczbą od 1 do 10!");
                Console.ResetColor();
            }

            database.Add(new Anime { Title = title, Episodes = episodes, Rating = rating });
            SaveToFile();
            Console.WriteLine("\nDodano pomyślnie! Naciśnij klawisz...");
            Console.ReadKey();
        }
    }
}