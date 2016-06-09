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

using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using System.Security;
using System.Runtime.InteropServices;
using DevExpress.Pdf.Native;
namespace DevExpress.Pdf.Drawing {
	public class EmfPlusImage : PdfDisposableObject {
		readonly Image image;
		public EmfPlusImage(EmfPlusReader reader) {
			reader.ReadInt32();
			EmfPlusImageDataType imageType = EmfEnumToValueConverter.ParseEmfEnum<EmfPlusImageDataType>(reader.ReadInt32());
			switch (imageType) {
				case EmfPlusImageDataType.ImageDataTypeBitmap:
					int widht = reader.ReadInt32();
					int height = reader.ReadInt32();
					int stride = reader.ReadInt32();
					int pixelFormatValue = reader.ReadInt32();
					PixelFormat pixelFormat = (pixelFormatValue != (int)EmfPlusPixelFormat.PixelFormat64bppPARGB) ? (PixelFormat)pixelFormatValue : PixelFormat.Format64bppPArgb;
					bool isCompressed = reader.ReadInt32() != 0;
					byte[] imageData = reader.ReadBytes((int)(reader.BaseStream.Length - reader.BaseStream.Position));
					if (isCompressed) {
						MemoryStream stream = new MemoryStream(imageData);
						image = new Bitmap(stream);
					}
					else
						image = CreateBitmap(widht, height, stride, pixelFormat, imageData);
					break;
				case EmfPlusImageDataType.ImageDataTypeMetafile:
					EmfPlusMetafileDataType type = EmfEnumToValueConverter.ParseEmfEnum<EmfPlusMetafileDataType>(reader.ReadInt32());
					if (type == EmfPlusMetafileDataType.MetafileDataTypeWmfPlaceable)
						reader.ReadBytes(24);
					byte[] content = reader.ReadBytes(reader.ReadInt32());
					image = Image.FromStream(new MemoryStream(content));
					break;
				default:
					break;
			}
		}
		public Image GetImage() {
			return image;
		}
		protected override void Dispose(bool disposing) {
			if (disposing)
				image.Dispose();
		}
		[SecuritySafeCritical]
		Bitmap CreateBitmap(int widht, int height, int stride, PixelFormat pixelFormat, byte[] bitmapData) {
			bool isIndexed = (pixelFormat & PixelFormat.Indexed) != PixelFormat.Undefined;
			Bitmap img = new Bitmap(widht, height, pixelFormat);
			int index = 0;
			if (isIndexed) {
				using (BinaryReader reader = new BinaryReader(new MemoryStream(bitmapData))) {
					int flags = reader.ReadInt32();
					int count = reader.ReadInt32();
					ColorPalette pallete = img.Palette;
					Color[] entries = pallete.Entries;
					for (int j = 0; j < count; j++)
						entries[j] = Color.FromArgb(reader.ReadInt32());
					index += 4 * (2 + count);
					img.Palette = pallete;
				}
			}
			BitmapData bmpBits = img.LockBits(new Rectangle(0, 0, widht, height), ImageLockMode.WriteOnly, pixelFormat);
			Marshal.Copy(bitmapData, index, bmpBits.Scan0, stride * height);
			img.UnlockBits(bmpBits);
			return img;
		}
	}
}
