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

using DevExpress.Xpf.Charts.Native;
namespace DevExpress.Xpf.Charts {
	public abstract class PredefinedPie2DModelControl : PredefinedModelControl, IFinishInvalidation {
	}
	public class BorderlessFlatPie2DModelControl : PredefinedPie2DModelControl {
		public BorderlessFlatPie2DModelControl() {
			DefaultStyleKey = typeof(BorderlessFlatPie2DModelControl);
		}
	}
	public class GlarePie2DModelControl : PredefinedPie2DModelControl {
		public GlarePie2DModelControl() {
			DefaultStyleKey = typeof(GlarePie2DModelControl);
		}
	}
	public class SimplePie2DModelControl : PredefinedPie2DModelControl {
		public SimplePie2DModelControl() {
			DefaultStyleKey = typeof(SimplePie2DModelControl);
		}
	}
	public class FlatPie2DModelControl : PredefinedPie2DModelControl {
		public FlatPie2DModelControl() {
			DefaultStyleKey = typeof(FlatPie2DModelControl);
		}
	}
	public class GlossyPie2DModelControl : PredefinedPie2DModelControl {
		public GlossyPie2DModelControl() {
			DefaultStyleKey = typeof(GlossyPie2DModelControl);
		}
	}
	public class GlassPie2DModelControl : PredefinedPie2DModelControl {
		public GlassPie2DModelControl() {
			DefaultStyleKey = typeof(GlassPie2DModelControl);
		}
	}
	public class CupidPie2DModelControl : PredefinedPie2DModelControl {
		public CupidPie2DModelControl() {
			DefaultStyleKey = typeof(CupidPie2DModelControl);
		}
	}
	public class CustomPie2DModelControl : CustomModelControl, IFinishInvalidation {
		public CustomPie2DModelControl() {
			DefaultStyleKey = typeof(CustomPie2DModelControl);
		}
	}
}
