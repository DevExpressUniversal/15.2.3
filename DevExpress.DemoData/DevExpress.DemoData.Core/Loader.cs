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
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Text;
using System.Xml.Serialization;
using DevExpress.DemoData.Helpers;
using DevExpress.DemoData.Utils;
using DevExpress.Internal;
using System.Linq;
using System.Threading;
using System.Diagnostics;
using System.Security.Permissions;
using DevExpress.DemoData.Core;
namespace DevExpress.DemoData {
	public static class Loader {
		public static Assembly DemoDataAssembly { get; set; }
	}
	public static class Linker {
		static object demosLock = new object();
		static Assembly dataAssembly;
		static long licensedProducts = -1;
		static object licensedProductsLock = new object();
		public static int UIThreadID = -1;
		public static bool BackgroundThreadOnly = false;
		public static bool IsOnline { get { return EnvironmentHelper.IsClickOnce; } }
		public static long LicensedProductsOverride = -1;
		public static bool IsRegistered { get { return LicensedProducts != 0; } }
		public static bool IsProductInstalled(string productComponentName) {
			if(EnvironmentHelper.IsClickOnce)
				return true;
			return (RegistryViewer.Current.IsKeyExists(Microsoft.Win32.RegistryHive.LocalMachine, "SOFTWARE/DevExpress/Developer")) ||
					RegistryViewer.Current.IsKeyExists(Microsoft.Win32.RegistryHive.LocalMachine, AssemblyInfo.InstallationRegistryKey + "/" + productComponentName)
#if DEBUG
					|| true
#endif
					;
		}
		static Assembly DataAssembly {
			get {
				if(dataAssembly == null) {
					dataAssembly = AssemblyHelper.GetLoadedAssembly(AssemblyInfo.SRAssemblyData);
					if(dataAssembly == null) {
						dataAssembly = AssemblyHelper.LoadDXAssembly(AssemblyInfo.SRAssemblyData);
					}
				}
				return dataAssembly;
			}
		}
		static long LicensedProducts {
			get {
				lock(licensedProductsLock) {
					if(licensedProducts < 0) {
						if(LicensedProductsOverride >= 0) {
							licensedProducts = LicensedProductsOverride;
						} else {
							licensedProducts = long.MaxValue;
						}
					}
					return licensedProducts;
				}
			}
		}
	}
	public enum ErrorMessageInfoKind { Error, Warning }
	public class ErrorMessageInfo {
		public LString Text { get; set; }
		public ErrorMessageInfoKind Kind { get; set; }
	}
}
