# Anime Manager

Aplikacja konsolowa w C# do zarządzania listą anime z zapisem do pliku JSON.

## Funkcjonalności
- Operacje CRUD (Create, Read, Update, Delete) na liście obiektów.
- Trwałość danych: zapis i odczyt bazy w formacie JSON.
- Walidacja danych wejściowych: sprawdzanie zakresu ocen (1-10) i poprawności typów liczbowych.
- Filtrowanie: wyszukiwanie tytułów przy użyciu LINQ.

## Specyfikacja techniczna
- **Język:** C# / .NET
- **Format danych:** JSON (System.Text.Json)
- **Kodowanie:** UTF-8 z obsługą Unicode (polskie znaki)
- **Obsługa błędów:** Bloki try-catch dla operacji I/O na plikach.