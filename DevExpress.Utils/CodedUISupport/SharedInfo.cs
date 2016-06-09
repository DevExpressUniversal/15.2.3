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
using System.Runtime.InteropServices;
namespace DevExpress.Utils.CodedUISupport {
	internal enum CodedUICommands : int {
		NotIdentified = 0,
		IdentifyDxControl = 1,
		ParentHandleRequest = 2,
		InitIpcConnection = 4,
		SaveUtilsAssemblyPath = 8,
		DisableControlAfterProcessing = 16
	}
	internal enum DXSupportLevels : int {
		Supported = 200,
		NotSupported = 0
	}
	internal class SharedMembers {
		const string UtilsAssemblyPathFileName = "UtilsAssemblyPathForCodedUIExtension";
		const string CodedUIMessageString = "DevExpressCodedUIMessage";
		internal static string UtilsAssemblyPathFilePath {
			get {
				string tempFolderPath = Environment.GetEnvironmentVariable("Temp");
				return tempFolderPath + "\\" + SharedMembers.UtilsAssemblyPathFileName;
			}
		}
		static int devExpressCodedUIMessage = 0;
		internal static int DevExpressCodedUIMessage {
			get {
				if(devExpressCodedUIMessage == 0)
					devExpressCodedUIMessage = GetCodedUIExtensionMessage(CodedUIMessageString + AssemblyInfo.VersionShort);
				return devExpressCodedUIMessage;
			}
		}
		[System.Security.SecuritySafeCritical]
		static int GetCodedUIExtensionMessage(string message) {
			return (int)RegisterWindowMessage(message);
		}
		[DllImport("user32.dll", CharSet = CharSet.Unicode)]
		static extern uint RegisterWindowMessage(string lpProcName);
	}
	[Serializable]
	internal struct SearchProperty {
		internal SearchProperty(string value, bool contains) {
			this.Name = null;
			this.Value = value;
			this.Contains = contains;
			Int32.TryParse(value, out this.ValueAsInt);
		}
		internal SearchProperty(string propertyName, string value, bool contains) {
			this.Name = propertyName;
			this.Value = value;
			this.Contains = contains;
			Int32.TryParse(value, out this.ValueAsInt);
		}
		public string Name;
		public string Value;
		public int ValueAsInt;
		public bool Contains;
		public bool Compare(string value) {
			if(value == null || this.Value == null)
				return value == this.Value;
			else if(this.Contains)
				return value.Contains(this.Value);
			else
				return value.Equals(this.Value);
		}
		public bool Compare(SearchProperty other) {
			if(other.Value == null || this.Value == null)
				return this.Value == other.Value;
			else if(this.Contains) {
				if(other.Contains)
					return other.Value.Contains(this.Value) || this.Value.Contains(other.Value);
				else
					return other.Value.Contains(this.Value);
			} else if(other.Contains)
				return this.Value.Contains(other.Value);
			else
				return other.Value.Equals(this.Value);
		}
	}
}
