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
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Windows;
using System.Windows.Input;
using DevExpress.Xpf.Core;
using DevExpress.XtraPrinting;
using DevExpress.Mvvm;
using DevExpress.Mvvm.Native;
namespace DevExpress.Xpf.Printing {
	public abstract partial class PreviewModelBase : IPreviewModel {
		#region Fields and Properties
		const double defaultZoomValue = 100d;
		protected readonly Dictionary<PrintingSystemCommand, DelegateCommand<object>> commands = new Dictionary<PrintingSystemCommand, DelegateCommand<object>>();
		readonly InputController inputController;
		bool isLoading;
		bool isIncorrectPageContent;
		double zoom = defaultZoomValue;
		string zoomDisplayFormat = PrintingLocalizer.GetString(PrintingStringId.ZoomDisplayFormat);
		ReadOnlyCollection<double> zoomValues;
		ReadOnlyCollection<ZoomItemBase> zoomModes;
		ZoomItemBase zoomMode;
		IDialogService dialogService;
		ICursorService cursorService;
		internal Dictionary<PrintingSystemCommand, DelegateCommand<object>> Commands {
			get { return commands; }
		}
		protected virtual ReadOnlyCollection<double> ZoomValues {
			get { return zoomValues ?? (zoomValues = new ReadOnlyCollection<double>(new double[] { 10, 25, 50, 75, 100, 150, 200, 400, 800 })); }
		}
		#endregion
		#region ctor
		public PreviewModelBase() {
			dialogService = new NonInteractiveDialogService();
			CreateCommands();
			inputController = new PreviewInputController() { Model = this };
			ZoomMode = GetInitialZoomMode(ZoomModes);
		}
		#endregion
		#region IPreviewModel Members
		public FrameworkElement PageContent {
			get {
				FrameworkElement pageContent = GetPageContent();
				ProcessPageContent(pageContent);
				return pageContent;
			}
		}
		public double PageViewWidth {
			get {
				if(PageContent == null)
					return 0;
				return PageContent.Width * Zoom / 100d;
			}
		}
		public double PageViewHeight {
			get {
				if(PageContent == null)
					return 0;
				return PageContent.Height * Zoom / 100d;
			}
		}
		public double Zoom {
			get { return zoom; }
			set {
				if(value < 0)
					throw new ArgumentOutOfRangeException("Zoom");
				if(zoom != value) {
					SetZoom(value);
					ZoomMode = new ZoomValueItem(value);
					RaisePropertyChanged(() => ZoomMode);
				}
			}
		}
		public abstract int PageCount { get; }
		[EditorBrowsable(EditorBrowsableState.Never)]
		public void SetZoom(double value) {
			if(zoom == value || value <= 0)
				return;
			zoom = value;
			RaisePropertyChanged(() => Zoom);
			RaisePropertyChanged(() => ZoomDisplayText);
			RaisePropertyChanged(() => PageViewWidth);
			RaisePropertyChanged(() => PageViewHeight);
			RaiseZoomCommandsCanExecuteChanged();
		}
		public string ZoomDisplayFormat {
			get { return zoomDisplayFormat; }
			set {
				if(value == null)
					throw new ArgumentNullException("ZoomDisplayFormat");
				if(zoomDisplayFormat == value)
					return;
				zoomDisplayFormat = value;
				RaisePropertyChanged(() => ZoomDisplayFormat);
				RaisePropertyChanged(() => ZoomDisplayText);
			}
		}
		public string ZoomDisplayText {
			get { return string.Format(ZoomDisplayFormat, Zoom); }
		}
		public IEnumerable<ZoomItemBase> ZoomModes {
			get { return zoomModes ?? (zoomModes = CreateZoomModes()); }
		}
		public ZoomItemBase ZoomMode {
			get { return zoomMode; }
			set {
				zoomMode = value;
				if(value is ZoomValueItem) {
					SetZoom(((ZoomValueItem)value).ZoomValue);
				}
				RaisePropertyChanged(() => ZoomMode);
			}
		}
		public abstract bool IsCreating { get; protected set; }
		public bool IsLoading {
			get { return isLoading; }
			protected set {
				if(isLoading == value)
					return;
				isLoading = value;
				RaisePropertyChanged(() => IsLoading);
				OnIsLoadingChanged();
			}
		}
		public bool IsIncorrectPageContent {
			get { return isIncorrectPageContent; }
			set {
				if(isIncorrectPageContent == value)
					return;
				isIncorrectPageContent = value;
				RaisePropertyChanged(() => IsIncorrectPageContent);
			}
		}
		[EditorBrowsable(EditorBrowsableState.Never)]
		public virtual IDialogService DialogService {
			get { return dialogService; }
			set {
				if(value == null)
					throw new ArgumentNullException("DialogService");
				dialogService = value;
			}
		}
		[EditorBrowsable(EditorBrowsableState.Never)]
		public virtual ICursorService CursorService {
			get { return cursorService; }
			set {
				if(value == null)
					throw new ArgumentNullException("CursorService");
				cursorService = value;
			}
		}
		[EditorBrowsable(EditorBrowsableState.Never)]
		public virtual bool UseSimpleScrolling { get; set; }
		public virtual InputController InputController {
			get { return inputController; }
		}
		public ICommand ZoomOutCommand {
			get { return commands[PrintingSystemCommand.ZoomOut]; }
		}
		public ICommand ZoomInCommand {
			get { return commands[PrintingSystemCommand.ZoomIn]; }
		}
		[EditorBrowsable(EditorBrowsableState.Never)]
		public abstract void HandlePreviewMouseLeftButtonUp(MouseButtonEventArgs e, FrameworkElement source);
		[EditorBrowsable(EditorBrowsableState.Never)]
		public abstract void HandlePreviewMouseLeftButtonDown(MouseButtonEventArgs e, FrameworkElement source);
		[EditorBrowsable(EditorBrowsableState.Never)]
		public abstract void HandlePreviewMouseMove(MouseEventArgs e, FrameworkElement source);
#if !SILVERLIGHT
		[EditorBrowsable(EditorBrowsableState.Never)]
		public abstract void HandlePreviewDoubleClick(MouseEventArgs e, FrameworkElement source);
#endif
		#endregion
		#region Methods
		protected virtual void ProcessPageContent(FrameworkElement pageContent) { }
		protected abstract FrameworkElement GetPageContent();
		protected ReadOnlyCollection<ZoomItemBase> CreateZoomModes() {
			List<ZoomItemBase> modes = new List<ZoomItemBase>();
			foreach(double zoomValue in ZoomValues) {
				modes.Add(new ZoomValueItem(zoomValue));
			}
#if !SILVERLIGHT
			modes.Add(new ZoomSeparatorItem());
#endif
			foreach(object fitMode in DevExpress.Utils.EnumExtensions.GetValues(typeof(ZoomFitMode))) {
				modes.Add(new ZoomFitModeItem((ZoomFitMode)fitMode));
			}
			return new ReadOnlyCollection<ZoomItemBase>(modes);
		}
		ZoomItemBase GetInitialZoomMode(IEnumerable<ZoomItemBase> zoomModes) {
			return zoomModes.First(mode => mode is ZoomValueItem && ((ZoomValueItem)mode).ZoomValue == 100);
		}
		void ZoomOut(object parameter) {
			if(!CanZoomOut(parameter))
				throw new InvalidOperationException();
			for(int i = ZoomValues.Count - 1; i >= 0; i--) {
				if(Zoom > ZoomValues[i]) {
					Zoom = ZoomValues[i];
					break;
				}
			}
		}
		bool CanZoomOut(object parameter) {
			return Zoom > ZoomValues[0];
		}
		void ZoomIn(object parameter) {
			if(!CanZoomIn(parameter))
				throw new InvalidOperationException();
			for(int i = 0; i < ZoomValues.Count; i++) {
				if(Zoom < ZoomValues[i]) {
					Zoom = ZoomValues[i];
					break;
				}
			}
		}
		bool CanZoomIn(object parameter) {
			return Zoom < ZoomValues[ZoomValues.Count - 1];
		}
		void CreateCommands() {
			commands.Add(
				PrintingSystemCommand.ZoomOut,
				DelegateCommandFactory.Create<object>(ZoomOut, CanZoomOut, false));
			commands.Add(
				PrintingSystemCommand.ZoomIn,
				DelegateCommandFactory.Create<object>(ZoomIn, CanZoomIn, false));
		}
		protected void SafeCommandHandler(Action handler) {
			try {
				handler();
			} catch(Exception exception) {
				dialogService.ShowError(exception.Message, PrintingLocalizer.GetString(PrintingStringId.Error));
			}
		}
		protected virtual void OnIsLoadingChanged() {
		}
		protected void RaiseZoomCommandsCanExecuteChanged() {
			commands[PrintingSystemCommand.ZoomOut].RaiseCanExecuteChanged();
			commands[PrintingSystemCommand.ZoomIn].RaiseCanExecuteChanged();
		}
		protected void RaiseAllCommandsCanExecuteChanged() {
			foreach(var command in commands.Values) {
				command.RaiseCanExecuteChanged();
			}
		}
		protected void RaiseOperationError(Exception error, EventHandler<FaultEventArgs> errorEvent) {
			var showError = true;
			if(errorEvent != null) {
				var args = new FaultEventArgs(error);
				errorEvent(this, args);
				showError = !args.Handled;
			}
			if(showError)
				ShowError(error.Message);
			return;
		}
		protected void ShowError(string message) {
			DialogService.ShowError(message, PrintingLocalizer.GetString(PrintingStringId.Error));
		}
		#endregion
		#region INotifyPropertyChanged
		public event PropertyChangedEventHandler PropertyChanged;
		protected void RaiseAllPropertiesChanged() {
			if(PropertyChanged != null) {
				PropertyChanged(this, new PropertyChangedEventArgs(null));
			}
		}
		protected void RaisePropertyChanged<T>(Expression<Func<T>> property) {
			PropertyExtensions.RaisePropertyChanged(this, PropertyChanged, property);
		}
		#endregion
	}
}
