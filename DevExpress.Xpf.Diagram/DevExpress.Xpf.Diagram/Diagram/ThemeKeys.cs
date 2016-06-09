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
namespace DevExpress.Xpf.Diagram.Themes {
	public enum DiagramToolboxControlThemeKeys {
		Background,
		StencilGroupMargin,
		ShapePresenterSize,
		ShapeDescriptionWidth,
	}
	public enum RulerThemeKeys {
		Shadow,
		Foreground,
		HorizontalBackground,
		VerticalBackground,
		BorderBrush,
		TickBrush,
		ScaleFontSize,
		Size,
	}
	public enum ControlsThemeKeys {
		PageBorderBrush,
		GridLineBrush,
		DiagramControlBackground,
		SelectionPreviewBackground,
		SelectionPreviewBorderBrush,
		PageBorderThickness,
		DiagramConnectorArrowSize,
		DragPreviewOpacity
	}
	public enum DiagramControlThemeKeys {
		DiagramControlBackground,
		BackgroundTemplate,
		PageBackground,
		PageBackgroundTemplate,
	}
	public enum DiagramDesignerControlThemeKeys {
		NewDocumentButtonStyle,
		LoadDocumentButtonStyle,
		SaveDocumentButtonStyle,
		SaveDocumentAsButtonStyle,
		UndoButtonStyle,
		RedoButtonStyle,
		PasteButtonStyle,
		CutButtonStyle,
		CopyButtonStyle,
		SingleToolItemStyle,
		SplitToolsItemStyle,
		ToolItemStyle,
		LiveResizingButtonStyle,
		SnapToGridButtonStyle,
		SnapToItemsButtonStyle,
		EmptySelectionButtonStyle,
		SnapDistanceEditorStyle,
		FontFamilySelectorStyle,
		FontSizeSelectorStyle,
		FontSizeIncreaseButtonStyle,
		FontSizeDecreaseButtonStyle,
		FontBoldButtonStyle,
		FontItalicButtonStyle,
		FontUnderlineButtonStyle,
		FontStrikethroughButtonStyle,
		FontBackgroundSplitButtonStyle,
		FontForegroundSplitButtonStyle,
		TextHorizontalAlignmentButtonsStyle,
		TextVerticalAlignmentButtonsStyle,
		BringToFrontSplitButtonStyle,
		BringToFrontButtonStyle,
		BringForwardButtonStyle,
		SendToBackSplitButtonStyle,
		SendToBackButtonStyle,
		SendBackwardButtonStyle,
		ThemeSelectorGalleryItemStyle,
		PageSizeBarItemStyle,
		PageSizeButtonsStyle,
		GridSizeBarItemStyle,
		CanvasAutoSizeBarItemStyle,
		CanvasAutoSizeButtonStyle,
		PaperOrientationBarItemStyle,
		PaperVerticalOrientationButtonStyle,
		PaperHorizontalOrientationButtonStyle,
		TreeLayoutRightToLeftButtonStyle,
		TreeLayoutBottomToTopButtonStyle,
		TreeLayoutLeftToRightButtonStyle,
		TreeLayoutTopToBottomButtonStyle,
		TreeLayoutDirectionButtonStyle,
		ShapeBackgroundColorSplitButtonStyle,
		ShapeForegroundColorSplitButtonStyle,
		ShapeStrokeColorSplitButtonStyle,
		PageSizeHeightEditorStyle,
		PageSizeWidthEditorStyle,
		SetPageSizeButtonStyle
	}
public enum AdornersThemeKeys {
		PointsSize,
		ResizeBoxBackground,
		SelectionAdornerStroke,
		ParameterPointsBackground,
		ParameterPointsStroke,
		SelectionAdornerRectStrokeThickness,
		SelectionAdornerMoveBorderThickness,
		SelectionAdornerMoveBorderBrush,
		SelectionPartBorderBrush,
		SelectionPartDefaultBorderThickness,
		SelectionPartSelectBorderThickness,
		ConnectorConnectedMarkerFill,
		ConnectorConnectedMarkerEndFill,
		ConnectorConnectedMarkerStroke,
		BeginConnectedElementSize,
		BeginConnectedElementBackground,
		EndConnectedElementSize,
		EndConnectedElementBackground,
		EndFreeElementBackground,
		ConnectorIntermediatePointFill,
		ConnectorIntermediatePointSize,
		ConnectorSelectionPartDefaultThickness,
		ConnectorSelectionPartSelectedThickness,
		ConnectorDragColor,
		ConnectorDragThickness,
		ConnectorMovePointColor,
		GlueHighlightBrush,
		GlueToPointBorderThickness,
		GlueToPointBorderSize,
		ShapeConnectionPointBrush,
		ShapeConnectionPointSize,
		RotationAdornerMargin,
		SnapLineBrush,
		SnapLineExtent,
		SizeSnapLineTemplate,
		ShapePresenterDefaultSize,
	}
	public class DiagramToolboxControlThemeKeyExtension : ThemeKeyExtensionBase<DiagramToolboxControlThemeKeys> { };
	public class RulerThemeKeyExtension : ThemeKeyExtensionBase<RulerThemeKeys> { };
	public class ControlsThemeKeysExtension : ThemeKeyExtensionBase<ControlsThemeKeys> { };
	public class DiagramControlThemeKeysExtension : ThemeKeyExtensionBase<DiagramControlThemeKeys> { };
	public class DiagramDesignerControlThemeKeysExtension : ThemeKeyExtensionBase<DiagramDesignerControlThemeKeys> { };
	public class AdornersThemeKeysExtension : ThemeKeyExtensionBase<AdornersThemeKeys> { };
}
