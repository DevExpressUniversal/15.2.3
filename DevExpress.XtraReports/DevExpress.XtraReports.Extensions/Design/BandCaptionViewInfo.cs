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
using System.Text;
using System.Drawing;
using DevExpress.Utils.Drawing;
using DevExpress.XtraReports.UI;
using DevExpress.XtraPrinting;
using DevExpress.XtraPrinting.Native;
namespace DevExpress.XtraReports.Design {
	public class BandCaptionViewInfo : ObjectInfoArgs {
		protected static Size buttonSize = new Size(13, 13);
		protected static Size imageSize = new Size(16, 16);
		static Font font;
		private static StringFormat stringFormat;
		public static StringFormat StringFormat {
			get { return stringFormat; }
		}
		static BandCaptionViewInfo() {
			stringFormat = (StringFormat)StringFormat.GenericTypographic.Clone();
			stringFormat.Alignment = StringAlignment.Near;
			stringFormat.LineAlignment = StringAlignment.Center;
			font = SystemFonts.DefaultFont;
		}
		protected RectangleF fButtonBounds;
		protected RectangleF fImageBounds;
		protected RectangleF fTextBounds;
		Image image;
		int level;
		IDesignFrame designFrame;
		bool locked;
		bool hasBottomBorder;
		bool hasTopBorder;
		public int Level {
			get { return level; }
		}
		public RectangleF ButtonBounds {
			get { return fButtonBounds; }
			set { fButtonBounds = value; }
		}
		public RectangleF TextBounds {
			get { return fTextBounds; }
			set { fTextBounds = value; }
		}
		public string Text {
			get { return designFrame.Text; }
		}
		public bool Expanded {
			get { return designFrame.Expanded; }
		}
		public Image Image {
			get { return image; }
		}
		public static Font Font {
			get { return font; }
		}
		public bool Selected {
			get { return ((XRControlDesignerBase)designFrame).IsComponentSelected; }
		}
		public RectangleF ImageBounds {
			get { return fImageBounds; }
			set { fImageBounds = value; }
		}
		public Band Band {
			get { return designFrame.Band; }
		}
		public virtual bool HasBottomBorder { get { return hasBottomBorder; } set { hasBottomBorder = value; } }
		public virtual bool HasTopBorder { get { return hasTopBorder; } set { hasTopBorder = value; } }
		public bool Locked { get { return locked; } set { locked = value; } }
		protected int CaptionHeight {
			get { return GetCaptionHeight(HasBottomBorder); }
		}
		protected virtual int GetCaptionHeight(bool hasBottomBorder) {
			return hasBottomBorder ? ReportFrame.CaptionHeight : ReportFrame.CaptionHeight - 1;
		}
		public BandCaptionViewInfo(IDesignFrame designFrame, Image image, int level) {
			this.designFrame = designFrame;
			this.image = image;
			this.level = level;
		}
		public virtual RectangleF Calculate(RectangleF baseBounds, float bandHeight) {
			CalculateCore(new Rectangle((int)baseBounds.X, (int)baseBounds.Y, (int)baseBounds.Width, GetCaptionHeight(true)));
			Bounds = RectHelper.CeilingVertical(new RectangleF(baseBounds.X, baseBounds.Y, baseBounds.Width, CaptionHeight));
			return new RectangleF(baseBounds.X, Bounds.Bottom, baseBounds.Width, bandHeight);
		}
		void CalculateCore(Rectangle bounds) {
			fButtonBounds = new Rectangle(bounds.Location, buttonSize);
			const int Delta = 3;
			const int NewLevelOffset = 20;
			int x = NewLevelOffset * level;
			fButtonBounds.Offset(x + Delta, (bounds.Height - fButtonBounds.Height) / 2);
			CalculateImageBounds(bounds, Delta);
			CalculateTextBounds(bounds, Delta);
		}
		protected void CalculateImageBounds(Rectangle bounds, int delta) {
			fImageBounds = new Rectangle(bounds.Location, imageSize);
			fImageBounds.X = fButtonBounds.Right + delta;
			fImageBounds.Offset(0, (int)Math.Ceiling((bounds.Height - fImageBounds.Height) / 2f));
		}
		protected void CalculateTextBounds(Rectangle bounds, int delta) {
			fTextBounds = bounds;
			fTextBounds.X = fImageBounds.Right + delta;
			fTextBounds.Width = Size.Ceiling(Measurement.MeasureString(Text, font, 0, StringFormat, GraphicsUnit.Pixel)).Width;
		}
	}
	public class EmptyCaptionViewInfoBase : BandCaptionViewInfo {
		public override bool HasBottomBorder { get { return false; } set { } }
		public EmptyCaptionViewInfoBase(IDesignFrame designFrame)
			: base(designFrame, null, 0) {
		}
		protected override int GetCaptionHeight(bool hasBottomBorder) {
			return 1;
		}
		public override RectangleF Calculate(RectangleF baseBounds, float bandHeight) {
			fButtonBounds = new RectangleF(baseBounds.X, baseBounds.Y, 0, 0);
			fImageBounds = new RectangleF(baseBounds.X, baseBounds.Y, 0, 0);
			fTextBounds = new RectangleF(baseBounds.X, baseBounds.Y, 0, 0);
			Bounds = Rectangle.Round(new RectangleF(baseBounds.X, baseBounds.Y, baseBounds.Width, CaptionHeight));
			return new RectangleF(baseBounds.X, baseBounds.Y + CaptionHeight, baseBounds.Width, bandHeight);
		}
	}
	public class EmptyCaptionViewInfo : EmptyCaptionViewInfoBase {
		public override bool HasTopBorder { get { return false; } set { } }
		public EmptyCaptionViewInfo(IDesignFrame designFrame)
			: base(designFrame) {
		}
		protected override int GetCaptionHeight(bool hasBottomBorder) {
			return 0;
		}
	}
}
