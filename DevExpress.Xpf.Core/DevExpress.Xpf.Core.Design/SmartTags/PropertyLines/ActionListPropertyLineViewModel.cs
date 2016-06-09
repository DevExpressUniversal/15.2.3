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

using DevExpress.Design.SmartTags;
using DevExpress.Design.UI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
namespace DevExpress.Xpf.Core.Design {
	public interface IActionListDataContext : INotifyPropertyChanged {
		IPropertyLineContext Context { get; }
		IEnumerable<MenuItemInfo> Items { get; set; }
		MenuItemInfo SelectedItem { get; set; }
		ICommand SelectedItemCommand { get; set; }
	}
	public abstract class ActionListPropertyLineContext : WpfBindableBase, IActionListDataContext {
		public IEnumerable<MenuItemInfo> Items {
			get {
				UpdateItems(items);
				return items; 
			}
			set { SetProperty(ref items, value, () => Items); }
		}
		protected virtual void UpdateItems(IEnumerable<MenuItemInfo> items) { }
		public MenuItemInfo SelectedItem {
			get { return selectedItem; }
			set { SetProperty(ref selectedItem, value, () => SelectedItem, () => OnSelectedItemChanged(selectedItem)); }
		}
		public ICommand SelectedItemCommand { get; set; }
		public ICommand SetSelectedItemCommand { get; protected set; }
		public IPropertyLineContext Context {
			get { return context; }
		}
		public ActionListPropertyLineContext(IPropertyLineContext context) {
			this.context = context;
			InitializeCommands();
			InitializeItems();
		}
		protected abstract void InitializeItems();
		protected abstract void OnSelectedItemExecute(MenuItemInfo param);
		protected virtual void InitializeCommands() {
			SetSelectedItemCommand = new WpfDelegateCommand<MenuItemInfo>(OnSetSelectedItemExecute);
			SelectedItemCommand = new WpfDelegateCommand<MenuItemInfo>(OnSelectedItemExecute);
		}
		protected virtual void OnSetSelectedItemExecute(MenuItemInfo param) {
			SelectedItem = param;
		}
		protected virtual void OnSelectedItemChanged(MenuItemInfo oldValue) { }
		MenuItemInfo selectedItem;
		IEnumerable<MenuItemInfo> items;
		IPropertyLineContext context;
	}
	public class ActionListPropertyLineViewModel : SmartTagLineViewModelBase {
		public ICommand ShowContextMenu { get; private set; }
		public IEnumerable<MenuItemInfo> Items {
			get { return dataContext.Items; }
			set { dataContext.Items = value; }
		}
		public MenuItemInfo SelectedMenuItem {
			get { return dataContext.SelectedItem; }
			set { dataContext.SelectedItem = value; }
		}
		public ICommand SelectedItemCommand { get { return dataContext.SelectedItemCommand; } }
		public string Text {
			get { return text; }
			set { SetProperty(ref text, value, ()=>Text); }
		}
		public ActionListPropertyLineViewModel(IActionListDataContext dataContext)
			: base(dataContext.Context) {
			this.dataContext = dataContext;
			dataContext.PropertyChanged += OnDataContextPropertyChanged;
			ShowContextMenu = new WpfDelegateCommand<object>(OpenContextMenuAction);
		}
		void OnDataContextPropertyChanged(object sender, PropertyChangedEventArgs e) {
			if(e.PropertyName == "SelectedItem")
				RaisePropertiesChanged("SelectedMenuItem");
			else
				RaisePropertiesChanged(e.PropertyName);
		}
		void OpenContextMenuAction(object param) {
			var fe = param as System.Windows.FrameworkElement;
			var cm = System.Windows.Controls.ContextMenuService.GetContextMenu(fe);
			if(cm != null && fe != null) {
				cm.Placement = System.Windows.Controls.ContextMenuService.GetPlacement(fe);
				cm.PlacementTarget = System.Windows.Controls.ContextMenuService.GetPlacementTarget(fe);
				cm.IsOpen = true;
			}
		}
		IActionListDataContext dataContext;
		string text;
	}
	public class MenuItemInfo : WpfBindableBase {
		public string Caption {
			get { return caption; }
			set { SetProperty(ref caption, value, () => Caption); }
		}
		public ICommand Command {
			get { return command; }
			set { SetProperty(ref command, value, () => Command); }
		}
		public Image Image {
			get { return image; }
			set { SetProperty(ref image, value, () => Image); }
		}
		public Uri ImageSource {
			get { return imageSource; }
			set {
				SetProperty(ref imageSource, value, () => ImageSource, OnImageSourceChanged);
			}
		}
		public ObservableCollection<MenuItemInfo> SubItems {
			get { return subItems; }
			set { SetProperty(ref subItems, value, () => SubItems); }
		}
		public object Tag {
			get { return tag; }
			set { SetProperty(ref tag, value, () => Tag); }
		}
		public bool IsChecked {
			get { return isChecked; }
			set { SetProperty(ref isChecked, value, () => IsChecked); }
		}
		public bool IsVisible {
			get { return isVisible; }
			set { SetProperty(ref isVisible, value, () => IsVisible); }
		}
		public MenuItemInfo() {
			isVisible = true;
		}
		protected virtual void OnImageSourceChanged() {
			try {
				Image = ImageSource == null ? null : new Image() { Source = new BitmapImage(ImageSource), MaxHeight = 16, MaxWidth = 16 };
			} catch {
				ImageSource = null;
			}
			RaisePropertyChanged("Image");
		}
		Uri imageSource;
		Image image;
		string caption;
		ICommand command;
		ObservableCollection<MenuItemInfo> subItems;
		object tag;
		bool isChecked, isVisible;
	}
}
