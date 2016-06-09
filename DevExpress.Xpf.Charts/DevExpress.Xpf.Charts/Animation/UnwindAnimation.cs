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
using DevExpress.Charts.Native;
namespace DevExpress.Xpf.Charts {
	public enum UnwindDirection {
		LeftToRight,
		RightToLeft,
		TopToBottom,
		BottomToTop
	}	
}
namespace DevExpress.Xpf.Charts.Native { 
	public static class UnwindAnimationHelper {
		public static Rect CreateAnimatedClipBounds(Rect clipBounds, double progress, UnwindDirection unwindDirection) {
			double x, y, width, height;
			switch (unwindDirection) {
				case UnwindDirection.LeftToRight:
					x = clipBounds.X;
					y = clipBounds.Y;
					width = clipBounds.Width * progress;
					height = clipBounds.Height;
					break;
				case UnwindDirection.RightToLeft:
					x = clipBounds.X + clipBounds.Width * (1.0 - progress);
					y = clipBounds.Y;
					width = clipBounds.Width * progress;
					height = clipBounds.Height;
					break;
				case UnwindDirection.TopToBottom:
					x = clipBounds.X;
					y = clipBounds.Y;
					width = clipBounds.Width;
					height = clipBounds.Height * progress;
					break;
				case UnwindDirection.BottomToTop:
					x = clipBounds.X;
					y = clipBounds.Y + clipBounds.Height * (1.0 - progress);
					width = clipBounds.Width;
					height = clipBounds.Height * progress;
					break;
				default:
					ChartDebug.Fail("Unknown unwind direction.");
					goto case UnwindDirection.LeftToRight;
			}
			return new Rect(x, y, width, height);
		}
	}
}
