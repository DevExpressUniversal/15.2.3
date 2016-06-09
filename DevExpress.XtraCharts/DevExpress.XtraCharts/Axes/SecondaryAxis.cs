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
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using DevExpress.Charts.Native;
using DevExpress.Utils;
using DevExpress.Utils.Serializing;
using DevExpress.XtraCharts.Design;
using DevExpress.XtraCharts.Localization;
using DevExpress.XtraCharts.Native;
namespace DevExpress.XtraCharts {
	[
	TypeConverter(typeof(SecondaryAxisXTypeConverter)),
	DesignerSerializer("DevExpress.XtraCharts.Design.ChartItemSerializer," + AssemblyInfo.SRAssemblyChartsExtensions,
					   "System.ComponentModel.Design.Serialization.CodeDomSerializer,System.Design")
	]
	public class SecondaryAxisX : AxisXBase, ISupportInitialize, IXtraSerializable, ISupportID {
		bool loading = false;
		int id = -1;
		int ISupportID.ID {
			get { return id; }
			set {
				ChartDebug.Assert(value >= 0);
				if (value >= 0)
					id = value;
			}
		}
		protected override bool AllowEmptyName { get { return false; } }
		protected override string EmptyNameExceptionText { get { return ChartLocalizer.GetString(ChartStringId.MsgEmptySecondaryAxisName); } }
		protected override AxisAlignment DefaultAlignment { get { return AxisAlignment.Far; } }
		protected override ChartElementVisibilityPriority Priority { get { return ChartElementVisibilityPriority.AxisX; } }
		protected internal override bool Loading { get { return loading || base.Loading; } }
		protected internal override bool ShouldFilterZeroAlignment { get { return true; } }
		protected internal override int ActualAxisID { get { return id; } }
		[
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		XtraSerializableProperty
		]
		public int AxisID {
			get { return id; }
			set {
				if (!Loading)
					throw new NotSupportedException(ChartLocalizer.GetString(ChartStringId.MsgInternalPropertyChangeError));
				id = value;
			}
		}
		[
		Obsolete("This property is obsolete now. Use DateTimeScaleOptions.ScaleMode instead."),
#if !SL
	DevExpressXtraChartsLocalizedDescription("SecondaryAxisXDateTimeScaleMode"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.SecondaryAxisX.DateTimeScaleMode"),
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		XtraSerializableProperty
		]
		public override DateTimeScaleMode DateTimeScaleMode {
			get { return base.DateTimeScaleMode; }
			set { base.DateTimeScaleMode = value; }
		}
		SecondaryAxisX(string name, XYDiagram diagram) : base(name, diagram) {
		}
		public SecondaryAxisX(string name) : this(name, null) {
			CheckName();
		}
		public SecondaryAxisX() : this(String.Empty, null) {
		}
		void ISupportInitialize.BeginInit() {
			loading = true;
		}
		void ISupportInitialize.EndInit() {
			loading = false;
		}
		void IXtraSerializable.OnStartSerializing() {
		}
		void IXtraSerializable.OnEndSerializing() {
		}
		void IXtraSerializable.OnStartDeserializing(LayoutAllowEventArgs e) {
			((ISupportInitialize)this).BeginInit();
		}
		void IXtraSerializable.OnEndDeserializing(string restoredVersion) {
			((ISupportInitialize)this).EndInit();
		}
		bool ShouldSerializeDateTimeScaleMode() {
			return false;
		}
		protected internal override bool ShouldSerialize() {
			return base.ShouldSerialize() || ShouldSerializeDateTimeScaleMode();
		}
		protected override ChartElement CreateObjectForClone() {
			return new SecondaryAxisX();
		}
		protected override GridLines CreateGridLines() {
			return new SecondaryGridLinesX(this);
		}
	}
	[
	TypeConverter(typeof(SecondaryAxisYTypeConverter)),
	DesignerSerializer("DevExpress.XtraCharts.Design.ChartItemSerializer," + AssemblyInfo.SRAssemblyChartsExtensions,
					   "System.ComponentModel.Design.Serialization.CodeDomSerializer,System.Design")
	]
	public class SecondaryAxisY : AxisYBase, ISupportInitialize, IXtraSerializable, ISupportID {
		bool loading = false;
		int id = -1;
		int ISupportID.ID {
			get { return id; }
			set {
				ChartDebug.Assert(value >= 0);
				if (value >= 0)
					id = value;
			}
		}
		protected override bool AllowEmptyName { get { return false; } }
		protected override string EmptyNameExceptionText { get { return ChartLocalizer.GetString(ChartStringId.MsgEmptySecondaryAxisName); } }
		protected override AxisAlignment DefaultAlignment { get { return AxisAlignment.Far; } }
		protected override ChartElementVisibilityPriority Priority { get { return ChartElementVisibilityPriority.AxisY; } }
		protected internal override bool Loading { get { return loading || base.Loading; } }
		protected internal override bool ShouldFilterZeroAlignment { get { return true; } }
		protected internal override int ActualAxisID { get { return AxisID; } }
		[
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		XtraSerializableProperty
		]
		public int AxisID {
			get { return id; }
			set {
				if (!Loading)
					throw new NotSupportedException(ChartLocalizer.GetString(ChartStringId.MsgInternalPropertyChangeError));
				id = value;
			}
		}
		SecondaryAxisY(string name, XYDiagram diagram)
			: base(name, diagram) {
		}
		public SecondaryAxisY(string name)
			: this(name, null) {
			CheckName();
		}
		public SecondaryAxisY()
			: this(String.Empty, null) {
		}
		void ISupportInitialize.BeginInit() {
			loading = true;
		}
		void ISupportInitialize.EndInit() {
			loading = false;
		}
		void IXtraSerializable.OnStartSerializing() {
		}
		void IXtraSerializable.OnEndSerializing() {
		}
		void IXtraSerializable.OnStartDeserializing(LayoutAllowEventArgs e) {
			((ISupportInitialize)this).BeginInit();
		}
		void IXtraSerializable.OnEndDeserializing(string restoredVersion) {
			((ISupportInitialize)this).EndInit();
		}
		protected internal override bool ShouldSerialize() {
			return true;
		}
		protected override ChartElement CreateObjectForClone() {
			return new SecondaryAxisY();
		}
		protected override GridLines CreateGridLines() {
			return new SecondaryGridLinesY(this);
		}
	}
	public abstract class SecondaryAxisCollection : ChartElementNamedCollection, IEnumerable<IAxisData>, IEnumerable<Axis2D> {
		public Axis2D this[int index] { get { return (Axis2D)List[index]; } }
		internal SecondaryAxisCollection(XYDiagram2D diagram) : base(diagram) { }
		IEnumerator IEnumerable.GetEnumerator() {
			return base.GetEnumerator();
		}
		IEnumerator<IAxisData> IEnumerable<IAxisData>.GetEnumerator() {
			foreach (IAxisData axis in InnerList)
				yield return axis;
		}
		IEnumerator<Axis2D> IEnumerable<Axis2D>.GetEnumerator() {
			foreach (Axis2D axis in InnerList)
				yield return axis;
		}
		protected void AddRangeInternal(ICollection coll) {
			base.AddRange(coll);
		}
		protected void RemoveInternal(Axis2D axis) {
			base.Remove(axis);
		}
		protected abstract Axis2D CreateNewAxis(string name);
		protected internal bool ContainsInternal(Axis2D axis) {
			return base.Contains(axis);
		}
		protected internal Axis2D GetAxisByNameInternal(string name) {
			return (Axis2D)base.GetElementByName(name);
		}
		protected internal int AddInternal(Axis2D axis) {
			if (axis.VisualRange.Auto && axis.WholeRange.Auto)
				RangeHelper.SetDefaultRange(axis);
			else
				RangeHelper.SynchronizeVisualRange(axis.WholeRangeData, axis.VisualRangeData, true);
			return base.Add(axis);
		}
		internal Axis2D CreateNewAxis() {
			return CreateNewAxis(GenerateName());
		}
		internal int IndexOf(Axis2D axis) {
			for (int i = 0; i < Count; i++)
				if (axis.ActualAxisID == ((Axis2D)InnerList[i]).ActualAxisID)
					return i;
			return -1;
		}
		internal int AddWithoutChanged(Axis2D axis) {
			return base.AddWithoutChanged(axis);
		}
		internal bool ContainsWithChildren(object obj) {
			foreach (Axis2D axis in this)
				if (obj == axis || axis.Contains(obj))
					return true;
			return false;
		}
		internal Axis2D GetAxisByIndex(int index) {
			return (Axis2D)InnerList[index];
		}
		[Obsolete("This method is now obsolete. Use the SecondaryAxisXCollection.AddRange or SecondaryAxisYCollection.AddRange method instead.")]
		public void AddRange(Axis[] coll) {
			base.AddRange(coll);
		}
		[Obsolete("This method is now obsolete. Use the SecondaryAxisXCollection.Remove or SecondaryAxisYCollection.Remove method instead.")]
		public void Remove(Axis axis) {
			base.Remove(axis);
		}
		[Obsolete("This method is now obsolete. Use the SecondaryAxisXCollection.Contains or SecondaryAxisYCollection.Contains method instead.")]
		public bool Contains(Axis axis) {
			return base.Contains(axis);
		}
		[Obsolete("This method is now obsolete. Use the SecondaryAxisXCollection.GetAxisByName or SecondaryAxisYCollection.GetAxisByName method instead.")]
		public Axis GetAxisByName(string name) {
			return (Axis)base.GetElementByName(name);
		}
		protected override ChartUpdateInfoBase CreateUpdateInfo(ChartCollectionOperation operation, object oldItem, int oldIndex, object newItem, int newIndex) {
			return new AxisCollectionUpdateInfo(this, operation, oldItem as IAxisData, oldIndex, newItem as IAxisData, newIndex);
		}
		protected override ChartUpdateInfoBase CreateBatchUpdateInfo(ChartCollectionOperation operation, ICollection oldItems, int oldIndex, ICollection newItems, int newIndex) {
			List<IAxisData> newlist = new List<IAxisData>();
			if (newItems != null)
				foreach (object obj in newItems) {
					IAxisData axis = obj as IAxisData;
					if (axis != null)
						newlist.Add(axis);
				}
			List<IAxisData> oldlist = new List<IAxisData>();
			if (oldItems != null)
				foreach (object obj in oldItems) {
					IAxisData axis = obj as IAxisData;
					if (axis != null)
						oldlist.Add(axis);
				}
			return new AxisCollectionBatchUpdateInfo(this, operation, oldlist, oldIndex, newlist, newIndex);
		}
	}
	[
	DesignerSerializer("DevExpress.XtraCharts.Design.ChartCollectionSerializer," + AssemblyInfo.SRAssemblyChartsExtensions,
					   "System.ComponentModel.Design.Serialization.CodeDomSerializer,System.Design")
	]
	public class SecondaryAxisXCollection : SecondaryAxisCollection {
		protected override string NamePrefix { get { return ChartLocalizer.GetString(ChartStringId.SecondaryAxisXPrefix); } }
#if !SL
	[DevExpressXtraChartsLocalizedDescription("SecondaryAxisXCollectionItem")]
#endif
		public new SecondaryAxisX this[int index] { get { return (SecondaryAxisX)List[index]; } }
		internal SecondaryAxisXCollection(XYDiagram diagram)
			: base(diagram) {
		}
		protected override void ChangeOwnerForItem(ChartElement item) {
			base.ChangeOwnerForItem(item);
			XYDiagram diagram = Owner as XYDiagram;
			if (diagram != null && !diagram.Loading && AxisNavigationUtils.IsScrollingZoomingEnabled(diagram)) {
				SecondaryAxisX axis = item as SecondaryAxisX;
				if ((axis != null) && (axis.DateTimeScaleOptions.ScaleMode != ScaleMode.Manual)) {
					axis.DateTimeScaleOptions.ScaleMode = ScaleMode.Manual;
					axis.DateTimeScaleOptions.MeasureUnit = DateTimeScaleOptions.DefaultMeasureUnit;
					axis.DateTimeScaleOptions.GridAlignment = DateTimeScaleOptions.DefaultGridAlignment;
				}
			}
		}
		protected override Axis2D CreateNewAxis(string name) {
			return new SecondaryAxisX(name);
		}
		public int Add(SecondaryAxisX axis) {
			return AddInternal(axis);
		}
		public void AddRange(SecondaryAxisX[] coll) {
			AddRangeInternal(coll);
		}
		public void Insert(int index, SecondaryAxisX axis) {
			base.Insert(index, axis);
		}
		public void Remove(SecondaryAxisX axis) {
			RemoveInternal(axis);
		}
		public bool Contains(SecondaryAxisX axis) {
			return ContainsInternal(axis);
		}
		public new SecondaryAxisX GetAxisByName(string name) {
			return (SecondaryAxisX)GetAxisByNameInternal(name);
		}
	}
	[
	DesignerSerializer("DevExpress.XtraCharts.Design.ChartCollectionSerializer," + AssemblyInfo.SRAssemblyChartsExtensions,
					   "System.ComponentModel.Design.Serialization.CodeDomSerializer,System.Design")
	]
	public class SecondaryAxisYCollection : SecondaryAxisCollection {
		protected override string NamePrefix { get { return ChartLocalizer.GetString(ChartStringId.SecondaryAxisYPrefix); } }
#if !SL
	[DevExpressXtraChartsLocalizedDescription("SecondaryAxisYCollectionItem")]
#endif
		public new SecondaryAxisY this[int index] { get { return (SecondaryAxisY)List[index]; } }
		internal SecondaryAxisYCollection(XYDiagram diagram)
			: base(diagram) {
		}
		protected override Axis2D CreateNewAxis(string name) {
			return new SecondaryAxisY(name);
		}
		public int Add(SecondaryAxisY axis) {
			return AddInternal(axis);
		}
		public void AddRange(SecondaryAxisY[] coll) {
			AddRangeInternal(coll);
		}
		public void Insert(int index, SecondaryAxisY axis) {
			base.Insert(index, axis);
		}
		public void Remove(SecondaryAxisY axis) {
			RemoveInternal(axis);
		}
		public bool Contains(SecondaryAxisY axis) {
			return ContainsInternal(axis);
		}
		public new SecondaryAxisY GetAxisByName(string name) {
			return (SecondaryAxisY)GetAxisByNameInternal(name);
		}
	}
}
