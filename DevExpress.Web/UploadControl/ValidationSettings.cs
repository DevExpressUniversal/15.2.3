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
using System.ComponentModel.Design;
using System.Drawing.Design;
using System.Web.UI;
using DevExpress.Web.Internal;
using DevExpress.Web.Localization;
namespace DevExpress.Web {
	[TypeConverter(typeof(ExpandableObjectConverter))]
	public class UploadControlValidationSettings : PropertiesBase, IPropertiesOwner {
		private AppearanceStyleBase errorStyle = null;
		private ASPxUploadControl UploadControl {
			get { return Owner as ASPxUploadControl; }
		}
		public UploadControlValidationSettings()
			: base() {
		}
		public UploadControlValidationSettings(ASPxUploadControl uploadControl)
			: base(uploadControl) {
		}
		protected internal UploadControlValidationSettings(IPropertiesOwner owner)
			: base(owner) {
		}
		[
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		PersistenceMode(PersistenceMode.InnerProperty), NotifyParentProperty(true)]
		public AppearanceStyleBase ErrorStyle {
			get {
				if(this.errorStyle == null)
					this.errorStyle = new AppearanceStyleBase();
				return this.errorStyle;
			}
		}
		[Obsolete("This property is now obsolete. Use the AllowedFileExtensions property instead. Note, it accepts file extensions instead of content types."), Browsable(false),
		AutoFormatDisable, Localizable(false),
		NotifyParentProperty(true), TypeConverter(typeof(StringListConverter))]
		public string[] AllowedContentTypes {
			get { return (string[])GetObjectProperty("AllowedContentTypes", GetDefaultAllowedContentTypes()); }
			set {
				SetObjectProperty("AllowedContentTypes", GetDefaultAllowedContentTypes(), value);
				Changed();
			}
		}
		#pragma warning disable 618
		protected internal string[] AllowedContentTypesInternal {
			get { return AllowedContentTypes; }
			set { AllowedContentTypes = value; }
		}
		#pragma warning restore 618
		protected bool ShouldSerializeAllowedContentTypes() {
			return !CommonUtils.AreEqualsArrays(AllowedContentTypesInternal, GetDefaultAllowedContentTypes());
		}
		protected void ResetAllowedContentTypes() {
			AllowedContentTypesInternal = GetDefaultAllowedContentTypes();
		}
		[
		AutoFormatDisable, Localizable(false),
		NotifyParentProperty(true), TypeConverter(typeof(StringListConverter))]
		public virtual string[] AllowedFileExtensions {
			get { return (string[])GetObjectProperty("AllowedFileExtensions", GetDefaultAllowedFileExtensions()); }
			set {
				SetObjectProperty("AllowedFileExtensions", GetDefaultAllowedFileExtensions(), value);
				Changed();
			}
		}
		protected bool ShouldSerializeMultiSelectionErrorText() {
			return !string.Equals(MultiSelectionErrorText, GetDefaultMultiSelectionErrorText());
		}
		protected void ResetMultiSelectionErrorText() {
			MultiSelectionErrorText = GetDefaultMultiSelectionErrorText();
		}
		protected bool ShouldSerializeAllowedFileExtensions() {
			return !CommonUtils.AreEqualsArrays(AllowedFileExtensions, GetDefaultAllowedFileExtensions());
		}
		protected void ResetAllowedFileExtensions() {
			AllowedFileExtensions = GetDefaultAllowedFileExtensions();
		}
		[
		DefaultValue(StringResources.UploadControl_GeneralErrorText), AutoFormatDisable, Localizable(true),
		NotifyParentProperty(true)]
		public string GeneralErrorText {
			get { return GetStringProperty("GeneralErrorText", ASPxperienceLocalizer.GetString(ASPxperienceStringId.UploadControl_GeneralError)); }
			set { SetStringProperty("GeneralErrorText", ASPxperienceLocalizer.GetString(ASPxperienceStringId.UploadControl_GeneralError), value); }
		}
		[Obsolete("This property is now obsolete. Use the NotAllowedFileExtensionErrorText property instead."), Browsable(false),
		DefaultValue(StringResources.UploadControl_NotAllowedContentTypes), AutoFormatDisable, Localizable(true),
		NotifyParentProperty(true)]
		public string NotAllowedContentTypeErrorText {
			get { return GetStringProperty("NotAllowedContentTypeErrorText", ASPxperienceLocalizer.GetString(ASPxperienceStringId.UploadControl_NotAllowedContentTypes)); }
			set { SetStringProperty("NotAllowedContentTypeErrorText", ASPxperienceLocalizer.GetString(ASPxperienceStringId.UploadControl_NotAllowedContentTypes), value); }
		}
		#pragma warning disable 618
		protected internal string NotAllowedContentTypeErrorTextInternal {
			get { return NotAllowedContentTypeErrorText; }
			set { NotAllowedContentTypeErrorText = value; }
		}
		#pragma warning restore 618
		[
		DefaultValue(StringResources.UploadControl_NotAllowedFileExtension), AutoFormatDisable, Localizable(true),
		NotifyParentProperty(true)]
		public string NotAllowedFileExtensionErrorText {
			get { return GetStringProperty("NotAllowedFileExtensionErrorText", ASPxperienceLocalizer.GetString(ASPxperienceStringId.UploadControl_NotAllowedFileExtension)); }
			set { SetStringProperty("NotAllowedFileExtensionErrorText", ASPxperienceLocalizer.GetString(ASPxperienceStringId.UploadControl_NotAllowedFileExtension), value); }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), Obsolete("This property is now obsolete."),
		DefaultValue(StringResources.UploadControl_FileDoesNotExistErrorText), AutoFormatDisable, Localizable(true),
		NotifyParentProperty(true)]
		public string FileDoesNotExistErrorText {
			get { return GetStringProperty("FileDoesNotExistErrorText", ASPxperienceLocalizer.GetString(ASPxperienceStringId.UploadControl_FileDoesNotExistError)); }
			set { SetStringProperty("FileDoesNotExistErrorText", ASPxperienceLocalizer.GetString(ASPxperienceStringId.UploadControl_FileDoesNotExistError), value); }
		}
		[
		DefaultValue(0L), AutoFormatDisable, Localizable(false),
		NotifyParentProperty(true)]
		public virtual long MaxFileSize {
			get { return GetLongProperty("MaxFileSize", GetMaxFileSizeDefaultValue()); }
			set {
				CommonUtils.CheckNegativeValue(value, "MaxFileSize");
				SetLongProperty("MaxFileSize", GetMaxFileSizeDefaultValue(), value);
				Changed();
			}
		}
		[
		DefaultValue(StringResources.UploadControl_MaxSize), AutoFormatDisable, Localizable(true),
		NotifyParentProperty(true)]
		public string MaxFileSizeErrorText {
			get { return GetStringProperty("MaxFileSizeErrorText", ASPxperienceLocalizer.GetString(ASPxperienceStringId.UploadControl_MaxSize)); }
			set { SetStringProperty("MaxFileSizeErrorText", ASPxperienceLocalizer.GetString(ASPxperienceStringId.UploadControl_MaxSize), value); }
		}
		[
		DefaultValue(0), AutoFormatDisable, Localizable(false), NotifyParentProperty(true)]
		public int MaxFileCount {
			get { return GetIntProperty("MaxFileCount", 0); }
			set {
				CommonUtils.CheckNegativeValue(value, "MaxFileCount");
				SetIntProperty("MaxFileCount", 0, value);
				if(UploadControl != null)
					UploadControl.ValidateFileInputCount();
				Changed();
			}
		}
		[
		DefaultValue(StringResources.UploadControl_MaxFileCountErrorText), AutoFormatDisable, Localizable(true), NotifyParentProperty(true)]
		public string MaxFileCountErrorText {
			get { return GetStringProperty("MaxFileSizeErrorText", ASPxperienceLocalizer.GetString(ASPxperienceStringId.UploadControl_MaxFileCountError)); }
			set { SetStringProperty("MaxFileSizeErrorText", ASPxperienceLocalizer.GetString(ASPxperienceStringId.UploadControl_MaxFileCountError), value); }
		}
		[
		AutoFormatDisable, Localizable(true),
		NotifyParentProperty(true), Editor(typeof(MultilineStringEditor), typeof(UITypeEditor))]
		public string MultiSelectionErrorText
		{
			get { return GetStringProperty("MultiSelectionErrorText", GetDefaultMultiSelectionErrorText()); }
			set { SetStringProperty("MultiSelectionErrorText", GetDefaultMultiSelectionErrorText(), value); }
		}
		[
		DefaultValue(true), AutoFormatDisable, NotifyParentProperty(true)]
		public virtual bool ShowErrors {
			get { return GetBoolProperty("ShowErrors", true); }
			set { SetBoolProperty("ShowErrors", true, value); }
		}
		public override void Assign(PropertiesBase source) {
			BeginUpdate();
			try {
				base.Assign(source);
				if(source is UploadControlValidationSettings) {
					UploadControlValidationSettings src = source as UploadControlValidationSettings;
					AllowedContentTypesInternal = src.AllowedContentTypesInternal;
					AllowedFileExtensions = src.AllowedFileExtensions;
					NotAllowedContentTypeErrorTextInternal = src.NotAllowedContentTypeErrorTextInternal;
					NotAllowedFileExtensionErrorText = src.NotAllowedFileExtensionErrorText;
					ErrorStyle.Assign(src.ErrorStyle);
					MaxFileSize = src.MaxFileSize;
					MaxFileSizeErrorText = src.MaxFileSizeErrorText;
					MaxFileCount = src.MaxFileCount;
					MaxFileCountErrorText = src.MaxFileCountErrorText;
					MultiSelectionErrorText = src.MultiSelectionErrorText;
					GeneralErrorText = src.GeneralErrorText;
					ShowErrors = src.ShowErrors;
				}
			} finally {
				EndUpdate();
			}
		}
		public override string ToString() {
			return "";
		}
		protected virtual long GetMaxFileSizeDefaultValue() {
			return 0;
		}
		protected override IStateManager[] GetStateManagedObjects() {
			return ViewStateUtils.GetMergedStateManagedObjects(base.GetStateManagedObjects(),
				new IStateManager[] { ErrorStyle });
		}
		protected virtual string GetDefaultMultiSelectionErrorText() {
			return ASPxperienceLocalizer.GetString(ASPxperienceStringId.UploadControl_MultiSelection);
		}
		protected virtual string[] GetDefaultAllowedContentTypes() {
			return new string[0];
		}
		protected virtual string[] GetDefaultAllowedFileExtensions() {
			return new string[0];
		}
		void IPropertiesOwner.Changed(PropertiesBase properties) {
			Changed();
		}
	}
}
