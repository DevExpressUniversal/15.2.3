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
using DevExpress.DashboardCommon.Native;
using DevExpress.DashboardCommon.Service;
using DevExpress.DashboardWin.Native;
using DevExpress.DashboardWin.ServiceModel;
using System;
using System.Collections;
namespace DevExpress.DashboardWin {
	public partial class DashboardViewer {
		void SubscribeDashboardEvents(Dashboard dashboard) {
			dashboard.DashboardChanged += OnDashboardChanged;
			dashboard.DashboardItemComponentNameChanged += OnDashboardItemComponentNameChanged;
			dashboard.RangeFilterRangeChanged += OnRangeFilterRangeChanged;
		}
		void UnsubscribeDashboardEvents(Dashboard dashboard) {
			dashboard.DashboardChanged -= OnDashboardChanged;
			dashboard.DashboardItemComponentNameChanged -= OnDashboardItemComponentNameChanged;
			dashboard.RangeFilterRangeChanged -= OnRangeFilterRangeChanged;
		}
		void OnDashboardChanged(object sender, EventArgs e) {
			if (!serviceClient.InServiceOperation)
				RefreshData(false);
		}
		void OnDashboardItemComponentNameChanged(object sender, ComponentNameChangedEventArgs e) {
			LayoutUpdateService.RenameLayoutItem(e.OldComponentName, e.NewComponentName);
		}
		void OnRangeFilterRangeChanged(object sender, RangeFilterRangeChangedEventArgs e) {
			if (!serviceClient.InServiceOperation) {
				RangeFilterDashboardItemViewer rangeViewer = FindDataDashboardItemViewerForAPI(e.ComponentName, false) as RangeFilterDashboardItemViewer;
				if (rangeViewer != null)
					rangeViewer.SetRange(e.MinValue, e.MaxValue);
			}
		}		
	}
}
