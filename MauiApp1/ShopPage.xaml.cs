using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MauiApp1.Models;
using Plugin.LocalNotification;

namespace MauiApp1;

public partial class ShopPage : ContentPage
{
    public ShopPage()
    {
        InitializeComponent();
    }
    
    async void OnSaveButtonClicked(object sender, EventArgs e)
    {
        var shop = (Shop)BindingContext;
        await App.Database.SaveShopAsync(shop);
        await Navigation.PopAsync();
    }
    
    async void OnShowMapButtonClicked(object sender, EventArgs e)
    {
        var shop = (Shop)BindingContext;
        var address = shop.Adress;
        var locations = await Geocoding.GetLocationsAsync(address);

        var options = new MapLaunchOptions { Name = "Magazinul meu preferat" };
            var shoplocation = locations?.FirstOrDefault();
            var myLocation = await Geolocation.GetLocationAsync();
            /* var myLocation = new Location(46.7731796289, 23.6213886738);
           //pentru Windows Machine */
            var distance = myLocation.CalculateDistance(locations, DistanceUnits.Kilometers);
            if (distance < 5)
            {
                var request = new NotificationRequest
                {
                    Title = "Ai de facut cumparaturi in apropiere!",
                    Description = address,
                    Schedule = new NotificationRequestSchedule
                    {
                        NotifyTime = DateTime.Now.AddSeconds(1)
                    }
                };
                LocalNotificationCenter.Current.Show(request);
            }
            
            await Map.OpenAsync(shoplocation, options);
        }
    
    async void OnDeleteButtonClicked(object sender, EventArgs e)
    {
        var shop = (Shop)BindingContext;
        if (shop == null || shop.ID == 0)
        {
            await DisplayAlert("Delete Shop", "No shop to delete.", "OK");
            return;
        }

        var confirm = await DisplayAlert("Delete Shop",
            $"Are you sure you want to delete '{shop.ShopName}'? This will also remove its shopping lists and their items.",
            "Delete", "Cancel");
        if (!confirm)
            return;

        try
        {
            await App.Database.DeleteShopAsync(shop);
            await Navigation.PopAsync();
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Failed to delete the shop: {ex.Message}", "OK");
        }
    }
    
}