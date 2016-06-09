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
using DevExpress.Xpf.Scheduler.Internal;
using System.Windows;
using System.Collections.Generic;
using DevExpress.Xpf.Scheduler.Drawing;
using DevExpress.Xpf.Core.Native;
namespace DevExpress.Xpf.Scheduler.Drawing {
	public class TimelineCellPanel : PixelSnappedUniformGrid {
		LoadedUnloadedSubscriber loadSubscriber;
		VisualTreeDependBaseCellsDateHeaderLevelInfo baseCellsInfo;
		public TimelineCellPanel() {
			this.loadSubscriber = new LoadedUnloadedSubscriber(this, OnPanelLoaded, OnPanelUnloaded);
		}
		public VisualTreeDependBaseCellsDateHeaderLevelInfo BaseCellsInfo { get { return baseCellsInfo; } }
		protected override System.Windows.Size ArrangeOverrideCore(System.Windows.Size finalSize) {
			System.Windows.Size resultSize = base.ArrangeOverrideCore(finalSize);
			if (BaseCellsInfo == null) {
				UpdateBaseCellsInfo();
				if (BaseCellsInfo == null)
					return resultSize;
			}			
			List<Size> sizes = new List<Size>();
			foreach (UIElement child in Children)
				sizes.Add(child.RenderSize);
			BaseCellsInfo.UpdateBaseSize(this, sizes);
			return resultSize;
		}
		protected virtual void OnPanelLoaded(FrameworkElement fe) {
			UpdateBaseCellsInfo();
		}
		void UpdateBaseCellsInfo() {
			VisualTreeDependBaseCellsDateHeaderLevelInfo info = VisualTreeDependBaseCellsDateHeaderLevelInfo.GetActive(this);
			if (info == null)
				return;
			this.baseCellsInfo = info;
			BaseCellsInfo.RegisterTimeCellPanel(this);
		}
		protected virtual void OnPanelUnloaded(FrameworkElement fe) {
			BaseCellsInfo.UnregisterTimeCellPanel(this);
			this.baseCellsInfo = null;
		}
	}
}
namespace DevExpress.Xpf.Scheduler.Internal {
	public abstract class BaseCellsDateHeaderLevelInfoBase {
		List<Rect> baseCellRects = new List<Rect>();
		public List<Rect> GetBaseCellsRects() {
			return this.baseCellRects;
		}
		protected void UpdateBaseSizeCore(List<Size> sizes, double totalWidth) {
			double posX = 0;
			this.baseCellRects.Clear();
			int count = sizes.Count;
			for(int i = 0; i < count; i++) {
				this.baseCellRects.Add(new Rect(new Point(posX, 0), sizes[i]));
				double cellWidth = sizes[i].Width;
				posX += cellWidth;
			}
			if(totalWidth > 0 && count > 0) {
				Rect rect = this.baseCellRects[count - 1];
				this.baseCellRects.RemoveAt(count - 1);
				double delta = totalWidth - posX;
				if(rect.Width + delta > 0)
					rect.Width += delta;
				this.baseCellRects.Add(rect);
			}
		}
	}
	public class BaseCellsDateHeaderLevelInfo : BaseCellsDateHeaderLevelInfoBase {
		public void UpdateBaseSize(List<Size> sizes, double snapToWidth) {
			UpdateBaseSizeCore(sizes, snapToWidth);
		}
	}
	public class VisualTreeDependBaseCellsDateHeaderLevelInfo : BaseCellsDateHeaderLevelInfoBase {
		public static VisualTreeDependBaseCellsDateHeaderLevelInfo GetActive(FrameworkElement element) {
			VisualTimelineViewGroupByDate root = LayoutHelper.FindParentObject<VisualTimelineViewGroupByDate>(element);
			if (root == null)
				return null;
			return root.BaseCellsInfo;
		}
		public VisualTreeDependBaseCellsDateHeaderLevelInfo() {
			TimeCellPanels = new List<TimelineCellPanel>();
			HeadersPanels = new List<TimelineLevelHeadersPanel>();
		}
		protected List<TimelineCellPanel> TimeCellPanels { get; private set; }
		protected List<TimelineLevelHeadersPanel> HeadersPanels { get; private set; }
		public void UpdateBaseSize(TimelineCellPanel cellPanel, List<Size> sizes) {
			if (!IsActivePanel(cellPanel))
				return;
			UpdateBaseSizeCore(sizes, -1);
			InvalidateHeadersPanel();
		}
		void InvalidateHeadersPanel() {
			foreach (TimelineLevelHeadersPanel panel in HeadersPanels) 
				panel.InvalidateArrange();
		}
		bool IsActivePanel(TimelineCellPanel cellPanel) {
			return TimeCellPanels.Count != 0 && TimeCellPanels[0] == cellPanel;
		}
		public void RegisterTimeCellPanel(TimelineCellPanel timeCellPanel) {
			if (!TimeCellPanels.Contains(timeCellPanel))
				TimeCellPanels.Add(timeCellPanel);
		}
		public void UnregisterTimeCellPanel(TimelineCellPanel timeCellsControl) {
			if (TimeCellPanels.Contains(timeCellsControl))
				TimeCellPanels.Remove(timeCellsControl);
		}
		public void RegisterTimelineLevelHeadersPanel(TimelineLevelHeadersPanel headersPanel) {
			if (!HeadersPanels.Contains(headersPanel))
				HeadersPanels.Add(headersPanel);
		}
		public void UnregisterTimelineLevelHeadersPanel(TimelineLevelHeadersPanel headersPanel) {
			if (HeadersPanels.Contains(headersPanel))
				HeadersPanels.Remove(headersPanel);
		}
	}
}
