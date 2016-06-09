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
using System.ComponentModel;
using System.ComponentModel.Design;
using DevExpress.DashboardCommon;
using DevExpress.DashboardCommon.Native;
using DevExpress.DashboardWin.Localization;
using DevExpress.Data.Filtering;
namespace DevExpress.DashboardWin.Native {
	public class FilterDashboardItemHistoryItem : IHistoryItem {
		readonly string previousFilterString;
		readonly string nextFilterString;
		readonly DataDashboardItem dashboardItem;
		DashboardParameterCollection parameters;
		ParameterChangesCollection parameterChanges;
		public string Caption {
			get { return String.Format(DashboardWinLocalizer.GetString(DashboardWinStringId.HistoryItemEditFilter), dashboardItem.Name); }
		}
		public FilterDashboardItemHistoryItem(DataDashboardItem dashboardItem, string nextFilterString, DashboardParameterCollection parameters, ParameterChangesCollection parametersChanged) {
			this.nextFilterString = nextFilterString;
			previousFilterString = dashboardItem.FilterString;
			this.dashboardItem = dashboardItem;
			this.parameters = parameters;
			this.parameterChanges = parametersChanged;
		}
		public FilterDashboardItemHistoryItem(DataDashboardItem dashboardItem, string nextFilterString)
			: this(dashboardItem, nextFilterString,  null, null) {			
		}
		public void Redo(DashboardDesigner designer) {
			Dashboard dashboard = designer.Dashboard;
			IDesignerHost designerHost = dashboard.GetService<IDesignerHost>();
			DesignerTransaction transaction = designerHost != null ? designerHost.CreateTransaction(Caption) : null;
			try {
				IComponentChangeService changeService = dashboard.GetService<IComponentChangeService>();
				PropertyDescriptor property = null;
				if(changeService != null) {
					property = Helper.GetProperty(dashboardItem, "FilterString");
					changeService.OnComponentChanging(dashboardItem, property);
				}
				PerformOperation(designer,true);
				if(changeService != null)
					changeService.OnComponentChanged(dashboardItem, property, previousFilterString, nextFilterString);
			}
			finally {
				if(transaction != null)
					transaction.Commit();
			}
		}
		public void Undo(DashboardDesigner designer) {
			PerformOperation(designer, false);
		}
		protected void PerformOperation(DashboardDesigner designer, bool redo) {						
			if(parameterChanges != null) {
				parameters.BeginUpdate();
				parameters.Clear();
				if(redo)
					parameters.AddRange(parameterChanges.Where(p => p.NewParameter != null).Select(p => p.NewParameter));
				else
					parameters.AddRange(parameterChanges.Where(p => p.OldParameter != null).Select(p => p.OldParameter));
				parameters.EndUpdate();
				foreach(IDashboardDataSource dataSource in designer.Dashboard.DataSources) {
					dataSource.SetParameters(parameters);
				}
			}
			dashboardItem.FilterString = redo ? nextFilterString : previousFilterString;
		}
	}
}
