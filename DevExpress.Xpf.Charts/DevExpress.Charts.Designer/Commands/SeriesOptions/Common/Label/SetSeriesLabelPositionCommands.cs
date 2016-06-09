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

extern alias Platform;
using System;
using System.Globalization;
using System.Security;
using System.Windows.Data;
using DevExpress.Xpf.Charts;
using DevExpress.Xpf.Charts.Native;
using Platform::DevExpress.Mvvm.UI.Native.ViewGenerator.Model;
namespace DevExpress.Charts.Designer.Native {
	public abstract class SetSeriesLabelPositionCommand : SeriesOptionsCommandBase, IValueConverter {
		readonly string caption;
		readonly CreateSeriesLabelCommand createSeriesLabelCommand;
		readonly string description;
		readonly string glyph;
		readonly Type[] seriesTypes;
		protected abstract ComplexLabelPosition LabelPosition {
			get;
		}
		public override string Caption {
			get { return caption; }
		}
		public override string Description {
			get { return description; }
		}
		public override string ImageName {
			get { return glyph; }
		}
		public SetSeriesLabelPositionCommand(WpfChartModel chartModel, Type[] seriesTypes, string caption, string description, string glyph)
			: base(chartModel) {
			this.caption = caption;
			this.glyph = glyph;
			this.seriesTypes = seriesTypes;
			this.description = description;
			this.createSeriesLabelCommand = new CreateSeriesLabelCommand(chartModel);
		}
		public SetSeriesLabelPositionCommand(WpfChartModel chartModel, Type seriesType, string caption, string description, string glyph)
			: this(chartModel, new Type[] { seriesType }, caption, description, glyph) { }
		object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			if (value != null) {
				if (value is WpfChartSeriesModel)
					return ((WpfChartSeriesModel)value).ComplexLabelPosition == LabelPosition;
				else if (value is ComplexLabelPosition)
					return (ComplexLabelPosition)value == LabelPosition;
			}
			return false;
		}
		object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
			return value;
		}
		protected override bool CanExecute(object parameter) {
			if (base.CanExecute(parameter)) {
				bool isSeriesCompatible = false;
				foreach (Type seriesType in seriesTypes)
					isSeriesCompatible |= seriesType.IsAssignableFrom(SeriesModel.Series.GetType());
				return isSeriesCompatible;
			}
			return false;
		}
		[SecuritySafeCritical]
		protected override void DesignTimeApplyInternal(IModelItem seriesAccess, object value) {
			if (value != null) {
				IModelItem labelAccess = seriesAccess.Properties["Label"].Value;
				ComplexLabelPosition labelPosition = (ComplexLabelPosition)value;
				Type seriesType = seriesAccess.ItemType;
				if (labelPosition == ComplexLabelPosition.Disabled) {
					seriesAccess.Properties["LabelsVisibility"].SetValue(false);
				}
				else {
					seriesAccess.Properties["LabelsVisibility"].SetValue(true);
					if (typeof(BarSideBySideSeries2D).IsAssignableFrom(seriesType))
						labelAccess.Properties[new DXPropertyIdentifier(typeof(BarSideBySideSeries2D), "LabelPosition")].SetValue(WpfChartSeriesModel.ComplexToFact(labelPosition));
					else if (typeof(PieSeries2D).IsAssignableFrom(seriesType))
						labelAccess.Properties[new DXPropertyIdentifier(typeof(PieSeries2D), "LabelPosition")].SetValue(WpfChartSeriesModel.ComplexToFact(labelPosition));
					else if (typeof(FunnelSeries2D).IsAssignableFrom(seriesType))
						labelAccess.Properties[new DXPropertyIdentifier(typeof(FunnelSeries2D), "LabelPosition")].SetValue(WpfChartSeriesModel.ComplexToFact(labelPosition));
					else if (typeof(MarkerSeries3D).IsAssignableFrom(seriesType))
						labelAccess.Properties[new DXPropertyIdentifier(typeof(MarkerSeries3D), "LabelPosition")].SetValue(WpfChartSeriesModel.ComplexToFact(labelPosition));
					else if (typeof(BubbleSeries2D).IsAssignableFrom(seriesType)) {
						object bubblePosition = WpfChartSeriesModel.ComplexToFact(labelPosition);
						if (bubblePosition != null)
							labelAccess.Properties[new DXPropertyIdentifier(typeof(BubbleSeries2D), "LabelPosition")].SetValue(WpfChartSeriesModel.ComplexToFact(labelPosition));
						else {
							double angle = WpfChartSeriesModel.ComplexToAngle(labelPosition);
							if (!double.IsNaN(angle)) {
								labelAccess.Properties[new DXPropertyIdentifier(typeof(BubbleSeries2D), "LabelPosition")].SetValue(Bubble2DLabelPosition.Outside);
								labelAccess.Properties[new DXPropertyIdentifier(typeof(MarkerSeries2D), "Angle")].SetValue(angle);
							}
						}
					}
					else if (typeof(MarkerSeries2D).IsAssignableFrom(seriesType))
						labelAccess.Properties[new DXPropertyIdentifier(typeof(MarkerSeries2D), "Angle")].SetValue(WpfChartSeriesModel.ComplexToAngle(labelPosition));
					else if (typeof(CircularSeries2D).IsAssignableFrom(seriesType))
						labelAccess.Properties[new DXPropertyIdentifier(typeof(CircularSeries2D), "Angle")].SetValue(WpfChartSeriesModel.ComplexToAngle(labelPosition));
				}
			}
		}
		protected override void RedoInternal(WpfChartSeriesModel model, object newValue) {
			model.ComplexLabelPosition = (ComplexLabelPosition)newValue;
		}
		protected override void RuntimeApplyInternal(Series series, object value) {
			WpfChartSeriesModel.SetComplexLabelPosition(series, (ComplexLabelPosition)value);
		}
		protected override void UndoInternal(WpfChartSeriesModel model, object oldValue) {
			model.ComplexLabelPosition = (ComplexLabelPosition)oldValue;
		}
		public override CommandResult RuntimeExecute(object parameter) {
			if ((parameter != null) && ((bool)parameter)) {
				CompositeHistoryItem compositeHistory = new CompositeHistoryItem();
				if (SeriesModel.Series.Label == null)
					compositeHistory.HistoryItems.Add(createSeriesLabelCommand.RuntimeExecute(SeriesModel).HistoryItem);
				ComplexLabelPosition oldValue = SeriesModel.ComplexLabelPosition;
				SeriesModel.ComplexLabelPosition = LabelPosition;
				ElementIndexItem[] indexItems = new ElementIndexItem[2];
				indexItems[0] = new ElementIndexItem(SeriesIndexKey, SeriesModel.GetSelfIndex());
				indexItems[1] = new ElementIndexItem(SeriesDesigntimeIndexKey, SeriesModel.GetSelfDesigntimeIndex());
				compositeHistory.HistoryItems.Add(new HistoryItem(new ExecuteCommandInfo(parameter, indexItems), this, SeriesModel, oldValue, LabelPosition));
				return new CommandResult(compositeHistory, SeriesModel.Series);
			}
			return null;
		}
	}
	public class SetNoneLabelPositionCommand : SetSeriesLabelPositionCommand {
		static string GetCaption() {
			return ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.SeriesOptions_Labels_NoneCaption);
		}
		static string GetDescription() {
			return ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.SeriesOptions_Labels_NoneDescription);
		}
		static string GetGlyphPath() {
			return GlyphUtils.GalleryItemImages + @"SeriesLabelPosition\None";
		}
		protected override ComplexLabelPosition LabelPosition {
			get { return ComplexLabelPosition.Disabled; }
		}
		public SetNoneLabelPositionCommand(WpfChartModel chartModel)
			: base(chartModel, typeof(object), GetCaption(), GetDescription(), GetGlyphPath()) {
		}
	}
	public class SetFinancialSeriesLabelEnabledCommand : SetSeriesLabelPositionCommand {
		static string GetCaption() {
			return ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.SeriesOptions_Labels_EnabledCaption);
		}
		static string GetDescription() {
			return ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.SeriesOptions_Labels_EnabledDescription);
		}
		static string GetGlyphPath() {
			return GlyphUtils.GalleryItemImages + @"SeriesLabelPosition\FinancialSeriesLabelVisible";
		}
		protected override ComplexLabelPosition LabelPosition {
			get { return ComplexLabelPosition.FinancialEnabled; }
		}
		public SetFinancialSeriesLabelEnabledCommand(WpfChartModel chartModel)
			: base(chartModel, typeof(FinancialSeries2D), GetCaption(), GetDescription(), GetGlyphPath()) {
		}
	}
	public class SetBarSeries3DLabelEnabledCommand : SetSeriesLabelPositionCommand {
		protected override ComplexLabelPosition LabelPosition {
			get { return ComplexLabelPosition.Bar3DEnabled; }
		}
		public SetBarSeries3DLabelEnabledCommand(WpfChartModel chartModel)
			: base(chartModel, typeof(BarSeries3D), ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.SeriesOptions_Labels_Bar3DEnabledCaption), ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.SeriesOptions_Labels_Bar3DEnabledDescription), GlyphUtils.GalleryItemImages + @"SeriesLabelPosition\BarSeries3DLabelVisible") { }
	}
	public class SetArea3DLabelEnabledCommand : SetSeriesLabelPositionCommand {
		protected override ComplexLabelPosition LabelPosition {
			get { return ComplexLabelPosition.Area3DEnabled; }
		}
		public SetArea3DLabelEnabledCommand(WpfChartModel chartModel)
			: base(chartModel, typeof(AreaSeries3D), ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.SeriesOptions_Labels_Area3DEnabledCaption), ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.SeriesOptions_Labels_Area3DEnabledDescription), GlyphUtils.GalleryItemImages + @"SeriesLabelPosition\AreaSeries3DLabelVisible") { }
	}
	public abstract class SetLabelPositionAtAngleCommandBase : SetSeriesLabelPositionCommand {
		public SetLabelPositionAtAngleCommandBase(WpfChartModel chartModel, string caption, string description, string glyph)
			: base(chartModel, new Type[] { typeof(MarkerSeries2D), typeof(CircularSeries2D) }, caption, description, glyph) { }
	}
	public class SetMarker2DLabelPositionAtAngle0 : SetLabelPositionAtAngleCommandBase {
		static string GetCaption() {
			return ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.SeriesOptions_Labels_0DegreesCaption);
		}
		static string GetDescription() {
			return ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.SeriesOptions_Labels_0DegreesDescription);
		}
		static string GetGlyphPath() {
			return GlyphUtils.GalleryItemImages + @"SeriesLabelPosition\Marker2DLabelPosition_0";
		}
		protected override ComplexLabelPosition LabelPosition {
			get { return ComplexLabelPosition.Angle0; }
		}
		public SetMarker2DLabelPositionAtAngle0(WpfChartModel chartModel)
			: base(chartModel, GetCaption(), GetDescription(), GetGlyphPath()) { }
	}
	public class SetLabelPositionAtAngle45 : SetLabelPositionAtAngleCommandBase {
		static string GetCaption() {
			return ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.SeriesOptions_Labels_45DegreesCaption);
		}
		static string GetDescription() {
			return ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.SeriesOptions_Labels_45DegreesDescription);
		}
		static string GetGlyphPath() {
			return GlyphUtils.GalleryItemImages + @"SeriesLabelPosition\Marker2DLabelPosition_45";
		}
		protected override ComplexLabelPosition LabelPosition {
			get { return ComplexLabelPosition.Angle45; }
		}
		public SetLabelPositionAtAngle45(WpfChartModel chartModel)
			: base(chartModel, GetCaption(), GetDescription(), GetGlyphPath()) { }
	}
	public class SetLabelPositionAtAngle90 : SetLabelPositionAtAngleCommandBase {
		static string GetCaption() {
			return ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.SeriesOptions_Labels_90DegreesCaption);
		}
		static string GetDescription() {
			return ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.SeriesOptions_Labels_90DegreesDescription);
		}
		static string GetGlyphPath() {
			return GlyphUtils.GalleryItemImages + @"SeriesLabelPosition\Marker2DLabelPosition_90";
		}
		protected override ComplexLabelPosition LabelPosition {
			get { return ComplexLabelPosition.Angle90; }
		}
		public SetLabelPositionAtAngle90(WpfChartModel chartModel)
			: base(chartModel, GetCaption(), GetDescription(), GetGlyphPath()) { }
	}
	public class SetLabelPositionAtAngle135 : SetLabelPositionAtAngleCommandBase {
		static string GetCaption() {
			return ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.SeriesOptions_Labels_135DegreesCaption);
		}
		static string GetDescription() {
			return ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.SeriesOptions_Labels_135DegreesDescription);
		}
		static string GetGlyphPath() {
			return GlyphUtils.GalleryItemImages + @"SeriesLabelPosition\Marker2DLabelPosition_135";
		}
		protected override ComplexLabelPosition LabelPosition {
			get { return ComplexLabelPosition.Angle135; }
		}
		public SetLabelPositionAtAngle135(WpfChartModel chartModel)
			: base(chartModel, GetCaption(), GetDescription(), GetGlyphPath()) { }
	}
	public class SetLabelPositionAtAngle180 : SetLabelPositionAtAngleCommandBase {
		static string GetCaption() {
			return ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.SeriesOptions_Labels_180DegreesCaption);
		}
		static string GetDescription() {
			return ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.SeriesOptions_Labels_180DegreesDescription);
		}
		static string GetGlyphPath() {
			return GlyphUtils.GalleryItemImages + @"SeriesLabelPosition\Marker2DLabelPosition_180";
		}
		protected override ComplexLabelPosition LabelPosition {
			get { return ComplexLabelPosition.Angle180; }
		}
		public SetLabelPositionAtAngle180(WpfChartModel chartModel)
			: base(chartModel, GetCaption(), GetDescription(), GetGlyphPath()) { }
	}
	public class SetLabelPositionAtAngle225 : SetLabelPositionAtAngleCommandBase {
		static string GetCaption() {
			return ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.SeriesOptions_Labels_225DegreesCaption);
		}
		static string GetDescription() {
			return ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.SeriesOptions_Labels_225DegreesDescription);
		}
		static string GetGlyphPath() {
			return GlyphUtils.GalleryItemImages + @"SeriesLabelPosition\Marker2DLabelPosition_225";
		}
		protected override ComplexLabelPosition LabelPosition {
			get { return ComplexLabelPosition.Angle225; }
		}
		public SetLabelPositionAtAngle225(WpfChartModel chartModel)
			: base(chartModel, GetCaption(), GetDescription(), GetGlyphPath()) { }
	}
	public class SetPositionAtAngle270 : SetLabelPositionAtAngleCommandBase {
		static string GetCaption() {
			return ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.SeriesOptions_Labels_270DegreesCaption);
		}
		static string GetDescription() {
			return ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.SeriesOptions_Labels_270DegreesDescription);
		}
		static string GetGlyphPath() {
			return GlyphUtils.GalleryItemImages + @"SeriesLabelPosition\Marker2DLabelPosition_270";
		}
		protected override ComplexLabelPosition LabelPosition {
			get { return ComplexLabelPosition.Angle270; }
		}
		public SetPositionAtAngle270(WpfChartModel chartModel)
			: base(chartModel, GetCaption(), GetDescription(), GetGlyphPath()) { }
	}
	public class SetLabelPositionAtAngle315 : SetLabelPositionAtAngleCommandBase {
		static string GetCaption() {
			return ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.SeriesOptions_Labels_315DegreesCaption);
		}
		static string GetDescription() {
			return ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.SeriesOptions_Labels_315DegreesDescription);
		}
		static string GetGlyphPath() {
			return GlyphUtils.GalleryItemImages + @"SeriesLabelPosition\Marker2DLabelPosition_315";
		}
		protected override ComplexLabelPosition LabelPosition {
			get { return ComplexLabelPosition.Angle315; }
		}
		public SetLabelPositionAtAngle315(WpfChartModel chartModel)
			: base(chartModel, GetCaption(), GetDescription(), GetGlyphPath()) { }
	}
	public abstract class SetBar2DLabelPositionCommand : SetSeriesLabelPositionCommand {
		public SetBar2DLabelPositionCommand(WpfChartModel chartModel, string caption, string description, string glyph)
			: base(chartModel, typeof(BarSideBySideSeries2D), caption, description, glyph) { }
	}
	public class SetBar2DOutsideLabelPositionCommand : SetBar2DLabelPositionCommand {
		static string GetCaption() {
			return ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.SeriesOptions_Labels_OutsideCaption);
		}
		static string GetDescription() {
			return ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.SeriesOptions_Labels_OutsideDescription);
		}
		static string GetGlyphPath() {
			return GlyphUtils.GalleryItemImages + @"SeriesLabelPosition\Bar2DLabelPositionOutside";
		}
		protected override ComplexLabelPosition LabelPosition {
			get { return ComplexLabelPosition.Bar2DOutside; }
		}
		public SetBar2DOutsideLabelPositionCommand(WpfChartModel chartModel)
			: base(chartModel, GetCaption(), GetDescription(), GetGlyphPath()) { }
	}
	public class SetBar2DCenterLabelPositionCommand : SetBar2DLabelPositionCommand {
		static string GetCaption() {
			return ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.SeriesOptions_Labels_CenteredInBarsCaption);
		}
		static string GetDescription() {
			return ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.SeriesOptions_Labels_CenteredInBarsDescription);
		}
		static string GetGlyphPath() {
			return GlyphUtils.GalleryItemImages + @"SeriesLabelPosition\Bar2DLabelPositionCenter";
		}
		protected override ComplexLabelPosition LabelPosition {
			get { return ComplexLabelPosition.Bar2DCenter; }
		}
		public SetBar2DCenterLabelPositionCommand(WpfChartModel chartModel)
			: base(chartModel, GetCaption(), GetDescription(), GetGlyphPath()) { }
	}
	public abstract class SetFunnelLabelPositionCommand : SetSeriesLabelPositionCommand {
		public SetFunnelLabelPositionCommand(WpfChartModel chartModel, string caption, string description, string glyph)
			: base(chartModel, new Type[] { typeof(FunnelSeries2D) }, caption, description, glyph) { }
	}
	public class SetFunnelLeftLabelPositionCommand : SetFunnelLabelPositionCommand {
		static string GetCaption() {
			return ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.SeriesOptions_Labels_FunnelLeftPointCaption);
		}
		static string GetDescription() {
			return ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.SeriesOptions_Labels_FunnelLeftPointDescription);
		}
		static string GetGlyphPath() {
			return GlyphUtils.GalleryItemImages + @"SeriesLabelPosition\FunnelLabelPositionLeft";
		}
		protected override ComplexLabelPosition LabelPosition {
			get { return ComplexLabelPosition.FunnelLeft; }
		}
		public SetFunnelLeftLabelPositionCommand(WpfChartModel chartModel)
			: base(chartModel, GetCaption(), GetDescription(), GetGlyphPath()) { }
	}
	public class SetFunnelLeftColumnLabelPositionCommand : SetFunnelLabelPositionCommand {
		static string GetCaption() {
			return ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.SeriesOptions_Labels_FunnelLeftColumnPointCaption);
		}
		static string GetDescription() {
			return ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.SeriesOptions_Labels_FunnelLeftColumnPointDescription);
		}
		static string GetGlyphPath() {
			return GlyphUtils.GalleryItemImages + @"SeriesLabelPosition\FunnelLabelPositionLeftColumn";
		}
		protected override ComplexLabelPosition LabelPosition {
			get { return ComplexLabelPosition.FunnelLeftColumn; }
		}
		public SetFunnelLeftColumnLabelPositionCommand(WpfChartModel chartModel)
			: base(chartModel, GetCaption(), GetDescription(), GetGlyphPath()) { }
	}
	public class SetFunnelRightLabelPositionCommand : SetFunnelLabelPositionCommand {
		static string GetCaption() {
			return ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.SeriesOptions_Labels_FunnelRightPointCaption);
		}
		static string GetDescription() {
			return ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.SeriesOptions_Labels_FunnelRightPointDescription);
		}
		static string GetGlyphPath() {
			return GlyphUtils.GalleryItemImages + @"SeriesLabelPosition\FunnelLabelPositionRight";
		}
		protected override ComplexLabelPosition LabelPosition {
			get { return ComplexLabelPosition.FunnelRight; }
		}
		public SetFunnelRightLabelPositionCommand(WpfChartModel chartModel)
			: base(chartModel, GetCaption(), GetDescription(), GetGlyphPath()) { }
	}
	public class SetFunnelRightColumnLabelPositionCommand : SetFunnelLabelPositionCommand {
		static string GetCaption() {
			return ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.SeriesOptions_Labels_FunnelRightColumnPointCaption);
		}
		static string GetDescription() {
			return ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.SeriesOptions_Labels_FunnelRightColumnPointDescription);
		}
		static string GetGlyphPath() {
			return GlyphUtils.GalleryItemImages + @"SeriesLabelPosition\FunnelLabelPositionRightColumn";
		}
		protected override ComplexLabelPosition LabelPosition {
			get { return ComplexLabelPosition.FunnelRightColumn; }
		}
		public SetFunnelRightColumnLabelPositionCommand(WpfChartModel chartModel)
			: base(chartModel, GetCaption(), GetDescription(), GetGlyphPath()) { }
	}
	public class SetFunnelCenterLabelPositionCommand : SetFunnelLabelPositionCommand {
		static string GetCaption() {
			return ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.SeriesOptions_Labels_FunnelCenterPointCaption);
		}
		static string GetDescription() {
			return ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.SeriesOptions_Labels_FunnelCenterPointDescription);
		}
		static string GetGlyphPath() {
			return GlyphUtils.GalleryItemImages + @"SeriesLabelPosition\FunnelLabelPositionCenter";
		}
		protected override ComplexLabelPosition LabelPosition {
			get { return ComplexLabelPosition.FunnelCenter; }
		}
		public SetFunnelCenterLabelPositionCommand(WpfChartModel chartModel)
			: base(chartModel, GetCaption(), GetDescription(), GetGlyphPath()) { }
	}
	public abstract class SetPieLabelPositionCommand : SetSeriesLabelPositionCommand {
		public SetPieLabelPositionCommand(WpfChartModel chartModel, string caption, string description, string glyph)
			: base(chartModel, new Type[] { typeof(PieSeries) }, caption, description, glyph) { }
	}
	public class SetPieInsideLabelPositionCommand : SetPieLabelPositionCommand {
		static string GetCaption() {
			return ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.SeriesOptions_Labels_InsideSlicesCaption);
		}
		static string GetDescription() {
			return ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.SeriesOptions_Labels_InsideSlicesDescription);
		}
		static string GetGlyphPath() {
			return GlyphUtils.GalleryItemImages + @"SeriesLabelPosition\PieLabelPositionInside";
		}
		protected override ComplexLabelPosition LabelPosition {
			get { return ComplexLabelPosition.PieInside; }
		}
		public SetPieInsideLabelPositionCommand(WpfChartModel chartModel)
			: base(chartModel, GetCaption(), GetDescription(), GetGlyphPath()) { }
	}
	public class SetPieOutsideLabelPositionCommand : SetPieLabelPositionCommand {
		static string GetCaption() {
			return ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.SeriesOptions_Labels_OutsideSlicesCaption);
		}
		static string GetDescription() {
			return ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.SeriesOptions_Labels_OutsideSlicesDescription);
		}
		static string GetGlyphPath() {
			return GlyphUtils.GalleryItemImages + @"SeriesLabelPosition\PieLabelPositionOutside";
		}
		protected override ComplexLabelPosition LabelPosition {
			get { return ComplexLabelPosition.PieOutside; }
		}
		public SetPieOutsideLabelPositionCommand(WpfChartModel chartModel)
			: base(chartModel, GetCaption(), GetDescription(), GetGlyphPath()) { }
		protected override bool CanExecute(object parameter) {
			if (!base.CanExecute(parameter))
				return false;
			if ((SeriesModel.Series is NestedDonutSeries2D) && ChartDesignerPropertiesProvider.IsOuter((NestedDonutSeries2D)SeriesModel.Series).HasValue && ChartDesignerPropertiesProvider.IsOuter((NestedDonutSeries2D)SeriesModel.Series).Value == false)
				return false;
			return true;
		}
	}
	public class SetPieTwoColumnsLabelPositionCommand : SetPieLabelPositionCommand {
		static string GetCaption() {
			return ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.SeriesOptions_Labels_TwoColumnsCaption);
		}
		static string GetDescription() {
			return ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.SeriesOptions_Labels_TwoColumnsDescription);
		}
		static string GetGlyphPath() {
			return GlyphUtils.GalleryItemImages + @"SeriesLabelPosition\PieLabelPositionTwoColumns";
		}
		protected override ComplexLabelPosition LabelPosition {
			get { return ComplexLabelPosition.PieTwoColumns; }
		}
		public SetPieTwoColumnsLabelPositionCommand(WpfChartModel chartModel)
			: base(chartModel, GetCaption(), GetDescription(), GetGlyphPath()) { }
		protected override bool CanExecute(object parameter) {
			if (!base.CanExecute(parameter))
				return false;
			if ((SeriesModel.Series is NestedDonutSeries2D) && ChartDesignerPropertiesProvider.IsOuter((NestedDonutSeries2D)SeriesModel.Series).HasValue && ChartDesignerPropertiesProvider.IsOuter((NestedDonutSeries2D)SeriesModel.Series).Value == false)
				return false;
			return true;
		}
	}
	public abstract class SetBubble2DLabelPositionCommand : SetSeriesLabelPositionCommand {
		public SetBubble2DLabelPositionCommand(WpfChartModel chartModel, string caption, string description, string glyph)
			: base(chartModel, typeof(BubbleSeries2D), caption, description, glyph) { }
	}
	public class SetBubble2DCenterLabelPosition : SetBubble2DLabelPositionCommand {
		static string GetCaption() {
			return ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.SeriesOptions_Labels_Bubble2DCenterCaption);
		}
		static string GetDescription() {
			return ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.SeriesOptions_Labels_Bubble2DCenterDescription);
		}
		static string GetGlyphPath() {
			return GlyphUtils.GalleryItemImages + @"SeriesLabelPosition\Bubble2DLabelPositionCenter";
		}
		protected override ComplexLabelPosition LabelPosition {
			get { return ComplexLabelPosition.Bubble2DCenter; }
		}
		public SetBubble2DCenterLabelPosition(WpfChartModel chartModel)
			: base(chartModel, GetCaption(), GetDescription(), GetGlyphPath()) { }
	}
	public abstract class SetMarker3DLabelPositionCommand : SetSeriesLabelPositionCommand {
		public SetMarker3DLabelPositionCommand(WpfChartModel chartModel, string caption, string description, string glyph)
			: base(chartModel, typeof(MarkerSeries3D), caption, description, glyph) { }
	}
	public class SetMarker3DCenterLabelPosition : SetMarker3DLabelPositionCommand {
		static string GetCaption() {
			return ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.SeriesOptions_Labels_Marker3DCenterCaption);
		}
		static string GetDescription() {
			return ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.SeriesOptions_Labels_Marker3DCenterDescription);
		}
		static string GetGlyphPath() {
			return GlyphUtils.GalleryItemImages + @"SeriesLabelPosition\Marker3DPositionCenter";
		}
		protected override ComplexLabelPosition LabelPosition {
			get { return ComplexLabelPosition.Marker3DCenter; }
		}
		public SetMarker3DCenterLabelPosition(WpfChartModel chartModel)
			: base(chartModel, GetCaption(), GetDescription(), GetGlyphPath()) { }
	}
	public class SetMarker3DTopLabelPosition : SetMarker3DLabelPositionCommand {
		static string GetCaption() {
			return ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.SeriesOptions_Labels_3DTopCaption);
		}
		static string GetDescription() {
			return ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.SeriesOptions_Labels_3DTopDescription);
		}
		static string GetGlyphPath() {
			return GlyphUtils.GalleryItemImages + @"SeriesLabelPosition\Marker3DPositionTop";
		}
		protected override ComplexLabelPosition LabelPosition {
			get { return ComplexLabelPosition.Marker3DTop; }
		}
		public SetMarker3DTopLabelPosition(WpfChartModel chartModel)
			: base(chartModel, GetCaption(), GetDescription(), GetGlyphPath()) { }
	}
	public abstract class SetStackedBar2DLabelPositionCommand : SetSeriesLabelPositionCommand {
		public SetStackedBar2DLabelPositionCommand(WpfChartModel chartModel, string caption, string description, string glyph)
			: base(chartModel, typeof(BarStackedSeries2D), caption, description, glyph) { }
	}
	public class SetStackedBar2DCenterLabelPosition : SetStackedBar2DLabelPositionCommand {
		static string GetCaption() {
			return ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.SeriesOptions_Labels_StackedBarCenteredInBarCaption);
		}
		static string GetDescription() {
			return ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.SeriesOptions_Labels_StackedBarCenteredInBarDescription);
		}
		static string GetGlyphPath() {
			return GlyphUtils.GalleryItemImages + @"SeriesLabelPosition\Bar2DLabelPositionCenter";
		}
		protected override ComplexLabelPosition LabelPosition {
			get { return ComplexLabelPosition.Bar2DCenter; }
		}
		public SetStackedBar2DCenterLabelPosition(WpfChartModel chartModel)
			: base(chartModel, GetCaption(), GetDescription(), GetGlyphPath()) { }
	}
	public class SetRangeArea2DMaxValueLabelPositionCommand : SetSeriesLabelPositionCommand {
		static string GetCaption() {
			return ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.SeriesOptions_RangeArea2DMaxValueLabelPositionCaption);
		}
		static string GetDescription() {
			return ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.SeriesOptions_RangeArea2DMaxValueLabelPositionDescription);
		}
		static string GetGlyph() {
			return GlyphUtils.GalleryItemImages + @"SeriesLabelPosition\RangeArea2DMaxValueLabelPosition";
		}
		protected override ComplexLabelPosition LabelPosition {
			get { return ComplexLabelPosition.RangeAreaMaxValueLabel; }
		}
		public SetRangeArea2DMaxValueLabelPositionCommand(WpfChartModel chartModel)
			: base(chartModel, typeof(RangeAreaSeries2D), GetCaption(), GetDescription(), GetGlyph()) { }
	}
	public class SetRangeArea2DMinValueLabelPositionCommand : SetSeriesLabelPositionCommand {
		static string GetCaption() {
			return ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.SeriesOptions_RangeArea2DMinValueLabelPositionCaption);
		}
		static string GetDescription() {
			return ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.SeriesOptions_RangeArea2DMinValueLabelPositionDescription);
		}
		static string GetGlyph() {
			return GlyphUtils.GalleryItemImages + @"SeriesLabelPosition\RangeArea2DMinValueLabelPosition";
		}
		protected override ComplexLabelPosition LabelPosition {
			get { return ComplexLabelPosition.RangeAreaMinValueLabel; }
		}
		public SetRangeArea2DMinValueLabelPositionCommand(WpfChartModel chartModel)
			: base(chartModel, typeof(RangeAreaSeries2D), GetCaption(), GetDescription(), GetGlyph()) { }
	}
	public class SetRangeArea2DOneLabelPositionCommand : SetSeriesLabelPositionCommand {
		static string GetCaption() {
			return ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.SeriesOptions_RangeArea2DOneLabelPositionCaption);
		}
		static string GetDescription() {
			return ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.SeriesOptions_RangeArea2DOneLabelPositionDescription);
		}
		static string GetGlyph() {
			return GlyphUtils.GalleryItemImages + @"SeriesLabelPosition\RangeArea2DOneLabelPosition";
		}
		protected override ComplexLabelPosition LabelPosition {
			get { return ComplexLabelPosition.RangeAreaOneLabel; }
		}
		public SetRangeArea2DOneLabelPositionCommand(WpfChartModel chartModel)
			: base(chartModel, typeof(RangeAreaSeries2D), GetCaption(), GetDescription(), GetGlyph()) { }
	}
	public class SetRangeArea2DTwoLabelsPositionCommand : SetSeriesLabelPositionCommand {
		static string GetCaption() {
			return ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.SeriesOptions_RangeArea2DTwoLabelsPositionCaption);
		}
		static string GetDescription() {
			return ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.SeriesOptions_RangeArea2DTwoLabelsPositionDescription);
		}
		static string GetGlyph() {
			return GlyphUtils.GalleryItemImages + @"SeriesLabelPosition\RangeArea2DTwoLabelsPosition";
		}
		protected override ComplexLabelPosition LabelPosition {
			get { return ComplexLabelPosition.RangeAreaTwoLabels; }
		}
		public SetRangeArea2DTwoLabelsPositionCommand(WpfChartModel chartModel)
			: base(chartModel, typeof(RangeAreaSeries2D), GetCaption(), GetDescription(), GetGlyph()) { }
	}
	public class SetRangeArea2DValue1LabelPositionCommand : SetSeriesLabelPositionCommand {
		static string GetCaption() {
			return ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.SeriesOptions_RangeArea2DValue1LabelPositionCaption);
		}
		static string GetDescription() {
			return ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.SeriesOptions_RangeArea2DValue1LabelPositionDescription);
		}
		static string GetGlyph() {
			return GlyphUtils.GalleryItemImages + @"SeriesLabelPosition\RangeArea2DValue1LabelPosition";
		}
		protected override ComplexLabelPosition LabelPosition {
			get { return ComplexLabelPosition.RangeAreaValue1Label; }
		}
		public SetRangeArea2DValue1LabelPositionCommand(WpfChartModel chartModel)
			: base(chartModel, typeof(RangeAreaSeries2D), GetCaption(), GetDescription(), GetGlyph()) { }
	}
	public class SetRangeArea2DValue2LabelPositionCommand : SetSeriesLabelPositionCommand {
		static string GetCaption() {
			return ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.SeriesOptions_RangeArea2DValue2LabelPositionCaption);
		}
		static string GetDescription() {
			return ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.SeriesOptions_RangeArea2DValue2LabelPositionDescription);
		}
		static string GetGlyph() {
			return GlyphUtils.GalleryItemImages + @"SeriesLabelPosition\RangeArea2DValue2LabelPosition";
		}
		protected override ComplexLabelPosition LabelPosition {
			get { return ComplexLabelPosition.RangeAreaValue2Label; }
		}
		public SetRangeArea2DValue2LabelPositionCommand(WpfChartModel chartModel)
			: base(chartModel, typeof(RangeAreaSeries2D), GetCaption(), GetDescription(), GetGlyph()) { }
	}
	public class SetRangeBarMaxValueLabelPositionCommand : SetSeriesLabelPositionCommand {
		static string GetCaption() {
			return ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.SeriesOptions_RangeBarMaxValueLabelPositionCaption);
		}
		static string GetDescription() {
			return ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.SeriesOptions_RangeBarMaxValueLabelPositionDescription);
		}
		static string GetGlyph() {
			return GlyphUtils.GalleryItemImages + @"SeriesLabelPosition\RangeBarMaxValueLabelPosition";
		}
		protected override ComplexLabelPosition LabelPosition {
			get { return ComplexLabelPosition.RangeBarMaxValueLabel; }
		}
		public SetRangeBarMaxValueLabelPositionCommand(WpfChartModel chartModel)
			: base(chartModel, typeof(RangeBarSeries2D), GetCaption(), GetDescription(), GetGlyph()) { }
	}
	public class SetRangeBarMinValueLabelPositionCommand : SetSeriesLabelPositionCommand {
		static string GetCaption() {
			return ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.SeriesOptions_RangeBarMinValueLabelPositionCaption);
		}
		static string GetDescription() {
			return ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.SeriesOptions_RangeBarMinValueLabelPositionDescription);
		}
		static string GetGlyph() {
			return GlyphUtils.GalleryItemImages + @"SeriesLabelPosition\RangeBarMinValueLabelPosition";
		}
		protected override ComplexLabelPosition LabelPosition {
			get { return ComplexLabelPosition.RangeBarMinValueLabel; }
		}
		public SetRangeBarMinValueLabelPositionCommand(WpfChartModel chartModel)
			: base(chartModel, typeof(RangeBarSeries2D), GetCaption(), GetDescription(), GetGlyph()) { }
	}
	public class SetRangeBarOneLabelPositionCommand : SetSeriesLabelPositionCommand {
		static string GetCaption() {
			return ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.SeriesOptions_RangeBarOneLabelPositionCaption);
		}
		static string GetDescription() {
			return ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.SeriesOptions_RangeBarOneLabelPositionDescription);
		}
		static string GetGlyph() {
			return GlyphUtils.GalleryItemImages + @"SeriesLabelPosition\RangeBarOneLabelPosition";
		}
		protected override ComplexLabelPosition LabelPosition {
			get { return ComplexLabelPosition.RangeBarOneLabel; }
		}
		public SetRangeBarOneLabelPositionCommand(WpfChartModel chartModel)
			: base(chartModel, typeof(RangeBarSeries2D), GetCaption(), GetDescription(), GetGlyph()) { }
	}
	public class SetRangeBarTwoLabelsLabelPositionCommand : SetSeriesLabelPositionCommand {
		static string GetCaption() {
			return ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.SeriesOptions_RangeBarTwoLabelsLabelPositionCaption);
		}
		static string GetDescription() {
			return ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.SeriesOptions_RangeBarTwoLabelsLabelPositionDescription);
		}
		static string GetGlyph() {
			return GlyphUtils.GalleryItemImages + @"SeriesLabelPosition\RangeBarTwoLabelsLabelPosition";
		}
		protected override ComplexLabelPosition LabelPosition {
			get { return ComplexLabelPosition.RangeBarTwoLabels; }
		}
		public SetRangeBarTwoLabelsLabelPositionCommand(WpfChartModel chartModel)
			: base(chartModel, typeof(RangeBarSeries2D), GetCaption(), GetDescription(), GetGlyph()) { }
	}
}
