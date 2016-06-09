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

using DevExpress.Utils.CodedUISupport;
using System;
using System.Collections.Generic;
using DevExpress.XtraBars.Docking.Helpers;
using DevExpress.XtraBars.Docking;
using System.Drawing;
using DevExpress.XtraBars.Docking2010.Views.Tabbed;
using System.Windows.Forms;
using DevExpress.XtraBars.Docking2010.Views;
using DevExpress.XtraBars.Docking2010;
namespace DevExpress.XtraBars.CodedUISupport {
	public class DockManagerCodedUIHelper {
		static Dictionary<Guid, DockPanelDockInfo> dockingOperationsCore;
		static Dictionary<Guid, DockPanelDockInfo> DockingOperations {
			get {
				if(dockingOperationsCore == null)
					dockingOperationsCore = new Dictionary<Guid, DockPanelDockInfo>();
				return dockingOperationsCore;
			}
		}
		public static void AddDockInfoBeforeStartDock(DockPanel sourcePanel, DockPanel targetPanel, DockingStyle dockStyle, int index) {
			if(DockingOperations.ContainsKey(sourcePanel.ID))
				DockingOperations.Remove(sourcePanel.ID);
			DockPanelDockInfo dockInfo = new DockPanelDockInfo();
			dockInfo.DockingStyleAsString = CodedUIUtils.ConvertToString(dockStyle);
			dockInfo.Index = index;
			if(targetPanel != null) {
				XtraBarsCodedUIHelper.Instance.WriteTargetPanelInfoInDockInfo(targetPanel, ref dockInfo);
			}
			DockingOperations.Add(sourcePanel.ID, dockInfo);
		}
		public static void AddDockInfoAfterEndDock(Guid panelId, DockPanel sourcePanel, DockPanel targetPanel) {
			if(DockingOperations.ContainsKey(panelId)) {
				DockPanelDockInfo dockInfo = DockingOperations[panelId];
				if(targetPanel != null)
					dockInfo.IsTab = targetPanel.Tabbed;
				if(sourcePanel != null)
					dockInfo.FloatLocation = sourcePanel.FloatLocation;
				DockingOperations[panelId] = dockInfo;
			}
		}
		public static void AddDockPanelDocumentDockInfo(BaseDocument document, DocumentGroup group, int insertIndex) {
			if(document is Document && document.IsDockPanel)
				if(document.Control is FloatForm && (document.Control as FloatForm).FloatLayout.Panel != null) {
					DockPanel panel = (document.Control as FloatForm).FloatLayout.Panel;
					if(DockingOperations.ContainsKey(panel.ID))
						DockingOperations.Remove(panel.ID);
					TabbedView view = group.Manager.View as TabbedView;
					if(insertIndex == -1)
						insertIndex = group.Items.Count;
					DockingOperations.Add(panel.ID, GetMdiTabDockInfo(view.Orientation, view.DocumentGroups.IndexOf(group), insertIndex, false));
				}
		}
		public static void AddDockPanelDocumentDockInfo(BaseDocument document, Orientation groupOrientation, int groupIndex) {
			if(document is Document && document.IsDockPanel)
				if(document.Control is FloatForm && (document.Control as FloatForm).FloatLayout.Panel != null) {
					DockPanel panel = (document.Control as FloatForm).FloatLayout.Panel;
					if(DockingOperations.ContainsKey(panel.ID))
						DockingOperations.Remove(panel.ID);
					DockingOperations.Add(panel.ID, GetMdiTabDockInfo(groupOrientation == Orientation.Horizontal ? Orientation.Vertical : Orientation.Horizontal, groupIndex, -1, false));
				}
		}
		public static void AddDockPanelDocumentDockInfo(BaseDocument document, bool isFloating) {
			if(document is Document && document.IsDockPanel)
				if(document.Control is FloatForm && (document.Control as FloatForm).FloatLayout.Panel != null) {
					DockPanel panel = (document.Control as FloatForm).FloatLayout.Panel;
					if(DockingOperations.ContainsKey(panel.ID))
						DockingOperations.Remove(panel.ID);
					DockingOperations.Add(panel.ID, GetMdiTabDockInfo(Orientation.Horizontal, 0, 0, isFloating));
				}
		}
		protected static DockPanelDockInfo GetMdiTabDockInfo(Orientation viewOrientation, int documentGroupIndex, int documentIndex, bool isFloating) {
			DockPanelDockInfo dockInfo = new DockPanelDockInfo();
			dockInfo.IsMdiTab = true;
			dockInfo.MdiTabDockInfo.Manager = DockingManager.DocumentManager;
			if(isFloating)
				dockInfo.MdiTabDockInfo.IsFloating = true;
			else {
				dockInfo.MdiTabDockInfo.Orientation = viewOrientation == Orientation.Horizontal ? OrientationKind.Horizontal : OrientationKind.Vertical;
				dockInfo.MdiTabDockInfo.DocumentGroupIndex = documentGroupIndex;
				dockInfo.MdiTabDockInfo.DocumentIndex = documentIndex;
			}
			return dockInfo;
		}
		public static DockPanelDockInfo GetDockingOperationInfo(Guid panelId) {
			DockPanelDockInfo result = new DockPanelDockInfo();
			if(DockingOperations.ContainsKey(panelId)) {
				result = DockingOperations[panelId];
				DockingOperations.Remove(panelId);
			}
			return result;
		}
	}
}
