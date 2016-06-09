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
using System.Drawing;
using DevExpress.Utils.Commands;
using DevExpress.Utils.Drawing;
using DevExpress.XtraBars;
using DevExpress.XtraBars.Commands;
using DevExpress.XtraBars.Commands.Internal;
using DevExpress.XtraBars.Commands.Ribbon;
using DevExpress.XtraBars.Ribbon;
using DevExpress.XtraBars.Ribbon.Gallery;
using DevExpress.XtraBars.Ribbon.ViewInfo;
using DevExpress.XtraSpreadsheet.Commands;
using DevExpress.XtraSpreadsheet.Localization;
using DevExpress.XtraSpreadsheet.Model;
namespace DevExpress.XtraSpreadsheet.UI {
	#region SpreadsheetPivotTableDesignLayoutItemBuilder
	public class SpreadsheetPivotTableDesignLayoutItemBuilder : CommandBasedBarItemBuilder {
		public override void PopulateItems(List<BarItem> items, BarCreationContextBase creationContext) {
			SpreadsheetCommandBarSubItem subtotalsSubItem = SpreadsheetCommandBarSubItem.Create(SpreadsheetCommandId.PivotTableLayoutSubtotalsGroup);
			subtotalsSubItem.AddBarItem(SpreadsheetCommandBarButtonItem.Create(SpreadsheetCommandId.PivotTableDoNotShowSubtotals, RibbonItemStyles.Large));
			subtotalsSubItem.AddBarItem(SpreadsheetCommandBarButtonItem.Create(SpreadsheetCommandId.PivotTableShowAllSubtotalsAtBottom, RibbonItemStyles.Large));
			subtotalsSubItem.AddBarItem(SpreadsheetCommandBarButtonItem.Create(SpreadsheetCommandId.PivotTableShowAllSubtotalsAtTop, RibbonItemStyles.Large));
			SpreadsheetCommandBarSubItem grandTotalsSubItem = SpreadsheetCommandBarSubItem.Create(SpreadsheetCommandId.PivotTableLayoutGrandTotalsGroup);
			grandTotalsSubItem.AddBarItem(SpreadsheetCommandBarButtonItem.Create(SpreadsheetCommandId.PivotTableGrandTotalsOffRowsColumns, RibbonItemStyles.Large));
			grandTotalsSubItem.AddBarItem(SpreadsheetCommandBarButtonItem.Create(SpreadsheetCommandId.PivotTableGrandTotalsOnRowsColumns, RibbonItemStyles.Large));
			grandTotalsSubItem.AddBarItem(SpreadsheetCommandBarButtonItem.Create(SpreadsheetCommandId.PivotTableGrandTotalsOnRowsOnly, RibbonItemStyles.Large));
			grandTotalsSubItem.AddBarItem(SpreadsheetCommandBarButtonItem.Create(SpreadsheetCommandId.PivotTableGrandTotalsOnColumnsOnly, RibbonItemStyles.Large));
			SpreadsheetCommandBarSubItem reportLayoutSubItem = SpreadsheetCommandBarSubItem.Create(SpreadsheetCommandId.PivotTableLayoutReportLayoutGroup);
			reportLayoutSubItem.AddBarItem(SpreadsheetCommandBarButtonItem.Create(SpreadsheetCommandId.PivotTableShowCompactForm, RibbonItemStyles.Large));
			reportLayoutSubItem.AddBarItem(SpreadsheetCommandBarButtonItem.Create(SpreadsheetCommandId.PivotTableShowOutlineForm, RibbonItemStyles.Large));
			reportLayoutSubItem.AddBarItem(SpreadsheetCommandBarButtonItem.Create(SpreadsheetCommandId.PivotTableShowTabularForm, RibbonItemStyles.Large));
			reportLayoutSubItem.AddBarItem(SpreadsheetCommandBarButtonItem.Create(SpreadsheetCommandId.PivotTableRepeatAllItemLabels, RibbonItemStyles.Large));
			reportLayoutSubItem.AddBarItem(SpreadsheetCommandBarButtonItem.Create(SpreadsheetCommandId.PivotTableDoNotRepeatItemLabels, RibbonItemStyles.Large));
			SpreadsheetCommandBarSubItem blankRowSubItem = SpreadsheetCommandBarSubItem.Create(SpreadsheetCommandId.PivotTableLayoutBlankRowsGroup);
			blankRowSubItem.AddBarItem(SpreadsheetCommandBarButtonItem.Create(SpreadsheetCommandId.PivotTableInsertBlankLineEachItem, RibbonItemStyles.Large));
			blankRowSubItem.AddBarItem(SpreadsheetCommandBarButtonItem.Create(SpreadsheetCommandId.PivotTableRemoveBlankLineEachItem, RibbonItemStyles.Large));
			items.Add(subtotalsSubItem);
			items.Add(grandTotalsSubItem);
			items.Add(reportLayoutSubItem);
			items.Add(blankRowSubItem);
		}
	}
	#endregion
	#region SpreadsheetPivotTableDesignPivotTableStyleOptionsItemBuilder
	public class SpreadsheetPivotTableDesignPivotTableStyleOptionsItemBuilder : CommandBasedBarItemBuilder {
		public override void PopulateItems(List<BarItem> items, BarCreationContextBase creationContext) {
			SpreadsheetCommandBarCheckItem rowHeadersCheckItem = SpreadsheetCommandBarCheckItem.Create(SpreadsheetCommandId.PivotTableToggleRowHeaders);
			rowHeadersCheckItem.CheckBoxVisibility = CheckBoxVisibility.BeforeText;
			SpreadsheetCommandBarCheckItem columnHeadersCheckItem = SpreadsheetCommandBarCheckItem.Create(SpreadsheetCommandId.PivotTableToggleColumnHeaders);
			columnHeadersCheckItem.CheckBoxVisibility = CheckBoxVisibility.BeforeText;
			SpreadsheetCommandBarCheckItem bandedRowsCheckItem = SpreadsheetCommandBarCheckItem.Create(SpreadsheetCommandId.PivotTableToggleBandedRows);
			bandedRowsCheckItem.CheckBoxVisibility = CheckBoxVisibility.BeforeText;
			SpreadsheetCommandBarCheckItem bandedColumnsCheckItem = SpreadsheetCommandBarCheckItem.Create(SpreadsheetCommandId.PivotTableToggleBandedColumns);
			bandedColumnsCheckItem.CheckBoxVisibility = CheckBoxVisibility.BeforeText;
			items.Add(rowHeadersCheckItem);
			items.Add(columnHeadersCheckItem);
			items.Add(bandedRowsCheckItem);
			items.Add(bandedColumnsCheckItem);
		}
	}
	#endregion
	#region SpreadsheetPivotTableDesignPivotTableStylesItemBuilder
	public class SpreadsheetPivotTableDesignPivotTableStylesItemBuilder : CommandBasedBarItemBuilder {
		public override void PopulateItems(List<BarItem> items, BarCreationContextBase creationContext) {
			items.Add(new GalleryPivotStylesItem());
		}
	}
	#endregion
	#region SpreadsheetPivotTableDesignLayoutBarCreator
	public class SpreadsheetPivotTableDesignLayoutBarCreator : ControlCommandBarCreator {
		public override Type SupportedRibbonPageType { get { return typeof(PivotTableDesignRibbonPage); } }
		public override Type SupportedRibbonPageGroupType { get { return typeof(PivotTableDesignLayoutRibbonPageGroup); } }
		public override Type SupportedRibbonPageCategoryType { get { return typeof(PivotTableToolsRibbonPageCategory); } }
		public override Type SupportedBarType { get { return typeof(PivotTableDesignLayoutBar); } }
		public override int DockRow { get { return 3; } }
		public override int DockColumn { get { return 0; } }
		public override Bar CreateBar() {
			return new PivotTableDesignLayoutBar();
		}
		public override CommandBasedBarItemBuilder CreateBarItemBuilder() {
			return new SpreadsheetPivotTableDesignLayoutItemBuilder();
		}
		public override CommandBasedRibbonPage CreateRibbonPageInstance() {
			return new PivotTableDesignRibbonPage();
		}
		public override CommandBasedRibbonPageGroup CreateRibbonPageGroupInstance() {
			return new PivotTableDesignLayoutRibbonPageGroup();
		}
		public override CommandBasedRibbonPageCategory CreateRibbonPageCategoryInstance() {
			return new PivotTableToolsRibbonPageCategory();
		}
	}
	#endregion
	#region SpreadsheetPivotTableDesignPivotTableStyleOptionsBarCreator
	public class SpreadsheetPivotTableDesignPivotTableStyleOptionsBarCreator : ControlCommandBarCreator {
		public override Type SupportedRibbonPageType { get { return typeof(PivotTableDesignRibbonPage); } }
		public override Type SupportedRibbonPageGroupType { get { return typeof(PivotTableDesignPivotTableStyleOptionsRibbonPageGroup); } }
		public override Type SupportedRibbonPageCategoryType { get { return typeof(PivotTableToolsRibbonPageCategory); } }
		public override Type SupportedBarType { get { return typeof(PivotTableDesignPivotTableStyleOptionsBar); } }
		public override int DockRow { get { return 3; } }
		public override int DockColumn { get { return 0; } }
		public override Bar CreateBar() {
			return new PivotTableDesignPivotTableStyleOptionsBar();
		}
		public override CommandBasedBarItemBuilder CreateBarItemBuilder() {
			return new SpreadsheetPivotTableDesignPivotTableStyleOptionsItemBuilder();
		}
		public override CommandBasedRibbonPage CreateRibbonPageInstance() {
			return new PivotTableDesignRibbonPage();
		}
		public override CommandBasedRibbonPageGroup CreateRibbonPageGroupInstance() {
			PivotTableDesignPivotTableStyleOptionsRibbonPageGroup pivotTableStyleOptionsRibbonPageGroup = new PivotTableDesignPivotTableStyleOptionsRibbonPageGroup();
			pivotTableStyleOptionsRibbonPageGroup.ItemsLayout = RibbonPageGroupItemsLayout.TwoRows;
			return pivotTableStyleOptionsRibbonPageGroup;
		}
		public override CommandBasedRibbonPageCategory CreateRibbonPageCategoryInstance() {
			return new PivotTableToolsRibbonPageCategory();
		}
	}
	#endregion
	#region SpreadsheetPivotTableDesignPivotTableStylesBarCreator
	public class SpreadsheetPivotTableDesignPivotTableStylesBarCreator : ControlCommandBarCreator {
		public override Type SupportedRibbonPageType { get { return typeof(PivotTableDesignRibbonPage); } }
		public override Type SupportedRibbonPageGroupType { get { return typeof(PivotTableDesignPivotTableStylesRibbonPageGroup); } }
		public override Type SupportedRibbonPageCategoryType { get { return typeof(PivotTableToolsRibbonPageCategory); } }
		public override Type SupportedBarType { get { return typeof(PivotTableDesignPivotTableStylesBar); } }
		public override int DockRow { get { return 3; } }
		public override int DockColumn { get { return 0; } }
		public override Bar CreateBar() {
			return new PivotTableDesignPivotTableStylesBar();
		}
		public override CommandBasedBarItemBuilder CreateBarItemBuilder() {
			return new SpreadsheetPivotTableDesignPivotTableStylesItemBuilder();
		}
		public override CommandBasedRibbonPage CreateRibbonPageInstance() {
			return new PivotTableDesignRibbonPage();
		}
		public override CommandBasedRibbonPageGroup CreateRibbonPageGroupInstance() {
			return new PivotTableDesignPivotTableStylesRibbonPageGroup();
		}
		public override CommandBasedRibbonPageCategory CreateRibbonPageCategoryInstance() {
			return new PivotTableToolsRibbonPageCategory();
		}
	}
	#endregion
	#region PivotTableDesignLayoutBar
	public class PivotTableDesignLayoutBar : ControlCommandBasedBar<SpreadsheetControl, SpreadsheetCommandId> {
		public PivotTableDesignLayoutBar() {
		}
		public PivotTableDesignLayoutBar(BarManager manager)
			: base(manager) {
		}
		public PivotTableDesignLayoutBar(BarManager manager, string name)
			: base(manager, name) {
		}
		public override string DefaultText { get { return XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Caption_PivotTableDesignLayout); } }
	}
	#endregion
	#region PivotTableDesignPivotTableStyleOptionsBar
	public class PivotTableDesignPivotTableStyleOptionsBar : ControlCommandBasedBar<SpreadsheetControl, SpreadsheetCommandId> {
		public PivotTableDesignPivotTableStyleOptionsBar() {
		}
		public PivotTableDesignPivotTableStyleOptionsBar(BarManager manager)
			: base(manager) {
		}
		public PivotTableDesignPivotTableStyleOptionsBar(BarManager manager, string name)
			: base(manager, name) {
		}
		public override string DefaultText { get { return XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Caption_PivotTableDesignPivotTableStyleOptions); } }
	}
	#endregion
	#region PivotTableDesignPivotTableStylesBar
	public class PivotTableDesignPivotTableStylesBar : ControlCommandBasedBar<SpreadsheetControl, SpreadsheetCommandId> {
		public PivotTableDesignPivotTableStylesBar() {
		}
		public PivotTableDesignPivotTableStylesBar(BarManager manager)
			: base(manager) {
		}
		public PivotTableDesignPivotTableStylesBar(BarManager manager, string name)
			: base(manager, name) {
		}
		public override string DefaultText { get { return XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Caption_PivotTableDesignPivotTableStyles); } }
	}
	#endregion
	#region PivotTableDesignRibbonPage
	public class PivotTableDesignRibbonPage : ControlCommandBasedRibbonPage {
		public PivotTableDesignRibbonPage() {
		}
		public PivotTableDesignRibbonPage(string text)
			: base(text) {
		}
		public override string DefaultText { get { return XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Caption_PivotTableDesign); } }
	}
	#endregion
	#region PivotTableDesignLayoutRibbonPageGroup
	public class PivotTableDesignLayoutRibbonPageGroup : SpreadsheetControlRibbonPageGroup {
		public PivotTableDesignLayoutRibbonPageGroup() {
		}
		public PivotTableDesignLayoutRibbonPageGroup(string text)
			: base(text) {
		}
		public override string DefaultText { get { return XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Caption_PivotTableDesignLayout); } }
	}
	#endregion
	#region PivotTableDesignPivotTableStyleOptionsRibbonPageGroup
	public class PivotTableDesignPivotTableStyleOptionsRibbonPageGroup : SpreadsheetControlRibbonPageGroup {
		public PivotTableDesignPivotTableStyleOptionsRibbonPageGroup() {
		}
		public PivotTableDesignPivotTableStyleOptionsRibbonPageGroup(string text)
			: base(text) {
		}
		public override string DefaultText { get { return XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Caption_PivotTableDesignPivotTableStyleOptions); } }
	}
	#endregion
	#region PivotTableDesignPivotTableStylesRibbonPageGroup
	public class PivotTableDesignPivotTableStylesRibbonPageGroup : SpreadsheetControlRibbonPageGroup {
		public PivotTableDesignPivotTableStylesRibbonPageGroup() {
		}
		public PivotTableDesignPivotTableStylesRibbonPageGroup(string text)
			: base(text) {
		}
		public override string DefaultText { get { return XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Caption_PivotTableDesignPivotTableStyles); } }
	}
	#endregion
	#region GalleryPivotStylesItem
	public class GalleryPivotStylesItem : SpreadsheetCommandGalleryBarItem {
		#region Fields
		internal const GalleryItemAutoSizeMode ItemAutoSizeMode = GalleryItemAutoSizeMode.None;
		const int minColumnCount = 1;
		const int maxColumnCount = 7;
		internal const int RowCount = 10;
		internal const int MaxDropDownColumnCount = 7;
		const int selectionWidth = 3;
		const int lightPresetsCount = 28;
		const int mediumPresetsCount = 28;
		const int darkPresetsCount = 28;
		const string nameIsNone = "None";
		const string namePrefix = "PivotStyle";
		const string customPrefix = "Custom";
		const string lightPrefix = "Light";
		const string mediumPrefix = "Medium";
		const string darkPrefix = "Dark";
		IList<string> customStyleNames;
		#endregion
		#region Static Members
		internal static Size DefaultItemSize = new Size(73, 61);
		static TableStyleViewInfoPainter viewPainter = new TableStyleViewInfoPainter();
		protected internal static void InitDropDownGalleryCore(InDropDownGallery gallery, IList<string> customStyleNames) {
			gallery.Groups.Clear();
			if (customStyleNames != null) {
				int count = customStyleNames.Count;
				if (count > 0) {
					GalleryItemGroup groupCustom = GalleryPivotStylesItem.CreateGalleryItemGroup(XtraSpreadsheetStringId.Caption_CustomTableStyleCategory);
					for (int i = 0; i < count; i++) {
						string name = customStyleNames[i];
						groupCustom.Items.Add(CreateGalleryItemCore(name, name));
					}
					AddGalleryItemGroup(groupCustom, gallery);
				}
			}
			PopulateGalleryItem(XtraSpreadsheetStringId.Caption_LightTableStyleCategory, lightPrefix, XtraSpreadsheetStringId.Caption_LightTableStyleNamePart, lightPresetsCount, gallery);
			PopulateGalleryItem(XtraSpreadsheetStringId.Caption_MediumTableStyleCategory, mediumPrefix, XtraSpreadsheetStringId.Caption_MediumTableStyleNamePart, mediumPresetsCount, gallery);
			PopulateGalleryItem(XtraSpreadsheetStringId.Caption_DarkTableStyleCategory, darkPrefix, XtraSpreadsheetStringId.Caption_DarkTableStyleNamePart, darkPresetsCount, gallery);
		}
		static void PopulateGalleryItem(XtraSpreadsheetStringId categoryId, string groupPrefix, XtraSpreadsheetStringId namePartId, int presetsCount, InDropDownGallery popupGallery) {
			GalleryItemGroup group = CreateGalleryItemGroup(categoryId);
			PopulateGalleryItems(group, groupPrefix, namePartId, presetsCount);
			AddGalleryItemGroup(group, popupGallery);
		}
		static GalleryItemGroup CreateGalleryItemGroup(XtraSpreadsheetStringId stringId) {
			GalleryItemGroup result = new GalleryItemGroup();
			result.Caption = XtraSpreadsheetLocalizer.GetString(stringId);
			return result;
		}
		static void AddGalleryItemGroup(GalleryItemGroup group, InDropDownGallery gallery) {
			if (group.Items.Count > 0)
				gallery.Groups.Add(group);
		}
		static void PopulateGalleryItems(GalleryItemGroup group, string groupPrefix, XtraSpreadsheetStringId groupPrefixId, int presetsCount) {
			if (groupPrefix == lightPrefix)
				group.Items.Add(CreateGalleryItemCore(XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Caption_TableStyleNameIsNone), nameIsNone));
			for (int i = 0; i < presetsCount; i++)
				group.Items.Add(CreateGalleryItem(groupPrefix, groupPrefixId, i));
		}
		static GalleryItem CreateGalleryItem(string groupPrefix, XtraSpreadsheetStringId groupPrefixId, int index) {
			string number = (index + 1).ToString();
			string hint =
				XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Caption_PrefixPivotStyleNamePart) + " " +
				XtraSpreadsheetLocalizer.GetString(groupPrefixId) + " " + number;
			string tag = namePrefix + groupPrefix + number;
			return CreateGalleryItemCore(hint, tag);
		}
		static GalleryItem CreateGalleryItemCore(string hint, string tag) {
			GalleryItem item = new GalleryItem();
			item.Hint = hint;
			item.Tag = tag;
			return item;
		}
		protected internal static void OnCustomDrawItemCore(GalleryItemCustomDrawEventArgs e, DocumentModel documentModel) {
			string styleName = e.Item.Tag as string;
			if (String.IsNullOrEmpty(styleName))
				return;
			GalleryItemViewInfo viewInfo = e.ItemInfo as GalleryItemViewInfo;
			if (viewInfo == null)
				return;
			Draw(styleName, e.Cache, Rectangle.Inflate(viewInfo.Bounds, -selectionWidth, -selectionWidth), documentModel);
			e.Handled = true;
		}
		static void Draw(string styleName, GraphicsCache cache, Rectangle rectangle, DocumentModel documentModel) {
			viewPainter.Info = new PivotStyleViewInfo(documentModel, styleName);
			viewPainter.Cache = cache;
			viewPainter.Draw(rectangle);
		}
		#endregion
		public GalleryPivotStylesItem() {
		}
		#region Properties
		protected override SpreadsheetCommandId CommandId { get { return SpreadsheetCommandId.ModifyPivotTableStyles; } }
		protected override bool DropDownGalleryShowGroupCaption { get { return true; } }
		#endregion
		protected override void Initialize() {
			base.Initialize();
			Gallery.MinimumColumnCount = minColumnCount;
			Gallery.ColumnCount = maxColumnCount;
			Gallery.RowCount = RowCount;
			Gallery.ItemSize = DefaultItemSize;
			Gallery.ItemAutoSizeMode = ItemAutoSizeMode;
			Gallery.ShowItemText = false;
			Gallery.ShowGroupCaption = false;
			Gallery.DrawImageBackground = false;
		}
		protected override void SubscribeEvents() {
			base.SubscribeEvents();
			GalleryCustomDrawItemImage += OnCustomDrawItem;
		}
		protected override void SubscribeControlEvents() {
			base.SubscribeControlEvents();
			Control.ModifiedChanged += OnCollectionChanged;
			Control.EmptyDocumentCreated += OnCollectionChanged;
			Control.DocumentLoaded += OnCollectionChanged;
			Control.DocumentModel.DocumentCleared += OnCollectionChanged;
			Control.DocumentModel.StyleSheet.TableStyles.CollectionChanged += OnCollectionChanged;
		}
		protected override void UnsubscribeControlEvents() {
			base.UnsubscribeControlEvents();
			Control.ModifiedChanged -= OnCollectionChanged;
			Control.EmptyDocumentCreated -= OnCollectionChanged;
			Control.DocumentLoaded -= OnCollectionChanged;
			Control.DocumentModel.DocumentCleared -= OnCollectionChanged;
			Control.DocumentModel.StyleSheet.TableStyles.CollectionChanged -= OnCollectionChanged;
		}
		void OnCollectionChanged(object sender, EventArgs e) {
			if (DesignMode)
				return;
			customStyleNames = Control.DocumentModel.StyleSheet.TableStyles.GetExistingNonHiddenCustomPivotStyleNames();
			Gallery.BeginUpdate();
			try {
				PopulateGalleryItems();
			}
			finally {
				Gallery.EndUpdate();
			}
		}
		protected internal virtual void PopulateGalleryItems() {
			Gallery.Groups.Clear();
			GalleryItemGroup commonGroup = new GalleryItemGroup();
			PopulateGalleryItemsForCustomStyles(commonGroup);
			PopulateGalleryItems(commonGroup, lightPrefix, XtraSpreadsheetStringId.Caption_LightTableStyleNamePart, lightPresetsCount);
			PopulateGalleryItems(commonGroup, mediumPrefix, XtraSpreadsheetStringId.Caption_MediumTableStyleNamePart, mediumPresetsCount);
			PopulateGalleryItems(commonGroup, darkPrefix, XtraSpreadsheetStringId.Caption_DarkTableStyleNamePart, darkPresetsCount);
			Gallery.Groups.Add(commonGroup);
		}
		protected override void InitDropDownGallery(object sender, InplaceGalleryEventArgs e) {
			base.InitDropDownGallery(sender, e);
			InDropDownGallery popupGallery = e.PopupGallery;
			popupGallery.ColumnCount = MaxDropDownColumnCount;
			InitDropDownGalleryCore(popupGallery, customStyleNames);
		}
		void PopulateGalleryItemsForCustomStyles(GalleryItemGroup group) {
			int count = customStyleNames.Count;
			for (int i = 0; i < count; i++) {
				string name = customStyleNames[i];
				group.Items.Add(CreateGalleryItemCore(name, name));
			}
		}
		void OnCustomDrawItem(object sender, GalleryItemCustomDrawEventArgs e) {
			if (Control == null || Control.IsDisposed)
				return;
			OnCustomDrawItemCore(e, Control.DocumentModel);
		}
		protected override void InvokeCommand() {
			Command command = CreateCommand();
			if (command != null) {
				ICommandUIState state = CreateCommandUIState(command);
				if (command.CanExecute())
					command.ForceExecute(state);
			}
		}
		protected internal virtual ICommandUIState CreateCommandUIState(Command command) {
			DefaultValueBasedCommandUIState<string> state = new DefaultValueBasedCommandUIState<string>();
			state.Value = SelectedItem.Tag as string;
			return state;
		}
		protected override ICommandUIState CreateGalleryItemUIState() {
			return new BarGalleryItemValueUIState<string>(this);
		}
	}
	#endregion
}
