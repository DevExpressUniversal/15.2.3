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
using System.Globalization;
using System.Linq;
using System.Text;
using DevExpress.DataAccess.Excel;
using DevExpress.DataAccess.UI.Localization;
using DevExpress.DataAccess.Wizard.Views;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraLayout.Utils;
namespace DevExpress.DataAccess.UI.Wizard.Views {
	public partial class ChooseFileOptionsPageView : WizardViewBase, IChooseFileOptionsPageView {
		public class ValueSeparator {
			public ValueSeparator() { }
			public ValueSeparator(char value) : this(value, value.ToString()) { }
			public ValueSeparator(char value, string displayName) {
				Value = value;
				DisplayName = displayName;
			}
			public char Value { get; set; }
			public string DisplayName { get; set; }
			#region Equality members
			protected bool Equals(ValueSeparator other) {
				return Value == other.Value;
			}
			public override bool Equals(object obj) {
				ValueSeparator other = obj as ValueSeparator;
				return other != null && Equals(other);
			}
			public override int GetHashCode() {
				return Value.GetHashCode();
			}
			#endregion
			#region Overrides of Object
			public override string ToString() {
				return DisplayName;
			}
			#endregion
		}
		readonly EncodingInfo[] encodings = Encoding.GetEncodings();
		readonly CultureInfo[] cultures = CultureInfo.GetCultures(CultureTypes.AllCultures);
		readonly Array csvNewlineTypeValues = typeof(CsvNewlineType).GetEnumValues();
		readonly IList<ValueSeparator> valueSeparators = new List<ValueSeparator>(new[] {
			new ValueSeparator {Value = ',', DisplayName = "Comma"}, 
			new ValueSeparator {Value = ';', DisplayName = "Semicolon"},
			new ValueSeparator {Value = '\t', DisplayName = "Tab"},
			new ValueSeparator {Value = ' ', DisplayName = "Space"}
		}); 
		ExcelSourceOptions excelOptions;
		CsvSourceOptions csvOptions;
		public ChooseFileOptionsPageView() {
			InitializeComponent();
			LocalizeComponent();
		}
		#region Overrides of WizardViewBase
		public override string HeaderDescription {
			get { return DataAccessUILocalizer.GetString(DataAccessUIStringId.WizardPageChooseFileOptions); }
		}
		#endregion
		#region Implementation of IChooseExcelFileAndOptionsPageView
		public ExcelSourceOptionsBase SourceOptions {
			get {
				if(DocumentFormat == ExcelDocumentFormat.Csv)
					return GetCsvSourceOptions();
				return GetExcelSourceOptions();
			}
		}
		public ExcelDocumentFormat DocumentFormat { get; set; }
		public void SetEncoding(Encoding encoding) {
			if(encoding != null)
				lookUpEncoding.EditValue = encodings.FirstOrDefault(encodingInfo => encodingInfo.Name == csvOptions.Encoding.WebName);
		}
		public void SetNewlineType(CsvNewlineType newlineType) {
			comboBoxNewlineType.EditValue = newlineType;
		}
		public void SetValueSeparator(char separator) {
			lookUpValueSeparator.EditValue = separator;
		}
		public event EventHandler DetectEncoding;
		public event EventHandler DetectNewlineType;
		public event EventHandler DetectValueSeparator;
		public void Initialize(ExcelSourceOptionsBase options) {
			SetBaseSourceOptions(options);
			excelOptions = options as ExcelSourceOptions;
			if(excelOptions != null) {
				SetExcelSourceOptions(excelOptions);
				return;
			}
			csvOptions = options as CsvSourceOptions;
			if(csvOptions != null) {
				SetCsvSourceOptions(csvOptions);
			}
		}
		#endregion
		void LocalizeComponent() {
			checkTrimBlanks.Properties.Caption = DataAccessUILocalizer.GetString(DataAccessUIStringId.WizardPageChooseFileOptions_TrimBlanks);
			checkDetectValueSeparator.Properties.Caption = DataAccessUILocalizer.GetString(DataAccessUIStringId.WizardPageChooseFileOptions_DetectAutomatically);
			checkDetectNewlineType.Properties.Caption = DataAccessUILocalizer.GetString(DataAccessUIStringId.WizardPageChooseFileOptions_DetectAutomatically);
			checkDetectEncoding.Properties.Caption = DataAccessUILocalizer.GetString(DataAccessUIStringId.WizardPageChooseFileOptions_DetectAutomatically);
			ceSkipHiddenColumns.Properties.Caption = DataAccessUILocalizer.GetString(DataAccessUIStringId.WizardPageChooseFileOptions_SkipHiddenColumns);
			ceSkipHiddenRows.Properties.Caption = DataAccessUILocalizer.GetString(DataAccessUIStringId.WizardPageChooseFileOptions_SkipHiddenRows);
			ceSkipEmptyRows.Properties.Caption = DataAccessUILocalizer.GetString(DataAccessUIStringId.WizardPageChooseFileOptions_SkipEmptyRows);
			ceUseFirstRowAsHeader.Properties.Caption = DataAccessUILocalizer.GetString(DataAccessUIStringId.WizardPageChooseFileOptions_FirstRowAsFieldNames);
			layoutItemEncoding.Text = DataAccessUILocalizer.GetString(DataAccessUIStringId.WizardPageChooseFileOptions_Encoding);
			layoutItemNewlineType.Text = DataAccessUILocalizer.GetString(DataAccessUIStringId.WizardPageChooseFileOptions_NewLineType);
			layoutControlItem1.Text = DataAccessUILocalizer.GetString(DataAccessUIStringId.WizardPageChooseFileOptions_ValueSeparator);
			layoutItemCulture.Text = DataAccessUILocalizer.GetString(DataAccessUIStringId.WizardPageChooseFileOptions_Culture);
			layoutItemTextQualifier.Text = DataAccessUILocalizer.GetString(DataAccessUIStringId.WizardPageChooseFileOptions_TextQualifier);
		}
		void GetBaseSourceOptions(ExcelSourceOptionsBase options) {
			options.UseFirstRowAsHeader = ceUseFirstRowAsHeader.Checked;
			options.SkipEmptyRows = ceSkipEmptyRows.Checked;
		}
		void SetBaseSourceOptions(ExcelSourceOptionsBase value) {
			ceSkipEmptyRows.Checked = value.SkipEmptyRows;
			ceUseFirstRowAsHeader.Checked = value.UseFirstRowAsHeader;
		}
		ExcelSourceOptions GetExcelSourceOptions() {
			GetBaseSourceOptions(excelOptions);
			excelOptions.SkipHiddenColumns = ceSkipHiddenColumns.Checked;
			excelOptions.SkipHiddenRows = ceSkipHiddenRows.Checked;
			return excelOptions;
		}
		void SetExcelSourceOptions(ExcelSourceOptions value) {
			layoutGroupExcelOptions.Visibility = LayoutVisibility.Always;
			layoutGroupCsvOptions.Visibility = LayoutVisibility.Never;
			emptySpaceTop.Visibility = LayoutVisibility.Always;
			emptySpaceBetweenGroups.Visibility = LayoutVisibility.Never;
			ceSkipHiddenColumns.Checked = value.SkipHiddenColumns;
			ceSkipHiddenRows.Checked = value.SkipHiddenRows;
		}
		CsvSourceOptions GetCsvSourceOptions() {
			GetBaseSourceOptions(csvOptions);
			csvOptions.Culture = (CultureInfo)lookUpCulture.EditValue;
			csvOptions.DetectEncoding = checkDetectEncoding.Checked;
			csvOptions.Encoding = ((EncodingInfo)lookUpEncoding.EditValue).GetEncoding();
			csvOptions.DetectNewlineType = checkDetectNewlineType.Checked;
			csvOptions.NewlineType = (CsvNewlineType)comboBoxNewlineType.EditValue;
			csvOptions.DetectValueSeparator = checkDetectValueSeparator.Checked;
			csvOptions.ValueSeparator = ((char)lookUpValueSeparator.EditValue);
			csvOptions.TextQualifier = string.IsNullOrEmpty(textTextQualifier.Text)
				? '\0'
				: textTextQualifier.Text[0];
			csvOptions.TrimBlanks = checkTrimBlanks.Checked;
			return csvOptions;
		}
		void SetCsvSourceOptions(CsvSourceOptions options) {
			layoutGroupExcelOptions.Visibility = LayoutVisibility.Never;
			layoutGroupCsvOptions.Visibility = LayoutVisibility.Always;
			emptySpaceTop.Visibility = LayoutVisibility.Never;
			emptySpaceBetweenGroups.Visibility = LayoutVisibility.Always;
			PopulateCsvEditors();
			checkDetectEncoding.Checked = options.DetectEncoding;
			lookUpEncoding.EditValue = GetEncoding();
			checkDetectNewlineType.Checked = options.DetectNewlineType;
			comboBoxNewlineType.EditValue = options.NewlineType;
			checkDetectValueSeparator.Checked = options.DetectValueSeparator;
			lookUpValueSeparator.EditValue = options.ValueSeparator;
			lookUpCulture.EditValue = options.Culture;
			textTextQualifier.Text = options.TextQualifier.ToString();
			checkTrimBlanks.Checked = options.TrimBlanks;
		}
		void PopulateCsvEditors() {
			lookUpEncoding.Properties.DataSource = encodings;
			comboBoxNewlineType.Properties.Items.Clear();
			comboBoxNewlineType.Properties.Items.AddRange(csvNewlineTypeValues);
			lookUpCulture.Properties.DataSource = cultures;
			lookUpValueSeparator.Properties.DataSource = valueSeparators;
		}
		#region UI Event handlers
		void lookUpValueSeparator_ProcessNewValue(object sender, ProcessNewValueEventArgs e) {
			char separatorChar = e.DisplayValue.ToString()[0];
			var valueSeparator = new ValueSeparator(separatorChar);
			if(!valueSeparators.Contains(valueSeparator)) {
				valueSeparators.Add(valueSeparator);
				e.Handled = true;
			}
		}
		void lookUpValueSeparator_CustomDisplayText(object sender, CustomDisplayTextEventArgs e) {
			if(e.Value == null) {
				return;
			}
			var valueSeparator = valueSeparators.FirstOrDefault(vs => vs.Value == e.Value.ToString()[0]);
			if(valueSeparator == null) {
				return;
			}
			e.DisplayText = valueSeparator.DisplayName;
		}
		void checkDetectEncoding_CheckedChanged(object sender, EventArgs e) {
			if(checkDetectEncoding.Checked) {
				if(DetectEncoding != null) {
					DetectEncoding(this, EventArgs.Empty);
				}
			}
			lookUpEncoding.Enabled = !checkDetectEncoding.Checked;
		}
		void checkDetectNewlineType_CheckedChanged(object sender, EventArgs e) {
			if(checkDetectNewlineType.Checked) {
				if(DetectNewlineType != null) {
					DetectNewlineType(this, EventArgs.Empty);
				}
			}
			comboBoxNewlineType.Enabled = !checkDetectNewlineType.Checked;
		}
		void checkDetectValueSeparator_CheckedChanged(object sender, EventArgs e) {
			if(checkDetectValueSeparator.Checked) {
				if(DetectValueSeparator != null) {
					DetectValueSeparator(this, EventArgs.Empty);
				}
			}
			lookUpValueSeparator.Enabled = !checkDetectValueSeparator.Checked;
		}
		#endregion
		EncodingInfo GetEncoding() {
			return csvOptions.Encoding != null
				? encodings.FirstOrDefault(encodingInfo => encodingInfo.Name == csvOptions.Encoding.WebName)
				: null;
		}
	}
}
