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
using System.Collections.ObjectModel;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Linq;
using DevExpress.Xpf.Editors;
using DevExpress.Xpf.Editors.Helpers;
using DevExpress.Mvvm;
using DevExpress.Mvvm.Native;
using DevExpress.Mvvm.UI;
using DevExpress.Xpf.PdfViewer.Internal;
using DevExpress.Xpf.PdfViewer.Themes;
using DevExpress.Xpf.Bars;
using DevExpress.Mvvm.UI.Interactivity;
using DevExpress.Xpf.Utils;
using System.Collections.Specialized;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Ribbon;
using DevExpress.Xpf.DocumentViewer;
using DevExpress.Pdf;
namespace DevExpress.Xpf.PdfViewer {
	public class PdfCommandProvider : CommandProvider {
		public ICommand OpenDocumentSplitCommand { get; protected set; }
		public ICommand OpenDocumentFromWebCommand { get; protected set; }
		public ICommand PrintDocumentCommand { get; protected set; }
		public ICommand ShowPropertiesCommand { get; protected set; }
		public ICommand HandToolCommand { get; protected set; }
		public ICommand SelectToolCommand { get; protected set; }
		public ICommand MarqueeZoomCommand { get; protected set; }
		public ICommand SelectAllCommand { get; protected set; }
		public ICommand SaveAsCommand { get; protected set; }
		public ICommand CopyCommand { get; protected set; }
		public ICommand SelectionCommand { get; protected set; }
		public ICommand UnselectAllCommand { get; protected set; }
		public ICommand ImportFormDataCommand { get; protected set; }
		public ICommand ExportFormDataCommand { get; protected set; }
		public ICommand PageLayoutCommand { get; protected set; }
		public ICommand OpenAttachmentCommand { get; protected set; }
		public ICommand SaveAttachmentCommand { get; protected set; }
		protected internal virtual ICommand PrintDocumentCommandInternal {
			get { return PdfViewer.With(x => x.PrintDocumentCommand); }
		}
		protected internal virtual ICommand ShowPropertiesCommandInternal {
			get { return PdfViewer.With(x => x.ShowPropertiesCommand); }
		}
		protected internal virtual ICommand SetCursorModeCommandInternal {
			get { return PdfViewer.With(x => x.SetCursorModeCommand); }
		}
		protected internal virtual ICommand SelectAllCommandInternal {
			get { return PdfViewer.With(x => x.SelectAllCommand); }
		}
		protected internal virtual ICommand SaveAsCommandInternal {
			get { return PdfViewer.With(x => x.SaveAsCommand); }
		}
		protected internal virtual ICommand CopyCommandInternal {
			get { return PdfViewer.With(x => x.CopyCommand); }
		}
		protected internal virtual ICommand SelectionCommandInternal {
			get { return PdfViewer.With(x => x.SelectionCommand); }
		}
		protected internal virtual ICommand OpenDocumentFromWebCommandInternal {
			get { return PdfViewer.With(x => x.OpenDocumentFromWebCommand); }
		}
		protected internal virtual ICommand UnselectAllCommandInternal {
			get { return PdfViewer.With(x => x.UnselectAllCommand); }
		}
		protected internal virtual ICommand ImportFormDataCommandInternal {
			get { return PdfViewer.With(x => x.ImportFormDataCommand); }
		}
		protected internal virtual ICommand ExportFormDataCommandInternal {
			get { return PdfViewer.With(x => x.ExportFormDataCommand); }
		}
		protected internal virtual ICommand SetPageLayoutCommandInternal {
			get { return PdfViewer.With(x => x.SetPageLayoutCommand); }
		}
		protected internal virtual ICommand ShowCoverPageCommandInternal {
			get { return PdfViewer.With(x => x.ShowCoverPageCommand); }
		}
		protected internal virtual ICommand OpenAttachmentCommandInternal {
			get { return PdfViewer.With(x => x.OpenAttachmentCommand); }
		}
		protected internal virtual ICommand SaveAttachmentCommandInternal {
			get { return PdfViewer.With(x => x.SaveAttachmentCommand); }
		}
		PdfViewerControl PdfViewer { get { return DocumentViewer as PdfViewerControl; } }
		protected override void InitializeElements() {
			base.InitializeElements();
			string dllName = Assembly.GetExecutingAssembly().GetName().Name;
			PrintDocumentCommand = new CommandButton {
				Caption = PdfViewerLocalizer.GetString(PdfViewerStringId.CommandPrintFileCaption),
				Hint = PdfViewerLocalizer.GetString(PdfViewerStringId.CommandPrintFileDescription),
				Command = new CommandWrapper(() => PrintDocumentCommandInternal),
				Group = PdfViewerLocalizer.GetString(PdfViewerStringId.FileRibbonGroupCaption),
				SmallGlyph = UriHelper.GetUri(dllName, @"\Images\Print_16x16.png"),
				LargeGlyph = UriHelper.GetUri(dllName, @"\Images\Print_32x32.png"),
			};
			ShowPropertiesCommand = new CommandButton {
				Caption = PdfViewerLocalizer.GetString(PdfViewerStringId.CommandShowDocumentProperties),
				Command = new CommandWrapper(() => ShowPropertiesCommandInternal),
				Group = PdfViewerLocalizer.GetString(PdfViewerStringId.FileRibbonGroupCaption)
			};
			HandToolCommand = new PdfSetCursorModeItem {
				Caption = PdfViewerLocalizer.GetString(PdfViewerStringId.CommandCursorModeHandToolCaption),
				Command = new CommandWrapper(() => SetCursorModeCommandInternal),
				Group = PdfViewerLocalizer.GetString(PdfViewerStringId.CommandViewCursorModeListGroupCaption),
				CommandValue = CursorModeType.HandTool,
				SmallGlyph = UriHelper.GetUri(dllName, @"\Images\HandTool_16x16.png"),
				LargeGlyph = UriHelper.GetUri(dllName, @"\Images\HandTool_32x32.png")
			};
			SelectToolCommand = new PdfSetCursorModeItem {
				Caption = PdfViewerLocalizer.GetString(PdfViewerStringId.CommandCursorModeSelectToolCaption),
				Command = new CommandWrapper(() => SetCursorModeCommandInternal),
				Group = PdfViewerLocalizer.GetString(PdfViewerStringId.CommandViewCursorModeListGroupCaption),
				CommandValue = CursorModeType.SelectTool,
				SmallGlyph = UriHelper.GetUri(dllName, @"\Images\SelectTool_16x16.png"),
				LargeGlyph = UriHelper.GetUri(dllName, @"\Images\SelectTool_32x32.png")
			};
			MarqueeZoomCommand = new PdfSetCursorModeItem {
				Caption = PdfViewerLocalizer.GetString(PdfViewerStringId.CommandCursorModeMarqueeZoomCaption),
				Command = new CommandWrapper(() => SetCursorModeCommandInternal),
				Group = PdfViewerLocalizer.GetString(PdfViewerStringId.CommandViewCursorModeListGroupCaption),
				CommandValue = CursorModeType.MarqueeZoom,
				SmallGlyph = UriHelper.GetUri(dllName, @"\Images\MarqueeZoom_16x16.png"),
				LargeGlyph = UriHelper.GetUri(dllName, @"\Images\MarqueeZoom_32x32.png")
			};
			UpdateCursorModeCheckState();
			SelectAllCommand = new CommandButton {
				Caption = PdfViewerLocalizer.GetString(PdfViewerStringId.CommandSelectAllCaption),
				Command = new CommandWrapper(() => SelectAllCommandInternal),
				SmallGlyph = UriHelper.GetUri(dllName, @"\Images\SelectAll_16x16.png"),
				LargeGlyph = UriHelper.GetUri(dllName, @"\Images\SelectAll_32x32.png")
			};
			SaveAsCommand = new CommandButton {
				Caption = PdfViewerLocalizer.GetString(PdfViewerStringId.CommandSaveAsCaption),
				Command = new CommandWrapper(() => SaveAsCommandInternal),
				Group = PdfViewerLocalizer.GetString(PdfViewerStringId.FileRibbonGroupCaption),
				SmallGlyph = UriHelper.GetUri(dllName, @"\Images\SaveAs_16x16.png"),
				LargeGlyph = UriHelper.GetUri(dllName, @"\Images\SaveAs_32x32.png")
			};
			CopyCommand = new CommandButton {
				Caption = PdfViewerLocalizer.GetString(PdfViewerStringId.CommandCopyCaption),
				Command = new CommandWrapper(() => CopyCommandInternal),
				SmallGlyph = UriHelper.GetUri(dllName, @"\Images\Copy_16x16.png"),
				LargeGlyph = UriHelper.GetUri(dllName, @"\Images\Copy_32x32.png")
			};
			OpenDocumentSplitCommand = new PdfOpenDocumentSplitItem {
				Caption = PdfViewerLocalizer.GetString(PdfViewerStringId.CommandOpenFileCaption),
				Hint = PdfViewerLocalizer.GetString(PdfViewerStringId.CommandOpenFileDescription),
				Command = new CommandWrapper(() => OpenDocumentCommandInternal),
				Group = PdfViewerLocalizer.GetString(PdfViewerStringId.FileRibbonGroupCaption),
				SmallGlyph = UriHelper.GetUri(dllName, @"\Images\Open_16x16.png"),
				LargeGlyph = UriHelper.GetUri(dllName, @"\Images\Open_32x32.png"),
				RecentFiles = PdfViewer.With(x => x.RecentFiles)
			};
			OpenDocumentFromWebCommand = new PdfSplitItem {
				Caption = PdfViewerLocalizer.GetString(PdfViewerStringId.CommandOpenFileCaption),
				Hint = PdfViewerLocalizer.GetString(PdfViewerStringId.CommandOpenFileDescription),
				Command = new CommandWrapper(() => OpenDocumentCommandInternal),
				Group = PdfViewerLocalizer.GetString(PdfViewerStringId.FileRibbonGroupCaption),
				SmallGlyph = UriHelper.GetUri(dllName, @"\Images\Open_16x16.png"),
				LargeGlyph = UriHelper.GetUri(dllName, @"\Images\Open_32x32.png"),
				Commands = new ObservableCollection<ICommand>() {
					OpenDocumentCommand,
					new CommandButton {
						Caption = PdfViewerLocalizer.GetString(PdfViewerStringId.CommandOpenFileFromWebCaption),
						Command = new CommandWrapper(() => OpenDocumentFromWebCommandInternal),
						SmallGlyph = UriHelper.GetUri(dllName, @"\Images\OpenHyperlink_16x16.png"),
						LargeGlyph = UriHelper.GetUri(dllName, @"\Images\OpenHyperlink_32x32.png")
					}
				}
			};
			SelectionCommand = new CommandWrapper(() => SelectionCommandInternal);
			UnselectAllCommand = new CommandWrapper(() => UnselectAllCommandInternal);
			ImportFormDataCommand = new CommandButton {
				Caption = PdfViewerLocalizer.GetString(PdfViewerStringId.CommandImportCaption),
				Hint = PdfViewerLocalizer.GetString(PdfViewerStringId.CommandImportDescription),
				Command = new CommandWrapper(() => ImportFormDataCommandInternal),
				Group = PdfViewerLocalizer.GetString(PdfViewerStringId.FormDataRibbonGroupCaption),
				SmallGlyph = UriHelper.GetUri(dllName, @"\Images\Import_16x16.png"),
				LargeGlyph = UriHelper.GetUri(dllName, @"\Images\Import_32x32.png")
			};
			ExportFormDataCommand = new CommandButton {
				Caption = PdfViewerLocalizer.GetString(PdfViewerStringId.CommandExportCaption),
				Hint = PdfViewerLocalizer.GetString(PdfViewerStringId.CommandExportDescription),
				Command = new CommandWrapper(() => ExportFormDataCommandInternal),
				Group = PdfViewerLocalizer.GetString(PdfViewerStringId.FormDataRibbonGroupCaption),
				SmallGlyph = UriHelper.GetUri(dllName, @"\Images\Export_16x16.png"),
				LargeGlyph = UriHelper.GetUri(dllName, @"\Images\Export_32x32.png")
			};
			PageLayoutCommand = CreatePageLayoutCommand(dllName);
			UpdatePageLayoutCommand();
			OpenAttachmentCommand = new CommandWrapper(() => OpenAttachmentCommandInternal);
			SaveAttachmentCommand = new CommandWrapper(() => SaveAttachmentCommandInternal);
		}
		protected virtual ICommand CreatePageLayoutCommand(string dllName) {
			var pageLayoutItems = new ObservableCollection<CommandToggleButton>();
			var pageLayoutCommand = new CommandCheckItems {
				Caption = PdfViewerLocalizer.GetString(PdfViewerStringId.CommandPageLayoutCaption),
				Hint = PdfViewerLocalizer.GetString(PdfViewerStringId.CommandPageLayoutDescription),
				Command = DelegateCommandFactory.Create(() => { }, () => pageLayoutItems.Any(x => x.CanExecute(null))),
				Items = pageLayoutItems,
				SmallGlyph = UriHelper.GetUri(dllName, @"\Images\Copy_16x16.png"),
				LargeGlyph = UriHelper.GetUri(dllName, @"\Images\Copy_32x32.png"),
			};
			pageLayoutItems.Add(new CommandSetPageLayout {
				Caption = PdfViewerLocalizer.GetString(PdfViewerStringId.CommandSinglePageView),
				Command = new CommandWrapper(() => SetPageLayoutCommandInternal),
				PageLayout = PdfPageLayout.SinglePage,
				GroupIndex = 3,
				SmallGlyph = UriHelper.GetUri(dllName, @"\Images\SinglePage_16x16.png"),
				LargeGlyph = UriHelper.GetUri(dllName, @"\Images\SinglePage_32x32.png"),
			});
			pageLayoutItems.Add(new CommandSetPageLayout {
				Caption = PdfViewerLocalizer.GetString(PdfViewerStringId.CommandOneColumnView),
				Command = new CommandWrapper(() => SetPageLayoutCommandInternal),
				PageLayout = PdfPageLayout.OneColumn,
				GroupIndex = 3,
				SmallGlyph = UriHelper.GetUri(dllName, @"\Images\SinglePageEnabledScrolling_16x16.png"),
				LargeGlyph = UriHelper.GetUri(dllName, @"\Images\SinglePageEnabledScrolling_32x32.png"),
			});
			pageLayoutItems.Add(new CommandSetPageLayout {
				Caption = PdfViewerLocalizer.GetString(PdfViewerStringId.CommandTwoPageView),
				Command = new CommandWrapper(() => SetPageLayoutCommandInternal),
				PageLayout = PdfPageLayout.TwoPageLeft,
				GroupIndex = 3,
				SmallGlyph = UriHelper.GetUri(dllName, @"\Images\TwoPages_16x16.png"),
				LargeGlyph = UriHelper.GetUri(dllName, @"\Images\TwoPages_32x32.png"),
			});
			pageLayoutItems.Add(new CommandSetPageLayout {
				Caption = PdfViewerLocalizer.GetString(PdfViewerStringId.CommandTwoColumnView),
				Command = new CommandWrapper(() => SetPageLayoutCommandInternal),
				PageLayout = PdfPageLayout.TwoColumnLeft,
				GroupIndex = 3,
				SmallGlyph = UriHelper.GetUri(dllName, @"\Images\TwoPagesEnabledScrolling_16x16.png"),
				LargeGlyph = UriHelper.GetUri(dllName, @"\Images\TwoPagesEnabledScrolling_32x32.png"),
			});
			pageLayoutItems.Add(new CommandSetPageLayout { IsSeparator = true });
			pageLayoutItems.Add(new CommandShowCoverPage {
				Caption = PdfViewerLocalizer.GetString(PdfViewerStringId.CommandShowCoverPage),
				Command = new CommandWrapper(() => ShowCoverPageCommandInternal),
				GroupIndex = 4,
			});
			return pageLayoutCommand;
		}
		void OnCursorModeChanged(object sender, RoutedEventArgs e) {
			UpdateCursorModeCheckState();
		}
		void UpdatePageLayoutCommand() {
			var pageLayoutCommand = (CommandCheckItems)PageLayoutCommand;
			pageLayoutCommand.UpdateCheckState(UpdatePageLayoutCheckState);
			pageLayoutCommand.SmallGlyph = pageLayoutCommand.Items.Where(x => x is CommandSetPageLayout).SingleOrDefault(x => x.IsChecked).SmallGlyph;
			pageLayoutCommand.LargeGlyph = pageLayoutCommand.Items.Where(x => x is CommandSetPageLayout).SingleOrDefault(x => x.IsChecked).LargeGlyph;
		}
		void UpdateCursorModeCheckState() {
			switch (PdfViewer.Return(x => x.CursorMode, () => CursorModeType.HandTool)) {
				case CursorModeType.HandTool:
					((PdfSetCursorModeItem)HandToolCommand).IsChecked = true;
					((PdfSetCursorModeItem)SelectToolCommand).IsChecked = false;
					((PdfSetCursorModeItem)MarqueeZoomCommand).IsChecked = false;
					break;
				case CursorModeType.SelectTool:
					((PdfSetCursorModeItem)HandToolCommand).IsChecked = false;
					((PdfSetCursorModeItem)SelectToolCommand).IsChecked = true;
					((PdfSetCursorModeItem)MarqueeZoomCommand).IsChecked = false;
					break;
				case CursorModeType.MarqueeZoom:
					((PdfSetCursorModeItem)HandToolCommand).IsChecked = false;
					((PdfSetCursorModeItem)SelectToolCommand).IsChecked = false;
					((PdfSetCursorModeItem)MarqueeZoomCommand).IsChecked = true;
					break;
			}
		}
		protected override void UnsubscribeFromEvents() {
			base.UnsubscribeFromEvents();
			PdfViewer.Do(x => x.CursorModeChanged -= OnCursorModeChanged);
			PdfViewer.Do(x => x.DocumentLoaded -= OnDocumentLoaded);
			PdfViewer.Do(x => x.PageLayoutChanged -= OnPageLayoutChanged);
		}
		protected override void SubscribeToEvents() {
			base.SubscribeToEvents();
			PdfViewer.Do(x => x.CursorModeChanged += OnCursorModeChanged);
			PdfViewer.Do(x => x.DocumentLoaded += OnDocumentLoaded);
			PdfViewer.Do(x => x.PageLayoutChanged += OnPageLayoutChanged);
		}
		protected override void OnDocumentChanged(object sender, RoutedEventArgs e) {
			base.OnDocumentChanged(sender, e);
			if (PdfViewer.RecentFiles != null)
				(OpenDocumentSplitCommand as PdfOpenDocumentSplitItem).Do(x => x.RecentFiles = new ObservableCollection<RecentFileViewModel>(PdfViewer.RecentFiles.Reverse()));
		}
		protected virtual void OnDocumentLoaded(object sender, RoutedEventArgs e) {
			UpdatePagination();
		}
		protected virtual void OnPageLayoutChanged(object sender, RoutedEventArgs e) {
			UpdatePageLayoutCommand();
		}
		protected virtual bool UpdatePageLayoutCheckState(CommandToggleButton button) {
			if (button is CommandShowCoverPage)
				return PdfViewer.Return(x => x.IsShowCoverPage, () => false);
			var setPageLayoutButton = button as CommandSetPageLayout;
			if (setPageLayoutButton == null || setPageLayoutButton.IsSeparator)
				return false;
			switch (setPageLayoutButton.PageLayout) {
				case PdfPageLayout.TwoColumnLeft:
					return PdfViewer.Return(x => x.PageLayout == PdfPageLayout.TwoColumnLeft || x.PageLayout == PdfPageLayout.TwoColumnRight, () => false);
				case PdfPageLayout.TwoPageLeft:
					return PdfViewer.Return(x => x.PageLayout == PdfPageLayout.TwoPageLeft || x.PageLayout == PdfPageLayout.TwoPageRight, () => false);
			}
			return setPageLayoutButton.PageLayout == PdfViewer.Return(y => y.PageLayout, () => PdfPageLayout.OneColumn);
		}
	}
}
