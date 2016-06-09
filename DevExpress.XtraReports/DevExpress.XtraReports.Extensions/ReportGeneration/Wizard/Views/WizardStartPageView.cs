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
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraExport.Helpers;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Printing;
namespace DevExpress.XtraReports.ReportGeneration.Wizard.Views {
	[ToolboxItem(false)]
	public partial class WizardStartPageView : WizardPageView, IWizardStartPageView {
		public WizardStartPageView() {
			InitializeComponent();
			this.Load += page_Load;
		}
		void page_Load(object sender, EventArgs e) {
			if(this.Components != null) {
				foreach(var view in this.Components) {
					this.listBoxControl1.Items.Add(new ImageListBoxItem(view, ((IComponent)view).Site != null ? ((IComponent)view).Site.Name : "unknown name")); 
				}
			}
			if(this.listBoxControl1.Items.Count > 0)
				this.listBoxControl1.SelectedIndex = 0;
		}
		public IGridViewFactory<ColumnImplementer, DataRowImplementer> View {
			get {
				var selectedItem = ((ImageListBoxItem)this.listBoxControl1.Items[this.listBoxControl1.SelectedIndex]).Value;
				return new GridViewFactoryImplementer((GridView) selectedItem);
			}
		}
		public IEnumerable<GridView> Components { get; set; }
	}
}
