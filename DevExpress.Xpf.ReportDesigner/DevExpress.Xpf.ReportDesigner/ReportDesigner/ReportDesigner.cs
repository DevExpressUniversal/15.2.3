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
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using DevExpress.Mvvm;
using DevExpress.Mvvm.Native;
using DevExpress.Mvvm.UI.Native;
using DevExpress.Utils;
using DevExpress.Xpf.Bars;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Reports.UserDesigner.Native;
using DevExpress.Xpf.Reports.UserDesigner.ReportModel;
using DevExpress.Xpf.Reports.UserDesigner.XRDiagram;
using DevExpress.XtraReports.Native;
using DevExpress.XtraReports.UI;
namespace DevExpress.Xpf.Reports.UserDesigner {
	[ContentProperty("View")]
	[TemplatePart(Name = PART_ViewContainer, Type = typeof(Decorator))]
	[DXToolboxBrowsable]
	public class ReportDesigner : ReportDesignerBase {
		public const string PART_ViewContainer = "ViewContainer";
		public static readonly DependencyProperty ViewProperty;
		public static readonly DependencyProperty DocumentSourceProperty;
		static readonly DependencyPropertyKey ActiveDocumentViewSourcePropertyKey;
		public static readonly DependencyProperty ActiveDocumentViewSourceProperty;
		static readonly DependencyPropertyKey ActiveDocumentPropertyKey;
		public static readonly DependencyProperty ActiveDocumentProperty;
		public static readonly DependencyProperty DesignerProperty;
		public static readonly DependencyProperty DocumentProperty;
		public static readonly DependencyProperty DocumentSelectorProperty;
		public static readonly DependencyProperty RibbonTemplateProperty;
		public static readonly DependencyProperty RibbonStatusBarTemplateProperty;
		public static readonly DependencyProperty PreviewTemplateProperty;
		public static readonly DependencyProperty DesignerPaneTemplateProperty;
		public static readonly DependencyProperty DocumentTemplateProperty;
		public static readonly DependencyProperty ScriptEditorTemplateProperty;
		public static readonly DependencyProperty PropertyGridIsActiveProperty;
		static readonly DependencyPropertyKey RibbonPropertyKey;
		public static readonly DependencyProperty RibbonProperty;
		static ReportDesigner() {
			DependencyPropertyRegistrator<ReportDesigner>.New()
				.Register(d => d.View, out ViewProperty, null, (d, e) => d.OnViewChanged(e), (d, value) => d.CoerceView(value))
				.Register(d => d.DocumentSource, out DocumentSourceProperty, null, d => d.OnDocumentSourceChanged(), FrameworkPropertyMetadataOptions.BindsTwoWayByDefault)
				.RegisterReadOnly(d => d.ActiveDocumentViewSource, out ActiveDocumentViewSourcePropertyKey, out ActiveDocumentViewSourceProperty, null)
				.RegisterReadOnly(d => d.ActiveDocument, out ActiveDocumentPropertyKey, out ActiveDocumentProperty, null, (d, e) => d.OnActiveDocumentChanged(e))
				.RegisterAttached((DependencyObject d) => GetDesigner(d), out DesignerProperty, null, FrameworkPropertyMetadataOptions.Inherits)
				.RegisterAttached((DependencyObject d) => GetDocument(d), out DocumentProperty, null, FrameworkPropertyMetadataOptions.Inherits)
				.RegisterAttached((DependencyObject d) => GetDocumentSelector(d), out DocumentSelectorProperty, null, FrameworkPropertyMetadataOptions.Inherits)
				.Register(d => d.RibbonTemplate, out RibbonTemplateProperty, null)
				.Register(d => d.RibbonStatusBarTemplate, out RibbonStatusBarTemplateProperty, null)
				.Register(d => d.PreviewTemplate, out PreviewTemplateProperty, null)
				.Register(d => d.DesignerPaneTemplate, out DesignerPaneTemplateProperty, null)
				.Register(d => d.ScriptEditorTemplate, out ScriptEditorTemplateProperty, null)
				.Register(d => d.DocumentTemplate, out DocumentTemplateProperty, null)
				.Register(d => d.PropertyGridIsActive, out PropertyGridIsActiveProperty, false, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault)
				.RegisterReadOnly(d => d.Ribbon, out RibbonPropertyKey, out RibbonProperty, null)
				.OverrideDefaultStyleKey()
			;
		}
		readonly ReportDesignerCommands commands;
		public ReportDesigner() {
			InternalDocuments = new HierarchyCollection<ReportDesignerDocument, ReportDesigner>(this,
				(document, control) => control.AttachDocument(document),
				(document, control) => control.DetachDocument(document)
			);
			onClosingCommand = new DelegateCommand<CancelEventArgs>(OnClosing, false);
			commands = new ReportDesignerCommands(this);
		}
		readonly DelegateCommand<CancelEventArgs> onClosingCommand;
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public ICommand<CancelEventArgs> OnClosingCommand { get { return onClosingCommand; } }
		void OnClosing(CancelEventArgs e) {
			foreach(IReportDesignerDocument document in InternalDocuments) {
				document.OnClosing(e);
				if(e.Cancel) break;
			}
		}
		public ReportDesignerCommands Commands { get { return commands; } }
		public ReportDesignerViewBase View {
			get { return (ReportDesignerViewBase)GetValue(ViewProperty); }
			set { SetValue(ViewProperty, value); }
		}
		ReportDesignerViewBase CoerceView(ReportDesignerViewBase value) {
			DebugHelper2.Assert(DesignerProperties.GetIsInDesignMode(this) || View == null, "ReportDesigner.CoerceView");
			return View != null ? View : (value ?? CreateDefaultView());
		}
		protected virtual ReportDesignerViewBase CreateDefaultView() { return new ReportDesignerBrowserView(); }
		void OnViewChanged(DependencyPropertyChangedEventArgs e) {
			var newValue = (ReportDesignerViewBase)e.NewValue;
			DebugHelper2.Assert(DesignerProperties.GetIsInDesignMode(this) || e.OldValue == null, "ReportDesigner.OnViewChanged");
			if(newValue != null && !DesignerProperties.GetIsInDesignMode(this)) {
				IReportDesignerView newView = newValue;
				newValue.ActiveDocumentViewSourceChanged += (s, _) => ActiveDocumentViewSource = View.ActiveDocumentViewSource;
				newValue.ActiveDocumentChanged += (s, _) => ActiveDocument = View.ActiveDocument;
				newView.AttachReportDesigner(this);
			}
			if(this.viewContainer != null)
				this.viewContainer.Child = newValue;
		}
		protected override IEnumerator LogicalChildren {
			get {
				return new MergedEnumerator(base.LogicalChildren, new SingleLogicalChildEnumerator(viewContainer));
			}
		}
		Decorator viewContainer;
		public override void OnApplyTemplate() {
			base.OnApplyTemplate();
			if(this.viewContainer != null) {
				this.viewContainer.Child = null;
				RemoveLogicalChild(this.viewContainer);
			}
			this.viewContainer = (Decorator)GetTemplateChild(PART_ViewContainer);
			if(this.viewContainer != null) {
				AddLogicalChild(this.viewContainer);
				if(!EnsureView())
					this.viewContainer.Child = View;
			}
		}
		bool EnsureView() {
			if(View != null) return false;
			View = CreateDefaultView();
			return true;
		}
		public void ShowWindow(FrameworkElement owner) {
			if(!IsInitialized) {
				BeginInit();
				EndInit();
			}
			EnsureView();
			IReportDesignerView view = View;
			view.ShowWindow(owner);
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public static ReportDesigner GetDesigner(DependencyObject d) { return (ReportDesigner)d.GetValue(DesignerProperty); }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public static void SetDesigner(DependencyObject d, ReportDesigner v) { d.SetValue(DesignerProperty, v); }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public static object GetDocument(DependencyObject d) { return d.GetValue(DocumentProperty); }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public static void SetDocument(DependencyObject d, object v) { d.SetValue(DocumentProperty, v); }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public static object GetDocumentSelector(DependencyObject d) { return d.GetValue(DocumentSelectorProperty); }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public static void SetDocumentSelector(DependencyObject d, object v) { d.SetValue(DocumentSelectorProperty, v); }
		public DataTemplate RibbonTemplate {
			get { return (DataTemplate)GetValue(RibbonTemplateProperty); }
			set { SetValue(RibbonTemplateProperty, value); }
		}
		public DataTemplate RibbonStatusBarTemplate {
			get { return (DataTemplate)GetValue(RibbonStatusBarTemplateProperty); }
			set { SetValue(RibbonStatusBarTemplateProperty, value); }
		}
		public DataTemplate DesignerPaneTemplate {
			get { return (DataTemplate)GetValue(DesignerPaneTemplateProperty); }
			set { SetValue(DesignerPaneTemplateProperty, value); }
		}
		public DataTemplate PreviewTemplate {
			get { return (DataTemplate)GetValue(PreviewTemplateProperty); }
			set { SetValue(PreviewTemplateProperty, value); }
		}
		public DataTemplate DocumentTemplate {
			get { return (DataTemplate)GetValue(DocumentTemplateProperty); }
			set { SetValue(DocumentTemplateProperty, value); }
		}
		public DataTemplate ScriptEditorTemplate {
			get { return (DataTemplate)GetValue(ScriptEditorTemplateProperty); }
			set { SetValue(ScriptEditorTemplateProperty, value); }
		}
		protected readonly HierarchyCollection<ReportDesignerDocument, ReportDesigner> InternalDocuments;
		List<ReportDesignerDocument> documents = new List<ReportDesignerDocument>();
		public ReadOnlyCollection<ReportDesignerDocument> Documents { get { return documents.AsReadOnly(); } }
		public object DocumentSource {
			get { return GetValue(DocumentSourceProperty); }
			set { SetValue(DocumentSourceProperty, value); }
		}
		public ReportDesignerDocumentViewSource ActiveDocumentViewSource {
			get { return (ReportDesignerDocumentViewSource)GetValue(ActiveDocumentViewSourceProperty); }
			private set { SetValue(ActiveDocumentViewSourcePropertyKey, value); }
		}
		public ReportDesignerDocument ActiveDocument {
			get { return (ReportDesignerDocument)GetValue(ActiveDocumentProperty); }
			private set { SetValue(ActiveDocumentPropertyKey, value); }
		}
		public ReportDesignerDocument OpenDocument(Action<ReportDesignerDocument> beforeLoad = null) {
			return OpenDocument(x => x.Load(), false, beforeLoad);
		}
		public void Exit() {
			CancelEventArgs args = new CancelEventArgs();
			OnClosing(args);
			if(args.Cancel) return;
			foreach(var document in InternalDocuments)
				document.Close(true);
			Window.GetWindow(this).Do(x => x.Close());
		}
		public ReportDesignerDocument NewDocument(Func<XtraReport> newReportFactory = null, Action<ReportDesignerDocument> beforeLoad = null) {
			return OpenDocument(x => x.New(newReportFactory), false, beforeLoad);
		}
		public ReportDesignerDocument NewDocument(XtraReport newReportTemplate, Action<ReportDesignerDocument> beforeLoad = null) {
			return OpenDocument(x => x.New(newReportTemplate), false, beforeLoad);
		}
		public ReportDesignerDocument OpenDocument(byte[] originalReport, Action<ReportDesignerDocument> beforeLoad = null) {
			Guard.ArgumentNotNull(originalReport, "originalReport");
			return OpenDocument(x => x.Load(originalReport), false, beforeLoad);
		}
		public ReportDesignerDocument OpenDocument(Stream originalReport, Action<ReportDesignerDocument> beforeLoad = null) {
			Guard.ArgumentNotNull(originalReport, "originalReport");
			return OpenDocument(x => x.Load(originalReport), false, beforeLoad);
		}
		public ReportDesignerDocument OpenDocument(XtraReport originalReport, Action<ReportDesignerDocument> beforeLoad = null) {
			Guard.ArgumentNotNull(originalReport, "originalReport");
			return OpenDocument(x => x.Load(originalReport), false, beforeLoad);
		}
		public ReportDesignerDocument OpenDocument(string reportFilePath, Action<ReportDesignerDocument> beforeLoad = null) {
			Guard.ArgumentNotNull(reportFilePath, "reportFilePath");
			return OpenDocument(x => x.Load(reportFilePath), false, beforeLoad);
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public ReportDesignerDocument OpenDocumentFrom(object source, Action<ReportDesignerDocument> beforeLoad = null) {
			Guard.ArgumentNotNull(source, "source");
			return OpenDocument(x => x.LoadFrom(source), false, beforeLoad);
		}
		ReportDesignerDocument OpenDocument(Func<ReportDesignerDocument, bool> loadAction, bool asSingleDocument, Action<ReportDesignerDocument> beforeLoad) {
			if(asSingleDocument && singleDocumentViewSource != null)
				return OpenDocumentAsSingle(loadAction, () => singleDocumentViewSource.Document, x => singleDocumentViewSource.SetSource(x));
			var document = CreateDocument();
			if(beforeLoad != null)
				beforeLoad(document);
			bool loaded = false;
			DoOperationWithDetachedDocument(document, x => loaded = loadAction(x));
			if(!loaded) return null;
			EnsureView();
			IReportDesignerView view = View;
			var viewSource = view.OpenDocument(document, RaiseDocumentOpened);
			if(asSingleDocument) {
				singleDocumentViewSource = viewSource;
				singleDocumentViewSource.DocumentSourceChanged += OnSingleDocumentViewSourceDocumentSourceChanged;
			}
			InternalDocuments.Add(document);
			return document;
		}
		protected override void OnDocumentLoadFailed(object sender, ReportDesignerDocumentLoadFailedEventArgs e) {
			if(e.Document.Tag is SubreportDocumentTag)
				e.ShowOpenFileDialogToSelectNewSource = false;
			base.OnDocumentLoadFailed(sender, e);
		}
		protected override void OpenSubreport(XRSubreportDiagramItem subreportDiagramItem) {
			var subreport = (XRSubreport)XRModelBase.GetXRModel(subreportDiagramItem).XRObject;
			var subreportDocument = Documents.FirstOrDefault(doc => (doc.Tag as SubreportDocumentTag).With(x => x.Tag) == subreport);
			if(subreportDocument != null) {
				var documentTag = (SubreportDocumentTag)subreportDocument.Tag;
				if(IsSubreportDocument(documentTag.Source, subreport)) {
					EnsureView();
					IReportDesignerView view = View;
					view.ActivateDocument(subreportDocument);
					return;
				}
			}
			Action<ReportDesignerDocument> initDocumentAction = x => x.Tag = new SubreportDocumentTag(subreport, (object)subreport.ReportSource ?? (object)subreport.ReportSourceUrl);
			var mainReportModel = ActiveDocument.ReportModel;
			if(subreport.ReportSource != null)
				subreportDocument = OpenDocument(subreport.ReportSource, initDocumentAction);
			else if(string.IsNullOrEmpty(subreport.ReportSourceUrl))
				subreportDocument = NewDocument(ReportStorage.CreateNewSubreport, initDocumentAction);
			else
				subreportDocument = OpenDocument(subreport.ReportSourceUrl, initDocumentAction);
			if(subreportDocument == null) return;
			bool fromSavedEvent = false;
			EventHandler<ReportDesignerDocumentEventArgs> saved = (s, e) => {
				fromSavedEvent = true;
				var selectionModel = XRDiagramItemBase.GetPropertiesModel(subreportDiagramItem);
				var result = ((ReportDesignerDocument)s).Source;
				var resultReport = result as XtraReport;
				if(resultReport != null) {
					resultReport.SourceUrl = string.Empty;
					SetDocumentTagSource(subreportDocument, subreport, resultReport);
					selectionModel.SetPropertyValueEx((XRSubreport x) => x.ReportSource, resultReport);
					selectionModel.SetPropertyValueEx((XRSubreport x) => x.ReportSourceUrl, string.Empty);
				} else {
					SetDocumentTagSource(subreportDocument, subreport, result.ToString());
					selectionModel.SetPropertyValueEx((XRSubreport x) => x.ReportSourceUrl, result.ToString());
					selectionModel.SetPropertyValueEx((XRSubreport x) => x.ReportSource, null);
				}
			};
			subreportDocument.Saved += saved;
			IObjectTracker tracker;
			Tracker.GetTracker(subreport, out tracker);
			tracker.ObjectPropertyChanged += (s, e) => {
				if(e.PropertyName == ExpressionHelper.GetPropertyName<XRSubreport, string>(x => x.ReportSourceUrl) && !fromSavedEvent) {
					subreportDocument.Saved -= saved;
					subreportDocument.Tag = null;
				}
				fromSavedEvent = false;
			};
		}
		static bool IsSubreportDocument(object documentSource, XRSubreport subreport) {
			if(subreport.ReportSource != null)
				return Equals(documentSource, subreport.ReportSource);
			return Equals(documentSource, subreport.ReportSourceUrl);
		}
		void SetDocumentTagSource(ReportDesignerDocument document, object tag, object source) {
			document.Tag = new SubreportDocumentTag(tag, source);
		}
		void CopyReportLayout(XtraReport source, XtraReport dest) {
			using(Stream stream = new MemoryStream()) {
				SaveReportLayout(source, stream);
				dest.LoadLayout(stream);
			}
		}
		void SaveReportLayout(XtraReport report, Stream stream) {
			ISite site = report.Site;
			try {
				report.Site = null;
				report.SaveLayout(stream);
			} finally {
				report.Site = site;
			}
		}
		public event DependencyPropertyChangedEventHandler ActiveDocumentChanged;
		protected override void DestroyDocument(ReportDesignerDocument document) {
			EnsureView();
			IReportDesignerView view = View;
			view.DestroyDocument(document);
		}
		ReportDesignerDocumentViewSource singleDocumentViewSource;
		void OnSingleDocumentViewSourceDocumentSourceChanged(object sender, EventArgs e) {
			UpdateDocumentSource();
		}
		Locker sourceLocker = new Locker();
		void OnDocumentSourceChanged() {
			sourceLocker.DoIfNotLocked(() => {
				if(DocumentSource == null) {
					if(singleDocumentViewSource != null)
						singleDocumentViewSource.Document.Close(true);
					return;
				}
				bool loaded;
				if(singleDocumentViewSource != null)
					loaded = singleDocumentViewSource.Document.LoadFrom(DocumentSource);
				else
					loaded = OpenDocument(x => x.LoadFrom(DocumentSource), true, null) != null;
				if(!loaded)
					UpdateDocumentSource();
			});
		}
		void UpdateDocumentSource() {
			sourceLocker.DoLockedAction(() => SetCurrentValue(DocumentSourceProperty, singleDocumentViewSource.With(x => x.Document.Source)));
		}
		void OnDocumentsSourceChanged(DependencyPropertyChangedEventArgs e) {
			throw new NotImplementedException();
		}
		void OnActiveDocumentSourceChanged() {
			throw new NotImplementedException();
		}
		void OnActiveDocumentChanged(DependencyPropertyChangedEventArgs e) {
			if(ActiveDocumentChanged != null)
				ActiveDocumentChanged(this, e);
		}
		protected override void OnDocumentOpened(object sender, ReportDesignerDocumentEventArgs e) {
			documents.Add(e.Document);
			base.OnDocumentOpened(sender, e);
		}
		protected override void OnDocumentClosed(object sender, ReportDesignerDocumentEventArgs e) {
			documents.Remove(e.Document);
			InternalDocuments.Remove(e.Document);
			if(singleDocumentViewSource != null && singleDocumentViewSource.Document == e.Document)
				singleDocumentViewSource = null;
			base.OnDocumentClosed(sender, e);
		}
		protected override IEnumerable<ReportDesignerDocument> GetExistingDocuments() {
			return Documents;
		}
		public bool PropertyGridIsActive {
			get { return (bool)GetValue(PropertyGridIsActiveProperty); }
			set { SetValue(PropertyGridIsActiveProperty, value); }
		}
		public object Ribbon {
			get { return GetValue(RibbonProperty); }
			internal set { SetValue(RibbonPropertyKey, value); }
		}
		protected override void ActivatePropertyGrid() {
			SetCurrentValue(PropertyGridIsActiveProperty, true);
		}
	}
}
