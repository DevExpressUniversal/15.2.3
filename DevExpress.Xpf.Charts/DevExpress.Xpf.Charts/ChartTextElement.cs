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
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Effects;
using DevExpress.Utils;
using DevExpress.Utils.Serializing;
using DevExpress.Xpf.Charts.Native;
using DevExpress.Xpf.Core.Serialization;
using DevExpress.Xpf.Utils;
namespace DevExpress.Xpf.Charts {
	public abstract class ChartTextElement : ChartElement, ISupportFlowDirection {
		public static readonly DependencyProperty VisibleProperty = DependencyPropertyManager.Register("Visible", 
			typeof(bool), typeof(ChartTextElement), new PropertyMetadata(true, ChartElementHelper.UpdateWithClearDiagramCache));
		public static readonly DependencyProperty ElementTemplateProperty = DependencyPropertyManager.Register("ElementTemplate", 
			typeof(DataTemplate), typeof(ChartTextElement), new PropertyMetadata(ChartElementHelper.UpdateWithClearDiagramCache));
		static ChartTextElement() {
			Type ownerType = typeof(ChartTextElement);
			ForegroundProperty.OverrideMetadata(ownerType, new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.Inherits, FontPropertyChanged));
			FontFamilyProperty.OverrideMetadata(ownerType, new FrameworkPropertyMetadata(SystemFonts.MessageFontFamily, FrameworkPropertyMetadataOptions.Inherits, FontPropertyChanged));
			FontSizeProperty.OverrideMetadata(ownerType, new FrameworkPropertyMetadata(SystemFonts.MessageFontSize, FrameworkPropertyMetadataOptions.Inherits, FontPropertyChanged));
			FontStretchProperty.OverrideMetadata(ownerType, 
				new FrameworkPropertyMetadata(TextElement.FontStretchProperty.DefaultMetadata.DefaultValue, FrameworkPropertyMetadataOptions.Inherits, FontPropertyChanged));
			FontStyleProperty.OverrideMetadata(ownerType, new FrameworkPropertyMetadata(SystemFonts.MessageFontStyle, FrameworkPropertyMetadataOptions.Inherits, FontPropertyChanged));
			FontWeightProperty.OverrideMetadata(ownerType, new FrameworkPropertyMetadata(SystemFonts.MessageFontWeight, FrameworkPropertyMetadataOptions.Inherits, FontPropertyChanged));
		}
		static void FontPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			ChartElementHelper.Update(d, e, ChartElementChange.Diagram3DOnly | ChartElementChange.ClearDiagramCache);
		}
		[
#if !SL
	DevExpressXpfChartsLocalizedDescription("ChartTextElementVisible"),
#endif
		Category(Categories.Behavior),
		XtraSerializableProperty
		]
		public bool Visible {
			get { return (bool)GetValue(VisibleProperty); }
			set { SetValue(VisibleProperty, value); }
		}
		[
#if !SL
	DevExpressXpfChartsLocalizedDescription("ChartTextElementElementTemplate"),
#endif
		Category(Categories.Presentation)
		]
		public DataTemplate ElementTemplate {
			get { return (DataTemplate)GetValue(ElementTemplateProperty); }
			set { SetValue(ElementTemplateProperty, value); }
		}
		#region Hidden properties
		[DXBrowsable(false), Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new FlowDirection FlowDirection { get { return base.FlowDirection; } set { base.FlowDirection = value; } }
		[DXBrowsable(false), Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new bool UseLayoutRounding { get { return base.UseLayoutRounding; } set { base.UseLayoutRounding = value; } }
		[DXBrowsable(false), Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new double Opacity { get { return base.Opacity; } set { base.Opacity = value; } }
		[DXBrowsable(false), Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new Brush OpacityMask { get { return base.OpacityMask; } set { base.OpacityMask = value; } }
		[DXBrowsable(false), Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new Effect Effect { get { return base.Effect; } set { base.Effect = value; } }
		[DXBrowsable(false), Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new Transform RenderTransform { get { return base.RenderTransform; } set { base.RenderTransform = value; } }
		[DXBrowsable(false), Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new Point RenderTransformOrigin { get { return base.RenderTransformOrigin; } set { base.RenderTransformOrigin = value; } }
		[DXBrowsable(false), Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new Visibility Visibility { get { return base.Visibility; } set { base.Visibility = value; } }
		[DXBrowsable(false), Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new Geometry Clip { get { return base.Clip; } set { base.Clip = value; } }
		[DXBrowsable(false), Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new Thickness Margin { get { return base.Margin; } set { base.Margin = value; } }
		[DXBrowsable(false), Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new Transform LayoutTransform { get { return base.LayoutTransform; } set { base.LayoutTransform = value; } }
		[DXBrowsable(false), Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new bool Focusable { get { return base.Focusable; } set { base.Focusable = value; } }
		[DXBrowsable(false), Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new Style FocusVisualStyle { get { return base.FocusVisualStyle; } set { base.FocusVisualStyle = value; } }
		[DXBrowsable(false), Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new object ToolTip { get { return base.ToolTip; } set { base.ToolTip = value; } }
		#endregion
		public ChartTextElement() {
			DXSerializer.AddCustomGetSerializablePropertiesHandler(this, CustomPropertiesSerializationUtils.SerializableControlProperties);
		}
		Transform ISupportFlowDirection.CreateDirectionTransform() {
			return FlowDirection == FlowDirection.RightToLeft ? (Transform)new ScaleTransform() { ScaleX = -1, ScaleY = 1, CenterX = 0.5, CenterY = 0.5 } : (Transform)new MatrixTransform();
		}
	}
}
