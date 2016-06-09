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
using System.Data;
using System.Linq;
using System.Text;
using DevExpress.PivotGrid.QueryMode;
namespace DevExpress.PivotGrid.OLAP {
	class OleCellSet : IOLAPCellSet {
		OleAxisCollection axes = new OleAxisCollection();
		List<string> columns = new List<string>();
		List<object[]> rows= new List<object[]>();
		int rowAreaColumnsCount;
		public int RowAreaColumnsCount { get { return rowAreaColumnsCount; } }
		public object GetValue(int columnIndex, int rowIndex) {
			return rows[rowIndex][columnIndex];
		}
		public int ColumnCount { get { return columns.Count; } }
		public int RowCount { get { return rows.Count; } }
		public OleCellSet(IDataReader reader, IOLAPQueryContext context) {
			Read(reader);
			rowAreaColumnsCount = columns.FindIndex(delegate(string target) { return !target.EndsWith(OLAPDataSourceQueryBase.MemberUniqueNameName); });
			axes.Add(GetColumnsTuples(context));
			OleDbAxis rowAxes = GetRowsTuples(context);
			if(rowAxes.Tuples != null)
				axes.Add(rowAxes);
		}
		void Read(IDataReader reader) {
			for(int i = 0; i < reader.FieldCount; i++)
				columns.Add(reader.GetName(i));
			while(reader.Read()) {
				object[] row = new object[reader.FieldCount];
				reader.GetValues(row);
				for(int i = 0; i < row.Length; i++)
					if(row[i] is DBNull)
						row[i] = null;
				rows.Add(row);
			}
		}
				void IOLAPCellSet.OnParsed() { }
		OleDbAxis GetRowsTuples(IOLAPQueryContext olapContext) {
			bool rowExpand = olapContext.RowExpand;
			OleDbTupleCollection tuples = new OleDbTupleCollection(RowCount);
			if(rowAreaColumnsCount == 0)
				return new OleDbAxis(null);
			List<OLAPMetadataColumn> areaColumns = new List<OLAPMetadataColumn>();
			for(int i = 0; i < rowAreaColumnsCount; i++) {
				string name = columns[i];
				name = name.Substring(0, name.Length - (OLAPDataSourceQueryBase.MemberUniqueNameName).Length - 1);
				OLAPCubeColumn col = olapContext.Areas.RowArea.Find(a => a.UniqueName == name);
				if(col != null)
					areaColumns.Add(col.Metadata);
				else
					areaColumns.Add(null);
			}
			IQueryMetadataColumn expanded = null;
			IQueryMetadataColumn preExpanded = null;
			if(rowExpand && olapContext.RowArea[0].ParentColumn != null) {
				int index = olapContext.Areas.RowArea.IndexOf(olapContext.RowArea[0]);
				if(index > 0 && olapContext.Areas.RowArea[index - 1].IsParent(olapContext.RowArea[0])) {
					expanded = olapContext.RowArea[0].Metadata;
					preExpanded = olapContext.Areas.RowArea[index - 1].Metadata;
				}
			}
			int notNullCount = areaColumns.Where(a => a != null).Count();
			bool columnExpand = olapContext.ColumnExpand;
			for(int i = 0; i < RowCount; i++) {
				OleDbTuple tuple = new OleDbTuple();
				for(int j = 0; j < rowAreaColumnsCount; j++) {
					string name = rows[i][j] as string;
					bool nb = false;
					OLAPMetadataColumn meta = areaColumns[j];
					if(meta == null)
						continue;
					bool isPrevParent = tuple.Count != 0 && tuple[tuple.Count - 1].Column.IsParent(meta);
					if(name == null) {
						if(tuple.Count != 0 && (tuple.Count != 0 && !columnExpand || isPrevParent)) {
							break;
						}
						name = meta.AllMemberUniqueName;
						nb = true;
					}
					if(isPrevParent && name != meta.AllMemberUniqueName) {
						tuple.RemoveAt(tuple.Count - 1);
					}
					InitMember(tuple, new OleMemberWrapper(name, meta), meta, olapContext.RowArea[0]);
					if(nb)
						break;
				}
				tuples.Add(tuple);
			}
			QueryNewMembers(olapContext);
			return new OleDbAxis(tuples);
		}
		Dictionary<string, List<OleMemberWrapper>> toRequest = new Dictionary<string, List<OleMemberWrapper>>();
		void InitMember(OleDbTuple tuple, OleMemberWrapper wrapper, OLAPMetadataColumn desiredColumn, OLAPCubeColumn areaColumn) {
			if(wrapper.NeedMember) {
				List<OleMemberWrapper> members;
				string columnName = wrapper.HasMember ? wrapper.LevelName : desiredColumn.UniqueName;
				if(!toRequest.TryGetValue(columnName, out members)) {
					members = new List<OleMemberWrapper>();
					toRequest.Add(columnName, members);
				}
				if(!wrapper.HasMember)
					wrapper.Column = desiredColumn;
				members.Add(wrapper);
			}
			tuple.Add(wrapper);
		}
		void QueryNewMembers(IOLAPQueryContext olapContext) {
			if(toRequest.Count == 0)
				return;
			foreach(KeyValuePair<string, List<OleMemberWrapper>> pair in toRequest) {
				olapContext.QueryMembers(olapContext.Owner.CubeColumns[pair.Value[0].Column.UniqueName], pair.Value.ConvertAll(a => a.UniqueName).ToArray());
				foreach(OleMemberWrapper wrapper in pair.Value)
					wrapper.Bind(null, false);
			}
			toRequest.Clear();
		}
		OleDbAxis GetColumnsTuples(IOLAPQueryContext olapContext) {
			List<OLAPCubeColumn> allArea = olapContext.Areas.ColumnArea;
			Dictionary<string, Dictionary<string, List<IQueryMetadataColumn>>> starts = new Dictionary<string, Dictionary<string, List<IQueryMetadataColumn>>>();
			for(int j = 0; j < allArea.Count; j++) {
				OLAPHierarchy hierarchy = allArea[j].Hierarchy;
				Dictionary<string, List<IQueryMetadataColumn>> names;
				if(!starts.TryGetValue(hierarchy.Dimension, out names)) {
					names = new Dictionary<string, List<IQueryMetadataColumn>>();
					starts.Add(hierarchy.Dimension, names);
				}
				string hName;
				List<IQueryMetadataColumn> cols;
				if(allArea[j].UniqueName.Split('.').Length == 2) {
					hName = allArea[j].AllMember.UniqueName.Substring(hierarchy.Dimension.Length + 1);
				} else {
					hName = string.Format("[{0}]", hierarchy.Name);
				}
				if(!names.TryGetValue(hName, out cols)) {
					cols = new List<IQueryMetadataColumn>();
					names.Add(hName, cols);
				}
				cols.Add(allArea[j].Metadata);
			}
			OleDbTupleCollection tuples = new OleDbTupleCollection(ColumnCount);
			Dictionary<string, OleMeasureMemberWrapper> measureMembers = new Dictionary<string, OleMeasureMemberWrapper>();
			foreach(string str in columns.Skip(rowAreaColumnsCount)) {
				string[] blocks = str.Split('.');
				for(int i = blocks.Length - 1; i > 0; i--)
					if(blocks[i - 1][blocks[i - 1].Length - 1] != ']') {
						blocks[i - 1] = blocks[i - 1] + "." + blocks[i];
						for(int j = i; j < blocks.Length - 1; j++)
							blocks[j] = blocks[j + 1];
						Array.Resize(ref blocks, blocks.Length - 1);
					}
				int blockStart = 0;
				int nextBlockStart = 1;
				int lastBlockIndex = blocks.Length - 2;
				OleDbTuple tuple = new OleDbTuple();
				while(nextBlockStart < lastBlockIndex) {
					List<IQueryMetadataColumn> meta = null;
					while(true) {
						Dictionary<string, List<IQueryMetadataColumn>> cols;
						if(!starts.TryGetValue(blocks[nextBlockStart], out cols) || !cols.TryGetValue(blocks[nextBlockStart + 1], out meta)) {
							nextBlockStart++;
							if(nextBlockStart == lastBlockIndex)
								break;
							continue;
						}
						break;
					}
					InitMember(tuple, new OleMemberWrapper(GetMemberName(blocks, blockStart, nextBlockStart), starts[blocks[blockStart]][blocks[blockStart + 1]]), olapContext.ColumnArea[0].Metadata, olapContext.ColumnArea[0]);
					blockStart = nextBlockStart;
					nextBlockStart++;
				}
				OleMeasureMemberWrapper measure;
				string measureName = string.Format("{0}.{1}", blocks[blockStart], blocks[blockStart + 1]);
				if(!measureMembers.TryGetValue(measureName, out measure)) {
					measure = new OleMeasureMemberWrapper(olapContext.Owner.CubeColumns[measureName]);
					measureMembers.Add(measureName, measure);
				}
				tuple.Add(measure);
				tuples.Add(tuple);
			}
			QueryNewMembers(olapContext);
			return new OleDbAxis(tuples);
		}
		string GetMemberName(string[] blocks, int blockStart, int nextBlockStart) {
			StringBuilder sb = new StringBuilder();
			for(int k = blockStart; k < nextBlockStart; k++)
				sb.Append(blocks[k]).Append('.');
			sb.Length = sb.Length - 1;
			return sb.ToString();
		}
		#region IOLAPCellSet
		IEnumerable<IOLAPCell> IOLAPCellSet.Cells {
			get {
				return new CellEnumerator(this);
			}
		}
		class CellEnumerator : IOLAPCollection<IOLAPCell> {
			OleCellSet set;
			int rowStart;
			public CellEnumerator(OleCellSet set) {
				this.set = set;
				rowStart = set.RowAreaColumnsCount;
			}
			IOLAPCell IOLAPCollection<IOLAPCell>.this[int index] {
				get { throw new NotImplementedException(); }
			}
			int IOLAPCollection.Count {
				get { return (set.ColumnCount - rowStart) * set.RowCount; }
			}
			IEnumerator IEnumerable.GetEnumerator() {
				return ((IEnumerable<OleDbCell>)this).GetEnumerator();
			}
			IEnumerator<IOLAPCell> IEnumerable<IOLAPCell>.GetEnumerator() {
				for(int i = 0; i < set.RowCount; i++)
					for(int j = rowStart; j < set.ColumnCount; j++)
						yield return new OleDbCell(set.GetValue(j, i));
			}
		}
		ITupleCollection IOLAPCellSet.GetColumnAxis(AxisColumnsProviderBase axisColumnsProvider) {
			return axes.Count > 0 ? axes[0].Tuples : null;
		}
		ITupleCollection IOLAPCellSet.GetRowAxis(AxisColumnsProviderBase axisColumnsProvider) {
			return axes.Count > 1 ? axes[1].Tuples : null;
		}
		#endregion
	}
}
