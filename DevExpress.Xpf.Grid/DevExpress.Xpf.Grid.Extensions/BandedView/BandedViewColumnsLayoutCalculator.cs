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

using System.Collections.Generic;
using DevExpress.Xpf.Grid.Native;
namespace DevExpress.Xpf.Grid {
	public class BandedViewColumnsLayoutCalculator : AutoWidthColumnsLayoutCalculator {
		public BandedViewColumnsLayoutCalculator(GridViewInfo viewInfo)
			: base(viewInfo) {
		}
		public override void ApplyResize(BaseColumn resizeColumn, double newWidth, double maxWidth, double indentWidth, bool correctWidths) {
			ColumnBase column = (ColumnBase)resizeColumn;
			ColumnsLayoutControl c = BandedViewBehavior.GetColumnsLayoutControl(column);
			c.Resize(column, newWidth);
		}
		protected override void UpdateHasLeftRightSibling(IList<ColumnBase> columns) {
			for(int i = 0; i < columns.Count; i++) {
				BandedViewBehavior.GetIsRightColumn(columns[i]);
				columns[i].HasRightSibling = !BandedViewBehavior.GetIsRightColumn(columns[i]);
				columns[i].HasLeftSibling = !BandedViewBehavior.GetIsLeftColumn(columns[i]);
			}
		}
	}
	public class BandedViewLayoutCalculatorFactory : GridTableViewLayoutCalculatorFactory {
		public override ColumnsLayoutCalculator CreateCalculator(GridViewInfo viewInfo, bool autoWidth) {
			return new BandedViewColumnsLayoutCalculator(viewInfo);
		}
	}
}
