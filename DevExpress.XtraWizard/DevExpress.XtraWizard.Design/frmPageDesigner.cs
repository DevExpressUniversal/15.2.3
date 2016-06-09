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
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DevExpress.Utils.Frames;
using DevExpress.XtraEditors;
namespace DevExpress.XtraWizard.Design {
	public partial class frmPageDesigner : Form {
		WizardControl control;
		BaseWizardPage selectedPage;
		bool lockControl = false;
		bool pagePositionChanged = false;
		public frmPageDesigner(WizardControl control) {
			InitializeComponent();
			this.control = control;
			this.selectedPage = control.SelectedPage;
			sbUp.Image = DevExpress.XtraEditors.Designer.Utils.XtraFrame.DesignerImages16.Images[DevExpress.XtraEditors.Designer.Utils.XtraFrame.DesignerImages16UpIndex];
			sbDown.Image = DevExpress.XtraEditors.Designer.Utils.XtraFrame.DesignerImages16.Images[DevExpress.XtraEditors.Designer.Utils.XtraFrame.DesignerImages16DownIndex];
			lbcPages.ItemHeight = lbcPages.Font.Height + 6;
		}
		public BaseWizardPage PrevSelectedPage { get { return selectedPage; } }
		private void frmPageDesigner_Load(object sender, EventArgs e) {
			if(control == null) return;
			lockControl = true;
			for(int i = 0; i < control.Pages.Count; i++) {
				lbcPages.Items.Add(new DesignerPage(control.Pages[i], lbcPages));
			}
			lbcPages.SelectedIndex = GetIndexByPage(selectedPage);
			lockControl = false;
			UpdatePageButtons();
		}
		int GetIndexByPage(BaseWizardPage selectedPage) {
			for(int i = 0; i < lbcPages.Items.Count; i++)
				if(GetPageByItem(lbcPages.Items[i]) == selectedPage) return i;
			return -1;
		}
		BaseWizardPage GetPageByItem(object item) {
			DesignerPage page = item as DesignerPage;
			if(page == null) return null;
			return page.Page;
		}
		private void lbcPages_SelectedIndexChanged(object sender, EventArgs e) {
			UpdatePageButtons();
			if(lockControl) return;
			control.SelectedPage = GetPageByItem(lbcPages.SelectedItem);
		}
		void UpdatePageButtons() {
			if(GetPageByItem(lbcPages.SelectedItem) is BaseWelcomeWizardPage)
				sbUp.Enabled = sbDown.Enabled = false;
			else {
				sbUp.Enabled = lbcPages.SelectedIndex > (control.IsWelcomePageCreated ? 1 : 0);
				sbDown.Enabled = lbcPages.SelectedIndex < lbcPages.ItemCount - (control.IsCompletionPageCreated ? 2 : 1);
			}
		}
		void MovePage(object item, int index) {
			lockControl = true;
			lbcPages.BeginUpdate();
			try {
				lbcPages.Items.Remove(item);
				lbcPages.Items.Insert(index, item);
			}
			finally {
				lbcPages.EndUpdate();
			}
			lbcPages.SelectedIndex = index;
			lockControl = false;
			pagePositionChanged = true;
		}
		public void ApplyPagePositions() {
			if(!pagePositionChanged) return;
			BaseWizardPage page = control.SelectedPage;
			control.BeginUpdate();
			try {
				control.Pages.Clear();
				for(int i = 0; i < lbcPages.Items.Count; i++)
					control.Pages.Add(GetPageByItem(lbcPages.Items[i]));
			}
			finally {
				control.EndUpdate();
			}
			control.SelectedPage = page;
		}
		private void sbUp_Click(object sender, EventArgs e) {
			MovePage(lbcPages.SelectedItem, lbcPages.SelectedIndex - 1);
		}
		private void sbDown_Click(object sender, EventArgs e) {
			MovePage(lbcPages.SelectedItem, lbcPages.SelectedIndex + 1);
		}
		class DesignerPage {
			BaseWizardPage page = null;
			ListBoxControl listBox;
			public DesignerPage(BaseWizardPage page, ListBoxControl listBox) {
				this.page = page;
				this.listBox = listBox;
			}
			public BaseWizardPage Page { get { return page; } }
			public bool IsCompletionPage { get { return Page is CompletionWizardPage; } }
			public bool IsWelcomePage { get { return Page is WelcomeWizardPage; } }
			int StartPageIndex {
				get {
					return (Owner.IsWelcomePageCreated) ? 0 : 1;
				}
			}
			WizardControl Owner { get { return page.Owner; } }
			public override string ToString() {
				if(IsCompletionPage) return string.Format("{0} (Completion Page)", page.Text);
				if(IsWelcomePage) return string.Format("{0} (Welcome Page)", page.Text);
				return string.Format("{1}. {0}", page.Text, StartPageIndex + listBox.Items.IndexOf(this));
			}
		}
	}
}
