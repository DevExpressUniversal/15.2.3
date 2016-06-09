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

using DevExpress.DashboardCommon.DataProcessing;
using DevExpress.DashboardCommon.Native;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
namespace DevExpress.DashboardCommon {
	public partial class ChoroplethMapDashboardItem {
		protected override SliceDataQuery GetDataQueryInternal(IActualParametersProvider provider) {
			if(!IsMapReady)
				return new SliceDataQuery();
			IEnumerable<Dimension> dimensions = new Dimension[] { AttributeDimension };
			IList<DeltaMeasureInfo> deltaInfo = new List<DeltaMeasureInfo>();
			if(ActiveMap!=null){
				DataItemContainerActualContent content = ActiveMap.GetActualContent();
				if(content.IsDelta)
					deltaInfo.Add(new DeltaMeasureInfo(content.DeltaActualValue, content.DeltaTargetValue, content.DeltaOptions));
			}
			SliceDataQueryBuilder queryBuilder = null;
			ItemModelBuilder itemBuilder = new ItemModelBuilder(DataSourceModel.DataSourceInfo, GetDataItemUniqueName, provider);
			if(IsBackCompatibilityDataSlicesRequired) {
				queryBuilder = SliceDataQueryBuilder.CreateWithPivotModel(itemBuilder, dimensions, new Dimension[0],
					QueryMeasures, deltaInfo, QueryFilterDimensions, GetQueryFilterCriteria(provider));
			} else {
				queryBuilder = SliceDataQueryBuilder.CreateEmpty(itemBuilder, QueryFilterDimensions, GetQueryFilterCriteria(provider));
				queryBuilder.AddSlice(dimensions, QueryMeasures, deltaInfo);
				queryBuilder.SetAxes(dimensions, new Dimension[0]);
			}
			return queryBuilder.FinalQuery();
		}
		protected override IEnumerable<Measure> GetQueryVisibleMeasures() {
			return ActiveMap != null && !ActiveMap.GetActualContent().IsDelta ? base.GetQueryVisibleMeasures().Union(ActiveMap.Measures).NotNull() : base.GetQueryVisibleMeasures();
		}
	}
}
