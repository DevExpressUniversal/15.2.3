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
using System.Linq;
using System.Text;
using System.Reflection;
using System.IO;
namespace DevExpress.XtraReports.Wizards {
	class ConfigFileHelper {
		string fileName;
		string filePath = null;
		string PreviousFilePath {
			get {
				Assembly assembly = Assembly.GetEntryAssembly();
				return assembly != null ? Path.Combine(Path.GetDirectoryName(assembly.Location), fileName) : string.Empty;
			}
		}
		string FilePath {
			get {
				if(filePath == null) {
					string folderPath = GetFolderPath();
					filePath = !string.IsNullOrEmpty(folderPath) ? Path.Combine(folderPath, fileName) : string.Empty;
				}
				return filePath;
			}
		}
		public ConfigFileHelper(string fileName) {
			this.fileName = fileName;
		}
		public string GetLoadFilePath() {
			return File.Exists(PreviousFilePath) ? PreviousFilePath :
				File.Exists(FilePath) ? FilePath :
				string.Empty;
		}
		public string GetSaveFilePath() {
			return !string.IsNullOrEmpty(FilePath) ? FilePath :
				!string.IsNullOrEmpty(PreviousFilePath) ? PreviousFilePath :
				string.Empty;
		}
		public void DeletePreviousFile() {
			if(File.Exists(FilePath) && File.Exists(PreviousFilePath))
				try {
					File.Delete(PreviousFilePath);
				} catch {
				}
		}
		static string GetFolderPath() {
			try {
				Type type = typeof(System.Configuration.ApplicationSettingsBase).Assembly.GetType("System.Configuration.ConfigurationManagerInternalFactory");
				if(type != null) {
					PropertyInfo property = type.GetProperty("Instance", BindingFlags.Static | BindingFlags.NonPublic);
					if(property != null) {
						System.Configuration.Internal.IConfigurationManagerInternal instance = property.GetValue(null, null) as System.Configuration.Internal.IConfigurationManagerInternal;
						if(instance != null) {
							if(!string.IsNullOrEmpty(instance.ExeLocalConfigDirectory) && !Directory.Exists(instance.ExeLocalConfigDirectory))
								Directory.CreateDirectory(instance.ExeLocalConfigDirectory);
							return instance.ExeLocalConfigDirectory;
						}
					}
				}
			} catch {
			}
			return string.Empty;
		}
	}
}
