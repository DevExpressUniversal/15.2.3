#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       XtraReports for ASP.NET                                     }
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
using DevExpress.XtraReports.Web.Native;
namespace DevExpress.XtraReports.Web {
	public class ReportToolbarImages : ImagesBase {
		#region resources
		internal const string
			BtnFirstPageImageName = "BtnFirstPage",
			BtnLastPageImageName = "BtnLastPage",
			BtnNextPageImageName = "BtnNextPage",
			BtnPrevPageImageName = "BtnPrevPage",
			BtnPrintImageName = "BtnPrint",
			BtnPrintPageImageName = "BtnPrintPage",
			BtnSaveImageName = "BtnSave",
			BtnSaveWindowImageName = "BtnSaveWindow",
			BtnSearchImageName = "BtnSearch";
		#endregion
		const string EnableDefaultImagesName = "EnableDefaultImages";
		const bool DefaultEnableDefaultImages = true;
		const int ImageSize = 16;
		#region properties
#if !SL
	[DevExpressXtraReportsWebLocalizedDescription("ReportToolbarImagesBtnFirstPage")]
#endif
		[Category("Images")]
		[PersistenceMode(PersistenceMode.InnerProperty)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		[NotifyParentProperty(true)]
		public ItemImageProperties BtnFirstPage {
			get { return (ItemImageProperties)GetImageBase(ReportToolbarImages.BtnFirstPageImageName); }
		}
#if !SL
	[DevExpressXtraReportsWebLocalizedDescription("ReportToolbarImagesBtnLastPage")]
#endif
		[Category("Images")]
		[PersistenceMode(PersistenceMode.InnerProperty)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		[NotifyParentProperty(true)]
		public ItemImageProperties BtnLastPage {
			get { return (ItemImageProperties)GetImageBase(ReportToolbarImages.BtnLastPageImageName); }
		}
#if !SL
	[DevExpressXtraReportsWebLocalizedDescription("ReportToolbarImagesBtnNextPage")]
#endif
		[Category("Images")]
		[PersistenceMode(PersistenceMode.InnerProperty)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		[NotifyParentProperty(true)]
		public ItemImageProperties BtnNextPage {
			get { return (ItemImageProperties)GetImageBase(ReportToolbarImages.BtnNextPageImageName); }
		}
#if !SL
	[DevExpressXtraReportsWebLocalizedDescription("ReportToolbarImagesBtnPrevPage")]
#endif
		[Category("Images")]
		[PersistenceMode(PersistenceMode.InnerProperty)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		[NotifyParentProperty(true)]
		public ItemImageProperties BtnPrevPage {
			get { return (ItemImageProperties)GetImageBase(ReportToolbarImages.BtnPrevPageImageName); }
		}
#if !SL
	[DevExpressXtraReportsWebLocalizedDescription("ReportToolbarImagesBtnPrint")]
#endif
		[Category("Images")]
		[PersistenceMode(PersistenceMode.InnerProperty)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		[NotifyParentProperty(true)]
		public ItemImageProperties BtnPrint {
			get { return (ItemImageProperties)GetImageBase(ReportToolbarImages.BtnPrintImageName); }
		}
#if !SL
	[DevExpressXtraReportsWebLocalizedDescription("ReportToolbarImagesBtnPrintPage")]
#endif
		[Category("Images")]
		[PersistenceMode(PersistenceMode.InnerProperty)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		[NotifyParentProperty(true)]
		public ItemImageProperties BtnPrintPage {
			get { return (ItemImageProperties)GetImageBase(ReportToolbarImages.BtnPrintPageImageName); }
		}
#if !SL
	[DevExpressXtraReportsWebLocalizedDescription("ReportToolbarImagesBtnSave")]
#endif
		[Category("Images")]
		[PersistenceMode(PersistenceMode.InnerProperty)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		[NotifyParentProperty(true)]
		public ItemImageProperties BtnSave {
			get { return (ItemImageProperties)GetImageBase(ReportToolbarImages.BtnSaveImageName); }
		}
#if !SL
	[DevExpressXtraReportsWebLocalizedDescription("ReportToolbarImagesBtnSaveWindow")]
#endif
		[Category("Images")]
		[PersistenceMode(PersistenceMode.InnerProperty)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		[NotifyParentProperty(true)]
		public ItemImageProperties BtnSaveWindow {
			get { return (ItemImageProperties)GetImageBase(ReportToolbarImages.BtnSaveWindowImageName); }
		}
#if !SL
	[DevExpressXtraReportsWebLocalizedDescription("ReportToolbarImagesBtnSearch")]
#endif
		[Category("Images")]
		[PersistenceMode(PersistenceMode.InnerProperty)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		[NotifyParentProperty(true)]
		public ItemImageProperties BtnSearch {
			get { return (ItemImageProperties)GetImageBase(ReportToolbarImages.BtnSearchImageName); }
		}
		#endregion
		#region hidden properties
		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override ImageProperties LoadingPanel {
			get { return base.LoadingPanel; }
		}
#if !SL
	[DevExpressXtraReportsWebLocalizedDescription("ReportToolbarImagesEnableDefaultImages")]
#endif
		[Category("Appearance")]
		[DefaultValue(DefaultEnableDefaultImages)]
		[NotifyParentProperty(true)]
		public bool EnableDefaultImages {
			get {
				return GetBoolProperty(EnableDefaultImagesName, DefaultEnableDefaultImages);
			}
			set {
				SetBoolProperty(EnableDefaultImagesName, DefaultEnableDefaultImages, value);
				Changed();
			}
		}
		#endregion
		public ReportToolbarImages(ISkinOwner owner)
			: base(owner) {
		}
		public override ImageProperties GetImageProperties(Page page, string imageName, bool encode) {
			if(EnableDefaultImages)
				return base.GetImageProperties(page, imageName, encode);
			var result = new ImageProperties();
			result.CopyFrom(GetImage(imageName));
			return result;
		}
		public override string ToString() {
			return string.Empty;
		}
		protected override void PopulateImageInfoList(List<ImageInfo> list) {
			base.PopulateImageInfoList(list);
			foreach(string name in ToolbarWebImageResource.GetImageResourceNames()) {
				var imageInfo = new ImageInfo(name, ImageFlags.HasDisabledState | ImageFlags.IsPng, ImageSize, typeof(ItemImageProperties), name);
				list.Add(imageInfo);
			}
		}
		protected override Type GetResourceType() {
			return typeof(ReportToolbar);
		}
		protected override string GetResourceImagePath() {
			return WebResourceNames.WebImagesResourcePath;
		}
		protected override string GetDesignTimeResourceSpriteImagePath() {
			return WebResourceNames.WebImagesResourcePath + GetDesignTimeResourceSpriteImageDefaultName();
		}
		protected override string GetResourceSpriteCssPath() {
			return ReportToolbar.SpriteCssResourceName;
		}
		protected override bool KeepDefaultSizes {
			get { return true; }
		}
	}
}
