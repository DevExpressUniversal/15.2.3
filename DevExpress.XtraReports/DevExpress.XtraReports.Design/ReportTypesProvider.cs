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
using System.Reflection;
using System.Text;
using DevExpress.Design.VSIntegration;
using DevExpress.XtraReports.UI;
using EnvDTE;
namespace DevExpress.XtraReports.Design {
	class ReportTypesProvider {
		public static List<Type> GetTypeNamesFromAssembly(string location, Type baseType) {
			List<Type> result = new List<Type>();
			try {
				Assembly assembly = Assembly.LoadFile(location);
				Type[] types = assembly.GetTypes();
				foreach(Type type in types) {
					if(IsTypeCanBeInherited(type) && baseType.IsAssignableFrom(type))
						result.Add(type);
				}
			} catch { }
			return result;
		}
		static bool IsTypeCanBeInherited(Type type) {
			return !type.IsAbstract && !type.IsSealed && !type.ContainsGenericParameters;
		}
		public static List<string> GetTypeNames(ProjectItems projectItems) {
			List<string> typeNames = new List<string>(new DTEService(null).GetClassesInfo(projectItems, typeof(XtraReport)));
			FilterNonInheritableTypes(typeNames);
			FilterDuplicatedTypeNames(typeNames);
			return typeNames;
		}
		static void FilterNonInheritableTypes(List<string> typeNames) {
			for(int i = typeNames.Count - 1; i >= 0; i--) {
				Type type = Type.GetType(typeNames[i]);
				if(type == null)
					continue;
				if(!IsTypeCanBeInherited(type))
					typeNames.RemoveAt(i);
			}
		}
		static void FilterDuplicatedTypeNames(List<string> typeNames) {
			typeNames.Sort();
			for(int i = typeNames.Count - 1; i >= 1; i--)
				if(typeNames[i] == typeNames[i - 1])
					typeNames.RemoveAt(i);
		}
	}
}
