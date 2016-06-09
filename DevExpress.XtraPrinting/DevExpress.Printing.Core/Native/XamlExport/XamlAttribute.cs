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

namespace DevExpress.XtraPrinting.XamlExport {
	public static class XamlAttribute {
		public static readonly string CanvasLeft = "Canvas.Left";
		public static readonly string CanvasTop = "Canvas.Top";
		public static readonly string Rect = "Rect";
		public static readonly string Text = "Text";
		public static readonly string FontFamily = "FontFamily";
		public static readonly string FontSize = "FontSize";
		public static readonly string Foreground = "Foreground";
		public static readonly string TextAlignment = "TextAlignment";
		public static readonly string VerticalAlignment = "VerticalAlignment";
		public static readonly string TextWrapping = "TextWrapping";
		public static readonly string TextTrimming = "TextTrimming";
		public static readonly string Margin = "Margin";
		public static readonly string Background = "Background";
		public static readonly string Width = "Width";
		public static readonly string Height = "Height";
		public static readonly string BorderThickness = "BorderThickness";
		public static readonly string BorderBrush = "BorderBrush";
		public static readonly string X1 = "X1";
		public static readonly string X2 = "X2";
		public static readonly string Y1 = "Y1";
		public static readonly string Y2 = "Y2";
		public static readonly string Stroke = "Stroke";
		public static readonly string StrokeThickness = "StrokeThickness";
		public static readonly string StrokeDashArray = "StrokeDashArray";
		public static readonly string Padding = "Padding";
		public static readonly string FontWeight = "FontWeight";
		public static readonly string TextDecorations = "TextDecorations";
		public static readonly string FontStyle = "FontStyle";
		public static readonly string IsChecked = "IsChecked";
		public static readonly string IsHitTestVisible = "IsHitTestVisible";
		public static readonly string IsTabStop = "IsTabStop";
		public static readonly string Style = "Style";
		public static readonly string Property = "Property";
		public static readonly string Value = "Value";
		public static readonly string Name = "Name";
		public static readonly string TargetType = "TargetType";
		public static readonly string UseLayoutRounding = "UseLayoutRounding";
		public static readonly string Source = "Source";
		public static readonly string Stretch = "Stretch";
		public static readonly string HorizontalAlignment = "HorizontalAlignment";
		public static readonly string CanvasZIndex = "Canvas.ZIndex";
		public static readonly string Key = "Key";
		public static readonly string ContentTemplate = "ContentTemplate";
		public static readonly string SnapsToDevicePixels = "SnapsToDevicePixels";
		public static readonly string Tag = "Tag";
		public static readonly string TileCount = "TileCount";
		public static readonly string MinWidth = "MinWidth";
		public static readonly string MinHeight = "MinHeight";
		public static readonly string MaxWidth = "MaxWidth";
		public static readonly string MaxHeight = "MaxHeight";
		public static readonly string Opacity = "Opacity";
		public static readonly string Angle = "Angle";
		public static readonly string RenderTransformOrigin = "RenderTransformOrigin";
		public static readonly string RenderOptionsBitmapScalingMode = "RenderOptions.BitmapScalingMode";
		public static readonly string StrokeStartLineCap = "StrokeStartLineCap";
		public static readonly string StrokeEndLineCap = "StrokeEndLineCap";
		public static readonly string BorderDashStyle = "BorderDashStyle";
		public static readonly string PreviewClickHelperTag = "PreviewClickHelper.Tag";
		public static readonly string PreviewClickHelperNavigationPair = "PreviewClickHelper.NavigationPair";
		public static readonly string PreviewClickHelperUrl = "PreviewClickHelper.Url";
		public static readonly string VisualHelperOffset = "VisualHelper.Offset";
		public static readonly string VisualHelperClipToBounds = "VisualHelper.ClipToBounds";
		public static readonly string VisualHelperIsVisualBrickBorder = "VisualHelper.IsVisualBrickBorder";
		public static readonly string FlowDirection = "FlowDirection";
#if !SILVERLIGHT
		public static readonly string DesignPropertiesName = "DesignProperties.Name";
		public static readonly string DesignPropertiesBrickOwnerType = "DesignProperties.BrickOwnerType";
		public static readonly string DesignPropertiesLeftMarginOffset = "DesignProperties.LeftMarginOffset";
		public static readonly string DesignPropertiesRightMarginOffset = "DesignProperties.RightMarginOffset";
		public static readonly string DesignPropertiesTopMarginOffset = "DesignProperties.TopMarginOffset";
		public static readonly string DesignPropertiesBottomMarginOffset = "DesignProperties.BottomMarginOffset";
		public static readonly string DesignPropertiesDetailPath = "DesignProperties.DetailPath";
		public static readonly string DesignPropertiesTextEditMode = "DesignProperties.TextEditMode";
		public static readonly string DesignPropertiesRealPageWidth = "DesignProperties.RealPageWidth";
		public static readonly string DesignPropertiesControlLayoutRules = "DesignProperties.ControlLayoutRules";
		public static readonly string DesignPropertiesText = "DesignProperties.Text";
		public static readonly string DesignPropertiesAllowDesign = "DesignProperties.AllowDesign";
		public static readonly string DesignPropertiesIsImageEditable = "DesignProperties.IsImageEditable";
		public static readonly string DesignPropertiesHasImage = "DesignProperties.HasImage";
		public static readonly string DesignPropertiesFontStyle = "DesignProperties.FontStyle";
		public static readonly string DesignPropertiesFontSize = "DesignProperties.FontSize";
		public static readonly string DesignPropertiesFontFamilyName = "DesignProperties.FontFamilyName";
		public static readonly string DesignPropertiesControlsUnityName = "DesignProperties.ControlsUnityName";
		public static readonly string DesignPropertiesPaperKind = "DesignProperties.PaperKind";
		public static readonly string DesignPropertiesLandscape = "DesignProperties.Landscape";
		public static readonly string DesignPropertiesForeColor = "DesignProperties.ForeColor";
		public static readonly string DesignPropertiesBackColor = "DesignProperties.BackColor";
		public static readonly string DesignPropertiesTextAlignment = "DesignProperties.TextAlignment";
		public static readonly string DesignPropertiesXRBinding = "DesignProperties.XRBinding";
		public static readonly string DesignPropertiesCanXRBindOnClient = "DesignProperties.CanXRBindOnClient";
		public static readonly string DesignPropertiesXRControlTypeName = "DesignProperties.XRControlTypeName";
		public static readonly string DesignPropertiesXRControlCustomProperties = "DesignProperties.XRControlCustomProperties";
		public static readonly string DesignPropertiesBorderColor = "DesignProperties.BorderColor";
		public static readonly string DesignPropertiesBorderWidth = "DesignProperties.BorderWidth";
		public static readonly string DesignPropertiesBorderDashStyle = "DesignProperties.BorderDashStyle";
		public static readonly string DesignPropertiesBorderSide = "DesignProperties.BorderSide";
		public static readonly string DesignPropertiesShapeSpecifics = "DesignProperties.ShapeSpecifics";
		public static readonly string DesignPropertiesLineSpecifics = "DesignProperties.LineSpecifics";
#endif
	}
}
