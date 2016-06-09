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
using System.Windows.Input;
using DevExpress.Diagram.Core.Localization;
using DevExpress.Utils;
namespace DevExpress.Diagram.Core {
	public abstract class DiagramTool : ImmutableObject {
		public abstract string ToolName { get; }
		public abstract string ToolId { get; }
		public virtual bool ShowFullSelectionUI { get { return true; } }
		public abstract DiagramCursor GetDefaultInputStateCursor(IDiagramControl diagram, IInputElement item, IMouseArgs mouseArgs);
		public abstract InputState CreateActiveInputState(IDiagramControl diagram, IInputElement item, IMouseButtonArgs mouseArgs);
		public override string ToString() {
			return string.Format(DiagramControlLocalizer.GetString(DiagramControlStringId.ToolDisplayFormat), ToolName);
		}
		public virtual void DefaultAction(IDiagramControl diagram) { }
		public virtual InputState StartDrag(InputState state, IDiagramControl diagram, IMouseArgs args, MouseButton button) {
			return state;
		}
		public virtual MouseMoveFeedbackHelper CreateFeedbackHelper(IDiagramControl diagram) {
			return MouseMoveFeedbackHelper.Empty;
		}
	}
	public class PointerTool : DiagramTool {
		public override InputState CreateActiveInputState(IDiagramControl diagram, IInputElement item, IMouseButtonArgs mouseArgs) {
			return item.CreatePointerToolMousePressedState(diagram, mouseArgs);
		}
		public override DiagramCursor GetDefaultInputStateCursor(IDiagramControl diagram, IInputElement item, IMouseArgs mouseArgs) {
			return item.GetPointerToolCursor(mouseArgs);
		}
		public override string ToolName { get { return DiagramControlLocalizer.GetString(DiagramControlStringId.Tool_Pointer); } }
		public override string ToolId { get { return "PointerTool"; } }
	}
	public class ConnectorTool : DiagramTool {
		public override bool ShowFullSelectionUI { get { return false; } }
		public override InputState CreateActiveInputState(IDiagramControl diagram, IInputElement item, IMouseButtonArgs mouseArgs) {
			return item.CreateConnectorToolMousePressedState(diagram, mouseArgs);
		}
		public override DiagramCursor GetDefaultInputStateCursor(IDiagramControl diagram, IInputElement item, IMouseArgs mouseArgs) {
			return item.GetConnectorToolCursor(diagram, mouseArgs);
		}
		public override MouseMoveFeedbackHelper CreateFeedbackHelper(IDiagramControl diagram) {
			return diagram.CreateMoveConnectorPointFeedbackHelper();
		}
		public override string ToolName { get { return DiagramControlLocalizer.GetString(DiagramControlStringId.Tool_Connector); } }
		public override string ToolId { get { return "ConnectorTool"; } }
	}
	public abstract class ItemTool : DiagramTool {
		public static readonly Size DefaultSize = new Size(100, 75);
		public ItemTool() {
		}
		public virtual Size DefaultItemSize { get { return DefaultSize; } }
		public virtual bool IsQuick { get { return false; } }
		public override InputState CreateActiveInputState(IDiagramControl diagram, IInputElement item, IMouseButtonArgs mouseArgs) {
			return item.CreateItemToolMousePressedState(diagram, mouseArgs, this);
		}
		public override DiagramCursor GetDefaultInputStateCursor(IDiagramControl diagram, IInputElement item, IMouseArgs mouseArgs) {
			return item.GetItemToolCursor(diagram, mouseArgs);
		}
		public override void DefaultAction(IDiagramControl diagram) {
			diagram.DrawItem(this);
		}
		public sealed override InputState StartDrag(InputState state, IDiagramControl diagram, IMouseArgs args, MouseButton button) {
			return ItemToolDropItemState.StartDrag(diagram, args, button, this);
		}
		public IDiagramItem CreateItem(IDiagramControl diagram) {
			var item = CreateItemCore(diagram);
			diagram.Controller.UpdateItemCustomStyle(item);
			return item;
		}
		protected abstract IDiagramItem CreateItemCore(IDiagramControl diagram);
	}
	public class ShapeTool : ItemTool {
		public ShapeDescription Shape { get; private set; }
		public sealed override string ToolName { get { return Shape.ToString(); } }
		public sealed override string ToolId { get { return Shape.Id; } }
		public override Size DefaultItemSize { get { return Shape.DefaultSize; } }
		public override bool IsQuick { get { return Shape.IsQuick; } }
		public ShapeTool(ShapeDescription shape) {
			this.Shape = shape;
		}
		protected override IDiagramItem CreateItemCore(IDiagramControl diagram) {
			var shape = diagram.CreateShape();
			shape.Shape = Shape;
			return shape;
		}
	}
	public class FactoryItemTool : ItemTool {
		public static ItemTool CreateContainerTool() {
			var toolId = "ContainerTool";
			Func<string> toolNameFunc = () => DiagramControlLocalizer.GetString(DiagramControlStringId.Tool_Container);
			return new FactoryItemTool(toolId, toolNameFunc, diagram => diagram.CreateContainer(), ItemTool.DefaultSize, false);
		}
		static IDiagramItem CreateShape(IDiagramControl diagram, ShapeDescription shape) {
			var diagramShape = diagram.CreateShape();
			diagramShape.Shape = shape;
			return diagramShape;
		}
		readonly string id;
		readonly Func<string> getName;
		readonly Func<IDiagramControl, IDiagramItem> createItem;
		readonly Size defaultSize;
		readonly bool isQuick;
		public sealed override string ToolId { get { return id; } }
		public sealed override string ToolName { get { return getName(); } }
		public sealed override Size DefaultItemSize { get { return defaultSize; } }
		public sealed override bool IsQuick { get { return isQuick; } }
		public FactoryItemTool(string id, Func<string> getName, Func<IDiagramControl, IDiagramItem> createItem, Size defaultSize, bool isQuick) {
			this.id = id;
			this.getName = getName;
			this.createItem = createItem;
			this.defaultSize = defaultSize;
			this.isQuick = isQuick;
		}
		protected sealed override IDiagramItem CreateItemCore(IDiagramControl diagram) {
			if(createItem != null)
				return createItem(diagram);
			return null;
		}
	}
}
