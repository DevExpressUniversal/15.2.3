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
using DevExpress.XtraMap.Drawing;
using DevExpress.XtraMap.Native;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
namespace DevExpress.XtraMap {
	public class MapPushpin : MapPointer {
		object information;
		Point textOrigin = Point.Empty;
		#region Style properties
		[SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "value"), 
		Browsable(false), 
		EditorBrowsable(EditorBrowsableState.Never)]
		public new Color Fill { get { return Color.Empty; } set { ; } }
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
		public new Color HighlightedFill { get { return Color.Empty; } set { ; } }
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
		public new Color SelectedFill { get { return Color.Empty; } set { ; } }
		[SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "value"), 
		Browsable(false), 
		EditorBrowsable(EditorBrowsableState.Never)]
		public new Color SelectedStroke { get { return Color.Empty; } set { ; } }
		[SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "value"),
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never)]
		public new int SelectedStrokeWidth { get { return MapItemStyle.EmptyStrokeWidth; } set { ; } }
		#endregion
#if DEBUGTEST
		protected internal override MapItemType ItemType { get { return MapItemType.Pushpin; } }
#endif
		[SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "value"), 
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never)]
		public new TextAlignment TextAlignment { get { return DefaultTextAlignment; } set { ; } }
		[SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "value"), 
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never)]
		public new int TextPadding { get { return MapPointer.DefaultTextPadding; } set { ; } }
		[
#if !SL
	DevExpressXtraMapLocalizedDescription("MapPushpinRenderOrigin"),
#endif
		Category(SRCategoryNames.Appearance)]
		new public MapPoint RenderOrigin {
			get { return base.RenderOrigin; }
			set { base.RenderOrigin = value; }
		}
		bool ShouldSerializeRenderOrigin() { return RenderOrigin != TextImageItemPainterBase.DefaultRenderOrigin; }
		void ResetRenderOrigin() { RenderOrigin = TextImageItemPainterBase.DefaultRenderOrigin; }
		[
#if !SL
	DevExpressXtraMapLocalizedDescription("MapPushpinTextOrigin"),
#endif
		Category(SRCategoryNames.Appearance)]
		public Point TextOrigin {
			get { return textOrigin; }
			set {
				if (textOrigin == value)
					return;
				textOrigin = value;
				if (!HasDefaultImage)
					UpdateItem(MapItemUpdateType.Layout);
			}
		}
		bool ShouldSerializeTextOrigin() { return TextOrigin != Point.Empty; }
		void ResetTextOrigin() { TextOrigin = Point.Empty; }
		[
#if !SL
	DevExpressXtraMapLocalizedDescription("MapPushpinTextGlowColor"),
#endif
		Category(SRCategoryNames.Appearance)]
		public new Color TextGlowColor {
			get { return base.TextGlowColor; }
			set { base.TextGlowColor = value; }
		}
		bool ShouldSerializeTextGlowColor() { return !IsStyleEmpty && TextGlowColor != Color.Empty; }
		void ResetTextGlowColor() { if(!IsStyleEmpty) TextGlowColor = Color.Empty; }
		protected bool HasDefaultImage {
			get {
				return LoadedImage == null && Image == null && (ActualImageIndex == DefaultImageIndex || ActualImageList == null);
			}
		}
		protected internal Point ActualTextOrigin {
			get {
				return HasDefaultImage ? DefaultStyleProvider.PushpinStyle.TextOrigin : TextOrigin;
			}
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public object Information { get { return information; } set { information = value; } }
		public override string ToString() {
			return "(MapPushpin)";
		}
		protected override void OnLocationChanged() {
			ReleaseHitTestGeometry();
			UpdateBounds();
			UpdateBoundingRect();
			InvalidateRender();
		}
		protected override IMapItemGeometry CreateGeometry() {
			return new ImageGeometry() { AlignImage = true };
		}
		protected override Image GetDefaultImage() {
			if(IsSelected) return DefaultStyleProvider.PushpinStyle.SelectedImage;
			if(IsHighlighted) return DefaultStyleProvider.PushpinStyle.HighlightedImage;
			return DefaultStyleProvider.PushpinStyle.Image;
		}
		protected override IClusterItem CreateInstance() {
			return new MapPushpin();
		}
		protected override TextImageItemPainterBase CreatePainter() {
			return new PushpinPainter(this);
		}
		protected internal override MapElementStyleBase GetDefaultItemStyle() {
			return DefaultStyleProvider.PushpinStyle;
		}
		protected internal override MapElementStyleBase GetDefaultSelectedItemStyle() {
			return GetDefaultItemStyle();
		}
		protected internal override MapElementStyleBase GetDefaultHighlightedItemStyle() {
			return GetDefaultItemStyle();
		}
		protected override void UpdateImageOrigin() {
			RecalculateImageOrigin();
		}
		protected internal void RecalculateImageOrigin() {
			ImageOrigin = HasDefaultImage ? DefaultStyleProvider.PushpinStyle.ImageOrigin : RenderOrigin;
		}
	}
}
