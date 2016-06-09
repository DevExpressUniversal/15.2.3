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
using System.Text;
using DevExpress.Web;
using DevExpress.Web.Internal;
using System.ComponentModel;
using System.Globalization;
using System.Web.UI;
using System.Collections.Specialized;
namespace DevExpress.Web {
	public class TimeEditProperties : SpinEditPropertiesBase {
		bool formatEnumTouched;
		public TimeEditProperties()
			: this(null) {
		}
		public TimeEditProperties(IPropertiesOwner owner)
			: base(owner) {
			SyncMaskAndEditFormat();
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("TimeEditPropertiesEditFormat"),
#endif
		NotifyParentProperty(true), DefaultValue(EditFormat.Time), AutoFormatDisable]
		public EditFormat EditFormat {
			get { return (EditFormat)GetIntProperty("EditFormat", (int)EditFormat.Time); }
			set {
				if(value == EditFormat)
					return;
				SetIntProperty("EditFormat", (int)EditFormat.Time, (int)value);
				this.formatEnumTouched = true;
				SyncMaskAndEditFormat();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("TimeEditPropertiesEditFormatString"),
#endif
		NotifyParentProperty(true), DefaultValue(""), Localizable(true), AutoFormatDisable, RefreshProperties(RefreshProperties.Repaint)]
		public string EditFormatString {
			get { return GetStringProperty("EditFormatString", ""); }
			set {
				if(value == EditFormatString)
					return;
				SetStringProperty("EditFormatString", "", value);
				CustomEditFormatAssigned(value);
				SyncMaskAndEditFormat();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("TimeEditPropertiesDisplayFormatString"),
#endif
		NotifyParentProperty(true), DefaultValue("t"), Localizable(true), AutoFormatDisable]
		public override string DisplayFormatString {
			get { return base.DisplayFormatString; }
			set { base.DisplayFormatString = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("TimeEditPropertiesAllowNull"),
#endif
		Category("Behavior"), NotifyParentProperty(true), DefaultValue(true), AutoFormatDisable]
		public bool AllowNull {
			get { return GetBoolProperty("AllowNull", true); }
			set { SetBoolProperty("AllowNull", true, value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("TimeEditPropertiesNullText"),
#endif
		DefaultValue(""), AutoFormatDisable, NotifyParentProperty(true), Localizable(true)]
		public string NullText {
			get { return NullTextInternal; }
			set { NullTextInternal = value; }
		}
		#region Hide non-usable props
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override int MaxLength { get { return base.MaxLength; } set { base.MaxLength = value; } }
		#endregion
		[
#if !SL
	DevExpressWebLocalizedDescription("TimeEditPropertiesSpinButtons"),
#endif
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		PersistenceMode(PersistenceMode.InnerProperty), NotifyParentProperty(true), AutoFormatEnable]
		public SimpleSpinButtons SpinButtons {
			get { return (SimpleSpinButtons)SpinButtonsInternal; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("TimeEditPropertiesClientSideEvents"),
#endif
		Category("Client-Side"), DesignerSerializationVisibility(DesignerSerializationVisibility.Content), AutoFormatDisable,
		PersistenceMode(PersistenceMode.InnerProperty), NotifyParentProperty(true)]
		public new TimeEditClientSideEvents ClientSideEvents {
			get { return base.ClientSideEvents as TimeEditClientSideEvents; }
		}
		protected override string DefaultDisplayFormatString {
			get { return "t"; }
		}
		protected override ASPxEditBase CreateEditInstance() {
			return new ASPxTimeEdit();
		}
		protected override EditClientSideEventsBase CreateClientSideEvents() {
			return new TimeEditClientSideEvents(this);
		}
		protected override SpinButtons CreateSpinButtons() {
			return new SimpleSpinButtons(this);
		}
		protected override MaskSettings CreateMaskSettings() {
			MaskSettings settings = new MaskSettings(this);
			settings.IsDateTimeOnly = true;
			return settings;
		}
		protected void SyncMaskAndEditFormat() {
			MaskSettingsInternal.Mask = GetDateFormatString();
		}
		protected internal string GetDateFormatString() {
			return DateTimeEditUtils.GetDateFormatString(EditFormat, EditFormatString,
				CultureInfo.CurrentCulture.DateTimeFormat.ShortTimePattern);
		}
		public override void Assign(PropertiesBase source) {
			base.Assign(source);
			TimeEditProperties src = source as TimeEditProperties;
			if(src != null) {
				AllowNull = src.AllowNull;
				EditFormat = src.EditFormat;
				EditFormatString = src.EditFormatString;
			}
		}
		void CustomEditFormatAssigned(string value) {
			if(!IsDesignMode() || String.IsNullOrEmpty(value) || this.formatEnumTouched)
				return;
			EditFormat = EditFormat.Custom;
			DateTimeEditUtils.NotifyOwnerComponentChanged(Owner);
		}
		protected internal override bool IsClearButtonVisibleAuto() {
			return AllowNull && base.IsClearButtonVisibleAuto();
		}
	}
	[DXWebToolboxItem(DXToolboxItemKind.Free),
	DefaultProperty("DateTime"), DefaultEvent("DateChanged"), ControlValueProperty("DateTime"),
	ValidationProperty("Text"), DevExpress.Utils.ToolboxTabName(AssemblyInfo.DXTabCommon),
	System.Drawing.ToolboxBitmap(typeof(ToolboxBitmapAccess), ToolboxBitmapAccess.BitmapPath + "ASPxTimeEdit.bmp"),
	Designer("DevExpress.Web.Design.ASPxTimeEditDesigner, " + AssemblyInfo.SRAssemblyWebDesignFull)]
	public class ASPxTimeEdit : ASPxSpinEditBase {
		public static readonly object DateChangedEvent = new object();
		public ASPxTimeEdit()
			: base() {
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxTimeEditEditFormat"),
#endif
		DefaultValue(EditFormat.Time), AutoFormatDisable]
		public EditFormat EditFormat {
			get { return Properties.EditFormat; }
			set { Properties.EditFormat = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxTimeEditEditFormatString"),
#endif
		DefaultValue(""), Localizable(true), AutoFormatDisable]
		public string EditFormatString {
			get { return Properties.EditFormatString; }
			set { Properties.EditFormatString = value; }
		}
#if !SL
	[DevExpressWebLocalizedDescription("ASPxTimeEditValue")]
#endif
		public override object Value {
			get { return base.Value; }
			set { base.Value = DateTimeEditUtils.PreprocessValueProperty(value); }
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		Bindable(false), AutoFormatDisable, Localizable(false)]
		public override string Text {
			get { return DateTimeEditUtils.GetTextProperty(this, Properties.GetDateFormatString(), base.Text); }
			set { DateTimeEditUtils.SetTextProperty(this, value, Properties.GetDateFormatString()); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxTimeEditDateTime"),
#endif
		DefaultValue(typeof(DateTime), ""), AutoFormatDisable]
		public DateTime DateTime {
			get { return DateTimeEditUtils.GetDateProperty(this); }
			set { DateTimeEditUtils.SetDateProperty(this, value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxTimeEditAllowNull"),
#endif
		Category("Behavior"), DefaultValue(true), AutoFormatDisable]
		public bool AllowNull {
			get { return Properties.AllowNull; }
			set { Properties.AllowNull = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxTimeEditNullText"),
#endif
		DefaultValue(""), AutoFormatDisable, Localizable(true)]
		public string NullText {
			get { return Properties.NullText; }
			set { Properties.NullText = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxTimeEditSpinButtons"),
#endif
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable]
		public SimpleSpinButtons SpinButtons {
			get { return Properties.SpinButtons; }
		}
		#region Hide non-usable props
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override int MaxLength { get { return base.MaxLength; } set { base.MaxLength = value; } }
		#endregion
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxTimeEditClientSideEvents"),
#endif
		Category("Client-Side"), AutoFormatDisable,
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		PersistenceMode(PersistenceMode.InnerProperty)]
		public new TimeEditClientSideEvents ClientSideEvents {
			get { return Properties.ClientSideEvents; }
		}
		protected internal new TimeEditProperties Properties {
			get { return base.Properties as TimeEditProperties; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxTimeEditDateChanged"),
#endif
		Category("Action")]
		public event EventHandler DateChanged
		{
			add { Events.AddHandler(DateChangedEvent, value); }
			remove { Events.RemoveHandler(DateChangedEvent, value); }
		}
		protected override EditPropertiesBase CreateProperties() {
			return new TimeEditProperties(this);
		}
		protected override string GetClientObjectClassName() {
			return "ASPxClientTimeEdit";
		}
		protected override void GetCreateClientObjectScript(StringBuilder stb, string localVarName, string clientName) {
			base.GetCreateClientObjectScript(stb, localVarName, clientName);
			if(DateTime > DateTime.MinValue)
				stb.AppendFormat(localVarName + ".date = " + HtmlConvertor.ToScript(DateTime) + ";\n");
			if (!AllowNull)
				stb.AppendFormat("{0}.allowNull = false;\n", localVarName);
		}
		public override void RegisterEditorIncludeScripts() {
			base.RegisterEditorIncludeScripts();
			RegisterDateFormatterScript();
		}
		protected override void RegisterScriptBlocks() {
			base.RegisterScriptBlocks();
			RegisterCultureInfoScript();
		}
		protected override string GetFormattedInputText() {
			if(DateTime == DateTime.MinValue)
				return "";
			return String.Format(CommonUtils.GetFormatString(DisplayFormatString), DateTime);
		}
		protected override bool RequireRenderRawInput() {
			return true;
		}
		protected override string GetRawValue() {
			return DateTimeEditUtils.GetRawValue(DateTime);
		}
		protected override object GetPostBackValue(string controlName, NameValueCollection postCollection) {
			if(ClientObjectState == null)
				return Value;
			return DateTimeEditUtils.ParseRawInputValue(GetClientObjectStateValueString(RawValueKey));
		}
		public void OnDateChanged(EventArgs e) {
			EventHandler handler = Events[DateChangedEvent] as EventHandler;
			if(handler != null)
				handler(this, e);
		}
		protected override void RaiseValueChanged() {
			base.RaiseValueChanged();
			OnDateChanged(EventArgs.Empty);
		}
		protected override bool IsMaskValidationPatternRequired() {
			return false;
		}
		protected override bool NeedNullTextStyle() {
			return IsEnabled();
		}
	}
}
