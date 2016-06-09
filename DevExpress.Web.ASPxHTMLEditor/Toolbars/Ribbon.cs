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
using System.Web.UI;
using DevExpress.Web.Internal;
namespace DevExpress.Web.ASPxHtmlEditor {
	using System.ComponentModel;
	using DevExpress.Web.ASPxHtmlEditor.Internal;
	[Editor("DevExpress.Web.Design.CommonDesignerEditor, " + AssemblyInfo.SRAssemblyWebDesignFull, typeof(System.Drawing.Design.UITypeEditor))]
	public class HtmlEditorRibbonTabCollection : Collection<RibbonTab> {
		public HtmlEditorRibbonTabCollection(IWebControlObject owner)
			: base(owner) {
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public ASPxHtmlEditor HtmlEditor { get { return Owner as ASPxHtmlEditor; } }
		public RibbonTab Add(string text) {
			RibbonTab tab = new RibbonTab(text);
			Add(tab);
			return tab;
		}
		public void CreateDefaultRibbonTabs() {
			AddRange(new HtmlEditorDefaultRibbon(HtmlEditor).DefaultRibbonTabs);
		}
		protected override void OnChanged() {
			if(Owner != null)
				Owner.LayoutChanged();
			base.OnChanged();
		}
	}
	public class HtmlEditorRibbonContextTabCategoryCollection : Collection<RibbonContextTabCategory> {
		public HtmlEditorRibbonContextTabCategoryCollection(IWebControlObject owner)
			: base(owner) {
		}
		public RibbonContextTabCategory Add(string name) {
			var tabGroup = new RibbonContextTabCategory(name);
			Add(tabGroup);
			return tabGroup;
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public ASPxHtmlEditor HtmlEditor { get { return Owner as ASPxHtmlEditor; } }
		public void CreateDefaultRibbonContextTabCategories() {
			AddRange(new HtmlEditorDefaultRibbon(HtmlEditor).DefaultRibbonContextTabCategories);
		}
		protected override void OnChanged() {
			if(Owner != null)
				Owner.LayoutChanged();
			base.OnChanged();
		}
	}
	public class HtmlEditorRibbonStyles : RibbonStyles {
		const string ControlStyleName = "ControlStyle";
		HtmlEditorRibbonTabControlStyles stylesTabControl;
		HtmlEditorRibbonPopupMenuStyles stylesPopupMenu;
		public HtmlEditorRibbonStyles(ISkinOwner owner)
			: base(owner) {
			this.stylesTabControl = new HtmlEditorRibbonTabControlStyles(owner);
			this.stylesPopupMenu = new HtmlEditorRibbonPopupMenuStyles(owner);
		}
		[Browsable(false)]
		public new string CssFilePath {
			get { return base.CssFilePath; }
			set { base.CssFilePath = value; }
		}
		[Browsable(false)]
		public override string CssPostfix {
			get { return base.CssPostfix; }
			set { base.CssPostfix = value; }
		}
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("HtmlEditorRibbonStylesControl"),
#endif
		NotifyParentProperty(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable]
		public AppearanceStyleBase Control
		{
			get { return GetStyle(ControlStyleName); }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new RibbonFileTabStyle FileTab {
			get { return base.FileTab; }
		}
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("HtmlEditorRibbonStylesTabControl"),
#endif
		NotifyParentProperty(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable]
		public HtmlEditorRibbonTabControlStyles TabControl { get { return stylesTabControl; } }
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("HtmlEditorRibbonStylesPopupMenu"),
#endif
		NotifyParentProperty(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable]
		public HtmlEditorRibbonPopupMenuStyles PopupMenu { get { return stylesPopupMenu; } }
		protected override void PopulateStyleInfoList(List<StyleInfo> list) {
			base.PopulateStyleInfoList(list);
			list.Add(new StyleInfo(ControlStyleName, delegate() { return new AppearanceStyleBase(); }));
		}
		public override void CopyFrom(StylesBase source) {
			base.CopyFrom(source);
			HtmlEditorRibbonStyles styles = source as HtmlEditorRibbonStyles;
			if(styles != null) {
				TabControl.CopyFrom(styles.TabControl);
				PopupMenu.CopyFrom(styles.PopupMenu);
			}
		}
		protected override IStateManager[] GetStateManagedObjects() {
			return ViewStateUtils.GetMergedStateManagedObjects(base.GetStateManagedObjects(),
				new IStateManager[] { TabControl, PopupMenu });
		}
	}
	public class HtmlEditorRibbonTabControlStyles : RibbonTabControlStyles {
		public HtmlEditorRibbonTabControlStyles(ISkinOwner owner)
			: base(owner) {
		}
		[Browsable(false)]
		public new string CssFilePath {
			get { return base.CssFilePath; }
			set { base.CssFilePath = value; }
		}
		[Browsable(false)]
		public override string CssPostfix {
			get { return base.CssPostfix; }
			set { base.CssPostfix = value; }
		}
	}
	public class HtmlEditorRibbonPopupMenuStyles : RibbonMenuStyles {
		public HtmlEditorRibbonPopupMenuStyles(ISkinOwner owner)
			: base(owner) {
		}
		[Browsable(false)]
		public new string CssFilePath {
			get { return base.CssFilePath; }
			set { base.CssFilePath = value; }
		}
		[Browsable(false)]
		public override string CssPostfix {
			get { return base.CssPostfix; }
			set { base.CssPostfix = value; }
		}
	}
}
namespace DevExpress.Web.ASPxHtmlEditor.Internal {
	internal static class HtmlEditorRibbonHelper {
		public static ASPxRibbon GetRibbonControl(object owner) {
			RibbonItemBase item = owner as RibbonItemBase;
			if(item != null)
				return GetRibbonControl(item.Ribbon);
			RibbonGroup group = owner as RibbonGroup;
			if(group != null)
				return GetRibbonControl(group.Ribbon);
			return owner as ASPxRibbon;
		}
		public static ItemImagePropertiesBase GetRibbonItemImageProperty(object owner, string imageName, string imageSize) {
			ASPxRibbon ribbon = GetRibbonControl(owner);
			ItemImageProperties properties = new ItemImageProperties();
			if(string.IsNullOrEmpty(imageName))
				properties.CopyFrom(EmptyImageProperties.GetGlobalEmptyImage(ribbon.Page));
			else
				properties.CopyFrom(RibbonHelper.GetRibbonImageProperties(ribbon, HERibbonImages.RibbonHESpriteName,
					delegate(ISkinOwner skinOwner) { return new HERibbonImages(skinOwner, ribbon.Images.IconSet); }, imageName, imageSize));
			return properties;
		}
	}
	public class HtmlEditorDefaultRibbon {
		RibbonTab[] defaultRibbonTabs;
		RibbonContextTabCategory[] defaultRibbonTabCategories;
		HEHomeRibbonTab homeTab;
		HEInsertRibbonTab insertTab;
		HEViewRibbonTab viewTab;
		HETableRibbonTab tableTab;
		HETableLayoutRibbonTab layoutTableTab;
		HEReviewRibbonTab reviewTab;
		HETableToolsRibbonTabCategory tableToolsTabCategory;
		HEUndoRibbonGroup undoGroup;
		HEClipboardRibbonGroup clipboardGroup;
		HEFontRibbonGroup fontGroup;
		HEParagraphRibbonGroup paragraphGroup;
		HEImagesRibbonGroup imagesGroup;
		HELinksRibbonGroup linksGroup;
		HEViewsRibbonGroup viewsGroup;
		HEMediaRibbonGroup mediaGroup;
		HESpellingRibbonGroup spellingGroup;
		HETablesRibbonGroup tablesGroup;
		HEDeleteTableRibbonGroup deleteTableGroup;
		HEInsertTableRibbonGroup insertTableGroup;
		HEMergeTableRibbonGroup mergeTableGroup;
		HETablePropertiesRibbonGroup tablePropertiesGroup;
		HEEditingRibbonGroup editingGroup;
		protected ASPxHtmlEditor HtmlEditor { get; set; }
		public HtmlEditorDefaultRibbon(ASPxHtmlEditor htmlEditor) {
			HtmlEditor = htmlEditor;
		}
		public RibbonTab[] DefaultRibbonTabs {
			get {
				if(defaultRibbonTabs == null) {
					List<RibbonTab> tabs = new List<RibbonTab>();
					tabs.Add(HomeTab);
					tabs.Add(InsertTab);
					tabs.Add(ViewTab);
					defaultRibbonTabs = tabs.ToArray();
				}
				return defaultRibbonTabs;
			}
		}
		public RibbonContextTabCategory[] DefaultRibbonContextTabCategories {
			get {
				if(defaultRibbonTabCategories == null) {
					List<RibbonContextTabCategory> tabCategories = new List<RibbonContextTabCategory>();
					tabCategories.Add(CreateTableToolsTabCategory());
					defaultRibbonTabCategories = tabCategories.ToArray();
				}
				return defaultRibbonTabCategories;
			}
		}
		public int DefaultActiveTabIndex { 
			get { return 0; } 
		}
		public HEHomeRibbonTab CreateHomeTab() {
			return HomeTab;
		}
		protected HEHomeRibbonTab HomeTab {
			get {
				if(homeTab == null) {
					homeTab = new HEHomeRibbonTab();
					homeTab.Groups.Add(UndoGroup);
					homeTab.Groups.Add(ClipboardGroup);
					homeTab.Groups.Add(FontGroup);
					homeTab.Groups.Add(ParagraphGroup);
					homeTab.Groups.Add(EditingGroup);
				}
				return homeTab;
			}
		}
		protected HEUndoRibbonGroup UndoGroup {
			get {
				if(undoGroup == null)
					undoGroup = CreateUndoGroup();
				return undoGroup;
			}
		}
		public HEUndoRibbonGroup CreateUndoGroup() {
			HEUndoRibbonGroup group = new HEUndoRibbonGroup();
			group.Items.Add(new HEUndoRibbonCommand(RibbonItemSize.Large));
			group.Items.Add(new HERedoRibbonCommand(RibbonItemSize.Large));
			return group;
		}
		protected HEClipboardRibbonGroup ClipboardGroup {
			get {
				if(clipboardGroup == null)
					clipboardGroup = CreateClipboardGroup();
				return clipboardGroup;
			}
		}
		public HEClipboardRibbonGroup CreateClipboardGroup() {
			HEClipboardRibbonGroup group = new HEClipboardRibbonGroup();
			group.Items.Add(new HEPasteFromWordRibbonCommand(RibbonItemSize.Large));
			group.Items.Add(new HECutSelectionRibbonCommand());
			group.Items.Add(new HECopySelectionRibbonCommand());
			group.Items.Add(new HEPasteSelectionRibbonCommand());
			return group;
		}
		protected HEFontRibbonGroup FontGroup {
			get {
				if(fontGroup == null)
					fontGroup = CreateFontGroup();
				return fontGroup;
			}
		}
		public HEFontRibbonGroup CreateFontGroup() {
			HEFontRibbonGroup group = new HEFontRibbonGroup();
			var fontName = new HEFontNameRibbonCommand();
			fontName.CreateDefaultItems(false);
			group.Items.Add(fontName);
			var fontSize = new HEFontSizeRibbonCommand();
			fontSize.CreateDefaultItems(false);
			group.Items.Add(fontSize);
			group.Items.Add(new HEBackColorRibbonCommand());
			group.Items.Add(new HEFontColorRibbonCommand());
			group.Items.Add(new HERemoveFormatRibbonCommand());
			var paragraphFormatting = new HEParagraphFormattingRibbonCommand();
			paragraphFormatting.CreateDefaultItems(false);
			group.Items.Add(paragraphFormatting);
			group.Items.Add(new HEBoldRibbonCommand());
			group.Items.Add(new HEItalicRibbonCommand());
			group.Items.Add(new HEUnderlineRibbonCommand());
			group.Items.Add(new HEStrikeoutRibbonCommand());
			group.Items.Add(new HESuperscriptRibbonCommand());
			group.Items.Add(new HESubscriptRibbonCommand());
			return group;
		}
		protected HEParagraphRibbonGroup ParagraphGroup {
			get {
				if(paragraphGroup == null)
					paragraphGroup = CreateParagraphGroup();
				return paragraphGroup;
			}
		}
		public HEParagraphRibbonGroup CreateParagraphGroup() {
			HEParagraphRibbonGroup group = new HEParagraphRibbonGroup();
			group.Items.Add(new HEInsertUnorderedListRibbonCommand());
			group.Items.Add(new HEInsertOrderedListRibbonCommand());
			group.Items.Add(new HEOutdentRibbonCommand());
			group.Items.Add(new HEIndentRibbonCommand());
			group.Items.Add(new HEAlignmentLeftRibbonCommand());
			group.Items.Add(new HEAlignmentCenterRibbonCommand());
			group.Items.Add(new HEAlignmentRightRibbonCommand());
			return group;
		}
		public HEInsertRibbonTab CreateInsertTab() {
			return InsertTab;
		}
		protected HEInsertRibbonTab InsertTab {
			get {
				if(insertTab == null) {
					insertTab = new HEInsertRibbonTab();
					insertTab.Groups.Add(TablesGroup);
					insertTab.Groups.Add(ImagesGroup);
					insertTab.Groups.Add(LinksGroup);
				}
				return insertTab;
			}
		}
		protected HEImagesRibbonGroup ImagesGroup {
			get {
				if(imagesGroup == null)
					imagesGroup = CreateImagesGroup();
				return imagesGroup;
			}
		}
		public HEImagesRibbonGroup CreateImagesGroup() {
			HEImagesRibbonGroup group = new HEImagesRibbonGroup();
			group.Items.Add(new HEInsertImageRibbonCommand(RibbonItemSize.Large));
			return group;
		}
		protected HELinksRibbonGroup LinksGroup {
			get {
				if(linksGroup == null)
					linksGroup = CreateLinksGroup();
				return linksGroup;
			}
		}
		public HELinksRibbonGroup CreateLinksGroup() {
			HELinksRibbonGroup group = new HELinksRibbonGroup();
			group.Items.Add(new HEInsertLinkDialogRibbonCommand(RibbonItemSize.Large));
			group.Items.Add(new HEUnlinkRibbonCommand(RibbonItemSize.Large));
			return group;
		}
		protected HEMediaRibbonGroup MediaGroup {
			get {
				if(mediaGroup == null)
					mediaGroup = CreateMediaGroup();
				return mediaGroup;
			}
		}
		public HEMediaRibbonGroup CreateMediaGroup() {
			HEMediaRibbonGroup group = new HEMediaRibbonGroup();
			group.Items.Add(new HEInsertYouTubeVideoDialogRibbonCommand(RibbonItemSize.Large));
			group.Items.Add(new HEInsertFlashDialogRibbonCommand());
			group.Items.Add(new HEInsertVideoDialogRibbonCommand());
			group.Items.Add(new HEInsertAudioDialogRibbonCommand());
			return group;
		}
		public HEViewRibbonTab CreateViewTab() {
			return ViewTab;
		}
		protected HEViewRibbonTab ViewTab {
			get {
				if(viewTab == null) {
					viewTab = new HEViewRibbonTab();
					viewTab.Groups.Add(ViewsGroup);
				}
				return viewTab;
			}
		}
		protected HEViewsRibbonGroup ViewsGroup {
			get {
				if(viewsGroup == null)
					viewsGroup = CreateViewsGroup();
				return viewsGroup;
			}
		}
		public HEViewsRibbonGroup CreateViewsGroup() {
			HEViewsRibbonGroup group = new HEViewsRibbonGroup();
			group.Items.Add(new HEFullscreenRibbonCommand(RibbonItemSize.Large));
			return group;
		}
		public HEReviewRibbonTab CreateReviewTab() {
			return ReviewTab;
		}
		protected HEReviewRibbonTab ReviewTab {
			get {
				if(reviewTab == null) {
					reviewTab = new HEReviewRibbonTab();
					reviewTab.Groups.Add(SpellingGroup);
				}
				return reviewTab;
			}
		}
		protected HESpellingRibbonGroup SpellingGroup {
			get {
				if(spellingGroup == null)
					spellingGroup = CreateSpellingGroup();
				return spellingGroup;
			}
		}
		public HESpellingRibbonGroup CreateSpellingGroup() {
			HESpellingRibbonGroup group = new HESpellingRibbonGroup();
			group.Items.Add(new HECheckSpellingRibbonCommand(RibbonItemSize.Large));
			return group;
		}
		public HETableRibbonTab CreateTableTab() {
			return TableTab;
		}
		protected HETableRibbonTab TableTab {
			get {
				if(tableTab == null) {
					tableTab = new HETableRibbonTab();
					tableTab.Groups.Add(TablesGroup);
					tableTab.Groups.Add(DeleteTableGroup);
					tableTab.Groups.Add(InsertTableGroup);
					tableTab.Groups.Add(MergeTableGroup);
					tableTab.Groups.Add(TablePropertiesGroup);
				}
				return tableTab;
			}
		}
		public HETableLayoutRibbonTab CreateTableLayoutTab() {
			return LayoutTableTab;
		}
		protected HETableLayoutRibbonTab LayoutTableTab {
			get {
				if(layoutTableTab == null) {
					layoutTableTab = new HETableLayoutRibbonTab();
					layoutTableTab.Groups.Add(DeleteTableGroup);
					layoutTableTab.Groups.Add(InsertTableGroup);
					layoutTableTab.Groups.Add(MergeTableGroup);
					layoutTableTab.Groups.Add(TablePropertiesGroup);
				}
				return layoutTableTab;
			}
		}
		public HETableToolsRibbonTabCategory CreateTableToolsTabCategory() {
			return TableToolsTabCategory;
		}
		protected HETableToolsRibbonTabCategory TableToolsTabCategory {
			get {
				if(tableToolsTabCategory == null) {
					tableToolsTabCategory = new HETableToolsRibbonTabCategory();
					tableToolsTabCategory.Tabs.Add(CreateTableLayoutTab());
				}
				return tableToolsTabCategory;
			}
		}
		protected HETablesRibbonGroup TablesGroup {
			get {
				if(tablesGroup == null)
					tablesGroup = CreateTablesGroup();
				return tablesGroup;
			}
		}
		public HETablesRibbonGroup CreateTablesGroup() {
			HETablesRibbonGroup group = new HETablesRibbonGroup();
			group.Items.Add(new HEInsertTableDropDownRibbonCommand(RibbonItemSize.Large));
			return group;
		}
		protected HEDeleteTableRibbonGroup DeleteTableGroup {
			get {
				if(deleteTableGroup == null)
					deleteTableGroup = CreateDeleteTableGroup();
				return deleteTableGroup;
			}
		}
		public HEDeleteTableRibbonGroup CreateDeleteTableGroup() {
			HEDeleteTableRibbonGroup group = new HEDeleteTableRibbonGroup();
			group.Items.Add(new HEDeleteTableRibbonCommand(RibbonItemSize.Large));
			group.Items.Add(new HEDeleteTableRowRibbonCommand(RibbonItemSize.Large));
			group.Items.Add(new HEDeleteTableColumnRibbonCommand(RibbonItemSize.Large));
			return group;
		}
		protected HEInsertTableRibbonGroup InsertTableGroup {
			get {
				if(insertTableGroup == null)
					insertTableGroup = CreateInsertTableGroup();
				return insertTableGroup;
			}
		}
		public HEInsertTableRibbonGroup CreateInsertTableGroup() {
			HEInsertTableRibbonGroup group = new HEInsertTableRibbonGroup();
			group.Items.Add(new HEInsertTableRowAboveRibbonCommand(RibbonItemSize.Large));
			group.Items.Add(new HEInsertTableRowBelowRibbonCommand(RibbonItemSize.Large));
			group.Items.Add(new HEInsertTableColumnToLeftRibbonCommand(RibbonItemSize.Large));
			group.Items.Add(new HEInsertTableColumnToRightRibbonCommand(RibbonItemSize.Large));
			return group;
		}
		protected HEMergeTableRibbonGroup MergeTableGroup {
			get {
				if(mergeTableGroup == null)
					mergeTableGroup = CreateMergeTableGroup();
				return mergeTableGroup;
			}
		}
		public HEMergeTableRibbonGroup CreateMergeTableGroup() {
			HEMergeTableRibbonGroup group = new HEMergeTableRibbonGroup();
			group.Items.Add(new HEMergeTableCellDownRibbonCommand(RibbonItemSize.Large));
			group.Items.Add(new HEMergeTableCellRightRibbonCommand());
			group.Items.Add(new HESplitTableCellHorizontallyRibbonCommand());
			group.Items.Add(new HESplitTableCellVerticallyRibbonCommand());
			return group;
		}
		protected HETablePropertiesRibbonGroup TablePropertiesGroup {
			get {
				if(tablePropertiesGroup == null)
					tablePropertiesGroup = CreateTablePropertiesGroup();
				return tablePropertiesGroup;
			}
		}
		public HETablePropertiesRibbonGroup CreateTablePropertiesGroup() {
			HETablePropertiesRibbonGroup group = new HETablePropertiesRibbonGroup();
			group.Items.Add(new HETablePropertiesRibbonCommand(RibbonItemSize.Large));
			group.Items.Add(new HETableRowPropertiesRibbonCommand());
			group.Items.Add(new HETableColumnPropertiesRibbonCommand());
			group.Items.Add(new HETableCellPropertiesRibbonCommand());
			return group;
		}
		protected HEEditingRibbonGroup EditingGroup {
			get {
				if(editingGroup == null)
					editingGroup = CreateEditingGroup();
				return editingGroup;
			}
		}
		public HEEditingRibbonGroup CreateEditingGroup() {
			HEEditingRibbonGroup group = new HEEditingRibbonGroup();
			group.Items.Add(new HEFindAndReplaceDialogRibbonCommand(RibbonItemSize.Large));
			return group;
		}
	}
}
