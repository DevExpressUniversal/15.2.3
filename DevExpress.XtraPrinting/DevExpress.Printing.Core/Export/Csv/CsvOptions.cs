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
using System.IO;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using DevExpress.Office.Utils;
using DevExpress.Utils;
using DevExpress.Export.Xl;
namespace DevExpress.XtraExport.Csv {
	public enum CsvNewlineType {
		CrLf = 0,
		Lf,
		Cr,
		LfCr,
		VerticalTab,
		FormFeed
	}
	public class CsvDataAwareExporterOptions : IXlDocumentOptions {
		const string defaultValueSeparator = ",";
		const string alternativeValueSeparator = ";";
		const char defaultQuoteChar = '\"';
		string valueSeparator;
		Encoding encoding = Encoding.UTF8;
		CultureInfo culture = CultureInfo.InvariantCulture;
		bool isCustomValueSeparator;
		public CsvDataAwareExporterOptions() {
			valueSeparator = defaultValueSeparator;
			isCustomValueSeparator = false;
			TextQualifier = defaultQuoteChar;
			NewlineType = CsvNewlineType.CrLf;
			UseCellNumberFormat = true;
		}
		public char ValueSeparator {
			get { return valueSeparator[0]; }
			set {
				valueSeparator = new string(value, 1);
				isCustomValueSeparator = true;
			}
		}
		protected internal string ValueSeparatorString {
			get { return valueSeparator; }
			set {
				Guard.ArgumentIsNotNullOrEmpty(value, "ValueSeparatorString");
				valueSeparator = value;
				isCustomValueSeparator = true;
			}
		}
		public bool IsCustomValueSeparator { get { return isCustomValueSeparator; } }
		public char TextQualifier { get; set; }
		public CsvNewlineType NewlineType { get; set; }
		public bool UseCellNumberFormat { get; set; }
		public bool WritePreamble { get; set; }
		public bool NewlineAfterLastRow { get; set; }
		public CultureInfo Culture {
			get { return culture; }
			set {
				if(value == null)
					value = CultureInfo.InvariantCulture;
				culture = value;
				if(valueSeparator == defaultValueSeparator && culture.NumberFormat.NumberDecimalSeparator.IndexOf(defaultValueSeparator[0]) >= 0 && !isCustomValueSeparator)
					valueSeparator = alternativeValueSeparator;
			}
		}
		public Encoding Encoding {
			get { return encoding; }
			set {
				if(value == null)
					value = Encoding.UTF8;
				encoding = value;
			}
		}
		public bool SupportsFormulas { get { return false; } }
		public bool SupportsDocumentParts { get { return false; } }
		public bool SupportsOutlineGrouping { get { return false; } }
		public int MaxColumnCount { get { return int.MaxValue; } }
		public int MaxRowCount { get { return int.MaxValue; } }
		public XlDocumentFormat DocumentFormat { get { return XlDocumentFormat.Csv; } }
		public void ResetValueSeparator() {
			valueSeparator = defaultValueSeparator;
			isCustomValueSeparator = false;
			if(valueSeparator == defaultValueSeparator && culture.NumberFormat.NumberDecimalSeparator.IndexOf(defaultValueSeparator[0]) >= 0)
				valueSeparator = alternativeValueSeparator;
		}
	}
}
