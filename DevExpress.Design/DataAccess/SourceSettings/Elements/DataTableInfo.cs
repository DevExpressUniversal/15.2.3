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
	class DataTableInfo : DataSourceElementInfo, IDataTableInfo {
		DataAccessTechnologyCodeName codeName;
		Type tableType;
		public DataTableInfo(IDataAccessTechnologyName technologyName, Type sourceType, string name)
			: base(sourceType, name) {
			AssertionException.IsNotNull(technologyName);
			this.codeName = technologyName.GetCodeName();
		}
		protected override Type CheckSourceType(Type sourceType, string name) {
			tableType = DataTableInfoHelper.GetTableType(sourceType, name);
			return DataTableInfoHelper.GetRowType(tableType);
		}
		protected override IEnumerable<string> GetFieldsCore() {
			return (codeName == DataAccessTechnologyCodeName.TypedDataSet) ?
				DataSourceElementInfoHelper.GetDeclaredFields(SourceType) : base.GetFieldsCore();
		}
		public Type TableType {
			get { return tableType; }
		}
		public Type RowType {
			get { return base.SourceType; }
		}
		IEnumerable<string> keyExpressionsCore;
		public IEnumerable<string> KeyExpressions {
			get {
				if(keyExpressionsCore == null) {
					string keyAttributeName, keyPropertyName;
					CodeNamesResolver.ResolveKeyExpressionParameters(codeName, out keyAttributeName, out keyPropertyName);
					keyExpressionsCore = (codeName == DataAccessTechnologyCodeName.TypedDataSet) ?
						DataTableInfoHelper.GetKeyExpressions(base.SourceType, keyAttributeName, keyPropertyName) :
						DataTableInfoHelper.GetKeyExpressions(base.SourceType, keyAttributeName, keyPropertyName, (type) => type.GetProperties());
				}
				return keyExpressionsCore;
			}
		}
		public static IEnumerable<IDataTableInfo> GetDataTables(IDataAccessTechnologyName technologyName, IDataAccessTechnologyInfoItem item) {
			foreach(var mInfo in item.Members)
				yield return new DataTableInfo(technologyName, item.Type, mInfo.Name);
		}
		#region CodeNamesResolver
		static class CodeNamesResolver {
			static IDictionary<DataAccessTechnologyCodeName, KeyParameter> parameters;
			static CodeNamesResolver() {
				parameters = new Dictionary<DataAccessTechnologyCodeName, KeyParameter>();
				parameters.Add(DataAccessTechnologyCodeName.EntityFramework, new KeyParameter("EdmScalarPropertyAttribute", "EntityKeyProperty"));
				parameters.Add(DataAccessTechnologyCodeName.LinqToSql, new KeyParameter("ColumnAttribute", "IsPrimaryKey"));
				parameters.Add(DataAccessTechnologyCodeName.Wcf, new KeyParameter("DataServiceKeyAttribute", "KeyNames"));
			}
			public static void ResolveKeyExpressionParameters(DataAccessTechnologyCodeName codeName, out string keyAttribute, out string keyProperty) {
				KeyParameter parameter;
				if(parameters.TryGetValue(codeName, out parameter)) {
					keyAttribute = parameter.KeyAttributeName;
					keyProperty = parameter.KeyPropertyName;
				}
				else {
					keyAttribute = keyProperty = null;
				}
			}
			class KeyParameter {
				public readonly string KeyAttributeName;
				public readonly string KeyPropertyName;
				public KeyParameter(string attribute, string property) {
					KeyAttributeName = attribute;
					KeyPropertyName = property;
				}
			}
		}
		#endregion CodeNamesResolver
	}
}
