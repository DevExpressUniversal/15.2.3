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
using System.Collections.Generic;
using System.Text;
using DevExpress.XtraReports.UI;
using System.Drawing;
using System.Windows.Forms.Design;
using DevExpress.XtraReports.Native;
using DevExpress.XtraPrinting;
using System.Windows.Forms;
using DevExpress.XtraPrinting.Native;
namespace DevExpress.XtraReports.Design.SnapLines {
	public enum SnapDirection { None, Left, Right, Up, Down };
	public enum SnapLineKind { LeftSide, TopSide, RightSide, BottomSide, Margin };
	public struct SnapLine {
		SnapLineKind kind;
		RectangleF bounds;
		public SnapLine(RectangleF bounds, SnapLineKind kind) { 
			this.bounds = bounds;
			this.kind = kind;
		}
		public SnapLineKind Kind {
			get { return kind; }
		}
		public RectangleF Bounds {
			get { return bounds; }
		}
	}
	public abstract class SnapLineHelper {
		public static bool IsSnappingAllowed { get { return (Control.ModifierKeys & Keys.Alt) != Keys.Alt; } }
		protected static RectangleF RectangleFToView(Band band, RectangleF rect, IServiceProvider serviceProvider) {
			RectangleF result = ZoomService.GetInstance(serviceProvider).ToScaledPixels(rect, band.Dpi);
			BandViewInfo bandViewInfo = GetBandViewInfo(serviceProvider, band);
			result.Offset(bandViewInfo.ClientBandBounds.Location);
			return result;
		}
		protected static float VertCoordinateToView(BandViewInfo viewInfo, float coordinate, IServiceProvider serviceProvider) {
			float result = ZoomService.GetInstance(serviceProvider).ToScaledPixels(coordinate, viewInfo.Band.Dpi);
			return result + viewInfo.ClientBandBounds.Top;
		}
		protected static float HorizCoordinateToView(BandViewInfo viewInfo, float coordinate, IServiceProvider serviceProvider) {
			float result = ZoomService.GetInstance(serviceProvider).ToScaledPixels(coordinate, viewInfo.Band.Dpi);
			return result + viewInfo.ClientBandBounds.Left;
		}
		protected static BandViewInfo GetBandViewInfo(IServiceProvider serviceProvider, Band band) {
			IBandViewInfoService svc = (IBandViewInfoService)serviceProvider.GetService(typeof(IBandViewInfoService));
			return svc.GetViewInfoByBand(band);
		}
		protected delegate float GetFloatMethod(RectangleF rectangle);
		protected delegate float[] GetMarginMethod(XRControl control);
		static int magicSnapNumber = 10;
		static SnapLineHelper CreateSnapLineHelper(XRControl control, RectangleF controlBounds, Band parentBand, SnapDirection direction) {
			switch(direction) {
				case SnapDirection.Left:
					return new SnapLineLeft(control, controlBounds, parentBand);
				case SnapDirection.Right:
					return new SnapLineRight(control, controlBounds, parentBand);
				case SnapDirection.Up:
					return new SnapLineUp(control, controlBounds, parentBand);
				case SnapDirection.Down:
					return new SnapLineDown(control, controlBounds, parentBand);
			}
			return new NullSnapLineHelper(control, controlBounds);
		}
		public static PointF GetNextSnapPosition(XRControl control, SnapDirection direction) { 
			return GetNextSnapPosition(control, new XRControl[] {}, direction);
		}
		public static PointF GetNextSnapPosition(XRControl control, XRControl[] controlsToExclude, SnapDirection direction) {
			SnapLineHelper snapLineHelper = CreateSnapLineHelper(control, control.BoundsRelativeToBand, control.Band,  direction);
			return snapLineHelper.GetNextSnapPosition(controlsToExclude);
		}
		public static SnapLine[] GetSnapLines(XRControl control, IServiceProvider serviceProvider) {
			return GetSnapLines(control, control.BoundsF, control.Band, serviceProvider, new XRControl[] { });
		}
		public static SnapLine[] GetSnapLines(XRControl control, RectangleF controlRect, Band parentBand, IServiceProvider serviceProvider, XRControl[] controlsToExclude, XtraReport rootReport) {
			SnapLineHelper snapLineHelper = new SnapLineHorizontal(control, controlRect, parentBand, rootReport);
			SnapLine[] horizontalSnapLines = snapLineHelper.GetSnapLines(serviceProvider, controlsToExclude);
			snapLineHelper = new SnapLineVertical(control, controlRect, parentBand, rootReport);
			SnapLine[] verticalSnapLines = snapLineHelper.GetSnapLines(serviceProvider, controlsToExclude);
			List<SnapLine> result = new List<SnapLine>();
			result.AddRange(horizontalSnapLines);
			result.AddRange(verticalSnapLines);
			return result.ToArray();
		}
		public static SnapLine[] GetSnapLines(XRControl control, RectangleF controlRect, Band parentBand, IServiceProvider serviceProvider, XRControl[] controlsToExclude) {
			return GetSnapLines(control, controlRect, parentBand, serviceProvider, controlsToExclude, control.RootReport);
		}
		protected abstract RectangleF GetBoundsSnapLine(XRControl control, float coordinate, IServiceProvider serviceProvider);
		static float GetClosestFloat(float value, float[] floats, bool firstThatMore) {
			List<float> floatList = new List<float>(floats);
			floatList.Sort(delegate(float x, float y) { return firstThatMore ? y.CompareTo(x) : x.CompareTo(y); });
			for(int i = 0; i < floatList.Count; i++) {
				if(firstThatMore && FloatsComparer.Default.FirstGreaterSecond(value, floatList[i]) || (!firstThatMore && FloatsComparer.Default.FirstGreaterSecond(floatList[i], value))) {
					return floatList[i];
				}
			}
			return int.MaxValue;
		}
		static float GetSnapOffset(float[] floats, float baseFloat) {
			float result = int.MaxValue;
			foreach(float f in floats) {
				if(Math.Abs(f - baseFloat) <= magicSnapNumber && Math.Abs(result) > Math.Abs(f - baseFloat))
					result = f - baseFloat;
			}
			return result == int.MaxValue ? 0 : result;
		}
		static float GetParentRelatedCoordinate(XRControl control, float bandRelatedCoordinate, GetFloatMethod parentGetFloatMethod) {
			float result = bandRelatedCoordinate;
			XRControl parent = control.Parent;
			while(!(parent is Band)) {
				result -= parentGetFloatMethod(parent.BoundsF);
				parent = parent.Parent;
			}
			return result;
		}
		public static SizeF SnapSize(XRControl control, SnapDirection snapDirection) {
			SnapLineHelper snapLineHelper = CreateSnapLineHelper(control, control.BoundsRelativeToBand, control.Band, snapDirection);
			return snapLineHelper.SnapSize();
		}
		public static PointF GetSnapOffset(XRControl control, RectangleF controlBounds, Band parentBand, XRControl[] controlsToExclude, SelectionRules selectionRules, XtraReport rootReport) {
			SnapLineHelper snapLineHelper = new SnapLineHorizontal(control, controlBounds, parentBand, rootReport);
			float x = snapLineHelper.GetCoordinateSnapOffset(controlBounds, controlsToExclude, selectionRules);
			snapLineHelper = new SnapLineVertical(control, controlBounds, parentBand, rootReport);
			float y = snapLineHelper.GetCoordinateSnapOffset(controlBounds, controlsToExclude, selectionRules);
			return new PointF(x, y);
		}
		public static PointF GetSnapOffset(XRControl control, RectangleF controlBounds, Band parentBand, XRControl[] controlsToExclude, SelectionRules selectionRules) {
			return GetSnapOffset(control, controlBounds, parentBand, controlsToExclude, selectionRules, control.RootReport);
		}
		public static PointF GetSnapOffset(XRControl control, RectangleF rect, XRControl[] controlsToExclude) {
			return GetSnapOffset(control, rect, control.Band, controlsToExclude);
		}
		public static PointF GetSnapOffset(XRControl control, RectangleF rect, Band parentBand, XRControl[] controlsToExclude) {
			return GetSnapOffset(control, rect, parentBand, controlsToExclude, SelectionRules.AllSizeable);
		}
		static void CollectChildren(XRControl control, List<XRControl> controlList) {
			foreach(XRControl child in control.Controls) {
				controlList.Add(child);
				CollectChildren(child, controlList);
			}
		}
		protected XRControl snappedControl;
		protected RectangleF snappedControlBounds;
		protected Band parentBand;
		XtraReport rootReport;
		protected SnapLineHelper(XRControl snappedControl, RectangleF bounds, Band parentBand) : this(snappedControl, bounds, parentBand, snappedControl.RootReport) { 
		}
		protected SnapLineHelper(XRControl snappedControl, RectangleF bounds, Band parentBand, XtraReport rootReport) : this(snappedControl) {
			this.rootReport = rootReport;
			this.snappedControlBounds = bounds;
			this.parentBand = parentBand;
		}
		protected SnapLineHelper(XRControl snappedControl) {
			this.snappedControl = snappedControl;
		}
		SnapLine[] GetSnapLines(IServiceProvider serviceProvider, XRControl[] controlsToExclude) {
			XRControl[] controls = GetControlsForSnapping(controlsToExclude);
			return GetSnapLines(controls, serviceProvider);
		}
		SnapLine[] GetSnapLines(XRControl[] controls, IServiceProvider serviceProvider) {
			List<SnapLine> result = new List<SnapLine>();
			foreach(XRControl control in controls) {
				AddBoundsSnapLine(control, GetFirstCoordinate, result, serviceProvider, FirstCoordinateKind);
				AddBoundsSnapLine(control, GetSecondCoordinate, result, serviceProvider, SecondCoordinateKind);
				if(ShouldIncludeControlInMarginSnapping(control)) {
					float[] marginSnappingValues = GetFirstCoordinateMarginSnappingValue(control);
					foreach(float value in marginSnappingValues)
						AddMarginSnapLine(control, value, GetFirstCoordinate, result, serviceProvider);
					marginSnappingValues = GetSecondCoordinateMarginSnappingValue(control);
					foreach(float value in marginSnappingValues)
						AddMarginSnapLine(control, value, GetSecondCoordinate, result, serviceProvider);
				}
			}
			return result.ToArray();
		}
		bool ShouldIncludeControlInMarginSnapping(XRControl control) {
			return parentBand == control.Band && ShouldIncludeControlInMarginInternal(control.BoundsRelativeToBand);
		}
		void AddMarginSnapLine(XRControl control, float marginValue, GetFloatMethod getCoordinate, List<SnapLine> result, IServiceProvider serviceProvider) {
			float coordinate = getCoordinate(snappedControlBounds);
			if(FloatsComparer.Default.FirstEqualsSecond(coordinate, marginValue))
				result.Add(new SnapLine(GetMarginSnapLine(control.BoundsRelativeToBand, getCoordinate, serviceProvider), SnapLineKind.Margin));
		}
		void AddBoundsSnapLine(XRControl control, GetFloatMethod getCoordinate, List<SnapLine> result, IServiceProvider serviceProvider, SnapLineKind snapLineKind) {
			float coordinate = getCoordinate(snappedControlBounds);
			float bandRelatedCoordinate = getCoordinate(control.BoundsRelativeToBand);
			if(FloatsComparer.Default.FirstEqualsSecond(bandRelatedCoordinate, coordinate)) {
				RectangleF bounds = GetBoundsSnapLine(control, coordinate, serviceProvider);
				if(bounds != RectangleF.Empty)
					result.Add(new SnapLine(bounds, snapLineKind));
			}
		}
		float[] GetBandRelatedCoordinatesList(XRControl[] controls, GetFloatMethod getFloatMethod, GetMarginMethod getMarginMethod) {
			List<float> result = new List<float>();
			foreach(XRControl control in controls) {
				float bandRelatedCoordinate = getFloatMethod(control.BoundsRelativeToBand);
				if(!result.Contains(bandRelatedCoordinate))
					result.Add(bandRelatedCoordinate);
				if(ShouldIncludeControlInMarginSnapping(control)) {
					float[] marginSnappingValues = getMarginMethod(control);
					foreach(float value in marginSnappingValues) {
						if(!result.Contains(value))
							result.Add(value);
					}	   
				}
			}
			return result.ToArray();
		}
		float GetCoordinateSnapOffset(RectangleF bounds, XRControl[] controlsToExclude, SelectionRules selectionRules) {
			XRControl[] controls = GetControlsForSnapping(controlsToExclude);
			float[] firstCoordinates = ShouldCollectFirstCoordinates(selectionRules) ? GetBandRelatedCoordinatesList(controls, GetFirstCoordinate, GetFirstCoordinateMarginSnappingValue) : new float[] { };
			float[] secondCoordinates = ShouldCollectSecondCoordinates(selectionRules) ? GetBandRelatedCoordinatesList(controls, GetSecondCoordinate, GetSecondCoordinateMarginSnappingValue) : new float[] { };
			float firstCoordinateOffset = GetSnapOffset(firstCoordinates, GetFirstCoordinate(bounds));
			float secondCoordinateOffset = GetSnapOffset(secondCoordinates, GetSecondCoordinate(bounds));
			if(firstCoordinateOffset == 0 || secondCoordinateOffset == 0)
				return firstCoordinateOffset == 0 ? secondCoordinateOffset : firstCoordinateOffset;
			return Math.Abs(firstCoordinateOffset) <= Math.Abs(secondCoordinateOffset) ? firstCoordinateOffset : secondCoordinateOffset;
		}
		XRControl[] GetControlsForSnapping(XRControl[] controlsToExclude) {
			List<XRControl> excludedControls = CollectExcludedComponents(controlsToExclude);
			List<XRControl> controls = new List<XRControl>();
			NestedComponentEnumerator enumerator = new NestedComponentEnumerator(rootReport.AllBands);
			while(enumerator.MoveNext()) {
				if(!(enumerator.Current is Band && (Band)enumerator.Current != parentBand) 
					&& enumerator.Current != snappedControl && !excludedControls.Contains(enumerator.Current) && 
					enumerator.Current.Snapable) {
					if(enumerator.Current.Band == parentBand || IncludeControlsOnDifferentBandsInBoundsSnapping)
						controls.Add(enumerator.Current);
				}
			}
			if(IncludeControlsOnDifferentBandsInBoundsSnapping) {
				foreach(Band band in rootReport.AllBands) {
					AddCrossBandPrintableControls(controls, band);
				}
			} else {
				AddCrossBandPrintableControls(controls, parentBand);
			}
			return controls.ToArray();
		}
		void AddCrossBandPrintableControls(List<XRControl> controls, Band band) {
			foreach(XRCrossBandControl crossBandControl in rootReport.CrossBandControls) {
				if(crossBandControl.IsInsideBand(band)) {
					XRControl[] crossBandControlPrintableControls = crossBandControl.GetPrintableControls(band);
					controls.AddRange(crossBandControlPrintableControls);
				}
			}
		}
		List<XRControl> CollectExcludedComponents(XRControl[] controlsToExclude) {
			List<XRControl> excludedControls = new List<XRControl>(controlsToExclude != null ? controlsToExclude : new XRControl[] { });
			foreach(XRControl child in controlsToExclude) {
				CollectChildren(child, excludedControls);
			}
			CollectChildren(snappedControl, excludedControls);
			return excludedControls;
		}
		SizeF SnapSize() {
			XRControl[] controls = GetControlsForSnapping(new XRControl[] { });
			float[] secondCoordinates = GetBandRelatedCoordinatesList(controls, GetSecondCoordinate, GetSecondCoordinateMarginSnappingValue);
			float secondCoordinate = GetClosestFloat(GetSecondCoordinate(snappedControlBounds), secondCoordinates, FirstThatMove);
			if(secondCoordinate == int.MaxValue)
				return snappedControl.SizeF;
			secondCoordinate = secondCoordinate < 0 || GetFirstCoordinate(snappedControlBounds) > secondCoordinate ? GetSecondCoordinate(snappedControlBounds) : secondCoordinate;
			return GetSize(snappedControl, secondCoordinate - GetFirstCoordinate(snappedControlBounds));
		}
		float[] GetFirstCoordinateMarginSnappingValue(XRControl control) {
			List<float> result = new List<float>();
			if(!(control is Band))
				result.Add(GetSecondCoordinate(control.BoundsRelativeToBand) + GetMarginSecondCoordinate(control.SnapLineMargin) + GetMarginFirstCoordinate(snappedControl.SnapLineMargin));
			if(IncludeExtraValueForFirstCoordinateMargin && control.CanHaveChildren) {
				result.Add(GetFirstCoordinate(control.BoundsRelativeToBand) + GetMarginFirstCoordinate(control.SnapLinePadding) + GetMarginFirstCoordinate(snappedControl.SnapLineMargin));
			}
			return result.ToArray();
		}
		float[] GetSecondCoordinateMarginSnappingValue(XRControl control) {
			List<float> result = new List<float>();
			if(!(control is Band))
				result.Add(GetFirstCoordinate(control.BoundsRelativeToBand) - GetMarginFirstCoordinate(control.SnapLineMargin) - GetMarginSecondCoordinate(snappedControl.SnapLineMargin));
			if(IncludeExtraValueForSecondCoordinateMargin && control.CanHaveChildren) {
				result.Add(GetSecondCoordinate(control.BoundsRelativeToBand) - GetMarginSecondCoordinate(control.SnapLinePadding) - GetMarginSecondCoordinate(snappedControl.SnapLineMargin));
			}
			return result.ToArray();
		}
		PointF GetNextSnapPosition(XRControl[] controlsToExclude) {
			XRControl[] controls = GetControlsForSnapping(controlsToExclude);
			float[] firstCoordinates = GetBandRelatedCoordinatesList(controls, GetFirstCoordinate, GetFirstCoordinateMarginSnappingValue);
			float firstCoordinate = GetClosestFloat(GetFirstCoordinate(snappedControl.BoundsRelativeToBand), firstCoordinates, FirstThatMove);
			float[] secondCoordinates = GetBandRelatedCoordinatesList(controls, GetSecondCoordinate, GetSecondCoordinateMarginSnappingValue);
			float secondCoordinate = GetClosestFloat(GetSecondCoordinate(snappedControl.BoundsRelativeToBand), secondCoordinates, FirstThatMove);
			if(firstCoordinate == int.MaxValue && secondCoordinate == int.MaxValue)
				return snappedControl.LocationF;
			firstCoordinate = GetParentRelatedCoordinate(snappedControl, firstCoordinate, ParentGetFloatMethod);
			secondCoordinate = GetParentRelatedCoordinate(snappedControl, secondCoordinate, ParentGetFloatMethod);
			float coordinate = Math.Abs(GetFirstCoordinate(snappedControl.BoundsF) - firstCoordinate) < Math.Abs(GetSecondCoordinate(snappedControl.BoundsF) - secondCoordinate) ? firstCoordinate :
				GetFirstCoordinate(snappedControl.BoundsF) - GetSecondCoordinate(snappedControl.BoundsF) + secondCoordinate;
			return GetLocation(snappedControl, coordinate);
		}
		protected abstract SnapLineKind FirstCoordinateKind { get; }
		protected abstract SnapLineKind SecondCoordinateKind { get; }
		protected abstract bool ShouldIncludeControlInMarginInternal(RectangleF controlBounds);
		protected abstract float ParentGetFloatMethod(RectangleF rectangle);
		protected abstract float GetFirstCoordinate(RectangleF rectangle);
		protected abstract float GetSecondCoordinate(RectangleF rectangle);
		protected abstract bool FirstThatMove { get;}
		protected abstract bool IncludeControlsOnDifferentBandsInBoundsSnapping { get;}
		protected abstract float GetMarginFirstCoordinate(PaddingInfo margin);
		protected abstract float GetMarginSecondCoordinate(PaddingInfo margin);
		protected abstract bool IncludeExtraValueForFirstCoordinateMargin { get; }
		protected abstract bool IncludeExtraValueForSecondCoordinateMargin { get; }
		protected abstract PointF GetLocation(XRControl control, float coordinate);
		protected abstract SizeF GetSize(XRControl control, float coordinate);
		protected abstract RectangleF GetMarginSnapLine(RectangleF controlBounds, GetFloatMethod getCoordinate, IServiceProvider serviceProvider);
		protected abstract bool ShouldCollectFirstCoordinates(SelectionRules selectionRules);
		protected abstract bool ShouldCollectSecondCoordinates(SelectionRules selectionRules);
	}
	public abstract class SnapLineXBase : SnapLineHelper {
		protected override bool ShouldCollectFirstCoordinates(SelectionRules selectionRules) {
			return (selectionRules & SelectionRules.LeftSizeable) != 0;
		}
		protected override bool ShouldCollectSecondCoordinates(SelectionRules selectionRules) {
			return (selectionRules & SelectionRules.RightSizeable) != 0;
		}
		protected SnapLineXBase(XRControl control, RectangleF bounds, Band parentBand)
			: this(control, bounds, parentBand, control.RootReport) { 
		}
		protected SnapLineXBase(XRControl control, RectangleF bounds, Band parentBand, XtraReport rootReport)
			: base(control, bounds, parentBand, rootReport) { 
		}
		protected override float ParentGetFloatMethod(RectangleF rectangle) {
			return rectangle.Left;
		}
		protected override SnapLineKind FirstCoordinateKind { get { return SnapLineKind.LeftSide; } }
		protected override SnapLineKind SecondCoordinateKind { get { return SnapLineKind.RightSide; } }
		protected override float GetFirstCoordinate(RectangleF rectangle) {
			return rectangle.Left;
		}
		protected override float GetSecondCoordinate(RectangleF rectangle) {
			return rectangle.Right;
		}
		protected override PointF GetLocation(XRControl control, float coordinate) {
			return new PointF(coordinate, control.TopF);
		}
		protected override SizeF GetSize(XRControl control, float coordinate) {
			return new SizeF(coordinate, control.HeightF);
		}
		protected override bool IncludeControlsOnDifferentBandsInBoundsSnapping {
			get { return true; }
		}
		protected override RectangleF GetMarginSnapLine(RectangleF controlBounds, GetFloatMethod getCoordinate, IServiceProvider serviceProvider) {
			RectangleF snapLine = RectangleF.FromLTRB(
				GetMarginLeft(controlBounds, getCoordinate),
				GetMarginVerticalPos(controlBounds),
				GetMarginRight(controlBounds, getCoordinate),
				GetMarginVerticalPos(controlBounds));
			return RectangleFToView(parentBand, snapLine, serviceProvider);
		}
		float GetMarginVerticalPos(RectangleF controlBounds) {
			float f1 =  Math.Max(controlBounds.Top, snappedControlBounds.Top);
			float f2 = Math.Min(controlBounds.Bottom, snappedControlBounds.Bottom);
			return (f1 + f2) / 2;
		}
		float GetMarginLeft(RectangleF controlBounds, GetFloatMethod getCoordinate) {
			float coordinate = getCoordinate(snappedControlBounds);
			if(FloatsComparer.Default.FirstEqualsSecond(coordinate, controlBounds.Left) ||
				FloatsComparer.Default.FirstEqualsSecond(coordinate, controlBounds.Right))
				return coordinate;
			if(FloatsComparer.Default.FirstGreaterSecondLessThird(coordinate, controlBounds.Left, controlBounds.Right)) {
				float controlCoord = getCoordinate(controlBounds);
				return Math.Min(controlCoord, coordinate);
			}
			return Math.Min(controlBounds.Right, snappedControlBounds.Right);
		}
		float GetMarginRight(RectangleF controlBounds, GetFloatMethod getCoordinate) {
			float coordinate = getCoordinate(snappedControlBounds);
			if(FloatsComparer.Default.FirstEqualsSecond(coordinate, controlBounds.Left) ||
				FloatsComparer.Default.FirstEqualsSecond(coordinate, controlBounds.Right))
				return coordinate;
			if(FloatsComparer.Default.FirstGreaterSecondLessThird(coordinate, controlBounds.Left, controlBounds.Right)) {
				float controlCoord = getCoordinate(controlBounds);
				return Math.Max(controlCoord, coordinate);
			}
			return Math.Max(controlBounds.Left, snappedControlBounds.Left);
		}
		protected override RectangleF GetBoundsSnapLine(XRControl control, float coordinate, IServiceProvider serviceProvider) {
			BandViewInfo viewInfo = GetBandViewInfo(serviceProvider, control.Band);
			if(viewInfo == null)
				return RectangleF.Empty;
			float controlTop = VertCoordinateToView(viewInfo,
				viewInfo.Expanded ? control.BoundsRelativeToBand.Top : 0,
				serviceProvider);
			float controlBottom = VertCoordinateToView(viewInfo,
				viewInfo.Expanded ? control.BoundsRelativeToBand.Bottom : 0,
				serviceProvider);
			BandViewInfo parentViewInfo = GetBandViewInfo(serviceProvider, parentBand);
			if(parentViewInfo == null)
				return RectangleF.Empty;
			float snappedControlTop = VertCoordinateToView(parentViewInfo, snappedControlBounds.Top, serviceProvider);
			float snappedControlBottom = VertCoordinateToView(parentViewInfo, snappedControlBounds.Bottom, serviceProvider);
			float x = HorizCoordinateToView(parentViewInfo, coordinate, serviceProvider);
			return RectangleF.FromLTRB(x, Math.Min(controlTop, snappedControlTop), x, Math.Max(controlBottom, snappedControlBottom));
		}
		protected override bool ShouldIncludeControlInMarginInternal(RectangleF controlBounds) {
			return (snappedControlBounds.Top <= controlBounds.Top && controlBounds.Top <= snappedControlBounds.Bottom) ||
				(controlBounds.Top <= snappedControlBounds.Top && snappedControlBounds.Top <= controlBounds.Bottom);
		}
		protected override float GetMarginFirstCoordinate(PaddingInfo margin) {
			return margin.Left;
		}
		protected override float GetMarginSecondCoordinate(PaddingInfo margin) {
			return margin.Right;
		}
		protected override bool IncludeExtraValueForFirstCoordinateMargin {
			get { return false; }
		}
		protected override bool IncludeExtraValueForSecondCoordinateMargin {
			get { return false; }
		}
	}
	public class SnapLineHorizontal : SnapLineXBase {
		public SnapLineHorizontal(XRControl control, RectangleF bounds, Band parentBand, XtraReport rootReport) : base(control, bounds, parentBand, rootReport) { 
		}
		public SnapLineHorizontal(XRControl control, RectangleF bounds, Band parentBand) : this(control, bounds, parentBand, control.RootReport) {
		}
		protected override bool IncludeExtraValueForFirstCoordinateMargin {
			get { return true; }
		}
		protected override bool IncludeExtraValueForSecondCoordinateMargin {
			get { return true; }
		}
		protected override bool FirstThatMove {
			get { throw new Exception("The method or operation is not implemented."); }
		}
	}
	public class SnapLineLeft : SnapLineXBase {
		public SnapLineLeft(XRControl control, RectangleF bounds, Band parentBand)
			: base(control, bounds, parentBand) { 
		}
		protected override bool FirstThatMove {
			get { return true; }
		}
		protected override bool IncludeExtraValueForFirstCoordinateMargin {
			get { return true; }
		}
	}
	public class SnapLineRight : SnapLineXBase {
		public SnapLineRight(XRControl control, RectangleF bounds, Band parentBand)
			: base(control, bounds, parentBand) { 
		}
		protected override bool FirstThatMove {
			get { return false; }
		}
		protected override bool IncludeExtraValueForSecondCoordinateMargin {
			get { return true; }
		}
	}
	public abstract class SnapLineYBase : SnapLineHelper {
		protected SnapLineYBase(XRControl control, RectangleF bounds, Band parentBand, XtraReport rootReport)
			: base(control, bounds, parentBand, rootReport) { 
		}
		protected SnapLineYBase(XRControl control, RectangleF bounds, Band parentBand)
			: this(control, bounds, parentBand, control.RootReport) {
		}
		protected override bool ShouldCollectFirstCoordinates(SelectionRules selectionRules) {
			return (selectionRules & SelectionRules.TopSizeable) != 0;
		}
		protected override bool ShouldCollectSecondCoordinates(SelectionRules selectionRules) {
			return (selectionRules & SelectionRules.BottomSizeable) != 0;
		}
		protected override RectangleF GetBoundsSnapLine(XRControl control, float coordinate, IServiceProvider serviceProvider) {
			RectangleF snapLine = RectangleF.FromLTRB(
				Math.Min(control.BoundsRelativeToBand.Left, snappedControlBounds.Left), 
				coordinate, 
				Math.Max(control.BoundsRelativeToBand.Right, snappedControlBounds.Right),
				coordinate);
			return RectangleFToView(parentBand, snapLine, serviceProvider);
		}
		protected override RectangleF GetMarginSnapLine(RectangleF controlBounds, GetFloatMethod getCoordinate, IServiceProvider serviceProvider) {
			RectangleF marginSnapLine = RectangleF.FromLTRB(
				GetMarginHorizPos(controlBounds),
				GetMarginTop(controlBounds, getCoordinate),
				GetMarginHorizPos(controlBounds),
				GetMarginBottom(controlBounds, getCoordinate));
			return RectangleFToView(parentBand,  marginSnapLine, serviceProvider);
		}
		float GetMarginHorizPos(RectangleF controlBounds) {
			float f1 = Math.Max(controlBounds.Left, snappedControlBounds.Left);
			float f2 = Math.Min(controlBounds.Right, snappedControlBounds.Right);
			return (f1 + f2) / 2;
		}
		float GetMarginTop(RectangleF controlBounds, GetFloatMethod getCoordinate) {
			float coordinate = getCoordinate(snappedControlBounds);
			if(FloatsComparer.Default.FirstEqualsSecond(coordinate, controlBounds.Top) ||
				FloatsComparer.Default.FirstEqualsSecond(coordinate, controlBounds.Bottom))
				return coordinate;
			if(FloatsComparer.Default.FirstGreaterSecondLessThird(coordinate, controlBounds.Top, controlBounds.Bottom)) {
				float controlCoord = getCoordinate(controlBounds);
				return Math.Min(controlCoord, coordinate);
			}
			return Math.Min(controlBounds.Bottom, snappedControlBounds.Bottom);
		}
		float GetMarginBottom(RectangleF controlBounds, GetFloatMethod getCoordinate) {
			float coordinate = getCoordinate(snappedControlBounds);
			if(FloatsComparer.Default.FirstEqualsSecond(coordinate, controlBounds.Top) ||
				FloatsComparer.Default.FirstEqualsSecond(coordinate, controlBounds.Bottom))
				return coordinate;
			if(FloatsComparer.Default.FirstGreaterSecondLessThird(coordinate, controlBounds.Top, controlBounds.Bottom)) {
				float controlCoord = getCoordinate(controlBounds);
				return Math.Max(controlCoord, coordinate);
			}
			return Math.Max(controlBounds.Top, snappedControlBounds.Top);
		}
		protected override float ParentGetFloatMethod(RectangleF rectangle) {
			return rectangle.Top;
		}
		protected override SnapLineKind FirstCoordinateKind { get { return SnapLineKind.TopSide; } }
		protected override SnapLineKind SecondCoordinateKind { get { return SnapLineKind.BottomSide; } }
		protected override float GetFirstCoordinate(RectangleF rectangle) {
			return rectangle.Top;
		}
		protected override float GetSecondCoordinate(RectangleF rectangle) {
			return rectangle.Bottom;
		}
		protected override PointF GetLocation(XRControl control, float coordinate) {
			return new PointF(control.LeftF, coordinate);
		}
		protected override SizeF GetSize(XRControl control, float coordinate) {
			return new SizeF(control.WidthF, coordinate);
		}
		protected override bool IncludeControlsOnDifferentBandsInBoundsSnapping {
			get { return false; }
		}
		protected override bool ShouldIncludeControlInMarginInternal(RectangleF controlBounds) {
			return (snappedControlBounds.Left <= controlBounds.Left && controlBounds.Left <= snappedControlBounds.Right) ||
				(controlBounds.Left <= snappedControlBounds.Left && snappedControlBounds.Left <= controlBounds.Right);
		}
		protected override float GetMarginFirstCoordinate(PaddingInfo margin) {
			return margin.Top;
		}
		protected override float GetMarginSecondCoordinate(PaddingInfo margin) {
			return margin.Bottom;
		}
		protected override bool IncludeExtraValueForFirstCoordinateMargin {
			get { return false; }
		}
		protected override bool IncludeExtraValueForSecondCoordinateMargin {
			get { return false; }
		}
	}
	public class SnapLineVertical : SnapLineYBase {
		public SnapLineVertical(XRControl control, RectangleF bounds, Band parentBand)
			: this(control, bounds, parentBand, control.RootReport) {
		}
		public SnapLineVertical(XRControl control, RectangleF bounds, Band parentBand, XtraReport rootReport)
			: base(control, bounds, parentBand, rootReport) { 
		}
		protected override bool IncludeExtraValueForFirstCoordinateMargin {
			get { return true; }
		}
		protected override bool IncludeExtraValueForSecondCoordinateMargin {
			get { return true; }
		}
		protected override bool FirstThatMove {
			get { throw new Exception("The method or operation is not implemented."); }
		}
	}
	public class SnapLineUp : SnapLineYBase {
		public SnapLineUp(XRControl control, RectangleF bounds, Band parentBand)
			: base(control, bounds, parentBand) { 
		}
		protected override bool FirstThatMove {
			get { return true; }
		}
		protected override bool IncludeExtraValueForFirstCoordinateMargin {
			get { return true; }
		}
	}
	public class SnapLineDown : SnapLineYBase {
		public SnapLineDown(XRControl control, RectangleF bounds, Band parentBand)
			: base(control, bounds, parentBand) { 
		}
		protected override bool FirstThatMove {
			get { return false; }
		}
		protected override bool IncludeExtraValueForSecondCoordinateMargin {
			get { return true; }
		}
	}
	public class NullSnapLineHelper : SnapLineHelper {
		public NullSnapLineHelper(XRControl control, RectangleF bounds)
			: base(control, bounds, null) {
		}
		protected override bool ShouldIncludeControlInMarginInternal(RectangleF controlBounds) {
			throw new Exception("The method or operation is not implemented.");
		}
		protected override float ParentGetFloatMethod(RectangleF rectangle) {
			throw new Exception("The method or operation is not implemented.");
		}
		protected override SnapLineKind FirstCoordinateKind { get { throw new Exception("The method or operation is not implemented."); } }
		protected override SnapLineKind SecondCoordinateKind { get { throw new Exception("The method or operation is not implemented."); } }
		protected override float GetFirstCoordinate(RectangleF rectangle) {
			throw new Exception("The method or operation is not implemented.");
		}
		protected override float GetSecondCoordinate(RectangleF rectangle) {
			throw new Exception("The method or operation is not implemented.");
		}
		protected override bool FirstThatMove {
			get { throw new Exception("The method or operation is not implemented."); }
		}
		protected override bool IncludeControlsOnDifferentBandsInBoundsSnapping {
			get { throw new Exception("The method or operation is not implemented."); }
		}
		protected override PointF GetLocation(XRControl control, float coordinate) {
			throw new Exception("The method or operation is not implemented.");
		}
		protected override SizeF GetSize(XRControl control, float coordinate) {
			throw new Exception("The method or operation is not implemented.");
		}
		protected override float GetMarginFirstCoordinate(PaddingInfo margin) {
			throw new Exception("The method or operation is not implemented.");
		}
		protected override float GetMarginSecondCoordinate(PaddingInfo margin) {
			throw new Exception("The method or operation is not implemented.");
		}
		protected override bool IncludeExtraValueForFirstCoordinateMargin {
			get { throw new Exception("The method or operation is not implemented."); }
		}
		protected override bool IncludeExtraValueForSecondCoordinateMargin {
			get { throw new Exception("The method or operation is not implemented."); }
		}
		protected override RectangleF GetBoundsSnapLine(XRControl control, float coordinate, IServiceProvider bandViewInfoService) {
			throw new Exception("The method or operation is not implemented.");
		}
		protected override RectangleF GetMarginSnapLine(RectangleF controlBounds, GetFloatMethod getCoordinate, IServiceProvider serviceProvider) {
			throw new Exception("The method or operation is not implemented.");
		}
		protected override bool ShouldCollectFirstCoordinates(SelectionRules selectionRules) {
			throw new Exception("The method or operation is not implemented.");
		}
		protected override bool ShouldCollectSecondCoordinates(SelectionRules selectionRules) {
			throw new Exception("The method or operation is not implemented.");
		}
	} 
}
