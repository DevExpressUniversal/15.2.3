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

using System;
using System.ComponentModel;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp.Win.Editors;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Views.Base;
namespace DevExpress.ExpressApp.Win.Core.ModelEditor.InternalDesigner {
	[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
	internal class DesignXafColumnViewActiveFilterStringAdapter : IColumnViewActiveFilterStringAdapter {
		private ColumnView view;
		private GridControl grid;
		private string activeFilterString;
		public DesignXafColumnViewActiveFilterStringAdapter(GridControl grid, ColumnView view) {
			this.view = view;
			this.grid = grid;
			((IFilteredComponentBase)grid).RowFilterChanged += FilteredComponentBase_RowFilterChanged;
		}
		private void FilteredComponentBase_RowFilterChanged(object sender, EventArgs e) {
			((IFilteredComponentBase)grid).RowFilterChanged -= FilteredComponentBase_RowFilterChanged;
			ActiveFilterString = CriteriaOperator.ToString(view.ActiveFilterCriteria);
			((IFilteredComponentBase)grid).RowFilterChanged += FilteredComponentBase_RowFilterChanged;
		}
		public string ActiveFilterString {
			get { return activeFilterString; }
			set {
				activeFilterString = value;
				if(ActiveFilterStringChanged != null) {
					ActiveFilterStringChanged(this, EventArgs.Empty);
				}
				view.ActiveFilterString = value;
			}
		}
		public event EventHandler<EventArgs> ActiveFilterStringChanged;
	}
}
