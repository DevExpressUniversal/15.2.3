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
using System.Linq;
using System.Text;
using DevExpress.XtraEditors.DXErrorProvider;
using DevExpress.Xpf.Grid;
namespace DevExpress.Xpf.Grid.TreeList {
	public class TreeListNodeValidationError : RowValidationError {
		public TreeListNodeValidationError(object errorContent, Exception exception, ErrorType errorType, int rowHandle, TreeListNode node)
			: base(errorContent, exception, errorType, rowHandle, false) {
				Node = node;
		}
		public int RowHandle { get { return fRowHandle; } }
		public TreeListNode Node { get; private set; }
	}
	public class TreeListCellValidationError : RowValidationError {
		public TreeListCellValidationError(object errorContent, Exception exception, ErrorType errorType, int rowHandle, TreeListNode node, ColumnBase column)
			: base(errorContent, exception, errorType, rowHandle, true) {
			Column = column;
			Node = node;
		}
		public int RowHandle { get { return fRowHandle; } }
		public ColumnBase Column { get; private set; }
		public TreeListNode Node { get; private set; }
		public override bool Equals(object obj) {
			TreeListCellValidationError error = obj as TreeListCellValidationError;
			return error != null &&
				object.Equals(ErrorContent, error.ErrorContent) &&
				object.Equals(Exception, error.Exception) &&
				object.Equals(ErrorType, error.ErrorType) &&
				object.Equals(RowHandle, error.RowHandle) &&
				object.Equals(Node, error.Node) &&
				object.Equals(Column, error.Column);
		}
		public override int GetHashCode() {
			return base.GetHashCode();
		}
	}
}
