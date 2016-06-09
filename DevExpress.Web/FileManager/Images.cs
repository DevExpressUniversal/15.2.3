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
using DevExpress.Web;
using System.ComponentModel;
using System.Web.UI;
using DevExpress.Web.Internal;
using System.Web.UI.WebControls;
using DevExpress.Web.Localization;
namespace DevExpress.Web {
	public class FileManagerImages : ImagesBase {
		const int ErrorImageDefaultSize = 14;
		protected internal const string
			SplitterSeparatorImageName = "fmSplitterSeparator",
			FolderExpandButtonImageName = "fmFolderExpand",
			FolderCollapseButtonImageName = "fmFolderCollapse",
			FolderNodeLoadingPanelImageName = "fmFolderNodeLoading",
			FileImageName = "fmFile",
			PdfFileImageName = "fmFilePdf",
			PlainTextFileImageName = "fmFilePlainText",
			PresentationFileImageName = "fmFilePresentation",
			RichTextFileImageName = "fmFileRichText",
			SpreadsheetFileImageName = "fmFileSpreadsheet",
			DetailsCheckBoxCheckedImageName = "fmGridCheckBoxChecked",
			DetailsCheckBoxUncheckedImageName = "fmGridCheckBoxUnchecked",
			ThumbnailsCheckBoxCheckedImageName = "fmThumbCheckBoxChecked",
			ThumbnailsCheckBoxUncheckedImageName = "fmThumbCheckBoxUnchecked",
			FolderBigImageName = "fmFolderBig",
			FolderLockedBigImageName = "fmFolderLockedBig",
			FolderUpImageName = "fmFolderUp";
		protected internal const string
			FolderImageName = "fmFolder",
			FolderLockedImageName = "fmFolderLocked",
			CreateButtonImageName = "fmCreateButton",
			MoveButtonImageName = "fmMoveButton",
			RenameButtonImageName = "fmRenameButton",
			DeleteButtonImageName = "fmDeleteButton",
			RefreshButtonImageName = "fmRefreshButton",
			DownloadButtonImageName = "fmDwnlButton",
			UploadButtonImageName = "fmUplButton",
			CopyButtonImageName = "fmCopyButton",
			BreadcrumbsParentFolderButtonImageName = "fmBreadCrumbsUpButton",
			BreadcrumbsSeparatorImageName = "fmBreadCrumbsSeparatorArrow";
		public FileManagerImages(ISkinOwner owner)
			:base(owner) {
		}
		protected override void PopulateImageInfoList(List<ImageInfo> list) {
			base.PopulateImageInfoList(list);
			list.Add(GetImageInfo(FolderExpandButtonImageName, ImageFlags.IsPng | ImageFlags.HasNoResourceImage, 9, 9));
			list.Add(GetImageInfo(FolderCollapseButtonImageName, ImageFlags.IsPng | ImageFlags.HasNoResourceImage, 9, 9));
			list.Add(GetImageInfo(FolderNodeLoadingPanelImageName, ImageFlags.Empty | ImageFlags.HasNoResourceImage, 15, 15));
			list.Add(GetImageInfo(FileImageName, ImageFlags.IsPng, 70, 70));
			list.Add(GetImageInfo(PdfFileImageName, ImageFlags.IsPng, 70, 70));
			list.Add(GetImageInfo(PlainTextFileImageName, ImageFlags.IsPng, 70, 70));
			list.Add(GetImageInfo(PresentationFileImageName, ImageFlags.IsPng, 70, 70));
			list.Add(GetImageInfo(RichTextFileImageName, ImageFlags.IsPng, 70, 70));
			list.Add(GetImageInfo(SpreadsheetFileImageName, ImageFlags.IsPng, 70, 70));
			list.Add(GetImageInfo(FolderBigImageName, ImageFlags.IsPng, 70, 70));
			list.Add(GetImageInfo(FolderLockedBigImageName, ImageFlags.IsPng, 70, 70));
			list.Add(GetImageInfo(FolderUpImageName, ImageFlags.IsPng, 70, 70));
			list.Add(new ImageInfo(RefreshButtonImageName, ImageFlags.IsPng | ImageFlags.HasDisabledState, 16, 16,
				ASPxperienceLocalizer.GetString(ASPxperienceStringId.FileManager_TbRefresh), typeof(ImagePropertiesEx),
				RefreshButtonImageName));
			list.Add(new ImageInfo(DeleteButtonImageName, ImageFlags.IsPng | ImageFlags.HasDisabledState, 16, 16,
				ASPxperienceLocalizer.GetString(ASPxperienceStringId.FileManager_TbDelete), typeof(ImagePropertiesEx),
				DeleteButtonImageName));
			list.Add(new ImageInfo(RenameButtonImageName, ImageFlags.IsPng | ImageFlags.HasDisabledState, 16, 16,
				ASPxperienceLocalizer.GetString(ASPxperienceStringId.FileManager_TbRename), typeof(ImagePropertiesEx),
				RenameButtonImageName));
			list.Add(new ImageInfo(MoveButtonImageName, ImageFlags.IsPng | ImageFlags.HasDisabledState, 16, 16,
				ASPxperienceLocalizer.GetString(ASPxperienceStringId.FileManager_TbMove), typeof(ImagePropertiesEx),
				MoveButtonImageName));
			list.Add(new ImageInfo(CreateButtonImageName, ImageFlags.IsPng | ImageFlags.HasDisabledState, 16, 16,
				ASPxperienceLocalizer.GetString(ASPxperienceStringId.FileManager_TbCreate), typeof(ImagePropertiesEx),
				CreateButtonImageName));
			list.Add(new ImageInfo(DownloadButtonImageName, ImageFlags.IsPng | ImageFlags.HasDisabledState, 16, 16,
				ASPxperienceLocalizer.GetString(ASPxperienceStringId.FileManager_TbDownload), typeof(ImagePropertiesEx),
				DownloadButtonImageName));
			list.Add(new ImageInfo(UploadButtonImageName, ImageFlags.IsPng | ImageFlags.HasDisabledState, 16, 16,
				ASPxperienceLocalizer.GetString(ASPxperienceStringId.UploadControl_UploadButton), typeof(ImagePropertiesEx),
				UploadButtonImageName));
			list.Add(new ImageInfo(CopyButtonImageName, ImageFlags.IsPng | ImageFlags.HasDisabledState, 16, 16,
				ASPxperienceLocalizer.GetString(ASPxperienceStringId.FileManager_TbCopy), typeof(ImagePropertiesEx),
				CopyButtonImageName));
			list.Add(new ImageInfo(FolderImageName, ImageFlags.IsPng, 16, 16,
				ASPxperienceLocalizer.GetString(ASPxperienceStringId.FileManager_Folder),
				FolderImageName));
			list.Add(new ImageInfo(FolderLockedImageName, ImageFlags.IsPng, 16, 16,
				ASPxperienceLocalizer.GetString(ASPxperienceStringId.FileManager_FolderLocked),
				FolderLockedImageName));
			list.Add(new ImageInfo(DetailsCheckBoxCheckedImageName, ImageFlags.IsPng, 8, 8, string.Empty, typeof(ImagePropertiesEx)));
			list.Add(new ImageInfo(DetailsCheckBoxUncheckedImageName, ImageFlags.IsPng, 8, 8, string.Empty, typeof(ImagePropertiesEx)));
			list.Add(new ImageInfo(ThumbnailsCheckBoxCheckedImageName, ImageFlags.IsPng, 8, 8, string.Empty, typeof(ImagePropertiesEx)));
			list.Add(new ImageInfo(ThumbnailsCheckBoxUncheckedImageName, ImageFlags.IsPng, 8, 8, string.Empty, typeof(ImagePropertiesEx)));
			list.Add(new ImageInfo(BreadcrumbsParentFolderButtonImageName, ImageFlags.IsPng | ImageFlags.HasDisabledState | ImageFlags.HasHottrackState, 16, 16, string.Empty, typeof(ItemImageProperties), BreadcrumbsParentFolderButtonImageName));
			list.Add(new ImageInfo(BreadcrumbsSeparatorImageName, ImageFlags.IsPng, 4, 7, string.Empty, typeof(ImagePropertiesEx), BreadcrumbsSeparatorImageName));
		}
		protected ImageInfo GetImageInfo(string name, ImageFlags flags, int width, int height) {
			return new ImageInfo(name, flags, Unit.Pixel(width), Unit.Pixel(height));
		}
		protected ImageInfo GetSpriteImageInfo(string name) {
			return new ImageInfo(name, name);
		}
		protected internal new ImagePropertiesBase GetImageBase(string name) {
			return base.GetImageBase(name);
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("FileManagerImagesDetailsCheckBoxChecked"),
#endif
		AutoFormatEnable, NotifyParentProperty(true), PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public ImagePropertiesEx DetailsCheckBoxChecked
		{
			get { return (ImagePropertiesEx)GetImageBase(DetailsCheckBoxCheckedImageName); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("FileManagerImagesDetailsCheckBoxUnchecked"),
#endif
		AutoFormatEnable, NotifyParentProperty(true), PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public ImagePropertiesEx DetailsCheckBoxUnchecked
		{
			get { return (ImagePropertiesEx)GetImageBase(DetailsCheckBoxUncheckedImageName); }
		}
		[AutoFormatEnable, NotifyParentProperty(true), PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public ImagePropertiesEx ThumbnailsCheckBoxChecked {
			get { return (ImagePropertiesEx)GetImageBase(ThumbnailsCheckBoxCheckedImageName); }
		}
		[AutoFormatEnable, NotifyParentProperty(true), PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public ImagePropertiesEx ThumbnailsCheckBoxUnchecked {
			get { return (ImagePropertiesEx)GetImageBase(ThumbnailsCheckBoxUncheckedImageName); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("FileManagerImagesFile"),
#endif
		AutoFormatEnable, NotifyParentProperty(true), PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public ImageProperties File {
			get { return GetImage(FileImageName); }
		}
		[AutoFormatEnable, NotifyParentProperty(true), PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public ImageProperties FileAreaFolder {
			get { return GetImage(FolderBigImageName); }
		}
		[AutoFormatEnable, NotifyParentProperty(true), PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public ImageProperties FileAreaFolderLocked {
			get { return GetImage(FolderLockedBigImageName); }
		}
		[AutoFormatEnable, NotifyParentProperty(true), PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public ImageProperties ParentFolder {
			get { return GetImage(FolderUpImageName); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("FileManagerImagesFolderExpandButton"),
#endif
		AutoFormatEnable, NotifyParentProperty(true), PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public ImageProperties FolderExpandButton {
			get { return GetImage(FolderExpandButtonImageName); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("FileManagerImagesFolderCollapseButton"),
#endif
		AutoFormatEnable, NotifyParentProperty(true), PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public ImageProperties FolderCollapseButton {
			get { return GetImage(FolderCollapseButtonImageName); }
		}
		[AutoFormatEnable, NotifyParentProperty(true), PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public ItemImageProperties BreadcrumbsParentFolderButton {
			get { return (ItemImageProperties)GetImageBase(BreadcrumbsParentFolderButtonImageName); }
		}
		[AutoFormatEnable, NotifyParentProperty(true), PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public ImagePropertiesBase BreadcrumbsSeparator {
			get { return (ImagePropertiesBase)GetImageBase(BreadcrumbsSeparatorImageName); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("FileManagerImagesRefreshButton"),
#endif
		AutoFormatEnable, NotifyParentProperty(true), PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public ImagePropertiesEx RefreshButton {
			get { return (ImagePropertiesEx)GetImageBase(RefreshButtonImageName); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("FileManagerImagesCreateButton"),
#endif
		AutoFormatEnable, NotifyParentProperty(true), PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public ImagePropertiesEx CreateButton {
			get { return (ImagePropertiesEx)GetImageBase(CreateButtonImageName); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("FileManagerImagesRenameButton"),
#endif
		AutoFormatEnable, NotifyParentProperty(true), PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public ImagePropertiesEx RenameButton {
			get { return (ImagePropertiesEx)GetImageBase(RenameButtonImageName); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("FileManagerImagesMoveButton"),
#endif
		AutoFormatEnable, NotifyParentProperty(true), PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public ImagePropertiesEx MoveButton {
			get { return (ImagePropertiesEx)GetImageBase(MoveButtonImageName); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("FileManagerImagesDeleteButton"),
#endif
		AutoFormatEnable, NotifyParentProperty(true), PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public ImagePropertiesEx DeleteButton {
			get { return (ImagePropertiesEx)GetImageBase(DeleteButtonImageName); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("FileManagerImagesDownloadButton"),
#endif
		AutoFormatEnable, NotifyParentProperty(true), PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public ImagePropertiesEx DownloadButton
		{
			get { return (ImagePropertiesEx)GetImageBase(DownloadButtonImageName); }
		}
		[AutoFormatEnable, NotifyParentProperty(true), PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public ImagePropertiesEx UploadButton {
			get { return (ImagePropertiesEx)GetImageBase(UploadButtonImageName); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("FileManagerImagesCopyButton"),
#endif
		AutoFormatEnable, NotifyParentProperty(true), PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public ImagePropertiesEx CopyButton
		{
			get { return (ImagePropertiesEx)GetImageBase(CopyButtonImageName); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("FileManagerImagesFolder"),
#endif
		AutoFormatEnable, NotifyParentProperty(true), PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public ImageProperties Folder {
			get { return GetImage(FolderImageName); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("FileManagerImagesFolderContainerNodeLoadingPanel"),
#endif
		AutoFormatEnable, NotifyParentProperty(true), PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public ImageProperties FolderContainerNodeLoadingPanel
		{
			get { return GetImage(FolderNodeLoadingPanelImageName); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("FileManagerImagesFolderLocked"),
#endif
		AutoFormatEnable, NotifyParentProperty(true), PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public ImageProperties FolderLocked {
			get { return GetImage(FolderLockedImageName); }
		}
	}
	public class FileManagerDetailsViewImages : GridViewImages {
		public FileManagerDetailsViewImages(ISkinOwner owner)
			:base(owner) { }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override ImageProperties LoadingPanel { get { return base.LoadingPanel; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new ImageProperties LoadingPanelOnStatusBar { get { return GetImage(LoadingPanelOnStatusBarName); } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new ImageProperties CollapsedButton { get { return GetImage(CollapsedButtonName); } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new ImageProperties CollapsedButtonRtl { get { return GetImage(CollapsedButtonRtlName); } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new ImageProperties ExpandedButton { get { return GetImage(ExpandedButtonName); } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new ImageProperties ExpandedButtonRtl { get { return GetImage(ExpandedButtonRtlName); } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new ImageProperties CustomizationWindowClose { get { return GetImage(CustomizationWindowCloseName); } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new ImageProperties FilterBuilderClose { get { return GetImage(FilterBuilderCloseName); } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new ImageProperties DetailCollapsedButton { get { return GetImage(DetailCollapsedButtonName); } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new ImageProperties DetailCollapsedButtonRtl { get { return GetImage(DetailCollapsedButtonRtlName); } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new ImageProperties DetailExpandedButton { get { return GetImage(DetailExpandedButtonName); } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new ImageProperties DetailExpandedButtonRtl { get { return GetImage(DetailExpandedButtonRtlName); } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new ImageProperties FilterRowButton { get { return GetImage(FilterRowButtonName); } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new ImageProperties ParentGroupRows { get { return GetImage(ParentGroupRowsName); } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new ImageProperties FixedGroupRow { get { return GetImage(FixedGroupRowName); } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new ImageProperties PopupEditFormWindowClose { get { return GetImage(PopupEditFormWindowCloseName); } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new ImageProperties WindowResizer { get { return WindowResizerInternal; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new ImageProperties WindowResizerRtl { get { return WindowResizerRtlInternal; } }
	}
}
