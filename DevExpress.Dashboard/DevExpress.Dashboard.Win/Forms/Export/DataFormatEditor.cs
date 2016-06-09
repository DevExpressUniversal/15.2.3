#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       Dashboard                                                   }
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

using System.Windows.Forms;
using DevExpress.DashboardCommon;
using DevExpress.DashboardCommon.Localization;
using DevExpress.DashboardWin.Native.Printing;
using DevExpress.XtraEditors;
namespace DevExpress.DashboardWin.Forms.Export {
	public class DataFormatEditor : LabeledEditor {
		readonly ComboBoxEdit dataFormatEdit;
		public DataFormatEditor()
			: base(DashboardLocalizer.GetString(DashboardStringId.ExcelFormat)) {
			ExcelFormat[] formats = new ExcelFormat[] { ExcelFormat.Xlsx, ExcelFormat.Xls, ExcelFormat.Csv, };
			dataFormatEdit = CreateComboBoxEdit(formats, (value) => { OnValueChanged(new ExportDocumenInfoEventArgs { Initiator = this }); });
		}
		protected override Control Editor { get { return dataFormatEdit; } }
		protected override void SetInternal(ExtendedReportOptions opts) {
			dataFormatEdit.EditValue = ComboBoxRadioGroupItemsHelper.CreateComboBoxItem(opts.FormatOptions.ExcelOptions.Format);
		}
		public override void Apply(ExtendedReportOptions opts) {
			ComboBoxItem item = (ComboBoxItem)dataFormatEdit.SelectedItem;
			opts.FormatOptions.ExcelOptions.Format = (ExcelFormat)item.Value;
		}
	}
	public class CsvValueSeparatorEditor : LabeledEditor {
		readonly TextEdit separatorEdit;
		public CsvValueSeparatorEditor()
			: base(DashboardLocalizer.GetString(DashboardStringId.CsvValueSeparator)) {
			separatorEdit = CreateTextEdit();
		}
		protected override Control Editor { get { return separatorEdit; } }
		protected override void SetInternal(ExtendedReportOptions opts) {
			separatorEdit.Text = opts.FormatOptions.ExcelOptions.CsvValueSeparator;
		}
		public override void Apply(ExtendedReportOptions opts) {
			opts.FormatOptions.ExcelOptions.CsvValueSeparator = separatorEdit.Text;
		}
	}
}
