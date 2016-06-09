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
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;
using DevExpress.Charts.Native;
using DevExpress.Utils.Design;
using DevExpress.Utils.Serializing;
using DevExpress.XtraCharts.Design;
using DevExpress.XtraCharts.Localization;
using DevExpress.XtraCharts.Native;
namespace DevExpress.XtraCharts {
	[
	TypeConverter(typeof(EnumTypeConverter)),
	ResourceFinder(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile),
	]
	public enum ToolTipDockCorner {
		TopRight,
		TopLeft,
		BottomRight,
		BottomLeft
	}
	public abstract class ToolTipPosition : ChartElement {
		[
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		NonTestableProperty,
		XtraSerializableProperty
		]
		public string TypeNameSerializable { get { return this.GetType().Name; } }
		#region XtraSerializing
		protected override bool XtraShouldSerialize(string propertyName) {
			if (propertyName == "TypeNameSerializable")
				return ShouldSerializeTypeNameSerializable();
			return base.XtraShouldSerialize(propertyName);
		}
		#endregion
		#region ShouldSerialize & Reset
		bool ShouldSerializeTypeNameSerializable() {
			return !(this is ToolTipMousePosition);
		}
		protected internal override bool ShouldSerialize() {
			return base.ShouldSerialize() || ShouldSerializeTypeNameSerializable();
		}
		#endregion
		protected internal virtual void OnEndLoading() {
		}
	}
	[
	TypeConverter(typeof(ToolTipMousePositionTypeConverter)),
	DesignerSerializer("DevExpress.XtraCharts.Design.ChartItemSerializer," + AssemblyInfo.SRAssemblyChartsExtensions,
					   "System.ComponentModel.Design.Serialization.CodeDomSerializer,System.Design")
	]
	public class ToolTipMousePosition : ToolTipPosition {
		protected override ChartElement CreateObjectForClone() {
			return new ToolTipMousePosition();
		}
	}
	public abstract class ToolTipPositionWithOffset : ToolTipPosition {
		const int DefaultOffset = 0;
		int offsetX = DefaultOffset;
		int offsetY = DefaultOffset;
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("ToolTipPositionWithOffsetOffsetX"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.ToolTipPositionWithOffset.OffsetX"),
		XtraSerializableProperty
		]
		public int OffsetX {
			get { return offsetX; }
			set {
				if (offsetX != value) {
					SendNotification(new ElementWillChangeNotification(this));
					offsetX = value;
					RaiseControlChanged();
				}
			}
		}
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("ToolTipPositionWithOffsetOffsetY"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.ToolTipPositionWithOffset.OffsetY"),
		XtraSerializableProperty
		]
		public int OffsetY {
			get { return offsetY; }
			set {
				if (offsetY != value) {
					SendNotification(new ElementWillChangeNotification(this));
					offsetY = value;
					RaiseControlChanged();
				}
			}
		}
		#region XtraSerializing
		protected override bool XtraShouldSerialize(string propertyName) {
			if (propertyName == "OffsetX")
				return ShouldSerializeOffsetX();
			if (propertyName == "OffsetY")
				return ShouldSerializeOffsetY();
			return base.XtraShouldSerialize(propertyName);
		}
		#endregion
		#region ShouldSerialize & Reset
		bool ShouldSerializeOffsetX() {
			return this.offsetX != DefaultOffset;
		}
		void ResetOffsetX() {
			OffsetX = DefaultOffset;
		}
		bool ShouldSerializeOffsetY() {
			return this.offsetY != DefaultOffset;
		}
		void ResetOffsetY() {
			OffsetY = DefaultOffset;
		}
		protected internal override bool ShouldSerialize() {
			return base.ShouldSerialize() ||
				ShouldSerializeOffsetX() ||
				ShouldSerializeOffsetY();
		}
		#endregion
		protected Point PointToScreen(Point chartPoint) {
			Control chartControl = ChartContainer as Control;
			if (chartControl == null)
				return chartPoint;
			return chartControl.PointToScreen(chartPoint);
		}
		protected internal abstract Point? CalculateInitialToolTipPosition(ChartFocusedArea focusedArea);
		protected internal abstract ToolTipDockCorner? GetToolTipDockCorner();
		internal Point OffsetPosition(Point point) {
			return new Point(point.X + offsetX, point.Y + offsetY);
		}
		public override void Assign(ChartElement obj) {
			base.Assign(obj);
			ToolTipPositionWithOffset position = obj as ToolTipPositionWithOffset;
			if (position == null)
				return;
			offsetX = position.offsetX;
			offsetY = position.offsetY;
		}
	}
	[
	TypeConverter(typeof(ToolTipRelativePositionTypeConverter)),
	DesignerSerializer("DevExpress.XtraCharts.Design.ChartItemSerializer," + AssemblyInfo.SRAssemblyChartsExtensions,
					   "System.ComponentModel.Design.Serialization.CodeDomSerializer,System.Design")
	]
	public class ToolTipRelativePosition : ToolTipPositionWithOffset {
		protected override ChartElement CreateObjectForClone() {
			return new ToolTipRelativePosition();
		}
		protected internal override Point? CalculateInitialToolTipPosition(ChartFocusedArea focusedArea) {
			if (focusedArea.Element is SeriesPoint) {
				if (focusedArea.RelativePosition == null)
					return null;
				return PointToScreen((Point)focusedArea.RelativePosition);
			}
			else
				return Cursor.Position;
		}
		protected internal override ToolTipDockCorner? GetToolTipDockCorner() {
			return null;
		}
	}
	[
	TypeConverter(typeof(ToolTipFreePositionTypeConverter)),
	DesignerSerializer("DevExpress.XtraCharts.Design.ChartItemSerializer," + AssemblyInfo.SRAssemblyChartsExtensions,
					   "System.ComponentModel.Design.Serialization.CodeDomSerializer,System.Design")
	]
	public class ToolTipFreePosition : ToolTipPositionWithOffset {
		const ToolTipDockCorner DefaultDockCorner = ToolTipDockCorner.TopLeft;
		IDockTarget dockTarget = null;
		ToolTipDockCorner dockCorner = DefaultDockCorner;
		string deserializedDockTargetName;
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("ToolTipFreePositionDockCorner"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.ToolTipFreePosition.DockCorner"),
		XtraSerializableProperty
		]
		public ToolTipDockCorner DockCorner {
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
	DevExpressXtraChartsLocalizedDescription("ToolTipFreePositionDockTarget"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.ToolTipFreePosition.DockTarget"),
		TypeConverter(typeof(ToolTipFreePositionDockTargetTypeConverter)),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),		
		NonTestableProperty,
		XtraSerializableProperty(XtraSerializationVisibility.Hidden)
		]
		public ChartElement DockTarget {
			get { return (ChartElement)dockTarget; }
			set {
				if (value != dockTarget) {
					SendNotification(new ElementWillChangeNotification(this));
					if (value != null)
						CommonUtils.CheckDockTarget(value, this);
					dockTarget = value as IDockTarget;
					RaiseControlChanged();
				}
			}
		}
		public ToolTipFreePosition() {
		}
		#region XtraSerializing
		protected override bool XtraShouldSerialize(string propertyName) {
			if (propertyName == "DockCorner")
				return ShouldSerializeDockCorner();
			if (propertyName == "DockTargetName")
				return ShouldSerializeDockTargetName();
			return base.XtraShouldSerialize(propertyName);
		}
		#endregion
		#region ShouldSerialize & Reset
		bool ShouldSerializeDockCorner() {
			return this.dockCorner != DefaultDockCorner;
		}
		void ResetDockCorner() {
			DockCorner = DefaultDockCorner;
		}
		bool ShouldSerializeDockTargetName() {
			return dockTarget != null;
		}
		protected internal override bool ShouldSerialize() {
			return base.ShouldSerialize() ||
				ShouldSerializeDockCorner() ||
				ShouldSerializeDockTargetName();
		}
		#endregion
		protected override ChartElement CreateObjectForClone() {
			return new ToolTipFreePosition();
		}
		protected internal override Point? CalculateInitialToolTipPosition(ChartFocusedArea focusedArea) {
			Rectangle bounds;
			if (dockTarget == null) {
				if (ContainerAdapter.DisplayBounds.IsEmpty)
					return null;
				bounds = new Rectangle(new Point(0, 0), ContainerAdapter.DisplayBounds.Size);
			}
			else {
				XYDiagramPaneBase pane = dockTarget as XYDiagramPaneBase;
				if (pane == null || !pane.LastMappingBounds.HasValue)
					return null;
				bounds = pane.LastMappingBounds.Value;
			}
			Point p;
			switch (dockCorner) {
				case ToolTipDockCorner.TopLeft:
					p = new Point(bounds.Left, bounds.Top);
					break;
				case ToolTipDockCorner.TopRight:
					p = new Point(bounds.Right, bounds.Top);
					break;
				case ToolTipDockCorner.BottomLeft:
					p = new Point(bounds.Left, bounds.Bottom);
					break;
				case ToolTipDockCorner.BottomRight:
					p = new Point(bounds.Right, bounds.Bottom);
					break;
				default:
					ChartDebug.Fail("Unexpected tool tip dock position");
					return null;
			}
			return PointToScreen(p);
		}
		protected internal override ToolTipDockCorner? GetToolTipDockCorner() {
			return DockCorner;
		}
		protected internal override void OnEndLoading() {
			XYDiagram2D diagram = CommonUtils.GetXYDiagram2D(this);
			if (diagram == null || deserializedDockTargetName == String.Empty)
				return;
			dockTarget = diagram.FindPaneByName(deserializedDockTargetName);
		}
		public override void Assign(ChartElement obj) {
			base.Assign(obj);
			ToolTipFreePosition position = obj as ToolTipFreePosition;
			if (position == null)
				return;
			dockTarget = position.dockTarget;
			DockTargetName = position.DockTargetName;
			dockCorner = position.dockCorner;
		}
	}
}
namespace DevExpress.XtraCharts.Native {
	public static class ToolTipPositionUtils {
		public static Point? CalculateInitialToolTipPosition(ToolTipPositionWithOffset position, ChartFocusedArea focusedArea) {
			return position.CalculateInitialToolTipPosition(focusedArea);	
		}
		public static Point ToolTipOffsetPosition(ToolTipPositionWithOffset position, Point point) {
			return position.OffsetPosition(point);
		}
		public static ToolTipDockCorner? GetToolTipDockCorner(ToolTipPositionWithOffset position) {
			return position.GetToolTipDockCorner();
		}
	}
}
