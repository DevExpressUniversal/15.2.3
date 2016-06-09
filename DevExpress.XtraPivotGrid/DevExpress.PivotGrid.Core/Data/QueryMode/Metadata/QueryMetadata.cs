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
using System.IO;
using DevExpress.Data.IO;
#if !SL
using System.IO.Compression;
using DevExpress.PivotGrid.DataCalculation;
#else
using DevExpress.Utils.Zip;
#endif
namespace DevExpress.PivotGrid.QueryMode {
	public abstract class QueryMetadata<AreasType> : IQueryMetadata where AreasType : QueryColumn {
		QueryMetadataColumns columns;
		IQueryExecutor<AreasType> queryExecutor;
		List<IDataSourceHelpersOwner<AreasType>> currentOwners = new List<IDataSourceHelpersOwner<AreasType>>();
		internal List<IDataSourceHelpersOwner<AreasType>> CurrentOwners { get { return currentOwners; } }
		public abstract bool Connected { get; }
		protected IQueryExecutor<AreasType> QueryExecutor {
			get { return queryExecutor; }
			set { queryExecutor = value; }
		}
		public QueryMetadataColumns Columns { get { return columns; } }
		#region IQueryMetadata
		protected QueryMetadata() {
			columns = CreateMetadataColumns();
		}
		protected abstract QueryMetadataColumns CreateMetadataColumns();
		public void RegisterOwner(IDataSourceHelpersOwner<AreasType> owner) {
			if(owner == null || currentOwners.Contains(owner))
				return;
			currentOwners.Add(owner);
		}
		public void UnregisterOwner(IDataSourceHelpersOwner<AreasType> owner) {
			if(owner == null)
				return;
			currentOwners.Remove(owner);
			if(currentOwners.Count == 0)
				Disconnect();
		}
		protected internal abstract bool Disconnect();
		public void SaveMember(QueryMember member, TypedBinaryWriter writer) {
			SaveMemberCore(member, writer);
		}
		protected virtual void SaveMemberCore(QueryMember member, TypedBinaryWriter writer) {
			SaveMemberValue(member, writer, false);
		}
		protected void SaveMemberValue(QueryMember member, TypedBinaryWriter writer, bool typed) {
				writer.Write((bool)(member.Value == null));
			if(member.Value != null)
				if(typed)
					writer.WriteTypedObject(member.Value);
				else
					writer.WriteObject(member.Value);
#if DEBUGTEST
			writer.Write((int)23534534);
#endif
		}
		public QueryMember LoadMember(IQueryMetadataColumn column, TypedBinaryReader reader) {
			return LoadMemberCore(column, reader);
		}
		protected abstract QueryMember LoadMemberCore(IQueryMetadataColumn column, TypedBinaryReader reader);
		protected object LoadMemberValue(IQueryMetadataColumn column, TypedBinaryReader reader, bool typed) {
			bool isNull = reader.ReadBoolean();
			object value = null;
			if(!isNull)
				if(typed)
					value = reader.ReadTypedObject();
				else
					value = reader.ReadObject(column.DataType);
#if DEBUGTEST
			int sign = reader.ReadInt32();
			if(sign != 23534534)
				throw new Exception("corr");
#endif
			return value;
		}
		#endregion
		protected internal virtual bool PopulateColumnsCore(IDataSourceHelpersOwner<AreasType> owner) { throw new NotImplementedException(); }
		protected virtual IQueryExecutor<AreasType> CreateQueryExecutor() {
			throw new NotImplementedException();
		}
		public bool HandleException(IDataSourceHelpersOwner<AreasType> desiredOwner, QueryHandleableException raisedException) {
			return (desiredOwner ?? CurrentOwners[0]).HandleException(raisedException);
		}
		public string SaveColumns() {
			return DevExpress.Data.PivotGrid.PivotGridSerializeHelper.ToBase64StringDeflateBuffered(SaveColumnsCore);
		}
		protected virtual void SaveColumnsCore(TypedBinaryWriter writer) {
			Columns.SaveToSteam(writer);
		}
		public void RestoreColumns(string stateString) {
			using(MemoryStream stream = new MemoryStream(Convert.FromBase64String(stateString))) {
				using(DeflateStream decompressor = new DeflateStream(stream, CompressionMode.Decompress)) {
					using(TypedBinaryReader reader = new TypedBinaryReader(decompressor)) {
						RestoreColumnsCore(reader);
					}
				}
			}
		}
		protected virtual void RestoreColumnsCore(TypedBinaryReader reader) {
			Columns.RestoreFromStream(reader);
		}
		protected internal virtual void OnInitialized(IDataSourceHelpersOwner<AreasType> owner) { }
		public bool HasNullValues(IDataSourceHelpersOwner<AreasType> owner, IQueryMetadataColumn column) {
			bool result = HasNullValuesCore(owner, column);
			column.HasNullValues = result;
			return result;
		}
		protected virtual bool HasNullValuesCore(IDataSourceHelpersOwner<AreasType> owner, IQueryMetadataColumn column) {
			bool result = false;
			if(column.HasNullValues == null)
				result = QueryNullValues(owner, column);
			else
				result = column.HasNullValues.Value;
			return result;
		}
		protected internal virtual bool QueryNullValues(IDataSourceHelpersOwner<AreasType> owner, IQueryMetadataColumn column) {
			if(QueryExecutor == null)
				return false;
			return QueryExecutor.QueryNullValues(owner, column);
		}
		internal CellSet<AreasType> QueryData(IDataSourceHelpersOwner<AreasType> owner, IQueryContext<AreasType> context) {
			return QueryExecutor.QueryData(owner, context);
		}
		internal IDataTable QueryDrillDown(IDataSourceHelpersOwner<AreasType> owner, QueryMember[] columnMembers, QueryMember[] rowMembers, AreasType column, int maxRowCount, List<string> customColumns) {
			return QueryExecutor.QueryDrillDown(owner, columnMembers, rowMembers, column, maxRowCount, customColumns);
		}
		internal object[] QueryAvailableValues(IDataSourceHelpersOwner<AreasType> owner, AreasType column, bool deferUpdates, List<AreasType> customFilters) {
			return QueryExecutor.QueryAvailableValues(owner, column, deferUpdates, customFilters);
		}
		internal List<object> QueryVisibleValues(IDataSourceHelpersOwner<AreasType> owner, AreasType column) {
			return QueryExecutor.QueryVisibleValues(owner, column);
		}
		internal virtual void QueryCustomSummary(IDataSourceHelpersOwner<AreasType> owner, IList<AggregationLevel> aggregationLevels) {
			if(QueryExecutor == null) {
				AggregationLevel.SetErrorValue(aggregationLevels);
				return;
			}
			QueryExecutor.QueryAggregations(owner, aggregationLevels);
		}
		internal string GetFieldCaption(string fieldName) {
			if(Columns.Count == 0)
				PopulateColumnsCore(null);
			return Columns.GetFieldCaption(fieldName);
		}
		internal virtual string GetColumnDisplayFolder(string columnName) {
			if(!string.IsNullOrEmpty(columnName)) {
				MetadataColumnBase columnValue;
				if(Columns.TryGetValue(columnName, out columnValue))
					return columnValue.DisplayFolder;
			}
			return string.Empty;
		}
		internal virtual string GetColumnCaption(string columnName) {
			if(!string.IsNullOrEmpty(columnName)) {
				MetadataColumnBase columnValue;
				if(Columns.TryGetValue(columnName, out columnValue))
					return columnValue.Caption;
			}
			return null;
		}
	}
}
