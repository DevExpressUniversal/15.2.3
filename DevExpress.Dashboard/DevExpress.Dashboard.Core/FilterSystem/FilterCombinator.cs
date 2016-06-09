#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       Dashboard                                                   }
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
using DevExpress.Data.Filtering;
using DevExpress.DataAccess;
namespace DevExpress.DashboardCommon.Native {
	public class FilterCombinator : IFilter {
		int updateCounter = 0;
		bool needChanged = false;
		bool needTraceLoops = false;
		NotifyingCollection<IFilter> elements;
		EventHandler filterChangedInternal;
		EventHandler<RequestAffectedEventArgs> requestAffectedIternal;
		public FilterCombinator() {
			elements = new NotifyingCollection<IFilter>();
		}
		public FilterCombinator(IEnumerable<IFilter> items)
			: this() {
			UpdateElements(items);
		}
		public void UpdateElements(IEnumerable<IFilter> newElements) {
			bool isRemoved = elements.Count > 0;
			bool isAdded = newElements.Count() > 0;
			UnbindEvents(elements);
			elements.Clear();
			if (isAdded) {
				elements.AddRange(newElements);
				BindEvents(elements);
			}
			if (isRemoved || isAdded) {
				needTraceLoops = true;
				RaiseChanged();
			}
		}
		public void BeginUpdate() {
			BeginUpdate(1);
		}
		public void BeginUpdate(int updateCount) {
			updateCounter += updateCount; 
		}
		public void EndUpdate() {
			updateCounter = Math.Max(0, updateCounter - 1);
			TryRaiseFilterChanged();
		}
		void BindEvents(IList<IFilter> bindList) {
			foreach (IFilter filter in bindList) {
				filter.FilterChanged += ElementFilterChanged;
				filter.RequestAffected += ElementRequestAffected;
			}
		}
		void UnbindEvents(IList<IFilter> unbindList) {
			foreach (IFilter filter in unbindList) {
				filter.FilterChanged -= ElementFilterChanged;
				filter.RequestAffected -= ElementRequestAffected;
			}
		}
		void ElementRequestAffected(object sender, RequestAffectedEventArgs e) {
			if (requestAffectedIternal != null)
				requestAffectedIternal(this, e);
		}
		void ElementFilterChanged(object sender, EventArgs e) {
			RaiseChanged();
		}
		void RaiseChanged() {
			needChanged = true;
			TryRaiseFilterChanged();
		}
		void TryRaiseFilterChanged() {
			if (updateCounter == 0 && needChanged) {
				if (needTraceLoops) {
					TraceLoops(new FilterLoopTracer());
					needTraceLoops = false;
				}
				if (filterChangedInternal != null)
					filterChangedInternal(this, EventArgs.Empty);
				needChanged = false;
			}
		}
		#region IFilter implementation
		CriteriaOperator IFilter.GetCriteria(DataSourceInfo dataSource, Func<Dimension, string> getDimensionName) {
			return CriteriaOperator.And(elements.Select(e => e.GetCriteria(dataSource, getDimensionName)));
		}
		public void TraceLoops(FilterLoopTracer tracer) {
			tracer.AddTracePoint(this);
			foreach(IFilter element in elements)
				element.TraceLoops(tracer);
		}
		event EventHandler IFilter.FilterChanged {
			add { filterChangedInternal += value; }
			remove { filterChangedInternal -= value; }
		}
		event EventHandler<RequestAffectedEventArgs> IFilter.RequestAffected {
			add { requestAffectedIternal += value; }
			remove { requestAffectedIternal -= value; }
		}
		IEnumerable<IMasterFilterItem> IFilter.GetFilterValuesProviders(DataSourceInfo dataSourceInfo) {
			return elements.SelectMany(element => element.GetFilterValuesProviders(dataSourceInfo));
		}
		#endregion
	}
}
