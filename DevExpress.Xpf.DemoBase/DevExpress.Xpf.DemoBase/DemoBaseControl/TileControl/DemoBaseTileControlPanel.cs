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

using DevExpress.Xpf.Core.Native;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
namespace DevExpress.Xpf.DemoBase {
	class DemoBaseTileControlPanel : Panel {
		bool isSimple = true;
		bool isSimpleSet = false;
		bool IsSimple {
			get {
				if(!isSimpleSet) {
					var tileControl = LayoutHelper.FindParentObject<DemoBaseTileControl>(this);
					isSimple = tileControl.IsSimpleGroupsLayout;
					isSimpleSet = true;
				}
				return isSimple;
			}
		}
		readonly double interGroupHorisontalSpace = 30;
		readonly double interGroupVerticalSpace = 27;
		protected override Size MeasureOverride(Size availableSize) {
			foreach(UIElement child in Children) {
				child.Measure(availableSize);
			}
			double x = 0;
			var children = Children.Cast<UIElement>().ToList();
			while(children.Count > 0) {
				List<UIElement> set = TakePillar(children, availableSize.Height);
				x += set.Max(e => e.DesiredSize.Width) + interGroupHorisontalSpace;
			}
			return new Size(x, availableSize.Height);
		}
		protected override Size ArrangeOverride(Size finalSize) {
			double x = 0;
			var children = Children.Cast<UIElement>().ToList();
			double finalHeight = 0;
			while(children.Count > 0) {
				List<UIElement> set = TakePillar(children, finalSize.Height);
				SpreadArrange(set, finalSize.Height, x);
				x += set.Max(e => e.DesiredSize.Width) + interGroupHorisontalSpace;
				finalHeight = Math.Max(finalHeight, set.Sum(e => e.DesiredSize.Height) + (set.Count - 1) * interGroupVerticalSpace);
			}
			return new Size(x, finalHeight);
		}
		private void SpreadArrange(List<UIElement> pillar, double height, double x) {
			double y = 0;
			foreach(FrameworkElement child in pillar) {
				Point curPos = PanelHelper.GetCurrentPosition(child, this);
				var newPos = new Point(x, y);
				child.Arrange(new Rect(curPos, child.DesiredSize));
				PanelHelper.Animate(child, curPos, newPos);
				y += child.DesiredSize.Height + interGroupVerticalSpace;
			}
		}
		private List<UIElement> TakePillar(List<UIElement> children, double height) {
			var pillar = new List<UIElement>();
			while(children.Count > 0) {
				UIElement child = children[0];
				if(height - child.DesiredSize.Height < 0 && pillar.Count != 0) {
					break;
				}
				height -= child.DesiredSize.Height + interGroupVerticalSpace;
				pillar.Add(child);
				children.Remove(child);
				if(IsSimple)
					break;
			}
			return pillar;
		}
	}
}
