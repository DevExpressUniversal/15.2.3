#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       Dashboard                                                   }
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
using System.Windows.Forms;
using DevExpress.DataAccess.Native;
using DevExpress.LookAndFeel;
using DevExpress.XtraLayout;
namespace DevExpress.DashboardWin.Native {
	public partial class FormatRuleFormBase : DashboardForm, IFormatRuleControlChanger {	   
		readonly Locker locker = new Locker();
		event EventHandler changed;
		event EventHandler created;
		event EventHandler destroyed;
		public FormatRuleFormBase() {
			InitializeComponent();
		}
		public FormatRuleFormBase(UserLookAndFeel lookAndFeel, Control control)
			: this(lookAndFeel, control, false) {
		}
		public FormatRuleFormBase(UserLookAndFeel lookAndFeel, Control control, bool closeOnly)
			: base(lookAndFeel) {
			InitializeComponent();
			locker.Lock();
			layoutControl.BeginUpdate();
			try {
				if(closeOnly) {
					lciClose.Visibility = XtraLayout.Utils.LayoutVisibility.Always;
					lciOK.Visibility = XtraLayout.Utils.LayoutVisibility.OnlyInCustomization;
					lciApply.Visibility = XtraLayout.Utils.LayoutVisibility.OnlyInCustomization;
					lciCancel.Visibility = XtraLayout.Utils.LayoutVisibility.OnlyInCustomization;
					this.AcceptButton = this.btnClose;
					this.CancelButton = this.btnClose;
				} else {
					lciClose.Visibility = XtraLayout.Utils.LayoutVisibility.OnlyInCustomization;
					lciOK.Visibility = XtraLayout.Utils.LayoutVisibility.Always;
					lciApply.Visibility = XtraLayout.Utils.LayoutVisibility.Always;
					lciCancel.Visibility = XtraLayout.Utils.LayoutVisibility.Always;
					this.AcceptButton = this.btnOK;
					this.CancelButton = this.btnCancel;
				}
				LayoutControlItem item = new LayoutControlItem();
				item.Control = control;
				item.TextVisible = false;
				lcgUserControl.AddItem(item);
			} finally {
				layoutControl.EndUpdate();
				locker.Unlock();
			}
#if DEBUGTEST
			layoutControl.AllowCustomization = true;
#endif
		}
		protected override void DisposeInternal(bool disposing) {
			base.DisposeInternal(disposing);
			if(disposing) {
			}
		}
		protected override void OnLoad(EventArgs e) {
			base.OnLoad(e);
			RaiseCreated();
		}
		void RaiseCreated() {
			if(created != null)
				created(this, EventArgs.Empty);
		}
		protected override void OnClosed(EventArgs e) {
			base.OnClosed(e);
			RaiseDestroyed();
		}
		void RaiseDestroyed() {
			if(destroyed != null)
				destroyed(this, EventArgs.Empty);
		}
		void OnBtnOKClick(object sender, EventArgs e) {
			DialogResult = DialogResult.OK;
			if(btnApply.Enabled)
				RaiseChanged();
		}
		void OnBtnCancelClick(object sender, EventArgs e) {
			DialogResult = DialogResult.Cancel;
		}
		void OnBtnApplyClick(object sender, EventArgs e) {
			RaiseChanged();
		}
		void RaiseChanged() {
			if(changed != null)
				changed(this, EventArgs.Empty);
		}
		#region IFormatRuleControlChanger Members
		event EventHandler IFormatRuleControlChanger.Changed {
			add { changed += value; }
			remove { changed -= value; }
		}
		event EventHandler IFormatRuleControlChanger.Created {
			add { created += value; }
			remove { created -= value; }
		}
		event EventHandler IFormatRuleControlChanger.Destroyed {
			add { destroyed += value; }
			remove { destroyed -= value; }
		}
		void IFormatRuleControlChanger.Enable(bool? enabled) {
			if(enabled.HasValue) {
				btnOK.Enabled = true;
				btnApply.Enabled = enabled.Value;
			} else {
				btnOK.Enabled = btnApply.Enabled = false;
			}
		}
		void IFormatRuleControlChanger.Refresh(string title) {
			Text = title;
		}
		#endregion
	}
}
