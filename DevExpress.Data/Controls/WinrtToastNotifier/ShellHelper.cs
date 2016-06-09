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
using System.Diagnostics;
using System.IO;
using DevExpress.Internal.WinApi;
namespace DevExpress.Data {
	public static class ShellHelper {
		public static void TryCreateShortcut(string applicationId, string name, string iconPath = null) {
			string shortcutPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Programs), name + ".lnk");
			if(!File.Exists(shortcutPath))
				InstallShortcut(shortcutPath, applicationId, iconPath);
		}
		public static void TryCreateShortcut(string sourcePath, string applicationId, string name, string iconPath = null) {
			string shortcutPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Programs), name + ".lnk");
			if(!File.Exists(shortcutPath))
				InstallShortcut(sourcePath, shortcutPath, applicationId, iconPath);
		}
		public static void RemoveShortcut(string name) {
			string shortcutPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Programs), name + ".lnk");
			if(!File.Exists(shortcutPath))
				File.Delete(shortcutPath);
		}
		static void InstallShortcut(string shortcutPath, string applicationId, string iconPath) {
			string exePath = Process.GetCurrentProcess().MainModule.FileName;
			InstallShortcut(exePath, shortcutPath, applicationId, iconPath);
		}
		static void InstallShortcut(string sourcePath, string shortcutPath, string applicationId, string iconPath) {
			IShellLinkW newShortcut = (IShellLinkW)new CShellLink();
			ErrorHelper.VerifySucceeded(newShortcut.SetPath(sourcePath));
			ErrorHelper.VerifySucceeded(newShortcut.SetArguments(""));
			IPropertyStore newShortcutProperties = (IPropertyStore)newShortcut;
			if(iconPath != null) {
				newShortcut.SetIconLocation(iconPath, 0);
			}
			using(PropVariant appId = new PropVariant(applicationId)) {
				PropertyKey SystemProperties_System_AppUserModel_ID = new PropertyKey(new Guid("{9F4C2855-9F79-4B39-A8D0-E1D42DE1D5F3}"), 5);
				ErrorHelper.VerifySucceeded(newShortcutProperties.SetValue(SystemProperties_System_AppUserModel_ID, appId));
				ErrorHelper.VerifySucceeded(newShortcutProperties.Commit());
			}
			IPersistFile newShortcutSave = (IPersistFile)newShortcut;
			ErrorHelper.VerifySucceeded(newShortcutSave.Save(shortcutPath, true));
		}
	}
}
