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
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Windows.Forms.Design;
using DevExpress.Utils.Design;
using DevExpress.XtraReports.UserDesigner;
using System.Collections.Generic;
using DevExpress.Utils;
namespace DevExpress.XtraReports.UserDesigner {
	[Flags]
	public enum CommandVisibility { 
		None = 0, 
		Toolbar = 1, 
		ContextMenu = 2,
		Verb = 4,
		All = Toolbar | ContextMenu | Verb
	};
	public interface ICommandHandler {
		void HandleCommand(ReportCommand command, object[] args);
		bool CanHandleCommand(ReportCommand command, ref bool useNextHandler);
	}
	#region ReportCommand enum
	public enum ReportCommand {
		None,
		#region UI commands
		NewReport,
		NewReportWizard,
		OpenFile,
		SaveFile,
		SaveFileAs,
		SaveAll,
		Exit,
		MdiCascade,
		MdiTileHorizontal,
		MdiTileVertical,
		ShowTabbedInterface,
		ShowWindowInterface,
		[Browsable(false)]
		Closing,
		AddNewDataSource,
		ShowDesignerTab,
		ShowScriptsTab,
		ShowPreviewTab,
		ShowHTMLViewTab,
		Undo,
		Redo,
		Close,
		[Browsable(false)]
		OpenSubreport,
		[Browsable(false)]
		CheckIn,
		[Browsable(false)]
		UndoCheckOut,
		[Browsable(false)]
		OpenRemoteReport,
		[Browsable(false)]
		UploadNewRemoteReport,
		[Browsable(false)]
		RevertToRevision,
		#endregion
		#region Edit commnands
		Cut,
		Copy,
		Paste,
		Delete,
		SelectAll,
		#endregion
		#region Format commands
		AlignLeft,
		AlignTop,
		AlignRight,
		AlignBottom,
		AlignVerticalCenters,
		AlignHorizontalCenters,
		AlignToGrid,
		SizeToGrid,
		SizeToControl,
		SizeToControlHeight,
		SizeToControlWidth,
		HorizSpaceConcatenate,
		HorizSpaceDecrease,
		HorizSpaceIncrease,
		HorizSpaceMakeEqual,
		VertSpaceConcatenate,
		VertSpaceDecrease,
		VertSpaceIncrease,
		VertSpaceMakeEqual,
		CenterVertically, 
		CenterHorizontally,
		BringToFront, 
		SendToBack,
		FontBold,
		FontItalic,
		FontUnderline,
		FontName,
		FontSize,
		ForeColor,
		BackColor,
		JustifyLeft,	
		JustifyCenter,
		JustifyRight,
		JustifyJustify,
		#endregion
		#region Context menu commands
		InsertTopMarginBand,
		InsertBottomMarginBand,
		InsertReportHeaderBand,
		InsertReportFooterBand,
		InsertPageHeaderBand,
		InsertPageFooterBand,
		InsertGroupHeaderBand,
		InsertGroupFooterBand,
		InsertDetailBand,
		InsertDetailReport,
		BandMoveUp,
		BandMoveDown,
		TableInsertRowAbove,
		TableInsertRowBelow,
		TableInsertColumnToLeft,
		TableInsertColumnToRight,
		TableInsertCell,
		TableDeleteRow,
		TableDeleteColumn,
		TableDeleteCell,
		TableConvertToLabels,
		BindFieldToXRLabel,
		BindFieldToXRPictureBox,
		BindFieldToXRRichText,
		BindFieldToXRCheckBox,
		BindFieldToXRBarCode,
		BindFieldToXRZipCode,
		PropertiesWindow,
		#endregion
		#region Key commands
		KeyMoveLeft,
		KeyMoveRight,
		KeyMoveUp,
		KeyMoveDown,
		KeyNudgeLeft,
		KeyNudgeRight,
		KeyNudgeUp,
		KeyNudgeDown,
		KeySizeWidthDecrease,
		KeySizeWidthIncrease,
		KeySizeHeightDecrease,
		KeySizeHeightIncrease,
		KeyNudgeWidthDecrease,
		KeyNudgeWidthIncrease,
		KeyNudgeHeightDecrease,
		KeyNudgeHeightIncrease,
		KeySelectNext,
		KeySelectPrevious,
		KeyCancel,
		KeyDefaultAction,
		#endregion
		#region Verb commands
		VerbReportWizard,
		VerbEditBands,
		VerbEditText,
		VerbRtfLoadFile,
		VerbRtfClear,
		VerbPivotGridDesigner,
		VerbLoadReportTemplate,
		VerbEditBindings,
		VerbImport,
		[EditorBrowsable(EditorBrowsableState.Never)]
		VerbExport,
		[EditorBrowsable(EditorBrowsableState.Never)]
		VerbExecute,
		#endregion
		#region Zoom commands
		Zoom,
		ZoomIn,
		ZoomOut,
		#endregion
		#region HtmlCommands
		HtmlRefresh,
		HtmlBackward,
		HtmlForward,
		HtmlHome,
		HtmlFind,
		#endregion
	}
	#endregion
}
namespace DevExpress.XtraReports.Design.Commands {
	public enum Priority {
		Low = 0,
		High = 1
	}
	public class ReportCommandEventArgs : MenuCommandEventArgs {
		ReportCommand command;
		public ReportCommand Command { get { return command; }
		}
		public ReportCommandEventArgs(ReportCommand command, MenuCommand menuCommand) : base(menuCommand) {
			this.command = command;
		}
	}
	public delegate void ReportCommandEventHandler(object sender, ReportCommandEventArgs e);
	#region ReportCommandService
	public class ReportCommandServiceBase : IDisposable {
		static ReportCommand[] AllCommands {
			get {
				return (ReportCommand[])Enum.GetValues(typeof(ReportCommand));
			}
		}
		CommandHandlerCollection handlers;
		Dictionary<ReportCommand, ReportCommandInfo> commandInfos = new Dictionary<ReportCommand, ReportCommandInfo>();
		protected ReportCommandInfo this[ReportCommand reportCommand] { 
			get {
				ReportCommandInfo value;
				return commandInfos.TryGetValue(reportCommand, out value) ? value : null; 
			}
		}
		protected ICollection ReportCommandInfos { get { return commandInfos.Values; } }
		public event ReportCommandEventHandler CommandChanged;
		public ReportCommandServiceBase() {
			handlers = new CommandHandlerCollection();
			foreach(ReportCommand command in AllCommands)
				commandInfos.Add(command, new ReportCommandInfo(command));
		}
		public void AddCommandHandler(ICommandHandler handler) {
			handlers.Add(handler);
		}
		public void RemoveCommandHandler(ICommandHandler handler) {
			handlers.Remove(handler);
		}
		public bool CanHandleCommand(ReportCommand command) {
			return handlers.CanHandleCommand(command);
		}
		public bool HandleCommand(ReportCommand command, object[] args) {
			return handlers.HandleCommand(command, args);
		}
		public void AddCommandHandlers(ReportCommandServiceBase source) {
			foreach(ICommandHandler item in source.handlers)
				AddCommandHandler(item);
		}
		public void CopyCommands(ReportCommandServiceBase source) {
			foreach(ReportCommandInfo item in ReportCommandInfos) {
				item.CopyFrom(source[item.ReportCommand]);
			}
		}
		public CommandVisibility GetCommandVisibility(ReportCommand command) {
			ReportCommandInfo value;
			return commandInfos.TryGetValue(command, out value) ? value.Visibility : CommandVisibility.None;
		}
		public void SetCommandVisibility(ReportCommand[] commands, CommandVisibility visibility) {
			foreach(ReportCommand command in commands)
				SetCommandVisibility(command, visibility);
		}
		public void SetCommandVisibility(ReportCommand command, CommandVisibility visibility) {
			ReportCommandInfo info = this[command];
			CommandVisibility savedVisibility = info.Visibility;
			info.SetVisibility(visibility, Priority.High);
			if(savedVisibility != info.Visibility)
				OnCommandChanged(info);
		}
		protected virtual void OnCommandChanged(ReportCommandInfo info) {
			RaiseCommandChanged(info.ReportCommand, null);
		}
		protected void RaiseCommandChanged(ReportCommand reportCommand, MenuCommand menuCommand) {
			if(CommandChanged != null) CommandChanged(this, new ReportCommandEventArgs(reportCommand, menuCommand));
		}
		#region IDisposable Members
		public void Dispose() {
			Dispose(true);
			GC.SuppressFinalize(this);
		}
		protected void Dispose(bool disposing) { 
			if(disposing)
				commandInfos.Clear();
		}
		~ReportCommandServiceBase() {
			Dispose(false);
		}
		#endregion
	}
	public class ReportCommandService : ReportCommandServiceBase {
		IMenuCommandServiceEx menuCommandService;
		public ReportCommandService() {
		}
		MenuCommand GetMenuCommand(ReportCommand reportCommand) {
			CommandID id = ToCommandID(reportCommand);
			return menuCommandService != null  && id != null ? menuCommandService.FindCommand(id) : null;
		}
		static CommandID ToCommandID(ReportCommandInfo info) {
			return ToCommandID(info.ReportCommand);
		}
		static CommandID ToCommandID(ReportCommand reportCommand) {
			return CommandIDReportCommandConverter.GetCommandID(reportCommand);
		}
		static ReportCommand ToReportCommand(CommandID commmandID) {
			return CommandIDReportCommandConverter.GetReportCommand(commmandID);
		}
		static bool ToVisibility(CommandVisibility commandVisibility) {
			return commandVisibility != CommandVisibility.None;
		}
		static CommandVisibility ToCommandVisibility(bool visible) {
			return visible ? CommandVisibility.All : CommandVisibility.None;
		}
		protected override void OnCommandChanged(ReportCommandInfo info) {
			MenuCommand menuCommand = GetMenuCommand(info.ReportCommand);
			UpdateMenuCommandVisible(info, menuCommand);
			RaiseCommandChanged(info.ReportCommand, menuCommand);
		}
		void UpdateMenuCommandVisible(ReportCommandInfo info, MenuCommand menuCommand) {
			if(menuCommand != null && info.Priority != Priority.Low)
				UpdateMenuCommandVisible(menuCommand, ToVisibility(info.Visibility));
		}
		void UpdateMenuCommandVisible(MenuCommand menuCommand, bool visibility) {
			Unsubscribe(menuCommandService);
			menuCommand.Visible = visibility;
			Subscribe(menuCommandService);
		}
		public bool GetCommandEnabled(ReportCommand command) {
			MenuCommand menuCommand = GetMenuCommand(command);
			return menuCommand != null ? menuCommand.Enabled : CanHandleCommand(command);
		}
		public bool GetCommandChecked(ReportCommand command) {
			MenuCommand menuCommand = GetMenuCommand(command);
			return menuCommand != null ? menuCommand.Checked : false;
		}
		public void AttachToMenuCommandService(IServiceProvider servProvider, MenuCommandHandlerBase menuCommandHandler) {
			Unsubscribe(this.menuCommandService);
			IMenuCommandServiceEx menuCommandService = servProvider.GetService(typeof(IMenuCommandService)) as IMenuCommandServiceEx;
			Subscribe(menuCommandService);
			this.menuCommandService = menuCommandService;
			foreach (ReportCommandInfo info in ReportCommandInfos) {
				MenuCommand menuCommand = GetMenuCommand(info.ReportCommand);
				if (menuCommand == null) {
					CommandID commandID = ToCommandID(info.ReportCommand);
					if (commandID != null) {
						CommandSetItem newMenuCommand = CreateMenuCommand(commandID, menuCommandService);
						menuCommandHandler.AddCommand(newMenuCommand);
						menuCommand = newMenuCommand;
					}
				}
				UpdateMenuCommandVisible(info, menuCommand);
			}
			IMenuCreationService serv = servProvider.GetService(typeof(IMenuCreationService)) as IMenuCreationService;
			if(serv == null) return;
			MenuCommandDescription[] items = serv.GetCustomMenuCommands();
			foreach(MenuCommandDescription item in items) {
				MenuCommand menuCommand = menuCommandService.FindCommand(item.CommandID);
				if(menuCommand != null) {
					UpdateMenuCommandVisible(menuCommand, true);
				} else {
					CommandSetItem commandSetItem = new CommandSetItem(menuCommandService, item.OnExecute, item.OnStatusRequire, item.CommandID);
					menuCommandHandler.AddCommand(commandSetItem);
					UpdateMenuCommandVisible(commandSetItem, true);
				}
			}
		}
		void Unsubscribe(IMenuCommandServiceEx svc) {
			if(svc != null)
				svc.MenuCommandChanged -= new EventHandler(menuCommandService_MenuCommandChanged);
		}
		void Subscribe(IMenuCommandServiceEx svc) {
			if(svc != null)
				svc.MenuCommandChanged += new EventHandler(menuCommandService_MenuCommandChanged);
		}
		CommandSetItem CreateMenuCommand(CommandID commandID, IMenuCommandServiceEx menuCommandService) {			
			return new CommandSetItem(menuCommandService, OnInvoke, OnStatus, commandID);
		}
		void OnStatus(object sender, EventArgs e) {
			MenuCommand cmd = (MenuCommand)sender;
			cmd.Enabled = cmd.Supported = CanHandleCommand(cmd);
		}
		bool CanHandleCommand(MenuCommand cmd) {
			return CanHandleCommand(CommandIDReportCommandConverter.GetReportCommand(cmd.CommandID));
		}
		void OnInvoke(object sender, CommandExecuteEventArgs e) {
			MenuCommand cmd = (MenuCommand)sender;
			if (CanHandleCommand(cmd))
				HandleCommand(cmd, e.Args);
		}
		bool HandleCommand(MenuCommand cmd, object[] args) {
			return HandleCommand(CommandIDReportCommandConverter.GetReportCommand(cmd.CommandID), args);
		}
		void menuCommandService_MenuCommandChanged(object sender, EventArgs e) {
			MenuCommand command = sender as MenuCommand;
			if(command != null) {
				ReportCommandInfo info = this[ToReportCommand(command.CommandID)];
				info.SetVisibility(ToCommandVisibility(command.Visible), Priority.Low);
				RaiseCommandChanged(info.ReportCommand, command); 
			}
		}
		public void DetachFromMenuCommandService() {
			Unsubscribe(menuCommandService);
			menuCommandService = null;
		}
		#region tests
		#if DEBUGTEST
		public Priority GetCommandPriority(ReportCommand command) {
			return this[command].Priority;
		}
		#endif
		#endregion
	}
	#endregion
	#region ReportCommandInfo
	public class ReportCommandInfo {
		CommandVisibility visibility;
		Priority priority;
		ReportCommand reportCommand;
		public ReportCommand ReportCommand { get { return reportCommand; } 
		}
		public CommandVisibility Visibility { get { return visibility; }
		}
		public Priority Priority { get { return priority; }
		}
		public void SetVisibility(CommandVisibility visibility, Priority priority) {
			if(this.priority <= priority) {
				this.priority = priority;
				this.visibility = visibility;
			}
		}
		public ReportCommandInfo(ReportCommand reportCommand) {
			this.visibility = CommandVisibility.All;
			this.priority = Priority.Low;
			this.reportCommand = reportCommand;
		}
		internal void CopyFrom(ReportCommandInfo source) {
			System.Diagnostics.Debug.Assert(source.reportCommand == reportCommand);
			visibility = source.visibility;
			priority = source.priority;
		}
	}
	#endregion
	#region CommandHandlerCollection
	public class CommandHandlerCollection : CollectionBase {
		ICommandHandler this[int index] { get { return (ICommandHandler)List[index]; }
		}
		public int Add(ICommandHandler handler) {
			return CanAddHandler(handler) ? List.Add(handler) : -1;
		}
		bool CanAddHandler(ICommandHandler handler) {
			return handler != null && !List.Contains(handler); 
		}
		public void Remove(ICommandHandler handler) {
			if(List.Contains(handler))
				List.Remove(handler);
		}
		public bool HandleCommand(ReportCommand command, object[] args) {
			for (int i = Count - 1; i >= 0; i--) {
				ICommandHandler commandHandler = this[i];
				bool useNextHandler = true;
				if (commandHandler.CanHandleCommand(command, ref useNextHandler))
					commandHandler.HandleCommand(command, args);
				if (!useNextHandler)
					return true;
			}
			return false;
		}
		public bool CanHandleCommand(ReportCommand command) {
			for (int i = Count - 1; i >= 0; i--) {
				ICommandHandler commandHandler = this[i];
				bool useNextHandler = true;
				bool canHandle = commandHandler.CanHandleCommand(command, ref useNextHandler);
				if (!useNextHandler || canHandle)
					return canHandle;
			}
			return false;
		}
		#region tests
		#if DEBUGTEST
		public int IndexOf(ICommandHandler handler) {
			return List.IndexOf(handler);
		}
		#endif //DEBUGTEST
		#endregion
	}
	#endregion
	#region CommandIDReportCommandConverter
	public static class CommandIDReportCommandConverter {
		static Dictionary<ReportCommand, CommandID> commandIDHT = new Dictionary<ReportCommand, CommandID>();
		static Dictionary<CommandID, ReportCommand> reportCommandHT = new Dictionary<CommandID, ReportCommand>();
		static CommandIDReportCommandConverter() {
			#region filling HTs
			#region Report commands
			AddCommandIDReportCommandPair(BandCommands.InsertTopMarginBand, ReportCommand.InsertTopMarginBand);
			AddCommandIDReportCommandPair(BandCommands.InsertBottomMarginBand, ReportCommand.InsertBottomMarginBand);
			AddCommandIDReportCommandPair(BandCommands.InsertReportHeaderBand, ReportCommand.InsertReportHeaderBand);
			AddCommandIDReportCommandPair(BandCommands.InsertReportFooterBand, ReportCommand.InsertReportFooterBand);
			AddCommandIDReportCommandPair(BandCommands.InsertPageHeaderBand, ReportCommand.InsertPageHeaderBand);
			AddCommandIDReportCommandPair(BandCommands.InsertPageFooterBand, ReportCommand.InsertPageFooterBand);
			AddCommandIDReportCommandPair(BandCommands.InsertGroupHeaderBand, ReportCommand.InsertGroupHeaderBand);
			AddCommandIDReportCommandPair(BandCommands.InsertGroupFooterBand, ReportCommand.InsertGroupFooterBand);
			AddCommandIDReportCommandPair(BandCommands.InsertDetailBand, ReportCommand.InsertDetailBand);
			AddCommandIDReportCommandPair(ReportCommands.InsertDetailReport, ReportCommand.InsertDetailReport);
			AddCommandIDReportCommandPair(BandCommands.BindFieldToXRLabel, ReportCommand.BindFieldToXRLabel);
			AddCommandIDReportCommandPair(BandCommands.BindFieldToXRPictureBox, ReportCommand.BindFieldToXRPictureBox);
			AddCommandIDReportCommandPair(BandCommands.BindFieldToXRRichText, ReportCommand.BindFieldToXRRichText);
			AddCommandIDReportCommandPair(BandCommands.BindFieldToXRCheckBox, ReportCommand.BindFieldToXRCheckBox);
			AddCommandIDReportCommandPair(BandCommands.BindFieldToXRBarCode, ReportCommand.BindFieldToXRBarCode);
			AddCommandIDReportCommandPair(BandCommands.BindFieldToXRZipCode, ReportCommand.BindFieldToXRZipCode);
			AddCommandIDReportCommandPair(TableCommands.InsertRowAbove, ReportCommand.TableInsertRowAbove);
			AddCommandIDReportCommandPair(TableCommands.InsertRowBelow, ReportCommand.TableInsertRowBelow);
			AddCommandIDReportCommandPair(TableCommands.InsertColumnToLeft, ReportCommand.TableInsertColumnToLeft);
			AddCommandIDReportCommandPair(TableCommands.InsertColumnToRight, ReportCommand.TableInsertColumnToRight);
			AddCommandIDReportCommandPair(TableCommands.InsertCell, ReportCommand.TableInsertCell);
			AddCommandIDReportCommandPair(TableCommands.DeleteRow, ReportCommand.TableDeleteRow);
			AddCommandIDReportCommandPair(TableCommands.DeleteColumn, ReportCommand.TableDeleteColumn);
			AddCommandIDReportCommandPair(TableCommands.DeleteCell, ReportCommand.TableDeleteCell);
			AddCommandIDReportCommandPair(TableCommands.ConvertToLabels, ReportCommand.TableConvertToLabels);
			#endregion
			#region Key commands
			AddCommandIDReportCommandPair(MenuCommands.KeyMoveLeft, ReportCommand.KeyMoveLeft);
			AddCommandIDReportCommandPair(MenuCommands.KeyMoveRight, ReportCommand.KeyMoveRight);
			AddCommandIDReportCommandPair(MenuCommands.KeyMoveUp, ReportCommand.KeyMoveUp);
			AddCommandIDReportCommandPair(MenuCommands.KeyMoveDown, ReportCommand.KeyMoveDown);
			AddCommandIDReportCommandPair(MenuCommands.KeyNudgeLeft, ReportCommand.KeyNudgeLeft);
			AddCommandIDReportCommandPair(MenuCommands.KeyNudgeRight, ReportCommand.KeyNudgeRight);
			AddCommandIDReportCommandPair(MenuCommands.KeyNudgeUp, ReportCommand.KeyNudgeUp);
			AddCommandIDReportCommandPair(MenuCommands.KeyNudgeDown, ReportCommand.KeyNudgeDown);
			AddCommandIDReportCommandPair(MenuCommands.KeySizeWidthDecrease, ReportCommand.KeySizeWidthDecrease);
			AddCommandIDReportCommandPair(MenuCommands.KeySizeWidthIncrease, ReportCommand.KeySizeWidthIncrease);
			AddCommandIDReportCommandPair(MenuCommands.KeySizeHeightDecrease, ReportCommand.KeySizeHeightDecrease);
			AddCommandIDReportCommandPair(MenuCommands.KeySizeHeightIncrease, ReportCommand.KeySizeHeightIncrease);
			AddCommandIDReportCommandPair(MenuCommands.KeyNudgeWidthDecrease, ReportCommand.KeyNudgeWidthDecrease);
			AddCommandIDReportCommandPair(MenuCommands.KeyNudgeWidthIncrease, ReportCommand.KeyNudgeWidthIncrease);
			AddCommandIDReportCommandPair(MenuCommands.KeyNudgeHeightDecrease, ReportCommand.KeyNudgeHeightDecrease);
			AddCommandIDReportCommandPair(MenuCommands.KeyNudgeHeightIncrease, ReportCommand.KeyNudgeHeightIncrease);
			AddCommandIDReportCommandPair(MenuCommands.KeySelectNext, ReportCommand.KeySelectNext);
			AddCommandIDReportCommandPair(MenuCommands.KeySelectPrevious, ReportCommand.KeySelectPrevious);
			AddCommandIDReportCommandPair(MenuCommands.KeyCancel, ReportCommand.KeyCancel);
			AddCommandIDReportCommandPair(MenuCommands.KeyDefaultAction, ReportCommand.KeyDefaultAction);
			#endregion
			#region Format commands
			AddCommandIDReportCommandPair(StandardCommands.AlignLeft, ReportCommand.AlignLeft);
			AddCommandIDReportCommandPair(StandardCommands.AlignTop, ReportCommand.AlignTop);
			AddCommandIDReportCommandPair(StandardCommands.AlignRight, ReportCommand.AlignRight);
			AddCommandIDReportCommandPair(StandardCommands.AlignBottom, ReportCommand.AlignBottom);
			AddCommandIDReportCommandPair(StandardCommands.AlignVerticalCenters, ReportCommand.AlignVerticalCenters);
			AddCommandIDReportCommandPair(StandardCommands.AlignHorizontalCenters, ReportCommand.AlignHorizontalCenters);
			AddCommandIDReportCommandPair(StandardCommands.AlignToGrid, ReportCommand.AlignToGrid);
			AddCommandIDReportCommandPair(StandardCommands.SizeToGrid, ReportCommand.SizeToGrid);
			AddCommandIDReportCommandPair(StandardCommands.SizeToControl, ReportCommand.SizeToControl);
			AddCommandIDReportCommandPair(StandardCommands.SizeToControlHeight, ReportCommand.SizeToControlHeight);
			AddCommandIDReportCommandPair(StandardCommands.SizeToControlWidth, ReportCommand.SizeToControlWidth);
			AddCommandIDReportCommandPair(StandardCommands.HorizSpaceConcatenate, ReportCommand.HorizSpaceConcatenate);
			AddCommandIDReportCommandPair(StandardCommands.HorizSpaceDecrease, ReportCommand.HorizSpaceDecrease);
			AddCommandIDReportCommandPair(StandardCommands.HorizSpaceIncrease, ReportCommand.HorizSpaceIncrease);
			AddCommandIDReportCommandPair(StandardCommands.HorizSpaceMakeEqual, ReportCommand.HorizSpaceMakeEqual);
			AddCommandIDReportCommandPair(StandardCommands.VertSpaceConcatenate, ReportCommand.VertSpaceConcatenate);
			AddCommandIDReportCommandPair(StandardCommands.VertSpaceDecrease, ReportCommand.VertSpaceDecrease);
			AddCommandIDReportCommandPair(StandardCommands.VertSpaceIncrease, ReportCommand.VertSpaceIncrease);
			AddCommandIDReportCommandPair(StandardCommands.VertSpaceMakeEqual, ReportCommand.VertSpaceMakeEqual);
			AddCommandIDReportCommandPair(StandardCommands.CenterVertically, ReportCommand.CenterVertically);
			AddCommandIDReportCommandPair(StandardCommands.CenterHorizontally, ReportCommand.CenterHorizontally);
			AddCommandIDReportCommandPair(StandardCommands.BringToFront, ReportCommand.BringToFront);
			AddCommandIDReportCommandPair(StandardCommands.SendToBack, ReportCommand.SendToBack);
			AddCommandIDReportCommandPair(FormattingCommands.Bold, ReportCommand.FontBold);
			AddCommandIDReportCommandPair(FormattingCommands.Italic, ReportCommand.FontItalic);
			AddCommandIDReportCommandPair(FormattingCommands.Underline, ReportCommand.FontUnderline);
			AddCommandIDReportCommandPair(FormattingCommands.FontName, ReportCommand.FontName);
			AddCommandIDReportCommandPair(FormattingCommands.FontSize, ReportCommand.FontSize);
			AddCommandIDReportCommandPair(FormattingCommands.ForeColor, ReportCommand.ForeColor);
			AddCommandIDReportCommandPair(FormattingCommands.BackColor, ReportCommand.BackColor);
			AddCommandIDReportCommandPair(FormattingCommands.JustifyLeft, ReportCommand.JustifyLeft);	
			AddCommandIDReportCommandPair(FormattingCommands.JustifyCenter, ReportCommand.JustifyCenter);
			AddCommandIDReportCommandPair(FormattingCommands.JustifyRight, ReportCommand.JustifyRight);
			AddCommandIDReportCommandPair(FormattingCommands.JustifyJustify, ReportCommand.JustifyJustify);
			#endregion
			AddCommandIDReportCommandPair(StandardCommands.Cut, ReportCommand.Cut);
			AddCommandIDReportCommandPair(StandardCommands.Copy, ReportCommand.Copy);
			AddCommandIDReportCommandPair(StandardCommands.Paste, ReportCommand.Paste);
			AddCommandIDReportCommandPair(StandardCommands.Delete, ReportCommand.Delete);
			AddCommandIDReportCommandPair(StandardCommands.SelectAll, ReportCommand.SelectAll);
			AddCommandIDReportCommandPair(WrappedCommands.PropertiesWindow, ReportCommand.PropertiesWindow);
			AddCommandIDReportCommandPair(UICommands.OpenFile, ReportCommand.OpenFile);
			AddCommandIDReportCommandPair(UICommands.SaveFile, ReportCommand.SaveFile);
			AddCommandIDReportCommandPair(UICommands.SaveFileAs, ReportCommand.SaveFileAs);
			AddCommandIDReportCommandPair(UICommands.SaveAll, ReportCommand.SaveAll);
			AddCommandIDReportCommandPair(UICommands.Exit, ReportCommand.Exit);
			AddCommandIDReportCommandPair(StandardCommands.Undo, ReportCommand.Undo);
			AddCommandIDReportCommandPair(StandardCommands.Redo, ReportCommand.Redo);
			AddCommandIDReportCommandPair(UICommands.NewReport, ReportCommand.NewReport);
			AddCommandIDReportCommandPair(UICommands.NewReportWizard, ReportCommand.NewReportWizard);
			AddCommandIDReportCommandPair(UICommands.Closing, ReportCommand.Closing);
			AddCommandIDReportCommandPair(UICommands.MdiCascade, ReportCommand.MdiCascade);
			AddCommandIDReportCommandPair(UICommands.MdiTileHorizontal, ReportCommand.MdiTileHorizontal);
			AddCommandIDReportCommandPair(UICommands.MdiTileVertical, ReportCommand.MdiTileVertical);
			AddCommandIDReportCommandPair(UICommands.ShowTabbedInterface, ReportCommand.ShowTabbedInterface);
			AddCommandIDReportCommandPair(UICommands.ShowWindowInterface, ReportCommand.ShowWindowInterface);
			AddCommandIDReportCommandPair(UICommands.Close, ReportCommand.Close);
			AddCommandIDReportCommandPair(UICommands.OpenSubreport, ReportCommand.OpenSubreport);
			AddCommandIDReportCommandPair(UICommands.CheckIn, ReportCommand.CheckIn);
			AddCommandIDReportCommandPair(UICommands.UndoCheckOut, ReportCommand.UndoCheckOut);
			AddCommandIDReportCommandPair(UICommands.OpenRemoteReport, ReportCommand.OpenRemoteReport);
			AddCommandIDReportCommandPair(UICommands.UploadNewRemoteReport, ReportCommand.UploadNewRemoteReport);
			AddCommandIDReportCommandPair(UICommands.RevertToRevision, ReportCommand.RevertToRevision);
			AddCommandIDReportCommandPair(ReportTabControlCommands.ShowDesignerTab, ReportCommand.ShowDesignerTab);
			AddCommandIDReportCommandPair(ReportTabControlCommands.ShowScriptsTab, ReportCommand.ShowScriptsTab);
			AddCommandIDReportCommandPair(ReportTabControlCommands.ShowPreviewTab, ReportCommand.ShowPreviewTab);
			AddCommandIDReportCommandPair(ReportTabControlCommands.ShowHTMLViewTab, ReportCommand.ShowHTMLViewTab);
			AddCommandIDReportCommandPair(ReportCommands.AddNewDataSource, ReportCommand.AddNewDataSource);
			AddCommandIDReportCommandPair(VerbCommands.ReportWizard, ReportCommand.VerbReportWizard);
			AddCommandIDReportCommandPair(VerbCommands.PivotGridDesigner, ReportCommand.VerbPivotGridDesigner);
			AddCommandIDReportCommandPair(VerbCommands.EditText, ReportCommand.VerbEditText);
			AddCommandIDReportCommandPair(VerbCommands.RtfClear, ReportCommand.VerbRtfClear);
			AddCommandIDReportCommandPair(VerbCommands.RtfLoadFile, ReportCommand.VerbRtfLoadFile);
			AddCommandIDReportCommandPair(VerbCommands.EditBands, ReportCommand.VerbEditBands);
			AddCommandIDReportCommandPair(VerbCommands.LoadReportTemplate, ReportCommand.VerbLoadReportTemplate);
			AddCommandIDReportCommandPair(VerbCommands.EditBindings, ReportCommand.VerbEditBindings);
			AddCommandIDReportCommandPair(VerbCommands.Import, ReportCommand.VerbImport);
			AddCommandIDReportCommandPair(VerbCommands.Export, ReportCommand.VerbExport);
			AddCommandIDReportCommandPair(VerbCommands.ExecuteVerb, ReportCommand.VerbExecute);
			AddCommandIDReportCommandPair(ZoomCommands.Zoom, ReportCommand.Zoom);
			AddCommandIDReportCommandPair(ZoomCommands.ZoomIn, ReportCommand.ZoomIn);
			AddCommandIDReportCommandPair(ZoomCommands.ZoomOut, ReportCommand.ZoomOut);
			AddCommandIDReportCommandPair(ReorderBandsCommands.MoveUp, ReportCommand.BandMoveUp);
			AddCommandIDReportCommandPair(ReorderBandsCommands.MoveDown, ReportCommand.BandMoveDown);
			AddCommandIDReportCommandPair(HtmlCommands.Home, ReportCommand.HtmlHome);
			AddCommandIDReportCommandPair(HtmlCommands.Backward, ReportCommand.HtmlBackward);
			AddCommandIDReportCommandPair(HtmlCommands.Forward, ReportCommand.HtmlForward);
			AddCommandIDReportCommandPair(HtmlCommands.Refresh, ReportCommand.HtmlRefresh);
			AddCommandIDReportCommandPair(HtmlCommands.Find, ReportCommand.HtmlFind);
			#endregion
		}
		public static CommandID GetCommandID(ReportCommand reportCommand) {
			return GetCommandID(reportCommand, null);
		}
		public static CommandID GetCommandID(ReportCommand reportCommand, CommandID defaultValue) {
			CommandID commandID;
			return commandIDHT.TryGetValue(reportCommand, out commandID) ? commandID : defaultValue;
		}
		public static ReportCommand GetReportCommand(CommandID commandID) {
			ReportCommand reportCommand;
			return commandID != null && reportCommandHT.TryGetValue(commandID, out reportCommand) ? 
				reportCommand : ReportCommand.None;
		}
		static void AddCommandIDReportCommandPair(CommandID commandID, ReportCommand reportCommand) {
			reportCommandHT.Add(commandID, reportCommand);
			commandIDHT.Add(reportCommand, commandID);
		}
	}
	#endregion
}
