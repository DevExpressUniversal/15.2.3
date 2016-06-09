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
using System.Linq;
using DevExpress.PivotGrid.QueryMode;
using DevExpress.Utils;
namespace DevExpress.PivotGrid.ServerMode {
	class ColumnState {
		readonly bool isColumn;
		readonly IServerModeHelpersOwner owner;
		readonly ColumnState preColumn;
		ColumnState nextColumn;
		readonly ServerModeColumn column;
		readonly MultipleLevelsQueryExecutor executor;
		List<QueryMember> staticMembers;
		List<QueryMember> StaticMembers {
			get {
				if(staticMembers == null && column.TopValueCount > 0 && (column.TopValueMode != XtraPivotGrid.TopValueMode.ParentFieldValues || preColumn == null))
					staticMembers = DXListExtensions.ConvertAll<object, QueryMember>(executor.GetTopSelectedRows(owner, column, (IRawCriteriaConvertible)null, MultipleLevelsQueryExecutor.CreateSortByCriteria(column, owner.Areas.ServerSideDataArea)), (v) => new ServerModeMember(column.Metadata, v)).ToList();
				return staticMembers;
			}
		}
		bool ShowOthers {
			get {
				return column.TopValueCount > 0 && column.TopValueShowOthers && (owner.Areas.DataArea.Count > 0 || column.SortBySummary != null);
			}
		}
		public bool ShowTotal {
			get {
				return column.CalculateTotals;
			}
		}
		public ColumnState(bool isColumn, IServerModeHelpersOwner owner, ColumnState preColumn, ServerModeColumn column, MultipleLevelsQueryExecutor executor) {
			this.isColumn = isColumn;
			this.owner = owner;
			this.preColumn = preColumn;
			if(preColumn != null)
				preColumn.nextColumn = this;
			this.column = column;
			this.executor = executor;
		}
		List<BaseCriteriaHolder> cachedFullBrach;
		public IEnumerable<BaseCriteriaHolder> GetFullBranch() {
			if(cachedFullBrach != null)
				return cachedFullBrach;
			cachedFullBrach = new List<BaseCriteriaHolder>();
			foreach(BaseCriteriaHolder holder in GetBranch(false))
				cachedFullBrach.Add(new AndCriteriaHolder(holder, nextColumn != null ? nextColumn.GetUpstairsCriteria() : null));
			return cachedFullBrach;
		}
		public IRawCriteriaConvertible GetUpstairsCriteria() {
			return GetUpstairsCriteriaCore();
		}
		IRawCriteriaConvertible cachedUpstairs = null;
		IRawCriteriaConvertible GetUpstairsCriteriaCore() {
			if(cachedUpstairs != null)
				return cachedUpstairs;
			IRawCriteriaConvertible thisCriteria = null;
			if(column.TopValueCount > 0 && !column.TopValueShowOthers) {
				if(column.TopValueMode != XtraPivotGrid.TopValueMode.ParentFieldValues) {
					thisCriteria = new OrMembers(StaticMembers);
					IRawCriteriaConvertible conv = nextColumn == null ? null : nextColumn.GetUpstairsCriteria();
					cachedUpstairs = conv == null ? thisCriteria : new AndCriteria(thisCriteria, conv);
				} else
					if(!column.TopValueHiddenOthersShowedInTotal) {
						IRawCriteriaConvertible conv = nextColumn == null ? null : nextColumn.GetUpstairsCriteria();
						cachedUpstairs = conv == null ? (IRawCriteriaConvertible)new OrCrossCriteriaHolder(GetBranch(false)) : new OrCrossJoinCriteria(GetBranch(false), conv);
					} else {
						cachedUpstairs = nextColumn == null ? null : nextColumn.GetUpstairsCriteria();
					}
			} else
				cachedUpstairs = nextColumn == null ? null : nextColumn.GetUpstairsCriteria();
			return cachedUpstairs;
		}
		IEnumerable<BaseCriteriaHolder> GetBranch(bool detalizeMembers) {
			if(!detalizeMembers) {
				if(column.TopValueCount != 0)
					return GetTopN(detalizeMembers);
				else
					return preColumn != null ? preColumn.GetBranch(detalizeMembers) : new List<BaseCriteriaHolder>() { BaseCriteriaHolder.Empty };
			} else {
				if(column.TopValueCount != 0) {
					return GetTopN(detalizeMembers);
				} else {
					return GetDetalizedLostBranch();
				}
			}
		}
		IEnumerable<BaseCriteriaHolder> GetDetalizedLostBranch() {
			IEnumerable<BaseCriteriaHolder> pre = preColumn != null ? preColumn.GetBranch(true) : null;
			if(pre != null) {
				foreach(BaseCriteriaHolder conv in pre) {
					List<object> vals = executor.GetTopSelectedRows(owner, column, conv, null);
					foreach(object val in vals)
						yield return new OrCrossJoinCriteria(new List<BaseCriteriaHolder>() { conv }, new InMembers(column, new List<QueryMember>() { new ServerModeMember(column.Metadata, val) }));
				}
			} else {
				List<object> vals = executor.GetTopSelectedRows(owner, column, (IRawCriteriaConvertible)null, null);
				foreach(object val in vals)
					yield return new OrCrossJoinCriteria(new List<BaseCriteriaHolder>() { }, new InMembers(column, new List<QueryMember>() { new ServerModeMember(column.Metadata, val) }));
			}
		}
		IEnumerable<BaseCriteriaHolder> GetTopN(bool detalizeMembers) {
			List<BaseCriteriaHolder> result = new List<BaseCriteriaHolder>();
			if(column.TopValueMode != XtraPivotGrid.TopValueMode.ParentFieldValues) {
				IEnumerable<BaseCriteriaHolder> preMembersList = preColumn != null ? preColumn.GetBranch(false) : null;
				if(preMembersList != null)
					foreach(BaseCriteriaHolder conv in preMembersList)
						AddMembers(result, detalizeMembers, StaticMembers, new List<BaseCriteriaHolder>() { conv });
				else
					AddMembers(result, detalizeMembers, StaticMembers, preMembersList);
			} else {
				IEnumerable<BaseCriteriaHolder> preMembersList = preColumn != null ? preColumn.GetBranch(true) : null;
				if(preMembersList != null)
					foreach(BaseCriteriaHolder conv in preMembersList) {
						List<QueryMember> columnValues = DXListExtensions.ConvertAll<object, QueryMember>(executor.GetTopSelectedRows(owner, column, conv, MultipleLevelsQueryExecutor.CreateSortByCriteria(column, owner.Areas.ServerSideDataArea)), (v) => new ServerModeMember(column.Metadata, v)).ToList();
						AddMembers(result, detalizeMembers, columnValues, new List<BaseCriteriaHolder>() { conv });
					} else {
					List<QueryMember> columnValues = DXListExtensions.ConvertAll<object, QueryMember>(executor.GetTopSelectedRows(owner, column, (IRawCriteriaConvertible)null, MultipleLevelsQueryExecutor.CreateSortByCriteria(column, owner.Areas.ServerSideDataArea)), (v) => new ServerModeMember(column.Metadata, v)).ToList();
					AddMembers(result, detalizeMembers, columnValues, preMembersList);
				}
			}
			return result;
		}
		void AddMembers(List<BaseCriteriaHolder> result, bool detalizeMembers, List<QueryMember> columnValues, IEnumerable<BaseCriteriaHolder> preMembersList) {
			if(detalizeMembers)
				foreach(ServerModeMember member in columnValues)
					result.Add(new OrCrossJoinCriteria(preMembersList, member));
			else
				result.Add(new OrCrossJoinCriteria(preMembersList, new InMembers(column, columnValues)));
			if(ShowOthers)
				result.Add(new OrCrossJoinCriteria(preMembersList, new OthersMember(column.Metadata, columnValues)));
		}
	}
}
