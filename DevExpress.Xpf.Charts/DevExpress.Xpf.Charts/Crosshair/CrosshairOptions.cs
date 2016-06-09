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

using System.ComponentModel;
using System.Windows;
using System.Windows.Media;
using DevExpress.Charts.Native;
using DevExpress.Utils.Serializing;
using DevExpress.Xpf.Charts.Native;
using DevExpress.Xpf.Utils;
namespace DevExpress.Xpf.Charts {
	public enum CrosshairSnapMode {
		NearestArgument = CrosshairSnapModeCore.NearestArgument,
		NearestValue = CrosshairSnapModeCore.NearestValue
	}
	public enum CrosshairLabelMode {
		ShowForEachSeries = CrosshairLabelModeCore.ShowForEachSeries,
		ShowForNearestSeries = CrosshairLabelModeCore.ShowForNearestSeries,
		ShowCommonForAllSeries = CrosshairLabelModeCore.ShowCommonForAllSeries,
	}
	public class CrosshairOptions : ChartDependencyObject, ICrosshairOptions {
		public static readonly DependencyProperty ShowArgumentLabelsProperty = DependencyPropertyManager.Register("ShowArgumentLabels",
			typeof(bool), typeof(CrosshairOptions), new PropertyMetadata(false));
		public static readonly DependencyProperty ShowArgumentLineProperty = DependencyPropertyManager.Register("ShowArgumentLine",
			typeof(bool), typeof(CrosshairOptions), new PropertyMetadata(true));
		public static readonly DependencyProperty ShowCrosshairLabelsProperty = DependencyPropertyManager.Register("ShowCrosshairLabels",
			typeof(bool), typeof(CrosshairOptions), new PropertyMetadata(true));
		public static readonly DependencyProperty ShowOnlyInFocusedPaneProperty = DependencyPropertyManager.Register("ShowOnlyInFocusedPane",
			typeof(bool), typeof(CrosshairOptions), new PropertyMetadata(true));
		public static readonly DependencyProperty ShowValueLabelsProperty = DependencyPropertyManager.Register("ShowValueLabels",
			typeof(bool), typeof(CrosshairOptions), new PropertyMetadata(false));
		public static readonly DependencyProperty ShowValueLineProperty = DependencyPropertyManager.Register("ShowValueLine",
			typeof(bool), typeof(CrosshairOptions), new PropertyMetadata(false));
		public static readonly DependencyProperty SnapModeProperty = DependencyPropertyManager.Register("SnapMode",
			typeof(CrosshairSnapMode), typeof(CrosshairOptions), new PropertyMetadata(CrosshairSnapMode.NearestArgument, NotifyPropertyChanged));
		public static readonly DependencyProperty CrosshairLabelModeProperty = DependencyPropertyManager.Register("CrosshairLabelMode",
			typeof(CrosshairLabelMode), typeof(CrosshairOptions), new PropertyMetadata(CrosshairLabelMode.ShowCommonForAllSeries));
		public static readonly DependencyProperty CommonLabelPositionProperty = DependencyPropertyManager.Register("CommonLabelPosition",
			typeof(CrosshairLabelPosition), typeof(CrosshairOptions), new PropertyMetadata(null));
		public static readonly DependencyProperty ArgumentLineBrushProperty = DependencyPropertyManager.Register("ArgumentLineBrush",
			typeof(Brush), typeof(CrosshairOptions), new PropertyMetadata(NotifyPropertyChanged));
		public static readonly DependencyProperty ArgumentLineStyleProperty = DependencyPropertyManager.Register("ArgumentLineStyle",
			typeof(LineStyle), typeof(CrosshairOptions), new PropertyMetadata(NotifyPropertyChanged));
		public static readonly DependencyProperty ValueLineBrushProperty = DependencyPropertyManager.Register("ValueLineBrush",
			typeof(Brush), typeof(CrosshairOptions), new PropertyMetadata(NotifyPropertyChanged));
		public static readonly DependencyProperty ValueLineStyleProperty = DependencyPropertyManager.Register("ValueLineStyle",
			typeof(LineStyle), typeof(CrosshairOptions), new PropertyMetadata(NotifyPropertyChanged));
		public static readonly DependencyProperty HighlightPointsProperty = DependencyPropertyManager.Register("HighlightPoints",
			typeof(bool), typeof(CrosshairOptions), new PropertyMetadata(true));
		public static readonly DependencyProperty ShowGroupHeadersProperty = DependencyPropertyManager.Register("ShowGroupHeaders",
			typeof(bool), typeof(CrosshairOptions), new PropertyMetadata(true));
		public static readonly DependencyProperty GroupHeaderPatternProperty = DependencyPropertyManager.Register("GroupHeaderPattern",
			typeof(string), typeof(CrosshairOptions), new PropertyMetadata(""));
		#region ICrosshairOptions
		CrosshairSnapModeCore ICrosshairOptions.SnapMode { get { return (CrosshairSnapModeCore)SnapMode; } }
		CrosshairLabelModeCore ICrosshairOptions.LabelMode { get { return (CrosshairLabelModeCore)CrosshairLabelMode; } }
		ICrosshairFreePosition ICrosshairOptions.LabelPosition { get { return ActualCrosshairLabelPosition; } }
		bool ICrosshairOptions.ShowGroupHeaders { get { return ShowGroupHeaders; } }
		string ICrosshairOptions.GroupHeaderPattern { get { return GroupHeaderPattern; } }
		bool ICrosshairOptions.ShowTail {
			get {
				if (CrosshairLabelMode == CrosshairLabelMode.ShowCommonForAllSeries)
					return false;
				else
					return true;
			}
		}
		#endregion
		CrosshairLabelPosition ActualCrosshairLabelPosition { get { return CommonLabelPosition ?? new CrosshairMousePosition(); } }
		[
		Category(Categories.Behavior),
		XtraSerializableProperty
		]
		public bool ShowArgumentLabels {
			get { return (bool)GetValue(ShowArgumentLabelsProperty); }
			set { SetValue(ShowArgumentLabelsProperty, value); }
		}
		[
		Category(Categories.Behavior),
		XtraSerializableProperty
		]
		public bool ShowArgumentLine {
			get { return (bool)GetValue(ShowArgumentLineProperty); }
			set { SetValue(ShowArgumentLineProperty, value); }
		}
		[
		Category(Categories.Behavior),
		XtraSerializableProperty
		]
		public bool ShowCrosshairLabels {
			get { return (bool)GetValue(ShowCrosshairLabelsProperty); }
			set { SetValue(ShowCrosshairLabelsProperty, value); }
		}
		[
		Category(Categories.Behavior),
		XtraSerializableProperty
		]
		public bool ShowOnlyInFocusedPane {
			get { return (bool)GetValue(ShowOnlyInFocusedPaneProperty); }
			set { SetValue(ShowOnlyInFocusedPaneProperty, value); }
		}
		[
		Category(Categories.Behavior),
		XtraSerializableProperty
		]
		public bool ShowValueLabels {
			get { return (bool)GetValue(ShowValueLabelsProperty); }
			set { SetValue(ShowValueLabelsProperty, value); }
		}
		[
		Category(Categories.Behavior),
		XtraSerializableProperty
		]
		public bool ShowValueLine {
			get { return (bool)GetValue(ShowValueLineProperty); }
			set { SetValue(ShowValueLineProperty, value); }
		}
		[
		Category(Categories.Behavior),
		XtraSerializableProperty
		]
		public CrosshairSnapMode SnapMode {
			get { return (CrosshairSnapMode)GetValue(SnapModeProperty); }
			set { SetValue(SnapModeProperty, value); }
		}
		[
		Category(Categories.Behavior),
		XtraSerializableProperty
		]
		public CrosshairLabelMode CrosshairLabelMode {
			get { return (CrosshairLabelMode)GetValue(CrosshairLabelModeProperty); }
			set { SetValue(CrosshairLabelModeProperty, value); }
		}
		[
		Category(Categories.Behavior),
		XtraSerializableProperty(XtraSerializationVisibility.Content, true)
		]
		public CrosshairLabelPosition CommonLabelPosition {
			get { return (CrosshairLabelPosition)GetValue(CommonLabelPositionProperty); }
			set { SetValue(CommonLabelPositionProperty, value); }
		}
		[
		Category(Categories.Behavior),
		XtraSerializableProperty
		]
		public Brush ArgumentLineBrush {
			get { return (Brush)GetValue(ArgumentLineBrushProperty); }
			set { SetValue(ArgumentLineBrushProperty, value); }
		}
		[
		Category(Categories.Behavior),
		XtraSerializableProperty(XtraSerializationVisibility.Content, true)
		]
		public LineStyle ArgumentLineStyle {
			get { return (LineStyle)GetValue(ArgumentLineStyleProperty); }
			set { SetValue(ArgumentLineStyleProperty, value); }
		}
		[
		Category(Categories.Behavior),
		XtraSerializableProperty
		]
		public Brush ValueLineBrush {
			get { return (Brush)GetValue(ValueLineBrushProperty); }
			set { SetValue(ValueLineBrushProperty, value); }
		}
		[
		Category(Categories.Behavior),
		XtraSerializableProperty(XtraSerializationVisibility.Content, true)
		]
		public LineStyle ValueLineStyle {
			get { return (LineStyle)GetValue(ValueLineStyleProperty); }
			set { SetValue(ValueLineStyleProperty, value); }
		}
		[
		Category(Categories.Behavior),
		XtraSerializableProperty
		]
		public bool HighlightPoints {
			get { return (bool)GetValue(HighlightPointsProperty); }
			set { SetValue(HighlightPointsProperty, value); }
		}
		[
		Category(Categories.Behavior),
		XtraSerializableProperty
		]
		public bool ShowGroupHeaders {
			get { return (bool)GetValue(ShowGroupHeadersProperty); }
			set { SetValue(ShowGroupHeadersProperty, value); }
		}
		[
		Category(Categories.Behavior),
		XtraSerializableProperty
		]
		public string GroupHeaderPattern {
			get { return (string)GetValue(GroupHeaderPatternProperty); }
			set { SetValue(GroupHeaderPatternProperty, value); }
		}
		SolidColorBrush defaultArgumentLineBrush;
		SolidColorBrush defaultValueLineBrush;
		LineStyle defaultArgumentLineStyle;
		LineStyle defaultValueLineStyle;
		internal Brush ActualArgumentLineBrush { get { return ArgumentLineBrush != null ? ArgumentLineBrush : defaultArgumentLineBrush; } }
		internal Brush ActualValueLineBrush { get { return ValueLineBrush != null ? ValueLineBrush : defaultValueLineBrush; } }
		internal LineStyle ActualArgumentLineStyle { get { return ArgumentLineStyle != null ? ArgumentLineStyle : defaultArgumentLineStyle; } }
		internal LineStyle ActualValueLineStyle { get { return ValueLineStyle != null ? ValueLineStyle : defaultValueLineStyle; } }
		public CrosshairOptions() {
			defaultArgumentLineBrush = new SolidColorBrush(Color.FromArgb(0xFF, 0xDE, 0x39, 0xCD));
			defaultValueLineBrush = new SolidColorBrush(Color.FromArgb(0xFF, 0xDE, 0x39, 0xCD));
			defaultArgumentLineStyle = new LineStyle();
			defaultValueLineStyle = new LineStyle();
		}
		internal string[] GetAvailablePatternPlaceholders() {
			return SnapMode == CrosshairSnapMode.NearestArgument ? new string[1] { ToolTipPatternUtils.ArgumentPattern } : new string[1] { ToolTipPatternUtils.ValuePattern };
		}
		protected override ChartDependencyObject CreateObject() {
			return new CrosshairOptions();
		}
	}
	public abstract class CrosshairLabelPosition : ChartDependencyObject, ICrosshairFreePosition {
		public static readonly DependencyProperty OffsetProperty = DependencyPropertyManager.Register("Offset",
		   typeof(Point), typeof(CrosshairLabelPosition), new PropertyMetadata(new Point(12, 12)));
		[
		Category(Categories.Layout),
		XtraSerializableProperty
		]
		public Point Offset {
			get { return (Point)GetValue(OffsetProperty); }
			set { SetValue(OffsetProperty, value); }
		}
		protected virtual bool IsMousePosition { get { return true; } }
		#region IDockablePosition
		bool ICrosshairFreePosition.IsMousePosition { get { return IsMousePosition; } }
		GRealRect2D ICrosshairFreePosition.DockBounds { get { return GetDockBounds(); } }
		DockCornerCore ICrosshairFreePosition.DockCorner { get { return GetDockCorner(); } }
		GRealPoint2D ICrosshairFreePosition.Offset { get { return new GRealPoint2D(Offset.X, Offset.Y); } }
		#endregion        
		protected virtual GRealRect2D GetDockBounds() {
			return GRealRect2D.Empty;
		}
		protected virtual DockCornerCore GetDockCorner() {
			return DockCornerCore.TopLeft;
		}
	}
	public class CrosshairMousePosition : CrosshairLabelPosition {
		protected override ChartDependencyObject CreateObject() {
			return new CrosshairMousePosition();
		}
	}
	public class CrosshairFreePosition : CrosshairLabelPosition {
		public static readonly DependencyProperty DockTargetProperty = DependencyPropertyManager.Register("DockTarget",
			typeof(IDockTarget), typeof(CrosshairFreePosition), new PropertyMetadata(null));
		public static readonly DependencyProperty DockCornerProperty = DependencyPropertyManager.Register("DockCorner",
			typeof(DockCorner), typeof(CrosshairFreePosition), new PropertyMetadata(DockCorner.TopLeft));
		[Category(Categories.Layout)]
		public IDockTarget DockTarget {
			get { return (IDockTarget)GetValue(DockTargetProperty); }
			set { SetValue(DockTargetProperty, value); }
		}
		[
		Category(Categories.Layout),
		XtraSerializableProperty
		]
		public DockCorner DockCorner {
			get { return (DockCorner)GetValue(DockCornerProperty); }
			set { SetValue(DockCornerProperty, value); }
		}
		protected override bool IsMousePosition { get { return false; } }
		protected override ChartDependencyObject CreateObject() {
			return new CrosshairFreePosition();
		}
		protected override GRealRect2D GetDockBounds() {
			IDockTarget dockTarget = DockTarget;
			if (dockTarget != null) {
				Rect bounds = dockTarget.GetBounds();
				return new GRealRect2D(bounds.X, bounds.Y, bounds.Width, bounds.Height);
			}
			return GRealRect2D.Empty;
		}
		protected override DockCornerCore GetDockCorner() {
			return (DockCornerCore)DockCorner;
		}
	}
}
