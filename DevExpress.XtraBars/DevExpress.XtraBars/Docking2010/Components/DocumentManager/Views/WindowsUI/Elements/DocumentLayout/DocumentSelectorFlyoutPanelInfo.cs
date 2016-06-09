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
using System.Windows.Forms;
using DevExpress.Utils;
using DevExpress.Utils.Drawing;
using DevExpress.Utils.Win;
using DevExpress.XtraEditors.ButtonPanel;
namespace DevExpress.XtraBars.Docking2010.Views.WindowsUI {
	public interface IDocumentSelectorFlyoutPanelInfo : IBaseFlyoutPanelInfo {
		void AttachDocumentButtons(IBaseButton[] buttons);
		void Invalidate();
		int ButtonsInterval { get; set; }
	}
	class DocumentSelectorFlyoutPanelInfo : BaseFlyoutPanelInfo, IDocumentSelectorFlyoutPanelInfo {
		DocumentSelectorContentControl contentControlCore;
		ButtonsPanel buttonsPanelCore;
		IButtonsPanel ownerButtonsPanel;
		public DocumentSelectorFlyoutPanelInfo(WindowsUIView owner, IButtonsPanel ownerButtonsPanel)
			: base(owner) {
			this.ownerButtonsPanel = ownerButtonsPanel;
			this.buttonsPanelCore = CreateButtonsPanel();
			this.contentControlCore = CreateContentControl();
			InitializeButtonsPanel();
			UpdateStyleCore();
			AttachContentControlCore(contentControlCore);
		}
		protected ButtonsPanel ButtonsPanel {
			get { return buttonsPanelCore; }
		}
		void InitializeButtonsPanel() {
			ButtonsPanel.Orientation = Orientation.Vertical;
			((DocumentSelectorButtonsPanel)ButtonsPanel).SetControlToInvalidate(contentControlCore);
		}
		protected override void InitializeFlyoutPanel() {
			base.InitializeFlyoutPanel();
			FlyoutPanel.Options.AnchorType = DevExpress.Utils.Win.PopupToolWindowAnchor.Manual;
			FlyoutPanel.Options.CloseOnOuterClick = false;
		}
		protected override void SetDirty() {
			ButtonsPanel.ViewInfo.SetDirty();
		}
		protected virtual ButtonsPanel CreateButtonsPanel() {
			return new DocumentSelectorButtonsPanel(ownerButtonsPanel.Owner);
		}
		protected override FlyoutPanel CreateFlyoutPanel() {
			return new DocumentSelectorFlyoutPanel();
		}
		DocumentSelectorContentControl CreateContentControl() {
			return new DocumentSelectorContentControl(ButtonsPanel);
		}
		protected override void UpdateStyleCore() {
			base.UpdateStyleCore();
			AppearanceFlyoutPanel.BackColor = Owner.GetBackColor();
		}
		protected override Control OwnerControl {
			get { return Owner.Manager.GetOwnerControl(); }
		}
		protected override bool ProcessShowCore(Point location, bool immediate) {
			FlyoutPanel.ShowPopup(immediate);
			return true;
		}
		protected override void CalcCore(Graphics g, Rectangle bounds) {
			FlyoutPanel.Options.Location = bounds.Location;
			FlyoutPanel.Size = DocumentSelectorToolFormPainter.GetObjectClientRectangle(bounds).Size;
		}
		protected override void ProcessHideCore() {
			FlyoutPanel.HidePopup(false);
		}
		protected override void ProcessCancelCore() {
			FlyoutPanel.HidePopup(true);
		}
		protected override Size CalcMinSize(Graphics g) {
			Size size = ButtonsPanel.ViewInfo.CalcMinSize(g);
			if(FlyoutPanel == null) return size;
			Size flyoutPanelSize = new Size(size.Width + FlyoutPanel.Padding.Horizontal, size.Height + FlyoutPanel.Padding.Vertical);
			return DocumentSelectorToolFormPainter.CalcBoundsByClientRectangle(new Rectangle(Point.Empty, flyoutPanelSize)).Size;
		}
		protected override void OnDispose() {
			Ref.Dispose(ref contentControlCore);
			Ref.Dispose(ref buttonsPanelCore);
			base.OnDispose();
		}
		#region IPopupDocumentSelectorInfo Members
		public void Invalidate() {
			if(!IsDisposing)
				contentControlCore.Invalidate();
		}
		public int ButtonsInterval {
			get { return ButtonsPanel.ButtonInterval; }
			set {
				if(ButtonsInterval == value) return;
				ButtonsPanel.ButtonInterval = value;
				FlyoutPanel.Padding = new Padding(value);
			}
		}
		void IDocumentSelectorFlyoutPanelInfo.AttachDocumentButtons(IBaseButton[] buttons) {
			ResetButtonsPanel();
			ButtonsPanel.BeginUpdate();
			ButtonsPanel.Buttons.AddRange(buttons);
			ButtonsPanel.EndUpdate();			
		}
		protected override void DetachContentControl() {
			ResetButtonsPanel();
			base.DetachContentControl();
		}
		void ResetButtonsPanel() {
			if(ButtonsPanel == null) return;
			ButtonsPanel.BeginUpdate();
			IBaseButton[] buttons = ButtonsPanel.Buttons.CleanUp();
			for(int i = 0; i < buttons.Length; i++)
				buttons[i].SetOwner(ownerButtonsPanel);
			ButtonsPanel.EndUpdate();
		}
		#endregion
		class DocumentSelectorButtonsPanel : ButtonsPanel {
			DocumentSelectorContentControl contentControl;
			public DocumentSelectorButtonsPanel(IButtonsPanelOwner owner)
				: base(owner) {
			}
			protected override void OnDispose() {
				this.contentControl = null;
				base.OnDispose();
			}
			protected override IButtonsPanelHandler CreateHandler() {
				return new DocumentSelectorButtonsPanelHandler(this);
			}
			public void SetControlToInvalidate(DocumentSelectorContentControl contentControlCore) {
				this.contentControl = contentControlCore;
			}
			public void Invalidate() {
				if(contentControl != null) contentControl.Invalidate();
			}
			class DocumentSelectorButtonsPanelHandler : ButtonsPanelHandler {
				public DocumentSelectorButtonsPanelHandler(IButtonsPanel panel)
					: base(panel) {
				}
				protected override void Invalidate() {
					((DocumentSelectorButtonsPanel)Panel).Invalidate();
				}
			}
		}
		class DocumentSelectorContentControl : Control {
			ButtonsPanel buttonsPanel;
			public DocumentSelectorContentControl(ButtonsPanel panel) {
				buttonsPanel = panel;
				SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint | ControlStyles.ResizeRedraw | ControlStyles.UserPaint, true);
			}
			protected override void Dispose(bool disposing) {
				base.Dispose(disposing);
				buttonsPanel = null;
			}
			protected override void OnResize(EventArgs e) {
				buttonsPanel.ViewInfo.SetDirty();
				base.OnResize(e);
			}
			protected override void OnPaint(PaintEventArgs e) {
				using(GraphicsCache cache = new GraphicsCache(e)) {
					if(!buttonsPanel.ViewInfo.IsReady)
						buttonsPanel.ViewInfo.Calc(e.Graphics, ClientRectangle);
					ObjectPainter.DrawObject(cache, buttonsPanel.Owner.GetPainter(), buttonsPanel.ViewInfo as ObjectInfoArgs);
				}
			}
			protected override void OnMouseMove(MouseEventArgs e) {
				buttonsPanel.Handler.OnMouseMove(e);
			}
			protected override void OnMouseUp(MouseEventArgs e) {
				buttonsPanel.Handler.OnMouseUp(e);
			}
			protected override void OnMouseDown(MouseEventArgs e) {
				buttonsPanel.Handler.OnMouseDown(e);
			}
			protected override void OnMouseLeave(EventArgs e) {
				buttonsPanel.Handler.OnMouseLeave();
			}
		}
		class DocumentSelectorFlyoutPanel : FlyoutPanel {
			protected override FlyoutPanelToolForm CreateToolFormCore(Control owner, FlyoutPanel content, FlyoutPanelOptions options) {
				DocumentSelectorToolForm toolForm = new DocumentSelectorToolForm(owner, content, options);
				toolForm.TopLevel = false;
				toolForm.Parent = OwnerControl.Parent;
				return toolForm;
			}
			protected override void UpdateContentPaddings() { }
		}
		class DocumentSelectorToolForm : FlyoutPanelToolForm {
			public DocumentSelectorToolForm(Control owner, FlyoutPanel flyoutPanel, FlyoutPanelOptions options)
				: base(owner, flyoutPanel, options) { }
			public new DocumentSelectorFlyoutPanel FlyoutPanel {
				get { return base.FlyoutPanel as DocumentSelectorFlyoutPanel; }
			}
			public new void DoShow() {
				Show();
			}
			protected override void OnContentSizeChanged() {
				Size = CalcFormSize();
			}
			protected override BasePopupToolWindowHandler CreateHandler() {
				return new DocumentSelectorToolFormHandler(this);
			}
			protected override void OnSaveContentState() {
				base.OnSaveContentState();
				if(DesignMode) return;
				EnsureContentFillNone();
				Rectangle rect = CalcFlyoutPanel();
				FlyoutPanel.SetBounds(rect.X, rect.Y, rect.Width, rect.Height);
			}
			protected virtual Rectangle CalcFlyoutPanel() {
				return DocumentSelectorToolFormPainter.GetObjectClientRectangle(new Rectangle(Point.Empty, FormSize));
			}
			protected virtual void EnsureContentFillNone() {
				if(FlyoutPanel.Dock != DockStyle.None)
					FlyoutPanel.Dock = DockStyle.None;
			}
			protected override Size FormSize {
				get { return DocumentSelectorToolFormPainter.CalcBoundsByClientRectangle(new Rectangle(Point.Empty, base.FormSize)).Size; }
			}
			protected override ToolWindowPainterBase CreatePainter() {
				return new DocumentSelectorToolFormPainter(this, this);
			}
		}
		class DocumentSelectorToolFormPainter : ToolWindowPainter {
			static readonly int BorderThickness = 1;
			public DocumentSelectorToolFormPainter(BasePopupToolWindow toolWindow, DevExpress.LookAndFeel.ISupportLookAndFeel lookAndFeel)
				: base(toolWindow, lookAndFeel) { }
			public new DocumentSelectorToolForm ToolWindow { 
				get { return base.ToolWindow as DocumentSelectorToolForm; } 
			}
			internal static Rectangle CalcBoundsByClientRectangle(Rectangle client) {
				return Rectangle.Inflate(client, BorderThickness, BorderThickness);
			}
			internal static Rectangle GetObjectClientRectangle(Rectangle bounds) {
				return Rectangle.Inflate(bounds, -BorderThickness, -BorderThickness);
			}
		}
		class DocumentSelectorToolFormHandler : FlyoutPanelToolFormHandler {
			public DocumentSelectorToolFormHandler(DocumentSelectorToolForm toolForm)
				: base(toolForm) {
			}
			public new DocumentSelectorToolForm ToolWindow { 
				get { return base.ToolWindow as DocumentSelectorToolForm; } 
			}
			public override void OnImmediateShowToolWindow() {
				CheckToolWindowLocation();
				ToolWindow.DoShow();
			}
			protected override PopupToolWindowAnimationProviderBase CreateAnimationProvider() { 
				return new DocumentSelectorUpDownSlideAnimation(this); 
			}
		}
		class DocumentSelectorUpDownSlideAnimation : PopupToolWindowUpDownSlideAnimationProvider {
			public DocumentSelectorUpDownSlideAnimation(IPopupToolWindowAnimationSupports info)
				: base(info) {
			}
			public override Point CalcTargetFormLocation() {
				Rectangle ownerBounds = GetOwnerClientBounds();
				ownerBounds.Location = OwnerControl.Location;
				switch(Info.AnchorType) {
					case PopupToolWindowAnchor.TopRight:
						return new Point(ownerBounds.Right - ToolWindow.Width - Info.HorzIndent, ownerBounds.Top + Info.VertIndent);
					case PopupToolWindowAnchor.Top:
					case PopupToolWindowAnchor.Left:
						return new Point(ownerBounds.Left, ownerBounds.Top);
					case PopupToolWindowAnchor.TopLeft:
						return new Point(ownerBounds.Left + Info.HorzIndent, ownerBounds.Top + Info.VertIndent);
					case PopupToolWindowAnchor.Bottom:
						return new Point(ownerBounds.Left, ownerBounds.Bottom - ToolWindow.Height);
					case PopupToolWindowAnchor.Right:
						return new Point(ownerBounds.Right - ToolWindow.Width, ownerBounds.Top);
					case PopupToolWindowAnchor.Center:
						return new Point(ownerBounds.X + ownerBounds.Width / 2 - ToolWindow.Width / 2, ownerBounds.Y + ownerBounds.Height / 2 - ToolWindow.Height / 2);
					case PopupToolWindowAnchor.Manual:
						return new Point(ownerBounds.X + Info.FormLocation.X, ownerBounds.Y + Info.FormLocation.Y);
					default: throw new NotSupportedException("Invalid Anchor Type");
				}
			}
		}
	}
}
