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
using System.Drawing.Design;
using DevExpress.Utils.Serializing;
using DevExpress.Utils.Serializing.Helpers;
using DevExpress.XtraCharts.Design;
using DevExpress.XtraCharts.Localization;
using DevExpress.XtraCharts.Native;
using DevExpress.Utils.Design;
using System.Collections;
using DevExpress.Charts.Native;
namespace DevExpress.XtraCharts {
	[
	DesignerSerializer("DevExpress.XtraCharts.Design.ChartItemSerializer," + AssemblyInfo.SRAssemblyChartsExtensions,
					   "System.ComponentModel.Design.Serialization.CodeDomSerializer,System.Design")
	]
	public class ToolTipOptions : ChartElement, IXtraSupportCreateContentPropertyValue, IXtraSupportAfterDeserialize {
		const bool DefaultShowForSeries = false;
		const bool DefaultShowForPoints = true;
		bool loading = false;
		bool showForSeries = DefaultShowForSeries;
		bool showForPoints = DefaultShowForPoints;
		ToolTipPosition toolTipPosition;
		readonly AspxSerializerWrapper<ToolTipPosition> toolTipPositionSerializerWrapper;
		protected internal override bool Loading { get { return base.Loading || loading; } }
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("ToolTipOptionsShowForSeries"),
#endif
		TypeConverter(typeof(BooleanTypeConverter)),
		XtraSerializableProperty
		]
		public bool ShowForSeries {
			get { return showForSeries; }
			set {
				if (value != showForSeries) {
					SendNotification(new ElementWillChangeNotification(this));
					showForSeries = value;
					RaiseControlChanged();
				}
			}
		}
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("ToolTipOptionsShowForPoints"),
#endif
		TypeConverter(typeof(BooleanTypeConverter)),
		XtraSerializableProperty
		]
		public bool ShowForPoints {
			get { return showForPoints; }
			set {
				if (value != showForPoints) {
					SendNotification(new ElementWillChangeNotification(this));
					showForPoints = value;
					RaiseControlChanged();
				}
			}
		}
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("ToolTipOptionsToolTipPosition"),
#endif
		Editor("DevExpress.XtraCharts.Design.ToolTipPositionEditor," + AssemblyInfo.SRAssemblyChartsWizard, typeof(UITypeEditor)),
		XtraSerializableProperty(XtraSerializationVisibility.Content, true),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Visible),
		TypeConverter(typeof(ExpandableObjectConverter)),
		RefreshProperties(RefreshProperties.All)
		]
		public ToolTipPosition ToolTipPosition {
			get { return toolTipPosition; }
			set {
				if (value == null)
					throw new ArgumentException(ChartLocalizer.GetString(ChartStringId.MsgIncorrectToolTipPosition));
				SendNotification(new ElementWillChangeNotification(this));
				SetToolTipPosition(value);
				RaiseControlChanged();
			}
		}
		[
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		NonTestableProperty,
		NestedTagProperty
		]
		public IList ToolTipPositionSerializable { get { return toolTipPositionSerializerWrapper; } }
		public ToolTipOptions()
			: this(null) {
		}
		public ToolTipOptions(ChartElement owner)
			: base(owner) {
			SetToolTipPosition(new ToolTipMousePosition());
			toolTipPositionSerializerWrapper = new AspxSerializerWrapper<ToolTipPosition>(delegate() { return ToolTipPosition; },
				delegate(ToolTipPosition value) { ToolTipPosition = value; });
		}
		#region XtraSerializing
		protected override bool XtraShouldSerialize(string propertyName) {
			if (propertyName == "ShowForSeries")
				return ShouldSerializeShowForSeries();
			if (propertyName == "ShowForPoints")
				return ShouldSerializeShowForPoints();
			if (propertyName == "ToolTipPosition")
				return true;
			return base.XtraShouldSerialize(propertyName);
		}
		object IXtraSupportCreateContentPropertyValue.Create(XtraItemEventArgs e) {
			return e.Item.Name == "ToolTipPosition" ? XtraSerializingUtils.GetContentPropertyInstance(e, SerializationUtils.ExecutingAssembly, SerializationUtils.PublicNamespace) : null;
		}
		void IXtraSupportAfterDeserialize.AfterDeserialize(XtraItemEventArgs e) {
			if (e.Item.Name == "ToolTipPosition")
				ToolTipPosition = e.Item.Value as ToolTipPosition;
		}
		#endregion
		#region ShouldSerialize & Reset
		bool ShouldSerializeShowForPoints() {
			return this.showForPoints != DefaultShowForPoints;
		}
		void ResetShowForPoints() {
		   ShowForPoints = DefaultShowForPoints;
		}
		bool ShouldSerializeShowForSeries() {
			return this.showForSeries != DefaultShowForSeries;
		}
		void ResetShowForSeries() {
			ShowForSeries = DefaultShowForSeries;
		}
		bool ShouldSerializeToolTipPosition() {
			Chart chart = Owner != null ? Owner as Chart : null;
			return chart != null && chart.Container != null && chart.Container.ControlType != ChartContainerType.WebControl && toolTipPosition.ShouldSerialize();
		}
		bool ShouldSerializeToolTipPositionSerializable() {
			Chart chart = Owner != null ? Owner as Chart : null;
			return chart != null && chart.Container != null && chart.Container.ControlType == ChartContainerType.WebControl && toolTipPosition.ShouldSerialize();
		}
		protected internal override bool ShouldSerialize() {
			return base.ShouldSerialize() ||
				ShouldSerializeToolTipPosition() ||
				ShouldSerializeShowForPoints() ||
				ShouldSerializeShowForSeries();
		}
		#endregion
		void SetToolTipPosition(ToolTipPosition position) {
			toolTipPosition = position;
			toolTipPosition.Owner = this;
		}
		void AssignToolTipPosition(ToolTipPosition position) {
			if (!toolTipPosition.GetType().Equals(position.GetType()))
				SetToolTipPosition((ToolTipPosition)position.Clone());
			else
				toolTipPosition.Assign(position);
		}	   
		protected override ChartElement CreateObjectForClone() {
			return new ToolTipOptions();
		}
		internal void OnEndLoading() {
			toolTipPosition.OnEndLoading();
		}	  
		public override void Assign(ChartElement obj) {
			base.Assign(obj);
			ToolTipOptions toolTipOptions = obj as ToolTipOptions;
			if (toolTipOptions != null) {
				showForPoints = toolTipOptions.showForPoints;
				showForSeries = toolTipOptions.showForSeries;
				AssignToolTipPosition(toolTipOptions.toolTipPosition);
				OnEndLoading();
			}
		}
	}
}
