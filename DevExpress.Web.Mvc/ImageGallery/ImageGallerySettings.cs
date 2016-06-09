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
using System.ComponentModel;
using System.Web.UI.WebControls;
namespace DevExpress.Web.Mvc {
	using DevExpress.Web;
	using DevExpress.Utils;
	public class ImageGallerySettings : DataViewSettingsBase {
		MVCxImageGalleryItemCollection items;
		ImageGalleryFullscreenViewerSettings settingsFullscreenViewer;
		ImageGalleryFullscreenViewerImages imagesFullscreenViewer;
		ImageGalleryFullscreenViewerStyles stylesFullscreenViewer;
		ImageGalleryFullscreenViewerNavigationBarImages imagesFullscreenViewerNavigationBar;
		ImageGalleryFullscreenViewerNavigationBarStyles stylesFullscreenViewerNavigationBar;
		public ImageGallerySettings() {
			AllowExpandText = true;
			ImageCacheFolder = "\\Thumb\\";
			PagerAlign = PagerAlign.Justify;
			TextVisibility = ElementVisibilityMode.OnMouseOver;
			ThumbnailImageSizeMode = ImageSizeMode.FillAndCrop;
			ThumbnailHeight = 200;
			ThumbnailWidth = 200;
			UseHash = true;
			this.items = new MVCxImageGalleryItemCollection();
			this.settingsFullscreenViewer = new ImageGalleryFullscreenViewerSettings(null);
			this.imagesFullscreenViewer = new ImageGalleryFullscreenViewerImages(null);
			this.stylesFullscreenViewer = new ImageGalleryFullscreenViewerStyles(null);
			this.imagesFullscreenViewerNavigationBar = new ImageGalleryFullscreenViewerNavigationBarImages(null);
			this.stylesFullscreenViewerNavigationBar = new ImageGalleryFullscreenViewerNavigationBarStyles(null);		}
		public object CallbackRouteValues { get; set; }
		public object CustomActionRouteValues { get; set; }
		public bool AllowExpandText { get; set; }
		public bool UseHash { get; set; }
		public Layout Layout { get; set; }
		public ElementVisibilityMode TextVisibility { get; set; }
		public ImageSizeMode ThumbnailImageSizeMode { get; set; }
		public Unit ThumbnailHeight { get; set; }
		public Unit ThumbnailWidth { get; set; }
		public ImageGalleryClientSideEvents ClientSideEvents { get { return (ImageGalleryClientSideEvents)ClientSideEventsInternal; } }
		public new ImageGalleryImages Images { get { return (ImageGalleryImages)ImagesInternal; } }
		public new ImageGalleryStyles Styles { get { return (ImageGalleryStyles)StylesInternal; } }
		public ImageGalleryFullscreenViewerImages ImagesFullscreenViewer { get { return imagesFullscreenViewer; } }
		public ImageGalleryFullscreenViewerStyles StylesFullscreenViewer { get { return stylesFullscreenViewer; } }
		public ImageGalleryFullscreenViewerNavigationBarImages ImagesFullscreenViewerNavigationBar { get { return imagesFullscreenViewerNavigationBar; } }
		public ImageGalleryFullscreenViewerNavigationBarStyles StylesFullscreenViewerNavigationBar { get { return stylesFullscreenViewerNavigationBar; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public new LoadingPanelStyle LoadingPanelStyle { get { return base.LoadingPanelStyle; } }
		public MVCxImageGalleryPagerSettings PagerSettings { get { return (MVCxImageGalleryPagerSettings)PagerSettingsInternal; } }
		public ImageGalleryFlowLayoutSettings SettingsFlowLayout { get { return (ImageGalleryFlowLayoutSettings)SettingsFlowLayoutInternal; } }
		public ImageGalleryTableLayoutSettings SettingsTableLayout { get { return (ImageGalleryTableLayoutSettings)SettingsTableLayoutInternal; } }
		public ImageGalleryFullscreenViewerSettings SettingsFullscreenViewer { get { return settingsFullscreenViewer; } }
		public string ImageCacheFolder { get; set; }
		public string NameField { get; set; }
		public string ImageContentBytesField { get; set; }
		public string ImageUrlField { get; set; }
		public string FullscreenViewerTextField { get; set; }
		public string FullscreenViewerThumbnailUrlField { get; set; }
		public string NavigateUrlField { get; set; }
		public string NavigateUrlFormatString { get; set; }
		public string TextField { get; set; }
		public string ThumbnailUrlField { get; set; }
		public string ImageUrlFormatString { get; set; }
		public string FullscreenViewerThumbnailUrlFormatString { get; set; }
		public string ThumbnailUrlFormatString { get; set; }
		public string Target { get; set; }
		public MVCxImageGalleryItemCollection Items { get { return items; } }
		public DefaultBoolean RightToLeft { get { return RightToLeftInternal; } set { RightToLeftInternal = value; } }
		public ImageGalleryCustomImageProcessingEventHandler CustomImageProcessing { get; set; }
		public ImageGalleryItemEventHandler ItemDataBound { get; set; }
		protected internal string ItemTextTemplateContent { get; set; }
		protected internal Action<ImageGalleryThumbnailTemplateContainer> ItemTextTemplateContentMethod { get; set; }
		protected internal string FullscreenViewerItemTextTemplateContent { get; set; }
		protected internal Action<ImageGalleryFullscreenViewerItemTemplateContainer> FullscreenViewerItemTextTemplateContentMethod { get; set; }
		protected internal string FullscreenViewerTextTemplateContent { get; set; }
		protected internal Action<ImageGalleryFullscreenViewerItemTemplateContainer> FullscreenViewerTextTemplateContentMethod { get; set; }
		public void SetItemTextTemplateContent(Action<ImageGalleryThumbnailTemplateContainer> contentMethod) {
			ItemTextTemplateContentMethod = contentMethod;
		}
		public void SetItemTextTemplateContent(string content) {
			ItemTextTemplateContent = content;
		}
		public void SetFullscreenViewerItemTextTemplateContent(Action<ImageGalleryFullscreenViewerItemTemplateContainer> contentMethod) {
			FullscreenViewerItemTextTemplateContentMethod = contentMethod;
		}
		public void SetFullscreenViewerItemTextTemplateContent(string content) {
			FullscreenViewerItemTextTemplateContent = content;
		}
		public void SetFullscreenViewerTextTemplateContent(Action<ImageGalleryFullscreenViewerItemTemplateContainer> contentMethod) {
			FullscreenViewerTextTemplateContentMethod = contentMethod;
		}
		public void SetFullscreenViewerTextTemplateContent(string content) {
			FullscreenViewerTextTemplateContent = content;
		}
		protected override ImagesBase CreateImages() {
			return new ImageGalleryImages(null);
		}
		protected override StylesBase CreateStyles() {
			return new ImageGalleryStyles(null);
		}
		protected override DataViewFlowLayoutSettings CreateFlowLayoutSettings() {
			return new ImageGalleryFlowLayoutSettings(null);
		}
		protected override DataViewTableLayoutSettings CreateTableLayoutSettings() {
			return new ImageGalleryTableLayoutSettings(null);
		}
		protected override PagerSettingsEx CreatePagerSettings() {
			return new MVCxImageGalleryPagerSettings() { ImageGallerySettings = this };
		}
		protected override ClientSideEventsBase CreateClientSideEvents() {
			return new ImageGalleryClientSideEvents();
		}
	}
	public class MVCxImageGalleryPagerSettings : ImageGalleryPagerSettings {
		public MVCxImageGalleryPagerSettings()
			: base(null) {
		}
		protected internal ImageGallerySettings ImageGallerySettings { get; set; }
		protected override PageSizeItemSettings CreatePageSizeItemSettings(IPropertiesOwner owner) {
			return new MVCxImageGalleryPagerPageSizeItemSettings(owner);
		}
	}
	public class MVCxImageGalleryPagerPageSizeItemSettings : ImageGalleryPagerPageSizeItemSettings {
		public MVCxImageGalleryPagerPageSizeItemSettings(IPropertiesOwner owner)
			: base(owner) {
		}
		protected internal new MVCxImageGalleryPagerSettings PagerSettings {
			get { return Owner as MVCxImageGalleryPagerSettings; }
		}
		protected internal override bool IsTableLayout() {
			return PagerSettings.ImageGallerySettings.Layout == Layout.Table;
		}
	}
}
