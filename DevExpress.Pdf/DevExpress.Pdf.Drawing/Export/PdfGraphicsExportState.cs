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
using System.Drawing.Drawing2D;
namespace DevExpress.Pdf.Drawing {
	public class PdfGraphicsExportState {
		double lineWidth = -1;
		double[] strokingColorComponents;
		double strokingAlpha = 1;
		double[] nonStrokingColorComponents;
		double nonStrokingAlpha = 1;
		DashStyle? dashStyle;
		DashCap? dashCap;
		LineCap? lineCap;
		LineJoin? lineJoin;
		double miterLimit;
		public double LineWidth {
			get { return lineWidth; }
			set { lineWidth = value; }
		}
		public double[] StrokingColorComponents {
			get { return strokingColorComponents; }
			set { strokingColorComponents = value;; }
		}
		public double StrokingAlpha {
			get { return strokingAlpha; }
			set { strokingAlpha = value; }
		}
		public double[] NonStrokingColorComponents {
			get { return nonStrokingColorComponents; }
			set { nonStrokingColorComponents = value; }
		}
		public double NonStrokingAlpha {
			get { return nonStrokingAlpha; }
			set { nonStrokingAlpha = value; }
		}
		public DashStyle? DashStyle {
			get { return dashStyle; }
			set { dashStyle = value; }
		}
		public DashCap? DashCap {
			get { return dashCap; }
			set { dashCap = value; }
		}
		public LineCap? LineCap {
			get { return lineCap; }
			set { lineCap = value; }
		}
		public LineJoin? LineJoin {
			get { return lineJoin; }
			set { lineJoin = value; }
		}
		public double MiterLimit {
			get { return miterLimit; }
			set { miterLimit = value; }
		}
		public PdfGraphicsExportState() {
		}
		public PdfGraphicsExportState(PdfGraphicsExportState s) {
			double lineWidth = s.lineWidth;
			strokingColorComponents = s.strokingColorComponents;
			strokingAlpha = s.strokingAlpha;
			nonStrokingColorComponents = s.nonStrokingColorComponents;
			nonStrokingAlpha = s.nonStrokingAlpha;
			dashStyle = s.dashStyle;
			lineCap = s.lineCap;
			lineJoin = s.lineJoin;
		}
	}
}
