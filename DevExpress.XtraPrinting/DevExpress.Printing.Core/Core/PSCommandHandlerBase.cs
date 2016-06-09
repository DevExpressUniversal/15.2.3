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
using System.Text;
using DevExpress.XtraPrinting.Export;
namespace DevExpress.XtraPrinting {
	public delegate void PrintingSystemCommandHandlerBase(object[] args);
	public abstract class PSCommandHandlerBase {
		protected Dictionary<PrintingSystemCommand, object> handlers;
		protected abstract PrintingSystemBase PrintingSystemBase { get; }
		public PSCommandHandlerBase() {
			handlers = new Dictionary<PrintingSystemCommand, object>();
			handlers.Add(PrintingSystemCommand.ExportCsv, new PrintingSystemCommandHandlerBase(HandleExportCsv));
			handlers.Add(PrintingSystemCommand.ExportGraphic, new PrintingSystemCommandHandlerBase(HandleExportGraphic));
			handlers.Add(PrintingSystemCommand.ExportHtm, new PrintingSystemCommandHandlerBase(HandleExportHtm));
			handlers.Add(PrintingSystemCommand.ExportMht, new PrintingSystemCommandHandlerBase(HandleExportMht));
			handlers.Add(PrintingSystemCommand.ExportPdf, new PrintingSystemCommandHandlerBase(HandleExportPdf));
			handlers.Add(PrintingSystemCommand.ExportRtf, new PrintingSystemCommandHandlerBase(HandleExportRtf));
			handlers.Add(PrintingSystemCommand.ExportTxt, new PrintingSystemCommandHandlerBase(HandleExportTxt));
			handlers.Add(PrintingSystemCommand.ExportXls, new PrintingSystemCommandHandlerBase(HandleExportXls));
			handlers.Add(PrintingSystemCommand.ExportXlsx, new PrintingSystemCommandHandlerBase(HandleExportXlsx));
			handlers.Add(PrintingSystemCommand.SendCsv, new PrintingSystemCommandHandlerBase(HandleSendCsv));
			handlers.Add(PrintingSystemCommand.SendGraphic, new PrintingSystemCommandHandlerBase(HandleSendGraphic));
			handlers.Add(PrintingSystemCommand.SendMht, new PrintingSystemCommandHandlerBase(HandleSendMht));
			handlers.Add(PrintingSystemCommand.SendPdf, new PrintingSystemCommandHandlerBase(HandleSendPdf));
			handlers.Add(PrintingSystemCommand.SendRtf, new PrintingSystemCommandHandlerBase(HandleSendRtf));
			handlers.Add(PrintingSystemCommand.SendTxt, new PrintingSystemCommandHandlerBase(HandleSendTxt));
			handlers.Add(PrintingSystemCommand.SendXls, new PrintingSystemCommandHandlerBase(HandleSendXls));
			handlers.Add(PrintingSystemCommand.SendXlsx, new PrintingSystemCommandHandlerBase(HandleSendXlsx));
			handlers.Add(PrintingSystemCommand.StopPageBuilding, new PrintingSystemCommandHandlerBase(HandleStopPageBuilding));
		}
		void HandleExportCsv(object[] args) {
			DoExport(PrintingSystemBase.ExportOptions.Csv);
		}
		void HandleExportGraphic(object[] args) {
			DoExport(PrintingSystemBase.ExportOptions.Image);
		}
		void HandleExportHtm(object[] args) {
			DoExport(PrintingSystemBase.ExportOptions.Html);
		}
		void HandleExportMht(object[] args) {
			DoExport(PrintingSystemBase.ExportOptions.Mht);
		}
		void HandleExportPdf(object[] args) {
			DoExport(PrintingSystemBase.ExportOptions.Pdf);
		}
		void HandleExportRtf(object[] args) {
			DoExport(PrintingSystemBase.ExportOptions.Rtf);
		}
		void HandleExportTxt(object[] args) {
			DoExport(PrintingSystemBase.ExportOptions.Text);
		}
		void HandleExportXls(object[] args) {
			DoExport(PrintingSystemBase.ExportOptions.Xls);
		}
		void HandleExportXlsx(object[] args) {
			DoExport(PrintingSystemBase.ExportOptions.Xlsx);
		}
		void HandleSendCsv(object[] args) {
			SendFileByEmail(PrintingSystemBase.ExportOptions.Csv);
		}
		void HandleSendGraphic(object[] args) {
			SendFileByEmail(PrintingSystemBase.ExportOptions.Image);
		}
		void HandleSendMht(object[] args) {
			SendFileByEmail(PrintingSystemBase.ExportOptions.Mht);
		}
		void HandleSendPdf(object[] args) {
			SendFileByEmail(PrintingSystemBase.ExportOptions.Pdf);
		}
		void HandleSendRtf(object[] args) {
			SendFileByEmail(PrintingSystemBase.ExportOptions.Rtf);
		}
		void HandleSendTxt(object[] args) {
			SendFileByEmail(PrintingSystemBase.ExportOptions.Text);
		}
		void HandleSendXls(object[] args) {
			SendFileByEmail(PrintingSystemBase.ExportOptions.Xls);
		}
		void HandleSendXlsx(object[] args) {
			SendFileByEmail(PrintingSystemBase.ExportOptions.Xlsx);
		}
		protected virtual void DoExport(ExportOptionsBase options) {
			CreateExportFileHelper().ExecExport(options, DisabledExportModes);
		}
		protected virtual void SendFileByEmail(ExportOptionsBase options) {
			CreateExportFileHelper().SendFileByEmail(options, DisabledExportModes);
		}
		void HandleStopPageBuilding(object[] args) {
			PrintingSystemBase.Document.StopPageBuilding();
		}
		protected virtual IDictionary<Type, object[]> DisabledExportModes {
			get { return null; }
		}
		protected abstract ExportFileHelperBase CreateExportFileHelper();
	}
}
