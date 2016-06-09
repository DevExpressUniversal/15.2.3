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

using System.ComponentModel;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Win.Model;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.BandedGrid;
namespace DevExpress.ExpressApp.Win.Editors {
	[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
	public class BandedGridColumnModelSynchronizer : GridColumnModelSynchronizer {
		public BandedGridColumnModelSynchronizer(IModelColumn modelColumn, ITypeInfo objectTypeInfo)
			: base(modelColumn, objectTypeInfo, false, false) {
		}
		public BandedGridColumnModelSynchronizer(IModelColumn modelColumn, ITypeInfo objectTypeInfo, bool isAsyncServerMode)
			: base(modelColumn, objectTypeInfo, isAsyncServerMode, false) {
		}
		public BandedGridColumnModelSynchronizer(IModelColumn modelColumn, ITypeInfo objectTypeInfo, bool isAsyncServerMode, bool isProtectedColumn)
			: base(modelColumn, objectTypeInfo, isAsyncServerMode, isProtectedColumn) {
		}
		public new IModelBandedColumnWin Model {
			get { return (IModelBandedColumnWin)base.Model; }
		}
		protected override void GridColumnApplyModel(GridColumn column) {
			base.GridColumnApplyModel(column);
			((BandedGridColumn)column).RowIndex = Model.RowIndex;
			((BandedGridColumn)column).RowCount = Model.BandColumnRowCount;
		}
		protected override void GridColumnSynchronizeModel(GridColumn column) {
			base.GridColumnSynchronizeModel(column);
			Model.RowIndex = ((BandedGridColumn)column).RowIndex;
			Model.BandColumnRowCount = ((BandedGridColumn)column).RowCount;
		}
	}
}
