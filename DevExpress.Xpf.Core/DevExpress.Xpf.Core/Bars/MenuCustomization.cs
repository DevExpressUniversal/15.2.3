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
using System.Windows.Markup;
using DevExpress.Utils;
using DevExpress.Xpf.Utils;
namespace DevExpress.Xpf.Bars {
	public interface IMenuCustomizationItem {
		void Execute(ISupportMenuCustomizations menu);
	}
	public interface ISupportMenuCustomizations {
		MenuCustomizations Customizations { get; }
		void ClearItems();
		void AddItem(BarItem barItem, bool isBeginGroup);
		void RemoveItem(BarItem item);
		void MoveItem(BarItem item, int index);
		BarManager Manager { get; }
		ReadOnlyCollection<BarItem> GetItems();
	}
	public class MenuCustomizations : ObservableCollection<IMenuCustomizationItem>, IMenuCustomizationItem {
		public void Execute(ISupportMenuCustomizations menu) {
			foreach(IMenuCustomizationItem action in Items) {
				action.Execute(menu);
			}
		}
		public void Merge(MenuCustomizations actionList) {
			if(actionList == null) return;
			foreach(IMenuCustomizationItem action in actionList.Items)
				Add(action);
		}
	}
	public class ShowMenuEventArgs<T> : RoutedEventArgs
		where T : PopupMenu, ISupportMenuCustomizations {
		public ShowMenuEventArgs(T menu) {
			Show = true;
			Menu = menu;
			Items = menu.GetItems();
		}
		public T Menu { get; private set; }
		public bool Show { get; set; }
		public ReadOnlyCollection<BarItem> Items { get; private set; }
		public MenuCustomizations ActionList { get { return Menu.Customizations; } }
		public IInputElement TargetElement { get { return (IInputElement)Menu.PlacementTarget; } }
	}
	#region Actions
	[ContentProperty("BarItem")]
	public abstract class MenuCustomizationItemBase : DependencyObject, IMenuCustomizationItem {
		public static readonly DependencyProperty BarItemProperty;
		static MenuCustomizationItemBase() {
			BarItemProperty = DependencyPropertyManager.Register(
					"BarItem", typeof(BarItem), typeof(MenuCustomizationItemBase), new FrameworkPropertyMetadata(null)
				);
		}
		public BarItem BarItem {
			get { return (BarItem)GetValue(BarItemProperty); }
			set { SetValue(BarItemProperty, value); }
		}
		public abstract void Execute(ISupportMenuCustomizations menu);
	}
	public abstract class MenuCustomizationItem : MenuCustomizationItemBase { 
		public static readonly DependencyProperty BarItemNameProperty;
		static MenuCustomizationItem() {
			BarItemNameProperty = DependencyPropertyManager.Register(
					"BarItemName", typeof(string), typeof(MenuCustomizationItem), new FrameworkPropertyMetadata(string.Empty)
				);
		}
		public string BarItemName {
			get { return (string)GetValue(BarItemNameProperty); }
			set { SetValue(BarItemNameProperty, value); }
		}
		protected BarItem GetBarItem(ISupportMenuCustomizations menu) {
			if(BarItem != null) return BarItem;
			return menu.Manager.Items[BarItemName];
		}
	}
	public class Clear : MenuCustomizationItemBase {
		public override void Execute(ISupportMenuCustomizations menu) {
			menu.ClearItems();
		}
	}
	public class Add : MenuCustomizationItem {
		public static readonly DependencyProperty IsBeginGroupProperty;
		static Add() {
			IsBeginGroupProperty = DependencyPropertyManager.Register(
					"IsBeginGroup", typeof(bool), typeof(Add), new FrameworkPropertyMetadata(false)
				);
		}
		public bool IsBeginGroup {
			get { return (bool)GetValue(IsBeginGroupProperty); }
			set { SetValue(IsBeginGroupProperty, value); }
		}
		public override void Execute(ISupportMenuCustomizations menu) {
			BarItem item = GetBarItem(menu);
			if(item == null) throw new ArgumentNullException("BarItem");
			if(string.IsNullOrEmpty(item.Name)) throw new ArgumentException("BarItem.Name");
			menu.AddItem(item, IsBeginGroup);
		}
	}
	public class Remove : MenuCustomizationItem {
		public override void Execute(ISupportMenuCustomizations menu) {
			BarItem item = GetBarItem(menu);
			if(item == null) throw new ArgumentNullException("BarItem");
			menu.RemoveItem(item);
		}
	}
	public class Move : MenuCustomizationItem {
		public static readonly DependencyProperty IndexProperty;
		static Move() {
			IndexProperty = DependencyPropertyManager.Register(
					"Index", typeof(int), typeof(Move), new FrameworkPropertyMetadata(-1)
				);
		}
		public int Index {
			get { return (int)GetValue(IndexProperty); }
			set { SetValue(IndexProperty, value); }
		}
		public override void Execute(ISupportMenuCustomizations menu) {
			BarItem item = GetBarItem(menu);
			if(item == null) throw new ArgumentNullException("BarItem");
			if(Index < 0) throw new ArgumentException("Index");
			menu.MoveItem(item, Index);
		}
	} 
	#endregion Actions
}
