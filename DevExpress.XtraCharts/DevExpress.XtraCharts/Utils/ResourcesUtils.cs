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
using System.Drawing;
using System.Reflection;
using DevExpress.Charts.Native;
using DevExpress.Utils;
using DevExpress.XtraCharts.Localization;
namespace DevExpress.XtraCharts.Native {
	public static class ImageResourcesUtils {
		public static string ImagePath { get { return "DevExpress.XtraCharts.Images."; } }
		public static string WizardImagePath { get { return "DevExpress.XtraCharts.Design.Wizard.Images."; } }
		public static string DesignerImagePath { get { return "DevExpress.XtraCharts.Wizard.ChartDesigner.Images."; } }
		public static string DesignerViewImagePath { get { return DesignerImagePath + "SeriesView."; } }
		const string largeViewImagePostfix = "_32x32";
		const string smallViewImagePostfix = "_16x16";
		public static Image GetImageFromResources(SeriesViewBase view, SeriesViewImageType imageType) {
			try {
				Assembly asm = Assembly.GetExecutingAssembly();
				switch (imageType) {
					case SeriesViewImageType.Image:
						return ResourceImageHelper.CreateBitmapFromResources(ImagePath + view.ImageNameBase + ".png", asm);
					case SeriesViewImageType.SmallImage:
						return ResourceImageHelper.CreateBitmapFromResources(ImagePath + view.ImageNameBase + "_s.png", asm);
					case SeriesViewImageType.WizardImage:
						return ResourceImageHelper.CreateBitmapFromResources(WizardImagePath + view.ImageNameBase + ".png", asm);
					default:
						throw new DefaultSwitchException();
				}
			}
			catch {
				return ResourceImageHelper.CreateBitmapFromResources(WizardImagePath + "empty.png", Assembly.GetExecutingAssembly());
			}
		}
		public static Image GetImageFromResources(AnnotationShapePosition position) {
			Type positionType = position.GetType();
			Assembly asm = Assembly.GetExecutingAssembly();
			if (positionType == typeof(RelativePosition))
				return ResourceImageHelper.CreateImageFromResources(ImagePath + "AnnotationRelativePosition.png", asm);
			if (positionType == typeof(FreePosition))
				return ResourceImageHelper.CreateImageFromResources(ImagePath + "AnnotationFreePosition.png", asm);
			return null;
		}
		public static Image GetImageFromResources(AnnotationAnchorPoint point, bool isEnabledImage) {
			Type pointType = point.GetType();
			Assembly asm = Assembly.GetExecutingAssembly();
			if (isEnabledImage) {
				if (pointType == typeof(ChartAnchorPoint))
					return ResourceImageHelper.CreateImageFromResources(ImagePath + "AnnotationChartAnchorPoint.png", asm);
				if (pointType == typeof(SeriesPointAnchorPoint))
					return ResourceImageHelper.CreateImageFromResources(ImagePath + "AnnotationSeriesPointAnchorPoint.png", asm);
				if (pointType == typeof(PaneAnchorPoint))
					return ResourceImageHelper.CreateImageFromResources(ImagePath + "AnnotationPaneAnchorPoint.png", asm);
			}
			else {
				if (pointType == typeof(ChartAnchorPoint))
					return ResourceImageHelper.CreateImageFromResources(ImagePath + "AnnotationChartAnchorPoint.png", asm);
				if (pointType == typeof(SeriesPointAnchorPoint))
					return ResourceImageHelper.CreateImageFromResources(ImagePath + "AnnotationSeriesPointAnchorPoint_Disabled.png", asm);
				if (pointType == typeof(PaneAnchorPoint))
					return ResourceImageHelper.CreateImageFromResources(ImagePath + "AnnotationPaneAnchorPoint_Disabled.png", asm);
			}
			return null;
		}
		public static Image GetImageFromResources(CrosshairLabelPosition position) {
			Type positionType = position.GetType();
			Assembly asm = Assembly.GetExecutingAssembly();
			if (positionType == typeof(CrosshairMousePosition))
				return ResourceImageHelper.CreateImageFromResources(ImagePath + "crosshairMousePosition.png", asm);
			if (positionType == typeof(CrosshairFreePosition))
				return ResourceImageHelper.CreateImageFromResources(ImagePath + "crosshairFreePosition.png", asm);
			return null;
		}
		public static Image GetImageFromResources(Indicator indicator, bool isSmallImage) {
			Assembly asm = Assembly.GetExecutingAssembly();
			FibonacciIndicator fibonacciIndicator = indicator as FibonacciIndicator;
			string imageName = fibonacciIndicator != null ? fibonacciIndicator.Kind.ToString() : indicator.GetType().Name;
			imageName = imageName.ToLower();
			imageName += isSmallImage ? "_s.png" : ".png";
			return ResourceImageHelper.CreateBitmapFromResources(ImagePath + imageName, asm);
		}
		public static Image GetImageFromResources(AnnotationType annotationType) {
			Assembly asm = Assembly.GetExecutingAssembly();
			string imageName;
			switch (annotationType) {
				case AnnotationType.Image:
					imageName = "ImageAnnotation.png";
					break;
				case AnnotationType.Text:
					imageName = "TextAnnotation.png";
					break;
				default:
					ChartDebug.Fail("Unknown annotation type: " + annotationType);
					return null;
			}
			return ResourceImageHelper.CreateBitmapFromResources(ImagePath + imageName, asm);
		}
		public static Image GetImageFromDesignerResources(AnnotationType annotationType) {
			Assembly asm = Assembly.GetCallingAssembly();
			string imageName;
			switch (annotationType) {
				case AnnotationType.Image:
					imageName = "ImageAnnotation.png";
					break;
				case AnnotationType.Text:
					imageName = "TextAnnotation.png";
					break;
				default:
					ChartDebug.Fail("Unknown annotation type: " + annotationType);
					return null;
			}
			return ResourceImageHelper.CreateBitmapFromResources(DesignerImagePath + imageName, asm);
		}
		public static Image GetImageFromResources(ToolTipPosition position) {
			Type positionType = position.GetType();
			Assembly asm = Assembly.GetExecutingAssembly();
			if (positionType == typeof(ToolTipMousePosition))
				return ResourceImageHelper.CreateImageFromResources(ImagePath + "toolTipMousePosition.png", asm);
			if (positionType == typeof(ToolTipFreePosition))
				return ResourceImageHelper.CreateImageFromResources(ImagePath + "toolTipFreePosition.png", asm);
			if (positionType == typeof(ToolTipRelativePosition))
				return ResourceImageHelper.CreateImageFromResources(ImagePath + "toolTipRelativePosition.png", asm);
			return null;
		}
		public static Image GetImageFromResources(ViewType viewType, bool isSmallImage) {
			try {
				Assembly asm = Assembly.GetCallingAssembly();
				string imagePosfix = isSmallImage ? smallViewImagePostfix : largeViewImagePostfix;
				return ResourceImageHelper.CreateBitmapFromResources(DesignerViewImagePath + viewType.ToString() + imagePosfix + ".png", asm);
			}
			catch {
				return ResourceImageHelper.CreateBitmapFromResources(WizardImagePath + "empty.png", Assembly.GetExecutingAssembly());
			}
		}
	}
	public static class StringResourcesUtils {
		public static string GetStringId(SeriesViewBase view) {
			return view.StringId;
		}
		public static string GetStringId(AnnotationShapePosition position) {
			Type positionType = position.GetType();
			if (positionType == typeof(RelativePosition))
				return ChartLocalizer.GetString(ChartStringId.AnnotationRelativePosition);
			if (positionType == typeof(FreePosition))
				return ChartLocalizer.GetString(ChartStringId.AnnotationFreePosition);
			return string.Empty;
		}
		public static string GetStringId(AnnotationAnchorPoint point) {
			Type pointType = point.GetType();
			if (pointType == typeof(ChartAnchorPoint))
				return ChartLocalizer.GetString(ChartStringId.AnnotationChartAnchorPoint);
			if (pointType == typeof(SeriesPointAnchorPoint))
				return ChartLocalizer.GetString(ChartStringId.AnnotationSeriesPointAnchorPoint);
			if (pointType == typeof(PaneAnchorPoint))
				return ChartLocalizer.GetString(ChartStringId.AnnotationPaneAnchorPoint);
			return string.Empty;
		}
		public static string GetStringId(CrosshairLabelPosition position) {
			Type positionType = position.GetType();
			if (positionType == typeof(CrosshairMousePosition))
				return ChartLocalizer.GetString(ChartStringId.CrosshairLabelMousePosition);
			if (positionType == typeof(CrosshairFreePosition))
				return ChartLocalizer.GetString(ChartStringId.CrosshairLabelFreePosition);
			return string.Empty;
		}
		public static string GetStringId(ToolTipPosition position) {
			Type positionType = position.GetType();
			if (positionType == typeof(ToolTipMousePosition))
				return ChartLocalizer.GetString(ChartStringId.ToolTipMousePosition);
			if (positionType == typeof(ToolTipFreePosition))
				return ChartLocalizer.GetString(ChartStringId.ToolTipFreePosition);
			if (positionType == typeof(ToolTipRelativePosition))
				return ChartLocalizer.GetString(ChartStringId.ToolTipRelativePosition);
			return string.Empty;
		}
		public static string GetStringId(FibonacciIndicator indicator) {
			return GetStringId(indicator.IndicatorBehavior);
		}
		public static string GetStringId(IndicatorBehavior behavior) {
			Type behaviorType = behavior.GetType();
			if (behaviorType == typeof(FibonacciFansBehavior))
				return ChartLocalizer.GetString(ChartStringId.FibonacciFans);
			if (behaviorType == typeof(FibonacciRetracementBehavior))
				return ChartLocalizer.GetString(ChartStringId.FibonacciRetracement);
			if (behaviorType == typeof(FibonacciArcsBehavior))
				return ChartLocalizer.GetString(ChartStringId.FibonacciArcs);
			return string.Empty;
		}
	}
}
