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
using System.Collections.Specialized;
using System.Reflection;
using System.Text;
using DevExpress.Utils.Commands;
using DevExpress.XtraSpreadsheet;
using DevExpress.XtraSpreadsheet.Commands;
using DevExpress.XtraSpreadsheet.Internal;
using DevExpress.XtraSpreadsheet.Model;
using DevExpress.XtraSpreadsheet.Commands.Internal;
using DevExpress.XtraSpreadsheet.Forms;
using System.Drawing;
using System.Text.RegularExpressions;
using System.IO;
using System.Web;
using DevExpress.Export.Xl;
using DevExpress.Utils;
using DevExpress.XtraSpreadsheet.Localization;
using System.Collections;
using DevExpress.Web.Internal;
using DevExpress.Web.Office.Internal;
using System.Drawing.Printing;
namespace DevExpress.Web.ASPxSpreadsheet.Internal.Commands {
	public enum WebSpreadsheetCommandID {
		LoadSheet = 0,
		LoadInvisibleTiles,
		UpdateCell,
		FormatFontNameWebCommand,
		FormatFontSizeWebCommand,
		ResizeColumn,
		ResizeRow,
		FormatFontColor,
		FormatFillColor,
		FormatBorderLineColor,
		FormatBorderLineStyle,
		InsertHyperlink,
		FileNew,
		FileSave,
		InsertPicture,
		RenameSheet,
		EditHyperlink,
		ShapeMoveAndResize,
		FileOpen,
		FileSaveAs,
		UnhideRows,
		UnhideColumns,
		FormatRowHeight,
		FormatColumnWidth,
		FormatDefaultColumnWidth,
		UnhideSheet,
		ChartChangeType,
		ChartSelectData,
		GetLocalizedStringConstant,
		ModifyChartLayout,
		ChartChangeTitle,
		ChartChangeHorizontalAxisTitle,
		ChartChangeVerticalAxisTitle,
		ModifyChartStyle,
		DownLoadCopy,
		Print,
		FindAll,
		ScrollTo,
		SetPaperKind,
		Paste,
		GetPictureContent,
		InsertTable,
		FetchAutoFilterViewModel,
		ApplyAutoFilter,
		FetchDataValidationViewModel,
		ApplyDataValidation,
		FetchListAllowedValues,
		MoveOrCopySheetWebCommand,
		FetchMessageForCell,
		RenameTableWebCommand,
		ModifyTableStyleWebCommand,
		FormatAsTableWebCommand,
		InsertTableWithStyleWebCommand,
		RemoveSheet,
		PageSetupWebCommand,
		FetchPageSetupViewModel,
		ApplyPageSetupSettings,
		MoveRangeWebCommand,
		FreezePanesWebCommand,
		FreezeRowWebCommand,
		FreezeColumnWebCommand,
		AutoFitHeaderSize,
		None
	}
	public static class DefaultCommandGetResultImpl {
		public static DocumentHandlerResponse GetResult(SpreadsheetRenderHelper renderHelper) {
			JSONDocumentHandlerResponse documentResponse = new JSONDocumentHandlerResponse();
			documentResponse.ContentEncoding = RequestContentEncoding;
			documentResponse.ResponseResult = GetRenderResult(renderHelper);
			documentResponse.ContentType = RequestContentType;
			return documentResponse;
		}
		public static string GetRenderResult(SpreadsheetRenderHelper renderHelper) {
			return renderHelper.GetRenderResult();
		}
		public static string GetRenderResult(SpreadsheetRenderHelper renderHelper, Hashtable json) {
			return renderHelper.GetRenderResult(json);
		}
		public static string RequestContentType {
			get { return DocumentRequestManager.DefaultResponseContentType; }
		}
		public static Encoding RequestContentEncoding {
			get { return Encoding.UTF8; }
		}
	}
	public class WebSpreadsheetCommandWrapper {
		SpreadsheetCommand SpreadsheetCommand { get; set; }
		WebSpreadsheetCommandBase WebSpreadsheetCommand { get; set; }
		public WebSpreadsheetCommandWrapper(SpreadsheetCommand spreadsheetCommand) {
			SpreadsheetCommand = spreadsheetCommand;
		}
		public WebSpreadsheetCommandWrapper(WebSpreadsheetCommandBase webSpreadsheetCommand) {
			WebSpreadsheetCommand = webSpreadsheetCommand;
		}
		public void Execute() {
			if(SpreadsheetCommand != null)
				SpreadsheetCommand.Execute();
			else if(WebSpreadsheetCommand != null)
				WebSpreadsheetCommand.Execute();
		}
		public virtual DocumentHandlerResponse GetResult(SpreadsheetRenderHelper renderHelper) {
			if(WebSpreadsheetCommand != null)
				return WebSpreadsheetCommand.GetResult(renderHelper);
			return DefaultCommandGetResultImpl.GetResult(renderHelper);
		}
		public virtual string GetContentType() {
			if(WebSpreadsheetCommand != null)
				return WebSpreadsheetCommand.RequestContentType;
			return DefaultCommandGetResultImpl.RequestContentType;
		}
		public virtual Encoding GetContentEncoding() {
			if(WebSpreadsheetCommand != null)
				return WebSpreadsheetCommand.RequestContentEncoding;
			return DefaultCommandGetResultImpl.RequestContentEncoding;
		}
	}
	public class WebSpreadsheetCommandContext {
		SpreadsheetRenderHelper renderHelper;
		NameValueCollection requestParams;
		public WebSpreadsheetCommandContext(SpreadsheetRenderHelper renderHelper, NameValueCollection requestParams) {
			this.renderHelper = renderHelper;
			this.requestParams = requestParams;
		}
		public NameValueCollection RequestParams { get { return requestParams; } }
		public InnerSpreadsheetControl InnerControl { get { return RenderHelper.WorkSession.WebSpreadsheetControl.InnerControl; } }
		protected internal SpreadsheetRenderHelper RenderHelper { get { return renderHelper; } } 
	}
	public static class WebSpreadsheetCommands {
		public static readonly string CommandIDDefaultPrefix = "w";
		static readonly string CommandIDParamName = "CommandID";
		static readonly Type[] constructorParamTypes = new Type[] { typeof(WebSpreadsheetCommandContext) };
		static Dictionary<WebSpreadsheetCommandID, ConstructorInfo> CommandConstructors = new Dictionary<WebSpreadsheetCommandID, ConstructorInfo>();
		static WebSpreadsheetCommands() {
			PopulateCommandConstuctors();
		}
		public static WebSpreadsheetCommandWrapper GetCommandFromContext(SpreadsheetRenderHelper renderHelper) {
			string clientCommandID = renderHelper.CommandContext[CommandIDParamName];
			WebSpreadsheetCommandContext commandContext = new WebSpreadsheetCommandContext(renderHelper, renderHelper.CommandContext);
			WebSpreadsheetCommandWrapper command = CreateCommand(clientCommandID, commandContext);
			return command;
		}
		public static WebSpreadsheetCommandWrapper ProcessCommand(SpreadsheetRenderHelper renderHelper) {
			WebSpreadsheetCommandWrapper command = GetCommandFromContext(renderHelper);
			if(command != null)
				command.Execute();
			return command;
		}
		public static WebSpreadsheetCommandWrapper GetCommandWrapperByCommand(WebSpreadsheetCommandBase webCommand) {
			WebSpreadsheetCommandWrapper commandWrapper = new WebSpreadsheetCommandWrapper(webCommand);
			return commandWrapper;
		}
		public static WebSpreadsheetCommandWrapper GetCommandWrapperByCommand(SpreadsheetCommand spreadsheetCommand) {
			WebSpreadsheetCommandWrapper commandWrapper = new WebSpreadsheetCommandWrapper(spreadsheetCommand);
			return commandWrapper;
		}
		public static string GetWebCommandName(WebSpreadsheetCommandID webCommandID) {
			if(webCommandID != WebSpreadsheetCommandID.None)
				return CommandIDDefaultPrefix + ((int)webCommandID).ToString();
			return string.Empty;
		}
		static void PopulateCommandConstuctors() {
			AddWebCommandConstructor(WebSpreadsheetCommandID.LoadSheet, typeof(LoadSheetWebCommand));
			AddWebCommandConstructor(WebSpreadsheetCommandID.LoadInvisibleTiles, typeof(LoadInvisibleTilesWebCommand));
			AddWebCommandConstructor(WebSpreadsheetCommandID.UpdateCell, typeof(UpdateCellWebCommand));
			AddWebCommandConstructor(WebSpreadsheetCommandID.FormatFontNameWebCommand, typeof(FormatFontNameWebCommand));
			AddWebCommandConstructor(WebSpreadsheetCommandID.FormatFontSizeWebCommand, typeof(FormatFontSizeWebCommand));
			AddWebCommandConstructor(WebSpreadsheetCommandID.ResizeColumn, typeof(ResizeColumnWebCommand));
			AddWebCommandConstructor(WebSpreadsheetCommandID.ResizeRow, typeof(ResizeRowWebCommand));
			AddWebCommandConstructor(WebSpreadsheetCommandID.FormatFontColor, typeof(FormatFontColorWebCommand));
			AddWebCommandConstructor(WebSpreadsheetCommandID.FormatFillColor, typeof(FormatFillColorWebCommand));
			AddWebCommandConstructor(WebSpreadsheetCommandID.FormatBorderLineColor, typeof(FormatBorderLineColorWebCommand));
			AddWebCommandConstructor(WebSpreadsheetCommandID.FormatBorderLineStyle, typeof(FormatBorderLineStyleWebCommand));
			AddWebCommandConstructor(WebSpreadsheetCommandID.InsertHyperlink, typeof(InsertHyperLinkWebCommand));
			AddWebCommandConstructor(WebSpreadsheetCommandID.FileNew, typeof(FileNewWebCommand));
			AddWebCommandConstructor(WebSpreadsheetCommandID.FileSave, typeof(FileSaveWebCommand));
			AddWebCommandConstructor(WebSpreadsheetCommandID.InsertPicture, typeof(InsertPictureWebCommand));
			AddWebCommandConstructor(WebSpreadsheetCommandID.RenameSheet, typeof(RenameSheetWebCommand));
			AddWebCommandConstructor(WebSpreadsheetCommandID.EditHyperlink, typeof(InsertHyperLinkWebCommand));
			AddWebCommandConstructor(WebSpreadsheetCommandID.ShapeMoveAndResize, typeof(ShapeMoveAndResizeWebCommand));
			AddWebCommandConstructor(WebSpreadsheetCommandID.FileSaveAs, typeof(FileSaveAsCommand));
			AddWebCommandConstructor(WebSpreadsheetCommandID.FileOpen, typeof(FileOpenCommand));
			AddWebCommandConstructor(WebSpreadsheetCommandID.UnhideRows, typeof(UnhideRowsWebCommand));
			AddWebCommandConstructor(WebSpreadsheetCommandID.UnhideColumns, typeof(UnhideColumnsWebCommand));
			AddWebCommandConstructor(WebSpreadsheetCommandID.FormatRowHeight, typeof(FormatRowHeightWebCommand));
			AddWebCommandConstructor(WebSpreadsheetCommandID.FormatColumnWidth, typeof(FormatColumnWidthWebCommand));
			AddWebCommandConstructor(WebSpreadsheetCommandID.FormatDefaultColumnWidth, typeof(FormatDefaultColumnWidthWebCommand));
			AddWebCommandConstructor(WebSpreadsheetCommandID.UnhideSheet, typeof(UnhideSheetWebCommand));
			AddWebCommandConstructor(WebSpreadsheetCommandID.ChartChangeType, typeof(ChartChangeTypeSheetWebCommand));
			AddWebCommandConstructor(WebSpreadsheetCommandID.ChartSelectData, typeof(ChartSelectDataSheetWebCommand));
			AddWebCommandConstructor(WebSpreadsheetCommandID.GetLocalizedStringConstant, typeof(GetLocalizedStringConstantWebCommand));
			AddWebCommandConstructor(WebSpreadsheetCommandID.ModifyChartLayout, typeof(ModifyChartLayoutWebCommand));
			AddWebCommandConstructor(WebSpreadsheetCommandID.ChartChangeTitle, typeof(ChartChangeTitleWebCommand));
			AddWebCommandConstructor(WebSpreadsheetCommandID.ChartChangeHorizontalAxisTitle, typeof(ChartChangeHorizontalAxisTitleWebCommand));
			AddWebCommandConstructor(WebSpreadsheetCommandID.ChartChangeVerticalAxisTitle, typeof(ChartChangeVerticalAxisTitleWebCommand));
			AddWebCommandConstructor(WebSpreadsheetCommandID.ModifyChartStyle, typeof(ModifyChartStyleWebCommand));
			AddWebCommandConstructor(WebSpreadsheetCommandID.DownLoadCopy, typeof(DownLoadCopyWebCommand));
			AddWebCommandConstructor(WebSpreadsheetCommandID.Print, typeof(PrintWebCommand));
			AddWebCommandConstructor(WebSpreadsheetCommandID.FindAll, typeof(FindAllWebCommand));
			AddWebCommandConstructor(WebSpreadsheetCommandID.ScrollTo, typeof(ScrollToWebCommand));
			AddWebCommandConstructor(WebSpreadsheetCommandID.SetPaperKind, typeof(SetPaperKindWebCommand));
			AddWebCommandConstructor(WebSpreadsheetCommandID.Paste, typeof(PasteWebCommand));
			AddWebCommandConstructor(WebSpreadsheetCommandID.GetPictureContent, typeof(GetPictureContentCommand));
			AddWebCommandConstructor(WebSpreadsheetCommandID.InsertTable, typeof(InsertTableWebCommand));
			AddWebCommandConstructor(WebSpreadsheetCommandID.FetchAutoFilterViewModel, typeof(FetchAutoFilterViewModelWebCommand));
			AddWebCommandConstructor(WebSpreadsheetCommandID.ApplyAutoFilter, typeof(ApplyAutoFilterWebCommand));
			AddWebCommandConstructor(WebSpreadsheetCommandID.FetchDataValidationViewModel, typeof(FetchDataValidationViewModelWebCommand));
			AddWebCommandConstructor(WebSpreadsheetCommandID.ApplyDataValidation, typeof(ApplyDataValidationWebCommand));
			AddWebCommandConstructor(WebSpreadsheetCommandID.FetchListAllowedValues, typeof(FetchListAllowedValuesWebCommand));
			AddWebCommandConstructor(WebSpreadsheetCommandID.MoveOrCopySheetWebCommand, typeof(MoveOrCopySheetWebCommand));
			AddWebCommandConstructor(WebSpreadsheetCommandID.FetchMessageForCell, typeof(FetchMessageForCellWebCommand));
			AddWebCommandConstructor(WebSpreadsheetCommandID.RenameTableWebCommand, typeof(RenameTableWebCommand));
			AddWebCommandConstructor(WebSpreadsheetCommandID.ModifyTableStyleWebCommand, typeof(ModifyTableStyleWebCommand));
			AddWebCommandConstructor(WebSpreadsheetCommandID.FormatAsTableWebCommand, typeof(FormatAsTableWebCommand));
			AddWebCommandConstructor(WebSpreadsheetCommandID.InsertTableWithStyleWebCommand, typeof(InsertTableWithStyleWebCommand));
			AddWebCommandConstructor(WebSpreadsheetCommandID.RemoveSheet, typeof(RemoveSheetWebCommand));
			AddWebCommandConstructor(WebSpreadsheetCommandID.PageSetupWebCommand, typeof(PageSetupWebCommand));
			AddWebCommandConstructor(WebSpreadsheetCommandID.FetchPageSetupViewModel, typeof(FetchPageSetupViewModel));
			AddWebCommandConstructor(WebSpreadsheetCommandID.ApplyPageSetupSettings, typeof(ApplyPageSetupSettingsWebCommand));
			AddWebCommandConstructor(WebSpreadsheetCommandID.MoveRangeWebCommand, typeof(MoveRangeWebCommand));
			AddWebCommandConstructor(WebSpreadsheetCommandID.FreezePanesWebCommand, typeof(FreezePanesWebCommand));
			AddWebCommandConstructor(WebSpreadsheetCommandID.FreezeRowWebCommand, typeof(FreezeRowWebCommand));
			AddWebCommandConstructor(WebSpreadsheetCommandID.FreezeColumnWebCommand, typeof(FreezeColumnWebCommand));
			AddWebCommandConstructor(WebSpreadsheetCommandID.AutoFitHeaderSize, typeof(AutoFitHeaderSizeWebCommand));
		}
		static WebSpreadsheetCommandWrapper CreateCommand(string clientCommandId, WebSpreadsheetCommandContext commandContext) {
			bool isWebSpreadsheetCommand = clientCommandId.StartsWith(CommandIDDefaultPrefix);
			if(isWebSpreadsheetCommand)
				clientCommandId = clientCommandId.Substring(1);
			int commandId = Int32.Parse(clientCommandId);
			if(isWebSpreadsheetCommand)
				return CreateWebCommand(commandId, commandContext);
			else
				return CreateCommand(commandId, commandContext);
		}
		static WebSpreadsheetCommandWrapper CreateWebCommand(int commandIDValue, WebSpreadsheetCommandContext commandContext) {
			WebSpreadsheetCommandID commandID = (WebSpreadsheetCommandID)commandIDValue;
			if(!CommandConstructors.ContainsKey(commandID))
				return null;
			var constuctorInfo = CommandConstructors[commandID];
			WebSpreadsheetCommandBase webCommand = (WebSpreadsheetCommandBase)constuctorInfo.Invoke(new object[] { commandContext });
			return new WebSpreadsheetCommandWrapper(webCommand);
		}
		static WebSpreadsheetCommandWrapper CreateCommand(int commandId, WebSpreadsheetCommandContext commandContext) {
			SpreadsheetCommandId spreadsheetCommandId = new SpreadsheetCommandId(commandId);
			SpreadsheetCommand spreadsheetCommand = commandContext.InnerControl.CreateCommand(spreadsheetCommandId);
			return new WebSpreadsheetCommandWrapper(spreadsheetCommand);
		}
		static void AddWebCommandConstructor(WebSpreadsheetCommandID commandID, Type commandType) {
			CommandConstructors.Add(commandID, commandType.GetConstructor(constructorParamTypes));
		}
	}
	public abstract class WebSpreadsheetCommandBase {
		public WebSpreadsheetCommandBase(WebSpreadsheetCommandContext commandContext) {
			RenderHelper = commandContext.RenderHelper;
			RequestParams = commandContext.RequestParams;
		}
		public SpreadsheetRenderHelper RenderHelper { get; private set; }
		public NameValueCollection RequestParams { get; private set; }
		public DocumentModel Model { get { return RenderHelper.Model; } }
		public Worksheet ActiveSheet { get { return Model.ActiveSheet; } }
		public InnerSpreadsheetControl InnerControl { get { return RenderHelper.WorkSession.WebSpreadsheetControl.InnerControl; } }
		public bool IsDocumentEditable() {
			return InnerControl != null && InnerControl.IsEnabled && !InnerControl.IsReadOnly;
		}
		protected virtual bool CommandExecuteAllowed() {
			return IsDocumentEditable();
		}
		public virtual string RequestContentType {
			get { return DocumentRequestManager.DefaultResponseContentType; }
		}
		public virtual Encoding RequestContentEncoding {
			get { return Encoding.UTF8; }
		}
		public virtual void Execute() {
			if(CommandExecuteAllowed()) {
				OnBeforeCommandExecuted();
				SpreadsheetCommand spreadsheetCommand = CreateSpreadsheetCommand();
				if(IsCustomExecuteImplementation(spreadsheetCommand)) {
					CustomExecuteImplementation();
				} else {
					ICommandUIState commandState = CreateCommandState(spreadsheetCommand);
					ExecuteCore(spreadsheetCommand, commandState);
				}
				OnAfterCommandExecuted();
			}
		}
		protected virtual void OnBeforeCommandExecuted() { }
		protected virtual void OnAfterCommandExecuted() { }
		protected void ExecuteCore(SpreadsheetCommand spreadsheetCommand, ICommandUIState commandState) {
			if(spreadsheetCommand.CanExecute())
				spreadsheetCommand.ForceExecute(commandState);
		}
		protected abstract void CustomExecuteImplementation();
		protected abstract SpreadsheetCommand CreateSpreadsheetCommand();
		protected abstract ICommandUIState CreateCommandState(SpreadsheetCommand spreadsheetCommand);
		protected bool IsCustomExecuteImplementation(SpreadsheetCommand spreadsheetCommand) {
			return spreadsheetCommand == null;
		}
		public virtual DocumentHandlerResponse GetResult(SpreadsheetRenderHelper renderHelper) {
			JSONDocumentHandlerResponse documentResponse = new JSONDocumentHandlerResponse();
			documentResponse.ContentEncoding = RequestContentEncoding;
			documentResponse.ResponseResult = DefaultCommandGetResultImpl.GetRenderResult(renderHelper, GetResponseJSONHashTable());
			documentResponse.ContentType = RequestContentType;
			return documentResponse;
		}
		public virtual Hashtable GetResponseJSONHashTable() {
			return new Hashtable();
		}
	}
	public abstract class CustomCommandExecuteImplementationBase : WebSpreadsheetCommandBase {
		public CustomCommandExecuteImplementationBase(WebSpreadsheetCommandContext commandContext)
			: base(commandContext) {
		}
		protected override SpreadsheetCommand CreateSpreadsheetCommand() {
			return null;
		}
		protected override ICommandUIState CreateCommandState(SpreadsheetCommand spreadsheetCommand) {
			return null;
		}
	}
	public abstract class StandardCommandExecuteImplementationBase : WebSpreadsheetCommandBase {
		public StandardCommandExecuteImplementationBase(WebSpreadsheetCommandContext commandContext)
			: base(commandContext) {
		}
		protected override void CustomExecuteImplementation() {
		}
	}
	public abstract class WorkSessionChangingCommandExecuteImplementationBase : StandardCommandExecuteImplementationBase {
		public WorkSessionChangingCommandExecuteImplementationBase(WebSpreadsheetCommandContext commandContext)
			: base(commandContext) {
		}
		public override Hashtable GetResponseJSONHashTable() {
			var json = new Hashtable();
			json[ResponseQueryStringKeys.NewWorkSessionGuid] = RenderHelper.WorkSession.ID;
			return json;
		}
	}
	public abstract class CellChangingWebCommandBase : CustomCommandExecuteImplementationBase {
		protected const string
			CellPositionColumnParamName = "CellPositionColumn",
			CellPositionRowParamName = "CellPositionRow";
		public CellChangingWebCommandBase(WebSpreadsheetCommandContext context)
			: base(context) {
		}
		protected CellPosition CellPosition {
			get {
				string strColumn = RequestParams[CellPositionColumnParamName];
				string strRow = RequestParams[CellPositionRowParamName];
				return new CellPosition(int.Parse(strColumn), int.Parse(strRow));
			}
		}
		protected override bool CommandExecuteAllowed() {
			return base.CommandExecuteAllowed() && CanEditCurrentCell;
		}
		protected bool CanEditCurrentCell {
			get { return ProtectionResolver.CanEditCellContent(ActiveSheet, CellPosition); }
		}
	}
	public class UpdateCellWebCommand : CellChangingWebCommandBase {
		protected const string NewValueParamName = "NewValue";
		protected const string ConfirmResultParamName = "ConfirmResult";
		public UpdateCellWebCommand(WebSpreadsheetCommandContext context)
			: base(context) {
		}
		protected CellInplaceEditorCommitControllerOwner CommitterControllerOwner { get; private set; }
		protected CellInplaceEditorCommitResult CommitResult { get; private set; }
		protected string NewCellValue { get { return HttpUtility.HtmlDecode(RequestParams[NewValueParamName]); } }
		protected string ConfirmResult { get { return HttpUtility.HtmlDecode(RequestParams[ConfirmResultParamName]); } }
		protected Hashtable GetConfirmParams() {
			var confirmParams = new Hashtable();
			confirmParams.Add("cell", new DevExpress.Web.ASPxSpreadsheet.Internal.JSONTypes.JSONCellPosition(CellPosition.Column, CellPosition.Row));
			confirmParams.Add("title", CommitterControllerOwner.ConfirmTitle);
			confirmParams.Add("msg", CommitterControllerOwner.ConfirmMessage);
			confirmParams.Add("value", NewCellValue);
			confirmParams.Add("type", (int)CommitterControllerOwner.ErrorStyle);
			return confirmParams;
		}
		protected override void CustomExecuteImplementation() {
			CommitterControllerOwner = new CellInplaceEditorCommitControllerOwner(ConfirmResult);
			var cell = new Cell(CellPosition, ActiveSheet);
			var commitController = new CellInplaceEditorCommitController(CommitterControllerOwner, cell);
			CommitResult = commitController.Commit(NewCellValue);
		}
		public override Hashtable GetResponseJSONHashTable() {
			var json = new Hashtable();
			if((!CommitResult.Success || CommitterControllerOwner.ErrorStyle == DataValidationErrorStyle.Stop) && !string.IsNullOrEmpty(CommitterControllerOwner.ConfirmMessage))
				json[ResponseQueryStringKeys.ValidationConfirm] = GetConfirmParams();
			return json;
		}
	}
	public class LoadSheetWebCommand : CustomCommandExecuteImplementationBase {
		public LoadSheetWebCommand(WebSpreadsheetCommandContext context)
			: base(context) {
		}
		protected override bool CommandExecuteAllowed() {
			return true;
		}
		protected override void CustomExecuteImplementation() {
		}
	}
	public class LoadInvisibleTilesWebCommand : CustomCommandExecuteImplementationBase {
		public LoadInvisibleTilesWebCommand(WebSpreadsheetCommandContext context)
			: base(context) {
		}
		protected override void CustomExecuteImplementation() { }
	}
	public class FormatFontNameWebCommand : StandardCommandExecuteImplementationBase {
		protected const string FontNameParamName = "FontName";
		public FormatFontNameWebCommand(WebSpreadsheetCommandContext context)
			: base(context) {
		}
		protected string FontName { get { return RequestParams[FontNameParamName]; } }
		protected override SpreadsheetCommand CreateSpreadsheetCommand() {
			return InnerControl.CreateCommand(SpreadsheetCommandId.FormatFontName);
		}
		protected override ICommandUIState CreateCommandState(SpreadsheetCommand spreadsheetCommand) {
			DefaultValueBasedCommandUIState<String> state = new DefaultValueBasedCommandUIState<String>();
			state.Value = FontName;
			return state;
		}
	}
	public class FormatFontSizeWebCommand : StandardCommandExecuteImplementationBase {
		protected const string FontSizeParamName = "FontSize";
		public FormatFontSizeWebCommand(WebSpreadsheetCommandContext context)
			: base(context) {
		}
		protected double FontSize { get { return double.Parse(RequestParams[FontSizeParamName]); } }
		protected override SpreadsheetCommand CreateSpreadsheetCommand() {
			return InnerControl.CreateCommand(SpreadsheetCommandId.FormatFontSize);
		}
		protected override ICommandUIState CreateCommandState(SpreadsheetCommand spreadsheetCommand) {
			var state = new DefaultValueBasedCommandUIState<double>();
			state.Value = FontSize;
			return state;
		}
	}
	public abstract class WebSpreadsheetStateCommandBase<T> : StandardCommandExecuteImplementationBase {
		string parameterName;
		SpreadsheetCommandId spreadsheetCommandId;
		public WebSpreadsheetStateCommandBase(WebSpreadsheetCommandContext context, string parameterName, SpreadsheetCommandId spreadsheetCommandId)
			: base(context) {
			this.parameterName = parameterName;
			this.spreadsheetCommandId = spreadsheetCommandId;
		}
		public string ParameterName { get { return parameterName; } }
		public SpreadsheetCommandId SpreadsheetCommandId { get { return spreadsheetCommandId; } }
		protected T Value {
			get {
				string clientValue = string.IsNullOrEmpty(ParameterName) ? null : RequestParams[ParameterName];
				return ConvertParameter(clientValue);
			}
		}
		protected abstract T ConvertParameter(string value);
		protected override SpreadsheetCommand CreateSpreadsheetCommand() {
			return InnerControl.CreateCommand(SpreadsheetCommandId);
		}
		protected override ICommandUIState CreateCommandState(SpreadsheetCommand spreadsheetCommand) {
			var state = new DefaultValueBasedCommandUIState<T>();
			state.Value = Value;
			return state;
		}
	}
	public abstract class WebSpreadsheetColorCommandBase : WebSpreadsheetStateCommandBase<Color> {
		public WebSpreadsheetColorCommandBase(WebSpreadsheetCommandContext context, string parameterName, SpreadsheetCommandId spreadsheetCommandId)
			: base(context, parameterName, spreadsheetCommandId) {
		}
		protected override Color ConvertParameter(string clientColor) {
			return WebColorUrils.ColorFromHexColor(clientColor);
		}
	}
	public class FormatFontColorWebCommand : WebSpreadsheetColorCommandBase {
		protected const string FontColorParamName = "FormatFontColor";
		public FormatFontColorWebCommand(WebSpreadsheetCommandContext context)
			: base(context, FontColorParamName, SpreadsheetCommandId.FormatFontColor) {
		}
	}
	public class FormatFillColorWebCommand : WebSpreadsheetColorCommandBase {
		protected const string FillColorParamName = "FormatFillColor";
		public FormatFillColorWebCommand(WebSpreadsheetCommandContext context)
			: base(context, FillColorParamName, SpreadsheetCommandId.FormatFillColor) {
		}
	}
	public class FormatBorderLineColorWebCommand : WebSpreadsheetColorCommandBase {
		protected const string BorderLineColorParamName = "FormatBorderLineColor";
		public FormatBorderLineColorWebCommand(WebSpreadsheetCommandContext context)
			: base(context, BorderLineColorParamName, SpreadsheetCommandId.FormatBorderLineColor) {
		}
	}
	public class FormatBorderLineStyleWebCommand : WebSpreadsheetStateCommandBase<XlBorderLineStyle> {
		protected const string BorderLineStyleParamName = "FormatBorderLineStyle";
		public FormatBorderLineStyleWebCommand(WebSpreadsheetCommandContext context)
			: base(context, BorderLineStyleParamName, SpreadsheetCommandId.FormatBorderLineStyle) {
		}
		protected override XlBorderLineStyle ConvertParameter(string clientBorderLineStyle) {
			return (XlBorderLineStyle)Enum.Parse(typeof(XlBorderLineStyle), clientBorderLineStyle);
		}
	}
	public class SetPaperKindWebCommand : WebSpreadsheetStateCommandBase<PaperKind> {
		protected const string PaperKindParamName = "PaperKind";
		public SetPaperKindWebCommand(WebSpreadsheetCommandContext context)
			: base(context, PaperKindParamName, SpreadsheetCommandId.PageSetupSetPaperKind) {
		}
		protected override PaperKind ConvertParameter(string clientPaperKind) {
			return (PaperKind)Enum.Parse(typeof(PaperKind), clientPaperKind);
		}
	}
	public class PageSetupWebCommand : WebSpreadsheetCommandBase {
		protected const string ViewModelParamName = "ViewModel";
		public PageSetupWebCommand(WebSpreadsheetCommandContext context)
			: base(context) {
		}
		protected override void CustomExecuteImplementation() {
			throw new NotImplementedException();
		}
		protected override SpreadsheetCommand CreateSpreadsheetCommand() {
			throw new NotImplementedException();
		}
		protected override ICommandUIState CreateCommandState(SpreadsheetCommand spreadsheetCommand) {
			throw new NotImplementedException();
		}
	}
	public abstract class PageSetupWebCommandBase : CustomCommandExecuteImplementationBase {
		protected ShowPageSetupCommand PageSetupCommand {
			get {
				return InnerControl.CreateCommand(SpreadsheetCommandId.PageSetup) as ShowPageSetupCommand;
			}
		}
		private PageSetupViewModel coreViewModel;
		protected PageSetupViewModel CoreViewModel {
			get {
				if(this.coreViewModel == null)
					this.coreViewModel = CreatePageSetupViewModel();
				return this.coreViewModel;
			}
		}
		public PageSetupWebCommandBase(WebSpreadsheetCommandContext context)
			: base(context) {
		}
		protected PageSetupViewModel CreatePageSetupViewModel() {
			var appPath = HttpUtils.GetRequest().ServerVariables["APPL_PHYSICAL_PATH"];
			var documentSaveOptions = Model.DocumentSaveOptions;
			var currentFileName = documentSaveOptions.CurrentFileName;
			documentSaveOptions.CurrentFileName = documentSaveOptions.CurrentFileName.Replace(appPath, string.Empty);
			var viewModel = PageSetupCommand.CreateViewModel();
			documentSaveOptions.CurrentFileName = currentFileName;
			return viewModel;
		}
		protected override void CustomExecuteImplementation() { }
	}
	public class FetchPageSetupViewModel : PageSetupWebCommandBase {
		public FetchPageSetupViewModel(WebSpreadsheetCommandContext context)
			: base(context) {
		}
		protected string CustomResultProcessingFunction {
			get {
				return SpreadSheetCustomResultProcessingFunctionHelper.GetCustomResultProcessingFunctionNameFromParams(RequestParams);
			}
		}
		public override DocumentHandlerResponse GetResult(SpreadsheetRenderHelper renderHelper) {
			var result = new Hashtable();
			result["pageSetupViewModel"] = GetPageSetupViewModelHashTable();
			SpreadSheetCustomResultProcessingFunctionHelper.SetCustomResultProcessingFunction(result, CustomResultProcessingFunction);
			JSONDocumentHandlerResponse documentResponse = new JSONDocumentHandlerResponse();
			documentResponse.ContentEncoding = RequestContentEncoding;
			documentResponse.ResponseResult = HtmlConvertor.ToJSON(result);
			documentResponse.ContentType = RequestContentType;
			return documentResponse;
		}
		protected Hashtable GetPageSetupViewModelHashTable() {
			Hashtable result = new Hashtable();
			result["DataSource"] = GetDataSourceHashTable();
			result["rlOrientationPortrait"] = CoreViewModel.OrientationPortrait ? 0 : 1;
			result["seScale"] = CoreViewModel.Scale;
			result["rbFitToPage"] = CoreViewModel.FitToPage;
			result["rbAdjustTo"] = !CoreViewModel.FitToPage;
			result["seFitToWidth"] = CoreViewModel.FitToWidth;
			result["seFitToHeight"] = CoreViewModel.FitToHeight;
			result["cbPaperType"] = CoreViewModel.PaperType;
			result["cbPrintQualityMode"] = CoreViewModel.PrintQualityMode;
			result["tbFirstPageNumber"] = CoreViewModel.FirstPageNumber;
			result["seHeaderMargin"] = CoreViewModel.HeaderMargin;
			result["seFooterMargin"] = CoreViewModel.FooterMargin;
			result["seTopMargin"] = CoreViewModel.TopMargin;
			result["seBottomMargin"] = CoreViewModel.BottomMargin;
			result["seLeftMargin"] = CoreViewModel.LeftMargin;
			result["seRightMargin"] = CoreViewModel.RightMargin;
			result["ckCenterHorizontally"] = CoreViewModel.CenterHorizontally;
			result["ckCenterVertically"] = CoreViewModel.CenterVertically;
			result["meOddHeader"] = CoreViewModel.OddHeader;
			result["meOddFooter"] = CoreViewModel.OddFooter;
			result["meEvenHeader"] = CoreViewModel.EvenHeader;
			result["meEvenFooter"] = CoreViewModel.EvenFooter;
			result["meFirstHeader"] = CoreViewModel.FirstHeader;
			result["meFirstFooter"] = CoreViewModel.FirstFooter;
			result["cbHeader"] = CoreViewModel.PredefinedHeaderValueForList;
			result["cbFooter"] = CoreViewModel.PredefinedFooterValueForList;
			result["ckDifferentOddEven"] = CoreViewModel.DifferentOddEven;
			result["ckDifferentFirstPage"] = CoreViewModel.DifferentFirstPage;
			result["ckScaleWithDocument"] = CoreViewModel.ScaleWithDocument;
			result["ckAlignWithMargins"] = CoreViewModel.AlignWithMargins;
			result["tbPrintArea"] = CoreViewModel.PrintArea;
			result["ckPrintGridlines"] = CoreViewModel.PrintGridlines;
			result["ckBlackAndWhite"] = CoreViewModel.BlackAndWhite;
			result["ckDraft"] = CoreViewModel.Draft;
			result["ckPrintHeadings"] = CoreViewModel.PrintHeadings;
			result["cbCommentsPrintMode"] = CoreViewModel.CommentsPrintMode;
			result["cbErrorsPrintMode"] = CoreViewModel.ErrorsPrintMode;
			result["rlDownThenOver"] = CoreViewModel.DownThenOver ? 0 : 1;
			return result;
		}
		protected Hashtable GetDataSourceHashTable() {
			var dataSource = new Hashtable();
			dataSource["cbHeader"] = CoreViewModel.PredefinedHeaderFooterList;
			dataSource["cbFooter"] = CoreViewModel.PredefinedHeaderFooterList;
			dataSource["cbCommentsPrintMode"] = CoreViewModel.CommentsPrintModeList;
			dataSource["cbErrorsPrintMode"] = CoreViewModel.ErrorsPrintModeList;
			return dataSource;
		}
	}
	public class ApplyPageSetupSettingsWebCommand : PageSetupWebCommandBase {
		protected const string ViewModelParamName = "pageSetupViewModel";
		protected string RequestViewModel {
			get { return RequestParams[ViewModelParamName]; }
		}
		protected Hashtable ClientViewModel {
			get { return HtmlConvertor.FromJSON<Hashtable>(RequestViewModel); }
		}
		public ApplyPageSetupSettingsWebCommand(WebSpreadsheetCommandContext context)
			: base(context) {
		}
		protected PropertyInfo GetCorePropertyInfo(string name) {
			return CoreViewModel.GetType().GetProperty(name);
		}
		protected object GetCorePropertyValue(string name) {
			return GetCorePropertyInfo(name).GetValue(CoreViewModel, null);
		}
		protected void SetCorePropertyValue(string name, object value) {
			GetCorePropertyInfo(name).SetValue(CoreViewModel, value, null);
		}
		protected override void CustomExecuteImplementation() {
			base.CustomExecuteImplementation();
			CoreViewModel.OrientationPortrait = ClientViewModel["rlOrientationPortrait"].ToString() == "Portrait";
			CoreViewModel.Scale = Convert.ToInt32(ClientViewModel["seScale"]);
			CoreViewModel.FitToPage = Convert.ToBoolean(ClientViewModel["rbFitToPage"]);
			CoreViewModel.FitToWidth = Convert.ToInt32(ClientViewModel["seFitToWidth"]);
			CoreViewModel.FitToHeight = Convert.ToInt32(ClientViewModel["seFitToHeight"]);
			CoreViewModel.PaperType = ConvertPaperKind(ClientViewModel["cbPaperType"]);
			CoreViewModel.PrintQualityMode = ClientViewModel["cbPrintQualityMode"].ToString();
			CoreViewModel.FirstPageNumber = ClientViewModel["tbFirstPageNumber"].ToString();
			CoreViewModel.HeaderMargin = Convert.ToSingle(ClientViewModel["seHeaderMargin"]);
			CoreViewModel.FooterMargin = Convert.ToSingle(ClientViewModel["seFooterMargin"]);
			CoreViewModel.TopMargin = Convert.ToSingle(ClientViewModel["seTopMargin"]);
			CoreViewModel.BottomMargin = Convert.ToSingle(ClientViewModel["seBottomMargin"]);
			CoreViewModel.LeftMargin = Convert.ToSingle(ClientViewModel["seLeftMargin"]);
			CoreViewModel.RightMargin = Convert.ToSingle(ClientViewModel["seRightMargin"]);
			CoreViewModel.CenterHorizontally = Convert.ToBoolean(ClientViewModel["ckCenterHorizontally"]);
			CoreViewModel.CenterVertically = Convert.ToBoolean(ClientViewModel["ckCenterVertically"]);
			CoreViewModel.PredefinedHeaderValueForList = ClientViewModel["cbHeader"].ToString();
			CoreViewModel.PredefinedFooterValueForList = ClientViewModel["cbFooter"].ToString();
			CoreViewModel.DifferentOddEven = Convert.ToBoolean(ClientViewModel["ckDifferentOddEven"]);
			CoreViewModel.DifferentFirstPage = Convert.ToBoolean(ClientViewModel["ckDifferentFirstPage"]);
			CoreViewModel.ScaleWithDocument = Convert.ToBoolean(ClientViewModel["ckScaleWithDocument"]);
			CoreViewModel.AlignWithMargins = Convert.ToBoolean(ClientViewModel["ckAlignWithMargins"]);
			CoreViewModel.PrintArea = ClientViewModel["tbPrintArea"] != null ? ClientViewModel["tbPrintArea"].ToString() : string.Empty;
			CoreViewModel.PrintGridlines = Convert.ToBoolean(ClientViewModel["ckPrintGridlines"]);
			CoreViewModel.BlackAndWhite = Convert.ToBoolean(ClientViewModel["ckBlackAndWhite"]);
			CoreViewModel.Draft = Convert.ToBoolean(ClientViewModel["ckDraft"]);
			CoreViewModel.PrintHeadings = Convert.ToBoolean(ClientViewModel["ckPrintHeadings"]);
			CoreViewModel.CommentsPrintMode = ClientViewModel["cbCommentsPrintMode"].ToString();
			CoreViewModel.ErrorsPrintMode = ClientViewModel["cbErrorsPrintMode"].ToString();
			CoreViewModel.DownThenOver = ClientViewModel["rlDownThenOver"].ToString() == "Down, then over";
			CoreViewModel.ApplyChanges();
		}
		protected PaperKind ConvertPaperKind(object clientPaperKind) {
			return (PaperKind)Enum.Parse(typeof(PaperKind), clientPaperKind.ToString());
		}
	}
	public class ResizeColumnWebCommand : StandardCommandExecuteImplementationBase {
		protected const string
			ColumnIndexParamName = "ColumnIndex",
			WidthParamName = "ColumnWidth";
		public ResizeColumnWebCommand(WebSpreadsheetCommandContext context)
			: base(context) {
		}
		protected int ColumnIndex { get { return Convert.ToInt32(RequestParams[ColumnIndexParamName]); } }
		protected int Width {
			get {
				int value = Convert.ToInt32(RequestParams[WidthParamName]);
				return value > 0 ? value : 0;
			}
		}
		protected float WidthInCharacters {
			get {
				var service = Model.GetService<IColumnWidthCalculationService>();
				var widthInCharacters = service.ConvertLayoutsToCharacters(ActiveSheet, Width, Width);
				return service.RemoveGaps(ActiveSheet, widthInCharacters);
			}
		}
		protected override bool CommandExecuteAllowed() {
			return base.CommandExecuteAllowed() && !ProtectionResolver.SheetLocked(ActiveSheet);
		}
		protected override SpreadsheetCommand CreateSpreadsheetCommand() {
			return new ResizeColumnCommand(InnerControl.Owner);
		}
		protected override ICommandUIState CreateCommandState(SpreadsheetCommand command) {
			var argument = new ResizeColumnCommandArgument();
			argument.ColumnIndex = ColumnIndex;
			argument.WidthInCharacters = CorrectWidth(WidthInCharacters);
			argument.IsFirstColumn = ColumnIndex == 0; 
			var state = (IValueBasedCommandUIState<ResizeColumnCommandArgument>)command.CreateDefaultCommandUIState();
			state.EditValue = argument;
			return state;
		}
		protected float CorrectWidth(float widthInCharacters) {
			return Math.Max(0, Math.Min(255, widthInCharacters));
		}
	}
	public class ResizeRowWebCommand : StandardCommandExecuteImplementationBase {
		protected const string
			RowIndexParamName = "RowIndex",
			HeightParamName = "RowHeight";
		public ResizeRowWebCommand(WebSpreadsheetCommandContext context)
			: base(context) {
		}
		protected int RowIndex { get { return Convert.ToInt32(RequestParams[RowIndexParamName]); } }
		protected int Height {
			get {
				int value = Convert.ToInt32(RequestParams[HeightParamName]);
				return value > 0 ? value : 0;
			}
		}
		protected int ModelHeight { get { return Model.ToDocumentLayoutUnitConverter.ToModelUnits(Height); } }
		protected override bool CommandExecuteAllowed() {
			return base.CommandExecuteAllowed() && !ProtectionResolver.SheetLocked(ActiveSheet);
		}
		protected override SpreadsheetCommand CreateSpreadsheetCommand() {
			return new ResizeRowCommand(InnerControl.Owner);
		}
		protected override ICommandUIState CreateCommandState(SpreadsheetCommand spreadsheetCommand) {
			var argument = new ResizeRowCommandArgument();
			argument.RowIndex = RowIndex;
			argument.Height = CorrectHeight(ModelHeight);
			argument.IsFirstRow = RowIndex == 0; 
			var state = (IValueBasedCommandUIState<ResizeRowCommandArgument>)spreadsheetCommand.CreateDefaultCommandUIState();
			state.EditValue = argument;
			return state;
		}
		protected float CorrectHeight(int height) {
			int maxHeight = InnerControl.DocumentModel.UnitConverter.TwipsToModelUnits(Row.MaxHeightInTwips);
			return Math.Max(0, Math.Min(maxHeight, height));
		}
	}
	public static class WebColorUrils {
		public static Color ColorFromHexColor(string hex) {
			Regex regExp = new Regex("^#?([a-f]|[A-F]|[0-9]){3}(([a-f]|[A-F]|[0-9]){3})?$");
			if(regExp.IsMatch(hex)) {
				hex = hex.Replace("#", "");
				if(hex.Length == 3) {
					string newHex = "";
					for(int i = 0; i < 3; i++)
						newHex += hex.Substring(i, 1) + hex.Substring(i, 1);
					hex = newHex;
				}
				byte r = 0, g = 0, b = 0;
				int start = 0;
				r = byte.Parse(hex.Substring(start, 2), System.Globalization.NumberStyles.HexNumber);
				g = byte.Parse(hex.Substring(start + 2, 2), System.Globalization.NumberStyles.HexNumber);
				b = byte.Parse(hex.Substring(start + 4, 2), System.Globalization.NumberStyles.HexNumber);
				return Color.FromArgb(0xFF, r, g, b);
			}
			return Color.Empty;
		}
	}
	public class CellInplaceEditorCommitControllerOwner : ICellInplaceEditorCommitControllerOwner {
		public string ConfirmResult { get; private set; }
		public string ConfirmMessage { get; private set; }
		public string ConfirmTitle { get; private set; }
		public DataValidationErrorStyle ErrorStyle { get; private set; }
		public CellInplaceEditorCommitControllerOwner(string confirmResult) {
			ConfirmResult = confirmResult;
		}
		public void RaiseCellValueChanged(SpreadsheetCellEventArgs args) {
		}
		public System.Windows.Forms.DialogResult ShowDataValidationDialog(string text, string message, string title, DataValidationErrorStyle errorStyle) {
			ConfirmMessage = message;
			ConfirmTitle = title == System.Windows.Forms.Application.ProductName ? "" : title;
			ErrorStyle = errorStyle;
			if(ConfirmResult == "OK")
				return System.Windows.Forms.DialogResult.OK;
			if(ConfirmResult == "Yes")
				return System.Windows.Forms.DialogResult.Yes;
			if(errorStyle == DataValidationErrorStyle.Stop)
				return System.Windows.Forms.DialogResult.Cancel;
			return System.Windows.Forms.DialogResult.None;
		}
	}
	public class InsertHyperLinkWebCommand : StandardCommandExecuteImplementationBase {
		protected const string
			HyperLinkDisplayTextParamName = "HyperLinkDisplayText",
			HyperLinkScreenTipParamName = "HyperLinkScreenTip",
			HyperLinkUrlAddressParamName = "HyperLinkUrlAddress";
		public InsertHyperLinkWebCommand(WebSpreadsheetCommandContext context)
			: base(context) {
		}
		protected string HyperLinkUrlAddress { get { return RequestParams[HyperLinkUrlAddressParamName]; } }
		protected string HyperLinkDisplayText { get { return RequestParams[HyperLinkDisplayTextParamName]; } }
		protected string HyperLinkScreenTip { get { return RequestParams[HyperLinkScreenTipParamName]; } }
		protected override SpreadsheetCommand CreateSpreadsheetCommand() {
			return InnerControl.CreateCommand(SpreadsheetCommandId.InsertHyperlink);
		}
		protected override ICommandUIState CreateCommandState(SpreadsheetCommand spreadsheetCommand) {
			InsertHyperlinkCommand hyperlink = new InsertHyperlinkCommand(InnerControl.ErrorHandler, ActiveSheet, ActiveSheet.Selection.ActiveRange, HyperLinkUrlAddress, true, HyperLinkDisplayText, false);
			hyperlink.TooltipText = HyperLinkScreenTip;
			var state = new DefaultValueBasedCommandUIState<IHyperlinkViewInfo>();
			state.Value = hyperlink;
			return state;
		}
	}
	public class InsertPictureWebCommand : StandardCommandExecuteImplementationBase {
		private Stream imageStream = null;
		public const string PicturePathParamName = "PicturePath";
		public Stream ImageStream {
			get {
				return imageStream;
			}
			private set {
				imageStream = value;
			}
		}
		public InsertPictureWebCommand(WebSpreadsheetCommandContext context)
			: base(context) {
		}
		public InsertPictureWebCommand(WebSpreadsheetCommandContext context, Stream fileStream)
			: base(context) {
			ImageStream = fileStream;
		}
		protected string PicturePath { get { return RequestParams[PicturePathParamName] ?? string.Empty; } }
		protected override SpreadsheetCommand CreateSpreadsheetCommand() {
			return InnerControl.CreateCommand(SpreadsheetCommandId.InsertPicture);
		}
		protected override ICommandUIState CreateCommandState(SpreadsheetCommand spreadsheetCommand) {
			var state = new DefaultValueBasedCommandUIState<InsertPictureCommandParameters>();
			InsertPictureCommandParameters insertedImageFileInfo = new InsertPictureCommandParameters();
			if(ImageStream != null) {
				insertedImageFileInfo.Stream = ImageStream;
				insertedImageFileInfo.FileName = PicturePath;
			} else {
				insertedImageFileInfo.FileName = UrlUtils.IsAbsolutePhysicalPath(PicturePath) ? PicturePath : UrlUtils.ResolvePhysicalPath(PicturePath).ToLower();
			}
			state.Value = insertedImageFileInfo;
			return state;
		}
		protected override void OnAfterCommandExecuted() {
			base.OnAfterCommandExecuted();
			ClearUploadFile();
		}
		 protected void ClearUploadFile() {
			if(ImageStream == null) {
				var fileName =  UrlUtils.IsAbsolutePhysicalPath(PicturePath) ? PicturePath : UrlUtils.ResolvePhysicalPath(PicturePath).ToLower();
				FileInfo cleanFile = new FileInfo(fileName);
				try {
					cleanFile.Delete();
				} catch { }
			}
		}
	}
	public class AutoFitHeaderSizeWebCommand : StandardCommandExecuteImplementationBase {
		protected const string IndexParamName = "Index";
		protected const string ColumnFlagParamName = "IsColumn";
		protected int Index { get { return int.Parse(RequestParams[IndexParamName]); } }
		protected bool IsColumn { get { return bool.Parse(RequestParams[ColumnFlagParamName]); } }
		public AutoFitHeaderSizeWebCommand(WebSpreadsheetCommandContext context)
			: base(context) {
		}
		protected override SpreadsheetCommand CreateSpreadsheetCommand() {
			if(IsColumn) {
				var autoFitColumn = InnerControl.CreateCommand(SpreadsheetCommandId.FormatAutoFitColumnWidthUsingMouse) as FormatAutoFitColumnWidthUsingMouseCommand;
				autoFitColumn.ColumnIndex = Index;
				return autoFitColumn;
			}
			var autoFitRow = InnerControl.CreateCommand(SpreadsheetCommandId.FormatAutoFitRowHeightUsingMouse) as FormatAutoFitRowHeightUsingMouseCommand;
			autoFitRow.RowIndex = Index;
			return autoFitRow;
		}
		protected override ICommandUIState CreateCommandState(SpreadsheetCommand spreadsheetCommand) {
			return spreadsheetCommand.CreateDefaultCommandUIState();
		}
	}
	public class InsertTableWebCommand : StandardCommandExecuteImplementationBase {
		protected const string TableRangeParamName = "SelectedRange";
		protected const string TableHasHeadersParamName = "HasHeaders";
		protected string TableRange { get { return RequestParams[TableRangeParamName]; } }
		protected bool TableHasHeaders { get { return bool.Parse(RequestParams[TableHasHeadersParamName]); } }
		public InsertTableWebCommand(WebSpreadsheetCommandContext context)
			: base(context) {
		}
		protected override SpreadsheetCommand CreateSpreadsheetCommand() {
			return InnerControl.CreateCommand(SpreadsheetCommandId.InsertTable);
		}
		protected override ICommandUIState CreateCommandState(SpreadsheetCommand spreadsheetCommand) {
			var state = spreadsheetCommand.CreateDefaultCommandUIState() as IValueBasedCommandUIState<InsertTableViewModel>;
			state.Value = GetViewModel();
			return state;
		}
		protected InsertTableViewModel GetViewModel() {
			var viewModel = new InsertTableViewModel(RenderHelper.WorkSession.WebSpreadsheetControl);
			viewModel.Reference = TableRange;
			viewModel.Style = GetTableStyleName();
			viewModel.HasHeaders = TableHasHeaders;
			return viewModel;
		}
		protected virtual string GetTableStyleName() {
			return string.Empty;
		}
	}
	public class InsertTableWithStyleWebCommand : InsertTableWebCommand {
		protected const string TableStyleParamName = "TableStyle";
		protected string TableStyleName { get { return RequestParams[TableStyleParamName]; } }
		public InsertTableWithStyleWebCommand(WebSpreadsheetCommandContext context)
			: base(context) {
		}
		protected override SpreadsheetCommand CreateSpreadsheetCommand() {
			return InnerControl.CreateCommand(SpreadsheetCommandId.FormatAsTable);
		}
		protected override string GetTableStyleName() {
			return TableStyleName;
		}
	}
	public class ModifyTableStyleWebCommand : StandardCommandExecuteImplementationBase {
		protected const string TableStyleNameParamName = "StyleName";
		protected string TableStyleName { get { return RequestParams[TableStyleNameParamName]; } }
		public ModifyTableStyleWebCommand(WebSpreadsheetCommandContext context) 
			: base(context) {
		}
		protected override SpreadsheetCommand CreateSpreadsheetCommand() {
			return InnerControl.CreateCommand(SpreadsheetCommandId.ModifyTableStyles);
		}
		protected override ICommandUIState CreateCommandState(SpreadsheetCommand spreadsheetCommand) {
			var state = spreadsheetCommand.CreateDefaultCommandUIState() as IValueBasedCommandUIState<string>;
			state.Value = TableStyleName;
			return state;
		}
	}
	public class FormatAsTableWebCommand : StandardCommandExecuteImplementationBase {
		protected const string TableStyleNameParamName = "StyleName";
		protected string TableStyleName { get { return RequestParams[TableStyleNameParamName]; } }
		public FormatAsTableWebCommand(WebSpreadsheetCommandContext context) 
			: base(context) {
		}
		protected override SpreadsheetCommand CreateSpreadsheetCommand() {
			return InnerControl.CreateCommand(SpreadsheetCommandId.FormatAsTable);
		}
		protected override ICommandUIState CreateCommandState(SpreadsheetCommand spreadsheetCommand) {
			var state = spreadsheetCommand.CreateDefaultCommandUIState() as IValueBasedCommandUIState<string>;
			state.Value = TableStyleName;
			return state;
		}
	}
	public class DataFilterSimpleWebCommand : StandardCommandExecuteImplementationBase {
		public DataFilterSimpleWebCommand(WebSpreadsheetCommandContext context)
			: base(context) {
		}
		protected override SpreadsheetCommand CreateSpreadsheetCommand() {
			return InnerControl.CreateCommand(SpreadsheetCommandId.DataFilterSimple);
		}
		protected override ICommandUIState CreateCommandState(SpreadsheetCommand spreadsheetCommand) {
			var state = new DefaultValueBasedCommandUIState<string>();
			return state;
		}
	}
	public class CustomDataFilterWebCommand : StandardCommandExecuteImplementationBase {
		public CustomDataFilterWebCommand(WebSpreadsheetCommandContext context)
			: base(context) {
		}
		protected override SpreadsheetCommand CreateSpreadsheetCommand() {
			return InnerControl.CreateCommand(SpreadsheetCommandId.DataFilterCustom);
		}
		protected override ICommandUIState CreateCommandState(SpreadsheetCommand spreadsheetCommand) {
			var state = new DefaultValueBasedCommandUIState<string>();
			return state;
		}
	}
	public class AutoFilterWebCommandBase : CustomCommandExecuteImplementationBase {
		public AutoFilterWebCommandBase(WebSpreadsheetCommandContext context)
			: base(context) { }
		protected const string FilterCommandIdParamName = "filterCommandId";
		private SpreadsheetCommand filterCommand;
		protected string FilterCommandId {
			get { return RequestParams[FilterCommandIdParamName]; }
		}
		protected SpreadsheetCommand FilterCommand {
			get {
				if(this.filterCommand == null) {
					this.filterCommand = CreateCommand();
				}
				return this.filterCommand;
			}
		}
		protected AutoFilterColumn GetAutoFilterColumn() {
			DataSortOrFilterAccessor accessor = new DataSortOrFilterAccessor(Model);
			CellRange range = accessor.GetSortOrFilterRange();
			AutoFilterBase autoFilter = accessor.Filter;
			if(autoFilter != null && autoFilter.IsNonDefault)
				return GetFilterColumn(autoFilter, range);
			return null;
		}
		protected SpreadsheetCommand CreateCommand() {
			int commandId;
			Int32.TryParse(FilterCommandId, out commandId);
			SpreadsheetCommandId spreadsheetCommandId = new SpreadsheetCommandId(commandId);
			return InnerControl.CreateCommand(spreadsheetCommandId);
		}
		protected AutoFilterColumn GetFilterColumn(AutoFilterBase autoFilter, CellRange range) {
			int columnId = ActiveSheet.Selection.ActiveCell.Column - range.TopLeft.Column;
			return autoFilter.FilterColumns[columnId];
		}
		protected override void CustomExecuteImplementation() {
		}
	}
	public class FetchAutoFilterViewModelWebCommand : AutoFilterWebCommandBase {
		public FetchAutoFilterViewModelWebCommand(WebSpreadsheetCommandContext context)
			: base(context) {
		}
		protected string CustomResultProcessingFunction {
			get {
				return SpreadSheetCustomResultProcessingFunctionHelper.GetCustomResultProcessingFunctionNameFromParams(RequestParams);
			}
		}
		protected GenericFilterOperator ConvertOperator(FilterComparisonOperator filterOperator, string filterValue) {
			switch(filterOperator) 
			{
				case FilterComparisonOperator.Equal:
					return ConvertOperatorCore(filterOperator, filterValue);
				case FilterComparisonOperator.GreaterThan:
					return GenericFilterOperator.Greater;
				case FilterComparisonOperator.GreaterThanOrEqual:
					return GenericFilterOperator.GreaterOrEqual;
				case FilterComparisonOperator.LessThan:
					return GenericFilterOperator.Less;
				case FilterComparisonOperator.LessThanOrEqual:
					return GenericFilterOperator.LessOrEqual;
				case FilterComparisonOperator.NotEqual:
					return ConvertOperatorCore(filterOperator, filterValue);
			}
			return GenericFilterOperator.None;
		}
		protected GenericFilterOperator ConvertOperatorCore(FilterComparisonOperator filterOperator, string filterValue) {
			bool starAtStart = filterValue[0] == '*';
			bool starAtEnd = filterValue[filterValue.Length - 1] == '*';
			switch(filterOperator) {
				case FilterComparisonOperator.Equal:
					if(starAtStart && starAtEnd)
						return GenericFilterOperator.Contains;
					if(starAtEnd)
						return GenericFilterOperator.BeginsWith;
					if(starAtStart)
						return GenericFilterOperator.EndsWith;
					return GenericFilterOperator.Equals;
				case FilterComparisonOperator.NotEqual:
					if(starAtStart && starAtEnd)
						return GenericFilterOperator.DoesNotContain;
					if(starAtEnd)
						return GenericFilterOperator.DoesNotBeginWith;
					if(starAtStart)
						return GenericFilterOperator.DoesNotEndWith;
					return GenericFilterOperator.DoesNotEqual;
			}
			return GenericFilterOperator.None;
		}
		protected Hashtable GetGenericFilterViewModel() {
			var result = new Hashtable();
			GenericFilterViewModel filterViewModel = (FilterCommand as ShowCustomFilterFormCommandBase).CreateViewModel();
			result["FilterOperatorsDataSource"] = filterViewModel.FilterOperatorDataSource;
			result["FilterOperator"] = filterViewModel.FilterOperator;
			result["UniqueFilterValues"] = filterViewModel.UniqueFilterValues;
			result["FilterValue"] = filterViewModel.FilterValue;
			result["SecondaryFilterValue"] = filterViewModel.SecondaryFilterValue;
			result["SecondaryFilterOperator"] = filterViewModel.SecondaryFilterOperator;
			result["CriterionAnd"] = filterViewModel.OperatorAnd;
			var filterColumn = GetAutoFilterColumn();
			if(filterColumn != null) {
				if(filterColumn.IsNonDefault)
					result["CriterionAnd"] = filterColumn.CustomFilters.CriterionAnd;
				if(filterColumn.CustomFilters.Count > 0) {
					var firstFilter = filterColumn.CustomFilters[0];
					result["FilterOperator"] = ConvertOperator(firstFilter.FilterOperator, firstFilter.Value);
					result["FilterValue"] = firstFilter.Value.Trim('*');
					result["IsDateTime"] = firstFilter.IsDateTime;
				}
				if(filterColumn.CustomFilters.Count > 1) {
					var secondFilter = filterColumn.CustomFilters[1];
					result["SecondaryFilterOperator"] = ConvertOperator(secondFilter.FilterOperator, secondFilter.Value);
					result["SecondaryFilterValue"] = secondFilter.Value.Trim('*');
					result["SecondaryIsDateTime"] = secondFilter.IsDateTime;
				}
			}
			return result;
		}
		protected Hashtable GetTop10FilterViewModel() {
			var result = new Hashtable();
			Top10FilterViewModel filterViewModel = (FilterCommand as FilterTop10Command).CreateViewModel();
			result["OrderDataSource"] = filterViewModel.OrderDataSource;
			result["TypeDataSource"] = filterViewModel.TypeDataSource;
			var filterColumn = GetAutoFilterColumn();
			if(filterColumn != null && filterColumn.IsTop10Filter) {
				result["Value"] = filterColumn.TopOrBottomDoubleValue;
				result["IsTop"] = filterColumn.FilterByTopOrder;
				result["IsPercent"] = filterColumn.Top10FilterType == Top10FilterType.Percent;
			} 
			else {
				result["Value"] = filterViewModel.Value;
				result["IsTop"] = filterViewModel.IsTop;
				result["IsPercent"] = filterViewModel.IsPercent;
			}
			return result;
		}
		protected Hashtable GetFilterViewModelHashTable() {
			if(FilterCommand is ShowCustomFilterFormCommandBase)
				return GetGenericFilterViewModel();
			else if(FilterCommand is FilterTop10Command)
				return GetTop10FilterViewModel();
			return new Hashtable();
		}
		public override DocumentHandlerResponse GetResult(SpreadsheetRenderHelper renderHelper) {
			var result = new Hashtable();
			result["autoFilterViewModel"] = GetFilterViewModelHashTable();
			SpreadSheetCustomResultProcessingFunctionHelper.SetCustomResultProcessingFunction(result, CustomResultProcessingFunction);
			JSONDocumentHandlerResponse documentResponse = new JSONDocumentHandlerResponse();
			documentResponse.ContentEncoding = RequestContentEncoding;
			documentResponse.ResponseResult = HtmlConvertor.ToJSON(result);
			documentResponse.ContentType = RequestContentType;
			return documentResponse;
		}
	}
	public class ApplyAutoFilterWebCommand : AutoFilterWebCommandBase {
		protected const string ViewModelParamName = "ViewModel";
		protected string ViewModel {
			get { return RequestParams[ViewModelParamName]; }
		}
		public ApplyAutoFilterWebCommand(WebSpreadsheetCommandContext context)
			: base(context) { }
		protected override void CustomExecuteImplementation() {
			base.CustomExecuteImplementation();
			if((FilterCommand as ShowCustomFilterFormCommandBase) != null)
				ApplyGenericFilter();
			else if((FilterCommand as FilterSimpleCommand) != null)
				ApplySimpleFilter();
			else if((FilterCommand as FilterTop10Command) != null)
				ApplyTop10Filter();
		}
		protected void ApplySimpleFilter() {
			SimpleFilterViewModel coreViewModel = (FilterCommand as FilterSimpleCommand).CreateViewModel();
			ArrayList checkedIDs = HtmlConvertor.FromJSON<ArrayList>(ViewModel);
			UpdateCheckedState(coreViewModel.Root, checkedIDs, coreViewModel);
			coreViewModel.ApplyChanges();
		}
		protected void UpdateCheckedState(FilterValueNode node, ArrayList checkedIDs, SimpleFilterViewModel viewModel) {
			node.IsChecked = (bool)checkedIDs[node.Id];
			if(node.Text == viewModel.BlankValue)
				viewModel.BlankValueChecked = node.IsChecked;
			foreach(var child in node.Children)
				UpdateCheckedState(child, checkedIDs, viewModel);
		}
		protected void ApplyGenericFilter() {
			GenericFilterViewModel coreViewModel = (FilterCommand as ShowCustomFilterFormCommandBase).CreateViewModel();
			Hashtable viewModel = HtmlConvertor.FromJSON<Hashtable>(ViewModel);
			if(viewModel.ContainsKey("FirstFilterOperator"))
				coreViewModel.FilterOperator = ConvertStringToFilterOperator(viewModel["FirstFilterOperator"]);
			if(viewModel.ContainsKey("FirstFilterValue"))
				coreViewModel.FilterValue = viewModel["FirstFilterValue"].ToString();
			if(viewModel.ContainsKey("SecondaryFilterOperator"))
				coreViewModel.SecondaryFilterOperator = ConvertStringToFilterOperator(viewModel["SecondaryFilterOperator"]);
			if(viewModel.ContainsKey("SecondaryFilterValue"))
				coreViewModel.SecondaryFilterValue = viewModel["SecondaryFilterValue"].ToString();
			if(viewModel.ContainsKey("OperatorAnd"))
				coreViewModel.OperatorAnd = bool.Parse(viewModel["OperatorAnd"].ToString());
			coreViewModel.ApplyChanges();
		}
		protected void ApplyTop10Filter() {
			Top10FilterViewModel coreViewModel = (FilterCommand as FilterTop10Command).CreateViewModel();
			Hashtable viewModel = HtmlConvertor.FromJSON<Hashtable>(ViewModel);
			coreViewModel.IsTop = bool.Parse(viewModel["IsTop"].ToString());
			coreViewModel.IsPercent = bool.Parse(viewModel["IsPercent"].ToString());
			coreViewModel.Value = Int32.Parse(viewModel["Value"].ToString());
			coreViewModel.ApplyChanges();
		}
		protected GenericFilterOperator ConvertStringToFilterOperator(object filterOperator) {
			return (GenericFilterOperator)Enum.Parse(typeof(GenericFilterOperator), filterOperator.ToString());
		}
	}
	public class RenameTableWebCommand : StandardCommandExecuteImplementationBase {
		protected const string TableNameParamName = "TableName";
		public RenameTableWebCommand(WebSpreadsheetCommandContext context)
			: base(context) {
		}
		protected string TableName { get { return RequestParams[TableNameParamName]; } }
		protected override SpreadsheetCommand CreateSpreadsheetCommand() {
			return InnerControl.CreateCommand(SpreadsheetCommandId.TableToolsRenameTable);
		}
		protected override ICommandUIState CreateCommandState(SpreadsheetCommand spreadsheetCommand) {
			var state = new DefaultValueBasedCommandUIState<string>();
			state.Value = TableName;
			return state;
		}
	}
	public class RenameSheetWebCommand : StandardCommandExecuteImplementationBase {
		protected const string NewNameParamName = "NewName";
		public RenameSheetWebCommand(WebSpreadsheetCommandContext context)
			: base(context) {
		}
		protected string NewName { get { return RequestParams[NewNameParamName]; } }
		protected override SpreadsheetCommand CreateSpreadsheetCommand() {
			return InnerControl.CreateCommand(SpreadsheetCommandId.RenameSheet);
		}
		protected override ICommandUIState CreateCommandState(SpreadsheetCommand spreadsheetCommand) {
			var state = new DefaultValueBasedCommandUIState<string>();
			state.Value = NewName;
			return state;
		}
	}
	public class RemoveSheetWebCommand : StandardCommandExecuteImplementationBase {
		protected const string ForceRemoveSheetParamName = "ForceRemove";
		protected bool ForceRemove { get { return bool.Parse(RequestParams[ForceRemoveSheetParamName]); } }
		protected bool RemoveSheetAllowed { get; private set; }
		public RemoveSheetWebCommand(WebSpreadsheetCommandContext context)
			: base(context) {
		}
		protected override SpreadsheetCommand CreateSpreadsheetCommand() {
			return InnerControl.CreateCommand(SpreadsheetCommandId.RemoveSheet);
		}
		protected override bool CommandExecuteAllowed() {
			RemoveSheetAllowed = ActiveSheet.IsEmptySheet() || ForceRemove;
			return base.CommandExecuteAllowed() && RemoveSheetAllowed;
		}
		public override Hashtable GetResponseJSONHashTable() {
			Hashtable json = new Hashtable();
			if(!RemoveSheetAllowed)
				json[ResponseQueryStringKeys.RemoveSheetConfirmation] = XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Msg_DeleteSheetConfirmation);
			return json;
		}
		protected override ICommandUIState CreateCommandState(SpreadsheetCommand spreadsheetCommand) {
			return null;
		}
	}
	#region FreezeCommands
	public class FreezePanesWebCommand : StandardCommandExecuteImplementationBase {
		public FreezePanesWebCommand(WebSpreadsheetCommandContext context)
			: base(context) {
		}
		protected override ICommandUIState CreateCommandState(SpreadsheetCommand spreadsheetCommand) {
			return spreadsheetCommand.CreateDefaultCommandUIState();
		}
		protected override SpreadsheetCommand CreateSpreadsheetCommand() {
			return InnerControl.CreateCommand(SpreadsheetCommandId.ViewFreezePanes);
		}
	}
	public class FreezeRowWebCommand : StandardCommandExecuteImplementationBase {
		private SpreadsheetCommandId Command { get { return SpreadsheetCommandId.ViewFreezeTopRow;  } }
		public FreezeRowWebCommand(WebSpreadsheetCommandContext context)
			: base(context) {
		}
		protected override ICommandUIState CreateCommandState(SpreadsheetCommand spreadsheetCommand) {
			return spreadsheetCommand.CreateDefaultCommandUIState();
		}
		protected override SpreadsheetCommand CreateSpreadsheetCommand() {
			return InnerControl.CreateCommand(Command);
		}
		protected override void OnAfterCommandExecuted() {
			base.OnAfterCommandExecuted();
			RenderHelper.CalculateScrollAnchor(Command);
		}
	}
	public class FreezeColumnWebCommand : StandardCommandExecuteImplementationBase {
		private SpreadsheetCommandId Command { get { return SpreadsheetCommandId.ViewFreezeFirstColumn; } }
		public FreezeColumnWebCommand(WebSpreadsheetCommandContext context)
			: base(context) {
		}
		protected override ICommandUIState CreateCommandState(SpreadsheetCommand spreadsheetCommand) {
			return spreadsheetCommand.CreateDefaultCommandUIState();
		}
		protected override SpreadsheetCommand CreateSpreadsheetCommand() {
			return InnerControl.CreateCommand(Command);
		}
		protected override void OnAfterCommandExecuted() {
			base.OnAfterCommandExecuted();
			RenderHelper.CalculateScrollAnchor(Command);
		}
	}
	#endregion
	public class MoveOrCopySheetWebCommand : StandardCommandExecuteImplementationBase {
		protected const string BeforeVisibleSheetIndexParamName = "BeforeVisibleSheetIndex",
							   CreateCopyParamName = "CreateCopy";
		public MoveOrCopySheetWebCommand(WebSpreadsheetCommandContext context)
			: base(context) {
		}
		protected int BeforeVisibleSheetIndex {
			get { return Int32.Parse(RequestParams[BeforeVisibleSheetIndexParamName]); }
		}
		protected bool CreateCopy {
			get { return Boolean.Parse(RequestParams[CreateCopyParamName]); }
		}
		protected override SpreadsheetCommand CreateSpreadsheetCommand() {
			return InnerControl.CreateCommand(SpreadsheetCommandId.MoveOrCopySheet);
		}
		protected override ICommandUIState CreateCommandState(SpreadsheetCommand spreadsheetCommand) {
			var argument = new MoveOrCopySheetCommandParameters(BeforeVisibleSheetIndex, CreateCopy);
			var state = new DefaultValueBasedCommandUIState<MoveOrCopySheetCommandParameters>();
			state.Value = argument;
			return state;
		}
	}
	public class WorkSessionChangingCommandExecuteImplementation : WorkSessionChangingCommandExecuteImplementationBase {
		public WorkSessionChangingCommandExecuteImplementation(WebSpreadsheetCommandContext context)
			: base(context) {
		}
		protected override void CustomExecuteImplementation() {
		}
		protected override SpreadsheetCommand CreateSpreadsheetCommand() {
			return null;
		}
		protected override ICommandUIState CreateCommandState(SpreadsheetCommand spreadsheetCommand) {
			return null;
		}
	}
	public class FileNewWebCommand : WorkSessionChangingCommandExecuteImplementation {
		public FileNewWebCommand(WebSpreadsheetCommandContext context)
			: base(context) {
		}
	}
	public class FileSaveWebCommand : CustomCommandExecuteImplementationBase {
		public FileSaveWebCommand(WebSpreadsheetCommandContext context)
			: base(context) {
		}
		protected override void CustomExecuteImplementation() {
			var command = (SaveDocumentCommand)InnerControl.CreateCommand(SpreadsheetCommandId.FileSave);
			command.Execute();
			RenderHelper.WorkSession.OnDocumentSaved();
		}
	}
	public class FileOpenCommand : WorkSessionChangingCommandExecuteImplementation {
		public FileOpenCommand(WebSpreadsheetCommandContext context)
			: base(context) {
		}
		protected override bool CommandExecuteAllowed() {
			return true;
		}
	}
	public class FileSaveAsCommand : WorkSessionChangingCommandExecuteImplementation {
		internal const string FilePathParamName = "FilePath";
		public FileSaveAsCommand(WebSpreadsheetCommandContext context)
			: base(context) {
		}
		protected override bool CommandExecuteAllowed() {
			return true;
		}
	}
	public class PasteWebCommand : StandardCommandExecuteImplementationBase {
		internal const string PasteJSONParamName = "PasteValue",
							  BufferIdParamName = "BufferId";
		public PasteWebCommand(WebSpreadsheetCommandContext context)
			: base(context) {
		}
		protected string PasteValue { get { return RequestParams[PasteJSONParamName]; } }
		protected Guid BufferId { get { return Guid.Parse(RequestParams[BufferIdParamName]); } }
		protected override bool CommandExecuteAllowed() {
			return true;
		}
		public override void Execute() {
			if(CommandExecuteAllowed()) {
				SpreadsheetClipboardHelper clipboardHelper = new SpreadsheetClipboardHelper(InnerControl, PasteValue, BufferId);
				clipboardHelper.ProcessClipboardEvent();
			}
		}
		protected override SpreadsheetCommand CreateSpreadsheetCommand() {
			return null;
		}
		protected override ICommandUIState CreateCommandState(SpreadsheetCommand spreadsheetCommand) {
			return null;
		}
	}
	public class ShapeMoveAndResizeWebCommand : StandardCommandExecuteImplementationBase {
		protected const string
			offsetXParamName = "shapeOffsetX",
			offsetYParamName = "shapeOffsetY",
			widthParamName = "shapeWidth",
			heightParamName = "shapeHeight";
		public ShapeMoveAndResizeWebCommand(WebSpreadsheetCommandContext context)
			: base(context) {
		}
		protected int OffsetX { get { return int.Parse(RequestParams[offsetXParamName]); } }
		protected int OffsetY { get { return int.Parse(RequestParams[offsetYParamName]); } }
		protected int Width { get { return int.Parse(RequestParams[widthParamName]); } }
		protected int Height { get { return int.Parse(RequestParams[heightParamName]); } }
		protected override SpreadsheetCommand CreateSpreadsheetCommand() {
			return InnerControl.CreateCommand(SpreadsheetCommandId.ShapeMoveAndResize);
		}
		protected override ICommandUIState CreateCommandState(SpreadsheetCommand spreadsheetCommand) {
			var state = new DefaultValueBasedCommandUIState<MoveAndResizeShapeInfo>();
			state.Value = new MoveAndResizeShapeInfo(OffsetX, OffsetY, Width, Height);
			return state;
		}
	}
	public abstract class UnhideRowsColumnsWebCommandBase : StandardCommandExecuteImplementationBase {
		public UnhideRowsColumnsWebCommandBase(WebSpreadsheetCommandContext context)
			: base(context) {
		}
		CellPosition backupTopLeftCellPosition = CellPosition.InvalidValue;
		public CellPosition BackupTopLeftCellPosition {
			get { return backupTopLeftCellPosition; }
		}
		protected abstract bool WorksWithRows { get; }
		void ExpandSelectionIfRequired() {
			bool topmostVisibleRowSelected = ActiveSheet.Selection.ActiveRange.TopLeft.Row <= ActiveSheet.WebRanges.FirstVisiblePosition.Row;
			bool leftmostVisibleRowSelected = ActiveSheet.Selection.ActiveRange.TopLeft.Column <= ActiveSheet.WebRanges.FirstVisiblePosition.Column;
			bool needTemporallyExpandSelection = topmostVisibleRowSelected && WorksWithRows || leftmostVisibleRowSelected && !WorksWithRows;
			if(needTemporallyExpandSelection) {
				int backupTopRow = ActiveSheet.Selection.ActiveRange.TopLeft.Row;
				int backupLeftColumn = ActiveSheet.Selection.ActiveRange.TopLeft.Column;
				backupTopLeftCellPosition = ActiveSheet.Selection.ActiveRange.TopLeft;
				CellPosition newTopLeftCellPosition = new CellPosition(0, 0);
				ActiveSheet.Selection.ActiveRange.TopLeft = newTopLeftCellPosition;
			}
		}
		void RestoreSelection() {
			if(backupTopLeftCellPosition.IsValid)
				ActiveSheet.Selection.ActiveRange.TopLeft = backupTopLeftCellPosition;
		}
		protected override void OnBeforeCommandExecuted() {
			base.OnBeforeCommandExecuted();
			ExpandSelectionIfRequired();
		}
		protected override void OnAfterCommandExecuted() {
			base.OnAfterCommandExecuted();
			RestoreSelection();
		}
	}
	public class UnhideRowsWebCommand : UnhideRowsColumnsWebCommandBase {
		public UnhideRowsWebCommand(WebSpreadsheetCommandContext context)
			: base(context) {
		}
		protected override bool WorksWithRows {
			get { return true; }
		}
		protected override SpreadsheetCommand CreateSpreadsheetCommand() {
			return InnerControl.CreateCommand(SpreadsheetCommandId.UnhideRows);
		}
		protected override ICommandUIState CreateCommandState(SpreadsheetCommand spreadsheetCommand) {
			return spreadsheetCommand.CreateDefaultCommandUIState();
		}
	}
	public class UnhideColumnsWebCommand : UnhideRowsColumnsWebCommandBase {
		public UnhideColumnsWebCommand(WebSpreadsheetCommandContext context)
			: base(context) {
		}
		protected override bool WorksWithRows {
			get { return false; }
		}
		protected override SpreadsheetCommand CreateSpreadsheetCommand() {
			return InnerControl.CreateCommand(SpreadsheetCommandId.UnhideColumns);
		}
		protected override ICommandUIState CreateCommandState(SpreadsheetCommand spreadsheetCommand) {
			return spreadsheetCommand.CreateDefaultCommandUIState();
		}
	}
	public class FormatRowHeightWebCommand : StandardCommandExecuteImplementationBase {
		protected const string RowHeightParamName = "RowHeight";
		public FormatRowHeightWebCommand(WebSpreadsheetCommandContext context)
			: base(context) {
		}
		protected int RowHeight { get { return int.Parse(RequestParams[RowHeightParamName]); } }
		protected override SpreadsheetCommand CreateSpreadsheetCommand() {
			return InnerControl.CreateCommand(SpreadsheetCommandId.FormatRowHeight);
		}
		protected override ICommandUIState CreateCommandState(SpreadsheetCommand spreadsheetCommand) {
			var state = new DefaultValueBasedCommandUIState<int>();
			state.Value = RowHeight;
			return state;
		}
	}
	public class FormatColumnWidthWebCommand : StandardCommandExecuteImplementationBase {
		protected const string ColumnWidthParamName = "ColumnWidth";
		public FormatColumnWidthWebCommand(WebSpreadsheetCommandContext context)
			: base(context) {
		}
		protected int ColumnWidth { get { return int.Parse(RequestParams[ColumnWidthParamName]); } }
		protected override SpreadsheetCommand CreateSpreadsheetCommand() {
			return InnerControl.CreateCommand(SpreadsheetCommandId.FormatColumnWidth);
		}
		protected override ICommandUIState CreateCommandState(SpreadsheetCommand spreadsheetCommand) {
			var state = new DefaultValueBasedCommandUIState<int>();
			state.Value = ColumnWidth;
			return state;
		}
	}
	public class FormatDefaultColumnWidthWebCommand : StandardCommandExecuteImplementationBase {
		protected const string DefaultColumnWidthParamName = "DefaultColumnWidth";
		public FormatDefaultColumnWidthWebCommand(WebSpreadsheetCommandContext context)
			: base(context) {
		}
		protected int DefaultColumnWidth { get { return int.Parse(RequestParams[DefaultColumnWidthParamName]); } }
		protected override SpreadsheetCommand CreateSpreadsheetCommand() {
			return InnerControl.CreateCommand(SpreadsheetCommandId.FormatDefaultColumnWidth);
		}
		protected override ICommandUIState CreateCommandState(SpreadsheetCommand spreadsheetCommand) {
			var state = new DefaultValueBasedCommandUIState<int>();
			state.Value = DefaultColumnWidth;
			return state;
		}
	}
	public class UnhideSheetWebCommand : StandardCommandExecuteImplementationBase {
		protected const string SheetNameParamName = "SheetName";
		public UnhideSheetWebCommand(WebSpreadsheetCommandContext context)
			: base(context) {
		}
		protected string SheetName { get { return RequestParams[SheetNameParamName]; } }
		protected override SpreadsheetCommand CreateSpreadsheetCommand() {
			return InnerControl.CreateCommand(SpreadsheetCommandId.UnhideSheet);
		}
		protected override ICommandUIState CreateCommandState(SpreadsheetCommand spreadsheetCommand) {
			var state = new DefaultValueBasedCommandUIState<string>();
			state.Value = SheetName;
			return state;
		}
	}
	public class ChartChangeTypeSheetWebCommand : StandardCommandExecuteImplementationBase {
		protected const string ChartTypeParamName = "ChartType";
		public ChartChangeTypeSheetWebCommand(WebSpreadsheetCommandContext context)
			: base(context) {
		}
		protected string ChartType { get { return RequestParams[ChartTypeParamName]; } }
		protected override SpreadsheetCommand CreateSpreadsheetCommand() {
			return InnerControl.CreateCommand(SpreadsheetCommandId.ChartChangeType);
		}
		protected override ICommandUIState CreateCommandState(SpreadsheetCommand spreadsheetCommand) {
			var state = new DefaultValueBasedCommandUIState<SpreadsheetCommandId>();
			state.Value = ((IConvertToInt<SpreadsheetCommandId>)SpreadsheetCommandId.None).FromInt(int.Parse(ChartType));
			return state;
		}
	}
	public class ChartSelectDataSheetWebCommand : StandardCommandExecuteImplementationBase {
		protected const string ChartSelectDataParamName = "SelectionRange";
		public ChartSelectDataSheetWebCommand(WebSpreadsheetCommandContext context)
			: base(context) {
		}
		protected string ChartSelectData { get { return RequestParams[ChartSelectDataParamName]; } }
		protected override SpreadsheetCommand CreateSpreadsheetCommand() {
			return InnerControl.CreateCommand(SpreadsheetCommandId.ChartSelectData);
		}
		protected override ICommandUIState CreateCommandState(SpreadsheetCommand spreadsheetCommand) {
			ChartSelectDataInfo chartInfo = new ChartSelectDataInfo(this.Model.ActiveSheet.Selection.SelectedChart, ChartSelectData);
			var state = new DefaultValueBasedCommandUIState<IChartSelectDataInfo>();
			state.Value = chartInfo;
			return state;
		}
	}
	public class ModifyChartLayoutWebCommand : StandardCommandExecuteImplementationBase {
		protected const string ChartLayoutPresetParamName = "ChartLayoutPreset";
		public ModifyChartLayoutWebCommand(WebSpreadsheetCommandContext context)
			: base(context) {
		}
		protected string CharLayoutPreset { get { return RequestParams[ChartLayoutPresetParamName]; } }
		protected override SpreadsheetCommand CreateSpreadsheetCommand() {
			return InnerControl.CreateCommand(SpreadsheetCommandId.ModifyChartLayout);
		}
		protected override ICommandUIState CreateCommandState(SpreadsheetCommand spreadsheetCommand) {
			ChartLayoutModifier currentChartModifier = null;
			ModifyChartLayoutCommand command = new ModifyChartLayoutCommand(RenderHelper.WorkSession.WebSpreadsheetControl);
			ChartPresetCategory chartPresetCategory = command.CalculateChartPresetCategory();
			IList<ChartLayoutModifier> modifiers = ChartLayoutModifier.GetModifiers(chartPresetCategory);
			foreach(ChartLayoutModifier modifier in modifiers) {
				if(modifier.ImageName == CharLayoutPreset) {
					currentChartModifier = modifier;
					break;
				}
			}
			var state = new DefaultValueBasedCommandUIState<ChartLayoutModifier>();
			state.Value = currentChartModifier;
			return state;
		}
	}
	public class GetLocalizedStringConstantWebCommand : WebSpreadsheetCommandBase {
		const string keyXtraSpreadsheetStringId = "XtraSpreadsheetStringId",
					 keyXtraSpreadsheetString = "XtraSpreadsheetString";
		public GetLocalizedStringConstantWebCommand(WebSpreadsheetCommandContext context)
			: base(context) {
		}
		protected string StringID { get { return RequestParams[keyXtraSpreadsheetStringId]; } }
		protected string CustomResultProcessingFunction {
			get {
				return SpreadSheetCustomResultProcessingFunctionHelper.GetCustomResultProcessingFunctionNameFromParams(RequestParams);
			}
		}
		protected override void CustomExecuteImplementation() {
		}
		protected override SpreadsheetCommand CreateSpreadsheetCommand() {
			return null;
		}
		protected override ICommandUIState CreateCommandState(SpreadsheetCommand spreadsheetCommand) {
			return null;
		}
		public override DocumentHandlerResponse GetResult(SpreadsheetRenderHelper renderHelper) {
			XtraSpreadsheetStringId stringID = (XtraSpreadsheetStringId)Enum.Parse(typeof(XtraSpreadsheetStringId), StringID);
			var result = new Hashtable();
			result[keyXtraSpreadsheetStringId] = StringID;
			result[keyXtraSpreadsheetString] = XtraSpreadsheetLocalizer.GetString(stringID);
			SpreadSheetCustomResultProcessingFunctionHelper.SetCustomResultProcessingFunction(result, CustomResultProcessingFunction);
			JSONDocumentHandlerResponse documentResponse = new JSONDocumentHandlerResponse();
			documentResponse.ContentEncoding = RequestContentEncoding;
			documentResponse.ResponseResult = HtmlConvertor.ToJSON(result);
			documentResponse.ContentType = RequestContentType;
			return documentResponse;
		}
	}
	public class DownLoadCopyWebCommand : WebSpreadsheetCommandBase {
		protected const string FileFormatParamName = "FileFormat";
		public DownLoadCopyWebCommand(WebSpreadsheetCommandContext context)
			: base(context) {
		}
		protected string FileFormat { get { return RequestParams[FileFormatParamName]; } }
		protected override void CustomExecuteImplementation() {
		}
		protected override SpreadsheetCommand CreateSpreadsheetCommand() {
			return null;
		}
		protected override ICommandUIState CreateCommandState(SpreadsheetCommand spreadsheetCommand) {
			return null;
		}
		public override DocumentHandlerResponse GetResult(SpreadsheetRenderHelper renderHelper) {
			string result = string.Empty;
			string fileName = string.Empty;
			using(MemoryStream stream = new MemoryStream()) {
				InnerControl.SaveDocument(stream, InnerControl.DocumentModel.AutodetectDocumentFormat(FileFormat, true));
				fileName = Path.GetFileNameWithoutExtension(InnerControl.DocumentModel.DocumentSaveOptions.CurrentFileName);
				if(string.IsNullOrEmpty(fileName))
					fileName = "Spreadsheet1";
				ResponseFileInfo fileInfo = new ResponseFileInfo(fileName + FileFormat, stream.ToArray());
				fileInfo.AsAttachment = true;
				AttachmentDocumentHandlerResponse documentResponse = new AttachmentDocumentHandlerResponse();
				documentResponse.ContentEncoding = RequestContentEncoding;
				documentResponse.ResponseFile = fileInfo;
				documentResponse.AutodetectContentType = true;
				return documentResponse;
			}
		}
	}
	public class PrintWebCommand : WebSpreadsheetCommandBase {
		public PrintWebCommand(WebSpreadsheetCommandContext context)
			: base(context) {
		}
		protected override void CustomExecuteImplementation() { }
		protected override SpreadsheetCommand CreateSpreadsheetCommand() {
			return null;
		}
		protected override ICommandUIState CreateCommandState(SpreadsheetCommand spreadsheetCommand) {
			return null;
		}
		public override DocumentHandlerResponse GetResult(SpreadsheetRenderHelper renderHelper) {
			string result = string.Empty;
			string fileName = string.Empty;
			using(MemoryStream stream = new MemoryStream()) {
				XtraPrinting.PdfExportOptions oprions = new XtraPrinting.PdfExportOptions();
				oprions.ShowPrintDialogOnOpen = true;
				InnerControl.ExportToPdf(stream, oprions);
				fileName = Path.GetFileNameWithoutExtension(InnerControl.DocumentModel.DocumentSaveOptions.CurrentFileName);
				if(string.IsNullOrEmpty(fileName))
					fileName = "Spreadsheet1";
				ResponseFileInfo fileInfo = new ResponseFileInfo(fileName + ".pdf", stream.ToArray());
				fileInfo.AsAttachment = false;
				AttachmentDocumentHandlerResponse documentResponse = new AttachmentDocumentHandlerResponse();
				documentResponse.ContentEncoding = RequestContentEncoding;
				documentResponse.ResponseFile = fileInfo;
				documentResponse.AutodetectContentType = true;
				return documentResponse;
			}
		}
	}
	public class ChartChangeTitleWebCommand : StandardCommandExecuteImplementationBase {
		protected const string ChartTitleParamName = "ChartTitle";
		public ChartChangeTitleWebCommand(WebSpreadsheetCommandContext context)
			: base(context) {
		}
		protected string ChartTitle { get { return RequestParams[ChartTitleParamName]; } }
		protected override SpreadsheetCommand CreateSpreadsheetCommand() {
			return InnerControl.CreateCommand(SpreadsheetCommandId.ChartChangeTitleContextMenuItem);
		}
		protected override ICommandUIState CreateCommandState(SpreadsheetCommand spreadsheetCommand) {
			var state = new DefaultValueBasedCommandUIState<string>();
			state.Value = ChartTitle;
			return state;
		}
	}
	public class ChartChangeHorizontalAxisTitleWebCommand : StandardCommandExecuteImplementationBase {
		protected const string ChartHorizontalAxisTitleParamName = "ChartHorizontalAxisTitle";
		public ChartChangeHorizontalAxisTitleWebCommand(WebSpreadsheetCommandContext context)
			: base(context) {
		}
		protected string ChartHorizontalAxisTitle { get { return RequestParams[ChartHorizontalAxisTitleParamName]; } }
		protected override SpreadsheetCommand CreateSpreadsheetCommand() {
			return InnerControl.CreateCommand(SpreadsheetCommandId.ChartChangeHorizontalAxisTitleContextMenuItem);
		}
		protected override ICommandUIState CreateCommandState(SpreadsheetCommand spreadsheetCommand) {
			var state = new DefaultValueBasedCommandUIState<string>();
			state.Value = ChartHorizontalAxisTitle;
			return state;
		}
	}
	public class ChartChangeVerticalAxisTitleWebCommand : StandardCommandExecuteImplementationBase {
		protected const string ChartVerticalAxisTitleParamName = "ChartVerticalAxisTitle";
		public ChartChangeVerticalAxisTitleWebCommand(WebSpreadsheetCommandContext context)
			: base(context) {
		}
		protected string ChartVerticalAxisTitle { get { return RequestParams[ChartVerticalAxisTitleParamName]; } }
		protected override SpreadsheetCommand CreateSpreadsheetCommand() {
			return InnerControl.CreateCommand(SpreadsheetCommandId.ChartChangeVerticalAxisTitleContextMenuItem);
		}
		protected override ICommandUIState CreateCommandState(SpreadsheetCommand spreadsheetCommand) {
			var state = new DefaultValueBasedCommandUIState<string>();
			state.Value = ChartVerticalAxisTitle;
			return state;
		}
	}
	public class ModifyChartStyleWebCommand : StandardCommandExecuteImplementationBase {
		protected const string ChartPresetParamName = "ChartPresetStyle";
		public ModifyChartStyleWebCommand(WebSpreadsheetCommandContext context)
			: base(context) {
		}
		protected int CharPresetStyle { get { return int.Parse(RequestParams[ChartPresetParamName]); } }
		protected override SpreadsheetCommand CreateSpreadsheetCommand() {
			return InnerControl.CreateCommand(SpreadsheetCommandId.ModifyChartStyle);
		}
		protected override ICommandUIState CreateCommandState(SpreadsheetCommand spreadsheetCommand) {
			var state = new DefaultValueBasedCommandUIState<int>();
			state.Value = CharPresetStyle;
			return state;
		}
	}
	public static class SpreadSheetCustomResultProcessingFunctionHelper {
		const string keySpreadsheetCustomResultProcessingFunction = "CustomResultProcessingFunction";
		public static string GetCustomResultProcessingFunctionNameFromParams(NameValueCollection requestParams) {
			return requestParams[keySpreadsheetCustomResultProcessingFunction];
		}
		public static void SetCustomResultProcessingFunction(Hashtable result, string customResultProcessingFunction) {
			result[keySpreadsheetCustomResultProcessingFunction] = customResultProcessingFunction;
		}
	}
	public class FindAllWebCommand : CustomCommandExecuteImplementationBase {
		public class FindResult {
			public FindResult(ICell cell) {
				DisplayText = cell.Text;
				ModelColumnIndex = cell.Position.Column;
				ModelRowIndex = cell.Position.Row;
				CellPosition = cell.Position.ToString();
			}
			public string DisplayText { get; private set; }
			public int ModelColumnIndex { get; private set; }
			public int ModelRowIndex { get; private set; }
			public string CellPosition { get; private set; }
		}
		protected const string
			FindWhatParamName = "FindWhat",
			MatchCaseParamName = "MatchCase",
			MatchEntireCellContentParamName = "MatchEntireCellContent",
			SearchByParamName = "SearchBy",
			SearchInParamName = "LookIn";
		public FindAllWebCommand(WebSpreadsheetCommandContext context)
			: base(context) {
		}
		protected string FindWhat {
			get { return RequestParams[FindWhatParamName]; }
		}
		protected bool MatchCase {
			get { return bool.Parse(RequestParams[MatchCaseParamName]); }
		}
		protected bool MatchEntireCellContent {
			get { return bool.Parse(RequestParams[MatchEntireCellContentParamName]); }
		}
		protected string SearchBy {
			get { return RequestParams[SearchByParamName]; }
		}
		protected string SearchIn {
			get { return RequestParams[SearchInParamName]; }
		}
		protected string CustomResultProcessingFunction {
			get {
				return SpreadSheetCustomResultProcessingFunctionHelper.GetCustomResultProcessingFunctionNameFromParams(RequestParams);
			}
		}
		protected override void CustomExecuteImplementation() {
		}
		protected Hashtable GetFindAllAndNextResult() {
			var result = new Hashtable();
			var viewModel = GetViewModel();
			IEnumerator<ICell> enumerator = SpreadsheetSearchHelper.FindAll(ActiveSheet, viewModel);
			var findAllList = new System.Collections.Generic.List<FindResult>();
			while(enumerator.MoveNext()) {
				findAllList.Add(new FindResult(enumerator.Current));
			}
			enumerator.Reset();
			CellPosition nextCellPosition = SpreadsheetSearchHelper.TryFindNextInAllFound(enumerator, ActiveSheet.Selection.ActiveCell, true);
			result["findAllList"] = findAllList;
			if(nextCellPosition.IsValid)
				result["findNextCellModelPosition"] = nextCellPosition;
			return result;
		}
		protected XtraSpreadsheet.Forms.FindReplaceViewModel GetViewModel() {
			var viewModel = new DevExpress.XtraSpreadsheet.Forms.FindReplaceViewModel(RenderHelper.WorkSession.WebSpreadsheetControl);
			viewModel.FindWhat = FindWhat;
			viewModel.MatchCase = MatchCase;
			viewModel.MatchEntireCellContents = MatchEntireCellContent;
			viewModel.SearchBy = SearchBy;
			viewModel.SearchIn = SearchIn;
			return viewModel;
		}
		public override DocumentHandlerResponse GetResult(SpreadsheetRenderHelper renderHelper) {
			var result = new Hashtable();
			var findAllResults = GetFindAllAndNextResult();
			result["FindAllResult"] = findAllResults;
			SpreadSheetCustomResultProcessingFunctionHelper.SetCustomResultProcessingFunction(result, CustomResultProcessingFunction);
			JSONDocumentHandlerResponse documentResponse = new JSONDocumentHandlerResponse();
			documentResponse.ContentEncoding = RequestContentEncoding;
			documentResponse.ResponseResult = HtmlConvertor.ToJSON(result);
			documentResponse.ContentType = RequestContentType;
			return documentResponse;
		}
	}
	public class ScrollToWebCommand : CustomCommandExecuteImplementationBase {
		protected const string
			CellPositionColumnParamName = "CellPositionColumn",
			CellPositionRowParamName = "CellPositionRow",
			SelectAfterScrollParamName = "SelectAfterScroll";
		public ScrollToWebCommand(WebSpreadsheetCommandContext context)
			: base(context) {
		}
		protected int CellPositionColumn {
			get {
				return int.Parse(RequestParams[CellPositionColumnParamName]);
			}
		}
		protected int CellPositionRow {
			get {
				return int.Parse(RequestParams[CellPositionRowParamName]);
			}
		}
		protected bool SelectAfterScroll {
			get {
				return bool.Parse(RequestParams[SelectAfterScrollParamName]);
			}
		}
		protected override void CustomExecuteImplementation() {
			ActiveSheet.ScrollTo(CellPositionRow, CellPositionColumn);
			if(SelectAfterScroll)
				ActiveSheet.Selection.SetSelection(new CellPosition(CellPositionColumn, CellPositionRow));
		}
		public override DocumentHandlerResponse GetResult(SpreadsheetRenderHelper renderHelper) {
			return base.GetResult(renderHelper);
		}
	}
	public class GetPictureContentCommand : CustomCommandExecuteImplementationBase {
		protected const string ImageParamName = "simg";
		public GetPictureContentCommand(WebSpreadsheetCommandContext context)
			: base(context) {
		}
		protected int ImageCacheId {
			get { return int.Parse(RequestParams[ImageParamName]); }
		}
		protected override void CustomExecuteImplementation() {
		}
		public override DocumentHandlerResponse GetResult(SpreadsheetRenderHelper renderHelper) {
			var result = new BinaryDocumentHandlerResponse();
			var image = Model.ImageCache.GetImageByKey(ImageCacheId);
			result.BinaryBuffer = image.GetImageBytes(DevExpress.Office.Utils.OfficeImageFormat.Png);
			result.ContentType = "image/png";
			return result;
		}
	}
	public abstract class DataValidationViewModelCommand : CustomCommandExecuteImplementationBase {
		protected const string ViewModelParamName = "ViewModel";
		DataValidationCommand spreadsheetDataValidationCommand;
		public DataValidationViewModelCommand(WebSpreadsheetCommandContext context)
			: base(context) {
		}
		protected DataValidationCommand SpreadsheetDataValidationCommand {
			get {
				if(spreadsheetDataValidationCommand == null)
					spreadsheetDataValidationCommand = InnerControl.CreateCommand(SpreadsheetCommandId.DataToolsDataValidation) as DataValidationCommand;
				return spreadsheetDataValidationCommand;
			}
		}
	}
	public class FetchDataValidationViewModelWebCommand : DataValidationViewModelCommand {
		public FetchDataValidationViewModelWebCommand(WebSpreadsheetCommandContext context)
			: base(context) {
		}
		protected string CustomResultProcessingFunction {
			get {
				return SpreadSheetCustomResultProcessingFunctionHelper.GetCustomResultProcessingFunctionNameFromParams(RequestParams);
			}
		}
		protected Hashtable GetViewModelFromServer(DataValidationCommandParameters parameters) {
			DataValidationViewModel viewModel = PrepareViewModel(parameters);
			viewModel.PrepareDateTimeFormulas();
			var result = new Hashtable();
			if(viewModel != null) {
				result["Type"] = ((int)viewModel.Type).ToString();
				result["Operator"] = ((int)viewModel.Operator).ToString();
				result["IgnoreBlank"] = viewModel.IgnoreBlank;
				result["InCellDropDown"] = viewModel.InCellDropDown;
				result["Formula1"] = viewModel.Formula1;
				result["Formula2"] = viewModel.Formula2;
				result["ShowMessage"] = viewModel.ShowMessage;
				result["MessageTitle"] = viewModel.MessageTitle;
				result["Message"] = viewModel.Message;
				result["ShowErrorMessage"] = viewModel.ShowErrorMessage;
				result["ErrorTitle"] = viewModel.ErrorTitle;
				result["ErrorMessage"] = viewModel.ErrorMessage;
				result["ErrorStyle"] = viewModel.ErrorStyle;
				if(viewModel.DataValidationRange != null)
					result["DataValidationRange"] = ValidationHelper.GetLTRBListFromCellRange(viewModel.DataValidationRange);
			}
			return result;
		}
		protected DataValidationViewModel PrepareViewModel(DataValidationCommandParameters parameters) {
			DataValidationViewModel viewModel;
			if(parameters.IntersectionCount == 1)
				viewModel = SpreadsheetDataValidationCommand.CreateViewModel(parameters.ActiveDataValidation, parameters.ActiveRange);
			else
				viewModel = SpreadsheetDataValidationCommand.CreateDefaultViewModel(parameters.ActiveRange);
			return viewModel;
		}
		protected override void CustomExecuteImplementation() {
		}
		protected Hashtable GetResponseResult() {
			DataValidationCommandParameters parameters = SpreadsheetDataValidationCommand.CalculateCommandParameters();
			return GetViewModelFromServer(parameters);
		}
		public override DocumentHandlerResponse GetResult(SpreadsheetRenderHelper renderHelper) {
			var result = new Hashtable();
			result[ViewModelParamName] = GetResponseResult();
			SpreadSheetCustomResultProcessingFunctionHelper.SetCustomResultProcessingFunction(result, CustomResultProcessingFunction);
			JSONDocumentHandlerResponse documentResponse = new JSONDocumentHandlerResponse();
			documentResponse.ContentEncoding = RequestContentEncoding;
			documentResponse.ResponseResult = HtmlConvertor.ToJSON(result);
			documentResponse.ContentType = RequestContentType;
			return documentResponse;
		}
	}
	public class ApplyDataValidationWebCommand : DataValidationViewModelCommand {
		protected string ViewModel {
			get { return RequestParams[ViewModelParamName]; }
		}
		public ApplyDataValidationWebCommand(WebSpreadsheetCommandContext context)
			: base(context) {
		}
		protected DataValidationViewModel GetViewModelFromClient() {
			DataValidationCommandParameters parameters = SpreadsheetDataValidationCommand.CalculateCommandParameters();
			DataValidationViewModel viewModel = SpreadsheetDataValidationCommand.CreateDefaultViewModel(parameters.ActiveRange);
			Hashtable viewModelFromClient = HtmlConvertor.FromJSON<Hashtable>(ViewModel);
			viewModel.Type = ToEnum<DataValidationType>(viewModelFromClient["Type"]);
			viewModel.Operator = ToEnum<DataValidationOperator>(viewModelFromClient["Operator"]);
			viewModel.IgnoreBlank = (bool)viewModelFromClient["IgnoreBlank"];
			viewModel.InCellDropDown = (bool)viewModelFromClient["InCellDropDown"];
			viewModel.Formula1 = (string)viewModelFromClient["Formula1"];
			viewModel.Formula2 = (string)viewModelFromClient["Formula2"];
			viewModel.ShowMessage = (bool)viewModelFromClient["ShowMessage"];
			viewModel.MessageTitle = (string)viewModelFromClient["MessageTitle"];
			viewModel.Message = (string)viewModelFromClient["Message"];
			viewModel.ShowErrorMessage = (bool)viewModelFromClient["ShowErrorMessage"];
			viewModel.ErrorTitle = (string)viewModelFromClient["ErrorTitle"];
			viewModel.ErrorMessage = (string)viewModelFromClient["ErrorMessage"];
			viewModel.ErrorStyle = ToEnum<DataValidationErrorStyle>(viewModelFromClient["ErrorStyle"]);
			return viewModel;
		}
		protected T ToEnum<T>(object obj) {
			return (T)Enum.Parse(typeof(T), obj.ToString());
		}
		protected override void CustomExecuteImplementation() {
			SpreadsheetDataValidationCommand.ApplyChanges(GetViewModelFromClient());
		}
	}
	public class FetchListAllowedValuesWebCommand : CustomCommandExecuteImplementationBase {
		const string AllowedValuesParamName = "allowedValues";
		public FetchListAllowedValuesWebCommand(WebSpreadsheetCommandContext context)
			: base(context) {
		}
		public int ColumnIndex {
			get { return ActiveSheet.Selection.ActiveCell.Column; }
		}
		public int RowIndex {
			get { return ActiveSheet.Selection.ActiveCell.Row; }
		}
		protected string CustomResultProcessingFunction {
			get {
				return SpreadSheetCustomResultProcessingFunctionHelper.GetCustomResultProcessingFunctionNameFromParams(RequestParams);
			}
		}
		public List<string> GetAllowedValues() {
			List<string> values = new List<string>();
			foreach(DataValidation dataValidation in ActiveSheet.DataValidations) {
				if(dataValidation.Type == DataValidationType.List && !dataValidation.SuppressDropDown) {
					if(dataValidation.CellRange.ContainsCell(ColumnIndex, RowIndex)) {
						DataValidationInplaceValueStorage valueStorage = DataValidationAllowedValueCalculator.CalculateAllowedValues(dataValidation.Expression1, ActiveSheet);
						return GetStringValuesFromStorage(valueStorage);
					}
				}
			}
			return values;
		}
		public override DocumentHandlerResponse GetResult(SpreadsheetRenderHelper renderHelper) {
			var result = new Hashtable();
			result[AllowedValuesParamName] = GetAllowedValues();
			SpreadSheetCustomResultProcessingFunctionHelper.SetCustomResultProcessingFunction(result, CustomResultProcessingFunction);
			JSONDocumentHandlerResponse documentResponse = new JSONDocumentHandlerResponse();
			documentResponse.ContentEncoding = RequestContentEncoding;
			documentResponse.ResponseResult = HtmlConvertor.ToJSON(result);
			documentResponse.ContentType = RequestContentType;
			return documentResponse;
		}
		protected List<string> GetStringValuesFromStorage(DataValidationInplaceValueStorage valueStorage) {
			List<string> values = new List<string>();
			if(valueStorage.IsTextValue)
				values = valueStorage.TextAllowedValues;
			else {
				foreach(DataValidationInplaceValue allowedValue in valueStorage.DataValidationInplaceAllowedValues) {
					values.Add(allowedValue.DisplayText);
				}
			}
			return values;
		}
		protected override void CustomExecuteImplementation() {
		}
	}
	public class FetchMessageForCellWebCommand : CustomCommandExecuteImplementationBase {
		const string Title = "title";
		const string Text = "text";
		public FetchMessageForCellWebCommand(WebSpreadsheetCommandContext context)
			: base(context) {
		}
		public int ColumnIndex {
			get { return ActiveSheet.Selection.ActiveCell.Column; }
		}
		public int RowIndex {
			get { return ActiveSheet.Selection.ActiveCell.Row; }
		}
		protected string CustomResultProcessingFunction {
			get {
				return SpreadSheetCustomResultProcessingFunctionHelper.GetCustomResultProcessingFunctionNameFromParams(RequestParams);
			}
		}
		public override DocumentHandlerResponse GetResult(SpreadsheetRenderHelper renderHelper) {
			var result = new Hashtable();
			var messageValidation = GetMessageValidationForCell();
			if(messageValidation != null) {
				result[Title] = messageValidation.PromptTitle;
				result[Text] = messageValidation.Prompt;
			}
			SpreadSheetCustomResultProcessingFunctionHelper.SetCustomResultProcessingFunction(result, CustomResultProcessingFunction);
			JSONDocumentHandlerResponse documentResponse = new JSONDocumentHandlerResponse();
			documentResponse.ContentEncoding = RequestContentEncoding;
			documentResponse.ResponseResult = HtmlConvertor.ToJSON(result);
			documentResponse.ContentType = RequestContentType;
			return documentResponse;
		}
		protected DataValidation GetMessageValidationForCell() {
			foreach(DataValidation dataValidation in ActiveSheet.DataValidations) {
				if(dataValidation.ShowInputMessage && !String.IsNullOrEmpty(dataValidation.Prompt)) {
					if(dataValidation.CellRange.ContainsCell(ColumnIndex, RowIndex)) {
						return dataValidation;
					}
				}
			}
			return null;
		}
		protected override void CustomExecuteImplementation() {
		}
	}
	class MoveRangeWebCommand : CustomCommandExecuteImplementationBase {
		protected const string TargetRangeParamName = "Target";
		public MoveRangeWebCommand(WebSpreadsheetCommandContext context)
			: base(context) {
		}
		CellRange SourceRange {
			get { return ActiveSheet.Selection.ActiveRange; }
		}
		CellRangeBase TargetRange {
			get {
				ArrayList ltrb = HtmlConvertor.FromJSON<ArrayList>(RequestParams[TargetRangeParamName]);
				return SpreadsheetRenderHelper.ConvertLTRBToCellRange(ltrb, ActiveSheet);
			}
		}
		protected override void CustomExecuteImplementation() {
			var ranges = new DevExpress.XtraSpreadsheet.Model.CopyOperation.SourceTargetRangesForCopy(SourceRange, TargetRange);
			DevExpress.XtraSpreadsheet.Model.CopyOperation.RangeCopyOperation operation = new DevExpress.XtraSpreadsheet.Model.CopyOperation.CutRangeOperation(ranges);
			if(operation.RangesInfo.First.SourceAndTargetEquals)
				return;
			bool valid = operation.Validate();
			if(!valid)
				return;
			Model.BeginUpdate();
			try {
				operation.Execute();
				ActiveSheet.Selection.SetSelection(TargetRange);
				if(operation.CutMode) {
					DevExpress.XtraSpreadsheet.Model.CellRangeBase rangeToClear = operation.GetRangeToClearAfterCut();
					ActiveSheet.ClearAll(rangeToClear, InnerControl.ErrorHandler);
					ActiveSheet.ClearCellsNoShift(rangeToClear);
				}
			} finally {
				Model.EndUpdate();
			}
		}
	}
}
