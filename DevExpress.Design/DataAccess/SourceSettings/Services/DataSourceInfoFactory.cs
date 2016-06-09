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
	using System.Collections.Generic;
	sealed class DefaultDataSourceInfoFactory : IDataSourceInfoFactory {
		public IDataSourceInfo GetInfo(IDataAccessTechnologyName technologyName, IDataAccessTechnologyInfoItem item) {
			return CodeNamesResolver.GetInitializer(technologyName.GetCodeName())(technologyName, item);
		}
		static class CodeNamesResolver {
			public delegate IDataSourceInfo Initializer(IDataAccessTechnologyName technologyName, IDataAccessTechnologyInfoItem item);
			static IDictionary<DataAccessTechnologyCodeName, Initializer> initializers;
			static CodeNamesResolver() {
				initializers = new Dictionary<DataAccessTechnologyCodeName, Initializer>();
				initializers.Add(DataAccessTechnologyCodeName.XmlDataSet, (technologyName, item) => new XmlDataSourceInfo(technologyName, item));
				initializers.Add(DataAccessTechnologyCodeName.TypedDataSet, (technologyName, item) => new TableSourceInfo(technologyName, item));
				initializers.Add(DataAccessTechnologyCodeName.SQLDataSource, (technologyName, item) => new QuerySourceInfo(technologyName, item));
				initializers.Add(DataAccessTechnologyCodeName.ExcelDataSource, (technologyName, item) => new ExcelDataSourceInfo(technologyName, item));
				initializers.Add(DataAccessTechnologyCodeName.LinqToSql, (technologyName, item) => new TableSourceInfo(technologyName, item));
				initializers.Add(DataAccessTechnologyCodeName.EntityFramework, (technologyName, item) => new TableSourceInfo(technologyName, item));
				initializers.Add(DataAccessTechnologyCodeName.Wcf, (technologyName, item) => new ServiceSourceInfo(technologyName, item));
				initializers.Add(DataAccessTechnologyCodeName.Ria, (technologyName, item) => new ServiceSourceInfo(technologyName, item));
				initializers.Add(DataAccessTechnologyCodeName.IEnumerable, (technologyName, item) => new TypeSourceInfo(technologyName, item));
				initializers.Add(DataAccessTechnologyCodeName.OLAP, (technologyName, item) => new OLAPSourceInfo(technologyName, item));
				initializers.Add(DataAccessTechnologyCodeName.Enum, (technologyName, item) => new EnumSourceInfo(technologyName, item));
				initializers.Add(DataAccessTechnologyCodeName.XPO, (technologyName, item) => new TypeSourceInfo(technologyName, item));
			}
			internal static Initializer GetInitializer(DataAccessTechnologyCodeName technologyCodeName) {
				return initializers[technologyCodeName];
			}
		}
		abstract class BaseDataSourceInfo : IDataSourceInfo {
			IEnumerable<IDataSourceElementInfo> elementsCore;
			System.Type sourceTypeCore;
			protected BaseDataSourceInfo(IDataAccessTechnologyName technologyName, IDataAccessTechnologyInfoItem item) {
				sourceTypeCore = GetSourceType(item);
				UpdateElements(technologyName, item);
			}
			protected void UpdateElements(IDataAccessTechnologyName technologyName, IDataAccessTechnologyInfoItem item) {
				elementsCore = System.Linq.Enumerable.OrderBy(GetElements(technologyName, item), (info) => info.Name);
			}
			IEnumerable<IDataSourceElementInfo> IDataSourceInfo.Elements {
				get { return elementsCore; }
			}
			System.Type IDataSourceInfo.SourceType {
				get { return sourceTypeCore; }
			}
			protected virtual System.Type GetSourceType(IDataAccessTechnologyInfoItem item) {
				return (item != null) ? item.Type : null;
			}
			protected abstract IEnumerable<IDataSourceElementInfo> GetElements(IDataAccessTechnologyName technologyName, IDataAccessTechnologyInfoItem item);
		}
		class TableSourceInfo : BaseDataSourceInfo {
			public TableSourceInfo(IDataAccessTechnologyName technologyName, IDataAccessTechnologyInfoItem item)
				: base(technologyName, item) {
			}
			protected override IEnumerable<IDataSourceElementInfo> GetElements(IDataAccessTechnologyName technologyName, IDataAccessTechnologyInfoItem item) {
				return DataTableInfo.GetDataTables(technologyName, item);
			}
		}
		class QuerySourceInfo : BaseDataSourceInfo, IComponentDataSourceInfo {
			public QuerySourceInfo(IDataAccessTechnologyName technologyName, IDataAccessTechnologyInfoItem item)
				: base(technologyName, item) {
				var componentItem = item as IComponentDataAccessTechnologyInfoItem;
				if(componentItem != null)
					Component = componentItem.Component;
			}
			public object Component {
				get;
				private set;
			}
			protected override System.Type GetSourceType(IDataAccessTechnologyInfoItem item) {
				return null;
			}
			protected override IEnumerable<IDataSourceElementInfo> GetElements(IDataAccessTechnologyName technologyName, IDataAccessTechnologyInfoItem item) {
				return QueryInfo.GetQueries(item);
			}
		}
		class ExcelDataSourceInfo : BaseDataSourceInfo, IExcelDataSourceInfo {
			public ExcelDataSourceInfo(IDataAccessTechnologyName technologyName, IDataAccessTechnologyInfoItem item)
				: base(technologyName, item) {
				var componentItem = item as IComponentDataAccessTechnologyInfoItem;
				if(componentItem != null)
					Component = componentItem.Component;
				if(Component != null) {
					var pFileName = componentItem.Type.GetProperty("FileName");
					if(pFileName != null && pFileName.CanRead)
						ExcelPath = pFileName.GetValue(Component, new object[] { }) as string;
				}
			}
			public object Component {
				get;
				private set;
			}
			public string ExcelPath {
				get;
				private set;
			}
			protected override System.Type GetSourceType(IDataAccessTechnologyInfoItem item) {
				return null;
			}
			protected override IEnumerable<IDataSourceElementInfo> GetElements(IDataAccessTechnologyName technologyName, IDataAccessTechnologyInfoItem item) {
				yield break;
			}
		}
		class TypeSourceInfo : BaseDataSourceInfo {
			public TypeSourceInfo(IDataAccessTechnologyName technologyName, IDataAccessTechnologyInfoItem item)
				: base(technologyName, item) {
			}
			protected override System.Type GetSourceType(IDataAccessTechnologyInfoItem item) {
				return null;
			}
			protected override IEnumerable<IDataSourceElementInfo> GetElements(IDataAccessTechnologyName technologyName, IDataAccessTechnologyInfoItem item) {
				return (technologyName.GetCodeName() == DataAccessTechnologyCodeName.XPO) ? XPODataTypeInfo.GetDataTypes(item) : DataTypeInfo.GetDataTypes(item);
			}
		}
		class EnumSourceInfo : BaseDataSourceInfo, IEnumDataSourceInfo {
			public EnumSourceInfo(IDataAccessTechnologyName technologyName, IDataAccessTechnologyInfoItem item)
				: base(technologyName, item) {
			}
			protected override System.Type GetSourceType(IDataAccessTechnologyInfoItem item) {
				return null;
			}
			protected override IEnumerable<IDataSourceElementInfo> GetElements(IDataAccessTechnologyName technologyName, IDataAccessTechnologyInfoItem item) {
				return EnumDataTypeInfo.GetDataTypes(item);
			}
		}
		class ServiceSourceInfo : BaseDataSourceInfo {
			public ServiceSourceInfo(IDataAccessTechnologyName technologyName, IDataAccessTechnologyInfoItem item)
				: base(technologyName, item) {
			}
			protected override IEnumerable<IDataSourceElementInfo> GetElements(IDataAccessTechnologyName technologyName, IDataAccessTechnologyInfoItem item) {
				return DataServiceTableInfo.GetDataServices(technologyName, item);
			}
		}
		class OLAPSourceInfo : BaseDataSourceInfo {
			public OLAPSourceInfo(IDataAccessTechnologyName technologyName, IDataAccessTechnologyInfoItem item)
				: base(technologyName, item) {
			}
			protected override IEnumerable<IDataSourceElementInfo> GetElements(IDataAccessTechnologyName technologyName, IDataAccessTechnologyInfoItem item) {
				yield break;
			}
		}
		class XmlDataSourceInfo : BaseDataSourceInfo, IXmlDataSourceInfo {
			string xmlPath;
			IDataAccessTechnologyName technologyName;
			public XmlDataSourceInfo(IDataAccessTechnologyName technologyName, IDataAccessTechnologyInfoItem item)
				: base(technologyName, item) {
				this.technologyName = technologyName;
			}
			public void UpdateElements(string xmlPath) {
				this.xmlPath = xmlPath;
				UpdateElements(technologyName, null);
			}
			protected override IEnumerable<IDataSourceElementInfo> GetElements(IDataAccessTechnologyName technologyName, IDataAccessTechnologyInfoItem item) {
				if(string.IsNullOrEmpty(xmlPath))
					yield break;
				using(System.Data.DataSet ds = new System.Data.DataSet()) {
					try { ds.ReadXmlSchema(xmlPath); }
					catch { yield break; }
					foreach(System.Data.DataTable table in ds.Tables) {
						yield return new DataSetTableInfo(ds, table.TableName);
					}
				}
			}
			class DataSetTableInfo : IDataSourceElementInfo {
				System.Data.DataSet dataSet;
				string tableName;
				public DataSetTableInfo(System.Data.DataSet dataSet, string tableName) {
					this.dataSet = dataSet;
					this.tableName = tableName;
				}
				public string Name {
					get { return tableName; }
				}
				public IEnumerable<string> Fields {
					get {
						var columns = dataSet.Tables[tableName].Columns;
						foreach(System.Data.DataColumn column in columns)
							yield return column.ColumnName;
					}
				}
			}
		}
	}
}
