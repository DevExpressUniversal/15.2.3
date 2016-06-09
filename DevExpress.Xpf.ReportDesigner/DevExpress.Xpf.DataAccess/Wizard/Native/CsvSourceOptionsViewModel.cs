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
using System.ComponentModel;
using System.Globalization;
using System.Text;
using DevExpress.DataAccess.Excel;
using DevExpress.DataAccess.Wizard.Views;
using DevExpress.Mvvm.DataAnnotations;
using DevExpress.Mvvm.POCO;
namespace DevExpress.Xpf.DataAccess.DataSourceWizard.Native {
	[POCOViewModel]
	public class CsvSourceOptionsViewModel : ExcelSourceOptionsBaseViewModel {
		public static CsvSourceOptionsViewModel Create(CsvSourceOptions csvOptions, Action detect) {
			return ViewModelSource.Create(() => new CsvSourceOptionsViewModel(csvOptions, detect));
		}
		protected CsvSourceOptionsViewModel(CsvSourceOptions csvOptions, Action detect)
			: base(csvOptions) {
			this.csvOptions = csvOptions;
			this.detect = detect;
		}
		readonly CsvSourceOptions csvOptions;
		readonly Action detect;
		public virtual CultureInfo Culture {
			get { return csvOptions.Culture; }
			set { csvOptions.Culture = value; }
		}
		public virtual bool DetectEncoding {
			get { return csvOptions.DetectEncoding; }
			set {
				csvOptions.DetectEncoding = value;
				if(value) {
					detect();
					POCOViewModelExtensions.RaisePropertyChanged(this, x => x.Encoding);
				}
				POCOViewModelExtensions.RaisePropertyChanged(this, x => x.DetectEncoding);
			}
		}
		public virtual bool DetectNewlineType {
			get { return csvOptions.DetectNewlineType; }
			set {
				csvOptions.DetectNewlineType = value;
				if(value) {
					detect();
					POCOViewModelExtensions.RaisePropertyChanged(this, x => x.NewlineType);
				}
				POCOViewModelExtensions.RaisePropertyChanged(this, x => x.DetectNewlineType);
			}
		}
		public virtual bool DetectValueSeparator {
			get { return csvOptions.DetectValueSeparator; }
			set {
				csvOptions.DetectValueSeparator = value;
				if(value) {
					detect();
					POCOViewModelExtensions.RaisePropertyChanged(this, x => x.ValueSeparator);
				}
				POCOViewModelExtensions.RaisePropertyChanged(this, x => x.DetectValueSeparator);
			}
		}
		public virtual Encoding Encoding {
			get { return csvOptions.Encoding; }
			set { csvOptions.Encoding = value; }
		}
		public virtual CsvNewlineType NewlineType {
			get { return csvOptions.NewlineType; }
			set { csvOptions.NewlineType = value; }
		}
		public virtual char TextQualifier {
			get { return csvOptions.TextQualifier; }
			set { csvOptions.TextQualifier = value; }
		}
		public virtual bool TrimBlanks {
			get { return csvOptions.TrimBlanks; }
			set { csvOptions.TrimBlanks = value; }
		}
		public virtual char ValueSeparator {
			get { return csvOptions.ValueSeparator; }
			set { csvOptions.ValueSeparator = value; }
		}
		#region Stubs
		public bool SkipHiddenColumns { get; set; }
		public bool SkipHiddenRows { get; set; }
		#endregion
	}
}
