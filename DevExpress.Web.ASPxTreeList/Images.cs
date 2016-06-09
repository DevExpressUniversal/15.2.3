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
using DevExpress.Web;
using DevExpress.Web.Internal;
using DevExpress.Web.ASPxTreeList.Localization;
using DevExpress.Web.Internal.InternalCheckBox;
namespace DevExpress.Web.ASPxTreeList {
	public class TreeListImages : ImagesBase {
		internal const string
			CollapsedButtonName = "CollapsedButton",
			CollapsedButtonRtlName = "CollapsedButtonRtl",
			ExpandedButtonName = "ExpandedButton",
			ExpandedButtonRtlName = "ExpandedButtonRtl",
			SortAscendingName = "SortAsc",
			SortDescendingName = "SortDesc",
			DragAndDropArrowDownName = "DragAndDropArrowDown",
			DragAndDropArrowUpName = "DragAndDropArrowUp",
			DragAndDropHideName = "DragAndDropHide",
			CustomizationWindowCloseName = "CustomizationWindowClose",
			PopupEditFormWindowCloseName = "PopupEditFormWindowCloseName",
			DragAndDropNodeName = "DragAndDropNode";
		public TreeListImages(ISkinOwner owner) 
			: base(owner) {
		}
		protected override void PopulateImageInfoList(List<ImageInfo> list) {
			base.PopulateImageInfoList(list);
			list.Add(new ImageInfo(CollapsedButtonName, ImageFlags.IsPng, 9, 10,
				delegate() { return ASPxTreeListLocalizer.GetString(ASPxTreeListStringId.Alt_Expand); }, CollapsedButtonName));
			list.Add(new ImageInfo(CollapsedButtonRtlName, ImageFlags.IsPng, 9, 10,
				delegate() { return ASPxTreeListLocalizer.GetString(ASPxTreeListStringId.Alt_Expand); }, CollapsedButtonRtlName));
			list.Add(new ImageInfo(ExpandedButtonName, ImageFlags.IsPng, 9, 10,
				delegate() { return ASPxTreeListLocalizer.GetString(ASPxTreeListStringId.Alt_Collapse); }, ExpandedButtonName));
			list.Add(new ImageInfo(ExpandedButtonRtlName, ImageFlags.IsPng, 9, 10,
				delegate() { return ASPxTreeListLocalizer.GetString(ASPxTreeListStringId.Alt_Collapse); }, ExpandedButtonRtlName));
			list.Add(new ImageInfo(SortAscendingName, ImageFlags.Empty, 
				delegate() { return ASPxTreeListLocalizer.GetString(ASPxTreeListStringId.Alt_SortedAscending); }, SortAscendingName));
			list.Add(new ImageInfo(SortDescendingName, ImageFlags.Empty, 
				delegate() { return ASPxTreeListLocalizer.GetString(ASPxTreeListStringId.Alt_SortedDescending); }, SortDescendingName));
			list.Add(new ImageInfo(DragAndDropArrowDownName, ImageFlags.Empty, "|", DragAndDropArrowDownName));
			list.Add(new ImageInfo(DragAndDropArrowUpName, ImageFlags.Empty, "|", DragAndDropArrowUpName));
			list.Add(new ImageInfo(DragAndDropHideName, ImageFlags.Empty, 
				delegate() { return ASPxTreeListLocalizer.GetString(ASPxTreeListStringId.Alt_DragAndDropHideColumnIcon); }, DragAndDropHideName));
			list.Add(new ImageInfo(CustomizationWindowCloseName, ImageFlags.HasNoResourceImage));
			list.Add(new ImageInfo(PopupEditFormWindowCloseName, ImageFlags.HasNoResourceImage));
			list.Add(new ImageInfo(DragAndDropNodeName, ImageFlags.Empty, ">", DragAndDropNodeName));
		}
		[
#if !SL
	DevExpressWebASPxTreeListLocalizedDescription("TreeListImagesCollapsedButton"),
#endif
		NotifyParentProperty(true), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public ImageProperties CollapsedButton { get { return GetImage(CollapsedButtonName); } }
		[
#if !SL
	DevExpressWebASPxTreeListLocalizedDescription("TreeListImagesCollapsedButtonRtl"),
#endif
		NotifyParentProperty(true), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public ImageProperties CollapsedButtonRtl { get { return GetImage(CollapsedButtonRtlName); } }
		[
#if !SL
	DevExpressWebASPxTreeListLocalizedDescription("TreeListImagesExpandedButton"),
#endif
		NotifyParentProperty(true), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public ImageProperties ExpandedButton { get { return GetImage(ExpandedButtonName); } }
		[
#if !SL
	DevExpressWebASPxTreeListLocalizedDescription("TreeListImagesExpandedButtonRtl"),
#endif
		NotifyParentProperty(true), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public ImageProperties ExpandedButtonRtl { get { return GetImage(ExpandedButtonRtlName); } }
		[
#if !SL
	DevExpressWebASPxTreeListLocalizedDescription("TreeListImagesSortAscending"),
#endif
		NotifyParentProperty(true), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public ImageProperties SortAscending { get { return GetImage(SortAscendingName); } }
		[
#if !SL
	DevExpressWebASPxTreeListLocalizedDescription("TreeListImagesSortDescending"),
#endif
		NotifyParentProperty(true), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public ImageProperties SortDescending { get { return GetImage(SortDescendingName); } }
		[
#if !SL
	DevExpressWebASPxTreeListLocalizedDescription("TreeListImagesDragAndDropArrowDown"),
#endif
		NotifyParentProperty(true), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public ImageProperties DragAndDropArrowDown { get { return GetImage(DragAndDropArrowDownName); } }
		[
#if !SL
	DevExpressWebASPxTreeListLocalizedDescription("TreeListImagesDragAndDropArrowUp"),
#endif
		NotifyParentProperty(true), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public ImageProperties DragAndDropArrowUp { get { return GetImage(DragAndDropArrowUpName); } }
		[
#if !SL
	DevExpressWebASPxTreeListLocalizedDescription("TreeListImagesDragAndDropHide"),
#endif
		NotifyParentProperty(true), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public ImageProperties DragAndDropHide { get { return GetImage(DragAndDropHideName); } }
		[
#if !SL
	DevExpressWebASPxTreeListLocalizedDescription("TreeListImagesCustomizationWindowClose"),
#endif
		NotifyParentProperty(true), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public ImageProperties CustomizationWindowClose { get { return GetImage(CustomizationWindowCloseName); } }
		[
#if !SL
	DevExpressWebASPxTreeListLocalizedDescription("TreeListImagesPopupEditFormWindowClose"),
#endif
		NotifyParentProperty(true), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public ImageProperties PopupEditFormWindowClose { get { return GetImage(PopupEditFormWindowCloseName); } }
		[
#if !SL
	DevExpressWebASPxTreeListLocalizedDescription("TreeListImagesDragAndDropNode"),
#endif
		NotifyParentProperty(true), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public ImageProperties DragAndDropNode { get { return GetImage(DragAndDropNodeName); } }
		protected override Type GetResourceType() {
			return typeof(ASPxTreeList);
		}
		protected override string GetResourceImagePath() {
			return ASPxTreeList.ResourceImagesPath;
		}
		protected override string GetDesignTimeResourceSpriteImagePath() {
			return ASPxTreeList.ResourceImagesPath + GetDesignTimeResourceSpriteImageDefaultName();
		}
		protected override string GetResourceSpriteCssPath() {
			return ASPxTreeList.SpriteCssName;
		}
	}
}
