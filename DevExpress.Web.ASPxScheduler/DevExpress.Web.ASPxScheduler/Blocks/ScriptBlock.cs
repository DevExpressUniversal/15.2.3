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

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
using DevExpress.Web;
using DevExpress.Web.Internal;
using DevExpress.Web.ASPxScheduler.Localization;
using DevExpress.Web.ASPxScheduler.Rendering;
using DevExpress.XtraScheduler.Native;
using DevExpress.XtraScheduler;
using DevExpress.XtraScheduler.Drawing;
using System.Collections;
namespace DevExpress.Web.ASPxScheduler.Internal {
	#region ScriptControlBlock
	public abstract class ScriptControlBlock<T> : ControlBlock<T> where T : ASPxWebControl, IMasterControl {
		readonly IScriptBlockOwner scriptBuilderOwner;
		List<IControlBlock> builders;
		protected ScriptControlBlock(T control, IScriptBlockOwner scriptBuilderOwner)
			: base(control) {
			if (scriptBuilderOwner == null)
				Exceptions.ThrowArgumentNullException("scriptBuilderOwner");
			this.scriptBuilderOwner = scriptBuilderOwner;
		}
		public override string ContentControlID { get { return "scriptBlock"; } }
		protected internal IScriptBlockOwner ScriptBuilderOwner { get { return scriptBuilderOwner; } }
		protected internal List<IControlBlock> Builders { get { return builders; } }
		protected internal override void CreateControlHierarchyCore(Control parent) {
		}
		protected internal override void FinalizeCreateControlHierarchyCore(Control parent) {
		}
		protected internal override void PrepareControlHierarchyCore() {
		}
		public override CallbackResult CalcCallbackResult() {
			FinalizeCreateControlHierarchy();
			PrepareControlHierarchy();
			CallbackResult result = new CallbackResult();
			result.ClientObjectId = Owner.ClientID;
			result.ElementId = ContentControl.ClientID;
			result.InnerHtml = RenderCallbackScript();
			result.Parameters = String.Empty;
			return result;
		}
		protected internal virtual string RenderCallbackScript() {
			StringBuilder sb = new StringBuilder();
			RenderCallbackScript(sb, GetClientInstanceName(), Owner.ClientID);
			string result = RenderUtils.GetScriptHtml(sb.ToString());
			if (Browser.IsIE) {
				result = "<div style=\"display:none;\">&nbsp;</div>" + result;
			}
			return result;
		}
		protected abstract string GetClientInstanceName();
		protected internal virtual void RenderCallbackScript(StringBuilder sb, string localVarName, string clientName) {
			BeforeRenderScript();
			RenderCallbackScriptBegin(sb, localVarName, clientName);
			RenderCommonScript(sb, localVarName, clientName);
			RenderCallbackScriptEnd(sb, localVarName, clientName);
		}
		protected internal virtual void RenderPostbackScript(StringBuilder sb, string localVarName, string clientName) {
			BeforeRenderScript();
			RenderPostbackScriptBegin(sb, localVarName, clientName);
			RenderCommonScript(sb, localVarName, clientName);
			RenderPostbackScriptEnd(sb, localVarName, clientName);
		}
		protected internal virtual void BeforeRenderScript() {
			SetScriptBuilders(ScriptBuilderOwner.GetScriptBuilderOrderedList());
		}
		protected internal virtual void SetScriptBuilders(List<IControlBlock> list) {
			if (list == null)
				Exceptions.ThrowInternalException();
			this.builders = list;
		}
		public override void RenderCallbackScriptBegin(StringBuilder sb, string localVarName, string clientName) {
			int count = Builders.Count;
			for (int i = 0; i < count; i++) {
				Builders[i].RenderCallbackScriptBegin(sb, localVarName, clientName);
			}
		}
		public override void RenderCallbackScriptEnd(StringBuilder sb, string localVarName, string clientName) {
			int count = Builders.Count;
			for (int i = count - 1; i >= 0; i--) {
				Builders[i].RenderCallbackScriptEnd(sb, localVarName, clientName);
			}
		}
		public override void RenderPostbackScriptBegin(StringBuilder sb, string localVarName, string clientName) {
			int count = Builders.Count;
			for (int i = 0; i < count; i++) {
				Builders[i].RenderPostbackScriptBegin(sb, localVarName, clientName);
			}
		}
		public override void RenderPostbackScriptEnd(StringBuilder sb, string localVarName, string clientName) {
			int count = builders.Count;
			for (int i = count - 1; i >= 0; i--) {
				builders[i].RenderPostbackScriptEnd(sb, localVarName, clientName);
			}
		}
		public override void RenderCommonScript(StringBuilder sb, string localVarName, string clientName) {
			int count = builders.Count;
			for (int i = 0; i < count; i++) {
				builders[i].RenderCommonScript(sb, localVarName, clientName);
			}
		}
		protected override bool IsCollapsedToZeroSize() {
			return true;
		}
	}
	#endregion
	#region ASPxSchedulerControlScriptBlock
	public class ASPxSchedulerControlScriptBlock : ScriptControlBlock<ASPxScheduler> {
		bool isExceptionHandled;
		public ASPxSchedulerControlScriptBlock(ASPxScheduler control, IScriptBlockOwner scriptBuilderOwner)
			: base(control, scriptBuilderOwner) {
		}
		public bool IsExceptionHandled { get { return isExceptionHandled; } set { isExceptionHandled = value; } }
		protected override string GetClientInstanceName() {
			return Owner.ProtectedGetClientInstanceName();
		}
		protected internal override string RenderCallbackScript() {
			string result = base.RenderCallbackScript();
			return result;
		}
		protected internal override void RenderCallbackScript(StringBuilder sb, string localVarName, string clientName) {
			if (IsExceptionHandled) {
				RenderExceptionScript(sb, localVarName, clientName);
				return;
			}
			base.RenderCallbackScript(sb, localVarName, clientName);
		}
		void RenderExceptionScript(StringBuilder sb, string localVarName, string clientName) {
			sb.AppendFormat("{0}.OnHandledException();", localVarName);
		}
		public override void RenderCommonScript(StringBuilder sb, string localVarName, string clientName) {
			base.RenderCommonScript(sb, localVarName, clientName);
			RenderVisibleIntervalScript(Owner.ActiveView, sb, localVarName);
			RenderAppointmentDisplayOptions(Owner.ActiveView, sb, localVarName);
			RenderOptionsBehavior(Owner.OptionsBehavior, sb, localVarName);
			RenderTimeIndicatorOptions(Owner.ActiveView, sb, localVarName);
			RenderTimeMarkerDelta(Owner, sb, localVarName);
			RenderCellAutoHeightOptions(Owner.ActiveView, sb, localVarName);
		}		
		protected internal virtual void RenderCellAutoHeightOptions(SchedulerViewBase view, StringBuilder sb, string localVarName) {
			ASPxSchedulerOptionsCellAutoHeight options = GetCellAutoHeightOptions(view);
			if (options == null || options.Mode == AutoHeightMode.None)
				return;
			sb.AppendFormat("{0}.cellAutoHeightMode=\"{1}\";\n", localVarName, options.Mode.ToString());
			if (options.Mode == AutoHeightMode.LimitHeight || (options.Mode != AutoHeightMode.None && options.MinHeight > 0))
				sb.AppendFormat("{0}.cellAutoHeightConstrant=[{1},{2}];\n", localVarName, options.MinHeight, options.MaxHeight);
		}
		ASPxSchedulerOptionsCellAutoHeight GetCellAutoHeightOptions(SchedulerViewBase view) {
			WeekView weekView = view as WeekView;
			if (weekView != null)
				return weekView.CellAutoHeightOptions;
			TimelineView timelineView = view as TimelineView;
			if (timelineView != null)
				return timelineView.CellAutoHeightOptions;
			return null;
		}
		protected internal virtual void RenderTimeMarkerDelta(ASPxScheduler Owner, StringBuilder sb, string localVarName) {
			TimeSpan delta = Owner.InnerControl.TimeZoneHelper.ClientTimeZone.GetUtcOffset(GetDateTimeNow());
			sb.AppendFormat("{0}.clientUtcOffset={1};\n", localVarName, delta.TotalMilliseconds);
		}
		protected internal virtual void RenderOptionsBehavior(ASPxSchedulerOptionsBehavior options, StringBuilder sb, string localVarName) {			
		}
		void RenderTimeIndicatorOptions(SchedulerViewBase view, StringBuilder sb, string localVarName) {
			TimeIndicatorDisplayOptions options = ObtainTimeIndicatorDisplayOptions(view);
			if (options == null)
				return;
			TimeIndicatorVisibility timeIndicatorVisibility = options.Visibility;
			if (timeIndicatorVisibility == TimeIndicatorVisibility.Never)
				return;
			sb.AppendFormat("{0}.timeIndicatorVisibility={1};\n", localVarName, (int)timeIndicatorVisibility);
		}
		TimeIndicatorDisplayOptions ObtainTimeIndicatorDisplayOptions(SchedulerViewBase view) {
			DayView dayView = view as DayView;
			if (dayView != null)
				return dayView.TimeIndicatorDisplayOptions;
			TimelineView timelineView = view as TimelineView;
			if (timelineView != null)
				return timelineView.TimeIndicatorDisplayOptions;
			return null;
		}
		void RenderAppointmentDisplayOptions(SchedulerViewBase view, StringBuilder sb, string localVarName) {
			int appointmentVerticalInterspacing = view.AppointmentDisplayOptionsInternal.InnerAppointmentVerticalInterspacing;
			if (appointmentVerticalInterspacing != 2) {
				sb.AppendFormat("{0}.appointmentVerticalInterspacing = {1};\n", localVarName, appointmentVerticalInterspacing);
			}
			DayView dayView = Owner.ActiveView as DayView;
			if (dayView == null)
				return;
			SchedulerColumnPadding columnPadding = dayView.AppointmentDisplayOptions.ColumnPadding;
			if (columnPadding.Left > 0)
				sb.AppendFormat("{0}.leftColumnPadding = {1};\n", localVarName, columnPadding.Left);
			if (columnPadding.Right != 2)
				sb.AppendFormat("{0}.rightColumnPadding = {1};\n", localVarName, columnPadding.Right);
		}
		internal void RenderVisibleIntervalScript(SchedulerViewBase activeView, StringBuilder sb, string localVarName) {
			List<object> transferObject = new List<object>();
			TimeIntervalCollection intervals = activeView.GetVisibleIntervals();
			int count = intervals.Count;
			for (int i = 0; i < count; i++) {
				TimeInterval interval = intervals[i];
				ClientIntervalProperties intervalProperties = new ClientIntervalProperties(interval);
				transferObject.Add(intervalProperties.ToPropertiesDictionary(null, null));
			}
			string jsonString = HtmlConvertor.ToJSON(transferObject);
			sb.AppendFormat("{0}.SetVisibleInterval({1});\n", localVarName, jsonString);
		}
		public override void RenderCallbackScriptEnd(StringBuilder sb, string localVarName, string clientName) {
			RenderReminderScript(sb, localVarName, clientName);
			base.RenderCallbackScriptEnd(sb, localVarName, clientName);
			sb.AppendFormat("{0}.ShowResourceNavigatorRow({1});\n", localVarName, HtmlConvertor.ToScript(Owner.ResourceNavigatorBlock.IsBlockVisible));
			RenderCheckSums(sb, localVarName, clientName);
		}
		internal void RenderReminderScript(StringBuilder sb, string localVarName, string clientName) {
			ReminderState reminderState = Owner.ReminderState;
			if (reminderState.IsRequireSetClientTimer && reminderState.IsRemindersExists()) {
				DateTime reminderTime = reminderState.GetNearestFutureReminderTime();
				TimeSpan delay = reminderTime - GetDateTimeNow();
				if (delay.Ticks <= 0)
					delay = TimeSpan.FromMilliseconds(1);
				if (delay.TotalDays > 10) 
					delay = TimeSpan.FromMinutes(20);
				sb.AppendFormat("{0}.SetReminders({1});\n", localVarName, Math.Truncate(delay.TotalMilliseconds));
			}
		}	  
		void RenderCheckSums(StringBuilder sb, string localVarName, string clientName) {
			string result = String.Empty;
			bool first = true;
			foreach (string elementId in Owner.BlockCheckSums.Keys) {
				if (!first) {
					result += " ";
				}
				result += String.Format("{0} {1}", elementId, Owner.BlockCheckSums[elementId]);
				first = false;
			}
			sb.AppendFormat("{0}.SetCheckSums(\"{1}\");\n", localVarName, result);
		}
		public override void RenderPostbackScriptEnd(StringBuilder sb, string localVarName, string clientName) {
			base.RenderPostbackScriptEnd(sb, localVarName, clientName);
			Owner.MasterControlDefaultImplementation.AddRelatedControlsRegistrationScript(sb, clientName);
			StringCollection formats = DateTimeFormatHelper.GenerateFormatsWithoutYear();
			sb.AppendFormat("{0}.formatsWithoutYear = {1};\n", localVarName, HtmlConvertor.ToJSON(formats));
			formats = DateTimeFormatHelper.GenerateFormatsWithoutYearAndWeekDay();
			sb.AppendFormat("{0}.formatsWithoutYearAndWeekDay = {1};\n", localVarName, HtmlConvertor.ToJSON(formats));
			formats = DateTimeFormatHelper.GenerateFormatsWithYearForJanuary();
			sb.AppendFormat("{0}.formatsNewYear = {1};\n", localVarName, HtmlConvertor.ToJSON(formats));
			formats = DateTimeFormatHelper.GenerateFormatsTimeOnly();
			sb.AppendFormat("{0}.formatsTimeOnly = {1};\n", localVarName, HtmlConvertor.ToJSON(formats));
			formats = DateTimeFormatHelper.GenerateFormatsTimeWithMonthDay();
			sb.AppendFormat("{0}.formatsTimeWithMonthDay = {1};\n", localVarName, HtmlConvertor.ToJSON(formats));
			formats = DateTimeFormatHelper.GenerateFormatsDateTimeWithYear();
			sb.AppendFormat("{0}.formatsDateTimeWithYear = {1};\n", localVarName, HtmlConvertor.ToJSON(formats));
			formats = DateTimeFormatHelper.GenerateFormatsDateWithYear();
			sb.AppendFormat("{0}.formatsDateWithYear = {1};\n", localVarName, HtmlConvertor.ToJSON(formats));
			sb.AppendFormat("{0}.operationToolTipCaption = '{1}';\n", localVarName, HtmlConvertor.EscapeString(ASPxSchedulerLocalizer.GetString(ASPxSchedulerStringId.Caption_OperationToolTip)));
			sb.AppendFormat("{0}.SetCheckSums(\"\");\n", localVarName);
			sb.AppendFormat("{0}.EnsureAllToolTipInitialized();\n", localVarName);
			RenderReminderScript(sb, localVarName, clientName);
		}
		protected internal virtual DateTime GetDateTimeNow() {
			return DateTime.Now;
		}
	}
	#endregion
	public interface IHeaderFormatScriptRenderer {
		void RenderScript(StringBuilder sb, string localVarName, Table parentTable);
		ICollection RemoveHeadersWithCustomCaption();
	}
	#region HeaderFormatScriptRenderer
	public abstract class HeaderFormatScriptRenderer<T> : IHeaderFormatScriptRenderer where T : InternalSchedulerCell {
		readonly List<T> headers;
		protected HeaderFormatScriptRenderer() {
			this.headers = new List<T>();
		}
		#region Properties
		public List<T> Headers { get { return headers; } }
		protected internal abstract string FormatGroupName { get; }
		#endregion
		#region RenderScript
		public virtual void RenderScript(StringBuilder sb, string localVarName, Table parentTable) {
			if (IsRequireRenderScriptForHeaders(headers)) {
				sb.AppendFormat("{0}.{1} = {2};\n", localVarName, FormatGroupName, GetDatesFromHeaders(localVarName));
				sb.AppendFormat("{0}.{1}Locations = {2};\n", localVarName, FormatGroupName, GetHeadersLocations(parentTable));
				string customToolTips = GetCustomToolTips();
				if (!String.IsNullOrEmpty(customToolTips))
					sb.AppendFormat("{0}.{1}ToolTips = {2};\n", localVarName, FormatGroupName, customToolTips);
				string customCaptions = GetCustomCaptions();
				if (!String.IsNullOrEmpty(customCaptions))
					sb.AppendFormat("{0}.{1}Captions = {2};\n", localVarName, FormatGroupName, customCaptions);
			}
		}
		#endregion
		#region Add
		public void Add(T header) {
			headers.Add(header);
		}
		#endregion
		#region IsRequireRenderScriptForHeaders
		protected internal virtual bool IsRequireRenderScriptForHeaders(List<T> headers) {
			return headers.Count > 0 && !HeadersContainTemplate(headers);
		}
		#endregion
		#region HeadersContainTemplate
		protected internal virtual bool HeadersContainTemplate(List<T> headers) {
			int count = headers.Count;
			for (int i = 0; i < count; i++) {
				if (IsTemplateInserted(headers[i].Controls))
					return true;
			}
			return false;
		}
		#endregion
		#region IsTemplateInserted
		bool IsTemplateInserted(ControlCollection controls) {
			int count = controls.Count;
			for (int i = 0; i < count; i++) {
				Control container = controls[0];
				if (container is SchedulerTemplateContainerBase)
					return true;
			}
			return false;
		}
		#endregion
		#region GetDatesFromHeaders
		protected internal virtual string GetDatesFromHeaders(string localVarName) {
			int count = Headers.Count;
			List<String> utcDatesInMs = new List<String>();
			for (int i = 0; i < count; i++) {
				ITimeCell headerTimeCell = Headers[i] as ITimeCell;
				if (headerTimeCell == null || headerTimeCell.Interval == null)
					continue;
				utcDatesInMs.Add(SchedulerWebUtils.ToJavaScriptDate(headerTimeCell.Interval.Start));
			}
			return String.Format("{0}.ConvertToSchedulerUTCDates({1})", localVarName, HtmlConvertor.ToJSON(utcDatesInMs));
		}
		#endregion
		#region GetHeadersLocations
		protected virtual string GetHeadersLocations(Table table) {
			StringBuilder result = new StringBuilder();
			int count = Headers.Count;
			result.Append("[");
			for (int i = 0; i < count; i++) {
				result.Append(CellLocationHelper.GetCellLocationString(Headers[i], table));
				if (i != count - 1)
					result.Append(", ");
			}
			result.Append("]");
			return result.ToString();
		}
		#endregion
		#region GetCustomToolTips
		protected internal virtual string GetCustomToolTips() {
			List<string> result = new List<string>();
			bool useCustomTooltip = false;
			int count = Headers.Count;
			for (int i = 0; i < count; i++) {
				string toolTip = Headers[i].ShadowToolTip;
				if (!String.IsNullOrEmpty(toolTip))
					useCustomTooltip = true;
				result.Add(@"""" + toolTip + @"""");
			}
			return useCustomTooltip ? "[" + String.Join(",", result.ToArray()) + "]" : String.Empty;
		}
		#endregion
		#region GetCustomCaptions
		protected internal virtual string GetCustomCaptions() {
			List<string> result = new List<string>();
			bool useCustomCaption = false;
			int count = Headers.Count;
			for (int i = 0; i < count; i++) {
				string captionText = Headers[i].ShadowText;
				if (!String.IsNullOrEmpty(captionText)) {
					useCustomCaption = true;
					result.Add(@"""" + captionText + @"""");
				}
				else
					result.Add(String.Empty);
			}
			return useCustomCaption ? "[" + String.Join(",", result.ToArray()) + "]" : String.Empty;
		}
		#endregion
		#region RemoveHeadersWithCustomCaption
		public virtual ICollection RemoveHeadersWithCustomCaption() {
			List<T> headersToRemove = new List<T>();
			int count = Headers.Count;
			for (int i = count - 1; i >= 0; i--) {
				T header = Headers[i];
				if (String.IsNullOrEmpty(header.ShadowText))
					continue;
				headersToRemove.Add(header);
				Headers.Remove(header);
			}
			return (ICollection)headersToRemove;
		}
		#endregion
	}
	#endregion
	#region HeaderFormatScriptWithoutYearRenderer
	public class HeaderFormatScriptWithoutYearRenderer : HeaderFormatScriptRenderer<WebDateHeader> {
		protected internal override string FormatGroupName { get { return "datesForFormatsWithoutYear"; } }
	}
	#endregion
	#region HeaderFormatScriptWithoutYearAndWeekDayRenderer
	public class HeaderFormatScriptWithoutYearAndWeekDayRenderer : HeaderFormatScriptRenderer<WebDateHeader> {
		protected internal override string FormatGroupName { get { return "datesForFormatsWithoutYearAndWeekDay"; } }
	}
	#endregion
	#region HeaderFormatScriptNewYearRenderer
	public class HeaderFormatScriptNewYearRenderer : HeaderFormatScriptRenderer<WebDateHeader> {
		protected internal override string FormatGroupName { get { return "datesForFormatsNewYear"; } }
	}
	#endregion
	#region HeaderFormatScriptDayOfWeekRenderer
	public class HeaderFormatScriptDayOfWeekRenderer : HeaderFormatScriptRenderer<WebDayOfWeekHeader> {
		protected internal override string FormatGroupName { get { return "daysForDayOfWeekFormats"; } }
		#region GetDatesFromHeaders
		protected internal override string GetDatesFromHeaders(string localVar) {
			int count = Headers.Count;
			if (count < 1)
				return String.Format("[]");
			List<string> result = new List<string>();
			for (int i = 0; i < count; i++) {
				int dayOfWeekCode = GetDayOfWeekCode(Headers[i]);
				result.Add(dayOfWeekCode.ToString());
			}
			return "[" + String.Join(",", result.ToArray()) + "]";
		}
		int GetDayOfWeekCode(WebDayOfWeekHeader webDayOfWeekHeader) {
			if (webDayOfWeekHeader is WebDayCompressedHeader)
				return 8;
			return (int)webDayOfWeekHeader.DayOfWeek;
		}
		#endregion
	}
	#endregion
	#region HeaderFormatScriptDayNumberRenderer
	public class HeaderFormatScriptDayNumberRenderer : HeaderFormatScriptRenderer<WebDateHeader> {
		protected internal override string FormatGroupName { get { return "datesForDayNumberFormat"; } }
	}
	#endregion
	#region CustomDateHeaderFormatScriptRenderer
	public class CustomDateHeaderFormatScriptRenderer : HeaderFormatScriptRenderer<WebDateHeader> {
		protected internal override string FormatGroupName { get { return "datesForDateCustomFormat"; } }
		public override ICollection RemoveHeadersWithCustomCaption() {
			return null;
		}
	}
	#endregion
	#region CustomDayOfWeekHeaderFormatScriptRenderer
	public class CustomDayOfWeekHeaderFormatScriptRenderer : HeaderFormatScriptDayOfWeekRenderer {
		protected internal override string FormatGroupName { get { return "datesForDayOfWeekCustomFormat"; } }
		public override ICollection RemoveHeadersWithCustomCaption() {
			return null;
		}
	}
	#endregion
	#region HeaderFormatSeparatorBase
	public abstract class HeaderFormatSeparatorBase {
		#region Fields
		readonly HeaderFormatScriptWithoutYearRenderer headerFormatScriptWithoutYearRenderer;
		readonly HeaderFormatScriptWithoutYearAndWeekDayRenderer headerFormatScriptWithoutYearAndWeekDayRenderer;
		readonly HeaderFormatScriptNewYearRenderer headerFormatScriptNewYearRenderer;
		readonly HeaderFormatScriptDayOfWeekRenderer headerFormatScriptDayOfWeekRenderer;
		readonly HeaderFormatScriptDayNumberRenderer headerFormatScriptDayNumberRenderer;
		readonly CustomDateHeaderFormatScriptRenderer customDateHeaderFormatScriptRenderer;
		readonly CustomDayOfWeekHeaderFormatScriptRenderer customDayOfWeekHeaderFormatScriptRenderer;
		readonly List<IHeaderFormatScriptRenderer> scriptRenderers;
		#endregion
		protected HeaderFormatSeparatorBase() {
			this.headerFormatScriptWithoutYearRenderer = new HeaderFormatScriptWithoutYearRenderer();
			this.headerFormatScriptWithoutYearAndWeekDayRenderer = new HeaderFormatScriptWithoutYearAndWeekDayRenderer();
			this.headerFormatScriptNewYearRenderer = new HeaderFormatScriptNewYearRenderer();
			this.headerFormatScriptDayOfWeekRenderer = new HeaderFormatScriptDayOfWeekRenderer();
			this.headerFormatScriptDayNumberRenderer = new HeaderFormatScriptDayNumberRenderer();
			this.customDateHeaderFormatScriptRenderer = new CustomDateHeaderFormatScriptRenderer();
			this.customDayOfWeekHeaderFormatScriptRenderer = new CustomDayOfWeekHeaderFormatScriptRenderer();
			this.scriptRenderers = new List<IHeaderFormatScriptRenderer>();
			RegisterScriptRenderers();
		}
		#region Properties
		public HeaderFormatScriptWithoutYearRenderer HeaderFormatScriptWithoutYearRenderer { get { return headerFormatScriptWithoutYearRenderer; } }
		public HeaderFormatScriptWithoutYearAndWeekDayRenderer HeaderFormatScriptWithoutYearAndWeekDayRenderer { get { return headerFormatScriptWithoutYearAndWeekDayRenderer; } }
		public HeaderFormatScriptNewYearRenderer HeaderFormatScriptNewYearRenderer { get { return headerFormatScriptNewYearRenderer; } }
		public HeaderFormatScriptDayOfWeekRenderer HeaderFormatScriptDayOfWeekRenderer { get { return headerFormatScriptDayOfWeekRenderer; } }
		public HeaderFormatScriptDayNumberRenderer HeaderFormatScriptDayNumberRenderer { get { return headerFormatScriptDayNumberRenderer; } }
		public CustomDateHeaderFormatScriptRenderer CustomDateHeaderFormatScriptRenderer { get { return customDateHeaderFormatScriptRenderer; } }
		public CustomDayOfWeekHeaderFormatScriptRenderer CustomDayOfWeekHeaderFormatScriptRenderer { get { return customDayOfWeekHeaderFormatScriptRenderer; } }
		public List<IHeaderFormatScriptRenderer> ScriptRenderers { get { return scriptRenderers; } }
		#endregion
		#region Add
		public abstract void Add(WebDateHeader header);
		public virtual void Add(WebDayOfWeekHeaderCollection headers) {
			int count = headers.Count;
			for (int i = 0; i < count; i++) {
				WebDayOfWeekHeader header = headers[i];
				headerFormatScriptDayOfWeekRenderer.Add(header);
			}
		}
		#endregion
		#region RenderScript
		public void RenderScript(StringBuilder sb, string localVarName, Table parentTable) {
			int count = ScriptRenderers.Count;
			for (int i = 0; i < count; i++)
				ScriptRenderers[i].RenderScript(sb, localVarName, parentTable);
		}
		#endregion
		#region RemoveHeadersWithCustomCaption
		public void RemoveHeadersWithCustomCaption() {
			int count = ScriptRenderers.Count;
			for (int i = 0; i < count; i++) {
				ICollection headers = ScriptRenderers[i].RemoveHeadersWithCustomCaption();
				List<WebDateHeader> webDateHeaders = headers as List<WebDateHeader>;
				if (webDateHeaders != null) {
					AddHeadersToCustomDateHeaderFormatScript(webDateHeaders);
					continue;
				}
				List<WebDayOfWeekHeader> webDayOfWeekHeaders = headers as List<WebDayOfWeekHeader>;
				if (webDayOfWeekHeaders != null)
					AddHeadersToDayOfWeekHeaderFormatScript(webDayOfWeekHeaders);
			}
		}
		#endregion
		#region AddHeadersToCustomDateHeaderFormatScript
		protected internal virtual void AddHeadersToCustomDateHeaderFormatScript(List<WebDateHeader> headers) {
			int count = headers.Count;
			for (int i = 0; i < count; i++)
				CustomDateHeaderFormatScriptRenderer.Add(headers[i]);
		}
		#endregion
		#region AddHeadersToDayOfWeekHeaderFormatScript
		protected internal virtual void AddHeadersToDayOfWeekHeaderFormatScript(List<WebDayOfWeekHeader> headers) {
			int count = headers.Count;
			for (int i = 0; i < count; i++)
				CustomDayOfWeekHeaderFormatScriptRenderer.Add(headers[i]);
		}
		#endregion
		#region RegisterScriptRenderers
		void RegisterScriptRenderers() {
			ScriptRenderers.Add(HeaderFormatScriptWithoutYearRenderer);
			ScriptRenderers.Add(HeaderFormatScriptWithoutYearAndWeekDayRenderer);
			ScriptRenderers.Add(HeaderFormatScriptNewYearRenderer);
			ScriptRenderers.Add(HeaderFormatScriptDayOfWeekRenderer);
			ScriptRenderers.Add(HeaderFormatScriptDayNumberRenderer);
			ScriptRenderers.Add(CustomDateHeaderFormatScriptRenderer);
			ScriptRenderers.Add(CustomDayOfWeekHeaderFormatScriptRenderer);
		}
		#endregion
	}
	#endregion
	#region WeekViewGroupByDateHeaderFormatSeparator
	public class WeekViewGroupByDateHeaderFormatSeparator : HeaderFormatSeparatorBase {
		public override void Add(WebDateHeader header) {
			if (IsLongFormatHeaderCaption(header)) {
				if (IsNewYearFormatHeaderCaption(header))
					HeaderFormatScriptNewYearRenderer.Add(header);
				else
					HeaderFormatScriptWithoutYearAndWeekDayRenderer.Add(header);
			}
			else
				HeaderFormatScriptDayNumberRenderer.Add(header);
		}
		protected internal virtual bool IsLongFormatHeaderCaption(WebDateHeader header) {
			return header.FirstVisible || header.Interval.Start.Date.Day == 1;
		}
		protected internal virtual bool IsNewYearFormatHeaderCaption(WebDateHeader header) {
			DateTime date = header.Interval.Start.Date;
			return date.Day == 1 && date.Month == 1;
		}
	}
	#endregion
	#region WeekViewGroupByNoneHeaderFormatSeparator
	public class WeekViewGroupByNoneHeaderFormatSeparator : HeaderFormatSeparatorBase {
		public override void Add(WebDateHeader header) {
			HeaderFormatScriptWithoutYearRenderer.Add(header);
		}
	}
	#endregion
	#region WeekViewGroupByResourceHeaderFormatSeparator
	public class WeekViewGroupByResourceHeaderFormatSeparator : HeaderFormatSeparatorBase {
		public override void Add(WebDateHeader header) {
			HeaderFormatScriptWithoutYearRenderer.Add(header);
		}
	}
	#endregion
	#region DayViewHeaderFormatSeparator
	public class DayViewHeaderFormatSeparator : HeaderFormatSeparatorBase {
		public override void Add(WebDateHeader header) {
			HeaderFormatScriptWithoutYearRenderer.Add(header);
		}
	}
	#endregion
	#region MonthViewHeaderFormatSeparatorBase
	public class MonthViewHeaderFormatSeparatorBase : WeekViewGroupByDateHeaderFormatSeparator {
	}
	#endregion
	public static class CellLocationHelper {
		#region GetHeaderLocationString
		static public string GetCellLocationString(InternalSchedulerCell cell, Table table) {
			TableRow row = (TableRow)cell.Parent;
			int rowIndex = table.Rows.GetRowIndex(row);
			if (rowIndex < 0)
				return String.Empty;
			int cellIndex = row.Cells.GetCellIndex(cell);
			if (cellIndex < 0)
				return String.Empty;
			return String.Format("[{0},{1}]", rowIndex, cellIndex);
		}
		#endregion
		#region GetCellPathLocationString
		static public string GetCellPathLocationString(InternalSchedulerCell cell, Table parentTable) {
			List<string> path = new List<string>();
			Table table = null;
			while (parentTable != table) {
				TableRow row = (TableRow)cell.Parent;
				table = (Table)row.Parent;
				if (table == null)
					return String.Empty;
				int rowIndex = table.Rows.GetRowIndex(row);
				int cellIndex = row.Cells.GetCellIndex(cell);
				path.Add(String.Format("[{0},{1}]", rowIndex, cellIndex));
				cell = table.Parent as InternalSchedulerCell;
				if (cell == null)
					break;
			}
			int count = path.Count;
			StringBuilder result = new StringBuilder();
			result.Append("[");
			for (int i = count - 1; i >= 0; i--) {
				result.Append(path[i]);
				if (i != 0)
					result.Append(",");
			}
			result.Append("]");
			return result.ToString();
		}
		#endregion
	}
}
