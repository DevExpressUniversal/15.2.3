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
using System.ComponentModel.Design;
using DevExpress.XtraBars.Commands;
using DevExpress.XtraBars.Commands.Design;
using DevExpress.XtraBars.Design.Forms;
using DevExpress.XtraDiagram.Bars;
namespace DevExpress.XtraDiagram.Design {
	public class DiagramDesignTimeBarsGenerator : DesignTimeBarsGenerator<DiagramControl, DiagramCommandId> {
		readonly Type componentType;
		public DiagramDesignTimeBarsGenerator(IDesignerHost host, IComponent component, Type componentType) : base(host, component) {
			this.componentType = componentType;
		}
		protected override BarGenerationManagerFactory<DiagramControl, DiagramCommandId> CreateBarGenerationManagerFactory() {
			return new DiagramBarGenerationManagerFactory();
		}
		protected override ControlCommandBarControllerBase<DiagramControl, DiagramCommandId> CreateBarController() {
			return new DiagramBarController();
		}
		protected override void EnsureReferences(IDesignerHost designerHost) {
		}
		protected override Component ChooseBarContainer(List<Component> supportedBarContainerCollection) {
			if(supportedBarContainerCollection.Count == 1)
				return supportedBarContainerCollection[0];
			List<Component> managers = new List<Component>();
			foreach(Component component in supportedBarContainerCollection)
				if(component.GetType() == componentType)
					managers.Add(component);
			if(managers.Count == 1) {
				return managers[0];
			}
			using(SelectBarManagerForm form = new SelectBarManagerForm()) {
				form.BarContainerCollection.AddRange(managers);
				form.ShowDialog();
				return form.SelectedContainer;
			}
		}
	}
}
