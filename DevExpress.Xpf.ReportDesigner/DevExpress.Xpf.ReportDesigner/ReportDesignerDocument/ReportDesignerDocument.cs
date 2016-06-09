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
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using DevExpress.Mvvm;
using DevExpress.Mvvm.Native;
using DevExpress.Mvvm.UI.Native;
using DevExpress.Utils;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Reports.UserDesigner.Native;
using DevExpress.Xpf.Reports.UserDesigner.Native.ReportExtensions;
using DevExpress.Xpf.Reports.UserDesigner.ReportModel;
using DevExpress.Xpf.Reports.UserDesigner.XRDiagram;
using DevExpress.XtraReports;
using DevExpress.XtraReports.UI;
namespace DevExpress.Xpf.Reports.UserDesigner {
	public class ReportDesignerDocument : DependencyObject, IReportDesignerDocument {
		static readonly DependencyPropertyKey SourcePropertyKey;
		public static readonly DependencyProperty SourceProperty;
		static readonly DependencyPropertyKey SavedReportPropertyKey;
		public static readonly DependencyProperty SavedReportProperty;
		static readonly DependencyPropertyKey TitlePropertyKey;
		public static readonly DependencyProperty TitleProperty;
		static readonly DependencyPropertyKey PreviewReportPropertyKey;
		public static readonly DependencyProperty PreviewReportProperty;
		static readonly DependencyPropertyKey PreviewPropertyKey;
		public static readonly DependencyProperty PreviewProperty;
		static readonly DependencyPropertyKey ReportPropertyKey;
		public static readonly DependencyProperty ReportProperty;
		[IgnoreDependencyPropertiesConsistencyChecker]
		public static readonly DependencyProperty HasChangesProperty;
		static readonly DependencyPropertyKey CanSavePropertyKey;
		public static readonly DependencyProperty CanSaveProperty;
		public static readonly DependencyProperty ViewKindProperty;
		public static readonly DependencyProperty TagProperty;
		static readonly DependencyPropertyKey ReportModelPropertyKey;
		public static readonly DependencyProperty ReportModelProperty;
		static readonly DependencyPropertyKey DiagramPropertyKey;
		public static readonly DependencyProperty DiagramProperty;
		static ReportDesignerDocument() {
			DependencyPropertyRegistrator<ReportDesignerDocument>.New()
				.RegisterReadOnly(d => d.Source, out SourcePropertyKey, out SourceProperty, null, (d, e) => d.OnSourceChanged())
				.RegisterReadOnly(d => d.SavedReport, out SavedReportPropertyKey, out SavedReportProperty, null)
				.RegisterReadOnly(d => d.Title, out TitlePropertyKey, out TitleProperty, null, (d, e) => d.OnTitleChanged())
				.RegisterReadOnly(d => d.PreviewReport, out PreviewReportPropertyKey, out PreviewReportProperty, null)
				.RegisterReadOnly(d => d.Preview, out PreviewPropertyKey, out PreviewProperty, null)
				.RegisterReadOnly(d => d.Report, out ReportPropertyKey, out ReportProperty, null, (d, e) => d.OnReportChanged(e))
				.Register(d => d.HasChanges, out HasChangesProperty, false, (d, e) => d.OnHasChangesChanged())
				.RegisterReadOnly(d => d.CanSave, out CanSavePropertyKey, out CanSaveProperty, true, (d, e) => d.OnCanSaveChanged())
				.Register(d => d.ViewKind, out ViewKindProperty, ReportDesignerDocumentViewKind.Designer, (d, e) => d.OnViewKindChanged(e))
				.Register(d => d.Tag, out TagProperty, null)
				.RegisterReadOnly(d => d.ReportModel, out ReportModelPropertyKey, out ReportModelProperty, null, (d, e) => d.Diagram = d.ReportModel.With(x => x.DiagramItem.Diagram))
				.RegisterReadOnly(d => d.Diagram, out DiagramPropertyKey, out DiagramProperty, null, (d, e) => d.OnDiagramChanged(e))
				.OverrideDefaultStyleKey()
			;
		}
		void OnViewKindChanged(DependencyPropertyChangedEventArgs e) {
			if((ReportDesignerDocumentViewKind)e.OldValue == ReportDesignerDocumentViewKind.Preview && PreviewReport != null)
				PreviewReport.StopPageBuilding();
			if(ViewKind == ReportDesignerDocumentViewKind.Preview)
				UpdatePreviewReport();
		}
		protected internal ReportDesignerDocument(INativeReportDesignerUI reportDesignerUI) {
			Guard.ArgumentNotNull(reportDesignerUI, "reportDesignerUI");
			ReportDesignerUI = reportDesignerUI;
			commands = new ReportDesignerDocumentCommands(this);
			BindingOperations.SetBinding(this, HasChangesProperty, new Binding() { Path = new PropertyPath("(0).(1)", DiagramProperty, XRDiagramControl.HasChangesProperty), Source = this, Mode = BindingMode.OneWay });
		}
		protected readonly INativeReportDesignerUI ReportDesignerUI;
		readonly ReportDesignerDocumentCommands commands;
		public ReportDesignerDocumentCommands Commands { get { return commands; } }
		public object Source {
			get { return GetValue(SourceProperty); }
			private set { SetValue(SourcePropertyKey, value); }
		}
		public XtraReport SavedReport {
			get { return (XtraReport)GetValue(SavedReportProperty); }
			private set { SetValue(SavedReportPropertyKey, value); }
		}
		public event EventHandler SourceChanged;
		void OnSourceChanged() {
			UpdateCanSave();
			if(SourceChanged != null)
				SourceChanged(this, EventArgs.Empty);
		}
		public bool New(XtraReport newReportTemplate) {
			return New(newReportTemplate == null ? (Func<XtraReport>)null : () => newReportTemplate.Clone());
		}
		public bool New(Func<XtraReport> newReportFactory = null) {
			if(newReportFactory == null && !ReportDesignerUI.ReportStorage.CanCreateNew()) return false;
			var report = newReportFactory == null ? ReportDesignerUI.ReportStorage.CreateNew() : newReportFactory();
			if(string.IsNullOrEmpty(report.Name))
				report.Name = ReportDesignerUI.GetNewDocumentName();
			return LoadFromSource(report, true);
		}
		public bool Load() {
			return LoadFromSource(null, false);
		}
		public bool Load(byte[] originalReport) {
			Guard.ArgumentNotNull(originalReport, "originalReport");
			return LoadFromSource(originalReport, false);
		}
		public bool Load(Stream originalReport) {
			Guard.ArgumentNotNull(originalReport, "originalReport");
			return LoadFromSource(originalReport, false);
		}
		public bool Load(XtraReport originalReport) {
			Guard.ArgumentNotNull(originalReport, "originalReport");
			return LoadFromSource(originalReport, false);
		}
		public bool Load(string reportFilePath) {
			Guard.ArgumentNotNull(reportFilePath, "reportFilePath");
			return LoadFromSource(reportFilePath, false);
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public bool LoadFrom(object source) {
			Guard.ArgumentNotNull(source, "source");
			return LoadFromSource(source, false);
		}
		public event EventHandler<ReportDesignerDocumentLoadFailedEventArgs> LoadFailed;
		public event EventHandler<ReportDesignerDocumentSubreportLoadFailedEventArgs> SubreportLoadFailed;
		public event EventHandler<ReportDesignerDocumentEventArgs> Loaded;
		public bool Save() {
			return SaveToStorage(Nowhere);
		}
		public bool SaveAs() {
			return SaveToStorage(null);
		}
		public bool Save(Action<byte[]> reportStorage) {
			Guard.ArgumentNotNull(reportStorage, "reportStorage");
			return SaveToStorage(reportStorage);
		}
		public bool Save(Stream reportStorage) {
			Guard.ArgumentNotNull(reportStorage, "reportStorage");
			return SaveToStorage(reportStorage);
		}
		public bool Save(Action<XtraReport> reportStorage) {
			Guard.ArgumentNotNull(reportStorage, "reportStorage");
			return SaveToStorage(reportStorage);
		}
		public bool Save(string reportFilePath) {
			Guard.ArgumentNotNull(reportFilePath, "reportFilePath");
			return SaveToStorage(reportFilePath);
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public bool SaveTo(object storage) {
			Guard.ArgumentNotNull(storage, "storage");
			return SaveToStorage(storage);
		}
		public event EventHandler<ReportDesignerDocumentSaveFailedEventArgs> SaveFailed;
		public event EventHandler<ReportDesignerDocumentEventArgs> Saved;
		public bool Close(bool force = false) {
			if(!force) {
				ReportDesignerDocumentClosingEventArgs e = new ReportDesignerDocumentClosingEventArgs(this);
				OnClosing(e);
				if(e.Cancel) return false;
			}
			ReportDesignerUI.DestroyDocument(this);
			return true;
		}
		protected virtual void OnClosing(ReportDesignerDocumentClosingEventArgs e) {
			if(Closing != null)
				Closing(this, e);
			if(e.Cancel || !e.ShowConfirmationMessageIfDocumentContainsUnsavedChanges || !HasChanges) return;
			MessageResult messageResult = MessageResult.None;
			ReportDesignerUI.DoWithMessageBoxService(message => {
				messageResult = message.ShowMessage("Do you want to save changes?", Title, MessageButton.YesNoCancel, MessageIcon.Question);
			});
			e.Cancel = messageResult == MessageResult.Cancel || messageResult == MessageResult.Yes && !Save();
		}
		protected virtual void OnClosed() {
			ReportDesignerDocumentEventArgs args = new ReportDesignerDocumentEventArgs(this);
			if(Closed != null)
				Closed(this, args);
		}
		public event EventHandler<ReportDesignerDocumentClosingEventArgs> Closing;
		public event EventHandler<ReportDesignerDocumentEventArgs> Closed;
		public bool HasChanges { get { return (bool)GetValue(HasChangesProperty); } }
		public bool CanSave {
			get { return (bool)GetValue(CanSaveProperty); }
			private set { SetValue(CanSavePropertyKey, value); }
		}
		public object Tag {
			get { return GetValue(TagProperty); }
			set { SetValue(TagProperty, value); }
		}
		void UpdateCanSave() {
			CanSave = Source == null || HasChanges;
		}
		void OnCanSaveChanged() {
			if(CanSaveChanged != null)
				CanSaveChanged(this, EventArgs.Empty);
		}
		public event EventHandler CanSaveChanged;
		void OnHasChangesChanged() {
			UpdateCanSave();
		}
		public ReportDesignerDocumentViewKind ViewKind {
			get { return (ReportDesignerDocumentViewKind)GetValue(ViewKindProperty); }
			set { SetValue(ViewKindProperty, value); }
		}
		public string Title {
			get { return (string)GetValue(TitleProperty); }
			private set { SetValue(TitlePropertyKey, value); }
		}
		public event EventHandler TitleChanged;
		void OnTitleChanged() {
			if(TitleChanged != null)
				TitleChanged(this, EventArgs.Empty);
		}
		IObjectTracker unsavedReportTracker;
		public XtraReport Report {
			get { return (XtraReport)GetValue(ReportProperty); }
			private set { SetValue(ReportPropertyKey, value); }
		}
		protected void EnsureReport() {
			if(Report == null) {
				DebugHelper2.Assert(Source == null, "ReportDesignerDocument.EnsureXRObject");
				New();
			}
		}
		void OnReportChanged(DependencyPropertyChangedEventArgs e) {
			var newValue = (XtraReport)e.NewValue;
			if(unsavedReportTracker != null)
				unsavedReportTracker.ObjectPropertyChanged -= OnReportPropertyChanged;
			Tracker.GetTracker(newValue, out unsavedReportTracker);
			if(unsavedReportTracker != null)
				unsavedReportTracker.ObjectPropertyChanged += OnReportPropertyChanged;
			ReportModel = Report.With(x => CreateModel(new ReportModelOwner(this), x));
			OnReportPropertyChanged(newValue, new PropertyChangedEventArgs(null));
		}
		public XtraReportModelBase ReportModel {
			get { return (XtraReportModelBase)GetValue(ReportModelProperty); }
			private set { SetValue(ReportModelPropertyKey, value); }
		}
		XtraReportModelBase CreateModel(IReportModelOwner owner, XtraReport report) {
			var model = CreateModelOverride(owner, report);
			model.DiagramItem.Diagram.SetBinding(XRDiagramControl.StyleProperty, new Binding(ExpressionHelper.GetPropertyName((INativeReportDesignerUI x) => x.DiagramStyle)) { Source = ReportDesignerUI, Mode = BindingMode.OneWay });
			return model;
		}
		protected virtual XtraReportModelBase CreateModelOverride(IReportModelOwner owner, XtraReport report) {
			return new XtraReportModel(owner, report);
		}
		public XRDiagramControl Diagram {
			get { return (XRDiagramControl)GetValue(DiagramProperty); }
			private set { SetValue(DiagramPropertyKey, value); }
		}
		void OnDiagramChanged(DependencyPropertyChangedEventArgs e) {
			var oldValue = (XRDiagramControl)e.OldValue;
			var newValue = (XRDiagramControl)e.NewValue;
			if(oldValue != null)
				ReportDesignerDocumentView.SetDocument(oldValue, null);
			if(newValue != null)
				ReportDesignerDocumentView.SetDocument(newValue, this);
		}
		void OnReportPropertyChanged(object sender, PropertyChangedEventArgs e) {
			if(e.PropertyName == null || e.PropertyName == BindableBase.GetPropertyName(() => new XtraReport().Name) || e.PropertyName == BindableBase.GetPropertyName(() => new XtraReport().DisplayName))
				Title = Report == null ? null : GetTitle(string.IsNullOrEmpty(Report.DisplayName) ? Report.Name : Report.DisplayName);
		}
		protected virtual string GetTitle(string reportDisplayName) { return reportDisplayName; }
		bool SaveToStorage(object storage) {
			EnsureReport();
			object newSource;
			XtraReport newReport;
			if(!(ReferenceEquals(storage, Nowhere) ? SaveWith(out newReport, out newSource) : SaveTo(storage, out newReport, out newSource))) return false;
			if(newSource == null)
				throw new InvalidOperationException(); 
			SavedReport = newReport;
			Source = newSource;
			if(Saved != null)
				Saved(this, new ReportDesignerDocumentEventArgs(this));
			Diagram.Do(x => x.SetHasChanges(false));
			return true;
		}
		bool LoadFromSource(object source, bool dropSource) {
			XtraReport report;
			object newSource;
			if(!LoadFrom(source, out report, out newSource)) return false;
			if(report == null)
				throw new InvalidOperationException(); 
			SavedReport = report;
			Source = dropSource ? null : newSource;
			Report = report.Clone();
			if(Loaded != null)
				Loaded(this, new ReportDesignerDocumentEventArgs(this));
			return true;
		}
		protected virtual void OnSaveFailed(ReportDesignerDocumentSaveFailedEventArgs e) {
			if(SaveFailed != null)
				SaveFailed(this, e);
		}
		protected virtual void OnLoadFailed(ReportDesignerDocumentLoadFailedEventArgs e) {
			if(LoadFailed != null)
				LoadFailed(this, e);
		}
		protected virtual bool LoadFrom(object source, out XtraReport report, out object newSource) {
			string fileSource;
			if(IsReportID(source, out fileSource))
				return DoLoad(fileSource, out report, out newSource, x => LoadFromFile(x));
			byte[] binarySource = source as byte[];
			if(binarySource != null)
				return DoLoad(binarySource, out report, out newSource, x => ReportDesignerUI.ReportSerializer.Load(x));
			Stream streamSource = source as Stream;
			if(streamSource != null)
				return DoLoad(streamSource, out report, out newSource, x => ReportDesignerUI.ReportSerializer.Load(x));
			XtraReport reportSource = source as XtraReport;
			if(reportSource != null)
				return DoLoad(reportSource, out report, out newSource, x => x);
			newSource = null;
			report = null;
			return true;
		}
		protected static readonly object Nowhere = new object();
		protected virtual bool SaveTo(object storage, out XtraReport newReport, out object newSource) {
			string reportID;
			if(IsReportID(storage, out reportID))
				return DoSave(reportID, out newReport, out newSource, x => SaveToFile(x, x == null));
			Action<byte[]> binaryStorage = storage as Action<byte[]>;
			if(binaryStorage != null)
				return DoSave(binaryStorage, out newReport, out newSource, SaveAsBinary);
			Stream streamStorage = storage as Stream;
			if(streamStorage != null)
				return DoSave(streamStorage, out newReport, out newSource, SaveToStreamAsBinary);
			Action<XtraReport> reportStorage = storage as Action<XtraReport>;
			if(reportStorage != null)
				return DoSave(reportStorage, out newReport, out newSource, SaveAsReport);
			newSource = null;
			newReport = null;
			return true;
		}
		protected virtual bool SaveWith(out XtraReport newReport, out object newSource) {
			string reportID;
			if(IsReportID(Source, out reportID))
				return DoSave(reportID, out newReport, out newSource, x => SaveToFile(x, false));
			if(Source is byte[])
				return DoSave(out newReport, out newSource, SaveToNothereAsBinary);
			if(Source is Stream)
				return DoSave(out newReport, out newSource, SaveToNothereAsBinary);
			if(Source is XtraReport)
				return DoSave(out newReport, out newSource, SaveToNothereAsReport);
			newSource = null;
			newReport = null;
			return true;
		}
		static bool IsReportID(object source, out string reportID) {
			reportID = source as string;
			return source == null || reportID != null;
		}
		bool DoSave(out XtraReport newReport, out object newSource, Func<Tuple<XtraReport, object>> saveAndReturnNewSourceAction) {
			return DoSave(new object(), out newReport, out newSource, _ => saveAndReturnNewSourceAction());
		}
		bool DoSave<TStorage>(TStorage storage, out XtraReport newReport, out object newSource, Func<TStorage, Tuple<XtraReport, object>> saveAndReturnNewSourceAction) {
			try {
				var x = saveAndReturnNewSourceAction(storage);
				if(x != null) {
					newReport = x.Item1;
					newSource = x.Item2;
					return true;
				}
			} catch(Exception e) {
				ReportDesignerDocumentSaveFailedEventArgs args = new ReportDesignerDocumentSaveFailedEventArgs(this, e, storage);
				string _;
				if(IsReportID(storage, out _)) {
					args.Rethrow = false;
					args.ErrorMessage = ReportDesignerUI.ReportStorage.GetErrorMessage(e);
					args.ShowErrorMessage = true;
					args.ShowSaveFileDialogToSelectNewStorage = true;
				}
				OnSaveFailed(args);
				if(args.Rethrow)
					throw;
				if(args.ShowErrorMessage)
					ShowErrorMessage(args.ErrorMessage);
				if(args.ShowSaveFileDialogToSelectNewStorage)
					return DoSave((string)null, out newReport, out newSource, x => SaveToFile(x, true));
			}
			newSource = null;
			newReport = null;
			return false;
		}
		bool DoLoad<TSource>(TSource source, out XtraReport report, out object newSource, Func<TSource, XtraReport> loadAction) {
			return DoLoad(source, out report, out newSource, x => new Tuple<XtraReport, object>(loadAction(x), x));
		}
		bool DoLoad<TSource>(TSource source, out XtraReport report, out object newSource, Func<TSource, Tuple<XtraReport, object>> loadAndReturnNewSourceAction) {
			try {
				var x = loadAndReturnNewSourceAction(source);
				if(x != null) {
					report = x.Item1;
					newSource = x.Item2;
					return true;
				}
			} catch(Exception e) {
				ReportDesignerDocumentLoadFailedEventArgs args = new ReportDesignerDocumentLoadFailedEventArgs(this, e, source);
				string _;
				if(IsReportID(source, out _)) {
					args.Rethrow = false;
					args.ErrorMessage = ReportDesignerUI.ReportStorage.GetErrorMessage(e);
					args.ShowErrorMessage = true;
					args.ShowOpenFileDialogToSelectNewSource = true;
				}
				OnLoadFailed(args);
				if(args.Rethrow)
					throw;
				if(args.ShowErrorMessage)
					ShowErrorMessage(args.ErrorMessage);
				if(args.ShowOpenFileDialogToSelectNewSource)
					return DoLoad((string)null, out report, out newSource, x => LoadFromFile(x));
			}
			report = null;
			newSource = null;
			return false;
		}
		void ShowErrorMessage(string errorText) {
			ReportDesignerUI.DoWithMessageBoxService(x => x.ShowMessage(errorText, "Error", MessageButton.OK, MessageIcon.Error));
		}
		Tuple<XtraReport, object> SaveToNothereAsBinary() {
			var savedReport = Report.Clone();
			var data = ReportDesignerUI.ReportSerializer.Save(savedReport);
			return Tuple.Create(savedReport, (object)data);
		}
		Tuple<XtraReport, object> SaveAsBinary(Action<byte[]> binaryStorage) {
			var savedReport = Report.Clone();
			var data = ReportDesignerUI.ReportSerializer.Save(savedReport);
			binaryStorage(data);
			return Tuple.Create(savedReport, (object)data);
		}
		Tuple<XtraReport, object> SaveToStreamAsBinary(Stream stream) {
			var savedReport = Report.Clone();
			var data = ReportDesignerUI.ReportSerializer.Save(savedReport);
			stream.Write(data, 0, data.Length);
			return Tuple.Create(savedReport, (object)data);
		}
		Tuple<XtraReport, object> SaveToNothereAsReport() {
			var data = Report.Clone();
			return Tuple.Create(data, (object)data);
		}
		Tuple<XtraReport, object> SaveAsReport(Action<XtraReport> reportStorage) {
			var data = Report.Clone();
			reportStorage(data);
			return Tuple.Create(data, (object)data);
		}
		class ReportProvider : IReportProvider {
			readonly ReportDesignerDocument document;
			public ReportProvider(ReportDesignerDocument document) {
				this.document = document;
			}
			public XtraReport SavedReport { get; private set; }
			public XtraReport GetReport() {
				SavedReport = document.Report.Clone();
				return SavedReport;
			}
			public XtraReport GetReport(string newReportTitle) {
				Tracker.Set(document.Report, x => x.DisplayName, newReportTitle);
				return GetReport();
			}
		}
		Tuple<XtraReport, object> SaveToFile(string reportID, bool saveAs) {
			var reportProvider = new ReportProvider(this);
			reportID = ReportDesignerUI.ReportStorage.Save(reportID, reportProvider, saveAs, Title, ReportDesignerUI);
			if(reportID == null) return null;
			if(reportProvider.SavedReport == null)
				throw new InvalidOperationException();
			return Tuple.Create(reportProvider.SavedReport, (object)reportID);
		}
		Tuple<XtraReport, object> LoadFromFile(string filePath) {
			if(filePath == null && ReportDesignerUI.ReportStorage.CanOpen())
				filePath = ReportDesignerUI.ReportStorage.Open(ReportDesignerUI);
			if(filePath == null) return null;
			return Tuple.Create(ReportDesignerUI.ReportStorage.Load(filePath, ReportDesignerUI.ReportSerializer), (object)filePath);
		}
		public IReport PreviewReport {
			get { return (IReport)GetValue(PreviewReportProperty); }
			private set { SetValue(PreviewReportPropertyKey, value); }
		}
		void UpdatePreviewReport() {
			PreviewReport = null;
			System.Windows.Threading.Dispatcher.CurrentDispatcher.BeginInvoke((Action)(() => {
				var preview = Report.Clone();
				ReportDesignerUI.ReportStorage.ResolveSubreports(preview, ReportDesignerUI.ReportSerializer, e => {
					var args = new ReportDesignerDocumentSubreportLoadFailedEventArgs(this, e.Exception, e.ReportSourceUrl);
					args.Rethrow = e.Rethrow;
					args.ShowErrorMessage = e.ShowErrorMessage;
					args.ErrorMessage = e.ErrorMessage;
					if(SubreportLoadFailed != null)
						SubreportLoadFailed(this, args);
					e.Rethrow = args.Rethrow;
					e.ShowErrorMessage = args.ShowErrorMessage;
					e.ErrorMessage = args.ErrorMessage;
					e.MessageBoxService = ReportDesignerUI.DoWithMessageBoxService;
				});
				Printing.Platform.Wpf.Native.BackgroundServiceHelper.ReplaceBackgroundService(preview.PrintingSystem);
				PreviewReport = preview;
				System.Windows.Threading.Dispatcher.CurrentDispatcher.BeginInvoke((Action)(() => {
					preview.CreateDocument(true);
				}), System.Windows.Threading.DispatcherPriority.Background);
			}), System.Windows.Threading.DispatcherPriority.Background);
		}
		#region IReportDesignerDocument
		public object Preview {
			get { return GetValue(PreviewProperty); }
			private set { SetValue(PreviewPropertyKey, value); }
		}
		void IReportDesignerDocument.OnClosing(CancelEventArgs e) {
			ReportDesignerDocumentClosingEventArgs args = new ReportDesignerDocumentClosingEventArgs(this, e.Cancel);
			OnClosing(args);
			e.Cancel = args.Cancel;
		}
		void IReportDesignerDocument.OnClosed() {
			OnClosed();
		}
		object IReportDesignerDocument.Preview { set { Preview = value; } }
		#endregion
		sealed class ReportModelOwner : IReportModelOwner {
			readonly ReportDesignerDocument document;
			public ReportModelOwner(ReportDesignerDocument document) {
				this.document = document;
			}
			public void ShowProperties() {
				document.ReportDesignerUI.ActivatePropertyGrid();
			}
			public void OpenSubreport(XRSubreportDiagramItem subreportDiagramItem) {
				document.ReportDesignerUI.OpenSubreport(subreportDiagramItem);
			}
			public void RunWizard() {
				document.RunWizard();
			}
			public IEnumerable<IModelRegistry> ModelRegistries { get { return document.ReportDesignerUI.ModelRegistries ?? Enumerable.Empty<IModelRegistry>(); } }
		}
		public void RunWizard() {
			ReportDesignerUI.DoWithWizardDialogService(dialog => {
				var viewModel = new ReportWizardViewModel() { EnableCustomSql = ReportDesignerUI.EnableCustomSql };
				if(dialog.ShowDialog(MessageButton.OKCancel, "Wizard", viewModel) == MessageResult.OK) {
					Report = viewModel.Report;
					Diagram.SetHasChanges(true);
				}
			});
		}
	}
}
