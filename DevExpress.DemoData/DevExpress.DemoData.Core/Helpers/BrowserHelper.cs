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
using System.Windows;
using System.Windows.Interop;
using System.Text;
using System.Deployment.Application;
namespace DevExpress.DemoData.Helpers {
	public static class BrowserHelper {
		static object locker = new object();
		static string applicationSource;
		static bool applicationSourceLoaded = false;
		static string applicationUrl;
		static string domainUrl;
		public static string DomainUrl {
			get {
				lock(locker) {
					if(!string.IsNullOrEmpty(domainUrl)) return domainUrl;
					string[] urlParts = GetUrlBaseParts(ApplicationUrl);
					domainUrl = Combine(urlParts, 0, 2);
					return domainUrl;
				}
			}
		}
		public static string GetRootUrl(params string[] folders) {
			for(int i = 0; i < folders.Length - 1; ++i) {
				string root = GetRootUrl(folders[i], true);
				if(root != null) return root;
			}
			return GetRootUrl(folders[folders.Length - 1]);
		}
		public static string GetRootUrl(string folder) {
			return GetRootUrl(folder, false);
		}
		public static string GetRootUrl(string folder, bool returnNullIfFolderNotFound) {
			return GetRootUrl(ApplicationUrl, folder, returnNullIfFolderNotFound);
		}
		public static string GetRootUrl(string applicationUrl, string folder, bool returnNullIfFolderNotFound) {
			string[] urlParts = GetUrlBaseParts(applicationUrl);
			int clientBinIndex = FindPart(urlParts, folder);
			if(clientBinIndex < 0) {
				if(returnNullIfFolderNotFound)
					return null;
				else
					clientBinIndex = urlParts.Length;
			}
			return Combine(urlParts, 0, clientBinIndex);
		}
		static string ApplicationUrl {
			get {
				lock(locker) {
					if(!string.IsNullOrEmpty(applicationUrl)) return applicationUrl;
					applicationUrl = ApplicationSource;
					return applicationUrl;
				}
			}
		}
		static string ApplicationSource {
			get {
				if(applicationSourceLoaded) return applicationSource;
				applicationSourceLoaded = true;
				BackgroundHelper.DoInMainThread(() => {
					try {
						Uri sourceUri;
						if(EnvironmentHelper.IsClickOnce) {
							sourceUri = ApplicationDeployment.CurrentDeployment.UpdateLocation;
						} else {
							sourceUri = BrowserInteropHelper.Source;
						}
						applicationSource = sourceUri == null ? string.Empty : sourceUri.ToString();
					} catch {
						applicationSource = string.Empty;
					}
				});
				return applicationSource;
			}
		}
		static string[] GetUrlBaseParts(string url) {
			url = RemoveEnd(url, '#');
			url = RemoveEnd(url, '?');
			string protocol = string.Empty;
			int p = url.IndexOf("://", StringComparison.Ordinal);
			if(p >= 0) {
				protocol = Remove(url, p + 3);
				url = Substring(url, p + 3);
			}
			List<string> parts = new List<string>();
			parts.Add(protocol);
			while(true) {
				int e = url.IndexOf('/');
				if(e < 0) break;
				parts.Add(Remove(url, e + 1));
				url = Substring(url, e + 1);
			}
			return parts.ToArray();
		}
		static string RemoveEnd(string url, char p) {
			int i = url.IndexOf(p);
			return i < 0 ? url : url.SafeRemove(i);
		}
		static int FindPart(string[] parts, string p) {
			for(int i = parts.Length; --i >= 0; ) {
				if(string.Equals(parts[i].TrimEnd('/'), p, StringComparison.InvariantCultureIgnoreCase)) return i;
			}
			return -1;
		}
		static string Combine(string[] parts, int start, int end) {
			StringBuilder ret = new StringBuilder();
			for(int i = start; i < end; ++i) {
				if(i >= 0 && i < parts.Length) {
					ret.Append(parts[i]);
				}
			}
			return ret.ToString();
		}
		static string Remove(string s, int startIndex) {
			return startIndex == s.Length ? s : s.Remove(startIndex);
		}
		static string Substring(string s, int startIndex) {
			return startIndex == s.Length ? string.Empty : s.Substring(startIndex);
		}
	}
}
