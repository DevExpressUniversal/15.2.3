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
using System.Globalization;
using System.Reflection;
using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using DevExpress.Utils.Design;
using DevExpress.Utils.Serializing;
using DevExpress.XtraCharts.Design;
using DevExpress.XtraCharts.Localization;
using DevExpress.XtraCharts.Native;
namespace DevExpress.XtraCharts {
	public abstract class Relation : ChartElement {
		SeriesPoint childPoint;
		int childPointID = -1;
		SeriesPointRelationCollection Relations { get { return ParentPoint != null ? ParentPoint.Relations : null; } }
		[
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		XtraSerializableProperty
		]
		public int ChildPointID { 
			get { return childPointID; } 
			set { 
				if (Owner != null && !Loading)
					throw new NotSupportedException(ChartLocalizer.GetString(ChartStringId.MsgInternalPropertyChangeError));
				childPointID = value; 
			} 
		}		
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("RelationChildPoint"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.Relation.ChildPoint"),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		TypeConverter(typeof(ExpandableObjectConverter)),
		XtraSerializableProperty(XtraSerializationVisibility.Hidden)
		]
		public SeriesPoint ChildPoint {
			get {
				if (Relations == null)
					return childPoint;
				return Relations.Series.Points.GetByID(childPointID);
			}
			set {
				if (Relations == null) {
					childPoint = value;
					return;
				}
				Relations.TestPoint(value);
				SendNotification(new ElementWillChangeNotification(this));
				childPoint = value;
				childPointID = value.SeriesPointID;
				RaiseControlChanged();
			}
		}
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("RelationParentPoint"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.Relation.ParentPoint"),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		TypeConverter(typeof(ExpandableObjectConverter)),
		XtraSerializableProperty(XtraSerializationVisibility.Hidden)
		]
		public SeriesPoint ParentPoint { get { return Owner as SeriesPoint; } }
		protected Relation(): base() {
		}
		protected Relation(SeriesPoint childPoint) : this(childPoint.SeriesPointID) {
			this.childPoint = childPoint;
		}		
		protected Relation(int childPointID) : this() {
			this.childPointID = childPointID;
		}
		internal void InitializeChildPointId() {
			if(childPoint != null)
				childPointID = childPoint.SeriesPointID;
		}
		public override void Assign(ChartElement obj) {
			base.Assign(obj);
			Relation relation = obj as Relation;
			if (relation == null)
				return;
			childPointID = relation.childPointID; 
			childPoint = relation.childPoint;
		}
	}
	[
	TypeConverter(typeof(EnumTypeConverter)),
	ResourceFinder(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile),
	]
	public enum TaskLinkType {
		FinishToStart,
		StartToStart,
		FinishToFinish,
		StartToFinish
	}
	[
	TypeConverter(typeof(TaskLink.TaskLinkTypeConverter)),
	DesignerSerializer("DevExpress.XtraCharts.Design.ChartItemSerializer," + AssemblyInfo.SRAssemblyChartsExtensions,
					   "System.ComponentModel.Design.Serialization.CodeDomSerializer,System.Design")
	]
	public class TaskLink : Relation {
		#region Nested class: TaskLinkTypeConverter
		internal class TaskLinkTypeConverter : System.ComponentModel.TypeConverter {
			public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType) {
				if (destinationType == typeof(InstanceDescriptor))
					return true;
				return base.CanConvertTo(context, destinationType);
			}
			public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType) {
				if (destinationType == typeof(InstanceDescriptor)) {
					ConstructorInfo ci = GetConstructorInfo(value);
					return new InstanceDescriptor(ci, GetConstructorParams(value), true);
				}
				return base.ConvertTo(context, culture, value, destinationType);
			}
			ConstructorInfo GetConstructorInfo(object value) {
				TaskLink link = (TaskLink)value;
				return link.ShouldSerializeLinkType() ?
					typeof(TaskLink).GetConstructor(new Type[] {typeof(int), typeof(TaskLinkType)}) :
					typeof(TaskLink).GetConstructor(new Type[] {typeof(int)});
				}
			object[] GetConstructorParams(object value) {
				TaskLink link = (TaskLink)value;
				ArrayList list = new ArrayList();
				list.Add(link.ChildPointID);
				if (link.ShouldSerializeLinkType())
					list.Add(link.LinkType);
				return (object[])list.ToArray(typeof(object));
			}
		}
		#endregion
		const TaskLinkType DefaultLinkType = TaskLinkType.FinishToStart;
		TaskLinkType linkType = DefaultLinkType;
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("TaskLinkLinkType"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.TaskLink.LinkType"),
		XtraSerializableProperty
		]
		public TaskLinkType LinkType {
			get { return linkType; }
			set {
				SendNotification(new ElementWillChangeNotification(this));
				linkType = value;
				RaiseControlChanged();
			}
		}
		public TaskLink() : base() {
		}
		public TaskLink(SeriesPoint point) : this(point, DefaultLinkType) {
		}
		public TaskLink(SeriesPoint point, TaskLinkType linkType) : base(point) {
			this.linkType = linkType;
		}
		[EditorBrowsable(EditorBrowsableState.Never)]
		public TaskLink(int childPointID) : base(childPointID) {
		}
		[EditorBrowsable(EditorBrowsableState.Never)]
		public TaskLink(int childPointID, TaskLinkType linkType) : base(childPointID) {
			this.linkType = linkType;
		}
		#region XtraSerializing
		protected override bool XtraShouldSerialize(string propertyName) {
			if(propertyName == "LinkType")
				return ShouldSerializeLinkType();
			return base.XtraShouldSerialize(propertyName);
		}
		#endregion
		#region ShouldSerialize & Reset
		bool ShouldSerializeLinkType() {
			return linkType != DefaultLinkType;
		}
		void ResetLinkType() {
			LinkType = DefaultLinkType;
		}
		protected internal override bool ShouldSerialize() {
			return true;
		}
		#endregion
		protected override ChartElement CreateObjectForClone() {
			return new TaskLink();
		}
		public override void Assign(ChartElement obj) {
			base.Assign(obj);
			TaskLink link = obj as TaskLink;
			if (link == null)
				return;
			linkType = link.linkType;
		}
	}
}
