#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       UWP Controls                                                }
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
using DevExpress.Pdf.Native;
using DevExpress.Pdf.Localization;
namespace DevExpress.Pdf {
	public class PdfLineStyle {
		public static PdfLineStyle CreateSolid() {
			return new PdfLineStyle(null, 0);
		}
		public static PdfLineStyle CreateDashed(double dashLength, double gapLength, double dashPhase) {
			if (dashLength < 0.0)
				throw new ArgumentOutOfRangeException("dashLength", PdfCoreLocalizer.GetString(PdfCoreStringId.MsgIncorrectDashLength));
			if (gapLength < 0.0)
				throw new ArgumentOutOfRangeException("gapLength", PdfCoreLocalizer.GetString(PdfCoreStringId.MsgIncorrectGapLength));
			if (dashLength + gapLength == 0.0)
				throw new ArgumentOutOfRangeException(PdfCoreLocalizer.GetString(PdfCoreStringId.MsgIncorrectDashPattern));
			return new PdfLineStyle(new double[] { dashLength, gapLength }, dashPhase);
		}
		public static PdfLineStyle CreateDashed(double[] dashPattern, double dashPhase) {
			if (dashPattern == null)
				throw new ArgumentNullException("dashPattern");
			if (dashPattern.Length == 0)
				throw new ArgumentOutOfRangeException("dashPattern", PdfCoreLocalizer.GetString(PdfCoreStringId.MsgIncorrectDashPatternArraySize));
			double length = 0.0;
			foreach (double size in dashPattern)
				length += size;
			if (length == 0.0)
				throw new ArgumentOutOfRangeException("dashPattern", PdfCoreLocalizer.GetString(PdfCoreStringId.MsgIncorrectDashPattern));
			return new PdfLineStyle(dashPattern, dashPhase);
		}
		internal static double[] ParseDashPattern(IList<object> dashArray) {
			double fullLength = 0.0;
			int dashArrayLength = dashArray.Count;
			double[] dashPattern = new double[dashArrayLength];
			for (int i = 0; i < dashArrayLength; i++) {
				double dashLength = PdfDocumentReader.ConvertToDouble(dashArray[i]);
				fullLength += dashLength;
				dashPattern[i] = dashLength;
			}
			if (fullLength == 0.0)
				PdfDocumentReader.ThrowIncorrectDataException();
			return dashPattern;
		}
		internal static PdfLineStyle Parse(IList<object> parameters) {
			if (parameters.Count != 2)
				PdfDocumentReader.ThrowIncorrectDataException();
			IList<object> dashArray = parameters[0] as IList<object>;
			if (dashArray == null)
				PdfDocumentReader.ThrowIncorrectDataException();
			return dashArray.Count == 0 ? CreateSolid() : CreateDashed(ParseDashPattern(dashArray), PdfDocumentReader.ConvertToDouble(parameters[1]));
		}
		readonly double[] dashPattern;
		readonly double dashPhase;
		public bool IsDashed { get { return dashPattern != null; } }
		public double[] DashPattern { get { return dashPattern; } }
		public double DashPhase { get { return dashPhase; } }
		internal IList<object> Data { 
			get { 
				int length = dashPattern == null ? 0 : dashPattern.Length;
				List<object> pattern = new List<object>(length);
				for (int i = 0; i < length; i++)
					pattern.Add(dashPattern[i]);
				return new List<object> { pattern, dashPhase };
			}
		}
		PdfLineStyle(double[] dashPattern, double dashPhase) {
			this.dashPattern = dashPattern;
			this.dashPhase = dashPhase;
		}
		internal bool IsSame(PdfLineStyle lineStyle) {
			double[] lineStyleDashPattern = lineStyle.dashPattern;
			if (dashPattern == null)
				return lineStyleDashPattern == null;
			int length = dashPattern.Length;
			if (lineStyleDashPattern == null || lineStyleDashPattern.Length != length || dashPhase != lineStyle.dashPhase)
				return false;
			for (int i = 0; i < length; i++)
				if (dashPattern[i] != lineStyleDashPattern[i])
					return false;
			return true;
		}
	}
}
