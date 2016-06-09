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
using DevExpress.PivotGrid.DataCalculation;
namespace DevExpress.PivotGrid.QueryMode {
	public abstract class QueryExecutorBase<TColumn, TOwner> : IQueryExecutor<TColumn> where TColumn : QueryColumn where TOwner : class, IDataSourceHelpersOwner<TColumn> {
		protected class OwnerSetter : IDisposable {
			QueryExecutorBase<TColumn, TOwner> owner;
			TOwner oldOwner;
			public OwnerSetter(QueryExecutorBase<TColumn, TOwner> owner, IDataSourceHelpersOwner<TColumn> currentOwner) {
				this.owner = owner;
				SetOwner(currentOwner);
			}
			void SetOwner(IDataSourceHelpersOwner<TColumn> currentOwner) {
				oldOwner = owner.currentOwner;
				owner.currentOwner = (TOwner)currentOwner;
			}
			void ResetOwner() {
				owner.currentOwner = oldOwner;
			}
			void IDisposable.Dispose() {
				ResetOwner();
			}
		}
		TOwner currentOwner;
		QueryMetadata<TColumn> metadata;
		protected QueryMetadata<TColumn> Metadata { get { return metadata; } }
		protected internal TOwner CurrentOwner {
			get { return currentOwner; }
#if DEBUGTEST
			set { currentOwner = value; }
#endif
		}
		protected QueryExecutorBase(QueryMetadata<TColumn> metadata) {
			this.metadata = metadata;
		}
		protected bool TryHandleException(QueryHandleableException raisedException, IDataSourceHelpersOwner<TColumn> desiredOwner, bool force) {
			if(!force) {
				if(raisedException.IsNullReference)
					return true;
				if(!raisedException.IsResponse)
					return false;
			}
			bool handled = CurrentOwner != null && CurrentOwner.IsDesignMode || metadata.HandleException(desiredOwner, raisedException);
			if(handled)
				return true;
			throw raisedException;
		}
		protected bool TryHandleException(QueryHandleableException raisedException, IDataSourceHelpersOwner<TColumn> desiredOwner) {
			return TryHandleException(raisedException, desiredOwner, false);
		}
		protected bool TryHandleException(QueryHandleableException raisedException) {
			return TryHandleException(raisedException, currentOwner, false);
		}
		protected abstract CellSet<TColumn> QueryData(IQueryContext<TColumn> context);
		protected abstract List<object> QueryVisibleValues(TColumn column);
		protected abstract object[] QueryAvailableValues(TColumn column, bool deferUpdates, List<TColumn> customFilters);
		protected abstract bool QueryNullValues(IQueryMetadataColumn column);
		protected abstract IDataTable QueryDrillDown(QueryMember[] columnMembers, QueryMember[] rowMembers, TColumn measure, int maxRowCount, List<string> customColumns);
		protected abstract void QueryAggregations(IList<AggregationLevel> aggregationLevels);
		CellSet<TColumn> IQueryExecutor<TColumn>.QueryData(IDataSourceHelpersOwner<TColumn> owner, IQueryContext<TColumn> context) {
			using(OwnerSetter ownerSetter = new OwnerSetter(this, owner))
				return QueryData(context);
		}
		List<object> IQueryExecutor<TColumn>.QueryVisibleValues(IDataSourceHelpersOwner<TColumn> owner, TColumn column) {
			using(OwnerSetter ownerSetter = new OwnerSetter(this, owner))
				return QueryVisibleValues(column);
		}
		object[] IQueryExecutor<TColumn>.QueryAvailableValues(IDataSourceHelpersOwner<TColumn> owner, TColumn column, bool deferUpdates, List<TColumn> customFilters) {
			using(OwnerSetter ownerSetter = new OwnerSetter(this, owner))
				return QueryAvailableValues(column, deferUpdates, customFilters);
		}
		bool IQueryExecutor<TColumn>.QueryNullValues(IDataSourceHelpersOwner<TColumn> owner, IQueryMetadataColumn column) {
			using(OwnerSetter ownerSetter = new OwnerSetter(this, owner))
				return QueryNullValues(column);
		}
		IDataTable IQueryExecutor<TColumn>.QueryDrillDown(IDataSourceHelpersOwner<TColumn> owner, QueryMember[] columnMembers, QueryMember[] rowMembers, TColumn measure, int maxRowCount, List<string> customColumns) {
			using(OwnerSetter ownerSetter = new OwnerSetter(this, owner))
				return QueryDrillDown(columnMembers, rowMembers, measure, maxRowCount, customColumns);
		}
		void IQueryExecutor<TColumn>.QueryAggregations(IDataSourceHelpersOwner<TColumn> owner, IList<AggregationLevel> aggregationLevels) {
			using(OwnerSetter ownerSetter = new OwnerSetter(this, owner))
				QueryAggregations(aggregationLevels);
		}
	}
}
