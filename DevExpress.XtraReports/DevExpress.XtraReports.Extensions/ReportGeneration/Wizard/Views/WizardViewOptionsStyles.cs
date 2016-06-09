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
using DevExpress.XtraReports.UI;
namespace DevExpress.XtraReports.ReportGeneration.Wizard.Views {
	[ToolboxItem(false)]
	public partial class WizardViewOptionsStyles : WizardViewOptions, IWizardViewOptionsStyles {
		public WizardViewOptionsStyles(){
			InitializeComponent();
			InitCheckBoxes();
			this.documentViewer2.Load += On_Load;
			this.checkBox1.CheckedChanged += OnCheckedChangedAppearanceEvenRow;
			this.checkBox2.CheckedChanged += OnCheckedChangedPrintHorzLines;
			this.checkBox3.CheckedChanged += OnCheckedChangedAppearanceOddRow;
			this.checkBox4.CheckedChanged += OnCheckedChangedPrintVertLines;
			this.checkBox5.CheckedChanged += OnCheckedChangedUsePrintStyles;
		}
		private void InitCheckBoxes(){
			this.checkBox1.Checked = GetValue(Options.EnablePrintAppearanceEvenRow, true);
			this.checkBox2.Checked = GetValue(Options.PrintHorizontalLines, true);
			this.checkBox3.Checked = GetValue(Options.EnablePrintAppearanceOddRow, true);
			this.checkBox4.Checked = GetValue(Options.PrintVerticalLines, true);
			this.checkBox5.Checked = GetValue(Options.UsePrintAppearances, false);
		}
		private void OnCheckedChangedAppearanceEvenRow(object sender, EventArgs e){
			this.Options.EnablePrintAppearanceEvenRow = GetValue(this.checkBox1.Checked);
			OptionsChangedCore();
		}
		private void OnCheckedChangedPrintHorzLines(object sender, EventArgs e){
			this.Options.PrintHorizontalLines = GetValue(this.checkBox2.Checked);
			OptionsChangedCore();
		}
		private void OnCheckedChangedAppearanceOddRow(object sender, EventArgs e){
			this.Options.EnablePrintAppearanceOddRow = GetValue(this.checkBox3.Checked);
			OptionsChangedCore();
		}
		private void OnCheckedChangedPrintVertLines(object sender, EventArgs e){
			this.Options.PrintVerticalLines = GetValue(checkBox4.Checked);
			OptionsChangedCore();
		}
		private void OnCheckedChangedUsePrintStyles(object sender, EventArgs e){
			this.Options.UsePrintAppearances = GetValue(checkBox5.Checked);
			OptionsChangedCore();
		}
	}
}
