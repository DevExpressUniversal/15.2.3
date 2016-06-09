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

using DevExpress.Utils;
using DevExpress.Web.Design;
using DevExpress.Web.Internal;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
namespace DevExpress.Web {
	public interface IEditorPropertiesContainer {
		Type GetEditorType();
	}
	public class EditPropertiesBaseBuilder : ControlBuilder {
		public override void Init(TemplateParser parser, ControlBuilder parentBuilder, Type type, string tagName, string id, IDictionary attribs) {
			IEditorPropertiesContainer container = parentBuilder as IEditorPropertiesContainer;
			if(container != null) type = container.GetEditorType();
			base.Init(parser, parentBuilder, type, tagName, id, attribs);
		}
	}
	public enum EditorRequiredMarkMode { Auto, Required, Optional, Hidden }
	public enum ErrorDisplayMode { Text, ImageWithTooltip, ImageWithText, None }
	public enum EditorType { Generic, Blob, Lookup }
	internal delegate bool EditorsProcessingProc(ASPxEdit edit);
	internal delegate bool EditorsChoiceCondition(ASPxEdit edit, string validationGroup);
	public interface IValueProvider {
		object GetValue(string fieldName);
	}
	public interface IHighlightTextProcessor {
		string HighlightText(string text, bool encode);
	}
	public interface IAssociatedControlID {
		string ClientID();
	}
	public class CreateEditorArgs {
		private object editValue;
		private Type dataType;
		private EditorImages images;
		private EditorStyles styles;
		public CreateEditorArgs(object editValue, Type dataType, EditorImages images, EditorStyles styles) {
			this.editValue = editValue;
			this.dataType = ReflectionUtils.StripNullableType(dataType);			
			this.images = images;
			this.styles = styles;
		}
		public Type DataType {
			get { return dataType; }
		}
		public object EditValue {
			get { return editValue; }
			set { editValue = value; }
		}
		public EditorImages Images {
			get { return images; }
		}
		public EditorStyles Styles {
			get { return styles; }
		}
	}
	public class CreateDisplayControlArgs : CreateEditorArgs {
		private string displayText;
		private IValueProvider valueProvider;
		private Control parent;
		private bool designMode;
		private ISkinOwner skinOwner;
		public CreateDisplayControlArgs(object editValue, Type dataType, string displayText, IValueProvider valueProvider, EditorImages images, EditorStyles styles, Control parent, bool designMode)
			: this(editValue, dataType, displayText, valueProvider, images, styles, null, parent, designMode) {
		}
		public CreateDisplayControlArgs(object editValue, Type dataType, string displayText, IValueProvider valueProvider, EditorImages images, EditorStyles styles, ISkinOwner skinOwner, Control parent, bool designMode)
			: this(editValue, dataType, displayText, valueProvider, images, styles, skinOwner, parent, designMode, null) {
		}
		public CreateDisplayControlArgs(object editValue, Type dataType, string displayText, IValueProvider valueProvider, EditorImages images, EditorStyles styles, ISkinOwner skinOwner, Control parent, bool designMode, bool? encodeHtml)
			: base(editValue, dataType, images, styles) {
			this.displayText = displayText;
			this.valueProvider = valueProvider;
			this.parent = parent;
			this.designMode = designMode;
			this.skinOwner = skinOwner;
			EncodeHtml = encodeHtml;
		}
		public string DisplayText {
			get { return displayText; }
		}
		public IValueProvider ValueProvider {
			get { return valueProvider; }
		}
		public Control Parent {
			get { return parent; }
		}
		public Page Page {
			get { return (Parent != null) ? Parent.Page : null; ; }
		}
		public bool DesignMode {
			get { return designMode; }
		}
		public ISkinOwner SkinOwner {
			get { return skinOwner; }
		}
		public bool? EncodeHtml { get; set; }
		public bool DecodeDisplayFormat { get; set; }
		internal IHighlightTextProcessor HighlightTextProcessor { get; set; }
	}
	public enum EditorInplaceMode { StandAlone, Inplace, EditForm }
	public class CreateEditControlArgs : CreateEditorArgs {
		private ISkinOwner skinOwner;
		private EditorInplaceMode inplaceMode;
		private bool inplaceAllowsEditorSizeRecalc;
		private int editingRowVisibleIndex = -1;
		public CreateEditControlArgs(object editValue, Type dataType,
			EditorImages images, EditorStyles styles, ISkinOwner skinOwner,
			EditorInplaceMode inplaceMode, bool inplaceAllowsEditorSizeRecalc)
			: this(-1, editValue, dataType, images, styles, skinOwner, inplaceMode, inplaceAllowsEditorSizeRecalc) {
		}
		public CreateEditControlArgs(int editingRowVisibleIndex, object editValue, Type dataType,
			EditorImages images, EditorStyles styles, ISkinOwner skinOwner,
			EditorInplaceMode inplaceMode, bool inplaceAllowsEditorSizeRecalc)
			: base(editValue, dataType, images, styles) {
			this.editingRowVisibleIndex = editingRowVisibleIndex;
			this.skinOwner = skinOwner;
			this.inplaceMode = inplaceMode;
			this.inplaceAllowsEditorSizeRecalc = inplaceAllowsEditorSizeRecalc;
		}
		public bool InplaceAllowsEditorSizeRecalc {
			get { return inplaceAllowsEditorSizeRecalc; }
		}
		public EditorInplaceMode InplaceMode {
			get { return inplaceMode; }
		}
		public ISkinOwner SkinOwner {
			get { return skinOwner; }
		}
		public int EditingRowVisibleIndex {
			get { return editingRowVisibleIndex; }
		}
	}
	public class EditRegistrationInfo {
		const string DefaultEditor = "TextBox";
		static Dictionary<string, Type> editors;
		public static Dictionary<string, Type> Editors {
			get {
				if(editors == null) editors = RegisterEditors();
				return editors;
			}
		}
		public static EditPropertiesBase CreateProperties(string editType) {
			Type type = GetEditType(editType);
			if(type == null) return null;
			return Activator.CreateInstance(type) as EditPropertiesBase;
		}
		public static Type GetEditType(string editType) {
			if(!Editors.ContainsKey(editType)) return null;
			return Editors[editType];
		}
		public static EditPropertiesBase CreatePropertiesByDataType(Type type) {
			type = ReflectionUtils.StripNullableType(type);
			if(type == typeof(DateTime)) return CreateProperties("DateEdit");
			if(type == typeof(bool)) return CreateProperties("CheckBox");
			return CreateProperties(DefaultEditor);
		}
		static Dictionary<string, Type> RegisterEditors() {
			Dictionary<string, Type> res = new Dictionary<string, Type>();
			res.Add(DefaultEditor, typeof(DevExpress.Web.TextBoxProperties));
			res.Add("ButtonEdit", typeof(DevExpress.Web.ButtonEditProperties));
			res.Add("CheckBox", typeof(DevExpress.Web.CheckBoxProperties));
			res.Add("Memo", typeof(DevExpress.Web.MemoProperties));
			res.Add("Image", typeof(DevExpress.Web.ImageEditProperties));
			res.Add("HyperLink", typeof(DevExpress.Web.HyperLinkProperties));
			res.Add("DateEdit", typeof(DevExpress.Web.DateEditProperties));
			return res;
		}
		public static string GetEditName(EditPropertiesBase properties) {
			if(properties == null) return string.Empty;
			foreach(KeyValuePair<string, Type> pair in Editors) {
				if(pair.Value.Equals(properties.GetType())) return pair.Key;
			}
			return string.Empty;
		}
	}
	[TypeConverter(typeof(ExpandableObjectConverter)), ControlBuilder(typeof(EditPropertiesBaseBuilder))]
	public abstract class EditPropertiesBase : PropertiesBase, IWebControlObject, ISkinOwner, IDesignTimePropertiesOwner, IAssignEditorProperties {
		private EditClientSideEventsBase clientSideEvents;
		private EditorImages images;
		private EditorStyles styles;
		private ISkinOwner parentSkinOwner;
		private ImagesBase parentImages;
		private StylesBase parentStyles;
		private EditorImages renderImages;
		private EditorStyles renderStyles;
		private bool inplaceEditorBound = false;
		private bool inInplaceBoud;
		private EditorCaptionSettingsBase captionSettings = null;
		public EditPropertiesBase()
			: this(null) {
		}
		public EditPropertiesBase(IPropertiesOwner owner)
			: base(owner) {
			this.clientSideEvents = CreateClientSideEvents();
			this.images = new EditorImages(this);
			this.styles = CreateEditorStyles();
		}
		protected internal virtual EditorStyles CreateEditorStyles() {
			return new EditorStyles(this);
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("EditPropertiesBaseDisplayFormatString"),
#endif
		DefaultValue(""), NotifyParentProperty(true), Localizable(true), AutoFormatDisable]
		public virtual string DisplayFormatString {
			get { return GetStringProperty("DisplayFormatString", DefaultDisplayFormatString); }
			set { 
				SetStringProperty("DisplayFormatString", DefaultDisplayFormatString, value);
				this.decodedDisplayFormat = null;
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("EditPropertiesBaseNullDisplayText"),
#endif
		Category("Behavior"), DefaultValue(""), NotifyParentProperty(true), Localizable(true), AutoFormatDisable]
		public virtual string NullDisplayText {
			get { return GetStringProperty("NullDisplayText", ""); }
			set { SetStringProperty("NullDisplayText", "", value); }
		}
		[
		Localizable(true), DefaultValue(""), AutoFormatEnable, NotifyParentProperty(true)]
		protected internal string Caption {
			get { return GetStringProperty("Caption", ""); }
			set {
				SetStringProperty("Caption", "", value);
				LayoutChanged();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("EditPropertiesBaseClientInstanceName"),
#endif
		Category("Client-Side"), DefaultValue(""), Localizable(false), NotifyParentProperty(true), AutoFormatDisable]
		public string ClientInstanceName {
			get { return GetStringProperty("ClientInstanceName", ""); }
			set { SetStringProperty("ClientInstanceName", "", value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("EditPropertiesBaseEnableClientSideAPI"),
#endif
		Category("Client-Side"), DefaultValue(false), NotifyParentProperty(true), AutoFormatDisable]
		public bool EnableClientSideAPI {
			get { return GetBoolProperty("EnableClientSideAPI", false); }
			set { SetBoolProperty("EnableClientSideAPI", false, value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("EditPropertiesBaseEncodeHtml"),
#endif
		Category("Behavior"), DefaultValue(true), NotifyParentProperty(true), AutoFormatDisable]
		public virtual bool EncodeHtml {
			get { return GetBoolProperty("EncodeHtml", true); }
			set { SetBoolProperty("EncodeHtml", true, value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("EditPropertiesBaseEnableDefaultAppearance"),
#endif
		Obsolete("This property is now obsolete. Use the corresponding style settings to override control elements' appearance."),
		Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		Category("Appearance"), DefaultValue(true), NotifyParentProperty(true), AutoFormatEnable]
		public virtual bool EnableDefaultAppearance {
			get { return true; }
			set { }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("EditPropertiesBaseCssPostfix"),
#endif
		Category("Styles"), DefaultValue(""), Localizable(false), AutoFormatEnable,
		NotifyParentProperty(true)]
		public virtual string CssPostfix {
			get { return Styles.CssPostfix; }
			set { Styles.CssPostfix = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("EditPropertiesBaseCssFilePath"),
#endif
		Category("Styles"), DefaultValue(""), Localizable(false),
		UrlProperty, AutoFormatEnable, AutoFormatUrlProperty, NotifyParentProperty(true),
		Editor(typeof(System.Web.UI.Design.UrlEditor), typeof(System.Drawing.Design.UITypeEditor))]
		public virtual string CssFilePath {
			get { return Styles.CssFilePath; }
			set { Styles.CssFilePath = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("EditPropertiesBaseStyle"),
#endif
		Category("Styles"), PersistenceMode(PersistenceMode.InnerProperty), NotifyParentProperty(true), AutoFormatEnable,
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public EditStyleBase Style {
			get { return Styles.Style; }
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public ISkinOwner ParentSkinOwner {
			get { return parentSkinOwner; }
			set { parentSkinOwner = value; }
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public ImagesBase ParentImages {
			get { return parentImages; }
			set { parentImages = value; }
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public StylesBase ParentStyles {
			get { return parentStyles; }
			set { parentStyles = value; }
		}
		protected internal bool Native {
			get { return Styles.NativeInternal; }
			set { Styles.NativeInternal = value; }
		}
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		Browsable(false), EditorBrowsable(EditorBrowsableState.Never), PersistenceMode(PersistenceMode.InnerProperty),
		DefaultValue(null), AutoFormatDisable,
		NotifyParentProperty(true)]
		public object HiddenSerializableObject { 
			get { return SerializationUtils.GetSerializableInnerPropertyObject(null, () => new object(), true); }
		}
		protected internal EditorCaptionCellStyle CaptionCellStyle {
			get { return Styles.CaptionCell; }
		}
		protected internal EditorCaptionStyle CaptionStyle {
			get { return Styles.Caption; }
		}
		protected internal EditorRootStyle RootStyle {
			get { return Styles.Root; }
		}
		internal virtual int EditingRowVisibleIndex {
			get { return -1; }
			set { }
		}
		protected internal EditorImages Images {
			get { return images; }
		}
		protected internal EditorStyles Styles {
			get { return styles; }
		}
		protected internal EditorStyles RenderStyles {
			get {
				if(ParentStyles == null) return Styles;
				if(this.renderStyles == null || !UseCachedObjects()) {
					this.renderStyles = new EditorStyles(this);
					this.renderStyles.CopyFrom(ParentStyles);
					if(!UseParentStylesOnly)
						this.renderStyles.CopyFrom(Styles);
				}
				return this.renderStyles;
			}
		}
		protected internal void ResetRenderStyles() {
			this.renderStyles = null;
		}
		protected virtual bool UseParentStylesOnly {
			get { return false; }
		}
		protected internal EditorImages RenderImages {
			get {
				if(ParentImages == null) return Images;
				if(this.renderImages == null || !UseCachedObjects()) {
					this.renderImages = new EditorImages(this);
					this.renderImages.CopyFrom(ParentImages);
					if(!UseParentImagesOnly)
						this.renderImages.CopyFrom(Images);
				}
				return this.renderImages;
			}
		}
		protected virtual bool UseParentImagesOnly {
			get { return false; }
		}
		protected virtual string DefaultDisplayFormatString {
			get { return ""; }
		}
		protected internal EditClientSideEventsBase ClientSideEvents {
			get { return clientSideEvents; }
		}
		protected virtual bool IsRequireInplaceBound {
			get { return false; }
		}
		public override void Assign(PropertiesBase source) {
			BeginUpdate();
			try {
				base.Assign(source);
				EditPropertiesBase src = source as EditPropertiesBase;
				if(src != null) {
					ClientInstanceName = src.ClientInstanceName;
					EnableClientSideAPI = src.EnableClientSideAPI;
					EncodeHtml = src.EncodeHtml;
					Caption = src.Caption;
					CaptionSettingsInternal.Assign(src.CaptionSettingsInternal);
					ClientSideEvents.Assign(src.ClientSideEvents);
					NullDisplayText = src.NullDisplayText;
					DisplayFormatString = src.DisplayFormatString;
					Images.Assign(src.Images);
					Styles.Assign(src.Styles);
				}
			}
			finally {
				EndUpdate();
			}
		}
		protected internal EditorCaptionSettingsBase CaptionSettingsInternal {
			get { return this.captionSettings = this.captionSettings ?? CreateCaptionSettings(); }
		}
		protected virtual EditorCaptionSettingsBase CreateCaptionSettings() {
			return new EditorCaptionSettingsBase(this);
		}
		protected internal void CheckInplaceBound(Type dataType, Control parent, bool designMode) {
			if(!IsRequireInplaceBound || this.inplaceEditorBound || designMode || parent == null)
				return;
			this.inInplaceBoud = true;
			try {
				ASPxEditBase edit = CreateEdit(new CreateEditControlArgs(null, dataType, null, null, null, EditorInplaceMode.Inplace, true));
				parent.Controls.Add(edit);
				edit.DataBind();
				parent.Controls.Remove(edit);
				if(edit.Bound)
					AssignInplaceBoundProperties(edit);
				this.inplaceEditorBound = true;
			} finally {
				this.inInplaceBoud = false;
			}
		}
		public void RequireDataBinding() {
			this.inplaceEditorBound = false;
		}
		protected virtual void AssignInplaceBoundProperties(ASPxEditBase edit) {
		}
		protected virtual void AssignInplaceProperties(CreateEditorArgs args) {
			ParentImages = args.Images;
			ParentStyles = args.Styles;
		}
		protected virtual void AssignEditorProperties(ASPxEditBase edit) {
		}
		public ASPxEditBase CreateEdit(CreateEditControlArgs args) {
			return CreateEdit(args, false);
		}
		public virtual ASPxEditBase CreateEdit(CreateEditControlArgs args, bool isInternal) {
			return CreateEdit(args, isInternal, CreateEditInstance);
		}
		protected internal virtual ASPxEditBase CreateEdit(CreateEditControlArgs args, bool isInternal, Func<ASPxEditBase> createEditAction) {
			AssignInplaceProperties(args);
			ASPxEditBase editBase = createEditAction();
			InitEdit(args, editBase);
			return editBase;
		}
		void InitEdit(CreateEditControlArgs args, ASPxEditBase editBase) {
			editBase.EnableViewState = false;
			editBase.Value = args.EditValue;
			editBase.InplaceMode = args.InplaceMode;
			editBase.InplaceAllowsEditorSizeRecalc = args.InplaceAllowsEditorSizeRecalc;
			editBase.Properties.Assign(this);
			editBase.Properties.EditingRowVisibleIndex = args.EditingRowVisibleIndex;
			editBase.Properties.ParentSkinOwner = args.SkinOwner;
			editBase.Properties.ParentImages = args.Images;
			editBase.Properties.ParentStyles = args.Styles;
			AssignEditorProperties(editBase);
			editBase.SetOwnerControl(args.SkinOwner as ASPxWebControl); 
			ASPxEdit edit = editBase as ASPxEdit;
			if(edit != null)
				AssignValidationSettings(edit);
		}
		void AssignValidationSettings(ASPxEdit edit) {
			edit.ValidationSettings.ErrorFrameStyle.BackColor = System.Drawing.Color.Transparent;
			if(!edit.ValidationSettings.ErrorFrameStyle.Paddings.IsEmpty) {
				edit.ValidationSettings.ErrorFrameStyle.Paddings.PaddingTop = 0;
				edit.ValidationSettings.ErrorFrameStyle.Paddings.PaddingLeft = 0;
				edit.ValidationSettings.ErrorFrameStyle.Paddings.PaddingBottom = 0;
			}
			if(!edit.ValidationSettings.ErrorFrameStyle.ErrorTextPaddings.IsEmpty) {
				edit.ValidationSettings.ErrorFrameStyle.ErrorTextPaddings.PaddingTop = 0;
				edit.ValidationSettings.ErrorFrameStyle.ErrorTextPaddings.PaddingRight = 0;
				edit.ValidationSettings.ErrorFrameStyle.ErrorTextPaddings.PaddingBottom = 0;
			}
			if(edit.ValidationSettings.ErrorDisplayMode != ErrorDisplayMode.None) 
				edit.ValidationSettings.ErrorFrameStyle.Border.BorderWidth = 0;
		}
		protected abstract ASPxEditBase CreateEditInstance();
		public Control CreateDisplayControl(CreateDisplayControlArgs args) {
			AssignInplaceProperties(args);
			ParentSkinOwner = args.SkinOwner;
			Control control = CreateDisplayControlInstance(args);
			control.EnableViewState = false;
			return control;
		}
		protected virtual Control CreateDisplayControlInstance(CreateDisplayControlArgs args) {
			string text = RenderUtils.CheckEmptyRenderText(GetDisplayText(args));
			return RenderUtils.CreateLiteralControl(text);
		}
		public virtual HorizontalAlign GetDisplayControlDefaultAlign() {
			return HorizontalAlign.NotSet;
		}
		string decodedDisplayFormat;
		protected internal string DecodedDisplayFormat {
			get {
				if(string.IsNullOrEmpty(DisplayFormatString))
					return string.Empty;
				if(string.IsNullOrEmpty(decodedDisplayFormat))
					decodedDisplayFormat = DecodeDisplayFormat(DisplayFormatString);
				return decodedDisplayFormat;
			}
		}
		protected virtual string DecodeDisplayFormat(string format) { 
			var result = StripHtmlTags(format);
			return HttpUtility.HtmlDecode(result);
		}
		static Regex StripHTMLTagsRegex = new Regex("<[^>]*>", RegexOptions.Compiled);
		protected virtual string StripHtmlTags(string format) {
			if(format.IndexOf("<") < 0) return format;
			return StripHTMLTagsRegex.Replace(format, string.Empty);
		}
		protected virtual bool ShouldEncodDisplayText(CreateDisplayControlArgs args) {
			return args.EncodeHtml.HasValue ? args.EncodeHtml.Value : EncodeHtml;
		}
		public string GetDisplayText(CreateDisplayControlArgs args) {
			return GetDisplayText(args, ShouldEncodDisplayText(args));
		}
		public virtual string GetDisplayText(CreateDisplayControlArgs args, bool encode) {
			var result = GetDisplayTextCore(args, encode);
			if(args.HighlightTextProcessor != null)
				result = args.HighlightTextProcessor.HighlightText(result, encode);
			return result;
		}
		protected virtual string GetDisplayTextCore(CreateDisplayControlArgs args, bool encode) {
			string result = null;
			if(args.DisplayText != null) {
				result = encode ? HttpUtility.HtmlEncode(args.DisplayText) : args.DisplayText;
			}
			else {
				object editValue = args.EditValue;
				if(!CommonUtils.IsNullValue(editValue))
					PrepareNotNullEditValue(ref editValue);
				if(CommonUtils.IsNullValue(editValue))
					result = NullDisplayText;
				else {
					if(encode && (editValue is string))
						editValue = HttpUtility.HtmlEncode((string)editValue);
					var formatString = args.DecodeDisplayFormat ? DecodedDisplayFormat : DisplayFormatString;
					result = string.Format(CommonUtils.GetFormatString(formatString), editValue);
				}
			}
			return result;
		}
		public override string ToString() {
			return "";
		}
		protected virtual EditClientSideEventsBase CreateClientSideEvents() {
			return new EditClientSideEventsBase();
		}
		protected virtual void PrepareNotNullEditValue(ref object editValue) {
		}
		protected bool UseCachedObjects() {
			return IsRendering() && !IsDesignMode();
		}
		public virtual EditorType GetEditorType() {
			return EditorType.Generic;
		}
		public virtual string GetExportDisplayText(CreateDisplayControlArgs args) {
			return GetDisplayText(args, false);
		}
		public virtual object GetExportValue(CreateDisplayControlArgs args) {
			return args.EditValue;
		}
		public virtual string GetExportNavigateUrl(CreateDisplayControlArgs args) {
			return string.Empty;
		}
		protected virtual bool IsClientSideEventsStoreToViewState() {
			return true;
		}
		protected virtual bool IsImagesStoreToViewState() {
			return true;
		}
		protected virtual bool IsStylesStoreToViewState() {
			return true;
		}
		protected override IStateManager[] GetStateManagedObjects() {
			List<IStateManager> objects = new List<IStateManager>();
			if(IsClientSideEventsStoreToViewState())
				objects.Add(ClientSideEvents);
			if(IsStylesStoreToViewState())
				objects.Add(Styles);
			if(IsImagesStoreToViewState())
				objects.Add(Images);
			objects.Add(CaptionSettingsInternal);
			return objects.ToArray();
		}
		void IPropertiesOwner.Changed(PropertiesBase properties) {
			if(properties != this.renderImages && properties != this.renderStyles)
				base.Changed();
		}
		void IAssignEditorProperties.AssignEditorProperties(ASPxEditBase editor) {
			AssignEditorProperties(editor);
		}
		bool IWebControlObject.IsDesignMode() {
			return IsDesignMode();
		}
		bool IWebControlObject.IsLoading() {
			return IsLoading();
		}
		bool IWebControlObject.IsRendering() {
			return IsRendering();
		}
		void IWebControlObject.LayoutChanged() {
			LayoutChanged();
		}
		void IWebControlObject.TemplatesChanged() {
			TemplatesChanged();
		}
		protected virtual bool IsDesignMode() {
			if(Owner is IWebControlObject)
				return (Owner as IWebControlObject).IsDesignMode();
			return false;
		}
		protected virtual bool IsLoading() {
			if(Owner is IWebControlObject)
				return (Owner as IWebControlObject).IsLoading();
			return false;
		}
		protected virtual bool IsRendering() {
			if(Owner is IWebControlObject)
				return (Owner as IWebControlObject).IsRendering();
			return false;
		}
		protected virtual bool LayoutChangedLocked() {
			return inInplaceBoud;
		}
		protected virtual void LayoutChanged() {
			this.decodedDisplayFormat = null;
			if(!LayoutChangedLocked() && Owner is IWebControlObject)
				(Owner as IWebControlObject).LayoutChanged();
		}
		protected virtual void TemplatesChanged() {
			if(Owner is IWebControlObject)
				(Owner as IWebControlObject).TemplatesChanged();
		}
		protected ISkinOwner GetParentSkinOwner() {
			if(ParentSkinOwner != null)
				return ParentSkinOwner;
			ISkinOwner skinOwner = RenderUtils.FindParentSkinOwner(Owner as Control);
			if(UseCachedObjects())
				ParentSkinOwner = skinOwner;
			return skinOwner;
		}
		protected virtual string GetCssFilePath() {
			if(!UseParentStylesOnly && !string.IsNullOrEmpty(Styles.CssFilePath))
				return Styles.CssFilePath;
			else if(ParentStyles != null && !string.IsNullOrEmpty(ParentStyles.CssFilePath))
				return ParentStyles.CssFilePath;
			else {
				ISkinOwner parentSkinOwner = GetParentSkinOwner();
				return (parentSkinOwner != null) ? parentSkinOwner.GetCssFilePath() : string.Empty;
			}
		}
		protected virtual string GetImageFolder() {
			if(!UseParentStylesOnly && !string.IsNullOrEmpty(Images.ImageFolder))
				return Images.ImageFolder;
			else if(ParentImages != null && !string.IsNullOrEmpty(ParentImages.ImageFolder))
				return ParentImages.ImageFolder;
			else {
				ISkinOwner parentSkinOwner = GetParentSkinOwner();
				return (parentSkinOwner != null) ? parentSkinOwner.GetImageFolder() : string.Empty;
			}
		}
		protected virtual string GetSpriteImageUrl() {
			if(!UseParentStylesOnly && !string.IsNullOrEmpty(Images.SpriteImageUrl))
				return Images.SpriteImageUrl;
			else if(ParentImages != null && !string.IsNullOrEmpty(ParentImages.SpriteImageUrl))
				return ParentImages.SpriteImageUrl;
			else {
				ISkinOwner parentSkinOwner = GetParentSkinOwner();
				return (parentSkinOwner != null) ? parentSkinOwner.GetSpriteImageUrl() : string.Empty;
			}
		}
		protected virtual string GetSpriteCssFilePath() {
			if(!UseParentStylesOnly && !string.IsNullOrEmpty(Images.SpriteCssFilePath))
				return Images.SpriteCssFilePath;
			else if(ParentImages != null && !string.IsNullOrEmpty(ParentImages.SpriteCssFilePath))
				return ParentImages.SpriteCssFilePath;
			else {
				ISkinOwner parentSkinOwner = GetParentSkinOwner();
				return (parentSkinOwner != null) ? parentSkinOwner.GetSpriteCssFilePath() : string.Empty;
			}
		}
		protected virtual string GetCssPostFix() {
			if(!UseParentStylesOnly && !string.IsNullOrEmpty(Styles.CssPostfix))
				return Styles.CssPostfix;
			else if(ParentStyles != null && !string.IsNullOrEmpty(ParentStyles.CssPostfix))
				return ParentStyles.CssPostfix;
			else {
				ISkinOwner parentSkinOwner = GetParentSkinOwner();
				return (parentSkinOwner != null) ? parentSkinOwner.GetCssPostFix() : string.Empty;
			}
		}
		protected virtual string GetTheme() {
			if (!UseParentStylesOnly && !string.IsNullOrEmpty(Styles.Theme))
				return Styles.Theme;
			else if (ParentStyles != null && !string.IsNullOrEmpty(ParentStyles.Theme))
				return ParentStyles.Theme;
			else {
				ISkinOwner parentSkinOwner = GetParentSkinOwner();
				return (parentSkinOwner != null) ? parentSkinOwner.GetTheme() : string.Empty;
			}
		}
		protected virtual void MergeControlStyle(AppearanceStyleBase style) {
		}
		protected internal void MergeParentSkinOwnerControlStyle(AppearanceStyleBase style) {
			ISkinOwner parentSkinOwner = GetParentSkinOwner();
			if(parentSkinOwner != null)
				parentSkinOwner.MergeControlStyle(style);
		}
		protected virtual bool IsDefaultAppearanceEnabled() {
			return true;
		}
		protected virtual bool IsAccessibilityCompliant() {
			bool ret = !UseParentStylesOnly && Styles.AccessibilityCompliantInternal;
			if(ParentStyles != null && ParentStyles is EditorStyles)
				ret = ret || ((EditorStyles)ParentStyles).AccessibilityCompliantInternal;
			ISkinOwner parentSkinOwner = GetParentSkinOwner();
			if(parentSkinOwner != null)
				ret = ret || parentSkinOwner.IsAccessibilityCompliant();
			return ret;
		}
		protected virtual bool IsNative() {
			bool ret = !UseParentStylesOnly && Styles.NativeInternal;
			if(ParentStyles != null && ParentStyles is EditorStyles)
				ret = ret || ((EditorStyles)ParentStyles).NativeInternal;
			ISkinOwner parentSkinOwner = GetParentSkinOwner();
			if(parentSkinOwner != null)
				ret = ret || parentSkinOwner.IsNative();
			return ret;
		}
		protected virtual bool IsRightToLeft() {
			DefaultBoolean value = Styles.RightToLeftInternal;
			if(UseParentStylesOnly || value == DefaultBoolean.Default) {
				EditorStyles parentEditorStyles = ParentStyles as EditorStyles;
				if(parentEditorStyles != null)
					value = parentEditorStyles.RightToLeftInternal;
			}
			if(value == DefaultBoolean.Default) {
				ISkinOwner parentSkinOwner = GetParentSkinOwner();
				if(parentSkinOwner != null)
					return parentSkinOwner.IsRightToLeft();
			}
			if(value == DefaultBoolean.True)
				return true;
			if(value == DefaultBoolean.False)
				return false;
			return ASPxWebControl.GlobalRightToLeft == DefaultBoolean.True;
		}
		protected virtual bool IsNativeSupported() {
			return false;
		}
		string ISkinOwner.GetControlName() {
			return "Editors";
		}
		string[] ISkinOwner.GetChildControlNames() {
			return new string[] { };
		}
		string ISkinOwner.GetCssFilePath() {
			return GetCssFilePath();
		}
		string ISkinOwner.GetImageFolder() {
			return GetImageFolder();
		}
		string ISkinOwner.GetSpriteImageUrl() {
			return GetSpriteImageUrl();
		}
		string ISkinOwner.GetSpriteCssFilePath() {
			return GetSpriteCssFilePath();
		}
		string ISkinOwner.GetCssPostFix() {
			return GetCssPostFix();
		}
		string ISkinOwner.GetTheme() {
			return GetTheme();
		}
		void ISkinOwner.MergeControlStyle(AppearanceStyleBase style) {
			MergeControlStyle(style);
		}
		bool ISkinOwner.IsDefaultAppearanceEnabled() {
			return IsDefaultAppearanceEnabled();
		}
		bool ISkinOwner.IsAccessibilityCompliant() {
			return IsAccessibilityCompliant();
		}
		bool ISkinOwner.IsNative() {
			return IsNative();
		}
		bool ISkinOwner.IsNativeSupported() {
			return IsNativeSupported();
		}
		bool ISkinOwner.IsRightToLeft() {
			return IsRightToLeft();
		}
		object IDesignTimePropertiesOwner.Owner { get { return Owner; } }
	}
	public class EditorCaptionSettingsBase : PropertiesBase {
		const EditorCaptionPosition DefaultPosition = EditorCaptionPosition.Left;
		const EditorCaptionHorizontalAlign DefaultHorizontalAlign = EditorCaptionHorizontalAlign.Left;
		const EditorCaptionHorizontalAlign DefaultRTLHorizontalAlign = EditorCaptionHorizontalAlign.Right;
		const EditorCaptionVerticalAlign DefaultVerticalAlign = EditorCaptionVerticalAlign.Top;
		[
#if !SL
	DevExpressWebLocalizedDescription("EditorCaptionSettingsBasePosition"),
#endif
		DefaultValue(EditorCaptionPosition.NotSet), NotifyParentProperty(true), AutoFormatEnable]
		public EditorCaptionPosition Position {
			get { return (EditorCaptionPosition)GetEnumProperty("Position", EditorCaptionPosition.NotSet); }
			set {
				SetEnumProperty("Position", EditorCaptionPosition.NotSet, value);
				Changed();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("EditorCaptionSettingsBaseHorizontalAlign"),
#endif
		DefaultValue(EditorCaptionHorizontalAlign.NotSet), NotifyParentProperty(true), AutoFormatEnable]
		public EditorCaptionHorizontalAlign HorizontalAlign {
			get { return (EditorCaptionHorizontalAlign)GetEnumProperty("HorizontalAlign", EditorCaptionHorizontalAlign.NotSet); }
			set { SetEnumProperty("HorizontalAlign", EditorCaptionHorizontalAlign.NotSet, value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("EditorCaptionSettingsBaseVerticalAlign"),
#endif
		DefaultValue(EditorCaptionVerticalAlign.NotSet), NotifyParentProperty(true), AutoFormatEnable]
		public EditorCaptionVerticalAlign VerticalAlign {
			get { return (EditorCaptionVerticalAlign)GetEnumProperty("VerticalAlign", EditorCaptionVerticalAlign.NotSet); }
			set { SetEnumProperty("VerticalAlign", EditorCaptionVerticalAlign.NotSet, value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("EditorCaptionSettingsBaseShowColon"),
#endif
		DefaultValue(true), NotifyParentProperty(true), AutoFormatEnable]
		public bool ShowColon {
			get { return GetBoolProperty("ShowColon", true); ; }
			set { SetBoolProperty("ShowColon", true, value); }
		}
		public EditorCaptionSettingsBase(IPropertiesOwner owner) : base(owner) { }
		public override void Assign(PropertiesBase source) {
			BeginUpdate();
			try {
				base.Assign(source);
				EditorCaptionSettingsBase src = source as EditorCaptionSettingsBase;
				if (src != null) {
					Position = src.Position;
					VerticalAlign = src.VerticalAlign;
					HorizontalAlign = src.HorizontalAlign;
					ShowColon = src.ShowColon;
				}
			}
			finally {
				EndUpdate();
			}
		}
		protected internal EditorCaptionPosition GetPosition() {
			return Position != EditorCaptionPosition.NotSet ? Position : DefaultPosition;
		}
		protected internal EditorCaptionHorizontalAlign GetHorizontalAlign() {
			return HorizontalAlign != EditorCaptionHorizontalAlign.NotSet ? HorizontalAlign : GetDefaultHorizontalAlign();
		}
		protected internal EditorCaptionVerticalAlign GetVerticalAlign() {
			return VerticalAlign != EditorCaptionVerticalAlign.NotSet ? VerticalAlign : DefaultVerticalAlign;
		}
		protected EditorCaptionHorizontalAlign GetDefaultHorizontalAlign() {
			return Owner != null && (Owner as ISkinOwner).IsRightToLeft() ? DefaultRTLHorizontalAlign : DefaultHorizontalAlign;
		}
	}
	public class EditorCaptionSettings : EditorCaptionSettingsBase {
		protected const string DefaultRequiredMark = "*";
		protected const string DefaultOptionalMark = "(optional)";
		[
#if !SL
	DevExpressWebLocalizedDescription("EditorCaptionSettingsRequiredMark"),
#endif
		Localizable(true), DefaultValue(DefaultRequiredMark), AutoFormatEnable, NotifyParentProperty(true)]
		public string RequiredMark {
			get { return GetStringProperty("RequiredMark", DefaultRequiredMark); }
			set { SetStringProperty("RequiredMark", DefaultRequiredMark, value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("EditorCaptionSettingsOptionalMark"),
#endif
		Localizable(true), DefaultValue(DefaultOptionalMark), AutoFormatEnable, NotifyParentProperty(true)]
		public string OptionalMark {
			get { return GetStringProperty("OptionalMark", DefaultOptionalMark); }
			set { SetStringProperty("OptionalMark", DefaultOptionalMark, value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("EditorCaptionSettingsRequiredMarkDisplayMode"),
#endif
		DefaultValue(EditorRequiredMarkMode.Auto), NotifyParentProperty(true), AutoFormatDisable]
		public EditorRequiredMarkMode RequiredMarkDisplayMode {
			get { return (EditorRequiredMarkMode)GetEnumProperty("RequiredMarkDisplayMode", EditorRequiredMarkMode.Auto); }
			set {
				SetEnumProperty("RequiredMarkDisplayMode", EditorRequiredMarkMode.Auto, value);
				Changed();
			}
		}
		public EditorCaptionSettings(IPropertiesOwner owner) : base(owner) { }
		public override void Assign(PropertiesBase source) {
			BeginUpdate();
			try {
				base.Assign(source);
				EditorCaptionSettings src = source as EditorCaptionSettings;
				if (src != null) {
					RequiredMark = src.RequiredMark;
					OptionalMark = src.OptionalMark;
					RequiredMarkDisplayMode = src.RequiredMarkDisplayMode;
				}
			}
			finally {
				EndUpdate();
			}
		}
	}
	[DevExpress.Utils.Design.DXClientDocumentationProviderWeb("ASPxEdit"),
	Designer("DevExpress.Web.Design.ASPxEditDesignerBase, " + AssemblyInfo.SRAssemblyWebDesignFull)
]
	public abstract class ASPxEditBase : ASPxDataWebControl {
		protected internal const string EditImagesResourcePath = "DevExpress.Web.Images.Editors.";
		protected internal const string EditScriptsResourcePath = "DevExpress.Web.Scripts.Editors.";
		protected internal const string EditScriptResourceName = EditScriptsResourcePath + "Edit.js";
		protected internal const string EditCssResourcePath = "DevExpress.Web.Css.Editors.";
		protected internal const string EditDefaultCssResourceName = EditCssResourcePath + "Default.css";
		protected internal const string EditSystemCssResourceName = EditCssResourcePath + "System.css";
		protected internal const string EditSpriteCssResourceName = EditCssResourcePath + "Sprite.css";
		protected const string Colon = ":";
		protected const string ExternalTableID = "ET";
		protected const string CaptionCellID = "CapC";
		protected const string ControlCellID = "CC";
		internal static readonly string[] ImportedStyleAttrKeys = new string[] {
				"position", "left", "top", "z-index",
				"margin", "margin-top", "margin-right", "margin-bottom", "margin-left",
				"float", "clear"
			};
		private EditPropertiesBase properties = null;
		private EditorInplaceMode inplaceMode = EditorInplaceMode.StandAlone;
		private bool inplaceAllowsEditorSizeRecalc;
		private object value = null;
		Table externalTable = null;
		TableCell captionCell = null;
		TableCell controlCell = null;
		TableCell fakeEmptyCell = null;
		WebControl captionElement = null;
		LiteralControl captionLiteralControl = null;
		protected TableCell CaptionCell {
			get { return this.captionCell; }
		}
		protected TableCell ControlCell {
			get { return this.controlCell; }
			set { this.controlCell = value; }
		}
		protected internal Table ExternalTable {
			get { return this.externalTable; }
			set { this.externalTable = value; }
		}
		protected internal string AccessibilityInputTitle { get; set; }
		public ASPxEditBase()
			: base() {
		}
		protected ASPxEditBase(ASPxWebControl ownerControl)
			: base(ownerControl) {
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxEditBaseCaption"),
#endif
		Localizable(true), DefaultValue(""), AutoFormatEnable]
		public string Caption {
			get { return Properties.Caption; }
			set { Properties.Caption = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxEditBaseCaptionCellStyle"),
#endif
		Category("Styles"), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public EditorCaptionCellStyle CaptionCellStyle {
			get { return Properties.CaptionCellStyle; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxEditBaseCaptionStyle"),
#endif
		Category("Styles"), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public EditorCaptionStyle CaptionStyle {
			get { return Properties.CaptionStyle; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxEditBaseRootStyle"),
#endif
		Category("Styles"), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public EditorRootStyle RootStyle {
			get { return Properties.RootStyle; }
		}
#if !SL
	[DevExpressWebLocalizedDescription("ASPxEditBaseControls")]
#endif
		public new ControlCollection Controls {
			get { return this.controlCell != null ? this.controlCell.Controls : base.Controls; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		AutoFormatDisable, Localizable(false)]
		public override string DataSourceID {
			get { return base.DataSourceID; }
			set { base.DataSourceID = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxEditBaseDataSource"),
#endif
		EditorBrowsable(EditorBrowsableState.Never), AutoFormatDisable]
		public override object DataSource {
			get { return base.DataSource; }
			set { base.DataSource = value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		AutoFormatDisable, Localizable(false)]
		public override string DataMember {
			get { return base.DataMember; }
			set { base.DataMember = value; }
		}
		[Browsable(false), DefaultValue(null), AutoFormatDisable, Bindable(true),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public virtual object Value {
			get { return IsValueStoreToViewState() ? GetObjectProperty("Value", null) : this.value; }
			set { 
				if(IsValueStoreToViewState())
					SetObjectProperty("Value", null, value);
				else
					this.value = value;
			}
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), AutoFormatDisable,
		Category("Behavior"), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override bool EncodeHtml {
			get { return Properties.EncodeHtml; }
			set { Properties.EncodeHtml = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxEditBaseCssPostfix"),
#endif
		Category("Styles"), DefaultValue("")]
		public override string CssPostfix {
			get { return Properties.CssPostfix; }
			set { Properties.CssPostfix = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxEditBaseCssFilePath"),
#endif
		Category("Styles"), DefaultValue(""), UrlProperty, AutoFormatUrlProperty,
		Editor(typeof(System.Web.UI.Design.UrlEditor), typeof(System.Drawing.Design.UITypeEditor))]
		public override string CssFilePath {
			get { return Properties.CssFilePath; }
			set { Properties.CssFilePath = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxEditBaseClientInstanceName"),
#endif
		Category("Client-Side"), DefaultValue(""), AutoFormatDisable, Localizable(false)]
		public string ClientInstanceName {
			get { return ClientInstanceNameInternal; }
			set { ClientInstanceNameInternal = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxEditBaseEnableClientSideAPI"),
#endif
		Category("Client-Side"), DefaultValue(false), AutoFormatDisable]
		public bool EnableClientSideAPI {
			get { return EnableClientSideAPIInternal; }
			set { EnableClientSideAPIInternal = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxEditBaseClientVisible"),
#endif
		Category("Client-Side"), DefaultValue(true), AutoFormatDisable, Localizable(false)]
		public bool ClientVisible {
			get { return base.ClientVisibleInternal; }
			set { base.ClientVisibleInternal = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxEditBaseClientEnabled"),
#endif
		Category("Client-Side"), DefaultValue(true), AutoFormatDisable, Localizable(false)]
		public bool ClientEnabled {
			get { return base.ClientEnabledInternal; }
			set { base.ClientEnabledInternal = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxEditBaseJSProperties"),
#endif
		Category("Client-Side"), Browsable(false), AutoFormatDisable,
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public Dictionary<string, object> JSProperties {
			get { return JSPropertiesInternal; }
		}
		[Category("Behavior"), Browsable(false), DefaultValue(false), AutoFormatDisable]
		public virtual bool ReadOnly {
			get { return GetBoolProperty("ReadOnly", false); }
			set { SetBoolProperty("ReadOnly", false, value); }
		}
		protected override bool Native {
			get { return Properties.Native; }
			set { Properties.Native = value; }
		}
		protected internal override ClientSideEventsBase ClientSideEventsInternal {
			get { return Properties.ClientSideEvents; }
		}
		protected override string ClientInstanceNameInternal {
			get { return Properties.ClientInstanceName; }
			set { Properties.ClientInstanceName = value; }
		}
		protected virtual bool ConvertEmptyStringToNull {
			get { return false; }
		}
		protected override bool IsDesignTimeDataBindingRequired() {
			return true;
		}
		protected override bool EnableClientSideAPIInternal {
			get { return Properties.EnableClientSideAPI; }
			set { Properties.EnableClientSideAPI = value; }
		}
		protected internal EditorInplaceMode InplaceMode {
			get { return inplaceMode; }
			set { inplaceMode = value; }
		}
		protected internal bool InplaceAllowsEditorSizeRecalc {
			get { return inplaceAllowsEditorSizeRecalc; }
			set { inplaceAllowsEditorSizeRecalc = value; }
		}
		protected internal EditPropertiesBase Properties {
			get {
				if(properties == null)
					properties = CreateProperties();
				return properties;
			}
		}
		protected override ImagesBase ImagesInternal {
			get { return Properties.Images; }
		}
		protected override ImagesBase RenderImagesInternal {
			get { return Properties.RenderImages; }
		}
		protected internal EditorImages RenderImages {
			get { return (EditorImages)RenderImagesInternal; }
		}
		protected override StylesBase StylesInternal {
			get { return Properties.Styles; }
		}
		protected override StylesBase RenderStylesInternal {
			get { return Properties.RenderStyles; }
		}
		protected internal EditorStyles RenderStyles {
			get { return (EditorStyles)RenderStylesInternal; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxEditBaseParentSkinOwner"),
#endif
		Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override ISkinOwner ParentSkinOwner {
			get { return Properties.ParentSkinOwner; }
			set { Properties.ParentSkinOwner = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxEditBaseParentImages"),
#endif
		Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override ImagesBase ParentImages {
			get { return Properties.ParentImages; }
			set { Properties.ParentImages = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxEditBaseParentStyles"),
#endif
		Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override StylesBase ParentStyles {
			get { return Properties.ParentStyles; }
			set { Properties.ParentStyles = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxEditBaseCustomJSProperties"),
#endif
		Category("Client-Side")]
		public event CustomJSPropertiesEventHandler CustomJSProperties
		{
			add { Events.AddHandler(EventCustomJsProperties, value); }
			remove { Events.RemoveHandler(EventCustomJsProperties, value); }
		}
		protected EditorCaptionSettingsBase CaptionSettingsInternal {
			get { return Properties.CaptionSettingsInternal; }
		}
		protected override void GetCreateClientObjectScript(StringBuilder stb, string localVarName, string clientName) {
			base.GetCreateClientObjectScript(stb, localVarName, clientName);
			if(CaptionCell != null && CaptionSettingsInternal.GetPosition() != EditorCaptionPosition.Left)
				stb.AppendFormat("{0}.captionPosition = {1};\n", localVarName, HtmlConvertor.ToScript(CaptionSettingsInternal.GetPosition()));
			if(!CaptionSettingsInternal.ShowColon)
				stb.AppendFormat("{0}.showCaptionColon = false;\n", localVarName);
		}
		protected virtual bool IsCaptionCellRequired() {
			return !string.IsNullOrEmpty(Caption);
		}
		protected void CreateCaptionCell() {
			this.captionCell = RenderUtils.CreateTableCell();
			this.captionCell.ID = CaptionCellID;
		}
		protected void CreateExternalTable() {
			this.externalTable = RenderUtils.CreateTable();
			this.externalTable.ID = ExternalTableID;
			TableRow row = RenderUtils.CreateTableRow();
			this.externalTable.Rows.Add(row);
			this.controlCell = RenderUtils.CreateTableCell();
			this.controlCell.ID = ControlCellID;
			row.Cells.Add(this.controlCell);
			CreateCaptionCell();
			EditorCaptionPosition captionPosition = CaptionSettingsInternal.GetPosition();
			if (captionPosition == EditorCaptionPosition.Left || captionPosition == EditorCaptionPosition.Right) {
				int captionIndex = captionPosition == EditorCaptionPosition.Left ? 0 : 1;
				row.Cells.AddAt(captionIndex, this.captionCell);
			}
			else {
				TableRow secondRow = RenderUtils.CreateTableRow();
				secondRow.Cells.Add(this.captionCell);
				int rowIndex = captionPosition == EditorCaptionPosition.Top ? 0 : 1;
				this.externalTable.Rows.AddAt(rowIndex, secondRow);
			}
		}
		private HtmlTextWriterTag GetCaptionTag() {
			return string.IsNullOrEmpty(GetAssociatedControlID()) ? HtmlTextWriterTag.Span : HtmlTextWriterTag.Label;
		}
		protected virtual void CreateCaptionCellContent() {
			if (!string.IsNullOrEmpty(Caption)) {
				this.captionElement = RenderUtils.CreateWebControl(GetCaptionTag());
				this.captionLiteralControl = new LiteralControl();
				this.captionElement.Controls.Add(this.captionLiteralControl);
				this.captionCell.Controls.Add(this.captionElement);
			}
		}
		protected override void ClearControlFields() {
			this.externalTable = null;
			this.captionCell = null;
			this.controlCell = null;
			this.captionElement = null;
			this.captionLiteralControl = null;
			this.fakeEmptyCell = null;
		}
		protected override void CreateControlHierarchy() {
			base.CreateControlHierarchy();
			if (IsCaptionCellRequired()) {
				if (this.externalTable == null) {
					CreateExternalTable();
					ControlsBase.Add(this.externalTable);
				}
				else
					AddCaptionCellToExternalTable();
				CreateCaptionCellContent();
			}
		}
		protected void AddCaptionCellToExternalTable() {
			CreateCaptionCell();
			EditorCaptionPosition captionPosition = CaptionSettingsInternal.GetPosition();
			TableRow editorRow = GetRowWithEditor();
			if (captionPosition == EditorCaptionPosition.Left || captionPosition == EditorCaptionPosition.Right) {
				int captionCellIndex = captionPosition == EditorCaptionPosition.Left ? 0 : editorRow.Cells.Count;
				editorRow.Cells.AddAt(captionCellIndex, this.captionCell);
				if (this.externalTable.Rows.Count > 1) {
					this.fakeEmptyCell = RenderUtils.CreateTableCell();
					int emptyCellParentRowIndex = this.externalTable.Rows.GetRowIndex(editorRow) == 0 ? 1 : 0;
					this.externalTable.Rows[emptyCellParentRowIndex].Cells.AddAt(captionCellIndex, this.fakeEmptyCell);
				}
			}
			else {
				TableRow newRow = RenderUtils.CreateTableRow();
				newRow.Cells.Add(this.captionCell);
				if (editorRow.Cells.Count > 1) {
					this.fakeEmptyCell = RenderUtils.CreateTableCell();
					newRow.Cells.AddAt(editorRow.Cells.GetCellIndex(this.controlCell) == 0 ? 1 : 0, this.fakeEmptyCell);
				}
				int newRowIndex = captionPosition == EditorCaptionPosition.Top ? 0 : this.externalTable.Rows.Count;
				this.externalTable.Rows.AddAt(newRowIndex, newRow);
			}
		}
		protected TableRow GetRowWithEditor() {
			foreach (TableRow row in this.externalTable.Rows) {
				foreach (TableCell cell in row.Cells)
					if (cell == this.controlCell)
						return row;
			}
			return null;
		}
		protected override void PrepareControlHierarchy() {
			base.PrepareControlHierarchy();
			if (this.captionLiteralControl != null)
				this.captionLiteralControl.Text = HtmlEncode(GetActualCaption());		   
			if (this.captionCell != null)
				PrepareCaptionCell();
			if (this.captionElement != null)
				PrepareCaptionElement();
			if (this.controlCell != null)
				PrepareControlCell();
			if (this.externalTable != null)
				PrepareExternalTable();
			if (this.fakeEmptyCell != null)
				this.fakeEmptyCell.CssClass = RenderUtils.CombineCssClasses(this.fakeEmptyCell.CssClass, "dxeFakeEmptyCell");
		}
		protected internal bool HasPercentageWidth() {
			return !Width.IsEmpty && Width.Type == UnitType.Percentage
				|| Style["width"] != null && Style["width"].EndsWith("%");
		}
		protected Unit GetEditorPercentageWidth() {
			if(!Width.IsEmpty && Width.Type == UnitType.Percentage)
				return Width;
			if(Style["width"] != null && Style["width"].EndsWith("%")) {
				string widthString = Style["width"].Replace("%", "");
				double width = 0;
				if(double.TryParse(widthString, out width))
					return Unit.Percentage(width);
			}  
			return Unit.Empty;
		}
		protected void PrepareCaptionCell() {
			AppearanceStyle captionCellStyle = GetCaptionCellStyle();
			captionCellStyle.AssignToControl(this.captionCell);
			captionCellStyle.Paddings.AssignToControl(this.captionCell);
			if (!captionCellStyle.Width.IsEmpty) {
				this.captionCell.Width = captionCellStyle.Width;
				if (HasPercentageWidth())
					RenderUtils.SetStyleAttribute(this.captionCell, "min-width", captionCellStyle.Width, null);
			}
			if (!captionCellStyle.Height.IsEmpty)
				this.captionCell.Height = captionCellStyle.Height;
		}
		protected void PrepareCaptionElement() {
			AppearanceStyleBase captionStyle = GetCaptionStyle();
			captionStyle.AssignToControl(this.captionElement);
			RenderUtils.SetStringAttribute(this.captionElement, "for", GetAssociatedControlID());
		}
		protected void PrepareControlCell() {
			if (HasPercentageWidth())
				this.controlCell.Width = Unit.Percentage(100);
		}
		protected void PrepareExternalTable() {
			GetRootStyle().AssignToControl(this.externalTable);
			GetRootStyle().Paddings.AssignToControl(this.externalTable);
			if (!IsClientVisible())
				RenderUtils.SetStyleAttribute(this.externalTable, "display", "none", "");
			if (IsRightToLeft())
				this.externalTable.Attributes["dir"] = "rtl";
			if (HasPercentageWidth())
				this.externalTable.Width = GetEditorPercentageWidth();
			if (Height.Type == UnitType.Percentage)
				this.externalTable.Height = Height;
			for (int i = 0; i < ImportedStyleAttrKeys.Length; i++) {
				string key = ImportedStyleAttrKeys[i];
				string attrValue = Style[key];
				if (!string.IsNullOrEmpty(attrValue))
					this.externalTable.Style[key] = attrValue;
			}
		}
		internal void RemoveImportedStyleAttrsFromMainElement(WebControl mainElement) {
			if (this.externalTable == null)
				return;
			for (int i = 0; i < ImportedStyleAttrKeys.Length; i++) {
				string key = ImportedStyleAttrKeys[i];
				if (!string.IsNullOrEmpty(mainElement.Style[key]))
					mainElement.Style.Remove(key);
			}
		}
		protected internal virtual string GetAssociatedControlID() {
			return string.Empty;
		}
		protected string GetActualCaption() {
			string trimmedCaption = Caption.Trim();
			if (!CaptionSettingsInternal.ShowColon || string.IsNullOrEmpty(trimmedCaption)
				|| trimmedCaption.Equals("&nbsp;", StringComparison.OrdinalIgnoreCase) || Caption.EndsWith(Colon))
				return Caption;
			else
				return Caption + Colon;
		}
		protected abstract EditPropertiesBase CreateProperties();
		protected internal void SetOwnerControl(ASPxWebControl control) {
			OwnerControl = control;
		}
		protected internal virtual void ValidateProperties() {
		}
		protected virtual string GetFocusableControlID() {
			return ClientID;
		}
		protected override void EnsurePreRender() {
			if(!DesignMode)
				ValidateProperties();
			base.EnsurePreRender();
		}
		protected override Style CreateControlStyle() {
			return new AppearanceStyle();
		}
		protected override StylesBase CreateStyles() {
			return null;
		}
		protected override void RegisterDefaultRenderCssFile() {
			ResourceManager.RegisterCssResource(Page, typeof(ASPxEditBase), EditDefaultCssResourceName);
		}
		protected override void RegisterSystemCssFile() {
			base.RegisterSystemCssFile();
			ResourceManager.RegisterCssResource(Page, typeof(ASPxEditBase), EditSystemCssResourceName);
		}
		protected override void PrepareControlStyle(AppearanceStyleBase style) {
			MergeParentSkinOwnerControlStyle(style);
			style.CopyFrom(ControlStyle);
		}
		protected virtual internal Unit GetHeight() {
			AppearanceStyleBase style = GetControlStyle();
			if(!style.Height.IsEmpty)
				return UnitUtils.GetCorrectedHeight(style.Height, style, ((AppearanceStyle)style).Paddings);
			return Unit.Empty;
		}
		protected virtual internal Unit GetWidth() {
			AppearanceStyleBase style = GetControlStyle();
			if(!style.Width.IsEmpty)
				return UnitUtils.GetCorrectedWidth(style.Width, style, ((AppearanceStyle)style).Paddings);
			return Unit.Empty;
		}
		protected internal DisabledStyle GetDisabledCssStyle() {
			DisabledStyle style = new DisabledStyle();
			style.CopyFrom(GetDisabledStyle());
			return style;
		}
		public EditClientSideEventsBase GetClientSideEvents() {
			return Properties.ClientSideEvents;
		}
		protected override string GetClientObjectClassName() {
			return "ASPxClientEditBase";
		}
		protected override void AddDisabledItems(StateScriptRenderHelper helper) {
			helper.AddStyle(GetDisabledCssStyle(), "", IsEnabled());
		}
		protected sealed override void RegisterIncludeScripts() {
			base.RegisterIncludeScripts();
			RegisterEditorIncludeScripts();
		}
		public virtual void RegisterEditorIncludeScripts() {
			RegisterIncludeScript(typeof(ASPxEditBase), EditScriptResourceName);
		}
		protected override bool IsClientSideEventsStoreToViewState() {
			return false;
		}
		protected override bool IsImagesStoreToViewState() {
			return false;
		}
		protected override bool IsStylesStoreToViewState() {
			return false;
		}
		protected virtual bool IsValueStoreToViewState() {
			return true;
		}
		protected override IStateManager[] GetStateManagedObjects() {
			return ViewStateUtils.GetMergedStateManagedObjects(base.GetStateManagedObjects(),
				new IStateManager[] { Properties });
		}
		protected internal AppearanceStyle GetCaptionCellStyle() {
			AppearanceStyle style = new AppearanceStyle();
			style.CopyFrom(RenderStyles.GetDefaultCaptionCellStyle());
			MergeControlStyle(style, false);
			style.CopyFrom(RenderStyles.CaptionCell);
			List<string> cssClasses = new List<string>(RenderStyles.GetCaptionAlignmentClassNames(
				CaptionSettingsInternal.GetHorizontalAlign(), CaptionSettingsInternal.GetVerticalAlign())
			);
			cssClasses.Add(CssClassNameBuilder.GetCssClassNameByControl(this, RenderStyles.GetControlTypeSystemClassNameTemplate()));
			cssClasses.Add(RenderStyles.GetCaptionPositionSystemClassName(CaptionSettingsInternal.GetPosition()));
			if (RenderUtils.Browser.IsSafari)
				cssClasses.Add(RenderStyles.GetCaptionCellSafariSystemClassName());
			cssClasses.Add(style.CssClass);
			style.CssClass = RenderUtils.CombineCssClasses(cssClasses.ToArray());
			return style;
		}
		protected internal AppearanceStyleBase GetCaptionStyle() {
			AppearanceStyleBase style = new AppearanceStyleBase();
			style.CopyFrom(RenderStyles.GetDefaultCaptionStyle());
			MergeControlStyle(style, false);
			style.CopyFrom(RenderStyles.Caption);
			return style;
		}
		protected internal AppearanceStyle GetRootStyle() {
			AppearanceStyle style = new AppearanceStyle();
			style.CopyFrom(RenderStyles.GetDefaultRootStyle());
			MergeControlStyle(style, false);
			style.CopyFrom(RenderStyles.Root);
			return style;
		}
		protected override bool CanLoadPostDataOnLoad() {
			return false;
		}
		protected virtual object GetPostBackValue(string controlName, NameValueCollection postCollection) {
			return postCollection[controlName];
		}
		protected virtual bool IsPostBackValueSecure(object value) {
			return true;
		}
		protected override bool LoadPostData(NameValueCollection postCollection) {
			if(IsEnabled()) {
				object oldValue = Value;
				object newValue = GetPostBackValue(UniqueID, postCollection); 
				if(IsPostBackValueSecure(newValue)) {
					Value = newValue;
					return !AreEditorValuesEqual(oldValue, Value, ConvertEmptyStringToNull);
				}
			}
			return false;
		}
		protected virtual bool AreEditorValuesEqual(object v1, object v2, bool convertEmptyStringToNull) {
			return CommonUtils.AreEqual(v1, v2, ConvertEmptyStringToNull);
		}
		protected override string GetSkinControlName() {
			return ((ISkinOwner)Properties).GetControlName();
		}
		protected override string[] GetChildControlNames() {
			return ((ISkinOwner)Properties).GetChildControlNames();
		}
		protected override string GetCssFilePath() {
			return ((ISkinOwner)Properties).GetCssFilePath();
		}
		protected override string GetImageFolder() {
			return ((ISkinOwner)Properties).GetImageFolder();
		}
		protected override string GetCssPostFix() {
			return ((ISkinOwner)Properties).GetCssPostFix();
		}
		protected override bool IsDefaultAppearanceEnabled() {
			return ((ISkinOwner)Properties).IsDefaultAppearanceEnabled();
		}
		protected override void MergeControlStyle(AppearanceStyleBase style) {
			base.MergeControlStyle(style);
			((ISkinOwner)Properties).MergeControlStyle(style);
		}
		protected override bool IsNative() {
			return ((ISkinOwner)Properties).IsNative();
		}
		protected override bool IsNativeSupported() {
			return ((ISkinOwner)Properties).IsNativeSupported();
		}
		protected override bool IsRightToLeft() {
			return ((ISkinOwner)Properties).IsRightToLeft();
		}
		protected override void PropertiesChanged(PropertiesBase properties) {
			if(properties is EditPropertiesBase && this.properties == null)
				return;
			base.PropertiesChanged(properties);
		}
		protected ControlCollection ControlsBase {
			get { return base.Controls; }
		}
	}
	public abstract class EditProperties : EditPropertiesBase {
		private ValidationSettings validationSettings = null;
		public EditProperties()
			: this(null) {
		}
		public EditProperties(IPropertiesOwner owner)
			: base(owner) {
			if(this is BinaryImageEditProperties)
				this.validationSettings = ValidationSettings.CreateBinaryImageValdationSettings(owner);
			else
				this.validationSettings = ValidationSettings.CreateValidationSettings(owner);
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("EditPropertiesConvertEmptyStringToNull"),
#endif
		DefaultValue(true), NotifyParentProperty(true), AutoFormatDisable]
		public bool ConvertEmptyStringToNull {
			get { return GetBoolProperty("ConvertEmptyStringToNull", true); }
			set { SetBoolProperty("ConvertEmptyStringToNull", true, value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("EditPropertiesReadOnlyStyle"),
#endif
		Category("Styles"), NotifyParentProperty(true), PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable,
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public ReadOnlyStyle ReadOnlyStyle {
			get { return Styles.ReadOnly; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("EditPropertiesFocusedStyle"),
#endif
		Category("Styles"), NotifyParentProperty(true), PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable,
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual EditorDecorationStyle FocusedStyle {
			get { return Styles.Focused; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("EditPropertiesInvalidStyle"),
#endif
		Category("Styles"), NotifyParentProperty(true), PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable,
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public EditorDecorationStyle InvalidStyle {
			get { return Styles.Invalid; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("EditPropertiesEnableFocusedStyle"),
#endif
		Category("Behavior"), NotifyParentProperty(true), AutoFormatDisable, DefaultValue(true)]
		public virtual bool EnableFocusedStyle {
			get { return Styles.EnableFocusedStyle; }
			set { Styles.EnableFocusedStyle = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("EditPropertiesValidationSettings"),
#endif
		Category("Validation"), NotifyParentProperty(true), AutoFormatEnable,
		PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public ValidationSettings ValidationSettings {
			get { return validationSettings; }
		}
		[
		AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true)]
		protected internal EditorCaptionSettings CaptionSettings {
			get { return base.CaptionSettingsInternal as EditorCaptionSettings; }
		}
		internal override int EditingRowVisibleIndex {
			get { return ValidationSettings.EditingRowVisibleIndex; }
			set { ValidationSettings.EditingRowVisibleIndex = value; }
		}
		public override void Assign(PropertiesBase source) {
			BeginUpdate();
			try {
				base.Assign(source);
				EditProperties src = source as EditProperties;
				if(src != null) {
					ValidationSettings.Assign(src.ValidationSettings);
					ConvertEmptyStringToNull = src.ConvertEmptyStringToNull;
				}
			}
			finally {
				EndUpdate();
			}
		}
		protected override void PrepareNotNullEditValue(ref object editValue) {
			if(ConvertEmptyStringToNull && (editValue is string) && ((string)editValue == ""))
				editValue = null;
		}
		protected override EditorCaptionSettingsBase CreateCaptionSettings() {
			return new EditorCaptionSettings(this);
		}
		protected override EditClientSideEventsBase CreateClientSideEvents() {
			return new EditClientSideEvents(this);
		}
		protected override IStateManager[] GetStateManagedObjects() {
			return ViewStateUtils.GetMergedStateManagedObjects(base.GetStateManagedObjects(),
				new IStateManager[] { ValidationSettings });	
		}
	}
	[Designer("DevExpress.Web.Design.ASPxEditDesigner, " + AssemblyInfo.SRAssemblyWebDesignFull)
]
	public abstract class ASPxEdit : ASPxEditBase, IAssociatedControlID, IValidationSummaryEditor {
		#region Nested Types
		private class EditorProcessingContext {
			private ASPxEdit firstInvalidEditor;
			private ASPxEdit firstVisibleInvalidEditor;
			public ASPxEdit FirstInvalidEditor {
				get { return firstInvalidEditor; }
				set {
					if(firstInvalidEditor != null)
						throw new InvalidOperationException("First invalid editor must be assigned only once.");
					firstInvalidEditor = value;
				}
			}
			public ASPxEdit FirstVisibleInvalidEditor {
				get { return firstVisibleInvalidEditor; }
				set {
					if(firstVisibleInvalidEditor != null)
						throw new InvalidOperationException("First visible invalid editor must be assigned only once.");
					firstVisibleInvalidEditor = value;
				}
			}
		}
		internal class ErrorFramePreparer {
			#region Nested Types
			private static class Utils {
				private const string NoBorderLeftStyleName = "dxeNoBorderLeft";
				private const string NoBorderTopStyleName = "dxeNoBorderTop";
				private const string NoBorderRightStyleName = "dxeNoBorderRight";
				private const string NoBorderBottomStyleName = "dxeNoBorderBottom";
				private static bool IsHorizontalBordersEmpty(AppearanceStyleBase style) {
					return style.BorderLeft.BorderWidth.IsEmpty && style.BorderRight.BorderWidth.IsEmpty;
				}
				private static bool IsVerticalBordersEmpty(AppearanceStyleBase style) {
					return style.BorderTop.BorderWidth.IsEmpty && style.BorderBottom.BorderWidth.IsEmpty;
				}
				private static void RemoveRedundantBordersFromCellStyles(ErrorTextPosition errorPosition, ErrorFrameStyle errorFrameStyle, ErrorFrameStyle controlCellStyle, ErrorFrameStyle errorCellStyle) {
					BorderWrapper borders = errorFrameStyle.Border;
					if (!borders.BorderWidth.IsEmpty || !IsHorizontalBordersEmpty(errorFrameStyle) || !IsVerticalBordersEmpty(errorFrameStyle)) {
						if (errorPosition == ErrorTextPosition.Left && (!borders.BorderWidth.IsEmpty || !IsHorizontalBordersEmpty(errorFrameStyle))) {
							controlCellStyle.BorderLeft.BorderWidth = 0;
							errorCellStyle.BorderRight.BorderWidth = 0;
						}
						if (errorPosition == ErrorTextPosition.Top && (!borders.BorderWidth.IsEmpty || !IsVerticalBordersEmpty(errorFrameStyle))) {
							controlCellStyle.BorderTop.BorderWidth = 0;
							errorCellStyle.BorderBottom.BorderWidth = 0;
						}
						if (errorPosition == ErrorTextPosition.Right && (!borders.BorderWidth.IsEmpty || !IsHorizontalBordersEmpty(errorFrameStyle))) {
							controlCellStyle.BorderRight.BorderWidth = 0;
							errorCellStyle.BorderLeft.BorderWidth = 0;
						}
						if (errorPosition == ErrorTextPosition.Bottom && (!borders.BorderWidth.IsEmpty || !IsVerticalBordersEmpty(errorFrameStyle))) {
							controlCellStyle.BorderBottom.BorderWidth = 0;
							errorCellStyle.BorderTop.BorderWidth = 0;
						}
					}
					else {
						if (errorPosition == ErrorTextPosition.Left) {
							controlCellStyle.CssClass = RenderUtils.CombineCssClasses(controlCellStyle.CssClass, NoBorderLeftStyleName);
							errorCellStyle.CssClass = RenderUtils.CombineCssClasses(errorCellStyle.CssClass, NoBorderRightStyleName);
						}
						if (errorPosition == ErrorTextPosition.Top) {
							controlCellStyle.CssClass = RenderUtils.CombineCssClasses(controlCellStyle.CssClass, NoBorderTopStyleName);
							errorCellStyle.CssClass = RenderUtils.CombineCssClasses(errorCellStyle.CssClass, NoBorderBottomStyleName);
						}
						if (errorPosition == ErrorTextPosition.Right) {
							controlCellStyle.CssClass = RenderUtils.CombineCssClasses(controlCellStyle.CssClass, NoBorderRightStyleName);
							errorCellStyle.CssClass = RenderUtils.CombineCssClasses(errorCellStyle.CssClass, NoBorderLeftStyleName);
						}
						if (errorPosition == ErrorTextPosition.Bottom) {
							controlCellStyle.CssClass = RenderUtils.CombineCssClasses(controlCellStyle.CssClass, NoBorderBottomStyleName);
							errorCellStyle.CssClass = RenderUtils.CombineCssClasses(errorCellStyle.CssClass, NoBorderTopStyleName);
						}
					}
				}
				public static void PrepareErrorFrame(ASPxEdit edit, WebControl controlCell) {
					ErrorFrameStyle errorFrameStyle = edit.GetErrorFrameStyle();
					ErrorFrameStyle controlCellStyle = new ErrorFrameStyle();
					controlCellStyle.CopyFrom(errorFrameStyle);
					if (edit.errorCell != null) {
						ErrorFrameStyle errorCellStyle = edit.GetErrorCellStyle();
						ErrorTextPosition errorPosition = edit.ValidationSettings.ErrorTextPosition;
						if ((errorPosition == ErrorTextPosition.Left || errorPosition == ErrorTextPosition.Right) && edit.IsRightToLeft()) {
							errorPosition = errorPosition == ErrorTextPosition.Left ? ErrorTextPosition.Right : ErrorTextPosition.Left;
						}
						RemoveRedundantBordersFromCellStyles(errorPosition, errorFrameStyle, controlCellStyle, errorCellStyle);
						errorCellStyle.AssignToControl(edit.errorCell, true);
					}
					controlCellStyle.CopyFrom(edit.GetControlCellStyle());
					controlCellStyle.AssignToControl(controlCell, true);
					ApplyVerticalAlignToCell(errorFrameStyle.VerticalAlign, controlCell, edit.ValidationSettings.ErrorTextPosition);
					if (edit.HasPercentageWidth())
						controlCell.Width = Unit.Percentage(100);
				}
				public static void PrepareErrorCell(ASPxEdit edit, TableCell errorCell, TableCell errorTextCell, TableCell errorImageCell, Image errorImage) {
					ErrorFrameStyle errorFrameStyle = edit.GetErrorFrameStyle();
					Unit errorImageSpacing = edit.GetErrorImageSpacing();
					if(errorTextCell != null)
						ApplyWrapToTextCell(errorTextCell, errorFrameStyle);
					else
						ApplyWrapToTextCell(errorCell, errorFrameStyle);
					edit.GetErrorCellPaddings().AssignToControl(errorCell);
					errorFrameStyle.AssignToControl(errorCell, AttributesRange.Font | AttributesRange.Cell);
					if(errorImageCell != null && !errorImageSpacing.IsEmpty)
						RenderUtils.SetStyleAttribute(errorImageCell, (edit as ISkinOwner).IsRightToLeft() ? "padding-left" : "padding-right", errorImageSpacing.ToString(), "");
					if(errorImage != null) {
						bool isResourcePng;
						ImageProperties errorImageProperties = edit.GetErrorImage(out isResourcePng);
						errorImageProperties.AssignToControl(errorImage, edit.DesignMode, isResourcePng);
						if(edit.ValidationSettings.IsImageMode && !edit.ValidationSettings.IsTextMode) {
							errorImage.AlternateText = edit.ErrorText;
							errorImage.ToolTip = edit.ErrorText;
						}
					}
					if(errorTextCell != null) {
						errorFrameStyle.AssignToControl(errorTextCell, AttributesRange.Font);
						errorTextCell.Width = Unit.Percentage(100); 
					}
					ApplyVerticalAlignToCell(errorFrameStyle.VerticalAlign, errorCell, edit.ValidationSettings.ErrorTextPosition);
				}
				public static void SetErrorCellVisibility(ASPxEdit edit, TableCell errorCell) {
					if(edit.IsValid || !edit.ClientEnabled)
						RenderUtils.SetStyleAttribute(errorCell, "display", "none", string.Empty);
				}
				public static bool ErrorFrameVisible(ASPxEdit edit) {
					return !edit.IsValid && edit.ClientEnabled;
				}
				private static void ApplyWrapToTextCell(TableCell textCell, AppearanceStyleBase errorFrameStyle) {
					if(errorFrameStyle.Wrap == DefaultBoolean.False || errorFrameStyle.Wrap == DefaultBoolean.Default)
						textCell.Style.Add(HtmlTextWriterStyle.WhiteSpace, "nowrap");
				}
				private static void ApplyVerticalAlignToCell(VerticalAlign verticalAlign, WebControl cell, ErrorTextPosition errorTextPosition) {
					TableCell tableCell = cell as TableCell;
					if(tableCell == null || tableCell.VerticalAlign != VerticalAlign.NotSet)
						return;
					if(errorTextPosition == ErrorTextPosition.Left ||
						errorTextPosition == ErrorTextPosition.Right) {
						VerticalAlign vAlign = verticalAlign != VerticalAlign.NotSet ?
							verticalAlign : VerticalAlign.Middle;
						RenderUtils.SetVerticalAlign(tableCell, vAlign);
					}
				}
			}
			private abstract class BaseState {
				private ASPxEdit edit;
				public BaseState(ASPxEdit edit) {
					this.edit = edit;
				}
				protected ASPxEdit Edit {
					get { return edit; }
				}
				public abstract void PrepareErrorFrame();
				public abstract void PrepareErrorCell(TableCell errorCell, TableCell errorTextCell, TableCell errorImageCell, Image errorImage);
				public virtual void InitControlCellStyles(StringBuilder clientObjInitScript, string localVarName) {
				}
			}
			private class StaticDisplayState : BaseState {
				private const string ValidStaticTableClassName = "dxeValidStEditorTable";
				public StaticDisplayState(ASPxEdit edit)
					: base(edit) {
				}
				public override void PrepareErrorFrame() {
					Utils.PrepareErrorFrame(Edit, Edit.ControlCell);
					if (!Utils.ErrorFrameVisible(Edit))
						Edit.ExternalTable.CssClass = RenderUtils.CombineCssClasses(Edit.ExternalTable.CssClass, ValidStaticTableClassName);
				}
				public override void PrepareErrorCell(TableCell errorCell, TableCell errorTextCell, TableCell errorImageCell, Image errorImage) {
					Utils.PrepareErrorCell(Edit, errorCell, errorTextCell, errorImageCell, errorImage);
					if(!Utils.ErrorFrameVisible(Edit))
						RenderUtils.SetStyleAttribute(errorCell, "visibility", "hidden", "");
				}
			}
			private class DynamicDisplayState : BaseState {
				private const string ValidDynamicTableClassName = "dxeValidDynEditorTable";
				private WebControl controlCellStyleCollector;
				public DynamicDisplayState(ASPxEdit edit)
					: base(edit) {
				}
				private WebControl ControlCellStyleCollector {
					get {
						if(controlCellStyleCollector == null)
							controlCellStyleCollector = new WebControl(HtmlTextWriterTag.Unknown);
						return controlCellStyleCollector;
					}
				}
				public override void PrepareErrorFrame() {
					if(Utils.ErrorFrameVisible(Edit))
						Utils.PrepareErrorFrame(Edit, Edit.ControlCell);
					else {
						Utils.PrepareErrorFrame(Edit, ControlCellStyleCollector);
						Edit.ExternalTable.CssClass = RenderUtils.CombineCssClasses(Edit.ExternalTable.CssClass, ValidDynamicTableClassName);
					}
				}
				public override void PrepareErrorCell(TableCell errorCell, TableCell errorTextCell, TableCell errorImageCell, Image errorImage) {
					Utils.PrepareErrorCell(Edit, errorCell, errorTextCell, errorImageCell, errorImage);
					Utils.SetErrorCellVisibility(Edit, errorCell);
				}
				public override void InitControlCellStyles(StringBuilder clientObjInitScript, string localVarName) {
					if(!Utils.ErrorFrameVisible(Edit)) {
						clientObjInitScript.AppendFormat("{0}.controlCellStyles = {{ cssClass: '{1}', style: \"{2}\" }};\n",
							localVarName, ControlCellStyleCollector.CssClass, GetStyleAttributeValue(ControlCellStyleCollector));
					}
				}
				private static string GetStyleAttributeValue(WebControl control) {
					return control.ControlStyle.GetStyleAttributes(control.Page).Value + control.Style.Value;
				}
			}
			#endregion
			private ASPxEdit edit;
			private BaseState staticDisplayState;
			private BaseState dynamicDisplayState;
			public ErrorFramePreparer(ASPxEdit edit) {
				this.edit = edit;
			}
			protected ASPxEdit Edit {
				get { return edit; }
			}
			private BaseState CurrentState {
				get {
					if(Edit.DesignMode || Edit.ValidationSettings.Display == Display.Static) {
						if(staticDisplayState == null)
							staticDisplayState = new StaticDisplayState(edit);
						return staticDisplayState;
					} else if(Edit.ValidationSettings.Display == Display.Dynamic) {
						if(dynamicDisplayState == null)
							dynamicDisplayState = new DynamicDisplayState(edit);
						return dynamicDisplayState;
					} else
						throw new InvalidOperationException("Unexpected display state.");
				}
			}
			public void InitControlCellStyles(StringBuilder clientObjInitScript, string localVarName) {
				CurrentState.InitControlCellStyles(clientObjInitScript, localVarName);
			}
			public void PrepareErrorFrame() {
				if(!Edit.DesignMode || Edit.ShowErrorFrame)
					CurrentState.PrepareErrorFrame();
			}
			public void PrepareErrorCell(TableCell errorCell, TableCell errorTextCell, TableCell errorImageCell, Image errorImage) {
				if((!Edit.DesignMode || Edit.ShowErrorFrame) && errorCell != null)
					CurrentState.PrepareErrorCell(errorCell, errorTextCell, errorImageCell, errorImage);
			}
		}
		#endregion
		private const string ErrorCellID = "EC";
		private const string ErrorTextCellID = "ETC";
		private const string ErrorImageID = "EI";
		protected internal const string ClientObjectStateInputIDSuffix = "$State";
		protected internal const string ValidationStateKey = "validationState";
		protected internal const string ValueChangedHandlerName = "ASPx.EValueChanged('{0}')";
		protected internal const string GotFocusHandlerName = "ASPx.EGotFocus('{0}')";
		protected internal const string LostFocusHandlerName = "ASPx.ELostFocus('{0}')";
		private bool showErrorFrame;
		private ErrorFramePreparer errorFramePreparer;
		private TableCell errorCell;
		private TableCell errorImageCell;
		private TableCell errorTextCell;
		private Image errorImage;
		private WebControl requiredMarkControl;
		private WebControl optionalMarkControl;
		private LiteralControl requiredMarkLiteralControl;
		private LiteralControl optionalMarkLiteralControl;
		private static readonly object EventValidation = new object();
		private static readonly object EventValueChanged = new object();
		public ASPxEdit()
			: base() {
			this.errorFramePreparer = new ErrorFramePreparer(this);
		}
		protected ASPxEdit(ASPxWebControl ownerControl)
			: base(ownerControl) {
			this.errorFramePreparer = new ErrorFramePreparer(this);
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxEditAutoPostBack"),
#endif
		Category("Behavior"), Browsable(true), DefaultValue(false), AutoFormatDisable]
		public bool AutoPostBack {
			get { return base.AutoPostBackInternal; }
			set { base.AutoPostBackInternal = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxEditCaptionSettings"),
#endif
		AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public EditorCaptionSettings CaptionSettings {
			get { return Properties.CaptionSettings; }
		}
		[Browsable(false), AutoFormatDisable, Localizable(true),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public string ErrorText {
			get { return ValidationSettings.ErrorText; }
			set {
				ValidationSettings.ErrorText = value;
				LayoutChanged();
				if(!LocalValidationSummaryUpdateRequestsLocked)
					ValidationSummaryCollection.Instance.OnEditorIsValidStateChanged(this);
			}
		}
		[Browsable(false), Bindable(true), AutoFormatDisable,
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public virtual bool IsValid {
			get {
				return HttpUtils.GetContextValue<bool>(IsValidContextValueKey, true) && !ShowErrorFrame;
			}
			set {
				HttpUtils.SetContextValue<bool>(IsValidContextValueKey, value);
				LayoutChanged();
				if(!LocalValidationSummaryUpdateRequestsLocked)
					ValidationSummaryCollection.Instance.OnEditorIsValidStateChanged(this);
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxEditReadOnly"),
#endif
		Category("Behavior"), Browsable(true), Bindable(true), AutoFormatDisable]
		public override bool ReadOnly {
			get { return base.ReadOnly; }
			set { base.ReadOnly = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxEditEnableFocusedStyle"),
#endif
		Category("Behavior"), AutoFormatDisable, DefaultValue(true)]
		public virtual bool EnableFocusedStyle {
			get { return Properties.EnableFocusedStyle; }
			set { Properties.EnableFocusedStyle = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxEditValidationSettings"),
#endif
		Category("Validation"), PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable,
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public ValidationSettings ValidationSettings {
			get { return Properties.ValidationSettings; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxEditVisible"),
#endif
 AutoFormatDisable]
		public override bool Visible {
			get { return base.Visible; }
			set {
				bool changingVisible = value != base.Visible;
				base.Visible = value;
				if(changingVisible) {
					if(Initialized)
						ValidationSummaryCollection.Instance.OnEditorPropertyAffectingValidationSettingsChanged(this);
					else
						validationSettingsChangedOnInit = true;
				}
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxEditEnabled"),
#endif
 AutoFormatDisable]
		public override bool Enabled {
			get { return base.Enabled; }
			set {
				if(value != base.Enabled) {
					base.Enabled = value;
					if(Initialized)
						ValidationSummaryCollection.Instance.OnEditorPropertyAffectingValidationSettingsChanged(this);
					else
						validationSettingsChangedOnInit = true;
				}
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxEditImageFolder"),
#endif
		Category("Images"), DefaultValue(""), Localizable(false), UrlProperty,
		AutoFormatEnable, AutoFormatImageFolderProperty, AutoFormatUrlProperty]
		public string ImageFolder {
			get { return ImageFolderInternal; }
			set { ImageFolderInternal = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxEditSpriteImageUrl"),
#endif
		Category("Images"), DefaultValue(""), Localizable(false), UrlProperty,
		AutoFormatEnable, AutoFormatUrlProperty,
		Editor(typeof(System.Web.UI.Design.UrlEditor), typeof(System.Drawing.Design.UITypeEditor))]
		public string SpriteImageUrl {
			get { return SpriteImageUrlInternal; }
			set { SpriteImageUrlInternal = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxEditSpriteCssFilePath"),
#endif
		Category("Images"), DefaultValue(""), Localizable(false), UrlProperty,
		AutoFormatEnable, AutoFormatUrlProperty,
		Editor(typeof(System.Web.UI.Design.UrlEditor), typeof(System.Drawing.Design.UITypeEditor))]
		public string SpriteCssFilePath {
			get { return SpriteCssFilePathInternal; }
			set { SpriteCssFilePathInternal = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxEditReadOnlyStyle"),
#endif
		Category("Styles"), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public ReadOnlyStyle ReadOnlyStyle {
			get { return Properties.ReadOnlyStyle; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxEditFocusedStyle"),
#endif
		Category("Styles"), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual EditorDecorationStyle FocusedStyle {
			get { return Properties.FocusedStyle; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxEditInvalidStyle"),
#endif
		Category("Styles"), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public EditorDecorationStyle InvalidStyle {
			get { return Properties.InvalidStyle; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxEditValidation"),
#endif
		Category("Action")]
		public event EventHandler<ValidationEventArgs> Validation
		{
			add { Events.AddHandler(EventValidation, value); }
			remove { Events.RemoveHandler(EventValidation, value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxEditValueChanged"),
#endif
		Category("Action")]
		public event EventHandler ValueChanged
		{
			add { Events.AddHandler(EventValueChanged, value); }
			remove { Events.RemoveHandler(EventValueChanged, value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxEditValue"),
#endif
		Bindable(true, BindingDirection.TwoWay), AutoFormatDisable]
		public override object Value {
			get {
				if(CommonUtils.IsNullValue(base.Value) || (base.Value.ToString() == "" && ConvertEmptyStringToNull))
					return null;
				else
					return base.Value;
			}
			set { base.Value = value; }
		}
		protected override string FocusedControlIDValue {
			get {
				string value = base.FocusedControlIDValue;
				if(ValidationSettings.EditingRowVisibleIndex >= 0)
					value += "_Row" + ValidationSettings.EditingRowVisibleIndex;
				return value;
			}
		}
		private static bool InvalidEditorFocused {
			get { return HttpUtils.GetContextValue("InvalidEditorFocused", false); }
			set { HttpUtils.SetContextValue("InvalidEditorFocused", value); }
		}
		private string IsValidContextValueKey {
			get {
				string key = UniqueID + "_IsValid";
				if(ValidationSettings.EditingRowVisibleIndex >= 0)
					key += "_Row" + ValidationSettings.EditingRowVisibleIndex;
				return key;
			}
		}
		protected virtual bool IsCustomValidationEnabled {
			get { return IsCustomValidationEnabledCore(IsValid, ValidationSettings.EnableCustomValidation, true); }
		}
		protected override bool IsFocused() {
			bool currentControlGetsFocused = EnsureFirstInvalidIsFocused();
			return currentControlGetsFocused || base.IsFocused();
		}
		protected bool EnsureFirstInvalidIsFocused() {
			return TryFocusEditorOnError();
		}
		internal bool IsCustomValidationEnabledCore(bool isValid, bool enableCustomValidation, bool isVisible) {
			return isVisible && IsEnabled() && (!isValid || enableCustomValidation || IsValidationEventsAssigned || HasValidationPatterns);
		}
		protected virtual bool HasValidationPatterns {
			get { return !ValidationSettings.ValidationPatterns.IsEmpty; }
		}
		protected internal virtual bool IsErrorFrameRequired {
			get { return IsCustomValidationEnabled && ValidationSettings.Display != Display.None; }
		}
		protected bool IsValidationEventsAssigned {
			get {
				return (Properties.ClientSideEvents as EditClientSideEvents).Validation != "" ||
					HasEvents() && Events[EventValidation] != null;
			}
		}
		protected internal new EditProperties Properties {
			get { return (EditProperties)base.Properties; }
		}
		protected internal bool ShowErrorFrame {
			get {
				return showErrorFrame;
			}
			set {
				showErrorFrame = value;
				LayoutChanged();
			}
		}
		protected override bool ConvertEmptyStringToNull {
			get { return Properties.ConvertEmptyStringToNull; }
		}
		private bool notifyValidationSummariesToAcceptNewError;
		internal bool NotifyValidationSummariesToAcceptNewError {
			get { return notifyValidationSummariesToAcceptNewError; }
			set { notifyValidationSummariesToAcceptNewError = value; }
		}
		bool validationSettingsChangedOnInit = false;
		protected internal override void InitInternal() {
			base.InitInternal();
			if(validationSettingsChangedOnInit) {
				ValidationSummaryCollection.Instance.OnEditorPropertyAffectingValidationSettingsChanged(this);
				validationSettingsChangedOnInit = false;
			}
		}
		protected virtual void RaiseValueChanged() {
			OnValueChanged(EventArgs.Empty);
		}
		public static void ClearEditorsInContainer(Control container) {
			ClearEditorsInContainer(container, null, false);
		}
		public static void ClearEditorsInContainer(Control container, string validationGroup) {
			ClearEditorsInContainer(container, validationGroup, false);
		}
		public static void ClearEditorsInContainer(Control container, bool clearInvisibleEditors) {
			ClearEditorsInContainer(container, null, clearInvisibleEditors);
		}
		public static void ClearEditorsInContainer(Control container, string validationGroup, bool clearInvisibleEditors) {
			EditorProcessingContext context = new EditorProcessingContext();
			ProcessEditorsInContainer(container,
				delegate(ASPxEdit edit) {
					edit.Value = null;
					edit.IsValid = true;
					return true;
				},
				delegate(ASPxEdit edit, string vGroup) {
					return vGroup == null || edit.ValidationSettings.ValidationGroup == vGroup;
				},
				validationGroup, clearInvisibleEditors, true, context);
			ExternalControlsValidator.OnClearEditorsInContainer(container, validationGroup, clearInvisibleEditors);
		}
		private bool localValidationSummaryUpdateRequestsLocked;
		private bool LocalValidationSummaryUpdateRequestsLocked {
			get { return localValidationSummaryUpdateRequestsLocked; }
			set { localValidationSummaryUpdateRequestsLocked = value; }
		}
		public void Validate() {
			if(!IsCustomValidationEnabledCore(true, false, true))
				return;
			ValidationResult patternValidationResult = ValidateInternal();
			LocalValidationSummaryUpdateRequestsLocked = true;
			try {
				bool isValid = true;
				string errorText = "";
				if(!patternValidationResult.IsValid) {
					isValid = false;
					errorText = patternValidationResult.ErrorText;
				} else {
					ValidationEventArgs args = new ValidationEventArgs(Value, ValidationSettings.ErrorText, true);
					OnValidation(args);
					isValid = args.IsValid;
					errorText = args.ErrorText;
					if(Value == null && args.Value != null || Value != null && !Value.Equals(args.Value))
						Value = args.Value;
				}
				if(!isValid) {
					this.IsValid = false;
					this.ErrorText = errorText;
				} else
					this.IsValid = true;
				TryFocusEditorOnError();
			} finally {
				LocalValidationSummaryUpdateRequestsLocked = false;
			}
			ValidationSummaryCollection.Instance.OnEditorIsValidStateChanged(this);
		}
		internal virtual ValidationResult ValidateInternal() {
			return ValidationSettings.ValidationPatterns.Validate(Value);
		}
		protected bool TryFocusEditorOnError() {
			if(!InvalidEditorFocused && this.Page != null && ValidationSettings.SetFocusOnError && !this.IsValid) {
				InvalidEditorFocused = true;
				this.Focus();
				return true;
			}
			return false;
		}
		public static bool ValidateEditorsInContainer(Control container) {
			return ValidateEditorsInContainer(container, null, false);
		}
		public static bool ValidateEditorsInContainer(Control container, bool validateInvisibleEditors) {
			return ValidateEditorsInContainer(container, null, validateInvisibleEditors);
		}
		public static bool ValidateEditorsInContainer(Control container, string validationGroup) {
			return ValidateEditorsInContainer(container, validationGroup, false);
		}
		public static bool ValidateEditorsInContainer(Control container, string validationGroup, bool validateInvisibleEditors) {
			if(container == null)
				throw new ArgumentNullException("Validation container is not specified.");
			Page page = container.Page;
			EditorProcessingContext context = new EditorProcessingContext();
			bool isValid = ProcessEditorsInContainer(container,
				delegate(ASPxEdit edit) {
					edit.Validate();
					return edit.IsValid;
				},
				delegate(ASPxEdit edit, string vGroup) {
					return (edit.IsCustomValidationEnabled &&
						(vGroup == null || edit.ValidationSettings.ValidationGroup == vGroup));
				},
				validationGroup, validateInvisibleEditors, false, context
			);
			isValid = ExternalControlsValidator.OnValidateEditorsInContainer(container, validationGroup, validateInvisibleEditors) && isValid;
			InvalidEditorFocused = false;
			if(GlobalEventsAccessor.GlobalEventsExists(page)) {
				ValidationCompletedEventArgs e = new ValidationCompletedEventArgs(container, validationGroup, validateInvisibleEditors,
					isValid, context.FirstInvalidEditor, context.FirstVisibleInvalidEditor);
				GlobalEventsAccessor.RaiseValidationCompleted(page, e);
				isValid = e.IsValid;
			}
			return isValid;
		}
		public static bool AreEditorsValid(Control container) {
			return AreEditorsValid(container, null, false);
		}
		public static bool AreEditorsValid(Control container, string validationGroup) {
			return AreEditorsValid(container, validationGroup, false);
		}
		public static bool AreEditorsValid(Control container, bool checkInvisibleEditors) {
			return AreEditorsValid(container, null, checkInvisibleEditors);
		}
		public static bool AreEditorsValid(Control container, string validationGroup, bool checkInvisibleEditors) {
			if(container == null)
				throw new ArgumentNullException("Container is not specified.");
			bool areEditorsValid = ProcessEditorsInContainer(container,
				delegate(ASPxEdit edit) {
					return edit.IsValid;
				},
				delegate(ASPxEdit edit, string vGroup) {
					return (edit.IsCustomValidationEnabled &&
						(vGroup == null || edit.ValidationSettings.ValidationGroup == vGroup));
				},
				validationGroup, checkInvisibleEditors, false, null
			);
			return ExternalControlsValidator.OnAreEditorsValidInContainer(container, validationGroup, checkInvisibleEditors) && areEditorsValid;
		}
		private static bool ProcessEditorsInContainer(Control container, EditorsProcessingProc processingProc, EditorsChoiceCondition choiceCondition,
			string validationGroup, bool processInvisibleEditors, bool processDisabledEditors, EditorProcessingContext context) {
			bool isSuccess = true;
			ASPxEdit edit = container as ASPxEdit;
			if(edit != null) {
				if(choiceCondition(edit, validationGroup) && (processDisabledEditors || edit.IsEnabled())) {
					bool isVisible = edit.IsVisibleAndClientVisible(); ;
					bool? isValid = null;
					if(processInvisibleEditors || isVisible)
						isValid = processingProc(edit);
					if(context != null && isValid.HasValue && !isValid.Value) {
						if(context.FirstInvalidEditor == null)
							context.FirstInvalidEditor = edit;
						if(context.FirstVisibleInvalidEditor == null && isVisible)
							context.FirstVisibleInvalidEditor = edit;
					}
					return isValid.HasValue ? isValid.Value : true;
				}
			} else if(processInvisibleEditors || container.Visible) {
				WebControl webControl = container as WebControl;
				if(processDisabledEditors || webControl == null || webControl.Enabled) {
					for(int i = 0; i < container.Controls.Count; i++)
						isSuccess = ProcessEditorsInContainer(container.Controls[i], processingProc, choiceCondition, validationGroup,
							processInvisibleEditors, processDisabledEditors, context) && isSuccess;
				}
			}
			return isSuccess;
		}
		protected internal virtual bool SendValueToServer {
			get { return true; }
		}
		protected override bool IsCaptionCellRequired() {
			return base.IsCaptionCellRequired() || CaptionSettings.RequiredMarkDisplayMode == EditorRequiredMarkMode.Required
				|| CaptionSettings.RequiredMarkDisplayMode == EditorRequiredMarkMode.Optional;
		}
		protected bool ShowRequiredMark() {
			return CaptionSettings.RequiredMarkDisplayMode == EditorRequiredMarkMode.Required ||
				CaptionSettings.RequiredMarkDisplayMode == EditorRequiredMarkMode.Auto && ValidationSettings.RequiredField.IsRequired;
		}
		protected bool ShowOptionalMark() {
			return CaptionSettings.RequiredMarkDisplayMode == EditorRequiredMarkMode.Optional;
		}
		protected override void CreateCaptionCellContent() {
			base.CreateCaptionCellContent();
			if (ShowRequiredMark()) {
				this.requiredMarkControl = RenderUtils.CreateWebControl(HtmlTextWriterTag.Em);
				this.requiredMarkLiteralControl = new LiteralControl();
				this.requiredMarkControl.Controls.Add(this.requiredMarkLiteralControl);
				CaptionCell.Controls.Add(this.requiredMarkControl);
			}
			if (ShowOptionalMark()) {
				this.optionalMarkControl = RenderUtils.CreateWebControl(HtmlTextWriterTag.Em);
				this.optionalMarkLiteralControl = new LiteralControl();
				this.optionalMarkControl.Controls.Add(this.optionalMarkLiteralControl);
				CaptionCell.Controls.Add(this.optionalMarkControl);
			}
		}
		protected override void ClearControlFields() {
			this.errorCell = null;
			this.errorImageCell = null;
			this.errorTextCell = null;
			this.errorImage = null;
			this.requiredMarkControl = null;
			this.optionalMarkControl = null;
			this.requiredMarkLiteralControl = null;
			this.optionalMarkLiteralControl = null;
			base.ClearControlFields();
		}
		protected override void CreateControlHierarchy() {
			if(IsErrorFrameRequired) {
				ExternalTable = RenderUtils.CreateTable();
				ExternalTable.ID = ExternalTableID;
				if(IsCustomValidationEnabled && (DesignMode && ShowErrorFrame || !DesignMode))
					CreateDoubleCellHierarchy(ExternalTable);
				else
					CreateSingleCellHierarchy(ExternalTable);
				ControlsBase.Add(ExternalTable);
			}
			base.CreateControlHierarchy();
		}
		protected void CreateSingleCellHierarchy(Table container) {
			TableRow row = RenderUtils.CreateTableRow();
			ControlCell = RenderUtils.CreateTableCell();
			row.Cells.Add(ControlCell);
			container.Rows.Add(row);
		}
		protected void CreateDoubleCellHierarchy(Table container) {
			ControlCell = RenderUtils.CreateTableCell();
			ControlCell.ID = ControlCellID;
			if(ValidationSettings.ErrorDisplayMode != ErrorDisplayMode.None) {
				this.errorCell = RenderUtils.CreateTableCell();
				errorCell.ID = ErrorCellID;
				CreateErrorCellContent(errorCell);
				if(ValidationSettings.ErrorTextPosition == ErrorTextPosition.Top ||
					ValidationSettings.ErrorTextPosition == ErrorTextPosition.Bottom) {
					TableRow controlsRow = RenderUtils.CreateTableRow();
					TableRow errorRow = RenderUtils.CreateTableRow();
					controlsRow.Cells.Add(ControlCell);
					errorRow.Cells.Add(this.errorCell);
					if(ValidationSettings.ErrorTextPosition == ErrorTextPosition.Top) {
						container.Rows.Add(errorRow);
						container.Rows.Add(controlsRow);
					} else {
						container.Rows.Add(controlsRow);
						container.Rows.Add(errorRow);
					}
				} else {
					TableRow row = RenderUtils.CreateTableRow();
					if(ValidationSettings.ErrorTextPosition == ErrorTextPosition.Left) {
						row.Cells.Add(this.errorCell);
						row.Cells.Add(ControlCell);
					} else {
						row.Cells.Add(ControlCell);
						row.Cells.Add(this.errorCell);
					}
					container.Rows.Add(row);
				}
			} else {
				TableRow row = RenderUtils.CreateTableRow();
				container.Rows.Add(row);
				row.Cells.Add(ControlCell);
			}
		}
		protected void CreateErrorCellContent(TableCell errorCell) {
			Control content;
			Control text = RenderUtils.CreateLiteralControl(HtmlEncode(ErrorText));
			if(ValidationSettings.IsImageMode && !GetErrorImage().IsEmpty) {
				content = RenderUtils.CreateTable();
				(content as Table).Width = Unit.Percentage(100);
				TableRow row = RenderUtils.CreateTableRow();
				this.errorImageCell = RenderUtils.CreateTableCell();
				(content as Table).Rows.Add(row);
				row.Cells.Add(this.errorImageCell);
				this.errorImage = RenderUtils.CreateImage();
				this.errorImage.ID = ErrorImageID;
				this.errorImageCell.Controls.Add(this.errorImage);
				if(ValidationSettings.IsTextMode) {
					this.errorTextCell = RenderUtils.CreateTableCell();
					errorTextCell.ID = ErrorTextCellID;
					row.Cells.Add(this.errorTextCell);
					this.errorTextCell.Controls.Add(text);
				}
			}
			else
				content = text;
			errorCell.Controls.Add(content);
		}
		protected override void PrepareControlHierarchy() {
			if(ExternalTable != null && IsErrorFrameRequired) {
				this.errorFramePreparer.PrepareErrorFrame();
				this.errorFramePreparer.PrepareErrorCell(this.errorCell, this.errorTextCell, this.errorImageCell, this.errorImage);
			}
			PrepareMarks();
			base.PrepareControlHierarchy();
		}
		protected void PrepareMarks() {
			if (this.requiredMarkLiteralControl != null) {
				this.requiredMarkLiteralControl.Text = HtmlEncode(CaptionSettings.RequiredMark);
			}
			if (this.optionalMarkLiteralControl != null) {
				this.optionalMarkLiteralControl.Text = HtmlEncode(CaptionSettings.OptionalMark);
			}
			if (this.requiredMarkControl != null) {
				AppearanceStyleBase requiredMarkStyle = GetRequiredMarkStyle();
				requiredMarkStyle.AssignToControl(this.requiredMarkControl);
			}
			if (this.optionalMarkControl != null) {
				AppearanceStyleBase optionalMarkStyle = GetOptionalMarkStyle();
				optionalMarkStyle.AssignToControl(this.optionalMarkControl);
			}
		}
		protected override bool LoadPostData(NameValueCollection postCollection) {
			bool valueChanged = base.LoadPostData(postCollection);
			string clientValidationStateStr = GetClientObjectStateValueString(ValidationStateKey);
			if(!string.IsNullOrEmpty(clientValidationStateStr)) {
				IsValid = false;
				ErrorText = HttpUtility.HtmlDecode(clientValidationStateStr.Substring("-".Length));
			}
			return valueChanged;
		}
		protected virtual void OnValidation(ValidationEventArgs e) {
			EventHandler<ValidationEventArgs> handler =
				Events[EventValidation] as EventHandler<ValidationEventArgs>;
			if(handler != null)
				handler(this, e);
		}
		protected override Style CreateControlStyle() {
			return new AppearanceStyle();
		}
		protected virtual void PrepareControlStyleCore(AppearanceStyleBase style) {
			style.CopyFrom(GetDefaultEditStyle());
			MergeParentSkinOwnerControlStyle(style);
			style.CopyFrom(GetEditStyleFromStylesStorage());
			style.CopyFrom(RenderStyles.Style);
			style.CopyFrom(ControlStyle);
			MergeDisableStyle(style);
		}
		protected virtual void PostPrepareControlStyle(AppearanceStyleBase style) {
		}
		protected sealed override void PrepareControlStyle(AppearanceStyleBase style) {
			PrepareControlStyleCore(style);
			if(ReadOnly) {
				style.CopyFrom(RenderStyles.GetDefaultReadOnlyStyle());
				style.CopyFrom(RenderStyles.ReadOnly);
			}
			PostPrepareControlStyle(style);
		}
		protected virtual AppearanceStyle GetDefaultEditStyle() {
			return AppearanceStyle.Empty;
		}
		protected virtual AppearanceStyle GetEditStyleFromStylesStorage() {
			return AppearanceStyle.Empty;
		}
		protected ErrorFrameStyle GetControlCellStyle() {
			ErrorFrameStyle errorFrameStyle = GetErrorFrameStyle();
			ErrorFrameStyle controlCellStyle = new ErrorFrameStyle();
			controlCellStyle.CopyFrom(RenderStyles.GetDefaultControlCellStyle());
			if(ValidationSettings.ErrorDisplayMode != ErrorDisplayMode.None) {
				bool generalPaddingIsNotEmpty = !errorFrameStyle.Paddings.Padding.IsEmpty;
				switch(ValidationSettings.ErrorTextPosition) {
					case ErrorTextPosition.Left:
						if(generalPaddingIsNotEmpty || !errorFrameStyle.Paddings.PaddingLeft.IsEmpty)
							errorFrameStyle.Paddings.PaddingLeft = 0;
						break;
					case ErrorTextPosition.Right:
						if(generalPaddingIsNotEmpty || !errorFrameStyle.Paddings.PaddingRight.IsEmpty)
							errorFrameStyle.Paddings.PaddingRight = 0;
						break;
					case ErrorTextPosition.Top:
						if(generalPaddingIsNotEmpty || !errorFrameStyle.Paddings.PaddingTop.IsEmpty)
							errorFrameStyle.Paddings.PaddingTop = 0;
						break;
					case ErrorTextPosition.Bottom:
						if(generalPaddingIsNotEmpty || !errorFrameStyle.Paddings.PaddingBottom.IsEmpty)
							errorFrameStyle.Paddings.PaddingBottom = 0;
						break;
					default:
						throw new InvalidOperationException("ValidationSettings.ErrorTextPosition has an unexpected value.");
				}
			}
			controlCellStyle.Paddings.CopyFrom(errorFrameStyle.Paddings);
			return controlCellStyle;
		}
		protected AppearanceStyleBase GetRequiredMarkStyle() {
			AppearanceStyleBase style = new AppearanceStyleBase();
			style.CopyFrom(RenderStyles.GetDefaultRequiredMarkStyle());
			return style;
		}
		protected AppearanceStyleBase GetOptionalMarkStyle() {
			AppearanceStyleBase style = new AppearanceStyleBase();
			style.CopyFrom(RenderStyles.GetDefaultOptionalMarkStyle());
			return style;
		}
		protected ErrorFrameStyle GetErrorFrameStyle() {
			ErrorFrameStyle style = new ErrorFrameStyle();
			if(ValidationSettings.ErrorDisplayMode != ErrorDisplayMode.None)
				style.CopyFrom(RenderStyles.GetDefaultErrorFrameStyle());
			else
				style.CopyFrom(RenderStyles.GetDefaultErrorFrameStyleErrorIsNotDisplayed());
			MergeControlStyle(style, false);
			style.CopyFrom(ValidationSettings.ErrorFrameStyle);
			style.CssClass = RenderUtils.CombineCssClasses(style.CssClass, EditorStyles.ErrorFrameSystemClassName);
			return style;
		}
		protected ErrorFrameStyle GetErrorCellStyle() {
			ErrorFrameStyle style = new ErrorFrameStyle();
			style.CopyFrom(RenderStyles.GetDefaultErrorCellStyle());
			style.CopyFrom(GetErrorFrameStyle());
			style.CssClass = RenderUtils.CombineCssClasses(style.CssClass, EditorStyles.ErrorCellSystemClassName);
			return style;
		}
		protected internal DisabledStyle GetDisabledCssStyleForInputElement() {
			DisabledStyle style = GetDisabledCssStyle();
			style.Border.Reset();
			style.BackgroundImage.Reset();
			return style;
		}
		protected AppearanceStyleBase GetReadOnlyStyle() {
			AppearanceStyleBase style = new AppearanceStyleBase();
			style.CopyFrom(RenderStyles.GetDefaultReadOnlyStyle());
			style.CopyFrom(ReadOnlyStyle);
			return style;
		}
		protected AppearanceStyleBase GetFocusedStyle() {
			AppearanceStyleBase style = new AppearanceStyleBase();
			style.CopyFrom(RenderStyles.GetDefaultFocusedStyle());
			style.CopyFrom(RenderStyles.Focused);
			style.CopyFrom(FocusedStyle);
			return style;
		}
		protected AppearanceStyleBase GetInvalidStyle() {
			AppearanceStyleBase style = new AppearanceStyleBase();
			style.CopyFrom(RenderStyles.GetDefaultInvalidStyle());
			style.CopyFrom(InvalidStyle);
			return style;
		}
		protected virtual Dictionary<string, AppearanceStyleBase> GetDecorationStyles() {
			Dictionary<string, AppearanceStyleBase> map = new Dictionary<string, AppearanceStyleBase>();
			if(IsEnabled()) {
				if(IsCustomValidationEnabled) {
					AppearanceStyleBase invalidStyle = GetInvalidStyle();
					if(!invalidStyle.IsEmpty)
						map.Add("I", invalidStyle);
				}
				if(RenderStyles.EnableFocusedStyle) {
					AppearanceStyleBase focused = GetFocusedStyle();
					if(!focused.IsEmpty)
						map.Add("F", focused);
				}
			}
			return map;
		}
		protected ImageProperties GetErrorImage() {
			bool dummy;
			return GetErrorImage(out dummy);
		}
		protected ImageProperties GetErrorImage(out bool isDefault) {
			if(!ValidationSettings.ErrorImage.IsEmpty) {
				isDefault = false;
				return ValidationSettings.ErrorImage;
			}
			else {
				isDefault = true;
				return RenderImages.GetImageProperties(Page, EditorImages.ErrorImageName);
			}
		}
		protected Paddings GetErrorCellPaddings() {
			ErrorFrameStyle errorFrameStyle = GetErrorFrameStyle();
			Paddings errorCellPaddings = errorFrameStyle.ErrorTextPaddings;
			OverrideConcurrentPadding(errorFrameStyle.Paddings, errorCellPaddings, ValidationSettings.ErrorTextPosition);
			if((ValidationSettings.ErrorTextPosition == ErrorTextPosition.Top || ValidationSettings.ErrorTextPosition == ErrorTextPosition.Bottom) &&
				errorCellPaddings.Padding.IsEmpty) {
				if(errorCellPaddings.PaddingLeft.IsEmpty)
					errorCellPaddings.PaddingLeft = errorFrameStyle.Paddings.GetPaddingLeft();
				if(errorCellPaddings.PaddingRight.IsEmpty)
					errorCellPaddings.PaddingRight = errorFrameStyle.Paddings.GetPaddingRight();
			}
			return errorCellPaddings;
		}
		protected static void OverrideConcurrentPadding(Paddings masterPadding, Paddings slavePaddings, ErrorTextPosition errorTextPosition) {
			Unit generalMasterPadding = masterPadding.Padding;
			if(!generalMasterPadding.IsEmpty)
				SetSpecificPadding(slavePaddings, errorTextPosition, generalMasterPadding);
			Unit specificMasterPadding = GetSpecificPadding(masterPadding, errorTextPosition);
			if(!specificMasterPadding.IsEmpty)
				SetSpecificPadding(slavePaddings, errorTextPosition, specificMasterPadding);
		}
		protected static Unit GetSpecificPadding(Paddings paddings, ErrorTextPosition errorTextPosition) {
			switch(errorTextPosition) {
				case ErrorTextPosition.Bottom:
					return paddings.PaddingBottom;
				case ErrorTextPosition.Left:
					return paddings.PaddingLeft;
				case ErrorTextPosition.Right:
					return paddings.PaddingRight;
				case ErrorTextPosition.Top:
					return paddings.PaddingTop;
				default:
					throw new ArgumentException("errorTextPosition");
			}
		}
		protected static void SetSpecificPadding(Paddings paddings, ErrorTextPosition errorTextPosition, Unit value) {
			switch(errorTextPosition) {
				case ErrorTextPosition.Bottom:
					paddings.PaddingBottom = value;
					break;
				case ErrorTextPosition.Left:
					paddings.PaddingLeft = value;
					break;
				case ErrorTextPosition.Right:
					paddings.PaddingRight = value;
					break;
				case ErrorTextPosition.Top:
					paddings.PaddingTop = value;
					break;
				default:
					throw new ArgumentException("errorTextPosition");
			}
		}
		protected Unit GetErrorImageSpacing() {
			return GetErrorFrameStyle().ImageSpacing;
		}
		protected internal override void RegisterExpandoAttributes(ExpandoAttributes expandoAttributes) {
			base.RegisterExpandoAttributes(expandoAttributes);
			if(IsErrorFrameRequired)
				expandoAttributes.AddAttribute("errorFrame", "errorFrame", string.Format("{0}_{1}", ClientID, ExternalTableID));
		}
		protected string GetPostBackEventReference(bool wrapWithAnonymFunc, bool replaceArgument) {
			return RenderUtils.GetPostBackEventReference(this, "", ValidationSettings.CausesValidation,
				ValidationSettings.ValidationGroup, "", true, replaceArgument, true, wrapWithAnonymFunc);
		}
		protected override string GetClientObjectClassName() {
			return "ASPxClientEdit";
		}
		protected override void GetCreateClientObjectScript(StringBuilder stb, string localVarName, string clientName) {
			base.GetCreateClientObjectScript(stb, localVarName, clientName);
			if(HeightCorrectionRequired)
				stb.Append(localVarName + ".heightCorrectionRequired = true;\n");
			if(ValidationSettings.CausesValidation && (Page == null || Page.GetValidators(ValidationSettings.ValidationGroup).Count > 0)) {
				string sendPostBackWithValidationScript = GetPostBackEventReference(true, true);
				if(!string.IsNullOrEmpty(sendPostBackWithValidationScript))
					stb.AppendFormat("{0}.sendPostBackWithValidation = {1};\n", localVarName, sendPostBackWithValidationScript);
			}
			if(ValidationSettings.ValidationGroup != "")
				stb.AppendFormat("{0}.validationGroup = \"{1}\";\n", localVarName, ValidationSettings.ValidationGroup);
			if(IsCustomValidationEnabled) {
				stb.Append(localVarName + ".customValidationEnabled = true;\n");
				stb.AppendFormat("{0}.isValid = {1};\n", localVarName, IsValid ? "true" : "false");
				if(ValidationSettings.ErrorText != "")
					stb.Append(String.Format("{0}.errorText = {1};\n", localVarName, HtmlConvertor.ToScript(ValidationSettings.ErrorText)));
				if(!ValidationSettings.ValidationPatterns.IsEmpty)
					stb.Append(String.Format("{0}.validationPatterns = {1};\n", localVarName, ValidationSettings.ValidationPatterns.GetClientValidationPatternsArray()));
				if(ValidationSettings.CausesValidation)
					stb.Append(localVarName + ".causesValidation = true;\n");
				if(!ValidationSettings.ValidateOnLeave)
					stb.Append(localVarName + ".validateOnLeave = false;\n");
				if(ValidationSettings.SetFocusOnError)
					stb.Append(localVarName + ".setFocusOnError = true;\n");
				if(NotifyValidationSummariesToAcceptNewError)
					stb.Append(localVarName + ".notifyValidationSummariesToAcceptNewError = true;\n");
				if(ValidationSettings.Display != Display.Static)
					stb.AppendFormat("{0}.display = \"{1}\";\n", localVarName, ValidationSettings.Display.ToString());
				if(IsErrorFrameRequired) {
					if(ValidationSettings.ErrorDisplayMode != ErrorDisplayMode.ImageWithText) {
						char? errorDisplayModeCode = null;
						if(ValidationSettings.ErrorDisplayMode == ErrorDisplayMode.Text)
							errorDisplayModeCode = 't';
						else if(ValidationSettings.ErrorDisplayMode == ErrorDisplayMode.ImageWithTooltip)
							errorDisplayModeCode = 'i';
						else if(ValidationSettings.ErrorDisplayMode == ErrorDisplayMode.None)
							errorDisplayModeCode = 'n';
						if(errorDisplayModeCode.HasValue)
							stb.AppendFormat("{0}.errorDisplayMode = \"{1}\";\n", localVarName, errorDisplayModeCode.Value);
					}
					if(ValidationSettings.IsImageMode && !GetErrorImage().IsEmpty)
						stb.Append(localVarName + ".errorImageIsAssigned = true;\n");
					this.errorFramePreparer.InitControlCellStyles(stb, localVarName);
				}
			}
			if(!ConvertEmptyStringToNull)
				stb.Append(localVarName + ".convertEmptyStringToNull = false;\n");
			if(ReadOnly)
				stb.AppendLine(localVarName + ".readOnly=true;");
			GenerateStyleDecorationScript(stb, localVarName);
		}
		void GenerateStyleDecorationScript(StringBuilder builder, string localVarName) {
			Dictionary<string, AppearanceStyleBase> map = GetDecorationStyles();
			if(map.Count < 1) return;
			builder.AppendFormat("{0}.RequireStyleDecoration();\n", localVarName);
			foreach(string key in map.Keys) {
				builder.AppendFormat("{0}.styleDecoration.AddStyle({1},{2},{3});\n", localVarName,
					HtmlConvertor.ToScript(key), 
					HtmlConvertor.ToScript(map[key].CssClass ?? ""),
					HtmlConvertor.ToScript(map[key].GetStyleAttributes(this).Value ?? ""));
			}
		}
		protected override string GetClientObjectStateInputID() {
			return UniqueID + ClientObjectStateInputIDSuffix;
		}
		protected virtual bool HeightCorrectionRequired {
			get { return false; }
		}
		protected virtual bool ClientFocusHandlersRequiredForKBSupport {
			get { return false; }
		}
		protected virtual bool HasFocusEvents() { 
			EditClientSideEvents editClientSideEvents = (EditClientSideEvents)ClientSideEventsInternal;
			return ClientFocusHandlersRequiredForKBSupport || ValidationSettings.SetFocusOnError ||
				editClientSideEvents.GotFocus != "" || editClientSideEvents.LostFocus != ""
				|| GetDecorationStyles().Count > 0;
		}
		protected virtual bool HasGotFocusEvent() {
			return HasFocusEvents();
		}
		protected virtual bool HasLostFocusEvent() {
			return HasFocusEvents();
		}
		protected internal virtual string GetOnGotFocus() {
			return HasGotFocusEvent() ? string.Format(GotFocusHandlerName, ClientID) : "";
		}
		protected internal virtual string GetOnLostFocus() {
			return HasLostFocusEvent() ? string.Format(LostFocusHandlerName, ClientID) : "";
		}
		internal bool requireValueChangedHandler = false;
		protected internal void ForceUseValueChangedClientEvent() {
			this.requireValueChangedHandler = true;
		}
		protected override bool HasFunctionalityScripts() {
			return true;
		}
		protected override bool HasClientInitialization() {
			return base.HasClientInitialization() || IsCustomValidationEnabled;
		}
		protected override void RaisePostDataChangedEvent() {
			if(ValidationSettings.CausesValidation) {
				ASPxEdit.ValidateEditorsInContainer(Page, ValidationSettings.ValidationGroup);
				Page.Validate(ValidationSettings.ValidationGroup);
			}
			else
				Validate();
			RaiseValueChanged();
		}
		protected internal string GetKBSupportInputId() {
			return "KBS";
		}
		string IAssociatedControlID.ClientID() {
			return GetAssociatedControlID();
		}
		protected internal override string GetAssociatedControlID() {
			return ClientID;
		}
		protected void OnValueChanged(EventArgs e) {
			EventHandler handler = Events[EventValueChanged] as EventHandler;
			if(handler != null) handler(this, e);
		}
		protected internal new void PropertyChanged(string name) {
			base.PropertyChanged(name);
		}
		string IValidationSummaryEditor.ValidationGroup {
			get { return ValidationSettings.ValidationGroup; }
			set { ValidationSettings.ValidationGroup = value; }
		}
		bool IValidationSummaryEditor.NotifyValidationSummariesToAcceptNewError {
			get { return NotifyValidationSummariesToAcceptNewError; }
			set { NotifyValidationSummariesToAcceptNewError = value; }
		}
		bool IValidationSummaryEditor.IsValidationEnabled() {
			return IsCustomValidationEnabledCore(IsValid, ValidationSettings.EnableCustomValidation, IsVisible());
		}
	}
}
namespace DevExpress.Web.Internal {
	public delegate bool ExternalEditorsEventHandler(Control container, string validationGroup, bool processingInvisibleEditors);
	public interface IValueTypeHolder {
		Type ValueType { get; set; }
	}
	public interface IAssignEditorProperties {
		void AssignEditorProperties(ASPxEditBase editor);
	}
	public class ExternalControlsValidator {
		private static event ExternalEditorsEventHandler ValidateEditorsEvents;
		private static event ExternalEditorsEventHandler ClearEditorsEvents;
		private static event ExternalEditorsEventHandler AreEditorsValidEvents;
		public static event ExternalEditorsEventHandler ValidateEditorsInContainer {
			add {
				if(!IsValidateEditorsHandlerExists(value))
					ValidateEditorsEvents += value;
			}
			remove { ValidateEditorsEvents -= value; }
		}
		public static event ExternalEditorsEventHandler ClearEditorsInContainer {
			add {
				if(!IsClearEditorsHandlerExists(value))
					ClearEditorsEvents += value;
			}
			remove { ClearEditorsEvents -= value; }
		}
		public static event ExternalEditorsEventHandler AreEditorsValidInContainer {
			add {
				if(!IsAreEditorsValidHandlerExists(value))
					AreEditorsValidEvents += value;
			}
			remove { AreEditorsValidEvents -= value; }
		}
		private static bool IsValidateEditorsHandlerExists(ExternalEditorsEventHandler handler) {
			return ProcessingHandlerExists(handler, ValidateEditorsEvents);
		}
		private static bool IsClearEditorsHandlerExists(ExternalEditorsEventHandler handler) {
			return ProcessingHandlerExists(handler, ClearEditorsEvents);
		}
		private static bool IsAreEditorsValidHandlerExists(ExternalEditorsEventHandler handler) {
			return ProcessingHandlerExists(handler, AreEditorsValidEvents);
		}
		private static bool ProcessingHandlerExists(ExternalEditorsEventHandler handler, ExternalEditorsEventHandler events) {
			if(events == null)
				return false;
			foreach(ExternalEditorsEventHandler item in events.GetInvocationList())
				if(item == handler)
					return true;
			return false;
		}
		public static bool OnValidateEditorsInContainer(Control container, string validationGroup, bool validateInvisibleEditors) {
			if(ValidateEditorsEvents == null)
				return true;
			bool isValid = true;
			foreach(ExternalEditorsEventHandler handler in ValidateEditorsEvents.GetInvocationList()) {
				if(!handler(container, validationGroup, validateInvisibleEditors))
					isValid = false;
			}
			return isValid;
		}
		public static void OnClearEditorsInContainer(Control container, string validationGroup, bool clearInvisibleEditors) {
			if(ClearEditorsEvents == null)
				return;
			foreach(ExternalEditorsEventHandler handler in ClearEditorsEvents.GetInvocationList())
				handler(container, validationGroup, clearInvisibleEditors);
		}
		public static bool OnAreEditorsValidInContainer(Control container, string validationGroup, bool checkInvisibleEditors) {
			if(AreEditorsValidEvents == null)
				return true;
			bool isValid = true;
			foreach(ExternalEditorsEventHandler handler in AreEditorsValidEvents.GetInvocationList()) {
				if(!handler(container, validationGroup, checkInvisibleEditors))
					isValid = false;
			}
			return isValid;
		}
	}
	public static class EditorsIntegrationHelper {
		public static void DisableValidation(ASPxEditBase editor) {
			ASPxEdit validatedEditor = editor as ASPxEdit;
			if(validatedEditor != null) {
				validatedEditor.ValidationSettings.RequiredField.IsRequired = false;
				validatedEditor.ValidationSettings.RegularExpression.ValidationExpression = string.Empty;
				validatedEditor.IsValid = true;
			}
			EditClientSideEvents events = editor.Properties.ClientSideEvents as EditClientSideEvents;
			if(events != null)
				events.Validation = string.Empty;
		}
		public static void LockClientValueChanged(ASPxSpinEdit spin) {
			spin.lockClientValueChanged = true;
		}
		public static void CheckInplaceBound(EditPropertiesBase properties, Type dataType, Control parent) {
			properties.CheckInplaceBound(dataType, parent, false);
		}
		public static void DisableScrolling(ASPxListBox listbox) {
			listbox.InternalDisableScrolling = true;
		}
	}
	public class CssClassNameBuilder {
		private enum ControlType {
			TextEdit, Button, Text, CheckBox, RadioButton, RadioButtonList, CheckBoxList, Captcha, ListBox,
			ProgressBar, Calendar, Image, TrackBar, Memo, Custom
		}
		public static string GetCssClassNameByControl(Control control, string cssClassNameTemplate) {
			ControlType controlType = GetControlType(control);
			return string.Format(cssClassNameTemplate, controlType);
		}
		private static ControlType GetControlType(Control control) {
			if (control != null) {
				Type controlType = control.GetType();
				if (typeof(ASPxMemo).IsAssignableFrom(controlType))
					return ControlType.Memo;
				else if (controlType.IsSubclassOf(typeof(ASPxTextEdit)))
					return ControlType.TextEdit;
				else if(typeof(ASPxLabel).IsAssignableFrom(controlType) || typeof(ASPxHyperLink).IsAssignableFrom(controlType)
					|| typeof(LiteralControl).IsAssignableFrom(controlType))
					return ControlType.Text;
				else if (typeof(ASPxButton).IsAssignableFrom(controlType))
					return ControlType.Button;
				else if (typeof(ASPxCheckBox).IsAssignableFrom(controlType))
					return ControlType.CheckBox;
				else if (typeof(ASPxTrackBar).IsAssignableFrom(controlType))
					return ControlType.TrackBar;
				else if (typeof(ASPxRadioButton).IsAssignableFrom(controlType))
					return ControlType.RadioButton;
				else if (typeof(ASPxRadioButtonList).IsAssignableFrom(controlType))
					return ControlType.RadioButtonList;
				else if (typeof(ASPxCheckBoxList).IsAssignableFrom(controlType))
					return ControlType.CheckBoxList;
				else if (typeof(ASPxCaptcha).IsAssignableFrom(controlType))
					return ControlType.Captcha;
				else if (typeof(ASPxListBox).IsAssignableFrom(controlType))
					return ControlType.ListBox;
				else if (typeof(DevExpress.Web.ASPxProgressBar).IsAssignableFrom(controlType))
					return ControlType.ProgressBar;
				else if (typeof(ASPxCalendar).IsAssignableFrom(controlType))
					return ControlType.Calendar;
				else if (typeof(ASPxImage).IsAssignableFrom(controlType) || typeof(ASPxBinaryImage).IsAssignableFrom(controlType))
					return ControlType.Image;
			}
			return ControlType.Custom;
		}
	}
}
