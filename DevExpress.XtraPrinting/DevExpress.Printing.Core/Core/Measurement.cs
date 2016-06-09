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
namespace DevExpress.XtraPrinting.Native {
	public abstract class Measurer : IMeasurer, IDisposable {
		Graphics graph;
		protected Graphics Graph {
			get { return graph; }
		}
		public Measurer()
			: this(null) {
		}
		protected Measurer(IServiceProvider servProvider) {
			graph = DevExpress.XtraPrinting.Native.GraphicsHelper.CreateGraphics();
		}
		public virtual void Dispose() {
			if(graph != null) {
				graph.Dispose();
				graph = null;
			}
		}
		public abstract RectangleF GetRegionBounds(Region rgn, GraphicsUnit pageUnit);
		public abstract Region[] MeasureCharacterRanges(string text, Font font, RectangleF layoutRect, StringFormat stringFormat, GraphicsUnit pageUnit);
		public abstract SizeF MeasureString(string text, Font font, PointF location, StringFormat stringFormat, GraphicsUnit pageUnit);
		public abstract SizeF MeasureString(string text, Font font, SizeF size, StringFormat stringFormat, GraphicsUnit pageUnit);
		public SizeF MeasureString(string text, Font font, float width, StringFormat stringFormat, GraphicsUnit pageUnit) {
			return MeasureString(text, font, new SizeF(width, 999999f), stringFormat, pageUnit);
		}
		public SizeF MeasureStringI(string text, Font font, float width, StringFormat stringFormat, GraphicsUnit pageUnit) {
			if(pageUnit == GraphicsUnit.Pixel) {
				float dpi = graph.DpiX;
				if(Math.Abs(dpi - GraphicsDpi.DeviceIndependentPixel) > 0.01) {
					width = GraphicsUnitConverter.Convert(width, GraphicsDpi.DeviceIndependentPixel, dpi);
					SizeF result = MeasureString(text, font, width, stringFormat, pageUnit);
					return GraphicsUnitConverter.Convert(result, dpi, GraphicsDpi.DeviceIndependentPixel);
				}
			}
			return MeasureString(text, font, width, stringFormat, pageUnit);
		}
		public SizeF MeasureString(string text, Font font, GraphicsUnit pageUnit) {
			return MeasureString(text, font, new SizeF(0f, 0f), null, pageUnit);
		}
#if DEBUGTEST
		public Graphics Test_Graph {
			get { return graph; }
			set { graph = value; }
		} 
#endif
	}
	public class GdiPlusMeasurer : Measurer {
		public GdiPlusMeasurer()
			: this(null) {
		}
		public GdiPlusMeasurer(IServiceProvider servProvider)
			: base(servProvider) { 
		}
		public override RectangleF GetRegionBounds(Region rgn, GraphicsUnit pageUnit) {
			Graph.PageUnit = pageUnit;
			return rgn.GetBounds(Graph);
		}
		public override Region[] MeasureCharacterRanges(string text, Font font, RectangleF layoutRect, StringFormat stringFormat, GraphicsUnit pageUnit) {
			Graph.PageUnit = pageUnit;
			return Graph.MeasureCharacterRanges(text, font, layoutRect, stringFormat);
		}
		public override SizeF MeasureString(string text, Font font, PointF location, StringFormat stringFormat, GraphicsUnit pageUnit) {
			Graph.PageUnit = pageUnit;
			SizeF result = Graph.MeasureString(text, font, location, stringFormat);
			if(!string.IsNullOrEmpty(text))
				foreach(string oneStringText in TextFormatter.SplitTextByNewLine(text))
					result.Width = Math.Max(result.Width, Graph.MeasureString(oneStringText, font, location, stringFormat).Width);
			return result;
		}
		public override SizeF MeasureString(string text, Font font, SizeF size, StringFormat stringFormat, GraphicsUnit pageUnit) {
			Graph.PageUnit = pageUnit;
			SizeF result = Graph.MeasureString(text, font, size, stringFormat);
			if(!string.IsNullOrEmpty(text))
				foreach(string oneStringText in TextFormatter.SplitTextByNewLine(text))
					result.Width = Math.Max(result.Width, Graph.MeasureString(oneStringText, font, size, stringFormat).Width);
			return result;
		}
	}
	public class Measurement {
		[ThreadStatic]
		static Measurer measurer;
		public static Measurer Measurer {
			get {
				if(measurer == null)
					measurer = new GdiPlusMeasurer();
				return measurer;
			}
		}
		public const string FontMeasureGlyph = "gM";
		public static SizeF MeasureString(string text, Font font, PointF location, StringFormat stringFormat, GraphicsUnit pageUnit) {
			lock(typeof(Measurement))
				return Measurer.MeasureString(text, font, location, stringFormat, pageUnit);
		}
		public static SizeF MeasureString(string text, Font font, SizeF size, StringFormat stringFormat, GraphicsUnit pageUnit) {
			lock(typeof(Measurement))
				return Measurer.MeasureString(text, font, size, stringFormat, pageUnit);
		}
		public static SizeF MeasureString(string text, Font font, float width, StringFormat stringFormat, GraphicsUnit pageUnit) {
			lock(typeof(Measurement))
				return Measurer.MeasureString(text, font, width, stringFormat, pageUnit);
		}
		public static SizeF MeasureString(string text, Font font, GraphicsUnit pageUnit) {
			lock(typeof(Measurement))
				return Measurer.MeasureString(text, font, pageUnit);
		}
	}
}
