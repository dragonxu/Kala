﻿using System;
using System.Collections.Generic;
using Xamarin.Forms;
using Newtonsoft.Json.Linq;
using FFImageLoading.Forms;
using FFImageLoading.Transformations;
using FFImageLoading.Work;
using Plugin.Logger;

namespace Kala
{
    public partial class Widgets : ContentPage
    {
        public static void Avatar(Grid grid, int px, int py, int sx, int sy, JArray data)
        {
            Microsoft.AppCenter.Analytics.Analytics.TrackEvent("Create Avatar Widget");
            CrossLogger.Current.Debug("Avatar", "Creating Avatar Widget");

            try
            {
                List<Models.Sitemap.Widget3> items = data.ToObject<List<Models.Sitemap.Widget3>>();
               
                #region w_grid
                Grid w_grid = new Grid
                {
                    Padding = new Thickness(0, 0, 0, 0),
                    RowSpacing = 0,
                    ColumnSpacing = 0,
                    BackgroundColor = App.Config.CellColor,
                    VerticalOptions = LayoutOptions.FillAndExpand,
                    HorizontalOptions = LayoutOptions.FillAndExpand,
                    RowDefinitions = new RowDefinitionCollection
                    {
                        new RowDefinition { Height = new GridLength(1, GridUnitType.Star) },
                        new RowDefinition { Height = GridLength.Auto },
                    },
                    ColumnDefinitions = new ColumnDefinitionCollection
                    {
                        new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) },
                    },
                };
                grid.Children.Add(w_grid, px, px + sx, py, py + sy);
                #endregion w_grid

                foreach (Models.Sitemap.Widget3 item in items)
                {
                    if (item.Url != null) {
                        var img = new CachedImage()
                        {
                            HorizontalOptions = LayoutOptions.Center,
                            VerticalOptions = LayoutOptions.StartAndExpand,
                            DownsampleToViewSize = false,
                            Aspect = Aspect.AspectFill,
                            RetryCount = 999,
                            RetryDelay = 1000,
                            BitmapOptimizations = true,
                            Transformations = new List<ITransformation>()
                            {
                                new CircleTransformation()
                            },
                            Source = item.Url,
                        };
                        w_grid.Children.Add(img, 0, 0);
                    } else {
                        var state = Helpers.GetTrueState(item);
                        ItemLabel l_mode = new ItemLabel
                        {
                            Text = state.Item1,
                            FontSize = 20,
                            TextColor = App.Config.TextColor,
                            BackgroundColor = Color.Transparent,
                            HorizontalOptions = LayoutOptions.Center,
                            VerticalOptions = LayoutOptions.End,
                            Name = item.Item.Name,
                            Transformed = state.Item2
                        };
                        App.Config.Itemlabels.Add(l_mode);
                        w_grid.Children.Add(l_mode, 0, 1);
                    }
                }
 
                //Button must be last to be added to work
                Button dummyButton = new Button
                {
                    HorizontalOptions = LayoutOptions.FillAndExpand,
                    VerticalOptions = LayoutOptions.FillAndExpand,
                    BackgroundColor = Color.Transparent,
                };
                grid.Children.Add(dummyButton, px, px + sx, py, py + sy);
                dummyButton.Clicked += OnDummyButtonClicked;
            }
            catch (Exception ex)
            {
                CrossLogger.Current.Error("Avatar", "Crashed: " + ex.ToString());
                Error(grid, px, py, sx, sy, ex.ToString());
            }
        }
    }
}
