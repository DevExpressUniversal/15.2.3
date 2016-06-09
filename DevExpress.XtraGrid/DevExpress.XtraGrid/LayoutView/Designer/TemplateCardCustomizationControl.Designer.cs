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

namespace DevExpress.XtraGrid.Views.Layout.Designer {
	partial class TemplateCardCustomizationControl {
		private System.ComponentModel.IContainer components = null;
		protected override void Dispose(bool disposing) {
			if(disposing) {
				fDisposing = true;
				DoDispose();
				if(components!=null) {
					components.Dispose();
					components = null;
				}
			}
			base.Dispose(disposing);
		}
		#region Component Designer generated code
		private void InitializeComponent() {
			this.layoutControl = new DevExpress.XtraLayout.LayoutControl();
			this.layoutControlGroup1 = new DevExpress.XtraLayout.LayoutControlGroup();
			this.emptySpaceItem1 = new DevExpress.XtraLayout.EmptySpaceItem();
			this.rootGroup = new DevExpress.XtraLayout.LayoutControlGroup();
			this.customizationGroup = new DevExpress.XtraLayout.LayoutControlGroup();
			this.hiddenItemsGroup = new DevExpress.XtraLayout.LayoutControlGroup();
			this.treeViewGroup = new DevExpress.XtraLayout.LayoutControlGroup();
			this.propertyGridGroup = new DevExpress.XtraLayout.LayoutControlGroup();
			this.emptySpaceItem2 = new DevExpress.XtraLayout.EmptySpaceItem();
			((System.ComponentModel.ISupportInitialize)(this.layoutControl)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.rootGroup)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.customizationGroup)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.hiddenItemsGroup)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.treeViewGroup)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.propertyGridGroup)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem2)).BeginInit();
			this.SuspendLayout();
			this.layoutControl.AllowCustomization = false;
			this.layoutControl.Dock = System.Windows.Forms.DockStyle.Fill;
			this.layoutControl.HiddenItems.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
			this.layoutControlGroup1});
			this.layoutControl.Location = new System.Drawing.Point(0, 0);
			this.layoutControl.Name = "layoutControl";
			this.layoutControl.Root = this.rootGroup;
			this.layoutControl.Size = new System.Drawing.Size(194, 531);
			this.layoutControl.TabIndex = 0;
			this.layoutControl.Text = "layoutControl1";
			this.layoutControlGroup1.CustomizationFormText = "layoutControlGroup1";
			this.layoutControlGroup1.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
			this.emptySpaceItem1});
			this.layoutControlGroup1.Location = new System.Drawing.Point(0, 0);
			this.layoutControlGroup1.Name = "layoutControlGroup1";
			this.layoutControlGroup1.Size = new System.Drawing.Size(180, 90);
			this.layoutControlGroup1.Text = "layoutControlGroup1";
			this.emptySpaceItem1.CustomizationFormText = "emptySpaceItem1";
			this.emptySpaceItem1.Location = new System.Drawing.Point(0, 0);
			this.emptySpaceItem1.Name = "emptySpaceItem1";
			this.emptySpaceItem1.Size = new System.Drawing.Size(174, 66);
			this.emptySpaceItem1.Text = "emptySpaceItem1";
			this.emptySpaceItem1.TextSize = new System.Drawing.Size(0, 0);
			this.rootGroup.CustomizationFormText = "layoutControlGroup1";
			this.rootGroup.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
			this.customizationGroup,
			this.emptySpaceItem2});
			this.rootGroup.Location = new System.Drawing.Point(0, 0);
			this.rootGroup.Name = "layoutControlGroup1";
			this.rootGroup.Size = new System.Drawing.Size(194, 531);
			this.rootGroup.Spacing = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
			this.rootGroup.Text = "layoutControlGroup1";
			this.rootGroup.TextVisible = false;
			this.customizationGroup.CustomizationFormText = "layoutControlGroup1";
			this.customizationGroup.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
			this.hiddenItemsGroup,
			this.treeViewGroup,
			this.propertyGridGroup});
			this.customizationGroup.Location = new System.Drawing.Point(0, 0);
			this.customizationGroup.Name = "layoutControlGroup1";
			this.customizationGroup.Size = new System.Drawing.Size(192, 297);
			this.customizationGroup.Text = "Customization";
			this.hiddenItemsGroup.CustomizationFormText = "Hidden Items";
			this.hiddenItemsGroup.HeaderButtonsLocation = DevExpress.Utils.GroupElementLocation.AfterText;
			this.hiddenItemsGroup.ExpandButtonVisible = true;
			this.hiddenItemsGroup.Location = new System.Drawing.Point(0, 0);
			this.hiddenItemsGroup.Name = "layoutControlGroup1";
			this.hiddenItemsGroup.Size = new System.Drawing.Size(186, 103);
			this.hiddenItemsGroup.Text = "Hidden Items";
			this.treeViewGroup.CustomizationFormText = "Layout Tree View";
			this.treeViewGroup.HeaderButtonsLocation = DevExpress.Utils.GroupElementLocation.AfterText;
			this.treeViewGroup.ExpandButtonVisible = true;
			this.treeViewGroup.Location = new System.Drawing.Point(0, 103);
			this.treeViewGroup.Name = "layoutControlGroup2";
			this.treeViewGroup.Size = new System.Drawing.Size(186, 101);
			this.treeViewGroup.Text = "Layout Tree View";
			this.propertyGridGroup.CustomizationFormText = "Property Grid";
			this.propertyGridGroup.HeaderButtonsLocation = DevExpress.Utils.GroupElementLocation.AfterText;
			this.propertyGridGroup.ExpandButtonVisible = true;
			this.propertyGridGroup.Location = new System.Drawing.Point(0, 204);
			this.propertyGridGroup.Name = "propertyGridGroup";
			this.propertyGridGroup.Size = new System.Drawing.Size(186, 69);
			this.propertyGridGroup.Text = "Property Grid";
			this.emptySpaceItem2.CustomizationFormText = "emptySpaceItem2";
			this.emptySpaceItem2.Location = new System.Drawing.Point(0, 297);
			this.emptySpaceItem2.Name = "emptySpaceItem2";
			this.emptySpaceItem2.Size = new System.Drawing.Size(192, 232);
			this.emptySpaceItem2.Text = "emptySpaceItem2";
			this.emptySpaceItem2.TextSize = new System.Drawing.Size(0, 0);
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.layoutControl);
			this.Name = "TemplateCardCustomizationControl";
			this.Size = new System.Drawing.Size(194, 531);
			((System.ComponentModel.ISupportInitialize)(this.layoutControl)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem1)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.rootGroup)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.customizationGroup)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.hiddenItemsGroup)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.treeViewGroup)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.propertyGridGroup)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem2)).EndInit();
			this.ResumeLayout(false);
		}
		#endregion
		public DevExpress.XtraLayout.LayoutControl layoutControl;
		private DevExpress.XtraLayout.LayoutControlGroup rootGroup;
		protected DevExpress.XtraLayout.LayoutControlGroup customizationGroup;
		protected DevExpress.XtraLayout.LayoutControlGroup hiddenItemsGroup;
		protected DevExpress.XtraLayout.LayoutControlGroup treeViewGroup;
		private DevExpress.XtraLayout.EmptySpaceItem emptySpaceItem1;
		private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup1;
		private DevExpress.XtraLayout.EmptySpaceItem emptySpaceItem2;
		private DevExpress.XtraLayout.LayoutControlGroup propertyGridGroup;
	}
}
