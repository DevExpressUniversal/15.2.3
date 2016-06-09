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
	public abstract class PredefinedMarker2DModelControl : PredefinedModelControl, IFinishInvalidation { 
	}
	public class CircleMarker2DModelControl : PredefinedMarker2DModelControl {
		public CircleMarker2DModelControl() {
			DefaultStyleKey = typeof(CircleMarker2DModelControl);
		}
	}
	public class CrossMarker2DModelControl : PredefinedMarker2DModelControl {
		public CrossMarker2DModelControl() {
			DefaultStyleKey = typeof(CrossMarker2DModelControl);
		}
	}
	public class DollarMarker2DModelControl : PredefinedMarker2DModelControl {
		public DollarMarker2DModelControl() {
			DefaultStyleKey = typeof(DollarMarker2DModelControl);
		}
	}
	public class PolygonMarker2DModelControl : PredefinedMarker2DModelControl {
		public PolygonMarker2DModelControl() {
			DefaultStyleKey = typeof(PolygonMarker2DModelControl);
		}
	}
	public class RingMarker2DModelControl : PredefinedMarker2DModelControl {
		public RingMarker2DModelControl() {
			DefaultStyleKey = typeof(RingMarker2DModelControl);
		}
	}
	public class SquareMarker2DModelControl : PredefinedMarker2DModelControl {
		public SquareMarker2DModelControl() {
			DefaultStyleKey = typeof(SquareMarker2DModelControl);
		}
	}
	public class StarMarker2DModelControl : PredefinedMarker2DModelControl {
		public StarMarker2DModelControl() {
			DefaultStyleKey = typeof(StarMarker2DModelControl);
		}
	}
	public class TriangleMarker2DModelControl : PredefinedMarker2DModelControl {
		public TriangleMarker2DModelControl() {
			DefaultStyleKey = typeof(TriangleMarker2DModelControl);
		}
	}
}
