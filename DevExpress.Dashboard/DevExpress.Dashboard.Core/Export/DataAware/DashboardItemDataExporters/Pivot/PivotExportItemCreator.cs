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
using DevExpress.DashboardCommon.ViewerData;
using DevExpress.DashboardCommon.ViewModel;
namespace DevExpress.DashboardExport {
	public enum PivotExportItemTotalLocation { Before, After }
	public abstract class PivotExportItemCreator<T> where T : new() {
		readonly MultiDimensionalData mDData;
		readonly PivotDashboardItemViewModel viewModel;
		List<T> items;
		protected PivotDashboardItemViewModel ViewModel { get { return viewModel; } }
		protected abstract bool SupportDataItem { get; }
		protected abstract PivotExportItemTotalLocation TotalLocation { get; }
		protected abstract bool ShowGrandTotals { get; }
		protected abstract bool ShowTotals { get; }
		protected abstract bool MultiplyItemsByMeasures { get; }
		protected abstract string FirstAxisName { get; }
		protected abstract string SecondAxisName { get; }
		public IList<T> Items {
			get {
				if(items == null) {
					items = new List<T>();
					CreateItems();
				}
				return items;
			}
		}
		protected PivotExportItemCreator(MultiDimensionalData mDData, PivotDashboardItemViewModel viewModel) {
			this.mDData = mDData;
			this.viewModel = viewModel;
		}
		void CreateItems() {
			MeasureDescriptorCollection measures = mDData.GetMeasures();
			DataAxis firstArea = mDData.GetAxis(FirstAxisName);
			if(firstArea.Dimensions.Count > 0) {
				int maxAxisPointLevel = 0;
				foreach(AxisPoint point in firstArea.GetPoints())
					if(point.Level.LevelNumber > maxAxisPointLevel)
						maxAxisPointLevel = point.Level.LevelNumber;
				for(int i = 0; i <= maxAxisPointLevel; i++)
					items.Add(CreateAreaItem(firstArea.Dimensions[i], items.Count));
			}
			else {
				if(measures.Count == 1)
					items.Add(CreateTotalDataItem(measures[0]));
				if(measures.Count > 1)
					items.Add(CreateGrandTotalDataItem());
			}
			if(SupportDataItem)
				items.Add(CreateDataItem(null));
			DataAxis secondArea = mDData.GetAxis(SecondAxisName);
			IList<AxisPoint> childs = secondArea.RootPoint.ChildItems;
			foreach(AxisPoint child in childs)
				CreateItems(child);
			if(ShowGrandTotals) {
				if(MultiplyItemsByMeasures) {
					foreach(MeasureDescriptor measure in measures)
						items.Add(CreateGrandTotalItem(measure));
				}
				else
					items.Add(CreateGrandTotalItem(null));
			}
		}
		void CreateItems(AxisPoint point) {
			IList<AxisPoint> childs = point.ChildItems;
			if(childs != null && childs.Count > 0) {
				if(TotalLocation == PivotExportItemTotalLocation.Before)
					CreateTotal(point);
				foreach(AxisPoint child in childs)
					CreateItems(child);
				if(TotalLocation == PivotExportItemTotalLocation.After)
					CreateTotal(point);
			}
			else {
				if(MultiplyItemsByMeasures) {
					MeasureDescriptorCollection measures = mDData.GetMeasures();
					foreach(MeasureDescriptor measure in measures)
						items.Add(CreateItem(point, measure));
				}
				else
					items.Add(CreateItem(point, null));
			}
		}
		void CreateTotal(AxisPoint point) {
			if(ShowTotals) {
				if(MultiplyItemsByMeasures) {
					MeasureDescriptorCollection measures = mDData.GetMeasures();
					foreach(MeasureDescriptor measure in measures)
						items.Add(CreateTotalItem(point, measure));
				}
				else
					items.Add(CreateTotalItem(point, null));
			}
		}
		protected abstract T CreateAreaItem(DimensionDescriptor dimension, int itemLogicalPosition);
		protected abstract T CreateGrandTotalDataItem();
		protected abstract T CreateDataItem(MeasureDescriptor measure);
		protected abstract T CreateTotalDataItem(MeasureDescriptor measure);
		protected abstract T CreateGrandTotalItem(MeasureDescriptor measure);
		protected abstract T CreateTotalItem(AxisPoint point, MeasureDescriptor measure);
		protected abstract T CreateItem(AxisPoint point, MeasureDescriptor measure);
	}
}
