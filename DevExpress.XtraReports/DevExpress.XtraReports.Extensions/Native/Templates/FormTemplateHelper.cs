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
using System.Linq;
using System.Text;
using System.Drawing;
using System.IO;
using System.Drawing.Drawing2D;
using DevExpress.XtraReports.UI;
using System.Reflection;
using System.ComponentModel;
namespace DevExpress.XtraReports.Native.Templates {
	 public static class FormTemplateHelper {
		 public static Bitmap CreateImage(Image source, Size imageSize, Color backColor) {
			RectangleF rect = InscribeSize(source.Size, imageSize);
			Bitmap bitmap = new Bitmap(imageSize.Width, imageSize.Height);
			using(Graphics g = Graphics.FromImage(bitmap)) {
				using(Brush brush = new SolidBrush(backColor)) {
					g.FillRectangle(brush, 0, 0, imageSize.Width, imageSize.Height);
				}
				g.DrawImage(source, rect);
			}
			return bitmap;
		}
		public static Image CreateCroppedImage(Image source, SizeF size) {
			RectangleF rect = InscribeSize(size, source.Size);
			Bitmap bitmap = new Bitmap((int)rect.Width, (int)rect.Height);
			using(Graphics graphics = Graphics.FromImage(bitmap)) {
				graphics.DrawImage(source, new Rectangle(0, 0, bitmap.Width, bitmap.Height), rect, GraphicsUnit.Pixel);
			}
			return bitmap;
		}
		public static RectangleF InscribeSize(SizeF size, SizeF baseSize) {
			float sourceWidth = size.Width;
			float sourceHeight = size.Height;
			float nPercentW = baseSize.Width / sourceWidth;
			float nPercentH = baseSize.Height / sourceHeight;
			float nPercent = Math.Min(nPercentH, nPercentW);
			SizeF newSize = new SizeF(sourceWidth * nPercent, sourceHeight * nPercent);
			PointF location = new PointF((baseSize.Width - newSize.Width) / 2, (baseSize.Height - newSize.Height) / 2);
			return new RectangleF(location, newSize);
		}
		 public static byte[] ImageToArray(System.Drawing.Image image) {
			using(MemoryStream ms = new MemoryStream()) {
				image.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
				return ms.ToArray();
			}
		}
		public static void ClearReportScripts(XtraReport report) {
			DevExpress.XtraReports.Native.NestedComponentEnumerator enumerator = new DevExpress.XtraReports.Native.NestedComponentEnumerator(report.Bands);
			report.ScriptsSource = string.Empty;
			while (enumerator.MoveNext()) {
				XRControl xrControl = (XRControl)enumerator.Current;
				IList<PropertyDescriptor> list = GetProperties(xrControl.Scripts, typeof(EventScriptAttribute));
				foreach (PropertyDescriptor pd in list) {
					pd.SetValue(xrControl.Scripts, string.Empty);
				}
			}
		}
		static IList<PropertyDescriptor> GetProperties(object obj, Type attributeType) {
			List<PropertyDescriptor> result = new List<PropertyDescriptor>();
			PropertyDescriptorCollection props = TypeDescriptor.GetProperties(obj.GetType());
			foreach (PropertyDescriptor property in props) {
				if (property.Attributes[attributeType] != null)
					result.Add(property);
			}
			return result;
		}
	}
}
