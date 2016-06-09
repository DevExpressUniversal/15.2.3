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

using System.Windows;
using DevExpress.Xpf.Docking.Base;
using DevExpress.Xpf.Layout.Core;
namespace DevExpress.Xpf.Docking.Platform {
	public class DockLayoutElementHitInfo : LayoutElementHitInfo {
		public DockLayoutElementHitInfo(Point point, BaseLayoutElement element)
			: base(point, element) {
		}
		public bool InCloseButton {
			get { return InControlBox && object.Equals(HitResult, HitTestType.CloseButton); }
		}
		public bool InPinButton {
			get { return InControlBox && object.Equals(HitResult, HitTestType.PinButton); }
		}
		public bool InExpandButton {
			get { return InControlBox && object.Equals(HitResult, HitTestType.ExpandButton); }
		}
		public bool InMaximizeButton {
			get { return InControlBox && object.Equals(HitResult, HitTestType.MaximizeButton); }
		}
		public bool InMinimizeButton {
			get { return InControlBox && object.Equals(HitResult, HitTestType.MinimizeButton); }
		}
		public bool InRestoreButton {
			get { return InControlBox && object.Equals(HitResult, HitTestType.RestoreButton); }
		}
		public bool InDropDownButton {
			get { return InControlBox && object.Equals(HitResult, HitTestType.DropDownButton); }
		}
		public bool InScrollPrevButton {
			get { return InControlBox && object.Equals(HitResult, HitTestType.ScrollPrevButton); }
		}
		public bool InScrollNextButton {
			get { return InControlBox && object.Equals(HitResult, HitTestType.ScrollNextButton); }
		}
		public bool InPageHeaders {
			get { return object.Equals(HitResult, HitTestType.PageHeaders); }
		}
		public bool InHideButton {
			get { return InControlBox && object.Equals(HitResult, HitTestType.HideButton); }
		}
		public bool InCollapseButton {
			get { return InControlBox && object.Equals(HitResult, HitTestType.CollapseButton); }
		}
		protected bool IsCustomization {
			get {
				DockLayoutManager manager = DockLayoutManager.GetDockLayoutManager(((IDockLayoutElement)Element).View);
				return manager != null ? manager.IsCustomization : false;
			}
		}
	}
	public class TabbedPaneElementHitInfo : DockLayoutElementHitInfo {
		public TabbedPaneElementHitInfo(Point point, TabbedPaneElement element)
			: base(point, element) {
		}
		public override bool InDragBounds { get { return false; } }
		public override bool InReorderingBounds { get { return InPageHeaders; } }
	}
	public class TabbedPaneItemElementHitInfo : DockLayoutElementHitInfo {
		public TabbedPaneItemElementHitInfo(Point point, TabbedPaneItemElement element)
			: base(point, element) {
		}
		public override bool InDragBounds { get { return false; } }
		public override bool InReorderingBounds { get { return InBounds; } }
	}
	public class TabbedLayoutGroupElementHitInfo : DockLayoutElementHitInfo {
		public TabbedLayoutGroupElementHitInfo(Point point, TabbedLayoutGroupElement element)
			: base(point, element) {
		}
		public override bool InDragBounds { get { return InBounds; } }
		public override bool InReorderingBounds { get { return InPageHeaders; } }
	}
	public class TabbedLayoutGroupHeaderElementHitInfo : DockLayoutElementHitInfo {
		public TabbedLayoutGroupHeaderElementHitInfo(Point point, TabbedLayoutGroupHeaderElement element)
			: base(point, element) {
		}
		public override bool InDragBounds { get { return false; } }
		public override bool InReorderingBounds { get { return InBounds; } }
	}
	public class DocumentPaneItemElementHitInfo : DockLayoutElementHitInfo {
		public DocumentPaneItemElementHitInfo(Point point, DocumentPaneItemElement element)
			: base(point, element) {
		}
		public override bool InDragBounds { get { return false; } }
		public override bool InReorderingBounds { get { return InBounds; } }
	}
	public class MDIDocumentElementHitInfo : DockLayoutElementHitInfo {
		public MDIDocumentElementHitInfo(Point point, MDIDocumentElement element)
			: base(point, element) {
		}
		public override bool InReorderingBounds { get { 
			return IsDragging ? InBounds : InHeader; } 
		}
	}
	public class DocumentPaneElementHitInfo : DockLayoutElementHitInfo {
		public DocumentPaneElementHitInfo(Point point, DocumentPaneElement element)
			: base(point, element) {
		}
		public override bool InDragBounds { get { return false; } }
		public override bool InReorderingBounds { get { return InPageHeaders; } }
	}
	public class AutoHideTrayElementHitInfo : DockLayoutElementHitInfo {
		public AutoHideTrayElementHitInfo(Point point, AutoHideTrayElement element)
			: base(point, element) {
		}
		public override bool InMenuBounds { get { return InBounds; } }
		public override bool InDragBounds { get { return false; } }
	}
	public class AutoHideTrayHeadersGroupElementHitInfo : DockLayoutElementHitInfo {
		public AutoHideTrayHeadersGroupElementHitInfo(Point point, AutoHideTrayHeadersGroupElement element)
			: base(point, element) {
		}
		public override bool InReorderingBounds { get { return InBounds; } }
	}
	public class AutoHidePaneHeaderItemElementHitInfo : DockLayoutElementHitInfo {
		public AutoHidePaneHeaderItemElementHitInfo(Point point, AutoHidePaneHeaderItemElement element)
			: base(point, element) {
		}
		public override bool InReorderingBounds { get { return IsDragging ? InBounds : InHeader; } }
	}
	public class ControlItemElementHitInfo : DockLayoutElementHitInfo {
		public ControlItemElementHitInfo(Point point, ControlItemElement element)
			: base(point, element) {
		}
		public override bool InDragBounds {
			get { return base.InDragBounds || (IsCustomization && InBounds); }
		}
		public override bool InMenuBounds {
			get { return base.InMenuBounds || (IsCustomization && InBounds); }
		}
	}
	public class GroupPaneElementHitInfo : DockLayoutElementHitInfo {
		public GroupPaneElementHitInfo(Point point, GroupPaneElement element)
			: base(point, element) {
		}
		public override bool InMenuBounds { 
			get { return base.InBounds; } 
		}
		public override bool InDragBounds {
			get { return base.InDragBounds || (IsCustomization && InBounds); }
		}
	}
	public class SplitterElementHitInfo : DockLayoutElementHitInfo {
		public SplitterElementHitInfo(Point pt, SplitterElement element)
			: base(pt, element) {
		}
		public override bool InMenuBounds { 
			get { return InBounds; } 
		}
	}
}
