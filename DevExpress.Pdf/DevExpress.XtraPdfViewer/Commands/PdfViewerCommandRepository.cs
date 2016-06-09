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
using System.Reflection;
using System.Collections.Generic;
using DevExpress.Utils.Commands;
using DevExpress.XtraPdfViewer.Localization;
namespace DevExpress.XtraPdfViewer.Commands {
	public class PdfViewerCommandRepository {
		readonly Dictionary<PdfViewerCommandId, ConstructorInfo> commandConstructorTable = new Dictionary<PdfViewerCommandId, ConstructorInfo>();
		readonly PdfViewer control;
		internal PdfViewerCommandRepository(PdfViewer control) {
			this.control = control;
			AddCommandConstructor(PdfViewerCommandId.OpenFile, typeof(PdfOpenFileCommand));
			AddCommandConstructor(PdfViewerCommandId.SaveAsFile, typeof(PdfSaveAsFileCommand));
			AddCommandConstructor(PdfViewerCommandId.ExportFormData, typeof(PdfExportFormDataCommand));
			AddCommandConstructor(PdfViewerCommandId.ImportFormData, typeof(PdfImportFormDataCommand));
			AddCommandConstructor(PdfViewerCommandId.PrintFile, typeof(PdfPrintFileCommand));
			AddCommandConstructor(PdfViewerCommandId.PreviousPage, typeof(PdfPreviousPageCommand));
			AddCommandConstructor(PdfViewerCommandId.NextPage, typeof(PdfNextPageCommand));
			AddCommandConstructor(PdfViewerCommandId.FindText, typeof(PdfFindTextCommand));
			AddCommandConstructor(PdfViewerCommandId.ZoomIn, typeof(PdfZoomInCommand));
			AddCommandConstructor(PdfViewerCommandId.ZoomOut, typeof(PdfZoomOutCommand));
			AddCommandConstructor(PdfViewerCommandId.SetZoom, typeof(PdfSetZoomCommand));
			AddCommandConstructor(PdfViewerCommandId.ShowExactZoomList, typeof(PdfShowExactZoomListCommand));
			AddCommandConstructor(PdfViewerCommandId.Zoom500, typeof(PdfZoom500Command));
			AddCommandConstructor(PdfViewerCommandId.Zoom400, typeof(PdfZoom400Command));
			AddCommandConstructor(PdfViewerCommandId.Zoom200, typeof(PdfZoom200Command));
			AddCommandConstructor(PdfViewerCommandId.Zoom150, typeof(PdfZoom150Command));
			AddCommandConstructor(PdfViewerCommandId.Zoom125, typeof(PdfZoom125Command));
			AddCommandConstructor(PdfViewerCommandId.Zoom100, typeof(PdfZoom100Command));
			AddCommandConstructor(PdfViewerCommandId.Zoom75, typeof(PdfZoom75Command));
			AddCommandConstructor(PdfViewerCommandId.Zoom50, typeof(PdfZoom50Command));
			AddCommandConstructor(PdfViewerCommandId.Zoom25, typeof(PdfZoom25Command));
			AddCommandConstructor(PdfViewerCommandId.Zoom10, typeof(PdfZoom10Command));
			AddCommandConstructor(PdfViewerCommandId.SetActualSizeZoomMode, typeof(PdfSetActualSizeZoomModeCommand));
			AddCommandConstructor(PdfViewerCommandId.SetPageLevelZoomMode, typeof(PdfSetPageLevelZoomModeCommand));
			AddCommandConstructor(PdfViewerCommandId.SetFitWidthZoomMode, typeof(PdfSetFitWidthZoomModeCommand));
			AddCommandConstructor(PdfViewerCommandId.SetFitVisibleZoomMode, typeof(PdfSetFitVisibleZoomModeCommand));
			AddCommandConstructor(PdfViewerCommandId.RotatePageClockwise, typeof(PdfRotatePageClockwiseCommand));
			AddCommandConstructor(PdfViewerCommandId.RotatePageCounterclockwise, typeof(PdfRotatePageCounterclockwiseCommand));
			AddCommandConstructor(PdfViewerCommandId.PreviousView, typeof(PdfPreviousViewCommand));
			AddCommandConstructor(PdfViewerCommandId.NextView, typeof(PdfNextViewCommand));
			AddCommandConstructor(PdfViewerCommandId.ShowDocumentProperties, typeof(PdfShowDocumentPropertiesCommand));
			AddCommandConstructor(PdfViewerCommandId.HandTool, typeof(PdfHandToolCommand));
			AddCommandConstructor(PdfViewerCommandId.SelectTool, typeof(PdfSelectToolCommand));
			AddCommandConstructor(PdfViewerCommandId.SelectAll, typeof(PdfSelectAllCommand));
			AddCommandConstructor(PdfViewerCommandId.Copy, typeof(PdfCopyCommand));
			AddCommandConstructor(PdfViewerCommandId.OutlinesWrapLongLines, typeof(PdfOutlinesWrapLongLinesCommand));
			AddCommandConstructor(PdfViewerCommandId.OutlinesTextSizeToLarge, typeof(PdfOutlinesTextSizeToLargeCommand));
			AddCommandConstructor(PdfViewerCommandId.OutlinesTextSizeToMedium, typeof(PdfOutlinesTextSizeToMediumCommand));
			AddCommandConstructor(PdfViewerCommandId.OutlinesTextSizeToSmall, typeof(PdfOutlinesTextSizeToSmallCommand));
			AddCommandConstructor(PdfViewerCommandId.GotoOutline, typeof(PdfOutlinesGotoCommand));
			AddCommandConstructor(PdfViewerCommandId.OutlineViewerHideAfterUse, typeof(PdfOutlinesHideAfterUseCommand));
			AddCommandConstructor(PdfViewerCommandId.ExpandCurrentOutline, typeof(PdfOutlinesExpandCurrentCommand));
			AddCommandConstructor(PdfViewerCommandId.OutlinePrintPages, typeof(PdfOutlinesPrintPagesCommand));
			AddCommandConstructor(PdfViewerCommandId.OutlinePrintSections, typeof(PdfOutlinesPrintSectionsCommand));
			AddCommandConstructor(PdfViewerCommandId.OutlinesExpandCollapseTopLevel, typeof(PdfOutlinesExpandCollapseTopLevelCommand));
		}
		protected void AddCommandConstructor(PdfViewerCommandId commandId, Type commandType) {
			ConstructorInfo ci = commandType.GetConstructor(new Type[] { control.GetType() });
			if (ci == null)
				throw new Exception(String.Format(XtraPdfViewerLocalizer.GetString(XtraPdfViewerStringId.MessageAddCommandConstructorError), commandType));
			commandConstructorTable.Add(commandId, ci);
		}
		internal Command CreateCommand(PdfViewerCommandId id) {
			ConstructorInfo ci;
			return (commandConstructorTable.TryGetValue(id, out ci) && ci != null) ? ci.Invoke(new object[] { control }) as Command : null;
		}
	}
}
