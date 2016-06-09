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
using System.ComponentModel;
using System.Drawing;
using DevExpress.Export.Xl;
using DevExpress.Utils;
using DevExpress.Utils.Commands;
using DevExpress.Utils.Drawing;
using DevExpress.XtraBars;
using DevExpress.XtraBars.Commands;
using DevExpress.XtraBars.Commands.Internal;
using DevExpress.XtraBars.Commands.Ribbon;
using DevExpress.XtraBars.Ribbon;
using DevExpress.XtraBars.Ribbon.Gallery;
using DevExpress.XtraBars.Ribbon.ViewInfo;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraSpreadsheet.Commands;
using DevExpress.XtraSpreadsheet.Layout;
using DevExpress.XtraSpreadsheet.Localization;
using DevExpress.XtraSpreadsheet.Model;
namespace DevExpress.XtraSpreadsheet.UI {
	#region TableToolsRibbonPageCategory
	public class TableToolsRibbonPageCategory : ControlCommandBasedRibbonPageCategory<SpreadsheetControl, SpreadsheetCommandId> {
		public TableToolsRibbonPageCategory() {
			this.Visible = false;
		}
		public override string DefaultText { get { return XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Caption_PageCategoryTableTools); } }
		protected override SpreadsheetCommandId EmptyCommandId { get { return SpreadsheetCommandId.None; } }
		public override SpreadsheetCommandId CommandId { get { return SpreadsheetCommandId.TableToolsCommandGroup; } }
		protected override RibbonPageCategory CreateRibbonPageCategory() {
			return new TableToolsRibbonPageCategory();
		}
	}
	#endregion
	#region SpreadsheetTablePropertiesItemBuilder
	public class SpreadsheetTablePropertiesItemBuilder : CommandBasedBarItemBuilder {
		public override void PopulateItems(List<BarItem> items, BarCreationContextBase creationContext) {
			BarStaticItem barItem = new BarStaticItem();
			barItem.Caption = XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.MenuCmd_TableToolsRenameTableCommand) + ":";
			items.Add(barItem);
			items.Add(new RenameTableItem());
		}
	}
	#endregion
	#region SpreadsheetTableToolsItemBuilder
	public class SpreadsheetTableToolsItemBuilder : CommandBasedBarItemBuilder {
		public override void PopulateItems(List<BarItem> items, BarCreationContextBase creationContext) {
			items.Add(SpreadsheetCommandBarCheckItem.Create(SpreadsheetCommandId.TableToolsConvertToRange, RibbonItemStyles.SmallWithText));
		}
	}
	#endregion
	#region SpreadsheetTableStyleOptionsItemBuilder
	public class SpreadsheetTableStyleOptionsItemBuilder : CommandBasedBarItemBuilder {
		public override void PopulateItems(List<BarItem> items, BarCreationContextBase creationContext) {
			SpreadsheetCommandBarCheckItem checkItem = SpreadsheetCommandBarCheckItem.Create(SpreadsheetCommandId.TableToolsToggleHeaderRow);
			checkItem.CheckBoxVisibility = CheckBoxVisibility.BeforeText;
			items.Add(checkItem);
			checkItem = SpreadsheetCommandBarCheckItem.Create(SpreadsheetCommandId.TableToolsToggleTotalRow);
			checkItem.CheckBoxVisibility = CheckBoxVisibility.BeforeText;
			items.Add(checkItem);
			checkItem = SpreadsheetCommandBarCheckItem.Create(SpreadsheetCommandId.TableToolsToggleBandedColumns);
			checkItem.CheckBoxVisibility = CheckBoxVisibility.BeforeText;
			items.Add(checkItem);
			checkItem = SpreadsheetCommandBarCheckItem.Create(SpreadsheetCommandId.TableToolsToggleFirstColumn);
			checkItem.CheckBoxVisibility = CheckBoxVisibility.BeforeText;
			items.Add(checkItem);
			checkItem = SpreadsheetCommandBarCheckItem.Create(SpreadsheetCommandId.TableToolsToggleLastColumn);
			checkItem.CheckBoxVisibility = CheckBoxVisibility.BeforeText;
			items.Add(checkItem);
			checkItem = SpreadsheetCommandBarCheckItem.Create(SpreadsheetCommandId.TableToolsToggleBandedRows);
			checkItem.CheckBoxVisibility = CheckBoxVisibility.BeforeText;
			items.Add(checkItem);
		}
	}
	#endregion
	#region SpreadsheetTableStylesItemBuilder
	public class SpreadsheetTableStylesItemBuilder : CommandBasedBarItemBuilder {
		public override void PopulateItems(List<BarItem> items, BarCreationContextBase creationContext) {
			if (creationContext.IsRibbon) {
				items.Add(new GalleryTableStylesItem());
			}
		}
	}
	#endregion
	#region GalleryTableStylesItem
	public class GalleryTableStylesItem : SpreadsheetCommandGalleryBarItem {
		#region Fields
		internal const GalleryItemAutoSizeMode ItemAutoSizeMode = GalleryItemAutoSizeMode.None;
		const int minColumnCount = 1;
		const int maxColumnCount = 7;
		internal const int RowCount = 10;
		internal const int MaxDropDownColumnCount = 7;
		const int selectionWidth = 3;
		const int lightPresetsCount = 21;
		const int mediumPresetsCount = 28;
		const int darkPresetsCount = 11;
		const string nameIsNone = "None";
		const string namePrefix = "TableStyle";
		const string customPrefix = "Custom";
		const string lightPrefix = "Light";
		const string mediumPrefix = "Medium";
		const string darkPrefix = "Dark";
		IList<string> customStyleNames;
		#endregion
		#region Static Members
		internal static Size DefaultItemSize = new Size(73, 58);
		static TableStyleViewInfoPainter viewPainter = new TableStyleViewInfoPainter();
		protected internal static void InitDropDownGalleryCore(InDropDownGallery gallery, IList<string> customStyleNames) {
			gallery.Groups.Clear();
			if (customStyleNames != null) {
				int count = customStyleNames.Count;
				if (count > 0) {
					GalleryItemGroup groupCustom = GalleryTableStylesItem.CreateGalleryItemGroup(XtraSpreadsheetStringId.Caption_CustomTableStyleCategory);
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
				XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Caption_PrefixTableStyleNamePart) + " " +
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
			viewPainter.Info = new TableStyleViewInfo(documentModel, styleName);
			viewPainter.Cache = cache;
			viewPainter.Draw(rectangle);
		}
		#endregion
		public GalleryTableStylesItem() {
		}
		#region Properties
		protected override SpreadsheetCommandId CommandId { get { return SpreadsheetCommandId.ModifyTableStyles; } }
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
			customStyleNames = Control.DocumentModel.StyleSheet.TableStyles.GetExistingNonHiddenCustomTableStyleNames();
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
	#region RenameTableItem
	public class RenameTableItem : SpreadsheetCommandBarEditItem<string> {
		const int defaultWidth = 80;
		public RenameTableItem() {
			Width = defaultWidth;
		}
		public RenameTableItem(BarManager manager, string caption)
			: base(manager, caption) {
			Width = defaultWidth;
		}
		[DefaultValue(defaultWidth)]
		public override int Width { get { return base.Width; } set { base.Width = value; } }
		protected override SpreadsheetCommandId CommandId { get { return SpreadsheetCommandId.TableToolsRenameTable; } }
		protected override RepositoryItem CreateEdit() {
			return new RepositoryItemTextEdit();
		}
	}
	#endregion
	#region SpreadsheetTablePropertiesBarCreator
	public class SpreadsheetTablePropertiesBarCreator : ControlCommandBarCreator {
		public override Type SupportedRibbonPageType { get { return typeof(TableToolsDesignRibbonPage); } }
		public override Type SupportedRibbonPageGroupType { get { return typeof(TablePropertiesRibbonPageGroup); } }
		public override Type SupportedBarType { get { return typeof(TablePropertiesBar); } }
		public override Type SupportedRibbonPageCategoryType { get { return typeof(TableToolsRibbonPageCategory); } }
		public override int DockRow { get { return 3; } }
		public override int DockColumn { get { return 1; } }
		public override Bar CreateBar() {
			return new TablePropertiesBar();
		}
		public override CommandBasedBarItemBuilder CreateBarItemBuilder() {
			return new SpreadsheetTablePropertiesItemBuilder();
		}
		public override CommandBasedRibbonPage CreateRibbonPageInstance() {
			return new TableToolsDesignRibbonPage();
		}
		public override CommandBasedRibbonPageGroup CreateRibbonPageGroupInstance() {
			return new TablePropertiesRibbonPageGroup();
		}
		public override CommandBasedRibbonPageCategory CreateRibbonPageCategoryInstance() {
			return new TableToolsRibbonPageCategory();
		}
	}
	#endregion
	#region SpreadsheetTableToolsBarCreator
	public class SpreadsheetTableToolsBarCreator : ControlCommandBarCreator {
		public override Type SupportedRibbonPageType { get { return typeof(TableToolsDesignRibbonPage); } }
		public override Type SupportedRibbonPageGroupType { get { return typeof(TableToolsRibbonPageGroup); } }
		public override Type SupportedBarType { get { return typeof(TableToolsBar); } }
		public override Type SupportedRibbonPageCategoryType { get { return typeof(TableToolsRibbonPageCategory); } }
		public override int DockRow { get { return 3; } }
		public override int DockColumn { get { return 2; } }
		public override Bar CreateBar() {
			return new TableToolsBar();
		}
		public override CommandBasedBarItemBuilder CreateBarItemBuilder() {
			return new SpreadsheetTableToolsItemBuilder();
		}
		public override CommandBasedRibbonPage CreateRibbonPageInstance() {
			return new TableToolsDesignRibbonPage();
		}
		public override CommandBasedRibbonPageGroup CreateRibbonPageGroupInstance() {
			return new TableToolsRibbonPageGroup();
		}
		public override CommandBasedRibbonPageCategory CreateRibbonPageCategoryInstance() {
			return new TableToolsRibbonPageCategory();
		}
	}
	#endregion
	#region SpreadsheetTableStyleOptionsBarCreator
	public class SpreadsheetTableStyleOptionsBarCreator : ControlCommandBarCreator {
		public override Type SupportedRibbonPageType { get { return typeof(TableToolsDesignRibbonPage); } }
		public override Type SupportedRibbonPageGroupType { get { return typeof(TableStyleOptionsRibbonPageGroup); } }
		public override Type SupportedBarType { get { return typeof(TableStyleOptionsBar); } }
		public override Type SupportedRibbonPageCategoryType { get { return typeof(TableToolsRibbonPageCategory); } } 
		public override int DockRow { get { return 3; } }
		public override int DockColumn { get { return 6; } }
		public override Bar CreateBar() {
			return new TableStyleOptionsBar();
		}
		public override CommandBasedBarItemBuilder CreateBarItemBuilder() {
			return new SpreadsheetTableStyleOptionsItemBuilder();
		}
		public override CommandBasedRibbonPage CreateRibbonPageInstance() {
			return new TableToolsDesignRibbonPage();
		}
		public override CommandBasedRibbonPageGroup CreateRibbonPageGroupInstance() {
			return new TableStyleOptionsRibbonPageGroup();
		}
		public override CommandBasedRibbonPageCategory CreateRibbonPageCategoryInstance() {
			return new TableToolsRibbonPageCategory();
		}
	}
	#endregion
	#region SpreadsheetTableStylesBarCreator
	public class SpreadsheetTableStylesBarCreator : ControlCommandBarCreator {
		public override Type SupportedRibbonPageType { get { return typeof(TableToolsDesignRibbonPage); } }
		public override Type SupportedRibbonPageGroupType { get { return typeof(TableStylesRibbonPageGroup); } }
		public override Type SupportedRibbonPageCategoryType { get { return typeof(TableToolsRibbonPageCategory); } }
		public override Type SupportedBarType { get { return typeof(TableStylesBar); } }
		public override int DockRow { get { return 2; } }
		public override int DockColumn { get { return 1; } }
		public override Bar CreateBar() {
			return new TableStylesBar();
		}
		public override CommandBasedBarItemBuilder CreateBarItemBuilder() {
			return new SpreadsheetTableStylesItemBuilder();
		}
		public override CommandBasedRibbonPage CreateRibbonPageInstance() {
			return new TableToolsDesignRibbonPage();
		}
		public override CommandBasedRibbonPageGroup CreateRibbonPageGroupInstance() {
			return new TableStylesRibbonPageGroup();
		}
		public override CommandBasedRibbonPageCategory CreateRibbonPageCategoryInstance() {
			return new TableToolsRibbonPageCategory();
		}
	}
	#endregion
	#region TablePropertiesBar
	public class TablePropertiesBar : ControlCommandBasedBar<SpreadsheetControl, SpreadsheetCommandId> {
		public TablePropertiesBar() {
		}
		public TablePropertiesBar(BarManager manager)
			: base(manager) {
		}
		public TablePropertiesBar(BarManager manager, string name)
			: base(manager, name) {
		}
		public override string DefaultText { get { return XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Caption_GroupTableProperties); } }
	}
	#endregion
	#region TableToolsBar
	public class TableToolsBar : ControlCommandBasedBar<SpreadsheetControl, SpreadsheetCommandId> {
		public TableToolsBar() {
		}
		public TableToolsBar(BarManager manager)
			: base(manager) {
		}
		public TableToolsBar(BarManager manager, string name)
			: base(manager, name) {
		}
		public override string DefaultText { get { return XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Caption_GroupTableTools); } }
	}
	#endregion
	#region TableStyleOptionsBar
	public class TableStyleOptionsBar : ControlCommandBasedBar<SpreadsheetControl, SpreadsheetCommandId> {
		public TableStyleOptionsBar() {
		}
		public TableStyleOptionsBar(BarManager manager)
			: base(manager) {
		}
		public TableStyleOptionsBar(BarManager manager, string name)
			: base(manager, name) {
		}
		public override string DefaultText { get { return XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Caption_GroupTableStyleOptions); } }
	}
	#endregion
	#region TableStylesBar
	public class TableStylesBar : ControlCommandBasedBar<SpreadsheetControl, SpreadsheetCommandId> {
		public TableStylesBar() {
		}
		public TableStylesBar(BarManager manager)
			: base(manager) {
		}
		public TableStylesBar(BarManager manager, string name)
			: base(manager, name) {
		}
		public override string DefaultText { get { return XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Caption_GroupTableStyleOptions); } }
	}
	#endregion
	#region TableToolsDesignRibbonPage
	public class TableToolsDesignRibbonPage : ControlCommandBasedRibbonPage {
		public TableToolsDesignRibbonPage() {
		}
		public TableToolsDesignRibbonPage(string text)
			: base(text) {
		}
		public override string DefaultText { get { return XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Caption_TableToolsDesignPage); } }
	}
	#endregion
	#region TablePropertiesRibbonPageGroup
	public class TablePropertiesRibbonPageGroup : SpreadsheetControlRibbonPageGroup {
		public TablePropertiesRibbonPageGroup() {
		}
		public TablePropertiesRibbonPageGroup(string text)
			: base(text) {
		}
		public override string DefaultText { get { return XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Caption_GroupTableProperties); } }
	}
	#endregion
	#region TableToolsRibbonPageGroup
	public class TableToolsRibbonPageGroup : SpreadsheetControlRibbonPageGroup {
		public TableToolsRibbonPageGroup() {
		}
		public TableToolsRibbonPageGroup(string text)
			: base(text) {
		}
		public override string DefaultText { get { return XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Caption_GroupTableTools); } }
	}
	#endregion
	#region TableStyleOptionsRibbonPageGroup
	public class TableStyleOptionsRibbonPageGroup : SpreadsheetControlRibbonPageGroup {
		public TableStyleOptionsRibbonPageGroup() {
		}
		public TableStyleOptionsRibbonPageGroup(string text)
			: base(text) {
		}
		public override string DefaultText { get { return XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Caption_GroupTableStyleOptions); } }
	}
	#endregion
	#region TableStylesRibbonPageGroup
	public class TableStylesRibbonPageGroup : SpreadsheetControlRibbonPageGroup {
		public TableStylesRibbonPageGroup() {
		}
		public TableStylesRibbonPageGroup(string text)
			: base(text) {
		}
		public override string DefaultText { get { return XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Caption_GroupTableStyleOptions); } }
	}
	#endregion
	#region TableStyleViewInfoPainter
	public class TableStyleViewInfoPainter {
		const int delta = 3;
		#region Properties
		public GraphicsCache Cache { get; set; }
		public ITableStyleViewInfo Info { get; set; }
		int ColumnCount { get { return Info.ColumnCount; } }
		int RowCount { get { return Info.RowCount; } }
		#endregion
		public void Draw(Rectangle tableRectangle) {
			int partWidth = tableRectangle.Width / ColumnCount;
			int partHeight = tableRectangle.Height / RowCount;
			for (int column = 0; column < ColumnCount; column++)
				for (int row = 0; row < RowCount; row++) {
					Rectangle rectanglePart = new Rectangle(tableRectangle.X + column * partWidth, tableRectangle.Y + row * partHeight, partWidth, partHeight);
					CellPosition absolutePosition = new CellPosition(column, row);
					DrawBackgroundPart(rectanglePart, absolutePosition);
					DrawLines(rectanglePart, absolutePosition);
				}
		}
		void DrawBackgroundPart(Rectangle rectanglePart, CellPosition absolutePosition) {
			IActualFillInfo fill = Info.GetActualFillInfo(absolutePosition);
			if (fill.FillType == ModelFillType.Pattern)
				DrawBackgroundPartFromPatternFill(rectanglePart, fill);
			else
				DrawBackgroundPartFromGradientFill(rectanglePart, fill.GradientFill);
		}
		void DrawBackgroundPartFromPatternFill(Rectangle fillBounds, IActualFillInfo fill) {
			Color backColor = Cell.GetBackgroundColor(fill);
			DrawBackgroundFromColor(fillBounds, backColor);
		}
		void DrawBackgroundPartFromGradientFill(Rectangle fillBounds, IActualGradientFillInfo gradientFill) {
			CellBackgroundDisplayFormat displayFormat = new CellBackgroundDisplayFormat();
			displayFormat.GradientFill = gradientFill;
			displayFormat.Bounds = fillBounds;
			DrawBackgroundFromBrush(displayFormat);
		}
		void DrawBackgroundFromColor(Rectangle fillBounds, Color backColor) {
			if (!DXColor.IsTransparentOrEmpty(backColor))
				Cache.FillRectangle(backColor, fillBounds);
		}
		void DrawBackgroundFromBrush(CellBackgroundDisplayFormat displayFormat) {
			Rectangle fillBounds = displayFormat.Bounds;
			Brush brush = displayFormat.CreateGradientBrush();
			if (brush != Brushes.Transparent)
				using (brush)
					Cache.FillRectangle(brush, fillBounds);
		}
		void DrawLines(Rectangle borderBounds, CellPosition absolutePosition) {
			DrawLeftLine(borderBounds, absolutePosition);
			DrawRightLine(borderBounds, absolutePosition);
			DrawTopLine(borderBounds, absolutePosition);
			DrawBottomLine(borderBounds, absolutePosition);
			DrawEmbeddedLine(borderBounds, absolutePosition);
		}
		void DrawLeftLine(Rectangle borderBounds, CellPosition absolutePosition) {
			XlBorderLineStyle style = Info.GetLeftBorderLineStyle(absolutePosition);
			if (style != XlBorderLineStyle.None) {
				Color color = Info.GetLeftBorderColor(absolutePosition);
				DrawLine(color, borderBounds.X, borderBounds.Y, borderBounds.X, borderBounds.Bottom);
			}
		}
		void DrawRightLine(Rectangle borderBounds, CellPosition absolutePosition) {
			XlBorderLineStyle style = Info.GetRightBorderLineStyle(absolutePosition);
			if (style != XlBorderLineStyle.None) {
				Color color = Info.GetRightBorderColor(absolutePosition);
				DrawLine(color, borderBounds.Right, borderBounds.Y, borderBounds.Right, borderBounds.Bottom);
			}
		}
		void DrawTopLine(Rectangle borderBounds, CellPosition absolutePosition) {
			XlBorderLineStyle style = Info.GetTopBorderLineStyle(absolutePosition);
			if (style != XlBorderLineStyle.None) {
				Color color = Info.GetTopBorderColor(absolutePosition);
				DrawLine(color, borderBounds.X, borderBounds.Y, borderBounds.Right, borderBounds.Y);
			}
		}
		void DrawBottomLine(Rectangle borderBounds, CellPosition absolutePosition) {
			XlBorderLineStyle style = Info.GetBottomBorderLineStyle(absolutePosition);
			if (style != XlBorderLineStyle.None) {
				Color color = Info.GetBottomBorderColor(absolutePosition);
				DrawLine(color, borderBounds.X, borderBounds.Bottom, borderBounds.Right, borderBounds.Bottom);
			}
		}
		void DrawEmbeddedLine(Rectangle borderBounds, CellPosition absolutePosition) {
			Color color = Info.GetTextColor(absolutePosition);
			color = DXColor.IsTransparentOrEmpty(color) ? DXColor.Black : color;
			int y = borderBounds.Y + borderBounds.Height / 2;
			DrawLine(color, borderBounds.X + delta, y, borderBounds.Right - delta, y);
		}
		void DrawLine(Color color, int beginX, int beginY, int endX, int endY) {
			using (Pen pen = new Pen(color, 1))
				Cache.Graphics.DrawLine(pen, beginX, beginY, endX, endY);
		}
	}
	#endregion
}
