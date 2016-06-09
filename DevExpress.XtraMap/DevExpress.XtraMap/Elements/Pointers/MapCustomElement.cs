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
using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Windows.Forms;
namespace DevExpress.XtraMap {
	public class MapCustomElement : MapPointer {
		Padding padding = CustomElementPainter.DefaultPadding;
		bool allowHtmlText;
		protected internal Point SoureImageLocation { get; set; }
		protected internal override bool AllowHtmlTextCore { get { return AllowHtmlText; } }
#if DEBUGTEST
		protected internal override MapItemType ItemType { get { return MapItemType.Custom; } }
#endif
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
		[
#if !SL
	DevExpressXtraMapLocalizedDescription("MapCustomElementAllowHtmlText"),
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
		[
#if !SL
	DevExpressXtraMapLocalizedDescription("MapCustomElementRenderOrigin"),
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
	DevExpressXtraMapLocalizedDescription("MapCustomElementTextGlowColor"),
#endif
		Category(SRCategoryNames.Appearance)]
		public new Color TextGlowColor {
			get { return base.TextGlowColor; }
			set { base.TextGlowColor = value; }
		}
		bool ShouldSerializeTextGlowColor() { return !IsStyleEmpty && TextGlowColor != Color.Empty; }
		void ResetTextGlowColor() { if(!IsStyleEmpty) TextGlowColor = Color.Empty; }
		[
#if !SL
	DevExpressXtraMapLocalizedDescription("MapCustomElementPadding"),
#endif
		Category(SRCategoryNames.Appearance)]
		public Padding Padding {
			get { return padding; }
			set {
				if (padding == value)
					return;
				padding.Left = Math.Max(0, value.Left);
				padding.Right = Math.Max(0, value.Right);
				padding.Top = Math.Max(0, value.Top);
				padding.Bottom = Math.Max(0, value.Bottom);
				PropertyChanged();
			}
		}
		void ResetPadding() { Padding = CustomElementPainter.DefaultPadding; }
		bool ShouldSerializePadding() { return Padding != CustomElementPainter.DefaultPadding; }
		void PropertyChanged() {
			RegisterLayoutUpdate();
			InvalidateRender();
		}
		protected override ElementState DefaultSkinBackgroundState { get { return ElementState.Highlighted | ElementState.Selected; } }
		protected internal Padding ActualPadding { get { return Padding == CustomElementPainter.DefaultPadding ? DefaultStyleProvider.CustomElementStyle.ContentPadding : Padding; } }
		protected override void UpdateImageOrigin() {
			if(Geometry != null) 
				RecalculateImageOrigin(ImageSafeAccess.GetSize(GetActualImage()), Geometry.ImageRect.Size.ToSize(), SoureImageLocation);
		}
		protected MapPoint CalculateImageOrigin(Size imageRect, Size boundsSize, Point imageLeftTop) {
			if(imageRect.IsEmpty || boundsSize.IsEmpty) {
				return RenderOrigin;
			}
			double dX = imageRect.Width * RenderOrigin.X / boundsSize.Width;
			double dY = imageRect.Height * RenderOrigin.Y / boundsSize.Height;
			dX += imageLeftTop.X / (double)boundsSize.Width;
			dY += imageLeftTop.Y / (double)boundsSize.Height;
			return new MapPoint(dX, dY);
		}
		protected internal void RecalculateImageOrigin(Size imageSize, Size boundsSize, Point imageLocation) {
			ImageOrigin = CalculateImageOrigin(imageSize, boundsSize, imageLocation);
		}
		protected override TextImageItemPainterBase CreatePainter() {
			return new CustomElementPainter(this);
		}
		protected override IClusterItem CreateInstance() {
			return new MapCustomElement();
		}
		protected internal override MapElementStyleBase GetDefaultItemStyle() {
			return DefaultStyleProvider.CustomElementStyle;
		}
		protected internal override MapElementStyleBase GetDefaultSelectedItemStyle() {
			return DefaultStyleProvider.SelectedCustomElementStyle;
		}
		protected internal override MapElementStyleBase GetDefaultHighlightedItemStyle() {
			return DefaultStyleProvider.HighlightedCustomElementStyle;
		}
		public override string ToString() {
			return "(MapCustomElement)";
		}
	}
}
namespace DevExpress.XtraMap.Native {
	public interface IMapItemImageTransform {
		MapPoint CalcImageOrigin(Image image);
	}
}
