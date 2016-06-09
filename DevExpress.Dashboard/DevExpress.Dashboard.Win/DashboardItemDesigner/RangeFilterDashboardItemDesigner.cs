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

using DevExpress.DashboardCommon;
using DevExpress.DashboardCommon.Localization;
using DevExpress.DashboardCommon.Native;
using DevExpress.DashboardWin.ServiceModel;
namespace DevExpress.DashboardWin.Native {
	public class RangeFilterDashboardItemDesigner : DataDashboardItemDesigner {
		RangeFilterDashboardItem RangeFilterDashboardItem { get { return (RangeFilterDashboardItem)DashboardItem; } }
		RangeFilterDashboardItemViewer RangeFilterViewer { get { return (RangeFilterDashboardItemViewer)ItemViewer; } }
		RangeFilterControlContainer RangeFilterContainer { get { return RangeFilterViewer.RangeFilterContainer; } }
		RangeFilterRangeControl RangeControl { get { return RangeFilterViewer.RangeControl; } }
		IDashboardDataSource SelectedDataSource { get { return SelectedDataSourceInfo != null ? SelectedDataSourceInfo.DataSource : null; } }
		DataSourceInfo SelectedDataSourceInfo {
			get {
				IDataSourceSelectionService service = ServiceProvider.RequestServiceStrictly<IDataSourceSelectionService>();
				return service.SelectedDataSourceInfo;
			}
		}
		public void UpdateErrorMessage() {
			string errorMessage = RangeFilterDashboardItem.ErrorMessage;
			IDashboardDataSource rangeFilterDataSource = RangeFilterDashboardItem.DataSource;
			bool dataSourceAssigned = rangeFilterDataSource != null && rangeFilterDataSource.GetIsRangeFilterSupported();
			if (!dataSourceAssigned && SelectedDataSource != null && !SelectedDataSource.GetIsRangeFilterSupported())
				errorMessage = DashboardLocalizer.GetString(DashboardStringId.RangeFilterOLAPDataSource);
			if (errorMessage != RangeFilterContainer.ErrorText) {
				RangeFilterViewer.SuspendLayout();
				try {
					RangeControl.Visible = string.IsNullOrEmpty(errorMessage);
					RangeFilterContainer.ErrorText = errorMessage;
				}
				finally {
					RangeFilterViewer.ResumeLayout(true);
				}
			}
		}
		protected override void InitializeInternal() {
			base.InitializeInternal();
			IDataSourceSelectionService service = ServiceProvider.RequestServiceStrictly<IDataSourceSelectionService>();
			service.DataSourceSelected += OnDataSourceSelected;
		}
		protected override void Dispose(bool disposing) {
			if (disposing) {
				if (ServiceProvider != null) {
					IDataSourceSelectionService service = ServiceProvider.RequestServiceStrictly<IDataSourceSelectionService>();
					if (service != null)
						service.DataSourceSelected -= OnDataSourceSelected;
				}				
			}
			base.Dispose(disposing);
		}
		void OnDataSourceSelected(object sender, DataSourceSelectedEventArgs e) {
			UpdateErrorMessage();
		}
	}
}
