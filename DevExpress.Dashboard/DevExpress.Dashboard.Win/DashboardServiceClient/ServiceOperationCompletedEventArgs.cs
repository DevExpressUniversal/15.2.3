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
using DevExpress.DashboardCommon.Service;
using DevExpress.DashboardCommon.Printing;
using DevExpress.DashboardCommon.Native;
namespace DevExpress.DashboardWin.Native {
	public class ServiceOperationCompletedEventArgs : EventArgs {
		public IDictionary<string, DashboardPaneContent> PaneContent { get; private set; }
		public IList<ClientActionNotification> Notifications { get; private set; }
		public List<DimensionFilterValues> MasterFilterValues { get; private set; }
		public ServiceOperationCompletedEventArgs(IDictionary<string, DashboardPaneContent> paneContent, IList<ClientActionNotification> notifications, List<DimensionFilterValues> masterFilterValues ) {
			PaneContent = paneContent;
			Notifications = notifications;
			MasterFilterValues = masterFilterValues;
		}
	}
	public enum ServiceOperationErrorType {
		DashboardNotFound = DashboardServiceResultCode.DashboardNotFound,
		DashboardNotRelevant = DashboardServiceResultCode.DashboardNotRelevant,
		InternalError = DashboardServiceResultCode.InternalError
	}
	public class ServiceOperationFailedEventArgs : EventArgs {
		public ServiceOperationErrorType ErrorType { get; private set; }
		public string ErrorMessage { get; private set; }
		public ServiceOperationFailedEventArgs(ServiceOperationErrorType errorType, string errorMessage) {
			ErrorType = errorType;
			ErrorMessage = errorMessage;
		}
	}
	public class ServiceExpandValueCompletedEventArgs : EventArgs {
		public string DashboardItemName { get; private set; }
		public HierarchicalItemData ItemData { get; private set; }
		public object[] Values { get; set; }
		public bool IsColumn { get; set; }
		public bool Expand { get; set; }
		public bool IsDataRequired { get; set; }
		public ServiceExpandValueCompletedEventArgs(string dashboardItemName, HierarchicalItemData itemData, params object[] parameters) {
			DashboardItemName = dashboardItemName;
			ItemData = itemData;
			Values = (object[])parameters[0];
			IsColumn = (bool)parameters[1];
			Expand = (bool)parameters[2];
			IsDataRequired = (bool)parameters[3];
		}
	}
}
