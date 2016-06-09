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

using DevExpress.Data.Mask;
using DevExpress.Utils;
using DevExpress.Utils.Design;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Drawing;
using DevExpress.XtraEditors.Mask;
using DevExpress.XtraEditors.Popup;
using DevExpress.XtraEditors.Popups;
using DevExpress.XtraEditors.Registrator;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraEditors.ViewInfo;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Globalization;
using System.Windows.Forms;
using System.Linq;
using DevExpress.XtraEditors.ToolboxIcons;
namespace DevExpress.XtraEditors {
	[DXToolboxItem(DXToolboxItemKind.Free),
	 Designer("DevExpress.XtraEditors.Design.TimeSpanEditDesigner, " + AssemblyInfo.SRAssemblyEditorsDesign),
	 Description("Allows an end-user to edit time span values."),
	 ToolboxTabName(AssemblyInfo.DXTabCommon),
	 SmartTagFilter(typeof(TimeSpanEditFilter)),
	ToolboxBitmap(typeof(ToolboxIconsRootNS), "TimeSpanEdit")
	]
	public class TimeSpanEdit : TimeEdit {
		static TimeSpanEdit() { RepositoryItemTimeSpanEdit.RegisterTimeSpanEdit(); }
		public TimeSpanEdit() : base() { }
		protected override void InitEditValue() {
			this.fOldEditValue = this.fEditValue = new TimeSpan(0);
		}
		public override string EditorTypeName { get { return RepositoryItemTimeSpanEdit.TimeSpanEditName; } }
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("TimeSpanEditProperties"),
#endif
 DXCategory(CategoryName.Properties), DesignerSerializationVisibility(DesignerSerializationVisibility.Content), SmartTagSearchNestedProperties]
		public new RepositoryItemTimeSpanEdit Properties { get { return base.Properties as RepositoryItemTimeSpanEdit; } }
		protected override PopupBaseForm CreatePopupForm() {
			return new TimeSpanEditDropDownForm(this);
		}
		protected internal override void SetEmptyEditValue(object emptyEditValue) {
			base.SetEmptyEditValue(new TimeSpan(0));
		}
		protected override void OnPopupClosed(PopupCloseMode closeMode) {
			base.OnPopupClosed(closeMode);
			DestroyPopupForm();
		}
		protected internal override MaskManager CreateMaskManager(MaskProperties mask) {
			TimeSpanMaskProperties timeSpanMask = new TimeSpanMaskProperties(this);
			timeSpanMask.Assign(mask);
			timeSpanMask.EditMask = Properties.GetFormatMaskAccessFunction(mask.EditMask, mask.Culture);
			return timeSpanMask.CreateTimeSpanMaskManager(mask);
		}
		public override object EditValue {
			get { return TimeSpanParser.ObjectToTimeSpan(base.EditValue, Properties); }
			set { base.EditValue = TimeSpanParser.ObjectToTimeSpan(value, Properties); }
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never)]
		public override DateTime Time { get { return base.Time; } set { } }
		[DXCategory(CategoryName.Appearance), 
#if !SL
	DevExpressXtraEditorsLocalizedDescription("TimeSpanEditTimeSpan"),
#endif
 DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public TimeSpan TimeSpan { 
			get { return TimeSpanParser.ObjectToTimeSpan(EditValue, Properties) ?? new TimeSpan(0); } 
			set { EditValue = value; } }
		protected override TextBoxMaskBox CreateMaskBoxInstance() {
			return new TimeSpanEditMaskBox(this);
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		protected virtual internal void ResetMaskBox(){
			CreateMaskBox();
			Refresh();
		}
		protected internal override void FlushPendingEditActions() {
			if(!MaskBox.Focused)
				base.FlushPendingEditActions();
		}
	}
	public class TimeSpanEditMaskBox : TextBoxMaskBox {
		public TimeSpanEditMaskBox(TimeSpanEdit owner) : base(owner) { }
	}
	public class TimeSpanMaskProperties : TimeEditMaskProperties {
		public TimeSpanMaskProperties(TimeSpanEdit editor) : base() {
			this.editor = editor;
		}
		TimeSpanEdit editor;
		public virtual MaskManager CreateTimeSpanMaskManager(MaskProperties mask) {
			CultureInfo managerCultureInfo = this.Culture;
			if(managerCultureInfo == null) managerCultureInfo = CultureInfo.CurrentCulture;
			string editMask = this.EditMask;
			if(editMask == null) editMask = string.Empty;
			return CreateTimeSpanMaskManagerCore(editor, mask, editMask, managerCultureInfo);
		}
		protected virtual MaskManager CreateTimeSpanMaskManagerCore(TimeSpanEdit editor, MaskProperties mask, string editorMask, CultureInfo ci) {
			switch(mask.MaskType) {
				case MaskType.DateTimeAdvancingCaret: return new TimeSpanMaskManager(editor, editorMask, true, ci, true);
			}
			return new TimeSpanMaskManager(editor, editorMask, false, ci, true);
		}
	}
	public class TimeSpanMaskManager : DateTimeMaskManager {
		public TimeSpanMaskManager(TimeSpanEdit editor, string mask, bool isOperatorMask, CultureInfo culture, bool allowNull)
			: base(new TimeSpanMaskManagerCore(editor, mask, isOperatorMask, culture, allowNull)) {
		}
	}
	public class TimeSpanMaskManagerCore : DateTimeMaskManagerCore {
		public TimeSpanMaskManagerCore(TimeSpanEdit editor, string mask, bool isOperatorMask, CultureInfo culture, bool allowNull)
			: base(mask, isOperatorMask, culture, allowNull) {
			fFormatInfo = new TimeSpanMaskFormatInfo(editor, mask, this.fInitialDateTimeFormatInfo);
		}
	}
	public class TimeSpanMaskFormatInfo : DateTimeMaskFormatInfo {
		TimeSpanEdit editor;
		DateTimeFormatInfo dateTimeFormatInfo;
		public TimeSpanMaskFormatInfo(TimeSpanEdit editor, string mask, DateTimeFormatInfo dateTimeFormatInfo) : base(mask, dateTimeFormatInfo) {
			this.editor = editor;
			this.dateTimeFormatInfo = dateTimeFormatInfo;
			CreateTimeSpanMaskFormatElements();
		}
		protected virtual void CreateTimeSpanMaskFormatElements() {
			for(int i = 0; i < Count; i++) {
				innerList[i] = CreateTimeSpanMaskFormatElement(innerList[i]);
			}
		}
		protected virtual DateTimeMaskFormatElement CreateTimeSpanMaskFormatElement(DateTimeMaskFormatElement element) {
			if(element is DateTimeMaskFormatElement_d) 
				return new TimeSpanMaskFormatElement_d("d", dateTimeFormatInfo, editor);
			if(element is DateTimeMaskFormatElement_H24 || element is DateTimeMaskFormatElement_h12) 
				return new TimeSpanMaskFormatElement_H24("H", dateTimeFormatInfo, editor);
			if(element is DateTimeMaskFormatElement_Min) 
				return new TimeSpanMaskFormatElement_Min("m", dateTimeFormatInfo, editor);
			if(element is DateTimeMaskFormatElement_s) 
				return new TimeSpanMaskFormatElement_s("s", dateTimeFormatInfo, editor);
			return element;
		}
	}
	internal abstract class TimeSpanNumericRangeFormatElementEditable : DateTimeNumericRangeFormatElementEditable {
		TimeSpanEdit ownerEdit;
		DateTimeFormatInfo formatInfo;
		string mask;
		public TimeSpanNumericRangeFormatElementEditable(string mask, DateTimeFormatInfo formatInfo, TimeSpanEdit ownerEdit) : base(mask, formatInfo, DateTimePart.None) {
			this.ownerEdit = ownerEdit;
			this.formatInfo = formatInfo;
			this.mask = mask;
		}
		public DateTimeFormatInfo FormatInfo { get { return formatInfo; } }
		public TimeSpanEdit OwnerEdit { get { return ownerEdit; } }
		protected virtual string FormatString { get { return OwnerEdit.Properties.EditFormat.FormatString.ToLower(); } }
		protected SeniorFormat SeniorFormat {
			get {
				if(!string.IsNullOrEmpty(FormatString)) return TimeSpanParser.GetSeniorFormat(FormatString);
				if(OwnerEdit.Properties.IsSeniorDay) return SeniorFormat.Days;
				if(OwnerEdit.Properties.IsSeniorHour) return SeniorFormat.Hours;
				if(OwnerEdit.Properties.IsSeniorMinute) return SeniorFormat.Minutes;
				if(OwnerEdit.Properties.IsSeniorSecond) return SeniorFormat.Seconds;
				return SeniorFormat.None;
			}
		}
		protected virtual int GetDaysCore(DateTime value) { return new TimeSpan(value.Ticks).Days; }
		protected virtual int GetHoursCore(DateTime value) { return new TimeSpan(value.Ticks).Hours; }
		protected virtual int GetMinutesCore(DateTime value) { return new TimeSpan(value.Ticks).Minutes; }
		protected virtual int GetSecondsCore(DateTime value) { return new TimeSpan(value.Ticks).Seconds; }
		protected virtual int GetTotalDaysCore(DateTime value) { return (int)Math.Floor(new TimeSpan(value.Ticks).TotalDays); }
		protected virtual int GetTotalHoursCore(DateTime value) { return (int)Math.Floor(new TimeSpan(value.Ticks).TotalHours); }
		protected virtual int GetTotalMinutesCore(DateTime value) { return (int)Math.Floor(new TimeSpan(value.Ticks).TotalMinutes); }
		protected virtual int GetTotalSecondsCore(DateTime value) { return (int)Math.Floor(new TimeSpan(value.Ticks).TotalSeconds); }
		protected virtual int GetDays(DateTime value) {
			if(SeniorFormat < SeniorFormat.Days) return 0;
			return GetDaysCore(value); 
		}
		protected virtual int GetHours(DateTime value) {
			if(SeniorFormat < SeniorFormat.Hours) return 0;
			return SeniorFormat == SeniorFormat.Hours ? GetTotalHoursCore(value) : GetHoursCore(value); 
		}
		protected virtual int GetMinutes(DateTime value) {
			if(SeniorFormat < SeniorFormat.Minutes) return 0;
			return SeniorFormat == SeniorFormat.Minutes ? GetTotalMinutesCore(value) : GetMinutesCore(value); 
		}
		protected virtual int GetSeconds(DateTime value) {
			if(SeniorFormat < SeniorFormat.Seconds) return 0;
			return SeniorFormat == SeniorFormat.Seconds ? GetTotalSecondsCore(value) : GetSecondsCore(value); 
		}
		public override DateTime ApplyElement(int result, DateTime editedDateTime) { 
			int days = TimeSpanParser.GetSeniorFormat(mask) == SeniorFormat.Days ? result : GetDays(editedDateTime);
			int hours = TimeSpanParser.GetSeniorFormat(mask) == SeniorFormat.Hours ? result : GetHours(editedDateTime);
			int minutes = TimeSpanParser.GetSeniorFormat(mask) == SeniorFormat.Minutes ? result : GetMinutes(editedDateTime);
			int seconds = TimeSpanParser.GetSeniorFormat(mask) == SeniorFormat.Seconds ? result : GetSeconds(editedDateTime);
			return new DateTime(new TimeSpan(days, hours, minutes, seconds).Ticks);
		}
	}
	internal class TimeSpanMaskFormatElement_d : TimeSpanNumericRangeFormatElementEditable {
		public TimeSpanMaskFormatElement_d(string mask, DateTimeFormatInfo dateTimeFormatInfo, TimeSpanEdit editor) : base(mask, dateTimeFormatInfo, editor) { }
		public override DateTimeElementEditor CreateElementEditor(DateTime editedDateTime) {
			return new DateTimeNumericRangeElementEditor(GetDays(editedDateTime), 0, OwnerEdit.Properties.MaxDays - 1, 2, 9);
		}
		public override string Format(DateTime formattedDateTime) {
			return string.Format("{0:00}", GetDays(formattedDateTime));
		}
	}
	internal class TimeSpanMaskFormatElement_H24 : TimeSpanNumericRangeFormatElementEditable {
		public TimeSpanMaskFormatElement_H24(string mask, DateTimeFormatInfo dateTimeFormatInfo, TimeSpanEdit editor) : base(mask, dateTimeFormatInfo, editor) { }
		public override DateTimeElementEditor CreateElementEditor(DateTime editedDateTime) {
			return new DateTimeNumericRangeElementEditor(GetHours(editedDateTime), 0, OwnerEdit.Properties.MaxHours - 1, 2, 9);
		}
		public override string Format(DateTime formattedDateTime) {
			return string.Format("{0:00}", GetHours(formattedDateTime));
		}
	}
	internal class TimeSpanMaskFormatElement_Min : TimeSpanNumericRangeFormatElementEditable {
		public TimeSpanMaskFormatElement_Min(string mask, DateTimeFormatInfo dateTimeFormatInfo, TimeSpanEdit editor) : base(mask, dateTimeFormatInfo, editor) { }
		public override DateTimeElementEditor CreateElementEditor(DateTime editedDateTime) {
			return new DateTimeNumericRangeElementEditor(GetMinutes(editedDateTime), 0, OwnerEdit.Properties.MaxMinutes - 1, 2, 9);
		}
		public override string Format(DateTime formattedDateTime) {
			return string.Format("{0:00}", GetMinutes(formattedDateTime));
		}
	}
	internal class TimeSpanMaskFormatElement_s : TimeSpanNumericRangeFormatElementEditable {
		public TimeSpanMaskFormatElement_s(string mask, DateTimeFormatInfo dateTimeFormatInfo, TimeSpanEdit editor) : base(mask, dateTimeFormatInfo, editor) { }
		public override DateTimeElementEditor CreateElementEditor(DateTime editedDateTime) {
			return new DateTimeNumericRangeElementEditor(GetSeconds(editedDateTime), 0, OwnerEdit.Properties.MaxSeconds - 1, 2, 9);
		}
		public override string Format(DateTime formattedDateTime) {
			return string.Format("{0:00}", GetSeconds(formattedDateTime));
		}
	}
	#region TimeSpanParser
	internal static class TimeSpanParser {
		internal static string dId = Guid.NewGuid().ToString();
		internal static string hId = Guid.NewGuid().ToString();
		internal static string mId = Guid.NewGuid().ToString();
		internal static string sId = Guid.NewGuid().ToString();
		internal static List<string> delimiterFormat = new List<string>() { "{", "}" };
		public static string GetDisplayText(object editValue, string format, RepositoryItemTimeSpanEdit properties) {
			TimeSpan? s = ObjectToTimeSpan(editValue, properties);
			if(s == null || properties.IsNullValue(editValue)) return string.Empty;
			TimeSpan span = (TimeSpan)s;
			string f = format;
			while(f.Any(x => delimiterFormat.Contains(x.ToString()))) {
				f = Replace(f);
			}
			SeniorFormat senior = GetSeniorFormat(format);
			f = f.Replace(dId, FormatValue(format, SeniorFormat.Days, senior == SeniorFormat.Days ? Math.Floor(span.TotalDays) : span.Days));
			f = f.Replace(hId, FormatValue(format, SeniorFormat.Hours, senior == SeniorFormat.Hours ? Math.Floor(span.TotalHours) : span.Hours));
			f = f.Replace(mId, FormatValue(format, SeniorFormat.Minutes, senior == SeniorFormat.Minutes ? Math.Floor(span.TotalMinutes) : span.Minutes));
			f = f.Replace(sId, FormatValue(format, SeniorFormat.Seconds, senior == SeniorFormat.Seconds ? Math.Floor(span.TotalSeconds) : span.Seconds));
			return f;
		}
		static string FormatValue(string format, SeniorFormat current, double value) {
			return string.Format(CalcFormat(format, current), value);
		}
		static string CalcFormat(string format, SeniorFormat current) {
			string f = "{0:";
			if(current == SeniorFormat.Days) return CalcFormatCore(f, CalcDigitCount(format, 'd'));
			if(current == SeniorFormat.Hours) return CalcFormatCore(f, CalcDigitCount(format, 'h'));
			if(current == SeniorFormat.Minutes) return CalcFormatCore(f, CalcDigitCount(format, 'm'));
			if(current == SeniorFormat.Seconds) return CalcFormatCore(f, CalcDigitCount(format, 's'));
			return string.Empty;
		}
		static int CalcDigitCount(string format, char c) {
			string[] ss = format.Split(new char[] { '{', '}' });
			for(int i = 1; i < ss.Length; i += 2) {
				if(ss[i].Contains(c)) return ss[i].Count(x => x == c);
			}
			return 0;
		}
		static string CalcFormatCore(string format, int p) {
			for(int i = 0; i < p; i++) {
				format += "0";
			}
			return format += "}";
		}
		static string Replace(string text) {
			int start = -1, end = -1;
			for(int i = 0; i < text.Length; i++) {
				if(delimiterFormat[0] == text[i].ToString()) start = i;
				if(delimiterFormat[1] == text[i].ToString()) end = i;
				if(start != -1 && end != -1) break;
			}
			if(end - start < 0) throw new Exception("Bad display format");
			string format = text.Substring(start, end - start + 1);
			switch(GetSeniorFormat(format)) {
				case SeniorFormat.Days:
					text = text.Replace(format, dId);
					break;
				case SeniorFormat.Hours:
					text = text.Replace(format, hId);
					break;
				case SeniorFormat.Minutes:
					text = text.Replace(format, mId);
					break;
				case SeniorFormat.Seconds:
					text = text.Replace(format, sId);
					break;
				case SeniorFormat.None:
					throw new Exception("Bad display format");
			}
			return text;
		}
		public static TimeSpan? ObjectToTimeSpan(object val, RepositoryItemTimeSpanEdit properties) {
			if(val is TimeSpan) return (TimeSpan)val;
			if(val is DateTime) return new TimeSpan(((DateTime)val).Ticks);
			if(val is string) {
				TimeSpan result;
				if(TimeSpan.TryParse((string)val, out result))
					return result;
			}
			if(properties.IsNullInputAllowed) return null;
			return new TimeSpan(0);
		}
		public static SeniorFormat GetSeniorFormat(string format) {
			if(format.Contains("d")) return SeniorFormat.Days;
			if(format.Contains("h") || format.Contains("H")) return SeniorFormat.Hours;
			if(format.Contains("m")) return SeniorFormat.Minutes;
			if(format.Contains("s")) return SeniorFormat.Seconds;
			return SeniorFormat.None;
		}
	}
	internal enum SeniorFormat {
		Days = 4,
		Hours = 3,
		Minutes = 2,
		Seconds = 1,
		None = 0
	}
	#endregion
}
namespace DevExpress.XtraEditors.Repository {
	public class RepositoryItemTimeSpanEdit : RepositoryItemTimeEdit {
		const int defaultMaxDays = 100;
		const int defaultMaxHours = 24;
		const int defaultMaxMinutes = 60;
		const int defaultMaxSeconds = 60;
		const bool defaultAllowEditDays = true;
		const bool defaultAllowEditHours = true;
		const bool defaultAllowEditMinutes = true;
		const bool defaultAllowEditSeconds = true;
		public static string TimeSpanEditName = "TimeSpanEdit";
		public override string EditorTypeName { get { return TimeSpanEditName; } }
		static RepositoryItemTimeSpanEdit() { RegisterTimeSpanEdit(); }
		public RepositoryItemTimeSpanEdit() : base() {
			this.maxDays = defaultMaxDays;
			this.maxHours = defaultMaxHours;
			this.maxMinutes = defaultMaxMinutes;
			this.maxSeconds = defaultMaxSeconds;
			this.allowEditDays = defaultAllowEditDays;
			this.allowEditHours = defaultAllowEditHours;
			this.allowEditMinutes = defaultAllowEditMinutes;
			this.allowEditSeconds = defaultAllowEditSeconds;
			this.TimeEditStyle = TimeEditStyle.TouchUI;
		}
		public override FormatInfo EditFormat { get { return DisplayFormat; } }
		[Browsable(false)]
		public new TimeSpanEdit OwnerEdit { get { return base.OwnerEdit as TimeSpanEdit; } }
		[DXCategory(CategoryName.Appearance), DefaultValue(TimeEditStyle.TouchUI), SmartTagProperty("Time Edit Style", "Editor Style", SmartTagActionType.RefreshBoundsAfterExecute)]
		public override TimeEditStyle TimeEditStyle {
			get { return base.TimeEditStyle; }
			set { base.TimeEditStyle = value; }
		}
		[Browsable(false)]
		public static void RegisterTimeSpanEdit() {
			EditorRegistrationInfo.Default.Editors.Add(new EditorClassInfo(TimeSpanEditName, 
			  typeof(TimeSpanEdit), typeof(RepositoryItemTimeSpanEdit),
			  typeof(TimeSpanEditViewInfo), new ButtonEditPainter(), true));
		}
		RepositoryItemTimeSpanEdit source;
		public override void Assign(RepositoryItem item) {
			BeginUpdate();
			try {
				base.Assign(item);
				source = item as RepositoryItemTimeSpanEdit;
				if(source == null) return;
				this.maxDays = source.MaxDays;
				this.maxHours = source.MaxHours;
				this.maxMinutes = source.MaxMinutes;
				this.maxSeconds = source.MaxSeconds;
				this.allowEditDays = source.AllowEditDays;
				this.allowEditHours = source.allowEditHours;
				this.allowEditMinutes = source.AllowEditMinutes;
				this.allowEditSeconds = source.AllowEditSeconds;
				this.customDropDownControl = source.CustomDropDownControl;
			}
			finally {
				EndUpdate();
			}
		}
		protected internal override bool IsNullValue(object editValue) {
			if(NullTimeSpan == null || !IsNullInputAllowed) return false; 
			return (TimeSpan)editValue == (TimeSpan)NullTimeSpan;
		}
		protected override object GetEditValueForExport(object editValue, XtraPrinting.ExportTarget exportTarget) {
			return ((TimeSpan)editValue).ToString();
		}
		protected internal virtual string GetFormatMaskAccessFunction(string editMask, CultureInfo managerCultureInfo) { 
			return GetFormatMask(editMask, managerCultureInfo);
		}
		public override string GetDisplayText(FormatInfo format, object editValue) {
			if(string.IsNullOrEmpty(format.FormatString))
				return base.GetDisplayText(CalcDefaultDisplayFormat(editValue), editValue);
			FormatInfo fi = CanUseDefaultDisplayFormat(format.FormatString) ? CalcDefaultDisplayFormat(editValue) : format;
			return TimeSpanParser.GetDisplayText(editValue, fi.FormatString, this);
		}
		protected virtual bool CanUseDefaultDisplayFormat(string format) {
			if(string.IsNullOrEmpty(format)) return true;
			foreach(string s in TimeSpanParser.delimiterFormat) {
				if(format.Contains(s)) return false;
			}
			return true;
		}
		protected virtual string RemoveFirstFormat(string displayText) {
			int res = 0;
			while(int.TryParse(displayText[0].ToString(), out res)) {
				displayText = displayText.Remove(0, 1);
			}
			return displayText;
		}
		FormatInfo defaultDisplayFormat;
		protected virtual FormatInfo CalcDefaultDisplayFormat(object editValue) {
			if(defaultDisplayFormat == null)
				defaultDisplayFormat = new FormatInfo();
			defaultDisplayFormat.FormatType = FormatType.Custom;
			defaultDisplayFormat.FormatString = string.Empty;
			if(AllowEditDays) defaultDisplayFormat.FormatString += "{0:dd}.";
			if(AllowEditHours) defaultDisplayFormat.FormatString += "{0:hh}:";
			if(AllowEditMinutes) defaultDisplayFormat.FormatString += "{0:mm}:";
			if(AllowEditSeconds) defaultDisplayFormat.FormatString += "{0:ss}:";
			if(string.IsNullOrEmpty(defaultDisplayFormat.FormatString))
				return defaultDisplayFormat;
			defaultDisplayFormat.FormatString = defaultDisplayFormat.FormatString.Substring(0, defaultDisplayFormat.FormatString.Length - 1);
			return defaultDisplayFormat;
		}
		protected internal override string GetFormatMask(string editMask, CultureInfo managerCultureInfo) {
			return string.IsNullOrEmpty(editMask) ? CalcDefaultMaskFormatString() : base.GetFormatMask(editMask, managerCultureInfo);
		}
		protected internal virtual string CalcDefaultMaskFormatString() {
			string res = string.Empty;
			if(AllowEditDays) res += "dd.";
			if(AllowEditHours) res += "HH:";
			if(AllowEditMinutes) res += "mm:";
			if(AllowEditSeconds) res += "ss:";
			if(string.IsNullOrEmpty(res))
				res = "dd.HH:mm:ss:";
			res = res.Substring(0, res.Length - 1);
			return res;
		}
		protected override MaskProperties CreateMaskProperties() {
			return new TimeSpanMaskProperties(this);
		}
		CustomTimeSpanDropDownControl customDropDownControl;
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("RepositoryItemTimeSpanEditCustomDropDownControl"),
#endif
 DefaultValue(null)]
		public CustomTimeSpanDropDownControl CustomDropDownControl {
			get { return customDropDownControl; }
			set {
				if(CustomDropDownControl == value) 
					return;
				customDropDownControl = value;
			}
		}
		bool allowEditDays;
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("RepositoryItemTimeSpanEditAllowEditDays"),
#endif
 DefaultValue(defaultAllowEditDays), SmartTagProperty("Allow Edit Days", "")]
		public bool AllowEditDays {
			get { return allowEditDays; } 
			set {
				if(AllowEditDays == value)
					return;
				allowEditDays = value;
				OnPropertiesChanged();
			}
		}
		bool allowEditHours;
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("RepositoryItemTimeSpanEditAllowEditHours"),
#endif
 DefaultValue(defaultAllowEditHours), SmartTagProperty("Allow Edit Hours", "")]
		public bool AllowEditHours {
			get { return allowEditHours; }
			set {
				if(AllowEditHours == value)
					return;
				allowEditHours = value;
				OnPropertiesChanged();
			}
		}
		bool allowEditMinutes;
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("RepositoryItemTimeSpanEditAllowEditMinutes"),
#endif
 DefaultValue(defaultAllowEditMinutes), SmartTagProperty("Allow Edit Minutes", "")]
		public bool AllowEditMinutes{
			get { return allowEditMinutes;}
			set{
				if(AllowEditMinutes == value)
					return;
				allowEditMinutes = value;
				OnPropertiesChanged();
			}
		}
		bool allowEditSeconds;
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("RepositoryItemTimeSpanEditAllowEditSeconds"),
#endif
 DefaultValue(defaultAllowEditSeconds), SmartTagProperty("Allow Edit Seconds", "")]
		public bool AllowEditSeconds {
			get { return allowEditSeconds; }
			set {
				if(AllowEditSeconds == value)
					return;
				allowEditSeconds = value;
				OnPropertiesChanged();
			}
		}
		int maxDays;
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("RepositoryItemTimeSpanEditMaxDays"),
#endif
 DefaultValue(defaultMaxDays), SmartTagProperty("Max Days", "")]
		public int MaxDays {
			get { return maxDays; }
			set {
				if(MaxDays == value) return;
				if(value < 0) throw new ArgumentException("MaxDays must be non-negative");
				maxDays = value;
			}
		}
		int maxHours;
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("RepositoryItemTimeSpanEditMaxHours"),
#endif
 DefaultValue(defaultMaxHours), SmartTagProperty("Max Hours", "")]
		public int MaxHours {
			get { return maxHours; }
			set {
				if(MaxHours == value) return;
				if(value < 0) throw new ArgumentException("MaxHours must be non-negative");
				maxHours = value;
			}
		}
		int maxMinutes;
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("RepositoryItemTimeSpanEditMaxMinutes"),
#endif
 DefaultValue(defaultMaxMinutes), SmartTagProperty("Max Minutes", "")]
		public int MaxMinutes {
			get { return maxMinutes; }
			set {
				if(MaxMinutes == value) return;
				if(value < 0) throw new ArgumentException("MaxMinutes must be non-negative");
				maxMinutes = value;
			}
		}
		int maxSeconds;
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("RepositoryItemTimeSpanEditMaxSeconds"),
#endif
 DefaultValue(defaultMaxSeconds), SmartTagProperty("Max Seconds", "")]
		public int MaxSeconds {
			get { return maxSeconds; }
			set {
				if(MaxSeconds == value) return;
				if(value < 0) throw new ArgumentException("MaxSeconds must be non-negative");
				maxSeconds = value;
			}
		}
		TimeSpan? nullTimeSpan;
		[ DefaultValue(null)]
		public TimeSpan? NullTimeSpan {
			get { return nullTimeSpan; }
			set {
				if(NullTimeSpan == value) return;
				nullTimeSpan = value;
			}
		}
		protected internal virtual bool IsSeniorDay { get { return AllowEditDays; } }
		protected internal virtual bool IsSeniorHour { get { return !IsSeniorDay && AllowEditHours; } }
		protected internal virtual bool IsSeniorMinute { get { return !IsSeniorDay && !IsSeniorHour && AllowEditMinutes; } }
		protected internal virtual bool IsSeniorSecond { get { return !IsSeniorDay && !IsSeniorHour && !IsSeniorMinute && AllowEditSeconds; } }
	}
	public class TimeSpanMaskProperties : MaskProperties {
		const string defaultEditMask = "";
		const MaskType defaultMaskType = MaskType.DateTime;
		public TimeSpanMaskProperties(RepositoryItemTimeSpanEdit repositoryItemTimeSpanEdit) {
			this.properties = repositoryItemTimeSpanEdit;
			MaskType = defaultMaskType;
			EditMask = defaultEditMask;
		}
		RepositoryItemTimeSpanEdit properties;
		[DefaultValue(defaultEditMask)]
		public override string EditMask {
			get { return string.IsNullOrEmpty(base.EditMask) ? properties.CalcDefaultMaskFormatString() : base.EditMask; }
			set { base.EditMask = value; }
		}
		[DefaultValue(typeof(MaskType), "DateTime")]
		public override MaskType MaskType { get { return base.MaskType; } set { base.MaskType = value; } }
	}
}
namespace DevExpress.XtraEditors.ViewInfo {
	public class TimeSpanEditViewInfo : TimeEditViewInfo {
		public TimeSpanEditViewInfo(RepositoryItem item) : base(item) {
			this.properties = (RepositoryItemTimeSpanEdit)item;
		}
		RepositoryItemTimeSpanEdit properties;
		protected override bool CanDisplayButton(EditorButton btn) {
			return properties.AllowEditDays || properties.AllowEditHours || properties.AllowEditMinutes || properties.AllowEditSeconds;
		}
	}
}
namespace DevExpress.XtraEditors.Drawing {
	public class TimeSpanEditPainter : ButtonEditPainter {
		public TimeSpanEditPainter() : base() { }
	}
}
namespace DevExpress.XtraEditors.Popup {
	[ToolboxItem(false)]
	public class TimeSpanEditDropDownForm : TouchPopupTimeEditForm {
		public TimeSpanEditDropDownForm(PopupBaseEdit ownerEdit) : base(ownerEdit) { }
		public TimeSpanEdit TimeSpanEdit { get { return (TimeSpanEdit)OwnerEdit; } }
		protected virtual CustomTimeSpanDropDownControl CustomDropDown { get { return TimeSpanEdit.Properties.CustomDropDownControl; } } 
		protected virtual ITimeSpanDropDownControl PopupControl { get { return (ITimeSpanDropDownControl)EmbeddedControl; } }
		protected override Control EmbeddedControl { get { return CustomDropDown ?? base.EmbeddedControl; } }
		protected internal override object QueryResultValue() { return PopupControl.SelectedDate - new DateTime(0); }
		protected override Size CalcBestSize() { return PopupControl.GetSize(); }
		public override void SetDate(object val) { }
		public override bool AllowSizing { get { return PopupControl.AllowSizing; } set { } }
		protected override void CreateProviders() {
			if(CustomDropDown == null)
				TouchCalendar.CreateProviders();
		}
		protected override void CreateTouchCalendar() {
			if(CustomDropDown == null)
				TouchCalendar = new DefaultTimeSpanDropDownControl(this);
		}
		protected override void Dispose(bool disposing) {
			if(Controls.Contains(CustomDropDown))
				Controls.Remove(CustomDropDown);
			base.Dispose(disposing);
		}
	}
	public interface ITimeSpanDropDownControl {
		DateTime SelectedDate { get; set; }
		Size GetSize();
		bool AllowSizing { get; }
	}
	[ToolboxItem(false)]
	public class CustomTimeSpanDropDownControl : XtraUserControl, ITimeSpanDropDownControl {
		public CustomTimeSpanDropDownControl() { }
		protected virtual Size CalcBestSize() { return new Size(400, 200); }
		protected virtual TimeSpan SelectedTimeSpan { get; set; }
		protected virtual bool AllowSizing { get { return true; } }
		DateTime ITimeSpanDropDownControl.SelectedDate {
			get { return new DateTime(0) + SelectedTimeSpan ; }
			set { SelectedTimeSpan = value - new DateTime(0); }
		}
		Size ITimeSpanDropDownControl.GetSize() { return CalcBestSize(); }
		bool ITimeSpanDropDownControl.AllowSizing { get { return AllowSizing; } }
	}
	public class DefaultTimeSpanDropDownControl : TimeEditTouchCalendar, ITimeSpanDropDownControl {
		public DefaultTimeSpanDropDownControl(TouchPopupTimeEditForm form) : base(form) { }
		public new TimeSpanEditDropDownForm Form { get { return (TimeSpanEditDropDownForm)base.Form; } }
		public RepositoryItemTimeSpanEdit Properties { get { return Form.TimeSpanEdit.Properties; } }
		protected override void InitializeItemsControl() {
			if(Form == null) return;
			Providers.Clear();
			TotalProviders = 0;
			if(Properties.AllowEditDays) {
				Providers.Add(CreateDaysProvider());
				TotalProviders++;
			}
			if(Properties.AllowEditHours) {
				Providers.Add(CreateHoursProvider());
				TotalProviders++;
			}
			if(Properties.AllowEditMinutes) {
				Providers.Add(CreateMinutesProvider());
				TotalProviders++;
			}
			if(Properties.AllowEditSeconds) {
				Providers.Add(CreateSecondsProvider());
				TotalProviders++;
			}
		}
		protected override void InitializeTouchCalendar() {
			SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
			PickContainer = CreatePickContainer();
			TotalProviders = 0;
			CreateProviders();
			FocusOnMouseDown = false;
			SetStyle(ControlStyles.Selectable, false);
			SetStyle(ControlStyles.UserMouse, false);
		}
		public new IItemsProvider CreateNewProvider(DateTimeMaskFormatElementEditable editableFormat) {
			if(Properties.AllowEditDays) return CreateDaysProvider();
			if(Properties.AllowEditHours) return CreateHoursProvider();
			if(Properties.AllowEditMinutes) return CreateMinutesProvider();
			if(Properties.AllowEditSeconds) return CreateSecondsProvider();
			return null;
		}
		protected virtual IItemsProvider CreateDaysProvider() {
			return new TimeSpanDaysItemsProvider(Properties.MaxDays);
		}
		protected virtual IItemsProvider CreateHoursProvider() {
			return new TimeSpanHoursItemsProvider(Properties.MaxHours);
		}
		protected virtual IItemsProvider CreateMinutesProvider() {
			return new TimeSpanMinutesItemsProvider(Properties.MaxMinutes / GetMinuteIncrement());
		}
		protected virtual IItemsProvider CreateSecondsProvider() {
			return new TimeSpanSecondsItemsProvider(Properties.MaxSeconds / GetSecondIncrement());
		}
		public override void OnDateChanged() {
			long days = GetValueFromContainer(typeof(TimeSpanDaysItemsProvider));
			long hours = GetValueFromContainer(typeof(TimeSpanHoursItemsProvider));
			long minutes = GetValueFromContainer(typeof(TimeSpanMinutesItemsProvider));
			long seconds = GetValueFromContainer(typeof(TimeSpanSecondsItemsProvider));
			long ticks = ConvertDaysToTicks(days) + ConvertHoursToTicks(hours) + ConvertMinutesToTicks(minutes) + ConvertSecondsToTicks(seconds);
			SelectedDate = new DateTime(ticks);
			if(ShouldUpdatePanels) UpdatePanels();
		}
		protected override bool IsMinsItemsProvider(Type type) { return type == typeof(TimeSpanMinutesItemsProvider); }
		protected override bool IsSecondsItemsProvider(Type type) { return type == typeof(TimeSpanSecondsItemsProvider); }
		protected override int GetValueForPanel(IItemsProvider iItemsProvider) {
			object editValue = Form.TimeSpanEdit.EditValue ?? new TimeSpan(0);
			TimeSpan time = TimeSpan.Parse(editValue.ToString());
			if(iItemsProvider is TimeSpanDaysItemsProvider) return GetDaysForPanel(time);
			if(iItemsProvider is TimeSpanHoursItemsProvider) return GetHoursForPanel(time);
			if(iItemsProvider is TimeSpanMinutesItemsProvider) return GetMinutesForPanel(time) / GetMinuteIncrement();
			if(iItemsProvider is TimeSpanSecondsItemsProvider) return GetSecondsForPanel(time) / GetSecondIncrement();
			return -1;
		}
		public override DateTime GetDateFromIndex(IItemsProvider provider, int itemIndex) {
			DateTime date = GetDateTimeFromContainer();
			int index = itemIndex < 0 ? Properties.MaxDays - 1 : itemIndex + 1;
			if(provider is DaysItemsProvider) {
				if(index > DateTime.DaysInMonth(date.Year, date.Month)) return date;
				return new DateTime(date.Year, date.Month, index);
			}
			return date;
		}
		protected virtual long ConvertDaysToTicks(long days) { return Math.Max(0, (Properties.AllowEditDays ? days : GetDaysForPanel(Form.TimeSpanEdit.TimeSpan)) * TimeSpan.TicksPerDay); }
		protected virtual long ConvertHoursToTicks(long hours) { return Math.Max(0, (Properties.AllowEditHours ? hours : GetHoursForPanel(Form.TimeSpanEdit.TimeSpan)) * TimeSpan.TicksPerHour); }
		protected virtual long ConvertMinutesToTicks(long minutes) { return Math.Max(0, (Properties.AllowEditMinutes ? minutes : GetMinutesForPanel(Form.TimeSpanEdit.TimeSpan)) * TimeSpan.TicksPerMinute); }
		protected virtual long ConvertSecondsToTicks(long seconds) { return Math.Max(0, (Properties.AllowEditSeconds ? seconds : GetSecondsForPanel(Form.TimeSpanEdit.TimeSpan)) * TimeSpan.TicksPerSecond); }
		protected virtual int GetDaysForPanel(TimeSpan time) { return Properties.IsSeniorDay ? (int)Math.Floor(time.TotalDays) : 0; }
		protected virtual int GetHoursForPanel(TimeSpan time) { return Properties.IsSeniorHour ? (int)Math.Floor(time.TotalHours) : time.Hours; }
		protected virtual int GetMinutesForPanel(TimeSpan time) { return Properties.IsSeniorMinute ? (int)Math.Floor(time.TotalMinutes) : time.Minutes; }
		protected virtual int GetSecondsForPanel(TimeSpan time) { return Properties.IsSeniorSecond ? (int)Math.Floor(time.TotalSeconds) : time.Seconds; }
		#region ITimeSpanDropDownControl
		DateTime ITimeSpanDropDownControl.SelectedDate { get { return SelectedDate; } set { SelectedDate = value; } }
		Size ITimeSpanDropDownControl.GetSize() { return CalcBestSize(); }
		bool ITimeSpanDropDownControl.AllowSizing { get { return false; } }
		#endregion
	}
	public class TimeSpanDaysItemsProvider : DaysItemsProvider {
		public TimeSpanDaysItemsProvider(int count) : base(count) { }
		protected override IItemPainter CreatePainter() {
			return new TimeSpanDaysItemsPainter();
		}
		protected override int FirstItemIndexCore { get { return 0; } }
	}
	public class TimeSpanDaysItemsPainter : DaysItemsPainter {
		public TimeSpanDaysItemsPainter() : base() { }
		protected override void DrawCore(Utils.Drawing.GraphicsCache cache, PickItemInfo info, IPickItemsContainerDrawInfo drawInfo) {
			PickItemsPainter painter = new PickItemsPainter();
			string firstString = painter.ConvertIntToString(info.ItemIndex, StringLength);
			bool descriptionIsExist = GetCalendar(info).ShowTime();
			string description = Localizer.Active.GetLocalizedString(StringId.Days);
			painter.DrawItem(cache, drawInfo, info, firstString, description);
		}
	}
	public class TimeSpanHoursItemsProvider : HoursItemsProvider {
		public TimeSpanHoursItemsProvider(int count) : base(count) { }
		protected override IItemPainter CreatePainter(int index) {
			return new TimeSpanHoursItemsPainter(index);
		}
	}
	public class TimeSpanHoursItemsPainter : HoursItemsPainter {
		public TimeSpanHoursItemsPainter(int startIndex) : base(startIndex) { }
		protected override void DrawCore(Utils.Drawing.GraphicsCache cache, PickItemInfo info, IPickItemsContainerDrawInfo drawInfo) {
			PickItemsPainter painter = new PickItemsPainter();
			string firstString = painter.ConvertIntToString(info.ItemIndex, StringLength);
			string description = Localizer.Active.GetLocalizedString(StringId.Hours);
			painter.DrawItem(cache, drawInfo, info, firstString, description);
		}
	}
	public class TimeSpanMinutesItemsProvider : MinsItemsProvider {
		public TimeSpanMinutesItemsProvider(int count) : base(count) { }
		protected override IItemPainter CreatePainter(int index) {
			return new TimeSpanMinutesItemsPainter(index);
		}
	}
	public class TimeSpanMinutesItemsPainter : MinsItemsPainter {
		public TimeSpanMinutesItemsPainter(int startIndex) : base() { }
		protected override void DrawCore(Utils.Drawing.GraphicsCache cache, PickItemInfo info, IPickItemsContainerDrawInfo drawInfo) {
			PickItemsPainter painter = new PickItemsPainter();
			TouchCalendar calendar = (TouchCalendar)info.Panel.Container.Owner;
			int minuteIncrement = calendar == null ? 1 : calendar.GetMinuteIncrement();
			string firstString = painter.ConvertIntToString(info.ItemIndex * minuteIncrement, StringLength);
			string description = Localizer.Active.GetLocalizedString(StringId.Mins);
			painter.DrawItem(cache, drawInfo, info, firstString, description);
		}
	}
	public class TimeSpanSecondsItemsProvider : SecondsItemsProvider {
		public TimeSpanSecondsItemsProvider(int count) : base(count) { }
		protected override IItemPainter CreatePainter(int index) {
			return new TimeSpanSecondsItemsPainter(index);
		}
	}
	public class TimeSpanSecondsItemsPainter : SecondsItemsPainter {
		public TimeSpanSecondsItemsPainter(int startIndex) : base() { }
		protected override void DrawCore(Utils.Drawing.GraphicsCache cache, PickItemInfo info, IPickItemsContainerDrawInfo drawInfo) {
			PickItemsPainter painter = new PickItemsPainter();
			TouchCalendar calendar = (TouchCalendar)info.Panel.Container.Owner;
			int secondIncrement = calendar == null ? 1 : calendar.GetSecondIncrement();
			string firstString = painter.ConvertIntToString(info.ItemIndex * secondIncrement, StringLength);
			string description = Localizer.Active.GetLocalizedString(StringId.Secs);
			painter.DrawItem(cache, drawInfo, info, firstString, description);
		}
	}
}
