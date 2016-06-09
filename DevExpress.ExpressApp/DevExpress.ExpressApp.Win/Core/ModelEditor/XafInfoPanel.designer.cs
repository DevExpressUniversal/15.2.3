#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       eXpressApp Framework                                        }
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

namespace DevExpress.ExpressApp.Win.Core.ModelEditor {
	partial class XafInfoPanel {
		private System.ComponentModel.IContainer components = null;
		protected override void Dispose(bool disposing) {
			if(disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}
		#region Component Designer generated code
		private void InitializeComponent() {
			this.richEditControl1 = new DevExpress.XtraRichEdit.RichEditControl();
			this.SuspendLayout();
			this.richEditControl1.ActiveViewType = DevExpress.XtraRichEdit.RichEditViewType.Simple;
			this.richEditControl1.Appearance.Text.Font = new System.Drawing.Font("Tahoma", 9.25F);
			this.richEditControl1.Appearance.Text.Options.UseFont = true;
			this.richEditControl1.AutoSizeMode = DevExpress.XtraRichEdit.AutoSizeMode.Vertical;
			this.richEditControl1.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.richEditControl1.Dock = System.Windows.Forms.DockStyle.Top;
			this.richEditControl1.EnableToolTips = true;
			this.richEditControl1.Location = new System.Drawing.Point(0, 0);
			this.richEditControl1.Name = "richEditControl1";
			this.richEditControl1.Options.Behavior.ShowPopupMenu = DevExpress.XtraRichEdit.DocumentCapability.Disabled;
			this.richEditControl1.Options.Comments.ShowAllAuthors = true;
			this.richEditControl1.Options.Comments.Visibility = DevExpress.XtraRichEdit.RichEditCommentVisibility.Auto;
			this.richEditControl1.Options.CopyPaste.MaintainDocumentSectionSettings = false;
			this.richEditControl1.Options.Fields.UseCurrentCultureDateTimeFormat = false;
			this.richEditControl1.Options.MailMerge.KeepLastParagraph = false;
			this.richEditControl1.Options.VerticalScrollbar.Visibility = DevExpress.XtraRichEdit.RichEditScrollbarVisibility.Hidden;
			this.richEditControl1.ReadOnly = true;
			this.richEditControl1.ShowCaretInReadOnly = false;
			this.richEditControl1.Size = new System.Drawing.Size(815, 19);
			this.richEditControl1.TabIndex = 0;
			this.richEditControl1.TabStop = false;
			this.richEditControl1.Text = "richEditControl1";
			this.richEditControl1.Views.SimpleView.Padding = new System.Windows.Forms.Padding(0, 4, 0, 0);
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.AutoSize = true;
			this.Controls.Add(this.richEditControl1);
			this.Name = "XafInfoPanel";
			this.Size = new System.Drawing.Size(815, 373);
			this.ResumeLayout(false);
		}
		#endregion
		private DevExpress.XtraRichEdit.RichEditControl richEditControl1;
	}
}
