using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace nAnyWebApp.Services
{
    // Prosty ORM do zarządzania danymi
    public class nJORM<T> : IEnumerable<T> where T : class
    {
        private List<T> _data;
        private string _filePath;


        public nJORM(string filePath)
        {
            _filePath = filePath;
            LoadData();
        }
        // Implementacja IEnumerable<T>
        public IEnumerator<T> GetEnumerator()
        {
            return _data.GetEnumerator();
        }

        // Implementacja IEnumerable (niegeneryczna)
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
        // Ładowanie danych z pliku JSON
        
        private void LoadData()
        {
            try
            {
                if (File.Exists(_filePath))
                {
                    var json = File.ReadAllText(_filePath);
                    _data = JsonSerializer.Deserialize<List<T>>(json) ?? new List<T>();
                }
                else
                {
                    _data = new List<T>();
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine($"{DateTime.Now} error: nJORM->LoadData: {ex.Message}");
            }
        }

        // Zapisywanie danych do pliku JSON
        private void SaveData()
        {
            try
            {
                var json = JsonSerializer.Serialize(_data);
                File.WriteAllText(_filePath, json);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{DateTime.Now} error: nJORM->SaveData: {ex.Message}");
                var json = JsonSerializer.Serialize(_data);
            }
        }

        // Dodawanie nowego rekordu
        public void Add(T item)
        {
            _data.Add(item);
            SaveData();
        }

        // Pobieranie wszystkich rekordów
        public IEnumerable<T> GetAll()
        {
            return _data;
        }

        // Pobieranie rekordu po ID (zakładając, że model ma właściwość Id)
        public T GetById(int id)
        {
            return _data.FirstOrDefault(item => (int)item.GetType().GetProperty("Id").GetValue(item) == id);
        }

        // Aktualizacja rekordu
        public void Update(T item)
        {
            var existingItem = GetById((int)item.GetType().GetProperty("Id").GetValue(item));
            if (existingItem != null)
            {
                _data.Remove(existingItem);
                _data.Add(item);
                SaveData();
            }
        }

        // Usuwanie rekordu
        public void Delete(int id)
        {
            var itemToRemove = GetById(id);
            if (itemToRemove != null)
            {
                _data.Remove(itemToRemove);
                SaveData();
            }
        }
    }

    // Przykład użycia
    class Program
    {
        static void Main(string[] args)
        {
            var orm = new nJORM<Person>("data.json");

            // Dodawanie nowych osób
            orm.Add(new Person { Id = 1, Name = "John Doe", Age = 30 });
            orm.Add(new Person { Id = 2, Name = "Jane Doe", Age = 25 });

            // Pobieranie wszystkich osób
            var allPeople = orm.GetAll();
            foreach (var person in allPeople)
            {
                Console.WriteLine($"ID: {person.Id}, Name: {person.Name}, Age: {person.Age}");
            }

            // Pobieranie osoby po ID
            var personById = orm.GetById(1);
            if (personById != null)
            {
                Console.WriteLine($"Found person: {personById.Name}");
            }

            // Aktualizacja osoby
            var personToUpdate = orm.GetById(2);
            if (personToUpdate != null)
            {
                personToUpdate.Name = "Jane Smith";
                orm.Update(personToUpdate);
                orm.GetAll();
            }

            // Usuwanie osoby
            orm.Delete(1);
        }
    }

    // Klasa reprezentująca model danych
    public class Person
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Age { get; set; }
    }
}