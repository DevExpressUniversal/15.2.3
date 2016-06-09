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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Collections.ObjectModel;
using DevExpress.Xpf.DemoBase.Helpers;
using DevExpress.DemoData.Model;
using DevExpress.DemoData;
using DevExpress.Mvvm;
namespace DevExpress.Xpf.DemoBase.Internal {
	public partial class DemoBaseProductTilesControl : Control {
		public IEnumerable<object> Products {
			get { return (IEnumerable<object>)GetValue(ProductsProperty); }
			set { SetValue(ProductsProperty, value); }
		}
		public static readonly DependencyProperty ProductsProperty =
			DependencyProperty.Register("Products", typeof(IEnumerable<object>), typeof(DemoBaseProductTilesControl), new PropertyMetadata(null));
		public string Title {
			get { return (string)GetValue(TitleProperty); }
			set { SetValue(TitleProperty, value); }
		}
		public static readonly DependencyProperty TitleProperty =
			DependencyProperty.Register("Title", typeof(string), typeof(DemoBaseProductTilesControl), new PropertyMetadata(null));
		public string Subtitle {
			get { return (string)GetValue(SubtitleProperty); }
			set { SetValue(SubtitleProperty, value); }
		}
		public static readonly DependencyProperty SubtitleProperty =
			DependencyProperty.Register("Subtitle", typeof(string), typeof(DemoBaseProductTilesControl), new PropertyMetadata(null));
		public TileType TileType {
			get { return (TileType)GetValue(TileTypeProperty); }
			set { SetValue(TileTypeProperty, value); }
		}
		public static readonly DependencyProperty TileTypeProperty =
			DependencyProperty.Register("TileType", typeof(TileType), typeof(DemoBaseProductTilesControl), new PropertyMetadata(TileType.Small, new PropertyChangedCallback(OnTileTypeChanged)));
		static void OnTileTypeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			TileTemplateSelector.ProductTileType = (TileType)e.NewValue;
		}
		public bool SimpleGroups {
			get { return (bool)GetValue(SimpleGroupsProperty); }
			set { SetValue(SimpleGroupsProperty, value); }
		}
		public static readonly DependencyProperty SimpleGroupsProperty =
			DependencyProperty.Register("SimpleGroups", typeof(bool), typeof(DemoBaseProductTilesControl), new PropertyMetadata(false));
		public DemoBaseProductTilesControl() {
			DefaultStyleKey = typeof(DemoBaseProductTilesControl);
		}
	}
	public enum TileType {
		Small, Large
	}
	public class ProductTilesControlItem {
		Action onSelected;
		public ProductTilesControlItem(Product product, ReallifeDemo demo, Action onSelected) {
			Product = product;
			Demo = demo;
			this.onSelected = onSelected;
			if (product != null) {
				Group = product.Group;
			} else {
				Group = Demo.Group ?? DemoDataSettings.ShowCasesTitle;
			}
		}
		public Color TileBackground { get; set; }
		public Product Product { get; private set; }
		public ReallifeDemo Demo { get; private set; }
		public string Group { get; private set; }
		public bool IsReallifeDemo { get { return Demo != null; } }
		public ICommand SelectCommand { get { return new DelegateCommand(onSelected); } }
		public List<OpenSolutionMenuItem> OpenSolutionMenu {
			get { return CreateSolutionMenu().ToList(); }
		}
		IEnumerable<OpenSolutionMenuItem> CreateSolutionMenu() {
			if(IsReallifeDemo) {
				return Demo.CreateOpenSolutionMenu(CallingSite.WinDemoChooser);
			}
			return Product.Demos.First().CreateOpenSolutionMenu(CallingSite.WinDemoChooser);
		}
	}
	class TileTemplateSelector : DataTemplateSelector {
		public static TileType ProductTileType { get; set; }
		public DataTemplate FeatureDemoTemplate { get; set; }
		public DataTemplate SmallProductTemplate { get; set; }
		public DataTemplate LargeProductTemplate { get; set; }
		public DataTemplate QuadSizedDemoTemplate { get; set; }
		public override DataTemplate SelectTemplate(object item, DependencyObject container) {
			var typed = (ProductTilesControlItem)item;
			if(typed.IsReallifeDemo) {
				if(typed.Demo.Name == "ProductsDemo")
					return QuadSizedDemoTemplate;
				return FeatureDemoTemplate;
			}
			return ProductTileType == TileType.Large ? LargeProductTemplate : SmallProductTemplate;
		}
	}
}
