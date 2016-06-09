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
using DevExpress.Web;
using DevExpress.Web.Internal;
using DevExpress.Web.ASPxPivotGrid.Data;
using System.Collections.Generic;
using System.Web.UI;
using System.ComponentModel;
using DevExpress.XtraPivotGrid;
using DevExpress.XtraPivotGrid.Data;
using DevExpress.Utils.Serializing.Helpers;
using DevExpress.Utils.Serializing;
using DevExpress.XtraPivotGrid.Localization;
namespace DevExpress.Web.ASPxPivotGrid {
	public class PivotGridImages : ImagesBase {
		internal const string ElementName_ArrowDragDownImage = "IADD";
		internal const string ElementName_ArrowDragUpImage = "IADU";
		internal const string ElementName_ArrowDragRightImage = "IADR";
		internal const string ElementName_ArrowDragLeftImage = "IADL";
		internal const string ElementName_DragHideFieldImage = "IDHF";
		internal const string ElementName_GroupSeparatorImage = "IGS";
		internal const string FieldValueCollapsedName = "pgCollapsedButton";
		internal const string FieldValueExpandedName = "pgExpandedButton";
		internal const string HeaderSortDownName = "pgSortDownButton";
		internal const string HeaderSortUpName = "pgSortUpButton";
		internal const string FilterWindowSizeGripName = "pgFilterResizer";
		internal const string HeaderFilterName = "pgFilterButton";
		internal const string HeaderActiveFilterName = "pgFilterButtonActive";
		internal const string CustomizationFieldsCloseName = "pgCustomizationFormCloseButton";
		internal const string CustomizationFieldsBackgroundName = "pgCustomizationFormBackground";
		internal const string DragArrowDownName = "pgDragArrowDown";
		internal const string DragArrowUpName = "pgDragArrowUp";
		internal const string DragArrowRightName = "pgDragArrowRight";
		internal const string DragArrowLeftName = "pgDragArrowLeft";
		internal const string DragHideFieldName = "pgDragHideField";
		internal const string DataHeadersPopupName = "pgDataHeaders";
		internal const string GroupSeparatorName = "pgGroupSeparator";
		internal const string SortByColumnName = "pgSortByColumn";
		internal const string PrefilterButtonName = "pgPrefilterButton";
		internal const string TreeViewNodeLoadingPanelImageName = "pgTreeViewNodeLoadingPanelImage";
		public PivotGridImages(ASPxPivotGrid owner)
			: base(owner) {
		}
		ASPxPivotGrid PivotGrid { get { return (ASPxPivotGrid)Owner; } }
		protected override void PopulateImageInfoList(List<ImageInfo> list) {
			list.Add(new ImageInfo(FieldValueCollapsedName, ImageFlags.Empty, 
				delegate() { return PivotGridLocalizer.GetString(PivotGridStringId.Alt_Expand); },
				typeof(PivotGridImageProperties), FieldValueCollapsedName));
			list.Add(new ImageInfo(FieldValueExpandedName, ImageFlags.Empty, 
				delegate() { return PivotGridLocalizer.GetString(PivotGridStringId.Alt_Collapse); },
				typeof(PivotGridImageProperties), FieldValueExpandedName));
			list.Add(new ImageInfo(HeaderSortDownName, ImageFlags.Empty, 
				delegate() { return PivotGridLocalizer.GetString(PivotGridStringId.Alt_SortedDescending); },
				typeof(PivotGridImageProperties), HeaderSortDownName));
			list.Add(new ImageInfo(HeaderSortUpName, ImageFlags.Empty, 
				delegate() { return PivotGridLocalizer.GetString(PivotGridStringId.Alt_SortedAscending); },
				typeof(PivotGridImageProperties), HeaderSortUpName));
			list.Add(new ImageInfo(FilterWindowSizeGripName, ImageFlags.Empty, 
				delegate() { return PivotGridLocalizer.GetString(PivotGridStringId.Alt_FilterWindowSizeGrip); },
				typeof(PivotGridImageProperties), FilterWindowSizeGripName));
			list.Add(new ImageInfo(HeaderFilterName, ImageFlags.Empty, 
				delegate() { return PivotGridLocalizer.GetString(PivotGridStringId.Alt_FilterButton); },
				typeof(PivotGridImageProperties), HeaderFilterName));
			list.Add(new ImageInfo(HeaderActiveFilterName, ImageFlags.Empty, 
				delegate() { return PivotGridLocalizer.GetString(PivotGridStringId.Alt_FilterButtonActive); },
				typeof(PivotGridImageProperties), HeaderActiveFilterName));
			list.Add(new ImageInfo(CustomizationFieldsCloseName, ImageFlags.Empty, "",
				typeof(PivotGridImageProperties), CustomizationFieldsCloseName));
			list.Add(new ImageInfo(CustomizationFieldsBackgroundName, typeof(PivotGridImageProperties)));
			list.Add(new ImageInfo(DragArrowDownName, ImageFlags.Empty, "|",
				typeof(PivotGridImageProperties), DragArrowDownName));
			list.Add(new ImageInfo(DragArrowUpName, ImageFlags.Empty, "|",
				typeof(PivotGridImageProperties), DragArrowUpName));
			list.Add(new ImageInfo(DragArrowRightName, ImageFlags.Empty, "|",
				typeof(PivotGridImageProperties), DragArrowRightName));
			list.Add(new ImageInfo(DragArrowLeftName, ImageFlags.Empty, "|",
				typeof(PivotGridImageProperties), DragArrowLeftName));
			list.Add(new ImageInfo(DragHideFieldName, ImageFlags.Empty, 
				delegate() { return PivotGridLocalizer.GetString(PivotGridStringId.Alt_DragHideField); },
				typeof(PivotGridImageProperties), DragHideFieldName));
			list.Add(new ImageInfo(LoadingPanelImageName, typeof(PivotGridImageProperties)));
			list.Add(new ImageInfo(DataHeadersPopupName, ImageFlags.Empty, "",
				typeof(PivotGridImageProperties), DataHeadersPopupName));
			list.Add(new ImageInfo(GroupSeparatorName, ImageFlags.Empty, "-",
				typeof(PivotGridImageProperties), GroupSeparatorName));
			list.Add(new ImageInfo(SortByColumnName, ImageFlags.Empty, "*",
				typeof(PivotGridImageProperties), SortByColumnName));
			list.Add(new ImageInfo(PrefilterButtonName, ImageFlags.IsPng, 13, 13, 
				delegate() { return PivotGridLocalizer.GetString(PivotGridStringId.Alt_FilterButton); },
				typeof(PivotGridImageProperties), PrefilterButtonName));
			list.Add(new ImageInfo(TreeViewNodeLoadingPanelImageName, ImageFlags.Empty | ImageFlags.HasNoResourceImage, 
				typeof(PivotGridImageProperties), TreeViewNodeLoadingPanelImageName));
		}
		protected override Type GetResourceType() {
			return typeof(ASPxPivotGrid);
		}
		protected override string GetResourceImagePath() {
			return PivotGridWebData.PivotGridImagesResourcePath;
		}
		protected override string GetDesignTimeResourceSpriteImagePath() {
			return PivotGridWebData.PivotGridImagesResourcePath + GetDesignTimeResourceSpriteImageDefaultName();
		}
		protected override string GetResourceSpriteCssPath() {
			return PivotGridWebData.PivotGridSpriteCssResourceName;
		}
		protected new PivotGridImageProperties GetImage(string imageName) {
			return (PivotGridImageProperties)base.GetImage(imageName);
		}
		[
#if !SL
	DevExpressWebASPxPivotGridLocalizedDescription("PivotGridImagesFieldValueCollapsed"),
#endif
		Category("Images"), PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true)]
		[XtraSerializableProperty(XtraSerializationVisibility.Content), AutoFormatEnable()]
		public PivotGridImageProperties FieldValueCollapsed { get { return GetImage(FieldValueCollapsedName); } }
		[
#if !SL
	DevExpressWebASPxPivotGridLocalizedDescription("PivotGridImagesFieldValueExpanded"),
#endif
		Category("Images"), PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true)]
		[XtraSerializableProperty(XtraSerializationVisibility.Content), AutoFormatEnable()]
		public PivotGridImageProperties FieldValueExpanded { get { return GetImage(FieldValueExpandedName); } }
		[
#if !SL
	DevExpressWebASPxPivotGridLocalizedDescription("PivotGridImagesHeaderFilter"),
#endif
		Category("Images"), PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true)]
		[XtraSerializableProperty(XtraSerializationVisibility.Content), AutoFormatEnable()]
		public PivotGridImageProperties HeaderFilter { get { return GetImage(HeaderFilterName); } }
		[
#if !SL
	DevExpressWebASPxPivotGridLocalizedDescription("PivotGridImagesHeaderActiveFilter"),
#endif
		Category("Images"), PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true)]
		[XtraSerializableProperty(XtraSerializationVisibility.Content), AutoFormatEnable()]
		public PivotGridImageProperties HeaderActiveFilter { get { return GetImage(HeaderActiveFilterName); } }
		[
#if !SL
	DevExpressWebASPxPivotGridLocalizedDescription("PivotGridImagesHeaderSortDown"),
#endif
		Category("Images"), PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true)]
		[XtraSerializableProperty(XtraSerializationVisibility.Content), AutoFormatEnable()]
		public PivotGridImageProperties HeaderSortDown { get { return GetImage(HeaderSortDownName); } }
		[
#if !SL
	DevExpressWebASPxPivotGridLocalizedDescription("PivotGridImagesHeaderSortUp"),
#endif
		Category("Images"), PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true)]
		[XtraSerializableProperty(XtraSerializationVisibility.Content), AutoFormatEnable()]
		public PivotGridImageProperties HeaderSortUp { get { return GetImage(HeaderSortUpName); } }
		[
#if !SL
	DevExpressWebASPxPivotGridLocalizedDescription("PivotGridImagesFilterWindowSizeGrip"),
#endif
		Category("Images"), PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true)]
		[XtraSerializableProperty(XtraSerializationVisibility.Content), AutoFormatEnable()]
		public PivotGridImageProperties FilterWindowSizeGrip { get { return GetImage(FilterWindowSizeGripName); } }
		[
#if !SL
	DevExpressWebASPxPivotGridLocalizedDescription("PivotGridImagesCustomizationFieldsClose"),
#endif
		Category("Images"), PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true)]
		[XtraSerializableProperty(XtraSerializationVisibility.Content), AutoFormatEnable()]
		public PivotGridImageProperties CustomizationFieldsClose { get { return GetImage(CustomizationFieldsCloseName); } }
		[
#if !SL
	DevExpressWebASPxPivotGridLocalizedDescription("PivotGridImagesCustomizationFieldsBackground"),
#endif
		Category("Images"), PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true)]
		[XtraSerializableProperty(XtraSerializationVisibility.Content), AutoFormatEnable()]
		public PivotGridImageProperties CustomizationFieldsBackground { get { return GetImage(CustomizationFieldsBackgroundName); } }
		[
#if !SL
	DevExpressWebASPxPivotGridLocalizedDescription("PivotGridImagesDragArrowDown"),
#endif
		Category("Images"), PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true)]
		[XtraSerializableProperty(XtraSerializationVisibility.Content), AutoFormatEnable()]
		public PivotGridImageProperties DragArrowDown { get { return GetImage(DragArrowDownName); } }
		[
#if !SL
	DevExpressWebASPxPivotGridLocalizedDescription("PivotGridImagesDragArrowUp"),
#endif
		Category("Images"), PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true)]
		[XtraSerializableProperty(XtraSerializationVisibility.Content), AutoFormatEnable()]
		public PivotGridImageProperties DragArrowUp { get { return GetImage(DragArrowUpName); } }
		[
#if !SL
	DevExpressWebASPxPivotGridLocalizedDescription("PivotGridImagesDragArrowRight"),
#endif
		Category("Images"), PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true)]
		[XtraSerializableProperty(XtraSerializationVisibility.Content), AutoFormatEnable()]
		public PivotGridImageProperties DragArrowRight { get { return GetImage(DragArrowRightName); } }
		[
#if !SL
	DevExpressWebASPxPivotGridLocalizedDescription("PivotGridImagesDragArrowLeft"),
#endif
		Category("Images"), PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true)]
		[XtraSerializableProperty(XtraSerializationVisibility.Content), AutoFormatEnable()]
		public PivotGridImageProperties DragArrowLeft { get { return GetImage(DragArrowLeftName); } }
		[
#if !SL
	DevExpressWebASPxPivotGridLocalizedDescription("PivotGridImagesDragHideField"),
#endif
		Category("Images"), PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true)]
		[XtraSerializableProperty(XtraSerializationVisibility.Content), AutoFormatEnable()]
		public PivotGridImageProperties DragHideField { get { return GetImage(DragHideFieldName); } }
		[
#if !SL
	DevExpressWebASPxPivotGridLocalizedDescription("PivotGridImagesDataHeadersPopup"),
#endif
		Category("Images"), PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true)]
		[XtraSerializableProperty(XtraSerializationVisibility.Content), AutoFormatEnable()]
		public PivotGridImageProperties DataHeadersPopup { get { return GetImage(DataHeadersPopupName); } }
		[
#if !SL
	DevExpressWebASPxPivotGridLocalizedDescription("PivotGridImagesGroupSeparator"),
#endif
		Category("Images"), PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true)]
		[XtraSerializableProperty(XtraSerializationVisibility.Content), AutoFormatEnable()]
		public PivotGridImageProperties GroupSeparator { get { return GetImage(GroupSeparatorName); } }
		[
#if !SL
	DevExpressWebASPxPivotGridLocalizedDescription("PivotGridImagesSortByColumn"),
#endif
		Category("Images"), PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true)]
		[XtraSerializableProperty(XtraSerializationVisibility.Content), AutoFormatEnable()]
		public PivotGridImageProperties SortByColumn { get { return GetImage(SortByColumnName); } }
		[
#if !SL
	DevExpressWebASPxPivotGridLocalizedDescription("PivotGridImagesPrefilterButton"),
#endif
		Category("Images"), PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true)]
		[XtraSerializableProperty(XtraSerializationVisibility.Content), AutoFormatEnable()]
		public PivotGridImageProperties PrefilterButton { get { return GetImage(PrefilterButtonName); } }
		[
#if !SL
	DevExpressWebASPxPivotGridLocalizedDescription("PivotGridImagesTreeViewNodeLoadingPanel"),
#endif
		Category("Images"), PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true)]
		[XtraSerializableProperty(XtraSerializationVisibility.Content), AutoFormatEnable()]
		public PivotGridImageProperties TreeViewNodeLoadingPanel {
			get { return GetImage(TreeViewNodeLoadingPanelImageName); }
		}
	}
	public class PivotGridImageProperties : ImageProperties, IXtraSerializable2 {
		public PivotGridImageProperties() : base() { }
		public PivotGridImageProperties(IPropertiesOwner owner) : base(owner) { }
		public PivotGridImageProperties(string url) : base(url) { }
		#region IXtraSerializable2 Members
		void IXtraSerializable2.Deserialize(System.Collections.IList list) {
			StateManagerSerializeHelper.DeserializeObject(this, (IXtraPropertyCollection)list);
		}
		XtraPropertyInfo[] IXtraSerializable2.Serialize() {
			ReadOnlyViewState.SetDirty(true);
			return StateManagerSerializeHelper.SerializeObject(this);
		}
		#endregion
	}
	public class PivotKPIImages : ImagesBase {
		ASPxPivotGrid pivot;
		public PivotKPIImages(ASPxPivotGrid owner)
			: base(null) {
			this.pivot = owner;
		}
		protected ASPxPivotGrid PivotGrid { get { return pivot; } }
		protected override void PopulateImageInfoList(List<ImageInfo> list) {
			foreach(PivotKPIGraphic graphic in Enum.GetValues(typeof(PivotKPIGraphic))) {
				if(graphic == PivotKPIGraphic.None || graphic == PivotKPIGraphic.ServerDefined) continue;
				string name = graphic.ToString();
				list.Add(new ImageInfo(name + ".-1", ImageFlags.IsPng));
				list.Add(new ImageInfo(name + ".0", ImageFlags.IsPng));
				list.Add(new ImageInfo(name + ".1", ImageFlags.IsPng));
			}
		}		
		protected override Type GetResourceType() {
			return typeof(PivotGridWebData);
		}
		protected override string GetResourceImagePath() {
			return PivotGridWebData.PivotGridImagesResourcePath;
		}
		protected override string GetDesignTimeResourceSpriteImagePath() {
			return PivotGridWebData.PivotGridImagesResourcePath + GetDesignTimeResourceSpriteImageDefaultName();
		}
		public ImageProperties GetImage(PivotKPIGraphic graphic, PivotKPIType type, int state) {
			if(state != 0 && state != -1 && state != 1) throw new ArgumentException("state");
			if(graphic == PivotKPIGraphic.None || graphic == PivotKPIGraphic.ServerDefined) throw new ArgumentException("graphic");
			ImageProperties res = GetImageProperties(PivotGrid.Page, graphic.ToString() + "." + state.ToString());
			res.AlternateText = PivotGridData.GetKPITooltipText(type, state);
			return res;
		}				
	}
	public class PivotCustomizationFormImages : ImagesBase {
		internal const string FilterAreaHeadersName = "FLFilterAreaHeaders";
		internal const string ColumnAreaHeadersName = "FLColumnAreaHeaders";
		internal const string RowAreaHeadersName = "FLRowAreaHeaders";
		internal const string DataAreaHeadersName = "FLDataAreaHeaders";
		internal const string FieldListHeadersName = "FLFieldList";
		internal const string LayoutButtonName = "FLButton";
		internal const string StackedDefaultLayoutName = "FLStackedDefault";
		internal const string StackedSideBySideLayoutName = "FLStackedSideBySide";
		internal const string TopPanelOnlyLayoutName = "FLTopPanelOnly";
		internal const string BottomPanelOnly2by2LayoutName = "FLBottomPanelOnly2by2";
		internal const string BottomPanelOnly1by4LayoutName = "FLBottomPanelOnly1by4";
		public PivotCustomizationFormImages(ISkinOwner owner)
			:
			base(owner) {
		}
		protected override void PopulateImageInfoList(List<ImageInfo> list) {
			list.Add(new ImageInfo(FilterAreaHeadersName, ImageFlags.Empty,
				delegate() { return PivotGridLocalizer.GetString(PivotGridStringId.Alt_FilterAreaHeaders); },
				typeof(PivotGridImageProperties), FilterAreaHeadersName));
			list.Add(new ImageInfo(ColumnAreaHeadersName, ImageFlags.Empty,
				delegate() { return PivotGridLocalizer.GetString(PivotGridStringId.Alt_ColumnAreaHeaders); },
				typeof(PivotGridImageProperties), ColumnAreaHeadersName));
			list.Add(new ImageInfo(RowAreaHeadersName, ImageFlags.Empty,
				delegate() { return PivotGridLocalizer.GetString(PivotGridStringId.Alt_RowAreaHeaders); },
				typeof(PivotGridImageProperties), RowAreaHeadersName));
			list.Add(new ImageInfo(DataAreaHeadersName, ImageFlags.Empty,
				delegate() { return PivotGridLocalizer.GetString(PivotGridStringId.Alt_DataAreaHeaders); },
				typeof(PivotGridImageProperties), DataAreaHeadersName));
			list.Add(new ImageInfo(FieldListHeadersName, ImageFlags.Empty,
				delegate() { return PivotGridLocalizer.GetString(PivotGridStringId.Alt_FieldListHeaders); },
				typeof(PivotGridImageProperties), FieldListHeadersName));
			list.Add(new ImageInfo(LayoutButtonName, ImageFlags.Empty,
				delegate() { return PivotGridLocalizer.GetString(PivotGridStringId.Alt_LayoutButton); },
				typeof(PivotGridImageProperties), LayoutButtonName));
			list.Add(new ImageInfo(StackedDefaultLayoutName, ImageFlags.Empty,
				delegate() { return PivotGridLocalizer.GetString(PivotGridStringId.Alt_StackedDefaultLayout); },
				typeof(PivotGridImageProperties), StackedDefaultLayoutName));
			list.Add(new ImageInfo(StackedSideBySideLayoutName, ImageFlags.Empty,
				delegate() { return PivotGridLocalizer.GetString(PivotGridStringId.Alt_StackedSideBySideLayout); },
				typeof(PivotGridImageProperties), StackedSideBySideLayoutName));
			list.Add(new ImageInfo(TopPanelOnlyLayoutName, ImageFlags.Empty,
				delegate() { return PivotGridLocalizer.GetString(PivotGridStringId.Alt_TopPanelOnlyLayout); },
				typeof(PivotGridImageProperties), TopPanelOnlyLayoutName));
			list.Add(new ImageInfo(BottomPanelOnly2by2LayoutName, ImageFlags.Empty,
				delegate() { return PivotGridLocalizer.GetString(PivotGridStringId.Alt_BottomPanelOnly2by2Layout); },
				typeof(PivotGridImageProperties), BottomPanelOnly2by2LayoutName));
			list.Add(new ImageInfo(BottomPanelOnly1by4LayoutName, ImageFlags.Empty,
				delegate() { return PivotGridLocalizer.GetString(PivotGridStringId.Alt_BottomPanelOnly1by4Layout); },
				typeof(PivotGridImageProperties), BottomPanelOnly1by4LayoutName));
		}
		protected override Type GetResourceType() {
			return typeof(ASPxPivotCustomizationControl);
		}
		protected override string GetResourceImagePath() {
			return PivotGridWebData.PivotGridImagesResourcePath;
		}
		protected override string GetDesignTimeResourceSpriteImagePath() {
			return PivotGridWebData.PivotGridImagesResourcePath + GetDesignTimeResourceSpriteImageDefaultName();
		}
		protected override string GetResourceSpriteCssPath() {
			return PivotGridWebData.PivotGridSpriteCssResourceName;
		}
		protected new PivotGridImageProperties GetImage(string imageName) {
			return (PivotGridImageProperties)base.GetImage(imageName);
		}
		[
#if !SL
	DevExpressWebASPxPivotGridLocalizedDescription("PivotCustomizationFormImagesFilterAreaHeaders"),
#endif
		Category("Images"), PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true)]
		[XtraSerializableProperty(XtraSerializationVisibility.Content), AutoFormatEnable()]
		public PivotGridImageProperties FilterAreaHeaders { get { return GetImage(FilterAreaHeadersName); } }
		[
#if !SL
	DevExpressWebASPxPivotGridLocalizedDescription("PivotCustomizationFormImagesColumnAreaHeaders"),
#endif
		Category("Images"), PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true)]
		[XtraSerializableProperty(XtraSerializationVisibility.Content), AutoFormatEnable()]
		public PivotGridImageProperties ColumnAreaHeaders { get { return GetImage(ColumnAreaHeadersName); } }
		[
#if !SL
	DevExpressWebASPxPivotGridLocalizedDescription("PivotCustomizationFormImagesRowAreaHeaders"),
#endif
		Category("Images"), PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true)]
		[XtraSerializableProperty(XtraSerializationVisibility.Content), AutoFormatEnable()]
		public PivotGridImageProperties RowAreaHeaders { get { return GetImage(RowAreaHeadersName); } }
		[
#if !SL
	DevExpressWebASPxPivotGridLocalizedDescription("PivotCustomizationFormImagesDataAreaHeaders"),
#endif
		Category("Images"), PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true)]
		[XtraSerializableProperty(XtraSerializationVisibility.Content), AutoFormatEnable()]
		public PivotGridImageProperties DataAreaHeaders { get { return GetImage(DataAreaHeadersName); } }
		[
#if !SL
	DevExpressWebASPxPivotGridLocalizedDescription("PivotCustomizationFormImagesFieldListHeaders"),
#endif
		Category("Images"), PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true)]
		[XtraSerializableProperty(XtraSerializationVisibility.Content), AutoFormatEnable()]
		public PivotGridImageProperties FieldListHeaders { get { return GetImage(FieldListHeadersName); } }
		[
#if !SL
	DevExpressWebASPxPivotGridLocalizedDescription("PivotCustomizationFormImagesStackedDefaultLayout"),
#endif
		Category("Images"), PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true)]
		[XtraSerializableProperty(XtraSerializationVisibility.Content), AutoFormatEnable()]
		public PivotGridImageProperties StackedDefaultLayout { get { return GetImage(StackedDefaultLayoutName); } }
		[
#if !SL
	DevExpressWebASPxPivotGridLocalizedDescription("PivotCustomizationFormImagesStackedSideBySideLayout"),
#endif
		Category("Images"), PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true)]
		[XtraSerializableProperty(XtraSerializationVisibility.Content), AutoFormatEnable()]
		public PivotGridImageProperties StackedSideBySideLayout { get { return GetImage(StackedSideBySideLayoutName); } }
		[
#if !SL
	DevExpressWebASPxPivotGridLocalizedDescription("PivotCustomizationFormImagesTopPanelOnlyLayout"),
#endif
		Category("Images"), PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true)]
		[XtraSerializableProperty(XtraSerializationVisibility.Content), AutoFormatEnable()]
		public PivotGridImageProperties TopPanelOnlyLayout { get { return GetImage(TopPanelOnlyLayoutName); } }
		[
#if !SL
	DevExpressWebASPxPivotGridLocalizedDescription("PivotCustomizationFormImagesBottomPanelOnly2by2Layout"),
#endif
		Category("Images"), PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true)]
		[XtraSerializableProperty(XtraSerializationVisibility.Content), AutoFormatEnable()]
		public PivotGridImageProperties BottomPanelOnly2by2Layout { get { return GetImage(BottomPanelOnly2by2LayoutName); } }
		[
#if !SL
	DevExpressWebASPxPivotGridLocalizedDescription("PivotCustomizationFormImagesBottomPanelOnly1by4Layout"),
#endif
		Category("Images"), PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true)]
		[XtraSerializableProperty(XtraSerializationVisibility.Content), AutoFormatEnable()]
		public PivotGridImageProperties BottomPanelOnly1by4Layout { get { return GetImage(BottomPanelOnly1by4LayoutName); } }
	}
}
