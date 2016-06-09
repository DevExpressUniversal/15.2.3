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

using DevExpress.LookAndFeel;
using DevExpress.Map.Localization;
using DevExpress.Skins;
using DevExpress.Utils;
using DevExpress.Utils.Drawing;
using DevExpress.XtraMap.Native;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
namespace DevExpress.XtraMap.Drawing {
	public class ErrorPanelViewInfo : OverlayViewInfoBase, ISupportIndexOverlay {
		const int ErrorPanelWidth = 400;
		const int DefaultTextMargin = 32;
		const int MaxErrorPanelIndex = 1;
		public const int CornerRadius = 5;
		Font defaultFont = new Font("Tahoma", 12, FontStyle.Regular);
		StringFormat defaultFormat;
		string text = MapLocalizer.GetString(MapStringId.InvalidBingKeyMessage);
		Rectangle? ownerContentRect = null;
		int errorPanelIndex;
		protected override int KeyInternal { get { return 2; } }
		public Font Font { get { return defaultFont; } }
		public StringFormat Format { get { return defaultFormat; } }
		public override ViewInfoUpdateType SupportedUpdateType { get { return ViewInfoUpdateType.ErrorPanel; } }
		public override bool CanStore { get { return true; } }
		public new ErrorPanelPainter Painter { get { return (ErrorPanelPainter)base.Painter; } }
		protected override MapStyleCollection DefaultAppearance { get { return Painter.ViewInfoAppearanceProvider.DefaultErrorPanelAppearance; } }
		public string Text { get { return text; } }
		public Rectangle TextBounds { get; private set; }
		public Rectangle OwnerContentRect {
			get { return ownerContentRect.HasValue ? ownerContentRect.Value : Map.ContentRectangle; }
		}
		public int ErrorPanelIndex {
			get { return errorPanelIndex; }
			set {
				if(value > MaxErrorPanelIndex || value < 0)
					throw new ArgumentException("ErrorPanelIndex");
				errorPanelIndex = value; 
			}
		}
		public ErrorPanelViewInfo(InnerMap map, ErrorPanelPainter painter)
			: base(map, new ErrorPanelAppearance(null), painter) {
			this.defaultFormat = StringFormat.GenericTypographic;
			this.defaultFormat.Alignment = StringAlignment.Center;
		}
		#region ISupportIndexOverlay implementation
		int ISupportIndexOverlay.Index {
			get { return ErrorPanelIndex; }
			set { ErrorPanelIndex = value; }
		}
		int ISupportIndexOverlay.MaxIndex {
			get { return MaxErrorPanelIndex; }
		}
		#endregion
		public ErrorPanelViewInfo(InnerMap map, Rectangle ownerContentRect, ErrorPanelPainter painter)
			: this(map, painter) {
			this.ownerContentRect = ownerContentRect;
		}
		protected override void DisposeOverride() {
			if(defaultFont != null) {
				defaultFont.Dispose();
				defaultFont = null;
			}
			if(defaultFormat != null) {
				defaultFormat.Dispose();
				defaultFormat = null;
			}
			base.DisposeOverride();
		}
		protected internal override void CalculateOverlay(Graphics gr, Rectangle controlBounds) {
			text = CalculateWrappedText(gr, text, ErrorPanelWidth - DefaultTextMargin * 2);
			Size textSize = MapUtils.CalcStringPixelSize(gr, text, defaultFont);
			ClientBounds = new Rectangle(0, 0, Math.Min(ErrorPanelWidth, OwnerContentRect.Width), Math.Min(textSize.Height + DefaultTextMargin * 2, OwnerContentRect.Height));
			Bounds = RectUtils.AlignRectangle(ClientBounds, OwnerContentRect, ContentAlignment.MiddleCenter);
			TextBounds = new Rectangle(new Point(), textSize);
			TextBounds = RectUtils.AlignRectangle(TextBounds, ClientBounds, ContentAlignment.MiddleCenter);
		}
		string CalculateWrappedText(Graphics gr, string initialString, int availableWidth) {
			string[] words = initialString.Split(new char[] { ' ', '\n', '\r' });
			List<string> resultStrings = new List<string>();
			resultStrings.Add(words.Count() > 0 ? words[0] : "");
			for(int i = 1; i < words.Count(); i++) {
				if(string.IsNullOrEmpty(words[i]))
					continue;
				string checkedString = string.Format("{0} {1}", resultStrings[resultStrings.Count - 1], words[i]);
				int checkedStringWidth = MapUtils.CalcStringPixelSize(gr, checkedString, defaultFont).Width;
				if(checkedStringWidth <= availableWidth)
					resultStrings[resultStrings.Count - 1] = checkedString;
				else
					resultStrings.Add(words[i]);
			}
			string result = resultStrings.Count > 0 ? resultStrings[0] : "";
			for(int i = 1; i < resultStrings.Count; i++)
				result = string.Format("{0}\n{1}", result, resultStrings[i]);
			return result;
		}
	}
	public class ErrorPanelPainter : OverlayViewInfoPainter {
		public ErrorPanelPainter(IViewInfoStyleProvider provider)
			: base(provider) {
		}
		public override void Draw(GraphicsCache cache, SelfpaintingViewinfo viewInfo) {
			ErrorPanelViewInfo vi = (ErrorPanelViewInfo)viewInfo;
			ErrorPanelAppearance paintAppearance = (ErrorPanelAppearance)(viewInfo.PaintAppearance);
			BackgroundStyle bgStyle = paintAppearance.BackgroundStyle;
			TextElementStyle txtStyle = paintAppearance.TextStyle;
			Rectangle clientBounds = vi.ClientBounds;
			int diameter = ErrorPanelViewInfo.CornerRadius * 2;
			Brush rectBrush = cache.GetSolidBrush(bgStyle.Fill);
			int farX = clientBounds.X + clientBounds.Width - diameter;
			int faxY = clientBounds.Y + clientBounds.Height - diameter;
			cache.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
			using(GraphicsPath path = new GraphicsPath()) {
				path.StartFigure();
				path.AddArc(clientBounds.X, clientBounds.Y, diameter, diameter, 180, 90);
				path.AddArc(farX, clientBounds.Y, diameter, diameter, 270, 90);
				path.AddArc(farX, faxY, diameter, diameter, 0, 90);
				path.AddArc(clientBounds.X, faxY, diameter, diameter, 90, 90);
				path.CloseFigure();
				cache.Graphics.FillPath(rectBrush, path);
			}
			cache.Graphics.SmoothingMode = SmoothingMode.HighQuality;
			using(Brush textBrush = new SolidBrush(txtStyle.TextColor)) {
				MapUtils.TextPainter.DrawString(cache, vi.Text, vi.Font, textBrush, vi.TextBounds, vi.Format);
			}
		}
	}
}
