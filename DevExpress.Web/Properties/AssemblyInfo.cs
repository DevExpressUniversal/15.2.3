﻿#region Copyright (c) 2000-2015 Developer Express Inc.
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
using System.Resources;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security;
#if ASP
using System.Web.UI;
using DevExpress.Web;
using DevExpress.Web.Internal;
using DevExpress.Web.Office.Internal;
#endif
#if ASP
[assembly: AllowPartiallyTrustedCallers]
#endif
[assembly: AssemblyTitle("DevExpress.Web")]
[assembly: AssemblyDescription("DevExpress.Web")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("Developer Express Inc.")]
[assembly: AssemblyProduct("DevExpress.Web")]
[assembly: AssemblyCopyright("Copyright (c) 2000-2015 Developer Express Inc.")]
[assembly: AssemblyTrademark("DevExpress.Web")]
[assembly: AssemblyCulture("")]
[assembly: NeutralResourcesLanguage("en-US")]
[assembly: CLSCompliant(true)]
[assembly: InternalsVisibleTo(AssemblyInfo.SRAssemblyWebDesign + ", PublicKey=0024000004800000940000000602000000240000525341310004000001000100dfcd8cadc2dd24a7cd4ce95c4a9c1b8e7cb1dc2d665120556b4b0ec35495fddb2bd6eed0ca1e56480276295a225ba2a9746f3d3e1a04547ccf5b26acc3f96eb2a13ac467512497aa79208e32f242fd0618014d53c95a36e5de0e891873841fa8f559566e38e968426488b4aa4d0f0b59e59f38dcf3fbccf25d990ab19c27ddc2")]
[assembly: InternalsVisibleTo(AssemblyInfo.SRAssemblyMVC + ", PublicKey=0024000004800000940000000602000000240000525341310004000001000100dfcd8cadc2dd24a7cd4ce95c4a9c1b8e7cb1dc2d665120556b4b0ec35495fddb2bd6eed0ca1e56480276295a225ba2a9746f3d3e1a04547ccf5b26acc3f96eb2a13ac467512497aa79208e32f242fd0618014d53c95a36e5de0e891873841fa8f559566e38e968426488b4aa4d0f0b59e59f38dcf3fbccf25d990ab19c27ddc2")]
[assembly: InternalsVisibleTo(AssemblyInfo.SRAssemblyMVC5 + ", PublicKey=0024000004800000940000000602000000240000525341310004000001000100dfcd8cadc2dd24a7cd4ce95c4a9c1b8e7cb1dc2d665120556b4b0ec35495fddb2bd6eed0ca1e56480276295a225ba2a9746f3d3e1a04547ccf5b26acc3f96eb2a13ac467512497aa79208e32f242fd0618014d53c95a36e5de0e891873841fa8f559566e38e968426488b4aa4d0f0b59e59f38dcf3fbccf25d990ab19c27ddc2")]
[assembly: InternalsVisibleTo(AssemblyInfo.SRAssemblyTreeListWeb + ", PublicKey=0024000004800000940000000602000000240000525341310004000001000100dfcd8cadc2dd24a7cd4ce95c4a9c1b8e7cb1dc2d665120556b4b0ec35495fddb2bd6eed0ca1e56480276295a225ba2a9746f3d3e1a04547ccf5b26acc3f96eb2a13ac467512497aa79208e32f242fd0618014d53c95a36e5de0e891873841fa8f559566e38e968426488b4aa4d0f0b59e59f38dcf3fbccf25d990ab19c27ddc2")]
[assembly: ComVisible(false)]
[assembly: Guid("5be632c3-7b8a-4a04-8bb9-64b2ac435bda")]
[assembly: AssemblyVersion(AssemblyInfo.Version)]
[assembly: SatelliteContractVersion(AssemblyInfo.SatelliteContractVersion)]
[assembly: AssemblyFileVersion(AssemblyInfo.FileVersion)]
#pragma warning disable 1699
#if ASP
[assembly: AssemblyKeyFile(@"..\..\..\Devexpress.Key\StrongKey.snk")]
#endif
#pragma warning restore 1699
[assembly: AssemblyKeyName("")]
#if ASP
[assembly: TagPrefix("DevExpress.Web", "dx")]
[assembly: TagPrefix("DevExpress.Data.Linq", "dx")]
[assembly: WebResource(RenderUtils.GlobalizeScriptResourceName, "text/javascript")]
[assembly: WebResource(RenderUtils.GlobalizeCulturesScriptResourceName, "text/javascript")]
[assembly: WebResource(RenderUtils.JQueryScriptResourceName, "text/javascript")]
[assembly: WebResource(RenderUtils.JQueryUIScriptResourceName, "text/javascript")]
[assembly: WebResource(RenderUtils.JQueryValidateScriptResourceName, "text/javascript")]
[assembly: WebResource(RenderUtils.JQueryUnobtrusiveScriptResourceName, "text/javascript")]
[assembly: WebResource(RenderUtils.JQueryUnobtrusiveAjaxScriptResourceName, "text/javascript")]
[assembly: WebResource(RenderUtils.KnockoutScriptResourceName, "text/javascript")]
[assembly: WebResource(RenderUtils.JQueryUICssResourceName, "text/css")]
[assembly: WebResource(ASPxCallback.CallbackScriptResourceName, "text/javascript")]
[assembly: WebResource(ASPxCallbackPanel.CallbackPanelScriptResourceName, "text/javascript")]
[assembly: WebResource(ASPxWebControl.DebugScriptResourceName, "text/javascript")]
[assembly: WebResource(ASPxWebControl.UtilsScriptResourceName, "text/javascript", PerformSubstitution = true)]
[assembly: WebResource(ASPxWebControl.ClassesScriptResourceName, "text/javascript", PerformSubstitution = true)]
[assembly: WebResource(ASPxWebControl.AnimationScriptResourceName, "text/javascript")]
[assembly: WebResource(ASPxWebControl.MobileScriptResourceName, "text/javascript")]
[assembly: WebResource(ASPxWebControl.PopupUtilsScriptResourceName, "text/javascript")]
[assembly: WebResource(ASPxWebControl.DialogUtilsScriptResourceName, "text/javascript")]
[assembly: WebResource(ASPxWebControl.DragAndDropUtilsScriptResourceName, "text/javascript")]
[assembly: WebResource(ASPxWebControl.RelatedControlManagerScriptResourceName, "text/javascript")]
[assembly: WebResource(ASPxWebControl.DateFormatterScriptResourceName, "text/javascript")]
[assembly: WebResource(ASPxWebControl.FormatterScriptResourceName, "text/javascript")]
[assembly: WebResource(ASPxWebControl.StateControllerScriptResourceName, "text/javascript")]
[assembly: WebResource(ASPxWebControl.ScrollUtilsScriptResourceName, "text/javascript")]
[assembly: WebResource(ASPxWebControl.ControlResizeManagerScriptResourceName, "text/javascript")]
[assembly: WebResource(ASPxWebControl.TableScrollUtilsScriptResourceName, "text/javascript")]
[assembly: WebResource(ASPxCloudControl.CloudControlScriptResourceName, "text/javascript")]
[assembly: WebResource(ASPxDataViewBase.DataViewScriptResourceName, "text/javascript")]
[assembly: WebResource(ASPxHiddenField.HiddenFieldScriptResourceName, "text/javascript")]
[assembly: WebResource(ASPxMenuBase.MenuScriptResourceName, "text/javascript")]
[assembly: WebResource(ASPxPopupMenu.PopupMenuScriptResourceName, "text/javascript")]
[assembly: WebResource(ASPxNavBar.NavBarScriptResourceName, "text/javascript")]
[assembly: WebResource(ASPxNewsControl.NewsControlScriptResourceName, "text/javascript")]
[assembly: WebResource(ASPxObjectContainer.ObjectContainerScriptResourceName, "text/javascript")]
[assembly: WebResource(ASPxRoundPanel.RoundPanelScriptResourceName, "text/javascript")]
[assembly: WebResource(ASPxPagerBase.PagerBaseScriptResourceName, "text/javascript")]
[assembly: WebResource(ASPxProgressBarBase.ProgressBarBaseScriptResourceName, "text/javascript")]
[assembly: WebResource(ASPxPopupControl.PopupControlScriptResourceName, "text/javascript")]
[assembly: WebResource(ASPxGlobalEvents.GlobalEventsScriptResourceName, "text/javascript")]
[assembly: WebResource(ASPxTabControlBase.TabControlScriptResourceName, "text/javascript")]
[assembly: WebResource(ASPxTimer.TimerScriptResourceName, "text/javascript")]
[assembly: WebResource(ASPxTitleIndex.TitleIndexScriptResourceName, "text/javascript")]
[assembly: WebResource(ASPxTreeView.ScriptResourceName, "text/javascript")]
[assembly: WebResource(ASPxUploadControl.UploadScriptsResourcePath, "text/javascript")]
[assembly: WebResource(ASPxPanel.PanelScriptResourceName, "text/javascript")]
[assembly: WebResource(ASPxLoadingPanel.LoadingPanelScriptResourceName, "text/javascript")]
[assembly: WebResource(ASPxRatingControl.ResourceScriptPath, "text/javascript")]
[assembly: WebResource(ASPxSplitter.ScriptName, "text/javascript")]
[assembly: WebResource(ASPxFileManager.ScriptName, "text/javascript")]
[assembly: WebResource(ASPxDockPanel.ScriptResourceName, "text/javascript")]
[assembly: WebResource(ASPxDockZone.ScriptResourceName, "text/javascript")]
[assembly: WebResource(ASPxDockManager.ScriptResourceName, "text/javascript")]
[assembly: WebResource(ASPxImageSlider.ScriptResourceName, "text/javascript")]
[assembly: WebResource(ASPxImageGallery.ScriptResourceName, "text/javascript")]
[assembly: WebResource(ASPxImageZoom.ScriptResourceName, "text/javascript")]
[assembly: WebResource(ASPxWebControl.ImageControlUtilsScriptResourceName, "text/javascript")]
[assembly: WebResource(ASPxFormLayout.ScriptResourceName, "text/javascript")]
[assembly: WebResource(InternalClock.InternalClockScriptResourceName, "text/javascript")]
[assembly: WebResource(ASPxRibbon.RibbonScriptResourceName, "text/javascript")]
[assembly: WebResource(ASPxImage.ImageScriptResourceName, "text/javascript")]
[assembly: WebResource(OfficeControl.OfficeControlsScriptResourceName, "text/javascript")]
[assembly: WebResource(RenderUtils.DevExtremeCoreScriptResourceName, "text/javascript")]
[assembly: WebResource(RenderUtils.DevExtremeWidgetsBaseScriptResourceName, "text/javascript")]
[assembly: WebResource(RenderUtils.DevExtremeWidgetsWebScriptResourceName, "text/javascript")]
[assembly: WebResource(RenderUtils.DevExtremeVizCoreScriptResourceName, "text/javascript")]
[assembly: WebResource(RenderUtils.DevExtremeVizChartsScriptResourceName, "text/javascript")]
[assembly: WebResource(RenderUtils.DevExtremeVizGaugesScriptResourceName, "text/javascript")]
[assembly: WebResource(RenderUtils.DevExtremeVizRangeSelectorScriptResourceName, "text/javascript")]
[assembly: WebResource(RenderUtils.DevExtremeVizSparklinesScriptResourceName, "text/javascript")]
[assembly: WebResource(RenderUtils.DevExtremeVizVectorMapScriptResourceName, "text/javascript")]
[assembly: WebResource(RenderUtils.DevExtremeCommonCssResourceName, "text/css")]
[assembly: WebResource(RenderUtils.DevExtremeLightCssResourceName, "text/css")]
[assembly: WebResource(RenderUtils.DevExtremeDarkCssResourceName, "text/css")]
[assembly: WebResource(RenderUtils.DevExtremeLightCompactCssResourceName, "text/css")]
[assembly: WebResource(RenderUtils.DevExtremeDarkCompactCssResourceName, "text/css")]
[assembly: WebResource(RenderUtils.DevExtremeCommonOverridesCssResourceName, "text/css", PerformSubstitution = true)]
[assembly: WebResource(RenderUtils.DevExtremeCssResourcePath + "icons.dxicons.eot", "application/vnd.ms-fontobject")]
[assembly: WebResource(RenderUtils.DevExtremeCssResourcePath + "icons.dxicons.woff", "application/font-woff")]
[assembly: WebResource(RenderUtils.DevExtremeCssResourcePath + "icons.dxicons.ttf", " application/font-sfnt")]
[assembly: WebResource(ASPxWebControl.WebSystemCssResourceName, "text/css", PerformSubstitution = true)]
[assembly: WebResource(ASPxWebControl.WebSystemHtml5CssResourceName, "text/css", PerformSubstitution = true)]
[assembly: WebResource(ASPxWebControl.WebSystemDesignModeCssResourceName, "text/css", PerformSubstitution = true)]
[assembly: WebResource(ASPxWebControl.WebClientUIControlSystemDesignModeCssResourceName, "text/css", PerformSubstitution = true)]
[assembly: WebResource(ASPxWebControl.WebDefaultCssResourceName, "text/css", PerformSubstitution = true)]
[assembly: WebResource(ASPxWebControl.WebSpriteCssResourceName, "text/css", PerformSubstitution = true)]
[assembly: WebResource(ASPxWebControl.WebCssResourcePath + HERibbonImages.RibbonHESpriteName + ".css", "text/css", PerformSubstitution = true)]
[assembly: WebResource(ASPxWebControl.WebCssResourcePath + HERibbonImages.RibbonHEGSpriteName + ".css", "text/css", PerformSubstitution = true)]
[assembly: WebResource(ASPxWebControl.WebCssResourcePath + HERibbonImages.RibbonHEGWSpriteName + ".css", "text/css", PerformSubstitution = true)]
[assembly: WebResource(ASPxWebControl.WebCssResourcePath + DocumentViewerRibbonImages.RibbonDVSpriteName + ".css", "text/css", PerformSubstitution = true)]
[assembly: WebResource(ASPxWebControl.WebCssResourcePath + DocumentViewerRibbonImages.RibbonDVGSpriteName + ".css", "text/css", PerformSubstitution = true)]
[assembly: WebResource(ASPxWebControl.WebCssResourcePath + DocumentViewerRibbonImages.RibbonDVGWSpriteName + ".css", "text/css", PerformSubstitution = true)]
[assembly: WebResource(ASPxWebControl.WebCssResourcePath + SpreadsheetRibbonImages.RibbonSSSpriteName + ".css", "text/css", PerformSubstitution = true)]
[assembly: WebResource(ASPxWebControl.WebCssResourcePath + SpreadsheetRibbonImages.RibbonSSGSpriteName + ".css", "text/css", PerformSubstitution = true)]
[assembly: WebResource(ASPxWebControl.WebCssResourcePath + SpreadsheetRibbonImages.RibbonSSGWSpriteName + ".css", "text/css", PerformSubstitution = true)]
[assembly: WebResource(ASPxWebControl.WebCssResourcePath + RichEditRibbonImages.RibbonRESpriteName + ".css", "text/css", PerformSubstitution = true)]
[assembly: WebResource(ASPxWebControl.WebCssResourcePath + RichEditRibbonImages.RibbonREGSpriteName + ".css", "text/css", PerformSubstitution = true)]
[assembly: WebResource(ASPxWebControl.WebCssResourcePath + RichEditRibbonImages.RibbonREGWSpriteName + ".css", "text/css", PerformSubstitution = true)]
[assembly: WebResource(ASPxWebControl.WebCssResourcePath + GridImages.FormatConditionIconSetSpriteName + ".css", "text/css", PerformSubstitution = true)]
[assembly: WebResource(ASPxWebControl.SSLSecureBlankUrlResourceName, "text/html")]
[assembly: WebResource(EmptyImageProperties.EmptyImageResourceName, "image/gif")]
[assembly: WebResource(ASPxWebControl.TrialMessageCloseImageName, "image/png")]
[assembly: WebResource(ASPxEditBase.EditImagesResourcePath + FilterControlImages.LineImageName + ".gif", "image/gif")]
[assembly: WebResource(ASPxEditBase.EditImagesResourcePath + FilterControlImages.ElbowImageName + ".gif", "image/gif")]
[assembly: WebResource(ASPxWebControl.WebImagesResourcePath + ImagesBase.SpriteImageName + ".png", "image/png")]
[assembly: WebResource(ASPxWebControl.WebImagesResourcePath + ImagesBase.LoadingPanelImageName + ".gif", "image/gif")]
[assembly: WebResource(ASPxWebControl.WebImagesResourcePath + ImageSliderImagesBase.DesignTimeSpriteImageName + ".png", "image/png")]
[assembly: WebResource(ASPxWebControl.WebImagesResourcePath + ImageSliderImagesBase.LoadingImageName + ".gif", "image/gif")]
[assembly: WebResource(ASPxWebControl.WebImagesResourcePath + ImageGalleryImages.LoadingImageName + ".gif", "image/gif")]
[assembly: WebResource(ASPxWebControl.WebImagesResourcePath + ImageGalleryImages.NavigationButtonsBack + ".png", "image/png")]
[assembly: WebResource(ASPxWebControl.WebImagesResourcePath + ImageControlsImagesBase.DesignTimeItemImageName + ".png", "image/png")]
[assembly: WebResource(ASPxWebControl.WebImagesResourcePath + MenuImages.GutterImageName + ".gif", "image/gif")]
[assembly: WebResource(ASPxWebControl.WebImagesResourcePath + ObjectContainerImages.ErrorImageName + ".gif", "image/gif")]
[assembly: WebResource(ASPxWebControl.WebImagesResourcePath + ObjectContainerImages.ImageImageName + ".gif", "image/gif")]
[assembly: WebResource(ASPxWebControl.WebImagesResourcePath + ObjectContainerImages.FlashImageName + ".gif", "image/gif")]
[assembly: WebResource(ASPxWebControl.WebImagesResourcePath + ObjectContainerImages.VideoImageName + ".gif", "image/gif")]
[assembly: WebResource(ASPxWebControl.WebImagesResourcePath + ObjectContainerImages.AudioImageName + ".gif", "image/gif")]
[assembly: WebResource(ASPxWebControl.WebImagesResourcePath + ObjectContainerImages.QuickTimeImageName + ".gif", "image/gif")]
[assembly: WebResource(ASPxWebControl.WebImagesResourcePath + ObjectContainerImages.Html5VideoImageName + ".gif", "image/gif")]
[assembly: WebResource(ASPxWebControl.WebImagesResourcePath + ObjectContainerImages.Html5AudioImageName + ".gif", "image/gif")]
[assembly: WebResource(ASPxWebControl.WebImagesResourcePath + PopupControlImages.ModalBackgroundImageName + ".gif", "image/gif")]
[assembly: WebResource(ASPxWebControl.WebImagesResourcePath + PagerImages.DropDownButtonBackImageName + ".gif", "image/gif")]
[assembly: WebResource(ASPxWebControl.WebImagesResourcePath + PagerImages.DropDownButtonHoverBackImageName + ".gif", "image/gif")]
[assembly: WebResource(ASPxWebControl.WebImagesResourcePath + ASPxRatingControl.ImageMapResourceName + ".png", "image/png")]
[assembly: WebResource(ASPxWebControl.WebImagesResourcePath + ASPxRatingControl.ImageMapResourceName + ".gif", "image/gif")]
[assembly: WebResource(ASPxWebControl.WebImagesResourcePath + SplitterImages.ResizingPointerBackImageName + ".gif", "image/gif")]
[assembly: WebResource(ASPxWebControl.WebImagesResourcePath + TreeViewImages.LineImageName + ".gif", "image/gif")]
[assembly: WebResource(ASPxWebControl.WebImagesResourcePath + TreeViewImages.ElbowImageName + ".gif", "image/gif")]
[assembly: WebResource(ASPxWebControl.WebImagesResourcePath + TreeViewImages.ElbowImageName + TreeViewImages.RtlImagePostfix + ".gif", "image/gif")]
[assembly: WebResource(ASPxWebControl.WebImagesResourcePath + TreeViewImages.NodeLoadingPanelImageName + ".gif", "image/gif")]
[assembly: WebResource(ASPxWebControl.WebImagesResourcePath + TabControlImages.ScrollButtonBackgroundImageName + ".gif", "image/gif")]
[assembly: WebResource(ASPxWebControl.WebImagesResourcePath + TabControlImages.ScrollButtonHoverBackgroundImageName + ".gif", "image/gif")]
[assembly: WebResource(ASPxWebControl.WebImagesResourcePath + TabControlImages.ScrollButtonDisableBackgroundImageName + ".gif", "image/gif")]
[assembly: WebResource(ASPxWebControl.WebImagesResourcePath + FileManagerImages.SplitterSeparatorImageName + ".gif", "image/gif")]
[assembly: WebResource(ASPxWebControl.WebImagesResourcePath + FileManagerImages.FileImageName + ".png", "image/png")]
[assembly: WebResource(ASPxWebControl.WebImagesResourcePath + FileManagerImages.PdfFileImageName + ".png", "image/png")]
[assembly: WebResource(ASPxWebControl.WebImagesResourcePath + FileManagerImages.PlainTextFileImageName + ".png", "image/png")]
[assembly: WebResource(ASPxWebControl.WebImagesResourcePath + FileManagerImages.PresentationFileImageName + ".png", "image/png")]
[assembly: WebResource(ASPxWebControl.WebImagesResourcePath + FileManagerImages.RichTextFileImageName + ".png", "image/png")]
[assembly: WebResource(ASPxWebControl.WebImagesResourcePath + FileManagerImages.SpreadsheetFileImageName + ".png", "image/png")]
[assembly: WebResource(ASPxWebControl.WebImagesResourcePath + FileManagerImages.FolderBigImageName + ".png", "image/png")]
[assembly: WebResource(ASPxWebControl.WebImagesResourcePath + FileManagerImages.FolderLockedBigImageName + ".png", "image/png")]
[assembly: WebResource(ASPxWebControl.WebImagesResourcePath + FileManagerImages.FolderUpImageName + ".png", "image/png")]
[assembly: WebResource(ASPxWebControl.WebImagesResourcePath + ASPxUploadControl.ButtonBackImageName + ".gif", "image/gif")]
[assembly: WebResource(ASPxWebControl.WebImagesResourcePath + ASPxUploadControl.ButtonHoverBackImageName + ".gif", "image/gif")]
[assembly: WebResource(ASPxUploadControl.SLUploadHelperName, "application/x-silverlight-2")]
[assembly: WebResource(ASPxWebControl.WebImagesResourcePath + RibbonImages.FileTabBackgroundImageName + ".png", "image/png")]
[assembly: WebResource(ASPxWebControl.WebImagesResourcePath + RibbonImages.FileTabHoverBackgroundImageName + ".png", "image/png")]
[assembly: WebResource(ASPxWebControl.WebImagesResourcePath + HERibbonImages.RibbonHESpriteName + ".png", "image/png")]
[assembly: WebResource(ASPxWebControl.WebImagesResourcePath + HERibbonImages.RibbonHEGSpriteName + ".png", "image/png")]
[assembly: WebResource(ASPxWebControl.WebImagesResourcePath + HERibbonImages.RibbonHEGWSpriteName + ".png", "image/png")]
[assembly: WebResource(ASPxWebControl.WebImagesResourcePath + DocumentViewerRibbonImages.RibbonDVSpriteName + ".png", "image/png")]
[assembly: WebResource(ASPxWebControl.WebImagesResourcePath + DocumentViewerRibbonImages.RibbonDVGSpriteName + ".png", "image/png")]
[assembly: WebResource(ASPxWebControl.WebImagesResourcePath + DocumentViewerRibbonImages.RibbonDVGWSpriteName + ".png", "image/png")]
[assembly: WebResource(ASPxWebControl.WebImagesResourcePath + SpreadsheetRibbonImages.RibbonSSSpriteName + ".png", "image/png")]
[assembly: WebResource(ASPxWebControl.WebImagesResourcePath + SpreadsheetRibbonImages.RibbonSSGSpriteName + ".png", "image/png")]
[assembly: WebResource(ASPxWebControl.WebImagesResourcePath + SpreadsheetRibbonImages.RibbonSSGWSpriteName + ".png", "image/png")]
[assembly: WebResource(ASPxWebControl.WebImagesResourcePath + RichEditRibbonImages.RibbonRESpriteName + ".png", "image/png")]
[assembly: WebResource(ASPxWebControl.WebImagesResourcePath + RichEditRibbonImages.RibbonREGSpriteName + ".png", "image/png")]
[assembly: WebResource(ASPxWebControl.WebImagesResourcePath + RichEditRibbonImages.RibbonREGWSpriteName + ".png", "image/png")]
[assembly: WebResource(ASPxWebControl.WebImagesResourcePath + GridImages.FormatConditionIconSetSpriteName + ".png", "image/png")]
[assembly: WebResource(ASPxEditBase.EditImagesResourcePath + ImagesBase.SpriteImageName + ".png", "image/png")]
[assembly: WebResource(ASPxEditBase.EditImagesResourcePath + EditorImages.TrackBarLargeTickHImageName + ".gif", "image/gif")]
[assembly: WebResource(ASPxEditBase.EditImagesResourcePath + EditorImages.TrackBarSmallTickHImageName + ".gif", "image/gif")]
[assembly: WebResource(ASPxEditBase.EditImagesResourcePath + EditorImages.TrackBarSmallTickVImageName + ".gif", "image/gif")]
[assembly: WebResource(ASPxEditBase.EditImagesResourcePath + EditorImages.TrackBarLargeTickVImageName + ".gif", "image/gif")]
[assembly: WebResource(ASPxEditBase.EditImagesResourcePath + EditorImages.TrackBarDoubleSmallTickVImageName + ".gif", "image/gif")]
[assembly: WebResource(ASPxEditBase.EditImagesResourcePath + EditorImages.TrackBarDoubleSmallTickHImageName + ".gif", "image/gif")]
[assembly: WebResource(ASPxEditBase.EditImagesResourcePath + EditorImages.TrackBarDoubleLargeTickHImageName + ".gif", "image/gif")]
[assembly: WebResource(ASPxEditBase.EditImagesResourcePath + EditorImages.TrackBarDoubleLargeTickVImageName + ".gif", "image/gif")]
[assembly: WebResource(ASPxEditBase.EditImagesResourcePath + EditorImages.TrackBarBarHighlightVImageName + ".gif", "image/gif")]
[assembly: WebResource(ASPxEditBase.EditImagesResourcePath + EditorImages.TrackBarBarHighlightHImageName + ".gif", "image/gif")]
[assembly: WebResource(ASPxEditBase.EditImagesResourcePath + EditorImages.TrackBarTrackVImageName + ".gif", "image/gif")]
[assembly: WebResource(ASPxEditBase.EditImagesResourcePath + EditorImages.TrackBarTrackHImageName + ".gif", "image/gif")]
[assembly: WebResource(ASPxEditBase.EditImagesResourcePath + ImagesBase.LoadingPanelImageName + ".gif", "image/gif")]
[assembly: WebResource(ASPxEditBase.EditImagesResourcePath + EditorImages.BinaryImageDesignImageName + ".gif", "image/gif")]
[assembly: WebResource(ASPxEditBase.EditImagesResourcePath + EditorImages.ButtonBackImageName + ".gif", "image/gif")]
[assembly: WebResource(ASPxEditBase.EditImagesResourcePath + EditorImages.ButtonHoverBackImageName + ".gif", "image/gif")]
[assembly: WebResource(ASPxEditBase.EditImagesResourcePath + EditorImages.SpinIncButtonBackImageName + ".gif", "image/gif")]
[assembly: WebResource(ASPxEditBase.EditImagesResourcePath + EditorImages.SpinDecButtonBackImageName + ".gif", "image/gif")]
[assembly: WebResource(ASPxEditBase.EditImagesResourcePath + EditorImages.CalendarButtonBackImageName + ".gif", "image/gif")]
[assembly: WebResource(ASPxEditBase.EditImagesResourcePath + EditorImages.CalendarButtonHoverBackImageName + ".gif", "image/gif")]
[assembly: WebResource(ASPxEditBase.EditImagesResourcePath + EditorImages.DropDownButtonBackImageName + ".gif", "image/gif")]
[assembly: WebResource(ASPxEditBase.EditImagesResourcePath + EditorImages.DropDownButtonHoverBackImageName + ".gif", "image/gif")]
[assembly: WebResource(ASPxEditBase.EditImagesResourcePath + EditorImages.TokenBoxTokenBackgroundImageName + ".png", "image/png")]
[assembly: WebResource(ColorPicker.ColorPickerImagesResourcePath + ColorPicker.ColorAreaImageName + ".png", "image/png")]
[assembly: WebResource(ColorPicker.ColorPickerImagesResourcePath + ColorPicker.ColorPointerImageName + ".gif", "image/gif")]
[assembly: WebResource(ColorPicker.ColorPickerImagesResourcePath + ColorPicker.HueAreaImageName + ".png", "image/png")]
[assembly: WebResource(ColorPicker.ColorPickerImagesResourcePath + ColorPicker.HuePointerImageName + ".gif", "image/gif")]
[assembly: WebResource(ASPxButton.ButtonScriptResourceName, "text/javascript")]
[assembly: WebResource(ASPxEditBase.EditScriptResourceName, "text/javascript")]
[assembly: WebResource(ASPxTextEdit.TextEditScriptResourceName, "text/javascript")]
[assembly: WebResource(ASPxTextEdit.MaskScriptResourceName, "text/javascript")]
[assembly: WebResource(ASPxListEdit.ListEditScriptResourceName, "text/javascript")]
[assembly: WebResource(ASPxCheckBox.CheckEditScriptResourceName, "text/javascript")]
[assembly: WebResource(ASPxStaticEdit.StaticEditScriptResourceName, "text/javascript")]
[assembly: WebResource(ASPxDropDownEditBase.DropDownEditScriptResourceName, "text/javascript")]
[assembly: WebResource(ASPxTokenBox.TokenBoxScriptResourceName, "text/javascript")]
[assembly: WebResource(ASPxDateEdit.DateEditScriptResourceName, "text/javascript")]
[assembly: WebResource(ASPxComboBox.ComboBoxScriptResourceName, "text/javascript")]
[assembly: WebResource(ASPxCalendar.CalendarScriptResourceName, "text/javascript")]
[assembly: WebResource(ASPxSpinEdit.SpinEditScriptResourceName, "text/javascript")]
[assembly: WebResource(ASPxFilterControlBase.FilterControlScriptResourceName, "text/javascript")]
[assembly: WebResource(ASPxProgressBar.ProgressBarScriptResourceName, "text/javascript")]
[assembly: WebResource(ASPxColorEdit.ColorEditScriptResourceName, "text/javascript")]
[assembly: WebResource(ColorNestedControl.ColorNestedControlScriptResourceName, "text/javascript")]
[assembly: WebResource(ColorTable.ColorTableScriptResourceName, "text/javascript")]
[assembly: WebResource(ColorPicker.ColorPickerScriptResourceName, "text/javascript")]
[assembly: WebResource(ASPxValidationSummary.ValidationSummaryScriptResourceName, "text/javascript")]
[assembly: WebResource(ASPxCaptcha.CaptchaScriptResourceName, "text/javascript")]
[assembly: WebResource(ASPxTrackBar.TrackBarScriptResourceName, "text/javascript")]
[assembly: WebResource(ASPxBinaryImage.PictureEditScriptResourceName, "text/javascript")]
[assembly: WebResource(ASPxEditBase.EditDefaultCssResourceName, "text/css", PerformSubstitution = true)]
[assembly: WebResource(ASPxEditBase.EditSpriteCssResourceName, "text/css", PerformSubstitution = true)]
[assembly: WebResource(ASPxEditBase.EditSystemCssResourceName, "text/css", PerformSubstitution = true)]
[assembly: WebResource(ASPxGridBase.GridScriptResourceName, "text/javascript")]
[assembly: WebResource(ASPxGridView.GridEndlessPagingScriptResourceName, "text/javascript")]
[assembly: WebResource(ASPxGridView.GridBatchEditingScriptResourceName, "text/javascript")]
[assembly: WebResource(ASPxGridView.GridViewScriptResourceName, "text/javascript")]
[assembly: WebResource(ASPxGridView.GridViewTableColumnResizingResourceName, "text/javascript")]
[assembly: WebResource(ASPxCardView.CardViewScriptResourceName, "text/javascript")]
[assembly: WebResource(ASPxGridLookup.GridLookupScriptResourceName, "text/javascript")]
[assembly: WebResource(ASPxGridView.GridViewDefaultCssResourceName, "text/css", PerformSubstitution = true)]
[assembly: WebResource(ASPxGridView.GridViewSpriteCssResourceName, "text/css", PerformSubstitution = true)]
[assembly: WebResource(ASPxCardView.CardViewDefaultCssResourceName, "text/css", PerformSubstitution = true)]
[assembly: WebResource(ASPxCardView.CardViewSpriteCssResourceName, "text/css", PerformSubstitution = true)]
[assembly: WebResource(ASPxGridView.GridViewResourceImagePath + GridViewImages.SpriteImageName + ".png", "image/png")]
[assembly: WebResource(ASPxGridView.GridViewResourceImagePath + GridViewImages.LoadingPanelImageName + ".gif", "image/gif")]
[assembly: WebResource(ASPxGridView.GridViewResourceImagePath + GridViewImages.LoadingPanelOnStatusBarName + ".gif", "image/gif")]
[assembly: WebResource(ASPxCardView.CardViewResourceImagePath + CardViewImages.SpriteImageName + ".png", "image/png")]
[assembly: WebResource(ASPxCardView.CardViewResourceImagePath + CardViewImages.LoadingPanelImageName + ".gif", "image/gif")]
[assembly: WebResource(ASPxCardView.CardViewResourceImagePath + CardViewImages.LoadingPanelOnStatusBarName + ".gif", "image/gif")]
[assembly: WebResource(MediaFileSelector.MediaFileSelectorScriptResourceName, "text/javascript")]
#endif
