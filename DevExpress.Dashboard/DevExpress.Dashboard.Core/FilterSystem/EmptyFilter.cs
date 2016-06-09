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
	public class EmptyFilter : IFilter, IMasterFilterItem {
		EventHandler<RequestAffectedEventArgs> RequestAffectedInternal;
		public IEnumerable<IMasterFilterItem> GetAffected() {
			IList<IMasterFilterItem> items = new List<IMasterFilterItem>();
			if (RequestAffectedInternal != null)
				RequestAffectedInternal(this, new RequestAffectedEventArgs(items, (ds) => true));
			return items;
		}
		IEnumerable<IMasterFilterItem> IFilter.GetFilterValuesProviders(DataSourceInfo dataSourceInfo) {
			return new IMasterFilterItem[0];
		}
		string IMasterFilterItem.Name { get { return String.Empty; } }
		CriteriaOperator IFilter.GetCriteria(DataSourceInfo dataSource, Func<Dimension, string> getDimensionName) {
			return null;
		}
		void IFilter.TraceLoops(FilterLoopTracer tracer) { }
		event EventHandler IFilter.FilterChanged { add { } remove { } }
		event EventHandler<RequestAffectedEventArgs> IFilter.RequestAffected {
			add { RequestAffectedInternal += value; }
			remove { RequestAffectedInternal -= value; }
		}
	}
}
