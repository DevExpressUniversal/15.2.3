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
using System.Collections.Specialized;
using System.ComponentModel;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
using DevExpress.Utils;
using DevExpress.Web.ASPxHtmlEditor.Internal;
using DevExpress.Web.Internal;
using System.Linq;
namespace DevExpress.Web.ASPxHtmlEditor.Internal {
	[ToolboxItem(false)]
	public class ToolbarComboBoxControl : ASPxComboBox, System.Web.UI.ITemplate {
		protected internal const string ToolbarComboBoxScriptResourceName = ASPxHtmlEditor.HtmlEditorScriptsResourcePath + "ToolbarComboBox.js";
		public ToolbarComboBoxControl(ASPxWebControl ownerControl, ToolbarComboBoxProperties properties)
			: base(ownerControl, properties) {
			ParentSkinOwner = ownerControl;
			EnableFocusedStyle = false;
			IncrementalFilteringMode = Web.IncrementalFilteringMode.None;
		}
		[AutoFormatDisable, Themeable(false), MergableProperty(false)]
		public new ToolbarComboBoxClientSideEvents ClientSideEvents {
			get { return Properties.ClientSideEvents; }
		}
		protected internal new ToolbarComboBoxProperties Properties {
			get { return (ToolbarComboBoxProperties)base.Properties; }
		}
		protected internal void AssignStyles(EditorStyles parentStyles, params AppearanceStyle[] styles) {
			StylesInternal.CopyFrom(parentStyles);
			EditorStyles editorStyles = StylesInternal as EditorStyles;
			for(int i = 0; i < styles.Length; i++) {
				editorStyles.TextBox.CopyFontFrom(styles[i]);
				editorStyles.ButtonEdit.CopyFontFrom(styles[i]);
				editorStyles.ListBox.CopyFontFrom(styles[i]);
				editorStyles.ListBoxItem.CopyFontFrom(styles[i]);
			}
		}
		public void InstantiateIn(System.Web.UI.Control container) {
			container.Controls.Add(this);
		}
		public override void RegisterEditorIncludeScripts() {
			base.RegisterEditorIncludeScripts();
			RegisterIncludeScript(typeof(ASPxListEdit), ToolbarListBoxControl.ListEditScriptResource);
			RegisterIncludeScript(typeof(ToolbarComboBoxControl), ToolbarComboBoxScriptResourceName);
		}
		protected override DropDownControlBase CreateDropDownControl() {
			CreateItemTemplates();
			return base.CreateDropDownControl();
		}
		protected override ASPxListBox CreateListBoxControlCore() {
			return new ToolbarListBoxControl(this, Properties.ListBoxProperties);
		}
		protected override string GetClientObjectClassName() {
			return IsNativeRender() ? "ASPx.HtmlEditorClasses.Controls.NativeToolbarComboBox" : "ASPx.HtmlEditorClasses.Controls.ToolbarComboBox";
		}
		protected override void GetCreateClientObjectScript(StringBuilder stb, string localVarName, string clientName) {
			base.GetCreateClientObjectScript(stb, localVarName, clientName);
			stb.AppendLine(localVarName + ".isToolbarItem=true;");
			if (!string.IsNullOrEmpty(Properties.DefaultCaption))
				stb.AppendFormat("{0}.defaultCaption={1};\n", localVarName, HtmlConvertor.ToScript(Properties.DefaultCaption));
		}
		protected virtual void CreateItemTemplates() {			
		}
	}
	public class ToolbarFontSizeComboBoxControl : ToolbarComboBoxControl {
		public static Dictionary<int, string> GetSizeRatio() {
			return new Dictionary<int,string>() {
				{1, "8pt"},
				{2, "10pt"},
				{3, "12pt"},
				{4, "14pt"},
				{5, "18pt"},
				{6, "24pt"},
				{7, "36pt"}
			};
		}
		public ToolbarFontSizeComboBoxControl(ASPxWebControl ownerControl, ToolbarComboBoxProperties properties)
			: base(ownerControl, properties) {
		}
		protected override void CreateItemTemplates() {
			foreach (ToolbarListEditItem item in Items) {
				NameValueCollection attrs = new NameValueCollection();
				if(!DesignMode) {
					Dictionary<int, string> sizeRatio = GetSizeRatio();
					var itemValue = Convert.ToInt32(item.Value);
					if(!sizeRatio.ContainsKey(itemValue))
						throw new Exception("Wrong font size value. Only numbers 1-7 are allowed.");
					attrs.Add("style", string.Format("font-size: {0}", sizeRatio[itemValue]));
				}
				item.TextTemplate = new ToolbarCustomListEditItemTemplate(GetText(item), "span", attrs);
			}
		}
		protected string GetText(ToolbarListEditItem item) {
			return string.IsNullOrEmpty(item.Text) ? item.Value.ToString() : item.Text;
		}
	}
	public class ToolbarFontNameComboBoxControl : ToolbarComboBoxControl {
		public ToolbarFontNameComboBoxControl(ASPxWebControl ownerControl, ToolbarComboBoxProperties properties)
			: base(ownerControl, properties) {
		}
		protected override void CreateItemTemplates() {
			foreach (ToolbarListEditItem item in Items) {
				NameValueCollection attrs = new NameValueCollection();
				List<string> items = item.Value.ToString().Split(',').ToList<string>();
				if(items.Count == 0)
					items.Add(item.Value.ToString());
				string result = "";
				foreach(string itemValue in items) {
					if(string.IsNullOrEmpty(itemValue))
						continue;
					if(!string.IsNullOrEmpty(result))
						result += ", ";
					string trimValue = itemValue.Trim(' ', '\'');
					result += trimValue.IndexOf(" ") > -1 ? "'" + trimValue + "'" : trimValue;
				}
				attrs.Add("style", string.Format("font-family: {0}", result));
				item.TextTemplate = new ToolbarCustomListEditItemTemplate(GetText(item), "span", attrs);
			}
		}
		protected string GetText(ToolbarListEditItem item) {
			return string.IsNullOrEmpty(item.Text) ? item.Value.ToString() : item.Text;
		}
	}
	public class ToolbarParagraphFormattingComboBoxControl : ToolbarComboBoxControl {
		public ToolbarParagraphFormattingComboBoxControl(ASPxWebControl ownerControl, ToolbarComboBoxProperties properties)
			: base(ownerControl, properties) {
		}
		protected override string GetClientObjectClassName() {
			return IsNative() ? "ASPxClientNativeToolbarParagraphFormattingComboBox" : "ASPx.HtmlEditorClasses.Controls.ToolbarParagraphFormattingComboBox";
		}
		protected override void CreateItemTemplates() {
			foreach (ToolbarListEditItem item in Items) {
				NameValueCollection attrs = new NameValueCollection();
				attrs.Add("style", "margin: 0px;");
				item.TextTemplate = new ToolbarCustomListEditItemTemplate(item.Text, item.Value.ToString(), attrs);
			}
		}
	}
	[ToolboxItem(false)]
	public class ToolbarCustomCssComboBoxControl : ToolbarComboBoxControl {
		public ToolbarCustomCssComboBoxControl(ASPxWebControl ownerControl, ToolbarCustomCssComboBoxProperties properties)
			: base(ownerControl, properties) {
		}
		public new ToolbarCustomCssListEditItemCollection Items {
			get { return Properties.Items as ToolbarCustomCssListEditItemCollection; }
		}
		protected override void CreateItemTemplates() {
			foreach (ToolbarCustomCssListEditItem item in Items) {
				item.TextTemplate = new ToolbarCustomListEditItemTemplate(item.GetText(), "div", item.PreviewStyle);
			}
		}
		protected override ASPxListBox CreateListBoxControlCore() {
			return new ToolbarListBoxControl(this, Properties.ListBoxProperties);
		}
		protected override string GetClientObjectClassName() {
			return IsNativeRender() ? "ASPx.HtmlEditorClasses.Controls.NativeToolbarCustomCssComboBox" : "ASPx.HtmlEditorClasses.Controls.ToolbarCustomCssComboBox";
		}
		protected override void GetCreateClientObjectScript(StringBuilder stb, string localVarName, string clientName) {
			base.GetCreateClientObjectScript(stb, localVarName, clientName);
			object[] itemsTagNameArray = null;
			object[] itemsCssClassArray = null;
			CreateClientItems(ref itemsTagNameArray, ref itemsCssClassArray);
			if (itemsTagNameArray.Length > 0)
				stb.Append(String.Format("{0}.tagNames={1};\n", localVarName, HtmlConvertor.ToJSON(itemsTagNameArray)));
			if (itemsCssClassArray.Length > 0)
				stb.Append(String.Format("{0}.cssClasses={1};\n", localVarName, HtmlConvertor.ToJSON(itemsCssClassArray)));
		}
		protected void CreateClientItems(ref object[] itemsTagNameArray, ref object[] itemsCssClassArray) {
			itemsTagNameArray = new object[Properties.Items.Count];
			itemsCssClassArray = new object[Properties.Items.Count];
			for (int i = 0; i < Properties.Items.Count; i++) {
				ToolbarCustomCssListEditItem item = Properties.Items[i] as ToolbarCustomCssListEditItem;
				itemsTagNameArray[i] = item.TagName.ToLower();
				itemsCssClassArray[i] = item.CssClass;
			}
		}
		protected override IStateManager[] GetStateManagedObjects() {
			return base.GetStateManagedObjects();
		}
	}
	public class ToolbarCustomComboBoxControl : ToolbarComboBoxControl {
		public ToolbarCustomComboBoxControl(ASPxWebControl ownerControl, ToolbarCustomComboBoxProperties properties)
			: base(ownerControl, properties) {
			Text = properties.DefaultCaption;
		}
		protected string GetText(ToolbarListEditItem item) {
			return string.IsNullOrEmpty(item.Text) ? item.Value.ToString() : item.Text;
		}
		protected override void GetCreateClientObjectScript(StringBuilder stb, string localVarName, string clientName) {
			base.GetCreateClientObjectScript(stb, localVarName, clientName);
			stb.AppendFormat("{0}.commandName='{1}';", localVarName, ((ToolbarCustomComboBoxProperties)Properties).CommandName);
		}
	}
	[ToolboxItem(false)]
	public class ToolbarListBoxControl : ASPxListBox {
		protected internal ToolbarListBoxControl(ASPxWebControl ownerControl, ListBoxProperties properties)
			: base(ownerControl, properties) {
		}
		protected internal static string ListEditScriptResource {
			get { return ASPxListEdit.ListEditScriptResourceName; }
		}
		protected override string GetClientObjectClassName() {
			return IsNativeRender() ? "ASPx.HtmlEditorClasses.Controls.NativeToolbarListBox" : "ASPx.HtmlEditorClasses.Controls.ToolbarListBox";
		}
	}
	public class ToolbarCustomListEditItemTemplate : ITemplate {
		NameValueCollection attributes = null;
		AppearanceStyleBase style = null;
		string tagName = "";
		string text = "";
		public ToolbarCustomListEditItemTemplate(string text, string tagName, NameValueCollection attributes)
			: this(text, tagName, attributes, null) {
		}
		public ToolbarCustomListEditItemTemplate(string text, string tagName, AppearanceStyleBase style)
			: this(text, tagName, null, style) {
		}
		public ToolbarCustomListEditItemTemplate(string text, string tagName, 
			NameValueCollection attributes, AppearanceStyleBase style) {
			this.attributes = attributes;
			this.text = text;
			this.tagName = tagName;
			this.style = style;
		}
		public AppearanceStyleBase Style { get { return style; } }
		public string Text { get { return text; } }
		public string TagName { get { return tagName; } }
		public NameValueCollection Attributes { get { return attributes; } }
		public void InstantiateIn(System.Web.UI.Control container) {
			container.Controls.Add(CreateItemControl());
		}
		protected Control CreateItemControl() {
			Control itemControl = null;
			object tagObject = CommonUtils.GetHtmlTextWriterTagObject(TagName);
			if (tagObject is HtmlTextWriterTag) {
				itemControl = RenderUtils.CreateWebControl((HtmlTextWriterTag)tagObject);
				WebControl itemWebControl = itemControl as WebControl;
				AddAttributes(itemWebControl);
				AssignStyle(itemWebControl);
				itemControl.Controls.Add(new LiteralControl(Text));
			} else
				itemControl = new LiteralControl(Text);
			return itemControl;
		}
		protected void AddAttributes(WebControl control) {
			if (Attributes != null) {
				foreach (string attrName in Attributes.AllKeys)
					control.Attributes.Add(attrName, Attributes[attrName]);
			}
		}
		protected void AssignStyle(WebControl control) {
			if (Style != null)
				Style.AssignToControl(control);
		}
	}
}
namespace DevExpress.Web.ASPxHtmlEditor {
	public class ToolbarComboBoxProperties : ComboBoxProperties {
		public ToolbarComboBoxProperties()
			: base() {
		}
		public ToolbarComboBoxProperties(IPropertiesOwner owner, string defaultCaption)
			: base(owner) {
			SetDefaultCaptionValue(defaultCaption);
		}
		[DefaultValue(""), Localizable(true), AutoFormatDisable, NotifyParentProperty(true), Themeable(false)]
		public virtual string DefaultCaption {
			get { return GetStringProperty("DefaultCaption", ""); }
			set { SetDefaultCaptionValue(value); }
		}
		[Localizable(false), AutoFormatDisable, NotifyParentProperty(true), Themeable(false),
		Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never)]
		public new ToolbarComboBoxClientSideEvents ClientSideEvents {
			get { return base.ClientSideEvents as ToolbarComboBoxClientSideEvents; }
		}
		protected internal new ListBoxProperties ListBoxProperties {
			get { return base.ListBoxProperties; }
		}
		public override void Assign(PropertiesBase source) {
			BeginUpdate();
			try {
				base.Assign(source);
				ToolbarComboBoxProperties src = source as ToolbarComboBoxProperties;
				if (src != null) {
					DefaultCaption = src.DefaultCaption;
				}
			} finally {
				EndUpdate();
			}
		}
		protected override ComboBoxListBoxProperties CreateListBoxProperties() {
			return new ToolbarListBoxProperties(this);
		}
		protected override EditClientSideEventsBase CreateClientSideEvents() {
			return new ToolbarComboBoxClientSideEvents();
		}
		private void SetDefaultCaptionValue(string value) {
			SetStringProperty("DefaultCaption", "", value);
		}
	}
	public class ToolbarCustomComboBoxProperties : ToolbarComboBoxProperties {
		public ToolbarCustomComboBoxProperties()
			: base() {
		}
		public ToolbarCustomComboBoxProperties(IPropertiesOwner owner, string defaultCaption)
			: base(owner, defaultCaption) {
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never)]
		public override string DefaultCaption {
			get { return base.DefaultCaption; }
			set { base.DefaultCaption = value; }
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never)]
		public new ListEditItemCollection Items {
			get { return base.Items; }
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never)]
		public new string ClientInstanceName { 
			get{ return base.ClientInstanceName; }
			set { base.ClientInstanceName = value; }
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never)]
		public override string CssFilePath {
			get { return base.CssFilePath; }
			set { base.CssFilePath = value; }
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never)]
		public override string CssPostfix {
			get { return base.CssPostfix; }
			set { base.CssPostfix = value; }
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never)]
		public override bool DisplayFormatInEditMode {
			get { return base.DisplayFormatInEditMode; }
			set { base.DisplayFormatInEditMode = value; }
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never)]
		public new bool EnableClientSideAPI {
			get { return base.EnableClientSideAPI; }
			set { base.EnableClientSideAPI = value; }
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never)]
		public new DefaultBoolean EnableSynchronization {
			get { return base.EnableSynchronization; }
			set { base.EnableSynchronization = value; }
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never)]
		public new EditorDecorationStyle InvalidStyle {
			get { return base.InvalidStyle; }
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never)]
		public new ValidationSettings ValidationSettings {
			get { return base.ValidationSettings; } 
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never)]
		public new DefaultBoolean RenderIFrameForPopupElements {
			get { return base.RenderIFrameForPopupElements; }
			set { base.RenderIFrameForPopupElements = value; }
		}
		[Browsable(false), Themeable(false), AutoFormatDisable, DefaultValue(typeof(string)),
		TypeConverter(typeof(ListEditValueTypeTypeConverter)), NotifyParentProperty(true)]
		public new Type ValueType {
			get { return base.ValueType; }
			set { base.ValueType = value; }
		}
		[Browsable(false), DefaultValue(DropDownStyle.DropDownList), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), 
		EditorBrowsable(EditorBrowsableState.Never), NotifyParentProperty(true), AutoFormatDisable]
		public new DropDownStyle DropDownStyle {
			get { return base.DropDownStyle; }
			set { base.DropDownStyle = value; }
		}
		[DefaultValue("")]
		protected internal string CommandName {
			get { return GetStringProperty("CommandName", ""); }
			set { SetStringProperty("CommandName", "", value); }
		}
		protected override ComboBoxListBoxProperties CreateListBoxProperties() {
			return new ToolbarCustomListBoxProperties(this);
		}
		public override void Assign(PropertiesBase source) {
			base.Assign(source);
			ToolbarCustomComboBoxProperties src = source as ToolbarCustomComboBoxProperties;
			if(src != null) {
				CommandName = src.CommandName;
			}
		}
	}
	public class ToolbarCustomCssComboBoxProperties : ToolbarComboBoxProperties {
		public ToolbarCustomCssComboBoxProperties()
			: base() {
		}
		public ToolbarCustomCssComboBoxProperties(IPropertiesOwner owner, string defaultCaption)
			: base(owner, defaultCaption) {
		}
		protected override ComboBoxListBoxProperties CreateListBoxProperties() {
			return new ToolbarCustomCssListBoxProperties(this);
		}
	}
	public class ToolbarListBoxProperties : ComboBoxListBoxProperties {
		public ToolbarListBoxProperties()
			: base() {
		}
		public ToolbarListBoxProperties(IPropertiesOwner owner)
			: base(owner) {
		}
		protected override ListEditItem CreateListEditItem() {
			return new ToolbarListEditItem();
		}
		protected override ListEditItemCollection CreateListEditItemCollection(bool withOwner) {
			return withOwner ? new ToolbarListEditItemCollection(this) : new ToolbarListEditItemCollection();
		}
	}
	public class ToolbarCustomCssListBoxProperties : ComboBoxListBoxProperties {
		public ToolbarCustomCssListBoxProperties()
			: base() {
		}
		public ToolbarCustomCssListBoxProperties(IPropertiesOwner owner)
			: base(owner) {
		}
		protected override ListEditItem CreateListEditItem() {
			return new ToolbarCustomCssListEditItem();
		}
		protected override ListEditItemCollection CreateListEditItemCollection(bool withOwner) {
			return withOwner ? new ToolbarCustomCssListEditItemCollection(this) : new ToolbarCustomCssListEditItemCollection();
		}
	}
	public class ToolbarCustomListBoxProperties : ComboBoxListBoxProperties {
		public ToolbarCustomListBoxProperties()
			: base() {
		}
		public ToolbarCustomListBoxProperties(IPropertiesOwner owner)
			: base(owner) {
		}
		protected override ListEditItem CreateListEditItem() {
			return new ToolbarCustomListEditItem();
		}
		protected override ListEditItemCollection CreateListEditItemCollection(bool withOwner) {
			return withOwner ? new ToolbarCustomListEditItemCollection(this) : new ToolbarCustomListEditItemCollection();
		}
	}
	public class ToolbarListEditItemCollection : ListEditItemCollection {
		public ToolbarListEditItemCollection()
			: base() {
		}
		public ToolbarListEditItemCollection(IWebControlObject owner)
			: base(owner) {
		}
		public new ToolbarListEditItem this[int index] {
			get { return (GetItem(index) as ToolbarListEditItem); }
		}
		private new void Add(ListEditItem item) { }
		public void Add(ToolbarListEditItem item) {
			base.Add(item);
		}
		public new ToolbarListEditItem Add() {
			ToolbarListEditItem item = new ToolbarListEditItem();
			Add(item);
			return item;
		}
		public new ToolbarListEditItem Add(string text) {
			return Add(text, text);
		}
		public new ToolbarListEditItem Add(string text, object value) {
			return Add(text, value, null);
		}
		public new ToolbarListEditItem Add(string text, object value, string imageUrl) {
			ToolbarListEditItem item = new ToolbarListEditItem(text, value, imageUrl);
			Add(item);
			return item;
		}
		protected override Type GetKnownType() {
			return typeof(ToolbarListEditItem);
		}
	}
	public class ToolbarCustomListEditItemCollection : ToolbarListEditItemCollection {
		public ToolbarCustomListEditItemCollection()
			: base() { }
		public ToolbarCustomListEditItemCollection(IWebControlObject owner)
			: base(owner) { }
		public new ToolbarCustomListEditItem this[int index] {
			get { return (GetItem(index) as ToolbarCustomListEditItem); }
		}
		protected override Type GetKnownType() {
			return typeof(ToolbarCustomListEditItem);
		}
		protected override void OnValidate(object obj) {
			if(!(obj is ToolbarCustomListEditItem)) {
				ToolbarCustomListEditItem item = new ToolbarCustomListEditItem();
				item.Assign(obj as CollectionItem);
				base.OnValidate(item);
			}
			else
				base.OnValidate(obj);
		}
	}
	public class ToolbarCustomCssListEditItemCollection : ToolbarListEditItemCollection {
		public ToolbarCustomCssListEditItemCollection()
			: base() {
		}
		public ToolbarCustomCssListEditItemCollection(IWebControlObject owner)
			: base(owner) {
		}
		public new ToolbarCustomCssListEditItem this[int index] {
			get { return (GetItem(index) as ToolbarCustomCssListEditItem); }
		}
		private new void Add(ToolbarListEditItem item) { }
		public void Add(ToolbarCustomCssListEditItem item) {
			base.Add(item);
		}
		public new ToolbarCustomCssListEditItem Add() {
			ToolbarCustomCssListEditItem item = new ToolbarCustomCssListEditItem();
			Add(item);
			return item;
		}
		public new ToolbarCustomCssListEditItem Add(string text) {
			return Add(text, text);
		}
		public new ToolbarCustomCssListEditItem Add(string text, object value) {
			return Add(text, value, null);
		}
		public ToolbarCustomCssListEditItem Add(string text, string tagName, string cssClass) {
			return Add(text, tagName, cssClass, string.Empty);
		}
		public ToolbarCustomCssListEditItem Add(string text, string tagName, string cssClass, string previewClass) {
			ToolbarCustomCssListEditItem item = new ToolbarCustomCssListEditItem(text, tagName, cssClass, previewClass);
			Add(item);
			return item;
		}
		public new ToolbarCustomCssListEditItem Add(string text, object value, string imageUrl) {
			ToolbarCustomCssListEditItem item = new ToolbarCustomCssListEditItem(text, value, imageUrl);
			Add(item);
			return item;
		}
		protected override Type GetKnownType() {
			return typeof(ToolbarCustomCssListEditItem);
		}
	}
	public class ToolbarListEditItem : ListEditItem {
		public ToolbarListEditItem()
			: base() {
		}
		public ToolbarListEditItem(string text)
			: base(text, text) {
		}
		public ToolbarListEditItem(string text, object value)
			: base(text, value, null) {
		}
		public ToolbarListEditItem(string text, object value, string imageUrl)
			: base(text, value, imageUrl) {
		}
		protected internal new ITemplate TextTemplate {
			get { return base.TextTemplate; }
			set { base.TextTemplate = value; }
		}
	}
	public class ToolbarCustomListEditItem : ToolbarListEditItem {
		public ToolbarCustomListEditItem() { }
		public ToolbarCustomListEditItem(string text)
			: base(text, text) {
		}
		public ToolbarCustomListEditItem(string text, object value)
			: base(text, value, null) {
		}
		public ToolbarCustomListEditItem(string text, object value, string imageUrl)
			: base(text, value, imageUrl) {
		}
	}
	public class ToolbarCustomCssListEditItem : ToolbarListEditItem {
		private CustomCssItemPreviewStyle previewStyle = null;
		public ToolbarCustomCssListEditItem()
			: base() { }
		public ToolbarCustomCssListEditItem(string text)
			: this(text, text) {
		}
		public ToolbarCustomCssListEditItem(string text, object value)
			: this(text, value, null) {
		}
		public ToolbarCustomCssListEditItem(string text, string tagName, string cssClass)
			: this(text, new object(), null) {
			TagName = tagName;
			CssClass = cssClass;
		}
		public ToolbarCustomCssListEditItem(string text, string tagName, string cssClass, string previewCssClass)
			: this(text, new object(), null) {
			TagName = tagName;
			CssClass = cssClass;
			PreviewStyle.CssClass = previewCssClass;
		}
		public ToolbarCustomCssListEditItem(string text, object value, string imageUrl)
			: base() {
			Text = text;
			ImageUrl = imageUrl;
		}
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("ToolbarCustomCssListEditItemPreviewStyle"),
#endif
		Category("Styles"), NotifyParentProperty(true),
		PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public CustomCssItemPreviewStyle PreviewStyle {
			get {
				if(this.previewStyle == null)
					this.previewStyle = new CustomCssItemPreviewStyle();
				return this.previewStyle;
			}
		}
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("ToolbarCustomCssListEditItemTagName"),
#endif
		NotifyParentProperty(true), AutoFormatDisable,
		Editor("DevExpress.Web.ASPxHtmlEditor.Design.TagNameEditor, " + AssemblyInfo.SRAssemblyWebDesignFull, typeof(System.Drawing.Design.UITypeEditor))]
		public string TagName {
			get { return GetStringProperty("TagName", ""); }
			set {
				if(!string.IsNullOrEmpty(value)) {
					value = value.ToLowerInvariant();
					if(Array.IndexOf<string>(TagUtils.TagNames, value) < 0)
						throw new ArgumentException(string.Format("Tag '{0}' is not a valid XHTML 1.0 Transitional tag.", value));
				}
				SetStringProperty("TagName", "", value);
			}
		}
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("ToolbarCustomCssListEditItemCssClass"),
#endif
		NotifyParentProperty(true), AutoFormatDisable]
		public string CssClass {
			get { return GetStringProperty("CssClass", ""); }
			set { SetStringProperty("CssClass", "", value); }
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never)]
		public override object Value {
			get { return string.Format("{0}|{1}", TagName, CssClass); }
			set { }
		}
		public override void Assign(CollectionItem source) {
			ToolbarCustomCssListEditItem src = source as ToolbarCustomCssListEditItem;
			if (src != null) {
				TagName = src.TagName;
				CssClass = src.CssClass;
				PreviewStyle.Assign(src.PreviewStyle);
			}
			base.Assign(source);
		}
		protected internal string GetText() {
			if(string.IsNullOrEmpty(Text))
				return string.IsNullOrEmpty(CssClass) ? Capitalize(TagName) : CssClass;
			return Text;
		}
		private string Capitalize(string tagName) {
			if(tagName == null || tagName.Length == 0)
				return string.Empty;
			return tagName.Substring(0, 1).ToUpperInvariant() + tagName.Substring(1);
		}
		protected override IStateManager[] GetStateManagedObjects() {
			return new IStateManager[] { PreviewStyle };
		}
	}
}
