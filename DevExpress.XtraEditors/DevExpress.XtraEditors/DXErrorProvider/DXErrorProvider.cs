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
using System.Text;
using System.Linq;
using System.ComponentModel;
using System.Windows.Forms;
using System.Security.Permissions;
using System.Drawing;
using System.ComponentModel.Design;
using DevExpress.XtraEditors;
using DevExpress.Utils;
using DevExpress.Data.Filtering;
using DevExpress.Data.Filtering.Helpers;
using DevExpress.Utils.Editors;
using System.Drawing.Design;
using System.Collections.Generic;
using DevExpress.XtraEditors.ToolboxIcons;
namespace DevExpress.XtraEditors.DXErrorProvider {
	[DXToolboxItem(DXToolboxItemKind.Free), ProvideProperty("Error", typeof(Control)), ProvideProperty("ErrorType", typeof(Control)), ProvideProperty("IconAlignment", typeof(Control)),
	 Description("Provides a mechanism for indicating to an end-user that an error is associated with an editor."),
	 Designer("DevExpress.Utils.Design.BaseComponentDesigner, " + AssemblyInfo.SRAssemblyDesign),
	 ToolboxTabName(AssemblyInfo.DXTabComponents),
	ToolboxBitmap(typeof(ToolboxIconsRootNS), "DXErrorProvider")
	]
	public class DXErrorProvider : Component, IExtenderProvider, ISupportInitialize {
		ContainerControl parentControl;
		object dataSource;
		string dataMember;
		BindingManagerBase bindingManager;
		bool isInitializing;
		Dictionary<Control, ErrorInfo> errorsInfo;
		[ThreadStatic]
		static Image informationIcon, warningIcon, errorIcon;
		static Image InformationIcon {
			get {
				if(informationIcon == null)
					informationIcon = DevExpress.Utils.ResourceImageHelper.CreateImageFromResources("DevExpress.XtraEditors.Images.Info.png", typeof(DXErrorProvider).Assembly);
				return informationIcon;
			}
		}
		static Image WarningIcon {
			get {
				if(warningIcon == null)
					warningIcon = DevExpress.Utils.ResourceImageHelper.CreateImageFromResources("DevExpress.XtraEditors.Images.Warning.png", typeof(DXErrorProvider).Assembly);
				return warningIcon;
			}
		}
		static Image ErrorIcon {
			get {
				if(errorIcon == null)
					errorIcon = DevExpress.Utils.ResourceImageHelper.CreateImageFromResources("DevExpress.XtraEditors.Images.Error.png", typeof(DXErrorProvider).Assembly);
				return errorIcon;
			}
		}
		public static event GetErrorIconEventHandler GetErrorIcon;
		public DXErrorProvider() {
			this.errorsInfo = new Dictionary<Control, ErrorInfo>();
			this.currentChanged = new EventHandler(BindingManager_CurrentChanged);
		}
		public DXErrorProvider(IContainer container)
			: this() {
			container.Add(this);
			this.bindingContextChanged = new EventHandler(ParentControl_BindingContextChanged);
		}
		public DXErrorProvider(ContainerControl parentControl)
			: this() {
			this.parentControl = parentControl;
			this.bindingContextChanged = new EventHandler(ParentControl_BindingContextChanged);
			parentControl.BindingContextChanged += bindingContextChanged;
		}
		[DefaultValue((string)null),
#if DXWhidbey
 AttributeProvider(typeof(IListSource))]
#else
		TypeConverter("System.Windows.Forms.Design.DataSourceConverter, System.Design")]
#endif
		public object DataSource {
			get { return dataSource; }
			set {
				if(((parentControl != null) && (value != null)) && (dataMember != null && dataMember != string.Empty)) {
					try {
						bindingManager = parentControl.BindingContext[value, dataMember];
					}
					catch(ArgumentException) {
						dataMember = "";
					}
				}
				UpdateBindingManager(value, DataMember, false);
			}
		}
		bool ShouldSerializeDataSource() { return (dataSource != null); }
		[DefaultValue((string)null), Editor(ControlConstants.DataMemberEditor, typeof(System.Drawing.Design.UITypeEditor))]
		public string DataMember {
			get { return dataMember; }
			set {
				if(value == null) {
					value = "";
				}
				UpdateBindingManager(DataSource, value, false);
			}
		}
		bool ShouldSerializeDataMember() {
			if(dataMember != null) return (dataMember.Length != 0);
			return false;
		}
		[DefaultValue((string)null)]
		public ContainerControl ContainerControl {
			[UIPermission(SecurityAction.LinkDemand, Window = UIPermissionWindow.AllWindows), UIPermission(SecurityAction.InheritanceDemand, Window = UIPermissionWindow.AllWindows)]
			get { return parentControl; }
			set {
				if(parentControl != value) {
					if(parentControl != null) {
						parentControl.BindingContextChanged -= bindingContextChanged;
					}
					parentControl = value;
					if(parentControl != null) {
						parentControl.BindingContextChanged += bindingContextChanged;
					}
					UpdateBindingManager(DataSource, DataMember, true);
				}
			}
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool HasErrors {
			get {
				foreach(ErrorInfo info in errorsInfo.Values) {
					if(!string.IsNullOrEmpty(info.ErrorText)) return true; 
				}
				return false;
			}
		}
		public bool HasErrorsOfType(ErrorType errorType) {
			foreach(ErrorInfo info in errorsInfo.Values) {
				if(!string.IsNullOrEmpty(info.ErrorText) && info.ErrorType == errorType) return true;
			}
			return false;
		}
		public IList<Control> GetControlsWithError() {
			List<Control> controls = new List<Control>();
			foreach(KeyValuePair<Control, ErrorInfo> pair in errorsInfo) {
				ErrorInfo info = pair.Value;
				if(!string.IsNullOrEmpty(info.ErrorText) && info.ErrorType != ErrorType.None)
					controls.Add(pair.Key);
			}
			return controls;
		}
		public IList<Control> GetControlsWithError(ErrorType errorType) {
			List<Control> controls = new List<Control>();
			foreach(KeyValuePair<Control, ErrorInfo> pair in errorsInfo) {
				ErrorInfo info = pair.Value;
				if(!string.IsNullOrEmpty(info.ErrorText) && info.ErrorType == errorType)
					controls.Add(pair.Key);
			}
			return controls;
		}
		public bool CanExtend(object extendee) {
			if(extendee is BaseEdit) return true;
			return false;
		}
		void ParentControl_BindingContextChanged(object sender, EventArgs e) {
			UpdateBindingManager(DataSource, DataMember, true);
		}
		public void UpdateBinding() {
			BindingManager_CurrentChanged(this.bindingManager, EventArgs.Empty);
		}
		void SubscribeBindingManagerEvents(BindingManagerBase listManager) {
			if(listManager != null) {
				listManager.CurrentChanged += currentChanged;
				listManager.BindingComplete += BindingManager_BindingComplete;
				CurrencyManager currencyManager = listManager as CurrencyManager;
				if(currencyManager != null) {
					currencyManager.ItemChanged += new ItemChangedEventHandler(BindingManager_ItemChanged);
					currencyManager.Bindings.CollectionChanged += new CollectionChangeEventHandler(BindingManager_BindingsChanged);
				}
			}
		}
		void UnsubscribeBindingManagerEvents(BindingManagerBase listManager) {
			if(listManager != null) {
				listManager.CurrentChanged -= currentChanged;
				listManager.BindingComplete -= BindingManager_BindingComplete;
				CurrencyManager currencyManager = listManager as CurrencyManager;
				if(currencyManager != null) {
					currencyManager.ItemChanged -= new ItemChangedEventHandler(BindingManager_ItemChanged);
					currencyManager.Bindings.CollectionChanged -= new CollectionChangeEventHandler(BindingManager_BindingsChanged);
				}
			}
		}
		bool isBindingManagerUpdated;
		bool updateBindingManagerOnInit;
		void UpdateBindingManager(object newDataSource, string newDataMember, bool force) {
			if(!isBindingManagerUpdated) {
				isBindingManagerUpdated = true;
				try {
					bool isNewDataSource = DataSource != newDataSource;
					bool isNewDataMember = DataMember != newDataMember;
					if((isNewDataSource || isNewDataMember) || force) {
						dataSource = newDataSource;
						dataMember = newDataMember;
						if(isInitializing) {
							updateBindingManagerOnInit = true;
						}
						else {
							UnsubscribeBindingManagerEvents(bindingManager);
							ClearErrors(true);
							if(((parentControl != null) && (dataSource != null)) && (parentControl.BindingContext != null)) {
								bindingManager = parentControl.BindingContext[dataSource, dataMember];
							}
							else {
								bindingManager = null;
							}
							SubscribeBindingManagerEvents(bindingManager);
							if(bindingManager != null) {
								UpdateBinding();
							}
						}
					}
				}
				finally {
					isBindingManagerUpdated = false;
				}
			}
		}
		void BindingManager_BindingComplete(object sender, BindingCompleteEventArgs e) {
			if(e.Binding != null && e.Binding.Control != null && e.Exception != null)
				SetError(e.Binding.Control, e.ErrorText == null ? string.Empty : e.ErrorText);
		}
		void BindingManager_ItemChanged(object sender, ItemChangedEventArgs e) {
			BindingsCollection bindings = bindingManager.Bindings;
			int count = bindings.Count;
			if((e.Index == -1) && (bindingManager.Count == 0)) {
				for(int i = 0; i < count; i++) {
					if(bindings[i].Control != null) SetError(bindings[i].Control, "", ErrorType.Default);
				}
			}
			else {
				BindingManager_CurrentChanged(sender, e);
			}
		}
		void BindingManager_BindingsChanged(object sender, CollectionChangeEventArgs e) {
			BindingManager_CurrentChanged(bindingManager, e);
		}
		void BindingManager_CurrentChanged(object sender, EventArgs e) {
			if(bindingManager == null || bindingManager.Count == 0) return;
			object obj = bindingManager.Current;
			if(IsDataObjectSupported(obj)) {
				BindingsCollection bindings = bindingManager.Bindings;
				int count = bindings.Count;
				Dictionary<Control, ErrorInfo> tempErrorsInfo = new Dictionary<Control, ErrorInfo>(count);
				for(int i = 0; i < count; i++) {
					if(bindings[i].Control == null) continue;
					Binding bindToObject = bindings[i];
					ErrorInfo error = GetErrorInfo(obj, bindToObject);
					string errorText = error.ErrorText;
					if(errorText == null) errorText = "";
					string text = "";
					ErrorType errorType = ErrorType.Default;
					if(tempErrorsInfo.ContainsKey(bindings[i].Control)) {
						ErrorInfo info = tempErrorsInfo[bindings[i].Control];
						errorType = info.ErrorType;
						text = info.ErrorText;
					}
					if(string.IsNullOrEmpty(text)) {
						text = errorText;
						errorType = error.ErrorType; 
					}
					else {
						text = text + "\r\n" + errorText;
						if(!string.IsNullOrEmpty(errorText))
							errorType = error.ErrorType;
					}
					error.ErrorType = errorType;
					error.ErrorText = text;
					tempErrorsInfo[bindings[i].Control] = error;
				}
				foreach(KeyValuePair<Control, ErrorInfo> entry in tempErrorsInfo) {
					SetError(entry.Key, entry.Value.ErrorText, entry.Value.ErrorType);
				}
			}
		}
		protected static void RaiseGetErrorIcon(GetErrorIconEventArgs e) {
			if(GetErrorIcon != null) GetErrorIcon(e);
		}
		public static Image GetErrorIconInternal(ErrorType type) {
			if(type == ErrorType.None) return null;
			GetErrorIconEventArgs args = new GetErrorIconEventArgs(type);
			RaiseGetErrorIcon(args);
			if(args.Icon != null) return args.Icon;
			switch(type) {
				case ErrorType.Warning:
					return WarningIcon;
				case ErrorType.Information:
					return InformationIcon;
				case ErrorType.Critical:
					return ErrorIcon;
			}
			return BaseEdit.DefaultErrorIcon;
		}
		protected virtual Image GetErrorIconCore(ErrorType type) {
			return GetErrorIconInternal(type);
		}
		public void RefreshControl(Control control) {
			Control_Validated(control, EventArgs.Empty);
		}
		public void RefreshControls() {
			foreach(Control c in errorsInfo.Keys.ToList()) {
				Control_Validated(c, EventArgs.Empty);
			}
		}
		void Control_Validated(object sender, EventArgs e) {
			BaseEdit control = sender as BaseEdit;
			if(!errorsInfo.ContainsKey(control) || CheckControlErrors(control)) return;
			ErrorInfo error = errorsInfo[control];
			SetError(control, error.ErrorText, error.ErrorType);
		}
		bool CheckControlErrors(Control control) {
			if(bindingManager == null || bindingManager.Count == 0) return false;
			BindingsCollection bindings = bindingManager.Bindings;
			ErrorType errorType = ErrorType.None;
			string errorText = null;
			for(int i = 0; i < bindings.Count; i++) {
				if(bindings[i].Control != control) continue;
				Binding bindToObject = bindings[i];
				ErrorInfo error = GetErrorInfo(bindingManager.Current, bindToObject);
				string text = (error.ErrorText == null) ? "" : error.ErrorText;
				if(errorText == null) errorText = text;
				else if(text != string.Empty) errorText += "\r\n" + text;
				errorType = error.ErrorType;
			}
			if(errorText != null) {
				SetErrorInfo(control, errorText, errorType);
				return true;
			}
			return false;
		}
		protected virtual ErrorInfo GetErrorInfo(object obj, Binding bindToObject) {
			string propertyName = bindToObject.BindingMemberInfo.BindingField;
			PropertyDescriptor fieldInfo = bindingManager.GetItemProperties().Find(propertyName, true);
			if(fieldInfo != null) propertyName = fieldInfo.Name;
			ErrorInfo info = new ErrorInfo();
			if(errorsInfo.ContainsKey(bindToObject.Control)) {
				info.ErrorType = errorsInfo[bindToObject.Control].ErrorType;
			}
			if(propertyName == null) return info;
			if(obj is IDataErrorInfo) {
				info.ErrorText = ((IDataErrorInfo)obj)[propertyName];
				info.ErrorType = ErrorType.Default;
			}
			if(obj is IDXDataErrorInfo) ((IDXDataErrorInfo)obj).GetPropertyError(propertyName, info);
			return info;
		}
		protected virtual bool IsDataObjectSupported(object obj) {
			return obj is IDataErrorInfo || obj is IDXDataErrorInfo;
		}
		bool IsBoundControl(Control control) {
			if(bindingManager == null) return false;
			for(int i = 0; i < bindingManager.Bindings.Count; i++) if(bindingManager.Bindings[i].Control == control) return true;
			return false;
		}
		bool CanRemoveErrorInfo(Control control) {
			if(clearErrors) return true;
			return !IsBoundControl(control);
		}
		public void SetError(Control control, string errorText) {
			SetError(control, errorText, ErrorType.Default);
		}
		public void SetError(Control control, string errorText, ErrorType errorType) {
			if(!CanSetError(control)) return;
			SetErrorCore(control, errorText, errorType);
		}
		public void SetIconAlignment(Control control, ErrorIconAlignment alignment) {
			BaseEdit edit = control as BaseEdit;
			if(edit != null)
				edit.ErrorIconAlignment = alignment;
		}
		[DefaultValue(ErrorIconAlignment.MiddleLeft)]
		public ErrorIconAlignment GetIconAlignment(Control control) {
			BaseEdit edit = control as BaseEdit;
			if(edit != null) return edit.ErrorIconAlignment;
			return BaseEdit.DefaultErrorIconAlignment;
		}
		protected bool CanSetError(Control control) {
			if(control == null) return false;
			return ((IExtenderProvider)this).CanExtend(control);
		}
		protected void SetErrorCore(Control control, string errorText, ErrorType errorType) {
			if(errorText == null) errorText = string.Empty;
			if(errorText == string.Empty && CanRemoveErrorInfo(control)) {
				if(errorsInfo.ContainsKey(control)) {
					RemoveErrorInfo(control);
					control.Validated -= new EventHandler(Control_Validated);
				}
			}
			else {
				if(!errorsInfo.ContainsKey(control)) control.Validated += new EventHandler(Control_Validated);
				SetErrorInfo(control, errorText, errorType);
			}
		}
		[DefaultValue(""), RefreshProperties(System.ComponentModel.RefreshProperties.All)]
		public string GetError(Control control) {
			if(control == null) return string.Empty;
			if(errorsInfo.ContainsKey(control)) return errorsInfo[control].ErrorText;
			if(control is BaseEdit) return (control as BaseEdit).ErrorText;
			return string.Empty;
		}
		protected virtual void RemoveErrorInfo(Control control) {
			errorsInfo.Remove(control);
			UpdateControlErrors(control, "", ErrorType.None);
		}
		protected virtual void SetErrorInfo(Control control, string errorText, ErrorType errorType) {
			if(errorsInfo.ContainsKey(control)) {
				ErrorInfo info = errorsInfo[control];
				info.ErrorText = errorText;
				info.ErrorType = errorType;
			}
			else {
				errorsInfo[control] = new ErrorInfo(errorText, errorType);
			}
			UpdateControlErrors(control, errorText, errorType);
		}
		void UpdateControlErrors(Control control, string errorText, ErrorType type) {
			BaseEdit edit = control as BaseEdit;
			if(type == ErrorType.None) errorText = "";
			edit.ErrorText = errorText;
			if(errorText != null && errorText.Length != 0) edit.ErrorIcon = GetErrorIconCore(type);
		}
		public void SetErrorType(Control control, ErrorType errorType) {
			if(!CanSetError(control)) return;
			string errorText = string.Empty;
			if(errorsInfo.ContainsKey(control)) {
				errorText = errorsInfo[control].ErrorText;
			}
			SetErrorCore(control, errorText, errorType);
		}
		[DefaultValue(ErrorType.None)]
		public ErrorType GetErrorType(Control control) {
			if(!errorsInfo.ContainsKey(control)) return ErrorType.None;
			return errorsInfo[control].ErrorType;
		}
		public void BindToDataAndErrors(object newDataSource, string newDataMember) {
			UpdateBindingManager(newDataSource, newDataMember, false);
		}
		void ISupportInitialize.BeginInit() {
			isInitializing = true;
		}
		void ISupportInitialize.EndInit() {
#if DXWhidbey
			ISupportInitializeNotification notification = DataSource as ISupportInitializeNotification;
			if((notification != null) && !notification.IsInitialized) {
				notification.Initialized += new EventHandler(new EventHandler(DataSource_Initialized));
			}
			else {
				EndInitCore();
			}
#else	
			EndInitCore();
#endif
		}
#if DXWhidbey
		private void DataSource_Initialized(object sender, EventArgs e) {
			ISupportInitializeNotification notification = this.DataSource as ISupportInitializeNotification;
			if(notification != null) {
				notification.Initialized -= new EventHandler(new EventHandler(DataSource_Initialized));
			}
			EndInitCore();
		}
#endif
		private void EndInitCore() {
			isInitializing = false;
			if(updateBindingManagerOnInit) {
				updateBindingManagerOnInit = false;
				UpdateBindingManager(DataSource, DataMember, true);
			}
		}
		bool clearErrors = false;
		protected virtual void ClearErrors(bool boundControlsOnly) {
			ArrayList controls = new ArrayList(errorsInfo.Keys);
			clearErrors = true;
			foreach(Control control in controls) {
				if(control == null || control.Disposing || control.IsDisposed) continue;
				if(!boundControlsOnly) {
					SetError(control, "");
					continue;
				}
				if(IsBoundControl(control)) SetError(control, "");
			}
			clearErrors = false;
			controls.Clear();
		}
		public void ClearErrors() { ClearErrors(false); }
		protected override void Dispose(bool disposing) {
			if(disposing) {
				UnsubscribeBindingManagerEvents(bindingManager);
				ClearErrors(false);
			}
			base.Dispose(disposing);
		}
		public override ISite Site {
			set {
				base.Site = value;
				if(value != null) {
					IDesignerHost host = value.GetService(typeof(IDesignerHost)) as IDesignerHost;
					if(host != null) {
						IComponent component = host.RootComponent;
						if(component is ContainerControl) {
							ContainerControl = (ContainerControl)component;
						}
					}
				}
			}
		}
		EventHandler currentChanged;
		EventHandler bindingContextChanged;
	}
	public delegate void GetErrorIconEventHandler(GetErrorIconEventArgs e);
	public class GetErrorIconEventArgs : EventArgs {
		ErrorType errorType;
		Image errorIcon;
		public GetErrorIconEventArgs(ErrorType errorType) {
			errorIcon = null;
			this.errorType = errorType;
		}
		public Image ErrorIcon { set { errorIcon = value; } }
		public ErrorType ErrorType { get { return errorType; } }
		protected internal Image Icon { get { return errorIcon; } }
	}
	#region DXValidationProvider
	public interface IValidatingControlCollection {
		ICollection Controls { get; set; }
	}
	public enum ValidationMode { Default, Auto, Manual }
	public enum CompareControlOperator {
		None,
		Equals,
		NotEquals,
		Less,
		Greater,
		GreaterOrEqual,
		LessOrEqual,
	}
	public enum ConditionOperator {
		None,
		Equals,
		NotEquals,
		Between,
		NotBetween,
		Less,
		Greater,
		GreaterOrEqual,
		LessOrEqual,
		BeginsWith,
		EndsWith,
		Contains,
		NotContains,
		Like,
		NotLike,
		IsBlank,
		IsNotBlank,
		AnyOf,
		NotAnyOf
	}
	public abstract class ValidationRuleBase : ICloneable {
		string name;
		string errorText;
		ErrorType errorType;
		public ValidationRuleBase() {
			this.errorText = this.name = string.Empty;
			this.errorType = ErrorType.Default;
		}
		public ValidationRuleBase(string name)
			: this() {
			this.name = name;
		}
		[Browsable(false)]
		public string Name { get { return this.name; } }
		[DefaultValue((string)null)]
		public string ErrorText { get { return this.errorText; } set { this.errorText = value; } }
		[DefaultValue(ErrorType.Default)]
		public ErrorType ErrorType { get { return this.errorType; } set { this.errorType = value; } }
		public virtual bool CanValidate(Control control) { return true; }
		public abstract bool Validate(Control control, object value);
		public abstract object Clone();
	}
	public class ValidationRule : ValidationRuleBase {
		bool caseSensitive;
		public ValidationRule()
			: base() {
			this.caseSensitive = false;
		}
		public ValidationRule(string name)
			: base(name) {
			this.caseSensitive = false;
		}
		[DefaultValue(false)]
		public bool CaseSensitive { get { return this.caseSensitive; } set { this.caseSensitive = value; } }
		public override bool Validate(Control control, object value) { return true; }
		public override object Clone() { return this.MemberwiseClone(); }
	}
	[Obsolete("Use the ConditionValidationRule class instead")]
	public class ConditionValidatonRule : ConditionValidationRule {
		public ConditionValidatonRule() : base() { }
		public ConditionValidatonRule(string name) : base(name) { }
		public ConditionValidatonRule(string name, ConditionOperator op, object value1, object value2) : base(name, op, value1, value2) { }
		public ConditionValidatonRule(string name, ConditionOperator op) : this(name, op, null, null) { }
		public ConditionValidatonRule(string name, ConditionOperator op, object value) : this(name, op, value, null) { }
		public ConditionValidatonRule(string name, ConditionOperator op, ICollection values) : this(name, op, null, null) { }
	}
	public class ConditionValidationRule : ValidationRule { 
		ConditionOperator condition;
		object value1;
		object value2;
		ArrayList values;
		public ConditionValidationRule()
			: base() {
			this.condition = ConditionOperator.None;
			this.value1 = null;
			this.value2 = null;
			this.values = new ArrayList();
		}
		public ConditionValidationRule(string name)
			: base(name) {
			this.condition = ConditionOperator.None;
			this.value1 = null;
			this.value2 = null;
			this.values = new ArrayList();
		}
		public ConditionValidationRule(string name, ConditionOperator op, object value1, object value2)
			: this(name) {
			this.condition = op;
			this.value1 = value1;
			this.value2 = value2;
			this.values = new ArrayList();
		}
		public ConditionValidationRule(string name, ConditionOperator op) : this(name, op, null, null) { }
		public ConditionValidationRule(string name, ConditionOperator op, object value) : this(name, op, value, null) { }
		public ConditionValidationRule(string name, ConditionOperator op, ICollection values)
			: this(name, op, null, null) {
			this.values = new ArrayList(values);
		}
		[DefaultValue(ConditionOperator.None)]
		public ConditionOperator ConditionOperator {
			get { return condition; }
			set { condition = value; }
		}
		[Editor("System.Windows.Forms.Design.StringCollectionEditor, System.Design", typeof(System.Drawing.Design.UITypeEditor)), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public ArrayList Values { get { return this.values; } }
		[Editor(typeof(UIObjectEditor), typeof(UITypeEditor)), TypeConverter(typeof(ObjectEditorTypeConverter)), DefaultValue((string)null)]
		public object Value1 {
			get { return value1; }
			set { value1 = value; }
		}
		[Editor(typeof(UIObjectEditor), typeof(UITypeEditor)), TypeConverter(typeof(ObjectEditorTypeConverter)), DefaultValue((string)null)]
		public object Value2 {
			get { return value2; }
			set { value2 = value; }
		}
		public override bool Validate(Control control, object value) {
			return ValidationHelper.Validate(value, ConditionOperator, Value1, Value2, Values, CaseSensitive);
		}
	}
	public class CompareAgainstControlValidationRule : ValidationRule, IValidatingControlCollection {
		Control controlCore;
		CompareControlOperator compareControlOperator;
		ICollection controls;
		public CompareAgainstControlValidationRule()
			: base() {
			this.controlCore = null;
			this.compareControlOperator = CompareControlOperator.None;
		}
		public CompareAgainstControlValidationRule(string name)
			: base(name) {
			this.controlCore = null;
			this.compareControlOperator = CompareControlOperator.None;
		}
		public CompareAgainstControlValidationRule(string name, Control control)
			: base(name) {
			this.controlCore = control;
			this.compareControlOperator = CompareControlOperator.None;
		}
		public CompareAgainstControlValidationRule(string name, Control control, CompareControlOperator compareOperator)
			: this(name, control) {
			this.compareControlOperator = compareOperator;
		}
		[Editor("DevExpress.XtraEditors.Design.ValidatingControlListUITypeEditor," + AssemblyInfo.SRAssemblyEditorsDesign, typeof(System.Drawing.Design.UITypeEditor)), DefaultValue((string)null)]
		public Control Control {
			get { return this.controlCore; }
			set { this.controlCore = value; }
		}
		[DefaultValue(CompareControlOperator.None)]
		public CompareControlOperator CompareControlOperator {
			get { return this.compareControlOperator; }
			set { this.compareControlOperator = value; }
		}
		public override bool CanValidate(Control control) {
			if(control != null && base.CanValidate(control)) {
				if(controlCore == null) return false;
				return control != controlCore;
			}
			return false;
		}
		public override bool Validate(Control control, object value) {
			return ValidationHelper.Validate(value, CompareControlOperator, GetControlValue(), null, null, CaseSensitive);
		}
		protected virtual object GetControlValue() {
			if(!(controlCore is BaseEdit)) return null;
			return (controlCore as BaseEdit).EditValue;
		}
		ICollection IValidatingControlCollection.Controls { get { return this.controls; } set { this.controls = value; } }
	}
	[DXToolboxItem(DXToolboxItemKind.Free), Designer("DevExpress.XtraEditors.Design.DXValidationProviderDesigner, " + AssemblyInfo.SRAssemblyEditorsDesignFull)]
	[ProvideProperty("ValidationRule", typeof(Control)), ProvideProperty("IconAlignment", typeof(Control)), ToolboxTabName(AssemblyInfo.DXTabComponents),
	ToolboxBitmap(typeof(ToolboxIconsRootNS), "DXValidationProvider")]
	public class DXValidationProvider : Component, IExtenderProvider, ISupportInitialize {
		static readonly object validationFailed = new object();
		static readonly object validationSucceeded = new object();
		ContainerControl parentControl;
		bool isInitializing;
		Hashtable validationRules;
		DXErrorProvider errorProvider;
		ValidationMode validationMode;
		List<Control> invalidControls = new List<Control>();
		bool validateHiddenControls;
		public DXValidationProvider() {
			this.validationRules = new Hashtable();
			this.errorProvider = new DXErrorProvider();
			this.validationMode = ValidationMode.Default;
			this.validateHiddenControls = true;
		}
		public DXValidationProvider(IContainer container)
			: this() {
			container.Add(this);
		}
		public DXValidationProvider(ContainerControl parentControl)
			: this() {
			this.parentControl = parentControl;
		}
		[DXCategory(CategoryName.Events)]
		public event ValidationFailedEventHandler ValidationFailed {
			add { Events.AddHandler(validationFailed, value); }
			remove { Events.RemoveHandler(validationFailed, value); }
		}
		[DXCategory(CategoryName.Events)]
		public event ValidationSucceededEventHandler ValidationSucceeded {
			add { Events.AddHandler(validationSucceeded, value); }
			remove { Events.RemoveHandler(validationSucceeded, value); }
		}
		public bool CanExtend(object extendee) {
			if(extendee is BaseEdit) return true;
			return false;
		}
		protected bool CanValidateControlCore(Control control) {
			if(control == null) return false;
			return ((IExtenderProvider)this).CanExtend(control);
		}
		protected virtual bool CanValidateControl(Control control) {
			bool result = CanValidateControlCore(control);
			if(result && !control.Visible)
				return ValidateHiddenControls;
			return result;
		}
		[DXCategory(CategoryName.Data), Editor("DevExpress.XtraEditors.Design.ValidationRulesUITypeEditor," + AssemblyInfo.SRAssemblyEditorsDesign, typeof(System.Drawing.Design.UITypeEditor)), DefaultValue((string)null)]
		public virtual ValidationRuleBase GetValidationRule(Control control) {
			if(control == null) return null;
			return (validationRules[control] as ValidationRuleBase);
		}
		[DXCategory(CategoryName.Data), Editor("DevExpress.XtraEditors.Design.ValidationRulesUITypeEditor," + AssemblyInfo.SRAssemblyEditorsDesign, typeof(System.Drawing.Design.UITypeEditor)), DefaultValue((string)null)]
		public virtual void SetValidationRule(Control control, ValidationRuleBase rule) {
			if(!CanValidateControlCore(control)) return;
			if(!isInitializing) {
				if((rule != null) && !base.DesignMode) {
					rule = rule.Clone() as ValidationRuleBase;
				}
			}
			if(rule != null) {
				validationRules[control] = rule;
				SubscribeValidatingEvent(control);
			}
			else {
				UnsubscribeValidatingEvent(control);
				validationRules.Remove(control);
				RemoveControlError(control); 
			}
		}
		[DefaultValue(ValidationMode.Default)]
		public ValidationMode ValidationMode {
			get { return this.validationMode; }
			set { this.validationMode = value; }
		}
		[DefaultValue(true)]
		public bool ValidateHiddenControls { get { return validateHiddenControls; } set { validateHiddenControls = value; } }
		protected virtual bool CanAutoValidate {
			get {
				if(ValidationMode == ValidationMode.Default || ValidationMode == ValidationMode.Manual) return false;
				return true;
			}
		}
		protected void SubscribeValidatingEvent(Control control) {
			control.Validating += new CancelEventHandler(Validating);
		}
		protected void UnsubscribeValidatingEvent(Control control) {
			control.Validating -= new CancelEventHandler(Validating);
		}
		[Obsolete("Use the GetInvalidControls method instead"), Browsable(false)]
		public virtual ICollection InvalidControls { get { return GetInvalidControls() as ICollection; } }
		public virtual IList<Control> GetInvalidControls() {
			return invalidControls.AsReadOnly();
		}
		protected virtual void Validating(object sender, CancelEventArgs e) {
			if(isInitializing || DesignMode || !CanAutoValidate) return;
			BaseEdit control = sender as BaseEdit;
			if(!CanValidateControl(control)) 
				return;
			ValidationRuleBase rule = validationRules[control] as ValidationRuleBase;
			if(rule == null || !rule.CanValidate(control)) return;
			e.Cancel = !rule.Validate(control, control.EditValue);
			if(e.Cancel)
				SetControlError(control, rule.ErrorText, rule.ErrorType);
			else
				RemoveControlError(control);
		}
		public bool Validate(Control control) {
			BaseEdit ctrl = control as BaseEdit;
			if(!CanValidateControl(ctrl)) {
				if(GetValidationRule(ctrl) != null)
					RemoveControlError(ctrl);
				return true;
			}
			ValidationRuleBase rule = GetValidationRule(ctrl);
			if(rule == null || !rule.CanValidate(ctrl)) return true;
			if(!rule.Validate(ctrl, ctrl.EditValue)) {
				SetControlError(ctrl, rule.ErrorText, rule.ErrorType);
				return false;
			}
			RemoveControlError(ctrl);
			return true;
		}
		public bool Validate() {
			bool isValid = true;
			foreach(Control control in validationRules.Keys) {
				if(!Validate(control)) isValid = false;
			}
			return isValid;
		}
		protected virtual void SetControlError(Control control, string errorText, ErrorType errorType) {
			errorProvider.SetError(control, errorText, errorType);
			if(!invalidControls.Contains(control)) invalidControls.Add(control);
			RaiseValidationFailed(new ValidationFailedEventArgs(control, errorText, errorType));
		}
		public virtual void RemoveControlError(Control control) { 
			errorProvider.SetError(control, null);
			if(invalidControls.Contains(control)) invalidControls.Remove(control);
			RaiseValidationSucceeded(new ValidationSucceededEventArgs(control));
		}
		void ISupportInitialize.BeginInit() {
			isInitializing = true;
		}
		void ISupportInitialize.EndInit() {
			isInitializing = false;
			EndInitCore();
		}
		void EndInitCore() {
			if(DesignMode) return;
			foreach(DictionaryEntry entry in new ArrayList(validationRules)) {
				ValidationRuleBase rule = entry.Value as ValidationRuleBase;
				if(rule != null) 
					validationRules[entry.Key] = rule.Clone();
			}
		}
		public void SetIconAlignment(Control control, ErrorIconAlignment alignment) {
			BaseEdit edit = control as BaseEdit;
			if(edit != null)
				edit.ErrorIconAlignment = alignment;
		}
		[DefaultValue(ErrorIconAlignment.MiddleLeft)]
		public ErrorIconAlignment GetIconAlignment(Control control) {
			BaseEdit edit = control as BaseEdit;
			if(edit != null) return edit.ErrorIconAlignment;
			return BaseEdit.DefaultErrorIconAlignment;
		}
		protected override void Dispose(bool disposing) {
			if(disposing) {
				ArrayList controls = new ArrayList(validationRules.Keys);
				foreach(Control control in controls) {
					if(control == null) continue;
					SetValidationRule(control, null);
					RemoveControlError(control);
				}
				invalidControls.Clear();
			}
			base.Dispose(disposing);
		}
		protected virtual void RaiseValidationFailed(ValidationFailedEventArgs e) {
			ValidationFailedEventHandler handler = (ValidationFailedEventHandler)Events[validationFailed];
			if(handler != null) handler(this, e);
		}
		protected virtual void RaiseValidationSucceeded(ValidationSucceededEventArgs e) {
			ValidationSucceededEventHandler handler = (ValidationSucceededEventHandler)Events[validationSucceeded];
			if(handler != null) handler(this, e);
		}
	}
	public class ValidationSucceededEventArgs : EventArgs {
		Control control;
		public ValidationSucceededEventArgs(Control control) {
			this.control = control;
		}
		public Control Control { get { return control; } }
	}
	public class ValidationFailedEventArgs : EventArgs {
		Control control;
		string errorText;
		ErrorType errorType;
		public ValidationFailedEventArgs(Control control, string errorText, ErrorType errorType) {
			this.control = control;
			this.errorText = errorText;
			this.errorType = errorType;
		}
		public Control InvalidControl { get { return control; } }
		public string ErrorText { get { return errorText; } }
		public ErrorType ErrorType { get { return errorType; } }
	}
	public delegate void ValidationFailedEventHandler(object sender, ValidationFailedEventArgs e);
	public delegate void ValidationSucceededEventHandler(object sender, ValidationSucceededEventArgs e);
	public static class ValidationHelper {
		public static ConditionOperator ParseCompareOperator(CompareControlOperator op) {
			switch(op) {
				case CompareControlOperator.Equals:
					return ConditionOperator.Equals;
				case CompareControlOperator.NotEquals:
					return ConditionOperator.NotEquals;
				case CompareControlOperator.Greater:
					return ConditionOperator.Greater;
				case CompareControlOperator.GreaterOrEqual:
					return ConditionOperator.GreaterOrEqual;
				case CompareControlOperator.Less:
					return ConditionOperator.Less;
				case CompareControlOperator.LessOrEqual:
					return ConditionOperator.LessOrEqual;
			}
			return ConditionOperator.None;
		}
		public static bool Validate(object validatingValue, CompareControlOperator condition, object value1, object value2, ICollection values) {
			return Validate(validatingValue, ParseCompareOperator(condition), value1, value2, values, false);
		}
		public static bool Validate(object validatingValue, CompareControlOperator condition, object value1, object value2, ICollection values, bool caseSensitive) {
			return Validate(validatingValue, ParseCompareOperator(condition), value1, value2, values, caseSensitive);
		}
		public static bool Validate(object validatingValue, ConditionOperator condition, object value1, object value2, ICollection values) {
			return Validate(validatingValue, condition, value1, value2, values, false);
		}
		public static bool Validate(object validatingValue, ConditionOperator condition, object value1, object value2, ICollection values, bool caseSensitive) {
			ComparerEvaluatorContextDescriptor contextDescriptor = new ComparerEvaluatorContextDescriptor(validatingValue);
			string s = value1 as string;
			if(s == null && validatingValue != null && value1 != null) s = value1.ToString();
			try {
				switch(condition) {
					case ConditionOperator.None:
						return true;
					case ConditionOperator.Equals:
						return EvaluateBinaryOperator(contextDescriptor, value1, BinaryOperatorType.Equal, caseSensitive);
					case ConditionOperator.NotEquals:
						return EvaluateBinaryOperator(contextDescriptor, value1, BinaryOperatorType.NotEqual, caseSensitive);
					case ConditionOperator.Between:
						return EvaluateBetweenOperator(contextDescriptor, value1, value2, caseSensitive);
					case ConditionOperator.NotBetween:
						return !EvaluateBetweenOperator(contextDescriptor, value1, value2, caseSensitive);
					case ConditionOperator.Less:
						return EvaluateBinaryOperator(contextDescriptor, value1, BinaryOperatorType.Less, caseSensitive);
					case ConditionOperator.Greater:
						return EvaluateBinaryOperator(contextDescriptor, value1, BinaryOperatorType.Greater, caseSensitive);
					case ConditionOperator.GreaterOrEqual:
						return EvaluateBinaryOperator(contextDescriptor, value1, BinaryOperatorType.GreaterOrEqual, caseSensitive);
					case ConditionOperator.LessOrEqual:
						return EvaluateBinaryOperator(contextDescriptor, value1, BinaryOperatorType.LessOrEqual, caseSensitive);
#pragma warning disable 618
					case ConditionOperator.BeginsWith:
						return EvaluateBinaryOperator(contextDescriptor, s + "%", BinaryOperatorType.Like, caseSensitive);
					case ConditionOperator.EndsWith:
						return EvaluateBinaryOperator(contextDescriptor, "%" + s, BinaryOperatorType.Like, caseSensitive);
					case ConditionOperator.Contains:
						return EvaluateBinaryOperator(contextDescriptor, "%" + s + "%", BinaryOperatorType.Like, caseSensitive);
					case ConditionOperator.NotContains:
						return !EvaluateBinaryOperator(contextDescriptor, "%" + s + "%", BinaryOperatorType.Like, caseSensitive);
					case ConditionOperator.Like:
						return EvaluateBinaryOperator(contextDescriptor, s, BinaryOperatorType.Like, caseSensitive);
					case ConditionOperator.NotLike:
						return !EvaluateBinaryOperator(contextDescriptor, s, BinaryOperatorType.Like, caseSensitive);
#pragma warning restore 618
					case ConditionOperator.IsBlank:
						return EvaluateIsBlankOperator(contextDescriptor, validatingValue as string, caseSensitive);
					case ConditionOperator.IsNotBlank:
						return !EvaluateIsBlankOperator(contextDescriptor, validatingValue as string, caseSensitive);
					case ConditionOperator.AnyOf:
						if(values == null || values.Count == 0) return true;
						return EvaluateInOperator(contextDescriptor, values, caseSensitive);
					case ConditionOperator.NotAnyOf:
						if(values == null || values.Count == 0) return true;
						return !EvaluateInOperator(contextDescriptor, values, caseSensitive);
					default:
						throw new NotImplementedException();
				}
			}
			catch {
				return false;
			}
		}
		static bool EvaluateIsBlankOperator(ComparerEvaluatorContextDescriptor contextDescriptor, string strValue, bool caseSensitive) {
			bool res = EvaluateUnaryOperator(contextDescriptor, UnaryOperatorType.IsNull, caseSensitive);
			if(!res && strValue != null) return strValue.Equals(string.Empty);
			return res;
		}
		static bool EvaluateBinaryOperator(ComparerEvaluatorContextDescriptor contextDescriptor, object value, BinaryOperatorType type, bool caseSensitive) {
			return new ExpressionEvaluator(contextDescriptor, new BinaryOperator("", value, type), caseSensitive).Fit(new object());
		}
		static bool EvaluateBetweenOperator(ComparerEvaluatorContextDescriptor contextDescriptor, object value1, object value2, bool caseSensitive) {
			return new ExpressionEvaluator(contextDescriptor, new BetweenOperator("", value1, value2), caseSensitive).Fit(new object());
		}
		static bool EvaluateUnaryOperator(ComparerEvaluatorContextDescriptor contextDescriptor, UnaryOperatorType type, bool caseSensitive) {
			return new ExpressionEvaluator(contextDescriptor, new UnaryOperator(type, ""), caseSensitive).Fit(new object());
		}
		static bool EvaluateInOperator(ComparerEvaluatorContextDescriptor contextDescriptor, ICollection values, bool caseSensitive) {
			return new ExpressionEvaluator(contextDescriptor, new InOperator("", values), caseSensitive).Fit(new object());
		}
		internal class ComparerEvaluatorContextDescriptor : EvaluatorContextDescriptor {
			object valueCore = null;
			public ComparerEvaluatorContextDescriptor() { }
			public ComparerEvaluatorContextDescriptor(object value) { this.valueCore = value; }
			public object Value { get { return valueCore; } set { valueCore = value; } }
			public override IEnumerable GetCollectionContexts(object source, string collectionName) { return null; }
			public override EvaluatorContext GetNestedContext(object source, string propertyPath) { return null; }
			public override IEnumerable GetQueryContexts(object source, string queryTypeName, CriteriaOperator condition, int top) { return null; }
			public override object GetPropertyValue(object source, EvaluatorProperty propertyPath) { return valueCore; }
		}
	}
	#endregion
}
