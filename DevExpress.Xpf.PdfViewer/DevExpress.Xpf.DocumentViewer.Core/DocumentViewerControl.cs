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
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using DevExpress.Xpf.Utils;
using DevExpress.Xpf.Core.Native;
using System.Windows.Input;
using DevExpress.Mvvm.Native;
using System.Windows.Forms;
using Control = System.Windows.Controls.Control;
using System.Windows.Media.Imaging;
using DevExpress.Xpf.DocumentViewer.Extensions;
using DevExpress.Xpf.Core;
using DevExpress.Mvvm;
using DevExpress.Mvvm.UI.Interactivity;
using DevExpress.Mvvm.UI;
using System.IO;
using DevExpress.Mvvm.UI.Native;
using DevExpress.Xpf.Core.Internal;
using System.Windows.Markup;
using System.Reflection;
namespace DevExpress.Xpf.DocumentViewer {
	public enum ScrollCommand {
		PageDown,
		PageUp,
		LineDown,
		LineUp,
		LineLeft,
		LineRight,
		Home,
		End
	}
	public enum ZoomMode {
		Custom,
		ActualSize,
		FitToWidth,
		FitToVisible,
		PageLevel
	}
	public enum CommandBarStyle {
		None,
		Bars,
		Ribbon
	}
	public class DocumentViewerControl : Control {
		public static readonly DependencyProperty BarItemNameProperty;
		public static readonly DependencyProperty ActualViewerProperty;
		public static readonly DependencyProperty DocumentSourceProperty;
		public static readonly DependencyProperty ZoomFactorProperty;
		public static readonly DependencyProperty ZoomModeProperty;
		public static readonly DependencyProperty PageRotationProperty;
		public static readonly DependencyProperty CurrentPageNumberProperty;
		public static readonly DependencyProperty BehaviorProviderProperty;
		public static readonly DependencyProperty CommandProviderProperty;
		public static readonly DependencyProperty CommandBarStyleProperty;
		public static readonly DependencyProperty BarsTemplateProperty;
		public static readonly DependencyProperty RibbonTemplateProperty;
		public static readonly DependencyProperty PresenterTemplateProperty;
		public static readonly DependencyProperty ResetSettingsOnDocumentCloseProperty;
		public static readonly DependencyProperty OpenFileDialogTemplateProperty;
		public static readonly DependencyProperty HorizontalPageSpacingProperty;
		public static readonly DependencyProperty DocumentProperty;
		static readonly DependencyPropertyKey DocumentPropertyKey;
		public static readonly DependencyProperty PageCountProperty;
		static readonly DependencyPropertyKey PageCountPropertyKey;
		public static readonly DependencyProperty ActualBehaviorProviderProperty;
		static readonly DependencyPropertyKey ActualBehaviorProviderPropertyKey;
		public static readonly DependencyProperty ActualCommandProviderProperty;
		static readonly DependencyPropertyKey ActualCommandProviderPropertyKey;
		public static readonly DependencyProperty ActualBarsTemplateProperty;
		static readonly DependencyPropertyKey ActualBarsTemplatePropertyKey;
		public static readonly DependencyProperty UndoRedoManagerProperty;
		static readonly DependencyPropertyKey UndoRedoManagerPropertyKey;
		public static readonly DependencyProperty IsSearchControlVisibleProperty;
		static readonly DependencyPropertyKey IsSearchControlVisiblePropertyKey;
		static readonly DependencyPropertyKey ActualDocumentMapSettingsPropertyKey;
		public static readonly DependencyProperty ActualDocumentMapSettingsProperty;
		private static readonly DependencyPropertyKey PropertyProviderPropertyKey;
		public static readonly DependencyProperty PropertyProviderProperty;
		public static RoutedEvent DocumentChangedEvent;
		public static RoutedEvent ZoomChangedEvent;
		public static RoutedEvent PageRotationChangedEvent;
		public static RoutedEvent CurrentPageNumberChangedEvent;
		static DocumentViewerControl() {
			Type ownerType = typeof(DocumentViewerControl);
			BarItemNameProperty = DependencyPropertyManager.RegisterAttached("BarItemName", typeof(string), ownerType,
				new FrameworkPropertyMetadata(string.Empty, FrameworkPropertyMetadataOptions.None, OnBarItemNameChanged));
			ActualViewerProperty = DependencyPropertyManager.RegisterAttached("ActualViewer", typeof(DocumentViewerControl), ownerType,
				new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.Inherits));
			DocumentSourceProperty = DependencyPropertyManager.Register("DocumentSource", typeof(object), ownerType,
				new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.None, 
					(obj, args) => ((DocumentViewerControl)obj).OnDocumentSourceChanged((object)args.OldValue, (object)args.NewValue)));
			ZoomFactorProperty = DependencyPropertyManager.Register("ZoomFactor", typeof(double), ownerType,
				new FrameworkPropertyMetadata(1d, FrameworkPropertyMetadataOptions.None, 
					(obj, args) => ((DocumentViewerControl)obj).OnZoomFactorChangedInternal((double)args.OldValue, (double)args.NewValue),
					(obj, arg) => ((DocumentViewerControl)obj).CoerceZoomFactor(arg)));
			ZoomModeProperty = DependencyPropertyManager.Register("ZoomMode", typeof(ZoomMode), ownerType, 
				new FrameworkPropertyMetadata(ZoomMode.ActualSize, FrameworkPropertyMetadataOptions.None, 
					(obj, args) => ((DocumentViewerControl)obj).OnZoomModeChangedInternal((ZoomMode)args.OldValue, (ZoomMode)args.NewValue)));
			PageRotationProperty = DependencyPropertyManager.Register("PageRotation", typeof(Rotation), ownerType,
				new FrameworkPropertyMetadata(Rotation.Rotate0, FrameworkPropertyMetadataOptions.None, 
					(obj, args) => ((DocumentViewerControl)obj).OnPageRotationChangedInternal((Rotation)args.OldValue, (Rotation)args.NewValue))); 
			CurrentPageNumberProperty = DependencyPropertyManager.Register("CurrentPageNumber", typeof(int), ownerType, 
				new FrameworkPropertyMetadata(1, FrameworkPropertyMetadataOptions.None, 
					(obj, args) => ((DocumentViewerControl)obj).OnCurrentPageNumberChangedInternal((int)args.OldValue, (int)args.NewValue),
					(obj, arg) => ((DocumentViewerControl)obj).CoerceCurrentPageNumber(arg)));
			BehaviorProviderProperty = DependencyPropertyManager.Register("BehaviorProvider", typeof(BehaviorProvider), ownerType, 
				new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.None, 
					(obj, args) => ((DocumentViewerControl)obj).OnBehaviorProviderChangedInternal((BehaviorProvider)args.OldValue, (BehaviorProvider)args.NewValue)));
			CommandProviderProperty = DependencyPropertyManager.Register("CommandProvider", typeof(CommandProvider), ownerType, 
				new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.None, 
					(obj, args) => ((DocumentViewerControl)obj).OnCommandProviderChangedInternal((CommandProvider)args.OldValue, (CommandProvider)args.NewValue)));
			CommandBarStyleProperty = DependencyPropertyManager.Register("CommandBarStyle", typeof(CommandBarStyle), ownerType,
				new FrameworkPropertyMetadata(CommandBarStyle.Ribbon, FrameworkPropertyMetadataOptions.None, 
					(obj, args) => ((DocumentViewerControl)obj).OnCommandBarStyleChangedInternal((CommandBarStyle)args.OldValue, (CommandBarStyle)args.NewValue)));
			BarsTemplateProperty = DependencyPropertyManager.Register("BarsTemplate", typeof(DataTemplate), ownerType, 
				new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.None, 
					(obj, args) => ((DocumentViewerControl)obj).OnBarsTemplateChangedInternal((DataTemplate)args.NewValue)));
			RibbonTemplateProperty = DependencyPropertyManager.Register("RibbonTemplate", typeof(DataTemplate), ownerType,
				new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.None, 
					(obj, args) => ((DocumentViewerControl)obj).OnRibbonTemplateChangedInternal((DataTemplate)args.NewValue)));
			PresenterTemplateProperty = DependencyPropertyManager.Register("PresenterTemplate", typeof(DataTemplate), ownerType, 
				new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.None,
					(obj, args) => ((DocumentViewerControl)obj).OnPresenterTemplateChanged((DataTemplate)args.NewValue)));
			ResetSettingsOnDocumentCloseProperty = DependencyPropertyManager.Register("ResetSettingsOnDocumentClose", typeof(bool), ownerType, 
				new FrameworkPropertyMetadata(true));
			OpenFileDialogTemplateProperty = DependencyPropertyManager.Register("OpenFileDialogTemplate", typeof(DataTemplate), ownerType,
				new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.None));
			HorizontalPageSpacingProperty = DependencyPropertyRegistrator.Register<DocumentViewerControl, double>(owner => owner.HorizontalPageSpacing, 10d, 
				(owner, oldValue, newValue) => owner.OnHorizontalPageSpacingPropertyChanged(oldValue, newValue));
			DocumentPropertyKey = DependencyPropertyManager.RegisterReadOnly("Document", typeof(IDocument), ownerType,
				new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.None,
					(obj, args) => ((DocumentViewerControl)obj).OnDocumentChangedInternal((IDocument)args.OldValue, (IDocument)args.NewValue)));
			DocumentProperty = DocumentPropertyKey.DependencyProperty;
			PageCountPropertyKey = DependencyPropertyManager.RegisterReadOnly("PageCount", typeof(int), ownerType, new FrameworkPropertyMetadata(0));
			PageCountProperty = PageCountPropertyKey.DependencyProperty;
			ActualBehaviorProviderPropertyKey = DependencyPropertyManager.RegisterReadOnly("ActualBehaviorProvider", typeof(BehaviorProvider), ownerType, 
				new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.None, 
					(obj, args) => ((DocumentViewerControl)obj).OnActualBehaviorProviderChangedInternal((BehaviorProvider)args.OldValue, (BehaviorProvider)args.NewValue)));
			ActualBehaviorProviderProperty = ActualBehaviorProviderPropertyKey.DependencyProperty;
			ActualCommandProviderPropertyKey = DependencyPropertyManager.RegisterReadOnly("ActualCommandProvider", typeof(CommandProvider), ownerType, 
				new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.None, 
					(obj, args) => ((DocumentViewerControl)obj).OnActualCommandProviderChangedInternal((CommandProvider)args.OldValue, (CommandProvider)args.NewValue)));
			ActualCommandProviderProperty = ActualCommandProviderPropertyKey.DependencyProperty;
			ActualBarsTemplatePropertyKey = DependencyPropertyManager.RegisterReadOnly("ActualBarsTemplate", typeof(DataTemplate), ownerType,
				new FrameworkPropertyMetadata(null));
			ActualBarsTemplateProperty = ActualBarsTemplatePropertyKey.DependencyProperty;
			UndoRedoManagerPropertyKey = DependencyPropertyManager.RegisterReadOnly("UndoRedoManager", typeof(UndoRedoManager), ownerType,
				new FrameworkPropertyMetadata(null));
			UndoRedoManagerProperty = UndoRedoManagerPropertyKey.DependencyProperty; 
			IsSearchControlVisiblePropertyKey = DependencyPropertyManager.RegisterReadOnly("IsSearchControlVisible", typeof(bool), ownerType,
				 new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.None, 
					 (obj, args) => ((DocumentViewerControl)obj).OnSearchControlVisibleChanged((bool)args.NewValue)));
			IsSearchControlVisibleProperty = IsSearchControlVisiblePropertyKey.DependencyProperty;
			ActualDocumentMapSettingsPropertyKey = DependencyPropertyRegistrator.RegisterReadOnly<DocumentViewerControl, DocumentMapSettings>(
				owner => owner.ActualDocumentMapSettings, null, (owner, oldValue, newValue) => owner.ActualDocumentMapSettingsChanged(oldValue, newValue));
			ActualDocumentMapSettingsProperty = ActualDocumentMapSettingsPropertyKey.DependencyProperty;
			PropertyProviderPropertyKey = DependencyPropertyRegistrator.RegisterReadOnly<DocumentViewerControl, PropertyProvider>(owner => owner.PropertyProvider, null);
			PropertyProviderProperty = PropertyProviderPropertyKey.DependencyProperty;
			DocumentChangedEvent = EventManager.RegisterRoutedEvent("DocumentChanged", RoutingStrategy.Direct, typeof(RoutedEventHandler), ownerType);
			ZoomChangedEvent = EventManager.RegisterRoutedEvent("ZoomChanged", RoutingStrategy.Direct, typeof(RoutedEventHandler), ownerType);
			PageRotationChangedEvent = EventManager.RegisterRoutedEvent("PageRotationChanged", RoutingStrategy.Direct, typeof(RoutedEventHandler), ownerType);
			CurrentPageNumberChangedEvent = EventManager.RegisterRoutedEvent("CurrentPageNumberChanged", RoutingStrategy.Direct, typeof(RoutedEventHandler), ownerType);
		}
		static void OnBarItemNameChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			if (!d.IsPropertySet(NameProperty))
				d.SetValue(NameProperty, e.NewValue);
		}
		public static string GetBarItemName(DependencyObject d) {
			return (string)d.GetValue(BarItemNameProperty);
		}
		public static void SetBarItemName(DependencyObject d, string value) {
			d.SetValue(BarItemNameProperty, value);
		}
		public static DocumentViewerControl GetActualViewer(DependencyObject d) {
			return (DocumentViewerControl)d.GetValue(ActualViewerProperty);
		}
		public static void SetActualViewer(DependencyObject d, DocumentViewerControl value) {
			d.SetValue(ActualViewerProperty, value);
		}
		void OnZoomFactorChangedInternal(double oldValue, double newValue) {
			ActualBehaviorProvider.Do(x => x.ZoomFactor = newValue);
			OnZoomFactorChanged(oldValue, newValue);
		}
		void OnZoomModeChangedInternal(ZoomMode oldValue, ZoomMode newValue) {
			ActualBehaviorProvider.Do(x => x.ZoomMode = newValue);
			OnZoomModeChanged(oldValue, newValue);
		}
		void OnPageRotationChangedInternal(Rotation oldValue, Rotation newValue) {
			ActualBehaviorProvider.Do(x => x.RotateAngle = 90 * (int)newValue);
		}
		void OnBehaviorProviderChangedInternal(BehaviorProvider oldValue, BehaviorProvider newValue) {
			OnBehaviorProviderChanged(oldValue, newValue);
			ActualBehaviorProvider = newValue;
		}
		void OnCommandProviderChangedInternal(CommandProvider oldValue, CommandProvider newValue) {
			OnCommandProviderChanged(oldValue, newValue);
			ActualCommandProvider = newValue;
		}
		void OnDocumentChangedInternal(IDocument oldValue, IDocument newValue) {
			documentInternal = newValue;
			PageCount = newValue.With(x => x.Pages).Return(x => x.Count(), () => 0);
			CurrentPageNumber = 1;
			AssignDocumentPresenterProperties();
			OnDocumentChanged(oldValue, newValue);
			RaiseDocumentChanged();
		}
		void OnActualCommandProviderChangedInternal(CommandProvider oldValue, CommandProvider newValue) {
			oldValue.Do(x => x.DocumentViewer = null);
			newValue.Do(x => x.DocumentViewer = this);
			OnActualCommandProviderChanged(oldValue, newValue);
		}
		void OnActualBehaviorProviderChangedInternal(BehaviorProvider oldValue, BehaviorProvider newValue) {
			oldValue.Do(x => x.ZoomChanged -= OnBehaviorProviderZoomChanged);
			oldValue.Do(x => x.RotateAngleChanged -= OnBehaviorProviderRotateAngleChanged);
			oldValue.Do(x => x.PageIndexChanged -= OnBehaviorProviderPageIndexChanged);
			newValue.Do(x => x.ZoomFactor = ZoomFactor);
			newValue.Do(x => x.ZoomChanged += OnBehaviorProviderZoomChanged);
			newValue.Do(x => x.RotateAngleChanged += OnBehaviorProviderRotateAngleChanged);
			newValue.Do(x => x.PageIndexChanged += OnBehaviorProviderPageIndexChanged);
			AssignDocumentPresenterProperties();
			OnActualBehaviorProviderChanged(oldValue, newValue);
		}
		void OnBehaviorProviderRotateAngleChanged(object sender, RotateAngleChangedEventArgs e) {
			int index = ((int)e.NewValue % 360) / 90;
			PageRotation = (Rotation)index;
			RaisePageRotationChanged();
		}
		void OnBehaviorProviderZoomChanged(object sender, ZoomChangedEventArgs e) {
			ZoomMode = e.ZoomMode;
			ZoomFactor = e.ZoomFactor;
			RaiseZoomChanged();
		}
		void OnBehaviorProviderPageIndexChanged(object sender, PageIndexChangedEventArgs e) {
			CurrentPageNumber = e.PageIndex + 1;
		}
		void OnCurrentPageNumberChangedInternal(int oldValue, int newValue) {
			ActualBehaviorProvider.PageIndex = newValue - 1;
			OnCurrentPageNumberChanged(oldValue, newValue);
			RaiseCurrentPageNumberChanged();
		}
		void OnCommandBarStyleChangedInternal(CommandBarStyle oldValue, CommandBarStyle newValue) {
			OnCommandBarStyleChanged(oldValue, newValue);
			UpdateCommandBar();
		}
		void OnBarsTemplateChangedInternal(DataTemplate newValue) {
			OnBarsTemplateChanged(newValue);
			UpdateCommandBar();
		}
		void OnRibbonTemplateChangedInternal(DataTemplate newValue) {
			OnRibbonTemplateChanged(newValue);
			UpdateCommandBar();
		}
		void OnSearchControlVisibleChanged(bool newValue) {
			AssignDocumentPresenterProperties();
		}
		public DocumentMapSettings ActualDocumentMapSettings {
			get { return (DocumentMapSettings)GetValue(ActualDocumentMapSettingsProperty); }
			protected set { SetValue(ActualDocumentMapSettingsPropertyKey, value);}
		}
		public IDocument Document {
			get { return (IDocument)GetValue(DocumentProperty); }
			protected set { SetValue(DocumentPropertyKey, value); }
		}
		public object DocumentSource {
			get { return (object)GetValue(DocumentSourceProperty); }
			set { SetValue(DocumentSourceProperty, value); }
		}
		public double ZoomFactor {
			get { return (double)GetValue(ZoomFactorProperty); }
			set { SetValue(ZoomFactorProperty, value); }
		}
		public ZoomMode ZoomMode {
			get { return (ZoomMode)GetValue(ZoomModeProperty); }
			set { SetValue(ZoomModeProperty, value); }
		}
		public Rotation PageRotation {
			get { return (Rotation)GetValue(PageRotationProperty); }
			set { SetValue(PageRotationProperty, value); }
		}
		public int CurrentPageNumber {
			get { return (int)GetValue(CurrentPageNumberProperty); }
			set { SetValue(CurrentPageNumberProperty, value); }
		}
		public BehaviorProvider BehaviorProvider {
			get { return (BehaviorProvider)GetValue(BehaviorProviderProperty); }
			set { SetValue(BehaviorProviderProperty, value); }
		}
		public CommandProvider CommandProvider {
			get { return (CommandProvider)GetValue(CommandProviderProperty); }
			set { SetValue(CommandProviderProperty, value); }
		}
		public CommandBarStyle CommandBarStyle {
			get { return (CommandBarStyle)GetValue(CommandBarStyleProperty); }
			set { SetValue(CommandBarStyleProperty, value); }
		}
		public DataTemplate BarsTemplate {
			get { return (DataTemplate)GetValue(BarsTemplateProperty); }
			set { SetValue(BarsTemplateProperty, value); }
		}
		public DataTemplate RibbonTemplate {
			get { return (DataTemplate)GetValue(RibbonTemplateProperty); }
			set { SetValue(RibbonTemplateProperty, value); }
		}
		public DataTemplate PresenterTemplate {
			get { return (DataTemplate)GetValue(PresenterTemplateProperty); }
			set { SetValue(PresenterTemplateProperty, value); }
		}
		public bool ResetSettingsOnDocumentClose {
			get { return (bool)GetValue(ResetSettingsOnDocumentCloseProperty); }
			set { SetValue(ResetSettingsOnDocumentCloseProperty, value); }
		}
		public DataTemplate OpenFileDialogTemplate {
			get { return (DataTemplate)GetValue(OpenFileDialogTemplateProperty); }
			set { SetValue(OpenFileDialogTemplateProperty, value); }
		}
		public double HorizontalPageSpacing {
			get { return (double)GetValue(HorizontalPageSpacingProperty); }
			set { SetValue(HorizontalPageSpacingProperty, value); }
		}
		public int PageCount {
			get { return (int)GetValue(PageCountProperty); }
			protected set { SetValue(PageCountPropertyKey, value); }
		}
		public BehaviorProvider ActualBehaviorProvider {
			get { return (BehaviorProvider)GetValue(ActualBehaviorProviderProperty); }
			private set { SetValue(ActualBehaviorProviderPropertyKey, value); }
		}
		public CommandProvider ActualCommandProvider {
			get { return (CommandProvider)GetValue(ActualCommandProviderProperty); }
			private set { SetValue(ActualCommandProviderPropertyKey, value); }
		}
		public DataTemplate ActualBarsTemplate {
			get { return (DataTemplate)GetValue(ActualBarsTemplateProperty); }
			private set { SetValue(ActualBarsTemplatePropertyKey, value); }
		}
		public UndoRedoManager UndoRedoManager {
			get { return (UndoRedoManager)GetValue(UndoRedoManagerProperty); }
			private set { SetValue(UndoRedoManagerPropertyKey, value); }
		}
		public bool IsSearchControlVisible {
			get { return (bool)GetValue(IsSearchControlVisibleProperty); }
			private set { SetValue(IsSearchControlVisiblePropertyKey, value); }
		}
		public PropertyProvider PropertyProvider {
			get { return (PropertyProvider)GetValue(PropertyProviderProperty); }
			private set { SetValue(PropertyProviderPropertyKey, value); }
		}
		public event RoutedEventHandler DocumentChanged {
			add { AddHandler(DocumentChangedEvent, value); }
			remove { RemoveHandler(DocumentChangedEvent, value); }
		}
		public event RoutedEventHandler ZoomChanged {
			add { AddHandler(ZoomChangedEvent, value); }
			remove { RemoveHandler(ZoomChangedEvent, value); }
		}
		public event RoutedEventHandler PageRotationChanged {
			add { AddHandler(PageRotationChangedEvent, value); }
			remove { RemoveHandler(PageRotationChangedEvent, value); }
		}
		public event RoutedEventHandler CurrentPageNumberChanged {
			add { AddHandler(CurrentPageNumberChangedEvent, value); }
			remove { RemoveHandler(CurrentPageNumberChangedEvent, value); }
		}
		public ICommand OpenDocumentCommand { get; private set; }
		public ICommand CloseDocumentCommand { get; private set; }
		public ICommand ZoomInCommand { get; private set; }
		public ICommand ZoomOutCommand { get; private set; }
		public ICommand ClockwiseRotateCommand { get; private set; }
		public ICommand CounterClockwiseRotateCommand { get; private set; }
		public ICommand NextPageCommand { get; private set; }
		public ICommand PreviousPageCommand { get; private set; }
		public ICommand SetPageNumberCommand { get; private set; }
		public ICommand SetZoomFactorCommand { get; private set; }
		public ICommand SetZoomModeCommand { get; private set; }
		public ICommand ScrollCommand { get; private set; }
		public ICommand NextViewCommand { get; private set; }
		public ICommand PreviousViewCommand { get; private set; }
		public ICommand ShowFindTextCommand { get; private set; }
		public ICommand FindTextCommand { get; private set; }
		public ICommand NavigateCommand { get; private set; }
		protected IDocument documentInternal;
		protected internal DocumentPresenterControl DocumentPresenter { get; private set; }
		public DocumentViewerControl() {
			DefaultStyleKey = typeof(DocumentViewerControl);
			PropertyProvider = CreatePropertyProvider();
			ActualBehaviorProvider = CreateBehaviorProvider();
			ActualCommandProvider = CreateCommandProvider();
			UndoRedoManager = CreateUndoRedoManager();
			ActualDocumentMapSettings = CreateDefaultDocumentMapSettings();
			SetActualViewer(this, this);
			InitializeCommands();
		}
		public virtual void ScrollToVerticalOffset(double offset) {
			DocumentPresenter.Do(x => x.ScrollToVerticalOffset(offset));
		}
		public virtual void ScrollToHorizontalOffset(double offset) {
			DocumentPresenter.Do(x => x.ScrollToHorizontalOffset(offset));
		}
		public virtual void FindText(TextSearchParameter search) {
			FindTextCommand.TryExecute(search);
		}
		void RaiseDocumentChanged() {
			RaiseEvent(new RoutedEventArgs(DocumentChangedEvent));
		}
		void RaiseZoomChanged() {
			RaiseEvent(new RoutedEventArgs(ZoomChangedEvent));
		}
		void RaisePageRotationChanged() {
			RaiseEvent(new RoutedEventArgs(PageRotationChangedEvent));
		}
		void RaiseCurrentPageNumberChanged() {
			RaiseEvent(new RoutedEventArgs(CurrentPageNumberChangedEvent));
		}
		void UpdateCommandBar() {
			switch(CommandBarStyle) {
				case CommandBarStyle.None:
					ActualBarsTemplate = PresenterTemplate;
					break;
				case CommandBarStyle.Bars:
					ActualBarsTemplate = BarsTemplate;
					break;
				case CommandBarStyle.Ribbon:
					ActualBarsTemplate = RibbonTemplate;
					break;
			}
		}
		bool ChooseFile(string documentName, out string fileName) {
			var openFileDialogService = OpenFileDialogTemplate.Return(TemplateHelper.LoadFromTemplate<OpenFileDialogService>, CreateDefaultOpenFileDialogService);
			IFileInfo fileInfo = null;
			AssignableServiceHelper2<DocumentViewerControl, OpenFileDialogService>.DoServiceAction(this, openFileDialogService, service => {
				IOpenFileDialogService openFileService = service;
				fileInfo = openFileService.ShowDialog() ? openFileService.Files.FirstOrDefault() : null;
			});
			if (fileInfo == null) {
				fileName = null;
				return false;
			}
			fileName = Path.Combine(fileInfo.DirectoryName, fileInfo.Name);
			return true;
		}
		Rotation GetClockwiseRotation(Rotation rotation) {
			int index = (int)rotation + 1;
			return index > Enum.GetValues(typeof(Rotation)).Length - 1 ? Rotation.Rotate0 : (Rotation)index;
		}
		Rotation GetCounterClockwiseRotation(Rotation rotation) {
			int index = (int)rotation - 1;
			return index < 0 ? Rotation.Rotate270 : (Rotation)index;
		}
		protected internal bool IsDocumentContainPages() {
			return Document.Return(x => x.IsLoaded, () => false) && Document.Pages.Any();
		}
		protected override void OnInitialized(EventArgs e) {
			base.OnInitialized(e);
			if (ActualBarsTemplate == null)
				UpdateCommandBar(); 
		}
		protected virtual string GetOpenFileFilter() {
			return "";
		}
		protected virtual IDocument CreateDocument(object source) {
			return null;
		}
		protected virtual void ReleaseDocument(IDocument document) {
		}
		protected virtual void InitializeCommands() {
			OpenDocumentCommand = DelegateCommandFactory.Create<string>(OpenDocument, CanOpenDocument);
			CloseDocumentCommand = DelegateCommandFactory.Create(CloseDocument, CanCloseDocument);
			ZoomInCommand = DelegateCommandFactory.Create(ActualBehaviorProvider.ZoomIn, CanZoomIn);
			ZoomOutCommand = DelegateCommandFactory.Create(ActualBehaviorProvider.ZoomOut, CanZoomOut);
			ClockwiseRotateCommand = DelegateCommandFactory.Create(ClockwiseRotate, CanClockwiseRotate);
			CounterClockwiseRotateCommand = DelegateCommandFactory.Create(CounterClockwiseRotate, CanCounterClockwiseRotate);
			NextPageCommand = DelegateCommandFactory.Create(NavigateNextPage, CanNavigateNextPage);
			PreviousPageCommand = DelegateCommandFactory.Create(NavigatePreviousPage, CanNavigatePreviousPage);
			SetPageNumberCommand = DelegateCommandFactory.Create<int>(SetPageNumber, CanSetPageNumber);
			SetZoomFactorCommand = DelegateCommandFactory.Create<double>(SetZoomFactor, CanSetZoomFactor);
			SetZoomModeCommand = DelegateCommandFactory.Create<ZoomMode>(SetZoomMode, CanSetZoomMode);
			ScrollCommand = DelegateCommandFactory.Create<ScrollCommand>(Scroll, CanScroll);
			NextViewCommand = DelegateCommandFactory.Create(Redo, CanRedo);
			PreviousViewCommand = DelegateCommandFactory.Create(Undo, CanUndo);
			ShowFindTextCommand = DelegateCommandFactory.Create<bool?>(ShowFindText, CanShowFindText);
			FindTextCommand = DelegateCommandFactory.Create<TextSearchParameter>(FindNextText, CanFindNextText);
			NavigateCommand = DelegateCommandFactory.Create<object>(ExecuteNavigate, CanExecuteNavigate);
		}
		protected virtual bool CanExecuteNavigate(object parameter) {
			return parameter != null;
		}
		protected virtual void ExecuteNavigate(object parameter) {
		}
		protected virtual OpenFileDialogService CreateDefaultOpenFileDialogService() {
			return new OpenFileDialogService() {
				Title = DocumentViewerLocalizer.GetString(DocumentViewerStringId.OpenFileDialogTitle),
				Filter = GetOpenFileFilter(),
				RestoreDirectory = true
			};
		}
		public virtual void OpenDocument(string filePath = null) {
			string path;
			if (!string.IsNullOrEmpty(filePath) && File.Exists(filePath)) {
				DocumentSource = filePath;
				return;
			}
			if (!ChooseFile(filePath, out path))
				return;
			DocumentSource = path;
		}
		protected virtual bool CanOpenDocument(string filePath = null) {
			return true;
		}
		protected virtual void CloseDocument() {
			DocumentSource = null;
		}
		protected virtual bool CanCloseDocument() {
			return DocumentSource != null;
		}
		protected virtual bool CanZoomIn() {
			return IsDocumentContainPages() && ActualBehaviorProvider.CanZoomIn();
		}
		protected virtual bool CanZoomOut() {
			return IsDocumentContainPages() && ActualBehaviorProvider.CanZoomOut();
		}
		protected virtual void ClockwiseRotate() {
			PageRotation = GetClockwiseRotation(PageRotation);
		}
		protected virtual bool CanClockwiseRotate() {
			return IsDocumentContainPages();
		}
		protected virtual void CounterClockwiseRotate() {
			PageRotation = GetCounterClockwiseRotation(PageRotation);
		}
		protected virtual bool CanCounterClockwiseRotate() {
			return IsDocumentContainPages();
		}
		protected virtual void NavigateNextPage() {
			CurrentPageNumber++;
		}
		protected virtual bool CanNavigateNextPage() {
			return Document.Return(x => x.IsLoaded, () => false) && CurrentPageNumber < PageCount;
		}
		protected virtual void NavigatePreviousPage() {
			CurrentPageNumber--;
		}
		protected virtual bool CanNavigatePreviousPage() {
			return Document.Return(x => x.IsLoaded, () => false) && CurrentPageNumber > 1;
		}
		protected virtual void SetPageNumber(int pageNumber) {
			CurrentPageNumber = pageNumber;
		}
		protected virtual bool CanSetPageNumber(int pageNumber) {
			return IsDocumentContainPages();
		}
		protected virtual void SetZoomFactor(double zoomFactor) {
			ActualBehaviorProvider.ZoomFactor = zoomFactor;
		}
		protected virtual bool CanSetZoomFactor(double zoomFactor) {
			return IsDocumentContainPages();
		}
		protected virtual void SetZoomMode(ZoomMode zoomMode) {
			ActualBehaviorProvider.ZoomMode = zoomMode;
		}
		protected virtual bool CanSetZoomMode(ZoomMode zoomMode) {
			return IsDocumentContainPages();
		}
		protected virtual void Scroll(ScrollCommand command) {
			DocumentPresenter.Do(x => x.Scroll(command));
		}
		protected virtual bool CanScroll(ScrollCommand command) {
			return Document.Return(x => x.IsLoaded, () => false);
		}
		protected virtual void Undo() {
			UndoRedoManager.Undo();
		}
		protected virtual bool CanUndo() {
			return UndoRedoManager.CanUndo;
		}
		protected virtual void Redo() {
			UndoRedoManager.Redo();
		}
		protected virtual bool CanRedo() {
			return UndoRedoManager.CanRedo;
		}
		protected virtual bool CanFindNextText(TextSearchParameter parameter) {
			return IsDocumentContainPages();
		}
		protected virtual void FindNextText(TextSearchParameter parameter) {
		}
		protected virtual bool CanShowFindText(bool? show) {
			return IsDocumentContainPages();
		}
		protected virtual void ShowFindText(bool? show) {
			IsSearchControlVisible = show == null || show.Value;
		}
		protected virtual PropertyProvider CreatePropertyProvider() {
			return new PropertyProvider();
		}
		protected virtual BehaviorProvider CreateBehaviorProvider() {
			return new BehaviorProvider();
		}
		protected virtual CommandProvider CreateCommandProvider() {
			return new CommandProvider();
		}
		protected virtual UndoRedoManager CreateUndoRedoManager() {
			return new UndoRedoManager(Dispatcher);
		}
		protected virtual DocumentMapSettings CreateDefaultDocumentMapSettings() {
			return new DocumentMapSettings();
		}
		protected virtual void OnDocumentChanged(IDocument oldValue, IDocument newValue) { }
		protected virtual void OnDocumentSourceChanged(object oldValue, object newValue) {
			ReleaseDocument(documentInternal);
			if (newValue == null) {
				CurrentPageNumber = 1;
				Document = null;
				documentInternal = null;
				return;
			} 
			if (ResetSettingsOnDocumentClose)
				ZoomFactor = 1;
			PageRotation = Rotation.Rotate0;
			CurrentPageNumber = 1;
			Document = CreateDocument(newValue);
			LoadDocument(newValue);
		}
		protected virtual void LoadDocument(object source) {
		}
		protected virtual void OnZoomFactorChanged(double oldValue, double newValue) { }
		protected virtual void OnZoomModeChanged(ZoomMode oldValue, ZoomMode newValue) { }
		protected virtual void OnPageRotationChanged(Rotation oldValue, Rotation newValue) { }
		protected virtual void OnCurrentPageNumberChanged(int oldValue, int newValue) { }
		protected virtual void OnBehaviorProviderChanged(BehaviorProvider oldValue, BehaviorProvider newValue) { }
		protected virtual void OnCommandProviderChanged(CommandProvider oldValue, CommandProvider newValue) {
			if (newValue.GetType() != typeof(CommandProvider))
				CommandBarStyle = CommandBarStyle.None;
		}
		protected virtual void OnActualCommandProviderChanged(CommandProvider oldValue, CommandProvider newValue) { }
		protected virtual void OnCommandBarStyleChanged(CommandBarStyle oldValue, CommandBarStyle newValue) { }
		protected virtual void OnActualBehaviorProviderChanged(BehaviorProvider oldValue, BehaviorProvider newValue) { }
		protected virtual void OnBarsTemplateChanged(DataTemplate newValue) { }
		protected virtual void OnRibbonTemplateChanged(DataTemplate newValue) { }
		protected virtual void OnPresenterTemplateChanged(DataTemplate newValue) { }
		protected virtual void OnHorizontalPageSpacingPropertyChanged(double oldValue, double newValue) {
			AssignDocumentPresenterProperties();
		}
		public virtual void AttachDocumentPresenterControl(DocumentPresenterControl documentPresenter) {
			if(DocumentPresenter != null)
				DetachDocumentPresenterControl();
			DocumentPresenter = documentPresenter;
			AssignDocumentPresenterProperties();
		}
		protected virtual void AssignDocumentPresenterProperties() {
			DocumentPresenter.Do(x => {
				x.Document = Document;
				x.BehaviorProvider = ActualBehaviorProvider;
				x.IsSearchControlVisible = IsSearchControlVisible;
				x.HorizontalPageSpacing = HorizontalPageSpacing;
			});
		}
		protected virtual void DetachDocumentPresenterControl() {
			DocumentPresenter.Do(x => {
				x.BehaviorProvider = null;
				x.Document = null;
			});
		}
		protected virtual object CoerceCurrentPageNumber(object value) {
			var intValue = (int)value;
			if (!Document.Return(x => x.IsLoaded, () => false))
				return 1;
			return Math.Max(1, Math.Min(PageCount, intValue));
		}
		protected virtual object CoerceZoomFactor(object value) {
			return value;
		}
		protected virtual void ActualDocumentMapSettingsChanged(DocumentMapSettings oldValue, DocumentMapSettings newValue) {
			oldValue.Do(x => x.Release());
			newValue.Do(x => x.Initialize(this));
		}
	}
	public class PropertyProvider : BindableBase {
	}
	public class DocumentViewerResourceExtension : MarkupExtension {
		public string ResourcePath { get; set; }
		readonly string dllName;
		public DocumentViewerResourceExtension(string resourcePath) {
			ResourcePath = resourcePath;
			dllName = Assembly.GetExecutingAssembly().GetName().Name;
		}
		public sealed override object ProvideValue(IServiceProvider serviceProvider) {
			return UriHelper.GetUri(dllName, ResourcePath);
		}
	}
}
