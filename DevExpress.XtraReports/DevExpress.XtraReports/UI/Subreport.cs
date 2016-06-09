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
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using DevExpress.XtraPrinting.Export.Web;
using DevExpress.XtraPrinting.Export.Pdf;
using DevExpress.XtraPrinting.Export.Rtf;
using DevExpress.XtraPrinting;
using System.ComponentModel.Design;
using System.Runtime.Serialization;
using DevExpress.XtraReports.Localization;
using System.IO;
using System.Reflection;
using System.Net;
using DevExpress.XtraReports.Serialization;
using DevExpress.XtraReports.Native;
using DevExpress.XtraPrinting.Native;
using DevExpress.XtraPrinting.Export;
using System.Collections.Specialized;
using DevExpress.XtraReports.Native.Printing;
using DevExpress.Utils;
using DevExpress.Utils.Serializing;
namespace DevExpress.XtraReports.UI {
	[
	ToolboxItem(false),
	Obsolete("The Subreport class is now obsolete. Use the XRSubreport class instead."),
	EditorBrowsable(EditorBrowsableState.Never),
	]
	public class Subreport : SubreportBase {
		private static Bitmap bitmap = ResourceImageHelper.CreateBitmapFromResources("Images.Subreport.gif", typeof(ResFinder));
		#region fields & properties
		[
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		XtraSerializableProperty(XtraSerializationVisibility.Hidden),
		]
		public override SizeF SizeF {
			get { return base.SizeF; }
			set { base.SizeF = value; }
		}
		public override float Dpi {
			get { return base.Dpi; }
			set {
				base.Dpi = value;
				UpdateBounds();
			}
		}
		#endregion
		protected internal override bool CanDrawBackground { get { return false; } }
		public Subreport() {
			UpdateBounds();
		}
		protected internal override void SyncDpi(float dpi) {
			base.SyncDpi(dpi);
			UpdateBounds();
		}
		#region Serialization
		protected override void DeserializeProperties(XRSerializer serializer) {
			base.DeserializeProperties(serializer);
			Type t = serializer.DeserializeType("SubrepType", null);
			if(t != null) {
				ReportSource = Activator.CreateInstance(t) as XtraReport;
				string reportContent = serializer.DeserializeString("SubReportContent", "");
				if(!String.IsNullOrEmpty(reportContent)) {
					MemoryStream memoryStream = new MemoryStream();
					StreamWriter streamWriter = new StreamWriter(memoryStream);
					streamWriter.Write(reportContent);
					streamWriter.Flush();
					ReportSource.LoadLayout(memoryStream);
				}
			}
		}
		#endregion
		#region Events
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public override event ChangeEventHandler SizeChanged { add { } remove { } }
		#endregion
		private void UpdateBounds() {
			Size size = XRConvert.Convert(bitmap.Size, GraphicsDpi.Pixel, Dpi);
			SetBoundsCore(0, 0, size.Width, size.Height, BoundsSpecified.Size);
		}
		protected override void SetBounds(float x, float y, float width, float height, BoundsSpecified specified) {
			specified &= ~BoundsSpecified.Size;
			base.SetBounds(x, y, width, height, specified);
		}
		protected internal override void PutStateToBrick(VisualBrick brick, PrintingSystemBase ps) {
			base.PutStateToBrick(brick, ps);
			brick.Padding = PaddingInfo.Empty;
			if(brick is SubreportBrick && !this.RootReport.ShrinkSubReportIconArea)
				((SubreportBrick)brick).AdditionalBottomSpan = brick.Rect.Height;
		}
		protected override VisualBrick CreateBrick(VisualBrick[] childrenBricks) {
			if(DesignMode) {
				ImageBrick imageBrick = new ImageBrick();
				imageBrick.SizeMode = ImageSizeMode.Normal;
				imageBrick.Image = bitmap;
				return imageBrick;
			}
			return base.CreateBrick(childrenBricks);
		}
	}
}
