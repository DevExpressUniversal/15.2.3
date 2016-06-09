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
using System.Collections;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Web;
using DevExpress.ExpressApp.Utils;
namespace DevExpress.ExpressApp.Web {
	public class ImageInfoEventArgs : EventArgs {
		private DevExpress.ExpressApp.Utils.ImageInfo imageInfo;
		public DevExpress.ExpressApp.Utils.ImageInfo ImageInfo {
			get { return imageInfo; }
			set { imageInfo = value; }
		}
		private string url;
		public string Url {
			get { return url; }
		}
		private string urlHashCode;
		public string UrlHashCode {
			get { return urlHashCode; }
		}
		public ImageInfoEventArgs(string url, string urlHashCode) {
			this.url = url;
			this.urlHashCode = urlHashCode;
		}
	}
	public class ImageResourceHttpHandler : IXafHttpHandler {
		private const string ImageLoaderImageNameParameterName = "name";
		private const string ImageLoaderEnabledParameterName = "enbl";
		private const string ImageLoaderFolderParameterName = "fldr";
		private const string ImageLoaderContentVersionParameterName = "v"; 
		private const string HandlerUrl = "DXX.axd?handlerName=" + HandlerName;
		private const string HandlerName = "ImageResource";
		private readonly object lockObject = new object();
		private static bool encodeImageUrl = true;
		public static bool EncodeImageUrl {
			get { return encodeImageUrl; }
			set { encodeImageUrl = value; }
		}
		private static IDictionary urlCache = Hashtable.Synchronized(new Hashtable());
		private static void Instance_CustomizeImageInfo(object sender, CustomizeImageInfoEventArgs e) {
			if(string.IsNullOrEmpty(e.ImageInfo.ImageUrl) || e.ImageInfo.ImageUrl == ImageInfo.NullImageUrlValue) {
				e.ImageInfo = new ImageInfo(e.ImageName, e.ImageInfo.Image,
					HandlerUrl
					+ "&" + ImageLoaderImageNameParameterName + "=" + HttpUtility.UrlEncode(e.ImageName)
					+ "&" + ImageLoaderEnabledParameterName + "=" + e.IsEnabled
					+ "&" + ImageLoaderFolderParameterName + "=" + e.ImageFolder
					+ "&" + ImageLoaderContentVersionParameterName + "=" + e.ImageInfo.MD5Hash);
			}
		}
		[Obsolete("Don't use this method")]
		public static string GetWebResourceUrl(AssemblyResourceImageSource imageSource, string imageName, string imageFolder, string MD5Hash) {
			return GetWebResourceUrl(imageSource, imageName, true, imageFolder, MD5Hash);
		}
		[Obsolete("Don't use this method")]
		public static string GetWebResourceUrl(AssemblyResourceImageSource imageSource, string imageName, bool enabled, string imageFolder, string MD5Hash) {
			return GetWebResourceUrl(String.Format("assemblyName:{0},assemblyVersion:{1},imagename:{2},enabled:{3},imageFolder:{4},MD5:{5}", imageSource.AssemblyName, imageSource.AssemblyVersion, imageName, enabled, imageFolder, MD5Hash));
		}
		public static string GetWebResourceUrl(string queryImageInfoEventKey) {
			string imageDescription = queryImageInfoEventKey;
			if(encodeImageUrl) {
				imageDescription = WebImageHelper.GetHashString(queryImageInfoEventKey);
			}
			lock(urlCache.SyncRoot) {
				if(!urlCache.Contains(imageDescription)) {
					urlCache.Add(imageDescription, queryImageInfoEventKey);
				}
			}
			return HandlerUrl + "&d=" + HttpUtility.UrlEncode(imageDescription);
		}
		public static void AddImageLoaderHandler() {
			ImageLoader.Instance.CustomizeImageInfo -= new EventHandler<CustomizeImageInfoEventArgs>(Instance_CustomizeImageInfo);
			ImageLoader.Instance.CustomizeImageInfo += new EventHandler<CustomizeImageInfoEventArgs>(Instance_CustomizeImageInfo);
		}
		public static void RemoveImageLoaderHandler() {
			ImageLoader.Instance.CustomizeImageInfo -= new EventHandler<CustomizeImageInfoEventArgs>(Instance_CustomizeImageInfo);
		}
		public static event EventHandler<ImageInfoEventArgs> QueryImageInfo;
		public void ProcessRequest(HttpContext context) {
			lock(lockObject) {
				DevExpress.ExpressApp.Utils.ImageInfo imageInfo = DevExpress.ExpressApp.Utils.ImageInfo.Empty;
				string imageLoaderImageName = context.Request.QueryString[ImageLoaderImageNameParameterName];
				if(!string.IsNullOrEmpty(imageLoaderImageName)) {
					bool imageLoaderEnabled;
					if(!bool.TryParse(context.Request.QueryString[ImageLoaderEnabledParameterName], out imageLoaderEnabled)) {
						imageLoaderEnabled = true;
					}
					string imageLoaderFolder = context.Request.QueryString[ImageLoaderFolderParameterName];
					imageInfo = ImageLoader.Instance.GetImageInfo(imageLoaderImageName, imageLoaderEnabled, imageLoaderFolder);
				}
				else {
					string urlHashCode = context.Request.QueryString["d"];
					if(!string.IsNullOrEmpty(urlHashCode) && (QueryImageInfo != null)) {
						String imageName = (urlCache.Contains(urlHashCode) ? urlCache[urlHashCode] : string.Empty) as String;
						ImageInfoEventArgs args = new ImageInfoEventArgs(imageName, urlHashCode);
						QueryImageInfo(this, args);
						imageInfo = args.ImageInfo;
					}
				}
				Image resultImage;
				if(imageInfo == DevExpress.ExpressApp.Utils.ImageInfo.Empty) {
					Size size = new Size(24, 24);
					Bitmap errorImage = new Bitmap(size.Width, size.Height);
					Graphics graphics = Graphics.FromImage(errorImage);
					graphics.FillRectangle(Brushes.White, new Rectangle(new Point(), errorImage.Size));
					Pen pen = new Pen(Color.Red, 4);
					graphics.DrawLine(pen, new Point(), new Point(size));
					graphics.DrawLine(pen, new Point(size.Width, 0), new Point(0, size.Height));
					resultImage = errorImage;
				}
				else {
					resultImage = imageInfo.Image;
				}
				lock(resultImage) {
					context.Response.BinaryWrite(DevExpress.Utils.HtmlImageHelper.ImageToArray(resultImage));
					context.Response.ContentType = "image/" + DevExpress.Utils.HtmlImageHelper.GetMimeType(resultImage);
					context.Response.Cache.SetExpires(DateTime.Now.AddYears(1));
					context.Response.Cache.SetLastModified(DateTime.Now.ToUniversalTime());
				}
			}
		}
		public bool CanProcessRequest(HttpRequest request) {
			return request.QueryString["handlerName"] == HandlerName;
		}
		public System.Web.SessionState.SessionStateBehavior SessionClientMode {
			get { return System.Web.SessionState.SessionStateBehavior.Disabled; }
		}
		#region Obsolete 15.1
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		[SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters")]
		[Obsolete("Use the GetWebResourceUrl(AssemblyResourceImageSource imageSource, string imageName, string imageFolder, string MD5Hash) method instead.", true)]
		public static string GetWebResourceUrl(AssemblyResourceImageSource imageSource, string imageName) {
			throw new NotImplementedException();
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		[SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters")]
		[Obsolete("Use the GetWebResourceUrl(AssemblyResourceImageSource imageSource, string imageName, bool enabled, string imageFolder, string MD5Hashd) method instead.", true)]
		public static string GetWebResourceUrl(AssemblyResourceImageSource imageSource, string imageName, bool enabled) {
			throw new NotImplementedException();
		}
		#endregion
	}
}
