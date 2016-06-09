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
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;
using DevExpress.XtraEditors;
namespace DevExpress.Tutorials {
	public class FrmMainBase : XtraForm, IMessageFilter {
		public DevExpress.Utils.Frames.NotePanel8_1 pnlHint;
		public DevExpress.Utils.Frames.ApplicationCaption8_1 pnlCaption;
		public DevExpress.XtraEditors.GroupControl gcNavigations;
		public DevExpress.XtraEditors.GroupControl gcContainer;
		public PanelControl horzSplitter;
		public DevExpress.XtraEditors.GroupControl gcDescription;
		public DevExpress.LookAndFeel.DefaultLookAndFeel defaultLookAndFeel;
		private System.ComponentModel.IContainer components;
		protected DevExpress.XtraEditors.PanelControl pcMain;
		private FormTutorialInfo tutorialInfo;
		public FrmMainBase() {
			this.tutorialInfo = new FormTutorialInfo();
			InitializeComponent();
		 Application.AddMessageFilter(this);
		}
		protected override void OnResize(EventArgs e) {
			base.OnResize(e);
		}
	  protected bool fHintVisible = true;
	  public bool HintVisible {
		 get { return fHintVisible; }
		 set { fHintVisible = value; }
	  }
	  public virtual bool PreFilterMessage(ref Message m) {
		 if(m.Msg == 0x201 && fHintVisible)
			HideHint();
		 return false;
	  }
	  protected virtual void HideHint() {
	  }
		protected override void Dispose( bool disposing ) {
			if( disposing ) {
				if (components != null) {
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}
		#region Windows Form Designer generated code
		private void InitializeComponent() {
			this.components = new System.ComponentModel.Container();
			this.gcNavigations = new DevExpress.XtraEditors.GroupControl();
			this.defaultLookAndFeel = new DevExpress.LookAndFeel.DefaultLookAndFeel(this.components);
			this.pcMain = new DevExpress.XtraEditors.PanelControl();
			this.gcContainer = new DevExpress.XtraEditors.GroupControl();
			this.horzSplitter = new DevExpress.XtraEditors.PanelControl();
			this.gcDescription = new DevExpress.XtraEditors.GroupControl();
			this.pnlHint = new DevExpress.Utils.Frames.NotePanel8_1();
			this.pnlCaption = new DevExpress.Utils.Frames.ApplicationCaption8_1();
			((System.ComponentModel.ISupportInitialize)(this.gcNavigations)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.pcMain)).BeginInit();
			this.pcMain.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.gcContainer)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.horzSplitter)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.gcDescription)).BeginInit();
			this.gcDescription.SuspendLayout();
			this.SuspendLayout();
			this.gcNavigations.CaptionLocation = DevExpress.Utils.Locations.Left;
			this.gcNavigations.Dock = System.Windows.Forms.DockStyle.Left;
			this.gcNavigations.Location = new System.Drawing.Point(0, 0);
			this.gcNavigations.Name = "gcNavigations";
			this.gcNavigations.Size = new System.Drawing.Size(166, 466);
			this.gcNavigations.TabIndex = 0;
			this.gcNavigations.Text = "Tutorial Names:";
			this.pcMain.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.pcMain.Controls.Add(this.gcContainer);
			this.pcMain.Controls.Add(this.horzSplitter);
			this.pcMain.Controls.Add(this.gcDescription);
			this.pcMain.Dock = System.Windows.Forms.DockStyle.Fill;
			this.pcMain.Location = new System.Drawing.Point(166, 51);
			this.pcMain.Name = "pcMain";
			this.pcMain.Padding = new System.Windows.Forms.Padding(8);
			this.pcMain.Size = new System.Drawing.Size(546, 415);
			this.pcMain.TabIndex = 1;
			this.gcContainer.Dock = System.Windows.Forms.DockStyle.Fill;
			this.gcContainer.Location = new System.Drawing.Point(8, 8);
			this.gcContainer.Name = "gcContainer";
			this.gcContainer.ShowCaption = false;
			this.gcContainer.Size = new System.Drawing.Size(530, 354);
			this.gcContainer.TabIndex = 1;
			this.gcContainer.Text = "Tutorial:";
			this.horzSplitter.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.horzSplitter.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.horzSplitter.Location = new System.Drawing.Point(8, 362);
			this.horzSplitter.Name = "horzSplitter";
			this.horzSplitter.Size = new System.Drawing.Size(530, 8);
			this.horzSplitter.TabIndex = 7;
			this.gcDescription.Controls.Add(this.pnlHint);
			this.gcDescription.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.gcDescription.Location = new System.Drawing.Point(8, 370);
			this.gcDescription.Name = "gcDescription";
			this.gcDescription.ShowCaption = false;
			this.gcDescription.Size = new System.Drawing.Size(530, 37);
			this.gcDescription.TabIndex = 3;
			this.gcDescription.Text = "Description:";
			this.pnlHint.Dock = System.Windows.Forms.DockStyle.Fill;
			this.pnlHint.ForeColor = System.Drawing.Color.Black;
			this.pnlHint.Location = new System.Drawing.Point(2, 2);
			this.pnlHint.MaxRows = 5;
			this.pnlHint.Name = "pnlHint";
			this.pnlHint.ParentAutoHeight = true;
			this.pnlHint.Size = new System.Drawing.Size(526, 33);
			this.pnlHint.TabIndex = 0;
			this.pnlHint.TabStop = false;
			this.pnlCaption.Dock = System.Windows.Forms.DockStyle.Top;
			this.pnlCaption.Location = new System.Drawing.Point(166, 0);
			this.pnlCaption.Name = "pnlCaption";
			this.pnlCaption.Size = new System.Drawing.Size(546, 51);
			this.pnlCaption.TabIndex = 4;
			this.pnlCaption.TabStop = false;
			this.pnlCaption.Text = "Tutorials";
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(712, 466);
			this.Controls.Add(this.pcMain);
			this.Controls.Add(this.pnlCaption);
			this.Controls.Add(this.gcNavigations);
			this.Name = "FrmMainBase";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Tutorials ";
			this.Load += new System.EventHandler(this.OnLoad);
			((System.ComponentModel.ISupportInitialize)(this.gcNavigations)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.pcMain)).EndInit();
			this.pcMain.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.gcContainer)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.horzSplitter)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.gcDescription)).EndInit();
			this.gcDescription.ResumeLayout(false);
			this.ResumeLayout(false);
		}
		#endregion
		protected virtual void OnLoad(object sender, System.EventArgs e) {
		}
		[Browsable(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public FormTutorialInfo TutorialInfo { get { return tutorialInfo; } }
	}
	public enum SourceFileType {CS, VB};
	[TypeConverter(typeof(ExpandableObjectConverter))]
	public class FormTutorialInfo {
		Image imageWhatsThisButton, imageWhatsThisButtonStop;
		Image imagePatternFill;
		string sourceFileComment;
		string aboutFile;
		SourceFileType sourceFileType;
		[Browsable(true)]
		public Image ImageWhatsThisButton {
			get { return imageWhatsThisButton; }
			set { imageWhatsThisButton = value; }
		}
		[Browsable(true)]
		public Image ImageWhatsThisButtonStop {
			get { return imageWhatsThisButtonStop; }
			set { imageWhatsThisButtonStop = value; }
		}
		[Browsable(true)]
		public Image ImagePatternFill {
			get { return imagePatternFill; }
			set { imagePatternFill = value; }
		}
		[Browsable(true)]
		public string SourceFileComment {
			get { return sourceFileComment; }
			set { sourceFileComment = value; }
		}
		[Browsable(true)]
		public string AboutFile {
			get { return aboutFile; }
			set { aboutFile = value; }
		}
		[Browsable(true)]
		public SourceFileType SourceFileType {
			get { return sourceFileType; }
			set { sourceFileType = value; }
		}
	}
}
