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
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using DevExpress.XtraPrinting.Preview.Native;
using System.Collections.Generic;
using DevExpress.XtraPrinting.Native;
using DevExpress.XtraPrinting.Preview.Native.Galleries;
namespace DevExpress.XtraPrinting.Preview {
	[TypeConverter(typeof(ImageCollectionTypeConverter))]
	[Editor(typeof(System.Drawing.Design.UITypeEditor), typeof(System.Drawing.Design.UITypeEditor))]
	public class RibbonImageCollection : Collection<ImageItem> {
		internal static readonly string[] ImageNames;
		static RibbonImageCollection() {
			List<string> imageNames = new List<string>();
			FillExportImageNames(imageNames, PSCommandHelper.ExportCommands);
			FillExportImageNames(imageNames, PSCommandHelper.SendCommands);
			FillImageNames(imageNames, PrintPreviewPageMarginsGalleryDropDown.PredefinedItemsData);
			FillImageNames(imageNames, PrintPreviewPageOrientationGalleryDropDown.PredefinedItemsData);
			FillImageNames(imageNames, PrintPreviewPaperSizeGalleryDropDown.PredefinedItemsData);
			ImageNames = imageNames.ToArray();
		}
		static void FillExportImageNames(List<string> imageNames, PrintingSystemCommand[] commands) {
			foreach(PrintPreviewGalleryItemData itemData in PrintPreviewExportGalleryDropDown.GetItemsData(commands)) {
				imageNames.Add(itemData.Alias);
				imageNames.Add(PrintRibbonControllerConfigurator.GetLargeImageName(itemData.Alias));
			}
		}
		static void FillImageNames(List<string> imageNames, PrintPreviewGalleryItemData[] itemsData) {
			foreach(PrintPreviewGalleryItemData itemData in itemsData) {
				imageNames.Add(itemData.Alias);
			}
		}
		ImageItem GetImageItem(string name) {
			foreach(ImageItem item in this) {
				if(name == item.Name)
					return item;
			}
			return null;
		}
		public Image GetImage(string name) {
			ImageItem item = GetImageItem(name);
			return item != null ? item.Image : null;
		}
		public void SetImage(string name, Image value) {
			ImageItem imageItem = GetImageItem(name);
			if(value == null) {
				if(imageItem != null)
					this.Remove(imageItem);
				return;
			}
			if(imageItem == null) {
				imageItem = new ImageItem(name, value);
				this.Add(imageItem);
			} else {
				imageItem.Image = value;
			}
		}
	}
	[TypeConverter(typeof(ImageItemTypeConverter))]
	public class ImageItem {
		string name;
		Image image;
		public ImageItem() {
		}
		public ImageItem(string name, Image image) {
			this.name = name;
			this.image = image;
		}
		public string Name {
			get { return name; }
			set { name = value; }
		}
		public Image Image {
			get { return image; }
			set { image = value; }
		}
	}
}
