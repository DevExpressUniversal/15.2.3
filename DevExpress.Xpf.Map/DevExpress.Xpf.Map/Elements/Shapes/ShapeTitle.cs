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
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Map.Native;
using DevExpress.Xpf.Utils;
using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
namespace DevExpress.Xpf.Map {
	public enum VisibilityMode {
		Visible,
		Hidden,
		Auto
	}
	public class ShapeTitleOptions : MapDependencyObject {
		const string templateSource = "<Grid> <TextBlock Text=\"{Binding Path=Text}\" HorizontalAlignment=\"Center\" VerticalAlignment=\"Center\"/> </Grid>";
		public static readonly DependencyProperty PatternProperty = DependencyPropertyManager.Register("Pattern",
			typeof(string), typeof(ShapeTitleOptions), new PropertyMetadata("{NAME}", NotifyPropertyChanged));
		public static readonly DependencyProperty TemplateProperty = DependencyPropertyManager.Register("Template",
			typeof(DataTemplate), typeof(ShapeTitleOptions), new PropertyMetadata(GetDefaultTemplate(), NotifyPropertyChanged));
		[
		Obsolete(ObsoleteMessages.ShapeTitleOptions_Visible),
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public static readonly DependencyProperty VisibleProperty = DependencyPropertyManager.Register("Visible",
			typeof(bool), typeof(ShapeTitleOptions), new PropertyMetadata(false, VisibleChanged));
		public static readonly DependencyProperty VisibilityModeProperty = DependencyPropertyManager.Register("VisibilityMode",
			typeof(VisibilityMode), typeof(ShapeTitleOptions), new PropertyMetadata(VisibilityMode.Auto, VisibilityModeChanged));
		bool visibilityEverSet = false;
		[Category(Categories.Presentation)]
		public string Pattern {
			get { return (string)GetValue(PatternProperty); }
			set { SetValue(PatternProperty, value); }
		}
		[Category(Categories.Presentation)]
		public DataTemplate Template {
			get { return (DataTemplate)GetValue(TemplateProperty); }
			set { SetValue(TemplateProperty, value); }
		}
		[
		Obsolete(ObsoleteMessages.ShapeTitleOptions_Visible), 
		Browsable(false), 
		EditorBrowsable(EditorBrowsableState.Never), 
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool Visible {
			get { return (bool)GetValue(VisibleProperty); }
			set { SetValue(VisibleProperty, value); }
		}
		[Category(Categories.Behavior)]
		public VisibilityMode VisibilityMode {
			get { return (VisibilityMode)GetValue(VisibilityModeProperty); }
			set { SetValue(VisibilityModeProperty, value); }
		}
		static DataTemplate GetDefaultTemplate() {
			return XamlHelper.GetTemplate(templateSource);
		}
		static void VisibleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			ShapeTitleOptions options = d as ShapeTitleOptions;
			if(options != null && !options.visibilityEverSet)
				options.VisibilityMode = (bool)e.NewValue ? VisibilityMode.Visible : VisibilityMode.Hidden;
		}
		static void VisibilityModeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			ShapeTitleOptions options = d as ShapeTitleOptions;
			if(options != null) {
				options.visibilityEverSet = true;
				NotifyPropertyChanged(d, e);
			}
		}
		protected override MapDependencyObject CreateObject() {
			return new ShapeTitleOptions();
		}
	}
	[NonCategorized]
	public class ShapeTitle : MapItem {
		static ControlTemplate templateTitle;
		VisibilityMode visibility;
		static ShapeTitle() {
			XamlHelper.SetLocalNamespace(CommonUtils.localNamespace);
			templateTitle = XamlHelper.GetControlTemplate("<ContentPresenter ContentTemplate=\"{Binding Template}\" Content=\"{Binding MapItem}\"/>");
		}
		public static readonly DependencyProperty TextProperty = DependencyPropertyManager.Register("Text",
			typeof(string), typeof(ShapeTitle), new PropertyMetadata(string.Empty));
		public static readonly DependencyProperty TemplateProperty = DependencyPropertyManager.Register("Template",
			typeof(DataTemplate), typeof(ShapeTitle), new PropertyMetadata(null, TemplatePropertyChanged));
		static void TemplatePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			ShapeTitle title = d as ShapeTitle;
			if (title != null)
				title.OnTemplateChanged((DataTemplate)e.OldValue, (DataTemplate)e.NewValue);
		}
		protected internal override ControlTemplate ItemTemplate {
			get {
				return templateTitle;
			}
		}
		[NonTestableProperty]
		public string Text {
			get { return (string)GetValue(TextProperty); }
			set { SetValue(TextProperty, value); }
		}
		[NonTestableProperty]
		public DataTemplate Template {
			get { return (DataTemplate)GetValue(TemplateProperty); }
			set { SetValue(TemplateProperty, value); }
		}
		public MapShapeBase MapShape {
			get { return mapShape; }
		}
		readonly MapShapeBase mapShape;
		protected override bool IsVisualElement { get { return false; } }
		protected internal override MapItemAttributeCollection ActualAttributes { get { return mapShape != null ? mapShape.Attributes : Attributes; } }
		internal ShapeTitle(MapShapeBase mapShape) {
			this.mapShape = mapShape;
		}
		void OnTemplateChanged(DataTemplate oldValue, DataTemplate newValue) {
		}
		internal void ApplyOptions(string text, DataTemplate dataTemplate, VisibilityMode visibilityMode) {
			this.Text = text;
			this.Template = dataTemplate;
			this.visibility = visibilityMode;
		}
		protected override void CalculateLayout() {
			Layout.SizeInPixels = new Size(double.PositiveInfinity, double.PositiveInfinity);
		}
		protected override void CompleteLayout(UIElement element) {
			if (mapShape != null) {
				Rect titleRect = mapShape.CalculateTitleLayout(element.DesiredSize, Layer);
				this.Visible = Layout.Visible = CalculateActualVisibility(titleRect.Size, mapShape.GetAvailableSizeForTitle());
				Layout.LocationInPixels = new Point(titleRect.Left, titleRect.Top);
				Layout.SizeInPixels = Visible ? new Size(titleRect.Width, titleRect.Height) : new Size();
			}
		}
		bool CalculateActualVisibility(Size titleSize, Size shapeSize) {
			switch(visibility) {
				case VisibilityMode.Visible: return true;
				case VisibilityMode.Hidden: return false;
				case VisibilityMode.Auto: return IsInShapeBounds(titleSize, shapeSize);
			}
			throw new InvalidEnumArgumentException();
		}
		bool IsInShapeBounds(Size titleSize, Size shapeSize) {
			return titleSize.Height <= shapeSize.Height && titleSize.Width <= shapeSize.Width;
		}
		protected override MapDependencyObject CreateObject() {
			return new ShapeTitle(null);
		}
		protected override void CalculateLayoutInMapUnits() {
		}
		protected override void UpdateVisibility() {
			base.UpdateVisibility();
			UpdateLayout();
		}
		protected internal void SetIsSelectedInternal(bool value) {
			base.SetIsSelected(value);
		}
		protected internal override CoordBounds CalculateBounds() {
			return CoordBounds.Empty;
		}
		protected internal override void SetIsSelected(bool value) {
			if (value && mapShape != null)
				mapShape.SetIsSelected(value);
		}
		protected internal override void SetIsHighlighted(bool value) {
			if (value && mapShape != null)
				mapShape.SetIsHighlighted(value);
		}
		protected internal void SetIsHighlightedInternal(bool value) {
			base.SetIsHighlighted(value);
		}
		protected internal override void OnMouseEnter(object sender, MouseEventArgs e) {
			base.OnMouseEnter(sender, e);
			if (mapShape != null)
				mapShape.OnMouseEnter(sender, e);
		}
		protected internal override void OnMouseLeave(object sender, MouseEventArgs e) {
			base.OnMouseLeave(sender, e);
			if (mapShape != null)
				mapShape.OnMouseLeave(sender, e);
		}
		protected internal override void OnMouseMove(object sender, MouseEventArgs e) {
			base.OnMouseMove(sender, e);
			if (mapShape != null)
				mapShape.OnMouseMove(sender, e);
		}
		protected internal override void OnMouseRightButtonDown(object sender, MouseButtonEventArgs e) {
			base.OnMouseRightButtonDown(sender, e);
			if (mapShape != null)
				mapShape.OnMouseRightButtonDown(sender, e);
		}
		protected internal override void OnMouseRightButtonUp(object sender, MouseButtonEventArgs e) {
			base.OnMouseRightButtonUp(sender, e);
			if (mapShape != null)
				mapShape.OnMouseRightButtonUp(sender, e);
		}
		protected internal override void OnMouseLeftButtonUp(object sender, MouseButtonEventArgs e) {
			base.OnMouseLeftButtonUp(sender, e);
			if (mapShape != null)
				mapShape.OnMouseLeftButtonUp(sender, e);
		}
		protected internal override void OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e) {
			base.OnMouseLeftButtonDown(sender, e);
			if (mapShape != null)
				mapShape.OnMouseLeftButtonDown(sender, e);
		}
		protected internal override System.Collections.Generic.IList<DevExpress.Map.CoordPoint> GetItemPoints() {
			return new DevExpress.Map.CoordPoint[]{ Layout.Location};
		}
	}
}
