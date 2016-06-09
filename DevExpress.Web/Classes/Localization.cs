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
using System.Globalization;
using System.Web;
using System.Xml;
using System.Resources;
using DevExpress.Utils.Localization;
using DevExpress.Utils.Localization.Internal;
using DevExpress.Web.Internal;
using System.Collections.Generic;
namespace DevExpress.Web.Localization {
	public abstract class ASPxResLocalizerBase<T> : XtraLocalizer<T> where T : struct {
		XtraLocalizer<T> embeddedLocalizer;		
		public ASPxResLocalizerBase(XtraLocalizer<T> embeddedLocalizer) {
			if(embeddedLocalizer == null)
				throw new ArgumentNullException();
			this.embeddedLocalizer = embeddedLocalizer;			
		}		
		protected override void PopulateStringTable() {
		}
		public bool IsDesignMode { get { return HttpContext.Current == null; } }
		public override string Language { get { return CultureInfo.CurrentUICulture.Name; } }		
		public override string GetLocalizedString(T id) {
			string result = base.GetLocalizedString(id);
			if(result == String.Empty) {
				lock(this) {
					result = base.GetLocalizedString(id);
					if(result == String.Empty) {
						result = GetLocalizedStringCore(id);
						AddString(id, result);
					}
				}
			}
			return result;
		}
		string GetLocalizedStringCore(T id) {
			if(IsDesignMode)
				return embeddedLocalizer.GetLocalizedString(id);
			string resourceKey = GetEnumTypeName() + "." + id;			
			string result = GetLocalizedStringFromGlobalResource(resourceKey);
			if(String.IsNullOrEmpty(result))
				result = GetLocalizedStringFromResx(resourceKey);
			if(String.IsNullOrEmpty(result))
				result = embeddedLocalizer.GetLocalizedString(id);
			return result;
		}
		public override XmlDocument CreateXmlDocument() {
			return embeddedLocalizer.CreateXmlDocument();
		}
		public override XtraLocalizer<T> CreateResXLocalizer() {
			return embeddedLocalizer.CreateResXLocalizer();
		}
		#region Support for LocalizationRes.resx
		System.Resources.ResourceManager ResxManager;
		protected abstract string ResxName { get; }
		string GetLocalizedStringFromResx(string resourceKey) {
			if(ResxManager == null)
				ResxManager = new System.Resources.ResourceManager(ResxName, typeof(T).Assembly);
			return ResxManager.GetString(resourceKey);
		}
		#endregion
		#region Support for App_GlobalResources
		bool GlobalResourceMissing;
		protected abstract string GlobalResourceAssemblyName { get; }		
		string GetLocalizedStringFromGlobalResource(string resourceKey) {
			if(GlobalResourceMissing)
				return null;
			string resourceName = GlobalResourceAssemblyName.Replace(".", "_");			
			try {
				return HttpContext.GetGlobalResourceObject(resourceName, resourceKey) as String;
			} catch(MissingManifestResourceException) {
				GlobalResourceMissing = true;
			} catch {
			}
			return null;
		}
		#endregion
	}
	public enum ASPxperienceStringId {
		CallbackGenericErrorText,
		Loading,
		Pager_All, Pager_First, Pager_Prev, Pager_Next, Pager_Last,
		Pager_SummaryFormat, Pager_SummaryAllPagesFormat, Pager_SummaryEmptyText, Pager_PageSize, Pager_PageSizeAllItem,
		DataView_PagerSummaryFormat, DataView_PagerSummaryAllPages, DataView_PagerPageSize, DataView_PagerRowPerPage, DataView_ShowMoreItemsText, DataViewBase_EmptyDataText,
		NewsControl_BackToTop, NewsControl_Page,
		PopupControl_CloseButton, PopupControl_PinButton, PopupControl_RefreshButton, PopupControl_CollapseButton, PopupControl_MaximizeButton, PopupControl_SizeGrip,
		RoundPanel_CollapseButton,
		TitleIndex_FilterCaption, TitleIndex_FilterHint, TitleIndex_BackToTop, TitleIndex_NoData,
		UploadControl_MaxSize, UploadControl_AccessDeniedError, UploadControl_GeneralError, UploadControl_UnspecifiedError, UploadControl_UploadWasCanceledError, UploadControl_PlatformErrorText, UploadControl_EnctypeError,
		UploadControl_MultiSelection, UploadControl_NotAllowedContentTypes, UploadControl_NotAllowedFileExtension, UploadControl_FileDoesNotExistError,
		UploadControl_ClearFileSelectionButtonToolTip, UploadControl_RemoveButton, UploadControl_AddButton, UploadControl_BrowseButton, UploadControl_UploadButton, UploadControl_CancelButton,
		UploadControl_InvalidWindowsFileName, UploadControl_SelectedSeveralFiles, UploadControl_TooManyFilesError, UploadControl_OperationTimeoutError, UploadControl_UploadModeNotSupported, UploadControl_DropZone,
		UploadControl_DragAndDropMoreThanOneFileError, UploadControl_MaxFileCountError, UploadControl_AccessibilityTitleForFakeInput,
		TreeView_AltExpand,
		TreeView_AltCollapse,
		TreeView_AltLoading,
		FileManager_FolderLocked,
		FileManager_Folder,
		FileManager_Filter,
		FileManager_Path,
		FileManager_TbCreate,
		FileManager_TbRename,
		FileManager_TbMove,
		FileManager_TbDelete,
		FileManager_TbRefresh,
		FileManager_TbDownload,
		FileManager_TbCopy,
		FileManager_DeleteConfirm,
		FileManager_FolderBrowserPopupHeader,
		FileManager_Ok,
		FileManager_Cancel,
		FileManager_NewFolderName,
		FileManager_InformationUnitSymbol,
		FileManager_UploadProgressPopupText,
		FileManager_ErrorRootFolderNotSpecified,
		FileManager_ErrorNoAccess,
		FileManager_ErrorIO,
		FileManager_ErrorFileNotFound,
		FileManager_ErrorFolderNotFound,
		FileManager_ErrorOther,
		FileManager_ErrorNameCannotBeEmpty,
		FileManager_ErrorInvalidSymbols,
		FileManager_ErrorWrongExtension,
		FileManager_ErrorAlreadyExists,
		FileManager_ErrorAccessProhibited,
		FileManager_ErrorCloudAccessFailed,
		FileManager_ErrorPathToLong,
		FileManager_ErrorThumbnail,
		FileManager_ErrorUsedByAnotherProcess,
		FileManager_ErrorUploadSeveralFiles,
		FileManager_ErrorWrongIdPathLength,
		FileManager_GridViewFilter_SizeEmpty,
		FileManager_GridViewFilter_SizeTiny,
		FileManager_GridViewFilter_SizeSmall,
		FileManager_GridViewFilter_SizeMedium,
		FileManager_GridViewFilter_SizeLarge,
		FileManager_GridViewFilter_SizeHuge,
		FileManager_GridViewFilter_SizeGigantic,
		FileManager_GridViewFilter_DateToday,
		FileManager_GridViewFilter_DateWeek,
		FileManager_GridViewFilter_DateMonth,
		FileManager_GridViewFilter_DateYear,
		FileManager_FileInfoTypeCaption_FileName,
		FileManager_FileInfoTypeCaption_LastWriteTime,
		FileManager_FileInfoTypeCaption_Size,
		Ribbon_FileTabText,
		ImageZoom_HintText,
		Documents_CantSaveToAlreadyOpenedFileMessage,
		InsertTableControl_InsertTable,
		GridViewFilterRow_InputTitle,
		AccessibilityIFrameTitle,
		Accessibility_NoneSortOrder,
		Accessibility_AscendingSortOrder,
		Accessibility_DescendingSortOrder,
		Accessibility_ClientCalendarMultiSelect,
		Accessibility_GridViewHeaderLinkFormat,
		Accessibility_PagerPreviousPage,
		Accessibility_PagerNextPage,
		Accessibility_PagerSummaryFormat,
		Accessibility_PagerNavigationFormat,
		Accessibility_PagerDescription,
		Accessibility_TableItemFormat,
		Accessibility_EmptyItem,
		Accessibility_ItemPositionFormat,
		Accessibility_CheckBoxColumnHeader,
		Accessibility_CheckBoxListItemFormat,
		Accessibility_CalendarDescription,
		Accessibility_TreeListDescriptionFormat,
		Accessibility_TreeListNavigationDescription,
		Accessibility_TreeListCheckBoxSelectionDescription,
		Accessibility_TreeListHeaderLinkFormat,
		Accessibility_TreeListSelectAllCheckBox,
		Accessibility_TreeListCollapseExpandButtonFormat,
		Accessibility_TreeListCollapse,
		Accessibility_TreeListExpand,
		Accessibility_TreeListDataCheckBoxDescriptionFormat,
		Accessibility_Not,
		Accessibility_TreeListNodeExpandedState,
		Accessibility_TreeListNodeCollapsedState
	}
	public class ASPxperienceLocalizer : XtraLocalizer<ASPxperienceStringId> {
		static ASPxperienceLocalizer() {
			SetActiveLocalizerProvider(new ASPxActiveLocalizerProvider<ASPxperienceStringId>(CreateResLocalizerInstance));
		}
		static XtraLocalizer<ASPxperienceStringId> CreateResLocalizerInstance() {
			return new ASPxperienceResLocalizer();
		}
		public static string GetString(ASPxperienceStringId id) {
			return Active.GetLocalizedString(id);
		}
		public override XtraLocalizer<ASPxperienceStringId> CreateResXLocalizer() {
			return CreateResLocalizerInstance();
		}
		protected override void PopulateStringTable() {
			AddString(ASPxperienceStringId.CallbackGenericErrorText, StringResources.CallbackGenericErrorText);
			AddString(ASPxperienceStringId.Loading, StringResources.LoadingPanelText);
			AddString(ASPxperienceStringId.Pager_All, StringResources.Pager_AllButtonText);
			AddString(ASPxperienceStringId.Pager_First, StringResources.Pager_FirstPageText);
			AddString(ASPxperienceStringId.Pager_Prev, StringResources.Pager_PreviousPageText);
			AddString(ASPxperienceStringId.Pager_Next, StringResources.Pager_NextPageText);
			AddString(ASPxperienceStringId.Pager_Last, StringResources.Pager_LastPageText);
			AddString(ASPxperienceStringId.Pager_SummaryAllPagesFormat, StringResources.Pager_SummaryAllPagesFormatString);
			AddString(ASPxperienceStringId.Pager_SummaryEmptyText, StringResources.Pager_SummaryEmptyTextString);
			AddString(ASPxperienceStringId.Pager_SummaryFormat, StringResources.Pager_SummaryFormatString);
			AddString(ASPxperienceStringId.Pager_PageSizeAllItem, StringResources.Pager_PageSizeAllItemText);
			AddString(ASPxperienceStringId.Pager_PageSize, StringResources.Pager_PageSizeText);
			AddString(ASPxperienceStringId.DataView_PagerSummaryFormat, StringResources.DataView_PagerSummaryFormat);
			AddString(ASPxperienceStringId.DataView_PagerSummaryAllPages, StringResources.DataView_PagerSummaryAllPages);
			AddString(ASPxperienceStringId.DataView_PagerPageSize, StringResources.DataView_PagerPageSizeText);
			AddString(ASPxperienceStringId.DataView_PagerRowPerPage, StringResources.DataView_PagerRowPerPageText);
			AddString(ASPxperienceStringId.DataView_ShowMoreItemsText, StringResources.DataView_ShowMoreItemsText);
			AddString(ASPxperienceStringId.DataViewBase_EmptyDataText, StringResources.DataViewBase_EmptyDataText);
			AddString(ASPxperienceStringId.NewsControl_BackToTop, StringResources.NewsControl_BackToTopText);
			AddString(ASPxperienceStringId.NewsControl_Page, StringResources.NewsControl_PagerSummary_Page);
			AddString(ASPxperienceStringId.PopupControl_CloseButton, StringResources.Alt_PopupControlCloseButton);
			AddString(ASPxperienceStringId.PopupControl_SizeGrip, StringResources.Alt_PopupControlSizeGrip);
			AddString(ASPxperienceStringId.PopupControl_CollapseButton, StringResources.Alt_PopupControlCollapseButton);
			AddString(ASPxperienceStringId.PopupControl_MaximizeButton, StringResources.Alt_PopupControlMaximizeButton);
			AddString(ASPxperienceStringId.PopupControl_PinButton, StringResources.Alt_PopupControlPinButton);
			AddString(ASPxperienceStringId.PopupControl_RefreshButton, StringResources.Alt_PopupControlRefreshButton);
			AddString(ASPxperienceStringId.RoundPanel_CollapseButton, StringResources.Alt_RoundPanelCollapseButton);
			AddString(ASPxperienceStringId.TitleIndex_FilterCaption, StringResources.TitleIndex_FilterCaption);
			AddString(ASPxperienceStringId.TitleIndex_FilterHint, StringResources.TitleIndex_FilterHint);
			AddString(ASPxperienceStringId.TitleIndex_BackToTop, StringResources.TitleIndex_DefaultBackToTopText);
			AddString(ASPxperienceStringId.TitleIndex_NoData, StringResources.TitleIndex_DefaultNoDataText);
			AddString(ASPxperienceStringId.UploadControl_MaxSize, StringResources.UploadControl_MaxSize);
			AddString(ASPxperienceStringId.UploadControl_AccessDeniedError, StringResources.UploadControl_AccessDeniedErrorText);
			AddString(ASPxperienceStringId.UploadControl_GeneralError, StringResources.UploadControl_GeneralErrorText);
			AddString(ASPxperienceStringId.UploadControl_UnspecifiedError, StringResources.UploadControl_UnspecifiedErrorText);
			AddString(ASPxperienceStringId.UploadControl_UploadWasCanceledError, StringResources.UploadControl_UploadWasCanceledErrorText);
			AddString(ASPxperienceStringId.UploadControl_PlatformErrorText, StringResources.UploadControl_PlatformErrorText);
			AddString(ASPxperienceStringId.UploadControl_UploadModeNotSupported, StringResources.UploadControl_UploadModeNotSupported);
			AddString(ASPxperienceStringId.UploadControl_EnctypeError, StringResources.UploadControl_EnctypeError);
			AddString(ASPxperienceStringId.UploadControl_MultiSelection, StringResources.UploadControl_MultiSelectionErrorText);
			AddString(ASPxperienceStringId.UploadControl_MaxFileCountError, StringResources.UploadControl_MaxFileCountErrorText);
			AddString(ASPxperienceStringId.UploadControl_NotAllowedContentTypes, StringResources.UploadControl_NotAllowedContentTypes);
			AddString(ASPxperienceStringId.UploadControl_NotAllowedFileExtension, StringResources.UploadControl_NotAllowedFileExtension);
			AddString(ASPxperienceStringId.UploadControl_FileDoesNotExistError, StringResources.UploadControl_FileDoesNotExistErrorText);
			AddString(ASPxperienceStringId.UploadControl_ClearFileSelectionButtonToolTip, StringResources.UploadControl_ClearFileSelectionButtonToolTip);
			AddString(ASPxperienceStringId.UploadControl_RemoveButton, StringResources.UploadControl_RemoveButtonText);
			AddString(ASPxperienceStringId.UploadControl_AddButton, StringResources.UploadControl_AddButtonText);
			AddString(ASPxperienceStringId.UploadControl_BrowseButton, StringResources.UploadControl_BrowseButtonText);
			AddString(ASPxperienceStringId.UploadControl_UploadButton, StringResources.UploadControl_UploadButtonText);
			AddString(ASPxperienceStringId.UploadControl_CancelButton, StringResources.UploadControl_CancelButtonText);
			AddString(ASPxperienceStringId.UploadControl_InvalidWindowsFileName, StringResources.UploadControl_InvalidWindowsFileNameText);
			AddString(ASPxperienceStringId.UploadControl_SelectedSeveralFiles, StringResources.UploadControl_SelectedSeveralFilesText);
			AddString(ASPxperienceStringId.UploadControl_TooManyFilesError, StringResources.UploadControl_TooManyFilesErrorText);
			AddString(ASPxperienceStringId.UploadControl_OperationTimeoutError, StringResources.UploadControl_OperationTimeoutErrorText);
			AddString(ASPxperienceStringId.UploadControl_DropZone, StringResources.UploadControl_DropZoneText);
			AddString(ASPxperienceStringId.UploadControl_DragAndDropMoreThanOneFileError, StringResources.UploadControl_DragAndDropMoreThanOneFileErrorText);
			AddString(ASPxperienceStringId.UploadControl_AccessibilityTitleForFakeInput, StringResources.UploadControl_AccessibilityTitleForFakeInputText);
			AddString(ASPxperienceStringId.TreeView_AltCollapse, StringResources.TreeView_AltCollapse);
			AddString(ASPxperienceStringId.TreeView_AltExpand, StringResources.TreeView_AltExpand);
			AddString(ASPxperienceStringId.TreeView_AltLoading, StringResources.TreeView_AltLoading);
			AddString(ASPxperienceStringId.FileManager_FolderLocked, StringResources.FileManager_FolderLocked);
			AddString(ASPxperienceStringId.FileManager_Folder, StringResources.FileManager_Folder);
			AddString(ASPxperienceStringId.FileManager_Filter, StringResources.FileManager_Filter);
			AddString(ASPxperienceStringId.FileManager_Path, StringResources.FileManager_Path);
			AddString(ASPxperienceStringId.FileManager_TbCreate, StringResources.FileManager_TbCreate);
			AddString(ASPxperienceStringId.FileManager_TbRename, StringResources.FileManager_TbRename);
			AddString(ASPxperienceStringId.FileManager_TbMove, StringResources.FileManager_TbMove);
			AddString(ASPxperienceStringId.FileManager_TbDelete, StringResources.FileManager_TbDelete);
			AddString(ASPxperienceStringId.FileManager_TbRefresh, StringResources.FileManager_TbRefresh);
			AddString(ASPxperienceStringId.FileManager_TbDownload, StringResources.FileManager_TbDownload);
			AddString(ASPxperienceStringId.FileManager_TbCopy, StringResources.FileManager_TbCopy);
			AddString(ASPxperienceStringId.FileManager_DeleteConfirm, StringResources.FileManager_DeleteConfirm);
			AddString(ASPxperienceStringId.FileManager_FolderBrowserPopupHeader, StringResources.FileManager_FolderBrowserPopupHeader);
			AddString(ASPxperienceStringId.FileManager_Ok, StringResources.FileManager_Ok);
			AddString(ASPxperienceStringId.FileManager_Cancel, StringResources.FileManager_Cancel);
			AddString(ASPxperienceStringId.FileManager_NewFolderName, StringResources.FileManager_NewFolderName);
			AddString(ASPxperienceStringId.FileManager_InformationUnitSymbol, StringResources.FileManager_InformationUnitSymbol);
			AddString(ASPxperienceStringId.FileManager_UploadProgressPopupText, StringResources.FileManager_UploadProgressPopupText);
			AddString(ASPxperienceStringId.FileManager_ErrorRootFolderNotSpecified, StringResources.FileManager_ErrorRootFolderNotSpecified);
			AddString(ASPxperienceStringId.FileManager_ErrorFileNotFound, StringResources.FileManager_ErrorFileNotFound);
			AddString(ASPxperienceStringId.FileManager_ErrorFolderNotFound, StringResources.FileManager_ErrorFolderNotFound);
			AddString(ASPxperienceStringId.FileManager_ErrorIO, StringResources.FileManager_ErrorIO);
			AddString(ASPxperienceStringId.FileManager_ErrorNoAccess, StringResources.FileManager_ErrorNoAccess);
			AddString(ASPxperienceStringId.FileManager_ErrorOther, StringResources.FileManager_ErrorOther);
			AddString(ASPxperienceStringId.FileManager_ErrorNameCannotBeEmpty, StringResources.FileManager_ErrorNameCannotBeEmpty);
			AddString(ASPxperienceStringId.FileManager_ErrorInvalidSymbols, StringResources.FileManager_ErrorInvalidSymbols);
			AddString(ASPxperienceStringId.FileManager_ErrorWrongExtension, StringResources.FileManager_ErrorWrongExtension);
			AddString(ASPxperienceStringId.FileManager_ErrorUsedByAnotherProcess, StringResources.FileManager_ErrorUsedByAnotherProcess);
			AddString(ASPxperienceStringId.FileManager_ErrorAlreadyExists, StringResources.FileManager_ErrorAlreadyExists);
			AddString(ASPxperienceStringId.FileManager_ErrorAccessProhibited, StringResources.FileManager_ErrorAccessProhibited);
			AddString(ASPxperienceStringId.FileManager_ErrorCloudAccessFailed, StringResources.FileManager_ErrorCloudAccessFailed);
			AddString(ASPxperienceStringId.FileManager_ErrorPathToLong, StringResources.FileManager_ErrorPathToLong);
			AddString(ASPxperienceStringId.FileManager_ErrorThumbnail, StringResources.FileManager_ErrorThumbnail);
			AddString(ASPxperienceStringId.FileManager_ErrorUploadSeveralFiles, StringResources.FileManager_ErrorUploadSeveralFiles);
			AddString(ASPxperienceStringId.FileManager_ErrorWrongIdPathLength, StringResources.FileManager_ErrorWrongIdPathLength);
			AddString(ASPxperienceStringId.FileManager_GridViewFilter_SizeEmpty, StringResources.FileManager_GridViewFilter_SizeEmpty);
			AddString(ASPxperienceStringId.FileManager_GridViewFilter_SizeTiny, StringResources.FileManager_GridViewFilter_SizeTiny);
			AddString(ASPxperienceStringId.FileManager_GridViewFilter_SizeSmall, StringResources.FileManager_GridViewFilter_SizeSmall);
			AddString(ASPxperienceStringId.FileManager_GridViewFilter_SizeMedium, StringResources.FileManager_GridViewFilter_SizeMedium);
			AddString(ASPxperienceStringId.FileManager_GridViewFilter_SizeLarge, StringResources.FileManager_GridViewFilter_SizeLarge);
			AddString(ASPxperienceStringId.FileManager_GridViewFilter_SizeHuge, StringResources.FileManager_GridViewFilter_SizeHuge);
			AddString(ASPxperienceStringId.FileManager_GridViewFilter_SizeGigantic, StringResources.FileManager_GridViewFilter_SizeGigantic);
			AddString(ASPxperienceStringId.FileManager_GridViewFilter_DateToday, StringResources.FileManager_GridViewFilter_DateToday);
			AddString(ASPxperienceStringId.FileManager_GridViewFilter_DateWeek, StringResources.FileManager_GridViewFilter_DateWeek);
			AddString(ASPxperienceStringId.FileManager_GridViewFilter_DateMonth, StringResources.FileManager_GridViewFilter_DateMonth);
			AddString(ASPxperienceStringId.FileManager_GridViewFilter_DateYear, StringResources.FileManager_GridViewFilter_DateYear);
			AddString(ASPxperienceStringId.FileManager_FileInfoTypeCaption_FileName, StringResources.FileManager_FileInfoTypeCaption_FileName);
			AddString(ASPxperienceStringId.FileManager_FileInfoTypeCaption_LastWriteTime, StringResources.FileManager_FileInfoTypeCaption_LastWriteTime);
			AddString(ASPxperienceStringId.FileManager_FileInfoTypeCaption_Size, StringResources.FileManager_FileInfoTypeCaption_Size);
			AddString(ASPxperienceStringId.Ribbon_FileTabText, StringResources.Ribbon_FileTabText);
			AddString(ASPxperienceStringId.ImageZoom_HintText, StringResources.ImageZoom_HintText);
			AddString(ASPxperienceStringId.InsertTableControl_InsertTable, StringResources.InsertTableControl_InsertTable);
			AddString(ASPxperienceStringId.GridViewFilterRow_InputTitle, StringResources.GridViewFilterRow_InputTitleText);
			AddString(ASPxperienceStringId.Accessibility_TableItemFormat, StringResources.Accessibility_TableItemFormatString);
			AddString(ASPxperienceStringId.Accessibility_ClientCalendarMultiSelect, StringResources.Accessibility_ClientCalendarMultiSelectText);
			AddString(ASPxperienceStringId.AccessibilityIFrameTitle, StringResources.AccessibilityIFrameTitleText);
			AddString(ASPxperienceStringId.Accessibility_NoneSortOrder, StringResources.Accessibility_NoneSortOrderText);
			AddString(ASPxperienceStringId.Accessibility_AscendingSortOrder, StringResources.Accessibility_AscendingSortOrderText);
			AddString(ASPxperienceStringId.Accessibility_DescendingSortOrder, StringResources.Accessibility_DescendingSortOrderText);
			AddString(ASPxperienceStringId.Accessibility_GridViewHeaderLinkFormat, StringResources.Accessibility_GridViewHeaderLinkFormatString);
			AddString(ASPxperienceStringId.Accessibility_PagerPreviousPage, StringResources.Accessibility_PagerPreviousPageText);
			AddString(ASPxperienceStringId.Accessibility_PagerNextPage, StringResources.Accessibility_PagerNextPageText);
			AddString(ASPxperienceStringId.Accessibility_PagerSummaryFormat, StringResources.Accessibility_PagerSummaryFormatString);
			AddString(ASPxperienceStringId.Accessibility_PagerNavigationFormat, StringResources.Accessibility_PagerNavigationFormatString);
			AddString(ASPxperienceStringId.Accessibility_PagerDescription, StringResources.Accessibility_PagerDescriptionText);
			AddString(ASPxperienceStringId.Accessibility_EmptyItem, StringResources.Accessibility_EmptyItemText);
			AddString(ASPxperienceStringId.Accessibility_ItemPositionFormat, StringResources.Accessibility_ItemPositionFormatString);
			AddString(ASPxperienceStringId.Accessibility_CheckBoxColumnHeader, StringResources.Accessibility_CheckBoxColumnHeaderText);
			AddString(ASPxperienceStringId.Accessibility_CheckBoxListItemFormat, StringResources.Accessibility_CheckBoxListItemFormatString);
			AddString(ASPxperienceStringId.Accessibility_CalendarDescription, StringResources.Accessibility_CalendarDescriptionText);
			AddString(ASPxperienceStringId.Accessibility_TreeListDescriptionFormat, StringResources.Accessibility_TreeListDescriptionFormatString);
			AddString(ASPxperienceStringId.Accessibility_TreeListNavigationDescription, StringResources.Accessibility_TreeListNavigationDescriptionText);
			AddString(ASPxperienceStringId.Accessibility_TreeListCheckBoxSelectionDescription, StringResources.Accessibility_TreeListCheckBoxSelectionDescriptionText);
			AddString(ASPxperienceStringId.Accessibility_TreeListHeaderLinkFormat, StringResources.Accessibility_TreeListHeaderLinkFormatString);
			AddString(ASPxperienceStringId.Accessibility_TreeListSelectAllCheckBox, StringResources.Accessibility_TreeListSelectAllCheckBoxText);
			AddString(ASPxperienceStringId.Accessibility_TreeListCollapseExpandButtonFormat, StringResources.Accessibility_TreeListCollapseExpandButtonFormatString);
			AddString(ASPxperienceStringId.Accessibility_TreeListCollapse, StringResources.Accessibility_TreeListCollapseText);
			AddString(ASPxperienceStringId.Accessibility_TreeListExpand, StringResources.Accessibility_TreeListExpandText);
			AddString(ASPxperienceStringId.Accessibility_TreeListDataCheckBoxDescriptionFormat, StringResources.Accessibility_TreeListDataCheckBoxDescriptionFormatString);
			AddString(ASPxperienceStringId.Accessibility_Not, StringResources.Accessibility_Not);
			AddString(ASPxperienceStringId.Accessibility_TreeListNodeExpandedState, StringResources.Accessibility_TreeListNodeExpandedStateText);
			AddString(ASPxperienceStringId.Accessibility_TreeListNodeCollapsedState, StringResources.Accessibility_TreeListNodeCollapsedStateText);
			AddString(ASPxperienceStringId.Documents_CantSaveToAlreadyOpenedFileMessage, StringResources.Documents_CantSaveToAlreadyOpenedFileMessage);
		}
	}
	public class ASPxperienceResLocalizer : ASPxResLocalizerBase<ASPxperienceStringId> {
		class ASPxperienceStringIdComparer : IEqualityComparer<ASPxperienceStringId> {
			public bool Equals(ASPxperienceStringId x, ASPxperienceStringId y) {
				return x == y;
			}
			public int GetHashCode(ASPxperienceStringId obj) {
				return (int)obj;
			}
		}
		public ASPxperienceResLocalizer()
			: base(new ASPxperienceLocalizer()) {
		}
		protected override string GlobalResourceAssemblyName { get { return AssemblyInfo.SRAssemblyWeb; } }
		protected override string ResxName { get { return "DevExpress.Web.Classes.LocalizationRes"; } }
		protected override IEqualityComparer<ASPxperienceStringId> CreateComparer() {
			return new ASPxperienceStringIdComparer();
		}
	}
	public class ASPxActiveLocalizerProvider<T> : ActiveLocalizerProvider<T> where T : struct {
		public delegate XtraLocalizer<U> InstanceActivator<U>() where U : struct;
		InstanceActivator<T> activator;
		public ASPxActiveLocalizerProvider(InstanceActivator<T> activator)
			: base(null) {
			this.activator = activator;
		}
		protected override XtraLocalizer<T> GetActiveLocalizerCore() {
			HttpContext context = HttpContext.Current;
			if(context == null)
				return this.activator();
			object key = GetType();
			if(!context.Items.Contains(key))
				context.Items[key] = this.activator();
			return (XtraLocalizer<T>)context.Items[key];
		}
		protected override void SetActiveLocalizerCore(XtraLocalizer<T> localizer) {
		}
	}
}
