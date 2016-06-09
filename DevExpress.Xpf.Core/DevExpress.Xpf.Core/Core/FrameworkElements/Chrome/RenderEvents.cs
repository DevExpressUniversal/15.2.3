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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;
namespace DevExpress.Xpf.Core.Native {
	[Flags]
	public enum RenderEvents {
		PreviewMouseUp = 0x1,
		PreviewMouseDown = 0x2,
		MouseDown = 0x4,
		MouseUp = 0x8,
		MouseEnter = 0x10,
		MouseLeave = 0x20,
		MouseMove = 0x40,		
	}
	[Flags]
	enum RenderEventKinds {
		Mouse =
			RenderEvents.PreviewMouseUp |
			RenderEvents.PreviewMouseDown |
			RenderEvents.MouseDown |
			RenderEvents.MouseUp |
			RenderEvents.MouseEnter |
			RenderEvents.MouseLeave |
			RenderEvents.MouseMove,		
	}
	public delegate void RenderEventHandler(IFrameworkRenderElementContext sender, RenderEventArgsBase args);
	public class RenderEventArgsBase : EventArgs {
		protected RenderEventArgsBase(IFrameworkRenderElementContext source, EventArgs originalEventArgs) {
			this.Source = source;
			this.OriginalEventArgs = originalEventArgs;
		}
		public bool Handled { get; set; }
		public IFrameworkRenderElementContext Source { get; private set; }
		public EventArgs OriginalEventArgs { get; private set; }
		protected internal virtual void InvokeEventHandler(IFrameworkRenderElementContext target) { }
	}
	public class RenderEventArgs : RenderEventArgsBase {
		public RenderEvents RenderEvent { get; private set; }
		public RenderEventArgs(IFrameworkRenderElementContext source, EventArgs originalEventArgs, RenderEvents renderEvent)
			: base(source, originalEventArgs) {			
			this.RenderEvent = renderEvent;
		}
		protected internal override void InvokeEventHandler(IFrameworkRenderElementContext target) {		   
		}
	}
	public class MouseRenderEventArgs : RenderEventArgs {
		public Point Position { get; private set; }
		public MouseRenderEventArgs(IFrameworkRenderElementContext source, EventArgs originalEventArgs, RenderEvents renderEvent, Point position)
			: base(source, originalEventArgs, renderEvent) {
				if (((int)RenderEventKinds.Mouse & (int)renderEvent) == 0)
					throw new ArgumentException("renderEvent");
				this.Position = position;
		}
		protected internal override void InvokeEventHandler(IFrameworkRenderElementContext target) {
			base.InvokeEventHandler(target);
			switch (RenderEvent) {
				case RenderEvents.MouseMove:
				target.OnMouseMove(this);
				return;
				case RenderEvents.MouseEnter:
				target.OnMouseEnter(this);
				return;
				case RenderEvents.MouseLeave:
				target.OnMouseLeave(this);
				return;
				case RenderEvents.MouseDown:
				target.OnMouseDown(this);
				return;
				case RenderEvents.MouseUp:
				target.OnMouseUp(this);
				return;
				case RenderEvents.PreviewMouseDown:
				target.OnPreviewMouseDown(this);
				return;
				case RenderEvents.PreviewMouseUp:
				target.OnPreviewMouseUp(this);
				return;
			}
		}
	}
	public static class RenderEventManager {
#if DEBUGTEST
		static Point? DEBUGTEST_MousePosition;
		internal static void DEBUGTEST_SetMousePosition(Point p) {
			DEBUGTEST_MousePosition = p;
		}
		internal static void DEBUGTEST_ResetMousePosition() {
			DEBUGTEST_MousePosition = null;
		}
#endif
		public static void PropagateEvent(this IChrome chrome, RoutedEventArgs args, RenderEvents renderEvent) {
			if (chrome.Root == null)
				return;
			if (((int)RenderEventKinds.Mouse & (int)renderEvent) != 0) {
				PropagateMouseEvent(chrome, (MouseEventArgs)args, renderEvent);
			}
			IEnumerable<FrameworkRenderElementContext> route = RenderTreeHelper.RenderDescendants(chrome.Root);
			var renderArgs = new RenderEventArgs(chrome.Root, args, renderEvent);
			if (args.RoutedEvent.RoutingStrategy == RoutingStrategy.Bubble)
				route = route.Reverse();
			RaiseEvent(renderArgs, route);
		}
		static void PropagateMouseEvent(IChrome chrome, MouseEventArgs args, RenderEvents renderEvent) {
			FrameworkRenderElementContext[] route = null;
			FrameworkRenderElementContext[] mouseEnterRoute = null;
			FrameworkRenderElementContext[] mouseLeaveRoute = null;
			var source = chrome.Root;
			FrameworkElement feChrome = (FrameworkElement)chrome;
			Point mousePosition = ((MouseEventArgs)args).GetPosition(feChrome);
			bool isMouseOver = feChrome.IsMouseOver;
#if DEBUGTEST
			if (DEBUGTEST_MousePosition.HasValue) {
				mousePosition = DEBUGTEST_MousePosition.Value;
				isMouseOver = true;
			}				
#endif
			route = GetDescendantsUnderMouse(source, mousePosition, out mouseEnterRoute, out mouseLeaveRoute, isMouseOver);
			if (args.RoutedEvent.RoutingStrategy == RoutingStrategy.Bubble)
				route = route.Reverse().ToArray();
			if (mouseEnterRoute != null) {
				RaiseEvent(new MouseRenderEventArgs(source, args, RenderEvents.MouseEnter, mousePosition), mouseEnterRoute);
			}
			if (mouseLeaveRoute != null) {
				RaiseEvent(new MouseRenderEventArgs(source, args, RenderEvents.MouseLeave, mousePosition), mouseLeaveRoute);
			}
			if (renderEvent != RenderEvents.MouseLeave && renderEvent != RenderEvents.MouseEnter)
				RaiseEvent(new MouseRenderEventArgs(source, args, renderEvent, mousePosition), route);
		}
		static FrameworkRenderElementContext[] GetDescendantsUnderMouse(FrameworkRenderElementContext context, Point relativePoint, out FrameworkRenderElementContext[] addedElements, out FrameworkRenderElementContext[] removedElements, bool actualIsMouseOver) {
			List<FrameworkRenderElementContext> result = new List<FrameworkRenderElementContext>();
			if (actualIsMouseOver)
				RenderTreeHelper.HitTest(context, frec => RenderHitTestFilterBehavior.Continue, frec => { result.Add(frec); return RenderHitTestResultBehavior.Continue; }, relativePoint);			
			addedElements = result.Where(x => x.IsMouseOverCore == false).ToArray();
			removedElements = RenderTreeHelper.RenderDescendants(context).Concat(new FrameworkRenderElementContext[] { context }).Except(result).Where(x => x.IsMouseOverCore == true).ToArray();
			return result.ToArray();
		}
		public static void RaiseEvent(RenderEventArgsBase args, IEnumerable<IFrameworkRenderElementContext> eventRoute) {
			if (args.Handled)
				return;
			foreach (var element in eventRoute) {
				args.InvokeEventHandler(element);
				if (args.Handled && args.With(x => x.OriginalEventArgs as RoutedEventArgs).With(x => x.RoutedEvent).If(x => x.RoutingStrategy == RoutingStrategy.Direct) == null) break;
			}
			if (args.Handled)
				(args.OriginalEventArgs as RoutedEventArgs).Do(x => x.Handled = true);
		}
	}
}
