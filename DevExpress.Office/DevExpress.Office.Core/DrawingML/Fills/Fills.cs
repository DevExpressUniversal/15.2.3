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
using DevExpress.Compatibility.System.Drawing;
using DevExpress.Office;
using DevExpress.Utils;
#if SL
using System.Windows.Media;
#endif
namespace DevExpress.Office.Drawing {
	#region DrawingFillType
	public enum DrawingFillType {
		Automatic,
		None,
		Solid,
		Gradient,
		Group,
		Pattern,
		Picture
	}
	#endregion
	#region IDrawingFill
	public interface IDrawingFill {
		DrawingFillType FillType { get; }
		ISupportsInvalidate Parent { get; set; }
		IDrawingFill CloneTo(IDocumentModel documentModel);
		void Visit(IDrawingFillVisitor visitor);
	}
	#endregion
	#region IDrawingFillVisitor
	public interface IDrawingFillVisitor {
		void Visit(DrawingFill fill);
		void Visit(DrawingSolidFill fill);
		void Visit(DrawingPatternFill fill);
		void Visit(DrawingGradientFill fill);
		void Visit(DrawingBlipFill fill);
	}
	#endregion
	#region IFillOwner
	public interface IFillOwner {
		void SetDrawingFillCore(IDrawingFill value);
		IDrawingFill Fill { get; set; }
		IDocumentModel DocumentModel { get; }
	}
	#endregion
	#region DrawingFill
	public sealed class DrawingFill : IDrawingFill, IUnderlineFill {
		public static DrawingFill Automatic = new DrawingFill(DrawingFillType.Automatic);
		public static DrawingFill None = new DrawingFill(DrawingFillType.None);
		public static DrawingFill Group = new DrawingFill(DrawingFillType.Group);
		DrawingFillType fillType;
		DrawingFill(DrawingFillType fillType) {
			this.fillType = fillType;
		}
		#region Equals
		public override bool Equals(object obj) {
			DrawingFill other = obj as DrawingFill;
			if (other == null)
				return false;
			return fillType == other.fillType;
		}
		public override int GetHashCode() {
			return (int)fillType;
		}
		#endregion
		#region IDrawingFill Members
		public DrawingFillType FillType { get { return fillType; } }
		public ISupportsInvalidate Parent { get { return null; } set {  } }
		public IDrawingFill CloneTo(IDocumentModel documentModel) {
			if (fillType == DrawingFillType.Automatic)
				return DrawingFill.Automatic;
			if (fillType == DrawingFillType.None)
				return DrawingFill.None;
			return DrawingFill.Group;
		}
		public void Visit(IDrawingFillVisitor visitor) {
			visitor.Visit(this);
		}
		#endregion
		#region IUnderlineFill Members
		DrawingUnderlineFillType IUnderlineFill.Type { get { return DrawingUnderlineFillType.Fill; } }
		IUnderlineFill IUnderlineFill.CloneTo(IDocumentModel documentModel) {
			return CloneTo(documentModel) as IUnderlineFill;
		}
		#endregion
	}
	#endregion
	#region DrawingSolidFill
	public class DrawingSolidFill : IDrawingFill, IUnderlineFill {
		#region Static Members
		public static DrawingSolidFill Create(IDocumentModel documentModel, Color color) {
			DrawingSolidFill result = new DrawingSolidFill(documentModel);
			result.color = DrawingColor.Create(documentModel, color);
			return result;
		} 
		#endregion 
		readonly InvalidateProxy innerParent;
		DrawingColor color;
		public DrawingSolidFill(IDocumentModel documentModel) {
			this.innerParent = new InvalidateProxy();
			this.color = new DrawingColor(documentModel) { Parent = this.innerParent };
		}
		public DrawingColor Color { get { return color; } }
		#region Equals
		public override bool Equals(object obj) {
			DrawingSolidFill other = obj as DrawingSolidFill;
			if (other == null)
				return false;
			if (FillType != other.FillType)
				return false;
			return Color.Equals(((DrawingSolidFill)other).Color);
		}
		public override int GetHashCode() {
			return color.GetHashCode();
		}
		#endregion
		#region IDrawingFill Members
		public DrawingFillType FillType { get { return DrawingFillType.Solid; } }
		public ISupportsInvalidate Parent { get { return innerParent.Target; } set { innerParent.Target = value; } }
		public IDrawingFill CloneTo(IDocumentModel documentModel) {
			DrawingSolidFill result = new DrawingSolidFill(documentModel);
			result.Color.CopyFrom(this.Color);
			return result;
		}
		public void Visit(IDrawingFillVisitor visitor) {
			visitor.Visit(this);
		}
		#endregion
		#region IUnderlineFill Members
		DrawingUnderlineFillType IUnderlineFill.Type { get { return DrawingUnderlineFillType.Fill; }
		}
		IUnderlineFill IUnderlineFill.CloneTo(IDocumentModel documentModel) {
			return CloneTo(documentModel) as IUnderlineFill;
		}
		#endregion
	}
	#endregion
	#region DrawingPatternType
	public enum DrawingPatternType {
		Cross,
		DashedDownwardDiagonal,
		DashedHorizontal,
		DashedUpwardDiagonal,
		DashedVertical,
		DiagonalBrick,
		DiagonalCross,
		Divot,
		DarkDownwardDiagonal,
		DarkHorizontal,
		DarkUpwardDiagonal,
		DarkVertical,
		DownwardDiagonal,
		DottedDiamond,
		DottedGrid,
		Horizontal,
		HorizontalBrick,
		LargeCheckerBoard,
		LargeConfetti,
		LargeGrid,
		LightDownwardDiagonal,
		LightHorizontal,
		LightUpwardDiagonal,
		LightVertical,
		NarrowHorizontal,
		NarrowVertical,
		OpenDiamond,
		Percent10,
		Percent20,
		Percent25,
		Percent30,
		Percent40,
		Percent5,
		Percent50,
		Percent60,
		Percent70,
		Percent75,
		Percent80,
		Percent90,
		Plaid,
		Shingle,
		SmallCheckerBoard,
		SmallConfetti,
		SmallGrid,
		SolidDiamond,
		Sphere,
		Trellis,
		UpwardDiagonal,
		Vertical,
		Wave,
		WideDownwardDiagonal,
		WideUpwardDiagonal,
		Weave,
		ZigZag
	}
	#endregion
	#region DrawingPatternFill
	public class DrawingPatternFill : IDrawingFill, IUnderlineFill {
		#region Static Members
		public static DrawingPatternFill Create(IDocumentModel documentModel, Color foregroundColor, Color backgroundColor, DrawingPatternType patternType) {
			DrawingPatternFill result = new DrawingPatternFill(documentModel);
			result.AssignProperties(foregroundColor, backgroundColor, patternType);
			return result;
		}
		#endregion
		#region Fields
		readonly InvalidateProxy innerParent;
		readonly IDocumentModel documentModel;
		DrawingPatternType patternType;
		DrawingColor foregroundColor;
		DrawingColor backgroundColor;
		#endregion
		public DrawingPatternFill(IDocumentModel documentModel) {
			Guard.ArgumentNotNull(documentModel, "documentModel");
			this.innerParent = new InvalidateProxy();
			this.documentModel = documentModel;
			this.patternType = DrawingPatternType.Percent5;
			this.foregroundColor = new DrawingColor(documentModel) { Parent = this.innerParent };
			this.backgroundColor = new DrawingColor(documentModel) { Parent = this.innerParent };
		}
		#region Properties
		#region PatternType
		public DrawingPatternType PatternType {
			get { return patternType; }
			set {
				if (PatternType == value)
					return;
				SetPatternType(value);
			}
		}
		void SetPatternType(DrawingPatternType value) {
			PatternTypePropertyChangedHistoryItem historyItem = new PatternTypePropertyChangedHistoryItem(documentModel.MainPart, this, patternType, value);
			this.documentModel.History.Add(historyItem);
			historyItem.Execute();
		}
		protected internal void SetPatternTypeCore(DrawingPatternType value) {
			this.patternType = value;
			this.innerParent.Invalidate();
		}
		#endregion
		public DrawingColor ForegroundColor { get { return foregroundColor; } }
		public DrawingColor BackgroundColor { get { return backgroundColor; } }
		#endregion
		void AssignProperties(Color foregroundColor, Color backgroundColor, DrawingPatternType patternType) {
			this.foregroundColor = DrawingColor.Create(documentModel, foregroundColor);
			this.backgroundColor = DrawingColor.Create(documentModel, backgroundColor);
			this.patternType = patternType;
		}
		#region Equals
		public override bool Equals(object obj) {
			DrawingPatternFill other = obj as DrawingPatternFill;
			if (other == null)
				return false;
			if (FillType != other.FillType)
				return false;
			DrawingPatternFill fill = other as DrawingPatternFill;
			return PatternType == fill.PatternType &&
				ForegroundColor.Equals(fill.ForegroundColor) &&
				BackgroundColor.Equals(fill.BackgroundColor);
		}
		public override int GetHashCode() {
			return (int)patternType ^ foregroundColor.GetHashCode() ^ backgroundColor.GetHashCode();
		}
		#endregion
		#region IDrawingFill Members
		public DrawingFillType FillType { get { return DrawingFillType.Pattern; } }
		public ISupportsInvalidate Parent { get { return innerParent.Target; } set { innerParent.Target = value; } }
		public IDrawingFill CloneTo(IDocumentModel documentModel) {
			DrawingPatternFill result = new DrawingPatternFill(documentModel);
			result.PatternType = PatternType;
			result.ForegroundColor.CopyFrom(this.ForegroundColor);
			result.BackgroundColor.CopyFrom(this.BackgroundColor);
			return result;
		}
		public void Visit(IDrawingFillVisitor visitor) {
			visitor.Visit(this);
		}
		#endregion
		#region IUnderlineFill Members
		DrawingUnderlineFillType IUnderlineFill.Type { get { return DrawingUnderlineFillType.Fill; } }
		IUnderlineFill IUnderlineFill.CloneTo(IDocumentModel documentModel) {
			return CloneTo(documentModel) as IUnderlineFill;
		}
		#endregion
	}
	#endregion
}
