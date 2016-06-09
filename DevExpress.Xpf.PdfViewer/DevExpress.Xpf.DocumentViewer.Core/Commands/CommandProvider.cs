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

using DevExpress.Mvvm;
using DevExpress.Mvvm.Native;
using DevExpress.Xpf.Bars;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Core.Native;
using System.Collections.ObjectModel;
using System.Reflection;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System;
using DevExpress.Mvvm.UI;
using DevExpress.Xpf.Editors;
namespace DevExpress.Xpf.DocumentViewer {
	public class CommandProvider : ViewModelBase {
		ObservableCollection<IControllerAction> actions;
		ObservableCollection<IControllerAction> ribbonActions;
		DocumentViewerControl documentViewer;
		public ICommand OpenDocumentCommand { get; private set; }
		public ICommand CloseDocumentCommand { get; private set; }
		public ICommand NextPageCommand { get; private set; }
		public ICommand PreviousPageCommand { get; private set; }
		public ICommand PaginationCommand { get; private set; }
		public ICommand ZoomInCommand { get; private set; }
		public ICommand ZoomOutCommand { get; private set; }
		public ICommand ZoomCommand { get; private set; }
		public ICommand ClockwiseRotateCommand { get; private set; }
		public ICommand CounterClockwiseRotateCommand { get; private set; }
		public ICommand ScrollCommand { get; private set; }
		public ICommand PreviousViewCommand { get; private set; }
		public ICommand NextViewCommand { get; private set; }
		public ICommand ShowFindTextCommand { get; private set; }
		public ICommand FindNextTextCommand { get; private set; }
		public ICommand FindPreviousTextCommand { get; private set; }
		public ICommand NavigateCommand { get; protected set; }
		public ObservableCollection<IControllerAction> Actions {
			get { return actions ?? (actions = new ObservableCollection<IControllerAction>()); }
			set { SetProperty(ref actions, value, () => Actions); }
		}
		public ObservableCollection<IControllerAction> RibbonActions {
			get { return ribbonActions ?? (ribbonActions = new ObservableCollection<IControllerAction>()); }
			set { SetProperty(ref ribbonActions, value, () => RibbonActions); }
		}
		public CommandProvider() { }
		protected internal DocumentViewerControl DocumentViewer {
			get { return documentViewer; }
			set {
				if (Equals(documentViewer, value))
					return;
				UnsubscribeFromEvents();
				documentViewer = value;
				InitializeElementsInternal();
				SubscribeToEvents();
			}
		}
		protected virtual ICommand OpenDocumentCommandInternal {
			get { return DocumentViewer.With(x => x.OpenDocumentCommand); }
		}
		protected virtual ICommand CloseDocumentCommandInternal {
			get { return DocumentViewer.With(x => x.CloseDocumentCommand); }
		}
		protected virtual ICommand PreviousPageCommandInternal {
			get { return DocumentViewer.With(x => x.PreviousPageCommand); }
		}
		protected virtual ICommand NextPageCommandInternal {
			get { return DocumentViewer.With(x => x.NextPageCommand); }
		}
		protected virtual ICommand SetPageNumberCommandInternal {
			get { return DocumentViewer.With(x => x.SetPageNumberCommand); }
		}
		protected virtual ICommand ZoomInCommandInternal {
			get { return DocumentViewer.With(x => x.ZoomInCommand); }
		}
		protected virtual ICommand ZoomOutCommandInternal {
			get { return DocumentViewer.With(x => x.ZoomOutCommand); }
		}
		protected virtual ICommand SetZoomModeCommandInternal {
			get { return DocumentViewer.With(x => x.SetZoomModeCommand); }
		}
		protected virtual ICommand SetZoomFactorCommandInternal {
			get { return DocumentViewer.With(x => x.SetZoomFactorCommand); }
		}
		protected virtual ICommand ClockwiseRotateCommandInternal {
			get { return DocumentViewer.With(x => x.ClockwiseRotateCommand); }
		}
		protected virtual ICommand CounterClockwiseRotateCommandInternal {
			get { return DocumentViewer.With(x => x.CounterClockwiseRotateCommand); }
		}
		protected virtual ICommand ScrollCommandInternal {
			get { return DocumentViewer.With(x => x.ScrollCommand); }
		}
		protected virtual ICommand PreviousViewCommandInternal {
			get { return DocumentViewer.With(x => x.PreviousViewCommand); }
		}
		protected virtual ICommand NextViewCommandInternal {
			get { return DocumentViewer.With(x => x.NextViewCommand); }
		}
		protected  virtual ICommand ShowFindTextCommandInternal {
			get { return DocumentViewer.With(x => x.ShowFindTextCommand); }
		}
		protected virtual ICommand FindTextCommandInternal {
			get { return DocumentViewer.With(x => x.FindTextCommand); }
		}
		protected internal virtual ICommand NavigateCommandInternal {
			get { return DocumentViewer.With(x => x.NavigateCommand); }
		}
		void InitializeElementsInternal() {
			string dllName = Assembly.GetExecutingAssembly().GetName().Name;
			OpenDocumentCommand = new CommandButton {
				Caption = DocumentViewerLocalizer.GetString(DocumentViewerStringId.CommandOpenCaption),
				Hint = DocumentViewerLocalizer.GetString(DocumentViewerStringId.CommandOpenDescription),
				Command = new CommandWrapper(() => OpenDocumentCommandInternal),
				Group = DocumentViewerLocalizer.GetString(DocumentViewerStringId.FileRibbonGroupCaption),
				SmallGlyph = UriHelper.GetUri(dllName, @"\Images\Open_16x16.png"),
				LargeGlyph = UriHelper.GetUri(dllName, @"\Images\Open_32x32.png"),
			};
			CloseDocumentCommand = new CommandButton {
				Caption = DocumentViewerLocalizer.GetString(DocumentViewerStringId.CommandCloseCaption),
				Hint = DocumentViewerLocalizer.GetString(DocumentViewerStringId.CommandCloseDescription),
				Command = new CommandWrapper(() => CloseDocumentCommandInternal),
				Group = DocumentViewerLocalizer.GetString(DocumentViewerStringId.FileRibbonGroupCaption),
				SmallGlyph = UriHelper.GetUri(dllName, @"\Images\Close_16x16.png"),
				LargeGlyph = UriHelper.GetUri(dllName, @"\Images\Close_32x32.png"),
			};
			PreviousPageCommand = new CommandButton {
				Caption = DocumentViewerLocalizer.GetString(DocumentViewerStringId.CommandPreviousPageCaption),
				Hint = DocumentViewerLocalizer.GetString(DocumentViewerStringId.CommandPreviousPageDescription),
				Command = new CommandWrapper(() => PreviousPageCommandInternal),
				Group = DocumentViewerLocalizer.GetString(DocumentViewerStringId.NavigationRibbonGroupCaption),
				SmallGlyph = UriHelper.GetUri(dllName, @"\Images\Prev_16x16.png"),
				LargeGlyph = UriHelper.GetUri(dllName, @"\Images\Prev_32x32.png"),
			};
			NextPageCommand = new CommandButton {
				Caption = DocumentViewerLocalizer.GetString(DocumentViewerStringId.CommandNextPageCaption),
				Hint = DocumentViewerLocalizer.GetString(DocumentViewerStringId.CommandNextPageDescription),
				Command = new CommandWrapper(() => NextPageCommandInternal),
				Group = DocumentViewerLocalizer.GetString(DocumentViewerStringId.NavigationRibbonGroupCaption),
				SmallGlyph = UriHelper.GetUri(dllName, @"\Images\Next_16x16.png"),
				LargeGlyph = UriHelper.GetUri(dllName, @"\Images\Next_32x32.png"),
			};
			PaginationCommand = new CommandPagination {
				PageCount = DocumentViewer.Return(x => x.PageCount, () => 0),
				CurrentPageNumber = DocumentViewer.Return(x => x.CurrentPageNumber, () => 0),
				Command = new CommandWrapper(() => SetPageNumberCommandInternal)
			};
			ZoomInCommand = new CommandButton {
				Caption = DocumentViewerLocalizer.GetString(DocumentViewerStringId.CommandZoomInCaption),
				Hint = DocumentViewerLocalizer.GetString(DocumentViewerStringId.CommandZoomInDescription),
				Command = new CommandWrapper(() => ZoomInCommandInternal),
				Group = DocumentViewerLocalizer.GetString(DocumentViewerStringId.ZoomRibbonGroupCaption),
				SmallGlyph = UriHelper.GetUri(dllName, @"\Images\ZoomIn_16x16.png"),
				LargeGlyph = UriHelper.GetUri(dllName, @"\Images\ZoomIn_32x32.png"),
			};
			ZoomOutCommand = new CommandButton {
				Caption = DocumentViewerLocalizer.GetString(DocumentViewerStringId.CommandZoomOutCaption),
				Hint = DocumentViewerLocalizer.GetString(DocumentViewerStringId.CommandZoomOutDescription),
				Command = new CommandWrapper(() => ZoomOutCommandInternal),
				Group = DocumentViewerLocalizer.GetString(DocumentViewerStringId.ZoomRibbonGroupCaption),
				SmallGlyph = UriHelper.GetUri(dllName, @"\Images\ZoomOut_16x16.png"),
				LargeGlyph = UriHelper.GetUri(dllName, @"\Images\ZoomOut_32x32.png"),
			};
			ZoomCommand = CreateZoomModeAndZoomFactorItem(dllName);
			UpdateZoomCommand();
			ClockwiseRotateCommand = new CommandButton {
				Caption = DocumentViewerLocalizer.GetString(DocumentViewerStringId.CommandClockwiseRotateCaption),
				Command = new CommandWrapper(() => ClockwiseRotateCommandInternal),
				Group = DocumentViewerLocalizer.GetString(DocumentViewerStringId.RotateRibbonGroupCaption),
				SmallGlyph = UriHelper.GetUri(dllName, @"\Images\RotateClockwise_16x16.png"),
				LargeGlyph = UriHelper.GetUri(dllName, @"\Images\RotateClockwise_32x32.png")
			};
			CounterClockwiseRotateCommand = new CommandButton {
				Caption = DocumentViewerLocalizer.GetString(DocumentViewerStringId.CommandCounterClockwiseRotateCaption),
				Command = new CommandWrapper(() => CounterClockwiseRotateCommandInternal),
				Group = DocumentViewerLocalizer.GetString(DocumentViewerStringId.RotateRibbonGroupCaption),
				SmallGlyph = UriHelper.GetUri(dllName, @"\Images\RotateCounterclockwise_16x16.png"),
				LargeGlyph = UriHelper.GetUri(dllName, @"\Images\RotateCounterclockwise_32x32.png")
			};
			ScrollCommand = new CommandWrapper(() => ScrollCommandInternal);
			PreviousViewCommand = new CommandButton {
				Caption = DocumentViewerLocalizer.GetString(DocumentViewerStringId.CommandPreviousViewCaption),
				Command = new CommandWrapper(() => PreviousViewCommandInternal),
				Group = DocumentViewerLocalizer.GetString(DocumentViewerStringId.NavigationRibbonGroupCaption),
				SmallGlyph = UriHelper.GetUri(dllName, @"\Images\PreviousView_16x16.png"),
				LargeGlyph = UriHelper.GetUri(dllName, @"\Images\PreviousView_32x32.png")
			};
			NextViewCommand = new CommandButton {
				Caption = DocumentViewerLocalizer.GetString(DocumentViewerStringId.CommandNextViewCaption),
				Command = new CommandWrapper(() => NextViewCommandInternal),
				Group = DocumentViewerLocalizer.GetString(DocumentViewerStringId.NavigationRibbonGroupCaption),
				SmallGlyph = UriHelper.GetUri(dllName, @"\Images\NextView_16x16.png"),
				LargeGlyph = UriHelper.GetUri(dllName, @"\Images\NextView_32x32.png")
			};
			ShowFindTextCommand = new CommandButton {
				Caption = DocumentViewerLocalizer.GetString(DocumentViewerStringId.MessageShowFindTextCaption),
				Hint = DocumentViewerLocalizer.GetString(DocumentViewerStringId.MessageShowFindTextHintCaption),
				Command = new CommandWrapper(() => ShowFindTextCommandInternal),
				Group = DocumentViewerLocalizer.GetString(DocumentViewerStringId.NavigationRibbonGroupCaption),
				SmallGlyph = UriHelper.GetUri(dllName, @"\Images\Find_16x16.png"),
				LargeGlyph = UriHelper.GetUri(dllName, @"\Images\Find_32x32.png"),
			};
			FindNextTextCommand = new CommandButton { Command = new CommandWrapper(() => FindTextCommandInternal) };
			FindPreviousTextCommand = new CommandButton { Command = new CommandWrapper(() => FindTextCommandInternal) };
			InitializeElements();
		}
		protected virtual ICommand CreateZoomModeAndZoomFactorItem(string dllName) {
			var zoomModeAndFactorsItems = new ObservableCollection<CommandToggleButton>();
			var setZoomModeAndFactor = new CommandCheckItems {
				Caption = DocumentViewerLocalizer.GetString(DocumentViewerStringId.CommandZoomCaption),
				Hint = DocumentViewerLocalizer.GetString(DocumentViewerStringId.CommandZoomDescription),
				Group = DocumentViewerLocalizer.GetString(DocumentViewerStringId.ZoomRibbonGroupCaption),
				Command = DelegateCommandFactory.Create(() => { }, () => zoomModeAndFactorsItems.Any(x => x.CanExecute(null))),
				Items = zoomModeAndFactorsItems,
				SmallGlyph = UriHelper.GetUri(dllName, @"\Images\Zoom_16x16.png"),
				LargeGlyph = UriHelper.GetUri(dllName, @"\Images\Zoom_32x32.png"),
			};
			var setZoomFactorCommand = DelegateCommandFactory.Create<double>(x => {
				SetZoomFactorCommandInternal.Execute(x);
				UpdateZoomCommand();
			}, x => SetZoomFactorCommandInternal.CanExecute(x));
			var setZoomModeCommand = DelegateCommandFactory.Create<ZoomMode>(x => {
				SetZoomModeCommandInternal.Execute(x);
				UpdateZoomCommand();
			}, x => SetZoomModeCommandInternal.CanExecute(x));
			zoomModeAndFactorsItems.Add(new CommandSetZoomFactorAndModeItem {
				Caption = DocumentViewerLocalizer.GetString(DocumentViewerStringId.CommandZoom25Caption),
				Command = new CommandWrapper(() => setZoomFactorCommand),
				ZoomFactor = 0.25,
				GroupIndex = 1
			});
			zoomModeAndFactorsItems.Add(new CommandSetZoomFactorAndModeItem {
				Caption = DocumentViewerLocalizer.GetString(DocumentViewerStringId.CommandZoom50Caption),
				Command = new CommandWrapper(() => setZoomFactorCommand),
				ZoomFactor = 0.5,
				GroupIndex = 1
			});
			zoomModeAndFactorsItems.Add(new CommandSetZoomFactorAndModeItem {
				Caption = DocumentViewerLocalizer.GetString(DocumentViewerStringId.CommandZoom75Caption),
				Command = new CommandWrapper(() => setZoomFactorCommand),
				ZoomFactor = 0.75,
				GroupIndex = 1
			});
			zoomModeAndFactorsItems.Add(new CommandSetZoomFactorAndModeItem {
				Caption = DocumentViewerLocalizer.GetString(DocumentViewerStringId.CommandZoom100Caption),
				Command = new CommandWrapper(() => setZoomFactorCommand),
				ZoomFactor = 1d,
				GroupIndex = 1
			});
			zoomModeAndFactorsItems.Add(new CommandSetZoomFactorAndModeItem {
				Caption = DocumentViewerLocalizer.GetString(DocumentViewerStringId.CommandZoom125Caption),
				Command = new CommandWrapper(() => setZoomFactorCommand),
				ZoomFactor = 1.25,
				GroupIndex = 1
			});
			zoomModeAndFactorsItems.Add(new CommandSetZoomFactorAndModeItem {
				Caption = DocumentViewerLocalizer.GetString(DocumentViewerStringId.CommandZoom150Caption),
				Command = new CommandWrapper(() => setZoomFactorCommand),
				ZoomFactor = 1.5,
				GroupIndex = 1
			});
			zoomModeAndFactorsItems.Add(new CommandSetZoomFactorAndModeItem {
				Caption = DocumentViewerLocalizer.GetString(DocumentViewerStringId.CommandZoom200Caption),
				Command = new CommandWrapper(() => setZoomFactorCommand),
				ZoomFactor = 2d,
				GroupIndex = 1
			});
			zoomModeAndFactorsItems.Add(new CommandSetZoomFactorAndModeItem {
				Caption = DocumentViewerLocalizer.GetString(DocumentViewerStringId.CommandZoom400Caption),
				Command = new CommandWrapper(() => setZoomFactorCommand),
				ZoomFactor = 4d,
				GroupIndex = 1
			});
			zoomModeAndFactorsItems.Add(new CommandSetZoomFactorAndModeItem {
				Caption = DocumentViewerLocalizer.GetString(DocumentViewerStringId.CommandZoom500Caption),
				Command = new CommandWrapper(() => setZoomFactorCommand),
				ZoomFactor = 5d,
				GroupIndex = 1
			});
			zoomModeAndFactorsItems.Add(new CommandSetZoomFactorAndModeItem { IsSeparator = true });
			zoomModeAndFactorsItems.Add(new CommandSetZoomFactorAndModeItem {
				Caption = DocumentViewerLocalizer.GetString(DocumentViewerStringId.CommandSetActualSizeZoomModeCaption),
				Command = new CommandWrapper(() => setZoomModeCommand),
				ZoomMode = ZoomMode.ActualSize,
				KeyGesture = new KeyGesture(Key.D1, ModifierKeys.Control),
				GroupIndex = 2
			});
			zoomModeAndFactorsItems.Add(new CommandSetZoomFactorAndModeItem {
				Caption = DocumentViewerLocalizer.GetString(DocumentViewerStringId.CommandSetPageLevelZoomModeCaption),
				Command = new CommandWrapper(() => setZoomModeCommand),
				ZoomMode = ZoomMode.PageLevel,
				KeyGesture = new KeyGesture(Key.D0, ModifierKeys.Control),
				GroupIndex = 2
			});
			zoomModeAndFactorsItems.Add(new CommandSetZoomFactorAndModeItem {
				Caption = DocumentViewerLocalizer.GetString(DocumentViewerStringId.CommandSetFitWidthZoomModeCaption),
				Command = new CommandWrapper(() => setZoomModeCommand),
				ZoomMode = ZoomMode.FitToWidth,
				KeyGesture = new KeyGesture(Key.D2, ModifierKeys.Control),
				GroupIndex = 2
			});
			zoomModeAndFactorsItems.Add(new CommandSetZoomFactorAndModeItem {
				Caption = DocumentViewerLocalizer.GetString(DocumentViewerStringId.CommandSetFitVisibleZoomModeCaption),
				Command = new CommandWrapper(() => setZoomModeCommand),
				ZoomMode = ZoomMode.FitToVisible,
				KeyGesture = new KeyGesture(Key.D3, ModifierKeys.Control),
				GroupIndex = 2
			});
			return setZoomModeAndFactor;
		}
		void UpdateZoomCommand() {
			var zoomCommand = ZoomCommand as CommandCheckItems;
			zoomCommand.Do(x => x.UpdateCheckState(UpdateZoomFactorCheckState));
		}
		protected void UpdatePagination() {
			if (DocumentViewer == null)
				return;
			((CommandPagination)PaginationCommand).CurrentPageNumber = DocumentViewer.CurrentPageNumber;
			((CommandPagination)PaginationCommand).PageCount = DocumentViewer.PageCount;
		}
		protected virtual void OnDocumentChanged(object sender, RoutedEventArgs e) {
			UpdatePagination();
		}
		protected virtual void OnCurrentPageNumberChanged(object sender, RoutedEventArgs e) {
			UpdatePagination();
		}
		protected virtual void OnZoomChanged(object sender, RoutedEventArgs e) {
			UpdateZoomCommand();
		}
		protected virtual bool UpdateZoomFactorCheckState(CommandToggleButton item) {
			if (DocumentViewer == null)
				return false;
			return (item as CommandSetZoomFactorAndModeItem).Return(x => (DocumentViewer.ZoomMode != ZoomMode.Custom && x.ZoomMode == DocumentViewer.ZoomMode) || x.ZoomFactor.AreClose(DocumentViewer.ZoomFactor), () => false);
		}
		protected virtual void UnsubscribeFromEvents() {
			DocumentViewer.Do(x => x.ZoomChanged -= OnZoomChanged);
			DocumentViewer.Do(x => x.CurrentPageNumberChanged -= OnCurrentPageNumberChanged);
			DocumentViewer.Do(x => x.DocumentChanged -= OnDocumentChanged);
		}
		protected virtual void SubscribeToEvents() {
			DocumentViewer.Do(x => x.ZoomChanged += OnZoomChanged);
			DocumentViewer.Do(x => x.CurrentPageNumberChanged += OnCurrentPageNumberChanged);
			DocumentViewer.Do(x => x.DocumentChanged += OnDocumentChanged);
		}
		protected virtual void InitializeElements() {
			NavigateCommand = new CommandWrapper(() => NavigateCommandInternal);
		}
	}
	public static class UriHelper {
		public static Uri GetUri(string dllName, string relativeFilePath) {
			return new Uri(string.Format("/{0};component/{1}", dllName, relativeFilePath), UriKind.RelativeOrAbsolute);
		}
	}
	public class TextEditEditValueChangedConverter : EventArgsConverterBase<EditValueChangedEventArgs> {
		protected override object Convert(object sender, EditValueChangedEventArgs args) {
			return args.NewValue;
		}
	}
}
