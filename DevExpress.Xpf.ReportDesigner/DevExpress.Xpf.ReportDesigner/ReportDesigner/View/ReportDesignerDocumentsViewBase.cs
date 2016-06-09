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
using System.ComponentModel;
using System.Linq;
using System.Windows;
using DevExpress.Mvvm;
using DevExpress.Mvvm.Native;
using DevExpress.Xpf.Reports.UserDesigner.Native;
namespace DevExpress.Xpf.Reports.UserDesigner {
	[TemplatePart(Name = PART_Root, Type = typeof(FrameworkElement))]
	public abstract class ReportDesignerDocumentsViewBase : ReportDesignerViewBase {
		public const string PART_Root = "Root";
		public static readonly object DocumentManagerServiceKey = new object();
		public override void OnApplyTemplate() {
			base.OnApplyTemplate();
			var templateRoot = (FrameworkElement)GetTemplateChild(PART_Root);
			DocumentManagerService = (IDocumentManagerService)templateRoot.With(x => x.Resources[DocumentManagerServiceKey]);
		}
		IDocumentManagerService documentManagerService;
		IDocumentManagerService DocumentManagerService {
			get { return documentManagerService; }
			set {
				if(documentManagerService == value) return;
				var oldValue = documentManagerService;
				documentManagerService = value;
				OnDocumentManagerServiceChanged(oldValue);
			}
		}
		protected virtual void OnDocumentManagerServiceChanged(IDocumentManagerService oldValue) {
			if(oldValue != null)
				oldValue.ActiveDocumentChanged -= OnDocumentManagerServiceActiveDocumentChanged;
			if(DocumentManagerService != null)
				DocumentManagerService.ActiveDocumentChanged += OnDocumentManagerServiceActiveDocumentChanged;
			documentManagerServiceActionsManager.ExecuteDelayedActions();
		}
		void OnDocumentManagerServiceActiveDocumentChanged(object sender, ActiveDocumentChangedEventArgs e) {
			ActiveDocumentViewSource = e.NewDocument.With(x => GetDocumentViewSource(x));
		}
		readonly DelayedActionsManager documentManagerServiceActionsManager = new DelayedActionsManager();
		protected void DoWithDocumentManagerService(Action<IDocumentManagerService> action) {
			documentManagerServiceActionsManager.DoAction(() => action(DocumentManagerService));
		}
		protected override ReportDesignerDocumentViewSource OpenDocument(ReportDesignerDocument document, Action<ReportDesignerDocument> onDocumentOpened) {
			var designerPaneSource = new DesignerPaneDocumentContent();
			designerPaneSource.SetSource(document);
			DoWithDocumentManagerService(documentManagerService => {
				var visualDocument = documentManagerService.CreateDocument(designerPaneSource);
				visualDocument.DestroyOnClose = true;
				visualDocument.Show();
				onDocumentOpened(designerPaneSource.Document);
			});
			return designerPaneSource;
		}
		protected override void DestroyDocument(ReportDesignerDocument document) {
			DoWithDocumentManagerService(documentManagerService => {
				var visualDocuments = documentManagerService.Documents.Where(x => GetDocumentViewSource(x).Document == document).ToArray();
				if(visualDocuments.Length == 0)
					throw new InvalidOperationException(); 
				foreach(var visualDocument in visualDocuments)
					visualDocument.Close(true);
			});
		}
		protected void DoWithVisualDocument(Func<ReportDesignerDocument> getDocumentFunc, Action<IDocument> action) {
			DoWithDocumentManagerService(documentManagerService => {
				var document = getDocumentFunc();
				Func<IDocument, bool> matchVisualDocument = visualDocument => {
					var viewSource = GetDocumentViewSource(visualDocument);
					return viewSource.Kind == ReportDesignerDocumentViewKind.Designer && viewSource.Document == document;
				};
				action(documentManagerService.Documents.Where(matchVisualDocument).First());
			});
		}
		protected ReportDesignerDocumentViewSource GetDocumentViewSource(IDocument visualDocument) {
			return (ReportDesignerDocumentViewSource)visualDocument.Content;
		}
		protected override void ActivateDocument(ReportDesignerDocument document) {
			DoWithDocumentManagerService(documentManagerService => {
				var visualDocument = documentManagerService.Documents.FirstOrDefault(x => GetDocumentViewSource(x).Document == document);
				visualDocument.Show();
			});
		}
		sealed class DesignerPaneDocumentContent : ReportDesignerDocumentViewSource, IDocumentContent {
			IDocumentOwner documentOwner;
			IDocumentOwner IDocumentContent.DocumentOwner {
				get { return documentOwner; }
				set { documentOwner = value; }
			}
			object IDocumentContent.Title { get { return Title; } }
			void IDocumentContent.OnClose(CancelEventArgs e) {
				IReportDesignerDocument document = Document;
				if(document != null)
					document.OnClosing(e);
			}
			void IDocumentContent.OnDestroy() {
				IReportDesignerDocument document = Document;
				if(document != null)
					document.OnClosed();
			}
		}
	}
}
