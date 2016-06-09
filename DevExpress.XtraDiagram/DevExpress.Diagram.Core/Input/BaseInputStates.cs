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
using System.Linq;
using System.Text;
using System.Windows.Input;
using DevExpress.Internal;
namespace DevExpress.Diagram.Core {
	public sealed class DefaultInputState : InputState {
		public static InputState Create(IDiagramControl diagram, DiagramCursor cursor, MouseMoveFeedbackHelper feedbackHelper) {
			return new DefaultInputState(diagram, cursor, feedbackHelper);
		}
		readonly IDiagramControl diagram;
		readonly MouseMoveFeedbackHelper feedbackHelper;
		DefaultInputState(IDiagramControl diagram, DiagramCursor cursor, MouseMoveFeedbackHelper feedbackHelper)
			: base(cursor) {
			this.diagram = diagram;
			this.feedbackHelper = feedbackHelper;
		}
		public sealed override InputResult HandleMouseUp(IInputElement item, IMouseButtonArgs mouseArgs) {
			return this.AsUnhandled();
		}
		public sealed override InputResult HandleEscape(IMouseArgs mouse) {
			feedbackHelper.ClearMouseMoveFeedback();
			return diagram.Controller.CreateDefaultInputState(mouse).AsUnhandled();
		}
		public sealed override InputResult HandleModifiersChanged(IMouseArgs mouseArgs) {
			return diagram.Controller.CreateDefaultInputState(mouseArgs, feedbackHelper).AsUnhandled();
		}
		public sealed override InputResult HandleMouseDown(IInputElement item, IMouseButtonArgs mouseArgs) {
			feedbackHelper.ClearMouseMoveFeedback();
			return MouseDown(item, mouseArgs).AsHandled();
		}
		public sealed override InputResult HandleMouseMove(IInputElement item, IMouseArgs mouseArgs) {
			feedbackHelper.ProvideMouseMoveFeedback(diagram, mouseArgs.Position);
			return diagram.Controller.CreateDefaultInputState(mouseArgs, feedbackHelper).AsUnhandled();
		}
		public sealed override InputResult HandleMouseLeave(IMouseArgs mouse) {
			feedbackHelper.ClearMouseMoveFeedback();
			return this.AsUnhandled();
		}
		InputState MouseDown(IInputElement item, IMouseButtonArgs mouseArgs) {
			diagram.Controller.FocusSurface();
			if(mouseArgs.IsPrimaryButton() && item != null) {
				mouseArgs.Capture();
				return diagram.Controller.CreateActiveInputState(item, mouseArgs);
			}
			return this;
		}
	}
	public abstract class MouseActiveInputState : InputState {
		protected readonly IDiagramControl diagram;
		protected readonly MouseButton button;
		public MouseActiveInputState(IDiagramControl diagram, DiagramCursor cursor, MouseButton button) : base(cursor) {
			this.diagram = diagram;
			this.button = button;
		}
		public sealed override InputResult HandleMouseUp(IInputElement item, IMouseButtonArgs mouseArgs) {
			if(button == mouseArgs.ChangedButton) {
				return MouseUp(mouseArgs).AsHandled();
			}
			return this.AsUnhandled();
		}
		public sealed override InputResult HandleEscape(IMouseArgs mouse) {
			return Escape(mouse).AsHandled();
		}
		public sealed override InputResult HandleModifiersChanged(IMouseArgs mouseArgs) {
			return ModifiersChanged(mouseArgs).AsHandled();
		}
		public sealed override InputResult HandleMouseDown(IInputElement item, IMouseButtonArgs mouseArgs) {
			return this.AsUnhandled();
		}
		public sealed override InputResult HandleMouseMove(IInputElement item, IMouseArgs mouseArgs) {
			return MouseMove(mouseArgs).AsHandled();
		}
		public sealed override InputResult HandleMouseLeave(IMouseArgs mouse) {
			throw new InvalidOperationException();
		}
		protected abstract InputState MouseUp(IMouseButtonArgs mouseArgs);
		protected virtual InputState MouseMove(IMouseArgs mouseArgs) {
			return this;
		}
		protected virtual InputState Escape(IMouseArgs mouse) {
			mouse.Release();
			return diagram.Controller.CreateDefaultInputState(mouse);
		}
		protected virtual InputState ModifiersChanged(IMouseArgs mouseArgs) {
			return this;
		}
	}
	public sealed class DragMouseActiveInputState : MouseActiveInputState {
		public static InputState Create(
			IDiagramControl diagram,
			DiagramCursor cursor,
			MouseButton button,
			IMouseArgs args,
			Action<IMouseArgs> onMouseMove,
			Action<IMouseArgs> onMouseUp,
			Action onEscape,
			Action<IMouseArgs> onInit = null,
			Action<IMouseArgs> onModifiersChanged = null) {
			var state = new DragMouseActiveInputState(diagram, cursor, button, onMouseMove, onMouseUp, onEscape, onModifiersChanged);
			(onInit ?? onMouseMove)(args);
			return state;
		}
		readonly Action<IMouseArgs> onMouseMove;
		readonly Action<IMouseArgs> onMouseUp;
		readonly Action onEscape;
		readonly Action<IMouseArgs> onModifiersChanged;
		DragMouseActiveInputState(
			IDiagramControl diagram,
			DiagramCursor cursor,
			MouseButton button,
			Action<IMouseArgs> onMouseMove,
			Action<IMouseArgs> onMouseUp,
			Action onEscape,
			Action<IMouseArgs> onModifiersChanged)
			: base(diagram, cursor, button) {
			this.onMouseMove = onMouseMove;
			this.onEscape = onEscape;
			this.onMouseUp = onMouseUp;
			this.onModifiersChanged = onModifiersChanged;
		}
		protected override InputState MouseMove(IMouseArgs mouseArgs) {
			onMouseMove(mouseArgs);
			return base.MouseMove(mouseArgs);
		}
		protected override InputState Escape(IMouseArgs mouse) {
			onEscape();
			return base.Escape(mouse);
		}
		protected override InputState MouseUp(IMouseButtonArgs mouseArgs) {
			onMouseUp(mouseArgs);
			return Escape(mouseArgs);
		}
		protected override InputState ModifiersChanged(IMouseArgs mouseArgs) {
			onModifiersChanged.Do(x => x(mouseArgs));
			return base.ModifiersChanged(mouseArgs);
		}
	}
	public delegate InputState StartDragHandler(IDiagramControl diagram, IMouseArgs mouseArgs, Point startPosition, MouseButton changedButton);
	public delegate void MouseUpHandler(IDiagramControl diagram, IMouseButtonArgs mouseArgs);
	public sealed class MousePressedActiveInputState : MouseActiveInputState {
		public static InputState Create(IDiagramControl diagram, Func<IMouseArgs, DiagramCursor> getCursor, IMouseButtonArgs mouseButtonArgs, MouseUpHandler mouseUp, StartDragHandler startDrag) {
			return new MousePressedActiveInputState(diagram, getCursor, mouseButtonArgs, mouseButtonArgs.Position, mouseButtonArgs.ChangedButton, mouseUp, startDrag);
		}
		readonly Point startPosition;
		readonly StartDragHandler startDrag;
		readonly MouseUpHandler mouseUp;
		readonly Func<IMouseArgs, DiagramCursor> getCursor;
		MousePressedActiveInputState(IDiagramControl diagram, Func<IMouseArgs, DiagramCursor> getCursor, IMouseArgs mouse, Point startPosition, MouseButton changedButton, MouseUpHandler mouseUp, StartDragHandler startDrag)
			: base(diagram, getCursor(mouse), changedButton) {
			this.startPosition = startPosition;
			this.mouseUp = mouseUp;
			this.startDrag = startDrag;
			this.getCursor = getCursor;
		}
		protected sealed override InputState MouseUp(IMouseButtonArgs mouseArgs) {
			mouseUp(diagram, mouseArgs);
			return Escape(mouseArgs);
		}
		protected sealed override InputState MouseMove(IMouseArgs mouseArgs) {
			if(MathHelper.IsDragGesture(mouseArgs.Position, startPosition, diagram.MinDragDistance)) {
				return startDrag(diagram, mouseArgs, startPosition, button);
			}
			return base.MouseMove(mouseArgs);
		}
		protected override InputState ModifiersChanged(IMouseArgs mouseArgs) {
			return new MousePressedActiveInputState(diagram, getCursor, mouseArgs, startPosition, button, mouseUp, startDrag);
		}
	}
	public sealed class NoActionDragState : MouseActiveInputState {
		public static InputState CheckNoAction(Func<InputState> getNormalState, IDiagramControl diagram, IMouseArgs mouseArgs, Func<IDiagramControl, IMouseArgs, InputState> recreateState, Func<IDiagramControl, IMouseArgs, bool> check, MouseButton button, Action clear) {
			if(!check(diagram, mouseArgs)) {
				clear.Do(x => x());
				return new NoActionDragState(diagram, recreateState, button, check);
			}
			return getNormalState();
		}
		public static bool IsInBoundsAndCanAdd(IDiagramControl diagram, IMouseArgs mouseArgs, IEnumerable<IDiagramItem> items, bool checkBounds) {
			return (!checkBounds || diagram.Controller.LayersHost.Controller.ClientArea.Contains(mouseArgs.Position.ScalePoint(diagram.ZoomFactor))) &&
				diagram.CanMoveItemsTo(items, mouseArgs.Position);
		}
		readonly Func<IDiagramControl, IMouseArgs, InputState> recreateState;
		readonly Func<IDiagramControl, IMouseArgs, bool> check;
		NoActionDragState(IDiagramControl diagram, Func<IDiagramControl, IMouseArgs, InputState> recreateState, MouseButton button, Func<IDiagramControl, IMouseArgs, bool> check)
			: base(diagram, DiagramCursor.No, button) {
			this.recreateState = recreateState;
			this.check = check;
		}
		protected override InputState MouseMove(IMouseArgs mouseArgs) {
			return RecreateOrContinueNoAction(mouseArgs, () => {
				return base.MouseMove(mouseArgs);
			});
		}
		protected override InputState ModifiersChanged(IMouseArgs mouseArgs) {
			return RecreateOrContinueNoAction(mouseArgs, () => {
				return base.ModifiersChanged(mouseArgs);
			});
		}
		protected override InputState MouseUp(IMouseButtonArgs mouseArgs) {
			return Escape(mouseArgs);
		}
		InputState RecreateOrContinueNoAction(IMouseArgs mouseArgs, Func<InputState> getContinueNoAction) {
			if(check(diagram, mouseArgs))
				return recreateState(diagram, mouseArgs);
			return getContinueNoAction();
		}
	}
	public class MouseMoveFeedbackHelper {
		public static readonly MouseMoveFeedbackHelper Empty = new MouseMoveFeedbackHelper((d, p) => { }, () => { });
		public readonly Action<IDiagramControl, Point> ProvideMouseMoveFeedback;
		public readonly Action ClearMouseMoveFeedback;
		public MouseMoveFeedbackHelper(Action<IDiagramControl, Point> provideMouseMoveFeedback, Action clearMouseMoveFeedback) {
			ProvideMouseMoveFeedback = provideMouseMoveFeedback;
			ClearMouseMoveFeedback = clearMouseMoveFeedback;
		}
	}
}
