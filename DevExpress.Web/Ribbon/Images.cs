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
using System.Linq;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
using DevExpress.Web;
using DevExpress.Web.Internal;
using System.Web;
using System.Text.RegularExpressions;
namespace DevExpress.Web {
	public class RibbonImages : ImagesBase {
		const string PopOutImageName = "rPopOut";
		const string MinimizeButtonImageName = "rMinBtn";
		const string DialogBoxLauncherImageName = "rDialogBoxLauncher";
		const string GalleryUpButtonImageName = "rGlrUp";
		const string GalleryDownButtonImageName = "rGlrDown";
		const string GalleryPopOutImageName = "rGlrPopOut";
		internal const string FileTabBackgroundImageName = "rFileTabBack";
		internal const string FileTabHoverBackgroundImageName = "rFileTabHoverBack";
		internal const int DefaultSmallSize = 16;
		internal const int DefaultLargeSize = 32;
		Dictionary<string, RibbonImages> imagesDictionary;
		public RibbonImages(ISkinOwner owner)
			: base(owner) { }
		[
#if !SL
	DevExpressWebLocalizedDescription("RibbonImagesPopOut"),
#endif
		AutoFormatEnable, NotifyParentProperty(true), PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public ItemImageProperties PopOut {
			get { return (ItemImageProperties)GetImageBase(PopOutImageName); }
		}
		[AutoFormatEnable, NotifyParentProperty(true), PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public ItemImageProperties GalleryScrollUpButton {
			get { return (ItemImageProperties)GetImageBase(GalleryUpButtonImageName); }
		}
		[AutoFormatEnable, NotifyParentProperty(true), PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public ItemImageProperties GalleryScrollDownButton {
			get { return (ItemImageProperties)GetImageBase(GalleryDownButtonImageName); }
		}
		[AutoFormatEnable, NotifyParentProperty(true), PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public ItemImageProperties GalleryPopOutButton {
			get { return (ItemImageProperties)GetImageBase(GalleryPopOutImageName); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("RibbonImagesMinimizeButton"),
#endif
		AutoFormatEnable, NotifyParentProperty(true), PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public CheckedButtonImageProperties MinimizeButton {
			get { return (CheckedButtonImageProperties)GetImageBase(MinimizeButtonImageName); }
		}
		[AutoFormatEnable, NotifyParentProperty(true), PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public ItemImageProperties GroupDialogBoxLauncher {
			get { return (ItemImageProperties)GetImageBase(DialogBoxLauncherImageName); }
		}
		[AutoFormatEnable, NotifyParentProperty(true), DefaultValue(MenuIconSetType.NotSet)]
		public MenuIconSetType IconSet {
			get { return (MenuIconSetType)GetEnumProperty("IconSet", MenuIconSetType.NotSet); }
			set {
				SetEnumProperty("IconSet", MenuIconSetType.NotSet, value);
				UpdateImagesDictionary();
			}
		}
		public override void CopyFrom(ImagesBase source) {
			base.CopyFrom(source);
			if(source is RibbonImages)
				IconSet = ((RibbonImages)source).IconSet;
		}
		public override void Reset() {
			base.Reset();
			IconSet = MenuIconSetType.NotSet;
		}
		protected override void PopulateImageInfoList(List<ImageInfo> list) {
			base.PopulateImageInfoList(list);
			list.Add(new ImageInfo(PopOutImageName, ImageFlags.Empty, Unit.Empty, Unit.Empty, "po",
				typeof(ItemImageProperties), PopOutImageName));
			list.Add(new ImageInfo(MinimizeButtonImageName, ImageFlags.HasDisabledState | ImageFlags.HasHottrackState | ImageFlags.HasPressedState | ImageFlags.HasCheckedState, Unit.Empty, Unit.Empty, "minimize",
				typeof(CheckedButtonImageProperties), MinimizeButtonImageName));
			list.Add(new ImageInfo(DialogBoxLauncherImageName, ImageFlags.HasDisabledState | ImageFlags.HasHottrackState | ImageFlags.HasPressedState, Unit.Empty, Unit.Empty, "dialogboxlauncher",
				typeof(ItemImageProperties), DialogBoxLauncherImageName));
			list.Add(new ImageInfo(GalleryUpButtonImageName, ImageFlags.HasDisabledState | ImageFlags.HasHottrackState | ImageFlags.HasPressedState, Unit.Empty, Unit.Empty, "up",
				typeof(ItemImageProperties), GalleryUpButtonImageName));
			list.Add(new ImageInfo(GalleryDownButtonImageName, ImageFlags.HasDisabledState | ImageFlags.HasHottrackState | ImageFlags.HasPressedState, Unit.Empty, Unit.Empty, "down",
				typeof(ItemImageProperties), GalleryDownButtonImageName));
			list.Add(new ImageInfo(GalleryPopOutImageName, ImageFlags.HasDisabledState | ImageFlags.HasHottrackState | ImageFlags.HasPressedState, Unit.Empty, Unit.Empty, "gpo",
				typeof(ItemImageProperties), GalleryPopOutImageName));
		}
		protected internal ItemImageProperties GetPopOutImageProperties() {
			ItemImageProperties image = new ItemImageProperties();
			image.MergeWith(GetImageProperties(((ASPxWebControl)Owner).Page, PopOutImageName));
			image.MergeWith(PopOut);
			return image;
		}
		protected internal CheckedButtonImageProperties GetMinimizeButtonImageProperties() {
			CheckedButtonImageProperties image = new CheckedButtonImageProperties();
			image.MergeWith(GetImageProperties(((ASPxWebControl)Owner).Page, MinimizeButtonImageName));
			image.MergeWith(MinimizeButton);
			return image;
		}
		protected internal ItemImageProperties GetDialogBoxLauncherImageProperties() {
			ItemImageProperties image = new ItemImageProperties();
			image.MergeWith(GetImageProperties(((ASPxWebControl)Owner).Page, DialogBoxLauncherImageName));
			image.MergeWith(GroupDialogBoxLauncher);
			return image;
		}
		protected internal ItemImageProperties GetGalleryUpButtonImageProperties() {
			ItemImageProperties image = new ItemImageProperties();
			image.MergeWith(GetImageProperties(((ASPxWebControl)Owner).Page, GalleryUpButtonImageName));
			image.MergeWith(GalleryScrollUpButton);
			return image;
		}
		protected internal ItemImageProperties GetGalleryDownButtonImageProperties() {
			ItemImageProperties image = new ItemImageProperties();
			image.MergeWith(GetImageProperties(((ASPxWebControl)Owner).Page, GalleryDownButtonImageName));
			image.MergeWith(GalleryScrollDownButton);
			return image;
		}
		protected internal ItemImageProperties GetGalleryPopOutButtonImageProperties() {
			ItemImageProperties image = new ItemImageProperties();
			image.MergeWith(GetImageProperties(((ASPxWebControl)Owner).Page, GalleryPopOutImageName));
			image.MergeWith(GalleryPopOutButton);
			return image;
		}
		protected internal Dictionary<string, RibbonImages> ImagesDictionary {
			get {
				if(imagesDictionary == null)
					imagesDictionary = new Dictionary<string, RibbonImages>();
				return imagesDictionary;
			}
		}
		protected internal RibbonImages GetImagesBySpriteName(string spriteName, CreateRibbonImages create) {
			if(!ImagesDictionary.ContainsKey(spriteName)) {
				RibbonImages ribbonImages = create((ASPxWebControl)Owner);
				ImagesDictionary.Add(spriteName, ribbonImages);
				RegisterRibbonIconsHelper.AddResourcePathToRegister(ribbonImages.GetResourceSpriteCssPath(), spriteName);
			}
			return ImagesDictionary[spriteName];
		}
		void UpdateImagesDictionary() {
			foreach(string spriteName in ImagesDictionary.Keys) {
				RegisterRibbonIconsHelper.UnregisterResource(ImagesDictionary[spriteName].GetResourceSpriteCssPath());
				ImagesDictionary[spriteName].IconSet = IconSet;
				RegisterRibbonIconsHelper.AddResourcePathToRegister(ImagesDictionary[spriteName].GetResourceSpriteCssPath(), spriteName);
			}
		}
	}
	public class RibbonItemImageProperties : ItemImagePropertiesBase {
		public RibbonItemImageProperties(IPropertiesOwner owner)
			: base(owner) { }
		[
#if !SL
	DevExpressWebLocalizedDescription("RibbonItemImagePropertiesSpriteProperties"),
#endif
		PersistenceMode(PersistenceMode.InnerProperty), DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true), AutoFormatEnable]
		public ImageSpriteProperties SpriteProperties { get { return (ImageSpriteProperties)SpritePropertiesInternal; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override Unit Width { get { return base.Width; } set { base.Width = value; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override Unit Height { get { return base.Height; } set { base.Height = value; } }
	}
}
namespace DevExpress.Web.Internal {
	public abstract class ControlRibbonImages : RibbonImages {
		public ControlRibbonImages(ISkinOwner properties, MenuIconSetType iconSetStyle)
			: base(properties) {
			IconSet = iconSetStyle;
		}
		public override string ToString() {
			return string.Empty;
		}
		protected override bool KeepDefaultSizes {
			get { return true; }
		}
		protected override string GetCssPostFix() {
			if(IconSet == MenuIconSetType.NotSet)
				return base.GetCssPostFix();
			return string.Empty;
		}
		protected abstract Dictionary<MenuIconSetType, string> Categories { get; }
		protected abstract Dictionary<MenuIconSetType, string> SpriteNames { get; }
		protected override string GetImageCategory() {
			if(Categories.ContainsKey(IconSet))
				return Categories[IconSet];
			return base.GetImageCategory();
		}
		protected override string GetDesignTimeResourceSpriteImagePath() {
			if(SpriteNames.ContainsKey(IconSet))
				return ASPxWebControl.WebImagesResourcePath + SpriteNames[IconSet] + ".png";
			return base.GetDesignTimeResourceSpriteImagePath();
		}
		protected override string GetResourceSpriteCssPath() {
			if(SpriteNames.ContainsKey(IconSet))
				return ASPxWebControl.WebCssResourcePath + SpriteNames[IconSet] + ".css";
			return base.GetResourceSpriteCssPath();
		}
		protected override string GetSpriteImageFileName() {
			if(SpriteNames.ContainsKey(IconSet))
				return SpriteNames[IconSet];
			return base.GetSpriteImageFileName();
		}
		protected internal string GetSpriteNameByIconSet(MenuIconSetType iconSet) {
			return SpriteNames != null && SpriteNames.ContainsKey(IconSet) ? SpriteNames[IconSet] : string.Empty;
		}
		protected internal virtual string GetDefaultSpriteName() {
			return string.Empty;
		}
	}
	public enum HEExportFormat {
		Rtf,
		Mht,
		Odt,
		Docx,
		Txt,
		Pdf
	}
	public class HERibbonImages : ControlRibbonImages {
		public const string RibbonHESpriteName = "HESprite";
		public const string RibbonHEGSpriteName = "HEGSprite";
		public const string RibbonHEGWSpriteName = "HEGWSprite";
		private const int ColorIconHeight = 13;
		public const string Cut = "heCut",
							CutLarge = "heCutLarge",
							Copy = "heCopy",
							CopyLarge = "heCopyLarge",
							Paste = "hePaste",
							PasteLarge = "hePasteLarge",
							PasteFromWord = "hePasteFromWord",
							PasteFromWordLarge = "hePasteFromWordLarge",
							Undo = "heUndo",
							UndoLarge = "heUndoLarge",
							Redo = "heRedo",
							RedoLarge = "heRedoLarge",
							Bold = "heBold",
							BoldLarge = "heBoldLarge",
							Italic = "heItalic",
							ItalicLarge = "heItalicLarge",
							Underline = "heUnderline",
							UnderlineLarge = "heUnderlineLarge",
							Strikethrough = "heStrikethrough",
							StrikethroughLarge = "heStrikethroughLarge",
							Center = "heCenter",
							CenterLarge = "heCenterLarge",
							Left = "heLeft",
							LeftLarge = "heLeftLarge",
							Right = "heRight",
							RightLarge = "heRightLarge",
							Justify = "heJustifyFull",
							JustifyLarge = "heJustifyFullLarge",
							Superscript = "heSuperscript",
							SuperscriptLarge = "heSuperscriptLarge",
							Subscript = "heSubscript",
							SubscriptLarge = "heSubscriptLarge",
							Indent = "heIndent",
							IndentLarge = "heIndentLarge",
							Outdent = "heOutdent",
							OutdentLarge = "heOutdentLarge",
							OrderedList = "heInsertOrderedList",
							OrderedListLarge = "heInsertOrderedListLarge",
							UnorderedList = "heInsertUnorderedList",
							UnorderedListLarge = "heInsertUnorderedListLarge",
							InsertImageDialog = "heInsertImageDialog",
							InsertImageDialogLarge = "heInsertImageDialogLarge",
							InsertLinkDialog = "heInsertLinkDialog",
							InsertLinkDialogLarge = "heInsertLinkDialogLarge",
							InsertTableDialog = "heInsertTableDialog",
							InsertTableDialogLarge = "heInsertTableDialogLarge",
							TablePropertiesDialog = "heChangeTableDialog",
							TablePropertiesDialogLarge = "heChangeTableDialogLarge",
							TableCellPropertiesDialog = "heChangeTableCellDialog",
							TableCellPropertiesDialogLarge = "heChangeTableCellDialogLarge",
							TableColumnPropertiesDialog = "heChangeTableColumnDialog",
							TableColumnPropertiesDialogLarge = "heChangeTableColumnDialogLarge",
							TableRowPropertiesDialog = "heChangeTableRowDialog",
							TableRowPropertiesDialogLarge = "heChangeTableRowDialogLarge",
							InsertTableRowAbove = "heInsertTableRowAbove",
							InsertTableRowAboveLarge = "heInsertTableRowAboveLarge",
							InsertTableRowBelow = "heInsertTableRowBelow",
							InsertTableRowBelowLarge = "heInsertTableRowBelowLarge",
							InsertTableColumnToLeft = "heInsertTableColumnOnLeft",
							InsertTableColumnToLeftLarge = "heInsertTableColumnOnLeftLarge",
							InsertTableColumnToRight = "heInsertTableColumnOnRight",
							InsertTableColumnToRightLarge = "heInsertTableColumnOnRightLarge",
							SplitTableCellHorizontal = "heSplitTableCellHorizontal",
							SplitTableCellHorizontalLarge = "heSplitTableCellHorizontalLarge",
							SplitTableCellVertical = "heSplitTableCellVertical",
							SplitTableCellVerticalLarge = "heSplitTableCellVerticalLarge",
							MergeTableCellHorizontal = "heMergeTableCellHorizontal",
							MergeTableCellHorizontalLarge = "heMergeTableCellHorizontalLarge",
							MergeTableCellDown = "heMergeTableCellVertical",
							MergeTableCellDownLarge = "heMergeTableCellVerticalLarge",
							DeleteTable = "heDeleteTable",
							DeleteTableLarge = "heDeleteTableLarge",
							DeleteTableRow = "heDeleteTableRow",
							DeleteTableRowLarge = "heDeleteTableRowLarge",
							DeleteTableColumn = "heDeleteTableColumn",
							DeleteTableColumnLarge = "heDeleteTableColumnLarge",
							UnLink = "heUnlink",
							UnLinkLarge = "heUnlinkLarge",
							BackColor = "heBackColor",
							BackColorLarge = "heBackColorLarge",
							ForeColor = "heForeColor",
							ForeColorLarge = "heForeColorLarge",
							RemoveFormat = "heRemoveFormat",
							RemoveFormatLarge = "heRemoveFormatLarge",
							CheckSpelling = "heCheckSpelling",
							CheckSpellingLarge = "heCheckSpellingLarge",
							Print = "hePrint",
							PrintLarge = "hePrintLarge",
							Fullscreen = "heFullscreen",
							FullscreenLarge = "heFullscreenLarge",
							ExportFormat = "heSaveAs{0}",
							ExportFormatLarge = "heSaveAs{0}Large",
							InsertAudioDialog = "heInsertAudioDialog",
							InsertAudioDialogLarge = "heInsertAudioDialogLarge",
							InsertVideoDialog = "heInsertVideoDialog",
							InsertVideoDialogLarge = "heInsertVideoDialogLarge",
							InsertFlashDialog = "heInsertFlashDialog",
							InsertFlashDialogLarge = "heInsertFlashDialogLarge",
							InsertYouTubeVideoDialog = "heInsertYouTubeDialog",
							InsertYouTubeVideoDialogLarge = "heInsertYouTubeDialogLarge",
							InsertPlaceholderDialog = "heInsertPlaceholder",
							InsertPlaceholderDialogLarge = "heInsertPlaceholderLarge",
							SelectAll = "heSelectAll",
							SelectAllLarge = "heSelectAllLarge",
							FindAndReplaceDialog = "heFindAndReplaceDialog",
							FindAndReplaceDialogLarge = "heFindAndReplaceDialogLarge",
							ViewsGroup = "heViewsGroup",
							FontGroup = "heFontGroup",
							MediaGroup = "heMediaGroup";
		internal readonly static Dictionary<MenuIconSetType, string> CategoryDictionary = new Dictionary<MenuIconSetType, string>();
		internal readonly static Dictionary<MenuIconSetType, string> SpriteNameDictionary = new Dictionary<MenuIconSetType, string>();
		static HERibbonImages() {
			CategoryDictionary.Add(MenuIconSetType.NotSet, "heRibbonIcons");
			CategoryDictionary.Add(MenuIconSetType.Colored, "heRibbonIcons");
			CategoryDictionary.Add(MenuIconSetType.ColoredLight, "heRibbonIcons");
			CategoryDictionary.Add(MenuIconSetType.GrayScaled, "heRibbonGIcons");
			CategoryDictionary.Add(MenuIconSetType.GrayScaledWithWhiteHottrack, "heRibbonGWIcons");
			SpriteNameDictionary.Add(MenuIconSetType.NotSet, RibbonHESpriteName);
			SpriteNameDictionary.Add(MenuIconSetType.Colored, RibbonHESpriteName);
			SpriteNameDictionary.Add(MenuIconSetType.ColoredLight, RibbonHESpriteName);
			SpriteNameDictionary.Add(MenuIconSetType.GrayScaled, RibbonHEGSpriteName);
			SpriteNameDictionary.Add(MenuIconSetType.GrayScaledWithWhiteHottrack, RibbonHEGWSpriteName);
		}
		public HERibbonImages(ISkinOwner properties, MenuIconSetType iconSetStyle)
			: base(properties, iconSetStyle) {
		}
		protected override Dictionary<MenuIconSetType, string> Categories { get { return CategoryDictionary; } }
		protected override Dictionary<MenuIconSetType, string> SpriteNames { get { return SpriteNameDictionary; } }
		protected override void PopulateImageInfoList(List<ImageInfo> list) {
			base.PopulateImageInfoList(list);
			ImageFlags imageFlags = ImageFlags.IsPng | ImageFlags.HasDisabledState;
			list.Add(new ImageInfo(Cut, imageFlags, DefaultSmallSize, Cut));
			list.Add(new ImageInfo(CutLarge, imageFlags, DefaultLargeSize, CutLarge));
			list.Add(new ImageInfo(Copy, imageFlags, DefaultSmallSize, Copy));
			list.Add(new ImageInfo(CopyLarge, imageFlags, DefaultLargeSize, CopyLarge));
			list.Add(new ImageInfo(Paste, imageFlags, DefaultSmallSize, Paste));
			list.Add(new ImageInfo(PasteLarge, imageFlags, DefaultLargeSize, PasteLarge));
			list.Add(new ImageInfo(PasteFromWord, imageFlags, DefaultSmallSize, PasteFromWord));
			list.Add(new ImageInfo(PasteFromWordLarge, imageFlags, DefaultLargeSize, PasteFromWordLarge));
			list.Add(new ImageInfo(Undo, imageFlags, DefaultSmallSize, Undo));
			list.Add(new ImageInfo(UndoLarge, imageFlags, DefaultLargeSize, UndoLarge));
			list.Add(new ImageInfo(Redo, imageFlags, DefaultSmallSize, Redo));
			list.Add(new ImageInfo(RedoLarge, imageFlags, DefaultLargeSize, RedoLarge));
			list.Add(new ImageInfo(Bold, imageFlags, DefaultSmallSize, Bold));
			list.Add(new ImageInfo(BoldLarge, imageFlags, DefaultLargeSize, BoldLarge));
			list.Add(new ImageInfo(Italic, imageFlags, DefaultSmallSize, Italic));
			list.Add(new ImageInfo(ItalicLarge, imageFlags, DefaultLargeSize, ItalicLarge));
			list.Add(new ImageInfo(Underline, imageFlags, DefaultSmallSize, Underline));
			list.Add(new ImageInfo(UnderlineLarge, imageFlags, DefaultLargeSize, UnderlineLarge));
			list.Add(new ImageInfo(Strikethrough, imageFlags, DefaultSmallSize, Strikethrough));
			list.Add(new ImageInfo(StrikethroughLarge, imageFlags, DefaultLargeSize, StrikethroughLarge));
			list.Add(new ImageInfo(Center, imageFlags, DefaultSmallSize, Center));
			list.Add(new ImageInfo(CenterLarge, imageFlags, DefaultLargeSize, CenterLarge));
			list.Add(new ImageInfo(Left, imageFlags, DefaultSmallSize, Left));
			list.Add(new ImageInfo(LeftLarge, imageFlags, DefaultLargeSize, LeftLarge));
			list.Add(new ImageInfo(Right, imageFlags, DefaultSmallSize, Right));
			list.Add(new ImageInfo(RightLarge, imageFlags, DefaultLargeSize, RightLarge));
			list.Add(new ImageInfo(Justify, imageFlags, DefaultSmallSize, Justify));
			list.Add(new ImageInfo(JustifyLarge, imageFlags, DefaultLargeSize, JustifyLarge));
			list.Add(new ImageInfo(Superscript, imageFlags, DefaultSmallSize, Superscript));
			list.Add(new ImageInfo(SuperscriptLarge, imageFlags, DefaultLargeSize, SuperscriptLarge));
			list.Add(new ImageInfo(Subscript, imageFlags, DefaultSmallSize, Subscript));
			list.Add(new ImageInfo(SubscriptLarge, imageFlags, DefaultLargeSize, SubscriptLarge));
			list.Add(new ImageInfo(Indent, imageFlags, DefaultSmallSize, Indent));
			list.Add(new ImageInfo(IndentLarge, imageFlags, DefaultLargeSize, IndentLarge));
			list.Add(new ImageInfo(Outdent, imageFlags, DefaultSmallSize, Outdent));
			list.Add(new ImageInfo(OutdentLarge, imageFlags, DefaultLargeSize, OutdentLarge));
			list.Add(new ImageInfo(OrderedList, imageFlags, DefaultSmallSize, OrderedList));
			list.Add(new ImageInfo(OrderedListLarge, imageFlags, DefaultLargeSize, OrderedListLarge));
			list.Add(new ImageInfo(UnorderedList, imageFlags, DefaultSmallSize, UnorderedList));
			list.Add(new ImageInfo(UnorderedListLarge, imageFlags, DefaultLargeSize, UnorderedListLarge));
			list.Add(new ImageInfo(InsertImageDialog, imageFlags, DefaultSmallSize, InsertImageDialog));
			list.Add(new ImageInfo(InsertImageDialogLarge, imageFlags, DefaultLargeSize, InsertImageDialogLarge));
			list.Add(new ImageInfo(InsertLinkDialog, imageFlags, DefaultSmallSize, InsertLinkDialog));
			list.Add(new ImageInfo(InsertLinkDialogLarge, imageFlags, DefaultLargeSize, InsertLinkDialogLarge));
			list.Add(new ImageInfo(InsertTableDialog, imageFlags, DefaultSmallSize, InsertTableDialog));
			list.Add(new ImageInfo(InsertTableDialogLarge, imageFlags, DefaultLargeSize, InsertTableDialogLarge));
			list.Add(new ImageInfo(TablePropertiesDialog, imageFlags, DefaultSmallSize, TablePropertiesDialog));
			list.Add(new ImageInfo(TablePropertiesDialogLarge, imageFlags, DefaultLargeSize, TablePropertiesDialogLarge));
			list.Add(new ImageInfo(TableCellPropertiesDialog, imageFlags, DefaultSmallSize, TableCellPropertiesDialog));
			list.Add(new ImageInfo(TableCellPropertiesDialogLarge, imageFlags, DefaultLargeSize, TableCellPropertiesDialogLarge));
			list.Add(new ImageInfo(TableColumnPropertiesDialog, imageFlags, DefaultSmallSize, TableColumnPropertiesDialog));
			list.Add(new ImageInfo(TableColumnPropertiesDialogLarge, imageFlags, DefaultLargeSize, TableColumnPropertiesDialogLarge));
			list.Add(new ImageInfo(TableRowPropertiesDialog, imageFlags, DefaultSmallSize, TableRowPropertiesDialog));
			list.Add(new ImageInfo(TableRowPropertiesDialogLarge, imageFlags, DefaultLargeSize, TableRowPropertiesDialogLarge));
			list.Add(new ImageInfo(InsertTableRowAbove, imageFlags, DefaultSmallSize, InsertTableRowAbove));
			list.Add(new ImageInfo(InsertTableRowAboveLarge, imageFlags, DefaultLargeSize, InsertTableRowAboveLarge));
			list.Add(new ImageInfo(InsertTableRowBelow, imageFlags, DefaultSmallSize, InsertTableRowBelow));
			list.Add(new ImageInfo(InsertTableRowBelowLarge, imageFlags, DefaultLargeSize, InsertTableRowBelowLarge));
			list.Add(new ImageInfo(InsertTableColumnToLeft, imageFlags, DefaultSmallSize, InsertTableColumnToLeft));
			list.Add(new ImageInfo(InsertTableColumnToLeftLarge, imageFlags, DefaultLargeSize, InsertTableColumnToLeftLarge));
			list.Add(new ImageInfo(InsertTableColumnToRight, imageFlags, DefaultSmallSize, InsertTableColumnToRight));
			list.Add(new ImageInfo(InsertTableColumnToRightLarge, imageFlags, DefaultLargeSize, InsertTableColumnToRightLarge));
			list.Add(new ImageInfo(SplitTableCellHorizontal, imageFlags, DefaultSmallSize, SplitTableCellHorizontal));
			list.Add(new ImageInfo(SplitTableCellHorizontalLarge, imageFlags, DefaultLargeSize, SplitTableCellHorizontalLarge));
			list.Add(new ImageInfo(SplitTableCellVertical, imageFlags, DefaultSmallSize, SplitTableCellVertical));
			list.Add(new ImageInfo(SplitTableCellVerticalLarge, imageFlags, DefaultLargeSize, SplitTableCellVerticalLarge));
			list.Add(new ImageInfo(MergeTableCellHorizontal, imageFlags, DefaultSmallSize, MergeTableCellHorizontal));
			list.Add(new ImageInfo(MergeTableCellHorizontalLarge, imageFlags, DefaultLargeSize, MergeTableCellHorizontalLarge));
			list.Add(new ImageInfo(MergeTableCellDown, imageFlags, DefaultSmallSize, MergeTableCellDown));
			list.Add(new ImageInfo(MergeTableCellDownLarge, imageFlags, DefaultLargeSize, MergeTableCellDownLarge));
			list.Add(new ImageInfo(DeleteTable, imageFlags, DefaultSmallSize, DeleteTable));
			list.Add(new ImageInfo(DeleteTableLarge, imageFlags, DefaultLargeSize, DeleteTableLarge));
			list.Add(new ImageInfo(DeleteTableColumn, imageFlags, DefaultSmallSize, DeleteTableColumn));
			list.Add(new ImageInfo(DeleteTableColumnLarge, imageFlags, DefaultLargeSize, DeleteTableColumnLarge));
			list.Add(new ImageInfo(DeleteTableRow, imageFlags, DefaultSmallSize, DeleteTableRow));
			list.Add(new ImageInfo(DeleteTableRowLarge, imageFlags, DefaultLargeSize, DeleteTableRowLarge));
			list.Add(new ImageInfo(UnLink, imageFlags, DefaultSmallSize, UnLink));
			list.Add(new ImageInfo(UnLinkLarge, imageFlags, DefaultLargeSize, UnLinkLarge));
			list.Add(new ImageInfo(BackColor, imageFlags, DefaultSmallSize, ColorIconHeight, BackColor));
			list.Add(new ImageInfo(BackColorLarge, imageFlags, DefaultLargeSize, ColorIconHeight, BackColorLarge));
			list.Add(new ImageInfo(ForeColor, imageFlags, DefaultSmallSize, ColorIconHeight, ForeColor));
			list.Add(new ImageInfo(ForeColorLarge, imageFlags, DefaultLargeSize, ColorIconHeight, ForeColorLarge));
			list.Add(new ImageInfo(RemoveFormat, imageFlags, DefaultSmallSize, RemoveFormat));
			list.Add(new ImageInfo(RemoveFormatLarge, imageFlags, DefaultLargeSize, RemoveFormatLarge));
			list.Add(new ImageInfo(Print, imageFlags, DefaultSmallSize, Print));
			list.Add(new ImageInfo(PrintLarge, imageFlags, DefaultLargeSize, PrintLarge));
			list.Add(new ImageInfo(CheckSpelling, imageFlags, DefaultSmallSize, CheckSpelling));
			list.Add(new ImageInfo(CheckSpellingLarge, imageFlags, DefaultLargeSize, CheckSpellingLarge));
			list.Add(new ImageInfo(Fullscreen, imageFlags, DefaultSmallSize, Fullscreen));
			list.Add(new ImageInfo(FullscreenLarge, imageFlags, DefaultLargeSize, FullscreenLarge));
			list.Add(new ImageInfo(InsertAudioDialog, imageFlags, DefaultSmallSize, InsertAudioDialog));
			list.Add(new ImageInfo(InsertAudioDialogLarge, imageFlags, DefaultLargeSize, InsertAudioDialogLarge));
			list.Add(new ImageInfo(InsertVideoDialog, imageFlags, DefaultSmallSize, InsertVideoDialog));
			list.Add(new ImageInfo(InsertVideoDialogLarge, imageFlags, DefaultLargeSize, InsertVideoDialogLarge));
			list.Add(new ImageInfo(InsertFlashDialog, imageFlags, DefaultSmallSize, InsertFlashDialog));
			list.Add(new ImageInfo(InsertFlashDialogLarge, imageFlags, DefaultLargeSize, InsertFlashDialogLarge));
			list.Add(new ImageInfo(InsertYouTubeVideoDialog, imageFlags, DefaultSmallSize, InsertYouTubeVideoDialog));
			list.Add(new ImageInfo(InsertYouTubeVideoDialogLarge, imageFlags, DefaultLargeSize, InsertYouTubeVideoDialogLarge));
			list.Add(new ImageInfo(InsertPlaceholderDialog, imageFlags, DefaultSmallSize, InsertPlaceholderDialog));
			list.Add(new ImageInfo(InsertPlaceholderDialogLarge, imageFlags, DefaultLargeSize, InsertPlaceholderDialogLarge));
			list.Add(new ImageInfo(SelectAll, imageFlags, DefaultSmallSize, SelectAll));
			list.Add(new ImageInfo(SelectAllLarge, imageFlags, DefaultLargeSize, SelectAllLarge));
			list.Add(new ImageInfo(FindAndReplaceDialog, imageFlags, DefaultSmallSize, FindAndReplaceDialog));
			list.Add(new ImageInfo(FindAndReplaceDialogLarge, imageFlags, DefaultLargeSize, FindAndReplaceDialogLarge));
			list.Add(new ImageInfo(ViewsGroup, imageFlags, DefaultLargeSize, ViewsGroup));
			list.Add(new ImageInfo(FontGroup, imageFlags, DefaultLargeSize, FontGroup));
			list.Add(new ImageInfo(MediaGroup, imageFlags, DefaultLargeSize, MediaGroup));
			foreach(string writeFormat in Enum.GetNames(typeof(HEExportFormat))) {
				string resourceName = GetExportFormatResourceName(writeFormat);
				list.Add(new ImageInfo(resourceName, imageFlags, DefaultSmallSize, resourceName));
				resourceName = GetExportLargeFormatResourceName(writeFormat);
				list.Add(new ImageInfo(resourceName, imageFlags, DefaultLargeSize, resourceName));
			}
		}
		internal static string GetExportFormatResourceName(string format) {
			return string.Format(ExportFormat, format);
		}
		internal static string GetExportLargeFormatResourceName(string format) {
			return string.Format(ExportFormatLarge, format);
		}
		internal static string GetExportFormatResourceName(HEExportFormat format) {
			return GetExportFormatResourceName(format.ToString());
		}
		protected internal override string GetDefaultSpriteName() {
			return RibbonHESpriteName;
		}
	}
	public class DocumentViewerRibbonImages : ControlRibbonImages {
		public const string RibbonDVSpriteName = "DVSprite";
		public const string RibbonDVGSpriteName = "DVGSprite";
		public const string RibbonDVGWSpriteName = "DVGWSprite";
		ImageFlags imageFlags = ImageFlags.HasDisabledState | ImageFlags.IsPng;
		public const string
			Search = "BtnSearch",
			Print = "BtnPrint",
			PrintPage = "BtnPrintPage",
			FirstPage = "BtnFirstPage",
			PrevPage = "BtnPrevPage",
			NextPage = "BtnNextPage",
			LastPage = "BtnLastPage",
			SaveToDisk = "BtnSave",
			SaveToWindow = "BtnSaveWindow",
			Parameters = "BtnParameters",
			DocumentMap = "BtnDocumentMap",
			ExportPdf = "ExportToPdf",
			ExportXls = "ExportToXls",
			ExportXlsx = "ExportToXlsx",
			ExportRtf = "ExportToRtf",
			ExportMht = "ExportToMht",
			ExportHtml = "ExportToHtml",
			ExportText = "ExportToTxt",
			ExportCsv = "ExportToCsv",
			ExportPng = "ExportToImg";
		internal const string
			SearchLarge = "BtnSearchLarge",
			PrintLarge = "BtnPrintLarge",
			PrintPageLarge = "BtnPrintPageLarge",
			FirstPageLarge = "BtnFirstPageLarge",
			PrevPageLarge = "BtnPrevPageLarge",
			NextPageLarge = "BtnNextPageLarge",
			LastPageLarge = "BtnLastPageLarge",
			SaveToDiskLarge = "BtnSaveLarge",
			SaveToWindowLarge = "BtnSaveWindowLarge",
			ParametersLarge = "BtnParametersLarge",
			DocumentMapLarge = "BtnDocumentMapLarge",
			ExportPdfLarge = "ExportToPdfLarge",
			ExportXlsLarge = "ExportToXlsLarge",
			ExportXlsxLarge = "ExportToXlsxLarge",
			ExportRtfLarge = "ExportToRtfLarge",
			ExportMhtLarge = "ExportToMhtLarge",
			ExportHtmlLarge = "ExportToHtmlLarge",
			ExportTextLarge = "ExportToTxtLarge",
			ExportCsvLarge = "ExportToCsvLarge",
			ExportPngLarge = "ExportToImgLarge";
		internal readonly static Dictionary<MenuIconSetType, string> CategoryDictionary = new Dictionary<MenuIconSetType, string>();
		internal readonly static Dictionary<MenuIconSetType, string> SpriteNameDictionary = new Dictionary<MenuIconSetType, string>();
		static DocumentViewerRibbonImages() {
			CategoryDictionary.Add(MenuIconSetType.NotSet, "xrdvRibbonIcon");
			CategoryDictionary.Add(MenuIconSetType.Colored, "xrdvRibbonIcon");
			CategoryDictionary.Add(MenuIconSetType.ColoredLight, "xrdvRibbonIcon");
			CategoryDictionary.Add(MenuIconSetType.GrayScaled, "xrdvRibbonGIcon");
			CategoryDictionary.Add(MenuIconSetType.GrayScaledWithWhiteHottrack, "xrdvRibbonGWIcon");
			SpriteNameDictionary.Add(MenuIconSetType.NotSet, RibbonDVSpriteName);
			SpriteNameDictionary.Add(MenuIconSetType.Colored, RibbonDVSpriteName);
			SpriteNameDictionary.Add(MenuIconSetType.ColoredLight, RibbonDVSpriteName);
			SpriteNameDictionary.Add(MenuIconSetType.GrayScaled, RibbonDVGSpriteName);
			SpriteNameDictionary.Add(MenuIconSetType.GrayScaledWithWhiteHottrack, RibbonDVGWSpriteName);
		}
		public DocumentViewerRibbonImages(ISkinOwner properties, MenuIconSetType iconSetStyle)
			: base(properties, iconSetStyle) {
		}
		protected override Dictionary<MenuIconSetType, string> Categories { get { return CategoryDictionary; } }
		protected override Dictionary<MenuIconSetType, string> SpriteNames { get { return SpriteNameDictionary; } }
		protected override void PopulateImageInfoList(List<ImageInfo> list) {
			base.PopulateImageInfoList(list);
			AddItemToImageInfoList(list, Search, DefaultSmallSize);
			AddItemToImageInfoList(list, Print, DefaultSmallSize);
			AddItemToImageInfoList(list, PrintPage, DefaultSmallSize);
			AddItemToImageInfoList(list, FirstPage, DefaultSmallSize);
			AddItemToImageInfoList(list, PrevPage, DefaultSmallSize);
			AddItemToImageInfoList(list, NextPage, DefaultSmallSize);
			AddItemToImageInfoList(list, LastPage, DefaultSmallSize);
			AddItemToImageInfoList(list, SaveToDisk, DefaultSmallSize);
			AddItemToImageInfoList(list, SaveToWindow, DefaultSmallSize);
			AddItemToImageInfoList(list, Parameters, DefaultSmallSize);
			AddItemToImageInfoList(list, DocumentMap, DefaultSmallSize);
			AddItemToImageInfoList(list, ExportPdf, DefaultSmallSize);
			AddItemToImageInfoList(list, ExportXls, DefaultSmallSize);
			AddItemToImageInfoList(list, ExportXlsx, DefaultSmallSize);
			AddItemToImageInfoList(list, ExportRtf, DefaultSmallSize);
			AddItemToImageInfoList(list, ExportMht, DefaultSmallSize);
			AddItemToImageInfoList(list, ExportHtml, DefaultSmallSize);
			AddItemToImageInfoList(list, ExportText, DefaultSmallSize);
			AddItemToImageInfoList(list, ExportCsv, DefaultSmallSize);
			AddItemToImageInfoList(list, ExportPng, DefaultSmallSize);
			AddItemToImageInfoList(list, SearchLarge, DefaultLargeSize);
			AddItemToImageInfoList(list, PrintLarge, DefaultLargeSize);
			AddItemToImageInfoList(list, PrintPageLarge, DefaultLargeSize);
			AddItemToImageInfoList(list, FirstPageLarge, DefaultLargeSize);
			AddItemToImageInfoList(list, PrevPageLarge, DefaultLargeSize);
			AddItemToImageInfoList(list, NextPageLarge, DefaultLargeSize);
			AddItemToImageInfoList(list, LastPageLarge, DefaultLargeSize);
			AddItemToImageInfoList(list, SaveToDiskLarge, DefaultLargeSize);
			AddItemToImageInfoList(list, SaveToWindowLarge, DefaultLargeSize);
			AddItemToImageInfoList(list, ParametersLarge, DefaultLargeSize);
			AddItemToImageInfoList(list, DocumentMapLarge, DefaultLargeSize);
			AddItemToImageInfoList(list, ExportPdfLarge, DefaultLargeSize);
			AddItemToImageInfoList(list, ExportXlsLarge, DefaultLargeSize);
			AddItemToImageInfoList(list, ExportXlsxLarge, DefaultLargeSize);
			AddItemToImageInfoList(list, ExportRtfLarge, DefaultLargeSize);
			AddItemToImageInfoList(list, ExportMhtLarge, DefaultLargeSize);
			AddItemToImageInfoList(list, ExportHtmlLarge, DefaultLargeSize);
			AddItemToImageInfoList(list, ExportTextLarge, DefaultLargeSize);
			AddItemToImageInfoList(list, ExportCsvLarge, DefaultLargeSize);
			AddItemToImageInfoList(list, ExportPngLarge, DefaultLargeSize);
		}
		void AddItemToImageInfoList(List<ImageInfo> list, string name, int size) {
			list.Add(new ImageInfo(name, imageFlags, typeof(ItemImageProperties), name) { Height = size, Width = size });
		}
		protected internal override string GetDefaultSpriteName() {
			return RibbonDVSpriteName;
		}
	}
	public class SpreadsheetRibbonImages : ControlRibbonImages {
		public const string RibbonSSSpriteName = "SSSprite";
		public const string RibbonSSGSpriteName = "SSGSprite";
		public const string RibbonSSGWSpriteName = "SSGWSprite";
		#region ImageNames
		protected internal const string
								AlignBottomCenter = "AlignBottomCenter",
								AlignBottomCenterLarge = "AlignBottomCenterLarge",
								AlignCenter = "AlignCenter",
								AlignCenterLarge = "AlignCenterLarge",
								AlignJustify = "AlignJustify",
								AlignJustifyLarge = "AlignJustifyLarge",
								AlignLeft = "AlignLeft",
								AlignLeftLarge = "AlignLeftLarge",
								AlignMiddleCenter = "AlignMiddleCenter",
								AlignMiddleCenterLarge = "AlignMiddleCenterLarge",
								AlignRight = "AlignRight",
								AlignRightLarge = "AlignRightLarge",
								AlignTopCenter = "AlignTopCenter",
								AlignTopCenterLarge = "AlignTopCenterLarge",
								Background = "Background",
								BackgroundLarge = "BackgroundLarge",
								Bold = "Bold",
								BoldLarge = "BoldLarge",
								BorderBottom = "BorderBottom",
								BorderBottomLarge = "BorderBottomLarge",
								BorderBottomThick = "BorderBottomThick",
								BorderBottomThickLarge = "BorderBottomThickLarge",
								BorderLeft = "BorderLeft",
								BorderLeftLarge = "BorderLeftLarge",
								BorderNone = "BorderNone",
								BorderNoneLarge = "BorderNoneLarge",
								BorderRight = "BorderRight",
								BorderRightLarge = "BorderRightLarge",
								BordersAll = "BordersAll",
								BordersAllLarge = "BordersAllLarge",
								BordersOutside = "BordersOutside",
								BordersOutsideLarge = "BordersOutsideLarge",
								BordersOutsideThick = "BordersOutsideThick",
								BordersOutsideThickLarge = "BordersOutsideThickLarge",
								BorderTop = "BorderTop",
								BorderTopAndBottom = "BorderTopAndBottom",
								BorderTopAndBottomLarge = "BorderTopAndBottomLarge",
								BorderTopAndBottomThick = "BorderTopAndBottomThick",
								BorderTopAndBottomThickLarge = "BorderTopAndBottomThickLarge",
								BorderTopLarge = "BorderTopLarge",
								CircleInvalidData = "CircleInvalidData",
								CircleInvalidDataLarge = "CircleInvalidDataLarge",
								ClearAll = "ClearAll",
								ClearAllLarge = "ClearAllLarge",
								ClearFormats = "ClearFormats",
								ClearFormatsLarge = "ClearFormatsLarge",
								ClearValidationCircles = "ClearValidationCircles",
								ClearValidationCirclesLarge = "ClearValidationCirclesLarge",
								Copy = "Copy",
								CopyLarge = "CopyLarge",
								CreateArea3DChart = "CreateArea3DChart",
								CreateArea3DChartLarge = "CreateArea3DChartLarge",
								CreateAreaChart = "CreateAreaChart",
								CreateAreaChartLarge = "CreateAreaChartLarge",
								CreateBar3DChart = "CreateBar3DChart",
								CreateBar3DChartLarge = "CreateBar3DChartLarge",
								CreateBarChart = "CreateBarChart",
								CreateBarChartLarge = "CreateBarChartLarge",
								CreateBubbleChart = "CreateBubbleChart",
								CreateBubbleChartLarge = "CreateBubbleChartLarge",
								CreateConeBar3DChart = "CreateConeBar3DChart",
								CreateConeBar3DChartLarge = "CreateConeBar3DChartLarge",
								CreateConeFullStackedBar3DChart = "CreateConeFullStackedBar3DChart",
								CreateConeFullStackedBar3DChartLarge = "CreateConeFullStackedBar3DChartLarge",
								CreateConeManhattanBarChart = "CreateConeManhattanBarChart",
								CreateConeManhattanBarChartLarge = "CreateConeManhattanBarChartLarge",
								CreateConeStackedBar3DChart = "CreateConeStackedBar3DChart",
								CreateConeStackedBar3DChartLarge = "CreateConeStackedBar3DChartLarge",
								CreateCylinderBar3DChart = "CreateCylinderBar3DChart",
								CreateCylinderBar3DChartLarge = "CreateCylinderBar3DChartLarge",
								CreateCylinderFullStackedBar3DChart = "CreateCylinderFullStackedBar3DChart",
								CreateCylinderFullStackedBar3DChartLarge = "CreateCylinderFullStackedBar3DChartLarge",
								CreateCylinderManhattanBarChart = "CreateCylinderManhattanBarChart",
								CreateCylinderManhattanBarChartLarge = "CreateCylinderManhattanBarChartLarge",
								CreateCylinderStackedBar3DChart = "CreateCylinderStackedBar3DChart",
								CreateCylinderStackedBar3DChartLarge = "CreateCylinderStackedBar3DChartLarge",
								CreateDoughnutChart = "CreateDoughnutChart",
								CreateDoughnutChartLarge = "CreateDoughnutChartLarge",
								CreateFullStackedArea3DChart = "CreateFullStackedArea3DChart",
								CreateFullStackedArea3DChartLarge = "CreateFullStackedArea3DChartLarge",
								CreateFullStackedAreaChart = "CreateFullStackedAreaChart",
								CreateFullStackedAreaChartLarge = "CreateFullStackedAreaChartLarge",
								CreateFullStackedBar3DChart = "CreateFullStackedBar3DChart",
								CreateFullStackedBar3DChartLarge = "CreateFullStackedBar3DChartLarge",
								CreateFullStackedBarChart = "CreateFullStackedBarChart",
								CreateFullStackedBarChartLarge = "CreateFullStackedBarChartLarge",
								CreateFullStackedLineChart = "CreateFullStackedLineChart",
								CreateFullStackedLineChartLarge = "CreateFullStackedLineChartLarge",
								CreateLine3DChart = "CreateLine3DChart",
								CreateLine3DChartLarge = "CreateLine3DChartLarge",
								CreateLineChart = "CreateLineChart",
								CreateLineChartLarge = "CreateLineChartLarge",
								CreateManhattanBarChart = "CreateManhattanBarChart",
								CreateManhattanBarChartLarge = "CreateManhattanBarChartLarge",
								CreatePie3DChart = "CreatePie3DChart",
								CreatePie3DChartLarge = "CreatePie3DChartLarge",
								CreatePieChart = "CreatePieChart",
								CreatePieChartLarge = "CreatePieChartLarge",
								CreatePyramidBar3DChart = "CreatePyramidBar3DChart",
								CreatePyramidBar3DChartLarge = "CreatePyramidBar3DChartLarge",
								CreatePyramidFullStackedBar3DChart = "CreatePyramidFullStackedBar3DChart",
								CreatePyramidFullStackedBar3DChartLarge = "CreatePyramidFullStackedBar3DChartLarge",
								CreatePyramidManhattanBarChart = "CreatePyramidManhattanBarChart",
								CreatePyramidManhattanBarChartLarge = "CreatePyramidManhattanBarChartLarge",
								CreatePyramidStackedBar3DChart = "CreatePyramidStackedBar3DChart",
								CreatePyramidStackedBar3DChartLarge = "CreatePyramidStackedBar3DChartLarge",
								CreateRadarLineChart = "CreateRadarLineChart",
								CreateRadarLineChartLarge = "CreateRadarLineChartLarge",
								CreateRotatedBarChart = "CreateRotatedBarChart",
								CreateRotatedBarChartLarge = "CreateRotatedBarChartLarge",
								CreateRotatedFullStackedBarChart = "CreateRotatedFullStackedBarChart",
								CreateRotatedFullStackedBarChartLarge = "CreateRotatedFullStackedBarChartLarge",
								CreateRotatedStackedBarChart = "CreateRotatedStackedBarChart",
								CreateRotatedStackedBarChartLarge = "CreateRotatedStackedBarChartLarge",
								CreateStackedArea3DChart = "CreateStackedArea3DChart",
								CreateStackedArea3DChartLarge = "CreateStackedArea3DChartLarge",
								CreateStackedAreaChart = "CreateStackedAreaChart",
								CreateStackedAreaChartLarge = "CreateStackedAreaChartLarge",
								CreateStackedBar3DChart = "CreateStackedBar3DChart",
								CreateStackedBar3DChartLarge = "CreateStackedBar3DChartLarge",
								CreateStackedBarChart = "CreateStackedBarChart",
								CreateStackedBarChartLarge = "CreateStackedBarChartLarge",
								CreateStackedLineChart = "CreateStackedLineChart",
								CreateStackedLineChartLarge = "CreateStackedLineChartLarge",
								CreateStockChart = "CreateStockChart",
								CreateStockChartLarge = "CreateStockChartLarge",
								Cut = "Cut",
								CutLarge = "CutLarge",
								Delete_Hyperlink = "Delete_Hyperlink",
								Delete_HyperlinkLarge = "Delete_HyperlinkLarge",
								DeleteComment = "DeleteComment",
								DeleteCommentLarge = "DeleteCommentLarge",
								DataValidation = "DataValidation",
								DataValidationLarge = "DataValidationLarge",
								EditComment = "EditComment",
								EditCommentLarge = "EditCommentLarge",
								EditingFillDown = "EditingFillDown",
								EditingFillDownLarge = "EditingFillDownLarge",
								EditingFillLeft = "EditingFillLeft",
								EditingFillLeftLarge = "EditingFillLeftLarge",
								EditingFillRight = "EditingFillRight",
								EditingFillRightLarge = "EditingFillRightLarge",
								EditingFillUp = "EditingFillUp",
								EditingFillUpLarge = "EditingFillUpLarge",
								EditingMergeAndCenterCells = "EditingMergeAndCenterCells",
								EditingMergeAndCenterCellsLarge = "EditingMergeAndCenterCellsLarge",
								EditingMergeCells = "EditingMergeCells",
								EditingMergeCellsAcross = "EditingMergeCellsAcross",
								EditingMergeCellsAcrossLarge = "EditingMergeCellsAcrossLarge",
								EditingMergeCellsLarge = "EditingMergeCellsLarge",
								EditingUnmergeCells = "EditingUnmergeCells",
								EditingUnmergeCellsLarge = "EditingUnmergeCellsLarge",
								Find = "Find",
								FindLarge = "FindLarge",
								FontColor = "FontColor",
								FontColorLarge = "FontColorLarge",
								FontSizeDecrease = "FontSizeDecrease",
								FontSizeDecreaseLarge = "FontSizeDecreaseLarge",
								FontSizeIncrease = "FontSizeIncrease",
								FontSizeIncreaseLarge = "FontSizeIncreaseLarge",
								Format = "Format",
								FormatLarge = "FormatLarge",
								FormatNumberCommaStyle = "FormatNumberCommaStyle",
								FormatNumberCommaStyleLarge = "FormatNumberCommaStyleLarge",
								FormatNumberCurrency = "FormatNumberCurrency",
								FormatNumberCurrencyLarge = "FormatNumberCurrencyLarge",
								FormatNumberDecreaseDecimal = "FormatNumberDecreaseDecimal",
								FormatNumberDecreaseDecimalLarge = "FormatNumberDecreaseDecimalLarge",
								FormatNumberIncreaseDecimal = "FormatNumberIncreaseDecimal",
								FormatNumberIncreaseDecimalLarge = "FormatNumberIncreaseDecimalLarge",
								FormatNumberPercent = "FormatNumberPercent",
								FormatNumberPercentLarge = "FormatNumberPercentLarge",
								FormatWrapText = "FormatWrapText",
								FormatWrapTextLarge = "FormatWrapTextLarge",
								FunctionsCompatibility = "FunctionsCompatibility",
								FunctionsCompatibilityLarge = "FunctionsCompatibilityLarge",
								FunctionsCube = "FunctionsCube",
								FunctionsCubeLarge = "FunctionsCubeLarge",
								FunctionsDateAndTime = "FunctionsDateAndTime",
								FunctionsDateAndTimeLarge = "FunctionsDateAndTimeLarge",
								FunctionsEngineering = "FunctionsEngineering",
								FunctionsEngineeringLarge = "FunctionsEngineeringLarge",
								FunctionsFinancial = "FunctionsFinancial",
								FunctionsFinancialLarge = "FunctionsFinancialLarge",
								FunctionsInformation = "FunctionsInformation",
								FunctionsInformationLarge = "FunctionsInformationLarge",
								FunctionsLogical = "FunctionsLogical",
								FunctionsLogicalLarge = "FunctionsLogicalLarge",
								FunctionsLookupAndReference = "FunctionsLookupAndReference",
								FunctionsLookupAndReferenceLarge = "FunctionsLookupAndReferenceLarge",
								FunctionsMathAndTrigonometry = "FunctionsMathAndTrigonometry",
								FunctionsMathAndTrigonometryLarge = "FunctionsMathAndTrigonometryLarge",
								FunctionsMore = "FunctionsMore",
								FunctionsMoreLarge = "FunctionsMoreLarge",
								FunctionsStatistical = "FunctionsStatistical",
								FunctionsStatisticalLarge = "FunctionsStatisticalLarge",
								FunctionsText = "FunctionsText",
								FunctionsTextLarge = "FunctionsTextLarge",
								FunctionSum = "FunctionSum",
								FunctionSumLarge = "FunctionSumLarge",
								Hyperlink = "Hyperlink",
								HyperlinkLarge = "HyperlinkLarge",
								ImagePlaceHolder = "ImagePlaceHolder",
								IndentDecrease = "IndentDecrease",
								IndentDecreaseLarge = "IndentDecreaseLarge",
								IndentIncrease = "IndentIncrease",
								IndentIncreaseLarge = "IndentIncreaseLarge",
								InsertCellsCommandGroup = "InsertCellsCommandGroup",
								InsertCellsCommandGroupLarge = "InsertCellsCommandGroupLarge",
								InsertComment = "InsertComment",
								InsertCommentLarge = "InsertCommentLarge",
								InsertFloatingObjectImage = "InsertFloatingObjectImage",
								InsertFloatingObjectImageLarge = "InsertFloatingObjectImageLarge",
								InsertSheet = "InsertSheet",
								InsertSheetColumns = "InsertSheetColumns",
								InsertSheetColumnsLarge = "InsertSheetColumnsLarge",
								InsertSheetLarge = "InsertSheetLarge",
								InsertSheetRows = "InsertSheetRows",
								InsertSheetRowsLarge = "InsertSheetRowsLarge",
								InsertTableLarge = "InsertTableLarge",
								InsertTable = "InsertTable",
								Italic = "Italic",
								ItalicLarge = "ItalicLarge",
								New = "New",
								NewLarge = "NewLarge",
								Open = "Open",
								OpenHyperlink = "OpenHyperlink",
								OpenHyperlinkLarge = "OpenHyperlinkLarge",
								OpenLarge = "OpenLarge",
								PageMargins = "PageMargins",
								PageMarginsLarge = "PageMarginsLarge",
								PageMarginsModerate = "PageMarginsModerate",
								PageMarginsModerateLarge = "PageMarginsModerateLarge",
								PageMarginsNarrow = "PageMarginsNarrow",
								PageMarginsNarrowLarge = "PageMarginsNarrowLarge",
								PageMarginsNormal = "PageMarginsNormal",
								PageMarginsNormalLarge = "PageMarginsNormalLarge",
								PageMarginsWide = "PageMarginsWide",
								PageMarginsWideLarge = "PageMarginsWideLarge",
								PageOrientation = "PageOrientation",
								PageOrientationLarge = "PageOrientationLarge",
								PaperSize = "PaperSize",
								PaperSizeLarge = "PaperSizeLarge",
								Paste = "Paste",
								PasteLarge = "PasteLarge",
								PasteSpecial = "PasteSpecial",
								PasteSpecialLarge = "PasteSpecialLarge",
								SortAsc = "SortAsc",
								SortAscLarge = "SortAscLarge",
								SortDesc = "SortDesc",
								SortDescLarge = "SortDescLarge",
								PenColor = "PenColor",
								PenColorLarge = "PenColorLarge",
								Preview = "Preview",
								PreviewLarge = "PreviewLarge",
								Print = "Print",
								PrintDialog = "PrintDialog",
								PrintDialogLarge = "PrintDialogLarge",
								PrintLarge = "PrintLarge",
								Redo = "Redo",
								RedoLarge = "RedoLarge",
								RemoveCellsCommandGroup = "RemoveCellsCommandGroup",
								RemoveCellsCommandGroupLarge = "RemoveCellsCommandGroupLarge",
								RemoveSheet = "RemoveSheet",
								RemoveSheetColumns = "RemoveSheetColumns",
								RemoveSheetColumnsLarge = "RemoveSheetColumnsLarge",
								RemoveSheetLarge = "RemoveSheetLarge",
								RemoveSheetRows = "RemoveSheetRows",
								RemoveSheetRowsLarge = "RemoveSheetRowsLarge",
								Save = "Save",
								SaveAll = "SaveAll",
								SaveAllLarge = "SaveAllLarge",
								SaveAs = "SaveAs",
								SaveAsLarge = "SaveAsLarge",
								SaveLarge = "SaveLarge",
								ShowHideComments = "ShowHideComments",
								ShowHideCommentsLarge = "ShowHideCommentsLarge",
								Strikeout = "Strikeout",
								StrikeoutLarge = "StrikeoutLarge",
								Underline = "Underline",
								UnderlineDouble = "UnderlineDouble",
								UnderlineDoubleLarge = "UnderlineDoubleLarge",
								UnderlineLarge = "UnderlineLarge",
								Undo = "Undo",
								UndoLarge = "UndoLarge",
								Zoom100Percent = "Zoom100Percent",
								Zoom100PercentLarge = "Zoom100PercentLarge",
								ZoomIn = "ZoomIn",
								ZoomInLarge = "ZoomInLarge",
								ZoomOut = "ZoomOut",
								ZoomOutLarge = "ZoomOutLarge",
								CreateRotatedBar3DChart = "CreateRotatedBar3DChart",
								CreateRotatedBar3DChartLarge = "CreateRotatedBar3DChartLarge",
								CreateRotatedStackedBar3DChart = "CreateRotatedStackedBar3DChart",
								CreateRotatedFullStackedBar3DChart = "CreateRotatedFullStackedBar3DChart",
								CreateRotatedCylinderBar3DChart = "CreateRotatedCylinderBar3DChart",
								CreateRotatedStackedCylinderBar3DChart = "CreateRotatedStackedCylinderBar3DChart",
								CreateRotatedFullStackedCylinderBar3DChar = "CreateRotatedFullStackedCylinderBar3DChart",
								CreateRotatedConeBar3DChart = "CreateRotatedConeBar3DChart",
								CreateRotatedStackedConeBar3DChart = "CreateRotatedStackedConeBar3DChart",
								CreateRotatedFullStackedConeBar3DChart = "CreateRotatedFullStackedConeBar3DChart",
								CreateRotatedPyramidBar3DChart = "CreateRotatedPyramidBar3DChart",
								CreateRotatedStackedPyramidBar3DChart = "CreateRotatedStackedPyramidBar3DChart",
								CreateRotatedFullStackedPyramidBar3DChart = "CreateRotatedFullStackedPyramidBar3DChart",
								CreateLineChartNoMarkers = "CreateLineChartNoMarkers",
								CreateStackedLineChartNoMarkers = "CreateStackedLineChartNoMarkers",
								CreateFullStackedLineChartNoMarkers = "CreateFullStackedLineChartNoMarkers",
								CreateScatterChartMarkersOnly = "CreateScatterChartMarkersOnly",
								CreateScatterChartLines = "CreateScatterChartLines",
								CreateScatterChartSmoothLines = "CreateScatterChartSmoothLines",
								CreateScatterChartLinesAndMarkers = "CreateScatterChartLinesAndMarkers",
								CreateScatterChartSmoothLinesAndMarkers = "CreateScatterChartSmoothLinesAndMarkers",
								CreateBubble3DChart = "CreateBubble3DChart",
								CreateRadarLineChartNoMarkers = "CreateRadarLineChartNoMarkers",
								CreateRadarLineChartFilled = "CreateRadarLineChartFilled",
								CreateStockChartOpenHighLowClose = "CreateStockChartOpenHighLowClose",
								CreateExplodedPieChart = "CreateExplodedPieChart",
								CreateExplodedPie3DChart = "CreateExplodedPie3DChart",
								CreateExplodedDoughnutChart = "CreateExplodedDoughnutChart",
								CreateStockChartHighLowClose = "CreateStockChartHighLowClose",
								CreateRotatedStackedBar3DChartLarge = "CreateRotatedStackedBar3DChartLarge",
								CreateRotatedFullStackedBar3DChartLarge = "CreateRotatedFullStackedBar3DChartLarge",
								CreateRotatedCylinderBar3DChartLarge = "CreateRotatedCylinderBar3DChartLarge",
								CreateRotatedStackedCylinderBar3DChartLarge = "CreateRotatedStackedCylinderBar3DChartLarge",
								CreateRotatedFullStackedCylinderBar3DCharLarge = "CreateRotatedFullStackedCylinderBar3DChartLarge",
								CreateRotatedConeBar3DChartLarge = "CreateRotatedConeBar3DChartLarge",
								CreateRotatedStackedConeBar3DChartLarge = "CreateRotatedStackedConeBar3DChartLarge",
								CreateRotatedFullStackedConeBar3DChartLarge = "CreateRotatedFullStackedConeBar3DChartLarge",
								CreateRotatedPyramidBar3DChartLarge = "CreateRotatedPyramidBar3DChartLarge",
								CreateRotatedStackedPyramidBar3DChartLarge = "CreateRotatedStackedPyramidBar3DChartLarge",
								CreateRotatedFullStackedPyramidBar3DChartLarge = "CreateRotatedFullStackedPyramidBar3DChartLarge",
								CreateLineChartNoMarkersLarge = "CreateLineChartNoMarkersLarge",
								CreateStackedLineChartNoMarkersLarge = "CreateStackedLineChartNoMarkersLarge",
								CreateFullStackedLineChartNoMarkersLarge = "CreateFullStackedLineChartNoMarkersLarge",
								CreateScatterChartMarkersOnlyLarge = "CreateScatterChartMarkersOnlyLarge",
								CreateScatterChartLinesLarge = "CreateScatterChartLinesLarge",
								CreateScatterChartSmoothLinesLarge = "CreateScatterChartSmoothLinesLarge",
								CreateScatterChartLinesAndMarkersLarge = "CreateScatterChartLinesAndMarkersLarge",
								CreateScatterChartSmoothLinesAndMarkersLarge = "CreateScatterChartSmoothLinesAndMarkersLarge",
								CreateBubble3DChartLarge = "CreateBubble3DChartLarge",
								CreateRadarLineChartNoMarkersLarge = "CreateRadarLineChartNoMarkersLarge",
								CreateRadarLineChartFilledLarge = "CreateRadarLineChartFilledLarge",
								CreateStockChartOpenHighLowCloseLarge = "CreateStockChartOpenHighLowCloseLarge",
								CreateExplodedPieChartLarge = "CreateExplodedPieChartLarge",
								CreateExplodedPie3DChartLarge = "CreateExplodedPie3DChartLarge",
								CreateExplodedDoughnutChartLarge = "CreateExplodedDoughnutChartLarge",
								CreateStockChartHighLowCloseLarge = "CreateStockChartHighLowCloseLarge",
								ChartGroupColumn = "ChartGroupColumn",
								ChartGroupColumnLarge = "ChartGroupColumnLarge",
								ChartGroupLine = "ChartGroupLine",
								ChartGroupLineLarge = "ChartGroupLineLarge",
								ChartGroupPie = "ChartGroupPie",
								ChartGroupPieLarge = "ChartGroupPieLarge",
								ChartGroupBar = "ChartGroupBar",
								ChartGroupBarLarge = "ChartGroupBarLarge",
								ChartGroupArea = "ChartGroupArea",
								ChartGroupAreaLarge = "ChartGroupAreaLarge",
								ChartGroupScatter = "ChartGroupScatter",
								ChartGroupScatterLarge = "ChartGroupScatterLarge",
								ChartGroupDoughnut = "ChartGroupDoughnut",
								ChartGroupDoughnutLarge = "ChartGroupDoughnutLarge",
								ChartGroupOther = "ChartGroupOther",
								ChartGroupOtherLarge = "ChartGroupOtherLarge",
								ShowFormulas = "ShowFormulas",
								ShowFormulasLarge = "ShowFormulasLarge",
								FontGroup = "FontGroup",
								FontGroupLarge = "FontGroupLarge",
								BorderLineStyle = "BorderLineStyle",
								BorderLineStyleLarge = "BorderLineStyleLarge",
								SortAndFilter = "SortAndFilter",
								SortAndFilterLarge = "SortAndFilterLarge",
								Filter = "Filter",
								FilterLarge = "FilterLarge",
								ClearFilter = "ClearFilter",
								ClearFilterLarge = "ClearFilterLarge",
								ReapplyFilter = "ReapplyFilter",
								ReapplyFilterLarge = "ReapplyFilterLarge",
								TableConvertToRange = "TableConvertToRange",
								TableConvertToRangeLarge = "TableConvertToRangeLarge",
								CalculateNow = "CalculateNow",
								CalculateNowLarge = "CalculateNowLarge",
								CalculateSheet = "CalculateSheet",
								CalculateSheetLarge = "CalculateSheetLarge",
								CalculationOptions = "CalculationOptions",
								CalculationOptionsLarge = "CalculationOptionsLarge",
								FullScreen = "FullScreen",
								FullScreenLarge = "FullScreenLarge",
								PrintArea = "PrintArea",
								PrintAreaLarge = "PrintAreaLarge",
								PageOrientationPortrait = "PageOrientationPortrait",
								PageOrientationPortraitLarge = "PageOrientationPortraitLarge",
								PageOrientationLandscape = "PageOrientationLandscape",
								PageOrientationLandscapeLarge = "PageOrientationLandscapeLarge",
								FloatingObjectBringForward = "FloatingObjectBringForward",
								FloatingObjectBringForwardLarge = "FloatingObjectBringForwardLarge",
								FloatingObjectBringToFront = "FloatingObjectBringToFront",
								FloatingObjectBringToFrontLarge = "FloatingObjectBringToFrontLarge",
								FloatingObjectSendBackward = "FloatingObjectSendBackward",
								FloatingObjectSendBackwardLarge = "FloatingObjectSendBackwardLarge",
								FloatingObjectSendToBack = "FloatingObjectSendToBack",
								FloatingObjectSendToBackLarge = "FloatingObjectSendToBackLarge",
								FreezeFirstColumn = "FreezeFirstColumn",
								FreezeFirstColumnLarge = "FreezeFirstColumnLarge",
								FreezePanes = "FreezePanes",
								FreezePanesLarge = "FreezePanesLarge",
								FreezeTopRow = "FreezeTopRow",
								FreezeTopRowLarge = "FreezeTopRowLarge",
								UnfreezePanes = "UnfreezePanes",
								UnfreezePanesLarge = "UnfreezePanesLarge",
								FormatAsTable = "FormatAsTable",
								FormatAsTableLarge = "FormatAsTableLarge",
								ChartSwitchRowColumn = "ChartSwitchRowColumn",
								ChartSwitchRowColumnLarge = "ChartSwitchRowColumnLarge",
								ChartSelectData = "ChartSelectData",
								ChartSelectDataLarge = "ChartSelectDataLarge",
								ChartTitleAbove = "ChartTitleAbove",
								ChartTitleAboveLarge = "ChartTitleAboveLarge",
								ChartTitleCenteredOverlay = "ChartTitleCenteredOverlay",
								ChartTitleCenteredOverlayLarge = "ChartTitleCenteredOverlayLarge",
								ChartTitleNone = "ChartTitleNone",
								ChartTitleNoneLarge = "ChartTitleNoneLarge",
								ChartAxisTitleGroup = "ChartAxisTitleGroup",
								ChartAxisTitleGroupLarge = "ChartAxisTitleGroupLarge",
								ChartAxisTitleHorizontal = "ChartAxisTitleHorizontal",
								ChartAxisTitleHorizontalLarge = "ChartAxisTitleHorizontalLarge",
								ChartAxisTitleHorizontal_None = "ChartAxisTitleHorizontal_None",
								ChartAxisTitleHorizontal_NoneLarge = "ChartAxisTitleHorizontal_NoneLarge",
								ChartAxisTitleVertical = "ChartAxisTitleVertical",
								ChartAxisTitleVerticalLarge = "ChartAxisTitleVerticalLarge",
								ChartAxisTitleVertical_HorizonlalText = "ChartAxisTitleVertical_HorizonlalText",
								ChartAxisTitleVertical_HorizonlalTextLarge = "ChartAxisTitleVertical_HorizonlalTextLarge",
								ChartAxisTitleVertical_None = "ChartAxisTitleVertical_None",
								ChartAxisTitleVertical_NoneLarge = "ChartAxisTitleVertical_NoneLarge",
								ChartAxisTitleVertical_RotatedText = "ChartAxisTitleVertical_RotatedText",
								ChartAxisTitleVertical_RotatedTextLarge = "ChartAxisTitleVertical_RotatedTextLarge",
								ChartAxisTitleVertical_VerticalText = "ChartAxisTitleVertical_VerticalText",
								ChartAxisTitleVertical_VerticalTextLarge = "ChartAxisTitleVertical_VerticalTextLarge",
								ChartAxesGroup = "ChartAxesGroup",
								ChartAxesGroupLarge = "ChartAxesGroupLarge",
								ChartHorizontalAxis_LeftToRight = "ChartHorizontalAxis_LeftToRight",
								ChartHorizontalAxis_LeftToRightLarge = "ChartHorizontalAxis_LeftToRightLarge",
								ChartHorizontalAxis_None = "ChartHorizontalAxis_None",
								ChartHorizontalAxis_NoneLarge = "ChartHorizontalAxis_NoneLarge",
								ChartHorizontalAxis_RightToLeft = "ChartHorizontalAxis_RightToLeft",
								ChartHorizontalAxis_RightToLeftLarge = "ChartHorizontalAxis_RightToLeftLarge",
								ChartHorizontalAxis_WithoutLabeling = "ChartHorizontalAxis_WithoutLabeling",
								ChartHorizontalAxis_WithoutLabelingLarge = "ChartHorizontalAxis_WithoutLabelingLarge",
								ChartHorizontalAxis_Default = "ChartHorizontalAxis_Default",
								ChartHorizontalAxis_DefaultLarge = "ChartHorizontalAxis_DefaultLarge",
								ChartVerticalAxis_None = "ChartVerticalAxis_None",
								ChartVerticalAxis_NoneLarge = "ChartVerticalAxis_NoneLarge",
								ChartVerticalAxis_Default = "ChartVerticalAxis_Default",
								ChartVerticalAxis_DefaultLarge = "ChartVerticalAxis_DefaultLarge",
								ChartVerticalAxis_Thousands = "ChartVerticalAxis_Thousands",
								ChartVerticalAxis_ThousandsLarge = "ChartVerticalAxis_ThousandsLarge",
								ChartVerticalAxis_Millions = "ChartVerticalAxis_Millions",
								ChartVerticalAxis_MillionsLarge = "ChartVerticalAxis_MillionsLarge",
								ChartVerticalAxis_Billions = "ChartVerticalAxis_Billions",
								ChartVerticalAxis_BillionsLarge = "ChartVerticalAxis_BillionsLarge",
								ChartVerticalAxis_LogScale = "ChartVerticalAxis_LogScale",
								ChartVerticalAxis_LogScaleLarge = "ChartVerticalAxis_LogScaleLarge",
								ChartLegend_Bottom = "ChartLegend_Bottom",
								ChartLegend_BottomLarge = "ChartLegend_BottomLarge",
								ChartLegend_Left = "ChartLegend_Left",
								ChartLegend_LeftLarge = "ChartLegend_LeftLarge",
								ChartLegend_LeftOverlay = "ChartLegend_LeftOverlay",
								ChartLegend_LeftOverlayLarge = "ChartLegend_LeftOverlayLarge",
								ChartLegend_None = "ChartLegend_None",
								ChartLegend_NoneLarge = "ChartLegend_NoneLarge",
								ChartLegend_Right = "ChartLegend_Right",
								ChartLegend_RightLarge = "ChartLegend_RightLarge",
								ChartLegend_RightOverlay = "ChartLegend_RightOverlay",
								ChartLegend_RightOverlayLarge = "ChartLegend_RightOverlayLarge",
								ChartLegend_Top = "ChartLegend_Top",
								ChartLegend_TopLarge = "ChartLegend_TopLarge",
								ChartLabels_InsideBase = "ChartLabels_InsideBase",
								ChartLabels_InsideBaseLarge = "ChartLabels_InsideBaseLarge",
								ChartLabels_InsideCenter = "ChartLabels_InsideCenter",
								ChartLabels_InsideCenterLarge = "ChartLabels_InsideCenterLarge",
								ChartLabels_InsideEnd = "ChartLabels_InsideEnd",
								ChartLabels_InsideEndLarge = "ChartLabels_InsideEndLarge",
								ChartLabels_None = "ChartLabels_None",
								ChartLabels_NoneLarge = "ChartLabels_NoneLarge",
								ChartLabels_OutsideEnd = "ChartLabels_OutsideEnd",
								ChartLabels_OutsideEndLarge = "ChartLabels_OutsideEndLarge",
								ChartGridlines = "ChartGridlines",
								ChartGridlinesLarge = "ChartGridlinesLarge",
								ChartGridlinesHorizontal_Major = "ChartGridlinesHorizontal_Major",
								ChartGridlinesHorizontal_MajorLarge = "ChartGridlinesHorizontal_MajorLarge",
								ChartGridlinesHorizontal_MajorMinor = "ChartGridlinesHorizontal_MajorMinor",
								ChartGridlinesHorizontal_MajorMinorLarge = "ChartGridlinesHorizontal_MajorMinorLarge",
								ChartGridlinesHorizontal_Minor = "ChartGridlinesHorizontal_Minor",
								ChartGridlinesHorizontal_MinorLarge = "ChartGridlinesHorizontal_MinorLarge",
								ChartGridlinesHorizontal_None = "ChartGridlinesHorizontal_None",
								ChartGridlinesHorizontal_NoneLarge = "ChartGridlinesHorizontal_NoneLarge",
								ChartGridlinesVertical_Major = "ChartGridlinesVertical_Major",
								ChartGridlinesVertical_MajorLarge = "ChartGridlinesVertical_MajorLarge",
								ChartGridlinesVertical_MajorMinor = "ChartGridlinesVertical_MajorMinor",
								ChartGridlinesVertical_MajorMinorLarge = "ChartGridlinesVertical_MajorMinorLarge",
								ChartGridlinesVertical_Minor = "ChartGridlinesVertical_Minor",
								ChartGridlinesVertical_MinorLarge = "ChartGridlinesVertical_MinorLarge",
								ChartGridlinesVertical_None = "ChartGridlinesVertical_None",
								ChartGridlinesVertical_NoneLarge = "ChartGridlinesVertical_NoneLarge";
		#endregion
		internal readonly static Dictionary<MenuIconSetType, string> CategoryDictionary = new Dictionary<MenuIconSetType, string>();
		internal readonly static Dictionary<MenuIconSetType, string> SpriteNameDictionary = new Dictionary<MenuIconSetType, string>();
		static SpreadsheetRibbonImages() {
			CategoryDictionary.Add(MenuIconSetType.NotSet, "ssRibbonIcons");
			CategoryDictionary.Add(MenuIconSetType.Colored, "ssRibbonIcons");
			CategoryDictionary.Add(MenuIconSetType.ColoredLight, "ssRibbonIcons");
			CategoryDictionary.Add(MenuIconSetType.GrayScaled, "ssRibbonGIcons");
			CategoryDictionary.Add(MenuIconSetType.GrayScaledWithWhiteHottrack, "ssRibbonGWIcons");
			SpriteNameDictionary.Add(MenuIconSetType.NotSet, RibbonSSSpriteName);
			SpriteNameDictionary.Add(MenuIconSetType.Colored, RibbonSSSpriteName);
			SpriteNameDictionary.Add(MenuIconSetType.ColoredLight, RibbonSSSpriteName);
			SpriteNameDictionary.Add(MenuIconSetType.GrayScaled, RibbonSSGSpriteName);
			SpriteNameDictionary.Add(MenuIconSetType.GrayScaledWithWhiteHottrack, RibbonSSGWSpriteName);
		}
		public SpreadsheetRibbonImages(ISkinOwner properties, MenuIconSetType iconSetStyle)
			: base(properties, iconSetStyle) {
		}
		protected override Dictionary<MenuIconSetType, string> Categories { get { return CategoryDictionary; } }
		protected override Dictionary<MenuIconSetType, string> SpriteNames { get { return SpriteNameDictionary; } }
		protected override void PopulateImageInfoList(List<ImageInfo> list) {
			base.PopulateImageInfoList(list);
			ImageFlags imageFlags = ImageFlags.IsPng;
			#region InitializeImages
			list.Add(new ImageInfo(AlignBottomCenter, imageFlags, DefaultSmallSize, AlignBottomCenter));
			list.Add(new ImageInfo(AlignBottomCenterLarge, imageFlags, DefaultLargeSize, AlignBottomCenterLarge));
			list.Add(new ImageInfo(AlignCenter, imageFlags, DefaultSmallSize, AlignCenter));
			list.Add(new ImageInfo(AlignCenterLarge, imageFlags, DefaultLargeSize, AlignCenterLarge));
			list.Add(new ImageInfo(AlignJustify, imageFlags, DefaultSmallSize, AlignJustify));
			list.Add(new ImageInfo(AlignJustifyLarge, imageFlags, DefaultLargeSize, AlignJustifyLarge));
			list.Add(new ImageInfo(AlignLeft, imageFlags, DefaultSmallSize, AlignLeft));
			list.Add(new ImageInfo(AlignLeftLarge, imageFlags, DefaultLargeSize, AlignLeftLarge));
			list.Add(new ImageInfo(AlignMiddleCenter, imageFlags, DefaultSmallSize, AlignMiddleCenter));
			list.Add(new ImageInfo(AlignMiddleCenterLarge, imageFlags, DefaultLargeSize, AlignMiddleCenterLarge));
			list.Add(new ImageInfo(AlignRight, imageFlags, DefaultSmallSize, AlignRight));
			list.Add(new ImageInfo(AlignRightLarge, imageFlags, DefaultLargeSize, AlignRightLarge));
			list.Add(new ImageInfo(AlignTopCenter, imageFlags, DefaultSmallSize, AlignTopCenter));
			list.Add(new ImageInfo(AlignTopCenterLarge, imageFlags, DefaultLargeSize, AlignTopCenterLarge));
			list.Add(new ImageInfo(Background, imageFlags, DefaultSmallSize, Background));
			list.Add(new ImageInfo(BackgroundLarge, imageFlags, DefaultLargeSize, BackgroundLarge));
			list.Add(new ImageInfo(Bold, imageFlags, DefaultSmallSize, Bold));
			list.Add(new ImageInfo(BoldLarge, imageFlags, DefaultLargeSize, BoldLarge));
			list.Add(new ImageInfo(BorderBottom, imageFlags, DefaultSmallSize, BorderBottom));
			list.Add(new ImageInfo(BorderBottomLarge, imageFlags, DefaultLargeSize, BorderBottomLarge));
			list.Add(new ImageInfo(BorderBottomThick, imageFlags, DefaultSmallSize, BorderBottomThick));
			list.Add(new ImageInfo(BorderBottomThickLarge, imageFlags, DefaultLargeSize, BorderBottomThickLarge));
			list.Add(new ImageInfo(BorderLeft, imageFlags, DefaultSmallSize, BorderLeft));
			list.Add(new ImageInfo(BorderLeftLarge, imageFlags, DefaultLargeSize, BorderLeftLarge));
			list.Add(new ImageInfo(BorderNone, imageFlags, DefaultSmallSize, BorderNone));
			list.Add(new ImageInfo(BorderNoneLarge, imageFlags, DefaultLargeSize, BorderNoneLarge));
			list.Add(new ImageInfo(BorderRight, imageFlags, DefaultSmallSize, BorderRight));
			list.Add(new ImageInfo(BorderRightLarge, imageFlags, DefaultLargeSize, BorderRightLarge));
			list.Add(new ImageInfo(BordersAll, imageFlags, DefaultSmallSize, BordersAll));
			list.Add(new ImageInfo(BordersAllLarge, imageFlags, DefaultLargeSize, BordersAllLarge));
			list.Add(new ImageInfo(BordersOutside, imageFlags, DefaultSmallSize, BordersOutside));
			list.Add(new ImageInfo(BordersOutsideLarge, imageFlags, DefaultLargeSize, BordersOutsideLarge));
			list.Add(new ImageInfo(BordersOutsideThick, imageFlags, DefaultSmallSize, BordersOutsideThick));
			list.Add(new ImageInfo(BordersOutsideThickLarge, imageFlags, DefaultLargeSize, BordersOutsideThickLarge));
			list.Add(new ImageInfo(BorderTop, imageFlags, DefaultSmallSize, BorderTop));
			list.Add(new ImageInfo(BorderTopAndBottom, imageFlags, DefaultSmallSize, BorderTopAndBottom));
			list.Add(new ImageInfo(BorderTopAndBottomLarge, imageFlags, DefaultLargeSize, BorderTopAndBottomLarge));
			list.Add(new ImageInfo(BorderTopAndBottomThick, imageFlags, DefaultSmallSize, BorderTopAndBottomThick));
			list.Add(new ImageInfo(BorderTopAndBottomThickLarge, imageFlags, DefaultLargeSize, BorderTopAndBottomThickLarge));
			list.Add(new ImageInfo(BorderTopLarge, imageFlags, DefaultLargeSize, BorderTopLarge));
			list.Add(new ImageInfo(ClearAll, imageFlags, DefaultSmallSize, ClearAll));
			list.Add(new ImageInfo(ClearAllLarge, imageFlags, DefaultLargeSize, ClearAllLarge));
			list.Add(new ImageInfo(ClearFormats, imageFlags, DefaultSmallSize, ClearFormats));
			list.Add(new ImageInfo(ClearFormatsLarge, imageFlags, DefaultLargeSize, ClearFormatsLarge));
			list.Add(new ImageInfo(Copy, imageFlags, DefaultSmallSize, Copy));
			list.Add(new ImageInfo(CopyLarge, imageFlags, DefaultLargeSize, CopyLarge));
			list.Add(new ImageInfo(CreateArea3DChart, imageFlags, DefaultSmallSize, CreateArea3DChart));
			list.Add(new ImageInfo(CreateArea3DChartLarge, imageFlags, DefaultLargeSize, CreateArea3DChartLarge));
			list.Add(new ImageInfo(CreateAreaChart, imageFlags, DefaultSmallSize, CreateAreaChart));
			list.Add(new ImageInfo(CreateAreaChartLarge, imageFlags, DefaultLargeSize, CreateAreaChartLarge));
			list.Add(new ImageInfo(CreateBar3DChart, imageFlags, DefaultSmallSize, CreateBar3DChart));
			list.Add(new ImageInfo(CreateBar3DChartLarge, imageFlags, DefaultLargeSize, CreateBar3DChartLarge));
			list.Add(new ImageInfo(CreateBarChart, imageFlags, DefaultSmallSize, CreateBarChart));
			list.Add(new ImageInfo(CreateBarChartLarge, imageFlags, DefaultLargeSize, CreateBarChartLarge));
			list.Add(new ImageInfo(CreateBubbleChart, imageFlags, DefaultSmallSize, CreateBubbleChart));
			list.Add(new ImageInfo(CreateBubbleChartLarge, imageFlags, DefaultLargeSize, CreateBubbleChartLarge));
			list.Add(new ImageInfo(CreateConeBar3DChart, imageFlags, DefaultSmallSize, CreateConeBar3DChart));
			list.Add(new ImageInfo(CreateConeBar3DChartLarge, imageFlags, DefaultLargeSize, CreateConeBar3DChartLarge));
			list.Add(new ImageInfo(CreateConeFullStackedBar3DChart, imageFlags, DefaultSmallSize, CreateConeFullStackedBar3DChart));
			list.Add(new ImageInfo(CreateConeFullStackedBar3DChartLarge, imageFlags, DefaultLargeSize, CreateConeFullStackedBar3DChartLarge));
			list.Add(new ImageInfo(CreateConeManhattanBarChart, imageFlags, DefaultSmallSize, CreateConeManhattanBarChart));
			list.Add(new ImageInfo(CreateConeManhattanBarChartLarge, imageFlags, DefaultLargeSize, CreateConeManhattanBarChartLarge));
			list.Add(new ImageInfo(CreateConeStackedBar3DChart, imageFlags, DefaultSmallSize, CreateConeStackedBar3DChart));
			list.Add(new ImageInfo(CreateConeStackedBar3DChartLarge, imageFlags, DefaultLargeSize, CreateConeStackedBar3DChartLarge));
			list.Add(new ImageInfo(CreateCylinderBar3DChart, imageFlags, DefaultSmallSize, CreateCylinderBar3DChart));
			list.Add(new ImageInfo(CreateCylinderBar3DChartLarge, imageFlags, DefaultLargeSize, CreateCylinderBar3DChartLarge));
			list.Add(new ImageInfo(CreateCylinderFullStackedBar3DChart, imageFlags, DefaultSmallSize, CreateCylinderFullStackedBar3DChart));
			list.Add(new ImageInfo(CreateCylinderFullStackedBar3DChartLarge, imageFlags, DefaultLargeSize, CreateCylinderFullStackedBar3DChartLarge));
			list.Add(new ImageInfo(CreateCylinderManhattanBarChart, imageFlags, DefaultSmallSize, CreateCylinderManhattanBarChart));
			list.Add(new ImageInfo(CreateCylinderManhattanBarChartLarge, imageFlags, DefaultLargeSize, CreateCylinderManhattanBarChartLarge));
			list.Add(new ImageInfo(CreateCylinderStackedBar3DChart, imageFlags, DefaultSmallSize, CreateCylinderStackedBar3DChart));
			list.Add(new ImageInfo(CreateCylinderStackedBar3DChartLarge, imageFlags, DefaultLargeSize, CreateCylinderStackedBar3DChartLarge));
			list.Add(new ImageInfo(CreateDoughnutChart, imageFlags, DefaultSmallSize, CreateDoughnutChart));
			list.Add(new ImageInfo(CreateDoughnutChartLarge, imageFlags, DefaultLargeSize, CreateDoughnutChartLarge));
			list.Add(new ImageInfo(CreateFullStackedArea3DChart, imageFlags, DefaultSmallSize, CreateFullStackedArea3DChart));
			list.Add(new ImageInfo(CreateFullStackedArea3DChartLarge, imageFlags, DefaultLargeSize, CreateFullStackedArea3DChartLarge));
			list.Add(new ImageInfo(CreateFullStackedAreaChart, imageFlags, DefaultSmallSize, CreateFullStackedAreaChart));
			list.Add(new ImageInfo(CreateFullStackedAreaChartLarge, imageFlags, DefaultLargeSize, CreateFullStackedAreaChartLarge));
			list.Add(new ImageInfo(CreateFullStackedBar3DChart, imageFlags, DefaultSmallSize, CreateFullStackedBar3DChart));
			list.Add(new ImageInfo(CreateFullStackedBar3DChartLarge, imageFlags, DefaultLargeSize, CreateFullStackedBar3DChartLarge));
			list.Add(new ImageInfo(CreateFullStackedBarChart, imageFlags, DefaultSmallSize, CreateFullStackedBarChart));
			list.Add(new ImageInfo(CreateFullStackedBarChartLarge, imageFlags, DefaultLargeSize, CreateFullStackedBarChartLarge));
			list.Add(new ImageInfo(CreateFullStackedLineChart, imageFlags, DefaultSmallSize, CreateFullStackedLineChart));
			list.Add(new ImageInfo(CreateFullStackedLineChartLarge, imageFlags, DefaultLargeSize, CreateFullStackedLineChartLarge));
			list.Add(new ImageInfo(CreateLine3DChart, imageFlags, DefaultSmallSize, CreateLine3DChart));
			list.Add(new ImageInfo(CreateLine3DChartLarge, imageFlags, DefaultLargeSize, CreateLine3DChartLarge));
			list.Add(new ImageInfo(CreateLineChart, imageFlags, DefaultSmallSize, CreateLineChart));
			list.Add(new ImageInfo(CreateLineChartLarge, imageFlags, DefaultLargeSize, CreateLineChartLarge));
			list.Add(new ImageInfo(CreateManhattanBarChart, imageFlags, DefaultSmallSize, CreateManhattanBarChart));
			list.Add(new ImageInfo(CreateManhattanBarChartLarge, imageFlags, DefaultLargeSize, CreateManhattanBarChartLarge));
			list.Add(new ImageInfo(CreatePie3DChart, imageFlags, DefaultSmallSize, CreatePie3DChart));
			list.Add(new ImageInfo(CreatePie3DChartLarge, imageFlags, DefaultLargeSize, CreatePie3DChartLarge));
			list.Add(new ImageInfo(CreatePieChart, imageFlags, DefaultSmallSize, CreatePieChart));
			list.Add(new ImageInfo(CreatePieChartLarge, imageFlags, DefaultLargeSize, CreatePieChartLarge));
			list.Add(new ImageInfo(CreatePyramidBar3DChart, imageFlags, DefaultSmallSize, CreatePyramidBar3DChart));
			list.Add(new ImageInfo(CreatePyramidBar3DChartLarge, imageFlags, DefaultLargeSize, CreatePyramidBar3DChartLarge));
			list.Add(new ImageInfo(CreatePyramidFullStackedBar3DChart, imageFlags, DefaultSmallSize, CreatePyramidFullStackedBar3DChart));
			list.Add(new ImageInfo(CreatePyramidFullStackedBar3DChartLarge, imageFlags, DefaultLargeSize, CreatePyramidFullStackedBar3DChartLarge));
			list.Add(new ImageInfo(CreatePyramidManhattanBarChart, imageFlags, DefaultSmallSize, CreatePyramidManhattanBarChart));
			list.Add(new ImageInfo(CreatePyramidManhattanBarChartLarge, imageFlags, DefaultLargeSize, CreatePyramidManhattanBarChartLarge));
			list.Add(new ImageInfo(CreatePyramidStackedBar3DChart, imageFlags, DefaultSmallSize, CreatePyramidStackedBar3DChart));
			list.Add(new ImageInfo(CreatePyramidStackedBar3DChartLarge, imageFlags, DefaultLargeSize, CreatePyramidStackedBar3DChartLarge));
			list.Add(new ImageInfo(CreateRadarLineChart, imageFlags, DefaultSmallSize, CreateRadarLineChart));
			list.Add(new ImageInfo(CreateRadarLineChartLarge, imageFlags, DefaultLargeSize, CreateRadarLineChartLarge));
			list.Add(new ImageInfo(CreateRotatedBarChart, imageFlags, DefaultSmallSize, CreateRotatedBarChart));
			list.Add(new ImageInfo(CreateRotatedBarChartLarge, imageFlags, DefaultLargeSize, CreateRotatedBarChartLarge));
			list.Add(new ImageInfo(CreateRotatedFullStackedBarChart, imageFlags, DefaultSmallSize, CreateRotatedFullStackedBarChart));
			list.Add(new ImageInfo(CreateRotatedFullStackedBarChartLarge, imageFlags, DefaultLargeSize, CreateRotatedFullStackedBarChartLarge));
			list.Add(new ImageInfo(CreateRotatedStackedBarChart, imageFlags, DefaultSmallSize, CreateRotatedStackedBarChart));
			list.Add(new ImageInfo(CreateRotatedStackedBarChartLarge, imageFlags, DefaultLargeSize, CreateRotatedStackedBarChartLarge));
			list.Add(new ImageInfo(CreateStackedArea3DChart, imageFlags, DefaultSmallSize, CreateStackedArea3DChart));
			list.Add(new ImageInfo(CreateStackedArea3DChartLarge, imageFlags, DefaultLargeSize, CreateStackedArea3DChartLarge));
			list.Add(new ImageInfo(CreateStackedAreaChart, imageFlags, DefaultSmallSize, CreateStackedAreaChart));
			list.Add(new ImageInfo(CreateStackedAreaChartLarge, imageFlags, DefaultLargeSize, CreateStackedAreaChartLarge));
			list.Add(new ImageInfo(CreateStackedBar3DChart, imageFlags, DefaultSmallSize, CreateStackedBar3DChart));
			list.Add(new ImageInfo(CreateStackedBar3DChartLarge, imageFlags, DefaultLargeSize, CreateStackedBar3DChartLarge));
			list.Add(new ImageInfo(CreateStackedBarChart, imageFlags, DefaultSmallSize, CreateStackedBarChart));
			list.Add(new ImageInfo(CreateStackedBarChartLarge, imageFlags, DefaultLargeSize, CreateStackedBarChartLarge));
			list.Add(new ImageInfo(CreateStackedLineChart, imageFlags, DefaultSmallSize, CreateStackedLineChart));
			list.Add(new ImageInfo(CreateStackedLineChartLarge, imageFlags, DefaultLargeSize, CreateStackedLineChartLarge));
			list.Add(new ImageInfo(CreateStockChart, imageFlags, DefaultSmallSize, CreateStockChart));
			list.Add(new ImageInfo(CreateStockChartLarge, imageFlags, DefaultLargeSize, CreateStockChartLarge));
			list.Add(new ImageInfo(Cut, imageFlags, DefaultSmallSize, Cut));
			list.Add(new ImageInfo(CutLarge, imageFlags, DefaultLargeSize, CutLarge));
			list.Add(new ImageInfo(Delete_Hyperlink, imageFlags, DefaultSmallSize, Delete_Hyperlink));
			list.Add(new ImageInfo(Delete_HyperlinkLarge, imageFlags, DefaultLargeSize, Delete_HyperlinkLarge));
			list.Add(new ImageInfo(DeleteComment, imageFlags, DefaultSmallSize, DeleteComment));
			list.Add(new ImageInfo(DeleteCommentLarge, imageFlags, DefaultLargeSize, DeleteCommentLarge));
			list.Add(new ImageInfo(EditComment, imageFlags, DefaultSmallSize, EditComment));
			list.Add(new ImageInfo(EditCommentLarge, imageFlags, DefaultLargeSize, EditCommentLarge));
			list.Add(new ImageInfo(EditingFillDown, imageFlags, DefaultSmallSize, EditingFillDown));
			list.Add(new ImageInfo(EditingFillDownLarge, imageFlags, DefaultLargeSize, EditingFillDownLarge));
			list.Add(new ImageInfo(EditingFillLeft, imageFlags, DefaultSmallSize, EditingFillLeft));
			list.Add(new ImageInfo(EditingFillLeftLarge, imageFlags, DefaultLargeSize, EditingFillLeftLarge));
			list.Add(new ImageInfo(EditingFillRight, imageFlags, DefaultSmallSize, EditingFillRight));
			list.Add(new ImageInfo(EditingFillRightLarge, imageFlags, DefaultLargeSize, EditingFillRightLarge));
			list.Add(new ImageInfo(EditingFillUp, imageFlags, DefaultSmallSize, EditingFillUp));
			list.Add(new ImageInfo(EditingFillUpLarge, imageFlags, DefaultLargeSize, EditingFillUpLarge));
			list.Add(new ImageInfo(EditingMergeAndCenterCells, imageFlags, DefaultSmallSize, EditingMergeAndCenterCells));
			list.Add(new ImageInfo(EditingMergeAndCenterCellsLarge, imageFlags, DefaultLargeSize, EditingMergeAndCenterCellsLarge));
			list.Add(new ImageInfo(EditingMergeCells, imageFlags, DefaultSmallSize, EditingMergeCells));
			list.Add(new ImageInfo(EditingMergeCellsAcross, imageFlags, DefaultSmallSize, EditingMergeCellsAcross));
			list.Add(new ImageInfo(EditingMergeCellsAcrossLarge, imageFlags, DefaultLargeSize, EditingMergeCellsAcrossLarge));
			list.Add(new ImageInfo(EditingMergeCellsLarge, imageFlags, DefaultLargeSize, EditingMergeCellsLarge));
			list.Add(new ImageInfo(EditingUnmergeCells, imageFlags, DefaultSmallSize, EditingUnmergeCells));
			list.Add(new ImageInfo(EditingUnmergeCellsLarge, imageFlags, DefaultLargeSize, EditingUnmergeCellsLarge));
			list.Add(new ImageInfo(Find, imageFlags, DefaultSmallSize, Find));
			list.Add(new ImageInfo(FindLarge, imageFlags, DefaultLargeSize, FindLarge));
			list.Add(new ImageInfo(FontColor, imageFlags, DefaultSmallSize, FontColor));
			list.Add(new ImageInfo(FontColorLarge, imageFlags, DefaultLargeSize, FontColorLarge));
			list.Add(new ImageInfo(FontSizeDecrease, imageFlags, DefaultSmallSize, FontSizeDecrease));
			list.Add(new ImageInfo(FontSizeDecreaseLarge, imageFlags, DefaultLargeSize, FontSizeDecreaseLarge));
			list.Add(new ImageInfo(FontSizeIncrease, imageFlags, DefaultSmallSize, FontSizeIncrease));
			list.Add(new ImageInfo(FontSizeIncreaseLarge, imageFlags, DefaultLargeSize, FontSizeIncreaseLarge));
			list.Add(new ImageInfo(Format, imageFlags, DefaultSmallSize, Format));
			list.Add(new ImageInfo(FormatLarge, imageFlags, DefaultLargeSize, FormatLarge));
			list.Add(new ImageInfo(FormatNumberCommaStyle, imageFlags, DefaultSmallSize, FormatNumberCommaStyle));
			list.Add(new ImageInfo(FormatNumberCommaStyleLarge, imageFlags, DefaultLargeSize, FormatNumberCommaStyleLarge));
			list.Add(new ImageInfo(FormatNumberCurrency, imageFlags, DefaultSmallSize, FormatNumberCurrency));
			list.Add(new ImageInfo(FormatNumberCurrencyLarge, imageFlags, DefaultLargeSize, FormatNumberCurrencyLarge));
			list.Add(new ImageInfo(FormatNumberDecreaseDecimal, imageFlags, DefaultSmallSize, FormatNumberDecreaseDecimal));
			list.Add(new ImageInfo(FormatNumberDecreaseDecimalLarge, imageFlags, DefaultLargeSize, FormatNumberDecreaseDecimalLarge));
			list.Add(new ImageInfo(FormatNumberIncreaseDecimal, imageFlags, DefaultSmallSize, FormatNumberIncreaseDecimal));
			list.Add(new ImageInfo(FormatNumberIncreaseDecimalLarge, imageFlags, DefaultLargeSize, FormatNumberIncreaseDecimalLarge));
			list.Add(new ImageInfo(FormatNumberPercent, imageFlags, DefaultSmallSize, FormatNumberPercent));
			list.Add(new ImageInfo(FormatNumberPercentLarge, imageFlags, DefaultLargeSize, FormatNumberPercentLarge));
			list.Add(new ImageInfo(FormatWrapText, imageFlags, DefaultSmallSize, FormatWrapText));
			list.Add(new ImageInfo(FormatWrapTextLarge, imageFlags, DefaultLargeSize, FormatWrapTextLarge));
			list.Add(new ImageInfo(FunctionsCompatibility, imageFlags, DefaultSmallSize, FunctionsCompatibility));
			list.Add(new ImageInfo(FunctionsCompatibilityLarge, imageFlags, DefaultLargeSize, FunctionsCompatibilityLarge));
			list.Add(new ImageInfo(FunctionsCube, imageFlags, DefaultSmallSize, FunctionsCube));
			list.Add(new ImageInfo(FunctionsCubeLarge, imageFlags, DefaultLargeSize, FunctionsCubeLarge));
			list.Add(new ImageInfo(FunctionsDateAndTime, imageFlags, DefaultSmallSize, FunctionsDateAndTime));
			list.Add(new ImageInfo(FunctionsDateAndTimeLarge, imageFlags, DefaultLargeSize, FunctionsDateAndTimeLarge));
			list.Add(new ImageInfo(FunctionsEngineering, imageFlags, DefaultSmallSize, FunctionsEngineering));
			list.Add(new ImageInfo(FunctionsEngineeringLarge, imageFlags, DefaultLargeSize, FunctionsEngineeringLarge));
			list.Add(new ImageInfo(FunctionsFinancial, imageFlags, DefaultSmallSize, FunctionsFinancial));
			list.Add(new ImageInfo(FunctionsFinancialLarge, imageFlags, DefaultLargeSize, FunctionsFinancialLarge));
			list.Add(new ImageInfo(FunctionsInformation, imageFlags, DefaultSmallSize, FunctionsInformation));
			list.Add(new ImageInfo(FunctionsInformationLarge, imageFlags, DefaultLargeSize, FunctionsInformationLarge));
			list.Add(new ImageInfo(FunctionsLogical, imageFlags, DefaultSmallSize, FunctionsLogical));
			list.Add(new ImageInfo(FunctionsLogicalLarge, imageFlags, DefaultLargeSize, FunctionsLogicalLarge));
			list.Add(new ImageInfo(FunctionsLookupAndReference, imageFlags, DefaultSmallSize, FunctionsLookupAndReference));
			list.Add(new ImageInfo(FunctionsLookupAndReferenceLarge, imageFlags, DefaultLargeSize, FunctionsLookupAndReferenceLarge));
			list.Add(new ImageInfo(FunctionsMathAndTrigonometry, imageFlags, DefaultSmallSize, FunctionsMathAndTrigonometry));
			list.Add(new ImageInfo(FunctionsMathAndTrigonometryLarge, imageFlags, DefaultLargeSize, FunctionsMathAndTrigonometryLarge));
			list.Add(new ImageInfo(FunctionsMore, imageFlags, DefaultSmallSize, FunctionsMore));
			list.Add(new ImageInfo(FunctionsMoreLarge, imageFlags, DefaultLargeSize, FunctionsMoreLarge));
			list.Add(new ImageInfo(FunctionsStatistical, imageFlags, DefaultSmallSize, FunctionsStatistical));
			list.Add(new ImageInfo(FunctionsStatisticalLarge, imageFlags, DefaultLargeSize, FunctionsStatisticalLarge));
			list.Add(new ImageInfo(FunctionsText, imageFlags, DefaultSmallSize, FunctionsText));
			list.Add(new ImageInfo(FunctionsTextLarge, imageFlags, DefaultLargeSize, FunctionsTextLarge));
			list.Add(new ImageInfo(FunctionSum, imageFlags, DefaultSmallSize, FunctionSum));
			list.Add(new ImageInfo(FunctionSumLarge, imageFlags, DefaultLargeSize, FunctionSumLarge));
			list.Add(new ImageInfo(Hyperlink, imageFlags, DefaultSmallSize, Hyperlink));
			list.Add(new ImageInfo(HyperlinkLarge, imageFlags, DefaultLargeSize, HyperlinkLarge));
			list.Add(new ImageInfo(ImagePlaceHolder, imageFlags, DefaultSmallSize, ImagePlaceHolder));
			list.Add(new ImageInfo(IndentDecrease, imageFlags, DefaultSmallSize, IndentDecrease));
			list.Add(new ImageInfo(IndentDecreaseLarge, imageFlags, DefaultLargeSize, IndentDecreaseLarge));
			list.Add(new ImageInfo(IndentIncrease, imageFlags, DefaultSmallSize, IndentIncrease));
			list.Add(new ImageInfo(IndentIncreaseLarge, imageFlags, DefaultLargeSize, IndentIncreaseLarge));
			list.Add(new ImageInfo(InsertCellsCommandGroup, imageFlags, DefaultSmallSize, InsertCellsCommandGroup));
			list.Add(new ImageInfo(InsertCellsCommandGroupLarge, imageFlags, DefaultLargeSize, InsertCellsCommandGroupLarge));
			list.Add(new ImageInfo(InsertComment, imageFlags, DefaultSmallSize, InsertComment));
			list.Add(new ImageInfo(InsertCommentLarge, imageFlags, DefaultLargeSize, InsertCommentLarge));
			list.Add(new ImageInfo(InsertFloatingObjectImage, imageFlags, DefaultSmallSize, InsertFloatingObjectImage));
			list.Add(new ImageInfo(InsertFloatingObjectImageLarge, imageFlags, DefaultLargeSize, InsertFloatingObjectImageLarge));
			list.Add(new ImageInfo(InsertSheet, imageFlags, DefaultSmallSize, InsertSheet));
			list.Add(new ImageInfo(InsertSheetColumns, imageFlags, DefaultSmallSize, InsertSheetColumns));
			list.Add(new ImageInfo(InsertSheetColumnsLarge, imageFlags, DefaultLargeSize, InsertSheetColumnsLarge));
			list.Add(new ImageInfo(InsertSheetLarge, imageFlags, DefaultLargeSize, InsertSheetLarge));
			list.Add(new ImageInfo(InsertSheetRows, imageFlags, DefaultSmallSize, InsertSheetRows));
			list.Add(new ImageInfo(InsertSheetRowsLarge, imageFlags, DefaultLargeSize, InsertSheetRowsLarge));
			list.Add(new ImageInfo(InsertTable, imageFlags, DefaultSmallSize, InsertTable));
			list.Add(new ImageInfo(InsertTableLarge, imageFlags, DefaultLargeSize, InsertTableLarge));
			list.Add(new ImageInfo(Italic, imageFlags, DefaultSmallSize, Italic));
			list.Add(new ImageInfo(ItalicLarge, imageFlags, DefaultLargeSize, ItalicLarge));
			list.Add(new ImageInfo(New, imageFlags, DefaultSmallSize, New));
			list.Add(new ImageInfo(NewLarge, imageFlags, DefaultLargeSize, NewLarge));
			list.Add(new ImageInfo(Open, imageFlags, DefaultSmallSize, Open));
			list.Add(new ImageInfo(OpenHyperlink, imageFlags, DefaultSmallSize, OpenHyperlink));
			list.Add(new ImageInfo(OpenHyperlinkLarge, imageFlags, DefaultLargeSize, OpenHyperlinkLarge));
			list.Add(new ImageInfo(OpenLarge, imageFlags, DefaultLargeSize, OpenLarge));
			list.Add(new ImageInfo(PageMargins, imageFlags, DefaultSmallSize, PageMargins));
			list.Add(new ImageInfo(PageMarginsLarge, imageFlags, DefaultLargeSize, PageMarginsLarge));
			list.Add(new ImageInfo(PageMarginsModerate, imageFlags, DefaultSmallSize, PageMarginsModerate));
			list.Add(new ImageInfo(PageMarginsModerateLarge, imageFlags, DefaultLargeSize, PageMarginsModerateLarge));
			list.Add(new ImageInfo(PageMarginsNarrow, imageFlags, DefaultSmallSize, PageMarginsNarrow));
			list.Add(new ImageInfo(PageMarginsNarrowLarge, imageFlags, DefaultLargeSize, PageMarginsNarrowLarge));
			list.Add(new ImageInfo(PageMarginsNormal, imageFlags, DefaultSmallSize, PageMarginsNormal));
			list.Add(new ImageInfo(PageMarginsNormalLarge, imageFlags, DefaultLargeSize, PageMarginsNormalLarge));
			list.Add(new ImageInfo(PageMarginsWide, imageFlags, DefaultSmallSize, PageMarginsWide));
			list.Add(new ImageInfo(PageMarginsWideLarge, imageFlags, DefaultLargeSize, PageMarginsWideLarge));
			list.Add(new ImageInfo(PageOrientation, imageFlags, DefaultSmallSize, PageOrientation));
			list.Add(new ImageInfo(PageOrientationLarge, imageFlags, DefaultLargeSize, PageOrientationLarge));
			list.Add(new ImageInfo(PaperSize, imageFlags, DefaultSmallSize, PaperSize));
			list.Add(new ImageInfo(PaperSizeLarge, imageFlags, DefaultLargeSize, PaperSizeLarge));
			list.Add(new ImageInfo(Paste, imageFlags, DefaultSmallSize, Paste));
			list.Add(new ImageInfo(PasteLarge, imageFlags, DefaultLargeSize, PasteLarge));
			list.Add(new ImageInfo(PasteSpecial, imageFlags, DefaultSmallSize, PasteSpecial));
			list.Add(new ImageInfo(PasteSpecialLarge, imageFlags, DefaultLargeSize, PasteSpecialLarge));
			list.Add(new ImageInfo(SortAsc, imageFlags, DefaultSmallSize, SortAsc));
			list.Add(new ImageInfo(SortAscLarge, imageFlags, DefaultLargeSize, SortAscLarge));
			list.Add(new ImageInfo(SortDesc, imageFlags, DefaultSmallSize, SortDesc));
			list.Add(new ImageInfo(SortDescLarge, imageFlags, DefaultLargeSize, SortDescLarge));
			list.Add(new ImageInfo(PenColor, imageFlags, DefaultSmallSize, PenColor));
			list.Add(new ImageInfo(PenColorLarge, imageFlags, DefaultLargeSize, PenColorLarge));
			list.Add(new ImageInfo(Preview, imageFlags, DefaultSmallSize, Preview));
			list.Add(new ImageInfo(PreviewLarge, imageFlags, DefaultLargeSize, PreviewLarge));
			list.Add(new ImageInfo(Print, imageFlags, DefaultSmallSize, Print));
			list.Add(new ImageInfo(PrintDialog, imageFlags, DefaultSmallSize, PrintDialog));
			list.Add(new ImageInfo(PrintDialogLarge, imageFlags, DefaultLargeSize, PrintDialogLarge));
			list.Add(new ImageInfo(PrintLarge, imageFlags, DefaultLargeSize, PrintLarge));
			list.Add(new ImageInfo(Redo, imageFlags, DefaultSmallSize, Redo));
			list.Add(new ImageInfo(RedoLarge, imageFlags, DefaultLargeSize, RedoLarge));
			list.Add(new ImageInfo(RemoveCellsCommandGroup, imageFlags, DefaultSmallSize, RemoveCellsCommandGroup));
			list.Add(new ImageInfo(RemoveCellsCommandGroupLarge, imageFlags, DefaultLargeSize, RemoveCellsCommandGroupLarge));
			list.Add(new ImageInfo(RemoveSheet, imageFlags, DefaultSmallSize, RemoveSheet));
			list.Add(new ImageInfo(RemoveSheetColumns, imageFlags, DefaultSmallSize, RemoveSheetColumns));
			list.Add(new ImageInfo(RemoveSheetColumnsLarge, imageFlags, DefaultLargeSize, RemoveSheetColumnsLarge));
			list.Add(new ImageInfo(RemoveSheetLarge, imageFlags, DefaultLargeSize, RemoveSheetLarge));
			list.Add(new ImageInfo(RemoveSheetRows, imageFlags, DefaultSmallSize, RemoveSheetRows));
			list.Add(new ImageInfo(RemoveSheetRowsLarge, imageFlags, DefaultLargeSize, RemoveSheetRowsLarge));
			list.Add(new ImageInfo(Save, imageFlags, DefaultSmallSize, Save));
			list.Add(new ImageInfo(SaveAll, imageFlags, DefaultSmallSize, SaveAll));
			list.Add(new ImageInfo(SaveAllLarge, imageFlags, DefaultLargeSize, SaveAllLarge));
			list.Add(new ImageInfo(SaveAs, imageFlags, DefaultSmallSize, SaveAs));
			list.Add(new ImageInfo(SaveAsLarge, imageFlags, DefaultLargeSize, SaveAsLarge));
			list.Add(new ImageInfo(SaveLarge, imageFlags, DefaultLargeSize, SaveLarge));
			list.Add(new ImageInfo(ShowHideComments, imageFlags, DefaultSmallSize, ShowHideComments));
			list.Add(new ImageInfo(ShowHideCommentsLarge, imageFlags, DefaultLargeSize, ShowHideCommentsLarge));
			list.Add(new ImageInfo(Strikeout, imageFlags, DefaultSmallSize, Strikeout));
			list.Add(new ImageInfo(StrikeoutLarge, imageFlags, DefaultLargeSize, StrikeoutLarge));
			list.Add(new ImageInfo(Underline, imageFlags, DefaultSmallSize, Underline));
			list.Add(new ImageInfo(UnderlineDouble, imageFlags, DefaultSmallSize, UnderlineDouble));
			list.Add(new ImageInfo(UnderlineDoubleLarge, imageFlags, DefaultLargeSize, UnderlineDoubleLarge));
			list.Add(new ImageInfo(UnderlineLarge, imageFlags, DefaultLargeSize, UnderlineLarge));
			list.Add(new ImageInfo(Undo, imageFlags, DefaultSmallSize, Undo));
			list.Add(new ImageInfo(UndoLarge, imageFlags, DefaultLargeSize, UndoLarge));
			list.Add(new ImageInfo(Zoom100Percent, imageFlags, DefaultSmallSize, Zoom100Percent));
			list.Add(new ImageInfo(Zoom100PercentLarge, imageFlags, DefaultLargeSize, Zoom100PercentLarge));
			list.Add(new ImageInfo(ZoomIn, imageFlags, DefaultSmallSize, ZoomIn));
			list.Add(new ImageInfo(ZoomInLarge, imageFlags, DefaultLargeSize, ZoomInLarge));
			list.Add(new ImageInfo(ZoomOut, imageFlags, DefaultSmallSize, ZoomOut));
			list.Add(new ImageInfo(ZoomOutLarge, imageFlags, DefaultLargeSize, ZoomOutLarge));
			list.Add(new ImageInfo(CreateRotatedBar3DChart, imageFlags, DefaultSmallSize, CreateRotatedBar3DChart));
			list.Add(new ImageInfo(CreateRotatedStackedBar3DChart, imageFlags, DefaultSmallSize, CreateRotatedStackedBar3DChart));
			list.Add(new ImageInfo(CreateRotatedFullStackedBar3DChart, imageFlags, DefaultSmallSize, CreateRotatedFullStackedBar3DChart));
			list.Add(new ImageInfo(CreateRotatedCylinderBar3DChart, imageFlags, DefaultSmallSize, CreateRotatedCylinderBar3DChart));
			list.Add(new ImageInfo(CreateRotatedStackedCylinderBar3DChart, imageFlags, DefaultSmallSize, CreateRotatedStackedCylinderBar3DChart));
			list.Add(new ImageInfo(CreateRotatedFullStackedCylinderBar3DChar, imageFlags, DefaultSmallSize, CreateRotatedFullStackedCylinderBar3DChar));
			list.Add(new ImageInfo(CreateRotatedConeBar3DChart, imageFlags, DefaultSmallSize, CreateRotatedConeBar3DChart));
			list.Add(new ImageInfo(CreateRotatedStackedConeBar3DChart, imageFlags, DefaultSmallSize, CreateRotatedStackedConeBar3DChart));
			list.Add(new ImageInfo(CreateRotatedFullStackedConeBar3DChart, imageFlags, DefaultSmallSize, CreateRotatedFullStackedConeBar3DChart));
			list.Add(new ImageInfo(CreateRotatedPyramidBar3DChart, imageFlags, DefaultSmallSize, CreateRotatedPyramidBar3DChart));
			list.Add(new ImageInfo(CreateRotatedStackedPyramidBar3DChart, imageFlags, DefaultSmallSize, CreateRotatedStackedPyramidBar3DChart));
			list.Add(new ImageInfo(CreateRotatedFullStackedPyramidBar3DChart, imageFlags, DefaultSmallSize, CreateRotatedFullStackedPyramidBar3DChart));
			list.Add(new ImageInfo(CreateLineChartNoMarkers, imageFlags, DefaultSmallSize, CreateLineChartNoMarkers));
			list.Add(new ImageInfo(CreateStackedLineChartNoMarkers, imageFlags, DefaultSmallSize, CreateStackedLineChartNoMarkers));
			list.Add(new ImageInfo(CreateFullStackedLineChartNoMarkers, imageFlags, DefaultSmallSize, CreateFullStackedLineChartNoMarkers));
			list.Add(new ImageInfo(CreateScatterChartMarkersOnly, imageFlags, DefaultSmallSize, CreateScatterChartMarkersOnly));
			list.Add(new ImageInfo(CreateScatterChartLines, imageFlags, DefaultSmallSize, CreateScatterChartLines));
			list.Add(new ImageInfo(CreateScatterChartSmoothLines, imageFlags, DefaultSmallSize, CreateScatterChartSmoothLines));
			list.Add(new ImageInfo(CreateScatterChartLinesAndMarkers, imageFlags, DefaultSmallSize, CreateScatterChartLinesAndMarkers));
			list.Add(new ImageInfo(CreateScatterChartSmoothLinesAndMarkers, imageFlags, DefaultSmallSize, CreateScatterChartSmoothLinesAndMarkers));
			list.Add(new ImageInfo(CreateBubble3DChart, imageFlags, DefaultSmallSize, CreateBubble3DChart));
			list.Add(new ImageInfo(CreateRadarLineChartNoMarkers, imageFlags, DefaultSmallSize, CreateRadarLineChartNoMarkers));
			list.Add(new ImageInfo(CreateRadarLineChartFilled, imageFlags, DefaultSmallSize, CreateRadarLineChartFilled));
			list.Add(new ImageInfo(CreateStockChartOpenHighLowClose, imageFlags, DefaultSmallSize, CreateStockChartOpenHighLowClose));
			list.Add(new ImageInfo(CreateExplodedPieChart, imageFlags, DefaultSmallSize, CreateExplodedPieChart));
			list.Add(new ImageInfo(CreateExplodedPie3DChart, imageFlags, DefaultSmallSize, CreateExplodedPie3DChart));
			list.Add(new ImageInfo(CreateExplodedDoughnutChart, imageFlags, DefaultSmallSize, CreateExplodedDoughnutChart));
			list.Add(new ImageInfo(CreateStockChartHighLowClose, imageFlags, DefaultSmallSize, CreateStockChartHighLowClose));
			list.Add(new ImageInfo(CreateRotatedStackedBar3DChartLarge, imageFlags, DefaultLargeSize, CreateRotatedStackedBar3DChartLarge));
			list.Add(new ImageInfo(CreateRotatedFullStackedBar3DChartLarge, imageFlags, DefaultLargeSize, CreateRotatedFullStackedBar3DChartLarge));
			list.Add(new ImageInfo(CreateRotatedCylinderBar3DChartLarge, imageFlags, DefaultLargeSize, CreateRotatedCylinderBar3DChartLarge));
			list.Add(new ImageInfo(CreateRotatedStackedCylinderBar3DChartLarge, imageFlags, DefaultLargeSize, CreateRotatedStackedCylinderBar3DChartLarge));
			list.Add(new ImageInfo(CreateRotatedFullStackedCylinderBar3DCharLarge, imageFlags, DefaultLargeSize, CreateRotatedFullStackedCylinderBar3DCharLarge));
			list.Add(new ImageInfo(CreateRotatedConeBar3DChartLarge, imageFlags, DefaultLargeSize, CreateRotatedConeBar3DChartLarge));
			list.Add(new ImageInfo(CreateRotatedStackedConeBar3DChartLarge, imageFlags, DefaultLargeSize, CreateRotatedStackedConeBar3DChartLarge));
			list.Add(new ImageInfo(CreateRotatedFullStackedConeBar3DChartLarge, imageFlags, DefaultLargeSize, CreateRotatedFullStackedConeBar3DChartLarge));
			list.Add(new ImageInfo(CreateRotatedPyramidBar3DChartLarge, imageFlags, DefaultLargeSize, CreateRotatedPyramidBar3DChartLarge));
			list.Add(new ImageInfo(CreateRotatedStackedPyramidBar3DChartLarge, imageFlags, DefaultLargeSize, CreateRotatedStackedPyramidBar3DChartLarge));
			list.Add(new ImageInfo(CreateRotatedFullStackedPyramidBar3DChartLarge, imageFlags, DefaultLargeSize, CreateRotatedFullStackedPyramidBar3DChartLarge));
			list.Add(new ImageInfo(CreateLineChartNoMarkersLarge, imageFlags, DefaultLargeSize, CreateLineChartNoMarkersLarge));
			list.Add(new ImageInfo(CreateStackedLineChartNoMarkersLarge, imageFlags, DefaultLargeSize, CreateStackedLineChartNoMarkersLarge));
			list.Add(new ImageInfo(CreateFullStackedLineChartNoMarkersLarge, imageFlags, DefaultLargeSize, CreateFullStackedLineChartNoMarkersLarge));
			list.Add(new ImageInfo(CreateScatterChartMarkersOnlyLarge, imageFlags, DefaultLargeSize, CreateScatterChartMarkersOnlyLarge));
			list.Add(new ImageInfo(CreateScatterChartLinesLarge, imageFlags, DefaultLargeSize, CreateScatterChartLinesLarge));
			list.Add(new ImageInfo(CreateScatterChartSmoothLinesLarge, imageFlags, DefaultLargeSize, CreateScatterChartSmoothLinesLarge));
			list.Add(new ImageInfo(CreateScatterChartLinesAndMarkersLarge, imageFlags, DefaultLargeSize, CreateScatterChartLinesAndMarkersLarge));
			list.Add(new ImageInfo(CreateScatterChartSmoothLinesAndMarkersLarge, imageFlags, DefaultLargeSize, CreateScatterChartSmoothLinesAndMarkersLarge));
			list.Add(new ImageInfo(CreateBubble3DChartLarge, imageFlags, DefaultLargeSize, CreateBubble3DChartLarge));
			list.Add(new ImageInfo(CreateRadarLineChartNoMarkersLarge, imageFlags, DefaultLargeSize, CreateRadarLineChartNoMarkersLarge));
			list.Add(new ImageInfo(CreateRadarLineChartFilledLarge, imageFlags, DefaultLargeSize, CreateRadarLineChartFilledLarge));
			list.Add(new ImageInfo(CreateStockChartOpenHighLowCloseLarge, imageFlags, DefaultLargeSize, CreateStockChartOpenHighLowCloseLarge));
			list.Add(new ImageInfo(CreateExplodedPieChartLarge, imageFlags, DefaultLargeSize, CreateExplodedPieChartLarge));
			list.Add(new ImageInfo(CreateExplodedPie3DChartLarge, imageFlags, DefaultLargeSize, CreateExplodedPie3DChartLarge));
			list.Add(new ImageInfo(CreateExplodedDoughnutChartLarge, imageFlags, DefaultLargeSize, CreateExplodedDoughnutChartLarge));
			list.Add(new ImageInfo(CreateStockChartHighLowCloseLarge, imageFlags, DefaultLargeSize, CreateStockChartHighLowCloseLarge));
			list.Add(new ImageInfo(ChartGroupColumn, imageFlags, DefaultSmallSize, ChartGroupColumn));
			list.Add(new ImageInfo(ChartGroupColumnLarge, imageFlags, DefaultLargeSize, ChartGroupColumnLarge));
			list.Add(new ImageInfo(ChartGroupLine, imageFlags, DefaultSmallSize, ChartGroupLine));
			list.Add(new ImageInfo(ChartGroupLineLarge, imageFlags, DefaultLargeSize, ChartGroupLineLarge));
			list.Add(new ImageInfo(ChartGroupPie, imageFlags, DefaultSmallSize, ChartGroupPie));
			list.Add(new ImageInfo(ChartGroupPieLarge, imageFlags, DefaultLargeSize, ChartGroupPieLarge));
			list.Add(new ImageInfo(ChartGroupColumn, imageFlags, DefaultLargeSize, ChartGroupColumn));
			list.Add(new ImageInfo(ChartGroupColumnLarge, imageFlags, DefaultLargeSize, ChartGroupColumnLarge));
			list.Add(new ImageInfo(ChartGroupLine, imageFlags, DefaultLargeSize, ChartGroupLine));
			list.Add(new ImageInfo(ChartGroupLineLarge, imageFlags, DefaultLargeSize, ChartGroupLineLarge));
			list.Add(new ImageInfo(ChartGroupPie, imageFlags, DefaultLargeSize, ChartGroupPie));
			list.Add(new ImageInfo(ChartGroupPieLarge, imageFlags, DefaultLargeSize, ChartGroupPieLarge));
			list.Add(new ImageInfo(ChartGroupBar, imageFlags, DefaultLargeSize, ChartGroupBar));
			list.Add(new ImageInfo(ChartGroupBarLarge, imageFlags, DefaultLargeSize, ChartGroupBarLarge));
			list.Add(new ImageInfo(ChartGroupArea, imageFlags, DefaultLargeSize, ChartGroupArea));
			list.Add(new ImageInfo(ChartGroupAreaLarge, imageFlags, DefaultLargeSize, ChartGroupAreaLarge));
			list.Add(new ImageInfo(ChartGroupScatter, imageFlags, DefaultLargeSize, ChartGroupScatter));
			list.Add(new ImageInfo(ChartGroupScatterLarge, imageFlags, DefaultLargeSize, ChartGroupScatterLarge));
			list.Add(new ImageInfo(ChartGroupDoughnut, imageFlags, DefaultLargeSize, ChartGroupDoughnut));
			list.Add(new ImageInfo(ChartGroupDoughnutLarge, imageFlags, DefaultLargeSize, ChartGroupDoughnutLarge));
			list.Add(new ImageInfo(ChartGroupOther, imageFlags, DefaultLargeSize, ChartGroupOther));
			list.Add(new ImageInfo(ChartGroupOtherLarge, imageFlags, DefaultLargeSize, ChartGroupOtherLarge));
			list.Add(new ImageInfo(CreateRotatedBar3DChartLarge, imageFlags, DefaultLargeSize, CreateRotatedBar3DChartLarge));
			list.Add(new ImageInfo(ShowFormulasLarge, imageFlags, DefaultLargeSize, ShowFormulasLarge));
			list.Add(new ImageInfo(ShowFormulas, imageFlags, DefaultSmallSize, ShowFormulas));
			list.Add(new ImageInfo(FontGroupLarge, imageFlags, DefaultLargeSize, FontGroupLarge));
			list.Add(new ImageInfo(FontGroup, imageFlags, DefaultSmallSize, FontGroup));
			list.Add(new ImageInfo(BorderLineStyleLarge, imageFlags, DefaultLargeSize, BorderLineStyleLarge));
			list.Add(new ImageInfo(BorderLineStyle, imageFlags, DefaultSmallSize, BorderLineStyle));
			list.Add(new ImageInfo(SortAndFilterLarge, imageFlags, DefaultLargeSize, SortAndFilterLarge));
			list.Add(new ImageInfo(SortAndFilter, imageFlags, DefaultSmallSize, SortAndFilter));
			list.Add(new ImageInfo(Filter, imageFlags, DefaultLargeSize, Filter));
			list.Add(new ImageInfo(FilterLarge, imageFlags, DefaultLargeSize, FilterLarge));
			list.Add(new ImageInfo(ClearFilter, imageFlags, DefaultLargeSize, ClearFilter));
			list.Add(new ImageInfo(ClearFilterLarge, imageFlags, DefaultLargeSize, ClearFilterLarge));
			list.Add(new ImageInfo(ReapplyFilter, imageFlags, DefaultLargeSize, ReapplyFilter));
			list.Add(new ImageInfo(ReapplyFilterLarge, imageFlags, DefaultLargeSize, ReapplyFilterLarge));
			list.Add(new ImageInfo(TableConvertToRange, imageFlags, DefaultSmallSize, TableConvertToRange));
			list.Add(new ImageInfo(TableConvertToRangeLarge, imageFlags, DefaultLargeSize, TableConvertToRangeLarge));
			list.Add(new ImageInfo(CalculateNow, imageFlags, DefaultSmallSize, CalculateNow));
			list.Add(new ImageInfo(CalculateNowLarge, imageFlags, DefaultLargeSize, CalculateNowLarge));
			list.Add(new ImageInfo(CalculateSheet, imageFlags, DefaultSmallSize, CalculateSheet));
			list.Add(new ImageInfo(CalculateSheetLarge, imageFlags, DefaultLargeSize, CalculateSheetLarge));
			list.Add(new ImageInfo(CalculationOptions, imageFlags, DefaultSmallSize, CalculationOptions));
			list.Add(new ImageInfo(CalculationOptionsLarge, imageFlags, DefaultLargeSize, CalculationOptionsLarge));
			list.Add(new ImageInfo(FullScreen, imageFlags, DefaultSmallSize, FullScreen));
			list.Add(new ImageInfo(FullScreenLarge, imageFlags, DefaultLargeSize, FullScreenLarge));
			list.Add(new ImageInfo(PrintArea, imageFlags, DefaultSmallSize, PrintArea));
			list.Add(new ImageInfo(PrintAreaLarge, imageFlags, DefaultLargeSize, PrintAreaLarge));
			list.Add(new ImageInfo(PageOrientationPortrait, imageFlags, DefaultSmallSize, PageOrientationPortrait));
			list.Add(new ImageInfo(PageOrientationPortraitLarge, imageFlags, DefaultLargeSize, PageOrientationPortraitLarge));
			list.Add(new ImageInfo(PageOrientationLandscape, imageFlags, DefaultSmallSize, PageOrientationLandscape));
			list.Add(new ImageInfo(PageOrientationLandscapeLarge, imageFlags, DefaultLargeSize, PageOrientationLandscapeLarge));
			list.Add(new ImageInfo(FloatingObjectBringForward, imageFlags, DefaultSmallSize, FloatingObjectBringForward));
			list.Add(new ImageInfo(FloatingObjectBringToFront, imageFlags, DefaultSmallSize, FloatingObjectBringToFront));
			list.Add(new ImageInfo(FloatingObjectSendBackward, imageFlags, DefaultSmallSize, FloatingObjectSendBackward));
			list.Add(new ImageInfo(FloatingObjectSendToBack, imageFlags, DefaultSmallSize, FloatingObjectSendToBack));
			list.Add(new ImageInfo(FloatingObjectBringForwardLarge, imageFlags, DefaultLargeSize, FloatingObjectBringForwardLarge));
			list.Add(new ImageInfo(FloatingObjectBringToFrontLarge, imageFlags, DefaultLargeSize, FloatingObjectBringToFrontLarge));
			list.Add(new ImageInfo(FloatingObjectSendBackwardLarge, imageFlags, DefaultLargeSize, FloatingObjectSendBackwardLarge));
			list.Add(new ImageInfo(FloatingObjectSendToBackLarge, imageFlags, DefaultLargeSize, FloatingObjectSendToBackLarge));
			list.Add(new ImageInfo(FreezeFirstColumn, imageFlags, DefaultSmallSize, FreezeFirstColumn));
			list.Add(new ImageInfo(FreezeFirstColumnLarge, imageFlags, DefaultLargeSize, FreezeFirstColumnLarge));
			list.Add(new ImageInfo(FreezePanes, imageFlags, DefaultSmallSize, FreezePanes));
			list.Add(new ImageInfo(FreezePanesLarge, imageFlags, DefaultLargeSize, FreezePanesLarge));
			list.Add(new ImageInfo(FreezeTopRow, imageFlags, DefaultSmallSize, FreezeTopRow));
			list.Add(new ImageInfo(FreezeTopRowLarge, imageFlags, DefaultLargeSize, FreezeTopRowLarge));
			list.Add(new ImageInfo(UnfreezePanes, imageFlags, DefaultSmallSize, UnfreezePanes));
			list.Add(new ImageInfo(UnfreezePanesLarge, imageFlags, DefaultLargeSize, UnfreezePanesLarge));
			list.Add(new ImageInfo(DataValidation, imageFlags, DefaultSmallSize, DataValidation));
			list.Add(new ImageInfo(DataValidationLarge, imageFlags, DefaultLargeSize, DataValidationLarge));
			list.Add(new ImageInfo(CircleInvalidData, imageFlags, DefaultSmallSize, CircleInvalidData));
			list.Add(new ImageInfo(CircleInvalidDataLarge, imageFlags, DefaultLargeSize, CircleInvalidDataLarge));
			list.Add(new ImageInfo(ClearValidationCircles, imageFlags, DefaultSmallSize, ClearValidationCircles));
			list.Add(new ImageInfo(ClearValidationCirclesLarge, imageFlags, DefaultLargeSize, ClearValidationCirclesLarge));
			list.Add(new ImageInfo(FormatAsTable, imageFlags, DefaultSmallSize, FormatAsTable));
			list.Add(new ImageInfo(FormatAsTableLarge, imageFlags, DefaultLargeSize, FormatAsTableLarge));
			list.Add(new ImageInfo(ChartSwitchRowColumn, imageFlags, DefaultSmallSize, ChartSwitchRowColumn));
			list.Add(new ImageInfo(ChartSwitchRowColumnLarge, imageFlags, DefaultLargeSize, ChartSwitchRowColumnLarge));
			list.Add(new ImageInfo(ChartSelectData, imageFlags, DefaultSmallSize, ChartSelectData));
			list.Add(new ImageInfo(ChartSelectDataLarge, imageFlags, DefaultLargeSize, ChartSelectDataLarge));
			list.Add(new ImageInfo(ChartTitleNone, imageFlags, DefaultSmallSize, ChartTitleNone));
			list.Add(new ImageInfo(ChartTitleNoneLarge, imageFlags, DefaultLargeSize, ChartTitleNoneLarge));
			list.Add(new ImageInfo(ChartTitleAbove, imageFlags, DefaultSmallSize, ChartTitleAbove));
			list.Add(new ImageInfo(ChartTitleAboveLarge, imageFlags, DefaultLargeSize, ChartTitleAboveLarge));
			list.Add(new ImageInfo(ChartTitleCenteredOverlay, imageFlags, DefaultSmallSize, ChartTitleCenteredOverlay));
			list.Add(new ImageInfo(ChartTitleCenteredOverlayLarge, imageFlags, DefaultLargeSize, ChartTitleCenteredOverlayLarge));
			list.Add(new ImageInfo(ChartAxisTitleGroup, imageFlags, DefaultSmallSize, ChartAxisTitleGroup));
			list.Add(new ImageInfo(ChartAxisTitleGroupLarge, imageFlags, DefaultLargeSize, ChartAxisTitleGroupLarge));
			list.Add(new ImageInfo(ChartAxisTitleHorizontal, imageFlags, DefaultSmallSize, ChartAxisTitleHorizontal));
			list.Add(new ImageInfo(ChartAxisTitleHorizontalLarge, imageFlags, DefaultLargeSize, ChartAxisTitleHorizontalLarge));
			list.Add(new ImageInfo(ChartAxisTitleHorizontal_None, imageFlags, DefaultSmallSize, ChartAxisTitleHorizontal_None));
			list.Add(new ImageInfo(ChartAxisTitleHorizontal_NoneLarge, imageFlags, DefaultLargeSize, ChartAxisTitleHorizontal_NoneLarge));
			list.Add(new ImageInfo(ChartAxisTitleVertical, imageFlags, DefaultSmallSize, ChartAxisTitleVertical));
			list.Add(new ImageInfo(ChartAxisTitleVerticalLarge, imageFlags, DefaultLargeSize, ChartAxisTitleVerticalLarge));
			list.Add(new ImageInfo(ChartAxisTitleVertical_HorizonlalText, imageFlags, DefaultSmallSize, ChartAxisTitleVertical_HorizonlalText));
			list.Add(new ImageInfo(ChartAxisTitleVertical_HorizonlalTextLarge, imageFlags, DefaultLargeSize, ChartAxisTitleVertical_HorizonlalTextLarge));
			list.Add(new ImageInfo(ChartAxisTitleVertical_None, imageFlags, DefaultSmallSize, ChartAxisTitleVertical_None));
			list.Add(new ImageInfo(ChartAxisTitleVertical_NoneLarge, imageFlags, DefaultLargeSize, ChartAxisTitleVertical_NoneLarge));
			list.Add(new ImageInfo(ChartAxisTitleVertical_RotatedText, imageFlags, DefaultSmallSize, ChartAxisTitleVertical_RotatedText));
			list.Add(new ImageInfo(ChartAxisTitleVertical_RotatedTextLarge, imageFlags, DefaultLargeSize, ChartAxisTitleVertical_RotatedTextLarge));
			list.Add(new ImageInfo(ChartAxisTitleVertical_VerticalText, imageFlags, DefaultSmallSize, ChartAxisTitleVertical_VerticalText));
			list.Add(new ImageInfo(ChartAxisTitleVertical_VerticalTextLarge, imageFlags, DefaultLargeSize, ChartAxisTitleVertical_VerticalTextLarge));
			list.Add(new ImageInfo(ChartLegend_Bottom, imageFlags, DefaultSmallSize, ChartLegend_Bottom));
			list.Add(new ImageInfo(ChartLegend_BottomLarge, imageFlags, DefaultLargeSize, ChartLegend_BottomLarge));
			list.Add(new ImageInfo(ChartLegend_Left, imageFlags, DefaultSmallSize, ChartLegend_Left));
			list.Add(new ImageInfo(ChartLegend_LeftLarge, imageFlags, DefaultLargeSize, ChartLegend_LeftLarge));
			list.Add(new ImageInfo(ChartLegend_LeftOverlay, imageFlags, DefaultSmallSize, ChartLegend_LeftOverlay));
			list.Add(new ImageInfo(ChartLegend_LeftOverlayLarge, imageFlags, DefaultLargeSize, ChartLegend_LeftOverlayLarge));
			list.Add(new ImageInfo(ChartLegend_None, imageFlags, DefaultSmallSize, ChartLegend_None));
			list.Add(new ImageInfo(ChartLegend_NoneLarge, imageFlags, DefaultLargeSize, ChartLegend_NoneLarge));
			list.Add(new ImageInfo(ChartLegend_Right, imageFlags, DefaultSmallSize, ChartLegend_Right));
			list.Add(new ImageInfo(ChartLegend_RightLarge, imageFlags, DefaultLargeSize, ChartLegend_RightLarge));
			list.Add(new ImageInfo(ChartLegend_RightOverlay, imageFlags, DefaultSmallSize, ChartLegend_RightOverlay));
			list.Add(new ImageInfo(ChartLegend_RightOverlayLarge, imageFlags, DefaultLargeSize, ChartLegend_RightOverlayLarge));
			list.Add(new ImageInfo(ChartLegend_Top, imageFlags, DefaultSmallSize, ChartLegend_Top));
			list.Add(new ImageInfo(ChartLegend_TopLarge, imageFlags, DefaultLargeSize, ChartLegend_TopLarge));
			list.Add(new ImageInfo(ChartLabels_None, imageFlags, DefaultSmallSize, ChartLabels_None));
			list.Add(new ImageInfo(ChartLabels_NoneLarge, imageFlags, DefaultLargeSize, ChartLabels_NoneLarge));
			list.Add(new ImageInfo(ChartLabels_OutsideEnd, imageFlags, DefaultSmallSize, ChartLabels_OutsideEnd));
			list.Add(new ImageInfo(ChartLabels_OutsideEndLarge, imageFlags, DefaultLargeSize, ChartLabels_OutsideEndLarge));
			list.Add(new ImageInfo(ChartLabels_InsideBase, imageFlags, DefaultSmallSize, ChartLabels_InsideBase));
			list.Add(new ImageInfo(ChartLabels_InsideBaseLarge, imageFlags, DefaultLargeSize, ChartLabels_InsideBaseLarge));
			list.Add(new ImageInfo(ChartLabels_InsideCenter, imageFlags, DefaultSmallSize, ChartLabels_InsideCenter));
			list.Add(new ImageInfo(ChartLabels_InsideCenterLarge, imageFlags, DefaultLargeSize, ChartLabels_InsideCenterLarge));
			list.Add(new ImageInfo(ChartLabels_InsideEnd, imageFlags, DefaultSmallSize, ChartLabels_InsideEnd));
			list.Add(new ImageInfo(ChartLabels_InsideEndLarge, imageFlags, DefaultLargeSize, ChartLabels_InsideEndLarge));
			list.Add(new ImageInfo(ChartAxesGroup, imageFlags, DefaultSmallSize, ChartAxesGroup));
			list.Add(new ImageInfo(ChartAxesGroupLarge, imageFlags, DefaultLargeSize, ChartAxesGroupLarge));
			list.Add(new ImageInfo(ChartHorizontalAxis_LeftToRight, imageFlags, DefaultSmallSize, ChartHorizontalAxis_LeftToRight));
			list.Add(new ImageInfo(ChartHorizontalAxis_LeftToRightLarge, imageFlags, DefaultLargeSize, ChartHorizontalAxis_LeftToRightLarge));
			list.Add(new ImageInfo(ChartHorizontalAxis_None, imageFlags, DefaultSmallSize, ChartHorizontalAxis_None));
			list.Add(new ImageInfo(ChartHorizontalAxis_NoneLarge, imageFlags, DefaultLargeSize, ChartHorizontalAxis_NoneLarge));
			list.Add(new ImageInfo(ChartHorizontalAxis_RightToLeft, imageFlags, DefaultSmallSize, ChartHorizontalAxis_RightToLeft));
			list.Add(new ImageInfo(ChartHorizontalAxis_RightToLeftLarge, imageFlags, DefaultLargeSize, ChartHorizontalAxis_RightToLeftLarge));
			list.Add(new ImageInfo(ChartHorizontalAxis_WithoutLabeling, imageFlags, DefaultSmallSize, ChartHorizontalAxis_WithoutLabeling));
			list.Add(new ImageInfo(ChartHorizontalAxis_WithoutLabelingLarge, imageFlags, DefaultLargeSize, ChartHorizontalAxis_WithoutLabelingLarge));
			list.Add(new ImageInfo(ChartHorizontalAxis_Default, imageFlags, DefaultSmallSize, ChartHorizontalAxis_Default));
			list.Add(new ImageInfo(ChartHorizontalAxis_DefaultLarge, imageFlags, DefaultLargeSize, ChartHorizontalAxis_DefaultLarge));
			list.Add(new ImageInfo(ChartVerticalAxis_None, imageFlags, DefaultSmallSize, ChartVerticalAxis_None));
			list.Add(new ImageInfo(ChartVerticalAxis_NoneLarge, imageFlags, DefaultLargeSize, ChartVerticalAxis_NoneLarge));
			list.Add(new ImageInfo(ChartVerticalAxis_Default, imageFlags, DefaultSmallSize, ChartVerticalAxis_Default));
			list.Add(new ImageInfo(ChartVerticalAxis_DefaultLarge, imageFlags, DefaultLargeSize, ChartVerticalAxis_DefaultLarge));
			list.Add(new ImageInfo(ChartVerticalAxis_Thousands, imageFlags, DefaultSmallSize, ChartVerticalAxis_Thousands));
			list.Add(new ImageInfo(ChartVerticalAxis_ThousandsLarge, imageFlags, DefaultLargeSize, ChartVerticalAxis_ThousandsLarge));
			list.Add(new ImageInfo(ChartVerticalAxis_Millions, imageFlags, DefaultSmallSize, ChartVerticalAxis_Millions));
			list.Add(new ImageInfo(ChartVerticalAxis_MillionsLarge, imageFlags, DefaultLargeSize, ChartVerticalAxis_MillionsLarge));
			list.Add(new ImageInfo(ChartVerticalAxis_Billions, imageFlags, DefaultSmallSize, ChartVerticalAxis_Billions));
			list.Add(new ImageInfo(ChartVerticalAxis_BillionsLarge, imageFlags, DefaultLargeSize, ChartVerticalAxis_BillionsLarge));
			list.Add(new ImageInfo(ChartVerticalAxis_LogScale, imageFlags, DefaultSmallSize, ChartVerticalAxis_LogScale));
			list.Add(new ImageInfo(ChartVerticalAxis_LogScaleLarge, imageFlags, DefaultLargeSize, ChartVerticalAxis_LogScaleLarge));
			list.Add(new ImageInfo(ChartGridlines, imageFlags, DefaultSmallSize, ChartGridlines));
			list.Add(new ImageInfo(ChartGridlinesLarge, imageFlags, DefaultLargeSize, ChartGridlinesLarge));
			list.Add(new ImageInfo(ChartGridlinesHorizontal_Major, imageFlags, DefaultSmallSize, ChartGridlinesHorizontal_Major));
			list.Add(new ImageInfo(ChartGridlinesHorizontal_MajorLarge, imageFlags, DefaultLargeSize, ChartGridlinesHorizontal_MajorLarge));
			list.Add(new ImageInfo(ChartGridlinesHorizontal_MajorMinor, imageFlags, DefaultSmallSize, ChartGridlinesHorizontal_MajorMinor));
			list.Add(new ImageInfo(ChartGridlinesHorizontal_MajorMinorLarge, imageFlags, DefaultLargeSize, ChartGridlinesHorizontal_MajorMinorLarge));
			list.Add(new ImageInfo(ChartGridlinesHorizontal_Minor, imageFlags, DefaultSmallSize, ChartGridlinesHorizontal_Minor));
			list.Add(new ImageInfo(ChartGridlinesHorizontal_MinorLarge, imageFlags, DefaultLargeSize, ChartGridlinesHorizontal_MinorLarge));
			list.Add(new ImageInfo(ChartGridlinesHorizontal_None, imageFlags, DefaultSmallSize, ChartGridlinesHorizontal_None));
			list.Add(new ImageInfo(ChartGridlinesHorizontal_NoneLarge, imageFlags, DefaultLargeSize, ChartGridlinesHorizontal_NoneLarge));
			list.Add(new ImageInfo(ChartGridlinesVertical_Major, imageFlags, DefaultSmallSize, ChartGridlinesVertical_Major));
			list.Add(new ImageInfo(ChartGridlinesVertical_MajorLarge, imageFlags, DefaultLargeSize, ChartGridlinesVertical_MajorLarge));
			list.Add(new ImageInfo(ChartGridlinesVertical_MajorMinor, imageFlags, DefaultSmallSize, ChartGridlinesVertical_MajorMinor));
			list.Add(new ImageInfo(ChartGridlinesVertical_MajorMinorLarge, imageFlags, DefaultLargeSize, ChartGridlinesVertical_MajorMinorLarge));
			list.Add(new ImageInfo(ChartGridlinesVertical_Minor, imageFlags, DefaultSmallSize, ChartGridlinesVertical_Minor));
			list.Add(new ImageInfo(ChartGridlinesVertical_MinorLarge, imageFlags, DefaultLargeSize, ChartGridlinesVertical_MinorLarge));
			list.Add(new ImageInfo(ChartGridlinesVertical_None, imageFlags, DefaultSmallSize, ChartGridlinesVertical_None));
			list.Add(new ImageInfo(ChartGridlinesVertical_NoneLarge, imageFlags, DefaultLargeSize, ChartGridlinesVertical_NoneLarge));
			#endregion
		}
		protected internal override string GetDefaultSpriteName() {
			return RibbonSSSpriteName;
		}
	}
	public class RichEditRibbonImages : ControlRibbonImages {
		public const string RibbonRESpriteName = "RESprite";
		public const string RibbonREGSpriteName = "REGSprite";
		public const string RibbonREGWSpriteName = "REGWSprite";
		public const string LargeImagePostfix = "Large";
		#region ImageNames
		public const string	 AddParagraphToTableOfContents = "AddParagraphToTableOfContents",
								AddParagraphToTableOfContentsLarge = "AddParagraphToTableOfContentsLarge",
								AlignBottomCenter = "AlignBottomCenter",
								AlignBottomCenterLarge = "AlignBottomCenterLarge",
								AlignBottomLeft = "AlignBottomLeft",
								AlignBottomLeftLarge = "AlignBottomLeftLarge",
								AlignBottomRight = "AlignBottomRight",
								AlignBottomRightLarge = "AlignBottomRightLarge",
								AlignCenter = "AlignCenter",
								AlignCenterLarge = "AlignCenterLarge",
								AlignFloatingObjectBottomCenter = "AlignFloatingObjectBottomCenter",
								AlignFloatingObjectBottomCenterLarge = "AlignFloatingObjectBottomCenterLarge",
								AlignFloatingObjectBottomLeft = "AlignFloatingObjectBottomLeft",
								AlignFloatingObjectBottomLeftLarge = "AlignFloatingObjectBottomLeftLarge",
								AlignFloatingObjectBottomRight = "AlignFloatingObjectBottomRight",
								AlignFloatingObjectBottomRightLarge = "AlignFloatingObjectBottomRightLarge",
								AlignFloatingObjectMiddleCenter = "AlignFloatingObjectMiddleCenter",
								AlignFloatingObjectMiddleCenterLarge = "AlignFloatingObjectMiddleCenterLarge",
								AlignFloatingObjectMiddleLeft = "AlignFloatingObjectMiddleLeft",
								AlignFloatingObjectMiddleLeftLarge = "AlignFloatingObjectMiddleLeftLarge",
								AlignFloatingObjectMiddleRight = "AlignFloatingObjectMiddleRight",
								AlignFloatingObjectMiddleRightLarge = "AlignFloatingObjectMiddleRightLarge",
								AlignFloatingObjectTopCenter = "AlignFloatingObjectTopCenter",
								AlignFloatingObjectTopCenterLarge = "AlignFloatingObjectTopCenterLarge",
								AlignFloatingObjectTopLeft = "AlignFloatingObjectTopLeft",
								AlignFloatingObjectTopLeftLarge = "AlignFloatingObjectTopLeftLarge",
								AlignFloatingObjectTopRight = "AlignFloatingObjectTopRight",
								AlignFloatingObjectTopRightLarge = "AlignFloatingObjectTopRightLarge",
								AlignJustify = "AlignJustify",
								AlignJustifyLarge = "AlignJustifyLarge",
								AlignLeft = "AlignLeft",
								AlignLeftLarge = "AlignLeftLarge",
								AlignMiddleCenter = "AlignMiddleCenter",
								AlignMiddleCenterLarge = "AlignMiddleCenterLarge",
								AlignMiddleLeft = "AlignMiddleLeft",
								AlignMiddleLeftLarge = "AlignMiddleLeftLarge",
								AlignMiddleRight = "AlignMiddleRight",
								AlignMiddleRightLarge = "AlignMiddleRightLarge",
								AlignRight = "AlignRight",
								AlignRightLarge = "AlignRightLarge",
								AlignTopCenter = "AlignTopCenter",
								AlignTopCenterLarge = "AlignTopCenterLarge",
								AlignTopLeft = "AlignTopLeft",
								AlignTopLeftLarge = "AlignTopLeftLarge",
								AlignTopRight = "AlignTopRight",
								AlignTopRightLarge = "AlignTopRightLarge",
								Background = "Background",
								BackgroundLarge = "BackgroundLarge",
								Bold = "Bold",
								BoldLarge = "BoldLarge",
								Bookmark = "Bookmark",
								BookmarkLarge = "BookmarkLarge",
								BorderAllLarge = "BorderAllLarge",
								BorderBottom = "BorderBottom",
								BorderBottomLarge = "BorderBottomLarge",
								BorderDiagonalDown = "BorderDiagonalDown",
								BorderDiagonalDownLarge = "BorderDiagonalDownLarge",
								BorderDiagonalUp = "BorderDiagonalUp",
								BorderDiagonalUpLarge = "BorderDiagonalUpLarge",
								BorderInsideHorizontal = "BorderInsideHorizontal",
								BorderInsideHorizontalLarge = "BorderInsideHorizontalLarge",
								BorderInsideVertical = "BorderInsideVertical",
								BorderInsideVerticalLarge = "BorderInsideVerticalLarge",
								BorderLeft = "BorderLeft",
								BorderLeftLarge = "BorderLeftLarge",
								BorderNone = "BorderNone",
								BorderNoneLarge = "BorderNoneLarge",
								BorderRight = "BorderRight",
								BorderRightLarge = "BorderRightLarge",
								BordersAll = "BordersAll",
								BordersAllLarge = "BordersAllLarge",
								BordersAndShading = "BordersAndShading",
								BordersAndShadingLarge = "BordersAndShadingLarge",
								BordersBoxLarge = "BordersBoxLarge",
								BordersCustomLarge = "BordersCustomLarge",
								BordersGridLarge = "BordersGridLarge",
								BordersInside = "BordersInside",
								BordersInsideLarge = "BordersInsideLarge",
								BordersNoneLarge = "BordersNoneLarge",
								BordersOutside = "BordersOutside",
								BordersOutsideLarge = "BordersOutsideLarge",
								BorderTop = "BorderTop",
								BorderTopLarge = "BorderTopLarge",
								ChangeFontStyle = "ChangeFontStyle",
								ChangeFontStyleLarge = "ChangeFontStyleLarge",
								ChangeTextCase = "ChangeTextCase",
								ChangeTextCaseLarge = "ChangeTextCaseLarge",
								CharacterStyleLarge = "CharacterStyleLarge",
								ClearFormatting = "ClearFormatting",
								ClearFormattingLarge = "ClearFormattingLarge",
								ClearTableStyle = "ClearTableStyle",
								ClearTableStyleLarge = "ClearTableStyleLarge",
								CloseHeaderAndFooter = "CloseHeaderAndFooter",
								CloseHeaderAndFooterLarge = "CloseHeaderAndFooterLarge",
								Columns = "Columns",
								ColumnsLarge = "ColumnsLarge",
								ColumnsLeft = "ColumnsLeft",
								ColumnsLeftLarge = "ColumnsLeftLarge",
								ColumnsOne = "ColumnsOne",
								ColumnsOneLarge = "ColumnsOneLarge",
								ColumnsRight = "ColumnsRight",
								ColumnsRightLarge = "ColumnsRightLarge",
								ColumnsThree = "ColumnsThree",
								ColumnsThreeLarge = "ColumnsThreeLarge",
								ColumnsTwo = "ColumnsTwo",
								ColumnsTwoLarge = "ColumnsTwoLarge",
								Copy = "Copy",
								CopyLarge = "CopyLarge",
								Cut = "Cut",
								CutLarge = "CutLarge",
								Delete = "Delete",
								DeleteComment = "DeleteComment",
								DeleteCommentLarge = "DeleteCommentLarge",
								DeleteLarge = "DeleteLarge",
								DeleteTable = "DeleteTable",
								DeleteTableCells = "DeleteTableCells",
								DeleteTableCellsLarge = "DeleteTableCellsLarge",
								DeleteTableColumns = "DeleteTableColumns",
								DeleteTableColumnsLarge = "DeleteTableColumnsLarge",
								DeleteTableLarge = "DeleteTableLarge",
								DeleteTableRows = "DeleteTableRows",
								DeleteTableRowsLarge = "DeleteTableRowsLarge",
								Delete_Hyperlink = "Delete_Hyperlink",
								Delete_HyperlinkLarge = "Delete_HyperlinkLarge",
								DifferentFirstPage = "DifferentFirstPage",
								DifferentFirstPageLarge = "DifferentFirstPageLarge",
								DifferentOddEvenPages = "DifferentOddEvenPages",
								DifferentOddEvenPagesLarge = "DifferentOddEvenPagesLarge",
								DraftView = "DraftView",
								DraftViewLarge = "DraftViewLarge",
								EditRangePermission = "EditRangePermission",
								EditRangePermissionLarge = "EditRangePermissionLarge",
								Find = "Find",
								FindLarge = "FindLarge",
								First = "First",
								FirstLarge = "FirstLarge",
								FloatingObjectAlignment = "FloatingObjectAlignment",
								FloatingObjectAlignmentLarge = "FloatingObjectAlignmentLarge",
								FloatingObjectBringForward = "FloatingObjectBringForward",
								FloatingObjectBringForwardLarge = "FloatingObjectBringForwardLarge",
								FloatingObjectBringInFrontOfText = "FloatingObjectBringInFrontOfText",
								FloatingObjectBringInFrontOfTextLarge = "FloatingObjectBringInFrontOfTextLarge",
								FloatingObjectBringToFront = "FloatingObjectBringToFront",
								FloatingObjectBringToFrontLarge = "FloatingObjectBringToFrontLarge",
								FloatingObjectBringToFrontOfText = "FloatingObjectBringToFrontOfText",
								FloatingObjectBringToFrontOfTextLarge = "FloatingObjectBringToFrontOfTextLarge",
								FloatingObjectFillColor = "FloatingObjectFillColor",
								FloatingObjectFillColorLarge = "FloatingObjectFillColorLarge",
								FloatingObjectLayoutOptions = "FloatingObjectLayoutOptions",
								FloatingObjectLayoutOptionsLarge = "FloatingObjectLayoutOptionsLarge",
								FloatingObjectOutlineColor = "FloatingObjectOutlineColor",
								FloatingObjectOutlineColorLarge = "FloatingObjectOutlineColorLarge",
								FloatingObjectSendBackward = "FloatingObjectSendBackward",
								FloatingObjectSendBackwardLarge = "FloatingObjectSendBackwardLarge",
								FloatingObjectSendBehindText = "FloatingObjectSendBehindText",
								FloatingObjectSendBehindTextLarge = "FloatingObjectSendBehindTextLarge",
								FloatingObjectSendToBack = "FloatingObjectSendToBack",
								FloatingObjectSendToBackLarge = "FloatingObjectSendToBackLarge",
								FloatingObjectTextWrapType = "FloatingObjectTextWrapType",
								FloatingObjectTextWrapTypeLarge = "FloatingObjectTextWrapTypeLarge",
								Font = "Font",
								FontColor = "FontColor",
								FontColorLarge = "FontColorLarge",
								FontLarge = "FontLarge",
								FontSize = "FontSize",
								FontSizeDecrease = "FontSizeDecrease",
								FontSizeDecreaseLarge = "FontSizeDecreaseLarge",
								FontSizeIncrease = "FontSizeIncrease",
								FontSizeIncreaseLarge = "FontSizeIncreaseLarge",
								FontSizeLarge = "FontSizeLarge",
								Footer = "Footer",
								FooterLarge = "FooterLarge",
								FullScreen = "FullScreen",
								FullScreenLarge = "FullScreenLarge",
								GoToFooter = "GoToFooter",
								GoToFooterLarge = "GoToFooterLarge",
								GoToHeader = "GoToHeader",
								GoToHeaderLarge = "GoToHeaderLarge",
								GoToNextHeaderFooter = "GoToNextHeaderFooter",
								GoToNextHeaderFooterLarge = "GoToNextHeaderFooterLarge",
								GoToPreviousHeaderFooter = "GoToPreviousHeaderFooter",
								GoToPreviousHeaderFooterLarge = "GoToPreviousHeaderFooterLarge",
								Header = "Header",
								HeaderLarge = "HeaderLarge",
								HiddenText = "HiddenText",
								HiddenTextLarge = "HiddenTextLarge",
								Highlight = "Highlight",
								HighlightLarge = "HighlightLarge",
								Hyperlink = "Hyperlink",
								HyperlinkLarge = "HyperlinkLarge",
								ImagePlaceHolder = "ImagePlaceHolder",
								IndentDecrease = "IndentDecrease",
								IndentDecreaseLarge = "IndentDecreaseLarge",
								IndentIncrease = "IndentIncrease",
								IndentIncreaseLarge = "IndentIncreaseLarge",
								InsertCaption = "InsertCaption",
								InsertCaptionLarge = "InsertCaptionLarge",
								InsertColumnBreak = "InsertColumnBreak",
								InsertColumnBreakLarge = "InsertColumnBreakLarge",
								InsertDataField = "InsertDataField",
								InsertDataFieldLarge = "InsertDataFieldLarge",
								InsertEquationCaption = "InsertEquationCaption",
								InsertEquationCaptionLarge = "InsertEquationCaptionLarge",
								InsertFigureCaption = "InsertFigureCaption",
								InsertFigureCaptionLarge = "InsertFigureCaptionLarge",
								InsertFloatingObjectImage = "InsertFloatingObjectImage",
								InsertFloatingObjectImageLarge = "InsertFloatingObjectImageLarge",
								InsertImage = "InsertImage",
								InsertImageLarge = "InsertImageLarge",
								InsertPageBreak = "InsertPageBreak",
								InsertPageBreakLarge = "InsertPageBreakLarge",
								InsertPageCount = "InsertPageCount",
								InsertPageCountLarge = "InsertPageCountLarge",
								InsertPageNumber = "InsertPageNumber",
								InsertPageNumberLarge = "InsertPageNumberLarge",
								InsertSectionBreakContinuous = "InsertSectionBreakContinuous",
								InsertSectionBreakContinuousLarge = "InsertSectionBreakContinuousLarge",
								InsertSectionBreakEvenPage = "InsertSectionBreakEvenPage",
								InsertSectionBreakEvenPageLarge = "InsertSectionBreakEvenPageLarge",
								InsertSectionBreakNextPage = "InsertSectionBreakNextPage",
								InsertSectionBreakNextPageLarge = "InsertSectionBreakNextPageLarge",
								InsertSectionBreakOddPage = "InsertSectionBreakOddPage",
								InsertSectionBreakOddPageLarge = "InsertSectionBreakOddPageLarge",
								InsertTable = "InsertTable",
								InsertTableCaption = "InsertTableCaption",
								InsertTableCaptionLarge = "InsertTableCaptionLarge",
								InsertTableCells = "InsertTableCells",
								InsertTableCellsLarge = "InsertTableCellsLarge",
								InsertTableColumnsToTheLeft = "InsertTableColumnsToTheLeft",
								InsertTableColumnsToTheLeftLarge = "InsertTableColumnsToTheLeftLarge",
								InsertTableColumnsToTheRight = "InsertTableColumnsToTheRight",
								InsertTableColumnsToTheRightLarge = "InsertTableColumnsToTheRightLarge",
								InsertTableLarge = "InsertTableLarge",
								InsertTableOfCaptions = "InsertTableOfCaptions",
								InsertTableOfCaptionsLarge = "InsertTableOfCaptionsLarge",
								InsertTableOfContents = "InsertTableOfContents",
								InsertTableOfContentsLarge = "InsertTableOfContentsLarge",
								InsertTableOfEquations = "InsertTableOfEquations",
								InsertTableOfEquationsLarge = "InsertTableOfEquationsLarge",
								InsertTableOfFigures = "InsertTableOfFigures",
								InsertTableOfFiguresLarge = "InsertTableOfFiguresLarge",
								InsertTableRowsAbove = "InsertTableRowsAbove",
								InsertTableRowsAboveLarge = "InsertTableRowsAboveLarge",
								InsertTableRowsBelow = "InsertTableRowsBelow",
								InsertTableRowsBelowLarge = "InsertTableRowsBelowLarge",
								InsertTextBox = "InsertTextBox",
								InsertTextBoxLarge = "InsertTextBoxLarge",
								Italic = "Italic",
								ItalicLarge = "ItalicLarge",
								Language = "Language",
								LanguageLarge = "LanguageLarge",
								Last = "Last",
								LastLarge = "LastLarge",
								LeftColumns = "LeftColumns",
								LineNumbering = "LineNumbering",
								LineNumberingLarge = "LineNumberingLarge",
								LineSpacing = "LineSpacing",
								LineSpacingLarge = "LineSpacingLarge",
								LinkToPrevious = "LinkToPrevious",
								LinkToPreviousLarge = "LinkToPreviousLarge",
								ListBullets = "ListBullets",
								ListBulletsLarge = "ListBulletsLarge",
								ListMultilevel = "ListMultilevel",
								ListMultilevelLarge = "ListMultilevelLarge",
								ListNumbers = "ListNumbers",
								ListNumbersLarge = "ListNumbersLarge",
								MailMerge = "MailMerge",
								MailMergeLarge = "MailMergeLarge",
								MergeTableCells = "MergeTableCells",
								MergeTableCellsLarge = "MergeTableCellsLarge",
								ModifyStyle = "ModifyStyle",
								ModifyStyleLarge = "ModifyStyleLarge",
								ModifyTableStyle = "ModifyTableStyle",
								ModifyTableStyleLarge = "ModifyTableStyleLarge",
								New = "New",
								NewComment = "NewComment",
								NewCommentLarge = "NewCommentLarge",
								NewLarge = "NewLarge",
								NewTableStyle = "NewTableStyle",
								NewTableStyleLarge = "NewTableStyleLarge",
								Next = "Next",
								NextComment = "NextComment",
								NextCommentLarge = "NextCommentLarge",
								NextLarge = "NextLarge",
								OneColumn = "OneColumn",
								Open = "Open",
								OpenLarge = "OpenLarge",
								PageColor = "PageColor",
								PageColorLarge = "PageColorLarge",
								PageMargins = "PageMargins",
								PageMarginsLarge = "PageMarginsLarge",
								PageMarginsModerate = "PageMarginsModerate",
								PageMarginsModerateLarge = "PageMarginsModerateLarge",
								PageMarginsNarrow = "PageMarginsNarrow",
								PageMarginsNarrowLarge = "PageMarginsNarrowLarge",
								PageMarginsNormal = "PageMarginsNormal",
								PageMarginsNormalLarge = "PageMarginsNormalLarge",
								PageMarginsWide = "PageMarginsWide",
								PageMarginsWideLarge = "PageMarginsWideLarge",
								PageOrientationLandscape = "PageOrientationLandscape",
								PageOrientationLandscapeLarge = "PageOrientationLandscapeLarge",
								PageOrientation = "PageOrientation",
								PageOrientationLarge = "PageOrientationLarge",
								PageOrientationPortrait = "PageOrientationPortrait",
								PageOrientationPortraitLarge = "PageOrientationPortraitLarge",
								PaperSize = "PaperSize",
								PaperSizeLarge = "PaperSizeLarge",
								Paragraph = "Paragraph",
								ParagraphLarge = "ParagraphLarge",
								ParagraphStyleLarge = "ParagraphStyleLarge",
								Paste = "Paste",
								PasteLarge = "PasteLarge",
								PasteSpecial = "PasteSpecial",
								PasteSpecialLarge = "PasteSpecialLarge",
								PenColor = "PenColor",
								PenColorLarge = "PenColorLarge",
								Prev = "Prev",
								Preview = "Preview",
								PreviewLarge = "PreviewLarge",
								PreviousComment = "PreviousComment",
								PreviousCommentLarge = "PreviousCommentLarge",
								PrevLarge = "PrevLarge",
								Print = "Print",
								PrintBrowser = "PrintBrowser",
								PrintBrowserLarge = "PrintBrowserLarge",
								PrintDialog = "PrintDialog",
								PrintDialogLarge = "PrintDialogLarge",
								PrintLarge = "PrintLarge",
								PrintLayoutView = "PrintLayoutView",
								PrintLayoutViewLarge = "PrintLayoutViewLarge",
								PrintPreviewBrowser = "PrintPreviewBrowser",
								PrintPreviewBrowserLarge = "PrintPreviewBrowserLarge",
								ProtectDocument = "ProtectDocument",
								ProtectDocumentLarge = "ProtectDocumentLarge",
								ReadingLayoutView = "ReadingLayoutView",
								ReadingLayoutViewLarge = "ReadingLayoutViewLarge",
								Redo = "Redo",
								RedoLarge = "RedoLarge",
								Replace = "Replace",
								ReplaceLarge = "ReplaceLarge",
								Reviewers = "Reviewers",
								ReviewersLarge = "ReviewersLarge",
								ReviewingPane = "ReviewingPane",
								ReviewingPaneLarge = "ReviewingPaneLarge",
								RightColumns = "RightColumns",
								RulerHorizontal = "RulerHorizontal",
								RulerHorizontalLarge = "RulerHorizontalLarge",
								RulerVertical = "RulerVertical",
								RulerVerticalLarge = "RulerVerticalLarge",
								Save = "Save",
								SaveAll = "SaveAll",
								SaveAllLarge = "SaveAllLarge",
								SaveAs = "SaveAs",
								SaveAsLarge = "SaveAsLarge",
								SaveLarge = "SaveLarge",
								Select = "Select",
								SelectAll = "SelectAll",
								SelectAllLarge = "SelectAllLarge",
								SelectLarge = "SelectLarge",
								SelectTable = "SelectTable",
								SelectTableCell = "SelectTableCell",
								SelectTableCellLarge = "SelectTableCellLarge",
								SelectTableColumn = "SelectTableColumn",
								SelectTableColumnLarge = "SelectTableColumnLarge",
								SelectTableLarge = "SelectTableLarge",
								SelectTableRow = "SelectTableRow",
								SelectTableRowLarge = "SelectTableRowLarge",
								Shading = "Shading",
								ShadingLarge = "ShadingLarge",
								ShowAllFieldCodes = "ShowAllFieldCodes",
								ShowAllFieldCodesLarge = "ShowAllFieldCodesLarge",
								ShowAllFieldResults = "ShowAllFieldResults",
								ShowAllFieldResultsLarge = "ShowAllFieldResultsLarge",
								ShowComments = "ShowComments",
								ShowCommentsLarge = "ShowCommentsLarge",
								ShowHidden = "ShowHidden",
								ShowHiddenLarge = "ShowHiddenLarge",
								SimpleView = "SimpleView",
								SimpleViewLarge = "SimpleViewLarge",
								SpacingDecrease = "SpacingDecrease",
								SpacingDecreaseLarge = "SpacingDecreaseLarge",
								SpellCheck = "SpellCheck",
								SpellCheckAsYouType = "SpellCheckAsYouType",
								SpellCheckAsYouTypeLarge = "SpellCheckAsYouTypeLarge",
								SpellCheckLarge = "SpellCheckLarge",
								SplitTable = "SplitTable",
								SplitTableCells = "SplitTableCells",
								SplitTableCellsLarge = "SplitTableCellsLarge",
								SplitTableLarge = "SplitTableLarge",
								Strikeout = "Strikeout",
								StrikeoutDouble = "StrikeoutDouble",
								StrikeoutDoubleLarge = "StrikeoutDoubleLarge",
								StrikeoutLarge = "StrikeoutLarge",
								Subscript = "Subscript",
								SubscriptLarge = "SubscriptLarge",
								Superscript = "Superscript",
								SuperscriptLarge = "SuperscriptLarge",
								Symbol = "Symbol",
								SymbolLarge = "SymbolLarge",
								TableAutoFitContents = "TableAutoFitContents",
								TableAutoFitContentsLarge = "TableAutoFitContentsLarge",
								TableAutoFitWindow = "TableAutoFitWindow",
								TableAutoFitWindowLarge = "TableAutoFitWindowLarge",
								TableCellMargins = "TableCellMargins",
								TableCellMarginsLarge = "TableCellMarginsLarge",
								TableFixedColumnWidth = "TableFixedColumnWidth",
								TableFixedColumnWidthLarge = "TableFixedColumnWidthLarge",
								TableProperties = "TableProperties",
								TablePropertiesLarge = "TablePropertiesLarge",
								TableStyleLarge = "TableStyleLarge",
								TextWrapBehind = "TextWrapBehind",
								TextWrapBehindLarge = "TextWrapBehindLarge",
								TextWrapInFrontOfText = "TextWrapInFrontOfText",
								TextWrapInFrontOfTextLarge = "TextWrapInFrontOfTextLarge",
								TextWrapSquare = "TextWrapSquare",
								TextWrapSquareLarge = "TextWrapSquareLarge",
								TextWrapThrough = "TextWrapThrough",
								TextWrapThroughLarge = "TextWrapThroughLarge",
								TextWrapTight = "TextWrapTight",
								TextWrapTightLarge = "TextWrapTightLarge",
								TextWrapTopAndBottom = "TextWrapTopAndBottom",
								TextWrapTopAndBottomLarge = "TextWrapTopAndBottomLarge",
								ThreeColumns = "ThreeColumns",
								ToggleFieldCodes = "ToggleFieldCodes",
								ToggleFieldCodesLarge = "ToggleFieldCodesLarge",
								TwoColumns = "TwoColumns",
								Underline = "Underline",
								UnderlineDouble = "UnderlineDouble",
								UnderlineDoubleLarge = "UnderlineDoubleLarge",
								UnderlineLarge = "UnderlineLarge",
								UnderlineWord = "UnderlineWord",
								UnderlineWordLarge = "UnderlineWordLarge",
								Undo = "Undo",
								UndoLarge = "UndoLarge",
								UnprotectDocument = "UnprotectDocument",
								UnprotectDocumentLarge = "UnprotectDocumentLarge",
								UpdateField = "UpdateField",
								UpdateFieldLarge = "UpdateFieldLarge",
								UpdateTableOfContents = "UpdateTableOfContents",
								UpdateTableOfContentsLarge = "UpdateTableOfContentsLarge",
								ViewMergedData = "ViewMergedData",
								ViewMergedDataLarge = "ViewMergedDataLarge",
								ViewTableGridlines = "ViewTableGridlines",
								ViewTableGridlinesLarge = "ViewTableGridlinesLarge",
								ZoomIn = "ZoomIn",
								ZoomInLarge = "ZoomInLarge",
								ZoomOut = "ZoomOut",
								ZoomOutLarge = "ZoomOutLarge";
		#endregion
		internal readonly static Dictionary<MenuIconSetType, string> CategoryDictionary = new Dictionary<MenuIconSetType, string>();
		internal readonly static Dictionary<MenuIconSetType, string> SpriteNameDictionary = new Dictionary<MenuIconSetType, string>();
		static RichEditRibbonImages() {
			CategoryDictionary.Add(MenuIconSetType.NotSet, "reRibbonIcons");
			CategoryDictionary.Add(MenuIconSetType.Colored, "reRibbonIcons");
			CategoryDictionary.Add(MenuIconSetType.ColoredLight, "reRibbonIcons");
			CategoryDictionary.Add(MenuIconSetType.GrayScaled, "reRibbonGIcons");
			CategoryDictionary.Add(MenuIconSetType.GrayScaledWithWhiteHottrack, "reRibbonGWIcons");
			SpriteNameDictionary.Add(MenuIconSetType.NotSet, RibbonRESpriteName);
			SpriteNameDictionary.Add(MenuIconSetType.Colored, RibbonRESpriteName);
			SpriteNameDictionary.Add(MenuIconSetType.ColoredLight, RibbonRESpriteName);
			SpriteNameDictionary.Add(MenuIconSetType.GrayScaled, RibbonREGSpriteName);
			SpriteNameDictionary.Add(MenuIconSetType.GrayScaledWithWhiteHottrack, RibbonREGWSpriteName);
		}
		public RichEditRibbonImages(ISkinOwner properties, MenuIconSetType iconSetStyle)
			: base(properties, iconSetStyle) {
		}
		protected override Dictionary<MenuIconSetType, string> Categories { get { return CategoryDictionary; } }
		protected override Dictionary<MenuIconSetType, string> SpriteNames { get { return SpriteNameDictionary; } }
		protected override void PopulateImageInfoList(List<ImageInfo> list) {
			base.PopulateImageInfoList(list);
			ImageFlags imageFlags = ImageFlags.IsPng;
			#region InitializeImages
			list.Add(new ImageInfo(AddParagraphToTableOfContents, imageFlags, DefaultSmallSize, AddParagraphToTableOfContents));
			list.Add(new ImageInfo(AddParagraphToTableOfContentsLarge, imageFlags, DefaultLargeSize, AddParagraphToTableOfContentsLarge));
			list.Add(new ImageInfo(AlignBottomCenter, imageFlags, DefaultSmallSize, AlignBottomCenter));
			list.Add(new ImageInfo(AlignBottomCenterLarge, imageFlags, DefaultLargeSize, AlignBottomCenterLarge));
			list.Add(new ImageInfo(AlignBottomLeft, imageFlags, DefaultSmallSize, AlignBottomLeft));
			list.Add(new ImageInfo(AlignBottomLeftLarge, imageFlags, DefaultLargeSize, AlignBottomLeftLarge));
			list.Add(new ImageInfo(AlignBottomRight, imageFlags, DefaultSmallSize, AlignBottomRight));
			list.Add(new ImageInfo(AlignBottomRightLarge, imageFlags, DefaultLargeSize, AlignBottomRightLarge));
			list.Add(new ImageInfo(AlignCenter, imageFlags, DefaultSmallSize, AlignCenter));
			list.Add(new ImageInfo(AlignCenterLarge, imageFlags, DefaultLargeSize, AlignCenterLarge));
			list.Add(new ImageInfo(AlignFloatingObjectBottomCenter, imageFlags, DefaultSmallSize, AlignFloatingObjectBottomCenter));
			list.Add(new ImageInfo(AlignFloatingObjectBottomCenterLarge, imageFlags, DefaultLargeSize, AlignFloatingObjectBottomCenterLarge));
			list.Add(new ImageInfo(AlignFloatingObjectBottomLeft, imageFlags, DefaultSmallSize, AlignFloatingObjectBottomLeft));
			list.Add(new ImageInfo(AlignFloatingObjectBottomLeftLarge, imageFlags, DefaultLargeSize, AlignFloatingObjectBottomLeftLarge));
			list.Add(new ImageInfo(AlignFloatingObjectBottomRight, imageFlags, DefaultSmallSize, AlignFloatingObjectBottomRight));
			list.Add(new ImageInfo(AlignFloatingObjectBottomRightLarge, imageFlags, DefaultLargeSize, AlignFloatingObjectBottomRightLarge));
			list.Add(new ImageInfo(AlignFloatingObjectMiddleCenter, imageFlags, DefaultSmallSize, AlignFloatingObjectMiddleCenter));
			list.Add(new ImageInfo(AlignFloatingObjectMiddleCenterLarge, imageFlags, DefaultLargeSize, AlignFloatingObjectMiddleCenterLarge));
			list.Add(new ImageInfo(AlignFloatingObjectMiddleLeft, imageFlags, DefaultSmallSize, AlignFloatingObjectMiddleLeft));
			list.Add(new ImageInfo(AlignFloatingObjectMiddleLeftLarge, imageFlags, DefaultLargeSize, AlignFloatingObjectMiddleLeftLarge));
			list.Add(new ImageInfo(AlignFloatingObjectMiddleRight, imageFlags, DefaultSmallSize, AlignFloatingObjectMiddleRight));
			list.Add(new ImageInfo(AlignFloatingObjectMiddleRightLarge, imageFlags, DefaultLargeSize, AlignFloatingObjectMiddleRightLarge));
			list.Add(new ImageInfo(AlignFloatingObjectTopCenter, imageFlags, DefaultSmallSize, AlignFloatingObjectTopCenter));
			list.Add(new ImageInfo(AlignFloatingObjectTopCenterLarge, imageFlags, DefaultLargeSize, AlignFloatingObjectTopCenterLarge));
			list.Add(new ImageInfo(AlignFloatingObjectTopLeft, imageFlags, DefaultSmallSize, AlignFloatingObjectTopLeft));
			list.Add(new ImageInfo(AlignFloatingObjectTopLeftLarge, imageFlags, DefaultLargeSize, AlignFloatingObjectTopLeftLarge));
			list.Add(new ImageInfo(AlignFloatingObjectTopRight, imageFlags, DefaultSmallSize, AlignFloatingObjectTopRight));
			list.Add(new ImageInfo(AlignFloatingObjectTopRightLarge, imageFlags, DefaultLargeSize, AlignFloatingObjectTopRightLarge));
			list.Add(new ImageInfo(AlignJustify, imageFlags, DefaultSmallSize, AlignJustify));
			list.Add(new ImageInfo(AlignJustifyLarge, imageFlags, DefaultLargeSize, AlignJustifyLarge));
			list.Add(new ImageInfo(AlignLeft, imageFlags, DefaultSmallSize, AlignLeft));
			list.Add(new ImageInfo(AlignLeftLarge, imageFlags, DefaultLargeSize, AlignLeftLarge));
			list.Add(new ImageInfo(AlignMiddleCenter, imageFlags, DefaultSmallSize, AlignMiddleCenter));
			list.Add(new ImageInfo(AlignMiddleCenterLarge, imageFlags, DefaultLargeSize, AlignMiddleCenterLarge));
			list.Add(new ImageInfo(AlignMiddleLeft, imageFlags, DefaultSmallSize, AlignMiddleLeft));
			list.Add(new ImageInfo(AlignMiddleLeftLarge, imageFlags, DefaultLargeSize, AlignMiddleLeftLarge));
			list.Add(new ImageInfo(AlignMiddleRight, imageFlags, DefaultSmallSize, AlignMiddleRight));
			list.Add(new ImageInfo(AlignMiddleRightLarge, imageFlags, DefaultLargeSize, AlignMiddleRightLarge));
			list.Add(new ImageInfo(AlignRight, imageFlags, DefaultSmallSize, AlignRight));
			list.Add(new ImageInfo(AlignRightLarge, imageFlags, DefaultLargeSize, AlignRightLarge));
			list.Add(new ImageInfo(AlignTopCenter, imageFlags, DefaultSmallSize, AlignTopCenter));
			list.Add(new ImageInfo(AlignTopCenterLarge, imageFlags, DefaultLargeSize, AlignTopCenterLarge));
			list.Add(new ImageInfo(AlignTopLeft, imageFlags, DefaultSmallSize, AlignTopLeft));
			list.Add(new ImageInfo(AlignTopLeftLarge, imageFlags, DefaultLargeSize, AlignTopLeftLarge));
			list.Add(new ImageInfo(AlignTopRight, imageFlags, DefaultSmallSize, AlignTopRight));
			list.Add(new ImageInfo(AlignTopRightLarge, imageFlags, DefaultLargeSize, AlignTopRightLarge));
			list.Add(new ImageInfo(Background, imageFlags, DefaultSmallSize, Background));
			list.Add(new ImageInfo(BackgroundLarge, imageFlags, DefaultLargeSize, BackgroundLarge));
			list.Add(new ImageInfo(Bold, imageFlags, DefaultSmallSize, Bold));
			list.Add(new ImageInfo(BoldLarge, imageFlags, DefaultLargeSize, BoldLarge));
			list.Add(new ImageInfo(Bookmark, imageFlags, DefaultSmallSize, Bookmark));
			list.Add(new ImageInfo(BookmarkLarge, imageFlags, DefaultLargeSize, BookmarkLarge));
			list.Add(new ImageInfo(BorderAllLarge, imageFlags, DefaultLargeSize, BorderAllLarge));
			list.Add(new ImageInfo(BorderBottom, imageFlags, DefaultSmallSize, BorderBottom));
			list.Add(new ImageInfo(BorderBottomLarge, imageFlags, DefaultLargeSize, BorderBottomLarge));
			list.Add(new ImageInfo(BorderDiagonalDown, imageFlags, DefaultSmallSize, BorderDiagonalDown));
			list.Add(new ImageInfo(BorderDiagonalDownLarge, imageFlags, DefaultLargeSize, BorderDiagonalDownLarge));
			list.Add(new ImageInfo(BorderDiagonalUp, imageFlags, DefaultSmallSize, BorderDiagonalUp));
			list.Add(new ImageInfo(BorderDiagonalUpLarge, imageFlags, DefaultLargeSize, BorderDiagonalUpLarge));
			list.Add(new ImageInfo(BorderInsideHorizontal, imageFlags, DefaultSmallSize, BorderInsideHorizontal));
			list.Add(new ImageInfo(BorderInsideHorizontalLarge, imageFlags, DefaultLargeSize, BorderInsideHorizontalLarge));
			list.Add(new ImageInfo(BorderInsideVertical, imageFlags, DefaultSmallSize, BorderInsideVertical));
			list.Add(new ImageInfo(BorderInsideVerticalLarge, imageFlags, DefaultLargeSize, BorderInsideVerticalLarge));
			list.Add(new ImageInfo(BorderLeft, imageFlags, DefaultSmallSize, BorderLeft));
			list.Add(new ImageInfo(BorderLeftLarge, imageFlags, DefaultLargeSize, BorderLeftLarge));
			list.Add(new ImageInfo(BorderNone, imageFlags, DefaultSmallSize, BorderNone));
			list.Add(new ImageInfo(BorderNoneLarge, imageFlags, DefaultLargeSize, BorderNoneLarge));
			list.Add(new ImageInfo(BorderRight, imageFlags, DefaultSmallSize, BorderRight));
			list.Add(new ImageInfo(BorderRightLarge, imageFlags, DefaultLargeSize, BorderRightLarge));
			list.Add(new ImageInfo(BordersAll, imageFlags, DefaultSmallSize, BordersAll));
			list.Add(new ImageInfo(BordersAllLarge, imageFlags, DefaultLargeSize, BordersAllLarge));
			list.Add(new ImageInfo(BordersAndShading, imageFlags, DefaultSmallSize, BordersAndShading));
			list.Add(new ImageInfo(BordersAndShadingLarge, imageFlags, DefaultLargeSize, BordersAndShadingLarge));
			list.Add(new ImageInfo(BordersBoxLarge, imageFlags, DefaultLargeSize, BordersBoxLarge));
			list.Add(new ImageInfo(BordersCustomLarge, imageFlags, DefaultLargeSize, BordersCustomLarge));
			list.Add(new ImageInfo(BordersGridLarge, imageFlags, DefaultLargeSize, BordersGridLarge));
			list.Add(new ImageInfo(BordersInside, imageFlags, DefaultSmallSize, BordersInside));
			list.Add(new ImageInfo(BordersInsideLarge, imageFlags, DefaultLargeSize, BordersInsideLarge));
			list.Add(new ImageInfo(BordersNoneLarge, imageFlags, DefaultLargeSize, BordersNoneLarge));
			list.Add(new ImageInfo(BordersOutside, imageFlags, DefaultSmallSize, BordersOutside));
			list.Add(new ImageInfo(BordersOutsideLarge, imageFlags, DefaultLargeSize, BordersOutsideLarge));
			list.Add(new ImageInfo(BorderTop, imageFlags, DefaultSmallSize, BorderTop));
			list.Add(new ImageInfo(BorderTopLarge, imageFlags, DefaultLargeSize, BorderTopLarge));
			list.Add(new ImageInfo(ChangeFontStyle, imageFlags, DefaultSmallSize, ChangeFontStyle));
			list.Add(new ImageInfo(ChangeFontStyleLarge, imageFlags, DefaultLargeSize, ChangeFontStyleLarge));
			list.Add(new ImageInfo(ChangeTextCase, imageFlags, DefaultSmallSize, ChangeTextCase));
			list.Add(new ImageInfo(ChangeTextCaseLarge, imageFlags, DefaultLargeSize, ChangeTextCaseLarge));
			list.Add(new ImageInfo(CharacterStyleLarge, imageFlags, DefaultLargeSize, CharacterStyleLarge));
			list.Add(new ImageInfo(ClearFormatting, imageFlags, DefaultSmallSize, ClearFormatting));
			list.Add(new ImageInfo(ClearFormattingLarge, imageFlags, DefaultLargeSize, ClearFormattingLarge));
			list.Add(new ImageInfo(ClearTableStyle, imageFlags, DefaultSmallSize, ClearTableStyle));
			list.Add(new ImageInfo(ClearTableStyleLarge, imageFlags, DefaultLargeSize, ClearTableStyleLarge));
			list.Add(new ImageInfo(CloseHeaderAndFooter, imageFlags, DefaultSmallSize, CloseHeaderAndFooter));
			list.Add(new ImageInfo(CloseHeaderAndFooterLarge, imageFlags, DefaultLargeSize, CloseHeaderAndFooterLarge));
			list.Add(new ImageInfo(Columns, imageFlags, DefaultSmallSize, Columns));
			list.Add(new ImageInfo(ColumnsLarge, imageFlags, DefaultLargeSize, ColumnsLarge));
			list.Add(new ImageInfo(ColumnsLeft, imageFlags, DefaultSmallSize, ColumnsLeft));
			list.Add(new ImageInfo(ColumnsLeftLarge, imageFlags, DefaultLargeSize, ColumnsLeftLarge));
			list.Add(new ImageInfo(ColumnsOne, imageFlags, DefaultSmallSize, ColumnsOne));
			list.Add(new ImageInfo(ColumnsOneLarge, imageFlags, DefaultLargeSize, ColumnsOneLarge));
			list.Add(new ImageInfo(ColumnsRight, imageFlags, DefaultSmallSize, ColumnsRight));
			list.Add(new ImageInfo(ColumnsRightLarge, imageFlags, DefaultLargeSize, ColumnsRightLarge));
			list.Add(new ImageInfo(ColumnsThree, imageFlags, DefaultSmallSize, ColumnsThree));
			list.Add(new ImageInfo(ColumnsThreeLarge, imageFlags, DefaultLargeSize, ColumnsThreeLarge));
			list.Add(new ImageInfo(ColumnsTwo, imageFlags, DefaultSmallSize, ColumnsTwo));
			list.Add(new ImageInfo(ColumnsTwoLarge, imageFlags, DefaultLargeSize, ColumnsTwoLarge));
			list.Add(new ImageInfo(Copy, imageFlags, DefaultSmallSize, Copy));
			list.Add(new ImageInfo(CopyLarge, imageFlags, DefaultLargeSize, CopyLarge));
			list.Add(new ImageInfo(Cut, imageFlags, DefaultSmallSize, Cut));
			list.Add(new ImageInfo(CutLarge, imageFlags, DefaultLargeSize, CutLarge));
			list.Add(new ImageInfo(Delete, imageFlags, DefaultSmallSize, Delete));
			list.Add(new ImageInfo(DeleteComment, imageFlags, DefaultSmallSize, DeleteComment));
			list.Add(new ImageInfo(DeleteCommentLarge, imageFlags, DefaultLargeSize, DeleteCommentLarge));
			list.Add(new ImageInfo(DeleteLarge, imageFlags, DefaultLargeSize, DeleteLarge));
			list.Add(new ImageInfo(DeleteTable, imageFlags, DefaultSmallSize, DeleteTable));
			list.Add(new ImageInfo(DeleteTableCells, imageFlags, DefaultSmallSize, DeleteTableCells));
			list.Add(new ImageInfo(DeleteTableCellsLarge, imageFlags, DefaultLargeSize, DeleteTableCellsLarge));
			list.Add(new ImageInfo(DeleteTableColumns, imageFlags, DefaultSmallSize, DeleteTableColumns));
			list.Add(new ImageInfo(DeleteTableColumnsLarge, imageFlags, DefaultLargeSize, DeleteTableColumnsLarge));
			list.Add(new ImageInfo(DeleteTableLarge, imageFlags, DefaultLargeSize, DeleteTableLarge));
			list.Add(new ImageInfo(DeleteTableRows, imageFlags, DefaultSmallSize, DeleteTableRows));
			list.Add(new ImageInfo(DeleteTableRowsLarge, imageFlags, DefaultLargeSize, DeleteTableRowsLarge));
			list.Add(new ImageInfo(Delete_Hyperlink, imageFlags, DefaultSmallSize, Delete_Hyperlink));
			list.Add(new ImageInfo(Delete_HyperlinkLarge, imageFlags, DefaultLargeSize, Delete_HyperlinkLarge));
			list.Add(new ImageInfo(DifferentFirstPage, imageFlags, DefaultSmallSize, DifferentFirstPage));
			list.Add(new ImageInfo(DifferentFirstPageLarge, imageFlags, DefaultLargeSize, DifferentFirstPageLarge));
			list.Add(new ImageInfo(DifferentOddEvenPages, imageFlags, DefaultSmallSize, DifferentOddEvenPages));
			list.Add(new ImageInfo(DifferentOddEvenPagesLarge, imageFlags, DefaultLargeSize, DifferentOddEvenPagesLarge));
			list.Add(new ImageInfo(DraftView, imageFlags, DefaultSmallSize, DraftView));
			list.Add(new ImageInfo(DraftViewLarge, imageFlags, DefaultLargeSize, DraftViewLarge));
			list.Add(new ImageInfo(EditRangePermission, imageFlags, DefaultSmallSize, EditRangePermission));
			list.Add(new ImageInfo(EditRangePermissionLarge, imageFlags, DefaultLargeSize, EditRangePermissionLarge));
			list.Add(new ImageInfo(Find, imageFlags, DefaultSmallSize, Find));
			list.Add(new ImageInfo(FindLarge, imageFlags, DefaultLargeSize, FindLarge));
			list.Add(new ImageInfo(First, imageFlags, DefaultSmallSize, First));
			list.Add(new ImageInfo(FirstLarge, imageFlags, DefaultLargeSize, FirstLarge));
			list.Add(new ImageInfo(FloatingObjectAlignment, imageFlags, DefaultSmallSize, FloatingObjectAlignment));
			list.Add(new ImageInfo(FloatingObjectAlignmentLarge, imageFlags, DefaultLargeSize, FloatingObjectAlignmentLarge));
			list.Add(new ImageInfo(FloatingObjectBringForward, imageFlags, DefaultSmallSize, FloatingObjectBringForward));
			list.Add(new ImageInfo(FloatingObjectBringForwardLarge, imageFlags, DefaultLargeSize, FloatingObjectBringForwardLarge));
			list.Add(new ImageInfo(FloatingObjectBringInFrontOfText, imageFlags, DefaultSmallSize, FloatingObjectBringInFrontOfText));
			list.Add(new ImageInfo(FloatingObjectBringInFrontOfTextLarge, imageFlags, DefaultLargeSize, FloatingObjectBringInFrontOfTextLarge));
			list.Add(new ImageInfo(FloatingObjectBringToFront, imageFlags, DefaultSmallSize, FloatingObjectBringToFront));
			list.Add(new ImageInfo(FloatingObjectBringToFrontLarge, imageFlags, DefaultLargeSize, FloatingObjectBringToFrontLarge));
			list.Add(new ImageInfo(FloatingObjectBringToFrontOfText, imageFlags, DefaultSmallSize, FloatingObjectBringToFrontOfText));
			list.Add(new ImageInfo(FloatingObjectBringToFrontOfTextLarge, imageFlags, DefaultLargeSize, FloatingObjectBringToFrontOfTextLarge));
			list.Add(new ImageInfo(FloatingObjectFillColor, imageFlags, DefaultSmallSize, FloatingObjectFillColor));
			list.Add(new ImageInfo(FloatingObjectFillColorLarge, imageFlags, DefaultLargeSize, FloatingObjectFillColorLarge));
			list.Add(new ImageInfo(FloatingObjectLayoutOptions, imageFlags, DefaultSmallSize, FloatingObjectLayoutOptions));
			list.Add(new ImageInfo(FloatingObjectLayoutOptionsLarge, imageFlags, DefaultLargeSize, FloatingObjectLayoutOptionsLarge));
			list.Add(new ImageInfo(FloatingObjectOutlineColor, imageFlags, DefaultSmallSize, FloatingObjectOutlineColor));
			list.Add(new ImageInfo(FloatingObjectOutlineColorLarge, imageFlags, DefaultLargeSize, FloatingObjectOutlineColorLarge));
			list.Add(new ImageInfo(FloatingObjectSendBackward, imageFlags, DefaultSmallSize, FloatingObjectSendBackward));
			list.Add(new ImageInfo(FloatingObjectSendBackwardLarge, imageFlags, DefaultLargeSize, FloatingObjectSendBackwardLarge));
			list.Add(new ImageInfo(FloatingObjectSendBehindText, imageFlags, DefaultSmallSize, FloatingObjectSendBehindText));
			list.Add(new ImageInfo(FloatingObjectSendBehindTextLarge, imageFlags, DefaultLargeSize, FloatingObjectSendBehindTextLarge));
			list.Add(new ImageInfo(FloatingObjectSendToBack, imageFlags, DefaultSmallSize, FloatingObjectSendToBack));
			list.Add(new ImageInfo(FloatingObjectSendToBackLarge, imageFlags, DefaultLargeSize, FloatingObjectSendToBackLarge));
			list.Add(new ImageInfo(FloatingObjectTextWrapType, imageFlags, DefaultSmallSize, FloatingObjectTextWrapType));
			list.Add(new ImageInfo(FloatingObjectTextWrapTypeLarge, imageFlags, DefaultLargeSize, FloatingObjectTextWrapTypeLarge));
			list.Add(new ImageInfo(Font, imageFlags, DefaultSmallSize, Font));
			list.Add(new ImageInfo(FontColor, imageFlags, DefaultSmallSize, FontColor));
			list.Add(new ImageInfo(FontColorLarge, imageFlags, DefaultLargeSize, FontColorLarge));
			list.Add(new ImageInfo(FontLarge, imageFlags, DefaultLargeSize, FontLarge));
			list.Add(new ImageInfo(FontSize, imageFlags, DefaultSmallSize, FontSize));
			list.Add(new ImageInfo(FontSizeDecrease, imageFlags, DefaultSmallSize, FontSizeDecrease));
			list.Add(new ImageInfo(FontSizeDecreaseLarge, imageFlags, DefaultLargeSize, FontSizeDecreaseLarge));
			list.Add(new ImageInfo(FontSizeIncrease, imageFlags, DefaultSmallSize, FontSizeIncrease));
			list.Add(new ImageInfo(FontSizeIncreaseLarge, imageFlags, DefaultLargeSize, FontSizeIncreaseLarge));
			list.Add(new ImageInfo(FontSizeLarge, imageFlags, DefaultLargeSize, FontSizeLarge));
			list.Add(new ImageInfo(Footer, imageFlags, DefaultSmallSize, Footer));
			list.Add(new ImageInfo(FooterLarge, imageFlags, DefaultLargeSize, FooterLarge));
			list.Add(new ImageInfo(FullScreen, imageFlags, DefaultLargeSize, FullScreen));
			list.Add(new ImageInfo(FullScreenLarge, imageFlags, DefaultLargeSize, FullScreenLarge));
			list.Add(new ImageInfo(GoToFooter, imageFlags, DefaultSmallSize, GoToFooter));
			list.Add(new ImageInfo(GoToFooterLarge, imageFlags, DefaultLargeSize, GoToFooterLarge));
			list.Add(new ImageInfo(GoToHeader, imageFlags, DefaultSmallSize, GoToHeader));
			list.Add(new ImageInfo(GoToHeaderLarge, imageFlags, DefaultLargeSize, GoToHeaderLarge));
			list.Add(new ImageInfo(GoToNextHeaderFooter, imageFlags, DefaultSmallSize, GoToNextHeaderFooter));
			list.Add(new ImageInfo(GoToNextHeaderFooterLarge, imageFlags, DefaultLargeSize, GoToNextHeaderFooterLarge));
			list.Add(new ImageInfo(GoToPreviousHeaderFooter, imageFlags, DefaultSmallSize, GoToPreviousHeaderFooter));
			list.Add(new ImageInfo(GoToPreviousHeaderFooterLarge, imageFlags, DefaultLargeSize, GoToPreviousHeaderFooterLarge));
			list.Add(new ImageInfo(Header, imageFlags, DefaultSmallSize, Header));
			list.Add(new ImageInfo(HeaderLarge, imageFlags, DefaultLargeSize, HeaderLarge));
			list.Add(new ImageInfo(HiddenText, imageFlags, DefaultSmallSize, HiddenText));
			list.Add(new ImageInfo(HiddenTextLarge, imageFlags, DefaultLargeSize, HiddenTextLarge));
			list.Add(new ImageInfo(Highlight, imageFlags, DefaultSmallSize, Highlight));
			list.Add(new ImageInfo(HighlightLarge, imageFlags, DefaultLargeSize, HighlightLarge));
			list.Add(new ImageInfo(Hyperlink, imageFlags, DefaultSmallSize, Hyperlink));
			list.Add(new ImageInfo(HyperlinkLarge, imageFlags, DefaultLargeSize, HyperlinkLarge));
			list.Add(new ImageInfo(ImagePlaceHolder, imageFlags, DefaultSmallSize, ImagePlaceHolder));
			list.Add(new ImageInfo(IndentDecrease, imageFlags, DefaultSmallSize, IndentDecrease));
			list.Add(new ImageInfo(IndentDecreaseLarge, imageFlags, DefaultLargeSize, IndentDecreaseLarge));
			list.Add(new ImageInfo(IndentIncrease, imageFlags, DefaultSmallSize, IndentIncrease));
			list.Add(new ImageInfo(IndentIncreaseLarge, imageFlags, DefaultLargeSize, IndentIncreaseLarge));
			list.Add(new ImageInfo(InsertCaption, imageFlags, DefaultSmallSize, InsertCaption));
			list.Add(new ImageInfo(InsertCaptionLarge, imageFlags, DefaultLargeSize, InsertCaptionLarge));
			list.Add(new ImageInfo(InsertColumnBreak, imageFlags, DefaultSmallSize, InsertColumnBreak));
			list.Add(new ImageInfo(InsertColumnBreakLarge, imageFlags, DefaultLargeSize, InsertColumnBreakLarge));
			list.Add(new ImageInfo(InsertDataField, imageFlags, DefaultSmallSize, InsertDataField));
			list.Add(new ImageInfo(InsertDataFieldLarge, imageFlags, DefaultLargeSize, InsertDataFieldLarge));
			list.Add(new ImageInfo(InsertEquationCaption, imageFlags, DefaultSmallSize, InsertEquationCaption));
			list.Add(new ImageInfo(InsertEquationCaptionLarge, imageFlags, DefaultLargeSize, InsertEquationCaptionLarge));
			list.Add(new ImageInfo(InsertFigureCaption, imageFlags, DefaultSmallSize, InsertFigureCaption));
			list.Add(new ImageInfo(InsertFigureCaptionLarge, imageFlags, DefaultLargeSize, InsertFigureCaptionLarge));
			list.Add(new ImageInfo(InsertFloatingObjectImage, imageFlags, DefaultSmallSize, InsertFloatingObjectImage));
			list.Add(new ImageInfo(InsertFloatingObjectImageLarge, imageFlags, DefaultLargeSize, InsertFloatingObjectImageLarge));
			list.Add(new ImageInfo(InsertImage, imageFlags, DefaultSmallSize, InsertImage));
			list.Add(new ImageInfo(InsertImageLarge, imageFlags, DefaultLargeSize, InsertImageLarge));
			list.Add(new ImageInfo(InsertPageBreak, imageFlags, DefaultSmallSize, InsertPageBreak));
			list.Add(new ImageInfo(InsertPageBreakLarge, imageFlags, DefaultLargeSize, InsertPageBreakLarge));
			list.Add(new ImageInfo(InsertPageCount, imageFlags, DefaultSmallSize, InsertPageCount));
			list.Add(new ImageInfo(InsertPageCountLarge, imageFlags, DefaultLargeSize, InsertPageCountLarge));
			list.Add(new ImageInfo(InsertPageNumber, imageFlags, DefaultSmallSize, InsertPageNumber));
			list.Add(new ImageInfo(InsertPageNumberLarge, imageFlags, DefaultLargeSize, InsertPageNumberLarge));
			list.Add(new ImageInfo(InsertSectionBreakContinuous, imageFlags, DefaultSmallSize, InsertSectionBreakContinuous));
			list.Add(new ImageInfo(InsertSectionBreakContinuousLarge, imageFlags, DefaultLargeSize, InsertSectionBreakContinuousLarge));
			list.Add(new ImageInfo(InsertSectionBreakEvenPage, imageFlags, DefaultSmallSize, InsertSectionBreakEvenPage));
			list.Add(new ImageInfo(InsertSectionBreakEvenPageLarge, imageFlags, DefaultLargeSize, InsertSectionBreakEvenPageLarge));
			list.Add(new ImageInfo(InsertSectionBreakNextPage, imageFlags, DefaultSmallSize, InsertSectionBreakNextPage));
			list.Add(new ImageInfo(InsertSectionBreakNextPageLarge, imageFlags, DefaultLargeSize, InsertSectionBreakNextPageLarge));
			list.Add(new ImageInfo(InsertSectionBreakOddPage, imageFlags, DefaultSmallSize, InsertSectionBreakOddPage));
			list.Add(new ImageInfo(InsertSectionBreakOddPageLarge, imageFlags, DefaultLargeSize, InsertSectionBreakOddPageLarge));
			list.Add(new ImageInfo(InsertTable, imageFlags, DefaultSmallSize, InsertTable));
			list.Add(new ImageInfo(InsertTableCaption, imageFlags, DefaultSmallSize, InsertTableCaption));
			list.Add(new ImageInfo(InsertTableCaptionLarge, imageFlags, DefaultLargeSize, InsertTableCaptionLarge));
			list.Add(new ImageInfo(InsertTableCells, imageFlags, DefaultSmallSize, InsertTableCells));
			list.Add(new ImageInfo(InsertTableCellsLarge, imageFlags, DefaultLargeSize, InsertTableCellsLarge));
			list.Add(new ImageInfo(InsertTableColumnsToTheLeft, imageFlags, DefaultSmallSize, InsertTableColumnsToTheLeft));
			list.Add(new ImageInfo(InsertTableColumnsToTheLeftLarge, imageFlags, DefaultLargeSize, InsertTableColumnsToTheLeftLarge));
			list.Add(new ImageInfo(InsertTableColumnsToTheRight, imageFlags, DefaultSmallSize, InsertTableColumnsToTheRight));
			list.Add(new ImageInfo(InsertTableColumnsToTheRightLarge, imageFlags, DefaultLargeSize, InsertTableColumnsToTheRightLarge));
			list.Add(new ImageInfo(InsertTableLarge, imageFlags, DefaultLargeSize, InsertTableLarge));
			list.Add(new ImageInfo(InsertTableOfCaptions, imageFlags, DefaultSmallSize, InsertTableOfCaptions));
			list.Add(new ImageInfo(InsertTableOfCaptionsLarge, imageFlags, DefaultLargeSize, InsertTableOfCaptionsLarge));
			list.Add(new ImageInfo(InsertTableOfContents, imageFlags, DefaultSmallSize, InsertTableOfContents));
			list.Add(new ImageInfo(InsertTableOfContentsLarge, imageFlags, DefaultLargeSize, InsertTableOfContentsLarge));
			list.Add(new ImageInfo(InsertTableOfEquations, imageFlags, DefaultSmallSize, InsertTableOfEquations));
			list.Add(new ImageInfo(InsertTableOfEquationsLarge, imageFlags, DefaultLargeSize, InsertTableOfEquationsLarge));
			list.Add(new ImageInfo(InsertTableOfFigures, imageFlags, DefaultSmallSize, InsertTableOfFigures));
			list.Add(new ImageInfo(InsertTableOfFiguresLarge, imageFlags, DefaultLargeSize, InsertTableOfFiguresLarge));
			list.Add(new ImageInfo(InsertTableRowsAbove, imageFlags, DefaultSmallSize, InsertTableRowsAbove));
			list.Add(new ImageInfo(InsertTableRowsAboveLarge, imageFlags, DefaultLargeSize, InsertTableRowsAboveLarge));
			list.Add(new ImageInfo(InsertTableRowsBelow, imageFlags, DefaultSmallSize, InsertTableRowsBelow));
			list.Add(new ImageInfo(InsertTableRowsBelowLarge, imageFlags, DefaultLargeSize, InsertTableRowsBelowLarge));
			list.Add(new ImageInfo(InsertTextBox, imageFlags, DefaultSmallSize, InsertTextBox));
			list.Add(new ImageInfo(InsertTextBoxLarge, imageFlags, DefaultLargeSize, InsertTextBoxLarge));
			list.Add(new ImageInfo(Italic, imageFlags, DefaultSmallSize, Italic));
			list.Add(new ImageInfo(ItalicLarge, imageFlags, DefaultLargeSize, ItalicLarge));
			list.Add(new ImageInfo(Language, imageFlags, DefaultSmallSize, Language));
			list.Add(new ImageInfo(LanguageLarge, imageFlags, DefaultLargeSize, LanguageLarge));
			list.Add(new ImageInfo(Last, imageFlags, DefaultSmallSize, Last));
			list.Add(new ImageInfo(LastLarge, imageFlags, DefaultLargeSize, LastLarge));
			list.Add(new ImageInfo(LeftColumns, imageFlags, DefaultSmallSize, LeftColumns));
			list.Add(new ImageInfo(LineNumbering, imageFlags, DefaultSmallSize, LineNumbering));
			list.Add(new ImageInfo(LineNumberingLarge, imageFlags, DefaultLargeSize, LineNumberingLarge));
			list.Add(new ImageInfo(LineSpacing, imageFlags, DefaultSmallSize, LineSpacing));
			list.Add(new ImageInfo(LineSpacingLarge, imageFlags, DefaultLargeSize, LineSpacingLarge));
			list.Add(new ImageInfo(LinkToPrevious, imageFlags, DefaultSmallSize, LinkToPrevious));
			list.Add(new ImageInfo(LinkToPreviousLarge, imageFlags, DefaultLargeSize, LinkToPreviousLarge));
			list.Add(new ImageInfo(ListBullets, imageFlags, DefaultSmallSize, ListBullets));
			list.Add(new ImageInfo(ListBulletsLarge, imageFlags, DefaultLargeSize, ListBulletsLarge));
			list.Add(new ImageInfo(ListMultilevel, imageFlags, DefaultSmallSize, ListMultilevel));
			list.Add(new ImageInfo(ListMultilevelLarge, imageFlags, DefaultLargeSize, ListMultilevelLarge));
			list.Add(new ImageInfo(ListNumbers, imageFlags, DefaultSmallSize, ListNumbers));
			list.Add(new ImageInfo(ListNumbersLarge, imageFlags, DefaultLargeSize, ListNumbersLarge));
			list.Add(new ImageInfo(MailMerge, imageFlags, DefaultSmallSize, MailMerge));
			list.Add(new ImageInfo(MailMergeLarge, imageFlags, DefaultLargeSize, MailMergeLarge));
			list.Add(new ImageInfo(MergeTableCells, imageFlags, DefaultSmallSize, MergeTableCells));
			list.Add(new ImageInfo(MergeTableCellsLarge, imageFlags, DefaultLargeSize, MergeTableCellsLarge));
			list.Add(new ImageInfo(ModifyStyle, imageFlags, DefaultSmallSize, ModifyStyle));
			list.Add(new ImageInfo(ModifyStyleLarge, imageFlags, DefaultLargeSize, ModifyStyleLarge));
			list.Add(new ImageInfo(ModifyTableStyle, imageFlags, DefaultSmallSize, ModifyTableStyle));
			list.Add(new ImageInfo(ModifyTableStyleLarge, imageFlags, DefaultLargeSize, ModifyTableStyleLarge));
			list.Add(new ImageInfo(New, imageFlags, DefaultSmallSize, New));
			list.Add(new ImageInfo(NewComment, imageFlags, DefaultSmallSize, NewComment));
			list.Add(new ImageInfo(NewCommentLarge, imageFlags, DefaultLargeSize, NewCommentLarge));
			list.Add(new ImageInfo(NewLarge, imageFlags, DefaultLargeSize, NewLarge));
			list.Add(new ImageInfo(NewTableStyle, imageFlags, DefaultSmallSize, NewTableStyle));
			list.Add(new ImageInfo(NewTableStyleLarge, imageFlags, DefaultLargeSize, NewTableStyleLarge));
			list.Add(new ImageInfo(Next, imageFlags, DefaultSmallSize, Next));
			list.Add(new ImageInfo(NextComment, imageFlags, DefaultSmallSize, NextComment));
			list.Add(new ImageInfo(NextCommentLarge, imageFlags, DefaultLargeSize, NextCommentLarge));
			list.Add(new ImageInfo(NextLarge, imageFlags, DefaultLargeSize, NextLarge));
			list.Add(new ImageInfo(OneColumn, imageFlags, DefaultSmallSize, OneColumn));
			list.Add(new ImageInfo(Open, imageFlags, DefaultSmallSize, Open));
			list.Add(new ImageInfo(OpenLarge, imageFlags, DefaultLargeSize, OpenLarge));
			list.Add(new ImageInfo(PageColor, imageFlags, DefaultSmallSize, PageColor));
			list.Add(new ImageInfo(PageColorLarge, imageFlags, DefaultLargeSize, PageColorLarge));
			list.Add(new ImageInfo(PageMargins, imageFlags, DefaultSmallSize, PageMargins));
			list.Add(new ImageInfo(PageMarginsLarge, imageFlags, DefaultLargeSize, PageMarginsLarge));
			list.Add(new ImageInfo(PageMarginsModerate, imageFlags, DefaultSmallSize, PageMarginsModerate));
			list.Add(new ImageInfo(PageMarginsModerateLarge, imageFlags, DefaultLargeSize, PageMarginsModerateLarge));
			list.Add(new ImageInfo(PageMarginsNarrow, imageFlags, DefaultSmallSize, PageMarginsNarrow));
			list.Add(new ImageInfo(PageMarginsNarrowLarge, imageFlags, DefaultLargeSize, PageMarginsNarrowLarge));
			list.Add(new ImageInfo(PageMarginsNormal, imageFlags, DefaultSmallSize, PageMarginsNormal));
			list.Add(new ImageInfo(PageMarginsNormalLarge, imageFlags, DefaultLargeSize, PageMarginsNormalLarge));
			list.Add(new ImageInfo(PageMarginsWide, imageFlags, DefaultSmallSize, PageMarginsWide));
			list.Add(new ImageInfo(PageMarginsWideLarge, imageFlags, DefaultLargeSize, PageMarginsWideLarge));
			list.Add(new ImageInfo(PageOrientationLandscape, imageFlags, DefaultSmallSize, PageOrientationLandscape));
			list.Add(new ImageInfo(PageOrientationLandscapeLarge, imageFlags, DefaultLargeSize, PageOrientationLandscapeLarge));
			list.Add(new ImageInfo(PageOrientation, imageFlags, DefaultSmallSize, PageOrientation));
			list.Add(new ImageInfo(PageOrientationLarge, imageFlags, DefaultLargeSize, PageOrientationLarge));
			list.Add(new ImageInfo(PageOrientationPortrait, imageFlags, DefaultSmallSize, PageOrientationPortrait));
			list.Add(new ImageInfo(PageOrientationPortraitLarge, imageFlags, DefaultLargeSize, PageOrientationPortraitLarge));
			list.Add(new ImageInfo(PaperSize, imageFlags, DefaultSmallSize, PaperSize));
			list.Add(new ImageInfo(PaperSizeLarge, imageFlags, DefaultLargeSize, PaperSizeLarge));
			list.Add(new ImageInfo(Paragraph, imageFlags, DefaultSmallSize, Paragraph));
			list.Add(new ImageInfo(ParagraphLarge, imageFlags, DefaultLargeSize, ParagraphLarge));
			list.Add(new ImageInfo(ParagraphStyleLarge, imageFlags, DefaultLargeSize, ParagraphStyleLarge));
			list.Add(new ImageInfo(Paste, imageFlags, DefaultSmallSize, Paste));
			list.Add(new ImageInfo(PasteLarge, imageFlags, DefaultLargeSize, PasteLarge));
			list.Add(new ImageInfo(PasteSpecial, imageFlags, DefaultSmallSize, PasteSpecial));
			list.Add(new ImageInfo(PasteSpecialLarge, imageFlags, DefaultLargeSize, PasteSpecialLarge));
			list.Add(new ImageInfo(PenColor, imageFlags, DefaultSmallSize, PenColor));
			list.Add(new ImageInfo(PenColorLarge, imageFlags, DefaultLargeSize, PenColorLarge));
			list.Add(new ImageInfo(Prev, imageFlags, DefaultSmallSize, Prev));
			list.Add(new ImageInfo(Preview, imageFlags, DefaultSmallSize, Preview));
			list.Add(new ImageInfo(PreviewLarge, imageFlags, DefaultLargeSize, PreviewLarge));
			list.Add(new ImageInfo(PreviousComment, imageFlags, DefaultSmallSize, PreviousComment));
			list.Add(new ImageInfo(PreviousCommentLarge, imageFlags, DefaultLargeSize, PreviousCommentLarge));
			list.Add(new ImageInfo(PrevLarge, imageFlags, DefaultLargeSize, PrevLarge));
			list.Add(new ImageInfo(Print, imageFlags, DefaultSmallSize, Print));
			list.Add(new ImageInfo(PrintBrowser, imageFlags, DefaultSmallSize, PrintBrowser));
			list.Add(new ImageInfo(PrintBrowserLarge, imageFlags, DefaultLargeSize, PrintBrowserLarge));
			list.Add(new ImageInfo(PrintDialog, imageFlags, DefaultSmallSize, PrintDialog));
			list.Add(new ImageInfo(PrintDialogLarge, imageFlags, DefaultLargeSize, PrintDialogLarge));
			list.Add(new ImageInfo(PrintLarge, imageFlags, DefaultLargeSize, PrintLarge));
			list.Add(new ImageInfo(PrintLayoutView, imageFlags, DefaultSmallSize, PrintLayoutView));
			list.Add(new ImageInfo(PrintLayoutViewLarge, imageFlags, DefaultLargeSize, PrintLayoutViewLarge));
			list.Add(new ImageInfo(PrintPreviewBrowser, imageFlags, DefaultSmallSize, PrintPreviewBrowser));
			list.Add(new ImageInfo(PrintPreviewBrowserLarge, imageFlags, DefaultLargeSize, PrintPreviewBrowserLarge));
			list.Add(new ImageInfo(ProtectDocument, imageFlags, DefaultSmallSize, ProtectDocument));
			list.Add(new ImageInfo(ProtectDocumentLarge, imageFlags, DefaultLargeSize, ProtectDocumentLarge));
			list.Add(new ImageInfo(ReadingLayoutView, imageFlags, DefaultSmallSize, ReadingLayoutView));
			list.Add(new ImageInfo(ReadingLayoutViewLarge, imageFlags, DefaultLargeSize, ReadingLayoutViewLarge));
			list.Add(new ImageInfo(Redo, imageFlags, DefaultSmallSize, Redo));
			list.Add(new ImageInfo(RedoLarge, imageFlags, DefaultLargeSize, RedoLarge));
			list.Add(new ImageInfo(Replace, imageFlags, DefaultSmallSize, Replace));
			list.Add(new ImageInfo(ReplaceLarge, imageFlags, DefaultLargeSize, ReplaceLarge));
			list.Add(new ImageInfo(Reviewers, imageFlags, DefaultSmallSize, Reviewers));
			list.Add(new ImageInfo(ReviewersLarge, imageFlags, DefaultLargeSize, ReviewersLarge));
			list.Add(new ImageInfo(ReviewingPane, imageFlags, DefaultSmallSize, ReviewingPane));
			list.Add(new ImageInfo(ReviewingPaneLarge, imageFlags, DefaultLargeSize, ReviewingPaneLarge));
			list.Add(new ImageInfo(RightColumns, imageFlags, DefaultSmallSize, RightColumns));
			list.Add(new ImageInfo(RulerHorizontal, imageFlags, DefaultSmallSize, RulerHorizontal));
			list.Add(new ImageInfo(RulerHorizontalLarge, imageFlags, DefaultLargeSize, RulerHorizontalLarge));
			list.Add(new ImageInfo(RulerVertical, imageFlags, DefaultSmallSize, RulerVertical));
			list.Add(new ImageInfo(RulerVerticalLarge, imageFlags, DefaultLargeSize, RulerVerticalLarge));
			list.Add(new ImageInfo(Save, imageFlags, DefaultSmallSize, Save));
			list.Add(new ImageInfo(SaveAll, imageFlags, DefaultSmallSize, SaveAll));
			list.Add(new ImageInfo(SaveAllLarge, imageFlags, DefaultLargeSize, SaveAllLarge));
			list.Add(new ImageInfo(SaveAs, imageFlags, DefaultSmallSize, SaveAs));
			list.Add(new ImageInfo(SaveAsLarge, imageFlags, DefaultLargeSize, SaveAsLarge));
			list.Add(new ImageInfo(SaveLarge, imageFlags, DefaultLargeSize, SaveLarge));
			list.Add(new ImageInfo(Select, imageFlags, DefaultSmallSize, Select));
			list.Add(new ImageInfo(SelectAll, imageFlags, DefaultSmallSize, SelectAll));
			list.Add(new ImageInfo(SelectAllLarge, imageFlags, DefaultLargeSize, SelectAllLarge));
			list.Add(new ImageInfo(SelectLarge, imageFlags, DefaultLargeSize, SelectLarge));
			list.Add(new ImageInfo(SelectTable, imageFlags, DefaultSmallSize, SelectTable));
			list.Add(new ImageInfo(SelectTableCell, imageFlags, DefaultSmallSize, SelectTableCell));
			list.Add(new ImageInfo(SelectTableCellLarge, imageFlags, DefaultLargeSize, SelectTableCellLarge));
			list.Add(new ImageInfo(SelectTableColumn, imageFlags, DefaultSmallSize, SelectTableColumn));
			list.Add(new ImageInfo(SelectTableColumnLarge, imageFlags, DefaultLargeSize, SelectTableColumnLarge));
			list.Add(new ImageInfo(SelectTableLarge, imageFlags, DefaultLargeSize, SelectTableLarge));
			list.Add(new ImageInfo(SelectTableRow, imageFlags, DefaultSmallSize, SelectTableRow));
			list.Add(new ImageInfo(SelectTableRowLarge, imageFlags, DefaultLargeSize, SelectTableRowLarge));
			list.Add(new ImageInfo(Shading, imageFlags, DefaultSmallSize, Shading));
			list.Add(new ImageInfo(ShadingLarge, imageFlags, DefaultLargeSize, ShadingLarge));
			list.Add(new ImageInfo(ShowAllFieldCodes, imageFlags, DefaultSmallSize, ShowAllFieldCodes));
			list.Add(new ImageInfo(ShowAllFieldCodesLarge, imageFlags, DefaultLargeSize, ShowAllFieldCodesLarge));
			list.Add(new ImageInfo(ShowAllFieldResults, imageFlags, DefaultSmallSize, ShowAllFieldResults));
			list.Add(new ImageInfo(ShowAllFieldResultsLarge, imageFlags, DefaultLargeSize, ShowAllFieldResultsLarge));
			list.Add(new ImageInfo(ShowComments, imageFlags, DefaultSmallSize, ShowComments));
			list.Add(new ImageInfo(ShowCommentsLarge, imageFlags, DefaultLargeSize, ShowCommentsLarge));
			list.Add(new ImageInfo(ShowHidden, imageFlags, DefaultSmallSize, ShowHidden));
			list.Add(new ImageInfo(ShowHiddenLarge, imageFlags, DefaultLargeSize, ShowHiddenLarge));
			list.Add(new ImageInfo(SimpleView, imageFlags, DefaultSmallSize, SimpleView));
			list.Add(new ImageInfo(SimpleViewLarge, imageFlags, DefaultLargeSize, SimpleViewLarge));
			list.Add(new ImageInfo(SpacingDecrease, imageFlags, DefaultSmallSize, SpacingDecrease));
			list.Add(new ImageInfo(SpacingDecreaseLarge, imageFlags, DefaultLargeSize, SpacingDecreaseLarge));
			list.Add(new ImageInfo(SpellCheck, imageFlags, DefaultSmallSize, SpellCheck));
			list.Add(new ImageInfo(SpellCheckAsYouType, imageFlags, DefaultSmallSize, SpellCheckAsYouType));
			list.Add(new ImageInfo(SpellCheckAsYouTypeLarge, imageFlags, DefaultLargeSize, SpellCheckAsYouTypeLarge));
			list.Add(new ImageInfo(SpellCheckLarge, imageFlags, DefaultLargeSize, SpellCheckLarge));
			list.Add(new ImageInfo(SplitTable, imageFlags, DefaultSmallSize, SplitTable));
			list.Add(new ImageInfo(SplitTableCells, imageFlags, DefaultSmallSize, SplitTableCells));
			list.Add(new ImageInfo(SplitTableCellsLarge, imageFlags, DefaultLargeSize, SplitTableCellsLarge));
			list.Add(new ImageInfo(SplitTableLarge, imageFlags, DefaultLargeSize, SplitTableLarge));
			list.Add(new ImageInfo(Strikeout, imageFlags, DefaultSmallSize, Strikeout));
			list.Add(new ImageInfo(StrikeoutDouble, imageFlags, DefaultSmallSize, StrikeoutDouble));
			list.Add(new ImageInfo(StrikeoutDoubleLarge, imageFlags, DefaultLargeSize, StrikeoutDoubleLarge));
			list.Add(new ImageInfo(StrikeoutLarge, imageFlags, DefaultLargeSize, StrikeoutLarge));
			list.Add(new ImageInfo(Subscript, imageFlags, DefaultSmallSize, Subscript));
			list.Add(new ImageInfo(SubscriptLarge, imageFlags, DefaultLargeSize, SubscriptLarge));
			list.Add(new ImageInfo(Superscript, imageFlags, DefaultSmallSize, Superscript));
			list.Add(new ImageInfo(SuperscriptLarge, imageFlags, DefaultLargeSize, SuperscriptLarge));
			list.Add(new ImageInfo(Symbol, imageFlags, DefaultSmallSize, Symbol));
			list.Add(new ImageInfo(SymbolLarge, imageFlags, DefaultLargeSize, SymbolLarge));
			list.Add(new ImageInfo(TableAutoFitContents, imageFlags, DefaultSmallSize, TableAutoFitContents));
			list.Add(new ImageInfo(TableAutoFitContentsLarge, imageFlags, DefaultLargeSize, TableAutoFitContentsLarge));
			list.Add(new ImageInfo(TableAutoFitWindow, imageFlags, DefaultSmallSize, TableAutoFitWindow));
			list.Add(new ImageInfo(TableAutoFitWindowLarge, imageFlags, DefaultLargeSize, TableAutoFitWindowLarge));
			list.Add(new ImageInfo(TableCellMargins, imageFlags, DefaultSmallSize, TableCellMargins));
			list.Add(new ImageInfo(TableCellMarginsLarge, imageFlags, DefaultLargeSize, TableCellMarginsLarge));
			list.Add(new ImageInfo(TableFixedColumnWidth, imageFlags, DefaultSmallSize, TableFixedColumnWidth));
			list.Add(new ImageInfo(TableFixedColumnWidthLarge, imageFlags, DefaultLargeSize, TableFixedColumnWidthLarge));
			list.Add(new ImageInfo(TableProperties, imageFlags, DefaultSmallSize, TableProperties));
			list.Add(new ImageInfo(TablePropertiesLarge, imageFlags, DefaultLargeSize, TablePropertiesLarge));
			list.Add(new ImageInfo(TableStyleLarge, imageFlags, DefaultLargeSize, TableStyleLarge));
			list.Add(new ImageInfo(TextWrapBehind, imageFlags, DefaultSmallSize, TextWrapBehind));
			list.Add(new ImageInfo(TextWrapBehindLarge, imageFlags, DefaultLargeSize, TextWrapBehindLarge));
			list.Add(new ImageInfo(TextWrapInFrontOfText, imageFlags, DefaultSmallSize, TextWrapInFrontOfText));
			list.Add(new ImageInfo(TextWrapInFrontOfTextLarge, imageFlags, DefaultLargeSize, TextWrapInFrontOfTextLarge));
			list.Add(new ImageInfo(TextWrapSquare, imageFlags, DefaultSmallSize, TextWrapSquare));
			list.Add(new ImageInfo(TextWrapSquareLarge, imageFlags, DefaultLargeSize, TextWrapSquareLarge));
			list.Add(new ImageInfo(TextWrapThrough, imageFlags, DefaultSmallSize, TextWrapThrough));
			list.Add(new ImageInfo(TextWrapThroughLarge, imageFlags, DefaultLargeSize, TextWrapThroughLarge));
			list.Add(new ImageInfo(TextWrapTight, imageFlags, DefaultSmallSize, TextWrapTight));
			list.Add(new ImageInfo(TextWrapTightLarge, imageFlags, DefaultLargeSize, TextWrapTightLarge));
			list.Add(new ImageInfo(TextWrapTopAndBottom, imageFlags, DefaultSmallSize, TextWrapTopAndBottom));
			list.Add(new ImageInfo(TextWrapTopAndBottomLarge, imageFlags, DefaultLargeSize, TextWrapTopAndBottomLarge));
			list.Add(new ImageInfo(ThreeColumns, imageFlags, DefaultSmallSize, ThreeColumns));
			list.Add(new ImageInfo(ToggleFieldCodes, imageFlags, DefaultSmallSize, ToggleFieldCodes));
			list.Add(new ImageInfo(ToggleFieldCodesLarge, imageFlags, DefaultLargeSize, ToggleFieldCodesLarge));
			list.Add(new ImageInfo(TwoColumns, imageFlags, DefaultSmallSize, TwoColumns));
			list.Add(new ImageInfo(Underline, imageFlags, DefaultSmallSize, Underline));
			list.Add(new ImageInfo(UnderlineDouble, imageFlags, DefaultSmallSize, UnderlineDouble));
			list.Add(new ImageInfo(UnderlineDoubleLarge, imageFlags, DefaultLargeSize, UnderlineDoubleLarge));
			list.Add(new ImageInfo(UnderlineLarge, imageFlags, DefaultLargeSize, UnderlineLarge));
			list.Add(new ImageInfo(UnderlineWord, imageFlags, DefaultSmallSize, UnderlineWord));
			list.Add(new ImageInfo(UnderlineWordLarge, imageFlags, DefaultLargeSize, UnderlineWordLarge));
			list.Add(new ImageInfo(Undo, imageFlags, DefaultSmallSize, Undo));
			list.Add(new ImageInfo(UndoLarge, imageFlags, DefaultLargeSize, UndoLarge));
			list.Add(new ImageInfo(UnprotectDocument, imageFlags, DefaultSmallSize, UnprotectDocument));
			list.Add(new ImageInfo(UnprotectDocumentLarge, imageFlags, DefaultLargeSize, UnprotectDocumentLarge));
			list.Add(new ImageInfo(UpdateField, imageFlags, DefaultSmallSize, UpdateField));
			list.Add(new ImageInfo(UpdateFieldLarge, imageFlags, DefaultLargeSize, UpdateFieldLarge));
			list.Add(new ImageInfo(UpdateTableOfContents, imageFlags, DefaultSmallSize, UpdateTableOfContents));
			list.Add(new ImageInfo(UpdateTableOfContentsLarge, imageFlags, DefaultLargeSize, UpdateTableOfContentsLarge));
			list.Add(new ImageInfo(ViewMergedData, imageFlags, DefaultSmallSize, ViewMergedData));
			list.Add(new ImageInfo(ViewMergedDataLarge, imageFlags, DefaultLargeSize, ViewMergedDataLarge));
			list.Add(new ImageInfo(ViewTableGridlines, imageFlags, DefaultSmallSize, ViewTableGridlines));
			list.Add(new ImageInfo(ViewTableGridlinesLarge, imageFlags, DefaultLargeSize, ViewTableGridlinesLarge));
			list.Add(new ImageInfo(ZoomIn, imageFlags, DefaultSmallSize, ZoomIn));
			list.Add(new ImageInfo(ZoomInLarge, imageFlags, DefaultLargeSize, ZoomInLarge));
			list.Add(new ImageInfo(ZoomOut, imageFlags, DefaultSmallSize, ZoomOut));
			list.Add(new ImageInfo(ZoomOutLarge, imageFlags, DefaultLargeSize, ZoomOutLarge));
			#endregion
		}
		protected internal override string GetDefaultSpriteName() {
			return RibbonRESpriteName;
		}
	}
	public static class RegisterRibbonIconsHelper {
		static Dictionary<string, string> registeredRibbonSpriteCssFiles = new Dictionary<string, string>();
		public static Dictionary<string, string> RegisteredRibbonSpriteCssFiles {
			get {
				if(HttpContext.Current == null)
					return registeredRibbonSpriteCssFiles;
				return HttpUtils.GetContextObject<Dictionary<string, string>>("DXRegisteredRibbonSpriteCssFiles");
			}
		}
		public static void AddResourcePathToRegister(string resourcePath, string spriteName) {
			if(!string.IsNullOrEmpty(resourcePath) && !RegisteredRibbonSpriteCssFiles.Keys.Contains(resourcePath))
				RegisteredRibbonSpriteCssFiles.Add(resourcePath, spriteName);
		}
		public static void UnregisterResource(string resourcePath) {
			if(!string.IsNullOrEmpty(resourcePath) && RegisteredRibbonSpriteCssFiles.Keys.Contains(resourcePath))
				RegisteredRibbonSpriteCssFiles.Remove(resourcePath);
		}
		public static void RegisterResources(Page page) {
			foreach(string resourcePath in RegisteredRibbonSpriteCssFiles.Keys)
				ResourceManager.RegisterCssResource(page, typeof(ASPxWebControl), resourcePath);
		}
		public static void RegisterSpriteResourceByOwner(ControlRibbonImages owner) {
			string defaultSpriteName = owner.GetDefaultSpriteName();
			if(!string.IsNullOrEmpty(defaultSpriteName) && owner.IconSet != MenuIconSetType.NotSet)
				ResourceManager.RegisterCssResource(null, typeof(ASPxWebControl), ASPxWebControl.WebCssResourcePath + owner.GetSpriteNameByIconSet(owner.IconSet) + ".css");
		}
	}
}
