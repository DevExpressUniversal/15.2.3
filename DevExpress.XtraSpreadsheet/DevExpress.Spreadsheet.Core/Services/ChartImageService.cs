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
using DevExpress.Compatibility.System.Drawing;
using DevExpress.XtraSpreadsheet.Model;
#if !SL
using ChartsModel = DevExpress.Charts.Model;
#else
using System.Windows.Controls;
#endif
namespace DevExpress.XtraSpreadsheet.Services {
	public interface IChartImageService {
		Image GetImage(Chart chart, Rectangle bounds);
	}
}
namespace DevExpress.XtraSpreadsheet.Services.Implementation {
	public class ChartImageService : IChartImageService {
#if !SL && !DXPORTABLE
		public Image GetImage(Chart chart, Rectangle bounds) {
			IChartControllerFactoryService service = chart.DocumentModel.GetService<IChartControllerFactoryService>();
			if (service == null || service.Factory == null || chart.Controller == null)
				return null;
			Rectangle pixelBounds = chart.DocumentModel.LayoutUnitConverter.LayoutUnitsToPixels(bounds, DocumentModel.DpiX, DocumentModel.DpiY);
			if (pixelBounds.Width == 0 || pixelBounds.Height == 0)
				return null;
			Bitmap image = new Bitmap(pixelBounds.Width, pixelBounds.Height);
			using (Graphics graphics = Graphics.FromImage(image)) {
				ChartsModel.ModelRect rect = new ChartsModel.ModelRect(0, 0, pixelBounds.Width, pixelBounds.Height);
				chart.Controller.RenderChart(service.Factory.CreateRenderContext(rect, graphics));
			}
			if (chart.Is3DChart)
				image.RotateFlip(RotateFlipType.RotateNoneFlipY);
			return image;
		}
#else
		public Image GetImage(Chart chart, Rectangle bounds) {
			return null;
		}
#endif
	}
}
