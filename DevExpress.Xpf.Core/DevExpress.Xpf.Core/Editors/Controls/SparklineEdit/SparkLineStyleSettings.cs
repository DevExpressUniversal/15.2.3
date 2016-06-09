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
using System.ComponentModel;
using System.Windows;
using System.Windows.Media;
using DevExpress.Utils.Design;
using DevExpress.Xpf.Utils;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Editors.Helpers;
using DevExpress.Xpf.Editors.Themes;
namespace DevExpress.Xpf.Editors {
	public abstract class SparklineStyleSettings : BaseEditStyleSettings {
		public static readonly DependencyProperty HighlightMinPointProperty;
		public static readonly DependencyProperty HighlightMaxPointProperty;
		public static readonly DependencyProperty HighlightStartPointProperty;
		public static readonly DependencyProperty HighlightEndPointProperty;
		public static readonly DependencyProperty BrushProperty;
		public static readonly DependencyProperty MaxPointBrushProperty;
		public static readonly DependencyProperty MinPointBrushProperty;
		public static readonly DependencyProperty StartPointBrushProperty;
		public static readonly DependencyProperty EndPointBrushProperty;
		public static readonly DependencyProperty NegativePointBrushProperty;
		static SparklineStyleSettings() {
			Type ownerType = typeof(SparklineStyleSettings);
			HighlightMinPointProperty = DependencyPropertyManager.Register("HighlightMinPoint", typeof(bool), ownerType, new FrameworkPropertyMetadata(false));
			HighlightMaxPointProperty = DependencyPropertyManager.Register("HighlightMaxPoint", typeof(bool), ownerType, new FrameworkPropertyMetadata(false));
			HighlightStartPointProperty = DependencyPropertyManager.Register("HighlightStartPoint", typeof(bool), ownerType, new FrameworkPropertyMetadata(false));
			HighlightEndPointProperty = DependencyPropertyManager.Register("HighlightEndPoint", typeof(bool), ownerType, new FrameworkPropertyMetadata(false));
			BrushProperty = DependencyPropertyManager.Register("Brush", typeof(SolidColorBrush), ownerType, new FrameworkPropertyMetadata(null));
			MaxPointBrushProperty = DependencyPropertyManager.Register("MaxPointBrush", typeof(SolidColorBrush), ownerType, new FrameworkPropertyMetadata(null));
			MinPointBrushProperty = DependencyPropertyManager.Register("MinPointBrush", typeof(SolidColorBrush), ownerType, new FrameworkPropertyMetadata(null));
			StartPointBrushProperty = DependencyPropertyManager.Register("StartPointBrush", typeof(SolidColorBrush), ownerType, new FrameworkPropertyMetadata(null));
			EndPointBrushProperty = DependencyPropertyManager.Register("EndPointBrush", typeof(SolidColorBrush), ownerType, new FrameworkPropertyMetadata(null));
			NegativePointBrushProperty = DependencyPropertyManager.Register("NegativePointBrush", typeof(SolidColorBrush), ownerType, new FrameworkPropertyMetadata(null));
		}
		public bool HighlightMinPoint {
			get { return (bool)GetValue(HighlightMinPointProperty); }
			set { SetValue(HighlightMinPointProperty, value); }
		}
		public bool HighlightMaxPoint {
			get { return (bool)GetValue(HighlightMaxPointProperty); }
			set { SetValue(HighlightMaxPointProperty, value); }
		}
		public bool HighlightStartPoint {
			get { return (bool)GetValue(HighlightStartPointProperty); }
			set { SetValue(HighlightStartPointProperty, value); }
		}
		public bool HighlightEndPoint {
			get { return (bool)GetValue(HighlightEndPointProperty); }
			set { SetValue(HighlightEndPointProperty, value); }
		}
		public SolidColorBrush Brush {
			get { return (SolidColorBrush)GetValue(BrushProperty); }
			set { SetValue(BrushProperty, value); }
		}
		public SolidColorBrush MaxPointBrush {
			get { return (SolidColorBrush)GetValue(MaxPointBrushProperty); }
			set { SetValue(MaxPointBrushProperty, value); }
		}
		public SolidColorBrush MinPointBrush {
			get { return (SolidColorBrush)GetValue(MinPointBrushProperty); }
			set { SetValue(MinPointBrushProperty, value); }
		}
		public SolidColorBrush StartPointBrush {
			get { return (SolidColorBrush)GetValue(StartPointBrushProperty); }
			set { SetValue(StartPointBrushProperty, value); }
		}
		public SolidColorBrush EndPointBrush {
			get { return (SolidColorBrush)GetValue(EndPointBrushProperty); }
			set { SetValue(EndPointBrushProperty, value); }
		}
		public SolidColorBrush NegativePointBrush {
			get { return (SolidColorBrush)GetValue(NegativePointBrushProperty); }
			set { SetValue(NegativePointBrushProperty, value); }
		}
		protected abstract SparklineViewType ViewType { get; }
		public override void ApplyToEdit(BaseEdit editor) {
			base.ApplyToEdit(editor);
			((SparklineEdit)editor).SparklineType = ViewType;
			SparklinePropertyProvider provider = (SparklinePropertyProvider)editor.PropertyProvider;
			provider.HighlightMinPoint = HighlightMinPoint;
			provider.HighlightMaxPoint = HighlightMaxPoint;
			provider.HighlightStartPoint = HighlightStartPoint;
			provider.HighlightEndPoint = HighlightEndPoint;
			provider.Brush = this.IsPropertySet(BrushProperty) ? Brush : FindResource(new SparklineEditThemeKeyExtension() { ResourceKey = SparklineEditThemeKeys.Brush, ThemeName = ThemeHelper.GetEditorThemeName(editor) }) as SolidColorBrush;
			provider.MaxPointBrush = this.IsPropertySet(MaxPointBrushProperty) ? MaxPointBrush : FindResource(new SparklineEditThemeKeyExtension() { ResourceKey = SparklineEditThemeKeys.MaxPointBrush, ThemeName = ThemeHelper.GetEditorThemeName(editor) }) as SolidColorBrush;
			provider.MinPointBrush = this.IsPropertySet(MinPointBrushProperty) ? MinPointBrush : FindResource(new SparklineEditThemeKeyExtension() { ResourceKey = SparklineEditThemeKeys.MinPointBrush, ThemeName = ThemeHelper.GetEditorThemeName(editor) }) as SolidColorBrush;
			provider.StartPointBrush = this.IsPropertySet(StartPointBrushProperty) ? StartPointBrush : FindResource(new SparklineEditThemeKeyExtension() { ResourceKey = SparklineEditThemeKeys.StartPointBrush, ThemeName = ThemeHelper.GetEditorThemeName(editor) }) as SolidColorBrush;
			provider.EndPointBrush = this.IsPropertySet(EndPointBrushProperty) ? EndPointBrush : FindResource(new SparklineEditThemeKeyExtension() { ResourceKey = SparklineEditThemeKeys.EndPointBrush, ThemeName = ThemeHelper.GetEditorThemeName(editor) }) as SolidColorBrush;
			provider.NegativePointBrush = this.IsPropertySet(NegativePointBrushProperty) ? NegativePointBrush : FindResource(new SparklineEditThemeKeyExtension() { ResourceKey = SparklineEditThemeKeys.NegativePointBrush, ThemeName = ThemeHelper.GetEditorThemeName(editor) }) as SolidColorBrush;
		}
	}
	public class LineSparklineStyleSettings : SparklineStyleSettings {
		public static readonly DependencyProperty LineWidthProperty;
		public static readonly DependencyProperty HighlightNegativePointsProperty;
		public static readonly DependencyProperty ShowMarkersProperty;
		public static readonly DependencyProperty MarkerSizeProperty;
		public static readonly DependencyProperty MaxPointMarkerSizeProperty;
		public static readonly DependencyProperty MinPointMarkerSizeProperty;
		public static readonly DependencyProperty StartPointMarkerSizeProperty;
		public static readonly DependencyProperty EndPointMarkerSizeProperty;
		public static readonly DependencyProperty NegativePointMarkerSizeProperty;
		public static readonly DependencyProperty MarkerBrushProperty;
		static LineSparklineStyleSettings() {
			Type ownerType = typeof(LineSparklineStyleSettings);
			LineWidthProperty = DependencyProperty.Register("LineWidth", typeof(int), ownerType, new FrameworkPropertyMetadata(1));
			HighlightNegativePointsProperty = DependencyProperty.Register("HighlightNegativePoints", typeof(bool), ownerType, new FrameworkPropertyMetadata(false));
			ShowMarkersProperty = DependencyProperty.Register("ShowMarkers", typeof(bool), ownerType, new FrameworkPropertyMetadata(false));
			MarkerSizeProperty = DependencyProperty.Register("MarkerSize", typeof(int), ownerType, new FrameworkPropertyMetadata(5));
			MaxPointMarkerSizeProperty = DependencyProperty.Register("MaxPointMarkerSize", typeof(int), ownerType, new FrameworkPropertyMetadata(5));
			MinPointMarkerSizeProperty = DependencyProperty.Register("MinPointMarkerSize", typeof(int), ownerType, new FrameworkPropertyMetadata(5));
			StartPointMarkerSizeProperty = DependencyProperty.Register("StartPointMarkerSize", typeof(int), ownerType, new FrameworkPropertyMetadata(5));
			EndPointMarkerSizeProperty = DependencyProperty.Register("EndPointMarkerSize", typeof(int), ownerType, new FrameworkPropertyMetadata(5));
			NegativePointMarkerSizeProperty = DependencyProperty.Register("NegativePointMarkerSize", typeof(int), ownerType, new FrameworkPropertyMetadata(5));
			MarkerBrushProperty = DependencyProperty.Register("MarkerBrush", typeof(SolidColorBrush), ownerType, new FrameworkPropertyMetadata(null));
		}
		protected override SparklineViewType ViewType { get { return SparklineViewType.Line; } }
		[Category("Behavior"), TypeConverter(typeof(BooleanTypeConverter))]
		public bool HighlightNegativePoints {
			get { return (bool)GetValue(HighlightNegativePointsProperty); }
			set { SetValue(HighlightNegativePointsProperty, value); }
		}
		[Category("Behavior"), TypeConverter(typeof(BooleanTypeConverter))]
		public bool ShowMarkers {
			get { return (bool)GetValue(ShowMarkersProperty); }
			set { SetValue(ShowMarkersProperty, value); }
		}
		[Category("Appearance")]
		public int LineWidth {
			get { return (int)GetValue(LineWidthProperty); }
			set { SetValue(LineWidthProperty, value); }
		}
		[Category("Appearance")]
		public int MarkerSize {
			get { return (int)GetValue(MarkerSizeProperty); }
			set { SetValue(MarkerSizeProperty, value); }
		}
		[Category("Appearance")]
		public int MaxPointMarkerSize {
			get { return (int)GetValue(MaxPointMarkerSizeProperty); }
			set { SetValue(MaxPointMarkerSizeProperty, value); }
		}
		[Category("Appearance")]
		public int MinPointMarkerSize {
			get { return (int)GetValue(MinPointMarkerSizeProperty); }
			set { SetValue(MinPointMarkerSizeProperty, value); }
		}
		[Category("Appearance")]
		public int StartPointMarkerSize {
			get { return (int)GetValue(StartPointMarkerSizeProperty); }
			set { SetValue(StartPointMarkerSizeProperty, value); }
		}
		[Category("Appearance")]
		public int EndPointMarkerSize {
			get { return (int)GetValue(EndPointMarkerSizeProperty); }
			set { SetValue(EndPointMarkerSizeProperty, value); }
		}
		[Category("Appearance")]
		public int NegativePointMarkerSize {
			get { return (int)GetValue(NegativePointMarkerSizeProperty); }
			set { SetValue(NegativePointMarkerSizeProperty, value); }
		}
		[Category("Appearance")]
		public SolidColorBrush MarkerBrush {
			get { return (SolidColorBrush)GetValue(MarkerBrushProperty); }
			set { SetValue(MarkerBrushProperty, value); }
		}
		public override void ApplyToEdit(BaseEdit editor) {
			base.ApplyToEdit(editor);
			SparklinePropertyProvider provider = (SparklinePropertyProvider)editor.PropertyProvider;
			provider.LineWidth = LineWidth;
			provider.HighlightNegativePoints = HighlightNegativePoints;
			provider.ShowMarkers = ShowMarkers;
			provider.MarkerSize = MarkerSize;
			provider.MaxPointMarkerSize = MaxPointMarkerSize;
			provider.MinPointMarkerSize = MinPointMarkerSize;
			provider.StartPointMarkerSize = StartPointMarkerSize;
			provider.EndPointMarkerSize = EndPointMarkerSize;
			provider.NegativePointMarkerSize = NegativePointMarkerSize;
			provider.MarkerBrush = this.IsPropertySet(MarkerBrushProperty) ? MarkerBrush : FindResource(new SparklineEditThemeKeyExtension() { ResourceKey = SparklineEditThemeKeys.MarkerBrush, ThemeName = ThemeHelper.GetEditorThemeName(editor) }) as SolidColorBrush;			
		}
	}
	public class AreaSparklineStyleSettings : LineSparklineStyleSettings {
		public static readonly DependencyProperty AreaOpacityProperty;
		static AreaSparklineStyleSettings() {
			Type ownerType = typeof(AreaSparklineStyleSettings);
			AreaOpacityProperty = DependencyPropertyManager.Register("AreaOpacity", typeof(double), ownerType, new FrameworkPropertyMetadata(135d / 255d));
		}
		public double AreaOpacity {
			get { return (double)GetValue(AreaOpacityProperty); }
			set { SetValue(AreaOpacityProperty, value); }
		}
		protected override SparklineViewType ViewType { get { return SparklineViewType.Area; } }
		public override void ApplyToEdit(BaseEdit editor) {
			base.ApplyToEdit(editor);
			SparklinePropertyProvider provider = (SparklinePropertyProvider)editor.PropertyProvider;
			provider.AreaOpacity = AreaOpacity;
		}
	}
	public class BarSparklineStyleSettings : SparklineStyleSettings {
		public static readonly DependencyProperty HighlightNegativePointsProperty;
		public static readonly DependencyProperty BarDistanceProperty;
		static BarSparklineStyleSettings() {
			Type ownerType = typeof(BarSparklineStyleSettings);
			HighlightNegativePointsProperty = DependencyProperty.Register("HighlightNegativePoints", typeof(bool), ownerType, new FrameworkPropertyMetadata(false));
			BarDistanceProperty = DependencyProperty.Register("BarDistance", typeof(int), ownerType, new FrameworkPropertyMetadata(2));
		}
		protected override SparklineViewType ViewType { get { return SparklineViewType.Bar; } }
		[Category("Behavior"), TypeConverter(typeof(BooleanTypeConverter))]
		public bool HighlightNegativePoints {
			get { return (bool)GetValue(HighlightNegativePointsProperty); }
			set { SetValue(HighlightNegativePointsProperty, value); }
		}
		public int BarDistance {
			get { return (int)GetValue(BarDistanceProperty); }
			set { SetValue(BarDistanceProperty, value); }
		}
		public override void ApplyToEdit(BaseEdit editor) {
			base.ApplyToEdit(editor);
			SparklinePropertyProvider provider = (SparklinePropertyProvider)editor.PropertyProvider;
			provider.HighlightNegativePoints = HighlightNegativePoints;
			provider.BarDistance = BarDistance;
		}
	}
	public class WinLossSparklineStyleSettings : SparklineStyleSettings {
		public static readonly DependencyProperty BarDistanceProperty;
		static WinLossSparklineStyleSettings() {
			Type ownerType = typeof(WinLossSparklineStyleSettings);
			BarDistanceProperty = DependencyProperty.Register("BarDistance", typeof(int), ownerType, new FrameworkPropertyMetadata(2));
		}
		protected override SparklineViewType ViewType { get { return SparklineViewType.WinLoss; } }
		public int BarDistance {
			get { return (int)GetValue(BarDistanceProperty); }
			set { SetValue(BarDistanceProperty, value); }
		}
		public override void ApplyToEdit(BaseEdit editor) {
			base.ApplyToEdit(editor);
			SparklinePropertyProvider provider = (SparklinePropertyProvider)editor.PropertyProvider;
			provider.BarDistance = BarDistance;
		}
	}
}
