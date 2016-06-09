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

using System.Collections.Generic;
using DevExpress.Data;
namespace DevExpress.XtraSpreadsheet {
	public class DataControllerState {
		#region fields
		object dataSource;
		string dataMember;
		int currentRow;
		int lastRow;
		bool isGroupHeader;
		bool isGroupFooter;
		List<GroupInfo> currentGroupInfo;
		FilterInfo filterInfo;
		#endregion
		public DataControllerState(BaseGridController controller, bool isGroupHeader, bool isGroupFooter, List<GroupInfo> currentGroupInfo, FilterInfo filterInfo, int lastRow) {
			this.dataSource = controller.DataSource;
			this.dataMember = controller.DataMember;
			this.currentRow = controller.CurrentControllerRow;
			this.lastRow = lastRow;
			this.isGroupHeader = isGroupHeader;
			this.isGroupFooter = isGroupFooter;
			this.currentGroupInfo = currentGroupInfo;
			this.filterInfo = filterInfo;
		}
		#region Properties
		public object DataSource { get { return dataSource; } }
		public string DataMember { get { return dataMember; } }
		public int CurrentRow { get { return currentRow; } }
		public int LastRow { get { return lastRow; } }
		public bool IsGroupHeader { get { return isGroupHeader; } }
		public bool IsGroupFooter { get { return isGroupFooter; } }
		public List<GroupInfo> CurrentGroupInfo { get { return currentGroupInfo; } }
		public FilterInfo FilterInfo { get { return filterInfo; } }
		#endregion
	}
}
