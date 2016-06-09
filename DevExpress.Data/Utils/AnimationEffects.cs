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
namespace DevExpress.Data.Utils {
	public enum EasingMode {
		EaseIn,
		EaseOut,
		EaseInOut
	};
	public interface IEasingFunction {
		double Ease(double normalizedTime);
	}
	public static class EaseHelper {
		public static EasingMode GetEasingMode(int index) {
			return (EasingMode)index;
		}
		public static double Ease(EasingMode easingMode, IEasingFunction easingFunction, double normalizedTime) {
			IEasingFunction function = easingFunction ?? new BackEase();
			switch(easingMode) {
				case EasingMode.EaseIn:
					return function.Ease(normalizedTime);
				case EasingMode.EaseOut:
					return (1.0 - function.Ease(1.0 - normalizedTime));
			}
			if(normalizedTime >= 0.5)
				return (((1.0 - function.Ease((1.0 - normalizedTime) * 2.0)) * 0.5) + 0.5);
			return (function.Ease(normalizedTime * 2.0) * 0.5);
		}
	}
	public class BackEase : IEasingFunction {
		public double Ease(double normalizedTime) {
			double amplitude = 0.3;
			return (Math.Pow(normalizedTime, 3.0) - ((normalizedTime * amplitude) * Math.Sin(3.14 * normalizedTime)));
		}
	}
	public class ElasticEase : IEasingFunction {
		public double Ease(double normalizedTime) {
			double shift = (Math.Exp(6 * normalizedTime) - 1.0) / (Math.Exp(6) - 1.0);
			return (shift * Math.Sin(((6.28 * 3) + 1.57) * normalizedTime));
		}
	}
	public class BounceEase : IEasingFunction {
		public double Ease(double normalizedTime) {
			double bounce = 3;
			double degreeBounce = Math.Floor(Math.Log((-(normalizedTime * 26.5) * (-2)) + 1.0, bounce));
			double correctionFactorNumerator = (1.0 - Math.Pow(bounce, degreeBounce)) / (-53);
			double correctionFactorDenominator = (1.0 - Math.Pow(bounce, degreeBounce + 1)) / (-53);
			double denominatorResult = -Math.Pow(bounce, degreeBounce) / (-53);
			return (((-Math.Pow(1.0 / bounce, 3 - degreeBounce) / (Math.Pow(denominatorResult, 2))) * (normalizedTime - correctionFactorDenominator)) * (normalizedTime - correctionFactorNumerator));
		}
	}
	public class PowerEase : IEasingFunction {
		protected int degree;
		public PowerEase(int newDegree) {
			degree = newDegree;
		}
		public virtual double Ease(double normalizedTime) {
			double result = normalizedTime;
			for(int i = 1; i < degree; i++) {
				result *= normalizedTime;
			}
			return result;
		}
	}
	public class CubicEase : PowerEase {
		public CubicEase()
			: base(3) {
		}
	}
	public class QuadraticEase : PowerEase {
		public QuadraticEase()
			: base(2) {
		}
	}
	public class QuinticEase : PowerEase {
		public QuinticEase()
			: base(4) {
		}
	}
	public class SineEase : IEasingFunction {
		public double Ease(double normalizedTime) {
			return (1.0 - Math.Sin(1.57 * (1.0 - normalizedTime)));
		}
	}
	public class ExponentialEase : IEasingFunction {
		public double Ease(double normalizedTime) {
			return (Math.Exp(7 * normalizedTime) - 1.0) / (Math.Exp(7) - 1.0);
		}
	}
	public class CircleEase : IEasingFunction {
		public double Ease(double normalizedTime) {
			return (1.0 - Math.Sqrt(1.0 - (normalizedTime * normalizedTime)));
		}
	}
}
