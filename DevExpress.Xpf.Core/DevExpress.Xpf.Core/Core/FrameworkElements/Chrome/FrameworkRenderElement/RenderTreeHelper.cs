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
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;
namespace DevExpress.Xpf.Core.Native {
	[Flags]
	public enum RenderHitTestFilterBehavior {
		Continue = 0x1,
		ContinueSkipChildren = Continue | 0x2,
		ContinueSkipSelf = Continue | 0x4,
		ContinueSkipSelfAndChildren = ContinueSkipChildren | ContinueSkipSelf,
		Stop = 0x8
	}
	public enum RenderHitTestResultBehavior {
		Stop,
		Continue,
	}
	public static class RenderTreeHelper {
		public static FrameworkRenderElementContext HitTest(FrameworkRenderElementContext context, Point relativePoint) {
			FrameworkRenderElementContext result = null;
			HitTest(context,
				frec => RenderHitTestFilterBehavior.Continue,
				frec => { result = frec; return RenderHitTestResultBehavior.Continue; },
				relativePoint);
			return result;
		}
		public static FrameworkRenderElementContext HitTest(FrameworkRenderElementContext context, Point relativePoint, Func<FrameworkRenderElementContext, bool> predicate) {
			FrameworkRenderElementContext result = null;
			HitTest(context,
				frec => RenderHitTestFilterBehavior.Continue,
				frec => { if (predicate(frec)) { result = frec; return RenderHitTestResultBehavior.Stop; } return RenderHitTestResultBehavior.Continue; },
				relativePoint);
			return result;
		}
		public static bool GetIsVisible(FrameworkRenderElementContext context) {
			if (context == null)
				throw new ArgumentNullException("context");
			return context.ElementHost.Parent.IsVisible && GetIsVisibleImpl(context);
		}
		static bool GetIsVisibleImpl(FrameworkRenderElementContext context) {
			if (context == null)
				return true;
			var selfVisibility = context.Visibility.HasValue ? context.Visibility.Value : context.Factory.Visibility;
			if (selfVisibility != Visibility.Visible)
				return false;
			return GetIsVisibleImpl(context.Parent);
		}
		public static void HitTest(FrameworkRenderElementContext context,
									Func<FrameworkRenderElementContext, RenderHitTestFilterBehavior> filterCallback,
									Func<FrameworkRenderElementContext, RenderHitTestResultBehavior> resultCallback,
									Point relativePoint) {
			List<FrameworkRenderElementContext> contexts = new List<FrameworkRenderElementContext>();
			List<FrameworkRenderElementContext> results = new List<FrameworkRenderElementContext>();
			var rootTransform = RenderTransformValue(RenderAncestors(context).LastOrDefault() ?? context);
			rootTransform.Invert();
			relativePoint = rootTransform.Transform(relativePoint);
			contexts.Add(context);
			while (contexts.Count > 0) {
				var current = contexts[0];
				contexts.RemoveAt(0);
				var hitTestResult = current.HitTest(GetMatrixTransformToDescendant(context, current).Transform(relativePoint));				
				var filter = filterCallback(current);
				if (!filter.HasFlag(RenderHitTestFilterBehavior.Continue))
					return;
				if (!filter.HasFlag(RenderHitTestFilterBehavior.ContinueSkipSelf) && hitTestResult)
					results.Add(current);
				if (!filter.HasFlag(RenderHitTestFilterBehavior.ContinueSkipChildren))
					contexts.InsertRange(0, RenderChildren(current));
			}
			foreach (var result in results)
				if (resultCallback(result) == RenderHitTestResultBehavior.Stop)
					return;
		}
		public static Transform TransformToAncestor(FrameworkRenderElementContext element, FrameworkRenderElementContext ancestor) {
			return new MatrixTransform(GetMatrixTransformToAncestor(element, ancestor));
		}
		public static Transform TransformToDescendant(FrameworkRenderElementContext element, FrameworkRenderElementContext descendant) {
			return new MatrixTransform(GetMatrixTransformToDescendant(element, descendant));
		}
		public static Transform TransformToRoot(FrameworkRenderElementContext element) {
			var root = RenderAncestors(element).LastOrDefault() ?? element;
			return new MatrixTransform(GetMatrixTransformToAncestor(element, root) * RenderTransformValue(root));
		}		
		static Matrix GetMatrixTransformToAncestor(FrameworkRenderElementContext element, FrameworkRenderElementContext ancestor) {			
			Matrix result = Matrix.Identity;
			if(element == ancestor)
				return result;
			result = RenderTransformValue(element);
			bool isAncestor = false;
			foreach (var anc in RenderAncestors(element)) {
				if (anc == ancestor) {
					isAncestor = true;
					break;
				}
				result *= RenderTransformValue(anc);
			}
			if (!isAncestor)
				throw new ArgumentException("ancestor");
			return result;
		}
		static Matrix RenderTransformValue(FrameworkRenderElementContext element) {
			if (element == null)
				throw new ArgumentNullException("element");
			if (element.RenderTransform == null)
				return Matrix.Identity;
			return element.RenderTransform.Value;
		}
		static Matrix GetMatrixTransformToDescendant(FrameworkRenderElementContext element, FrameworkRenderElementContext descendant) {
			var matrix = GetMatrixTransformToAncestor(descendant, element);
			matrix.Invert();
			return matrix;
		}
		public static IEnumerable<FrameworkRenderElementContext> RenderAncestors(FrameworkRenderElementContext context) {
			while (context != null) {
				context = context.Parent;
				if (context == null)
					break;
				yield return context;
			}
		}
		public static IEnumerable<FrameworkRenderElementContext> RenderDescendants(FrameworkRenderElementContext context) {
			foreach (var child in RenderChildren(context)) {
				yield return child;
				foreach (var descendant in RenderDescendants(child))
					yield return descendant;
			}
		}
		static IEnumerable<FrameworkRenderElementContext> RenderChildren(FrameworkRenderElementContext context) {
			var iContext = (IFrameworkRenderElementContext)context;
			for (int i = 0; i < iContext.RenderChildrenCount; i++) {
				yield return iContext.GetRenderChild(i);
			}
		}
		public static T FindDescendant<T>(FrameworkRenderElementContext context) where T : FrameworkRenderElementContext {
			return (T)FindDescendant(context, x => x is T);
		}		
		public static FrameworkRenderElementContext FindDescendant(FrameworkRenderElementContext context, Func<FrameworkRenderElementContext, bool> predicate) {
			return RenderDescendants(context).FirstOrDefault(x => predicate(x));
		}
	}
}
