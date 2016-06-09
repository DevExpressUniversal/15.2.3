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
namespace DevExpress.XtraPrinting.Native.Presentation {
	public class TileEffect : ShaderEffect {
		public static readonly DependencyProperty InputProperty = ShaderEffect.RegisterPixelShaderSamplerProperty("Input", typeof(TileEffect), 0, SamplingMode.NearestNeighbor);
		public static readonly DependencyProperty TileCountProperty = DependencyProperty.Register("TileCount", typeof(Point), typeof(TileEffect), new PropertyMetadata(new Point(2, 2), PixelShaderConstantCallback(1)));
		public static readonly DependencyProperty OpacityProperty = DependencyProperty.Register("Opacity", typeof(double), typeof(TileEffect), new PropertyMetadata(1.0, PixelShaderConstantCallback(2)));
		public TileEffect() {
			PixelShader pixelShader = new PixelShader();
			pixelShader.UriSource = new Uri(string.Format("/{0};component/Native/Presentation/TileEffect.ps", AssemblyInfo.SRAssemblyPrintingCore), UriKind.RelativeOrAbsolute);
			this.PixelShader = pixelShader;
			this.UpdateShaderValue(InputProperty);
			this.UpdateShaderValue(TileCountProperty);
			this.UpdateShaderValue(OpacityProperty);
		}
		public Brush Input {
			get {
				return ((Brush)(this.GetValue(InputProperty)));
			}
			set {
				this.SetValue(InputProperty, value);
			}
		}
		public Point TileCount {
			get {
				return ((Point)(this.GetValue(TileCountProperty)));
			}
			set {
				this.SetValue(TileCountProperty, value);
			}
		}
		public double Opacity {
			get {
				return ((double)(this.GetValue(OpacityProperty)));
			}
			set {
				this.SetValue(OpacityProperty, value);
			}
		}
	}
}
