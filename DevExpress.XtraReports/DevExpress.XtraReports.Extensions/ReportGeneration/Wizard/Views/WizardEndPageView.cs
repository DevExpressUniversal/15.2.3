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
namespace DevExpress.XtraReports.ReportGeneration.Wizard.Views {
	[ToolboxItem(false)]
	public partial class WizardEndPageView : WizardPageView, IWizardEndPageView {
		public WizardEndPageView(){
			InitializeComponent();
			this.Load += On_Load;
		}
		protected void On_Load(object sender, EventArgs e){ 
			if(PageLoad != null)
				PageLoad(this, EventArgs.Empty);
			this.textEdit1.Text = this.ReportFileName;
		}
		public bool AddToProject { get; set; }
		public string ReportFileName { get; set; }
		public string ReportFilePath { get; set; }
		public event EventHandler PageLoad;
		private void finishButton_Click(object sender, EventArgs e){
			this.ReportFileName = textEdit1.Text;
			if(this.checkBox1.Checked){
				this.saveFileDialog1.FileName = textEdit1.Text;
				this.saveFileDialog1.Filter = "C# (*.cs)|*.cs|All files (*.*)|*.*";
				this.saveFileDialog1.ShowDialog();
				this.ReportFilePath = saveFileDialog1.FileName;
			} else this.AddToProject = true;
		}
	}
}
