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

namespace DevExpress.DXperience.Demos.CodeDemo {
	partial class CodeTutorialControlBase {
		private System.ComponentModel.IContainer components = null;
		protected override void Dispose(bool disposing) {
			if(disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}
		#region Component Designer generated code
		private void InitializeComponent() {
			this.layoutControl1 = new DevExpress.XtraLayout.LayoutControl();
			this.layoutControlForExampleCode = new DevExpress.XtraLayout.LayoutControl();
			this.Root = new DevExpress.XtraLayout.LayoutControlGroup();
			this.rootContainer = new DevExpress.XtraEditors.XtraUserControl();
			this.codeTreeList = new DevExpress.XtraTreeList.TreeList();
			this.layoutControlGroup1 = new DevExpress.XtraLayout.LayoutControlGroup();
			this.codeTreeListLCI = new DevExpress.XtraLayout.LayoutControlItem();
			this.splitterItem1 = new DevExpress.XtraLayout.SplitterItem();
			this.layoutControlItem1 = new DevExpress.XtraLayout.LayoutControlItem();
			this.splitterItem2 = new DevExpress.XtraLayout.SplitterItem();
			this.itemForLayoutControl = new DevExpress.XtraLayout.LayoutControlItem();
			((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).BeginInit();
			this.layoutControl1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlForExampleCode)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.Root)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.codeTreeList)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.codeTreeListLCI)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.splitterItem1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.splitterItem2)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.itemForLayoutControl)).BeginInit();
			this.SuspendLayout();
			this.layoutControl1.AllowCustomization = false;
			this.layoutControl1.Controls.Add(this.layoutControlForExampleCode);
			this.layoutControl1.Controls.Add(this.rootContainer);
			this.layoutControl1.Controls.Add(this.codeTreeList);
			this.layoutControl1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.layoutControl1.Location = new System.Drawing.Point(0, 0);
			this.layoutControl1.Name = "layoutControl1";
			this.layoutControl1.OptionsCustomizationForm.DesignTimeCustomizationFormPositionAndSize = new System.Drawing.Rectangle(156, 68, 956, 917);
			this.layoutControl1.OptionsCustomizationForm.EnableUndoManager = false;
			this.layoutControl1.Root = this.layoutControlGroup1;
			this.layoutControl1.Size = new System.Drawing.Size(955, 587);
			this.layoutControl1.TabIndex = 1;
			this.layoutControl1.Text = "layoutControl2";
			this.layoutControlForExampleCode.AllowCustomization = false;
			this.layoutControlForExampleCode.Location = new System.Drawing.Point(12, 298);
			this.layoutControlForExampleCode.Name = "layoutControlForExampleCode";
			this.layoutControlForExampleCode.Root = this.Root;
			this.layoutControlForExampleCode.Size = new System.Drawing.Size(931, 277);
			this.layoutControlForExampleCode.TabIndex = 20;
			this.layoutControlForExampleCode.Text = "layoutControl2";
			this.Root.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
			this.Root.GroupBordersVisible = false;
			this.Root.Location = new System.Drawing.Point(0, 0);
			this.Root.Name = "Root";
			this.Root.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
			this.Root.Size = new System.Drawing.Size(931, 277);
			this.Root.TextVisible = false;
			this.rootContainer.Location = new System.Drawing.Point(318, 12);
			this.rootContainer.Name = "rootContainer";
			this.rootContainer.Size = new System.Drawing.Size(625, 277);
			this.rootContainer.TabIndex = 19;
			this.codeTreeList.Location = new System.Drawing.Point(12, 12);
			this.codeTreeList.Name = "codeTreeList";
			this.codeTreeList.OptionsSelection.EnableAppearanceFocusedCell = false;
			this.codeTreeList.OptionsView.AllowGlyphSkinning = true;
			this.codeTreeList.OptionsView.AnimationType = DevExpress.XtraTreeList.TreeListAnimationType.AnimateFocusedNode;
			this.codeTreeList.OptionsView.FocusRectStyle = DevExpress.XtraTreeList.DrawFocusRectStyle.None;
			this.codeTreeList.OptionsView.ShowHorzLines = false;
			this.codeTreeList.OptionsView.ShowIndicator = false;
			this.codeTreeList.OptionsView.ShowVertLines = false;
			this.codeTreeList.Size = new System.Drawing.Size(297, 277);
			this.codeTreeList.TabIndex = 18;
			this.codeTreeList.TreeLineStyle = DevExpress.XtraTreeList.LineStyle.None;
			this.layoutControlGroup1.CustomizationFormText = "layoutControlGroup1";
			this.layoutControlGroup1.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
			this.layoutControlGroup1.GroupBordersVisible = false;
			this.layoutControlGroup1.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
			this.codeTreeListLCI,
			this.splitterItem1,
			this.layoutControlItem1,
			this.splitterItem2,
			this.itemForLayoutControl});
			this.layoutControlGroup1.Location = new System.Drawing.Point(0, 0);
			this.layoutControlGroup1.Name = "Root";
			this.layoutControlGroup1.Size = new System.Drawing.Size(955, 587);
			this.layoutControlGroup1.TextVisible = false;
			this.codeTreeListLCI.Control = this.codeTreeList;
			this.codeTreeListLCI.Location = new System.Drawing.Point(0, 0);
			this.codeTreeListLCI.Name = "codeTreeListLCI";
			this.codeTreeListLCI.Size = new System.Drawing.Size(301, 281);
			this.codeTreeListLCI.TextSize = new System.Drawing.Size(0, 0);
			this.codeTreeListLCI.TextVisible = false;
			this.splitterItem1.AllowHotTrack = true;
			this.splitterItem1.Location = new System.Drawing.Point(0, 281);
			this.splitterItem1.Name = "splitterItem1";
			this.splitterItem1.Size = new System.Drawing.Size(935, 5);
			this.layoutControlItem1.Control = this.rootContainer;
			this.layoutControlItem1.Location = new System.Drawing.Point(306, 0);
			this.layoutControlItem1.MinSize = new System.Drawing.Size(100, 100);
			this.layoutControlItem1.Name = "layoutControlItem1";
			this.layoutControlItem1.Size = new System.Drawing.Size(629, 281);
			this.layoutControlItem1.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
			this.layoutControlItem1.TextSize = new System.Drawing.Size(0, 0);
			this.layoutControlItem1.TextVisible = false;
			this.splitterItem2.AllowHotTrack = true;
			this.splitterItem2.Location = new System.Drawing.Point(301, 0);
			this.splitterItem2.Name = "splitterItem2";
			this.splitterItem2.Size = new System.Drawing.Size(5, 281);
			this.itemForLayoutControl.Control = this.layoutControlForExampleCode;
			this.itemForLayoutControl.Location = new System.Drawing.Point(0, 286);
			this.itemForLayoutControl.Name = "itemForLayoutControl";
			this.itemForLayoutControl.Size = new System.Drawing.Size(935, 281);
			this.itemForLayoutControl.TextSize = new System.Drawing.Size(0, 0);
			this.itemForLayoutControl.TextVisible = false;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.layoutControl1);
			this.Name = "CodeTutorialControlBase";
			this.Size = new System.Drawing.Size(955, 587);
			((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).EndInit();
			this.layoutControl1.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.layoutControlForExampleCode)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.Root)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.codeTreeList)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.codeTreeListLCI)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.splitterItem1)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.splitterItem2)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.itemForLayoutControl)).EndInit();
			this.ResumeLayout(false);
		}
		#endregion
		private DevExpress.XtraLayout.LayoutControl layoutControl1;
		private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup1;
		private XtraTreeList.TreeList codeTreeList;
		private DevExpress.XtraLayout.LayoutControlItem codeTreeListLCI;
		private XtraLayout.SplitterItem splitterItem1;
		protected XtraEditors.XtraUserControl rootContainer;
		private XtraLayout.SplitterItem splitterItem2;
		private XtraLayout.LayoutControlItem layoutControlItem1;
		private XtraLayout.LayoutControl layoutControlForExampleCode;
		private XtraLayout.LayoutControlGroup Root;
		private XtraLayout.LayoutControlItem itemForLayoutControl;
	}
}
