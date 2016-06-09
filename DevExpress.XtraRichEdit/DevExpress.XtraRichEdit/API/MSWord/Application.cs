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
using System.CodeDom.Compiler;
using DevExpress.API.Mso;
namespace DevExpress.XtraRichEdit.API.Word {
	#region Application
	[GeneratedCode("Suppress FxCop check", "")]
	public interface Application : ApplicationInner, ApplicationEvents4_Event {
	}
	#endregion
	#region ApplicationInner
	[GeneratedCode("Suppress FxCop check", "")]
	public interface ApplicationInner : IWordObject {
		string Name { get; }
		Document ActiveDocument { get; }
		Selection Selection { get; }
		object WordBasic { get; }
		bool Visible { get; set; }
		string Version { get; }
		bool ScreenUpdating { get; set; }
		bool PrintPreview { get; set; }
		bool DisplayStatusBar { get; set; }
		bool SpecialMode { get; }
		int UsableWidth { get; }
		int UsableHeight { get; }
		bool MathCoprocessorAvailable { get; }
		bool MouseAvailable { get; }
		object this[WdInternationalIndex Index] { get; }
		string Build { get; }
		bool CapsLock { get; }
		bool NumLock { get; }
		string UserName { get; set; }
		string UserInitials { get; set; }
		string UserAddress { get; set; }
		object MacroContainer { get; }
		bool DisplayRecentFiles { get; set; }
		SynonymInfo this[string Word, object LanguageID] { get; } 
		string DefaultSaveFormat { get; set; }
		string ActivePrinter { get; set; }
		object CustomizationContext { get; set; }
		string Caption { get; set; }
		string Path { get; }
		bool DisplayScrollBars { get; set; }
		string StartupPath { get; set; }
		int BackgroundSavingStatus { get; }
		int BackgroundPrintingStatus { get; }
		int Left { get; set; }
		int Top { get; set; }
		int Width { get; set; }
		int Height { get; set; }
		WdWindowState WindowState { get; set; }
		bool DisplayAutoCompleteTips { get; set; }
		WdAlertLevel DisplayAlerts { get; set; }
		string PathSeparator { get; }
		string StatusBar { set; }
		bool MAPIAvailable { get; }
		bool DisplayScreenTips { get; set; }
		WdEnableCancelKey EnableCancelKey { get; set; }
		bool UserControl { get; }
		WdMailSystem MailSystem { get; }
		string DefaultTableSeparator { get; set; }
		bool ShowVisualBasicEditor { get; set; }
		string BrowseExtraFileTypes { get; set; }
		bool this[object Object] { get; }
		bool FocusInMailHeader { get; }
		void Quit(ref object SaveChanges, ref object OriginalFormat, ref object RouteDocument);
		void ScreenRefresh();
		void PrintOutOld(ref object Background, ref object Append, ref object Range, ref object OutputFileName, ref object From, ref object To, ref object Item, ref object Copies, ref object Pages, ref object PageType, ref object PrintToFile, ref object Collate, ref object FileName, ref object ActivePrinterMacGX, ref object ManualDuplexPrint);
		void LookupNameProperties(string Name);
		void SubstituteFont(string UnavailableFont, string SubstituteFont);
		bool Repeat(ref object Times);
		void DDEExecute(int Channel, string Command);
		int DDEInitiate(string App, string Topic);
		void DDEPoke(int Channel, string Item, string Data);
		string DDERequest(int Channel, string Item);
		void DDETerminate(int Channel);
		void DDETerminateAll();
		int BuildKeyCode(WdKey Arg1, ref object Arg2, ref object Arg3, ref object Arg4);
		string KeyString(int KeyCode, ref object KeyCode2);
		void OrganizerCopy(string Source, string Destination, string Name, WdOrganizerObject Object);
		void OrganizerDelete(string Source, string Name, WdOrganizerObject Object);
		void OrganizerRename(string Source, string Name, string NewName, WdOrganizerObject Object);
		void AddAddress(ref Array TagID, ref Array Value);
		string GetAddress(ref object Name, ref object AddressProperties, ref object UseAutoText, ref object DisplaySelectDialog, ref object SelectDialog, ref object CheckNamesDialog, ref object RecentAddressesChoice, ref object UpdateRecentAddresses);
		bool CheckGrammar(string String);
		bool CheckSpelling(string Word, ref object CustomDictionary, ref object IgnoreUppercase, ref object MainDictionary, ref object CustomDictionary2, ref object CustomDictionary3, ref object CustomDictionary4, ref object CustomDictionary5, ref object CustomDictionary6, ref object CustomDictionary7, ref object CustomDictionary8, ref object CustomDictionary9, ref object CustomDictionary10);
		void ResetIgnoreAll();
		void GoBack();
		void Help(ref object HelpType);
		void AutomaticChange();
		void ShowMe();
		void HelpTool();
		void ListCommands(bool ListAllCommands);
		void ShowClipboard();
		void OnTime(ref object When, string Name, ref object Tolerance);
		void NextLetter();
		short MountVolume(string Zone, string Server, string Volume, ref object User, ref object UserPassword, ref object VolumePassword);
		string CleanString(string String);
		void SendFax();
		void ChangeFileOpenDirectory(string Path);
		void RunOld(string MacroName);
		void GoForward();
		void Move(int Left, int Top);
		void Resize(int Width, int Height);
		float InchesToPoints(float Inches);
		float CentimetersToPoints(float Centimeters);
		float MillimetersToPoints(float Millimeters);
		float PicasToPoints(float Picas);
		float LinesToPoints(float Lines);
		float PointsToInches(float Points);
		float PointsToCentimeters(float Points);
		float PointsToMillimeters(float Points);
		float PointsToPicas(float Points);
		float PointsToLines(float Points);
		void Activate();
		float PointsToPixels(float Points, ref object fVertical);
		float PixelsToPoints(float Pixels, ref object fVertical);
		void KeyboardLatin();
		void KeyboardBidi();
		void ToggleKeyboard();
		int Keyboard(int LangId);
		string ProductCode();
		void DiscussionSupport(ref object Range, ref object cid, ref object piCSE);
		void SetDefaultTheme(string Name, WdDocumentMedium DocumentType);
		string GetDefaultTheme(WdDocumentMedium DocumentType);
		MsoLanguageID Language { get; }
		bool CheckLanguage { get; set; }
		bool Dummy1 { get; }
		MsoFeatureInstall FeatureInstall { get; set; }
		void PrintOut2000(ref object Background, ref object Append, ref object Range, ref object OutputFileName, ref object From, ref object To, ref object Item, ref object Copies, ref object Pages, ref object PageType, ref object PrintToFile, ref object Collate, ref object FileName, ref object ActivePrinterMacGX, ref object ManualDuplexPrint, ref object PrintZoomColumn, ref object PrintZoomRow, ref object PrintZoomPaperWidth, ref object PrintZoomPaperHeight);
		object Run(string MacroName, ref object varg1, ref object varg2, ref object varg3, ref object varg4, ref object varg5, ref object varg6, ref object varg7, ref object varg8, ref object varg9, ref object varg10, ref object varg11, ref object varg12, ref object varg13, ref object varg14, ref object varg15, ref object varg16, ref object varg17, ref object varg18, ref object varg19, ref object varg20, ref object varg21, ref object varg22, ref object varg23, ref object varg24, ref object varg25, ref object varg26, ref object varg27, ref object varg28, ref object varg29, ref object varg30);
		void PrintOut(ref object Background, ref object Append, ref object Range, ref object OutputFileName, ref object From, ref object To, ref object Item, ref object Copies, ref object Pages, ref object PageType, ref object PrintToFile, ref object Collate, ref object FileName, ref object ActivePrinterMacGX, ref object ManualDuplexPrint, ref object PrintZoomColumn, ref object PrintZoomRow, ref object PrintZoomPaperWidth, ref object PrintZoomPaperHeight);
		MsoAutomationSecurity AutomationSecurity { get; set; }
		string EmailTemplate { get; set; }
		bool ShowWindowsInTaskbar { get; set; }
		bool ShowStartupDialog { get; set; }
		bool DefaultLegalBlackline { get; set; }
		bool Dummy2();
		void PutFocusInMailHeader();
		bool ArbitraryXMLSupportAvailable { get; }
		string BuildFull { get; }
		string BuildFeatureCrew { get; }
		void LoadMasterList(string FileName);
		Document CompareDocuments(Document OriginalDocument, Document RevisedDocument, WdCompareDestination Destination, WdGranularity Granularity, bool CompareFormatting, bool CompareCaseChanges, bool CompareWhitespace, bool CompareTables, bool CompareHeaders, bool CompareFootnotes, bool CompareTextboxes, bool CompareFields, bool CompareComments, bool CompareMoves, string RevisedAuthor, bool IgnoreAllComparisonWarnings);
		Document MergeDocuments(Document OriginalDocument, Document RevisedDocument, WdCompareDestination Destination, WdGranularity Granularity, bool CompareFormatting, bool CompareCaseChanges, bool CompareWhitespace, bool CompareTables, bool CompareHeaders, bool CompareFootnotes, bool CompareTextboxes, bool CompareFields, bool CompareComments, bool CompareMoves, string OriginalAuthor, string RevisedAuthor, WdMergeFormatFrom FormatFrom);
		bool ShowStylePreviews { get; set; }
		bool RestrictLinkedStyles { get; set; }
		bool DisplayDocumentInformationPanel { get; set; }
		bool OpenAttachmentsInFullScreen { get; set; }
		int ActiveEncryptionSession { get; }
		bool DontResetInsertionPointProperties { get; set; }
	}
	#endregion
	[GeneratedCode("Suppress FxCop check", "")]
	public delegate void ApplicationEvents4_DocumentBeforeCloseEventHandler(Document Doc, ref bool Cancel);
	[GeneratedCode("Suppress FxCop check", "")]
	public delegate void ApplicationEvents4_DocumentBeforePrintEventHandler(Document Doc, ref bool Cancel);
	[GeneratedCode("Suppress FxCop check", "")]
	public delegate void ApplicationEvents4_DocumentBeforeSaveEventHandler(Document Doc, ref bool SaveAsUI, ref bool Cancel);
	[GeneratedCode("Suppress FxCop check", "")]
	public delegate void ApplicationEvents4_DocumentChangeEventHandler();
	[GeneratedCode("Suppress FxCop check", "")]
	public delegate void ApplicationEvents4_DocumentOpenEventHandler(Document Doc);
	[GeneratedCode("Suppress FxCop check", "")]
	public delegate void ApplicationEvents4_DocumentSyncEventHandler(Document Doc, MsoSyncEventType SyncEventType);
	[GeneratedCode("Suppress FxCop check", "")]
	public delegate void ApplicationEvents4_EPostageInsertEventHandler(Document Doc);
	[GeneratedCode("Suppress FxCop check", "")]
	public delegate void ApplicationEvents4_EPostageInsertExEventHandler(Document Doc, int cpDeliveryAddrStart, int cpDeliveryAddrEnd, int cpReturnAddrStart, int cpReturnAddrEnd, int xaWidth, int yaHeight, string bstrPrinterName, string bstrPaperFeed, bool fPrint, ref bool fCancel);
	[GeneratedCode("Suppress FxCop check", "")]
	public delegate void ApplicationEvents4_EPostagePropertyDialogEventHandler(Document Doc);
	[GeneratedCode("Suppress FxCop check", "")]
	public delegate void ApplicationEvents4_MailMergeAfterMergeEventHandler(Document Doc, Document DocResult);
	[GeneratedCode("Suppress FxCop check", "")]
	public delegate void ApplicationEvents4_MailMergeAfterRecordMergeEventHandler(Document Doc);
	[GeneratedCode("Suppress FxCop check", "")]
	public delegate void ApplicationEvents4_MailMergeBeforeMergeEventHandler(Document Doc, int StartRecord, int EndRecord, ref bool Cancel);
	[GeneratedCode("Suppress FxCop check", "")]
	public delegate void ApplicationEvents4_MailMergeBeforeRecordMergeEventHandler(Document Doc, ref bool Cancel);
	[GeneratedCode("Suppress FxCop check", "")]
	public delegate void ApplicationEvents4_MailMergeDataSourceLoadEventHandler(Document Doc);
	[GeneratedCode("Suppress FxCop check", "")]
	public delegate void ApplicationEvents4_MailMergeDataSourceValidateEventHandler(Document Doc, ref bool Handled);
	[GeneratedCode("Suppress FxCop check", "")]
	public delegate void ApplicationEvents4_MailMergeDataSourceValidate2EventHandler(Document Doc, ref bool Handled);
	[GeneratedCode("Suppress FxCop check", "")]
	public delegate void ApplicationEvents4_MailMergeWizardSendToCustomEventHandler(Document Doc);
	[GeneratedCode("Suppress FxCop check", "")]
	public delegate void ApplicationEvents4_MailMergeWizardStateChangeEventHandler(Document Doc, ref int FromState, ref int ToState, ref bool Handled);
	[GeneratedCode("Suppress FxCop check", "")]
	public delegate void ApplicationEvents4_NewDocumentEventHandler(Document Doc);
	[GeneratedCode("Suppress FxCop check", "")]
	public delegate void ApplicationEvents4_QuitEventHandler();
	[GeneratedCode("Suppress FxCop check", "")]
	public delegate void ApplicationEvents4_StartupEventHandler();
	[GeneratedCode("Suppress FxCop check", "")]
	public delegate void ApplicationEvents4_WindowBeforeDoubleClickEventHandler(Selection Sel, ref bool Cancel);
	[GeneratedCode("Suppress FxCop check", "")]
	public delegate void ApplicationEvents4_WindowBeforeRightClickEventHandler(Selection Sel, ref bool Cancel);
	[GeneratedCode("Suppress FxCop check", "")]
	public delegate void ApplicationEvents4_WindowSelectionChangeEventHandler(Selection Sel);
	[GeneratedCode("Suppress FxCop check", "")]
	public delegate void ApplicationEvents4_XMLSelectionChangeEventHandler(Selection Sel, XMLNode OldXMLNode, XMLNode NewXMLNode, ref int Reason);
	[GeneratedCode("Suppress FxCop check", "")]
	public delegate void ApplicationEvents4_XMLValidationErrorEventHandler(XMLNode XMLNode);
	#region ApplicationEvents4_Event
	[GeneratedCode("Suppress FxCop check", "")]
	public interface ApplicationEvents4_Event {
		event ApplicationEvents4_DocumentBeforeCloseEventHandler DocumentBeforeClose;
		event ApplicationEvents4_DocumentBeforePrintEventHandler DocumentBeforePrint;
		event ApplicationEvents4_DocumentBeforeSaveEventHandler DocumentBeforeSave;
		event ApplicationEvents4_DocumentChangeEventHandler DocumentChange;
		event ApplicationEvents4_DocumentOpenEventHandler DocumentOpen;
		event ApplicationEvents4_DocumentSyncEventHandler DocumentSync;
		event ApplicationEvents4_EPostageInsertEventHandler EPostageInsert;
		event ApplicationEvents4_EPostageInsertExEventHandler EPostageInsertEx;
		event ApplicationEvents4_EPostagePropertyDialogEventHandler EPostagePropertyDialog;
		event ApplicationEvents4_MailMergeAfterMergeEventHandler MailMergeAfterMerge;
		event ApplicationEvents4_MailMergeAfterRecordMergeEventHandler MailMergeAfterRecordMerge;
		event ApplicationEvents4_MailMergeBeforeMergeEventHandler MailMergeBeforeMerge;
		event ApplicationEvents4_MailMergeBeforeRecordMergeEventHandler MailMergeBeforeRecordMerge;
		event ApplicationEvents4_MailMergeDataSourceLoadEventHandler MailMergeDataSourceLoad;
		event ApplicationEvents4_MailMergeDataSourceValidateEventHandler MailMergeDataSourceValidate;
		event ApplicationEvents4_MailMergeDataSourceValidate2EventHandler MailMergeDataSourceValidate2;
		event ApplicationEvents4_MailMergeWizardSendToCustomEventHandler MailMergeWizardSendToCustom;
		event ApplicationEvents4_MailMergeWizardStateChangeEventHandler MailMergeWizardStateChange;
		event ApplicationEvents4_NewDocumentEventHandler NewDocument;
		event ApplicationEvents4_QuitEventHandler Quit;
		event ApplicationEvents4_StartupEventHandler Startup;
		event ApplicationEvents4_WindowBeforeDoubleClickEventHandler WindowBeforeDoubleClick;
		event ApplicationEvents4_WindowBeforeRightClickEventHandler WindowBeforeRightClick;
		event ApplicationEvents4_WindowSelectionChangeEventHandler WindowSelectionChange;
		event ApplicationEvents4_XMLSelectionChangeEventHandler XMLSelectionChange;
		event ApplicationEvents4_XMLValidationErrorEventHandler XMLValidationError;
	}
	#endregion
	#region WdAlertLevel
	[GeneratedCode("Suppress FxCop check", "")]
	public enum WdAlertLevel {
		wdAlertsAll = -1,
		wdAlertsMessageBox = -2,
		wdAlertsNone = 0
	}
	#endregion
	#region WdKeyCategory
	[GeneratedCode("Suppress FxCop check", "")]
	public enum WdKeyCategory {
		wdKeyCategoryAutoText = 4,
		wdKeyCategoryCommand = 1,
		wdKeyCategoryDisable = 0,
		wdKeyCategoryFont = 3,
		wdKeyCategoryMacro = 2,
		wdKeyCategoryNil = -1,
		wdKeyCategoryPrefix = 7,
		wdKeyCategoryStyle = 5,
		wdKeyCategorySymbol = 6
	}
	#endregion
	#region WdWindowState
	[GeneratedCode("Suppress FxCop check", "")]
	public enum WdWindowState {
		wdWindowStateNormal,
		wdWindowStateMaximize,
		wdWindowStateMinimize
	}
	#endregion
	#region WdEnableCancelKey
	[GeneratedCode("Suppress FxCop check", "")]
	public enum WdEnableCancelKey {
		wdCancelDisabled,
		wdCancelInterrupt
	}
	#endregion
	#region WdKey
	[GeneratedCode("Suppress FxCop check", "")]
	public enum WdKey {
		wdKey0 = 0x30,
		wdKey1 = 0x31,
		wdKey2 = 50,
		wdKey3 = 0x33,
		wdKey4 = 0x34,
		wdKey5 = 0x35,
		wdKey6 = 0x36,
		wdKey7 = 0x37,
		wdKey8 = 0x38,
		wdKey9 = 0x39,
		wdKeyA = 0x41,
		wdKeyAlt = 0x400,
		wdKeyB = 0x42,
		wdKeyBackSingleQuote = 0xc0,
		wdKeyBackSlash = 220,
		wdKeyBackspace = 8,
		wdKeyC = 0x43,
		wdKeyCloseSquareBrace = 0xdd,
		wdKeyComma = 0xbc,
		wdKeyCommand = 0x200,
		wdKeyControl = 0x200,
		wdKeyD = 0x44,
		wdKeyDelete = 0x2e,
		wdKeyE = 0x45,
		wdKeyEnd = 0x23,
		wdKeyEquals = 0xbb,
		wdKeyEsc = 0x1b,
		wdKeyF = 70,
		wdKeyF1 = 0x70,
		wdKeyF10 = 0x79,
		wdKeyF11 = 0x7a,
		wdKeyF12 = 0x7b,
		wdKeyF13 = 0x7c,
		wdKeyF14 = 0x7d,
		wdKeyF15 = 0x7e,
		wdKeyF16 = 0x7f,
		wdKeyF2 = 0x71,
		wdKeyF3 = 0x72,
		wdKeyF4 = 0x73,
		wdKeyF5 = 0x74,
		wdKeyF6 = 0x75,
		wdKeyF7 = 0x76,
		wdKeyF8 = 0x77,
		wdKeyF9 = 120,
		wdKeyG = 0x47,
		wdKeyH = 0x48,
		wdKeyHome = 0x24,
		wdKeyHyphen = 0xbd,
		wdKeyI = 0x49,
		wdKeyInsert = 0x2d,
		wdKeyJ = 0x4a,
		wdKeyK = 0x4b,
		wdKeyL = 0x4c,
		wdKeyM = 0x4d,
		wdKeyN = 0x4e,
		wdKeyNumeric0 = 0x60,
		wdKeyNumeric1 = 0x61,
		wdKeyNumeric2 = 0x62,
		wdKeyNumeric3 = 0x63,
		wdKeyNumeric4 = 100,
		wdKeyNumeric5 = 0x65,
		wdKeyNumeric5Special = 12,
		wdKeyNumeric6 = 0x66,
		wdKeyNumeric7 = 0x67,
		wdKeyNumeric8 = 0x68,
		wdKeyNumeric9 = 0x69,
		wdKeyNumericAdd = 0x6b,
		wdKeyNumericDecimal = 110,
		wdKeyNumericDivide = 0x6f,
		wdKeyNumericMultiply = 0x6a,
		wdKeyNumericSubtract = 0x6d,
		wdKeyO = 0x4f,
		wdKeyOpenSquareBrace = 0xdb,
		wdKeyOption = 0x400,
		wdKeyP = 80,
		wdKeyPageDown = 0x22,
		wdKeyPageUp = 0x21,
		wdKeyPause = 0x13,
		wdKeyPeriod = 190,
		wdKeyQ = 0x51,
		wdKeyR = 0x52,
		wdKeyReturn = 13,
		wdKeyS = 0x53,
		wdKeyScrollLock = 0x91,
		wdKeySemiColon = 0xba,
		wdKeyShift = 0x100,
		wdKeySingleQuote = 0xde,
		wdKeySlash = 0xbf,
		wdKeySpacebar = 0x20,
		wdKeyT = 0x54,
		wdKeyTab = 9,
		wdKeyU = 0x55,
		wdKeyV = 0x56,
		wdKeyW = 0x57,
		wdKeyX = 0x58,
		wdKeyY = 0x59,
		wdKeyZ = 90,
		wdNoKey = 0xff
	}
	#endregion
	#region WdMailSystem
	[GeneratedCode("Suppress FxCop check", "")]
	public enum WdMailSystem {
		wdNoMailSystem,
		wdMAPI,
		wdPowerTalk,
		wdMAPIandPowerTalk
	}
	#endregion
	#region WdOrganizerObject
	[GeneratedCode("Suppress FxCop check", "")]
	public enum WdOrganizerObject {
		wdOrganizerObjectStyles,
		wdOrganizerObjectAutoText,
		wdOrganizerObjectCommandBars,
		wdOrganizerObjectProjectItems
	}
	#endregion
	#region WdCompareDestination
	[GeneratedCode("Suppress FxCop check", "")]
	public enum WdCompareDestination {
		wdCompareDestinationOriginal,
		wdCompareDestinationRevised,
		wdCompareDestinationNew
	}
	#endregion
	#region WdGranularity
	[GeneratedCode("Suppress FxCop check", "")]
	public enum WdGranularity {
		wdGranularityCharLevel,
		wdGranularityWordLevel
	}
	#endregion
	#region WdMergeFormatFrom
	[GeneratedCode("Suppress FxCop check", "")]
	public enum WdMergeFormatFrom {
		wdMergeFormatFromOriginal,
		wdMergeFormatFromRevised,
		wdMergeFormatFromPrompt
	}
	#endregion
	#region WdDocumentMedium
	[GeneratedCode("Suppress FxCop check", "")]
	public enum WdDocumentMedium {
		wdEmailMessage,
		wdDocument,
		wdWebPage
	}
	#endregion
	#region WdInternationalIndex
	[GeneratedCode("Suppress FxCop check", "")]
	public enum WdInternationalIndex {
		wd24HourClock = 0x15,
		wdCurrencyCode = 20,
		wdDateSeparator = 0x19,
		wdDecimalSeparator = 0x12,
		wdInternationalAM = 0x16,
		wdInternationalPM = 0x17,
		wdListSeparator = 0x11,
		wdProductLanguageID = 0x1a,
		wdThousandsSeparator = 0x13,
		wdTimeSeparator = 0x18
	}
	#endregion
}
