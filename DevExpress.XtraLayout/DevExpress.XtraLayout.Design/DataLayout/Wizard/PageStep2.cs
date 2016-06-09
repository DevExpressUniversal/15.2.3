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

using System.Linq;
using System.Reflection;
using System.ComponentModel;
using DevExpress.Utils;
namespace DevExpress.XtraDataLayout.DesignTime {
	[ToolboxItem(false)]
	public class WizardPageStep2 :LayoutBasedWizardPage {
		DataLayoutDesigner ownerCore;
		DataSourceStructureView structureView;
		LayoutElementsBindingInfo info;
		public WizardPageStep2(DataLayoutDesigner owner) {
			ownerCore = owner;
			InitTexts();
			structureView = new DataSourceStructureView();
			structureView.Dock = System.Windows.Forms.DockStyle.Fill;
			structureView.Parent = panelControl1;
			selectAllLCI.Visibility = XtraLayout.Utils.LayoutVisibility.Always;
		}
		private void InitTexts() {
			this.subtitleLabel.Text = "Check data source fields that will be edited in the DataLayoutControl. For each f" +
			  "ield, specify the editor and bindable property.";
			this.titleLabel.Appearance.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold);
			this.titleLabel.Appearance.Options.UseFont = true;
			this.titleLabel.Text = "Which data source fields must be edited, and which editors must be used to edit t" +
				"hem?";
			this.headerPicture.Image = ResourceImageHelper.CreateBitmapFromResources("DevExpress.XtraLayout.Design.DataLayout.Wizard.feilds2.gif", Assembly.GetExecutingAssembly());
		}
		protected override bool OnSetActive() {
			Wizard.WizardButtons = WizardButton.Next | WizardButton.Back;
			Wizard.Text = "Step 2. Manage Data Bindings";
			RefreshInfo();
			if(info.GetAllBindings().Any(e => e.InnerLayoutElementsBindingInfo != null)) {
				columnCountItem.Visibility = XtraLayout.Utils.LayoutVisibility.Never;
				useGroupNameAttributeItem.Visibility = XtraLayout.Utils.LayoutVisibility.Never;
			} else {
				columnCountItem.Visibility = XtraLayout.Utils.LayoutVisibility.Always;
				useGroupNameAttributeItem.Visibility = XtraLayout.Utils.LayoutVisibility.Always;
			}
			return true;
		}
		protected void RefreshInfo() {
			LayoutElementsBindingInfoHelper bindingHelper = new LayoutElementsBindingInfoHelper(ownerCore.Component);
			info = bindingHelper.CreateDataLayoutElementsBindingInfo();
			bindingHelper.FillWithSuggestedValues(info);
			bindingHelper.CorrectLayoutElementsBindingInfo(info);
			structureView.EnsureInfo(info);
			structureView.EnsureBindingManager(new BindingMenuManager(ownerCore.Component.ControlsManager));
			structureView.UpdateView();
		}
		protected override bool OnWizardFinish() {
			ownerCore.UpdateLayout(info, info.ColumnCount > 1 && useFlowGenerationCheckEdit.Checked);
			return base.OnWizardFinish();
		}
		protected override void columnCountSpinEdit_EditValueChanged(object sender, System.EventArgs e) {
			info.ColumnCount = (int)columnCountSpinEdit.Value;
			if(info.ColumnCount > 1) useFlowGenerationCheckEdit.Checked = true;
			else useFlowGenerationCheckEdit.Checked = false;
		}
		protected override void selectAllCheckEdit_CheckedChanged(object sender, System.EventArgs e) {
			if(info == null || info.ColumnCount == 0) return;
			selectAllCore(info);
			structureView.Refresh();
		}
		private void selectAllCore(LayoutElementsBindingInfo layoutElementsBindingInfo) {
			foreach(var binding in layoutElementsBindingInfo.GetAllBindings()) {
				if(binding.InnerLayoutElementsBindingInfo != null) {
					selectAllCore(binding.InnerLayoutElementsBindingInfo);
					continue;
				}
				binding.Visible = selectAllCheckEdit.Checked;
			}
		}
	}
}
