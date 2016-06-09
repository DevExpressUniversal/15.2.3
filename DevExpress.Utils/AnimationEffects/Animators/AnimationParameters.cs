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

using System.ComponentModel;
using DevExpress.Utils.Controls;
namespace DevExpress.Utils.Animation {
	public enum Transitions {
		Push,
		Shape,
		Fade,
		Clock,
		Dissolve,
		SlideFade,
		Cover,
		Comb
	}
	public enum PushEffectOptions {
		FromLeft,
		FromTop,
		FromRight,
		FromBottom
	}
	public enum CoverEffectOptions {
		FromLeft,
		FromTop,
		FromRight,
		FromBottom,
		FromTopRight,
		FromBottomRight,
		FromTopLeft,
		FromBottomLeft
	}
	public enum ShapeEffectOptions {
		CircleIn,
		CircleOut,
		In,
		Out
	}
	public enum ClockEffectOptions {
		Clockwise,
		Counterclockwise,
		Wedge
	}
	[TypeConverter(typeof(ExpandableObjectConverter))]
	public interface IAnimationParameters {
		int? FrameInterval { get; set; }
		int? FrameCount { get; set; }
		void Combine(IAnimationParameters defaultParameters);
	}
	[TypeConverter(typeof(ExpandableObjectConverter))]
	public class AnimationParameters : IAnimationParameters {
		public AnimationParameters() { }
		public AnimationParameters(int interval, int count) {
			FrameInterval = interval;
			FrameCount = count;
		}
		[ DefaultValue(null)]
		public int? FrameInterval { get; set; }
		[ DefaultValue(null)]
		public int? FrameCount { get; set; }
		public virtual void Combine(IAnimationParameters defaultParameters) {
			if(FrameCount == null) FrameCount = defaultParameters.FrameCount;
			if(FrameInterval == null) FrameInterval = defaultParameters.FrameInterval;
		}
		public override string ToString() {
			return OptionsHelper.GetObjectText(this);
		}
	}
	public interface ISlideFadeAnimationParameters : IPushAnimationParameters {
		System.Drawing.Color Background { get; set; }
	}
	public interface IPushAnimationParameters : IAnimationParameters {
		PushEffectOptions EffectOptions { get; set; }
	}
	public interface IShapeAnimationParameters : IAnimationParameters {
		ShapeEffectOptions EffectOptions { get; set; }
	}
	public interface IClockAnimationParameters : IAnimationParameters {
		ClockEffectOptions EffectOptions { get; set; }
	}
	public interface ICoverAnimationParameters : IAnimationParameters {
		CoverEffectOptions EffectOptions { get; set; }
		bool UnCover { get; set; }
	}
	public interface ICombAnimationParameters : IAnimationParameters {
		bool Vertical { get; set; }
		bool Inward { get; set; }
		int StripeCount { get; set; }
	}
	public class ShapeAnimationParameters : AnimationParameters, IShapeAnimationParameters {
		public ShapeAnimationParameters()
			: base() {
			EffectOptions = ShapeEffectOptions.CircleIn;
		}
		[ DefaultValue(ShapeEffectOptions.CircleIn)]
		public ShapeEffectOptions EffectOptions { get; set; }
	}
	public class PushAnimationParameters : AnimationParameters, IPushAnimationParameters {
		public PushAnimationParameters()
			: base() {
			EffectOptions = PushEffectOptions.FromLeft;
		}
		[ DefaultValue(PushEffectOptions.FromLeft)]
		public PushEffectOptions EffectOptions { get; set; }
	}
	public class SlideFadeAnimationParameters : PushAnimationParameters, ISlideFadeAnimationParameters {
		public SlideFadeAnimationParameters() : base() { }
		[ DefaultValue(typeof(System.Drawing.Color), "0, 0, 0")]
		public System.Drawing.Color Background { get; set; }
	}
	public class ClockAnimationParameters : AnimationParameters, IClockAnimationParameters {
		public ClockAnimationParameters()
			: base() {
			EffectOptions = ClockEffectOptions.Clockwise;
		}
		[ DefaultValue(ClockEffectOptions.Clockwise)]
		public ClockEffectOptions EffectOptions { get; set; }
	}
	public class CoverAnimationParameters : AnimationParameters, ICoverAnimationParameters {
		public CoverAnimationParameters()
			: base() {
			EffectOptions = CoverEffectOptions.FromLeft;
		}
		[ DefaultValue(CoverEffectOptions.FromLeft)]
		public CoverEffectOptions EffectOptions { get; set; }
		[ DefaultValue(false)]
		public bool UnCover { get; set; }
	}
	public class CombAnimationParameters : AnimationParameters, ICombAnimationParameters {
		int stripeCountCore;
		public CombAnimationParameters()
			: base() {
			stripeCountCore = 10;
		}
		[ DefaultValue(false)]
		public bool Vertical { get; set; }
		[ DefaultValue(false)]
		public bool Inward { get; set; }
		[ DefaultValue(10)]
		public int StripeCount {
			get { return stripeCountCore; }
			set { stripeCountCore = value; }
		}
	}
}
