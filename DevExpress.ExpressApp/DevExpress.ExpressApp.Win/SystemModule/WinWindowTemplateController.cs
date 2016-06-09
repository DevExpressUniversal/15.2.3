#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       eXpressApp Framework                                        }
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
using System.Windows.Forms;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.ExpressApp.Utils;
using DevExpress.ExpressApp.Win.Templates;
using DevExpress.XtraBars.Docking2010;
using DevExpress.XtraBars.Docking2010.Views;
namespace DevExpress.ExpressApp.Win.SystemModule {
	public class WinWindowTemplateController : WindowTemplateController {
		private void SubscribeTemplateEvents() {
			if(FormTemplate != null) {
				FormTemplate.Load += FormTemplate_Load;
			}
			if(DocumentsHostWindow != null && DocumentsHostWindow.DocumentManager != null) {
				DocumentsHostWindow.DocumentManager.ViewChanging += DocumentManager_ViewChanging;
				DocumentsHostWindow.DocumentManager.ViewChanged += DocumentManager_ViewChanged;
			}
			SubscribeDocumentManagerViewEvents(DocumentManagerView);
		}
		private void UnsubscribeTemplateEvents() {
			if(FormTemplate != null) {
				FormTemplate.Load -= FormTemplate_Load;
			}
			if(DocumentsHostWindow != null && DocumentsHostWindow.DocumentManager != null) {
				DocumentsHostWindow.DocumentManager.ViewChanging -= DocumentManager_ViewChanging;
				DocumentsHostWindow.DocumentManager.ViewChanged -= DocumentManager_ViewChanged;
			}
			UnsubscribeDocumentManagerViewEvents(DocumentManagerView);
		}
		private void SubscribeDocumentManagerViewEvents(BaseView view) {
			if(view != null) {
				view.DocumentActivated += View_DocumentActivated;
				view.DocumentDeactivated += View_DocumentDeactivated;
			}
		}
		private void UnsubscribeDocumentManagerViewEvents(BaseView view) {
			if(view != null) {
				view.DocumentActivated -= View_DocumentActivated;
				view.DocumentDeactivated -= View_DocumentDeactivated;
			}
		}
		private void Window_TemplateChanging(object sender, EventArgs e) {
			UnsubscribeTemplateEvents();
		}
		private void Window_TemplateChanged(object sender, EventArgs e) {
			SubscribeTemplateEvents();
		}
		private void DocumentManager_ViewChanging(object sender, ViewEventArgs args) {
			UnsubscribeDocumentManagerViewEvents(args.View);
		}
		private void DocumentManager_ViewChanged(object sender, ViewEventArgs args) {
			SubscribeDocumentManagerViewEvents(args.View);
		}
		private void FormTemplate_Load(object sender, EventArgs e) {
			if(DocumentManagerView != null && DocumentManagerView.ActiveDocument != null && DocumentManagerView.ActiveDocument.Form != null) {
				UpdateWindowCaption(DocumentManagerView.ActiveDocument.Caption);
			}
		}
		private void View_DocumentActivated(object sender, DocumentEventArgs e) {
			OnDocumentActivated(e);
		}
		private void View_DocumentDeactivated(object sender, DocumentEventArgs e) {
			OnDocumentDeactivated(sender, e);
		}
		private void DocumentForm_TextChanged(object sender, EventArgs e) {
			UpdateWindowCaption(((Form)sender).Text);
		}
		private void DocumentForm_FormClosed(object sender, FormClosedEventArgs e) {
			((Form)sender).TextChanged -= DocumentForm_TextChanged;
			((Form)sender).FormClosed -= DocumentForm_FormClosed;
		}
		protected virtual void OnDocumentActivated(DocumentEventArgs e) {
			if(!e.Document.IsFloating) {
				UpdateWindowCaption(e.Document.Form.Text);
				e.Document.Form.TextChanged += DocumentForm_TextChanged;
				e.Document.Form.FormClosed += DocumentForm_FormClosed;
			}
		}
		protected virtual void OnDocumentDeactivated(object sender, DocumentEventArgs e) {
			if(e.Document.Form != null && !e.Document.IsFloating) {
				e.Document.Form.TextChanged -= DocumentForm_TextChanged;
				e.Document.Form.FormClosed -= DocumentForm_FormClosed;
			}
			if(((BaseView)sender).Documents.Count == 0) {
				UpdateWindowCaption();
			}
		}
		protected override SplitString GetWindowCaption(string formText) {
			SplitString result = new SplitString();
			if(FormTemplate != null && IsMdi) {
				if(FormTemplate.MdiChildren.Length > 0 && formText != null) {
					result.FirstPart = formText;
				}
				result.SecondPart = Application.Model.Title;
			}
			else {
				result = base.GetWindowCaption(formText);
			}
			return result;
		}
		protected override void OnActivated() {
			base.OnActivated();
			Window.TemplateChanging += Window_TemplateChanging;
			Window.TemplateChanged += Window_TemplateChanged;
			SubscribeTemplateEvents();
		}
		protected override void OnDeactivated() {
			UnsubscribeTemplateEvents();
			Window.TemplateChanging -= Window_TemplateChanging;
			Window.TemplateChanged -= Window_TemplateChanged;
			base.OnDeactivated();
		}
		protected BaseView DocumentManagerView {
			get {
				if(DocumentsHostWindow != null && DocumentsHostWindow.DocumentManager != null) {
					return DocumentsHostWindow.DocumentManager.View;
				}
				return null;
			}
		}
		public bool IsMdi {
			get {
				if(DocumentsHostWindow != null) {
					return DocumentsHostWindow.UIType == UIType.TabbedMDI || DocumentsHostWindow.UIType == UIType.StandardMDI;
				}
				return false;
			}
		}
		public Form FormTemplate {
			get { return Window.Template as Form; }
		}
		public IXafDocumentsHostWindow DocumentsHostWindow {
			get { return Window.Template as IXafDocumentsHostWindow; }
		}
		#region Obsolete 14.2
		[Obsolete("Use the FormTemplate or DocumentsHostWindow properties instead.")]
		public MainFormTemplateBase MainFormTemplateBase {
			get { return Window.Template as MainFormTemplateBase; }
		}
		#endregion
	}
}
