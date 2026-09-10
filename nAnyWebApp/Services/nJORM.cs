using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace nAnyWebApp.Services;

/// <summary>
/// Prosty, bezpieczny menedżer przechowywania danych w pliku JSON
/// </summary>
public class nJORM<T> : IEnumerable<T> where T : class
{
    private List<T> _data = new();
    private readonly string _filePath;
    private readonly object _lock = new();

    public nJORM(string filePath)
    {
        _filePath = filePath;
        LoadData();
    }

    public IEnumerator<T> GetEnumerator()
    {
        lock (_lock)
        {
            return new List<T>(_data).GetEnumerator();
        }
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public void LoadData()
    {
        lock (_lock)
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
            catch (Exception ex)
            {
                Debug.WriteLine($"nJORM->LoadData error: {ex.Message}");
                _data = new List<T>();
            }
        }
    }

    public void SaveData()
    {
        lock (_lock)
        {
            try
            {
                var dir = Path.GetDirectoryName(_filePath);
                if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
                {
                    Directory.CreateDirectory(dir);
                }

                var options = new JsonSerializerOptions { WriteIndented = true };
                var json = JsonSerializer.Serialize(_data, options);
                File.WriteAllText(_filePath, json);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"nJORM->SaveData error: {ex.Message}");
            }
        }
    }

    public void Add(T item)
    {
        lock (_lock)
        {
            _data.Add(item);
            SaveData();
        }
    }

    public IEnumerable<T> GetAll()
    {
        lock (_lock)
        {
            return new List<T>(_data);
        }
    }

    public T? GetById(object id)
    {
        lock (_lock)
        {
            return _data.FirstOrDefault(item =>
            {
                var prop = item.GetType().GetProperty("Id");
                return prop != null && Equals(prop.GetValue(item)?.ToString(), id.ToString());
            });
        }
    }

    public void Update(T item)
    {
        lock (_lock)
        {
            var prop = item.GetType().GetProperty("Id");
            if (prop == null) return;

            var id = prop.GetValue(item);
            var existingItem = _data.FirstOrDefault(i => Equals(prop.GetValue(i)?.ToString(), id?.ToString()));
            if (existingItem != null)
            {
                var index = _data.IndexOf(existingItem);
                _data[index] = item;
                SaveData();
            }
        }
    }

    public void Delete(object id)
    {
        lock (_lock)
        {
            var itemToRemove = GetById(id);
            if (itemToRemove != null)
            {
                _data.Remove(itemToRemove);
                SaveData();
            }
        }
    }
}