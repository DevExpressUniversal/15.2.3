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
using System.Windows;
using System.Windows.Media;
using DevExpress.Utils;
using DevExpress.Xpf.Printing.Native;
using DevExpress.Xpf.Utils;
using DevExpress.XtraPrinting;
namespace DevExpress.Xpf.Printing.BrickCollection {
	class EffectiveExportSettings : IExportSettings {
		protected readonly DependencyObject source;
		public EffectiveExportSettings(DependencyObject source) {
			Guard.ArgumentNotNull(source, "source");
			this.source = source;
		}
		#region IExportSettings Members
		public Color Background { get { return GetEffectiveExportValue<Color>(ExportSettings.BackgroundProperty, o => o.Background); } }
		public Color BorderColor {
			get { return GetEffectiveExportValue<Color>(ExportSettings.BorderColorProperty, o => o.BorderColor); }
		}
		public Thickness BorderThickness {
			get { return GetEffectiveExportValue<Thickness>(ExportSettings.BorderThicknessProperty, o => o.BorderThickness); }
		}
		public Color Foreground {
			get { return GetEffectiveExportValue<Color>(ExportSettings.ForegroundProperty, o => o.Foreground); }
		}
		public string Url {
			get { return GetEffectiveExportValue<string>(ExportSettings.UrlProperty, o => o.Url); }
		}
		public IOnPageUpdater OnPageUpdater {
			get { return GetEffectiveExportValue<IOnPageUpdater>(ExportSettings.OnPageUpdaterProperty, o => o.OnPageUpdater); }
		}
		public BorderDashStyle BorderDashStyle {
			get { return GetEffectiveExportValue<BorderDashStyle>(ExportSettings.BorderDashStyleProperty, o => o.BorderDashStyle); }
		}
		public object MergeValue {
			get { return GetEffectiveExportValue<object>(ExportSettings.MergeValueProperty, o => o.MergeValue); }
		}
		#endregion
		T GetEffectiveExportValue<T>(DependencyProperty property, Func<IExportSettings, T> getValue) {
			return GetEffectiveValue<T, IExportSettings>(property, getValue);
		}
		protected T GetEffectiveValue<T, TInterface>(DependencyProperty property, Func<TInterface, T> getValue) where TInterface : class, IExportSettings {
			ValueSource valueSource = DependencyPropertyHelper.GetValueSource(source, property);
			if(valueSource.BaseValueSource > BaseValueSource.Default) {
				return (T)source.GetValue(property);
			}
			TInterface sourceView = source as TInterface;
			if(sourceView != null) {
				return getValue(sourceView);
			}
			return (T)property.GetDefaultValue();
		}
	}
}
