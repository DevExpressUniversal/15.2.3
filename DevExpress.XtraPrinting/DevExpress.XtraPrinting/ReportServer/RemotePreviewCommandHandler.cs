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

using DevExpress.XtraPrinting;
using DevExpress.XtraPrinting.Control;
using DevExpress.XtraPrinting.Export;
using System;
using System.Windows.Forms;
using DevExpress.XtraPrinting.Native;
using DevExpress.ReportServer.Printing.Services;
using DevExpress.ReportServer.ServiceModel.Native.RemoteOperations;
namespace DevExpress.ReportServer.Printing {
	class RemotePreviewCommandHandler : PSCommandHandlerBase, ICommandHandler {
		#region fields and properties
		readonly PrintingSystemBase ps;
		protected override PrintingSystemBase PrintingSystemBase { get { return ps; } }
		IPrintingSystemExtender Extender { get { return PrintingSystemBase.Extend(); } }
		IPrintControlActionInvoker ActionInvoker { get { return PrintingSystemBase.GetService<IPrintControlActionInvoker>(); } }
		#endregion
		#region ctor
		public RemotePreviewCommandHandler(PrintingSystemBase ps) {
			this.ps = ps;
			handlers.Add(PrintingSystemCommand.PrintDirect, new PrintingSystemCommandHandlerBase(HandlePrintDirect));
			handlers.Add(PrintingSystemCommand.Print, new PrintingSystemCommandHandlerBase(HandlePrint));
		}
		#endregion
		#region methods
		protected override ExportFileHelperBase CreateExportFileHelper() {
			var factory = ((IServiceProvider)ps).GetService<RemoteOperationFactory>();
			return new RemoteExportFileHelper(ps, new EmailSender(Extender!=null ? Extender.ActiveViewer : null), factory);
		}
		public bool CanHandleCommand(PrintingSystemCommand command, IPrintControl printControl) {
			return handlers.ContainsKey(command);
		}
		public void HandleCommand(PrintingSystemCommand command, object[] args, IPrintControl pc, ref bool handled) {
			object handler;
			if(handlers.TryGetValue(command, out handler)) {
				if(handler is PrintingSystemCommandHandlerBase) {
					((PrintingSystemCommandHandlerBase)handler)(args);
					handled = true;
					return;
				}
			}
			handled = false;
		}
		IRemotePrintService RemotePrintService {
			get {
				return (IRemotePrintService)((IServiceProvider)PrintingSystemBase).GetService(typeof(IRemotePrintService));
			}
		}
		void HandlePrintDirect(object[] args) {
			PrintControl pc = Extender.ActiveViewer;
			RemotePrintService.PrintDirect(0, PrintingSystemBase.PageCount - 1, (printerName) => { new PrintCommandExecutor(PrintingSystemBase,  pc.LookAndFeel, pc.FindForm()).Print(printerName); });
		}
		void HandlePrint(object[] args) {
			ActionInvoker.BeginInvoke(PrintDlgProc);
		}
#if DEBUG
		protected
#endif
		void PrintDlgProc() {
			RemotePrintService.Print(pd => {
				PrintingSystemBase.PrintDocument(pd);
				PrintingSystemBase.OnEndPrint(EventArgs.Empty);
			}, () => { return Extender.RunPrintDlg(); });
		}
		#endregion
	}
}
