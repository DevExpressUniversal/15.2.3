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
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Templates;
using DevExpress.ExpressApp.Chart;
using DevExpress.XtraCharts;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.Data;
namespace DevExpress.ExpressApp.Chart {
	public abstract class ChartListEditorBase : ListEditor, IControlOrderProvider {
		public static string Alias = "Chart";
		private IList chartDataSource;
		protected ChartControlContainer chartControlContainer;
		protected object chartControl;
		private void ReleaseObjects() {
			chartControl = null;
			if(chartControlContainer != null) {
				chartControlContainer.Dispose();
				chartControlContainer = null;
			}
		}
		private void UpdateControlDataSource() {
			if(chartControlContainer != null) {
				if(List != null) {
					if(List is IListServer) {
						throw new Exception("The ChartListEditor doesn't support Server Mode and so cannot use an IListServer object as the data source.");
					}
					else {
						chartDataSource = List;
					}
					chartControlContainer.EditValue = new ChartSource(chartDataSource, Model);
					if(ControlDataSourceChanged != null) {
						ControlDataSourceChanged(this, EventArgs.Empty);
					}
				}
				else {
				}
			}
		}
		protected override void OnDataSourceChanged() {
			base.OnDataSourceChanged();
			UpdateControlDataSource();
		}
		protected override void OnControlsCreated() {
			base.OnControlsCreated();
			UpdateControlDataSource();
		}
		protected override void AssignDataSourceToControl(Object dataSource) {
		}
		protected abstract ChartControlContainer CreateChartControlContainerCore();
		protected override object CreateControlsCore() {
			chartControlContainer = CreateChartControlContainerCore();
			return chartControlContainer.Control;
		}
		public ChartListEditorBase(IModelListView model)
			: base(model) {
		}
		public override void Refresh() {
			UpdateControlDataSource();
		}
		public override void BreakLinksToControls() {
			ReleaseObjects();
			base.BreakLinksToControls();
		}
		public override IList GetSelectedObjects() {
			return new List<object>();
		}
		public override SelectionType SelectionType {
			get { return SelectionType.None; }
		}
		public override IContextMenuTemplate ContextMenuTemplate {
			get { return null; }
		}
		public object ChartControl {
			get { return chartControl; }
		}
		#region IControlOrderProvider Members
		public int GetIndexByObject(object obj) {
			IList objects = GetOrderedObjects();
			return objects.IndexOf(obj);
		}
		public object GetObjectByIndex(int index) {
			IList objects = GetOrderedObjects();
			if((index >= 0) && (index < objects.Count)) {
				return objects[index];
			}
			return null;
		}
		public IList GetOrderedObjects() {
			return chartDataSource;
		}
		#endregion
		public override void Dispose() {
			chartDataSource = null;
			ReleaseObjects();
			base.Dispose();
		}
		public event EventHandler<EventArgs> ControlDataSourceChanged;
	}
}
