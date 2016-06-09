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

using DevExpress.DashboardCommon;
using DevExpress.DashboardCommon.Native;
using System;
using System.Drawing;
namespace DevExpress.DashboardWin.Native {
	public interface IDraggableDataFieldsBrowserView {
		event EventHandler<DataFieldStartDragEventArgs> DataFieldStartDrag;
		event EventHandler CalculatedFieldAdd;
		event EventHandler<DataFieldEventArgs> DataFieldDelete;
		event EventHandler<DataFieldEventArgs> DataFieldRename;
		event EventHandler<DataFieldBeginRenameEventArgs> DataFieldRenamed;
		event EventHandler DataFieldAfterRenamed;
		event EventHandler<DataFieldEventArgs> DataFieldEdit;
		event EventHandler<CalculatedFieldEditTypeEventArgs> CalculatedFieldEditType;
		event EventHandler<DataFieldActionsAvailabilityEventArgs> DataFieldActionsAvailability;
		void RenameSelectedDataField();
	}
	public class DataFieldStartDragEventArgs : DataFieldEventArgs {
		public Point StartScreenPt { get; private set; }
		public DataFieldStartDragEventArgs(DataField dataField, Point startScreenPt)
			: base(dataField) {
			StartScreenPt = startScreenPt;
		}
	}
	public class DataFieldBeginRenameEventArgs : DataFieldEventArgs {
		public string NewName { get; private set; }
		public DataFieldBeginRenameEventArgs(DataField dataField, string newName)
			: base(dataField) {
			NewName = newName;
		}
	}
	public class CalculatedFieldEditTypeEventArgs : DataFieldEventArgs {
		public CalculatedFieldType NewType { get; private set;}
		public CalculatedFieldEditTypeEventArgs(DataField dataField, CalculatedFieldType newType)
			: base(dataField) {
			NewType = newType;
		}
	}
	public class DataFieldActionsAvailabilityEventArgs : DataFieldEventArgs {
		public bool AddCalculatedField { get; set; }
		public bool EditCalculatedFieldType { get; set; }
		public bool EditDataField { get; set; }
		public bool RenameDataField { get; set; }
		public bool DeleteDataField { get; set; }
		public CalculatedFieldType CalculatedFieldType { get; set; }
		public DataFieldActionsAvailabilityEventArgs(DataField dataField)
			: base(dataField) {
		}
	}
}
