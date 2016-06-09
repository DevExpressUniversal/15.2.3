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
using EnvDTE;
using System.Reflection;
using System.IO;
namespace DevExpress.XtraReports.Design {
	public class DTEHelper {
		public static Assembly GetRawAssembly(string path) {
			if(!string.IsNullOrEmpty(path) && File.Exists(path)) {
				byte[] rawAssembly = File.ReadAllBytes(path);
				return Assembly.Load(rawAssembly);
			}
			return null;
		}
		public static string BuildProject(Project project) {
			try {
				OutputGroup group = project.ConfigurationManager.ActiveConfiguration.OutputGroups.Item("Built");
				if(group != null) {
					object[] fileURLs = group.FileURLs as object[];
					if(fileURLs != null && fileURLs.Length > 0) {
						string location = fileURLs[0] as string;
						return string.IsNullOrEmpty(location) ? GetLocalPathUnescaped(location) : string.Empty;
					}
				}
			} catch {
			}
			return string.Empty;
		}
		static string GetLocalPathUnescaped(string url) {
			string s = "file:///";
			if(url.StartsWith(s, StringComparison.OrdinalIgnoreCase)) {
				return url.Substring(s.Length);
			}
			return GetLocalPath(url);
		}
		static string GetLocalPath(string fileName) {
			Uri uri = new Uri(fileName);
			return (uri.LocalPath + uri.Fragment);
		}
	}
}
