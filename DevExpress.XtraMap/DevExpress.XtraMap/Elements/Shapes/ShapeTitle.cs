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

using DevExpress.Utils;
using DevExpress.XtraMap.Drawing;
using DevExpress.XtraMap.Native;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Linq;
using System.Text;
using System.ComponentModel;
using DevExpress.Map.Native;
using DevExpress.Map;
namespace DevExpress.XtraMap {
	public class ShapeTitleOptions : MapNotificationOptions {
		const string DefaultPattern = "";
		const VisibilityMode DefaultVisibility = VisibilityMode.Auto;
		string pattern;
		Color textColor = Color.Empty;
		Color textGlowColor = Color.Empty;
		VisibilityMode visibitility = DefaultVisibility;
		[
#if !SL
	DevExpressXtraMapLocalizedDescription("ShapeTitleOptionsPattern"),
#endif
		Category(SRCategoryNames.Appearance), DefaultValue(DefaultPattern)]
		public string Pattern {
			get { return pattern; }
			set {
				if (pattern.Equals(value))
					return;
				string oldValue = pattern;
				pattern = value;
				OnChanged("Pattern", oldValue, pattern);
			}
		}
		[
#if !SL
	DevExpressXtraMapLocalizedDescription("ShapeTitleOptionsTextColor"),
#endif
		Category(SRCategoryNames.Appearance)]
		public Color TextColor {
			get { return textColor; }
			set {
				if (textColor == value)
					return;
				Color oldValue = textColor;
				textColor = value;
				OnChanged("TextColor", oldValue, textColor);
			}
		}
		void ResetTextColor() { TextColor = Color.Empty; }
		bool ShouldSerializeTextColor() { return TextColor != Color.Empty; }
		[
#if !SL
	DevExpressXtraMapLocalizedDescription("ShapeTitleOptionsTextGlowColor"),
#endif
		Category(SRCategoryNames.Appearance)]
		public Color TextGlowColor {
			get { return textGlowColor; }
			set {
				if (textGlowColor == value)
					return;
				Color oldValue = textGlowColor;
				textGlowColor = value;
				OnChanged("TextGlowColor", oldValue, textGlowColor);
			}
		}
		void ResetTextGlowColor() { TextGlowColor = Color.Empty; }
		bool ShouldSerializeTextGlowColor() { return TextGlowColor != Color.Empty; }
		[
#if !SL
	DevExpressXtraMapLocalizedDescription("ShapeTitleOptionsVisibility"),
#endif
		Category(SRCategoryNames.Behavior), DefaultValue(DefaultVisibility)]
		public VisibilityMode Visibility {
			get { return visibitility; }
			set {
				if (visibitility == value)
					return;
				VisibilityMode oldValue = visibitility;
				visibitility = value;
				OnChanged("Visibility", oldValue, visibitility);
			}
		}
		protected internal override void ResetCore() {
			pattern = DefaultPattern;
			textColor = Color.Empty;
			textGlowColor = Color.Empty;
			visibitility = DefaultVisibility;
		}
	}
}
namespace DevExpress.XtraMap.Native {
	public class ShapeTitle : ILocatableRenderItem, IOwnedElement, ISupportImagePainter, IRenderShapeTitle {
		MapShape shape;
		bool allowUseAntiAliasing;
		string text;
		Size imageSize = Size.Empty;
		bool? defaultIsInShapeBounds;
		MapUnit? location = null;
		VisibilityMode visibilityMode = VisibilityMode.Auto;
		ImageGeometry geometry;
		protected MapUnitConverter UnitConverter { get { return Shape.UnitConverter; } }
		protected internal ImageGeometry Geometry { get { return geometry; } }
		protected Font Font { get { return Shape.ActualStyle.Font; } }
		public string Text {
			get { return text; }
			set {
				text = value;
				UpdateItem(MapItemUpdateType.Layout);
			}
		}
		public MapShape Shape { get { return shape; } }
		protected internal bool ActualVisible { get { return Shape.ActualVisible && CalculateActualVisibility(); } }
		protected internal VisibilityMode VisibilityMode { get { return visibilityMode; } set { visibilityMode = value; } }
		#region IRenderItem members
		bool IRenderShapeTitle.Visible { get { return ActualVisible; } }
		bool IRenderShapeTitle.UseAntiAliasing { get { return allowUseAntiAliasing; } }
		IImageGeometry IRenderShapeTitle.Geometry { get { return geometry; } }
		#endregion
		#region ILocatableRenderItem Members
		MapUnit ILocatableRenderItem.Location {
			get {
				if(!location.HasValue)
					location = UnitConverter.CoordPointToMapUnit(Shape.GetCenterCore(), true);
				return location.Value * MapShape.RenderScaleFactor;
			}
		}
		Size ILocatableRenderItem.SizeInPixels {
			get { return this.imageSize; }
		}
		MapPoint ILocatableRenderItem.Origin {
			get { return TextImageItemPainterBase.DefaultRenderOrigin; }
		}
		MapPoint ILocatableRenderItem.StretchFactor {
			get { return new MapPoint(1.0, 1.0); }
		}
		void ILocatableRenderItem.ResetLocation() { this.location = null; }
		#endregion
		#region IOwnedElement members
		object IOwnedElement.Owner {
			get { return Shape; }
			set {
				MapShape shape = value as MapShape;
				this.shape = shape;
			}
		}
		#endregion
		#region ISupportImagePainter members
		MapItemStyle ISupportImagePainter.ActualStyle { get { return Shape.ActualStyle; } }
		ImageGeometry ISupportImagePainter.Geometry { get { return Geometry; } }
		#endregion
		public ShapeTitle(MapShape shape) {
			Guard.ArgumentNotNull(shape, "shape");
			this.shape = shape;
			this.defaultIsInShapeBounds = Shape.CalculateDefaultIsInShapeBounds();
		}
		bool CalculateActualVisibility() {
			switch(VisibilityMode) {
				case VisibilityMode.Auto: return IsInsideShapeBounds();
				case VisibilityMode.Hidden: return false;
				case VisibilityMode.Visible: return true;
			}
			throw new InvalidEnumArgumentException();
		}
		bool IsInsideShapeBounds() {
			if(defaultIsInShapeBounds.HasValue)
				return defaultIsInShapeBounds.Value;
			if(imageSize.IsEmpty)
				return false;
			MapRect shapeBound = Shape.BoundsForTitle;
			MapPoint p1 = UnitConverter.MapUnitToScreenPoint(new MapUnit(shapeBound.Left, shapeBound.Top), true);
			MapPoint p2 = UnitConverter.MapUnitToScreenPoint(new MapUnit(shapeBound.Right, shapeBound.Bottom), true);
			return (imageSize.Width <= Math.Abs(p1.X - p2.X)) && (imageSize.Height <= Math.Abs(p1.Y - p2.Y));
		}
		void UpdateItem(MapItemUpdateType mapItemUpdateType) {
			Shape.UpdateItem(mapItemUpdateType);
		}
		internal void UpdateGeometry(MapItemUpdateType mapItemUpdateType) {
			Shape.RegisterUpdate(mapItemUpdateType, false);
		}
		internal void ResetLayout() {
			UpdateGeometry(MapItemUpdateType.Layout);
			location = null;
		}
		TextImageItemPainterBase CreatePainter() {
			return new ShapeTitlePainter(this);
		}
		internal void CustomizeGeometry(string text) {
			if(Geometry == null)
				RecreateGeometry();
			PrepareImageGeometry(Geometry, null, text, ImageGeometry.DefaultTransparency, false);
		}
		internal void PrepareGeometry() {
			PrepareImageGeometry(Geometry, null, Text, ImageGeometry.DefaultTransparency, true);
		}
		void PrepareImageGeometry(ImageGeometry geometry, Image image, string text, byte transparency, bool storingInPool) {
			TextImageItemPainterBase painter = CreatePainter();
			painter.SourceText = text;
			painter.Draw(null);
			this.allowUseAntiAliasing = painter.AllowUseAntiAliasing;
			geometry.SetImage(painter.FinalImage, painter.IsExternalImage);
			geometry.ImageRect = painter.FinalImageRect;
			geometry.StoringInPool = storingInPool;
			geometry.Transparency = transparency;
			this.imageSize = painter.FinalImageRect.Size;
		}
		internal void RecreateGeometry() {
			this.geometry = new ImageGeometry();
		}
	}
}
