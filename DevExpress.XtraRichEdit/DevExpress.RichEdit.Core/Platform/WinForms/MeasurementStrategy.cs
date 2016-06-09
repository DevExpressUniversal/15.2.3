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
using DevExpress.XtraRichEdit.Model;
using DevExpress.XtraRichEdit.Utils;
using DevExpress.XtraRichEdit.Layout.Engine;
using DevExpress.XtraRichEdit.Layout.Export;
using DevExpress.Office.Drawing;
using DevExpress.Office.Utils;
namespace DevExpress.XtraRichEdit.Drawing {
	#region GdiPlusMeasurementAndDrawingStrategyBase (abstract class)
	public abstract class GdiPlusMeasurementAndDrawingStrategyBase : MeasurementAndDrawingStrategy {
		#region Fields
		GraphicsToLayoutUnitsModifier graphicsModifier;
		Graphics measureGraphics;
		#endregion
		protected GdiPlusMeasurementAndDrawingStrategyBase(DocumentModel documentModel)
			: base(documentModel) {
		}
		public override void Initialize() {
			this.measureGraphics = CreateMeasureGraphics();
			base.Initialize();
		}
		#region Properties
		public Graphics MeasureGraphics { get { return measureGraphics; } }
		#endregion
		protected override void Dispose(bool disposing) {
			try {
				if (disposing) {
					if (graphicsModifier != null) {
						graphicsModifier.Dispose();
						graphicsModifier = null;
					}
					if (measureGraphics != null) {
						measureGraphics.Dispose();
						measureGraphics = null;
					}
				}
			}
			finally {
				base.Dispose(disposing);
			}
		}
		protected internal override void OnLayoutUnitChanged() {
			this.graphicsModifier.Dispose();
			this.graphicsModifier = new GraphicsToLayoutUnitsModifier(measureGraphics, DocumentModel.LayoutUnitConverter);
			base.OnLayoutUnitChanged();
		}
		protected internal virtual Graphics CreateMeasureGraphics() {
			Graphics result = Graphics.FromHwnd(IntPtr.Zero);
			this.graphicsModifier = new GraphicsToLayoutUnitsModifier(result, DocumentModel.LayoutUnitConverter);
			return result;
		}
		public override BoxMeasurer CreateBoxMeasurer() {
			return new GdiPlusBoxMeasurer(DocumentModel, MeasureGraphics);
		}
		public override FontCacheManager CreateFontCacheManager() {
			return new GdiPlusFontCacheManager(DocumentModel.LayoutUnitConverter);
		}
	}
	#endregion
	#region GdiMeasurementAndDrawingStrategyBase (abstract class)
	public abstract class GdiMeasurementAndDrawingStrategyBase : GdiPlusMeasurementAndDrawingStrategyBase {
		GraphicsToLayoutUnitsModifier hdcGraphicsModifier;
		Graphics hdcMeasureGraphics;
		protected GdiMeasurementAndDrawingStrategyBase(DocumentModel documentModel)
			: base(documentModel) {
		}
		protected override void Dispose(bool disposing) {
			try {
				if (disposing) {
					GdiBoxMeasurerLockHdc measurer = Measurer as GdiBoxMeasurerLockHdc;
					if (measurer != null)
						measurer.ReleaseCachedHdc();
					if (hdcGraphicsModifier != null) {
						hdcGraphicsModifier.Dispose();
						hdcGraphicsModifier = null;
					}
					if (hdcMeasureGraphics != null) {
						hdcMeasureGraphics.Dispose();
						hdcMeasureGraphics = null;
					}
				}
			}
			finally {
				base.Dispose(disposing);
			}
		}
		~GdiMeasurementAndDrawingStrategyBase() {
			Dispose(false);
		}
		public override void Initialize() {
			this.hdcMeasureGraphics = Graphics.FromHwnd(IntPtr.Zero);
			this.hdcGraphicsModifier = new GraphicsToLayoutUnitsModifier(hdcMeasureGraphics, DocumentModel.LayoutUnitConverter);
			base.Initialize();
		}
		protected internal override void OnLayoutUnitChanged() {
			GdiBoxMeasurerLockHdc measurer = Measurer as GdiBoxMeasurerLockHdc;
			if (measurer != null)
				measurer.ReleaseCachedHdc();
			this.hdcGraphicsModifier.Dispose();
			this.hdcGraphicsModifier = new GraphicsToLayoutUnitsModifier(hdcMeasureGraphics, DocumentModel.LayoutUnitConverter);
			if (measurer != null)
				measurer.ObtainCachedHdc();
			base.OnLayoutUnitChanged();
		}
		public override BoxMeasurer CreateBoxMeasurer() {
			return new GdiBoxMeasurerLockHdc(DocumentModel, MeasureGraphics, hdcMeasureGraphics);
		}
		public override FontCacheManager CreateFontCacheManager() {
			return new GdiFontCacheManager(DocumentModel.LayoutUnitConverter);
		}
	}
	#endregion
	#region ServerGdiPlusMeasurementAndDrawingStrategy
	public class ServerGdiPlusMeasurementAndDrawingStrategy : GdiPlusMeasurementAndDrawingStrategyBase {
		public ServerGdiPlusMeasurementAndDrawingStrategy(DocumentModel documentModel)
			: base(documentModel) {
		}
		public override Painter CreateDocumentPainter(IDrawingSurface surface) {
			return new EmptyPainter();
		}
	}
	#endregion
	#region ServerGdiMeasurementAndDrawingStrategy
	public class ServerGdiMeasurementAndDrawingStrategy : GdiMeasurementAndDrawingStrategyBase {
		public ServerGdiMeasurementAndDrawingStrategy(DocumentModel documentModel)
			: base(documentModel) {
		}
		public override Painter CreateDocumentPainter(IDrawingSurface surface) {
			return new EmptyPainter();
		}
	}
	#endregion
}
