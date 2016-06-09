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
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.ComponentModel;
using DevExpress.XtraPivotGrid.Data;
using DevExpress.Xpf.Core.Native;
using System.Windows.Controls.Primitives;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Utils.Themes;
namespace DevExpress.Xpf.PivotGrid.Internal {
	public enum PivotGridThemeKeys {
		[BlendVisibility(false)]
		Template,
		BorderTemplate,
		ColumnValuesContentTemplate,
		RowValuesContentTemplate,
		CellsContentTemplate,
		CellsTemplate,
		FieldListDragTextStyle,
		FieldListBackgroundBrush,
		FieldListTemplate,
		ResizingIndicatorTemplate,		
		ExcelFieldListTemplate,
		ExcelFieldListItemTemplate,
		FilterCheckedTreeViewPopupTemplate,
		FilterCheckedTreeViewItemHeaderTemplate,
		ScrollerTemplate,
		ThemeLoaderTemplate,
		FilterWaitIndicatorTemplate,
	}
	public class PivotGridThemeKeyExtension : ThemeKeyExtensionBase<PivotGridThemeKeys> {
	}
	public enum ScrollableAreaThemeKeys {
		DataAreaItemTemplate,
		DataAreaItemContentTemplate,
		FieldAreaItemContentTemplate,
		FocusedDataAreaItemTemplate,
		ColumnAreaItemTemplate,
		RowAreaItemTemplate,
		ExpandButtonStyle,
		ExpandButtonTemplate,
		SortedByIndicatorTemplate,
		FieldCellKpiTemplate,
	}
	public class ScrollableAreaThemeKeyExtension : ThemeKeyExtensionBase<ScrollableAreaThemeKeys> {
	}
	public enum FieldHeadersThemeKeys {
		DataAreaContentTemplate,
		ColumnAreaContentTemplate,
		RowAreaContentTemplate,
		FilterAreaContentTemplate,
		DataAreaStyle,
		ColumnAreaStyle,
		RowAreaStyle,
		FilterAreaStyle,
		DataAreaPopupPadding,
	}
	public class FieldHeadersThemeKeyExtension : ThemeKeyExtensionBase<FieldHeadersThemeKeys> {
	}
	public enum FieldHeaderThemeKeys {
		DragElementTemplate,
		GroupDragElementTemplate,
		DragIndicatorTemplate,		
		ContentBorderStyle,
		ButtonStyle,
		ButtonContentTemplate,
		GroupContentTemplate,
		GroupButtonExpandItemStyle,
		GroupButtonCollapseItemStyle,
		CaptionToArrowIndentBorderStyle,
		SortArrowUpStyle,
		SortArrowUpTemplate,
		SortArrowDownStyle,
		SortArrowDownTemplate,
		ContentTemplate,
		ListContentTemplate,
		ContentStyle,
		HeadersVerticalLineBrush,
		FilterTemplate,
		FilterButtonTemplate,
		FieldListTemplate,
		FieldHeaderTemplate,
		InnerGroupHeaderTemplate,
		GroupHeaderTemplate,
		HeaderEmptyTextStyle,
		FieldHeaderTreeViewTemplate,
	}
	public class FieldHeaderThemeKeyExtension : ThemeKeyExtensionBase<FieldHeaderThemeKeys> {
	}
	public enum PrintingThemeKeys {
		FieldHeaderTemplate,
		FieldCellKpiTemplate,
		FieldValueTemplate,
		FieldCellTemplate,
		PageTemplate,
	}
	public class PrintingThemeKeyExtension : ThemeKeyExtensionBase<PrintingThemeKeys> {
	}
	public enum AppearanceThemeKeys {
		HeaderEmptyTextForeground,
		HeaderNormalGradient,
		HeaderActiveGradient,
		HeaderControlsColor,
		FilterColor,
		FilterMouseOverColor,
		FieldTotalBrush,
		ExpandButtonColor,
		ExpandButtonBackground,
		ExpandButtonInnerBackground,
		FocusedDataItemStroke,
		FieldListDragTextForeground,
		GroupBorderShadowColor,
		GroupButtonFill,
		GroupBorderBrush,
		ScrollingCornerGradient,
		AreaSeparatorBrush,
		EmptyAreaHighlighterStyle,
		RightDownCornerRadius,
		FieldListDragOverBorderStyle,
		FieldValueBorderBrush,
		FieldValueOuterBorderBrush,
		PivotBorderBrush,
		CellValueBrush,
		CellValueBorderBrush,
		CellTotalBrush,
		CellSelectedBrush,
		CellTotalSelectedBrush,
		ValueTotalSelectedBrush,
		ValueSelectedForegroundBrush,
		FieldValueSelectedBrush,
		FieldValueBackground,
		PivotBackground,
		ValueTotalSelectedForegroundBrush,
		ValueTotalForegroundBrush,
		ValueForegroundBrush,
		CellSelectedForegroundBrush,
		CellTotalSelectedForegroundBrush,
		CellTotalForegroundBrush,
		CellForegroundBrush,
		DataAreaCuttedTextForeground,
		ResizingIndicatorBrush,
	}
	public class AppearanceThemeKeyExtension : ThemeKeyExtensionBase<AppearanceThemeKeys> {
	}
}
