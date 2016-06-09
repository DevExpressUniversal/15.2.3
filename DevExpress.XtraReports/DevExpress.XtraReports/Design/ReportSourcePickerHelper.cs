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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using DevExpress.Utils;
using DevExpress.XtraReports.UI;
namespace DevExpress.XtraReports.Design {
	public static class ReportSourcePickerHelper {
		static List<Type> GetTypes(Assembly asm, Type baseType) {
			List<Type> list = new List<Type>();
			try {
				Type[] types = asm.GetTypes();
				foreach(Type type in types) {
					if(type.IsSubclassOf(baseType))
						list.Add(type);
				}
			} catch { }
			return list;
		}
		static void RemoveDescendants(IList types, Type baseType) {
			for(int i = types.Count - 1; i >= 0; i--)
				if(baseType.IsAssignableFrom((Type)types[i]))
					types.RemoveAt(i);
		}
		static List<Type> CollectReportTypes(Assembly entryAssembly, Type rootType) {
			List<Type> types = GetReportTypes(entryAssembly, rootType);
			AssemblyName[] assemblyNames = entryAssembly.GetReferencedAssemblies();
			foreach(AssemblyName assemblyName in assemblyNames) {
				try {
					Assembly assembly = Assembly.Load(assemblyName);
					if(assembly == typeof(XtraReport).Assembly)
						continue;
					List<Type> referencedTypes = GetReportTypes(assembly, rootType);
					types.AddRange(referencedTypes);
				} catch { }
			}
			return types;
		}
		static List<Type> GetReportTypes(Assembly assembly, Type rootType) {
			List<Type> types = GetTypes(assembly, typeof(XtraReport));
			if(!rootType.Equals(typeof(XtraReport)))
				RemoveDescendants(types, rootType);
			return types;
		}
		public static IEnumerable<Type> GetAvailableReports(object rootComponent) {
			try {
				var entryAssembly = AssemblyHelper.EntryAssembly;
				return entryAssembly == null ? Enumerable.Empty<Type>() : CollectReportTypes(entryAssembly, rootComponent.GetType());
			} catch {
				return Enumerable.Empty<Type>();
			}
		}
	}
}
