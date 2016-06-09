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
namespace DevExpress.Xpf.Layout.Core {
	public static class TabHeaderHelper {
		public static Rect Arrange(Rect rect, bool horz, double offset, Size size) {
			return new Rect(horz ? offset : rect.Left, horz ? rect.Top : offset, size.Width, size.Height);
		}
		public static Size GetSize(bool horz, double length, double dim) {
			return new Size(horz ? length : dim, horz ? dim : length);
		}
		public static double GetLength(bool horz, Rect rect) {
			return horz ? rect.Width : rect.Height;
		}
		public static double GetLength(bool horz, Size size) {
			return horz ? size.Width : size.Height;
		}
		public static Size GetSize(ITabHeaderInfo info, bool horz) {
			double length = GetLength(info, horz);
			return horz ? new Size(length, info.DesiredSize.Height) : 
				new Size(info.DesiredSize.Width, length);
		}
		public static double GetLength(ITabHeaderInfo info, bool horz) {
			double length = GetLength(horz, info.DesiredSize);
			if(!info.ShowCaptionImage)
				length -= (GetLength(horz, info.CaptionImage) + info.CaptionImageToCaptionDistance);
			if(!info.ShowCaption)
				length -= (GetLength(horz, info.CaptionText) + info.CaptionToControlBoxDistance);
			if(length < 0)
				length = 0;
			return length;
		}
		public static Size GetScaledSize(ITabHeaderInfo info, bool horz, double factor, ref double summaryRounding) {
			double length = GetLength(horz, info.DesiredSize);
			double captionLength = GetLength(horz, info.CaptionText);
			double captionScaledLength = RoundSummary(captionLength * factor, ref summaryRounding);
			length = info.IsPinned ? length : System.Math.Max(length + (captionScaledLength - captionLength), 0);
			return horz ? new Size(length, info.DesiredSize.Height) : new Size(info.DesiredSize.Width, length);
		}
		static double RoundSummary(double value, ref double summaryRounding) {
			double dResult = value + summaryRounding;
			double result = (double)(int)(dResult + 0.5);
			summaryRounding = dResult - result;
			return result;
		}
	}
}
