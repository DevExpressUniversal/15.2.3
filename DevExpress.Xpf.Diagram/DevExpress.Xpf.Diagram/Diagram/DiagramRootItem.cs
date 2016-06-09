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

using DevExpress.Diagram.Core;
using DevExpress.Xpf.Diagram.Native;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Media;
using DevExpress.Mvvm.UI.Native;
namespace DevExpress.Xpf.Diagram {
	public class DiagramRoot : DiagramContainer, ILayer, IDiagramRoot {
		static DiagramRoot() {
			DependencyPropertyRegistrator<DiagramRoot>.New()
				.FixPropertyValue(CanDeleteProperty, false)
				.FixPropertyValue(CanResizeProperty, false)
				.FixPropertyValue(CanMoveProperty, false)
				.FixPropertyValue(AdjustBoundsBehaviorProperty, AdjustBoundaryBehavior.None)
				.FixPropertyValue(AngleProperty, 0d)
				.FixPropertyValue(CanRotateProperty, false)
				.OverrideMetadata(ClipToBoundsProperty, false)
				.OverrideMetadata(CanSelectProperty, false)
				.OverrideDefaultStyleKey();
		}
		protected internal readonly DiagramControl diagramCore;
		public DiagramRoot(DiagramControl diagram) {
			this.diagramCore = diagram;
			VerticalAlignment = System.Windows.VerticalAlignment.Top;
			HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
		}
		protected sealed override DiagramContainerController CreateContainerController() {
			return CreateRootController();
		}
		protected virtual DiagramRootController CreateRootController() {
			return new DiagramRootController(this);
		}
		void ILayer.Update(Point offset, Point location, Size viewport, double zoomFactor) {
			RenderTransform = new MatrixTransform(LayersHostController.CreateLogicToDisplayTransform(offset, zoomFactor));
		}
		protected override IEnumerable<PropertyDescriptor> GetEditablePropertiesCore() {
			return this.Controller().GetProxyDiagramProperties();
		}
		#region IDiagramRoot
		IDiagramControl IDiagramRoot.Diagram { get { return diagramCore; } }
		#endregion
	}
}
