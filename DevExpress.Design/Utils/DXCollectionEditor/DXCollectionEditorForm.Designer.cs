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

using System.ComponentModel;
namespace DevExpress.Utils.Design {
	partial class DXCollectionEditorForm {
		private System.ComponentModel.IContainer components = null;
		protected override void Dispose(bool disposing) {
			UnregisterContentEvents();
			if(disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}
		private void UnregisterContentEvents() {
			if(collectionEditorContent != null) {
				this.collectionEditorContent.ItemChanged -= new DevExpress.Utils.Design.Internal.PropertyItemChangedEventHandler(this.CollectionEditorContent_ItemChanged);
				this.collectionEditorContent.QueryCustomDisplayText -= new DevExpress.Utils.Design.Internal.QueryCustomDisplayTextEventHandler(this.СollectionEditorContent_GetCustomDisplayText);
				this.collectionEditorContent.QueryNewItem -= new DevExpress.Utils.Design.Internal.QueryNewItemEventHandler(this.CollectionEditorContent_QueryNewItem);
				this.collectionEditorContent.CollectionChanged -= new DevExpress.Utils.Design.Internal.CollectionChangedEventHandler(this.CollectionEditorContent_CollectionChanged);
				this.collectionEditorContent.CollectionChanging -= new DevExpress.Utils.Design.Internal.CollectionChangingEventHandler(this.CollectionEditorContent_CollectionChanging);
			}
		}
		#region Windows Form Designer generated code
		private void InitializeComponent() {
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DXCollectionEditorForm));
			this.cancelButton = new DevExpress.XtraEditors.SimpleButton();
			this.stackPanel = new DevExpress.XtraEditors.Internal.StackPanelControl();
			this.okButton = new DevExpress.XtraEditors.SimpleButton();
			((System.ComponentModel.ISupportInitialize)(this.stackPanel)).BeginInit();
			this.stackPanel.SuspendLayout();
			this.SuspendLayout();
			resources.ApplyResources(this.cancelButton, "cancelButton");
			this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.cancelButton.Name = "cancelButton";
			this.stackPanel.AllowFixedSide = false;
			this.stackPanel.ContentAlignment = HorzAlignment.Far;
			this.stackPanel.Controls.Add(this.okButton);
			this.stackPanel.Controls.Add(this.cancelButton);
			resources.ApplyResources(this.stackPanel, "stackPanel");
			this.stackPanel.ItemIndent = 8;
			this.stackPanel.Name = "stackPanel";
			resources.ApplyResources(this.okButton, "okButton");
			this.okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.okButton.Name = "okButton";
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
			this.CancelButton = this.cancelButton;
			resources.ApplyResources(this, "$this");
			this.Controls.Add(this.stackPanel);
			this.HelpButton = true;
			this.LookAndFeel.UseDefaultLookAndFeel = false;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "DXCollectionEditorForm";
			this.Load += new System.EventHandler(this.XtraCollectionForm_Load);
			((System.ComponentModel.ISupportInitialize)(this.stackPanel)).EndInit();
			this.stackPanel.ResumeLayout(false);
			this.ResumeLayout(false);
			this.PerformLayout();
		}
		#endregion
		private XtraEditors.SimpleButton cancelButton;
		private XtraEditors.SimpleButton okButton;
		private DevExpress.XtraEditors.Internal.StackPanelControl stackPanel;
	}
}
