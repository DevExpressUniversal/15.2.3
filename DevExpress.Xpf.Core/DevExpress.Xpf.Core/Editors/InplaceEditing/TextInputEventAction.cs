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

using System.Windows.Input;
using System.Windows;
using System.Windows.Media;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Editors.Helpers;
using DevExpress.Xpf.Editors;
#if !SL
using DevExpress.Xpf.Utils;
using DevExpress.Xpf.Core;
#else
using System;
using DevExpress.Xpf.Core.WPFCompatibility.Helpers;
#endif
#if SL
using RoutedEvent = DevExpress.Xpf.Core.WPFCompatibility.SLRoutedEvent;
using RoutedEventArgs = DevExpress.Xpf.Core.WPFCompatibility.SLRoutedEventArgs;
#endif
namespace DevExpress.Xpf.Editors {
	public abstract class CellEditorAction : IAction {
		protected InplaceEditorBase editor;
		public CellEditorAction(InplaceEditorBase editor) {
			this.editor = editor;
		}
		#region IAction Members
		public abstract void Execute();
		#endregion
	}
	public class SelectAllAction : CellEditorAction {
		public SelectAllAction(InplaceEditorBase editor)
			: base(editor) {
		}
		public override void Execute() {
			editor.SelectAll();
		}
	}
	public abstract class RaisePosponedEventAction<T> : CellEditorAction where T : RoutedEventArgs {
		T posponedEventArgs;
		protected RaisePosponedEventAction(InplaceEditorBase editor, T e)
			: base(editor) {
			this.posponedEventArgs = e;
		}
		public sealed override void Execute() {
#if !SL
			if(!editor.IsInTree)
				return;
			ReraiseEventHelper.ReraiseEvent<T>(posponedEventArgs, GetElement(posponedEventArgs), TunnelingEvent, BubblingEvent, CloneEventArgs);
#else
			throw new NotImplementedException();
#endif
		}
		protected abstract RoutedEvent BubblingEvent { get; }
		protected abstract RoutedEvent TunnelingEvent { get; }
		protected abstract T CloneEventArgs(T posponedEventArgs);
		protected abstract UIElement GetElement(T posponedEventArgs);
	}
	public class ProcessActivatingKeyAction : CellEditorAction {
		KeyEventArgs e;
		public ProcessActivatingKeyAction(InplaceEditorBase editor, KeyEventArgs e)
			: base(editor) {
			this.e = e;
		}
		public override void Execute() {
			editor.ProcessActivatingKey(e);
		}
	}
#if !SL
	public class RaisePostponedTextInputEventAction : RaisePosponedEventAction<TextCompositionEventArgs> {
		public RaisePostponedTextInputEventAction(InplaceEditorBase editor, TextCompositionEventArgs e)
			: base(editor, e) {
		}
		protected override RoutedEvent BubblingEvent { get { return UIElement.TextInputEvent; } }
		protected override RoutedEvent TunnelingEvent { get { return UIElement.PreviewTextInputEvent; } }
		protected override TextCompositionEventArgs CloneEventArgs(TextCompositionEventArgs posponedEventArgs) {
			return new TextCompositionEventArgs(posponedEventArgs.Device, posponedEventArgs.TextComposition);
		}
		protected override UIElement GetElement(TextCompositionEventArgs posponedEventArgs) {
			return (UIElement)FocusHelper.GetFocusedElement();
		}
	}
	public class RaisePostponedMouseEventAction : RaisePosponedEventAction<MouseButtonEventArgs> {
#if DEBUGTEST
		internal static UIElement forcedHitTestResult;
		public static void ForceHitTestResult(UIElement hitTestResult) {
			forcedHitTestResult = hitTestResult;
		}
#endif
		public static UIElement GetMouseEventReraiseElement(UIElement sourceElement, Point position) {
			HitTestResult hitTestResult = VisualTreeHelper.HitTest(sourceElement, position);
#if DEBUGTEST
			if(forcedHitTestResult != null) {
				hitTestResult = new PointHitTestResult(forcedHitTestResult, new Point());
			}
#endif
			if(hitTestResult == null)
				return null;
			return LayoutHelper.FindParentObject<UIElement>(hitTestResult.VisualHit);
		}
		public RaisePostponedMouseEventAction(InplaceEditorBase editor, MouseButtonEventArgs e)
			: base(editor, e) {
		}
		protected override RoutedEvent BubblingEvent { get { return UIElement.MouseDownEvent; } }
		protected override RoutedEvent TunnelingEvent { get { return UIElement.PreviewMouseDownEvent; } }
		protected override MouseButtonEventArgs CloneEventArgs(MouseButtonEventArgs posponedEventArgs) {
			return ReraiseEventHelper.CloneMouseButtonEventArgs(posponedEventArgs);
		}
		protected override UIElement GetElement(MouseButtonEventArgs posponedEventArgs) {
			return GetMouseEventReraiseElement(editor, posponedEventArgs.GetPosition(editor));
		}
	}
#endif
}
