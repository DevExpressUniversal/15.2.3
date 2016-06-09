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
using System.ComponentModel;
using System.Reflection;
using DevExpress.ExpressApp.Core;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Utils;
using DevExpress.ExpressApp.Utils.Reflection;
namespace DevExpress.ExpressApp {
	public static class ModuleHelper {
		public static IEnumerable<Type> CollectControllerTypesFromAssembly(Assembly assembly) {
			return ControllersManager.CollectControllerTypesFromAssembly(assembly);
		}
		public static IEnumerable<Type> CollectExportedTypesFromAssembly(Assembly assembly, Predicate<Type> isExportedType) {
			Guard.ArgumentNotNull(assembly, "assembly");
			Guard.ArgumentNotNull(isExportedType, "isExportedType");
			return ((TypesInfo)XafTypesInfo.Instance).GetAssemblyTypes(assembly, isExportedType);
		}
		public static IEnumerable<Type> CollectRequiredExportedTypes(IEnumerable<Type> exportedTypes, Predicate<Type> isExportedType) {
			Guard.ArgumentNotNull(exportedTypes, "exportedTypes");
			Guard.ArgumentNotNull(isExportedType, "isExportedType");
			ITypesInfo typesInfo = XafTypesInfo.Instance;
			ExportedTypeCollection result = new ExportedTypeCollection();
			foreach(Type type in exportedTypes) {
				if(!result.Contains(type)) {
					result.Add(type);
					ITypeInfo info = typesInfo.FindTypeInfo(type);
					foreach(ITypeInfo requiredTypeInfo in info.GetRequiredTypes(delegate(ITypeInfo typeInfo) { return isExportedType(typeInfo.Type); })) {
						result.Add(requiredTypeInfo.Type);
					}
				}
			}
			return result;
		}
		private static Boolean IsExportedType(Type type) {
			return ExportedTypeHelpers.IsExportedType(type);
		}
	}
}
