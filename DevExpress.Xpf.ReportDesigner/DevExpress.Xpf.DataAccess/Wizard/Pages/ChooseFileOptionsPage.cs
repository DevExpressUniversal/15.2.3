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
using System.Text;
using DevExpress.DataAccess.Excel;
using DevExpress.DataAccess.Wizard.Views;
using DevExpress.Mvvm.DataAnnotations;
using DevExpress.Mvvm.POCO;
using DevExpress.Xpf.DataAccess.DataSourceWizard.Native;
namespace DevExpress.Xpf.DataAccess.DataSourceWizard {
	[POCOViewModel]
	public class ChooseFileOptionsPage : DataSourceWizardPage, IChooseFileOptionsPageView {
		public static ChooseFileOptionsPage Create(DataSourceWizardModelBase model) {
			return ViewModelSource.Create(() => new ChooseFileOptionsPage(model));
		}
		protected ChooseFileOptionsPage(DataSourceWizardModelBase model)
			: base(model) {
			this.cultures = CultureInfo.GetCultures(CultureTypes.AllCultures);
			this.encodings = Encoding.GetEncodings();
			this.valueSeparators = new List<ValueSeparator>() {
				new ValueSeparator(',', "Comma"),
				new ValueSeparator(':', "Semicolon"),
				new ValueSeparator('\t', "Tab"),
				new ValueSeparator(' ', "Space")
			};
		}
		readonly CultureInfo[] cultures;
		public CultureInfo[] Cultures { get { return cultures; } }
		readonly EncodingInfo[] encodings;
		public EncodingInfo[] Encodings { get { return encodings; } }
		public ExcelDocumentFormat DocumentFormat { get; set; }
		readonly List<ValueSeparator> valueSeparators;
		public IEnumerable<ValueSeparator> ValueSeparators { get { return valueSeparators; } }
		ExcelSourceOptionsBaseViewModel sourceOptionsViewModel;
		public ExcelSourceOptionsBaseViewModel SourceOptionsViewModel { get { return sourceOptionsViewModel; } }
		public event EventHandler DetectEncoding;
		public event EventHandler DetectNewlineType;
		public event EventHandler DetectValueSeparator;
		void IChooseFileOptionsPageView.Initialize(ExcelSourceOptionsBase options) {
			InitializeSourceOptionsStub(options);
		}
		ExcelSourceOptionsBase IChooseFileOptionsPageView.SourceOptions { get { return sourceOptionsViewModel.options; } }
		void InitializeSourceOptionsStub(ExcelSourceOptionsBase options) {
			sourceOptionsViewModel = DocumentFormat == ExcelDocumentFormat.Csv
				? (ExcelSourceOptionsBaseViewModel)CsvSourceOptionsViewModel.Create((CsvSourceOptions)options, Detect)
				: (ExcelSourceOptionsBaseViewModel)ExcelSourceOptionsViewModel.Create((ExcelSourceOptions)options);
		}
		void IChooseFileOptionsPageView.SetEncoding(Encoding encoding) {
			((CsvSourceOptionsViewModel)sourceOptionsViewModel).Encoding = encoding;
		}
		void IChooseFileOptionsPageView.SetNewlineType(CsvNewlineType newlineType) {
			((CsvSourceOptionsViewModel)sourceOptionsViewModel).NewlineType = newlineType;
		}
		void IChooseFileOptionsPageView.SetValueSeparator(char separator) {
			((CsvSourceOptionsViewModel)sourceOptionsViewModel).ValueSeparator = separator;
		}
		void Detect() {
			if(DetectEncoding != null)
				DetectEncoding(this, EventArgs.Empty);
			if(DetectNewlineType != null)
				DetectNewlineType(this, EventArgs.Empty);
			if(DetectValueSeparator != null)
				DetectValueSeparator(this, EventArgs.Empty);
		}
		public class ValueSeparator {
			public ValueSeparator(char value, string displayName) {
				this.value = value;
				this.displayName = displayName;
			}
			readonly char value;
			public char Value { get { return value; } }
			readonly string displayName;
			public string DisplayName { get { return displayName; } }
		}
	}
}
