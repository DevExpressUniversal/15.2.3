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

using System.Collections.Generic;
using System;
using DevExpress.XtraCharts.Wizard.SeriesViewControls;
using DevExpress.XtraCharts.Wizard.ChartDiagramControls;
using DevExpress.XtraCharts.Wizard.SeriesLabelsControls;
using DevExpress.XtraCharts.Wizard.ChartAxesControls;
namespace DevExpress.XtraCharts.Native {
	internal class WizardControlBaseFactory<BaseType, ControlType> {
		Dictionary<Type, Type> hashTable = new Dictionary<Type, Type>();
		protected Type this[Type index] {
			get {
				Type type = GetControlType(index);
				return type != null ? type : typeof(BaseType);
			}
		}
		public WizardControlBaseFactory() {
			Initialize();
		}
		protected void Register(Type baseType, Type controlType) {
			hashTable.Add(baseType, controlType);
		}
		public virtual ControlType CreateInstance(BaseType type) {
			return (ControlType)Activator.CreateInstance(this[type.GetType()]);
		}
		protected virtual void Initialize() {
		}
		Type GetControlType(Type type) {
			if (hashTable.ContainsKey(type))
				return this.hashTable[type];
			Type baseType = type;
			do {
				baseType = baseType.BaseType;
				if (this.hashTable.ContainsKey(baseType))
					return this.hashTable[baseType];
			}
			while (baseType != null);
			return null;
		}
	}
	internal class SeriesViewControlFactory : WizardControlBaseFactory<SeriesViewBase, SeriesViewControlBase> {	
		protected override void Initialize() {
			Register(typeof(XYDiagram2DSeriesViewBase), typeof(XYDiagramSeriesViewBaseControl));
			Register(typeof(RadarSeriesViewBase), typeof(RadarSeriesViewControl));
			Register(typeof(XYDiagram3DSeriesViewBase), typeof(XYDiagram3DSeriesViewBaseControl));
			Register(typeof(SimpleDiagramSeriesViewBase), typeof(SimpleDiagramSeriesViewBaseControl));
		}
	}
	internal class DiagramControlFactory : WizardControlBaseFactory<Diagram, DiagramControlBase> {
		protected override void Initialize() {
			Register(typeof(XYDiagram2D), typeof(XYDiagramControl));
			Register(typeof(XYDiagram3D), typeof(XYDiagram3DControl));
			Register(typeof(SimpleDiagram), typeof(PieDiagramControl));
			Register(typeof(SimpleDiagram3D), typeof(Pie3DDiagramControl));
			Register(typeof(RadarDiagram), typeof(RadarDiagramControl));
		}
		public override DiagramControlBase CreateInstance(Diagram type) {
			return type == null ? new DiagramNotFoundControl() : base.CreateInstance(type);
		}
	}
}
