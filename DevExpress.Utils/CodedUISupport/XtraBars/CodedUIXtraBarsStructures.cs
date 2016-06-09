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
using System.Drawing;
namespace DevExpress.Utils.CodedUISupport {
	[Serializable]
	public struct RibbonElementSearchInfo {
		public RibbonElements ElementType;
		public string PageCategory;
		public string Page;
		public string PageGroup;
		public string Item;
		public string OwnerItem;
		public string GroupCaptionButton;
		public string GalleryGroup;
		public string GalleryItem;
		public string FormCaptionButtonKind;
		public bool InToolbar;
	}
	public enum AlertFormElements {
		Default = 0,
		AlertButton,
		AlertText,
		AlertCaption,
		AlertCloseButton,
		AlertPinButton,
		AlertPopupMenuButton
	}
	public enum RibbonElements {
		Unknown,
		Ribbon,
		PageCategory,
		Page,
		PageGroup,
		Item,
		EditItem,
		BaseButtonItem,
		ItemDrop,
		GalleryItem,
		GroupCaptionButton,
		GalleryGroup,
		Gallery,
		GalleryDownButton,
		GalleryUpButton,
		GalleryDropDownButton,
		QuickAccessToolbar,
		PageHeader,
		FormCaption,
		FormCaptionButton,
		ApplicationButton,
		PanelRightScroll,
		PanelLeftScroll
	}
	public enum BarElementTypes : int {
		Default = 0,
		BarBaseButtonItemLink,
		BarCheckItemLink,
		BarEditItemLink,
		BarItemLink,
		DragBorder,
		BarItemOpenArrow
	}
	public enum XtraBarsPropertyNames {
		EditValue,
		Checked,
		Enabled,
		Text,
		Selected
	}
	[Serializable]
	public struct MdiClientElementInfo {
		public MdiClientElementType ElementType;
		public string DocumentName;
		public string ButtonName;
		public TabPanelButtonType ButtonType;
		public int DocumentGroupIndex;
		public int MdiClientHandle;
		public Guid DockPanelId;
		public DockingManager Manager;
	}
	public enum MdiClientElementType : int {
		Unknown = 0,
		Document,
		DocumentCloseButton,
		TabPanelButton,
		DocumentGroup,
	}
	public enum TabPanelButtonType {
		Custom,
		Close,
		Prev,
		Next,
		DropDown
	}
	[Serializable]
	public struct MdiClientDocumentDockInfo {
		public int DocumentGroupIndex;
		public int DocumentIndex;
		public bool IsFloating;
		public OrientationKind Orientation;
		public DockingManager Manager;
	}
	public enum DockingManager {
		Undefined,
		XtraTabbedMdiManager,
		DocumentManager
	}
	[Serializable]
	public struct DockPanelDockInfo {
		public string DockingStyleAsString;
		public bool KeepCurrentParent;
		public string TargetPanelName;
		public string TargetPanelLastChildName;
		public string TargetPanelLayoutAsString;
		public string LayoutAsString;
		public int TargetPanelLastChildHierarchyLevel;
		public Point FloatLocation;
		public int Index;
		public bool IsTab;
		public bool IsMdiTab;
		public MdiClientDocumentDockInfo MdiTabDockInfo;
	}
	[Serializable]
	public struct DockPanelElementInfo {
		public bool IsButton;
		public bool IsTabButton;
		public bool IsResizeZone;
		public string ButtonName;
		public string ResizeZoneSide;
		public Guid TabButtonPanelId;
	}
	public enum OrientationKind : int {
		Undefined,
		Vertical,
		Horizontal
	}
	public enum DocumentContainerButtonType {
		Undefined,
		Close,
		Maximize,
		Pin
	}
	[Serializable]
	public struct AccordionControlElementInfo {
		public AccordionControlElements ElementType;
		public string GroupName;
		public string ItemName;
	}
	public enum AccordionControlElements : int {
		Unknown,
		Group,
		Item
	}
}
