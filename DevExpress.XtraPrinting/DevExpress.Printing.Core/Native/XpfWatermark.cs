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
using DevExpress.XtraPrinting.Drawing;
using DevExpress.Utils.Serializing;
using System.IO;
#if SL
using DevExpress.Xpf.Drawing;
using System.Windows.Media.Imaging;
#else
using System.Drawing;
using System.Drawing.Imaging;
#endif
namespace DevExpress.XtraPrinting.Native {
	public class XpfWatermark : Watermark {
		byte[] imageArray;
		[XtraSerializableProperty(XtraSerializationVisibility.Hidden)]
		public override Image Image {
			get { return base.Image; }
			set { throw new ArgumentException(); }
		}
		[XtraSerializableProperty]
		public string ImageBase64 {
			get {
				if(imageArray == null || imageArray.Length == 0)
					return null;
				return Convert.ToBase64String(imageArray);
			}
			set {
				string imageBase64 = value;
				if(imageBase64 == null) {
					ImageArray = null;
					return;
				}
				byte[] decodedArray;
				try {
					decodedArray = Convert.FromBase64String(imageBase64);
				} catch {
					decodedArray = null;
				}
				ImageArray = decodedArray;
			}
		}
		public byte[] ImageArray {
			get {
				return imageArray;
			}
			set {
				if(imageArray == value)
					return;
				imageArray = value;
				if(imageArray != null && imageArray.Length != 0) {
					MemoryStream stream = new MemoryStream(imageArray);
#if SL
					BitmapImage bitmapImage = new BitmapImage();
					bitmapImage.SetSource(stream);
					WriteableBitmap writeableBitmap = new WriteableBitmap(bitmapImage);
					base.Image = Bitmap.FromWriteableBitmap(writeableBitmap);
#else
					base.Image = new Bitmap(stream);
#endif
				} else
					base.Image = null;
			}
		}
		public void CopyFrom(XpfWatermark watermark) {
			base.CopyFrom((XpfWatermark)watermark);
			ImageArray = watermark.imageArray;
		}
#if !SL
		public override void CopyFrom(Watermark watermark) {
			base.CopyFrom(watermark);
			if(watermark.Image != null) {
				using(var stream = new MemoryStream()) {
					watermark.Image.Save(stream, ImageFormat.Png);
					ImageArray = stream.ToArray();
				}
			}
		}
#endif
		public override bool Equals(object obj) {
			XpfWatermark watermarkProxy = obj as XpfWatermark;
			return watermarkProxy != null && base.Equals(obj) && watermarkProxy.ImageArray == ImageArray && watermarkProxy.ImageBase64 == ImageBase64;
		}
		public override int GetHashCode() {
			return base.GetHashCode();
		}
	}
}
