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
using System.Windows.Media;
using System.Collections.ObjectModel;
using DevExpress.Xpf.Bars;
using System.Windows;
using System.Windows.Input;
using DevExpress.Data;
using DevExpress.Xpf.Utils.Themes;
using DevExpress.Xpf.Data;
using DevExpress.Data.Summary;
using DevExpress.Xpf.Core;
using System.ComponentModel;
using System.Windows.Controls;
using System.Windows.Documents;
using DevExpress.Xpf.Core.Native;
using System.Windows.Media.Imaging;
using DevExpress.Data.Filtering.Helpers;
using DevExpress.Xpf.Utils;
#if SL
using IInputElement = System.Windows.UIElement;
using PropertyChangedCallback = DevExpress.Xpf.Core.WPFCompatibility.SLPropertyChangedCallback;
using DependencyPropertyChangedEventArgs = DevExpress.Xpf.Core.WPFCompatibility.SLDependencyPropertyChangedEventArgs;
using RoutedEvent = DevExpress.Xpf.Core.WPFCompatibility.SLRoutedEvent;
using RoutedEventArgs = DevExpress.Xpf.Core.WPFCompatibility.SLRoutedEventArgs;
using DevExpress.WPFToSLUtils;
using DevExpress.Xpf.Editors.WPFCompatibility;
using DevExpress.Xpf.Core.WPFCompatibility;
#endif
namespace DevExpress.Xpf.Grid {
	public abstract class GridMenuInfo : GridMenuInfoBase {
		protected GridMenuInfo(DataControlPopupMenu menu)
			: base(menu) {
		}
		public new DataControlPopupMenu Menu { get { return (DataControlPopupMenu)base.Menu; } }
		public DataViewBase View { get { return Menu.View; } }
		public DataControlBase DataControl { get { return View.DataControl; } }
		protected internal BaseColumn BaseColumn { get; set; }
		public ColumnBase Column { get { return BaseColumn as ColumnBase; } }
		public abstract GridMenuType MenuType { get; }
		protected virtual BarButtonItem CreateBarButtonItem(string name, GridControlStringId id, bool beginGroup, ImageSource image, ICommand command, object commandParameter = null) {
			return CreateBarButtonItem(name, View.GetLocalizedString(id), beginGroup, image, command, commandParameter);
		}
		protected virtual BarButtonItem CreateBarButtonItem(BarItemLinkCollection links, string name, GridControlStringId id, bool beginGroup, ImageSource image, ICommand command, object commandParameter = null) {
			return CreateBarButtonItem(links, name, View.GetLocalizedString(id), beginGroup, image, command, commandParameter);
		}
		protected virtual BarSplitButtonItem CreateBarSplitButtonItem(BarItemLinkCollection links, string name, GridControlStringId id, bool beginGroup, ImageSource image) {
			return CreateBarSplitButtonItem(links, name, View.GetLocalizedString(id), beginGroup, image);
		}
		protected virtual BarCheckItem CreateBarCheckItem(string name, GridControlStringId id, bool? isChecked, bool beginGroup, ImageSource image, ICommand command) {
			return CreateBarCheckItem(name, View.GetLocalizedString(id), isChecked, beginGroup, image, command);
		}
		protected virtual BarSubItem CreateBarSubItem(string name, GridControlStringId id, bool beginGroup, ImageSource image, ICommand command) {
			return CreateBarSubItem(name, View.GetLocalizedString(id), beginGroup, image, command);
		}
		protected virtual BarSubItem CreateBarSubItem(BarItemLinkCollection links, string name, GridControlStringId id, bool beginGroup, ImageSource image, ICommand command) {
			return CreateBarSubItem(links, name, View.GetLocalizedString(id), beginGroup, image, command);
		}
	}
	public class GridCellMenuInfo : GridMenuInfo {
		public GridCellMenuInfo(DataControlPopupMenu menu)
			: base(menu) {
		}
		public RowData Row { get; private set; }
		public bool IsCellMenu { get; private set; }
		protected override void CreateItems() {
		}
		public override bool Initialize(IInputElement value) {
			Row = RowData.FindRowData(value as DependencyObject);
			CellContentPresenter presenter = value as CellContentPresenter;
			if(presenter != null) {
				BaseColumn = presenter.Column;
			}
			else {
				CellEditor cellEditor = value as CellEditor;
				if(cellEditor != null)
					BaseColumn = cellEditor.Column;
			}
			IsCellMenu = Column != null;
			return base.Initialize(value);
		}
		public override GridMenuType MenuType {
			get { return GridMenuType.RowCell; }
		}
		public override bool CanCreateItems {
			get { return View.IsRowCellMenuEnabled; }
		}
		public override BarManagerMenuController MenuController {
			get { return View.RowCellMenuController; }
		}
		protected override void ExecuteMenuController() {
			base.ExecuteMenuController();
			Menu.ExecuteOriginationViewMenuController((view) => { return view.RowCellMenuController; });
		}
	}
	public abstract class DataControlPopupMenu : GridPopupMenuBase {
		DataViewBase view;
		#region static
		public static readonly DependencyProperty GridMenuTypeProperty;
#if !SL
		protected internal static readonly RoutedEvent ManagerChangedEvent;
#endif
		static DataControlPopupMenu() {
			GridMenuTypeProperty = DependencyPropertyManager.RegisterAttached("GridMenuType", typeof(GridMenuType?),
				typeof(DataControlPopupMenu), new FrameworkPropertyMetadata(null));
#if !SL
			ManagerChangedEvent = EventManager.RegisterRoutedEvent("ManagerChanged", RoutingStrategy.Direct, typeof(RoutedPropertyChangedEventHandler<BarManager>), typeof(DataControlPopupMenu));
#endif
		}
		public static void SetGridMenuType(DependencyObject element, GridMenuType? value) {
			if(element == null)
				throw new ArgumentNullException("element");
			element.SetValue(GridMenuTypeProperty, value);
		}
		public static GridMenuType? GetGridMenuType(DependencyObject element) {
			if(element == null)
				throw new ArgumentNullException("element");
			return (GridMenuType?)element.GetValue(GridMenuTypeProperty);
		}
		#endregion
		public DataControlPopupMenu(DataViewBase view)
			: base(view.RootView) {
			this.view = view;
		}
		protected override bool ShouldClearItemsOnClose { get { return true; } }
		public new GridMenuInfo MenuInfo { get { return (GridMenuInfo)base.MenuInfo; } }
		public DataViewBase View { get { return view; } }		
		public GridMenuType? MenuType {
			get {
				if(MenuInfo != null)
					return MenuInfo.MenuType;
				return null;
			}
		}
		bool RequestUIUpdate() {
			View.LockEditorClose = true;
			try {
				return View.RequestUIUpdate();
			} finally {
				View.LockEditorClose = false;
			}
		}
		protected override bool RaiseShowMenu() {
			if(!RequestUIUpdate())
				return false;
			GridMenuEventArgs e = new GridMenuEventArgs(this) { RoutedEvent = DataViewBase.ShowGridMenuEvent };
			View.RaiseShowGridMenu(e);
			return !e.Handled;
		}
		protected override MenuInfoBase CreateMenuInfo(UIElement placementTarget) {
			if(placementTarget == null)
				throw new ArgumentException();
			GridMenuType? type = GetGridMenuType(placementTarget);
			if(type == null)
				return null;
			return CreateMenuInfoCore(type);
		}
		protected abstract MenuInfoBase CreateMenuInfoCore(GridMenuType? type);
#if DEBUGTEST
		public PopupMenuBarControl GetPopupMenuBarControl() {
			return ContentControl;
		}
#endif
		protected internal bool ExecuteOriginationViewMenuController(Func<DataViewBase, BarManagerMenuController> getMenuController) {
			DataViewBase view = View.OriginationView;
			if(view == null) return false;
			BarManagerMenuController originationViewController = getMenuController(view);
			if(originationViewController == null) return false;
			originationViewController.Menu = this;
			try {
				originationViewController.Execute();
			}
			finally {
				originationViewController.Menu = view.DataControlMenu;
			}
			return true;
		}
	}
	public delegate void GridMenuEventHandler(object sender, GridMenuEventArgs e);
	public class GridMenuEventArgs : RoutedEventArgs {
		DataControlPopupMenu menu;
		ReadOnlyCollection<BarItem> items;
		public GridMenuEventArgs(DataControlPopupMenu menu) {
			this.menu = menu;
			this.items = menu.GetItems();
		}
		protected DataControlPopupMenu GridMenu { get { return menu; } }
		public GridMenuType? MenuType { get { return GridMenu.MenuType; } }
		public ReadOnlyCollection<BarItem> Items { get { return items; } }
		public BarManagerActionCollection Customizations { get { return GridMenu.Customizations; } }
		public IInputElement TargetElement { get { return GridMenu.PlacementTarget; } }
		public GridMenuInfo MenuInfo { get { return GridMenu.MenuInfo; } }
		public new DataViewBase Source { get { return GridMenu.View; } }
	}
	public interface IGridSummaryItemsOwner : ISummaryItemsOwner {
		string FormatSummaryItemCaption(ISummaryItem summaryItem, string defaultCaption);
	}
	public abstract class GridSummaryHelper : IGridSummaryItemsOwner {
		GridSummaryItemsEditorController summaryController;
		internal GridSummaryItemsEditorController Controller {
			get {
				if(summaryController == null) {
					summaryController = CreateSummaryItemController();
					int i = 0;
					while(i < summaryController.Items.Count) {
						if(!((SummaryItemBase)summaryController.Items[i]).Visible) {
							summaryController.Items.Remove(summaryController.Items[i]);
						} else {
							i++;
						}
					}
				}
				return summaryController;
			}
		}
		protected virtual GridSummaryItemsEditorController CreateSummaryItemController() {
			return new GridSummaryItemsEditorController(this);
		}
		internal readonly DataViewBase view;
		protected abstract ISummaryItemOwner SummaryItems { get; }
		protected GridSummaryHelper(DataViewBase view) {
			this.view = view;
		}
		#region ISummaryItemsOwner Members
		ISummaryItem ISummaryItemsOwner.CreateItem(string fieldName, SummaryItemType summaryType) {
			return CreateItemCore(fieldName, summaryType);
		}
		protected virtual SummaryItemBase CreateItemCore(string fieldName, SummaryItemType summaryType) {
			SummaryItemBase item = view.DataControl.CreateSummaryItem();
			item.FieldName = fieldName;
			item.SummaryType = summaryType;
			return item;
		}
		string ISummaryItemsOwner.GetCaptionByFieldName(string fieldName) {
			return ColumnBase.GetDisplayName(view.DataControl.ColumnsCore[fieldName], fieldName);
		}
		List<string> ISummaryItemsOwner.GetFieldNames() {
			List<string> list = new List<string>();
			foreach(ColumnBase column in view.DataControl.ColumnsCore) {
				if(CanBeUsedInSummary(column))
					list.Add(column.FieldName);
			}
			return list;
		}
		protected virtual bool CanBeUsedInSummary(ColumnBase column) {
			return view.CanShowColumnInSummaryEditor(column);
		}
		List<ISummaryItem> ISummaryItemsOwner.GetItems() {
			List<ISummaryItem> items = new List<ISummaryItem>();
			foreach(SummaryItemBase item in SummaryItems) {
				if(CanUseSummaryItem(item))
					items.Add(item);
			}
			return items;
		}
		protected internal virtual bool CanUseSummaryItem(ISummaryItem item) {
			return !string.IsNullOrEmpty(item.FieldName);
		}
		Type ISummaryItemsOwner.GetTypeByFieldName(string fieldName) {
			return view.GetColumnType(view.DataControl.ColumnsCore[fieldName]);
		}
		void ISummaryItemsOwner.SetItems(List<ISummaryItem> items) {
			SetItemsCore(items);
		}
		string IGridSummaryItemsOwner.FormatSummaryItemCaption(ISummaryItem item, string defaultCaption) {
			return view.FormatSummaryItemCaptionInSummaryEditor(item, defaultCaption);
		}
		SummaryItemBase FindSummaryItem(SummaryItemBase item, ISummaryItemOwner list) {
			foreach(SummaryItemBase currentItem in list) {
				if(AreItemsEqual(currentItem, item))
					return currentItem;
			}
			return null;
		}
		protected virtual void SetItemsCore(List<ISummaryItem> items) {
			try {
				SummaryItems.BeginUpdate();
				foreach(SummaryItemBase item in SummaryItems) {
					if(CanUseSummaryItem(item))
						item.Visible = false;
				}
				foreach(SummaryItemBase item in items) {
					SummaryItemBase itemOfSummaryItems = FindSummaryItem(item, SummaryItems);
					if(itemOfSummaryItems == null) {
						SummaryItems.Add((SummaryItemBase)item);
					} else {
						SummaryItems.Remove(itemOfSummaryItems);
						SummaryItems.Add(itemOfSummaryItems);
						itemOfSummaryItems.Visible = true;
					}
				}
			} finally {
				SummaryItems.EndUpdate();
			}
		}
		protected virtual bool AreItemsEqual(SummaryItemBase item1, SummaryItemBase item2) {
			return item1.FieldName == item2.FieldName && item1.SummaryType == item2.SummaryType && item1.Alignment == item2.Alignment && item1.ShowInColumn == item2.ShowInColumn;
		}
		protected abstract string GetEditorTitle();
		#endregion
#if !SL
		public virtual FloatingContainer ShowSummaryEditor() {
			return ShowSummaryEditor(SummaryEditorType.Default);
		}
		public virtual FloatingContainer ShowSummaryEditor(SummaryEditorType summaryEditorType) {
			return 
#else
		public virtual void ShowSummaryEditor() {
			ShowSummaryEditor(SummaryEditorType.Default);
		}
		public virtual void ShowSummaryEditor(SummaryEditorType summaryEditorType) {
#endif
			view.SummaryEditorContainer = view.ShowDialogContent(new SummaryEditorControl(Controller, summaryEditorType), Size.Empty, new FloatingContainerParameters() { Title = GetEditorTitle(), AllowSizing = true, CloseOnEscape = true });
		}
	}
	public class GridTotalSummaryPanelHelper : GridTotalSummaryHelper {
		public GridTotalSummaryPanelHelper(DataViewBase view) 
			: base(view, null) {
		}
		public GridTotalSummaryPanelHelper(DataViewBase view, Func<ColumnBase> getColumn)
			: base(view, getColumn) {
		}
		protected override void InitializeSummaryItemCore(SummaryItemBase item) {
			item.Alignment = GridSummaryItemAlignment.Right;
		}
		protected internal override bool CanUseSummaryItem(ISummaryItem item) {
			return true;
		}
		protected override string GetEditorTitle() {
			return view.GetLocalizedString(GridControlStringId.TotalSummaryPanelEditorFormCaption);
		}
#if !SL
		public override FloatingContainer ShowSummaryEditor() {
			return ShowSummaryEditor(SummaryEditorType.TotalSummaryPanel);
		}
#else
		public override void ShowSummaryEditor() {
			ShowSummaryEditor(SummaryEditorType.TotalSummaryPanel);
		}
#endif
		protected override GridSummaryItemsEditorController CreateSummaryItemController() {
			return new GridSummaryPanelItemsEditorController(this);
		}
	}
	public class GridTotalSummaryHelper : GridSummaryHelper {
		readonly Func<ColumnBase> getColumn;
		protected ColumnBase Column { get { return getColumn(); } }
		public GridTotalSummaryHelper(DataViewBase view, Func<ColumnBase> getColumn)
			: base(view) {
			this.getColumn = getColumn;
		}
		protected override string GetEditorTitle() {
			return string.Format(view.GetLocalizedString(GridControlStringId.TotalSummaryEditorFormCaption), Column.HeaderCaption.ToString());
		}
		protected override ISummaryItemOwner SummaryItems {
			get { return view.DataControl.DataProviderBase.TotalSummaryCore; }
		}
		protected internal override bool CanUseSummaryItem(ISummaryItem item) {
			SummaryItemBase sumItem = item as SummaryItemBase;
			bool showInColumn = !string.IsNullOrEmpty(sumItem.ShowInColumn);
			if(item.FieldName != Column.FieldName && (showInColumn && sumItem.ShowInColumn == Column.FieldName)) return true;
			if(item.FieldName == Column.FieldName && (showInColumn && sumItem.ShowInColumn != Column.FieldName)) return false;
			return item.FieldName == Column.FieldName;
		}
		protected override SummaryItemBase CreateItemCore(string fieldName, SummaryItemType summaryType) {
			SummaryItemBase item = base.CreateItemCore(fieldName, summaryType);
			InitializeSummaryItemCore(item);
			return item;
		}
		protected virtual void InitializeSummaryItemCore(SummaryItemBase item) {
			item.ShowInColumn = Column.FieldName;
		}
	}
	public class GridGroupFooterSummaryHelper : GridTotalSummaryHelper {
		public GridGroupFooterSummaryHelper(DataViewBase view, Func<ColumnBase> getColumn)
			: base(view, getColumn) {
		}
		protected override string GetEditorTitle() {
			return string.Format(view.GetLocalizedString(GridControlStringId.TotalSummaryEditorFormCaption), Column.HeaderCaption.ToString());
		}
		protected override ISummaryItemOwner SummaryItems {
			get { return view.DataControl.DataProviderBase.GroupSummaryCore; }
		}
		protected override void InitializeSummaryItemCore(SummaryItemBase item) {
			base.InitializeSummaryItemCore(item);
			IGroupFooterSummaryItem summaryItem = (IGroupFooterSummaryItem)item;
			summaryItem.ShowInGroupColumnFooter = Column.FieldName;
		}
		protected internal override bool CanUseSummaryItem(ISummaryItem item) {
			IGroupFooterSummaryItem summaryItem = (IGroupFooterSummaryItem)item;
			return summaryItem.ShowInGroupColumnFooter == Column.FieldName;
		}
		protected override bool AreItemsEqual(SummaryItemBase item1, SummaryItemBase item2) {
			return base.AreItemsEqual(item1, item2) && ((IGroupFooterSummaryItem)item1).ShowInGroupColumnFooter == ((IGroupFooterSummaryItem)item2).ShowInGroupColumnFooter;
		}
	}
}
