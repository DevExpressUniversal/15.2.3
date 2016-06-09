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
using System.Globalization;
using DevExpress.XtraPrinting.Native;
namespace DevExpress.XtraPrinting.XamlExport {
	public abstract class VisualBrickXamlExporter : BrickXamlExporterBase {
		int borderStartsCount;
		protected const string staticResourceFormat = "{{StaticResource {0}}}";
		public override void WriteBrickToXaml(XamlWriter writer, BrickBase brick, XamlExportContext exportContext, RectangleF clipRect, Action<XamlWriter> declareNamespaces, Action<XamlWriter, object> writeCustomProperties) {
			VisualBrick visualBrick = brick as VisualBrick;
			if(visualBrick == null) {
				throw new ArgumentException("brick");
			}
			borderStartsCount = 0;
			if(visualBrick.Style.BorderDashStyle == BorderDashStyle.Solid)
				WriteSolidBorder(writer, exportContext, visualBrick, writeCustomProperties);
			else if(visualBrick.Style.BorderDashStyle == BorderDashStyle.Double)
				WriteDoubleBorder(writer, exportContext, visualBrick, writeCustomProperties);
			else
				WriteDashedBorder(writer, exportContext, visualBrick, writeCustomProperties);
			WriteBrickToXamlCore(writer, visualBrick, exportContext);
			if(BrickExportMode == XamlBrickExportMode.ChildElements) {
				writer.WriteStartElement(XamlTag.Canvas);
				float insideBorderPart = (visualBrick.BorderStyle == BrickBorderStyle.Outset) ? 0 : (visualBrick.BorderStyle == BrickBorderStyle.Center) ? 0.5f : 1;
				float leftOffset = (visualBrick.Sides & BorderSide.Left) > 0 ? -visualBrick.Style.BorderWidth * insideBorderPart : 0;
				float topOffset = (visualBrick.Sides & BorderSide.Top) > 0 ? -visualBrick.Style.BorderWidth * insideBorderPart : 0;
				writer.WriteAttribute(XamlNsPrefix.dxpn, XamlAttribute.VisualHelperOffset, leftOffset, topOffset);
			}
		}
		public override void WriteEndTags(XamlWriter writer) {
			for(int i = 0; i < borderStartsCount; i++) {
				writer.WriteEndElement();
			}
		}
		public override bool RequiresBorderStyle() {
			return true;
		}
		protected virtual void WriteBrickToXamlCore(XamlWriter writer, VisualBrick brick, XamlExportContext exportContext) { }
		protected static float GetBorderWidth(BorderSide borderSide, VisualBrick brick) {
			return (brick.Style.Sides & borderSide) > 0 ? brick.Style.BorderWidth : 0;
		}
		protected virtual RectangleF GetBorderBoundsInPixels(VisualBrick brick) {
			RectangleF bounds = new RectangleF(brick.Location, brick.Size).DocToDip();
			float outsideBorderPart = (brick.BorderStyle == BrickBorderStyle.Outset) ? 1 : (brick.BorderStyle == BrickBorderStyle.Center) ? 0.5f : 0;
			bounds.X -= brick.Style.BorderWidth * outsideBorderPart;
			bounds.Y -= brick.Style.BorderWidth * outsideBorderPart;
			bounds.Width += brick.Style.BorderWidth * 2 * outsideBorderPart;
			bounds.Height += brick.Style.BorderWidth * 2 * outsideBorderPart;
			return bounds.Width > 0 ? bounds.Height > 0 ? bounds : RectangleF.Empty : RectangleF.Empty;
		}
		static void WriteTagAttribute(XamlWriter writer, XamlExportContext exportContext, VisualBrick visualBrick) {
			writer.WriteAttribute(XamlAttribute.Tag, DocumentMapTreeViewNodeHelper.GetTagByIndices(exportContext.Page.GetIndicesByBrick(visualBrick), exportContext.PageNumber - 1));
		}
		static System.Windows.TextAlignment MapPrintingTextAlignmentToSystemTextAlignment(TextAlignment printingTextAlignment) {
			switch(printingTextAlignment) {
				case TextAlignment.BottomCenter:
				case TextAlignment.MiddleCenter:
				case TextAlignment.TopCenter:
					return System.Windows.TextAlignment.Center;
				case TextAlignment.BottomLeft:
				case TextAlignment.MiddleLeft:
				case TextAlignment.TopLeft:
					return System.Windows.TextAlignment.Left;
				case TextAlignment.BottomRight:
				case TextAlignment.MiddleRight:
				case TextAlignment.TopRight:
					return System.Windows.TextAlignment.Right;
				case TextAlignment.BottomJustify:
				case TextAlignment.MiddleJustify:
				case TextAlignment.TopJustify:
					return System.Windows.TextAlignment.Justify;
				default:
					throw new ArgumentException("printingTextAlignment");
			}
		}
		void WriteBorderStart(XamlWriter writer, XamlExportContext exportContext, VisualBrick visualBrick, RectangleF borderBounds) {
			writer.WriteStartElement(XamlTag.Border);
			borderStartsCount++;
			if(exportContext.ResourceMap.BorderStylesDictionary.ContainsKey(visualBrick)) {
				string borderStyleName = exportContext.ResourceMap.BorderStylesDictionary[visualBrick];
				writer.WriteAttribute(XamlAttribute.Style, string.Format(staticResourceFormat, borderStyleName));
			}
			writer.WriteAttribute(XamlAttribute.Width, borderBounds.Width);
			writer.WriteAttribute(XamlAttribute.Height, borderBounds.Height);
			writer.WriteAttribute(XamlAttribute.CanvasLeft, borderBounds.X);
			writer.WriteAttribute(XamlAttribute.CanvasTop, borderBounds.Y);
			if(visualBrick.BrickType == BrickTypes.Image) {
				writer.WriteAttribute(XamlAttribute.UseLayoutRounding, true.ToString());
			}
		}
		void WriteSolidBorder(XamlWriter writer, XamlExportContext exportContext, VisualBrick visualBrick, Action<XamlWriter, object> writeCustomProperties) {
			WriteBorderStart(writer, exportContext, visualBrick, GetBorderBoundsInPixels(visualBrick));
			WriteAttachedValues(writer, exportContext, visualBrick, writeCustomProperties);
		}
		void WriteDoubleBorder(XamlWriter writer, XamlExportContext exportContext, VisualBrick visualBrick, Action<XamlWriter, object> writeCustomProperties) {
			float thinBorderWidth = visualBrick.Style.BorderWidth / 3;
			RectangleF borderBoundsInPixels = GetBorderBoundsInPixels(visualBrick);
			WriteBorderStart(writer, exportContext, visualBrick, borderBoundsInPixels);
			WriteAttachedValues(writer, exportContext, visualBrick, writeCustomProperties);
			writer.WriteAttribute(XamlAttribute.BorderThickness, GetThicknessValues(visualBrick.Style.Sides, thinBorderWidth));
			writer.WriteAttribute(XamlAttribute.Padding, GetThicknessValues(visualBrick.Style.Sides, thinBorderWidth));
			borderBoundsInPixels = RectHelper.InflateRect(borderBoundsInPixels, -thinBorderWidth * 2, visualBrick.Style.Sides);
			WriteBorderStart(writer, exportContext, visualBrick, borderBoundsInPixels);
			writer.WriteAttribute(XamlAttribute.BorderThickness, GetThicknessValues(visualBrick.Style.Sides, thinBorderWidth));
		}
		void WriteDashedBorder(XamlWriter writer, XamlExportContext exportContext, VisualBrick visualBrick, Action<XamlWriter, object> writeCustomProperties) {
			RectangleF borderBoundsInPixels = GetBorderBoundsInPixels(visualBrick);
			float strokeOffset = -visualBrick.Style.BorderWidth / 2;
			RectangleF bounds = RectangleF.Inflate(borderBoundsInPixels, strokeOffset, strokeOffset);
			if((visualBrick.Style.Sides & BorderSide.Left) > 0)
				WriteLine(writer, exportContext, visualBrick, RectHelper.TopLeft(bounds), RectHelper.BottomLeft(bounds));
			if((visualBrick.Style.Sides & BorderSide.Right) > 0)
				WriteLine(writer, exportContext, visualBrick, RectHelper.BottomRight(bounds), RectHelper.TopRight(bounds));
			if((visualBrick.Style.Sides & BorderSide.Top) > 0)
				WriteLine(writer, exportContext, visualBrick, RectHelper.TopRight(bounds), RectHelper.TopLeft(bounds));
			if((visualBrick.Style.Sides & BorderSide.Bottom) > 0)
				WriteLine(writer, exportContext, visualBrick, RectHelper.BottomLeft(bounds), RectHelper.BottomRight(bounds));
			WriteBorderStart(writer, exportContext, visualBrick, borderBoundsInPixels);
			WriteAttachedValues(writer, exportContext, visualBrick, writeCustomProperties);
		}
		protected static float[] GetThicknessValues(BorderSide brickSides, float value) {
			return new float[] {
				GetThicknessValue(BorderSide.Left, brickSides, value), GetThicknessValue(BorderSide.Top, brickSides, value),
				GetThicknessValue(BorderSide.Right, brickSides, value), GetThicknessValue(BorderSide.Bottom, brickSides, value)
			};
		}
		static float GetThicknessValue(BorderSide borderSide, BorderSide brickSides, float value) {
			return (brickSides & borderSide) > 0 ? value : 0;
		}
		static void WriteLine(XamlWriter writer, XamlExportContext exportContext, VisualBrick visualBrick, PointF startPoint, PointF endPoint) {
			writer.WriteStartElement(XamlTag.Line);
			if(exportContext.ResourceMap.BorderDashStylesDictionary.ContainsKey(visualBrick)) {
				string borderDashStyleName = exportContext.ResourceMap.BorderDashStylesDictionary[visualBrick];
				writer.WriteAttribute(XamlAttribute.Style, string.Format(staticResourceFormat, borderDashStyleName));
			}
			writer.WriteAttribute(XamlAttribute.X1, startPoint.X);
			writer.WriteAttribute(XamlAttribute.X2, endPoint.X);
			writer.WriteAttribute(XamlAttribute.Y1, startPoint.Y);
			writer.WriteAttribute(XamlAttribute.Y2, endPoint.Y);
			writer.WriteAttribute(XamlAttribute.StrokeDashArray, VisualBrick.GetDashPattern(visualBrick.Style.BorderDashStyle));
			const int overBorderZIndex = 1;
			writer.WriteAttribute(XamlAttribute.CanvasZIndex, overBorderZIndex);
			writer.WriteEndElement();
		}
		static void WriteAttachedValues(XamlWriter writer, XamlExportContext exportContext, VisualBrick visualBrick, Action<XamlWriter, object> writeCustomProperties) {
			writer.WriteAttribute(XamlNsPrefix.dxpn, XamlAttribute.VisualHelperIsVisualBrickBorder, true.ToString());
			WriteTagAttribute(writer, exportContext, visualBrick);
			if(visualBrick.Value != null && !visualBrick.Value.Equals(string.Empty)) {
				writer.WriteAttribute(XamlNsPrefix.dxpn, XamlAttribute.PreviewClickHelperTag,
					string.Format(CultureInfo.InvariantCulture, "{0}", visualBrick.Value));
			}
			if(visualBrick.NavigationPair != BrickPagePair.Empty) {
				string navigationPair = DocumentMapTreeViewNodeHelper.GetTagByIndices(visualBrick.NavigationBrickIndices, visualBrick.NavigationPageIndex);
				writer.WriteAttribute(XamlNsPrefix.dxpn, XamlAttribute.PreviewClickHelperNavigationPair, navigationPair);
			} else if(!string.IsNullOrEmpty(visualBrick.GetActualUrl()))
				writer.WriteAttribute(XamlNsPrefix.dxpn, XamlAttribute.PreviewClickHelperUrl, visualBrick.GetActualUrl());
		}
	}
}
