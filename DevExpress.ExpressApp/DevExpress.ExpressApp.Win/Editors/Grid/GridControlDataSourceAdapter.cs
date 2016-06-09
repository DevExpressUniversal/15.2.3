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
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using DevExpress.Data;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Filtering;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Model.NodeWrappers;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.ExpressApp.Templates;
using DevExpress.ExpressApp.Utils;
using DevExpress.ExpressApp.Win.Controls;
using DevExpress.ExpressApp.Win.Core;
using DevExpress.ExpressApp.Win.SystemModule;
using DevExpress.Persistent.Base;
using DevExpress.Utils;
using DevExpress.Utils.Design;
using DevExpress.Utils.Menu;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraPrinting;
using System.IO;
namespace DevExpress.ExpressApp.Win.Editors {
	public interface IGridControlDataSourceAdapter {
		object DataSource { get; set; }
		event EventHandler<EventArgs> ControlDataSourceChanging;
		event EventHandler<EventArgs> ControlDataSourceChanged;
	}
	public class XafGridControlDataSourceAdapter : IGridControlDataSourceAdapter {
		private GridControl grid;
		private object dataSource;
		private void grid_HandleCreated(object sender, EventArgs e) {
			TryAssignDataSourceToGridControl();
		}
		private void grid_VisibleChanged(object sender, EventArgs e) {
			if(!((Control)sender).Disposing) {
				TryAssignDataSourceToGridControl();
			}
		}
		private static bool NeedUpdateGridDataSource(GridControl grid, object newDataSource) {
			return grid.DataSource != newDataSource && grid.Visible && grid.IsHandleCreated;
		}
		private void AssignDataSourceToGridControl(object dataSource) {
			OnControlDataSourceChanging();
			grid.DataSource = dataSource;
			OnControlDataSourceChanged();
		}
		private void TryAssignDataSourceToGridControl() {
			if(NeedUpdateGridDataSource(grid, dataSource)) {
				AssignDataSourceToGridControl(dataSource);
			}
		}
		protected virtual void OnControlDataSourceChanging() {
			if(ControlDataSourceChanging != null) {
				ControlDataSourceChanging(this, EventArgs.Empty);
			}
		}
		protected virtual void OnControlDataSourceChanged() {
			if(ControlDataSourceChanged != null) {
				ControlDataSourceChanged(this, EventArgs.Empty);
			}
		}
		public XafGridControlDataSourceAdapter(GridControl grid) {
			this.grid = grid;
			grid.VisibleChanged += grid_VisibleChanged;
			grid.HandleCreated += grid_HandleCreated;
		}
		public object DataSource {
			get {
				return dataSource;
			}
			set {
				dataSource = value;
				if(grid.DataSource == dataSource) {
					return; 
				}
				if(!NeedUpdateGridDataSource(grid, dataSource) && (grid.DataSource != null)) {
					AssignDataSourceToGridControl(null);
				}
				else {
					TryAssignDataSourceToGridControl();
				}
			}
		}
		public event EventHandler<EventArgs> ControlDataSourceChanging;
		public event EventHandler<EventArgs> ControlDataSourceChanged;
	}
}
