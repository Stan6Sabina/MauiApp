using SQLite;
using SQLiteNetExtensions.Attributes;
namespace MauiApp1.Models;

public class Product
{
    [PrimaryKey, AutoIncrement]
    public int ID { get; set; }
    public string Description { get; set; }
    [OneToMany]
    public List<ListProduct> ListProducts { get; set; } 
}