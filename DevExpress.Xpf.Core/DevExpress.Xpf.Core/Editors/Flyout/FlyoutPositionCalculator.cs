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

using DevExpress.Xpf.Core;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Editors.Helpers;
using DevExpress.Mvvm.Native;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Windows;
namespace DevExpress.Xpf.Editors.Flyout.Native {
	public class FlyoutPositionCalculator {
		Lazy<CalculatorResult> result;
		protected virtual CalculatorResult CreateResult() {
			return new CalculatorResult() { Location = new Point(), Size = Size.Empty, State = CalculationState.Calculating };
		}
		public FlyoutPositionCalculator() {
			InitializeInternal();
		}
		public IndicatorDirection ActualIndicatorDirection { get; set; }
		public Size PopupDesiredSize { get; set; }
		public Size ContentSize { get; set; }
		public CalculatorResult Result { get { return result.Value; } }
		public Size IndicatorSize { get; set; }
		public Rect ScreenRect { get; protected set; }
		protected bool AllowOutOfScreen { get; set; }
		public Rect TargetBounds { get; protected set; }
		protected VerticalAlignment VerticalAlignment { get; set; }
		protected HorizontalAlignment HorizontalAlignment { get; set; }
		public VerticalAlignment IndicatorVerticalAlignment { get; set; }
		public HorizontalAlignment IndicatorHorizontalAlignment { get; set; }
		public FlyoutPlacement Placement { get; set; }
		public Rect IndicatorTargetBounds { get; set; }
		protected CalculationState State { get { return Result.Return(x => x.State, () => CalculationState.Uninitialized); } }
		public void Initialize(Rect targetBounds, VerticalAlignment verticalAlignment, HorizontalAlignment horizontalAlignment, bool allowOutOfScreen) {
			InitializeInternal();
			TargetBounds = targetBounds;
			AllowOutOfScreen = allowOutOfScreen;
			VerticalAlignment = verticalAlignment;
			HorizontalAlignment = horizontalAlignment;
		}
		public virtual void CalcLocation() {
			ContentSize = CalcSize();
			Result.Placement = CalcResultPlacement(Placement);
			Result.Size = GetPopupSize(Result.Placement);
			Rect targetBounds = GetCorrectedTargetBounds(Result.Placement, TargetBounds);
			Point resultLocation = CalcAlignedLocationInternal(Result.Placement, targetBounds);
			ScreenRect = GetScreenRect(new Rect(resultLocation, Result.Size).Center());
			Result.Location = CorrectPostionByScreenRect(new Rect(ApplyAlignment(resultLocation, HorizontalAlignment, VerticalAlignment, targetBounds, Result.Size, Result.Placement), Result.Size));
			if (!IndicatorSize.IsEmpty) {
				Point baseIndicatorPoint = ApplyAlignment(Result.Location, IndicatorHorizontalAlignment, IndicatorVerticalAlignment, GetCorrectedTargetBounds(Result.Placement, IndicatorTargetBounds), IndicatorSize, Result.Placement);
				Result.IndicatorOffset = (Point)(CorrectPostionByScreenRect(new Rect(baseIndicatorPoint, IndicatorSize)) - Result.Location);
			}
			Result.State = CalculationState.Finished;
		}
		public virtual Rect GetScreenRect(Point point) {
			Rect screen = PopupScreenHelper.GetNearestScreenRect(point);
			Rect workingArea = System.Windows.Forms.Screen.GetWorkingArea(screen.Center().ToWinFormsPoint()).FromWinForms();
			return PopupScreenHelper.GetScaledRect(workingArea);
		}
		protected virtual FlyoutPlacement CalcResultPlacement(FlyoutPlacement defaultPlacement) {
			List<CheckPlacementResult> checks = new List<CheckPlacementResult>();
			foreach (FlyoutPlacement placement in Enum.GetValues(typeof(FlyoutPlacement))) {
				if (placement != defaultPlacement)
					checks.Add(CheckPlacement(placement));
				else
					checks.Insert(0, CheckPlacement(placement));
			}
			CheckPlacementResult fullMatch = checks.FirstOrDefault(checkResult => checkResult.IsMatch && !checkResult.IsShifted);
			if (fullMatch != null)
				return fullMatch.Placement;
			CheckPlacementResult shifted = checks.FirstOrDefault(checkResult => checkResult.IsMatch);
			if (shifted != null)
				return shifted.Placement;
			return defaultPlacement;
		}
		CheckPlacementResult CheckPlacement(FlyoutPlacement placement) {
			CheckPlacementResult result = new CheckPlacementResult { Placement = placement };
			Rect targetBounds = GetCorrectedTargetBounds(placement, TargetBounds);
			Point baseLocation = CalcAlignedLocationInternal(placement, targetBounds);
			Size currentSize = GetPopupSize(placement);
			Rect baseRect = new Rect(baseLocation, currentSize);
			Point correctedLocation = CorrectPostionByRect(baseRect, GetScreenRect(baseRect.Center()));
			if (baseLocation == correctedLocation) {
				result.IsMatch = true;
			}
			else {
				result.IsShifted = true;
				Rect intersectWithTarget = Rect.Intersect(new Rect(correctedLocation, currentSize), TargetBounds);
				result.IsMatch = !object.Equals(correctedLocation, NaN) && (intersectWithTarget.Width <= 1 || intersectWithTarget.Height <= 1);
			}
			return result;
		}
		Point ApplyAlignment(Point location, HorizontalAlignment horizontalAlignment, VerticalAlignment verticalAlignment, Rect targetBounds, Size elementSize, FlyoutPlacement placement) {
			if (placement == FlyoutPlacement.Top || placement == FlyoutPlacement.Bottom) {
				return new Point(GetX(targetBounds, elementSize, horizontalAlignment), location.Y);
			}
			if (placement == FlyoutPlacement.Left || placement == FlyoutPlacement.Right) {
				return new Point(location.X, GetY(targetBounds, elementSize, verticalAlignment));
			}
			return location;
		}
		public Point CalcLocationInternal(FlyoutPlacement placement, Rect targetBounds) {
			switch (placement) {
				case FlyoutPlacement.Bottom:
					return CalcBottomPlacementPosition(targetBounds);
				case FlyoutPlacement.Left:
					return CalcLeftPlacementPosition(targetBounds);
				case FlyoutPlacement.Right:
					return CalcRightPlacementPosition(targetBounds);
				case FlyoutPlacement.Top:
					return CalcTopPlacementPosition(targetBounds);
				default:
					throw new Exception();
			}
		}
		public Point CalcAlignedLocationInternal(FlyoutPlacement placement, Rect targetBounds) {
			return ApplyAlignment(CalcLocationInternal(placement, targetBounds), HorizontalAlignment, VerticalAlignment, targetBounds, GetPopupSize(placement), placement);
		}
		Rect GetCorrectedTargetBounds(FlyoutPlacement placement, Rect targetBounds) {
			if (IsPlacementHorizontal(placement)) {
				if (targetBounds.Width < IndicatorSize.Width) {
					return GetIncrementedX(targetBounds, IndicatorSize, IndicatorHorizontalAlignment);
				}
				else
					return targetBounds;
			}
			else {
				if (targetBounds.Height < IndicatorSize.Height) {
					return GetIncrementedY(targetBounds, IndicatorSize, IndicatorVerticalAlignment);
				}
				else
					return targetBounds;
			}
		}
		protected Size GetPopupSize(FlyoutPlacement placement) {
			if (State == CalculationState.Uninitialized)
				throw new Exception();
			bool isPlacementHorizontal = IsPlacementHorizontal(placement);
			bool isDesiredPlacementHorizontal = IsPlacementHorizontal(ActualIndicatorDirection);
			Size indicatorCorrectedSize = (isPlacementHorizontal == isDesiredPlacementHorizontal) ? IndicatorSize : new Size(IndicatorSize.Height, IndicatorSize.Width);
			Size popupSize = new Size(
				ContentSize.Width + (isPlacementHorizontal ? 0 : indicatorCorrectedSize.Width),
				ContentSize.Height + (isPlacementHorizontal ? indicatorCorrectedSize.Height : 0)
				);
			return popupSize;
		}
		bool IsPlacementHorizontal(FlyoutPlacement placement) {
			return placement == FlyoutPlacement.Top || placement == FlyoutPlacement.Bottom;
		}
		bool IsPlacementHorizontal(IndicatorDirection indicatorDirection) {
			return indicatorDirection == IndicatorDirection.Bottom || indicatorDirection == IndicatorDirection.Top;
		}
		protected void InitializeInternal() {
			 this.result = new Lazy<CalculatorResult>(() => CreateResult());
		}
		static Point NaN = new Point(double.NaN, double.NaN);
		protected virtual Point CorrectPostionByScreenRect(Rect bounds) {
			return CorrectPostionByRect(bounds, ScreenRect);
		}
		protected virtual Point CorrectPostionByRect(Rect bounds, Rect restrictRect) {
			if (AllowOutOfScreen)
				return bounds.Location;
			var res = new Point(bounds.Left, bounds.Top);
			if (bounds.Right >= restrictRect.Right)
				res.X = restrictRect.Right - bounds.Width;
			if (bounds.Bottom > restrictRect.Bottom)
				res.Y = restrictRect.Bottom - bounds.Height;
			res.X = Math.Max(restrictRect.Left, res.X);
			res.Y = Math.Max(restrictRect.Top, res.Y);
			return res;
		}
		protected virtual Point CalcBottomPlacementPosition(Rect targetBounds) {
			double y = targetBounds.Bottom;
			return new Point(targetBounds.Left, y);
		}
		protected virtual Point CalcLeftPlacementPosition(Rect targetBounds) {
			double x = targetBounds.Left - GetPopupSize(FlyoutPlacement.Left).Width;
			return new Point(x, targetBounds.Top);
		}
		protected virtual Point CalcRightPlacementPosition(Rect targetBounds) {
			double x = targetBounds.Right;
			double y = targetBounds.Top;
			return new Point(x, y);
		}
		protected virtual Point CalcTopPlacementPosition(Rect targetBounds) {
			double y = targetBounds.Top - GetPopupSize(FlyoutPlacement.Top).Height;
			return new Point(targetBounds.Left, y);
		}
		public virtual Size CalcSize() {
			double height = VerticalAlignment == VerticalAlignment.Stretch ? TargetBounds.Height : PopupDesiredSize.Height;
			double width = HorizontalAlignment == HorizontalAlignment.Stretch ? TargetBounds.Width : PopupDesiredSize.Width;
			if (double.IsInfinity(height) || double.IsInfinity(width)) {
				return new Size();
			}
			return new Size(width, height);
		}
		protected double GetY(Rect targetBounds, Size elementSize, VerticalAlignment verticalAlignment) {
			if (verticalAlignment == VerticalAlignment.Top)
				return targetBounds.Top;
			if (verticalAlignment == VerticalAlignment.Bottom)
				return targetBounds.Bottom - elementSize.Height;
			if (verticalAlignment == VerticalAlignment.Center || verticalAlignment == VerticalAlignment.Stretch)
				return targetBounds.Top + (targetBounds.Height - elementSize.Height) / 2d;
			return targetBounds.Top;
		}
		protected double GetX(Rect targetBounds, Size elementSize, HorizontalAlignment horizontalAlignment) {
			if (horizontalAlignment == HorizontalAlignment.Left)
				return targetBounds.Left;
			if (horizontalAlignment == HorizontalAlignment.Right)
				return targetBounds.Right - elementSize.Width;
			if (horizontalAlignment == HorizontalAlignment.Center || horizontalAlignment == HorizontalAlignment.Stretch)
				return targetBounds.Left + (targetBounds.Width - elementSize.Width) / 2d;
			return targetBounds.Left;
		}
		protected Rect GetIncrementedY(Rect targetBounds, Size elementSize, VerticalAlignment verticalAlignment) {
			if (verticalAlignment == VerticalAlignment.Top) {
				double y = targetBounds.Top - elementSize.Height / 2d;
				return new Rect(targetBounds.X, y, targetBounds.Width, elementSize.Height);
			}
			if (verticalAlignment == VerticalAlignment.Bottom) {
				double y = targetBounds.Bottom - elementSize.Height / 2d;
				return new Rect(targetBounds.X, y, targetBounds.Width, elementSize.Height);
			}
			if (verticalAlignment == VerticalAlignment.Center || verticalAlignment == VerticalAlignment.Stretch) {
				double y = targetBounds.Top + (targetBounds.Height - elementSize.Height) / 2d;
				return new Rect(targetBounds.X, y, targetBounds.Width, elementSize.Height);
			}
			return targetBounds;
		}
		protected Rect GetIncrementedX(Rect targetBounds, Size elementSize, HorizontalAlignment horizontalAlignment) {
			if (horizontalAlignment == HorizontalAlignment.Left) {
				double x = targetBounds.Left - elementSize.Width / 2d;
				return new Rect(x, targetBounds.Y, elementSize.Width, targetBounds.Height);
			}
			if (horizontalAlignment == HorizontalAlignment.Right) {
				double x = targetBounds.Right - elementSize.Width / 2d;
				return new Rect(x, targetBounds.Y, elementSize.Width, targetBounds.Height);
			}
			if (horizontalAlignment == HorizontalAlignment.Center || horizontalAlignment == HorizontalAlignment.Stretch) {
				double x = targetBounds.Left + (targetBounds.Width - elementSize.Width) / 2d;
				return new Rect(x, targetBounds.Y, elementSize.Width, targetBounds.Height);
			}
			return targetBounds;
		}
	}
	class CheckPlacementResult {
		public FlyoutPlacement Placement { get; set; }
		public bool IsMatch { get; set; }
		public bool IsShifted { get; set; }
	}
}
