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
using DevExpress.XtraBars.Docking2010.Customization;
using DevExpress.Utils;
using DevExpress.Skins;
using System.Windows.Forms;
using DevExpress.Utils.Win;
using DevExpress.Utils.Extensions;
namespace DevExpress.XtraBars.Docking2010.Views.WindowsUI {
	class SearchPanelInfo : BaseFlyoutPanelInfo {
		public SearchPanelInfo(WindowsUIView owner)
			: base(owner) {
			AttachContentControlCore(new SearchTileBar());
			UpdateStyleCore();
		}
		protected override FlyoutPanel CreateFlyoutPanel() { return new SearchFlyoutPanel(); }
		public new SearchTileBar ContentControl { get { return base.ContentControl as SearchTileBar; } }
		protected override System.Windows.Forms.Control OwnerControl { get { return Owner.Manager.GetOwnerControl(); } }
		protected virtual int MinWidth { get { return 320; } }
		DevExpress.Utils.Win.PopupToolWindowAnchor ConvertAnchor() {
			return AnchorType == SearchPanelAnchor.Right ? DevExpress.Utils.Win.PopupToolWindowAnchor.Right : DevExpress.Utils.Win.PopupToolWindowAnchor.Left;
		}
		protected override void UpdateStyleCore() {
			base.UpdateStyleCore();
			AppearanceHelper.Combine(AppearanceFlyoutPanel, new AppearanceObject[] { Owner.AppearanceSearchPanel }, DefaultAppearance);
			AppearanceHelper.Combine(ContentControl.AppearanceText, new AppearanceObject[] { AppearanceFlyoutPanel });
			AppearanceHelper.Combine(ContentControl.Appearance, new AppearanceObject[] { AppearanceFlyoutPanel });
			ContentControl.AppearanceGroupText.Font = SegoeUIFontsCache.GetFont("Segoe UI Light", 12);
			ContentControl.AppearanceGroupText.ForeColor = CalcGroupTextForeColor(AppearanceFlyoutPanel.ForeColor, AppearanceFlyoutPanel.BackColor);
			ContentControl.AppearanceItem.Normal.BackColor = AppearanceFlyoutPanel.BackColor;
			ContentControl.AppearanceItem.Normal.ForeColor = AppearanceFlyoutPanel.ForeColor;
		}
		Color CalcGroupTextForeColor(Color foreColor, Color backGround) {
			float r = (foreColor.R * 100.0f + (255.0f - 100.0f) * backGround.R) / 255.0f;
			float g = (foreColor.G * 100.0f + (255.0f - 100.0f) * backGround.G) / 255.0f;
			float b = (foreColor.B * 100.0f + (255.0f - 100.0f) * backGround.B) / 255.0f;
			return Color.FromArgb((int)r, (int)g, (int)b);
		}
		protected virtual AppearanceDefault DefaultAppearance { get { return GetDefaultAppearance(); } }
		protected AppearanceDefault GetDefaultAppearance() {
			AppearanceDefault defaultAppearanceCore = new AppearanceDefault(Color.White, Color.Black, new Font(AppearanceObject.ControlAppearance.Font.FontFamily, 20));
			SkinElement buttonElement = GetActionsBarElement();
			if(buttonElement != null)
				return buttonElement.Apply(defaultAppearanceCore);
			return defaultAppearanceCore;
		}
		protected virtual Skin GetSkin() { return MetroUISkins.GetSkin(Owner.ElementsLookAndFeel.ActiveLookAndFeel); }
		protected virtual SkinElement GetActionsBarElement() {
			return GetSkin()[MetroUISkins.SkinActionsBar];
		}
		protected override void InitializeFlyoutPanel() {
			FlyoutPanel.Options.AnchorType = ConvertAnchor();
			base.InitializeFlyoutPanel();
		}
		protected override bool ProcessShowCore(Point location, bool immediate) {
			if(!ContentControl.AttachProvider(Owner)) return false;
			FlyoutPanel.ShowPopup(immediate);
			ContentControl.SetFocus();
			FocusHelper.LockFocus(OwnerControl.FindForm());
			return true;
		}
		protected SearchPanelAnchor AnchorType { get { return Owner.SearchPanelProperties.AnchorType; } }
		public int Width { get { return Owner.SearchPanelProperties.Width; } }
		public DevExpress.Utils.KeyShortcut Shortcut { get { return Owner.SearchPanelProperties.Shortcut; } }
		public void ProcessMouseWheel(MouseEventArgs e) {
			Control control = FindControl(e.Location, ContentControl);
			DevExpress.XtraEditors.Drawing.IMouseWheelSupport wheelSupport = control as DevExpress.XtraEditors.Drawing.IMouseWheelSupport;
			if(wheelSupport == null) return;
			wheelSupport.OnMouseWheel(e);
		}
		Control FindControl(Point screenPoint, Control parent) {
			Point point = parent.PointToClient(screenPoint);
			if(parent == null || !parent.IsHandleCreated) return null;
			foreach(Control control in parent.Controls) {
				if(!control.Enabled || !control.Visible) continue;
				if(control.Bounds.Contains(point))
					return control;
			}
			return parent;
		}
		protected override void ProcessCancelCore() {
			if(ContentControl != null)
				ContentControl.DetachProvider();
			FocusHelper.UnlockFocus();
			FlyoutPanel.HidePopup(true);
			ContentControl.ResetFocus();
		}
		protected override Size CalcMinSize(Graphics g) {
			if(OwnerControl == null || Owner == null) return Size.Empty;
			int height = OwnerControl.Height;
			int width = (Width <= MinWidth) ? MinWidth : Width;
			return new Size(width, height);
		}
		protected override void ProcessHideCore() {
			if(ContentControl != null)
				ContentControl.DetachProvider();
			FocusHelper.UnlockFocus();
			FlyoutPanel.HidePopup();
			ContentControl.ResetFocus();
		}
		class SearchFlyoutPanel : FlyoutPanel {
			protected override FlyoutPanelToolForm CreateToolFormCore(Control owner, FlyoutPanel content, FlyoutPanelOptions options) {
				SearchToolForm toolForm = new SearchToolForm(owner, content, options);
				toolForm.TopLevel = false;
				toolForm.Parent = OwnerControl.Parent;
				return toolForm;
			}
			protected override void UpdateContentPaddings() { }
		}
		class SearchToolForm : FlyoutPanelToolForm {
			public SearchToolForm(Control owner, FlyoutPanel flyoutPanel, FlyoutPanelOptions options)
				: base(owner, flyoutPanel, options) { }
			public new void DoShow() { Show(); }
			public new SearchFlyoutPanel FlyoutPanel { get { return base.FlyoutPanel as SearchFlyoutPanel; } }
			protected override BasePopupToolWindowHandler CreateHandler() { return new SearchToolFormHandler(this); }
		}
		class SearchToolFormHandler : FlyoutPanelToolFormHandler {
			public SearchToolFormHandler(SearchToolForm toolForm)
				: base(toolForm) {
			}
			public new SearchToolForm ToolWindow { get { return base.ToolWindow as SearchToolForm; } }
			public override void OnImmediateShowToolWindow() {
				CheckToolWindowLocation();
				ToolWindow.DoShow();
			}
			protected override PopupToolWindowAnimationProviderBase CreateAnimationProvider() { return new SearchSlideAnimation(this); }
		}
		class SearchSlideAnimation : PopupToolWindowUpDownSlideAnimationProvider {
			public SearchSlideAnimation(IPopupToolWindowAnimationSupports info) : base(info) { }
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
