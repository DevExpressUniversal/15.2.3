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
using DevExpress.XtraEditors;
using DevExpress.XtraTreeList;
using DevExpress.XtraTreeList.Nodes;
using DevExpress.XtraTreeList.Columns;
namespace DevExpress.XtraSpreadsheet.Forms.Design {
	[DXToolboxItem(false)]
	public partial class CellReferenceAddressControl : XtraUserControl {
		public CellReferenceAddressControl() {
			InitializeComponent();
		}
		#region Properties
		public TreeListColumnCollection TreeListColumns { get { return tlCellReference.Columns; } }
		public TreeListNode TreeListFocusedNode { get { return tlCellReference.FocusedNode; } set { tlCellReference.FocusedNode = value; } }
		public override string Text { get { return teCellReference.Text; } set { teCellReference.Text = value; } }
		public bool TextEditEnabled { get { return teCellReference.Enabled; } set { teCellReference.Enabled = value; } }
		public XtraEditors.TextEdit TextEdit { get { return teCellReference; } }
		public int TextEditSelectionStart { get { return teCellReference.SelectionStart; } set { teCellReference.SelectionStart = value; } }
		#endregion
		#region Events
		public event FocusedNodeChangedEventHandler FocusedNodeChanged { add { tlCellReference.FocusedNodeChanged += value; } remove { tlCellReference.FocusedNodeChanged -= value; } }
		public event EventHandler EditValueChanged { add { teCellReference.EditValueChanged += value; } remove { teCellReference.EditValueChanged -= value; } }
		#endregion
		#region TreeListExpandAll
		public void TreeListExpandAll() {
			tlCellReference.ExpandAll();
		}
		#endregion
		#region TreeListBeginUnboundLoad
		public void TreeListBeginUnboundLoad() {
			tlCellReference.BeginUnboundLoad();
		}
		#endregion
		#region TreeListAppendNode
		public TreeListNode TreeListAppendNode(object nodeData, TreeListNode parentNode) {
			return tlCellReference.AppendNode(nodeData, parentNode);
		}
		#endregion
		#region TreeListEndUnboundLoad
		public void TreeListEndUnboundLoad() {
			tlCellReference.EndUnboundLoad();
		}
		#endregion
		#region TreeListBeginUpdate
		public void TreeListBeginUpdate() {
			tlCellReference.BeginUpdate();
		}
		#endregion
		#region TreeListEndUpdate
		public void TreeListEndUpdate() {
			tlCellReference.EndUpdate();
		}
		#endregion
		#region TreeListFocus
		public bool TreeListFocus() {
			return tlCellReference.Focus();
		}
		#endregion
	}
}
