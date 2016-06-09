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

using DevExpress.XtraSpreadsheet.Forms;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Linq;
using System;
using DevExpress.XtraSpreadsheet.Model;
namespace DevExpress.Xpf.Spreadsheet.Forms {
	public partial class FieldSettingsPivotTableControl : UserControl {
		FieldSettingsPivotTableViewModel viewModel;
		public FieldSettingsPivotTableControl(FieldSettingsPivotTableViewModel viewModel) {
			InitializeComponent();
			DataContext = viewModel;
			this.viewModel = viewModel;
			InitializeSubtotalFunctionList();
		}
		public List<string> CastToListSelectedFunctions() {
			return this.lbFunctions.SelectedItems.Cast<string>().ToList();
		}
		void InitializeSubtotalFunctionList() {
			this.lbFunctions.Items.AddRange(viewModel.FunctionList.ToArray());
			foreach (PivotFieldItemType value in Enum.GetValues(typeof(PivotFieldItemType)))
				if (viewModel.Subtotal.HasFlag(value) && value != PivotFieldItemType.Blank && value != PivotFieldItemType.DefaultValue) 
					this.lbFunctions.SelectedItems.Add(viewModel.FunctionTable[value]);
		}
	}
}
