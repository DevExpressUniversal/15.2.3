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
using DevExpress.Utils.Localization;
using DevExpress.Utils.Localization.Internal;
using System.Resources;
namespace DevExpress.XtraBars.Docking2010 {
	#region enum DocumentManagerLocalizerId
	public enum DocumentManagerStringId {
		CommandActivate,
		CommandClose,
		CommandCloseAll,
		CommandCloseAllButThis,
		CommandFloat,
		CommandFloatAll,
		CommandOpenedWindowsDialog,
		CommandResetLayout,
		CommandWorkspacesDialog,
		CommandDock,
		CommandDockWidget,
		CommandNewDocumentGroup,
		CommandNewHorizontalDocumentGroup,
		CommandNewVerticalDocumentGroup,
		CommandMoveToNextDocumentGroup,
		CommandMoveToPrevDocumentGroup,
		CommandMoveToMainDocumentGroup,
		CommandPinTab,
		CommandUnpinTab,
		CommandCloseAllButPinned,
		CommandVerticalOrientation,
		CommandHorizontalOrientation,
		CommandDockAll,
		CommandCascade,
		CommandTileVertical,
		CommandTileHorizontal,
		CommandMinimizeAll,
		CommandArrangeIcons,
		CommandRestoreAll,
		CommandMaximize,
		CommandRestore,
		CommandBack,
		CommandBackDescription,
		CommandHome,
		CommandHomeDescription,
		CommandRotate,
		CommandFlip,
		CommandRotateDescription,
		CommandFlipDescription,
		CommandTileSmallSize,
		CommandTileSmallSizeDescription,
		CommandTileLargeSize,
		CommandTileLargeSizeDescription,
		CommandTileWideSize,
		CommandTileWideSizeDescription,
		CommandTileMediumSize,
		CommandTileMediumSizeDescription,
		CommandTileSizeFlyoutPanel,
		CommandTileSizeFlyoutPanelDescription,
		CommandToggleTileSize,
		CommandToggleTileSizeDescription,
		CommandClearSelection,
		CommandClearSelectionDescription,
		CommandDetail,
		CommandDetailDescription,
		CommandOverview,
		CommandOverviewDescription,
		CommandExit,
		CommandExitDescription,
		CommandEnableFullScreenMode,
		CommandEnableFullScreenModeDescription,
		CommandDisableFullScreenMode,
		CommandDisableFullScreenModeDescription,
		RemoveWorkspaceItemCaption,
		CaptureWorkspaceItemCaption,
		LoadWorkspaceItemCaption,
		SaveWorkspaceItemCaption,
		SaveCurrentWorkspaceItemCaption,
		SaveWorkspaceFormCaption,
		LoadWorkspaceFormCaption,
		WorkspaceNameFormErrorMessage,
		WorkspaceNameFormCaption,
		WorkspaceNameWarningMessage,
		WorkspaceRemoveWarningMessage,
		WorkspaceRemoveCaption,
		OpenedWindowsDialogNameColumnCaption,
		OpenedWindowsDialogPathColumnCaption,
		WorkspacesDialogNameColumnCaption,
		WorkspacesDialogPathColumnCaption,
		OpenedWindowsDialogCaption,
		WorkspacesDialogCaption,
		ActivateDocumentButtonText,
		CloseDocumentButtonText,
		CloseAllDocumentsButtonText,
		CloseDocumentWarningMessage,
		CloseAllDocumentsWarningMessage,
		ApplyWorkspaceButtonText,
		RemoveWorkspaceButtonText,
		RemoveAllWorkspacesButtonText,
		LoadWorkspaceButtonText,
		SaveWorkspaceButtonText,
		SaveWorkspaceAsButtonText,
		RemoveAllWorkspacesWarningMessage,
		DeferredLoadingExceptionMessage,
		DuplicateDocumentInTileExceptionMessage,
		NonDocumentModeInitializationExceptionMessage,
		NullParentContainerExceptionMessage,
		OverlapAllControlsWarning,
		LoadingIndicatorCaption,
		LoadingIndicatorDescription,
		SplashScreenCaption,
		NoPreviewAvailableText
	}
	#endregion
	public class DocumentManagerLocalizer : XtraLocalizer<DocumentManagerStringId> {
		#region static
		static DocumentManagerLocalizer() {
			SetActiveLocalizerProvider(
					new DefaultActiveLocalizerProvider<DocumentManagerStringId>(CreateDefaultLocalizer())
				);
		}
		public static XtraLocalizer<DocumentManagerStringId> CreateDefaultLocalizer() {
			return new DocumentManagerResXLocalizer();
		}
		public new static XtraLocalizer<DocumentManagerStringId> Active { 
			get { return XtraLocalizer<DocumentManagerStringId>.Active; }
			set { XtraLocalizer<DocumentManagerStringId>.Active = value; }
		}
		public static string GetString(DocumentManagerStringId id) {
			return Active.GetLocalizedString(id);
		}
		#endregion static
		public override XtraLocalizer<DocumentManagerStringId> CreateResXLocalizer() {
			return new DocumentManagerResXLocalizer();
		}
		protected override void PopulateStringTable() {
			#region AddString
			AddString(DocumentManagerStringId.CommandActivate, "Activate");
			AddString(DocumentManagerStringId.CommandClose, "Close");
			AddString(DocumentManagerStringId.CommandCloseAll, "Close All Documents");
			AddString(DocumentManagerStringId.CommandCloseAllButThis, "Close All But This");
			AddString(DocumentManagerStringId.CommandOpenedWindowsDialog, "Windows...");
			AddString(DocumentManagerStringId.CommandResetLayout, "Reset Window Layout");
			AddString(DocumentManagerStringId.CommandWorkspacesDialog, "Workspaces...");
			AddString(DocumentManagerStringId.OpenedWindowsDialogNameColumnCaption, "Name");
			AddString(DocumentManagerStringId.OpenedWindowsDialogPathColumnCaption, "Path");
			AddString(DocumentManagerStringId.WorkspacesDialogNameColumnCaption, "Name");
			AddString(DocumentManagerStringId.WorkspacesDialogPathColumnCaption, "Path");
			AddString(DocumentManagerStringId.OpenedWindowsDialogCaption, "Windows");
			AddString(DocumentManagerStringId.WorkspacesDialogCaption, "Workspaces");
			AddString(DocumentManagerStringId.ActivateDocumentButtonText, "Activate Document");
			AddString(DocumentManagerStringId.CloseDocumentButtonText, "Close Document");
			AddString(DocumentManagerStringId.CloseAllDocumentsButtonText, "Close All Documents");
			AddString(DocumentManagerStringId.CloseDocumentWarningMessage, "Are you sure you want to close the {0} document?");
			AddString(DocumentManagerStringId.CloseAllDocumentsWarningMessage, "Are you sure you want to close all documents?");
			AddString(DocumentManagerStringId.ApplyWorkspaceButtonText, "Apply Workspace");
			AddString(DocumentManagerStringId.RemoveWorkspaceButtonText, "Remove Workspace");
			AddString(DocumentManagerStringId.RemoveAllWorkspacesButtonText, "Remove All Workspaces");
			AddString(DocumentManagerStringId.LoadWorkspaceButtonText, "Load Workspace...");
			AddString(DocumentManagerStringId.SaveWorkspaceButtonText, "Save Workspace");
			AddString(DocumentManagerStringId.SaveWorkspaceAsButtonText, "Save Workspace As...");
			AddString(DocumentManagerStringId.RemoveAllWorkspacesWarningMessage, "Are you sure you want to remove all workspaces?");
			AddString(DocumentManagerStringId.CommandFloat, "Float");
			AddString(DocumentManagerStringId.CommandFloatAll, "Float All");
			AddString(DocumentManagerStringId.CommandDock, "Dock as Tabbed Document");
			AddString(DocumentManagerStringId.CommandDockWidget, "Dock");
			AddString(DocumentManagerStringId.CommandNewDocumentGroup, "New Tab Group");
			AddString(DocumentManagerStringId.CommandNewHorizontalDocumentGroup, "New Horizontal Tab Group");
			AddString(DocumentManagerStringId.CommandNewVerticalDocumentGroup, "New Vertical Tab Group");
			AddString(DocumentManagerStringId.CommandMoveToNextDocumentGroup, "Move to Next Tab Group");
			AddString(DocumentManagerStringId.CommandMoveToPrevDocumentGroup, "Move to Previous Tab Group");
			AddString(DocumentManagerStringId.CommandMoveToMainDocumentGroup, "Move to Main Document Group");
			AddString(DocumentManagerStringId.CommandPinTab, "Pin Tab");
			AddString(DocumentManagerStringId.CommandUnpinTab, "Unpin Tab");
			AddString(DocumentManagerStringId.CommandCloseAllButPinned, "Close All But Pinned");
			AddString(DocumentManagerStringId.CommandVerticalOrientation, "Arrange Tab Groups Vertically");
			AddString(DocumentManagerStringId.CommandHorizontalOrientation, "Arrange Tab Groups Horizontally");
			AddString(DocumentManagerStringId.CommandDockAll, "Dock All");
			AddString(DocumentManagerStringId.CommandCascade, "Cascade Windows");
			AddString(DocumentManagerStringId.CommandTileVertical, "Show Windows Side by Side");
			AddString(DocumentManagerStringId.CommandTileHorizontal, "Show Windows Stacked");
			AddString(DocumentManagerStringId.CommandMinimizeAll, "Minimize Windows");
			AddString(DocumentManagerStringId.CommandArrangeIcons, "Arrange Icons");
			AddString(DocumentManagerStringId.CommandRestoreAll, "Restore Windows");
			AddString(DocumentManagerStringId.CommandBack, "Back");
			AddString(DocumentManagerStringId.CommandHome, "Home");
			AddString(DocumentManagerStringId.CommandExit, "Exit");
			AddString(DocumentManagerStringId.CommandBackDescription, "Navigate backwards");
			AddString(DocumentManagerStringId.CommandHomeDescription, "Navigate to home screen");
			AddString(DocumentManagerStringId.CommandExitDescription, "Exit application");
			AddString(DocumentManagerStringId.CommandMaximize, "Maximize");
			AddString(DocumentManagerStringId.CommandRestore, "Restore");
			AddString(DocumentManagerStringId.CommandTileLargeSize, "Large");
			AddString(DocumentManagerStringId.CommandTileLargeSizeDescription, "Change the tile size to large");
			AddString(DocumentManagerStringId.CommandTileSmallSize, "Small");
			AddString(DocumentManagerStringId.CommandTileSmallSizeDescription, "Change the tile size to small");
			AddString(DocumentManagerStringId.CommandTileWideSize, "Wide");
			AddString(DocumentManagerStringId.CommandTileWideSizeDescription, "Change the tile size to wide");
			AddString(DocumentManagerStringId.CommandTileMediumSize, "Medium");
			AddString(DocumentManagerStringId.CommandTileMediumSizeDescription, "Change the tile size to medium");
			AddString(DocumentManagerStringId.CommandTileSizeFlyoutPanel, "Tile size");
			AddString(DocumentManagerStringId.CommandTileSizeFlyoutPanelDescription, "Change the tile size");
			AddString(DocumentManagerStringId.CommandToggleTileSize, "Toggle size");
			AddString(DocumentManagerStringId.CommandToggleTileSizeDescription, "Toggle tile size");
			AddString(DocumentManagerStringId.CommandClearSelection, "Clear selection");
			AddString(DocumentManagerStringId.CommandClearSelectionDescription, "Clear tiles selection");
			AddString(DocumentManagerStringId.CommandRotate, "Rotate");
			AddString(DocumentManagerStringId.CommandRotateDescription, "Rotate layout");
			AddString(DocumentManagerStringId.CommandFlip, "Flip");
			AddString(DocumentManagerStringId.CommandFlipDescription, "Flip documents");
			AddString(DocumentManagerStringId.CommandDetail, "Detail");
			AddString(DocumentManagerStringId.CommandOverview, "Overview");
			AddString(DocumentManagerStringId.CommandDetailDescription, "Navigate to this document");
			AddString(DocumentManagerStringId.CommandOverviewDescription, "View documents as list");
			AddString(DocumentManagerStringId.CommandEnableFullScreenMode, "Full screen");
			AddString(DocumentManagerStringId.CommandEnableFullScreenModeDescription, "Switch to full screen");
			AddString(DocumentManagerStringId.CommandDisableFullScreenMode, "Disable full screen");
			AddString(DocumentManagerStringId.CommandDisableFullScreenModeDescription, "Switch to normal view");
			AddString(DocumentManagerStringId.RemoveWorkspaceItemCaption, "Remove Workspace");
			AddString(DocumentManagerStringId.CaptureWorkspaceItemCaption, "Capture Workspace");
			AddString(DocumentManagerStringId.LoadWorkspaceItemCaption, "Load Workspace...");
			AddString(DocumentManagerStringId.SaveWorkspaceItemCaption, "Save Workspace");
			AddString(DocumentManagerStringId.SaveCurrentWorkspaceItemCaption, "Save Current Workspace...");
			AddString(DocumentManagerStringId.SaveWorkspaceFormCaption, "Save Workspace");
			AddString(DocumentManagerStringId.LoadWorkspaceFormCaption, "Load Workspace");
			AddString(DocumentManagerStringId.WorkspaceNameFormErrorMessage, "The {0} workspace already exist. Do you want to replace it?");
			AddString(DocumentManagerStringId.WorkspaceNameFormCaption, "Replace Workspace");
			AddString(DocumentManagerStringId.WorkspaceNameWarningMessage, "Workspace name can\'t be empty.");
			AddString(DocumentManagerStringId.WorkspaceRemoveWarningMessage, "Are you sure you want to remove the {0} workspace?");
			AddString(DocumentManagerStringId.WorkspaceRemoveCaption, "Remove Workspace");
			AddString(DocumentManagerStringId.DeferredLoadingExceptionMessage, 
				"Deferred content load was not performed. To provide the content, subscribe to the View's QueryControl event.");
			AddString(DocumentManagerStringId.DuplicateDocumentInTileExceptionMessage,
				"You can only have one Tile at a time that relates to each Document through the Document property. If you want to create multiple Tiles that refer to the same Document, place this Document into a Page container and set this container as an ActivationTarget for these Tiles.");
			AddString(DocumentManagerStringId.NonDocumentModeInitializationExceptionMessage,
				"Non-Document mode initialization was not performed. To enable this mode, specify the DocumentManager's ClientControl property.");
			AddString(DocumentManagerStringId.NullParentContainerExceptionMessage,
				"The DocumentManager must have a parent container. Ensure that one of the following properties is set: MdiParent, ClientControl or ContainerControl.");
			AddString(DocumentManagerStringId.OverlapAllControlsWarning,
			"The DocumentManager occupies all available space and may overlap other controls. To limit the Document Manager's size, use the ContainerControl property to place it into a separate container.");
			AddString(DocumentManagerStringId.LoadingIndicatorCaption, "Please Wait");
			AddString(DocumentManagerStringId.LoadingIndicatorDescription, "Loading...");
			AddString(DocumentManagerStringId.SplashScreenCaption, "Loading...");
			AddString(DocumentManagerStringId.NoPreviewAvailableText, "No Preview Available");
			#endregion AddString
		}
	}
	public class DocumentManagerResXLocalizer : XtraResXLocalizer<DocumentManagerStringId> {
		public DocumentManagerResXLocalizer()
			: base(new DocumentManagerLocalizer()) {
		}
		protected override ResourceManager CreateResourceManagerCore() {
			return new ResourceManager("DevExpress.XtraBars.Docking2010.LocalizationRes", typeof(DocumentManagerResXLocalizer).Assembly);
		}
	}
}
