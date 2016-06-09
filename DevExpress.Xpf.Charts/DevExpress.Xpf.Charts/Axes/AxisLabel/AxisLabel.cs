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
using DevExpress.Charts.Native;
using DevExpress.Utils.Serializing;
using DevExpress.Xpf.Charts.Localization;
using DevExpress.Xpf.Charts.Native;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Utils;
namespace DevExpress.Xpf.Charts {
	public class AxisLabel : ChartTextElement, IAxisLabel, ISupportFlowDirection, IWeakEventListener, IPatternHolder {
		public static readonly DependencyProperty StaggeredProperty = DependencyPropertyManager.Register("Staggered",
			typeof(bool), typeof(AxisLabel), new PropertyMetadata(false, ChartElementHelper.UpdateWithClearDiagramCache));
		[Obsolete(ObsoleteMessages.AxisLabelBeginTextProperty)]
		public static readonly DependencyProperty BeginTextProperty = DependencyPropertyManager.Register("BeginText",
			typeof(string), typeof(AxisLabel), new PropertyMetadata("", BeginTextPropertyChanged));
		[Obsolete(ObsoleteMessages.AxisLabelEndTextProperty)]
		public static readonly DependencyProperty EndTextProperty = DependencyPropertyManager.Register("EndText",
			typeof(string), typeof(AxisLabel), new PropertyMetadata("", EndTextPropertyChanged));
		public static readonly DependencyProperty AngleProperty = DependencyPropertyManager.Register("Angle",
			typeof(int), typeof(AxisLabel), new PropertyMetadata(0, ChartElementHelper.UpdateWithClearDiagramCache, CoerceAngle));
		public static readonly DependencyProperty TextPatternProperty = DependencyPropertyManager.Register("TextPattern",
			typeof(string), typeof(AxisLabel), new PropertyMetadata(string.Empty, TextPatternPropertyChanged));
		static object CoerceAngle(DependencyObject d, object value) {
			int angle = (int)value;
			if (angle > 360 || angle < -360)
				throw new InvalidOperationException(ChartLocalizer.GetString(ChartStringId.MsgIncorrectLabelAngle));
			return value;
		}
		internal static void ResolveOverlappingOptionsPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			AxisLabel axisLabel = d as AxisLabel;
			if (axisLabel != null) {
				AxisLabelResolveOverlappingOptions newOptions = e.NewValue as AxisLabelResolveOverlappingOptions;
				CommonUtils.SubscribePropertyChangedWeakEvent(e.OldValue as AxisLabelResolveOverlappingOptions, newOptions, axisLabel);
				axisLabel.actualResolveOverlappingOptions = newOptions != null ? newOptions : new AxisLabelResolveOverlappingOptions();
			}
			ChartElementHelper.Update(d, e);
		}
		static void BeginTextPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			AxisLabel axisLabel = d as AxisLabel;
			if (axisLabel != null) {
				axisLabel.beginText = (string)e.NewValue;
				axisLabel.UpdateTextPattern();
			}
			ChartElementHelper.UpdateWithClearDiagramCache(d, e);
		}
		static void EndTextPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			AxisLabel axisLabel = d as AxisLabel;
			if (axisLabel != null) {
				axisLabel.endText = (string)e.NewValue;
				axisLabel.UpdateTextPattern();
			}
			ChartElementHelper.UpdateWithClearDiagramCache(d, e);
		}
		internal static void TextPatternPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			AxisLabel axisLabel = d as AxisLabel;
			if (axisLabel != null) {
				axisLabel.textPattern = (string)e.NewValue;
				axisLabel.textPatternSyncronized = false;
				if (!axisLabel.textPatternUpdateLocked)
					ChartElementHelper.UpdateWithClearDiagramCache(d, e);
			}
		}
		static AxisLabel() {
			FlowDirectionProperty.OverrideMetadata(typeof(AxisLabel), new FrameworkPropertyMetadata(ChartElementHelper.UpdateWithClearDiagramCache));
		}
		[
#if !SL
	DevExpressXpfChartsLocalizedDescription("AxisLabelStaggered"),
#endif
		Category(Categories.Behavior),
		XtraSerializableProperty
		]
		public bool Staggered {
			get { return (bool)GetValue(StaggeredProperty); }
			set { SetValue(StaggeredProperty, value); }
		}
		[
		Obsolete(ObsoleteMessages.AxisLabelBeginTextProperty),
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never)
		]
		public string BeginText {
			get { return (string)GetValue(BeginTextProperty); }
			set { SetValue(BeginTextProperty, value); }
		}
		[
		Obsolete(ObsoleteMessages.AxisLabelEndTextProperty),
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never)
		]
		public string EndText {
			get { return (string)GetValue(EndTextProperty); }
			set { SetValue(EndTextProperty, value); }
		}
		[
#if !SL
	DevExpressXpfChartsLocalizedDescription("AxisLabelAngle"),
#endif
		Category(Categories.Presentation),
		XtraSerializableProperty
		]
		public int Angle {
			get { return (int)GetValue(AngleProperty); }
			set { SetValue(AngleProperty, value); }
		}
		[
#if !SL
	DevExpressXpfChartsLocalizedDescription("AxisLabelTextPattern"),
#endif
		Category(Categories.Presentation),
		XtraSerializableProperty
		]
		public string TextPattern {
			get { return (string)GetValue(TextPatternProperty); }
			set { SetValue(TextPatternProperty, value); }
		}
		string beginText;
		string endText;
		string textPattern;
		bool textPatternSyncronized = false;
		bool textPatternUpdateLocked = false;
		AxisLabelResolveOverlappingOptions actualResolveOverlappingOptions;
		IAxisLabelFormatter formatter;
		#region IAxisLabel implememtation
		IAxisLabelResolveOverlappingOptions IAxisLabel.ResolveOverlappingOptions {
			get { return actualResolveOverlappingOptions; }
		}
		IAxisLabelFormatterCore IAxisLabel.Formatter {
			get { return Formatter; }
			set { Formatter = (IAxisLabelFormatter)value; }
		}
		string IAxisLabel.TextPattern { get { return ActualTextPattern; } }
		#endregion
		#region IWeakEventListener implementation
		bool IWeakEventListener.ReceiveWeakEvent(Type managerType, object sender, EventArgs e) {
			return managerType == typeof(PropertyChangedWeakEventManager) && ProcessWeakEvent(sender);
		}
		#endregion
		#region IPatternHolder implementation
		PatternDataProvider IPatternHolder.GetDataProvider(PatternConstants patternConstant) { return new AxisPatternDataProvider(); }
		string IPatternHolder.PointPattern { get { return ActualTextPattern; } }
		#endregion
		internal string ActualTextPattern { get { return !string.IsNullOrEmpty(textPattern) ? textPattern : PatternUtils.ConstructDefaultPattern(Axis); } }
		internal AxisBase Axis { get { return ((IOwnedElement)this).Owner as AxisBase; } }
		internal IAxisLabelFormatter Formatter {
			get { return formatter; }
			set { formatter = value; }
		}
		public AxisLabel() {
			DefaultStyleKey = typeof(AxisLabel);
			actualResolveOverlappingOptions = new AxisLabelResolveOverlappingOptions();
		}
		bool ProcessWeakEvent(object sender) {
			if ((sender is AxisLabelResolveOverlappingOptions)) {
				ChartElementHelper.UpdateWithClearDiagramCache(this);
				return true;
			}
			return false;
		}
		Transform ISupportFlowDirection.CreateDirectionTransform() {
			if (FlowDirection == FlowDirection.RightToLeft)
				return new ScaleTransform() { ScaleX = -1, ScaleY = 1, CenterX = 0.5, CenterY = 0.5 };
			return Transform.Identity;
		}
		internal void UpdateTextPattern() {
			string valuePlaceholder = string.Empty;
			string format = string.Empty;
			if (Axis != null) {
				valuePlaceholder = Axis.IsValuesAxis ? PatternUtils.ValuePlaceholder : PatternUtils.ArgumentPlaceholder;
				if (Axis.ScaleMap.ScaleType == ActualScaleType.DateTime && Axis.DateTimeOptionsImpl != null)
					format = ":" + DateTimeOptionsHelper.GetFormatString(Axis.DateTimeOptionsImpl);
				if (Axis.ScaleMap.ScaleType == ActualScaleType.Numerical && Axis.NumericOptionsInternal != null)
					format = ":" + NumericOptionsHelper.GetFormatString(Axis.NumericOptionsInternal);
			}
			TextPattern = beginText + "{" + valuePlaceholder + format + "}" + endText;
			textPatternSyncronized = true;
		}
		internal void SyncronizeTextPatterWithScaleType() {
			if (textPatternSyncronized) {
				textPatternUpdateLocked = true;
				UpdateTextPattern();
				textPatternUpdateLocked = false;
			}
		}
	}
}
