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

using DevExpress.DashboardCommon.ViewerData;
using DevExpress.DashboardCommon.ViewModel;
using System.Collections;
using System.Collections.Generic;
namespace DevExpress.DashboardCommon.Viewer {
	public class FilterElementDashboardItemViewControl {
		IFilterElementControl elementControl;
		FilterElementViewerDataController dataController;
		public FilterElementDashboardItemViewControl(IFilterElementControl elementControl) {
			this.elementControl = elementControl;
		}
		public void Update(FilterElementDashboardItemViewModel viewModel, MultiDimensionalData data) {
			dataController = FilterElementViewerDataController.CreateInstance(viewModel, data);
			UpdateViewModel(viewModel);
			UpdateData();
		}
		public void SetSelection(IList<AxisPointTuple> tuples) {
			if(dataController != null && !dataController.IsEmpty)
				elementControl.SetSelection(dataController.ConvertToIndices(tuples));
		}
		public IEnumerable<AxisPointTuple> GetSelection() {
			return dataController.GetSelectionValues(elementControl.GetSelection());
		}
		public IEnumerable<object> GetExportTexts() {
			return dataController.GetExportTexts(elementControl.GetSelection());
		}
		public IList GetDimensionValues(int indice) {
			return dataController.GetDimensionValues(indice);
		}
		void UpdateViewModel(FilterElementDashboardItemViewModel viewModel) {
			elementControl.Configure(viewModel);
		}
		void UpdateData() {
			elementControl.SetData(dataController.CreateDataSource());
		}
	}
}
