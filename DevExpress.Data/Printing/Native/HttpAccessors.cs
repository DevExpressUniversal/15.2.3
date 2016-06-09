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
using System.Reflection;
using System.IO;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Security;
namespace DevExpress.XtraPrinting.Native {
	internal static class SystemWebAssemblyLoader {
		static SystemWebAssemblyLoader() {
			int clrMajorVersion = 2;
			try {
				clrMajorVersion = GetClrMajorVersion();
			} catch(SecurityException) {
			}
			if(clrMajorVersion == 2)
				systemWeb = Load("System.Web, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a");
			if(clrMajorVersion == 4 || systemWeb == null)
				systemWeb = Load("System.Web, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a");
		}
		static Assembly Load(string name) {
			try {
				return Assembly.Load(name);
			} catch(FileNotFoundException) {
				return null;
			}
		}
		static int GetClrMajorVersion() {
			int clrVersion = 0;
			string versionStr = RuntimeEnvironment.GetSystemVersion();
			Regex regex = new Regex("[0-9]", RegexOptions.Compiled | RegexOptions.Singleline);
			Match match = regex.Match(versionStr);
			if(match.Success)
				int.TryParse(match.Value, out clrVersion);
			return clrVersion;
		}
		static readonly Assembly systemWeb;
		public static Assembly SystemWeb {
			get { return systemWeb; }
		}
	}
	public static class HttpRuntimeAccessor {
		public static readonly string AppDomainAppVirtualPath;
		static HttpRuntimeAccessor() {
			if(SystemWebAssemblyLoader.SystemWeb == null)
				return;
			try {
				Type type = SystemWebAssemblyLoader.SystemWeb.GetType("System.Web.HttpRuntime");
				PropertyInfo property = type.GetProperty("AppDomainAppVirtualPath", BindingFlags.Static | BindingFlags.Public);
				AppDomainAppVirtualPath = (string)property.GetValue(null, new object[0]);
			} catch {
			}
		}
	}
	public static class HttpContextAccessor {
		static readonly PropertyInfo property;
		public static object Current {
			get {
				try {
					return property != null ? property.GetValue(null, new object[0]) : null;
				}
				catch {
					return null;
				}
			}
		}
		public static object Server {
			get { return GetPropertyValue(Current, "Server"); }
		}
		public static object Request {
			get { return GetPropertyValue(Current, "Request"); }
		}
		public static Uri Url {
			get { return (Uri)GetPropertyValue(Request, "Url"); }
		}
		static object GetPropertyValue(object obj, string name) {
			if(obj != null) {
				PropertyInfo property = obj.GetType().GetProperty(name, BindingFlags.Instance | BindingFlags.Public);
				return property.GetValue(obj, new object[0]);
			}
			return null;
		}
		static HttpContextAccessor() {
			if(SystemWebAssemblyLoader.SystemWeb == null)
				return;
			try {
				Type type = SystemWebAssemblyLoader.SystemWeb.GetType("System.Web.HttpContext");
				property = type.GetProperty("Current", BindingFlags.Static | BindingFlags.Public);
			} catch {
			}
		}
	}
}
