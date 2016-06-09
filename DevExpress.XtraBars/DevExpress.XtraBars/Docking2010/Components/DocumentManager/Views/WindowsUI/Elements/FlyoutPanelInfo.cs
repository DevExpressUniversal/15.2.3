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
using DevExpress.Utils;
using System.Windows.Forms;
namespace DevExpress.XtraBars.Docking2010.Views.WindowsUI {
	public interface IBaseFlyoutPanelInfo : IBaseElementInfo {
		bool ProcessShow();
		bool ProcessShow(Point location);
		bool ProcessShow(Point location, bool immediate);
		void ProcessHide();
		void ProcessCancel();
		bool AttachContentControl(Control contentControl);
		void DetachContentControl();
		bool IsShown { get; }
		bool HitTest(Point screenPoint);
		Size CalcMinSize(Graphics g);
		AppearanceObject AppearanceFlyoutPanel { get; }
		void SetDirty();
	}
	abstract class BaseFlyoutPanelInfo : BaseElementInfo, IBaseFlyoutPanelInfo {
		FlyoutPanel flyoutPanelCore;
		Control contentControlCore;
		bool isContentControlRegistered, isShownCore;
		public BaseFlyoutPanelInfo(WindowsUIView owner)
			: base(owner) {
			flyoutPanelCore = CreateFlyoutPanel();
			if(FlyoutPanel != null)
				InitializeFlyoutPanel();
		}
		protected virtual void InitializeFlyoutPanel() { SubscribeFlyoutPanelEvets(); }
		protected virtual void SubscribeFlyoutPanelEvets() {
			FlyoutPanel.Hidden += FlyoutPanelHidden;
			FlyoutPanel.Disposed += FlyoutPanel_Disposed;
		}
		protected virtual void UnsubscribeFlyoutPanelEvets() {
			FlyoutPanel.Hidden -= FlyoutPanelHidden;
			FlyoutPanel.Disposed -= FlyoutPanel_Disposed;
		}
		void FlyoutPanel_Disposed(object sender, EventArgs e) { Dispose(); }
		void FlyoutPanelHidden(object sender, DevExpress.Utils.FlyoutPanelEventArgs e) {
			if(IsDisposing) return;
			UnregisterContentControl();
		}
		public AppearanceObject AppearanceFlyoutPanel { get { return FlyoutPanel.Appearance; } }
		public bool IsShown { get { return isShownCore; } }
		public new WindowsUIView Owner { get { return base.Owner as WindowsUIView; } }
		protected abstract Control OwnerControl { get; }
		protected virtual FlyoutPanel CreateFlyoutPanel() { return new FlyoutPanel(); }
		public Control ContentControl { get { return contentControlCore; } }		
		protected FlyoutPanel FlyoutPanel { get { return flyoutPanelCore; } }
		protected bool IsContentControlRegistered { get { return isContentControlRegistered; } }
		public bool HitTest(Point screenPoint) {
			if(!flyoutPanelCore.FlyoutPanelState.IsActive) return false;
			if(flyoutPanelCore.FlyoutPanelState.Form.Bounds.Contains(screenPoint))
				return true;
			return false;
		}
		void RegisterContentControl(Control contentControl) {
			if(contentControl == null || contentControl is Form) return;
			if(IsContentControlRegistered) UnregisterContentControl();
			isContentControlRegistered = RegisterContentControlCore(contentControl);
		}
		protected virtual bool RegisterContentControlCore(Control contentControl) {
			contentControl.Dock = DockStyle.Fill;
			contentControl.Parent = FlyoutPanel;
			return true;
		}
		void UnregisterContentControl() {
			if(FlyoutPanel == null || !IsContentControlRegistered) return;
			UnregisterContentControlCore();
			isContentControlRegistered = false;
		}
		protected virtual void UnregisterContentControlCore() {
			FlyoutPanel.Controls.Clear();
		}
		public override Type GetUIElementKey() { return typeof(IBaseFlyoutPanelInfo); }
		protected override void CalcCore(System.Drawing.Graphics g, Rectangle bounds) {
			FlyoutPanel.Size = bounds.Size;
			base.CalcCore(g, bounds);
		}
		#region IBaseFlayoutPanelInfo Members
		protected virtual bool AttachContentControlCore(Control contentControl) {
			if(ContentControl != null) return false;
			contentControlCore = contentControl ?? RaiseQueryContentControl();
			return contentControlCore != null;
		}
		protected virtual void DetachContentControl() {
			if(ContentControl == null) return;
			UnregisterContentControl();
			contentControlCore = null;
		}
		protected bool ProcessShow(Point location, bool immediate) {
			if(!CanShow) return false;
			if(IsShown) ProcessHide(true);
			RegisterContentControl(ContentControl);
			FlyoutPanel.OwnerControl = OwnerControl;
			isShownCore = ProcessShowCore(location, immediate);
			return isShownCore;
		}		
		protected virtual Control RaiseQueryContentControl() { return null; }
		protected void ProcessHide(bool immediate) {
			if(!IsShown) return;
			if(immediate)
				ProcessCancelCore();
			else
				ProcessHideCore();
			FlyoutPanel.OwnerControl = null;
			isShownCore = false;
		}
		protected abstract bool ProcessShowCore(Point location, bool immediate);
		protected abstract void ProcessHideCore();
		protected abstract void ProcessCancelCore();
		protected virtual bool CanShow { get { return OwnerControl != null && FlyoutPanel != null; } }
		bool IBaseFlyoutPanelInfo.ProcessShow() { return ProcessShow(Point.Empty, false); }
		bool IBaseFlyoutPanelInfo.ProcessShow(Point location) { return ProcessShow(location, false); }
		bool IBaseFlyoutPanelInfo.ProcessShow(Point location, bool immediate) { return ProcessShow(location, immediate); }
		void IBaseFlyoutPanelInfo.ProcessHide() { ProcessHide(false); }
		void IBaseFlyoutPanelInfo.ProcessCancel() { ProcessHide(true); }
		bool IBaseFlyoutPanelInfo.AttachContentControl(Control contentControl) {
			if(contentControl == null || contentControl is Form || IsDisposing) return false;
			return AttachContentControlCore(contentControl);
		}
		void IBaseFlyoutPanelInfo.DetachContentControl() {
			if(IsDisposing) return;
			DetachContentControl();
		}
		Size IBaseFlyoutPanelInfo.CalcMinSize(Graphics g) { return CalcMinSize(g); }
		protected abstract Size CalcMinSize(Graphics g);
		void IBaseFlyoutPanelInfo.SetDirty() { SetDirty(); }
		protected virtual void SetDirty() { }
		#endregion
		protected override void OnDispose() {
			UnsubscribeFlyoutPanelEvets();
			DetachContentControl();
			Ref.Dispose(ref flyoutPanelCore);
			isShownCore = false;
			base.OnDispose();
		}
	}
}
