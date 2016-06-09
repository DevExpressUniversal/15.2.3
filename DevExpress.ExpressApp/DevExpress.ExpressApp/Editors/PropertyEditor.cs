#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       eXpressApp Framework                                        }
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
using System.ComponentModel;
using DevExpress.Persistent.Base;
using DevExpress.ExpressApp.Utils;
using DevExpress.ExpressApp.Localization;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Model;
namespace DevExpress.ExpressApp.Editors {
	public interface IDependentPropertyEditor {
		IList<string> MasterProperties { get; }
		void Refresh();
	}
	public abstract class PropertyEditor : ViewItem, IAppearanceEnabled, IAppearanceVisibility, INotifyAppearanceVisibilityChanged, IDisposable {
		public const string TheDataTypeIsDefined = "The data type is defined";
		public const string MemberIsNotReadOnly = "MemberIsNotReadOnly";
		public const string IsNotInStruct = "IsNotInStruct";
		public const string PropertyEditorAllowEdit = "PropertyEditor.AllowEdit";
		public const string ModelAllowEdit = "Model.AllowEdit";
		private IModelMemberViewItem model;
		private bool immediatePostData;
		private string caption;
		private bool isPassword;
		private IMemberInfo memberInfo;
		private int maxLength;
		private string displayFormat;
		private string editMask;
		private EditMaskType editMaskType = EditMaskType.Default;
		private string errorMessage;
		private BoolList allowEditList;
		private bool valueWriting;
		protected bool inReadValue = false;
		protected string propertyName;
		private void allowEditList_ResultValueChanged(object sender, BoolValueChangedEventArgs e) {
			OnAllowEditChanged();
		}
		protected void UpdateEditorState() {
			UpdateMemberInfo();
			RefreshReadOnly();
			ReadValue();
		}
		protected virtual void UpdateFormatting() {
			if(CanFormatPropertyValue) {
				if(displayFormat == null) {
					displayFormat = PropertyEditorHelper.CalcDisplayFormat(model, memberInfo);
				}
				if(editMask == null) {
					editMask = PropertyEditorHelper.CalcEditMask(model, memberInfo);
				}
				if(editMaskType == EditMaskType.Default) {
					editMaskType = PropertyEditorHelper.CalcEditMaskType(model, memberInfo);
				}
			}
		}
		protected void UpdateMemberInfo() {
			if((ObjectType != null) && !String.IsNullOrEmpty(propertyName)) {
				memberInfo = ObjectTypeInfo.FindMember(propertyName);
			}
			else {
				memberInfo = null;
			}
		}
		protected internal override void UpdateErrorMessage(ErrorMessages errorMessages) {
			var message = errorMessages.GetRelatedMessage(PropertyName, CurrentObject);
			ErrorMessage = message != null ? message.Message : null;
			if(ErrorMessage != null) {
				ErrorIcon = message.Icon;
			}
			base.UpdateErrorMessage(errorMessages);
			OnErrorMessageChanged();
		}
		protected virtual void OnErrorMessageChanged() {
			if(ErrorMessageChanged != null) {
				ErrorMessageChanged(this, EventArgs.Empty);
			}
		}
		protected virtual void OnControlValueChanged() {
			if(ControlValueChanged != null) {
				ControlValueChanged(this, EventArgs.Empty);
			}
		}
		protected virtual void OnValueStoring(Object newValue) {
			Tracing.Tracer.LogText("'{0}' property editor: new value is '{1}'", propertyName, (isPassword ? "*******" : newValue));
			if(ValueStoring != null) {
				ValueStoring(this, new ValueStoringEventArgs(PropertyValue, newValue));
			}
		}
		protected virtual void OnValueStored() {
			if(ValueStored != null) {
				ValueStored(this, EventArgs.Empty);
			}
		}
		protected virtual void OnValueRead() {
			if(ValueRead != null) {
				ValueRead(this, EventArgs.Empty);
			}
		}
		protected virtual void OnAllowEditChanged() {
			if(AllowEditChanged != null) {
				AllowEditChanged(this, EventArgs.Empty);
			}
		}
		protected override void OnControlCreating() {
			base.OnControlCreating();
			if(string.IsNullOrEmpty(caption)) {
				caption = PropertyName;
			}
			if(ObjectType != null) {
				UpdateMemberInfo();
				maxLength = GetMaxLength();
			}
		}
		protected int GetMaxLength() {
			return GetMaxLengthCore(maxLength, memberInfo.ValueMaxLength);
		}
		protected virtual int GetMaxLengthCore(int initialMaxLength, int memberInfoValueMaxLength) {
			return PropertyEditorHelper.CalcMaxLengthCore(initialMaxLength, memberInfoValueMaxLength);
		}
		protected override void OnCurrentObjectChanged() {
			base.OnCurrentObjectChanged();
			UpdateEditorState();
			ErrorMessage = String.Empty;
		}
		protected abstract void ReadValueCore();
		protected abstract object GetControlValueCore();
		protected virtual void WriteValueCore() {
			PropertyValue = ControlValue;
		}
		protected virtual bool IsMemberSetterRequired() {
			return true;
		}
		protected virtual void RefreshReadOnly() {
			UpdateMemberInfo();
			AllowEdit.BeginUpdate();
			try {
				if(model != null) {
					AllowEdit.SetItemValue(ModelAllowEdit, model.AllowEdit);
				}
				AllowEdit.RemoveItem(MemberIsNotReadOnly);
				AllowEdit.RemoveItem(IsNotInStruct);
				bool isSetterRequired = IsMemberSetterRequired();
				if(memberInfo != null) {
					if(isSetterRequired) {
						AllowEdit.SetItemValue(MemberIsNotReadOnly, !memberInfo.IsReadOnly);
					}
					AllowEdit.SetItemValue(IsNotInStruct, !memberInfo.IsInStruct);
				}
			}
			finally {
				AllowEdit.EndUpdate();
			}
		}
		protected override void Dispose(bool disposing) {
			if(disposing) {
				memberInfo = null;
				ControlValueChanged = null;
				ValueStored = null;
				ValueStoring = null;
				AllowEditChanged = null;
			}
			base.Dispose(disposing);
		}
		protected virtual Boolean CanReadValue() {
			return (Control != null);
		}
		protected PropertyEditor(Type objectType, IModelMemberViewItem model)
			: base(objectType, model != null ? model.Id : "") {
			this.model = model;
			if(model != null) {
				propertyName = model.PropertyName;
				caption = model.Caption;
				maxLength = model.MaxLength;
				immediatePostData = model.ImmediatePostData;
				isPassword = model.IsPassword;
			}
			UpdateMemberInfo();
			UpdateFormatting();
			allowEditList = new BoolList(false, BoolListOperatorType.And);
			RefreshReadOnly();
			allowEditList.ResultValueChanged += new EventHandler<BoolValueChangedEventArgs>(allowEditList_ResultValueChanged);
		}
		public override void Refresh() {
			base.Refresh();
			ReadValue();
		}
		public void ReadValue() {
			if(!valueWriting && CanReadValue()) {
				inReadValue = true;
				try {
					ReadValueCore();
					OnValueRead();
				}
				finally {
					inReadValue = false;
				}
			}
		}
		public void WriteValue() {
			try {
				valueWriting = true;
				if(CurrentObject == null) {
					throw new ArgumentNullException("CurrentObject");
				}
				if(!AllowEdit) {
					throw new InvalidOperationException("ReadOnly");
				}
				Object currentValue = MemberInfo.GetValue(CurrentObject);
				Object newValue = GetControlValueCore();
				if((currentValue == null) || !currentValue.Equals(newValue)) {
					OnValueStoring(newValue);
					WriteValueCore();
					OnValueStored();
				}
			}
			catch(Exception e) {
				object val = GetControlValueCore();
				throw new InvalidOperationException(SystemExceptionLocalizer.GetExceptionMessage(ExceptionId.ValueCannotBeStoredIntoTheProperty,
					(val == null) ? "null" : val.ToString(), Caption), e);
			}
			finally {
				valueWriting = false;
			}
		}
		public Type GetUnderlyingType() {
			return PropertyEditorHelper.CalcUnderlyingType(MemberInfo);
		}
		public IModelMemberViewItem Model {
			get { return model; }
		}
		public BoolList AllowEdit {
			get { return allowEditList; }
		}
#if !SL
	[DevExpressExpressAppLocalizedDescription("PropertyEditorControlValue")]
#endif
		public virtual object ControlValue {
			get {
				if(Control == null) {
					throw new InvalidOperationException("You cannot access the 'ControlValue' property until control is created.");
				}
				return GetControlValueCore();
			}
		}
#if !SL
	[DevExpressExpressAppLocalizedDescription("PropertyEditorPropertyValue")]
#endif
		public object PropertyValue {
			get {
				if(CurrentObject != null) {
					return MemberInfo.GetValue(CurrentObject);
				}
				else {
					return null;
				}
			}
			set {
				if(!AllowEdit) {
					throw new InvalidOperationException(String.Format(SystemExceptionLocalizer.GetExceptionMessage(ExceptionId.UnableToSetReadOnlyProperty), PropertyName));
				}
				if(CurrentObject == null) {
					throw new InvalidOperationException(String.Format(SystemExceptionLocalizer.GetExceptionMessage(ExceptionId.CurrentObjectIsNotSet), PropertyName));
				}
				MemberInfo.SetValue(CurrentObject, value);
			}
		}
		public bool AllowNull {
			get {
				return !MemberInfo.LastMember.MemberTypeInfo.IsValueType ||
					MemberInfo.LastMember.MemberTypeInfo.IsNullable;
			}
		}
#if !SL
	[DevExpressExpressAppLocalizedDescription("PropertyEditorPropertyName")]
#endif
		public String PropertyName {
			get { return propertyName; }
			set {
				propertyName = value;
				UpdateEditorState();
				UpdateFormatting();
			}
		}
#if !SL
	[DevExpressExpressAppLocalizedDescription("PropertyEditorMemberInfo")]
#endif
		public IMemberInfo MemberInfo {
			get { return memberInfo; }
		}
#if !SL
	[DevExpressExpressAppLocalizedDescription("PropertyEditorCaption")]
#endif
		public override string Caption {
			get { return caption; }
			set {
				caption = value;
				if(model != null) {
					model.Caption = value;
				}
			}
		}
#if !SL
	[DevExpressExpressAppLocalizedDescription("PropertyEditorIsCaptionVisible")]
#endif
		public override bool IsCaptionVisible {
			get { return true; }
		}
#if !SL
	[DevExpressExpressAppLocalizedDescription("PropertyEditorMaxLength")]
#endif
		public int MaxLength {
			get { return maxLength; }
			set { maxLength = value; }
		}
#if !SL
	[DevExpressExpressAppLocalizedDescription("PropertyEditorEditMask")]
#endif
		public string EditMask {
			get { return editMask; }
			set { editMask = value; }
		}
#if !SL
	[DevExpressExpressAppLocalizedDescription("PropertyEditorEditMaskType")]
#endif
		public EditMaskType EditMaskType {
			get { return editMaskType; }
			set { editMaskType = value; }
		}
#if !SL
	[DevExpressExpressAppLocalizedDescription("PropertyEditorDisplayFormat")]
#endif
		public string DisplayFormat {
			get { return displayFormat; }
			set { displayFormat = value; }
		}
#if !SL
	[DevExpressExpressAppLocalizedDescription("PropertyEditorCanFormatPropertyValue")]
#endif
		public virtual bool CanFormatPropertyValue {
			get { return false; }
		}
#if !SL
	[DevExpressExpressAppLocalizedDescription("PropertyEditorIsPassword")]
#endif
		public bool IsPassword {
			get { return isPassword; }
			set { isPassword = value; }
		}
#if !SL
	[DevExpressExpressAppLocalizedDescription("PropertyEditorImmediatePostData")]
#endif
		public bool ImmediatePostData {
			get { return immediatePostData; }
			set { immediatePostData = value; }
		}
#if !SL
	[DevExpressExpressAppLocalizedDescription("PropertyEditorErrorMessage")]
#endif
		public virtual string ErrorMessage {
			get { return errorMessage; }
			set { errorMessage = value; }
		}
#if !SL
	[DevExpressExpressAppLocalizedDescription("PropertyEditorErrorIcon")]
#endif
		public virtual ImageInfo ErrorIcon {
			get;
			set;
		}
		#region IAppearanceEnabled Members
		bool IAppearanceEnabled.Enabled {
			get { return AllowEdit; }
			set { AllowEdit.SetItemValue("ByAppearance", value); }
		}
		void IAppearanceEnabled.ResetEnabled() {
			AllowEdit.RemoveItem("ByAppearance");
		}
		#endregion
		#region IAppearanceVisible Members
		private ViewItemVisibility visibility = ViewItemVisibility.Show;
		ViewItemVisibility IAppearanceVisibility.Visibility {
			get { return visibility; }
			set {
				if(visibility != value) {
					visibility = value;
					if(VisibilityChanged != null) {
						VisibilityChanged(this, EventArgs.Empty);
					}
				}
			}
		}
		void IAppearanceVisibility.ResetVisibility() {
			((IAppearanceVisibility)this).Visibility = ViewItemVisibility.Show;
		}
		public event EventHandler VisibilityChanged;
		#endregion
		public event EventHandler ControlValueChanged;
		public event EventHandler ValueRead;
		public event EventHandler<ValueStoringEventArgs> ValueStoring;
		public event EventHandler ValueStored;
		public event EventHandler AllowEditChanged;
		public event EventHandler ErrorMessageChanged;
	}
}
