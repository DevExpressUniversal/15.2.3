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
using DevExpress.Xpf.PivotGrid.Internal;
using System.Windows;
using DevExpress.XtraPivotGrid;
#if SL
using DevExpress.Xpf.Core.WPFCompatibility;
#else
using DevExpress.Xpf.Utils;
#endif
namespace DevExpress.Xpf.PivotGrid.Printing {
	public class PrintFieldHeader : FieldHeader {
		public PrintFieldHeader() { }
		protected override void SetDefaultStyleKey() {
			this.SetDefaultStyleKey(typeof(PrintFieldHeader));
		}
		protected internal override void OnFieldChanged(PivotGridField oldField) {
			base.OnFieldChanged(oldField);
			EnsureBorderPosition();
		}
		void EnsureBorderPosition() {
			if(Field == null || Data == null) return;
			EnsureMinHeight();
			int right = 0;
			if(Field.Area != FieldArea.RowArea || Data.PivotGrid.RowTotalsLocation == FieldRowTotalsLocation.Tree) {
				List<PivotGridField> fields = Field.Data.GetFieldsByArea(Field.Area, true);
				right = fields.IndexOf(Field) == fields.Count - 1 ? 1 : 0;
				Width = double.NaN;
			} else {
				Width = Field.Width;
			}
			double bottom = BorderThickness.Bottom;
			double top = BorderThickness.Top;
			BorderThickness = new Thickness(1, top, right, bottom);
			int topPadding = PivotPrintHeaderArea.TopPadding;
			int leftPadding = PivotPrintHeaderArea.LeftPadding;
			Padding = new Thickness(topPadding, top == 0 ? leftPadding + 1 : leftPadding, topPadding, Field.Area != FieldArea.RowArea && bottom == 0 ? leftPadding + 1 : leftPadding);
		}
		void EnsureMinHeight() {
			if(Field.PivotGrid == null) {
				MinHeight = 0.0;
				return;
			}
			if(Field.Area == FieldArea.RowArea) {
				MinHeight = PivotGrid.VisualItems.GetHeightDifference(PivotGrid.VisualItems.GetLevelCount(true) - 1, PivotGrid.VisualItems.GetLevelCount(true), true);
			} else {
				if(Field.Area == FieldArea.DataArea &&
					PivotGrid.GetPrintHeaders(PivotArea.DataArea) &&
					PivotGrid.Data.GetFieldsByArea(FieldArea.DataArea, false).Count > 1 &&
					Field.PivotGrid.VisualItems.GetLevelCount(true) == 2 &&
				(PivotGrid.Data.GetFieldsByArea(FieldArea.ColumnArea, true).Count == 0 || !PivotGrid.GetPrintHeaders(PivotArea.ColumnArea)))
					MinHeight = PivotGrid.VisualItems.GetHeightDifference(0, 1, true);
				else
					MinHeight = 0.0;
			}
		}
		protected override void CreateDragDropElementHelper() { }
		protected override void SubscribeEvents(PivotGridField field) { }
		protected override void UnsubscribeEvents(PivotGridField field) { }
		protected override bool? GetIsMustBindToFieldWidth() {
			return null;
		}
	}
	public class PrintHeaderContentPresenter : PivotContentPresenter {
		public PrintHeaderContentPresenter() { }
	}
}
