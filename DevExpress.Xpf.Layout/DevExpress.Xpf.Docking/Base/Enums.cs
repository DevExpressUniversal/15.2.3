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

namespace DevExpress.Xpf.Docking {
	public enum AutoHideType {
		Default, Left, Top, Right, Bottom
	}
	public enum CaptionLocation {
		Default, Left, Top, Right, Bottom
	}
	public enum ImageLocation {
		Default, BeforeText, AfterText
	}
	public enum CaptionAlignMode {
		Default,
		AlignInGroup,
		AutoSize,
		Custom
	}
	public enum GroupBorderStyle {
		NoBorder,
		Group,
		GroupBox,
		Tabbed
	}
	public enum FloatingMode {
		Window = 0,
		Desktop = 1
	}
	public enum FixedItemStyle {
		EmptySpace,
		Label,
		Separator
	}
	public enum DockLayoutManagerParameter {
		DockingItemIntervalHorz, DockingItemIntervalVert, DockingRootMargin,
		LayoutItemIntervalHorz, LayoutItemIntervalVert,
		LayoutGroupIntervalHorz, LayoutGroupIntervalVert, LayoutRootMargin,
		CaptionToControlDistanceLeft, CaptionToControlDistanceTop, CaptionToControlDistanceRight, CaptionToControlDistanceBottom,
		LayoutPanelCaptionFormat, LayoutControlItemCaptionFormat, LayoutGroupCaptionFormat, TabCaptionFormat,
		WindowTitleFormat,
		AutoHidePanelsFitToContainer,
	}
	public enum ClosePageButtonShowMode {
		Default,
		InTabControlHeader,
		InAllTabPageHeaders,
		InActiveTabPageHeader,
		InAllTabPagesAndTabControlHeader,
		InActiveTabPageAndTabControlHeader,
		NoWhere
	}
	public enum MDIStyle {
		Default,
		Tabbed,
		MDI
	}
	public enum MDIState {
		Normal,
		Minimized,
		Maximized
	}
	public enum FloatGroupBorderStyle {
		Single, Form, Empty
	}
	public enum DockHint {
		AutoHideLeft,
		AutoHideTop,
		AutoHideRight,
		AutoHideBottom,
		SideLeft,
		SideTop,
		SideRight,
		SideBottom,
		CenterLeft,
		CenterTop,
		CenterRight,
		CenterBottom,
		Center,
		TabLeft,
		TabRight,
		TabTop,
		TabBottom,
		TabHeader
	}
	public enum DockGuide {
		Left,
		Top,
		Right,
		Bottom,
		Center,
	}
	public enum ClosingBehavior {
		Default,
		HideToClosedPanelsCollection,
		ImmediatelyRemove
	}
	public enum DockingStyle {
		Default,
		VS2010,
	}
	public enum DockOperation {
		Close,
		Dock,
		Float,
		Hide,
		Restore,
		Reorder,
		Resize,
		Move
	}
	public enum DockItemState {
		Undefined,
		AutoHidden,
		Closed,
		Docked,
		Floating,
	}
}
namespace DevExpress.Xpf.Docking.Base {
	public enum HitTestType {
		Undefined,
		Bounds,
		Header,
		Content,
		Border,
		ControlBox,
		CloseButton,
		PinButton,
		ExpandButton,
		MinimizeButton,
		MaximizeButton,
		RestoreButton,
		Label,
		PageHeaders,
		ScrollPrevButton,
		ScrollNextButton,
		DropDownButton,
		ShowButton,
		HideButton,
		CollapseButton
	}
	public enum ClosedPanelsBarVisibility {
		Default,
		Manual,
		Auto,
		Never
	}
	public enum AutoHideExpandMode {
		Default,
		MouseHover,
		MouseDown,
	}
	public enum AutoHideMode {
		Default,
		Inline,
		Overlay,
	}
	public enum AutoHideExpandState {
		Hidden,
		Visible,
		Expanded,
	}
	public enum FloatingDocumentContainer { Default, SingleDocument, DocumentHost }
}
