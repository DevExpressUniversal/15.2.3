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
using System.ComponentModel.Design.Serialization;
using System.Globalization;
using DevExpress.Utils;
using DevExpress.Utils.Serializing;
using DevExpress.XtraCharts.Native;
using DevExpress.XtraCharts.Design;
using DevExpress.XtraCharts.Localization;
using DevExpress.Charts.Native;
namespace DevExpress.XtraCharts {
	[
	TypeConverter(typeof(SwiftPlotDiagramSecondaryAxisXTypeConverter)),
	DesignerSerializer("DevExpress.XtraCharts.Design.ChartItemSerializer," + AssemblyInfo.SRAssemblyChartsExtensions,
					   "System.ComponentModel.Design.Serialization.CodeDomSerializer,System.Design")
	]
	public class SwiftPlotDiagramSecondaryAxisX : SwiftPlotDiagramAxisXBase, ISupportInitialize, IXtraSerializable, ISupportID {
		bool loading = false;
		int id = -1;
		int ISupportID.ID {
			get { return id; }
			set {
				ChartDebug.Assert(value >= 0);
				if(value >= 0)
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
				if(!Loading)
					throw new NotSupportedException(ChartLocalizer.GetString(ChartStringId.MsgInternalPropertyChangeError));
				id = value;
			}
		}
		SwiftPlotDiagramSecondaryAxisX(string name, SwiftPlotDiagram diagram) : base(name, diagram) { 
		}
		public SwiftPlotDiagramSecondaryAxisX(string name) : this(name, null) { 
			CheckName();
		}
		public SwiftPlotDiagramSecondaryAxisX() : this(String.Empty, null) { 
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
			return new SwiftPlotDiagramSecondaryAxisX();
		}
		protected override GridLines CreateGridLines() {
			return new SecondaryGridLinesX(this);
		}
	}
	[
	TypeConverter(typeof(SwiftPlotDiagramSecondaryAxisYTypeConverter)),
	DesignerSerializer("DevExpress.XtraCharts.Design.ChartItemSerializer," + AssemblyInfo.SRAssemblyChartsExtensions,
					   "System.ComponentModel.Design.Serialization.CodeDomSerializer,System.Design")
	]
	public class SwiftPlotDiagramSecondaryAxisY : SwiftPlotDiagramAxisYBase, ISupportInitialize, IXtraSerializable, ISupportID {
		bool loading = false;
		int id = -1;
		int ISupportID.ID {
			get { return id; }
			set {
				ChartDebug.Assert(value >= 0);
				if(value >= 0)
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
				if(!Loading)
					throw new NotSupportedException(ChartLocalizer.GetString(ChartStringId.MsgInternalPropertyChangeError));
				id = value;
			}
		}
		SwiftPlotDiagramSecondaryAxisY(string name, SwiftPlotDiagram diagram) : base(name, diagram) { 
		}
		public SwiftPlotDiagramSecondaryAxisY(string name) : this(name, null) {  
			CheckName();
		}
		public SwiftPlotDiagramSecondaryAxisY() : this(String.Empty, null) {  
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
			return new SwiftPlotDiagramSecondaryAxisY();
		}
		protected override GridLines CreateGridLines() {
			return new SecondaryGridLinesY(this);
		}
	}
	[
	DesignerSerializer("DevExpress.XtraCharts.Design.ChartCollectionSerializer," + AssemblyInfo.SRAssemblyChartsExtensions,
					   "System.ComponentModel.Design.Serialization.CodeDomSerializer,System.Design")
	]
	public class SwiftPlotDiagramSecondaryAxisXCollection : SecondaryAxisCollection {
		protected override string NamePrefix { get { return ChartLocalizer.GetString(ChartStringId.SecondaryAxisXPrefix); } }
		public new SwiftPlotDiagramSecondaryAxisX this[int index] { get { return (SwiftPlotDiagramSecondaryAxisX)List[index]; } }
		internal SwiftPlotDiagramSecondaryAxisXCollection(SwiftPlotDiagram diagram) : base(diagram) {
		}
		protected override Axis2D CreateNewAxis(string name) {
			return new SwiftPlotDiagramSecondaryAxisX(name);
		}
		public int Add(SwiftPlotDiagramSecondaryAxisX axis) {
			return AddInternal(axis);
		}
		public void AddRange(SwiftPlotDiagramSecondaryAxisX[] coll) {
			AddRangeInternal(coll);
		}
		public void Insert(int index, SwiftPlotDiagramSecondaryAxisX axis) {
			base.Insert(index, axis);
		}
		public void Remove(SwiftPlotDiagramSecondaryAxisX axis) {
			RemoveInternal(axis);
		}
		public bool Contains(SwiftPlotDiagramSecondaryAxisX axis) {
			return ContainsInternal(axis);
		}
		public new SwiftPlotDiagramSecondaryAxisX GetAxisByName(string name) {
			return (SwiftPlotDiagramSecondaryAxisX)GetAxisByNameInternal(name);
		}
	}
	[
	DesignerSerializer("DevExpress.XtraCharts.Design.ChartCollectionSerializer," + AssemblyInfo.SRAssemblyChartsExtensions,
					   "System.ComponentModel.Design.Serialization.CodeDomSerializer,System.Design")
	]
	public class SwiftPlotDiagramSecondaryAxisYCollection : SecondaryAxisCollection {
		protected override string NamePrefix { get { return ChartLocalizer.GetString(ChartStringId.SecondaryAxisYPrefix); } }
		public new SwiftPlotDiagramSecondaryAxisY this[int index] { get { return (SwiftPlotDiagramSecondaryAxisY)List[index]; } }
		internal SwiftPlotDiagramSecondaryAxisYCollection(SwiftPlotDiagram diagram) : base(diagram) { }
		protected override Axis2D CreateNewAxis(string name) {
			return new SwiftPlotDiagramSecondaryAxisY(name);
		}
		public int Add(SwiftPlotDiagramSecondaryAxisY axis) {
			return AddInternal(axis);
		}
		public void AddRange(SwiftPlotDiagramSecondaryAxisY[] coll) {
			AddRangeInternal(coll);
		}
		public void Insert(int index, SwiftPlotDiagramSecondaryAxisY axis) {
			base.Insert(index, axis);
		}
		public void Remove(SwiftPlotDiagramSecondaryAxisY axis) {
			RemoveInternal(axis);
		}
		public bool Contains(SwiftPlotDiagramSecondaryAxisY axis) {
			return ContainsInternal(axis);
		}
		public new SwiftPlotDiagramSecondaryAxisY GetAxisByName(string name) {
			return (SwiftPlotDiagramSecondaryAxisY)GetAxisByNameInternal(name);
		}
	}
}
