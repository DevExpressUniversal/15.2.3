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
using System.ComponentModel;
using System.Web.UI;
using DevExpress.Web.Internal;
using DevExpress.Web.Localization;
namespace DevExpress.Web.Captcha {
	public class CaptchaValidationSettings : PropertiesBase, IValidationSettings {
		ErrorFrameStyle errorFrameStyle;
		ImageProperties errorImage;
		ValidationPatterns validationPatterns;
		public CaptchaValidationSettings() {
			Initialize();
		}
		public CaptchaValidationSettings(ASPxCaptcha owner)
			: base(owner) {
			Initialize();
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("CaptchaValidationSettingsEnableValidation"),
#endif
		NotifyParentProperty(true), DefaultValue(true), AutoFormatDisable]
		public bool EnableValidation {
			get { return GetBoolProperty("EnableValidation", true); }
			set {
				SetBoolProperty("EnableValidation", true, value);
				Changed();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("CaptchaValidationSettingsErrorDisplayMode"),
#endif
		NotifyParentProperty(true), DefaultValue(ErrorDisplayMode.Text), AutoFormatDisable]
		public ErrorDisplayMode ErrorDisplayMode {
			get { return (ErrorDisplayMode)GetEnumProperty("ErrorDisplayMode", ErrorDisplayMode.Text); }
			set {
				SetEnumProperty("ErrorDisplayMode", ErrorDisplayMode.Text, value);
				Changed();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("CaptchaValidationSettingsErrorImage"),
#endif
		NotifyParentProperty(true), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public ImageProperties ErrorImage {
			get { return errorImage; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("CaptchaValidationSettingsErrorText"),
#endif
		NotifyParentProperty(true), AutoFormatEnable, DefaultValue(StringResources.Captcha_DefaultErrorText),
		Localizable(true)]
		public virtual string ErrorText {
			get {
				string errorTextFromContext = GetErrorTextFromContext();
				if (!string.IsNullOrEmpty(errorTextFromContext))
					return errorTextFromContext;
				return GetStringProperty("ErrorText",
					ASPxEditorsLocalizer.GetString(ASPxEditorsStringId.Captcha_DefaultErrorText));
			}
			set {
				SetErrorTextInContext(value);
				SetStringProperty("ErrorText",
					ASPxEditorsLocalizer.GetString(ASPxEditorsStringId.Captcha_DefaultErrorText), value);
				Changed();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("CaptchaValidationSettingsErrorFrameStyle"),
#endif
		NotifyParentProperty(true), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public ErrorFrameStyle ErrorFrameStyle {
			get { return errorFrameStyle; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("CaptchaValidationSettingsValidationGroup"),
#endif
		NotifyParentProperty(true), DefaultValue(""), Localizable(false), AutoFormatDisable]
		public string ValidationGroup {
			get { return GetStringProperty("ValidationGroup", string.Empty); }
			set { SetStringProperty("ValidationGroup", string.Empty, value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("CaptchaValidationSettingsRequiredField"),
#endif
		NotifyParentProperty(true), PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content), AutoFormatDisable]
		public RequiredFieldValidationPattern RequiredField {
			get { return this.validationPatterns.RequiredField; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("CaptchaValidationSettingsDisplay"),
#endif
		NotifyParentProperty(true), DefaultValue(Display.Static), AutoFormatDisable]
		public Display Display {
			get {
				if (!ValidationSummaryCollection.Instance.EditorsAllowedToShowErrors(ValidationGroup))
					return Display.None;
				else
					return (Display)GetEnumProperty("Display", Display.Static);
			}
			set {
				SetEnumProperty("Display", Display.Static, value);
				Changed();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("CaptchaValidationSettingsSetFocusOnError"),
#endif
		NotifyParentProperty(true), DefaultValue(false), AutoFormatDisable,
		Localizable(false)]
		public bool SetFocusOnError
		{
			get { return GetBoolProperty("SetFocusOnError", false); }
			set { SetBoolProperty("SetFocusOnError", false, value); }
		}
		protected void Initialize() {
			this.errorFrameStyle = new ErrorFrameStyle();
			this.errorImage = new ImageProperties();
			this.validationPatterns = new ValidationPatterns(this);
		}
		protected string GetErrorTextContextKey(ASPxCaptcha owner) {
			return owner.UniqueID + "_ErrorText";
		}
		protected string GetErrorTextFromContext() {
			ASPxCaptcha owner = Owner as ASPxCaptcha;
			if (owner == null)
				return null;
			string key = GetErrorTextContextKey(owner);
			return HttpUtils.GetContextValue(key, string.Empty);
		}
		protected void SetErrorTextInContext(string errorText) {
			ASPxCaptcha owner = Owner as ASPxCaptcha;
			if (owner == null)
				return;
			string key = GetErrorTextContextKey(owner);
			HttpUtils.SetContextValue(key, errorText);
		}
		protected internal void AssignToControl(ASPxTextBox textBox) {
			textBox.ValidationSettings.ErrorDisplayMode = ErrorDisplayMode;
			textBox.ValidationSettings.ErrorTextPosition = ErrorTextPosition.Bottom;
			textBox.ValidationSettings.ErrorText = ErrorText;
			textBox.ValidationSettings.ValidationGroup = ValidationGroup;
			textBox.ValidationSettings.ErrorImage.Assign(ErrorImage);
			textBox.ValidationSettings.ErrorFrameStyle.Assign(ErrorFrameStyle);
			textBox.ValidationSettings.RequiredField.Assign(RequiredField);
			textBox.ValidationSettings.Display = Display;
			textBox.ValidationSettings.SetFocusOnError = SetFocusOnError;
		}
		protected override IStateManager[] GetStateManagedObjects() {
			return ViewStateUtils.GetMergedStateManagedObjects(base.GetStateManagedObjects(),
				new IStateManager[] { ErrorFrameStyle, ErrorImage, this.validationPatterns });
		}
		void IPropertiesOwner.Changed(PropertiesBase properties) {
			Changed();
		}
		public override void Assign(PropertiesBase source) {
			BeginUpdate();
			try {
				base.Assign(source);
				CaptchaValidationSettings src = source as CaptchaValidationSettings;
				if (src == null)
					return;
				EnableValidation = src.EnableValidation;
				ErrorDisplayMode = src.ErrorDisplayMode;
				ErrorText = src.ErrorText;
				ValidationGroup = src.ValidationGroup;
				Display = src.Display;
				SetFocusOnError = src.SetFocusOnError;
				ErrorImage.Assign(src.ErrorImage);
				ErrorFrameStyle.Assign(src.ErrorFrameStyle);
				RequiredField.Assign(src.RequiredField);
			} finally {
				EndUpdate();
			}
		}
	}
}
