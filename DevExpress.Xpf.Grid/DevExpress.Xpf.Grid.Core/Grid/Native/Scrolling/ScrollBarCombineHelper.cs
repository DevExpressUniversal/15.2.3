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
namespace DevExpress.Xpf.Grid.Native {
	public class ScrollBarCombineHelper {
		double maxRatio = 0;
		double maxInvisiblePart = 0;
		double finalViewport;
		double finalExtent;
		public void ProcessScrollInfo(double viewPort, double extent) {
			if(viewPort != 0) {
				double ratio = extent / viewPort;
				if(ratio > maxRatio) {
					maxRatio = ratio;
					finalExtent = extent;
					finalViewport = viewPort;
				}
				maxInvisiblePart = Math.Max(maxInvisiblePart, extent - viewPort);
			}
		}
		public double FinalViewport { get { return finalViewport; } }
		public double FinalExtent { get { return finalExtent; } }
		public double ConverToRealOffset(double scrollBarOffset, double viewPort, double extent) {
			if(finalExtent == finalViewport || viewPort >= extent)
				return 0;
			scrollBarOffset *= maxInvisiblePart / (finalExtent - finalViewport);
			return Math.Max(0, Math.Min(scrollBarOffset, extent - viewPort));
		}
	}
}
