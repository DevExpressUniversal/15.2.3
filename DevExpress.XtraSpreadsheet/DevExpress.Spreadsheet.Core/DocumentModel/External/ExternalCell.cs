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
using System.Collections;
using DevExpress.Office.Utils;
using DevExpress.Utils;
namespace DevExpress.XtraSpreadsheet.Model.External {
	#region IExternalCellContainer
	public interface IExternalCellContainer : ICellPosition {
		ExternalWorksheet Sheet { get; }
	}
	#endregion
	#region ExternalCell
	public class ExternalCell : CellBase {
		#region Fields
		readonly ExternalWorksheet sheet;
		VariantValue value;
		#endregion
		public ExternalCell(CellPosition position, ExternalWorksheet worksheet)
			: base(position, worksheet.SheetId) {
				Guard.ArgumentNotNull(worksheet, "worksheet");
			this.sheet = worksheet;
		}
		#region Properties
		public override IWorksheet Sheet { get { return sheet; } }
		public override VariantValue Value { get { return value; } set { this.value = value; } }
		#endregion
		public override void SetValue(VariantValue value) {
			AssignValue(value);
		}
		public override void AssignValue(VariantValue value) {
			this.value = value;
		}
		public override void AssignValueCore(VariantValue value) {
			this.value = value;
		}
		public override void OffsetRowIndex(int offset, bool needChangeRow) {
		}
		public override void OffsetColumnIndex(int offset) {
		}
		public override bool IsVisible() {
			return true;
		}
	}
	#endregion
	#region ExternalCellCollection
	public class ExternalCellCollection : CellCollectionBase<ExternalCell> {
		public ExternalCellCollection(IExternalCellContainer owner)
			: base(owner) {
		}
		protected internal new IExternalCellContainer Owner { get { return (IExternalCellContainer)base.Owner; } }
		protected override void AfterInsertCell(int innerIndex, ExternalCell cell) {
		}
		protected override void BeforeRemoveCell(ExternalCell cell) {
		}
		public override ExternalCell CreateNewCellCore(CellPosition position) {
			return new ExternalCell(position, Owner.Sheet);
		}
		public override void OffsetRowIndex(int offset) {
		}
		public override void OffsetColumnIndex(int offset) {
		}
	}
	#endregion
}
