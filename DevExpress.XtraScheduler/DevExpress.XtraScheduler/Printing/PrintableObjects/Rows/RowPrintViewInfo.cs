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

using DevExpress.Utils.Drawing;
using DevExpress.XtraScheduler.Native;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
namespace DevExpress.XtraScheduler.Printing.Native {
	class RowPrintViewInfo : IPrintableObject {
		List<IPrintableObject> cells;
		Size minSize;
		Rectangle bounds;
		public RowPrintViewInfo() {
			cells = new List<IPrintableObject>();
		}
		public Size MinSize { get { return minSize; } }
		public Rectangle Bounds { get { return bounds; } set { bounds = value; } }
		public IPrintableObject this[int index] { get { return cells[index]; } }
		public int Count { get { return cells.Count; } }
		internal List<IPrintableObject> Cells { get { return cells; } }
		public void AddCell(IPrintableObject cell) {
			if (cell == null)
				Exceptions.ThrowArgumentNullException("cell");
			cells.Add(cell);
		}
		public void Print(GraphicsInfoArgs graphicsInfoArgs) {
			int count = cells.Count;
			for (int i = 0; i < count; i++)
				cells[i].Print(graphicsInfoArgs);
		}
		public Size Measure(GraphicsCache cache, int maxWidth) {
			int count = cells.Count;
			int remainWidth = maxWidth;
			for (int i = 0; i < count; i++) {
				int currentCellWidth = (int)cells[i].Measure(cache, remainWidth).Width;
				remainWidth -= currentCellWidth;
			}
			int width = 0;
			int height = 0;
			for (int i = 0; i < count; i++) {
				width += cells[i].MinSize.Width;
				height = Math.Max(height, cells[i].MinSize.Height);
			}
			minSize = new Size(width, height);
			return minSize;
		}
	}
}
