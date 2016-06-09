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
using DevExpress.Xpf.Utils.Themes;
namespace DevExpress.Xpf.Grid.Themes {
	public enum TableViewThemeKeys {
		[BlendVisibility(false)]
		ControlTemplate,
		ScrollViewerTemplate,
		DataNavigatorTemplate,
		ButtonNavigationStyle,
		DataNavigatorTextSize,
		DataNavigatorTextBrush,
		DataNavigatorBackground,
		DataNavigatorBackgroundText,
		DataNavigatorBorderMarginText,
		DataNavigatorBorderThickness,
		DataNavigatorFocusedBackground,
		DataNavigatorDisableBackground,
		UseGroupShadowIndent,
		LeftDataAreaIndent,
		RightDataAreaIndent,
		LeftGroupAreaIndent,
		RightGroupAreaIndent,
		Margin,
		[BlendVisibility(false)]
		FixedNoneHeadersPanelTemplate,
		[BlendVisibility(false)]
		FixedNoneDropPanelTemplate,
		[BlendVisibility(false)]
		FixedLeftNoneDropPanelTemplate,
		[BlendVisibility(false)]
		FixedRightNoneDropPanelTemplate,
		[BlendVisibility(false)]
		FixedLeftHeadersPanelTemplate,
		[BlendVisibility(false)]
		FixedRightHeadersPanelTemplate,
		[BlendVisibility(false)]
		FixedSummaryTextBlockStyle,
		[BlendVisibility(false)]
		HeadersTemplate,
		GroupPanelContentTemplate,
		GroupPanelDragTextStyle,
		GroupPanelBorderThickness,
		GroupPanelBorderBrush,
		GroupPanelSeparatorMargin,
		GroupPanelMargin,
		[BlendVisibility(false)]
		TotalSummaryContainerTemplate,
		[BlendVisibility(false)]
		PrintRowTemplate,
		[BlendVisibility(false)]
		PrintGroupRowTemplate,
		[BlendVisibility(false)]
		PrintGroupFooterTemplate,
		[BlendVisibility(false)]
		PrintHeaderTemplate,
		[BlendVisibility(false)]
		PrintFooterTemplate,
		[BlendVisibility(false)]
		PrintFixedFooterTemplate,
		ColumnChooserTemplate,
		ColumnBandChooserTemplate,
		ColumnChooserDragTextStyle,
		ColumnChooserBackgroundBrush,
		ColumnChooserBorderBrush,
		FixedLineWidth,
		IndicatorWidth,
		ExpandDetailButtonWidth,
		ExpandDetailButtonWidthTouch,
		DetailMargin,
		OuterBorderElementTemplate,
		OuterBorderElementStyle,
		HorizontalScrollbarMargin,
		RowPresenterGridMargin,
		IndicatorMargin,
		DataPresenterTemplate,
		SearchPanelWithoutGroupedPanelMargin,
		SearchPanelWithGroupedPanelMargin,
		SearchPanelBorderBottomBrush,
		SearchPanelBackground,
		SearchPanelMargin,
		SearchPanelWidth,
		TotalSummaryPanelMargin,
		SearchPanelContentTemplate,
		DefaultPrintStyleBase,
		DefaultPrintHeaderStyle,
		DefaultPrintTotalSummaryStyle,
		DefaultPrintFixedTotalSummaryStyle,
		DefaultPrintGroupRowStyle,
		DefaultPrintGroupFooterStyle,
		DefaultPrintCellStyle,
		DefaultPrintRowIndentStyle,
		GroupPanelSeparatorBrush,
		CheckBoxSelectorColumnWidth,
		EditFormDialogServiceTemplate,
		EditFormItemsPanelTemplate,
		EditFormItemTemplate,
		EditFormContentTemplate,
		EditFormForeground,
		EditFormBackground
	}
	public class TableViewThemeKeyExtension : ThemeKeyExtensionBase<TableViewThemeKeys> {
	}
}
