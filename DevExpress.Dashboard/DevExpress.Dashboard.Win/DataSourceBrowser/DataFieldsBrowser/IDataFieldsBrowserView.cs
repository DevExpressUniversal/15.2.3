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

using DevExpress.DashboardCommon.Native;
using System;
using System.Collections;
namespace DevExpress.DashboardWin.Native {
	public interface IDataFieldsBrowserView {
		DataField SelectedDataField { get; }
		DataField FocusedDataField { get; }
		bool IsOlap { get; set; }
		bool Enabled { get; set; }
		bool GroupByTypeEnabled { get; set; }
		bool GroupByType { get; set; }
		bool SortAscending { get; set; }
		bool SortDescending { get; set; }
		bool RefreshButtonVisible { get; set; }
		DataFieldsBrowserDisplayMode DisplayMode { get; set; }
		event EventHandler GroupAndSortChanged;
		event EventHandler<RequestChildNodesEventArgs> RequestChildNodes;
		event EventHandler<DataFieldEventArgs> DataFieldDoubleClick;
		event EventHandler<DataFieldEventArgs> FocusedDataFieldChanged;
		event EventHandler RefreshFieldListClick;
		void ClearAndBuildNodes(DataSourceNodeBase dataSourceNode, bool expandAll, IServiceProvider provider);
		void BuildNodes(DataSourceNodeBase dataSourceNode, IServiceProvider provider);
		void SelectNode(string dataMember);
		void RestoreSelection(string dataMember);
	}
	public class RequestChildNodesEventArgs : EventArgs {
		public DataNode DataNode { get; private set; }
		public IList ChildNodes { get; set; }
		public RequestChildNodesEventArgs(DataNode dataNode) {
			DataNode = dataNode;
		}
	}
	public class DataFieldEventArgs : EventArgs {
		public DataField DataField { get; private set; }
		public DataFieldEventArgs(DataField dataField) {
			DataField = dataField;
		}
	}
}
