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
using System.ComponentModel;
namespace DevExpress.Utils.Animation {
	public class TransitionAnimatorTypeConverter : ExpandableObjectConverter {
		public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType) {
			if(context != null && context.Instance != null && value is ITransitionAnimator)
				return ConvertToString(value as ITransitionAnimator);
			return base.ConvertTo(context, culture, value, destinationType);
		}
		public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType) {
			return true;
		}
		public override object ConvertFrom(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value) {
			Transitions code = (Transitions)TypeDescriptor.GetConverter(typeof(Transitions)).ConvertFrom(value);
			BaseTransition generator = CreateInstance(code);
			return generator;
		}		
		public override bool GetStandardValuesSupported(ITypeDescriptorContext context) {
			return true;
		}
		public override TypeConverter.StandardValuesCollection GetStandardValues(ITypeDescriptorContext context) {
			return new StandardValuesCollection(TypeDescriptor.GetConverter(typeof(Transitions)).GetStandardValues());
		}
		public override bool GetStandardValuesExclusive(ITypeDescriptorContext context) {
			return true;
		}
		BaseTransition CreateInstance(Transitions transition) {
			switch(transition) {				
				case Transitions.Dissolve: return new DissolveTransition();
				case Transitions.Fade: return new FadeTransition();
				case Transitions.Shape: return new ShapeTransition();
				case Transitions.Clock: return new ClockTransition();
				case Transitions.SlideFade: return new SlideFadeTransition();
				case Transitions.Cover: return new CoverTransition();
				case Transitions.Comb: return new CombTransition();
				default: return new PushTransition();
			}
		}
		string ConvertToString(ITransitionAnimator animator) {
			if(animator == null) return string.Empty;
			return animator.Title;
		}
	}
}
