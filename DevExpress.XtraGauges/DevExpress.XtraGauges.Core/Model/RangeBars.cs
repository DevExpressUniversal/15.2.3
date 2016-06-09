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
using System.ComponentModel;
using System.Drawing;
using DevExpress.Utils.Serializing;
using DevExpress.XtraGauges.Core.Base;
using DevExpress.XtraGauges.Core.Drawing;
using DevExpress.XtraGauges.Core.Primitive;
using DevExpress.Compatibility.System.Drawing;
using DevExpress.Compatibility.System.ComponentModel;
namespace DevExpress.XtraGauges.Core.Model {
	public class BaseRangeBarProvider : BaseProvider, IRangeBar {
		float anchorValueCore;
		float startOffsetCore;
		float endOffsetCore;
		RangeBarAppearance appearanceCore;
		bool fIsValueLockedCore = false;
		float lockedValueCore;
		public BaseRangeBarProvider(OwnerChangedAction rangeBarChanged)
			: base(rangeBarChanged) {
		}
		protected override void OnCreate() {
			this.lockedValueCore = float.NaN;
			this.anchorValueCore = 0f;
			this.startOffsetCore = 0f;
			this.endOffsetCore = 10f;
			this.appearanceCore = new RangeBarAppearance();
			Appearance.Changed += OnAppearanceChanged;
		}
		protected override void OnDispose() {
			if(Appearance != null) {
				Appearance.Changed -= OnAppearanceChanged;
				Ref.Dispose(ref appearanceCore);
			}
			base.Dispose();
		}
		void OnAppearanceChanged(object sender, EventArgs e) {
			OnObjectChanged("Appearance");
		}
		public IScale Scale {
			get { return null; }
		}
		public RangeBarAppearance Appearance {
			get { return appearanceCore; }
		}
		public float AnchorValue {
			get { return anchorValueCore; }
			set {
				if(AnchorValue == value) return;
				anchorValueCore = value;
				OnObjectChanged("AnchorValue");
			}
		}
		public float StartOffset {
			get { return startOffsetCore; }
			set {
				if(StartOffset == value) return;
				startOffsetCore = value;
				OnObjectChanged("StartOffset");
			}
		}
		public float EndOffset {
			get { return endOffsetCore; }
			set {
				if(EndOffset == value) return;
				endOffsetCore = value;
				OnObjectChanged("EndOffset");
			}
		}
		public void LockValue(float value) {
			this.fIsValueLockedCore = true;
			this.lockedValueCore = value;
			OnObjectChanged("LockedValue");
		}
		public void UnlockValue() {
			this.fIsValueLockedCore = false;
			this.lockedValueCore = float.NaN;
			OnObjectChanged("LockedValue");
		}
		public bool IsValueLocked {
			get { return fIsValueLockedCore; }
		}
		public float LockedValue {
			get { return lockedValueCore; }
		}
		public void Assign(IRangeBar source) {
			BeginUpdate();
			if(source != null) {
				this.startOffsetCore = source.StartOffset;
				this.endOffsetCore = source.EndOffset;
				this.anchorValueCore = source.AnchorValue;
				this.Appearance.Assign(source.Appearance);
			}
			EndUpdate();
		}
		public bool IsDifferFrom(IRangeBar source) {
			return (source == null) ? true :
				(this.startOffsetCore != source.StartOffset) ||
				(this.endOffsetCore != source.EndOffset) ||
				(this.anchorValueCore != source.AnchorValue) ||
				(this.Appearance.IsDifferFrom(source.Appearance));
		}
	}
	public abstract class BaseRangeBar : BaseScaleDependentComponent<BaseRangeBarProvider>, IRangeBar {
		protected BaseRangeBar() : base() { }
		protected BaseRangeBar(string name) : base(name) { }
		protected override BaseRangeBarProvider CreateProvider() {
			return new BaseRangeBarProvider(OnScaleDependentComponentChanged);
		}
		protected sealed override bool DependsOnValueBounds {
			get { return true; }
		}
		#region Properties
#if !SL
	[DevExpressXtraGaugesCoreLocalizedDescription("BaseRangeBarScale")]
#endif
		public IScale Scale {
			get { return ScaleCore; }
		}
		[
#if !SL
	DevExpressXtraGaugesCoreLocalizedDescription("BaseRangeBarAppearance"),
#endif
XtraSerializableProperty(XtraSerializationVisibility.Content)]
		public RangeBarAppearance Appearance {
			get { return Provider.Appearance; }
		}
		[
#if !SL
	DevExpressXtraGaugesCoreLocalizedDescription("BaseRangeBarAnchorValue"),
#endif
 DefaultValue(0f)]
		[XtraSerializableProperty]
		public float AnchorValue {
			get { return Provider.AnchorValue; }
			set { Provider.AnchorValue = value; }
		}
		[
#if !SL
	DevExpressXtraGaugesCoreLocalizedDescription("BaseRangeBarStartOffset"),
#endif
 DefaultValue(0f)]
		[XtraSerializableProperty]
		public float StartOffset {
			get { return Provider.StartOffset; }
			set { Provider.StartOffset = value; }
		}
		[
#if !SL
	DevExpressXtraGaugesCoreLocalizedDescription("BaseRangeBarEndOffset"),
#endif
 DefaultValue(10f)]
		[XtraSerializableProperty]
		public float EndOffset {
			get { return Provider.EndOffset; }
			set { Provider.EndOffset = value; }
		}
		public void LockValue(float value) {
			Provider.LockValue(value);
		}
		public void UnlockValue() {
			Provider.UnlockValue();
		}
		float? valueCore;
#if !SL
	[DevExpressXtraGaugesCoreLocalizedDescription("BaseRangeBarValue")]
#endif
		[DefaultValue(null), DevExpress.Utils.Serializing.XtraSerializableProperty]
		public float? Value {
			get { return valueCore; }
			set {
				if(valueCore == value) return;
				float prevValue = GetValue(Value);
				valueCore = value;
				OnScaleDependentComponentChanged(new ValueChangedEventArgs(prevValue, ActualValue));
			}
		}
		public float ActualValue {
			get { return GetValue(Value); }
		}
		protected float GetValue(float? value) {
			return value.HasValue ? value.Value : (ScaleCore.Value);
		}
		#endregion Properties
		internal bool ShouldSerializeAppearance() {
			return Appearance.ShouldSerialize();
		}
		internal void ResetAppearance() {
			Appearance.Reset();
		}
	}
	public class ArcScaleRangeBar : BaseRangeBar,
		IArcScalePointer, ISupportAssign<ArcScaleRangeBar> {
		SectorShape rangeSectorShape;
		public ArcScaleRangeBar() : base() { }
		public ArcScaleRangeBar(string name) : base(name) { }
		protected override void OnCreate() {
			base.OnCreate();
			this.rangeSectorShape = new SectorShape();
			rangeSectorShape.Name = PredefinedShapeNames.RangeBar;
		}
		protected override void OnDispose() {
			Ref.Dispose(ref rangeSectorShape);
			base.OnDispose();
		}
		protected override IScale GetDefaultScaleCore() {
			return ScaleFactory.EmptyArcScale;
		}
		protected override void ShapeProcessing(BaseShape shape) {
			base.ShapeProcessing(shape);
			(shape as SectorShape).RoundedCaps = RoundedCaps;
			(shape as SectorShape).ShowBackground = ShowBackground;
			(shape as SectorShape).Appearance.Assign(Appearance);
			(shape as SectorShape).AppearanceRangeBar.Assign(Appearance);
		}
		protected override void CalculateScaleDependentComponent() {
			Self.SetViewInfoDirty();
			SectorShape shape = Shapes[PredefinedShapeNames.RangeBar] as SectorShape;
			float value = Provider.IsValueLocked ? Provider.LockedValue : ActualValue;
			shape.BeginUpdate();
			ShadingHelper.ProcessShape(shape, Shader, Enabled);
			shape.StartAngle = ArcScale.StartAngle + (ArcScale.EndAngle - ArcScale.StartAngle) * ArcScale.ValueToPercent(AnchorValue);
			shape.EndAngle = ArcScale.StartAngle + (ArcScale.EndAngle - ArcScale.StartAngle) * ArcScale.ValueToPercent(value);
			shape.MinimumAngle = ArcScale.StartAngle;
			shape.MaximumAngle = ArcScale.EndAngle;
			float left = (ArcScale.Center.X - ArcScale.RadiusX) + EndOffset;
			float top = (ArcScale.Center.Y - ArcScale.RadiusY) + EndOffset;
			shape.Box = new RectangleF(left, top, (ArcScale.RadiusX - EndOffset) * 2f, (ArcScale.RadiusY - EndOffset) * 2f);
			shape.InternalRadius = (EndOffset == ArcScale.RadiusX) ? 0 : StartOffset / (ArcScale.RadiusX - EndOffset);
			shape.EndUpdate();
		}
		#region Properties
#if !SL
	[DevExpressXtraGaugesCoreLocalizedDescription("ArcScaleRangeBarArcScale")]
#endif
		public IArcScale ArcScale {
			get { return ScaleCore as IArcScale; }
			set { ScaleCore = value; }
		}
		bool roundedCapsCore;
		[XtraSerializableProperty, 
#if !SL
	DevExpressXtraGaugesCoreLocalizedDescription("ArcScaleRangeBarRoundedCaps"),
#endif
		DesignerSerializationVisibility(DesignerSerializationVisibility.Visible), Category("Appearance")]
		public bool RoundedCaps {
			get { return roundedCapsCore; }
			set {
				if(roundedCapsCore == value) return;
				roundedCapsCore = value;
				OnObjectChanged("RoundedCaps");
			}
		}
		bool showBackgroundCore;
		[XtraSerializableProperty, DefaultValue(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Visible), Category("Appearance")]
		public bool ShowBackground {
			get { return showBackgroundCore; }
			set {
				if(showBackgroundCore == value) return;
				showBackgroundCore = value;
				OnObjectChanged("ShowBackground");
			}
		}
		protected override void OnUpdateObjectCore() { Provider.Update(); }
		bool ShouldSerializeRoundedCaps() { return RoundedCaps != false; }
		void ResetRoundedCaps() { RoundedCaps = false; }
		bool ShouldSerializeShowBackground() { return ShowBackground != false; }
		void ResetShowBackground() { ShowBackground = false; }
		#endregion Properties
		protected override void OnLoadShapes() {
			base.OnLoadShapes();
			Shapes.Add(rangeSectorShape.Clone());
		}
		public void Assign(ArcScaleRangeBar source) {
			BeginUpdate();
			AssignPrimitiveProperties(source);
			if(source != null) {
				this.showBackgroundCore = source.ShowBackground;
				this.ArcScale = source.ArcScale;
				this.RoundedCaps = source.RoundedCaps;
				this.Value = source.Value;
				Provider.Assign(source);
			}
			EndUpdate();
		}
		public bool IsDifferFrom(ArcScaleRangeBar source) {
			return (source == null) ? true : IsDifferFromPrimitive(source) ||
				(this.ArcScale != source.ArcScale) || (this.RoundedCaps != source.RoundedCaps) || (this.ShowBackground != source.ShowBackground) || Provider.IsDifferFrom(source) || 
				(this.Value != source.Value);
		}
	}
	public class LinearScaleRangeBar : BaseRangeBar,
		ILinearScalePointer, IRangeBar, ISupportAssign<LinearScaleRangeBar> {
		PolygonShape rangeBarShape;
		public LinearScaleRangeBar() : base() { }
		public LinearScaleRangeBar(string name) : base(name) { }
		protected override void OnCreate() {
			base.OnCreate();
			rangeBarShape = new PolygonShape(new PointF[4]);
			rangeBarShape.Name = PredefinedShapeNames.RangeBar;
		}
		protected override void OnDispose() {
			Ref.Dispose(ref rangeBarShape);
			base.OnDispose();
		}
		protected override IScale GetDefaultScaleCore() {
			return ScaleFactory.EmptyLinearScale;
		}
		protected override void CalculateScaleDependentComponent() {
			Self.SetViewInfoDirty();
			PolygonShape shape = Shapes[PredefinedShapeNames.RangeBar] as PolygonShape;
			float value = Provider.IsValueLocked ? Provider.LockedValue : ActualValue;
			float valuePercent = LinearScale.ValueToPercent(value);
			float anchorPercent = LinearScale.ValueToPercent(AnchorValue);
			shape.BeginUpdate();
			shape.Appearance.AssignInternal(Appearance);
			ShadingHelper.ProcessShape(shape, Shader, Enabled);
			PointF2D s = LinearScale.StartPoint;
			PointF2D e = LinearScale.EndPoint;
			PointF v = new PointF(e.X - s.X, e.Y - s.Y);
			PointF org1 = new PointF(s.X + v.X * anchorPercent, s.Y + v.Y * anchorPercent);
			PointF org2 = new PointF(s.X + v.X * valuePercent, s.Y + v.Y * valuePercent);
			float s1 = -StartOffset / (float)MathHelper.CalcVectorLength(v);
			float s2 = -EndOffset / (float)MathHelper.CalcVectorLength(v);
			shape.Points[0] = org1 + new SizeF(v.Y * s1, -v.X * s1);
			shape.Points[1] = org1 + new SizeF(v.Y * s2, -v.X * s2);
			shape.Points[2] = org2 + new SizeF(v.Y * s2, -v.X * s2);
			shape.Points[3] = org2 + new SizeF(v.Y * s1, -v.X * s1);
			shape.EndUpdate();
		}
		#region Properties
#if !SL
	[DevExpressXtraGaugesCoreLocalizedDescription("LinearScaleRangeBarLinearScale")]
#endif
		public ILinearScale LinearScale {
			get { return ScaleCore as ILinearScale; }
			set { ScaleCore = value; }
		}
		#endregion Properties
		protected override void OnLoadShapes() {
			base.OnLoadShapes();
			Shapes.Add(rangeBarShape.Clone());
		}
		public void Assign(LinearScaleRangeBar source) {
			BeginUpdate();
			AssignPrimitiveProperties(source);
			if(source != null) {
				this.LinearScale = source.LinearScale;
				this.Value = source.Value;
				Provider.Assign(source);
			}
			EndUpdate();
		}
		public bool IsDifferFrom(LinearScaleRangeBar source) {
			return (source == null) ? true : IsDifferFromPrimitive(source) ||
				(this.LinearScale != source.LinearScale) || Provider.IsDifferFrom(source)||
				(this.Value != source.Value);
		}
	}
}
