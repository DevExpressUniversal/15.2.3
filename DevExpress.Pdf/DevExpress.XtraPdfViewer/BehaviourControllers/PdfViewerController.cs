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
using System.Threading;
using System.Windows.Forms;
using DevExpress.Pdf.Drawing;
using DevExpress.Pdf.Native;
namespace DevExpress.XtraPdfViewer.Native {
	public class PdfViewerController : PdfDisposableObject, IPdfViewerController {
		readonly PdfViewer viewer;
		readonly SynchronizationContext synchronizationContext = new WindowsFormsSynchronizationContext();
		readonly PdfViewerNavigationController navigationController;
		readonly PdfViewerValueEditingController valueEditingController;
		readonly PdfViewerActionController actionController;
		public IPdfViewerNavigationController NavigationController { get { return navigationController; } }
		public PdfDocumentViewStateHistoryController HistoryController { get { return navigationController.HistoryController; } }
		SynchronizationContext IPdfViewerController.SynchronizationContext { get { return synchronizationContext; } }
		PdfViewerTool IPdfViewerController.ViewerTool { get { return viewer.HandTool ? PdfViewerTool.Hand : PdfViewerTool.Selection; } }
		bool IPdfViewerController.ReadOnly { get { return viewer.ReadOnly; } }
		IPdfViewerActionController IPdfViewerController.ActionController { get { return actionController; } }
		IPdfViewerValueEditingController IPdfViewerController.ValueEditingController { get { return valueEditingController; } }
		public PdfViewerController(PdfViewer viewer) {
			this.viewer = viewer;
			navigationController = new PdfViewerNavigationController(viewer);
			valueEditingController = new PdfViewerValueEditingController(viewer);
			actionController = new PdfViewerActionController(viewer);
		}
		protected override void Dispose(bool disposing) {
			if (disposing) 
				valueEditingController.Dispose();
		}
	}
}
