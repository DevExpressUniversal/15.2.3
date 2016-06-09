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
using System.Collections.Specialized;
using System.ComponentModel;
using System.Drawing.Design;
using System.Linq;
using System.Xml.Linq;
using DevExpress.DashboardCommon;
using DevExpress.DashboardCommon.DataProcessing;
using DevExpress.DashboardCommon.Localization;
using DevExpress.DashboardCommon.Native;
using DevExpress.DashboardCommon.ViewModel;
using DevExpress.Data.Filtering;
namespace DevExpress.DashboardCommon {
	public partial class GridDashboardItem {
		protected override IEnumerable<Measure> GetQueryVisibleMeasures() {
			return Columns.OfType<GridMeasureColumn>().Select(c => c.Measure).NotNull();
		} 
		protected override SliceDataQuery GetDataQueryInternal(IActualParametersProvider provider) {
			var dimensionColumns = Columns.OfType<GridDimensionColumn>().Select(c => c.Dimension).NotNull().ToList();
			var dimensionsInteractivity = IsDrillDownEnabled ? SelectionDimensionList.Take(SelectionDimensionList.IndexOf(CurrentDrillDownDimension) + 1).ToArray() : new Dimension[0];
			var dimensionsInteractivityInvisible = IsDrillDownEnabled ? SelectionDimensionList.Except(dimensionsInteractivity).ToArray() : new Dimension[0];
			var dimensionsNonInteractivity = IsDrillDownEnabled ? dimensionColumns.Except(SelectionDimensionList).ToList() : dimensionColumns;
			var mainDimensions = dimensionsInteractivity.Concat(dimensionsNonInteractivity).NotNull();
			var mainMeasures = QueryMeasures;
			var deltaMeasures = Columns.OfType<GridDeltaColumn>().Select(delta => new DeltaMeasureInfo(delta.ActualValue, delta.TargetValue, delta.DeltaOptions)).NotNull();
			var sparklineMeasures = Columns.OfType<GridSparklineColumn>().Select(c => c.Measure).NotNull();
			if(SparklineArgument == null)
				mainMeasures = mainMeasures.Concat(sparklineMeasures);
			IEnumerable<Dimension> filterDimensions = QueryFilterDimensions.Concat(dimensionsInteractivityInvisible);
			Dimension currentSparklineArgument;
			if(sparklineMeasures.Count() > 0) {
				currentSparklineArgument = SparklineArgument;
			} else {
				currentSparklineArgument = null;
				if(SparklineArgument != null)
					filterDimensions = filterDimensions.Concat(new Dimension[] { SparklineArgument });
			}
			SliceDataQueryBuilder queryBuilder;
			ItemModelBuilder itemBuilder = new ItemModelBuilder(DataSourceModel.DataSourceInfo, GetDataItemUniqueName, provider);
			if(IsBackCompatibilityDataSlicesRequired) {
				queryBuilder = SliceDataQueryBuilder.CreateWithPivotModel(itemBuilder, mainDimensions, new[] { currentSparklineArgument }.NotNull(),
					mainMeasures.Union(sparklineMeasures), deltaMeasures, filterDimensions, GetQueryFilterCriteria(provider));
			} else {
				queryBuilder = SliceDataQueryBuilder.CreateEmpty(itemBuilder, filterDimensions, GetQueryFilterCriteria(provider));
				SliceModel mainSlice = queryBuilder.AddSlice(mainDimensions, mainMeasures, deltaMeasures);
				mainSlice.SummaryAggregations.AddRange(columns.SelectMany(column => column.GetGetSummaryAggregationModels(itemBuilder)));
				if(currentSparklineArgument != null)
					queryBuilder.AddSlice(mainDimensions.Append(currentSparklineArgument), sparklineMeasures);
				IList<GridMeasureColumn> automaticMeasures = Columns.OfType<GridMeasureColumn>().Where(c => c.Totals.Any(type => type.TotalType.Equals(GridColumnTotalType.Auto))).ToList();
				if (automaticMeasures.Count > 0)
					queryBuilder.AddSlice(new Dimension[0], automaticMeasures.Select(c => c.Measure));
				queryBuilder.SetAxes(mainDimensions, new[] { currentSparklineArgument }.NotNull());
			}
			return queryBuilder.FinalQuery();
		}
	}
}
