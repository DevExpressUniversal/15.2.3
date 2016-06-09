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
using System.Reflection;
using DevExpress.Utils;
using DevExpress.Utils.Design;
using System.ComponentModel;
using System.Windows.Forms;
using DevExpress.Utils.Frames;
using DevExpress.Utils.Controls;
namespace DevExpress.XtraDataLayout.DesignTime {
	[ToolboxItem(false)]
	public class WizardPageStep1 : LayoutBasedWizardPage {
		DataLayoutDesigner ownerCore;
		DevExpress.XtraEditors.Designer.Utils.DXPropertyGridEx pGrid;
		FilterObject filterObject;
		public WizardPageStep1(DataLayoutDesigner owner) {
			this.ownerCore = owner;
			InitTexts();
			pGrid = new DevExpress.XtraEditors.Designer.Utils.DXPropertyGridEx();
			pGrid.Dock = DockStyle.Fill;
			pGrid.ToolbarVisible = false;
			pGrid.HelpVisible = false;
			pGrid.Site = owner.Component.Site;
			pGrid.CommandsVisibleIfAvailable = false;
			pGrid.Parent = base.panelControl1;
			filterObject = new FilterObject(owner.Component, new string[] { "DataSource", "DataMember", "AllowGeneratingNestedGroups" });
			pGrid.SelectedObject = filterObject;
			SubscribePgridEvents();
			columnCountItem.Visibility = XtraLayout.Utils.LayoutVisibility.Never;
			selectAllLCI.Visibility = XtraLayout.Utils.LayoutVisibility.Never;
			useGroupNameAttributeItem.Visibility = XtraLayout.Utils.LayoutVisibility.Never;
		}
		protected override void OnParentChanged(EventArgs e) {
			base.OnParentChanged(e);
			pGrid_PropertyValueChanged(null, null);
		}
		private void InitTexts() {
			this.subtitleLabel.Text = "Specify the DataSource and DataMember (if necessary) to connect to data.\r\n\r\n\r\n";
			this.titleLabel.Appearance.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold);
			this.titleLabel.Appearance.Options.UseFont = true;
			this.titleLabel.Text = "Where will controls get their data from?";
			this.headerPicture.Image = ResourceImageHelper.CreateBitmapFromResources("DevExpress.XtraLayout.Design.DataLayout.Wizard.dataConnection.gif", Assembly.GetExecutingAssembly());
		}
		protected override bool OnSetActive() {
			Wizard.Text = "Step 1. Select Binding Source";
			return true;
		}
		protected void SubscribePgridEvents() {
			pGrid.PropertyValueChanged += new PropertyValueChangedEventHandler(pGrid_PropertyValueChanged);
		}
		protected void UnSubscribePgridEvents() {
			pGrid.PropertyValueChanged -= new PropertyValueChangedEventHandler(pGrid_PropertyValueChanged);
		}
		void pGrid_PropertyValueChanged(object s, PropertyValueChangedEventArgs e) {
			if(ownerCore.Component.DataSource == null)
				Wizard.WizardButtons = WizardButton.Cancel;
			else
				Wizard.WizardButtons = WizardButton.Next;
		}
		protected override void Dispose(bool disposing) {
			base.Dispose(disposing);
			if(disposing) UnSubscribePgridEvents();
		}
	}
}
