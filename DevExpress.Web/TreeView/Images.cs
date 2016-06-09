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
using System.Text;
using DevExpress.Web.Internal;
using DevExpress.Web;
using DevExpress.Web.Localization;
using System.ComponentModel;
using System.Web.UI;
using System.Web.UI.WebControls;
using DevExpress.Web.Internal.InternalCheckBox;
namespace DevExpress.Web {
	public class TreeViewImages : ImagesBase {
		internal const string
			CollapseButtonImageName = "tvColBtn",
			ExpandButtonImageName = "tvExpBtn",
			CollapseButtonRtlImageName = CollapseButtonImageName + RtlImagePostfix,
			ExpandButtonRtlImageName = ExpandButtonImageName + RtlImagePostfix,
			LineImageName = "tvLine",
			ElbowImageName = "tvElbow",
			NodeImageName = "tvNode",
			NodeLoadingPanelImageName = "tvNodeLoading",
			RtlImagePostfix = "Rtl";
		public TreeViewImages(ASPxTreeView treeView)
			: this(treeView as ISkinOwner) {
		}
		public TreeViewImages(ISkinOwner skinOwner)
			: base(skinOwner) {
		}
		protected bool IsRightToLeft {
			get { return (Owner as ASPxTreeView).IsRightToLeft(); }
		}
		protected string GetEffectiveImageName(string defaultImageName) {
			return IsRightToLeft ? defaultImageName + RtlImagePostfix : defaultImageName;
		}
		protected internal string EffectiveElbowImageName {
			get { return GetEffectiveImageName(ElbowImageName); }
		}
		protected internal string EffectiveExpandButtonImageName {
			get { return GetEffectiveImageName(ExpandButtonImageName); }
		}
		protected internal string EffectiveCollapseButtonImageName {
			get { return GetEffectiveImageName(CollapseButtonImageName); }
		}
		protected override void PopulateImageInfoList(List<ImageInfo> list) {
			base.PopulateImageInfoList(list);
			list.Add(new ImageInfo(CollapseButtonImageName, ImageFlags.IsPng, 9, 9,
				ASPxperienceLocalizer.GetString(ASPxperienceStringId.TreeView_AltCollapse),
				CollapseButtonImageName));
			list.Add(new ImageInfo(ExpandButtonImageName, ImageFlags.IsPng, 9, 9,
				ASPxperienceLocalizer.GetString(ASPxperienceStringId.TreeView_AltExpand),
				ExpandButtonImageName));
			list.Add(new ImageInfo(CollapseButtonRtlImageName, ImageFlags.IsPng, 9, 9,
				ASPxperienceLocalizer.GetString(ASPxperienceStringId.TreeView_AltCollapse),
				CollapseButtonRtlImageName));
			list.Add(new ImageInfo(ExpandButtonRtlImageName, ImageFlags.IsPng, 9, 9,
				ASPxperienceLocalizer.GetString(ASPxperienceStringId.TreeView_AltExpand),
				ExpandButtonRtlImageName));
			list.Add(new ImageInfo(NodeLoadingPanelImageName, ImageFlags.Empty, 15, 15,
				ASPxperienceLocalizer.GetString(ASPxperienceStringId.TreeView_AltLoading), string.Empty));
			list.Add(new ImageInfo(NodeImageName, ImageFlags.HasNoResourceImage,
				typeof(ItemImageProperties)));
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("TreeViewImagesCollapseButton"),
#endif
		NotifyParentProperty(true), PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content), AutoFormatEnable]
		public ImageProperties CollapseButton { get { return GetImage(CollapseButtonImageName); } }
		[
#if !SL
	DevExpressWebLocalizedDescription("TreeViewImagesExpandButton"),
#endif
		NotifyParentProperty(true), PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content), AutoFormatEnable]
		public ImageProperties ExpandButton { get { return GetImage(ExpandButtonImageName); } }
		[
#if !SL
	DevExpressWebLocalizedDescription("TreeViewImagesCollapseButtonRtl"),
#endif
		NotifyParentProperty(true), PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content), AutoFormatEnable]
		public ImageProperties CollapseButtonRtl { get { return GetImage(CollapseButtonRtlImageName); } }
		[
#if !SL
	DevExpressWebLocalizedDescription("TreeViewImagesExpandButtonRtl"),
#endif
		NotifyParentProperty(true), PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content), AutoFormatEnable]
		public ImageProperties ExpandButtonRtl { get { return GetImage(ExpandButtonRtlImageName); } }
		[
#if !SL
	DevExpressWebLocalizedDescription("TreeViewImagesNodeLoadingPanel"),
#endif
		NotifyParentProperty(true), PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content), AutoFormatEnable]
		public ImageProperties NodeLoadingPanel { get { return GetImage(NodeLoadingPanelImageName); } }
		[
#if !SL
	DevExpressWebLocalizedDescription("TreeViewImagesNodeImage"),
#endif
		NotifyParentProperty(true), PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content), AutoFormatEnable]
		public ItemImageProperties NodeImage {
			get { return (ItemImageProperties)GetImageBase(NodeImageName); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("TreeViewImagesCheckBoxChecked"),
#endif
		NotifyParentProperty(true), PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content), AutoFormatEnable]
		public InternalCheckBoxImageProperties CheckBoxChecked {
			get { return base.CheckedImage; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("TreeViewImagesCheckBoxUnchecked"),
#endif
		NotifyParentProperty(true), PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content), AutoFormatEnable]
		public InternalCheckBoxImageProperties CheckBoxUnchecked {
			get { return base.UncheckedImage; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("TreeViewImagesCheckBoxGrayed"),
#endif
		NotifyParentProperty(true), PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content), AutoFormatEnable]
		public InternalCheckBoxImageProperties CheckBoxGrayed {
			get { return base.GrayedImage; }
		}
	}
}
