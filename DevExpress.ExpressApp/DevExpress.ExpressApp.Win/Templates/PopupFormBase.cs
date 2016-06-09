#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       eXpressApp Framework                                        }
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
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using DevExpress.ExpressApp.Templates;
using DevExpress.ExpressApp.Utils;
using DevExpress.ExpressApp.Win.Controls;
using DevExpress.ExpressApp.Win.Utils;
using DevExpress.Utils.Controls;
using DevExpress.XtraLayout;
using DevExpress.ExpressApp.Win.SystemModule;
using DevExpress.ExpressApp.Win.Layout;
using DevExpress.XtraEditors;
using DevExpress.ExpressApp.Win.Core;
using DevExpress.ExpressApp.Model;
namespace DevExpress.ExpressApp.Win.Templates {
	public class PopupFormBase : DevExpress.XtraEditors.XtraForm, IWindowTemplate, ISupportViewChanged, ISupportStoreSettings, IViewSiteTemplate, IBarManagerHolder {
		private Size initialMinimumSize = new Size(420, 150);
		private const string FrameTemplatesPopupForm = @"FrameTemplates\PopupForm";
		private bool autoShrink;
		private Size GetViewSitePanelMinSize() {
			Control viewControl = ViewSitePanel.Controls[0];
			if(viewControl is LayoutControl) {
				LayoutControl layoutControl = viewControl as LayoutControl;
				layoutControl.BeginUpdate();
				layoutControl.EndUpdate();
				return layoutControl.Root.MinSize;
			}
			IXtraResizableControl resizableControl = viewControl as IXtraResizableControl;
			if(resizableControl != null) {
				return resizableControl.MinSize;
			}
			return viewControl.MinimumSize;
		}
		private TemplatesHelper templatesHelper;
		private IModelTemplateWin modelTemplate;
		protected virtual XafLayoutControl BottomPanel {
			get { return null; }
		}
		protected virtual PanelControl ViewSitePanel {
			get { return null; }
		}
		protected override void OnShown(EventArgs e) {
			base.OnShown(e);
			MoveFocusToFirstViewControl();
		}
		protected override void OnLoad(System.EventArgs e) {
			base.OnLoad(e);
			for(int i=0;i<5;i++) {
				UpdateSize();
			}
			if(StartPosition == FormStartPosition.CenterScreen) {
				System.Windows.Forms.Screen screen = System.Windows.Forms.Screen.FromPoint(Location);
				int x = screen.WorkingArea.X + ((screen.WorkingArea.Width - Width) / 2);
				int y = screen.WorkingArea.Y + (screen.WorkingArea.Height - Height) / 2;
				Location = new Point(x, y);
			}
		}
		protected void OnSettingsReloaded() {
			if(SettingsReloaded != null) {
				SettingsReloaded(this, EventArgs.Empty);
			}
		}
		public virtual void UpdateSize() {
			if(BottomPanel != null) {
				BottomPanel.MinimumSize = BottomPanel.MinSize;
			}
			if(ViewSitePanel != null) {
				if(ViewSitePanel.Controls.Count > 0) {
					Size viewControlMinimumSize = GetViewSitePanelMinSize();
					Size calculatedSize = new Size(viewControlMinimumSize.Width + ViewSitePanel.Padding.Size.Width, viewControlMinimumSize.Height + ViewSitePanel.Padding.Size.Height + BottomPanel.Size.Height + Padding.Size.Height);
					calculatedSize = OnCustomizeMinimumSize(calculatedSize);
					calculatedSize = new Size(Math.Max(calculatedSize.Width, initialMinimumSize.Width), Math.Max(calculatedSize.Height, initialMinimumSize.Height));
					MinimumSize = calculatedSize;
					if(AutoShrink && !IsSizeable) {
						calculatedSize = OnCustomizeClientSize(calculatedSize);
						if(WindowState == FormWindowState.Maximized) {
							ClientSize = new Size(
								Math.Max(calculatedSize.Width, Size.Width),
								Math.Max(calculatedSize.Height, Size.Height));
						}
						else {
							ClientSize = calculatedSize;
						}
					}
				}
			}
		}
		protected virtual Size OnCustomizeClientSize(Size calculatedMinumumSize) {
			Size result = calculatedMinumumSize;
			if(CustomizeClientSize != null) {
				CustomSizeEventArgs eventArgs = new CustomSizeEventArgs(calculatedMinumumSize);
				CustomizeClientSize(this, eventArgs);
				if(eventArgs.Handled) {
					result = eventArgs.CustomSize;
				}
			}
			return result;
		}
		protected virtual Size OnCustomizeMinimumSize(Size calculatedMinumumSize) {
			Size result = calculatedMinumumSize;
			if(CustomizeMinimumSize != null) {
				CustomSizeEventArgs eventArgs = new CustomSizeEventArgs(calculatedMinumumSize);
				CustomizeMinimumSize(this, eventArgs);
				if(eventArgs.Handled) {
					result = new Size(Math.Max(calculatedMinumumSize.Width, eventArgs.CustomSize.Width), Math.Max(calculatedMinumumSize.Height, eventArgs.CustomSize.Height));
				}
			}
			return result;
		}
		public Size InitialMinimumSize {
			get { return initialMinimumSize; }
			set { initialMinimumSize = value; }
		}
		public bool AutoShrink {
			get { return autoShrink; }
			set { autoShrink = value; }
		}
		public virtual DevExpress.XtraBars.BarManager BarManager {
			get { return null; }
		}
#pragma warning disable 0067
		public event EventHandler BarManagerChanged;
#pragma warning restore 0067
		public event EventHandler<CustomSizeEventArgs> CustomizeClientSize;
		public event EventHandler<CustomSizeEventArgs> CustomizeMinimumSize;
		public void AddControl(Control control, string caption) {
			if(control != null) {
				ViewSitePanel.Controls.Add(control);
				control.Dock = DockStyle.Fill;
				Text = caption;
			}
		}
		public PopupFormBase() {
			NativeMethods.SetExecutingApplicationIcon(this);
			autoShrink = true;
			ShowInTaskbar = true;
			KeyPreview = true;
		}
		#region ISupportStoreSettings Members
		public void SetSettings(IModelTemplate modelTemplate) {
			this.modelTemplate = (IModelTemplateWin)modelTemplate;
			templatesHelper = new TemplatesHelper(this.modelTemplate);
			string viewId = String.Empty;
			if(ViewSiteManager.View != null) {
				viewId = ViewSiteManager.View.Id;
			}
			FormStateModelSynchronizer.Model = templatesHelper.GetFormStateNode(viewId);
			ReloadSettings();
		}
		public void ReloadSettings() {
			ReloadSettingsCore();
			OnSettingsReloaded();
		}
		protected virtual void ReloadSettingsCore() {
			if(modelTemplate != null && IsSizeable) {
				FormStateModelSynchronizer.ApplyModel();
			}
		}
		public virtual void SaveSettings() {
			if(modelTemplate != null && IsSizeable) {
				FormStateModelSynchronizer.SynchronizeModel();
			}
		}
		public event EventHandler SettingsReloaded;
		#endregion
		#region IFrameTemplate Members
		public virtual ICollection<IActionContainer> GetContainers() { return null; }
		public virtual IActionContainer DefaultContainer {
			get { return null; }
		}
		protected virtual ViewSiteManager ViewSiteManager {
			get { return null; }
		}
		public virtual void SetView(DevExpress.ExpressApp.View view) {
			ViewSiteManager.SetView(view);
			if(ViewChanged != null) {
				ViewChanged(this, new TemplateViewChangedEventArgs(view));
			}
			if(view != null) {
				if(view.Model != null) {
					string imageName = ViewImageNameHelper.GetImageName(view);
					NativeMethods.SetFormIcon(this,
						ImageLoader.Instance.GetImageInfo(imageName).Image,
						ImageLoader.Instance.GetLargeImageInfo(imageName).Image);
				}
				else {
					NativeMethods.SetFormIcon(this,
						NativeMethods.ExeIconSmall,
						NativeMethods.ExeIconLarge);
				}
				Text = view.Caption;
				MoveFocusToFirstViewControl();
			}
		}
		private void MoveFocusToFirstViewControl() {
			if(ViewSitePanel != null) {
				ViewSitePanel.SelectNextControl(ViewSitePanel, true, true, true, false);
			}
		}
		#endregion
		#region ISupportViewChanged Members
		public event EventHandler<TemplateViewChangedEventArgs> ViewChanged;
		#endregion
		#region IViewSiteTemplate Members
		public virtual object ViewSiteControl {
			get { return null; }
		}
		#endregion
		#region IWindowTemplate Members
		protected virtual FormStateModelSynchronizer FormStateModelSynchronizer { get { return null; } }
		public virtual void SetStatus(System.Collections.Generic.ICollection<string> statusMessages) { }
		public virtual void SetCaption(string caption) {
			Text = caption;
		}
		public virtual bool IsSizeable {
			get {
				return FormBorderStyle == FormBorderStyle.Sizable;
			}
			set {
				Rectangle storedBounds = DesktopBounds;
				if(value) {
					FormBorderStyle = FormBorderStyle.Sizable;
				}
				else {
					FormBorderStyle = FormBorderStyle.FixedDialog;
				}
				DesktopBounds = storedBounds;
				MinimizeBox = value;
				MaximizeBox = value;
			}
		}
		#endregion
	}
}
