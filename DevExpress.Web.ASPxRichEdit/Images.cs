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
using System.Linq;
using System.Text;
using DevExpress.Web.Internal;
using DevExpress.Web;
using System.ComponentModel;
using System.Web.UI;
namespace DevExpress.Web.ASPxRichEdit {
	public class RichEditImages : ImagesBase {
		protected internal const string LoadingPanelOnStatusBarName = "reLoadingOnStatusBar";
		RichEditIconImages iconImages;
		[Category("Images"), AutoFormatEnable, NotifyParentProperty(true), DefaultValue(MenuIconSetType.NotSet)]
		public MenuIconSetType MenuIconSet {
			get { return IconImages.MenuIconSet; }
			set {
				IconImages.MenuIconSet = value;
				Changed();
			}
		}
		protected RichEditIconImages IconImages {
			get {
				if(iconImages == null)
					iconImages = new RichEditIconImages((ISkinOwner)Owner);
				return iconImages;
			}
		}
		public RichEditImages(ISkinOwner owner)
			: base(owner) {
		}
		public override string ToString() {
			return string.Empty;
		}
		public override void CopyFrom(ImagesBase source) {
			base.CopyFrom(source);
			if(source is RichEditImages)
				MenuIconSet = ((RichEditImages)source).MenuIconSet;
		}
		public override void Reset() {
			base.Reset();
			MenuIconSet = MenuIconSetType.NotSet;
		}
		public override ImageProperties GetImageProperties(Page page, string imageName, bool encode) {
			if(InfoIndex.ContainsKey(imageName))
				return base.GetImageProperties(page, imageName, encode);
			return IconImages.GetImageProperties(page, imageName, encode);
		}
		[Category("Images"), PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable,
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true)]
		public ImageProperties LoadingPanelOnStatusBar { get { return GetImage(LoadingPanelOnStatusBarName); } }
		protected override Type GetResourceType() {
			return typeof(ASPxRichEdit);
		}
		protected override string GetResourceImagePath() {
			return ASPxRichEdit.ImagesResourcePath;
		}
		protected override string GetDesignTimeResourceSpriteImagePath() {
			return ASPxRichEdit.ImagesResourcePath + GetDesignTimeResourceSpriteImageDefaultName();
		}
		protected override string GetResourceSpriteCssPath() {
			return ASPxRichEdit.SpriteCssResourceName;
		}
		protected internal void RegisterIconSpriteCssFile(Page page) {
			IconImages.RegisterIconSpriteCssFile(page);
		}
		protected override bool KeepDefaultSizes {
			get { return true; }
		}
		protected internal ImageProperties GetLoadingPanelImageProperties() {
			ImageProperties properties = new ImageProperties();
			properties.CopyFrom(GetDefaultLoadingImageProperties());
			properties.CopyFrom(LoadingPanelOnStatusBar);
			return properties;
		}
		protected override void PopulateImageInfoList(List<ImageInfo> list) {
			base.PopulateImageInfoList(list);
			list.Add(new ImageInfo(LoadingPanelOnStatusBarName));
		}
		protected override IStateManager[] GetStateManagedObjects() {
			return ViewStateUtils.GetMergedStateManagedObjects(base.GetStateManagedObjects(),
				new IStateManager[] { IconImages });
		}
	}
	public class RichEditIconImages : ImagesBase {
		internal readonly static Dictionary<MenuIconSetType, string> Categories = new Dictionary<MenuIconSetType, string>();
		internal readonly static Dictionary<MenuIconSetType, string> SpriteCssResourceNames = new Dictionary<MenuIconSetType, string>();
		internal readonly static Dictionary<MenuIconSetType, string> SpriteImageResourceNames = new Dictionary<MenuIconSetType, string>();
		public const string IconSpriteImageName = "ISprite";
		public const string GrayScaleIconSpriteImageName = "GISprite";
		public const string GrayScaleWithWhiteHottrackIconSpriteImageName = "GWISprite";
		private const int DefaultSmallSize = 16;
		private const int DefaultLargeSize = 32;
		#region ImageNames
		public const string
			AlignBottomCenter = "AlignBottomCenter",
			AlignBottomLeft = "AlignBottomLeft",
			AlignBottomRight = "AlignBottomRight",
			AlignMiddleCenter = "AlignMiddleCenter",
			AlignMiddleLeft = "AlignMiddleLeft",
			AlignMiddleRight = "AlignMiddleRight",
			AlignTopCenter = "AlignTopCenter",
			AlignTopLeft = "AlignTopLeft",
			AlignTopRight = "AlignTopRight",
			Bookmark = "Bookmark",
			BorderTop = "BorderTop",
			BorderBottom = "BorderBottom",
			BorderLeft = "BorderLeft",
			BorderRight = "BorderRight",
			BorderInsideHorizontal = "BorderInsideHorizontal",
			BorderInsideVertical = "BorderInsideVertical",
			BordersAll = "BordersAll",
			BordersBox = "BordersBox",
			BordersCustom = "BordersCustom",
			BordersGrid = "BordersGrid",
			BordersNone = "BordersNone",
			Copy = "Copy",
			Cut = "Cut",
			Delete_Hyperlink = "Delete_Hyperlink",
			Font = "Font",
			Hyperlink = "Hyperlink",
			IndentDecrease = "IndentDecrease",
			IndentIncrease = "IndentIncrease",
			InsertTableCells = "InsertTableCells",
			InsertTableColumnsToTheLeft = "InsertTableColumnsToTheLeft",
			InsertTableColumnsToTheRight = "InsertTableColumnsToTheRight",
			InsertTableRowsAbove = "InsertTableRowsAbove",
			InsertTableRowsBelow = "InsertTableRowsBelow",
			LeftColumns = "LeftColumns",
			ListBullets = "ListBullets",
			MergeTableCells = "MergeTableCells",
			OneColumn = "OneColumn",
			Paragraph = "Paragraph",
			Paste = "Paste",
			RightColumns = "RightColumns",
			SelectAll = "SelectAll",
			SplitTableCells = "SplitTableCells",
			TableAutoFitContents = "TableAutoFitContents",
			TableAutoFitWindow = "TableAutoFitWindow",
			TableFixedColumnWidth = "TableFixedColumnWidth",
			TableProperties = "TableProperties",
			ThreeColumns = "ThreeColumns",
			TwoColumns = "TwoColumns",
			RestartNumbering = "RestartNumberingList",
			ContinueNumbering = "ContinueNumberingList",
			UpdateField = "UpdateField",
			ToggleFieldCodes = "ToggleFieldCodes";
		#endregion
		static RichEditIconImages() {
			Categories.Add(MenuIconSetType.NotSet, "Icons");
			Categories.Add(MenuIconSetType.Colored, "Icons");
			Categories.Add(MenuIconSetType.ColoredLight, "Icons");
			Categories.Add(MenuIconSetType.GrayScaled, "GIcons");
			Categories.Add(MenuIconSetType.GrayScaledWithWhiteHottrack, "GWIcons");
			SpriteCssResourceNames.Add(MenuIconSetType.NotSet, ASPxRichEdit.IconSpriteCssResourceName);
			SpriteCssResourceNames.Add(MenuIconSetType.Colored, ASPxRichEdit.IconSpriteCssResourceName);
			SpriteCssResourceNames.Add(MenuIconSetType.GrayScaled, ASPxRichEdit.GrayScaleIconSpriteCssResourceName);
			SpriteCssResourceNames.Add(MenuIconSetType.GrayScaledWithWhiteHottrack, ASPxRichEdit.GrayScaleWithWhiteHottrackIconSpriteCssResourceName);
			SpriteImageResourceNames.Add(MenuIconSetType.NotSet, ASPxRichEdit.IconSpriteImageName);
			SpriteImageResourceNames.Add(MenuIconSetType.Colored, ASPxRichEdit.IconSpriteImageName);
			SpriteImageResourceNames.Add(MenuIconSetType.GrayScaled, ASPxRichEdit.GrayScaleIconSpriteImageName);
			SpriteImageResourceNames.Add(MenuIconSetType.GrayScaledWithWhiteHottrack, ASPxRichEdit.GrayScaleWithWhiteHottrackIconSpriteImageName);
		}
		public RichEditIconImages(ISkinOwner owner)
			: base(owner) {
		}
		public MenuIconSetType MenuIconSet {
			get { return (MenuIconSetType)GetEnumProperty("MenuIconSet", MenuIconSetType.NotSet); }
			set { SetEnumProperty("MenuIconSet", MenuIconSetType.NotSet, value); }
		}
		protected override Type GetResourceType() {
			return typeof(ASPxRichEdit);
		}
		protected override string GetResourceImagePath() {
			return ASPxRichEdit.ImagesResourcePath;
		}
		protected override string GetDesignTimeResourceSpriteImagePath() {
			return SpriteImageResourceNames[MenuIconSet];
		}
		protected override string GetResourceSpriteCssPath() {
			return SpriteCssResourceNames[MenuIconSet];
		}
		protected override string GetImageCategory() {
			return Categories[MenuIconSet];
		}
		protected internal void RegisterIconSpriteCssFile(Page page) {
			base.RegisterDefaultSpriteCssFile(page);
		}
		protected override string GetCssPostFix() {
			return string.Empty;
		}
		protected override bool KeepDefaultSizes {
			get { return true; }
		}
		protected override void PopulateImageInfoList(List<ImageInfo> list) {
			base.PopulateImageInfoList(list);
			ImageFlags imageFlags = ImageFlags.IsPng;
			#region InitializeImages
			list.Add(new ImageInfo(AlignBottomCenter, imageFlags, DefaultSmallSize, AlignBottomCenter));
			list.Add(new ImageInfo(AlignBottomLeft, imageFlags, DefaultSmallSize, AlignBottomLeft));
			list.Add(new ImageInfo(AlignBottomRight, imageFlags, DefaultSmallSize, AlignBottomRight));
			list.Add(new ImageInfo(AlignMiddleCenter, imageFlags, DefaultSmallSize, AlignMiddleCenter));
			list.Add(new ImageInfo(AlignMiddleLeft, imageFlags, DefaultSmallSize, AlignMiddleLeft));
			list.Add(new ImageInfo(AlignMiddleRight, imageFlags, DefaultSmallSize, AlignMiddleRight));
			list.Add(new ImageInfo(AlignTopCenter, imageFlags, DefaultSmallSize, AlignTopCenter));
			list.Add(new ImageInfo(AlignTopLeft, imageFlags, DefaultSmallSize, AlignTopLeft));
			list.Add(new ImageInfo(AlignTopRight, imageFlags, DefaultSmallSize, AlignTopRight));
			list.Add(new ImageInfo(Bookmark, imageFlags, DefaultSmallSize, Bookmark));
			list.Add(new ImageInfo(BorderTop, imageFlags, DefaultSmallSize, BorderTop));
			list.Add(new ImageInfo(BorderBottom, imageFlags, DefaultSmallSize, BorderBottom));
			list.Add(new ImageInfo(BorderLeft, imageFlags, DefaultSmallSize, BorderLeft));
			list.Add(new ImageInfo(BorderRight, imageFlags, DefaultSmallSize, BorderRight));
			list.Add(new ImageInfo(BorderInsideHorizontal, imageFlags, DefaultSmallSize, BorderInsideHorizontal));
			list.Add(new ImageInfo(BorderInsideVertical, imageFlags, DefaultSmallSize, BorderInsideVertical));
			list.Add(new ImageInfo(BordersAll, imageFlags, DefaultLargeSize, BordersAll));
			list.Add(new ImageInfo(BordersBox, imageFlags, DefaultLargeSize, BordersBox));
			list.Add(new ImageInfo(BordersCustom, imageFlags, DefaultLargeSize, BordersCustom));
			list.Add(new ImageInfo(BordersGrid, imageFlags, DefaultLargeSize, BordersGrid));
			list.Add(new ImageInfo(BordersNone, imageFlags, DefaultLargeSize, BordersNone));
			list.Add(new ImageInfo(Copy, imageFlags, DefaultSmallSize, Copy));
			list.Add(new ImageInfo(Cut, imageFlags, DefaultSmallSize, Cut));
			list.Add(new ImageInfo(Delete_Hyperlink, imageFlags, DefaultSmallSize, Delete_Hyperlink));
			list.Add(new ImageInfo(Font, imageFlags, DefaultSmallSize, Font));
			list.Add(new ImageInfo(Hyperlink, imageFlags, DefaultSmallSize, Hyperlink));
			list.Add(new ImageInfo(IndentDecrease, imageFlags, DefaultSmallSize, IndentDecrease));
			list.Add(new ImageInfo(IndentIncrease, imageFlags, DefaultSmallSize, IndentIncrease));
			list.Add(new ImageInfo(InsertTableCells, imageFlags, DefaultSmallSize, InsertTableCells));
			list.Add(new ImageInfo(InsertTableColumnsToTheLeft, imageFlags, DefaultSmallSize, InsertTableColumnsToTheLeft));
			list.Add(new ImageInfo(InsertTableColumnsToTheRight, imageFlags, DefaultSmallSize, InsertTableColumnsToTheRight));
			list.Add(new ImageInfo(InsertTableRowsAbove, imageFlags, DefaultSmallSize, InsertTableRowsAbove));
			list.Add(new ImageInfo(InsertTableRowsBelow, imageFlags, DefaultSmallSize, InsertTableRowsBelow));
			list.Add(new ImageInfo(LeftColumns, imageFlags, DefaultSmallSize, LeftColumns));
			list.Add(new ImageInfo(ListBullets, imageFlags, DefaultSmallSize, ListBullets));
			list.Add(new ImageInfo(MergeTableCells, imageFlags, DefaultSmallSize, MergeTableCells));
			list.Add(new ImageInfo(OneColumn, imageFlags, DefaultSmallSize, OneColumn));
			list.Add(new ImageInfo(Paragraph, imageFlags, DefaultSmallSize, Paragraph));
			list.Add(new ImageInfo(Paste, imageFlags, DefaultSmallSize, Paste));
			list.Add(new ImageInfo(RightColumns, imageFlags, DefaultSmallSize, RightColumns));
			list.Add(new ImageInfo(SelectAll, imageFlags, DefaultSmallSize, SelectAll));
			list.Add(new ImageInfo(SplitTableCells, imageFlags, DefaultSmallSize, SplitTableCells));
			list.Add(new ImageInfo(TableAutoFitContents, imageFlags, DefaultSmallSize, TableAutoFitContents));
			list.Add(new ImageInfo(TableAutoFitWindow, imageFlags, DefaultSmallSize, TableAutoFitWindow));
			list.Add(new ImageInfo(TableFixedColumnWidth, imageFlags, DefaultSmallSize, TableFixedColumnWidth));
			list.Add(new ImageInfo(TableProperties, imageFlags, DefaultSmallSize, TableProperties));
			list.Add(new ImageInfo(ThreeColumns, imageFlags, DefaultSmallSize, ThreeColumns));
			list.Add(new ImageInfo(TwoColumns, imageFlags, DefaultSmallSize, TwoColumns));
			list.Add(new ImageInfo(RestartNumbering, imageFlags, DefaultSmallSize, RestartNumbering));
			list.Add(new ImageInfo(ContinueNumbering, imageFlags, DefaultSmallSize, ContinueNumbering));
			list.Add(new ImageInfo(UpdateField, imageFlags, DefaultSmallSize, UpdateField));
			list.Add(new ImageInfo(ToggleFieldCodes, imageFlags, DefaultSmallSize, ToggleFieldCodes));
			#endregion
		}
	}
	public class RichEditRulerImages : ImagesBase {
		public const string LeftIdentDragHandleImageName = "LeftIdentDragHandle";
		public const string RightIdentDragHandleImageName = "RightIdentDragHandle";
		public const string FirstLineIdentDragHandleImageName = "FirstLineIdentDragHandle";
		public const string RightTabDragHandleImageName = "RightTabDragHandle";
		public const string LeftTabDragHandleImageName = "LeftTabDragHandle";
		public const string DecimalTabDragHandleImageName = "DecimalTabDragHandle";
		public const string CenterTabDragHandleImageName = "CenterTabDragHandle";
		public RichEditRulerImages(ISkinOwner skinOwner)
			: base(skinOwner) {
		}
		protected override Type GetResourceType() {
			return typeof(ASPxRichEdit);
		}
		[
#if !SL
	DevExpressWebASPxRichEditLocalizedDescription("RichEditRulerImagesDecimalTabDragHandle"),
#endif
		NotifyParentProperty(true), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public ImageProperties DecimalTabDragHandle {
			get { return (ImageProperties)GetImageBase(DecimalTabDragHandleImageName); }
		}
		[
#if !SL
	DevExpressWebASPxRichEditLocalizedDescription("RichEditRulerImagesLeftTabDragHandle"),
#endif
		NotifyParentProperty(true), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public ImageProperties LeftTabDragHandle {
			get { return (ImageProperties)GetImageBase(LeftTabDragHandleImageName); }
		}
		[
#if !SL
	DevExpressWebASPxRichEditLocalizedDescription("RichEditRulerImagesRightTabDragHandle"),
#endif
		NotifyParentProperty(true), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public ImageProperties RightTabDragHandle {
			get { return (ImageProperties)GetImageBase(RightTabDragHandleImageName); }
		}
		[
#if !SL
	DevExpressWebASPxRichEditLocalizedDescription("RichEditRulerImagesCenterTabDragHandle"),
#endif
		NotifyParentProperty(true), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public ImageProperties CenterTabDragHandle {
			get { return (ImageProperties)GetImageBase(CenterTabDragHandleImageName); }
		}
		[
#if !SL
	DevExpressWebASPxRichEditLocalizedDescription("RichEditRulerImagesLeftIdentDragHandle"),
#endif
		NotifyParentProperty(true), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public ImageProperties LeftIdentDragHandle {
			get { return (ImageProperties)GetImageBase(LeftIdentDragHandleImageName); }
		}
		[
#if !SL
	DevExpressWebASPxRichEditLocalizedDescription("RichEditRulerImagesRightIdentDragHandle"),
#endif
		NotifyParentProperty(true), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public ImageProperties RightIdentDragHandle {
			get { return (ImageProperties)GetImageBase(RightIdentDragHandleImageName); }
		}
		[
#if !SL
	DevExpressWebASPxRichEditLocalizedDescription("RichEditRulerImagesFirstLineIdentDragHandle"),
#endif
		NotifyParentProperty(true), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public ImageProperties FirstLineIdentDragHandle {
			get { return (ImageProperties)GetImageBase(FirstLineIdentDragHandleImageName); }
		}
		protected override void PopulateImageInfoList(List<ImageInfo> list) {
			base.PopulateImageInfoList(list);
			AddImageInfo(list, LeftIdentDragHandleImageName);
			AddImageInfo(list, RightIdentDragHandleImageName);
			AddImageInfo(list, FirstLineIdentDragHandleImageName);
			AddImageInfo(list, DecimalTabDragHandleImageName);
			AddImageInfo(list, LeftTabDragHandleImageName);
			AddImageInfo(list, RightTabDragHandleImageName);
			AddImageInfo(list, CenterTabDragHandleImageName);
		}
		protected void AddImageInfo(List<ImageInfo> list, string imageName) {
			list.Add(new ImageInfo(imageName, ImageFlags.HasDisabledState, string.Empty, typeof(ImageProperties), imageName));
		}
	}
}
