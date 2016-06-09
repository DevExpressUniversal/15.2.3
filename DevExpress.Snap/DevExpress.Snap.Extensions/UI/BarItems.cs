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
using System.ComponentModel;
using System.Drawing;
using DevExpress.Snap.Core.Commands;
using DevExpress.Snap.Localization;
using DevExpress.Snap.Core.Native.Services;
using DevExpress.Snap.Design;
using DevExpress.Snap.Extensions.Localization;
using DevExpress.Utils.Commands;
using DevExpress.XtraBars;
using DevExpress.XtraBars.Commands;
using DevExpress.XtraBars.Ribbon;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraRichEdit;
using DevExpress.XtraRichEdit.Commands;
using DevExpress.XtraRichEdit.Fields;
using DevExpress.XtraRichEdit.Model;
using DevExpress.XtraRichEdit.UI;
using System.Collections.Generic;
using DevExpress.Utils.Drawing;
using DevExpress.XtraRichEdit.Localization;
using DevExpress.XtraRichEdit.Design;
using DevExpress.Utils;
using DevExpress.XtraBars.Ribbon.ViewInfo;
using DevExpress.XtraBars.Commands.Internal;
using System.Windows.Forms;
using DevExpress.Snap.Core.Native;
using DevExpress.Snap.Core.Forms;
using DevExpress.Snap.Forms;
using DevExpress.XtraRichEdit.Forms;
using DevExpress.XtraBars.Ribbon.Gallery;
using DevExpress.Snap.Core;
namespace DevExpress.Snap.Extensions.UI {
	public enum SnapCommand {
		SortFieldAscending, SortFieldDescending, GroupByField, InsertListHeader, InsertListFooter,
		SummaryCount, SummaryAverage, SummaryMax, SummaryMin, SummarySum,
		RemoveListHeader, RemoveListFooter,
		InsertGroupHeader, InsertGroupFooter,
		RemoveGroupHeader, RemoveGroupFooter,
		ExportDocument,
		HighlightActiveElement,
		ShowReportStructureEditorForm,
		InsertPageBreakGroupSeparator, InsertNoneGroupSeparator,
		InsertEmptyParagraphGroupSeparator, InsertEmptyRowGroupSeparator,
		InsertSectionBreakNextPageGroupSeparator, InsertSectionBreakEvenPageGroupSeparator,
		InsertSectionBreakOddPageGroupSeparator, InsertPageBreakDataRowSeparator,
		InsertNoneDataRowSeparator, InsertEmptyParagraphDataRowSeparator,
		InsertEmptyRowDataRowSeparator, InsertSectionBreakNextPageDataRowSeparator,
		InsertSectionBreakEvenPageDataRowSeparator, InsertSectionBreakOddPageDataRowSeparator,
		GroupHeader, GroupFooter, ListHeader, ListFooter, SummaryByField,
		InsertGroupSeparator, InsertDataRowSeparator,
		ConvertToParagraphs, InsertBarCode, InsertCheckBox, InsertSparkline,
		InsertChart, InsertIndex, GroupFieldsCollection, DeleteList,
		FilterList,  NewDataSource,
		SaveTheme, LoadTheme,
		MailMergeFilters, MailMergeSorting, SnapMailMergeExportDocument, SnapMailMergePrint, SnapMailMergePrintPreview, FinishAndMerge,
		SnapFilterField, SnapSortFieldAscending, SnapSortFieldDescending
	}
	public class CommandContainer {
		static Dictionary<SnapCommand, RichEditCommandId> dictionary = new Dictionary<SnapCommand, RichEditCommandId>();
		static CommandContainer() {
			dictionary.Add(SnapCommand.SortFieldAscending, SnapCommandId.SortFieldAscending);
			dictionary.Add(SnapCommand.SortFieldDescending, SnapCommandId.SortFieldDescending);
			dictionary.Add(SnapCommand.GroupByField, SnapCommandId.GroupByField);
			dictionary.Add(SnapCommand.InsertListHeader, SnapCommandId.InsertListHeader);
			dictionary.Add(SnapCommand.InsertListFooter, SnapCommandId.InsertListFooter);
			dictionary.Add(SnapCommand.SummaryCount, SnapCommandId.SummaryCount);
			dictionary.Add(SnapCommand.SummaryAverage, SnapCommandId.SummaryAverage);
			dictionary.Add(SnapCommand.SummaryMax, SnapCommandId.SummaryMax);
			dictionary.Add(SnapCommand.SummaryMin, SnapCommandId.SummaryMin);
			dictionary.Add(SnapCommand.SummarySum, SnapCommandId.SummarySum);
			dictionary.Add(SnapCommand.RemoveListHeader, SnapCommandId.RemoveListHeader);
			dictionary.Add(SnapCommand.RemoveListFooter, SnapCommandId.RemoveListFooter);
			dictionary.Add(SnapCommand.InsertGroupHeader, SnapCommandId.InsertGroupHeader);
			dictionary.Add(SnapCommand.InsertGroupFooter, SnapCommandId.InsertGroupFooter);
			dictionary.Add(SnapCommand.RemoveGroupHeader, SnapCommandId.RemoveGroupHeader);
			dictionary.Add(SnapCommand.RemoveGroupFooter, SnapCommandId.RemoveGroupFooter);
			dictionary.Add(SnapCommand.ExportDocument, SnapCommandId.ExportDocument);
			dictionary.Add(SnapCommand.HighlightActiveElement, SnapCommandId.HighlightActiveElement);
			dictionary.Add(SnapCommand.ShowReportStructureEditorForm, SnapCommandId.ShowReportStructureEditorForm);
			dictionary.Add(SnapCommand.InsertPageBreakGroupSeparator, SnapCommandId.InsertPageBreakGroupSeparator);
			dictionary.Add(SnapCommand.InsertNoneGroupSeparator, SnapCommandId.InsertNoneGroupSeparator);
			dictionary.Add(SnapCommand.InsertEmptyParagraphGroupSeparator, SnapCommandId.InsertEmptyParagraphGroupSeparator);
			dictionary.Add(SnapCommand.InsertEmptyRowGroupSeparator, SnapCommandId.InsertEmptyRowGroupSeparator);
			dictionary.Add(SnapCommand.InsertSectionBreakNextPageGroupSeparator, SnapCommandId.InsertSectionBreakNextPageGroupSeparator);
			dictionary.Add(SnapCommand.InsertSectionBreakEvenPageGroupSeparator, SnapCommandId.InsertSectionBreakEvenPageGroupSeparator);
			dictionary.Add(SnapCommand.InsertSectionBreakOddPageGroupSeparator, SnapCommandId.InsertSectionBreakOddPageGroupSeparator);
			dictionary.Add(SnapCommand.InsertPageBreakDataRowSeparator, SnapCommandId.InsertPageBreakDataRowSeparator);
			dictionary.Add(SnapCommand.InsertNoneDataRowSeparator, SnapCommandId.InsertNoneDataRowSeparator);
			dictionary.Add(SnapCommand.InsertEmptyParagraphDataRowSeparator, SnapCommandId.InsertEmptyParagraphDataRowSeparator);
			dictionary.Add(SnapCommand.InsertEmptyRowDataRowSeparator, SnapCommandId.InsertEmptyRowDataRowSeparator);
			dictionary.Add(SnapCommand.InsertSectionBreakNextPageDataRowSeparator, SnapCommandId.InsertSectionBreakNextPageDataRowSeparator);
			dictionary.Add(SnapCommand.InsertSectionBreakEvenPageDataRowSeparator, SnapCommandId.InsertSectionBreakEvenPageDataRowSeparator);
			dictionary.Add(SnapCommand.InsertSectionBreakOddPageDataRowSeparator, SnapCommandId.InsertSectionBreakOddPageDataRowSeparator);
			dictionary.Add(SnapCommand.GroupHeader, SnapCommandId.GroupHeader);
			dictionary.Add(SnapCommand.GroupFooter, SnapCommandId.GroupFooter);
			dictionary.Add(SnapCommand.ListHeader, SnapCommandId.ListHeader);
			dictionary.Add(SnapCommand.ListFooter, SnapCommandId.ListFooter);
			dictionary.Add(SnapCommand.SummaryByField, SnapCommandId.SummaryByField);
			dictionary.Add(SnapCommand.InsertGroupSeparator, SnapCommandId.InsertGroupSeparator);
			dictionary.Add(SnapCommand.InsertDataRowSeparator, SnapCommandId.InsertDataRowSeparator);
			dictionary.Add(SnapCommand.ConvertToParagraphs, SnapCommandId.ConvertToParagraphs);
			dictionary.Add(SnapCommand.InsertBarCode, SnapCommandId.InsertBarCode);
			dictionary.Add(SnapCommand.InsertSparkline, SnapCommandId.InsertSparkline);
			dictionary.Add(SnapCommand.InsertCheckBox, SnapCommandId.InsertCheckBox);
			dictionary.Add(SnapCommand.InsertChart, SnapCommandId.InsertChart);
			dictionary.Add(SnapCommand.InsertIndex, SnapCommandId.InsertIndex);
			dictionary.Add(SnapCommand.GroupFieldsCollection, SnapCommandId.GroupFieldsCollection);
			dictionary.Add(SnapCommand.DeleteList, SnapCommandId.DeleteList);
			dictionary.Add(SnapCommand.FilterList, SnapCommandId.FilterList);
			dictionary.Add(SnapCommand.NewDataSource, SnapCommandId.NewDataSource);
			dictionary.Add(SnapCommand.SaveTheme, SnapCommandId.SaveTheme);
			dictionary.Add(SnapCommand.LoadTheme, SnapCommandId.LoadTheme);
			dictionary.Add(SnapCommand.MailMergeFilters, SnapCommandId.MailMergeFilters);
			dictionary.Add(SnapCommand.MailMergeSorting, SnapCommandId.MailMergeSorting);
			dictionary.Add(SnapCommand.SnapMailMergeExportDocument, SnapCommandId.SnapMailMergeExportDocument);
			dictionary.Add(SnapCommand.SnapMailMergePrint, SnapCommandId.SnapMailMergePrint);
			dictionary.Add(SnapCommand.SnapMailMergePrintPreview, SnapCommandId.SnapMailMergePrintPreview);
			dictionary.Add(SnapCommand.FinishAndMerge, SnapCommandId.FinishAndMerge);
			dictionary.Add(SnapCommand.SnapFilterField, SnapCommandId.SnapFilterField);
			dictionary.Add(SnapCommand.SnapSortFieldAscending, SnapCommandId.SnapSortFieldAscending);
			dictionary.Add(SnapCommand.SnapSortFieldDescending, SnapCommandId.SnapSortFieldDescending);
		}
		public RichEditCommandId CommandID { get { return dictionary[SnapCommand]; } }
		public SnapCommand SnapCommand { get; set; }
	}
	public class CommandBarItem : RichEditCommandBarButtonItem {
		CommandContainer commandContainer = new CommandContainer();
		public CommandBarItem() {
		}
		public CommandBarItem(BarManager manager)
			: base(manager) {
		}
		public CommandBarItem(string caption)
			: base(caption) {
		}
		public CommandBarItem(BarManager manager, string caption)
			: base(manager, caption) {
		}
		protected override RichEditCommandId CommandId { get { return commandContainer.CommandID; } }
		[Browsable(false)]
		public SnapCommand SnapCommand { get { return commandContainer.SnapCommand; } set { commandContainer.SnapCommand = value; } }
	}
	public class CommandBarCheckItem : RichEditCommandBarCheckItem {
		CommandContainer commandContainer = new CommandContainer();
		public CommandBarCheckItem() {
		}
		public CommandBarCheckItem(BarManager manager)
			: base(manager) {
		}
		public CommandBarCheckItem(string caption)
			: base(caption) {
		}
		public CommandBarCheckItem(BarManager manager, string caption)
			: base(manager, caption) {
		}
		protected override RichEditCommandId CommandId { get { return commandContainer.CommandID; } }
		[Browsable(false)]
		public SnapCommand SnapCommand { get { return commandContainer.SnapCommand; } set { commandContainer.SnapCommand = value; } }
	}
	public class CommandBarSubItem : RichEditCommandBarSubItem {
		CommandContainer commandContainer = new CommandContainer();
		public CommandBarSubItem() {
		}
		public CommandBarSubItem(BarManager manager)
			: base(manager) {
		}
		public CommandBarSubItem(string caption)
			: base(caption) {
		}
		public CommandBarSubItem(BarManager manager, string caption)
			: base(manager, caption) {
		}
		protected override RichEditCommandId CommandId { get { return commandContainer.CommandID; } }
		[Browsable(false)]
		public SnapCommand SnapCommand { get { return commandContainer.SnapCommand; } set { commandContainer.SnapCommand = value; } }
	}
	public class ThemesGalleryBarItem : ControlCommandGalleryBarItem<RichEditControl, RichEditCommandId> {
		public ThemesGalleryBarItem() {
			Gallery.ColumnCount = 7;
			Gallery.RowCount = 1;
			Gallery.ItemCheckMode = ItemCheckMode.SingleCheck;
			GalleryItemCheckedChanged += OnGalleryItemCheckedChanged;
			InitizlizeMenuBarItems();
		}
		BarItem SaveItem { get; set; }
		BarItem LoadItem { get; set; }
		BarItem ResetToDefaultItem { get; set; }
		BarItemLink RemoveItemLink { get; set; }
		BarItemLink RestoreDefaultsItemLink { get; set; }
		BarItemLink AssignDocumentStylesItemLink { get; set; }
		SnapDocumentModel DocumentModel { get { return ((SnapControl)SnapControl).DocumentModel; } }
		ISnapControl SnapControl { get { return Control as ISnapControl; } }
		void OnGalleryItemCheckedChanged(object sender, GalleryItemEventArgs e) {
			ThemeGalleryItem item = e.Item as ThemeGalleryItem;
			if (item == null)
				return;
			e.Item.Checked = Object.ReferenceEquals(DocumentModel.ActiveTheme, item.Theme);
		}
		protected override void AfterLoad() {
			base.AfterLoad();
			this.GalleryInitDropDownGallery += OnGalleryInitDropDownGalerry;
			if (Ribbon != null)
				this.Ribbon.ShowCustomizationMenu += OnShowCustomizationMenu;
			Gallery.CustomDrawItemImage += OnCustomDrawItemImage;
		}
		void OnCustomDrawItemImage(object sender, GalleryItemCustomDrawEventArgs e) {
			Image themeNativeImage = (((ThemeGalleryItem)e.Item).Value as Theme).Icon.NativeImage;
			e.Cache.Graphics.DrawImage(themeNativeImage, e.Bounds);
			e.Handled = true;
		}
		void OnShowCustomizationMenu(object sender, RibbonCustomizationMenuEventArgs e) {
			if (e.HitInfo == null || e.HitInfo.GalleryItem == null || e.Link.Item != this)
				return;
			DevExpress.XtraBars.Ribbon.Internal.RibbonCustomizationPopupMenu menu = e.CustomizationMenu;
			Theme theme = ((ThemeGalleryItem)e.HitInfo.GalleryItem).Value as Theme;
			if (RemoveItemLink == null && !theme.IsDefault) {
				BarItem removeItem = CreateRemoveItem(theme);
				RemoveItemLink = menu.InsertItem(menu.ItemLinks[0], removeItem);
			}
			if (RestoreDefaultsItemLink == null && theme.IsDefault) {
				BarItem restoreDefaultsItem = CreateRestoreDefaultsItem(theme, e.HitInfo.GalleryItem);
				RestoreDefaultsItemLink = menu.InsertItem(menu.ItemLinks[0], restoreDefaultsItem);
			}
			if (AssignDocumentStylesItemLink == null) {
				BarItem assignDocumentStylesItem = CreateAssignDocumentStylesItem(theme);
				AssignDocumentStylesItemLink = menu.InsertItem(menu.ItemLinks[0], assignDocumentStylesItem);
			}
			if (menu.ItemLinks[2] != null)
				menu.ItemLinks[2].BeginGroup = true;
			menu.CloseUp += OnMenuCloseUp;
		}
		void OnMenuCloseUp(object sender, EventArgs e) {
			PopupMenu menu = (PopupMenu)sender;
			menu.RemoveLink(RemoveItemLink);
			RemoveItemLink = null;
			menu.RemoveLink(RestoreDefaultsItemLink);
			RestoreDefaultsItemLink = null;
			menu.RemoveLink(AssignDocumentStylesItemLink);
			AssignDocumentStylesItemLink = null;
			menu.CloseUp -= OnMenuCloseUp;
		}
		void OnGalleryInitDropDownGalerry(object sender, InplaceGalleryEventArgs e) {
			AddItemsToDropDownGallery(e);
			e.PopupGallery.RowCount = 2;
			e.PopupGallery.ShowGroupCaption = true;
		}
		void AddItemsToDropDownGallery(InplaceGalleryEventArgs e) {
			InDropDownGallery popupGallery = e.PopupGallery;
			BarItemLinkCollection galleryDropDownItemLinks = popupGallery.GalleryDropDown.ItemLinks;
			galleryDropDownItemLinks.Add(SaveItem);
			galleryDropDownItemLinks.Add(LoadItem);
			galleryDropDownItemLinks.Add(ResetToDefaultItem);
			popupGallery.ItemRightClick += OnPopupGalleryItemRightClick;
		}
		void OnPopupGalleryItemRightClick(object sender, GalleryItemClickEventArgs e) {
			if (e.Item == null)
				return;
			PopupMenu menu = new PopupMenu(this.Manager);
			Theme theme = ((ThemeGalleryItem)e.Item).Value as Theme;
			if (!theme.IsDefault) {
				BarItem removeItem = CreateRemoveItem(theme);
				menu.AddItem(removeItem);
			}
			else {
				BarItem restoreDefaults = CreateRestoreDefaultsItem(theme, e.Item);
				menu.AddItem(restoreDefaults);
			}
			BarItem assignDocumentStylesItem = CreateAssignDocumentStylesItem(theme);
			menu.AddItem(assignDocumentStylesItem);
			menu.ShowPopup(RichEditControl.MousePosition);
		}
		protected void InitizlizeMenuBarItems() {
			SaveItem = CreateItem(SnapExtensionsLocalizer.GetString(SnapExtensionsStringId.SaveThemeBarItem_Caption), (sender, e) => SnapControl.SaveTheme());
			SaveItem.Glyph = CommandResourceImageLoader.LoadSmallImage("DevExpress.Snap.Images", "SaveCurrentTheme", typeof(ISnapControl).Assembly);
			LoadItem = CreateItem(SnapExtensionsLocalizer.GetString(SnapExtensionsStringId.LoadThemeBarItem_Caption), (sender, e) => SnapControl.LoadTheme());
			LoadItem.Glyph = CommandResourceImageLoader.LoadSmallImage("DevExpress.Snap.Images", "LoadTheme", typeof(ISnapControl).Assembly);
			ResetToDefaultItem = CreateItem(SnapExtensionsLocalizer.GetString(SnapExtensionsStringId.RestoreDefaultDocumentStylesBarItem_Caption), (sender, e) => DocumentModel.SetDefaultTheme());
		}
		protected BarButtonItem CreateItem(string caption, ItemClickEventHandler onItemClick) {
			BarButtonItem item = new BarButtonItem();
			item.Caption = caption;
			item.ItemClick += onItemClick;
			return item;
		}
		protected BarButtonItem CreateAssignDocumentStylesItem(Theme theme) {
			return CreateItem(SnapExtensionsLocalizer.GetString(SnapExtensionsStringId.UpdateToMatchDocumentStylesBarItem_Caption), (sender, e) => theme.Actualize(DocumentModel));
		}
		protected BarButtonItem CreateRemoveItem(Theme theme) {
			return CreateItem(SnapExtensionsLocalizer.GetString(SnapExtensionsStringId.RemoveThemeBarItem_Caption), (sender, e) => DocumentModel.RemoveTheme(theme));
		}
		protected BarButtonItem CreateRestoreDefaultsItem(Theme theme, GalleryItem galleryItem) {
			BarButtonItem item = CreateItem(SnapExtensionsLocalizer.GetString(SnapExtensionsStringId.RestoreDefaultsBarItem_Caption), (sender, e) => {
				DocumentModel.RestoreDefaults(theme);
				galleryItem.Caption = theme.Name;
				galleryItem.Hint = theme.Name;
			});
			item.Enabled = theme.IsModified;
			return item;
		}
		protected override void OnControlChanged() {
			base.OnControlChanged();
			if (DesignMode || Control == null || Control.Site != null)
				return;
			DocumentModel.Themes.CollectionChanged += OnCollectionChanged;
			DocumentModel.ActiveThemeChanged += OnActiveThemeChanged;
			PopulateGallaryItems();
		}
		void OnActiveThemeChanged(object sender, EventArgs e) {
			if (DocumentModel.ActiveTheme == null)
				UncheckItems();
			else {
				GalleryItem item = Gallery.GetItemByCaption(DocumentModel.ActiveTheme.Name);
				item.Checked = true;
			}
		}
		void UncheckItems() {
			Gallery.BeginUpdate();
			try {
				foreach (GalleryItem item in Gallery.GetCheckedItems())
					item.Checked = false;
			}
			finally {
				Gallery.EndUpdate();
			}
		}
		void OnCollectionChanged(object sender, EventArgs e) {
			PopulateGallaryItems();
		}
		void PopulateGallaryItems() {
			Gallery.BeginUpdate();
			try {
				if (Gallery.Groups.Count == 0)
					CreateGroups();
				GalleryItemGroup builtInGroup = Gallery.Groups[0];
				DisposeThemeGalleryItems(builtInGroup.Items);
				PopulateGallery(builtInGroup, true);
				GalleryItemGroup customGroup = Gallery.Groups[1];
				DisposeThemeGalleryItems(customGroup.Items);
				PopulateGallery(customGroup, false);
				UpdateItemCaption();
			}
			finally {
				Gallery.EndUpdate();
			}
		}
		void CreateGroups() {
			GalleryItemGroup builtInGroup = new GalleryItemGroup();
			builtInGroup.Caption = SnapExtensionsLocalizer.GetString(SnapExtensionsStringId.GalleryGroupCaption_Regular);
			Gallery.Groups.Add(builtInGroup);
			GalleryItemGroup customGroup = new GalleryItemGroup();
			customGroup.Caption = SnapExtensionsLocalizer.GetString(SnapExtensionsStringId.GalleryGroupCaption_Custom);
			Gallery.Groups.Add(customGroup);
		}
		void PopulateGallery(GalleryItemGroup group, bool withDefaultThemes) {
			foreach (Theme theme in DocumentModel.Themes) {
				if (withDefaultThemes != theme.IsDefault || !theme.EnsureThemeLoaded())
					continue;
				ThemeGalleryItem galleryItem = new ThemeGalleryItem();
				galleryItem.Control = SnapControl;
				galleryItem.Value = theme;
				group.Items.Add(galleryItem);
			}
			group.Visible = group.HasVisibleItems();
		}
		void DisposeThemeGalleryItems(GalleryItemCollection items) {
			foreach (ThemeGalleryItem item in items)
				item.Dispose();
			items.Clear();
		}
		protected override RichEditCommandId CommandId {
			get { return RichEditCommandId.None; }
		}
		protected override Command CreateCommand() {
			return null;
		}
	}
	#region ThemeGalleryItem
	public class ThemeGalleryItem : CommandBasedGalleryItem {
		public ISnapControl Control { get; set; }
		public Theme Theme { get { return Value as Theme; } }
		protected override void OnPropertiesChanged(string propName) {
			base.OnPropertiesChanged(propName);
			if (propName == "Value" && Theme != null)
				Theme.PropertyChanged += OnThemeNameGhanged;
		}
		void OnThemeNameGhanged(object sender, EventArgs e) {
			ResetCaption();
			ResetHint();
		}
		protected override GalleryItem CreateItem() {
			return new ThemeGalleryItem();
		}
		public override void Assign(GalleryItem galleryItem) {
			base.Assign(galleryItem);
			ThemeGalleryItem themeItem = galleryItem as ThemeGalleryItem;
			if (themeItem == null)
				return;
			Control = themeItem.Control;
			Value = themeItem.Value;
		}
		protected override Command CreateCommand() {
			return new ChangeThemeCoreCommand(Control, Theme);
		}
		protected override void Dispose(bool disposing) {
			if (Theme != null)
				Theme.PropertyChanged -= OnThemeNameGhanged;
			base.Dispose(disposing);
		}
	}
	#endregion
	public class SnapBarToolbarsListItem : BarToolbarsListItem {
		public SnapBarToolbarsListItem() {
			ShowDockPanels = true;
			ShowToolbars = false;
			ShowCustomizationItem = false;
			Caption = SnapExtensionsLocalizer.Active.GetLocalizedString(SnapExtensionsStringId.SnapBarToolbarsListItem_Caption);
			Hint = SnapExtensionsLocalizer.Active.GetLocalizedString(SnapExtensionsStringId.SnapBarToolbarsListItem_Hint);
			this.Glyph = Image.FromStream(GetType().Assembly.GetManifestResourceStream("DevExpress.Snap.Extensions.Images.Windows.png"));
			this.LargeGlyph = Image.FromStream(GetType().Assembly.GetManifestResourceStream("DevExpress.Snap.Extensions.Images.WindowsLarge.png"));
		}
		protected override bool ShouldSerializeCaption() {
			return string.Compare(Caption, SnapExtensionsLocalizer.Active.GetLocalizedString(SnapExtensionsStringId.SnapBarToolbarsListItem_Caption)) != 0;
		}
		bool ShouldSerializeHint() { return string.Compare(Caption, SnapExtensionsLocalizer.Active.GetLocalizedString(SnapExtensionsStringId.SnapBarToolbarsListItem_Hint)) != 0; }
	}
	public class ToggleFieldHighlightingItem : RichEditCommandBarCheckItem {
		static readonly Rectangle SmallGlyphColorRect = new Rectangle(new Point(1, 12), new Size(14, 3));
		static readonly Rectangle LargeGlyphColorRect = new Rectangle(new Point(2, 26), new Size(28, 4));
		public ToggleFieldHighlightingItem() {
		}
		public ToggleFieldHighlightingItem(BarManager manager)
			: base(manager) {
		}
		public ToggleFieldHighlightingItem(string caption)
			: base(caption) {
		}
		public ToggleFieldHighlightingItem(BarManager manager, string caption)
			: base(manager, caption) {
		}
		protected override RichEditCommandId CommandId { get { return SnapCommandId.ToggleFieldHighlighting; } }
		protected Color Color { get { return SnapControl != null ? SnapControl.Options.Fields.HighlightColor : Color.Empty; } }
		SnapControl SnapControl { get { return Control != null ? (SnapControl)Control : null; } }
		void OnFieldsOptionsChanged(object sender, Utils.Controls.BaseOptionChangedEventArgs e) {
			if (e.Name == "HighlightColor") {
				Glyph = null;
				LargeGlyph = null;
				UpdateItemGlyphs();
			}
		}
		protected override void SubscribeControlEvents() {
			base.SubscribeControlEvents();
			SnapControl.Options.Fields.Changed += OnFieldsOptionsChanged;
		}
		protected override void UnsubscribeControlEvents() {
			base.UnsubscribeControlEvents();
			SnapControl.Options.Fields.Changed -= OnFieldsOptionsChanged;
		}
		protected override Image LoadDefaultImage() {
			using (Image image = base.LoadDefaultImage()) {
				return CreateImage(image, SmallGlyphColorRect);
			}
		}
		protected override Image LoadDefaultLargeImage() {
			using (Image image = base.LoadDefaultLargeImage()) {
				return CreateImage(image, LargeGlyphColorRect);
			}
		}
		protected virtual Image CreateImage(Image image, Rectangle rect) {
			if (image == null)
				return null;
			Bitmap bitmap = new Bitmap(image.Width, image.Height);
			using (Graphics gr = Graphics.FromImage(bitmap)) {
				using (SolidBrush brush = new SolidBrush(Color)) {
					gr.FillRectangle(brush, rect);
				}
				gr.DrawImage(image, new Point(0, 0));
			}
			return bitmap;
		}
	}
	public class ChangeEditorRowLimitItem : RichEditCommandBarEditItem<EditorRowLimitCommandValue>, IBarButtonGroupMember {
		public ChangeEditorRowLimitItem() {
		}
		public ChangeEditorRowLimitItem(BarManager manager, string caption)
			: base(manager, caption) {
		}
		protected override RichEditCommandId CommandId { get { return SnapCommandId.ChangeEditorRowLimit; } }
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new SnapControl Control { get { return (SnapControl)base.Control; } set { Control = value; } }
		protected override ICommandUIState CreateCommandUIState(Command command) {
			DefaultValueBasedCommandUIState<EditorRowLimitCommandValue> value = new DefaultValueBasedCommandUIState<EditorRowLimitCommandValue>();
			value.Value = TryParseEditValue(EditValue);
			return value;
		}
		protected override RepositoryItem CreateEdit() {
			RepositoryItemEditorRowLimitEdit edit = new RepositoryItemEditorRowLimitEdit();
			if (Control != null)
				edit.Control = Control;
			return edit;
		}
		protected override void OnControlChanged() {
			RepositoryItemEditorRowLimitEdit edit = (RepositoryItemEditorRowLimitEdit)Edit;
			if (edit != null)
				edit.Control = Control;
		}
		protected override void OnEditValidating(object sender, CancelEventArgs e) {
			ComboBoxEdit edit = (ComboBoxEdit)sender;
			EditorRowLimitCommandValue value = TryParseEditValue(edit.EditValue);
			if (value == null || value.Value < 0) {
				edit.ErrorText = SnapLocalizer.GetString(SnapStringId.Msg_InvalidEditorRowLimit);
				e.Cancel = true;
			}
		}
		EditorRowLimitCommandValue TryParseEditValue(object editValue) {
			try {
				EditorRowLimitCommandValue value = editValue as EditorRowLimitCommandValue;
				if (value != null)
					return value;
				int newValue = Convert.ToInt32(editValue);
				return new EditorRowLimitCommandValue(newValue);
			}
			catch {
				return null;
			}
		}
		#region IBarButtonGroupMember Members
		object IBarButtonGroupMember.ButtonGroupTag { get { return HomeButtonGroups.FontNameAndSizeButtonGroup; } }
		#endregion
	}
	#region GalleryChangeTableCellStyleItem
	public class GalleryChangeTableCellStyleItem : GalleryChangeTableStyleItemBase { 
		#region Fields
		const string text = "Aa";
		const char endOfCellSymbol = (char)164;
		static StringFormat format = CreateFormat();
		TableCellStyle currentCellStyle;
		#endregion
		static StringFormat CreateFormat() {
			StringFormat result = new StringFormat(StringFormatFlags.NoWrap);
			result.LineAlignment = StringAlignment.Center;
			result.Alignment = StringAlignment.Center;
			return result;
		}
		public GalleryChangeTableCellStyleItem() {
		}
		#region Properties
		public TableCellStyle CurrentCellStyle { get { return currentCellStyle; } set { currentCellStyle = value; } }
		public TableCellStyle CurrentItemCellStyle { get; set; }
		protected override RichEditCommandId CommandId { get { return SnapCommandId.ChangeTableCellStyle; } }
		#endregion
		protected override RepositoryItemRichEditTableStyleEditBase CreateRepository() {
			return new RepositoryItemRichEditTableCellStyleEdit();
		}
		protected override IXtraRichEditFormatting CreateStyleFormatting() {
			TableCellStyle defaultStyle = Control.DocumentModel.TableCellStyles.DefaultItem;
			return new TableCellStyleFormatting(defaultStyle.Id);
		}
		protected override void InitizlizeMenuBarItems() {
			NewItem = CreateItem(SnapLocalizer.GetString(SnapStringId.MenuCmd_NewTableCellStyle), OnNewItemClick);
			NewItem.Glyph = LoadSmallImageToGlyph("NewTableCellStyle");
			ModifyItem = CreateItem(SnapLocalizer.GetString(SnapStringId.MenuCmd_ModifyTableCellStyle), OnModifyItemClick);
			ModifyItem.Glyph = LoadSmallImageToGlyph("ModifyTableCellStyle");
			DeleteItem = CreateItem(SnapLocalizer.GetString(SnapStringId.MenuCmd_DeleteTableCellStyle), OnDeleteItemClick);
			DeleteItem.Glyph = LoadSmallImageToGlyph("ClearTableCellStyle");
		}
		public static Image LoadSmallImageToGlyph(string name) {
			return CommandResourceImageLoader.LoadSmallImage("DevExpress.Snap.Images", name, typeof(DevExpress.Snap.Core.ISnapControl).Assembly);
		}
		protected override void AddItemsToDropDownGallery(InplaceGalleryEventArgs e) {
			base.AddItemsToDropDownGallery(e);
			if (((SnapControl)this.Control).DocumentModel.TableCellStyles.DefaultItem == CurrentCellStyle) {
				DeleteItem.Enabled = false;
				ModifyItem.Enabled = false;
			}
		}
		protected override void OnMenuCloseUp(object sender, EventArgs e) {
			base.OnMenuCloseUp(sender, e);
			if (CurrentItemCellStyle != CurrentCellStyle)
				CurrentItem.Checked = false;
			CurrentItemCellStyle = null;
		}
		protected override void GalleryMenuCloseUp() {
			CurrentItemCellStyle = null;
		}
		protected override PopupMenu GetMenu() {
			TableCellStyleFormatting styleFormatting = CurrentItem.Tag as TableCellStyleFormatting;
			CurrentItemCellStyle = Control.DocumentModel.TableCellStyles.GetStyleById(styleFormatting.StyleId);
			PopupMenu menu = new PopupMenu();
			menu.Manager = this.Manager;
			BarItem newTableItem = CreateItem(SnapLocalizer.GetString(SnapStringId.MenuCmd_NewTableCellStyle), OnNewItemClick);
			newTableItem.Glyph = LoadSmallImageToGlyph("NewTableCellStyle");
			menu.AddItem(newTableItem);
			BarItem modifyTableItem = CreateItem(SnapLocalizer.GetString(SnapStringId.MenuCmd_ModifyTableCellStyle), OnModifyItemClick);
			modifyTableItem.Glyph = LoadSmallImageToGlyph("ModifyTableCellStyle");
			menu.AddItem(modifyTableItem);
			BarItem deleteTableItem = CreateItem(SnapLocalizer.GetString(SnapStringId.MenuCmd_DeleteTableCellStyle), OnDeleteItemClick);
			deleteTableItem.Glyph = LoadSmallImageToGlyph("ClearTableCellStyle");
			menu.AddItem(deleteTableItem);
			if (((SnapControl)this.Control).DocumentModel.TableCellStyles.DefaultItem == CurrentItemCellStyle) {
				modifyTableItem.Enabled = false;
				deleteTableItem.Enabled = false;
			}
			return menu;
		}
		protected override void GalleryChangeStyleItemCheckedChanged(GalleryItemEventArgs e) {
			if (Control == null || Control.IsDisposed)
				return;
			GalleryItem currentItem = e.Item;
			TableCellStyleFormatting styleFormatting = currentItem.Tag as TableCellStyleFormatting;
			if (styleFormatting == null || Control.DocumentModel == null)
				return;
			TableCellStyle tableCellStyle = Control.DocumentModel.TableCellStyles.GetStyleById(styleFormatting.StyleId);
			if (currentItem.Checked && CurrentItemCellStyle != tableCellStyle)
				CurrentItemCellStyle = tableCellStyle;
		}
		protected override void ShowCustomizationMenu(RibbonCustomizationMenuEventArgs e) {
			if (e.HitInfo != null)
				CurrentItem = e.HitInfo.GalleryItem;
			if (CurrentItem == null || e.Link.Item != this)
				return;
			CurrentItem.Checked = true;
			DeleteItem.Enabled = true;
			ModifyItem.Enabled = true;
			TableCellStyleFormatting cellStyleFormatting = (TableCellStyleFormatting)CurrentItem.Tag;
			CurrentItemCellStyle = Control.DocumentModel.TableCellStyles.GetStyleById(cellStyleFormatting.StyleId); 
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
			if (((SnapControl)this.Control).DocumentModel.TableCellStyles.DefaultItem == CurrentItemCellStyle) {
				DeleteItem.Enabled = false;
				ModifyItem.Enabled = false;
			}
		}
		private void ShowTableCellForm(DocumentModel documentModel, TableCellStyle newStyle) {
			TableCellStyleFormControllerParameters controllerParameters = new TableCellStyleFormControllerParameters(Control, newStyle);
			controllerParameters.CurrentStyleEnabled = false;
			TableCellStyleFormShowingEventArgs args = new TableCellStyleFormShowingEventArgs(controllerParameters);
			SnapControl control = (SnapControl)this.Control;
			control.RaiseTableCellStyleFormShowing(args);
			if (!args.Handled) {
				using (SnapTableStyleForm form = new SnapTableStyleForm(controllerParameters)) {
					DialogResult result = form.ShowDialog(Control);
					if (result == DialogResult.OK)
						documentModel.TableCellStyles.AddNewStyle(newStyle);
				}
			}
		}
		protected override void NewItemClick() {
			SnapControl control = (SnapControl)this.Control;
			DocumentModel documentModel = control.DocumentModel;
			TableCellStyleCollection styles = documentModel.TableCellStyles;
			TableCellStyle newStyle = new TableCellStyle(documentModel);
			newStyle.StyleName = "Style";
			newStyle.Parent = styles.DefaultItem;
			for (int i = 1; i <= styles.Count; i++) {
				TableCellStyle style = styles.GetStyleByName(newStyle.StyleName + i);
				if (style == null) {
					newStyle.StyleName += i;
					break;
				}
			}
			ShowTableCellForm(documentModel, newStyle);
		}
		protected override void ModifyItemClick() {
			SnapShowTableStyleFormCommand command = new SnapShowTableStyleFormCommand(this.Control, CurrentItemCellStyle);
			command.Execute();
		}
		protected override void DeleteItemClick() {
			string styleName = CurrentCellStyle.StyleName;
			if (CurrentItemCellStyle != null)
				styleName = CurrentItemCellStyle.StyleName;
			string questionString = String.Format(XtraRichEditLocalizer.GetString(XtraRichEditStringId.Msg_DeleteTableStyleQuestion), styleName);
			DialogResult isDelete = XtraMessageBox.Show(questionString, Application.ProductName, MessageBoxButtons.YesNo, MessageBoxIcon.Question);
			if (isDelete == DialogResult.Yes) {
				TableCellStyleCollection tableCellStyles = ((SnapControl)this.Control).DocumentModel.TableCellStyles;
				if (CurrentItemCellStyle != null)
					tableCellStyles.Delete(CurrentItemCellStyle);
				else
					tableCellStyles.Delete(CurrentCellStyle);
			}
		}
		protected override void CustomDrawItem(GalleryItemCustomDrawEventArgs e) {
			if (Control == null || Control.IsDisposed)
				return;
			TableCellStyleFormatting currentCellStyle = e.Item.Tag as TableCellStyleFormatting;
			if (currentCellStyle != null)
				DrawTableCellStyleItem(e, currentCellStyle);
		}
		private void DrawTableCellStyleItem(GalleryItemCustomDrawEventArgs e, TableCellStyleFormatting currentStyleFormatting) {
			GraphicsCache cache = e.Cache;
			Rectangle itemBounds = e.Bounds;
			int itemHeight = itemBounds.Height;
			int itemWidth = itemBounds.Width;
			TableCellStyle style = Control.DocumentModel.TableCellStyles.GetStyleById(currentStyleFormatting.StyleId); ;
			SnapDocumentModel documentModel = ((SnapControl)this.Control).DocumentModel;
			RunInfo interval = documentModel.Selection.Interval;
			DocumentModelPosition start = interval.Start;
			DocumentModelPosition end = interval.End;
			ParagraphCollection paragraphs = documentModel.ActivePieceTable.Paragraphs;
			TableCell startCell = paragraphs[start.ParagraphIndex].GetCell();
			TableCell endCell = paragraphs[end.ParagraphIndex].GetCell();
			if (startCell != null && endCell != null && startCell == endCell)
				currentCellStyle = startCell.TableCellStyle;
			Rectangle rect = new Rectangle(itemBounds.Left, itemBounds.Top, itemWidth, itemHeight);
			int innerLeftLocation = itemBounds.Left + 2;
			int innerTopLocation = itemBounds.Top + 2;
			int innerWidth = itemWidth - 5;
			int innerHeight = itemHeight - 6;
			Rectangle innerRect = new Rectangle(innerLeftLocation, innerTopLocation, innerWidth, innerHeight);
			Point leftTop = new Point(innerRect.Left, innerRect.Top);
			Point leftBottom = new Point(innerRect.Left, innerRect.Bottom);
			Point rightTop = new Point(innerRect.Right, innerRect.Top);
			Point rightBottom = new Point(innerRect.Right, innerRect.Bottom);
			Point boldLeftTop = new Point(innerRect.Left + 1, innerRect.Top + 1);
			Point boldLeftBottom = new Point(innerRect.Left + 1, innerRect.Bottom + 1);
			Point boldRightTop = new Point(innerRect.Right + 1, innerRect.Top + 1);
			Point boldRightBottom = new Point(innerRect.Right + 1, innerRect.Bottom + 1);
			DrawGalleryBackground(Color.FromArgb(255, 255, 255), cache.Graphics, rect);
			cache.Graphics.FillRectangle(cache.GetSolidBrush(style.TableCellProperties.BackgroundColor), innerRect);
			MergedCharacterProperties characterProperties = style.GetMergedWithDefaultCharacterProperties(ConditionalRowType.Normal, ConditionalColumnType.Normal);
			DrawStyle(characterProperties.Info, cache, innerRect);
			DrawBorder(style.TableCellProperties.Borders.LeftBorder, leftTop, leftBottom, boldLeftTop, boldLeftBottom, cache.Graphics);
			DrawBorder(style.TableCellProperties.Borders.BottomBorder, leftBottom, rightBottom, boldLeftTop, boldRightTop, cache.Graphics);
			DrawBorder(style.TableCellProperties.Borders.RightBorder, rightTop, rightBottom, boldRightTop, boldRightBottom, cache.Graphics);
			DrawBorder(style.TableCellProperties.Borders.TopBorder, leftTop, rightTop, boldLeftBottom, boldRightBottom, cache.Graphics);
			e.Handled = true;
		}
		protected internal virtual Brush CalculateBrush(GraphicsCache cache, Color charForeColor) {
			Color foreColor = DXColor.IsTransparentOrEmpty(charForeColor) ? Color.Black : charForeColor;
			return cache.GetSolidBrush(foreColor);
		}
		protected internal virtual FontStyle CalculateFontStyle(CharacterFormattingInfo charInfo) {
			FontStyle result = FontStyle.Regular;
			if (charInfo.FontBold)
				result |= FontStyle.Bold;
			if (charInfo.FontItalic)
				result |= FontStyle.Italic;
			return result;
		}
		protected virtual void DrawStyle(CharacterFormattingInfo charInfo, GraphicsCache cache, Rectangle rect) {
			FontStyle style = CalculateFontStyle(charInfo);
			Font font = new Font(charInfo.FontName, charInfo.DoubleFontSize / 2f, style);
			Brush brush = CalculateBrush(cache, charInfo.ForeColor);
			cache.Graphics.DrawString(text + endOfCellSymbol, font, brush, rect, format);
		}
	}
	#endregion
}
