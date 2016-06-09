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
using DevExpress.Office.History;
using System.Collections.Generic;
using DevExpress.XtraSpreadsheet.Model.History;
namespace DevExpress.XtraSpreadsheet.Model {
	partial class Worksheet {
		#region RemoveHyperlinkAt
		public void RemoveHyperlinkAt(int index) {
			RemoveHyperlinkAt(index, true);
		}
		public void RemoveHyperlinkAt(int index, bool clearFormats) {
			DeleteWorksheetHyperlinkCommand command = new DeleteWorksheetHyperlinkCommand(this, index);
			command.ClearFormats = clearFormats;
			command.Execute();
		}
		#endregion
		#region ClearHyperlinks
		public void ClearHyperlinks() {
			DeleteAllWorksheetHyperlinkCommand command = new DeleteAllWorksheetHyperlinkCommand(this, Hyperlinks);
			command.Execute();
		}
		#endregion
		#region ClearHyperlinks
		protected internal void ClearHyperlinks(CellRangeBase range, bool clearFormats) {
			ClearHyperlinks(range, clearFormats, false);
		}
		protected internal void ClearHyperlinks(CellRangeBase range, bool clearFormats, bool checkFilteredRanges) {
			List<CellRange> filteredRanges = checkFilteredRanges ? GetFilteredRanges(range) : null;
			Workbook.BeginUpdate();
			try {
				using (HistoryTransaction transaction = new HistoryTransaction(Workbook.History)) {
					int count = this.Hyperlinks.Count;
					for (int i = count - 1; i >= 0; i--) {
						ModelHyperlink hyperlink = Hyperlinks[i];
						CellRange hyperlinkRange = hyperlink.Range;
						bool shouldDeleteHyperlink = (range.Includes(hyperlinkRange) || hyperlinkRange.Includes(range)) && !IsHyperLinkRangeFiltered(hyperlinkRange, filteredRanges);
						if (shouldDeleteHyperlink) {
							RemoveHyperlinkAt(i, clearFormats);
							if (clearFormats)
								this.ClearFormats(range, checkFilteredRanges);
						}
					}
					IgnoredErrors.Clear(range);
				}
			}
			finally {
				Workbook.EndUpdate();
			}
		}
		bool IsHyperLinkRangeFiltered(CellRange hyperlinkRange, List<CellRange> filteredRanges) {
			if (filteredRanges != null) {
				for (int rowIndex = hyperlinkRange.TopRowIndex; rowIndex <= hyperlinkRange.BottomRowIndex; rowIndex++) {
					if (IsRowFiltered(rowIndex, filteredRanges) && !IsRowVisible(rowIndex))
						return true;
				}
			}
			return false;
		}
		#endregion
		protected internal ModelHyperlink CreateHyperlinkCoreFromImport(CellRange range, string uri, bool isExternal) {
			return new ModelHyperlink(this, range, uri, isExternal);
		}
	}
}
