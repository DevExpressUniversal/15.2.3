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
using System.Collections.ObjectModel;
using System.Collections;
using DevExpress.XtraBars.Docking;
namespace DevExpress.XtraPrinting.Control.Native {
	public class DockPanelHelper {
		public void RestorePanelsVisibility(Collection<DockPanel> panels, DockVisibility visibility) {
			if(panels == null) return;
			for(int i = panels.Count - 1; i >= 0; i--) {
				panels[i].Visibility = visibility;
			}
			panels.Clear();
		}
		public void HidePanels(ICollection allPanels, Collection<DockPanel> panels, DockVisibility visibility) {
			foreach(DockPanel panel in allPanels)
				if(panel.ParentPanel == null && panel.Visibility == visibility) {
					panels.Add(panel);
					panel.Visibility = DockVisibility.Hidden;
				}
		}
	}
	public class PanelHelper : DockPanelHelper {
		Collection<DockPanel> visiblePanels;
		Collection<DockPanel> autoHidePanels;
		public PanelHelper() {
			visiblePanels = new Collection<DockPanel>();
			autoHidePanels = new Collection<DockPanel>();
		}
		~PanelHelper() {
			Dispose();
		}
		public void RestorePanelsVisibility() {
			base.RestorePanelsVisibility(visiblePanels, DockVisibility.Visible);
			base.RestorePanelsVisibility(autoHidePanels, DockVisibility.AutoHide);
		}
		public void HidePanels(ICollection allPanels) {
			base.HidePanels(allPanels, visiblePanels, DockVisibility.Visible);
			base.HidePanels(allPanels, autoHidePanels, DockVisibility.AutoHide);
		}
		protected void Dispose() {
			if(autoHidePanels != null) {
				autoHidePanels.Clear();
				autoHidePanels = null;
			}
			if(visiblePanels != null) {
				visiblePanels.Clear();
				visiblePanels = null;
			}
		}
	}
}
