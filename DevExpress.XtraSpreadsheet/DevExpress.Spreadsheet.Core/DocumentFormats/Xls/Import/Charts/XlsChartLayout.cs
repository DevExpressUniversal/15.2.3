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
using System.IO;
using System.Reflection;
using System.Text;
using System.Globalization;
using DevExpress.Office;
using DevExpress.Utils;
using DevExpress.XtraSpreadsheet.Model;
using DevExpress.XtraSpreadsheet.Utils;
using DevExpress.Office.Utils;
using DevExpress.XtraSpreadsheet.Internal;
using DevExpress.XtraSpreadsheet.Drawing;
#if !SL
using System.Drawing;
#else
using System.Windows.Media;
#endif
namespace DevExpress.XtraSpreadsheet.Import.Xls {
	#region XlsChartPosMode
	public enum XlsChartPosMode {
		RelativePoints = 0,
		AbsolutePoints = 1,
		Parent = 2,
		Offset = 3,
		RelativeChart = 5
	}
	#endregion
	#region IXlsChartPosition
	public interface IXlsChartPosition {
		bool Apply { get; set; }
		XlsChartPosMode TopLeftMode { get; set; }
		XlsChartPosMode BottomRightMode { get; set; }
		int Left { get; set; }
		int Top { get; set; }
		int Width { get; set; }
		int Height { get; set; }
	}
	#endregion
	#region XlsChartPosition
	public class XlsChartPosition : IXlsChartPosition {
		#region IXlsChartPosition Members
		public bool Apply { get; set; }
		public XlsChartPosMode TopLeftMode { get; set; }
		public XlsChartPosMode BottomRightMode { get; set; }
		public int Left { get; set; }
		public int Top { get; set; }
		public int Width { get; set; }
		public int Height { get; set; }
		#endregion
	}
	#endregion
	#region XlsChartLayout12Base
	public abstract class XlsChartLayout12Base {
		#region Properties
		public bool Apply { get; set; }
		public LayoutMode XMode { get; set; }
		public LayoutMode YMode { get; set; }
		public LayoutMode WidthMode { get; set; }
		public LayoutMode HeightMode { get; set; }
		public double X { get; set; }
		public double Y { get; set; }
		public double Width { get; set; }
		public double Height { get; set; }
		#endregion
		public virtual void SetupLayout(LayoutOptions layout) {
			if (!Apply)
				return;
			layout.Left = GetLayoutPosition(X, XMode);
			layout.Top = GetLayoutPosition(Y, YMode);
			layout.Width = GetLayoutPosition(Width, WidthMode);
			layout.Height = GetLayoutPosition(Height, HeightMode);
		}
		ManualLayoutPosition GetLayoutPosition(double value, LayoutMode mode) {
			if (mode == LayoutMode.Auto)
				return LayoutOptions.AutoPosition;
			return new ManualLayoutPosition(value, mode);
		}
	}
	#endregion
	#region IXlsChartLayout12
	public interface IXlsChartLayout12 {
		bool Apply { get; set; }
		LegendPosition LegendPos { get; set; }
		LayoutMode XMode { get; set; }
		LayoutMode YMode { get; set; }
		LayoutMode WidthMode { get; set; }
		LayoutMode HeightMode { get; set; }
		double X { get; set; }
		double Y { get; set; }
		double Width { get; set; }
		double Height { get; set; }
	}
	#endregion
	#region XlsChartLayout12
	public class XlsChartLayout12 : XlsChartLayout12Base, IXlsChartLayout12 {
		#region Properties
		public LegendPosition LegendPos { get; set; }
		#endregion
	}
	#endregion
	#region XlsChartLayout12A
	public class XlsChartLayout12A : XlsChartLayout12Base {
		#region Properties
		public LayoutTarget Target { get; set; }
		#endregion
		public override void SetupLayout(LayoutOptions layout) {
			if (!Apply)
				return;
			layout.Target = Target;
			base.SetupLayout(layout);
		}
	}
	#endregion
}
