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
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using DevExpress.LookAndFeel;
using DevExpress.Utils;
using DevExpress.Utils.Drawing;
using DevExpress.Utils.Drawing.Animation;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.ViewInfo;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Drawing;
using DevExpress.XtraEditors.Mask;
using DevExpress.Skins;
using DevExpress.Utils.Design;
using System.Collections.Generic;
using DevExpress.XtraEditors.ToolboxIcons;
namespace DevExpress.XtraEditors.Repository {
	public class RepositoryItemButtonEdit : RepositoryItemTextEdit {
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never), Browsable(false), Obsolete(ObsoleteText.SRObsoletePropertiesText)]
		public new RepositoryItemButtonEdit Properties { get { return this; } }
		private static readonly object buttonPressed = new object();
		private static readonly object buttonClick = new object();
		private static readonly object customDrawButton = new object();
		EditorButtonCollection _buttons;
		protected TextEditStyles fTextEditStyle;
		protected BorderStyles fButtonsStyle;
		bool resetTextEditStyleToStandardInFilterControl;
		public RepositoryItemButtonEdit() {
			this.fTextEditStyle = TextEditStyles.Standard;
			this.fButtonsStyle = BorderStyles.Default;
			this._buttons = CreateButtonCollection();
			this.resetTextEditStyleToStandardInFilterControl = true;
			Buttons.CollectionChanged += new CollectionChangeEventHandler(OnButtons_CollectionChanged);
			CreateDefaultButton();
		}
		protected virtual EditorButtonCollection CreateButtonCollection() {
			return new EditorButtonCollection();
		}
		public virtual void CreateDefaultButton() {
			EditorButton btn = new EditorButton();
			btn.IsDefaultButton = true;
			Buttons.Add(btn);
		}
		public override void BeginInit() {
			bool isFirst = IsFirstInit;
			base.BeginInit();
			if(isFirst) {
				Buttons.Clear();
			}
		}
		DefaultBoolean allowGlyphSkinning = DefaultBoolean.Default;
		[DXCategory(CategoryName.Behavior), 
#if !SL
	DevExpressXtraEditorsLocalizedDescription("RepositoryItemButtonEditAllowGlyphSkinning"),
#endif
 DefaultValue(DefaultBoolean.Default)]
		public virtual DefaultBoolean AllowGlyphSkinning {
			get { return allowGlyphSkinning; }
			set {
				if(AllowGlyphSkinning == value) return;
				allowGlyphSkinning = value;
				OnPropertiesChanged();
			}
		}
		protected internal virtual bool GetAllowGlyphSkinning() {
			return AllowGlyphSkinning == DefaultBoolean.True;
		}
		[DXCategory(CategoryName.Behavior), 
#if !SL
	DevExpressXtraEditorsLocalizedDescription("RepositoryItemButtonEditTextEditStyle"),
#endif
 DefaultValue(TextEditStyles.Standard), SmartTagProperty("Text Edit Style", "Editor Style",SmartTagActionType.RefreshAfterExecute)]
		public virtual TextEditStyles TextEditStyle {
			get { return fTextEditStyle; }
			set {
				if(TextEditStyle == value) return;
				fTextEditStyle = value;
				OnTextEditStyleChanged();
				OnPropertiesChanged();
			}
		}
		[Localizable(true), DXCategory(CategoryName.Behavior), RefreshProperties(RefreshProperties.All), 
#if !SL
	DevExpressXtraEditorsLocalizedDescription("RepositoryItemButtonEditButtons"),
#endif
 DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual EditorButtonCollection Buttons {
			get { return _buttons; }
		}
		[Browsable(false)]
		public override string EditorTypeName { get { return "ButtonEdit"; } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never)]
		public Padding Padding { get; set; }
		bool ShouldSerializeButtonsStyle() { return StyleController == null && BorderStyles.Default != ButtonsStyle; }
		[DXCategory(CategoryName.Appearance), 
#if !SL
	DevExpressXtraEditorsLocalizedDescription("RepositoryItemButtonEditButtonsStyle"),
#endif
 SmartTagProperty("Buttons Style", "Editor Style", SmartTagActionType.RefreshAfterExecute)]
		public virtual BorderStyles ButtonsStyle { 
			get { 
				if(StyleController != null) return StyleController.ButtonsStyle;
				return fButtonsStyle; 
			}
			set {
				if(ButtonsStyle == value) return;
				fButtonsStyle = value;
				OnPropertiesChanged();
			}
		}
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never), Browsable(false), Obsolete(ObsoleteText.SRObsoleteButtonsBorderStyle)]
		public virtual BorderStyles ButtonsBorderStyle { 
			get { return ButtonsStyle; }
			set { }
		}
		public override void Assign(RepositoryItem item) {
			RepositoryItemButtonEdit source = item as RepositoryItemButtonEdit;
			BeginUpdate(); 
			try {
				base.Assign(item);
				if(source == null) return;
				this.fButtonsStyle = source.ButtonsStyle;
				this.fTextEditStyle = source.TextEditStyle;
				this.resetTextEditStyleToStandardInFilterControl = source.ResetTextEditStyleToStandardInFilterControl;
				this.Buttons.Assign(source.Buttons);
			}
			finally {
				EndUpdate();
			}
			Events.AddHandler(buttonClick, source.Events[buttonClick]);
			Events.AddHandler(buttonPressed, source.Events[buttonPressed]);
			Events.AddHandler(customDrawButton, source.Events[customDrawButton]);
		}
		[Browsable(false)]
		public new ButtonEdit OwnerEdit { get { return base.OwnerEdit as ButtonEdit; } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), DXCategory(CategoryName.Behavior), 
#if !SL
	DevExpressXtraEditorsLocalizedDescription("RepositoryItemButtonEditResetTextEditStyleToStandardInFilterControl"),
#endif
 DefaultValue(true)]
		public virtual bool ResetTextEditStyleToStandardInFilterControl {
			get { return resetTextEditStyleToStandardInFilterControl; }
			set { resetTextEditStyleToStandardInFilterControl = value; }
		}
		protected internal override string GetNullEditText() {
			if(TextEditStyle == TextEditStyles.Standard) return base.GetNullEditText();
			return NullText;
		}
		protected virtual void OnButtons_CollectionChanged(object sender, CollectionChangeEventArgs e) {
			OnPropertiesChanged();
		}
		protected virtual void OnTextEditStyleChanged() {
			if(OwnerEdit != null) OwnerEdit.OnTextEditPropertiesChanged();
		}
		protected override bool IsNeededTextKeys { get { return base.IsNeededTextKeys || TextEditStyle != TextEditStyles.HideTextEditor; } }
		protected internal override bool NeededKeysContains(Keys key) {
			if(TextEditStyle != TextEditStyles.Standard) {
				switch(key) {
					case Keys.Left:
					case Keys.Right:
					case Keys.Home:
					case Keys.PageDown:
					case Keys.PageUp:
					case Keys.End:
						return false;
				}
			}
			if(Buttons != null) {
				for(int n = 0; n < Buttons.Count; n ++) {
					EditorButton button = Buttons[n];
					if(!IsButtonEnabled(button) || !button.Visible || !button.Shortcut.IsExist) continue;
					if(key == button.Shortcut.Key)
						return true;
				}
			}
			return base.NeededKeysContains(key);
		}
		protected internal virtual bool IsButtonEnabled(EditorButton button) {
			return button != null && button.Enabled;
		}
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("RepositoryItemButtonEditCustomDrawButton"),
#endif
 DXCategory(CategoryName.Events)]
		public event CustomDrawButtonEventHandler CustomDrawButton {
			add { this.Events.AddHandler(customDrawButton, value); }
			remove { this.Events.RemoveHandler(customDrawButton, value); }
		}
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("RepositoryItemButtonEditButtonClick"),
#endif
 DXCategory(CategoryName.Events)]
		public event ButtonPressedEventHandler ButtonClick {
			add { this.Events.AddHandler(buttonClick, value); }
			remove { this.Events.RemoveHandler(buttonClick, value); }
		}
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("RepositoryItemButtonEditButtonPressed"),
#endif
 DXCategory(CategoryName.Events)]
		public event ButtonPressedEventHandler ButtonPressed {
			add { this.Events.AddHandler(buttonPressed, value); }
			remove { this.Events.RemoveHandler(buttonPressed, value); }
		}
		protected internal virtual void RaiseButtonPressed(ButtonPressedEventArgs e) {
			ButtonPressedEventHandler handler = (ButtonPressedEventHandler)this.Events[buttonPressed];
			if(handler != null) handler(GetEventSender(), e);
		}
		protected internal virtual void RaiseCustomDrawButton(CustomDrawButtonEventArgs e) {
			CustomDrawButtonEventHandler handle = Events[customDrawButton] as CustomDrawButtonEventHandler;
			if(handle == null) return;		   
			handle(this.OwnerEdit, e);
		}
		protected internal virtual void RaiseButtonClick(ButtonPressedEventArgs e) {
			ButtonPressedEventHandler handler = (ButtonPressedEventHandler)this.Events[buttonClick];
			if(handler != null) handler(GetEventSender(), e);
			if(e.Button != null) e.Button.RaiseClick();
		}
		protected internal override bool IsUseDisplayFormat {
			get {
				if(TextEditStyle == TextEditStyles.DisableTextEditor)
					return true;
				return base.IsUseDisplayFormat;
			}
		}
	}
}
namespace DevExpress.XtraEditors {
	[Designer("DevExpress.XtraEditors.Design.ButtonEditDesigner, " + AssemblyInfo.SRAssemblyEditorsDesign),
	 Description("Represents a text editor that allows you to display any number of buttons within the edit box."),
	 ToolboxTabName(AssemblyInfo.DXTabCommon),
	 SmartTagAction(typeof(ButtonEditActions), "Buttons", "Buttons", SmartTagActionType.CloseAfterExecute),
	ToolboxBitmap(typeof(ToolboxIconsRootNS), "ButtonEdit")
	]
	public class ButtonEdit : TextEdit {
		public ButtonEdit() {
		}
		[Browsable(false)]
		public override string EditorTypeName { get { return "ButtonEdit"; } }
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("ButtonEditProperties"),
#endif
 DXCategory(CategoryName.Properties), DesignerSerializationVisibility(DesignerSerializationVisibility.Content), SmartTagSearchNestedProperties]
		public new RepositoryItemButtonEdit Properties { get { return base.Properties as RepositoryItemButtonEdit; } }
		protected internal new ButtonEditViewInfo ViewInfo { get { return base.ViewInfo as ButtonEditViewInfo; } }
		[Browsable(false), 
#if !SL
	DevExpressXtraEditorsLocalizedDescription("ButtonEditIsNeedFocus")
#else
	Description("")
#endif
]
		public override bool IsNeedFocus {
			get {
				return Properties.TextEditStyle == TextEditStyles.Standard;
			}
		}
		protected override void OnMouseDown(MouseEventArgs e) {
			base.OnMouseDown(e);
			if(Parent == null) return; 
			UpdateViewInfoState();
			if(e.Button == MouseButtons.Left) {
				EditHitInfo hi = ViewInfo.CalcHitInfo(new Point(e.X, e.Y));
				if(hi.HitTest == EditHitTest.Button) {
					if(AllowButtonPress && ViewInfo.CanPress(hi)) {
						ViewInfo.PressedInfo = hi;
						RefreshVisualLayout();
						OnPressButton(hi.HitObject as EditorButtonObjectInfoArgs);
					}
				}
			}
		}
		protected override void ClearHotPressedOnLostFocus() {
			Point pt = IsHandleCreated? PointToClient(Control.MousePosition): Point.Empty;
			EditHitInfo hi = ViewInfo.CalcHitInfo(pt);
			ClearHotPressedCore(hi, true);
		}
		protected override void OnMouseUp(MouseEventArgs e) {
			base.OnMouseUp(e);
			UpdateViewInfoState();
			if(e.Button == MouseButtons.Left) {
				EditHitInfo hi = ViewInfo.CalcHitInfo(new Point(e.X, e.Y));
				EditHitInfo prevPressed = ViewInfo.PressedInfo;
				ClearHotPressed(hi);
				if(prevPressed != null && prevPressed.HitTest == EditHitTest.Button) {
					NotifyButtonStateChanged((prevPressed.HitObject as EditorButtonObjectInfoArgs).Button);
				}
				if(AllowButtonPress && !hi.IsEmpty && ViewInfo.CompareHitInfo(hi, prevPressed)) {
					if(hi.HitTest == EditHitTest.Button) {
						OnClickButton(hi.HitObject as EditorButtonObjectInfoArgs);
					}
				}
			}
		}
		protected bool AllowButtonPress { 
			get { return !GetValidationCanceled(); }
		}
		protected override void OnEditorKeyDown(KeyEventArgs e) {
			if(!e.Handled && EditorContainsFocus && Enabled) {
				for(int n = 0; n < Properties.Buttons.Count; n++) {
					EditorButton button = Properties.Buttons[n];
					EditorButtonObjectInfoArgs buttonInfo = ViewInfo.ButtonInfoByButton(button);
					if(buttonInfo == null || !button.Visible || !Properties.IsButtonEnabled(button) || !button.Shortcut.IsExist) continue;
					if(button.Shortcut.Key == e.KeyData) {
						e.Handled = true;
						OnClickButton(buttonInfo);
					}
				}
			}
			base.OnEditorKeyDown(e);
		}
		protected virtual void NotifyButtonStateChanged(EditorButton button) {
			AccessibilityNotifyClients(AccessibleEvents.StateChange, button.Index + 1);
		}
		protected virtual void OnPressButton(EditorButtonObjectInfoArgs buttonInfo) {
			Properties.RaiseButtonPressed(new ButtonPressedEventArgs(buttonInfo.Button));
			NotifyButtonStateChanged(buttonInfo.Button);
		}
		protected internal virtual void OnClickButton(EditorButtonObjectInfoArgs buttonInfo) {
			Properties.RaiseButtonClick(new ButtonPressedEventArgs(buttonInfo.Button));
		}
		public void PerformClick(EditorButton button) {
			Properties.RaiseButtonClick(new ButtonPressedEventArgs(button));
		}
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("ButtonEditButtonClick"),
#endif
 DXCategory(CategoryName.Events)]
		public event ButtonPressedEventHandler ButtonClick {
			add { Properties.ButtonClick += value; }
			remove { Properties.ButtonClick -= value; }
		}
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("ButtonEditButtonPressed"),
#endif
 DXCategory(CategoryName.Events)]
		public event ButtonPressedEventHandler ButtonPressed {
			add { Properties.ButtonPressed += value; }
			remove { Properties.ButtonPressed -= value; }
		}
	}
}
namespace DevExpress.XtraEditors.ViewInfo {
	public class EditorButtonObjectCollection : CollectionBase {
		public EditorButtonObjectCollection() {
		}
		protected internal virtual void Assign(EditorButtonObjectCollection collection) {
			Clear();
			for(int n = 0; n < collection.Count; n++) {
				Add(collection[n].Clone());
			}
		}
		public virtual bool Compare(EditorButtonObjectCollection collection) {
			if(Count != collection.Count) return false;
			for(int n = 0; n < Count; n++) {
				if(!this[n].Compare(collection[n])) return false;
			}
			return true;
		}
		public EditorButtonObjectInfoArgs this[int index] { get { return List[index] as EditorButtonObjectInfoArgs; } }
		protected internal void Insert(int index, EditorButtonObjectInfoArgs info) {
			List.Insert(index, info);
		}
		public virtual int Add(EditorButtonObjectInfoArgs info) { return List.Add(info); }
		public virtual int Width {
			get {
				int res = 0;
				for(int n = 0; n < Count; n++) {
					res += this[n].Bounds.Width;
				}
				return res;
			}
		}
		public virtual EditorButtonObjectInfoArgs this[Point p] {
			get {
				foreach(EditorButtonObjectInfoArgs bi in this) {
					if(bi.Bounds.Contains(p)) return bi;
				}
				return null;
			}
		}
		public virtual EditorButtonObjectInfoArgs this[EditorButton button] {
			get {
				foreach(EditorButtonObjectInfoArgs bi in this) {
					if(bi.Button == button) return bi;
				}
				return null;
			}
		}
		public virtual void SetCache(GraphicsCache cache) {
			for(int n = 0; n < Count; n ++) this[n].Cache = cache;
		}
		public virtual void ResetState() {
			for(int n = 0; n < Count; n ++) this[n].State = ObjectState.Normal;
		}
		public virtual void Offset(int x, int y) {
			for(int n = 0; n < Count; n ++) this[n].OffsetContent(x, y);
		}
	}
	public class ButtonEditViewInfo : TextEditViewInfo {
		EditorButtonObjectCollection rightButtons, leftButtons;
		List<EditorButtonObjectInfoArgs> prevButtons;
		EditorButtonPainter editorButtonPainter;
		public ButtonEditViewInfo(RepositoryItem item) : base(item) {
			this.editorButtonPainter = CreateButtonPainter();
			this.rightButtons = new EditorButtonObjectCollection();
			this.leftButtons = new EditorButtonObjectCollection();
		}
		protected override void Assign(BaseControlViewInfo info) {
			base.Assign(info);
			ButtonEditViewInfo be = info as ButtonEditViewInfo;
			if(be == null) return;
			this.editorButtonPainter = be.editorButtonPainter;
			this.rightButtons.Assign(be.RightButtons);
			this.leftButtons.Assign(be.LeftButtons);
			this.glyphBounds = be.GlyphBounds;
		}
		protected override void OnPaintAppearanceChanged() {
			base.OnPaintAppearanceChanged();
			for(int n = 0; n < RightButtons.Count; n++) { RightButtons[n].BackAppearance = PaintAppearance; }
			for(int n = 0; n < LeftButtons.Count; n++) { LeftButtons[n].BackAppearance = PaintAppearance; }
		}
		[Browsable(false)]
		public override bool IsRequiredUpdateOnMouseMove { 
			get { 
				if(LeftButtons.Count == 0 && RightButtons.Count == 0) return base.IsRequiredUpdateOnMouseMove; 
				return true;
			} 
		}
		public override bool Compare(BaseEditViewInfo vi) {
			bool res = base.Compare(vi);
			if(!res) return false;
			ButtonEditViewInfo bvi = vi as ButtonEditViewInfo;
			if(bvi == null) return false;
			if(!LeftButtons.Compare(bvi.LeftButtons) || !RightButtons.Compare(bvi.RightButtons)) return false;
			return true;
		}
		public virtual EditorButtonObjectInfoArgs ButtonInfoByPoint(Point p) {
			EditorButtonObjectInfoArgs res = RightButtons[p];
			if(res == null) res = LeftButtons[p];
			return res;
		}
		public override ToolTipControlInfo GetToolTipInfo(Point point) {
			CheckShowHint();
			EditorButtonObjectInfoArgs btn = ButtonInfoByPoint(point);
			if(btn != null && btn.Button != null){
				if(btn.Button.SuperTip != null && !btn.Button.SuperTip.IsEmpty){
					ToolTipControlInfo info = new ToolTipControlInfo(string.Format("Button{0}", Item.Buttons.IndexOf(btn.Button)), btn.Button.ToolTip);
					info.SuperTip = btn.Button.SuperTip;
					return info;
				}
				else if(btn.Button.ToolTip != null && btn.Button.ToolTip.Length > 0) {
					return new ToolTipControlInfo(string.Format("Button{0}", Item.Buttons.IndexOf(btn.Button)), btn.Button.ToolTip);
				}
			}
			return base.GetToolTipInfo(point);
		}
		public override bool IsSupportIncrementalSearch { get { return Item.TextEditStyle != TextEditStyles.HideTextEditor; } }
		protected virtual bool IsAutoHotSingleButton { 
			get { 
				if(EditorButtonPainter.ButtonPainter is UltraFlatButtonObjectPainter) return true;
				return false; 
			} 
		}
		public virtual EditorButtonObjectInfoArgs ButtonInfoById(int buttonId) {
			if(buttonId >= 0 && buttonId < Item.Buttons.Count) return ButtonInfoByButton(Item.Buttons[buttonId]);
			return null;
		}
		public virtual EditorButtonObjectInfoArgs ButtonInfoByButton(EditorButton button) {
			EditorButtonObjectInfoArgs res = RightButtons[button];
			if(res == null) res = LeftButtons[button];
			return res;
		}
		public virtual EditorButtonPainter EditorButtonPainter { get { return editorButtonPainter; } }
		protected virtual EditorButtonPainter CreateButtonPainter() {
			return EditorButtonHelper.GetPainter(Item.ButtonsStyle, LookAndFeel, true);
		}
		public virtual EditorButtonObjectCollection RightButtons { get { return rightButtons; } }
		public virtual EditorButtonObjectCollection LeftButtons { get { return leftButtons; } }
		protected internal List<EditorButtonObjectInfoArgs> PrevButtons { 
			get { 
				if(prevButtons == null)
					prevButtons = new List<EditorButtonObjectInfoArgs>();
				return prevButtons; 
			} 
		}
		protected ObjectState GetPrevButtonState(EditorButton button) {
			foreach(EditorButtonObjectInfoArgs info in PrevButtons) {
				if(info.Button == button)
					return info.State;
			}
			return ObjectState.Normal;
		}
		protected virtual bool CanDisplayButton(EditorButton btn) { return true; }
		protected virtual Rectangle CalcButtons(GraphicsCache cache) {
			bool autoFit = Item.TextEditStyle == TextEditStyles.HideTextEditor;
			for(int n = 0; n < Item.Buttons.Count; n++) {
				EditorButton btn = Item.Buttons[n];
				if(!btn.Visible || !CanDisplayButton(btn)) continue;
				EditorButtonObjectInfoArgs info = CreateButtonInfo(btn, n);
				info.State = GetPrevButtonState(btn);
				info.FillBackground = FillBackground;
				CalcButtonState(info, n);
				info.Cache = cache;
				if(RightToLeft) {
					if(!btn.IsLeft)
						LeftButtons.Insert(0, info);
					else
						RightButtons.Insert(0, info);
				}
				else {
				if(btn.IsLeft) 
					LeftButtons.Add(info);
				else
					RightButtons.Add(info);
			}
			}
			PrevButtons.Clear();
			CalcButtonsRectsArgs cArgs = CreateCalcButtonsRectsArgs(ConstraintPadding(ClientRect));
			cArgs.UseAutoFit = autoFit;
			cArgs.ViewInfo = this;
			cArgs.Calc();
			RightButtons.SetCache(null);
			LeftButtons.SetCache(null);
			return cArgs.ClientRect;
		}
		Rectangle ConstraintPadding(Rectangle rect) {
			if(Item == null || Item.Padding == Padding.Empty) return rect;
			return RectangleHelper.Deflate(ClientRect, Item.Padding);
		}
		protected virtual Size CalcMinButtonBounds(EditorButtonObjectInfoArgs info) {
			return GetButtonPainter(info).CalcObjectMinBounds(info).Size;
		}
		protected override Size CalcClientSize(Graphics g) {
			Size size = base.CalcClientSize(g);
			Size buttonSize = Size.Empty;
			GInfo.AddGraphics(g);
			try {
				for(int n = 0; n < Item.Buttons.Count; n++) {
					EditorButton btn = Item.Buttons[n];
					if(!btn.Visible) continue;
					EditorButtonObjectInfoArgs info = CreateButtonInfo(btn, n);
					info.Cache = GInfo.Cache;
					Size bsize = CalcMinButtonBounds(info);
					buttonSize.Height = Math.Max(buttonSize.Height, bsize.Height);
					buttonSize.Width += bsize.Width;
				}
			}
			finally {
				GInfo.ReleaseGraphics();
			}
			size.Height = Math.Max(size.Height, buttonSize.Height);
			if(IsDrawButtons) size.Width += buttonSize.Width;
			size = UpdateByGlyphSize(size);
			return size;
		}
		protected virtual Size UpdateByGlyphSize(Size size) {
			if(GlyphDrawMode == TextGlyphDrawModeEnum.Text) return size;
			if(!ImageSize.IsEmpty) {
				Size iSize = ImageSize;
				size.Height = Math.Max(size.Height, iSize.Height);
				if(GlyphDrawMode == TextGlyphDrawModeEnum.Glyph) return new Size(iSize.Width, size.Height);
				size.Width += TextGlyphIndent + iSize.Width;
			}
			return size;
		}
		protected override bool UpdateObjectState() { 
			bool res = base.UpdateObjectState();
			for(int n = 0; n < LeftButtons.Count; n++) {
				if(CalcButtonState(LeftButtons[n], n)) res = true;
			}
			for(int n = 0; n < RightButtons.Count; n++) {
				if(CalcButtonState(RightButtons[n], n)) res = true;
			}
			return res;
		}
		protected virtual bool IsAnyButtonPressed(EditHitInfo hitInfo) {
			return hitInfo.HitTest == EditHitTest.Button;
		}
		protected virtual bool IsButtonPressed(EditorButtonObjectInfoArgs info) { 
			EditorButtonObjectInfoArgs press = PressedInfo.HitObject as EditorButtonObjectInfoArgs;
			return press != null && press.Button == info.Button;
		}
		protected virtual bool CalcButtonState(EditorButtonObjectInfoArgs info, int index) {
			ObjectState prevState = info.State;
			if(!Item.IsButtonEnabled(info.Button) || State == ObjectState.Disabled) {
				info.State = ObjectState.Disabled;
				return info.State != prevState;
			}
			ObjectState newState = ObjectState.Normal;
			newState = ObjectState.Normal;
			if(IsHotEdit(HotInfo)) {
				EditorButtonObjectInfoArgs hotButton = HotInfo.HitObject as EditorButtonObjectInfoArgs;
				if(IsAnyButtonPressed(PressedInfo)) {
					if(IsButtonPressed(info)) {
						newState = ObjectState.Pressed;
						if(hotButton != null && hotButton.Button == info.Button) {
							newState |= ObjectState.Hot;
						}
					}
					if(newState != prevState) OnBeforeButtonStateChanged(info, newState, info.Button.Index);
					info.State = newState;
					return newState != prevState;
				}
				if(hotButton != null) {
					if(hotButton.Button == info.Button) {
						newState = ObjectState.Hot;
					}
				}
			}
			if(IsAutoHotSingleButton && newState == ObjectState.Normal && (State != ObjectState.Disabled && (State != ObjectState.Normal || (OwnerEdit != null && OwnerEdit.EditorContainsFocus)))) {
				if(Item.Buttons.VisibleCount == 1) {
					newState = ObjectState.Hot;
				}
			}
			if(newState != prevState) OnBeforeButtonStateChanged(info, newState, info.Button.Index);
			info.State = newState;
			return newState != prevState;
		}
		protected virtual UserLookAndFeel ActiveLookAndFeel { get { return Item.LookAndFeel.ActiveLookAndFeel; } }
		public override bool AllowAnimation {
			get {
				if(!WindowsFormsSettings.GetAllowHoverAnimation(LookAndFeel.ActiveLookAndFeel))
					return false;
				if(ActiveLookAndFeel.UseWindowsXPTheme) return false;
				return base.AllowAnimation;
			}
		}
		protected virtual void OnBeforeButtonStateChanged(EditorButtonObjectInfoArgs info, ObjectState newState, int buttonId) {
			if(IsLockStateChanging) return;
			if(AllowAnimation)
				XtraAnimator.RemoveObject(OwnerEdit, buttonId);
			if(Bounds.IsEmpty || ((info.State | newState) & ObjectState.Pressed) != 0) return;
			if(newState == ObjectState.Pressed) return;
			if(!AllowAnimation) return;
			XtraAnimator.Current.AddBitmapAnimation(OwnerEdit, buttonId, XtraAnimator.Current.CalcAnimationLength(info.State, newState),
				null, new ObjectPaintInfo(GetButtonPainter(info), info), new BitmapAnimationImageCallback(OnGetButtonBitmap));
		}
		Bitmap OnGetButtonBitmap(BaseAnimationInfo info) {
			if(!(info.AnimationId is int)) return null;
			EditorButtonObjectInfoArgs actButton = ButtonInfoById((int)info.AnimationId);
			if(actButton == null) return null;
			return XtraAnimator.Current.CreateBitmap(GetButtonPainter(actButton), actButton);
		}
		public virtual bool CanPress(EditHitInfo hitInfo) {
			if(hitInfo.HitTest == EditHitTest.Button) {
				EditorButtonObjectInfoArgs bi = hitInfo.HitObject as EditorButtonObjectInfoArgs;
				if(Item.IsButtonEnabled(bi.Button) && bi.State != ObjectState.Disabled) return true;
			}
			return false;
		}
		protected virtual EditorButtonObjectInfoArgs CreateButtonInfo(EditorButton button, int index) {
			EditorButtonObjectInfoArgs res = new EditorButtonObjectInfoArgs(button, PaintAppearance);
			res.ContextHtml = GetHtmlContext();
			res.AllowGlyphSkinning = Item.GetAllowGlyphSkinning();
			res.AllowHtmlDraw = AllowHtmlString;
			res.Appearance.TextOptions.RightToLeft = RightToLeft;
			return res;
		}
		protected virtual CalcButtonsRectsArgs CreateCalcButtonsRectsArgs(Rectangle clientBounds) {
			return new CalcButtonsRectsArgs(LeftButtons, RightButtons, clientBounds);
		}
		protected virtual void ResetState() {
			LeftButtons.ResetState();
			RightButtons.ResetState();
		}
		public override void Offset(int x, int y) { 
			base.Offset(x, y);
			LeftButtons.Offset(x, y);
			RightButtons.Offset(x, y);
			if(!this.glyphBounds.IsEmpty) this.glyphBounds.Offset(x, y);
		}
		public override bool CompareHitInfo(EditHitInfo h1, EditHitInfo h2) {
			bool res = base.CompareHitInfo(h1, h2);
			if(res) return true;
			if(h1.HitTest == h2.HitTest && h1.HitTest == EditHitTest.Button) {
				EditorButtonObjectInfoArgs b1 = h1.HitObject as EditorButtonObjectInfoArgs,
					b2 = h2.HitObject as EditorButtonObjectInfoArgs;
				if(b1 != null && b2 != null && b1.Button == b2.Button) return true;
			}
			return h1.IsEquals(h2);
		}
		public override EditHitInfo CalcHitInfo(Point p) {
			EditHitInfo hi = base.CalcHitInfo(p);
			if(Bounds.Contains(p)) {
				EditorButtonObjectInfoArgs bi = ButtonInfoByPoint(p);
				if(bi != null) {
					hi.SetHitTest(EditHitTest.Button);
					hi.SetHitObject(bi);
				}
			}
			return hi;
		}
		public override bool DrawFocusRect {
			get {
				if(OwnerEdit != null && Item.TextEditStyle == TextEditStyles.DisableTextEditor) {
					return (Item.AllowFocused && OwnerEdit.EditorContainsFocus);
				}
				return false;
			}
		}
		public virtual ObjectPainter GetButtonPainter(EditorButtonObjectInfoArgs e) {
			return EditorButtonPainter;
		}
		public override bool AllowMaskBox { get { return Item.TextEditStyle == TextEditStyles.Standard && base.AllowMaskBox; } }
		public override bool AllowDrawText { get { return Item.TextEditStyle != TextEditStyles.HideTextEditor; } }
		public new RepositoryItemButtonEdit Item { get { return base.Item as RepositoryItemButtonEdit; } }
		public override void Clear() {
			base.Clear();
			SavePrevButtons();
			if(RightButtons != null) RightButtons.Clear();
			if(LeftButtons != null) LeftButtons.Clear();
		}
		private void SavePrevButtons() {
			if(LeftButtons == null || RightButtons == null)
				return;
			if(PrevButtons.Count == 0) {
				foreach(EditorButtonObjectInfoArgs button in LeftButtons)
					PrevButtons.Add(button);
				foreach(EditorButtonObjectInfoArgs button in RightButtons)
					PrevButtons.Add(button);
			}
		}
		protected override void CalcBestFitTextSize(Graphics g) {
			CalcTextSize(g, GlyphDrawMode != TextGlyphDrawModeEnum.Glyph);
		}
		protected override void CalcTextSize(Graphics g, bool useDisplayText) {
			if(!AllowDrawText) return;
			base.CalcTextSize(g, useDisplayText);
		}
		protected override void UpdateFromEditor() {
			base.UpdateFromEditor();
			if(OwnerEdit == null) return;
		}
		protected override void UpdatePainters() {
			base.UpdatePainters();
			this.editorButtonPainter = CreateButtonPainter();
		}
		protected virtual bool IsDrawButtons {
			get {
				if(DetailLevel == DetailLevel.Minimum && Item.TextEditStyle != TextEditStyles.HideTextEditor) return false;
				if(Item.Buttons.VisibleCount == 0) return false;
				return true;
			}
		}
		protected override void CalcContentRect(Rectangle bounds) {
			base.CalcContentRect(ConstraintPadding(bounds));
			if(IsDrawButtons) {
				GInfo.AddGraphics(null);
				try {
					Rectangle r = CalcButtons(GInfo.Cache);
					if(AllowDrawFocusRect) this.fFocusRect = r;
					base.CalcContentRect(r);
				}
				finally {
					GInfo.ReleaseGraphics();
				}
			}
			CalcGlyphInfo();
		}
		public virtual HorzAlignment GlyphAlignment { get { return HorzAlignment.Default; } }
		public virtual TextGlyphDrawModeEnum GlyphDrawMode { get { return TextGlyphDrawModeEnum.Text; } }
		public virtual bool IsExistImage { get { return ImageCollection.IsImageListImageExists(Images, ImageIndex); } }
		public virtual Size ImageSize { get { return ImageCollection.GetImageListSize(Images); } }
		public virtual int ImageIndex { get { return -1; } }
		public virtual object Images { get { return null; } }
		protected override bool IsTextToolTipPoint(Point point) {
			if(GlyphBounds.IsEmpty) return base.IsTextToolTipPoint(point);
			return ContentRect.Contains(point) && !MaskBoxRect.IsEmpty;
		}
		public const int TextGlyphIndent = 4;
		Rectangle glyphBounds = Rectangle.Empty;
		public Rectangle GlyphBounds { get { return glyphBounds; } }
		void CalcGlyphInfo() {
			this.glyphBounds = CalcGlyphBounds();
			if(GlyphBounds.IsEmpty) return;
			HorzAlignment align = GlyphAlignment;
			if(align == HorzAlignment.Center) {
				this.fMaskBoxRect = Rectangle.Empty;
				return;
			}
			Rectangle r = fMaskBoxRect;
			r.Width -= (GlyphBounds.Width + TextGlyphIndent);
			if(!FocusRect.IsEmpty) {
				fFocusRect.Width -= (GlyphBounds.Width + TextGlyphIndent / 2);
			}
			if(align != HorzAlignment.Far && align != HorzAlignment.Center && !RightToLeft) {
				r.X += (GlyphBounds.Width + TextGlyphIndent);
				if(!FocusRect.IsEmpty) {
					fFocusRect.X += (GlyphBounds.Width + TextGlyphIndent / 2);
				}
			}
			this.fMaskBoxRect = r;
		}
		protected virtual Rectangle CalcGlyphBounds() {
			Rectangle res = Rectangle.Empty;
			if(GlyphDrawMode == TextGlyphDrawModeEnum.Text || Item.TextEditStyle == TextEditStyles.HideTextEditor) return res;
			res = ContentRect;
			res.Y = ClientRect.Y;
			res.Size = ImageSize;
			if(GlyphDrawMode == TextGlyphDrawModeEnum.Glyph) 
				res.X = res.X + (ContentRect.Width - ImageSize.Width) / 2;
			if((GlyphAlignment == HorzAlignment.Far && !RightToLeft) || 
				(GlyphAlignment == HorzAlignment.Near && RightToLeft)) 
					res.X = ContentRect.Right - res.Width;
			res.Y = res.Y + (ClientRect.Height - ImageSize.Height) / 2;
			return res;
		}
		#region class CalcButtonsRectsArgs
		protected class CalcButtonsRectsArgs {
			public ButtonEditViewInfo ViewInfo;
			public int AutoWidthCount, AutoWidth;
			public EditorButtonObjectCollection LeftButtons, RightButtons;
			public Rectangle ClientRect;
			public bool IsLeft, UseAutoFit, AllowFixedWidth, NeedRecalc;
			public bool StopCalc;
			public CalcButtonsRectsArgs() : this(null, null, Rectangle.Empty) {  }
			public CalcButtonsRectsArgs(EditorButtonObjectCollection leftButtons, EditorButtonObjectCollection rightButtons, Rectangle clientRect) {
				ClearAutoWidthInfo();
				this.UseAutoFit = false;
				this.LeftButtons = leftButtons;
				this.RightButtons = rightButtons;
				this.ClientRect = clientRect;
				this.IsLeft = false;
				this.StopCalc = false;
				this.NeedRecalc = false;
				this.ViewInfo = null;
			}
			public virtual void Assign(CalcButtonsRectsArgs source) {
				this.ViewInfo = source.ViewInfo;
				this.LeftButtons = source.LeftButtons;
				this.RightButtons = source.RightButtons;
				this.ClientRect = source.ClientRect;
				this.AllowFixedWidth = source.AllowFixedWidth;
				this.AutoWidth = source.AutoWidth;
				this.AutoWidthCount = source.AutoWidthCount;
				this.IsLeft = source.IsLeft;
				this.StopCalc = source.StopCalc;
				this.UseAutoFit = source.UseAutoFit;
				this.NeedRecalc = source.NeedRecalc;
			}
			protected virtual void ClearAutoWidthInfo() {
				this.AllowFixedWidth = false;
				this.AutoWidthCount = 0;
				this.AutoWidth = 0;
			}
			public virtual void UpdateAutoWidthInfo() {
				UpdateAutoWidthInfoCore(new EditorButtonObjectCollection[] {LeftButtons, RightButtons});
			}
			bool IsFixedButton(EditorButtonObjectInfoArgs bi) {
				return (bi.Button.Width > -1 || (bi.IsCustomWidth && (bi.Button.Width > -1 || !bi.ImageSize.IsEmpty)));
			}
			protected virtual void UpdateAutoWidthInfoCore(EditorButtonObjectCollection[] colls) {
				ClearAutoWidthInfo();
				if(!UseAutoFit) return;
				int fixedWidth = 0, totalAutoWidth = 0, fixedWidthCount = 0;
				for(int k = 0; k < colls.Length; k++) {
					for(int n = 0; n < colls[k].Count; n++) {
						EditorButtonObjectInfoArgs bi = colls[k][n];
						if(IsFixedButton(bi)) {
							fixedWidthCount ++;
							int minWidth = ViewInfo.GetButtonPainter(bi).CalcObjectMinBounds(bi).Width;
							fixedWidth += Math.Max(bi.Button.Width, minWidth);
						}
						else
							AutoWidthCount ++;
					}
				}
				if(AutoWidthCount == 0) {
					AutoWidthCount = fixedWidthCount;
					fixedWidthCount = 0;
					fixedWidth = 0;
				}
				AllowFixedWidth = fixedWidthCount > 0;
				totalAutoWidth = (ClientRect.Width - fixedWidth);
				if(AutoWidthCount == 0) AutoWidth = totalAutoWidth;
				else AutoWidth = totalAutoWidth / AutoWidthCount;
			}
			public virtual void Calc() {
				if(UseAutoFit) {
					UpdateAutoWidthInfo();
				}
				CalcButtonRects(RightButtons);
				IsLeft = true;
				CalcButtonRects(LeftButtons);
			}
			protected virtual void CalcButtonRects(EditorButtonObjectCollection collection) {
				CalcButtonsRectsArgs clone = new CalcButtonsRectsArgs();
				clone.Assign(this);
				CalcButtonRectsCore(collection);
				for(int n = 0; n < 3; n ++) {
					if(this.NeedRecalc) {
						this.Assign(clone);
						this.UpdateAutoWidthInfo();
						CalcButtonRectsCore(collection);
					} else
						break;
				}
			}
			protected virtual void CalcButtonRectsCore(EditorButtonObjectCollection collection) {
				for(int n = collection.Count - 1; n >= 0; n --) {
					EditorButtonObjectInfoArgs info = collection[n];
					ObjectPainter painter = ViewInfo.GetButtonPainter(info);
					Rectangle buttonRect = painter.CalcObjectMinBounds(info);
					if(this.UseAutoFit) {
						if(!IsFixedButton(info) || !this.AllowFixedWidth) {
							int minWidth = buttonRect.Width;
							int width = (n == 0 ? this.ClientRect.Width : this.AutoWidth);
							if(LeftButtons.Count != 0)
								width = (AutoWidthCount == 1 ? this.ClientRect.Width : this.AutoWidth);
							if(info.Button.Kind == ButtonPredefines.Glyph && minWidth > width) {
								if(info.Graphics == null || info.Appearance.CalcTextSize(info.Cache.Graphics, info.Button.Caption, 0).Width > width) {
									info.SetAppearance(info.Appearance.Clone() as AppearanceObject);
									info.Appearance.TextOptions.HAlignment = HorzAlignment.Near;
								}
								minWidth = Math.Max(8, width);
							}
							width = Math.Max(width, minWidth);
							buttonRect.Width = width;
							this.AutoWidthCount --;
						} else {
							if(n == 0 && LeftButtons.Count == 0) {
								int width = Math.Max(this.ClientRect.Width, buttonRect.Width);
								buttonRect.Width = width;
							}
						}
					} 
					if(this.StopCalc || buttonRect.Width > this.ClientRect.Width) {
						collection.RemoveAt(n);
						if(this.UseAutoFit) 
							this.NeedRecalc = true;
						else 
							this.StopCalc = true;
						continue;
					}
					buttonRect = new Rectangle((this.IsLeft ? this.ClientRect.X : this.ClientRect.Right - buttonRect.Width), this.ClientRect.Y, buttonRect.Width, this.ClientRect.Height);
					Rectangle realButtonRect = buttonRect;
					if(IsUltraFlatButtonBorder) {
						realButtonRect = buttonRect;
						if(IsLeft) {
							realButtonRect.X --;
							realButtonRect.Width ++;
						} else {
							realButtonRect.Width ++;
						}
						if(UseAutoFit) {
							if(n == 0 && AutoWidthCount == 0) {
								if(!IsLeft) {
									realButtonRect.X --;
									realButtonRect.Width ++;
								} else {
									realButtonRect.Width ++;
								}
							}
						}
						realButtonRect.Inflate(0, 1);
					}
					info.Bounds = realButtonRect;
					painter.CalcObjectBounds(info);
					this.ClientRect.Width -= buttonRect.Width;
					if(this.IsLeft) this.ClientRect.X += buttonRect.Width;
				}
			}
			protected virtual bool IsUltraFlatButtonBorder {
				get {
					if(ViewInfo.EditorButtonPainter is SkinEditorButtonPainter) {
						SkinEditorButtonPainter sbp = ViewInfo.EditorButtonPainter as SkinEditorButtonPainter;
						return sbp.IsSingleLineBorder;
					}
					if(ViewInfo.EditorButtonPainter.ButtonPainter is UltraFlatButtonObjectPainter &&
						ViewInfo.BorderPainter is UltraFlatBorderPainter) return true;
					if(ViewInfo.EditorButtonPainter.ButtonPainter is XPAdvButtonPainter &&
						ViewInfo.BorderPainter is WindowsXPButtonEditBorderPainter) return DevExpress.Utils.WXPaint.WXPPainter.Default.IsVista;
					return false;
				}
			}
		}
		#endregion
	}
}
namespace DevExpress.XtraEditors.Drawing {
	public class ButtonEditPainter : TextEditPainter {
		 void DrawBackground(ControlGraphicsInfoArgs info) {
			ButtonEditViewInfo vi = info.ViewInfo as ButtonEditViewInfo;
			if(vi == null || vi.Item.Padding == Padding.Empty) return;
			bool drawParent = DrawParentBackground(vi, info.Cache);
			if(!drawParent && vi.FillBackground) {
				info.Cache.Paint.FillRectangle(info.Graphics, vi.PaintAppearance.GetBackBrush(info.Cache), vi.ClientRect);
			}
		}
		protected override void DrawContent(ControlGraphicsInfoArgs info) {
			DrawBackground(info);
			base.DrawContent(info);
			DrawButtons(info);
			DrawGlyph(info);
		}
		protected virtual void DrawGlyph(ControlGraphicsInfoArgs info) {
			ButtonEditViewInfo be = info.ViewInfo as ButtonEditViewInfo;
			if(!be.GlyphBounds.IsEmpty && be.IsExistImage && CheckRectangle(be.ClientRect, be.GlyphBounds)) { 
				DrawGlyphCore(info, be);
			}
			else {
				if(be.FillBackground)
					info.Cache.Paint.FillRectangle(info.Graphics, be.PaintAppearance.GetBackBrush(info.Cache), UpdateRectangle(be.ClientRect, be.GlyphBounds));
			}
		}
		protected virtual void DrawGlyphCore(ControlGraphicsInfoArgs info, ButtonEditViewInfo be) {
			info.Cache.Paint.DrawImage(info, be.Images, be.ImageIndex, be.GlyphBounds, be.State != ObjectState.Disabled);
		}
		protected virtual void DrawButtons(ControlGraphicsInfoArgs info) {
			ButtonEditViewInfo vi = info.ViewInfo as ButtonEditViewInfo;
			DrawButtons(info, vi.RightButtons, true);
			DrawButtons(info, vi.LeftButtons, true);
			DrawButtons(info, vi.RightButtons, false);
			DrawButtons(info, vi.LeftButtons, false);
		}
		protected virtual void DrawButtons(ControlGraphicsInfoArgs info, EditorButtonObjectCollection collection, bool drawNormalDisabled) {
			ButtonEditViewInfo vi = info.ViewInfo as ButtonEditViewInfo;
			foreach(EditorButtonObjectInfoArgs bi in collection) {
				bool isNormalOrDisabled = bi.State == ObjectState.Normal || bi.State == ObjectState.Disabled;
				if(isNormalOrDisabled != drawNormalDisabled) continue;
				bi.Cache = info.Cache;
				DrawButton(vi, bi);
				bi.Cache = null;
			}
		}
		protected virtual void DrawButton(ButtonEditViewInfo viewInfo, EditorButtonObjectInfoArgs info) {
			ObjectPainter painter = viewInfo.GetButtonPainter(info);
			CustomDrawButtonEventArgs e = new CustomDrawButtonEventArgs(info, painter);
			e.SetDefaultDraw(() =>
			{
				if(XtraAnimator.Current.DrawFrame(info.Cache, viewInfo.OwnerEdit, info.Button.Index)) {
					EditorButtonPainter eb = painter as EditorButtonPainter;
					if(eb != null) eb.DrawTextOnly(info);
					return;
				}
				painter.DrawObject(info);
			});
			viewInfo.Item.RaiseCustomDrawButton(e);
			e.DefaultDraw();
		}
		protected override bool ShouldUpdateStringInfoAppearanceColors { get { return true; } }
	}
}
