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
using System.Linq;
using DevExpress.Xpf.Docking.Platform;
namespace DevExpress.Xpf.Docking {
	[System.ComponentModel.Browsable(false), System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
	public enum DockGuidDisplayMode { Full, DockOnly, TabOnly, DockAndFill, FillOnly }
	[System.ComponentModel.Browsable(false), System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
	public class DockHintsConfiguration {
		class DockHintState {
			private bool _IsEnabled = true;
			public bool IsEnabled {
				get { return _IsEnabled; }
				set { _IsEnabled = value; }
			}
			private bool _IsVisible = true;
			public bool IsVisible {
				get { return _IsVisible; }
				set { _IsVisible = value; }
			}
		}
		Dictionary<object, DockHintState> Hints;
		public bool DisableAll { get; set; }
		public bool HideAll { get; set; }
		public bool ShowSideDockHints { get; set; }
		public bool ShowSelfDockHint { get; set; }
		public DockGuidDisplayMode DisplayMode { get; set; }
		public DockHintsConfiguration() {
			Hints = new Dictionary<object, DockHintState>();
		}
		internal bool GetSideIsEnabled(DockVisualizerElement element) {
			return GetIsEnabled(element.ToSideDockHint());
		}
		internal bool GetSideIsVisible(DockVisualizerElement element) {
			return GetIsVisible(element.ToSideDockHint());
		}
		internal bool GetAutoHideIsEnabled(DockVisualizerElement element) {
			return GetIsEnabled(element.ToAutoHideDockHint());
		}
		internal bool GetAutoHideIsVisible(DockVisualizerElement element) {
			return GetIsVisible(element.ToAutoHideDockHint());
		}
		internal bool GetGuideIsVisible(DockVisualizerElement element) {
			return GetIsVisible(element.ToDockGuide());
		}
		public void Hide(object hint) {
			DockHintState result = GetDockHintState(hint);
			result.IsVisible = false;
		}
		public void Disable(object hint) {
			DockHintState result = GetDockHintState(hint);
			result.IsEnabled = false;
		}
		public bool GetIsVisible(object hint) {
			DockHintState result = GetDockHintState(hint);
			return result.IsVisible && !HideAll;
		}
		public bool GetIsEnabled(object hint) {
			DockHintState result = GetDockHintState(hint);
			return result.IsEnabled && !DisableAll;
		}
		DockHintState GetDockHintState(object hint) {
			DockHintState result;
			if(!Hints.TryGetValue(hint, out result)) {
				result = new DockHintState();
				Hints[hint] = result;
			}
			return result;
		}
		public void SetCanHide(bool canHide) {
			GetDockHintState(DockHint.AutoHideBottom).IsEnabled = GetDockHintState(DockHint.AutoHideLeft).IsEnabled =
				GetDockHintState(DockHint.AutoHideRight).IsEnabled = GetDockHintState(DockHint.AutoHideTop).IsEnabled = canHide;
		}
		internal void SetCanDockCenter(bool canDock) {
			GetDockHintState(DockHint.CenterLeft).IsEnabled = GetDockHintState(DockHint.CenterRight).IsEnabled =
				GetDockHintState(DockHint.CenterTop).IsEnabled = GetDockHintState(DockHint.CenterBottom).IsEnabled
				= canDock;
		}
		internal void SetCanDockToSide(bool canDock) {
			GetDockHintState(DockHint.SideLeft).IsEnabled = GetDockHintState(DockHint.SideRight).IsEnabled =
				GetDockHintState(DockHint.SideTop).IsEnabled = GetDockHintState(DockHint.SideBottom).IsEnabled
				= canDock;
		}
		internal void SetCanDockToTab(bool canDock) {
			GetDockHintState(DockHint.TabBottom).IsEnabled = GetDockHintState(DockHint.TabLeft).IsEnabled =
				GetDockHintState(DockHint.TabRight).IsEnabled = GetDockHintState(DockHint.TabTop).IsEnabled
				= canDock;
		}
		internal void SetCanFill(bool canFill) {
			DockHintState dockHintState = GetDockHintState(DockHint.Center);
			dockHintState.IsVisible = dockHintState.IsEnabled = canFill;
		}
		internal void SetSideGuidsVisibility(bool isVisible) {
			GetDockHintState(DockGuide.Left).IsVisible = GetDockHintState(DockGuide.Right).IsVisible =
				GetDockHintState(DockGuide.Top).IsVisible = GetDockHintState(DockGuide.Bottom).IsVisible = isVisible;
		}
		internal void SetCenterGuidVisibility(bool isVisible) {
			GetDockHintState(DockGuide.Center).IsVisible = isVisible;
		}
		public void Invalidate() {
			Hints.Clear();
			DisableAll = false;
			HideAll = false;
			ShowSideDockHints = false;
			ShowSelfDockHint = false;
		}
		public void SetConfiguration(DockLayoutManager manager, DockLayoutElementDragInfo dragInfo) {
			if(manager == null) return;
			ShowSideDockHints = dragInfo.View.Type == DevExpress.Xpf.Layout.Core.HostType.Layout;
			ShowSelfDockHint = dragInfo.AcceptSelfDock();
			bool canDockCenter = dragInfo.AcceptDockCenter();
			bool canFill = dragInfo.AcceptFill();
			bool canDock = dragInfo.CanDock;
			bool canDockToTab = dragInfo.CanDockToTab;
			bool canDockToSide = dragInfo.CanDockToSide;
			bool canHide = dragInfo.CanHide;
			SetCanHide(canHide);
			SetCanDockCenter(canDock);
			SetCanDockToTab(canDockToTab);
			SetCanDockToSide(canDockToSide);
			SetCanFill(canFill);
			SetSideGuidsVisibility(ShowSideDockHints && (canDockToSide || canHide));
			SetCenterGuidVisibility(ShowSelfDockHint && canDockCenter && (canFill || canDock || canDockToTab));
			BaseLayoutItem dragInfoTarget = dragInfo.Target;
			if(dragInfoTarget.GetIsDocumentHost()) {
				if(manager.DockingStyle == DockingStyle.VS2010)
					DisplayMode = DockGuidDisplayMode.Full;
				else {
					DisplayMode = DockGuidDisplayMode.DockOnly;
				}
				BaseLayoutItem dragInfoItem = dragInfo.Item;
				if(dragInfoItem.ItemType == Layout.Core.LayoutItemType.Document)
					DisplayMode = DockGuidDisplayMode.TabOnly;
				if(dragInfoItem is FloatGroup && ((FloatGroup)dragInfoItem).IsDocumentHost)
					DisplayMode = DockGuidDisplayMode.TabOnly;
				LayoutGroup documentHost = dragInfoTarget.Parent ?? (LayoutGroup)dragInfoTarget;
				int count = documentHost.Items.Count(item => item is DocumentGroup && ((DocumentGroup)item).Items.Count > 0);
				if(count > 1) {
					bool fHorz = documentHost.Orientation == System.Windows.Controls.Orientation.Horizontal;
					if(fHorz) {
						Disable(DockHint.TabTop);
						Disable(DockHint.TabBottom);
						if(DockControllerHelper.GetNextNotEmptyDocumentGroup(dragInfoTarget) != null) {
							Disable(DockHint.CenterRight);
						}
						if(DockControllerHelper.GetPreviousNotEmptyDocumentGroup(dragInfoTarget) != null) {
							Disable(DockHint.CenterLeft);
						}
					}
					else {
						Disable(DockHint.TabLeft);
						Disable(DockHint.TabRight);
						if(DockControllerHelper.GetNextNotEmptyDocumentGroup(dragInfoTarget) != null) {
							Disable(DockHint.CenterBottom);
						}
						if(DockControllerHelper.GetPreviousNotEmptyDocumentGroup(dragInfoTarget) != null) {
							Disable(DockHint.CenterTop);
						}
					}
				}
			}
			else DisplayMode = DockGuidDisplayMode.DockOnly;
		}
	}
}
