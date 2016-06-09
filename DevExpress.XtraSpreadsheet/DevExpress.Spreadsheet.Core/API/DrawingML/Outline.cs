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

namespace DevExpress.Spreadsheet.Drawings {
	public enum ShapeLineCompoundType {
		Single = DevExpress.Office.Utils.OutlineCompoundType.Single,
		Double = DevExpress.Office.Utils.OutlineCompoundType.Double,
		ThickThin = DevExpress.Office.Utils.OutlineCompoundType.ThickThin,
		ThinThick = DevExpress.Office.Utils.OutlineCompoundType.ThinThick,
		Triple = DevExpress.Office.Utils.OutlineCompoundType.Triple
	}
	public enum ShapeLineDashing {
		Solid = DevExpress.Office.Utils.OutlineDashing.Solid,
		SystemDash = DevExpress.Office.Utils.OutlineDashing.SystemDash,
		SystemDot = DevExpress.Office.Utils.OutlineDashing.SystemDot,
		SystemDashDot = DevExpress.Office.Utils.OutlineDashing.SystemDashDot,
		SystemDashDotDot = DevExpress.Office.Utils.OutlineDashing.SystemDashDotDot,
		Dot = DevExpress.Office.Utils.OutlineDashing.Dot,
		Dash = DevExpress.Office.Utils.OutlineDashing.Dash,
		DashDot = DevExpress.Office.Utils.OutlineDashing.DashDot,
		LongDash = DevExpress.Office.Utils.OutlineDashing.LongDash,
		LongDashDot = DevExpress.Office.Utils.OutlineDashing.LongDashDot,
		LongDashDotDot = DevExpress.Office.Utils.OutlineDashing.LongDashDotDot
	}
	public enum ShapeLineEndCapStyle {
		Round = DevExpress.Office.Utils.OutlineEndCapStyle.Round,
		Square = DevExpress.Office.Utils.OutlineEndCapStyle.Square,
		Flat = DevExpress.Office.Utils.OutlineEndCapStyle.Flat
	}
	public enum ShapeLineJoinStyle {
		Bevel = DevExpress.Office.Utils.LineJoinStyle.Bevel,
		Miter = DevExpress.Office.Utils.LineJoinStyle.Miter,
		Round = DevExpress.Office.Utils.LineJoinStyle.Round
	}
	public interface ShapeOutline : ShapeOutlineFill {
		double Width { get; set; }
		ShapeLineCompoundType CompoundType { get; set; }
		ShapeLineDashing Dashing { get; set; }
		ShapeLineEndCapStyle CapType { get; set; }
		ShapeLineJoinStyle JoinType { get; set; }
		double MiterLimit { get; set; }
	}
}
namespace DevExpress.XtraSpreadsheet.API.Native.Implementation {
	using System;
	using DevExpress.Office.API.Internal;
	using DevExpress.Office.Utils;
	using DevExpress.Spreadsheet;
	using DevExpress.Spreadsheet.Drawings;
	using DevExpress.XtraSpreadsheet.Utils;
	partial class NativeShapeOutline : NativeShapeOutlineFill<DevExpress.Office.Drawing.Outline>, ShapeOutline {
		public NativeShapeOutline(DevExpress.Office.Drawing.Outline modelOutline, NativeWorkbook nativeWorkbook)
			: base(modelOutline, nativeWorkbook) {
		}
		#region ShapeOutline Members
		#region Width
		public double Width {
			get {
				CheckValid();
				return ConvertModelUnitsToPoints(ModelFillOwner.Width);
			}
			set {
				CheckValid();
				ValueChecker.CheckValue(value, 0, DevExpress.Office.Drawing.DrawingValueConstants.MaxWidthInPoints, "OutlineWidth");
				this.ModelFillOwner.Width = ConvertPointsToModelUnits(value);
			}
		}
		#endregion
		#region CompoundType
		public ShapeLineCompoundType CompoundType {
			get {
				CheckValid();
				return (ShapeLineCompoundType)ModelFillOwner.CompoundType;
			}
			set {
				CheckValid();
				ModelFillOwner.CompoundType = (OutlineCompoundType)value;
			}
		}
		#endregion
		#region Dashing
		public ShapeLineDashing Dashing {
			get {
				CheckValid();
				return (ShapeLineDashing)ModelFillOwner.Dashing;
			}
			set {
				CheckValid();
				ModelFillOwner.Dashing = (OutlineDashing)value;
			}
		}
		#endregion
		#region CapType
		public ShapeLineEndCapStyle CapType {
			get {
				CheckValid();
				return (ShapeLineEndCapStyle)ModelFillOwner.EndCapStyle;
			}
			set {
				CheckValid();
				ModelFillOwner.EndCapStyle = (OutlineEndCapStyle)value;
			}
		}
		#endregion
		#region JoinType
		public ShapeLineJoinStyle JoinType {
			get {
				CheckValid();
				return (ShapeLineJoinStyle)ModelFillOwner.JoinStyle;
			}
			set {
				CheckValid();
				ModelFillOwner.JoinStyle = (LineJoinStyle)value;
			}
		}
		#endregion
		#region MiterLimit
		public double MiterLimit {
			get {
				CheckValid();
				return DevExpress.Office.Drawing.DrawingValueConverter.FromPercentage(ModelFillOwner.MiterLimit);
			}
			set {
				CheckValid();
				if (ModelFillOwner.JoinStyle == LineJoinStyle.Miter)
					ModelFillOwner.MiterLimit = DevExpress.Office.Drawing.DrawingValueConverter.ToPercentage(value);
			}
		}
		#endregion
		#endregion
		#region SubscribeEvents
		protected override void SubscribeEvents() {
			ModelFillOwner.SetFill += OnSetFill;
		}
		protected override void UnsubscribeEvents() {
			ModelFillOwner.SetFill -= OnSetFill;
		}
		#endregion
		#region Internal
		double ConvertModelUnitsToPoints(int value) {
			return UnitConverter.Converter.ModelUnitsToEmu(value) / (double)DevExpress.Office.Drawing.DrawingValueConstants.EmusInPoint;
		}
		int ConvertPointsToModelUnits(double value) {
			return UnitConverter.Converter.EmuToModelUnits((int)Math.Round(value * DevExpress.Office.Drawing.DrawingValueConstants.EmusInPoint));
		}
		#endregion
	}
}
