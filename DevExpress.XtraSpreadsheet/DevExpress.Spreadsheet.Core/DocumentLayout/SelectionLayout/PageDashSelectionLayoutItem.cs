﻿#region Copyright (c) 2000-2015 Developer Express Inc.
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
namespace DevExpress.XtraSpreadsheet.Layout {
	#region PageDashSelectionLayoutItem
	public class PageDashSelectionLayoutItem : RangeSelectionLayoutItemBase {
		List<RangeBorderDashSelectionLayoutItem> borderItems;
		public PageDashSelectionLayoutItem(SelectionLayout layout, CellPosition topLeft, CellPosition bottomRight)
			: base(layout, topLeft, bottomRight) {
			this.borderItems = new List<RangeBorderDashSelectionLayoutItem>();
		}
		public List<RangeBorderDashSelectionLayoutItem> BorderItems { get { return borderItems; } set { borderItems = value; } }
		public override void Draw(ISelectionPainter selectionPainter) {
			selectionPainter.Draw(this);
		}
		public override void Update(Page page) {
			base.Update(page);
			for (int i = 0; i < borderItems.Count; i++) {
				borderItems[i].Update(page);
			}
		}
		public void Invalidate() {
			for (int i = 0; i < borderItems.Count; i++) {
				borderItems[i] = null;
			}
		}
	}
	#endregion
}
