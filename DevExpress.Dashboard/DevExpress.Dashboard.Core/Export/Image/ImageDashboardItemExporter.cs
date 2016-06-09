#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       Dashboard                                                   }
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

using DevExpress.DashboardCommon.Printing;
using DevExpress.DashboardCommon.Service;
using DevExpress.DashboardCommon.ViewModel;
using DevExpress.XtraPrinting;
using System;
using System.Drawing;
using System.IO;
namespace DevExpress.DashboardExport {
	public class ImageDashboardItemExporter : DashboardItemExporter {
		DashboardImagePrinter printer;
		MemoryStream imageStream;
		public override IPrintable PrintableComponent {
			get { return printer; }
		}
		public ImageDashboardItemExporter(DashboardExportMode mode, DashboardItemExportData data)
			: base(mode, data) {
			byte[] imageData = null;
			Image image = null;
			string url = null;
			ImageDashboardItemViewModel viewModel = (ImageDashboardItemViewModel)ServerData.ViewModel;
			string base64SourceString = viewModel.ImageViewModel.SourceBase64String;
			if(!String.IsNullOrEmpty(base64SourceString)) {
				imageData = Convert.FromBase64String(base64SourceString);
				imageStream = new MemoryStream(imageData);
				image = Image.FromStream(imageStream);
			}
			else
				url = viewModel.ImageViewModel.Url;
			Size size = new Size(data.ViewerClientState.ViewerArea.Width, data.ViewerClientState.ViewerArea.Height);
			this.printer = new DashboardImagePrinter(image, url, GetExportImageSizeMode(viewModel.SizeMode), size, viewModel.HorizontalAlignment, viewModel.VerticalAlignment);
		}
		protected override void Dispose(bool disposing) {
			if(disposing) {
				printer.Dispose();
				printer = null;
				if(imageStream != null) {
					imageStream.Dispose();
					imageStream = null;
				}
			}
			base.Dispose(disposing);
		}
		XtraPrinting.ImageSizeMode GetExportImageSizeMode(DashboardCommon.ImageSizeMode sizeMode) {
			switch(sizeMode) {
				case DashboardCommon.ImageSizeMode.Clip:
					return XtraPrinting.ImageSizeMode.AutoSize;
				case DashboardCommon.ImageSizeMode.Squeeze:
					return XtraPrinting.ImageSizeMode.Squeeze;
				case DashboardCommon.ImageSizeMode.Stretch:
					return XtraPrinting.ImageSizeMode.StretchImage;
				case DashboardCommon.ImageSizeMode.Zoom:
					return XtraPrinting.ImageSizeMode.ZoomImage;
				default:
					throw new NotSupportedException();
			}
		}
	}
}
