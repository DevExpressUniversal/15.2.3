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
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using DevExpress.Mvvm;
using DevExpress.Mvvm.Native;
using DevExpress.Mvvm.UI.Native;
using DevExpress.Xpf.Reports.UserDesigner.Native;
using DevExpress.Xpf.Reports.UserDesigner.ReportModel;
using DevExpress.Xpf.Reports.UserDesigner.XRDiagram;
using DevExpress.XtraReports.UI;
namespace DevExpress.Xpf.Reports.UserDesigner {
	[StyleTypedProperty(Property = "DiagramStyle", StyleTargetType = typeof(XRDiagramControl))]
	public abstract class ReportDesignerBase : Control, INativeReportDesignerUI {
		public static readonly DependencyProperty OpenFileDialogServiceTemplateProperty;
		static readonly Action<ReportDesignerBase, Action<IOpenFileDialogService>> openFileDialogServiceAccessor;
		public static readonly DependencyProperty SaveFileDialogServiceTemplateProperty;
		static readonly Action<ReportDesignerBase, Action<ISaveFileDialogService>> saveFileDialogServiceAccessor;
		public static readonly DependencyProperty MessageBoxServiceTemplateProperty;
		static readonly Action<ReportDesignerBase, Action<IMessageBoxService>> messageBoxServiceAccessor;
		public static readonly DependencyProperty ReportSerializerProperty;
		public static readonly DependencyProperty ReportStorageProperty;
		public static readonly DependencyProperty ModelRegistriesProperty;
		public static readonly DependencyProperty DiagramStyleProperty;
		public static readonly DependencyProperty WizardDialogServiceTemplateProperty;
		static readonly Action<ReportDesignerBase, Action<IDialogService>> wizardDialogServiceTemplateAccessor;
		public static readonly DependencyProperty EnableCustomSqlProperty;
		static ReportDesignerBase() {
			AssemblyInitializer.Init();
			DependencyPropertyRegistrator<ReportDesignerBase>.New()
				.RegisterServiceTemplateProperty(d => d.OpenFileDialogServiceTemplate, out OpenFileDialogServiceTemplateProperty, out openFileDialogServiceAccessor)
				.RegisterServiceTemplateProperty(d => d.SaveFileDialogServiceTemplate, out SaveFileDialogServiceTemplateProperty, out saveFileDialogServiceAccessor)
				.RegisterServiceTemplateProperty(d => d.MessageBoxServiceTemplate, out MessageBoxServiceTemplateProperty, out messageBoxServiceAccessor)
				.RegisterServiceTemplateProperty(d => d.WizardDialogServiceTemplate, out WizardDialogServiceTemplateProperty, out wizardDialogServiceTemplateAccessor)
				.Register(d => d.ReportSerializer, out ReportSerializerProperty, new DefaultReportSerializer())
				.Register(d => d.ReportStorage, out ReportStorageProperty, new DefaultReportStorage())
				.Register(d => d.ModelRegistries, out ModelRegistriesProperty, new StandardModelsRegistry().Yield().ToList().AsReadOnly())
				.Register(d => d.DiagramStyle, out DiagramStyleProperty, null)
				.Register(owner => owner.EnableCustomSql, out EnableCustomSqlProperty, false)
				.OverrideDefaultStyleKey();
			;
		}
		public Style DiagramStyle {
			get { return (Style)GetValue(DiagramStyleProperty); }
			set { SetValue(DiagramStyleProperty, value); }
		}
		public IEnumerable<IModelRegistry> ModelRegistries {
			get { return (IEnumerable<IModelRegistry>)GetValue(ModelRegistriesProperty); }
			set { SetValue(ModelRegistriesProperty, value); }
		}
		public IReportSerializer ReportSerializer {
			get { return (IReportSerializer)GetValue(ReportSerializerProperty); }
			set { SetValue(ReportSerializerProperty, value); }
		}
		public IReportStorage ReportStorage {
			get { return (IReportStorage)GetValue(ReportStorageProperty); }
			set { SetValue(ReportStorageProperty, value); }
		}
		public bool EnableCustomSql {
			get { return (bool)GetValue(EnableCustomSqlProperty); }
			set { SetValue(EnableCustomSqlProperty, value); }
		}
		protected virtual ReportDesignerDocument CreateDocument() { return new ReportDesignerDocument(this); }
		protected void DoWithOpenFileDialogService(Action<IOpenFileDialogService> action) { openFileDialogServiceAccessor(this, action); }
		public DataTemplate OpenFileDialogServiceTemplate {
			get { return (DataTemplate)GetValue(OpenFileDialogServiceTemplateProperty); }
			set { SetValue(OpenFileDialogServiceTemplateProperty, value); }
		}
		protected void DoWithSaveFileDialogService(Action<ISaveFileDialogService> action) { saveFileDialogServiceAccessor(this, action); }
		public DataTemplate SaveFileDialogServiceTemplate {
			get { return (DataTemplate)GetValue(SaveFileDialogServiceTemplateProperty); }
			set { SetValue(SaveFileDialogServiceTemplateProperty, value); }
		}
		protected void DoWithMessageBoxService(Action<IMessageBoxService> action) { messageBoxServiceAccessor(this, action); }
		public DataTemplate MessageBoxServiceTemplate {
			get { return (DataTemplate)GetValue(MessageBoxServiceTemplateProperty); }
			set { SetValue(MessageBoxServiceTemplateProperty, value); }
		}
		protected void DoWithWizardDialogService(Action<IDialogService> action) { wizardDialogServiceTemplateAccessor(this, action); }
		public DataTemplate WizardDialogServiceTemplate {
			get { return (DataTemplate)GetValue(WizardDialogServiceTemplateProperty); }
			set { SetValue(WizardDialogServiceTemplateProperty, value); }
		}
		public event EventHandler<ReportDesignerDocumentSubreportLoadFailedEventArgs> SubreportLoadFailed;
		public event EventHandler<ReportDesignerDocumentLoadFailedEventArgs> DocumentLoadFailed;
		public event EventHandler<ReportDesignerDocumentEventArgs> DocumentLoaded;
		public event EventHandler<ReportDesignerDocumentSaveFailedEventArgs> DocumentSaveFailed;
		public event EventHandler<ReportDesignerDocumentEventArgs> DocumentSaved;
		public event EventHandler<ReportDesignerDocumentClosingEventArgs> DocumentClosing;
		public event EventHandler<ReportDesignerDocumentEventArgs> DocumentClosed;
		public event EventHandler<ReportDesignerDocumentEventArgs> DocumentOpened;
		protected void DoOperationWithDetachedDocument(ReportDesignerDocument document, Action<ReportDesignerDocument> action) {
			AttachDocument(document);
			try {
				action(document);
			} finally {
				DetachDocument(document);
			}
		}
		protected virtual void AttachDocument(ReportDesignerDocument document) {
			document.LoadFailed += OnDocumentLoadFailed;
			document.SubreportLoadFailed += OnDocumentSubreportLoadFailed;
			document.SaveFailed += OnDocumentSaveFailed;
			document.Closed += OnDocumentClosed;
			document.Closing += OnDocumentClosing;
			document.Saved += OnDocumentSaved;
			document.Loaded += OnDocumentLoaded;
		}
		protected virtual void DetachDocument(ReportDesignerDocument document) {
			document.Loaded -= OnDocumentLoaded;
			document.Saved -= OnDocumentSaved;
			document.Closing -= OnDocumentClosing;
			document.Closed -= OnDocumentClosed;
			document.LoadFailed -= OnDocumentLoadFailed;
			document.SubreportLoadFailed -= OnDocumentSubreportLoadFailed;
			document.SaveFailed -= OnDocumentSaveFailed;
		}
		const string DefaultReportNamePrefix = "Report";
		protected virtual string GetNewDocumentName() {
			int existingReportIndex = GetExistingDocuments()
				.Select(x => x.Title)
				.Where(x => x.StartsWith(DefaultReportNamePrefix))
				.Select(x => x.Substring(DefaultReportNamePrefix.Length))
				.Select(x => { int v; return int.TryParse(x, NumberStyles.None, CultureInfo.InvariantCulture, out v) ? v : 0; })
				.Where(x => x > 0)
				.DefaultIfEmpty(0)
				.Max();
			return DefaultReportNamePrefix + (existingReportIndex + 1).ToString(CultureInfo.InvariantCulture);
		}
		protected abstract IEnumerable<ReportDesignerDocument> GetExistingDocuments();
		protected abstract void DestroyDocument(ReportDesignerDocument document);
		protected virtual void OnDocumentSaveFailed(object sender, ReportDesignerDocumentSaveFailedEventArgs e) {
			if(DocumentSaveFailed != null)
				DocumentSaveFailed(this, e);
		}
		protected virtual void OnDocumentLoadFailed(object sender, ReportDesignerDocumentLoadFailedEventArgs e) {
			if(DocumentLoadFailed != null)
				DocumentLoadFailed(this, e);
		}
		protected virtual void OnDocumentSubreportLoadFailed(object sender, ReportDesignerDocumentSubreportLoadFailedEventArgs e) {
			if(SubreportLoadFailed != null)
				SubreportLoadFailed(this, e);
		}
		protected virtual void OnDocumentLoaded(object sender, ReportDesignerDocumentEventArgs e) {
			if(DocumentLoaded != null)
				DocumentLoaded(this, e);
		}
		protected virtual void OnDocumentSaved(object sender, ReportDesignerDocumentEventArgs e) {
			if(DocumentSaved != null)
				DocumentSaved(this, e);
		}
		protected virtual void OnDocumentClosing(object sender, ReportDesignerDocumentClosingEventArgs e) {
			if(DocumentClosing != null)
				DocumentClosing(this, e);
		}
		protected virtual void OnDocumentOpened(object sender, ReportDesignerDocumentEventArgs e) {
			if(DocumentOpened != null)
				DocumentOpened(this, e);
		}
		protected virtual void OnDocumentClosed(object sender, ReportDesignerDocumentEventArgs e) {
			if(DocumentClosed != null)
				DocumentClosed(this, e);
		}
		protected void RaiseDocumentOpened(ReportDesignerDocument document) {
			OnDocumentOpened(this, new ReportDesignerDocumentEventArgs(document));
		}
		protected ReportDesignerDocument OpenDocumentAsSingle(Func<ReportDesignerDocument, bool> loadAction, Func<ReportDesignerDocument> getDocument, Action<ReportDesignerDocument> setDocument) {
			IReportDesignerDocument document = getDocument();
			if(document != null)
				PrepareToCloseDocument(false, getDocument());
			return CreateAndAttachNewDocumentAndDestroyCurrent(loadAction, getDocument, setDocument) ? getDocument() : null;
		}
		protected bool PrepareToCloseDocument(bool force, ReportDesignerDocument document) {
			IReportDesignerDocument nativeDocument = document;
			CancelEventArgs args = new CancelEventArgs();
			if(!force)
				nativeDocument.OnClosing(args);
			return !args.Cancel;
		}
		protected bool CreateAndAttachNewDocumentAndDestroyCurrent(Func<ReportDesignerDocument, bool> initDocumentAction, Func<ReportDesignerDocument> getDocument, Action<ReportDesignerDocument> setDocument) {
			var newDocument = CreateDocument();
			bool initialized = false;
			DoOperationWithDetachedDocument(newDocument, x => initialized = initDocumentAction(x));
			if(!initialized) return false;
			AttachNewDocumentAndDestroyCurrent(newDocument, getDocument, setDocument);
			return true;
		}
		protected void AttachNewDocumentAndDestroyCurrent(ReportDesignerDocument newDocument, Func<ReportDesignerDocument> getDocument, Action<ReportDesignerDocument> setDocument) {
			var oldDocument = getDocument();
			setDocument(newDocument);
			if(oldDocument != null) {
				DoOperationWithDetachedDocument(oldDocument, x => {
					IReportDesignerDocument document = x;
					document.OnClosed();
				});
			}
			RaiseDocumentOpened(getDocument());
		}
		#region IReportDesignerUI
		void IReportDesignerUI.DoWithMessageBoxService(Action<IMessageBoxService> action) {
			DoWithMessageBoxService(action);
		}
		void IReportDesignerUI.DoWithOpenFileDialogService(Action<IOpenFileDialogService> action) {
			DoWithOpenFileDialogService(action);
		}
		void IReportDesignerUI.DoWithSaveFileDialogService(Action<ISaveFileDialogService> action) {
			DoWithSaveFileDialogService(action);
		}
		void INativeReportDesignerUI.DoWithWizardDialogService(Action<IDialogService> action) {
			DoWithWizardDialogService(action);
		}
		void INativeReportDesignerUI.DestroyDocument(ReportDesignerDocument document) {
			DestroyDocument(document);
		}
		string INativeReportDesignerUI.GetNewDocumentName() {
			return GetNewDocumentName();
		}
		void INativeReportDesignerUI.ActivatePropertyGrid() {
			ActivatePropertyGrid();
		}
		void INativeReportDesignerUI.OpenSubreport(XRSubreportDiagramItem subreportDiagramItem) {
			OpenSubreport(subreportDiagramItem);
		}
		#endregion
		protected abstract void ActivatePropertyGrid();
		protected abstract void OpenSubreport(XRSubreportDiagramItem subreportDiagramItem);
	}
}
