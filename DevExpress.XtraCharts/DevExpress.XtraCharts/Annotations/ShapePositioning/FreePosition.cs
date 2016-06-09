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

using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using System.Drawing;
using System.Reflection;
using DevExpress.XtraCharts.Design;
using DevExpress.XtraCharts.Localization;
using DevExpress.XtraCharts.Native;
using DevExpress.Utils;
using DevExpress.Utils.Design;
using DevExpress.Utils.Serializing;
using System;
using System.Drawing.Design;
using DevExpress.Charts.Native;
namespace DevExpress.XtraCharts {
	[
	TypeConverter(typeof(EnumTypeConverter)),
	ResourceFinder(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile),
	]
	public enum DockCorner {
		LeftTop = DockCornerCore.TopLeft,
		LeftBottom = DockCornerCore.BottomLeft,
		RightTop = DockCornerCore.TopRight,
		RightBottom = DockCornerCore.BottomRight,
	}
	[
	TypeConverter(typeof(FreePositionTypeConverter)),
	DesignerSerializer("DevExpress.XtraCharts.Design.ChartItemSerializer," + AssemblyInfo.SRAssemblyChartsExtensions,
					   "System.ComponentModel.Design.Serialization.CodeDomSerializer,System.Design")
	]
	public class FreePosition : AnnotationShapePosition {
		const DockCorner DefaultDockCorner = DockCorner.LeftTop;
		const int defaultInnerIndent = 10;
		const int defaultOuterIndent = 0;
		readonly RectangleIndents innerIndents;
		readonly RectangleIndents outerIndents;
		IDockTarget dockTarget = null;
		DockCorner dockCorner = DefaultDockCorner;
		string deserializedDockTargetName;
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("FreePositionDockCorner"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.FreePosition.DockCorner"),
		XtraSerializableProperty
		]
		public DockCorner DockCorner {
			get { return dockCorner; }
			set {
				if (value == dockCorner)
					return;
				SendNotification(new ElementWillChangeNotification(this));
				dockCorner = value;
				RaiseControlChanged();
			}
		}
		[
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		NonTestableProperty,
		XtraSerializableProperty
		]
		public string DockTargetName {
			get {
				if (dockTarget != null)
					return dockTarget.Name;
				return String.Empty;
			}
			set { deserializedDockTargetName = value; }
		}
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("FreePositionDockTarget"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.FreePosition.DockTarget"),
		DXDisplayNameIgnore(IgnoreRecursionOnly=true),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		TypeConverter(typeof(AnnotationFreePositionDockTargetTypeConverter)),
		NonTestableProperty,
		XtraSerializableProperty(XtraSerializationVisibility.Hidden)
		]
		public ChartElement DockTarget {
			get { return (ChartElement)dockTarget; }
			set {
				if (value != dockTarget) {
					SendNotification(new ElementWillChangeNotification(this));
					SetDockTarget(value);
					RaiseControlChanged();
				}
			}
		}
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("FreePositionInnerIndents"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.FreePosition.InnerIndents"),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		TypeConverter(typeof(ExpandableObjectConverter)),
		NestedTagProperty,
		XtraSerializableProperty(XtraSerializationVisibility.Content)
		]
		public RectangleIndents InnerIndents { get { return innerIndents; } }
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("FreePositionOuterIndents"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.FreePosition.OuterIndents"),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		TypeConverter(typeof(ExpandableObjectConverter)),
		NestedTagProperty,
		XtraSerializableProperty(XtraSerializationVisibility.Content)
		]
		public RectangleIndents OuterIndents { get { return outerIndents; } }
		#region XtraSerializing
		protected override bool XtraShouldSerialize(string propertyName) {
			if (propertyName == "DockCorner")
				return ShouldSerializeDockCorner();
			if (propertyName == "InnerIndents")
				return ShouldSerializeInnerIndents();
			if (propertyName == "OuterIndents")
				return ShouldSerializeOuterIndents();
			if (propertyName == "DockTargetName")
				return ShouldSerializeDockTargetName();
			return base.XtraShouldSerialize(propertyName);
		}
		#endregion
		#region ShouldSerialize & Reset
		bool ShouldSerializeDockCorner() {
			return dockCorner != DefaultDockCorner;
		}
		void ResetDockCorner() {
			DockCorner = DefaultDockCorner;
		}
		bool ShouldSerializeInnerIndents() {
			return innerIndents.ShouldSerialize();
		}
		bool ShouldSerializeOuterIndents() {
			return outerIndents.ShouldSerialize();
		}
		bool ShouldSerializeDockTargetName() {
			return dockTarget != null;
		}
		protected internal override bool ShouldSerialize() {
			return base.ShouldSerialize() ||
				ShouldSerializeDockCorner() ||
				ShouldSerializeInnerIndents() ||
				ShouldSerializeOuterIndents() ||
				ShouldSerializeDockTargetName();
		}
		#endregion
		public FreePosition() {
			innerIndents = new RectangleIndents(this, defaultInnerIndent);
			outerIndents = new RectangleIndents(this, defaultOuterIndent);
		}
		void EnsureDockTarget() {
			XYDiagram2D diagram = CommonUtils.GetXYDiagram2D(this);
			if (diagram == null || deserializedDockTargetName == String.Empty)
				return;
			dockTarget = diagram.FindPaneByName(deserializedDockTargetName);			
		}
		void SetDockTarget(ChartElement value) {
			if (value != null)
				CommonUtils.CheckDockTarget(value, this);
			dockTarget = value as IDockTarget;
		}
		protected override ChartElement CreateObjectForClone() {
			return new FreePosition();
		}
		protected internal DiagramPoint GetShapeLocation(ZPlaneRectangle viewport) {
			int left, right, bottom, top;
			GetIndents(out left, out top, out right, out bottom);
			switch (DockCorner) {
				case DockCorner.LeftTop:
					return new DiagramPoint(viewport.Left + left, viewport.Bottom + top);
				case DockCorner.LeftBottom:
					return new DiagramPoint(viewport.Left + left, viewport.Top - bottom - Annotation.Height);
				case DockCorner.RightTop:
					return new DiagramPoint(viewport.Right - right - Annotation.Width, viewport.Bottom + top);
				case DockCorner.RightBottom:
					return new DiagramPoint(viewport.Right - right - Annotation.Width, viewport.Top - bottom - Annotation.Height);
				default:
					return new DiagramPoint(viewport.Left, viewport.Top);
			}
		}
		internal void GetIndents(out int left, out int top, out int right, out int bottom) {
			left = innerIndents.Left - outerIndents.Left;
			right = innerIndents.Right - outerIndents.Right;
			bottom = innerIndents.Bottom - outerIndents.Bottom;
			top = innerIndents.Top - outerIndents.Top;
		}
		internal void SetLeftIndent(int value) {
			if (value > 0) {
				innerIndents.SetIndents(value, innerIndents.Top, innerIndents.Right, innerIndents.Bottom);
				outerIndents.SetIndents(0, outerIndents.Top, outerIndents.Right, outerIndents.Bottom);
			}
			else {
				innerIndents.SetIndents(0, innerIndents.Top, innerIndents.Right, innerIndents.Bottom);
				outerIndents.SetIndents(- value, outerIndents.Top, outerIndents.Right, outerIndents.Bottom);
			}
		}
		internal void SetTopIndent(int value) {
			if (value > 0) {
				innerIndents.SetIndents(innerIndents.Left, value, innerIndents.Right, innerIndents.Bottom);
				outerIndents.SetIndents(outerIndents.Left, 0, outerIndents.Right, outerIndents.Bottom);
			}
			else {
				innerIndents.SetIndents(innerIndents.Left, 0, innerIndents.Right, innerIndents.Bottom);
				outerIndents.SetIndents(outerIndents.Left, - value, outerIndents.Right, outerIndents.Bottom);
			}
		}
		internal void SetRightIndent(int value) {
			if (value > 0) {
				innerIndents.SetIndents(innerIndents.Left, innerIndents.Top, value, innerIndents.Bottom);
				outerIndents.SetIndents(outerIndents.Left, outerIndents.Top, 0, outerIndents.Bottom);
			}
			else {
				innerIndents.SetIndents(innerIndents.Left, innerIndents.Top, 0, innerIndents.Bottom);
				outerIndents.SetIndents(outerIndents.Left, outerIndents.Top, - value, outerIndents.Bottom);
			}
		}
		internal void SetBottomIndent(int value) {
			if (value > 0) {
				innerIndents.SetIndents(innerIndents.Left, innerIndents.Top, innerIndents.Right, value);
				outerIndents.SetIndents(outerIndents.Left, outerIndents.Top, outerIndents.Right, 0);
			}
			else {
				innerIndents.SetIndents(innerIndents.Left, innerIndents.Top, innerIndents.Right, 0);
				outerIndents.SetIndents(outerIndents.Left, outerIndents.Top, outerIndents.Right, - value);
			}
		}
		protected internal override void OnEndLoading() {
			EnsureDockTarget();			
		}
		public override void Assign(ChartElement obj) {
			base.Assign(obj);
			FreePosition position = obj as FreePosition;
			if (position == null)
				return;
			dockTarget = position.dockTarget;
			DockTargetName = position.DockTargetName;
			dockCorner = position.dockCorner;
			innerIndents.Assign(position.innerIndents);
			outerIndents.Assign(position.outerIndents);
		}
	}
	public interface IDockTarget {
		string Name { get; }
	}
}
