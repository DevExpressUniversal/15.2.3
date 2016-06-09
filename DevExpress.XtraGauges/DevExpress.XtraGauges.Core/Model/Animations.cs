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
namespace DevExpress.XtraGauges.Core.Model {
	public enum EasingMode { 
		EaseIn, 
		EaseOut, 
		EaseInOut 
	};
	public interface IEasingFunction : DevExpress.Data.Utils.IEasingFunction { }
	public class BackEase : DevExpress.Data.Utils.BackEase, IEasingFunction { }
	public class ElasticEase : DevExpress.Data.Utils.ElasticEase, IEasingFunction { }
	public class BounceEase : DevExpress.Data.Utils.BounceEase, IEasingFunction { }
	public class PowerEase : DevExpress.Data.Utils.PowerEase, IEasingFunction {
		public PowerEase(int newDegree) : base(newDegree) { }
	}
	public class CubicEase : DevExpress.Data.Utils.CubicEase, IEasingFunction { }
	public class QuadraticEase : DevExpress.Data.Utils.QuadraticEase, IEasingFunction { }
	public class QuinticEase : DevExpress.Data.Utils.QuinticEase, IEasingFunction { }
	public class SineEase : DevExpress.Data.Utils.SineEase, IEasingFunction { }
	public class ExponentialEase : DevExpress.Data.Utils.ExponentialEase, IEasingFunction { }
	public class CircleEase : DevExpress.Data.Utils.CircleEase, IEasingFunction { }
}
