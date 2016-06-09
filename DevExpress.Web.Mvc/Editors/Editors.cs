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

using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Web.UI;
using DevExpress.Web.Internal;
namespace DevExpress.Web.Mvc {
	using DevExpress.Web.Internal.InternalCheckBox;
	using DevExpress.Web.Mvc.Internal;
	using DevExpress.Web.Mvc.UI;
	[ToolboxItem(false)]
	public class MVCxBinaryImage: ASPxBinaryImage {
		protected internal const string UploadControlPostfix = "_DXUploadEditor";
		public MVCxBinaryImage()
			: base() {
		}
		public new MVCxBinaryImageEditProperties Properties { get { return (MVCxBinaryImageEditProperties)base.Properties; } }
		public override bool IsLoading() { return false; }
		public override bool IsCallback { get { return MvcUtils.CallbackName == ClientID; } }
		protected internal new MVCxUploadControl UploadControl { get { return (MVCxUploadControl)base.UploadControl; } }
		protected internal new void EnsureChildControls() {
			base.EnsureChildControls();
		}
		protected internal override ASPxUploadControl CreateUploadControl() {
			return new MVCxBinaryImageUploadControl(this);
		}
		protected override EditPropertiesBase CreateProperties() {
			return new MVCxBinaryImageEditProperties(this);
		}
		protected override string GetClientObjectClassName() { return "MVCxClientBinaryImage"; }
		public override void RegisterEditorIncludeScripts() {
			base.RegisterEditorIncludeScripts();
			RegisterIncludeScript(typeof(MVCxBinaryImage), Utils.BinaryImageResourceName);
		}
		protected override void GetCreateClientObjectScript(StringBuilder stb, string localVarName, string clientName) {
			base.GetCreateClientObjectScript(stb, localVarName, clientName);
			if(Properties.CallbackRouteValues != null)
				stb.Append(localVarName + ".callbackUrl=\"" + Utils.GetUrl(Properties.CallbackRouteValues) + "\";\n");
		}
	}
	[ToolboxItem(false)]
	public class MVCxBinaryImageUploadControl : MVCxUploadControl {
		public MVCxBinaryImageUploadControl(MVCxBinaryImage owner)
			: base(owner) {
			if(owner != null)
				CallbackRouteValues = owner.Properties.CallbackRouteValues;
		}
	}
	[ToolboxItem(false)]
	public class MVCxButtonEdit : ASPxButtonEdit {
		ViewContext viewContext;
		ModelMetadata metadata;
		public MVCxButtonEdit()
			: this(null) {
		}
		protected internal MVCxButtonEdit(ViewContext viewContext)
			: this(viewContext, null) {
		}
		protected internal MVCxButtonEdit(ViewContext viewContext, ModelMetadata metadata)
			: base() {
			this.viewContext = viewContext ?? HtmlHelperExtension.ViewContext;
			this.metadata = metadata;
		}
		public new ButtonEditProperties Properties {
			get { return base.Properties; }
		}
		protected ViewContext ViewContext {
			get { return viewContext; }
		}
		protected internal ViewDataDictionary ViewData {
			get { return (ViewContext != null) ? ViewContext.ViewData : null; }
		}
		protected ModelMetadata Metadata {
			get { return metadata; }
		}
		public override bool IsLoading() {
			return false;
		}
		public override void RegisterEditorIncludeScripts() {
			base.RegisterEditorIncludeScripts();
			RegisterIncludeScript(typeof(MVCxButtonEdit), Utils.UtilsScriptResourceName);
		}
		public bool ShowModelErrors { get; set; }
		protected override bool IsCustomValidationEnabled {
			get { return ShowModelErrors || base.IsCustomValidationEnabled; }
		}
		protected override bool HasOnTextChanged() {
			return true;
		}
		protected override ButtonEditControl CreateButtonEditControl() {
			return new MVCxButtonEditControl(this, Metadata);
		}
		#region Control
		public class MVCxButtonEditControl : ButtonEditControl {
			public MVCxButtonEditControl(MVCxButtonEdit buttonEdit, ModelMetadata metadata)
				: base(buttonEdit) {
				Metadata = metadata;
			}
			protected ModelMetadata Metadata { get; set; }
			protected override void PrepareInputControl(InputControl input) {
				base.PrepareInputControl(input);
				ExtensionsHelper.SetUnobtrusiveValidationAttributes(input, Edit.ID, Metadata);
			}
		}
		#endregion
	}
	[ToolboxItem(false)]
	public class MVCxCalendar : ASPxCalendar {
		ViewContext viewContext;
		ModelMetadata metadata;
		public MVCxCalendar()
			: this(null) {
		}
		protected internal MVCxCalendar(ViewContext viewContext)
			: this(viewContext, null) {
		}
		protected internal MVCxCalendar(ViewContext viewContext, ModelMetadata metadata)
			: base() {
			this.viewContext = viewContext ?? HtmlHelperExtension.ViewContext;
			this.metadata = metadata;
		}
		public new CalendarProperties Properties {
			get { return base.Properties; }
		}
		protected ViewContext ViewContext {
			get { return viewContext; }
		}
		protected internal ViewDataDictionary ViewData {
			get { return (ViewContext != null) ? ViewContext.ViewData : null; }
		}
		protected ModelMetadata Metadata {
			get { return metadata; }
		}
		public override bool IsLoading() {
			return false;
		}
		public object CallbackRouteValues { get; set; }
		public override bool IsCallback {
			get { return MvcUtils.CallbackName == ID; }
		}
		protected internal override bool IsCallBacksEnabled() {
			return CallbackRouteValues != null;
		}
		protected internal new object GetCallbackResult() {
			return Enumerable.Repeat(Utils.CallbackHtmlContentPlaceholder, Rows * Columns).ToArray();
		}
		protected internal new object GetCallbackRenderResult() {
			return base.GetCallbackRenderResult();
		}
		protected internal new Control GetCallbackResultControl() {
			return base.GetCallbackResultControl();
		}
		protected internal new bool IsDateEditCalendar { get { return base.IsDateEditCalendar; } }
		public override void RegisterEditorIncludeScripts() {
			base.RegisterEditorIncludeScripts();
			RegisterIncludeScript(typeof(MVCxCalendar), Utils.UtilsScriptResourceName);
			RegisterIncludeScript(typeof(MVCxCalendar), Utils.CalendarScriptResourceName);
		}
		public bool ShowModelErrors { get; set; }
		protected override bool IsCustomValidationEnabled {
			get { return ShowModelErrors || base.IsCustomValidationEnabled; }
		}
		protected override void GetCreateClientObjectScript(StringBuilder stb, string localVarName, string clientName) {
			base.GetCreateClientObjectScript(stb, localVarName, clientName);
			if (CallbackRouteValues != null)
				stb.Append(localVarName + ".callbackUrl=\"" + Utils.GetUrl(CallbackRouteValues) + "\";\n");
		}
		protected override string GetClientObjectClassName() {
			return "MVCxClientCalendar";
		}
		protected override ASPxInternalWebControl CreateCalendarControl() {
			return new MVCxCalendarControl(this, Metadata);
		}
		#region Control
		public class MVCxCalendarControl : CalendarControl {
			public MVCxCalendarControl(MVCxCalendar calendar, ModelMetadata metadata)
				: base(calendar) {
					Metadata = metadata;
			}
			protected ModelMetadata Metadata { get; private set; }
			protected override void PrepareControlHierarchy() {
				base.PrepareControlHierarchy();
				if(!((MVCxCalendar)Calendar).IsDateEditCalendar) {
					KbInput.Input.Attributes.Add("name", Calendar.UniqueID);
					ExtensionsHelper.SetUnobtrusiveValidationAttributes(KbInput.Input, Calendar.UniqueID, Metadata);
				}
			}
		}
		#endregion
	}
	[ToolboxItem(false)]
	public class MVCxCheckBox : ASPxCheckBox {
		ViewContext viewContext;
		ModelMetadata metadata;
		public MVCxCheckBox()
			: this(null) {
		}
		protected internal MVCxCheckBox(ViewContext viewContext)
			: this(viewContext, null) {
		}
		public MVCxCheckBox(ViewContext viewContext, ModelMetadata metadata)
			: base() {
			this.viewContext = viewContext ?? HtmlHelperExtension.ViewContext;
			this.metadata = metadata;
		}
		public new CheckBoxProperties Properties {
			get { return base.Properties; }
		}
		protected ViewContext ViewContext {
			get { return viewContext; }
		}
		protected internal ViewDataDictionary ViewData {
			get { return (ViewContext != null) ? ViewContext.ViewData : null; }
		}
		protected ModelMetadata Metadata {
			get { return metadata; }
		}
		public override bool IsLoading() {
			return false;
		}
		public override void RegisterEditorIncludeScripts() {
			base.RegisterEditorIncludeScripts();
			RegisterIncludeScript(typeof(MVCxCheckBox), Utils.UtilsScriptResourceName);
		}
		public bool ShowModelErrors { get; set; }
		protected override bool IsCustomValidationEnabled {
			get { return ShowModelErrors || base.IsCustomValidationEnabled; }
		}
		protected override CheckBoxControl CreateCheckBoxControl() {
			return new MVCxCheckBoxControl(this, Metadata);
		}
		#region Control
		public class MVCxCheckBoxControl : CheckBoxControl {
			public MVCxCheckBoxControl(ASPxCheckBox checkBox, ModelMetadata metadata)
				: base(checkBox) {
				Metadata = metadata;
			}
			protected ModelMetadata Metadata { get; private set; }
			protected override void PrepareCheckableElement() {
				base.PrepareCheckableElement();
				ExtensionsHelper.SetUnobtrusiveValidationAttributes((CheckableElement as InternalCheckboxControl).Input, Edit.ID, Metadata);
			}
		}
		#endregion
	}
	[ToolboxItem(false)]
	public class MVCxCheckBoxList : ASPxCheckBoxList {
		ViewContext viewContext;
		ModelMetadata metadata;
		public MVCxCheckBoxList()
			: this(null) {
		}
		protected internal MVCxCheckBoxList(ViewContext viewContext)
			: this(viewContext, null) {
		}
		protected internal MVCxCheckBoxList(ViewContext viewContext, ModelMetadata metadata)
			: base() {
			this.viewContext = viewContext ?? HtmlHelperExtension.ViewContext;
			this.metadata = metadata;
		}
		public new CheckBoxListProperties Properties {
			get { return base.Properties; }
		}
		protected ViewContext ViewContext {
			get { return viewContext; }
		}
		protected internal ViewDataDictionary ViewData {
			get { return (ViewContext != null) ? ViewContext.ViewData : null; }
		}
		protected ModelMetadata Metadata {
			get { return metadata; }
		}
		public override bool IsLoading() {
			return false;
		}
		public override void RegisterEditorIncludeScripts() {
			base.RegisterEditorIncludeScripts();
			RegisterIncludeScript(typeof(MVCxCheckBoxList), Utils.UtilsScriptResourceName);
		}
		public bool ShowModelErrors { get; set; }
		protected override bool IsCustomValidationEnabled {
			get { return ShowModelErrors || base.IsCustomValidationEnabled; }
		}
		protected override ItemsControl<ListEditItem> GetCreateControl() {
			return new MVCxButtonListItemsControl(this, GetItemsList(), Metadata);
		}
		#region Control
		public class MVCxButtonListItemsControl : ButtonListItemsControlBase {
			public MVCxButtonListItemsControl(ASPxCheckListBase checkList, List<ListEditItem> items, ModelMetadata metadata)
				: base(checkList, items) {
				Metadata = metadata;
			}
			protected ModelMetadata Metadata { get; private set; }
			protected override void PrepareControlHierarchy() {
				base.PrepareControlHierarchy();
				ExtensionsHelper.SetUnobtrusiveValidationAttributes(VCHiddenField, CheckListBase.ID, Metadata);
			}
		}
		#endregion
	}
	[ToolboxItem(false)]
	public class MVCxColorEdit : ASPxColorEdit {
		ViewContext viewContext;
		ModelMetadata metadata;
		public MVCxColorEdit()
			: this(null) {
		}
		protected internal MVCxColorEdit(ViewContext viewContext)
			: this(viewContext, null) {
		}
		protected internal MVCxColorEdit(ViewContext viewContext, ModelMetadata metadata)
			: base() {
			this.viewContext = viewContext ?? HtmlHelperExtension.ViewContext;
			this.metadata = metadata;
		}
		public new ColorEditProperties Properties {
			get { return base.Properties; }
		}
		protected ViewContext ViewContext {
			get { return viewContext; }
		}
		protected internal ViewDataDictionary ViewData {
			get { return (ViewContext != null) ? ViewContext.ViewData : null; }
		}
		protected ModelMetadata Metadata {
			get { return metadata; }
		}
		public override bool IsLoading() {
			return false;
		}
		public override void RegisterEditorIncludeScripts() {
			base.RegisterEditorIncludeScripts();
			RegisterIncludeScript(typeof(MVCxColorEdit), Utils.UtilsScriptResourceName);
		}
		protected override bool HasOnTextChanged() {
			return true;
		}
		public bool ShowModelErrors { get; set; }
		protected override bool IsCustomValidationEnabled {
			get { return ShowModelErrors || base.IsCustomValidationEnabled; }
		}
		protected override DropDownControlBase CreateDropDownControl() {
			return new MVCxColorEditControl(this, Metadata);
		}
		#region Control
		public class MVCxColorEditControl : ColorEditControl {
			public MVCxColorEditControl(MVCxColorEdit colorEdit, ModelMetadata metadata)
				: base(colorEdit) {
				Metadata = metadata;
			}
			protected ModelMetadata Metadata { get; private set; }
			protected override void PrepareInputControl(InputControl input) {
				base.PrepareInputControl(input);
				ExtensionsHelper.SetUnobtrusiveValidationAttributes(input, Edit.ID, Metadata);
			}
		}
		#endregion
	}
	[ToolboxItem(false)]
	public class MVCxComboBox : ASPxComboBox {
		ViewContext viewContext;
		ModelMetadata metadata;
		protected internal MVCxComboBox()
			: this(null) {
		}
		public MVCxComboBox(ViewContext viewContext)
			: this(viewContext, null) {
		}
		protected internal MVCxComboBox(ViewContext viewContext, ModelMetadata metadata)
			: base() {
			this.viewContext = viewContext ?? HtmlHelperExtension.ViewContext;
			this.metadata = metadata;
		}
		public object CallbackRouteValues { get; set; }
		public new ComboBoxProperties Properties {
			get { return base.Properties; }
		}
		protected ViewContext ViewContext {
			get { return viewContext; }
		}
		protected internal ViewDataDictionary ViewData {
			get { return (ViewContext != null) ? ViewContext.ViewData : null; }
		}
		protected ModelMetadata Metadata {
			get { return metadata; }
		}
		public override bool IsCallback {
			get { return MvcUtils.CallbackName == ID; }
		}
		protected internal override bool IsCallBacksEnabled() {
			return CallbackRouteValues != null;
		}
		protected override string GetCallbackParamName() {
			return MvcUtils.CallbackArgumentParam;
		}
		protected override void GetCreateClientObjectScript(StringBuilder stb, string localVarName, string clientName) {
			base.GetCreateClientObjectScript(stb, localVarName, clientName);
			if(CallbackRouteValues != null)
				stb.Append(localVarName + ".callbackUrl=\"" + Utils.GetUrl(CallbackRouteValues) + "\";\n");
		}
		protected override string GetClientObjectClassName() {
			return IsNativeRender() ? "ASPxClientNativeComboBox" : "MVCxClientComboBox";
		}
		public override void RegisterEditorIncludeScripts() {
			base.RegisterEditorIncludeScripts();
			RegisterIncludeScript(typeof(MVCxComboBox), Utils.UtilsScriptResourceName);
			RegisterIncludeScript(typeof(MVCxComboBox), Utils.ComboBoxScriptResourceName);
		}
		protected override bool HasOnTextChanged() {
			return true;
		}
		public override bool IsLoading() {
			return false;
		}
		public bool ShowModelErrors { get; set; }
		protected override bool IsCustomValidationEnabled {
			get { return ShowModelErrors || base.IsCustomValidationEnabled; }
		}
		protected override DropDownControlBase CreateDropDownControl() {
			return new MVCxComboBoxControl(this, Metadata);
		}
		#region Control
		public class MVCxComboBoxControl : ComboBoxControl {
			public MVCxComboBoxControl(MVCxComboBox comboBox, ModelMetadata metadata)
				: base(comboBox) {
				Metadata = metadata;
			}
			protected ModelMetadata Metadata { get; private set; }
			protected override void PrepareInputControl(InputControl input) {
				base.PrepareInputControl(input);
				ExtensionsHelper.SetUnobtrusiveValidationAttributes(input, Edit.ID, Metadata);
			}
		}
		#endregion
	}
	[ToolboxItem(false)]
	public class MVCxDateEdit : ASPxDateEdit {
		ViewContext viewContext;
		ModelMetadata metadata;
		public MVCxDateEdit()
			: this(null) {
		}
		protected internal MVCxDateEdit(ViewContext viewContext)
			: this(viewContext, null) {
		}
		protected internal MVCxDateEdit(ViewContext viewContext, ModelMetadata metadata)
			: base() {
			this.viewContext = viewContext ?? HtmlHelperExtension.ViewContext;
			this.metadata = metadata;
		}
		public new DateEditProperties Properties {
			get { return base.Properties; }
		}
		protected ViewContext ViewContext {
			get { return viewContext; }
		}
		protected internal ViewDataDictionary ViewData {
			get { return (ViewContext != null) ? ViewContext.ViewData : null; }
		}
		protected ModelMetadata Metadata {
			get { return metadata; }
		}
		public override bool IsLoading() {
			return false;
		}
		public object CallbackRouteValues { get; set; }
		public override bool IsCallback {
			get { return !string.IsNullOrEmpty(MvcUtils.CallbackName) && MvcUtils.CallbackName == string.Format("{0}_DDD_C", ID); }
		}
		public override void RegisterEditorIncludeScripts() {
			base.RegisterEditorIncludeScripts();
			RegisterIncludeScript(typeof(MVCxDateEdit), Utils.UtilsScriptResourceName);
		}
		protected override bool HasOnTextChanged() {
			return true;
		}
		protected internal override void ValidateProperties() {
		}
		protected override string PopupCalendarOwnerClientID {
			get { return PopupCalendarOwnerID; }
		}
		protected override string StartDateEditClientID {
			get { return DateRangeSettings.StartDateEditID; }
		}
		protected internal override void ApplyDateRangeCalendarColumnCount(CalendarProperties targetProperties) {
		}
		public bool ShowModelErrors { get; set; }
		protected override bool IsCustomValidationEnabled {
			get { return ShowModelErrors || base.IsCustomValidationEnabled; }
		}
		protected override DropDownControlBase CreateDropDownControl() {
			return new MVCxDateEditControl(this, Metadata);
		}
		protected override object GetCallbackResult() {
			return Calendar.GetCallbackResult();
		}
		protected internal Control GetCallbackResultControl() {
			return Calendar.GetCallbackResultControl();
		}
		protected internal object GetCallbackRenderResult() {
			return Calendar.GetCallbackRenderResult();
		}
		protected internal MVCxCalendar Calendar {
			get { return ((MVCxDateEditControl)DropDownControl).Calendar; }
		}
		protected internal new void EnsureChildControls() {
			base.EnsureChildControls();
		}
		#region Control
		public class MVCxDateEditControl : DateEditControl {
			public MVCxDateEditControl()
				: base(null) {
			}
			public MVCxDateEditControl(MVCxDateEdit dateEdit, ModelMetadata metadata)
				: base(dateEdit) {
					Metadata = metadata;
			}
			protected internal new MVCxCalendar Calendar { get { return (MVCxCalendar)base.Calendar; } }
			protected new MVCxDateEdit DateEdit { get { return (MVCxDateEdit)base.DateEdit; } }
			protected ModelMetadata Metadata { get; private set; }
			protected override ASPxCalendar CreateCalendar() {
				return new MVCxCalendar() { 
					CallbackRouteValues = DateEdit.CallbackRouteValues 
				};
			}
			protected override void PrepareInputControl(InputControl input) {
				base.PrepareInputControl(input);
				ExtensionsHelper.SetUnobtrusiveValidationAttributes(input, Edit.ID, Metadata);
			}
		}
		#endregion
	}
	[ToolboxItem(false)]
	public class MVCxDropDownEdit : ASPxDropDownEdit {
		ViewContext viewContext;
		ModelMetadata metadata;
		public MVCxDropDownEdit()
			: this(null) {
		}
		protected internal MVCxDropDownEdit(ViewContext viewContext)
			: this(viewContext, null) {
		}
		protected internal MVCxDropDownEdit(ViewContext viewContext, ModelMetadata metadata)
			: base() {
			this.viewContext = viewContext ?? HtmlHelperExtension.ViewContext;
			this.metadata = metadata;
		}
		public new DropDownEditProperties Properties {
			get { return base.Properties; }
		}
		protected ViewContext ViewContext {
			get { return viewContext; }
		}
		protected internal ViewDataDictionary ViewData {
			get { return (ViewContext != null) ? ViewContext.ViewData : null; }
		}
		protected ModelMetadata Metadata {
			get { return metadata; }
		}
		public override bool IsLoading() {
			return false;
		}
		public override void RegisterEditorIncludeScripts() {
			base.RegisterEditorIncludeScripts();
			RegisterIncludeScript(typeof(MVCxDropDownEdit), Utils.UtilsScriptResourceName);
		}
		protected override bool HasOnTextChanged() {
			return true;
		}
		public bool ShowModelErrors { get; set; }
		protected override bool IsCustomValidationEnabled {
			get { return ShowModelErrors || base.IsCustomValidationEnabled; }
		}
		protected override DropDownControlBase CreateDropDownControl() {
			return new MVCxDropDownControl(this, Metadata);
		}
		#region Control
		public class MVCxDropDownControl : DropDownControl {
			public MVCxDropDownControl(MVCxDropDownEdit dropDown, ModelMetadata metadata)
				: base(dropDown) {
				Metadata = metadata;
			}
			protected ModelMetadata Metadata { get; private set; }
			protected override void PrepareInputControl(InputControl input) {
				base.PrepareInputControl(input);
				ExtensionsHelper.SetUnobtrusiveValidationAttributes(input, Edit.ID, Metadata);
			}
		}
		#endregion
	}
	[ToolboxItem(false)]
	public class MVCxHyperLink : ASPxHyperLink {
		public MVCxHyperLink()
			: base() {
		}
		public new HyperLinkProperties Properties {
			get { return base.Properties; }
		}
		public override bool IsLoading() {
			return false;
		}
	}
	[ToolboxItem(false)]
	public class MVCxImage : ASPxImage {
		public MVCxImage()
			: base() {
		}
		public new ImageEditProperties Properties {
			get { return base.Properties; }
		}
		public override bool IsLoading() {
			return false;
		}
	}
	[ToolboxItem(false)]
	public class MVCxLabel : ASPxLabel {
		public MVCxLabel()
			: base() {
		}
		public new LabelProperties Properties {
			get { return base.Properties; }
		}
		public override bool IsLoading() {
			return false;
		}
		public override string ClientID {
			get {
				return string.IsNullOrEmpty(ID) && !string.IsNullOrEmpty(AssociatedControlID)
					? string.Format("dxLabelFor{0}", AssociatedControlID)
					: base.ClientID;
			}
		}
		protected internal override string GetAssociatedControlClientID() {
			return AssociatedControlID;
		}
		protected override bool HasFunctionalityScripts() {
			return !string.IsNullOrEmpty(AssociatedControlID) || base.HasFunctionalityScripts();
		}
		protected override void GetCreateClientObjectScript(System.Text.StringBuilder stb, string localVarName, string clientName) {
			base.GetCreateClientObjectScript(stb, localVarName, clientName);
			if(!string.IsNullOrEmpty(AssociatedControlID))
				stb.Append(localVarName + ".associatedControlName=\"" + AssociatedControlID + "\";\n");
		}
		protected override string GetClientObjectClassName() {
			return "MVCxClientLabel";
		}
		public override void RegisterEditorIncludeScripts() {
			base.RegisterEditorIncludeScripts();
			RegisterIncludeScript(typeof(MVCxLabel), Utils.UtilsScriptResourceName);
			RegisterIncludeScript(typeof(MVCxLabel), Utils.LabelScriptResourceName);
		}
	}
	[ToolboxItem(false)]
	public class MVCxListBox : ASPxListBox {
		ViewContext viewContext;
		ModelMetadata metadata;
		public MVCxListBox()
			: this(null) {
		}
		protected internal MVCxListBox(ViewContext viewContext)
			: this(viewContext, null) {
		}
		protected internal MVCxListBox(ViewContext viewContext, ModelMetadata metadata)
			: base() {
			this.viewContext = viewContext ?? HtmlHelperExtension.ViewContext;
			this.metadata = metadata;
		}
		public object CallbackRouteValues { get; set; }
		public new ListBoxProperties Properties {
			get { return base.Properties; }
		}
		protected ViewContext ViewContext {
			get { return viewContext; }
		}
		protected internal ViewDataDictionary ViewData {
			get { return (ViewContext != null) ? ViewContext.ViewData : null; }
		}
		protected ModelMetadata Metadata {
			get { return metadata; }
		}
		public override bool IsCallback {
			get { return MvcUtils.CallbackName == ID; }
		}
		protected internal override bool IsCallBacksEnabled() {
			return CallbackRouteValues != null;
		}
		protected override void GetCreateClientObjectScript(StringBuilder stb, string localVarName, string clientName) {
			base.GetCreateClientObjectScript(stb, localVarName, clientName);
			if(CallbackRouteValues != null)
				stb.Append(localVarName + ".callbackUrl=\"" + Utils.GetUrl(CallbackRouteValues) + "\";\n");
		}
		protected override string GetClientObjectClassName() {
			return IsNativeRender() ? "ASPxClientNativeListBox" : "MVCxClientListBox";
		}
		public override void RegisterEditorIncludeScripts() {
			base.RegisterEditorIncludeScripts();
			RegisterIncludeScript(typeof(MVCxListBox), Utils.UtilsScriptResourceName);
			RegisterIncludeScript(typeof(MVCxListBox), Utils.ListBoxScriptResourceName);
		}
		public override bool IsLoading() {
			return false;
		}
		public bool ShowModelErrors { get; set; }
		protected override bool IsCustomValidationEnabled {
			get { return ShowModelErrors || base.IsCustomValidationEnabled; }
		}
		protected override ListBoxControl CreateListBoxControl() {
			return new MVCxListBoxControl(this, Metadata);
		}
		#region Control
		public class MVCxListBoxControl : ListBoxControl {
			public MVCxListBoxControl(MVCxListBox listbox, ModelMetadata metadata)
				: base(listbox) {
				Metadata = metadata;
			}
			protected ModelMetadata Metadata { get; private set; }
			protected override void PrepareMainCellContent() {
				base.PrepareMainCellContent();
				ExtensionsHelper.SetUnobtrusiveValidationAttributes(VCHiddenField, ListBox.ID, Metadata);
			}
		}
		#endregion
	}
	[ToolboxItem(false)]
	public class MVCxMemo : ASPxMemo {
		ViewContext viewContext;
		ModelMetadata metadata;
		public MVCxMemo()
			: this(null) {
		}
		protected internal MVCxMemo(ViewContext viewContext)
			: this(viewContext, null) {
		}
		protected internal MVCxMemo(ViewContext viewContext, ModelMetadata metadata)
			: base() {
			this.viewContext = viewContext ?? HtmlHelperExtension.ViewContext;
			this.metadata = metadata;
		}
		public new MemoProperties Properties {
			get { return base.Properties; }
		}
		protected ViewContext ViewContext {
			get { return viewContext; }
		}
		protected internal ViewDataDictionary ViewData {
			get { return (ViewContext != null) ? ViewContext.ViewData : null; }
		}
		protected ModelMetadata Metadata {
			get { return metadata; }
		}
		public override bool IsLoading() {
			return false;
		}
		public override void RegisterEditorIncludeScripts() {
			base.RegisterEditorIncludeScripts();
			RegisterIncludeScript(typeof(MVCxMemo), Utils.UtilsScriptResourceName);
		}
		protected override bool HasOnTextChanged() {
			return true;
		}
		public bool ShowModelErrors { get; set; }
		protected override bool IsCustomValidationEnabled {
			get { return ShowModelErrors || base.IsCustomValidationEnabled; }
		}
		protected override MemoControl CreateMemoControl() {
			return new MVCxMemoControl(this, Metadata);
		}
		#region Control
		public class MVCxMemoControl : MemoControl {
			public MVCxMemoControl(MVCxMemo memo, ModelMetadata metadata)
				: base(memo) {
				Metadata = metadata;
			}
			protected ModelMetadata Metadata { get; private set; }
			protected override void PrepareMainCellContent() {
				base.PrepareMainCellContent();
				ExtensionsHelper.SetUnobtrusiveValidationAttributes(TextArea, Edit.ID, Metadata);
			}
		}
		#endregion
	}
	[ToolboxItem(false)]
	public class MVCxProgressBar : ASPxProgressBar {
		public MVCxProgressBar()
			: base() {
		}
		public new ProgressBarProperties Properties {
			get { return base.Properties; }
		}
		public override bool IsLoading() {
			return false;
		}
	}
	[ToolboxItem(false)]
	public class MVCxRadioButton : ASPxRadioButton {
		ViewContext viewContext;
		ModelMetadata metadata;
		public MVCxRadioButton()
			: this(null) {
		}
		protected internal MVCxRadioButton(ViewContext viewContext)
			: this(viewContext, null) {
		}
		protected internal MVCxRadioButton(ViewContext viewContext, ModelMetadata metadata)
			: base() {
			this.viewContext = viewContext ?? HtmlHelperExtension.ViewContext;
			this.metadata = metadata;
		}
		public new RadioButtonProperties Properties {
			get { return base.Properties; }
		}
		protected ViewContext ViewContext {
			get { return viewContext; }
		}
		protected internal ViewDataDictionary ViewData {
			get { return (ViewContext != null) ? ViewContext.ViewData : null; }
		}
		protected ModelMetadata Metadata {
			get { return metadata; }
		}
		public override bool IsLoading() {
			return false;
		}
		public override void RegisterEditorIncludeScripts() {
			base.RegisterEditorIncludeScripts();
			RegisterIncludeScript(typeof(MVCxRadioButton), Utils.UtilsScriptResourceName);
		}
		public bool ShowModelErrors { get; set; }
		protected override bool IsCustomValidationEnabled {
			get { return ShowModelErrors || base.IsCustomValidationEnabled; }
		}
		protected override RadioButtonControl CreateRadioButtonControl() {
			return new MVCxRadioButtonControl(this, Metadata);
		}
		#region Control
		public class MVCxRadioButtonControl : RadioButtonControl {
			public MVCxRadioButtonControl(ASPxRadioButton radioButton, ModelMetadata metadata)
				: base(radioButton) {
				Metadata = metadata;
			}
			protected ModelMetadata Metadata { get; private set; }
			protected override void PrepareCheckableElement() {
				base.PrepareCheckableElement();
				ExtensionsHelper.SetUnobtrusiveValidationAttributes((CheckableElement as InternalCheckboxControl).Input, Edit.ID, Metadata);
			}
		}
		#endregion
	}
	[ToolboxItem(false)]
	public class MVCxRadioButtonList : ASPxRadioButtonList {
		ViewContext viewContext;
		ModelMetadata metadata;
		public MVCxRadioButtonList()
			: this(null) {
		}
		protected internal MVCxRadioButtonList(ViewContext viewContext)
			: this(viewContext, null) {
		}
		protected internal MVCxRadioButtonList(ViewContext viewContext, ModelMetadata metadata)
			: base() {
			this.viewContext = viewContext ?? HtmlHelperExtension.ViewContext;
			this.metadata = metadata;
		}
		public new RadioButtonListProperties Properties {
			get { return base.Properties; }
		}
		protected ViewContext ViewContext {
			get { return viewContext; }
		}
		protected internal ViewDataDictionary ViewData {
			get { return (ViewContext != null) ? ViewContext.ViewData : null; }
		}
		protected ModelMetadata Metadata {
			get { return metadata; }
		}
		public override bool IsLoading() {
			return false;
		}
		public override void RegisterEditorIncludeScripts() {
			base.RegisterEditorIncludeScripts();
			RegisterIncludeScript(typeof(MVCxRadioButtonList), Utils.UtilsScriptResourceName);
		}
		public bool ShowModelErrors { get; set; }
		protected override bool IsCustomValidationEnabled {
			get { return ShowModelErrors || base.IsCustomValidationEnabled; }
		}
		protected override ItemsControl<ListEditItem> GetCreateControl() {
			return new MVCxCheckBoxList.MVCxButtonListItemsControl(this, GetItemsList(), Metadata);
		}
	}
	[ToolboxItem(false)]
	public class MVCxSpinEdit : ASPxSpinEdit {
		ViewContext viewContext;
		ModelMetadata metadata;
		public MVCxSpinEdit()
			: this(null) {
		}
		protected internal MVCxSpinEdit(ViewContext viewContext)
			: this(viewContext, null) {
		}
		protected internal MVCxSpinEdit(ViewContext viewContext, ModelMetadata metadata)
			: base() {
			this.viewContext = viewContext ?? HtmlHelperExtension.ViewContext;
			this.metadata = metadata;
		}
		public new SpinEditProperties Properties {
			get { return base.Properties; }
		}
		protected ViewContext ViewContext {
			get { return viewContext; }
		}
		protected internal ViewDataDictionary ViewData {
			get { return (ViewContext != null) ? ViewContext.ViewData : null; }
		}
		protected ModelMetadata Metadata {
			get { return metadata; }
		}
		public override bool IsLoading() {
			return false;
		}
		public override void RegisterEditorIncludeScripts() {
			base.RegisterEditorIncludeScripts();
			RegisterIncludeScript(typeof(MVCxSpinEdit), Utils.UtilsScriptResourceName);
		}
		protected override bool HasOnTextChanged() {
			return true;
		}
		public bool ShowModelErrors { get; set; }
		protected override bool IsCustomValidationEnabled {
			get { return ShowModelErrors || base.IsCustomValidationEnabled; }
		}
		protected override string GetInvalidValueRangeWarningMessage() {
			string message = AttributeMapper.GetRangeAttributeErrorMessage(Metadata);
			return !string.IsNullOrEmpty(message) ? message : base.GetInvalidValueRangeWarningMessage();
		}
		public override bool IsClientSideAPIEnabled() {
			return !string.IsNullOrEmpty(this.ID) || base.IsClientSideAPIEnabled();
		}
		protected override SpinEditControl CreateSpinEditControl() {
			return new MVCxSpinEditControl(this, Metadata);
		}
		#region Control
			public class MVCxSpinEditControl : SpinEditControl {
			public MVCxSpinEditControl(ASPxSpinEditBase spinEdit, ModelMetadata metadata)
				: base(spinEdit) {
					Metadata = metadata;
			}
			protected ModelMetadata Metadata { get; private set; }
			protected override void PrepareInputControl(InputControl input) {
				base.PrepareInputControl(input);
				ExtensionsHelper.SetUnobtrusiveValidationAttributes(input, Edit.ID, Metadata);
			}
		}
		#endregion
	}
	[ToolboxItem(false)]
	public class MVCxTextBox : ASPxTextBox {
		ViewContext viewContext;
		ModelMetadata metadata;
		public MVCxTextBox()
			: this(null) {
		}
		protected internal MVCxTextBox(ViewContext viewContext)
			: this(viewContext, null) {
		}
		protected internal MVCxTextBox(ViewContext viewContext, ModelMetadata metadata)
			: base() {
			this.viewContext = viewContext ?? HtmlHelperExtension.ViewContext;
			this.metadata = metadata;
		}
		public new TextBoxProperties Properties {
			get { return base.Properties; }
		}
		protected ViewContext ViewContext {
			get { return viewContext; }
		}
		protected internal ViewDataDictionary ViewData {
			get { return (ViewContext != null) ? ViewContext.ViewData : null; }
		}
		protected ModelMetadata Metadata {
			get { return metadata; }
		}
		public override bool IsLoading() {
			return false;
		}
		public override void RegisterEditorIncludeScripts() {
			base.RegisterEditorIncludeScripts();
			RegisterIncludeScript(typeof(MVCxTextBox), Utils.UtilsScriptResourceName);
		}
		protected override bool HasOnTextChanged() {
			return true;
		}
		public bool ShowModelErrors { get; set; }
		protected override bool IsCustomValidationEnabled {
			get { return ShowModelErrors || base.IsCustomValidationEnabled; }
		}
		protected override TextBoxControl CreateTextBoxControl() {
			return new MVCxTextBoxControl(this, Metadata);
		}
		#region Control
		public class MVCxTextBoxControl : TextBoxControl {
			public MVCxTextBoxControl(MVCxTextBox textBox, ModelMetadata metadata)
				: base(textBox) {
				Metadata = metadata;
			}
			protected ModelMetadata Metadata { get; private set; }
			protected override void PrepareInputControl(InputControl input) {
				base.PrepareInputControl(input);
				ExtensionsHelper.SetUnobtrusiveValidationAttributes(input, Edit.ID, Metadata);
			}
		}
		#endregion
	}
	[ToolboxItem(false)]
	public class MVCxTimeEdit : ASPxTimeEdit {
		ViewContext viewContext;
		ModelMetadata metadata;
		public MVCxTimeEdit()
			: this(null) {
		}
		protected internal MVCxTimeEdit(ViewContext viewContext)
			: this(viewContext, null) {
		}
		protected internal MVCxTimeEdit(ViewContext viewContext, ModelMetadata metadata)
			: base() {
			this.viewContext = viewContext ?? HtmlHelperExtension.ViewContext;
			this.metadata = metadata;
		}
		public new TimeEditProperties Properties {
			get { return base.Properties; }
		}
		protected ViewContext ViewContext {
			get { return viewContext; }
		}
		protected internal ViewDataDictionary ViewData {
			get { return (ViewContext != null) ? ViewContext.ViewData : null; }
		}
		protected ModelMetadata Metadata {
			get { return metadata; }
		}
		public override bool IsLoading() {
			return false;
		}
		public override void RegisterEditorIncludeScripts() {
			base.RegisterEditorIncludeScripts();
			RegisterIncludeScript(typeof(MVCxTimeEdit), Utils.UtilsScriptResourceName);
		}
		protected override bool HasOnTextChanged() {
			return true;
		}
		public bool ShowModelErrors { get; set; }
		protected override bool IsCustomValidationEnabled {
			get { return ShowModelErrors || base.IsCustomValidationEnabled; }
		}
		protected override SpinEditControl CreateSpinEditControl() {
			return new MVCxSpinEdit.MVCxSpinEditControl(this, Metadata);
		}
	}
	[ToolboxItem(false)]
	public class MVCxTokenBox : ASPxTokenBox {
		ViewContext viewContext;
		ModelMetadata metadata;
		public MVCxTokenBox()
			: this(null) {
		}
		protected internal MVCxTokenBox(ViewContext viewContext)
			: this(viewContext, null) {
		}
		protected internal MVCxTokenBox(ViewContext viewContext, ModelMetadata metadata)
			: base() {
			this.viewContext = viewContext ?? HtmlHelperExtension.ViewContext;
			this.metadata = metadata;
		}
		public object CallbackRouteValues { get; set; }
		public new TokenBoxProperties Properties {
			get { return base.Properties; }
		}
		protected ViewContext ViewContext {
			get { return viewContext; }
		}
		protected internal ViewDataDictionary ViewData {
			get { return (ViewContext != null) ? ViewContext.ViewData : null; }
		}
		protected ModelMetadata Metadata {
			get { return metadata; }
		}
		public override bool IsCallback {
			get { return MvcUtils.CallbackName == ID; }
		}
		protected internal override bool IsCallBacksEnabled() {
			return CallbackRouteValues != null;
		}
		protected override void GetCreateClientObjectScript(StringBuilder stb, string localVarName, string clientName) {
			base.GetCreateClientObjectScript(stb, localVarName, clientName);
			if(CallbackRouteValues != null)
				stb.Append(localVarName + ".callbackUrl=\"" + Utils.GetUrl(CallbackRouteValues) + "\";\n");
		}
		protected override string GetClientObjectClassName() {
			return "MVCxClientTokenBox";
		}
		public override void RegisterEditorIncludeScripts() {
			base.RegisterEditorIncludeScripts();
			RegisterIncludeScript(typeof(MVCxTokenBox), Utils.UtilsScriptResourceName);
			RegisterIncludeScript(typeof(MVCxTokenBox), Utils.TokenBoxScriptResourceName);
		}
		protected override bool HasOnTextChanged() {
			return true;
		}
		public bool ShowModelErrors { get; set; }
		protected override bool IsCustomValidationEnabled {
			get { return ShowModelErrors || base.IsCustomValidationEnabled; }
		}
		protected internal new void ResetControlHierarchy() {
			base.ResetControlHierarchy();
		}
		protected override DropDownControlBase CreateDropDownControl() {
			return new MVCxTokenBoxControl(this, Metadata);
		}
		#region Control
		public class MVCxTokenBoxControl : TokenBoxControl {
			public MVCxTokenBoxControl(MVCxTokenBox edit, ModelMetadata metadata)
				: base(edit) {
				Metadata = metadata;
			}
			protected ModelMetadata Metadata { get; private set; }
			protected override void PrepareInputControl(InputControl input) {
				base.PrepareInputControl(input);
				ExtensionsHelper.SetUnobtrusiveValidationAttributes(input, Edit.ID, Metadata);
			}
		}
		#endregion
	}
	[ToolboxItem(false)]
	public class MVCxTrackBar : ASPxTrackBar {
		ViewContext viewContext;
		ModelMetadata metadata;
		public MVCxTrackBar()
			: base() {
		}
		protected internal MVCxTrackBar(ViewContext viewContext)
			: this(viewContext, null) {
		}
		protected internal MVCxTrackBar(ViewContext viewContext, ModelMetadata metadata)
			: base() {
			this.viewContext = viewContext ?? HtmlHelperExtension.ViewContext;
			this.metadata = metadata;
		}
		protected ViewContext ViewContext {
			get { return viewContext; }
		}
		protected internal ViewDataDictionary ViewData {
			get { return (ViewContext != null) ? ViewContext.ViewData : null; }
		}
		protected ModelMetadata Metadata {
			get { return metadata; }
		}
		public bool ShowModelErrors { get; set; }
		protected override bool IsCustomValidationEnabled {
			get { return ShowModelErrors || base.IsCustomValidationEnabled; }
		}
		public new TrackBarProperties Properties {
			get { return base.Properties; }
		}
		public override bool IsLoading() {
			return false;
		}
		protected override TrackBarControl CreateTrackBarControl() {
			return new MVCxTrackBarControl(this, Metadata);
		}
		#region Control
		public class MVCxTrackBarControl : TrackBarControl {
			public MVCxTrackBarControl(MVCxTrackBar trackBar, ModelMetadata metadata)
				: base(trackBar) {
				Metadata = metadata;
			}
			protected ModelMetadata Metadata { get; private set; }
			protected override void PrepareInput() {
				base.PrepareInput();
				ExtensionsHelper.SetUnobtrusiveValidationAttributes(KbInput.Input, TrackBar.ID, Metadata);
			}
		}
		#endregion
	}
	[ToolboxItem(false)]
	public class MVCxValidationEdit : ASPxEdit {
		public MVCxValidationEdit()
			: base() {
		}
		protected override EditPropertiesBase CreateProperties() {
			return new MVCxValidationEditProperties();
		}
	}
	public class MVCxValidationEditProperties : EditProperties {
		protected override ASPxEditBase CreateEditInstance() {
			return new MVCxValidationEdit();
		}
	}
}
