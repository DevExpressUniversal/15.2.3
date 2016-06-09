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

using DevExpress.Office.History;
using DevExpress.Office.Utils;
using DevExpress.Utils;
using System;
namespace DevExpress.Office.DrawingML {
	#region DrawingTextAutoFitType
	public enum DrawingTextAutoFitType {
		None,
		Normal,
		Shape,
		Automatic
	}
	#endregion
	#region IDrawingTextAutoFit
	public interface IDrawingTextAutoFit {
		DrawingTextAutoFitType Type { get; }
		bool Equals(IDrawingTextAutoFit other);
		IDrawingTextAutoFit CloneTo(IDocumentModel documentModel);
		void Visit(IDrawingTextAutoFitVisitor visitor);
	}
	#endregion
	#region IDrawingTextAutoFitVisitor
	public interface IDrawingTextAutoFitVisitor {
		void VisitAutoFitNone();
		void VisitAutoFitShape();
		void Visit(DrawingTextNormalAutoFit autoFit);
	}
	#endregion
	#region DrawingTextAutoFit
	public class DrawingTextAutoFit : IDrawingTextAutoFit {
		public static DrawingTextAutoFit None = new DrawingTextAutoFit(DrawingTextAutoFitType.None);
		public static DrawingTextAutoFit Shape = new DrawingTextAutoFit(DrawingTextAutoFitType.Shape);
		public static DrawingTextAutoFit Automatic = new DrawingTextAutoFit(DrawingTextAutoFitType.Automatic);
		DrawingTextAutoFitType fitType;
		public DrawingTextAutoFit(DrawingTextAutoFitType fitType) {
			this.fitType = fitType;
		}
		#region IAutoFitMembers
		public DrawingTextAutoFitType Type { get { return fitType; } }
		public bool Equals(IDrawingTextAutoFit other) {
			if (other == null)
				return false;
			return Type == other.Type;
		}
		public IDrawingTextAutoFit CloneTo(IDocumentModel documentModel) {
			if (Type == DrawingTextAutoFitType.Shape)
				return DrawingTextAutoFit.Shape;
			if (Type == DrawingTextAutoFitType.None)
				return DrawingTextAutoFit.None;
			return DrawingTextAutoFit.Automatic;
		}
		#endregion
		public void Visit(IDrawingTextAutoFitVisitor visitor) {
			if (Type == DrawingTextAutoFitType.None)
				visitor.VisitAutoFitNone();
			if (Type == DrawingTextAutoFitType.Shape)
				visitor.VisitAutoFitShape();
		}
	}
	#endregion
	#region DrawingTextNormalAutoFit
	public class DrawingTextNormalAutoFit : IDrawingTextAutoFit {
		#region Fields
		public const int DefaultFontScale = 100000;
		readonly IDocumentModel documentModel;
		int fontScale;
		int lineSpaceReduction;
		#endregion
		public DrawingTextNormalAutoFit(IDocumentModel documentModel) {
			Guard.ArgumentNotNull(documentModel, "documentModel");
			this.documentModel = documentModel;
			this.fontScale = 100000;
			this.lineSpaceReduction = 0;
		}
		#region Properties
		public IDocumentModel DocumentModel { get { return documentModel; } }
		#region FontScale
		public int FontScale {
			get { return fontScale; }
			set {
				if (fontScale == value)
					return;
				ValueChecker.CheckValue(value, 1000, 100000, "FontScale");
				SetAutoFitValue(SetFontScale, fontScale, value);
			}
		}
		protected internal void SetFontScale(int value) {
			this.fontScale = value;
		}
		#endregion
		#region LineSpaceReduction
		public int LineSpaceReduction {
			get { return lineSpaceReduction; }
			set {
				if (lineSpaceReduction == value)
					return;
				ValueChecker.CheckValue(value, 0, 13200000, "LineSpaceReduction");
				SetAutoFitValue(SetLineSpaceReduction, lineSpaceReduction, value);
			}
		}
		void SetAutoFitValue(Action<int> action, int oldValue, int newValue) {
			DrawingTextNormalAutoFitChangedHistoryItem<int> historyItem = new DrawingTextNormalAutoFitChangedHistoryItem<int>(this, action, oldValue, newValue);
			this.documentModel.History.Add(historyItem);
			historyItem.Execute();
		}
		protected internal void SetLineSpaceReduction(int value) {
			this.lineSpaceReduction = value;
		}
		#endregion
		#endregion
		#region IDrawingTextAutoFit
		public DrawingTextAutoFitType Type { get { return DrawingTextAutoFitType.Normal; } }
		public bool Equals(IDrawingTextAutoFit other) {
			if (other == null)
				return false;
			if (Type != other.Type)
				return false;
			DrawingTextNormalAutoFit autoFit = other as DrawingTextNormalAutoFit;
			return FontScale == autoFit.FontScale && LineSpaceReduction == autoFit.LineSpaceReduction;
		}
		public IDrawingTextAutoFit CloneTo(IDocumentModel documentModel) {
			DrawingTextNormalAutoFit result = new DrawingTextNormalAutoFit(documentModel);
			result.fontScale = this.fontScale;
			result.lineSpaceReduction = this.lineSpaceReduction;
			return result;
		}
		public void Visit(IDrawingTextAutoFitVisitor visitor) {
			visitor.Visit(this);
		}
		#endregion
	}
	#endregion
}
