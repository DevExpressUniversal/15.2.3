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
namespace DevExpress.XtraSpreadsheet.Utils.Trees {
	partial class RTree2D<T> {
		class Node : NodeBase {
			readonly bool isLeaf;
			readonly List<Node> children;
			Node parent;
			public Node(int columnIndex, int rowIndex, int width, int height, bool isLeaf)
				: base(columnIndex, rowIndex, width, height) {
				this.children = new List<Node>();
				this.isLeaf = isLeaf;
			}
			public List<Node> Children { get { return children; } }
			public bool IsLeaf { get { return isLeaf; } }
			public Node Parent { get { return parent; } set { parent = value; } }
#if DEBUG
			public override string ToString() {
				string isleafLabel = isLeaf ? "L, Childrens: " : "N, Childrens: ";
				var topLeft = new Model.CellPosition(this.ColumnIndex, this.RowIndex);
				var bottomRight = new Model.CellPosition(this.LastColumnIndex, this.LastRowIndex);
				Model.CellReferenceParser.ToString(topLeft);
				string areaStr = String.Concat(topLeft.ToString(), "_:_", bottomRight.ToString());
				return String.Concat("L: ", isleafLabel, children.Count.ToString(), ", Area: ", areaStr);
			}
#endif
		}
	}
}
