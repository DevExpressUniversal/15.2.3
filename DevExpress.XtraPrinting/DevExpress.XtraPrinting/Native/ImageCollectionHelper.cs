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
using System.Drawing;
using DevExpress.Utils;
namespace DevExpress.XtraPrinting.Native {
	public class ImageCollectionHelper {
		public static DevExpress.Utils.ImageCollection CreateVoidImageCollection() {
			DevExpress.Utils.ImageCollection imageList = new DevExpress.Utils.ImageCollection();
			imageList.ImageSize = new System.Drawing.Size(16, 16);
			imageList.TransparentColor = System.Drawing.Color.Fuchsia;
			return imageList;
		}
		public static DevExpress.Utils.ImageCollection CreateImageCollectionFromResources(string ResourceBmpName, System.Reflection.Assembly asm) {
			DevExpress.Utils.ImageCollection imageList = CreateVoidImageCollection();
			FillImageCollectionFromResources(imageList, ResourceBmpName, asm);
			return imageList;
		}
		public static void FillImageCollectionFromResources(DevExpress.Utils.ImageCollection images, string name, System.Reflection.Assembly asm) {
			Bitmap bitmap1 = ResourceImageHelper.CreateBitmapFromResources(name, asm);
			images.AddImageStrip(bitmap1);
		}
		public static void AddImagesToCollectionFromResources(DevExpress.Utils.ImageCollection images, string name, System.Reflection.Assembly asm) {
			using(DevExpress.Utils.ImageCollection tempCollection = CreateImageCollectionFromResources(name, asm)) {
				foreach(Image image in tempCollection.Images)
					images.AddImage(image);
			}
		}
	}
}
