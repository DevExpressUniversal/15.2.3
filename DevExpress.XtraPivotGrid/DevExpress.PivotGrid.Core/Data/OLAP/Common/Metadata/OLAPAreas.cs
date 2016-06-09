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
using DevExpress.Data;
using DevExpress.Data.Filtering;
using DevExpress.Data.IO;
using DevExpress.Data.PivotGrid;
using DevExpress.PivotGrid.QueryMode;
using DevExpress.XtraPivotGrid;
using DevExpress.XtraPivotGrid.Data;
using DevExpress.XtraPivotGrid.Localization;
namespace DevExpress.PivotGrid.OLAP {
	class OLAPCellTable : CellTable<OLAPCubeColumn> {
		public OLAPCellTable(OLAPAreas areas) : base(areas) { }
		public override void Clear() {
			base.Clear();
		}
		protected override MeasureStorageKeepHelperBase CreateSerializeHelper(bool save) {
			return new MeasureStorageKeepHelper();
		}
	}
	class MeasureStorageKeepHelper : MeasureStorageKeepHelperBase {
		public static Func<OlapMeasuresStorage> CreateBySummaryCount(int summaryCount) {
			switch(summaryCount) {
				case 1:
					return () => new OneMeasureStorage();
				case 2:
					return () => new TwoMeasuresStorage();
				case 3:
					return () => new ThreeMeasuresStorage();
				default:
					return () => new MultipleMeasuresStorage();
			}
		}
		internal override MeasuresStorage Load(TypedBinaryReader reader, List<IQueryMetadataColumn> columnIndexes) {
			int summaryCount = reader.ReadInt32();
			OlapMeasuresStorage storage = CreateBySummaryCount(summaryCount)();
			storage.LoadFromStream(reader, columnIndexes, summaryCount);
			return storage;
		}
	}
	public class OLAPAreas : QueryAreas<OLAPCubeColumn> {
		public OLAPAreas(IOLAPHelpersOwner owner)
			: base(owner) {
		}
		protected override bool NeedAddErrorColumnIfInvalid(PivotGridFieldBase field) {
			return field.Area == PivotArea.DataArea;
		}
		protected override CellTable<OLAPCubeColumn> CreateCellTable() {
			return new OLAPCellTable(this);
		}
		protected override bool AllowDuplicatedMeasures() {
			return Owner.Options.AllowDuplicatedMeasures;
		}
		protected new IOLAPHelpersOwner Owner { get { return (IOLAPHelpersOwner)base.Owner; } }
		protected IOLAPMetadata Metadata { get { return Owner.Metadata; } }
		protected OLAPHierarchies Hierarchies { get { return Metadata.Hierarchies; } }	
		protected override QueryMember GetMemberByCondition(IQueryMetadataColumn conditionColumn, PivotGridFieldSortCondition condition) {
			string uniqueName = condition.OLAPUniqueMemberName;
			OLAPMetadataColumn olapColumn = (OLAPMetadataColumn)conditionColumn;
			OLAPMember res = olapColumn[uniqueName];
			if(res == null && !olapColumn.AllMembersLoaded) {
				Metadata.QueryMembers(Owner.CubeColumns[olapColumn], uniqueName);
				res = olapColumn[uniqueName];
			}
			return res;
		}
		protected internal override QueryMember GetMemberByValue(IQueryMetadataColumn conditionColumn, object value) {
			return ((OLAPMetadataColumn)conditionColumn)[(string)value];
		}
		protected override void DoAdditionalSpread(List<OLAPCubeColumn> columnRowFilterFields, List<OLAPCubeColumn>[] areas) {
			if(Owner.Options.UsePrefilter) {
				areas[(int)PivotArea.FilterArea].Clear();
				foreach(OLAPCubeColumn column in Owner.FilterHelper.GetAdditionalFilteredColumns())
					if(!areas[(int)PivotArea.ColumnArea].Contains(column) && !areas[(int)PivotArea.RowArea].Contains(column) && !column.IsMeasure)
						areas[(int)PivotArea.FilterArea].Add(column);
			}
		}
		protected override PivotCellValue GetCellValueCore(GroupInfo column, GroupInfo row, OLAPCubeColumn data) {
			return base.GetCellValueCore(column, row, MetadataColumnBase.GetOriginalColumn(Owner.CubeColumns, data));
		}
		protected override OLAPCubeColumn GetColumnByField(PivotGridFieldBase field) {
			OLAPCubeColumns cubeColumns = Owner.CubeColumns;
			OLAPCubeColumn column;
			if(field.UnboundType != UnboundColumnType.Bound ||
					!cubeColumns.ContainsKey(field.FieldName)) {
				if(!field.IsProcessOnSummaryLevel || field.Area != PivotArea.DataArea || !string.IsNullOrEmpty(field.FieldName) || string.IsNullOrEmpty(field.ExpressionFieldName) || field.UnboundType == UnboundColumnType.Bound)
					return null;
				else {
					if(!cubeColumns.ContainsKey(field.ExpressionFieldName)) {
						column = CreateUnboundSummaryColumn(field, true);
					} else
						column = cubeColumns[field.ExpressionFieldName];
				}
			} else {
				column = cubeColumns[field.FieldName];
			}
			return column;
		}
		protected override OLAPCubeColumn CreateErrorColumn(PivotGridFieldBase field) {
			OLAPCubeColumn column;
			if(field.Area != PivotArea.DataArea)
				column = CreateUnboundSummaryColumn(field, false);
			else
				column = CreateUnboundSummaryErrorColumn(field, false);
			FieldsByColumns.Add(column, field);
			return column;
		}
		OLAPCubeColumn CreateUnboundSummaryColumn(PivotGridFieldBase field, bool add) {
			UnboundSummaryLevelMetadataColumn unboundMetadata = new UnboundSummaryLevelMetadataColumn(field.ActualDataType,
																							  new OLAPHierarchy(field.ExpressionFieldName, field.ExpressionFieldName),
																							  OLAPDataType.Variant, Owner.PatchCriteria(CriteriaOperator.Parse(field.UnboundExpression)),
																							  Cells.Descriptor, Owner);
			OLAPCubeColumn column = Owner.CreateColumn(unboundMetadata, field);
			column.Assign(field, false);
			if(add)
				Owner.CubeColumns.Add(column);
			unboundMetadata.Owner = Owner.Metadata;
			unboundMetadata.EnsureExpressionEvaluator();
			return column;
		}
		OLAPCubeColumn CreateUnboundSummaryErrorColumn(PivotGridFieldBase field, bool add) {
			UnboundSummaryLevelErrorMetadataColumn unboundMetadata = new UnboundSummaryLevelErrorMetadataColumn(field.ActualDataType,
																							  new OLAPHierarchy(field.ExpressionFieldName, field.ExpressionFieldName),
																							  OLAPDataType.Variant);
			OLAPCubeColumn column = Owner.CreateColumn(unboundMetadata, field);
			column.Assign(field, false);
			if(add)
				Owner.CubeColumns.Add(column);
			unboundMetadata.Owner = Owner.Metadata;
			return column;
		}
		protected override bool GetActualProcessModeOnQuery(PivotSummaryType summaryType, OLAPCubeColumn dataField, GroupInfo column, GroupInfo row) {
			return summaryType == PivotSummaryType.Sum;
		}
		protected override IList<QueryMember> GetColumnMembersForSavingToStream(TypedBinaryWriter writer, OLAPCubeColumn column) {
			return column.GetQueryMembers();
		}
		protected override IList<QueryMember> GetColumnMembersForRestoringFromStream(TypedBinaryReader reader, OLAPCubeColumn column) {
			return column.GetQueryMembers();
		}
		protected override PartialUpdaterBase<OLAPCubeColumn> CreatePartialUpdater(PivotGridFieldReadOnlyCollection sortedFields, AreasState<OLAPCubeColumn> oldAreas, List<IQueryMetadataColumn>[] oldMetas, AreasState<OLAPCubeColumn> newAreas, IPartialUpdaterOwner<OLAPCubeColumn> owner, bool isDataSourceFullyExpanded, CollapsedState row, CollapsedState column) {
			return new PartialUpdater(sortedFields, oldAreas, oldMetas, newAreas, owner, isDataSourceFullyExpanded, !Owner.Options.UseDefaultMeasure, row, column);
		}
		protected override CollapsedStateManager<OLAPCubeColumn> CreateCollapsedStateManager() {
			return new OLAPCollapsedStateManager(this, Owner);
		}
		protected override object GetDisplayValueCore(GroupInfo groupInfo) {
			return groupInfo == null ? null : groupInfo.Member.Caption;
		}
	}
}
