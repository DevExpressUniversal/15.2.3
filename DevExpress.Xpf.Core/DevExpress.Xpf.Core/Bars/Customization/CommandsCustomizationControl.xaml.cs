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

using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using DevExpress.Xpf.Utils;
using System.Windows.Media;
using System;
using DevExpress.Mvvm.Native;
using System.Linq;
namespace DevExpress.Xpf.Bars.Customization {
	public partial class CommandsCustomizationControl : UserControl {
		public static double DragStartDistance = 5.0;
		BarManagerCustomizationHelper helper;
		public CommandsCustomizationControl(BarManagerCustomizationHelper helper) {
			InitializeComponent();
			this.helper = helper;
		}		
		protected virtual void OnLoaded(object sender, RoutedEventArgs e) {
			InitializeCategoriesList();
			categoriesList.SelectedIndex = 0;
			OnCategoriesListSelectionChanged(categoriesList, null);
		}
		protected virtual void InitializeCommandsList(BarManagerCategory cat) {
			commandsList.Items.Clear();
			if(cat == null) return;
			List<BarItem> itemList = cat.GetBarItems();
			foreach(BarItem item in itemList) {
				if(item.IsPrivate) continue;
				if (item is BarItemSeparator) continue;
				commandsList.Items.Add(new BarItemInfo() { Item = item });
			}
		}
		protected virtual void InitializeCategoriesList() {						
			categoriesList.Items.Clear();			
			foreach(BarManagerCategory cat in helper.GetCategories()) {
				var format = cat.IsSpecialCategory ? "{0}" : "[{0}]";
				var caption = string.IsNullOrEmpty(cat.Caption) ? cat.Name : cat.Caption;
				categoriesList.Items.Add(String.Format(format, caption));
			}
		}
		protected virtual void OnCategoriesListSelectionChanged(object sender, SelectionChangedEventArgs e) {			
			ListBox lb = ((ListBox)sender);
			var index = lb.SelectedIndex;
			var categories = helper.GetCategories().ToList();
			if (!categories.IsValidIndex(index))
				return;			
			InitializeCommandsList(categories[index]);
		}
	}
	public class BarItemInfo : DependencyObject {
		#region static
		public static readonly DependencyProperty ItemProperty;
		public static readonly DependencyProperty GlyphProperty;
		public static readonly DependencyProperty DescriptionProperty;
		static BarItemInfo() {
			ItemProperty = DependencyPropertyManager.Register("Item", typeof(BarItem), typeof(BarItemInfo), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsMeasure, new PropertyChangedCallback(OnItemPropertyChanged)));
			GlyphProperty = DependencyPropertyManager.Register("Glyph", typeof(ImageSource), typeof(BarItemInfo), new FrameworkPropertyMetadata(null));
			DescriptionProperty = DependencyPropertyManager.Register("Description", typeof(string), typeof(BarItemInfo), new FrameworkPropertyMetadata(string.Empty));
		}
		protected static void OnItemPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((BarItemInfo)d).UpdateProperties();
		}
		#endregion
		public BarItem Item {
			get { return (BarItem)GetValue(ItemProperty); }
			set { SetValue(ItemProperty, value); }
		}
		public ImageSource Glyph {
			get { return (ImageSource)GetValue(GlyphProperty); }
			set { SetValue(GlyphProperty, value); }
		}
		public string Description {
			get { return (string)GetValue(DescriptionProperty); }
			set { SetValue(DescriptionProperty, value); }
		}
		protected virtual void UpdateProperties() {
			OnGlyphChanged();
			UpdateDescription();
		}
		protected virtual void UpdateDescription() {
			Description = Item.GetDescription();
		}
		protected virtual void OnGlyphChanged() {
			Glyph = Item.ActualCustomizationGlyph;
		}
	}
}
