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
	public partial class WizardViewOptionsGeneral : WizardViewOptions, IWizardViewOptionsGeneral {
		public WizardViewOptionsGeneral(){
			InitializeComponent();
			InitCheckBoxes();
			this.documentViewer2.Load += On_Load;
			this.checkBox3.CheckedChanged += OnCheckedChangedHeader;
			this.checkBox2.CheckedChanged += OnCheckedChangedFooter;
			this.checkBox1.CheckedChanged += OnCheckedChangedMerge;
		}
		private void InitCheckBoxes(){
			this.checkBox3.Checked = GetValue(Options.PrintColumnHeaders, true);
			this.checkBox2.Checked = GetValue(Options.PrintTotalSummaryFooter, true);
		}
		private void OnCheckedChangedFooter(object sender, EventArgs e){
			this.Options.PrintTotalSummaryFooter = GetValue(checkBox2.Checked);
			OptionsChangedCore();
		}
		private void OnCheckedChangedMerge(object sender, EventArgs e){
			OptionsChangedCore();
		}
		private void OnCheckedChangedHeader(object sender, EventArgs e){
			this.Options.PrintColumnHeaders = GetValue(checkBox3.Checked);
			OptionsChangedCore();
		}
	}
}
