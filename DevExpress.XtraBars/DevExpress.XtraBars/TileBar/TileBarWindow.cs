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

using DevExpress.Utils;
using DevExpress.Utils.Drawing.Animation;
using DevExpress.Utils.Win;
using DevExpress.XtraEditors;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Permissions;
using System.Text;
using System.Windows.Forms;
namespace DevExpress.XtraBars.Navigation {
	public class TileBarWindow : BasePopupToolWindow, ISupportToolTipsForm{
		Control owner;
		public ITileBarWindowOwner DropDownOwner { get; protected set; }
		FlyoutPanelOptions DefaultDropDownOptions = new FlyoutPanelOptions();
		protected override bool AutoInitialization { get { return false; } }
		protected override bool ShouldUseSkinPadding { get { return false; } }
		protected override PopupToolWindowAnimation AnimationType { get { return Options.AnimationType; } }
		protected override PopupToolWindowAnchor AnchorType { get { return Options.AnchorType; } }
		protected override int HorzIndent { get { return Options.HorzIndent; } }
		protected override int VertIndent { get { return Options.VertIndent; } }
		protected override Control OwnerControl { get { return owner; } }
		protected override bool SyncLocationWithOwner { get { return true; } }
		protected override Point FormLocation { get { return Options.Location; } }
		protected override Control MessageRoutingTarget { get { return null; } }
		protected override LookAndFeel.ISupportLookAndFeel LookAndFeelProvider { get { return null; } }
		protected override bool KeepParentFormActive { get { return true; } }
		protected override int AnimationLength { get { return 1500; } }
		public override string ToString() { return string.IsNullOrEmpty(Text) ? base.ToString() : Text; }
		protected override Size FormSize { get { return GetFormSize(); } }
		protected override Control CreateContentControl() { return DropDownOwner == null ? null : DropDownOwner.GetDropDownContent(); }
		protected override bool CloseOnOuterClick { get { return DropDownOwner == null ? true : DropDownOwner.CloseOnOuterClick; } }
		public FlyoutPanelOptions Options { get { return GetOptions(); } }
		FlyoutPanelOptions GetOptions() {
			if(DropDownOwner == null || DropDownOwner.DropDownOptions == null)
				return DefaultDropDownOptions;
			return DropDownOwner.DropDownOptions;
		}
		Size GetFormSize() {
			if(DropDownOwner == null) return owner.Size;
			if(DropDownOwner.Orientation == Orientation.Horizontal)
				return new Size(owner.Width, DropDownOwner.DropDownHeight);
			return new Size(DropDownOwner.DropDownHeight, owner.Height);
		}
		public TileBarWindow ChildWindow { get; set; }
		public TileBarWindow(Control owner, ITileBarWindowOwner dropDownOwner) {
			this.DropDownOwner = dropDownOwner;
			this.owner = owner;
		}
		const int WS_POPUP = unchecked((int)0x80000000);
		protected override CreateParams CreateParams {
			get {
				CreateParams cp = base.CreateParams;
				cp.Style |= WS_POPUP;
				return cp;
			}
		}
		Form clipForm;
		Form ClipForm {
			get { return clipForm; }
			set {
				if(clipForm == value) return;
				var oldValue = clipForm;
				clipForm = value;
				OnClipFormChanged(oldValue, ClipForm);
			}
		}
		void OnClipFormChanged(Form oldClipForm, Form clipForm) {
			UnsubscribeOnClipForm(oldClipForm);
			SubscribeOnClipForm(clipForm);
		}
		Form ownerFormCore;
		Form OwnerFormInternal {
			get { return ownerFormCore; }
			set {
				if(ownerFormCore == value) return;
				ownerFormCore = value;
				OnOwnerFormChangedInternal(ownerFormCore);
			}
		}
		void OnOwnerFormChangedInternal(Form ownerForm) { }
		protected override Size CalcFormSize() {
			EnsureOwnerForm();
			EnsureClipForm();
			UpdateContent();
			return base.CalcFormSize();
		}
		void UpdateContent() {
			UpdateDropDownBackColor();
			if(DropDownOwner.IsTileNavPane)
				UpdateTileBar();
		}
		void UpdateDropDownBackColor() {
			if(Content == null || DropDownOwner == null) return;
			if(DropDownOwner.DropDownBackColor.IsEmpty) return;
			Content.BackColor = DropDownOwner.DropDownBackColor;
		}
		protected internal void UpdateTileBar() {
			if(Content == null) return;
			if(Content is TileBar)
				DropDownOwner.UpdateTileBar(Content as TileBar);
		}
		void EnsureOwnerForm() {
			OwnerFormInternal = this.owner.FindForm();
		}
		void EnsureClipForm() {
			ClipForm = FindClipForm();
		}
		Form FindClipForm() {
			if(OwnerFormInternal is TileBarWindow)
				return (OwnerFormInternal as TileBarWindow).FindClipForm();
			return OwnerFormInternal;
		}
		void SubscribeOnClipForm(Form form) {
			if(form == null) return;
			form.SizeChanged += OnClipFormSizeChanged;
		}
		void UnsubscribeOnClipForm(Form form) {
			if(form == null) return;
			form.SizeChanged -= OnClipFormSizeChanged;
		}
		void OnClipFormSizeChanged(object sender, EventArgs e) {
			MakeFormClipping();
		}
		public bool HideDropDown() {
			if(OwnerFormInternal == null) 
				return false;
			if(ChildWindow != null && ChildWindow.Visible) 
				ChildWindow.HideDropDown();
			base.HideToolWindow();
			if(DropDownOwner != null)
				DropDownOwner.OnDropDownClosed();
			return true;
		}
		protected override void ForceClosingCore(bool immediate) {
			base.ForceClosingCore(immediate);
			if(DropDownOwner != null)
				DropDownOwner.OnDropDownClosed();
		}
		public override void ShowToolWindow(bool immediate) {
			base.ShowToolWindow(immediate);
			MakeFormClipping();
		}
		protected internal bool TryToShowToolWindow() {
			bool inAnimation = XtraAnimator.Current.Animations.GetAnimationsCountByObject(Handler.AnimationProvider) > 0;
			if(inAnimation) {
				return false;
			}
			this.ShowToolWindow();
			return true;
		}
		public override bool Contains(Point point) {
			if(OwnerControl == null || OwnerControl.IsDisposed || OwnerFormInternal == null) return false;
			Rectangle rectControl = OwnerControl.RectangleToScreen(OwnerControl.ClientRectangle);
			if(ChildWindow != null && ChildWindow.Visible) {
				Rectangle rectChildWindow = ChildWindow.Bounds;
				return this.Bounds.Contains(point) || rectControl.Contains(point) || rectChildWindow.Contains(point);
			}
			return this.Bounds.Contains(point) || rectControl.Contains(point);
		}
		void MakeFormClipping() {
			if(ClipForm == null) return;
			Rectangle clipScreenRect = ClipForm.RectangleToScreen(ClipForm.ClientRectangle);
			Rectangle thisScreenRect = this.RectangleToScreen(this.ClientRectangle);
			Rectangle rect = Rectangle.Intersect(clipScreenRect, thisScreenRect);
			GraphicsPath clipShape = new GraphicsPath();
			clipShape.AddRectangle(this.RectangleToClient(rect));
			this.Region = new Region(clipShape);
		}
		protected override void Dispose(bool disposing) {
			if(disposing) {
				UnsubscribeOnClipForm(ClipForm);
			}
			base.Dispose(disposing);
		}
		protected override void DoShow() {
			if(Visible) 
				return;
			bool? oldValue = null;
			XtraForm xform = OwnerForm as XtraForm;
			if(xform != null && xform.IsHandleCreated) {
				oldValue = XtraForm.SuppressDeactivation;
				XtraForm.SuppressDeactivation = true;
			}
			DoShowCore();
			if(oldValue.HasValue)
				XtraForm.SuppressDeactivation = oldValue.Value;
		}
		void DoShowCore() {
			if(OwnerFormInternal != null && !OwnerFormInternal.IsMdiChild) {
				this.TopMost = OwnerFormInternal.TopMost;
				Show(OwnerFormInternal);
			}
			else
				Show();
		}
		bool ISupportToolTipsForm.ShowToolTipsFor(Form form) {
			return true;
		}
		bool ISupportToolTipsForm.ShowToolTipsWhenInactive {
			get { return true; }
		}
	}
	public interface ITileBarWindowOwner {
		void OnDropDownClosed();
		TileBarWindow GetDropDown();
		bool CloseOnOuterClick { get; }
		FlyoutPanelOptions DropDownOptions { get; }
		Control GetDropDownContent();
		int DropDownHeight { get; }
		Color DropDownBackColor { get; }
		bool IsTileNavPane { get; }
		Orientation Orientation { get; }
		void UpdateTileBar(TileBar tileBar);
	}
}
