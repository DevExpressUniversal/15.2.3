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
using DevExpress.Compatibility.System.Drawing;
#if !SL
using System.Drawing;
#else
using System.Windows.Media;
#endif
namespace DevExpress.Spreadsheet {
	public interface Fill {
		Color BackgroundColor { get; set; }
		Color PatternColor { get; set; }
		PatternType PatternType { get; set; }
		FillType FillType { get; set; }
		GradientFill Gradient { get; }
	}
	public enum PatternType {
		None = DevExpress.Export.Xl.XlPatternType.None,
		Solid = DevExpress.Export.Xl.XlPatternType.Solid,
		MediumGray = DevExpress.Export.Xl.XlPatternType.MediumGray,
		DarkGray = DevExpress.Export.Xl.XlPatternType.DarkGray,
		LightGray = DevExpress.Export.Xl.XlPatternType.LightGray,
		DarkHorizontal = DevExpress.Export.Xl.XlPatternType.DarkHorizontal,
		DarkVertical = DevExpress.Export.Xl.XlPatternType.DarkVertical,
		DarkDown = DevExpress.Export.Xl.XlPatternType.DarkDown,
		DarkUp = DevExpress.Export.Xl.XlPatternType.DarkUp,
		DarkGrid = DevExpress.Export.Xl.XlPatternType.DarkGrid,
		DarkTrellis = DevExpress.Export.Xl.XlPatternType.DarkTrellis,
		LightHorizontal = DevExpress.Export.Xl.XlPatternType.LightHorizontal,
		LightVertical = DevExpress.Export.Xl.XlPatternType.LightVertical,
		LightDown = DevExpress.Export.Xl.XlPatternType.LightDown,
		LightUp = DevExpress.Export.Xl.XlPatternType.LightUp,
		LightGrid = DevExpress.Export.Xl.XlPatternType.LightGrid,
		LightTrellis = DevExpress.Export.Xl.XlPatternType.LightTrellis,
		Gray125 = DevExpress.Export.Xl.XlPatternType.Gray125,
		Gray0625 = DevExpress.Export.Xl.XlPatternType.Gray0625
	}
	public enum FillType {
		Pattern = DevExpress.XtraSpreadsheet.Model.ModelFillType.Pattern,
		Gradient = DevExpress.XtraSpreadsheet.Model.ModelFillType.Gradient
	}
	public interface GradientFill {
		GradientFillType Type { get; set; }
		double Degree { get; set; }
		float RectangleLeft { get; set; }
		float RectangleRight { get; set; }
		float RectangleTop { get; set; }
		float RectangleBottom { get; set; }
		GradientStopCollection Stops { get; }
	}
	public enum GradientFillType {
		Linear = DevExpress.XtraSpreadsheet.Model.ModelGradientFillType.Linear,
		Path = DevExpress.XtraSpreadsheet.Model.ModelGradientFillType.Path
	}
	public interface GradientStopCollection {
		int Count { get; }
		GradientStop this[int index] { get; }
		void Add(double position, Color color);
		void RemoveAt(int index);
		void Clear();
	}
	public interface GradientStop {
		double Position { get; }
		Color Color { get; }
	}
}
