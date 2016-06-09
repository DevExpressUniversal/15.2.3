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

using System.Drawing;
namespace DevExpress.XtraCharts.Native {
	public struct Limits {
		public int Start;
		public int End;
		public int Length { get { return End - Start; } }
		public Limits(int start, int end) {
			Start = start;
			End = end;
		}
	}
	public class LimitsCorrection {
		int start = 0;
		int end = 0;
		public int Start {
			get { return start; }
			set {
				if(value > 0)
					start = value;
			}
		}
		public int End {
			get { return end; }
			set {
				if(value > 0)
					end = value;
			}
		}
		public bool ShouldCorrect { get { return start > 0 || end > 0; } }
		public LimitsCorrection() {
		}
		public Limits Correct(Limits limits) {
			limits.Start += start;
			limits.End -= end;
			return limits;
		}
		public void ApplyHorizontalCorrection(RectangleCorrection correction) {
			start += correction.Left;
			end += correction.Right;
		}
		public void ApplyVerticalCorrection(RectangleCorrection correction) {
			start += correction.Top;
			end += correction.Bottom;
		}
	}
}
