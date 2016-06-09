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
using System.IO;
using System.Net;
using DevExpress.Utils;
using DevExpress.XtraPrinting.Native;
namespace DevExpress.XtraReports.Native {
	public class ImageResolver {
		static Image CreateImage(Uri uri) {
			try {
				if(uri.IsFile)
					return CreateFromFile(uri.LocalPath);
			} catch { }
			try {
				using(WebClient webClient = new WebClient()) {
					using(Stream stream = webClient.OpenRead(uri.AbsoluteUri)) {
						if(stream != null) {
							using(Image image = Image.FromStream(stream)) {
								if(image != null)
									return BitmapCreator.CreateBitmap(image, Color.Transparent);
							}
						}
					}
				}
			} catch { }
			return null;
		}
		static Image CreateFromFile(string path) {
			if(!File.Exists(path))
				return null;
			string fullPath = Path.GetFullPath(path);
			byte[] bytes = File.ReadAllBytes(fullPath);
			return PSConvert.ImageFromArray(bytes);
		}
		public static Image GetImage(string url, string sourceDirectory, UrlResolver urlResolver) {
			if(!string.IsNullOrEmpty(url)) {
				try {
					Uri uri = UrlResolver.CreateUri(url, sourceDirectory, urlResolver);
					if(uri != null)
						return CreateImage(uri);
				} catch { }
			}
			return null;
		}
	}
}
