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
using System.Linq;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.ComponentModel;
using DevExpress.Web.Internal;
using DevExpress.Web.Design;
using DevExpress.XtraPrinting.Native;
namespace DevExpress.Web {
	public enum CustomButtonsPosition { Near, Far };
	public abstract class ButtonEditPropertiesBase: TextBoxPropertiesBase {
		private EditButtonCollection buttons = null;
		private ClearButton clearButton = null;
		private ITemplate buttonTemplate = null;
		public ButtonEditPropertiesBase()
			: base() {
		}
		public ButtonEditPropertiesBase(IPropertiesOwner owner)
			: base(owner) {
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ButtonEditPropertiesBaseButtonEditEllipsisImage"),
#endif
		Category("Images"), NotifyParentProperty(true), PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		MergableProperty(false), AutoFormatDisable]
		public ButtonImageProperties ButtonEditEllipsisImage {
			get {
				return Images.ButtonEditEllipsis;
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ButtonEditPropertiesBaseClearButton"),
#endif
		NotifyParentProperty(true), PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		MergableProperty(false), AutoFormatDisable]
		public virtual ClearButton ClearButton {
			get {
				if(clearButton == null)
					clearButton = new ClearButton(this);
				return clearButton;
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ButtonEditPropertiesBaseButtons"),
#endif
		NotifyParentProperty(true), PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		MergableProperty(false), AutoFormatDisable,
		Editor("DevExpress.Web.Design.ColumnsPropertiesCommonEditor, " + AssemblyInfo.SRAssemblyWebDesignFull, typeof(System.Drawing.Design.UITypeEditor))]
		public EditButtonCollection Buttons {
			get {
				if(buttons == null)
					buttons = new EditButtonCollection(this);
				return buttons;
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ButtonEditPropertiesBaseSpacing"),
#endif
		Category("Layout"), NotifyParentProperty(true), DefaultValue(1), AutoFormatEnable]
		public int Spacing {
			get { return Styles.ButtonEditCellSpacing; }
			set { Styles.ButtonEditCellSpacing = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ButtonEditPropertiesBaseAllowUserInput"),
#endif
		Category("Behavior"), NotifyParentProperty(true), DefaultValue(true), AutoFormatDisable]
		public virtual bool AllowUserInput {
			get { return GetBoolProperty("AllowUserInput", true); }
			set { SetBoolProperty("AllowUserInput", true, value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ButtonEditPropertiesBaseAllowMouseWheel"),
#endif
		Category("Behavior"), DefaultValue(true), NotifyParentProperty(true), AutoFormatDisable]
		public virtual bool AllowMouseWheel {
			get { return GetBoolProperty("AllowMouseWheel", true); }
			set { SetBoolProperty("AllowMouseWheel", true, value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ButtonEditPropertiesBaseButtonStyle"),
#endif
		Category("Styles"), PersistenceMode(PersistenceMode.InnerProperty), NotifyParentProperty(true), AutoFormatEnable,
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public EditButtonStyle ButtonStyle {
			get { return Styles.ButtonEditButton; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ButtonEditPropertiesBaseClearButtonStyle"),
#endif
		Category("Styles"), PersistenceMode(PersistenceMode.InnerProperty), NotifyParentProperty(true), AutoFormatEnable,
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public EditButtonStyle ClearButtonStyle {
			get { return Styles.ButtonEditClearButton; }
		}
		[Browsable(false), DefaultValue(null), 
		TemplateContainer(typeof(TemplateContainerBase)),
		PersistenceMode(PersistenceMode.InnerProperty),
		NotifyParentProperty(true), AutoFormatEnable, 
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual ITemplate ButtonTemplate {
			get { return buttonTemplate; }
			set {
				buttonTemplate = value;
				Changed();
			}
		}
		[
		Category("Layout"), NotifyParentProperty(true), DefaultValue(CustomButtonsPosition.Near), AutoFormatEnable]
		protected CustomButtonsPosition CustomButtonsPositionInternal { 
			get { return (CustomButtonsPosition)GetEnumProperty("CustomButtonsPosition", CustomButtonsPosition.Near); }
			set { 
					SetEnumProperty("CustomButtonsPosition", CustomButtonsPosition.Near, value);
					Changed();
			}
		}
		protected internal bool ForceShowClearButtonAlways {
			get { return GetBoolProperty("ForceShowClearButtonAlways", false); }
			set { SetEnumProperty("ForceShowClearButtonAlways", false, value); }
		}
		protected internal new ButtonEditClientSideEventsBase ClientSideEvents {
			get { return (ButtonEditClientSideEventsBase)base.ClientSideEvents; }
		}
		protected internal ClearButtonDisplayMode ClearButtonDisplayModeInternal {
			get {
				if(ClearButton.DisplayMode == ClearButtonDisplayMode.Auto)
					return IsClearButtonVisibleAuto() || ForceShowClearButtonAlways  ? ClearButtonDisplayMode.Always : ClearButtonDisplayMode.Never;
				return ClearButton.DisplayMode;
			}
		}
		protected internal virtual bool IsClearButtonVisible() {
			return ClearButtonDisplayModeInternal != ClearButtonDisplayMode.Never;
		}
		protected internal virtual bool IsClearButtonVisibleAuto() {
			return RenderUtils.Browser.Platform.IsTouchUI;
		}
		public override void Assign(PropertiesBase source) {
			BeginUpdate();
			try {
				base.Assign(source);
				ButtonEditPropertiesBase src = source as ButtonEditPropertiesBase;
				if(src != null) {
					AllowUserInput = src.AllowUserInput;
					AllowMouseWheel = src.AllowMouseWheel;
					ClearButton.Assign(src.ClearButton);
					Buttons.Assign(src.Buttons);
					ButtonTemplate = src.ButtonTemplate;
					CustomButtonsPositionInternal = src.CustomButtonsPositionInternal;
					ForceShowClearButtonAlways = src.ForceShowClearButtonAlways;
				}
			} finally {
				EndUpdate();
			}
		}
		protected override EditClientSideEventsBase CreateClientSideEvents() {
			return new ButtonEditClientSideEventsBase(this);
		}
		protected override IStateManager[] GetStateManagedObjects() {
			return ViewStateUtils.GetMergedStateManagedObjects(base.GetStateManagedObjects(),
				new IStateManager[] { ClearButton, Buttons });
		}
	}
	[Designer("DevExpress.Web.Design.ASPxButtonEditDesigner, " + AssemblyInfo.SRAssemblyWebDesignFull)
]
	public abstract class ASPxButtonEditBase: ASPxTextBoxBase, IPostBackEventHandler, IControlDesigner {
		protected internal const string ButtonControlSuffix = "B";
		protected internal const string ButtonClickHandlerName = "ASPx.BEClick('{0}',{1})";
		protected internal const string ClearHandlerName = "ASPx.BEClear('{0}', event)";
		protected internal static string ButtonImageIdPostfix = "Img";
		private static readonly object EventButtonClick = new object();
		public ASPxButtonEditBase()
			: base() {
		}
		protected ASPxButtonEditBase(ASPxWebControl ownerControl)
			: base(ownerControl) {
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxButtonEditBaseButtons"),
#endif
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		PersistenceMode(PersistenceMode.InnerProperty), AutoFormatDisable,
		Editor("DevExpress.Web.Design.CommonDesignerEditor, " + AssemblyInfo.SRAssemblyWebDesignFull, typeof(System.Drawing.Design.UITypeEditor))]
		public EditButtonCollection Buttons {
			get { return Properties.Buttons; }
		}
		[AutoFormatEnable, DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
#if !SL
	DevExpressWebLocalizedDescription("ASPxButtonEditBaseClearButton"),
#endif
		NotifyParentProperty(true), PersistenceMode(PersistenceMode.InnerProperty)]
		public virtual ClearButton ClearButton {
			get { return Properties.ClearButton; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxButtonEditBaseSpacing"),
#endif
		Category("Layout"), DefaultValue(1), AutoFormatEnable]
		public int Spacing {
			get { return Properties.Spacing; }
			set { Properties.Spacing = value; }
		}
		internal int ActualCellSpacing {
			get { return RenderStyles.ButtonEditCellSpacing; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxButtonEditBaseEncodeHtml"),
#endif
		Browsable(true), EditorBrowsable(EditorBrowsableState.Always),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Visible),
		Category("Behavior"), DefaultValue(true), NotifyParentProperty(true), AutoFormatDisable]
		public override bool EncodeHtml {
			get { return Properties.EncodeHtml; }
			set { Properties.EncodeHtml = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxButtonEditBaseAllowUserInput"),
#endif
		Category("Behavior"), DefaultValue(true), AutoFormatDisable]
		public virtual bool AllowUserInput {
			get { return Properties.AllowUserInput; }
			set { Properties.AllowUserInput = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxButtonEditBaseAllowMouseWheel"),
#endif
		Category("Behavior"), DefaultValue(true), AutoFormatDisable]
		public virtual bool AllowMouseWheel {
			get { return Properties.AllowMouseWheel; }
			set { Properties.AllowMouseWheel = value; }
		}
		protected internal bool ForceShowClearButtonAlways {
			get { return Properties.ForceShowClearButtonAlways; }
			set {
				Properties.ForceShowClearButtonAlways = value; 
				LayoutChanged();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxButtonEditBaseButtonEditEllipsisImage"),
#endif
		Category("Images"), AutoFormatEnable,
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		PersistenceMode(PersistenceMode.InnerProperty)]
		public ButtonImageProperties ButtonEditEllipsisImage {
			get { return Properties.ButtonEditEllipsisImage; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxButtonEditBaseButtonStyle"),
#endif
		Category("Styles"), AutoFormatEnable,
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		PersistenceMode(PersistenceMode.InnerProperty)]
		public EditButtonStyle ButtonStyle {
			get { return Properties.ButtonStyle; }
		}
		[Browsable(false), DefaultValue(null),
		TemplateContainer(typeof(TemplateContainerBase)),
		AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual ITemplate ButtonTemplate {
			get { return Properties.ButtonTemplate; }
			set { Properties.ButtonTemplate = value; }
		}
		protected internal new ButtonEditClientSideEventsBase ClientSideEvents {
			get { return Properties.ClientSideEvents; }
		}
		protected internal new ButtonEditPropertiesBase Properties {
			get { return (ButtonEditPropertiesBase)base.Properties; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxButtonEditBaseButtonClick"),
#endif
		Category("Action")]
		public event ButtonEditClickEventHandler ButtonClick
		{
			add { Events.AddHandler(EventButtonClick, value); }
			remove { Events.RemoveHandler(EventButtonClick, value); }
		}
		protected bool IsButtonEnabled(EditButton button) {
			return IsEnabled() && button.Enabled && button.Visible;
		}
		protected internal override bool IsUserInputAllowed() {
			return AllowUserInput;
		}
		protected internal void ItemsChanged() {
			if(!IsLoading()) {
				ResetControlHierarchy();
			}
		}
		protected override bool HasHoverScripts() {
			return ButtonsChangeStyleOnClient();
		}
		protected override void AddHoverItems(StateScriptRenderHelper helper) {
			for(int i = 0; i < GetButtons().Count; i++){
				EditButton button = GetButtons()[i];
				if(!button.Image.IsEmptyHottracked || !GetButtonHoverStyle().IsEmpty) {
					helper.AddStyle(GetButtonHoverCssStyle(button), GetButtonID(button), new string[0],
						GetButtonImage(button).GetHottrackedScriptObject(Page), ButtonImageIdPostfix, IsButtonEnabled(button));
				}
			}
		}
		protected override bool HasPressedScripts() {
			return ButtonsChangeStyleOnClient() && !GetButtonPressedStyle().IsEmpty;
		}
		protected override void AddPressedItems(StateScriptRenderHelper helper) {
			for(int i = 0; i < GetButtons().Count; i++) {
				EditButton button = GetButtons()[i];
				helper.AddStyle(GetButtonPressedCssStyle(button), GetButtonID(button), new string[0],
					GetButtonImage(button).GetPressedScriptObject(Page), ButtonImageIdPostfix, IsButtonEnabled(button));
			}
		}
		protected override void GetCreateClientObjectScript(StringBuilder stb, string localVarName, string clientName) {
			base.GetCreateClientObjectScript(stb, localVarName, clientName);
			if(Buttons.Count > 0)
				stb.Append(localVarName + ".buttonCount = " + Buttons.Count.ToString() + ";\n");
			if(!AllowUserInput)
				stb.Append(localVarName + ".allowUserInput = false;\n");
			if(!AllowMouseWheel)
				stb.Append(localVarName + ".allowMouseWheel = false;\n");
			bool showClearButtonAlways = Properties.ClearButtonDisplayModeInternal == ClearButtonDisplayMode.Always ||
										 ClearButton.DisplayMode == ClearButtonDisplayMode.Auto && ForceShowClearButtonAlways;
			if(showClearButtonAlways)
				stb.Append(localVarName + ".showClearButtonAlways = true;\n");
		}
		protected override void GetClientObjectAssignedServerEvents(List<string> eventNames) {
			if(Events[EventButtonClick] != null)
				eventNames.Add("ButtonClick");
		}
		protected override void AddDisabledItems(StateScriptRenderHelper helper) {
			base.AddDisabledItems(helper);
			for(int i = 0; i < GetButtons().Count; i++) {
				EditButton button = GetButtons()[i];
				helper.AddStyle(GetButtonDisabledCssStyle(button), GetButtonID(button), new string[0],
					GetButtonImage(button).GetDisabledScriptObject(Page), ButtonImageIdPostfix, IsButtonEnabled(button));
			}
		}
		protected virtual bool ButtonsChangeStyleOnClient(){
			foreach (EditButton button in GetButtons()){
				if(button.Visible && button.Enabled)
					return true;
			}
			return false;
		}
		protected override AppearanceStyle GetDefaultEditStyle() {
			AppearanceStyle defaultEditStyle = new AppearanceStyle();
			defaultEditStyle.CssClass = EditorStyles.ButtonEditSystemClassName;
			defaultEditStyle.CopyFrom(RenderStyles.GetDefaultButtonEditStyle());
			return defaultEditStyle;
		}
		protected override AppearanceStyle GetEditStyleFromStylesStorage() {
			AppearanceStyle style = new AppearanceStyle();
			style.CopyFontFrom(RenderStyles.TextBox);
			style.CopyFrom(RenderStyles.ButtonEdit);
			return style;
		}
		protected EditButtonStyle GetButtonStyleInternal() {
			EditButtonStyle style = new EditButtonStyle();
			style.CopyFrom(RenderStyles.GetDefaultButtonEditButtonStyle());
			style.CopyFrom(RenderStyles.ButtonEditButton);
			return style;
		}		
		protected EditButtonStyle GetClearButtonStyleInternal() {
			EditButtonStyle style = new EditButtonStyle();
			style.CopyFrom(RenderStyles.GetDefaultButtonEditClearButtonStyle());
			style.CopyFrom(RenderStyles.ButtonEditClearButton);
			return style;
		}
		protected virtual EditButtonStyle GetButtonStyleInternal(EditButton button) { 
			if(button is ClearButton)
				return GetClearButtonStyleInternal();
			return GetButtonStyleInternal();
		}
		protected internal EditButtonStyle GetButtonStyle(EditButton button) {
			EditButtonStyle style = new EditButtonStyle();
			style.CopyFrom(GetButtonStyleInternal(button));
			MergeDisableStyle(style, IsEnabled() && button.Enabled, GetButtonDisabledStyle(button), false);
			return style;
		}
		protected virtual internal DisabledStyle GetButtonDisabledStyle(EditButton button) {
			DisabledStyle style = new DisabledStyle();
			style.CopyFrom(GetDisabledStyle());
			style.CopyFrom(RenderStyles.GetDefaultButtonDisabledStyle());
			style.CopyFrom(RenderStyles.ButtonEditButton.DisabledStyle);
			return style;
		}
		protected DisabledStyle GetButtonDisabledCssStyle(EditButton button) {
			DisabledStyle style = new DisabledStyle();
			style.CopyFrom(GetButtonDisabledStyle(button));
			return style;
		}
		protected internal AppearanceSelectedStyle GetButtonHoverStyle() {
			return GetButtonStyleInternal().HoverStyle;
		}
		protected virtual internal AppearanceSelectedStyle GetButtonHoverStyle(EditButton button) {
			return GetButtonHoverStyle();
		}
		protected internal AppearanceStyleBase GetButtonHoverCssStyle(EditButton button) {
			AppearanceStyle style = new AppearanceStyle();
			style.CopyFrom(GetButtonHoverStyle(button));
			style.Paddings.CopyFrom(GetButtonHoverPaddings(button));
			return style;
		}
		protected Paddings GetButtonHoverPaddings(EditButton button) {
			EditButtonStyle style = GetButtonStyle(button);
			return UnitUtils.GetSelectedCssStylePaddings(style, GetButtonHoverStyle(button), 
				style.Paddings);
		}
		protected internal AppearanceSelectedStyle GetButtonPressedStyle() {
			return GetButtonStyleInternal().PressedStyle;
		}
		protected virtual internal AppearanceSelectedStyle GetButtonPressedStyle(EditButton button) {
			return GetButtonPressedStyle();
		}
		protected internal AppearanceStyleBase GetButtonPressedCssStyle(EditButton button) {
			AppearanceStyle style = new AppearanceStyle();
			style.CopyFrom(GetButtonPressedStyle(button));
			style.Paddings.CopyFrom(GetButtonPressedPaddings(button));
			return style;
		}
		protected Paddings GetButtonPressedPaddings(EditButton button) {
			EditButtonStyle style = GetButtonStyle(button);
			return UnitUtils.GetSelectedCssStylePaddings(style, GetButtonPressedStyle(button), 
				style.Paddings);
		}
		protected internal ButtonImageProperties GetButtonImage(EditButton button) {
			ButtonImageProperties image = new ButtonImageProperties();
			if(String.IsNullOrEmpty(button.Text))
				image.CopyFrom(button.GetDefaultImage(Page, RenderImages, IsRightToLeft()));
			image.CopyFrom(button.Image);
			return image;
		}
		protected internal Unit GetButtonWidth(EditButton button) {
			EditButtonStyle style = GetButtonStyle(button);
			Unit width = button.Width.IsEmpty ? style.Width : button.Width;
			if(!width.IsEmpty)
				return UnitUtils.GetCorrectedWidth(width, style, style.Paddings);
			return width.IsEmpty ? style.Width : width;
		}
		protected internal ITemplate GetButtonTemplate() {
			return Properties.ButtonTemplate;
		}
		protected internal virtual string GetButtonOnClick(EditButton button) {
			if(button is ClearButton)
				return string.Empty;
			return string.Format(ButtonClickHandlerName, ClientID, button.Index);
		}
		protected internal virtual string GetButtonOnMouseDown(EditButton button) {
			if(button is ClearButton)
				return string.Format(ClearHandlerName, ClientID);
			return string.Empty;
		}
		protected internal string GetButtonID(EditButton button) {
			return ButtonControlSuffix + GetButtonIndex(button);
		}
		protected internal virtual int GetButtonIndex(EditButton button) {
			if(button == ClearButton)
				return -100;
			return button.Index;
		}
		protected internal virtual string GetButtonImageID(EditButton button) {
			return GetButtonID(button) + ButtonImageIdPostfix;
		}
		protected internal virtual List<EditButton> GetButtons() {
			List<EditButton> buttons = GetButtonsCore();
			buttons.Insert(0, ClearButton);
			return buttons;
		}
		protected internal virtual List<EditButton> GetButtonsCore() {
			return Buttons.OfType<EditButton>().ToList();
		}
		protected internal Unit GetInputHeight() {
			Unit height = GetHeight();
			if(!height.IsEmpty)
				return UnitUtils.GetCorrectedHeight(height, GetEditAreaStyle(), GetEditAreaStyle().Paddings);
			return Unit.Empty;
		}
		protected internal static void AppendButtons(IList<EditButton> list, params EditButton[] buttons) {
			buttons.ForEach(button => {
				switch(button.Position) {
					case ButtonsPosition.Left:
						list.Insert(0, button);
						break;
					case ButtonsPosition.Right:
						list.Add(button);
						break;
				}
			});
		}
		protected override bool IsServerSideEventsAssigned() {
			return Events[EventButtonClick] != null;
		}
		protected void OnButtonClick(ButtonEditClickEventArgs e) {
			ButtonEditClickEventHandler handler = (ButtonEditClickEventHandler)Events[EventButtonClick];
			if(handler != null)
				handler(this, e);
			this.RaiseBubbleEvent(this, e);
		}
		protected override void RaisePostBackEvent(string eventArgument) {
			base.RaisePostBackEvent(eventArgument);
			string[] arguments = eventArgument.Split(new char[] { ':' });
			switch(arguments[0]) {
				case "BC":
					OnButtonClick(new ButtonEditClickEventArgs(Int32.Parse(arguments[1])));
					break;
			}
		}
		string IControlDesigner.DesignerType { get { return "DevExpress.Web.Design.ButtonEditButtonsOwner"; } }
	}
	public class ButtonEditProperties: ButtonEditPropertiesBase {
		public ButtonEditProperties()
			: base() {
		}
		public ButtonEditProperties(IPropertiesOwner owner)
			: base(owner) {
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ButtonEditPropertiesClientSideEvents"),
#endif
		Category("Client-Side"), DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		PersistenceMode(PersistenceMode.InnerProperty),
		NotifyParentProperty(true), AutoFormatDisable]
		public new ButtonEditClientSideEvents ClientSideEvents {
			get { return (ButtonEditClientSideEvents)base.ClientSideEvents; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ButtonEditPropertiesMaxLength"),
#endif
		DefaultValue(0), NotifyParentProperty(true), AutoFormatDisable]
		public new int MaxLength {
			get { return base.MaxLength; }
			set { base.MaxLength = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ButtonEditPropertiesPassword"),
#endif
		DefaultValue(false), NotifyParentProperty(true), AutoFormatDisable]
		public new bool Password {
			get { return base.Password; }
			set { base.Password = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ButtonEditPropertiesMaskSettings"),
#endif
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		PersistenceMode(PersistenceMode.InnerProperty), AutoFormatDisable, NotifyParentProperty(true)]
		public MaskSettings MaskSettings {
			get { return MaskSettingsInternal; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ButtonEditPropertiesMaskHintStyle"),
#endif
		Category("Styles"), DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable, NotifyParentProperty(true)]
		public MaskHintStyle MaskHintStyle {
			get { return Styles.MaskHint; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ButtonEditPropertiesNullText"),
#endif
		DefaultValue(""), AutoFormatDisable, NotifyParentProperty(true), Localizable(true)]
		public string NullText {
			get { return NullTextInternal; }
			set { NullTextInternal = value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override bool AllowUserInput {
			get { return base.AllowUserInput; }
			set { base.AllowUserInput = value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public override bool AllowMouseWheel {
			get { return base.AllowMouseWheel; }
			set { base.AllowMouseWheel = value; }
		}
		protected override ASPxEditBase CreateEditInstance() {
			return new ASPxButtonEdit();
		}
		protected override EditClientSideEventsBase CreateClientSideEvents() {
			return new ButtonEditClientSideEvents(this);
		}
	}
	[DXWebToolboxItem(DXToolboxItemKind.Free),
	ToolboxData("<{0}:ASPxButtonEdit runat=\"server\">\n\t<Buttons>\n\t\t<{0}:EditButton>\n\t</{0}:EditButton>\n\t</Buttons>\n</{0}:ASPxButtonEdit>"),
	System.Drawing.ToolboxBitmap(typeof(ToolboxBitmapAccess), ToolboxBitmapAccess.BitmapPath + "ASPxButtonEdit.bmp"),
	DevExpress.Utils.ToolboxTabName(AssemblyInfo.DXTabCommon)]
	public class ASPxButtonEdit: ASPxButtonEditBase {
		private ButtonEditControl control = null;
		public ASPxButtonEdit()
			: base() {
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxButtonEditClientSideEvents"),
#endif
		Category("Client-Side"), DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		PersistenceMode(PersistenceMode.InnerProperty), AutoFormatDisable]
		public new ButtonEditClientSideEvents ClientSideEvents {
			get { return Properties.ClientSideEvents; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), 
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override bool AllowUserInput {
			get { return base.AllowUserInput; }
			set { base.AllowUserInput = value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override bool AllowMouseWheel {
			get { return base.AllowMouseWheel; }
			set { base.AllowMouseWheel = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxButtonEditMaskSettings"),
#endif
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		PersistenceMode(PersistenceMode.InnerProperty), AutoFormatDisable]
		public MaskSettings MaskSettings {
			get { return Properties.MaskSettings; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxButtonEditNullText"),
#endif
		DefaultValue(""), AutoFormatDisable, Localizable(true)]
		public string NullText {
			get { return Properties.NullText; }
			set { Properties.NullText = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxButtonEditMaskHintStyle"),
#endif
		Category("Styles"), DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable]
		public MaskHintStyle MaskHintStyle {
			get { return Properties.MaskHintStyle; }
		}
		protected new internal ButtonEditProperties Properties {
			get { return (ButtonEditProperties)base.Properties; }
		}
		protected override EditPropertiesBase CreateProperties() {
			return new ButtonEditProperties(this);
		}
		protected override void ClearControlFields() {
			base.ClearControlFields();
			control = null;
		}
		protected override void CreateControlHierarchy() {
			base.CreateControlHierarchy();
			control = CreateButtonEditControl();
			Controls.Add(control);
		}
		protected virtual ButtonEditControl CreateButtonEditControl() {
			return new ButtonEditControl(this);
		}
		protected override string GetClientObjectClassName() {
			return "ASPxClientButtonEdit";
		}
	}
}
namespace DevExpress.Web.Internal {
	public class ButtonsMergeHelper {
		CustomButtonsPosition customButtonsPostion;
		List<EditButton> defaultButtons;
		List<EditButton> customButtons;
		ButtonsPosition defaultButtonsPosition;
		public ButtonsMergeHelper(ButtonsPosition defaultButtonsPosition, CustomButtonsPosition customButtonsPostion, List<EditButton> defaultButtons, List<EditButton> customButtons)
			: base() {
			this.customButtonsPostion = customButtonsPostion;
			this.defaultButtons = defaultButtons;
			this.customButtons = customButtons;
			this.defaultButtonsPosition = defaultButtonsPosition;
		}
		public List<EditButton> GetMergedButtons() {
			List<EditButton> buttons = new List<EditButton>();
			buttons.AddRange(this.defaultButtons);
			AppendCustomButtons(buttons);
			return buttons;
		}
		private bool IsDefaultButtonsPositionRight() {
			return defaultButtonsPosition == ButtonsPosition.Right;
		}
		private bool IsCustomButtonsPositionNear() {
			return customButtonsPostion == CustomButtonsPosition.Near;
		}
		private bool IsDefaultButtonsPositionLeft() {
			return defaultButtonsPosition == ButtonsPosition.Left;
		}
		private bool IsCustomButtonsPositionFar() {
			return customButtonsPostion == CustomButtonsPosition.Far;
		}
		private bool CustomButtonsNeedToInsert() {
			return (IsCustomButtonsPositionFar() && IsDefaultButtonsPositionLeft()) ||
				(IsCustomButtonsPositionNear() && IsDefaultButtonsPositionRight());
		}
		protected void AppendCustomButtons(List<EditButton> buttons) {
			if(CustomButtonsNeedToInsert())
				buttons.InsertRange(0, customButtons);
			else
				buttons.AddRange(customButtons);
		}
	}
}
