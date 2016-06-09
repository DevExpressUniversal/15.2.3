#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       XtraReports for ASP.NET                                     }
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
using System.Linq;
using System.Reflection;
namespace DevExpress.XtraReports.Web.Native {
	static class AsmHelper {
		public static object ResolveAppCodeInstance(string path, string typeName) {
			string[] files = Directory.GetFiles(path, "App_Code*.dll", SearchOption.AllDirectories);
			foreach(string file in files) {
				try {
					Assembly asm = Assembly.LoadFrom(file);
					object obj = CreateAssemblyInstance(asm, typeName);
					if(obj != null)
						return obj;
				} catch { }
			}
			return null;
		}
		public static object CreateInstance(Assembly asm, string typeName) {
			if(typeName.Contains(',')) {
				return CreateInstanceByFullTypeName(typeName);
			}
			if(asm != null) {
				try {
					object obj = CreateAssemblyInstance(asm, typeName);
					if(obj != null)
						return obj;
					AssemblyName[] asmRefs = asm.GetReferencedAssemblies();
					foreach(AssemblyName asmRef in asmRefs) {
						obj = CreateInstance(asmRef, typeName);
						if(obj != null)
							return obj;
					}
				} catch {
				}
			}
			return FindInDomainAndCreate(typeName);
		}
		static object FindInDomainAndCreate(string typeName) {
			return AppDomain.CurrentDomain
				.GetAssemblies()
				.Select(x => CreateAssemblyInstance(x, typeName))
				.FirstOrDefault(x => x != null);
		}
		static object CreateInstanceByFullTypeName(string typeName) {
			Type type = Type.GetType(typeName, false, true);
			return type == null ? null : Activator.CreateInstance(type);
		}
		static object CreateAssemblyInstance(Assembly asm, string typeName) {
			if(asm != null) {
				try {
					return asm.CreateInstance(typeName);
				} catch { }
			}
			return null;
		}
		static object CreateInstance(AssemblyName asmRef, string typeName) {
			try {
				return CreateAssemblyInstance(Assembly.Load(asmRef), typeName);
			} catch { }
			return null;
		}
	}
}
