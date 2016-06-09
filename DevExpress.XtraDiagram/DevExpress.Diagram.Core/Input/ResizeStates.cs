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
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using DevExpress.Diagram.Core;
using DevExpress.Internal;
using ResizeMode = DevExpress.Diagram.Core.ResizeMode;
using IInputElement = DevExpress.Diagram.Core.IInputElement;
using DevExpress.Diagram.Core.Native;
namespace DevExpress.Diagram.Core {
	public class SnapLinesPresenter {
		IAdorner[] snapLineAdorners;
		public void DestroySnapLineAdorners() {
			if(snapLineAdorners != null) {
				snapLineAdorners.ForEach(x => x.Destroy());
				snapLineAdorners = null;
			}
		}
		public void RecreateSnapLineAdorners(IDiagramControl diagram, AxisLine[] snapLines) {
			DestroySnapLineAdorners();
			snapLineAdorners = snapLines.Select(snapLine => {
				var adorner = CreateSnapLineAdorner(diagram.AdornerFactory(), snapLine);
				adorner.Bounds = new Rect(snapLine.From, snapLine.To);
				return adorner;
			}).ToArray();
		}
		static IAdorner CreateSnapLineAdorner(IAdornerFactory adornerFactory, AxisLine snapLine) {
			if(snapLine is BoundsSnapLine) {
				if(snapLine.Orientation == Orientation.Horizontal)
					return adornerFactory.CreateHBoundsSnapLine((BoundsSnapLine)snapLine);
				else
					return adornerFactory.CreateVBoundsSnapLine((BoundsSnapLine)snapLine);
			}
			if(snapLine is SizeSnapLine) {
				if(snapLine.Orientation == Orientation.Horizontal)
					return adornerFactory.CreateHSizeSnapLine((SizeSnapLine)snapLine);
				else
					return adornerFactory.CreateVSizeSnapLine((SizeSnapLine)snapLine);
			}
			throw new InvalidOperationException();
		}
	}
	public static class ResizeState {
		public static InputState Create<TResizable>(IMouseArgs mouseArgs,
			DiagramCursor cursor,
			Func<IMouseArgs, SnapInfo> getSnapInfo,
			MouseButton button,
			IEnumerable<IDiagramItem> items,
			IDiagramControl diagram,
			SizeInfo<TResizable>[] resizeables,
			Func<TResizable, Rect> getBounds,
			Func<Point, SnapInfo, SizeInfo<TResizable>[], AxisLine[]> resizeCore,
			Action onEscape,
			Action<IMouseArgs> onMouseUp) {
			var snapLinesPresenter = new SnapLinesPresenter();
			var shadows = items.Select(x => diagram.CreateShadow()).ToArray();
			Action<IMouseArgs> resize = e => {
				AxisLine[] snapLines = resizeCore(e.Position, getSnapInfo(e), resizeables);
				snapLinesPresenter.RecreateSnapLineAdorners(diagram, snapLines);
				resizeables.ForEach(shadows, (resizeable, shadow) => shadow.Bounds = getBounds(resizeable.Item));
			};
			return DragMouseActiveInputState.Create(diagram, cursor, button, mouseArgs,
				onMouseMove: resize,
				onMouseUp: onMouseUp,
				onEscape: () => {
					onEscape();
					shadows.Destroy();
					snapLinesPresenter.DestroySnapLineAdorners();
				},
				onInit: e => { },
				onModifiersChanged: resize);
		}
		public static InputState CreateResizeLiveState(IDiagramControl diagram, ResizeMode mode, IMouseArgs mouseArgs, IEnumerable<IDiagramItem> items, Func<IMouseArgs, SnapInfo> getSnapInfo, MouseButton button, double rotationAngle) {
			var transaction = new Transaction();
			var startPosition = mouseArgs.Position;
			return ResizeState.Create(mouseArgs, mode.Rotate(rotationAngle).GetCursor(), getSnapInfo, button, items, diagram, SizeInfo.GetSizeInfo(items, mode),
				getBounds: x => x.RotatedDiagramBounds().RotatedRect,
				resizeCore: (position, snapInfo, resizeables) => diagram.ResizeSelection(startPosition, position, mode, transaction, resizeables, snapInfo, rotationAngle),
				onEscape: () => transaction.Rollback(),
				onMouseUp: e => diagram.UndoManager().Commit(transaction, allowMerge: false));
		}
		public static InputState CreateResizePreviewState(IDiagramControl diagram, ResizeMode mode, IMouseArgs mouseArgs, SizeInfo<IAdorner>[] adorners, IEnumerable<IDiagramItem> items, Action onResize, Action onFinish, Func<IMouseArgs, SnapInfo> getSnapInfo, MouseButton button, double rotationAngle) {
			var startPosition = mouseArgs.Position;
			return ResizeState.Create(mouseArgs, mode.Rotate(rotationAngle).GetCursor(), getSnapInfo, button, items, diagram, adorners,
				getBounds: x => x.RotatedBounds().RotatedRect,
				resizeCore: (position, snapInfo, resizeables) => {
					var snapLines = diagram.ResizeAdorners(startPosition, position, mode, resizeables, snapInfo, rotationAngle);
					onResize();
					return snapLines;
				},
				onEscape: () => {
					onFinish();
					diagram.Selection().UpdateSelectionAdorners();
				},
				onMouseUp: e => diagram.UndoManager().Execute(t => diagram.ResizeSelection(startPosition, e.Position, mode, t, SizeInfo.GetSizeInfo(items, mode), getSnapInfo(e), rotationAngle), allowMerge: false));
		}
	}
}
