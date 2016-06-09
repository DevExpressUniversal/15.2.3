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
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;
using System.Xml.Linq;
using DevExpress.DashboardCommon.Localization;
using DevExpress.DashboardCommon.Native;
using DevExpress.DashboardCommon.ViewModel;
using DevExpress.XtraPivotGrid.Data;
using DevExpress.DashboardCommon.DataProcessing;
namespace DevExpress.DashboardCommon {
	public partial class PivotDashboardItem {
		protected override IEnumerable<Measure> GetQueryVisibleMeasures() {
			return Values;
		}
		protected override SliceDataQuery GetDataQueryInternal(IActualParametersProvider provider) {
			ItemModelBuilder itemBuilder = new ItemModelBuilder(DataSourceModel.DataSourceInfo, GetDataItemUniqueName, provider);
			SliceDataQueryBuilder sliceBuilder = SliceDataQueryBuilder.CreateWithPivotModel(itemBuilder,
				new PivotAxisModel<Dimension> {
					Dimensions = Columns,
					ExpandModel = new AxisExpandModel(columnExpandCollapseActionController.GetEntireActionValues(), AutoExpandColumnGroups ? ExpandAction.Collapse : ExpandAction.Expand)
				},
				new PivotAxisModel<Dimension> {
					Dimensions = Rows,
					ExpandModel = new AxisExpandModel(rowExpandCollapseActionController.GetEntireActionValues(), AutoExpandRowGroups ? ExpandAction.Collapse : ExpandAction.Expand)
				}, QueryMeasures, new DeltaMeasureInfo[0], QueryFilterDimensions, GetQueryFilterCriteria(provider));
			return sliceBuilder.FinalQuery();
		}
	}
}
