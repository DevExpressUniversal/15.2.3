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

using DevExpress.Mvvm.Native;
using DevExpress.Mvvm.UI;
using DevExpress.Utils;
using DevExpress.Xpf.Core.Native;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;
namespace DevExpress.Xpf.Core {
	public enum TabControlDragDropMode { ReorderOnly, ReorderAndMove, Full, None }
	public enum TabPinMode { None, Left, Right }
	public class TabControlStretchView : TabControlViewBase {
		#region Properties
		public static readonly DependencyProperty PinModeProperty = DependencyProperty.RegisterAttached("PinMode", typeof(TabPinMode), typeof(TabControlStretchView),
			new PropertyMetadata(TabPinMode.None, (d, e) => OnPinModeChanged((DXTabItem)d, (TabPinMode)e.OldValue, (TabPinMode)e.NewValue)));
		public static TabPinMode GetPinMode(DXTabItem obj) { return (TabPinMode)obj.GetValue(PinModeProperty); }
		public static void SetPinMode(DXTabItem obj, TabPinMode value) { obj.SetValue(PinModeProperty, value); }
		public static readonly DependencyProperty PinnedTabAllowHideProperty = DependencyProperty.Register("PinnedTabAllowHide", typeof(bool), typeof(TabControlStretchView), 
			new PropertyMetadata(false, (d, e) => ((TabControlStretchView)d).UpdateViewProperties()));
		public static readonly DependencyProperty PinnedTabAllowDragProperty = DependencyProperty.Register("PinnedTabAllowDrag", typeof(bool), typeof(TabControlStretchView), new PropertyMetadata(false));
		public static readonly DependencyProperty PinnedTabSizeProperty = DependencyProperty.Register("PinnedTabSize", typeof(int), typeof(TabControlStretchView), new PropertyMetadata(0));
		public static readonly DependencyProperty TabNormalSizeProperty = DependencyProperty.Register("TabNormalSize", typeof(int), typeof(TabControlStretchView), new PropertyMetadata(160));
		public static readonly DependencyProperty SelectedTabMinSizeProperty = DependencyProperty.Register("SelectedTabMinSize", typeof(int), typeof(TabControlStretchView), new PropertyMetadata(80));
		public static readonly DependencyProperty TabMinSizeProperty = DependencyProperty.Register("TabMinSize", typeof(int), typeof(TabControlStretchView), new PropertyMetadata(35));
		public static readonly DependencyProperty MoveItemsWhenDragDropProperty = DependencyProperty.Register("MoveItemsWhenDragDrop", typeof(bool), typeof(TabControlStretchView), new PropertyMetadata(true));
		public static readonly DependencyProperty DragDropModeProperty = DependencyProperty.Register("DragDropMode", typeof(TabControlDragDropMode), typeof(TabControlStretchView), new PropertyMetadata(TabControlDragDropMode.ReorderOnly));
		public static readonly DependencyProperty DragDropRegionProperty = DependencyProperty.Register("DragDropRegion", typeof(string), typeof(TabControlStretchView),
			new PropertyMetadata((d, e) => ((TabControlStretchView)d).OnDragDropRegionChanged((string)e.OldValue, (string)e.NewValue)));
		public static readonly DependencyProperty CloseWindowOnSingleTabItemHidingProperty = DependencyProperty.Register("CloseWindowOnSingleTabItemHiding", typeof(bool), typeof(TabControlStretchView), new PropertyMetadata(false));
		public static readonly DependencyProperty NewWindowStyleProperty = DependencyProperty.Register("NewWindowStyle", typeof(Style), typeof(TabControlStretchView), new PropertyMetadata(null));
		public static readonly DependencyProperty NewTabControlStyleProperty = DependencyProperty.Register("NewTabControlStyle", typeof(Style), typeof(TabControlStretchView), new PropertyMetadata(null));
		public bool PinnedTabAllowHide { get { return (bool)GetValue(PinnedTabAllowHideProperty); } set { SetValue(PinnedTabAllowHideProperty, value); } }
		public bool PinnedTabAllowDrag { get { return (bool)GetValue(PinnedTabAllowDragProperty); } set { SetValue(PinnedTabAllowDragProperty, value); } }
		public int PinnedTabSize { get { return (int)GetValue(PinnedTabSizeProperty); } set { SetValue(PinnedTabSizeProperty, value); } }
		public int TabNormalSize { get { return (int)GetValue(TabNormalSizeProperty); } set { SetValue(TabNormalSizeProperty, value); } }
		public int SelectedTabMinSize { get { return (int)GetValue(SelectedTabMinSizeProperty); } set { SetValue(SelectedTabMinSizeProperty, value); } }
		public int TabMinSize { get { return (int)GetValue(TabMinSizeProperty); } set { SetValue(TabMinSizeProperty, value); } }
		public bool MoveItemsWhenDragDrop { get { return (bool)GetValue(MoveItemsWhenDragDropProperty); } set { SetValue(MoveItemsWhenDragDropProperty, value); } }
		public TabControlDragDropMode DragDropMode { get { return (TabControlDragDropMode)GetValue(DragDropModeProperty); } set { SetValue(DragDropModeProperty, value); } }
		public string DragDropRegion { get { return (string)GetValue(DragDropRegionProperty); } set { SetValue(DragDropRegionProperty, value); } }
		public Style NewWindowStyle { get { return (Style)GetValue(NewWindowStyleProperty); } set { SetValue(NewWindowStyleProperty, value); } }
		public Style NewTabControlStyle { get { return (Style)GetValue(NewTabControlStyleProperty); } set { SetValue(NewTabControlStyleProperty, value); } }
		public bool CloseWindowOnSingleTabItemHiding { get { return (bool)GetValue(CloseWindowOnSingleTabItemHidingProperty); } set { SetValue(CloseWindowOnSingleTabItemHidingProperty, value); } }
		protected internal TabPanelStretchView StretchPanel { get { return Owner.With(x => x.TabPanel).With(x => x.Panel as TabPanelStretchView); } }
		#endregion Properties
		static void OnPinModeChanged(DXTabItem tab, TabPinMode oldValue, TabPinMode newValue) {
			var view = tab.With(x => x.Owner).With(x => x.View.StretchView);
			view.With(x => x.StretchPanel).Do(x => x.ForceUpdateLayout());
			view.Do(x => x.UpdateViewProperties());
		}
		protected internal override int CoerceSelection(int index, NotifyCollectionChangedAction? originativeAction) {
			if(originativeAction != NotifyCollectionChangedAction.Remove) return base.CoerceSelection(index, originativeAction);
			if(Owner == null) return index;
			var selectedContainer = Owner.SelectedContainer ?? Owner.GetContainer(index);
			if(selectedContainer == null || index < 0) return Owner.GetCoercedSelectedIndexCore(Owner.GetContainers(), index);
			var pinMode = GetPinMode(selectedContainer);
			var all = Owner.GetContainers().ToList();
			var pinNone = all.Where(x => GetPinMode(x) == TabPinMode.None).ToList();
			var pinLeft = all.Where(x => GetPinMode(x) == TabPinMode.Left).ToList();
			var pinRight = all.Where(x => GetPinMode(x) == TabPinMode.Right).ToList();
			Func<IList<DXTabItem>, int, int> mapIndex = (filteredChildren, ind) => {
				ind = Math.Max(0, ind);
				ind = Math.Min(filteredChildren.Count - 1, ind);
				var resIndex = Owner.GetCoercedSelectedIndexCore(filteredChildren, ind);
				return all.IndexOf(filteredChildren.ElementAtOrDefault(resIndex));
			};
			Func<IList<DXTabItem>, int> mapLastIndex = (filteredChildren) => {
				var lastIndex = Owner.GetCoercedSelectedIndexCore(filteredChildren, filteredChildren.Count - 1);
				return all.IndexOf(filteredChildren.ElementAtOrDefault(lastIndex));
			};
			Func<IList<DXTabItem>, int> getVisibleItemsCount = (filteredChildren) => {
				return filteredChildren.Where(Owner.IsVisibleAndEnabledItem).Count();
			};
			if(pinMode == TabPinMode.None) {
				if(getVisibleItemsCount(pinNone) > 0) return mapIndex(pinNone, index - pinLeft.Count - pinRight.Count);
				if(getVisibleItemsCount(pinLeft) > 0) return mapLastIndex(pinLeft);
				if(getVisibleItemsCount(pinRight) > 0) return mapLastIndex(pinRight);
				return -1;
			}
			if(pinMode == TabPinMode.Left) {
				if(getVisibleItemsCount(pinLeft) > 0) return mapIndex(pinLeft, index - pinNone.Count - pinRight.Count);
				if(getVisibleItemsCount(pinNone) > 0) return mapLastIndex(pinNone);
				if(getVisibleItemsCount(pinRight) > 0) return mapLastIndex(pinRight);
				return -1;
			}
			if(pinMode == TabPinMode.Right) {
				if(getVisibleItemsCount(pinRight) > 0) return mapIndex(pinRight, index - pinNone.Count - pinLeft.Count);
				if(getVisibleItemsCount(pinNone) > 0) return mapLastIndex(pinNone);
				if(getVisibleItemsCount(pinLeft) > 0) return mapLastIndex(pinLeft);
				return -1;
			}
			return -1;
		}
		protected internal override void UpdateViewPropertiesCore() {
			if(lockUpdateDragDropControls) return;
			base.UpdateViewPropertiesCore();
			UpdateDragDropControls(true, false, x => x.Owner.UpdateViewProperties());
		}
		protected override void OnOwnerChanged(DXTabControl oldValue, DXTabControl newValue) {
			base.OnOwnerChanged(oldValue, newValue);
			oldValue.Do(x => x.Loaded -= OnOwnerLoaded);
			oldValue.Do(x => x.Unloaded -= OnOwnerUnloaded);
			newValue.Do(x => x.Loaded += OnOwnerLoaded);
			newValue.Do(x => x.Unloaded += OnOwnerUnloaded);
			oldValue.Do(x => DragDropRegionManager.UnregisterDragDropRegion(x, DragDropRegion));
			newValue.Do(x => DragDropRegionManager.RegisterDragDropRegion(x, DragDropRegion));
		}
		void OnOwnerLoaded(object sender, EventArgs e) {
			DXTabControl owner = (DXTabControl)sender;
			owner.Do(x => DragDropRegionManager.UnregisterDragDropRegion(x, DragDropRegion));
			owner.Do(x => DragDropRegionManager.RegisterDragDropRegion(x, DragDropRegion));
		}
		void OnOwnerUnloaded(object sender, EventArgs e) {
			DXTabControl owner = (DXTabControl)sender;
			owner.Do(x => DragDropRegionManager.UnregisterDragDropRegion(x, DragDropRegion));
		}
		void OnDragDropRegionChanged(string oldValue, string newValue) {
			Owner.Do(x => DragDropRegionManager.UnregisterDragDropRegion(x, oldValue));
			Owner.Do(x => DragDropRegionManager.RegisterDragDropRegion(x, newValue));
		}
		protected internal override bool CanCloseTabItem(DXTabItem item) {
			if(GetPinMode(item) != TabPinMode.None && !PinnedTabAllowHide) return false;
			if(base.CanCloseTabItem(item)) return true;
			return AreAnyTabControls();
		}
		protected internal override void OnTabItemClosed(DXTabItem item) {
			OnTabItemClosedCore(item, true);
		}
		void OnTabItemClosedCore(DXTabItem item, bool closeOwnerWindowImmediately) {
			if(Owner == null || Owner.VisibleItemsCount > 0) return;
			bool isLastTabItem = true;
			var tabControls = GetDragTabControls(true, true);
			foreach(var tabControl in tabControls) {
				if(tabControl.VisibleItemsCount > 0) {
					isLastTabItem = false;
					break;
				}
			}
			if(!isLastTabItem && CloseWindowOnSingleTabItemHiding) {
				CloseWindow(Window.GetWindow(Owner), closeOwnerWindowImmediately);
				return;
			}
			if(isLastTabItem && SingleTabItemHideMode == SingleTabItemHideMode.HideAndShowNewItem) {
				Owner.AddNewTabItem();
				return;
			}
			if(isLastTabItem && SingleTabItemHideMode == SingleTabItemHideMode.Hide && CloseWindowOnSingleTabItemHiding) {
				CloseWindow(Window.GetWindow(Owner), closeOwnerWindowImmediately);
				return;
			}
		}
		#region DragDrop 
		void CloseWindow(Window wnd, bool closeImmediately) {
			if(wnd == null) return;
			try {
				if(closeImmediately) {
					windowsToClose.Remove(wnd);
					wnd.Close();
				} else {
					windowsToClose.Add(wnd);
					wnd.Hide();
				}
			} catch { }
		}
		void CloseWindowsToClose() {
			foreach(var wnd in windowsToClose.ToList())
				CloseWindow(wnd, true);
		}
		List<Window> windowsToClose = new List<Window>();
		IEnumerable<DXTabControl> GetDragTabControls(bool excludeOwner, bool excludeInvisible) {
			var tabControls = DragDropRegionManager.GetDragDropControls(DragDropRegion).OfType<DXTabControl>();
			if(excludeOwner)
				tabControls = tabControls.Where(x => x != Owner);
			if(excludeInvisible)
				tabControls = tabControls.Where(x => x.IsVisible);
			return tabControls;
		}
		bool lockUpdateDragDropControls = false;
		void UpdateDragDropControls(bool excludeOwner, bool excludeInvisible, Action<TabControlStretchView> updateAction) {
			if(lockUpdateDragDropControls) return;
			var views = GetDragTabControls(excludeOwner, excludeInvisible).Select(x => x.View.StretchView);
			views.ForEach(x => x.lockUpdateDragDropControls = true);
			views.ForEach(x => updateAction(x));
			views.ForEach(x => x.lockUpdateDragDropControls = false);
		}
		bool AreAnyTabControls() {
			var tabControls = GetDragTabControls(true, true);
			foreach(var tabControl in tabControls) {
				if(tabControl.VisibleItemsCount > 0)
					return true;
			}
			return false;
		}
		internal void OnDragFinished() {
			UpdateDragDropControls(false, false, x => x.CloseWindowsToClose());
		}
		internal bool CanStartDrag(DXTabItem child) {
			if(GetPinMode(child) != TabPinMode.None && !PinnedTabAllowDrag)
				return false;
			if(DragDropMode == TabControlDragDropMode.None)
				return false;
			if(DragDropMode == TabControlDragDropMode.ReorderOnly)
				return Owner.VisibleItemsCount > 1;
			return true;
		}
		internal void SetVisibility(DXTabItem child, Visibility visibility) {
			if(visibility == Visibility.Visible) Owner.ShowTabItem(child, false);
			else if(visibility == Visibility.Collapsed) Owner.HideTabItem(child, false);
			else child.Visibility = visibility;
			if(visibility == Visibility.Collapsed && DragDropMode == TabControlDragDropMode.Full && Owner.VisibleItemsCount == 0)
				Window.GetWindow(Owner).Do(x => CloseWindow(x, false));
		}
		internal void Insert(DXTabItem child, int index) {
			child.Insert(Owner, index);
		}
		internal void Remove(DXTabItem child) {
			child.Remove();
			OnTabItemClosedCore(child, false);
		}
		internal void Move(DXTabItem child, int index) {
			child.Move(index, MoveItemsWhenDragDrop);
		}
		internal void DropOnEmptySpace(DXTabItem child) {
			if(DragDropMode != TabControlDragDropMode.Full) {
				SetVisibility(child, Visibility.Visible);
				return;
			}
			SetVisibility(child, Visibility.Visible);
			Remove(child);
			object data = child.UnderlyingData;
			Window sourceWindow = Window.GetWindow(Owner);
			DXTabControl sourceTabControl = Owner;
			Window newWindow = CreateNewWindow(sourceWindow);
			DXTabControl newTabControl = CreateNewTabControl(data != null);
			newWindow.Content = newTabControl;
			var args = Owner.RaiseNewTabbedWindow(newWindow, newTabControl);
			if(args.NewWindow != newWindow) {
				newWindow.Content = null;
				CloseWindow(newWindow, true);
			}
			(args.NewTabControl.View as TabControlStretchView).Do(x => x.Insert(child, 0));
			args.NewWindow.Show();
		}
		DXTabbedWindow CreateNewWindow(Window sourceWindow) {
			DXTabbedWindow newWindow = sourceWindow != null ? DXTabbedWindow.CloneCore(sourceWindow) : new DXTabbedWindow();
			var mousePosition = DragControllerHelper.GetMousePositionOnScreen();
			newWindow.Left = mousePosition.X - 50;
			newWindow.Top = mousePosition.Y - 20;
			newWindow.DataContext = Owner.DataContext;
			newWindow.Style = NewWindowStyle;
			return newWindow;
		}
		DXTabControl CreateNewTabControl(bool initItemsSource) {
			DXTabControl newTabControl = ((ICloneable)Owner).Clone() as DXTabControl;
			if(initItemsSource)
				newTabControl.ItemsSource = new ObservableCollection<object>();
			newTabControl.Style = NewTabControlStyle;
			return newTabControl;
		}
		TabControlDragWidgetHelper dragWidgetHelper;
		internal TabControlDragWidgetHelper DragWidgetHelper { get { return dragWidgetHelper ?? (dragWidgetHelper = CreateDragWidgetHelper()); } }
		protected virtual TabControlDragWidgetHelper CreateDragWidgetHelper() {
			return new TabControlDragWidgetHelper();
		}
		#endregion DragDrop
	}
}
