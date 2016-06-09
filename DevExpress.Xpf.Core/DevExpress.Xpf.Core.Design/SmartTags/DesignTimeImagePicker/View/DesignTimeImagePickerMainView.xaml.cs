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
namespace DevExpress.Xpf.Core.Design.SmartTags {
	public partial class DesignTimeImagePickerMainView : UserControl {
		public DesignTimeImagePickerMainView() {
			InitializeComponent();
		}
		void DesignTimeListBox_SelectionChanged(object sender, SelectionChangedEventArgs e) {
			DesignTimeListBox designTimeListBox = sender as DesignTimeListBox;
			DesignTimeImagePickerFilterItem selectedItem = (DesignTimeImagePickerFilterItem)designTimeListBox.SelectedItem;
			string name = selectedItem.Name;
			if(name.Equals("Select All") || selectedItem.IsSelected != true)
				return;
			ShowSelectedCategory(name, designTimeListBox);
		}
		void ShowSelectedCategory(string selectedItemName, DependencyObject element) {
			DesignTimeAdaptablePanel panel = GetParent<DesignTimeAdaptablePanel>(element);
			DesignTimeTreeView treeView = GetChild<DesignTimeTreeView>(panel);
			ScrollViewer scrollViewer = GetChild<ScrollViewer>(treeView);
			double scrollViewerActualHeight = scrollViewer.ActualHeight;
			ScrollContentPresenter presenter = GetChild<ScrollContentPresenter>(treeView);
			StackPanel stackPanel = GetChild<StackPanel>(presenter);
			List<DesignTimeTreeViewItem> listTreeViewItem = GetChildren<DesignTimeTreeViewItem>(stackPanel);
			foreach(DesignTimeTreeViewItem item in listTreeViewItem) {
				if(selectedItemName.Equals(GetCategoryName(item))) {
					item.BringIntoView(new Rect(new Point(0.0, 0.0), new Point(0.0, scrollViewerActualHeight)));
					break;
				}
			}
		}
		string GetCategoryName(DesignTimeTreeViewItem item) {
			TextBlock textBlock = GetChild<TextBlock>(item);
			return textBlock == null ? null : textBlock.Text;
		}
		List<T> GetChildren<T>(DependencyObject element) where T : DependencyObject {
			List<T> listT = new List<T>();
			for(int i=0;i<VisualTreeHelper.GetChildrenCount(element);i++)
				listT.Add((T)VisualTreeHelper.GetChild(element, i));
			return listT;
		}
		T GetParent<T>(DependencyObject parent) where T : DependencyObject {
			while(parent != null && !(parent is T))
				parent = VisualTreeHelper.GetParent(parent) as DependencyObject;
			return parent as T;
		}
		T GetChild<T>(DependencyObject element) where T : DependencyObject {
			int count=VisualTreeHelper.GetChildrenCount(element);
			for(int i = 0; i < count; i++) {
				DependencyObject child = VisualTreeHelper.GetChild(element, i);
				if(child is T)
					return (T)child;
				T childT = GetChild<T>(child);
				if(childT != null)
					return childT;
			}
			return null;
		}
	}
}
