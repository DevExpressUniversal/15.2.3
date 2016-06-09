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
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DevExpress.Data.XtraReports.ReportGeneration;
using DevExpress.Utils;
using DevExpress.XtraReports.UI;
namespace DevExpress.XtraReports.ReportGeneration.Wizard.Views {
	[ToolboxItem(false)]
	public partial class WizardViewOptions : WizardPageView, IWizardOptionsView {
		ReportGenerationOptions options;
		public WizardViewOptions(){
			InitializeComponent();
			this.options = new ReportGenerationOptions();
			this.documentViewer2.PageBorderColor = Color.Gray;
			this.documentViewer2.PageBorderVisibility = DocumentView.PageBorderVisibility.AllWithoutSelection;
		}
		protected void OptionsChangedCore(){
			if(OptionsChanged != null){
				OptionsChanged(this, EventArgs.Empty);
			}
			UpdatePreview();
		}
		protected bool GetValue(DefaultBoolean val, bool defaultValue){
			if(val == DefaultBoolean.Default) return defaultValue;
			return val == DefaultBoolean.True;
		}
		protected DefaultBoolean GetValue(bool val){
			return val ? DefaultBoolean.True : DefaultBoolean.False;
		}
		protected void On_Load(object sender, EventArgs e){ 
			if(PageLoad != null)
				PageLoad(this, EventArgs.Empty);
			UpdatePreview();
		}
		protected void UpdatePreview(){
			if(this.Report != null){
				this.documentViewer2.Zoom = 0.35f;
				this.documentViewer2.DocumentSource = this.Report;
				this.Report.CreateDocument();
			}
		}
		public XtraReport Report { get; set; }
		public ReportGenerationOptions Options { get { return options; } }
		public event EventHandler OptionsChanged;
		public event EventHandler PageLoad;
	}
}
