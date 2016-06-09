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
using DevExpress.Office.Utils;
namespace DevExpress.Office.Drawing {
	#region ISupportsInvalidate
	public interface ISupportsInvalidate {
		void Invalidate();
	}
	#endregion
	#region ISupportsInvalidateNotify
	public interface ISupportsInvalidateNotify {
		void InvalidateNotify();
	}
	#endregion
	#region InvalidateProxy
	public class InvalidateProxy : ISupportsInvalidate {
		ISupportsInvalidate target = null;
		ISupportsInvalidateNotify notifyTarget = null;
		public ISupportsInvalidate Target { get { return target; } set { target = value; } }
		public ISupportsInvalidateNotify NotifyTarget { get { return notifyTarget; } set { notifyTarget = value; } }
		#region ISupportsInvalidate Members
		public void Invalidate() {
			if (target != null) {
				target.Invalidate();
				if (notifyTarget != null)
					notifyTarget.InvalidateNotify();
			}
		}
		#endregion
	}
	#endregion
	#region DrawingValueConstants
	public static class DrawingValueConstants {
		public const int ThousandthOfPercentage = 100000;
		public const long MinCoordinate = -27273042329600;
		public const long MaxCoordinate = 27273042316900;
		public const int OnePositiveFixedAngle = 60000;
		public const int MaxPositiveFixedAngle = 21600000;
		public const int MaxFixedAngle = 5400000;
		public const int MaxFOVAngle = 10800000;
		public const int MaxOutlineWidth = 20116800;
		public const int MaxPositiveCoordinate32 = 51206400;
		public const int MinTextIndentLevel = -2;
		public const int MaxTextIndentLevel = 8;
		public const int MaxTextSpacingPercent = 13200000;
		public const int MaxTextSpacingPoints = 158400;
		public const int MaxStartAtNumValue = short.MaxValue;
		public const int MinTextBulletSizePercent = 25000;
		public const int MaxTextBulletSizePercent = 400000;
		public const int MinTextBulletSizePoints = 100;
		public const int MaxTextBulletSizePoints = 400000;
		public const int MaxWidthInPoints = 1584;
		public const int EmusInPoint = 12700;
	}
	#endregion
	#region DrawingValueChecker
	public static class DrawingValueChecker {
		public static void CheckPositiveFixedPercentage(int value, string propertyName) {
			ValueChecker.CheckValue(value, 0, DrawingValueConstants.ThousandthOfPercentage, propertyName);
		}
		public static void CheckFixedPercentage(int value, string propertyName) {
			ValueChecker.CheckValue(value, -DrawingValueConstants.ThousandthOfPercentage, DrawingValueConstants.ThousandthOfPercentage, propertyName);
		}
		public static void CheckPositiveCoordinate(long value, string propertyName) {
			ValueChecker.CheckValue(value, 0, DrawingValueConstants.MaxCoordinate, propertyName);
		}
		public static void CheckCoordinate(long value, string propertyName) {
			ValueChecker.CheckValue(value, DrawingValueConstants.MinCoordinate, DrawingValueConstants.MaxCoordinate, propertyName);
		}
		public static void CheckPositiveFixedAngle(int value, string propertyName) {
			ValueChecker.CheckValue(value, 0, DrawingValueConstants.MaxPositiveFixedAngle, propertyName);
		}
		public static void CheckFixedAngle(int value, string propertyName) {
			ValueChecker.CheckValue(value, -DrawingValueConstants.MaxFixedAngle, DrawingValueConstants.MaxFixedAngle, propertyName);
		}
		public static void CheckFOVAngle(int value, string propertyName) {
			ValueChecker.CheckValue(value, 0, DrawingValueConstants.MaxFOVAngle, propertyName);
		}
		public static void CheckOutlineWidth(int value, string propertyName) {
			ValueChecker.CheckValue(value, 0, DrawingValueConstants.MaxOutlineWidth, propertyName);
		}
		public static void CheckCoordinate32(int value, string propertyName) {
			ValueChecker.CheckValue(value, -DrawingValueConstants.MaxPositiveCoordinate32, DrawingValueConstants.MaxPositiveCoordinate32, propertyName);
		}
		public static void CheckPositiveCoordinate32(int value, string propertyName) {
			ValueChecker.CheckValue(value, 0, DrawingValueConstants.MaxPositiveCoordinate32, propertyName);
		}
		public static void CheckTextIndentLevelValue(int value) {
			ValueChecker.CheckValue(value, DrawingValueConstants.MinTextIndentLevel, DrawingValueConstants.MaxTextIndentLevel, "TextIndentLevel");
		}
		public static void CheckTextSpacingPercentValue(int value) {
			ValueChecker.CheckValue(value, 0, DrawingValueConstants.MaxTextSpacingPercent, "TextSpacingPercent");
		}
		public static void CheckTextSpacingPointsValue(int value) {
			ValueChecker.CheckValue(value, 0, DrawingValueConstants.MaxTextSpacingPoints, "TextSpacingPoints");
		}
		public static void CheckTextBulletStartAtNumValue(int value) {
			ValueChecker.CheckValue(value, 1, DrawingValueConstants.MaxStartAtNumValue, "TextBulletStartAtNum");
		}
		public static void CheckTextBulletSizePercentValue(int value) {
			ValueChecker.CheckValue(value, DrawingValueConstants.MinTextBulletSizePercent, DrawingValueConstants.MaxTextBulletSizePercent, "TextBulletSizePercent");
		}
		public static void CheckTextBulletSizePointsValue(int value) {
			ValueChecker.CheckValue(value, DrawingValueConstants.MinTextBulletSizePoints, DrawingValueConstants.MaxTextBulletSizePoints, "TextBulletSizePoints");
		}
	}
	#endregion
	#region DrawingValueConverter
	public static class DrawingValueConverter {
		public static int ToPercentage(double value) {
			return (int)(value * DrawingValueConstants.ThousandthOfPercentage);
		}
		public static double FromPercentage(int value) {
			return value / (double)DrawingValueConstants.ThousandthOfPercentage;
		}
		public static int ToPositiveFixedAngle(double value) {
			return (int)(value * DrawingValueConstants.OnePositiveFixedAngle);
		}
		public static double FromPositiveFixedAngle(int value) {
			return value / (double)DrawingValueConstants.OnePositiveFixedAngle;
		}
	}
	#endregion
}
