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
using System.Windows.Markup;
using System.Windows.Media;
using DevExpress.Utils;
using DevExpress.Utils.Serializing;
using DevExpress.Xpf.Charts.Native;
using DevExpress.Xpf.Core.Serialization;
using DevExpress.Xpf.Utils;
namespace DevExpress.Xpf.Charts {
	[ContentProperty("Content")]
	public abstract class TitleBase : ChartElement, ISupportFlowDirection {
		public static readonly DependencyProperty ContentProperty = DependencyPropertyManager.Register("Content", typeof(object), typeof(TitleBase), new PropertyMetadata("Title", ContentChanged));
		public static readonly DependencyProperty ContentTemplateProperty = DependencyPropertyManager.Register("ContentTemplate",
			typeof(DataTemplate), typeof(TitleBase), new PropertyMetadata(ChartElementHelper.UpdateWithClearDiagramCache));
		public static readonly DependencyProperty VisibleProperty = DependencyPropertyManager.Register("Visible", typeof(bool?), typeof(TitleBase), new PropertyMetadata(true, VisibleChanged));
		static void ContentChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			if (e.OldValue != null)
				((IChartElement)d).RemoveChild(e.OldValue);
			if (e.NewValue != null)
				((IChartElement)d).AddChild(e.NewValue);
			ChartElementHelper.UpdateWithClearDiagramCache(d, e);
		}
		static void VisibleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			TitleBase baseTitle = d as TitleBase;
			if (baseTitle != null)
				baseTitle.InvalidateMeasure();
			ChartElementHelper.UpdateWithClearDiagramCache(d, e);
		}
		[
#if !SL
	DevExpressXpfChartsLocalizedDescription("TitleBaseContent"),
#endif
		Category(Categories.Common),
		XtraSerializableProperty
		]
		public object Content {
			get { return (object)GetValue(ContentProperty); }
			set { SetValue(ContentProperty, value); }
		}
		[
#if !SL
	DevExpressXpfChartsLocalizedDescription("TitleBaseContentTemplate"),
#endif
		Category(Categories.Presentation)
		]
		public DataTemplate ContentTemplate {
			get { return (DataTemplate)GetValue(ContentTemplateProperty); }
			set { SetValue(ContentTemplateProperty, value); }
		}
		[
#if !SL
	DevExpressXpfChartsLocalizedDescription("TitleBaseVisible"),
#endif
		Category(Categories.Behavior),
		XtraSerializableProperty
		]
		public bool? Visible {
			get { return (bool?)GetValue(VisibleProperty); }
			set { SetValue(VisibleProperty, value); }
		}
		[
#if !SL
	DevExpressXpfChartsLocalizedDescription("TitleBaseHorizontalAlignment"),
#endif
		Category(Categories.Layout),
		DXBrowsable(true), Browsable(true),
		EditorBrowsable(EditorBrowsableState.Always),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Visible),
		XtraSerializableProperty
		]
		public new HorizontalAlignment HorizontalAlignment {
			get { return base.HorizontalAlignment; }
			set { base.HorizontalAlignment = value; }
		}
		[
#if !SL
	DevExpressXpfChartsLocalizedDescription("TitleBaseVerticalAlignment"),
#endif
		Category(Categories.Layout),
		DXBrowsable(true), Browsable(true),
		EditorBrowsable(EditorBrowsableState.Always),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Visible),
		XtraSerializableProperty
		]
		public new VerticalAlignment VerticalAlignment {
			get { return base.VerticalAlignment; }
			set { base.VerticalAlignment = value; }
		}
		internal virtual bool ActualVisible {
			get {
				if (!Visible.HasValue)
					return true;
				return Visible.Value;
			}
		}
		#region Hidden properties
		[DXBrowsable(false), Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new FlowDirection FlowDirection { get { return base.FlowDirection; } set { base.FlowDirection = value; } }
		[DXBrowsable(false), Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new bool UseLayoutRounding { get { return base.UseLayoutRounding; } set { base.UseLayoutRounding = value; } }
		[DXBrowsable(false), Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new Visibility Visibility { get { return base.Visibility; } set { base.Visibility = value; } }
		[DXBrowsable(false), Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new HorizontalAlignment HorizontalContentAlignment { get { return base.HorizontalContentAlignment; } set { base.HorizontalContentAlignment = value; } }
		[DXBrowsable(false), Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new VerticalAlignment VerticalContentAlignment { get { return base.VerticalContentAlignment; } set { base.VerticalContentAlignment = value; } }
		[DXBrowsable(false), Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new Style FocusVisualStyle { get { return base.FocusVisualStyle; } set { base.FocusVisualStyle = value; } }
		[DXBrowsable(false), Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new bool Focusable { get { return base.Focusable; } set { base.Focusable = value; } }
		[DXBrowsable(false), Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new object ToolTip { get { return base.ToolTip; } set { base.ToolTip = value; } }
		#endregion
		public TitleBase() {
			DXSerializer.AddCustomGetSerializablePropertiesHandler(this, CustomPropertiesSerializationUtils.SerializableControlProperties);
		}
		Transform ISupportFlowDirection.CreateDirectionTransform() {
			if (FlowDirection == FlowDirection.RightToLeft)
				return new ScaleTransform() { ScaleX = -1, ScaleY = 1, CenterX = 0.5, CenterY = 0.5 };
			return Transform.Identity;
		}
		protected override Size MeasureOverride(Size constraint) {
			return base.MeasureOverride(ActualVisible ? constraint : new Size(0, 0));
		}
		public bool ShouldSerializeVisible(XamlDesignerSerializationManager manager) { return this.Visible.HasValue; }
	}
}
