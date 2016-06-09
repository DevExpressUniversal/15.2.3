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

using System.Drawing;
using System.IO;
using DevExpress.Utils;
using System.Collections.Generic;
using DevExpress.XtraSpreadsheet.Model;
using DevExpress.Office.Layout;
using DevExpress.Office.Internal;
using DevExpress.XtraSpreadsheet.Internal;
using DevExpress.Office.Drawing;
using DevExpress.XtraSpreadsheet.Layout.Engine;
using DevExpress.Compatibility.System.Drawing;
namespace DevExpress.XtraSpreadsheet.Layout {
	#region RangeSelectionLayoutItemBase
	public abstract class RangeSelectionLayoutItemBase : ISelectionLayoutItem {
		#region Fields
		readonly SelectionLayout layout;
		CellPosition topLeft;
		CellPosition bottomRight;
		Rectangle bounds;
		#endregion
		protected RangeSelectionLayoutItemBase(SelectionLayout layout, CellPosition topLeft, CellPosition bottomRight) {
			Guard.ArgumentNotNull(layout, "layout");
			Guard.ArgumentNotNull(topLeft, "topLeft");
			Guard.ArgumentNotNull(bottomRight, "bottomRight");
			this.layout = layout;
			this.topLeft = topLeft;
			this.bottomRight = bottomRight;
		}
		#region Properties
		public SelectionLayout Layout { get { return layout; } }
		public Rectangle Bounds { get { return bounds; } }
		protected internal CellPosition TopLeft { get { return topLeft; } }
		protected internal CellPosition BottomRight { get { return bottomRight; } }
		#endregion
		public virtual void Update(Page page) {
			this.bounds = GetBounds(page);
		}
		Rectangle GetBounds(Page page) {
			return Layout.CalculateBounds(page, topLeft, bottomRight);
		}
		public void InflateBounds(int pixels) {
			this.bounds.Inflate(pixels, pixels);
		}
		public abstract void Draw(ISelectionPainter selectionPainter);
	}
	#endregion
}
