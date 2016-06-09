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
using DevExpress.Utils;
using DevExpress.Office;
using DevExpress.Office.History;
using DevExpress.Office.Utils;
using DevExpress.XtraSpreadsheet.Model.History;
using DevExpress.XtraSpreadsheet.Utils;
namespace DevExpress.XtraSpreadsheet.Model {
	#region MarkerStyle
	public enum MarkerStyle {
		Auto,
		None,
		Circle,
		Dash,
		Diamond,
		Dot,
		Picture,
		Plus,
		Square,
		Star,
		Triangle,
		X
	}
	#endregion
	public class Marker : ISupportsCopyFrom<Marker> {
		#region Fields
		readonly IChart parent;
		MarkerStyle symbol;
		int size;
		ShapeProperties shapeProperties;
		#endregion
		public Marker(IChart parent) {
			this.parent = parent;
			this.symbol = MarkerStyle.Auto;
			this.size = 7;
			this.shapeProperties = new ShapeProperties(DocumentModel) { Parent = parent };
		}
		#region Properties
		protected internal IChart Parent { get { return parent; } }
		protected internal DocumentModel DocumentModel { get { return parent.DocumentModel; } }
		#region Symbol
		public MarkerStyle Symbol {
			get { return symbol; }
			set {
				if(symbol == value)
					return;
				SetSymbol(value);
			}
		}
		void SetSymbol(MarkerStyle value) {
			MarkerSymbolPropertyChangedHistoryItem historyItem = new MarkerSymbolPropertyChangedHistoryItem(DocumentModel, this, symbol, value);
			DocumentModel.History.Add(historyItem);
			historyItem.Execute();
		}
		protected internal void SetSymbolCore(MarkerStyle value) {
			this.symbol = value;
			Parent.Invalidate();
		}
		#endregion
		#region Size
		public int Size {
			get { return size; }
			set {
				ValueChecker.CheckValue(value, 2, 72, "Size");
				SetSize(value);
			}
		}
		void SetSize(int value) {
			MarkerSizePropertyChangedHistoryItem historyItem = new MarkerSizePropertyChangedHistoryItem(DocumentModel, this, size, value);
			DocumentModel.History.Add(historyItem);
			historyItem.Execute();
		}
		protected internal void SetSizeCore(int value) {
			this.size = value;
			Parent.Invalidate();
		}
		#endregion
		public ShapeProperties ShapeProperties { get { return shapeProperties; } }
		#endregion
		#region ISupportsCopyFrom<Marker> Members
		public void CopyFrom(Marker value) {
			Guard.ArgumentNotNull(value, "value");
			Symbol = value.Symbol;
			Size = value.Size;
			this.shapeProperties.CopyFrom(value.shapeProperties);
		}
		#endregion
		public void ResetToStyle(MarkerStyle markerSymbol) {
			ShapeProperties.ResetToStyle();
			Symbol = markerSymbol;
		}
	}
}
