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

using DevExpress.Mvvm.Native;
using DevExpress.Xpf.Docking.Platform.Win32;
using DevExpress.Xpf.Layout.Core;
using DevExpress.Xpf.Layout.Core.Platform;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;
namespace DevExpress.Xpf.Docking.Platform {
	public class DockLayoutManagerViewAdapter : ViewAdapter {
		public DockLayoutManagerViewAdapter(DockLayoutManager container) {
			NotificationSource = container;
		}
		DockLayoutManager Container {
			get { return NotificationSource as DockLayoutManager; }
		}
		protected override Layout.Core.IView GetBehindViewOverride(Layout.Core.IView source, System.Windows.Point screenPoint) {
			IView baseView = null;
			LayoutView sourceView = source as LayoutView;
			if(Container.Linked.Count > 0 && sourceView != null) {
				FloatingPaneWindow sourceWindow = (sourceView as FloatingView)
					.With(x => x.RootElement as VisualElements.FloatingWindowPresenter)
					.Return(x => x.Window, () => null);
				var point1 = sourceView.ScreenToClient(screenPoint);
				var point2 = sourceView.RootUIElement.PointToScreen(point1);
				var windows = NativeHelper.SortWindowsTopToBottom(Application.Current.Windows.OfType<Window>());
				var targetWindow = windows.FirstOrDefault(x => {
					return !(x is AdornerWindow) && x != sourceWindow && x.GetScreenBounds().Contains(point2);
				});
				if(targetWindow != null) {
					BehindViewHitTest behindViewHitTest = new BehindViewHitTest();
					behindViewHitTest.HitTest(targetWindow, targetWindow.PointFromScreen(point2));
					FloatingPaneWindow fpw = targetWindow as FloatingPaneWindow;
					var targetWindowManager = fpw.Return(x => x.Manager, () => null);
					foreach(DockLayoutManager potentialTarget in behindViewHitTest.HitObjects) {
						if(potentialTarget == Container) break;
						if(Container.Linked.Contains(potentialTarget) && potentialTarget.GetRealFloatingMode() == Container.GetRealFloatingMode()) {
							if(PresentationSource.FromVisual(potentialTarget) == null) continue;
							if(targetWindowManager == potentialTarget && potentialTarget.IsDescendantOf(sourceView.RootUIElement)) return null; 
							var point3 = potentialTarget.PointFromScreen(point2);
							baseView = potentialTarget.ViewAdapter.GetView(point3);
							if(baseView != null) {
								potentialTarget.LinkedDragService = Container.ViewAdapter.DragService;
								break;
							}
						}
					}
				}
			}
			return baseView ?? base.GetBehindViewOverride(source, screenPoint);
		}
		protected override Point GetBehindViewPointOverride(IView source, IView behindView, Point screenPoint) {
			if(behindView.Adapter != this) {
				var point1 = ((LayoutView)source).ScreenToClient(screenPoint);
				var point2 = ((LayoutView)source).RootUIElement.PointToScreen(point1);
				foreach(var linked in Container.Linked) {
					if(linked != null && linked.ViewAdapter == behindView.Adapter) {
						var point3 = ((LayoutView)behindView).RootUIElement.PointFromScreen(point2);
						return point3;
					}
				}
			}
			return base.GetBehindViewPointOverride(source, behindView, screenPoint);
		}
		class BehindViewHitTest {
			Stack<object> list = new Stack<object>();
			public IEnumerable<object> HitObjects { get { return list; } }
			HitTestResultBehavior HitTestResult(HitTestResult result) {
				return HitTestResultBehavior.Continue;
			}
			HitTestFilterBehavior HitTestFilter(DependencyObject potentialHitTestTarget) {
				var dock = potentialHitTestTarget as DockLayoutManager;
				dock.Do(x => list.Push(x));
				return HitTestFilterBehavior.Continue;
			}
			HitTestFilterBehavior HitTestFilterSkipAdornerLayer(DependencyObject potentialHitTestTarget) {
				if(potentialHitTestTarget is System.Windows.Documents.AdornerLayer) return HitTestFilterBehavior.ContinueSkipSelfAndChildren;
				var dock = potentialHitTestTarget as DockLayoutManager;
				dock.Do(x => list.Push(x));
				return HitTestFilterBehavior.Continue;
			}
			FrameworkElement GetTopLevelAdornerLayer(FrameworkElement reference) {
				var topLevelContainer = DevExpress.Xpf.Core.Native.LayoutHelper.GetTopContainerWithAdornerLayer(reference);
				return topLevelContainer != null ? System.Windows.Documents.AdornerLayer.GetAdornerLayer(topLevelContainer) : null;
			}
			public void HitTest(FrameworkElement reference, Point hitPoint) {
				list.Clear();
				(reference as FloatingPaneWindow).With(x => x.Manager).Do(x => list.Push(x));
				VisualTreeHelper.HitTest(reference, HitTestFilterSkipAdornerLayer, HitTestResult, new PointHitTestParameters(hitPoint));
				var adornerLayer = GetTopLevelAdornerLayer(reference);
				if(adornerLayer != null) {
					VisualTreeHelper.HitTest(adornerLayer, HitTestFilter, HitTestResult, new PointHitTestParameters(hitPoint));
				}
			}
		}
	}
}
