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
using DevExpress.XtraRichEdit.Layout;
using DevExpress.XtraRichEdit.Model;
using DevExpress.XtraRichEdit.Internal;
using DevExpress.XtraRichEdit.Layout.Export;
using DevExpress.Utils;
namespace DevExpress.XtraRichEdit.Services.Implementation {
	#region IDocumentLayoutService
	public interface IDocumentLayoutService {
		DocumentLayout CalculateDocumentLayout();
		void ResetLayout();
		IDocumentLayoutService CreateService(DocumentModel documentModel);
		void RemoveServise(DocumentModel documentModel);
	}
	#endregion
	#region DocumentServerLayoutService
	public class DocumentServerLayoutService : IDocumentLayoutService {
		readonly InnerRichEditDocumentServer server;
		DocumentLayout cachedLayout;
		public DocumentServerLayoutService(InnerRichEditDocumentServer server) {
			Guard.ArgumentNotNull(server, "server");
			this.server = server;
			SubscribeEvents();
		}
		void SubscribeEvents() {
			server.DocumentModel.BeforeEndDocumentUpdate += OnDocumentModelBeforeEndDocumentUpdate;
		}
		void UnsubscribeEvents() {
			server.DocumentModel.BeforeEndDocumentUpdate -= OnDocumentModelBeforeEndDocumentUpdate;
		}
		InnerRichEditDocumentServer Server { get { return server; } }
		void OnDocumentModelBeforeEndDocumentUpdate(object sender, DocumentUpdateCompleteEventArgs e) {
			DocumentModelChangeActions changes = e.DeferredChanges.ChangeActions;
			if ((changes & DocumentModelChangeActions.ResetAllPrimaryLayout) != 0 ||
				(changes & DocumentModelChangeActions.ResetPrimaryLayout) != 0 ||
				(changes & DocumentModelChangeActions.ResetSecondaryLayout) != 0)
				this.cachedLayout = null;
		}
		#region IDocumentLayoutService Members
		public DocumentLayout CalculateDocumentLayout() {
			if (cachedLayout != null)
				return cachedLayout;
			bool useGdiPlus = false;
			InnerRichEditControl control = server as InnerRichEditControl;
			if (control != null)
				useGdiPlus = control.Owner.UseGdiPlus;
			if (control != null)
				control.Formatter.BeginDocumentUpdate();
			Server.DocumentModel.ResetParagraphs();
			try {
				using (BrickDocumentPrinter documentPrinter = new BrickDocumentPrinter(Server.DocumentModel, useGdiPlus)) {
					documentPrinter.Format();
					cachedLayout = documentPrinter.DocumentLayout;
				}
			}
			finally {
				Server.DocumentModel.ResetParagraphs();
				if (control != null)
					control.Formatter.EndDocumentUpdate();
			}
			return cachedLayout;
		}
		public void ResetLayout() {
			cachedLayout = null;
		}
		public IDocumentLayoutService CreateService(DocumentModel documentModel) {
			InnerRichEditDocumentServer server = new InnerRichEditDocumentServer(Server.Owner, documentModel);
			server.BeginInitialize();
			server.EndInitialize();
			return documentModel.GetService<IDocumentLayoutService>();
		}
		public void RemoveServise(DocumentModel documentModel) {
			DocumentServerLayoutService service = documentModel.GetService<IDocumentLayoutService>() as DocumentServerLayoutService;
			service.UnsubscribeEvents();
			if (service != null)
				service.Server.Dispose();
			documentModel.RemoveService(typeof(IDocumentLayoutService));
		}
		#endregion
	}
	#endregion
}
