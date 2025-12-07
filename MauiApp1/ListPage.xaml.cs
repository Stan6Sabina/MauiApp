using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MauiApp1.Models;

namespace MauiApp1;

public partial class ListPage : ContentPage
{
    public ListPage()
    {
        InitializeComponent();
    }
    async void OnSaveButtonClicked(object sender, EventArgs e)
    {
        var slist = (ShopList)BindingContext;
        slist.Date = DateTime.UtcNow;
        await App.Database.SaveShopListAsync(slist);
        await Navigation.PopAsync();
    }
    async void OnDeleteButtonClicked(object sender, EventArgs e)
    {
        var slist = (ShopList)BindingContext;
        await App.Database.DeleteShopListAsync(slist);
        await Navigation.PopAsync();
    }
    
    async void OnChooseButtonClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new ProductPage((ShopList)
            this.BindingContext)
        {
            BindingContext = new Product()
        });

    }
    
    async void OnDeleteItemClicked(object sender, EventArgs e)
    {
        var selected = listView.SelectedItem as Product;
        if (selected == null)
        {
            await DisplayAlert("Delete Item", "Please select an item to delete.", "OK");
            return;
        }

        var shopList = (ShopList)BindingContext;

        var confirm = await DisplayAlert("Delete Item",
            $"Remove '{selected.Description}' from this list?",
            "Delete", "Cancel");
        if (!confirm)
            return;

        await App.Database.DeleteListProductAsync(shopList.ID, selected.ID);

        // Refresh items and clear selection
        listView.ItemsSource = await App.Database.GetListProductsAsync(shopList.ID);
        listView.SelectedItem = null;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        var shopl = (ShopList)BindingContext;

        listView.ItemsSource = await App.Database.GetListProductsAsync(shopl.ID);
    }
}