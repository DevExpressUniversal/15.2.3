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
	public class FilterIMasterFilter : IFilter {
		readonly IMasterFilter masterFilter;
		public FilterIMasterFilter(IMasterFilter masterFilter) {
			this.masterFilter = masterFilter;
		}
		public IEnumerable<IMasterFilterItem> GetAffected() {
			IList<IMasterFilterItem> items = new List<IMasterFilterItem>();
			if (RequestAffected != null)
				RequestAffected(this, new RequestAffectedEventArgs(items, masterFilter.IsFilterDataSource));
			return items;
		}
		public void Changed() {
			if (FilterChanged != null)
				FilterChanged(this, EventArgs.Empty);
		}
		#region IFilter implementation
		public event EventHandler FilterChanged;
		public event EventHandler<RequestAffectedEventArgs> RequestAffected;
		CriteriaOperator IFilter.GetCriteria(DataSourceInfo dataSource, Func<Dimension, string> getDimensionName) {
			if(!masterFilter.IsFilterDataSource(dataSource))
				return null;
			MasterFilterCriteriaGenerator generator = new MasterFilterCriteriaGenerator(dataSource != null ? dataSource.GetPickManager(): null, getDimensionName);
			return CriteriaOperator.And(masterFilter.Parameters.Select(p => generator.GetParametersCriteria(p)));
		}
		public void TraceLoops(FilterLoopTracer tracer) {
			tracer.AddTracePoint(this);
		}
		IEnumerable<IMasterFilterItem> IFilter.GetFilterValuesProviders(DataSourceInfo dataSourceInfo) {
			return dataSourceInfo == null || masterFilter.IsFilterDataSource(dataSourceInfo) ? new[] { masterFilter } : new IMasterFilterItem[0];
		}
		#endregion
	}
}
