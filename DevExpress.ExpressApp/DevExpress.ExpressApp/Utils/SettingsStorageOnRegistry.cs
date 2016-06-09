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
using System.Security.AccessControl;
using Microsoft.Win32;
using System.Collections.Generic;
using DevExpress.Persistent.Base;
namespace DevExpress.ExpressApp.Utils {
	public class SettingsStorageOnRegistry : SettingsStorage {
		private string registryKeyName;
		public SettingsStorageOnRegistry(string registryKeyName) : base() {
			this.registryKeyName = registryKeyName;
			if (!registryKeyName.EndsWith("\\")) {
				registryKeyName += "\\";
			}
		}
		public override bool IsPathExist(string optionPath) {
			RegistryKey registryKey = Registry.CurrentUser.OpenSubKey(registryKeyName + "\\" + optionPath, RegistryKeyPermissionCheck.Default, RegistryRights.WriteKey);
			if (registryKey != null) {
				return true;
			}
			return false;
		}
		public override void SaveOption(string optionPath, string optionName, string optionValue) {
			RegistryKey registryKey = Registry.CurrentUser.CreateSubKey(registryKeyName + "\\" + optionPath);
			registryKey.SetValue(optionName, optionValue);
		}
		public override string LoadOption(string optionPath, string optionName) {
			string optionValue = string.Empty;
			RegistryKey registryKey = Registry.CurrentUser.OpenSubKey(registryKeyName + "\\" + optionPath);
			if (registryKey != null) {
				optionValue = registryKey.GetValue(optionName, string.Empty).ToString();
			}
			return optionValue;
		}
	}
}
