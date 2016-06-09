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

using System.Collections.Generic;
using DevExpress.DashboardCommon.ViewerData;
using DevExpress.DashboardCommon.ViewModel;
using DevExpress.DataAccess.Native.Data;
namespace DevExpress.DashboardCommon.Viewer {
#if DEBUG
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1039")]
#endif
	public class GeoPointMapMultiDimensionalDataSourceBase : ReadOnlyTypedList {
		readonly GeoPointMapDashboardItemViewModelBase viewModel;
		readonly MultiDimensionalData data;
		readonly IList<AxisPoint> rows;
		protected GeoPointMapDashboardItemViewModelBase ViewModel { get { return viewModel; } }
		protected MultiDimensionalData Data { get { return data; } }
		protected virtual string AxisPointDimensionDescriptorId { get { return viewModel.LongitudeDataId; } }
		protected bool EmptyData { get { return data.IsEmpty || viewModel.LatitudeDataId == null || viewModel.LongitudeDataId == null; } }
		public GeoPointMapMultiDimensionalDataSourceBase(GeoPointMapDashboardItemViewModelBase viewModel, MultiDimensionalData data) {
			this.viewModel = viewModel;
			this.data = data;
			if(EmptyData)
				rows = new AxisPoint[0];
			else {
				rows = data.GetAxisPointsByDimensionId(AxisPointDimensionDescriptorId);
				Properties.Add(new CoordinateValueMDDataPropertyDescriptor(viewModel.LatitudeDataId));
				Properties.Add(new CoordinateValueMDDataPropertyDescriptor(viewModel.LongitudeDataId));
				Properties.Add(new CoordinateUniqueValueMDDataPropertyDescriptor(GeoPointMapViewerDataController.LatitudeSelection, viewModel.LatitudeDataId));
				Properties.Add(new CoordinateUniqueValueMDDataPropertyDescriptor(GeoPointMapViewerDataController.LongitudeSelection, viewModel.LongitudeDataId));
				if(viewModel.TooltipDimensions != null) {
					foreach(TooltipDataItemViewModel itemViewModel in viewModel.TooltipDimensions)
						Properties.Add(new TooltipDimensionMDDataPropertyDescriptor(data, itemViewModel.DataId));
				}
				if(viewModel.TooltipMeasures != null) {
					foreach(TooltipDataItemViewModel itemViewModel in viewModel.TooltipMeasures)
						Properties.Add(new GeoPointDisplayTextMDDataPropertyDescriptor(data, itemViewModel.DataId, itemViewModel.DataId));
				}
				if(viewModel.EnableClustering)
					Properties.Add(new GeoPointClusterCountMDdataPropertyDescriptor(data, viewModel.PointsCountDataId, viewModel.LongitudeDataId));
			}
		}
		protected ValueFormatViewModel GetFormat(string measureId) {
			MeasureDescriptor descriptor = Data.GetMeasureDescriptorByID(measureId);
			return descriptor != null ? descriptor.InternalDescriptor.Format : null;
		}
		protected override object GetItemValue(int index) {
			return rows[index];
		}
		protected override int GetItemsCount() {
			return rows.Count;
		}
	}
#if DEBUG
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1039")]
#endif
	public class GeoPointMapMultiDimensionalDataSource : GeoPointMapMultiDimensionalDataSourceBase {
		public GeoPointMapMultiDimensionalDataSource(GeoPointMapDashboardItemViewModel viewModel, MultiDimensionalData data) :
			base(viewModel, data) {
			if(!EmptyData) {
				Properties.Add(new GeoPointValueMDDataPropertyDescriptor(data, viewModel.ValueId));
				Properties.Add(new GeoPointDisplayTextMDDataPropertyDescriptor(data, viewModel.ValueId, viewModel.ValueId + GeoPointMapViewerDataController.DisplayTextPostFix));
			}
		}
	}
#if DEBUG
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1039")]
#endif
	public class BubbleMapMultiDimensionalDataSource : GeoPointMapMultiDimensionalDataSourceBase {
		BubbleMapDashboardItemViewModel BubbleViewModel { get { return (BubbleMapDashboardItemViewModel)ViewModel; } }
		public BubbleMapMultiDimensionalDataSource(BubbleMapDashboardItemViewModel viewModel, MultiDimensionalData data) :
			base(viewModel, data) {
			if(!EmptyData) {
				if(viewModel.WeightId != null) {
					Properties.Add(new GeoPointValueMDDataPropertyDescriptor(data, viewModel.WeightId));
					Properties.Add(new GeoPointDisplayTextMDDataPropertyDescriptor(data, viewModel.WeightId, viewModel.WeightId + GeoPointMapViewerDataController.DisplayTextPostFix));
				}
				if(viewModel.ColorId != null) {
					Properties.Add(new GeoPointValueMDDataPropertyDescriptor(data, viewModel.ColorId));
					Properties.Add(new GeoPointDisplayTextMDDataPropertyDescriptor(data, viewModel.ColorId, viewModel.ColorId + GeoPointMapViewerDataController.DisplayTextPostFix));
				}
			}
		}
		public ValueFormatViewModel GetColorFormat() {
			return BubbleViewModel.ColorId != null ? GetFormat(BubbleViewModel.ColorId) : null;
		}
		public ValueFormatViewModel GetWeightFormat() {
			return BubbleViewModel.WeightId != null ? GetFormat(BubbleViewModel.WeightId) : null;
		}
	}
#if DEBUG
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1039")]
#endif
	public class PieMapMultiDimensionalDataSource : GeoPointMapMultiDimensionalDataSourceBase {
		public const string ArgumentValuePostFix = "_ArgumentValue";
		public const string ArgumentDisplayTextPostFix = "_ArgumentDisplayText";
		PieMapDashboardItemViewModel PieViewModel { get { return (PieMapDashboardItemViewModel)ViewModel; } }
		protected override string AxisPointDimensionDescriptorId {
			get { return PieViewModel.ArgumentDataId != null ? PieViewModel.ArgumentDataId : base.AxisPointDimensionDescriptorId; }
		}
		public PieMapMultiDimensionalDataSource(PieMapDashboardItemViewModel viewModel, MultiDimensionalData data) :
			base(viewModel, data) {
			if(!EmptyData) {
				if(viewModel.ArgumentDataId != null) {
					Properties.Add(new GeoPointPieArgumentUniqueValueMDdataPropertyDescriptor(viewModel.ArgumentDataId));
					Properties.Add(new GeoPointPieArgumentDisplayTextMDdataPropertyDescriptor(viewModel.ArgumentDataId + ArgumentDisplayTextPostFix));
				}
				foreach(string valueId in viewModel.Values) {
					if(viewModel.ArgumentDataId == null) {
						Properties.Add(new GeoPointPieArgumentNameMDdataPropertyDescriptor(data, valueId, valueId + ArgumentValuePostFix));
						Properties.Add(new GeoPointPieArgumentNameMDdataPropertyDescriptor(data, valueId, valueId + ArgumentDisplayTextPostFix));
					}
					Properties.Add(new GeoPointValueMDDataPropertyDescriptor(data, valueId));
					Properties.Add(new GeoPointDisplayTextMDDataPropertyDescriptor(data, valueId, valueId + GeoPointMapViewerDataController.DisplayTextPostFix));
				}
			}
		}
		public ValueFormatViewModel GetValueFormat() {
			return PieViewModel.Values.Count > 0 ? GetFormat(PieViewModel.Values[0]) : null;
		}
	}
}
