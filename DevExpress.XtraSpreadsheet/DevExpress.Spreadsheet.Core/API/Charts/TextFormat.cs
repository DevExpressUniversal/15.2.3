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
using System.ComponentModel;
using DevExpress.Office;
using DevExpress.Spreadsheet.Drawings;
namespace DevExpress.Spreadsheet.Charts {
	public enum ShapeTextDirection {
		Horizontal,
		Rotated90,
		Rotated270,
		Stacked
	}
	public interface ShapeTextFormat {
		ShapeTextFont Font { get; }
		ShapeTextDirection TextDirection { get; set; }
		int TextRotation { get; set; }
	}
}
namespace DevExpress.XtraSpreadsheet.API.Native.Implementation {
	using DevExpress.Spreadsheet;
	using DevExpress.Spreadsheet.Charts;
	using DevExpress.Office.API.Internal;
	using DevExpress.XtraSpreadsheet.Drawing;
	using DevExpress.Office.DrawingML;
	using DevExpress.Office.Drawing;
	partial class NativeShapeTextFormat : NativeObjectBase, ShapeTextFormat {
		#region Fields
		readonly Model.TextProperties modelTextProperties;
		readonly Model.IChartTextOwnerEx modelTextOwner;
		NativeShapeTextFont font;
		#endregion
		public NativeShapeTextFormat(Model.TextProperties modelTextProperties) {
			this.modelTextProperties = modelTextProperties;
		}
		public NativeShapeTextFormat(Model.IChartTextOwnerEx modelTextOwner) {
			this.modelTextOwner = modelTextOwner;
		}
		protected override void SetIsValid(bool value) {
			base.SetIsValid(value);
			if (font != null)
				font.IsValid = value;
		}
		#region Properties
		DrawingTextBodyProperties BodyProperties { 
			get {
				if (modelTextOwner != null) {
					if (modelTextOwner.Text.TextType == Model.ChartTextType.Rich)
						return ((Model.ChartRichText)modelTextOwner.Text).BodyProperties;
					return modelTextOwner.TextProperties.BodyProperties;
				}
				return modelTextProperties.BodyProperties; 
			} 
		}
		DrawingTextParagraphCollection Paragraphs {
			get {
				if (modelTextOwner != null) {
					if (modelTextOwner.Text.TextType == Model.ChartTextType.Rich)
						return ((Model.ChartRichText)modelTextOwner.Text).Paragraphs;
					return modelTextOwner.TextProperties.Paragraphs;
				}
				return modelTextProperties.Paragraphs; 
			}
		}
		#endregion
		#region ShapeTextFormat Members
		public ShapeTextFont Font {
			get {
				CheckValid();
				if (font == null) {
					if (modelTextOwner != null)
						font = new NativeShapeTextFont(modelTextOwner);
					else
						font = new NativeShapeTextFont(modelTextProperties);
				}
				return font;
			}
		}
		public int TextRotation {
			get {
				CheckValid();
				return (int)DrawingValueConverter.FromPositiveFixedAngle(BodyProperties.Rotation);
			}
			set {
				CheckValid();
				DrawingTextBodyProperties bodyProperties = BodyProperties;
				IDocumentModel documentModel = bodyProperties.DocumentModel;
				documentModel.BeginUpdate();
				try {
					CheckParagraphs();
					BodyProperties.Rotation = DrawingValueConverter.ToPositiveFixedAngle(value);
				}
				finally {
					documentModel.EndUpdate();
				}
			}
		}
		public ShapeTextDirection TextDirection {
			get {
				CheckValid();
				return GetShapeTextDirection();
			}
			set {
				CheckValid();
				SetShapeTextDirection(value);
			}
		}
		#endregion
		#region Internal
		ShapeTextDirection GetShapeTextDirection() {
			int rotation = (int)DrawingValueConverter.FromPositiveFixedAngle(BodyProperties.Rotation);
			DrawingTextVerticalTextType type = BodyProperties.VerticalText;
			if (type == DrawingTextVerticalTextType.WordArtVertical)
				return ShapeTextDirection.Stacked;
			bool positiveAngle = rotation >= 0;
			int remainder = rotation % 360;
			int absRemainder = Math.Abs(remainder);
			if (absRemainder == 0)
				return ShapeTextDirection.Horizontal;
			else if ((positiveAngle && remainder == 90) || (!positiveAngle && absRemainder == 270))
				return ShapeTextDirection.Rotated90;
			else if ((positiveAngle && remainder == 270) || (!positiveAngle && absRemainder == 90))
				return ShapeTextDirection.Rotated270;
			return ShapeTextDirection.Horizontal;
		}
		void SetShapeTextDirection(ShapeTextDirection textDirection) {
			DrawingTextBodyProperties bodyProperties = BodyProperties;
			IDocumentModel documentModel = bodyProperties.DocumentModel;
			documentModel.BeginUpdate();
			try {
				CheckParagraphs();
				if (textDirection == ShapeTextDirection.Stacked) {
					bodyProperties.VerticalText = DrawingTextVerticalTextType.WordArtVertical;
					bodyProperties.Rotation = DrawingValueConverter.ToPositiveFixedAngle(0);
				}
				else {
					bodyProperties.VerticalText = DrawingTextVerticalTextType.Horizontal;
					if (textDirection == ShapeTextDirection.Rotated90)
						bodyProperties.Rotation = DrawingValueConverter.ToPositiveFixedAngle(90);
					else if (textDirection == ShapeTextDirection.Rotated270)
						bodyProperties.Rotation = DrawingValueConverter.ToPositiveFixedAngle(-90);
					else
						bodyProperties.Rotation = DrawingValueConverter.ToPositiveFixedAngle(0);
				}
			}
			finally {
				documentModel.EndUpdate();
			}
		}
		void CheckParagraphs() {
			DrawingTextParagraphCollection paragraphs = Paragraphs;
			if (paragraphs.Count == 0) {
				DrawingTextParagraph paragraph = new DrawingTextParagraph(paragraphs.DocumentModel);
				paragraph.ApplyEndRunProperties = true;
				paragraph.ApplyParagraphProperties = true;
				paragraph.ParagraphProperties.ApplyDefaultCharacterProperties = true;
				paragraphs.Add(paragraph);
			}
		}
		#endregion
	}
}
