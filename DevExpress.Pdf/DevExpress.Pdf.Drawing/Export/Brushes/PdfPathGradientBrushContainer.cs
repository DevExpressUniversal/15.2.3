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
using System.IO;
using System.Drawing;
using System.Security;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
namespace DevExpress.Pdf.Drawing {
	public class PdfPathGradientBrushContainer : PdfTilingBrushContainer {
		[SecuritySafeCritical]
		public static PdfPathGradientBrushContainer CreateFromPathGradientBrush(PathGradientBrush brush) {
			PdfPathGradientBrushContainer pdfBrushContainer = null;
			using (Bitmap bmp = new Bitmap(1, 1)) {
				using (Graphics graphics = Graphics.FromImage(bmp)) {
					IntPtr hdc = graphics.GetHdc();
					using (Metafile metafile = new Metafile(new MemoryStream(), hdc, EmfType.EmfPlusOnly)) {
						using (Graphics metafileGraphics = Graphics.FromImage(metafile))
							metafileGraphics.FillRectangle(brush, new Rectangle(0, 0, 100, 100));
						using (EmfMetafile meta = new EmfMetafile(metafile)) {
							foreach (EmfRecord record in meta.Records) {
								EmfPlusObjectRecord objectRecord = record as EmfPlusObjectRecord;
								if (objectRecord != null) {
									EmfPlusBrush emfBrush = objectRecord.Value as EmfPlusBrush;
									if (emfBrush == null)
										return null;
									pdfBrushContainer = emfBrush.BrushContainer as PdfPathGradientBrushContainer;
								}
							}
						}
					}
					graphics.ReleaseHdc(hdc);
				}
			}
			return pdfBrushContainer;
		}
		public PdfPathGradientBrushContainer(PdfPathGradientBrush brush) {
			SetBrush(brush);
		}
	}
}
