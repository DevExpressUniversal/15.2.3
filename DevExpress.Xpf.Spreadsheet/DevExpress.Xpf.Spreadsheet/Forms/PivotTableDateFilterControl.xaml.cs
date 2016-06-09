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

using DevExpress.Xpf.Editors;
using DevExpress.XtraSpreadsheet.Forms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
namespace DevExpress.Xpf.Spreadsheet.Forms {
	public partial class PivotTableDateFilterControl : UserControl {
		#region Field
		PivotTableDateFiltersViewModel viewModel;
		#endregion
		public PivotTableDateFilterControl(PivotTableDateFiltersViewModel viewModel) {
			InitializeComponent();
			DataContext = viewModel;
			this.viewModel = viewModel;
			InitizlizeDateEdits();
		}
		void InitizlizeDateEdits() {
			this.dateEdit.MaskType = MaskType.None;
			this.dateEdit1.MaskType = MaskType.None;
			this.dateEdit2.MaskType = MaskType.None;
		}
		void OnDateEditEditValueChanged(object sender, Editors.EditValueChangedEventArgs e) {
			CorrectDateEditText(sender as DateEdit);
		}
		void OnDateEdit1EditValueChanged(object sender, EditValueChangedEventArgs e) {
			CorrectDateEditText(sender as DateEdit);
		}
		void OnDateEdit2EditValueChanged(object sender, EditValueChangedEventArgs e) {
			CorrectDateEditText(sender as DateEdit);
		}
		void CorrectDateEditText(DateEdit dateEdit) {
			DateTime date = dateEdit.DateTime;
			if (date.Year < 1900)
				dateEdit.Text = null;
			else {
				string formatString = date.Hour > 0 || date.Minute > 0 || date.Second > 0 ? "G" : "d";
				dateEdit.DisplayFormatString = formatString;
			}
		}
		public void SetViewModelStringValues() {
			if (viewModel.IsOneValueFilter)
				viewModel.FirstStringValue = dateEdit.Text;
			else {
				viewModel.FirstStringValue = dateEdit1.Text;
				viewModel.SecondStringValue = dateEdit2.Text;
			}
		}
	}
}
