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
using System.Windows;
using System.Windows.Documents;
using DevExpress.Xpf.Layout.Core;
namespace DevExpress.Xpf.Docking.Platform {
	class TabHeaderInsertHelper : IComparer<double> {
		Rect headerPanelCore;
		Rect tabCore;
		Rect headerCore;
		int insertIndexCore;
		bool IsHorizontal;
		double[] intervals;
		public TabHeaderInsertHelper(IDockLayoutContainer container, Point point, bool canInsertAfterAll) {
			if(container != null && container.HasHeadersPanel) {
				IsHorizontal = container.IsHorizontalHeaders;
				headerPanelCore = MathHelper.Round(container.GetHeadersPanelBounds());
				tabCore = MathHelper.Round(container.GetSelectedPageBounds());
				tabCore = MathHelper.Edge(Tab, HeaderPanel, !IsHorizontal);
				intervals = GetIntervals(container);
				headerCore = GetInsertRect(point, canInsertAfterAll);
			}
		}
		double[] GetIntervals(ILayoutContainer container) {
			List<double> intervals = new List<double>();
			for(int i = 0; i < container.Items.Count; i++) {
				IDockLayoutElement element = container.Items[i] as IDockLayoutElement;
				if(element != null && element.IsPageHeader) {
					Rect r = ElementHelper.GetRect(element);
					intervals.Add(IsHorizontal ? r.Right : r.Bottom);
				}
			}
			intervals.Sort(this);
			return intervals.ToArray();
		}
		public Rect HeaderPanel {
			get { return headerPanelCore; }
		}
		public Rect Tab {
			get { return tabCore; }
		}
		public Rect Header {
			get { return headerCore; }
		}
		public int InsertIndex {
			get { return insertIndexCore; }
		}
		Rect GetInsertRect(Point point, bool canInsertAfterAll) {
			insertIndexCore = -1;
			if(HeaderPanel.Contains(point)) {
				double pos = IsHorizontal ? point.X : point.Y;
				double start = IsHorizontal ? HeaderPanel.Left : HeaderPanel.Top;
				double length = 0;
				for(int i = 0; i < intervals.Length; i++) {
					length = intervals[i] - start;
					if(pos < intervals[i] || (i == intervals.Length - 1 && !canInsertAfterAll)) {
						insertIndexCore = i;
						return GetRect(start, length);
					}
					start += length;
				}
				insertIndexCore = intervals.Length;
				return MathHelper.Round(GetRect(start, length));
			}
			return new Rect(0, 0, 0, 0);
		}
		Rect GetRect(double start, double length) {
			return new Rect(
					IsHorizontal ? start : HeaderPanel.Left, IsHorizontal ? HeaderPanel.Top : start,
					IsHorizontal ? length : HeaderPanel.Width, IsHorizontal ? HeaderPanel.Height : length
				);
		}
		public int Compare(double x, double y) {
			return x.CompareTo(y);
		}
	}
}
