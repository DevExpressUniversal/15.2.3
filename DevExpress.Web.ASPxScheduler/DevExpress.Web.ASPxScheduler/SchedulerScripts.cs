#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{                                                                   }
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

using DevExpress.XtraScheduler;
using DevExpress.Web.Internal;
using System;
namespace DevExpress.Web.ASPxScheduler.Internal {
	public static class ASPxSchedulerScripts {
		public const string PreventDragStartHandlerName = "return ASPx.Evt.PreventDragStart(event)";
		public static string GetPreventOnDragStart() {
			return PreventDragStartHandlerName;
		}
		public static string GetMainDivMouseDownFunction(string name) {
			return String.Format("ASPx.SchedulerMainDivMouseDown('{0}', event);", name);
		}
		public static string GetMainDivMouseUpFunction(string name) {
			return String.Format("ASPx.SchedulerMainDivMouseUp('{0}', event);", name);
		}
		public static string GetMainDivMouseClickFunction(string name) {
			return String.Format("ASPx.SchedulerMainDivMouseClick('{0}', event);", name);
		}
		public static string GetMainDivMouseDoubleClickFunction(string name) {
			return String.Format("ASPx.SchedulerMainDivMouseDoubleClick('{0}', event);", name);
		}
		public static string GetViewContextMenuFunction(string name) {
			return GetShowViewMenuFunction(name);
		}
		public static string GetShowViewMenuFunction(string name) {
			return string.Format("return ASPx.SchedulerShowViewMenu('{0}', this, event);", name);
		}
		public static string GetAppointmentContextMenuFunction(string name) {
			return string.Format("ASPx.SchedulerShowAptMenu('{0}', this, event);", name);
		}
		public static string GetAppointmentSelectionContextMenuFunction(string name) {
			return string.Format("ASPx.SchedulerShowAptMenu('{0}', this, event);return false;", name);
		}
		public static string GetLeftResizeDivMouseDownFunction(string name) {
			return string.Format("ASPx.SchedulerLeftResizeDivMouseDown('{0}', this, event);", name);
		}
		public static string GetRightResizeDivMouseDownFunction(string name) {
			return string.Format("ASPx.SchedulerRightResizeDivMouseDown('{0}', this, event);", name);
		}
		public static string GetTopResizeDivMouseDownFunction(string name) {
			return string.Format("ASPx.SchedulerTopResizeDivMouseDown('{0}', this, event);", name);
		}
		public static string GetBottomResizeDivMouseDownFunction(string name) {
			return string.Format("ASPx.SchedulerBottomResizeDivMouseDown('{0}', this, event);", name);
		}
		public static string GetNavigationButtonClickFunction(string name, TimeInterval interval, string resourceId) {
			string startTime = HtmlConvertor.ToScript(interval.Start);
			string duration = HtmlConvertor.ToScript(interval.Duration.TotalMilliseconds);
			return string.Format("ASPx.SchedulerNavBtnClick('{0}', {1}, {2}, '{3}');", name, startTime, duration, resourceId);
		}
		public static string GetCallbackHandlerFunction(int callbackHandlerID, string callbackResult) {
			return string.Format("{0}{1}this.PerformCallbackHandler({2}{3}{4});", SchedulerCallbackSR.FunctionCallbackPrefix, SchedulerCallbackSR.CallbackDivider,
			callbackHandlerID, SchedulerCallbackSR.ParameterDivider, callbackResult);
		}
	}
}
