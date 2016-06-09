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
using DevExpress.DashboardCommon;
using DevExpress.DashboardCommon.Native;
using DevExpress.DashboardWin.Localization;
using DevExpress.DashboardCommon.DB;
using DevExpress.DataAccess.UI.Native;
using DevExpress.DataAccess;
using DevExpress.DashboardWin.ServiceModel;
using DevExpress.DataAccess.Native;
using DevExpress.DataAccess.Native.Data;
namespace DevExpress.DashboardWin.Native {
	public class NewDataSourceHistoryItem : IHistoryItem {
		readonly IDashboardDataSource dataSource;
		DataSourceInfo previousSelectedDataSource;
		DashboardParametersHistoryItem dashboardParametersHistoryItem;
		public string Caption { get { return String.Format(DashboardWinLocalizer.GetString(DashboardWinStringId.HistoryItemNewDataSource), dataSource.Name); } }
		public NewDataSourceHistoryItem(IDashboardDataSource dataSource, DashboardParameterCollection parameters, ParameterChangesCollection parametersChanged)
			: base() {
			this.dataSource = dataSource;
			dashboardParametersHistoryItem = new DashboardParametersHistoryItem(parameters, parametersChanged);			
		} 
		public void Undo(DashboardDesigner designer) {		 
			designer.Dashboard.DataSources.Remove(dataSource);
			if (previousSelectedDataSource != null) {
				IDataSourceSelectionService service = designer.RequestServiceStrictly<IDataSourceSelectionService>();
				service.SelectedDataSourceInfo = previousSelectedDataSource;
			}
			designer.DragAreaScrollableControl.DragArea.UpdateDataSource();
			dashboardParametersHistoryItem.Undo(designer);
		}
		public void Redo(DashboardDesigner designer) {
			IDataComponent  dataComponent = dataSource as IDataComponent;
			if(dataComponent == null)
				dashboardParametersHistoryItem.Redo(designer);				  
			IDataSourceSelectionService service = designer.RequestServiceStrictly<IDataSourceSelectionService>();
			previousSelectedDataSource = service.SelectedDataSourceInfo;
			designer.Dashboard.BeginUpdate();
			designer.Dashboard.DataSources.Add(dataSource);
			designer.Dashboard.FillDataSource(dataSource);
			designer.Dashboard.EndUpdate();
			designer.DragAreaScrollableControl.DragArea.UpdateDataSource();
		}
	}
}
