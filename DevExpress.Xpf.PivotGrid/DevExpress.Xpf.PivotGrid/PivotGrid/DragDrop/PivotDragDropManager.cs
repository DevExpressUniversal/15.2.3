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
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Core;
#if SL
using Control = DevExpress.Xpf.Core.WPFCompatibility.SLControl;
#else
using System.Windows.Controls;
#endif
namespace DevExpress.Xpf.PivotGrid.Internal {
	public class PivotDragDropManager {
		List<Window> listenedWindows;
		Dictionary<Control, Window> windowByLists;
		List<Control> externalLists;
		List<Control> externalPopupLists;
		public PivotDragDropManager() {
			listenedWindows = new List<Window>();
			windowByLists = new Dictionary<Control, Window>();
			externalLists = new List<Control>();
			externalPopupLists = new List<Control>();
		}
		public IEnumerable<UIElement> GetTopLevelDropContainers() {
			for(int i = 0; i < externalPopupLists.Count; i++)
				if(externalPopupLists[i].IsLoaded && externalPopupLists[i].IsVisible())
#if !SL
					yield return LayoutHelper.GetTopContainerWithAdornerLayer(externalPopupLists[i]);
#else
					yield return LayoutHelper.GetTopLevelVisual(externalPopupLists[i]) as UIElement;
#endif
			List<Control> outl = new List<Control>();
			outl.AddRange(externalLists);
			outl.Sort(new DragElementsComparer(windowByLists, externalLists));
			for(int i = 0; i < outl.Count; i++)
				if(outl[i].IsLoaded && outl[i].IsVisible())
#if !SL
					yield return LayoutHelper.GetTopContainerWithAdornerLayer(outl[i]);
#else
					yield return LayoutHelper.GetTopLevelVisual(outl[i]) as UIElement;
#endif
		}
		public void UnregisteredFieldListControl(Control fieldList) {
			for(int i = externalLists.Count - 1; i >= 0; i--)
				if(externalLists[i] == fieldList) {
					externalLists.RemoveAt(i);
					UnsubscribeWindowEvents(fieldList);
					break;
				}
			for(int i = externalPopupLists.Count - 1; i >= 0; i--)
				if(externalPopupLists[i] == fieldList) {
					externalPopupLists.RemoveAt(i);
					break;
				}
		}
		public void RegisteredFieldListControl(Control fieldList) {
			for(int i = externalLists.Count - 1; i >= 0; i--)
				if(externalLists[i] == fieldList) {
					SubscribeWindowEvents(externalLists[i]);
					return;
				}
			for(int i = externalPopupLists.Count - 1; i >= 0; i--)
				if(externalPopupLists[i] == fieldList) {
					return;
				}
			FrameworkElement topElement = (FrameworkElement)LayoutHelper.GetTopLevelVisual(fieldList);
			if(topElement.GetType().FullName !=
#if !SL
				"System.Windows.Controls.Primitives.PopupRoot"
#else
				"DevExpress.Xpf.Editors.Popups.EditorPopupBase"   
#endif
				) {
				externalLists.Add(fieldList);
				SubscribeWindowEvents(fieldList);
			} else {
				externalPopupLists.Add(fieldList);
			}
		}
		void SubscribeWindowEvents(Control element) {
#if !SL
			FrameworkElement topElement = (FrameworkElement)LayoutHelper.GetTopLevelVisual(element);
			Window window = Window.GetWindow(topElement);
			Window oldWindow;
			if(windowByLists.TryGetValue(element, out oldWindow)) {
				if(oldWindow == window)
					return;
				else
					UnsubscribeWindowEvents(element);
			}
			if(window == null)
				return;
			windowByLists.Add(element, window);
			if(listenedWindows.Contains(window))
				return;
			listenedWindows.Add(window);
			window.Activated += onWindowActivated;
#endif
		}
		void UnsubscribeWindowEvents(Control element) {
			if(!windowByLists.ContainsKey(element))
				return;
			Window window = windowByLists[element];
			windowByLists.Remove(element);
			if(windowByLists.ContainsValue(window))
				return;
			listenedWindows.Remove(window);
#if !SL
			window.Activated -= onWindowActivated;
#endif
		}
		void onWindowActivated(object sender, EventArgs e) {
			Window window = (Window)sender;
			foreach(KeyValuePair<Control, Window> keyValuePair in windowByLists)
				if(keyValuePair.Value == window) {
					externalLists.Remove(keyValuePair.Key);
					externalLists.Insert(0, keyValuePair.Key);
				}
		}
		public class DragElementsComparer : IComparer<Control> {
			Dictionary<Control, Window> windowByLists;
			List<Control> externalLists;
			public DragElementsComparer(Dictionary<Control, Window> windowByLists, List<Control> externalLists) {
				this.windowByLists = windowByLists;
				this.externalLists = externalLists;
			}
			int IComparer<Control>.Compare(Control x, Control y) {
				Window win1, win2;
				windowByLists.TryGetValue(x, out win1);
				windowByLists.TryGetValue(y, out win2);
#if !SL
				if(win1 != null && win2 != null) {
					if(win1.Owner == win2)
						return -1;
					if(win2.Owner == win1)
						return 1;
				}
#endif
				if(win1 != win2)
					return Comparer<int>.Default.Compare(externalLists.IndexOf(x), externalLists.IndexOf(y));
				int a = -1, b = -1;
				if(x as InnerFieldListControl != null)
					a = 1;
				if(y as InnerFieldListControl != null)
					b = 1;
				if(x as FieldListControlBase != null)
					a = 0;
				if(y as FieldListControlBase != null)
					b = 0;
				return Comparer<int>.Default.Compare(b, a);
			}
		}
	}
}
