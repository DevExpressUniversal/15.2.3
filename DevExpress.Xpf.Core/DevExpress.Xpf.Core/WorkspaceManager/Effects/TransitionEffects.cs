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
using System.Windows.Media.Imaging;
namespace DevExpress.Xpf.Core {
	public enum TransitionEffect {
		None,
		Dissolve,
		Fade,
		LineReveal,
		RadialBlur,
		Ripple,
		Wave
	}
	public abstract class TransitionEffectBase : ShaderEffect {
		public static readonly DependencyProperty InputProperty = ShaderEffect.RegisterPixelShaderSamplerProperty("Input", typeof(TransitionEffectBase), 0, SamplingMode.NearestNeighbor);
		public static readonly DependencyProperty OldInputProperty = ShaderEffect.RegisterPixelShaderSamplerProperty("OldInput", typeof(TransitionEffectBase), 1, SamplingMode.NearestNeighbor);
		public static readonly DependencyProperty ProgressProperty = DependencyProperty.Register("Progress", typeof(double), typeof(TransitionEffectBase), new PropertyMetadata(0.0, PixelShaderConstantCallback(0)));
		public TransitionEffectBase() {
			UpdateShaderValue(InputProperty);
			UpdateShaderValue(OldInputProperty);
			UpdateShaderValue(ProgressProperty);
		}
		public Brush Input {
			get { return (Brush)GetValue(InputProperty); }
			set { SetValue(InputProperty, value); }
		}
		public Brush OldInput {
			get { return (Brush)GetValue(OldInputProperty); }
			set { SetValue(OldInputProperty, value); }
		}
		public double Progress {
			get { return (double)GetValue(ProgressProperty); }
			set { SetValue(ProgressProperty, value); }
		}
	}
	public abstract class RandomizedTransitionEffectBase : TransitionEffectBase {
		public static readonly DependencyProperty RandomSeedProperty = DependencyProperty.Register("RandomSeed", typeof(double), typeof(RandomizedTransitionEffectBase), new PropertyMetadata(0.0, PixelShaderConstantCallback(1)));
		public RandomizedTransitionEffectBase() {
			UpdateShaderValue(RandomSeedProperty);
		}
		public double RandomSeed {
			get { return (double)GetValue(RandomSeedProperty); }
			set { SetValue(RandomSeedProperty, value); }
		}
	}
	public abstract class CloudyTransitionEffectBase : RandomizedTransitionEffectBase {
		const int Width = 512;
		const int Height = 512;
		const int Gamma = 510;
		public static readonly DependencyProperty CloudImageProperty = ShaderEffect.RegisterPixelShaderSamplerProperty("CloudImage", typeof(CloudyTransitionEffectBase), 2, SamplingMode.Bilinear);
		static readonly ImageSource cloudsBitmap = GenerateCloudsBitmap();
		static Random random;
		public CloudyTransitionEffectBase() {
			CloudImage = new ImageBrush();
			((ImageBrush)CloudImage).ImageSource = cloudsBitmap;
			UpdateShaderValue(CloudImageProperty);
		}
		public Brush CloudImage {
			get { return (Brush)GetValue(CloudImageProperty); }
			set { SetValue(CloudImageProperty, value); }
		}
		static ImageSource GenerateCloudsBitmap() {
			return BitmapHelpers.GenerateBitmap(Width, Height, GenerateClouds);
		}
		static void GenerateClouds(int[] pixels) {
			random = new Random();
			Plasma(pixels, 0, 0, Width, Height, random.NextDouble(), random.NextDouble(), random.NextDouble(), random.NextDouble());
		}
		static double Displace(double num) {
			double max = num / (Width + Height) * 10.0;
			return (random.NextDouble() - 0.5) * max;
		}
		static void Plasma(int[] pixels, double x, double y, double width, double height, double c1, double c2, double c3, double c4) {
			double edge1, edge2, edge3, edge4, midPoint;
			double newWidth = width / 2;
			double newHeight = height / 2;
			if(width > 1 || height > 1) {
				midPoint = (c1 + c2 + c3 + c4) / 4 + Displace(newWidth + newHeight);
				edge1 = (c1 + c2) / 2;
				edge2 = (c2 + c3) / 2;
				edge3 = (c3 + c4) / 2;
				edge4 = (c4 + c1) / 2;
				if(midPoint < 0)
					midPoint = 0;
				else if(midPoint > 1.0)
					midPoint = 1.0;
				Plasma(pixels, x, y, newWidth, newHeight, c1, edge1, midPoint, edge4);
				Plasma(pixels, x + newWidth, y, newWidth, newHeight, edge1, c2, edge2, midPoint);
				Plasma(pixels, x + newWidth, y + newHeight, newWidth, newHeight, midPoint, edge2, c3, edge3);
				Plasma(pixels, x, y + newHeight, newWidth, newHeight, edge4, midPoint, edge3, c4);
			}
			else {
				double c = (c1 + c2 + c3 + c4) / 4;
				double r = 0, g = 0, b = 0;
				if(c < 0.5)
					r = c * Gamma;
				else
					r = (1.0 - c) * Gamma;
				if(c >= 0.3 && c < 0.8)
					g = (c - 0.3) * Gamma;
				else if(c < 0.3)
					g = (0.3 - c) * Gamma;
				else
					g = (1.3 - c) * Gamma;
				if(c >= 0.5)
					b = (c - 0.5) * Gamma;
				else
					b = (0.5 - c) * Gamma;
				pixels[(int)y * Width + (int)x] = BitmapHelpers.CreateArgb32(0xFF, (int)r, (int)g, (int)b);
			}
		}
	}
	public class DissolveTransitionEffect : RandomizedTransitionEffectBase {
		public static readonly DependencyProperty NoiseImageProperty = ShaderEffect.RegisterPixelShaderSamplerProperty("NoiseImage", typeof(DissolveTransitionEffect), 2, SamplingMode.Bilinear);
		static readonly ImageSource noiseBitmap = GenerateNoiseBitmap();
		public DissolveTransitionEffect() {
			PixelShader shader = new PixelShader();
			shader.UriSource = ResourceUtils.MakeUri("WorkspaceManager/Effects/Shaders/DisolveTransitionEffect.ps");
			PixelShader = shader;
			NoiseImage = new ImageBrush();
			((ImageBrush)NoiseImage).ImageSource = noiseBitmap;
			UpdateShaderValue(NoiseImageProperty);
		}
		public Brush NoiseImage {
			get { return (Brush)GetValue(NoiseImageProperty); }
			set { SetValue(NoiseImageProperty, value); }
		}
		static ImageSource GenerateNoiseBitmap() {
			return BitmapHelpers.GenerateBitmap(512, 512, GenerateNoise);
		}
		static void GenerateNoise(int[] pixels) {
			Random rnd = new Random();
			for(int i = 0; i < pixels.Length; i++) {
				int r = rnd.Next(0, 255);
				int g = rnd.Next(0, 255);
				int b = rnd.Next(0, 255);
				pixels[i] = BitmapHelpers.CreateArgb32(0xFF, r, g, b);
			}
		}
	}
	public class DropFadeTransitionEffect : CloudyTransitionEffectBase {
		public DropFadeTransitionEffect() {
			PixelShader shader = new PixelShader();
			shader.UriSource = ResourceUtils.MakeUri("WorkspaceManager/Effects/Shaders/DropFadeTransitionEffect.ps");
			PixelShader = shader;
		}
	}
	public class FadeTransitionEffect : TransitionEffectBase {
		public FadeTransitionEffect() {
			PixelShader shader = new PixelShader();
			shader.UriSource = ResourceUtils.MakeUri("WorkspaceManager/Effects/Shaders/FadeTransitionEffect.ps");
			PixelShader = shader;
		}
	}
	public class LineRevealTransitionEffect : TransitionEffectBase {
		public static readonly DependencyProperty LineOriginProperty = DependencyProperty.Register("LineOrigin", typeof(Point), typeof(LineRevealTransitionEffect), new PropertyMetadata(new Point(-0.2, -0.2), PixelShaderConstantCallback(1)));
		public static readonly DependencyProperty LineNormalProperty = DependencyProperty.Register("LineNormal", typeof(Point), typeof(LineRevealTransitionEffect), new PropertyMetadata(new Point(1.0, 0.0), PixelShaderConstantCallback(2)));
		public static readonly DependencyProperty LineOffsetProperty = DependencyProperty.Register("LineOffset", typeof(Point), typeof(LineRevealTransitionEffect), new PropertyMetadata(new Point(1.4, 1.4), PixelShaderConstantCallback(3)));
		public static readonly DependencyProperty FuzzyAmountProperty = DependencyProperty.Register("FuzzyAmount", typeof(double), typeof(LineRevealTransitionEffect), new PropertyMetadata(0.2, PixelShaderConstantCallback(4)));
		public LineRevealTransitionEffect() {
			UpdateShaderValue(LineOriginProperty);
			UpdateShaderValue(LineNormalProperty);
			UpdateShaderValue(LineOffsetProperty);
			UpdateShaderValue(FuzzyAmountProperty);
			PixelShader shader = new PixelShader();
			shader.UriSource = ResourceUtils.MakeUri("WorkspaceManager/Effects/Shaders/LineRevealTransitionEffect.ps");
			PixelShader = shader;
		}
		public Point LineOrigin {
			get { return (Point)GetValue(LineOriginProperty); }
			set { SetValue(LineOriginProperty, value); }
		}
		public Point LineNormal {
			get { return (Point)GetValue(LineNormalProperty); }
			set { SetValue(LineNormalProperty, value); }
		}
		public Point LineOffset {
			get { return (Point)GetValue(LineOffsetProperty); }
			set { SetValue(LineOffsetProperty, value); }
		}
		public double FuzzyAmount {
			get { return (double)GetValue(FuzzyAmountProperty); }
			set { SetValue(FuzzyAmountProperty, value); }
		}
	}
	public class RadialBlurTransitionEffect : TransitionEffectBase {
		public RadialBlurTransitionEffect() {
			PixelShader shader = new PixelShader();
			shader.UriSource = ResourceUtils.MakeUri("WorkspaceManager/Effects/Shaders/RadialBlurTransitionEffect.ps");
			PixelShader = shader;
		}
	}
	public class RandomCircleRevealTransitionEffect : CloudyTransitionEffectBase {
		public RandomCircleRevealTransitionEffect() {
			PixelShader shader = new PixelShader();
			shader.UriSource = ResourceUtils.MakeUri("WorkspaceManager/Effects/Shaders/RandomCircleRevealTransitionEffect.ps");
			PixelShader = shader;
		}
	}
	public class RippleTransitionEffect : TransitionEffectBase {
		public RippleTransitionEffect() {
			PixelShader shader = new PixelShader();
			shader.UriSource = ResourceUtils.MakeUri("WorkspaceManager/Effects/Shaders/RippleTransitionEffect.ps");
			PixelShader = shader;
		}
	}
	public class SlideInTransitionEffect : TransitionEffectBase {
		public static readonly DependencyProperty SlideAmountProperty = DependencyProperty.Register("SlideAmount", typeof(Point), typeof(SlideInTransitionEffect), new PropertyMetadata(new Point(1.0, 0.0), PixelShaderConstantCallback(1)));
		public SlideInTransitionEffect() {
			PixelShader shader = new PixelShader();
			shader.UriSource = ResourceUtils.MakeUri("WorkspaceManager/Effects/Shaders/SlideInTransitionEffect.ps");
			PixelShader = shader;
			UpdateShaderValue(SlideAmountProperty);
		}
		public Point SlideAmount {
			get { return (Point)GetValue(SlideAmountProperty); }
			set { SetValue(SlideAmountProperty, value); }
		}
	}
	public class WaterTransitionEffect : CloudyTransitionEffectBase {
		public WaterTransitionEffect() {
			PixelShader shader = new PixelShader();
			shader.UriSource = ResourceUtils.MakeUri("WorkspaceManager/Effects/Shaders/WaterTransitionEffect.ps");
			PixelShader = shader;
		}
	}
	public class WaveTransitionEffect : TransitionEffectBase {
		public WaveTransitionEffect() {
			PixelShader shader = new PixelShader();
			shader.UriSource = ResourceUtils.MakeUri("WorkspaceManager/Effects/Shaders/WaveTransitionEffect.ps");
			PixelShader = shader;
		}
	}
	static class BitmapHelpers {
		public static ImageSource GenerateBitmap(int width, int height, Action<int[]> generator) {
#if SL
			WriteableBitmap bitmap = new WriteableBitmap(width, height);
			generator(bitmap.Pixels);
#else
			WriteableBitmap bitmap = new WriteableBitmap(width, height, 96.0, 96.0, PixelFormats.Bgra32, null);
			int[] pixels = new int[bitmap.PixelWidth * bitmap.PixelHeight];
			generator(pixels);
			Int32Rect rect = new Int32Rect(0, 0, bitmap.PixelWidth, bitmap.PixelHeight);
			int stride = bitmap.PixelWidth * bitmap.Format.BitsPerPixel / 8;
			bitmap.WritePixels(rect, pixels, stride, 0);
#endif
			return bitmap;
		}
		public static int CreateArgb32(int a, int r, int g, int b) {
			return (a << 24) | (r << 16) | (g << 8) | b;
		}
	}
}
