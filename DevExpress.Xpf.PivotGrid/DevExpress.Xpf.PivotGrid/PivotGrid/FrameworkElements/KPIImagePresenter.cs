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
using System.Windows;
using System.Windows.Data;
using DevExpress.Xpf.Core.Native;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using DevExpress.XtraPivotGrid.Localization;
using DevExpress.XtraPivotGrid.Data;
#if !SL
using CoreGridLengthConverter = System.Windows.GridLengthConverter;
using DevExpress.Xpf.Utils;
#else
using CoreGridLengthConverter = DevExpress.Xpf.Core.GridLengthConverter;
using DevExpress.Xpf.Core.WPFCompatibility;
using PropertyMetadata = DevExpress.Xpf.Core.WPFCompatibility.SLPropertyMetadata;
using DependencyPropertyChangedEventArgs = DevExpress.Xpf.Core.WPFCompatibility.SLDependencyPropertyChangedEventArgs;
using DevExpress.Xpf.Core;
#endif
namespace DevExpress.Xpf.PivotGrid.Internal {
public class KpiImagePresenter : Control {
	#region static
	public static readonly DependencyProperty KpiGraphicProperty;
	public static readonly DependencyProperty KpiTypeProperty;
	public static readonly DependencyProperty ValueProperty;
	public static readonly DependencyProperty ImageSourceProperty;
	static readonly DependencyPropertyKey ImageSourcePropertyKey;
	public static readonly DependencyProperty TooltipProperty;
	static readonly DependencyPropertyKey TooltipPropertyKey;
	static KpiImagePresenter() {
		Type ownerType = typeof(KpiImagePresenter);
		KpiGraphicProperty = DependencyPropertyManager.Register("KpiGraphic", typeof(PivotKpiGraphic), ownerType, new PropertyMetadata(PivotKpiGraphic.None, (d, e) => ((KpiImagePresenter)d).UpdateImageSource()));
		KpiTypeProperty = DependencyPropertyManager.Register("KpiType", typeof(PivotKpiType), ownerType, new PropertyMetadata(PivotKpiType.None, (d, e) => ((KpiImagePresenter)d).UpdateToolTip()));
		ValueProperty = DependencyPropertyManager.Register("Value", typeof(object), ownerType, new PropertyMetadata(null, (d, e) => ((KpiImagePresenter)d).UpdateImageSource()));
		ImageSourcePropertyKey = DependencyPropertyManager.RegisterReadOnly("ImageSource", typeof(ImageSource), ownerType, new PropertyMetadata(null, (d, e) => ((KpiImagePresenter)d).OnImageSourceChanged()));
		ImageSourceProperty = ImageSourcePropertyKey.DependencyProperty;
		TooltipPropertyKey = DependencyPropertyManager.RegisterReadOnly("Tooltip", typeof(string), ownerType, new PropertyMetadata(null));
		TooltipProperty = TooltipPropertyKey.DependencyProperty;
	}
	#endregion
	public KpiImagePresenter() {
		this.SetDefaultStyleKey(typeof(KpiImagePresenter));
	}
	public PivotKpiGraphic KpiGraphic {
		get { return (PivotKpiGraphic)GetValue(KpiGraphicProperty); }
		set { SetValue(KpiGraphicProperty, value); }
	}
	public PivotKpiType KpiType {
		get { return (PivotKpiType)GetValue(KpiTypeProperty); }
		set { SetValue(KpiTypeProperty, value); }
	}
	public object Value {
		get { return (object)GetValue(ValueProperty); }
		set { SetValue(ValueProperty, value); }
	}
	public ImageSource ImageSource {
		get { return (ImageSource)GetValue(ImageSourceProperty); }
		protected set { this.SetValue(ImageSourcePropertyKey, value); }
	}
	public string Tooltip {
		get { return (string)GetValue(TooltipProperty); }
		protected set { this.SetValue(TooltipPropertyKey, value); }
	}
	protected virtual void UpdateImageSource() {
		if(KpiGraphic == PivotKpiGraphic.None || Value == null) {
			ImageSource = null;
			Tooltip = null;
		} else {
			ImageSource = ImageHelper.GetImage(KpiGraphic.ToString() + "." + Value.ToString(), true);
			UpdateToolTip();
		}
	}
	void UpdateToolTip() {
		if(!KpiType.NeedGraphic()) {
			Tooltip = null;
			return;
		}
		int value = -2;
		try {
			value = Convert.ToInt32(Value);
		} catch {
			Tooltip = null;
			return;
		}
		if(value > 1 || value < -1) {
			Tooltip = null;
			return;
		}
		Tooltip = PivotGridData.GetKPITooltipText(KpiType.ToXtraType(), value);
	}
	void OnImageSourceChanged() {
		InvalidateMeasure();
		InvalidateArrange();
	}
}
}
