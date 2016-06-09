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
using System.ComponentModel.Design;
using System.Reflection;
using DevExpress.XtraReports.Design.Commands;
using DevExpress.Utils.Design;
public static class OleMenuCommandHelper {
	public static MenuCommand CreateOleMenuCommand(IServiceProvider serviceProvider, EventHandler eventHandler, CommandID commandID) {
		try {
			Assembly assembly = GetAssembly(serviceProvider);
			Type oleMenuCommandType = assembly.GetType("Microsoft.VisualStudio.Shell.OleMenuCommand");
			ConstructorInfo ci = oleMenuCommandType.GetConstructor(new Type[] { typeof(EventHandler), typeof(CommandID) });
			return (MenuCommand)ci.Invoke(new object[] { eventHandler, commandID });
		} catch {
			throw new Exception("Failed to create OleMenuCommand");
		}
	}
	public static bool IsOleMenuCmdEventArgs(EventArgs e) {
		return e.GetType().FullName == "Microsoft.VisualStudio.Shell.OleMenuCmdEventArgs";
	}
	public static object GetInValue(EventArgs e) {
		return e.GetType().InvokeMember("InValue", BindingFlags.GetProperty | BindingFlags.Public | BindingFlags.Instance, null, e, new object[0]);
	}
	public static IntPtr GetOutValue(EventArgs e) {
		return (IntPtr)e.GetType().InvokeMember("OutValue", BindingFlags.GetProperty | BindingFlags.Public | BindingFlags.Instance, null, e, new object[0]);
	}
	static Assembly GetAssembly(IServiceProvider serviceProvider) {
		string assemblyName = "Microsoft.VisualStudio.Shell, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a";
		Version version = VSVersions.GetVersion(serviceProvider);
		if(version >= VSVersions.VS2010)
			assemblyName = string.Format("Microsoft.VisualStudio.Shell.{0}.{1}, Version={0}.{1}.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", version.Major, version.Minor);
		return Assembly.Load(new AssemblyName(assemblyName));
	}
}
