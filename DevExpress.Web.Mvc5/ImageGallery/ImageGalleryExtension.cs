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

using System.Web.Mvc;
using System.Web.UI;
namespace DevExpress.Web.Mvc {
	using System;
	using DevExpress.Data.Linq;
	using DevExpress.Web;
	using DevExpress.Web.Mvc.Internal;
	public class ImageGalleryExtension : DataViewExtensionBase {
		public ImageGalleryExtension(ImageGallerySettings settings)
			: base(settings) {
		}
		public ImageGalleryExtension(ImageGallerySettings settings, ViewContext viewContext)
			: base(settings, viewContext) {
		}
		protected internal new MVCxImageGallery Control {
			get { return (MVCxImageGallery)base.Control; }
		}
		protected internal new ImageGallerySettings Settings {
			get { return (ImageGallerySettings)base.Settings; }
		}
		protected override void AssignInitialProperties() {
			Control.CallbackRouteValues = Settings.CallbackRouteValues;
			Control.CustomActionRouteValues = Settings.CustomActionRouteValues;
			Control.AllowExpandText = Settings.AllowExpandText;
			Control.UseHash = Settings.UseHash;
			Control.Layout = Settings.Layout;
			Control.Target = Settings.Target;
			Control.TextVisibility = Settings.TextVisibility;
			Control.ThumbnailImageSizeMode = Settings.ThumbnailImageSizeMode;
			Control.ThumbnailHeight = Settings.ThumbnailHeight;
			Control.ThumbnailWidth = Settings.ThumbnailWidth;
			Control.ClientSideEvents.Assign(Settings.ClientSideEvents);
			Control.Images.CopyFrom(Settings.Images);
			Control.Styles.CopyFrom(Settings.Styles);
			Control.ImagesFullscreenViewer.CopyFrom(Settings.ImagesFullscreenViewer);
			Control.StylesFullscreenViewer.CopyFrom(Settings.StylesFullscreenViewer);
			Control.ImagesFullscreenViewerNavigationBar.CopyFrom(Settings.ImagesFullscreenViewerNavigationBar);
			Control.StylesFullscreenViewerNavigationBar.CopyFrom(Settings.StylesFullscreenViewerNavigationBar);
			Control.PagerSettings.Assign(Settings.PagerSettings);
			Control.SettingsFolder.ImageCacheFolder = Settings.ImageCacheFolder;
			Control.SettingsFlowLayout.Assign(Settings.SettingsFlowLayout);
			Control.SettingsTableLayout.Assign(Settings.SettingsTableLayout);
			Control.SettingsFullscreenViewer.Assign(Settings.SettingsFullscreenViewer);
			Control.Items.Assign(Settings.Items);
			Control.NameField = Settings.NameField;
			Control.ImageContentBytesField = Settings.ImageContentBytesField;
			Control.ImageUrlField = Settings.ImageUrlField;
			Control.FullscreenViewerTextField = Settings.FullscreenViewerTextField;
			Control.FullscreenViewerThumbnailUrlField = Settings.FullscreenViewerThumbnailUrlField;
			Control.NavigateUrlField = Settings.NavigateUrlField;
			Control.NavigateUrlFormatString = Settings.NavigateUrlFormatString;
			Control.TextField = Settings.TextField;
			Control.ThumbnailUrlField = Settings.ThumbnailUrlField;
			Control.ImageUrlFormatString = Settings.ImageUrlFormatString;
			Control.FullscreenViewerThumbnailUrlFormatString = Settings.FullscreenViewerThumbnailUrlFormatString;
			Control.ThumbnailUrlFormatString = Settings.ThumbnailUrlFormatString;
			Control.RightToLeft = Settings.RightToLeft;
			base.AssignInitialProperties();
			Control.CustomImageProcessing += Settings.CustomImageProcessing;
			Control.ItemDataBound += Settings.ItemDataBound;
		}
		protected override void AssignRenderProperties() {
			base.AssignRenderProperties();
			bool replaceWriter = IsCallback();
			Control.ItemTextTemplate = ContentControlTemplate<ImageGalleryThumbnailTemplateContainer>.Create(
				Settings.ItemTextTemplateContent, Settings.ItemTextTemplateContentMethod, typeof(ImageGalleryThumbnailTemplateContainer));
			Control.FullscreenViewerItemTextTemplate = ContentControlTemplate<ImageGalleryFullscreenViewerItemTemplateContainer>.Create(
				Settings.FullscreenViewerItemTextTemplateContent, Settings.FullscreenViewerItemTextTemplateContentMethod, typeof(ImageGalleryFullscreenViewerItemTemplateContainer), replaceWriter);
			Control.FullscreenViewerTextTemplate = ContentControlTemplate<ImageGalleryFullscreenViewerItemTemplateContainer>.Create(
				Settings.FullscreenViewerTextTemplateContent, Settings.FullscreenViewerTextTemplateContentMethod, typeof(ImageGalleryFullscreenViewerItemTemplateContainer), replaceWriter);
			if(Settings.Items.Count > 0) {
				for(int i = 0; i < Control.Items.Count; i++) {
					Control.Items[i].FullscreenViewerTextTemplate = ContentControlTemplate<ImageGalleryThumbnailTemplateContainer>.Create(
						Settings.Items[i].FullscreenViewerTextTemplateContent, Settings.Items[i].FullscreenViewerTextTemplateContentMethod, typeof(ImageGalleryThumbnailTemplateContainer), replaceWriter);
					Control.Items[i].TextTemplate = ContentControlTemplate<ImageGalleryThumbnailTemplateContainer>.Create(
						Settings.Items[i].TextTemplateContent, Settings.Items[i].TextTemplateContentMethod, typeof(ImageGalleryThumbnailTemplateContainer));
				}
			}
		}
		public ImageGalleryExtension BindToFolder(string imageSourceFolder) {
			Control.SettingsFolder.ImageSourceFolder = imageSourceFolder;
			Control.DataBind();
			return this;
		}
		protected override void RenderCallbackResultControl() {
			if(!Control.IsEndlessPagingCallback) {
				base.RenderCallbackResultControl();
				return;
			}
			var writer = Utils.CreateHtmlTextWriter(Writer);
			Control.ContentControl.RenderEndlessPagingItems(writer);
		}
		protected internal override void PrepareControlProperties() {
			base.PrepareControlProperties();
			Control.ValidateProperties();
		}
		protected internal override void PrepareControl() {
			base.PrepareControl();
			Control.PerformOnInit();
		}
		protected override Control GetCallbackResultControl() {
			return Control.GetCallbackResultControl();
		}
		protected override ASPxWebControl CreateControl() {
			return new MVCxImageGallery(ViewContext);
		}
		protected override void BindToLINQDataSourceInternal(string contextTypeName, string tableName, EventHandler<LinqServerModeDataSourceSelectEventArgs> selectingMethod,
			EventHandler<DevExpress.Data.ServerModeExceptionThrownEventArgs> exceptionThrownMethod) {
			base.BindToLINQDataSourceInternal(contextTypeName, tableName, selectingMethod, exceptionThrownMethod);
			Control.DataBind();
		}
		protected override void BindToEFDataSourceInternal(string contextTypeName, string tableName, EventHandler<LinqServerModeDataSourceSelectEventArgs> selectingMethod,
			EventHandler<DevExpress.Data.ServerModeExceptionThrownEventArgs> exceptionThrownMethod) {
			base.BindToEFDataSourceInternal(contextTypeName, tableName, selectingMethod, exceptionThrownMethod);
			Control.DataBind();
		}
	}
}
