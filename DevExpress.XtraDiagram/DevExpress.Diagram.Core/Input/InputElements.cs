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

using DevExpress.Diagram.Core;
using DevExpress.Diagram.Core.Native;
using DevExpress.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;
namespace DevExpress.Diagram.Core {
	public abstract class DefaultInputElement : IInputElement {
		InputState IInputElement.CreatePointerToolMousePressedState(IDiagramControl diagram, IMouseButtonArgs mouseArgs) {
			return CreatePointerToolMousePressedState(diagram, mouseArgs);
		}
		InputState IInputElement.CreateItemToolMousePressedState(IDiagramControl diagram, IMouseButtonArgs mouseArgs, ItemTool tool) {
			return CreateItemToolMousePressedState(diagram, mouseArgs, tool);
		}
		InputState IInputElement.CreateConnectorToolMousePressedState(IDiagramControl diagram, IMouseButtonArgs mouseArgs) {
			return ChooseConnectorToolEffect<InputState>(diagram, mouseArgs,
				connectedConnector: () => CreateConnectorToolMousePressedState(diagram, mouseArgs),
				freeConnector: () => CreateConnectorToolMousePressedState(diagram, mouseArgs),
				noConnector: () => CreatePointerToolMousePressedState(diagram, mouseArgs));
		}
		DiagramCursor IInputElement.GetPointerToolCursor(IMouseArgs mouse) {
			return GetPointerToolCursor(mouse);
		}
		DiagramCursor IInputElement.GetItemToolCursor(IDiagramControl diagram, IMouseArgs mouse) {
			return GetItemToolCursor(mouse);
		}
		DiagramCursor IInputElement.GetConnectorToolCursor(IDiagramControl diagram, IMouseArgs mouse) {
			return GetConnectorToolCursor(diagram, mouse);
		}
		protected DiagramCursor GetItemToolCursor(IMouseArgs mouse) {
			if(CanUseItemTool(mouse))
				return DiagramCursor.DrawItem;
			else
				return GetPointerToolCursor(mouse);
		}
		protected DiagramCursor GetConnectorToolCursor(IDiagramControl diagram, IMouseArgs mouse) {
			return ChooseConnectorToolEffect<DiagramCursor>(diagram, mouse,
				connectedConnector: () => DiagramCursor.HoverConnectedConnectorSource,
				freeConnector: () => DiagramCursor.HoverFreeConnectorSource,
				noConnector: () => GetPointerToolCursor(mouse));
		}
		protected abstract bool CanDrawFreeConnector { get; }
		T ChooseConnectorToolEffect<T>(IDiagramControl diagram, IMouseArgs mouseArgs, Func<T> connectedConnector, Func<T> freeConnector, Func<T> noConnector) {
			return diagram.CanDrawConnectedConnector(mouseArgs)
				? connectedConnector()
				: (CanDrawFreeConnector
					? freeConnector()
					: noConnector());
		}
		protected virtual InputState CreatePointerToolMousePressedState(IDiagramControl diagram, IMouseButtonArgs mouseArgs) {
			return CreatePointerPressedState(diagram, mouseArgs);
		}
		protected virtual DiagramCursor GetPointerToolCursor(IMouseArgs args) {
			return GetMoveEffect(args).GetCursor();
		}
		protected MoveEffect? GetMoveEffect(IMouseArgs args) {
			var allowedEffects = GetAllowedEffects(args.Position);
			if(args.IsCloneSelectionModifierPressed() && allowedEffects.CanCopy())
				return MoveEffect.Copy;
			if(!args.IsCloneSelectionModifierPressed() && allowedEffects.CanMove())
				return MoveEffect.Move;
			return null;
		}
		protected abstract IDiagramItem GetStartDragItem();
		protected abstract MouseUpHandler GetMouseUpHandler(IMouseButtonArgs buttonArgs);
		protected abstract AllowedMoveEffects GetAllowedEffects(Point diagramPoint);
		protected abstract bool CanUseItemTool(IMouseArgs mouse);
		InputState CreatePointerPressedState(IDiagramControl diagram, IMouseButtonArgs mouseButtonArgs) {
			return MousePressedActiveInputState.Create(diagram, GetPointerToolCursor, mouseButtonArgs,
				GetMouseUpHandler(mouseButtonArgs), StartPointerDrag);
		}
		protected InputState CreateItemToolMousePressedState(IDiagramControl diagram, IMouseButtonArgs mouseButtonArgs, ItemTool tool) {
			StartDragHandler startDragHandler = (d, mouseArgs, startPosition, changedButton) => {
				if(CanUseItemTool(mouseArgs)) {
					return StartDrawItem(d, mouseArgs, tool, startPosition, DiagramCursor.DrawItem, changedButton);
				} else {
					return StartPointerDrag(d, mouseArgs, startPosition, changedButton);
				}
			};
			return MousePressedActiveInputState.Create(diagram, GetItemToolCursor, mouseButtonArgs,
				GetMouseUpHandler(mouseButtonArgs), startDragHandler);
		}
		protected InputState CreateConnectorToolMousePressedState(IDiagramControl diagram, IMouseButtonArgs mouseArgs) {
			return MousePressedActiveInputState.Create(diagram, m => GetConnectorToolCursor(diagram, m), mouseArgs,
				GetMouseUpHandler(mouseArgs), StartDrawConnector);
		}
		static InputState StartDrawConnector(IDiagramControl diagram, IMouseArgs mouseArgs, Point startPosition, MouseButton changedButton) {
			var connector = diagram.CreateConnector();
			diagram.MoveStandaloneConnectorBeginEndPoint(connector, startPosition, ConnectorPointType.Begin);
			return MoveConnectorPointInputElement.CreateMoveConnectorBeginEndPointState(connector, diagram, mouseArgs, changedButton, ConnectorPointType.End);
		}
		InputState StartPointerDrag(IDiagramControl d, IMouseArgs mouseArgs, Point startPosition, MouseButton changedButton) {
			if(GetMoveEffect(mouseArgs) != null) {
				return MoveItemsState.Create(d, GetMoveEffect, startPosition, mouseArgs, GetStartDragItem(), changedButton);
			} else {
				d.UnselectItemWithModifier(mouseArgs, null);
				return StartDrawSelection(d, DiagramCursor.DrawSelection, startPosition, mouseArgs, changedButton);
			}
		}
		static InputState StartDrawItem(IDiagramControl diagram, IMouseArgs mouseArgs, ItemTool tool, Point startPosition, DiagramCursor cursor, MouseButton button) {
			var item = tool.CreateItem(diagram);
			var preview = item.Controller.CreateDragPreviewAdorner(diagram);
			return DragMouseActiveInputState.Create(diagram, cursor, button, mouseArgs,
				onMouseMove: e => {
					preview.SetBounds(startPosition, e.Position);
					item.Size = preview.Bounds.Size;
				},
				onMouseUp: e => {
					diagram.DrawItem(startPosition, e.Position, item);
				},
				onEscape: () => {
					preview.Destroy();
				});
		}
		static InputState StartDrawSelection(IDiagramControl diagram, DiagramCursor cursor, Point startPosition, IMouseArgs mouseArgs, MouseButton button) {
			var preview = diagram.AdornerFactory().CreateSelectionPreview();
			var selectionModifierWasPressed = mouseArgs.IsSelectionModifierPressed();
			return DragMouseActiveInputState.Create(diagram, cursor, button, mouseArgs,
				onMouseMove: e => {
					preview.SetBounds(startPosition, e.Position);
				},
				onMouseUp: e => {
					diagram.Controller.SelectItemsArea(startPosition, e.Position, addToSelection: selectionModifierWasPressed);
				},
				onEscape: () => {
					preview.Destroy();
				});
		}
		protected static AllowedMoveEffects GetAllowedEffects(bool canMove, bool canCopy) {
			var allowedEffects = AllowedMoveEffects.None;
			if(canMove)
				allowedEffects |= AllowedMoveEffects.Move;
			if(canCopy)
				allowedEffects |= AllowedMoveEffects.Copy;
			return allowedEffects;
		}
	}
	public sealed class SimpleInputElement : IInputElement {
		public static IInputElement Create(Func<DiagramCursor> getCursor, Func<IDiagramControl, IMouseButtonArgs, InputState> createState) {
			return new SimpleInputElement(getCursor, createState);
		}
		public static IInputElement CreateStandard(DiagramCursor cursor, IDiagramItem item, StartDragHandler startDragHandler) {
			return Create(
				getCursor: () => cursor,
				createState: (diagram, mouseArgs) => {
					return MousePressedActiveInputState.Create(diagram, x => cursor, mouseArgs, DiagramItemInputElement.GetMouseUpHandler(item, mouseArgs), startDragHandler);
				});
		}
		Func<DiagramCursor> getCursor;
		Func<IDiagramControl, IMouseButtonArgs, InputState> createState;
		SimpleInputElement(Func<DiagramCursor> getCursor, Func<IDiagramControl, IMouseButtonArgs, InputState> createState) {
			this.getCursor = getCursor;
			this.createState = createState;
		}
		InputState IInputElement.CreatePointerToolMousePressedState(IDiagramControl diagram, IMouseButtonArgs mouseArgs) {
			return createState(diagram, mouseArgs);
		}
		InputState IInputElement.CreateItemToolMousePressedState(IDiagramControl diagram, IMouseButtonArgs mouseArgs, ItemTool tool) {
			return createState(diagram, mouseArgs);
		}
		InputState IInputElement.CreateConnectorToolMousePressedState(IDiagramControl diagram, IMouseButtonArgs mouseArgs) {
		   return createState(diagram, mouseArgs);
		}
		DiagramCursor IInputElement.GetPointerToolCursor(IMouseArgs mouse) {
			return getCursor();
		}
		DiagramCursor IInputElement.GetItemToolCursor(IDiagramControl diagram, IMouseArgs mouse) {
			return getCursor();
		}
		DiagramCursor IInputElement.GetConnectorToolCursor(IDiagramControl diagram, IMouseArgs mouse) {
			return getCursor();
		}
	}
	public static class ItemResizeInputElement {
		public static IInputElement Create(ResizeMode mode, DefaultSelectionLayerHandler selectionHandler) {
			return SimpleInputElement.Create(
				getCursor: () => mode.Rotate(selectionHandler.SelectionAdornerAngle).GetCursor(), 
				createState: (diagram, mouseArgs) => {
					Func<IMouseArgs, SnapInfo> getSnapInfo = args => diagram.GetSnapInfo(diagram.SelectedItems(), diagram.PrimarySelection(), args.IsSnappingEnabled());
					if(diagram.ResizingMode == ResizingMode.Preview)
						return ResizeState.CreateResizePreviewState(diagram, mode, mouseArgs, selectionHandler.GetAdornersSizeInfo(mode), diagram.GetResizableSelectedItems(), () => selectionHandler.UpdateMultipleSelectionAdorner(), () => { }, getSnapInfo, mouseArgs.ChangedButton, selectionHandler.SelectionAdornerAngle);
					else
						return ResizeState.CreateResizeLiveState(diagram, mode, mouseArgs, diagram.GetResizableSelectedItems(), getSnapInfo, mouseArgs.ChangedButton, selectionHandler.SelectionAdornerAngle);
				});
		}
	}
	public static class ChangeParameterInputElement {
		public static IInputElement Create(IDiagramShape shape, ParameterDescription parameter) {
			return SimpleInputElement.CreateStandard(
				cursor: DiagramCursor.HoverParameter,
				item: shape,
				startDragHandler: (diagram, mouseArgs, startPosition, changedButton) => {
					return CreateDragHandler(DiagramCursor.ChangeParameter, startPosition, mouseArgs, changedButton, diagram, shape, parameter);
				});
		}
		static InputState CreateDragHandler(DiagramCursor cursor, Point startPosition, IMouseArgs mouseArgs, MouseButton button, IDiagramControl diagram, IDiagramShape shape, ParameterDescription parameter) {
			Func<IMouseArgs, SnapInfo> getSnapInfo = args => diagram.GetSnapInfo(snapScopeItem: diagram.PrimarySelection(), isSnappingEnabled: args.IsSnappingEnabled());
			var transaction = new Transaction();
			var startParameterPosition = shape.GetActualParameterPoint(parameter);
			var startPositionDelta = MathHelper.GetOffset(startPosition, startParameterPosition);
			return DragMouseActiveInputState.Create(diagram, cursor, button, mouseArgs,
				onMouseMove: e => {
					transaction.ExecuteWithSelectionRestore(diagram, nestedTransaction => {
						diagram.ChangeParameter(shape, parameter, e.Position.OffsetPoint(startPositionDelta), getSnapInfo(e), nestedTransaction);
					});
				},
				onMouseUp: e => {
					diagram.UndoManager().Commit(transaction, allowMerge: false);
				},
				onEscape: () => {
					transaction.Rollback();
				});
		}
	}
	public static class RotateShapeInputElement {
		public static IInputElement Create(DefaultSelectionLayerHandler selectionHandler) {
			return SimpleInputElement.CreateStandard(
				cursor: DiagramCursor.HoverRotation,
				item: selectionHandler.Diagram.PrimarySelection(),
				startDragHandler: (diagram, mouseArgs, startPosition, changedButton) => {
					return CreateDragHandler(DiagramCursor.Rotation, startPosition, mouseArgs, changedButton, selectionHandler);
				});
		}
		static InputState CreateDragHandler(DiagramCursor cursor, Point startPosition, IMouseArgs mouseArgs, MouseButton button, DefaultSelectionLayerHandler selectionHandler) {
			IDiagramControl diagram = selectionHandler.Diagram;
			var transaction = new Transaction();
			double startAngle = selectionHandler.SelectionAdornerAngle;
			Point rotationCenter = selectionHandler.GetDiagramRotationCenter();
			double startAngleDelta = MathHelper.GetRotationAngle(rotationCenter, startPosition) - startAngle;
			Item_Angle[] rotateableItems = diagram.SelectedItems().Where(item => item.CanRotate).GetParentsOnly().Select(item => new Item_Angle(item, item.Angle)).ToArray();
			return DragMouseActiveInputState.Create(diagram, cursor, button, mouseArgs,
				onMouseMove: e => {
					transaction.ExecuteWithSelectionRestore(diagram, nestedTransaction => {
						double endAngle = diagram.SnapRotationAngle(MathHelper.GetRotationAngle(rotationCenter, e.Position) - startAngleDelta, e.IsSnappingEnabled());
						diagram.RotateItems(nestedTransaction, endAngle - startAngle, rotateableItems, rotationCenter);
						selectionHandler.SetMultipleSelectionAdornerAngle(nestedTransaction, endAngle);
					});
				},
				onMouseUp: e => {
					diagram.UndoManager().Commit(transaction, allowMerge: false);
				},
				onEscape: () => {
					transaction.Rollback();
				});
		}
	}
	public static class MoveConnectorPointInputElement {
		public static IInputElement CreateMoveBeginEndPointElement(IDiagramConnector connector, ConnectorPointType pointType) {
			return connector.CreateMoveConnectorPointElement(
				startDragHandler: (diagram, mouseArgs, startPosition, changedButton) => {
					return CreateMoveConnectorBeginEndPointState(connector, diagram, mouseArgs, changedButton, pointType);
				});
		}
		public static IInputElement CreateMoveMiddlePointElement(IDiagramConnector connector, int pointIndex) {
			return connector.CreateMoveConnectorPointElement(
				startDragHandler: (diagram, mouseArgs, startPosition, changedButton) => {
					return CreateMoveConnectorMiddlePointState(connector, diagram, mouseArgs, changedButton, pointIndex);
				});
		}
		static IInputElement CreateMoveConnectorPointElement(this IDiagramConnector connector, StartDragHandler startDragHandler) { 
			return SimpleInputElement.CreateStandard(
				cursor: DiagramCursor.HoverConnectorPoint,
				item: connector,
				startDragHandler: startDragHandler);
		}
		public static InputState CreateMoveConnectorBeginEndPointState(IDiagramConnector connector, IDiagramControl diagram, IMouseArgs mouseArgs, MouseButton changedButton, ConnectorPointType pointType) {
			return CreateMoveConnectorPointState(connector, diagram, mouseArgs, changedButton,
				transformMoveAdornerProxy: (proxy, mousePosition) => diagram.TransformProxyBeginEndPoint(connector, pointType, proxy, mousePosition),
				doMovePoint: (d, mousePosition) => d.MoveConnectorBeginEndPoint(connector, mousePosition, pointType),
				feedbackHelper: diagram.CreateMoveConnectorPointFeedbackHelper()
				);
		}
		static InputState CreateMoveConnectorMiddlePointState(IDiagramConnector connector, IDiagramControl diagram, IMouseArgs mouseArgs, MouseButton changedButton, int pointIndex) {
			return CreateMoveConnectorPointState(connector, diagram, mouseArgs, changedButton,
				transformMoveAdornerProxy: (proxy, mousePosition) => diagram.TransformProxyMiddlePoint(connector, pointIndex, proxy, mousePosition),
				doMovePoint: (d, mousePosition) => d.MoveConnectorMiddlePoint(connector, mousePosition, pointIndex),
				feedbackHelper: MouseMoveFeedbackHelper.Empty
				);
		}
		static InputState CreateMoveConnectorPointState(IDiagramConnector connector, IDiagramControl diagram, IMouseArgs mouseArgs, MouseButton changedButton, Func<ConnectorProxy, Point, ConnectorProxy> transformMoveAdornerProxy, Action<IDiagramControl, Point> doMovePoint, MouseMoveFeedbackHelper feedbackHelper) {
			var adorner = diagram.AdornerFactory().CreateConnectorMovePointPreview();
			return DragMouseActiveInputState.Create(diagram, DiagramCursor.MoveConnectorPoint, changedButton, mouseArgs,
				onMouseMove: e => {
					connector.UpdateMoveAdorner(adorner, x => transformMoveAdornerProxy(x, e.Position));
					feedbackHelper.ProvideMouseMoveFeedback(diagram, e.Position);
				},
				onMouseUp: e => {
					doMovePoint(diagram, e.Position);
				},
				onEscape: () => {
					adorner.Destroy();
					feedbackHelper.ClearMouseMoveFeedback();
				});
		}
	}
	public sealed class MultipleSelectionAdornerInputElement : DefaultInputElement {
		readonly IDiagramControl diagram;
		public MultipleSelectionAdornerInputElement(IDiagramControl diagram) {
			this.diagram = diagram;
		}
		protected override AllowedMoveEffects GetAllowedEffects(Point diagramPoint) {
			return GetAllowedEffects(diagram.GetSelectedMovableItems().Any(), diagram.GetSelectedCopyableItems().Any());
		}
		protected override bool CanUseItemTool(IMouseArgs mouse) {
			return GetMoveEffect(mouse) == null;
		}
		protected override IDiagramItem GetStartDragItem() {
			return diagram.PrimarySelection();
		}
		protected override MouseUpHandler GetMouseUpHandler(IMouseButtonArgs buttonArgs) {
			return MouseUpHandler;
		}
		static void MouseUpHandler(IDiagramControl diagram, IMouseButtonArgs mouseArgs) {
			diagram.UnselectItemWithModifier(mouseArgs, null);
			diagram.ShowPopupMenuIfRightButton(mouseArgs);
		}
		protected override bool CanDrawFreeConnector { get { return false; } }
	}
	public class DiagramItemInputElement : DefaultInputElement {
		readonly Func<IDiagramItem> getItem;
		protected IDiagramItem Item { get { return getItem(); } }
		public DiagramItemInputElement(Func<IDiagramItem> getItem) {
			this.getItem = getItem;
			;
		}
		protected override bool CanUseItemTool(IMouseArgs mouse) {
			return !Item.IsSelected || GetMoveEffect(mouse) == null;
		}
		protected override AllowedMoveEffects GetAllowedEffects(Point diagramPoint) {
			return GetAllowedEffects(Item.CanMove, Item.Controller.CanCopyCore());
		}
		protected override MouseUpHandler GetMouseUpHandler(IMouseButtonArgs buttonArgs) {
			var item = GetActualItem(buttonArgs.Position);
			return GetMouseUpHandler(item, buttonArgs);
		}
		internal static MouseUpHandler GetMouseUpHandler(IDiagramItem item, IMouseButtonArgs buttonArgs) {
			bool isDoubleClick = buttonArgs.IsDoubleClick();
			return (diagram, mouseArgs) => {
				if(isDoubleClick) {
					if(diagram.PrimarySelection() == null)
						diagram.SelectItem(item);
					diagram.StartEditPrimarySelection();
				} else {
					if(item.IsSelected) {
						if(mouseArgs.IsRightButton())
							diagram.ShowPopupMenu();
						else
							diagram.UnselectItemWithModifier(mouseArgs, item);
					} else {
						diagram.SelectItemWithModifier(item, mouseArgs);
						diagram.ShowPopupMenuIfRightButton(mouseArgs);
					}
				}
			};
		}
		protected override IDiagramItem GetStartDragItem() {
			return Item;
		}
		protected virtual IDiagramItem GetActualItem(Point diagramPoint) {
			return Item;
		}
		protected override bool CanDrawFreeConnector { get { return false; } }
	}
	public sealed class RootItemInputElement : DiagramItemInputElement {
		public RootItemInputElement(Func<IDiagramItem> getItem) : base(getItem) {
		}
		protected override AllowedMoveEffects GetAllowedEffects(Point diagramPoint) {
			return AllowedMoveEffects.None;
		}
		protected override bool CanUseItemTool(IMouseArgs mouse) {
			return true;
		}
		protected override bool CanDrawFreeConnector { get { return true; } }
	}
	public class ContainerItemInputElement : DiagramItemInputElement {
		public ContainerItemInputElement(IDiagramItem item) : base(() => item) {
		}
		protected override bool CanUseItemTool(IMouseArgs mouse) {
			return base.CanUseItemTool(mouse) || IsInnerArea(mouse.Position);
		}
		protected override AllowedMoveEffects GetAllowedEffects(Point diagramPoint) {
			if(Item.IsSelected || !IsInnerArea(diagramPoint))
				return base.GetAllowedEffects(diagramPoint);
			return AllowedMoveEffects.None;
		}
		bool IsInnerArea(Point diagramPoint) {
			return Item.ActualDiagramBounds().InflateRect(-5).Contains(diagramPoint);
		}
	}
}
