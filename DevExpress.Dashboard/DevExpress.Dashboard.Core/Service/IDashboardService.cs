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

using DevExpress.DashboardCommon.Layout;
using DevExpress.DashboardCommon.Native;
using DevExpress.DashboardCommon.Printing;
using DevExpress.DashboardCommon.Server;
using DevExpress.DashboardCommon.ViewModel;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
namespace DevExpress.DashboardCommon.Service {
	public interface IDashboardService {
		InitializeResult Initialize(InitializeSessionArgs initializeArgs);
		DashboardServiceResult PerformAction(PerformActionArgs performActionArgs);
		DashboardServiceResult ReloadData(ReloadDataArgs  reloadDataArgs);
		DashboardServiceResult Export(ExportArgs exportArgs);
		RefreshResult Refresh(RefreshArgs args);
		GetExportReportResult GetExportReport(ExportArgs exportArgs);
	}
	public class DashboardServiceOperationArgs {
		public int RequestMarker { get; set; }
		public Guid ServerRequestMarker { get; set; }
		public bool IsDesignMode { get; set; }
		public Hashtable ClientState { get; set; }
		public bool ForceRefreshAllItemData { get; set; }
	}
	public class InitializeSessionArgs : DashboardServiceOperationArgs {
		public string DashboardId { get; set; }
		public SessionSettings Settings { get; set; }
	}
	public class DashboardServiceSessionOperationArgs : DashboardServiceOperationArgs {
		public string SessionId { get; set; }
		public string Context { get; set; }		
	}
	public class PerformActionArgs : DashboardServiceSessionOperationArgs {
		public IEnumerable<ActionInfo> ActionInfo { get; set; }
	}
	public class ReloadDataArgs : DashboardServiceSessionOperationArgs {
		public IEnumerable<DashboardParameterInfo> Parameters { get; set; }
		public bool SuppressWaitForm { get; set; }
	}
	public class ExportArgs : DashboardServiceSessionOperationArgs {
		public Stream Stream { get; set; }
		public ExportInfo ExportInfo { get; set; }
		public IDashboardExporter Exporter { get; set; }
	}
	public class RefreshArgs : ReloadDataArgs {
	}
	public class ActionInfo {
		public string ItemName { get; set; }
		public ActionType ActionType { get; set; }
		public object[] Parameters { get; set; }
	}
	public class DashboardParameterInfo {
		public string Name { get; set; }
		public object Value { get; set; }	
	}
	public enum DashboardServiceResultCode {
		InternalError = -1,
		Success = 0,
		DashboardNotFound = 1,
		DashboardNotRelevant = 2
	}
	public class DashboardServiceResult {
		List<UnderlyingData> underlyingData = new List<UnderlyingData>();
		public string Context { get; set; }
		public Hashtable ClientState { get; set; }
		public int RequestMarker { get; set; }
		public IList<DimensionFilterValues> MasterFilterValues { get; set; }
		public IList<DashboardPaneContent> PaneContent { get; set; }
		public DashboardServiceResultCode ResultCode { get; set; }
		public string InternalErrorType { get; set; }
		public Exception Error { get; set; }
		public List<UnderlyingData> UnderlyingData { get { return underlyingData; } }
	}
	public class UnderlyingData {
		public string[] DataMembers { get; set; }
		public IEnumerable Data { get; set; }
		public string ErrorMessage { get; set; }
	}
	public class RefreshResult : DashboardServiceResult {
		public DashboardPane RootPane { get; set; }
		public DashboardTitleViewModel TitleViewModel { get; set; }
		public IList<DashboardParameterViewModel> DashboardParameters { get; set; }
	}
	public class InitializeResult : RefreshResult {
		public string SessionId { get; set; }		
		public ExportData ExportData { get; set; }
		public DashboardLocalizationViewModel Localization { get; set; }
		public string LoadingDataErrorMessage { get; set; }
	}
	public class GetExportReportResult : DashboardServiceResult {
		public IReportHolder ReportHolder { get; set; }
	}
	public class ExportData {
		public string ExportTitle { get; set; }
		public Dictionary<string, string> ExportCaptions { get; set; }
	}
	public static class DashboardDataSourceSpecialValues {
		public static object DimensionNullValue { get { return DashboardSpecialValues.NullValue; } }
		public static object OthersValue { get { return DashboardSpecialValues.OthersValue; } }
	}
}
