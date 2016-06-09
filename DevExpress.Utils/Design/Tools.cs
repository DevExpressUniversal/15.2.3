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
using System.Reflection;
using DevExpress.Skins;
using DevExpress.Utils.Helpers;
using System.Collections.Generic;
using System.IO;
using System.Linq;
namespace DevExpress.Utils.Design {
	public class DesignTimeTools {
		public static bool IsDesignMode {
			get {
				Process process = Process.GetCurrentProcess();
				return (process != null) && IsVSProcess(process.ProcessName);
			}
		}
		public static void EnsureBonusSkins() {
			SkinManager.EnableFormSkins();
			try {
				Assembly.Load(AssemblyInfo.SRAssemblyBonusSkinsFull);
				ReflectionHelper.InvokeStaticMethod(AssemblyInfo.SRAssemblyBonusSkins, "DevExpress.UserSkins.BonusSkins", "Register");
			}
			catch {
			}
		}
		public static Assembly LoadAssembly(string assemblyName, bool checkDTClones) {
			if(!checkDTClones) return DoLoadDefaultAssembly(assemblyName);
			Assembly loadedAsm = GetLoadedAssembly(assemblyName);
			if(loadedAsm != null) return loadedAsm;
			return DoLoadDefaultAssembly(assemblyName);
		}
		static Assembly DoLoadDefaultAssembly(string assemblyName) {
			return Assembly.Load(assemblyName);
		}
		static Assembly GetLoadedAssembly(string assemblyName) {
			List<Assembly> asms = GetLoadedAssemblies(assemblyName);
			return asms != null ? asms.FirstOrDefault() : null;
		}
		static List<Assembly> GetLoadedAssemblies(string assemblyName) {
			List<Assembly> asms = null;
			try {
				asms = GetAssemblyClonesCore(assemblyName);
				if(asms != null && asms.Count > 1) {
					asms.Sort(new AssemblyComparer());
				}
			}
			catch { }
			return asms;
		}
		static List<Assembly> GetAssemblyClonesCore(string assemblyName) {
			List<Assembly> asms = new List<Assembly>();
			foreach(Assembly asm in AppDomain.CurrentDomain.GetAssemblies()) {
				AssemblyName asmName = asm.GetName();
				if(asmName != null && string.Equals(asmName.Name, assemblyName, StringComparison.OrdinalIgnoreCase)) asms.Add(asm);
			}
			return asms.Count > 0 ? asms : null;
		}
		class AssemblyComparer : IComparer<Assembly> {
			public int Compare(Assembly x, Assembly y) {
				DateTime xts = GetTimestamp(x), yts = GetTimestamp(y);
				if(xts.Equals(yts)) return 0;
				return xts < yts ? 1 : -1;
			}
			protected DateTime GetTimestamp(Assembly asm) {
				return File.GetCreationTime(asm.Location);
			}
		}
		internal static bool IsVSProcess(string processName) {
			return Array.Exists(Names, delegate(string name) {
				return string.Equals(processName, name, StringComparison.OrdinalIgnoreCase);
			});
		}
		static string[] namesCore = null;
		static string[] Names {
			get {
				if(namesCore == null) namesCore = CreateListCore();
				return namesCore;
			}
		}
		static string[] CreateListCore() {
			return new string[] { "devenv", "VCSExpress", "VBExpress", "WDExpress" };
		}
	}
}
#if DEBUGTEST
namespace DevExpress.Utils.Design.Tests {
	using NUnit.Framework;
	[TestFixture]
	public class DesignTimeToolsTester {
		[Test]
		public void Test_ValidProcName() {
			Assert.IsTrue(DesignTimeTools.IsVSProcess("devenv"));
			Assert.IsTrue(DesignTimeTools.IsVSProcess("VCSExpress"));
			Assert.IsTrue(DesignTimeTools.IsVSProcess("VBExpress"));
			Assert.IsTrue(DesignTimeTools.IsVSProcess("WDExpress"));
		}
		[Test]
		public void Test_ValidProcName_CaseTest() {
			Assert.IsTrue(DesignTimeTools.IsVSProcess("DEVENV"));
			Assert.IsTrue(DesignTimeTools.IsVSProcess("VCSExprESS"));
			Assert.IsTrue(DesignTimeTools.IsVSProcess("VBExpreSS"));
			Assert.IsTrue(DesignTimeTools.IsVSProcess("WDEXPRESS"));
		}
		[Test]
		public void Test_InvalidProcName() {
			Assert.IsFalse(DesignTimeTools.IsVSProcess("notepad"));
			Assert.IsFalse(DesignTimeTools.IsVSProcess("winword"));
			Assert.IsFalse(DesignTimeTools.IsVSProcess("invalid name"));
		}
	}
}
#endif
