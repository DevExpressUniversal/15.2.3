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
using DevExpress.Mvvm;
using DevExpress.Mvvm.Native;
namespace DevExpress.Xpf.Reports.UserDesigner.Native {
	public abstract class ReportDesignerDocumentViewSource : BindableBase, IReportDesignerDocumentViewSource {
		public ReportDesignerDocumentViewKind Kind {
			get { return document.Return(x => x.ViewKind, () => (ReportDesignerDocumentViewKind)ReportDesignerDocument.ViewKindProperty.DefaultMetadata.DefaultValue); }
		}
		ReportDesignerDocument document;
		public ReportDesignerDocument Document {
			get { return document; }
			private set {
				var oldDocument = document;
				if(SetProperty(ref document, value, () => Document))
					OnDocumentChanged(oldDocument);
			}
		}
		public event EventHandler DocumentChanged;
		public event EventHandler DocumentSourceChanged;
		IReportDesignerDocumentViewSource source;
		public void SetSource(ReportDesignerDocument document) {
			SetSource(new DocumentAsSource(document));
		}
		public void SetSource(IReportDesignerDocumentViewSource source) {
			if(this.source != null)
				this.source.DocumentChanged -= OnSourceDocumentChanged;
			this.source = source;
			if(this.source != null)
				this.source.DocumentChanged += OnSourceDocumentChanged;
			UpdateDocument();
		}
		void OnSourceDocumentChanged(object sender, EventArgs e) {
			UpdateDocument();
		}
		void UpdateDocument() {
			Document = source.With(x => x.Document);
		}
		public string Title { get { return Document.With(x => x.Title); } }
		protected virtual void OnDocumentChanged(ReportDesignerDocument oldDocument) {
			if(oldDocument != null) {
				oldDocument.SourceChanged -= OnDocumentSourceChanged;
				oldDocument.TitleChanged -= OnDocumentTitleChanged;
			}
			if(Document != null) {
				Document.SourceChanged += OnDocumentSourceChanged;
				Document.TitleChanged += OnDocumentTitleChanged;
			}
			if(DocumentChanged != null)
				DocumentChanged(this, EventArgs.Empty);
			RaiseDocumentSourceChanged();
			RaisePropertyChanged(() => Title);
		}
		void OnDocumentSourceChanged(object sender, EventArgs e) {
			RaiseDocumentSourceChanged();
		}
		void OnDocumentTitleChanged(object sender, EventArgs e) {
			RaisePropertyChanged(() => Title);
		}
		void RaiseDocumentSourceChanged() {
			if(DocumentSourceChanged != null)
				DocumentSourceChanged(this, EventArgs.Empty);
		}
		sealed class DocumentAsSource : IReportDesignerDocumentViewSource {
			readonly ReportDesignerDocument document;
			public DocumentAsSource(ReportDesignerDocument document) {
				this.document = document;
			}
			ReportDesignerDocument IReportDesignerDocumentViewSource.Document { get { return document; } }
			event EventHandler IReportDesignerDocumentViewSource.DocumentChanged { add { } remove { } }
		}
		IReportDesignerDocumentViewSource attachedSelector;
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public IReportDesignerDocumentViewSource AttachedSelector {
			get { return attachedSelector; }
			set { SetProperty(ref attachedSelector, value, () => AttachedSelector); }
		}
	}
}
