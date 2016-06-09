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
using System.IO;
using DevExpress.Internal;
using DevExpress.DemoData.Helpers;
using Microsoft.Win32;
using System.Collections.Generic;
namespace DevExpress.DemoData.Core {
	public static class PathHelper {
		public static string GetCopyrightYear() {
			const string StartYear = "2000-";
			int i = AssemblyInfo.AssemblyCopyright.IndexOf(StartYear, StringComparison.Ordinal) + StartYear.Length;
			return AssemblyInfo.AssemblyCopyright.Substring(i, 4);
		}
		readonly static string[] BinFolders = new string[] {
				"",
#if DEBUG
				"..\\CS\\$DEMO$.Wpf\\bin\\Debug",
				"..\\CS\\$DEMO$.Wpf\\bin\\Release",
				"..\\VB\\$DEMO$.Wpf\\bin\\Debug",
				"..\\VB\\$DEMO$.Wpf\\bin\\Release",
#endif
				"..\\..\\..\\..\\Bin",
				"..\\..\\Reporting\\Bin",
#if DEBUG
				"..\\..\\Reporting\\CS\\ReportWpfDemo\\bin\\Debug",
				"..\\..\\Reporting\\CS\\ReportWpfDemo\\bin\\Release",
				"..\\..\\Reporting\\VB\\ReportWpfDemo\\bin\\Debug",
				"..\\..\\Reporting\\VB\\ReportWpfDemo\\bin\\Release",
				"..\\..\\..\\..\\$DEMO$\\$DEMO$\\bin\\Debug",
				"..\\..\\..\\..\\$DEMO$\\$DEMO$\\bin\\Release",
				"..\\..\\..\\..\\..\\Demos.Win\\ReportsDemos\\CS\\ReportWpfDemo\\bin\\Debug",
				"..\\..\\..\\..\\..\\Demos.Win\\ReportsDemos\\CS\\ReportWpfDemo\\bin\\Release",
				"..\\..\\..\\..\\..\\Demos.Win\\ReportsDemos\\VB\\ReportWpfDemo\\bin\\Debug",
				"..\\..\\..\\..\\..\\Demos.Win\\ReportsDemos\\VB\\ReportWpfDemo\\bin\\Release",
#endif
			};
		public static string GetDemoExePath(string demoName) {
			string assemblyFilePath = null;
			string directory = AppDomain.CurrentDomain.BaseDirectory;
			foreach(string binFolder in BinFolders) {
				string filePath = Path.Combine(directory, Path.Combine(binFolder, demoName)).Replace("$DEMO$", demoName);
				foreach(string ext in new string[] { ".dll", ".exe" }) {
					string filePathWithExt = filePath + ext;
					if(File.Exists(filePathWithExt)) {
						assemblyFilePath = filePathWithExt;
						break;
					}
				}
			}
			return assemblyFilePath;
		}
	}
}
