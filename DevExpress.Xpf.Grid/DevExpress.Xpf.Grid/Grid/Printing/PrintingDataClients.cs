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
using System.ComponentModel;
using DevExpress.Data;
using DevExpress.Data.Filtering;
#if SL
using DevExpress.Data.Browsing;
using PropertyDescriptor = DevExpress.Data.Browsing.PropertyDescriptor;
#endif
namespace DevExpress.Xpf.Grid.Printing {
	public class PrintingDataClient : IDataControllerData2 {
		IDataControllerData2 realClient;
		public PrintingDataClient(IDataControllerData2 realClient) {
			this.realClient = realClient;
		}
		#region IDataControllerData Members
		public UnboundColumnInfoCollection GetUnboundColumns() {
			return realClient.GetUnboundColumns();
		}
		public object GetUnboundData(int listSourceRow1, DataColumnInfo column, object value) {
			return realClient.GetUnboundData(listSourceRow1, column, value);
		}
		public void SetUnboundData(int listSourceRow1, DataColumnInfo column, object value) { }
		#endregion
		#region IDataControllerData2 Members
		public bool CanUseFastProperties {
			get { return realClient.CanUseFastProperties; }
		}
		public ComplexColumnInfoCollection GetComplexColumns() {
			return realClient.GetComplexColumns();
		}
		public void SubstituteFilter(SubstituteFilterEventArgs args) { }
		public bool HasUserFilter {
			get { return realClient.HasUserFilter; }
		}
		public bool? IsRowFit(int listSourceRow, bool fit) {
			return realClient.IsRowFit(listSourceRow, fit);
		}
		public PropertyDescriptorCollection PatchPropertyDescriptorCollection(PropertyDescriptorCollection collection) {
			return realClient.PatchPropertyDescriptorCollection(collection);
		}
		#endregion
	}
	public class PrintingVisualClient : IDataControllerVisualClient {
		IDataControllerVisualClient realClient;
		public PrintingVisualClient(IDataControllerVisualClient realClient) {
			this.realClient = realClient;
		}
		#region IDataControllerVisualClient Members
		public void ColumnsRenewed() { }
		public bool IsInitializing {
			get { return false; }
		}
		public int PageRowCount {
			get { return realClient.PageRowCount; }
		}
		public void RequestSynchronization() {
			realClient.RequestSynchronization();
		}
		public void RequireSynchronization(IDataSync dataSync) { }
		public int TopRowIndex {
			get { return realClient.TopRowIndex; }
		}
		public void UpdateColumns() { }
		public void UpdateLayout() { }
		public void UpdateRow(int controllerRowHandle) { }
		public void UpdateRowIndexes(int newTopRowIndex) { }
		public void UpdateRows(int topRowIndexDelta) { }
		public void UpdateScrollBar() { }
		public void UpdateTotalSummary() { }
		public int VisibleRowCount {
			get { return realClient.VisibleRowCount; }
		}
		#endregion
	}
}
