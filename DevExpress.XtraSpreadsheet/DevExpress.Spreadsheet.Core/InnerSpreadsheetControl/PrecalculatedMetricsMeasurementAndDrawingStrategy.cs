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
using DevExpress.XtraSpreadsheet;
using DevExpress.XtraSpreadsheet.Drawing;
using DevExpress.XtraSpreadsheet.Layout;
using DevExpress.XtraSpreadsheet.Layout.Engine;
using DevExpress.XtraSpreadsheet.Model;
using DevExpress.XtraSpreadsheet.Utils;
using DevExpress.Office.Drawing;
using DevExpress.Office.Layout;
using DevExpress.Office.Utils;
#if !SL
#else
using DevExpress.Xpf.Drawing;
#endif
namespace DevExpress.XtraSpreadsheet.Internal {
	#region PrecalculatedMetricsMeasurementAndDrawingStrategyBase (abstract class)
	public abstract class PrecalculatedMetricsMeasurementAndDrawingStrategyBase : MeasurementAndDrawingStrategy {
		protected PrecalculatedMetricsMeasurementAndDrawingStrategyBase(DocumentModel documentModel)
			: base(documentModel) {
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
