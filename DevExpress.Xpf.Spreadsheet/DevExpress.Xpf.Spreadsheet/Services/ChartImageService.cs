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
using System.IO;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using DevExpress.XtraSpreadsheet.Model;
using ChartsModel = DevExpress.Charts.Model;
namespace DevExpress.XtraSpreadsheet.Services.Implementation {
	public class WpfChartImageService : IChartImageService {
		public System.Drawing.Image GetImage(Chart chart, Rectangle bounds) {
			IChartControllerFactoryService service = chart.DocumentModel.GetService<IChartControllerFactoryService>();
			if (service == null || service.Factory == null || chart.Controller == null)
				return null;
			bounds = chart.DocumentModel.LayoutUnitConverter.LayoutUnitsToPixels(bounds, DocumentModel.DpiX, DocumentModel.DpiY);
			ContentPresenter chartContainer = CreateChartTree(chart, bounds, service);
			System.Windows.Rect rect = new System.Windows.Rect(new System.Windows.Point(), new System.Windows.Size(bounds.Width, bounds.Height));
			return ConvertChartTreeToImage(chartContainer, rect);
		}
		ContentPresenter CreateChartTree(Chart chart, Rectangle bounds, IChartControllerFactoryService service) {
			ContentPresenter chartContainer = new ContentPresenter();
			ChartsModel.ModelRect rect = new ChartsModel.ModelRect(0, 0, bounds.Width, bounds.Height);
			chart.Controller.RenderChart(service.Factory.CreateRenderContext(rect, chartContainer));
			return chartContainer;
		}
		System.Drawing.Image ConvertChartTreeToImage(ContentPresenter chartContainer, System.Windows.Rect bounds) {
			int pixelWidth = (int)(bounds.Width * DocumentModel.DpiX / 96);
			int pixelHeight = (int)(bounds.Height * DocumentModel.DpiY / 96);
			RenderTargetBitmap renderTarget = new RenderTargetBitmap(pixelWidth, pixelHeight, DocumentModel.DpiX, DocumentModel.DpiY, System.Windows.Media.PixelFormats.Default);
			chartContainer.Measure(bounds.Size);
			chartContainer.Arrange(bounds);
			chartContainer.ApplyTemplate();
			chartContainer.UpdateLayout();
			renderTarget.Render(chartContainer);
			BitmapEncoder encoder = new System.Windows.Media.Imaging.PngBitmapEncoder();
			MemoryStream stream = new MemoryStream();
			encoder.Frames.Add(System.Windows.Media.Imaging.BitmapFrame.Create(renderTarget));
			encoder.Save(stream);
			return System.Drawing.Image.FromStream(stream);
		}
	}
}
