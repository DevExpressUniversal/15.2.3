﻿#region Copyright (c) 2000-2015 Developer Express Inc.
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
using System.Collections.Generic;
using DevExpress.DashboardCommon;
using DevExpress.DashboardCommon.ViewerData;
using DevExpress.DashboardCommon.ViewModel;
namespace DevExpress.DashboardExport {
	public class PivotExportRowCreator : PivotExportItemCreator<PivotExportRow> {
		protected override bool MultiplyItemsByMeasures { get { return false; } }
		protected override string FirstAxisName { get { return DashboardDataAxisNames.PivotColumnAxis; } }
		protected override string SecondAxisName { get { return DashboardDataAxisNames.PivotRowAxis; } }
		protected override bool SupportDataItem { get { return ViewModel.Values != null && ViewModel.Values.Count > 1; } }
		protected override PivotExportItemTotalLocation TotalLocation { get { return PivotExportItemTotalLocation.Before; } }
		protected override bool ShowGrandTotals {
			get {
				PivotDashboardItemViewModel viewModel = ViewModel;
				return viewModel.ShowRowGrandTotals || (viewModel.Rows.Count == 0 && viewModel.Columns.Count > 0);
			}
		}
		protected override bool ShowTotals { get { return ViewModel.ShowRowTotals; } }
		public PivotExportRowCreator(MultiDimensionalData mDData, PivotDashboardItemViewModel viewModel)
			: base(mDData, viewModel) {
		}
		protected override PivotExportRow CreateAreaItem(DimensionDescriptor dimension, int logicalPosition) {
			return new PivotExportRow() {
				IsColumnAreaRow = true,
				Dimension = dimension,
				LogicalPosition = logicalPosition,
				FormatSettings = DashboardItemDataFormatter.CreateValueFormatSettings(dimension.InternalDescriptor.Format)
			};
		}
		protected override PivotExportRow CreateDataItem(MeasureDescriptor measure) {
			return new PivotExportRow() {
				IsColumnDataAreaRow = true,
				IsColumnAreaRow = true
			};
		}
		protected override PivotExportRow CreateGrandTotalItem(MeasureDescriptor measure) {
			return new PivotExportRow() { IsGrandTotalRow = true };
		}
		protected override PivotExportRow CreateTotalItem(AxisPoint point, MeasureDescriptor measure) {
			return new PivotExportRow() {
				IsTotalRow = true,
				AxisPoint = point
			};
		}
		protected override PivotExportRow CreateItem(AxisPoint point, MeasureDescriptor measure) {
			return new PivotExportRow() {
				AxisPoint = point
			};
		}
		protected override PivotExportRow CreateTotalDataItem(MeasureDescriptor measure) {
			return new PivotExportRow() {
				IsColumnDataAreaRow = true,
				IsColumnAreaRow = true,
				IsSingleTotalRow = true
			};
		}
		protected override PivotExportRow CreateGrandTotalDataItem() {
			return new PivotExportRow() {
				IsColumnAreaRow = true,
				IsGrandTotalRow = true
			};
		}
	}
}
