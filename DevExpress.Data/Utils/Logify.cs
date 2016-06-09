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
using System.ComponentModel;
using System.IO;
using System.Reflection;
using DevExpress.Internal;
using Microsoft.Win32;
namespace DevExpress.Logify {
	public class LogifyExceptionHandler {
		static LogifyExceptionHandler instance;
		bool isInitialized;
		bool isSuccessfullyInstalled;
		public static LogifyExceptionHandler Instance {
			get {
				if (instance != null)
					return instance;
				lock (typeof(LogifyExceptionHandler)) {
					if (instance != null)
						return instance;
					instance = new LogifyExceptionHandler();
					return instance;
				}
			}
		}
		CancelEventHandler onCanReportException;
		public event CancelEventHandler CanReportException { add { onCanReportException += value; } remove { onCanReportException -= value; } }
		void RaiseCanReportException(CancelEventArgs args) {
			if (onCanReportException != null)
				onCanReportException(this, args);
		}
		public bool Initialize(string logId, string lastExecptionReportFileName) {
			if (isInitialized)
				return isSuccessfullyInstalled;
			if (IsWebEnvironment())
				isSuccessfullyInstalled = TryInstallWebExceptionHandler(logId);
			else if (IsDesktopEnvironment())
				isSuccessfullyInstalled = TryInstallDesktopExceptionHandler(logId, lastExecptionReportFileName);
			this.isInitialized = true;
			return isSuccessfullyInstalled;
		}
		bool IsDesktopEnvironment() {
			return Environment.UserInteractive || Assembly.GetEntryAssembly() != null;
		}
		const string logifyAsmShortName = "Logify.Client.Win";
		Assembly LoadLogifyAssembly() {
			return TryLoadLogifyAssembly();
		}
		Assembly TryLoadLogifyAssembly() {
			try {
				RegistryKey key = Registry.LocalMachine.OpenSubKey(AssemblyInfo.InstallationRegistryKey);
				if (key == null)
					return null;
				string rootDirectory = key.GetValue(AssemblyInfo.InstallationRegistryRootPathValueName) as String;
				if (String.IsNullOrEmpty(rootDirectory))
					return null;
				string logifyCorePath = Path.Combine(rootDirectory, @"System\Components\Logify\Logify.Client.Core.dll");
				string logifyWinPath = Path.Combine(rootDirectory, @"System\Components\Logify\Logify.Client.Win.dll");
				if (!File.Exists(logifyCorePath) || !File.Exists(logifyWinPath))
					return null;
				Assembly.LoadFrom(logifyCorePath);
				return Assembly.LoadFrom(logifyWinPath);
			}
			catch {
				return null;
			}
		}
		bool TryInstallDesktopExceptionHandler(string logId, string lastExecptionReportFileName) {
			try {
				Assembly asm = LoadLogifyAssembly();
				if (asm == null)
					return false;
				Type exceptionLogType = asm.GetType("DevExpress.Logify.Win.DxLogifyClient");
				if (exceptionLogType == null)
					return false;
				ConstructorInfo ctor = exceptionLogType.GetConstructor(new Type[] { });
				if (ctor == null)
					return false;
				object instance = ctor.Invoke(new object[] { });
				if (instance == null)
					return false;
				MethodInfo runMethod = exceptionLogType.GetMethod("Run");
				if (runMethod == null)
					return false;
				runMethod.Invoke(instance, new object[] { });
				MethodInfo reportToDevExpress = exceptionLogType.GetMethod("ReportToDevExpress");
				if (reportToDevExpress != null)
					reportToDevExpress.Invoke(instance, new object[] { logId, lastExecptionReportFileName, GetType().Assembly });
				EventInfo eventInfo = exceptionLogType.GetEvent("CanReportException");
				if (eventInfo != null)
					eventInfo.AddEventHandler(instance, new CancelEventHandler(OnCanReportException));
				return true;
			}
			catch {
				return false;
			}
		}
		void OnCanReportException(object sender, CancelEventArgs e) {
			RaiseCanReportException(e);
		}
		bool IsWebEnvironment() {
			return false;
		}
		bool TryInstallWebExceptionHandler(string logId) {
			return false;
		}
	}
}
