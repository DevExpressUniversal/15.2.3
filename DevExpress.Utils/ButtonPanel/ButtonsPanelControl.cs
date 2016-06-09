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
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using DevExpress.Utils;
using DevExpress.Utils.Drawing;
using DevExpress.XtraBars.Docking2010;
using DevExpress.XtraEditors.ButtonPanel;
namespace DevExpress.XtraEditors.ButtonsPanelControl {
	public class ButtonsPanelControl : BaseButtonsPanel {
		public ButtonsPanelControl(IButtonsPanelOwner owner)
			: base(owner) {
		}
		protected override IButtonsPanelViewInfo CreateViewInfo() {
			return new ButtonsPanelControlViewInfo(this);
		}
		#region Events
		public event BaseButtonEventHandler ButtonClick;
		public event BaseButtonEventHandler ButtonUnchecked;
		public event BaseButtonEventHandler ButtonChecked;
		protected override void RaiseButtonClick(IBaseButton button) {
			if(ButtonClick != null)
				ButtonClick(this, new BaseButtonEventArgs(button));
		}
		protected override void RaiseButtonChecked(IBaseButton button) {
			if(ButtonChecked != null)
				ButtonChecked(this, new BaseButtonEventArgs(button));
		}
		protected override void RaiseButtonUnchecked(IBaseButton button) {
			if(ButtonUnchecked != null)
				ButtonUnchecked(this, new BaseButtonEventArgs(button));
		}
		#endregion Events
	}
}
namespace DevExpress.XtraEditors.ButtonPanel {
	public enum RotationAngle { None = 0, Rotate90 = 90, Rotate270 = 270 }
	public class GroupBoxButtonsPanel : BaseButtonsPanel {
		public bool  RaiseEvents { get; set; }
		public GroupBoxButtonsPanel(IButtonsPanelOwner owner) : base(owner) {
			RaiseEvents = true;
		}
		public new IGroupBoxButtonsPanelOwner Owner {
			get { return (IGroupBoxButtonsPanelOwner) base.Owner; }
		}
		protected override IButtonsPanelViewInfo CreateViewInfo() {
			return new GroupBoxButtonsPanelViewInfo(this);
		}
		protected override void RaiseButtonChecked(IBaseButton button) {
			if(!RaiseEvents) return;
			Owner.RaiseButtonsPanelButtonChecked(new BaseButtonEventArgs(button));
		}
		protected override void RaiseButtonUnchecked(IBaseButton button) {
			if(!RaiseEvents) return;
			Owner.RaiseButtonsPanelButtonUnchecked(new BaseButtonEventArgs(button));
		}
		protected override void RaiseButtonClick(IBaseButton button) {
			if(!RaiseEvents) return;
			Owner.RaiseButtonsPanelButtonClick(new BaseButtonEventArgs(button));
		}
	}
	public class BaseButtonsPanel : IButtonsPanel {
		IButtonsPanelOwner ownerCore;
		IButtonsPanelHandler handlerCore;
		IButtonsPanelViewInfo viewInfoCore;
		BaseButtonCollection buttonsCore;
		bool showToolTipCore;
		bool wrapButtonsCore;
		public BaseButtonsPanel(IButtonsPanelOwner owner) {
			this.ownerCore = owner;
			OnCreate();
		}
		protected virtual void OnCreate() {
			showToolTipCore = true;
			wrapButtonsCore = false;
			buttonRotationAngleCore = RotationAngle.None;
			buttonsCore = CreateButtons();
			handlerCore = CreateHandler();
			viewInfoCore = CreateViewInfo();
			Buttons.CollectionChanged += OnButtonsCollectionChanged;
		}
		protected virtual void OnDispose() {
			if(Buttons != null)
				Buttons.CollectionChanged -= OnButtonsCollectionChanged;
			buttonsCore = null;
		}
		public void Dispose() {
			OnDispose();
		}
		public IButtonsPanelOwner Owner {
			get { return ownerCore; }
		}
		public IButtonsPanelHandler Handler {
			get { return handlerCore; }
		}
		public IButtonsPanelViewInfo ViewInfo {
			get { return viewInfoCore; }
		}
		#region Properties
		Rectangle boundsCore;
		[Browsable(false)]
		public Rectangle Bounds {
			get { return boundsCore; }
			set { SetValue(ref boundsCore, value, "Bounds"); }
		}
		public BaseButtonCollection Buttons {
			get { return buttonsCore; }
		}
		RotationAngle buttonRotationAngleCore;
		[Browsable(false)]
		public RotationAngle ButtonRotationAngle {
			get { return buttonRotationAngleCore; }
			set { SetValue(ref buttonRotationAngleCore, value, "ButtonRotationAngle"); }
		}
		[Browsable(false)]
		public bool IsHorizontal {
			get { return Orientation == Orientation.Horizontal; }
		}
		[Browsable(false)]
		public bool IsSelected {
			get { return Owner.IsSelected; }
		}
		bool rightToLeftCore;
		[DefaultValue(false), Category("Layout")]
		public bool RightToLeft {
			get { return rightToLeftCore; }
			set { SetValue(ref rightToLeftCore, value, "RightToLeft"); }
		}
		Orientation orientationCore;
		[DefaultValue(Orientation.Horizontal), Category("Layout")]
		public Orientation Orientation {
			get { return orientationCore; }
			set { SetValue(ref orientationCore, value, "Orientation"); }
		}
		ContentAlignment contentAlignmentCore = ContentAlignment.MiddleRight;
		[DefaultValue(ContentAlignment.MiddleRight), Category("Layout")]
		public ContentAlignment ContentAlignment {
			get { return contentAlignmentCore; }
			set { SetValue(ref contentAlignmentCore, value, "ContentAlignment"); }
		}
		int intervalCore;
		[DefaultValue(0), Category("Layout")]
		public int ButtonInterval {
			get { return intervalCore; }
			set { SetValue(ref intervalCore, value, "Interval"); }
		}
		[DefaultValue(null)]
		public object Images {
			get { return Owner != null ? Owner.Images : null; }
		}
		[DefaultValue(false)]
		public bool WrapButtons {
			get { return wrapButtonsCore; }
			set { SetValue(ref wrapButtonsCore, value, "WrapButtons"); }
		}
		#endregion Properties
		#region Events
		BaseButtonEventHandler buttonClick;
		event BaseButtonEventHandler IButtonsPanel.ButtonClick {
			add { buttonClick += value; }
			remove { buttonClick -= value; }
		}
		BaseButtonEventHandler buttonUnchecked;
		event BaseButtonEventHandler IButtonsPanel.ButtonUnchecked {
			add { buttonUnchecked += value; }
			remove { buttonUnchecked -= value; }
		}
		BaseButtonEventHandler buttonChecked;
		event BaseButtonEventHandler IButtonsPanel.ButtonChecked {
			add { buttonChecked += value; }
			remove { buttonChecked -= value; }
		}
		protected virtual void RaiseButtonClick(IBaseButton button) {
			if(buttonClick != null)
				buttonClick(this, new BaseButtonEventArgs(button));
		}
		protected virtual void RaiseButtonChecked(IBaseButton button) {
			if(buttonChecked != null)
				buttonChecked(this, new BaseButtonEventArgs(button));
		}
		protected virtual void RaiseButtonUnchecked(IBaseButton button) {
			if(buttonUnchecked != null)
				buttonUnchecked(this, new BaseButtonEventArgs(button));
		}
		#endregion Events
		protected virtual BaseButtonCollection CreateButtons() {
			return new BaseButtonCollection(this);
		}
		protected virtual IButtonsPanelHandler CreateHandler() {
			return new ButtonsPanelHandler(this);
		}
		protected virtual IButtonsPanelViewInfo CreateViewInfo() {
			return new BaseButtonsPanelViewInfo(this);
		}
		protected virtual void OnUpdateObjectCore(EventArgs e) {
			ViewInfo.SetDirty();
			Owner.Invalidate();
		}
		public void OnButtonsCollectionChanged(object sender, CollectionChangeEventArgs e) {
			OnObjectChanged("Buttons");
		}
		void IButtonsPanel.LayoutChanged() {
			OnObjectChanged();
		}
		void IButtonsPanel.CheckedChanged(object sender, EventArgs e) {
			IBaseButton button = sender as IBaseButton;
			if(button != null && Buttons.Contains(button)) {
				if(button.Properties.Style == ButtonStyle.CheckButton) {
					CheckButtonGroupIndex(button);
					if(button.Properties.Checked)
						RaiseButtonChecked(button);
					else RaiseButtonUnchecked(button);
				}
			}
		}
		void IButtonsPanel.PerformClick(IBaseButton button) {
			if(Buttons.Contains(button)) {
				IButtonsPanelOwnerEx ownerEx = Owner as IButtonsPanelOwnerEx;
				if(ownerEx != null && !ownerEx.CanPerformClick(button)) return;
				if(button.Properties.Style == ButtonStyle.CheckButton) {
					CheckButtonGroupIndex(button);
					button.Properties.BeginUpdate();
					button.Properties.Checked = CalcNewCheckedState(button);
					button.Properties.CancelUpdate();
				}
				else RaiseButtonClick(button);
			}
		}
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public bool CalcNewCheckedState(IBaseButton button) {
			if(button.Properties.GroupIndex != -1 && button.Properties.Checked && !IsSinglebuttonInGroup(button)) {
				return true;
			}
			return !button.Properties.Checked;
		}
		bool IsSinglebuttonInGroup(IBaseButton clickedButton) {
			if(clickedButton.Properties.GroupIndex != -1) {
				foreach(IBaseButton button in Buttons) {
					if(button.Properties.GroupIndex == clickedButton.Properties.GroupIndex && button != clickedButton)
						return false;
				}
			}
			return true;
		}
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public virtual void CheckButtonGroupIndex(IBaseButton clickedButton) {
			if(clickedButton.Properties.GroupIndex == -1) return;
			BeginUpdate();
			foreach(IBaseButton button in Buttons) {
				button.Properties.LockCheckEvent();
				if(button.Properties.GroupIndex == clickedButton.Properties.GroupIndex && button != clickedButton)
					button.Properties.Checked = false;
				button.Properties.UnlockCheckEvent();
			}
			CancelUpdate();
			Owner.Invalidate();
		}
		#region IToolTipControlClient Members
		public ToolTipControlInfo GetObjectInfo(Point point) {
			ToolTipControlInfo result = null;
			BaseButtonInfo buttoInfo = ViewInfo.CalcHitInfo(point);
			if(buttoInfo != null && (!string.IsNullOrEmpty(buttoInfo.Button.Properties.ToolTip) || buttoInfo.Button.Properties.SuperTip != null)) {
				result = new ToolTipControlInfo(buttoInfo.Button.Properties, buttoInfo.Button.Properties.ToolTip);
				result.SuperTip = buttoInfo.Button.Properties.SuperTip != null ? buttoInfo.Button.Properties.SuperTip : null;
			}
			return result;
		}
		[DefaultValue(true)]
		public bool ShowToolTips {
			get { return showToolTipCore; }
			set { showToolTipCore = value; }
		}
		#endregion
		protected void OnObjectChanged() {
			OnObjectChanged(EventArgs.Empty);
		}
		protected void OnObjectChanged(string propertyName) {
			OnObjectChanged(new PropertyChangedEventArgs(propertyName));
		}
		protected void OnObjectChanged(EventArgs e) {
			if(IsUpdateLocked) return;
			OnUpdateObjectCore(e);
			RaiseChanged(e);
		}
		protected void RaiseChanged(EventArgs e) {
			if(Changed != null) Changed(this, e);
		}
		public event EventHandler Changed;
		protected void SetValue<T>(ref T field, T value, string propertyName) {
			if(object.Equals(field, value)) return;
			field = value;
			OnObjectChanged(propertyName);
		}
		public void UpdateStyle() {
			BeginUpdate();
			foreach(IBaseButton button in Buttons) {
				if(button is DefaultButton)
					button.Properties.Glyphs = null;
			}
			EndUpdate();
		}
		int lockUpdateCore = 0;
		[Browsable(false)]
		public bool IsUpdateLocked {
			get { return lockUpdateCore != 0; }
		}
		public void BeginUpdate() {
			lockUpdateCore++;
		}
		public void EndUpdate() {
			lockUpdateCore--;
			RaiseChanged(EventArgs.Empty);
		}
		public void CancelUpdate() {
			lockUpdateCore--;
		}
	}
	public abstract class BaseButtonHandler {
		public virtual void OnMouseDown(MouseEventArgs e) {
			if(e.Button == MouseButtons.Left)
				PressedInfo = CalcHitInfo(e.Location);
		}
		public virtual void OnMouseUp(MouseEventArgs e) {
			if(e.Button == MouseButtons.Left) {
				BaseButtonInfo hitInfo = CalcHitInfo(e.Location);
				if(hitInfo != null && AreEqual(hitInfo.Button, PressedButton))
					PerformClick(hitInfo.Button);
			}
			Reset();
		}
		protected static bool AreEqual(IBaseButton hitButton, IBaseButton button) {
			return hitButton == button || AreEqual(hitButton as IDefaultButton, button as IDefaultButton);
		}
		protected static bool AreEqual(IDefaultButton hitButton, IDefaultButton button) {
			if(button == null || hitButton == null) return false;
			if(button != hitButton)
				return button.GetType() == hitButton.GetType();
			return hitButton == button;
		}
		public virtual void OnMouseMove(MouseEventArgs e) {
			HotInfo = CalcHitInfo(e.Location);
		}
		public virtual void OnMouseLeave() {
			HotInfo = null;
		}
		public virtual void Reset() {
			HotInfo = null;
			PressedInfo = null;
		}
		public virtual IBaseButton PressedButton {
			get { return PressedInfo != null ? PressedInfo.Button : null; }
		}
		public IBaseButton HotButton {
			get { return HotInfo != null ? HotInfo.Button : null; }
		}
		BaseButtonInfo pressedInfoCore;
		protected BaseButtonInfo PressedInfo {
			get { return pressedInfoCore; }
			private set { SetPressedState(ref pressedInfoCore, value); }
		}
		BaseButtonInfo hotInfoCore;
		protected BaseButtonInfo HotInfo {
			get { return hotInfoCore; }
			private set { SetHotState(ref hotInfoCore, value); }
		}
		protected void ClearState(IButtonsPanel panel, ObjectState state) {
			if(state == ObjectState.Hot)
				HotInfo = null;
			if(state == ObjectState.Pressed)
				PressedInfo = null;
		}
		protected virtual void SetHotState(ref BaseButtonInfo info, BaseButtonInfo value) {
			SetState(ref info, value);
		}
		protected virtual void SetPressedState(ref BaseButtonInfo info, BaseButtonInfo value) {
			SetState(ref info, value);
		}
		protected void SetState(ref BaseButtonInfo info, BaseButtonInfo value) {
			if(info == value) return;
			info = value;
			Invalidate();
		}
		protected abstract void Invalidate();
		protected abstract BaseButtonInfo CalcHitInfo(Point point);
		protected abstract void PerformClick(IBaseButton button);
	}
	public class ButtonsPanelHandler : BaseButtonHandler, IButtonsPanelHandler {
		public ButtonsPanelHandler(IButtonsPanel panel) {
			Panel = panel;
		}
		public IButtonsPanel Panel { get; private set; }
		protected override void Invalidate() {
			Panel.Owner.Invalidate();
		}
		protected override BaseButtonInfo CalcHitInfo(Point point) {
			return Panel.ViewInfo.CalcHitInfo(point);
		}
		protected override void PerformClick(IBaseButton button) {
			Panel.PerformClick(button);
		}
	}
	public class BaseButtonsPanelWithState : BaseButtonsPanel {
		public BaseButtonsPanelWithState(IButtonsPanelOwner owner)
			: base(owner) {
		}
		protected override IButtonsPanelHandler CreateHandler() {
			return new ButtonsPanelHandlerWithState(this);
		}
	}
	public class ButtonsPanelHandlerWithState : ButtonsPanelHandler, IButtonsPanelHandlerWithState {
		public ButtonsPanelHandlerWithState(IButtonsPanel panel)
			: base(panel) {
		}
		readonly static object pressedButton = new object();
		WeakReference pressedButtonState;
		public void SaveState(System.Collections.Hashtable storage) {
			storage[pressedButton] = new WeakReference(base.PressedButton);
			this.pressedButtonState = null;
		}
		public void ApplyState(System.Collections.Hashtable storage) {
			this.pressedButtonState = storage[pressedButton] as WeakReference;
		}
		public override IBaseButton PressedButton {
			get { return GetPressedButtonState() ?? base.PressedButton; }
		}
		protected override void SetPressedState(ref BaseButtonInfo info, BaseButtonInfo value) {
			this.pressedButtonState = null;
			base.SetPressedState(ref info, value);
		}
		public override void Reset() {
			this.pressedButtonState = null;
			base.Reset();
		}
		IBaseButton GetPressedButtonState() {
			return (pressedButtonState != null) ? pressedButtonState.Target as IBaseButton : null;
		}
	}
}
