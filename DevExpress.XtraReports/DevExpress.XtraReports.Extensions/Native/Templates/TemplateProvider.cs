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

using System.IO;
using System.Text;
using System.Runtime.Serialization.Json;
using System.Net;
using DevExpress.Utils.OAuth;
using System;
using System.Drawing;
using System.Collections.Generic;
using DevExpress.XtraReports.Native.Templates;
using DevExpress.Utils.Zip;
using DevExpress.XtraReports.UI;
using DevExpress.XtraPrinting;
using DevExpress.XtraReports.Templates;
namespace DevExpress.XtraReports.Native.Templates {
	public abstract class TemplateProvider {
		protected readonly ITemplateArchiveManager templateArchiveManager;
		protected string extension;
		public TemplateProvider(string extension, string category) {
			this.extension = extension;
			templateArchiveManager = new TemplateArchiveManager(extension, category);
		}
		SizeF GetReportPageSize(byte[] bytes) {
			try {
				using(XtraReport report = new XtraReport()) {
					using(MemoryStream ms = new MemoryStream(bytes)) {
						report.LoadLayoutFromXml(ms);
						return XRConvert.Convert(report.PageSize, report.Dpi, GraphicsDpi.Pixel);
					}
				}
			} catch {
				return SizeF.Empty;
			}
		}
		protected void SetReportRegularImage(Template template,byte[] imageBytes,byte[] layoutBytes) {
			SizeF reportSize = GetReportPageSize(layoutBytes);
			if(reportSize != SizeF.Empty) {
				using(Image image = Image.FromStream(new MemoryStream(imageBytes))) {
					using(Image croppedImage = FormTemplateHelper.CreateCroppedImage(image, reportSize)) {
						template.PreviewBytes = FormTemplateHelper.ImageToArray(croppedImage);
					}
				}
			}
		}
	}	   
}
