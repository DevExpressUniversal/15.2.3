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
using System.Drawing.Drawing2D;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DevExpress.Utils.Drawing;
using System.Windows.Forms;
using System.ComponentModel;
using DevExpress.Compatibility.System.Drawing.Drawing2D;
using DevExpress.Compatibility.System.Drawing;
using DevExpress.Compatibility.System.ComponentModel;
using DevExpress.Compatibility.System.Windows.Forms;
namespace DevExpress.XtraEditors {
	public interface IRangeControl {
		int CalcX(double normalizedValue);
		Color BorderColor { get; }
		Color RulerColor { get; }
		Color LabelColor { get; }
		Matrix NormalTransform { get; }
		double VisibleRangeStartPosition { get; }
		double VisibleRangeWidth { get; }
		RangeControlRange SelectedRange { get; set; }
		void CenterSelectedRange();
		bool AnimateOnDataChange { get; set; }
		bool IsValidValue(object value);
		void OnRangeMinimumChanged(object range);
		void OnRangeMaximumChanged(object range);
		double ConstrainRangeMinimum(double value);
		double ConstrainRangeMaximum(double value);
		IRangeControlClient Client { get; set; }
	}
	public interface IRangeControlClient {
		void DrawContent(RangeControlPaintEventArgs e);
		bool DrawRuler(RangeControlPaintEventArgs e);
		int RangeBoxTopIndent { get; }
		int RangeBoxBottomIndent { get; }
		void ValidateRange(NormalizedRangeInfo info);
		double GetNormalizedValue(object value);
		object GetValue(double normalizedValue);
		bool IsCustomRuler { get; }
		List<object> GetRuler(RulerInfoArgs e);
		object RulerDelta { get; }
		double NormalizedRulerDelta { get; }
		string RulerToString(int ruleIndex);
		bool SupportOrientation(Orientation orientation);
		void OnRangeChanged(object rangeMinimum, object rangeMaximum);
		event ClientRangeChangedEventHandler RangeChanged;
		object GetOptions();
		bool IsValid { get; }
		string InvalidText { get; }
		void UpdateHotInfo(RangeControlHitInfo hitInfo);
		void UpdatePressedInfo(RangeControlHitInfo hitInfo);
		void OnClick(RangeControlHitInfo hitInfo);
		void OnRangeControlChanged(IRangeControl rangeControl);
		bool IsValidType(Type type);
		double ValidateScale(double newScale);
		void OnResize();
		void Calculate(Rectangle contentRect);
		string ValueToString(double normalizedValue);
	}
	public interface IRangeControlClientExtension : IRangeControlClient {
		object NativeValue(double normalizedValue);
	}
	public class RangeControlRange {
		public RangeControlRange() : this(null, null) { }
		public RangeControlRange(object minimum, object maximum) {
			Minimum = minimum;
			Maximum = maximum;
		}
		public IRangeControl Owner { get; set; }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public void InternalSetMinimum(object value) {
			this.rangeMinimum = value;
		}
		internal object rangeMinimum;
		[DefaultValue(null)]
#if !DXPORTABLE
		[Editor(typeof(DevExpress.Utils.Editors.UIObjectEditor), typeof(System.Drawing.Design.UITypeEditor))]
		[TypeConverter(typeof(DevExpress.Utils.Editors.ObjectEditorTypeConverter))]
#endif
		public object Minimum {
			get { return rangeMinimum; }
			set {
				if(Owner != null && !Owner.IsValidValue(value)) {
					throw new ArgumentException(value.GetType() + " is not supported for RangeMinimum");
				}
				if(Minimum == value)
					return;
				rangeMinimum = value;
				OnRangeMinimumChanged();
			}
		}
		protected virtual void OnRangeMinimumChanged() {
			if(Owner != null)
				Owner.OnRangeMinimumChanged(this);
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public void InternalSetMaximum(object value) {
			this.rangeMaximum = value;
		}
		internal object rangeMaximum;
		[DefaultValue(null)]
#if !DXPORTABLE
		[Editor(typeof(DevExpress.Utils.Editors.UIObjectEditor), typeof(System.Drawing.Design.UITypeEditor))]
		[TypeConverter(typeof(DevExpress.Utils.Editors.ObjectEditorTypeConverter))]
#endif
		public object Maximum {
			get { return rangeMaximum; }
			set {
				if(Owner != null && !Owner.IsValidValue(value)) {
					throw new ArgumentException(value.GetType() + " is not supported for RangeMaxumum");
				}
				if(Maximum == value)
					return;
				rangeMaximum = value;
				OnRangeMaximumChanged();
			}
		}
		protected virtual void OnRangeMaximumChanged() {
			if(Owner != null)
				Owner.OnRangeMaximumChanged(this);
		}
		public bool ShouldSerialize() {
			return Maximum != null || Minimum != null;
		}
		public void Reset() {
			this.rangeMaximum = null;
			this.rangeMinimum = null;
		}
	}
	public class RangeControlPaintEventArgs : EventArgs {
		public IRangeControl RangeControl { get; set; }
		public IGraphicsCache Cache { get; set; }
		public Graphics Graphics { get { return Cache.Graphics; } }
		public Rectangle ContentBounds { get; set; }
		public int CalcX(double normalizedValue) {
			return RangeControl.CalcX(normalizedValue);
		}
		public Color BorderColor { get { return RangeControl.BorderColor; } }
		public Color RulerColor { get { return RangeControl.RulerColor; } }
		public Color LabelColor { get { return RangeControl.LabelColor; } }
		public Matrix NormalTransform { get { return RangeControl.NormalTransform; } }
		public RangeControlHitInfo HotInfo { get; set; }
		public RangeControlHitInfo PressedInfo { get; set; }
		public double ActualRangeMinimum { get; set; }
		public double ActualRangeMaximum { get; set; }
	}
	public enum RangeControlHitTest { None, MinRangeThumb, MaxRangeThumb, RangeBox, ScrollBarThumb, ViewPort, ScrollBarArea, LeftScaleThumb, RightScaleThumb, RangeIndicator, Client }
	public class RangeControlHitInfo  {
		public static Point InvalidPoint = new Point(-10000, -10000);
		public static RangeControlHitInfo Empty = new RangeControlHitInfo();
		public RangeControlHitInfo()
			: this(InvalidPoint) {
			AllowSelection = true;
		}
		public RangeControlHitInfo(Point pt) {
			HitPoint = pt;
		}
		public RangeControlHitInfo(Point pt, bool allowSelection)
			: this(pt) {
			AllowSelection = allowSelection;
		}
		public bool AllowSelection { get; set; }
		public Rectangle ObjectBounds { get; set; }
		public Point HitPoint { get; set; }
		public RangeControlHitTest HitTest { get; set; }
		public object ClientHitTest { get; set; }
		public object HitObject { get; set; }
		public bool ContainsSet(Rectangle rect, RangeControlHitTest hitTest) {
			if(rect.Contains(HitPoint)) {
				if(hitTest == RangeControlHitTest.MaxRangeThumb || hitTest == RangeControlHitTest.MinRangeThumb || hitTest == RangeControlHitTest.RangeBox || hitTest == RangeControlHitTest.RangeIndicator)
					if(!AllowSelection) return false;
				HitTest = hitTest;
				ObjectBounds = rect;
				return true;
			}
			return false;
		}
		public override int GetHashCode() {
			return base.GetHashCode();
		}
		public override bool Equals(object obj) {
			RangeControlHitInfo hitInfo = obj as RangeControlHitInfo;
			bool res = hitInfo != null && hitInfo.HitTest == HitTest;
			if(!res)
				return false;
			if(hitInfo.HitTest == RangeControlHitTest.Client)
				return ClientHitTest == hitInfo.ClientHitTest && HitObject == hitInfo.HitObject;
			return true;
		}
	}
	public class NormalizedRangeInfo {
		public NormalizedRangeInfo() {
			Type = RangeControlValidationType.Range;
			this.range = new RangeControlNormalizedRange();
		}
		public NormalizedRangeInfo(double minimum, double maximum)
			: this() {
			Range.Minimum = minimum;
			Range.Maximum = maximum;
		}
		public NormalizedRangeInfo(double minimum, double maximum, RangeControlValidationType type)
			: this(minimum, maximum) {
			Type = type;
		}
		public NormalizedRangeInfo(double minimum, double maximum, RangeControlValidationType type, ChangedBoundType changedBound)
			: this(minimum, maximum, type) {
			ChangedBound = changedBound;
		}
		public RangeControlValidationType Type { get; set; }
		RangeControlNormalizedRange range;
		public RangeControlNormalizedRange Range { get { return range; } }
		public ChangedBoundType ChangedBound { get; set; }
	}
	public enum RangeControlSelectionType { Thumb, Flag, ThumbAndFlag }
	public enum ChangedBoundType { None, Minimum, Maximum, Both }
	public enum RangeControlValidationType { Range, Selection }
	public class RangeControlNormalizedRange {
		public IRangeControl Owner { get; set; }
		double minimum, maximum;
		public double Minimum {
			get { return minimum; }
			set {
				value = ConstrainMinimum(value);
				if(Minimum == value)
					return;
				minimum = value;
				OnMinimumChanged();
			}
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public void InternalSetMinimum(double value) {
			this.minimum = value;
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public void InternalSetMaximum(double value) {
			this.maximum = value;
		}
		protected virtual double ConstrainMinimum(double value) {
			if(Owner != null)
				return Owner.ConstrainRangeMinimum(value);
			return value;
		}
		protected virtual void OnMinimumChanged() {
			if(Owner != null)
				Owner.OnRangeMinimumChanged(this);
		}
		public double Maximum {
			get { return maximum; }
			set {
				value = ConstrainMaximum(value);
				if(Maximum == value)
					return;
				maximum = value;
				OnMaximumChanged();
			}
		}
		protected virtual double ConstrainMaximum(double value) {
			if(Owner != null)
				return Owner.ConstrainRangeMaximum(value);
			return value;
		}
		protected virtual void OnMaximumChanged() {
			if(Owner != null)
				Owner.OnRangeMaximumChanged(this);
		}
	}
	public class RulerInfoArgs {
		public RulerInfoArgs(object minValue, object maxValue, double rulerWidthInPixels) {
			MinValue = minValue;
			MaxValue = maxValue;
			RulerWidthInPixels = rulerWidthInPixels;
		}
		public object MinValue { get; private set; }
		public object MaxValue { get; private set; }
		public double RulerWidthInPixels { get; private set; }
	}
	public delegate void ClientRangeChangedEventHandler(object sender, RangeControlClientRangeEventArgs range);
	public class RangeControlClientRangeEventArgs : RangeControlRangeEventArgs {
		public bool InvalidateContent { get; set; }
		public bool MakeRangeVisible { get; set; }
		public bool AnimatedViewport { get; set; }
	}
	public class RangeControlRangeEventArgs : EventArgs {
		public RangeControlRangeEventArgs() { }
		public RangeControlRange Range { get; set; }
	}
}
