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
using System.Drawing.Printing;
using System.Collections.Generic;
namespace DevExpress.ReportServer.Printing.Services {
	class RemotePrintService : IRemotePrintService {
		#region fields and properties
		readonly IPageListService pageListService;
		Action printAction;
		readonly Action<int[]> invalidatePagesAction;
		#endregion
		#region ctor
		public RemotePrintService(IPageListService pageListService, Action<int[]> invalidatePagesAction) {
			this.pageListService = pageListService;
			this.invalidatePagesAction = invalidatePagesAction;
		}
		#endregion
		#region IRemotePrintService
		public void Print(Action<PrintDocument> printAction, Func<PrintDocument> runDialogAction) {
			PrintDocument printDocument = runDialogAction();
			if(printDocument == null)
				return;
			this.printAction = () => {
				printAction(printDocument);
				printDocument.Dispose();
			};
			PrintPages(printDocument.PrinterSettings.FromPage - 1, printDocument.PrinterSettings.ToPage - 1);
		}
		public void PrintDirect(int fromIndex, int toIndex, Action<string> printDirectAction) {
			printAction = () => { printDirectAction(String.Empty); };
			PrintPages(fromIndex, toIndex);
		}
		#endregion
		#region methods
		void PrintPages(int fromIndex, int toIndex) {
			var indexes = CreatePageIndexes(fromIndex, toIndex);
			if(pageListService.PagesShouldBeLoaded(indexes)) {
				pageListService.Ensure(indexes, result => {
					invalidatePagesAction(indexes);
					printAction();
				});
				return;
			}
			printAction();
		}
		static int[] CreatePageIndexes(int from, int to) {
			List<int> indexes = new List<int>();
			for(int i = from; i <= to; i++)
				indexes.Add(i);
			return indexes.ToArray();
		}
		#endregion
	}
}
