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

using System.Windows;
using System.ComponentModel;
namespace DevExpress.Xpf.Carousel {
	public class Parameter : DependencyObject {
		public static readonly DependencyProperty AnimationMulFunctionProperty;
		public static readonly DependencyProperty AnimationAddFunctionProperty;
		public static readonly DependencyProperty DistributionFunctionProperty;
		public delegate void ParameterChangedHandler();
		ParameterChangedHandler ParameterChangedStorage = null;
		public event ParameterChangedHandler ParameterChanged {
			add {
				ParameterChangedStorage += value;
			}
			remove {
				ParameterChangedStorage -= value;
			}
		}
		protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e) {
			base.OnPropertyChanged(e);
			if(ParameterChangedStorage != null)
				ParameterChangedStorage();
		}
#if !SL
	[DevExpressXpfCarouselLocalizedDescription("ParameterName")]
#endif
		public string Name { get; set; }
#if !SL
	[DevExpressXpfCarouselLocalizedDescription("ParameterAnimationMulFunction")]
#endif
		public FunctionBase AnimationMulFunction {
			get { return (FunctionBase)GetValue(AnimationMulFunctionProperty); }
			set { SetValue(AnimationMulFunctionProperty, value); }
		}
#if !SL
	[DevExpressXpfCarouselLocalizedDescription("ParameterAnimationAddFunction")]
#endif
		public FunctionBase AnimationAddFunction {
			get { return (FunctionBase)GetValue(AnimationAddFunctionProperty); }
			set { SetValue(AnimationAddFunctionProperty, value); }
		}
#if !SL
	[DevExpressXpfCarouselLocalizedDescription("ParameterDistributionFunction")]
#endif
		public FunctionBase DistributionFunction {
			get { return (FunctionBase)GetValue(DistributionFunctionProperty); }
			set { SetValue(DistributionFunctionProperty, value); }
		}
#if !SL
	[DevExpressXpfCarouselLocalizedDescription("ParameterInvertDistortionOnReverse")]
#endif
		public bool InvertDistortionOnReverse { get; set; }
		static Parameter() {
			DistributionFunctionProperty = DependencyProperty.Register("DistributionFunction", typeof(FunctionBase), typeof(Parameter), new PropertyMetadata(new EqualFunction()));
			AnimationMulFunctionProperty = DependencyProperty.Register("AnimationMulFunction", typeof(FunctionBase), typeof(Parameter), new PropertyMetadata(new ConstantFunction(1d)));
			AnimationAddFunctionProperty = DependencyProperty.Register("AnimationAddFunction", typeof(FunctionBase), typeof(Parameter), new PropertyMetadata(new ConstantFunction(0d)));
		}
		public Parameter() { }
		public Parameter(string name, FunctionBase distributionFunction)
			: this(name, distributionFunction, new ConstantFunction(1d), new ConstantFunction(0d)) {
		}
		public Parameter(string name, FunctionBase distributionFunction, FunctionBase animationAddFunction)
			: this(name, distributionFunction, new ConstantFunction(1d), animationAddFunction) {
		}
		public Parameter(string name, FunctionBase distributionFunction, FunctionBase animationMulFunction, FunctionBase animationAddFunction) {
			Name = name;
			DistributionFunction = distributionFunction;
			AnimationMulFunction = animationMulFunction;
			AnimationAddFunction = animationAddFunction;
		}
	}
}
