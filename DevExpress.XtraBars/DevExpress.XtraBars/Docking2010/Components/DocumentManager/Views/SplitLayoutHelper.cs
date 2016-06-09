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
using System.Drawing;
namespace DevExpress.XtraBars.Docking2010.Views {
	public static class SplitLayoutHelper {
		public static Rectangle[] Calc(Rectangle bounds, int[] splitLengths, bool horizontal) {
			Rectangle[] splitters;
			return Calc(bounds, splitLengths, 0, horizontal, out splitters);
		}
		public static Rectangle[] Calc(Rectangle bounds, int[] splitLengths, int interval, bool horizontal, out Rectangle[] splitters) {
			int[] intervals = new int[splitLengths.Length - 1];
			for(int i = 0; i < intervals.Length; i++)
				intervals[i] = interval;
			return Calc(bounds, splitLengths, intervals, horizontal, out splitters);
		}
		public static Rectangle[] Calc(Rectangle bounds, int[] splitLengths, int[] intervals, bool horizontal, out Rectangle[] splitters) {
			splitters = new Rectangle[0];
			Rectangle[] result = new Rectangle[splitLengths.Length];
			if(splitLengths.Length == 0)
				return result;
			if(splitLengths.Length < 2)
				result[0] = bounds;
			else {
				splitters = new Rectangle[splitLengths.Length - 1];
				int totalInterval = 0;
				for(int i = 0; i < intervals.Length; i++) 
					totalInterval += intervals[i];
				int size = (horizontal ? bounds.Width : bounds.Height) - totalInterval;
				int notSetLengthCount = 0;
				int setLengthCount = 0;
				int totalLength = 0;
				for(int i = 0; i < splitLengths.Length; i++) {
					if(splitLengths[i] == -1) continue;
					if(splitLengths[i] == 0) notSetLengthCount++;
					else {
						setLengthCount++;
						totalLength += splitLengths[i];
					}
				}
				double e = (double)size / (double)(notSetLengthCount + setLengthCount);
				double[] lengths = new double[splitLengths.Length];
				for(int i = 0; i < lengths.Length; i++) {
					if(splitLengths[i] == -1) continue;
					if(splitLengths[i] == 0)
						lengths[i] = e;
					else
						lengths[i] = ((double)splitLengths[i] / (double)totalLength) * (double)setLengthCount * e;
				}
				int offset = 0; int length = 0;
				double round = 0;
				for(int i = 0; i < result.Length; i++) {
					double d = (double)lengths[i] + round;
					length = (int)(d + 0.5d);
					round = d - (double)length;
					if(horizontal) {
						result[i] = new Rectangle(bounds.Left + offset, bounds.Top, length, bounds.Height);
						if(i + 1 < result.Length)
							splitters[i] = new Rectangle(bounds.Left + offset + length, bounds.Top, intervals[i], bounds.Height);
					}
					else {
						result[i] = new Rectangle(bounds.Left, bounds.Top + offset, bounds.Width, length);
						if(i + 1 < result.Length)
							splitters[i] = new Rectangle(bounds.Left, bounds.Top + offset + length, bounds.Width, intervals[i]);
					}
					if(i + 1 < result.Length)
						offset += (length + intervals[i]);
				}
			}
			return result;
		}
	}
}
