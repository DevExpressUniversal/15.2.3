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
using System.ComponentModel;
using System.Windows.Forms;
using DevExpress.ExpressApp.Utils;
namespace DevExpress.ExpressApp.Win.Templates {
	[Browsable(true)]
	[EditorBrowsable(EditorBrowsableState.Always)]
	[ToolboxItem(false)] 
	[DesignerCategory("Component")]
	public class ViewSiteManager : Component {
		private const bool DefaultUseDeferredLoading = false;
		private Control viewSiteControl;
		private bool isViewSiteEmpty;
		private View view;
		private bool isViewCreateControlsProcessing;
		private bool useDeferredLoading = DefaultUseDeferredLoading;
		public ViewSiteManager(IContainer container)
			: this() {
			container.Add(this);
		}
		public ViewSiteManager() {
			isViewSiteEmpty = true;
		}
		private void viewSiteControl_HandleCreated(object sender, EventArgs e) {
			if(useDeferredLoading && isViewSiteEmpty) {
				FillViewSite();
			}
		}
		private void OnViewChanged() {
			if(GetIsViewSiteReady()) {
				ClearViewSite();
				FillViewSite();
			}
		}
		private void view_ControlsCreated(object sender, EventArgs e) {
			if(!isViewCreateControlsProcessing && GetIsViewSiteReady()) {
				ClearViewSite();
				FillViewSite((Control)view.Control);
			}
		}
		private void FillViewSite() {
			if(view != null) {
				EnsureViewControl();
				FillViewSite((Control)view.Control);
			}
		}
		private void EnsureViewControl() {
			if(!view.IsControlCreated) {
				isViewCreateControlsProcessing = true;
				try {
					view.CreateControls();
				}
				finally {
					isViewCreateControlsProcessing = false;
				}
			}
		}
		private bool GetIsViewSiteReady() {
			return viewSiteControl != null && (viewSiteControl.IsHandleCreated || !useDeferredLoading);
		}
		private void FillViewSite(Control control) {
			Guard.ArgumentNotNull(control, "control");
			if(!isViewSiteEmpty) {
				throw new InvalidOperationException("ViewSite is not empty.");
			}
			viewSiteControl.SuspendLayout();
			try {
				if(control is ISupportUpdate) {
					((ISupportUpdate)control).BeginUpdate();
				}
				control.Dock = DockStyle.Fill;
				Form form = viewSiteControl.FindForm();
				if(form != null && form.Visible) {
					control.Bounds = viewSiteControl.ClientRectangle;
				}
				viewSiteControl.Controls.Add(control);
				if(control is ISupportUpdate) {
					((ISupportUpdate)control).EndUpdate();
				}
			}
			finally {
				viewSiteControl.ResumeLayout();
				isViewSiteEmpty = false;
			}
		}
		private void ClearViewSite() {
			if(!isViewSiteEmpty) {
				try {
					viewSiteControl.Controls.Clear();
				}
				finally {
					isViewSiteEmpty = true;
				}
			}
		}
		protected override void Dispose(bool disposing) {
			base.Dispose(disposing);
			ViewSiteControl = null;
		}
		public void SetView(View view) {
			if(this.view != null) {
				this.view.ControlsCreated -= new EventHandler(view_ControlsCreated);
			}
			this.view = view;
			if(this.view != null) {
				this.view.ControlsCreated += new EventHandler(view_ControlsCreated);
			}
			OnViewChanged();
		}
		public Control ViewSiteControl {
			get { return viewSiteControl; }
			set {
				if(viewSiteControl != null) {
					viewSiteControl.HandleCreated -= new EventHandler(viewSiteControl_HandleCreated);
				}
				viewSiteControl = value;
				if(viewSiteControl != null) {
					viewSiteControl.HandleCreated += new EventHandler(viewSiteControl_HandleCreated);
				}
			}
		}
		[Browsable(false)]
		public View View {
			get { return view; }
		}
		[DefaultValue(DefaultUseDeferredLoading)]
		public bool UseDeferredLoading {
			get { return useDeferredLoading; }
			set { useDeferredLoading = value; }
		}
		#region Obsolete 15.1
		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[Obsolete("Use the UseDeferredLoading property instead.")]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool UseDefferedLoading {
			get { return UseDeferredLoading; }
			set { UseDeferredLoading = value; }
		}
		#endregion
	}
}
