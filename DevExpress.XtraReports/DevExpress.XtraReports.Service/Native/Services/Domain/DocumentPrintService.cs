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

using System.Diagnostics;
using System.IO;
using DevExpress.DocumentServices.ServiceModel.DataContracts;
using DevExpress.Utils;
using DevExpress.XtraReports.Service.Native.Services.Factories;
using DevExpress.XtraReports.Service.Native.Services.Transient;
namespace DevExpress.XtraReports.Service.Native.Services.Domain {
	public class DocumentPrintService : IDocumentPrintService {
		readonly IPrintFactory printFactory;
		readonly IThreadFactoryService threadFactory;
		public DocumentPrintService(
			IPrintFactory printFactory,
			IThreadFactoryService threadFactory) {
			Guard.ArgumentNotNull(printFactory, "printFactory");
			Guard.ArgumentNotNull(threadFactory, "threadFactory");
			this.printFactory = printFactory;
			this.threadFactory = threadFactory;
		}
		#region IPrintService Members
		public virtual PrintId StartPrint(DocumentId documentId, PageCompatibility compatibility) {
			var printId = PrintId.GenerateNew();
			using(var mediator = printFactory.CreatePrintMediator(printId, MediatorInitialization.New)) {
				mediator.Entity.Status = TaskStatus.InProgress;
				mediator.Save();
			}
			var printer = printFactory.CreateDocumentPrinter();
			threadFactory.Start(() => printer.PreparePages(documentId, printId, compatibility));
			return printId;
		}
		public virtual void StopPrint(PrintId printId) {
			using(var mediator = printFactory.CreatePrintMediator(printId)) {
				mediator.ShouldStop = true;
				mediator.Save();
			}
		}
		public virtual PrintStatus GetPrintStatus(PrintId printId) {
			using(var mediator = printFactory.CreatePrintMediator(printId)) {
				var entity = mediator.Entity;
				return new PrintStatus {
					PrintId = printId,
					Status = entity.Status,
					ProgressPosition = entity.ProgressPosition,
					Fault = mediator.Fault
				};
			}
		}
		public virtual Stream GetPrintDocument(PrintId printId) {
			using(var mediator = printFactory.CreatePrintMediator(printId)) {
				Debug.Assert(mediator.Entity.Status == TaskStatus.Complete, ReportServiceMessages.NoPrintDocument);
				return mediator.Content;
			}
		}
		#endregion
	}
}
