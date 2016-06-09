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
using DevExpress.XtraPrinting;
namespace DevExpress.Xpf.Printing.Native {
	public static class ExportFormatConverter {
		public static ExportFormat ToExportFormat(PrintingSystemCommand command) {
			switch(command) {
				case PrintingSystemCommand.ExportPdf:
				case PrintingSystemCommand.SendPdf:
					return ExportFormat.Pdf;
				case PrintingSystemCommand.ExportHtm:
					return ExportFormat.Htm;
				case PrintingSystemCommand.ExportMht:
				case PrintingSystemCommand.SendMht:
					return ExportFormat.Mht;
				case PrintingSystemCommand.ExportRtf:
				case PrintingSystemCommand.SendRtf:
					return ExportFormat.Rtf;
				case PrintingSystemCommand.ExportXls:
				case PrintingSystemCommand.SendXls:
					return ExportFormat.Xls;
				case PrintingSystemCommand.ExportXlsx:
				case PrintingSystemCommand.SendXlsx:
					return ExportFormat.Xlsx;
				case PrintingSystemCommand.ExportCsv:
				case PrintingSystemCommand.SendCsv:
					return ExportFormat.Csv;
				case PrintingSystemCommand.ExportTxt:
				case PrintingSystemCommand.SendTxt:
					return ExportFormat.Txt;
				case PrintingSystemCommand.ExportGraphic:
				case PrintingSystemCommand.SendGraphic:
					return ExportFormat.Image;
				case PrintingSystemCommand.ExportXps:
				case PrintingSystemCommand.SendXps:
					return ExportFormat.Xps;
				default:
					throw new ArgumentException("command");
			}
		}
		public static PrintingSystemCommand ToExportCommand(ExportFormat format) {
			switch(format) {
				case ExportFormat.Csv:
					return PrintingSystemCommand.ExportCsv;
				case ExportFormat.Image:
					return PrintingSystemCommand.ExportGraphic;
				case ExportFormat.Mht:
					return PrintingSystemCommand.ExportMht;
				case ExportFormat.Pdf:
					return PrintingSystemCommand.ExportPdf;
				case ExportFormat.Rtf:
					return PrintingSystemCommand.ExportRtf;
				case ExportFormat.Txt:
					return PrintingSystemCommand.ExportTxt;
				case ExportFormat.Xls:
					return PrintingSystemCommand.ExportXls;
				case ExportFormat.Xlsx:
					return PrintingSystemCommand.ExportXlsx;
				case ExportFormat.Xps:
					return PrintingSystemCommand.ExportXps;
				case ExportFormat.Htm:
					return PrintingSystemCommand.ExportHtm;
			}
			throw new ArgumentException("format");
		}
		public static PrintingSystemCommand ToSendCommand(ExportFormat format) {
			switch(format) {
				case ExportFormat.Csv:
					return PrintingSystemCommand.SendCsv;
				case ExportFormat.Image:
					return PrintingSystemCommand.SendGraphic;
				case ExportFormat.Mht:
					return PrintingSystemCommand.SendMht;
				case ExportFormat.Pdf:
					return PrintingSystemCommand.SendPdf;
				case ExportFormat.Rtf:
					return PrintingSystemCommand.SendRtf;
				case ExportFormat.Txt:
					return PrintingSystemCommand.SendTxt;
				case ExportFormat.Xls:
					return PrintingSystemCommand.SendXls;
				case ExportFormat.Xlsx:
					return PrintingSystemCommand.SendXlsx;
				case ExportFormat.Xps:
					return PrintingSystemCommand.SendXps;
			}
			throw new ArgumentException("format");
		}
	}
}
