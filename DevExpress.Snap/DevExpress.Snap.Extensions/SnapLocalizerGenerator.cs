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
namespace DevExpress.Snap.Extensions.Localization {
	#region enum SnapExtensionsStringId
	public enum SnapExtensionsStringId {
		BarCodeSmartTagItem_Symbology,
		BarCodeSmartTagItem_Data,
		BarCodeSmartTagItem_Module,
		BarCodeSmartTagItem_AutoModule,
		BarCodeSmartTagItem_ShowData,
		BarCodeSmartTagItem_Alignment,
		BarCodeSmartTagItem_TextAlignment,
		BarCodeSmartTagItem_Orientation,
		ImageSmartTagItem_Sizing,
		ImageSmartTagItem_UpdateMode,
		FieldListDockPanel_Text,
		Caption_Data,
		Caption_SNList,
		Caption_SNListDesign,
		Caption_SNMergeField,
		Caption_SNMergeFieldDesign,
		Caption_Group,
		Caption_GroupDesign,
		Caption_Appearance,
		GalleryGroupCaption_Regular,
		GalleryGroupCaption_Custom,
		ThemesBar_Text,
		ThemesRibbonPageGroup_Text,
		SnapBarToolbarsListItem_Caption,
		SnapBarToolbarsListItem_Hint,
		ViewBar_Text,
		ViewRibbonPageGroup_Text,
		ViewFieldsRibbonPageGroup_Text,
		MailMergeRibbonPageGroup_Text,
		GroupingRibbonPageGroup_Text,
		ListHeaderAndFooterRibbonPageGroup_Text,
		ReportExplorerDockPanel_Text,
		Msg_ContainsIllegalSymbols,
		Msg_Error,
		SaveThemeBarItem_Caption,
		LoadThemeBarItem_Caption,
		RestoreDefaultsBarItem_Caption,
		UpdateToMatchDocumentStylesBarItem_Caption,
		RestoreDefaultDocumentStylesBarItem_Caption,
		RemoveThemeBarItem_Caption,
		SNTextFieldTagItem_TextFormat,
		SNTextFieldTagItem_KeepLastParagraph,
		SparklineSmartTagItem_ViewType,
		SparklineSmartTagItem_Width,
		SparklineSmartTagItem_Height,
		SparklineSmartTagItem_HighlightMaxPoint,
		SparklineSmartTagItem_HighlightMinPoint,
		SparklineSmartTagItem_HighlightStartPoint,
		SparklineSmartTagItem_HighlightEndPoint,
		SparklineSmartTagItem_Color,
		SparklineSmartTagItem_MaxPointColor,
		SparklineSmartTagItem_MinPointColor,
		SparklineSmartTagItem_StartPointColor,
		SparklineSmartTagItem_EndPointColor,
		SparklineSmartTagItem_NegativePointColor,
		SparklineSmartTagItem_LineWidth,
		SparklineSmartTagItem_HighlightNegativePoints,
		SparklineSmartTagItem_ShowMarkers,
		SparklineSmartTagItem_MarkerSize,
		SparklineSmartTagItem_MaxPointMarkerSize,
		SparklineSmartTagItem_MinPointMarkerSize,
		SparklineSmartTagItem_StartPointMarkerSize,
		SparklineSmartTagItem_EndPointMarkerSize,
		SparklineSmartTagItem_NegativePointMarkerSize,
		SparklineSmartTagItem_MarkerColor,
		SparklineSmartTagItem_AreaOpacity,
		SparklineSmartTagItem_BarDistance,
		IndexSmartTagItem_GroupIndexMode,
		AddDataSource,
		RemoveDataSource,
		UpdateDataSource,
		ConfigureConnection,
		ManageRelations,
		MailMergeDataSource,
		ManageQueries,
		RebuildResultSchema,
		EditDataSource,
		Configure,
		SortingCollectionEditor_SortBy,
		ActionList_ContentType,
		ActionList_Binding,
		ActionList_EnableEmptyFieldDataAlias,
		ActionList_EmptyFieldDataAlias,
		ActionList_FormatString,
		ActionList_Summary,
		ActionList_Text,
		ActionList_ScreenTip,
		ActionList_TargetFrame,
		ActionList_Checked,
		ParametersErrorDialogCaption,
		ParametersErrorDialogTextInvalidCharacters,
		ParametersErrorDialogTextNoName,
		ParametersErrorDialogTextIdenticalNames,
		MailMergeCurrentRecordEdit_PreviousButtonTooltip,
		MailMergeCurrentRecordEdit_NextButtonTooltip,
		MailMergeCurrentRecordEdit_FirstButtonTooltip,
		MailMergeCurrentRecordEdit_LastButtonTooltip,
		FinishAndMergePageGroup_DefaultText,
		DataShapingPageGroup_DefaultText,
		ToolboxPageGroup_DefaultText,
		SNMergeFieldPropertiesRibbonPageGroup_DefaultText,
		DataRibbonPageGroup_DefaultText,
		MailMergeCurrentRecordRibbonPageGroup_DefaultText,
		MailMergeRibbonPage_DefaultText,
		ListCommandsRibbonPageGroup_DefaultText,
		ListEditorRowLimitRibbonPageGroup_DefaultText,
		TableCellStylesRibbonPageGroup_DefaultText,
	}
	#endregion
	#region SnapExtensionsLocalizer.AddStrings 
	public partial class SnapExtensionsLocalizer {
		void AddStrings() {
			AddString(SnapExtensionsStringId.BarCodeSmartTagItem_Symbology, "Symbology");
			AddString(SnapExtensionsStringId.BarCodeSmartTagItem_Data, "Data");
			AddString(SnapExtensionsStringId.BarCodeSmartTagItem_Module, "Module");
			AddString(SnapExtensionsStringId.BarCodeSmartTagItem_AutoModule, "Auto-Module");
			AddString(SnapExtensionsStringId.BarCodeSmartTagItem_ShowData, "Show Data");
			AddString(SnapExtensionsStringId.BarCodeSmartTagItem_Alignment, "Alignment");
			AddString(SnapExtensionsStringId.BarCodeSmartTagItem_TextAlignment, "Text Alignment");
			AddString(SnapExtensionsStringId.BarCodeSmartTagItem_Orientation, "Orientation");
			AddString(SnapExtensionsStringId.ImageSmartTagItem_Sizing, "Sizing");
			AddString(SnapExtensionsStringId.ImageSmartTagItem_UpdateMode, "Update Mode");
			AddString(SnapExtensionsStringId.FieldListDockPanel_Text, "Data Explorer");
			AddString(SnapExtensionsStringId.Caption_Data, "Data Tools");
			AddString(SnapExtensionsStringId.Caption_SNList, "List Tools");
			AddString(SnapExtensionsStringId.Caption_SNListDesign, "List");
			AddString(SnapExtensionsStringId.Caption_SNMergeField, "Field Tools");
			AddString(SnapExtensionsStringId.Caption_SNMergeFieldDesign, "Field");
			AddString(SnapExtensionsStringId.Caption_Group, "Group Tools");
			AddString(SnapExtensionsStringId.Caption_GroupDesign, "Group");
			AddString(SnapExtensionsStringId.Caption_Appearance, "Appearance");
			AddString(SnapExtensionsStringId.GalleryGroupCaption_Regular, "Regular");
			AddString(SnapExtensionsStringId.GalleryGroupCaption_Custom, "Custom");
			AddString(SnapExtensionsStringId.ThemesBar_Text, "Themes");
			AddString(SnapExtensionsStringId.ThemesRibbonPageGroup_Text, "Report Themes");
			AddString(SnapExtensionsStringId.SnapBarToolbarsListItem_Caption, "&Windows");
			AddString(SnapExtensionsStringId.SnapBarToolbarsListItem_Hint, "Show or hide the Data Explorer and Report Explorer windows.");
			AddString(SnapExtensionsStringId.ViewBar_Text, "View");
			AddString(SnapExtensionsStringId.ViewRibbonPageGroup_Text, "View");
			AddString(SnapExtensionsStringId.ViewFieldsRibbonPageGroup_Text, "Fields");
			AddString(SnapExtensionsStringId.MailMergeRibbonPageGroup_Text, "Data");
			AddString(SnapExtensionsStringId.GroupingRibbonPageGroup_Text, "Layout");
			AddString(SnapExtensionsStringId.ListHeaderAndFooterRibbonPageGroup_Text, "Layout");
			AddString(SnapExtensionsStringId.ReportExplorerDockPanel_Text, "Report Explorer");
			AddString(SnapExtensionsStringId.Msg_ContainsIllegalSymbols, "Input format string contains illegal symbol(s).");
			AddString(SnapExtensionsStringId.Msg_Error, "Error");
			AddString(SnapExtensionsStringId.SaveThemeBarItem_Caption, "Save the Current Theme to a File...");
			AddString(SnapExtensionsStringId.LoadThemeBarItem_Caption, "Load a Theme from a File...");
			AddString(SnapExtensionsStringId.RestoreDefaultsBarItem_Caption, "Restore Defaults");
			AddString(SnapExtensionsStringId.UpdateToMatchDocumentStylesBarItem_Caption, "Update to Match the Document Styles");
			AddString(SnapExtensionsStringId.RestoreDefaultDocumentStylesBarItem_Caption, "Restore the Default Document Styles");
			AddString(SnapExtensionsStringId.RemoveThemeBarItem_Caption, "Remove");
			AddString(SnapExtensionsStringId.SNTextFieldTagItem_TextFormat, "Text Format");
			AddString(SnapExtensionsStringId.SNTextFieldTagItem_KeepLastParagraph, "Keep Last Paragraph");
			AddString(SnapExtensionsStringId.SparklineSmartTagItem_ViewType, "View Type");
			AddString(SnapExtensionsStringId.SparklineSmartTagItem_Width, "Width");
			AddString(SnapExtensionsStringId.SparklineSmartTagItem_Height, "Height");
			AddString(SnapExtensionsStringId.SparklineSmartTagItem_HighlightMaxPoint, "Highlight Max Point");
			AddString(SnapExtensionsStringId.SparklineSmartTagItem_HighlightMinPoint, "Highlight Min Point");
			AddString(SnapExtensionsStringId.SparklineSmartTagItem_HighlightStartPoint, "Highlight Start Point");
			AddString(SnapExtensionsStringId.SparklineSmartTagItem_HighlightEndPoint, "Highlight End Point");
			AddString(SnapExtensionsStringId.SparklineSmartTagItem_Color, "Color");
			AddString(SnapExtensionsStringId.SparklineSmartTagItem_MaxPointColor, "Max Point Color");
			AddString(SnapExtensionsStringId.SparklineSmartTagItem_MinPointColor, "Min Point Color");
			AddString(SnapExtensionsStringId.SparklineSmartTagItem_StartPointColor, "Start Point Color");
			AddString(SnapExtensionsStringId.SparklineSmartTagItem_EndPointColor, "End Point Color");
			AddString(SnapExtensionsStringId.SparklineSmartTagItem_NegativePointColor, "Negative Point Color");
			AddString(SnapExtensionsStringId.SparklineSmartTagItem_LineWidth, "Line Width");
			AddString(SnapExtensionsStringId.SparklineSmartTagItem_HighlightNegativePoints, "Highlight Negative Points");
			AddString(SnapExtensionsStringId.SparklineSmartTagItem_ShowMarkers, "Show Markers");
			AddString(SnapExtensionsStringId.SparklineSmartTagItem_MarkerSize, "Marker Size");
			AddString(SnapExtensionsStringId.SparklineSmartTagItem_MaxPointMarkerSize, "Max Point Marker Size");
			AddString(SnapExtensionsStringId.SparklineSmartTagItem_MinPointMarkerSize, "Min Point Marker Size");
			AddString(SnapExtensionsStringId.SparklineSmartTagItem_StartPointMarkerSize, "Start Point Marker Size");
			AddString(SnapExtensionsStringId.SparklineSmartTagItem_EndPointMarkerSize, "End Point Marker Size");
			AddString(SnapExtensionsStringId.SparklineSmartTagItem_NegativePointMarkerSize, "Negative Point Marker Size");
			AddString(SnapExtensionsStringId.SparklineSmartTagItem_MarkerColor, "Marker Color");
			AddString(SnapExtensionsStringId.SparklineSmartTagItem_AreaOpacity, "Area Opacity");
			AddString(SnapExtensionsStringId.SparklineSmartTagItem_BarDistance, "Bar Distance");
			AddString(SnapExtensionsStringId.IndexSmartTagItem_GroupIndexMode, "Group Mode");
			AddString(SnapExtensionsStringId.AddDataSource, "Add Data Source...");
			AddString(SnapExtensionsStringId.RemoveDataSource, "Remove Data Source");
			AddString(SnapExtensionsStringId.UpdateDataSource, "Update Data Source");
			AddString(SnapExtensionsStringId.ConfigureConnection, "Configure Connection...");
			AddString(SnapExtensionsStringId.ManageRelations, "Manage Relations...");
			AddString(SnapExtensionsStringId.MailMergeDataSource, "Use for Mail Merge");
			AddString(SnapExtensionsStringId.ManageQueries, "Manage Queries...");
			AddString(SnapExtensionsStringId.RebuildResultSchema, "Rebuild Schema...");
			AddString(SnapExtensionsStringId.EditDataSource, "Edit Data Source...");
			AddString(SnapExtensionsStringId.Configure, "Configure Data Source...");
			AddString(SnapExtensionsStringId.SortingCollectionEditor_SortBy, "Sort by");
			AddString(SnapExtensionsStringId.ActionList_ContentType, "Content Type");
			AddString(SnapExtensionsStringId.ActionList_Binding, "Binding");
			AddString(SnapExtensionsStringId.ActionList_EnableEmptyFieldDataAlias, "Enable Empty Field Data Alias");
			AddString(SnapExtensionsStringId.ActionList_EmptyFieldDataAlias, "Empty Field Data Alias");
			AddString(SnapExtensionsStringId.ActionList_FormatString, "Format String");
			AddString(SnapExtensionsStringId.ActionList_Summary, "Summary");
			AddString(SnapExtensionsStringId.ActionList_Text, "Text");
			AddString(SnapExtensionsStringId.ActionList_ScreenTip, "Screen Tip");
			AddString(SnapExtensionsStringId.ActionList_TargetFrame, "Target Frame");
			AddString(SnapExtensionsStringId.ActionList_Checked, "Checked");
			AddString(SnapExtensionsStringId.ParametersErrorDialogCaption, "Incorrect parameter name");
			AddString(SnapExtensionsStringId.ParametersErrorDialogTextInvalidCharacters, "Cannot create parameters with invalid names: ");
			AddString(SnapExtensionsStringId.ParametersErrorDialogTextNoName, "Cannot create a parameter without specifying its name.");
			AddString(SnapExtensionsStringId.ParametersErrorDialogTextIdenticalNames, "Cannot create parameters with identical names: ");
			AddString(SnapExtensionsStringId.MailMergeCurrentRecordEdit_PreviousButtonTooltip, "Previous");
			AddString(SnapExtensionsStringId.MailMergeCurrentRecordEdit_NextButtonTooltip, "Next");
			AddString(SnapExtensionsStringId.MailMergeCurrentRecordEdit_FirstButtonTooltip, "First");
			AddString(SnapExtensionsStringId.MailMergeCurrentRecordEdit_LastButtonTooltip, "Last");
			AddString(SnapExtensionsStringId.FinishAndMergePageGroup_DefaultText, "Publish");
			AddString(SnapExtensionsStringId.DataShapingPageGroup_DefaultText, "Data Shaping");
			AddString(SnapExtensionsStringId.ToolboxPageGroup_DefaultText, "Toolbox");
			AddString(SnapExtensionsStringId.SNMergeFieldPropertiesRibbonPageGroup_DefaultText, "Element");
			AddString(SnapExtensionsStringId.DataRibbonPageGroup_DefaultText, "Data");
			AddString(SnapExtensionsStringId.MailMergeCurrentRecordRibbonPageGroup_DefaultText, "Current Record");
			AddString(SnapExtensionsStringId.MailMergeRibbonPage_DefaultText, "Mail Merge");
			AddString(SnapExtensionsStringId.ListCommandsRibbonPageGroup_DefaultText, "Commands");
			AddString(SnapExtensionsStringId.ListEditorRowLimitRibbonPageGroup_DefaultText, "Editor Row Limit");
			AddString(SnapExtensionsStringId.TableCellStylesRibbonPageGroup_DefaultText, "Cell Styles");
		}
	}
	 #endregion
}
