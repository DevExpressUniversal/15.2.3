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
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using DevExpress.Compatibility.System;
using DevExpress.Compatibility.System.ComponentModel;
using DevExpress.DataAccess.Localization;
using DevExpress.DataAccess.Sql;
using DevExpress.DataAccess.Sql.DataApi;
namespace DevExpress.DataAccess.Native.Sql {
	public class ResultRelation : PropertyDescriptor, IRelation, ICloneable {
		struct IndexPair {
			public int MasterColumnIndex;
			public int DetailColumnIndex;
		}
		static object TryConvertValue(object value, Type type) {
			if(value.GetType() == type)
				return value;
			try {
				return Convert.ChangeType(value, type);
			}
			catch {
				return null;
			}
		}
		readonly ResultTable master;
		readonly ResultTable detail;
		IndexPair[] indexPairsKeyColumns;
		readonly IEnumerable<RelationColumnInfo> keyColumns;
		readonly Dictionary<ResultRow, FilteredResultList> resultSets = new Dictionary<ResultRow, FilteredResultList>();
		IDictionary index;
		Type detailsKeyType;
		public ResultRelation(ResultTable master, ResultTable detail, IEnumerable<RelationColumnInfo> keyColumns)
			: this(master, detail, keyColumns, master.TableName + detail.TableName) { }
		public ResultRelation(ResultTable master, ResultTable detail, IEnumerable<RelationColumnInfo> keyColumns, string name)
			: base(name, new Attribute[0]) {
			this.master = master;
			this.detail = detail;
			this.keyColumns = keyColumns;
		}
		IndexPair[] IndexPairsKeyColumns {
			get {
				if(indexPairsKeyColumns == null) {
					this.indexPairsKeyColumns = keyColumns.Select((colInfo, i) => {
						int mstIdx = master.Columns.FindIndex(col => col.Name == colInfo.ParentKeyColumn);
						if(mstIdx == -1)
							throw new InvalidOperationException(string.Format(DataAccessLocalizer.GetString(DataAccessStringId.ResultRelation_ColumnNotFoundError),
								master.TableName,
								colInfo.ParentKeyColumn));
						int detIdx = detail.Columns.FindIndex(col => col.Name == colInfo.NestedKeyColumn);
						if(detIdx == -1)
							throw new InvalidOperationException(string.Format(DataAccessLocalizer.GetString(DataAccessStringId.ResultRelation_ColumnNotFoundError),
								detail.TableName,
								colInfo.NestedKeyColumn));
						return new IndexPair { MasterColumnIndex = mstIdx, DetailColumnIndex = detIdx };
					}).ToArray();
				}
				return indexPairsKeyColumns;
			}
		}
		#region Overrides of PropertyDescriptor
		public override bool CanResetValue(object component) { return false; }
		public override object GetValue(object component) {
			ResultRow masterRow = (ResultRow)component;
			if(this.resultSets.ContainsKey(masterRow))
				return this.resultSets[masterRow];
			int length = this.IndexPairsKeyColumns.Length;
			object[] masterValues = new object[length];
			for(int i = 0; i < length; i++) {
				object masterValue = this.master.Columns[this.IndexPairsKeyColumns[i].MasterColumnIndex].GetValue(masterRow);
				if(masterValue == null)
					return new FilteredResultList(Detail, new List<ResultRow>(0));
				masterValues[i] = masterValue;
			}
			List<ResultRow> results = new List<ResultRow>();
			if(this.index == null)
				CreateIndex();
			object value = masterValues[0];
			IDictionary dictionary = this.index;
			if(dictionary == null)
				return null;
			value = TryConvertValue(value, this.detailsKeyType);
			if(value != null && dictionary.Contains(value))
				results = (List<ResultRow>)dictionary[value];
			for(int i = 1; i < length; i++) {
				List<ResultRow> filtered = new List<ResultRow>();
				foreach(ResultRow resultRow in results) {
					IndexPair pair = IndexPairsKeyColumns[i];
					object detailValue = this.detail.Columns[pair.DetailColumnIndex].GetValue(resultRow);
					if(detailValue == null)
						continue;
					detailValue = TryConvertValue(detailValue, masterValues[i].GetType());
					if(detailValue == null)
						continue;
					if(masterValues[i].GetHashCode() != detailValue.GetHashCode())
						continue;
					if(masterValues[i].Equals(detailValue))
						filtered.Add(resultRow);
				}
				results = filtered;
				if(filtered.Count == 0)
					break;
			}
			FilteredResultList filteredResultSet = new FilteredResultList(Detail, results);
			this.resultSets.Add(masterRow, filteredResultSet);
			return filteredResultSet;
		}
		void CreateIndex() {
			int keyColumnIndex = IndexPairsKeyColumns[0].DetailColumnIndex;
			this.detailsKeyType = Detail.Columns[keyColumnIndex].PropertyType;
			this.index = (IDictionary)Activator.CreateInstance(typeof(Dictionary<,>).MakeGenericType(this.detailsKeyType, typeof(List<ResultRow>)));
			foreach(ResultRow row in Detail) {
				object value = Detail.Columns[keyColumnIndex].GetValue(row);
				if(value == null || value is DBNull)
					continue;
				if(!this.index.Contains(value))
					this.index.Add(value, new List<ResultRow>(new[] { row }));
				else
					((List<ResultRow>)this.index[value]).Add(row);
			}
		}
		public override void ResetValue(object component) { throw new NotSupportedException(); }
		public override void SetValue(object component, object value) { throw new NotSupportedException(); }
		public override bool ShouldSerializeValue(object component) { return false; }
		public override Type ComponentType { get { return typeof(ResultRow); } }
		public override bool IsReadOnly { get { return true; } }
		public override Type PropertyType { get { return typeof(FilteredResultList); } }
		#endregion
		#region Implementation of IRelation
		ITable IRelation.Master { get { return Master; } }
		ITable IRelation.Detail { get { return Detail; } }
		IEnumerable<IColumn> IRelation.MasterKeyColumns { get { return KeyColumns.Select(info => ((ITable)Master).GetColumn(info.ParentKeyColumn)); } }
		IEnumerable<IColumn> IRelation.DetailKeyColumns { get { return KeyColumns.Select(info => ((ITable)Detail).GetColumn(info.NestedKeyColumn)); } }
		IEnumerable<IRow> IRelation.GetDetailRows(IRow masterRow) { return (ITable)GetValue(masterRow); }
		IEnumerable<IRow> IRelation.GetDetailRows(int masterRowIndex) {
			return ((IRelation)this).GetDetailRows(((ITable)Master)[masterRowIndex]);
		}
		#endregion
		public ResultTable Master { get { return this.master; } }
		public ResultTable Detail { get { return this.detail; } }
		public IEnumerable<RelationColumnInfo> KeyColumns {
			get {
				return keyColumns;
			}
		}
		public object Clone() {
			return new ResultRelation(Master, Detail, KeyColumns);
		}
	}
}
