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
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using DevExpress.Data.Utils;
using DevExpress.Mvvm;
using DevExpress.Mvvm.Native;
using DevExpress.Xpf.Bars;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Core.ConditionalFormatting.Native;
using DevExpress.Xpf.Editors;
using DevExpress.Xpf.Editors.Helpers;
using DevExpress.Xpf.Editors.Popups;
using DevExpress.Xpf.PivotGrid.Internal;
using DevExpress.XtraPivotGrid;
using DevExpress.XtraPivotGrid.Data;
using DevExpress.XtraPivotGrid.Localization;
using DependencyPropertyManager = System.Windows.DependencyProperty;
namespace DevExpress.Xpf.PivotGrid {
	public enum PivotGridMenuType { HeadersArea, Header, FieldValue, FilterPopup, None, Cell }
	public class PivotGridPopupMenu : GridPopupMenuBase {
		#region static stuff
		public static readonly DependencyProperty GridMenuTypeProperty;
		static PivotGridPopupMenu() {
			Type ownerType = typeof(PivotGridPopupMenu);
			GridMenuTypeProperty = DependencyPropertyManager.RegisterAttached("GridMenuType", typeof(PivotGridMenuType),
				ownerType, new FrameworkPropertyMetadata(PivotGridMenuType.None));
		}
		public static void SetGridMenuType(DependencyObject element, PivotGridMenuType value) {
			if(element == null)
				throw new ArgumentNullException("element");
			element.SetValue(GridMenuTypeProperty, value);
		}
		public static PivotGridMenuType GetGridMenuType(DependencyObject element) {
			if(element == null)
				throw new ArgumentNullException("element");
			return (PivotGridMenuType)element.GetValue(GridMenuTypeProperty);
		}
		#endregion
		public PivotGridPopupMenu(PivotGridControl owner)
			: base(owner) {
		}
		public new PivotGridMenuInfo MenuInfo { get { return (PivotGridMenuInfo)base.MenuInfo; } }
		public PivotGridMenuType MenuType {
			get {
				if(MenuInfo != null)
					return MenuInfo.MenuType;
				return PivotGridMenuType.None;
			}
		}
		public PivotGridControl PivotGrid { get { return (PivotGridControl)Owner; } }
#if SL
		protected override bool ShouldClearItemsOnClose { get { return true; } }
#endif
		protected override MenuInfoBase CreateMenuInfo(UIElement placementTarget) {
			PivotGridMenuType? menuType = GetGridMenuType(placementTarget);
			if(menuType != null) {
				switch(menuType) {
					case PivotGridMenuType.HeadersArea:
						return new PivotGridHeadersAreaMenuInfo(this);
					case PivotGridMenuType.Header:
						return new PivotGridHeaderMenuInfo(this);
					case PivotGridMenuType.FieldValue:
						return new PivotGridFieldValueMenuInfo(this);
					case PivotGridMenuType.FilterPopup:
						return new PivotGridFilterPopupMenuInfo(this);
					case PivotGridMenuType.Cell:
						return new PivotGridCellMenuInfo(this);
				}
			}
			if(CreateMenuInfoDelegate != null) return CreateMenuInfoDelegate(placementTarget);
			return null;
		}
		internal Func<UIElement, MenuInfoBase> CreateMenuInfoDelegate = null;
		protected override bool RaiseShowMenu() {
		   return PivotGrid.RaiseShowMenu();
		}
		protected override void OnIsOpenChanged(DependencyPropertyChangedEventArgs e) {
			base.OnIsOpenChanged(e);
			PivotGrid.UserAction = IsOpen ? UserAction.MenuOpen : UserAction.None;
		}
	}
	public static class DefaultMenuItemNames {
		public const string RefreshData = "ItemRefreshData", ShowFieldList = "ItemShowFieldList", 
			HideFieldList = "ItemHideFieldList", ShowPrefilter = "ItemShowPrefilter", HidePrefilter = "ItemHidePrefilter";
		public const string ShowUnboundExpressionEditor = "ShowUnboundExpressionEditor",
			HideField = "ItemHideField", FieldOrder = "ItemFieldOrder",
			FieldMoveToBeginning = "ItemFieldMoveToBeginning", FieldMoveLeft = "ItemFieldMoveLeft",
			FieldMoveRight = "ItemFieldMoveRight", FieldMoveToEnd = "ItemFieldMoveToEnd";
		public const string CollapseItem = "ItemCollapse", ExpandItem = "ItemExpand",
			CollapseAll = "ItemCollapseAll", ExpandAll = "ItemExpandAll", SortByItem = "ItemSortBySummary",
			RemoveAllSorting = "ItemRemoveAllSorting", InvertFilter = "ItemInvertFilter", KpiGraphic = "ItemKpiGraphic";
		public const string SortAscending = "ItemSortAZ", SortDescending = "ItemSortZA", ClearSorting = "ItemClearSorting";
	}
	public abstract class PivotGridMenuInfo : GridMenuInfoBase {
		protected PivotGridMenuInfo(PivotGridPopupMenu menu) 
			: base(menu) {
		}
		public abstract PivotGridMenuType MenuType { get; }
		public new PivotGridPopupMenu Menu { get { return (PivotGridPopupMenu)base.Menu; } }
		public PivotGridControl PivotGrid { get { return Menu.PivotGrid; } }
		protected PivotGridWpfData Data { get { return PivotGrid.Data; } }
		public override bool CanCreateItems { get { return true; } }
		protected virtual BarButtonItem CreateBarButtonItem(string name, PivotGridStringId id, bool beginGroup, 
				ImageSource image, ICommand command) {
			return CreateBarButtonItem(name, PivotGridLocalizer.GetString(id), beginGroup, image, command);
		}
		protected virtual BarButtonItem CreateBarButtonItem(string name, PivotGridStringId id, bool beginGroup, 
				ImageSource image, ICommand command, object commandParameter) {
			BarButtonItem item = CreateBarButtonItem(name, PivotGridLocalizer.GetString(id), beginGroup, image, command);
			item.CommandParameter = commandParameter;
			return item;
		}
		protected virtual BarCheckItem CreateBarCheckItem(string name, PivotGridStringId id, bool? isChecked, bool beginGroup, 
				ImageSource image, ICommand command) {
			return CreateBarCheckItem(name, PivotGridLocalizer.GetString(id), isChecked, beginGroup, image, command);
		}
		protected virtual BarCheckItem CreateBarCheckItem(string name, PivotGridStringId id, bool? isChecked, bool beginGroup, 
				ImageSource image, ICommand command, object commandParameter) {
			BarCheckItem item = CreateBarCheckItem(name, PivotGridLocalizer.GetString(id), isChecked, beginGroup, image, command);
			item.CommandParameter = commandParameter;
			return item;
		}
		protected virtual BarSubItem CreateBarSubItem(string name, PivotGridStringId id, bool beginGroup, ImageSource image, 
				ICommand command) {
			return CreateBarSubItem(name, PivotGridLocalizer.GetString(id), beginGroup, image, command);
		}
		public virtual PivotCellBaseEventArgs GetCellInfo() {
			return null;
		}
		public virtual PivotFieldValueEventArgs GetFieldValueInfo() {
			return null;
		}
		public virtual PivotFieldEventArgs GetFieldInfo() {
			return null;
		}
	}
	public class PivotGridHeadersAreaMenuInfo : PivotGridMenuInfo {
		public PivotGridHeadersAreaMenuInfo(PivotGridPopupMenu menu)
			: base(menu) {
		}
		public override PivotGridMenuType MenuType {
			get { return PivotGridMenuType.HeadersArea; }
		}
		public override bool CanCreateItems { get { return PivotGrid.IsHeaderAreaMenuEnabled; } }
		protected override void CreateItems() {
			CreateRefreshDataItem();
			CreateFieldListItem();
			CreatePrefilterButton();
		}
		protected void CreateRefreshDataItem() {
			CreateBarButtonItem(DefaultMenuItemNames.RefreshData, PivotGridStringId.PopupMenuRefreshData,
				true, ImageHelper.GetImage(DefaultMenuItemNames.RefreshData), PivotGrid.Commands.ReloadData);
		}
		protected internal void CreateFieldListItem() {
			if(PivotGrid.ExternalFieldListCount > 0 || !PivotGrid.AllowCustomizationForm) return;
			if(PivotGrid.IsFieldListVisible)
				CreateBarButtonItem(DefaultMenuItemNames.HideFieldList, PivotGridStringId.PopupMenuHideFieldList,
					true, ImageHelper.GetImage(DefaultMenuItemNames.HideFieldList), PivotGrid.Commands.HideFieldList);
			else
				CreateBarButtonItem(DefaultMenuItemNames.ShowFieldList, PivotGridStringId.PopupMenuShowFieldList,
					true, ImageHelper.GetImage(DefaultMenuItemNames.ShowFieldList), PivotGrid.Commands.ShowFieldList);
		}
		protected void CreatePrefilterButton() {
			if(!PivotGrid.AllowPrefilter || 
				!Data.IsCapabilitySupported(PivotDataSourceCaps.Prefilter)) return;
			if(PivotGrid.IsPrefilterVisible)
				CreateBarButtonItem(DefaultMenuItemNames.HidePrefilter, PivotGridStringId.PopupMenuHidePrefilter,
									true, ImageHelper.GetImage(DefaultMenuItemNames.HidePrefilter), PivotGrid.Commands.HidePrefilter);
			else
				CreateBarButtonItem(DefaultMenuItemNames.ShowPrefilter, PivotGridStringId.PopupMenuShowPrefilter,
									true, ImageHelper.GetImage(DefaultMenuItemNames.ShowPrefilter), PivotGrid.Commands.ShowPrefilter);							
		}
		public override BarManagerMenuController MenuController {
			get { return PivotGrid.HeadersAreaMenuController; }
		}
		protected void AssignCommandToItem(BarItem item, ICommand command, object parameter) {
			item.Command = command;
			item.CommandParameter = parameter;
			item.CommandTarget = TargetElement;
		}
	}
	public class PivotGridHeaderMenuInfo : PivotGridHeadersAreaMenuInfo {
		PivotGridField field;
		FieldListArea fieldListArea;
		public PivotGridHeaderMenuInfo(PivotGridPopupMenu menu)
			: base(menu) {
		}
		public PivotGridField Field { get { return field; } }
		protected FieldListArea FieldListArea { get { return fieldListArea; } }
		public override PivotGridMenuType MenuType {
			get { return PivotGridMenuType.Header; }
		}
		public override bool Initialize(IInputElement value) {
			DependencyObject dependencyObject = (DependencyObject)value;
			field = FieldHeader.GetField(dependencyObject);
			fieldListArea = FieldHeadersBase.GetFieldListArea(dependencyObject);
			return base.Initialize(value);
		}
		public override bool CanCreateItems { get { return PivotGrid.IsHeaderMenuEnabled; } }
		protected override void CreateItems() {
			CreateOlapSortingMenuItems();
			if(FieldListArea != FieldListArea.All)
				CreateRefreshDataItem();
			CreateExpressionEditorItem();
			if(FieldListArea != FieldListArea.All) {
				CreateHideMenuItem();
				CreateOrderMenuItem();
				CreateFieldListItem();
				CreatePrefilterButton();
				CreateKpiMenuItems();
			}
		}
		public override PivotFieldEventArgs GetFieldInfo() {
			if(Field == null)
				return null;
			return new PivotFieldEventArgs(null, Field);
		}
		string kpiImageTemplateString =
#if !SL
			"<dx:DXImage" +
#else
			"<Image" +
#endif
 " xmlns:dx=\"http://schemas.devexpress.com/winfx/2008/xaml/core\" " +
			"x:Name=\"PART_Glyph\" VerticalAlignment=\"Center\" HorizontalAlignment=\"Center\" MaxWidth=\"15\" MinWidth=\"15\" " +
		 "MaxHeight=\"15\" MinHeight=\"15\" Source=\"{Binding Path=Content.Source, RelativeSource={RelativeSource TemplatedParent}}\" Stretch=\"None\" " +
											  "RenderTransform=\"{Binding Path=Content.RenderTransform, RelativeSource={RelativeSource TemplatedParent}}\"/>";
		DataTemplate kpiImageTemplate;
		DataTemplate KpiImageTemplate {
			get {
				if(kpiImageTemplate == null)
					kpiImageTemplate = XamlHelper.GetTemplate(kpiImageTemplateString);
				return kpiImageTemplate;
			}
		}
		protected void CreateKpiMenuItems() {
			if(Field.Area != FieldArea.DataArea || !Field.KpiType.NeedGraphic())
				return;
			BarSubItem kpiItem = CreateBarSubItem(DefaultMenuItemNames.KpiGraphic, PivotGridStringId.PopupMenuKPIGraphic, false, null, null);
			DelegateCommand<PivotKpiGraphic> command = DelegateCommandFactory.Create<PivotKpiGraphic>(SetKpi, CanSetKpi, false);
			foreach(PivotKpiGraphic graphic in Helpers.GetEnumValues(typeof(PivotKpiGraphic))) {
				BarButtonItem item = Menu.CreateBarButtonItem("Item" + graphic.ToString(), GetMenuText(graphic), false, ImageHelper.GetImage(GetRealKpiName(graphic) + ".1", true), kpiItem.ItemLinks);
				item.CommandParameter = graphic;
				item.Command = command;
				item.GlyphTemplate = KpiImageTemplate;
			}
		}
		string GetRealKpiName(PivotKpiGraphic graphic) {
			if(graphic == PivotKpiGraphic.ServerDefined)
				return Data.GetKPIGraphic(Field.InternalField).ToString();
			else
				return graphic.ToString();
		}
		string GetMenuText(PivotKpiGraphic graphic) {
			if(graphic == PivotKpiGraphic.ServerDefined)
				return GetStringFromKpiGraphic(graphic) + " (" + GetStringFromKpiGraphic(Data.GetKPIGraphic(Field.InternalField).ToKpiGraphic()) + ")";
			else
				return GetStringFromKpiGraphic(graphic);
		}
		private static string GetStringFromKpiGraphic(PivotKpiGraphic graphic) {
			return PivotGridLocalizer.GetString(graphic.GetStringId());
		}
		public void SetKpi(PivotKpiGraphic graphic) {
			Field.KpiGraphic = graphic;
		}
		public bool CanSetKpi(PivotKpiGraphic graphic) {
			return Field.KpiGraphic != graphic;
		}
		protected void CreateOlapSortingMenuItems() {
			if(!Field.IsOlapSortModeNone || !Field.InternalField.CanSortCore) return;
			bool isAscendingChecked = Field.IsOlapSorted && Field.SortOrder == FieldSortOrder.Ascending,
				isDescendingChecked = Field.IsOlapSorted && Field.SortOrder == FieldSortOrder.Descending;
			CreateBarCheckItem(DefaultMenuItemNames.SortAscending, PivotGridStringId.PopupMenuSortAscending,
				isAscendingChecked, true, null, PivotGrid.Commands.SortAscending, Field);
			CreateBarCheckItem(DefaultMenuItemNames.SortDescending, PivotGridStringId.PopupMenuSortDescending,
				isDescendingChecked, false, null, PivotGrid.Commands.SortDescending, Field);
			CreateBarButtonItem(DefaultMenuItemNames.ClearSorting, PivotGridStringId.PopupMenuClearSorting,
				false, null, PivotGrid.Commands.ClearSorting, Field);
		}
		protected void CreateExpressionEditorItem() {
			if(!Field.CanShowUnboundExpressionMenu) return;
			CreateBarButtonItem(DefaultMenuItemNames.ShowUnboundExpressionEditor,
					PivotGridStringId.PopupMenuShowExpression, false, null, PivotGrid.Commands.ShowUnboundExpressionEditor, Field);
		}
		protected void CreateHideMenuItem() {
			CreateBarButtonItem(DefaultMenuItemNames.HideField, PivotGridStringId.PopupMenuHideField,
					true, null, PivotGrid.Commands.HideField, Field);
		}
		protected void CreateOrderMenuItem() {
			BarSubItem orderItem = CreateBarSubItem(DefaultMenuItemNames.FieldOrder, PivotGridStringId.PopupMenuFieldOrder,
				false, null, null);
			BarButtonItem moveToBeginning = Menu.CreateBarButtonItem(DefaultMenuItemNames.FieldMoveToBeginning,
				PivotGridLocalizer.GetString(PivotGridStringId.PopupMenuMovetoBeginning), false, null, orderItem.ItemLinks);
			AssignCommandToItem(moveToBeginning, DelegateCommandFactory.Create(MoveToBeginning, CanMoveBeginning, false), Field);
			BarButtonItem moveLeft = Menu.CreateBarButtonItem(DefaultMenuItemNames.FieldMoveLeft,
				PivotGridLocalizer.GetString(PivotGridStringId.PopupMenuMovetoLeft), false, null, orderItem.ItemLinks);
			AssignCommandToItem(moveLeft, DelegateCommandFactory.Create(MoveLeft, CanMoveLeft, false), Field);
			BarButtonItem moveRight = Menu.CreateBarButtonItem(DefaultMenuItemNames.FieldMoveRight,
				PivotGridLocalizer.GetString(PivotGridStringId.PopupMenuMovetoRight), false, null, orderItem.ItemLinks);
			AssignCommandToItem(moveRight, DelegateCommandFactory.Create(MoveRight, CanMoveRight, false), Field);
			BarButtonItem moveToEnd = Menu.CreateBarButtonItem(DefaultMenuItemNames.FieldMoveToEnd,
				PivotGridLocalizer.GetString(PivotGridStringId.PopupMenuMovetoEnd), false, null, orderItem.ItemLinks);
			AssignCommandToItem(moveToEnd, DelegateCommandFactory.Create(MoveToEnd, CanMoveEnd, false), Field);
		}
		public override BarManagerMenuController MenuController {
			get { return PivotGrid.HeaderMenuController; }
		}
		void MoveToBeginning() { Data.SetFieldAreaPositionAsync(Field, Field.Area.ToPivotArea(), 0, false); }
		bool CanMoveBeginning() { return CanMoveLeftCore() && Data.OnFieldAreaChanging(Field.InternalField, Field.Area.ToPivotArea(), 0); }
		void MoveLeft() {
			if(Field.AreaIndex == 0) return;
			int newAreaIndex = IsFieldAfterDataField() ? Field.AreaIndex : Field.AreaIndex - 1;
			List<PivotGridField> fields = Field.Data.GetFieldsByArea(Field.Area, true);
			fields.Sort(PivotGridDropTarget.PivotGridFieldComparisonByAreaIndex);
			while(newAreaIndex > 0 && fields[newAreaIndex].InternalField.InnerGroupIndex > 0)
				newAreaIndex--;
			Data.SetFieldAreaPositionAsync(Field, Field.Area.ToPivotArea(), newAreaIndex, false);
		}
		bool CanMoveLeft() { return CanMoveLeftCore() && Data.OnFieldAreaChanging(Field.InternalField, Field.Area.ToPivotArea(), IsFieldAfterDataField() ? Field.AreaIndex : Field.AreaIndex - 1); }
		bool CanMoveLeftCore() {
			return (Field.AreaIndex > 0 && (Field.Group == null || Field.Group.IndexOf(Field) < 1) || Field.Area == Data.DataField.Area && Field.AreaIndex == Data.DataField.AreaIndex && !Field.InternalField.IsDataField);
		}
		void MoveRight() {
			int newAreaIndex = IsFieldAfterDataField() ? Field.AreaIndex + 2 : Field.AreaIndex + 1;
			List<PivotGridField> fields = Field.Data.GetFieldsByArea(Field.Area, true);
			fields.Sort(PivotGridDropTarget.PivotGridFieldComparisonByAreaIndex);
			while(newAreaIndex < fields.Count - 1 && fields[newAreaIndex].InternalField.InnerGroupIndex > 0)
				newAreaIndex++;
			Data.SetFieldAreaPositionAsync(Field, Field.Area.ToPivotArea(), newAreaIndex, false);
		}
		bool CanMoveRight() { return CanMoveRightCore() && Data.OnFieldAreaChanging(Field.InternalField, Field.Area.ToPivotArea(), IsFieldAfterDataField() ? Field.AreaIndex + 2 : Field.AreaIndex + 1); }
		bool CanMoveRightCore() {
			int fieldCount = GetFieldCount();
			return Field.AreaIndex < fieldCount - 1 && (Field.Group == null || Field.Group.IndexOf(Field) < 1);
		}
		int GetFieldCount() {
			int fieldCount = Data.GetFieldCountByArea(Field.Area);
			if(Data.DataField.Visible && Data.DataField.Area == Field.Area && Field.AreaIndex < Data.DataField.AreaIndex || Field.InternalField.IsDataField)
				fieldCount++;
			foreach(PivotGridField field in Data.Fields) {
				if(field.Area == Field.Area && field.Group != null && field.Visible && field.InternalField.InnerGroupIndex > 0 && field.Group.VisibleCount > field.InternalField.InnerGroupIndex)
					fieldCount--;
			}
			return fieldCount;
		}
		void MoveToEnd() {
			Data.SetFieldAreaPositionAsync(Field, Field.Area.ToPivotArea(), int.MaxValue, false);
		}
		bool CanMoveEnd() { return CanMoveRightCore() && Data.OnFieldAreaChanging(Field.InternalField, Field.Area.ToPivotArea(), int.MaxValue); }
		bool IsFieldAfterDataField() {
			return !Field.InternalField.IsDataField && Field.Area == Data.DataField.Area && Data.DataField.Visible && Data.DataField.AreaIndex <= Field.AreaIndex;
		}
	}
	public class PivotGridCellMenuInfo : PivotGridMenuInfo, IConditionalFormattingDialogBuilder {
		CellsAreaItem cellItem;
		public PivotGridCellMenuInfo(PivotGridPopupMenu menu)
			: base(menu) {
		}
		public CellsAreaItem CellItem {
			get { return cellItem; }
			internal set { cellItem = value; }
		}
		public override PivotGridMenuType MenuType {
			get { return PivotGridMenuType.Cell; }
		}
		public override BarManagerMenuController MenuController {
			get { return PivotGrid.CellMenuController; }
		}
		public override bool Initialize(IInputElement value) {
			this.cellItem = ScrollableAreaCell.GetValueItem((DependencyObject)value) as CellsAreaItem;
			return base.Initialize(value);
		}
		public override bool CanCreateItems { get { return PivotGrid.IsCellMenuEnabled  ; } }
		protected override void CreateItems() {
			if(!PivotGrid.AllowConditionalFormattingMenu || CellItem == null || CellItem.Item == null)
				return;
			if(CellItem.Item.DataField == null || CellItem.Item.DataField.IsDataField || !HasName(CellItem.Item.DataField))
				return;
			if(!HasName(CellItem.Item.ColumnField))
				return;
			if(!HasName(CellItem.Item.RowField))
				return;
			FormatConditionCommandParameters sett = new FormatConditionCommandParameters() { Measure = CellItem.Field, Row = GetField(CellItem.Item.RowField), Column = GetField(CellItem.Item.ColumnField) };
			PivotConditionalFormattingDialogDirector director = new PivotConditionalFormattingDialogDirector(new DataControlDialogContext(CellItem.Field, sett), new FormatConditionsCommands(PivotGrid), this, PivotGrid);
			director.AllowConditionalFormattingManager = PivotGrid.AllowConditionalFormattingManager;
			director.IsServerMode = false;
			director.CreateMenuItems(sett);
		}
		bool HasName(PivotFieldItemBase item) {
			if(item == null)
				return true;
			PivotGridField field = PivotGrid.Data.GetField(item);
			return field != null && !string.IsNullOrEmpty(field.Name);
		}
		PivotGridField GetField(PivotFieldItemBase field) {
			return field == null ? null : ((PivotGridInternalField)PivotGrid.Data.GetField(field)).Wrapper;
		}
		public override PivotCellBaseEventArgs GetCellInfo() {
			if(CellItem == null || CellItem.Item == null)
				return null;
			return new PivotCellBaseEventArgs(null, CellItem.Item);
		}
		BarButtonItem IConditionalFormattingDialogBuilder.CreateBarButtonItem(BarItemLinkCollection links, string name, string content, bool beginGroup, ImageSource image, ICommand command, object commandParameter) {
			return CreateBarButtonItem(links, name, content, beginGroup, image, command, commandParameter);
		}
		BarSplitButtonItem IConditionalFormattingDialogBuilder.CreateBarSplitButtonItem(BarItemLinkCollection links, string name, string content, bool beginGroup, ImageSource image) {
			return CreateBarSplitButtonItem(links, name, content, beginGroup, image);
		}
		BarSubItem IConditionalFormattingDialogBuilder.CreateBarSubItem(BarItemLinkCollection links, string name, string content, bool beginGroup, ImageSource image, ICommand command) {
			return CreateBarSubItem(links, name, content, beginGroup, image, command);
		}
		BarSubItem IConditionalFormattingDialogBuilder.CreateBarSubItem(string name, string content, bool beginGroup, ImageSource image, ICommand command) {
			return CreateBarSubItem(name, content, beginGroup, image, command);
		}
	}
	public class PivotGridFieldValueMenuInfo : PivotGridMenuInfo {
		FieldValueItem valueItem;
		public PivotGridFieldValueMenuInfo(PivotGridPopupMenu menu)
			: base(menu) {
		}
		public FieldValueItem ValueItem { 
			get { return valueItem; }
			internal set { valueItem = value; }
		} 
		public override PivotGridMenuType MenuType {
			get { return PivotGridMenuType.FieldValue; }
		}
		public override BarManagerMenuController MenuController {
			get { return PivotGrid.FieldValueMenuController; }
		}
		public override bool Initialize(IInputElement value) {
			this.valueItem = ScrollableAreaCell.GetValueItem((DependencyObject)value) as FieldValueItem;
			return base.Initialize(value);
		}
		public override bool CanCreateItems { get { return PivotGrid.IsFieldValueMenuEnabled; } }
		protected override void CreateItems() {
			if(ValueItem == null)
				return;
			if(ValueItem.CanExpand) {
				CreateCollapseExpandItem();
				CreateExpandAllItem();
				CreateCollapseAllItem();
			}
			if(ValueItem.CanShowSortBySummary) {
				CreateSortBySummaryMenuItems();
			}
		}
		public override PivotFieldValueEventArgs GetFieldValueInfo() {
			if(ValueItem == null || ValueItem.Item == null)
				return null;
			return new PivotFieldValueEventArgs(null, ValueItem.Item);
		}
		protected void CreateCollapseExpandItem() {
			BarButtonItem item;
			if(ValueItem.IsExpanded)
				item = CreateBarButtonItem(DefaultMenuItemNames.CollapseItem, PivotGridStringId.PopupMenuCollapse,
					true, null, PivotGrid.Commands.ChangeFieldValueExpanded);
			else
				item = CreateBarButtonItem(DefaultMenuItemNames.ExpandItem, PivotGridStringId.PopupMenuExpand,
					true, null, PivotGrid.Commands.ChangeFieldValueExpanded);
			item.CommandParameter = ValueItem.Item;
		}
		protected void CreateExpandAllItem() {
			BarButtonItem item = CreateBarButtonItem(DefaultMenuItemNames.ExpandAll, PivotGridStringId.PopupMenuExpandAll,
					true, null, PivotGrid.Commands.ExpandField);
			item.CommandParameter = ValueItem.Field;
		}
		protected void CreateCollapseAllItem() {
			BarButtonItem item = CreateBarButtonItem(DefaultMenuItemNames.CollapseAll, PivotGridStringId.PopupMenuCollapseAll,
					false, null, PivotGrid.Commands.CollapseField);
			item.CommandParameter = ValueItem.Field;
		}
		protected void CreateSortBySummaryMenuItems() {
			List<PivotGridField> crossAreaFields = ValueItem.GetCrossAreaFields();
			bool showRemoveAllSortingItem = false,
				hideDataFields = ValueItem.IsDataLocatedInThisArea || Data.DataFieldCount == 1;
			string captionTemplate = PivotGridLocalizer.GetString(ValueItem.IsColumn ? PivotGridStringId.PopupMenuSortFieldByColumn : PivotGridStringId.PopupMenuSortFieldByRow);
			List<PivotGridField> dataFields = hideDataFields ? null : Data.GetFieldsByArea(FieldArea.DataArea, false);
			for(int i = 0; i < crossAreaFields.Count; i++) {
				PivotGridField field = crossAreaFields[i];
				if(!field.CanSortBySummary) 
					continue;
				if(hideDataFields) 
					showRemoveAllSortingItem |= CreateSortByMenuItem(captionTemplate, field, ValueItem.DataField, i == 0);
				else 
					showRemoveAllSortingItem |= CreateSortByWithDataMenuItems(captionTemplate, field, dataFields, i == 0);				
			}
			if(showRemoveAllSortingItem) {
				BarButtonItem item = CreateBarButtonItem(DefaultMenuItemNames.RemoveAllSorting, PivotGridStringId.PopupMenuRemoveAllSortByColumn,
					true, null, null);
				item.ItemClick += OnRemoveSortBySummaryClick;
			}
		}		
		protected bool CreateSortByMenuItem(string captionTemplate, PivotGridField field, PivotGridField dataField, bool beginGroup) {
			string caption = string.Format(captionTemplate, field.DisplayText);
			return CreateSortByMenuItemCore(field, dataField, beginGroup, caption);
		}
		protected bool CreateSortByWithDataMenuItems(string captionTemplate, PivotGridField field, List<PivotGridField> dataFields, bool beginGroup) {
			bool result = false;
			for(int i = 0; i < dataFields.Count; i++) {
				PivotGridField dataField = dataFields[i];
				string caption = string.Format(captionTemplate, field.DisplayText + " - " + dataField.DisplayText);
				result |= CreateSortByMenuItemCore(field, dataField, beginGroup && i == 0, caption);
			}
			return result;
		}
		bool CreateSortByMenuItemCore(PivotGridField field, PivotGridField dataField, bool beginGroup, string caption) {
			bool isChecked = IsFieldSortedBySummary(field, dataField);
			string name = DefaultMenuItemNames.SortByItem + ((uint)caption.GetHashCode()).ToString();			
			BarCheckItem item = CreateBarCheckItem(name, caption, isChecked, beginGroup, null, null);
			item.Tag = new SortByMenuTag(field, dataField);
			item.CheckedChanged += OnSortByItemChecked;
			return isChecked;
		}		
		protected virtual void OnSortByItemChecked(object sender, ItemClickEventArgs e) {
			BarCheckItem item = (BarCheckItem)e.Item;
			SortByMenuTag tag = (SortByMenuTag)item.Tag;
			SetFieldSortBySummaryAsync(tag.Field, tag.DataField, item.IsChecked.HasValue ? item.IsChecked.Value : false);
		}
		protected internal void SetFieldSortBySummaryAsync(PivotGridField field, PivotGridField dataField, bool sort) {
			if(!field.CanSortBySummary) return;
			Data.BeginUpdate();
			try {
				if(sort) {
					field.SortByField = dataField;
					field.SortByCustomTotalSummaryType = ValueItem.CustomTotalSummaryType;
					field.SortByConditions.Clear();
					field.SortByConditions.AddRange(GetFieldSortConditions());
					if(PivotGrid != null)
						field.SortOrder = PivotGrid.SortBySummaryDefaultOrder.ToFieldSortOrder(field.SortOrder);
				} else {
					field.ResetSortBySummary();
				}
			} finally {
				Data.EndUpdateAsync(false);
			}
		}
		protected bool IsFieldSortedBySummary(PivotGridField field, PivotGridField dataField) {
			return Data.VisualItems.IsFieldSortedBySummary(ValueItem.IsColumn, field,
				dataField, ValueItem.Item.Index);
		}
		protected List<PivotGridFieldSortCondition> GetFieldSortConditions() {
			return Data.VisualItems.GetFieldSortConditions(ValueItem.IsColumn, ValueItem.Item.Index);
		}
		protected virtual void OnRemoveSortBySummaryClick(object sender, ItemClickEventArgs e) {
			RemoveSortBySummaryAsync();
		}
		protected void RemoveSortBySummaryAsync() {
			List<PivotGridField> crossAreaFields = Data.GetFieldsByArea(ValueItem.Item.CrossArea.ToFieldArea(), false);
			Data.BeginUpdate();
			try {
				foreach(PivotGridField field in crossAreaFields) {
					if(field.CanSortBySummary && IsFieldSortedBySummary(field, null)) {
						field.ResetSortBySummary();
					}
				}
			} finally {
				Data.EndUpdateAsync(false);
			}
		}
		public class SortByMenuTag {
			WeakReference field, dataField;
			public SortByMenuTag(PivotGridField field, PivotGridField dataField) {
				this.field = new WeakReference(field);
				this.dataField = new WeakReference(dataField);
			}
			public PivotGridField Field { get { return (PivotGridField)field.Target; } }
			public PivotGridField DataField { get { return (PivotGridField)dataField.Target; } }
		}
	}
	public class PivotGridFilterPopupMenuInfo: PivotGridMenuInfo {
		public PivotGridFilterPopupMenuInfo(PivotGridPopupMenu menu)
			: base(menu) {
		}
		FilterCheckedTreeView FilterCheckedTreeView { get; set; }
		FilterCheckedTreeViewItem FilterCheckedTreeViewItem { get; set; }
		PopupListBox PopupListBox { get; set; }
		public override PivotGridMenuType MenuType {
			get { return PivotGridMenuType.FilterPopup; }
		}
		public override BarManagerMenuController MenuController {
			get { return PivotGrid.FilterPopupMenuController; }
		}
		public override bool Initialize(IInputElement value) {
			FilterCheckedTreeView tv = value as FilterCheckedTreeView;
			if(tv != null) {
				this.FilterCheckedTreeView = tv;
			} else {
				FilterCheckedTreeViewItem tvi = value as FilterCheckedTreeViewItem;
				if(tvi != null) {
					this.FilterCheckedTreeViewItem = tvi;
				} else {
					PopupListBox plb = value as PopupListBox;
					if(plb != null)
						this.PopupListBox = plb;
					else
						throw new Exception();
				}
			}
			return base.Initialize(value);
		}
		public override bool CanCreateItems { get { return PivotGrid.IsFilterPopupMenuEnabled; } }
		protected override void CreateItems() {
			if(FilterCheckedTreeViewItem != null)
				FilterCheckedTreeView = FilterCheckedTreeViewItem.ParentTreeView;
			BarButtonItem invertFilterItem = CreateBarButtonItem(DefaultMenuItemNames.InvertFilter, PivotGridStringId.FilterInvert, false, null, PivotGrid.Commands.ChangeFieldValueExpanded);
			if(FilterCheckedTreeView != null) {
				invertFilterItem.Command = DelegateCommandFactory.Create<FilterCheckedTreeView>(InvertFilter, false);
				invertFilterItem.CommandParameter = FilterCheckedTreeView;
			} else {
				invertFilterItem.Command = DelegateCommandFactory.Create<PopupListBox>(InvertFilter, false);
				invertFilterItem.CommandParameter = PopupListBox;
			}
			if(FilterCheckedTreeViewItem != null && !FilterCheckedTreeViewItem.IsLastLevel) {
				BarButtonItem collapseAllItem = CreateBarButtonItem(DefaultMenuItemNames.CollapseAll, PivotGridStringId.PopupMenuCollapseAll, false, null, PivotGrid.Commands.ChangeFieldValueExpanded);
				collapseAllItem.Command = DelegateCommandFactory.Create<FilterCheckedTreeViewItem>(CollapseAll, false);
				collapseAllItem.CommandParameter = FilterCheckedTreeViewItem;
				BarButtonItem expandAllItem = CreateBarButtonItem(DefaultMenuItemNames.ExpandAll, PivotGridStringId.PopupMenuExpandAll, false, null, PivotGrid.Commands.ChangeFieldValueExpanded);
				expandAllItem.Command = DelegateCommandFactory.Create<FilterCheckedTreeViewItem>(ExpandAll, false);
				expandAllItem.CommandParameter = FilterCheckedTreeViewItem;
			}
		}
		public void InvertFilter(PopupListBox popupListBox) {
			ComboBoxEdit editor = popupListBox.OwnerEdit;
			editor.BeginInit();
			editor.SelectedItems.Clear();
			for(int i = 1; i < popupListBox.Items.Count; i++) {
				PivotGridFilterItem item = (PivotGridFilterItem)popupListBox.Items[i];
				item.IsChecked = !item.IsChecked;
				if(item.IsChecked == true)
					editor.SelectedItems.Add(item);
			}
			editor.EndInit();
			PopupBaseEditHelper.GetOkButton(editor).IsEnabled = ((FilterPopupEdit)editor).FilterItems.CanAccept;
		}
		public void InvertFilter(FilterCheckedTreeView filterCheckedTreeView) {
			filterCheckedTreeView.InvertFilter();
		}
		public void CollapseAll(FilterCheckedTreeViewItem item) {
			FilterCheckedTreeView.CollapseAll(item.FilterItem.Level);
		}
		public void ExpandAll(FilterCheckedTreeViewItem item) {
			FilterCheckedTreeView.ExpandAll(item.FilterItem.Level);
		}
	}
}
