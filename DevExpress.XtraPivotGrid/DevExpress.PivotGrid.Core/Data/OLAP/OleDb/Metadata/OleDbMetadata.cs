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
using System.Data;
using DevExpress.PivotGrid.QueryMode;
using DevExpress.XtraPivotGrid;
using DevExpress.XtraPivotGrid.Data;
namespace DevExpress.PivotGrid.OLAP {
	public class OleDbMetadata : OLAPMetadata {
		protected internal override string ServerVersion { get { return Connection.ServerVersion; } }
		internal new OleConnection Connection { get { return (OleConnection)base.Connection; } }
		public new string FullConnectionString {
			get { return base.FullConnectionString; }
			set { base.FullConnectionString = value; }
		}
		public OleDbMetadata() : base() { }
		protected override IQueryExecutor<OLAPCubeColumn> CreateQueryExecutor() {
			return new OLAPDataSourceQueryExecutor(new OleCellSetParser(), this);
		}
		#region schema
		DataTable GetSchema(Guid guid, object[] restrictions) {
			if(!Connected)
				return null;
			return Connection.GetOleDbSchemaTable(guid, restrictions);
		}
		#endregion
		protected internal override bool PopulateColumnsCore(IDataSourceHelpersOwner<OLAPCubeColumn> owner) {
			return PopulateColumns(true, (IOLAPHelpersOwner)owner);
		}
		protected internal override IOLAPConnection CreateConnection(IOLAPConnectionSettings connectionSettings, IOLAPMetadata data) {
			return new OleConnection(connectionSettings.ConnectionString);
		}
		public override bool DimensionPropertiesSupported {
			get { return false; }
		}
		protected override bool ContainsMemberPropertiesInResponse() {
			return false;
		}
		public override IOLAPRowSet GetShemaRowSet(string name, Dictionary<string, object> restrictions) {
			Guid guid;
			switch(name) {
				case OlapSchema.Members:
					guid = OLAPSchemaGuid.Members;
					break;
				case OlapSchema.Dimensions:
					guid = OLAPSchemaGuid.Dimensions;
					break;
				case OlapSchema.Hierarchies:
					guid = OLAPSchemaGuid.Hierarchies;
					break;
				case OlapSchema.Properties:
					guid = OLAPSchemaGuid.Properties;
					break;
				case OlapSchema.Levels:
					guid = OLAPSchemaGuid.Levels;
					break;
				case OlapSchema.Cubes:
					guid = OLAPSchemaGuid.Cubes;
					break;
				case OlapSchema.Measures:
					guid = OLAPSchemaGuid.Measures;
					break;
				case OlapSchema.MeasureGroups:
					guid = OLAPSchemaGuid.MeasureGroups;
					break;
				case OlapSchema.KPIs:
					guid = OLAPSchemaGuid.KPIs;
					break;
				default:
					throw new NotImplementedException(name);
			}
			return DataReaderWrapper.Wrap(Connection.GetOleDbSchemaTable(guid, GetRowsByRestrictions(restrictions, name)));
		}
		object[] GetRowsByRestrictions(Dictionary<string, object> restrictions, string name) {
			List<string> restrictionsNameList = GetRestrictionNames(name);
			object[] result = new object[0];
			foreach(KeyValuePair<string, object> pair in restrictions) {
				int index = restrictionsNameList.IndexOf(pair.Key);
				if(index < 0)
					throw new Exception(string.Format("Unknown restriction for {0}: {1}", name, pair.Key));
				if(result.Length < index + 1)
					Array.Resize(ref result, index + 1);
				result[index] = pair.Value;
			}
			return result;
		}
		List<string> GetRestrictionNames(string name) {
			switch(name) {
				case OlapSchema.Members:
					return new List<string>() {
												"CATALOG_NAME", "SCHEMA_NAME", "CUBE_NAME",
												"DIMENSION_UNIQUE_NAME", "HIERARCHY_UNIQUE_NAME", "LEVEL_UNIQUE_NAME",
												"LEVEL_NUMBER", "MEMBER_NAME", "MEMBER_UNIQUE_NAME",
												"MEMBER_CAPTION", "MEMBER_TYPE", "TREE_OP",
												"CUBE_SOURCE" };
				case OlapSchema.Dimensions:
					return new List<string>() {
												"CATALOG_NAME", "SCHEMA_NAME", "CUBE_NAME",
												"DIMENSION_NAME", "DIMENSION_UNIQUE_NAME", "CUBE_SOURCE",
												"DIMENSION_VISIBILITY"
					 };
				case OlapSchema.Hierarchies:
					return new List<string>() {
												"CATALOG_NAME", "SCHEMA_NAME", "CUBE_NAME", 
												"DIMENSION_UNIQUE_NAME", "HIERARCHY_NAME", "HIERARCHY_UNIQUE_NAME",
												"HIERARCHY_ORIGIN", "CUBE_SOURCE", "HIERARCHY_VISIBILITY"
					};
				case OlapSchema.Properties:
					return new List<string>() {
												"CATALOG_NAME", "SCHEMA_NAME", "CUBE_NAME",
												"DIMENSION_UNIQUE_NAME", "HIERARCHY_UNIQUE_NAME", "LEVEL_UNIQUE_NAME",
												"MEMBER_UNIQUE_NAME", "PROPERTY_TYPE", "PROPERTY_NAME", 
												"PROPERTY_CONTENT_TYPE", "PROPERTY_ORIGIN", "CUBE_SOURCE",
												"PROPERTY_VISIBILITY"
					};
				case OlapSchema.Levels:
					return new List<string>() {
												"CATALOG_NAME", "SCHEMA_NAME", "CUBE_NAME",
												"DIMENSION_UNIQUE_NAME", "HIERARCHY_UNIQUE_NAME", "LEVEL_NAME",
												"LEVEL_UNIQUE_NAME", "LEVEL_ORIGIN", "CUBE_SOURCE",
												"LEVEL_VISIBILITY"
							 };
				case OlapSchema.Cubes:
					return new List<string>() {
												"CATALOG_NAME", "SCHEMA_NAME", "CUBE_NAME", 
					};
				case OlapSchema.Measures:
					return new List<string>() {
												"CATALOG_NAME", "SCHEMA_NAME", "CUBE_NAME", 
					};
				case OlapSchema.MeasureGroups:
					return new List<string>() {
												"CATALOG_NAME", "SCHEMA_NAME", "CUBE_NAME", 
					};
				case OlapSchema.KPIs:
					return new List<string>() {
												"CATALOG_NAME", "SCHEMA_NAME", "CUBE_NAME", 
					};
				default:
					throw new NotImplementedException();
			}
		}
	}
}
