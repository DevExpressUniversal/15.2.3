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

namespace DevExpress.Design.DataAccess {
	using System;
	using System.Collections.Generic;
	using System.Linq;
	abstract class BaseDataTypeInfo : DataSourceElementInfo, IDataTypeInfo {
		public BaseDataTypeInfo(Type sourceType)
			: base(sourceType, sourceType.Name) {
		}
		public Type ElementType {
			get { return base.SourceType; }
		}
	}
	sealed class DataTypeInfo : BaseDataTypeInfo {
		public DataTypeInfo(Type sourceType)
			: base(sourceType) {
		}
		public static IEnumerable<IDataTypeInfo> GetDataTypes(IDataAccessTechnologyInfoItem item) {
			IEnumerable<Type> typesProvider = (item as ITypesProviderDataAccessTechnologyInfoItem) ?? 
				DataTypeInfoHelper.GetLocalDataTypes();
			foreach(System.Type type in typesProvider)
				yield return new DataTypeInfo(type);
		}
	}
	sealed class XPODataTypeInfo : BaseDataTypeInfo {
		public XPODataTypeInfo(Type sourceType)
			: base(sourceType) {
		}
		protected override IEnumerable<string> GetFieldsCore() {
			return base.GetFieldsCore().Except(GetXPObjectExcludeProperties());
		}
		public static IEnumerable<IDataTypeInfo> GetDataTypes(IDataAccessTechnologyInfoItem item) {
			IEnumerable<Type> typesProvider = (item as ITypesProviderDataAccessTechnologyInfoItem) ??
				DataTypeInfoHelper.GetLocalDataTypes();
			foreach(System.Type type in typesProvider)
				yield return new XPODataTypeInfo(type);
		}
		static IDictionary<string, System.Type> typesCache = new Dictionary<string, System.Type>();
		internal static System.Type GetXPOType(string typeName) {
			System.Type xpoType;
			if(!typesCache.TryGetValue(typeName, out xpoType)) {
				xpoType = DevExpress.Utils.Design.DXAssemblyHelper.GetTypeFromAssembly(typeName, "DevExpress.Xpo", AssemblyInfo.SRAssemblyXpo);
				typesCache.Add(typeName, xpoType);
			}
			return xpoType;
		}
		static string[] excludeProperties;
		internal static string[] GetXPObjectExcludeProperties() {
			if(excludeProperties == null)
				excludeProperties = GetXPOType("XPObject").GetProperties().Select(p => p.Name).ToArray();
			return excludeProperties;
		}
	}
}
