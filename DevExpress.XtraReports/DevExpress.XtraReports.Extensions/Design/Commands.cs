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
using System.ComponentModel.Design;
using System.Collections;
using DevExpress.XtraReports.UI;
using System.Windows.Forms.Design;
using DevExpress.XtraReports.Native;
namespace DevExpress.XtraReports.Design.Commands {
	public static class CommandGroups {
		public static readonly CommandID[]
			PivotGridCommands = { VerbCommands.PivotGridDesigner },
			RootReportCommands = { VerbCommands.ReportWizard,
								   VerbCommands.EditBands,
								   VerbCommands.EditBindings,
								   VerbCommands.Import,
								   VerbCommands.LoadReportTemplate},
			TextControlCommands = { VerbCommands.EditText },
			RichTextBoxCommands = { VerbCommands.RtfClear, 
									VerbCommands.RtfLoadFile },
			BandInsertCommands = {  BandCommands.InsertDetailBand, 
									BandCommands.InsertGroupFooterBand,
									BandCommands.InsertGroupHeaderBand,
									BandCommands.InsertBottomMarginBand,
									BandCommands.InsertTopMarginBand,
									BandCommands.InsertReportFooterBand,
									BandCommands.InsertReportHeaderBand,
									BandCommands.InsertPageFooterBand,
									BandCommands.InsertPageHeaderBand,
								 },
			FieldDropCommands = {
				BandCommands.BindFieldToXRLabel,
				BandCommands.BindFieldToXRPictureBox,
				BandCommands.BindFieldToXRBarCode,
				BandCommands.BindFieldToXRCheckBox,
				BandCommands.BindFieldToXRRichText,
				BandCommands.BindFieldToXRZipCode,
				BandCommands.BindFieldsToXRTable 
			},
			TableCellCommands = {
									Commands.TableCommands.DeleteCell, 
									Commands.TableCommands.InsertCell},
			TableRowCommands = {
								   Commands.TableCommands.DeleteRow, 
								   Commands.TableCommands.InsertRowAbove, 
								   Commands.TableCommands.InsertRowBelow},
			TableColumnCommands = {
									  Commands.TableCommands.DeleteColumn,
									  Commands.TableCommands.InsertColumnToLeft,
									  Commands.TableCommands.InsertColumnToRight},
			TableCommands = {
									  Commands.TableCommands.ConvertToLabels},
			KeyMoveCommands = { 																	
								  MenuCommands.KeyMoveLeft,
								  MenuCommands.KeyMoveRight,
								  MenuCommands.KeyMoveUp,
								  MenuCommands.KeyMoveDown},
			KeyNudgeCommands = { 
								   MenuCommands.KeyNudgeLeft,
								   MenuCommands.KeyNudgeRight,
								   MenuCommands.KeyNudgeUp,
								   MenuCommands.KeyNudgeDown},
			KeySizeCommands = { 
								  MenuCommands.KeySizeWidthDecrease,
								  MenuCommands.KeySizeWidthIncrease, 
								  MenuCommands.KeySizeHeightDecrease,
								  MenuCommands.KeySizeHeightIncrease},
			KeyNudgeSizeCommands = { 
								  MenuCommands.KeyNudgeWidthDecrease,
								  MenuCommands.KeyNudgeWidthIncrease, 
								  MenuCommands.KeyNudgeHeightDecrease,
								  MenuCommands.KeyNudgeHeightIncrease},
			KeySelectionCommands = {
								  MenuCommands.KeySelectNext,
								  MenuCommands.KeySelectPrevious },
			AlignCommands = {   
								StandardCommands.AlignLeft, 
								StandardCommands.AlignTop,
								StandardCommands.AlignRight,
								StandardCommands.AlignBottom,
								StandardCommands.AlignVerticalCenters,
								StandardCommands.AlignHorizontalCenters},
			SizeCommands = {	   
							   StandardCommands.SizeToControl, 
							   StandardCommands.SizeToControlHeight,
							   StandardCommands.SizeToControlWidth},
			HorizSpaceCommands = {	 
									 StandardCommands.HorizSpaceConcatenate, 
									 StandardCommands.HorizSpaceDecrease,
									 StandardCommands.HorizSpaceIncrease,
									 StandardCommands.HorizSpaceMakeEqual},
			VertSpaceCommands = {  
									StandardCommands.VertSpaceConcatenate,
									StandardCommands.VertSpaceDecrease,
									StandardCommands.VertSpaceIncrease,
									StandardCommands.VertSpaceMakeEqual},
			SingleSelectionCommands = {
										  StandardCommands.SizeToGrid, 
										  StandardCommands.AlignToGrid, 
										  StandardCommands.CenterVertically, 
										  StandardCommands.CenterHorizontally,
										  MenuCommands.BringToFront, 
										  MenuCommands.SendToBack},
			FormatCommands = { 
								 null,
								 FormattingCommands.Bold,
								 FormattingCommands.Italic,
								 FormattingCommands.Underline,
							 },
			ColorCommands = { 
								 null,
								 FormattingCommands.ForeColor,
   								 FormattingCommands.BackColor,
							 },
			JustifyCommands = { 
								  null,
								  FormattingCommands.JustifyLeft,	
								  FormattingCommands.JustifyCenter,
								  FormattingCommands.JustifyRight,
								  FormattingCommands.JustifyJustify
							  },
			FontInfoCommands = { 
								   FormattingCommands.FontName, 
								   FormattingCommands.FontSize	
							   },
			ZoomCommands = { 
							   DevExpress.XtraReports.Design.Commands.ZoomCommands.Zoom, 
							   DevExpress.XtraReports.Design.Commands.ZoomCommands.ZoomIn, 
							   DevExpress.XtraReports.Design.Commands.ZoomCommands.ZoomOut
						   },
			ReorderBandsCommands = {
								DevExpress.XtraReports.Design.Commands.ReorderBandsCommands.MoveUp,
								DevExpress.XtraReports.Design.Commands.ReorderBandsCommands.MoveDown   
						},
			ReportTabControlCommands = {
								DevExpress.XtraReports.Design.Commands.ReportTabControlCommands.ShowDesignerTab,
								DevExpress.XtraReports.Design.Commands.ReportTabControlCommands.ShowScriptsTab,
								DevExpress.XtraReports.Design.Commands.ReportTabControlCommands.ShowPreviewTab,
								DevExpress.XtraReports.Design.Commands.ReportTabControlCommands.ShowHTMLViewTab,
			},
			HtmlCommands = {
							DevExpress.XtraReports.Design.Commands.HtmlCommands.Backward,
							DevExpress.XtraReports.Design.Commands.HtmlCommands.Forward,
							DevExpress.XtraReports.Design.Commands.HtmlCommands.Home,
							DevExpress.XtraReports.Design.Commands.HtmlCommands.Refresh,
							DevExpress.XtraReports.Design.Commands.HtmlCommands.Find
			},
			FieldListCommands = {
							DevExpress.XtraReports.Design.Commands.FieldListCommands.AddCalculatedField,
							DevExpress.XtraReports.Design.Commands.FieldListCommands.EditCalculatedFields,
							DevExpress.XtraReports.Design.Commands.FieldListCommands.EditExpressionCalculatedField,
							DevExpress.XtraReports.Design.Commands.FieldListCommands.DeleteCalculatedField,
							DevExpress.XtraReports.Design.Commands.FieldListCommands.AddParameter,
							DevExpress.XtraReports.Design.Commands.FieldListCommands.EditParameters,
							DevExpress.XtraReports.Design.Commands.FieldListCommands.DeleteParameter,
							DevExpress.XtraReports.Design.Commands.FieldListCommands.ClearCalculatedFields,
							DevExpress.XtraReports.Design.Commands.FieldListCommands.ClearParameters,
			},
			FormattingComponentCommands = {
						   DevExpress.XtraReports.Design.Commands.FormattingComponentCommands.AddStyle,
						   DevExpress.XtraReports.Design.Commands.FormattingComponentCommands.EditStyles,
						   DevExpress.XtraReports.Design.Commands.FormattingComponentCommands.SelectControlsWithStyle,
						   DevExpress.XtraReports.Design.Commands.FormattingComponentCommands.PurgeStyles,
						   DevExpress.XtraReports.Design.Commands.FormattingComponentCommands.ClearStyles,
						   DevExpress.XtraReports.Design.Commands.FormattingComponentCommands.AddFormattingRule,
						   DevExpress.XtraReports.Design.Commands.FormattingComponentCommands.EditFormattingRules,
						   DevExpress.XtraReports.Design.Commands.FormattingComponentCommands.SelectControlsWithFormattingRule,
						   DevExpress.XtraReports.Design.Commands.FormattingComponentCommands.PurgeFormattingRules,
						   DevExpress.XtraReports.Design.Commands.FormattingComponentCommands.ClearFormattingRules,
						   DevExpress.XtraReports.Design.Commands.FormattingComponentCommands.AssignStyleToXRControl,
						   DevExpress.XtraReports.Design.Commands.FormattingComponentCommands.AssignRuleToXRControl,
			},
			KeyCommands,
			StrongCommands,
			MultiSelectionCommands,
			ReportCommands = { Commands.ReportCommands.InsertDetailReport };
		static CommandGroups() {
			ArrayList commands = NativeMethods.MergeLists(KeyMoveCommands, KeyNudgeCommands, KeySizeCommands, KeyNudgeSizeCommands);
			commands.Add(MenuCommands.KeyCancel);
			KeyCommands = commands.ToArray(typeof(CommandID)) as CommandID[];
			commands = NativeMethods.MergeLists(KeyCommands, SizeCommands);
			commands.AddRange(new object[] {
											   StandardCommands.AlignLeft, 
											   StandardCommands.AlignRight,
											   StandardCommands.AlignVerticalCenters,
											   StandardCommands.AlignToGrid,
											   StandardCommands.SizeToGrid,
											   StandardCommands.SendToBack,
											   StandardCommands.BringToFront,
											   FormattingCommands.Bold,
											   FormattingCommands.Italic,
											   FormattingCommands.Underline,
											   FormattingCommands.JustifyLeft,
											   FormattingCommands.JustifyCenter,
											   FormattingCommands.JustifyRight,
											   FormattingCommands.JustifyJustify
										   });
			StrongCommands = commands.ToArray(typeof(CommandID)) as CommandID[];
			commands = NativeMethods.MergeLists(SingleSelectionCommands, AlignCommands, SizeCommands, HorizSpaceCommands, VertSpaceCommands);
			MultiSelectionCommands = commands.ToArray(typeof(CommandID)) as CommandID[];
		}
	}
	public static class FormattingCommands {
		public static readonly Guid EnvCommandSet = new Guid("{5EFC7975-14BC-11CF-9B2B-00AA00573819}");
		static readonly Guid fontCommandSet = new Guid("{619E7FD2-6457-4619-8F3E-4188EECF9A10}");
		public static byte[] FontSizeSet = { 8, 9, 10, 11, 12, 14, 16, 18, 20, 22, 24, 26, 28, 36, 48, 72 };
		private const int cmdidFontName = 1;
		private const int cmdidFontSize = 2;
		private const int cmdidBold = 52;
		private const int cmdidItalic = 70;
		private const int cmdidUnderline = 77;
		private const int cmdidJustifyCenter = 71;
		private const int cmdidJustifyLeft = 73;
		private const int cmdidJustifyRight = 74;
		private const int cmdidJustifyJustify = 72;
		private const int cmdidForeColor = 69;
		private const int cmdidBackColor = 51;
		public static readonly CommandID FontName = new CommandID(fontCommandSet, cmdidFontName);
		public static readonly CommandID FontSize = new CommandID(fontCommandSet, cmdidFontSize);
		public static readonly CommandID Bold = new CommandID(EnvCommandSet, cmdidBold);
		public static readonly CommandID Italic = new CommandID(EnvCommandSet, cmdidItalic);
		public static readonly CommandID Underline = new CommandID(EnvCommandSet, cmdidUnderline);
		public static readonly CommandID JustifyCenter = new CommandID(EnvCommandSet, cmdidJustifyCenter);
		public static readonly CommandID JustifyLeft = new CommandID(EnvCommandSet, cmdidJustifyLeft);
		public static readonly CommandID JustifyRight = new CommandID(EnvCommandSet, cmdidJustifyRight);
		public static readonly CommandID JustifyJustify = new CommandID(EnvCommandSet, cmdidJustifyJustify);
		public static readonly CommandID ForeColor = new CommandID(EnvCommandSet, cmdidForeColor);
		public static readonly CommandID BackColor = new CommandID(EnvCommandSet, cmdidBackColor);
	}
	public static class WrappedCommands {
		private const int cmdidPropertiesWindow = 1;
		private const int cmdidViewCode = 2;
		private const int cmdidToolbox = 3;
		static readonly Guid WrappedCommandSet = new Guid("{8D9B7533-BF18-40c4-8BAB-80B4A6D125FD}");
		public static readonly CommandID PropertiesWindow = new CommandID(WrappedCommandSet, cmdidPropertiesWindow);
		public static readonly CommandID ViewCode = new CommandID(WrappedCommandSet, cmdidViewCode);
		public static readonly CommandID Toolbox = new CommandID(WrappedCommandSet, cmdidToolbox);
	}
	public static class ReportCommands {
		private const int cmdidInsDetailReport = 1;
		private const int cmdidNone = 2;
		private const int cmdidAddNewDataSource = 3;
		private static readonly Guid reportCommandSet = new Guid("{91EE9205-B4F8-4d6c-BA02-741365F24330}");
		public static readonly CommandID InsertDetailReport = new CommandID(reportCommandSet, cmdidInsDetailReport);
		public static readonly CommandID None = new CommandID(reportCommandSet, cmdidNone);
		public static readonly CommandID AddNewDataSource = new CommandID(reportCommandSet, cmdidAddNewDataSource);
	}
	public static class BandCommands {
		static int i = 1;
		static int cmdidInsTopMarginBand = i++;
		static int cmdidInsBottomMarginBand = i++;
		static int cmdidInsReportHeaderBand = i++;
		static int cmdidInsReportFooterBand = i++;
		static int cmdidInsPageHeaderBand = i++;
		static int cmdidInsPageFooterBand = i++;
		static int cmdidInsGroupHeaderBand = i++;
		static int cmdidInsGroupFooterBand = i++;
		static int cmdidInsDetailBand = i++;
		static int cmdidAddSubBand = i++;
		static int cmdidBindFieldToXRLabel = i++;
		static int cmdidBindFieldToXRPictureBox = i++;
		static int cmdidBindFieldToXRRichTextBox = i++;
		static int cmdidBindFieldToXRCheckBox = i++;
		static int cmdidBindFieldToXRBarCode = i++;
		static int cmdidBindFieldToXRZipCode = i++;
		static int cmdidBindFieldsToXRTable = i++;
		private static readonly Guid bandCommandSet = new Guid("{BB714C9C-51CC-4e0d-A584-542DD0177E71}");
		public static readonly CommandID InsertTopMarginBand = new BandCommandID(bandCommandSet, cmdidInsTopMarginBand, BandKind.TopMargin);
		public static readonly CommandID InsertBottomMarginBand = new BandCommandID(bandCommandSet, cmdidInsBottomMarginBand, BandKind.BottomMargin);
		public static readonly CommandID InsertReportHeaderBand = new BandCommandID(bandCommandSet, cmdidInsReportHeaderBand, BandKind.ReportHeader);
		public static readonly CommandID InsertReportFooterBand = new BandCommandID(bandCommandSet, cmdidInsReportFooterBand, BandKind.ReportFooter);
		public static readonly CommandID InsertPageHeaderBand = new BandCommandID(bandCommandSet, cmdidInsPageHeaderBand, BandKind.PageHeader);
		public static readonly CommandID InsertPageFooterBand = new BandCommandID(bandCommandSet, cmdidInsPageFooterBand, BandKind.PageFooter);
		public static readonly CommandID InsertGroupHeaderBand = new BandCommandID(bandCommandSet, cmdidInsGroupHeaderBand, BandKind.GroupHeader);
		public static readonly CommandID InsertGroupFooterBand = new BandCommandID(bandCommandSet, cmdidInsGroupFooterBand, BandKind.GroupFooter);
		public static readonly CommandID InsertDetailBand = new BandCommandID(bandCommandSet, cmdidInsDetailBand, BandKind.Detail);
		public static readonly CommandID AddSubBand = new BandCommandID(bandCommandSet, cmdidAddSubBand, BandKind.SubBand);
		public static readonly CommandID BindFieldToXRLabel = new CommandID(bandCommandSet, cmdidBindFieldToXRLabel);
		public static readonly CommandID BindFieldToXRPictureBox = new CommandID(bandCommandSet, cmdidBindFieldToXRPictureBox);
		public static readonly CommandID BindFieldToXRRichText = new CommandID(bandCommandSet, cmdidBindFieldToXRRichTextBox);
		public static readonly CommandID BindFieldToXRCheckBox = new CommandID(bandCommandSet, cmdidBindFieldToXRCheckBox);
		public static readonly CommandID BindFieldToXRBarCode = new CommandID(bandCommandSet, cmdidBindFieldToXRBarCode);
		public static readonly CommandID BindFieldToXRZipCode = new CommandID(bandCommandSet, cmdidBindFieldToXRZipCode);
		public static readonly CommandID BindFieldsToXRTable = new CommandID(bandCommandSet, cmdidBindFieldsToXRTable);
	}
	public class BandCommandID : CommandID {
		public static BandKind GetBandKind(CommandID cmdID) {
			return cmdID is BandCommandID ? ((BandCommandID)cmdID).BandKind : BandKind.None;
		}
		BandKind bandKind;
		public BandKind BandKind { get { return bandKind; } }
		public BandCommandID(Guid menuGroup, int commandID, BandKind bandKind)
			: base(menuGroup, commandID) {
			this.bandKind = bandKind;
		}
	}
	public static class VerbCommands {
		static int i = 1;
		static int cmdidReportWizard = i++,
			cmdidEditText = i++,
			cmdidRtfClear = i++,
			cmdidRtfLoadFile = i++,
			cmdidEditBands = i++,
			cmdidPivotGridDesigner = i++,
			cmdidLoadReportTemplate = i++,
			cmdidEditBindings = i++,
			cmdidImport = i++,
			cmdidExport = i++,
			cmdidExecuteVerb = i++;
		static readonly Guid verbCommandSet = new Guid("{A67B5BF2-E5AA-49f9-9D79-068D89B8EAEE}");
		public static readonly CommandID ReportWizard = new CommandID(verbCommandSet, cmdidReportWizard);
		public static readonly CommandID EditText = new CommandID(verbCommandSet, cmdidEditText);
		public static readonly CommandID RtfClear = new CommandID(verbCommandSet, cmdidRtfClear);
		public static readonly CommandID RtfLoadFile = new CommandID(verbCommandSet, cmdidRtfLoadFile);
		public static readonly CommandID EditBands = new CommandID(verbCommandSet, cmdidEditBands);
		public static readonly CommandID PivotGridDesigner = new CommandID(verbCommandSet, cmdidPivotGridDesigner);
		public static readonly CommandID LoadReportTemplate = new CommandID(verbCommandSet, cmdidLoadReportTemplate);
		public static readonly CommandID EditBindings = new CommandID(verbCommandSet, cmdidEditBindings);
		public static readonly CommandID Import = new CommandID(verbCommandSet, cmdidImport);
		public static readonly CommandID Export = new CommandID(verbCommandSet, cmdidExport);
		public static readonly CommandID ExecuteVerb = new CommandID(verbCommandSet, cmdidExecuteVerb);
	}
	public static class MenuCommandServiceCommands {
		const int cmdidSelectionMenu = 1;
		const int cmdidFieldDropMenu = 2;
		const int cmdidReportExplorerMenu = 3;
		static readonly Guid menuCommandServiceCommandSet = new Guid("{CA385642-5D1E-4cb6-90FB-F43CB66B6E55}");
		public static readonly CommandID SelectionMenu = new CommandID(menuCommandServiceCommandSet, cmdidSelectionMenu);
		public static readonly CommandID FieldDropMenu = new CommandID(menuCommandServiceCommandSet, cmdidFieldDropMenu);
		public static readonly CommandID ReportExplorerMenu = new CommandID(menuCommandServiceCommandSet, cmdidReportExplorerMenu);
	}
	public static class ZoomCommands {
		const int cmdidZoom = 1;
		const int cmdidZoomIn = 2;
		const int cmdidZoomOut = 3;
		static readonly Guid zoomCommandSet = new Guid("{E64E28BE-94CE-4191-A9D8-6D3EEB34A7F0}");
		public static readonly CommandID Zoom = new CommandID(zoomCommandSet, cmdidZoom);
		public static readonly CommandID ZoomIn = new CommandID(zoomCommandSet, cmdidZoomIn);
		public static readonly CommandID ZoomOut = new CommandID(zoomCommandSet, cmdidZoomOut);
	}
	public static class ReorderBandsCommands {
		const int cmdidMoveUp = 1;
		const int cmdidMoveDown = 2;
		static readonly Guid reorderBandsCommandSet = new Guid("{5C7A8AB6-E5C6-42bf-AC5E-29F6B6803A6B}");
		public static readonly CommandID MoveUp = new CommandID(reorderBandsCommandSet, cmdidMoveUp);
		public static readonly CommandID MoveDown = new CommandID(reorderBandsCommandSet, cmdidMoveDown);
	}
	public static class ReportTabControlCommands {
		private const int cmdidShowDesignerTab = TabIndices.Designer;
		private const int cmdidShowPreviewTab = TabIndices.Preview;
		private const int cmdidShowHTMLViewTab = TabIndices.Html;
		private const int cmdidShowScriptsTab = TabIndices.Scripts;
		static readonly Guid reportTabControlCommandSet = new Guid("{42734816-DC27-4730-AA64-33FDAF306CF2}");
		public static readonly CommandID ShowDesignerTab = new CommandID(reportTabControlCommandSet, cmdidShowDesignerTab);
		public static readonly CommandID ShowPreviewTab = new CommandID(reportTabControlCommandSet, cmdidShowPreviewTab);
		public static readonly CommandID ShowHTMLViewTab = new CommandID(reportTabControlCommandSet, cmdidShowHTMLViewTab);
		public static readonly CommandID ShowScriptsTab = new CommandID(reportTabControlCommandSet, cmdidShowScriptsTab);
	}
	public static class HtmlCommands {
		const int cmdidHtmlRefresh = 1;
		const int cmdidHtmlBackward = 2;
		const int cmdidHtmlForward = 3;
		const int cmdidHtmlHome = 4;
		const int cmdidHtmlFind = 5;
		static readonly Guid htmlCommandSet = new Guid("{5F350DAC-D86A-4c25-B25D-C4F8E324BE6C}");
		public static readonly CommandID Refresh = new CommandID(htmlCommandSet, cmdidHtmlRefresh);
		public static readonly CommandID Backward = new CommandID(htmlCommandSet, cmdidHtmlBackward);
		public static readonly CommandID Forward = new CommandID(htmlCommandSet, cmdidHtmlForward);
		public static readonly CommandID Home = new CommandID(htmlCommandSet, cmdidHtmlHome);
		public static readonly CommandID Find = new CommandID(htmlCommandSet, cmdidHtmlFind);
	}
	public static class UICommands {
		static int i = 0;
		static readonly int cmdidOpenFile = ++i;
		static readonly int cmdidSaveFile = ++i;
		static readonly int cmdidSaveFileAs = ++i;
		static readonly int cmdidSaveAll = ++i;
		static readonly int cmdidExit = ++i;
		static readonly int cmdidNewReport = ++i;
		static readonly int cmdidNewReportWizard = ++i;
		static readonly int cmdidClosing = ++i;
		static readonly int cmdidMdiCascade = ++i;
		static readonly int cmdidMdiTileHorizontal = ++i;
		static readonly int cmdidMdiTileVertical = ++i;
		static readonly int cmdidShowTabbedInderface = ++i;
		static readonly int cmdidShowWindowInterface = ++i;
		static readonly int cmdidClose = ++i;
		static readonly int cmdidOpenSubreport = ++i;
		static readonly int cmdidCheckIn = ++i;
		static readonly int cmdidUndoCheckOut = ++i;
		static readonly int cmdidOpenRemoteReport = ++i;
		static readonly int cmdidUploadNewRemoteReport = ++i;
		static readonly int cmdidRevertToRevision = ++i;
		private static readonly Guid UICommandSet = new Guid("{2B998921-D5C4-4eb9-B4A3-3D72732325D5}");
		public static readonly CommandID OpenFile = new CommandID(UICommandSet, cmdidOpenFile);
		public static readonly CommandID SaveFile = new CommandID(UICommandSet, cmdidSaveFile);
		public static readonly CommandID SaveFileAs = new CommandID(UICommandSet, cmdidSaveFileAs);
		public static readonly CommandID SaveAll = new CommandID(UICommandSet, cmdidSaveAll);
		public static readonly CommandID Exit = new CommandID(UICommandSet, cmdidExit);
		public static readonly CommandID NewReport = new CommandID(UICommandSet, cmdidNewReport);
		public static readonly CommandID NewReportWizard = new CommandID(UICommandSet, cmdidNewReportWizard);
		public static readonly CommandID Closing = new CommandID(UICommandSet, cmdidClosing);
		public static readonly CommandID MdiCascade = new CommandID(UICommandSet, cmdidMdiCascade);
		public static readonly CommandID MdiTileHorizontal = new CommandID(UICommandSet, cmdidMdiTileHorizontal);
		public static readonly CommandID MdiTileVertical = new CommandID(UICommandSet, cmdidMdiTileVertical);
		public static readonly CommandID ShowTabbedInterface = new CommandID(UICommandSet, cmdidShowTabbedInderface);
		public static readonly CommandID ShowWindowInterface = new CommandID(UICommandSet, cmdidShowWindowInterface);
		public static readonly CommandID Close = new CommandID(UICommandSet, cmdidClose);
		public static readonly CommandID OpenSubreport = new CommandID(UICommandSet, cmdidOpenSubreport);
		public static readonly CommandID CheckIn = new CommandID(UICommandSet, cmdidCheckIn);
		public static readonly CommandID UndoCheckOut = new CommandID(UICommandSet, cmdidUndoCheckOut);
		public static readonly CommandID OpenRemoteReport = new CommandID(UICommandSet, cmdidOpenRemoteReport);
		public static readonly CommandID UploadNewRemoteReport = new CommandID(UICommandSet, cmdidUploadNewRemoteReport);
		public static readonly CommandID RevertToRevision = new CommandID(UICommandSet, cmdidRevertToRevision);
	}
	public static class FormattingComponentCommands {
		static int i = 0;
		static int 
			cmdidAddStyle = ++i,
			cmdidEditStyles = ++i,
			cmdidSelectControlsWithStyle = ++i,
			cmdidPurgeStyles = ++i,
			cmdidClearStyles = ++i,
			cmdidAddFormattingRule = ++i,
			cmdidEditFormattingRules = ++i,
			cmdidSelectControlsWithFormattingRule = ++i,
			cmdidPurgeFormattingRules = ++i,
			cmdidClearFormattingRules = ++i,
			cmdidAssignStyleToXRControl = ++i,
			cmdidAssignOddStyleToXRControl = ++i,
			cmdidAssignEvenStyleToXRControl = ++i,
			cmdidAssignRuleToXRControl = ++i;
		static readonly Guid formattingComponentCommandSet = new Guid("{D8BFC58C-32D8-4856-85DB-72877B40EC00}");
		public static readonly CommandID AddStyle = new CommandID(formattingComponentCommandSet, cmdidAddStyle);
		public static readonly CommandID EditStyles = new CommandID(formattingComponentCommandSet, cmdidEditStyles);
		public static readonly CommandID SelectControlsWithStyle = new CommandID(formattingComponentCommandSet, cmdidSelectControlsWithStyle);
		public static readonly CommandID PurgeStyles = new CommandID(formattingComponentCommandSet, cmdidPurgeStyles);
		public static readonly CommandID ClearStyles = new CommandID(formattingComponentCommandSet, cmdidClearStyles);
		public static readonly CommandID AddFormattingRule = new CommandID(formattingComponentCommandSet, cmdidAddFormattingRule);
		public static readonly CommandID EditFormattingRules = new CommandID(formattingComponentCommandSet, cmdidEditFormattingRules);
		public static readonly CommandID SelectControlsWithFormattingRule = new CommandID(formattingComponentCommandSet, cmdidSelectControlsWithFormattingRule);
		public static readonly CommandID PurgeFormattingRules = new CommandID(formattingComponentCommandSet, cmdidPurgeFormattingRules);
		public static readonly CommandID ClearFormattingRules = new CommandID(formattingComponentCommandSet, cmdidClearFormattingRules);
		public static readonly CommandID AssignStyleToXRControl = new CommandID(formattingComponentCommandSet, cmdidAssignStyleToXRControl);
		public static readonly CommandID AssignRuleToXRControl = new CommandID(formattingComponentCommandSet, cmdidAssignRuleToXRControl);
	}
}
