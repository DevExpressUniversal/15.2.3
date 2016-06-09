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
namespace DevExpress.XtraEditors.Repository {
	[ToolboxItem(false)]
	public class RepositoryItemBaseSpinEdit : RepositoryItemPopupBase {
		int _spinButtonIndex;
		SpinStyles _spinStyle;
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never), Browsable(false), Obsolete(ObsoleteText.SRObsoletePropertiesText)]
		public new RepositoryItemBaseSpinEdit Properties { get { return this; } }
		public RepositoryItemBaseSpinEdit() {
			this._spinButtonIndex = 0;
			this._spinStyle = SpinStyles.Vertical;
			this.fEditValueChangedFiringMode = EditValueChangedFiringMode.Buffered;
		}
		public override void Assign(RepositoryItem item) {
			RepositoryItemBaseSpinEdit source = item as RepositoryItemBaseSpinEdit;
			BeginUpdate(); 
			try {
				base.Assign(item);
				if(source == null) return;
				this._spinButtonIndex = source.SpinButtonIndex;
				this._spinStyle = source.SpinStyle;
			}
			finally {
				EndUpdate();
			}
		}
		[DXCategory(CategoryName.Behavior), 
#if !SL
	DevExpressXtraEditorsLocalizedDescription("RepositoryItemBaseSpinEditSpinStyle"),
#endif
 DefaultValue(SpinStyles.Vertical), DevExpress.Utils.Design.SmartTagProperty("Spin Style", "Editor Style", DevExpress.Utils.Design.SmartTagActionType.RefreshBoundsAfterExecute)]
		public virtual SpinStyles SpinStyle {
			get { return _spinStyle; }
			set {
				if(SpinStyle == value) return;
				_spinStyle = value;
				OnPropertiesChanged();
			}
		}
		[Browsable(false)]
		public override string EditorTypeName { get { return "BaseSpinEdit"; } }
		[DXCategory(CategoryName.Behavior), 
#if !SL
	DevExpressXtraEditorsLocalizedDescription("RepositoryItemBaseSpinEditSpinButtonIndex"),
#endif
 DefaultValue(0)]
		public virtual int SpinButtonIndex { 
			get { return _spinButtonIndex; }
			set {
				if(!IsLoading) {
					if(value >= Buttons.Count) value = Buttons.Count - 1;
				}
				if(value < 0) value = 0;
				if(SpinButtonIndex == value) return;
				_spinButtonIndex = value;
				OnPropertiesChanged();
			}
		}
		[Obsolete(ObsoleteText.SRObsoleteUseCtrlIncrement)]
		[DXCategory(CategoryName.Behavior)]
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[DefaultValue(true)]
		public virtual bool UseCtrlIncrement {
			get { return true; }
			set { }
		}
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("RepositoryItemBaseSpinEditEditValueChangedFiringMode"),
#endif
 DXCategory(CategoryName.Behavior), DefaultValue(EditValueChangedFiringMode.Buffered)]
		public override EditValueChangedFiringMode EditValueChangedFiringMode {
			get { return base.EditValueChangedFiringMode; }
			set { base.EditValueChangedFiringMode = value; }
		}
		protected internal override bool IsButtonEnabled(EditorButton button) {
			if(!base.IsButtonEnabled(button)) return false;
			if(ReadOnly && button.Index == SpinButtonIndex) return false;
			return true;
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override AppearanceObject AppearanceDropDown {
			get {
				return base.AppearanceDropDown;
			}
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override bool ShowPopupShadow {
			get {
				return base.ShowPopupShadow;
			}
			set {
				base.ShowPopupShadow = value;
			}
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override bool SuppressMouseEventOnOuterMouseClick {
			get {
				return base.SuppressMouseEventOnOuterMouseClick;
			}
			set {
				base.SuppressMouseEventOnOuterMouseClick = value;
			}
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override DefaultBoolean AllowDropDownWhenReadOnly {
			get {
				return base.AllowDropDownWhenReadOnly;
			}
			set {
				base.AllowDropDownWhenReadOnly = value;
			}
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override Size PopupFormSize {
			get {
				return base.PopupFormSize;
			}
			set {
				base.PopupFormSize = value;
			}
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override Size PopupFormMinSize {
			get {
				return base.PopupFormMinSize;
			}
			set {
				base.PopupFormMinSize = value;
			}
		}
		[DefaultValue(PopupBorderStyles.Default), Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override PopupBorderStyles PopupBorderStyle {
			get {
				return base.PopupBorderStyle;
			}
			set {
				base.PopupBorderStyle = value;
			}
		}
		bool ShouldSerializeCloseUpKey() { return CloseUpKey.Key != Keys.F4; }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override KeyShortcut CloseUpKey {
			get {
				return base.CloseUpKey;
			}
			set {
				base.CloseUpKey = value;
			}
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override ResizeMode PopupResizeMode {
			get {
				return base.PopupResizeMode;
			}
			set {
				base.PopupResizeMode = value;
			}
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override ShowDropDown ShowDropDown {
			get {
				return base.ShowDropDown;
			}
			set {
				base.ShowDropDown = value;
			}
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override int ActionButtonIndex {
			get {
				return base.ActionButtonIndex;
			}
			set {
				base.ActionButtonIndex = value;
			}
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override event CancelEventHandler QueryCloseUp {
			add { this.Events.AddHandler(queryCloseUp, value); }
			remove { this.Events.RemoveHandler(queryCloseUp, value); }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override event EventHandler Popup {
			add { this.Events.AddHandler(popup, value); }
			remove { this.Events.RemoveHandler(popup, value); }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override event CancelEventHandler QueryPopUp {
			add { this.Events.AddHandler(queryPopUp, value); }
			remove { this.Events.RemoveHandler(queryPopUp, value); }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override event CloseUpEventHandler CloseUp {
			add { this.Events.AddHandler(closeUp, value); }
			remove { this.Events.RemoveHandler(closeUp, value); }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override event ClosedEventHandler Closed {
			add { this.Events.AddHandler(closed, value); }
			remove { this.Events.RemoveHandler(closed, value); }
		}
	}
}
namespace DevExpress.XtraEditors {
	[ToolboxItem(false), Designer("DevExpress.XtraEditors.Design.BaseSpinEditDesigner, " + AssemblyInfo.SRAssemblyEditorsDesign)]
	public class BaseSpinEdit : PopupBaseEdit {
		Timer spinTimer;
		SpinButtonEnum spinTimerAction;
		public BaseSpinEdit() {
			this.spinTimerAction = SpinButtonEnum.None;
			this.spinTimer = new Timer();
			this.spinTimer.Tick += new EventHandler(OnSpinTimer_Tick);
		}
		protected virtual Timer SpinTimer { get { return spinTimer; } }
		protected SpinButtonEnum SpinTimerAction { get { return spinTimerAction; } }
		protected override void Dispose(bool disposing) {
			fDisposing = true;
			if(disposing) {
				if(spinTimer != null) {
					spinTimer.Tick -= new EventHandler(OnSpinTimer_Tick);
					spinTimer.Dispose();
					spinTimer = null;
				}
			}
			base.Dispose(disposing);
		}
		[Browsable(false)]
		public override string EditorTypeName { get { return "BaseSpinEdit"; } }
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("BaseSpinEditProperties"),
#endif
 DXCategory(CategoryName.Properties), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public new RepositoryItemBaseSpinEdit Properties { get { return base.Properties as RepositoryItemBaseSpinEdit; } }
		protected override void OnPressButton(EditorButtonObjectInfoArgs buttonInfo) {
			SpinButtonObjectInfoArgs spin = buttonInfo.ParentButtonInfo as SpinButtonObjectInfoArgs;
			if(spin != null) {
				if(GetSpinButton(buttonInfo) == SpinButtonEnum.Increment)
					OnPressSpinUp();
				else
					OnPressSpinDown();
				return;
			}
			base.OnPressButton(buttonInfo);
		}
		protected override void OnMouseUp(MouseEventArgs e) {
			if(e.Button == MouseButtons.Left) {
				StopSpinTimer();
			}
			base.OnMouseUp(e);
		}
		protected SpinButtonEnum GetSpinButton(EditorButtonObjectInfoArgs info) {
			if(info != null && info.ParentButtonInfo is SpinButtonObjectInfoArgs) {
				if(info.Button.Kind == ButtonPredefines.SpinRight | info.Button.Kind == ButtonPredefines.SpinUp) return SpinButtonEnum.Increment;
				if(info.Button.Kind == ButtonPredefines.SpinLeft | info.Button.Kind == ButtonPredefines.SpinDown) return SpinButtonEnum.Decrement;
			}
			return SpinButtonEnum.None;
		}
		protected internal override void OnClickButton(EditorButtonObjectInfoArgs buttonInfo) {
			SpinButtonObjectInfoArgs spin = buttonInfo.ParentButtonInfo as SpinButtonObjectInfoArgs;
			if(spin != null) return;
			base.OnClickButton(buttonInfo);
		}
		protected virtual void OnPressSpinUp() {
			if(Properties.ReadOnly) return;
			DoSpin(true);
			StartSpinTimer(SpinButtonEnum.Increment);
		}
		protected virtual void OnPressSpinDown() {
			if(Properties.ReadOnly) return;
			DoSpin(false);
			StartSpinTimer(SpinButtonEnum.Decrement);
		}
		protected virtual void OnSpinTimer_Tick(object sender, EventArgs e) {
			SpinButtonEnum pressedSpin = GetSpinButton(ViewInfo.PressedInfo.HitObject as EditorButtonObjectInfoArgs);
			SpinButtonEnum hotSpin = GetSpinButton(ViewInfo.HotInfo.HitObject as EditorButtonObjectInfoArgs);
			if(SpinTimerAction == SpinButtonEnum.None || pressedSpin == SpinButtonEnum.None) {
				StopSpinTimer();
			}
			if(hotSpin != pressedSpin) { 
				return; 
			}
			switch(SpinTimerAction) {
				case SpinButtonEnum.Increment:
					DoSpin(true);
					break;
				case SpinButtonEnum.Decrement:
					DoSpin(false);
					break;
			}
			int interval = SpinTimer.Interval - TimerDecrementValue;
			if(interval < 10) interval = 10;
			SpinTimer.Interval = interval;
		}
		protected virtual void StopSpinTimer() {
			this.spinTimerAction = SpinButtonEnum.None;
			SpinTimer.Stop();
		}
		protected virtual int TimerStartValue { get { return 500; } }
		protected virtual int TimerDecrementValue { get { return 50; } }
		protected virtual void StartSpinTimer(SpinButtonEnum action) {
			if(Properties.ReadOnly || action == SpinButtonEnum.None) {
				StopSpinTimer();
				return;
			}
			SpinTimer.Interval = TimerStartValue;
			this.spinTimerAction = action;
			SpinTimer.Start();
		}
		protected enum SpinButtonEnum { None, Increment, Decrement };
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override void ShowPopup() {
			base.ShowPopup();
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override void ClosePopup() {
			base.ClosePopup();
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override void CancelPopup() {
			base.CancelPopup();
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override void RefreshEditValue() {
			base.RefreshEditValue();
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override event CancelEventHandler QueryCloseUp {
			add { Properties.QueryCloseUp += value; }
			remove { Properties.QueryCloseUp -= value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override event EventHandler Popup {
			add { Properties.Popup += value; }
			remove { Properties.Popup -= value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override event CancelEventHandler QueryPopUp {
			add { Properties.QueryPopUp += value; }
			remove { Properties.QueryPopUp -= value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override event CloseUpEventHandler CloseUp {
			add { Properties.CloseUp += value; }
			remove { Properties.CloseUp -= value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override event ClosedEventHandler Closed {
			add { Properties.Closed += value; }
			remove { Properties.Closed -= value; }
		}
		protected override Popup.PopupBaseForm CreatePopupForm() {
			return null;
		}
	}
}
namespace DevExpress.XtraEditors.ViewInfo {
	public class BaseSpinEditViewInfo : ButtonEditViewInfo {
		SpinButtonObjectPainter spinButtonPainter;
		public BaseSpinEditViewInfo(RepositoryItem item) : base(item) {
			this.spinButtonPainter = CreateSpinButtonPainter();
		}
		protected override void Assign(BaseControlViewInfo info) {
			base.Assign(info);
			BaseSpinEditViewInfo be = info as BaseSpinEditViewInfo;
			if(be == null) return;
			this.spinButtonPainter = be.spinButtonPainter;
		}
		protected override bool IsAutoHotSingleButton { get { return false; } }
		public SpinButtonObjectPainter SpinButtonPainter { get { return spinButtonPainter; } }
		public new RepositoryItemBaseSpinEdit Item { get { return base.Item as RepositoryItemBaseSpinEdit; } }
		protected virtual SpinButtonObjectPainter CreateSpinButtonPainter() {
			return new SpinButtonObjectPainter(EditorButtonPainter);
		}
		SpinButtonObjectInfoArgs FindSpin(EditorButtonObjectCollection buttons) {
			SpinButtonObjectInfoArgs spin = null;
			foreach(EditorButtonObjectInfoArgs info in buttons) {
				spin = info as SpinButtonObjectInfoArgs;
				if(spin != null) return spin;
			}
			return null;
		}
		public override EditorButtonObjectInfoArgs ButtonInfoById(int buttonId) {
			EditorButtonObjectInfoArgs res = base.ButtonInfoById(buttonId);
			if(res != null) return res;
			SpinButtonObjectInfoArgs spin = FindSpin(RightButtons);
			if(spin == null) spin = FindSpin(LeftButtons);
			if(spin != null) {
				if(buttonId == -1000) return spin.UpButtonInfo;
				if(buttonId == -1001) return spin.DownButtonInfo;
			}
			return null;
		}
		protected override Size CalcMinButtonBounds(EditorButtonObjectInfoArgs info) {
			Size ret = base.CalcMinButtonBounds(info);
			if(info is SpinButtonObjectInfoArgs && Item != null && Item.SpinStyle == SpinStyles.Vertical) {
				if(spinButtonPainter != null && !(spinButtonPainter.ButtonPainter is WindowsXPEditorButtonPainter))
				ret.Height = ret.Height * 2;
			}
			return ret;
		}
		protected override void UpdatePainters() {
			base.UpdatePainters();
			this.spinButtonPainter = CreateSpinButtonPainter();
		}
		protected override EditorButtonObjectInfoArgs CreateButtonInfo(EditorButton button, int index) {
			if(Item.SpinButtonIndex == index) {
				return new SpinButtonObjectInfoArgs(this, button, PaintAppearance, Item.SpinStyle);
			}
			return base.CreateButtonInfo(button, index);
		}
		public override ObjectPainter GetButtonPainter(EditorButtonObjectInfoArgs e) {
			if(e is SpinButtonObjectInfoArgs) return SpinButtonPainter;
			return base.GetButtonPainter(e);
		}
		public override EditorButtonObjectInfoArgs ButtonInfoByPoint(Point p) {
			EditorButtonObjectInfoArgs res = base.ButtonInfoByPoint(p);
			if(res is SpinButtonObjectInfoArgs) {
				SpinButtonObjectInfoArgs ee = res as SpinButtonObjectInfoArgs;
				if(ee.DownButtonInfo.Bounds.Contains(p)) return ee.DownButtonInfo;
				return ee.UpButtonInfo;
			}
			return res;
		}
		protected override bool CalcButtonState(EditorButtonObjectInfoArgs info, int index) {
			SpinButtonObjectInfoArgs ee = info as SpinButtonObjectInfoArgs;
			if(ee == null) {
				return base.CalcButtonState(info, index);
			}
			ObjectState prevUpState = ee.UpButtonInfo.State, prevDownState = ee.DownButtonInfo.State,
				newUpState = ObjectState.Normal, newDownState = ObjectState.Normal;
			if(!Item.IsButtonEnabled(ee.Button) || State == ObjectState.Disabled) {
				ee.UpButtonInfo.State = ee.DownButtonInfo.State = ee.State = ObjectState.Disabled;
				return prevUpState != ee.UpButtonInfo.State || prevDownState != ee.DownButtonInfo.State;
			}
			EditorButtonObjectInfoArgs hotButton = HotInfo.HitObject as EditorButtonObjectInfoArgs;
			if(PressedInfo.HitTest == EditHitTest.Button) {
				EditorButtonObjectInfoArgs press = PressedInfo.HitObject as EditorButtonObjectInfoArgs;
				EditorButtonObjectInfoArgs pressedSpin = null;
				if(IsEqualsButtons(press, ee.UpButtonInfo)) {
					pressedSpin = ee.UpButtonInfo;
				}
				if(IsEqualsButtons(press, ee.DownButtonInfo)) {
					pressedSpin = ee.DownButtonInfo;
				}
				if(pressedSpin != null) {
					ObjectState st = ObjectState.Pressed;
					if(IsEqualsButtons(pressedSpin, hotButton)) {
						st |= ObjectState.Hot;
					}
					if(pressedSpin == ee.DownButtonInfo)
						newDownState = st;
					else
						newUpState = st;
				}
			} else {
				if(IsEqualsButtons(hotButton, ee.UpButtonInfo)) {
					newUpState = ObjectState.Hot;
				} else {
					if(IsEqualsButtons(hotButton, ee.DownButtonInfo)) {
						newDownState = ObjectState.Hot;
					}
				}
			}
			if(prevUpState != newUpState) OnBeforeButtonStateChanged(ee.UpButtonInfo, newUpState, -1000);
			if(prevDownState != newDownState) OnBeforeButtonStateChanged(ee.DownButtonInfo, newDownState, -1001);
			ee.UpButtonInfo.State = newUpState;
			ee.DownButtonInfo.State = newDownState;
			return prevUpState != ee.UpButtonInfo.State || prevDownState != ee.DownButtonInfo.State;
		}
		bool IsEqualsButtons(EditorButtonObjectInfoArgs b1, EditorButtonObjectInfoArgs b2) {
			if(b1 == null || b2 == null) return false;
			return b1.Button.Kind == b2.Button.Kind;
		}
	}
}
namespace DevExpress.XtraEditors.Drawing {
	public class SpinButtonObjectPainter : ObjectPainter {
		EditorButtonPainter buttonPainter;
		public SpinButtonObjectPainter(EditorButtonPainter buttonPainter) {
			this.buttonPainter = buttonPainter;
		}
		public EditorButtonPainter ButtonPainter { get { return buttonPainter; } }
		public override Rectangle CalcObjectBounds(ObjectInfoArgs e) {
			SpinButtonObjectInfoArgs ee = e as SpinButtonObjectInfoArgs;
			Rectangle upButton, downButton;
			upButton = downButton = ee.Bounds;
			if(ee.SpinStyle == SpinStyles.Vertical) {
				downButton.Height = ee.Bounds.Height / 2;
				upButton.Height = ee.Bounds.Height - downButton.Height;
				downButton.Y = upButton.Bottom;
			} else {
				downButton.Width = ee.Bounds.Width / 2;
				upButton.Width = ee.Bounds.Width - downButton.Width;
				downButton.X = upButton.Right;
			}
			ee.UpButtonInfo.Bounds = upButton;
			ee.DownButtonInfo.Bounds = downButton;
			ButtonPainter.CalcObjectBounds(ee.UpButtonInfo);
			ButtonPainter.CalcObjectBounds(ee.DownButtonInfo);
			return e.Bounds;
		}
		public override Rectangle GetObjectClientRectangle(ObjectInfoArgs e) {
			return ButtonPainter.GetObjectClientRectangle(e);
		}
		public override Rectangle CalcBoundsByClientRectangle(ObjectInfoArgs e, Rectangle client) {
			return ButtonPainter.CalcBoundsByClientRectangle(e, client);
		}
		public override Rectangle CalcObjectMinBounds(ObjectInfoArgs e) {
			SpinButtonObjectInfoArgs ee = e as SpinButtonObjectInfoArgs;
			Size res = Size.Empty;
			Size sizeUp = ButtonPainter.CalcObjectMinBounds(ee.UpButtonInfo).Size;
			Size sizeDown = ButtonPainter.CalcObjectMinBounds(ee.DownButtonInfo).Size;
			if(ee.SpinStyle == SpinStyles.Vertical) {
				res.Width = Math.Max(sizeUp.Width, sizeDown.Width);
				res.Height = (sizeUp.Height + sizeDown.Height) / 2;
			} else {
				res.Height = Math.Max(sizeUp.Height, sizeDown.Height);
				res.Width = (sizeUp.Width + sizeDown.Width);
			}
			return new Rectangle(Point.Empty, res);
		}
		public override void DrawObject(ObjectInfoArgs e) {
			SpinButtonObjectInfoArgs ee = e as SpinButtonObjectInfoArgs;
			ee.UpButtonInfo.BackAppearance = ee.DownButtonInfo.BackAppearance = ee.BackAppearance;
			DrawButton(ee, ee.UpButtonInfo, -1000);
			DrawButton(ee, ee.DownButtonInfo, -1001);
		}
		protected virtual void DrawButton(SpinButtonObjectInfoArgs ee, EditorButtonObjectInfoArgs info, int buttonId) {
			if(XtraAnimator.Current.DrawFrame(info.Cache, ee.ViewInfo.OwnerEdit, buttonId)) return;
			ButtonPainter.DrawObject(info);
		}
	}
	public class SpinButtonObjectInfoArgs : EditorButtonObjectInfoArgs {
		EditorButtonObjectInfoArgs upButtonInfo, downButtonInfo;
		SpinStyles spinStyle;
		BaseSpinEditViewInfo viewInfo;
		public SpinButtonObjectInfoArgs(BaseSpinEditViewInfo viewInfo, EditorButton button, AppearanceObject backStyle, SpinStyles spinStyle) : this(null, viewInfo, button, backStyle, spinStyle) { }
		public SpinButtonObjectInfoArgs(GraphicsCache cache, BaseSpinEditViewInfo viewInfo, EditorButton button, AppearanceObject backStyle, SpinStyles spinStyle) : base(cache, button, backStyle) {
			this.viewInfo = viewInfo;
			this.spinStyle = spinStyle;
			this.SetAppearance(button.Appearance);
			EditorButton upButton = new EditorButton(SpinStyle == SpinStyles.Vertical ? ButtonPredefines.SpinUp : ButtonPredefines.SpinLeft);
			EditorButton downButton = new EditorButton(SpinStyle == SpinStyles.Vertical ? ButtonPredefines.SpinDown : ButtonPredefines.SpinRight);
			upButton.Appearance.Assign(button.Appearance);
			downButton.Appearance.Assign(button.Appearance);
			this.upButtonInfo = new EditorButtonObjectInfoArgs(upButton, backStyle);
			this.downButtonInfo = new EditorButtonObjectInfoArgs(downButton, backStyle);
			this.upButtonInfo.Button.Width = this.downButtonInfo.Button.Width = button.Width;
			UpButtonInfo.ParentButtonInfo = DownButtonInfo.ParentButtonInfo = this;
		}
		protected override EditorButtonObjectInfoArgs CreateInstance() {
			return new SpinButtonObjectInfoArgs(ViewInfo, Button, null, SpinStyle);
		}
		public BaseSpinEditViewInfo ViewInfo { get { return viewInfo; } }
		public override void Assign(ObjectInfoArgs info) {
			base.Assign(info);
			SpinButtonObjectInfoArgs bInfo = info as SpinButtonObjectInfoArgs;
			if(bInfo == null) return;
			this.spinStyle = bInfo.spinStyle;
			this.upButtonInfo.Assign(bInfo.upButtonInfo);
			this.downButtonInfo.Assign(bInfo.downButtonInfo);
			this.viewInfo = bInfo.ViewInfo;
		}
		public override void OffsetContent(int x, int y) {
			base.OffsetContent(x, y);
			UpButtonInfo.OffsetContent(x, y);
			DownButtonInfo.OffsetContent(x, y);
		}
		public override bool Compare(EditorButtonObjectInfoArgs info) {
			bool res = base.Compare(info);
			if(!res) return false;
			SpinButtonObjectInfoArgs e = info as SpinButtonObjectInfoArgs;
			if(e == null) return false;
			return UpButtonInfo.Compare(e.UpButtonInfo) && DownButtonInfo.Compare(e.DownButtonInfo);
		}
		public override bool FillBackground { 
			get { return base.FillBackground; }
			set {
				base.FillBackground = value;
				UpButtonInfo.FillBackground = DownButtonInfo.FillBackground = value;
			}
		}
		public override ObjectState State { get { return ObjectState.Hot; } }
		public virtual SpinStyles SpinStyle { get { return spinStyle; } }
		public virtual EditorButton UpButton { get { return UpButtonInfo.Button; } }
		public virtual EditorButton DownButton { get { return DownButtonInfo.Button; } }
		public virtual EditorButtonObjectInfoArgs UpButtonInfo { get { return upButtonInfo; } }
		public virtual EditorButtonObjectInfoArgs DownButtonInfo { get { return downButtonInfo; } }
		public override GraphicsCache Cache {
			get { return base.Cache; }
			set {
				base.Cache = value;
				UpButtonInfo.Cache = value;
				DownButtonInfo.Cache = value;
			}
		}
	}
} 
