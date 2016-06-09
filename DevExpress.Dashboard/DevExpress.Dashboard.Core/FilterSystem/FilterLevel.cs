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
using DevExpress.DashboardCommon.Printing;
using DevExpress.Data.Filtering;
namespace DevExpress.DashboardCommon.Native {
	public class FilterLevel : IFilterLevel {
		class ElementEntry {
			public IFilter Element { get; set; }
			public int Priority { get; set; }
		}
		readonly string levelId;
		readonly bool filterItself;
		readonly FilterCombinator cascadeCombinator;
		readonly FilterCombinator allElementsCombinator;
		int number;
		int updateCounter = 0;
		IFilter inputFilter;
		IList<ElementEntry> elements = new List<ElementEntry>();
		IDictionary<IFilter, FilterCombinator> elementCombinatorMap = new Dictionary<IFilter, FilterCombinator>();
		IFilter OutputFilter { get { return allElementsCombinator; } }
		IEnumerable<IFilter> OrderedElements { get { return elements.OrderByDescending(e=>e.Priority).Select(e=>e.Element); } }
		public int Number { get { return number; } }
		public FilterLevel(string id, int number, bool filterItself) {
			this.levelId = id;
			this.number = number;
			this.filterItself = filterItself;
			cascadeCombinator = new FilterCombinator();
			allElementsCombinator = new FilterCombinator();
		}
		void UpdateAllCombinators() {
			foreach (IFilter element in OrderedElements)
				UpdateElementCombinator(element);
			UpdateCascadeCombinator();
			UpdateAllElementsCombinator();
		}
		void UpdateCascadeCombinator() {
			List<IFilter> orderedFilters = new List<IFilter>();
			if (inputFilter != null)
				orderedFilters.Add(inputFilter);
			orderedFilters.AddRange(OrderedElements);
			cascadeCombinator.UpdateElements(orderedFilters);
		}
		void UpdateAllElementsCombinator() {
			allElementsCombinator.UpdateElements(OrderedElements);
		}
		void UpdateElementCombinator(IFilter element) {
			List<IFilter> orderedFilters = new List<IFilter>();
			if (inputFilter != null)
				orderedFilters.Add(inputFilter);
			orderedFilters.AddRange(filterItself ? OrderedElements.Where(e => e != element) : Enumerable.Empty<IFilter>());
			elementCombinatorMap[element].UpdateElements(orderedFilters);
		}
		#region IFilterLevel implementation
		public string Id { get { return levelId; } }
		public IFilter InputFilter {
			get { return inputFilter; }
			set {
				if (inputFilter != value) {
					inputFilter = value;
					UpdateAllCombinators();
				}
			}
		}
		public IFilter CascadeFilter { get { return cascadeCombinator; } }
		public IFilter RegisterElement(IFilter element, int priorityCode) {
			elements.Add(new ElementEntry() { Element = element, Priority = priorityCode });
			FilterCombinator filterCombinator = new FilterCombinator();
			if (updateCounter > 0)
				filterCombinator.BeginUpdate(updateCounter);
			elementCombinatorMap.Add(element, filterCombinator);
			UpdateAllCombinators();
			return filterCombinator;
		}
		public void UnregisterElement(IFilter element) {
			elements.Remove(elements.Single(e => e.Element == element));
			elementCombinatorMap[element].UpdateElements(Enumerable.Empty<IFilter>());
			elementCombinatorMap.Remove(element);
			UpdateAllCombinators();
		}
		void IFilterLevel.BeginUpdate() {
			updateCounter++;
			foreach (FilterCombinator combinator in elementCombinatorMap.Values)
				combinator.BeginUpdate();
			cascadeCombinator.BeginUpdate();
			allElementsCombinator.BeginUpdate();
		}
		void IFilterLevel.EndUpdate() {
			updateCounter = Math.Max(0, updateCounter - 1);
			foreach (FilterCombinator combinator in elementCombinatorMap.Values)
				combinator.EndUpdate();
			cascadeCombinator.EndUpdate();
			allElementsCombinator.EndUpdate();
		}
		CriteriaOperator IFilter.GetCriteria(DataSourceInfo dataSource, Func<Dimension, string> getDimensionName) {
			return OutputFilter.GetCriteria(dataSource, getDimensionName);
		}
		public void TraceLoops(FilterLoopTracer tracer) {
			tracer.AddTracePoint(this);
			OutputFilter.TraceLoops(tracer);
		}
		event EventHandler IFilter.FilterChanged {
			add { OutputFilter.FilterChanged += value; }
			remove { OutputFilter.FilterChanged -= value; }
		}
		event EventHandler<RequestAffectedEventArgs> IFilter.RequestAffected {
			add { OutputFilter.RequestAffected += value; }
			remove { OutputFilter.RequestAffected -= value; }
		}
		IEnumerable<IMasterFilterItem> IFilter.GetFilterValuesProviders(DataSourceInfo dataSourceInfo) {
			return OutputFilter.GetFilterValuesProviders(dataSourceInfo);
		}
		#endregion
	}
}
