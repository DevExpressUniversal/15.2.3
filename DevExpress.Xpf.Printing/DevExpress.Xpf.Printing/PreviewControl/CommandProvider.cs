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
using System.Linq;
using System.Text;
using System.Windows.Input;
using DevExpress.Xpf.DocumentViewer;
using DevExpress.Mvvm.Native;
using System.Windows;
using System.Reflection;
using DevExpress.XtraPrinting;
using DevExpress.XtraPrinting.Native;
using System.Collections.ObjectModel;
using DevExpress.Xpf.Printing.PreviewControl.Bars;
using DevExpress.Xpf.Bars;
using System.Windows.Media.Imaging;
using System.Globalization;
using System.Windows.Markup;
using System.Windows.Data;
namespace DevExpress.Xpf.Printing {
	public class DocumentCommandProvider : CommandProvider {
		new DocumentPreviewControl DocumentViewer { get { return base.DocumentViewer as DocumentPreviewControl; } }
		public ICommand HandToolCommand { get; protected set; }
		public ICommand SelectToolCommand { get; protected set; }
		public ICommand CopyCommand { get; private set; }
		public ICommand ToggleParametersPanelCommand { get; private set; }
		public ICommand ToggleDocumentMapCommand { get; private set; }
		public ICommand SaveCommand { get; private set; }
		public ICommand PrintCommand { get; private set; }
		public ICommand PrintDirectCommand { get; private set; }
		public ICommand PageSetupCommand { get; private set; }
		public ICommand ScaleCommand { get; private set; }
		public ICommand FirstPageCommand { get; private set; }
		public ICommand LastPageCommand { get; private set; }
		public ICommand ExportSplitCommand { get; private set; }
		public ICommand SendSplitCommand { get; private set; }
		public ICommand SetWatermarkCommand { get; private set; }
		public ICommand StopPageBuildingCommand { get; private set; }
		protected override void InitializeElements() {
			base.InitializeElements();
			var dllName = Assembly.GetExecutingAssembly().GetName().Name;
			#region document
			((CommandButton)OpenDocumentCommand).Caption = PrintingLocalizer.GetString(PrintingStringId.Open);
			((CommandButton)OpenDocumentCommand).Hint = PrintingLocalizer.GetString(PrintingStringId.Open_Hint);
			SaveCommand = new CommandButton() {
				Caption = PrintingLocalizer.GetString(PrintingStringId.Save),
				Hint = PrintingLocalizer.GetString(PrintingStringId.Save_Hint),
				Command = new CommandWrapper(() => SaveInternal),
				LargeGlyph = UriHelper.GetUri(dllName, @"\Images\BarItems\Save_32x32.png"),
				SmallGlyph = UriHelper.GetUri(dllName, @"\Images\BarItems\Save_16x16.png"),
				Group = "DocumentGroup"
			};
			ToggleParametersPanelCommand = new CommandCheckItem() {
				Caption = PrintingLocalizer.GetString(PrintingStringId.Parameters),
				Hint = PrintingLocalizer.GetString(PrintingStringId.Parameters_Hint),
				Command = new CommandWrapper(() => ToggleParametersPanelInternal),
				LargeGlyph = UriHelper.GetUri(dllName, @"\Images\BarItems\Parameters_32x32.png"),
				SmallGlyph = UriHelper.GetUri(dllName, @"\Images\BarItems\Parameters_16x16.png"),
				Group = "DocumentGroup",
				IsChecked = true
			};
			ToggleDocumentMapCommand = new CommandCheckItem() {
				Caption = PrintingLocalizer.GetString(PrintingStringId.DocumentMap),
				Hint = PrintingLocalizer.GetString(PrintingStringId.DocumentMap_Hint),
				Command = new CommandWrapper(() => ToggleDocumentMapInternal),
				LargeGlyph = UriHelper.GetUri(dllName, @"\Images\BarItems\DocumentMap_32x32.png"),
				SmallGlyph = UriHelper.GetUri(dllName, @"\Images\BarItems\DocumentMap_16x16.png"),
				Group = "DocumentGroup",
				IsChecked = true
			};
			#endregion
			#region print
			PrintCommand = new CommandButton() {
				Caption = PrintingLocalizer.GetString(PrintingStringId.Print),
				Hint = PrintingLocalizer.GetString(PrintingStringId.Print_Hint),
				Command = new CommandWrapper(() => PrintInternal),
				LargeGlyph = UriHelper.GetUri(dllName, @"\Images\BarItems\PrintDialog_32x32.png"),
				SmallGlyph = UriHelper.GetUri(dllName, @"\Images\BarItems\PrintDialog_16x16.png"),
				Group = "PrintGroup"
			};
			PrintDirectCommand = new CommandButton() {
				Caption = PrintingLocalizer.GetString(PrintingStringId.PrintDirect),
				Hint = PrintingLocalizer.GetString(PrintingStringId.PrintDirect_Hint),
				Command = new CommandWrapper(() => PrintDirectInternal),
				LargeGlyph = UriHelper.GetUri(dllName, @"\Images\BarItems\Print_32x32.png"),
				SmallGlyph = UriHelper.GetUri(dllName, @"\Images\BarItems\Print_16x16.png"),
				Group = "PrintGroup"
			};
			PageSetupCommand = new CommandButton() {
				Caption = PrintingLocalizer.GetString(PrintingStringId.PageSetup),
				Hint = PrintingLocalizer.GetString(PrintingStringId.PageSetup_Hint),
				Command = new CommandWrapper(() => PageSetupInternal),
				LargeGlyph = UriHelper.GetUri(dllName, @"\Images\BarItems\PageSetup_32x32.png"),
				SmallGlyph = UriHelper.GetUri(dllName, @"\Images\BarItems\PageSetup_16x16.png"),
				Group = "PrintGroup"
			};
			ScaleCommand = new CommandButton() {
				Caption = PrintingLocalizer.GetString(PrintingStringId.Scaling),
				Hint = PrintingLocalizer.GetString(PrintingStringId.Scaling_Hint),
				Command = new CommandWrapper(() => ScaleInternal),
				LargeGlyph = UriHelper.GetUri(dllName, @"\Images\BarItems\Scaling_32x32.png"),
				SmallGlyph = UriHelper.GetUri(dllName, @"\Images\BarItems\Scaling_16x16.png"),
				Group = "PrintGroup"
			};
			#endregion
			#region Navigation
			FirstPageCommand = new CommandButton() {
				Caption = PrintingLocalizer.GetString(PrintingStringId.FirstPage),
				Hint = PrintingLocalizer.GetString(PrintingStringId.FirstPage_Hint),
				Command = new CommandWrapper(() => FirstPageInternal),
				LargeGlyph = UriHelper.GetUri(dllName, @"\Images\BarItems\First_32x32.png"),
				SmallGlyph = UriHelper.GetUri(dllName, @"\Images\BarItems\First_16x16.png"),
				Group = "NavigationGroup"
			};
			((CommandButton)NextPageCommand).Do(x=> {
				x.LargeGlyph = UriHelper.GetUri(dllName, @"\Images\BarItems\Next_32x32.png");
				x.SmallGlyph = UriHelper.GetUri(dllName, @"\Images\BarItems\Next_16x16.png");
				x.Caption = PrintingLocalizer.GetString(PrintingStringId.NextPage);
				x.Hint = PrintingLocalizer.GetString(PrintingStringId.NextPage_Hint);
			});
			((CommandButton)PreviousPageCommand).Do(x => {
				x.LargeGlyph = UriHelper.GetUri(dllName, @"\Images\BarItems\Prev_32x32.png");
				x.SmallGlyph = UriHelper.GetUri(dllName, @"\Images\BarItems\Prev_16x16.png");
				x.Caption = PrintingLocalizer.GetString(PrintingStringId.PreviousPage);
				x.Hint = PrintingLocalizer.GetString(PrintingStringId.PreviousPage);
			});
			LastPageCommand = new CommandButton() {
				Caption = PrintingLocalizer.GetString(PrintingStringId.LastPage),
				Hint = PrintingLocalizer.GetString(PrintingStringId.LastPage_Hint),
				Command = new CommandWrapper(() => LastPageInternal),
				LargeGlyph = UriHelper.GetUri(dllName, @"\Images\BarItems\Last_32x32.png"),
				SmallGlyph = UriHelper.GetUri(dllName, @"\Images\BarItems\Last_16x16.png"),
				Group = "NavigationGroup"
			};
			#endregion
			#region zoom 
			((CommandButton)ZoomInCommand).Do(x => {
				x.Caption = PrintingLocalizer.GetString(PrintingStringId.IncreaseZoom);
				x.Hint = PrintingLocalizer.GetString(PrintingStringId.IncreaseZoom_Hint);
			});
			((CommandButton)ZoomOutCommand).Do(x => {
				x.Caption = PrintingLocalizer.GetString(PrintingStringId.DecreaseZoom);
				x.Hint = PrintingLocalizer.GetString(PrintingStringId.DecreaseZoom_Hint);
			});
			#endregion
			#region export
			var exportCommandWrapper = new CommandWrapper(() => ExportInternal);
			ExportSplitCommand = new CommandSplitItem() {
				Caption = PrintingLocalizer.GetString(PrintingStringId.ExportFile),
				Hint = PrintingLocalizer.GetString(PrintingStringId.ExportFile_Hint),
				Command = exportCommandWrapper,
				Commands = new ObservableCollection<ICommand>(GetExportFormats()
					.Select(x => CreateExportCommand(x, exportCommandWrapper, false))),
				LargeGlyph = UriHelper.GetUri(dllName, @"\Images\BarItems\Export_32x32.png"),
				SmallGlyph = UriHelper.GetUri(dllName, @"\Images\BarItems\Export_16x16.png"),
				Group = "NavigationGroup"
			};
			var sendCommandWrapper = new CommandWrapper(() => SendInternal);
			SendSplitCommand = new CommandSplitItem() {
				Caption = PrintingLocalizer.GetString(PrintingStringId.SendFile),
				Hint = PrintingLocalizer.GetString(PrintingStringId.SendFile_Hint),
				Command = sendCommandWrapper,
				Commands = new ObservableCollection<ICommand>(GetSendFormats()
					.Select(x => CreateExportCommand(x, sendCommandWrapper, false))),
				LargeGlyph = UriHelper.GetUri(dllName, @"\Images\BarItems\Mail_32x32.png"),
				SmallGlyph = UriHelper.GetUri(dllName, @"\Images\BarItems\Mail_16x16.png"),
				Group = "NavigationGroup"
			};
			#endregion
			#region interactivity
			HandToolCommand = new SetCursorModeItem {
				Caption = PrintingLocalizer.GetString(PrintingStringId.HandTool),
				Command = new CommandWrapper(() => SetCursorModeCommandInternal),
				LargeGlyph = UriHelper.GetUri(dllName, @"\Images\BarItems\HandTool_32x32.png"),
				SmallGlyph = UriHelper.GetUri(dllName, @"\Images\BarItems\HandTool_16x16.png"),
				Group = "CursorMode",
				CommandValue = CursorModeType.HandTool,
			};
			SelectToolCommand = new SetCursorModeItem {
				Caption = PrintingLocalizer.GetString(PrintingStringId.SelectTool),
				Command = new CommandWrapper(() => SetCursorModeCommandInternal),
				LargeGlyph = UriHelper.GetUri(dllName, @"\Images\BarItems\SelectTool_32x32.png"),
				SmallGlyph = UriHelper.GetUri(dllName, @"\Images\BarItems\SelectTool_16x16.png"),
				Group = "CursorMode",
				CommandValue = CursorModeType.SelectTool,
			};
			CopyCommand = new CommandButton {
				Caption = PrintingLocalizer.GetString(PrintingStringId.Copy),
				Command = new CommandWrapper(() => CopyCommandInternal),
				LargeGlyph = UriHelper.GetUri(dllName, @"\Images\BarItems\Copy_32x32.png"),
				SmallGlyph = UriHelper.GetUri(dllName, @"\Images\BarItems\Copy_16x16.png"),
			};
			#endregion interactivity
			((CommandButton)this.ShowFindTextCommand).Do(x => {
				x.Caption = PrintingLocalizer.GetString(PrintingStringId.Search);
				x.Hint = PrintingLocalizer.GetString(PrintingStringId.Search_Hint);
			});
			SetWatermarkCommand = new CommandButton() {
				Caption = PrintingLocalizer.GetString(PrintingStringId.Watermark),
				Hint = PrintingLocalizer.GetString(PrintingStringId.Watermark_Hint),
				Command = new CommandWrapper(() => SetWatermarkInternal),
				LargeGlyph = UriHelper.GetUri(dllName, @"\Images\BarItems\Watermark_32x32.png"),
				SmallGlyph = UriHelper.GetUri(dllName, @"\Images\BarItems\Watermark_16x16.png"),
				Group = "Watermark"
			};
			StopPageBuildingCommand = new CommandButton() {
				Caption = PrintingLocalizer.GetString(PrintingStringId.StopPageBuilding),
				Command = new CommandWrapper(() => StopPageBuildingCommandInternal),
				SmallGlyph = UriHelper.GetUri(dllName, @"\Images\BarItems\Stop_16x16.png")
			};
			UpdateCursorModeCheckState();
		}
		protected override ICommand CreateZoomModeAndZoomFactorItem(string dllName) {
			var zoomValues = new[] { 0.25, 0.5, 0.75, 1, 1.5, 1.75, 2, 4, 6 };
			var zoomItems = new ObservableCollection<CommandToggleButton>();
			var setZoomModeAndFactor = new CommandCheckItems {
				Caption = PrintingLocalizer.GetString(PrintingStringId.Zoom),
				Hint = PrintingLocalizer.GetString(PrintingStringId.Zoom_Hint),
				Group = DefaultPreviewBarItemNames.ZoomGroup,
				Command = DelegateCommandFactory.Create(() => { }, () => zoomItems.Any(x => x.CanExecute(null))),
				Items = zoomItems,
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
			foreach(var zoomValue in zoomValues)
				zoomItems.Add(new CommandSetZoomFactorAndModeItem {
					Caption = string.Format("{0:P0}", zoomValue),
					Command = new CommandWrapper(() => setZoomFactorCommand),
					ZoomFactor = zoomValue
				});
			zoomItems.Add(new CommandSetZoomFactorAndModeItem { IsSeparator = true });
			zoomItems.Add(new CommandSetZoomFactorAndModeItem {
				Caption = PrintingLocalizer.GetString(PrintingStringId.ZoomToWholePage),
				Command = new CommandWrapper(() => setZoomModeCommand),
				ZoomMode = ZoomMode.PageLevel,
				KeyGesture = new KeyGesture(Key.D0, ModifierKeys.Control),
				GroupIndex = 2
			});
			zoomItems.Add(new CommandSetZoomFactorAndModeItem {
				Caption = PrintingLocalizer.GetString(PrintingStringId.ZoomToPageWidth),
				Command = new CommandWrapper(() => setZoomModeCommand),
				ZoomMode = ZoomMode.FitToWidth,
				KeyGesture = new KeyGesture(Key.D1, ModifierKeys.Control),
				GroupIndex = 2
			});
			return setZoomModeAndFactor;
		}
		void UpdateZoomCommand() {
			var zoomCommand = ZoomCommand as CommandCheckItems;
			zoomCommand.Do(x => x.UpdateCheckState(UpdateZoomFactorCheckState));
		}
		ICommand CreateExportCommand(ExportFormat format, ICommand exportCommand, bool isSendOperation) {
			var commandButton = new ExportCommandButton(format) {
				Command = exportCommand,
				SmallGlyph = ExportFormatHelepr.GetImageUri(format, GlyphSize.Small),
				LargeGlyph = ExportFormatHelepr.GetImageUri(format, GlyphSize.Large),
				Caption = PrintingLocalizer.GetString(ExportFormatHelepr.GetExportCaption(format, isSendOperation)),
				Tag = isSendOperation ? "Send" : "Export"
			};
			return commandButton;
		}
		protected virtual IEnumerable<ExportFormat> GetExportFormats() {
			var availableFormats = Enum.GetValues(typeof(ExportFormat)).Cast<ExportFormat>().Except(new [] {ExportFormat.Prnx});
			DocumentViewer.Do(x => availableFormats = availableFormats.Except(x.HiddenExportFormats));
			return availableFormats;
		}
		protected virtual IEnumerable<ExportFormat> GetSendFormats() {
			var availableFormats = Enum.GetValues(typeof(ExportFormat)).Cast<ExportFormat>().Except(new[] { ExportFormat.Prnx, ExportFormat.Htm });
			DocumentViewer.Do(x => availableFormats = availableFormats.Except(x.HiddenExportFormats));
			return availableFormats;
		}
		protected internal virtual ICommand SetCursorModeCommandInternal { get { return DocumentViewer.With(x => x.SetCursorModeCommand); } }
		protected internal virtual ICommand CopyCommandInternal { get { return DocumentViewer.With(x => x.CopyCommand); } }
		protected internal virtual ICommand SaveInternal { get { return DocumentViewer.With(x => x.SaveCommand); } }
		protected internal virtual ICommand ToggleParametersPanelInternal { get { return DocumentViewer.With(x => x.ToggleParametersPanelCommand); } }
		protected internal virtual ICommand ExportInternal { get { return DocumentViewer.With(x => x.ExportCommand); } }
		protected internal virtual ICommand SendInternal { get { return DocumentViewer.With(x => x.SendCommand); } }
		protected internal virtual ICommand LastPageInternal { get { return DocumentViewer.With(x => x.LastPageCommand); } }
		protected internal virtual ICommand FirstPageInternal { get { return DocumentViewer.With(x => x.FirstPageCommand); } }
		protected internal virtual ICommand ScaleInternal { get { return DocumentViewer.With(x => x.ScaleCommand); } }
		protected internal virtual ICommand PageSetupInternal { get { return DocumentViewer.With(x => x.PageSetupCommand); } }
		protected internal virtual ICommand PrintDirectInternal { get { return DocumentViewer.With(x => x.PrintDirectCommand); } }
		protected internal virtual ICommand PrintInternal { get { return DocumentViewer.With(x => x.PrintCommand); } }
		protected internal virtual ICommand ToggleDocumentMapInternal { get { return DocumentViewer.With(x => x.ToggleDocumentMapCommand); } }
		protected internal virtual ICommand SetWatermarkInternal { get { return DocumentViewer.With(x => x.SetWatermarkCommand); } }
		protected internal virtual ICommand StopPageBuildingCommandInternal { get { return DocumentViewer.With(x => x.StopPageBuildingCommand); } }
		protected override void SubscribeToEvents() {
			base.SubscribeToEvents();
			DocumentViewer.Do(x => x.CursorModeChanged += OnCursorModeChanged);
		}
		protected override void UnsubscribeFromEvents() {
			base.UnsubscribeFromEvents();
			DocumentViewer.Do(x => x.CursorModeChanged -= OnCursorModeChanged);
		}
		void OnCursorModeChanged(object sender, RoutedEventArgs e) {
			UpdateCursorModeCheckState();
		}
		void UpdateCursorModeCheckState() {
			if(DocumentViewer == null)
				return;
			switch(DocumentViewer.Return(x => x.CursorMode, () => CursorModeType.HandTool)) {
				case CursorModeType.HandTool:
					((SetCursorModeItem)HandToolCommand).IsChecked = true;
					((SetCursorModeItem)SelectToolCommand).IsChecked = false;
					DocumentViewer.DocumentPresenter.SelectionService.ResetSelectedBricks();
					break;
				case CursorModeType.SelectTool:
					((SetCursorModeItem)HandToolCommand).IsChecked = false;
					((SetCursorModeItem)SelectToolCommand).IsChecked = true;
					break;
			}
		}
		internal void UpdateCommands() {
			if(!DocumentViewer.DocumentPresenter.Return(x => x.IsContentLoaded, () => true))
				return;
			((CommandCheckItem)ToggleDocumentMapCommand).Do(c => {
				var canShow = DocumentViewer.Document.Return(x => x.HasBookmarks, () => false);
				var autoShow = DocumentViewer.AutoShowDocumentMap;
				c.IsChecked = canShow ? autoShow ? true | c.IsChecked : false : false;
			});
			((CommandCheckItem)ToggleParametersPanelCommand).Do(c => {
				var canShow = DocumentViewer.ParametersModel.HasVisibleParameters;
				var autoShow = DocumentViewer.AutoShowParametersPanel;
				c.IsChecked = canShow ? autoShow ? true | c.IsChecked : false : false;
			});
			((CommandSplitItem)ExportSplitCommand).Do(c => c.Commands = new ObservableCollection<ICommand>(GetExportFormats()
					.Select(x => CreateExportCommand(x, c.Command, false))));
			((CommandSplitItem)SendSplitCommand).Do(c => c.Commands = new ObservableCollection<ICommand>(GetSendFormats()
					.Select(x => CreateExportCommand(x, c.Command, false))));
			CommandManager.InvalidateRequerySuggested();
		}
	}
	public class CommandCheckItem : CommandBase {
		bool isChecked;
		public bool IsChecked {
			get { return isChecked; }
			set { SetProperty(ref isChecked, value, () => IsChecked); }
		}
	}
	public class CommandSplitItem : CommandBase {
		ObservableCollection<ICommand> commands = new ObservableCollection<ICommand>();
		public ObservableCollection<ICommand> Commands {
			get { return commands; }
			set{ SetProperty(ref commands, value, ()=> Commands);}
		}
	}
	public class ExportCommandButton : CommandButton {
		readonly ExportFormat format;
		public ExportFormat Format {
			get { return format; }
		}
		public object Tag { get; internal set; }
		public ExportCommandButton(ExportFormat format) {
			this.format = format;
		}
	}
	static class ExportFormatHelepr {
		const string large = "32x32";
		const string small = "16x16";
		const string relativePath = @"\Images\BarItems\ExportTo{0}_{1}.png";
		const string export = "Export";
		const string send = "Send";
		readonly static string dllName = Assembly.GetExecutingAssembly().GetName().Name;
		public static Uri GetImageUri(ExportFormat format, GlyphSize size) {
			if(format == ExportFormat.Htm)
				return UriHelper.GetUri(dllName, string.Format(relativePath, "html", GlyphSizeToString(size)));
			else if(format == ExportFormat.Txt)
				return UriHelper.GetUri(dllName, string.Format(relativePath, "Text", GlyphSizeToString(size)));
			else
				return UriHelper.GetUri(dllName, string.Format(relativePath, format.ToString(), GlyphSizeToString(size)));
		}
		public static PrintingStringId GetExportCaption(ExportFormat format, bool isSendOperation) {
			var stringFormat = isSendOperation ? send : export + format.ToString();
			PrintingStringId stringId;
			if(Enum.TryParse(stringFormat, out stringId))
				return stringId;
			else
				return isSendOperation ? PrintingStringId.SendFile : PrintingStringId.ExportFile;
		}
		public 
		static string GlyphSizeToString(GlyphSize size) {
			switch(size) {
				case GlyphSize.Large:
					return large;
				case GlyphSize.Default:
				case GlyphSize.Small:
				default:
					return small;
			}
		}
	}
	public class SetCursorModeItem : CommandCheckItem {
		CursorModeType commandValue;
		public CursorModeType CommandValue {
			get { return commandValue; }
			set { SetProperty(ref commandValue, value, () => CommandValue); }
		}
	}
}
