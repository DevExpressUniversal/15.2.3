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

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI.WebControls;
using DevExpress.Utils;
using DevExpress.Web.ASPxHtmlEditor.Internal;
using DevExpress.Web.ASPxHtmlEditor.Localization;
using DevExpress.Web.Internal;
namespace DevExpress.Web.ASPxHtmlEditor {
	public partial class ASPxHtmlEditor {
		protected internal const string DesignViewIframeLoadHandler = "ASPx.HEDesignViewIframeOnLoad(\"{0}\", iframe);";
		protected internal const string HtmlEditorConstantsResourceName = HtmlEditorScriptsResourcePath + "HtmlEditorConstants.js";
		protected internal const string HtmlEditorCommandsResourceName = HtmlEditorScriptsResourcePath + "Commands.js";
		protected internal const string HtmlEditorSelectionResourceName = HtmlEditorScriptsResourcePath + "Selection.js";
		protected internal const string HtmlEditorSelectionManagerResourceName = HtmlEditorScriptsResourcePath + "SelectionManager.js";
		protected internal const string HtmlEditorUtilsScriptsResourcePath = HtmlEditorScriptsResourcePath + "Utils.";
		protected internal const string HtmlEditorDoctypeDescriptionResourceName = HtmlEditorUtilsScriptsResourcePath + "DoctypeDescription.js";
		protected internal const string HtmlEditorDialogsResourceName = HtmlEditorScriptsResourcePath + "Dialogs.";
		protected internal const string HtmlEditorBaseDialogsResourceName = HtmlEditorDialogsResourceName + "BaseDialogs.js";
		protected internal const string HtmlEditorCommonDialogsResourceName = HtmlEditorDialogsResourceName + "CommonDialogs.js";
		protected internal const string HtmlEditorMediaDialogsResourceName = HtmlEditorDialogsResourceName + "MediaDialogs.js";
		protected internal const string HtmlEditorTableDialogsResourceName = HtmlEditorDialogsResourceName + "TableDialogs.js";
		protected internal const string HtmlEditorSearchScriptResourceName = HtmlEditorScriptsResourcePath + "Search.js";
		protected internal const string HtmlEditorScriptResourceName = HtmlEditorScriptsResourcePath + "HtmlEditor.js";
		protected internal const string HtmlEditorCoreScriptResourceName = HtmlEditorScriptsResourcePath + "HtmlEditorCore.js";
		protected internal const string HtmlEditorHtmlProcessingScriptResourceName = HtmlEditorScriptsResourcePath + "HtmlProcessing.js";
		protected internal const string HtmlEditorEventManagerScriptResourceName = HtmlEditorScriptsResourcePath + "EventManager.js";
		protected internal const string HtmlEditorClientStateManagerResourceName = HtmlEditorScriptsResourcePath + "ClientStateManager.js";
		protected internal const string HtmlEditorLayoutCalculatorResourceName = HtmlEditorScriptsResourcePath + "LayoutCalculator.js";
		protected internal const string HtmlEditorKeyboardManagerResourceName = HtmlEditorScriptsResourcePath + "KeyboardManager.js";
		protected internal const string HtmlEditorValidationManagerResourceName = HtmlEditorScriptsResourcePath + "ValidationManager.js";
		protected internal const string HtmlEditorLayoutManagerResourceName = HtmlEditorScriptsResourcePath + "LayoutManager.js";
		protected internal const string HtmlEditorBaseWrapperResourceName = HtmlEditorWrappersScriptsResourcePath + "BaseWrapper.js";
		protected internal const string HtmlEditorDesignViewIFrameWrapperResourceName = HtmlEditorWrappersScriptsResourcePath + "DesignViewIFrameWrapper.js";
		protected internal const string HtmlEditorHtmlViewWrapperResourceName = HtmlEditorWrappersScriptsResourcePath + "HtmlViewWrapper.js";
		protected internal const string HtmlEditorHtmlViewMemoWrapperResourceName = HtmlEditorWrappersScriptsResourcePath + "HtmlViewMemoWrapper.js";
		protected internal const string HtmlEditorIFrameWrapperResourceName = HtmlEditorWrappersScriptsResourcePath + "IFrameWrapper.js";
		protected internal const string HtmlEditorPreviewIFrameWrapperResourceName = HtmlEditorWrappersScriptsResourcePath + "PreviewIFrameWrapper.js";
		protected internal const string HtmlEditorContextMenuManagerResourceName = HtmlEditorUIScriptsResourcePath + "ContextMenuManager.js";
		protected internal const string HtmlEditorStatusBarManagerResourceName = HtmlEditorUIScriptsResourcePath + "StatusBarManager.js";
		protected internal const string HtmlEditorTagInspectorResourceName = HtmlEditorUIScriptsResourcePath + "TagInspector.js";
		protected internal const string HtmlEditorBarDockManagerResourceName = HtmlEditorUIScriptsResourcePath + "BarDockManager.js";
		protected internal const string HtmlEditorPasteOptionsBarManagerResourceName = HtmlEditorUIScriptsResourcePath + "PasteOptionsBarManager.js";
		protected internal const string HtmlEditorFindManagerResourceName = HtmlEditorUIScriptsResourcePath + "FindManager.js";
		protected internal const string HtmlEditorIntelliSenseManagerResourceName = HtmlEditorUIScriptsResourcePath + "IntelliSenseManager.js";
		protected internal const string HtmlEditorFrameworksScriptsResourcePath = HtmlEditorScriptsResourcePath + "Frameworks.";
		protected internal const string HtmlEditorCodeMirrorModeScriptsResourcePath = HtmlEditorFrameworksScriptsResourcePath + "Mode.";
		protected internal const string HtmlEditorCodeMirrorAddonScriptsResourcePath = HtmlEditorFrameworksScriptsResourcePath + "Addon.";
		protected internal const string HtmlEditorCodeMirrorResourceName = HtmlEditorFrameworksScriptsResourcePath + "codemirror.js";
		protected internal const string HtmlEditorCodeMirrorCssModeResourceName = HtmlEditorCodeMirrorModeScriptsResourcePath + "css.js";
		protected internal const string HtmlEditorCodeMirrorHtmlMixedModeResourceName = HtmlEditorCodeMirrorModeScriptsResourcePath + "htmlmixed.js";
		protected internal const string HtmlEditorCodeMirrorJavaScriptModeResourceName = HtmlEditorCodeMirrorModeScriptsResourcePath + "javascript.js";
		protected internal const string HtmlEditorCodeMirrorXmlModeResourceName = HtmlEditorCodeMirrorModeScriptsResourcePath + "xml.js";
		protected internal const string HtmlEditorCodeMirrorActiveLineAddonResourceName = HtmlEditorCodeMirrorAddonScriptsResourcePath + "active-line.js";
		protected internal const string HtmlEditorCodeMirrorCloseTagAddonResourceName = HtmlEditorCodeMirrorAddonScriptsResourcePath + "closetag.js";
		protected internal const string HtmlEditorCodeMirrorCommentAddonResourceName = HtmlEditorCodeMirrorAddonScriptsResourcePath + "comment.js";
		protected internal const string HtmlEditorCodeMirrorMatchTagsAddonResourceName = HtmlEditorCodeMirrorAddonScriptsResourcePath + "matchtags.js";
		protected internal const string HtmlEditorCodeMirrorXmlFoldAddonResourceName = HtmlEditorCodeMirrorAddonScriptsResourcePath + "xml-fold.js";
		protected internal const string HtmlEditorCodeMirrorCommentFoldAddonResourceName = HtmlEditorCodeMirrorAddonScriptsResourcePath + "comment-fold.js";
		protected internal const string HtmlEditorCodeMirrorFoldCodeAddonResourceName = HtmlEditorCodeMirrorAddonScriptsResourcePath + "foldcode.js";
		protected internal const string HtmlEditorCodeMirrorFoldGutterAddonResourceName = HtmlEditorCodeMirrorAddonScriptsResourcePath + "foldgutter.js";
		protected internal const string HtmlEditorBeautifyHtmlResourceName = HtmlEditorFrameworksScriptsResourcePath + "beautify-html.js";
		protected internal const string HtmlEditorInsertPlaceholderCommandResourceName = HtmlEditorCommandsScriptsResourcePath + "InsertPlaceholderCommand.js";
		protected internal const string HtmlEditorDeleteElementCommandResourceName = HtmlEditorCommandsScriptsResourcePath + "DeleteElementCommand.js";
		protected internal const string HtmlEditorInsertMediaCommandResourceName = HtmlEditorCommandsScriptsResourcePath + "InsertMediaCommand.js";
		protected internal const string HtmlEditorChangeElementPropertiesCommandResourceName = HtmlEditorCommandsScriptsResourcePath + "ChangeElementPropertiesCommand.js";
		protected internal const string HtmlEditorApplyCssCommandResourceName = HtmlEditorCommandsScriptsResourcePath + "ApplyCssCommand.js";
		protected internal const string HtmlEditorBgColorCommandResourceName = HtmlEditorCommandsScriptsResourcePath + "BgColorCommand.js";
		protected internal const string HtmlEditorBrowserCommandBaseResourceName = HtmlEditorCommandsScriptsResourcePath + "BrowserCommandBase.js";
		protected internal const string HtmlEditorChangeImageCommandResourceName = HtmlEditorCommandsScriptsResourcePath + "ChangeImageCommand.js";
		protected internal const string HtmlEditorCheckSpellingCommandResourceName = HtmlEditorCommandsScriptsResourcePath + "CheckSpellingCommand.js";
		protected internal const string HtmlEditorCheckSpellingCoreCommandResourceName = HtmlEditorCommandsScriptsResourcePath + "CheckSpellingCoreCommand.js";
		protected internal const string HtmlEditorClipboardCommandResourceName = HtmlEditorCommandsScriptsResourcePath + "ClipboardCommand.js";
		protected internal const string HtmlEditorCommandArgumentsResourceName = HtmlEditorCommandsScriptsResourcePath + "CommandArguments.js";
		protected internal const string HtmlEditorCommandBaseResourceName = HtmlEditorCommandsScriptsResourcePath + "CommandBase.js";
		protected internal const string HtmlEditorDeleteCommandResourceName = HtmlEditorCommandsScriptsResourcePath + "DeleteCommand.js";
		protected internal const string HtmlEditorDeleteWithoutSelectionCommandResourceName = HtmlEditorCommandsScriptsResourcePath + "DeleteWithoutSelectionCommand.js";
		protected internal const string HtmlEditorEnterCommandResourceName = HtmlEditorCommandsScriptsResourcePath + "EnterCommand.js";
		protected internal const string HtmlEditorFontColorCommandResourceName = HtmlEditorCommandsScriptsResourcePath + "FontColorCommand.js";
		protected internal const string HtmlEditorFontNameCommandResourceName = HtmlEditorCommandsScriptsResourcePath + "FontNameCommand.js";
		protected internal const string HtmlEditorFontSizeCommandResourceName = HtmlEditorCommandsScriptsResourcePath + "FontSizeCommand.js";
		protected internal const string HtmlEditorFontStyleCommandResourceName = HtmlEditorCommandsScriptsResourcePath + "FontStyleCommand.js";
		protected internal const string HtmlEditorFormatBlockCommandResourceName = HtmlEditorCommandsScriptsResourcePath + "FormatBlockCommand.js";
		protected internal const string HtmlEditorFullscreenCommandResourceName = HtmlEditorCommandsScriptsResourcePath + "FullscreenCommand.js";
		protected internal const string HtmlEditorIndentCommandResourceName = HtmlEditorCommandsScriptsResourcePath + "IndentCommand.js";
		protected internal const string HtmlEditorQuickSearchCommandResourceName = HtmlEditorCommandsScriptsResourcePath + "QuickSearchCommand.js";
		protected internal const string HtmlEditorFindAndReplaceDialogCommandResourceName = HtmlEditorCommandsScriptsResourcePath + "FindAndReplaceDialogCommand.js";
		protected internal const string HtmlEditorInsertImageCommandResourceName = HtmlEditorCommandsScriptsResourcePath + "InsertImageCommand.js";
		protected internal const string HtmlEditorInsertLinkCommandResourceName = HtmlEditorCommandsScriptsResourcePath + "InsertLinkCommand.js";
		protected internal const string HtmlEditorInsertListCommandResourceName = HtmlEditorCommandsScriptsResourcePath + "InsertListCommand.js";
		protected internal const string HtmlEditorJustifyCommandResourceName = HtmlEditorCommandsScriptsResourcePath + "JustifyCommand.js";
		protected internal const string HtmlEditorKbCopyCommandResourceName = HtmlEditorCommandsScriptsResourcePath + "KbCopyCommand.js";
		protected internal const string HtmlEditorKbCutCommandResourceName = HtmlEditorCommandsScriptsResourcePath + "KbCutCommand.js";
		protected internal const string HtmlEditorKbPasteCommandResourceName = HtmlEditorCommandsScriptsResourcePath + "KbPasteCommand.js";
		protected internal const string HtmlEditorPasteOptionsCommandResourceName = HtmlEditorCommandsScriptsResourcePath + "PasteOptionsCommand.js";
		protected internal const string HtmlEditorLineBreakTypeCommandResourceName = HtmlEditorCommandsScriptsResourcePath + "LineBreakTypeCommand.js";
		protected internal const string HtmlEditorNewParagraphTypeCommandResourceName = HtmlEditorCommandsScriptsResourcePath + "NewParagraphTypeCommand.js";
		protected internal const string HtmlEditorPasteFromWordCommandResourceName = HtmlEditorCommandsScriptsResourcePath + "PasteFromWordCommand.js";
		protected internal const string HtmlEditorPasteHtmlCommandResourceName = HtmlEditorCommandsScriptsResourcePath + "PasteHtmlCommand.js";
		protected internal const string HtmlEditorPrintCommandResourceName = HtmlEditorCommandsScriptsResourcePath + "PrintCommand.js";
		protected internal const string HtmlEditorRedoCommandResourceName = HtmlEditorCommandsScriptsResourcePath + "RedoCommand.js";
		protected internal const string HtmlEditorRemoveFormatCommandResourceName = HtmlEditorCommandsScriptsResourcePath + "RemoveFormatCommand.js";
		protected internal const string HtmlEditorSaveAsCommandResourceName = HtmlEditorCommandsScriptsResourcePath + "SaveAsCommand.js";
		protected internal const string HtmlEditorSelectAllCommandResourceName = HtmlEditorCommandsScriptsResourcePath + "SelectAllCommand.js";
		protected internal const string HtmlEditorTextTypeCommandResourceName = HtmlEditorCommandsScriptsResourcePath + "TextTypeCommand.js";
		protected internal const string HtmlEditorUndoCommandResourceName = HtmlEditorCommandsScriptsResourcePath + "UndoCommand.js";
		protected internal const string HtmlEditorUnlinkCommandResourceName = HtmlEditorCommandsScriptsResourcePath + "UnlinkCommand.js";
		protected internal const string HtmlEditorWrappedCommandBaseResourceName = HtmlEditorCommandsScriptsResourcePath + "WrappedCommandBase.js";
		protected internal const string HtmlEditorContextTabStateCommandResourceName = HtmlEditorCommandsScriptsResourcePath + "ContextTabStateCommand.js";
		protected internal const string HtmlEditorFormatDocumentCommandResourceName = HtmlEditorCommandsScriptsResourcePath + "FormatDocumentCommand.js";
		protected internal const string HtmlEditorDialogCommandResourceName = HtmlEditorCommandsScriptsResourcePath + "DialogCommands." + "DialogCommands.js";
		protected internal const string HtmlEditorTableCellCommandResourceName = HtmlEditorCommandsScriptsResourcePath + "TableCommands." + "TableCellCommands.js";
		protected internal const string HtmlEditorTableColumnCommandResourceName = HtmlEditorCommandsScriptsResourcePath + "TableCommands." + "TableColumnCommands.js";
		protected internal const string HtmlEditorTableCommandResourceName = HtmlEditorCommandsScriptsResourcePath + "TableCommands." + "TableCommands.js";
		protected internal const string HtmlEditorTableRowCommandResourceName = HtmlEditorCommandsScriptsResourcePath + "TableCommands." + "TableRowCommands.js";
		protected internal const string HtmlEditorCMCommandsScriptsResourcePath = HtmlEditorCommandsScriptsResourcePath + "HtmlViewCMCommands.";
		protected internal const string HtmlEditorIntelliSenseCommandsResourceName = HtmlEditorCMCommandsScriptsResourcePath + "IntelliSenseCommands.js";
		protected internal const string HtmlEditorCommentHtmlCommandResourceName = HtmlEditorCMCommandsScriptsResourcePath + "CommentHtmlCommand.js";
		protected internal const string HtmlEditorIndentLineCommandResourceName = HtmlEditorCMCommandsScriptsResourcePath + "IndentLineCommand.js";
		protected internal const string HtmlEditorCollapseTagCommandResourceName = HtmlEditorCMCommandsScriptsResourcePath + "CollapseTagCommand.js";
		protected internal const string ErrorFrameCloseButtonClickHandlerName = "ASPx.HEEFCBClick(event, '{0}')";
		protected internal const string ButtonImageIdPostfix = "Img";
		protected internal const string HtmlEditorAudioImageResourceName = HtmlEditorImagesResourcePath + "dxheAudio.png";
		protected internal const string HtmlEditorFlashImageResourceName = HtmlEditorImagesResourcePath + "dxheFlash.png";
		protected internal const string HtmlEditorVideoImageResourceName = HtmlEditorImagesResourcePath + "dxheVideo.png";
		protected internal const string HtmlEditorYouTubeImageResourceName = HtmlEditorImagesResourcePath + "dxheYoutube.png";
		protected internal const string HtmlEditorNotSupportedImageResourceName = HtmlEditorImagesResourcePath + "dxheNotSupported.png";
		protected override string GetClientObjectClassName() {
			return "ASPxClientHtmlEditor";
		}
		protected internal string GetDesignViewIframeOnLoadHandler() {
			return
				@"if(typeof(ASPx.HEDesignViewIframeOnLoad) != 'undefined') {
                var iframe = window.event;
                        setTimeout(function() {" +
							string.Format(DesignViewIframeLoadHandler, ClientID) +
						@"}, 200);
                } else { window.event.target.loadUnhandled = true; }";
		}
		protected override bool HasHoverScripts() {
			return true;
		}
		protected override void AddHoverItems(StateScriptRenderHelper helper) {
			helper.AddStyle(GetErrorFrameCloseButtonHoverStyle(), GetErrorFrameCloseButtonID(), new string[0],
				GetErrorFrameCloseButtonImage().GetHottrackedScriptObject(Page), ButtonImageIdPostfix, IsEnabled());
		}
		protected override void RegisterScriptBlocks() {
			base.RegisterScriptBlocks();
			string script = string.Format("ASPx.HtmlEditorDialogSR={{PasteFromWord:{0},InsertImage:{1},ChangeImage:{2},InsertLink:{3},ChangeLink:{4},InsertTable:{5}," +
				"ChangeTable:{6},ChangeTableCell:{7},ChangeTableColumn:{8},ChangeTableRow:{9},InsertFlash:{10},ChangeFlash:{11}," +
				"InsertVideo:{12},ChangeVideo:{13},InsertAudio:{14},ChangeAudio:{15},InsertYouTubeVideo:{16},ChangeYouTubeVideo:{17},InsertPlaceholder:{18},ChangePlaceholder:{19}}}",
				HtmlConvertor.ToScript(ASPxHtmlEditorLocalizer.GetString(ASPxHtmlEditorStringId.CommandPasteRtf)),
				HtmlConvertor.ToScript(ASPxHtmlEditorLocalizer.GetString(ASPxHtmlEditorStringId.InsertImage)),
				HtmlConvertor.ToScript(ASPxHtmlEditorLocalizer.GetString(ASPxHtmlEditorStringId.ChangeImage)),
				HtmlConvertor.ToScript(ASPxHtmlEditorLocalizer.GetString(ASPxHtmlEditorStringId.InsertLink)),
				HtmlConvertor.ToScript(ASPxHtmlEditorLocalizer.GetString(ASPxHtmlEditorStringId.ChangeLink)),
				HtmlConvertor.ToScript(ASPxHtmlEditorLocalizer.GetString(ASPxHtmlEditorStringId.InsertTable)),
				HtmlConvertor.ToScript(ASPxHtmlEditorLocalizer.GetString(ASPxHtmlEditorStringId.TableProperties)),
				HtmlConvertor.ToScript(ASPxHtmlEditorLocalizer.GetString(ASPxHtmlEditorStringId.TableCellProperties)),
				HtmlConvertor.ToScript(ASPxHtmlEditorLocalizer.GetString(ASPxHtmlEditorStringId.TableColumnProperties)),
				HtmlConvertor.ToScript(ASPxHtmlEditorLocalizer.GetString(ASPxHtmlEditorStringId.TableRowProperties)),
				HtmlConvertor.ToScript(ASPxHtmlEditorLocalizer.GetString(ASPxHtmlEditorStringId.InsertFlash)),
				HtmlConvertor.ToScript(ASPxHtmlEditorLocalizer.GetString(ASPxHtmlEditorStringId.ChangeFlash)),
				HtmlConvertor.ToScript(ASPxHtmlEditorLocalizer.GetString(ASPxHtmlEditorStringId.InsertVideo)),
				HtmlConvertor.ToScript(ASPxHtmlEditorLocalizer.GetString(ASPxHtmlEditorStringId.ChangeVideo)),
				HtmlConvertor.ToScript(ASPxHtmlEditorLocalizer.GetString(ASPxHtmlEditorStringId.InsertAudio)),
				HtmlConvertor.ToScript(ASPxHtmlEditorLocalizer.GetString(ASPxHtmlEditorStringId.ChangeAudio)),
				HtmlConvertor.ToScript(ASPxHtmlEditorLocalizer.GetString(ASPxHtmlEditorStringId.InsertYouTubeVideo)),
				HtmlConvertor.ToScript(ASPxHtmlEditorLocalizer.GetString(ASPxHtmlEditorStringId.ChangeYouTubeVideo)),
				HtmlConvertor.ToScript(ASPxHtmlEditorLocalizer.GetString(ASPxHtmlEditorStringId.InsertPlaceholder)),
				HtmlConvertor.ToScript(ASPxHtmlEditorLocalizer.GetString(ASPxHtmlEditorStringId.ChangePlaceholder)));
			RegisterScriptBlock("HtmlEditorDialogSR", RenderUtils.GetScriptHtml(script));
		}
		protected override bool HasFunctionalityScripts() {
			return true;
		}
		protected override void RegisterIncludeScripts() {
			base.RegisterIncludeScripts();
			RegisterIncludeScript(typeof(ASPxHtmlEditor), HtmlEditorConstantsResourceName);
			RegisterDialogUtilsScripts();
			if(SpellChecker != null)
				SpellChecker.RegisterIncludeScripts();
			if(Settings.AllowHtmlView) {
				if(IsAdvancedHtmlEditingMode())
					RegisterCodeMirrorFrameworkScripts();
				RegisterIncludeScript(typeof(ASPxHtmlEditor), HtmlEditorBeautifyHtmlResourceName);
			}
			string[] sortedResourcePathList = new string[] {
				HtmlEditorSelectionResourceName,
				HtmlEditorSelectionManagerResourceName,
				HtmlEditorCommandsResourceName,
				HtmlEditorTagInspectorResourceName,
				HtmlEditorKeyboardManagerResourceName,
				HtmlEditorStatusBarManagerResourceName,
				HtmlEditorContextMenuManagerResourceName,
				HtmlEditorValidationManagerResourceName,
				HtmlEditorPasteOptionsBarManagerResourceName,
				HtmlEditorLayoutManagerResourceName,
				HtmlEditorCommandArgumentsResourceName,
				HtmlEditorCommandBaseResourceName,
				HtmlEditorBrowserCommandBaseResourceName,
				HtmlEditorWrappedCommandBaseResourceName,
				HtmlEditorInsertPlaceholderCommandResourceName,
				HtmlEditorDeleteElementCommandResourceName,
				HtmlEditorInsertMediaCommandResourceName,
				HtmlEditorChangeElementPropertiesCommandResourceName,
				HtmlEditorApplyCssCommandResourceName,
				HtmlEditorChangeImageCommandResourceName,
				HtmlEditorCheckSpellingCommandResourceName,
				HtmlEditorCheckSpellingCoreCommandResourceName,
				HtmlEditorClipboardCommandResourceName,
				HtmlEditorDeleteCommandResourceName,
				HtmlEditorDeleteWithoutSelectionCommandResourceName,
				HtmlEditorEnterCommandResourceName,
				HtmlEditorFontColorCommandResourceName,
				HtmlEditorBgColorCommandResourceName,
				HtmlEditorFontNameCommandResourceName,
				HtmlEditorFontSizeCommandResourceName,
				HtmlEditorFontStyleCommandResourceName,
				HtmlEditorFormatBlockCommandResourceName,
				HtmlEditorFullscreenCommandResourceName,
				HtmlEditorIndentCommandResourceName,
				HtmlEditorInsertImageCommandResourceName,
				HtmlEditorInsertLinkCommandResourceName,
				HtmlEditorInsertListCommandResourceName,
				HtmlEditorJustifyCommandResourceName,
				HtmlEditorNewParagraphTypeCommandResourceName,
				HtmlEditorPasteFromWordCommandResourceName,
				HtmlEditorPasteHtmlCommandResourceName,
				HtmlEditorPasteOptionsCommandResourceName,
				HtmlEditorPrintCommandResourceName,
				HtmlEditorRedoCommandResourceName,
				HtmlEditorRemoveFormatCommandResourceName,
				HtmlEditorSaveAsCommandResourceName,
				HtmlEditorSelectAllCommandResourceName,
				HtmlEditorTextTypeCommandResourceName,
				HtmlEditorKbCopyCommandResourceName,
				HtmlEditorKbCutCommandResourceName,
				HtmlEditorKbPasteCommandResourceName,
				HtmlEditorLineBreakTypeCommandResourceName,
				HtmlEditorUndoCommandResourceName,
				HtmlEditorUnlinkCommandResourceName,
				HtmlEditorQuickSearchCommandResourceName,
				HtmlEditorFindAndReplaceDialogCommandResourceName,
				HtmlEditorFormatDocumentCommandResourceName,
				HtmlEditorContextTabStateCommandResourceName,
				HtmlEditorCommentHtmlCommandResourceName,
				HtmlEditorIndentLineCommandResourceName,
				HtmlEditorCollapseTagCommandResourceName,
				HtmlEditorDialogCommandResourceName,
				HtmlEditorTableCommandResourceName,
				HtmlEditorTableColumnCommandResourceName,
				HtmlEditorTableRowCommandResourceName,
				HtmlEditorTableCellCommandResourceName,
				HtmlEditorBaseDialogsResourceName,
				HtmlEditorCommonDialogsResourceName,
				HtmlEditorMediaDialogsResourceName,
				HtmlEditorTableDialogsResourceName,
				HtmlEditorIntelliSenseCommandsResourceName,
				HtmlEditorClientStateManagerResourceName,
				HtmlEditorLayoutCalculatorResourceName,
				HtmlEditorEventManagerScriptResourceName,
				HtmlEditorHtmlProcessingScriptResourceName,
				HtmlEditorBaseWrapperResourceName,
				HtmlEditorHtmlViewMemoWrapperResourceName,
				HtmlEditorHtmlViewWrapperResourceName,
				HtmlEditorIFrameWrapperResourceName,
				HtmlEditorPreviewIFrameWrapperResourceName,
				HtmlEditorDesignViewIFrameWrapperResourceName,
				HtmlEditorBarDockManagerResourceName,
				HtmlEditorCoreScriptResourceName,
				HtmlEditorSearchScriptResourceName,
				HtmlEditorScriptResourceName
			};
			bool isAdvancedHtmlEditingMode = IsAdvancedHtmlEditingMode();
			foreach(string resourceName in sortedResourcePathList) {
				if((resourceName == HtmlEditorHtmlViewMemoWrapperResourceName && isAdvancedHtmlEditingMode) ||
					(resourceName == HtmlEditorHtmlViewWrapperResourceName && !isAdvancedHtmlEditingMode))
					continue;
				RegisterIncludeScript(typeof(ASPxHtmlEditor), resourceName);
			}
			RegisterIncludeScript(typeof(ASPxEditBase), ASPxEditBaseAccessor.EditScriptResourceAccessName); 
		}
		void RegisterCodeMirrorFrameworkScripts() {
			List<string> resourcePathList = new List<string>();
			resourcePathList.AddRange(new string[] { 
				HtmlEditorCodeMirrorResourceName, 
				HtmlEditorCodeMirrorCssModeResourceName ,
				HtmlEditorCodeMirrorJavaScriptModeResourceName,
				HtmlEditorCodeMirrorXmlModeResourceName,
				HtmlEditorCodeMirrorHtmlMixedModeResourceName,
				HtmlEditorCodeMirrorCommentAddonResourceName,
				HtmlEditorCodeMirrorCommentFoldAddonResourceName, 
				HtmlEditorCodeMirrorFoldCodeAddonResourceName,
				HtmlEditorCodeMirrorMatchTagsAddonResourceName,
				HtmlEditorCodeMirrorXmlFoldAddonResourceName
			});
			if(Settings.SettingsHtmlView.EnableTagAutoClosing)
				resourcePathList.Add(HtmlEditorCodeMirrorCloseTagAddonResourceName);
			if(Settings.SettingsHtmlView.HighlightActiveLine)
				resourcePathList.Add(HtmlEditorCodeMirrorActiveLineAddonResourceName);
			if(Settings.SettingsHtmlView.ShowCollapseTagButtons)
				resourcePathList.Add(HtmlEditorCodeMirrorFoldGutterAddonResourceName);
			if(Settings.SettingsHtmlView.EnableAutoCompletion) {
				resourcePathList.Add(HtmlEditorDoctypeDescriptionResourceName);
				resourcePathList.Add(HtmlEditorIntelliSenseManagerResourceName);
			}
			foreach(string resourceName in resourcePathList)
				RegisterIncludeScript(typeof(ASPxHtmlEditor), resourceName);
		}
		void AppendClientScriptObjectProperty(StringBuilder stb, string localVarName, string key, string value) {
			if(!string.IsNullOrEmpty(value))
				stb.AppendFormat("{0}.{1} = {2};\n", localVarName, key, HtmlConvertor.ToScript(value));
		}
		protected override void GetCreateClientObjectScript(StringBuilder stb, string localVarName, string clientName) {
			base.GetCreateClientObjectScript(stb, localVarName, clientName);
			stb.AppendFormat("{0}.appDomainPath = '{1}';\n", localVarName, UrlUtils.AppDomainAppVirtualPathString);
			if(SettingsHtmlEditing.AllowEditFullDocument)
				stb.Append(localVarName + ".allowEditFullDocument = true;\n");
			if(SettingsHtmlEditing.ResourcePathMode != ResourcePathMode.NotSet)
				stb.AppendFormat("{0}.resourcePathMode = {1};\n", localVarName, (int)SettingsHtmlEditing.ResourcePathMode);
			if(SettingsHtmlEditing.EnablePasteOptions)
				stb.Append(localVarName + ".enablePasteOptions = true;\n");
			if(SettingsHtmlEditing.AllowYouTubeVideoIFrames)
				stb.Append(localVarName + ".allowYouTubeVideoIFrames = true;\n");
			if(SettingsHtmlEditing.PasteMode != HtmlEditorPasteMode.SourceFormatting)
				stb.AppendFormat("{0}.pasteMode = '{1}';\n", localVarName, SettingsHtmlEditing.PasteMode.ToString());
			if(SettingsHtmlEditing.AllowHTML5MediaElements)
				stb.Append(localVarName + ".allowHTML5MediaElements = true;\n");
			if(SettingsHtmlEditing.AllowObjectAndEmbedElements)
				stb.Append(localVarName + ".allowObjectAndEmbedElements = true;\n");
			if(Settings.AllowScriptExecutionInPreview)
				stb.Append(localVarName + ".allowScriptExecution = true;\n");
			if(SettingsResize.AllowResize) {
				stb.Append(localVarName + ".allowResize = true;\n");
				stb.AppendFormat("{0}.layoutManager.setResizeRanges({1}, {2}, {3}, {4});\n", localVarName,
					SettingsResize.MinWidth, SettingsResize.MinHeight, SettingsResize.MaxWidth, SettingsResize.MaxHeight);
			}
			if(Settings.AllowHtmlView) {
				if(!IsAdvancedHtmlEditingMode())
					stb.AppendFormat("{0}.htmlEditingMode = 'Simple';\n", localVarName);
				else {
					if(SettingsHtmlEditing.AllowedDocumentType != AllowedDocumentType.XHTML)
						stb.AppendFormat("{0}.documentType = '{1}';\n", localVarName, SettingsHtmlEditing.AllowedDocumentType.ToString());
					if(Settings.SettingsHtmlView.EnableAutoCompletion) {
						stb.Append(localVarName + ".enableAutoCompletion = true;\n");
						stb.AppendFormat("{0}.{1};\n", localVarName, GetHtmlViewAutoCompletionIconSettingsString(this));
					}
					if(Settings.SettingsHtmlView.ShowCollapseTagButtons)
						stb.Append(localVarName + ".htmlViewSettingsShowCollapseTagButtons = true;\n");
					if(Settings.SettingsHtmlView.ShowLineNumbers)
						stb.Append(localVarName + ".htmlViewSettingsShowLineNumbers = true;\n");
					if(Settings.SettingsHtmlView.HighlightActiveLine)
						stb.Append(localVarName + ".htmlViewSettingsHighlightActiveLine = true;\n");
					if(Settings.SettingsHtmlView.HighlightMatchingTags)
						stb.Append(localVarName + ".htmlViewSettingsHighlightMatchingTags = true;\n");
					if(!Settings.SettingsHtmlView.EnableTagAutoClosing)
						stb.Append(localVarName + ".htmlViewSettingsEnableTagAutoClosing = false;\n");
				}
			}
			if(!string.IsNullOrEmpty(Html))
				stb.AppendFormat("{0}.html = {1};\n", localVarName, HtmlConvertor.ToScript(Html));
			AppendClientScriptObjectProperty(stb, localVarName, "designViewAreaStyleCssText", GetDesignViewAreaStyleForScript().GetStyleAttributes(Page).Value);
			AppendClientScriptObjectProperty(stb, localVarName, "previewViewAreaStyleCssText", GetPreviewAreaStyleForScript().GetStyleAttributes(Page).Value);
			AppendClientScriptObjectProperty(stb, localVarName, "docStyleCssText", GetDocumentStyleCssText());
			if(!AreDictionariesAssigned())
				stb.Append(localVarName + ".spellCheckerHelper.areDictionariesAssigned = false;\n");
			if(SpellChecker != null)
				stb.AppendFormat("{0}.spellCheckerHelper.scStartOptions = {1};\n", localVarName, HtmlConvertor.ToJSON(SpellChecker.Options));
			if(SettingsHtmlEditing.AllowedDocumentType != AllowedDocumentType.HTML5 && !SettingsHtmlEditing.UpdateDeprecatedElements)
				stb.Append(localVarName + ".updateDeprecatedElements = false;\n");
			if(!SettingsHtmlEditing.UpdateBoldItalic)
				stb.Append(localVarName + ".updateBoldItalic = false;\n");
			stb.AppendFormat("{0}.advancedSearchOfLocalization = {1};\r\n", localVarName, HtmlConvertor.ToScript(ASPxHtmlEditorLocalizer.GetString(ASPxHtmlEditorStringId.AdvancedSearch_Of)));
			stb.AppendFormat("{0}.validationManager.clientValidationEnabled = {1};\n", localVarName, IsClientValidationEnabled() ? "true" : "false");
			stb.AppendFormat("{0}.validationManager.validationPatterns = {1};\n", localVarName, SettingsValidation.GetClientValidationPatternsArray());
			stb.AppendFormat("{0}.validationManager.errorText = {1};\n", localVarName, HtmlConvertor.ToScript(ErrorText));
			stb.AppendFormat("{0}.validationManager.isValid = {1};\n", localVarName, IsValid ? "true" : "false");
			if(SettingsValidation.ValidationGroup != "")
				stb.AppendFormat("{0}.validationGroup = \"{1}\";\n", localVarName, SettingsValidation.ValidationGroup);
			if(NotifyValidationSummariesToAcceptNewError)
				stb.Append(localVarName + ".validationManager.notifyValidationSummariesToAcceptNewError = true;\n");
			if(Shortcuts.Count > 0) {
				if(Settings.AllowDesignView)
					GenerateDesignViewShortcutsList(stb, localVarName);
				if(Settings.AllowHtmlView)
					GenerateHtmlViewShortcutsList(stb, localVarName);
				if(Settings.AllowPreview)
					GeneratePreviewShortcutsList(stb, localVarName);
			}
			if(Placeholders.Count > 0)
				stb.AppendFormat("{0}.placeholders = {1};\r\n", localVarName, HtmlConvertor.ToJSON(Placeholders.GetValuesArray()));
			if(SettingsHtmlEditing.ContentElementFiltering.Tags.Length > 0 || SettingsHtmlEditing.ContentElementFiltering.TagFilterMode == HtmlEditorFilterMode.WhiteList)
				stb.AppendFormat("{0}.tagFilterSettings = {1}\r\n", localVarName, GetFilterSettingsString(SettingsHtmlEditing.ContentElementFiltering.Tags, SettingsHtmlEditing.ContentElementFiltering.TagFilterMode));
			if(SettingsHtmlEditing.ContentElementFiltering.Attributes.Length > 0 || SettingsHtmlEditing.ContentElementFiltering.AttributeFilterMode == HtmlEditorFilterMode.WhiteList)
				stb.AppendFormat("{0}.attributeFilterSettings = {1}\r\n", localVarName, GetFilterSettingsString(SettingsHtmlEditing.ContentElementFiltering.Attributes, SettingsHtmlEditing.ContentElementFiltering.AttributeFilterMode));
			if(SettingsHtmlEditing.ContentElementFiltering.StyleAttributes.Length > 0 || SettingsHtmlEditing.ContentElementFiltering.StyleAttributeFilterMode == HtmlEditorFilterMode.WhiteList)
				stb.AppendFormat("{0}.styleAttributeFilterSettings = {1}\r\n", localVarName, GetFilterSettingsString(SettingsHtmlEditing.ContentElementFiltering.StyleAttributes, SettingsHtmlEditing.ContentElementFiltering.StyleAttributeFilterMode));
			GenerateClientSettings(stb, localVarName);
			GenerateClientSettingsHtmlEditing(stb, localVarName);
			GenerateClientCustomDialogsCaptions(stb, localVarName);
			GetCreateHoverState(stb, localVarName);
			if(Settings.ShowTagInspector && Settings.AllowDesignViewInternal) {
				stb.AppendFormat("{0}.tagInspectorSeparatorImageHtml = {1};\n", localVarName, HtmlConvertor.ToScript(GetCreateSeparatorImageHtml()));
				stb.Append(localVarName + ".enableTagInspector = true;\n");
				stb.AppendFormat("{0}.tagInspectorStyles = {1};\n", localVarName, GetTagInspectorStylesString());
			}
		}
		protected string GetCreateSeparatorImageHtml() {
			Image img = RenderUtils.CreateImage();
			ImageProperties imageProperties = GetTagInspectorSeparatorImageProperties();
			imageProperties.AssignToControl(img, false);
			return RenderUtils.GetRenderResult(img);
		}
		protected string GetTagInspectorStylesString() {
			Hashtable result = new Hashtable();
			result.Add("tagStyle", GetStyleHashtable(StylesTagInspector.GetTagStyle(), StylesTagInspector.GetTagHoverStyle()));
			result.Add("selectedTagStyle", GetStyleHashtable(StylesTagInspector.GetSelectedTagStyle(), StylesTagInspector.GetSelectedTagHoverStyle()));
			result.Add("ellipsisStyle", GetStyleHashtable(StylesTagInspector.GetEllipsisStyle()));
			result.Add("selectedTagContainerStyle", GetStyleHashtable(StylesTagInspector.GetSelectedTagContainerStyle()));
			result.Add("selectionStyle", GetStyleHashtable(StylesTagInspector.GetSelectionStyle()));
			return HtmlConvertor.ToJSON(result);
		}
		protected Hashtable GetStyleHashtable(AppearanceStyleBase style, AppearanceStyleBase hoverStyle) {
			Hashtable result = GetStyleHashtable((AppearanceStyleBase)style);
			if(hoverStyle != null)
				result.Add("hoverStyle", GetStyleHashtable(hoverStyle));
			return result;
		}
		protected Hashtable GetStyleHashtable(AppearanceStyleBase style) {
			Hashtable result = new Hashtable();
			string cssText = style.GetStyleAttributes(Page).Value;
			if(!string.IsNullOrEmpty(cssText))
				result.Add("cssText", cssText);
			if(!string.IsNullOrEmpty(style.CssClass))
				result.Add("className", style.CssClass);
			return result;
		}
		protected virtual void GetStyleScript(StringBuilder stb, string localVarName, AppearanceStyleBase style, string cssField) {
			string cssText = style.GetStyleAttributes(Page).Value;
			if(!string.IsNullOrEmpty(cssText))
				stb.AppendFormat("{0}.{1} = {2};\n", localVarName, cssField, HtmlConvertor.ToScript(cssText));
		}
		protected virtual void GetStyleScript(StringBuilder stb, string localVarName, AppearanceStyleBase style, string classField, string cssField) {
			GetStyleScript(stb, localVarName, style, cssField);
			if(!string.IsNullOrEmpty(style.CssClass))
				stb.AppendFormat("{0}.{1} = {2};\n", localVarName, classField, HtmlConvertor.ToScript(style.CssClass));
		}
		protected virtual void GetStyleScript(StringBuilder stb, string localVarName, AppearanceStyleBase style, object spriteProp, string classField, string cssField, string spriteField) {
			GetStyleScript(stb, localVarName, style, classField, cssField);
			stb.AppendFormat("{0}.{1} = {2};\n", localVarName, spriteField, HtmlConvertor.ToJSON(spriteProp));
		}
		protected void GenerateShortcutsList(StringBuilder stb, string localVarName, string settingName, HtmlEditorView view) {
			Dictionary<string, string> scs = new Dictionary<string, string>();
			foreach(HtmlEditorShortcut sc in Shortcuts) {
				if(sc.IsShortcutAllowed(view))
					scs[sc.Shortcut] = sc.CommandName;
			}
			if(scs.Keys.Count > 0)
				stb.AppendFormat("{0}.{1} = {2};\n", localVarName, settingName, HtmlConvertor.ToJSON(scs));
		}
		protected void GenerateDesignViewShortcutsList(StringBuilder stb, string localVarName) {
			GenerateShortcutsList(stb, localVarName, "designViewShortcuts", HtmlEditorView.Design);
		}
		protected void GenerateHtmlViewShortcutsList(StringBuilder stb, string localVarName) {
			GenerateShortcutsList(stb, localVarName, "htmlViewShortcuts", HtmlEditorView.Html);
		}
		protected void GeneratePreviewShortcutsList(StringBuilder stb, string localVarName) {
			GenerateShortcutsList(stb, localVarName, "previewShortcuts", HtmlEditorView.Preview);
		}
		protected void GenerateClientSettings(StringBuilder stb, string localVarName) {
			DefaultBoolean allowContextMenu = Settings.AllowContextMenu;
			if(allowContextMenu == DefaultBoolean.True && ContextMenuItems.Count > 0 && ContextMenuItems.GetVisibleItemCount() == 0)
				allowContextMenu = DefaultBoolean.Default;
			if(allowContextMenu != DefaultBoolean.True) {
				stb.AppendFormat("{0}.allowContextMenu = {1};\n", localVarName, allowContextMenu == DefaultBoolean.Default
					? "'default'"
					: "'false'");
			}
			if(!Settings.AllowInsertDirectImageUrls)
				stb.Append(localVarName + ".allowInsertDirectImageUrls = false;\n");
		}
		protected void GenerateClientSettingsHtmlEditing(StringBuilder stb, string localVarName) {
			if(ScriptsAllowed)
				stb.Append(localVarName + ".allowScripts = true;\n");
			if(SettingsHtmlEditing.EnterMode != HtmlEditorEnterMode.Default)
				stb.AppendFormat("{0}.enterMode = '{1}';\n", localVarName, SettingsHtmlEditing.EnterMode.ToString());
		}
		protected void GenerateClientCustomDialogsCaptions(StringBuilder stb, string localVarName) {
			if(CustomDialogs.Count > 0) {
				List<string> dialogParts = new List<string>();
				foreach(HtmlEditorCustomDialog dialog in CustomDialogs) {
					dialogParts.Add(
						string.Format(
							"{0}:{1}",
							HtmlConvertor.ToScript(CustomDialogNamePrefix + dialog.Name),
							HtmlConvertor.ToScript(dialog.Caption)
						)
					);
				}
				stb.AppendFormat("{0}.customDialogsCaptions = {1};\n", localVarName, "{" + string.Join(",", dialogParts.ToArray()) + "}");
			}
		}
		string GetFilterSettingsString(string[] elements, HtmlEditorFilterMode filterMode) {
			return string.Format("{{ list: {0}, filterMode: '{1}' }};", HtmlConvertor.ToJSON(elements.ToList<string>()), filterMode.ToString());
		}
		string GetHtmlViewAutoCompletionIconSettingsString(ASPxHtmlEditor htmlEditor) {
			return string.Format("htmlViewAutoCompletionIcons = {{ fieldIconClassName: '{0}', eventIconClassName: '{1}', xmlItemIconClassName: '{2}', enumIconClassName: '{3}' }};\n",
				GetCompletionFieldItemImageProperties().SpriteProperties.CssClass,
				GetCompletionEventItemImageProperties().SpriteProperties.CssClass,
				GetCompletionXmlItemImageProperties().SpriteProperties.CssClass,
				GetCompletionEnumItemImageProperties().SpriteProperties.CssClass
			);
		}
		string GetResourceUrlByName(ASPxHtmlEditor htmlEditor, string name) {
			return ResourceManager.GetResourceUrl(htmlEditor.Page, typeof(ASPxHtmlEditor), name);
		}
	}
}
