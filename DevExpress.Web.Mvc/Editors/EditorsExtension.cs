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
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using DevExpress.Web.Internal;
namespace DevExpress.Web.Mvc {
	using DevExpress.Web.Mvc.Internal;
	public abstract class EditorExtension: ExtensionBase {
		static Dictionary<string, string> errorTextsStaticObj = new Dictionary<string, string>();
		static MVCxValidationEdit validationEdit;
		static ValidationSettings emptyValidationSettings = ValidationSettings.CreateValidationSettings(null);
		public EditorExtension(EditorSettings settings)
			: base(settings) {
		}
		public EditorExtension(EditorSettings settings, ViewContext viewContext)
			: base(settings, viewContext) {
		}
		protected internal EditorExtension(EditorSettings settings, ViewContext viewContext, ModelMetadata metadata)
			: base(settings, viewContext, metadata) {
		}
		protected internal new ASPxEditBase Control {
			get { return (ASPxEditBase)base.Control; }
		}
		protected abstract EditPropertiesBase Properties {
			get;
		}
		protected internal new EditorSettings Settings {
			get { return (EditorSettings)base.Settings; }
		}
		internal static Dictionary<string, string> ErrorTexts {
			get {
				if(Context == null) return errorTextsStaticObj;
				return HttpUtils.GetContextObject<Dictionary<string, string>>("DXEditorsErrorTexts");
			}
		}
		public virtual EditorExtension Bind(object value) {
			Control.Value = value;
			return this;
		}
		public EditorExtension Bind(object dataObject, string propertyName) {
			return Bind(ReflectionUtils.GetPropertyValue(dataObject, propertyName));
		}
		public static T GetValue<T>(string name) {
			return GetValue<T>(name: name, maskSettings: null);
		}
		public static T GetValue<T>(string name, MaskSettings maskSettings) {
			return GetValue<T>(name: name, validationSettings: null, maskSettings: maskSettings);
		}
		public static T GetValue<T>(string name, ValidationSettings validationSettings) {
			return GetValue<T>(name: name, validationSettings: validationSettings, maskSettings: null);
		}
		public static T GetValue<T>(string name, ValidationSettings validationSettings, MaskSettings maskSettings) {
			return GetValue<T>(name: name, validationSettings: validationSettings, maskSettings: maskSettings, validationDelegate: null);
		}
		public static T GetValue<T>(string name, ValidationSettings validationSettings, EventHandler<ValidationEventArgs> validationDelegate) {
			bool dummy = true;
			return GetValue<T>(name, validationSettings, validationDelegate, ref dummy);
		}
		public static T GetValue<T>(string name, ValidationSettings validationSettings, EventHandler<ValidationEventArgs> validationDelegate, MaskSettings maskSettings) {
			bool dummy = true;
			return GetValue<T>(name, validationSettings, validationDelegate, maskSettings, ref dummy);
		}
		public static T GetValue<T>(string name, ValidationSettings validationSettings, EventHandler<ValidationEventArgs> validationDelegate, ref bool isValid) {
			return GetValue<T>(name, validationSettings, validationDelegate, null, ref isValid);
		}
		public static T GetValue<T>(string name, ValidationSettings validationSettings, EventHandler<ValidationEventArgs> validationDelegate, MaskSettings maskSettings, ref bool isValid) {
			T value = EditorValueProvider.GetValue<T>(name);
			if(validationSettings != null || validationDelegate != null) {
				if(validationEdit == null)
					validationEdit = new MVCxValidationEdit();
				validationEdit.Value = value;
				validationEdit.ID = name;
				validationEdit.ValidationSettings.Assign(validationSettings ?? emptyValidationSettings);
				if(validationDelegate != null)
					validationEdit.Validation += validationDelegate;
				try {
					validationEdit.Validate();
					isValid = isValid && validationEdit.IsValid;
					if(!validationEdit.IsValid)
						ErrorTexts[name] = validationEdit.ErrorText;
					else if(ErrorTexts.ContainsKey(name))
						ErrorTexts.Remove(name);
				}
				finally {
					if(validationDelegate != null)
						validationEdit.Validation -= validationDelegate;
				}
			}
			if(maskSettings != null && value is string)
				isValid = isValid && MaskValidator.IsValueValid(value as string, maskSettings);
			return value;
		}
		protected override void AssignInitialProperties() {
			Properties.Assign(Settings.Properties); 
			Control.ClientEnabled = Settings.ClientEnabled;
			Control.ClientVisible = Settings.ClientVisible;
			Control.CustomJSProperties += Settings.CustomJSProperties;
			base.AssignInitialProperties();
		}
		protected internal override void PrepareControl() {
			base.PrepareControl();
			ASPxEdit edit = Control as ASPxEdit;
			if(edit != null && !string.IsNullOrEmpty(edit.ID) && ErrorTexts.ContainsKey(edit.ID)) {
				edit.IsValid = false;
				edit.ErrorText = ErrorTexts[edit.ID];
			}
		}
		protected void PrepareValidationForEditor(bool showModelErrors) {
			PrepareUnobtrusiveValidationForNativeEditor();
			PrepareModelValidationForEditor(showModelErrors);
		}
		void PrepareUnobtrusiveValidationForNativeEditor() {
			if (Control.IsNativeRender())
				ExtensionsHelper.SetUnobtrusiveValidationAttributes(Control, Control.ID, Metadata);
		}
		void PrepareModelValidationForEditor(bool showModelErrors) {
			ASPxEdit editor = Control as ASPxEdit;
			if(editor == null || ViewContext == null || !showModelErrors )
				return;
			AddValidationRulesForEditor();
			if(!ModelState.IsValidField(editor.ClientID)) {
				editor.IsValid = false;
				editor.ErrorText = ModelState[editor.ClientID].GetErrorMessage();
			}
		}
		void AddValidationRulesForEditor() {
			if(!ViewContext.ClientValidationEnabled || FormContext == null)
				return;
			FieldValidationMetadata validationMetadataForEditor = FormContext.GetValidationMetadataForField(Control.ID, true);
			IEnumerable<ModelValidator> validators = ModelValidatorProviders.Providers.GetValidators(ModelMetadata.FromStringExpression(Control.ID, ViewData), ViewContext);
			foreach(ModelValidator validator in validators) {
				foreach(ModelClientValidationRule rule in validator.GetClientValidationRules()) {
					validationMetadataForEditor.ValidationRules.Add(rule);
				};
			}
		}
	}
	public class BinaryImageEditExtension : EditorExtension {
		protected internal const string LoadedUniqueIDKey = "MVCxBinaryImageUniqueID";
		public BinaryImageEditExtension(BinaryImageEditSettings settings)
			: base(settings) {
		}
		public BinaryImageEditExtension(BinaryImageEditSettings settings, ViewContext viewContext)
			: base(settings, viewContext) {
		}
		public BinaryImageEditExtension(BinaryImageEditSettings settings, ViewContext viewContext, ModelMetadata modelMetadata)
			: base(settings, viewContext, modelMetadata) {
		}
		protected internal new MVCxBinaryImage Control {
			get { return (MVCxBinaryImage)base.Control; }
		}
		protected override EditPropertiesBase Properties {
			get { return Control.Properties; }
		}
		protected internal new BinaryImageEditSettings Settings {
			get { return (BinaryImageEditSettings)base.Settings; }
		}
		protected override void AssignInitialProperties() {
			base.AssignInitialProperties();
			if(Settings.ContentBytesAssigned)
				Control.ContentBytes = Settings.ContentBytes;
		}
		protected internal override void PrepareControl() {
			base.PrepareControl();
			if(Control.Properties.EditingSettings.Enabled) {
				Control.EnsureChildControls();
				Control.UploadControl.EnsureUploaded();
			}
		}
		protected override ASPxWebControl CreateControl() {
			return new MVCxBinaryImage();
		}
		protected override bool IsSimpleIDsRenderModeSupported() {
			BinaryImageEditProperties properties = (BinaryImageEditProperties)Properties;
			return properties.ClientSideEvents.IsEmpty() && !properties.EnableClientSideAPI &&
				string.IsNullOrEmpty(Properties.ClientInstanceName) && Settings.ClientVisible;
		}
		public static ContentResult GetCallbackResult() {
			return GetCallbackResultInternal(null, null);
		}
		public static ContentResult GetCallbackResult(BinaryStorageMode storageMode) {
			return GetCallbackResultInternal(storageMode, null);
		}
		public static ContentResult GetCallbackResult(BinaryStorageMode storageMode, BinaryImageUploadValidationSettings uploadValidationSettings) {
			return GetCallbackResultInternal(storageMode, uploadValidationSettings);
		}
		static ContentResult GetCallbackResultInternal(BinaryStorageMode? storageMode, BinaryImageUploadValidationSettings uploadValidationSettings) {
			string extensionName = GetExtensionName();
			if(string.IsNullOrEmpty(extensionName))
				return null;
			var extension = new BinaryImageEditExtension(CreateSettings(extensionName, storageMode, uploadValidationSettings));
			extension.PrepareControl();
			if(IsUploadCallback())
				return new ContentResult();
			extension.LoadPostData();
			extension.CallbackEventHandler.RaiseCallbackEvent(MvcUtils.CallbackArgument);
			return new ContentResult { Content = extension.CallbackEventHandler.GetCallbackResult()};
		}
		static BinaryImageEditSettings CreateSettings(string name, BinaryStorageMode? storageMode, BinaryImageUploadValidationSettings uploadValidationSettings) {
			BinaryImageEditSettings settings = new BinaryImageEditSettings();
			settings.Name = name;
			settings.Properties.EditingSettings.Enabled = true;
			if(uploadValidationSettings != null)
				settings.Properties.EditingSettings.UploadSettings.UploadValidationSettings.Assign(uploadValidationSettings);
			if(storageMode != null)
				settings.Properties.BinaryStorageMode = storageMode.Value;
			return settings;
		}
		static string GetExtensionName() {
			var request = HttpUtils.GetRequest();
			string uniqueID = request[LoadedUniqueIDKey] as string;
			if(!String.IsNullOrEmpty(uniqueID))
				return uniqueID;
			if(!string.IsNullOrEmpty(MvcUtils.CallbackName)) {
				return MvcUtils.CallbackName;
			}
			string uploadID = request[RenderUtils.HelperUploadingCallbackQueryParamName] ?? string.Empty;
			return uploadID.Replace(MVCxBinaryImage.UploadControlPostfix, "");
		}
		static bool IsUploadCallback(){
			var request = HttpUtils.GetRequest();
			return !string.IsNullOrEmpty(request[RenderUtils.HelperUploadingCallbackQueryParamName]);
		}
	}
	public class ButtonEditExtension : EditorExtension {
		public ButtonEditExtension(ButtonEditSettings settings)
			: base(settings) {
		}
		public ButtonEditExtension(ButtonEditSettings settings, ViewContext viewContext)
			: base(settings, viewContext) {
		}
		public ButtonEditExtension(ButtonEditSettings settings, ViewContext viewContext, ModelMetadata metadata)
			: base(settings, viewContext, metadata) {
		}
		protected internal new MVCxButtonEdit Control {
			get { return (MVCxButtonEdit)base.Control; }
		}
		protected override EditPropertiesBase Properties {
			get { return Control.Properties; }
		}
		protected internal new ButtonEditSettings Settings {
			get { return (ButtonEditSettings)base.Settings; }
		}
		protected override void AssignInitialProperties() {
			base.AssignInitialProperties();
			Control.AutoCompleteType = Settings.AutoCompleteType;
			Control.DisplayFormatString = Properties.DisplayFormatString;
			Control.ReadOnly = Settings.ReadOnly;
			if(Settings.TextAssigned)
				Control.Text = Settings.Text;
			Control.RightToLeft = Settings.RightToLeft;
			Control.ShowModelErrors = Settings.ShowModelErrors;
		}
		protected override void AssignRenderProperties() {
			base.AssignRenderProperties();
			Control.ButtonTemplate = ContentControlTemplate<TemplateContainerBase>.Create(
				Settings.ButtonTemplateContent, Settings.ButtonTemplateContentMethod, typeof(TemplateContainerBase));
		}
		protected override ASPxWebControl CreateControl() {
			return new MVCxButtonEdit(ViewContext, Metadata);
		}
		protected internal override void PrepareControl() {
			base.PrepareControl();
			PrepareValidationForEditor(Settings.ShowModelErrors);
		}
	}
	public class CalendarExtension : EditorExtension {
		public CalendarExtension(CalendarSettings settings)
			: base(settings) {
		}
		public CalendarExtension(CalendarSettings settings, ViewContext viewContext)
			: base(settings, viewContext) {
		}
		public CalendarExtension(CalendarSettings settings, ViewContext viewContext, ModelMetadata metadata)
			: base(settings, viewContext, metadata) {
		}
		protected internal new MVCxCalendar Control {
			get { return (MVCxCalendar)base.Control; }
		}
		protected override EditPropertiesBase Properties {
			get { return Control.Properties; }
		}
		protected internal new CalendarSettings Settings {
			get { return (CalendarSettings)base.Settings; }
		}
		protected override void AssignInitialProperties() {
			base.AssignInitialProperties();
			if(Settings.SelectedDateAssigned)
				Control.SelectedDate = Settings.SelectedDate;
			foreach(DateTime date in Settings.SelectedDates)
				Control.SelectedDates.Add(date);
			Control.VisibleDate = Settings.VisibleDate;
			Control.RenderIFrameForPopupElements = Settings.RenderIFrameForPopupElements;
			Control.ReadOnly = Settings.ReadOnly;
			Control.RightToLeft = Settings.RightToLeft;
			Control.ShowModelErrors = Settings.ShowModelErrors;
			Control.SettingsLoadingPanel.Assign(Settings.SettingsLoadingPanel);			
			Control.LoadingPanelImage.Assign(Settings.LoadingPanelImage);
			Control.LoadingPanelStyle.Assign(Settings.LoadingPanelStyle);
			Control.CallbackRouteValues = Settings.CallbackRouteValues;
			Control.DayCellInitialize += Settings.DayCellInitialize;
			Control.DayCellCreated += Settings.DayCellCreated;
			Control.DayCellPrepared += Settings.DayCellPrepared;
		}
		protected override ASPxWebControl CreateControl() {
			return new MVCxCalendar(ViewContext, Metadata);
		}
		protected internal override void PrepareControl() {
			base.PrepareControl();
			PrepareValidationForEditor(Settings.ShowModelErrors);
		}
		protected override void RenderCallbackResultControl() {
			string result = HtmlConvertor.ToJSON(Control.GetCallbackRenderResult());
			RenderString(result);
		}
		protected override Control GetCallbackResultControl() {
			return Control.GetCallbackResultControl();
		}
	}
	public class CheckBoxExtension : EditorExtension {
		public CheckBoxExtension(CheckBoxSettings settings)
			: base(settings) {
		}
		public CheckBoxExtension(CheckBoxSettings settings, ViewContext viewContext)
			: base(settings, viewContext) {
		}
		public CheckBoxExtension(CheckBoxSettings settings, ViewContext viewContext, ModelMetadata metadata)
			: base(settings, viewContext, metadata) {
		}
		protected internal new MVCxCheckBox Control {
			get { return (MVCxCheckBox)base.Control; }
		}
		protected override EditPropertiesBase Properties {
			get { return Control.Properties; }
		}
		protected internal new CheckBoxSettings Settings {
			get { return (CheckBoxSettings)base.Settings; }
		}
		protected override void AssignInitialProperties() {
			base.AssignInitialProperties();
			if(Settings.CheckedAssigned)
				Control.Checked = Settings.Checked;
			Control.ReadOnly = Settings.ReadOnly;
			Control.Native = Settings.Native;
			Control.RightToLeft = Settings.RightToLeft;
			Control.Text = Settings.Text;
			Control.ShowModelErrors = Settings.ShowModelErrors;
		}
		protected override ASPxWebControl CreateControl() {
			return new MVCxCheckBox(ViewContext, Metadata);
		}
		protected internal override void PrepareControl() {
			base.PrepareControl();
			PrepareValidationForEditor(Settings.ShowModelErrors);
		}
	}
	public class ColorEditExtension : EditorExtension {
		public ColorEditExtension(ColorEditSettings settings)
			: base(settings) {
		}
		public ColorEditExtension(ColorEditSettings settings, ViewContext viewContext)
			: base(settings, viewContext) {
		}
		public ColorEditExtension(ColorEditSettings settings, ViewContext viewContext, ModelMetadata metadata)
			: base(settings, viewContext, metadata) {
		}
		protected internal new MVCxColorEdit Control {
			get { return (MVCxColorEdit)base.Control; }
		}
		protected override EditPropertiesBase Properties {
			get { return Control.Properties; }
		}
		protected internal new ColorEditSettings Settings {
			get { return (ColorEditSettings)base.Settings; }
		}
		protected override void AssignInitialProperties() {
			base.AssignInitialProperties();
			Control.DisplayFormatString = Properties.DisplayFormatString;
			if(Settings.ColorAssigned)
				Control.Color = Settings.Color;
			Control.ReadOnly = Settings.ReadOnly;
			Control.RightToLeft = Settings.RightToLeft;
			Control.ShowModelErrors = Settings.ShowModelErrors;
		}
		protected override void AssignRenderProperties() {
			base.AssignRenderProperties();
			Control.ButtonTemplate = ContentControlTemplate<TemplateContainerBase>.Create(
				Settings.ButtonTemplateContent, Settings.ButtonTemplateContentMethod, typeof(TemplateContainerBase));
		}
		protected override ASPxWebControl CreateControl() {
			return new MVCxColorEdit(ViewContext, Metadata);
		}
		protected internal override void PrepareControl() {
			base.PrepareControl();
			PrepareValidationForEditor(Settings.ShowModelErrors);
		}
	}
	public delegate object ItemsRequestedByFilterConditionMethod(ListEditItemsRequestedByFilterConditionEventArgs args);
	public delegate object ItemRequestedByValueMethod(ListEditItemRequestedByValueEventArgs args);
	public class ComboBoxExtension : EditorExtension {
		ItemsRequestedByFilterConditionMethod ItemsRequestedByFilterConditionMethod { get; set; }
		ItemRequestedByValueMethod ItemRequestedByValueMethod { get; set; }
		bool HasItemRequestEvents { get { return ItemsRequestedByFilterConditionMethod != null || ItemRequestedByValueMethod != null; } }
		public ComboBoxExtension(ComboBoxSettings settings)
			: base(settings) {
		}
		public ComboBoxExtension(ComboBoxSettings settings, ViewContext viewContext)
			: base(settings, viewContext) {
		}
		public ComboBoxExtension(ComboBoxSettings settings, ViewContext viewContext, ModelMetadata metadata)
			: base(settings, viewContext, metadata) {
		}
		protected internal new MVCxComboBox Control {
			get { return (MVCxComboBox)base.Control; }
		}
		protected override EditPropertiesBase Properties {
			get { return Control.Properties; }
		}
		protected internal new ComboBoxSettings Settings {
			get { return (ComboBoxSettings)base.Settings; }
		}
		public override EditorExtension Bind(object value) {
			base.Bind(value);
			if(HasItemRequestEvents && !IsCallback())
				Control.DataBind();
			return this;
		}
		public ComboBoxExtension BindList(object dataObject) {
			BindInternal(dataObject);
			return this;
		}
		public ComboBoxExtension BindList(ItemsRequestedByFilterConditionMethod itemsRequestedByFilterConditionMethod,
			ItemRequestedByValueMethod itemRequestedByValueMethod) {
			ItemsRequestedByFilterConditionMethod = itemsRequestedByFilterConditionMethod;
			Control.ItemsRequestedByFilterCondition += ItemsRequestedByFilterConditionEventHandler;
			ItemRequestedByValueMethod = itemRequestedByValueMethod;
			Control.ItemRequestedByValue += ItemRequestedByValueEventHandler;
			return this;
		}
		protected override void AssignInitialProperties() {
			base.AssignInitialProperties();
			Control.CallbackRouteValues = Settings.CallbackRouteValues;
			Control.DisplayFormatString = Properties.DisplayFormatString;
			Control.LoadingPanelImage.Assign(Settings.LoadingPanelImage);
			Control.LoadingPanelStyle.Assign(Settings.LoadingPanelStyle);
			Control.ReadOnly = Settings.ReadOnly;
			Control.RightToLeft = Settings.RightToLeft;
			Control.SettingsLoadingPanel.Assign(Settings.SettingsLoadingPanel);
			if(Settings.SelectedIndexAssigned)
				Control.SelectedIndex = Settings.SelectedIndex;
			Control.ShowModelErrors = Settings.ShowModelErrors;
			Control.Properties.EnableCallbackMode = Settings.CallbackRouteValues != null;  
		}
		protected override void AssignRenderProperties() {
			base.AssignRenderProperties();
			Control.ButtonTemplate = ContentControlTemplate<TemplateContainerBase>.Create(
				Settings.ButtonTemplateContent, Settings.ButtonTemplateContentMethod, typeof(TemplateContainerBase));
		}
		void ItemsRequestedByFilterConditionEventHandler(object source, ListEditItemsRequestedByFilterConditionEventArgs e) {
			if(ItemsRequestedByFilterConditionMethod == null) return;
			object dataSource = ItemsRequestedByFilterConditionMethod(e);
			if(dataSource != null) {
				MVCxComboBox comboBox = (MVCxComboBox)source;
				comboBox.DataSource = dataSource;
				comboBox.DataBind();
			}
		}
		void ItemRequestedByValueEventHandler(object source, ListEditItemRequestedByValueEventArgs e) {
			if(ItemRequestedByValueMethod == null) return;
			object dataSource = ItemRequestedByValueMethod(e);
			if(dataSource != null) {
				MVCxComboBox comboBox = (MVCxComboBox)source;
				comboBox.DataSource = dataSource;
				comboBox.DataBind();
			}
		}
		protected override ASPxWebControl CreateControl() {
			return new MVCxComboBox(ViewContext, Metadata);
		}
		protected internal override void PrepareControl() {
			base.PrepareControl();
			PrepareValidationForEditor(Settings.ShowModelErrors);
		}
	}
	public class DateEditExtension : EditorExtension {
		public DateEditExtension(DateEditSettings settings)
			: base(settings) {
		}
		public DateEditExtension(DateEditSettings settings, ViewContext viewContext)
			: base(settings, viewContext) {
		}
		public DateEditExtension(DateEditSettings settings, ViewContext viewContext, ModelMetadata metadata)
			: base(settings, viewContext, metadata) {
		}
		protected internal new MVCxDateEdit Control {
			get { return (MVCxDateEdit)base.Control; }
		}
		protected override EditPropertiesBase Properties {
			get { return Control.Properties; }
		}
		protected internal new DateEditSettings Settings {
			get { return (DateEditSettings)base.Settings; }
		}
		protected override void AssignInitialProperties() {
			base.AssignInitialProperties();
			Control.CallbackRouteValues = Settings.CallbackRouteValues;
			Control.DisplayFormatString = Properties.DisplayFormatString;
			if(Settings.DateAssigned)
				Control.Date = Settings.Date;
			Control.PopupCalendarOwnerID = Settings.PopupCalendarOwnerName;
			Control.ReadOnly = Settings.ReadOnly;
			Control.RightToLeft = Settings.RightToLeft;
			Control.ShowModelErrors = Settings.ShowModelErrors;
			Control.CalendarDayCellCreated += Settings.CalendarDayCellCreated;
			Control.CalendarDayCellInitialize += Settings.CalendarDayCellInitialize;
			Control.CalendarDayCellPrepared += Settings.CalendarDayCellPrepared;
		}
		protected override void AssignRenderProperties() {
			base.AssignRenderProperties();
			Control.ButtonTemplate = ContentControlTemplate<TemplateContainerBase>.Create(
				Settings.ButtonTemplateContent, Settings.ButtonTemplateContentMethod, typeof(TemplateContainerBase));
		}
		protected override ASPxWebControl CreateControl() {
			return new MVCxDateEdit(ViewContext, Metadata);
		}
		protected internal override void PrepareControl() {
			base.PrepareControl();
			PrepareValidationForEditor(Settings.ShowModelErrors);
		}
		protected internal override bool IsCallback(){
			return Control.IsCallback;
		}
		protected override Control GetCallbackResultControl() {
			return Control.GetCallbackResultControl();
		}
		protected override void RenderCallbackResultControl() {
			string result = HtmlConvertor.ToJSON(Control.GetCallbackRenderResult());
			RenderString(result);
		}
		protected override void LoadPostDataInternal() {
			base.LoadPostDataInternal();
			Control.EnsureChildControls();
			(Control.Calendar as IPostBackDataHandler).LoadPostData("", HttpContext.Current.Request.Params);
		}
	}
	public class DropDownEditExtension : EditorExtension {
		public DropDownEditExtension(DropDownEditSettings settings)
			: base(settings) {
		}
		public DropDownEditExtension(DropDownEditSettings settings, ViewContext viewContext)
			: base(settings, viewContext) {
		}
		public DropDownEditExtension(DropDownEditSettings settings, ViewContext viewContext, ModelMetadata metadata)
			: base(settings, viewContext, metadata) {
		}
		protected internal new MVCxDropDownEdit Control {
			get { return (MVCxDropDownEdit)base.Control; }
		}
		protected override EditPropertiesBase Properties {
			get { return Control.Properties; }
		}
		protected internal new DropDownEditSettings Settings {
			get { return (DropDownEditSettings)base.Settings; }
		}
		protected override void AssignInitialProperties() {
			base.AssignInitialProperties();
			Control.DisplayFormatString = Properties.DisplayFormatString;
			Control.RenderIFrameForPopupElements = Settings.RenderIFrameForPopupElements;
			Control.ReadOnly = Settings.ReadOnly;
			Control.RightToLeft = Settings.RightToLeft;
			if(Settings.TextAssigned)
				Control.Text = Settings.Text;
			Control.ShowModelErrors = Settings.ShowModelErrors;
		}
		protected override void AssignRenderProperties() {
			base.AssignRenderProperties();
			Control.ButtonTemplate = ContentControlTemplate<TemplateContainerBase>.Create(
				Settings.ButtonTemplateContent, Settings.ButtonTemplateContentMethod, typeof(TemplateContainerBase));
			Control.DropDownWindowTemplate = ContentControlTemplate<TemplateContainerBase>.Create(
				Settings.DropDownWindowTemplateContent, Settings.DropDownWindowTemplateContentMethod, typeof(TemplateContainerBase));
		}
		protected override ASPxWebControl CreateControl() {
			return new MVCxDropDownEdit(ViewContext, Metadata);
		}
		protected internal override void PrepareControl() {
			base.PrepareControl();
			PrepareValidationForEditor(Settings.ShowModelErrors);
		}
	}
	public class HyperLinkExtension : EditorExtension {
		public HyperLinkExtension(HyperLinkSettings settings)
			: base(settings) {
		}
		public HyperLinkExtension(HyperLinkSettings settings, ViewContext viewContext)
			: base(settings, viewContext) {
		}
		public HyperLinkExtension(HyperLinkSettings settings, ViewContext viewContext, ModelMetadata metadata)
			: base(settings, viewContext, metadata) {
		}
		protected internal new MVCxHyperLink Control {
			get { return (MVCxHyperLink)base.Control; }
		}
		protected override EditPropertiesBase Properties {
			get { return Control.Properties; }
		}
		protected internal new HyperLinkSettings Settings {
			get { return (HyperLinkSettings)base.Settings; }
		}
		protected override void AssignInitialProperties() {
			base.AssignInitialProperties();
			if(Settings.NavigateUrlAssigned)
				Control.NavigateUrl = Settings.NavigateUrl;
			Control.RightToLeft = Settings.RightToLeft;
		}
		protected override ASPxWebControl CreateControl() {
			return new MVCxHyperLink();
		}
		protected override bool IsSimpleIDsRenderModeSupported() {
			HyperLinkProperties properties = (HyperLinkProperties)Properties;
			return properties.ClientSideEvents.IsEmpty() && !properties.EnableClientSideAPI &&
				string.IsNullOrEmpty(Properties.ClientInstanceName) && Settings.ClientVisible;
		}
	}
	public class ImageEditExtension : EditorExtension {
		public ImageEditExtension(ImageEditSettings settings)
			: base(settings) {
		}
		public ImageEditExtension(ImageEditSettings settings, ViewContext viewContext)
			: base(settings, viewContext) {
		}
		public ImageEditExtension(ImageEditSettings settings, ViewContext viewContext, ModelMetadata metadata)
			: base(settings, viewContext, metadata) {
		}
		protected internal new MVCxImage Control {
			get { return (MVCxImage)base.Control; }
		}
		protected override EditPropertiesBase Properties {
			get { return Control.Properties; }
		}
		protected internal new ImageEditSettings Settings {
			get { return (ImageEditSettings)base.Settings; }
		}
		protected override void AssignInitialProperties() {
			base.AssignInitialProperties();
			if(Settings.ImageUrlAssigned)
				Control.ImageUrl = Settings.ImageUrl;
		}
		protected override ASPxWebControl CreateControl() {
			return new MVCxImage();
		}
		protected override bool IsSimpleIDsRenderModeSupported() {
			ImageEditProperties properties = (ImageEditProperties)Properties;
			return properties.ClientSideEvents.IsEmpty() && !properties.EnableClientSideAPI &&
				string.IsNullOrEmpty(Properties.ClientInstanceName) && Settings.ClientVisible;
		}
	}
	public class LabelExtension : EditorExtension {
		public LabelExtension(LabelSettings settings)
			: base(settings) {
		}
		public LabelExtension(LabelSettings settings, ViewContext viewContext)
			: base(settings, viewContext) {
		}
		protected internal new MVCxLabel Control {
			get { return (MVCxLabel)base.Control; }
		}
		protected override EditPropertiesBase Properties {
			get { return Control.Properties; }
		}
		protected internal new LabelSettings Settings {
			get { return (LabelSettings)base.Settings; }
		}
		protected override void AssignInitialProperties() {
			base.AssignInitialProperties();
			Control.AssociatedControlID = Settings.AssociatedControlName;
			Control.RightToLeft = Settings.RightToLeft;
			if(Settings.TextAssigned)
				Control.Text = Settings.Text;
		}
		protected override ASPxWebControl CreateControl() {
			return new MVCxLabel();
		}
		protected override bool IsSimpleIDsRenderModeSupported() {
			LabelProperties properties = (LabelProperties)Properties;
			return properties.ClientSideEvents.IsEmpty() && !properties.EnableClientSideAPI &&
				string.IsNullOrEmpty(Properties.ClientInstanceName) && Settings.ClientVisible;
		}
	}
	public class ListBoxExtension : EditorExtension {
		public ListBoxExtension(ListBoxSettings settings)
			: base(settings) {
		}
		public ListBoxExtension(ListBoxSettings settings, ViewContext viewContext)
			: base(settings, viewContext) {
		}
		public ListBoxExtension(ListBoxSettings settings, ViewContext viewContext, ModelMetadata metadata)
			: base(settings, viewContext, metadata) {
		}
		public static T[] GetSelectedValues<T>(string name) {
			return EditorValueProvider.GetValue<T[]>(name);
		}
		protected internal new MVCxListBox Control {
			get { return (MVCxListBox)base.Control; }
		}
		protected override EditPropertiesBase Properties {
			get { return Control.Properties; }
		}
		protected internal new ListBoxSettings Settings {
			get { return (ListBoxSettings)base.Settings; }
		}
		public ListBoxExtension BindList(object dataObject) {
			BindInternal(dataObject);
			return this;
		}
		protected override void AssignInitialProperties() {
			base.AssignInitialProperties();
			Control.CallbackRouteValues = Settings.CallbackRouteValues;
			Control.ReadOnly = Settings.ReadOnly;
			Control.RightToLeft = Settings.RightToLeft;
			for(int i = 0; i < Settings.Properties.Items.Count; i++) {
				if (Settings.Properties.Items[i].Selected) {
					Control.Items[i].Selected = true;
					if (Control.SelectionMode == ListEditSelectionMode.Single)
						break;
				}
			}
			Control.SettingsLoadingPanel.Assign(Settings.SettingsLoadingPanel);
			if(Settings.SelectedIndexAssigned)
				Control.SelectedIndex = Settings.SelectedIndex;
			Control.ShowModelErrors = Settings.ShowModelErrors;
			Control.Properties.EnableCallbackMode = Settings.CallbackRouteValues != null;  
		}
		protected override ASPxWebControl CreateControl() {
			return new MVCxListBox(ViewContext, Metadata);
		}
		protected internal override void PrepareControl() {
			base.PrepareControl();
			PrepareValidationForEditor(Settings.ShowModelErrors);
		}
	}
	public class MemoExtension : EditorExtension {
		public MemoExtension(MemoSettings settings)
			: base(settings) {
		}
		public MemoExtension(MemoSettings settings, ViewContext viewContext)
			: base(settings, viewContext) {
		}
		public MemoExtension(MemoSettings settings, ViewContext viewContext, ModelMetadata metadata)
			: base(settings, viewContext, metadata) {
		}
		protected internal new MVCxMemo Control {
			get { return (MVCxMemo)base.Control; }
		}
		protected override EditPropertiesBase Properties {
			get { return Control.Properties; }
		}
		protected internal new MemoSettings Settings {
			get { return (MemoSettings)base.Settings; }
		}
		protected override void AssignInitialProperties() {
			base.AssignInitialProperties();
			Control.DisplayFormatString = Properties.DisplayFormatString;
			Control.ReadOnly = Settings.ReadOnly;
			Control.RightToLeft = Settings.RightToLeft;
			if(Settings.TextAssigned)
				Control.Text = Settings.Text;
			Control.ShowModelErrors = Settings.ShowModelErrors;
		}
		protected override ASPxWebControl CreateControl() {
			return new MVCxMemo(ViewContext, Metadata);
		}
		protected internal override void PrepareControl() {
			base.PrepareControl();
			PrepareValidationForEditor(Settings.ShowModelErrors);
		}
	}
	public class ProgressBarExtension : EditorExtension {
		public ProgressBarExtension(ProgressBarSettings settings)
			: base(settings) {
		}
		public ProgressBarExtension(ProgressBarSettings settings, ViewContext viewContext)
			: base(settings, viewContext) {
		}
		public ProgressBarExtension(ProgressBarSettings settings, ViewContext viewContext, ModelMetadata metadata)
			: base(settings, viewContext, metadata) {
		}
		protected internal new MVCxProgressBar Control {
			get { return (MVCxProgressBar)base.Control; }
		}
		protected override EditPropertiesBase Properties {
			get { return Control.Properties; }
		}
		protected internal new ProgressBarSettings Settings {
			get { return (ProgressBarSettings)base.Settings; }
		}
		protected override void AssignInitialProperties() {
			base.AssignInitialProperties();
			if(Settings.PositionAssigned)
				Control.Position = Settings.Position;
			Control.RightToLeft = Settings.RightToLeft;
		}
		protected override ASPxWebControl CreateControl() {
			return new MVCxProgressBar();
		}
	}
	public class RadioButtonExtension : EditorExtension {
		public RadioButtonExtension(RadioButtonSettings settings)
			: base(settings) {
		}
		public RadioButtonExtension(RadioButtonSettings settings, ViewContext viewContext)
			: base(settings, viewContext) {
		}
		public RadioButtonExtension(RadioButtonSettings settings, ViewContext viewContext, ModelMetadata metadata)
			: base(settings, viewContext, metadata) {
		}
		protected internal new MVCxRadioButton Control {
			get { return (MVCxRadioButton)base.Control; }
		}
		protected override EditPropertiesBase Properties {
			get { return Control.Properties; }
		}
		protected internal new RadioButtonSettings Settings {
			get { return (RadioButtonSettings)base.Settings; }
		}
		protected override void AssignInitialProperties() {
			base.AssignInitialProperties();
			if(Settings.CheckedAssigned)
				Control.Checked = Settings.Checked;
			Control.GroupName = Settings.GroupName;
			Control.ReadOnly = Settings.ReadOnly;
			Control.RightToLeft = Settings.RightToLeft;
			Control.Text = Settings.Text;
			Control.Native = Settings.Native;
			Control.ShowModelErrors = Settings.ShowModelErrors;
		}
		protected override ASPxWebControl CreateControl() {
			return new MVCxRadioButton(ViewContext, Metadata);
		}
		protected internal override void PrepareControl() {
			base.PrepareControl();
			PrepareValidationForEditor(Settings.ShowModelErrors);
		}
	}
	public class CheckBoxListExtension : EditorExtension {
		public CheckBoxListExtension(CheckBoxListSettings settings)
			: base(settings) {
		}
		public CheckBoxListExtension(CheckBoxListSettings settings, ViewContext viewContext)
			: base(settings, viewContext) {
		}
		public CheckBoxListExtension(CheckBoxListSettings settings, ViewContext viewContext, ModelMetadata metadata)
			: base(settings, viewContext, metadata) {
		}
		public static T[] GetSelectedValues<T>(string name) {
			return EditorValueProvider.GetValue<T[]>(name);
		}
		protected internal new MVCxCheckBoxList Control {
			get { return (MVCxCheckBoxList)base.Control; }
		}
		protected override EditPropertiesBase Properties {
			get { return Control.Properties; }
		}
		protected internal new CheckBoxListSettings Settings {
			get { return (CheckBoxListSettings)base.Settings; }
		}
		public CheckBoxListExtension BindList(object dataObject) {
			BindInternal(dataObject);
			return this;
		}
		public CheckBoxListExtension BindToXML(string fileName) {
			return BindToXML(fileName, string.Empty, string.Empty);
		}
		public CheckBoxListExtension BindToXML(string fileName, string xPath) {
			return BindToXML(fileName, xPath, string.Empty);
		}
		public CheckBoxListExtension BindToXML(string fileName, string xPath, string transformFileName) {
			BindToXMLInternal(fileName, xPath, transformFileName);
			return this;
		}
		protected override void AssignInitialProperties() {
			base.AssignInitialProperties();
			Control.ReadOnly = Settings.ReadOnly;
			Control.RightToLeft = Settings.RightToLeft;
			Control.Native = Settings.Native;
			for(int i = 0; i < Settings.Properties.Items.Count; i++) {
				if(Settings.Properties.Items[i].Selected)
					Control.Items[i].Selected = true;
			}
			if(Settings.SelectedIndexAssigned)
				Control.SelectedIndex = Settings.SelectedIndex;
			Control.ShowModelErrors = Settings.ShowModelErrors;
		}
		protected override ASPxWebControl CreateControl() {
			return new MVCxCheckBoxList(ViewContext);
		}
		protected internal override void PrepareControl() {
			base.PrepareControl();
			PrepareValidationForEditor(Settings.ShowModelErrors);
		}
	}
	public class RadioButtonListExtension : EditorExtension {
		public RadioButtonListExtension(RadioButtonListSettings settings)
			: base(settings) {
		}
		public RadioButtonListExtension(RadioButtonListSettings settings, ViewContext viewContext)
			: base(settings, viewContext) {
		}
		public RadioButtonListExtension(RadioButtonListSettings settings, ViewContext viewContext, ModelMetadata metadata)
			: base(settings, viewContext, metadata) {
		}
		protected internal new MVCxRadioButtonList Control {
			get { return (MVCxRadioButtonList)base.Control; }
		}
		protected override EditPropertiesBase Properties {
			get { return Control.Properties; }
		}
		protected internal new RadioButtonListSettings Settings {
			get { return (RadioButtonListSettings)base.Settings; }
		}
		public RadioButtonListExtension BindList(object dataObject) {
			BindInternal(dataObject);
			return this;
		}
		public RadioButtonListExtension BindToXML(string fileName) {
			return BindToXML(fileName, string.Empty, string.Empty);
		}
		public RadioButtonListExtension BindToXML(string fileName, string xPath) {
			return BindToXML(fileName, xPath, string.Empty);
		}
		public RadioButtonListExtension BindToXML(string fileName, string xPath, string transformFileName) {
			BindToXMLInternal(fileName, xPath, transformFileName);
			return this;
		}
		protected override void AssignInitialProperties() {
			base.AssignInitialProperties();
			Control.ReadOnly = Settings.ReadOnly;
			Control.RightToLeft = Settings.RightToLeft;
			Control.Native = Settings.Native;
			for (int i = 0; i < Settings.Properties.Items.Count; i++) {
				if (Settings.Properties.Items[i].Selected) {
					Control.Items[i].Selected = true;
					break;
				}
			}
			if(Settings.SelectedIndexAssigned)
				Control.SelectedIndex = Settings.SelectedIndex;
			Control.ShowModelErrors = Settings.ShowModelErrors;
		}
		protected override ASPxWebControl CreateControl() {
			return new MVCxRadioButtonList(ViewContext, Metadata);
		}
		protected internal override void PrepareControl() {
			base.PrepareControl();
			PrepareValidationForEditor(Settings.ShowModelErrors);
		}
	}
	public class SpinEditExtension : EditorExtension {
		public SpinEditExtension(SpinEditSettings settings)
			: base(settings) {
		}
		public SpinEditExtension(SpinEditSettings settings, ViewContext viewContext)
			: base(settings, viewContext) {
		}
		public SpinEditExtension(SpinEditSettings settings, ViewContext viewContext, ModelMetadata metadata)
			: base(settings, viewContext, metadata) {
		}
		protected internal new MVCxSpinEdit Control {
			get { return (MVCxSpinEdit)base.Control; }
		}
		protected override EditPropertiesBase Properties {
			get { return Control.Properties; }
		}
		protected internal new SpinEditSettings Settings {
			get { return (SpinEditSettings)base.Settings; }
		}
		protected override void AssignInitialProperties() {
			base.AssignInitialProperties();
			Control.DisplayFormatString = Properties.DisplayFormatString;
			if(Settings.NumberAssigned)
				Control.Number = Settings.Number;
			Control.ReadOnly = Settings.ReadOnly;
			Control.RightToLeft = Settings.RightToLeft;
			Control.ShowModelErrors = Settings.ShowModelErrors;
		}
		protected override void AssignRenderProperties() {
			base.AssignRenderProperties();
			Control.ButtonTemplate = ContentControlTemplate<TemplateContainerBase>.Create(
				Settings.ButtonTemplateContent, Settings.ButtonTemplateContentMethod, typeof(TemplateContainerBase));
		}
		protected override ASPxWebControl CreateControl() {
			return new MVCxSpinEdit(ViewContext, Metadata);
		}
		protected internal override void PrepareControl() {
			base.PrepareControl();
			PrepareValidationForEditor(Settings.ShowModelErrors);
		}
	}
	public class TextBoxExtension : EditorExtension {
		public TextBoxExtension(TextBoxSettings settings)
			: base(settings) {
		}
		public TextBoxExtension(TextBoxSettings settings, ViewContext viewContext)
			: base(settings, viewContext) {
		}
		public TextBoxExtension(TextBoxSettings settings, ViewContext viewContext, ModelMetadata metadata)
			: base(settings, viewContext, metadata) {
		}
		protected internal new MVCxTextBox Control {
			get { return (MVCxTextBox)base.Control; }
		}
		protected override EditPropertiesBase Properties {
			get { return Control.Properties; }
		}
		protected internal new TextBoxSettings Settings {
			get { return (TextBoxSettings)base.Settings; }
		}
		protected override void AssignInitialProperties() {
			base.AssignInitialProperties();
			Control.AutoCompleteType = Settings.AutoCompleteType;
			Control.DisplayFormatString = Properties.DisplayFormatString;
			Control.ReadOnly = Settings.ReadOnly;
			Control.RightToLeft = Settings.RightToLeft;
			if(Settings.TextAssigned)
				Control.Text = Settings.Text;
			Control.ShowModelErrors = Settings.ShowModelErrors;
		}
		protected override ASPxWebControl CreateControl() {
			return new MVCxTextBox(ViewContext, Metadata);
		}
		protected internal override void PrepareControl() {
			base.PrepareControl();
			PrepareValidationForEditor(Settings.ShowModelErrors);
		}
	}
	public class TimeEditExtension : EditorExtension {
		public TimeEditExtension(TimeEditSettings settings)
			: base(settings) {
		}
		public TimeEditExtension(TimeEditSettings settings, ViewContext viewContext)
			: base(settings, viewContext) {
		}
		public TimeEditExtension(TimeEditSettings settings, ViewContext viewContext, ModelMetadata metadata)
			: base(settings, viewContext, metadata) {
		}
		protected internal new MVCxTimeEdit Control {
			get { return (MVCxTimeEdit)base.Control; }
		}
		protected override EditPropertiesBase Properties {
			get { return Control.Properties; }
		}
		protected internal new TimeEditSettings Settings {
			get { return (TimeEditSettings)base.Settings; }
		}
		protected override void AssignInitialProperties() {
			base.AssignInitialProperties();
			Control.DisplayFormatString = Properties.DisplayFormatString;
			if(Settings.DateTimeAssigned)
				Control.DateTime = Settings.DateTime;
			Control.ReadOnly = Settings.ReadOnly;
			Control.RightToLeft = Settings.RightToLeft;
			Control.ShowModelErrors = Settings.ShowModelErrors;
		}
		protected override void AssignRenderProperties() {
			base.AssignRenderProperties();
			Control.ButtonTemplate = ContentControlTemplate<TemplateContainerBase>.Create(
				Settings.ButtonTemplateContent, Settings.ButtonTemplateContentMethod, typeof(TemplateContainerBase));
		}
		protected override ASPxWebControl CreateControl() {
			return new MVCxTimeEdit(ViewContext, Metadata);
		}
		protected internal override void PrepareControl() {
			base.PrepareControl();
			PrepareValidationForEditor(Settings.ShowModelErrors);
		}
	}
	public class TokenBoxExtension : EditorExtension {
		public TokenBoxExtension(TokenBoxSettings settings)
			: base(settings) {
		}
		public TokenBoxExtension(TokenBoxSettings settings, ViewContext viewContext)
			: base(settings, viewContext) {
		}
		public TokenBoxExtension(TokenBoxSettings settings, ViewContext viewContext, ModelMetadata metadata)
			: base(settings, viewContext, metadata) {
		}
		public static T[] GetSelectedValues<T>(string name) {
			return EditorValueProvider.GetValue<T[]>(name);
		}
		protected internal new MVCxTokenBox Control {
			get { return (MVCxTokenBox)base.Control; } 
		}
		protected override EditPropertiesBase Properties {
			get { return Control.Properties; }
		}
		protected internal new TokenBoxSettings Settings {
			get { return (TokenBoxSettings)base.Settings; } 
		}
		protected override void AssignInitialProperties() {
			base.AssignInitialProperties();
			Control.CallbackRouteValues = Settings.CallbackRouteValues;
			Control.DisplayFormatString = Properties.DisplayFormatString;
			Control.LoadingPanelImage.Assign(Settings.LoadingPanelImage);
			Control.LoadingPanelStyle.Assign(Settings.LoadingPanelStyle);
			Control.ReadOnly = Settings.ReadOnly;
			Control.RightToLeft = Settings.RightToLeft;
			Control.SettingsLoadingPanel.Assign(Settings.SettingsLoadingPanel);
			Control.ShowModelErrors = Settings.ShowModelErrors;
			Control.Properties.EnableCallbackMode = Settings.CallbackRouteValues != null;
		}
		public override EditorExtension Bind(object value) {
			value = ConvertCollectionToString(value);
			return base.Bind(value);
		}
		protected internal object ConvertCollectionToString(object value) {
			var enumerableValue = value as IEnumerable;
			return !(value is string) && enumerableValue != null
				? string.Join(Settings.Properties.ValueSeparator.ToString(), enumerableValue.Cast<object>())
				: value;
		}
		public TokenBoxExtension BindList(object dataObject) {
			BindInternal(dataObject);
			return this;
		}
		public TokenBoxExtension BindToXML(string fileName) {
			return BindToXML(fileName, string.Empty, string.Empty);
		}
		public TokenBoxExtension BindToXML(string fileName, string xPath) {
			return BindToXML(fileName, xPath, string.Empty);
		}
		public TokenBoxExtension BindToXML(string fileName, string xPath, string transformFileName) {
			BindToXMLInternal(fileName, xPath, transformFileName);
			return this;
		}
		protected override ASPxWebControl CreateControl() {
			return new MVCxTokenBox(ViewContext, Metadata);
		}
		protected internal override void PrepareControl() {
			base.PrepareControl();
			Control.ResetControlHierarchy();
			PrepareValidationForEditor(Settings.ShowModelErrors);
		}
	}
	public class TrackBarExtension : EditorExtension {
		public TrackBarExtension(TrackBarSettings settings)
			: base(settings) {
		}
		public TrackBarExtension(TrackBarSettings settings, ViewContext viewContext)
			: base(settings, viewContext) {
		}
		public TrackBarExtension(TrackBarSettings settings, ViewContext viewContext, ModelMetadata metadata)
			: base(settings, viewContext, metadata) {
		}
		protected internal new MVCxTrackBar Control {
			get { return (MVCxTrackBar)base.Control; }
		}
		protected override EditPropertiesBase Properties {
			get { return Control.Properties; }
		}
		protected internal new TrackBarSettings Settings {
			get { return (TrackBarSettings)base.Settings; }
		}
		public TrackBarExtension BindList(object dataObject) {
			BindInternal(dataObject);
			return this;
		}
		protected override void AssignInitialProperties() {
			base.AssignInitialProperties();
			Control.Position = Settings.Position;
			Control.PositionStart = Settings.PositionStart;
			Control.PositionEnd = Settings.PositionEnd;
			Control.ShowModelErrors = Settings.ShowModelErrors;
			Control.ReadOnly = Settings.ReadOnly;
			Control.RightToLeft = Settings.RightToLeft;
		}
		protected override ASPxWebControl CreateControl() {
			return new MVCxTrackBar(ViewContext, Metadata);
		}
		protected internal override void PrepareControl() {
			base.PrepareControl();
			PrepareValidationForEditor(Settings.ShowModelErrors);
		}
	}
}
