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
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Windows;
using DevExpress.Xpf.Core;
namespace DevExpress.Xpf.Carousel {
	public abstract class ParameterDescriptorBase : CustomPropertyDescriptor {
		protected ParameterDescriptorBase(string name)
			: base(name) {
		}
		public override Type PropertyType { get { return typeof(object); } }
		public override void SetValue(object component, object value) { throw new NotImplementedException(); }
		public override bool IsReadOnly { get { return true; } }
		public sealed override object GetValue(object component) {
			ParametersTypeDescriptor paramters = (ParametersTypeDescriptor)component;
			return GetValueCore(paramters.Owner, paramters.ActualPosition);
		}
		protected abstract double GetValueCore(CarouselPanel carousel, double actualPosition);
	}
	public class ParameterDescriptor : ParameterDescriptorBase {
		Parameter parameter;
		public ParameterDescriptor(Parameter param)
			: base(param.Name) {
			this.parameter = param;
		}
		public override string Name { 
			get { return parameter.Name; } 
		}
		protected override double GetValueCore(CarouselPanel carousel, double actualPosition) {
			double progress = carousel.AnimationProgress;
			double actualValue = parameter.DistributionFunction.GetValue(actualPosition);
			double multiplier = parameter.AnimationMulFunction.GetValue(progress);
			double distortion = parameter.AnimationAddFunction.GetValue(progress);
			return actualValue * multiplier + distortion;
		}
	}
	public abstract class BuiltInParameterDescriptorBase : ParameterDescriptorBase {
		string name;
		public BuiltInParameterDescriptorBase(string name)
			: base(name) {
			this.name = name;
		}
		public override string Name { get { return name; } }
	}
	public class OffsetParameterDescriptor : BuiltInParameterDescriptorBase {
		public static readonly OffsetParameterDescriptor OffsetXDescriptor = new OffsetParameterDescriptor(BuiltInParametersNames.OffsetX, 
			HorizontalSizeHelper.Instance);
		public static readonly OffsetParameterDescriptor OffsetYDescriptor = new OffsetParameterDescriptor(BuiltInParametersNames.OffsetY, 
			VerticalSizeHelper.Instance);
		SizeHelperBase sizeHelper;
		private OffsetParameterDescriptor(string name, SizeHelperBase sizeHelper)
			: base(name) {
			this.sizeHelper = sizeHelper;
		}
		protected override double GetValueCore(CarouselPanel carousel, double actualPosition) {
			Point p1, p2;
			if(carousel.IsInvertedDirection)
				actualPosition = 1d - actualPosition;
			if(carousel.TransformedPath.Bounds.Width > 0 || carousel.TransformedPath.Bounds.Height > 0) {
				if(actualPosition >= 0d && actualPosition <= 1d)
					carousel.TransformedPath.GetPointAtFractionLength(actualPosition, out p1, out p2);
				else
					p1 = new Point(0, 0);
			}
			else
				p1 = new Point(
					carousel.ActualPathPadding.Left + (carousel.CarouselSize.Width - (carousel.ActualPathPadding.Left + carousel.ActualPathPadding.Right)) / 2, 
					carousel.ActualPathPadding.Top + (carousel.CarouselSize.Height - (carousel.ActualPathPadding.Top + carousel.ActualPathPadding.Bottom)) / 2
					);
			if(carousel.SnapItemsToDevicePixels) {
				p1.X = Math.Round(p1.X);
				p1.Y = Math.Round(p1.Y);
			}
			return sizeHelper.GetDefinePoint(p1);
		}
	}
}
