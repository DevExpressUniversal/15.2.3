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

namespace DevExpress.XtraPdfViewer.Forms
{
	partial class PdfDocumentPropertiesForm
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
		#region Windows Form Designer generated code
		private void InitializeComponent()
		{
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PdfDocumentPropertiesForm));
			this.btnOk = new DevExpress.XtraEditors.SimpleButton();
			this.lblDescription = new DevExpress.XtraEditors.LabelControl();
			this.lblFile = new DevExpress.XtraEditors.LabelControl();
			this.lblTitle = new DevExpress.XtraEditors.LabelControl();
			this.lblAuthor = new DevExpress.XtraEditors.LabelControl();
			this.lblSubject = new DevExpress.XtraEditors.LabelControl();
			this.lblKeywords = new DevExpress.XtraEditors.LabelControl();
			this.lblKeywordsText = new DevExpress.XtraEditors.LabelControl();
			this.lblProducer = new DevExpress.XtraEditors.LabelControl();
			this.lblVersion = new DevExpress.XtraEditors.LabelControl();
			this.lblLocation = new DevExpress.XtraEditors.LabelControl();
			this.lblFileSize = new DevExpress.XtraEditors.LabelControl();
			this.lblNumberOfPages = new DevExpress.XtraEditors.LabelControl();
			this.lblCreated = new DevExpress.XtraEditors.LabelControl();
			this.lblModified = new DevExpress.XtraEditors.LabelControl();
			this.lblApplication = new DevExpress.XtraEditors.LabelControl();
			this.lblNumberOfPagesText = new DevExpress.XtraEditors.LabelControl();
			this.lblFileSizeText = new DevExpress.XtraEditors.LabelControl();
			this.lblLocationText = new DevExpress.XtraEditors.LabelControl();
			this.lblVersionText = new DevExpress.XtraEditors.LabelControl();
			this.lblProducerText = new DevExpress.XtraEditors.LabelControl();
			this.lblApplicationText = new DevExpress.XtraEditors.LabelControl();
			this.lblModifiedText = new DevExpress.XtraEditors.LabelControl();
			this.lblCreatedText = new DevExpress.XtraEditors.LabelControl();
			this.lblSubjectText = new DevExpress.XtraEditors.LabelControl();
			this.lblAuthorText = new DevExpress.XtraEditors.LabelControl();
			this.lblTitleText = new DevExpress.XtraEditors.LabelControl();
			this.lblFileText = new DevExpress.XtraEditors.LabelControl();
			this.lblRevision = new DevExpress.XtraEditors.LabelControl();
			this.lblAdvanced = new DevExpress.XtraEditors.LabelControl();
			this.lblPageSizeText = new DevExpress.XtraEditors.LabelControl();
			this.lblPageSize = new DevExpress.XtraEditors.LabelControl();
			this.SuspendLayout();
			this.btnOk.DialogResult = System.Windows.Forms.DialogResult.OK;
			resources.ApplyResources(this.btnOk, "btnOk");
			this.btnOk.Name = "btnOk";
			this.lblDescription.Appearance.Font = ((System.Drawing.Font)(resources.GetObject("lblDescription.Appearance.Font")));
			resources.ApplyResources(this.lblDescription, "lblDescription");
			this.lblDescription.Name = "lblDescription";
			resources.ApplyResources(this.lblFile, "lblFile");
			this.lblFile.Name = "lblFile";
			resources.ApplyResources(this.lblTitle, "lblTitle");
			this.lblTitle.Name = "lblTitle";
			resources.ApplyResources(this.lblAuthor, "lblAuthor");
			this.lblAuthor.Name = "lblAuthor";
			resources.ApplyResources(this.lblSubject, "lblSubject");
			this.lblSubject.Name = "lblSubject";
			resources.ApplyResources(this.lblKeywords, "lblKeywords");
			this.lblKeywords.Name = "lblKeywords";
			this.lblKeywordsText.Appearance.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Top;
			this.lblKeywordsText.Appearance.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
			resources.ApplyResources(this.lblKeywordsText, "lblKeywordsText");
			this.lblKeywordsText.Name = "lblKeywordsText";
			resources.ApplyResources(this.lblProducer, "lblProducer");
			this.lblProducer.Name = "lblProducer";
			resources.ApplyResources(this.lblVersion, "lblVersion");
			this.lblVersion.Name = "lblVersion";
			resources.ApplyResources(this.lblLocation, "lblLocation");
			this.lblLocation.Name = "lblLocation";
			resources.ApplyResources(this.lblFileSize, "lblFileSize");
			this.lblFileSize.Name = "lblFileSize";
			resources.ApplyResources(this.lblNumberOfPages, "lblNumberOfPages");
			this.lblNumberOfPages.Name = "lblNumberOfPages";
			resources.ApplyResources(this.lblCreated, "lblCreated");
			this.lblCreated.Name = "lblCreated";
			resources.ApplyResources(this.lblModified, "lblModified");
			this.lblModified.Name = "lblModified";
			resources.ApplyResources(this.lblApplication, "lblApplication");
			this.lblApplication.Name = "lblApplication";
			this.lblNumberOfPagesText.Appearance.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
			resources.ApplyResources(this.lblNumberOfPagesText, "lblNumberOfPagesText");
			this.lblNumberOfPagesText.Name = "lblNumberOfPagesText";
			this.lblFileSizeText.Appearance.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
			resources.ApplyResources(this.lblFileSizeText, "lblFileSizeText");
			this.lblFileSizeText.Name = "lblFileSizeText";
			this.lblLocationText.Appearance.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
			resources.ApplyResources(this.lblLocationText, "lblLocationText");
			this.lblLocationText.Name = "lblLocationText";
			this.lblVersionText.Appearance.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
			resources.ApplyResources(this.lblVersionText, "lblVersionText");
			this.lblVersionText.Name = "lblVersionText";
			this.lblProducerText.Appearance.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
			resources.ApplyResources(this.lblProducerText, "lblProducerText");
			this.lblProducerText.Name = "lblProducerText";
			this.lblApplicationText.Appearance.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
			resources.ApplyResources(this.lblApplicationText, "lblApplicationText");
			this.lblApplicationText.Name = "lblApplicationText";
			this.lblModifiedText.Appearance.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
			resources.ApplyResources(this.lblModifiedText, "lblModifiedText");
			this.lblModifiedText.Name = "lblModifiedText";
			this.lblCreatedText.Appearance.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
			resources.ApplyResources(this.lblCreatedText, "lblCreatedText");
			this.lblCreatedText.Name = "lblCreatedText";
			this.lblSubjectText.Appearance.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Top;
			this.lblSubjectText.Appearance.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
			resources.ApplyResources(this.lblSubjectText, "lblSubjectText");
			this.lblSubjectText.Name = "lblSubjectText";
			this.lblAuthorText.Appearance.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
			resources.ApplyResources(this.lblAuthorText, "lblAuthorText");
			this.lblAuthorText.Name = "lblAuthorText";
			this.lblTitleText.Appearance.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
			resources.ApplyResources(this.lblTitleText, "lblTitleText");
			this.lblTitleText.Name = "lblTitleText";
			this.lblFileText.Appearance.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
			resources.ApplyResources(this.lblFileText, "lblFileText");
			this.lblFileText.Name = "lblFileText";
			this.lblRevision.Appearance.Font = ((System.Drawing.Font)(resources.GetObject("lblRevision.Appearance.Font")));
			resources.ApplyResources(this.lblRevision, "lblRevision");
			this.lblRevision.Name = "lblRevision";
			this.lblAdvanced.Appearance.Font = ((System.Drawing.Font)(resources.GetObject("lblAdvanced.Appearance.Font")));
			resources.ApplyResources(this.lblAdvanced, "lblAdvanced");
			this.lblAdvanced.Name = "lblAdvanced";
			this.lblPageSizeText.Appearance.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
			resources.ApplyResources(this.lblPageSizeText, "lblPageSizeText");
			this.lblPageSizeText.Name = "lblPageSizeText";
			resources.ApplyResources(this.lblPageSize, "lblPageSize");
			this.lblPageSize.Name = "lblPageSize";
			this.AcceptButton = this.btnOk;
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.btnOk;
			this.ControlBox = false;
			this.Controls.Add(this.lblPageSizeText);
			this.Controls.Add(this.lblPageSize);
			this.Controls.Add(this.lblFileText);
			this.Controls.Add(this.lblTitleText);
			this.Controls.Add(this.lblAuthorText);
			this.Controls.Add(this.lblSubjectText);
			this.Controls.Add(this.lblCreatedText);
			this.Controls.Add(this.lblModifiedText);
			this.Controls.Add(this.lblApplicationText);
			this.Controls.Add(this.lblProducerText);
			this.Controls.Add(this.lblVersionText);
			this.Controls.Add(this.lblLocationText);
			this.Controls.Add(this.lblFileSizeText);
			this.Controls.Add(this.lblNumberOfPagesText);
			this.Controls.Add(this.lblApplication);
			this.Controls.Add(this.lblModified);
			this.Controls.Add(this.lblCreated);
			this.Controls.Add(this.lblNumberOfPages);
			this.Controls.Add(this.lblFileSize);
			this.Controls.Add(this.lblLocation);
			this.Controls.Add(this.lblVersion);
			this.Controls.Add(this.lblProducer);
			this.Controls.Add(this.lblKeywordsText);
			this.Controls.Add(this.lblKeywords);
			this.Controls.Add(this.lblSubject);
			this.Controls.Add(this.lblAuthor);
			this.Controls.Add(this.lblTitle);
			this.Controls.Add(this.lblFile);
			this.Controls.Add(this.lblAdvanced);
			this.Controls.Add(this.lblRevision);
			this.Controls.Add(this.lblDescription);
			this.Controls.Add(this.btnOk);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "PdfDocumentPropertiesForm";
			this.ShowIcon = false;
			this.ShowInTaskbar = false;
			this.ResumeLayout(false);
			this.PerformLayout();
		}
		#endregion
		private DevExpress.XtraEditors.SimpleButton btnOk;
		private DevExpress.XtraEditors.LabelControl lblDescription;
		private DevExpress.XtraEditors.LabelControl lblFile;
		private DevExpress.XtraEditors.LabelControl lblTitle;
		private DevExpress.XtraEditors.LabelControl lblAuthor;
		private DevExpress.XtraEditors.LabelControl lblSubject;
		private DevExpress.XtraEditors.LabelControl lblKeywords;
		private DevExpress.XtraEditors.LabelControl lblKeywordsText;
		private DevExpress.XtraEditors.LabelControl lblProducer;
		private DevExpress.XtraEditors.LabelControl lblVersion;
		private DevExpress.XtraEditors.LabelControl lblLocation;
		private DevExpress.XtraEditors.LabelControl lblFileSize;
		private DevExpress.XtraEditors.LabelControl lblNumberOfPages;
		private DevExpress.XtraEditors.LabelControl lblCreated;
		private DevExpress.XtraEditors.LabelControl lblModified;
		private DevExpress.XtraEditors.LabelControl lblApplication;
		private DevExpress.XtraEditors.LabelControl lblNumberOfPagesText;
		private DevExpress.XtraEditors.LabelControl lblFileSizeText;
		private DevExpress.XtraEditors.LabelControl lblLocationText;
		private DevExpress.XtraEditors.LabelControl lblVersionText;
		private DevExpress.XtraEditors.LabelControl lblProducerText;
		private DevExpress.XtraEditors.LabelControl lblApplicationText;
		private DevExpress.XtraEditors.LabelControl lblModifiedText;
		private DevExpress.XtraEditors.LabelControl lblCreatedText;
		private DevExpress.XtraEditors.LabelControl lblSubjectText;
		private DevExpress.XtraEditors.LabelControl lblAuthorText;
		private DevExpress.XtraEditors.LabelControl lblTitleText;
		private DevExpress.XtraEditors.LabelControl lblFileText;
		private DevExpress.XtraEditors.LabelControl lblRevision;
		private DevExpress.XtraEditors.LabelControl lblAdvanced;
		private XtraEditors.LabelControl lblPageSizeText;
		private XtraEditors.LabelControl lblPageSize;
	}
}
