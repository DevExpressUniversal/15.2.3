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
using DevExpress.XtraPrinting.Native;
using System.Reflection;
using System.IO;
namespace DevExpress.Utils {
	public class UrlResolver {
		static readonly object lockInstance = new object();
		static UrlResolver instance;
		public static UrlResolver Instance {
			get {
				lock(lockInstance) {
					if(instance == null)
						instance = new UrlResolver();
					return instance;
				}
			}
		}
		public static string ResolveUrl(string url) {
			if(!string.IsNullOrEmpty(url) && url.StartsWith("~") && PSNativeMethods.AspIsRunning) {
				string virtPath = HttpRuntimeAccessor.AppDomainAppVirtualPath.TrimEnd('/');
				url = string.Concat(virtPath, "/", TrimUrl(url));
			}
			return url;
		}
		public static string TrimUrl(string url) {
			return url.TrimStart('~', Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
		}
		static Uri CreateUri(string path) {
			try {
				return new Uri(path);
			}
			catch(UriFormatException) {
				return new Uri(Path.GetFullPath(path));
			}
		}
		public static Uri CreateUri(string url, string sourceDirectory, UrlResolver urlResolver) {
			if(string.IsNullOrEmpty(url))
				return null;
			if(urlResolver.CanMapPath && (url.StartsWith("~") || url.StartsWith(@"/")))
				url = urlResolver.MapPath(url);
			else if(url.StartsWith("~"))
				url = Path.Combine(sourceDirectory, UrlResolver.TrimUrl(url));
			return CreateUri(url);
		}
		MethodInfo mapPathMethod;
		public bool CanMapPath {
			get { return Server != null; }
		}
		protected virtual object Server {
			get { return HttpContextAccessor.Server; }
		}
		protected virtual Uri Url {
			get { return HttpContextAccessor.Url; }
		}
		MethodInfo MapPathMethod {
			get {
				if(mapPathMethod == null) {
					Type type = Server.GetType();
					mapPathMethod = type.GetMethod("MapPath", BindingFlags.Instance | BindingFlags.Public);
				}
				return mapPathMethod;
			}
		}
		public virtual string MapPath(string url) {
			try {
				string path = (string)MapPathMethod.Invoke(Server, new object[] { url });
				if(File.Exists(path))
					return path;
			} catch {}
			string s = ResolveUrl(url);
			return string.Concat(Url.GetLeftPart(UriPartial.Authority), "/", TrimUrl(s));
		}
	}
}
