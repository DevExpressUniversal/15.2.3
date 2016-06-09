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
using System.Windows.Media;
using DevExpress.Xpf.Utils;
namespace DevExpress.Xpf.Charts {
	public abstract class PredefinedRangeBar2DModelControl : PredefinedModelControl {
	}
	public class BorderlessGradientRangeBar2DModelControl : PredefinedRangeBar2DModelControl {
		public BorderlessGradientRangeBar2DModelControl() {
			DefaultStyleKey = typeof(BorderlessGradientRangeBar2DModelControl);
		}
	}
	public class BorderlessSimpleRangeBar2DModelControl : PredefinedRangeBar2DModelControl {
		public BorderlessSimpleRangeBar2DModelControl() {
			DefaultStyleKey = typeof(BorderlessSimpleRangeBar2DModelControl);
		}
	}
	public class GradientRangeBar2DModelControl : PredefinedRangeBar2DModelControl {
		public GradientRangeBar2DModelControl() {
			DefaultStyleKey = typeof(GradientRangeBar2DModelControl);
		}
	}
	public class SimpleRangeBar2DModelControl : PredefinedRangeBar2DModelControl {
		public SimpleRangeBar2DModelControl() {
			DefaultStyleKey = typeof(SimpleRangeBar2DModelControl);
		}
	}
	public class TransparentRangeBar2DModelControl : PredefinedRangeBar2DModelControl {
		public TransparentRangeBar2DModelControl() {
			DefaultStyleKey = typeof(TransparentRangeBar2DModelControl);
		}
	}
	public class FlatRangeBar2DModelControl : PredefinedRangeBar2DModelControl {
		public FlatRangeBar2DModelControl() {
			DefaultStyleKey = typeof(FlatRangeBar2DModelControl);
		}
	}
	public class FlatGlassRangeBar2DModelControl : PredefinedRangeBar2DModelControl {
		public FlatGlassRangeBar2DModelControl() {
			DefaultStyleKey = typeof(FlatGlassRangeBar2DModelControl);
		}
	}
	public class OutsetRangeBar2DModelControl : PredefinedRangeBar2DModelControl {
		public OutsetRangeBar2DModelControl() {
			DefaultStyleKey = typeof(OutsetRangeBar2DModelControl);
		}
	}
}
