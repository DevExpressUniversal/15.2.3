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
namespace DevExpress.Xpf.Grid {
	public enum WaitIndicatorType {
		Default,
		Panel,
		None,
	}
	public enum ColumnPosition { Left, Middle, Right, Single, Standalone }
	[Flags]
	public enum BestFitArea {
		None = 0,
		Header = 1,
		Rows = 2,
		TotalSummary = 4,
		GroupSummary = 6,
		All = Header | Rows | TotalSummary | GroupSummary,
	}
	public enum FixedStyle { None, Left, Right };
	public enum AutoFilterCondition { Like, Equals, Contains, Default }
	public enum FilterPopupMode { Default, List, CheckedList, Custom, Date, DateAlt, DateSmart, DateCompact }
	public enum ColumnFilterMode { Value, DisplayText }
	public enum HeaderPresenterType { Headers, GroupPanel, ColumnChooser }
	public enum ScrollingMode { Normal, Smart }
	public enum ShowFilterPanelMode {
		Default,
		ShowAlways,
		Never
	}
	public enum ShowSearchPanelMode {
		Default,
		Always,
		Never
	}
	public enum GridContainerRowsLocation { TopOnly, BottomOnly, TopAndBottom, Middle }
	public enum RowPosition { Top, Middle, Bottom, Single }
	public enum IndicatorState {
		None,
		Focused,
		Changed,
		NewItemRow,
		Editing,
		Error,
		FocusedError,
		AutoFilterRow
	}
	public enum GridViewNavigationStyle { Row, Cell, None }
	public enum SelectionState { None, Focused, Selected, FocusedAndSelected, CellMerge }
	public enum EditorButtonShowMode { ShowOnlyInEditor, ShowForFocusedCell, ShowForFocusedRow, ShowAlways }
	public enum GridMenuType { Column, TotalSummary, RowCell, GroupPanel, FixedTotalSummary, Band, GroupFooterSummary, GroupRow };
	public enum AutoGenerateColumnsMode { None, KeepOld, AddNew, RemoveOld }
	public enum MultiSelectMode { None, Row, Cell, MultipleRow }
	public enum ClipboardCopyMode { Default, None, ExcludeHeader, IncludeHeader }
	[Flags]
	public enum ClipboardCopyOptions {
		None = 0x0,
		Csv = 0x1,
		Excel = 0x2,
		Html = 0x4,
		Rtf = 0x8,
		Txt = 0x10,
		All = Csv | Excel | Html | Rtf | Txt
	}
	public enum ClipboardMode {
		PlainText = 0,
		Formatted = 1,
	}
	public enum EditFormPostMode {
		Cached,
		Immediate
	}
	public enum EditFormShowMode {
		Dialog,
		Inline,
		InlineHideRow,
		None
	}
	public enum PostConfirmationMode {
		YesNoCancel,
		YesNo,
		None
	}
}
