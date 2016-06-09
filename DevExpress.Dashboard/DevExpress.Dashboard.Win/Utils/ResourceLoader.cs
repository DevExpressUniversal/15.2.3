#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       Dashboard                                                   }
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
using DevExpress.DashboardCommon;
using DevExpress.DashboardCommon.Native;
namespace DevExpress.DashboardWin.Native {
	public abstract class ResourceLoader {
		protected abstract object LoadResourceFromFile(string path);
		protected abstract object LoadResourceFromStream(Stream stream);
		protected object LoadResource(string resourceUrl, string sourceDirectory) {
			Uri uri = UrlResolver.CreateUri(resourceUrl, sourceDirectory ?? ".\\", UrlResolver.Instance);
			if (uri != null) {
				if (uri.IsFile)
					return LoadResourceFromFile(uri.LocalPath);
				using (WebClient webClient = new WebClient())
					using (Stream stream = webClient.OpenRead(uri.AbsoluteUri))
						if (stream != null)
							return LoadResourceFromStream(stream);
			}
			return null;
		}
	}
	public class DashboardLoader : ResourceLoader {
		protected override object LoadResourceFromFile(string path) {
			using (FileStream stream = new FileStream(path, FileMode.Open, FileAccess.Read))
				return LoadResourceFromStream(stream);
		}
		protected override object LoadResourceFromStream(Stream stream) {
			Dashboard dashboard = new Dashboard();
			try {
				dashboard.LoadFromXml(stream);
			}
			catch {
				dashboard.Dispose();
				throw;
			}
			return dashboard;
		}
		public Dashboard Load(Uri dashboardUri, string sourceDirectory) {
			return (Dashboard)LoadResource(dashboardUri.ToString(), sourceDirectory);
		}
	}
	public class ImageLoader : ResourceLoader {
		static Bitmap LoadImageFromStream(Stream stream) {
			using (Image image = Image.FromStream(stream))
				return BitmapCreator.CreateBitmap(image, Color.Transparent);
		}
		public static Bitmap FromData(byte[] data) {
			using (Stream stream = new MemoryStream(data))
				return LoadImageFromStream(stream);
		}
		protected override object LoadResourceFromFile(string path) {
			return Image.FromFile(path);
		}
		protected override object LoadResourceFromStream(Stream stream) {
			return LoadImageFromStream(stream);
		}
		public Image Load(string imageUrl) {
			return (Image)LoadResource(imageUrl, null);
		}
	}
}
