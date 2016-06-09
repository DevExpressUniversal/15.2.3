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
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security;
using System.Web.UI;
using DevExpress.Web.ASPxHtmlEditor;
using DevExpress.Web.ASPxHtmlEditor.Internal;
using DevExpress.Web;
using DevExpress.Web.Internal;
using System.Resources;
[assembly: AllowPartiallyTrustedCallers]
[assembly: AssemblyTitle("DevExpress.Web.ASPxHtmlEditor")]
[assembly: AssemblyDescription("")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("Developer Express Inc.")]
[assembly: AssemblyProduct("DevExpress.Web.ASPxHtmlEditor")]
[assembly: AssemblyCopyright("Copyright (c) 2000-2015 Developer Express Inc.")]
[assembly: AssemblyTrademark("ASPxHtmlEditor")]
[assembly: AssemblyCulture("")]
[assembly: NeutralResourcesLanguage("en-US")]
#pragma warning disable 1699
[assembly: AssemblyKeyFile(@"..\..\..\Devexpress.Key\StrongKey.snk")]
#pragma warning restore 1699
[assembly: AssemblyKeyName("")]
[assembly: TagPrefix("DevExpress.Web.ASPxHtmlEditor", "dx")]
[assembly: CLSCompliant(true)]
[assembly: InternalsVisibleTo(AssemblyInfo.SRAssemblyWebDesign + ", PublicKey=0024000004800000940000000602000000240000525341310004000001000100dfcd8cadc2dd24a7cd4ce95c4a9c1b8e7cb1dc2d665120556b4b0ec35495fddb2bd6eed0ca1e56480276295a225ba2a9746f3d3e1a04547ccf5b26acc3f96eb2a13ac467512497aa79208e32f242fd0618014d53c95a36e5de0e891873841fa8f559566e38e968426488b4aa4d0f0b59e59f38dcf3fbccf25d990ab19c27ddc2")]
[assembly: ComVisible(false)]
[assembly: AssemblyVersion(AssemblyInfo.Version)]
[assembly: SatelliteContractVersion(AssemblyInfo.SatelliteContractVersion)]
[assembly: AssemblyFileVersion(AssemblyInfo.FileVersion)]
[assembly: WebResource(ASPxHtmlEditor.HtmlEditorConstantsResourceName, "text/javascript")]
[assembly: WebResource(ASPxHtmlEditor.HtmlEditorDoctypeDescriptionResourceName, "text/javascript")]
[assembly: WebResource(ASPxHtmlEditor.HtmlEditorCommandsResourceName, "text/javascript")]
[assembly: WebResource(ASPxHtmlEditor.HtmlEditorSelectionResourceName, "text/javascript")]
[assembly: WebResource(ASPxHtmlEditor.HtmlEditorSearchScriptResourceName, "text/javascript")]
[assembly: WebResource(ASPxHtmlEditor.HtmlEditorScriptResourceName, "text/javascript")]
[assembly: WebResource(ASPxHtmlEditor.HtmlEditorBaseWrapperResourceName, "text/javascript")]
[assembly: WebResource(ASPxHtmlEditor.HtmlEditorHtmlViewWrapperResourceName, "text/javascript")]
[assembly: WebResource(ASPxHtmlEditor.HtmlEditorHtmlViewMemoWrapperResourceName, "text/javascript")]
[assembly: WebResource(ASPxHtmlEditor.HtmlEditorIFrameWrapperResourceName, "text/javascript")]
[assembly: WebResource(ASPxHtmlEditor.HtmlEditorPreviewIFrameWrapperResourceName, "text/javascript")]
[assembly: WebResource(ASPxHtmlEditor.HtmlEditorDesignViewIFrameWrapperResourceName, "text/javascript")]
[assembly: WebResource(ASPxHtmlEditor.HtmlEditorCoreScriptResourceName, "text/javascript")]
[assembly: WebResource(ASPxHtmlEditor.HtmlEditorHtmlProcessingScriptResourceName, "text/javascript")]
[assembly: WebResource(ASPxHtmlEditor.HtmlEditorBaseDialogsResourceName, "text/javascript")]
[assembly: WebResource(ASPxHtmlEditor.HtmlEditorCommonDialogsResourceName, "text/javascript")]
[assembly: WebResource(ASPxHtmlEditor.HtmlEditorMediaDialogsResourceName, "text/javascript")]
[assembly: WebResource(ASPxHtmlEditor.HtmlEditorTableDialogsResourceName, "text/javascript")]
[assembly: WebResource(ASPxHtmlEditor.HtmlEditorBarDockManagerResourceName, "text/javascript")]
[assembly: WebResource(ASPxHtmlEditor.HtmlEditorKeyboardManagerResourceName, "text/javascript")]
[assembly: WebResource(ASPxHtmlEditor.HtmlEditorStatusBarManagerResourceName, "text/javascript")]
[assembly: WebResource(ASPxHtmlEditor.HtmlEditorTagInspectorResourceName, "text/javascript")]
[assembly: WebResource(ASPxHtmlEditor.HtmlEditorClientStateManagerResourceName, "text/javascript")]
[assembly: WebResource(ASPxHtmlEditor.HtmlEditorLayoutCalculatorResourceName, "text/javascript")]
[assembly: WebResource(ASPxHtmlEditor.HtmlEditorContextMenuManagerResourceName, "text/javascript")]
[assembly: WebResource(ASPxHtmlEditor.HtmlEditorValidationManagerResourceName, "text/javascript")]
[assembly: WebResource(ASPxHtmlEditor.HtmlEditorLayoutManagerResourceName, "text/javascript")]
[assembly: WebResource(ASPxHtmlEditor.HtmlEditorEventManagerScriptResourceName, "text/javascript")]
[assembly: WebResource(ASPxHtmlEditor.HtmlEditorSelectionManagerResourceName, "text/javascript")]
[assembly: WebResource(ASPxHtmlEditor.HtmlEditorPasteOptionsBarManagerResourceName, "text/javascript")]
[assembly: WebResource(ASPxHtmlEditor.HtmlEditorFindManagerResourceName, "text/javascript")]
[assembly: WebResource(ASPxHtmlEditor.HtmlEditorIntelliSenseManagerResourceName, "text/javascript")]
[assembly: WebResource(ASPxHtmlEditor.HtmlEditorInsertPlaceholderCommandResourceName, "text/javascript")]
[assembly: WebResource(ASPxHtmlEditor.HtmlEditorDeleteElementCommandResourceName, "text/javascript")]
[assembly: WebResource(ASPxHtmlEditor.HtmlEditorInsertMediaCommandResourceName, "text/javascript")]
[assembly: WebResource(ASPxHtmlEditor.HtmlEditorChangeElementPropertiesCommandResourceName, "text/javascript")]
[assembly: WebResource(ASPxHtmlEditor.HtmlEditorApplyCssCommandResourceName, "text/javascript")]
[assembly: WebResource(ASPxHtmlEditor.HtmlEditorBgColorCommandResourceName, "text/javascript")]
[assembly: WebResource(ASPxHtmlEditor.HtmlEditorBrowserCommandBaseResourceName, "text/javascript")]
[assembly: WebResource(ASPxHtmlEditor.HtmlEditorChangeImageCommandResourceName, "text/javascript")]
[assembly: WebResource(ASPxHtmlEditor.HtmlEditorCheckSpellingCommandResourceName, "text/javascript")]
[assembly: WebResource(ASPxHtmlEditor.HtmlEditorCheckSpellingCoreCommandResourceName, "text/javascript")]
[assembly: WebResource(ASPxHtmlEditor.HtmlEditorClipboardCommandResourceName, "text/javascript")]
[assembly: WebResource(ASPxHtmlEditor.HtmlEditorCommandArgumentsResourceName, "text/javascript")]
[assembly: WebResource(ASPxHtmlEditor.HtmlEditorCommandBaseResourceName, "text/javascript")]
[assembly: WebResource(ASPxHtmlEditor.HtmlEditorDeleteCommandResourceName, "text/javascript")]
[assembly: WebResource(ASPxHtmlEditor.HtmlEditorDeleteWithoutSelectionCommandResourceName, "text/javascript")]
[assembly: WebResource(ASPxHtmlEditor.HtmlEditorEnterCommandResourceName, "text/javascript")]
[assembly: WebResource(ASPxHtmlEditor.HtmlEditorFontColorCommandResourceName, "text/javascript")]
[assembly: WebResource(ASPxHtmlEditor.HtmlEditorFontNameCommandResourceName, "text/javascript")]
[assembly: WebResource(ASPxHtmlEditor.HtmlEditorFontSizeCommandResourceName, "text/javascript")]
[assembly: WebResource(ASPxHtmlEditor.HtmlEditorFontStyleCommandResourceName, "text/javascript")]
[assembly: WebResource(ASPxHtmlEditor.HtmlEditorFormatBlockCommandResourceName, "text/javascript")]
[assembly: WebResource(ASPxHtmlEditor.HtmlEditorFullscreenCommandResourceName, "text/javascript")]
[assembly: WebResource(ASPxHtmlEditor.HtmlEditorIndentCommandResourceName, "text/javascript")]
[assembly: WebResource(ASPxHtmlEditor.HtmlEditorInsertImageCommandResourceName, "text/javascript")]
[assembly: WebResource(ASPxHtmlEditor.HtmlEditorInsertLinkCommandResourceName, "text/javascript")]
[assembly: WebResource(ASPxHtmlEditor.HtmlEditorInsertListCommandResourceName, "text/javascript")]
[assembly: WebResource(ASPxHtmlEditor.HtmlEditorJustifyCommandResourceName, "text/javascript")]
[assembly: WebResource(ASPxHtmlEditor.HtmlEditorKbCopyCommandResourceName, "text/javascript")]
[assembly: WebResource(ASPxHtmlEditor.HtmlEditorKbCutCommandResourceName, "text/javascript")]
[assembly: WebResource(ASPxHtmlEditor.HtmlEditorKbPasteCommandResourceName, "text/javascript")]
[assembly: WebResource(ASPxHtmlEditor.HtmlEditorLineBreakTypeCommandResourceName, "text/javascript")]
[assembly: WebResource(ASPxHtmlEditor.HtmlEditorNewParagraphTypeCommandResourceName, "text/javascript")]
[assembly: WebResource(ASPxHtmlEditor.HtmlEditorPasteFromWordCommandResourceName, "text/javascript")]
[assembly: WebResource(ASPxHtmlEditor.HtmlEditorPasteHtmlCommandResourceName, "text/javascript")]
[assembly: WebResource(ASPxHtmlEditor.HtmlEditorPasteOptionsCommandResourceName, "text/javascript")]
[assembly: WebResource(ASPxHtmlEditor.HtmlEditorPrintCommandResourceName, "text/javascript")]
[assembly: WebResource(ASPxHtmlEditor.HtmlEditorRedoCommandResourceName, "text/javascript")]
[assembly: WebResource(ASPxHtmlEditor.HtmlEditorRemoveFormatCommandResourceName, "text/javascript")]
[assembly: WebResource(ASPxHtmlEditor.HtmlEditorSaveAsCommandResourceName, "text/javascript")]
[assembly: WebResource(ASPxHtmlEditor.HtmlEditorSelectAllCommandResourceName, "text/javascript")]
[assembly: WebResource(ASPxHtmlEditor.HtmlEditorTextTypeCommandResourceName, "text/javascript")]
[assembly: WebResource(ASPxHtmlEditor.HtmlEditorUndoCommandResourceName, "text/javascript")]
[assembly: WebResource(ASPxHtmlEditor.HtmlEditorUnlinkCommandResourceName, "text/javascript")]
[assembly: WebResource(ASPxHtmlEditor.HtmlEditorWrappedCommandBaseResourceName, "text/javascript")]
[assembly: WebResource(ASPxHtmlEditor.HtmlEditorDialogCommandResourceName, "text/javascript")]
[assembly: WebResource(ASPxHtmlEditor.HtmlEditorTableCellCommandResourceName, "text/javascript")]
[assembly: WebResource(ASPxHtmlEditor.HtmlEditorTableColumnCommandResourceName, "text/javascript")]
[assembly: WebResource(ASPxHtmlEditor.HtmlEditorTableCommandResourceName, "text/javascript")]
[assembly: WebResource(ASPxHtmlEditor.HtmlEditorTableRowCommandResourceName, "text/javascript")]
[assembly: WebResource(ASPxHtmlEditor.HtmlEditorQuickSearchCommandResourceName, "text/javascript")]
[assembly: WebResource(ASPxHtmlEditor.HtmlEditorFindAndReplaceDialogCommandResourceName, "text/javascript")]
[assembly: WebResource(ASPxHtmlEditor.HtmlEditorCommentHtmlCommandResourceName, "text/javascript")]
[assembly: WebResource(ASPxHtmlEditor.HtmlEditorFormatDocumentCommandResourceName, "text/javascript")]
[assembly: WebResource(ASPxHtmlEditor.HtmlEditorIndentLineCommandResourceName, "text/javascript")]
[assembly: WebResource(ASPxHtmlEditor.HtmlEditorCollapseTagCommandResourceName, "text/javascript")]
[assembly: WebResource(ASPxHtmlEditor.HtmlEditorIntelliSenseCommandsResourceName, "text/javascript")]
[assembly: WebResource(ASPxHtmlEditor.HtmlEditorContextTabStateCommandResourceName, "text/javascript")]
[assembly: WebResource(HtmlEditorBarDockControl.BarDockScriptResourceName, "text/javascript")]
[assembly: WebResource(ToolbarControl.ToolbarScriptResourceName, "text/javascript")]
[assembly: WebResource(ToolbarComboBoxControl.ToolbarComboBoxScriptResourceName, "text/javascript")]
[assembly: WebResource(ToolbarCustomDropDownItemPicker.CustomDropDownItemPickerScriptResourceName, "text/javascript")]
[assembly: WebResource(ASPxHtmlEditor.HtmlEditorCodeMirrorResourceName, "text/javascript")]
[assembly: WebResource(ASPxHtmlEditor.HtmlEditorCodeMirrorJavaScriptModeResourceName, "text/javascript")]
[assembly: WebResource(ASPxHtmlEditor.HtmlEditorCodeMirrorCssModeResourceName, "text/javascript")]
[assembly: WebResource(ASPxHtmlEditor.HtmlEditorCodeMirrorXmlModeResourceName, "text/javascript")]
[assembly: WebResource(ASPxHtmlEditor.HtmlEditorCodeMirrorHtmlMixedModeResourceName, "text/javascript")]
[assembly: WebResource(ASPxHtmlEditor.HtmlEditorCodeMirrorActiveLineAddonResourceName, "text/javascript")]
[assembly: WebResource(ASPxHtmlEditor.HtmlEditorCodeMirrorCloseTagAddonResourceName, "text/javascript")]
[assembly: WebResource(ASPxHtmlEditor.HtmlEditorCodeMirrorCommentAddonResourceName, "text/javascript")]
[assembly: WebResource(ASPxHtmlEditor.HtmlEditorCodeMirrorXmlFoldAddonResourceName, "text/javascript")]
[assembly: WebResource(ASPxHtmlEditor.HtmlEditorCodeMirrorMatchTagsAddonResourceName, "text/javascript")]
[assembly: WebResource(ASPxHtmlEditor.HtmlEditorCodeMirrorCommentFoldAddonResourceName, "text/javascript")]
[assembly: WebResource(ASPxHtmlEditor.HtmlEditorCodeMirrorFoldCodeAddonResourceName, "text/javascript")]
[assembly: WebResource(ASPxHtmlEditor.HtmlEditorCodeMirrorFoldGutterAddonResourceName, "text/javascript")]
[assembly: WebResource(ASPxHtmlEditor.HtmlEditorCodeMirrorCssResourceName, "text/css", PerformSubstitution = true)]
[assembly: WebResource(ASPxHtmlEditor.HtmlEditorFoldGutterCssResourceName, "text/css", PerformSubstitution = true)]
[assembly: WebResource(ASPxHtmlEditor.HtmlEditorBeautifyHtmlResourceName, "text/javascript")]
[assembly: WebResource(HtmlEditorFileManager.ScriptName, "text/javascript")]
[assembly: WebResource(HtmlEditorSpellChecker.ScriptName, "text/javascript")]
[assembly: WebResource(ASPxHtmlEditor.HtmlEditorDefaultCssResourceName, "text/css", PerformSubstitution = true)]
[assembly: WebResource(ASPxHtmlEditor.HtmlEditorSystemDesignViewCssResourceName, "text/css", PerformSubstitution = true)]
[assembly: WebResource(ASPxHtmlEditor.HtmlEditorSpriteCssResourceName, "text/css", PerformSubstitution = true)]
[assembly: WebResource(ASPxHtmlEditor.HtmlEditorSystemCssResourceName, "text/css", PerformSubstitution = true)]
[assembly: WebResource(ASPxHtmlEditor.HtmlEditorToolbarSpriteCssResourceName, "text/css", PerformSubstitution = true)]
[assembly: WebResource(ASPxHtmlEditor.HtmlEditorToolbarWhiteSpriteCssResourceName, "text/css", PerformSubstitution = true)]
[assembly: WebResource(ASPxHtmlEditor.HtmlEditorToolbarGrayScaleSpriteCssResourceName, "text/css", PerformSubstitution = true)]
[assembly: WebResource(ASPxHtmlEditor.HtmlEditorToolbarGrayScaleWithWhiteHottrackSpriteCssResourceName, "text/css", PerformSubstitution = true)]
[assembly: WebResource(ASPxHtmlEditor.HtmlEditorSpriteImageResourceName, "image/png")]
[assembly: WebResource(ASPxHtmlEditor.HtmlEditorIconSpriteImageResourceName, "image/png")]
[assembly: WebResource(ASPxHtmlEditor.HtmlEditorWhiteIconSpriteImageResourceName, "image/png")]
[assembly: WebResource(ASPxHtmlEditor.HtmlEditorGrayScaleIconSpriteImageResourceName, "image/png")]
[assembly: WebResource(ASPxHtmlEditor.HtmlEditorGrayScaleWithWhiteHottrackIconSpriteImageResourceName, "image/png")]
[assembly: WebResource(ASPxHtmlEditor.HtmlEditorImagesResourcePath + HtmlEditorImages.LoadingPanelImageName + ".gif", "image/gif")]
[assembly: WebResource(ASPxHtmlEditor.HtmlEditorAudioImageResourceName, "image/png")]
[assembly: WebResource(ASPxHtmlEditor.HtmlEditorFlashImageResourceName, "image/png")]
[assembly: WebResource(ASPxHtmlEditor.HtmlEditorVideoImageResourceName, "image/png")]
[assembly: WebResource(ASPxHtmlEditor.HtmlEditorYouTubeImageResourceName, "image/png")]
[assembly: WebResource(ASPxHtmlEditor.HtmlEditorNotSupportedImageResourceName, "image/png")]
[assembly: WebResource(ASPxHtmlEditor.HtmlEditorFoldGutterFoldedImageResourceName, "image/png")]
[assembly: WebResource(ASPxHtmlEditor.HtmlEditorFoldGutterOpenImageResourceName, "image/png")]
[assembly: WebResource(ASPxHtmlEditor.HtmlEditorFoldMarkerImageResourceName, "image/png")]
[assembly: WebResource("DevExpress.Web.ASPxHtmlEditor.Tests.Scripts._reference.js", "text/javascript")]
[assembly: WebResource("DevExpress.Web.ASPxHtmlEditor.Tests.Scripts.TestUtils.js", "text/javascript")]
[assembly: WebResource("DevExpress.Web.ASPxHtmlEditor.Tests.Scripts.CommandTestsOnJasmine.js", "text/javascript")]
[assembly: WebResource("DevExpress.Web.ASPxHtmlEditor.Tests.Scripts.MediaObjectTests.js", "text/javascript")]
[assembly: WebResource("DevExpress.Web.ASPxHtmlEditor.Tests.Scripts.DialogTests.js", "text/javascript")]
[assembly: WebResource("DevExpress.Web.ASPxHtmlEditor.Tests.Scripts.TagInspectorTests.js", "text/javascript")]
[assembly: WebResource("DevExpress.Web.ASPxHtmlEditor.Tests.Scripts.ClientHtmlProcessingTestsOnJasmine.js", "text/javascript")]
[assembly: WebResource("DevExpress.Web.ASPxHtmlEditor.Tests.Scripts.SearchTests.js", "text/javascript")]
[assembly: WebResource("DevExpress.Web.ASPxHtmlEditor.Tests.Scripts.HtmlViewCommandTests.js", "text/javascript")]
