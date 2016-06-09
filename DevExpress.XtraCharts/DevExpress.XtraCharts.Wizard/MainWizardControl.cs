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
using DevExpress.XtraCharts.Native;
using DevExpress.XtraNavBar;
using DevExpress.Utils.Frames;
using DevExpress.XtraEditors.DXErrorProvider;
using DevExpress.XtraNavBar.ViewInfo;
namespace DevExpress.XtraCharts.Wizard {
	internal partial class MainWizardControl : ChartUserControl {
		bool loading = true;
		WizardFormBase parentForm;
		IParentControl parentControl;
		NavBarItemLink linkToRestore;
		public IParentControl ParentControl { get { return parentControl; } }
		public MainWizardControl() {
			InitializeComponent();
		}
		void GeneratePages(IList<WizardPage> pageData) {
			this.parentControl = new PanelContainer(this);
			nbWizard.Items.Clear();
			nbWizard.Groups.Clear();
			foreach (WizardPage data in pageData) {
				NavBarItemContainer container = new NavBarItemContainer(data, this);
				data.LabelControl = container;
				data.ParentControl = this.parentControl;
				nbWizard.Items.Add(container.NavBarItem);
				NavBarGroup group = nbWizard.Groups[data.Group.Name];
				if (group == null) {
					group = new NavBarGroup(data.Group.Name);
					group.Expanded = true;
					group.Name = data.Group.Name;
					nbWizard.Groups.Add(group);
				}
				group.ItemLinks.Add(container.NavBarItem);
			}
		}
		void nbWizard_LinkPressed(object sender, NavBarLinkEventArgs e) {
			NavBarControl navBar = e.Link.NavBar;
			linkToRestore = e.Link;
			if (!parentForm.SelectCustomPage(navBar.Items.IndexOf(e.Link.Item)))
				linkToRestore = navBar.SelectedLink;
		}
		void nbWizard_SelectedLinkChanged(object sender, NavBarSelectedLinkChangedEventArgs e) {
			if (!loading && linkToRestore != null) {
				NavBarControl navBar = e.Link.NavBar;
				if (linkToRestore != navBar.SelectedLink)
					navBar.SelectedLink = linkToRestore;
				linkToRestore = null;
			}
		}
		public void Initialize(WizardFormBase parentForm, IList<WizardPage> pageData) {
			this.parentForm = parentForm;
			GeneratePages(pageData);
			loading = false;
		}
	}
	internal class NavBarItemContainer : ILabelControl {
		WizardPage pageData;
		NavBarItem item;
		public NavBarItem NavBarItem { get { return item; } }
		#region ILabelControl Members
		public string Text { get { return this.pageData.Label; } }
		public Image Image { get { return this.pageData.Image; } }
		public void Highlight() {
			foreach (NavBarItemLink link in item.Links)
				link.Group.SelectedLink = link;
		}
		#endregion
		public NavBarItemContainer(WizardPage data, MainWizardControl wizardControl) {
			this.pageData = data;
			this.item = new NavBarItem(data.Label);
			this.item.LargeImage = data.Image;
			this.item.SmallImage = data.Image;
		}
	}
	internal class PanelContainer : IParentControl {
		MainWizardControl wizardControl;
		public PanelContainer(MainWizardControl wizardControl) {
			this.wizardControl = wizardControl;
		}
		#region IParentControl Members
		public Control Control {
			get { return this.wizardControl.pnlParent; }
		}
		public void SetDescription(string description) {
			this.wizardControl.pnlHint.Text = description;
			this.wizardControl.pnlHint.Visible = description != string.Empty;
			this.wizardControl.pnlHintOffset.Visible = description != string.Empty;
		}
		public void SetHeader(string header) {
			wizardControl.panelCaption.Visible = !String.IsNullOrEmpty(header);
			wizardControl.labelCaption.Text = header;
		}
		#endregion
	}
}
