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
using System.Drawing;
using System.Drawing.Imaging;
using System.Collections.Generic;
using DevExpress.Utils;
using DevExpress.Printing;
using DevExpress.Printing.Native.PrintEditor;
namespace DevExpress.XtraEditors.Preview {
	public class PrinterImagesContainer : IDisposable {
		static Image checkMarkSmall = DevExpress.Utils.ResourceImageHelper.CreateImageFromResources("DevExpress.XtraEditors.Preview.Images.CheckMark_8x8.png", System.Reflection.Assembly.GetExecutingAssembly());
		static Image checkMarkLarge = DevExpress.Utils.ResourceImageHelper.CreateImageFromResources("DevExpress.XtraEditors.Preview.Images.CheckMark_15x15.png", System.Reflection.Assembly.GetExecutingAssembly());
		Dictionary<PrinterType, int> imageIndices = new Dictionary<PrinterType, int>();
		System.ComponentModel.IContainer components;
		Utils.ImageCollection smallImages;
		Utils.ImageCollection largeImages;
		public int GetImageIndex(PrinterItem item) {
			return imageIndices[item.PrinterType];
		}
		public Utils.ImageCollection SmallImages {
			get {
				if(smallImages == null) {
					smallImages = new DevExpress.Utils.ImageCollection(components);
					LoadSmallImages(smallImages);
				}
				return smallImages; 
			}
		}
		public Utils.ImageCollection LargeImages {
			get {
				if(largeImages == null) {
					largeImages = new DevExpress.Utils.ImageCollection(components) { ImageSize = new Size(32, 32) };
					LoadLargeImages(largeImages);
				}
				return largeImages; 
			}
		}
		public PrinterImagesContainer() {
			components = new System.ComponentModel.Container();
		}
		public void Dispose() {
			if(components != null) {
				components.Dispose();
				components = null;
			}
		}
		void LoadSmallImages(ImageCollection imageCollection) {
			AddImagesToCollection("DevExpress.XtraEditors.Preview.Images.Fax_16x16.png", imageCollection, checkMarkSmall, PrinterType.Fax);
			AddImagesToCollection("DevExpress.XtraEditors.Preview.Images.FaxNetwork_16x16.png", imageCollection, checkMarkSmall, PrinterType.Fax | PrinterType.Network);
			AddImagesToCollection("DevExpress.XtraEditors.Preview.Images.Printer_16x16.png", imageCollection, checkMarkSmall, PrinterType.Printer);
			AddImagesToCollection("DevExpress.XtraEditors.Preview.Images.PrinterNetwork_16x16.png", imageCollection, checkMarkSmall, PrinterType.Network);
		}
		void LoadLargeImages(ImageCollection imageCollection) {
			AddImagesToCollection("DevExpress.XtraEditors.Preview.Images.Fax_32x32.png", imageCollection, checkMarkLarge, PrinterType.Fax);
			AddImagesToCollection("DevExpress.XtraEditors.Preview.Images.FaxNetwork_32x32.png", imageCollection, checkMarkLarge, PrinterType.Fax | PrinterType.Network);
			AddImagesToCollection("DevExpress.XtraEditors.Preview.Images.Printer_32x32.png", imageCollection, checkMarkLarge, PrinterType.Printer);
			AddImagesToCollection("DevExpress.XtraEditors.Preview.Images.PrinterNetwork_32x32.png", imageCollection, checkMarkLarge, PrinterType.Network);
		}
		void AddImagesToCollection(string resourceName, ImageCollection imgCollection, Image checkMark, PrinterType flags) {
			const int imageTransparency = 120;
			Image sourceImage = CreateImage(resourceName);
			imgCollection.AddImage(sourceImage);
			imageIndices[flags] = imgCollection.Images.Count - 1;
			Image transparentSourceImage = CreateTransparentBitmap(sourceImage, imageTransparency);
			imgCollection.AddImage(transparentSourceImage);
			imageIndices[flags | PrinterType.Offline] = imgCollection.Images.Count - 1;
			imgCollection.AddImage(CreateImageDefaultPrinter(sourceImage, checkMark));
			imageIndices[flags | PrinterType.Default] = imgCollection.Images.Count - 1;
			imgCollection.AddImage(CreateImageDefaultPrinter(transparentSourceImage, checkMark));
			imageIndices[flags | PrinterType.Default | PrinterType.Offline] = imgCollection.Images.Count - 1;
		}
		static Image CreateImageDefaultPrinter(Image source, Image checkMark) {
			Bitmap bmpWithCheckMark = new Bitmap(source);
			using(Bitmap cloneCheckMark = new Bitmap(checkMark)) {
				if(cloneCheckMark.HorizontalResolution != bmpWithCheckMark.HorizontalResolution || cloneCheckMark.VerticalResolution != bmpWithCheckMark.VerticalResolution)
					cloneCheckMark.SetResolution(bmpWithCheckMark.HorizontalResolution, bmpWithCheckMark.VerticalResolution);
				using(Graphics gr = Graphics.FromImage(bmpWithCheckMark))
					gr.DrawImage(cloneCheckMark, source.Width - cloneCheckMark.Width, 0);
			}
			return bmpWithCheckMark;
		}
		static Bitmap CreateTransparentBitmap(Image original, int imageTransparency) {
			Bitmap bitmap = BitmapCreator.CreateClearBitmap(original, 600);
			ImageAttributes attributes = BitmapCreator.CreateTransparencyAttributes(imageTransparency);
			BitmapCreator.TransformBitmap(original, bitmap, attributes);
			return bitmap;
		}
		static Image CreateImage(string resourceName) {
			return DevExpress.Utils.ResourceImageHelper.CreateImageFromResources(resourceName, System.Reflection.Assembly.GetExecutingAssembly());
		}
	}
}
