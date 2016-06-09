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
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Globalization;
using DevExpress.Utils.Serializing;
using DevExpress.Utils.Serializing.Helpers;
using DevExpress.XtraGauges.Core.Base;
using DevExpress.XtraGauges.Core.Drawing;
using DevExpress.XtraGauges.Core.Primitive;
using DevExpress.Compatibility.System.ComponentModel;
using DevExpress.Compatibility.System.Drawing;
using DevExpress.Compatibility.System.Drawing.Drawing2D;
#if !DXPORTABLE
using System.Drawing.Design;
#endif
namespace DevExpress.XtraGauges.Core.Model {
#if !DXPORTABLE
	[Editor("DevExpress.XtraGauges.Design.IndicatorStateCollectionEditor, " + AssemblyInfo.SRAssemblyGaugesDesignWin, typeof(UITypeEditor))]
#endif
	[TypeConverter(typeof(IndicatorStateCollectionObjectTypeConverter))]
	public class IndicatorStateCollection : BaseChangeableList<IIndicatorState> {
		public int IndexOf(IIndicatorState state) {
			return List.IndexOf(state);
		}
		public bool Contains(string name) {
			string[] names = CollectionHelper.GetNames(this);
			return CollectionHelper.NamesContains(name, names);
		}
		protected override void OnBeforeElementAdded(IIndicatorState element) {
			string[] names = CollectionHelper.GetNames(this);
			if(CollectionHelper.NamesContains(element.Name, names)) {
				element.Name = UniqueNameHelper.GetUniqueName("State", names, names.Length);
			}
			base.OnBeforeElementAdded(element);
		}
		protected override void OnElementAdded(IIndicatorState element) {
			base.OnElementAdded(element);
			BaseIndicatorState state = element as BaseIndicatorState;
			if(state != null) {
				state.Changed += OnElementChanged;
				state.Disposed += OnElementDisposed;
			}
		}
		protected override void OnElementRemoved(IIndicatorState element) {
			BaseIndicatorState state = element as BaseIndicatorState;
			if(state != null) {
				state.Disposed -= OnElementDisposed;
				state.Changed -= OnElementChanged;
			}
			base.OnElementRemoved(element);
		}
		void OnElementChanged(object sender, EventArgs ea) {
			RaiseCollectionChanged(new CollectionChangedEventArgs<IIndicatorState>(sender as IIndicatorState, ElementChangedType.ElementUpdated));
		}
		void OnElementDisposed(object sender, EventArgs ea) {
			RaiseCollectionChanged(new CollectionChangedEventArgs<IIndicatorState>(sender as IIndicatorState, ElementChangedType.ElementDisposed));
			if(List != null) Remove(sender as IIndicatorState);
		}
	}
	public class BaseStateIndicatorProvider : BaseProvider, IStateIndicator {
		SizeF sizeCore;
		PointF2D positionCore;
		IndicatorStateCollection stateCollectionCore;
		StateIndicator ownerCore;
		int stateIndexCore;
		public BaseStateIndicatorProvider(StateIndicator owner, OwnerChangedAction stateIndicatorChanged)
			: base(stateIndicatorChanged) {
			this.stateIndexCore = -1;
			this.ownerCore = owner;
			this.stateCollectionCore = Owner.CreateStates();
			States.CollectionChanged += OnStatesCollectionChanged;
			this.sizeCore = Owner.GetDefaultSize();
			this.positionCore = Owner.GetDefaultPosition();
		}
		protected override void OnDispose() {
			if(States != null) {
				States.CollectionChanged -= OnStatesCollectionChanged;
				States.Clear();
			}
			Ref.Dispose(ref stateCollectionCore);
			base.OnDispose();
		}
		void OnStatesCollectionChanged(CollectionChangedEventArgs<IIndicatorState> ea) {
			switch(ea.ChangedType) {
				case ElementChangedType.ElementAdded:
				case ElementChangedType.ElementRemoved:
				case ElementChangedType.ElementUpdated:
					OnObjectChanged("States");
					break;
			}
		}
		protected IIndicatorState CreateState(string name) {
			return Owner.CreateState(name);
		}
		protected StateIndicator Owner {
			get { return ownerCore; }
		}
		public IIndicatorState State {
			get { return (StateIndex >= 0 && StateIndex < States.Count) ? States[StateIndex] : BaseIndicatorState.Unknown; }
		}
		public void AddEnum(Enum states) {
			string[] existingNames = CollectionHelper.GetNames(States);
			string[] names = Enum.GetNames(states.GetType());
			for(int i = 0; i < names.Length; i++) {
				if(!CollectionHelper.NamesContains(names[i], existingNames)) {
					States.Add(CreateState(names[i]));
				}
			}
		}
		public int StateIndex {
			get { return stateIndexCore; }
			set {
				if(StateIndex == value) return;
				this.stateIndexCore = value;
				OnObjectChanged("StateIndex");
			}
		}
		public void SetStateByName(string stateName) {
			IIndicatorState newState = BaseIndicatorState.Unknown;
			foreach(IIndicatorState state in States) {
				if(state.Name == stateName) {
					newState = state;
					break;
				}
			}
			SetStateCore(newState);
			OnObjectChanged("StateIndex");
		}
		protected internal void SetStateCore(IIndicatorState newState) {
			stateIndexCore = (newState != null) ? States.IndexOf(newState) : -1;
		}
		public SizeF Size {
			get { return sizeCore; }
			set {
				if(Size == value) return;
				sizeCore = value;
				OnObjectChanged("Size");
			}
		}
		public PointF2D Center {
			get { return positionCore; }
			set {
				if(Center == value) return;
				positionCore = value;
				OnObjectChanged("Position");
			}
		}
		public IndicatorStateCollection States {
			get { return stateCollectionCore; }
		}
	}
	[TypeConverter(typeof(IndicatorStateObjectTypeConverter))]
	public abstract class BaseIndicatorState : BaseShapeProvider<StateIndicatorShapeType>, ISupportAcceptOrder {
		[ThreadStatic]
		static IIndicatorState unknownStateCore;
		public static IIndicatorState Unknown {
			get {
				if(unknownStateCore == null) unknownStateCore = new UnknownIndicatorState();
				return unknownStateCore;
			}
		}
		string nameCore;
		float startValueCore;
		float intervalLengthCore;
		public BaseIndicatorState()
			: this("Default") {
		}
		public BaseIndicatorState(StateIndicatorShapeType shapeType)
			: base(null, shapeType) {
		}
		public BaseIndicatorState(string name)
			: base(null) {
			this.nameCore = name;
		}
		protected override StateIndicatorShapeType DefaultShapeType {
			get { return StateIndicatorShapeType.Default; }
		}
		protected override BaseShape GetShape(StateIndicatorShapeType value) {
			return StateIndicatorShapesFactory.GetIndicatorStateShape(value);
		}
		protected override void OnCreate() {
			base.OnCreate();
			this.startValueCore = 0;
			this.intervalLengthCore = 1f;
		}
		[XtraSerializableProperty]
		public string Name {
			get { return nameCore; }
			set {
				if(Name == value) return;
				nameCore = value;
				OnObjectChanged("Name");
			}
		}
		[DefaultValue(0f)]
		[XtraSerializableProperty]
		public float StartValue {
			get { return startValueCore; }
			set {
				if(StartValue == value) return;
				startValueCore = value;
				OnObjectChanged("StartValue");
			}
		}
		[DefaultValue(1f)]
		[XtraSerializableProperty]
		public float IntervalLength {
			get { return intervalLengthCore; }
			set {
				if(IntervalLength == value) return;
				intervalLengthCore = value;
				OnObjectChanged("IntervalLength");
			}
		}
		int ISupportAcceptOrder.AcceptOrder {
			get { return String.IsNullOrEmpty(Name) ? 0 : (int)((char)Name[0]); }
			set { }
		}
		public bool IsUnknown {
			get { return this is UnknownIndicatorState; }
		}
		protected void AssignIndicatorStateCore(IIndicatorState source) {
			this.nameCore = source.Name;
			this.ShapeType = source.ShapeType;
		}
		protected void AssignScaleIndicatorStateCore(IScaleIndicatorState source) {
			if(source != null) {
				this.intervalLengthCore = source.IntervalLength;
				this.startValueCore = source.StartValue;
			}
		}
		protected bool IsDifferFromIndicatorStateCore(IIndicatorState source) {
			return (this.Name != source.Name) || (this.ShapeType != source.ShapeType);
		}
		protected bool IsDifferFromScaleIndicatorStateCore(IScaleIndicatorState source) {
			if(source != null) {
				return (this.IntervalLength != source.IntervalLength) || (this.StartValue != source.StartValue);
			}
			return false;
		}
		public void Assign(IIndicatorState source) {
			AssignIndicatorStateCore(source);
			AssignScaleIndicatorStateCore(source as IScaleIndicatorState);
		}
		public bool IsDifferFrom(IIndicatorState source) {
			return (source == null) ? true :
					IsDifferFromIndicatorStateCore(source) ||
					IsDifferFromScaleIndicatorStateCore(source as IScaleIndicatorState);
		}
		protected override void CopyToCore(BaseObject clone) {
			IIndicatorState state = clone as IIndicatorState;
			if(state != null) {
				state.Assign(this as IIndicatorState);
			}
		}
		sealed class UnknownIndicatorState : IndicatorState {
			internal UnknownIndicatorState() : base("Unknown") { }
			protected override BaseObject CloneCore() {
				return new UnknownIndicatorState();
			}
		}
	}
	public class IndicatorState : BaseIndicatorState, IIndicatorState, ISupportAssign<IIndicatorState> {
		public IndicatorState() : base() { }
		public IndicatorState(string name) : base(name) { }
		public IndicatorState(StateIndicatorShapeType shapeType) : base(shapeType) { }
		protected override BaseObject CloneCore() {
			return new IndicatorState();
		}
	}
	public class ScaleIndicatorState : BaseIndicatorState, IScaleIndicatorState, ISupportAssign<IIndicatorState> {
		public ScaleIndicatorState() : base() { }
		public ScaleIndicatorState(string name) : base(name) { }
		public ScaleIndicatorState(StateIndicatorShapeType shapeType) : base(shapeType) { }
		protected override BaseObject CloneCore() {
			return new ScaleIndicatorState();
		}
	}
	public class StateIndicator : BaseScaleIndependentComponent<BaseStateIndicatorProvider>,
		IStateIndicator, ISupportAssign<StateIndicator> {
		protected internal SizeF DefaultSize;
		public StateIndicator() : base() { }
		public StateIndicator(string name) : base(name) { }
		protected internal virtual PointF2D GetDefaultPosition() {
			return new PointF2D(125, 125);
		}
		protected internal virtual SizeF GetDefaultSize() {
			return new SizeF(200, 200);
		}
		protected override BaseStateIndicatorProvider CreateProvider() {
			return new BaseStateIndicatorProvider(this, OnScaleIndependentComponentChanged);
		}
		protected override void CalculateScaleIndependentComponent() {
			BaseShape stateShape = Shapes[PredefinedShapeNames.IndicatorState];
			RectangleF box = stateShape.BoundingBox;
			PointF center = new PointF(box.Left + box.Width * 0.5f, box.Top + box.Height * 0.5f);
			SizeF scale = new SizeF(Size.Width / box.Width, Size.Height / box.Height);
			Transform = new Matrix(scale.Width, 0.0f, 0.0f, scale.Height, Center.X - center.X * scale.Width, Center.Y - center.Y * scale.Height);
		}
		protected internal virtual IIndicatorState CreateState(string name) {
			return new IndicatorState(name);
		}
		protected internal virtual IndicatorStateCollection CreateStates() {
			return new IndicatorStateCollection();
		}
#if !SL
	[DevExpressXtraGaugesCoreLocalizedDescription("StateIndicatorStateIndex")]
#endif
		[DefaultValue(-1), Bindable(true), XtraSerializableProperty]
		public int StateIndex {
			get { return Provider.StateIndex; }
			set { Provider.StateIndex = value; }
		}
		[
#if !SL
	DevExpressXtraGaugesCoreLocalizedDescription("StateIndicatorState"),
#endif
DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public IIndicatorState State {
			get { return Provider.State; }
		}
		[
#if !SL
	DevExpressXtraGaugesCoreLocalizedDescription("StateIndicatorStates"),
#endif
DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		[XtraSerializableProperty(XtraSerializationVisibility.Collection, true)]
		public virtual IndicatorStateCollection States {
			get { return Provider.States; }
		}
		[
#if !SL
	DevExpressXtraGaugesCoreLocalizedDescription("StateIndicatorCenter"),
#endif
XtraSerializableProperty]
		public PointF2D Center {
			get { return Provider.Center; }
			set { Provider.Center = value; }
		}
		[
#if !SL
	DevExpressXtraGaugesCoreLocalizedDescription("StateIndicatorSize"),
#endif
XtraSerializableProperty]
		public SizeF Size {
			get { return Provider.Size; }
			set { Provider.Size = value; }
		}
		public void AddEnum(Enum states) {
			Provider.AddEnum(states);
		}
		public void SetStateByName(string name) {
			Provider.SetStateByName(name);
		}
		protected override void OnLoadShapes() {
			base.OnLoadShapes();
			Shapes.Add(Provider.State.Shape.Clone());
		}
		public void Assign(StateIndicator source) {
			BeginUpdate();
			AssignPrimitiveProperties(source);
			if(source != null) {
				this.AssignStates(source.States);
				this.StateIndex = source.StateIndex;
				this.Size = source.Size;
				this.Center = source.Center;
			}
			EndUpdate();
		}
		public bool IsDifferFrom(StateIndicator source) {
			return (source == null) ? true : IsDifferFromPrimitive(source) ||
					(this.IsStatesDifferFrom(source.States)) ||
					(this.StateIndex != source.StateIndex) ||
					(this.Size != source.Size) ||
					(this.Center != source.Center);
		}
		void AssignStates(IndicatorStateCollection states) {
			States.Clear();
			for(int i = 0; i < states.Count; i++) {
				IIndicatorState state = CreateState(states[i].Name);
				state.Assign(states[i]);
				States.Add(state);
			}
		}
		bool IsStatesDifferFrom(IndicatorStateCollection states) {
			if(states.Count != States.Count) return true;
			for(int i = 0; i < states.Count; i++) {
				if(States[i].IsDifferFrom(states[i])) return true;
			}
			return false;
		}
		internal bool ShouldSerializeSize() {
			return Size != DefaultSize;
		}
		internal bool ShouldSerializePosition() {
			return Center != GetDefaultPosition();
		}
		internal virtual void ResetPosition() {
			Center = GetDefaultPosition();
		}
		internal virtual void ResetSize() {
			Size = GetDefaultSize();
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public object XtraCreateStatesItem(XtraItemEventArgs e) {
			XtraPropertyInfo propInfo = e.Item;
			IndicatorStateCollection collection = e.Collection as IndicatorStateCollection;
			if(propInfo == null || collection == null) return null;
			BeginUpdate();
			string stateName = (string)propInfo.ChildProperties["Name"].Value;
			IIndicatorState state = CreateState(stateName);
			if(!collection.Contains(stateName)) collection.Add(state);
			CancelUpdate();
			return state;
		}
	}
	public class ScaleStateIndicator : StateIndicator, IScaleStateIndicator, ISupportAssign<ScaleStateIndicator> {
		IScale scaleCore;
		public ScaleStateIndicator() : base() { }
		public ScaleStateIndicator(string name) : base(name) { }
		protected override void OnDispose() {
			UnsubscribeScaleEvents();
			scaleCore = null;
			base.OnDispose();
		}
		protected internal override SizeF GetDefaultSize() {
			return new SizeF(25, 25);
		}
		protected internal override PointF2D GetDefaultPosition() {
			return new PointF2D(125, 125);
		}
		protected internal override IIndicatorState CreateState(string name) {
			return new ScaleIndicatorState(name);
		}
		public IScale Scale {
			get { return scaleCore; }
		}
		public IScale IndicatorScale {
			get { return scaleCore; }
			set {
				if(scaleCore == value || (value == null && !AllowNullScale)) return;
				SetScaleCore(value);
				Provider.PerformOwnerChanged("IndicatorScale");
			}
		}
		bool AllowNullScale {
			get { return Site != null; }
		}
		void SetScaleCore(IScale value) {
			UnsubscribeScaleEvents();
			this.scaleCore = value;
			SubscribeScaleEvents();
		}
		void SubscribeScaleEvents() {
			if(Scale != null) Scale.ValueChanged += OnScaleValueChanged;
		}
		void UnsubscribeScaleEvents() {
			if(Scale != null) Scale.ValueChanged -= OnScaleValueChanged;
		}
		protected void OnScaleValueChanged(object sender, EventArgs ea) {
			if(IsDisposing) return;
			SetCalculationDelayed();
			RaiseChanged(ea);
		}
		protected override void OnLoadShapes() {
			if(!IsDisposing && Scale != null) {
				IScaleIndicatorState state = CalcState(Scale.Value);
				Provider.SetStateCore(state);
			}
			base.OnLoadShapes();
		}
		protected IScaleIndicatorState CalcState(float val) {
			foreach(IScaleIndicatorState state in States) {
				float min = Math.Min(state.StartValue, state.StartValue + state.IntervalLength);
				float max = Math.Max(state.StartValue, state.StartValue + state.IntervalLength);
				if(val >= min && val <= max)
					return state;
			}
			return null;
		}
		public void Assign(ScaleStateIndicator source) {
			BeginUpdate();
			if(source != null) {
				base.Assign(source);
				this.IndicatorScale = source.IndicatorScale;
			}
			EndUpdate();
		}
		public bool IsDifferFrom(ScaleStateIndicator source) {
			return (source == null) ? true : IsDifferFromPrimitive(source) ||
					(base.IsDifferFrom(source)) ||
					(this.Scale != source.Scale);
		}
	}
	public class StateIndicatorObjectTypeConverter : ExpandableObjectConverter {
		public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object value, Attribute[] attributes) {
			PropertyDescriptorCollection props = base.GetProperties(context, value, attributes);
			return AllowFilterProperties(value) ? FilterPropertiesHelper.FilterProperties(props, GetExpectedProperties(value)) : props;
		}
		protected virtual bool AllowFilterProperties(object value) {
			return !(value as StateIndicator).IsXtraSerializing;
		}
		static string[] GetExpectedProperties(object value) {
			IStateIndicator stateIndicator = value as IStateIndicator;
			string[] res = new string[0];
			if(stateIndicator == null) return res;
			if(stateIndicator is StateIndicator) res = new string[] { "Shader", "StateIndex", "States", "Center", "Size" };
			if(stateIndicator is ScaleStateIndicator) res = new string[] { "Shader", "IndicatorScale", "States", "Center", "Size" };
			List<string> list = new List<string>(res);
			list.Add("(Name)");
			list.AddRange(new string[] { "DataBindings", "Name", "ZOrder" });
			return list.ToArray();
		}
		public override bool CanConvertTo(ITypeDescriptorContext context, Type destType) {
			return destType == typeof(string);
		}
		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destType) {
			IStateIndicator stateIndicator = value as IStateIndicator;
			return "<IStateIndicator>";
		}
	}
	public class IndicatorStateObjectTypeConverter : ExpandableObjectConverter {
		public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object value, Attribute[] attributes) {
			return FilterPropertiesHelper.FilterProperties(base.GetProperties(context, value, attributes), GetExpectedProperties(value));
		}
		static string[] GetExpectedProperties(object value) {
			IIndicatorState state = value as IIndicatorState;
			string[] res = new string[0];
			if(state == null || state.IsUnknown) return res;
			if(state is IIndicatorState) res = new string[] { "Name", "ShapeType" };
			if(state is IScaleIndicatorState) res = new string[] { "Name", "StartValue", "IntervalLength", "ShapeType" };
			return res;
		}
		public override bool CanConvertTo(ITypeDescriptorContext context, Type destType) {
			return destType == typeof(string);
		}
		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destType) {
			IIndicatorState state = value as IIndicatorState;
			if(state == null || state.IsUnknown) return "<UnknownIndicatorState>";
			return "<IIndicatorState>";
		}
	}
	public class ScaleIndicatorStateObjectTypeConverter : ExpandableObjectConverter {
		public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object value, Attribute[] attributes) {
			return FilterPropertiesHelper.FilterProperties(base.GetProperties(context, value, attributes), GetExpectedProperties(value));
		}
		static string[] GetExpectedProperties(object value) {
			IIndicatorState state = value as IIndicatorState;
			if(state == null || state.IsUnknown) return new string[0];
			return new string[] { "Name", "StartValue", "IntervalLength", "ShapeType" };
		}
		public override bool CanConvertTo(ITypeDescriptorContext context, Type destType) {
			return destType == typeof(string);
		}
		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destType) {
			IIndicatorState state = value as IIndicatorState;
			if(state == null || state.IsUnknown) return "<UnknownIndicatorState>";
			return "<IIndicatorState>";
		}
	}
	public class IndicatorStateCollectionObjectTypeConverter : TypeConverter {
		public override bool CanConvertTo(ITypeDescriptorContext context, Type destType) {
			return destType == typeof(string);
		}
		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destType) {
			return "<States...>";
		}
	}
}
