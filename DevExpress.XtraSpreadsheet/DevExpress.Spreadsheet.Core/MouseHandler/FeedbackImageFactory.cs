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
using DevExpress.Office.Utils;
using DevExpress.XtraSpreadsheet.Model;
using DevExpress.XtraSpreadsheet.Layout;
using DevExpress.Compatibility.System.Drawing;
#if !SL
using System.Drawing;
using System.Drawing.Imaging;
using DevExpress.XtraSpreadsheet.Services;
using ChartsModel = DevExpress.Charts.Model;
#endif
namespace DevExpress.XtraSpreadsheet.Mouse {
	public class FeedbackImageFactory : IDrawingBoxVisitor {
		OfficeImage result;
		public OfficeImage CreateImage(DrawingBox box) {
			box.Visit(this);
			return result;
		}
		#region IDrawingBoxVisitor Members
		public void Visit(PictureBox value) {
			this.result = value.Image.Clone(value.Picture.DocumentModel);
		}
		public void Visit(ChartBox value) {
			result = null;
#if !SL && !DXPORTABLE
			Chart chart = value.Chart;
			IChartControllerFactoryService service = chart.DocumentModel.GetService<IChartControllerFactoryService>();
			if (service == null || service.Factory == null || chart.Controller == null)
				return;
			Rectangle bounds = value.Bounds;
			if (chart.Is3DChart) {
				OfficeNativeImage cachedImage = chart.GetCachedImage(bounds.Size) as OfficeNativeImage;
				if(cachedImage != null)
					result = new OfficeReferenceImage(chart.DocumentModel, cachedImage);
			}
			else {
				Image nativeImage = new Bitmap(bounds.Width, bounds.Height);
				using (Graphics graphics = Graphics.FromImage(nativeImage)) {
					ChartsModel.ModelRect rect = new ChartsModel.ModelRect(0, 0, bounds.Width, bounds.Height);
					chart.Controller.RenderChart(service.Factory.CreateRenderContext(rect, graphics));
				}
				result = OfficeImage.CreateImage(nativeImage);
			}
#endif
		}
		public void Visit(ShapeBox value) {
#if !SL && !DXPORTABLE
			IShapeRenderService service = value.Shape.DocumentModel.GetService<IShapeRenderService>();
			if(service == null)
				return;
			Image nativeImage = new Bitmap(value.Bounds.Width, value.Bounds.Height);
			using(Graphics graphics = Graphics.FromImage(nativeImage)) {
				service.RenderShape(value.Shape, graphics, new Rectangle(0, 0, value.Bounds.Width, value.Bounds.Height));
			}
			result = OfficeImage.CreateImage(nativeImage);
#endif
		}
		#endregion
	}
}
