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
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using Microsoft.Windows.Design.PropertyEditing;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Editors;
using DevExpress.Xpf.Editors.Popups;
namespace DevExpress.Xpf.Charts.VisualStudio.Design {
	public partial class ColorCollectionEditorForm : DXWindow {
		#region inner class
		public class ColorItem : DependencyObject {
			public static readonly DependencyProperty ColorProperty;
			public Color Color {
				get { return (Color)GetValue(ColorProperty); }
				set { SetValue(ColorProperty, value); }
			}
			static ColorItem() {
				Type ownerType = typeof(ColorItem);
				ColorProperty = DependencyProperty.Register("Color", typeof(Color), ownerType);
			}
			public ColorItem(Color color) {
				Color = color;
			}
		}
		#endregion
		public static readonly DependencyProperty CurrentColorProperty;
		public static readonly DependencyProperty ColorsProperty;
		public Color CurrentColor {
			get { return (Color)GetValue(CurrentColorProperty); }
			set { SetValue(CurrentColorProperty, value); }
		}
		public ObservableCollection<ColorItem> Colors {
			get { return (ObservableCollection<ColorItem>)GetValue(ColorsProperty); }
			set { SetValue(ColorsProperty, value); }
		}
		static ColorCollectionEditorForm() {
			Type ownerType = typeof(ColorCollectionEditorForm);
			CurrentColorProperty = DependencyProperty.Register("CurrentColor", typeof(Color), ownerType, new FrameworkPropertyMetadata(CurrentColorChanged));
			ColorsProperty = DependencyProperty.Register("Colors", typeof(ObservableCollection<ColorItem>), ownerType, new FrameworkPropertyMetadata(null));  
		}
		static void CurrentColorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			ColorCollectionEditorForm form = d as ColorCollectionEditorForm;
			if (form != null) {
				ColorItem item = form.listBoxColors.SelectedItem as ColorItem;
				if (item != null)
					item.Color = form.colorChooser.Color;
			}
		}
		readonly PropertyValueCollection editValue;
		public ColorCollectionEditorForm(PropertyValueCollection collection) {
			InitializeComponent();
			editValue = collection;
			Colors = new ObservableCollection<ColorItem>();			
			InitColors();
			InitColorChooser();
		}
		void InitColors() {
			if (editValue != null) {
				for (int i = 0; i < editValue.Count; i++){
					ColorItem item = new ColorItem((Color)(editValue[i].Value));
					Colors.Add(item);
				}
			}			
		}
		void InitColorChooser() {
			Binding binding = new Binding("Color");
			binding.Source = colorChooser;
			this.SetBinding(CurrentColorProperty, binding);
		}
		void UpdateControls() { 
			btnRemove.IsEnabled = listBoxColors.SelectedItem != null;
			btnUp.IsEnabled = listBoxColors.SelectedIndex > 0;
			btnDown.IsEnabled = listBoxColors.SelectedIndex < Colors.Count - 1;
		}
		void listBoxColors_SelectedIndexChanged(object sender, RoutedEventArgs e) {
			ColorItem item = listBoxColors.SelectedItem as ColorItem;
			if (item != null)
				colorChooser.Color = (Color)item.Color;
			UpdateControls(); 
		}
		void btnAdd_Click(object sender, RoutedEventArgs e) {
			ColorItem item = new ColorItem(colorChooser.Color);
			if (listBoxColors.SelectedIndex > -1)
				Colors.Insert(listBoxColors.SelectedIndex + 1, item);
			else 
				Colors.Add(item);			
			listBoxColors.SelectedItem = item;
		}
		void btnRemove_Click(object sender, RoutedEventArgs e) {
			int selectedIndex = listBoxColors.SelectedIndex;
			if (selectedIndex >= 0)
				Colors.RemoveAt(selectedIndex);
			selectedIndex = selectedIndex == Colors.Count ? selectedIndex - 1 : selectedIndex;
			listBoxColors.SelectedIndex = selectedIndex;
		}
		void btnUp_Click(object sender, RoutedEventArgs e) {
			Colors.Move(listBoxColors.SelectedIndex, listBoxColors.SelectedIndex - 1);
			UpdateControls(); 
		}
		void btnDown_Click(object sender, RoutedEventArgs e) {
			Colors.Move(listBoxColors.SelectedIndex, listBoxColors.SelectedIndex + 1);
			UpdateControls(); 
		}
		void btnCancel_Click(object sender, RoutedEventArgs e) {
			Close();
		}
		void btnOK_Click(object sender, RoutedEventArgs e) {
			if (editValue != null) {
				while (editValue.Count > 0)
					editValue.RemoveAt(0);
				foreach (ColorItem item in Colors) 
					editValue.Add(item.Color);				
			}
			Close();
		}
		void DXWindow_Activated(object sender, EventArgs e) {
			if (MinHeight == 0)
				MinHeight = ActualHeight;
			if (MinWidth == 0)
				MinWidth = ActualWidth;
		}
	}
}
