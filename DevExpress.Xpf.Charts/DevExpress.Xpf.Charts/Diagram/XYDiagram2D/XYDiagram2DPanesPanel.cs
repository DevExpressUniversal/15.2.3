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
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Charts.Native;
namespace DevExpress.Xpf.Charts {
	public class XYDiagram2DPanesPanel : Panel {
		readonly Dictionary<Pane, Point> locations = new Dictionary<Pane, Point>();
		public Orientation Orientation { 
			get {
				XYDiagram2D diagram = LayoutHelper.FindParentObject<XYDiagram2D>(this);
				return diagram != null ? diagram.PaneOrientation : Orientation.Vertical; } 
		}
		public XYDiagram2DPanesPanel() {
		}
		protected override Size MeasureOverride(Size constraint) {
			double defaultSize = ChartControl.DefaultConstraint;
			constraint = new Size(MathUtils.ConvertInfinityToDefault(constraint.Width, defaultSize), MathUtils.ConvertInfinityToDefault(constraint.Height, defaultSize));
			locations.Clear();
			UIElementCollection items = Children;
			int count = items.Count;
			if (count == 0)
				return base.MeasureOverride(constraint);
			Rect[] paneRects = new Rect[count];
			int lastIndex = count - 1;
			double width = constraint.Width;
			double height = constraint.Height;
			if (Orientation == Orientation.Horizontal) {
				int location = 0;
				int size = (int)Math.Floor(width) / count;
				for (int i = 0; i < lastIndex; i++) {
					paneRects[i] = new Rect(location, 0, size, height);
					location += size;
				}
				paneRects[lastIndex] = new Rect(location, 0, width - lastIndex * size, height);
			}
			else {
				int location = 0;
				int size = (int)Math.Floor(height) / count;
				for (int i = 0; i < lastIndex; i++) {
					paneRects[i] = new Rect(0, location, width, size);
					location += size;
				}
				paneRects[lastIndex] = new Rect(0, location, width, height - lastIndex * size);
			}
			for (int index = 0; index < count; index++) {
				Pane pane = items[index] as Pane;
				if (pane != null) {
					Rect rect = paneRects[index];
					locations.Add(pane, new Point(rect.X, rect.Y));
					pane.Measure(new Size(rect.Width, rect.Height));
				}
			}
			return constraint;
		}
		protected override Size ArrangeOverride(Size arrangeBounds) {
			foreach (Pane pane in Children)
				pane.Arrange(new Rect(locations[pane], pane.DesiredSize));
			return arrangeBounds;
		}
	}
}
