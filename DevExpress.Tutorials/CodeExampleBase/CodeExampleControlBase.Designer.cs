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
	partial class CodeExampleControlBase {
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
			this.simpleButton1 = new DevExpress.XtraEditors.SimpleButton();
			this.rootContainer = new DevExpress.XtraEditors.PanelControl();
			this.codeTreeList = new DevExpress.XtraTreeList.TreeList();
			this.richEditControlVB = new DevExpress.XtraRichEdit.RichEditControl();
			this.richEditControlCS = new DevExpress.XtraRichEdit.RichEditControl();
			this.layoutControlGroup1 = new DevExpress.XtraLayout.LayoutControlGroup();
			this.tabbedControlGroup = new DevExpress.XtraLayout.TabbedControlGroup();
			this.richEditControlCSGroup = new DevExpress.XtraLayout.LayoutControlGroup();
			this.richEditControlCSLCI = new DevExpress.XtraLayout.LayoutControlItem();
			this.richEditControlVBGroup = new DevExpress.XtraLayout.LayoutControlGroup();
			this.richEditControlVBLCI = new DevExpress.XtraLayout.LayoutControlItem();
			this.codeTreeListLCI = new DevExpress.XtraLayout.LayoutControlItem();
			this.codeExampleName = new DevExpress.XtraLayout.SimpleLabelItem();
			this.splitterItem1 = new DevExpress.XtraLayout.SplitterItem();
			this.layoutControlItem1 = new DevExpress.XtraLayout.LayoutControlItem();
			this.layoutControlItem2 = new DevExpress.XtraLayout.LayoutControlItem();
			this.treeListColumnName = new DevExpress.XtraTreeList.Columns.TreeListColumn();
			this.treeListColumnUri = new DevExpress.XtraTreeList.Columns.TreeListColumn();
			this.treeListColumnDescription = new DevExpress.XtraTreeList.Columns.TreeListColumn();
			((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).BeginInit();
			this.layoutControl1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.rootContainer)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.codeTreeList)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.tabbedControlGroup)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.richEditControlCSGroup)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.richEditControlCSLCI)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.richEditControlVBGroup)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.richEditControlVBLCI)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.codeTreeListLCI)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.codeExampleName)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.splitterItem1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).BeginInit();
			this.SuspendLayout();
			this.layoutControl1.AllowCustomization = false;
			this.layoutControl1.Controls.Add(this.simpleButton1);
			this.layoutControl1.Controls.Add(this.rootContainer);
			this.layoutControl1.Controls.Add(this.codeTreeList);
			this.layoutControl1.Controls.Add(this.richEditControlVB);
			this.layoutControl1.Controls.Add(this.richEditControlCS);
			this.layoutControl1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.layoutControl1.Location = new System.Drawing.Point(0, 0);
			this.layoutControl1.Name = "layoutControl1";
			this.layoutControl1.OptionsCustomizationForm.DesignTimeCustomizationFormPositionAndSize = new System.Drawing.Rectangle(-1364, 48, 956, 917);
			this.layoutControl1.OptionsCustomizationForm.EnableUndoManager = false;
			this.layoutControl1.Root = this.layoutControlGroup1;
			this.layoutControl1.Size = new System.Drawing.Size(955, 510);
			this.layoutControl1.TabIndex = 1;
			this.layoutControl1.Text = "layoutControl2";
			this.simpleButton1.Location = new System.Drawing.Point(318, 12);
			this.simpleButton1.Name = "simpleButton1";
			this.simpleButton1.Size = new System.Drawing.Size(625, 33);
			this.simpleButton1.StyleController = this.layoutControl1;
			this.simpleButton1.TabIndex = 20;
			this.simpleButton1.Text = "simpleButton1";
			this.simpleButton1.Click += new System.EventHandler(this.simpleButton1_Click);
			this.rootContainer.Location = new System.Drawing.Point(12, 242);
			this.rootContainer.Name = "rootContainer";
			this.rootContainer.Size = new System.Drawing.Size(725, 256);
			this.rootContainer.TabIndex = 19;
			this.codeTreeList.Columns.AddRange(new DevExpress.XtraTreeList.Columns.TreeListColumn[] {
			this.treeListColumnName,
			this.treeListColumnUri,
			this.treeListColumnDescription});
			this.codeTreeList.Location = new System.Drawing.Point(741, 49);
			this.codeTreeList.Name = "codeTreeList";
			this.codeTreeList.Size = new System.Drawing.Size(202, 449);
			this.codeTreeList.TabIndex = 18;
			this.richEditControlVB.ActiveViewType = DevExpress.XtraRichEdit.RichEditViewType.Draft;
			this.richEditControlVB.EnableToolTips = true;
			this.richEditControlVB.Location = new System.Drawing.Point(24, 83);
			this.richEditControlVB.Name = "richEditControlVB";
			this.richEditControlVB.Options.HorizontalRuler.Visibility = DevExpress.XtraRichEdit.RichEditRulerVisibility.Hidden;
			this.richEditControlVB.Size = new System.Drawing.Size(701, 138);
			this.richEditControlVB.TabIndex = 16;
			this.richEditControlCS.ActiveViewType = DevExpress.XtraRichEdit.RichEditViewType.Draft;
			this.richEditControlCS.EnableToolTips = true;
			this.richEditControlCS.Location = new System.Drawing.Point(24, 83);
			this.richEditControlCS.Name = "richEditControlCS";
			this.richEditControlCS.Options.HorizontalRuler.Visibility = DevExpress.XtraRichEdit.RichEditRulerVisibility.Hidden;
			this.richEditControlCS.Size = new System.Drawing.Size(701, 138);
			this.richEditControlCS.TabIndex = 15;
			this.layoutControlGroup1.CustomizationFormText = "layoutControlGroup1";
			this.layoutControlGroup1.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
			this.layoutControlGroup1.GroupBordersVisible = false;
			this.layoutControlGroup1.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
			this.tabbedControlGroup,
			this.codeTreeListLCI,
			this.codeExampleName,
			this.splitterItem1,
			this.layoutControlItem1,
			this.layoutControlItem2});
			this.layoutControlGroup1.Location = new System.Drawing.Point(0, 0);
			this.layoutControlGroup1.Name = "Root";
			this.layoutControlGroup1.Size = new System.Drawing.Size(955, 510);
			this.layoutControlGroup1.TextVisible = false;
			this.tabbedControlGroup.Location = new System.Drawing.Point(0, 37);
			this.tabbedControlGroup.MultiLine = DevExpress.Utils.DefaultBoolean.True;
			this.tabbedControlGroup.Name = "tabbedControlGroup";
			this.tabbedControlGroup.SelectedTabPage = this.richEditControlCSGroup;
			this.tabbedControlGroup.SelectedTabPageIndex = 0;
			this.tabbedControlGroup.Size = new System.Drawing.Size(729, 188);
			this.tabbedControlGroup.TabPages.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
			this.richEditControlCSGroup,
			this.richEditControlVBGroup});
			this.richEditControlCSGroup.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
			this.richEditControlCSLCI});
			this.richEditControlCSGroup.Location = new System.Drawing.Point(0, 0);
			this.richEditControlCSGroup.Name = "richEditControlCSGroup";
			this.richEditControlCSGroup.Size = new System.Drawing.Size(705, 142);
			this.richEditControlCSGroup.Text = "C#";
			this.richEditControlCSLCI.Control = this.richEditControlCS;
			this.richEditControlCSLCI.Location = new System.Drawing.Point(0, 0);
			this.richEditControlCSLCI.Name = "richEditControlCSLCI";
			this.richEditControlCSLCI.Size = new System.Drawing.Size(705, 142);
			this.richEditControlCSLCI.TextSize = new System.Drawing.Size(0, 0);
			this.richEditControlCSLCI.TextVisible = false;
			this.richEditControlVBGroup.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
			this.richEditControlVBLCI});
			this.richEditControlVBGroup.Location = new System.Drawing.Point(0, 0);
			this.richEditControlVBGroup.Name = "richEditControlVBGroup";
			this.richEditControlVBGroup.Size = new System.Drawing.Size(705, 142);
			this.richEditControlVBGroup.Text = "VB";
			this.richEditControlVBLCI.Control = this.richEditControlVB;
			this.richEditControlVBLCI.Location = new System.Drawing.Point(0, 0);
			this.richEditControlVBLCI.Name = "richEditControlVBLCI";
			this.richEditControlVBLCI.Size = new System.Drawing.Size(705, 142);
			this.richEditControlVBLCI.TextSize = new System.Drawing.Size(0, 0);
			this.richEditControlVBLCI.TextVisible = false;
			this.codeTreeListLCI.Control = this.codeTreeList;
			this.codeTreeListLCI.Location = new System.Drawing.Point(729, 37);
			this.codeTreeListLCI.Name = "codeTreeListLCI";
			this.codeTreeListLCI.Size = new System.Drawing.Size(206, 453);
			this.codeTreeListLCI.TextSize = new System.Drawing.Size(0, 0);
			this.codeTreeListLCI.TextVisible = false;
			this.codeExampleName.AllowHotTrack = false;
			this.codeExampleName.AppearanceItemCaption.Font = new System.Drawing.Font("Tahoma", 20.25F);
			this.codeExampleName.AppearanceItemCaption.Options.UseFont = true;
			this.codeExampleName.Location = new System.Drawing.Point(0, 0);
			this.codeExampleName.Name = "simpleLabelItem1";
			this.codeExampleName.Size = new System.Drawing.Size(306, 37);
			this.codeExampleName.Text = "CodeExampleName";
			this.codeExampleName.TextSize = new System.Drawing.Size(229, 33);
			this.splitterItem1.AllowHotTrack = true;
			this.splitterItem1.Location = new System.Drawing.Point(0, 225);
			this.splitterItem1.Name = "splitterItem1";
			this.splitterItem1.Size = new System.Drawing.Size(729, 5);
			this.layoutControlItem1.Control = this.rootContainer;
			this.layoutControlItem1.Location = new System.Drawing.Point(0, 230);
			this.layoutControlItem1.Name = "layoutControlItem1";
			this.layoutControlItem1.Size = new System.Drawing.Size(729, 260);
			this.layoutControlItem1.TextSize = new System.Drawing.Size(0, 0);
			this.layoutControlItem1.TextVisible = false;
			this.layoutControlItem2.Control = this.simpleButton1;
			this.layoutControlItem2.Location = new System.Drawing.Point(306, 0);
			this.layoutControlItem2.MinSize = new System.Drawing.Size(82, 26);
			this.layoutControlItem2.Name = "layoutControlItem2";
			this.layoutControlItem2.Size = new System.Drawing.Size(629, 37);
			this.layoutControlItem2.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
			this.layoutControlItem2.TextSize = new System.Drawing.Size(0, 0);
			this.layoutControlItem2.TextVisible = false;
			this.treeListColumnName.Caption = "Name";
			this.treeListColumnName.FieldName = "Name";
			this.treeListColumnName.Name = "treeListColumnName";
			this.treeListColumnName.Visible = true;
			this.treeListColumnName.VisibleIndex = 0;
			this.treeListColumnUri.Caption = "Uri";
			this.treeListColumnUri.FieldName = "Uri";
			this.treeListColumnUri.Name = "treeListColumnUri";
			this.treeListColumnDescription.Caption = "Description";
			this.treeListColumnDescription.FieldName = "Description";
			this.treeListColumnDescription.Name = "treeListColumnDescription";
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.layoutControl1);
			this.Name = "CodeTutorialControlBase";
			this.Size = new System.Drawing.Size(955, 510);
			((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).EndInit();
			this.layoutControl1.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.rootContainer)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.codeTreeList)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.tabbedControlGroup)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.richEditControlCSGroup)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.richEditControlCSLCI)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.richEditControlVBGroup)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.richEditControlVBLCI)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.codeTreeListLCI)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.codeExampleName)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.splitterItem1)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).EndInit();
			this.ResumeLayout(false);
		}
		#endregion
		private DevExpress.XtraLayout.LayoutControl layoutControl1;
		private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup1;
		private XtraRichEdit.RichEditControl richEditControlVB;
		private XtraRichEdit.RichEditControl richEditControlCS;
		private DevExpress.XtraLayout.TabbedControlGroup tabbedControlGroup;
		private DevExpress.XtraLayout.LayoutControlGroup richEditControlVBGroup;
		private DevExpress.XtraLayout.LayoutControlItem richEditControlVBLCI;
		private DevExpress.XtraLayout.LayoutControlGroup richEditControlCSGroup;
		private DevExpress.XtraLayout.LayoutControlItem richEditControlCSLCI;
		private XtraTreeList.TreeList codeTreeList;
		private DevExpress.XtraLayout.LayoutControlItem codeTreeListLCI;
		private DevExpress.XtraLayout.SimpleLabelItem codeExampleName;
		private XtraLayout.SplitterItem splitterItem1;
		protected XtraEditors.PanelControl rootContainer;
		private XtraLayout.LayoutControlItem layoutControlItem1;
		private XtraEditors.SimpleButton simpleButton1;
		private XtraLayout.LayoutControlItem layoutControlItem2;
		private XtraTreeList.Columns.TreeListColumn treeListColumnName;
		private XtraTreeList.Columns.TreeListColumn treeListColumnUri;
		private XtraTreeList.Columns.TreeListColumn treeListColumnDescription;
	}
}
