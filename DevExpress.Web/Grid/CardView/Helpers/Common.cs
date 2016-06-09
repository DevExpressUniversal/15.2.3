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
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Web.UI.WebControls;
namespace DevExpress.Web.Internal {
	public class CardViewColumnHelper : GridColumnHelper {
		static ReadOnlyCollection<IWebGridColumn> EmptyBandsCollection = new List<IWebGridColumn>().AsReadOnly();
		ReadOnlyCollection<IWebGridColumn> leafsForCustWindow;
		ReadOnlyCollection<CardViewColumnLayoutItem> columnLayoutItems;
		public CardViewColumnHelper(ASPxCardView grid)
			: base(grid) {
		}
		public new ASPxCardView Grid { get { return base.Grid as ASPxCardView; } }
		public override ReadOnlyCollection<IWebGridColumn> BandsForCustWindow { get { return EmptyBandsCollection; } }
		public override ReadOnlyCollection<IWebGridColumn> LeafsForCustWindow {
			get {
				if(leafsForCustWindow == null)
					leafsForCustWindow = AllVisibleDataColumns.OfType<IWebGridColumn>().Where(c => (c as CardViewColumn).ShowInCustomizationForm).ToList().AsReadOnly();
				return leafsForCustWindow;
			}
		}
		public ReadOnlyCollection<CardViewColumnLayoutItem> ColumnLayoutItems {
			get {
				if(columnLayoutItems == null)
					columnLayoutItems = CreateColumnLayoutItemsList().AsReadOnly();
				return columnLayoutItems;
			}
		}
		public override void Invalidate() {
			base.Invalidate();
			this.leafsForCustWindow = null;
			this.columnLayoutItems = null;
		}
		protected virtual List<CardViewColumnLayoutItem> CreateColumnLayoutItemsList() {
			var result = new List<CardViewColumnLayoutItem>();
			FormLayoutProperties prop = Grid.CardLayoutProperties;
			if(prop.Items.IsEmpty)
				prop = Grid.GenerateDefaultLayout(false);
			prop.ForEach((layoutItem) => {
				var columnLayoutItem = layoutItem as CardViewColumnLayoutItem;
				if(columnLayoutItem != null && columnLayoutItem.Column != null)
					result.Add(columnLayoutItem);
			});
			return result;
		}
		protected override bool HasEditTemplate(IWebGridDataColumn column) {
			return (column as CardViewColumn).EditItemTemplate != null;
		}
	}
	public class CardViewBatchEditHelper : GridBatchEditHelper {
		public CardViewBatchEditHelper(ASPxCardView grid) 
			: base(grid) {
		}
		public new ASPxCardView Grid { get { return base.Grid as ASPxCardView; } }
		public new CardViewRenderHelper RenderHelper { get { return base.RenderHelper as CardViewRenderHelper; } }
		protected override bool HasEditItemTemplate(IWebGridDataColumn column) {
			var dataColumn = column as CardViewColumn;
			return dataColumn != null && dataColumn.EditItemTemplate != null;
		}
		public override void CreateTemplateEditor(IWebGridDataColumn column, WebControl container) {
			RenderHelper.AddEditItemTemplateControl(-1, column as CardViewColumn, container); 
		}
	}
}
