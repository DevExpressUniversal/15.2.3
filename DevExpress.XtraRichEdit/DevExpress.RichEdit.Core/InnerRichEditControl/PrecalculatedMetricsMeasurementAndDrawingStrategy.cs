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
using System.Drawing;
using DevExpress.Data.Helpers;
using DevExpress.Utils.Internal;
using DevExpress.XtraRichEdit;
using DevExpress.XtraRichEdit.Drawing;
using DevExpress.XtraRichEdit.Layout;
using DevExpress.XtraRichEdit.Layout.Engine;
using DevExpress.XtraRichEdit.Model;
using DevExpress.XtraRichEdit.Utils;
using DevExpress.Office.Drawing;
using DevExpress.Office.Layout;
using DevExpress.Office.Utils;
using DevExpress.Compatibility.System.Drawing;
#if !SL
#else
using DevExpress.Xpf.Drawing;
#endif
namespace DevExpress.XtraRichEdit.Internal {
	#region PrecalculatedMetricsMeasurementAndDrawingStrategyBase (abstract class)
	public abstract class PrecalculatedMetricsMeasurementAndDrawingStrategyBase : MeasurementAndDrawingStrategy {
		protected PrecalculatedMetricsMeasurementAndDrawingStrategyBase(DocumentModel documentModel)
			: base(documentModel) {
		}
		public override BoxMeasurer CreateBoxMeasurer() {
			return new PrecalculatedMetricsBoxMeasurer(DocumentModel);
		}
		public override FontCacheManager CreateFontCacheManager() {
			return new PrecalculatedMetricsFontCacheManager(DocumentModel.LayoutUnitConverter);
		}
	}
	#endregion
	#region ServerPrecalculatedMetricsMeasurementAndDrawingStrategy
	public class ServerPrecalculatedMetricsMeasurementAndDrawingStrategy : PrecalculatedMetricsMeasurementAndDrawingStrategyBase {
		public ServerPrecalculatedMetricsMeasurementAndDrawingStrategy(DocumentModel documentModel)
			: base(documentModel) {
		}
		public override Painter CreateDocumentPainter(IDrawingSurface surface) {
			return new EmptyPainter();
		}
	}
	#endregion
}
namespace DevExpress.XtraRichEdit.Layout.Engine {
	public class MeasureQueueItem {
		readonly BoxInfo boxInfo;
		readonly string text;
		readonly FontInfo fontInfo;
		public MeasureQueueItem(BoxInfo boxInfo, string text, FontInfo fontInfo) {
			this.boxInfo = boxInfo;
			this.text = text;
			this.fontInfo = fontInfo;
		}
		public BoxInfo BoxInfo { get { return boxInfo; } }
		public string Text { get { return text; } }
		public FontInfo FontInfo { get { return fontInfo; } }
	}
	public class PrecalculatedMetricsBoxMeasurer : BoxMeasurer {
		PrecalculatedMetricsFontInfoMeasurer fontInfoMeasurer;
		public PrecalculatedMetricsBoxMeasurer(DocumentModel documentModel)
			: base(documentModel) {
			this.fontInfoMeasurer = new PrecalculatedMetricsFontInfoMeasurer(DocumentModel.LayoutUnitConverter);
		}
		#region Properties
		protected PrecalculatedMetricsFontInfoMeasurer FontInfoMeasurer { get { return fontInfoMeasurer; } }
		#endregion
		public override void MeasureText(BoxInfo boxInfo, string text, FontInfo fontInfo) {
			TextViewInfo textViewInfo = CreateTextViewInfo(boxInfo, text, fontInfo);
			boxInfo.Size = textViewInfo.Size;
		}
		public override Rectangle[] MeasureCharactersBounds(string text, FontInfo fontInfo, Rectangle bounds) {
			PrecalculatedMetricsFontInfo precalculatedMetricsFontInfo = (PrecalculatedMetricsFontInfo)fontInfo;
			Rectangle[] res = FontInfoMeasurer.MeasureCharactersBounds(text, precalculatedMetricsFontInfo);
			for (int i = 0; i < res.Length; i++)
				res[i].Offset(bounds.X, bounds.Y);
			return res;
		}
		protected internal override TextViewInfo CreateTextViewInfo(BoxInfo boxInfo, string text, FontInfo fontInfo) {
			TextViewInfo result = new TextViewInfo();
			result.Size = FontInfoMeasurer.MeasureString(text, fontInfo);
			return result;
		}
		protected internal override void OnLayoutUnitChanged() {
			base.OnLayoutUnitChanged();
			this.fontInfoMeasurer = new PrecalculatedMetricsFontInfoMeasurer(DocumentModel.LayoutUnitConverter);
		}
	}
}
