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
using System.Collections.ObjectModel;
using System.Windows;
using DevExpress.Xpf.Bars;
using DevExpress.Xpf.Layout.Core;
using SWC = System.Windows.Controls;
namespace DevExpress.Xpf.Docking.Base {
	public delegate void DockItemActivatedEventHandler(
			object sender, DockItemActivatedEventArgs ea
		);
	public delegate void LayoutItemActivatedEventHandler(
			object sender, LayoutItemActivatedEventArgs ea
		);
	public delegate void MDIItemActivatedEventHandler(
			object sender, MDIItemActivatedEventArgs ea
		);
	public delegate void ShowingMenuEventHandler(
			object sender, ShowingMenuEventArgs e
		);
	public delegate void DockItemEventHandler(
			object sender, ItemEventArgs e
		);
	public delegate void DockItemCancelEventHandler(
			object sender, ItemCancelEventArgs e
		);
	public delegate void LayoutItemCancelEventHandler(
			object sender, ItemCancelEventArgs e
		);
	public delegate void MDIItemCancelEventHandler(
			object sender, ItemCancelEventArgs e
		);
	public delegate void DockItemDockingEventHandler(
			object sender, DockItemDockingEventArgs e
		 );
	public delegate void LayoutItemSelectionChangingEventHandler(
			object sender, LayoutItemSelectionChangingEventArgs e
		 );
	public delegate void LayoutItemSelectionChangedEventHandler(
			object sender, LayoutItemSelectionChangedEventArgs e
		 );
	public delegate void IsCustomizationChangedEventHandler(
			object sender, IsCustomizationChangedEventArgs e
		);
	public delegate void CustomizationFormVisibleChangedEventHandler(
			object sender, CustomizationFormVisibleChangedEventArgs e
		);
	public delegate void DockItemDraggingEventHandler(
			object sender, DockItemDraggingEventArgs e
		);
	public delegate void DockItemExpandedEventHandler(
			object sender, DockItemExpandedEventArgs e
		);
	public delegate void DockItemCollapsedEventHandler(
			object sender, DockItemCollapsedEventArgs e
		);
	public delegate void DockItemClosedEventHandler(
			object sender, DockItemClosedEventArgs e
		);
	public delegate void LayoutItemSizeChangedEventHandler(
			object sender, LayoutItemSizeChangedEventArgs e
		);
	public delegate void LayoutItemHiddenEventHandler(
			object sender, LayoutItemHiddenEventArgs e
		);
	public delegate void LayoutItemRestoredEventHandler(
			object sender, LayoutItemRestoredEventArgs e
		);
	public delegate void LayoutItemMovedEventHandler(
			object sender, LayoutItemMovedEventArgs e
		);
	public delegate void LayoutItemEndRenamingEventHandler(
			object sender, LayoutItemEndRenamingEventArgs e
		);
	public delegate void ShowInvisibleItemsChangedEventHandler(
			object sender, ShowInvisibleItemsChangedEventArgs e
		);
	public delegate void ItemIsVisibleChangedEventHandler(
			object sender, ItemIsVisibleChangedEventArgs e
		);
	public delegate void SelectedItemChangedEventHandler(
			object sender, SelectedItemChangedEventArgs e
		);
	public delegate void ShowingDockHintsEventHandler (
			object sender, ShowingDockHintsEventArgs e
	);
	public delegate void BeforeItemAddedEventHandler(
			object sender, BeforeItemAddedEventArgs e
		);
	public delegate void DockOperationStartingEventHandler(
			object sender, DockOperationStartingEventArgs e
		);
	public delegate void DockOperationCompletedEventHandler(
				object sender, DockOperationCompletedEventArgs e
			);
	public class DockItemActivatedEventArgs : RoutedEventArgs {
		public DockItemActivatedEventArgs(BaseLayoutItem item, BaseLayoutItem oldItem) :
			base(DockLayoutManager.DockItemActivatedEvent) {
			Item = item;
			OldItem = oldItem;
		}
		public BaseLayoutItem Item { get; private set; }
		public BaseLayoutItem OldItem { get; private set; }
	}
	public class LayoutItemActivatedEventArgs : RoutedEventArgs {
		public LayoutItemActivatedEventArgs(BaseLayoutItem item, BaseLayoutItem oldItem) :
			base(DockLayoutManager.LayoutItemActivatedEvent) {
			Item = item;
			OldItem = oldItem;
		}
		public BaseLayoutItem Item { get; private set; }
		public BaseLayoutItem OldItem { get; private set; }
	}
	public class MDIItemActivatedEventArgs : RoutedEventArgs {
		public MDIItemActivatedEventArgs(DocumentPanel item, DocumentPanel oldItem) :
			base(DockLayoutManager.MDIItemActivatedEvent) {
			Item = item;
			OldItem = oldItem;
		}
		public DocumentPanel Item { get; private set; }
		public DocumentPanel OldItem { get; private set; }
	}
	public class ShowingMenuEventArgs :
		ShowMenuEventArgs<BaseLayoutElementMenu> {
		public ShowingMenuEventArgs(BaseLayoutElementMenu menu)
			: base(menu) {
			RoutedEvent = DockLayoutManager.ShowingMenuEvent;
		}
	}
	public class ShowMenuEventArgs<T> : RoutedEventArgs where T : BaseLayoutElementMenu {
		public ShowMenuEventArgs(T menu) {
			Show = true;
			Menu = menu;
			Items = menu.GetItems();
		}
		public T Menu { get; private set; }
		public bool Show { get; set; }
		public ReadOnlyCollection<BarItem> Items { get; private set; }
		public BarManagerActionCollection ActionList { get { return Menu.Customizations; } }
		public IInputElement TargetElement { get { return Menu.PlacementTarget as IInputElement; } }
	}
	public class SelectedItemChangedEventArgs : RoutedEventArgs {
		public BaseLayoutItem Item { get; private set; }
		public BaseLayoutItem OldItem { get; private set; }
		public SelectedItemChangedEventArgs(BaseLayoutItem item, BaseLayoutItem oldItem) {
			RoutedEvent = LayoutGroup.SelectedItemChangedEvent;
			Item = item;
			OldItem = oldItem;
		}
	}
	public class ItemEventArgs : RoutedEventArgs {
		public BaseLayoutItem Item { get; private set; }
		public ItemEventArgs(BaseLayoutItem item) {
			Item = item;
		}
	}
	public class ItemCancelEventArgs : ItemEventArgs {
		public bool Cancel { get; set; }
		public ItemCancelEventArgs(BaseLayoutItem item)
			: this(false, item) {
		}
		public ItemCancelEventArgs(bool cancel, BaseLayoutItem item)
			: base(item) {
			Cancel = cancel;
		}
	}
	public class LayoutItemSelectionChangingEventArgs : ItemCancelEventArgs {
		public bool Selected { get; private set; }
		public LayoutItemSelectionChangingEventArgs(BaseLayoutItem item, bool selected)
			: base(false, item) {
			RoutedEvent = DockLayoutManager.LayoutItemSelectionChangingEvent;
			Selected = selected;
		}
	}
	public class LayoutItemSelectionChangedEventArgs : ItemEventArgs {
		public bool Selected { get; private set; }
		public LayoutItemSelectionChangedEventArgs(BaseLayoutItem item, bool selected)
			: base(item) {
			RoutedEvent = DockLayoutManager.LayoutItemSelectionChangedEvent;
			Selected = selected;
		}
	}
	public class DockItemDockingEventArgs : ItemCancelEventArgs {
		public Point DragPoint { get; private set; }
		public BaseLayoutItem DockTarget { get; private set; }
		public DockType DockType { get; private set; }
		public bool IsHiding { get; private set; }
		public DockItemDockingEventArgs(BaseLayoutItem item, Point pt, BaseLayoutItem target, DockType type, bool isHiding)
			: this(false, item, pt, target, type, isHiding) {
		}
		public DockItemDockingEventArgs(bool cancel, BaseLayoutItem item, Point pt, BaseLayoutItem target, DockType type, bool isHiding)
			: base(cancel, item) {
			DragPoint = pt;
			DockTarget = target;
			DockType = type;
			IsHiding = isHiding;
		}
	}
	public class IsCustomizationChangedEventArgs : RoutedEventArgs {
		public bool Value { get; private set; }
		public IsCustomizationChangedEventArgs(bool newValue) {
			Value = newValue;
			RoutedEvent = DockLayoutManager.IsCustomizationChangedEvent;
		}
	}
	public class CustomizationFormVisibleChangedEventArgs : RoutedEventArgs {
		public bool Value { get; private set; }
		public CustomizationFormVisibleChangedEventArgs(bool newValue) {
			Value = newValue;
			RoutedEvent = DockLayoutManager.CustomizationFormVisibleChangedEvent;
		}
	}
	public class DockItemDraggingEventArgs : ItemCancelEventArgs {
		public Point ScreenPoint { get; private set; }
		public DockItemDraggingEventArgs(Point screenPoint, BaseLayoutItem item)
			: base(item) {
			ScreenPoint = screenPoint;
			RoutedEvent = DockLayoutManager.DockItemDraggingEvent;
		}
	}
	public class DockItemExpandedEventArgs : ItemEventArgs {
		public DockItemExpandedEventArgs(BaseLayoutItem item)
			: base(item) {
			RoutedEvent = DockLayoutManager.DockItemExpandedEvent;
		}
	}
	public class DockItemClosedEventArgs : ItemEventArgs {
		public DockItemClosedEventArgs(BaseLayoutItem item, IEnumerable<BaseLayoutItem> affectedItems)
			: base(item) {
			RoutedEvent = DockLayoutManager.DockItemClosedEvent;
			AffectedItems = affectedItems;
		}
		public IEnumerable<BaseLayoutItem> AffectedItems { get; private set; }
	}
	public class DockItemCollapsedEventArgs : ItemEventArgs {
		public DockItemCollapsedEventArgs(BaseLayoutItem item)
			: base(item) {
			RoutedEvent = DockLayoutManager.DockItemCollapsedEvent;
		}
	}
	public class LayoutItemSizeChangedEventArgs : ItemEventArgs {
		public GridLength Value { get; private set; }
		public GridLength PrevValue { get; private set; }
		public bool HeightChanged { get; private set; }
		public bool WidthChanged { get; private set; }
		public LayoutItemSizeChangedEventArgs(BaseLayoutItem item, bool isWidth, GridLength value, GridLength prevValue)
			: base(item) {
			RoutedEvent = DockLayoutManager.LayoutItemSizeChangedEvent;
			Value = value;
			PrevValue = prevValue;
			WidthChanged = isWidth;
			HeightChanged = !isWidth;
		}
	}
	public class LayoutItemHiddenEventArgs : ItemEventArgs {
		public LayoutItemHiddenEventArgs(BaseLayoutItem item)
			: base(item) {
			RoutedEvent = DockLayoutManager.LayoutItemHiddenEvent;
		}
	}
	public class LayoutItemRestoredEventArgs : ItemEventArgs {
		public LayoutItemRestoredEventArgs(BaseLayoutItem item)
			: base(item) {
			RoutedEvent = DockLayoutManager.LayoutItemRestoredEvent;
		}
	}
	public class LayoutItemMovedEventArgs : ItemEventArgs {
		public BaseLayoutItem Target { get; private set; }
		public MoveType Type { get; private set; }
		public LayoutItemMovedEventArgs(BaseLayoutItem item, BaseLayoutItem target, MoveType type)
			: base(item) {
			Target = target;
			Type = type;
			RoutedEvent = DockLayoutManager.LayoutItemMovedEvent;
		}
	}
	public class LayoutItemEndRenamingEventArgs : ItemEventArgs {
		public string OldCaption { get; private set; }
		public string NewCaption { get; private set; }
		public bool Canceled { get; private set; }
		public LayoutItemEndRenamingEventArgs(BaseLayoutItem item, string oldCaption)
			: this(item, oldCaption, false) {
		}
		public LayoutItemEndRenamingEventArgs(BaseLayoutItem item, string oldCaption, bool canceled)
			: base(item) {
			RoutedEvent = DockLayoutManager.LayoutItemEndRenamingEvent;
			OldCaption = oldCaption;
			NewCaption = item != null ? item.Caption as string : null;
			Canceled = canceled;
		}
	}
	public class ShowInvisibleItemsChangedEventArgs : RoutedEventArgs {
		public bool? Value { get; private set; }
		public ShowInvisibleItemsChangedEventArgs(bool? newValue) {
			Value = newValue;
			RoutedEvent = DockLayoutManager.ShowInvisibleItemsChangedEvent;
		}
	}
	public class ItemIsVisibleChangedEventArgs : RoutedEventArgs {
		public bool IsVisible { get; private set; }
		public BaseLayoutItem Item { get; private set; }
		public ItemIsVisibleChangedEventArgs(BaseLayoutItem item, bool isVisible) {
			Item = item;
			IsVisible = isVisible;
			RoutedEvent = DockLayoutManager.ItemIsVisibleChangedEvent;
		}
	}
	public class ShowingDockHintsEventArgs : RoutedEventArgs {
		internal DockHintsConfiguration DockHintsConfiguration { get; set; }
		public BaseLayoutItem DraggingSource { get; private set; }
		public BaseLayoutItem DraggingTarget { get; private set; }
		internal ShowingDockHintsEventArgs(BaseLayoutItem draggingSource, BaseLayoutItem draggingTarget) {
			DraggingSource = draggingSource;
			DraggingTarget = draggingTarget;
			RoutedEvent = DockLayoutManager.ShowingDockHintsEvent;
		}
		public bool GetIsVisible(DockGuide hint) {
			return DockHintsConfiguration.GetIsVisible(hint);
		}
		public bool GetIsEnabled(DockHint hint) {
			return DockHintsConfiguration.GetIsEnabled(hint);
		}
		public void Disable(DockHint hint) {
			DockHintsConfiguration.Disable(hint);
		}
		[System.ComponentModel.Browsable(false), System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
		public void Hide(DockHint hint) {
			DockHintsConfiguration.Hide(hint);
		}
		public void Hide(DockGuide hint) {
			DockHintsConfiguration.Hide(hint);
		}
		public void HideAll() {
			DockHintsConfiguration.HideAll = true;
		}
		public void DisableAll() {
			DockHintsConfiguration.DisableAll = true;
		}
	}
	public class BeforeItemAddedEventArgs : RoutedEventArgs {
		public BaseLayoutItem Target { get; set; }
		public object Item { get; private set; }
		public bool Cancel { get; set; }
		public BeforeItemAddedEventArgs(object item, BaseLayoutItem potentialTarget) {
			RoutedEvent = DockLayoutManager.BeforeItemAddedEvent;
			Item = item;
			Target = potentialTarget;
		}
	}
	public class DockOperationStartingEventArgs : ItemCancelEventArgs {
		public DockOperation DockOperation { get; private set; }
		public BaseLayoutItem DockTarget { get; private set; }
		public DockOperationStartingEventArgs(BaseLayoutItem item, DockOperation dockOperation)
			: this(item, null, dockOperation) {
		}
		public DockOperationStartingEventArgs(BaseLayoutItem item, BaseLayoutItem dockTarget, DockOperation dockOperation)
			: base(item) {
			DockOperation = dockOperation;
			RoutedEvent = DockLayoutManager.DockOperationStartingEvent;
			DockTarget = dockTarget;
		}
	}
	public class DockOperationCompletedEventArgs : ItemEventArgs {
		public DockOperation DockOperation { get; private set; }
		public DockOperationCompletedEventArgs(BaseLayoutItem item, DockOperation dockOperation)
			: base(item) {
			DockOperation = dockOperation;
			RoutedEvent = DockLayoutManager.DockOperationCompletedEvent;
		}
	}
}
namespace DevExpress.Xpf.Docking {
	public delegate void DockTypeChangedEventHandler(
		object sender, DockTypeChangedEventArgs e
	);
	public class DockTypeChangedEventArgs : RoutedEventArgs {
		public SWC.Dock Value { get; private set; }
		public SWC.Dock PrevValue { get; private set; }
		public DockTypeChangedEventArgs(SWC.Dock value, SWC.Dock prev) {
			Value = value; PrevValue = prev;
		}
	}
	public delegate void BarMergeEventHandler(
		object sender, BarMergeEventArgs e
	);
	public class BarMergeEventArgs : RoutedEventArgs {
		public BarMergeEventArgs(BarManager manager, BarManager childManager) {
			BarManager = manager;
			ChildBarManager = childManager;
		}
		public BarManager BarManager { get; private set; }
		public BarManager ChildBarManager { get; private set; }
		public bool Cancel { get; set; }
	}
}
namespace DevExpress.Xpf.Docking.VisualElements {
	public delegate void HotItemChangedEventHandler(
		object sender, HotItemChangedEventArgs e
	);
	public class HotItemChangedEventArgs : RoutedEventArgs {
		public BaseLayoutItem Hot { get; private set; }
		public BaseLayoutItem PrevHot { get; private set; }
		public HotItemChangedEventArgs(BaseLayoutItem hot, BaseLayoutItem prevHot) {
			Hot = hot; PrevHot = prevHot;
		}
	}
	public delegate void PanelResizingEventHandler(
		object sender, PanelResizingEventArgs e
	);
	public class PanelResizingEventArgs : RoutedEventArgs {
		public double Size { get; private set; }
		public PanelResizingEventArgs(double size) {
			Size = size;
		}
	}
}
