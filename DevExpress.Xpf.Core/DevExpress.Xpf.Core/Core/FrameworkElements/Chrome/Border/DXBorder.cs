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
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using DevExpress.Mvvm.Native;
using System.Windows.Input;
using DevExpress.Xpf.Core.Internal;
using DevExpress.Xpf.Core.Native;
using System.Collections;
namespace DevExpress.Xpf.Core {
	public class DXBorder : Border, IChrome, IElementOwner {
		static readonly RenderTemplate RenderTemplate;
		static DXBorder() {
			Type ownerType = typeof(DXBorder);
			RenderTemplate = CreateRenderTemplate();
			BackgroundProperty.OverrideMetadata(ownerType, new FrameworkPropertyMetadata(UpdateContextPropertyValue));
			BorderBrushProperty.OverrideMetadata(ownerType, new FrameworkPropertyMetadata(UpdateContextPropertyValue));
			BorderThicknessProperty.OverrideMetadata(ownerType, new FrameworkPropertyMetadata(UpdateContextPropertyValue));
			CornerRadiusProperty.OverrideMetadata(ownerType, new FrameworkPropertyMetadata(UpdateContextPropertyValue));
			MarginProperty.OverrideMetadata(ownerType, new FrameworkPropertyMetadata(UpdateContextPropertyValue));
			PaddingProperty.OverrideMetadata(ownerType, new FrameworkPropertyMetadata(UpdateContextPropertyValue));
			MinWidthProperty.OverrideMetadata(ownerType, new FrameworkPropertyMetadata(UpdateContextPropertyValue));
			MaxWidthProperty.OverrideMetadata(ownerType, new FrameworkPropertyMetadata(UpdateContextPropertyValue));
			WidthProperty.OverrideMetadata(ownerType, new FrameworkPropertyMetadata(UpdateContextPropertyValue));
			MinHeightProperty.OverrideMetadata(ownerType, new FrameworkPropertyMetadata(UpdateContextPropertyValue));
			HeightProperty.OverrideMetadata(ownerType, new FrameworkPropertyMetadata(UpdateContextPropertyValue));
			MaxHeightProperty.OverrideMetadata(ownerType, new FrameworkPropertyMetadata(UpdateContextPropertyValue));
			HorizontalAlignmentProperty.OverrideMetadata(ownerType, new FrameworkPropertyMetadata(UpdateContextPropertyValue));
			VerticalAlignmentProperty.OverrideMetadata(ownerType, new FrameworkPropertyMetadata(UpdateContextPropertyValue));
			VisibilityProperty.OverrideMetadata(ownerType, new FrameworkPropertyMetadata(UpdateContextPropertyValue));
			FlowDirectionProperty.OverrideMetadata(ownerType, new FrameworkPropertyMetadata(FlowDirection.LeftToRight, ChromeSlave.GetDefaultOptions(FlowDirectionProperty), (o, args) => ChromeSlave.CoerceValue((IChrome)o, FrameworkRenderElementContext.FlowDirectionPropertyKey)));
			UseLayoutRoundingProperty.OverrideMetadata(ownerType, new FrameworkPropertyMetadata(false, ChromeSlave.GetDefaultOptions(UseLayoutRoundingProperty), (o, args) => ChromeSlave.CoerceValue((IChrome)o, FrameworkRenderElementContext.UseLayoutRoundingPropertyKey)));
			ThemeManager.TreeWalkerProperty.OverrideMetadata(ownerType, new FrameworkPropertyMetadata(null, ChromeSlave.GetDefaultOptions(ThemeManager.TreeWalkerProperty), OnTreeWalkerPropertyChanged));
		}
		static void OnTreeWalkerPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((DXBorder)d).OnTreeWalkerChanged((ThemeTreeWalker)e.OldValue, (ThemeTreeWalker)e.NewValue);
		}
		static void UpdateContextPropertyValue(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			var chrome = (DXBorder)d;
			if (chrome.Root != null) {
				chrome.Root.SetValue(e.Property.Name, e.NewValue);
			}
		}
		readonly ChromeSlave slave;
		public bool EnableDPICorrection {
			get { return ThemeManager.EnableDPICorrection ?? enableDPICorrection; }
			set {
				if (enableDPICorrection == value)
					return;
				enableDPICorrection = value;
				OnEnableDPICorrectionChanged();
			}
		}
		FrameworkRenderElementContext Root { get { return slave.Root; } }
		FrameworkElement IElementOwner.Child { get { return (FrameworkElement)Child; } }
		public DXBorder() {
			slave = new ChromeSlave(this, false, InitializeContext, ReleaseContext);
			EnableDPICorrection = true;
		}
		FrameworkRenderElementContext InitializeContext() {
			var result = slave.CreateContext(RenderTemplate);
			AssignProperties(result);
			return result;
		}
		void ReleaseContext(FrameworkRenderElementContext context) { slave.ReleaseContext(context); }
		void AssignProperties(FrameworkRenderElementContext context) {
			context.SetValue(BackgroundProperty.Name, Background);
			context.SetValue(BorderBrushProperty.Name, BorderBrush);
			context.SetValue(BorderThicknessProperty.Name, BorderThickness);
			context.SetValue(CornerRadiusProperty.Name, CornerRadius);
			context.SetValue(HorizontalAlignmentProperty.Name, HorizontalAlignment);
			context.SetValue(MarginProperty.Name, Margin);
			context.SetValue(MaxHeightProperty.Name, MaxHeight);
			context.SetValue(MaxWidthProperty.Name, MaxWidth);
			context.SetValue(MinHeightProperty.Name, MinHeight);
			context.SetValue(MinWidthProperty.Name, MinWidth);
			context.SetValue(HeightProperty.Name, Height);
			context.SetValue(WidthProperty.Name, Width);
			context.SetValue(PaddingProperty.Name, Padding);
			context.SetValue(VerticalAlignmentProperty.Name, VerticalAlignment);
			context.SetValue(VisibilityProperty.Name, Visibility);
			context.SetValue(UseLayoutRoundingProperty.Name, UseLayoutRounding);
		}
		static RenderTemplate CreateRenderTemplate() {
			var renderTemplate = new RenderTemplate();
			renderTemplate.RenderTree = new RenderBorder() { 
				Child = new RenderElementStub() 
			};
			return renderTemplate;
		}
		void IChrome.AddChild(FrameworkElement element) {
			AddLogicalChild(element);
			AddVisualChild(element);
		}
		void IChrome.RemoveChild(FrameworkElement element) {
			RemoveLogicalChild(element);
			RemoveVisualChild(element);
		}
		protected override Size MeasureOverride(Size availableSize) {
			if (EnableDPICorrection)
				return slave.MeasureOverride(availableSize);
			ReleaseContext(Root);
			return base.MeasureOverride(availableSize);
		}
		protected override Size ArrangeOverride(Size finalSize) {
			if (EnableDPICorrection)
				return slave.ArrangeOverride(finalSize);
			return base.ArrangeOverride(finalSize);
		}
		protected override void OnRender(DrawingContext dc) {
			if (EnableDPICorrection)
				slave.OnRender(dc);
			else
				base.OnRender(dc);
		}
		protected virtual void OnEnableDPICorrectionChanged() {
			this.SetBypassLayoutPolicies(enableDPICorrection);
			InvalidateMeasure();
		}
		protected virtual void OnTreeWalkerChanged(ThemeTreeWalker oldValue, ThemeTreeWalker newValue) {
			ChromeSlave.CoerceValue(this, FrameworkRenderElementContext.IsTouchEnabledPropertyKey);
		}
		protected override Geometry GetLayoutClip(Size layoutSlotSize) {
			Geometry clip = base.GetLayoutClip(layoutSlotSize);
			if (!EnableDPICorrection || Root == null)
				return clip;
			if (clip == null && Root.NeedsClipBounds)
				return Root.LayoutClip;
			if (Root.LayoutClip == null)
				return clip;
			return Root.NeedsClipBounds && Root.LayoutClip != null ? new CombinedGeometry(clip, Root.LayoutClip) : clip;
		}
		FrameworkRenderElementContext IChrome.Root { get { return Root; } }
		void IChrome.GoToState(string stateName) {
			if (EnableDPICorrection)
				slave.GoToState(stateName);
		}
		bool enableDPICorrection;
	}
	static class DXBorderHelper {
		public static bool IsValid(this Thickness thickness) {
			double left = thickness.Left;
			double right = thickness.Right;
			double top = thickness.Top;
			double bottom = thickness.Bottom;
			if(left < 0d || right < 0d || top < 0d || bottom < 0d)
				return false;
			if(double.IsNaN(left) || double.IsNaN(right) || double.IsNaN(top) || double.IsNaN(bottom))
				return false;
			if(Double.IsPositiveInfinity(left) || Double.IsPositiveInfinity(right) || Double.IsPositiveInfinity(top) || Double.IsPositiveInfinity(bottom)) {
				return false;
			}
			if(Double.IsNegativeInfinity(left) || Double.IsNegativeInfinity(right) || Double.IsNegativeInfinity(top) || Double.IsNegativeInfinity(bottom)) {
				return false;
			}
			return true;
		}
		public static bool IsValid(this CornerRadius cornerRadius) {
			double topLeft = cornerRadius.TopLeft;
			double topRight = cornerRadius.TopRight;
			double bottomLeft = cornerRadius.BottomLeft;
			double bottomRight = cornerRadius.BottomRight;
			if(topLeft < 0d || topRight < 0d || bottomLeft < 0d || bottomRight < 0d) {
				return (false);
			}
			if(double.IsNaN(topLeft) || double.IsNaN(topRight) || double.IsNaN(bottomLeft) || double.IsNaN(bottomRight)) {
				return (false);
			}
			if(Double.IsPositiveInfinity(topLeft) || Double.IsPositiveInfinity(topRight) || Double.IsPositiveInfinity(bottomLeft) || Double.IsPositiveInfinity(bottomRight)) {
				return (false);
			}
			if(Double.IsNegativeInfinity(topLeft) || Double.IsNegativeInfinity(topRight) || Double.IsNegativeInfinity(bottomLeft) || Double.IsNegativeInfinity(bottomRight)) {
				return (false);
			}
			return true;
		}
	}
}
