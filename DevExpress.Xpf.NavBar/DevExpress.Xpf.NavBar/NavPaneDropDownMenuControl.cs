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
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows;
using System.Windows.Controls.Primitives;
using DevExpress.Xpf.Bars;
using DevExpress.Xpf.Core.Native;
using System.Windows.Data;
using DevExpress.Xpf.Utils;
using System.Windows.Media;
using DevExpress.Mvvm.Native;
using DevExpress.Xpf.Core.Internal;
namespace DevExpress.Xpf.NavBar {
	public class NavBarPopupMenu : PopupMenu {
		protected override void UpdatePlacement(UIElement control) {
			base.UpdatePlacement(control);
			if(DataContext is NavBarControl && ((NavBarControl)DataContext).View.Orientation == Orientation.Horizontal) {
				RenderTransform = new RotateTransform() { Angle = 90 };
				VerticalOffset -= ((FrameworkElement)control).ActualWidth;
			}
		}
	}
	public static class BarItemNames {
		public const string ShowMoreButtons = "btShowMoreButtons";
		public const string ShowFewerButtons = "btShowFewerButtons";
		public const string GroupsMenu = "smGroupsMenu";
		public const string HiddenGroups = "smHiddenGroups";
	}
	public partial class NavPaneDropDownMenuControl : ToggleButton {
		static NavPaneDropDownMenuControl() {
			IsManipulationEnabledProperty.OverrideMetadata(typeof(NavPaneDropDownMenuControl), new FrameworkPropertyMetadata(true));
			DefaultStyleKeyProperty.OverrideMetadata(typeof(NavPaneDropDownMenuControl), new FrameworkPropertyMetadata(typeof(NavPaneDropDownMenuControl)));
			createBindingClone = ReflectionHelper.CreateInstanceMethodHandler<Func<BindingBase, BindingMode, BindingBase>>(null, "Clone", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic, typeof(BindingBase));
		}
		public NavPaneDropDownMenuControl() {
			Unloaded += OnUnloaded;
		}
		void OnUnloaded(object sender, RoutedEventArgs e) {
			ClearMenu();
		}
		public NavBarPopupMenu DropDownMenu {
			get;
			set;
		}
		public BarManagerMenuController MenuController { get; private set; }
		protected override void OnClick() {
			base.OnClick();			
			UpdateMenu();
			DropDownMenu.PlacementTarget = this;
			DropDownMenu.Placement = PlacementMode.Right;
			DropDownMenu.ShowPopup(this);
		}
		protected virtual void ClearMenu() {
			if(DropDownMenu != null)
				DropDownMenu.ItemLinks.Clear();
		}
		protected virtual void InitializeMenu() {
			DropDownMenu = new NavBarPopupMenu();
			MenuController = new BarManagerMenuController(DropDownMenu);
#if DEBUGTEST
			(NavBar.View as NavigationPaneView).NavPaneDropDownMenu = DropDownMenu;
#endif
			DropDownMenu.Closed += OnDropDownMenuClosed;
			MenuController.ActionContainer.Actions.Clear();
			BarButtonItem showMoreButtons = new BarButtonItem() { Name = BarItemNames.ShowMoreButtons };
			BarButtonItem showFewerButtons = new BarButtonItem() { Name = BarItemNames.ShowFewerButtons };
			BarSubItem groupMenu = new BarSubItem() { Name = BarItemNames.GroupsMenu };
			groupMenu.Content = NavBarLocalizer.GetString(NavBarStringId.NavPaneMenuAddRemoveButtons);
			showMoreButtons.Content = NavBarLocalizer.GetString(NavBarStringId.NavPaneMenuShowMoreButtons);
			showMoreButtons.Command = NavigationPaneCommands.ShowMoreGroups;
			showMoreButtons.CommandTarget = NavBar.View;
			showFewerButtons.Content = NavBarLocalizer.GetString(NavBarStringId.NavPaneMenuShowFewerButtons);
			showFewerButtons.Command = NavigationPaneCommands.ShowFewerGroups;
			showFewerButtons.CommandTarget = NavBar.View;
			CreateGroupItems(groupMenu);
#if DEBUGTEST
			groupMenu.Tag = "groupMenu";
#endif
			MenuController.ActionContainer.Actions.Add(showMoreButtons);
			MenuController.ActionContainer.Actions.Add(showFewerButtons);
			MenuController.ActionContainer.Actions.Add(groupMenu);
			MenuController.ActionContainer.Actions.Add(new BarItemLinkSeparator());
			var hiddenGroups = new BarLinkContainerItem() { Name = BarItemNames.HiddenGroups };
			foreach (var group in NavBar.Groups) {
				hiddenGroups.Items.Add(group.MenuGroupItem);
			}
			MenuController.ActionContainer.Actions.Add(hiddenGroups);
			if (NavBar != null && NavBar.View as NavigationPaneView != null && ((NavigationPaneView)(NavBar.View)).MenuCustomizations != null) {
				foreach (var item in ((NavigationPaneView)(NavBar.View)).MenuCustomizations) {
					MenuController.ActionContainer.Actions.Add(item);
				}
			}
			MenuController.Execute();
		}
		void OnDropDownMenuClosed(object sender, EventArgs e) {
#if DEBUGTEST
			if (NavBar != null && NavBar.View as NavigationPaneView != null)
				(NavBar.View as NavigationPaneView).NavPaneDropDownMenu = null;
#endif            
			DropDownMenu.Closed -= OnDropDownMenuClosed;
		}
		protected virtual void UpdateMenu() {
			ClearMenu();
			InitializeMenu();
		}
		protected NavBarControl NavBar { get { return DataContext as NavBarControl; } }
		protected virtual void CreateGroupItems(BarSubItem groupMenu) {
			foreach(NavBarGroup group in NavBar.Groups) {
				BarItem item = CreateGroupItems(group);
				groupMenu.Items.Add(item);
			}
		}
		static readonly Func<BindingBase, BindingMode, BindingBase> createBindingClone;		
		protected virtual BarItem CreateGroupItems(NavBarGroup group) {
			BarCheckItem item = new BarCheckItem() { Name = "GroupItem" + ItemCount.ToString() };
			item.Tag = true;
			if (group.MenuItemDisplayBinding != null) {
				item.SetBinding(BarItem.ContentProperty, (createBindingClone(group.MenuItemDisplayBinding, BindingMode.OneWay) as Binding).Do(x => x.Source = group.Header));
			} else {
				if ((group.Header as DependencyObject).With(LogicalTreeHelper.GetParent) == null)
					item.Content = group.Header;
				else
					item.Content = group.Header.ToString();
			}
			Binding b = new Binding("ActualIsVisible") { Source = group, Mode = BindingMode.OneWay };
			item.CheckedChanged += new ItemClickEventHandler((o, a) => { group.CustomIsVisible = ((BarCheckItem)o).IsChecked == true; });
			BindingOperations.SetBinding(item, BarCheckItem.IsCheckedProperty,  b);
			item.ContentTemplate = group.HeaderTemplate;
			ItemCount++;
			return item;
		}
#if DEBUGTEST 
		public void DisplayPopup(){
			OnClick();
		}
#endif
		static int ItemCount = 0;
		#region Touch
		bool emulateClick = false;
		TouchPoint point = null;
		protected override void OnTouchDown(System.Windows.Input.TouchEventArgs e) {
			base.OnTouchDown(e);
			point = e.GetTouchPoint(this);
			emulateClick = true;
		}
		protected override void OnTouchMove(System.Windows.Input.TouchEventArgs e) {
			base.OnTouchMove(e);
			if(point == null || !(object.Equals(e.GetTouchPoint(this).Position, point.Position)))
				emulateClick = false;
		}
		protected override void OnTouchUp(System.Windows.Input.TouchEventArgs e) {
			base.OnTouchUp(e);
			if(emulateClick) {
				point = null;
				emulateClick = false;
				if(!e.Handled) {
					OnClick();
				}
			}
		}
		#endregion
	}	
}
