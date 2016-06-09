#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       eXpressApp Framework                                        }
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

using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Web.Layout;
using DevExpress.Web;
namespace DevExpress.ExpressApp.Web.Editors.ASPx {
	public class GridViewBandColumnInfo : IColumnInfo {
		IModelBand modelBand;
		public GridViewBandColumnInfo(IModelBand modelBand) {
			this.modelBand = modelBand;
		}
		public IModelBand Model {
			get { return modelBand; }
		}
		protected virtual void GridViewBandColumnApplyModel(GridViewBandColumn column) { }
		protected virtual void GridViewBandColumnSynchronizeModel(GridViewBandColumn column) { }
		protected virtual WebColumnBaseColumnWrapper CreateColumnWrapper(WebColumnBase column) {
			return new ASPxGridViewBandColumnWrapper((GridViewBandColumn)column, this);
		}
		IModelLayoutElement IColumnInfo.Model {
			get { return modelBand; }
		}
		void IColumnInfo.ApplyModel(WebColumnBase column) {
			string columnCaption = modelBand.Caption;
			if(WebApplicationStyleManager.IsNewStyle && WebApplicationStyleManager.GridColumnsUpperCase && columnCaption != null) {
				column.Caption = columnCaption.ToUpper();
			}
			else {
				column.Caption = columnCaption;
			}
			((GridViewBandColumn)column).HeaderStyle.HorizontalAlign = System.Web.UI.WebControls.HorizontalAlign.Center;
			GridViewBandColumnApplyModel((GridViewBandColumn)column);
		}
		void IColumnInfo.SynchronizeModel(WebColumnBase column) {
			GridViewBandColumnSynchronizeModel((GridViewBandColumn)column);
		}
		WebColumnBaseColumnWrapper IColumnInfo.CreateColumnWrapper(WebColumnBase column) {
			return CreateColumnWrapper(column);
		}
	}
}
