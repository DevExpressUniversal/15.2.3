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

using DevExpress.Web;
using System;
using System.ComponentModel;
namespace DevExpress.Web.Mvc {
	public class MVCxBinaryImageEditProperties: BinaryImageEditProperties {
		public MVCxBinaryImageEditProperties() {
		}
		public MVCxBinaryImageEditProperties(IPropertiesOwner owner)
			: base(owner) {
		}
		[Obsolete("Use the BinaryImageEditSettings.ControlStyle property instead."),
		Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public new EditStyleBase Style { get { return base.Style; } }
		[Obsolete("Use the BinaryImageEditSettings.EncodeHtml property instead."),
		Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public new bool EncodeHtml { get { return base.EncodeHtml; } set { base.EncodeHtml = value; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public new bool StoreContentBytesInViewState { get { return false; } set { } }
		public new string Caption {
			get { return base.Caption; }
			set { base.Caption = value; }
		}
		public new EditorCaptionSettingsBase CaptionSettings {
			get { return base.CaptionSettings; }
		}
		public new EditorCaptionCellStyle CaptionCellStyle {
			get { return base.CaptionCellStyle; }
		}
		public new EditorCaptionStyle CaptionStyle {
			get { return base.CaptionStyle; }
		}
		public new EditorRootStyle RootStyle {
			get { return base.RootStyle; }
		}
		public object CallbackRouteValues { get; set; }
		protected override ASPxEditBase CreateEditInstance() { 
			return new MVCxBinaryImage(); 
		}
		public override void Assign(PropertiesBase source) {
			base.Assign(source);
			var properties = source as MVCxBinaryImageEditProperties;
			if(properties != null)
				CallbackRouteValues = properties.CallbackRouteValues;
		}
	}
	public class MVCxButtonEditProperties: ButtonEditProperties {
		[Obsolete("Use the ButtonEditSettings.ControlStyle property instead."),
		Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public new EditStyleBase Style { get { return base.Style; } }
		[Obsolete("Use the ButtonEditSettings.EncodeHtml property instead."),
		Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public new bool EncodeHtml { get { return base.EncodeHtml; } set { base.EncodeHtml = value; } }
		public new string Caption {
			get { return base.Caption; }
			set { base.Caption = value; }
		}
		public new EditorCaptionSettings CaptionSettings {
			get { return base.CaptionSettings; }
		}
		public new EditorCaptionCellStyle CaptionCellStyle {
			get { return base.CaptionCellStyle; }
		}
		public new EditorCaptionStyle CaptionStyle {
			get { return base.CaptionStyle; }
		}
		public new EditorRootStyle RootStyle {
			get { return base.RootStyle; }
		}
	}
	public class MVCxCalendarProperties: CalendarProperties {
		public MVCxCalendarProperties()
			: base() {
			DisplayFormatString = string.Empty;
		}
		[Obsolete("Use the CalendarSettings.ControlStyle property instead."),
		Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public new EditStyleBase Style { get { return base.Style; } }
		[Obsolete("Use the CalendarSettings.EncodeHtml property instead."),
		Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public new bool EncodeHtml { get { return base.EncodeHtml; } set { base.EncodeHtml = value; } }
		public new string Caption {
			get { return base.Caption; }
			set { base.Caption = value; }
		}
		public new EditorCaptionSettings CaptionSettings {
			get { return base.CaptionSettings; }
		}
		public new EditorCaptionCellStyle CaptionCellStyle {
			get { return base.CaptionCellStyle; }
		}
		public new EditorCaptionStyle CaptionStyle {
			get { return base.CaptionStyle; }
		}
		public new EditorRootStyle RootStyle {
			get { return base.RootStyle; }
		}
	}
	public class MVCxCheckBoxProperties: CheckBoxProperties {
		[Obsolete("Use the CheckBoxSettings.ControlStyle property instead."),
		Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public new EditStyleBase Style { get { return base.Style; } }
		[Obsolete("Use the CheckBoxSettings.EncodeHtml property instead."),
		Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public new bool EncodeHtml { get { return base.EncodeHtml; } set { base.EncodeHtml = value; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public new string Caption {
			get { return base.Caption; }
			set { base.Caption = value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public new EditorCaptionSettings CaptionSettings {
			get { return base.CaptionSettings; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public new EditorCaptionCellStyle CaptionCellStyle {
			get { return base.CaptionCellStyle; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public new EditorCaptionStyle CaptionStyle {
			get { return base.CaptionStyle; }
		}
		public new EditorRootStyle RootStyle {
			get { return base.RootStyle; }
		}
	}
	public class MVCxCheckBoxListProperties: CheckBoxListProperties {
		[Obsolete("Use the CheckBoxListSettings.ControlStyle property instead."),
		Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public new EditStyleBase Style { get { return base.Style; } }
		[Obsolete("Use the CheckBoxListSettings.EncodeHtml property instead."),
		Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public new bool EncodeHtml { get { return base.EncodeHtml; } set { base.EncodeHtml = value; } }
		public new string Caption {
			get { return base.Caption; }
			set { base.Caption = value; }
		}
		public new EditorCaptionSettings CaptionSettings {
			get { return base.CaptionSettings; }
		}
		public new EditorCaptionCellStyle CaptionCellStyle {
			get { return base.CaptionCellStyle; }
		}
		public new EditorCaptionStyle CaptionStyle {
			get { return base.CaptionStyle; }
		}
		public new EditorRootStyle RootStyle {
			get { return base.RootStyle; }
		}
	}
	public class MVCxColorEditProperties: ColorEditProperties {
		[Obsolete("Use the ColorEditSettings.ControlStyle property instead."),
		Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public new EditStyleBase Style { get { return base.Style; } }
		[Obsolete("Use the ColorEditSettings.EncodeHtml property instead."),
		Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public new bool EncodeHtml { get { return base.EncodeHtml; } set { base.EncodeHtml = value; } }
		public new string Caption {
			get { return base.Caption; }
			set { base.Caption = value; }
		}
		public new EditorCaptionSettings CaptionSettings {
			get { return base.CaptionSettings; }
		}
		public new EditorCaptionCellStyle CaptionCellStyle {
			get { return base.CaptionCellStyle; }
		}
		public new EditorCaptionStyle CaptionStyle {
			get { return base.CaptionStyle; }
		}
		public new EditorRootStyle RootStyle {
			get { return base.RootStyle; }
		}
	}
	public class MVCxComboBoxProperties: ComboBoxProperties {
		[Obsolete("Use the ComboBoxSettings.CallbackRouteValues property instead."),
		Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public new bool EnableCallbackMode { get { return base.EnableCallbackMode; } set { base.EnableCallbackMode = value; } }
		[Obsolete("Use the ComboBoxSettings.ControlStyle property instead."),
		Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public new EditStyleBase Style { get { return base.Style; } }
		[Obsolete("Use the ComboBoxSettings.EncodeHtml property instead."),
		Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public new bool EncodeHtml { get { return base.EncodeHtml; } set { base.EncodeHtml = value; } }
		public new string Caption {
			get { return base.Caption; }
			set { base.Caption = value; }
		}
		public new EditorCaptionSettings CaptionSettings {
			get { return base.CaptionSettings; }
		}
		public new EditorCaptionCellStyle CaptionCellStyle {
			get { return base.CaptionCellStyle; }
		}
		public new EditorCaptionStyle CaptionStyle {
			get { return base.CaptionStyle; }
		}
		public new EditorRootStyle RootStyle {
			get { return base.RootStyle; }
		}
	}
	public class MVCxDateEditProperties: DateEditProperties {
		public MVCxDateEditProperties()
			: base() {
			DisplayFormatString = string.Empty;
		}
		[Obsolete("Use the DateEditSettings.ControlStyle property instead."),
		Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public new EditStyleBase Style { get { return base.Style; } }
		[Obsolete("Use the DateEditSettings.EncodeHtml property instead."),
		Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public new bool EncodeHtml { get { return base.EncodeHtml; } set { base.EncodeHtml = value; } }
		public new string Caption {
			get { return base.Caption; }
			set { base.Caption = value; }
		}
		public new EditorCaptionSettings CaptionSettings {
			get { return base.CaptionSettings; }
		}
		public new EditorCaptionCellStyle CaptionCellStyle {
			get { return base.CaptionCellStyle; }
		}
		public new EditorCaptionStyle CaptionStyle {
			get { return base.CaptionStyle; }
		}
		public new EditorRootStyle RootStyle {
			get { return base.RootStyle; }
		}
		public new MVCxDateEditRangeSettings DateRangeSettings {
			get { return base.DateRangeSettings as MVCxDateEditRangeSettings; }
		}
		protected override DateEditRangeSettings CreateDateRangeSettings() {
			return new MVCxDateEditRangeSettings(this);
		}
	}
	public class MVCxDropDownEditProperties: DropDownEditProperties {
		[Obsolete("Use the DropDownEditSettings.ControlStyle property instead."),
		Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public new EditStyleBase Style { get { return base.Style; } }
		[Obsolete("Use the DropDownEditSettings.EncodeHtml property instead."),
		Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public new bool EncodeHtml { get { return base.EncodeHtml; } set { base.EncodeHtml = value; } }
		public new string Caption {
			get { return base.Caption; }
			set { base.Caption = value; }
		}
		public new EditorCaptionSettings CaptionSettings {
			get { return base.CaptionSettings; }
		}
		public new EditorCaptionCellStyle CaptionCellStyle {
			get { return base.CaptionCellStyle; }
		}
		public new EditorCaptionStyle CaptionStyle {
			get { return base.CaptionStyle; }
		}
		public new EditorRootStyle RootStyle {
			get { return base.RootStyle; }
		}
	}
	public class MVCxHyperLinkProperties: HyperLinkProperties {
		[Obsolete("Use the HyperLinkSettings.ControlStyle property instead."),
		Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public new EditStyleBase Style { get { return base.Style; } }
		[Obsolete("Use the HyperLinkSettings.EncodeHtml property instead."),
		Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public new bool EncodeHtml { get { return base.EncodeHtml; } set { base.EncodeHtml = value; } }
	}
	public class MVCxImageEditProperties: ImageEditProperties {
		[Obsolete("Use the ImageEditSettings.ControlStyle property instead."),
		Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public new EditStyleBase Style { get { return base.Style; } }
		[Obsolete("Use the ImageEditSettings.EncodeHtml property instead."),
		Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public new bool EncodeHtml { get { return base.EncodeHtml; } set { base.EncodeHtml = value; } }
		public new string Caption {
			get { return base.Caption; }
			set { base.Caption = value; }
		}
		public new EditorCaptionSettingsBase CaptionSettings {
			get { return base.CaptionSettings; }
		}
		public new EditorCaptionCellStyle CaptionCellStyle {
			get { return base.CaptionCellStyle; }
		}
		public new EditorCaptionStyle CaptionStyle {
			get { return base.CaptionStyle; }
		}
		public new EditorRootStyle RootStyle {
			get { return base.RootStyle; }
		}
	}
	public class MVCxLabelProperties: LabelProperties {
		[Obsolete("Use the LabelSettings.ControlStyle property instead."),
		Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public new EditStyleBase Style { get { return base.Style; } }
		[Obsolete("Use the MVCxLabelProperties.EncodeHtml property instead."),
		Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public new bool EncodeHtml { get { return base.EncodeHtml; } set { base.EncodeHtml = value; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public new string DisplayFormatString { 
			get { return base.DisplayFormatString; }
			set { base.DisplayFormatString = value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public new string NullDisplayText {
			get { return base.NullDisplayText; }
			set { base.NullDisplayText = value; }
		}
	}
	public class MVCxListBoxProperties: ListBoxProperties {
		[Obsolete("Use the ListBoxSettings.CallbackRouteValues property instead."),
		Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public new bool EnableCallbackMode { get { return base.EnableCallbackMode; } set { base.EnableCallbackMode = value; } }
		[Obsolete("Use the ListBoxSettings.ControlStyle property instead."),
		Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public new EditStyleBase Style { get { return base.Style; } }
		[Obsolete("Use the ListBoxSettings.EncodeHtml property instead."),
		Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public new bool EncodeHtml { get { return base.EncodeHtml; } set { base.EncodeHtml = value; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public new string NullDisplayText { get { return base.NullDisplayText; } set { base.NullDisplayText = value; } }
		public new string Caption {
			get { return base.Caption; }
			set { base.Caption = value; }
		}
		public new EditorCaptionSettings CaptionSettings {
			get { return base.CaptionSettings; }
		}
		public new EditorCaptionCellStyle CaptionCellStyle {
			get { return base.CaptionCellStyle; }
		}
		public new EditorCaptionStyle CaptionStyle {
			get { return base.CaptionStyle; }
		}
		public new EditorRootStyle RootStyle {
			get { return base.RootStyle; }
		}
	}
	public class MVCxMemoProperties: MemoProperties {
		[Obsolete("Use the MemoSettings.ControlStyle property instead."),
		Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public new EditStyleBase Style { get { return base.Style; } }
		[Obsolete("Use the MemoSettings.EncodeHtml property instead."),
		Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public new bool EncodeHtml { get { return base.EncodeHtml; } set { base.EncodeHtml = value; } }
		public new string Caption {
			get { return base.Caption; }
			set { base.Caption = value; }
		}
		public new EditorCaptionSettings CaptionSettings {
			get { return base.CaptionSettings; }
		}
		public new EditorCaptionCellStyle CaptionCellStyle {
			get { return base.CaptionCellStyle; }
		}
		public new EditorCaptionStyle CaptionStyle {
			get { return base.CaptionStyle; }
		}
		public new EditorRootStyle RootStyle {
			get { return base.RootStyle; }
		}
	}
	public class MVCxProgressBarProperties: ProgressBarProperties {
		[Obsolete("Use the ProgressBarSettings.ControlStyle property instead."),
		Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public new EditStyleBase Style { get { return base.Style; } }
		[Obsolete("Use the ProgressBarSettings.EncodeHtml property instead."),
		Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public new bool EncodeHtml { get { return base.EncodeHtml; } set { base.EncodeHtml = value; } }
		public new string Caption {
			get { return base.Caption; }
			set { base.Caption = value; }
		}
		public new EditorCaptionSettingsBase CaptionSettings {
			get { return base.CaptionSettings; }
		}
		public new EditorCaptionCellStyle CaptionCellStyle {
			get { return base.CaptionCellStyle; }
		}
		public new EditorCaptionStyle CaptionStyle {
			get { return base.CaptionStyle; }
		}
		public new EditorRootStyle RootStyle {
			get { return base.RootStyle; }
		}
	}
	public class MVCxRadioButtonProperties: RadioButtonProperties {
		[Obsolete("Use the RadioButtonSettings.ControlStyle property instead."),
		Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public new EditStyleBase Style { get { return base.Style; } }
		[Obsolete("Use the RadioButtonSettings.EncodeHtml property instead."),
		Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public new bool EncodeHtml { get { return base.EncodeHtml; } set { base.EncodeHtml = value; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public new string Caption {
			get { return base.Caption; }
			set { base.Caption = value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public new EditorCaptionSettings CaptionSettings {
			get { return base.CaptionSettings; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public new EditorCaptionCellStyle CaptionCellStyle {
			get { return base.CaptionCellStyle; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public new EditorCaptionStyle CaptionStyle {
			get { return base.CaptionStyle; }
		}
		public new EditorRootStyle RootStyle {
			get { return base.RootStyle; }
		}
	}
	public class MVCxRadioButtonListProperties: RadioButtonListProperties {
		[Obsolete("Use the RadioButtonListSettings.ControlStyle property instead."),
		Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public new EditStyleBase Style { get { return base.Style; } }
		[Obsolete("Use the RadioButtonListSettings.EncodeHtml property instead."),
		Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public new bool EncodeHtml { get { return base.EncodeHtml; } set { base.EncodeHtml = value; } }
		public new string Caption {
			get { return base.Caption; }
			set { base.Caption = value; }
		}
		public new EditorCaptionSettings CaptionSettings {
			get { return base.CaptionSettings; }
		}
		public new EditorCaptionCellStyle CaptionCellStyle {
			get { return base.CaptionCellStyle; }
		}
		public new EditorCaptionStyle CaptionStyle {
			get { return base.CaptionStyle; }
		}
		public new EditorRootStyle RootStyle {
			get { return base.RootStyle; }
		}
	}
	public class MVCxSpinEditProperties: SpinEditProperties {
		public MVCxSpinEditProperties()
			: base() {
			DisplayFormatString = string.Empty;
		}
		[Obsolete("Use the SpinEditSettings.Style property instead."),
		Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public new EditStyleBase Style { get { return base.Style; } }
		[Obsolete("Use the SpinEditSettings.EncodeHtml property instead."),
		Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public new bool EncodeHtml { get { return base.EncodeHtml; } set { base.EncodeHtml = value; } }
		public new string Caption {
			get { return base.Caption; }
			set { base.Caption = value; }
		}
		public new EditorCaptionSettings CaptionSettings {
			get { return base.CaptionSettings; }
		}
		public new EditorCaptionCellStyle CaptionCellStyle {
			get { return base.CaptionCellStyle; }
		}
		public new EditorCaptionStyle CaptionStyle {
			get { return base.CaptionStyle; }
		}
		public new EditorRootStyle RootStyle {
			get { return base.RootStyle; }
		}
	}
	public class MVCxTextBoxProperties: TextBoxProperties {
		[Obsolete("Use the TextBoxSettings.Style property instead."),
		Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public new EditStyleBase Style { get { return base.Style; } }
		[Obsolete("Use the TextBoxSettings.EncodeHtml property instead."),
		Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public new bool EncodeHtml { get { return base.EncodeHtml; } set { base.EncodeHtml = value; } }
		public new string Caption {
			get { return base.Caption; }
			set { base.Caption = value; }
		}
		public new EditorCaptionSettings CaptionSettings {
			get { return base.CaptionSettings; }
		}
		public new EditorCaptionCellStyle CaptionCellStyle {
			get { return base.CaptionCellStyle; }
		}
		public new EditorCaptionStyle CaptionStyle {
			get { return base.CaptionStyle; }
		}
		public new EditorRootStyle RootStyle {
			get { return base.RootStyle; }
		}
	}
	public class MVCxTimeEditProperties: TimeEditProperties {
		public MVCxTimeEditProperties()
			: base() {
			DisplayFormatString = string.Empty;
		}
		[Obsolete("Use the TimeEditSettings.Style property instead."),
		Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public new EditStyleBase Style { get { return base.Style; } }
		[Obsolete("Use the TimeEditSettings.EncodeHtml property instead."),
		Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public new bool EncodeHtml { get { return base.EncodeHtml; } set { base.EncodeHtml = value; } }
		public new string Caption {
			get { return base.Caption; }
			set { base.Caption = value; }
		}
		public new EditorCaptionSettings CaptionSettings {
			get { return base.CaptionSettings; }
		}
		public new EditorCaptionCellStyle CaptionCellStyle {
			get { return base.CaptionCellStyle; }
		}
		public new EditorCaptionStyle CaptionStyle {
			get { return base.CaptionStyle; }
		}
		public new EditorRootStyle RootStyle {
			get { return base.RootStyle; }
		}
	}
	public class MVCxTokenBoxProperties : TokenBoxProperties {
		[Obsolete("Use the TokenBoxSettings.CallbackRouteValues property instead."),
		Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public new bool EnableCallbackMode { get { return base.EnableCallbackMode; } set { base.EnableCallbackMode = value; } }
		[Obsolete("Use the TokenBoxSettings.ControlStyle property instead."),
		Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public new EditStyleBase Style { get { return base.Style; } }
		[Obsolete("Use the TokenBoxSettings.EncodeHtml property instead."),
		Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public new bool EncodeHtml { get { return base.EncodeHtml; } set { base.EncodeHtml = value; } }
		public new string Caption {
			get { return base.Caption; }
			set { base.Caption = value; }
		}
		public new EditorCaptionSettings CaptionSettings {
			get { return base.CaptionSettings; }
		}
		public new EditorCaptionCellStyle CaptionCellStyle {
			get { return base.CaptionCellStyle; }
		}
		public new EditorCaptionStyle CaptionStyle {
			get { return base.CaptionStyle; }
		}
		public new EditorRootStyle RootStyle {
			get { return base.RootStyle; }
		}
	}
	public class MVCxTrackBarProperties: TrackBarProperties {
		public MVCxTrackBarProperties()
			: base(null) {
		}
		[Obsolete("Use the TrackBarSettings.Style property instead."),
		Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public new EditStyleBase Style { get { return base.Style; } }
		[Obsolete("Use the TrackBarSettings.EncodeHtml property instead."),
		Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public new bool EncodeHtml { get { return base.EncodeHtml; } set { base.EncodeHtml = value; } }
		public new string Caption {
			get { return base.Caption; }
			set { base.Caption = value; }
		}
		public new EditorCaptionSettings CaptionSettings {
			get { return base.CaptionSettings; }
		}
		public new EditorCaptionCellStyle CaptionCellStyle {
			get { return base.CaptionCellStyle; }
		}
		public new EditorCaptionStyle CaptionStyle {
			get { return base.CaptionStyle; }
		}
		public new EditorRootStyle RootStyle {
			get { return base.RootStyle; }
		}
	}
}
