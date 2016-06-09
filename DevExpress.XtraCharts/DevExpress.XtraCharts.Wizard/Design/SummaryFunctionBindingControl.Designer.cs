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

namespace DevExpress.XtraCharts.Design
{
	partial class SummaryFunctionBindingControl
	{
		private System.ComponentModel.IContainer components = null;
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}
		#region Component Designer generated code
		private void InitializeComponent()
		{
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SummaryFunctionBindingControl));
			this.annotationPanel = new System.Windows.Forms.Panel();
			this.label1 = new System.Windows.Forms.Label();
			this.contentPanel = new System.Windows.Forms.Panel();
			this.fakeAnnotaqtionPanel = new System.Windows.Forms.Panel();
			this.panelError = new System.Windows.Forms.Panel();
			this.panel3 = new DevExpress.XtraEditors.PanelControl();
			this.lblError = new System.Windows.Forms.Label();
			this.panelAvailable = new DevExpress.XtraEditors.PanelControl();
			this.tvAvailable = new DevExpress.XtraCharts.Design.DataMemberPicker();
			this.panelBound = new System.Windows.Forms.Panel();
			this.lvSelected = new System.Windows.Forms.ListView();
			this.columnArgumentName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.columnBoundValue = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.panelButtons = new System.Windows.Forms.Panel();
			this.btnRemoveParameter = new DevExpress.XtraEditors.SimpleButton();
			this.btnAddParameter = new DevExpress.XtraEditors.SimpleButton();
			this.panelFunctionName = new System.Windows.Forms.Panel();
			this.cbFunctionName = new DevExpress.XtraEditors.ComboBoxEdit();
			this.splitContainerControlContent = new DevExpress.XtraCharts.Wizard.SplitContainerControlWin64();
			this.annotationPanel.SuspendLayout();
			this.contentPanel.SuspendLayout();
			this.panelError.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.panel3)).BeginInit();
			this.panel3.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.panelAvailable)).BeginInit();
			this.panelAvailable.SuspendLayout();
			this.panelBound.SuspendLayout();
			this.panelButtons.SuspendLayout();
			this.panelFunctionName.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.cbFunctionName.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.splitContainerControlContent)).BeginInit();
			this.splitContainerControlContent.SuspendLayout();
			this.SuspendLayout();
			this.annotationPanel.Controls.Add(this.label1);
			resources.ApplyResources(this.annotationPanel, "annotationPanel");
			this.annotationPanel.Name = "annotationPanel";
			this.label1.BackColor = System.Drawing.Color.Transparent;
			resources.ApplyResources(this.label1, "label1");
			this.label1.ForeColor = System.Drawing.Color.Black;
			this.label1.Name = "label1";
			this.contentPanel.Controls.Add(this.splitContainerControlContent);
			resources.ApplyResources(this.contentPanel, "contentPanel");
			this.contentPanel.Name = "contentPanel";
			resources.ApplyResources(this.fakeAnnotaqtionPanel, "fakeAnnotaqtionPanel");
			this.fakeAnnotaqtionPanel.Name = "fakeAnnotaqtionPanel";
			this.panelError.Controls.Add(this.panel3);
			resources.ApplyResources(this.panelError, "panelError");
			this.panelError.Name = "panelError";
			resources.ApplyResources(this.panel3, "panel3");
			this.panel3.Controls.Add(this.lblError);
			this.panel3.Name = "panel3";
			this.panel3.TabStop = true;
			this.lblError.BackColor = System.Drawing.Color.LavenderBlush;
			resources.ApplyResources(this.lblError, "lblError");
			this.lblError.ForeColor = System.Drawing.Color.Maroon;
			this.lblError.Name = "lblError";
			resources.ApplyResources(this.panelAvailable, "panelAvailable");
			this.panelAvailable.Controls.Add(this.tvAvailable);
			this.panelAvailable.Name = "panelAvailable";
			this.panelAvailable.TabStop = true;
			resources.ApplyResources(this.tvAvailable, "tvAvailable");
			this.tvAvailable.Name = "tvAvailable";
			this.tvAvailable.SelectionChanged += new System.EventHandler(this.tvAvailable_SelectionChanged);
			this.panelBound.Controls.Add(this.lvSelected);
			this.panelBound.Controls.Add(this.panelButtons);
			this.panelBound.Controls.Add(this.panelFunctionName);
			resources.ApplyResources(this.panelBound, "panelBound");
			this.panelBound.Name = "panelBound";
			this.lvSelected.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.lvSelected.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
			this.columnArgumentName,
			this.columnBoundValue});
			resources.ApplyResources(this.lvSelected, "lvSelected");
			this.lvSelected.FullRowSelect = true;
			this.lvSelected.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
			this.lvSelected.HideSelection = false;
			this.lvSelected.MultiSelect = false;
			this.lvSelected.Name = "lvSelected";
			this.lvSelected.Scrollable = false;
			this.lvSelected.UseCompatibleStateImageBehavior = false;
			this.lvSelected.View = System.Windows.Forms.View.Details;
			this.lvSelected.SelectedIndexChanged += new System.EventHandler(this.lvSelected_SelectedIndexChanged);
			this.lvSelected.SizeChanged += new System.EventHandler(this.lvSelected_SizeChanged);
			resources.ApplyResources(this.columnArgumentName, "columnArgumentName");
			resources.ApplyResources(this.columnBoundValue, "columnBoundValue");
			this.panelButtons.Controls.Add(this.btnRemoveParameter);
			this.panelButtons.Controls.Add(this.btnAddParameter);
			resources.ApplyResources(this.panelButtons, "panelButtons");
			this.panelButtons.Name = "panelButtons";
			resources.ApplyResources(this.btnRemoveParameter, "btnRemoveParameter");
			this.btnRemoveParameter.Name = "btnRemoveParameter";
			this.btnRemoveParameter.Click += new System.EventHandler(this.btnRemoveParameter_Click);
			resources.ApplyResources(this.btnAddParameter, "btnAddParameter");
			this.btnAddParameter.Name = "btnAddParameter";
			this.btnAddParameter.Click += new System.EventHandler(this.btnAddParameter_Click);
			this.panelFunctionName.Controls.Add(this.cbFunctionName);
			resources.ApplyResources(this.panelFunctionName, "panelFunctionName");
			this.panelFunctionName.Name = "panelFunctionName";
			resources.ApplyResources(this.cbFunctionName, "cbFunctionName");
			this.cbFunctionName.Name = "cbFunctionName";
			this.cbFunctionName.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(((DevExpress.XtraEditors.Controls.ButtonPredefines)(resources.GetObject("cbFunctionName.Properties.Buttons"))))});
			this.cbFunctionName.Properties.EditValueChanged += new System.EventHandler(this.cbFunctionName_Properties_EditValueChanged);
			this.cbFunctionName.Properties.EditValueChanging += new DevExpress.XtraEditors.Controls.ChangingEventHandler(this.cbFunctionName_Properties_EditValueChanging);
			resources.ApplyResources(this.splitContainerControlContent, "splitContainerControlContent");
			this.splitContainerControlContent.Name = "splitContainerControlContent";
			this.splitContainerControlContent.Panel1.Controls.Add(this.panelBound);
			resources.ApplyResources(this.splitContainerControlContent.Panel1, "splitContainerControlContent.Panel1");
			this.splitContainerControlContent.Panel2.Controls.Add(this.panelAvailable);
			resources.ApplyResources(this.splitContainerControlContent.Panel2, "splitContainerControlContent.Panel2");
			this.splitContainerControlContent.SplitterPosition = 156;
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.contentPanel);
			this.Controls.Add(this.panelError);
			this.Controls.Add(this.fakeAnnotaqtionPanel);
			this.Controls.Add(this.annotationPanel);
			this.Name = "SummaryFunctionBindingControl";
			this.annotationPanel.ResumeLayout(false);
			this.contentPanel.ResumeLayout(false);
			this.panelError.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.panel3)).EndInit();
			this.panel3.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.panelAvailable)).EndInit();
			this.panelAvailable.ResumeLayout(false);
			this.panelBound.ResumeLayout(false);
			this.panelButtons.ResumeLayout(false);
			this.panelFunctionName.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.cbFunctionName.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.splitContainerControlContent)).EndInit();
			this.splitContainerControlContent.ResumeLayout(false);
			this.ResumeLayout(false);
		}
		#endregion
		private System.Windows.Forms.Panel annotationPanel;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Panel contentPanel;
		private System.Windows.Forms.Panel panelBound;
		private System.Windows.Forms.ListView lvSelected;
		private System.Windows.Forms.ColumnHeader columnArgumentName;
		private System.Windows.Forms.ColumnHeader columnBoundValue;
		private System.Windows.Forms.Panel panelButtons;
		private DevExpress.XtraEditors.SimpleButton btnRemoveParameter;
		private DevExpress.XtraEditors.SimpleButton btnAddParameter;
		private System.Windows.Forms.Panel panelFunctionName;
		private DevExpress.XtraEditors.ComboBoxEdit cbFunctionName;
		private System.Windows.Forms.Panel fakeAnnotaqtionPanel;
		private System.Windows.Forms.Panel panelError;
		private DevExpress.XtraEditors.PanelControl panel3;
		private System.Windows.Forms.Label lblError;
		private DevExpress.XtraEditors.PanelControl panelAvailable;
		private DataMemberPicker tvAvailable;
		private Wizard.SplitContainerControlWin64 splitContainerControlContent;
	}
}
