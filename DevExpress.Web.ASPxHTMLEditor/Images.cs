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
using System.Web.UI;
using System.Web.UI.WebControls;
using DevExpress.Web.ASPxHtmlEditor.Localization;
using DevExpress.Web.Internal;
using DevExpress.Web.ASPxHtmlEditor.Internal;
namespace DevExpress.Web.ASPxHtmlEditor {
	public class PopOutImageSpriteProperties: ItemImageSpriteProperties {
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new string SelectedCssClass {
			get { return base.SelectedCssClass; }
			set { base.SelectedCssClass = value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new Unit SelectedLeft {
			get { return base.SelectedLeft; }
			set { base.SelectedLeft = value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new Unit SelectedTop {
			get { return base.SelectedTop; }
			set { base.SelectedTop = value; }
		}
		public PopOutImageSpriteProperties()
			: base() {
		}
		public PopOutImageSpriteProperties(IPropertiesOwner owner)
			: base(owner) {
		}
	}
	public class PopOutImageProperties: ItemImagePropertiesBase {
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("PopOutImagePropertiesSpriteProperties"),
#endif
		PersistenceMode(PersistenceMode.InnerProperty), DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		NotifyParentProperty(true), AutoFormatEnable]
		public PopOutImageSpriteProperties SpriteProperties {
			get { return (PopOutImageSpriteProperties)SpritePropertiesInternal; }
		}
#if !SL
	[DevExpressWebASPxHtmlEditorLocalizedDescription("PopOutImagePropertiesUrlSelected")]
#endif
		public new string UrlSelected {
			get { return base.UrlSelected; }
			set { base.UrlSelected = value; }
		}
		public PopOutImageProperties()
			: base() {
		}
		public PopOutImageProperties(string url)
			: base(url) {
		}
		public PopOutImageProperties(IPropertiesOwner owner)
			: base(owner) {
		}
		protected override ImageSpriteProperties CreateSpriteProperties() {
			return new PopOutImageSpriteProperties(this);
		}
	}
	public class HtmlEditorImages: ImagesBase {
		public const string CloseButtonImageName = "heCloseButton",
							ErrorFrameCloseButtonImageName = "heErrorFrameCloseButton",
							ToolbarPopOutImageName = "heToolbarPopOut",
							ConstrainProportionsBottomImageName = "heConstrainProportionsBottom",
							ConstrainProportionsTopImageName = "heConstrainProportionsTop",
							ConstrainProportionsOffImageName = "heConstrainProportionsMiddleOff",
							ConstrainProportionsMiddleOnImageName = "heConstrainProportionsMiddleOn",
							ConstrainProportionsBottomRtlImageName = "heConstrainProportionsBottomRtl",
							ConstrainProportionsTopRtlImageName = "heConstrainProportionsTopRtl",
							ConstrainProportionsOffRtlImageName = "heConstrainProportionsMiddleOffRtl",
							ConstrainProportionsMiddleOnRtlImageName = "heConstrainProportionsMiddleOnRtl",
							SizeGripImageName = "heSizeGrip",
							SizeGripRtlImageName = "heSizeGripRtl",
							TagInspectorRemoveElementButtonImageName = "heTagInspectorRemoveButton",
							TagInspectorChangeElementButtonImageName = "heTagInspectorChangeButton",
							TagInspectorSeparatorImageName = "heTagInspectorSeparator",
							InsertImageDialogResetImageName = "heReset";
		protected internal const string  FieldImageName = "heFieldIcon",
							  EventImageName = "heEventIcon",
							  XmlItemImageName = "heXmlItemIcon",
							  EnumImageName = "heEnumIcon";
		HtmlEditorIconImages iconImages;
		public HtmlEditorImages(ISkinOwner owner)
			: base(owner) {
		}
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("HtmlEditorImagesTagInspectorRemoveElementButton"),
#endif
		Category("Images"), PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable,
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true)]
		public CheckedButtonImageProperties TagInspectorRemoveElementButton {
			get { return (CheckedButtonImageProperties)GetImageBase(TagInspectorRemoveElementButtonImageName); }
		}
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("HtmlEditorImagesTagInspectorChangeElementButton"),
#endif
		Category("Images"), PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable,
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true)]
		public CheckedButtonImageProperties TagInspectorChangeElementButton {
			get { return (CheckedButtonImageProperties)GetImageBase(TagInspectorChangeElementButtonImageName); }
		}
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("HtmlEditorImagesDialogFormCloseButton"),
#endif
		Category("Images"), PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable,
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true)]
		public HeaderButtonImageProperties DialogFormCloseButton {
			get { return (HeaderButtonImageProperties)GetImageBase(CloseButtonImageName); }
		}
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("HtmlEditorImagesErrorFrameCloseButton"),
#endif
		Category("Images"), PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable,
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true)]
		public HeaderButtonImageProperties ErrorFrameCloseButton {
			get { return (HeaderButtonImageProperties)GetImageBase(ErrorFrameCloseButtonImageName); }
		}
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("HtmlEditorImagesToolbarPopOut"),
#endif
		Category("Images"), PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable,
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true)]
		public PopOutImageProperties ToolbarPopOut {
			get { return (PopOutImageProperties)GetImageBase(ToolbarPopOutImageName); }
		}
		[
		Category("Images"), AutoFormatEnable, NotifyParentProperty(true), DefaultValue(MenuIconSetType.NotSet)]
		public MenuIconSetType MenuIconSet {
			get { return IconImages.MenuIconSet; }
			set { IconImages.MenuIconSet = value; }
		}
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("HtmlEditorImagesInsertImageDialogConstrainProportionsMiddleOn"),
#endif
		Category("Images"), PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable,
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true)]
		public ImageProperties InsertImageDialogConstrainProportionsMiddleOn {
			get { return GetImage(ConstrainProportionsMiddleOnImageName); }
		}
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("HtmlEditorImagesInsertImageDialogConstrainProportionsMiddleOff"),
#endif
		Category("Images"), PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable,
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true)]
		public ImageProperties InsertImageDialogConstrainProportionsMiddleOff {
			get { return GetImage(ConstrainProportionsOffImageName); }
		}
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("HtmlEditorImagesInsertImageDialogConstrainProportionsBottom"),
#endif
		Category("Images"), PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable,
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true)]
		public ImageProperties InsertImageDialogConstrainProportionsBottom {
			get { return GetImage(ConstrainProportionsBottomImageName); }
		}
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("HtmlEditorImagesInsertImageDialogConstrainProportionsTop"),
#endif
		Category("Images"), PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable,
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true)]
		public ImageProperties InsertImageDialogConstrainProportionsTop {
			get { return GetImage(ConstrainProportionsTopImageName); }
		}
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("HtmlEditorImagesInsertImageDialogResetButton"),
#endif
		Category("Images"), PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable,
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true)]
		public ImageProperties InsertImageDialogResetButton {
			get { return GetImage(InsertImageDialogResetImageName); }
		}
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("HtmlEditorImagesSizeGrip"),
#endif
		Category("Images"), PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable,
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true)]
		public ImageProperties SizeGrip {
			get { return GetImage(SizeGripImageName); }
		}
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("HtmlEditorImagesTagInspectorSeparator"),
#endif
		Category("Images"), PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable,
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true)]
		public ImageProperties TagInspectorSeparator {
			get { return GetImage(TagInspectorSeparatorImageName); }
		}
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("HtmlEditorImagesInsertImageDialogConstrainProportionsMiddleOnRtl"),
#endif
		Category("Images"), PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable,
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true)]
		public ImageProperties InsertImageDialogConstrainProportionsMiddleOnRtl {
			get { return GetImage(ConstrainProportionsMiddleOnRtlImageName); }
		}
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("HtmlEditorImagesInsertImageDialogConstrainProportionsMiddleOffRtl"),
#endif
		Category("Images"), PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable,
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true)]
		public ImageProperties InsertImageDialogConstrainProportionsMiddleOffRtl {
			get { return GetImage(ConstrainProportionsOffRtlImageName); }
		}
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("HtmlEditorImagesInsertImageDialogConstrainProportionsBottomRtl"),
#endif
		Category("Images"), PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable,
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true)]
		public ImageProperties InsertImageDialogConstrainProportionsBottomRtl {
			get { return GetImage(ConstrainProportionsBottomRtlImageName); }
		}
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("HtmlEditorImagesInsertImageDialogConstrainProportionsTopRtl"),
#endif
		Category("Images"), PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable,
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true)]
		public ImageProperties InsertImageDialogConstrainProportionsTopRtl {
			get { return GetImage(ConstrainProportionsTopRtlImageName); }
		}
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("HtmlEditorImagesSizeGripRtl"),
#endif
		Category("Images"), PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable,
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true)]
		public ImageProperties SizeGripRtl {
			get { return GetImage(SizeGripRtlImageName); }
		}
		protected internal ImageProperties FieldImage {
			get { return GetImage(FieldImageName); }
		}
		protected internal ImageProperties EventImage {
			get { return GetImage(EventImageName); }
		}
		protected internal ImageProperties XmlItemImage {
			get { return GetImage(XmlItemImageName); }
		}
		protected internal ImageProperties EnumImage {
			get { return GetImage(EnumImageName); }
		}
		protected internal HtmlEditorIconImages IconImages {
			get {
				if(iconImages == null)
					iconImages = new HtmlEditorIconImages((ISkinOwner)Owner);
				return iconImages;
			}
		}
		public override void CopyFrom(ImagesBase source) {
			base.CopyFrom(source);
			if(source is HtmlEditorImages)
				IconImages.CopyFrom(((HtmlEditorImages)source).IconImages);
		}
		public override void Reset() {
			base.Reset();
			MenuIconSet = MenuIconSetType.NotSet;
		}
		protected override bool KeepDefaultSizes {
			get { return true; }
		}
		protected override Type GetResourceType() {
			return typeof(ASPxHtmlEditor);
		}
		protected override string GetResourceImagePath() {
			return ASPxHtmlEditor.HtmlEditorImagesResourcePath;
		}
		protected override string GetDesignTimeResourceSpriteImagePath() {
			return ASPxHtmlEditor.HtmlEditorImagesResourcePath + GetDesignTimeResourceSpriteImageDefaultName();
		}
		protected override string GetResourceSpriteCssPath() {
			return ASPxHtmlEditor.HtmlEditorSpriteCssResourceName;
		}
		protected override void PopulateImageInfoList(List<ImageInfo> list) {
			base.PopulateImageInfoList(list);
			list.Add(new ImageInfo(InsertImageDialogResetImageName, ImageFlags.IsPng, 20, 20,
			   InsertImageDialogResetImageName));
			list.Add(new ImageInfo(TagInspectorSeparatorImageName, ImageFlags.IsPng, 4, 7,
			   TagInspectorSeparatorImageName));
			list.Add(new ImageInfo(TagInspectorChangeElementButtonImageName, ImageFlags.IsPng, 19, 19,
				delegate() { return ASPxHtmlEditorLocalizer.GetString(ASPxHtmlEditorStringId.ChangeElementProperties); },
				typeof(CheckedButtonImageProperties), TagInspectorChangeElementButtonImageName));
			list.Add(new ImageInfo(TagInspectorRemoveElementButtonImageName, ImageFlags.IsPng, 19, 19,
				delegate() { return ASPxHtmlEditorLocalizer.GetString(ASPxHtmlEditorStringId.DeleteElement); },
				typeof(CheckedButtonImageProperties), TagInspectorRemoveElementButtonImageName));
			list.Add(new ImageInfo(CloseButtonImageName, ImageFlags.HasNoResourceImage, typeof(HeaderButtonImageProperties)));
			list.Add(new ImageInfo(ErrorFrameCloseButtonImageName, ImageFlags.IsPng, 13, 9, "Close",
				typeof(HeaderButtonImageProperties), ErrorFrameCloseButtonImageName));
			list.Add(new ImageInfo(ToolbarPopOutImageName, ImageFlags.IsPng, "", typeof(PopOutImageProperties), 
				ToolbarPopOutImageName));
			list.Add(new ImageInfo(ConstrainProportionsMiddleOnImageName, ImageFlags.IsPng, 11, 18,
				delegate() { return ASPxHtmlEditorLocalizer.GetString(ASPxHtmlEditorStringId.Alt_ConstrainProportions); }, 
				ConstrainProportionsMiddleOnImageName));
			list.Add(new ImageInfo(ConstrainProportionsOffImageName, ImageFlags.IsPng, 11, 18,
				delegate() { return ASPxHtmlEditorLocalizer.GetString(ASPxHtmlEditorStringId.Alt_ConstrainProportionsOff); },
				ConstrainProportionsOffImageName));
			list.Add(new ImageInfo(ConstrainProportionsBottomImageName, ImageFlags.IsPng, 11, 6, 
				ConstrainProportionsBottomImageName));
			list.Add(new ImageInfo(ConstrainProportionsTopImageName, ImageFlags.IsPng, 11, 5, 
				ConstrainProportionsTopImageName));
			list.Add(new ImageInfo(SizeGripImageName, ImageFlags.IsPng, 16, 16,
				SizeGripImageName));
			list.Add(new ImageInfo(SizeGripRtlImageName, ImageFlags.IsPng, 16, 16,
				SizeGripRtlImageName));
			list.Add(new ImageInfo(ConstrainProportionsMiddleOnRtlImageName, ImageFlags.IsPng, 11, 18,
				delegate() { return ASPxHtmlEditorLocalizer.GetString(ASPxHtmlEditorStringId.Alt_ConstrainProportions); },
				ConstrainProportionsMiddleOnRtlImageName));
			list.Add(new ImageInfo(ConstrainProportionsOffRtlImageName, ImageFlags.IsPng, 11, 18,
				delegate() { return ASPxHtmlEditorLocalizer.GetString(ASPxHtmlEditorStringId.Alt_ConstrainProportionsOff); },
				ConstrainProportionsOffRtlImageName));
			list.Add(new ImageInfo(ConstrainProportionsBottomRtlImageName, ImageFlags.IsPng, 11, 6,
				ConstrainProportionsBottomRtlImageName));
			list.Add(new ImageInfo(ConstrainProportionsTopRtlImageName, ImageFlags.IsPng, 11, 5,
				ConstrainProportionsTopRtlImageName));
			list.Add(new ImageInfo(FieldImageName, ImageFlags.IsPng, 16, 16, FieldImageName));
			list.Add(new ImageInfo(EnumImageName, ImageFlags.IsPng, 16, 16, EnumImageName));
			list.Add(new ImageInfo(EventImageName, ImageFlags.IsPng, 16, 16, EventImageName));
			list.Add(new ImageInfo(XmlItemImageName, ImageFlags.IsPng, 16, 16, XmlItemImageName));
		}
		protected override IStateManager[] GetStateManagedObjects() {
			return ViewStateUtils.GetMergedStateManagedObjects(base.GetStateManagedObjects(),
				new IStateManager[] { IconImages });
		}
	}
	public class HtmlEditorEditorImages : EditorImages {
		public HtmlEditorEditorImages(ISkinOwner skinOwner) : base(skinOwner) { }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new ImagePropertiesEx CheckBoxChecked {
			get { return base.CheckBoxChecked; }
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new ImagePropertiesEx CheckBoxUnchecked {
			get { return base.CheckBoxUnchecked; }
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new ImagePropertiesEx CheckBoxUndefined {
			get { return base.CheckBoxGrayed; }
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new ImagePropertiesEx CheckBoxGrayed {
			get { return base.CheckBoxGrayed; }
		}
	}
	public class HtmlEditorFileManagerImages : FileManagerImages {
		public HtmlEditorFileManagerImages(ISkinOwner skinOwner) : base(skinOwner) { }
	}
}
namespace DevExpress.Web.ASPxHtmlEditor.Internal {
	public class HtmlEditorIconImages : ImagesBase {
		internal readonly static Dictionary<MenuIconSetType, string> Categories = new Dictionary<MenuIconSetType, string>();
		internal readonly static Dictionary<MenuIconSetType, string> SpriteCssResourceNames = new Dictionary<MenuIconSetType, string>();
		internal readonly static Dictionary<MenuIconSetType, string> SpriteImageResourceNames = new Dictionary<MenuIconSetType, string>();
		private const int DefaultSmallSize = 16;
		private const int ColorIconHeight = 13;
		protected internal const string
							Cut = "heCut",
							Copy = "heCopy",
							Paste = "hePaste",
							PasteFromWord = "hePasteFromWord",
							Undo = "heUndo",
							Redo = "heRedo",
							Bold = "heBold",
							Italic = "heItalic",
							Underline = "heUnderline",
							Strikethrough = "heStrikethrough",
							Center = "heCenter",
							Left = "heLeft",
							Right = "heRight",
							Justify = "heJustifyFull",
							Superscript = "heSuperscript",
							Subscript = "heSubscript",
							Indent = "heIndent",
							Outdent = "heOutdent",
							FormatDocument = "heFormatDocument",
							OrderedList = "heInsertOrderedList",
							UnorderedList = "heInsertUnorderedList",
							InsertImageDialog = "heInsertImageDialog",
							InsertLinkDialog = "heInsertLinkDialog",
							InsertTableDialog = "heInsertTableDialog",
							TablePropertiesDialog = "heChangeTableDialog",
							TableCellPropertiesDialog = "heChangeTableCellDialog",
							TableColumnPropertiesDialog = "heChangeTableColumnDialog",
							TableRowPropertiesDialog = "heChangeTableRowDialog",
							InsertTableRowAbove = "heInsertTableRowAbove",
							InsertTableRowBelow = "heInsertTableRowBelow",
							InsertTableColumnToLeft = "heInsertTableColumnOnLeft",
							InsertTableColumnToRight = "heInsertTableColumnOnRight",
							SplitTableCellHorizontal = "heSplitTableCellHorizontal",
							SplitTableCellVertical = "heSplitTableCellVertical",
							MergeTableCellHorizontal = "heMergeTableCellHorizontal",
							MergeTableCellDown = "heMergeTableCellVertical",
							DeleteTable = "heDeleteTable",
							DeleteTableRow = "heDeleteTableRow",
							DeleteTableColumn = "heDeleteTableColumn",
							UnLink = "heUnlink",
							BackColor = "heBackColor",
							ForeColor = "heForeColor",
							RemoveFormat = "heRemoveFormat",
							CheckSpelling = "heCheckSpelling",
							Print = "hePrint",
							Fullscreen = "heFullscreen",
							ExportFormat = "heSaveAs{0}",
							InsertAudioDialog = "heInsertAudioDialog",
							InsertVideoDialog = "heInsertVideoDialog",
							InsertFlashDialog = "heInsertFlashDialog",
							InsertYouTubeVideoDialog = "heInsertYouTubeDialog",
							PasteHtmlSourceFormatting = "hePasteHtmlSourceFormatting",
							PasteHtmlMergeFormatting = "hePasteHtmlMergeFormatting",
							PasteHtmlPlainText = "hePasteHtmlPlainText",
							InsertPlaceholderDialog = "heInsertPlaceholder",
							SelectAll = "heSelectAll",
							ChangeElementPropertiesDialog = "dxhe-tiPropertyButton",
							FindAndReplaceDialog = "heFindAndReplaceDialog",
							Comment = "heComment",
							Uncomment = "heUncomment";
		static HtmlEditorIconImages() {
			Categories.Add(MenuIconSetType.NotSet, "Icons");
			Categories.Add(MenuIconSetType.Colored, "Icons");
			Categories.Add(MenuIconSetType.ColoredLight, "WIcons");
			Categories.Add(MenuIconSetType.GrayScaled, "GIcons");
			Categories.Add(MenuIconSetType.GrayScaledWithWhiteHottrack, "GWIcons");
			SpriteCssResourceNames.Add(MenuIconSetType.NotSet, ASPxHtmlEditor.HtmlEditorToolbarSpriteCssResourceName);
			SpriteCssResourceNames.Add(MenuIconSetType.Colored, ASPxHtmlEditor.HtmlEditorToolbarSpriteCssResourceName);
			SpriteCssResourceNames.Add(MenuIconSetType.ColoredLight, ASPxHtmlEditor.HtmlEditorToolbarWhiteSpriteCssResourceName);
			SpriteCssResourceNames.Add(MenuIconSetType.GrayScaled, ASPxHtmlEditor.HtmlEditorToolbarGrayScaleSpriteCssResourceName);
			SpriteCssResourceNames.Add(MenuIconSetType.GrayScaledWithWhiteHottrack, ASPxHtmlEditor.HtmlEditorToolbarGrayScaleWithWhiteHottrackSpriteCssResourceName);
			SpriteImageResourceNames.Add(MenuIconSetType.NotSet, ASPxHtmlEditor.HtmlEditorIconSpriteImageResourceName);
			SpriteImageResourceNames.Add(MenuIconSetType.Colored, ASPxHtmlEditor.HtmlEditorIconSpriteImageResourceName);
			SpriteImageResourceNames.Add(MenuIconSetType.ColoredLight, ASPxHtmlEditor.HtmlEditorWhiteIconSpriteImageResourceName);
			SpriteImageResourceNames.Add(MenuIconSetType.GrayScaled, ASPxHtmlEditor.HtmlEditorGrayScaleIconSpriteImageResourceName);
			SpriteImageResourceNames.Add(MenuIconSetType.GrayScaledWithWhiteHottrack, ASPxHtmlEditor.HtmlEditorGrayScaleWithWhiteHottrackIconSpriteImageResourceName);
		}
		public HtmlEditorIconImages(ISkinOwner properties)
			: base(properties) {
		}
		public MenuIconSetType MenuIconSet {
			get { return (MenuIconSetType)GetEnumProperty("MenuIconSet", MenuIconSetType.NotSet); }
			set {
				SetEnumProperty("MenuIconSet", MenuIconSetType.NotSet, value);
				Changed();
			}
		}
		protected override void PopulateImageInfoList(List<ImageInfo> list) {
			base.PopulateImageInfoList(list);
			ImageFlags imageFlags = ImageFlags.IsPng | ImageFlags.HasDisabledState;
			list.Add(new ImageInfo(Cut, imageFlags, DefaultSmallSize, Cut));
			list.Add(new ImageInfo(Copy, imageFlags, DefaultSmallSize, Copy));
			list.Add(new ImageInfo(Paste, imageFlags, DefaultSmallSize, Paste));
			list.Add(new ImageInfo(PasteFromWord, imageFlags, DefaultSmallSize, PasteFromWord));
			list.Add(new ImageInfo(Undo, imageFlags, DefaultSmallSize, Undo));
			list.Add(new ImageInfo(Redo, imageFlags, DefaultSmallSize, Redo));
			list.Add(new ImageInfo(Bold, imageFlags, DefaultSmallSize, Bold));
			list.Add(new ImageInfo(Italic, imageFlags, DefaultSmallSize, Italic));
			list.Add(new ImageInfo(Underline, imageFlags, DefaultSmallSize, Underline));
			list.Add(new ImageInfo(Strikethrough, imageFlags, DefaultSmallSize, Strikethrough));
			list.Add(new ImageInfo(Center, imageFlags, DefaultSmallSize, Center));
			list.Add(new ImageInfo(Left, imageFlags, DefaultSmallSize, Left));
			list.Add(new ImageInfo(Right, imageFlags, DefaultSmallSize, Right));
			list.Add(new ImageInfo(Justify, imageFlags, DefaultSmallSize, Justify));
			list.Add(new ImageInfo(Superscript, imageFlags, DefaultSmallSize, Superscript));
			list.Add(new ImageInfo(Subscript, imageFlags, DefaultSmallSize, Subscript));
			list.Add(new ImageInfo(Indent, imageFlags, DefaultSmallSize, Indent));
			list.Add(new ImageInfo(Outdent, imageFlags, DefaultSmallSize, Outdent));
			list.Add(new ImageInfo(FormatDocument, imageFlags, DefaultSmallSize, FormatDocument));
			list.Add(new ImageInfo(OrderedList, imageFlags, DefaultSmallSize, OrderedList));
			list.Add(new ImageInfo(UnorderedList, imageFlags, DefaultSmallSize, UnorderedList));
			list.Add(new ImageInfo(InsertImageDialog, imageFlags, DefaultSmallSize, InsertImageDialog));
			list.Add(new ImageInfo(InsertLinkDialog, imageFlags, DefaultSmallSize, InsertLinkDialog));
			list.Add(new ImageInfo(InsertTableDialog, imageFlags, DefaultSmallSize, InsertTableDialog));
			list.Add(new ImageInfo(TablePropertiesDialog, imageFlags, DefaultSmallSize, TablePropertiesDialog));
			list.Add(new ImageInfo(TableCellPropertiesDialog, imageFlags, DefaultSmallSize, TableCellPropertiesDialog));
			list.Add(new ImageInfo(TableColumnPropertiesDialog, imageFlags, DefaultSmallSize, TableColumnPropertiesDialog));
			list.Add(new ImageInfo(TableRowPropertiesDialog, imageFlags, DefaultSmallSize, TableRowPropertiesDialog));
			list.Add(new ImageInfo(InsertTableRowAbove, imageFlags, DefaultSmallSize, InsertTableRowAbove));
			list.Add(new ImageInfo(InsertTableRowBelow, imageFlags, DefaultSmallSize, InsertTableRowBelow));
			list.Add(new ImageInfo(InsertTableColumnToLeft, imageFlags, DefaultSmallSize, InsertTableColumnToLeft));
			list.Add(new ImageInfo(InsertTableColumnToRight, imageFlags, DefaultSmallSize, InsertTableColumnToRight));
			list.Add(new ImageInfo(SplitTableCellHorizontal, imageFlags, DefaultSmallSize, SplitTableCellHorizontal));
			list.Add(new ImageInfo(SplitTableCellVertical, imageFlags, DefaultSmallSize, SplitTableCellVertical));
			list.Add(new ImageInfo(MergeTableCellHorizontal, imageFlags, DefaultSmallSize, MergeTableCellHorizontal));
			list.Add(new ImageInfo(MergeTableCellDown, imageFlags, DefaultSmallSize, MergeTableCellDown));
			list.Add(new ImageInfo(ChangeElementPropertiesDialog, imageFlags, DefaultSmallSize, ChangeElementPropertiesDialog));
			list.Add(new ImageInfo(DeleteTable, imageFlags, DefaultSmallSize, DeleteTable));
			list.Add(new ImageInfo(DeleteTableColumn, imageFlags, DefaultSmallSize, DeleteTableColumn));
			list.Add(new ImageInfo(DeleteTableRow, imageFlags, DefaultSmallSize, DeleteTableRow));
			list.Add(new ImageInfo(UnLink, imageFlags, DefaultSmallSize, UnLink));
			list.Add(new ImageInfo(BackColor, imageFlags, DefaultSmallSize, ColorIconHeight, BackColor));
			list.Add(new ImageInfo(ForeColor, imageFlags, DefaultSmallSize, ColorIconHeight, ForeColor));
			list.Add(new ImageInfo(RemoveFormat, imageFlags, DefaultSmallSize, RemoveFormat));
			list.Add(new ImageInfo(Print, imageFlags, DefaultSmallSize, Print));
			list.Add(new ImageInfo(CheckSpelling, imageFlags, DefaultSmallSize, CheckSpelling));
			list.Add(new ImageInfo(Fullscreen, imageFlags, DefaultSmallSize, Fullscreen));
			list.Add(new ImageInfo(InsertAudioDialog, imageFlags, DefaultSmallSize, InsertAudioDialog));
			list.Add(new ImageInfo(InsertVideoDialog, imageFlags, DefaultSmallSize, InsertVideoDialog));
			list.Add(new ImageInfo(InsertFlashDialog, imageFlags, DefaultSmallSize, InsertFlashDialog));
			list.Add(new ImageInfo(InsertYouTubeVideoDialog, imageFlags, DefaultSmallSize, InsertYouTubeVideoDialog));
			list.Add(new ImageInfo(PasteHtmlSourceFormatting, imageFlags, DefaultSmallSize, PasteHtmlSourceFormatting));
			list.Add(new ImageInfo(PasteHtmlMergeFormatting, imageFlags, DefaultSmallSize, PasteHtmlMergeFormatting));
			list.Add(new ImageInfo(PasteHtmlPlainText, imageFlags, DefaultSmallSize, PasteHtmlPlainText));
			list.Add(new ImageInfo(InsertPlaceholderDialog, imageFlags, DefaultSmallSize, InsertPlaceholderDialog));
			list.Add(new ImageInfo(SelectAll, imageFlags, DefaultSmallSize, SelectAll));
			list.Add(new ImageInfo(FindAndReplaceDialog, imageFlags, DefaultSmallSize, FindAndReplaceDialog));
			list.Add(new ImageInfo(Comment, imageFlags, DefaultSmallSize, Comment));
			list.Add(new ImageInfo(Uncomment, imageFlags, DefaultSmallSize, Uncomment));
			foreach(string writeFormat in Enum.GetNames(typeof(HtmlEditorExportFormat))) {
				string resourceName = GetExportFormatResourceName(writeFormat);
				list.Add(new ImageInfo(resourceName, imageFlags, DefaultSmallSize, resourceName));
			}
		}
		internal static string GetExportFormatResourceName(string format) {
			return string.Format(ExportFormat, format);
		}
		internal static string GetExportFormatResourceName(HtmlEditorExportFormat format) {
			return GetExportFormatResourceName(format.ToString());
		}
		public override void CopyFrom(ImagesBase source) {
			base.CopyFrom(source);
			if(source is HtmlEditorIconImages)
				MenuIconSet = ((HtmlEditorIconImages)source).MenuIconSet;
		}
		public override void Reset() {
			base.Reset();
			MenuIconSet = MenuIconSetType.NotSet;
		}
		protected override bool KeepDefaultSizes {
			get { return true; }
		}
		protected override string GetCssPostFix() {
			if(MenuIconSet == MenuIconSetType.NotSet)
				return base.GetCssPostFix();
			return string.Empty;
		}
		protected override string GetImageCategory() {
			return Categories[MenuIconSet];
		}
		protected override Type GetResourceType() {
			return typeof(ASPxHtmlEditor);
		}
		protected override string GetResourceImagePath() {
			return ASPxHtmlEditor.HtmlEditorImagesResourcePath;
		}
		protected override string GetDesignTimeResourceSpriteImagePath() {
			return SpriteImageResourceNames[MenuIconSet];
		}
		protected override string GetResourceSpriteCssPath() {
			return SpriteCssResourceNames[MenuIconSet];
		}
		protected internal void RegisterIconSpriteCssFile(Page page) {
			base.RegisterDefaultSpriteCssFile(page);
		}
	}
	public class PasteOptionsBarItemImageProperties : DevExpress.Web.MenuItemImageProperties {
		public PasteOptionsBarItemImageProperties() : base() { }
		public PasteOptionsBarItemImageProperties(IPropertiesOwner owner) : base(owner) { }
		public PasteOptionsBarItemImageProperties(string url) : base(url) { }
		protected new string UrlChecked {
			get { return base.UrlChecked; }
			set { base.UrlChecked = value; }
		}
		protected new string UrlHottracked {
			get { return base.UrlHottracked; }
			set { base.UrlHottracked = value; }
		}
		protected new string UrlPressed {
			get { return base.UrlPressed; }
			set { base.UrlPressed = value; }
		}
		protected new string UrlSelected {
			get { return base.UrlSelected; }
			set { base.UrlSelected = value; }
		}
	}
}
