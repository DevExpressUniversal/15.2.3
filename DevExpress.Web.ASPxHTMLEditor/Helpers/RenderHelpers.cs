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

using System.Collections.Generic;
using DevExpress.Web.Internal;
using System.Text;
namespace DevExpress.Web.ASPxHtmlEditor.Rendering {
	public class ASPxHtmlEditorRenderHelper {
		private ASPxHtmlEditor htmlEditor;
		private ASPxHtmlEditorScripts scripts;
		public ASPxHtmlEditorRenderHelper(ASPxHtmlEditor htmlEditor) {
			this.htmlEditor = htmlEditor;
			this.scripts = new ASPxHtmlEditorScripts(this.htmlEditor);
		}
		private ASPxHtmlEditor HtmlEditor { get { return htmlEditor; } }
		public ASPxHtmlEditorScripts Scripts { get { return scripts; } }
		public string TagInspectorWrapperID { get { return "TI"; } }
		public string TagInspectorTagsContainerID { get { return "TITC"; } }
		public string TagInspectorControlsWrapperID { get { return "TICW"; } }
		public string TagInspectorTagRemoveButtonID { get { return "TITRB"; } }
		public string TagInspectorTagPropertiesButtonID { get { return "TITPB"; } }
		public string BarDockControlID { get { return "TD"; } }
		public string ContentHtmlStateKey { get { return "html"; } }
		public string ValidationStateKey { get { return "validationState"; } }
		public string ContextMenuID { get { return "PPM"; } }
		public string CurrentDialogStateKey { get { return "currentDialog"; } }
		public string CssFilesStateKey { get { return "cssFiles"; } }
		public string ClientStateKey { get { return "clientState"; } }
		public string DesignViewTabName { get { return "D"; } }
		public string DesignViewCell { get { return "DesignViewCell"; } }
		public string DesignViewIFrameID { get { return "DesignIFrame"; } }
		public string DesignViewIFrameName { get { return string.Format("{0}_{1}",HtmlEditor.ClientID, DesignViewIFrameID); } }
		public string DialogPopupFormID { get { return "DPP"; } }
		public string ErrorFrameID { get { return "EF"; } }
		public string ErrorTextCellID { get { return "ETC"; } }
		public string ErrorFrameCloseButtonID { get { return "EFCB"; } }
		public string EditAreaCellID { get { return "EdtCell"; } }
		public string SystemPopupID { get { return "SPC"; } }
		public string FakeFocusInputID { get { return "FFI"; } }
		public string HtmlEditorMainCellID { get { return "MainCell"; } }
		public string SourceEditorID { get { return "SourceEditor"; } }
		public string HtmlViewEditID { get { return "HtmlViewEdit"; } }
		public string HtmlViewTabName { get { return "H"; } }
		public string PreviewCellID { get { return "PreviewCell"; } }
		public string PreviewIFrameID { get { return "PreviewIFrame"; } }
		public string PreviewIFrameName { get { return string.Format("{0}_{1}", HtmlEditor.ClientID, PreviewIFrameID); } }
		public string PreviewTabName { get { return "P"; } }
		public string StatusBarCellID { get { return "SBarCell"; } }
		public string SizeGripID { get { return "SG"; } }
		public string ToolbarRowID { get { return "TBRow"; } }
		public string TabControlID { get { return "TC"; } }
		public string PasteOptionsBarControlID { get { return "POB"; } }
		public string IntelliSenseListBoxId { get { return "ListBox"; } }
	}
	public class ASPxHtmlEditorScripts {
		const string ContextMenuItemClickHandlerFormat = "function(s, e) {{ ASPx.HEContextMenuItemClick('{0}', e); }}";
		const string ContextMenuCloseUpHandlerFormat = "function(s, e) {{ ASPx.HEContextMenuCloseUp('{0}'); }}";
		const string ChangeModeHandlerFormat = "function(s, e) {{ ASPx.HEChangeActiveView('{0}', e); }}";
		const string ChangingModeHandlerFormat = "function(s, e) {{ ASPx.HEChangingActiveView('{0}', e); }}";
		const string RibbonCommandHandlerFormat = "function(s, e) {{ ASPx.HEToolbarCommand('{0}', e.item, e.parameter); }}";
		const string RibbonMinimizationStateChangedHandlerFormat = "function(s, e) {{ ASPx.HERibbonMinimizationStateChanged('{0}', e); }}";
		const string RibbonActiveTabChangedHandlerFormat = "function(s, e) {{ ASPx.HERibbonActiveTabChanged('{0}', e); }}";
		const string PasteOptionsItemClickHandlerFormat = "function(s, e) {{ ASPx.HEPasteOptionsItemClick('{0}', e); }}";
		private ASPxHtmlEditor htmlEditor;
		public ASPxHtmlEditorScripts(ASPxHtmlEditor htmlEditor) {
			this.htmlEditor = htmlEditor;
		}
		private ASPxHtmlEditor HtmlEditor { get { return htmlEditor; } }
		public string ContextMenuItemClickHandleName { get { return string.Format(ContextMenuItemClickHandlerFormat, HtmlEditor.ClientID); } }
		public string ContextMenuCloseUpHandleName { get { return string.Format(ContextMenuCloseUpHandlerFormat, HtmlEditor.ClientID); } }
		public string ChangeViewHandler { get { return string.Format(ChangeModeHandlerFormat, HtmlEditor.ClientID); } }
		public string ChangingViewHandler { get { return string.Format(ChangingModeHandlerFormat, HtmlEditor.ClientID); } }
		public string RibbonCommandHandler { get { return string.Format(RibbonCommandHandlerFormat, HtmlEditor.ClientID); } }
		public string RibbonMinimizationStateChangedHandler { get { return string.Format(RibbonMinimizationStateChangedHandlerFormat, HtmlEditor.ClientID); } }
		public string RibbonActiveTabChanged { get { return string.Format(RibbonActiveTabChangedHandlerFormat, HtmlEditor.ClientID); } }
		public string PasteOptionsItemClick { get { return string.Format(PasteOptionsItemClickHandlerFormat, HtmlEditor.ClientID); } }
	}
	public class ASPxHtmlEditorCallbackReader {
		private const string ProcessHtmlPrefixPart = "ProcessHtml_";
		public const string SpellCheckPrefix = "SpellCheck";
		public const string SpellCheckLoadControlPrefix = "SpellCheckLoadControl";
		public const string SpellCheckerOptionsPrefix = "SpellCheckerOptions";
		public const string SwitchToDesignViewCallbackPrefix = ProcessHtmlPrefixPart + "Design";
		public const string SwitchToHtmlViewCallbackPrefix = ProcessHtmlPrefixPart + "Html";
		public const string SwitchToPreviewCallbackPrefix = ProcessHtmlPrefixPart + "Preview";
		public const string SaveImageToServerCallbackPrefix = "ImageToServer";
		public const string ThumbnailImageWidthCallbackPrefix = "TNIW";
		public const string ThumbnailImageHeightCallbackPrefix = "TNIH";
		public const string ThumbnailImageFileNameCallbackPrefix = "TNIF";
		public const string FileManagerCallbackPrefix = "FileManager";
		public const string SaveFlashToServerCallbackPrefix = "FSC";
		public const string SaveFlashToServerErrorCallbackPrefix = "FSE";
		public const string SaveFlashToServerNewUrlCallbackPrefix = "FSU";
		public const string SaveAudioToServerCallbackPrefix = "ASC";
		public const string SaveAudioToServerErrorCallbackPrefix = "ASE";
		public const string SaveAudioToServerNewUrlCallbackPrefix = "ASU";
		public const string SaveVideoToServerCallbackPrefix = "VSC";
		public const string SaveVideoToServerErrorCallbackPrefix = "VSE";
		public const string SaveVideoToServerNewUrlCallbackPrefix = "VSU";
		internal const string CreateThumbnailImageCallbackPrefix = "TNI";
		internal const string SaveImageToServerErrorCallbackPrefix = "ISE";
		internal const string SaveImageToServerNewUrlCallbackPrefix = "ISU";
		private Dictionary<string, object> callbackArgs = new Dictionary<string, object>();
		public ASPxHtmlEditorCallbackReader(string callbackArgs) {
			string[] prefixes = new string[] {
				RenderUtils.DialogFormCallbackStatus,
				SwitchToDesignViewCallbackPrefix, SwitchToHtmlViewCallbackPrefix, SwitchToPreviewCallbackPrefix,
				SaveImageToServerCallbackPrefix,
				ThumbnailImageWidthCallbackPrefix, ThumbnailImageHeightCallbackPrefix, ThumbnailImageFileNameCallbackPrefix,
				SpellCheckPrefix, SpellCheckerOptionsPrefix, FileManagerCallbackPrefix
			};
			DictionarySerializer.Deserialize(callbackArgs, this.callbackArgs);
		}
		public string SwitchToDesignView { get { return GetCallbackArg(SwitchToDesignViewCallbackPrefix); } }
		public string SwitchToHtmlView { get { return GetCallbackArg(SwitchToHtmlViewCallbackPrefix); } }
		public string SwitchToPreview { get { return GetCallbackArg(SwitchToPreviewCallbackPrefix); } }
		public string DialogFormName { get { return GetCallbackArg(RenderUtils.DialogFormCallbackStatus); } }
		public string SpellCheck { get { return GetCallbackArg(SpellCheckPrefix); } }
		public string SpellCheckLoadControl { get { return GetCallbackArg(SpellCheckLoadControlPrefix); } }
		public string SpellCheckerOptions { get { return GetCallbackArg(SpellCheckerOptionsPrefix); } }
		public string ImageUrl { get { return GetCallbackArg(SaveImageToServerCallbackPrefix); } }
		public string FlashUrl { get { return GetCallbackArg(SaveFlashToServerCallbackPrefix); } }
		public string AudioUrl { get { return GetCallbackArg(SaveAudioToServerCallbackPrefix); } }
		public string VideoUrl { get { return GetCallbackArg(SaveVideoToServerCallbackPrefix); } }
		public int ThumbnailImageWidth { get { return ParseInt(GetCallbackArg(ThumbnailImageWidthCallbackPrefix)); } }
		public int ThumbnailImageHeight { get { return ParseInt(GetCallbackArg(ThumbnailImageHeightCallbackPrefix)); } }
		public string ThumbnailImageFileName { get { return GetCallbackArg(ThumbnailImageFileNameCallbackPrefix); } }
		public bool IsFileManagerCallback { get { return !string.IsNullOrEmpty(FileManagerCallbackData); } }
		public string FileManagerCallbackData { get { return GetCallbackArg(FileManagerCallbackPrefix); } }
		public bool IsViewModeSwitchCallback { get { return SwitchToDesignView != null || SwitchToHtmlView != null || SwitchToPreview != null; } }
		public bool IsSpellCheckCallback { get { return SpellCheck != null || SpellCheckerOptions != null || SpellCheckLoadControl != null; } }
		public bool IsDialogFormCallback { get { return !string.IsNullOrEmpty(DialogFormName); } }
		public bool IsImageUploadCallback { get { return !string.IsNullOrEmpty(ImageUrl); } }
		public bool IsFlashUploadCallback { get { return !string.IsNullOrEmpty(FlashUrl); } }
		public bool IsAudioUploadCallback { get { return !string.IsNullOrEmpty(AudioUrl); } }
		public bool IsVideoUploadCallback { get { return !string.IsNullOrEmpty(VideoUrl); } }
		private string GetCallbackArg(string callbackPrefix) {
			return callbackArgs.ContainsKey(callbackPrefix) ? (string)callbackArgs[callbackPrefix] : null;
		}
		private int ParseInt(string value) {
			int result;
			return int.TryParse(value, out result) ? result : -1;
		}
	}
}
