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
using IInputElement = DevExpress.Diagram.Core.IInputElement;
using DevExpress.Internal;
namespace DevExpress.Diagram.Core {
	public sealed class ItemToolDropItemState : MouseActiveInputState {
		public static InputState StartDrag(IDiagramControl diagram, IMouseArgs args, MouseButton button, ItemTool tool) {
			diagram.Controller.FocusSurface();
			args.Capture();
			var item = tool.CreateItem(diagram);
			item.Size = tool.DefaultItemSize;
			return CheckCanDrop(
				() => {
					var preview = item.Controller.CreateDragPreviewAdorner(diagram).AddShadow(diagram);
					return new ItemToolDropItemState(diagram, DiagramCursor.Add, preview, args, button, tool, item);
				}, diagram, args, button, tool, item);
		}
		static InputState CheckCanDrop(Func<InputState> getNormalState, IDiagramControl diagram, IMouseArgs args, MouseButton button, ItemTool tool, IDiagramItem item, Action clear = null) {
			return NoActionDragState.CheckNoAction(getNormalState, diagram, args, 
				(d, e) => StartDrag(d, e, button, tool),
				(d, m) => NoActionDragState.IsInBoundsAndCanAdd(d, m, item.Yield(), true),
				button, clear);
		}
		readonly IAdorner preview;
		readonly IDiagramItem item;
		readonly SnapLinesPresenter snapLinesPresenter = new SnapLinesPresenter();
		readonly ItemTool tool;
		ItemToolDropItemState(IDiagramControl diagram, DiagramCursor cursor, IAdorner preview, IMouseArgs args, MouseButton button, ItemTool tool, IDiagramItem item)
			: base(diagram, cursor, button) {
			this.preview = preview;
			this.item = item;
			this.tool = tool;
			UpdatePreview(args);
		}
		protected override InputState MouseMove(IMouseArgs mouseArgs) {
			return CheckCanDrop(
				() => {
					UpdatePreview(mouseArgs);
					return base.MouseMove(mouseArgs);
				}, diagram, mouseArgs, button, tool, this.item, clear: DestroyAdorners);
		}
		protected override InputState ModifiersChanged(IMouseArgs mouseArgs) {
			UpdatePreview(mouseArgs);
			return base.ModifiersChanged(mouseArgs);
		}
		protected override InputState MouseUp(IMouseButtonArgs mouseArgs) {
			diagram.DrawItemAtPoint(this.item, mouseArgs.Position, GetSnapResult(mouseArgs).Result);
			return Escape(mouseArgs);
		}
		protected override InputState Escape(IMouseArgs mouse) {
			DestroyAdorners();
			return base.Escape(mouse);
		}
		void DestroyAdorners() {
			preview.Destroy();
			snapLinesPresenter.DestroySnapLineAdorners();
		}
		SnapResult<Rect> GetSnapResult(IMouseArgs args) {
			var snapInfo = diagram.GetSnapInfo(item.Yield(), args.Position, isSnappingEnabled: args.IsSnappingEnabled(), allowSnapToItems: true);
			var snapResult = snapInfo.SnapRectLocation(args.Position.GetCenteredRect(tool.DefaultItemSize));
			return snapResult;
		}
		void UpdatePreview(IMouseArgs args) {
			var snapResult = GetSnapResult(args);
			preview.Bounds = snapResult.Result;
			snapLinesPresenter.RecreateSnapLineAdorners(diagram, snapResult.SnapLines);
		}
	}
}
