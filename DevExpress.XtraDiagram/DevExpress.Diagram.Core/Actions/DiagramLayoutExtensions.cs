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

using DevExpress.Diagram.Core.Layout;
using DevExpress.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
namespace DevExpress.Diagram.Core {
	public static class DiagramLayoutExtensions {
		const double treeLayoutHorizontalBuffer = 20.0d;
		const double treeLayoutVerticalBuffer = 20.0d;
		public static void LayoutTreeDiagram(this IDiagramControl diagram, TreeLayoutSettings settings) {
			var items = diagram.Items();
			var connectors = items.OfType<IDiagramConnector>().ToArray();
			var connections = connectors
				.Select(x => new Connection<IDiagramItem>(x.BeginItem, x.EndItem))
				.Concat(items.Where(item => !(item is IDiagramConnector) && !(item is IDiagramContainer)).Select(item => new Connection<IDiagramItem>(item, null)))
				;
			var positions = TreeLayout.LayoutGraph(connections, x => x.ActualSize, new GraphTreeLayoutSettings(settings, diagram.PageSize));
			var moveInfo = positions
				.Select(position => Item_Owner_Bounds.FromDiagramBounds(position.Item, position.Item.Owner(), position.Item.ActualBounds().SetLocation(position.Position)))
				.ToArray();
			diagram.MoveItemsCore(moveInfo);
		}
		public static void LayoutTreeDiagram(this IDiagramControl diagram, string direction) {
			diagram.LayoutTreeDiagram(new TreeLayoutSettings(treeLayoutHorizontalBuffer, treeLayoutVerticalBuffer, (Direction)Enum.Parse(typeof(Direction), direction)));
		}
	}
}
