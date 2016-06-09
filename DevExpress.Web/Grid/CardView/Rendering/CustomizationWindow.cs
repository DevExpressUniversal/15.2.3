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

using System.Web.UI.WebControls;
using DevExpress.Web.Internal;
namespace DevExpress.Web.Rendering {
	public class CardViewCustomizationWindow : GridCustomizationWindow {
		public CardViewCustomizationWindow(ASPxCardView grid)
			: base(grid) {
		}
		protected new CardViewRenderHelper RenderHelper { get { return (CardViewRenderHelper)base.RenderHelper; } }
		protected override TableCell CreateHeaderCell(IWebGridColumn column) {
			return new CardViewHeaderCell(RenderHelper, (CardViewColumn)column);
		}
	}
	public class CardViewHeaderCell : GridTableHeaderCell {
		public CardViewHeaderCell(CardViewRenderHelper renderHelper, CardViewColumn column)
			: base(renderHelper, column, GridHeaderLocation.Customization, false, false) {
		}
		public new CardViewColumn Column { get { return (CardViewColumn)base.Column; } }
		protected override bool IsClickable { get { return Column.IsClickable(); } }
		protected override System.Web.UI.Control CreateGridHeaderContent() {
			return new CardViewHtmlHeaderContent(Column, Location);
		}
	}
	public class CardViewHtmlHeaderContent : GridHtmlHeaderContent {
		public CardViewHtmlHeaderContent(CardViewColumn column, GridHeaderLocation location)
			: base(column.CardView.RenderHelper, column, location) {
		}
		public new CardViewColumn Column { get { return (CardViewColumn)base.Column; } }
		protected override bool ShowFilterButton { get { return Column.ColumnAdapter.HasFilterButton; } }
		protected override bool IsFilterActive { get { return Column.ColumnAdapter.IsFiltered; } }
	}
}
