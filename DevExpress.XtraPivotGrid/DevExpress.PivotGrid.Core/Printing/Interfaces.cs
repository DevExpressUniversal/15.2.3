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
using System.Drawing;
using DevExpress.Utils;
using DevExpress.XtraPivotGrid;
using DevExpress.XtraPivotGrid.Data;
using DevExpress.XtraPrinting;
namespace DevExpress.PivotGrid.Printing {
	public interface IPivotGridPrinterOwner : IPrintable {
		bool CustomExportCell(ref IVisualBrick brick, PivotGridCellItem cellItem, IPivotPrintAppearance appearance, GraphicsUnit graphicsUnit, ref Rectangle rect);
		bool CustomExportFieldValue(ref IVisualBrick brick, PivotFieldValueItem fieldValue, IPivotPrintAppearance appearance, ref Rectangle rect);
		bool CustomExportHeader(ref IVisualBrick brick, PivotFieldItemBase field, IPivotPrintAppearance appearance, ref Rectangle rect);
	}
	public interface IPivotPrintAppearance : ICloneable {
		Color BorderColor { get; }
		Color ForeColor { get; }
		Color BackColor { get; }
		Font Font { get; }
		float BorderWidth { get; }
		BrickBorderStyle BorderStyle { get; }
		HorzAlignment TextHorizontalAlignment { get; set; }
		VertAlignment TextVerticalAlignment { get; set; }
		StringFormat StringFormat { get; }
		IPivotPrintAppearanceOptions Options { get; }
	}
	public interface IPivotPrintAppearanceOptions {
		bool UseTextOptions { get; }
		bool UseBorderColor { get; }
		bool UseForeColor { get; }
		bool UseBackColor { get; }
		bool UseFont { get; }
		bool UseBorderWidth { get; }
		bool UseBorderStyle { get; }
	}
	public interface IPivotGridOptionsPrintOwner { 
		PivotGridOptionsPrint OptionsPrint { get; }
	}
}
