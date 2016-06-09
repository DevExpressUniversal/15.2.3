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
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using DevExpress.Diagram.Core;
using DevExpress.Internal;
using IInputElement = DevExpress.Diagram.Core.IInputElement;
namespace DevExpress.Diagram.Core {
	public sealed class MoveItemsState : MouseActiveInputState {
		class Item_Adorner {
			public readonly IDiagramItem Item;
			public readonly IAdorner Adorner;
			public Item_Adorner(IDiagramItem item, IAdorner adorner) {
				this.Item = item;
				this.Adorner = adorner;
			}
		}
		public static InputState Create(IDiagramControl diagram, Func<IMouseArgs, MoveEffect?> getMoveEffect, Point startPosition, IMouseArgs mouse, IDiagramItem startDragItem, MouseButton button) {
			if(!startDragItem.IsSelected)
				diagram.SelectItemWithModifier(startDragItem, mouse);
			return CheckCanDrop(() => {
				var moveEffect = getMoveEffect(mouse).Value;
				var adorners = ((moveEffect == MoveEffect.Move) ? diagram.GetSelectedMovableItems() : diagram.GetSelectedCopyableItems())
					.GetParentsOnly()
					.Select(item => new Item_Adorner(item, item.Controller.CreateDragPreviewAdorner(diagram).AddShadow(diagram)))
					.ToArray();
				return new MoveItemsState(diagram, getMoveEffect, startPosition, mouse, adorners, startDragItem, new SnapLinesPresenter(), button);
			}, getMoveEffect, diagram, startPosition, mouse, startDragItem, button, checkBounds: false);
		}
		static InputState CheckCanDrop(Func<InputState> getNormalState, Func<IMouseArgs, MoveEffect?> getMoveEffect, IDiagramControl diagram, Point startPosition, IMouseArgs mouse, IDiagramItem startDragItem, MouseButton button, Action clear = null, bool checkBounds = true) {
			return NoActionDragState.CheckNoAction(getNormalState, diagram, mouse,
				(d, e) => Create(diagram, getMoveEffect, startPosition, e, startDragItem, button),
				(d, m) => getMoveEffect(m) != null && NoActionDragState.IsInBoundsAndCanAdd(d, m, diagram.SelectedItems(), checkBounds),
				button, clear);
		}
		readonly Point startPosition;
		readonly Item_Adorner[] adorners;
		readonly IDiagramItem startDragItem;
		readonly SnapLinesPresenter snapLinesPresenter;
		readonly Func<IMouseArgs, MoveEffect?> getMoveEffect;
		MoveItemsState(IDiagramControl diagram, Func<IMouseArgs, MoveEffect?> getMoveEffect, Point startPosition, IMouseArgs mouse, Item_Adorner[] adorners, IDiagramItem startDragItem, SnapLinesPresenter snapLinesPresenter, MouseButton button)
			: base(diagram, getMoveEffect(mouse).GetCursor(), button) {
			this.startPosition = startPosition;
			this.adorners = adorners;
			this.startDragItem = startDragItem;
			this.snapLinesPresenter = snapLinesPresenter;
			this.getMoveEffect = getMoveEffect;
			UpdateAdorners(mouse);
		}
		protected override InputState MouseUp(IMouseButtonArgs mouseArgs) {
			var targetPosition = SnapTargetPosition( mouseArgs);
			if(mouseArgs.IsCloneSelectionModifierPressed())
				diagram.CopySelection(startPosition, targetPosition);
			else
				diagram.MoveSelection(startPosition, targetPosition);
			return Escape(mouseArgs);
		}
		protected override InputState Escape(IMouseArgs mouse) {
			Clear();
			return base.Escape(mouse);
		}
		void Clear() {
			adorners.ForEach(x => x.Adorner.Destroy());
			snapLinesPresenter.DestroySnapLineAdorners();
		}
		protected override InputState MouseMove(IMouseArgs mouseArgs) {
			return CheckCanDrop(() => {
				UpdateAdorners(mouseArgs);
				return base.MouseMove(mouseArgs);
			}, getMoveEffect, diagram, startPosition, mouseArgs, startDragItem, button, clear: Clear);
		}
		void UpdateAdorners(IMouseArgs mouseArgs) {
			UpdateDragAdorners(startPosition, SnapTargetPosition(mouseArgs));
		}
		Point SnapTargetPosition(IMouseArgs mouseArgs) {
			var snapOffsetResult = diagram.GetSnapInfo(diagram.SelectedItems(), mouseArgs.Position, mouseArgs.IsSnappingEnabled()).GetRectSnapOffset(MathHelper.GetOffset(startPosition, mouseArgs.Position), startDragItem.RotatedDiagramBounds().RotatedRect);
			snapLinesPresenter.RecreateSnapLineAdorners(diagram, snapOffsetResult.SnapLines);
			return mouseArgs.Position.OffsetPoint(snapOffsetResult.Offset);
		}
		protected override InputState ModifiersChanged(IMouseArgs mouseArgs) {
			return CheckCanDrop(() => {
				Clear();
				return Create(diagram, getMoveEffect, startPosition, mouseArgs, startDragItem, button);
			}, getMoveEffect, diagram, startPosition, mouseArgs, startDragItem, button, clear: Clear);
		}
		void UpdateDragAdorners(Point from, Point to) {
			var offset = MathHelper.GetOffset(from, to);
			adorners.ForEach(x => x.Adorner.Bounds = x.Item.RotatedDiagramBounds().RotatedRect.OffsetRect(offset));
		}
	}
}
