#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       UWP Controls                                                }
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
using DevExpress.Pdf.Native;
namespace DevExpress.Pdf {
	public class PdfColumnLayoutLogicalStructureElementAttribute : PdfLayoutLogicalStructureElementAttribute {
		const string columnCountKey = "ColumnCount";
		const string columnGapKey = "ColumnGap";
		const string columnWidthsKey = "ColumnWidths";
		internal static string[] Keys = new string[] { columnCountKey, columnGapKey, columnWidthsKey };
		readonly int columnCount = 1;
		readonly IList<double> columnGap;
		readonly IList<double> columnWidths;
		public int ColumnCount { get { return columnCount; } }
		public IList<double> ColumnGap { get { return columnGap; } }
		public IList<double> ColumnWidths { get { return columnWidths; } }
		internal PdfColumnLayoutLogicalStructureElementAttribute(PdfReaderDictionary dictionary)
			: base(dictionary) {
			PdfObjectCollection collection = dictionary.Objects;
			object value;
			columnCount = dictionary.GetInteger(columnCountKey) ?? 1;
			if (dictionary.TryGetValue(columnGapKey, out value))
				columnGap = GetValues(value, columnCount - 1, collection);
			if (dictionary.TryGetValue(columnWidthsKey, out value))
				columnWidths = GetValues(value, columnCount, collection);
		}
		protected override PdfWriterDictionary CreateDictionary(PdfObjectCollection collection) {
			PdfWriterDictionary dictionary = base.CreateDictionary(collection);
			dictionary.Add(columnCountKey, columnCount, 1);
			dictionary.AddIfPresent(columnGapKey, WriteValues(columnGap));
			dictionary.AddIfPresent(columnWidthsKey, WriteValues(columnWidths));
			return dictionary;
		}
		IList<double> GetValues(object value, int listLength, PdfObjectCollection collection) {
			value = collection.TryResolve(value);
			if (value is double || value is int) {
				double val = Convert.ToDouble(value);
				List<double> result = new List<double>(listLength);
				for (int i = 0; i < listLength; i++)
					result.Add(val);
				return result;
			}
			IList<object> list = value as IList<object>;
			if (list != null) {
				int listCount = list.Count;
				int count = list.Count > listLength ? list.Count : listLength;
				List<double> result = new List<double>(count);
				for (int i = 0; i < count; i++)
					result.Add(i < listCount ? PdfDocumentReader.ConvertToDouble(list[i]) : result[i - 1]);
				return result;
			}
			return null;
		}
		object WriteValues(IList<double> value) {
			if (value != null) {
				for (int lastIndex = value.Count - 1; lastIndex >= 1; lastIndex--)
					if (value[lastIndex] != value[lastIndex - 1]) {
						List<double> prepare = new List<double>();
						for (int i = 0; i <= lastIndex; i++)
							prepare.Add(value[i]);
						return new PdfWritableDoubleArray(prepare);
					}
				return value[0];
			}
			return null;
		}
	}
}
