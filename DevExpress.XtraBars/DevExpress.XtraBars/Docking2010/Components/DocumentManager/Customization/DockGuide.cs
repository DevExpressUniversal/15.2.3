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
namespace DevExpress.XtraBars.Docking2010.Customization {
	public enum DockHint {
		None,
		SideLeft,
		SideTop,
		SideRight,
		SideBottom,
		CenterSideLeft,
		CenterSideTop,
		CenterSideRight,
		CenterSideBottom,
		CenterLeft,
		CenterTop,
		CenterRight,
		CenterBottom,
		Center,
		SnapLeft,
		SnapScreen,
		SnapRight,
		SnapBottom,
	}
	public enum DockGuide {
		Left,
		Top,
		Right,
		Bottom,
		Center,
		CenterDock,
		SnapLeft,
		SnapTop,
		SnapRight,
		SnapBottom,
	}
	public sealed class DockGuidesConfiguration {
		IDictionary<DockGuide, bool> Guides;
		IDictionary<DockHint, bool> Hints;
		bool tabHintEnabledCore;
		public DockGuidesConfiguration(DockGuide[] guides, DockHint[] hints) {
			tabHintEnabledCore = true;
			Guides = new Dictionary<DockGuide, bool>();
			for(int i = 0; i < guides.Length; i++) {
				Guides.Add(guides[i], true);
			}
			Hints = new Dictionary<DockHint, bool>();
			for(int i = 0; i < hints.Length; i++) {
				Hints.Add(hints[i], true);
			}
		}
		public bool IsEmpty {
			get { return Guides.Count == 0 && Hints.Count == 0; }
		}
		public bool IsEnabled(DockGuide guide) {
			bool result;
			return Guides.TryGetValue(guide, out result) && result;
		}
		public bool IsEnabled(DockHint hint) {
			bool result;
			return Hints.TryGetValue(hint, out result) && result;
		}
		internal bool IsTabHintEnabled {
			get { return tabHintEnabledCore; }
		}
		public void DisableAllGuides() {
			Guides.Clear();
		}
		public void DisableSnapGuides() {
			Guides.Remove(DockGuide.SnapLeft);
			Guides.Remove(DockGuide.SnapTop);
			Guides.Remove(DockGuide.SnapRight);
			Guides.Remove(DockGuide.SnapBottom);
		}
		public void DisableSideGuides() {
			Guides.Remove(DockGuide.Left);
			Guides.Remove(DockGuide.Top);
			Guides.Remove(DockGuide.Right);
			Guides.Remove(DockGuide.Bottom);
		}
		internal void DisableCenterHints() {
			Hints.Remove(DockHint.CenterLeft);
			Hints.Remove(DockHint.CenterTop);
			Hints.Remove(DockHint.CenterRight);
			Hints.Remove(DockHint.CenterBottom);
		}
		public void DisableSideHints() {
			Hints.Remove(DockHint.CenterSideLeft);
			Hints.Remove(DockHint.CenterSideTop);
			Hints.Remove(DockHint.CenterSideRight);
			Hints.Remove(DockHint.CenterSideBottom);
			Hints.Remove(DockHint.SideLeft);
			Hints.Remove(DockHint.SideTop);
			Hints.Remove(DockHint.SideRight);
			Hints.Remove(DockHint.SideBottom);
		}
		public void Disable(DockGuide guide) {
			Guides[guide] = false;
		}
		public void Disable(DockHint hint) {
			Hints[hint] = false;
		}
		public void DisableTabHint() {
			Hints.Remove(DockHint.Center);
			tabHintEnabledCore = false;
		}
		internal int? tabReorderingIndex;
		public bool IsTabReordering {
			get { return tabReorderingIndex.HasValue; }
		}
		public int GetTabReorderingIndex() {
			return tabReorderingIndex.Value;
		}
		internal void Calc(Docking.VisualizerState state, Docking.VisualizerRole role, Docking.VisualizerVisibilityArgs stateArgs) {
			switch(state) {
				case Docking.VisualizerState.AllHidden:
					DisableAllGuides();
					break;
				case Docking.VisualizerState.CenterVisibleWithoutTabs:
					Disable(DockHint.Center);
					break;
				case Docking.VisualizerState.AllButCenter:
					Disable(DockGuide.Center);
					break;
			}
			if(stateArgs != null) {
				if(role == Docking.VisualizerRole.PanelVisualizer) {
					Disable(DockGuide.Left);
					Disable(DockGuide.Top);
					Disable(DockGuide.Right);
					Disable(DockGuide.Bottom);
					if(!stateArgs.Left)
						Disable(DockHint.CenterLeft);
					if(!stateArgs.Top)
						Disable(DockHint.CenterTop);
					if(!stateArgs.Right)
						Disable(DockHint.CenterRight);
					if(!stateArgs.Bottom)
						Disable(DockHint.CenterBottom);
					if(!stateArgs.Tabbed)
						Disable(DockHint.Center);
					bool allHidden = !stateArgs.Tabbed && !stateArgs.Top && !stateArgs.Left && !stateArgs.Right && !stateArgs.Bottom;
					if(allHidden)
						Disable(DockGuide.Center);
				}
				if(!stateArgs.Left)
					Disable(DockGuide.Left);
				if(!stateArgs.Top)
					Disable(DockGuide.Top);
				if(!stateArgs.Right)
					Disable(DockGuide.Right);
				if(!stateArgs.Bottom)
					Disable(DockGuide.Bottom);
			}
		}
		internal void UpdateVisibilityArgsByCenterDockGuide(Docking.VisualizerVisibilityArgs stateArgs) {
			if(!IsEnabled(DockHint.CenterLeft))
				stateArgs.Left = false;
			if(!IsEnabled(DockHint.CenterTop))
				stateArgs.Top = false;
			if(!IsEnabled(DockHint.CenterRight))
				stateArgs.Right = false;
			if(!IsEnabled(DockHint.CenterBottom))
				stateArgs.Bottom = false;
			if(!IsEnabled(DockGuide.Center) || !IsEnabled(DockHint.Center))
				stateArgs.Tabbed = false;
		}
		internal void UpdateVisibilityArgsBySideDockGuides(Docking.VisualizerVisibilityArgs stateArgs) {
			if(!IsEnabled(DockGuide.Left) || !IsEnabled(DockHint.SideLeft))
				stateArgs.Left = false;
			if(!IsEnabled(DockGuide.Top) || !IsEnabled(DockHint.SideTop))
				stateArgs.Top = false;
			if(!IsEnabled(DockGuide.Right) || !IsEnabled(DockHint.SideRight))
				stateArgs.Right = false;
			if(!IsEnabled(DockGuide.Bottom) || !IsEnabled(DockHint.SideBottom))
				stateArgs.Bottom = false;
		}
		internal void RaiseShowingDockGuides(VS2010StyleDockZoneVisualizer Owner) {
			if((!IsEmpty || IsTabHintEnabled) && Owner.DockManager != null) {
				Owner.DockManager.RaiseShowingDockGuides(VS2010StyleDockZoneVisualizer.OwnerPanel, VS2010StyleDockZoneVisualizer.TargetPanel, this);
			}
		}
	}
}
