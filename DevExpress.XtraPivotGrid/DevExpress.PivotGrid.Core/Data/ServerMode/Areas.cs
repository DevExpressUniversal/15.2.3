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
using System.Linq;
using DevExpress.Data;
using DevExpress.Data.IO;
using DevExpress.Data.PivotGrid;
using DevExpress.PivotGrid.QueryMode;
using DevExpress.XtraPivotGrid;
using DevExpress.XtraPivotGrid.Data;
namespace DevExpress.PivotGrid.ServerMode {
	class Areas : QueryAreas<ServerModeColumn> {
		IServerModeHelpersOwner ServerModeHelpersOwner { get { return (IServerModeHelpersOwner)Owner; } }
		internal Areas(IDataSourceHelpersOwner<ServerModeColumn> owner)
			: base(owner) {
		}
		protected override bool AllowDuplicatedMeasures() {
			return true;
		}
		protected override bool NeedAddErrorColumnIfInvalid(PivotGridFieldBase field) {
			return true;
		}
		protected override ServerModeColumn GetColumnByField(PivotGridFieldBase field) {
			QueryColumns<ServerModeColumn> cubeColumns = Owner.CubeColumns;
			ServerModeColumn column;
			if(field.UnboundType == UnboundColumnType.Bound) {
				bool caseSensitiveDataBinding = ServerModeHelpersOwner.CaseSensitiveDataBinding;
				IQueryMetadataColumn metadataColumn = Owner.Metadata.Columns.FindColumn(field.FieldName, caseSensitiveDataBinding);
				if(metadataColumn == null)
					return null;
				VirtualMetadataColumn virt = null;
				if(field.Area == PivotArea.DataArea || field.GroupInterval != PivotGroupInterval.Default) {
					virt = new VirtualMetadataColumn(metadataColumn);
					metadataColumn = virt;
				}
				if(!cubeColumns.TryGetValue(field, out column)) {
					cubeColumns.Add(Owner.CreateColumn(metadataColumn, field));
				} else {
					if(!string.Equals(column.Name, field.FieldName, caseSensitiveDataBinding ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase)) {
						column = Owner.CreateColumn(metadataColumn, field);
						cubeColumns.Remove(field);
						cubeColumns.Add(column);
					}
					if(column.Metadata != metadataColumn && column.Metadata.GetType() != metadataColumn.GetType()) { 
						column.SetMetadata(metadataColumn);
					}
				}
				column = cubeColumns[field];
				if(virt != null && object.ReferenceEquals(column.Metadata, virt))
					virt.SetIConvertible(column);
			} else {
				if(!CriteriaValidator.IsCriteriaValid(field))
					return field.Area == PivotArea.DataArea ? new ServerModeColumn(new EmptyDataColumn(new PivotGridFieldBase(), ServerModeHelpersOwner, Cells.Descriptor), true) : null;
				if(field.IsProcessOnSummaryLevel && field.UnboundExpressionMode != UnboundExpressionMode.UseAggregateFunctions) {
					if(!cubeColumns.TryGetValue(field, out column)) {
						column = CreateSummaryUnboundField(cubeColumns, field, true);
					} else {
						if(!(column.Metadata is UnboundSummaryLevelColumn)) {
							CubeColumns.Remove(field);
							column = CreateSummaryUnboundField(cubeColumns, field, true);
						}
					}
				} else {
					if(!cubeColumns.TryGetValue(field, out column)) {
						column = CreateServerUnboundField(cubeColumns, field, true);
					} else {
						if(!(column.Metadata is UnboundDataSourceLevelColumn)) {
							CubeColumns.Remove(field);
							column = CreateServerUnboundField(cubeColumns, field, true);
						}
					}
				}
			}
			column.SetIsMeasure(field.Area == PivotArea.DataArea);
			return column;
		}
		protected override ServerModeColumn CreateErrorColumn(PivotGridFieldBase field) {
			ServerModeColumn column;
			if(field.Area == PivotArea.DataArea)
				column = CreateSummaryErrorUnboundField(Owner.CubeColumns, field, false);
			else
				column = CreateServerUnboundField(Owner.CubeColumns, field, false);
			column.SetIsMeasure(field.Area == PivotArea.DataArea);
			return column;
		}
		ServerModeColumn CreateServerUnboundField(QueryColumns<ServerModeColumn> cubeColumns, PivotGridFieldBase field, bool add) {
			ServerModeColumn column;
			UnboundDataSourceLevelColumn unboundMetadata = new UnboundDataSourceLevelColumn(field, ServerModeHelpersOwner);
			column = Owner.CreateColumn(unboundMetadata, field);
			unboundMetadata.OwnerColumn = column;
			column.Assign(field, false);
			if(add)
				cubeColumns.Add(column);
			unboundMetadata.Owner = Owner.Metadata;
			return column;
		}
		ServerModeColumn CreateSummaryUnboundField(QueryColumns<ServerModeColumn> cubeColumns, PivotGridFieldBase field, bool add) {
			UnboundSummaryLevelColumn unboundMetadata = new UnboundSummaryLevelColumn(field, ServerModeHelpersOwner, Cells.Descriptor);
			ServerModeColumn column = Owner.CreateColumn(unboundMetadata, field);
			column.Assign(field, false);
			if(add)
				cubeColumns.Add(column);
			unboundMetadata.Owner = Owner.Metadata;
			return column;
		}
		ServerModeColumn CreateSummaryErrorUnboundField(QueryColumns<ServerModeColumn> cubeColumns, PivotGridFieldBase field, bool add) {
			UnboundSummaryLevelColumn unboundMetadata = new UnboundSummaryLevelErrorColumn(field, ServerModeHelpersOwner, Cells.Descriptor);
			ServerModeColumn column = Owner.CreateColumn(unboundMetadata, field);
			column.Assign(field, false);
			if(add)
				cubeColumns.Add(column);
			unboundMetadata.Owner = Owner.Metadata;
			return column;
		}
		protected override IList<QueryMember> GetColumnMembersForSavingToStream(TypedBinaryWriter writer, ServerModeColumn column) {
			IList<QueryMember> result;
			bool isColumn = false;
			int level = RowArea.IndexOf(column);
			if(level == -1) {
				level = ColumnArea.IndexOf(column);
				isColumn = true;
			}
			if(level == -1)
				result = new QueryMember[0];
			else {
				NullableDictionary<object, QueryMember> dic = new NullableDictionary<object, QueryMember>();
				GetFieldValues(isColumn).ForEachGroupInfo((list, b) => {
					if(b == level)
						list.ForEach((gi) => dic[gi.Member.Value] = gi.Member);
				}, level);
				result = dic.Values.ToArray();
			}
			writer.Write(result.Count);
			IQueryMetadata meta = Owner.Metadata;
			for(int i = 0; i < result.Count; i++)
				meta.SaveMember(result[i], writer);
			return result;
		}
		protected override IList<QueryMember> GetColumnMembersForRestoringFromStream(TypedBinaryReader reader, ServerModeColumn column) {
			int count = reader.ReadInt32();
			List<QueryMember> list = new List<QueryMember>(count);
			IQueryMetadata meta = Owner.Metadata;
			IQueryMetadataColumn metaColumn = column.Metadata;
			for(int i = 0; i < count; i++)
				list.Add(meta.LoadMember(metaColumn, reader));
			return list;
		}
		protected override QueryMember GetMemberByCondition(IQueryMetadataColumn conditionColumn, PivotGridFieldSortCondition condition) {
			return new ServerModeMember(conditionColumn, condition.Value);
		}
		protected internal override QueryMember GetMemberByValue(IQueryMetadataColumn conditionColumn, object value) {
			 return new ServerModeMember(conditionColumn, value);
		}
		protected override CellTable<ServerModeColumn> CreateCellTable() {
			return new ServerModeCellTable(this);
		}
		protected override PartialUpdaterBase<ServerModeColumn> CreatePartialUpdater(PivotGridFieldReadOnlyCollection sortedFields, AreasState<ServerModeColumn> oldAreas, List<IQueryMetadataColumn>[] oldMetas, AreasState<ServerModeColumn> newAreas, IPartialUpdaterOwner<ServerModeColumn> owner, bool isDataSourceFullyExpanded, CollapsedState row, CollapsedState column) {
			return new PartialUpdater(sortedFields, oldAreas, oldMetas, newAreas, owner, isDataSourceFullyExpanded, row, column);
		}
		protected override void DoAdditionalSpread(List<ServerModeColumn> columnRowFilterFields, List<ServerModeColumn>[] areas) {
			base.DoAdditionalSpread(columnRowFilterFields, areas);
			List<KeyValuePair<string, ServerModeColumn>> invalidList = new List<KeyValuePair<string, ServerModeColumn>>();
			foreach(KeyValuePair<string, ServerModeColumn> pair in CubeColumns)
				if(!areas[0].Contains(pair.Value) && !areas[1].Contains(pair.Value) && !areas[2].Contains(pair.Value) && !areas[3].Contains(pair.Value))
					invalidList.Add(pair);
			foreach(KeyValuePair<string, ServerModeColumn> pair in invalidList)
				if(!FieldsByColumns.ContainsKey(pair.Value))
					CubeColumns.Remove(pair.Key);
		}
	}
}
