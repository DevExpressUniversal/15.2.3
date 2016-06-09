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
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using System.ComponentModel.Design;
using DevExpress.XtraEditors.Controls;
namespace DevExpress.XtraBars.Ribbon.Design {
	[CLSCompliant(false)]
	public partial class RibbonMiniToolbarItemsManager : ItemLinksBaseManager {
		public RibbonMiniToolbarItemsManager() {
			InitializeComponent();
		}
		public override void InitComponent() {
			base.InitComponent();
			ItemsTree.AllowGallery = true;
			ItemsTree.Location = new Point(ItemsTree.Location.X, ItemsTree.Location.Y + 28);
			ItemsTree.Height -= 28;
			FillToolbarList();
			LinksToolbar.ButtonContainer.Controls.Add(toolbarList);
		}
		private void FillToolbarList() {
			this.toolbarList.Properties.Items.Clear();
			foreach(RibbonMiniToolbar tb in Ribbon.MiniToolbars) {
				this.toolbarList.Properties.Items.Add(tb);
			}
			if(this.toolbarList.Properties.Items.Count > 0) {
				this.toolbarList.SelectedIndex = 0;
			}
		}
		protected override void OnLoad(EventArgs e) {
			base.OnLoad(e);
			if(Parent == null)
				return;
			RibbonEditorForm rf = Parent.Parent.Parent as RibbonEditorForm;
			if(rf != null && rf.ComponentObj is RibbonMiniToolbar)
				this.toolbarList.SelectedItem = rf.ComponentObj;
		}
		IDesignerHost GetHost() { return GetDesignerService(typeof(IDesignerHost)) as IDesignerHost; }
		object GetDesignerService(Type serviceType) {
			if(Ribbon == null || Ribbon.Site == null) return null;
			return Ribbon.Site.GetService(serviceType);
		}
		private void CreateNewMiniToolbar() {
			IDesignerHost host = GetHost();
			if(host != null && host.Container != null) {
				RibbonMiniToolbar toolbar = new RibbonMiniToolbar();
				Ribbon.MiniToolbars.Add(toolbar);
				host.Container.Add(toolbar);
				FillToolbarList();
			}
			this.toolbarList.SelectedIndex = this.toolbarList.Properties.Items.Count - 1;
		}
		private void toolbarList_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e) {
			if(e.Button.Kind == DevExpress.XtraEditors.Controls.ButtonPredefines.Plus) CreateNewMiniToolbar();
			else if(e.Button.Kind == DevExpress.XtraEditors.Controls.ButtonPredefines.Delete) DeleteMiniToolbar();
		}
		RibbonMiniToolbar SelectedToolbar { get { return (RibbonMiniToolbar)this.toolbarList.SelectedItem; } }
		private void DeleteMiniToolbar() {
			if(XtraMessageBox.Show(this.FindForm(), "Are you sure you want to delete the mini toolbar?", "Ribbon Control Designer", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.No)
				return;
			IDesignerHost host = GetHost();
			if(host != null && host.Container != null && SelectedToolbar != null) {
				host.Container.Remove(SelectedToolbar);
				Ribbon.MiniToolbars.Remove(SelectedToolbar);
				FillToolbarList();
			}
		}
		private void toolbarList_SelectedIndexChanged(object sender, EventArgs e) {
			this.toolbarList.Properties.Buttons[2].Enabled = this.toolbarList.SelectedItem != null;
			ItemsTree.ItemLinks = SelectedToolbar == null? null: SelectedToolbar.ItemLinks;
			ItemsTree.CreateTree();
		}
	}
}
