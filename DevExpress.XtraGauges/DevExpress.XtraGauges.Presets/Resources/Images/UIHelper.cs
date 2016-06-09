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

using System.Drawing;
using System.Reflection;
using DevExpress.Utils;
using DevExpress.Utils.Controls;
using DevExpress.XtraGauges.Core;
namespace DevExpress.XtraGauges.Presets.Resources {
	public sealed class UIHelperLoader {
		static UIHelperLoader() {
			Core.Styles.StyleCollectionHelper.Register<IUIHelperService, UIHelperServiceProvider>();
		}
	}
	static class UIHelper {
		static ImageCollection circularGaugeElementImages;
		static ImageCollection linearGaugeElementImages;
		static ImageCollection gaugeTypeImages;
		static ImageCollection buttonImages;
		static ImageCollection otherImages;
		static ImageCollection digitalGaugesMenuImages;
		static ImageCollection galleryItemBGImages;
		static ImageCollection presetCategoryImagesCore;
		static ImageCollection overviewImagesCore;
		static ImageCollection actionImagesCore;
		static Image changeStyleImageCore;
		static ImageCollection expandCollapseImagesCore;
		const string resourcesImages = "DevExpress.XtraGauges.Presets.Resources.Images.";
		static UIHelper() {
			Assembly presetsAssembly = typeof(UIHelper).Assembly;
			circularGaugeElementImages = ImageHelper.CreateImageCollectionFromResources(
					resourcesImages + "gauge-circular-elements.png",
					presetsAssembly,
					new Size(15, 15)
				);
			linearGaugeElementImages = ImageHelper.CreateImageCollectionFromResources(
					resourcesImages + "gauge-linear-elements.png",
					presetsAssembly,
					new Size(15, 15)
				);
			gaugeTypeImages = ImageHelper.CreateImageCollectionFromResources(
					resourcesImages + "gauge-types.png",
					presetsAssembly,
					new Size(15, 15)
				);
			buttonImages = ImageHelper.CreateImageCollectionFromResources(
					resourcesImages + "gauge-UI-icons.png",
					 presetsAssembly,
					new Size(15, 15)
				);
			otherImages = ImageHelper.CreateImageCollectionFromResources(
					resourcesImages + "gauge-UI2-icons.png",
					 presetsAssembly,
					new Size(15, 15)
				);
			digitalGaugesMenuImages = ImageHelper.CreateImageCollectionFromResources(
					resourcesImages + "gauge-digital-menu.png",
					 presetsAssembly,
					new Size(15, 15)
				);
			galleryItemBGImages = new ImageCollection();
			galleryItemBGImages.ImageSize = new Size(182, 200);
			galleryItemBGImages.AddImage(
				ResourceImageHelper.CreateImageFromResources(resourcesImages + "HoveredItem.png", presetsAssembly));
			galleryItemBGImages.AddImage(
				ResourceImageHelper.CreateImageFromResources(resourcesImages + "SelectedItem.png", presetsAssembly));
			presetCategoryImagesCore = ImageHelper.CreateImageCollectionFromResources(
					resourcesImages + "PresetCategoryIcons.png",
					presetsAssembly,
					new Size(15, 15)
				);
			actionImagesCore = ImageHelper.CreateImageCollectionFromResources(
					resourcesImages + "NavigationControlImages.png",
					presetsAssembly,
					new Size(13, 13)
				);
			changeStyleImageCore = ResourceImageHelper.CreateImageFromResources(
					resourcesImages + "ChangeStyle.png", presetsAssembly);
			expandCollapseImagesCore = ImageHelper.CreateImageCollectionFromResources(
					resourcesImages + "gauge-expand-collapse-buttons.png",
					presetsAssembly,
					new Size(15, 15)
				);
			overviewImagesCore = new ImageCollection();
			overviewImagesCore.ImageSize = new Size(721, 576);
			overviewImagesCore.Images.Add(ResourceImageHelper.CreateImageFromResources(
				resourcesImages + "Circular-Gauge-Overview.jpg", presetsAssembly));
			overviewImagesCore.Images.Add(ResourceImageHelper.CreateImageFromResources(
				resourcesImages + "Linear-Gauge-Overview.jpg", presetsAssembly));
			overviewImagesCore.Images.Add(ResourceImageHelper.CreateImageFromResources(
				resourcesImages + "Digital-Gauge-Overview.jpg", presetsAssembly));
			overviewImagesCore.Images.Add(ResourceImageHelper.CreateImageFromResources(
				resourcesImages + "StateIndicator-Gauge-Overview.jpg", presetsAssembly));
		}
		public static Image ChangeStyleImage {
			get { return changeStyleImageCore; }
		}
		public static ImageCollection ExpandCollapseImages {
			get { return expandCollapseImagesCore; }
		}
		public static ImageCollection CircularGaugeElementImages {
			get { return circularGaugeElementImages; }
		}
		public static ImageCollection LinearGaugeElementImages {
			get { return linearGaugeElementImages; }
		}
		public static ImageCollection DigitalGaugesMenu {
			get { return digitalGaugesMenuImages; }
		}
		public static ImageCollection GaugeTypeImages {
			get { return gaugeTypeImages; }
		}
		public static ImageCollection UIButtonImages {
			get { return buttonImages; }
		}
		public static ImageCollection UIOtherImages {
			get { return otherImages; }
		}
		public static ImageCollection OverviewImages {
			get { return overviewImagesCore; }
		}
		public static ImageCollection GalleryItemImages {
			get { return galleryItemBGImages; }
		}
		public static ImageCollection PresetCategoryImages {
			get { return presetCategoryImagesCore; }
		}
		public static ImageCollection UIActionImages {
			get { return actionImagesCore; }
		}
	}
	sealed class UIHelperServiceProvider : IUIHelperService {
		IImageAccessor IUIHelperService.GaugeTypeImages {
			get { return new ImageAccessor(UIHelper.GaugeTypeImages.Images); }
		}
		IImageAccessor IUIHelperService.UIOtherImages {
			get { return new ImageAccessor(UIHelper.UIOtherImages.Images); }
		}
		IImageAccessor IUIHelperService.UIActionImages {
			get { return new ImageAccessor(UIHelper.UIActionImages.Images); }
		}
		IImageAccessor IUIHelperService.UIButtonImages {
			get { return new ImageAccessor(UIHelper.UIButtonImages.Images); }
		}
		IImageAccessor IUIHelperService.LinearGaugeElementImages {
			get { return new ImageAccessor(UIHelper.LinearGaugeElementImages.Images); }
		}
		IImageAccessor IUIHelperService.CircularGaugeElementImages {
			get { return new ImageAccessor(UIHelper.CircularGaugeElementImages.Images); }
		}
		IImageAccessor IUIHelperService.DigitalGaugesMenu {
			get { return new ImageAccessor(UIHelper.DigitalGaugesMenu.Images); }
		}
		IImageAccessor IUIHelperService.OverviewImages {
			get { return new ImageAccessor(UIHelper.OverviewImages.Images); }
		}
		IImageAccessor IUIHelperService.ExpandCollapseImages {
			get { return new ImageAccessor(UIHelper.ExpandCollapseImages.Images); }
		}
		Image IUIHelperService.ChangeStyleImage {
			get { return UIHelper.ChangeStyleImage; }
		}
		sealed class ImageAccessor : IImageAccessor {
			Images images;
			public ImageAccessor(Images images) {
				this.images = images;
			}
			public Image this[int index] {
				get { return images[index]; }
			}
		}
	}
}
