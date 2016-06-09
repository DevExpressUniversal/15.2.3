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
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Reflection;
using System.Web;
using System.Web.Caching;
using System.Web.Hosting;
using System.Web.UI;
using System.Web.UI.WebControls;
using DevExpress.Utils.Serializing;
using DevExpress.Utils.Serializing.Helpers;
using DevExpress.Web.Internal;
using DevExpress.Web;
using DevExpress.Web.ASPxScheduler.Rendering;
using DevExpress.XtraScheduler;
using DevExpress.XtraScheduler.Design;
using DevExpress.XtraScheduler.Native;
using System.Text;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using System.Collections.Specialized;
using DevExpress.Utils;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using DevExpress.XtraScheduler.Internal.Diagnostics;
using DevExpress.XtraScheduler.Internal.Implementations;
internal class ToolboxBitmapAccess {
	public const string BitmapPath = "Bitmaps256.";
}
namespace DevExpress.Web.ASPxScheduler {
	public class AppointmentNotFoundException : Exception {
		public AppointmentNotFoundException(string message)
			: base(message) {
		}
		public AppointmentNotFoundException(string message, Exception innerException)
			: base(message, innerException) {
		}
		protected AppointmentNotFoundException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
			: base(info, context) {
		}
	}
}
namespace DevExpress.Web.ASPxScheduler.Internal {
	internal static class ExceptionText {
		internal const string CannotFindAppointment = "Cannot find the appointment with ID: {0}";
	}
	public class StringValuePersistentStorage {
		Dictionary<String, String> formUrlDictionary = new Dictionary<string, string>();
		public void SetProperty(string name, string value) {
			this.formUrlDictionary[name] = value;
		}
		public String GetProperty(string name) {
			if (!this.formUrlDictionary.ContainsKey(name))
				return String.Empty;
			return this.formUrlDictionary[name];
		}
	}
	public abstract class HashValueBase {
		readonly static HashCalculatorBase HashCalculator;
		static HashValueBase() {
			HashCalculator = HashCalculatorBase.CreateHashCalculator();
		}
		readonly byte[] hash;
		protected HashValueBase(byte[] hash) {
			if (hash == null || hash.Length != HashLength)
				Exceptions.ThrowArgumentException("hash", hash);
			this.hash = hash;
		}
		protected abstract int HashLength { get; }
		public override int GetHashCode() {
			return hash.GetHashCode();
		}
		public override bool Equals(object obj) {
			HashValueBase objHash = obj as HashValueBase;
			if (objHash == null)
				return false;
			if (this.GetType() != obj.GetType())
				return false;
			for (int i = 0; i < HashLength; i++)
				if (hash[i] != objHash.hash[i])
					return false;
			return true;
		}
		public override string ToString() {
			return Convert.ToBase64String(hash);
		}
		public static bool operator ==(HashValueBase hash1, HashValueBase hash2) {
			if (Object.ReferenceEquals(hash1, hash2))
				return true;
			if (Object.ReferenceEquals(hash1, null) || Object.ReferenceEquals(hash2, null))
				return false;
			return hash1.Equals(hash2);
		}
		public static bool operator !=(HashValueBase hash1, HashValueBase hash2) {
			return !(hash1 == hash2);
		}
		public static HashValueBase CalcHashValue(string str) {
			byte[] bytes = Encoding.Unicode.GetBytes(str);
			return HashCalculator.ComputeHash(bytes);
		}
		public static HashValueBase FromBase64String(string str) {
			byte[] bytes = Convert.FromBase64String(str);
			return HashCalculator.CreateHash(bytes);
		}
	}
	public abstract class HashCalculatorBase {
		public abstract HashValueBase ComputeHash(byte[] bytes);
		public static HashCalculatorBase CreateHashCalculator() {
			try {
				new MD5CryptoServiceProvider(); 
				return new MD5HashCalculator();
			} catch {
				return new SHA1HashCalculator();
			}
		}
		class MD5HashValue : HashValueBase {
			public MD5HashValue(byte[] hash)
				: base(hash) {
			}
			protected override int HashLength { get { return 16; } }
		}
		class SHA1HashValue : HashValueBase {
			public SHA1HashValue(byte[] hash)
				: base(hash) {
			}
			protected override int HashLength { get { return 20; } }
		}
		class MD5HashCalculator : HashCalculatorBase {
			public override HashValueBase ComputeHash(byte[] bytes) {
				MD5 md5 = new MD5CryptoServiceProvider();
				return new MD5HashValue(md5.ComputeHash(bytes));
			}
			public override HashValueBase CreateHash(byte[] bytes) {
				return new MD5HashValue(bytes);
			}
		}
		class SHA1HashCalculator : HashCalculatorBase {
			public override HashValueBase ComputeHash(byte[] bytes) {
				SHA1CryptoServiceProvider md5 = new SHA1CryptoServiceProvider();
				return new SHA1HashValue(md5.ComputeHash(bytes));
			}
			public override HashValueBase CreateHash(byte[] bytes) {
				return new SHA1HashValue(bytes);
			}
		}
		public abstract HashValueBase CreateHash(byte[] bytes);
	}
	public class ViewAppointmentOperationFlags {
		bool allowAppointmentCreate = true;
		bool allowAppointmentConflicts = true;
		public bool AllowAppointmentCreate { get { return allowAppointmentCreate; } set { allowAppointmentCreate = value; } }
		public bool AllowAppointmentConflicts { get { return allowAppointmentConflicts; } set { allowAppointmentConflicts = value; } }
	}
	public class AppointmentOperationFlags {
		bool allowDelete = true;
		bool allowEdit = true;
		bool allowResize = true;
		bool allowCopy = true;
		bool allowDrag = true;
		bool allowDragBetweenResources = true;
		bool allowInplaceEditor = true;
		bool allowConflicts = true;
		public bool AllowDelete { get { return allowDelete; } set { allowDelete = value; } }
		public bool AllowEdit { get { return allowEdit; } set { allowEdit = value; } }
		public bool AllowResize { get { return allowResize; } set { allowResize = value; } }
		public bool AllowCopy { get { return allowCopy; } set { allowCopy = value; } }
		public bool AllowDrag { get { return allowDrag; } set { allowDrag = value; } }
		public bool AllowDragBetweenResources { get { return allowDragBetweenResources; } set { allowDragBetweenResources = value; } }
		public bool AllowInplaceEditor { get { return allowInplaceEditor; } set { allowInplaceEditor = value; } }
		public bool AllowConflicts { get { return allowConflicts; } set { allowConflicts = value; } }
	}
	public class AppointmentOperationFlagsHelper {
		public AppointmentOperationFlags CalculateAppointmentFlags(InnerSchedulerControl control, Appointment apt) {
			AppointmentOperationFlags flags = new AppointmentOperationFlags();
			AppointmentOperationHelper helper = new AppointmentOperationHelper(control);
			flags.AllowDelete = helper.CanDeleteAppointment(apt);
			flags.AllowEdit = helper.CanEditAppointment(apt);
			flags.AllowResize = helper.CanResizeAppointment(apt);
			flags.AllowCopy = helper.CanCopyAppointment(apt);
			flags.AllowDrag = helper.CanDragAppointment(apt);
			flags.AllowDragBetweenResources = helper.CanDragAppointmentBetweenResources(apt);
			flags.AllowInplaceEditor = helper.CanEditAppointmentViaInplaceEditor(apt);
			flags.AllowConflicts = true; 
			return flags;
		}
		public ViewAppointmentOperationFlags CalculateViewAppointmentOperations(InnerSchedulerControl control, TimeInterval interval, XtraScheduler.Resource resource, bool recurring) {
			ViewAppointmentOperationFlags flags = new ViewAppointmentOperationFlags();
			AppointmentOperationHelper helper = new AppointmentOperationHelper(control);
			flags.AllowAppointmentCreate = helper.CanCreateAppointment(interval, resource, recurring);
			flags.AllowAppointmentConflicts = true; 
			return flags;
		}
		public static string GenerateScriptAppointmentFlagsParameters(AppointmentOperationFlags flags) {
			string result = string.Empty;
			result += SchedulerWebUtils.BoolToStr(flags.AllowDelete);
			result += SchedulerWebUtils.BoolToStr(flags.AllowEdit);
			result += SchedulerWebUtils.BoolToStr(flags.AllowResize);
			result += SchedulerWebUtils.BoolToStr(flags.AllowCopy);
			result += SchedulerWebUtils.BoolToStr(flags.AllowDrag);
			result += SchedulerWebUtils.BoolToStr(flags.AllowDragBetweenResources);
			result += SchedulerWebUtils.BoolToStr(flags.AllowInplaceEditor);
			result += SchedulerWebUtils.BoolToStr(flags.AllowConflicts);
			return result;
		}
	}
	public static class SchedulerWebStyleHelper {
		internal static void SetDisplayAttributeCore(WebControl control, HtmlTextWriterStyle key, string value) {
			control.Style.Add(key, value);
		}
		public static void SetDisplayAttribute(WebControl control, bool visible) {
			if (visible)
				SetDisplayBlock(control);
			else
				SetDisplayNone(control);
		}
		public static void SetDisplayNone(WebControl control) {
			SetDisplayAttributeCore(control, HtmlTextWriterStyle.Display, "none");
		}
		public static void SetDisplayBlock(WebControl control) {
			SetDisplayAttributeCore(control, HtmlTextWriterStyle.Display, "block");
		}
	}
	public static class SchedulerWebEventHelper {
		internal static void AddEventCore(WebControl control, string eventName, string eventHandler) {
			RenderUtils.SetStringAttribute(control, eventName, eventHandler);
		}
		public static void AddMouseDownEvent(WebControl control, string eventHandler) {
			AddEventCore(control, "onmousedown", eventHandler);
		}
		public static void AddContextMenuEvent(WebControl control, string eventHandler) {
			AddEventCore(control, "oncontextmenu", eventHandler);
		}
		public static void AddClickEvent(WebControl control, string eventHandler) {
			AddEventCore(control, "onclick", eventHandler);
		}
		public static void AddDoubleClickEvent(WebControl control, string eventHandler) {
			AddEventCore(control, "ondblclick", eventHandler); 
		}
		public static void AddMouseUpEvent(Table control, string eventHandler) {
			AddEventCore(control, "onmouseup", eventHandler);
		}
		public static void AddClearSelectionEvent(WebControl control) {
			AddEventCore(control, "onselectstart", "return false;");
		}
		public static void AddOnDragStartEvent(WebControl control, string eventHandler) {
			AddEventCore(control, "ondragstart", eventHandler);
		}
	}
	public static class SchedulerIdHelper {
		public const string NewAppointmentId = "_APTNULL"; 
		public const string NonPermanentAppointmentDivSuffix = "TMP";
		public const string EmptyResourceId = "null";
		public const string EmptyId = "null";
		#region Scheduler Form Templates
		public const string FormTemplateControlId = "FrmTemplate";
		public const string AppointmentFormContainerId = "AptFrmContainer";
		public const string AppointmentFormTemplateContainerId = "AptFrmTemplateContainer";
		public const string AppointmentInplaceEditorContainerId = "AptInpEdtContainer";
		public const string AppointmentEditorTemplateContainerId = "AptEdtTemplateContainer";
		public const string GotoDateFormContainerId = "GotoDateFrmContainer";
		public const string GotoDateFormTemplateContainerId = "GotoDateFrmTemplateContainer";
		public const string RecurrentAppointmentDeleteFormContainerId = "RecurAptDeleteFrmContainer";
		public const string RecurrentAppointmentDeleteFormTemplateContainerId = "RecurAptDeleteFrmTemplateContainer";
		public const string RecurrentAppointmentEditFormContainerId = "RecurAptEditFrmContainer";
		public const string RecurrentAppointmentEditFormTemplateContainerId = "RecurAptDeleteFrmTemplateContainer";
		public const string RemindersFormContainerId = "RemindersFrmContainer";
		public const string RemindersFormTemplateContainerId = "RemindersFrmTemplateContainer";
		public const string MessageBoxTemplateContainerId = "MessageBoxTemplateContainer";
		#endregion
		#region Scheduler Blocks
		public const string AppointmentsBlockId = "aptsBlock";
		#endregion
		#region Scheduler Templates
		public const string InternalCellTemplateContainerId = "CellTC";
		public const string AppointmentTemplateContainerId = "AptTemplateContainer";
		#endregion
		public const string NavigationButtonDivId = "navBtnDiv";
		public const string AppointmentDivId = "AptDiv";
		public static string GenerateNavigationButtonDivId(int divIndex) {
			return String.Format("{0}{1}", NavigationButtonDivId, divIndex);
		}
		public static string GenerateResourceId(XtraScheduler.Resource resource) {
			return GenerateResourceId(resource.Id);
		}
		public static string GenerateAppointmentDivId(int aptIndex) {
			return String.Format("{0}{1}", AppointmentDivId, aptIndex);
		}
		public static string GenerateAppointmentTemplateContainerId(int aptIndex) {
			return String.Format("{0}{1}{2}", AppointmentTemplateContainerId, aptIndex.ToString(), GetActiveViewAndGroupTypeValue());
		}
		public static string GenerateInternalCellTemplateContainerId() {
			return String.Format("{0}{1}", InternalCellTemplateContainerId, GetActiveViewAndGroupTypeValue());
		}
		static string GetActiveViewAndGroupTypeValue() {
			ASPxScheduler control = ASPxScheduler.ActiveControl;
			return String.Format("{0}{1}", (int)control.ActiveViewType, (int)control.GroupType);
		}
		public static object ConvertToResourceId(string resourceIdString, Type type) {
			if (EmptyResourceId.Equals(resourceIdString))
				return ResourceBase.Empty.Id;
			else {
				object result = DevExpress.Utils.Serializing.Helpers.ObjectConverter.StringToObject(resourceIdString, type);
				if (result == null)
					result = ResourceBase.Empty.Id;
				return result;
			}
		}
		public static Type GetResourceIdType(IResourceStorageBase resources) {
			return (resources != null && resources.Count > 0) ? resources[0].Id.GetType() : typeof(object);
		}
		public static string GenerateScriptResourceId(XtraScheduler.Resource resource) {
			return String.Format("\"{0}\"", GenerateResourceId(resource));
		}
		internal static string GenerateAnchorDivId(XtraScheduler.Resource resource, AnchorType anchorType) {
			string resId = GenerateResourceId(resource);
			return String.Format("anch{0}_{1}", resId, anchorType);
		}
		internal static string[] GenerateResourceIds(AppointmentResourceIdCollection resourceIds) {
			int count = resourceIds.Count;
			string[] result = new string[count];
			for (int i = 0; i < count; i++)
				result[i] = GenerateResourceId(resourceIds[i]);
			return result;
		}
		static string GenerateResourceId(object resourceId) {
			return resourceId != ResourceBase.Empty.Id ? HtmlConvertor.EscapeString(resourceId.ToString()) : EmptyResourceId;
		}
		internal static string GenerateCustomContainerId(string formContainerId) {
			return String.Format("Custom{0}", formContainerId);
		}
		public static string GenerateObjectId(object value) {
			return value != null ? value.ToString() : EmptyId;
		}
	}
	public static class SchedulerWebUtils { 
		static DateTime baseDate = new DateTime(1970, 1, 1);
		public static DateTime BaseDate { get { return baseDate; } }
		const string ValueOn = "1";
		const string ValueOff = "0";
		internal static string BoolToStr(bool val) {
			return val ? ValueOn : ValueOff; 
		}
		internal static bool BoolFromStr(string val) {
			return val == ValueOn ? true : false;
		}
		public static TimeInterval ToTimeInterval(string startMilliseconds, string durationMilliseconds) {
			return ToTimeInterval(Convert.ToInt64(startMilliseconds), Convert.ToInt64(durationMilliseconds));
		}
		public static TimeInterval ToTimeInterval(long startMilliseconds, long durationMilliseconds) {
			return new TimeInterval(baseDate.AddMilliseconds(startMilliseconds), TimeSpan.FromMilliseconds(durationMilliseconds));
		}
		public static TimeInterval ToTimeInterval(double startMilliseconds, double durationMilliseconds) {
			return new TimeInterval(baseDate.AddMilliseconds(startMilliseconds), TimeSpan.FromMilliseconds(durationMilliseconds));
		}
		public static DateTime ToDateTime(string milliseconds) {
			return baseDate.AddMilliseconds(Convert.ToInt64(milliseconds));
		}
		public static TimeSpan ToTimeSpan(string milliseconds) {
			return TimeSpan.FromMilliseconds(Convert.ToInt64(milliseconds));
		}
		public static string ToJavaScriptDate(DateTime date) {
			TimeSpan duration = date.Subtract(baseDate);
			return ToJavaScriptDurationString(duration);
		}
		public static string ToJavaScriptDurationString(TimeSpan duration) {
			return Convert.ToInt64(duration.TotalMilliseconds).ToString();
		}
		public static long ToJavaScriptDuration(TimeSpan duration) {
			return Convert.ToInt64(duration.TotalMilliseconds);
		}
		public static void SetPreventSelectionAttribute(WebControl control) {
			if (RenderUtils.Browser.IsFirefox)  
				control.Style.Add("-moz-user-select", "-moz-none");
			else
				RenderUtils.SetPreventSelectionAttribute(control);
		}
	}
	#region WebSerializationPropertyDescriptorModifier
	public class WebSerializationPropertyDescriptorModifier {
		public virtual PropertyDescriptor Modify(PropertyDescriptor pd) {
			System.ComponentModel.AttributeCollection attrs = pd.Attributes;
			int count = attrs.Count;
			for (int i = 0; i < count; i++) {
				XtraSerializableProperty xtraSerializablePropertyAttr = attrs[i] as XtraSerializableProperty;
				if (xtraSerializablePropertyAttr != null)
					return AppendWebSerializationAttribute(pd, xtraSerializablePropertyAttr);
			}
			return pd;
		}
		protected internal virtual PropertyDescriptor SuppressSerialization(PropertyDescriptor source) {
			DesignOnlyAttribute attr = new DesignOnlyAttribute(true);
			return new AddAttributesPropertyDescriptor(source, new Attribute[] { attr });
		}
		protected internal virtual PropertyDescriptor AppendWebSerializationAttribute(PropertyDescriptor source, XtraSerializableProperty attr) {
			if (attr.Visibility == XtraSerializationVisibility.Hidden)
				return source;
			PersistenceMode mode = ConvertToPersistenceMode(attr.Visibility);
			PersistenceModeAttribute pma = new PersistenceModeAttribute(mode);
			return new AddAttributesPropertyDescriptor(source, new Attribute[] { pma });
		}
		protected internal virtual PersistenceMode ConvertToPersistenceMode(XtraSerializationVisibility visibility) {
			switch (visibility) {
				case XtraSerializationVisibility.NameCollection:
				case XtraSerializationVisibility.SimpleCollection:
				case XtraSerializationVisibility.Collection:
				case XtraSerializationVisibility.Content:
					return PersistenceMode.InnerProperty;
				case XtraSerializationVisibility.Visible:
				default:
					return PersistenceMode.Attribute;
			}
		}
	}
	#endregion
	public class CompressedBase64XtraSerializer : BinaryXtraSerializer {
		public string Serialize(IXtraPropertyCollection properties) {
			using (MemoryStream memoryStream = new MemoryStream()) {
				Serialize(memoryStream, properties, "XtraScheduler");
				return Convert.ToBase64String(memoryStream.ToArray());
			}
		}
		public void Deserialize(object obj, string base64String) {
			byte[] bytes = Convert.FromBase64String(base64String);
			using (MemoryStream memoryStream = new MemoryStream(bytes)) {
				DeserializeObject(obj, memoryStream, "XtraScheduler");
			}
		}
	}
	public delegate string GetItemCaptionHandler(object item);
	#region FormTemplateContainerHelper
	public static class FormTemplateContainerHelper {
		public static IEnumerable CreateEmptyListEditData() {
			return new ListEditItemCollection();
		}
		public static IEnumerable CreateListEditData(IEnumerable list, GetItemCaptionHandler getCaptionHandler) {
			ListEditItemCollection collection = new ListEditItemCollection();
			int index = 0;
			foreach (object item in list) {
				IUserInterfaceObject uio = item as IUserInterfaceObject;
				string caption = getCaptionHandler(item);
				int id = index++;
				collection.Add(caption, (uio == null) ? id : uio.Id);
			}
			return collection;
		}
		public static IEnumerable CreateListEditData(ResourceBaseCollection resources) {
			ListEditItemCollection collection = new ListEditItemCollection();
			int count = resources.Count;
			for (int i = 0; i < count; i++) {
				XtraScheduler.Resource resource = resources[i];
				collection.Add(resource.Caption, resource.Id);
			}
			return collection;
		}
	}
	#endregion
	#region FormCallbackCommandHelper
	public class FormCallbackCommandHelper {
		string containerName;
		public FormCallbackCommandHelper(string containerName) {
			Guard.ArgumentIsNotNullOrEmpty(containerName, "containerName");
			this.containerName = containerName;
		}
		public string GetDefaultInputPath(string inputName, char idSeparator) {
			return this.containerName + idSeparator + inputName;
		}
	}
	#endregion
	public static class ExceptionHelper {
		static string NewLinesToBr(string text) {
			text = text.Replace("\r", string.Empty);
			return text.Replace("\n", "<br/>");
		}
		public static Exception PrepareDetailedExceptionMessageAsHtml(ASPxScheduler control, Exception ex, bool showDetailedErrorInfo) {
			string message = GetDetailedExceptionMessageAsHtml(control, ex, showDetailedErrorInfo);
			return new Exception(message, ex);
		}
		public static string GetDetailedExceptionMessageAsHtml(ASPxScheduler control, Exception ex, bool showDetailedErrorInfo) {
			string message = PrepareDefaultErrorMessage(control, ex, showDetailedErrorInfo);
			try {
				ASPxSchedulerCustomErrorTextEventArgs args = new ASPxSchedulerCustomErrorTextEventArgs(ex, message);
				control.RaiseCustomErrorText(args);
				message = args.ErrorText;
			} catch {
			}
			return message;
		}
		static string PrepareDefaultErrorMessage(ASPxScheduler control, Exception ex, bool showDetailedErrorInfo) {
			showDetailedErrorInfo &= !HttpUtils.IsCustomErrorEnabled();
			string subject = String.Format("{0}\n", control.GetCallbackErrorMessage(ex));
			string detailInfo = String.Format("An exception of the {0} type has been thrown\nSource: {1}\n\nStack:\n{2}", ex.GetType(), ex.Source, ex.StackTrace);
			if (!showDetailedErrorInfo)
				detailInfo = String.Empty;
			subject = NewLinesToBr(HttpUtility.HtmlEncode(subject));
			detailInfo = NewLinesToBr(HttpUtility.HtmlEncode(detailInfo));
			return String.Format("{0},{1}|{2}{3}", subject.Length, detailInfo.Length, subject, detailInfo);
		}
	}	   
	public enum StatusInfoType {
		Info = 0,
		Warning = 1,
		Error = 2,
		Exception = 3,
	}
	public class SchedulerStatusInfo {
		string subject;
		string detail;
		StatusInfoType type;
		public SchedulerStatusInfo(StatusInfoType type, string subject)
			: this(type, subject, String.Empty) {
		}
		public SchedulerStatusInfo(StatusInfoType type, string subject, string detail) {
			this.type = type;
			this.subject = HtmlConvertor.EscapeString(subject);
			this.detail = HtmlConvertor.EscapeString(detail);
		}
		public StatusInfoType InfoType { get { return type; } }
		public string Subject { get { return subject; } }
		public string Detail { get { return detail; } }
	}
	public class SchedulerStatusInfoCollection : List<SchedulerStatusInfo> {
	}
	public static class SchedulerStatusInfoHelper {
		static SchedulerStatusInfoCollection GetSchedulerStatusInfos(ASPxScheduler scheduler) {
			if (!HasStatusInfo(scheduler))
				AddSchedulerStatusInfos(scheduler, new SchedulerStatusInfoCollection());
			return GetSchedulerStatusInfosCore(scheduler);
		}
		public static void AddStatusInfo(ASPxScheduler scheduler, SchedulerStatusInfo info) {
			GetSchedulerStatusInfos(scheduler).Add(info);
		}
		static int SchedulerStatusInfoComparsion(SchedulerStatusInfo info1, SchedulerStatusInfo info2) {
			return (int)info2.InfoType - (int)info1.InfoType;
		}
		public static SchedulerStatusInfoCollection GetSortedSchedulerStatusInfos(ASPxScheduler scheduler) {
			if (!HasStatusInfo(scheduler))
				return null;
			SchedulerStatusInfoCollection infos = GetSchedulerStatusInfosCore(scheduler);
			infos.Sort(SchedulerStatusInfoComparsion);
			return infos;
		}
		internal static bool HasStatusInfo(ASPxScheduler scheduler) {
			HttpContext currentContext = HttpContext.Current;
			if (currentContext != null)
				return currentContext.Items.Contains(scheduler);
			else
				return false;
		}
		internal static SchedulerStatusInfoCollection GetSchedulerStatusInfosCore(ASPxScheduler scheduler) {
			HttpContext currentContext = HttpContext.Current;
			if (currentContext != null)
				return (SchedulerStatusInfoCollection)currentContext.Items[scheduler];
			else
				return new SchedulerStatusInfoCollection();
		}
		internal static void AddSchedulerStatusInfos(ASPxScheduler scheduler, SchedulerStatusInfoCollection infos) {
			HttpContext currentContext = HttpContext.Current;
			if (currentContext != null)
				currentContext.Items.Add(scheduler, infos);
		}
		public static void AddError(ASPxScheduler control, string subject) {
			AddStatusInfo(control, new SchedulerStatusInfo(StatusInfoType.Error, subject));
		}
		internal static void AddError(ASPxScheduler control, Exception exception) {
			AddError(control, String.Empty, exception);
		}
		internal static void AddError(ASPxScheduler control, string mainSubject, Exception exception) {
			string exceptionText = exception.Message;
			int separatorIndex = exceptionText.IndexOf('|');
			if (separatorIndex < 0)
				return;
			string[] textLengths = exceptionText.Substring(0, separatorIndex).Split(',');
			if (textLengths.Length != 2)
				return;
			int subjectLength;
			int detailLength;
			if (!Int32.TryParse(textLengths[0], out subjectLength) || !Int32.TryParse(textLengths[1], out detailLength))
				return;
			string messageText = exceptionText.Substring(separatorIndex + 1);
			string subject = messageText.Substring(0, subjectLength);
			if (!String.IsNullOrEmpty(mainSubject))
				subject = String.Format("{0}. {1}", mainSubject, subject);
			string detail = messageText.Substring(subjectLength, detailLength);
			AddError(control, subject, detail);
		}
		public static void AddError(ASPxScheduler control, string subject, string detail) {
			AddStatusInfo(control, new SchedulerStatusInfo(StatusInfoType.Error, subject, detail));
		}
		public static void AddWarning(ASPxScheduler control, string subject) {
			AddStatusInfo(control, new SchedulerStatusInfo(StatusInfoType.Warning, subject));
		}
		public static void AddWarning(ASPxScheduler control, string subject, string detail) {
			AddStatusInfo(control, new SchedulerStatusInfo(StatusInfoType.Warning, subject, detail));
		}
		public static void AddInfo(ASPxScheduler control, string subject) {
			AddStatusInfo(control, new SchedulerStatusInfo(StatusInfoType.Info, subject));
		}
		public static void AddInfo(ASPxScheduler control, string subject, string detail) {
			AddStatusInfo(control, new SchedulerStatusInfo(StatusInfoType.Info, subject, detail));
		}
	}
	#region SchedulerEditableAppointmentInfo
	public class SchedulerEditableAppointmentInfo : IDisposable {
		#region Fields
		string id;
		AppointmentType type = AppointmentType.Normal;
		Appointment newInstance;
		bool isDisposed;
		Appointment instance;
		IDefaultUserData defaultUserData;
		#endregion
		#region Constructor
		public SchedulerEditableAppointmentInfo() {
			Reset();
		}
		#endregion
		#region Properties
		public string Id { get { return id; } }
		public AppointmentType Type { get { return type; } }
		public bool IsNewlyCreated { get { return Id == SchedulerIdHelper.NewAppointmentId; } }
		internal bool IsDisposed { get { return isDisposed; } }
		internal Appointment Instance { get { return instance; } }
		protected internal Appointment NewInstance { get { return newInstance; } }
		#endregion
		#region IDisposable Members
		public void Dispose() {
			try {
				type = AppointmentType.Normal;
				id = null;
				if (newInstance != null) {
					newInstance.Dispose();
					newInstance = null;
				}
			} finally {
				isDisposed = true;
			}
		}
		#endregion
		protected internal void Reset() {
			this.newInstance = null;
			this.id = null;
			this.type = AppointmentType.Normal;
			this.defaultUserData = null;
		}
		public void SetNewInstance(Appointment apt) {
			if (apt == null) {
				Reset();
				return;
			}
			XtraSchedulerDebug.Assert(apt.IsBase);
			this.newInstance = apt;
			this.type = apt.Type;
			this.id = SchedulerIdHelper.NewAppointmentId;
			this.instance = null;
			this.defaultUserData = null;
		}
		public void SetExistingReference(AppointmentType type, string clientId) {
			this.id = clientId;
			this.type = type;
			this.newInstance = null;
			this.instance = null;
			this.defaultUserData = null;
		}
		public void SetExistingReference(Appointment apt) {
			this.id = AppointmentSearchHelper.GetAppointmentClientId(apt);
			this.type = apt.Type;
			this.newInstance = null;
			this.instance = apt;
			this.defaultUserData = null;
		}
		public void SetExistingReference(Appointment apt, IDefaultUserData userData) {
			this.id = AppointmentSearchHelper.GetAppointmentClientId(apt);
			this.type = apt.Type;
			this.newInstance = null;
			this.instance = apt;
			this.defaultUserData = userData;
		}
		public IDefaultUserData GetDefaultUserData(ASPxScheduler control) {
			return this.defaultUserData;
		}
		public Appointment GetAppointment(ASPxScheduler control) {
			if (IsNewlyCreated)
				return (NewInstance != null) ? NewInstance : control.Storage.CreateAppointment(Type);
			if (!IsInstaceValid() && !String.IsNullOrEmpty(Id))
				return control.LookupAppointmentByIdString(Id);
			if (IsInstaceValid())
				return Instance;
			return null;
		}
		bool IsInstaceValid() {
			return Instance != null && !Instance.IsDisposed;
		}
	}
	#endregion
	#region AppointmentSearchHelper
	public class AppointmentSearchHelper {
		#region Feilds
		SchedulerViewBase view;
		IAppointmentStorageBase storage;
		#endregion
		#region Constructor
		#region Old Constructor
		#endregion
		public AppointmentSearchHelper(SchedulerViewBase view, IAppointmentStorageBase storage) {
			this.view = view;
			this.storage = storage;
		}
		#endregion
		#region Properties
		public SchedulerViewBase View { get { return view; } }
		public IAppointmentStorageBase Storage { get { return storage; } }
		#endregion
		#region FindAppointmentByClientId
		public Appointment FindAppointmentByClientId(string appointmentIdString) {
			if (String.IsNullOrEmpty(appointmentIdString))
				return null;
			Appointment result = null;
			if (View != null)
				result = FindInViewFilteredAppointments(appointmentIdString, true);
			if (result != null)
				return result;
			if (Storage != null)
				return FindInAppointmentStorageByClientIdString(appointmentIdString);
			return null;
		}
		#endregion
		#region FindAppointmentByServerId
		public Appointment FindAppointmentByServerId(string appointmentIdString) {
			if (String.IsNullOrEmpty(appointmentIdString))
				return null;
			Appointment result = null;
			if (View != null)
				result = FindInViewFilteredAppointments(appointmentIdString, false);
			if (result != null)
				return result;
			if (Storage != null)
				return FindInAppointmentStorageByServerIdString(appointmentIdString);
			return null;
		}
		#endregion
		#region FindInViewFilteredAppointments
		protected internal virtual Appointment FindInViewFilteredAppointments(string appointmentIdString, bool isClientId) {
			AppointmentBaseCollection appointments = View.FilteredAppointments;
			int count = appointments.Count;
			for (int i = 0; i < count; i++) {
				Appointment appointment = appointments[i];
				if (GetAppointmentIdString(appointment, isClientId) == appointmentIdString)
					return appointment;
				if (appointment.IsRecurring) {
					Appointment pattern = appointment.RecurrencePattern;
					if (GetAppointmentIdString(pattern, isClientId) == appointmentIdString)
						return pattern;
				}
			}
			return null;
		}
		#endregion
		#region FindInAppointmentStorageByClientIdString
		protected internal virtual Appointment FindInAppointmentStorageByClientIdString(string appointmentIdString) {
			AppointmentBaseCollection appointments = storage.Items;
			string patternIdString = String.Empty;
			int occurrenceNumber = 0;
			bool isRecurrenceId = TryParseRecurrenceAppointmentIdString(appointmentIdString, out patternIdString, out occurrenceNumber);
			int count = appointments.Count;
			for (int i = 0; i < count; i++) {
				Appointment appointment = appointments[i];
				if (GetAppointmentIdString(appointment, true) == appointmentIdString)
					return appointment;
				if (appointment.Type == AppointmentType.Pattern && isRecurrenceId) {
					if (GetAppointmentIdString(appointment, true) == patternIdString)
						return appointment.GetOccurrence(occurrenceNumber);
				}
			}
			return null;
		}
		#endregion
		#region FindInAppointmentStorageByServerIdString
		protected internal virtual Appointment FindInAppointmentStorageByServerIdString(string appointmentIdString) {
			AppointmentBaseCollection appointments = storage.Items;
			return FindInAppointmentStorageByServerIdStringCore(appointmentIdString, appointments);
		}
		#endregion
		#region FindInAppointmentStorageByServerIdStringCore
		protected internal virtual Appointment FindInAppointmentStorageByServerIdStringCore(string appointmentIdString, AppointmentBaseCollection appointments) {
			int count = appointments.Count;
			for (int i = 0; i < count; i++) {
				Appointment appointment = appointments[i];
				if (appointment.Id == null)
					continue;
				if (GetAppointmentIdString(appointment, false) == appointmentIdString)
					return appointment;
				if (appointment.Type == AppointmentType.Pattern) {
					Appointment result = FindInAppointmentStorageByServerIdStringCore(appointmentIdString, appointment.GetExceptions());
					if (result != null)
						return result;
				}
			}
			return null;
		}
		#endregion
		#region TryParseRecurrenceAppointmentIdString
		internal bool TryParseRecurrenceAppointmentIdString(string appointmentIdString, out string patternId, out int occurrence) {
			patternId = String.Empty;
			occurrence = 0;
			Match r = Regex.Match(appointmentIdString, @"^(.+)_(\d+)$");
			if (!r.Success)
				return false;
			patternId = r.Groups[1].Value;
			occurrence = Int32.Parse(r.Groups[2].Value);
			return true;
		}
		#endregion
		#region GetAppointmentClientId
		internal static string GetAppointmentClientId(Appointment appointment) {
			return GetAppointmentIdString(appointment, true);
		}
		#endregion
		#region GetAppointmentIdString
		internal static string GetAppointmentIdString(Appointment appointment, bool isClientId) {
			if (isClientId && (appointment.IsException || appointment.IsOccurrence)) {
				if (appointment.RecurrencePattern == null)
					return appointment.GetHashCode().ToString();
				return String.Format("{0}_{1}", appointment.RecurrencePattern.Id, appointment.RecurrenceIndex);
			}
			return appointment.Id == null ? String.Empty : appointment.Id.ToString();
		}
		#endregion
	}
	#endregion
	public static class StringArgumentsHelper {
		const string MasterSlaveArgumentSeparator = "|";
		public static bool IsMasterSlavePair(string pair) {
			return pair.Contains(MasterSlaveArgumentSeparator);
		}
		public static string[] SplitMasterSlaveArgumentPair(string argumentsString) {
			string[] result = new string[2] { String.Empty, String.Empty };
			if (String.IsNullOrEmpty(argumentsString))
				return result;
			int separatorIndex = argumentsString.IndexOf(MasterSlaveArgumentSeparator);
			if (separatorIndex == -1) {
				result[0] = argumentsString;
			} else {
				result[0] = argumentsString.Substring(0, separatorIndex);
				result[1] = argumentsString.Substring(separatorIndex + 1);
			}
			return result;
		}
	}
	public static class DesignTimeDateHeaderFormatter {
		public static string FormatHeaderDate(WebDateHeader header) {
			StringCollection formats = DateTimeFormatHelper.GenerateFormatsWithoutYear();
			if (formats.Count > 0)
				return header.Interval.Start.ToString(formats[0]);
			return String.Empty;
		}
		public static string FormatMonthHeaderDate(WebDateHeader header) {
			DateTime date = header.Interval.Start;
			if (IsLongFormatHeaderCaption(header)) {
				if (IsNewYearFormatHeaderCaption(header)) {
					return DateTimeFormatHelper.DateToStringForNewYear(date);
				} else {
					return DateTimeFormatHelper.DateToStringWithoutYearAndWeekDay(date);
				}
			} else
				return date.Day.ToString();
		}
		static bool IsLongFormatHeaderCaption(WebDateHeader header) {
			return header.FirstVisible || header.Interval.Start.Date.Day == 1;
		}
		static bool IsNewYearFormatHeaderCaption(WebDateHeader header) {
			DateTime date = header.Interval.Start.Date;
			return date.Day == 1 && date.Month == 1;
		}
	}
	public class OccurrenceIdProxy {
		object id;
		int occurrenceIndex;
		public OccurrenceIdProxy(Appointment occurrence) {
			System.Diagnostics.Debug.Assert(occurrence.Type == AppointmentType.Occurrence || occurrence.Type == AppointmentType.DeletedOccurrence || occurrence.Type == AppointmentType.ChangedOccurrence);
			if (occurrence.RecurrencePattern == null)
				return;
			this.id = occurrence.RecurrencePattern.Id;
			this.occurrenceIndex = occurrence.RecurrenceIndex;
		}
		public object Id { get { return id; } }
		public int OccurrenceIndex { get { return occurrenceIndex; } }
	}
	public class AppointmentSelectionKeeper {
		InnerSchedulerControl innerControl;
		List<Object> selectedAppointmentIds;
		bool isSaved;
		public AppointmentSelectionKeeper(InnerSchedulerControl innerControl) {
			Guard.ArgumentNotNull(innerControl, "innerControl");
			this.innerControl = innerControl;
			this.selectedAppointmentIds = new List<object>();
		}
		public bool IsSaved {
			get { return isSaved; }
		}
		public void SaveSelection() {
			this.selectedAppointmentIds.Clear();
			AppointmentBaseCollection selectedAppointments = this.innerControl.SelectedAppointments;
			int count = selectedAppointments.Count;
			for (int i = 0; i < count; i++) {
				if (selectedAppointments[i].Type == AppointmentType.Occurrence || selectedAppointments[i].Type == AppointmentType.DeletedOccurrence || selectedAppointments[i].Type == AppointmentType.ChangedOccurrence) {
					this.selectedAppointmentIds.Add(new OccurrenceIdProxy(selectedAppointments[i]));
				} else
					this.selectedAppointmentIds.Add(selectedAppointments[i].Id);
			}
			this.isSaved = true;
		}
		public void RestoreSelection() {
			int appointmentIdsCount = this.selectedAppointmentIds.Count;
			for (int i = 0; i < appointmentIdsCount; i++) {
				object aptId = this.selectedAppointmentIds[i];
				if (aptId == null)
					continue;
				Appointment apt = null;
				OccurrenceIdProxy idProxy = aptId as OccurrenceIdProxy;
				if (idProxy != null) {
					Appointment pattern = this.innerControl.Storage.Appointments.GetAppointmentById(idProxy.Id);
					apt = pattern.GetOccurrence(idProxy.OccurrenceIndex);
				} else
					apt = this.innerControl.Storage.Appointments.GetAppointmentById(aptId);
				if (apt == null)
					continue;
				this.innerControl.SelectedAppointments.Add(apt);
			}
		}
		public void Reset() {
			this.isSaved = false;
		}
	}
	public class SchedulerDataBoundLocker {
		int locked;
		bool deferredOnDataBound;
		ASPxScheduler scheduler;
		public SchedulerDataBoundLocker(ASPxScheduler scheduler) {
			Guard.ArgumentNotNull(scheduler, "scheduler");
			this.scheduler = scheduler;
			this.locked = 0;
		}
		public bool Locked { get { return locked > 0; } }
		public void BeginUpdate() {
			Lock();
			deferredOnDataBound = false;
		}
		public void EndUpdate() {
			Unlock();
			if (Locked)
				return;
			if (this.deferredOnDataBound)
				this.scheduler.ProtectedRaiseDataBound();
		}
		public void SetOnDataBoundRaised() {
			this.deferredOnDataBound = true;
		}
		void Lock() {
			this.locked++;
		}
		void Unlock() {
			if (Locked)
				this.locked--;
		}
	}
	public class ASPxSchedulerPropertyChangeTracker {
		public ASPxSchedulerPropertyChangeTracker(ASPxScheduler scheduler) {
			Scheduler = scheduler;
		}
		ASPxScheduler Scheduler { get; set; }
		TimeIntervalCollection SavedVisibleInterval { get; set; }
		SchedulerViewType SavedActiveViewType { get; set; }
		public void BeginTrack() {
			SavedVisibleInterval = Scheduler.ActiveView.GetVisibleIntervals();
			SavedActiveViewType = Scheduler.ActiveViewType;
		}
		public bool IsVisibleIntervalChanged() {
			if (SavedVisibleInterval == null)
				return false;
			return SavedActiveViewType != Scheduler.ActiveViewType || !SavedVisibleInterval.Interval.Equals(Scheduler.ActiveView.GetVisibleIntervals().Interval);
		}
	}
	public static class ObjectToByteArrayConverter {
		public static byte[] ObjectToByteArray(object value) {
			BinaryFormatter bf = new BinaryFormatter();
			using (MemoryStream ms = new MemoryStream()) {
				bf.Serialize(ms, value);
				return ms.ToArray();
			}
		}
		public static object ByteArrayToObject(byte[] bytes) {
			using (MemoryStream memStream = new MemoryStream()) {
				BinaryFormatter bf = new BinaryFormatter();
				memStream.Write(bytes, 0, bytes.Length);
				memStream.Position = 0;
				return bf.Deserialize(memStream);
			}
		}
	}
}
