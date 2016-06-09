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
using DevExpress.Data.Filtering;
namespace DevExpress.DashboardCommon.Native.DashboardRestfulService {
	class ExternalFilter : IFilter {
		readonly IMasterFilterParameters parameters;
		public ExternalFilter(IMasterFilterParameters parameters) {
			this.parameters = parameters;
		}
		public IEnumerable<IMasterFilterItem> GetAffected() {
			IList<IMasterFilterItem> items = new List<IMasterFilterItem>();
			if(RequestAffected != null)
				RequestAffected(this, new RequestAffectedEventArgs(items, (ds) => true));
			return items;
		}
		public void Changed() {
			if(FilterChanged != null)
				FilterChanged(this, EventArgs.Empty);
		}
		#region IFilter implementation
		public event EventHandler FilterChanged;
		public event EventHandler<RequestAffectedEventArgs> RequestAffected;
		CriteriaOperator IFilter.GetCriteria(DataSourceInfo dataSource, Func<Dimension, string> getDimensionName) {
			MasterFilterCriteriaGenerator generator = new MasterFilterCriteriaGenerator(dataSource != null ? dataSource.GetPickManager() : null, getDimensionName);
			return generator.GetParametersCriteria(parameters);
		}
		public void TraceLoops(FilterLoopTracer tracer) {
			tracer.AddTracePoint(this);
		}
		IEnumerable<IMasterFilterItem> IFilter.GetFilterValuesProviders(DataSourceInfo dataSourceInfo) {
			return new IMasterFilterItem[] { };
		}
		#endregion
	}
	class UnboundFilterValues : IMasterFilterParameters {
		readonly DimensionValueSet dimensionValueSet;
		readonly Dictionary<Dimension, MasterFilterRange> ranges;
		readonly bool isExcludingAllFilter;
		public UnboundFilterValues(OrderedDictionary<Dimension, IList<object>> dimensionValues, bool isExcludingAllFilter) {
			this.dimensionValueSet = new DimensionValueSet(dimensionValues);
			this.ranges = new Dictionary<Dimension, MasterFilterRange>();
			this.isExcludingAllFilter = isExcludingAllFilter;
		}
		#region IMasterFilterParameters Members
		public DimensionValueSet Values { get { return dimensionValueSet; } }
		public Dictionary<Dimension, MasterFilterRange> Ranges { get { return this.ranges; } }
		public bool EmptyCriteria { get { return !isExcludingAllFilter && this.dimensionValueSet.Count == 0 && this.ranges.Count == 0; } }		
		public bool IsExcludingAllFilter { get { return isExcludingAllFilter; } }
		#endregion
	}
}
