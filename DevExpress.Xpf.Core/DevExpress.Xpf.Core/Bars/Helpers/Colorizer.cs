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

using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Effects;
namespace DevExpress.Xpf.Bars {
	public class ImageColorizer {
		public static readonly DependencyProperty IsEnabledProperty;
		public static readonly DependencyProperty ColorProperty;
		public static Color GetColor(DependencyObject obj) {
			return (Color)obj.GetValue(ColorProperty);
		}
		public static void SetColor(DependencyObject obj, Color value) {
			obj.SetValue(ColorProperty, value);
		}
		public static bool GetIsEnabled(DependencyObject obj) {
			return (bool)obj.GetValue(IsEnabledProperty);
		}
		public static void SetIsEnabled(DependencyObject obj, bool value) {
			obj.SetValue(IsEnabledProperty, value);
		}
		static ImageColorizer() {
			IsEnabledProperty = DependencyProperty.RegisterAttached("IsEnabled", typeof(bool), typeof(ImageColorizer), new PropertyMetadata(false, new PropertyChangedCallback(OnIsEnabledPropertyChanged)));
			ColorProperty = DependencyProperty.RegisterAttached("Color", typeof(Color), typeof(ImageColorizer), new PropertyMetadata(Colors.White, new PropertyChangedCallback(OnColorPropertyChanged)));
		}
		protected static void OnColorPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			Image image = d as Image;
			if (image == null)
				return;
			UpdateImageColor(image, ((Color)e.NewValue), GetIsEnabled(d));
		}
		protected static void OnIsEnabledPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			Image image = d as Image;
			if (image == null)
				return;
			UpdateImageColor(image, GetColor(d), ((bool)e.NewValue));
		}
		protected internal static void UpdateImageColor(Image image, Color color, bool enabled) {
			ColorizerEffect effect = image.Effect as ColorizerEffect;
			if (!enabled) {
				image.Effect = null;
				return;
			}
			if (image.Effect == null) {
				image.Effect = new ColorizerEffect() { Color = color };
				return;
			}
			effect.Color = color;
		}
	}
	[System.Windows.Markup.ContentProperty("States")]
	public class ImageColorizerSettings<T> {
		public Dictionary<T, Color> States { get; set; }
		public ImageColorizerSettings() {
			States = new Dictionary<T, Color>();
		}
		public virtual void Apply(DependencyObject target, T state) {
			if (target == null)
				return;
			var color = States.ContainsKey(state) ? States[state] : Color.FromArgb(0, 0, 0, 0);
			ImageColorizer.SetColor(target, color);
		}
	}
	public class ColorizerEffect : ShaderEffect {
		[IgnoreDependencyPropertiesConsistencyCheckerAttribute]
		public static readonly DependencyProperty InputProperty = ShaderEffect.RegisterPixelShaderSamplerProperty("Input", typeof(ColorizerEffect), 0);
		[IgnoreDependencyPropertiesConsistencyCheckerAttribute]
		public static readonly DependencyProperty ColorProperty = DependencyProperty.Register("Color", typeof(Color), typeof(ColorizerEffect), new PropertyMetadata(Colors.White, PixelShaderConstantCallback(0)));
		[ThreadStatic]
		static internal PixelShader _pixelShader;
		PixelShader GetPixelShader() {
			if(_pixelShader == null) {
#if DEBUGTEST
				new System.Windows.Documents.FlowDocument();
#endif
				var uriPrefix = "pack://application:,,,/";
				_pixelShader = new PixelShader() { UriSource = new Uri(uriPrefix + AssemblyInfo.SRAssemblyXpfCore + ";component/Bars/colorShader.ps", UriKind.RelativeOrAbsolute) };
			}
			return _pixelShader;
		}
		static ColorizerEffect() {
		}
		public ColorizerEffect() {
			if(GetPixelShader().CanFreeze) {
				this.PixelShader = GetPixelShader().GetAsFrozen() as PixelShader;
			}
			UpdateShaderValue(InputProperty);
			UpdateShaderValue(ColorProperty);
		}
		public Color Color {
			get { return (Color)GetValue(ColorProperty); }
			set { SetValue(ColorProperty, value); }
		}
	}
}
