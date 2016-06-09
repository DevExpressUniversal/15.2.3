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
using System.Text.RegularExpressions;
using System.Text;
namespace DevExpress.Office {
	#region HyperlinkUriHelper
	public static class HyperlinkUriHelper {
		const string FileNamePattern = @"^file:[\\/]*(?<path>(?<root>[A-Za-z]:[\\/]+)?[^:\*\?<>\|]+)$";
		const string UrlPattern = @"^((?<prefix>http|https|ftp|news)\:[\\/]*)(?<url>[a-zA-Z0-9\-\.]+(\.[a-zA-Z]{2,3})?(\:\d{1,5})?([\\/]\S*)?)$";
		const string RelativePathPattern = @"^(?:\.{1,2}[\\/])*[^:\*\?<>\|\.\\/][^:\*\?<>\|]*$";
		const string LocalPathPattern = @"^[A-Za-z]:[\\/]+[^:\*\?<>\|]*$";
		const string RemotePathPattern = @"^\\{2,}(?<path>[^:\*\?<>\|]+)$";
		const string MailAddressPattern = @"^mailto:(?<email>\S+)$";
		const string SlashPattern = @"[\\/]+";
		public static string EscapeHyperlinkFieldParameterString(string value) {
			return Replace(value, @"\\", @"\\");
		}
		public static string ConvertToHyperlinkUri(string uri) {
			return Replace(uri, @"\\", @"\\");
		}
		public static string PrepareHyperlinkTooltipQuotes(string value) {
			return Replace(value, "\"", "\\\"");
		}
		public static string ConvertFromHyperlinkUri(string uri) {
			return Replace(uri, @"\\\\", @"\");
		}
		public static string ConvertToUrl(string uri) {
			if (IsLocalPath(uri))
				return Regex.Replace(String.Format(@"file:///{0}", uri), "\\\\+", "/");
			if(IsRemotePath(uri))
				return Regex.Replace(String.Format(@"file://{0}", GetRemotePath(uri)), "\\\\+", "/");
			return uri;
		}
		static string GetRemotePath(string uri) {
			Regex regex = new Regex(RemotePathPattern);
			return regex.Match(uri).Groups["path"].Value;
		}
		public static string EnsureUriIsValid(string uri) {
			if (IsFileUri(uri))
				return EnsureFileUriIsValid(uri);
			if (IsUrl(uri))
				return EnsureUrlIsValid(uri);
			if (IsLocalPath(uri))
				return EnsureLocalPathIsValid(uri);
			if (IsRemotePath(uri))
				return EnsureRemotePathIsValid(uri);
			if (IsMailAddress(uri))
				return EnsureMailAddressIsValid(uri);
			if(IsRelativePath(uri))
				return EnsureRelativePathIsValid(uri);
			return uri;
		}
		static string EnsureFileUriIsValid(string uri) {
			Regex regex = new Regex(FileNamePattern);
			Match match = regex.Match(uri);
			string[] entries = Split(match.Groups["path"].Value, @"[\\/]+");
			string path = String.Join(@"\", entries);
			if (match.Groups["root"].Success)
				return path;
			return String.Format(@"\\{0}", path);
		}
		static string EnsureUrlIsValid(string uri) {
			Regex regex = new Regex(UrlPattern);
			Match match = regex.Match(uri);
			string url = match.Groups["url"].Value;
			string prefix = match.Groups["prefix"].Value;
			if (String.IsNullOrEmpty(prefix))
				prefix = "http";
			return String.Format(@"{0}://{1}", prefix, Regex.Replace(url, "\\\\+", "/"));
		}
		static string EnsureLocalPathIsValid(string uri) {
			string[] entries = Split(uri, @"[\\/]+");
			string result = String.Join(@"\", entries);
			if (entries.Length == 1)
				return result + "\\";
			return result;
		}
		static string EnsureRemotePathIsValid(string uri) {
			Regex regex = new Regex(RemotePathPattern);
			Match match = regex.Match(uri);
			string[] entries = Split(match.Groups["path"].Value, @"[\\/]+");
			return String.Format(@"\\{0}", String.Join(@"\", entries));
		}
		static string EnsureMailAddressIsValid(string uri) {
			Regex regex = new Regex(MailAddressPattern);
			Match match = regex.Match(uri);
			return String.Format(@"mailto:{0}", match.Groups["email"].Value);
		}
		static string EnsureRelativePathIsValid(string uri) {
			List<string> parts = new List<string>(Split(uri, SlashPattern));
			parts.RemoveAll(item => item.Equals(".", StringComparison.Ordinal));
			return String.Join(@"\", parts);
		}
		public static string ConvertRelativePathToAbsolute(string uri, string baseUri) {
			List<string> parts = new List<string>(Split(baseUri, SlashPattern));
			parts.RemoveAt(parts.Count - 1);
			parts.AddRange(Split(uri, SlashPattern));
			for(int i = 0; i < parts.Count; i++) {
				if(parts[i] == ".")
					parts.RemoveAt(i);
				else if(parts[i] == "..") {
					if(i == 0)
						parts.RemoveAt(0);
					else {
						parts.RemoveRange(i - 1, 2);
						i--;
					}
				}
				else
					i++;
			}
			return EnsureLocalPathIsValid(String.Join(@"\", parts));
		}
		internal static bool IsFileUri(string uri) {
			return IsMatch(uri, FileNamePattern);
		}
		internal static bool IsUrl(string uri) {
			return IsMatch(uri, UrlPattern);
		}
		internal static bool IsMailAddress(string uri) {
			return IsMatch(uri, MailAddressPattern);
		}
		public static bool IsLocalPath(string uri) {
			return IsMatch(uri, LocalPathPattern);
		}
		internal static bool IsRemotePath(string uri) {
			return IsMatch(uri, RemotePathPattern);
		}
		public static bool IsRelativePath(string uri) {
			return IsMatch(uri, RelativePathPattern);
		}
		internal static bool IsMatch(string uri, string pattern) {
			Regex regex = new Regex(pattern);
			return regex.IsMatch(uri);
		}
		internal static string Replace(string uri, string pattern, string replacement) {
			Regex regex = new Regex(pattern);
			return regex.Replace(uri, replacement);
		}
		internal static string[] Split(string uri, string pattern) {
			return Regex.Split(uri, pattern);
		}
	}
	#endregion
}
