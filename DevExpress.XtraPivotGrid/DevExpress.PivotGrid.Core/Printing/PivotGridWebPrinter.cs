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

using System.Drawing;
using DevExpress.XtraPivotGrid;
using DevExpress.XtraPivotGrid.Data;
namespace DevExpress.PivotGrid.Printing {
	public class PivotGridWebPrinter : PivotGridPrinterBase {
		PrintAppearance appearances;
		public PivotGridWebPrinter(IPivotGridPrinterOwner owner, PivotGridData data, PrintAppearance appearance) : this(owner, data, appearance, null) {
		}
		public PivotGridWebPrinter(IPivotGridPrinterOwner owner, PivotGridData data, PrintAppearance appearance, IPivotGridOptionsPrintOwner optionsPrintOwner) : base(owner, data, optionsPrintOwner) {
			this.appearances = appearance;
		}
		protected PrintAppearance Appearances { get { return appearances; } }
		protected internal override IPivotPrintAppearance GetCellAppearance() {
			return appearances.GetActualCellAppearance((IPrintAppearanceOwner)null);
		}
		protected internal override IPivotPrintAppearance GetCellAppearance(PivotGridCellItem cell, Rectangle? bounds) {
			return appearances.GetActualCellAppearance(cell);
		}
		protected internal override IPivotPrintAppearance GetFieldAppearance(PivotFieldItemBase field) {
			return appearances.GetActualFieldAppearance(field as IPrintAppearanceOwner);
		}
		protected internal override IPivotPrintAppearance GetGrandTotalCellAppearance() {
			return appearances.GetGrandTotalCellAppearance(null);
		}
		protected internal override IPivotPrintAppearance GetTotalCellAppearance() {
			return appearances.GetTotalCellAppearance(null);
		}
		protected internal override IPivotPrintAppearance GetValueAppearance(PivotGridValueType valueType, PivotFieldItemBase field) {
			PrintAppearanceObject appearance = appearances.GetActualFieldValueAppearance(valueType, field as IPrintAppearanceOwner);
			if(field != null) {
				switch(field.Area) {
					case PivotArea.ColumnArea:
						if(field.ColumnValueLineCount > 1)
							appearance.WordWrap = true;
						break;
					case PivotArea.RowArea:
						if(field.RowValueLineCount > 1)
							appearance.WordWrap = true;
						break;
					case PivotArea.DataArea:
						if(Data.OptionsDataField.Area == PivotDataArea.RowArea && Data.OptionsDataField.RowValueLineCount > 1)
							appearance.WordWrap = true;
						else
							if(Data.OptionsDataField.ColumnValueLineCount > 1)
								appearance.WordWrap = true;
						break;
				}
			}
			return appearance;
		}
	}
}
