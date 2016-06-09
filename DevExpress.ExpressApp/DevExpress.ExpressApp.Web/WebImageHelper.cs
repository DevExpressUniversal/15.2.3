#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       eXpressApp Framework                                        }
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
using System.Security.Cryptography;
using System.IO;
using System.Drawing.Imaging;
using System.Drawing;
namespace DevExpress.ExpressApp.Web {
	public class WebImageHelper {
		public static ImageFormat FindAppropriateImageFormat(ImageFormat imageFormat) {
			ImageFormat result = null;
			foreach(ImageCodecInfo info in ImageCodecInfo.GetImageEncoders()) {
				if(imageFormat.Guid == info.FormatID) {
					result = imageFormat;
				}
			}
			if(result == null || result == System.Drawing.Imaging.ImageFormat.MemoryBmp) {
				result = System.Drawing.Imaging.ImageFormat.Bmp;
			}
			return result;
		}
		public static byte[] ConvertImageToByteArray(Image image) {
			using(MemoryStream ms = new MemoryStream()) {
				image.Save(ms, FindAppropriateImageFormat(image.RawFormat));
				return ms.ToArray();
			}
		}
		public static string GetImageHash(Image image) {
			string result = string.Empty;
			if(image != null) {
				Byte[] imageByteArray = WebImageHelper.ConvertImageToByteArray(image);
				result = GetHashString(imageByteArray);
			}
			return result;
		}
		public static string GetHashString(Byte[] arrayToHash) {
			MD5 urlEncoder = MD5.Create();
			string mD5ComputeHash = "";
			foreach(byte byte_ in urlEncoder.ComputeHash(arrayToHash)) {
				mD5ComputeHash += byte_.ToString("X");
			}
			return mD5ComputeHash;
		}
		public static string GetHashString(string stringToHash) {
			return GetHashString(Encoding.UTF8.GetBytes(stringToHash));
		}
	}
}
