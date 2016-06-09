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

using System.Globalization;
using System.Text;
using DevExpress.DataAccess.Excel;
using DevExpress.Mvvm.DataAnnotations;
using DevExpress.Mvvm.POCO;
namespace DevExpress.Xpf.DataAccess.DataSourceWizard.Native {
	[POCOViewModel]
	public class ExcelSourceOptionsViewModel : ExcelSourceOptionsBaseViewModel {
		public static ExcelSourceOptionsViewModel Create(ExcelSourceOptions excelOptions) {
			return ViewModelSource.Create(() => new ExcelSourceOptionsViewModel(excelOptions));
		}
		protected ExcelSourceOptionsViewModel(ExcelSourceOptions excelOptions)
			: base(excelOptions) {
			this.excelOptions = excelOptions;
		}
		readonly ExcelSourceOptions excelOptions;
		public virtual bool SkipHiddenColumns {
			get { return excelOptions.SkipHiddenColumns; }
			set { excelOptions.SkipHiddenColumns = value; }
		}
		public virtual bool SkipHiddenRows {
			get { return excelOptions.SkipHiddenRows; }
			set { excelOptions.SkipHiddenRows = value; }
		}
		#region Stubs
		public CultureInfo Culture { get; set; }
		public bool DetectEncoding { get; set; }
		public bool DetectNewlineType { get; set; }
		public bool DetectValueSeparator { get; set; }
		public Encoding Encoding { get; set; }
		public CsvNewlineType NewlineType { get; set; }
		public char TextQualifier { get; set; }
		public bool TrimBlanks { get; set; }
		public char ValueSeparator { get; set; }
		#endregion
	}
}
