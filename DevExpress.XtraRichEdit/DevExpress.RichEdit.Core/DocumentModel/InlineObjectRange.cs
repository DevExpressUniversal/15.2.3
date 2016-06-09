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
using System.Text;
using DevExpress.Utils;
using DevExpress.XtraRichEdit.Utils;
using DevExpress.XtraRichEdit.Layout.Engine;
using DevExpress.XtraRichEdit.Export.Rtf;
using DevExpress.Office;
using DevExpress.XtraRichEdit.Layout;
using DevExpress.Office.Utils;
using DevExpress.Compatibility.System.Drawing;
namespace DevExpress.XtraRichEdit.Model {
	public interface IRectangularObject {
		Size ActualSize { get; set; }
	}
	public interface IResizeableObject {
		bool Resizeable { get; }
	}
	public interface IRectangularScalableObject : IRectangularObject {
		Size OriginalSize { get; }
		float ScaleX { get; set; }
		float ScaleY { get; set; }
	}
	public interface IInlineObjectRun {
		Size MeasureRun(IObjectMeasurer measurer);
		Box CreateBox();
	}
	#region InlineObjectRun (abstract class)
	public abstract class InlineObjectRun : TextRunBase, IRectangularScalableObject {
		protected InlineObjectRun(Paragraph paragraph, Size originalSize)
			: base(paragraph) {
			SetOriginalSize(originalSize);
		}
		#region Properties
		public abstract Size OriginalSize { get; }
		public abstract SizeF ActualSizeF { get; }
		public abstract Size ActualSize { get; set; }
		public abstract float ScaleX { get; set; }
		public abstract float ScaleY { get; set; }
		#endregion
		protected internal abstract void SetOriginalSize(Size size);
		public override bool CanJoinWith(TextRunBase nextRun) {
			Guard.ArgumentNotNull(nextRun, "nextRun");
			return false;
		}
		protected internal override void Measure(BoxInfo boxInfo, IObjectMeasurer measurer) {
			boxInfo.Size = OriginalSize;
		}
		protected internal override bool TryAdjustEndPositionToFit(BoxInfo boxInfo, int maxWidth, IObjectMeasurer measurer) {
			return false;
		}
		public override string GetPlainText(ChunkedStringBuilder growBuffer) {
			return String.Empty;
		}
		protected internal override string GetPlainText(ChunkedStringBuilder growBuffer, int from, int to) {
			return GetPlainText(growBuffer);
		}
		public override void Export(IDocumentModelExporter exporter) {
			exporter.Export(this);
		}
		protected internal abstract void CopyOriginalSize(Size originalSize);
	}
	#endregion
	#region InlineObjectRunBase<T> (abstract class)
	public abstract class InlineObjectRunBase<T> : InlineObjectRun where T : InlineObjectInfo, ICloneable<T>, ISupportsCopyFrom<T> {
		readonly InlineObjectProperties<T> properties;
		protected InlineObjectRunBase(Paragraph paragraph, Size originalSize)
			: base(paragraph, originalSize) {
			this.properties = CreateProperties(paragraph.PieceTable);
		}
		#region Properties
		public override float ScaleX {
			get { return Properties.ScaleX; }
			set {
				if (ScaleX == value)
					return;
				Properties.ScaleX = value;
			}
		}
		public override float ScaleY {
			get { return Properties.ScaleY; }
			set {
				if (ScaleY == value)
					return;
				Properties.ScaleY = value;
			}
		}
		public override SizeF ActualSizeF {
			get {
				return new SizeF(OriginalSize.Width * ScaleX / 100.0f, OriginalSize.Height * ScaleY / 100.0f);
			}
		}
		public override Size ActualSize {
			get {
				return Size.Round(new SizeF(OriginalSize.Width * ScaleX / 100, OriginalSize.Height * ScaleY / 100));
			}
			set {
				SetActualSizeInternal(value);
			}
		}
		public void SetActualSizeInternal(Size actualSize) {
			DocumentModel documentModel = Paragraph.DocumentModel;
			documentModel.BeginUpdate();
			try {
				this.ScaleX = ImageScaleCalculator.GetScale(actualSize.Width, OriginalSize.Width, 100f);
				this.ScaleY = ImageScaleCalculator.GetScale(actualSize.Height, OriginalSize.Height, 100f);
			}
			finally {
				documentModel.EndUpdate();
			}
		}
		protected internal InlineObjectProperties<T> Properties { get { return properties; } }
		#endregion
		protected internal abstract InlineObjectProperties<T> CreateProperties(PieceTable pieceTable);
	}
	#endregion
}
