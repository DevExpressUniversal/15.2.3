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
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Web.Mvc;
using System.Web.UI;
using DevExpress.Web.Internal;
using DevExpress.Web;
using DevExpress.Web.Mvc.Internal;
using DevExpress.Web.Mvc.UI;
using System.ComponentModel.DataAnnotations;
namespace DevExpress.Web.Mvc {
	public enum FormLayoutNestedExtensionItemType { Default, BinaryImage, Button, ButtonEdit, Calendar, Captcha, CheckBox, CheckBoxList, 
		ColorEdit, ComboBox, DateEdit, DropDownEdit, HyperLink, Image, Label, ListBox, Memo, ProgressBar, RadioButton, RadioButtonList, 
		SpinEdit, TextBox, TimeEdit, TokenBox, TrackBar, UploadControl, ValidationSummary };
	public class MVCxFormLayoutItemCollection<ModelType>: MVCxFormLayoutItemCollection, IFormLayoutHtmlHelperOwner {
		HtmlHelper htmlHelper;
		public MVCxFormLayoutItemCollection()
			: this(null) {
		}
		public MVCxFormLayoutItemCollection(IWebControlObject owner)
			: this(owner, null) {
		}
		internal MVCxFormLayoutItemCollection(IWebControlObject owner, HtmlHelper htmlHelper)
			: base(owner) {
			this.htmlHelper = htmlHelper ?? HtmlHelperExtension.HtmlHelper;
		}
		public MVCxFormLayoutItem Add<ValueType>(Expression<Func<ModelType, ValueType>> expression) {
			return Add(expression, null);
		}
		public MVCxFormLayoutItem Add<ValueType>(Expression<Func<ModelType, ValueType>> expression, Action<MVCxFormLayoutItem> method) {
			var item = expression != null ? Add<MVCxFormLayoutItem, ValueType>(expression) : Add((i) => { });
			if(method != null)
				method(item);
			return item;
		}
		public new MVCxFormLayoutGroup<ModelType> AddGroupItem() {
			return (MVCxFormLayoutGroup<ModelType>)Add(new MVCxFormLayoutGroup<ModelType>(this.htmlHelper));
		}
		public MVCxFormLayoutGroup<ModelType> AddGroupItem(Action<MVCxFormLayoutGroup<ModelType>> method) {
			var item = new MVCxFormLayoutGroup<ModelType>(this.htmlHelper);
			if(method != null)
				method(item);
			return (MVCxFormLayoutGroup<ModelType>)Add(item);
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		new MVCxFormLayoutGroup AddGroupItem(Action<MVCxFormLayoutGroup> method) {
			return base.AddGroupItem(method);
		}
		public new MVCxFormLayoutGroup<ModelType> AddGroupItem(string caption) {
			return Add<MVCxFormLayoutGroup<ModelType>>(caption);
		}
		public MVCxFormLayoutGroup<ModelType> AddGroupItem<ValueType>(Expression<Func<ModelType, ValueType>> expression) {
			return AddGroupItem(expression, null);
		}
		public MVCxFormLayoutGroup<ModelType> AddGroupItem<ValueType>(Expression<Func<ModelType, ValueType>> expression, Action<MVCxFormLayoutGroup<ModelType>> method) {
			var item = expression != null ? Add<MVCxFormLayoutGroup<ModelType>, ValueType>(expression) : AddGroupItem();
			if(method != null)
				method(item);
			return item;
		}
		public new MVCxTabbedFormLayoutGroup<ModelType> AddTabbedGroupItem() {
			return (MVCxTabbedFormLayoutGroup<ModelType>)Add(new MVCxTabbedFormLayoutGroup<ModelType>(this.htmlHelper));
		}
		public MVCxTabbedFormLayoutGroup<ModelType> AddTabbedGroupItem(Action<MVCxTabbedFormLayoutGroup<ModelType>> method) {
			var item = new MVCxTabbedFormLayoutGroup<ModelType>(this.htmlHelper);
			if(method != null)
				method(item);
			return (MVCxTabbedFormLayoutGroup<ModelType>)Add(item);
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		new MVCxTabbedFormLayoutGroup AddTabbedGroupItem(Action<MVCxTabbedFormLayoutGroup> method) {
			return base.AddTabbedGroupItem(method);
		}
		public new MVCxTabbedFormLayoutGroup<ModelType> AddTabbedGroupItem<ValueType>(string caption) {
			return Add<MVCxTabbedFormLayoutGroup<ModelType>>(caption);
		}
		public MVCxTabbedFormLayoutGroup<ModelType> AddTabbedGroupItem<ValueType>(Expression<Func<ModelType, ValueType>> expression) {
			return AddTabbedGroupItem(expression, null);
		}
		public MVCxTabbedFormLayoutGroup<ModelType> AddTabbedGroupItem<ValueType>(Expression<Func<ModelType, ValueType>> expression, Action<MVCxTabbedFormLayoutGroup<ModelType>> method) {
			var item = Add<MVCxTabbedFormLayoutGroup<ModelType>, ValueType>(expression);
			if(method != null)
				method(item);
			return item;
		}
		T Add<T, ValueType>(Expression<Func<ModelType, ValueType>> expression) where T: LayoutItemBase, new() {
			var item = CreateLayoutItem<T>();
			FormLayoutItemHelper.ConfigureByExpression(item, expression);
			Add(item);
			return item;
		}
		T CreateLayoutItem<T>() where T: LayoutItemBase, new() {
			if(typeof(IFormLayoutHtmlHelperOwner).IsAssignableFrom(typeof(T)))
				return (T)Activator.CreateInstance(typeof(T), BindingFlags.NonPublic | BindingFlags.Instance, null, new object[]{ this.htmlHelper }, null);
			return new T();
		}
		#region IFormLayoutHtmlHelperOwner Members
		HtmlHelper IFormLayoutHtmlHelperOwner.HtmlHelper { get { return htmlHelper; } }
		ModelMetadata IFormLayoutHtmlHelperOwner.Metadata { get { return null; } set { } }
		#endregion
	}
	public class MVCxFormLayoutItemCollection: LayoutItemCollection {
		public MVCxFormLayoutItemCollection()
			: base() {
		}
		public MVCxFormLayoutItemCollection(IWebControlObject owner)
			: base(owner) {
		}
		public MVCxFormLayoutGroup AddGroupItem() {
			return (MVCxFormLayoutGroup)Add(new MVCxFormLayoutGroup());
		}
		public MVCxFormLayoutGroup AddGroupItem(Action<MVCxFormLayoutGroup> method) {
			var item = new MVCxFormLayoutGroup();
			if(method != null)
				method(item);
			return (MVCxFormLayoutGroup)Add(item);
		}
		public MVCxFormLayoutGroup AddGroupItem(string caption) {
			return Add<MVCxFormLayoutGroup>(caption);
		}
		public MVCxTabbedFormLayoutGroup AddTabbedGroupItem() {
			return (MVCxTabbedFormLayoutGroup)Add(new MVCxTabbedFormLayoutGroup());
		}
		public MVCxTabbedFormLayoutGroup AddTabbedGroupItem(Action<MVCxTabbedFormLayoutGroup> method) {
			var item = new MVCxTabbedFormLayoutGroup();
			if(method != null)
				method(item);
			return (MVCxTabbedFormLayoutGroup)Add(item);
		}
		public MVCxTabbedFormLayoutGroup AddTabbedGroupItem<ValueType>(string caption) {
			return Add<MVCxTabbedFormLayoutGroup>(caption);
		}
		public MVCxFormLayoutItem Add() {
			return Add((i) => { });
		}
		public MVCxFormLayoutItem Add(Action<MVCxFormLayoutItem> method) {
			var htmlHelpertOwner = Owner as IFormLayoutHtmlHelperOwner;
			var htmlHelper = htmlHelpertOwner != null ? htmlHelpertOwner.HtmlHelper : null;
			var item = new MVCxFormLayoutItem(htmlHelper);
			if(method != null)
				method(item);
			return (MVCxFormLayoutItem)Add(item);
		}
		public EmptyLayoutItem AddEmptyItem() {
			return (EmptyLayoutItem)Add(new EmptyLayoutItem());
		}
		[EditorBrowsable(EditorBrowsableState.Never)]
		public new LayoutItemBase Add(LayoutItemBase item) { return base.Add(item); }
		[EditorBrowsable(EditorBrowsableState.Never)]
		public new T Add<T>(string caption) where T : LayoutItemBase, new() { return base.Add<T>(caption); }
		[EditorBrowsable(EditorBrowsableState.Never)]
		public new T Add<T>(string caption, string name) where T : LayoutItemBase, new() { return base.Add<T>(caption, name); }
		protected override void OnBeforeAdd(CollectionItem item) {
			base.OnBeforeAdd(item);
			var layoutItem = item as MVCxFormLayoutItem;
			if(layoutItem != null)
				layoutItem.RefreshNestedExtensionName();
		}
	}
	public class MVCxFormLayoutItem: LayoutItem, IFormLayoutHtmlHelperOwner {
		FormLayoutNestedExtensionInfo nestedExtensionInfo;
		FormLayoutNestedExtensionFactory nestedExtensionFactory;
		ExtensionBase nestedExtensionInst;
		HtmlHelper htmlHelper;
		public MVCxFormLayoutItem()
			: this((HtmlHelper)null) {
		}
		public MVCxFormLayoutItem(string caption)
			: this(caption, null) {
		}
		internal MVCxFormLayoutItem(HtmlHelper htmlHelper)
			: base() {
			this.htmlHelper = htmlHelper ?? HtmlHelperExtension.HtmlHelper;
		}
		internal MVCxFormLayoutItem(string caption, HtmlHelper htmlHelper)
			: base(caption) {
			this.htmlHelper = htmlHelper ?? HtmlHelperExtension.HtmlHelper;
		}
		public FormLayoutNestedExtensionItemType NestedExtensionType {
			get { return NestedExtensionInfo.ItemExtensionType; }
			set {
				if(NestedExtensionInfo.ItemExtensionType == value)
					return;
				ChangeNestedExtension(value);
			}
		}
		public FormLayoutNestedExtensionFactory NestedExtension() {
			return NestedExtensionFactory;
		}
		protected FormLayoutNestedExtensionInfo ChangeNestedExtension(FormLayoutNestedExtensionItemType extensionType) {
			NestedExtensionInfo.ItemExtensionType = extensionType;
			RefreshNestedExtensionName();
			var editorSettings = NestedExtensionInfo.Settings as EditorSettings;
			if(editorSettings != null)
				ExtensionsHelper.ConfigureEditPropertiesByMetadata(editorSettings.Properties, NestedExtensionInfo.Metadata);
			return NestedExtensionInfo;
		}
		public SettingsBase NestedExtensionSettings { get { return NestedExtensionInfo.Settings; } }
		public new MVCxLayoutItemCaptionSettings CaptionSettings {
			get { return (MVCxLayoutItemCaptionSettings)base.CaptionSettings; }
		}
		protected internal new MVCxFormLayout FormLayout { get { return base.FormLayout as MVCxFormLayout; } }
		protected internal FormLayoutNestedExtensionInfo NestedExtensionInfo {
			get {
				if(nestedExtensionInfo == null)
					nestedExtensionInfo = new FormLayoutNestedExtensionInfo();
				return nestedExtensionInfo;
			}
		}
		protected FormLayoutNestedExtensionFactory NestedExtensionFactory {
			get {
				if(nestedExtensionFactory == null)
					nestedExtensionFactory = new FormLayoutNestedExtensionFactory(ChangeNestedExtension);
				return nestedExtensionFactory;
			}
		}
		protected internal new Type DataType {
			get { return base.DataType; }
			set { base.DataType = value; }
		}
		protected override void SetDataTypeCore(Type dataType) {
			base.SetDataTypeCore(dataType);
			if(NestedExtensionInfo.ItemExtensionType == FormLayoutNestedExtensionItemType.Default)
				NestedExtensionInfo.ChangeExtensionTypeByDataType(dataType);
		}
		protected internal string NestedContent { get; set; }
		protected internal Action NestedContentMethod { get; set; }
		protected internal ExtensionBase NestedExtensionInst {
			get {
				if(HasNestedContentTemplate)
					return null;
				if(nestedExtensionInst == null) {
					EnsureNestedControl();
					nestedExtensionInst = CreateAndPrepareNestedExtension();
				}
				return nestedExtensionInst;
			}
		}
		protected internal Control NestedControl { get { return NestedExtensionInst != null ? NestedExtensionInst.Control : null; } }
		protected internal bool HasNestedContentTemplate { get { return NestedContent != null || NestedContentMethod != null; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public new Control GetNestedControl() {
			return NestedControl;
		}
		protected internal override Control GetNestedControl(NestedControlSearchMode searchMode) {
			return NestedControl;
		}
		public void SetNestedContent(string content) {
			NestedContent = content;
		}
		public void SetNestedContent(Action method) {
			NestedContentMethod = method;
		}
		protected internal override void EnsureNestedControl() {
			RefreshNestedExtensionName();
		}
		internal void RefreshNestedExtensionName() {
			if(string.IsNullOrEmpty(NestedExtensionInfo.Settings.Name) || NestedExtensionInfo.Settings.Name == NestedExtensionInfo.DefaultExtensionName) {
				NestedExtensionInfo.DefaultExtensionName = !string.IsNullOrEmpty(Name) ? Name : FieldName;
				NestedExtensionInfo.Settings.Name = NestedExtensionInfo.DefaultExtensionName;
			}
		}
		ExtensionBase CreateAndPrepareNestedExtension() {
			if(string.IsNullOrEmpty(NestedExtensionInfo.Settings.Name) && NestedExtensionInfo.ItemExtensionType == FormLayoutNestedExtensionItemType.Default)
				return null;
			bool hasMetadataParam = NestedExtensionInfo.Type.GetConstructors().Where(c => c.GetParameters().Count() >= 3).Count() > 0;
			ViewContext viewContext = this.htmlHelper != null ? this.htmlHelper.ViewContext : null;
			List<object> constructorParams = new List<object> { NestedExtensionInfo.Settings, viewContext };
			if(hasMetadataParam)
				constructorParams.Add(NestedExtensionInfo.Metadata);
			ExtensionBase nestedExtension = (ExtensionBase)Activator.CreateInstance(NestedExtensionInfo.Type, constructorParams.ToArray());
			EditorSettings editorSettings = NestedExtensionInfo.Settings as EditorSettings;
			if(editorSettings != null) {
				((IAssignEditorProperties)editorSettings.Properties).AssignEditorProperties((ASPxEditBase)nestedExtension.Control);
				nestedExtension.Control.DataBind();  
			}
			nestedExtension.PrepareControlProperties();
			nestedExtension.PrepareControl();
			return nestedExtension;
		}
		public override void Assign(CollectionItem source) {
			base.Assign(source);
			MVCxFormLayoutItem item = source as MVCxFormLayoutItem;
			if(item != null) {
				NestedExtensionInfo.Assign(item.NestedExtensionInfo);
				htmlHelper = item.htmlHelper ?? HtmlHelperExtension.HtmlHelper;
				NestedContent = item.NestedContent;
				NestedContentMethod = item.NestedContentMethod;
			}
		}
		protected override LayoutItemCaptionSettings CreateCaptionSettings() {
			return new MVCxLayoutItemCaptionSettings(this);
		}
		#region IFormLayoutHtmlHelperOwner Members
		HtmlHelper IFormLayoutHtmlHelperOwner.HtmlHelper { get { return this.htmlHelper; } }
		ModelMetadata IFormLayoutHtmlHelperOwner.Metadata { get { return NestedExtensionInfo.Metadata; } set { NestedExtensionInfo.Metadata = value; } }
		#endregion
	}
	public class MVCxLayoutItemCaptionSettings : LayoutItemCaptionSettings {
		public MVCxLayoutItemCaptionSettings()
			: this(null) {
		}
		public MVCxLayoutItemCaptionSettings(MVCxFormLayoutItem item)
			: base(item) {
		}
		public string AssociatedNestedExtensionName { get; set; }
		public override void Assign(PropertiesBase source) {
			base.Assign(source);
			MVCxLayoutItemCaptionSettings captionSettings = source as MVCxLayoutItemCaptionSettings;
			if(captionSettings != null)
				AssociatedNestedExtensionName = captionSettings.AssociatedNestedExtensionName;
		}
	}
	public class MVCxFormLayoutGroup<ModelType>: MVCxFormLayoutGroup, IFormLayoutHtmlHelperOwner {
		HtmlHelper htmlHelper;
		public MVCxFormLayoutGroup()
			: this(null, null) {
		}
		public MVCxFormLayoutGroup(string caption)
			: this(caption, null) {
		}
		internal MVCxFormLayoutGroup(HtmlHelper htmlHelper): this(null, htmlHelper) {
		}
		internal MVCxFormLayoutGroup(string caption, HtmlHelper htmlHelper)
			:base(){
			this.htmlHelper = htmlHelper ?? HtmlHelperExtension.HtmlHelper;
		}
		public new MVCxFormLayoutItemCollection<ModelType> Items {
			get { return (MVCxFormLayoutItemCollection<ModelType>)base.Items; }
		}
		protected override LayoutItemCollection CreateItems() {
			return new MVCxFormLayoutItemCollection<ModelType>(this, htmlHelper);
		}
		#region IFormLayoutHtmlHelperOwner Members
		HtmlHelper IFormLayoutHtmlHelperOwner.HtmlHelper { get { return htmlHelper; } }
		ModelMetadata IFormLayoutHtmlHelperOwner.Metadata { get { return null; } set { } }
		#endregion
	}
	public class MVCxFormLayoutGroup: LayoutGroup {
		public MVCxFormLayoutGroup()
			: base() {
		}
		public MVCxFormLayoutGroup(string caption)
			: base() {
		}
		public new MVCxFormLayoutItemCollection Items {
			get { return (MVCxFormLayoutItemCollection)base.Items; }
		}
		protected override LayoutItemCollection CreateItems() {
			return new MVCxFormLayoutItemCollection(this);
		}
	}
	public class MVCxTabbedFormLayoutGroup<ModelType>: MVCxTabbedFormLayoutGroup, IFormLayoutHtmlHelperOwner {
		HtmlHelper htmlHelper;
		public MVCxTabbedFormLayoutGroup()
			: this(null, null) {
		}
		public MVCxTabbedFormLayoutGroup(string caption)
			: this(caption, null) {
		}
		internal MVCxTabbedFormLayoutGroup(HtmlHelper htmlHelper)
			: this(null, htmlHelper) {
		}
		internal MVCxTabbedFormLayoutGroup(string caption, HtmlHelper htmlHelper)
			: base(caption) {
			this.htmlHelper = htmlHelper ?? HtmlHelperExtension.HtmlHelper;
		}
		public new MVCxFormLayoutItemCollection<ModelType> Items {
			get { return (MVCxFormLayoutItemCollection<ModelType>)base.Items; }
		}
		protected override LayoutItemCollection CreateItems() {
			return new MVCxFormLayoutItemCollection<ModelType>(this);
		}
		#region IFormLayoutHtmlHelperOwner Members
		HtmlHelper IFormLayoutHtmlHelperOwner.HtmlHelper { get { return htmlHelper; } }
		ModelMetadata IFormLayoutHtmlHelperOwner.Metadata { get { return null; } set { } }
		#endregion
	}
	public class MVCxTabbedFormLayoutGroup: TabbedLayoutGroup {
		public MVCxTabbedFormLayoutGroup()
			: base() {
		}
		public MVCxTabbedFormLayoutGroup(string caption)
			: base(caption) {
		}
		public new MVCxFormLayoutItemCollection Items {
			get { return (MVCxFormLayoutItemCollection)base.Items; }
		}
		protected override LayoutItemCollection CreateItems() {
			return new MVCxFormLayoutItemCollection(this);
		}
		public override void Assign(CollectionItem source) {
			base.Assign(source);
			var item = source as MVCxTabbedFormLayoutGroup;
			if(item != null && string.IsNullOrEmpty(item.ClientInstanceName))
				PageControl.ClientInstanceName = item.Name;
		}
	}
	public class FormLayoutNestedExtensionFactory {
		internal FormLayoutNestedExtensionFactory(Func<FormLayoutNestedExtensionItemType, FormLayoutNestedExtensionInfo> method) {
			ChangeNestedExtensionItemMethod = method;
		}
		protected Func<FormLayoutNestedExtensionItemType, FormLayoutNestedExtensionInfo> ChangeNestedExtensionItemMethod { get; private set; }
		public void BinaryImage(Action<BinaryImageEditSettings> settingsMethod) {
			ChangeNestedExtension(FormLayoutNestedExtensionItemType.BinaryImage, settingsMethod);
		}
		public void Button(Action<ButtonSettings> settingsMethod) {
			ChangeNestedExtension(FormLayoutNestedExtensionItemType.Button, settingsMethod);
		}
		public void ButtonEdit(Action<ButtonEditSettings> settingsMethod) {
			ChangeNestedExtension(FormLayoutNestedExtensionItemType.ButtonEdit, settingsMethod);
		}
		public void Calendar(Action<CalendarSettings> settingsMethod) {
			ChangeNestedExtension(FormLayoutNestedExtensionItemType.Calendar, settingsMethod);
		}
		public void Captcha(Action<CaptchaSettings> settingsMethod) {
			ChangeNestedExtension(FormLayoutNestedExtensionItemType.Captcha, settingsMethod);
		}
		public void CheckBox(Action<CheckBoxSettings> settingsMethod) {
			ChangeNestedExtension(FormLayoutNestedExtensionItemType.CheckBox, settingsMethod);
		}
		public void CheckBoxList(Action<CheckBoxListSettings> settingsMethod) {
			ChangeNestedExtension(FormLayoutNestedExtensionItemType.CheckBoxList, settingsMethod);
		}
		public void ColorEdit(Action<ColorEditSettings> settingsMethod) {
			ChangeNestedExtension(FormLayoutNestedExtensionItemType.ColorEdit, settingsMethod);
		}
		public void ComboBox(Action<ComboBoxSettings> settingsMethod) {
			ChangeNestedExtension(FormLayoutNestedExtensionItemType.ComboBox, settingsMethod);
		}
		public void DateEdit(Action<DateEditSettings> settingsMethod) {
			ChangeNestedExtension(FormLayoutNestedExtensionItemType.DateEdit, settingsMethod);
		}
		public void DropDownEdit(Action<DropDownEditSettings> settingsMethod) {
			ChangeNestedExtension(FormLayoutNestedExtensionItemType.DropDownEdit, settingsMethod);
		}
		public void HyperLink(Action<HyperLinkSettings> settingsMethod) {
			ChangeNestedExtension(FormLayoutNestedExtensionItemType.HyperLink, settingsMethod);
		}
		public void Image(Action<ImageEditSettings> settingsMethod) {
			ChangeNestedExtension(FormLayoutNestedExtensionItemType.Image, settingsMethod);
		}
		public void Label(Action<LabelSettings> settingsMethod) {
			ChangeNestedExtension(FormLayoutNestedExtensionItemType.Label, settingsMethod);
		}
		public void ListBox(Action<ListBoxSettings> settingsMethod) {
			ChangeNestedExtension(FormLayoutNestedExtensionItemType.ListBox, settingsMethod);
		}
		public void Memo(Action<MemoSettings> settingsMethod) {
			ChangeNestedExtension(FormLayoutNestedExtensionItemType.Memo, settingsMethod);
		}
		public void ProgressBar(Action<ProgressBarSettings> settingsMethod) {
			ChangeNestedExtension(FormLayoutNestedExtensionItemType.ProgressBar, settingsMethod);
		}
		public void RadioButton(Action<RadioButtonSettings> settingsMethod) {
			ChangeNestedExtension(FormLayoutNestedExtensionItemType.RadioButton, settingsMethod);
		}
		public void RadioButtonList(Action<RadioButtonListSettings> settingsMethod) {
			ChangeNestedExtension(FormLayoutNestedExtensionItemType.RadioButtonList, settingsMethod);
		}
		public void SpinEdit(Action<SpinEditSettings> settingsMethod) {
			ChangeNestedExtension(FormLayoutNestedExtensionItemType.SpinEdit, settingsMethod);
		}
		public void TextBox(Action<TextBoxSettings> settingsMethod) {
			ChangeNestedExtension(FormLayoutNestedExtensionItemType.TextBox, settingsMethod);
		}
		public void TimeEdit(Action<TimeEditSettings> settingsMethod) {
			ChangeNestedExtension(FormLayoutNestedExtensionItemType.TimeEdit, settingsMethod);
		}
		public void TokenBox(Action<TokenBoxSettings> settingsMethod) {
			ChangeNestedExtension(FormLayoutNestedExtensionItemType.TokenBox, settingsMethod);
		}
		public void TrackBar(Action<TrackBarSettings> settingsMethod) {
			ChangeNestedExtension(FormLayoutNestedExtensionItemType.TrackBar, settingsMethod);
		}
		public void UploadControl(Action<UploadControlSettings> settingsMethod) {
			ChangeNestedExtension(FormLayoutNestedExtensionItemType.UploadControl, settingsMethod);
		}
		public void ValidationSummary(Action<ValidationSummarySettings> settingsMethod) {
			ChangeNestedExtension(FormLayoutNestedExtensionItemType.ValidationSummary, settingsMethod);
		}
		protected void ChangeNestedExtension<T>(FormLayoutNestedExtensionItemType extensionType, Action<T> settignsMethod) where T: SettingsBase {
			var info = ChangeNestedExtensionItemMethod(extensionType);
			if(settignsMethod != null)
				settignsMethod((T)info.Settings);
		}
	}
}
namespace DevExpress.Web.Mvc.Internal {
	public interface IFormLayoutHtmlHelperOwner {
		HtmlHelper HtmlHelper { get; }
		ModelMetadata Metadata { get; set; }
	}
	public class FormLayoutItemHelper {
		public static void ConfigureLayoutItemsByMetadata(FormLayoutProperties formLayoutProperties) {
			formLayoutProperties.ForEach(item => {
				string stringExpression = GetStringExpressionForMetadata(item);
				if(!string.IsNullOrEmpty(stringExpression)) {
					ModelMetadata metadata = ExtensionsHelper.GetMetadataForColumn(stringExpression);
					FormLayoutItemHelper.ConfigureLayoutItemByMetadata(item, metadata);
				}
			});
		}
		static string GetStringExpressionForMetadata(LayoutItemBase item) {
			GridViewColumnLayoutItem columnLayoutItem = item as GridViewColumnLayoutItem;
			if(columnLayoutItem != null && columnLayoutItem.Column != null) {
				GridViewDataColumn dataColumn = columnLayoutItem.Column as GridViewDataColumn;
				if(dataColumn != null)
					return dataColumn.FieldName;
			}
			return item.Name;
		}
		public static void ConfigureByExpression<ModelType, ValueType>(LayoutItemBase item, Expression<Func<ModelType, ValueType>> expression) {
			ModelMetadata metadata = GetModelMetadataByExpression(item, expression);
			ConfigureLayoutItemByMetadata(item, metadata);
			var layoutItem = item as MVCxFormLayoutItem;
			if(layoutItem != null) {
				layoutItem.Name = layoutItem.FieldName = GetPropertyNameByExpression(item, expression);
				layoutItem.DataType = typeof(ValueType); 
			}
		}
		public static void ConfigureByMetadata(LayoutItemBase item) {
			var htmlHelper = GetHtmlHelpertByItem(item);
			if(htmlHelper == null)
				return;
			ModelMetadata metadata = GetMetadataByEditorName(item, htmlHelper.ViewData);
			ConfigureLayoutItemByMetadata(item, metadata);
		}
		static ModelMetadata GetMetadataByEditorName(LayoutItemBase item, ViewDataDictionary viewData) {
			string propertyName = GetPropertyNameByItem(item);
			ModelMetadata metadata = ExtensionsHelper.GetMetadataByEditorName(propertyName, viewData);
			if(metadata != null && metadata.ContainerType != null)
				return metadata;
			var layoutItem = item as MVCxFormLayoutItem;
			if(layoutItem == null || layoutItem.FormLayout == null)
				return metadata;
			if(!IsPossibleReadMetadataForProperty(layoutItem.FormLayout.DataSource, propertyName))
				return metadata;
			try {
				return ExtensionsHelper.GetMetadataByEditorName(propertyName, new ViewDataDictionary(layoutItem.FormLayout.DataSource));
			} catch { return null; }
		}
		static bool IsPossibleReadMetadataForProperty(object data, string propertyName) {
			if(data == null)
				return false;
			var type = data.GetType();
			var descriptionProvider = new AssociatedMetadataTypeTypeDescriptionProvider(type).GetTypeDescriptor(type);
			return ReflectionUtils.IsPropertyExist(descriptionProvider, propertyName);
		}
		public static void ConfigureLayoutItemByMetadata(LayoutItemBase item, ModelMetadata metadata) {
			if(metadata == null || metadata.ContainerType == null)
				return;
			var htmlHelperOwner = item as IFormLayoutHtmlHelperOwner;
			if(htmlHelperOwner != null) {
				if(htmlHelperOwner.Metadata != null)
					return;
				htmlHelperOwner.Metadata = metadata;
			}
			if(RequireSetCaptionFromMetadata(item)) 
				item.Caption = metadata.DisplayName ?? metadata.PropertyName;
			ApplyRequiredAttribute(item, metadata);
			var layoutItem = item as MVCxFormLayoutItem;
			if(layoutItem != null) {
				if(layoutItem.DataType == null && layoutItem.NestedExtensionType == FormLayoutNestedExtensionItemType.Default)
					layoutItem.DataType = metadata.ModelType;
				var editorSettings = layoutItem.NestedExtensionInfo.Settings as EditorSettings;
				if (editorSettings != null)
					ExtensionsHelper.ConfigureEditPropertiesByMetadata(editorSettings.Properties, metadata);
			}
		}
		static bool RequireSetCaptionFromMetadata(LayoutItemBase item) {
			if(item.IsCaptionAssigned)
				return false;
			var cardItem = item as CardViewColumnLayoutItem;
			if(cardItem != null)
				return cardItem.Column != null ? string.IsNullOrEmpty(cardItem.Column.Caption) : true;
			var gridItem = item as GridViewColumnLayoutItem;
			if(gridItem != null)
				return gridItem.Column != null ? string.IsNullOrEmpty(gridItem.Column.Caption) : true;
			return true;
		}
		static void ApplyRequiredAttribute(LayoutItemBase item, ModelMetadata metadata) {
			LayoutItem layoutItem = item as LayoutItem;
			if(layoutItem != null && metadata.IsRequired && layoutItem.RequiredMarkDisplayMode == FieldRequiredMarkMode.Auto)
				layoutItem.RequiredMarkDisplayMode = FieldRequiredMarkMode.Required;
		}
		static string GetPropertyNameByItem(LayoutItemBase item) {
			string propertyName = item.Name;
			var layoutItem = item as LayoutItem;
			if(layoutItem != null && !string.IsNullOrEmpty(layoutItem.FieldName))
				propertyName = layoutItem.FieldName;
			return propertyName;
		}
		static string GetPropertyNameByExpression<ModelType, ValueType>(LayoutItemBase item, Expression<Func<ModelType, ValueType>> expression) {
			string fieldName = ExpressionHelper.GetExpressionText(expression);
			ViewDataDictionary<ModelType> viewData = GetViewDataByItem<ModelType>(item);
			return viewData.TemplateInfo.GetFullHtmlFieldName(fieldName);
		}
		static ModelMetadata GetModelMetadataByExpression<ModelType, ValueType>(LayoutItemBase item, Expression<Func<ModelType, ValueType>> expression) {
			var viewData = GetViewDataByItem<ModelType>(item);
			return ModelMetadata.FromLambdaExpression<ModelType, ValueType>(expression, viewData);
		}
		static ViewDataDictionary<ModelType> GetViewDataByItem<ModelType>(LayoutItemBase item) {
			ViewDataDictionary<ModelType> viewData = null;
			HtmlHelper htmlHelper = GetHtmlHelpertByItem(item);
			if(htmlHelper != null)
				viewData = htmlHelper.ViewData as ViewDataDictionary<ModelType>;
			if(viewData == null)
				viewData = HtmlHelperExtension.HtmlHelper.ViewData as ViewDataDictionary<ModelType>;
			return viewData;
		}
		static HtmlHelper GetHtmlHelpertByItem(LayoutItemBase item) {
			var htmlHelperOwner = item as IFormLayoutHtmlHelperOwner;
			return htmlHelperOwner != null ? htmlHelperOwner.HtmlHelper : null;
		}
	}
	public class FormLayoutNestedExtensionInfo {
		static IDictionary<FormLayoutNestedExtensionItemType, Tuple<Type, Type>> ExtensionTypeToInfoMap = new Dictionary<FormLayoutNestedExtensionItemType, Tuple<Type, Type>>() {
			{ FormLayoutNestedExtensionItemType.Default, new Tuple<Type, Type>(typeof(TextBoxExtension), typeof(TextBoxSettings)) },
			{ FormLayoutNestedExtensionItemType.BinaryImage, new Tuple<Type, Type>(typeof(BinaryImageEditExtension), typeof(BinaryImageEditSettings)) },
			{ FormLayoutNestedExtensionItemType.Button, new Tuple<Type, Type>(typeof(ButtonExtension), typeof(ButtonSettings)) },
			{ FormLayoutNestedExtensionItemType.ButtonEdit, new Tuple<Type, Type>(typeof(ButtonEditExtension), typeof(ButtonEditSettings)) },
			{ FormLayoutNestedExtensionItemType.Calendar, new Tuple<Type, Type>(typeof(CalendarExtension), typeof(CalendarSettings)) },
			{ FormLayoutNestedExtensionItemType.Captcha, new Tuple<Type, Type>(typeof(CaptchaExtension), typeof(CaptchaSettings)) },
			{ FormLayoutNestedExtensionItemType.CheckBox, new Tuple<Type, Type>(typeof(CheckBoxExtension), typeof(CheckBoxSettings)) },
			{ FormLayoutNestedExtensionItemType.CheckBoxList, new Tuple<Type, Type>(typeof(CheckBoxListExtension), typeof(CheckBoxListSettings)) },
			{ FormLayoutNestedExtensionItemType.ColorEdit, new Tuple<Type, Type>(typeof(ColorEditExtension), typeof(ColorEditSettings)) },
			{ FormLayoutNestedExtensionItemType.ComboBox, new Tuple<Type, Type>(typeof(ComboBoxExtension), typeof(ComboBoxSettings)) },
			{ FormLayoutNestedExtensionItemType.DateEdit, new Tuple<Type, Type>(typeof(DateEditExtension), typeof(DateEditSettings)) },
			{ FormLayoutNestedExtensionItemType.DropDownEdit, new Tuple<Type, Type>(typeof(DropDownEditExtension), typeof(DropDownEditSettings)) },
			{ FormLayoutNestedExtensionItemType.HyperLink, new Tuple<Type, Type>(typeof(HyperLinkExtension), typeof(HyperLinkSettings)) },
			{ FormLayoutNestedExtensionItemType.Image, new Tuple<Type, Type>(typeof(ImageEditExtension), typeof(ImageEditSettings)) },
			{ FormLayoutNestedExtensionItemType.Label, new Tuple<Type, Type>(typeof(LabelExtension), typeof(LabelSettings)) },
			{ FormLayoutNestedExtensionItemType.ListBox, new Tuple<Type, Type>(typeof(ListBoxExtension), typeof(ListBoxSettings)) },
			{ FormLayoutNestedExtensionItemType.Memo, new Tuple<Type, Type>(typeof(MemoExtension), typeof(MemoSettings)) },
			{ FormLayoutNestedExtensionItemType.ProgressBar, new Tuple<Type, Type>(typeof(ProgressBarExtension), typeof(ProgressBarSettings)) },
			{ FormLayoutNestedExtensionItemType.RadioButton, new Tuple<Type, Type>(typeof(RadioButtonExtension), typeof(RadioButtonSettings)) },
			{ FormLayoutNestedExtensionItemType.RadioButtonList, new Tuple<Type, Type>(typeof(RadioButtonListExtension), typeof(RadioButtonListSettings)) },
			{ FormLayoutNestedExtensionItemType.SpinEdit, new Tuple<Type, Type>(typeof(SpinEditExtension), typeof(SpinEditSettings)) },
			{ FormLayoutNestedExtensionItemType.TextBox, new Tuple<Type, Type>(typeof(TextBoxExtension), typeof(TextBoxSettings)) },
			{ FormLayoutNestedExtensionItemType.TimeEdit, new Tuple<Type, Type>(typeof(TimeEditExtension), typeof(TimeEditSettings)) },
			{ FormLayoutNestedExtensionItemType.TokenBox, new Tuple<Type, Type>(typeof(TokenBoxExtension), typeof(TokenBoxSettings)) },
			{ FormLayoutNestedExtensionItemType.TrackBar, new Tuple<Type, Type>(typeof(TrackBarExtension), typeof(TrackBarSettings)) },
			{ FormLayoutNestedExtensionItemType.UploadControl, new Tuple<Type, Type>(typeof(UploadControlExtension), typeof(UploadControlSettings)) },
			{ FormLayoutNestedExtensionItemType.ValidationSummary, new Tuple<Type, Type>(typeof(ValidationSummaryExtension), typeof(ValidationSummarySettings)) }
		};
		static IDictionary<Type, FormLayoutNestedExtensionItemType> DataTypeToExtensionTypeMap = new Dictionary<Type, FormLayoutNestedExtensionItemType>{
			{ typeof(String), FormLayoutNestedExtensionItemType.TextBox },
			{ typeof(Char), FormLayoutNestedExtensionItemType.TextBox },
			{ typeof(Byte), FormLayoutNestedExtensionItemType.SpinEdit },
			{ typeof(SByte), FormLayoutNestedExtensionItemType.SpinEdit },
			{ typeof(Int16), FormLayoutNestedExtensionItemType.SpinEdit },
			{ typeof(UInt16), FormLayoutNestedExtensionItemType.SpinEdit },
			{ typeof(Int32), FormLayoutNestedExtensionItemType.SpinEdit },
			{ typeof(UInt32), FormLayoutNestedExtensionItemType.SpinEdit },
			{ typeof(Int64), FormLayoutNestedExtensionItemType.SpinEdit },
			{ typeof(UInt64), FormLayoutNestedExtensionItemType.SpinEdit },
			{ typeof(Single), FormLayoutNestedExtensionItemType.SpinEdit },
			{ typeof(Double), FormLayoutNestedExtensionItemType.SpinEdit },
			{ typeof(Decimal), FormLayoutNestedExtensionItemType.SpinEdit },
			{ typeof(Boolean), FormLayoutNestedExtensionItemType.CheckBox },
			{ typeof(DateTime), FormLayoutNestedExtensionItemType.DateEdit },
			{ typeof(Enum), FormLayoutNestedExtensionItemType.ComboBox },
			{ typeof(byte[]), FormLayoutNestedExtensionItemType.BinaryImage }
		};
		public FormLayoutNestedExtensionInfo(Type dataType)
			: this() {
			ItemExtensionType = FindItemExtensionType(dataType);
		}
		public FormLayoutNestedExtensionInfo() {
			ItemExtensionType = FormLayoutNestedExtensionItemType.Default;
		}
		FormLayoutNestedExtensionItemType itemExtensionType;
		public FormLayoutNestedExtensionItemType ItemExtensionType {
			get { return itemExtensionType; }
			set {
				if((itemExtensionType == value && Type != null) || !ExtensionTypeToInfoMap.ContainsKey(value))
					return;
				itemExtensionType = value;
				Type = ExtensionTypeToInfoMap[itemExtensionType].Item1;
				Settings = (SettingsBase)Activator.CreateInstance(ExtensionTypeToInfoMap[itemExtensionType].Item2);
			}
		}
		public Type Type { get; private set; }
		public SettingsBase Settings { get; private set; }
		public string DefaultExtensionName { get; set; }
		public ModelMetadata Metadata { get; set; }
		public object Model { get { return Metadata != null ? Metadata.Model : null; } }
		public virtual void Assign(FormLayoutNestedExtensionInfo source) {
			if(source == null)
				return;
			ItemExtensionType = source.ItemExtensionType;
			Settings = source.Settings;
			DefaultExtensionName = source.DefaultExtensionName;
			Metadata = source.Metadata;
		}
		public void ChangeExtensionTypeByDataType(Type dataType){
			ItemExtensionType = FindItemExtensionType(dataType);
		}
		static FormLayoutNestedExtensionItemType FindItemExtensionType(Type dataType) {
			if(dataType == null)
				return FormLayoutNestedExtensionItemType.Default;
			foreach(Type possibleDataType in DataTypeToExtensionTypeMap.Keys)
				if(possibleDataType.IsAssignableFrom(dataType) || possibleDataType.IsAssignableFrom(Nullable.GetUnderlyingType(dataType)))
					return DataTypeToExtensionTypeMap[possibleDataType];
			return FormLayoutNestedExtensionItemType.Default;
		}
	}
}
