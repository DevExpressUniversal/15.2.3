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
using System.Web.UI.Design;
using DevExpress.XtraReports.Web;
using DevExpress.XtraReports.Web.Native.DocumentViewer;
using DevExpress.XtraReports.Web.WebDocumentViewer;
namespace DevExpress.Web.Design.Reports {
	static class ViewersSwitcher {
		public static void Switch(ASPxWebDocumentViewer component, IDesignerHost designerHost) {
			var rootDesigner = (WebFormsRootDesigner)designerHost.GetDesigner(designerHost.RootComponent);
			var transaction = designerHost.CreateTransaction();
			DesignModeSwitcher switcher = null;
			try {
				ASPxDocumentViewer documentViewer = CreateASPxDocumentViewer(component);
				switcher = new DesignModeSwitcher(documentViewer);
				rootDesigner.AddControlToDocument(documentViewer, component, ControlLocation.After);
				rootDesigner.RemoveControlFromDocument(component);
				transaction.Commit();
			} finally {
				((IDisposable)transaction).Dispose();
				if(switcher != null) {
					switcher.Dispose();
				}
			}
		}
		public static void Switch(ASPxDocumentViewer component, IDesignerHost designerHost) {
			var rootDesigner = (WebFormsRootDesigner)designerHost.GetDesigner(designerHost.RootComponent);
			using(var transaction = designerHost.CreateTransaction()) {
				var webDocumentViewer = CreateASPxWebDocumentViewer(component);
				rootDesigner.AddControlToDocument(webDocumentViewer, component, ControlLocation.After);
				rootDesigner.RemoveControlFromDocument(component);
				transaction.Commit();
			}
		}
		static ASPxWebDocumentViewer CreateASPxWebDocumentViewer(ASPxDocumentViewer documentViewer) {
			var webDocumentViewer = new ASPxWebDocumentViewer();
			if(!documentViewer.Width.IsEmpty) {
				webDocumentViewer.Width = documentViewer.Width;
			}
			if(!documentViewer.Height.IsEmpty) {
				webDocumentViewer.Height = documentViewer.Height;
			}
			if(!string.IsNullOrEmpty(documentViewer.ReportTypeName)) {
				webDocumentViewer.ReportSourceId = documentViewer.ReportTypeName;
				webDocumentViewer.ReportSourceKind = ReportSourceKind.ReportType;
			}
			return webDocumentViewer;
		}
		static ASPxDocumentViewer CreateASPxDocumentViewer(ASPxWebDocumentViewer webDocumentViewer) {
			var documentViewer = new ASPxDocumentViewer();
			if(!webDocumentViewer.Width.IsEmpty) {
				documentViewer.Width = webDocumentViewer.Width;
			}
			if(!webDocumentViewer.Height.IsEmpty) {
				documentViewer.Height = webDocumentViewer.Height;
			}
			if(webDocumentViewer.ReportSourceKind == ReportSourceKind.ReportType) {
				documentViewer.ReportTypeName = webDocumentViewer.ReportSourceId;
			}
			return documentViewer;
		}
	}
}
