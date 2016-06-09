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
using DevExpress.DashboardCommon;
using DevExpress.Data.Filtering;
using DevExpress.XtraEditors.Filtering;
using DevExpress.DashboardCommon.Native;
namespace DevExpress.DashboardWin.Native {
	public class FormatConditionExpressionFilterContext : IFilteredComponent {
		readonly DataDashboardItem dashboardItem;
		readonly IActualParametersProvider provider;
		public FormatConditionExpressionFilterContext(DataDashboardItem dashboardItem, IActualParametersProvider provider)
			: base() {
			this.dashboardItem = dashboardItem;
			this.provider = provider;
		}
		IBoundPropertyCollection GetFilterPropertyCollection() {
			return new DataItemFilterColumnCollection(dashboardItem, provider);
		}
		#region IFilteredComponentBase
		CriteriaOperator IFilteredComponentBase.RowCriteria { get; set; }
		IBoundPropertyCollection IFilteredComponent.CreateFilterColumnCollection() {
			return GetFilterPropertyCollection();
		}
		event EventHandler IFilteredComponentBase.RowFilterChanged { add { } remove { } }
		event EventHandler IFilteredComponentBase.PropertiesChanged { add { } remove { } }
		#endregion
	}
	public class DataItemFilterColumnCollection : FilterColumnCollection {
		public DataItemFilterColumnCollection(DataDashboardItem dashboardItem, IActualParametersProvider provider) :
			base() {
			foreach(Measure measure in dashboardItem.UniqueMeasures)
				Add(new MeasureFilterColumn(dashboardItem, measure, provider));
			List<Dimension> uniqueDimensions = new List<Dimension>();
			foreach(Dimension dimension in dashboardItem.Dimensions) {
				if(!dashboardItem.IsDimensionHidden(dimension) && uniqueDimensions.Find(uniqueDataItem => uniqueDataItem.EqualsByDefinition(dimension)) == null) {
					Add(new DimensionFilterColumn(dashboardItem, dimension, provider));
					uniqueDimensions.Add(dimension);
				}
			}
			uniqueDimensions.Clear();
		}
	}
}
