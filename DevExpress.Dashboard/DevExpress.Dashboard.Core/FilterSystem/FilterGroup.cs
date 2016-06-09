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
namespace DevExpress.DashboardCommon.Native {
	public class FilterGroup : IFilterGroup {
		const string LevelIdIgnoreMasterFilter = "IgnoreMasterFilter";
		const string LevelIdMasterFilter = "MasterFilter";
		const string LevelIdDetails = "Details";
		const string LevelIdIndependent = "Independent";
		readonly FilterCombinator outputFilter;
		readonly IFilterLevel independentLevel;
		readonly IEnumerable<FilterLevel> cascadeOrderedLevels;
		IFilter inputFilter;
		IEnumerable<IFilterLevel> FiltrationLevels { get { return cascadeOrderedLevels.Where(l => l.Id != LevelIdDetails); } }
		IFilter OutputFilter { get { return outputFilter; } }
		public IFilter InputFilter {
			get { return inputFilter; }
			set {
				inputFilter = value;
				cascadeOrderedLevels.First().InputFilter = inputFilter;
				independentLevel.InputFilter = inputFilter;
			}
		}
		public FilterGroup() {
			outputFilter = new FilterCombinator();
			IList<FilterLevel> levels = new List<FilterLevel>();
			independentLevel = new FilterLevel(LevelIdIndependent, 0, false);
			levels.Add(new FilterLevel(LevelIdIgnoreMasterFilter, 0, false));
			levels.Add(new FilterLevel(LevelIdMasterFilter, 1, true));
			levels.Add(new FilterLevel(LevelIdDetails, 2, false));
			cascadeOrderedLevels = levels.OrderBy(l => l.Number);
			BeginUpdate();
			try {
				IFilterLevel prevLevel = cascadeOrderedLevels.First();
				foreach (IFilterLevel level in cascadeOrderedLevels.Skip(1)) {
					level.InputFilter = prevLevel.CascadeFilter;
					prevLevel = level;
				}
				outputFilter.UpdateElements(FiltrationLevels);
			} finally {
				EndUpdate();
			}
		}
		#region IFilterGroup implementation
		event EventHandler IFilter.FilterChanged { 
			add { OutputFilter.FilterChanged += value; }
			remove { OutputFilter.FilterChanged -= value; }
		}
		event EventHandler<RequestAffectedEventArgs> IFilter.RequestAffected {
			add { OutputFilter.RequestAffected += value; }
			remove { OutputFilter.RequestAffected -= value; }
		}
		IFilterLevel IFilterGroup.GetFilterLevel(bool ignoreExternalFilter, bool isFilter) {
			Func<string, IFilterLevel> getLevel = (id) => cascadeOrderedLevels.First(l => l.Id == id);
			if (ignoreExternalFilter && isFilter)
				return cascadeOrderedLevels.First(l => l.Id == LevelIdIgnoreMasterFilter);
			else if (!ignoreExternalFilter && isFilter)
				return cascadeOrderedLevels.First(l => l.Id == LevelIdMasterFilter);
			else if (!ignoreExternalFilter && !isFilter)
				return cascadeOrderedLevels.First(l => l.Id == LevelIdDetails);
			else 
				return independentLevel;
		}
		CriteriaOperator IFilter.GetCriteria(DataSourceInfo dataSource, Func<Dimension, string> getDimensionName) {
			return OutputFilter.GetCriteria(dataSource, getDimensionName);
		}
		public void TraceLoops(FilterLoopTracer tracer) {
			tracer.AddTracePoint(this);
			OutputFilter.TraceLoops(tracer);
		}
		public void BeginUpdate() {
			outputFilter.BeginUpdate();
			independentLevel.BeginUpdate();
			foreach (IFilterLevel level in cascadeOrderedLevels)
				level.BeginUpdate();
		}
		public void EndUpdate() {
			foreach (IFilterLevel level in cascadeOrderedLevels)
				level.EndUpdate();
			independentLevel.EndUpdate();
			outputFilter.EndUpdate();
		}
		IEnumerable<IMasterFilterItem> IFilter.GetFilterValuesProviders(DataSourceInfo dataSourceInfo) {
			return OutputFilter.GetFilterValuesProviders(dataSourceInfo); 
		}
		#endregion
	}
}
