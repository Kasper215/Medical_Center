using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using MedCenterApp.Models;
using MedCenterApp.Services;

namespace MedCenterApp.Data;

public class JsonRepository<T> : IRepository<T> where T : BaseEntity
{
    private readonly string _filePath;

    public JsonRepository(string fileName)
    {
        _filePath = $"Data/{fileName}.json";
        if (!Directory.Exists("Data"))
        {
            Directory.CreateDirectory("Data");
        }
        
        if (!File.Exists(_filePath))
        {
            File.WriteAllText(_filePath, "[]");
        }
    }

    public List<T> GetAll()
    {
        try
        {
            var json = File.ReadAllText(_filePath);
            return JsonSerializer.Deserialize<List<T>>(json) ?? new List<T>();
        }
        catch
        {
            return new List<T>();
        }
    }

    public T? GetById(int id)
    {
        return GetAll().FirstOrDefault(x => x.Id == id);
    }

    public void Add(T item)
    {
        var items = GetAll();
        item.Id = items.Any() ? items.Max(x => x.Id) + 1 : 1;
        items.Add(item);
        Save(items);
    }

    public void Update(T item)
    {
        var items = GetAll();
        var index = items.FindIndex(x => x.Id == item.Id);
        if (index != -1)
        {
            items[index] = item;
            Save(items);
        }
    }

    public void Delete(int id)
    {
        var items = GetAll();
        var item = items.FirstOrDefault(x => x.Id == id);
        if (item != null)
        {
            items.Remove(item);
            Save(items);
        }
    }

    private void Save(List<T> items)
    {
        var json = JsonSerializer.Serialize(items, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(_filePath, json);
    }
}
