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
namespace DevExpress.XtraSpreadsheet.Model {
	public class CopiedRangeProvider {
		CellRange range;
		bool isCut;
		int transactedVersion = 0;
		string rangeReferenceCroppedByPrintRange;
		public void SetRange(CellRange range, bool isCutMode, int transactedVersion) {
			this.range = range;
			this.isCut = isCutMode;
			this.transactedVersion = transactedVersion;
			rangeReferenceCroppedByPrintRange = string.Empty;
		}
		public CellRange Range { get { return range; } }
		public bool IsCut { get { return isCut; } }
		public int TransactedVersion { get { return transactedVersion; } }
		public string RangeReferenceCroppedByPrintRange {
			get {
				if (range == null)
					return String.Empty;
				if (String.IsNullOrEmpty(rangeReferenceCroppedByPrintRange)) {
					rangeReferenceCroppedByPrintRange = GetRangeReferenceCroppedByPrintRange(Range);
				}
				return rangeReferenceCroppedByPrintRange;
			}
		}
		public void UpdateTransactedVersion(int newValue) {
			this.transactedVersion = newValue;
		}
		string GetRangeReferenceCroppedByPrintRange(CellRange range) {
			Worksheet sheet = range.Worksheet as Worksheet;
			CellRange printRange = sheet.GetPrintRange();
			String rangeToExportReference = ClipRangeByPrintableRange(range, printRange).ToString();
			return rangeToExportReference;
		}
		CellRange ClipRangeByPrintableRange(CellRange range, CellRange printableRange) {
			CellRange rangeToExport = range;
			if (printableRange != null) {
				VariantValue calculatedIntersection = range.IntersectionWith(printableRange);
				if (calculatedIntersection != VariantValue.ErrorNullIntersection)
					rangeToExport = calculatedIntersection.CellRangeValue as CellRange;
				else {
					var expanded = range.GetSubRange(0, 0, 1, 1);
					if (expanded != null)
						return expanded;
				}
			}
			return rangeToExport;
		}
	}
}
