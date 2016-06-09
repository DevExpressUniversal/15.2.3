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

using DevExpress.Map;
using DevExpress.Map.Native;
using DevExpress.XtraMap.Drawing;
using DevExpress.XtraMap.Native;
using System;
using System.ComponentModel;
using System.Drawing;
namespace DevExpress.XtraMap {
	public abstract class MapShape : MapItem, IRenderItemStyle, IColorizerElement, ISupportStyleCore, IMapShapeStyleCore {
		ShapeTitle title;
		readonly ShapeTitleOptions titleOptions;
		ShapeTitleOptions ActualTitleOptions {
			get { return MergeTitleOptions(); }
		}
		protected internal override GeometryType GeometryType { get { return GeometryType.Unit; } }
		protected internal ShapeTitle Title { get { return title; } }
		protected new UnitGeometry Geometry { get { return base.Geometry as UnitGeometry; } }
		protected internal virtual MapRect BoundsForTitle { get { return Bounds; } }
		[
#if !SL
	DevExpressXtraMapLocalizedDescription("MapShapeTitleOptions"),
#endif
		Category(SRCategoryNames.Options), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public ShapeTitleOptions TitleOptions { get { return titleOptions; } }
		protected MapShape() {
			this.titleOptions = new ShapeTitleOptions();
			SubscribeTitleOptionsChangedEvent();
		}
		void SubscribeTitleOptionsChangedEvent() {
			TitleOptions.Changed += OnTitleOptionsChanged;
		}
		void OnTitleOptionsChanged(object sender, Utils.Controls.BaseOptionChangedEventArgs e) {
			ApplyTitleOptions();
		}
		VisibilityMode CalculateMergedVisibility(VisibilityMode visibility1, VisibilityMode visibility2) {
			if(visibility1 == VisibilityMode.Hidden || visibility2 == VisibilityMode.Hidden)
				return VisibilityMode.Hidden;
			if(visibility1 == VisibilityMode.Visible || visibility2 == VisibilityMode.Visible)
				return VisibilityMode.Visible;
			return VisibilityMode.Auto;
		}
		void CustomizeTitle(string title, Color textColor, Color textGlowColor) {
			EnsureStyle();
			ActualStyle.TextColor = textColor;
			ActualStyle.TextGlowColor = textGlowColor;
			Title.CustomizeGeometry(title);
		}
		ShapeTitleOptions MergeTitleOptions() {
			if(Layer == null) return TitleOptions;
			ShapeTitleOptions mergedOptions = new ShapeTitleOptions();
			mergedOptions.Visibility = CalculateMergedVisibility(Layer.ShapeTitlesVisibility, TitleOptions.Visibility);
			mergedOptions.Pattern = String.IsNullOrEmpty(TitleOptions.Pattern) ? Layer.ShapeTitlesPattern : TitleOptions.Pattern;
			return mergedOptions;
		}
		internal void CreateTitleIfNecessary(bool nonEmptyContent) {
			if(Title == null && nonEmptyContent)
				this.title = new ShapeTitle(this);
		}
		#region IRenderItemStyle Members
		Color IRenderItemStyle.Fill { get { return ActualStyle.Fill; } }
		Color IRenderItemStyle.Stroke { get { return ActualStyle.Stroke; } }
		int IRenderItemStyle.StrokeWidth { get { return ActualStyle.StrokeWidth; } }
		#endregion
		#region IColorizerElement Members
		Color IColorizerElement.ColorizerColor { get { return ColorizerColor; } set { ColorizerColor = value; } } 
		#endregion
		#region ISupportStyleCore
		void ISupportStyleCore.SetStrokeWidth(double width) {
			int _width = Convert.ToInt32(width);
			StrokeWidth = _width;
			HighlightedStrokeWidth = _width;
			SelectedStrokeWidth = _width;
		}
		void ISupportStyleCore.SetFill(System.Drawing.Color color) {
			Fill = color;
		}
		void ISupportStyleCore.SetStroke(System.Drawing.Color color) {
			Stroke = color;
		}
		#endregion
		#region IMapShapeStyleCore implementation
		Color IMapShapeStyleCore.Fill {
			get { return GetFill(); }
			set { Fill = value; } 
		}
		Color IMapShapeStyleCore.Stroke { 
			get { return ActualStyle.Stroke; }
			set { Stroke = value; }
		}
		double IMapShapeStyleCore.StrokeWidth { 
			get { return ActualStyle.StrokeWidth; }
			set {
				int resultValue = (int)value;
				if (value < 1.0 && value > 0.1)
					resultValue = 1;
				else if (value < 0)
					resultValue = 0;
				StrokeWidth = resultValue; 
			}
		}
		#endregion
		protected virtual Color GetFill() {
			return ActualStyle.Fill;
		}
		protected internal override void ResetStyle() {
			base.ResetStyle();
		}
		protected internal override void ResetColorizerColor() {
			base.ResetColorizerColor();
			ColorizerColor = Color.Empty;
		}
		protected override void SetOwner(object value) {
			base.SetOwner(value);
			if(value == null && title != null) {
				IOwnedElement ownedEl = (IOwnedElement)Title;
				ownedEl.Owner = null;
			} else
				ApplyTitleOptions();
		}
		protected override string GetTextCore() {
			return title != null ? title.Text : base.GetTextCore(); 
		}
		protected override Color GetTextColorCore() {
			return GetTitleColor();
		}
		protected internal virtual Color GetTitleColor() {
			return ActualStyle.TextColor;
		}
		protected internal virtual Color GetTitleGlowColor() {
			return ActualStyle.TextGlowColor;
		}
		protected override IMapItemGeometry CreateGeometry() {
			if(Title != null)
				Title.RecreateGeometry();
			return CreateShapeGeometry();
		}
		protected override void PrepareImageGeometry() {
			if(Title != null)
				Title.PrepareGeometry();
		}
		protected abstract IMapItemGeometry CreateShapeGeometry();
		protected internal override void RegisterUpdate(MapItemUpdateType updateType) {
			RegisterUpdate(updateType, true);
		}
		internal void RegisterUpdate(MapItemUpdateType updateType, bool needUpdateTitle) {
			base.RegisterUpdate(updateType);
			if(Title != null && needUpdateTitle)
				Title.ResetLayout();
		}
		protected override IHitTestGeometry CreateHitTestGeometry() {
			return Geometry != null ? PathHitTestGeometry.CreateFromUnitGeometry(Geometry) : null;
		}
		protected internal virtual bool? CalculateDefaultIsInShapeBounds() {
			return null;
		}
		protected override IRenderItemStyle GetStyle() {
			return this;
		}
		protected override IRenderShapeTitle GetTitle() {
			return Title;
		}
		protected override void RegisterHitTestableItem() {
			base.RegisterHitTestableItem();
			ReleaseHitTestGeometry();
		}
		protected internal override void ApplyTitleOptions() {
			ShapeTitleOptions options = ActualTitleOptions;
			string[] template = options.Pattern.Split('{', '}');
			string result = string.Empty;
			for(int i = 0; i < template.Length; i++) {
				if(i % 2 == 0)
					result += template[i];
				else {
					MapItemAttribute attribute = Attributes[template[i]];
					if(attribute != null)
						result += attribute.Value.ToString();
				}
			}
			UpdateShapeTitle(result, options.Visibility);
		}
		void UpdateShapeTitle(string text, VisibilityMode visibility) {
			CreateTitleIfNecessary(!string.IsNullOrEmpty(text) && visibility != VisibilityMode.Hidden);
			if(Title == null)
				return;
			Title.Text = text;
			Title.VisibilityMode = visibility;
		}
		bool NeedCustomizeTitle(IShapeRenderItemStyle style) {
			return style != null && Title != null && (!string.Equals(style.ActualTitle, Title.Text) || style.TitleColor != GetTitleColor() || style.TitleGlowColor != GetTitleGlowColor());
		}
		protected internal override DrawMapItemEventArgs CreateDrawEventArgs() {
			return new DrawMapShapeEventArgs(this) { TitleColor = GetTitleColor(), TitleGlowColor = GetTitleGlowColor(), ActualTitle = Title != null ? Title.Text : string.Empty };
		}
		protected override void AfterDrawMapItemEvent(IRenderItemStyle style) {
			IShapeRenderItemStyle shapeStyle = style as IShapeRenderItemStyle;
			CreateTitleIfNecessary(!string.IsNullOrEmpty(shapeStyle.ActualTitle));
			if(NeedCustomizeTitle(shapeStyle))
				CustomizeTitle(shapeStyle.ActualTitle, shapeStyle.TitleColor, shapeStyle.TitleGlowColor);
		}
		protected MapRect CalculateLocatableItemBounds(MapPoint location, double halfSize) {
			MapUnit unit1 = UnitConverter.ScreenPointToMapUnit(new MapPoint(location.X - halfSize, location.Y - halfSize), Layer.Map == null);
			MapUnit unit2 = UnitConverter.ScreenPointToMapUnit(new MapPoint(location.X + halfSize, location.Y + halfSize), Layer.Map == null);
			return new MapRect(unit1.X, unit1.Y, unit2.X - unit1.X, unit2.Y - unit1.Y);
		}
		protected internal virtual CoordPoint GetCenterCore() {
			return UnitConverter.MapUnitToCoordPoint(RectUtils.GetCenter(Bounds));
		}
		public CoordPoint GetCenter() {
			return GetCenterCore();
		}
	}
}
