﻿using System;
using System.Collections.Generic;
using Xamarin.Forms;
using DrawShape;
using Newtonsoft.Json.Linq;
using Plugin.Logger;
using CircularProgressBar.FormsPlugin.Abstractions;
using System.Linq;
using System.Threading.Tasks;

namespace Kala
{
    public partial class Widgets : ContentPage
    {
        public static void Switch(Grid grid, string x1, string y1, string header, JObject data)
        {
            int.TryParse(x1, out int px);
            int.TryParse(y1, out int py);
            
            try
            {
                Models.Sitemap.Widget3 item = data.ToObject<Models.Sitemap.Widget3>();
                Dictionary<string, string> widgetKeyValuePairs = Helpers.SplitCommand(item.Label);
                CrossLogger.Current.Debug("Switch", "Label: " + widgetKeyValuePairs["label"]);

                //Master Grid for Widget
                Grid Widget_Grid = new Grid
                {
                    RowDefinitions = new RowDefinitionCollection {
                        new RowDefinition { Height = new GridLength(1, GridUnitType.Star) }
                    },
                    ColumnDefinitions = new ColumnDefinitionCollection {
                        new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) },
                    },
                    RowSpacing = 0,
                    ColumnSpacing = 0,
                    BackgroundColor = App.Config.CellColor,
                    HorizontalOptions = LayoutOptions.FillAndExpand,
                    VerticalOptions = LayoutOptions.FillAndExpand,
                };
                grid.Children.Add(Widget_Grid, px, py);

                App.TrackItem i = new App.TrackItem
                {
                    Grid = Widget_Grid,
                    Name = item.Item.Name,
                    Header = header,
                    Icon = widgetKeyValuePairs["icon"],
                    State = item.Item.State,
                    Type = Models.Itemtypes.Switch,
                };
                App.Config.Items.Add(i);

                Switch_update(true, i);
            }
            catch (Exception ex)
            {
                CrossLogger.Current.Error("Switch", "Widgets.Switch crashed: " + ex.ToString());
                Error(grid, px, py, 1, 1, ex.ToString());
            }
        }

        public static void Switch_update(bool Create, App.TrackItem item)
        {
            string status = "N/A";

            item.Grid.Children.Clear();
            
            //Header
            item.Grid.Children.Add(new Label
            {
                Text = item.Header,
                FontSize = Device.GetNamedSize(NamedSize.Medium, typeof(Label)),
                TextColor = App.Config.TextColor,
                BackgroundColor = App.Config.CellColor,
                HorizontalTextAlignment = TextAlignment.Center,
                VerticalTextAlignment = TextAlignment.Start
            }, 0, 0);

            if (item.State != null && !item.State.Equals("Uninitialized"))
            {
                try
                {
                    if (int.TryParse(item.State, out int stat))
                    {
                        if (stat > 0)
                        {
                            Switch_On(item);
                            status = "ON";
                        }
                        else
                        {
                            Switch_Off(item);
                            status = "OFF";
                        }
                    }
                    else
                    {
                        if (item.State.ToUpper().Equals("OFF"))
                        {
                            Switch_Off(item);
                            status = "OFF";
                        }
                        else
                        {
                            Switch_On(item);
                            status = "ON";
                        }
                    }
                }
                catch (Exception ex)
                {
                    Error(item.Grid, 0, 0, 1, 1, ex.ToString());
                }
            }

            //Image
            item.Grid.Children.Add(new Image
            {
                Source = item.Icon,
                Aspect = Aspect.AspectFill,
                BackgroundColor = Color.Transparent,
                VerticalOptions = LayoutOptions.Center,
                HorizontalOptions = LayoutOptions.Center
            }, 0, 0);

            //Status
            ItemLabel l_status = new ItemLabel
            {
                Text = status.ToUpper(),
                FontSize = Device.GetNamedSize(NamedSize.Small, typeof(Label)),
                TextColor = App.Config.TextColor,
                BackgroundColor = App.Config.CellColor,
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.End,
                TranslationY = -10,
                Name = item.Name
            };
            item.Grid.Children.Add(l_status, 0, 0);

            //Button must be last to be added to work
            Button switchButton = new Button
            {
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.FillAndExpand,
                BackgroundColor = Color.Transparent,
                StyleId = item.Name //StyleID is not used on buttons
            };
            item.Grid.Children.Add(switchButton, 0, 0);
            switchButton.Clicked += OnSwitchButtonClicked;
            CrossLogger.Current.Debug("Switch", "Button ID: " + switchButton.Id + " created.");
        }

        private static void Switch_On(App.TrackItem item)
        {
            item.Grid.Children.Add(new ShapeView()
            {
                ShapeType = ShapeType.Circle,
                StrokeColor = App.Config.ValueColor,
                Color = App.Config.ValueColor,
                StrokeWidth = 10.0f,
                Scale = 2,
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center
            }, 0, 0);
        }

        private static void Switch_Off(App.TrackItem item)
        {
            int intStrokeThickness = 2;
            switch (Device.RuntimePlatform)
            {
                case Device.Android:
                    intStrokeThickness = 4;
                    break;
            }

            item.Grid.Children.Add(new CircularProgressBarView
            {
                Progress = 100,
                StrokeThickness = intStrokeThickness,
                BackgroundColor = Color.Transparent,
                ProgressBackgroundColor = App.Config.BackGroundColor,
                ProgressColor = App.Config.ValueColor,
                Scale = 0.5f
            }, 0, 0);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Await.Warning", "CS4014:Await.Warning")]
        private static void OnSwitchButtonClicked(object sender, EventArgs e)
        {
            Button button = sender as Button;
            string name = button.StyleId;

            //foreach (App.TrackItem item in App.Config.items)
            foreach (App.TrackItem item in App.Config.Items.Where(n => n.Name == name))
            {
                if (!item.State.ToLower().Equals("uninitialized"))
                {
                    if (int.TryParse(item.State, out int stat))
                    {
                        item.State = (stat > 0) ? "OFF" : "ON";
                    }
                    else
                    {
                        item.State = (item.State.ToUpper().Equals("ON")) ? "OFF" : "ON";
                    }
                }
                else
                {
                    item.State = "ON";
                }

                CrossLogger.Current.Debug("Switch", "Button ID: '" + button.Id.ToString() + "', URL: '" + button.StyleId + "', New State: '" + item.State + "'");
                Switch_update(false, item);
                Task.Run(async () =>
                {
                    await new RestService().SendCommand(name, item.State);
                });              
            }
        }
    }
}
