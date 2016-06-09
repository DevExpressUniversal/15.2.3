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
using System.IO;
using System.Security;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
namespace DevExpress.Pdf.Native {
	public static class PdfDCTDecoder {
		[SecuritySafeCritical]
		public static PdfDCTDecodeResult Decode(byte[] imageData, int imageWidth, int imageHeight) {
			using (MemoryStream stream = new MemoryStream(imageData))
				using (Bitmap bitmap = new Bitmap(stream)) {
					int sourceStride;
					PixelFormat pixelFormat = bitmap.PixelFormat;
					byte[] data;
					BitmapData bitmapData = bitmap.LockBits(new Rectangle(0, 0, imageWidth, imageHeight), ImageLockMode.ReadOnly, pixelFormat);
					try {
						IntPtr ptr = bitmapData.Scan0;
						sourceStride = bitmapData.Stride;
						int count = sourceStride * imageHeight;
						data = new byte[count];
						Marshal.Copy(ptr, data, 0, count);
					}
					finally {
						bitmap.UnlockBits(bitmapData);
					}
					return new PdfDCTDecodeResult(data, sourceStride);
				}
		}
	}
}
