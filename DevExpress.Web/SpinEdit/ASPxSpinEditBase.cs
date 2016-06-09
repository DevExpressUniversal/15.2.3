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

using DevExpress.Web;
using System.ComponentModel;
using System.Web.UI;
using System;
using System.Collections.Generic;
using DevExpress.Web.Internal;
using System.Web.UI.WebControls;
using System.Text;
namespace DevExpress.Web {
	public abstract class SpinEditPropertiesBase : ButtonEditPropertiesBase {
		SpinButtons spinButtons;
		public SpinEditPropertiesBase()
			: base() {
		}
		public SpinEditPropertiesBase(IPropertiesOwner owner)
			: base(owner) {
		}
		protected internal SpinButtons SpinButtonsInternal {
			get {
				if(spinButtons == null)
					spinButtons = CreateSpinButtons();
				return spinButtons;
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("SpinEditPropertiesBaseValueChangedDelay"),
#endif
		Category("Behavior"), DefaultValue(0), AutoFormatDisable, NotifyParentProperty(true)]
		public int ValueChangedDelay {
			get { return GetIntProperty("ValueChangedDelay", 0); }
			set {
				CommonUtils.CheckNegativeValue(value, "ValueChangedDelay");
				SetIntProperty("ValueChangedDelay", 0, value);
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("SpinEditPropertiesBaseIncrementButtonStyle"),
#endif
		Category("Styles"), PersistenceMode(PersistenceMode.InnerProperty), NotifyParentProperty(true), AutoFormatEnable,
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public SpinButtonStyle IncrementButtonStyle {
			get { return Styles.SpinEditIncrementButton; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("SpinEditPropertiesBaseDecrementButtonStyle"),
#endif
		Category("Styles"), PersistenceMode(PersistenceMode.InnerProperty), NotifyParentProperty(true), AutoFormatEnable,
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public SpinButtonStyle DecrementButtonStyle {
			get { return Styles.SpinEditDecrementButton; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("SpinEditPropertiesBaseCustomButtonsPosition"),
#endif
		Category("Layout"), NotifyParentProperty(true), DefaultValue(CustomButtonsPosition.Near), AutoFormatEnable]
		public  CustomButtonsPosition CustomButtonsPosition {
			get { return CustomButtonsPositionInternal; }
			set { CustomButtonsPositionInternal = value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override bool Password { get { return false; } set { } }
		public override void Assign(PropertiesBase source) {
			base.Assign(source);
			SpinEditPropertiesBase props = source as SpinEditPropertiesBase;
			if(props == null) return;
			ValueChangedDelay = props.ValueChangedDelay;
			SpinButtonsInternal.Assign(props.SpinButtonsInternal);
		}
		protected override IStateManager[] GetStateManagedObjects() {
			List<IStateManager> list = new List<IStateManager>(base.GetStateManagedObjects());
			list.Add(SpinButtonsInternal);
			return list.ToArray();
		}
		protected virtual SpinButtons CreateSpinButtons() {
			return new SpinButtons(this);
		}
	}
	public abstract class ASPxSpinEditBase : ASPxButtonEditBase {
		internal const string SpinEditScriptResourceName = EditScriptsResourcePath + "SpinEdit.js";
		SpinEditControl spinEditControl = null;
		protected internal new SpinEditPropertiesBase Properties {
			get { return (SpinEditPropertiesBase)base.Properties; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxSpinEditBaseIncrementButtonStyle"),
#endif
		Category("Styles"), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public SpinButtonStyle IncrementButtonStyle {
			get { return Properties.IncrementButtonStyle; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxSpinEditBaseDecrementButtonStyle"),
#endif
		Category("Styles"), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public SpinButtonStyle DecrementButtonStyle {
			get { return Properties.DecrementButtonStyle; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxSpinEditBaseValueChangedDelay"),
#endif
		Category("Behavior"), DefaultValue(0), AutoFormatDisable]
		public int ValueChangedDelay {
			get { return Properties.ValueChangedDelay; }
			set { Properties.ValueChangedDelay = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxSpinEditBaseCustomButtonsPosition"),
#endif
		Category("Layout"), NotifyParentProperty(true), DefaultValue(CustomButtonsPosition.Near), AutoFormatEnable]
		public CustomButtonsPosition CustomButtonsPosition {
			get { return Properties.CustomButtonsPosition; }
			set { Properties.CustomButtonsPosition = value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override AutoCompleteType AutoCompleteType { get { return base.AutoCompleteType; } set { base.AutoCompleteType = value; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override bool Password { get { return false; } set { } }
		protected internal SpinEditControl SpinEditControl {
			get { return spinEditControl; }
		}
		protected override bool HeightCorrectionRequired {
			get { return base.HeightCorrectionRequired || Browser.Family.IsWebKit; }
		}		
		protected internal SpinButtons SpinButtonsInternal {
			get { return Properties.SpinButtonsInternal; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new event EventHandler TextChanged { add { } remove { } }
		protected override void ClearControlFields() {
			base.ClearControlFields();
			this.spinEditControl = null;
		}
		protected override void CreateControlHierarchy() {
			base.CreateControlHierarchy();
			this.spinEditControl = CreateSpinEditControl();
			Controls.Add(SpinEditControl);
		}
		protected override void PrepareControlHierarchy() {
			base.PrepareControlHierarchy();
			if(!String.IsNullOrEmpty(AccessibilityInputTitle))
				SpinEditControl.SetAccessibilityInputTitle(AccessibilityInputTitle);
		}
		protected virtual SpinEditControl CreateSpinEditControl() {
			return new SpinEditControl(this);
		}
		protected internal override List<EditButton> GetButtonsCore() {
			List<EditButton> spinButtons = new List<EditButton>();
			AddSpinButtons(spinButtons);
			ButtonsMergeHelper helper = new ButtonsMergeHelper(SpinButtonsInternal.Position, CustomButtonsPosition, spinButtons, base.GetButtonsCore());
			List<EditButton> buttons = helper.GetMergedButtons();
			return buttons;
		}
		protected void AddSpinButtons(List<EditButton> buttons) {
			AddSpinButton(buttons, IsRightToLeft() ? SpinButtonsInternal.LargeIncrementImage : SpinButtonsInternal.LargeDecrementImage, SpinButtonsInternal.ShowLargeIncrementButtons, SpinButtonKind.LargeDecrement);
			AddSpinButton(buttons, SpinButtonsInternal.IncrementImage, SpinButtonsInternal.ShowIncrementButtons, SpinButtonKind.Increment);
			AddSpinButton(buttons, SpinButtonsInternal.DecrementImage, SpinButtonsInternal.ShowIncrementButtons, SpinButtonKind.Decrement);
			AddSpinButton(buttons, IsRightToLeft() ? SpinButtonsInternal.LargeDecrementImage : SpinButtonsInternal.LargeIncrementImage, SpinButtonsInternal.ShowLargeIncrementButtons, SpinButtonKind.LargeIncrement);
		}
		protected void AddSpinButton(List<EditButton> buttons, ButtonImageProperties image, bool visible, SpinButtonKind kind) {
			buttons.Add(CreateSpinButton(image, visible, kind));
		}
		protected EditButton CreateSpinButton(ButtonImageProperties image, bool visible,
			SpinButtonKind buttonKind) {
			SpinButtonExtended button = new SpinButtonExtended(buttonKind);
			button.Assign(SpinButtonsInternal);
			button.Image.Assign(image);
			button.Visible = visible;
			return button;
		}
		protected internal override string GetButtonOnClick(EditButton button) {
			if(button is SpinButtonExtended)
				return string.Format(ButtonClickHandlerName, ClientID, GetButtonIndex(button));
			return base.GetButtonOnClick(button);
		}
		protected internal string GetButtonOnDblClick(EditButton button) {
			if(Browser.IsIE && Browser.MajorVersion < 9)
				return string.Format(ButtonClickHandlerName, ClientID, GetButtonIndex(button));
			return "";
		}
		protected internal override int GetButtonIndex(EditButton button) {
			SpinButtonExtended spinButton = button as SpinButtonExtended;
			if(spinButton != null) {
				switch(spinButton.ButtonKind) {
					case SpinButtonKind.LargeDecrement:
						return -1;
					case SpinButtonKind.Increment:
						return -2;
					case SpinButtonKind.Decrement:
						return -3;
					case SpinButtonKind.LargeIncrement:
						return -4;
				}
			}
			return base.GetButtonIndex(button);
		}
		protected internal bool IsShowOnlyLargeButton() {
			return SpinButtonsInternal.ShowLargeIncrementButtons && !SpinButtonsInternal.ShowIncrementButtons;
		}
		protected internal override void RegisterExpandoAttributes(ExpandoAttributes expandoAttributes) {
			base.RegisterExpandoAttributes(expandoAttributes);
			expandoAttributes.AddAttribute("autocomplete", "off", GetFocusableControlID());
		}
		public override void RegisterEditorIncludeScripts() {
			base.RegisterEditorIncludeScripts();
			RegisterIncludeScript(typeof(ASPxSpinEdit), SpinEditScriptResourceName);
		}
		protected override void GetCreateClientObjectScript(StringBuilder stb, string localVarName, string clientName) {
			base.GetCreateClientObjectScript(stb, localVarName, clientName);			
			if(ValueChangedDelay > 0)
				stb.Append(localVarName + ".valueChangedDelay = " + ValueChangedDelay.ToString() + ";\n");
		}
		protected internal override string GetOnTextChanged() {
			return string.Format(TextChangedHandlerName, ClientID);
		}
		protected override bool HasFocusEvents() {
			return true;
		}
		protected internal override string GetOnMouseOver() {
			if(Browser.Family.IsNetscape)
				return string.Format("ASPx.SEMouseOver('{0}',event)", ClientID);
			return base.GetOnMouseOver();
		}
		protected override EditButtonStyle GetButtonStyleInternal(EditButton button) {
			EditButtonStyle ret = base.GetButtonStyleInternal(button);
			if(button is SpinButtonExtended)
				ret.CopyFrom(GetSpinButtonStyle(button as SpinButtonExtended));
			return ret;
		}
		protected override internal AppearanceSelectedStyle GetButtonPressedStyle(EditButton button) {
			AppearanceSelectedStyle ret = base.GetButtonPressedStyle(button);
			if(button is SpinButtonExtended)
				ret.CopyFrom(GetSpinButtonStyle(button as SpinButtonExtended).PressedStyle);
			return ret;
		}
		protected override internal AppearanceSelectedStyle GetButtonHoverStyle(EditButton button) {
			AppearanceSelectedStyle ret = base.GetButtonHoverStyle(button);
			if(button is SpinButtonExtended)
				ret.CopyFrom(GetSpinButtonStyle(button as SpinButtonExtended).HoverStyle);
			return ret;
		}
		protected override internal DisabledStyle GetButtonDisabledStyle(EditButton button) {
			DisabledStyle ret = base.GetButtonDisabledStyle(button);
			if(button is SpinButtonExtended)
				ret.CopyFrom(GetSpinButtonStyle(button as SpinButtonExtended).DisabledStyle);
			return ret;
		}
		protected virtual EditButtonStyle GetSpinButtonStyle(SpinButtonExtended button) {
			EditButtonStyle style = new EditButtonStyle();
			switch(button.ButtonKind) {
				case SpinButtonKind.Increment:
					style.CopyFrom(GetIncrementButtonStyle());
					break;
				case SpinButtonKind.Decrement:
					style.CopyFrom(GetDecrementButtonStyle());
					break;
			}
			return style;
		}
		protected EditButtonStyle GetIncrementButtonStyle() {
			EditButtonStyle ret = new EditButtonStyle();
			ret.CopyFrom(RenderStyles.GetDefaultSpinIncrementButtonStyle());
			ret.CopyFrom(RenderStyles.SpinEditIncrementButton);
			return ret;
		}
		protected EditButtonStyle GetDecrementButtonStyle() {
			EditButtonStyle ret = new EditButtonStyle();
			ret.CopyFrom(RenderStyles.GetDefaultSpinDecrementButtonStyle());
			ret.CopyFrom(RenderStyles.SpinEditDecrementButton);
			return ret;
		}
		protected List<EditButton> GetVisibleButtons() {
			List<EditButton> btns = GetButtons();
			List<EditButton> ret = new List<EditButton>();
			for(int i = 0; i < btns.Count; i++) {
				if(btns[i].Visible)
					ret.Add(btns[i]);
			}
			return ret;
		}
		protected bool IsButtonOnLeft(SpinButtonExtended spinButton) {
			List<EditButton> buttons = GetVisibleButtons();
			int index = IndexOfInVisibleCollection(buttons, spinButton);
			EditButton prevBtn = null;
			if((index - 1 >= 0) && (buttons[index - 1].Visible))
				prevBtn = buttons[index - 1];
			if(prevBtn != null) {
				if(spinButton.ButtonKind == SpinButtonKind.Decrement)
					return IsButtonOnLeft(prevBtn as SpinButtonExtended);
				else
					return prevBtn.Position == spinButton.Position;
			}
			return false;
		}
		protected bool IsButtonOnRight(SpinButtonExtended spinButton) {
			List<EditButton> buttons = GetVisibleButtons();
			int index = IndexOfInVisibleCollection(buttons, spinButton);
			EditButton nextBtn = null;
			if((index + 1 < buttons.Count) && (buttons[index + 1].Visible))
				nextBtn = buttons[index + 1];
			if(nextBtn != null) {
				if(spinButton.ButtonKind == SpinButtonKind.Increment)
					return IsButtonOnRight(nextBtn as SpinButtonExtended);
				else
					return nextBtn.Position == spinButton.Position;
			}
			return false;
		}
		protected bool IsMostLeftSpinButton(SpinButtonExtended spinButton) {
			List<EditButton> buttons = GetVisibleButtons();
			int index = IndexOfInVisibleCollection(buttons, spinButton);
			if(index - 1 >= 0) {
				SpinButtonExtended prevSpin = buttons[index - 1] as SpinButtonExtended;
				if((prevSpin != null) &&
					(spinButton.ButtonKind == SpinButtonKind.Decrement)) {
					return IsMostLeftSpinButton(prevSpin);
				}
				return prevSpin == null;
			}
			return true;
		}
		protected bool IsMostRightSpinButton(SpinButtonExtended spinButton) {
			List<EditButton> buttons = GetVisibleButtons();
			int index = IndexOfInVisibleCollection(buttons, spinButton);
			if(index + 1 < buttons.Count) {
				SpinButtonExtended nextSpin = buttons[index + 1] as SpinButtonExtended;
				if((nextSpin != null) &&
					(spinButton.ButtonKind == SpinButtonKind.Increment))
					return IsMostRightSpinButton(nextSpin);
				return nextSpin == null;
			}
			return true;
		}
		protected bool IsIncrementButton(SpinButtonExtended spinButton) {
			return (spinButton.ButtonKind == SpinButtonKind.Increment) ||
				(spinButton.ButtonKind == SpinButtonKind.Decrement);
		}
		protected int IndexOfInVisibleCollection(List<EditButton> col,
			SpinButtonExtended spinButton) {
			for(int i = 0; i < col.Count; i++) {
				SpinButtonExtended curBtn = col[i] as SpinButtonExtended;
				if(curBtn != null && spinButton.ButtonKind == curBtn.ButtonKind)
					return i;
			}
			return -1;
		}
	}
}
