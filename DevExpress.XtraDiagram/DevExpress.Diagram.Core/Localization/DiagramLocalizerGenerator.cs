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

using System.ComponentModel;
using System;
namespace DevExpress.Diagram.Core.Localization {
	#region enum DiagramControlStringId
	public enum DiagramControlStringId {
		ContextMenu_Cut,
		ContextMenu_Copy,
		ContextMenu_Paste,
		ContextMenu_EditText,
		ContextMenu_View,
		ContextMenu_Order,
		ContextMenu_Move_Up,
		ContextMenu_Move_Down,
		ContextMenu_Close,
		ContextMenu_Icons_And_Names,
		ContextMenu_Names_Under_Icons,
		ContextMenu_Icons_Only,
		ContextMenu_Names_Only,
		Menu_Undo_Button,
		Menu_Undo_Button_Description,
		Menu_Redo_Button,
		Menu_Redo_Button_Description,
		Menu_Delete_Button,
		Menu_AllowLiveResizing_Button,
		Menu_SnapToGrid_Button,
		Menu_SnapToItems_Button,
		Menu_AllowEmptySelection_Button,
		Menu_SnapDistance_Editor,
		ToolDisplayFormat,
		Tool_Pointer,
		Tool_Connector,
		Tool_Container,
		MeasureUnit_Pixels,
		MeasureUnit_Inches,
		MeasureUnit_Millimeters,
		Layout_BringToFront,
		Layout_BringToFront_Description,
		Layout_BringForward,
		Layout_SendToBack,
		Layout_SendToBack_Description,
		Layout_SendBackward,
		Themes_Office_Name,
		Themes_Linear_Name,
		Themes_NoTheme_Name,
		Themes_Integral_Name,
		Themes_Daybreak_Name,
		Themes_Parallel_Name,
		Themes_Sequence_Name,
		Themes_Lines_Name,
		Themes_VariantStyleId_Name,
		Themes_ThemeStyleId_Name,
		Themes_SubtleEffect_Name,
		Themes_RefinedEffect_Name,
		Themes_BalancedEffect_Name,
		Themes_ModerateEffect_Name,
		Themes_FocusedEffect_Name,
		Themes_IntenseEffect_Name,
		Search_Shapes_Null_Text,
		QuickShapes_Name,
		Stencils_Name,
		No_Stencils_Open_Name,
		More_Shapes_Name,
		Panel_Shapes_Name,
		Panel_Properties_Name,
		BasicShapes_Name,
		BasicShapes_Rectangle_Name,
		BasicShapes_Ellipse_Name,
		BasicShapes_Triangle_Name,
		BasicShapes_RightTriangle_Name,
		BasicShapes_Pentagon_Name,
		BasicShapes_Hexagon_Name,
		BasicShapes_Heptagon_Name,
		BasicShapes_Octagon_Name,
		BasicShapes_Decagon_Name,
		BasicShapes_Can_Name,
		BasicShapes_Parallelogram_Name,
		BasicShapes_Trapezoid_Name,
		BasicShapes_Diamond_Name,
		BasicShapes_Cross_Name,
		BasicShapes_Chevron_Name,
		BasicShapes_Cube_Name,
		BasicShapes_Star4_Name,
		BasicShapes_Star5_Name,
		BasicShapes_Star6_Name,
		BasicShapes_Star7_Name,
		BasicShapes_Star16_Name,
		BasicShapes_Star24_Name,
		BasicShapes_Star32_Name,
		BasicShapes_RoundedRectangle_Name,
		BasicShapes_SingleSnipCornerRectangle_Name,
		BasicShapes_SnipSameSideCornerRectangle_Name,
		BasicShapes_SnipDiagonalCornerRectangle_Name,
		BasicShapes_SingleRoundCornerRectangle_Name,
		BasicShapes_RoundSameSideCornerRectangle_Name,
		BasicShapes_RoundDiagonalCornerRectangle_Name,
		BasicShapes_SnipAndRoundSingleCornerRectangle_Name,
		BasicShapes_SnipCornerRectangle_Name,
		BasicShapes_RoundCornerRectangle_Name,
		BasicShapes_SnipAndRoundCornerRectangle_Name,
		BasicShapes_Plaque_Name,
		BasicShapes_Frame_Name,
		BasicShapes_FrameCorner_Name,
		BasicShapes_LShape_Name,
		BasicShapes_DiagonalStripe_Name,
		BasicShapes_Donut_Name,
		BasicShapes_NoSymbol_Name,
		BasicShapes_LeftParenthesis_Name,
		BasicShapes_RightParenthesis_Name,
		BasicShapes_LeftBrace_Name,
		BasicShapes_RightBrace_Name,
		BasicFlowchartShapes_Name,
		BasicFlowchartShapes_Process_Name,
		BasicFlowchartShapes_Decision_Name,
		BasicFlowchartShapes_Subprocess_Name,
		BasicFlowchartShapes_StartEnd_Name,
		BasicFlowchartShapes_Document_Name,
		BasicFlowchartShapes_Data_Name,
		BasicFlowchartShapes_Database_Name,
		BasicFlowchartShapes_ExternalData_Name,
		BasicFlowchartShapes_Custom1_Name,
		BasicFlowchartShapes_Custom2_Name,
		BasicFlowchartShapes_Custom3_Name,
		BasicFlowchartShapes_Custom4_Name,
		BasicFlowchartShapes_OnPageReference_Name,
		BasicFlowchartShapes_OffPageReference_Name,
		SDLDiagramShapes_Name,
		SDLDiagramShapes_Start_Name,
		SDLDiagramShapes_VariableStart_Name,
		SDLDiagramShapes_Procedure_Name,
		SDLDiagramShapes_VariableProcedure_Name,
		SDLDiagramShapes_CreateRequest_Name,
		SDLDiagramShapes_Alternative_Name,
		SDLDiagramShapes_Return_Name,
		SDLDiagramShapes_Decision1_Name,
		SDLDiagramShapes_MessageFromUser_Name,
		SDLDiagramShapes_PrimitiveFromCallControl_Name,
		SDLDiagramShapes_Decision2_Name,
		SDLDiagramShapes_MessageToUser_Name,
		SDLDiagramShapes_PrimitiveToCallControl_Name,
		SDLDiagramShapes_Save_Name,
		SDLDiagramShapes_OnPageReference_Name,
		SDLDiagramShapes_OffPageReference_Name,
		SDLDiagramShapes_Document_Name,
		SDLDiagramShapes_DiskStorage_Name,
		SDLDiagramShapes_DividedProcess_Name,
		SDLDiagramShapes_DividedEvent_Name,
		SDLDiagramShapes_Terminator_Name,
		SoftwareIcons_Name,
		SoftwareIcons_Back_Name,
		SoftwareIcons_Forward_Name,
		SoftwareIcons_Expand_Name,
		SoftwareIcons_Collapse_Name,
		SoftwareIcons_Add_Name,
		SoftwareIcons_Remove_Name,
		SoftwareIcons_ZoomIn_Name,
		SoftwareIcons_ZoomOut_Name,
		SoftwareIcons_Lock_Name,
		SoftwareIcons_Permission_Name,
		SoftwareIcons_Sort_Name,
		SoftwareIcons_Filter_Name,
		SoftwareIcons_Tools_Name,
		SoftwareIcons_Properties_Name,
		SoftwareIcons_Calendar_Name,
		SoftwareIcons_Document_Name,
		SoftwareIcons_Database_Name,
		SoftwareIcons_HardDrive_Name,
		SoftwareIcons_Network_Name,
		DecorativeShapes_Name,
		DecorativeShapes_LightningBolt_Name,
		DecorativeShapes_Moon_Name,
		DecorativeShapes_Wave_Name,
		DecorativeShapes_DoubleWave_Name,
		DecorativeShapes_VerticalScroll_Name,
		DecorativeShapes_HorizontalScroll_Name,
		DecorativeShapes_Heart_Name,
		DecorativeShapes_DownRibbon_Name,
		DecorativeShapes_UpRibbon_Name,
		DecorativeShapes_Cloud_Name,
		ArrowShapes_Name,
		ArrowShapes_SimpleArrow_Name,
		ArrowShapes_SimpleDoubleArrow_Name,
		ArrowShapes_ModernArrow_Name,
		ArrowShapes_FlexibleArrow_Name,
		ArrowShapes_BentArrow_Name,
		ArrowShapes_UTurnArrow_Name,
		ArrowShapes_SharpBentArrow_Name,
		ArrowShapes_CurvedRightArrow_Name,
		ArrowShapes_CurvedLeftArrow_Name,
		ArrowShapes_NotchedArrow_Name,
		ArrowShapes_StripedArrow_Name,
		ArrowShapes_BlockArrow_Name,
		ArrowShapes_CircularArrow_Name,
		ArrowShapes_QuadArrow_Name,
		ArrowShapes_LeftRightUpArrow_Name,
		ArrowShapes_LeftRightArrowBlock_Name,
		ArrowShapes_QuadArrowBlock_Name,
		Arrow_Open90,
		Arrow_Filled90,
		Arrow_ClosedDot,
		Arrow_FilledDot,
		Arrow_OpenFletch,
		Arrow_FilledFletch,
		Arrow_Diamond,
		Arrow_FilledDiamond,
		Arrow_ClosedDiamond,
		Arrow_IndentedFilledArrow,
		Arrow_OutdentedFilledArrow,
		Arrow_FilledSquare,
		Arrow_ClosedASMEArrow,
		Arrow_FilledDoubleArrow,
		Arrow_ClosedDoubleArrow,
		RibbonPage_Home,
		RibbonPageGroup_Action,
		RibbonPageGroup_Toolbox,
		RibbonPageGroup_Document,
		RibbonPageGroup_File,
		RibbonPageGroup_Clipboard,
		RibbonPageGroup_Options,
		RibbonPageGroup_Font,
		RibbonPageGroup_Paragraph,
		RibbonPageGroup_Tools,
		RibbonPage_View,
		RibbonPageGroup_Show,
		RibbonPageGroup_Appearance,
		RibbonPageGroup_Arrange,
		RibbonPageGroup_PageSetup,
		RibbonPageGroup_Themes,
		RibbonPageGroup_ShapeStyles,
		RibbonPageGroup_TreeLayout,
		RibbonPageGroup_UserInterfaceThemes,
		RibbonPage_Design,
		RibbonGallery_VariantStyles,
		RibbonGallery_ThemeStyles,
		Shape_BackgroundColor,
		Shape_ForegroundColor,
		Shape_StrokeColor,
		TreeLayout_ReLayoutPage,
		TreeLayout_BufferSize,
		TreeLayout_RightToLeft,
		TreeLayout_BottomToTop,
		TreeLayout_LeftToRight,
		TreeLayout_TopToBottom,
		Ribbon_PageSize,
		Ribbon_MeasureUnit,
		Ribbon_GridSize,
		Ribbon_CanvasAutoSize,
		Ribbon_CanvasNoneAutoSize,
		Ribbon_CanvasAutoSizeDescription,
		Ribbon_CanvasNoneAutoSizeDescription,
		Ribbon_Orientation,
		Ribbon_VerticalOrientation,
		Ribbon_HorizontalOrientation,
		DiagramCommand_Paste,
		DiagramCommand_Paste_Description,
		DiagramCommand_Cut,
		DiagramCommand_Cut_Description,
		DiagramCommand_Copy,
		DiagramCommand_Copy_Description,
		DiagramCommand_Open,
		DiagramCommand_Open_Description,
		DiagramCommand_Save,
		DiagramCommand_Save_Description,
		DiagramCommand_SaveAs,
		DiagramCommand_SaveAs_Description,
		DiagramCommand_New,
		DiagramCommand_New_Description,
		DiagramDesigner,
		DiagramDesigner_SaveChangesConfirmation,
		Font_Font,
		Font_Font_Description,
		Font_FontSize,
		Font_FontSize_Description,
		Font_FontSizeIncrease,
		Font_FontSizeIncrease_Description,
		Font_FontSizeDecrease,
		Font_FontSizeDecrease_Description,
		Font_FontBold,
		Font_FontBold_Description,
		Font_FontItalic,
		Font_FontItalic_Description,
		Font_FontUnderline,
		Font_FontUnderline_Description,
		Font_FontStrikethrough,
		Font_FontStrikethrough_Description,
		Font_FontColor,
		Font_FontColor_Description,
		Font_FontBackgroundColor,
		Font_FontBackgroundColor_Description,
		Paragraph_AlignTop,
		Paragraph_AlignTop_Description,
		Paragraph_AlignMiddle,
		Paragraph_AlignMiddle_Description,
		Paragraph_AlignBottom,
		Paragraph_AlignBottom_Description,
		Paragraph_AlignLeft,
		Paragraph_AlignLeft_Description,
		Paragraph_AlignCenter,
		Paragraph_AlignCenter_Description,
		Paragraph_AlignRight,
		Paragraph_AlignRight_Description,
		Paragraph_Justify,
		Paragraph_Justify_Description,
		Tools_Pointer,
		Tools_Pointer_Description,
		Tools_Connector,
		Tools_Connector_Description,
		Tools_Text,
		Tools_Text_Description,
		Tools_Rectangle_Description,
		Tools_Ellipse_Description,
		Tools_RightTriangle_Description,
		Tools_Hexagon_Description,
		Show_Ruler,
		Show_Ruler_Description,
		Show_Grid,
		Show_Grid_Description,
		String_None,
		PageSize_Default,
		PageSize_Letter,
		PageSize_Legal,
		PageSize_A3,
		PageSize_A4,
		PageSize_A5,
		PageSize_Width,
		PageSize_Height,
		PageSize_SetSize,
		ShapeInfo_Width,
		ShapeInfo_Height,
		ShapeInfo_Angle,
		DiagramNotification_DefaultCaption,
		DefaultDocumentName,
		DocumentFormat_XmlFile,
		DocumentFormat_AllFiles,
		DocumentLoadErrorMessage,
		CreateRibbonDesignerActionListCommand,
		MessageAddCommandConstructorError,
		ConnectorType_Curved,
		ConnectorType_RightAngle,
		ConnectorType_Straight,
		PropertyGridView_Alphabetical,
		PropertyGridView_Categorized,
		PropertyGridOption_None,
	}
	#endregion
	#region DiagramControlLocalizer.AddStrings 
	public partial class DiagramControlLocalizer {
		void AddStrings() {
			AddString(DiagramControlStringId.ContextMenu_Cut, "Cut");
			AddString(DiagramControlStringId.ContextMenu_Copy, "Copy");
			AddString(DiagramControlStringId.ContextMenu_Paste, "Paste");
			AddString(DiagramControlStringId.ContextMenu_EditText, "Edit text");
			AddString(DiagramControlStringId.ContextMenu_View, "View");
			AddString(DiagramControlStringId.ContextMenu_Order, "Order");
			AddString(DiagramControlStringId.ContextMenu_Move_Up, "Move Up");
			AddString(DiagramControlStringId.ContextMenu_Move_Down, "Move Down");
			AddString(DiagramControlStringId.ContextMenu_Close, "Close");
			AddString(DiagramControlStringId.ContextMenu_Icons_And_Names, "Icons And Names");
			AddString(DiagramControlStringId.ContextMenu_Names_Under_Icons, "Names Under Icons");
			AddString(DiagramControlStringId.ContextMenu_Icons_Only, "Icons Only");
			AddString(DiagramControlStringId.ContextMenu_Names_Only, "Names Only");
			AddString(DiagramControlStringId.Menu_Undo_Button, "Undo");
			AddString(DiagramControlStringId.Menu_Undo_Button_Description, "Undo the last operation.");
			AddString(DiagramControlStringId.Menu_Redo_Button, "Redo");
			AddString(DiagramControlStringId.Menu_Redo_Button_Description, "Redo the last operation.");
			AddString(DiagramControlStringId.Menu_Delete_Button, "Delete");
			AddString(DiagramControlStringId.Menu_AllowLiveResizing_Button, "Live resizing");
			AddString(DiagramControlStringId.Menu_SnapToGrid_Button, "Snap to grid");
			AddString(DiagramControlStringId.Menu_SnapToItems_Button, "Snap to items");
			AddString(DiagramControlStringId.Menu_AllowEmptySelection_Button, "Empty selection");
			AddString(DiagramControlStringId.Menu_SnapDistance_Editor, "Snap distance");
			AddString(DiagramControlStringId.ToolDisplayFormat, "{0} tool");
			AddString(DiagramControlStringId.Tool_Pointer, "Pointer");
			AddString(DiagramControlStringId.Tool_Connector, "Connector");
			AddString(DiagramControlStringId.Tool_Container, "Container");
			AddString(DiagramControlStringId.MeasureUnit_Pixels, "Pixels");
			AddString(DiagramControlStringId.MeasureUnit_Inches, "Inches");
			AddString(DiagramControlStringId.MeasureUnit_Millimeters, "Millimeters");
			AddString(DiagramControlStringId.Layout_BringToFront, "Bring to Front");
			AddString(DiagramControlStringId.Layout_BringToFront_Description, "Bring the selected object in front of all other objects.");
			AddString(DiagramControlStringId.Layout_BringForward, "Bring Forward");
			AddString(DiagramControlStringId.Layout_SendToBack, "Send to Back");
			AddString(DiagramControlStringId.Layout_SendToBack_Description, "Send the selected object behind all other objects.");
			AddString(DiagramControlStringId.Layout_SendBackward, "Send Backward");
			AddString(DiagramControlStringId.Themes_Office_Name, "Office");
			AddString(DiagramControlStringId.Themes_Linear_Name, "Linear");
			AddString(DiagramControlStringId.Themes_NoTheme_Name, "No Theme");
			AddString(DiagramControlStringId.Themes_Integral_Name, "Integral");
			AddString(DiagramControlStringId.Themes_Daybreak_Name, "Daybreak");
			AddString(DiagramControlStringId.Themes_Parallel_Name, "Parallel");
			AddString(DiagramControlStringId.Themes_Sequence_Name, "Sequence");
			AddString(DiagramControlStringId.Themes_Lines_Name, "Lines");
			AddString(DiagramControlStringId.Themes_VariantStyleId_Name, "Variant {0}");
			AddString(DiagramControlStringId.Themes_ThemeStyleId_Name, "{0} Effect, Accent {1}");
			AddString(DiagramControlStringId.Themes_SubtleEffect_Name, "Subtle");
			AddString(DiagramControlStringId.Themes_RefinedEffect_Name, "Refined");
			AddString(DiagramControlStringId.Themes_BalancedEffect_Name, "Balanced");
			AddString(DiagramControlStringId.Themes_ModerateEffect_Name, "Moderate");
			AddString(DiagramControlStringId.Themes_FocusedEffect_Name, "Focused");
			AddString(DiagramControlStringId.Themes_IntenseEffect_Name, "Intense");
			AddString(DiagramControlStringId.Search_Shapes_Null_Text, "Search shapes...");
			AddString(DiagramControlStringId.QuickShapes_Name, "Quick Shapes");
			AddString(DiagramControlStringId.Stencils_Name, "STENCILS");
			AddString(DiagramControlStringId.No_Stencils_Open_Name, "There are no stencils open.");
			AddString(DiagramControlStringId.More_Shapes_Name, "More Shapes");
			AddString(DiagramControlStringId.Panel_Shapes_Name, "Shapes");
			AddString(DiagramControlStringId.Panel_Properties_Name, "Properties");
			AddString(DiagramControlStringId.BasicShapes_Name, "Basic Shapes");
			AddString(DiagramControlStringId.BasicShapes_Rectangle_Name, "Rectangle");
			AddString(DiagramControlStringId.BasicShapes_Ellipse_Name, "Ellipse");
			AddString(DiagramControlStringId.BasicShapes_Triangle_Name, "Triangle");
			AddString(DiagramControlStringId.BasicShapes_RightTriangle_Name, "Right Triangle");
			AddString(DiagramControlStringId.BasicShapes_Pentagon_Name, "Pentagon");
			AddString(DiagramControlStringId.BasicShapes_Hexagon_Name, "Hexagon");
			AddString(DiagramControlStringId.BasicShapes_Heptagon_Name, "Heptagon");
			AddString(DiagramControlStringId.BasicShapes_Octagon_Name, "Octagon");
			AddString(DiagramControlStringId.BasicShapes_Decagon_Name, "Decagon");
			AddString(DiagramControlStringId.BasicShapes_Can_Name, "Can");
			AddString(DiagramControlStringId.BasicShapes_Parallelogram_Name, "Parallelogram");
			AddString(DiagramControlStringId.BasicShapes_Trapezoid_Name, "Trapezoid");
			AddString(DiagramControlStringId.BasicShapes_Diamond_Name, "Diamond");
			AddString(DiagramControlStringId.BasicShapes_Cross_Name, "Cross");
			AddString(DiagramControlStringId.BasicShapes_Chevron_Name, "Chevron");
			AddString(DiagramControlStringId.BasicShapes_Cube_Name, "Cube");
			AddString(DiagramControlStringId.BasicShapes_Star4_Name, "4-Point Star");
			AddString(DiagramControlStringId.BasicShapes_Star5_Name, "5-Point Star");
			AddString(DiagramControlStringId.BasicShapes_Star6_Name, "6-Point Star");
			AddString(DiagramControlStringId.BasicShapes_Star7_Name, "7-Point Star");
			AddString(DiagramControlStringId.BasicShapes_Star16_Name, "16-Point Star");
			AddString(DiagramControlStringId.BasicShapes_Star24_Name, "24-Point Star");
			AddString(DiagramControlStringId.BasicShapes_Star32_Name, "32-Point Star");
			AddString(DiagramControlStringId.BasicShapes_RoundedRectangle_Name, "Rounded Rectangle");
			AddString(DiagramControlStringId.BasicShapes_SingleSnipCornerRectangle_Name, "Single Snip Corner Rectangle");
			AddString(DiagramControlStringId.BasicShapes_SnipSameSideCornerRectangle_Name, "Snip Same Side Corner Rectangle");
			AddString(DiagramControlStringId.BasicShapes_SnipDiagonalCornerRectangle_Name, "Snip Diagonal Corner Rectangle");
			AddString(DiagramControlStringId.BasicShapes_SingleRoundCornerRectangle_Name, "Single Round Corner Rectangle");
			AddString(DiagramControlStringId.BasicShapes_RoundSameSideCornerRectangle_Name, "Round Same Side Corner Rectangle");
			AddString(DiagramControlStringId.BasicShapes_RoundDiagonalCornerRectangle_Name, "Round Diagonal Corner Rectangle");
			AddString(DiagramControlStringId.BasicShapes_SnipAndRoundSingleCornerRectangle_Name, "Snip And Round Single Corner Rectangle");
			AddString(DiagramControlStringId.BasicShapes_SnipCornerRectangle_Name, "Snip Corner Rectangle");
			AddString(DiagramControlStringId.BasicShapes_RoundCornerRectangle_Name, "Round Corner Rectangle");
			AddString(DiagramControlStringId.BasicShapes_SnipAndRoundCornerRectangle_Name, "Snip And Round Corner Rectangle");
			AddString(DiagramControlStringId.BasicShapes_Plaque_Name, "Plaque");
			AddString(DiagramControlStringId.BasicShapes_Frame_Name, "Frame");
			AddString(DiagramControlStringId.BasicShapes_FrameCorner_Name, "Frame Corner");
			AddString(DiagramControlStringId.BasicShapes_LShape_Name, "L Shape");
			AddString(DiagramControlStringId.BasicShapes_DiagonalStripe_Name, "Diagonal Stripe");
			AddString(DiagramControlStringId.BasicShapes_Donut_Name, "Donut");
			AddString(DiagramControlStringId.BasicShapes_NoSymbol_Name, "NoSymbol");
			AddString(DiagramControlStringId.BasicShapes_LeftParenthesis_Name, "Left Parenthesis");
			AddString(DiagramControlStringId.BasicShapes_RightParenthesis_Name, "Right Parenthesis");
			AddString(DiagramControlStringId.BasicShapes_LeftBrace_Name, "Left Brace");
			AddString(DiagramControlStringId.BasicShapes_RightBrace_Name, "Right Brace");
			AddString(DiagramControlStringId.BasicFlowchartShapes_Name, "Basic Flowchart Shapes");
			AddString(DiagramControlStringId.BasicFlowchartShapes_Process_Name, "Process");
			AddString(DiagramControlStringId.BasicFlowchartShapes_Decision_Name, "Decision");
			AddString(DiagramControlStringId.BasicFlowchartShapes_Subprocess_Name, "Subprocess");
			AddString(DiagramControlStringId.BasicFlowchartShapes_StartEnd_Name, "Start/End");
			AddString(DiagramControlStringId.BasicFlowchartShapes_Document_Name, "Document");
			AddString(DiagramControlStringId.BasicFlowchartShapes_Data_Name, "Data");
			AddString(DiagramControlStringId.BasicFlowchartShapes_Database_Name, "Database");
			AddString(DiagramControlStringId.BasicFlowchartShapes_ExternalData_Name, "External Data");
			AddString(DiagramControlStringId.BasicFlowchartShapes_Custom1_Name, "Custom1");
			AddString(DiagramControlStringId.BasicFlowchartShapes_Custom2_Name, "Custom2");
			AddString(DiagramControlStringId.BasicFlowchartShapes_Custom3_Name, "Custom3");
			AddString(DiagramControlStringId.BasicFlowchartShapes_Custom4_Name, "Custom4");
			AddString(DiagramControlStringId.BasicFlowchartShapes_OnPageReference_Name, "On-page reference");
			AddString(DiagramControlStringId.BasicFlowchartShapes_OffPageReference_Name, "Off-page reference");
			AddString(DiagramControlStringId.SDLDiagramShapes_Name, "SDL Diagram Shapes");
			AddString(DiagramControlStringId.SDLDiagramShapes_Start_Name, "Start");
			AddString(DiagramControlStringId.SDLDiagramShapes_VariableStart_Name, "Variable Start");
			AddString(DiagramControlStringId.SDLDiagramShapes_Procedure_Name, "Procedure");
			AddString(DiagramControlStringId.SDLDiagramShapes_VariableProcedure_Name, "Variable Procedure");
			AddString(DiagramControlStringId.SDLDiagramShapes_CreateRequest_Name, "CreateRequest");
			AddString(DiagramControlStringId.SDLDiagramShapes_Alternative_Name, "Alternative");
			AddString(DiagramControlStringId.SDLDiagramShapes_Return_Name, "Return");
			AddString(DiagramControlStringId.SDLDiagramShapes_Decision1_Name, "Decision1");
			AddString(DiagramControlStringId.SDLDiagramShapes_MessageFromUser_Name, "Message from user");
			AddString(DiagramControlStringId.SDLDiagramShapes_PrimitiveFromCallControl_Name, "Primitive from call control");
			AddString(DiagramControlStringId.SDLDiagramShapes_Decision2_Name, "Decision2");
			AddString(DiagramControlStringId.SDLDiagramShapes_MessageToUser_Name, "Message to user");
			AddString(DiagramControlStringId.SDLDiagramShapes_PrimitiveToCallControl_Name, "Primitive to call control");
			AddString(DiagramControlStringId.SDLDiagramShapes_Save_Name, "Save");
			AddString(DiagramControlStringId.SDLDiagramShapes_OnPageReference_Name, "On page reference");
			AddString(DiagramControlStringId.SDLDiagramShapes_OffPageReference_Name, "Off page reference");
			AddString(DiagramControlStringId.SDLDiagramShapes_Document_Name, "Document");
			AddString(DiagramControlStringId.SDLDiagramShapes_DiskStorage_Name, "Disk storage");
			AddString(DiagramControlStringId.SDLDiagramShapes_DividedProcess_Name, "Divided process");
			AddString(DiagramControlStringId.SDLDiagramShapes_DividedEvent_Name, "Divided event");
			AddString(DiagramControlStringId.SDLDiagramShapes_Terminator_Name, "Terminator");
			AddString(DiagramControlStringId.SoftwareIcons_Name, "Software Icons");
			AddString(DiagramControlStringId.SoftwareIcons_Back_Name, "Back");
			AddString(DiagramControlStringId.SoftwareIcons_Forward_Name, "Forward");
			AddString(DiagramControlStringId.SoftwareIcons_Expand_Name, "Expand");
			AddString(DiagramControlStringId.SoftwareIcons_Collapse_Name, "Collapse");
			AddString(DiagramControlStringId.SoftwareIcons_Add_Name, "Add");
			AddString(DiagramControlStringId.SoftwareIcons_Remove_Name, "Remove");
			AddString(DiagramControlStringId.SoftwareIcons_ZoomIn_Name, "ZoomIn");
			AddString(DiagramControlStringId.SoftwareIcons_ZoomOut_Name, "ZoomOut");
			AddString(DiagramControlStringId.SoftwareIcons_Lock_Name, "Lock");
			AddString(DiagramControlStringId.SoftwareIcons_Permission_Name, "Permission");
			AddString(DiagramControlStringId.SoftwareIcons_Sort_Name, "Sort");
			AddString(DiagramControlStringId.SoftwareIcons_Filter_Name, "Filter");
			AddString(DiagramControlStringId.SoftwareIcons_Tools_Name, "Tools");
			AddString(DiagramControlStringId.SoftwareIcons_Properties_Name, "Properties");
			AddString(DiagramControlStringId.SoftwareIcons_Calendar_Name, "Calendar");
			AddString(DiagramControlStringId.SoftwareIcons_Document_Name, "Document");
			AddString(DiagramControlStringId.SoftwareIcons_Database_Name, "Database");
			AddString(DiagramControlStringId.SoftwareIcons_HardDrive_Name, "HardDrive");
			AddString(DiagramControlStringId.SoftwareIcons_Network_Name, "Network");
			AddString(DiagramControlStringId.DecorativeShapes_Name, "Decorative Shapes");
			AddString(DiagramControlStringId.DecorativeShapes_LightningBolt_Name, "Lightning Bolt");
			AddString(DiagramControlStringId.DecorativeShapes_Moon_Name, "Moon");
			AddString(DiagramControlStringId.DecorativeShapes_Wave_Name, "Wave");
			AddString(DiagramControlStringId.DecorativeShapes_DoubleWave_Name, "Double Wave");
			AddString(DiagramControlStringId.DecorativeShapes_VerticalScroll_Name, "Vertical Scroll");
			AddString(DiagramControlStringId.DecorativeShapes_HorizontalScroll_Name, "Horizontal Scroll");
			AddString(DiagramControlStringId.DecorativeShapes_Heart_Name, "Heart");
			AddString(DiagramControlStringId.DecorativeShapes_DownRibbon_Name, "Down Ribbon");
			AddString(DiagramControlStringId.DecorativeShapes_UpRibbon_Name, "Up Ribbon");
			AddString(DiagramControlStringId.DecorativeShapes_Cloud_Name, "Cloud");
			AddString(DiagramControlStringId.ArrowShapes_Name, "Arrow Shapes");
			AddString(DiagramControlStringId.ArrowShapes_SimpleArrow_Name, "Simple Arrow");
			AddString(DiagramControlStringId.ArrowShapes_SimpleDoubleArrow_Name, "Simple Double Arrow");
			AddString(DiagramControlStringId.ArrowShapes_ModernArrow_Name, "Modern Arrow");
			AddString(DiagramControlStringId.ArrowShapes_FlexibleArrow_Name, "Flexible Arrow");
			AddString(DiagramControlStringId.ArrowShapes_BentArrow_Name, "Bent Arrow");
			AddString(DiagramControlStringId.ArrowShapes_UTurnArrow_Name, "U Turn Arrow");
			AddString(DiagramControlStringId.ArrowShapes_SharpBentArrow_Name, "Sharp Bent Arrow");
			AddString(DiagramControlStringId.ArrowShapes_CurvedRightArrow_Name, "Curved Right Arrow");
			AddString(DiagramControlStringId.ArrowShapes_CurvedLeftArrow_Name, "Curved Left Arrow");
			AddString(DiagramControlStringId.ArrowShapes_NotchedArrow_Name, "Notched Arrow");
			AddString(DiagramControlStringId.ArrowShapes_StripedArrow_Name, "Striped Arrow");
			AddString(DiagramControlStringId.ArrowShapes_BlockArrow_Name, "Block Arrow");
			AddString(DiagramControlStringId.ArrowShapes_CircularArrow_Name, "Circular Arrow");
			AddString(DiagramControlStringId.ArrowShapes_QuadArrow_Name, "Quad Arrow");
			AddString(DiagramControlStringId.ArrowShapes_LeftRightUpArrow_Name, "Left Right Up Arrow");
			AddString(DiagramControlStringId.ArrowShapes_LeftRightArrowBlock_Name, "Left Right Arrow Block");
			AddString(DiagramControlStringId.ArrowShapes_QuadArrowBlock_Name, "Quad Arrow Block");
			AddString(DiagramControlStringId.Arrow_Open90, "Open 90 arrow");
			AddString(DiagramControlStringId.Arrow_Filled90, "Filled 90 arrow");
			AddString(DiagramControlStringId.Arrow_ClosedDot, "Closed dot");
			AddString(DiagramControlStringId.Arrow_FilledDot, "Filled dot");
			AddString(DiagramControlStringId.Arrow_OpenFletch, "Open fletch");
			AddString(DiagramControlStringId.Arrow_FilledFletch, "Filled fletch");
			AddString(DiagramControlStringId.Arrow_Diamond, "Diamond");
			AddString(DiagramControlStringId.Arrow_FilledDiamond, "Filled diamond");
			AddString(DiagramControlStringId.Arrow_ClosedDiamond, "Closed diamond");
			AddString(DiagramControlStringId.Arrow_IndentedFilledArrow, "Indented filled arrow");
			AddString(DiagramControlStringId.Arrow_OutdentedFilledArrow, "Outdented filled arrow");
			AddString(DiagramControlStringId.Arrow_FilledSquare, "Filled square");
			AddString(DiagramControlStringId.Arrow_ClosedASMEArrow, "Closed ASME arrow");
			AddString(DiagramControlStringId.Arrow_FilledDoubleArrow, "Filled double arrow");
			AddString(DiagramControlStringId.Arrow_ClosedDoubleArrow, "Closed double arrow");
			AddString(DiagramControlStringId.RibbonPage_Home, "Home");
			AddString(DiagramControlStringId.RibbonPageGroup_Action, "Action");
			AddString(DiagramControlStringId.RibbonPageGroup_Toolbox, "Toolbox");
			AddString(DiagramControlStringId.RibbonPageGroup_Document, "Document");
			AddString(DiagramControlStringId.RibbonPageGroup_File, "File");
			AddString(DiagramControlStringId.RibbonPageGroup_Clipboard, "Clipboard");
			AddString(DiagramControlStringId.RibbonPageGroup_Options, "Options");
			AddString(DiagramControlStringId.RibbonPageGroup_Font, "Font");
			AddString(DiagramControlStringId.RibbonPageGroup_Paragraph, "Paragraph");
			AddString(DiagramControlStringId.RibbonPageGroup_Tools, "Tools");
			AddString(DiagramControlStringId.RibbonPage_View, "View");
			AddString(DiagramControlStringId.RibbonPageGroup_Show, "Show");
			AddString(DiagramControlStringId.RibbonPageGroup_Appearance, "Appearance");
			AddString(DiagramControlStringId.RibbonPageGroup_Arrange, "Arrange");
			AddString(DiagramControlStringId.RibbonPageGroup_PageSetup, "Page Setup");
			AddString(DiagramControlStringId.RibbonPageGroup_Themes, "Themes");
			AddString(DiagramControlStringId.RibbonPageGroup_ShapeStyles, "Shape styles");
			AddString(DiagramControlStringId.RibbonPageGroup_TreeLayout, "Layout");
			AddString(DiagramControlStringId.RibbonPageGroup_UserInterfaceThemes, "UI Themes");
			AddString(DiagramControlStringId.RibbonPage_Design, "Design");
			AddString(DiagramControlStringId.RibbonGallery_VariantStyles, "Variant Styles");
			AddString(DiagramControlStringId.RibbonGallery_ThemeStyles, "Theme Styles");
			AddString(DiagramControlStringId.Shape_BackgroundColor, "Background");
			AddString(DiagramControlStringId.Shape_ForegroundColor, "Foreground");
			AddString(DiagramControlStringId.Shape_StrokeColor, "Stroke");
			AddString(DiagramControlStringId.TreeLayout_ReLayoutPage, "Re-Layout Page");
			AddString(DiagramControlStringId.TreeLayout_BufferSize, "Buffer size");
			AddString(DiagramControlStringId.TreeLayout_RightToLeft, "Right To Left");
			AddString(DiagramControlStringId.TreeLayout_BottomToTop, "Bottom To Top");
			AddString(DiagramControlStringId.TreeLayout_LeftToRight, "Left To Right");
			AddString(DiagramControlStringId.TreeLayout_TopToBottom, "Top To Bottom");
			AddString(DiagramControlStringId.Ribbon_PageSize, "Size");
			AddString(DiagramControlStringId.Ribbon_MeasureUnit, "Measure unit");
			AddString(DiagramControlStringId.Ribbon_GridSize, "Grid size");
			AddString(DiagramControlStringId.Ribbon_CanvasAutoSize, "Auto Size");
			AddString(DiagramControlStringId.Ribbon_CanvasNoneAutoSize, "None");
			AddString(DiagramControlStringId.Ribbon_CanvasAutoSizeDescription, "Adjust the page size on moving elements outside of its borders.");
			AddString(DiagramControlStringId.Ribbon_CanvasNoneAutoSizeDescription, "The page size is not changed on moving elements outside of its borders.");
			AddString(DiagramControlStringId.Ribbon_Orientation, "Orientation");
			AddString(DiagramControlStringId.Ribbon_VerticalOrientation, "Portrait");
			AddString(DiagramControlStringId.Ribbon_HorizontalOrientation, "Landscape");
			AddString(DiagramControlStringId.DiagramCommand_Paste, "Paste");
			AddString(DiagramControlStringId.DiagramCommand_Paste_Description, "Add content on the Clipboard to your document.");
			AddString(DiagramControlStringId.DiagramCommand_Cut, "Cut");
			AddString(DiagramControlStringId.DiagramCommand_Cut_Description, "Remove the selection and put it on the Clipboard so you can paste it somewhere else.");
			AddString(DiagramControlStringId.DiagramCommand_Copy, "Copy");
			AddString(DiagramControlStringId.DiagramCommand_Copy_Description, "Put a copy of the selection on the Clipboard so you can paste it somewhere else.");
			AddString(DiagramControlStringId.DiagramCommand_Open, "Open");
			AddString(DiagramControlStringId.DiagramCommand_Open_Description, "Open a diagram file");
			AddString(DiagramControlStringId.DiagramCommand_Save, "Save");
			AddString(DiagramControlStringId.DiagramCommand_Save_Description, "Save this diagram");
			AddString(DiagramControlStringId.DiagramCommand_SaveAs, "Save As");
			AddString(DiagramControlStringId.DiagramCommand_SaveAs_Description, "Save this diagram with a different name");
			AddString(DiagramControlStringId.DiagramCommand_New, "New");
			AddString(DiagramControlStringId.DiagramCommand_New_Description, "Create a new diagram");
			AddString(DiagramControlStringId.DiagramDesigner, "Diagram Designer");
			AddString(DiagramControlStringId.DiagramDesigner_SaveChangesConfirmation, "Do you want to apply the changes ?");
			AddString(DiagramControlStringId.Font_Font, "Font");
			AddString(DiagramControlStringId.Font_Font_Description, "Pick a new font for your text.");
			AddString(DiagramControlStringId.Font_FontSize, "Font Size");
			AddString(DiagramControlStringId.Font_FontSize_Description, "Change the size of your text.");
			AddString(DiagramControlStringId.Font_FontSizeIncrease, "Increase Font Size");
			AddString(DiagramControlStringId.Font_FontSizeIncrease_Description, "Make your text a bit bigger.");
			AddString(DiagramControlStringId.Font_FontSizeDecrease, "Decrease Font Size");
			AddString(DiagramControlStringId.Font_FontSizeDecrease_Description, "Make your text a bit smaller.");
			AddString(DiagramControlStringId.Font_FontBold, "Bold");
			AddString(DiagramControlStringId.Font_FontBold_Description, "Make your text bold.");
			AddString(DiagramControlStringId.Font_FontItalic, "Italic");
			AddString(DiagramControlStringId.Font_FontItalic_Description, "Italicize your text.");
			AddString(DiagramControlStringId.Font_FontUnderline, "Underline");
			AddString(DiagramControlStringId.Font_FontUnderline_Description, "Underline your text.");
			AddString(DiagramControlStringId.Font_FontStrikethrough, "Strikethrough");
			AddString(DiagramControlStringId.Font_FontStrikethrough_Description, "Cross something out by drawing a line through it.");
			AddString(DiagramControlStringId.Font_FontColor, "Font Color");
			AddString(DiagramControlStringId.Font_FontColor_Description, "Change the color of your text.");
			AddString(DiagramControlStringId.Font_FontBackgroundColor, "Font Background Color");
			AddString(DiagramControlStringId.Font_FontBackgroundColor_Description, "Change the background color.");
			AddString(DiagramControlStringId.Paragraph_AlignTop, "Align Top");
			AddString(DiagramControlStringId.Paragraph_AlignTop_Description, "Align text to the top of the text block.");
			AddString(DiagramControlStringId.Paragraph_AlignMiddle, "Align Middle");
			AddString(DiagramControlStringId.Paragraph_AlignMiddle_Description, "Align text so that it is centered between the top and bottom of the text block.");
			AddString(DiagramControlStringId.Paragraph_AlignBottom, "Align Bottom");
			AddString(DiagramControlStringId.Paragraph_AlignBottom_Description, "Align text to the bottom of the text block.");
			AddString(DiagramControlStringId.Paragraph_AlignLeft, "Align Left");
			AddString(DiagramControlStringId.Paragraph_AlignLeft_Description, "Align your content to the left.");
			AddString(DiagramControlStringId.Paragraph_AlignCenter, "Align Center");
			AddString(DiagramControlStringId.Paragraph_AlignCenter_Description, "Center your content.");
			AddString(DiagramControlStringId.Paragraph_AlignRight, "Align Right");
			AddString(DiagramControlStringId.Paragraph_AlignRight_Description, "Align your content to the right.");
			AddString(DiagramControlStringId.Paragraph_Justify, "Justify");
			AddString(DiagramControlStringId.Paragraph_Justify_Description, "Distribute your text evenly between the margins.");
			AddString(DiagramControlStringId.Tools_Pointer, "Pointer tool");
			AddString(DiagramControlStringId.Tools_Pointer_Description, "Select, move, and resize objects.");
			AddString(DiagramControlStringId.Tools_Connector, "Connector");
			AddString(DiagramControlStringId.Tools_Connector_Description, "Draw connectors between objects.");
			AddString(DiagramControlStringId.Tools_Text, "Text");
			AddString(DiagramControlStringId.Tools_Text_Description, "Add a text shape or select existing text.");
			AddString(DiagramControlStringId.Tools_Rectangle_Description, "Drag to draw a rectangle.");
			AddString(DiagramControlStringId.Tools_Ellipse_Description, "Drag to draw an ellipse.");
			AddString(DiagramControlStringId.Tools_RightTriangle_Description, "Drag to draw a right triangle.");
			AddString(DiagramControlStringId.Tools_Hexagon_Description, "Drag to draw a hexagon.");
			AddString(DiagramControlStringId.Show_Ruler, "Ruler");
			AddString(DiagramControlStringId.Show_Ruler_Description, "View the rulers used to measure and line up objects in the document.");
			AddString(DiagramControlStringId.Show_Grid, "Grid");
			AddString(DiagramControlStringId.Show_Grid_Description, "The gridlines make it easy for to you align objects with text, other objects, or a particular spot");
			AddString(DiagramControlStringId.String_None, "none");
			AddString(DiagramControlStringId.PageSize_Default, "Default");
			AddString(DiagramControlStringId.PageSize_Letter, "Letter");
			AddString(DiagramControlStringId.PageSize_Legal, "Legal");
			AddString(DiagramControlStringId.PageSize_A3, "A3");
			AddString(DiagramControlStringId.PageSize_A4, "A4");
			AddString(DiagramControlStringId.PageSize_A5, "A5");
			AddString(DiagramControlStringId.PageSize_Width, "Width");
			AddString(DiagramControlStringId.PageSize_Height, "Height");
			AddString(DiagramControlStringId.PageSize_SetSize, "Set Size");
			AddString(DiagramControlStringId.ShapeInfo_Width, "Width");
			AddString(DiagramControlStringId.ShapeInfo_Height, "Height");
			AddString(DiagramControlStringId.ShapeInfo_Angle, "Angle");
			AddString(DiagramControlStringId.DiagramNotification_DefaultCaption, "Diagrams");
			AddString(DiagramControlStringId.DefaultDocumentName, "Document");
			AddString(DiagramControlStringId.DocumentFormat_XmlFile, "Xml files");
			AddString(DiagramControlStringId.DocumentFormat_AllFiles, "All files");
			AddString(DiagramControlStringId.DocumentLoadErrorMessage, "The specified file cannot be loaded");
			AddString(DiagramControlStringId.CreateRibbonDesignerActionListCommand, "Create Ribbon Menu");
			AddString(DiagramControlStringId.MessageAddCommandConstructorError, "Cannot find a constructor with a DiagramControl type parameter in the {0} class");
			AddString(DiagramControlStringId.ConnectorType_Curved, "Curved");
			AddString(DiagramControlStringId.ConnectorType_RightAngle, "RightAngle");
			AddString(DiagramControlStringId.ConnectorType_Straight, "Straight");
			AddString(DiagramControlStringId.PropertyGridView_Alphabetical, "Alphabetical");
			AddString(DiagramControlStringId.PropertyGridView_Categorized, "Categorized");
			AddString(DiagramControlStringId.PropertyGridOption_None, "(none)");
		}
	}
	 #endregion
}
