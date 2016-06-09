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

using System.ComponentModel;
using DevExpress.XtraPrinting;
using DevExpress.XtraReports.Web.Native.DocumentMap;
namespace DevExpress.XtraReports.Web {
	public abstract class BookmarkFiller<T> : BookmarkFillerBase<T> {
		readonly IReportProvider reportProvider;
		protected override Document Document {
			get {
				var report = reportProvider.Report;
				if(report == null) {
					return null;
				}
				return report.PrintingSystem.Document;
			}
		}
		protected BookmarkFiller(ASPxDocumentViewer documentViewer)
			: this(documentViewer.ClientID, new DocumentViewerReportProvider(documentViewer)) {
		}
		[EditorBrowsable(EditorBrowsableState.Never)]
		protected BookmarkFiller(ReportViewer reportViewer)
			: this(reportViewer.ClientID, new ReportViewerReportProvider(reportViewer)) {
		}
		BookmarkFiller(string viewerClientId, IReportProvider reportProvider)
			: base(viewerClientId) {
			this.reportProvider = reportProvider;
		}
		public override void Fill(T parent) {
			reportProvider.PrepareDocument();
			base.Fill(parent);
		}
	}
}
