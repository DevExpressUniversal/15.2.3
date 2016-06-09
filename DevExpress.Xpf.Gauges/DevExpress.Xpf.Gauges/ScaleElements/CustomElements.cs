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

using System.Collections.Specialized;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Gauges.Native;
using DevExpress.Xpf.Utils;
namespace DevExpress.Xpf.Gauges {
	[ContentProperty("Content")]
	public class ScaleCustomElement : GaugeElement, IGaugeLayoutElement, IElementInfo {
		public static readonly DependencyProperty ContentProperty = DependencyPropertyManager.Register("Content",
			typeof(object), typeof(ScaleCustomElement), new PropertyMetadata(ContentPropertyChanged));
		public static readonly DependencyProperty ContentTemplateProperty = DependencyPropertyManager.Register("ContentTemplate",
			typeof(DataTemplate), typeof(ScaleCustomElement));
		public static readonly DependencyProperty VisibleProperty = DependencyPropertyManager.Register("Visible",
			typeof(bool), typeof(ScaleCustomElement), new PropertyMetadata(true, PropertyChanged));
		public static readonly DependencyProperty ZIndexProperty = DependencyPropertyManager.Register("ZIndex",
			typeof(int), typeof(ScaleCustomElement), new PropertyMetadata(30, ZIndexPropertyChanged));
		[
#if !SL
	DevExpressXpfGaugesLocalizedDescription("ScaleCustomElementContent"),
#endif
		TypeConverter(typeof(StringConverter)),
		Category(Categories.Data)
		]
		public object Content {
			get { return (object)GetValue(ContentProperty); }
			set { SetValue(ContentProperty, value); }
		}
		[
#if !SL
	DevExpressXpfGaugesLocalizedDescription("ScaleCustomElementContentTemplate"),
#endif
		Category(Categories.Presentation)
		]
		public DataTemplate ContentTemplate {
			get { return (DataTemplate)GetValue(ContentTemplateProperty); }
			set { SetValue(ContentTemplateProperty, value); }
		}
		[
#if !SL
	DevExpressXpfGaugesLocalizedDescription("ScaleCustomElementVisible"),
#endif
		Category(Categories.Behavior)
		]
		public bool Visible {
			get { return (bool)GetValue(VisibleProperty); }
			set { SetValue(VisibleProperty, value); }
		}
		[
#if !SL
	DevExpressXpfGaugesLocalizedDescription("ScaleCustomElementZIndex"),
#endif
		Category(Categories.Layout)
		]
		public int ZIndex {
			get { return (int)GetValue(ZIndexProperty); }
			set { SetValue(ZIndexProperty, value); }
		}
		static void ZIndexPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			ScaleCustomElement label = d as ScaleCustomElement;
			if (label != null)
				Canvas.SetZIndex(label, (int)e.NewValue);
		}
		static void ContentPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			ILogicalParent label = d as ILogicalParent;
			if (label != null) {
				if (e.OldValue != null)
					label.RemoveChild(e.OldValue);
				if (e.NewValue != null)
					label.AddChild(e.NewValue);
			}
		}
		protected static void PropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			ScaleCustomElement element = d as ScaleCustomElement;
			if (element != null && element.Scale != null)
				element.Scale.Invalidate();
		}
		ElementLayout layout = null;
		internal Scale Scale { get { return Owner as Scale; } }
		public ScaleCustomElement() {
			DefaultStyleKey = typeof(ScaleCustomElement);
		}
		#region IGaugeLayoutElement implementation
		Point IGaugeLayoutElement.Offset { get { return Scale != null ? Scale.GetLayoutOffset() : new Point(0, 0); } }
		bool IGaugeLayoutElement.InfluenceOnGaugeSize { get { return false; } }
		ElementLayout IGaugeLayoutElement.Layout {
			get { return layout; }
		}
		#endregion
		#region IElementInfo implementation
		void IElementInfo.Invalidate() {
			UIElement parent = LayoutHelper.GetParent(this) as UIElement;
			if (parent != null)
				parent.InvalidateMeasure();
		}
		#endregion
		protected virtual ElementLayout CreateLayout(ScaleMapping mapping) {
			ElementLayout result = new ElementLayout(mapping.Layout.InitialBounds.Width, mapping.Layout.InitialBounds.Height);
			result.CompleteLayout(new Point(0, 0), null, null);
			return result;
		}
		internal void CalculateLayout(ScaleMapping mapping) {
			layout = Visible ? CreateLayout(mapping) : null;
		}		
	}
	public class ScaleCustomElementCollection : GaugeElementCollection<ScaleCustomElement> {
		Scale Scale { get { return Owner as Scale; } }
		public ScaleCustomElementCollection(Scale scale) {
			((IOwnedElement)this).Owner = scale;
		}
		protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e) {
			base.OnCollectionChanged(e);
			if (Scale != null && Scale.Gauge != null)
				Scale.Gauge.UpdateElements();
		}
	}
}
