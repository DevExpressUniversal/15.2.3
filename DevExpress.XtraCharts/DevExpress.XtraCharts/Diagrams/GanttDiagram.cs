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
using System.Globalization;
using System.Reflection;
using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using DevExpress.Utils.Serializing;
using DevExpress.XtraCharts.Design;
using DevExpress.Charts.Native;
using System.Collections.Generic;
namespace DevExpress.XtraCharts {
	[
	TypeConverter(typeof(GanttDiagramTypeConverter)),
	DesignerSerializer("DevExpress.XtraCharts.Design.ChartItemSerializer," + AssemblyInfo.SRAssemblyChartsExtensions,
					   "System.ComponentModel.Design.Serialization.CodeDomSerializer,System.Design")
	]
	public class GanttDiagram : XYDiagram {
		const int DefaultAxisXLength = 500;
		const int DefaultAxisYLength = 500;
		protected override bool DefaultRotatedValue { get { return true; } }		
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("GanttDiagramRotated"),
#endif
		Category("Behavior"),
		RefreshProperties(RefreshProperties.All),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		EditorBrowsable(EditorBrowsableState.Never),
		Browsable(false),
		XtraSerializableProperty(XtraSerializationVisibility.Hidden)
		]
		public override bool Rotated { get { return base.Rotated; } set {} }
		public GanttDiagram() : base() {
		}
		protected override AxisX CreateAxisX() {
			return new GanttAxisX(this);
		}
		protected override ChartElement CreateObjectForClone() {
			return new GanttDiagram();
		}
		protected override int GetArgumentAxisLength(XYDiagramPaneBase pane) {
			if (pane.LastMappingBounds.HasValue)
				return ActualRotated ? pane.LastMappingBounds.Value.Width : pane.LastMappingBounds.Value.Height;
			return DefaultAxisYLength;
		}
		protected override int GetValueAxisLength(XYDiagramPaneBase pane) {
			if(pane.LastMappingBounds.HasValue)
				return ActualRotated ? pane.LastMappingBounds.Value.Height : pane.LastMappingBounds.Value.Width;
			return DefaultAxisXLength;
		}
		protected override IList<IAxisData> GetArgumentAxesWithAutomaticUnits(PaneAxesContainer container) {
			return container.AxesY;
		}
		protected override IList<IAxisData> GetValueAxesWithAutomaticUnits(PaneAxesContainer container) {
			return container.AxesX;
		}
		public override void Assign(ChartElement obj) {
			base.Assign(obj);
			GanttDiagram diagram = obj as GanttDiagram;
			if (diagram == null)
				return;
			SetRotated(true);
		}
	}
}
