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
using DevExpress.Xpf.Editors;
using DevExpress.XtraEditors.DXErrorProvider;
using DevExpress.Xpf.Editors.Validation;
namespace DevExpress.Xpf.Grid {
	public class GridRowValidationError : RowValidationError {
		public GridRowValidationError(object errorContent, Exception exception, ErrorType errorType, int rowHandle)
			: base(errorContent, exception, errorType, rowHandle, false) {
		}
		public int RowHandle { get { return fRowHandle; } }
	}
	public class GridCellValidationError : RowValidationError {
		public GridCellValidationError(object errorContent, Exception exception, ErrorType errorType, int rowHandle, GridColumn column)
			: base(errorContent, exception, errorType, rowHandle, true) {
			Column = column;
		}
		public int RowHandle { get { return fRowHandle; } }
		public GridColumn Column { get; private set; }
		public override bool Equals(object obj) {
			GridCellValidationError error = obj as GridCellValidationError;
			return error != null &&
				object.Equals(ErrorContent, error.ErrorContent) &&
				object.Equals(Exception, error.Exception) &&
				object.Equals(ErrorType, error.ErrorType) &&
				object.Equals(RowHandle, error.RowHandle) &&
				object.Equals(Column, error.Column);
		}
		public override int GetHashCode() {
			return base.GetHashCode();
		}
	}
}
