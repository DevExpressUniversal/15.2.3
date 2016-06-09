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
using System.Windows.Input;
namespace DevExpress.Xpf.TreeMap.Native {
	public class ViewController {
		readonly TreeMapControl treeMap;
		readonly ToolTipController toolTipController;
		TreeMapItemPresentation highlightedItemPresentation;
		protected TreeMapControl TreeMap { get { return treeMap; } }
		protected ToolTipInfo ToolTipInfo { get { return TreeMap.ToolTipInfo; } }
		public ViewController(TreeMapControl treeMap) {
			this.treeMap = treeMap;
			toolTipController = new ToolTipController(treeMap);
		}
		void SetHighLightedState(IHitTestableElement hitTestableElement, bool isHighLighted) {
			if (hitTestableElement != null) {
				TreeMapItem item = hitTestableElement.Element as TreeMapItem;
				if (item != null)
					SetHighLighting(item, isHighLighted);
			}
		}
		void SetHighLighting(TreeMapItem item, bool isHighLighted) {
			if (item.Children.Count > 0) {
				foreach (TreeMapItem child in item.Children) {
					SetHighLighting(child, isHighLighted);
				}
			}
			else
				item.IsHighlighted = isHighLighted;
		}
		void UpdateSelection(IHitTestableElement item, ModifierKeys modifiers) {
			if (item != null) {
				ITreeMapItem treeMapItem = item.Element as ITreeMapItem;
				if (treeMapItem != null && TreeMap.SelectionMode != SelectionMode.None)
					TreeMap.UpdateItemSelection(modifiers, treeMapItem);
			}
			else {
				if (modifiers == ModifierKeys.None)
					ClearAllSelectedItems();
			}
		}
		void ClearAllSelectedItems() {
			TreeMap.ClearSelection();
		}
		void UpdateHighLighting(TreeMapItemPresentation currentItemPresentation) {
			if (treeMap.EnableHighlighting && highlightedItemPresentation != currentItemPresentation) {
				SetHighLightedState(highlightedItemPresentation, false);
				SetHighLightedState(currentItemPresentation, true);
			}
			highlightedItemPresentation = currentItemPresentation;
		}
		internal void SetActualItemHighlighting(bool highlighted) {
			SetHighLightedState(highlightedItemPresentation, highlighted);
		}
		internal void OnMouseMove(object sender, MouseEventArgs e) {
			Point hitPoint = e.GetPosition(treeMap);
			UpdateHighLighting(hitPoint);
			toolTipController.UpdateToolTip(hitPoint, ToolTipNavigationAction.MouseMove, highlightedItemPresentation);
		}
		internal void OnMouseLeave(object sender, MouseEventArgs e) {
			if (treeMap.EnableHighlighting)
				SetActualItemHighlighting(false);
			HideToolTip();
			highlightedItemPresentation = null;
		}
		internal void OnLeftButtonUp(object sender, MouseButtonEventArgs e) {
			Point hitPoint = e.GetPosition(treeMap);
			UpdateSelection(highlightedItemPresentation, Keyboard.Modifiers);
			toolTipController.UpdateToolTip(hitPoint, ToolTipNavigationAction.MouseUp, highlightedItemPresentation);
		}
		internal void HideToolTip() {
			toolTipController.UpdateToolTip(new Point(), ToolTipNavigationAction.MouseLeave, null);
		}
		internal void UpdateHighLighting(Point position) {
			TreeMapHitInfo hitInfo = treeMap.CalcHitInfo(position);
			UpdateHighLighting(hitInfo.TreeMapItemPresentation);
		}
#if DEBUGTEST
		internal void UpdateToolTip(Point position, ToolTipNavigationAction navigationAction) {
			TreeMapHitInfo hitInfo = treeMap.CalcHitInfo(position);
			toolTipController.UpdateToolTip(position, navigationAction, hitInfo.TreeMapItemPresentation);
		}
#endif
	}
}
