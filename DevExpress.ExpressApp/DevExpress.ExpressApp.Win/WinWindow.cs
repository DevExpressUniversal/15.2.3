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
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Utils;
using DevExpress.ExpressApp.Win.Controls;
using DevExpress.ExpressApp.Win.Core;
using DevExpress.ExpressApp.Win.Templates;
using DevExpress.ExpressApp.Win.Utils;
using DevExpress.Persistent.Base;
namespace DevExpress.ExpressApp.Win {
	public class QueryIconEventArgs : EventArgs {
		private Icon iconLarge;
		private Icon iconSmall;
		public QueryIconEventArgs(Icon iconSmall, Icon iconLarge) {
			this.iconLarge = iconLarge;
			this.iconSmall = iconSmall;
		}
		public Icon IconSmall {
			get { return iconSmall; }
			set { iconSmall = value; }
		}
		public Icon IconLarge {
			get { return iconLarge; }
			set { iconLarge = value; }
		}
	}
	public class WinWindow : Window {
		private static readonly Icon defaultFormIcon = new Form().Icon;
		public static Form LastActiveExplorer { set; get; }
		private bool checkCanClose = true;
		private Exception lastExceptionOnClosing;
		private int closingExceptionCount = 0;
		private KeyEventHandler keyDown;
		private Boolean isClosing;
		private Form SuspendMdiParent(Form child) {
			if(child == null) return null;
			Form mdiParent;
			mdiParent = Form.MdiParent;
			if(mdiParent != null) {
				IBarManagerHolder holder = mdiParent as IBarManagerHolder;
				if(holder != null && holder.BarManager != null) {
					holder.BarManager.BeginUpdate();
				}
				mdiParent.SuspendLayout();
			}
			return mdiParent;
		}
		private void ResumeMdiParent(Form mdiParent) {
			if(mdiParent != null) {
				IBarManagerHolder holder = mdiParent as IBarManagerHolder;
				if(holder != null && holder.BarManager != null) {
					holder.BarManager.EndUpdate();
				}
				mdiParent.ResumeLayout();
			}
		}
		private void DoOnFormClosing(CancelEventArgs e) {
			if(!isClosing) {
				Tracing.Tracer.LogText("Window closing: " + Form.Text);
				BindingHelper.EndCurrentEdit(Form);
				SaveTemplateModel();
				SaveViewModel();
				checkCanClose = false;
				try {
					if(Closing != null) {
						Closing(this, e);
					}
				}
				finally {
					checkCanClose = true;
				}
				if(!e.Cancel) {
					e.Cancel = !CanClose();
				}
				if(!e.Cancel && ((FormClosingEventArgs)e).CloseReason == CloseReason.UserClosing) {
					checkCanClose = false;
					try {
						if(Form.IsMdiContainer && Application.ShowViewStrategy is WinShowViewStrategyBase) {
							foreach(WinWindow window in ((WinShowViewStrategyBase)Application.ShowViewStrategy).Inspectors) {
								if(window.Form.MdiParent == Form) {
									window.ProcessFormClosing();
								}
							}
						}
						ProcessFormClosing();
					}
					finally {
						checkCanClose = true;
					}
				}
			}
		}
		private void ProcessFormClosing() {
			isClosing = true;
			Form mdiParent = SuspendMdiParent(Form);
			if(Form != null) {
				Form.SuspendLayout();
			}
			try {
				if(View != null) {
					View.Close(false);
				}
				DeactivateViewControllers();
			}
			finally {
				ResumeMdiParent(mdiParent);
				isClosing = false;
			}
		}
		private void Form_Closing(object sender, CancelEventArgs e) {
			DoOnFormClosing(e);
		}
		private void Form_Closed(object sender, EventArgs e) {
			Tracing.Tracer.LogText("Window closed: " + Form.Text);
			if(LastActiveExplorer == Form) {
				LastActiveExplorer = null;
			}
			OnWindowClosed();
		}
		private void OnWindowClosed() {
			if(Closed != null) {
				Closed(this, EventArgs.Empty);
			}
		}
		private void Form_Disposed(object sender, EventArgs e) {
			Dispose();
		}
		private void Form_KeyDown(object sender, KeyEventArgs e) {
			if(keyDown != null) {
				keyDown(this, e);
			}
		}
		private void Form_KeyUp(object sender, KeyEventArgs e) {
			if(KeyUp != null) {
				KeyUp(this, e);
			}
		}
		private void Form_Load(object sender, EventArgs e) {
			if(LastActiveExplorer == null && Form is IXafDocumentsHostWindow) {
				LastActiveExplorer = Form;
			}
		}
		private void Form_Activated(object sender, EventArgs e) {
			if(Form is IXafDocumentsHostWindow) {
				LastActiveExplorer = Form;
			}
			for(int i = System.Windows.Forms.Application.OpenForms.Count - 1; i >= 0; i--) {
				Form form = System.Windows.Forms.Application.OpenForms[i];
				if(Form == form) {
					break;
				}
				if(form.Modal) {
					form.Invoke(new System.Threading.ThreadStart(delegate() {
						form.Activate();
						form.Focus();
					}));
					break;
				}
			}
			if(Activated != null) {
				Activated(this, EventArgs.Empty);
			}
		}
		private void Form_Shown(Object sender, EventArgs e) {
			if((Form != null) && (View is DetailView)) {
				Form.Update();
				Form.Invalidate(true);
				Form.Cursor = Cursors.WaitCursor;
				View.RaiseActivated();
				Form.Cursor = Cursors.Default;
			}
		}
		private void UpdateFormKeyPreview() {
			if(Form != null) {
				Form.KeyPreview = (keyDown != null && keyDown.GetInvocationList().Length > 0);
			}
		}
		private bool GetUserConfirmationToForcedClose() {
			DialogResult result = ((WinApplication)Application).GetUserChoice(
				CaptionHelper.GetLocalizedText("Confirmations", "ConfirmForcedCloseWindow"), MessageBoxButtons.YesNo);
			if(result == DialogResult.Yes) {
				Tracing.Tracer.LogText("User chooses to force window closing process");
				return true;
			}
			return false;
		}
		private bool IsRepeatedException(Exception exception) {
			if((lastExceptionOnClosing != null)
				&& (lastExceptionOnClosing.Message != exception.Message)) {
				closingExceptionCount = 0;
			}
			else {
				closingExceptionCount++;
			}
			return (closingExceptionCount > 3);
		}
		private void WinWindow_CustomHandleExceptionOnClosing(object sender, HandleExceptionEventArgs e) {
			Tracing.Tracer.LogError(e.Exception);
			HandleExceptionEventArgs args = new HandleExceptionEventArgs(e.Exception);
			if(CustomHandleExceptionOnClosing != null) {
				CustomHandleExceptionOnClosing(this, args);
			}
			if(!args.Handled) {
				if(IsRepeatedException(e.Exception) && GetUserConfirmationToForcedClose()) {
					if(Form.IsMdiContainer && Application.ShowViewStrategy is WinShowViewStrategyBase) {
						List<WinWindow> inspectors = new List<WinWindow>(((WinShowViewStrategyBase)Application.ShowViewStrategy).Inspectors);
						foreach(WinWindow window in inspectors) {
							if(window.Form.MdiParent == Form) {
								window.OnWindowClosed();
							}
						}
					}
					OnWindowClosed();
					e.Handled = true;
				}
				else {
					lastExceptionOnClosing = e.Exception;
					e.Handled = false;
				}
			}
		}
		private void SubscribeToForm() {
			if(Form != null) {
				Form.Closing -= new CancelEventHandler(Form_Closing);
				Form.Closing += new CancelEventHandler(Form_Closing);
				if(Form is XtraFormTemplateBase) {
					((XtraFormTemplateBase)Form).CustomHandleExceptionOnClosing += new EventHandler<HandleExceptionEventArgs>(WinWindow_CustomHandleExceptionOnClosing);
				}
				Form.Closed += new EventHandler(Form_Closed);
				Form.Load += new EventHandler(Form_Load);
				Form.Activated += new EventHandler(Form_Activated);
				Form.Disposed += new EventHandler(Form_Disposed);
				Form.KeyDown += new KeyEventHandler(Form_KeyDown);
				Form.KeyUp += new KeyEventHandler(Form_KeyUp);
				Form.Shown += new EventHandler(Form_Shown);
			}
		}
		private void UnsubscribeFromForm() {
			if(Form != null) {
				Form.Closing -= new CancelEventHandler(Form_Closing);
				if(Form is XtraFormTemplateBase) {
					((XtraFormTemplateBase)Form).CustomHandleExceptionOnClosing -= new EventHandler<HandleExceptionEventArgs>(WinWindow_CustomHandleExceptionOnClosing);
				}
				Form.Closed -= new EventHandler(Form_Closed);
				Form.Load -= new EventHandler(Form_Load);
				Form.Activated -= new EventHandler(Form_Activated);
				Form.Disposed -= new EventHandler(Form_Disposed);
				Form.KeyDown -= new KeyEventHandler(Form_KeyDown);
				Form.KeyUp -= new KeyEventHandler(Form_KeyUp);
				Form.Shown -= new EventHandler(Form_Shown);
			}
		}
		protected override void OnTemplateChanging() {
			UnsubscribeFromForm();
			base.OnTemplateChanging();
		}
		protected override void OnTemplateChanged() {
			base.OnTemplateChanged();
			if(Form != null) {
				UpdateFormKeyPreview();
				SubscribeToForm();
				if(Form.Icon == defaultFormIcon) {
					QueryIconEventArgs iconArgs = new QueryIconEventArgs(NativeMethods.ExeIconSmall, NativeMethods.ExeIconLarge);
					if(QueryDefaultFormIcon != null) {
						QueryDefaultFormIcon(this, iconArgs);
					}
					NativeMethods.SetFormIcon(Form, iconArgs.IconSmall, iconArgs.IconLarge);
				}
				Form.Tag = EasyTestTagHelper.FormatTestField("FormCaption");
			}
		}
		protected override void OnViewChanging(View view, Frame sourceFrame, ViewChangingEventArgs args) {
			base.OnViewChanging(view, sourceFrame, args);
			lastExceptionOnClosing = null;
			closingExceptionCount = 0;
		}
		protected override void OnViewChanged(Frame sourceFrame) {
			base.OnViewChanged(sourceFrame);
			if(Form != null && Form.IsHandleCreated && (View != null) && View is DetailView) {
				Form.Update();
				Form.Invalidate(true);
				Form.Cursor = Cursors.WaitCursor;
				View.RaiseActivated();
				Form.Cursor = Cursors.Default;
			}
		}
		public Form Form {
			get { return (Form)Template; }
		}
		public WinWindow(XafApplication application, TemplateContext context, ICollection<Controller> controllers, bool isMain, bool activateControllersImmediatelly)
			: base(application, context, controllers, isMain, activateControllersImmediatelly) {
			Tracing.Tracer.LogVerboseValue("WinWindow.activateControllersImmediatelly", activateControllersImmediatelly);
			CreateTemplate();
		}
		protected override void DisposeCore() {
			UnsubscribeFromForm();
			base.DisposeCore();
		}
		public override bool Close(bool isForceRefresh) {
			if(!IsClosing && (Form != null)) {
				Form mdiParent = SuspendMdiParent(Form);
				try {
					Form.Close();
				}
				finally {
					ResumeMdiParent(mdiParent);
				}
			}
			return Form == null;
		}
		public bool CanClose() {
			if(checkCanClose && (View != null)) {
				return View.CanClose();
			}
			return true;
		}
		public void Show() {
			if(Form != null) {
				if(Form.WindowState == FormWindowState.Minimized) {
					NativeMethods.RestoreForm(Form);
				}
				Form.Show();
				Form.Activate();
				Form.BringToFront();
			}
		}
		public DialogResult ShowDialog() {
			return Form.ShowDialog();
		}
		public Boolean IsClosing {
			get { return isClosing; }
		}
		public event EventHandler Closed;
		public event CancelEventHandler Closing;
		public event KeyEventHandler KeyDown {
			add {
				keyDown += value;
				UpdateFormKeyPreview();
			}
			remove {
				keyDown -= value;
				UpdateFormKeyPreview();
			}
		}
		public event KeyEventHandler KeyUp;
		public event EventHandler<HandleExceptionEventArgs> CustomHandleExceptionOnClosing;
		protected internal event EventHandler Activated;
		public static event EventHandler<QueryIconEventArgs> QueryDefaultFormIcon;
	}
}
