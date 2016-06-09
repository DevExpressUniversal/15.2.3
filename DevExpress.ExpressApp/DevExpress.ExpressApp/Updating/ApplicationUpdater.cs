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
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.Serialization;
using DevExpress.ExpressApp.Localization;
using DevExpress.Persistent.Base;
namespace DevExpress.ExpressApp.Updating {
	[Serializable]
	public class AppUpdaterInvalidPathException : UpdatingException {
		protected AppUpdaterInvalidPathException(SerializationInfo info, StreamingContext context) : base(info, context) { }
		public AppUpdaterInvalidPathException(string path) : base(SystemExceptionLocalizer.GetExceptionMessage(ExceptionId.InvalidApplicationVersion, path)) { }
		public AppUpdaterInvalidPathException(string path, string assemblyName) : base(SystemExceptionLocalizer.GetExceptionMessage(ExceptionId.ApplicationUpdaterAssemblyDoesNotExist, path, assemblyName)) { }
	}
	public interface IApplicationUpdater {
		bool CheckAndUpdate();
		bool NeedAppUpdate();
	}
	public class ApplicationUpdater : Updater, IApplicationUpdater {
		private string serverPath;
		private static string updaterFileName = "DevExpress.ExpressApp.Updater.exe";
		private static string updaterConfigFileNameExtension = ".config";
		public virtual bool NeedAppUpdate() {
			bool result = false;
			if(IsDatabaseExist()) {
				foreach(ModuleBase module in Modules) {
					IModuleInfo moduleInfo = GetModuleInfoFromDB(module.Name);
					Version moduleDBVersion = new Version(0, 0, 0, 0);
					if(moduleInfo != null) {
						moduleDBVersion = new Version(moduleInfo.Version);
					}
					Tracing.Tracer.LogText("module '{0}'. Local version: {1}, Version on DB: {2}", module.Name, module.Version, moduleDBVersion);
					if((module.Version != null) && (module.Version < moduleDBVersion)) {
						result = true;
						Tracing.Tracer.LogText("module '{0}' is outdated", module.Name);
						break;
					}
				}
			}
			Tracing.Tracer.LogValue("NeedAppUpdate", result);
			return result;
		}
		public ApplicationUpdater(IObjectSpace objectSpace, IList<ModuleBase> modules, String applicationName, Type moduleInfoType)
			: base(objectSpace, modules, applicationName, moduleInfoType) { }
		public ApplicationUpdater(IObjectSpace objectSpace, IList<ModuleBase> modules, String applicationName, String serverPath, Type moduleInfoType)
			: this(objectSpace, modules, applicationName, moduleInfoType) {
			this.serverPath = serverPath;
		}
		public virtual bool CheckAndUpdate() {
			bool result = false;
			Tracing.Tracer.LogText("UpdateApplication");
			Tracing.Tracer.LogSubSeparator("Check for executables new version");
			if(NeedAppUpdate()) {
				if(serverPath == null || !Directory.Exists(serverPath)) {
					throw new Exception(SystemExceptionLocalizer.GetExceptionMessage(ExceptionId.ApplicationUpdaterDatabaseVersionIsNewer));
				}
				string localUpdaterPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, UpdaterFileName);
				try {
					String updaterFullFileName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, UpdaterFileName);
					File.SetAttributes(updaterFullFileName, FileAttributes.Normal);
					File.Delete(updaterFullFileName);
				}
				catch { }
				File.Copy(Path.Combine(serverPath, UpdaterFileName), localUpdaterPath, true);
				string configFileName = Path.Combine(serverPath, UpdaterFileName + updaterConfigFileNameExtension);
				if(File.Exists(configFileName)) {
					File.Copy(configFileName, localUpdaterPath + updaterConfigFileNameExtension, true);
				}
				ProcessStartInfo info = new ProcessStartInfo(localUpdaterPath);
				info.Arguments = String.Format(@"""{0}"" ""{1}"" ""{2}""", serverPath.TrimEnd('\\'), AppDomain.CurrentDomain.FriendlyName, Process.GetCurrentProcess().Id);
				Process.Start(info);
				result = true;
			}
			return result;
		}
		public static string UpdaterFileName {
			get { return updaterFileName; }
			set { updaterFileName = value; }
		}
		public string ServerPath { get { return serverPath; } }
	}
}
