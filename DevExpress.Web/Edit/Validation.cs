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
using System.ComponentModel;
using System.Collections;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.Design;
using System.Web.UI.WebControls;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections.Specialized;
using DevExpress.Web;
using DevExpress.Web.Internal;
namespace DevExpress.Web {
	using DevExpress.Web.Internal;
	using DevExpress.Web.Localization;
	public class ValidationEventArgs : EventArgs {
		private string errorText;
		private bool isValid;
		private object value;
		public string ErrorText {
			get { return errorText; }
			set { errorText = value; }
		}
		public bool IsValid {
			get { return this.isValid; }
			set { this.isValid = value; }
		}
		public object Value {
			get { return this.value; }
			set { this.value = value; }
		}
		public ValidationEventArgs(object value, string errorText, bool isValid)
			: base() {
			this.value = value;
			this.errorText = errorText;
			this.isValid = isValid;
		}
	}
	public abstract class ValidationPattern : PropertiesBase, IPropertiesOwner {
		IValidationSettings validationSettings;
		public ValidationPattern(IPropertiesOwner owner, IValidationSettings validationSettings)
			: base(owner) {
			this.validationSettings = validationSettings;
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ValidationPatternErrorText"),
#endif
		Category("Appearance"), DefaultValue(StringResources.ASPxEdit_RequiredFieldDefaultErrorText),
		NotifyParentProperty(true), Localizable(true), AutoFormatDisable]
		public virtual string ErrorText {
			get { return GetStringProperty("ErrorText", ASPxEditorsLocalizer.GetString(ASPxEditorsStringId.RequiredFieldErrorText)); }
			set { SetStringProperty("ErrorText", ASPxEditorsLocalizer.GetString(ASPxEditorsStringId.RequiredFieldErrorText), value); }
		}
		public string GetErrorText() {
			if(ErrorText == "" && this.validationSettings != null && this.validationSettings.ErrorText != "")
				return this.validationSettings.ErrorText;
			else
				return ErrorText;
		}
		public override void Assign(PropertiesBase source) {
			base.Assign(source);
			ValidationPattern src = source as ValidationPattern;
			if(src != null) {
				ErrorText = src.ErrorText;
			}
		}
		public abstract bool EvaluateIsValid(object value);
		public abstract string GetClientInstanceCreationScript();
		protected internal abstract bool IsEmpty { get; }
		void IPropertiesOwner.Changed(PropertiesBase properties) {
			base.Changed();
		}
	}
	public class RequiredFieldValidationPattern : ValidationPattern, IPropertiesOwner {
		protected const string ClientInstanceCreationScript = "new ASPx.RequiredFieldValidationPattern({0})";
		public RequiredFieldValidationPattern(IPropertiesOwner owner, IValidationSettings validationSettings)
			: base(owner, validationSettings) {
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("RequiredFieldValidationPatternIsRequired"),
#endif
		Category("Behavior"), DefaultValue(false), NotifyParentProperty(true)]
		public bool IsRequired {
			get { return GetBoolProperty("IsRequired", false); }
			set {
				SetBoolProperty("IsRequired", false, value);
				Changed();
			}
		}
		public override void Assign(PropertiesBase source) {
			base.Assign(source);
			RequiredFieldValidationPattern src = source as RequiredFieldValidationPattern;
			if(src != null) {
				IsRequired = src.IsRequired;
			}
		}
		public override bool EvaluateIsValid(object value) {
			return !CommonUtils.IsNullValue(value) && (value.ToString().Trim() != "");
		}
		public override string GetClientInstanceCreationScript() {
			return String.Format(ClientInstanceCreationScript, HtmlConvertor.ToScript(GetErrorText()));
		}
		public override string ToString() {
			return IsRequired ? "*" : "";
		}
		protected internal override bool IsEmpty {
			get { return !IsRequired; }
		}
	}
	public class RegularExpressionValidationPattern : ValidationPattern {
		protected const string ClientInstanceCreationScript = "new ASPx.RegularExpressionValidationPattern({0}, {1})";
		public RegularExpressionValidationPattern(IPropertiesOwner owner, IValidationSettings validationSettings)
			: base(owner, validationSettings) {
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("RegularExpressionValidationPatternErrorText"),
#endif
		Category("Appearance"), DefaultValue(StringResources.ASPxEdit_RegExValidationDefaultErrorText),
		NotifyParentProperty(true), Localizable(true)]
		public override string ErrorText {
			get { return GetStringProperty("ErrorText", ASPxEditorsLocalizer.GetString(ASPxEditorsStringId.RegExValidationErrorText)); }
			set { SetStringProperty("ErrorText", ASPxEditorsLocalizer.GetString(ASPxEditorsStringId.RegExValidationErrorText), value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("RegularExpressionValidationPatternValidationExpression"),
#endif
		Category("Behavior"), DefaultValue(""), NotifyParentProperty(true), Localizable(false),
		Editor("DevExpress.Web.Design.Editors.RegularExpressionEditor, " + AssemblyInfo.SRAssemblyWebDesignFull, typeof(System.Drawing.Design.UITypeEditor))]
		public string ValidationExpression {
			get { return GetStringProperty("ValidationExpression", ""); }
			set {
				SetStringProperty("ValidationExpression", "", value);
				Changed();
			}
		}
		public override void Assign(PropertiesBase source) {
			base.Assign(source);
			RegularExpressionValidationPattern src = source as RegularExpressionValidationPattern;
			if(src != null) {
				ValidationExpression = src.ValidationExpression;
			}
		}
		public override bool EvaluateIsValid(object value) {
			string strValue = CommonUtils.ValueToString(value);
			if(String.IsNullOrEmpty(strValue) || strValue.Trim() == "")
				return true;
			try {
				Match match = Regex.Match(strValue, ValidationExpression);
				return match.Success && (match.Index == 0) && (match.Length == strValue.Length);
			} catch {
				return true;
			}
		}
		public override string GetClientInstanceCreationScript() {
			return String.Format(ClientInstanceCreationScript, HtmlConvertor.ToScript(GetErrorText()),
				HtmlConvertor.ToScript(ValidationExpression));
		}
		public override string ToString() {
			return ValidationExpression;
		}
		protected internal override bool IsEmpty {
			get { return ValidationExpression == ""; }
		}
	}
	internal struct ValidationResult {
		private bool isValid;
		private string errorText;
		public static readonly ValidationResult Success;
		static ValidationResult() {
			Success = new ValidationResult(true, "");
		}
		public ValidationResult(bool isValid, string errorText) {
			this.isValid = isValid;
			this.errorText = errorText;
		}
		public bool IsValid {
			get { return isValid; }
		}
		public string ErrorText {
			get { return errorText; }
		}
	}
	internal class ValidationPatterns : PropertiesBase, IPropertiesOwner {
		private Dictionary<Type, ValidationPattern> validationPatterns = null;
		public ValidationPatterns(IValidationSettings validationSettings)
			: base(validationSettings) {
			this.validationPatterns = new Dictionary<Type, ValidationPattern>();
			this.validationPatterns.Add(typeof(RequiredFieldValidationPattern),
				new RequiredFieldValidationPattern(this, validationSettings));
			this.validationPatterns.Add(typeof(RegularExpressionValidationPattern),
				new RegularExpressionValidationPattern(this, validationSettings));
		}
		public RequiredFieldValidationPattern RequiredField {
			get {
				return (RequiredFieldValidationPattern)this.validationPatterns[typeof(RequiredFieldValidationPattern)];
			}
		}
		public RegularExpressionValidationPattern RegularExpression {
			get {
				return (RegularExpressionValidationPattern)this.validationPatterns[typeof(RegularExpressionValidationPattern)];
			}
		}
		public override void Assign(PropertiesBase source) {
			base.Assign(source);
			ValidationPatterns src = source as ValidationPatterns;
			if(src != null) {
				foreach(KeyValuePair<Type, ValidationPattern> pair in this.validationPatterns)
					pair.Value.Assign(src.validationPatterns[pair.Key]);
			}
		}
		public string GetClientValidationPatternsArray() {
			if(IsEmpty)
				return "";
			StringBuilder sb = new StringBuilder("[ ");
			foreach(KeyValuePair<Type, ValidationPattern> pair in this.validationPatterns) {
				if(pair.Value.IsEmpty)
					continue;
				sb.Append(pair.Value.GetClientInstanceCreationScript() + ", ");
			}
			string tmp = sb.ToString();
			return tmp.ToString().Remove(tmp.Length - 2, 1) + "]";
		}
		public override string ToString() {
			return "";
		}
		internal ValidationResult Validate(object value) {
			ValidationPattern validator;
			foreach(KeyValuePair<Type, ValidationPattern> pair in this.validationPatterns) {
				validator = pair.Value;
				if(validator.IsEmpty)
					continue;
				if(!validator.EvaluateIsValid(value))
					return new ValidationResult(false, validator.ErrorText);
			}
			return ValidationResult.Success;
		}
		protected override IStateManager[] GetStateManagedObjects() {
			List<IStateManager> validators = new List<IStateManager>();
			foreach(KeyValuePair<Type, ValidationPattern> pair in this.validationPatterns)
				validators.Add(pair.Value);
			return ViewStateUtils.GetMergedStateManagedObjects(base.GetStateManagedObjects(), validators.ToArray());
		}
		protected internal bool IsEmpty {
			get {
				foreach(KeyValuePair<Type, ValidationPattern> pair in this.validationPatterns) {
					if(!pair.Value.IsEmpty)
						return false;
				}
				return true;
			}
		}
		void IPropertiesOwner.Changed(PropertiesBase properties) {
			base.Changed();
		}
	}
	public interface IValidationSettings : IPropertiesOwner {
		string ErrorText { get; }
	}
	public enum Display { Static, Dynamic, None }
	public abstract class ValidationSettings : PropertiesBase, IValidationSettings {
		#region Specific validation settings
		class StandAloneValidationSettings : ValidationSettings {
			public StandAloneValidationSettings(IPropertiesOwner owner)
				: base(owner) {
			}
			[DefaultValue(ErrorDisplayMode.ImageWithText)]
			public override ErrorDisplayMode ErrorDisplayMode {
				get { return (ErrorDisplayMode)GetEnumProperty("ErrorDisplayMode", ErrorDisplayMode.ImageWithText); }
				set {
					SetEnumProperty("ErrorDisplayMode", ErrorDisplayMode.ImageWithText, value);
					Changed();
				}
			}
		}
		class GridModeValidationSettings : ValidationSettings {
			private int editingRowVisibleIndex = -1;
			public GridModeValidationSettings(IPropertiesOwner owner)
				: base(owner) {
			}
			internal override int EditingRowVisibleIndex {
				get { return editingRowVisibleIndex; }
				set { editingRowVisibleIndex = value; }
			}
			[DefaultValue(ErrorDisplayMode.ImageWithTooltip)]
			public override ErrorDisplayMode ErrorDisplayMode {
				get { return (ErrorDisplayMode)GetEnumProperty("ErrorDisplayMode", ErrorDisplayMode.ImageWithTooltip); }
				set {
					SetEnumProperty("ErrorDisplayMode", ErrorDisplayMode.ImageWithTooltip, value);
					Changed();
				}
			}
			protected override string GetErrorTextContextKey(ASPxEdit owner) {
				string key = base.GetErrorTextContextKey(owner);
				if(EditingRowVisibleIndex >= 0)
					key += "_Row" + EditingRowVisibleIndex;
				return key;
			}
		}
		class BinaryImageStandAloneValidationSettings : BinaryImageValidationSettings {
			public BinaryImageStandAloneValidationSettings(IPropertiesOwner owner) : base(owner) { }
			[DefaultValue(ErrorDisplayMode.ImageWithText)]
			public override ErrorDisplayMode ErrorDisplayMode {
				get { return (ErrorDisplayMode) GetEnumProperty("ErrorDisplayMode", ErrorDisplayMode.ImageWithText); }
				set {
					SetEnumProperty("ErrorDisplayMode", ErrorDisplayMode.ImageWithText, value);
					Changed();
				}
			}
		}
		class BinaryImageGridModeValidationSettings : BinaryImageValidationSettings {
			private int editingRowVisibleIndex = -1;
			public BinaryImageGridModeValidationSettings(IPropertiesOwner owner)
				: base(owner) {
			}
			internal override int EditingRowVisibleIndex {
				get { return editingRowVisibleIndex; }
				set { editingRowVisibleIndex = value; }
			}
			[DefaultValue(ErrorDisplayMode.ImageWithTooltip)]
			public override ErrorDisplayMode ErrorDisplayMode {
				get { return (ErrorDisplayMode)GetEnumProperty("ErrorDisplayMode", ErrorDisplayMode.ImageWithTooltip); }
				set {
					SetEnumProperty("ErrorDisplayMode", ErrorDisplayMode.ImageWithTooltip, value);
					Changed();
				}
			}
			protected override string GetErrorTextContextKey(ASPxEdit owner) {
				string key = base.GetErrorTextContextKey(owner);
				if(EditingRowVisibleIndex >= 0)
					key += "_Row" + EditingRowVisibleIndex;
				return key;
			}
		}
		#endregion
		private ErrorFrameStyle errorFrameStyle = null;
		private ImageProperties errorImage = null;
		private ValidationPatterns validationPatterns = null;
		protected internal ValidationSettings(IPropertiesOwner owner)
			: base(owner) {
			this.errorFrameStyle = new ErrorFrameStyle();
			this.errorImage = new ImageProperties(this);
			this.validationPatterns = new ValidationPatterns(this);
		}
		internal virtual int EditingRowVisibleIndex {
			get { return -1; }
			set { }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ValidationSettingsCausesValidation"),
#endif
		Category("Behavior"), NotifyParentProperty(true), DefaultValue(false), AutoFormatDisable]
		public bool CausesValidation {
			get { return GetBoolProperty("CausesValidation", false); }
			set { SetBoolProperty("CausesValidation", false, value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ValidationSettingsEnableCustomValidation"),
#endif
		Category("Behavior"), NotifyParentProperty(true), DefaultValue(false), AutoFormatDisable]
		public bool EnableCustomValidation {
			get { return GetBoolProperty("EnableCustomValidation", false); }
			set {
				SetBoolProperty("EnableCustomValidation", false, value);
				Changed();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ValidationSettingsDisplay"),
#endif
		Category("Behavior"), NotifyParentProperty(true), DefaultValue(Display.Static), AutoFormatDisable]
		public Display Display {
			get {
				if(!ValidationSummaryCollection.Instance.EditorsAllowedToShowErrors(ValidationGroup))
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
	DevExpressWebLocalizedDescription("ValidationSettingsValidateOnLeave"),
#endif
		Category("Behavior"), NotifyParentProperty(true), DefaultValue(true), AutoFormatDisable]
		public bool ValidateOnLeave {
			get { return GetBoolProperty("ValidateOnLeave", true); }
			set { SetBoolProperty("ValidateOnLeave", true, value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ValidationSettingsErrorDisplayMode"),
#endif
		Category("Layout"), NotifyParentProperty(true), AutoFormatDisable]
		public abstract ErrorDisplayMode ErrorDisplayMode { get; set; }
		[
#if !SL
	DevExpressWebLocalizedDescription("ValidationSettingsErrorImage"),
#endif
		Category("Appearance"), NotifyParentProperty(true), AutoFormatEnable,
		PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public ImageProperties ErrorImage {
			get { return errorImage; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ValidationSettingsErrorText"),
#endif
		NotifyParentProperty(true), AutoFormatEnable,
		DefaultValue(StringResources.ASPxEdit_DefaultErrorText), Localizable(true)]
		public virtual string ErrorText {
			get {
				string errorTextFromContext = GetErrorTextFromContext();
				if(!string.IsNullOrEmpty(errorTextFromContext))
					return errorTextFromContext;
				return GetStringProperty("ErrorText", ASPxEditorsLocalizer.GetString(ASPxEditorsStringId.DefaultErrorText));
			}
			set {
				SetErrorTextInContext(value);
				SetStringProperty("ErrorText", ASPxEditorsLocalizer.GetString(ASPxEditorsStringId.DefaultErrorText), value);
				Changed();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ValidationSettingsErrorTextPosition"),
#endif
		Category("Layout"), AutoFormatEnable, NotifyParentProperty(true), DefaultValue(ErrorTextPosition.Right)]
		public ErrorTextPosition ErrorTextPosition {
			get { return (ErrorTextPosition)GetEnumProperty("ErrorTextPosition", ErrorTextPosition.Right); }
			set {
				SetEnumProperty("ErrorTextPosition", ErrorTextPosition.Right, value);
				Changed();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ValidationSettingsErrorFrameStyle"),
#endif
		Category("Appearance"), NotifyParentProperty(true), AutoFormatEnable,
		PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public ErrorFrameStyle ErrorFrameStyle {
			get { return errorFrameStyle; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ValidationSettingsRegularExpression"),
#endif
		Category("Behavior"), NotifyParentProperty(true),
		PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content), AutoFormatDisable]
		public RegularExpressionValidationPattern RegularExpression {
			get { return ValidationPatterns.RegularExpression; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ValidationSettingsRequiredField"),
#endif
		Category("Behavior"), NotifyParentProperty(true),
		PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content), AutoFormatDisable]
		public RequiredFieldValidationPattern RequiredField {
			get { return ValidationPatterns.RequiredField; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ValidationSettingsSetFocusOnError"),
#endif
		Category("Behavior"), DefaultValue(false), NotifyParentProperty(true), AutoFormatDisable]
		public bool SetFocusOnError {
			get { return GetBoolProperty("SetFocusOnError", false); }
			set { SetBoolProperty("SetFocusOnError", false, value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ValidationSettingsValidationGroup"),
#endif
		Category("Behavior"), NotifyParentProperty(true), DefaultValue(""), Localizable(false), AutoFormatDisable]
		public string ValidationGroup {
			get { return GetStringProperty("ValidationGroup", ""); }
			set { SetStringProperty("ValidationGroup", "", value); }
		}
		internal ValidationPatterns ValidationPatterns {
			get { return validationPatterns; }
		}
		internal bool IsTextMode {
			get { return ErrorDisplayMode == ErrorDisplayMode.Text || ErrorDisplayMode == ErrorDisplayMode.ImageWithText; }
		}
		internal bool IsImageMode {
			get { return ErrorDisplayMode == ErrorDisplayMode.ImageWithText || ErrorDisplayMode == ErrorDisplayMode.ImageWithTooltip; }
		}
		public override void Assign(PropertiesBase source) {
			base.Assign(source);
			ValidationSettings src = source as ValidationSettings;
			if(src != null) {
				CausesValidation = src.CausesValidation;
				Display = src.Display;
				EnableCustomValidation = src.EnableCustomValidation;
				ErrorDisplayMode = src.ErrorDisplayMode;
				ErrorImage.Assign(src.ErrorImage);
				ErrorText = src.ErrorText;
				ErrorTextPosition = src.ErrorTextPosition;
				ErrorFrameStyle.Assign(src.ErrorFrameStyle);
				ValidationPatterns.Assign(src.ValidationPatterns);
				SetFocusOnError = src.SetFocusOnError;
				ValidationGroup = src.ValidationGroup;
				ValidateOnLeave = src.ValidateOnLeave;
			}
		}
		public override string ToString() {
			return "";
		}
		public static ValidationSettings CreateValidationSettings(ASPxEdit edit) {
			if(edit is ASPxBinaryImage)
				return new BinaryImageGridModeValidationSettings(null);
			return new GridModeValidationSettings(null);
		}
		protected internal static ValidationSettings CreateValidationSettings(IPropertiesOwner owner) {
			var binaryImage = owner as ASPxBinaryImage;
			if(binaryImage != null)
				return CreateBinaryImageValdationSettings(binaryImage);
			var edit = owner as ASPxEdit;
			bool isStandAlone = edit != null && edit.InplaceMode == EditorInplaceMode.StandAlone;
			if(isStandAlone)
				return new StandAloneValidationSettings(owner);
			return new GridModeValidationSettings(owner);
		}
		protected internal static ValidationSettings CreateBinaryImageValdationSettings(IPropertiesOwner owner) {
			var binaryImage = owner as ASPxBinaryImage;
			if(binaryImage != null && binaryImage.InplaceMode == EditorInplaceMode.StandAlone)
				return new BinaryImageStandAloneValidationSettings(binaryImage);
			return new BinaryImageGridModeValidationSettings(binaryImage);
		}
		protected override IStateManager[] GetStateManagedObjects() {
			return ViewStateUtils.GetMergedStateManagedObjects(base.GetStateManagedObjects(),
				new IStateManager[] { ErrorImage, ErrorFrameStyle, ValidationPatterns });
		}
		void IPropertiesOwner.Changed(PropertiesBase properties) {
			base.Changed();
		}
		private string GetErrorTextFromContext() {
			ASPxEdit edit = Owner as ASPxEdit;
			if(edit == null)
				return null;
			else {
				string key = GetErrorTextContextKey(edit);
				return HttpUtils.GetContextValue(key, string.Empty);
			}
		}
		private void SetErrorTextInContext(string errorText) {
			ASPxEdit edit = Owner as ASPxEdit;
			if(edit != null) {
				string key = GetErrorTextContextKey(edit);
				HttpUtils.SetContextValue(key, errorText);
			}
		}
		protected virtual string GetErrorTextContextKey(ASPxEdit owner) {
			return owner.UniqueID + "_ErrorText";
		}
	}
	public abstract class BinaryImageValidationSettings : ValidationSettings {
		protected internal BinaryImageValidationSettings(IPropertiesOwner owner) : base(owner) { }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		 EditorBrowsable(EditorBrowsableState.Never)]
		public new RegularExpressionValidationPattern RegularExpression { get { return base.RegularExpression; } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		 EditorBrowsable(EditorBrowsableState.Never)]
		public new bool SetFocusOnError { get { return base.SetFocusOnError; } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		 EditorBrowsable(EditorBrowsableState.Never)]
		public new bool ValidateOnLeave { get { return base.SetFocusOnError; } }
	}
}
