#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       XtraReports for ASP.NET                                     }
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
using System.Collections.Concurrent;
using DevExpress.XtraPrinting;
namespace DevExpress.XtraReports.Web.WebDocumentViewer.Native {
	public class DocumentInfo : IWebPreviewDocument {
		const int StopBuildingRequested = 1;
		const int DisposeDocumentRequested = 2;
		volatile int documentRequestState = 0;
		Document document;
		public ConcurrentBag<IDocumentManagerRequest> Requests { get; private set; }
		public HotWebDocumentProgressObservable ProgressObservable { get; private set; }
		public Document Document {
			get { return document; }
			set {
				if(document != null) {
					throw new InvalidOperationException("Document is already assigned.");
				}
				document = value;
				ProgressObservable.AssignPrintingSystem(document.PrintingSystem);
			}
		}
		public string Id { get; private set; }
		public bool IsStopBuildRequested {
			get { return documentRequestState >= StopBuildingRequested; }
			set {
				if(!value)
					throw new InvalidOperationException("StopBuildRequest can not be cancelled");
				if(!IsDisposeDocumentRequested)
					documentRequestState = StopBuildingRequested;
			}
		}
		public bool IsDisposeDocumentRequested {
			get {
				return documentRequestState == DisposeDocumentRequested;
			}
			set {
				if(!value)
					throw new InvalidOperationException("DisposeDocumentRequest can not be cancelled");
				documentRequestState = DisposeDocumentRequested;
			}
		}
		public bool IsBuildCompleted { get; private set; }
		public DocumentInfo(string documentId) {
			Id = documentId;
			Requests = new ConcurrentBag<IDocumentManagerRequest>();
			ProgressObservable = new HotWebDocumentProgressObservable();
		}
		public void Complete(Exception optionanBuildException) {
			IsBuildCompleted = true;
			ProgressObservable.Complete(optionanBuildException);
		}
	}
}
