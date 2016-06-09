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

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DevExpress.DashboardCommon.Localization;
using DevExpress.DashboardCommon.Native;
using DevExpress.DashboardCommon.ViewerData;
using DevExpress.DashboardCommon.ViewModel;
namespace DevExpress.DashboardCommon.Viewer {
	public class FilterElementViewerDataController {
		public static FilterElementViewerDataController CreateInstance(FilterElementDashboardItemViewModel viewModel, MultiDimensionalData data) {
			ComboBoxDashboardItemViewModel comboBoxViewModel = viewModel as ComboBoxDashboardItemViewModel;
			if(comboBoxViewModel != null)
				if(comboBoxViewModel.ComboBoxType == ComboBoxDashboardItemType.Standard)
					return new FilterElementSingleSelectViewerDataController(viewModel, data);
				else
					return new FilterElementViewerDataController(viewModel, data);
			ListBoxDashboardItemViewModel listBoxViewModel = viewModel as ListBoxDashboardItemViewModel;
			if(listBoxViewModel != null) {
				if(listBoxViewModel.ListBoxType == ListBoxDashboardItemType.Radio)
					return new FilterElementSingleSelectViewerDataController(viewModel, data);
				else
					return new FilterElementViewerDataController(viewModel, data);
			}
			TreeViewDashboardItemViewModel treeViewViewModel = viewModel as TreeViewDashboardItemViewModel;
			if(treeViewViewModel != null)
				return new TreeViewViewerDataController(viewModel, data);
			return new FilterElementViewerDataController(viewModel, data);
		}
		readonly MultiDimensionalData data;
		readonly FilterElementDashboardItemViewModel viewModel;
		public bool IsEmpty { get { return data == null || !data.Axes.ContainsKey(DashboardDataAxisNames.DefaultAxis); } }
		protected MultiDimensionalData Data { get { return data; } }
		protected FilterElementDashboardItemViewModel ViewModel { get { return viewModel; } }
		protected FilterElementViewerDataController(FilterElementDashboardItemViewModel viewModel, MultiDimensionalData data) {
			this.data = data;
			this.viewModel = viewModel;
		}
		public object CreateDataSource() {
			return !IsEmpty ? CreateDataSourceInternal() : null;
		}
		public IEnumerable<AxisPointTuple> GetSelectionValues(IEnumerable<int> indices) {
			List<AxisPointTuple> tupleList = new List<AxisPointTuple>();
			foreach(AxisPoint axisPoint in GetSelectionAxisPoints(indices))
				tupleList.Add(Data.CreateTuple(axisPoint));
			return tupleList;
		}
		public IEnumerable<object> GetExportTexts(IEnumerable<int> indices) {
			if(indices != null) {
				indices = CheckFullSelection(indices);
				if(indices.Count() == 1 && indices.First() == -1)
					return new List<string[]> { new string[] { DashboardLocalizer.GetString(DashboardStringId.FilterElementShowAllItem) } };
				IEnumerable<AxisPoint> axisPoints = GetSelectionAxisPoints(indices);
				return axisPoints.Select(point => point.RootPath.Select(pt => pt.DisplayText));
			}
			return new IEnumerable<object>[0];
		}
		public IList GetDimensionValues(int indice) {
			AxisPoint axisPoint = data.GetAxisRoot(DashboardDataAxisNames.DefaultAxis).FindPointByIndex(indice);
			if(axisPoint != null)
				return axisPoint.RootPath.Select(pt => pt.UniqueValue).ToList();
			return new object[0];
		}
		protected virtual IEnumerable<AxisPoint> GetSelectionAxisPoints(IEnumerable<int> indices) {
			return indices.Count() > 0 ?
				data.GetAxisRoot(DashboardDataAxisNames.DefaultAxis).FindPointsByIndices(new HashSet<int>(indices)) :
				new AxisPoint[0];
		}
		public IEnumerable<int> ConvertToIndices(IEnumerable<AxisPointTuple> tuples) {
			return CheckIndices(tuples.Select(tuple => tuple.GetAxisPoint(DashboardDataAxisNames.DefaultAxis).Index));
		}
		protected IEnumerable<int> CheckFullSelection(IEnumerable<int> indices) {
			AxisPoint rootPoint = Data.GetAxisRoot(DashboardDataAxisNames.DefaultAxis);
			IEnumerable<int> lastLevelIndices = rootPoint.GetAxisPoints().Select(point => point.Index);
			if(viewModel.ShowAllValue && lastLevelIndices.SequenceEqual(indices))
				return  new List<int> { rootPoint.Index };
			 return indices;
		}
		protected virtual IEnumerable<int> CheckIndices(IEnumerable<int> indices) {
			return indices;
		}
		protected virtual object CreateDataSourceInternal() {
			bool generateAllItem = false;
			ComboBoxDashboardItemViewModel comboBoxModel = viewModel as ComboBoxDashboardItemViewModel;
			if(comboBoxModel != null && comboBoxModel.ComboBoxType == ComboBoxDashboardItemType.Checked)
				generateAllItem = false;
			else 
				generateAllItem = viewModel.ShowAllValue;
			return new FilterElementMultiDimensionalDataSource(Data, generateAllItem);
		}
	}
	public class FilterElementSingleSelectViewerDataController : FilterElementViewerDataController {
		public FilterElementSingleSelectViewerDataController(FilterElementDashboardItemViewModel viewModel, MultiDimensionalData data)
			: base(viewModel, data) {
		}
		protected override IEnumerable<int> CheckIndices(IEnumerable<int> indices) {
			return CheckFullSelection(indices);
		}
		protected override IEnumerable<AxisPoint> GetSelectionAxisPoints(IEnumerable<int> indices) {
			if(indices != null && indices.Count() == 1 && indices.First() == -1)
				return Data.GetAxisPoints(DashboardDataAxisNames.DefaultAxis);
			return base.GetSelectionAxisPoints(indices);
		}
	}
	public class TreeViewViewerDataController : FilterElementViewerDataController {
		public TreeViewViewerDataController(FilterElementDashboardItemViewModel viewModel, MultiDimensionalData data)
			: base(viewModel, data) {
		}
		protected override object CreateDataSourceInternal() {
			return new TreeViewMultiDimensionalDataSource(Data);
		}
		protected override IEnumerable<AxisPoint> GetSelectionAxisPoints(IEnumerable<int> indices) {
			AxisPoint axisRoot = Data.GetAxisRoot(DashboardDataAxisNames.DefaultAxis);
			foreach(AxisPoint axisPoint in axisRoot.FindPointsByIndices(indices)) {
				foreach(AxisPoint lastLevelPoint in axisPoint.GetAxisPoints())
					yield return lastLevelPoint;
			}
		}
	}
}
