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
using System.Linq;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using DevExpress.Skins.XtraForm;
using DevExpress.Utils;
using DevExpress.Utils.Drawing.Animation;
using DevExpress.Utils.Drawing.Helpers;
using DevExpress.XtraBars.Docking;
using DevExpress.XtraEditors.ButtonPanel;
using DevExpress.XtraEditors.Drawing;
namespace DevExpress.XtraBars.Docking2010.Views.Widget {
	[ToolboxItem(false), DesignTimeVisible(false)]
	public class WidgetContainer : DocumentContainer, IXtraObjectWithBounds, IMouseWheelSupport, IToolTipControlClient {
		public WidgetContainer(BaseDocument document)
			: base(document) {
			SuspendLayout();
			ResumeLayout(false);
		}
		protected override void OnDispose() {
			RemoveBoundsAnimation();
			base.OnDispose();
		}
		public WidgetsHost Host { get { return Parent as WidgetsHost; } }
		public IWidgetsHostHandler Handler { get { return Host != null ? Host.Handler : null; } }
		public new Document Document { get { return base.Document as Document; } }
		protected override void WndProc(ref Message m) {
			if(Handler == null) { DoBaseWndProc(ref m); return; }
			switch(m.Msg) {
				case 0x0232:
					Handler.ProcessExitSizeMove(this);
					DoBaseWndProc(ref m);
					break;
				case 0x0003:
					Handler.ProcessMove(Document as Document);
					DoBaseWndProc(ref m);
					break;
				case MSG.WM_NCACTIVATE:
					DoBaseWndProc(ref m);
					break;
				case 0x0231:
					Handler.ProcessEnterSizeMove(this);
					m.Result = new IntPtr(1);
					DoBaseWndProc(ref m);
					break;
				case MSG.WM_ERASEBKGND:
					m.Result = new IntPtr(1);
					DoBaseWndProc(ref m);
					break;
				case MSG.WM_WINDOWPOSCHANGING:
					ProcessWindowPosChanging(m.LParam);
					DoBaseWndProc(ref m);
					break;
				case MSG.WM_WINDOWPOSCHANGED:
					ProcessWindowPosChanged(m.LParam);
					DoBaseWndProc(ref m);
					break;
				default:
					base.WndProc(ref m);
					break;
			}
		}
		void ProcessWindowPosChanging(IntPtr intPtr) {
			if(Handler != null) {
				Handler.ProcessWindowPosChanging(this, intPtr);
			}
		}
		protected override FloatDocumentFormInfo CreateInfo() {
			return new WidgetFormInfo(this);
		}
		protected override void DoNCMouseLeave() {
			base.DoNCMouseLeave();
			ToolTipController.HideHint();
		}
		protected override void ProcessGetMinMaxInfo(ref Message m) {
			if(m.LParam == IntPtr.Zero || Document == null) return;
			var minMax = (NativeMethods.MINMAXINFO)BarNativeMethods.PtrToStructure(m.LParam, typeof(NativeMethods.MINMAXINFO));
			if(!Borderless) {
				minMax.ptMaxPosition = new NativeMethods.POINT(Bounds.X, Bounds.Y);
				minMax.ptMaxSize = new NativeMethods.POINT(Bounds.Width, Bounds.Height);
				BarNativeMethods.StructureToPtr(minMax, m.LParam, false);
			}
		}
		protected override void OnDefaultButtonClick(object sender, ButtonEventArgs e) {
			IButton button = e.Button;
			if(!(button is DefaultButton)) return;
			if(button is CloseButton)
				OnButtonClick(FormCaptionButtonAction.Close);
			if(button is MaximizeButton) {
				OnButtonClick(button.Properties.Checked ? FormCaptionButtonAction.Maximize : FormCaptionButtonAction.Restore);
			}
		}
		protected override void OnButtonChecked(object sender, ButtonEventArgs e) {
			if(Document == null) return;
			Document.RaiseCustomButtonChecked(e);
		}
		protected override void OnButtonUnchecked(object sender, ButtonEventArgs e) {
			if(Document == null) return;
			Document.RaiseCustomButtonUnchecked(e);
		}
		protected override void OnButtonClick(object sender, ButtonEventArgs e) {
			if(Document == null) return;
			Document.RaiseCustomButtonClick(e);
		}
		protected override bool ProcessSysCommand(ref Message m) {
			return Handler.ProcessSysCommand(Document as Document, this, ref m);
		}
		protected override void ProcessNCMouseMove(Point location) {
			IToolTipControlClient toolTipClient = GetToolTipClient(ref location);
			ToolTipController.ProccessNCMouseMove(toolTipClient, location);
		}
		protected virtual IToolTipControlClient GetToolTipClient(ref Point location){
			if(ButtonsPanel.Handler.HotButton != null)
				return ButtonsPanel;
			location.Offset(Bounds.Location);
			return this;
		}
		bool updateIsMaximizedCore;
		public void AddBoundsAnimation(Rectangle beginRect, Rectangle endRect, bool updateIsMaximized) {
			updateIsMaximizedCore = updateIsMaximized;
			BoundsAnimationInfo existAnimation = DevExpress.Utils.Drawing.Animation.XtraAnimator.Current.Get(Host, this) as BoundsAnimationInfo;
			BoundsAnimationInfo newAnimation = new BoundsAnimationInfo(Host, this, this, beginRect, endRect, 250, false, false);
			newAnimation.DeltaTick = Host.View.DocumentAnimationProperties.FrameInterval;
			newAnimation.FrameCount = Host.View.DocumentAnimationProperties.FrameCount;
			if(existAnimation != null) {
				newAnimation.BeginTick = existAnimation.CurrentTick;
				XtraAnimator.RemoveObject(Host, this);
				existAnimation.Dispose();
			}
			DevExpress.Utils.Drawing.Animation.XtraAnimator.Current.AddAnimation(newAnimation);
		}
		public void AddBoundsAnimation(Rectangle beginRect, Rectangle endRect) {
			AddBoundsAnimation(beginRect, endRect, true);
		}
		protected internal void RemoveBoundsAnimation() {
			var existingAnimation = XtraAnimator.Current.Get(Host, this);
			if(existingAnimation != null) {
				XtraAnimator.RemoveObject(Host, this);
				Ref.Dispose(ref existingAnimation);
			}
		}
		public void SetMaximized(bool value, bool allowUpdateBounds) {
			if(IsDisposing) return;
			if(Host.View.AllowDocumentStateChangeAnimation != DefaultBoolean.True || !allowUpdateBounds) {
				isMaximizedCore = value;
				Document.SetMaximized(value);
				if(IsMaximized)
					BeginInvoke(new MethodInvoker(Host.LayoutChanged));
				InvalidateNC();
				return;
			}
			if(value) {
				AddBoundsAnimation(Bounds, Host.ClientRectangle);
				Document.SetMaximized(value);
			}
			else {
				if(Document.Info != null) {
					AddBoundsAnimation(Host.ClientRectangle, Document.Info.Bounds);
				}
				else {
					isMaximizedCore = value;
					Document.SetMaximized(value);
				}
			}
		}
		protected override void SetIsActive(bool isActive) {
			bool shouldInvalidate = false;
			if(Info != null && Info.IsActive != isActive) {
				shouldInvalidate = true;
				Info.IsActive = isActive;
			}
			if(shouldInvalidate && !Borderless) {
				if(isActive)
					BringToFront();
				InvalidateNC();
			}
		}
		public void SetMaximized(bool value) {
			SetMaximized(value, true);
		}
		bool isMaximizedCore;
		protected internal override bool IsMaximized {
			get { return isMaximizedCore; }
		}
		protected override void ProcessWindowPosChanged(IntPtr lParam) {
			if(Handler != null)
				Handler.ProcessWindowPosChanged(this, lParam);
		}
		protected override void OnMouseWheel(MouseEventArgs e) {
			var eax = e as HandledMouseEventArgs;
			if(IsMaximized && eax != null) eax.Handled = true;
			base.OnMouseWheel(e);
		}
		protected override bool HasMaximizeButton {
			get { return (Document != null) ? Document.HasMaximizeButton() : base.HasMaximizeButton; }
		}
		public Rectangle AnimatedBounds {
			get { return Bounds; }
			set { Bounds = value; }
		}
		public void OnEndBoundAnimation(DevExpress.Utils.Drawing.Animation.BoundsAnimationInfo anim) {
			if(!updateIsMaximizedCore) return;
			isMaximizedCore = !isMaximizedCore;
			InvalidateNC();
			if(!isMaximizedCore)
				Document.SetMaximized(isMaximizedCore);
		}
		#region IMouseWheelSupport Members
		void IMouseWheelSupport.OnMouseWheel(MouseEventArgs e) {
			if(Parent is IMouseWheelSupport && !IsMaximized)
				(Parent as IMouseWheelSupport).OnMouseWheel(e);
		}
		#endregion
		#region IToolTipControlClient Members
		ToolTipControlInfo IToolTipControlClient.GetObjectInfo(Point point) {
			ToolTipControlInfo toolTipInfo = null;
			if(Document == null || (string.IsNullOrEmpty(Document.ToolTip) && Document.SuperTip == null &&
				Document.TooltipIconType == DevExpress.Utils.ToolTipIconType.None && Document.TooltipTitle == null)) return null;
			toolTipInfo = new ToolTipControlInfo(point, Document.ToolTip);
			toolTipInfo.SuperTip = Document.SuperTip;
			toolTipInfo.IconType = Document.TooltipIconType;
			toolTipInfo.Title = Document.TooltipTitle;
			ToolTipController.ShowHint(toolTipInfo);
			return toolTipInfo;
		}
		bool IToolTipControlClient.ShowToolTips {
			get { return true; }
		}
		#endregion
	}
	public class WidgetFormInfo : FloatDocumentFormInfo {
		public WidgetFormInfo(IFloatDocumentFormInfoOwner owner)
			: base(owner) {
		}
		protected override int HitTestCore(Point pt) {
			WidgetContainer container = Owner as WidgetContainer;
			if(!container.Document.Properties.CanDragging) return NativeMethods.HT.HTOBJECT;
			int animationCount = XtraAnimator.Current.Animations.GetAnimationsCountByObject(container.Parent as ISupportXtraAnimation);
			if(animationCount != 0) return NativeMethods.HT.HTNOWHERE;
			int result = NativeMethods.HT.HTTRANSPARENT;
			if(caption.Contains(pt)) result = NativeMethods.HT.HTCAPTION;
			if(client.Contains(pt)) result = NativeMethods.HT.HTCLIENT;
			if(controlBox.Contains(pt)) result = NativeMethods.HT.HTOBJECT;
			result = GetHitTestForContainerResizeZone(pt, container, result);
			return result;
		}
		public override void OnContextMenu(MouseEventArgs e) {
			WidgetContainer container = Owner as WidgetContainer;
			if(caption.Contains(e.Location)) {
				if(!controlBox.Contains(e.Location)) {
					Owner.ShowContextMenu(new Point(e.Location.X - left.Width, e.Location.Y - caption.Height));
				}
			}
		}
		protected virtual int GetHitTestForContainerResizeZone(Point pt, WidgetContainer container, int result) {
			if(!container.Document.Properties.CanResize) return result;
			if(container.Host.View.LayoutMode == LayoutMode.StackLayout) {
				if(container.Document != null && container.Document.Parent != null && container.Document.Parent.IsHorizontal) {
					if(right.Contains(pt)) return NativeMethods.HT.HTRIGHT;
				}
				else
					if(bottom.Contains(pt)) return NativeMethods.HT.HTBOTTOM;
			}
			if(container.Document != null && container.Host.View.LayoutMode == LayoutMode.TableLayout) {
				Document document = container.Document;
				TableGroup tableGroup = container.Host.View.TableGroup;
				bool canHorizontalResize = document.ColumnSpan != 1;
				bool canVerticalResize = document.RowSpan != 1;
				bool canCornerResize = canVerticalResize && canHorizontalResize;
				if(topLeft.Contains(pt) && (canCornerResize || tableGroup.CheckTopLeft(document))) return NativeMethods.HT.HTTOPLEFT;
				if(topRight.Contains(pt) && (canCornerResize || tableGroup.CheckTopRight(document))) return NativeMethods.HT.HTTOPRIGHT;
				if(top.Contains(pt) && (canVerticalResize || tableGroup.CheckTop(document))) return NativeMethods.HT.HTTOP;
				if(bottomLeft.Contains(pt) && (canCornerResize || tableGroup.CheckBottomLeft(document))) return NativeMethods.HT.HTBOTTOMLEFT;
				if(left.Contains(pt) && (canHorizontalResize || tableGroup.CheckLeft(document))) return NativeMethods.HT.HTLEFT;
				if(bottomRight.Contains(pt) && (canCornerResize || tableGroup.CheckBottomRight(document))) return NativeMethods.HT.HTBOTTOMRIGHT;
				if(right.Contains(pt) && (canHorizontalResize || tableGroup.CheckRight(document))) return NativeMethods.HT.HTRIGHT;
				if(bottom.Contains(pt) && (canVerticalResize || tableGroup.CheckBottom(document))) return NativeMethods.HT.HTBOTTOM;
			}
			if(container.Document != null && container.Host.View.LayoutMode == LayoutMode.FlowLayout) {
				FlowDirection flowDirection = container.Host.View.FlowLayoutProperties.FlowDirection;
				if(flowDirection == FlowDirection.LeftToRight || flowDirection == FlowDirection.TopDown) {
					if(bottomRight.Contains(pt)) return NativeMethods.HT.HTBOTTOMRIGHT;
					if(bottom.Contains(pt)) return NativeMethods.HT.HTBOTTOM;
					if(right.Contains(pt)) return NativeMethods.HT.HTRIGHT;
				}
				if(flowDirection == FlowDirection.RightToLeft) {
					if(bottomLeft.Contains(pt)) return NativeMethods.HT.HTBOTTOMLEFT;
					if(bottom.Contains(pt)) return NativeMethods.HT.HTBOTTOM;
					if(left.Contains(pt)) return NativeMethods.HT.HTLEFT;
				}
				if(flowDirection == FlowDirection.BottomUp) {
					if(topRight.Contains(pt)) return NativeMethods.HT.HTTOPRIGHT;
					if(top.Contains(pt)) return NativeMethods.HT.HTTOP;
					if(right.Contains(pt)) return NativeMethods.HT.HTRIGHT;
				}
			}
			return result;
		}
		protected override void MergeCustomButtons(ButtonsPanel ownerPanel) {
			WidgetContainer container = Owner as WidgetContainer;
			ownerPanel.Buttons.Merge(container.Document.CustomHeaderButtons);
		}
	}
}
