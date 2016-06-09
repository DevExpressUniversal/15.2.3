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

using System.Collections;
using DevExpress.Utils.Zip;
using DevExpress.XtraPrinting.Native;
#if SILVERLIGHT
using DevExpress.Xpf.Drawing;
using DevExpress.Xpf.Drawing.Imaging;
using DevExpress.XtraPrinting.Stubs;
#else
using System.Drawing;
using System.Drawing.Imaging;
using System;
using System.Collections.Generic;
#endif
namespace DevExpress.Utils {
	public class HtmlImageHelper {
		private static Dictionary<Guid, string> mimeHT = null;
		static HtmlImageHelper() {
			mimeHT = new Dictionary<Guid, string>();
			ImageFormat[] formats = {ImageFormat.Bmp, ImageFormat.Gif, ImageFormat.Jpeg, ImageFormat.Png, ImageFormat.Tiff };
			string[] mimes = { "bmp", "gif", "jpg", "png", "tiff" };
			for(int i = 0; i < formats.Length; i++)
				mimeHT.Add(GetKey(formats[i]), mimes[i]);
		}
		public static long GetImageHashCode(Image img) {
			if(img == null)
				return 0;
			return Adler32.CalculateChecksum(HtmlImageHelper.ImageToArray(img));
		}
		public static string GetMimeType(Image img) {
			string val = GetValue(img.RawFormat);
			return val != null ? val : "png";
		}
#if !SILVERLIGHT
		public static void SaveImage(Image image, string path) {
			image.Save(path, GetImageFormat(image));
		}
#endif
		public static byte[] ImageToArray(Image img) {
			return PSConvert.ImageToArray(img, GetImageFormat(img));
		}
		private static ImageFormat GetImageFormat(Image img) {
			if (img is Metafile)
				return ImageFormat.Emf;
			string val = GetValue(img.RawFormat);
			return val != null ? img.RawFormat : ImageFormat.Png;
		}
		private static string GetValue(ImageFormat format) {
			string result;
			mimeHT.TryGetValue(GetKey(format), out result);
			return result;
		}
		private static Guid GetKey(ImageFormat format) {
			return format.Guid;
		}
	}
}
