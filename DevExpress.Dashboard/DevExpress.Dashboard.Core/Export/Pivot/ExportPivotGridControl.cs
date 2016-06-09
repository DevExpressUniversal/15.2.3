#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       Dashboard                                                   }
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
using System.Drawing;
using DevExpress.DashboardCommon.Viewer;
using DevExpress.DashboardCommon.ViewModel;
using DevExpress.PivotGrid.Internal.ThinClientDataSource;
using DevExpress.XtraPivotGrid;
using DevExpress.XtraPivotGrid.Data;
namespace DevExpress.DashboardExport {
	public class ExportPivotGridControl : IDisposable, IPivotGridControl {
		readonly PivotGridData pivotData = new PivotGridData();
		PivotGridThinClientData data;
		event EventHandler<PivotCellDisplayTextEventArgsBase> customCellDisplayTextEventHandler;
		event EventHandler<PivotFieldDisplayTextEventArgsBase> fieldValueDisplayTextEventHandler;
		event EventHandler<PivotCustomDrawCellEventArgsBase> customDrawCellEventHandler;
		public PivotGridData PivotData { get { return pivotData; } }
		public PivotGridThinClientData Data {
			get { return data; }
			set { data = value; }
		}
		IEnumerable<IPivotGridField> IPivotGridControl.Fields {
			get {
				List<IPivotGridField> fields = new List<IPivotGridField>();
				foreach(PivotGridFieldBase field in pivotData.Fields)
					fields.Add(new ExportPivotGridFieldWrapper(field));
				return fields;
			}
		}
		bool IPivotGridControl.ShowColumnGrandTotals { get; set; }
		bool IPivotGridControl.ShowRowGrandTotals { get; set; }
		bool IPivotGridControl.ShowColumnTotals { get; set; }
		bool IPivotGridControl.ShowRowTotals { get; set; }
		event EventHandler<PivotCellDisplayTextEventArgsBase> IPivotGridControl.CustomCellDisplayText {
			add { customCellDisplayTextEventHandler += value; }
			remove { customCellDisplayTextEventHandler -= value; }
		}
		event EventHandler<PivotFieldDisplayTextEventArgsBase> IPivotGridControl.FieldValueDisplayText {
			add { fieldValueDisplayTextEventHandler += value; }
			remove { fieldValueDisplayTextEventHandler -= value; }
		}
		event EventHandler<PivotCustomDrawCellEventArgsBase> IPivotGridControl.CustomDrawCell {
			add { customDrawCellEventHandler += value; }
			remove { customDrawCellEventHandler -= value; }
		}
		event EventHandler<PivotFieldValueCollapseStateChangedEventArgs> IPivotGridControl.FieldValueCollapseStateChanged {
			add { }
			remove { }
		}
		public ExportPivotGridControl() {
			ExportPivotDataEventsImplementor implementor = new ExportPivotDataEventsImplementor(pivotData);
			implementor.RequestFieldValueDisplayText += OnRequestFieldValueDisplayText;
			implementor.RequestCustomCellDisplayText += OnRequestCustomCellDisplayText;
			pivotData.EventsImplementor = implementor;
		}
		public void DataBind() {
			PivotGridThinClientDataSource oldDataSource = pivotData.PivotDataSource as PivotGridThinClientDataSource;
			if(oldDataSource != null)
				((IDisposable)oldDataSource).Dispose();
			pivotData.PivotDataSource = new PivotGridThinClientDataSource(data);
		}
		void OnRequestFieldValueDisplayText(object sender, ExportFieldValueDisplayTextEventArgsWrapper e) {
			if(fieldValueDisplayTextEventHandler != null)
				fieldValueDisplayTextEventHandler.Invoke(this, e);
		}
		void OnRequestCustomCellDisplayText(object sender, PivotCellDisplayTextEventArgsBase e) {
			if(customCellDisplayTextEventHandler != null)
				customCellDisplayTextEventHandler.Invoke(this, e);
		}
		public void OnRequestCustomDrawCell(object sender, PivotCustomDrawCellEventArgsBase e) {
			if(customDrawCellEventHandler != null)
				customDrawCellEventHandler.Invoke(this, e);
		}
		public void Dispose() {
			Dispose(true);
		}
		protected virtual void Dispose(bool disposing) {
			if(disposing)
				pivotData.Dispose();
		}
		void IPivotGridControl.SetData(PivotGridThinClientData  data) {
			this.data = data;
		}
		void IPivotGridControl.BeginUpdate() {
			pivotData.BeginUpdate();
		}
		void IPivotGridControl.EndUpdate() {
			pivotData.EndUpdate();
		}
		void IPivotGridControl.ClearFields() {
			pivotData.Fields.Clear();
		}
		IPivotGridField IPivotGridControl.AddField(string fieldName, PivotColumnViewModel column, PivotDashboardItemArea area) {
			PivotGridFieldBase field = new PivotGridFieldBase(fieldName, PivotDashboardItemViewControl.GetPivotArea(area));
			PivotFieldExportHelper.SetupFieldFormat(field, column.Format);
			field.Caption = column.Caption;
			pivotData.Fields.Add(field);
			return new ExportPivotGridFieldWrapper(field);
		}
	}
	public class ExportPivotGridFieldWrapper : IPivotGridField {
		readonly PivotGridFieldBase field;
		PivotDashboardItemArea IPivotGridField.Area {
			get { return PivotDashboardItemViewControl.GetPivotGridArea(field.Area); }
		}
		int IPivotGridField.AreaIndex {
			get { return field.AreaIndex; }
		}
		string IPivotGridField.FieldName {
			get { return field.FieldName; }
		}
		int IPivotGridField.Width {
			get { return field.Width; }
			set { field.Width = value; }
		}
		public ExportPivotGridFieldWrapper(PivotGridFieldBase field) {
			this.field = field;			
		}
	}
}
