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
using System.Linq;
using System.Reflection;
namespace DevExpress.XtraReports.Native {
	[Flags]
	public enum TypeResolvingOptions {
		None = 0,
		IgnoreAssemblyName = 1
	}
	public static class TypeResolver {
		public static Type GetType(string fullTypeName) {
			return GetType(fullTypeName, TypeResolvingOptions.None);
		}
		public static Type GetType(string fullTypeName, TypeResolvingOptions typeResolvingOptions) {
			if(string.IsNullOrWhiteSpace(fullTypeName))
				return null;
			string requiredTypeName;
			string requiredAssemblyName;
			SplitTypeName(fullTypeName, out requiredTypeName, out requiredAssemblyName);
			Assembly executingAssembly = Assembly.GetExecutingAssembly();
			Type requiredType = FindType(executingAssembly, requiredTypeName, requiredAssemblyName, typeResolvingOptions);
			if(requiredType != null)
				return requiredType;
			foreach(Assembly assembly in AppDomain.CurrentDomain.GetAssemblies().Except(new[] { executingAssembly })) {
				try {
					requiredType = FindType(assembly, requiredTypeName, requiredAssemblyName, typeResolvingOptions);
				} catch { }
				if(requiredType != null)
					return requiredType;
			}
			string shortTypeName = string.IsNullOrEmpty(requiredAssemblyName) ? requiredTypeName : string.Concat(requiredTypeName, ", ", requiredAssemblyName);
			return Type.GetType(shortTypeName);
		}
		public static void SplitTypeName(string fullTypeName, out string typeName, out string assemblyName) {
			string[] fullTypeNameParts = fullTypeName.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
			typeName = fullTypeNameParts.Length > 0 ? fullTypeNameParts[0].Trim() : string.Empty;
			assemblyName = fullTypeNameParts.Length > 1 ? fullTypeNameParts[1].Trim() : string.Empty;
		}
		static Type FindType(Assembly assembly, string requiredTypeName, string requiredAssemblyName, TypeResolvingOptions typeResolvingOptions) {
			string assemblyName = assembly.GetName().Name;
			if(typeResolvingOptions == TypeResolvingOptions.IgnoreAssemblyName || assemblyName.Length == 0 || requiredAssemblyName.Length == 0 || AreAssembliesNamesMatch(assemblyName, requiredAssemblyName)) {
				return assembly.GetType(requiredTypeName, throwOnError: false, ignoreCase: true);
			}
			return null;
		}
		static bool AreAssembliesNamesMatch(string firstName, string secondName) {
			return string.Compare(firstName, secondName, ignoreCase: true) == 0;
		}
	}
}
