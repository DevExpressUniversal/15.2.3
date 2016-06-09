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
using System.Deployment.Application;
using System.Diagnostics;
using System.IO;
using DevExpress.Internal;
using Microsoft.Win32;
namespace DevExpress.DemoData.Core {
	public class StartClickOnceDemoHelper {
		const string applicationDisplayName = "DevExpress WPF Demos";
		const string pathPattern = "{0}\\Developer Express Inc\\{1}\\{1}.appref-ms";
		public static void Start(string args) {
			Start(ApplicationDeployment.CurrentDeployment.UpdatedApplicationFullName.Split('#')[1].Split('.')[0], args);
		}
		public static void Start(string appName, string args) {
			string path = BuildRefPath();
			if(!File.Exists(path)) {
				CreateRef(path);
			}
			Process.Start(new ProcessStartInfo(path, args));
		}
		static void CreateRef(string path) {
			RegistryKey baseKey = Registry.CurrentUser.OpenSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Uninstall");
			foreach(string currentSubKeyName in baseKey.GetSubKeyNames()) {
				RegistryKey subKey = baseKey.OpenSubKey(currentSubKeyName);
				string displayNameValue = subKey.GetValue("DisplayName") as string;
				string versionValue = subKey.GetValue("DisplayVersion") as string;
				if(!string.IsNullOrEmpty(displayNameValue) && displayNameValue == applicationDisplayName && !string.IsNullOrEmpty(versionValue) && versionValue.StartsWith(AssemblyInfo.VersionShort)) {
					string appIdValue = subKey.GetValue("ShortcutAppId") as string;
					using(StreamWriter writer = new StreamWriter(path)) {
						foreach(char charItem in appIdValue) {
							writer.Write(charItem);
							writer.Write((char)0x00);
						}
						writer.Close();
					}
					break;
				}
			}
		}
		static string BuildRefPath() {
			return string.Format(pathPattern, Environment.GetFolderPath(Environment.SpecialFolder.Programs), applicationDisplayName);
		}
	}
}
