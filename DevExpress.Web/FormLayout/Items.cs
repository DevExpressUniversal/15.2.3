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
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using DevExpress.Utils;
using DevExpress.Web.Internal;
using DevExpress.Web;
using DevExpress.Web.Design;
namespace DevExpress.Web {
	public interface IFormLayoutOwner {
		string[] GetColumnNames();
		object FindColumnByName(string columnName);
		FormLayoutProperties GenerateDefaultLayout(bool fromControlDesigner);
	}
	public class LayoutItemNestedControlContainer : ContentControl {
		public LayoutItemNestedControlContainer()
			: base() {
		}
		[Obsolete("This method is now obsolete. Use the LayoutItem.GetNestedControl() method instead.")]
		public ASPxWebControl GetNestedControl() {
			return LayoutItem.GetNestedControl(Controls, NestedControlSearchMode.ReturnFirstAllowedControl) as ASPxWebControl;
		}
		protected override HtmlTextWriterTag TagKey {
			get { return HtmlTextWriterTag.Div; }
		}
		protected override bool HasRootTag() {
			return DesignMode;
		}
	}
	public class LayoutItemNestedControlCollection : ContentControlCollection {
		public LayoutItemNestedControlCollection(Control owner)
			: base(owner) {
		}
		public void Assign(LayoutItemNestedControlCollection source) {
			Clear();
			foreach (LayoutItemNestedControlContainer layoutItemEditor in source)
				Add(layoutItemEditor);
		}
		public new LayoutItemNestedControlContainer this[int i] {
			get { return (LayoutItemNestedControlContainer)base[i]; }
		}
		internal sealed override bool IsChildTypeValid(Control child) {
			return typeof(LayoutItemNestedControlContainer).IsAssignableFrom(child.GetType());
		}
		protected override Type GetChildType() {
			return typeof(LayoutItemNestedControlContainer);
		}
	}
	public abstract class ContentPlaceholderLayoutItem : LayoutItem {
		[Browsable(false), AutoFormatDisable, EditorBrowsableAttribute(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new LayoutItemNestedControlCollection LayoutItemNestedControlCollection {
			get { return base.LayoutItemNestedControlCollection; }
		}
#pragma warning disable 618
		[Browsable(false), AutoFormatDisable, EditorBrowsableAttribute(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new LayoutItemNestedControlContainer LayoutItemNestedControlContainer {
			get { return base.LayoutItemNestedControlContainer; }
		}
#pragma warning restore 618
		[Browsable(false), AutoFormatDisable, EditorBrowsableAttribute(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new ControlCollection Controls {
			get { return base.Controls; }
		}
		[Browsable(false), AutoFormatDisable, EditorBrowsableAttribute(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new string FieldName { get { return base.FieldName; } set { base.FieldName = value; } }
	}
	public abstract class ColumnLayoutItem : ContentPlaceholderLayoutItem {
		ITemplate template;
		object column;
		public ColumnLayoutItem() : base() { }
		protected internal ColumnLayoutItem(object column) {
			this.column = column;
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ColumnLayoutItemColumnName"),
#endif
 Category("Data"),
		DefaultValue(""), NotifyParentProperty(true), AutoFormatEnable, Localizable(true), RefreshProperties(RefreshProperties.All),
		TypeConverter("DevExpress.Web.Design.Converters.LayoutItemColumnNameConverter, " + AssemblyInfo.SRAssemblyWebDesignFull)]
		public string ColumnName {
			get { return GetStringProperty("ColumnName", ""); }
			set {
				if(value == ColumnName)
					return;
				SetStringProperty("ColumnName", "", value);
				this.column = null;
			}
		}
		public virtual ITemplate Template {
			get { return template; }
			set {
				if(template == value)
					return;
				template = value;
				TemplatesChanged();
			}
		}
		public override void Assign(CollectionItem source) {
			base.Assign(source);
			ColumnLayoutItem item = source as ColumnLayoutItem;
			if(item != null) {
				column = item.column;
				ColumnName = item.ColumnName;
				Template = item.Template;
			}
		}
		protected internal object ColumnInternal {
			get {
				if(column == null && Owner != null && Owner.DataOwner != null && !string.IsNullOrEmpty(ColumnName))
					column = Owner.DataOwner.FindColumnByName(ColumnName);
				return column;
			}
		}
		protected virtual string GetColumnCaption() { return null; }
		protected override string GetDefaultCaption() {
			if(ColumnInternal != null)
				return GetColumnCaption();
			return StringResources.FormLayout_LayoutItemDefaultCaption;
		}
		protected internal override ICollection GetActualControlsCollection() {
			TemplateContainerBase nestedTemplateContainer = FindNestedTemplateContainer();
			if(nestedTemplateContainer != null)
				return GetTemplateContainerActualChildControls(nestedTemplateContainer);
			return base.GetActualControlsCollection();
		}
		protected TemplateContainerBase FindNestedTemplateContainer() {
			return FindNestedTemplateContainer(NestedControlContainer);
		}
		protected TemplateContainerBase FindNestedTemplateContainer(Control parentControl) {
			foreach(Control childControl in parentControl.Controls) {
				if(childControl is TemplateContainerBase)
					return childControl as TemplateContainerBase;
				TemplateContainerBase result = FindNestedTemplateContainer(childControl);
				if(result != null)
					return result;
			}
			return null;
		}
		protected ICollection GetTemplateContainerActualChildControls(TemplateContainerBase nestedTemplateContainer) {
			Control firstNotLiteralChildControl = GetNestedControl(nestedTemplateContainer.Controls, NestedControlSearchMode.ReturnFirstNotLiteralControl);
			return IsTemplateReplacement(firstNotLiteralChildControl) ? firstNotLiteralChildControl.Controls : nestedTemplateContainer.Controls;
		}
		protected virtual bool IsTemplateReplacement(Control control) {
			return false;
		}
		protected override PropertiesBase GetDesignTimeItemEditProperties() {
			var designTimeColumn = ColumnInternal as IDesignTimeCollectionItem;
			return designTimeColumn != null ? designTimeColumn.EditorProperties : null;
		}
	}
	public abstract class CommandLayoutItem : ContentPlaceholderLayoutItem {
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override string Caption {
			get { return string.Empty; }
			set { }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override DefaultBoolean ShowCaption {
			get { return DefaultBoolean.False; }
			set { }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new LayoutItemCaptionSettings CaptionSettings {
			get { return base.CaptionSettings; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new LayoutItemCellStyle CaptionCellStyle {
			get { return base.CaptionCellStyle; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new LayoutItemCaptionStyle CaptionStyle {
			get { return base.CaptionStyle; }
		}
		protected internal override LayoutItemStyle GetLayoutItemStyle() {
			LayoutItemStyle style = base.GetLayoutItemStyle();
			style.CssClass = RenderUtils.CombineCssClasses(style.CssClass, FormLayoutStyles.CommandItemSystemClassName);
			return style;
		}
	}
	public class EditModeCommandLayoutItem : CommandLayoutItem {
		[
#if !SL
	DevExpressWebLocalizedDescription("EditModeCommandLayoutItemShowUpdateButton"),
#endif
 Category("Buttons"), DefaultValue(true), NotifyParentProperty(true)]
		public bool ShowUpdateButton {
			get { return GetBoolProperty("ShowUpdateButton", true); }
			set {
				SetBoolProperty("ShowUpdateButton", true, value);
				LayoutChanged();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("EditModeCommandLayoutItemShowCancelButton"),
#endif
 Category("Buttons"), DefaultValue(true), NotifyParentProperty(true)]
		public bool ShowCancelButton {
			get { return GetBoolProperty("ShowCancelButton", true); }
			set {
				SetBoolProperty("ShowCancelButton", true, value);
				LayoutChanged();
			}
		}
		public override void Assign(CollectionItem source) {
			base.Assign(source);
			EditModeCommandLayoutItem commandItem = source as EditModeCommandLayoutItem;
			if(commandItem != null) {
				ShowUpdateButton = commandItem.ShowUpdateButton;
				ShowCancelButton = commandItem.ShowCancelButton;
			}
		}
	}
	public class LayoutItem : LayoutItemBase, IDataSourceViewSchemaAccessor {
		protected const string Colon = ":";
		LayoutItemNestedControlCollection contentCollection = null;
		LayoutItemCaptionSettings captionSettings = null;
		LayoutItemHelpTextSettings helpTextSettings = null;
		LayoutItemStyle itemStyle = null;
		public LayoutItem()
			: base() {
				Initialize();
		}
		public LayoutItem(string caption)
			: base(caption) {
				Initialize();
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("LayoutItemCaption"),
#endif
		DefaultValue(StringResources.FormLayout_LayoutItemDefaultCaption)]
		public override string Caption {
			get { return base.Caption; }
			set { base.Caption = value; }
		}
		protected internal string FirstControlID {
			get { return GetStringProperty("FirstControlID", ""); }
			set { SetStringProperty("FirstControlID", "", value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("LayoutItemControls"),
#endif
 Browsable(false)]
		public ControlCollection Controls {
			get { return NestedControlContainer.Controls; }
		}
		void Initialize() {
			captionSettings = CreateCaptionSettings();
			helpTextSettings = new LayoutItemHelpTextSettings(this);
		}
		public override void Assign(CollectionItem source) {
			LayoutItem item = source as LayoutItem;
			if (item != null) {
				CaptionSettings.Assign(item.CaptionSettings);
				HelpText = item.HelpText;
				FieldName = item.FieldName;
				RequiredMarkDisplayMode = item.RequiredMarkDisplayMode;
				DataType = item.DataType;
				HelpTextSettings.Assign(item.HelpTextSettings);
				ItemStyle.Assign(item.ItemStyle);
				LayoutItemNestedControlCollection.Assign(item.LayoutItemNestedControlCollection);
			}
			base.Assign(source);
		}
		public virtual Control GetNestedControl() {
			return GetNestedControlCore();
		}
		protected Control GetNestedControlCore(){
			return GetNestedControl(NestedControlSearchMode.ReturnFirstNotLiteralControl);
		}
		protected override string GetItemCaptionCore() {
			string itemCaption = base.GetItemCaptionCore();
			string trimmedCaption = itemCaption.Trim();
			if (!Owner.ShowItemCaptionColon || string.IsNullOrEmpty(trimmedCaption) || trimmedCaption.Equals("&nbsp;", StringComparison.OrdinalIgnoreCase) ||
				itemCaption.EndsWith(Colon))
				return itemCaption;
			else
				return itemCaption + Colon;
		}
		internal bool IsCaptionCellRequired() {
			return GetShowCaption() || FormLayout.ShowItemRequiredMark(this) || FormLayout.ShowItemOptionalMark(this);
		}
		private List<Control> GetDesignTimeNestedControls() {
			if (Owner.DesignTimeNestedControlsStorage != null && Owner.DesignTimeNestedControlsStorage.ContainsKey(this))
				return Owner.DesignTimeNestedControlsStorage[this];
			return new List<Control>();
		}
		private bool GetNestedControlIsRequired() {
			Control editor = GetNestedControl(NestedControlSearchMode.ReturnFirstAllowedControl);
			return editor is ASPxEdit ? ((ASPxEdit)editor).ValidationSettings.RequiredField.IsRequired : false;
		}
		protected override string GetDefaultCaption() {
			return !string.IsNullOrEmpty(FieldName) ? CommonUtils.SplitPascalCaseString(FieldName) : StringResources.FormLayout_LayoutItemDefaultCaption;
		}
		protected internal static List<Type> GetAllowedNestedControlTypes() {
			return new List<Type>(new Type[] {
				typeof(ASPxBinaryImage),
				typeof(ASPxButton),
				typeof(ASPxButtonEdit),
				typeof(ASPxCalendar),
				typeof(ASPxCaptcha),
				typeof(ASPxCheckBox),
				typeof(ASPxCheckBoxList),
				typeof(ASPxColorEdit),
				typeof(ASPxComboBox),
				typeof(ASPxDateEdit),
				typeof(ASPxDropDownEdit),
				typeof(ASPxGridLookup),
				typeof(ASPxHyperLink),
				typeof(ASPxImage),
				typeof(ASPxLabel),
				typeof(ASPxListBox),
				typeof(ASPxMemo),
				typeof(ASPxProgressBar),
				typeof(ASPxRadioButton),
				typeof(ASPxRadioButtonList),
				typeof(ASPxRatingControl),
				typeof(ASPxSpinEdit),
				typeof(ASPxTextBox),
				typeof(ASPxTimeEdit),
				typeof(ASPxTokenBox),
				typeof(ASPxTrackBar),
				typeof(ASPxUploadControl),
				typeof(ASPxValidationSummary)
			});
		}
		protected static bool IsControlTypeAllowed(Type controlType) {
			foreach (Type allowedType in GetAllowedNestedControlTypes())
				if (allowedType.IsAssignableFrom(controlType))
					return true;
			return false;
		}
		protected internal virtual ICollection GetActualControlsCollection() {
			return Owner.DesignTimeEditingMode ? (ICollection)GetDesignTimeNestedControls() : (ICollection)Controls;
		}
		protected internal virtual Control GetNestedControl(NestedControlSearchMode searchMode) {
			return GetNestedControl(GetActualControlsCollection(), searchMode);
		}
		protected internal static Control GetNestedControl(ICollection controls, NestedControlSearchMode searchMode) {
			foreach (Control control in controls) {
				if (searchMode == NestedControlSearchMode.ReturnFirstNotLiteralControl && control is LiteralControl)
					continue;
				if (searchMode == NestedControlSearchMode.ReturnFirstAllowedControl && !IsControlTypeAllowed(control.GetType()))
					continue;
				return control;
			}
			return null;
		}
		protected internal bool ContainsSeveralNotLiteralControls() {
			int controlsCount = 0;
			foreach (Control control in GetActualControlsCollection()) {
				if (control is LiteralControl)
					continue;
				else
					controlsCount++;
				if (controlsCount > 1)
					return true;
			}
			return false;
		}
		protected internal bool ContainsLiteralControlsOnly() {
			return GetNestedControl() == null;
		}
		protected internal bool IsRequired() {
			return RequiredMarkDisplayMode == FieldRequiredMarkMode.Required
				|| RequiredMarkDisplayMode == FieldRequiredMarkMode.Auto && GetNestedControlIsRequired();
		}
		protected internal override LayoutGroupBase ParentGroupInternal {
			get { return base.ParentGroupInternal; }
			set {
				base.ParentGroupInternal = value;
				RecreateNestedControlCollection();
			}
		}
		[Browsable(false), AutoFormatDisable, EditorBrowsableAttribute(EditorBrowsableState.Never),
		PersistenceMode(PersistenceMode.InnerProperty), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public LayoutItemNestedControlCollection LayoutItemNestedControlCollection {
			get {
				if (contentCollection == null) {
					Control owner = FormLayout == null ? new Control() : FormLayout;
					contentCollection = new LayoutItemNestedControlCollection(owner);
				}
				return contentCollection;
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("LayoutItemLayoutItemNestedControlContainer"),
#endif
		Obsolete("This property is now obsolete. Use the LayoutItem.Controls property instead.")]
		public LayoutItemNestedControlContainer LayoutItemNestedControlContainer
		{
			get { return NestedControlContainer; }
		}
		protected internal override bool GetShowCaption() {
			if(ParentGroupInternal is TabbedLayoutGroup && ShowCaption == DefaultBoolean.Default)
				return false;
			else
				return base.GetShowCaption();
		}
		protected override IStateManager[] GetStateManagedObjects() {
			return ViewStateUtils.GetMergedStateManagedObjects(base.GetStateManagedObjects(),
				new IStateManager[] { CaptionSettings, ItemStyle, HelpTextSettings });
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("LayoutItemFieldName"),
#endif
 Category("Data"),
		DefaultValue(""), NotifyParentProperty(true), AutoFormatEnable, Localizable(true), RefreshProperties(RefreshProperties.All),
		TypeConverter("DevExpress.Web.Design.Converters.LayoutItemFieldNameConverter, " + AssemblyInfo.SRAssemblyWebDesignFull)]
		public string FieldName {
			get { return GetStringProperty("FieldName", ""); }
			set { SetStringProperty("FieldName", "", value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("LayoutItemHelpTextSettings"),
#endif
		RefreshProperties(RefreshProperties.All), NotifyParentProperty(true), AutoFormatEnable,
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content), Category("Layout"),
		PersistenceMode(PersistenceMode.InnerProperty)]
		public LayoutItemHelpTextSettings HelpTextSettings {
			get { return this.helpTextSettings; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("LayoutItemHelpText"),
#endif
		DefaultValue(""), NotifyParentProperty(true), AutoFormatEnable, Localizable(true)]
		public string HelpText {
			get { return GetStringProperty("HelpText", string.Empty); }
			set { 
				SetStringProperty("HelpText", string.Empty, value);
				LayoutChanged();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("LayoutItemRequiredMarkDisplayMode"),
#endif
		DefaultValue(FieldRequiredMarkMode.Auto), NotifyParentProperty(true),  Category("Appearance"), AutoFormatDisable]
		public FieldRequiredMarkMode RequiredMarkDisplayMode {
			get { return (FieldRequiredMarkMode)GetEnumProperty("RequiredMarkDisplayMode", FieldRequiredMarkMode.Auto); }
			set {
				SetEnumProperty("RequiredMarkDisplayMode", FieldRequiredMarkMode.Auto, value);
				LayoutChanged();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("LayoutItemCaptionSettings"),
#endif
		RefreshProperties(RefreshProperties.All), NotifyParentProperty(true), AutoFormatEnable,
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content), Category("Layout"),
		PersistenceMode(PersistenceMode.InnerProperty)]
		public LayoutItemCaptionSettings CaptionSettings {
			get { return this.captionSettings; }
		}
		protected LayoutItemStyle ItemStyle {
			get {
				if (this.itemStyle == null)
					this.itemStyle = new LayoutItemStyle();
				return this.itemStyle;
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("LayoutItemBorderTop"),
#endif
		Category("Appearance"), NotifyParentProperty(true), AutoFormatEnable,
		PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual Border BorderTop {
			get { return ItemStyle.BorderTop; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("LayoutItemBorderRight"),
#endif
		Category("Appearance"), NotifyParentProperty(true), AutoFormatEnable,
		PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual Border BorderRight {
			get { return ItemStyle.BorderRight; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("LayoutItemBorderLeft"),
#endif
		Category("Appearance"), NotifyParentProperty(true), AutoFormatEnable,
		PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual Border BorderLeft {
			get { return ItemStyle.BorderLeft; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("LayoutItemBorderBottom"),
#endif
		Category("Appearance"), NotifyParentProperty(true), AutoFormatEnable,
		PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual Border BorderBottom {
			get { return ItemStyle.BorderBottom; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("LayoutItemPaddings"),
#endif
		Category("Appearance"), NotifyParentProperty(true), AutoFormatEnable,
		PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual Paddings Paddings {
			get { return ItemStyle.Paddings; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("LayoutItemBackgroundImage"),
#endif
		Category("Appearance"), NotifyParentProperty(true), AutoFormatEnable,
		PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual BackgroundImage BackgroundImage {
			get { return ItemStyle.BackgroundImage; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("LayoutItemBorder"),
#endif
		Category("Appearance"), NotifyParentProperty(true), AutoFormatEnable,
		PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual BorderWrapper Border {
			get { return ItemStyle.Border; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("LayoutItemCssClass"),
#endif
		Category("Appearance"), AutoFormatEnable, NotifyParentProperty(true), DefaultValue(""), Localizable(false)]
		public string CssClass {
			get { return ItemStyle.CssClass; }
			set { ItemStyle.CssClass = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("LayoutItemBackColor"),
#endif
		Category("Appearance"), AutoFormatEnable, NotifyParentProperty(true), DefaultValue(typeof(Color), ""),
		TypeConverter(typeof(WebColorConverter))]
		public Color BackColor {
			get { return ItemStyle.BackColor; }
			set { ItemStyle.BackColor = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("LayoutItemHelpTextStyle"),
#endif
		Category("Styles"), NotifyParentProperty(true),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable]
		public HelpTextStyle HelpTextStyle {
			get { return ItemStyle.HelpText; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("LayoutItemCaptionCellStyle"),
#endif
		Category("Styles"), NotifyParentProperty(true),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable]
		public LayoutItemCellStyle CaptionCellStyle
		{
			get { return ItemStyle.CaptionCell; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("LayoutItemNestedControlCellStyle"),
#endif
		Category("Styles"), NotifyParentProperty(true),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable]
		public LayoutItemCellStyle NestedControlCellStyle
		{
			get { return ItemStyle.NestedControlCell; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("LayoutItemCaptionStyle"),
#endif
		Category("Styles"), NotifyParentProperty(true),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable]
		public LayoutItemCaptionStyle CaptionStyle
		{
			get { return ItemStyle.Caption; }
		}
		protected internal AppearanceStyleBase GetInternalNestedControlTableStyle() {
			return GetLayoutItemStyle().InternalNestedControlTable;
		}
		protected internal HelpTextStyle GetLayoutItemHelpTextStyle() {
			HelpTextStyle style = GetLayoutItemStyle().HelpText;
			List<string> cssClasses = new List<string>();
			cssClasses.AddRange(FormLayoutStyles.GetAlignmentClassNames(HelpTextSettings.GetHorizontalAlign(), HelpTextSettings.GetVerticalAlign()));
			cssClasses.Add(FormLayoutStyles.GetHelpTextClassName(HelpTextSettings.GetPosition()));
			cssClasses.Add(style.CssClass);
			style.CssClass = RenderUtils.CombineCssClasses(cssClasses.ToArray());
			return style;
		}
		protected internal virtual LayoutItemStyle GetLayoutItemStyle() {
			LayoutItemStyle style = FormLayout.GetLayoutItemStyle();
			style.CopyFrom(ItemStyle);
			List<string> cssClasses = new List<string>(); 
			cssClasses.AddRange(FormLayoutStyles.GetLayoutItemSystemClassNames(CaptionSettings.GetLocation()));
			cssClasses.Add(FormLayoutRenderHelper.GetItemTypeClassName(this));
			if(RequiresEdgeHelpTextClassName())
				cssClasses.Add("dxflItemWithEdgeHelpTextSys");
			cssClasses.Add(style.CssClass);
			style.CssClass = RenderUtils.CombineCssClasses(cssClasses.ToArray());
			return style;
		}
		protected internal AppearanceStyleBase GetItemTableStyle() {
			return GetLayoutItemStyle().ItemTable;
		}
		protected internal AppearanceStyle GetCaptionCellStyle() {
			LayoutItemCellStyle style = GetLayoutItemStyle().CaptionCell;
			List<string> cssClasses = new List<string>(FormLayoutStyles.GetAlignmentClassNames(CaptionSettings.GetHorizontalAlign(), CaptionSettings.GetVerticalAlign()));
			cssClasses.Add(style.CssClass);
			style.CssClass = RenderUtils.CombineCssClasses(cssClasses.ToArray());
			return style;
		}
		protected internal AppearanceStyle GetNestedControlCellStyle() {
			return GetLayoutItemStyle().NestedControlCell;
		}
		protected internal LayoutItemCaptionStyle GetCaptionStyle() {
			return GetLayoutItemStyle().Caption;
		}
		protected bool RequiresEdgeHelpTextClassName() {
			return ParentGroupInternal is LayoutGroup && !((LayoutGroup)ParentGroupInternal).UseDefaultPaddings
				&& HasHelpTextInEdgePosition();
		}
		protected bool HasHelpTextInEdgePosition() {
			if(string.IsNullOrEmpty(HelpText))
				return false;
			if(!GetShowCaption())
				return true;
			return CaptionSettings.GetLocation().ToString() != HelpTextSettings.GetPosition().ToString();
		}
		protected void RecreateNestedControlCollection() {
			LayoutItemNestedControlCollection oldNestedControlCollection = LayoutItemNestedControlCollection;
			this.contentCollection = null;
			foreach (LayoutItemNestedControlContainer editor in oldNestedControlCollection)
				LayoutItemNestedControlCollection.Add(editor);
		}
		protected internal LayoutItemNestedControlContainer NestedControlContainer {
			get {
				if (LayoutItemNestedControlCollection.Count == 0)
					LayoutItemNestedControlCollection.Add(new LayoutItemNestedControlContainer());
				return LayoutItemNestedControlCollection[0];
			}
		}
		protected internal Type DataType {
			get { return (Type)GetObjectProperty("DataType", null); }
			set { 
				if(DataType != value)
					SetDataTypeCore(value); 
			}
		}
		protected virtual void SetDataTypeCore(Type dataType){
			SetObjectProperty("DataType", null, dataType);
		}
		protected internal virtual void EnsureNestedControl() {
			if (DataType != null && GetNestedControl() == null) {
				Control nestedControl = CreateNestedControl();
				EnsureNestedControlIdAssigned(nestedControl);
				Controls.Add(nestedControl);
			}
		}
		protected virtual LayoutItemCaptionSettings CreateCaptionSettings() {
			return new LayoutItemCaptionSettings(this);
		}
		protected virtual Control CreateNestedControl() {
			var control = NestedControlHelper.CreateControlByDataType(DataType);
			if(control is ASPxWebControl)
				((ASPxWebControl)control).ParentSkinOwner = Owner;
			return control;
		}
		protected internal virtual string GetNestedControlID() {
			Control editorControl = GetNestedControl(NestedControlSearchMode.ReturnFirstAllowedControl);
			if(editorControl is ASPxEdit)
				return (editorControl as ASPxEdit).GetAssociatedControlID();
			return string.Empty;
		}
		protected internal virtual void EnsureNestedControlIdAssigned(Control nestedControl) {
			if(string.IsNullOrEmpty(nestedControl.ID) && !string.IsNullOrEmpty(FormLayout.ID))
				nestedControl.ID = FormLayout.GetVacantItemNestedControlID();
		}
		protected internal virtual bool NeedAdditionalTableForRender() {
			return false;
		}
		object IDataSourceViewSchemaAccessor.DataSourceViewSchema {
			get {
				var accessor = FormLayout as IDataSourceViewSchemaAccessor;
				return accessor != null ? accessor.DataSourceViewSchema : null;
			}
			set { }
		}
		protected override string GetDesignTimeCaption() {
			var nestedControl = GetNestedControl();
			if(nestedControl != null) {
				var caption = !string.IsNullOrEmpty(Caption) ? Caption : "(" + base.GetDesignTimeCaption() + ")";
				return caption + " (" + nestedControl.GetType().Name + ")";
			}
			return !string.IsNullOrEmpty(Caption) ? Caption : base.GetDesignTimeCaption();
		}
	}
	public class EmptyLayoutItem : LayoutItemBase {
		EmptyLayoutItemStyle emptyItemStyle = null;
		public EmptyLayoutItem()
			: base() {
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override DefaultBoolean ShowCaption {
			get { return DefaultBoolean.False; }
			set { }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override string Caption {
			get { return string.Empty; }
			set { }
		}
		protected override IStateManager[] GetStateManagedObjects() {
			return ViewStateUtils.GetMergedStateManagedObjects(base.GetStateManagedObjects(),
				new IStateManager[] { EmptyItemStyle });
		}
		public override void Assign(CollectionItem source) {
			EmptyLayoutItem emptyLayoutItem = source as EmptyLayoutItem;
			if (emptyLayoutItem != null)
				EmptyItemStyle.Assign(emptyLayoutItem.EmptyItemStyle);
			base.Assign(source);
		}
		protected EmptyLayoutItemStyle EmptyItemStyle {
			get {
				if (this.emptyItemStyle == null)
					this.emptyItemStyle = new EmptyLayoutItemStyle();
				return this.emptyItemStyle;
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("EmptyLayoutItemBorderTop"),
#endif
		Category("Appearance"), NotifyParentProperty(true), AutoFormatEnable,
		PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual Border BorderTop {
			get { return EmptyItemStyle.BorderTop; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("EmptyLayoutItemBorderRight"),
#endif
		Category("Appearance"), NotifyParentProperty(true), AutoFormatEnable,
		PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual Border BorderRight {
			get { return EmptyItemStyle.BorderRight; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("EmptyLayoutItemBorderLeft"),
#endif
		Category("Appearance"), NotifyParentProperty(true), AutoFormatEnable,
		PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual Border BorderLeft {
			get { return EmptyItemStyle.BorderLeft; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("EmptyLayoutItemBorderBottom"),
#endif
		Category("Appearance"), NotifyParentProperty(true), AutoFormatEnable,
		PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual Border BorderBottom {
			get { return EmptyItemStyle.BorderBottom; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("EmptyLayoutItemBackgroundImage"),
#endif
		Category("Appearance"), NotifyParentProperty(true), AutoFormatEnable,
		PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual BackgroundImage BackgroundImage {
			get { return EmptyItemStyle.BackgroundImage; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("EmptyLayoutItemBorder"),
#endif
		Category("Appearance"), NotifyParentProperty(true), AutoFormatEnable,
		PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual BorderWrapper Border {
			get { return EmptyItemStyle.Border; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("EmptyLayoutItemCssClass"),
#endif
		Category("Appearance"), AutoFormatEnable, NotifyParentProperty(true), DefaultValue(""), Localizable(false)]
		public string CssClass {
			get { return EmptyItemStyle.CssClass; }
			set { EmptyItemStyle.CssClass = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("EmptyLayoutItemBackColor"),
#endif
		Category("Appearance"), AutoFormatEnable, NotifyParentProperty(true), DefaultValue(typeof(Color), ""),
		TypeConverter(typeof(WebColorConverter))]
		public Color BackColor {
			get { return EmptyItemStyle.BackColor; }
			set { EmptyItemStyle.BackColor = value; }
		}
		protected internal EmptyLayoutItemStyle GetEmptyLayoutItemStyle() {
			EmptyLayoutItemStyle style = FormLayout.GetEmptyLayoutItemStyle();
			style.CopyFrom(EmptyItemStyle);
			return style;
		}
	}
	public class LayoutGroup : LayoutGroupBase {
		const FormLayoutVerticalAlign DefaultVerticalAlign = FormLayoutVerticalAlign.Top;
		LayoutGroupStyle groupStyle = null;
		LayoutGroupBoxStyle groupBoxStyle = null;
		protected override string GetDefaultCaption() {
			return StringResources.FormLayout_LayoutGroupDefaultCaption;
		}
		public LayoutGroup()
			: base() {
		}
		public LayoutGroup(string caption)
			: base(caption) {
		}
		protected internal LayoutGroup(FormLayoutProperties owner)
			: base(owner) {
		}
		protected internal LayoutGroup(ASPxFormLayout formLayout)
			: this(formLayout.Properties) {
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("LayoutGroupCaption"),
#endif
		DefaultValue(StringResources.FormLayout_LayoutGroupDefaultCaption)]
		public override string Caption {
			get { return base.Caption; }
			set { base.Caption = value; }
		}
		public override void Assign(CollectionItem source) {
			LayoutGroup layoutGroup = source as LayoutGroup;
			if (layoutGroup != null) {
				if (ColCount < layoutGroup.ColCount)
					ColCount = layoutGroup.ColCount;
				GroupBoxDecoration = layoutGroup.GroupBoxDecoration;
				UseDefaultPaddings = layoutGroup.UseDefaultPaddings;
				GroupBoxStyle.Assign(layoutGroup.GroupBoxStyle);
				GroupStyle.Assign(layoutGroup.GroupStyle);
				AlignItemCaptions = layoutGroup.AlignItemCaptions;
			}
			base.Assign(source);
			if (layoutGroup != null && ColCount != layoutGroup.ColCount)
				ColCount = layoutGroup.ColCount;
		}
		protected internal override FormLayoutVerticalAlign GetVerticalAlign() {
			FormLayoutVerticalAlign vAlign = base.GetVerticalAlign();
#pragma warning disable 618
			return (vAlign == FormLayoutVerticalAlign.NoSet || vAlign == FormLayoutVerticalAlign.NotSet) ? DefaultVerticalAlign : vAlign;
#pragma warning restore 618
		}
		protected internal GroupBoxDecoration GetGroupBoxDecoration(){
			if(GroupBoxDecoration == GroupBoxDecoration.Default)
				return GetDefaultGroupBoxDecoration();
			return GroupBoxDecoration;
		}
		protected GroupBoxDecoration GetDefaultGroupBoxDecoration() {
			if(IsRootGroup() || ParentGroupInternal is TabbedLayoutGroup)
				return GroupBoxDecoration.None;
			return GroupBoxDecoration.Box;
		}
		protected override IStateManager[] GetStateManagedObjects() {
			return ViewStateUtils.GetMergedStateManagedObjects(base.GetStateManagedObjects(),
				new IStateManager[] { GroupStyle, GroupBoxStyle });
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("LayoutGroupAlignItemCaptions"),
#endif
		DefaultValue(true), NotifyParentProperty(true), AutoFormatEnable, Category("Behavior")]
		public bool AlignItemCaptions {
			get { return GetBoolProperty("AlignItemCaptions", true); }
			set { SetBoolProperty("AlignItemCaptions", true, value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("LayoutGroupColCount"),
#endif
 Category("Layout"),
		DefaultValue(1), NotifyParentProperty(true), AutoFormatEnable]
		public int ColCount {
			get { return GetIntProperty("ColCount", 1); }
			set {
				CommonUtils.CheckNegativeOrZeroValue((double)value, "ColCount");				
				int maxColSpan = -1;
				foreach(LayoutItemBase item in Items)
					maxColSpan = item.ColSpan > maxColSpan ? item.ColSpan : maxColSpan;
				CommonUtils.CheckMinimumValue((double)value, maxColSpan, "ColCount");				
				SetIntProperty("ColCount", 1, value);
				LayoutChanged();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("LayoutGroupGroupBoxDecoration"),
#endif
 Category("Layout"),
		DefaultValue(GroupBoxDecoration.Default), NotifyParentProperty(true), AutoFormatEnable]
		public GroupBoxDecoration GroupBoxDecoration {
			get { return (GroupBoxDecoration)GetEnumProperty("GroupBoxDecoration", GroupBoxDecoration.Default); }
			set {
				if (GroupBoxDecoration != value) {
					SetEnumProperty("GroupBoxDecoration", GroupBoxDecoration.Default, value);
					LayoutChanged();
				}
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("LayoutGroupUseDefaultPaddings"),
#endif
		DefaultValue(true), NotifyParentProperty(true), AutoFormatEnable, Category("Layout")]
		public bool UseDefaultPaddings {
			get { return GetBoolProperty("UseDefaultPaddings", true); }
			set { SetBoolProperty("UseDefaultPaddings", true, value); }
		}
		protected LayoutGroupStyle GroupStyle {
			get {
				if (this.groupStyle == null)
					this.groupStyle = new LayoutGroupStyle();
				return this.groupStyle;
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("LayoutGroupBorderTop"),
#endif
		Category("Appearance"), NotifyParentProperty(true), AutoFormatEnable,
		PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual Border BorderTop {
			get { return GroupStyle.BorderTop; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("LayoutGroupBorderRight"),
#endif
		Category("Appearance"), NotifyParentProperty(true), AutoFormatEnable,
		PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual Border BorderRight {
			get { return GroupStyle.BorderRight; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("LayoutGroupBorderLeft"),
#endif
		Category("Appearance"), NotifyParentProperty(true), AutoFormatEnable,
		PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual Border BorderLeft {
			get { return GroupStyle.BorderLeft; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("LayoutGroupBorderBottom"),
#endif
		Category("Appearance"), NotifyParentProperty(true), AutoFormatEnable,
		PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual Border BorderBottom {
			get { return GroupStyle.BorderBottom; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("LayoutGroupBackgroundImage"),
#endif
		Category("Appearance"), NotifyParentProperty(true), AutoFormatEnable,
		PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual BackgroundImage BackgroundImage {
			get { return GroupStyle.BackgroundImage; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("LayoutGroupBorder"),
#endif
		Category("Appearance"), NotifyParentProperty(true), AutoFormatEnable,
		PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual BorderWrapper Border {
			get { return GroupStyle.Border; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("LayoutGroupPaddings"),
#endif
		Category("Appearance"), NotifyParentProperty(true), AutoFormatEnable,
		PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual Paddings Paddings {
			get { return GroupStyle.Paddings; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("LayoutGroupCssClass"),
#endif
		Category("Appearance"), AutoFormatEnable, NotifyParentProperty(true), DefaultValue(""), Localizable(false)]
		public string CssClass
		{
			get { return GroupStyle.CssClass; }
			set { GroupStyle.CssClass = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("LayoutGroupBackColor"),
#endif
		Category("Appearance"), AutoFormatEnable, NotifyParentProperty(true), DefaultValue(typeof(Color), ""),
		TypeConverter(typeof(WebColorConverter))]
		public Color BackColor
		{
			get { return GroupStyle.BackColor; }
			set { GroupStyle.BackColor = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("LayoutGroupGroupBoxStyle"),
#endif
		Category("Styles"), NotifyParentProperty(true),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable]
		public LayoutGroupBoxStyle GroupBoxStyle {
			get {
				if (this.groupBoxStyle == null)
					this.groupBoxStyle = new LayoutGroupBoxStyle();
				return this.groupBoxStyle;
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("LayoutGroupCellStyle"),
#endif
		Category("Styles"), NotifyParentProperty(true),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable]
		public LayoutGroupCellStyle CellStyle {
			get { return GroupStyle.Cell; }
		}
		protected internal LayoutGroupBoxStyle GetGroupBoxStyle() {
			LayoutGroupBoxStyle style = FormLayout.GetLayoutGroupBoxStyle();
			style.CopyFrom(GroupBoxStyle);
			if (GetGroupBoxDecoration() == GroupBoxDecoration.HeadingLine)
				style.CssClass = RenderUtils.CombineCssClasses(style.CssClass, FormLayoutStyles.HeadingLineGroupBoxSystemClassName);
			style.CssClass = RenderUtils.CombineCssClasses(style.CssClass, FormLayoutStyles.GroupBoxSystemClassName);
			return style;
		}
		protected internal LayoutGroupStyle GetGroupStyle() {
			LayoutGroupStyle style = FormLayout.GetLayoutGroupStyle();
			style.CopyFrom(GroupStyle);
			return style;
		}
		protected internal AppearanceStyle GetCellStyle(LayoutItemBase childItem) {
			LayoutGroupCellStyle style = GetGroupStyle().Cell;
			List<string> classNames = new List<string>(FormLayoutStyles.GetAlignmentClassNames(childItem.GetHorizontalAlign(), childItem.GetVerticalAlign()));
			classNames.Add(style.CssClass);
			style.CssClass = RenderUtils.CombineCssClasses(classNames.ToArray());
			style.CopyFrom(CellStyle);
			style.CopyFrom(childItem.ParentContainerStyle);
			return style;
		}
		protected internal AppearanceStyleBase GetGroupTableStyle() {
			return GetGroupStyle().GroupTable;
		}
	}
	public class TabbedLayoutGroup : LayoutGroupBase {
		const string PageControlCssPrefix = "PC_";
		ASPxPageControl pageControl = null;
		TabbedLayoutGroupTabPageSettings settingsTabPages = null;
		TabbedLayoutGroupTabPageImage tabPagesImages  = null;
		TabbedLayoutGroupTabPageStyles tabPagesStyles  = null;
		public TabbedLayoutGroup()
			: base() {
		}
		public TabbedLayoutGroup(string caption)
			: base(caption) {
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("TabbedLayoutGroupCaption"),
#endif
		DefaultValue(StringResources.FormLayout_TabbedGroupDefaultCaption)]
		public override string Caption {
			get { return base.Caption; }
			set { base.Caption = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("TabbedLayoutGroupBackgroundImage"),
#endif
		Category("Appearance"), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public BackgroundImage BackgroundImage {
			get { return PageControl.BackgroundImage; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("TabbedLayoutGroupBorder"),
#endif
		Category("Appearance"), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public  BorderWrapper Border {
			get { return PageControl.Border; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("TabbedLayoutGroupBorderLeft"),
#endif
		Category("Appearance"), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public Border BorderLeft {
			get { return PageControl.BorderLeft; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("TabbedLayoutGroupBorderTop"),
#endif
		Category("Appearance"), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public Border BorderTop {
			get { return PageControl.BorderTop; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("TabbedLayoutGroupBorderRight"),
#endif
		Category("Appearance"), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public Border BorderRight {
			get { return PageControl.BorderRight; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("TabbedLayoutGroupBorderBottom"),
#endif
		Category("Appearance"), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public Border BorderBottom {
			get { return PageControl.BorderBottom; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("TabbedLayoutGroupPaddings"),
#endif
		Category("Layout"), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public Paddings Paddings {
			get { return PageControl.Paddings; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("TabbedLayoutGroupTabAlign"),
#endif
		Category("Appearance"), DefaultValue(TabAlign.Left), AutoFormatEnable]
		public TabAlign TabAlign {
			get { return PageControl.TabAlign; }
			set { PageControl.TabAlign = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("TabbedLayoutGroupTabPosition"),
#endif
		Category("Appearance"), DefaultValue(TabPosition.Top), AutoFormatDisable]
		public TabPosition TabPosition {
			get { return PageControl.TabPosition; }
			set { PageControl.TabPosition = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("TabbedLayoutGroupTabSpacing"),
#endif
		Category("Layout"), AutoFormatEnable, DefaultValue(typeof(Unit), "")]
		public Unit TabSpacing {
			get { return PageControl.TabSpacing; }
			set { PageControl.TabSpacing = value; }
		}
		protected override string GetDefaultCaption() {
			return StringResources.FormLayout_TabbedGroupDefaultCaption;
		}
		public override void Assign(CollectionItem source) {
			TabbedLayoutGroup tabbedGroup = source as TabbedLayoutGroup;
			if(tabbedGroup != null) {
				ActiveTabIndex = tabbedGroup.ActiveTabIndex;
				BackgroundImage.Assign(tabbedGroup.BackgroundImage);
				Border.Assign(tabbedGroup.Border);
				BorderBottom.Assign(tabbedGroup.BorderBottom);
				BorderLeft.Assign(tabbedGroup.BorderLeft);
				BorderRight.Assign(tabbedGroup.BorderRight);
				BorderTop.Assign(tabbedGroup.BorderTop);
				ClientInstanceName = tabbedGroup.ClientInstanceName;
				ClientSideEvents.Assign(tabbedGroup.ClientSideEvents);
				Paddings.Assign(tabbedGroup.Paddings);
				ShowGroupDecoration = tabbedGroup.ShowGroupDecoration;
				TabAlign = tabbedGroup.TabAlign;
				TabPosition = tabbedGroup.TabPosition;
				TabSpacing = tabbedGroup.TabSpacing;
				SettingsTabPages.Assign(tabbedGroup.SettingsTabPages);
				Images.Assign(tabbedGroup.Images);
				Styles.Assign(tabbedGroup.Styles);
			}
			base.Assign(source);
		}
		internal string PageControlId {
			get { return PageControlCssPrefix + Path; }
		}
#if !SL
	[DevExpressWebLocalizedDescription("TabbedLayoutGroupPageControl")]
#endif
		public ASPxPageControl PageControl {
			get {
				if(pageControl == null)
					pageControl = new ASPxPageControl() {
						IsNotBindingContainer = true, ActiveTabIndex = ActiveTabIndexInternal
					};
				return pageControl;
			}
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override DefaultBoolean ShowCaption {
			get { return DefaultBoolean.False; }
			set { }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("TabbedLayoutGroupActiveTabIndex"),
#endif
		DefaultValue(0), NotifyParentProperty(true), AutoFormatEnable, Category("Behavior")]
		public int ActiveTabIndex {
			get {
				if(FormLayout != null && FormLayout.DesignMode)
					return ActiveTabIndexInternal;
				return PageControl.ActiveTabIndex == -1 ? ActiveTabIndexInternal : PageControl.ActiveTabIndex;
			}
			set {
				ActiveTabIndexInternal = value;
				PageControl.ActiveTabIndex = value;
			}
		}
		protected int ActiveTabIndexInternal {
			get { return GetIntProperty("ActiveTabIndex", 0); }
			set { SetIntProperty("ActiveTabIndex", 0, value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("TabbedLayoutGroupClientInstanceName"),
#endif
		Category("Client-Side"), DefaultValue(""), Localizable(false), AutoFormatDisable]
		public string ClientInstanceName {
			get { return PageControl.ClientInstanceName; }
			set { PageControl.ClientInstanceName = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("TabbedLayoutGroupClientSideEvents"),
#endif
		Category("Client-Side"), PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		AutoFormatDisable, MergableProperty(false)]
		public TabControlClientSideEvents ClientSideEvents {
			get { return PageControl.ClientSideEvents; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("TabbedLayoutGroupShowGroupDecoration"),
#endif
 Category("Layout"),
		DefaultValue(true), NotifyParentProperty(true)]
		public bool ShowGroupDecoration {
			get { return GetBoolProperty("ShowGroupDecoration", true); }
			set {
				if (ShowGroupDecoration != value) {
					SetBoolProperty("ShowGroupDecoration", true, value);
					LayoutChanged();
				}
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("TabbedLayoutGroupSettingsTabPages"),
#endif
		Category("Settings"), AutoFormatDisable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public TabbedLayoutGroupTabPageSettings SettingsTabPages {
			get {
				return this.settingsTabPages = this.settingsTabPages ?? new TabbedLayoutGroupTabPageSettings(PageControl);
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("TabbedLayoutGroupImages"),
#endif
		Category("Appearance"), AutoFormatDisable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public TabbedLayoutGroupTabPageImage Images
		{
			get
			{
				return this.tabPagesImages = this.tabPagesImages ?? new TabbedLayoutGroupTabPageImage(PageControl);
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("TabbedLayoutGroupStyles"),
#endif
		Category("Styles"), AutoFormatDisable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public TabbedLayoutGroupTabPageStyles Styles
		{
			get
			{
				return this.tabPagesStyles = this.tabPagesStyles ?? new TabbedLayoutGroupTabPageStyles(PageControl);
			}
		}
	}
	[Editor("DevExpress.Web.Design.CommonDesignerEditor, " + AssemblyInfo.SRAssemblyWebDesignFull, typeof(System.Drawing.Design.UITypeEditor)),
	TypeConverter(typeof(DevExpress.Utils.Design.UniversalCollectionTypeConverter))]
	public class LayoutItemCollection : Collection {
		LayoutItemCollection cachedCollection = null;
		public LayoutItemCollection()
			: base() {
		}
		public LayoutItemCollection(IWebControlObject owner)
			: base(owner) {
		}
#if !SL
	[DevExpressWebLocalizedDescription("LayoutItemCollectionItem")]
#endif
		public LayoutItemBase this[int index] {
			get { return (GetItem(index) as LayoutItemBase); }
		}
		protected internal LayoutItemBase GetVisibleItemOrGroup(int index) {
			return base.GetVisibleItem(index) as LayoutItemBase;
		}
		public LayoutItemBase Add(LayoutItemBase item) {
			return (LayoutItemBase)base.Add(item);
		}
		public T Add<T>(string caption) where T : LayoutItemBase, new() {
			return Add<T>(caption, string.Empty);
		}
		public T Add<T>(string caption, string name) where T : LayoutItemBase, new() {
			LayoutItemBase item = new T();
			item.Caption = caption;
			item.Name = name;
			Add(item);
			return item as T;
		}
		protected override Type GetKnownType() {
			return typeof(LayoutItemBase);
		}
		protected override Type[] GetKnownTypes() {
			return new Type[] { 
				typeof(LayoutItem),
				typeof(LayoutGroup),
				typeof(TabbedLayoutGroup),
				typeof(EmptyLayoutItem)
			};
		}
		protected FormLayoutProperties GetOwnerProperties() {
			return Owner is LayoutItemBase ? ((LayoutItemBase)Owner).Owner : null;
		}
		protected override void OnInsert(int index, object value) {
			base.OnInsert(index, value);
			(value as LayoutItemBase).ParentGroupInternal = this.Owner as LayoutGroupBase;
		}
		protected override void OnChanged() {
			if (Owner != null)
				Owner.LayoutChanged();
		}
		protected LayoutItem FindItemByNestedControlID(string id) {
			return ((LayoutGroup)Owner).FindItemOrGroupByCondition(delegate(LayoutItemBase item) {
				return item is LayoutItem && ((LayoutItem)item).GetNestedControl() != null && ((LayoutItem)item).GetNestedControl().ID == id;
			}) as LayoutItem;
		}
		protected void BeforeLoadItemsFromViewState() {
			if(IsSavedAll) {
				cachedCollection = new LayoutGroup(new ASPxFormLayout()).Items;
				if(Owner is LayoutGroup)
					((LayoutGroup)cachedCollection.Owner).ColCount = ((LayoutGroup)Owner).ColCount;
				cachedCollection.Assign(this);
			}
		}
		protected void AfterLoadItemsFromViewState() {
			if(IsSavedAll) {
				((LayoutGroupBase)Owner).ForEach(delegate(LayoutItemBase item) {
					LayoutItem layoutItem = item as LayoutItem;
					if(layoutItem != null && !string.IsNullOrEmpty(layoutItem.FirstControlID)) {
						LayoutItem cachedItem = cachedCollection.FindItemByNestedControlID(layoutItem.FirstControlID);
						if(cachedItem != null)
							foreach(Control control in new List<Control>(cachedItem.Controls.Cast<Control>()))
								layoutItem.Controls.Add(control);
					}
				});
			}
		}
		protected override void LoadItemsFromViewState(Pair savedStatePair) {
			BeforeLoadItemsFromViewState();
			base.LoadItemsFromViewState(savedStatePair);
			AfterLoadItemsFromViewState();
		}
		protected override object SaveViewState() {
			if (SaveAll) {
				((LayoutGroupBase)Owner).ForEach(delegate(LayoutItemBase item) {
					LayoutItem layoutItem = item as LayoutItem;
					if (layoutItem != null && layoutItem.GetNestedControl() != null) {
						layoutItem.FirstControlID = layoutItem.GetNestedControl().ID;
					}
				});
			}
			return base.SaveViewState();
		}
	}
	public abstract class LayoutGroupBase : LayoutItemBase {
		LayoutItemCollection items = null;
		LayoutItemCaptionSettings itemsCaptionSettings = null;
		LayoutItemHelpTextSettings itemsHelpTextSettings = null;
		LayoutGroupItemSettings itemsSettings = null;
		public LayoutGroupBase()
			: base() {
				Initialize();
		}		
		public LayoutGroupBase(string caption)
			: base(caption) {
				Initialize();
		}
		protected LayoutGroupBase(FormLayoutProperties owner)
			: base(owner) {
				Initialize();
		}
		protected LayoutGroupBase(ASPxFormLayout formLayout)
			: this(formLayout.Properties) {
		}
		protected virtual void Initialize() {
			this.itemsCaptionSettings = new LayoutItemCaptionSettings(this);
			this.itemsHelpTextSettings = new LayoutItemHelpTextSettings(this);
			this.itemsSettings = new LayoutGroupItemSettings(this);
		}
		public override void Assign(CollectionItem source) {
			LayoutGroupBase group = source as LayoutGroupBase;
			if (group != null) {
				SettingsItemCaptions.Assign(group.SettingsItemCaptions);
				SettingsItemHelpTexts.Assign(group.SettingsItemHelpTexts);
				SettingsItems.Assign(group.SettingsItems);
				Items.Assign(group.Items);
			}
			base.Assign(source);
		}
		public void ForEach(Action<LayoutItemBase> method) {
			foreach (LayoutItemBase child in Items) {
				method(child);
				if (child is LayoutGroupBase)
					(child as LayoutGroupBase).ForEach(method);
			}
		}
		public LayoutItemBase FindItemOrGroupByName(string name) {
			return FindItemOrGroupByCondition(delegate(LayoutItemBase item) {
				return item.Name == name;
			});
		}
		public LayoutItem FindItemByFieldName(string fieldName) {
			return FindItemOrGroupByCondition(delegate(LayoutItemBase item) {
				return item is LayoutItem && ((LayoutItem)item).FieldName == fieldName;
			}) as LayoutItem;
		}
		public Control FindNestedControlByFieldName(string fieldName) {
			LayoutItem layoutItem = FindItemByFieldName(fieldName);
			return layoutItem != null ? layoutItem.GetNestedControl() : null;
		}
		public object GetNestedControlValueByFieldName(string fieldName) {
			Control control = FindNestedControlByFieldName(fieldName);
			return control is ASPxEditBase ? ((ASPxEditBase)control).Value : null;
		}
		[EditorBrowsable(EditorBrowsableState.Never)]
		public LayoutItemBase FindItemOrGroupByPath(string path) {
			List<string> pathIndexes = GetPathIndices(path);
			int num = 0;
			if (!Int32.TryParse(pathIndexes[0], out num))
				return null;
			pathIndexes.RemoveAt(0);
			if (num > Items.Count - 1)
				return null;
			if (pathIndexes.Count > 0 && Items[num] is LayoutGroupBase) {
				string newPath = string.Empty;
				pathIndexes.ForEach(delegate(string currentNum) { newPath += PathSeparator + currentNum; });
				LayoutItemBase result = (Items[num] as LayoutGroupBase).FindItemOrGroupByPath(newPath.Substring(1));
				if (result != null)
					return result;
			} else
				return pathIndexes.Count > 0 ? null : Items[num];
			return null;
		}
		[Obsolete("This method is now obsolete. Use the FindItemOrGroupByName method instead."), EditorBrowsable(EditorBrowsableState.Never)]
		public LayoutItemBase FindItemByPath(string path) {
			return FindItemOrGroupByPath(path);
		}
		protected internal ColumnLayoutItem FindColumnItemInternal(string Name_ColumnName) {
			return FindItemOrGroupByCondition(delegate(LayoutItemBase item) {
				ColumnLayoutItem columnItem = item as ColumnLayoutItem;
				if(columnItem == null)
					return false;
				return columnItem.Name == Name_ColumnName || columnItem.ColumnName == Name_ColumnName;
			}) as ColumnLayoutItem;
		}
		protected internal LayoutItemBase FindItemOrGroupByCondition(Predicate<LayoutItemBase> predicate) {
			foreach (LayoutItemBase childItem in Items) {
				if (predicate(childItem))
					return childItem;
				if (childItem is LayoutGroupBase) {
					LayoutItemBase result = ((LayoutGroupBase)childItem).FindItemOrGroupByCondition(predicate);
					if (result != null)
						return result;
				}
			}
			return null;
		}
		protected override IStateManager[] GetStateManagedObjects() {
			return ViewStateUtils.GetMergedStateManagedObjects(base.GetStateManagedObjects(),
				new IStateManager[] { Items, SettingsItemCaptions, SettingsItemHelpTexts, SettingsItems });
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("LayoutGroupBaseItems"),
#endif
 PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content), MergableProperty(false), NotifyParentProperty(true), 
		AutoFormatEnable, Themeable(true), Browsable(false)]
		public LayoutItemCollection Items {
			get {
				return items = items ?? CreateItems();
			}
		}
		protected virtual LayoutItemCollection CreateItems() {
			return new LayoutItemCollection(this);
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("LayoutGroupBaseSettingsItemCaptions"),
#endif
		RefreshProperties(RefreshProperties.All), NotifyParentProperty(true), AutoFormatEnable,
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content), Category("Layout"),
		PersistenceMode(PersistenceMode.InnerProperty)]
		public LayoutItemCaptionSettings SettingsItemCaptions {
			get { return this.itemsCaptionSettings; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("LayoutGroupBaseSettingsItemHelpTexts"),
#endif
		RefreshProperties(RefreshProperties.All), NotifyParentProperty(true), AutoFormatEnable,
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content), Category("Layout"),
		PersistenceMode(PersistenceMode.InnerProperty)]
		public LayoutItemHelpTextSettings SettingsItemHelpTexts {
			get { return this.itemsHelpTextSettings; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("LayoutGroupBaseSettingsItems"),
#endif
		Category("Layout"), NotifyParentProperty(true), AutoFormatEnable,
		PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public LayoutGroupItemSettings SettingsItems {
			get { return this.itemsSettings; }
		}
		protected override IList GetDesignTimeItems() {
			return Items;
		}
	}
	public abstract class LayoutItemBase : CollectionItem {
		protected const char PathSeparator = '_';
		protected const bool DefaultShowCaption = true;
		FormLayoutProperties owner = null;
		LayoutGroupBase parentGroup = null;
		LayoutGroupCellStyle parentCellStyle = null;
		TabImageProperties tabImage = null;
		public LayoutItemBase()
			: base() {
		}
		public LayoutItemBase(string caption)
			: this(caption, string.Empty) {
		}
		public LayoutItemBase(string caption, string name)
			: this() {
				Caption = caption;
				Name = name;
		}
		protected LayoutItemBase(FormLayoutProperties owner)
			: this() {
			Owner = owner;
		}
		protected LayoutItemBase(ASPxFormLayout formLayout)
			: this(formLayout.Properties) {
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public LayoutGroupBase ParentGroup {
			get { return IsLocatedInRootGroup() ? null : ParentGroupInternal; }
		}
		protected bool IsLocatedInRootGroup() {
			return Owner != null && Owner.Root == ParentGroupInternal;
		}
		protected bool IsRootGroup() {
			return Owner != null && Owner.Root == this;
		}
		internal string GetItemCaption() {
			if (GetShowCaption())
				return GetItemCaptionCore();
			return string.Empty;
		}
		protected virtual string GetItemCaptionCore() {
			return Caption;
		}
		protected internal string Path {
			get {
				if (ParentGroupInternal == null)
					return string.Empty;
				else {
					string path = ParentGroupInternal.Path;
					path += (!string.IsNullOrEmpty(path) ? PathSeparator.ToString() : string.Empty) + ParentGroupInternal.Items.IndexOf(this);
					return path;
				}
			}
		}
		protected internal virtual LayoutGroupBase ParentGroupInternal {
			get { return this.parentGroup; }
			set { 
				this.parentGroup = value;
				CheckColSpanValue(ColSpan);
			}
		}
		protected internal FormLayoutProperties Owner {
			get { return this.ParentGroupInternal != null ? this.ParentGroupInternal.Owner : owner; }
			set { owner = value; }
		}
		protected internal ASPxFormLayout FormLayout {
			get { return Owner != null ? Owner.FormLayout : null; }
		}
		protected internal List<string> GetPathIndices() {
			return GetPathIndices(Path);
		}
		protected static List<string> GetPathIndices(string path) {
			return new List<string>(path.Split(PathSeparator));
		}
		protected override void LayoutChanged() {
			if(ParentGroupInternal == null && Owner != null)
				((IPropertiesOwner)Owner).Changed(null);
			base.LayoutChanged();
		}
		public override void Assign(CollectionItem source) {
			LayoutItemBase item = source as LayoutItemBase;
			if (item != null) {
				Name = item.Name;
				if(item.IsCaptionAssigned)
					Caption = item.Caption;
				ShowCaption = item.ShowCaption;
				RowSpan = item.RowSpan;
				ColSpan = item.ColSpan;
				Width = item.Width;
				Height = item.Height;
				VerticalAlign = item.VerticalAlign;
				HorizontalAlign = item.HorizontalAlign;
				Visible = item.Visible;
				ClientVisible = item.ClientVisible;
				ParentContainerStyle.Assign(item.ParentContainerStyle);
				TabImage.Assign(item.TabImage);
			}
			base.Assign(source);
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("LayoutItemBaseName"),
#endif
		DefaultValue(""), NotifyParentProperty(true), Localizable(false)]
		public string Name {
			get { return GetStringProperty("Name", string.Empty); }
			set { SetStringProperty("Name", string.Empty, value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("LayoutItemBaseCaption"),
#endif
		DefaultValue(""), NotifyParentProperty(true), AutoFormatDisable, Localizable(true)]
		public virtual string Caption {
			get { return GetStringProperty("Caption", GetDefaultCaption()); }
			set {
				SetStringProperty("Caption", GetDefaultCaption(), value);
				LayoutChanged();
				IsCaptionAssigned = value != null;
			}
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool IsCaptionAssigned {
			get { return GetBoolProperty("IsCaptionAssigned", false); }
			private set { SetBoolProperty("IsCaptionAssigned", false, value); }
		}
		protected bool ShouldSerializeCaption() {
			return IsCaptionAssigned;
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("LayoutItemBaseShowCaption"),
#endif
 Category("Layout"),
		DefaultValue(DefaultBoolean.Default), NotifyParentProperty(true), AutoFormatDisable]
		public virtual DefaultBoolean ShowCaption {
			get { return (DefaultBoolean)GetObjectProperty("ShowCaption", DefaultBoolean.Default); }
			set {
				if (ShowCaption != value) {
					SetObjectProperty("ShowCaption", DefaultBoolean.Default, value);
					LayoutChanged();
				}
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("LayoutItemBaseRowSpan"),
#endif
 Category("Layout"),
		DefaultValue(1), NotifyParentProperty(true), AutoFormatEnable]
		public int RowSpan {
			get { return GetIntProperty("RowSpan", 1); }
			set {
				CommonUtils.CheckNegativeOrZeroValue((double)value, "RowSpan");
				SetIntProperty("RowSpan", 1, value); 
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("LayoutItemBaseColSpan"),
#endif
 Category("Layout"),
		DefaultValue(1), NotifyParentProperty(true), AutoFormatEnable]
		public int ColSpan {
			get { return GetIntProperty("ColSpan", 1); }
			set {
				CheckColSpanValue(value);
				SetIntProperty("ColSpan", 1, value);
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("LayoutItemBaseWidth"),
#endif
 Category("Layout"),
		DefaultValue(typeof(Unit), ""), NotifyParentProperty(true), AutoFormatDisable]
		public Unit Width {
			get { return (Unit)GetObjectProperty("Width", Unit.Empty); }
			set { SetObjectProperty("Width", Unit.Empty, value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("LayoutItemBaseHeight"),
#endif
 Category("Layout"),
		DefaultValue(typeof(Unit), ""), NotifyParentProperty(true), AutoFormatDisable]
		public Unit Height {
			get { return (Unit)GetObjectProperty("Height", Unit.Empty); }
			set { SetObjectProperty("Height", Unit.Empty, value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("LayoutItemBaseVerticalAlign"),
#endif
 Category("Layout"),
		DefaultValue(FormLayoutVerticalAlign.NotSet), NotifyParentProperty(true), AutoFormatDisable]
		public FormLayoutVerticalAlign VerticalAlign {
			get { return (FormLayoutVerticalAlign)GetEnumProperty("VerticalAlign", FormLayoutVerticalAlign.NotSet); }
			set { SetEnumProperty("VerticalAlign", FormLayoutVerticalAlign.NotSet, value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("LayoutItemBaseHorizontalAlign"),
#endif
 Category("Layout"),
		DefaultValue(FormLayoutHorizontalAlign.NotSet), NotifyParentProperty(true), AutoFormatDisable]
		public FormLayoutHorizontalAlign HorizontalAlign {
			get { return (FormLayoutHorizontalAlign)GetEnumProperty("HorizontalAlign", FormLayoutHorizontalAlign.NotSet); }
			set { SetEnumProperty("HorizontalAlign", FormLayoutHorizontalAlign.NotSet, value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("LayoutItemBaseParentContainerStyle"),
#endif
		Category("Styles"), NotifyParentProperty(true),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable]
		public LayoutGroupCellStyle ParentContainerStyle {
			get { return this.parentCellStyle = this.parentCellStyle ?? new LayoutGroupCellStyle(); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("LayoutItemBaseVisible"),
#endif
		DefaultValue(true), NotifyParentProperty(true)]
		public bool Visible {
			get { return GetVisible(); }
			set { SetVisible(value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("LayoutItemBaseVisibleIndex"),
#endif
		NotifyParentProperty(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public int VisibleIndex {
			get { return GetVisibleIndex(); }
			set { SetVisibleIndex(value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("LayoutItemBaseClientVisible"),
#endif
		Category("Client-Side"), DefaultValue(true), AutoFormatDisable, Localizable(false), NotifyParentProperty(true)]
		public bool ClientVisible {
			get { return base.ClientVisibleInternal; }
			set { base.ClientVisibleInternal = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("LayoutItemBaseTabImage"),
#endif
 Category("Images"), NotifyParentProperty(true),
		PersistenceMode(PersistenceMode.InnerProperty), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public TabImageProperties TabImage {
			get {
				if(tabImage == null)
					tabImage = new TabImageProperties(this);
				return tabImage;
			}
		}
		protected internal virtual bool AllowEllipsisInText {
			get { return false; }
		}
		protected internal bool IsVisible() {
			if(!Visible)
				return false;
			LayoutGroupBase currentParent = ParentGroupInternal;
			while(currentParent != null) {
				if(!currentParent.Visible)
					return false;
				currentParent = currentParent.ParentGroupInternal;
			}
			return true;
		}
		protected override IStateManager[] GetStateManagedObjects() {
			return new IStateManager[] { ParentContainerStyle, TabImage };
		}
		protected internal Unit GetWidth() {
			if (IsRootGroup())
				return Unit.Empty;
			if(!Width.IsEmpty)
				return Width;
			if(ParentGroupInternal != null && !ParentGroupInternal.SettingsItems.Width.IsEmpty)
				return ParentGroupInternal.SettingsItems.Width;
			if(!Owner.SettingsItems.Width.IsEmpty)
				return Owner.SettingsItems.Width;
			return Unit.Empty;
		}
		protected internal Unit GetHeight() {
			if (IsRootGroup())
				return Unit.Empty;
			if(!Height.IsEmpty)
				return Height;
			if(ParentGroupInternal != null && !ParentGroupInternal.SettingsItems.Height.IsEmpty)
				return ParentGroupInternal.SettingsItems.Height;
			return Owner.SettingsItems.Height;
		}
		protected internal virtual bool GetShowCaption() {
			if(ShowCaption != DefaultBoolean.Default)
				return ShowCaption == DefaultBoolean.True;
			if(ParentGroupInternal != null && ParentGroupInternal.SettingsItems.ShowCaption != DefaultBoolean.Default)
				return ParentGroupInternal.SettingsItems.ShowCaption == DefaultBoolean.True;
			if(Owner != null && Owner.SettingsItems.ShowCaption != DefaultBoolean.Default)
				return Owner.SettingsItems.ShowCaption == DefaultBoolean.True;
			return DefaultShowCaption;
		}
		protected internal virtual FormLayoutVerticalAlign GetVerticalAlign() {
#pragma warning disable 618
			if (VerticalAlign != FormLayoutVerticalAlign.NoSet && VerticalAlign != FormLayoutVerticalAlign.NotSet)
				return VerticalAlign;
			if (ParentGroupInternal != null && ParentGroupInternal.SettingsItems.VerticalAlign != FormLayoutVerticalAlign.NoSet
				&& ParentGroupInternal.SettingsItems.VerticalAlign != FormLayoutVerticalAlign.NotSet)
				return ParentGroupInternal.SettingsItems.VerticalAlign;
			if (Owner != null && Owner.SettingsItems.VerticalAlign != FormLayoutVerticalAlign.NoSet
				&& Owner.SettingsItems.VerticalAlign != FormLayoutVerticalAlign.NotSet)
				return Owner.SettingsItems.VerticalAlign;
			return FormLayoutVerticalAlign.NotSet;
#pragma warning restore 618
		}
		protected internal FormLayoutHorizontalAlign GetHorizontalAlign() {
#pragma warning disable 618
			if (HorizontalAlign != FormLayoutHorizontalAlign.NoSet && HorizontalAlign != FormLayoutHorizontalAlign.NotSet)
				return HorizontalAlign;
			if (ParentGroupInternal != null && ParentGroupInternal.SettingsItems.HorizontalAlign != FormLayoutHorizontalAlign.NoSet
				&& ParentGroupInternal.SettingsItems.HorizontalAlign != FormLayoutHorizontalAlign.NotSet)
				return ParentGroupInternal.SettingsItems.HorizontalAlign;
			if (Owner != null && Owner.SettingsItems.HorizontalAlign != FormLayoutHorizontalAlign.NoSet
				&& Owner.SettingsItems.HorizontalAlign != FormLayoutHorizontalAlign.NotSet)
				return Owner.SettingsItems.HorizontalAlign;
			return FormLayoutHorizontalAlign.NotSet;
#pragma warning restore 618
		}
		protected internal int GetColSpan() {
			return Owner != null && Owner.IsFlowRender ? 1 : ColSpan;
		}
		protected internal int GetRowSpan() {
			return Owner != null && Owner.IsFlowRender ? 1 : RowSpan;
		}
		protected void CheckColSpanValue(int value) {
			if(ParentGroupInternal is LayoutGroup) {
				int maxColSpan = (ParentGroupInternal as LayoutGroup).ColCount;
				CommonUtils.CheckValueRange(value, 1, maxColSpan, "ColSpan");
			}
		}
		protected virtual string GetDefaultCaption() {
			return string.Empty;
		}
	}
}
namespace DevExpress.Web.Internal {
	public enum NestedControlSearchMode { ReturnFirstAllowedControl, ReturnFirstNotLiteralControl }
}
