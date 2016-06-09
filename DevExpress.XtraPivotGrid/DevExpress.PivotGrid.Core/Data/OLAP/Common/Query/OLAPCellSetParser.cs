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
using DevExpress.PivotGrid.QueryMode;
using DevExpress.XtraPivotGrid;
namespace DevExpress.PivotGrid.OLAP {
public abstract class OLAPCellSetParserBase {
		public abstract CellSet<OLAPCubeColumn> QueryData(IOLAPCommand command, IQueryContext<OLAPCubeColumn> context);
		public abstract List<object> QueryVisibleOrAvailableValues(IOLAPCommand command, OLAPCubeColumn olapColumn);
		public abstract IEnumerable<object> QueryValue(IOLAPCommand command);
		public abstract List<OLAPMember> QueryMembers(IOLAPCommand command, OLAPMetadataColumn meta, OLAPCubeColumn childColumn, OLAPChildMember olapMember);
		public abstract PivotOLAPKPIValue QueryKPIValue(IOLAPCommand command, PivotOLAPKPIMeasures measures, string kpiName, OLAPMetadata metadata);
		public abstract IOLAPRowSet QueryDrillDown(IOLAPCommand command);
		protected void InitKpiParameters(PivotOLAPKPIMeasures measures, string name, object cellValue,
					ref object value, ref object goal, ref int status, ref int trend, ref double weight) {
			if(name == measures.ValueMeasure)
				value = cellValue;
			if(name == measures.GoalMeasure)
				goal = cellValue;
			if(name == measures.StatusMeasure)
				status = Convert.ToInt32(cellValue);
			if(name == measures.TrendMeasure)
				trend = Convert.ToInt32(cellValue);
			if(name == measures.WeightMeasure)
				weight = Convert.ToDouble(cellValue);
		}
	}
	public abstract class OLAPCellSetParser : OLAPCellSetParserBase {
		IOLAPMetadata owner;
		protected OLAPCellSetParser(IOLAPMetadata owner) {
			this.owner = owner;
		}
		protected IOLAPMetadata Owner { get { return owner; } }
		protected OLAPCachedCellSet CreateProviderCachedCellSet(AxisColumnsProviderBase axisColumnsProvider, IOLAPCellSet olapCellSet) {
			if(axisColumnsProvider == null)
				throw new ArgumentNullException("AxisColumnsProvider");
			return new OLAPCachedCellSet(axisColumnsProvider, olapCellSet);
		}
		protected abstract IOLAPCellSet ExecuteCellSet(IOLAPCommand command, IQueryContext<OLAPCubeColumn> context);
		protected PivotOLAPKPIValue ParseKPIValues(OLAPCachedCellSet serverCellSet, PivotOLAPKPIMeasures measures) {
			ITupleCollection columnTuples = serverCellSet.ColumnAxis;
			object value = null, goal = null;
			int status = 0, trend = 0;
			double weight = double.NaN;
			int index = 0;
			List<string> tuples = new List<string>();
			foreach(IOLAPTuple tuple in columnTuples)
				tuples.Add(tuple[0].UniqueName);
			foreach(string name in tuples) {
				object cellValue = serverCellSet.GetColumnValueValueSafe(index);
				InitKpiParameters(measures, name, cellValue,
					ref value, ref goal, ref status, ref trend, ref weight);
				index++;
			}
			return new PivotOLAPKPIValue(value, goal, status, trend, weight);
		}
		public override CellSet<OLAPCubeColumn> QueryData(IOLAPCommand command, IQueryContext<OLAPCubeColumn> context) {
			IOLAPCellSet cellSet = ExecuteCellSet(command, context);
			CellSet<OLAPCubeColumn> result;
			try {
				result = CellSetCreator.CreateCellSet(CreateProviderCachedCellSet(new ByAreasAxisColumnProvider(context.Areas, (OLAPMetadata)context.Owner.Metadata), cellSet), context, context.Owner);
			} finally {
				cellSet.OnParsed();
			}
			return result;
		}
		public override List<object> QueryVisibleOrAvailableValues(IOLAPCommand command, OLAPCubeColumn olapColumn) {
			IOLAPCellSet cellSet = ExecuteCellSet(command, null);
			List<object> result;
			try {
				List<string> visibleMembers = ReadColumnMemberNamesCore(olapColumn, cellSet);
				result = new List<object>(visibleMembers.Count);
				if(olapColumn.OLAPFilterByUniqueName) {
					foreach(string uniqueName in visibleMembers) {
						result.Add(uniqueName);
					}
				} else {
					foreach(string uniqueName in visibleMembers) {
						OLAPMember member = olapColumn[uniqueName];
						if(member != null)
							result.Add(member.Value);
					}
				}
			} finally {
				cellSet.OnParsed();
			}
			return result;
		}
		public override IEnumerable<object> QueryValue(IOLAPCommand command) {
			IOLAPCellSet cellSet = ExecuteCellSet(command, null);
			try {
				foreach(IOLAPCell cell in cellSet.Cells)
					yield return cell.Value;
			} finally {
				cellSet.OnParsed();
			}
		}
		public override List<OLAPMember> QueryMembers(IOLAPCommand command, OLAPMetadataColumn meta, OLAPCubeColumn childColumn, OLAPChildMember olapMember) {
			IOLAPCellSet cellSet = ExecuteCellSet(command, null);
			List<OLAPMember> childMembers;
			try {
				childMembers = ReadColumnMembersCore(meta, childColumn, cellSet);
				if(olapMember != null)
					olapMember.SetChildMembers(childMembers);
			} finally {
				cellSet.OnParsed();
			}
			return childMembers;
		}
		public override PivotOLAPKPIValue QueryKPIValue(IOLAPCommand command, PivotOLAPKPIMeasures measures, string kpiName, OLAPMetadata metadata) {
			IOLAPCellSet cellSet = ExecuteCellSet(command, null);
			PivotOLAPKPIValue result;
			try {
				OLAPCachedCellSet cachedCellSet = CreateProviderCachedCellSet(new KpiAxisColumnProvider(metadata), cellSet); 
				result = ParseKPIValues(cachedCellSet, measures);
			} finally {
				cellSet.OnParsed();
			}
			return result;
		}
		List<string> ReadColumnMemberNamesCore(OLAPCubeColumn column, IOLAPCellSet cellSet) {
			int memberIndex = -1;
			List<string> members = new List<string>();
			foreach(IOLAPTuple tuple in cellSet.GetRowAxis(new OneColumnAxisColumnsProvider(column, column.Metadata))) {
				if(memberIndex < 0) {
					for(int i = 0; i < tuple.Count; i++) {
						if(tuple[i].Column == column.Metadata) {
							memberIndex = i;
							break;
						}
					}
					if(memberIndex < 0)
						return members;
				}
				members.Add(tuple[memberIndex].UniqueName);
			}
			return members;
		}
		List<OLAPMember> ReadColumnMembersCore(OLAPMetadataColumn olapColumn, OLAPCubeColumn column, IOLAPCellSet cellSet) {
			List<OLAPMember> members = new List<OLAPMember>();
			foreach(IOLAPTuple tuple in (IEnumerable<IOLAPTuple>)cellSet.GetRowAxis(new OneColumnAxisColumnsProvider(column, olapColumn)))
				members.Add(tuple.Single());
			return members.Count > 0 ? members : null;
		}
	}
}
