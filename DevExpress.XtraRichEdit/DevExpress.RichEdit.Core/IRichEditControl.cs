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
using System.ComponentModel.Design;
using System.Drawing;
using System.Runtime.InteropServices;
using DevExpress.Utils;
using DevExpress.Utils.Commands;
using DevExpress.Office.Forms;
using DevExpress.Office.Internal;
using DevExpress.XtraRichEdit.Commands;
using DevExpress.XtraRichEdit.Internal;
using DevExpress.XtraRichEdit.Model;
using DevExpress.XtraRichEdit.Utils;
using DevExpress.XtraRichEdit.Internal.PrintLayout;
using DevExpress.XtraRichEdit.Tables.Native;
using DevExpress.XtraRichEdit.Forms;
using DevExpress.XtraRichEdit.API.Native;
using System.Windows.Forms;
using DevExpress.Compatibility.System.Windows.Forms;
using DevExpress.Compatibility.System.Drawing;
using DevExpress.XtraRichEdit.Layout;
using DevExpress.Compatibility.System.ComponentModel.Design;
namespace DevExpress.XtraRichEdit {
#region DocumentCapability
	[ComVisible(true)]
	public enum DocumentCapability {
		Default = 0,
		Disabled = 1,
		Enabled = 2,
		Hidden = 3
	}
#endregion
	public interface IRichEditControl : IBatchUpdateable, IServiceContainer, IWin32Window, ICommandAwareControl<RichEditCommandId> {
		InnerRichEditControl InnerControl { get; }
		InnerRichEditDocumentServer InnerDocumentServer { get; }
		Document Document { get; }
		bool IsPrintingAvailable { get; }
		bool IsPrintPreviewAvailable { get; }
		Cursor Cursor { get; set; }
		bool UseGdiPlus { get; }
		bool Overtype { get; set; }
		bool OvertypeAllowed { get; }
		AutoSizeMode AutoSizeMode { get; }
		DialogResult ShowWarningMessage(string message);
		DialogResult ShowErrorMessage(string message);
		bool IsHyperlinkActive();
		void RedrawEnsureSecondaryFormattingComplete(RefreshAction action);
		void UpdateUIFromBackgroundThread(Action method);
		void ShowCaret();
		void HideCaret();
		void OnViewPaddingChanged();
		void Print();
		void ShowPrintDialog();
		void ShowPrintPreview();
		void ShowBookmarkForm();
		void ShowSearchForm();
		void ShowReplaceForm();
		bool CanShowNumberingListForm { get; }
		void ShowNumberingListForm(DevExpress.XtraRichEdit.Model.ParagraphList paragraphs, ShowNumberingListFormCallback callback, object callbackData);
		void ShowEditStyleForm(DevExpress.XtraRichEdit.Model.ParagraphStyle paragraphSourceStyle, ParagraphIndex index, ShowEditStyleFormCallback callback);
		void ShowEditStyleForm(DevExpress.XtraRichEdit.Model.CharacterStyle characterSourceStyle, ParagraphIndex index, ShowEditStyleFormCallback callback);
		void ShowTableStyleForm(DevExpress.XtraRichEdit.Model.TableStyle style);
		void ShowParagraphForm(MergedParagraphProperties paragraphProperties, ShowParagraphFormCallback callback, object callbackData);
		void ShowFontForm(MergedCharacterProperties characterProperties, ShowFontFormCallback callback, object callbackData);
		void ShowTabsForm(TabFormattingInfo tabInfo, int defaultTabWidth, ShowTabsFormCallback callback, object callbackData);
		void ShowHyperlinkForm(HyperlinkInfo hyperlinkInfo, RunInfo runInfo, string title, ShowHyperlinkFormCallback callback);
		void ShowSymbolForm(InsertSymbolViewModel viewModel);
		void ShowInsertMergeFieldForm();
		void ShowInsertTableForm(CreateTableParameters parameters, ShowInsertTableFormCallback callback, object callbackData);
		void ShowInsertTableCellsForm(TableCellsParameters parameters, ShowInsertDeleteTableCellsFormCallback callback, object callbackData);
		void ShowDeleteTableCellsForm(TableCellsParameters parameters, ShowInsertDeleteTableCellsFormCallback callback, object callbackData);
		void ShowSplitTableCellsForm(SplitTableCellsParameters parameters, ShowSplitTableCellsFormCallback callback, object callbackData);
		void ShowRangeEditingPermissionsForm();
		void ShowDocumentProtectionQueryNewPasswordForm(PasswordInfo passwordInfo, PasswordFormCallback callback);
		void ShowDocumentProtectionQueryPasswordForm(PasswordInfo passwordInfo, PasswordFormCallback callback);
		void ShowLineNumberingForm(LineNumberingInfo properties, ShowLineNumberingFormCallback callback, object callbackData);
		void ShowPageSetupForm(PageSetupInfo properties, ShowPageSetupFormCallback callback, object callbackData, PageSetupFormInitialTabPage initialTabPage);
		void ShowColumnsSetupForm(ColumnsInfoUI properties, ShowColumnsSetupFormCallback callback, object callbackData);
		void ShowPasteSpecialForm(PasteSpecialInfo properties, ShowPasteSpecialFormCallback callback, object callbackData);
		void ShowTablePropertiesForm(SelectedCellsCollection selectedCells);
		void ShowBorderShadingForm(SelectedCellsCollection selectedCells);
		void ShowFloatingInlineObjectLayoutOptionsForm(FloatingInlineObjectParameters floatingObjectParameters, ShowFloatingInlineObjectLayoutOptionsFormCallback callback, object callbackData);
		void ShowTableOptionsForm(DevExpress.XtraRichEdit.Model.Table table);
		void ShowTOCForm(DevExpress.XtraRichEdit.Model.Field field);
		void ShowLanguageForm(DevExpress.XtraRichEdit.Model.DocumentModel documentModel);
		void ShowReviewingPaneForm(DevExpress.XtraRichEdit.Model.DocumentModel documentModel, CommentViewInfo commentViewInfo, bool selectParagraph);
		void ShowReviewingPaneForm(DevExpress.XtraRichEdit.Model.DocumentModel documentModel, CommentViewInfo commentViewInfo, DocumentLogPosition start, DocumentLogPosition end, bool setFocus);
		void CloseReviewingPaneForm();
		void ScrollToCaret(); 
		bool IsVisibleReviewingPane();
		Rectangle ViewBounds { get; }
		bool ShowCaretInReadOnly { get; }
		bool UseStandardDragDropMode { get; }
		void UpdateControlAutoSize();
		DragDropEffects DoDragDrop(object data, DragDropEffects allowedEffects);
		IPlatformSpecificScrollBarAdapter CreatePlatformSpecificScrollBarAdapter();
		RichEditViewVerticalScrollController CreateRichEditViewVerticalScrollController(RichEditView view);
		RichEditViewHorizontalScrollController CreateRichEditViewHorizontalScrollController(RichEditView view);
		DevExpress.XtraRichEdit.Mouse.RichEditMouseHandlerStrategyFactory CreateRichEditMouseHandlerStrategyFactory();
		TextColors SkinTextColors { get; }
		int SkinLeftMargin { get; }
		int SkinRightMargin { get; }
		int SkinTopMargin { get; }
		int SkinBottomMargin { get; }
		bool UseSkinMargins { get; }
		void ForceFlushPendingTextInput();
	}
	public interface IRulerControl {
		bool IsVisible { get; }
		int GetRulerSizeInPixels();
		IRichEditControl RichEditControl { get; }
	}
#region DragDropMode
	public enum DragDropMode {
		Standard,
		Manual
	}
#endregion
	public delegate void ShowFontFormCallback(MergedCharacterProperties properties, object data);
	public delegate void ShowParagraphFormCallback(MergedParagraphProperties properties, object data);
	public delegate void ShowEditStyleFormCallback(IStyle sourceStyle, IStyle targetStyle);
	public delegate void ShowTableStyleFormCallback(DevExpress.XtraRichEdit.Model.TableStyle sourceStyle, DevExpress.XtraRichEdit.Model.TableStyle targetStyle);
	public delegate void ShowLineNumberingFormCallback(LineNumberingInfo properties, object data);
	public delegate void ShowPageSetupFormCallback(PageSetupInfo properties, object data);
	public delegate void ShowColumnsSetupFormCallback(ColumnsInfoUI properties, object data);
	public delegate void ShowPasteSpecialFormCallback(PasteSpecialInfo properties, object data);
	public delegate void ShowTabsFormCallback(TabFormattingInfo tabInfo, int defaultTabWidth, object data);
	public delegate void ShowNumberingListFormCallback(DevExpress.XtraRichEdit.Model.ParagraphList paragraphs, object data);
	public delegate void ShowSymbolFormCallback(SymbolProperties symbolProperties, object data);
	public delegate void ShowHyperlinkFormCallback(HyperlinkInfo hyperlinkInfo, TextToDisplaySource source, RunInfo runInfo, string text);
	public delegate void ShowInsertTableFormCallback(CreateTableParameters parameters, object data);
	public delegate void ShowInsertDeleteTableCellsFormCallback(TableCellsParameters parameters, object data);
	public delegate void ShowSplitTableCellsFormCallback(SplitTableCellsParameters parameters, object data);
	public delegate void PasswordFormCallback(PasswordInfo passwordInfo);
	public delegate void ShowFloatingInlineObjectLayoutOptionsFormCallback(FloatingInlineObjectParameters floatingInlineObjectParameters, object data);
}
