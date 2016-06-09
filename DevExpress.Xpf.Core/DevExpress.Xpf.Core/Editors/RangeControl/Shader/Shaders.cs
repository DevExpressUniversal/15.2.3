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
using System.Windows.Media.Effects;
namespace DevExpress.Xpf.Editors.RangeControl.Internal {
	public class GrayScaleEffect : ShaderEffect {
		public static readonly DependencyProperty InputProperty;
		public static readonly DependencyProperty LeftProperty;
		public static readonly DependencyProperty TopProperty;
		public static readonly DependencyProperty RightProperty;
		public static readonly DependencyProperty BottomProperty;
		public static readonly DependencyProperty RFactorProperty;
		public static readonly DependencyProperty GFactorProperty;
		public static readonly DependencyProperty BFactorProperty;
		PixelShader shader;
		static GrayScaleEffect() {
			InputProperty = ShaderEffect.RegisterPixelShaderSamplerProperty("Input", typeof(GrayScaleEffect), 0);
			LeftProperty = DependencyProperty.Register("Left", typeof(double), typeof(GrayScaleEffect),
													   new UIPropertyMetadata(0d, PixelShaderConstantCallback(0)));
			TopProperty = DependencyProperty.Register("Top", typeof(double), typeof(GrayScaleEffect),
													  new UIPropertyMetadata(0d, PixelShaderConstantCallback(1)));
			RightProperty = DependencyProperty.Register("Right", typeof(double), typeof(GrayScaleEffect),
														new UIPropertyMetadata(0d, PixelShaderConstantCallback(2)));
			BottomProperty = DependencyProperty.Register("Bottom", typeof(double), typeof(GrayScaleEffect),
														 new UIPropertyMetadata(0d, PixelShaderConstantCallback(3)));
			RFactorProperty = DependencyProperty.Register("RFactor", typeof(double), typeof(GrayScaleEffect),
														new UIPropertyMetadata(0d, PixelShaderConstantCallback(6)));
			GFactorProperty = DependencyProperty.Register("GFactor", typeof(double), typeof(GrayScaleEffect),
														new UIPropertyMetadata(0d, PixelShaderConstantCallback(7)));
			BFactorProperty = DependencyProperty.Register("BFactor", typeof(double), typeof(GrayScaleEffect),
														new UIPropertyMetadata(0d, PixelShaderConstantCallback(8)));
		}
		public GrayScaleEffect() {
			InitializeShader();
			this.UpdateShaderValue(InputProperty);
		}
		public Brush Input {
			get { return ((Brush)this.GetValue(InputProperty)); }
			set { this.SetValue(InputProperty, value); }
		}
		bool isEnable = true;
		public bool IsEnable {
			get { return isEnable; }
			set { isEnable = value; }
		}
		public double Left {
			get { return (double)GetValue(LeftProperty); }
			set { SetValue(LeftProperty, value); }
		}
		public double Top {
			get { return (double)GetValue(TopProperty); }
			set { SetValue(TopProperty, value); }
		}
		public double Right {
			get { return (double)GetValue(RightProperty); }
			set { SetValue(RightProperty, value); }
		}
		public double Bottom {
			get { return (double)GetValue(BottomProperty); }
			set { SetValue(BottomProperty, value); }
		}
		public double RFactor {
			get { return (double)GetValue(RFactorProperty); }
			set { SetValue(RFactorProperty, value); }
		}
		public double GFactor {
			get { return (double)GetValue(GFactorProperty); }
			set { SetValue(GFactorProperty, value); }
		}
		public double BFactor {
			get { return (double)GetValue(BFactorProperty); }
			set { SetValue(BFactorProperty, value); }
		}
		string UriString { get { return "/DevExpress.Xpf.Core" + AssemblyInfo.VSuffix + ";component/editors/rangecontrol/shader/grayscaleshader.ps"; } }
		public void Invalidate(double[] bounds) {
			if(!IsEnable) bounds = new double[]{0,0,1,1};
			SetValue(LeftProperty, bounds[0]);
			this.UpdateShaderValue(LeftProperty);
			SetValue(TopProperty, bounds[1]);
			this.UpdateShaderValue(TopProperty);
			SetValue(RightProperty, bounds[2]);
			this.UpdateShaderValue(RightProperty);
			SetValue(BottomProperty, bounds[3]);
			this.UpdateShaderValue(BottomProperty);
		}
		private void InitializeShader() {
			shader = new PixelShader();
			shader.UriSource = new Uri(UriString, UriKind.Relative);
			this.PixelShader = shader;
		}
	}
	public class TransparencyEffect : ShaderEffect {
		public static readonly DependencyProperty InputProperty;
		public static readonly DependencyProperty LeftProperty;
		public static readonly DependencyProperty RightProperty;
		PixelShader shader;
		static TransparencyEffect() {
			InputProperty = ShaderEffect.RegisterPixelShaderSamplerProperty("Input", typeof(TransparencyEffect), 1);
			LeftProperty = DependencyProperty.Register("Left", typeof(double), typeof(TransparencyEffect),
													   new UIPropertyMetadata(0d, PixelShaderConstantCallback(4)));
			RightProperty = DependencyProperty.Register("Right", typeof(double), typeof(TransparencyEffect),
														new UIPropertyMetadata(0d, PixelShaderConstantCallback(5)));
		}
		public TransparencyEffect() {
			InitializeShader();
			this.UpdateShaderValue(InputProperty);
		}
		public double Left {
			get {return (double)GetValue(LeftProperty); }
			set{ SetValue(LeftProperty, value);}
		}
		public double Right {
			get { return (double)GetValue(RightProperty); }
			set { SetValue(RightProperty, value); }
		}
		public Brush Input {
			get { return ((Brush)this.GetValue(InputProperty)); }
			set { this.SetValue(InputProperty, value); }
		}
		string UriString { get { return "/DevExpress.Xpf.Core" + AssemblyInfo.VSuffix + ";component/editors/rangecontrol/shader/transparencyshader.ps"; } }
		public void Invalidate(double left, double right) {
			SetValue(LeftProperty, left);
			this.UpdateShaderValue(LeftProperty);
			SetValue(RightProperty, right);
			this.UpdateShaderValue(RightProperty);
		}
		private void InitializeShader() {
			shader = new PixelShader();
			shader.UriSource = new Uri(UriString, UriKind.Relative);
			this.PixelShader = shader;
		}
	}
}
