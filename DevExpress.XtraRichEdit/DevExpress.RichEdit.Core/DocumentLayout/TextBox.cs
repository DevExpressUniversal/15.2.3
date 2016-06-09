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
using DevExpress.XtraRichEdit;
using DevExpress.XtraRichEdit.Layout.Export;
namespace DevExpress.XtraRichEdit.Layout {
	#region TextBox
	public class TextBox : MultiPositionBox {
		public override Box CreateBox() {
			return new TextBox();
		}
		public override bool IsVisible { get { return true; } }
		public override bool IsNotWhiteSpaceBox { get { return true; } }
		public override bool IsLineBreak { get { return false; } }
		public override void ExportTo(IDocumentLayoutExporter exporter) {
			exporter.ExportTextBox(this);
		}
		public override BoxHitTestManager CreateHitTestManager(IBoxHitTestCalculator calculator) {
			return calculator.CreateTextBoxHitTestManager(this);
		}
		public override void Accept(IRowBoxesVisitor visitor) {
			visitor.Visit(this);
		}
	}
	#endregion
	#region SpecialTextBox
	public class SpecialTextBox : TextBox {
		public override Box CreateBox() {
			return new SpecialTextBox();
		}
		public override void ExportTo(IDocumentLayoutExporter exporter) {
			exporter.ExportSpecialTextBox(this);
		}
	}
	#endregion
	#region SpecialTextBoxCollection
	public class SpecialTextBoxCollection : List<SpecialTextBox> {
	}
	#endregion
	#region LayoutDependentTextBox
	public class LayoutDependentTextBox : TextBox {
		string calculatedText;
		public override Box CreateBox() {
			return new LayoutDependentTextBox();
		}
		public string CalculatedText { get { return calculatedText; } set { calculatedText = value; } }
		public override void ExportTo(IDocumentLayoutExporter exporter) {
			exporter.ExportLayoutDependentTextBox(this);
		}
		public override BoxHitTestManager CreateHitTestManager(IBoxHitTestCalculator calculator) {
			return calculator.CreateTextBoxHitTestManager(this);
		}
		public override string GetText(DevExpress.XtraRichEdit.Model.PieceTable table) {
			return CalculatedText;
		}
		public override void Accept(IRowBoxesVisitor visitor) {
			visitor.Visit(this);
		}
	}
	#endregion
	#region DashBox
	public class DashBox : TextBox {
		public override Box CreateBox() {
			return new DashBox();
		}
		public override void Accept(IRowBoxesVisitor visitor) {
			visitor.Visit(this);
		}
	}
	#endregion
}
