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
using DevExpress.Xpf.Gauges.Native;
using DevExpress.Xpf.Utils;
namespace DevExpress.Xpf.Gauges {
	public class ScaleCustomLabel : ScaleCustomElement { 
		public static readonly DependencyProperty OffsetProperty = DependencyPropertyManager.Register("Offset",
			typeof(double), typeof(ScaleCustomLabel), new PropertyMetadata(0.0, PropertyChanged));
		public static readonly DependencyProperty ValueProperty = DependencyPropertyManager.Register("Value",
			typeof(double), typeof(ScaleCustomLabel), new PropertyMetadata(0.0, PropertyChanged));
		[
#if !SL
	DevExpressXpfGaugesLocalizedDescription("ScaleCustomLabelOffset"),
#endif
		Category(Categories.Layout)
		]
		public double Offset {
			get { return (double)GetValue(OffsetProperty); }
			set { SetValue(OffsetProperty, value); }
		}
		[
#if !SL
	DevExpressXpfGaugesLocalizedDescription("ScaleCustomLabelValue"),
#endif
		Category(Categories.Data)
		]
		public double Value {
			get { return (double)GetValue(ValueProperty); }
			set { SetValue(ValueProperty, value); }
		}
		public ScaleCustomLabel() {
			DefaultStyleKey = typeof(ScaleCustomLabel);
		}
		protected override ElementLayout CreateLayout(ScaleMapping mapping) {
			ElementLayout result = new ElementLayout();
			result.CompleteLayout(mapping.GetPointByValue(Value, Offset), null, null);
			return result;
		}
	}
	public class ScaleCustomLabelCollection : GaugeElementCollection<ScaleCustomLabel> {
		Scale Scale { get { return Owner as Scale; } }
		public ScaleCustomLabelCollection(Scale scale) {
			((IOwnedElement)this).Owner = scale;
		}
		protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e) {
			base.OnCollectionChanged(e);
			if (Scale != null && Scale.Gauge != null)
				Scale.Gauge.UpdateElements();
		}
	}
}
