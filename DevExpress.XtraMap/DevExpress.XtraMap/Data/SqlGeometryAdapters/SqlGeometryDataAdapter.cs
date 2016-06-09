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
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using DevExpress.Map.Native;
using DevExpress.XtraMap.Native;
namespace DevExpress.XtraMap {
	public abstract class SqlGeometryDataAdapterBase : CoordinateSystemDataAdapterBase {
		WinSqlGeometryLoader sqlLoader;
		protected override bool IsReady { get { return AreItemsLoaded; } }
		protected WinSqlGeometryLoader SqlLoader { get { return sqlLoader; } }
		protected SqlGeometryDataAdapterBase() {
			InitLoader();
		}
		void InitLoader() {
			this.sqlLoader = new WinSqlGeometryLoader();
			this.sqlLoader.ItemsLoaded += OnLoaderItemsLoaded;
		}
		protected void LoadCore(IList<SQLGeometryItemCore> content) {
			SqlLoader.Load(content, GetActualCoordinateSystem().CoordSystemCore);
		}
		protected override bool IsCSCompatibleTo(MapCoordinateSystem mapCS) {
			return mapCS.PointType == SourceCoordinateSystem.GetSourcePointType();
		}
		#region DataAdapter
		protected override MapItem GetItemBySourceObject(object sourceObject) {
			return null;
		}
		protected override object GetItemSourceObject(MapItem item) {
			return item;
		}
		protected internal override MapItemType GetDefaultMapItemType() {
			return MapItemType.Custom;
		}
		#endregion
	}
	public class SqlGeometryDataAdapter : SqlGeometryDataAdapterBase {
		string connectionString;
		string sqlText;
		string spatialDataMember;
		[
#if !SL
	DevExpressXtraMapLocalizedDescription("SqlGeometryDataAdapterConnectionString"),
#endif
		Category(SRCategoryNames.Data), DefaultValue("")]
		public string ConnectionString {
			get { return connectionString; }
			set {
				if(connectionString == value)
					return;
				connectionString = value;
				OnPropertyChanged();
			}
		}
		[
#if !SL
	DevExpressXtraMapLocalizedDescription("SqlGeometryDataAdapterSqlText"),
#endif
		Category(SRCategoryNames.Data), DefaultValue("")]
		public string SqlText {
			get { return sqlText; }
			set {
				if(sqlText == value)
					return;
				sqlText = value;
				OnPropertyChanged();
			}
		}
		[
#if !SL
	DevExpressXtraMapLocalizedDescription("SqlGeometryDataAdapterSpatialDataMember"),
#endif
		Category(SRCategoryNames.Data), DefaultValue("")]
		public string SpatialDataMember {
			get { return spatialDataMember; }
			set {
				if(spatialDataMember == value)
					return;
				spatialDataMember = value;
				OnPropertyChanged();
			}
		}
		SqlCommand CreateCommand(SqlConnection connection, string sqlText) {
			SqlCommand command = connection.CreateCommand();
			command.CommandType = CommandType.Text;
			command.CommandText = sqlText;
			return command;
		}
		IList<SQLGeometryItemCore> PopulateData(SqlConnection connection, string sqlText, string dataMember) {
			IList <SQLGeometryItemCore> geometry = new List<SQLGeometryItemCore>();
			try {
				connection.Open();
				using(SqlCommand command = CreateCommand(connection, sqlText)) {
					try {
						using(SqlDataReader reader = command.ExecuteReader()) {
							reader.Read();
							do {
								object geometryValue = reader[dataMember];
								if(geometryValue != DBNull.Value)
									geometry.Add(new SQLGeometryItemCore() { GeometryString = geometryValue.ToString(), Attributes = PopulateAttributes(reader, dataMember) });
							} while(reader.Read());
						}
					}
					catch(SqlException ex) {
						ReportError(ex.Message);
					}
				}
				connection.Close();
			}
			catch(Exception ex) {
				ReportError(ex.Message);
			}
			return geometry;
		}
		IList<IMapItemAttribute> PopulateAttributes(SqlDataReader reader, string geometryMember) {
			IList<IMapItemAttribute> attributes = new List<IMapItemAttribute>();
			for(int i = 0; i < reader.FieldCount;i++ ) {
				object attr = reader[i];
				if(attr != DBNull.Value) {
					string attrName = reader.GetName(i);
					if(attrName.Equals(geometryMember))
						continue;
					attributes.Add(new MapItemAttribute() {Name = attrName, Type = reader.GetFieldType(i), Value = reader.GetValue(i) });
				}
			}
				return attributes;
		}
		void ReportError(string errorMessage) {
			throw new Exception(errorMessage);
		}
		protected override void LoadData(IMapItemFactory factory) {
			SqlConnection connection = new SqlConnection(ConnectionString);
			if(connection != null && !string.IsNullOrEmpty(SpatialDataMember) && !string.IsNullOrEmpty(SqlText)) {
				IList<SQLGeometryItemCore> content = PopulateData(connection, SqlText, SpatialDataMember);
				LoadCore(content);
			}
		}
		public override string ToString() {
			return "(SqlDbGeometryAdapter)";
		}
	}
	public class SqlGeometryItemStorage : SqlGeometryDataAdapterBase {
		readonly SqlGeometryItemCollection sqlItems;
		[Category(SRCategoryNames.Data),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		TypeConverter(typeof(CollectionConverter))]
		public SqlGeometryItemCollection Items { get { return sqlItems; } }
		public SqlGeometryItemStorage() {
			this.sqlItems = new SqlGeometryItemCollection(this);
		}
		protected override void LoadData(IMapItemFactory factory) {
			IList<SQLGeometryItemCore> geometry = new List<SQLGeometryItemCore>();
			foreach(SqlGeometryItem item in Items) 
				geometry.Add(new SQLGeometryItemCore() { GeometryString = item.AsWkt(), Attributes = item.GetAttributes() });
			LoadCore(geometry);
		}
		public override string ToString() {
			return "(SqlGeometryItemStorage)";
		}
	}
}
namespace DevExpress.XtraMap.Native {
	public class WinSqlGeometryLoader : SqlGeometryLoader<MapItem> {
		public WinSqlGeometryLoader() : base(new WinMapLoaderFactory()) { } 
	}
}
