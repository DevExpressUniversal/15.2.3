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
using DevExpress.Office.UI;
using DevExpress.Utils.Commands;
using DevExpress.XtraBars;
using DevExpress.XtraBars.Commands;
using DevExpress.XtraBars.Commands.Internal;
using DevExpress.XtraBars.Commands.Ribbon;
using DevExpress.XtraBars.Ribbon;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraRichEdit.Commands;
using DevExpress.XtraRichEdit.Forms.Design;
using DevExpress.XtraRichEdit.Localization;
using DevExpress.XtraRichEdit.Model;
using System.Drawing;
using DevExpress.XtraBars.Ribbon.ViewInfo;
using DevExpress.Utils.Drawing;
using DevExpress.Utils;
using DevExpress.Office;
using DevExpress.XtraRichEdit.Design;
using DevExpress.XtraEditors;
using System.Windows.Forms;
using DevExpress.XtraRichEdit.Forms;
namespace DevExpress.XtraRichEdit.UI {
	#region RichEditTableStyleOptionsItemBuilder
	public class RichEditTableStyleOptionsItemBuilder : CommandBasedBarItemBuilder {
		public override void PopulateItems(List<BarItem> items, BarCreationContextBase creationContext) {
			items.Add(new ToggleFirstRowItem());
			items.Add(new ToggleLastRowItem());
			items.Add(new ToggleBandedRowsItem());
			items.Add(new ToggleFirstColumnItem());
			items.Add(new ToggleLastColumnItem());
			items.Add(new ToggleBandedColumnsItem());
		}
	}
	#endregion
	#region ToggleFirstRowItem
	public class ToggleFirstRowItem : ToggleTableLookItem {
		protected override RichEditCommandId CommandId { get { return RichEditCommandId.ToggleFirstRow; } }
	}
	#endregion
	#region ToggleLastRowItem
	public class ToggleLastRowItem : ToggleTableLookItem {
		protected override RichEditCommandId CommandId { get { return RichEditCommandId.ToggleLastRow; } }
	}
	#endregion
	#region ToggleFirstColumnItem
	public class ToggleFirstColumnItem : ToggleTableLookItem {
		protected override RichEditCommandId CommandId { get { return RichEditCommandId.ToggleFirstColumn; } }
	}
	#endregion
	#region ToggleLastColumnItem
	public class ToggleLastColumnItem : ToggleTableLookItem {
		protected override RichEditCommandId CommandId { get { return RichEditCommandId.ToggleLastColumn; } }
	}
	#endregion
	#region ToggleBandedRowsItem
	public class ToggleBandedRowsItem : ToggleTableLookItem {
		protected override RichEditCommandId CommandId { get { return RichEditCommandId.ToggleBandedRows; } }
	}
	#endregion
	[Obsolete("You should use the 'ToggleBandedColumnsItem' instead")]
	#region ToggleBandedColumnItem
	public class ToggleBandedColumnItem : ToggleTableLookItem {
		protected override Command CreateCommand() {
			if (Control == null)
				return null;
			Command command = new ToggleBandedColumnCommand(Control);
			return command;
		}
	}
	#endregion
	#region ToggleBandedColumnsItem
	public class ToggleBandedColumnsItem : ToggleTableLookItem {
		protected override RichEditCommandId CommandId { get { return RichEditCommandId.ToggleBandedColumns; } }
	}
	#endregion
	#region ToggleTableLookItem
	public abstract class ToggleTableLookItem : RichEditCommandBarCheckItem {
		protected ToggleTableLookItem() {
			this.CheckBoxVisibility = XtraBars.CheckBoxVisibility.BeforeText;
		}
		protected override RichEditCommandId CommandId { get { return RichEditCommandId.None; } }
		protected  override Command CreateCommand(){
			 if (Control == null)
				return null;
			Command command = Control.CreateCommand(CommandId);
			return command;
		}
	}
	#endregion
	#region RichEditTableStylesItemBuilder
	public class RichEditTableStylesItemBuilder : CommandBasedBarItemBuilder {
		public override void PopulateItems(List<BarItem> items, BarCreationContextBase creationContext) {
			items.Add(GetChangeStyleItem(creationContext.IsRibbon));
		}
		protected virtual BarItem GetChangeStyleItem(bool isRibbon) {
			GalleryChangeTableStyleItem gallery = new GalleryChangeTableStyleItem();
			gallery.Gallery.Groups.Add(new GalleryItemGroup());
			return gallery;
		}
	}
	#endregion
	#region GalleryChangeTableStyleItemBase
	public abstract class GalleryChangeTableStyleItemBase : RichEditCommandGalleryBarItem {
		#region Fields
		Size defaultImageSize = new Size(70, 50);
		const int minColumnCount = 1;
		const int maxColumnCount = 3;
		GalleryItem currentItem;
		BarItem newItem;
		BarItem modifyItem;
		BarItem deleteItem;
		BarItemLink newItemLink;
		BarItemLink modifyItemLink;
		BarItemLink deleteItemLink;
		RepositoryItemRichEditTableStyleEditBase repository;
		#endregion
		#region Properties
		public GalleryItem CurrentItem { get { return currentItem; } set { currentItem = value; } }
		public BarItem NewItem { get { return newItem; } set { newItem = value; } }
		public BarItem ModifyItem { get { return modifyItem; } set { modifyItem = value; } }
		public BarItem DeleteItem { get { return deleteItem; } set { deleteItem = value; } }
		public BarItemLink NewItemLink { get { return newItemLink; } set { newItemLink = value; } }
		public BarItemLink ModifyItemLink { get { return modifyItemLink; } set { modifyItemLink = value; } }
		public BarItemLink DeleteItemLink { get { return deleteItemLink; } set { deleteItemLink = value; } }
		public GalleryDropDown PopupGallery { get; set; }
		#endregion
		protected override void Initialize() {
			base.Initialize();
			Gallery.MinimumColumnCount = minColumnCount;
			Gallery.ColumnCount = maxColumnCount;
			this.repository = CreateRepository();
			InitizlizeMenuBarItems();
		}
		protected abstract void InitizlizeMenuBarItems();
		protected abstract RepositoryItemRichEditTableStyleEditBase CreateRepository();
		protected override void AfterLoad() {
			base.AfterLoad();
			if (Ribbon != null)
				this.Ribbon.ShowCustomizationMenu += OnShowCustomizationMenu;
			this.GalleryInitDropDownGallery += OnGalleryInitDropDownGallery;
			this.GalleryItemCheckedChanged += OnGalleryChangeStyleItemCheckedChanged;
		}
		protected override void SubscribeEvents() {
			base.SubscribeEvents();
			GalleryCustomDrawItemImage += OnCustomDrawItem;
			repository.Items.CollectionChanged += OnRepositoryItemsChanged;
		}
		protected abstract void CustomDrawItem(GalleryItemCustomDrawEventArgs e);
		protected abstract void ShowCustomizationMenu(RibbonCustomizationMenuEventArgs e);
		protected abstract void GalleryChangeStyleItemCheckedChanged(GalleryItemEventArgs e);
		protected abstract void NewItemClick();
		protected abstract void ModifyItemClick();
		protected abstract void DeleteItemClick();
		protected abstract IXtraRichEditFormatting CreateStyleFormatting();
		public void OnShowCustomizationMenu(object sender, RibbonCustomizationMenuEventArgs e) {
			ShowCustomizationMenu(e);
		}
		void OnGalleryInitDropDownGallery(object sender, InplaceGalleryEventArgs e) {
			AddItemsToDropDownGallery(e);
		}
		void OnCustomDrawItem(object sender, GalleryItemCustomDrawEventArgs e) {
			string styleName = ((StyleFormattingBase)e.Item.Tag).GetLocalizedCaption(Control.Model);
			if (styleName != e.Item.Caption)
				ActualizeGalaryItem(e.Item, styleName);
			CustomDrawItem(e);
		}
		void OnGalleryChangeStyleItemCheckedChanged(object sender, GalleryItemEventArgs e) {
			GalleryChangeStyleItemCheckedChanged(e);
		}
		protected void OnNewItemClick(object sender, ItemClickEventArgs e) {
			NewItemClick();
		}
		protected void OnModifyItemClick(object sender, ItemClickEventArgs e) {
			ModifyItemClick();
		}
		protected void OnDeleteItemClick(object sender, ItemClickEventArgs e) {
			DeleteItemClick();
		}
		protected virtual void OnMenuCloseUp(object sender, EventArgs e) {
			PopupMenu menu = (PopupMenu)sender;
			menu.RemoveLink(NewItemLink);
			NewItemLink = null;
			menu.RemoveLink(ModifyItemLink);
			ModifyItemLink = null;
			menu.RemoveLink(DeleteItemLink);
			DeleteItemLink = null;
			menu.CloseUp -= OnMenuCloseUp;
		}
		void OnRepositoryItemsChanged(object sender, CollectionChangeEventArgs e) {
			if (DesignMode)
				return;
			Gallery.BeginUpdate();
			try {
				PopulateGalleryItems();
			}
			finally {
				Gallery.EndUpdate();
			}
		}
		protected override void OnControlChanged() {
			repository.Control = Control;
		}
		protected override void OnEnabledChanged() {
			if (!Enabled) {
				GalleryItemProcessorDelegate itemsUpdateCaption = delegate(GalleryItem item) {
					item.Checked = false;
				};
				ForEachGalleryItems(itemsUpdateCaption);
			}
			base.OnEnabledChanged();
		}
		protected ICommandUIState CreateCommandUIState(Command command) {
			IXtraRichEditFormatting formatting = SelectedItem != null ? SelectedItem.Tag as IXtraRichEditFormatting : null;
			if (formatting == null)
				formatting = CreateStyleFormatting();
			DefaultValueBasedCommandUIState<IXtraRichEditFormatting> state = new DefaultValueBasedCommandUIState<IXtraRichEditFormatting>();
			state.Value = formatting;
			return state;
		}
		protected override void InvokeCommand() {
			Command command = CreateCommand();
			if (command != null) {
				ICommandUIState state = CreateCommandUIState(command);
				if (command.CanExecute())
					command.ForceExecute(state);
			}
		}
		protected override ICommandUIState CreateGalleryItemUIState() {
			return new BarGalleryItemValueUIState<IXtraRichEditFormatting>(this);
		}
		protected void PopulateGalleryItems() {
			GalleryItemCollection galleryItems = Gallery.Groups[0].Items;
			galleryItems.Clear();
			int count = repository.Items.Count;
			for (int i = 0; i < count; i++) {
				GalleryItem galleryItem = new GalleryItem();
				IXtraRichEditFormatting currenItem = repository.Items[i] as IXtraRichEditFormatting;
				string localizedCaption = currenItem.GetLocalizedCaption(Control.Model);
				galleryItem.Caption = localizedCaption;
				galleryItem.Hint = localizedCaption;
				galleryItem.Tag = currenItem;
				galleryItems.Add(galleryItem);
			}
		}
		protected virtual void AddItemsToDropDownGallery(InplaceGalleryEventArgs e) {
			DevExpress.XtraBars.Ribbon.Gallery.InDropDownGallery popupGallery = e.PopupGallery;
			PopupGallery = popupGallery.GalleryDropDown;
			BarItemLinkCollection galleryDropDownItemLinks = PopupGallery.ItemLinks;
			galleryDropDownItemLinks.Add(NewItem);
			galleryDropDownItemLinks.Add(ModifyItem);
			galleryDropDownItemLinks.Add(DeleteItem);
			DeleteItem.Enabled = true;
			ModifyItem.Enabled = true;
			popupGallery.ItemRightClick += OnPopupGalleryItemRightClick;
		}
		protected abstract PopupMenu GetMenu();
		private void OnPopupGalleryItemRightClick(object sender, GalleryItemClickEventArgs e) {
			if (CurrentItem != null)
				CurrentItem.Checked = false;
			CurrentItem = e.Item;
			CurrentItem.Checked = true;
			PopupMenu menu = GetMenu();
			menu.CloseUp += OnGalleryMenuCloseUp;
			menu.ShowPopup(Manager, RichEditControl.MousePosition, PopupGallery);
		}
		protected abstract void GalleryMenuCloseUp();
		void OnGalleryMenuCloseUp(object sender, EventArgs e) {
			GalleryMenuCloseUp();
		}
		protected BarButtonItem CreateItem(string caption, ItemClickEventHandler onItemClick) {
			BarButtonItem item = new BarButtonItem();
			item.Caption = caption;
			item.ItemClick += onItemClick;
			return item;
		}
		protected void DrawBorder(BorderBase border, Point start, Point finish, Point startBold, Point finishBold, Graphics graphics) {
			Color borderColor = border.Color;
			if (borderColor == Color.Empty)
				borderColor = Color.Black;
			if (border.Width != 0)
				graphics.DrawLine(new Pen(borderColor), start, finish);
			if (border.Width > 20) {
				graphics.DrawLine(new Pen(borderColor), startBold, finishBold);
				graphics.DrawLine(new Pen(borderColor), new Point(startBold.X - 1, startBold.Y), startBold);
				graphics.DrawLine(new Pen(borderColor), new Point(startBold.X, startBold.Y - 1), startBold);
			}
		}
		protected void DrawGalleryBackground(Color backgroundColor, Graphics graphics, Rectangle rect) {
			if (!DXColor.IsTransparentOrEmpty(backgroundColor))
				graphics.FillRectangle(new SolidBrush(backgroundColor), rect);
		}
		void ActualizeGalaryItem(GalleryItem galleryItem, string styleName) {
			galleryItem.Caption = styleName;
			galleryItem.Hint = styleName;
		}
	}
	#endregion
	#region GalleryChangeTableStyleItem
	public class GalleryChangeTableStyleItem : GalleryChangeTableStyleItemBase { 
		class BitmapKey {
			public Guid StyleId { get; set; }
			public TableLookTypes TableLook { get; set; }
			public override int GetHashCode() {
				return ((int)TableLook << 20) | StyleId.GetHashCode();
			}
			public override bool Equals(object obj) {
				BitmapKey key = obj as BitmapKey;
				if (key == null)
					return false;
				return key.TableLook == TableLook && key.StyleId == StyleId;
			}
		}
		#region Fields
		Dictionary<BitmapKey, Bitmap> imageCache = new Dictionary<BitmapKey, Bitmap>();
		TableStyle currentStyle;
		#endregion
		public GalleryChangeTableStyleItem() {
		}
		#region Properties
		public TableStyle CurrentStyle { get { return currentStyle; } set { currentStyle = value; } }
		public TableStyle CurrentItemStyle { get; set; }
		protected override RichEditCommandId CommandId { get { return RichEditCommandId.ChangeTableStyle; } }
		#endregion
		protected override RepositoryItemRichEditTableStyleEditBase CreateRepository() {
			return new RepositoryItemRichEditTableStyleEdit();
		}
		protected override IXtraRichEditFormatting CreateStyleFormatting() {
			TableStyle defaultStyle = Control.DocumentModel.TableStyles.DefaultItem;
			return new TableStyleFormatting(defaultStyle.Id);
		}
		protected override void InitizlizeMenuBarItems() {
			NewItem = CreateItem(XtraRichEditLocalizer.GetString(XtraRichEditStringId.MenuCmd_NewTableStyle), OnNewItemClick);
			NewItem.Glyph = EditStyleHelper.LoadSmallImageToGlyph("NewTableStyle");
			ModifyItem = CreateItem(XtraRichEditLocalizer.GetString(XtraRichEditStringId.MenuCmd_ModifyTableStyle), OnModifyItemClick);
			ModifyItem.Glyph = EditStyleHelper.LoadSmallImageToGlyph("ModifyTableStyle");
			DeleteItem = CreateItem(XtraRichEditLocalizer.GetString(XtraRichEditStringId.MenuCmd_DeleteTableStyle), OnDeleteItemClick);
			DeleteItem.Glyph = EditStyleHelper.LoadSmallImageToGlyph("ClearTableStyle");
		}
		protected override void AddItemsToDropDownGallery(InplaceGalleryEventArgs e) {
			base.AddItemsToDropDownGallery(e);
			if (this.Control.DocumentModel.TableStyles.DefaultItem == CurrentStyle) {
				DeleteItem.Enabled = false;
				ModifyItem.Enabled = false;
			}
		}
		protected override void OnMenuCloseUp(object sender, EventArgs e) {
			base.OnMenuCloseUp(sender, e);
			if (CurrentItemStyle != CurrentStyle)
				CurrentItem.Checked = false;
			CurrentItemStyle = null;
		}
		protected override void GalleryMenuCloseUp() {
			CurrentItemStyle = null;
		}
		protected override PopupMenu GetMenu() {
			TableStyleFormatting styleFormatting = CurrentItem.Tag as TableStyleFormatting;
			CurrentItemStyle = Control.DocumentModel.TableStyles.GetStyleById(styleFormatting.StyleId);
			PopupMenu menu = new PopupMenu();
			menu.Manager = this.Manager;
			BarItem newTableItem = CreateItem(XtraRichEditLocalizer.GetString(XtraRichEditStringId.MenuCmd_NewTableStyle), OnNewItemClick);
			newTableItem.Glyph = EditStyleHelper.LoadSmallImageToGlyph("NewTableStyle");
			menu.AddItem(newTableItem);
			BarItem modifyTableItem = CreateItem(XtraRichEditLocalizer.GetString(XtraRichEditStringId.MenuCmd_ModifyTableStyle), OnModifyItemClick);
			modifyTableItem.Glyph = EditStyleHelper.LoadSmallImageToGlyph("ModifyTableStyle");
			menu.AddItem(modifyTableItem);
			BarItem deleteTableItem = CreateItem(XtraRichEditLocalizer.GetString(XtraRichEditStringId.MenuCmd_DeleteTableStyle), OnDeleteItemClick);
			deleteTableItem.Glyph = EditStyleHelper.LoadSmallImageToGlyph("ClearTableStyle");
			menu.AddItem(deleteTableItem);
			if (this.Control.DocumentModel.TableStyles.DefaultItem == CurrentItemStyle) {
				modifyTableItem.Enabled = false;
				deleteTableItem.Enabled = false;
			}
			return menu;
		}
		protected override void GalleryChangeStyleItemCheckedChanged(GalleryItemEventArgs e) {
			if (Control == null || Control.IsDisposed)
				return;
			GalleryItem currentItem = e.Item;
			TableStyleFormatting styleFormatting = currentItem.Tag as TableStyleFormatting;
			if (styleFormatting != null && Control.DocumentModel != null) {				
				TableStyle tableStyle = Control.DocumentModel.TableStyles.GetStyleById(styleFormatting.StyleId);
				if (currentItem.Checked && tableStyle != null && CurrentItemStyle != tableStyle)
					CurrentItemStyle = tableStyle;
			}
		}
		protected override void ShowCustomizationMenu(RibbonCustomizationMenuEventArgs e) {
			if (e.HitInfo != null)
				CurrentItem = e.HitInfo.GalleryItem;
			if (CurrentItem == null || e.Link.Item != this)
				return;
			CurrentItem.Checked = true;
			DeleteItem.Enabled = true;
			ModifyItem.Enabled = true;
			TableStyleFormatting styleFormatting = (TableStyleFormatting)CurrentItem.Tag;
			CurrentItemStyle = Control.DocumentModel.TableStyles.GetStyleById(styleFormatting.StyleId);
			DevExpress.XtraBars.Ribbon.Internal.RibbonCustomizationPopupMenu menu = e.CustomizationMenu;
			menu.CloseUp += OnMenuCloseUp;
			if (NewItemLink == null)
				NewItemLink = menu.InsertItem(menu.ItemLinks[0], NewItem);
			if (ModifyItemLink == null)
				ModifyItemLink = menu.InsertItem(menu.ItemLinks[1], ModifyItem);
			if (DeleteItemLink == null)
				DeleteItemLink = menu.InsertItem(menu.ItemLinks[2], DeleteItem);
			if (menu.ItemLinks[3] != null)
				menu.ItemLinks[3].BeginGroup = true;
			if (this.Control.DocumentModel.TableStyles.DefaultItem == CurrentItemStyle) {
				DeleteItem.Enabled = false;
				ModifyItem.Enabled = false;
			}
		}
		private void ShowTableForm(DocumentModel documentModel, TableStyle newStyle) {
			Control.ShowTableStyleForm(newStyle);
		}
		protected override void NewItemClick() {
			DocumentModel documentModel = this.Control.DocumentModel;
			TableStyleCollection styles = documentModel.TableStyles;
			TableStyle newStyle = new TableStyle(documentModel);
			newStyle.StyleName = "Style";
			newStyle.Parent = styles.DefaultItem;
			for (int i = 1; i <= styles.Count; i++) {
				TableStyle style = styles.GetStyleByName(newStyle.StyleName + i);
				if (style == null) {
					newStyle.StyleName += i;
					break;
				}
			}
			ShowTableForm(documentModel, newStyle);
		}
		protected override void ModifyItemClick() {
			ShowTableStyleFormCommand command = new ShowTableStyleFormCommand(this.Control, CurrentItemStyle);
			command.Execute();
		}
		protected override void DeleteItemClick() {
			string styleName = CurrentStyle.StyleName;
			if (CurrentItemStyle != null)
				styleName = CurrentItemStyle.StyleName;
			string questionString = String.Format(XtraRichEditLocalizer.GetString(XtraRichEditStringId.Msg_DeleteTableStyleQuestion), styleName);
			DialogResult isDelete = XtraMessageBox.Show(questionString, Application.ProductName, MessageBoxButtons.YesNo, MessageBoxIcon.Question);
			if (isDelete == DialogResult.Yes) {
				TableStyleCollection tableStyles = this.Control.DocumentModel.TableStyles;
				if (CurrentItemStyle != null)
					tableStyles.Delete(CurrentItemStyle);
				else
					tableStyles.Delete(CurrentStyle);
			}
		}
		protected void DrawTableStyleItem(GalleryItemCustomDrawEventArgs e, TableStyleFormatting currentStyleFormatting) {
			GraphicsCache cache = e.Cache;
			Rectangle itemBounds = e.Bounds;
			DocumentModelPosition start = this.RichEditControl.DocumentModel.Selection.Interval.Start;
			DocumentModelPosition end = this.RichEditControl.DocumentModel.Selection.Interval.End;
			TableCell startCell = this.RichEditControl.DocumentModel.ActivePieceTable.Paragraphs[start.ParagraphIndex].GetCell();
			TableCell endCell = this.RichEditControl.DocumentModel.ActivePieceTable.Paragraphs[end.ParagraphIndex].GetCell();
			Table sourceTable = null;
			if (startCell != null && endCell != null && startCell.Table == endCell.Table)
				sourceTable = startCell.Table;
			BitmapKey key = new BitmapKey();
			key.StyleId = currentStyleFormatting.StyleId;
			key.TableLook = sourceTable != null ? sourceTable.TableLook : TableLookTypes.None;
			Bitmap bitmap;
			if (!this.imageCache.TryGetValue(key, out bitmap)) {
				bitmap = CreateBitmap(sourceTable, currentStyleFormatting, itemBounds.Width, itemBounds.Height);
				this.imageCache.Add(key, bitmap);
			}
			cache.Graphics.DrawImage(bitmap, itemBounds);
			e.Handled = true;
		}
		Bitmap CreateBitmap(Table sourceTable, TableStyleFormatting currentStyleFormatting, int width, int height) {
			DocumentModel documentModel = new DocumentModel(Control.DocumentModel.DocumentFormatsDependencies);
			PieceTable pieceTable = documentModel.ActivePieceTable;
			TableStyle style = Control.DocumentModel.TableStyles.GetStyleById(currentStyleFormatting.StyleId);
			pieceTable.InsertTable(pieceTable.DocumentStartLogPosition, 5, 5);
			while (documentModel.TableStyles.Count > 0) {
				documentModel.TableStyles.RemoveLastStyle();
			}
			documentModel.BeginUpdate();
			try {
				style.Copy(documentModel);
			}
			finally {
				documentModel.EndUpdate();
			}
			int styleIndex = documentModel.TableStyles.GetStyleIndexByName(style.StyleName);
			documentModel.TableStyles[styleIndex].ConditionalStyleProperties.CopyFrom(style.ConditionalStyleProperties);
			Rectangle rect = new Rectangle(0, 0, width, height);
			Table tempTable = pieceTable.Tables[0];
			tempTable.StyleIndex = styleIndex;
			if (sourceTable != null) {
				tempTable.TableLook = sourceTable.TableLook;
				currentStyle = sourceTable.TableStyle;
			}
			Bitmap result = new Bitmap(rect.Width, rect.Height);
			using (Graphics graphics = Graphics.FromImage(result)) {
				DrawGalleryBackground(Color.FromArgb(255, 255, 255), graphics, rect);
				DrawStyleBackground(graphics, tempTable, 2, 2);
				DrawBorders(graphics, tempTable, 2, 2);
			}
			return result;
		}
		protected override void CustomDrawItem(GalleryItemCustomDrawEventArgs e) {
			if (Control == null || Control.IsDisposed)
				return;
			TableStyleFormatting currentStyleFormatting = e.Item.Tag as TableStyleFormatting;
			if (currentStyleFormatting != null)
				DrawTableStyleItem(e, currentStyleFormatting);
		}
		protected void DrawStyleBackgroundCore(TableCell cell, Graphics graphics, int leftImageLocation, int topImageLocation) {
			const int cellWidth = 12;
			const int cellHeight = 8;
			int leftCellLocation = leftImageLocation + cellWidth * cell.IndexInRow;
			int topCellLocation = topImageLocation + cellHeight * cell.RowIndex;
			using (Brush brush = new SolidBrush(cell.BackgroundColor)) {
				graphics.FillRectangle(brush, leftCellLocation, topCellLocation, cellWidth, cellHeight);
			}
		}
		protected void DrawStyleBackground(Graphics graphics, Table table, int leftImageLocation, int topImageLocation) {
			TableCellProcessorDelegate setBackground = delegate(TableCell cell) {
				DrawStyleBackgroundCore(cell, graphics, leftImageLocation, topImageLocation);
			};
			table.ForEachCell(setBackground);
		}
		protected void DrawCharacterLine(TableCell cell, int horizontalCellLocation, int verticalCellLocation, Graphics graphics) {
			PieceTable pieceTable = cell.DocumentModel.ActivePieceTable;
			ParagraphIndex paragraphIndex = cell.StartParagraphIndex;
			RunIndex runIndex = pieceTable.Paragraphs[paragraphIndex].FirstRunIndex;
			Color color = pieceTable.Runs[runIndex].ForeColor;
			if (color == Color.Empty)
				color = Color.Black;
			graphics.DrawLine(new Pen(color), horizontalCellLocation + 4, verticalCellLocation + 5, horizontalCellLocation + 9, verticalCellLocation + 5);
		}
		protected void DrawBordersCore(TableCell cell, Graphics graphics, int leftImageLocation, int topImageLocation) {
			const int cellWidth = 12;
			const int cellHeight = 8;
			int leftCellLocation = leftImageLocation + cellWidth * cell.IndexInRow;
			int topCellLocation = topImageLocation + cellHeight * cell.RowIndex;
			int rightCellLocation = leftCellLocation + cellWidth;
			int bottomCellLocation = topCellLocation + cellHeight;
			Point leftTopCell = new Point(leftCellLocation, topCellLocation);
			Point leftBottomCell = new Point(leftCellLocation, bottomCellLocation);
			Point rightTopCell = new Point(rightCellLocation, topCellLocation);
			Point rightBottomCell = new Point(rightCellLocation, bottomCellLocation);
			Point leftTopCellBold = new Point(leftCellLocation + 1, topCellLocation + 1);
			Point leftBottomCellBold = new Point(leftCellLocation + 1, bottomCellLocation + 1);
			Point rightTopCellBold = new Point(rightCellLocation + 1, topCellLocation + 1);
			Point rightBottomCellBold = new Point(rightCellLocation + 1, bottomCellLocation + 1);
			DrawBorder(cell.GetActualLeftCellBorder(), leftTopCell, leftBottomCell, leftTopCellBold, leftBottomCellBold, graphics);
			DrawBorder(cell.GetActualRightCellBorder(), rightTopCell, rightBottomCell, rightTopCellBold, rightBottomCellBold, graphics);
			DrawBorder(cell.GetActualTopCellBorder(), leftTopCell, rightTopCell, leftTopCellBold, rightTopCellBold, graphics);
			DrawBorder(cell.GetActualBottomCellBorder(), leftBottomCell, rightBottomCell, leftBottomCellBold, rightBottomCellBold, graphics);
			DrawCharacterLine(cell, leftCellLocation, topCellLocation, graphics);
		}
		protected void DrawBorders(Graphics graphics, Table table, int leftImageLocation, int topImageLocation) {
			TableCellProcessorDelegate setBorders = delegate(TableCell cell) {
				DrawBordersCore(cell, graphics, leftImageLocation, topImageLocation);
			};
			table.ForEachCell(setBorders);
		}
		protected override void Dispose(bool disposing) {
			if (disposing) {
				if (this.imageCache != null) {
					foreach (Bitmap bitmap in this.imageCache.Values)
						bitmap.Dispose();
					this.imageCache.Clear();
					this.imageCache = null;
				}
			}
			base.Dispose(disposing);
		}
	}
	#endregion
	#region ChangeTableCellsShadingItem
	public class ChangeTableCellsShadingItem : ChangeColorItemBase<RichEditControl, RichEditCommandId> {
		public ChangeTableCellsShadingItem() {
		}
		public ChangeTableCellsShadingItem(BarManager manager)
			: base(manager) {
		}
		public ChangeTableCellsShadingItem(string caption)
			: base(caption) {
		}
		public ChangeTableCellsShadingItem(BarManager manager, string caption)
			: base(manager, caption) {
		}
		protected override RichEditCommandId CommandId { get { return RichEditCommandId.None; } }
		protected override RibbonItemStyles DefaultRibbonStyle { get { return RibbonItemStyles.All; } }
		protected override string DefaultColorButtonCaption { get { return XtraRichEditLocalizer.GetString(XtraRichEditStringId.Caption_NoColor); } }
		#region Properties
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never)]
		public RichEditControl RichEditControl { get { return this.Control; } set { this.Control = value; } }
		#endregion
		protected override Command CreateCommand() {
			if (Control == null)
				return null;
			Command command = Control.CreateCommand(RichEditCommandId.ChangeTableCellsShading);
			if (command != null) {
				command.CommandSourceType = CommandSourceType.Menu;
				return command;
			}
			return null;
		}
	}
	#endregion
	#region ChangeTableBordersItem
	public class ChangeTableBordersItem : RichEditCommandBarSubItem {
		public ChangeTableBordersItem() {
		}
		public ChangeTableBordersItem(BarManager manager)
			: base(manager) {
		}
		public ChangeTableBordersItem(string caption)
			: base(caption) {
		}
		public ChangeTableBordersItem(BarManager manager, string caption)
			: base(manager, caption) {
		}
		protected override RichEditCommandId CommandId { get { return RichEditCommandId.ChangeTableBordersPlaceholder; } }
	}
	#endregion
	#region ToggleTableCellsOutsideBorderItem
	public class ToggleTableCellsOutsideBorderItem : RichEditCommandBarCheckItem {
		public ToggleTableCellsOutsideBorderItem() {
		}
		public ToggleTableCellsOutsideBorderItem(BarManager manager)
			: base(manager) {
		}
		public ToggleTableCellsOutsideBorderItem(string caption)
			: base(caption) {
		}
		public ToggleTableCellsOutsideBorderItem(BarManager manager, string caption)
			: base(manager, caption) {
		}
		protected override RichEditCommandId CommandId { get { return RichEditCommandId.ToggleTableCellsOutsideBorder; } }
	}
	#endregion
	#region ToggleTableCellsInsideBorderItem
	public class ToggleTableCellsInsideBorderItem : RichEditCommandBarCheckItem {
		public ToggleTableCellsInsideBorderItem() {
		}
		public ToggleTableCellsInsideBorderItem(BarManager manager)
			: base(manager) {
		}
		public ToggleTableCellsInsideBorderItem(string caption)
			: base(caption) {
		}
		public ToggleTableCellsInsideBorderItem(BarManager manager, string caption)
			: base(manager, caption) {
		}
		protected override RichEditCommandId CommandId { get { return RichEditCommandId.ToggleTableCellsInsideBorder; } }
	}
	#endregion
	#region ToggleTableCellsAllBordersItem
	public class ToggleTableCellsAllBordersItem : RichEditCommandBarCheckItem {
		public ToggleTableCellsAllBordersItem() {
		}
		public ToggleTableCellsAllBordersItem(BarManager manager)
			: base(manager) {
		}
		public ToggleTableCellsAllBordersItem(string caption)
			: base(caption) {
		}
		public ToggleTableCellsAllBordersItem(BarManager manager, string caption)
			: base(manager, caption) {
		}
		protected override RichEditCommandId CommandId { get { return RichEditCommandId.ToggleTableCellsAllBorders; } }
	}
	#endregion
	#region ResetTableCellsAllBordersItem
	public class ResetTableCellsAllBordersItem : RichEditCommandBarButtonItem {
		public ResetTableCellsAllBordersItem() {
		}
		public ResetTableCellsAllBordersItem(BarManager manager)
			: base(manager) {
		}
		public ResetTableCellsAllBordersItem(string caption)
			: base(caption) {
		}
		public ResetTableCellsAllBordersItem(BarManager manager, string caption)
			: base(manager, caption) {
		}
		protected override RichEditCommandId CommandId { get { return RichEditCommandId.ResetTableCellsAllBorders; } }
	}
	#endregion
	#region ToggleTableCellsLeftBorderItem
	public class ToggleTableCellsLeftBorderItem : RichEditCommandBarCheckItem {
		public ToggleTableCellsLeftBorderItem() {
		}
		public ToggleTableCellsLeftBorderItem(BarManager manager)
			: base(manager) {
		}
		public ToggleTableCellsLeftBorderItem(string caption)
			: base(caption) {
		}
		public ToggleTableCellsLeftBorderItem(BarManager manager, string caption)
			: base(manager, caption) {
		}
		protected override RichEditCommandId CommandId { get { return RichEditCommandId.ToggleTableCellsLeftBorder; } }
	}
	#endregion
	#region ToggleTableCellsRightBorderItem
	public class ToggleTableCellsRightBorderItem : RichEditCommandBarCheckItem {
		public ToggleTableCellsRightBorderItem() {
		}
		public ToggleTableCellsRightBorderItem(BarManager manager)
			: base(manager) {
		}
		public ToggleTableCellsRightBorderItem(string caption)
			: base(caption) {
		}
		public ToggleTableCellsRightBorderItem(BarManager manager, string caption)
			: base(manager, caption) {
		}
		protected override RichEditCommandId CommandId { get { return RichEditCommandId.ToggleTableCellsRightBorder; } }
	}
	#endregion
	#region ToggleTableCellsTopBorderItem
	public class ToggleTableCellsTopBorderItem : RichEditCommandBarCheckItem {
		public ToggleTableCellsTopBorderItem() {
		}
		public ToggleTableCellsTopBorderItem(BarManager manager)
			: base(manager) {
		}
		public ToggleTableCellsTopBorderItem(string caption)
			: base(caption) {
		}
		public ToggleTableCellsTopBorderItem(BarManager manager, string caption)
			: base(manager, caption) {
		}
		protected override RichEditCommandId CommandId { get { return RichEditCommandId.ToggleTableCellsTopBorder; } }
	}
	#endregion
	#region ToggleTableCellsBottomBorderItem
	public class ToggleTableCellsBottomBorderItem : RichEditCommandBarCheckItem {
		public ToggleTableCellsBottomBorderItem() {
		}
		public ToggleTableCellsBottomBorderItem(BarManager manager)
			: base(manager) {
		}
		public ToggleTableCellsBottomBorderItem(string caption)
			: base(caption) {
		}
		public ToggleTableCellsBottomBorderItem(BarManager manager, string caption)
			: base(manager, caption) {
		}
		protected override RichEditCommandId CommandId { get { return RichEditCommandId.ToggleTableCellsBottomBorder; } }
	}
	#endregion
	#region ToggleTableCellsInsideHorizontalBorderItem
	public class ToggleTableCellsInsideHorizontalBorderItem : RichEditCommandBarCheckItem {
		public ToggleTableCellsInsideHorizontalBorderItem() {
		}
		public ToggleTableCellsInsideHorizontalBorderItem(BarManager manager)
			: base(manager) {
		}
		public ToggleTableCellsInsideHorizontalBorderItem(string caption)
			: base(caption) {
		}
		public ToggleTableCellsInsideHorizontalBorderItem(BarManager manager, string caption)
			: base(manager, caption) {
		}
		protected override RichEditCommandId CommandId { get { return RichEditCommandId.ToggleTableCellsInsideHorizontalBorder; } }
	}
	#endregion
	#region ToggleTableCellsInsideVerticalBorderItem
	public class ToggleTableCellsInsideVerticalBorderItem : RichEditCommandBarCheckItem {
		public ToggleTableCellsInsideVerticalBorderItem() {
		}
		public ToggleTableCellsInsideVerticalBorderItem(BarManager manager)
			: base(manager) {
		}
		public ToggleTableCellsInsideVerticalBorderItem(string caption)
			: base(caption) {
		}
		public ToggleTableCellsInsideVerticalBorderItem(BarManager manager, string caption)
			: base(manager, caption) {
		}
		protected override RichEditCommandId CommandId { get { return RichEditCommandId.ToggleTableCellsInsideVerticalBorder; } }
	}
	#endregion
	#region RichEditTableDrawBordersItemBuilder
	public class RichEditTableDrawBordersItemBuilder : CommandBasedBarItemBuilder {
		public override void PopulateItems(List<BarItem> items, BarCreationContextBase creationContext) {
			items.Add(new ChangeTableBorderLineStyleItem());
			items.Add(new ChangeTableBorderLineWeightItem());
			items.Add(new ChangeTableBorderColorItem());
			ChangeTableBordersItem bordersItem = new ChangeTableBordersItem();
			items.Add(bordersItem);
			IBarSubItem bordersSubItem = bordersItem;
			bordersSubItem.AddBarItem(new ToggleTableCellsBottomBorderItem());
			bordersSubItem.AddBarItem(new ToggleTableCellsTopBorderItem());
			bordersSubItem.AddBarItem(new ToggleTableCellsLeftBorderItem());
			bordersSubItem.AddBarItem(new ToggleTableCellsRightBorderItem());
			bordersSubItem.AddBarItem(new ResetTableCellsAllBordersItem());
			bordersSubItem.AddBarItem(new ToggleTableCellsAllBordersItem());
			bordersSubItem.AddBarItem(new ToggleTableCellsOutsideBorderItem());
			bordersSubItem.AddBarItem(new ToggleTableCellsInsideBorderItem());
			bordersSubItem.AddBarItem(new ToggleTableCellsInsideHorizontalBorderItem());
			bordersSubItem.AddBarItem(new ToggleTableCellsInsideVerticalBorderItem());
			bordersSubItem.AddBarItem(new ToggleShowTableGridLinesItem());
			items.Add(new ChangeTableCellsShadingItem());
		}
	}
	#endregion
	#region ChangeTableBorderLineStyleItem
	public class ChangeTableBorderLineStyleItem : RichEditCommandBarEditItem<BorderInfo> {
		const int defaultWidth = 130;
		public ChangeTableBorderLineStyleItem() {
			Width = defaultWidth;
		}
		public ChangeTableBorderLineStyleItem(BarManager manager, string caption)
			: base(manager, caption) {
			Width = defaultWidth;
		}
		[DefaultValue(defaultWidth)]
		public override int Width { get { return base.Width; } set { base.Width = value; } }
		protected override RichEditCommandId CommandId { get { return RichEditCommandId.ChangeTableCellsBorderLineStyle; } }
		protected override ICommandUIState CreateCommandUIState(Command command) {
			BorderInfo borderInfo = EditValue as BorderInfo;
			if (borderInfo == null)
				borderInfo = Control.DocumentModel.TableBorderInfoRepository.CurrentItem;
			DefaultValueBasedCommandUIState<BorderInfo> state = new DefaultValueBasedCommandUIState<BorderInfo>();
			state.Value = borderInfo;
			return state;
		}
		protected override RepositoryItem CreateEdit() {
			RepositoryItemBorderLineStyle edit = new RepositoryItemBorderLineStyle();
			if (RichEditControl != null)
				edit.Control = Control;
			return edit;
		}
		protected override void OnControlChanged() {
			RepositoryItemBorderLineStyle edit = (RepositoryItemBorderLineStyle)Edit;
			if (edit != null) {
				edit.Control = Control;
				if (Control == null)
					EditValue = null;
				else
					EditValue = Control.DocumentModel.TableBorderInfoRepository.GetItemByLineStyle(BorderLineStyle.Single);
			}
		}
	}
	#endregion
	#region ChangeTableBorderLineWeightItem
	public class ChangeTableBorderLineWeightItem : RichEditCommandBarEditItem<int> {
		const int defaultWidth = 130;
		public ChangeTableBorderLineWeightItem() {
			Width = defaultWidth;
		}
		public ChangeTableBorderLineWeightItem(BarManager manager, string caption)
			: base(manager, caption) {
			Width = defaultWidth;
		}
		[DefaultValue(defaultWidth)]
		public override int Width { get { return base.Width; } set { base.Width = value; } }
		protected override RichEditCommandId CommandId { get { return RichEditCommandId.ChangeTableCellsBorderLineWeight; } }
		protected override ICommandUIState CreateCommandUIState(Command command) {
			int weight;
			if (EditValue is int)
				weight = (int)EditValue;
			else
				weight = Math.Max(1, Control.DocumentModel.UnitConverter.PointsToModelUnits(1));
			DefaultValueBasedCommandUIState<int> state = new DefaultValueBasedCommandUIState<int>();
			state.Value = weight;
			return state;
		}
		protected override RepositoryItem CreateEdit() {
			RepositoryItemBorderLineWeight edit = new RepositoryItemBorderLineWeight();
			if (RichEditControl != null)
				edit.Control = Control;
			return edit;
		}
		protected override void OnControlChanged() {
			RepositoryItemBorderLineWeight edit = (RepositoryItemBorderLineWeight)Edit;
			if (edit != null) {
				edit.Control = Control;
				if (Control == null)
					EditValue = null;
				else
					EditValue = Math.Max(1, Control.DocumentModel.UnitConverter.PointsToModelUnits(1));
			}
		}
	}
	#endregion
	#region ChangeTableBorderColorItem
	public class ChangeTableBorderColorItem : ChangeColorItemBase<RichEditControl, RichEditCommandId> {
		public ChangeTableBorderColorItem() {
		}
		public ChangeTableBorderColorItem(BarManager manager)
			: base(manager) {
		}
		public ChangeTableBorderColorItem(string caption)
			: base(caption) {
		}
		public ChangeTableBorderColorItem(BarManager manager, string caption)
			: base(manager, caption) {
		}
		protected override RichEditCommandId CommandId { get { return RichEditCommandId.None; } }
		protected override RibbonItemStyles DefaultRibbonStyle { get { return RibbonItemStyles.All; } }
		protected override string DefaultColorButtonCaption { get { return XtraRichEditLocalizer.GetString(XtraRichEditStringId.Caption_ColorAutomatic); } }
		#region Properties
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never)]
		public RichEditControl RichEditControl { get { return this.Control; } set { this.Control = value; } }
		#endregion
		protected override Command CreateCommand() {
			if (Control == null)
				return null;
			Command command = Control.CreateCommand(RichEditCommandId.ChangeTableCellsBorderColor);
			if (command != null) {
				command.CommandSourceType = CommandSourceType.Menu;
				return command;
			}
			return null;
		}
	}
	#endregion
	#region RichEditTableStyleOptionsBarCreator
	public class RichEditTableStyleOptionsBarCreator : ControlCommandBarCreator {
		public override Type SupportedRibbonPageType { get { return typeof(TableDesignRibbonPage); } }
		public override Type SupportedRibbonPageGroupType { get { return typeof(TableStyleOptionsRibbonPageGroup); } }
		public override Type SupportedRibbonPageCategoryType { get { return typeof(TableToolsRibbonPageCategory); } }
		public override Type SupportedBarType { get { return typeof(TableStyleOptionsBar); } }
		public override int DockRow { get { return 3; } }
		public override int DockColumn { get { return 5; } }
		public override Bar CreateBar() {
			return new TableStyleOptionsBar();
		}
		public override CommandBasedBarItemBuilder CreateBarItemBuilder() {
			return new RichEditTableStyleOptionsItemBuilder();
		}
		public override CommandBasedRibbonPageCategory CreateRibbonPageCategoryInstance() {
			return new TableToolsRibbonPageCategory();
		}
		public override CommandBasedRibbonPage CreateRibbonPageInstance() {
			return new TableDesignRibbonPage();
		}
		public override CommandBasedRibbonPageGroup CreateRibbonPageGroupInstance() {
			return new TableStyleOptionsRibbonPageGroup();
		}
	}
	#endregion
	#region RichEditTableStylesBarCreator
	public class RichEditTableStylesBarCreator : ControlCommandBarCreator {
		public override Type SupportedRibbonPageType { get { return typeof(TableDesignRibbonPage); } }
		public override Type SupportedRibbonPageGroupType { get { return typeof(TableStylesRibbonPageGroup); } }
		public override Type SupportedRibbonPageCategoryType { get { return typeof(TableToolsRibbonPageCategory); } }
		public override Type SupportedBarType { get { return typeof(TableStylesBar); } }
		public override int DockRow { get { return 3; } }
		public override int DockColumn { get { return 6; } }
		public override Bar CreateBar() {
			return new TableStylesBar();
		}
		public override CommandBasedBarItemBuilder CreateBarItemBuilder() {
			return new RichEditTableStylesItemBuilder();
		}
		public override CommandBasedRibbonPageCategory CreateRibbonPageCategoryInstance() {
			return new TableToolsRibbonPageCategory();
		}
		public override CommandBasedRibbonPage CreateRibbonPageInstance() {
			return new TableDesignRibbonPage();
		}
		public override CommandBasedRibbonPageGroup CreateRibbonPageGroupInstance() {
			return new TableStylesRibbonPageGroup();
		}
	}
	#endregion
	#region RichEditTableDrawBordersBarCreator
	public class RichEditTableDrawBordersBarCreator : ControlCommandBarCreator {
		public override Type SupportedRibbonPageType { get { return typeof(TableDesignRibbonPage); } }
		public override Type SupportedRibbonPageGroupType { get { return typeof(TableDrawBordersRibbonPageGroup); } }
		public override Type SupportedRibbonPageCategoryType { get { return typeof(TableToolsRibbonPageCategory); } }
		public override Type SupportedBarType { get { return typeof(TableDrawBordersBar); } }
		public override int DockRow { get { return 3; } }
		public override int DockColumn { get { return 7; } }
		public override Bar CreateBar() {
			return new TableDrawBordersBar();
		}
		public override CommandBasedBarItemBuilder CreateBarItemBuilder() {
			return new RichEditTableDrawBordersItemBuilder();
		}
		public override CommandBasedRibbonPageCategory CreateRibbonPageCategoryInstance() {
			return new TableToolsRibbonPageCategory();
		}
		public override CommandBasedRibbonPage CreateRibbonPageInstance() {
			return new TableDesignRibbonPage();
		}
		public override CommandBasedRibbonPageGroup CreateRibbonPageGroupInstance() {
			return new TableDrawBordersRibbonPageGroup();
		}
	}
	#endregion
	#region TableStyleOptionsBar
	public class TableStyleOptionsBar : ControlCommandBasedBar<RichEditControl, RichEditCommandId> {
		public TableStyleOptionsBar()
			: base() {
		}
		public TableStyleOptionsBar(BarManager manager)
			: base(manager) {
		}
		public TableStyleOptionsBar(BarManager manager, string name)
			: base(manager, name) {
		}
		public override string DefaultText { get { return XtraRichEditLocalizer.GetString(XtraRichEditStringId.Caption_GroupTableStyleOptions); } }
	}
	#endregion
	#region TableStylesBar
	public class TableStylesBar : ControlCommandBasedBar<RichEditControl, RichEditCommandId> {
		public TableStylesBar()
			: base() {
		}
		public TableStylesBar(BarManager manager)
			: base(manager) {
		}
		public TableStylesBar(BarManager manager, string name)
			: base(manager, name) {
		}
		public override string DefaultText { get { return XtraRichEditLocalizer.GetString(XtraRichEditStringId.Caption_GroupTableStyles); } }
	}
	#endregion
	#region TableDrawBordersBar
	public class TableDrawBordersBar : ControlCommandBasedBar<RichEditControl, RichEditCommandId> {
		public TableDrawBordersBar()
			: base() {
		}
		public TableDrawBordersBar(BarManager manager)
			: base(manager) {
		}
		public TableDrawBordersBar(BarManager manager, string name)
			: base(manager, name) {
		}
		public override string DefaultText { get { return XtraRichEditLocalizer.GetString(XtraRichEditStringId.Caption_GroupTableDrawBorders); } }
	}
	#endregion
	#region TableDesignRibbonPage
	public class TableDesignRibbonPage : ControlCommandBasedRibbonPage {
		public TableDesignRibbonPage() {
		}
		public TableDesignRibbonPage(string text)
			: base(text) {
		}
		public override string DefaultText { get { return XtraRichEditLocalizer.GetString(XtraRichEditStringId.Caption_PageTableDesign); } }
		protected override RibbonPage CreatePage() {
			return new TableDesignRibbonPage();
		}
	}
	#endregion
	#region TableStyleOptionsRibbonPageGroup
	public class TableStyleOptionsRibbonPageGroup : RichEditControlRibbonPageGroup {
		public TableStyleOptionsRibbonPageGroup() {
		}
		public TableStyleOptionsRibbonPageGroup(string text)
			: base(text) {
		}
		public override string DefaultText { get { return XtraRichEditLocalizer.GetString(XtraRichEditStringId.Caption_GroupTableStyleOptions); } }
	}
	#endregion
	#region TableStylesRibbonPageGroup
	public class TableStylesRibbonPageGroup : RichEditControlRibbonPageGroup {
		public TableStylesRibbonPageGroup() {
		}
		public TableStylesRibbonPageGroup(string text)
			: base(text) {
		}
		public override string DefaultText { get { return XtraRichEditLocalizer.GetString(XtraRichEditStringId.Caption_GroupTableStyles); } }
	}
	#endregion
	#region TableDrawBordersRibbonPageGroup
	public class TableDrawBordersRibbonPageGroup : RichEditControlRibbonPageGroup {
		public TableDrawBordersRibbonPageGroup() {
		}
		public TableDrawBordersRibbonPageGroup(string text)
			: base(text) {
		}
		public override string DefaultText { get { return XtraRichEditLocalizer.GetString(XtraRichEditStringId.Caption_GroupTableDrawBorders); } }
	}
	#endregion
}
