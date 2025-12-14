using SQLite;
using System.Collections.Generic;
using System.Threading.Tasks;
using MauiApp1.Models;
namespace MauiApp1.Data;

public class ShoppingListDatabase
{
    readonly SQLiteAsyncConnection _database;
    public ShoppingListDatabase(string dbPath)
    {
        _database = new SQLiteAsyncConnection(dbPath);
        _database.CreateTableAsync<ShopList>().Wait();
        _database.CreateTableAsync<Product>().Wait();
        _database.CreateTableAsync<ListProduct>().Wait();
        _database.CreateTableAsync<Shop>().Wait();
    }
    public Task<List<Shop>> GetShopsAsync()
    {
        return _database.Table<Shop>().ToListAsync();
    }
    public Task<int> SaveShopAsync(Shop shop)
    {
        if (shop.ID != 0)
        {
            return _database.UpdateAsync(shop);
        }
        else
        {
            return _database.InsertAsync(shop);
        }
    }
    public async Task<int> DeleteShopAsync(Shop shop)
    {
        // Delete dependent records: ListProduct entries for the shop's lists, then the lists, then the shop
        // 1) Delete ListProduct rows that belong to any ShopList of this Shop
        await _database.ExecuteAsync(
            "DELETE FROM ListProduct WHERE ShopListID IN (SELECT ID FROM ShopList WHERE ShopID = ?)",
            shop.ID);

        // 2) Delete ShopList rows for this Shop
        await _database.ExecuteAsync("DELETE FROM ShopList WHERE ShopID = ?", shop.ID);

        // 3) Finally, delete the Shop row
        return await _database.DeleteAsync(shop);
    }
    public Task<int> SaveProductAsync(Product product)
    {
        if (product.ID != 0)
        {
            return _database.UpdateAsync(product);
        }
        else
        {
            return _database.InsertAsync(product);
        }
    }
    public Task<int> DeleteProductAsync(Product product)
    {
        return _database.DeleteAsync(product);
    }
    public Task<List<Product>> GetProductsAsync()
    {
        return _database.Table<Product>().ToListAsync();
    }



    public Task<List<ShopList>> GetShopListsAsync()
    {
        return _database.Table<ShopList>().ToListAsync();
    }
    public Task<ShopList> GetShopListAsync(int id)
    {
        return _database.Table<ShopList>()
            .Where(i => i.ID == id)
            .FirstOrDefaultAsync();
    }
    public Task<int> SaveShopListAsync(ShopList slist)
    {
        if (slist.ID != 0)
        {
            return _database.UpdateAsync(slist);
        }
        else
        {
            return _database.InsertAsync(slist);
        }
    }
    public Task<int> DeleteShopListAsync(ShopList slist)
    {
        return _database.DeleteAsync(slist);
    }
    
    public Task<int> SaveListProductAsync(ListProduct listp)
    {
        if (listp.ID != 0)
        {
            return _database.UpdateAsync(listp);
        }
        else
        {
            return _database.InsertAsync(listp);
        }
    }

    // Deletes only the association between a ShopList and a Product
    public Task<int> DeleteListProductAsync(int shopListId, int productId)
    {
        return _database.ExecuteAsync(
            "DELETE FROM ListProduct WHERE ShopListID = ? AND ProductID = ?",
            shopListId, productId);
    }

    public Task<List<Product>> GetListProductsAsync(int shoplistid)
    {
        return _database.QueryAsync<Product>(
            "select P.ID, P.Description from Product P"
            + " inner join ListProduct LP"
            + " on P.ID = LP.ProductID where LP.ShopListID = ?",
            shoplistid);
    }

}
 
