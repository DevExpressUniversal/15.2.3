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

using System.Drawing;
using DevExpress.XtraEditors.ButtonPanel;
using DevExpress.Utils;
namespace DevExpress.XtraBars.Docking2010.Views.WindowsUI {
	interface IPopupActionInfo : IBaseFlyoutPanelInfo {
		PopupActionButton ActualButton { get; }
		bool AttachActionButton(PopupActionButton button);
		void Update();
		BeakPanelBeakLocation BeakLocation { get; set; }
	}
	class PopupActionInfo : BaseFlyoutPanelInfo, IPopupActionInfo {
		const int paddingCore = 12;
		PopupItemActionButtonPanel buttonPanel;
		PopupActionButton actualButtonCore;
		public PopupActionInfo(WindowsUIView owner)
			: base(owner) {
			buttonPanel = CreatePopupButtonPanel();
			buttonPanel.Padding = new System.Windows.Forms.Padding(paddingCore);
		}
		protected override void InitializeFlyoutPanel() {
			FlyoutPanel.OptionsBeakPanel.CloseOnOuterClick = false;
			base.InitializeFlyoutPanel();
		}
		protected override FlyoutPanel CreateFlyoutPanel() { return new PopupActionPanel(); }
		protected virtual PopupItemActionButtonPanel CreatePopupButtonPanel() { return new PopupItemActionButtonPanel(); }
		protected override System.Windows.Forms.Control OwnerControl { get { return Owner.Manager.GetOwnerControl(); } }
		void OnActionButtonClick(object sender, ButtonEventArgs e) {
			ActionButton button = e.Button as ActionButton;
			if(button == null) return;
			var tag = button.Tag as ContentContainerAction.Tag;
			ContentContainerAction.Execute(tag.View, tag.Args);
			PerformBehavior(ContentContainerAction.GetActionBehavior(tag.Args.Action).Behavior, tag.Args.Parameter);
		}
		void PerformBehavior(ActionBehavior behavior, IContentContainer parameter) {
			if(behavior == ActionBehavior.HideFlyoutPanelOnClick)
				((IBaseFlyoutPanelInfo)this).ProcessHide();
			if(behavior == ActionBehavior.HideBarOnClick)
				Owner.HideNavigationAdorner();
			if(behavior == ActionBehavior.UpdateBarOnClick)
				Owner.UpdateNavigationBars();
			if(behavior == ActionBehavior.UpdateFlyoutPanelOnClick)
				UpdatePopupActionButtonPanel();
		}
		protected override bool ProcessShowCore(Point location, bool immediate) {
			if(ActualButton == null) return false;
			FlyoutPanel.ShowBeakForm(location, immediate);
			return true;
		}
		protected override void ProcessHideCore() { FlyoutPanel.HideBeakForm(); }
		protected override void ProcessCancelCore() { FlyoutPanel.HideBeakForm(true); }
		public virtual BeakPanelBeakLocation BeakLocation {
			get { return FlyoutPanel.OptionsBeakPanel.BeakLocation; }
			set { FlyoutPanel.OptionsBeakPanel.BeakLocation = value; }
		}
		public PopupActionButton ActualButton { get { return actualButtonCore; } }
		bool IPopupActionInfo.AttachActionButton(PopupActionButton button) {
			if(button == null) return false;
			var tag = button.Tag as ContentContainerAction.Tag;
			if(tag == null) return false;
			actualButtonCore = button;
			if(button.IsPopupMenu) {
				IContentContainerPopupMenuAction actualAcion = (IContentContainerPopupMenuAction)tag.Args.Action;
				buttonPanel.Orientation = actualAcion.Orientation;
				buttonPanel.Buttons.AddRange(GetActionButtonCollection(tag.Args.Parameter, actualAcion.Actions));
				buttonPanel.ButtonClick += OnActionButtonClick;
				return AttachContentControlCore(buttonPanel);
			}
			return AttachContentControlCore(((IContentContainerPopupControlAction)tag.Args.Action).Control);
		}
		protected IBaseButton[] GetActionButtonCollection(IContentContainer parameter, System.Collections.Generic.IEnumerable<IUIAction<IContentContainer>> actualActions) {
			if(actualActions == null) return new IBaseButton[] { };
			System.Collections.Generic.List<IBaseButton> buttons = new System.Collections.Generic.List<IBaseButton>();
			foreach(IContentContainerAction action in actualActions)
				buttons.Add(CreateActionButton(parameter, action));
			return buttons.ToArray();
		}
		IButton CreateActionButton(IContentContainer parameter, IContentContainerAction action) {
			ContentContainerAction.Args args = new ContentContainerAction.Args(action, parameter);
			ActionButton button = new PopupItemActionButton(action);
			button.Tag = new ContentContainerAction.Tag(Owner, args);
			button.Visible = ContentContainerAction.CanExecute(args);
			AppearanceHelper.Combine(button.Appearance, new AppearanceObject[] { ActualButton.Appearance });
			button.IsLeft = ContentContainerAction.GetActionLayout(action).Edge == ActionEdge.Left;
			button.Image = action.Image ?? Resources.ContentContainterActionResourceLoader.GetImage("Default");
			return button;
		}
		void ResetPopupActionButtonPanel() {
			if(buttonPanel == null) return;
			buttonPanel.BeginUpdate();
			IBaseButton[] buttons = buttonPanel.Buttons.CleanUp<IBaseButton>() as IBaseButton[];
			for(int i = 0; i < buttons.Length; i++)
				buttons[i].Dispose();
			buttonPanel.EndUpdate();
		}
		void UpdatePopupActionButtonPanel() {
			if(buttonPanel == null) return;
			buttonPanel.BeginUpdate();
			IButton[] buttons = buttonPanel.Buttons.ToArray<IButton>() as IButton[];
			for(int i = 0; i < buttons.Length; i++) {
				ActionButton button = buttons[i] as ActionButton;
				var tag = button.Tag as ContentContainerAction.Tag;
				button.Visible = ContentContainerAction.CanExecute(tag.Args);
				AppearanceHelper.Combine(button.Appearance, new AppearanceObject[] { ActualButton.Appearance });
			}
			buttonPanel.EndUpdate();
		}
		protected override void DrawCore(DevExpress.Utils.Drawing.GraphicsCache cache) {
			base.DrawCore(cache);
			FlyoutPanel.Invalidate(true);
			FlyoutPanel.Update();
		}
		protected override void SetDirty() {
			base.SetDirty();
			buttonPanel.SetDirty();
		}
		void IPopupActionInfo.Update() {
			if(actualButtonCore == null) return;
			var tag = actualButtonCore.Tag as ContentContainerAction.Tag;
			if(tag == null) return;
			UpdatePopupActionButtonPanel();
		}
		protected override void DetachContentControl() {
			base.DetachContentControl();
			buttonPanel.ButtonClick -= OnActionButtonClick;
			ResetPopupActionButtonPanel();
			actualButtonCore = null;
		}
		protected override Size CalcMinSize(Graphics g) { return CalcMinSizeCore(g); }
		protected override void CalcCore(Graphics g, Rectangle bounds) {
			using(var update = BatchUpdate.Enter(FlyoutPanel)) {
				FlyoutPanel.OptionsBeakPanel.ScreenFormLocation = bounds.Location;
				base.CalcCore(g, bounds);
			}
		}
		new PopupActionPanel FlyoutPanel { get { return base.FlyoutPanel as PopupActionPanel; } }
		Size CalcMinSizeCore(Graphics g) {
			if(buttonPanel != null && ContentControl == buttonPanel) {
				Size size = buttonPanel.CalcMinSize(g);
				return new Size(size.Width + 2 * paddingCore, size.Height + 2 * paddingCore);
			}
			return ContentControl.Size;
		}
		protected override System.Windows.Forms.Control RaiseQueryContentControl() {
			if(ActualButton == null) return null;
			var tag = ActualButton.Tag as ContentContainerAction.Tag;
			if(tag == null) return null;
			QueryFlyoutActionsControlArgs e = new QueryFlyoutActionsControlArgs(tag.Args.Action);
			Owner.RaiseQueryFlyoutActionsControl(e);
			return e.Control;
		}
		protected override void OnDispose() {
			base.OnDispose();
			Ref.Dispose(ref buttonPanel);
		}
		class PopupActionPanel : FlyoutPanel, DevExpress.Utils.Base.ISupportBatchUpdate {
			protected override void OnSizeChanged(System.EventArgs e) {
				if(IsUpdateLocked) return;
				base.OnSizeChanged(e);
			}
			public override Color BackColor {
				get { return Appearance.BackColor; }
				set { }
			}
			protected override BeakPanelOptions CreateBeakPanelOptions() { return new BeakPopupActionOptions(this); }
			protected override FlyoutPanelToolForm CreateBeakFormCore(System.Windows.Forms.Control owner, FlyoutPanel content, Point loc, Point offset) {
				if(loc.IsEmpty) loc = OptionsBeakPanel.ScreenFormLocation;
				BeakPopupActionPanelOptions beakOptions = new BeakPopupActionPanelOptions(loc);
				beakOptions.AnchorType = DevExpress.Utils.Win.PopupToolWindowAnchor.Manual;
				beakOptions.AnimationType = DevExpress.Utils.Win.PopupToolWindowAnimation.Fade;
				beakOptions.CloseOnOuterClick = OptionsBeakPanel.CloseOnOuterClick;
				beakOptions.Offset = offset;
				return new FlyoutPanelBeakForm(owner, content, beakOptions);
			}
			public new BeakPopupActionOptions OptionsBeakPanel { get { return base.OptionsBeakPanel as BeakPopupActionOptions; } }
			#region IBatchUpdateable Members
			int isUpdateLocked = 0;
			public void CancelUpdate() {
				isUpdateLocked = 0;
				EndUpdate();
			}
			public void EndUpdate() {
				if(--isUpdateLocked == 0 && FlyoutPanelState.IsActive) {
					((BeakPopupActionPanelOptions)FlyoutPanelState.Form.Options).ScreenFormLocation = OptionsBeakPanel.ScreenFormLocation;
					OnSizeChanged(System.EventArgs.Empty);
					FlyoutPanelState.Form.UpdateLocation();
				}
			}
			public bool IsUpdateLocked { get { return isUpdateLocked > 0; } }
			public void BeginUpdate() { isUpdateLocked++; }
			#endregion
		}
		class BeakPopupActionPanelOptions : BeakFlyoutPanelOptions {
			public BeakPopupActionPanelOptions(Point location) : base(location) { }
			public new Point Offset {
				get { return base.Offset; }
				set { base.Offset = value; }
			}
			public new Point ScreenFormLocation {
				get { return base.ScreenFormLocation; }
				set { base.ScreenFormLocation = value; }
			}
		}
		class BeakPopupActionOptions : BeakPanelOptions {
			Point screenFormLocation;
			public BeakPopupActionOptions(FlyoutPanel owner) : base(owner) { }
			public override Color BackColor {
				get { return Owner.Appearance.BackColor; }
				set { }
			}
			public override Color BorderColor {
				get { return Owner.Appearance.ForeColor; }
				set { }
			}
			public Point ScreenFormLocation {
				get { return screenFormLocation; }
				set {
					if(ScreenFormLocation == value)
						return;
					Point prev = ScreenFormLocation;
					screenFormLocation = value;
					OnChanged("ScreenFormLocation", prev, ScreenFormLocation);
				}
			}
		}
	}
	#region PopupItemActionButtonPanel
	class PopupItemActionButtonPanel : WindowsUIButtonPanel {
		public Size CalcMinSize(Graphics g) { return ButtonsPanel.ViewInfo.CalcMinSize(g); }
		public void SetDirty() { ButtonsPanel.ViewInfo.SetDirty(); }
		public void BeginUpdate() { ButtonsPanel.BeginUpdate(); }
		public void EndUpdate() { ButtonsPanel.EndUpdate(); }
		protected override void UpdateButtonPanel() { }
		protected override WindowsUIButtonsPanel CreateButtonsPanel() {
			return new PopupItemActionButtonsPanel(this);
		}
	}
	class PopupItemActionButtonsPanel : WindowsUIButtonsPanel {
		public PopupItemActionButtonsPanel(IButtonsPanelOwner owner) : base(owner) { }
		protected override IButtonsPanelViewInfo CreateViewInfo() {
			return new PopupItemActionButtonsPanelViewInfo(this);
		}
	}
	class PopupItemActionButtonsPanelViewInfo : WindowsUIButtonsPanelViewInfo {
		public PopupItemActionButtonsPanelViewInfo(IButtonsPanel panel) : base(panel) { }
		protected override BaseButtonInfo CreateButtonInfo(IBaseButton button) {
			PopupItemActionButton popupButton = button as PopupItemActionButton;
			if(popupButton == null) return base.CreateButtonInfo(button);
			return new PopupItemActionButtonInfo(popupButton);
		}
	}
	class PopupItemActionButtonInfo : WindowsUIButtonInfo {
		public PopupItemActionButtonInfo(PopupItemActionButton button) : base(button) { }
		public new PopupItemActionButton Button { get { return base.Button as PopupItemActionButton; } }
		protected override int GetImageToTextInterval(BaseButtonPainter painter) {
			if(Button == null || Button.ImageToTextInterval == null) return base.GetImageToTextInterval(painter);
			return (int)Button.ImageToTextInterval;
		}
	}
	class PopupItemActionButton : ActionButton {
		const int imageToTextInterval = 8;
		public PopupItemActionButton(IContentContainerAction action) : base(action) { }
		protected override DevExpress.XtraEditors.ButtonPanel.ImageLocation ImageLocationCore {
			get {
				if(this.GetOwner().Orientation == System.Windows.Forms.Orientation.Vertical)
					return XtraEditors.ButtonPanel.ImageLocation.BeforeText;
				return DevExpress.XtraEditors.ButtonPanel.ImageLocation.AboveText;
			}
		}
		public virtual int? ImageToTextInterval {
			get {
				if(this.GetOwner().Orientation == System.Windows.Forms.Orientation.Vertical)
					return imageToTextInterval;
				return null;
			}
		}
	}
	#endregion
	class PopupActionButton : ActionButton {
		public PopupActionButton(IContentContainerPopupControlAction action) : base(action) { }
		public PopupActionButton(IContentContainerPopupMenuAction action) : base(action) { IsPopupMenu = true; }
		public bool IsPopupMenu { get; private set; }
	}
}
