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
using System.Drawing;
using DevExpress.Utils;
using DevExpress.Utils.Commands;
using DevExpress.Utils.KeyboardHandler;
using DevExpress.XtraRichEdit.Commands;
using DevExpress.XtraRichEdit.Layout;
using DevExpress.XtraRichEdit.Model;
using DevExpress.XtraRichEdit.Commands.Internal;
using DevExpress.XtraRichEdit.Internal.PrintLayout;
using DevExpress.XtraRichEdit.Utils;
using DevExpress.Office.Utils;
using System.Windows.Forms;
using DevExpress.Compatibility.System.Drawing;
using DevExpress.Compatibility.System.Windows.Forms;
namespace DevExpress.XtraRichEdit.Mouse {
	public abstract class RichEditMouseHandlerStrategyFactory {
		public abstract RichEditMouseHandlerStrategy CreateMouseHandlerStrategy(RichEditMouseHandler mouseHandler);
		public abstract DragFloatingObjectManuallyMouseHandlerStateStrategy CreateDragFloatingObjectManuallyMouseHandlerStateStrategy(DragFloatingObjectManuallyMouseHandlerState state);
		public abstract ResizeTableRowMouseHandlerStateStrategy CreateResizeTableRowMouseHandlerStateStrategy(ResizeTableRowMouseHandlerState state);
		public abstract ResizeTableVirtualColumnMouseHandlerStateStrategy CreateResizeTableVirtualColumnMouseHandlerStateStrategy(ResizeTableVirtualColumnMouseHandlerState state);
		public abstract DragContentMouseHandlerStateBaseStrategy CreateDragContentMouseHandlerStateBaseStrategy(DragContentMouseHandlerStateBase state);
		public abstract RichEditRectangularObjectResizeMouseHandlerStateStrategy CreateRichEditRectangularObjectResizeMouseHandlerStateStrategy(RichEditRectangularObjectResizeMouseHandlerState state);
	}
	public abstract class RichEditMouseHandlerStrategyBase {
		readonly RichEditMouseHandler mouseHandler;
		protected RichEditMouseHandlerStrategyBase(RichEditMouseHandler mouseHandler) {
			this.mouseHandler = mouseHandler;
		}
		public RichEditMouseHandler MouseHandler { get { return mouseHandler; } }
		public IRichEditControl Control { get { return MouseHandler.Control; } }
	}
	public abstract class RichEditMouseHandlerStrategy : RichEditMouseHandlerStrategyBase {
		protected RichEditMouseHandlerStrategy(RichEditMouseHandler mouseHandler)
			: base(mouseHandler) {
		}
		protected virtual IOfficeScroller OfficeScroller { get { return MouseHandler.OfficeScroller; } }
		protected internal abstract void StartOfficeScroller(Point clientPoint);
		protected internal abstract IOfficeScroller CreateOfficeScroller();
		protected internal abstract MouseEventArgs CreateFakeMouseMoveEventArgs();
		protected internal abstract MouseEventArgs ConvertMouseEventArgs(MouseEventArgs screenMouseEventArgs);
		protected internal abstract DragEventArgs ConvertDragEventArgs(DragEventArgs screenDragEventArgs);
		protected internal abstract void AutoScrollerOnDragOver(Point pt);
		protected internal abstract void OnMouseUp(MouseEventArgs e);
		protected internal abstract DragContentMouseHandlerStateBase CreateInternalDragState();
	}
	public abstract class RichEditMouseHandlerStateStrategyBase<T> : RichEditMouseHandlerStrategyBase where T : RichEditMouseHandlerState {
		readonly T state;
		protected RichEditMouseHandlerStateStrategyBase(T state)
			: base(state.MouseHandler) {
			Guard.ArgumentNotNull(state, "state");
			this.state = state;
		}
		protected T State { get { return state; } }
	}
	public abstract class DragFloatingObjectManuallyMouseHandlerStateStrategy : RichEditMouseHandlerStateStrategyBase<DragFloatingObjectManuallyMouseHandlerState> {
		protected DragFloatingObjectManuallyMouseHandlerStateStrategy(DragFloatingObjectManuallyMouseHandlerState state)
			: base(state) {
		}
		protected DocumentModel DocumentModel { get { return State.DocumentModel; } }
		protected Rectangle InitialShapeBounds { get { return State.InitialShapeBounds; } }
		protected Rectangle InitialContentBounds { get { return State.InitialContentBounds; } }
		protected FloatingObjectAnchorRun Run { get { return State.Run; } }
		protected internal abstract void ShowVisualFeedbackCore(Rectangle bounds, PageViewInfo pageViewInfo, OfficeImage image);
		protected internal abstract void HideVisualFeedbackCore(Rectangle bounds, PageViewInfo pageViewInfo);
		protected internal abstract OfficeImage CreateFeedbackImage(OfficeImage originalImage);
		protected internal abstract void BeginVisualFeedback();
		protected internal abstract void EndVisualFeedback();
	}
	public abstract class ResizeTableRowMouseHandlerStateStrategy : RichEditMouseHandlerStateStrategyBase<ResizeTableRowMouseHandlerState> {
		protected ResizeTableRowMouseHandlerStateStrategy(ResizeTableRowMouseHandlerState state)
			: base(state) {
		}
		protected PageViewInfo PageViewInfo { get { return State.PageViewInfo; } }
		protected internal abstract void DrawReversibleLineCore(int y);
		protected internal abstract void BeginVisualFeedback();
		protected internal abstract void ShowVisualFeedback();
		protected internal abstract void EndVisualFeedback();
		protected internal virtual void HideVisualFeedback() {
			State.DrawReversibleLineCore();
		}
	}
	public abstract class ResizeTableVirtualColumnMouseHandlerStateStrategy : RichEditMouseHandlerStateStrategyBase<ResizeTableVirtualColumnMouseHandlerState> {
		protected ResizeTableVirtualColumnMouseHandlerStateStrategy(ResizeTableVirtualColumnMouseHandlerState state)
			: base(state) {
		}
		protected PageViewInfo PageViewInfo { get { return State.PageViewInfo; } }
		protected internal abstract void DrawReversibleLineCore(int x);
		protected internal abstract void BeginVisualFeedback();
		protected internal abstract void ShowVisualFeedback();
		protected internal abstract void HideVisualFeedback();		
		protected internal abstract void EndVisualFeedback();
	}
	public abstract class DragContentMouseHandlerStateBaseStrategy : RichEditMouseHandlerStateStrategyBase<DragContentMouseHandlerStateBase> {
		protected DragContentMouseHandlerStateBaseStrategy(DragContentMouseHandlerStateBase state)
			: base(state) {
		}
		public abstract void Finish();
		protected internal abstract DragCaretVisualizer CreateCaretVisualizer();
	}
	public abstract class RichEditRectangularObjectResizeMouseHandlerStateStrategy : RichEditMouseHandlerStateStrategyBase<RichEditRectangularObjectResizeMouseHandlerState> {
		protected RichEditRectangularObjectResizeMouseHandlerStateStrategy(RichEditRectangularObjectResizeMouseHandlerState state)
			: base(state) {
		}
		protected Rectangle ObjectActualBounds { get { return State.ObjectActualBounds; } }
		protected PageViewInfo PageViewInfo { get { return State.PageViewInfo; } }
		protected internal abstract void BeginVisualFeedback();
		protected internal abstract void ShowVisualFeedback();
		protected internal abstract void HideVisualFeedback();
		protected internal abstract void EndVisualFeedback();
	}
}
