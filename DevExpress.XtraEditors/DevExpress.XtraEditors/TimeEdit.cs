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
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using DevExpress.LookAndFeel;
using DevExpress.Utils;
using DevExpress.Utils.Drawing;
using DevExpress.Utils.Paint;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.ViewInfo;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Drawing;
using DevExpress.Data.Mask;
using DevExpress.XtraEditors.Mask;
using DevExpress.XtraEditors.Popup;
using DevExpress.Utils.Win;
using System.Collections.Generic;
using DevExpress.XtraEditors.Popups;
using DevExpress.Utils.Design;
using DevExpress.XtraEditors.ToolboxIcons;
namespace DevExpress.XtraEditors.Mask {
	public class TimeEditMaskProperties : DateTimeMaskProperties {
		public TimeEditMaskProperties() : base() {
			this.fEditMask = "T";
		}
		[CategoryAttribute("Mask"),
		DefaultValue("T"),
		Localizable(true),
		RefreshProperties(RefreshProperties.All),
		]
		public override string EditMask {
			get { return base.EditMask; }
			set { base.EditMask = value; }
		}
	}
}
namespace DevExpress.XtraEditors.Repository {
	public enum TimeEditStyle { SpinButtons, TouchUI }
	public class RepositoryItemTimeEdit : RepositoryItemBaseSpinEdit {
		int touchUISecondIncrement, touchUIMinuteIncrement;
		public RepositoryItemTimeEdit() {
			this.timeEditStyle = TimeEditStyle.SpinButtons;
			this.touchUISecondIncrement = this.touchUIMinuteIncrement = 1;
		}
		protected override MaskProperties CreateMaskProperties() {
			return new TimeEditMaskProperties();
		}
		public override void Assign(RepositoryItem item) {
			RepositoryItemTimeEdit source = item as RepositoryItemTimeEdit;
			BeginUpdate();
			try {
				base.Assign(item);
				if(source == null) return;
				this.timeEditStyle = source.TimeEditStyle;
				this.touchUISecondIncrement = source.TouchUISecondIncrement;
				this.touchUIMinuteIncrement = source.TouchUIMinuteIncrement;
			}
			finally {
				EndUpdate();
			}
		}
		protected internal override bool IsButtonEnabled(EditorButton button) {
			if(!base.IsButtonEnabled(button)) return false;
			return IsReadOnlyAllowsDropDown || !ReadOnly;
		}
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never), Browsable(false), Obsolete(ObsoleteText.SRObsoletePropertiesText)]
		public new RepositoryItemTimeEdit Properties { get { return this; } }
		[Browsable(false)]
		public override string EditorTypeName { get { return "TimeEdit"; } }
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("RepositoryItemTimeEditTimeFormat"),
#endif
 DefaultValue(TimeFormat.HourMinSec)]
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Obsolete(ObsoleteText.SRObsoleteTimeFormat)]
		[RefreshProperties(RefreshProperties.All)]
		public TimeFormat TimeFormat {
			get {
				if(Regex.IsMatch(Mask.EditMask, @"^(h|(\%?h{1,2} t{1,2})|(\%?H{1,2}))$"))
					return TimeFormat.Hour;
				else if(Regex.IsMatch(Mask.EditMask, @"^(t|(\%?h{1,2}\:m{1,2} t{1,2})|(\%?H{1,2}\:m{1,2}))$"))
					return TimeFormat.HourMin;
				else
					return TimeFormat.HourMinSec;
			}
			set {
				if(TimeFormat == value)
					return;
				Mask.EditMask = GetMaskFromTimeModeEnums(value, HourFormat);
			}
		}
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("RepositoryItemTimeEditHourFormat"),
#endif
 DefaultValue(HourFormat.Default)]
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Obsolete(ObsoleteText.SRObsoleteHourFormat)]
		[RefreshProperties(RefreshProperties.All)]
		public virtual HourFormat HourFormat {
			get {
				if(Mask.EditMask == null || Mask.EditMask.Length <= 1)
					return HourFormat.Default;
				else if(Regex.IsMatch(Mask.EditMask, @"H"))
					return HourFormat.Hour24;
				else if(Regex.IsMatch(Mask.EditMask, @"h"))
					return HourFormat.Hour12;
				else
					return HourFormat.Default;
			}
			set {
				if(HourFormat == value)
					return;
				Mask.EditMask = GetMaskFromTimeModeEnums(TimeFormat, value);
			}
		}
		static string GetMaskFromTimeModeEnums(TimeFormat timeFormat, HourFormat hourFormat) {
			switch(timeFormat) {
				case TimeFormat.Hour:
				switch(hourFormat) {
					case HourFormat.Hour12:
						return "hh tt";
					case HourFormat.Hour24:
						return "HH";
					default:
						return "h";
				}
				case TimeFormat.HourMin:
				switch(hourFormat) {
					case HourFormat.Hour12:
						return "hh:mm tt";
					case HourFormat.Hour24:
						return "HH:mm";
					default:
						return "t";
				}
				default:
				switch(hourFormat) {
					case HourFormat.Hour12:
						return "hh:mm:ss tt";
					case HourFormat.Hour24:
						return "HH:mm:ss";
					default:
						return "T";
				}
			}
		}
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("RepositoryItemTimeEditCycleEditing"),
#endif
 DefaultValue(true)]
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Obsolete(ObsoleteText.SRObsoleteCycleEditing)]
		public bool CycleEditing {
			get { return true; }
			set {}
		}
		[DXCategory(CategoryName.Behavior), 
#if !SL
	DevExpressXtraEditorsLocalizedDescription("RepositoryItemTimeEditTextEditStyle"),
#endif
 DefaultValue(TextEditStyles.Standard), SmartTagProperty("Text Edit Style", "Editor Style", SmartTagActionType.RefreshBoundsAfterExecute)]
		public override TextEditStyles TextEditStyle {
			get { return base.TextEditStyle; }
			set {
				if(IsLoading) {
					this.textEditStyleValueCore = value;
					return;
				}
				if(!AcceptStyle(value)) value = TextEditStyles.Standard;
				base.TextEditStyle = value;
			}
		}
		protected virtual bool AcceptStyle(TextEditStyles style) {
			return style != TextEditStyles.DisableTextEditor || TimeEditStyle == TimeEditStyle.TouchUI;
		}
		protected void CheckTextEditStyle() {
			if(!AcceptStyle(TextEditStyle)) TextEditStyle = TextEditStyles.Standard;
		}
		TextEditStyles? textEditStyleValueCore = null;
		public override void EndInit() {
			base.EndInit();
			if(this.textEditStyleValueCore.HasValue) {
				TextEditStyle = textEditStyleValueCore.Value;
			}
		}
		public override string GetDisplayText(FormatInfo format, object editValue) {
			if(editValue is DateTime && (format == null || format.FormatString == null || format.FormatString.Length == 0))
								return RaiseCustomDisplayText(format, editValue, ((DateTime)editValue).ToString(GetFormatMask(Mask.EditMask, Mask.Culture), Mask.Culture));
			else
				return base.GetDisplayText(format, editValue);
		}
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("RepositoryItemTimeEditEditMask"),
#endif
		DXCategory(CategoryName.Format), 
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		RefreshProperties(RefreshProperties.All),
		DefaultValue("T"),
		]
		public string EditMask {
			get { return Mask.EditMask; }
			set { Mask.EditMask = value; }
		}
		TimeEditStyle timeEditStyle;
		[DXCategory(CategoryName.Appearance), DefaultValue(TimeEditStyle.SpinButtons), SmartTagProperty("Time Edit Style", "Editor Style", SmartTagActionType.RefreshBoundsAfterExecute)]
		public virtual TimeEditStyle TimeEditStyle {
			get { return timeEditStyle; }
			set {
				if(TimeEditStyle == value)
					return;
				timeEditStyle = value;
				CheckTextEditStyle();
				OnPropertiesChanged();
			}
		}
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("RepositoryItemTimeEditTouchUISecondIncrement"),
#endif
 DXCategory(CategoryName.Appearance), DefaultValue(1)]
		public int TouchUISecondIncrement {
			get { return touchUISecondIncrement; }
			set {
				if(TouchUISecondIncrement == value)
					return;
				touchUISecondIncrement = CheckTimeIncrement(value);
				OnPropertiesChanged();
			}
		}
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("RepositoryItemTimeEditTouchUIMinuteIncrement"),
#endif
 DXCategory(CategoryName.Appearance), DefaultValue(1)]
		public int TouchUIMinuteIncrement {
			get { return touchUIMinuteIncrement; }
			set {
				if(TouchUIMinuteIncrement == value)
					return;
				touchUIMinuteIncrement = CheckTimeIncrement(value);
				OnPropertiesChanged();
			}
		}
		protected int CheckTimeIncrement(int desiredValue){
			return (desiredValue < 1 || desiredValue > 60 || 60 % desiredValue != 0) ? 1 : desiredValue;
		}
		protected internal virtual string GetFormatMask(string editMask, CultureInfo managerCultureInfo) {
			if(editMask == "h") {
				CultureInfo workingCulture = managerCultureInfo == null ? CultureInfo.CurrentCulture : managerCultureInfo;
				if(workingCulture.DateTimeFormat.AMDesignator == workingCulture.DateTimeFormat.PMDesignator) {
					return "HH";
				} else {
					return "hh tt";
				}
			}
			return editMask;
		}
		[Browsable(true), EditorBrowsable(EditorBrowsableState.Always), DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
		public override bool ShowPopupShadow {
			get {
				return base.ShowPopupShadow;
			}
			set {
				base.ShowPopupShadow = value;
			}
		}
		[Browsable(true), EditorBrowsable(EditorBrowsableState.Always), DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
		public override bool SuppressMouseEventOnOuterMouseClick {
			get {
				return base.SuppressMouseEventOnOuterMouseClick;
			}
			set {
				base.SuppressMouseEventOnOuterMouseClick = value;
			}
		}
		[Browsable(true), EditorBrowsable(EditorBrowsableState.Always), DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
		public override DefaultBoolean AllowDropDownWhenReadOnly {
			get {
				return base.AllowDropDownWhenReadOnly;
			}
			set {
				base.AllowDropDownWhenReadOnly = value;
			}
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override Size PopupFormSize {
			get { return base.PopupFormSize; }
			set { base.PopupFormSize = value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override Size PopupFormMinSize {
			get { return base.PopupFormMinSize; }
			set { base.PopupFormMinSize = value; }
		}
		[DefaultValue(PopupBorderStyles.Default), Browsable(true), EditorBrowsable(EditorBrowsableState.Always), DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
		public override PopupBorderStyles PopupBorderStyle {
			get {
				return base.PopupBorderStyle;
			}
			set {
				base.PopupBorderStyle = value;
			}
		}
		bool ShouldSerializeCloseUpKey() { return CloseUpKey.Key != Keys.F4; }
		[Browsable(true), EditorBrowsable(EditorBrowsableState.Always), DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
		public override KeyShortcut CloseUpKey {
			get {
				return base.CloseUpKey;
			}
			set {
				base.CloseUpKey = value;
			}
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override ResizeMode PopupResizeMode {
			get { return base.PopupResizeMode; }
			set { base.PopupResizeMode = value; }
		}
		[Browsable(true), EditorBrowsable(EditorBrowsableState.Always), DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
		public override ShowDropDown ShowDropDown {
			get {
				return base.ShowDropDown;
			}
			set {
				base.ShowDropDown = value;
			}
		}
		[Browsable(true), EditorBrowsable(EditorBrowsableState.Always), DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
		public override int ActionButtonIndex {
			get {
				return base.ActionButtonIndex;
			}
			set {
				base.ActionButtonIndex = value;
			}
		}
		[Browsable(true), EditorBrowsable(EditorBrowsableState.Always), DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
		public override event CancelEventHandler QueryCloseUp {
			add { this.Events.AddHandler(queryCloseUp, value); }
			remove { this.Events.RemoveHandler(queryCloseUp, value); }
		}
		[Browsable(true), EditorBrowsable(EditorBrowsableState.Always), DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
		public override event EventHandler Popup {
			add { this.Events.AddHandler(popup, value); }
			remove { this.Events.RemoveHandler(popup, value); }
		}
		[Browsable(true), EditorBrowsable(EditorBrowsableState.Always), DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
		public override event CancelEventHandler QueryPopUp {
			add { this.Events.AddHandler(queryPopUp, value); }
			remove { this.Events.RemoveHandler(queryPopUp, value); }
		}
		[Browsable(true), EditorBrowsable(EditorBrowsableState.Always), DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
		public override event CloseUpEventHandler CloseUp {
			add { this.Events.AddHandler(closeUp, value); }
			remove { this.Events.RemoveHandler(closeUp, value); }
		}
		[Browsable(true), EditorBrowsable(EditorBrowsableState.Always), DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
		public override event ClosedEventHandler Closed {
			add { this.Events.AddHandler(closed, value); }
			remove { this.Events.RemoveHandler(closed, value); }
		}
	}
}
namespace DevExpress.XtraEditors.ViewInfo {
	public class TimeEditViewInfo : BaseSpinEditViewInfo {
		public TimeEditViewInfo(RepositoryItem item) : base(item) { 
		}
		protected override EditorButtonObjectInfoArgs CreateButtonInfo(EditorButton button, int index) {
			if(((RepositoryItemTimeEdit)Item).TimeEditStyle == TimeEditStyle.TouchUI && index == 0) {
				return new EditorButtonObjectInfoArgs(button, PaintAppearance);
			} 
			return base.CreateButtonInfo(button, index);
		}
	}
}
namespace DevExpress.XtraEditors {
	[DXToolboxItem(DXToolboxItemKind.Free),
	 Designer("DevExpress.XtraEditors.Design.TimeEditDesigner, " + AssemblyInfo.SRAssemblyEditorsDesign),
	 Description("Allows an end-user to edit time values."),
	 ToolboxTabName(AssemblyInfo.DXTabCommon),
	ToolboxBitmap(typeof(ToolboxIconsRootNS), "TimeEdit")
	]
	public class TimeEdit : BaseSpinEdit {
		public TimeEdit() { 
			InitEditValue();
		}
		protected virtual void InitEditValue() {
			this.fOldEditValue = this.fEditValue = DateTime.Today;
		}
		[Browsable(false)]
		public override string EditorTypeName { get { return "TimeEdit"; } }
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("TimeEditProperties"),
#endif
 DXCategory(CategoryName.Properties), DesignerSerializationVisibility(DesignerSerializationVisibility.Content), SmartTagSearchNestedProperties]
		public new RepositoryItemTimeEdit Properties { get { return base.Properties as RepositoryItemTimeEdit; } }
		[Browsable(false), Bindable(false)]
		public override string Text { get { return base.Text; } set {} }
		static DateTime ConvertObjectToDateTime(object val) {
			if(val is DateTime)
				return (DateTime)val;
			if(val is TimeSpan)
				return new DateTime(((TimeSpan)val).Ticks);
			try {
				return DateTime.Parse(val.ToString());
			} catch {}
			return DateTime.MinValue;
		}
		[DXCategory(CategoryName.Appearance), 
#if !SL
	DevExpressXtraEditorsLocalizedDescription("TimeEditTime"),
#endif
 DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public virtual DateTime Time {
			get { return ConvertObjectToDateTime(EditValue); }
			set { EditValue = value; }
		}
		protected override void OnEditValueChanged() {
			base.OnEditValueChanged();
			TrySyncEditValueWithPopup();
		}
		protected virtual void TrySyncEditValueWithPopup() {
			if(PopupForm == null || !PopupForm.Visible)
				return;
			((TouchPopupTimeEditForm)PopupForm).SetDate(Time);
		}
		protected internal override MaskManager CreateMaskManager(MaskProperties mask) {
			MaskProperties patchedMask = new TimeEditMaskProperties();
			patchedMask.Assign(mask);
			patchedMask.EditMask = Properties.GetFormatMask(mask.EditMask, mask.Culture);
			return BaseCreateMaskManager(patchedMask);
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		protected virtual MaskManager BaseCreateMaskManager(MaskProperties patchedMask) {
			return base.CreateMaskManager(patchedMask);
		}
		protected override PopupBaseForm CreatePopupForm() {
			return new TouchPopupTimeEditForm(this);
		}
		[Browsable(true), EditorBrowsable(EditorBrowsableState.Always), DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
		public override void ShowPopup() {
			base.ShowPopup();
		}
		[Browsable(true), EditorBrowsable(EditorBrowsableState.Always), DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
		public override void ClosePopup() {
			base.ClosePopup();
		}
		[Browsable(true), EditorBrowsable(EditorBrowsableState.Always), DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
		public override void CancelPopup() {
			base.CancelPopup();
		}
		[Browsable(true), EditorBrowsable(EditorBrowsableState.Always), DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
		public override void RefreshEditValue() {
			base.RefreshEditValue();
		}
		[Browsable(true), EditorBrowsable(EditorBrowsableState.Always), DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
		public override event CancelEventHandler QueryCloseUp {
			add { Properties.QueryCloseUp += value; }
			remove { Properties.QueryCloseUp -= value; }
		}
		[Browsable(true), EditorBrowsable(EditorBrowsableState.Always), DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
		public override event EventHandler Popup {
			add { Properties.Popup += value; }
			remove { Properties.Popup -= value; }
		}
		[Browsable(true), EditorBrowsable(EditorBrowsableState.Always), DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
		public override event CancelEventHandler QueryPopUp {
			add { Properties.QueryPopUp += value; }
			remove { Properties.QueryPopUp -= value; }
		}
		[Browsable(true), EditorBrowsable(EditorBrowsableState.Always), DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
		public override event CloseUpEventHandler CloseUp {
			add { Properties.CloseUp += value; }
			remove { Properties.CloseUp -= value; }
		}
		[Browsable(true), EditorBrowsable(EditorBrowsableState.Always), DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
		public override event ClosedEventHandler Closed {
			add { Properties.Closed += value; }
			remove { Properties.Closed -= value; }
		}
		protected override void OnKeyDown(KeyEventArgs e) {
			if(IsPopupOpen) PopupForm.OnKeyDownCore(e);			
			base.OnKeyDown(e);
		}
		protected override void OnKeyPress(KeyPressEventArgs e) {
			base.OnKeyPress(e);
			if(IsPopupOpen) e.Handled = true;
		}
		protected override void OnMaskBox_MouseWheel(object sender, MouseEventArgs e) {
			if(IsPopupOpen) {
				if(e is DXMouseEventArgs)
					((DXMouseEventArgs)e).Handled = true;
				if(PopupForm is TouchPopupForm)
					((TouchPopupForm)PopupForm).TouchCalendar.OnMouseWheelCore(e);
			}
			base.OnMaskBox_MouseWheel(sender, e);
		}
	}
}
namespace DevExpress.XtraEditors.Controls {
	public enum TimeFormat { HourMinSec, HourMin, Hour }
	public enum HourFormat { Default, Hour12, Hour24 }
}
namespace DevExpress.XtraEditors.Popup {
	[ToolboxItem(false)]
	public class TouchPopupTimeEditForm : TouchPopupForm {
		public TouchPopupTimeEditForm(PopupBaseEdit ownerEdit)
			: base(ownerEdit) {
		}
		public TimeEdit TimeEdit { get { return (TimeEdit)OwnerEdit; } }
		protected override void CreateTouchCalendar() {
			TouchCalendar = new TimeEditTouchCalendar(this);
		}
		public override void SetDate(object val) {
			DateTime date = IsNull(val) ? DateTime.Now : TimeEdit.Time;
			TouchCalendar.SelectedDate = date;
			if(Visible) {
				TouchCalendar.SetDate();
				OldEditValue = date;
			}
		}
	}  
	public class TimeEditTouchCalendar : TouchCalendar {
		protected TouchPopupTimeEditForm Form { get; set; }
		protected internal TimeEdit OwnerEdit { get { return Form.TimeEdit; } }
		public TimeEditTouchCalendar(TouchPopupTimeEditForm form)
			: base() {
			Form = form;
		}
		public override bool ShowTime() {
			return true;
		}
		protected override void CheckContainer() {
			if(PickContainer.IsReady)
				OnDateChanged();
		}
		public override void AddNewProvider(DateTimeMaskFormatElementEditable editableFormat) {
			IItemsProvider provider = CreateNewProvider(editableFormat);
			if(provider != null) {
				if(ShouldInsertProvider(provider))
					Providers.Insert(0, provider);
				else Providers.Add(provider);
				TotalProviders += 1;
			}
		}
		protected bool ShouldInsertProvider(IItemsProvider provider) {
			return provider is MeridiemItemsProvider && OwnerEdit != null && OwnerEdit.IsRightToLeft;
		}
		public override void UpdateMaskManager() {
			if(Form == null || Form.TimeEdit == null)
				return;
			MaskManager = new DateTimeMaskManager(Form.TimeEdit.Properties.Mask.EditMask, true, CultureInfo.CurrentCulture, false);
		}
		protected override void OnSizeChanged(EventArgs e) {
			base.OnSizeChanged(e);
			SetDate();
		}
		protected internal override int GetSecondIncrement() {
			return OwnerEdit.Properties.TouchUISecondIncrement;
		}
		protected internal override int GetMinuteIncrement() {
			return OwnerEdit.Properties.TouchUIMinuteIncrement;
		}
	}  
}
