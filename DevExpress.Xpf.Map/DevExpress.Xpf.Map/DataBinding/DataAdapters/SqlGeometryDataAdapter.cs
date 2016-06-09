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
using System.Windows;
using System.Windows.Markup;
using DevExpress.Map.Native;
using DevExpress.Xpf.Map.Native;
using DevExpress.Xpf.Utils;
namespace DevExpress.Xpf.Map {
	public abstract class SqlGeometryDataAdapterBase : CoordinateSystemDataAdapterBase {
		readonly MapVectorItemCollection itemsCollection;
		XpfSqlGeometryLoader sqlLoader;
		protected XpfSqlGeometryLoader SqlLoader { get { return sqlLoader; } }
		protected internal override MapVectorItemCollection ItemsCollection { get { return itemsCollection; } }
		public SqlGeometryDataAdapterBase() {
			itemsCollection = new MapVectorItemCollection();
			InitLoader();
		}
		void InitLoader() {
			this.sqlLoader = new XpfSqlGeometryLoader();
			this.sqlLoader.ItemsLoaded += OnLoaderItemsLoaded;
		}
		void OnLoaderItemsLoaded(object sender, ItemsLoadedEventArgs<MapItem> e) {
			ItemsCollection.Clear();
			foreach(MapItem item in e.Items)
				ItemsCollection.Insert(0, item);
			if (Layer != null)
				Layer.UpdateBoundingRect();
		}
		protected void LoadCore(IList<SQLGeometryItemCore> content) {
			SqlLoader.Load(content, GetActualCoordinateSystem().CoordSystemCore);
		}
		protected abstract IList<SQLGeometryItemCore> GetSqlData();
		public override object GetItemSourceObject(MapItem item) {
			return item;
		}
		protected override void LoadDataCore() {
			IList<SQLGeometryItemCore> content = GetSqlData();
			if (content != null)
				LoadCore(content);
		}
	}
	public class SqlGeometryDataAdapter : SqlGeometryDataAdapterBase {
		public static readonly DependencyProperty ConnectionStringProperty = DependencyPropertyManager.Register("ConnectionString",
		   typeof(string), typeof(SqlGeometryDataAdapter), new PropertyMetadata(null, SqlAdapterPropertyChanged));
		public static readonly DependencyProperty SqlTextProperty = DependencyPropertyManager.Register("SqlText",
		   typeof(string), typeof(SqlGeometryDataAdapter), new PropertyMetadata(null, SqlAdapterPropertyChanged));
		public static readonly DependencyProperty SpatialDataMemberProperty = DependencyPropertyManager.Register("SpatialDataMember",
		   typeof(string), typeof(SqlGeometryDataAdapter), new PropertyMetadata(null, SqlAdapterPropertyChanged));
		protected static void SqlAdapterPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			SqlGeometryDataAdapter dataAdapter = d as SqlGeometryDataAdapter;
			if(dataAdapter != null)
				dataAdapter.LoadDataInternal();
		}
		[Category(Categories.Data)]
		public string ConnectionString {
			get { return (string)GetValue(ConnectionStringProperty); }
			set { SetValue(ConnectionStringProperty, value); }
		}
		[Category(Categories.Data)]
		public string SqlText {
			get { return (string)GetValue(SqlTextProperty); }
			set { SetValue(SqlTextProperty, value); }
		}
		[Category(Categories.Data)]
		public string SpatialDataMember {
			get { return (string)GetValue(SpatialDataMemberProperty); }
			set { SetValue(SpatialDataMemberProperty, value); }
		}
		SqlCommand CreateCommand(SqlConnection connection, string sqlText) {
			SqlCommand command = connection.CreateCommand();
			command.CommandType = CommandType.Text;
			command.CommandText = sqlText;
			return command;
		}
		IList<SQLGeometryItemCore> PopulateData(SqlConnection connection, string sqlText, string dataMember) {
			IList<SQLGeometryItemCore> geometry = new List<SQLGeometryItemCore>();
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
			for(int i = 0; i < reader.FieldCount; i++) {
				object attr = reader[i];
				if(attr != DBNull.Value) {
					string attrName = reader.GetName(i);
					if(attrName.Equals(geometryMember))
						continue;
					attributes.Add(new MapItemAttribute() { Name = attrName, Type = reader.GetFieldType(i), Value = reader.GetValue(i) });
				}
			}
			return attributes;
		}
		void ReportError(string errorMessage) {
			throw new Exception(errorMessage);
		}
		protected override IList<SQLGeometryItemCore> GetSqlData() {
			SqlConnection connection = new SqlConnection(ConnectionString);
			if(connection != null && !string.IsNullOrEmpty(SpatialDataMember) && !string.IsNullOrEmpty(SqlText)) {
				return PopulateData(connection, SqlText, SpatialDataMember);
			}
			return null;
		}
		protected override MapDependencyObject CreateObject() {
			return new SqlGeometryDataAdapter();
		}
	}
	[ContentProperty("Items")]
	public class SqlGeometryItemStorage : SqlGeometryDataAdapterBase {
		static readonly DependencyPropertyKey ItemsPropertyKey = DependencyPropertyManager.RegisterReadOnly("Items",
		   typeof(SqlGeometryItemCollection), typeof(SqlGeometryItemStorage), new PropertyMetadata());
		public static readonly DependencyProperty ItemsProperty = ItemsPropertyKey.DependencyProperty;
		[Category(Categories.Data)]
		public SqlGeometryItemCollection Items { get { return (SqlGeometryItemCollection)GetValue(ItemsProperty); } }
		public SqlGeometryItemStorage() {
			this.SetValue(ItemsPropertyKey, new SqlGeometryItemCollection());
			Items.Changed += OnCollectionChanged;
		}
		void OnCollectionChanged(object sender, EventArgs e) {
			LoadDataInternal();
		}
		protected override IList<SQLGeometryItemCore> GetSqlData() {
			IList<SQLGeometryItemCore> geometry = new List<SQLGeometryItemCore>();
			foreach(SqlGeometryItem item in Items) {
				geometry.Add(new SQLGeometryItemCore() { GeometryString = item.AsWkt(), Attributes = item.GetAttributes() });
			}
			return geometry;
		}
		protected override MapDependencyObject CreateObject() {
			return new SqlGeometryItemStorage();
		}
	}
}
namespace DevExpress.Xpf.Map.Native{
	public class XpfSqlGeometryLoader : SqlGeometryLoader<MapItem> {
		public XpfSqlGeometryLoader() : base(new XpfMapLoaderFactory()) { }
	}
}
