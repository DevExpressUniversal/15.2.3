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
using System.Text;
using DevExpress.Data;
using DevExpress.Utils;
using System.ComponentModel.Design;
using DevExpress.XtraRichEdit.Utils;
using DevExpress.Data.Browsing;
using MergeFieldName = DevExpress.XtraRichEdit.API.Native.MergeFieldName;
using DevExpress.Compatibility.System.Windows.Forms;
using DevExpress.Compatibility.System.ComponentModel;
#if !SL
using System.Windows.Forms;
using System.ComponentModel;
#endif
namespace DevExpress.XtraRichEdit.Native {
	public class RichEditDataControllerAdapter : RichEditDataControllerAdapterBase, IDataControllerCurrentSupport, IDataControllerData2 {
		#region Fields
		BaseGridController dataController;
		readonly Dictionary<string, bool> complexColumnNames;
		#endregion
		public RichEditDataControllerAdapter(BaseGridController dataController) {
			Guard.ArgumentNotNull(dataController, "dataController");
			this.complexColumnNames = new Dictionary<string, bool>();
			this.dataController = dataController;
			InitializeDataController();
		}
		#region Properties
		protected internal virtual BaseGridController DataController { get { return dataController; } }
		public override bool IsReady { get { return DataController.IsReady; } }
		public override int ListSourceRowCount { get { return DataController.ListSourceRowCount; } }
		public override int CurrentControllerRow { get { return DataController.CurrentControllerRow; } set { DataController.CurrentControllerRow = value; } }
		public override object DataSource { get { return DataController.DataSource; } set { DataController.DataSource = value; } }
		public override string DataMember { get { return DataController.DataMember; } set { DataController.DataMember = value; } }
		#endregion
		protected internal virtual void ReplaceDataController(BaseGridController controller) {
			if (DataController != null)
				DisposeDataController();
			this.dataController = controller;
			InitializeDataController();
		}
		protected internal virtual void InitializeDataController() {
			this.DataController.CurrentClient = this;
			this.dataController.DataClient = this;
			SubscribeDataControllerEvents();
		}
		protected internal virtual void DisposeDataController() {
			UnsubscribeDataControllerEvents();
			DataController.Dispose();
		}
#if !SL
		public virtual void SetBindingContext(BindingContext bindingContext) {
			DataController.SetDataSource(bindingContext, DataSource, DataMember);
		}
#endif
		public override MergeFieldName[] GetColumnNames() {
			DataColumnInfoCollection columns = DataController.Columns;
			int count = columns.Count;
			List<MergeFieldName> result = new List<MergeFieldName>();
			for (int i = 0; i < count; i++) {
				if (columns[i].Visible)
					result.Add(new MergeFieldName(columns[i].Name, columns[i].Caption));
			}
			return result.ToArray();
		}
		public override int GetColumnIndex(string name) {
			int index = DataController.Columns.GetColumnIndex(name);
			if (index < 0) {
				if (complexColumnNames.ContainsKey(name))
					return -1;
				complexColumnNames.Add(name, true);
				DataController.RePopulateColumns();
				index = DataController.Columns.GetColumnIndex(name);
			}
			return index;
		}
		public override object GetCurrentRowValue(int columnIndex) {
			return DataController.GetCurrentRowValue(columnIndex);
		}
		public override object GetCurrentRow() {
			return DataController.CurrentControllerRowObject;
		}
		protected virtual void ListSourceChanged(object sender, EventArgs e) {
			RaiseDataSourceChanged();
		}
		protected virtual void SubscribeDataControllerEvents() {
			this.DataController.ListSourceChanged += ListSourceChanged;
		}
		protected virtual void UnsubscribeDataControllerEvents() {
			this.DataController.ListSourceChanged -= ListSourceChanged;
		}
		protected override void Dispose(bool disposing) {
			if (disposing) {
				if (DataController != null) {
					DisposeDataController();
					dataController = null;
				}
			}
			base.Dispose(disposing);
		}
		#region IDataControllerCurrentSupport Members
		public void OnCurrentControllerRowChanged(CurrentRowEventArgs e) {
			RaiseCurrentRowChangedEvent();
		}
		public void OnCurrentControllerRowObjectChanged(CurrentRowChangedEventArgs e) {
			RaiseCurrentRowChangedEvent();
		}
		#endregion
		#region IDataControllerData2 Members
		bool IDataControllerData2.CanUseFastProperties {
			get {
				return true;
			}
		}
		ComplexColumnInfoCollection IDataControllerData2.GetComplexColumns() {
			ComplexColumnInfoCollection res = new ComplexColumnInfoCollection();
			foreach (string name in complexColumnNames.Keys) {
				if (name.Contains(".") && DataController.Columns[name] == null)
					res.Add(name);
			}
			return res;
		}
		void IDataControllerData2.SubstituteFilter(SubstituteFilterEventArgs args) { }
		bool IDataControllerData2.HasUserFilter {
			get { return false; }
		}
		bool? IDataControllerData2.IsRowFit(int listSourceRow, bool fit) {
			return null;
		}
		PropertyDescriptorCollection IDataControllerData2.PatchPropertyDescriptorCollection(PropertyDescriptorCollection collection) {
			if (collection == null)
				return collection;
			foreach (string name in complexColumnNames.Keys) {
				if (collection.Find(name, false) == null)
					collection.Find(name, true);
			}
			return collection;
		}
		#endregion
		#region IDataControllerData Members
		UnboundColumnInfoCollection IDataControllerData.GetUnboundColumns() {
			return null;
		}
		object IDataControllerData.GetUnboundData(int listSourceRow1, DataColumnInfo column, object value) {
			return null;
		}
		void IDataControllerData.SetUnboundData(int listSourceRow1, DataColumnInfo column, object value) {
		}
		#endregion
	}
}
