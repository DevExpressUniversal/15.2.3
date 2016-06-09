#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{                                                                   }
{                                                                   }
{       Copyright (c) 2000-2015 Developer Express Inc.              }
{       ALL RIGHTS RESERVED                                         }
{                                                                   }
{   The entire contents of this file is protected by U.S. and       }
{   International Copyright Laws. Unauthorized reproduction,        }
{   reverse-engineering, and distribution of all or any portion of  }
{   the code contained in this file is strictly prohibited and may  }
{   result in severe civil and criminal penalties and will be       }
{   prosecuted to the maximum extent possible under the law.        }
{                                                                   }
{   RESTRICTIONS                                                    }
{                                                                   }
{   THIS SOURCE CODE AND ALL RESULTING INTERMEDIATE FILES           }
{   ARE CONFIDENTIAL AND PROPRIETARY TRADE                          }
{   SECRETS OF DEVELOPER EXPRESS INC. THE REGISTERED DEVELOPER IS   }
{   LICENSED TO DISTRIBUTE THE PRODUCT AND ALL ACCOMPANYING .NET    }
{   CONTROLS AS PART OF AN EXECUTABLE PROGRAM ONLY.                 }
{                                                                   }
{   THE SOURCE CODE CONTAINED WITHIN THIS FILE AND ALL RELATED      }
{   FILES OR ANY PORTION OF ITS CONTENTS SHALL AT NO TIME BE        }
{   COPIED, TRANSFERRED, SOLD, DISTRIBUTED, OR OTHERWISE MADE       }
{   AVAILABLE TO OTHER INDIVIDUALS WITHOUT EXPRESS WRITTEN CONSENT  }
{   AND PERMISSION FROM DEVELOPER EXPRESS INC.                      }
{                                                                   }
{   CONSULT THE END USER LICENSE AGREEMENT FOR INFORMATION ON       }
{   ADDITIONAL RESTRICTIONS.                                        }
{                                                                   }
{*******************************************************************}
*/
#endregion Copyright (c) 2000-2015 Developer Express Inc.

using DevExpress.Xpf.Core.Native;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
namespace DevExpress.Xpf.DemoBase {
	public enum DemoBaseTileSize {
		Unit, Double, Quad
	}
	public partial class DemoBaseTileControl : ItemsControl {
		DemoBaseScrollViewer scrollViewer;
		public Visibility ComputedLeftShadowVisibility {
			get { return (Visibility)GetValue(ComputedLeftShadowVisibilityProperty); }
			set { SetValue(ComputedLeftShadowVisibilityProperty, value); }
		}
		public static readonly DependencyProperty ComputedLeftShadowVisibilityProperty =
			DependencyProperty.Register("ComputedLeftShadowVisibility", typeof(Visibility), typeof(DemoBaseTileControl), new PropertyMetadata(Visibility.Collapsed));
		public Visibility ComputedRightShadowVisibility {
			get { return (Visibility)GetValue(ComputedRightShadowVisibilityProperty); }
			set { SetValue(ComputedRightShadowVisibilityProperty, value); }
		}
		public static readonly DependencyProperty ComputedRightShadowVisibilityProperty =
			DependencyProperty.Register("ComputedRightShadowVisibility", typeof(Visibility), typeof(DemoBaseTileControl), new PropertyMetadata(Visibility.Collapsed));
		public bool IsSimpleGroupsLayout {
			get { return (bool)GetValue(IsSimpleGroupsLayoutProperty); }
			set { SetValue(IsSimpleGroupsLayoutProperty, value); }
		}
		public static readonly DependencyProperty IsSimpleGroupsLayoutProperty =
			DependencyProperty.Register("IsSimpleGroupsLayout", typeof(bool), typeof(DemoBaseTileControl), new PropertyMetadata(true));
		public bool IsSimpleTilesLayout {
			get { return (bool)GetValue(IsSimpleTilesLayoutProperty); }
			set { SetValue(IsSimpleTilesLayoutProperty, value); }
		}
		public static readonly DependencyProperty IsSimpleTilesLayoutProperty =
			DependencyProperty.Register("IsSimpleTilesLayout", typeof(bool), typeof(DemoBaseTileControl), new PropertyMetadata(true));
		public DataTemplate TileTemplate {
			get { return (DataTemplate)GetValue(TileTemplateProperty); }
			set { SetValue(TileTemplateProperty, value); }
		}
		public static readonly DependencyProperty TileTemplateProperty =
			DependencyProperty.Register("TileTemplate", typeof(DataTemplate), typeof(DemoBaseTileControl), new PropertyMetadata(null, (d, e) => {
				((DemoBaseTileControl)d).OnTileTemplateChanged((DataTemplate)e.NewValue);
			}));
		void OnTileTemplateChanged(DataTemplate dataTemplate) {
			TileTemplateSelector = new TrivialTemplateSelector { Template = dataTemplate };
		}
		public IEnumerable<object> UngroupedItems {
			get { return (IEnumerable<object>)GetValue(UngroupedItemsProperty); }
			set { SetValue(UngroupedItemsProperty, value); }
		}
		public static readonly DependencyProperty UngroupedItemsProperty =
			DependencyProperty.Register("UngroupedItems", typeof(IEnumerable<object>), typeof(DemoBaseTileControl), new PropertyMetadata(null, (d, e) => {
				((DemoBaseTileControl)d).OnUngroupedItemsChanged((IEnumerable<object>)e.NewValue);
			}));
		public DataTemplateSelector TileTemplateSelector {
			get { return (DataTemplateSelector)GetValue(TileTemplateSelectorProperty); }
			set { SetValue(TileTemplateSelectorProperty, value); }
		}
		public static readonly DependencyProperty TileTemplateSelectorProperty =
			DependencyProperty.Register("TileTemplateSelector", typeof(DataTemplateSelector), typeof(DemoBaseTileControl), new PropertyMetadata(null));
		public double ScrollViewerHorizontalOffset {
			get { return (double)GetValue(ScrollViewerHorizontalOffsetProperty); }
			set { SetValue(ScrollViewerHorizontalOffsetProperty, value); }
		}
		public static readonly DependencyProperty ScrollViewerHorizontalOffsetProperty =
			DependencyProperty.Register("ScrollViewerHorizontalOffset", typeof(double), typeof(DemoBaseTileControl), new PropertyMetadata(0d, (d, e) => {
				((DemoBaseTileControl)d).OnSCrollViewerHorizontalOffsetChanged((double)e.NewValue);
			}));
		void OnSCrollViewerHorizontalOffsetChanged(double newValue) {
			UpdateShadowsVisibility(newValue);
		}
		private void OnUngroupedItemsChanged(IEnumerable<object> enumerable) {
			if(enumerable is INotifyCollectionChanged) {
				((INotifyCollectionChanged)enumerable).CollectionChanged += (s, e) => {
					UpdateItemsSource();
				};
			}
			UpdateItemsSource();
		}
		public static DemoBaseTileSize GetTileSize(DependencyObject obj) {
			return (DemoBaseTileSize)obj.GetValue(TileSizeProperty);
		}
		public static void SetTileSize(DependencyObject obj, DemoBaseTileSize value) {
			obj.SetValue(TileSizeProperty, value);
		}
		public static readonly DependencyProperty TileSizeProperty =
			DependencyProperty.RegisterAttached("TileSize", typeof(DemoBaseTileSize), typeof(DemoBaseTileControl), new PropertyMetadata(DemoBaseTileSize.Unit));
		public double ScrollViewerScrollableWidth {
			get { return (double)GetValue(ScrollViewerScrollableWidthProperty); }
			set { SetValue(ScrollViewerScrollableWidthProperty, value); }
		}
		public static readonly DependencyProperty ScrollViewerScrollableWidthProperty =
			DependencyProperty.Register("ScrollViewerScrollableWidth", typeof(double), typeof(DemoBaseTileControl), new PropertyMetadata(0d, (d, e) => {
				((DemoBaseTileControl)d).OnScrollViewerScrollableWidthChanged();
			}));
		public string GroupPropertyName {
			get { return (string)GetValue(GroupPropertyNameProperty); }
			set { SetValue(GroupPropertyNameProperty, value); }
		}
		public static readonly DependencyProperty GroupPropertyNameProperty =
			DependencyProperty.Register("GroupPropertyName", typeof(string), typeof(DemoBaseTileControl), new PropertyMetadata(null, (d, e) => {
				((DemoBaseTileControl)d).OnPropertyNamePropertyChanged();
			}));
		void OnPropertyNamePropertyChanged() {
			UpdateItemsSource();
		}
		void OnScrollViewerScrollableWidthChanged() {
			UpdateShadowsVisibility(scrollViewer.HorizontalOffset);
		}
		void UpdateItemsSource() {
			if(UngroupedItems == null || GroupPropertyName == null)
				return;
			var viewSource = new CollectionViewSource();
			viewSource.Source = UngroupedItems;
			viewSource.GroupDescriptions.Add(new PropertyGroupDescription(GroupPropertyName));
			ItemsSource = viewSource.View.Groups;
		}
		protected override DependencyObject GetContainerForItemOverride() {
			DataTemplate groupTemplate = (DataTemplate)Resources.Cast<DictionaryEntry>().First(r => (string)r.Key == "groupTemplate").Value;
			FrameworkElement groupContainer = (FrameworkElement)groupTemplate.LoadContent();
			return groupContainer;
		}
		protected override void PrepareContainerForItemOverride(DependencyObject element, object item) {
			DemoBaseTileGroupControl groupControl = (DemoBaseTileGroupControl)((FrameworkElement)element).FindName("groupControl");
			groupControl.IsSimpleTilesLayout = IsSimpleTilesLayout;
			groupControl.TileTemplateSelector = TileTemplateSelector;
		}
		public DemoBaseTileControl() {
			InitializeComponent();
		}
		public override void OnApplyTemplate() {
			base.OnApplyTemplate();
			scrollViewer = (DemoBaseScrollViewer)GetTemplateChild("scrollViewer");
			SetBinding(DemoBaseTileControl.ScrollViewerScrollableWidthProperty, new Binding("ScrollableWidth") { Source = scrollViewer });
			SetBinding(DemoBaseTileControl.ScrollViewerHorizontalOffsetProperty, new Binding("HorizontalOffset") { Source = scrollViewer });
		}
		void UpdateShadowsVisibility(double horizontalOffset) {
			if(scrollViewer.ScrollableWidth == 0) {
				ComputedLeftShadowVisibility = Visibility.Collapsed;
				ComputedRightShadowVisibility = Visibility.Collapsed;
				return;
			}
			ComputedLeftShadowVisibility = horizontalOffset <= 0 ? Visibility.Collapsed : Visibility.Visible;
			ComputedRightShadowVisibility = horizontalOffset >= scrollViewer.ScrollableWidth ? Visibility.Collapsed : Visibility.Visible;
		}
	}
	public class DemoBaseTileControlItem {
		public string Title { get; set; }
		public string Group { get; set; }
	}
	class TrivialTemplateSelector : DataTemplateSelector {
		public DataTemplate Template { get; set; }
		public override DataTemplate SelectTemplate(object item, DependencyObject container) {
			return Template;
		}
	}
}
