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

using DevExpress.Map.Native;
using DevExpress.Skins;
using DevExpress.XtraMap.Drawing;
using DevExpress.XtraMap.Native;
using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
namespace DevExpress.XtraMap {
	public class MapCallout : MapPointer, IColorizerElement {
		bool allowHtmlText;
		protected internal override bool AllowHtmlTextCore { get { return AllowHtmlText; } }
		#region Style properties
		[SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "value"),
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never)]
		public new Color Stroke { get { return Color.Empty; } set { ; } }
		[SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "value"),
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never)]
		public new int StrokeWidth { get { return MapItemStyle.EmptyStrokeWidth; } set { ; } }
		[SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "value"),
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never)]
		public new Color HighlightedStroke { get { return Color.Empty; } set { ; } }
		[SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "value"),
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never)]
		public new int HighlightedStrokeWidth { get { return MapItemStyle.EmptyStrokeWidth; } set { ; } }
		[SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "value"),
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never)]
		public new Color SelectedStroke { get { return Color.Empty; } set { ; } }
		[SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "value"),
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never)]
		public new int SelectedStrokeWidth { get { return MapItemStyle.EmptyStrokeWidth; } set { ; } }
		#endregion
		[Category(SRCategoryNames.Appearance)]
		public Color HighlightedTextColor {
			get { return IsHighlightedStyleEmpty ? Color.Empty : HighlightedStyle.TextColor; }
			set {
				EnsureHighlightedStyle();
				HighlightedStyle.TextColor = value;
			}
		}
		void ResetHighlightedTextColor() { if (!IsHighlightedStyleEmpty) HighlightedTextColor = Color.Empty; }
		protected bool ShouldSerializeHighlightedTextColor() { return !IsHighlightedStyleEmpty && HighlightedTextColor != Color.Empty; }
		[Category(SRCategoryNames.Appearance)]
		public Color SelectedTextColor {
			get { return IsSelectedStyleEmpty ? Color.Empty : SelectedStyle.TextColor; }
			set {
				EnsureSelectedStyle();
				SelectedStyle.TextColor = value;
			}
		}
		void ResetSelectedTextColor() { if (!IsSelectedStyleEmpty) SelectedTextColor = Color.Empty; }
		protected bool ShouldSerializeSelectedTextColor() { return !IsSelectedStyleEmpty && SelectedTextColor != Color.Empty; }
		[
#if !SL
	DevExpressXtraMapLocalizedDescription("MapCalloutAllowHtmlText"),
#endif
		DefaultValue(false), Category(SRCategoryNames.Appearance)]
		public bool AllowHtmlText {
			get {
				return allowHtmlText;
			}
			set {
				if (allowHtmlText == value)
					return;
				this.allowHtmlText = value;
				OnAllowHtmlTextChanged();
			}
		}
		void OnAllowHtmlTextChanged() {
			UpdateItem(MapItemUpdateType.Layout);
		}
#if DEBUGTEST
		protected internal override MapItemType ItemType { get { return MapItemType.Callout; } }
#endif
		#region IColorizerElement Members
		Color IColorizerElement.ColorizerColor { get { return ColorizerColor; } set { ColorizerColor = value; } }
		#endregion
		protected override TextImageItemPainterBase CreatePainter() {
			return new CalloutPainter(this);
		}
		protected internal override MapElementStyleBase GetDefaultItemStyle() {
			return DefaultStyleProvider.CalloutStyle;
		}
		protected override IHitTestGeometry CreateHitTestGeometry() {
			MapPoint pixelLocation = UnitConverter.CoordPointToScreenPoint(CurrentLocation);
			SkinPaddingEdges effectiveArea = DefaultStyleProvider.CalloutStyle.HighlightedEffectiveArea;
			Size size = ImageSize;
			MapPoint origin = ImageOrigin;
			pixelLocation = new MapPoint(pixelLocation.X - size.Width * origin.X, pixelLocation.Y - size.Height * origin.Y);
			pixelLocation.X = pixelLocation.X + effectiveArea.Left;
			pixelLocation.Y = pixelLocation.Y + effectiveArea.Top;
			size = new Size(Math.Max(0, size.Width - effectiveArea.Width), Math.Max(0, size.Height - effectiveArea.Height));
			return new RectangleScreenHitTestGeometry(pixelLocation, size, MapPoint.Empty);
		}
		protected internal override MapElementStyleBase GetDefaultSelectedItemStyle() {
			return DefaultStyleProvider.SelectedCalloutStyle;
		}
		protected internal override MapElementStyleBase GetDefaultHighlightedItemStyle() {
			return DefaultStyleProvider.HighlightedCalloutStyle;
		}
		protected internal void RecalculateImageOrigin(Size size, Point point) {
			double x = size.Width > 0 && point.X > 0 ? (double)point.X / size.Width : 0.0;
			double y = size.Height > 0 && point.Y > 0 ? 1.0 - (double)point.Y / size.Height : 1.0;
			ImageOrigin = new MapPoint(x, y);
		}
		protected override Point GetLeftTopPointOffset() {
			return DefaultStyleProvider.CalloutStyle.BaseOffset;
		}
		protected override IClusterItem CreateInstance() {
			return new MapCallout();
		}
		public override string ToString() {
			return "(MapCallout)";
		}
	}
}
