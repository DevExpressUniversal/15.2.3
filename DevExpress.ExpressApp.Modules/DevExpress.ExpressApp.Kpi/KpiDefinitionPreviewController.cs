#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       eXpressApp Framework                                        }
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
using System.Text;
using DevExpress.ExpressApp.Editors;
namespace DevExpress.ExpressApp.Kpi {
	public class KpiDefinitionPreviewController : ViewController<DetailView> {
		internal ListPropertyEditor ObjectsPreviewEditor {
			get { return View.FindItem("Objects") as ListPropertyEditor; }
		}
		private bool IsValidKpiDefinition(IKpiDefinition kpiDefinition) {
			return kpiDefinition != null && kpiDefinition.TargetObjectType != null;
		}
		private void ObjectSpace_ObjectChanged(object sender, ObjectChangedEventArgs e) {
			if(object.Equals(View.CurrentObject, e.Object) && e.PropertyName == "TargetObjectType") {
				ListPropertyEditor objectsPreview = ObjectsPreviewEditor;
				if(objectsPreview != null && IsValidKpiDefinition(View.CurrentObject as IKpiDefinition)) {
					if(objectsPreview.Frame != null) {
						objectsPreview.BreakLinksToControl(true);
						objectsPreview.Frame.SetView(null);
					}
					objectsPreview.Model.View = DrilldownController.GetDrilldownListViewModel(Application, View.CurrentObject as IKpiDefinition);
					objectsPreview.CreateControl();
				}
			}
		}
		protected override void OnActivated() {
			base.OnActivated();
			ListPropertyEditor objectsPreview = ObjectsPreviewEditor;
			if(objectsPreview != null && IsValidKpiDefinition(View.CurrentObject as IKpiDefinition)) {
				objectsPreview.Model.View = DrilldownController.GetDrilldownListViewModel(Application, View.CurrentObject as IKpiDefinition);
			}
			View.ObjectSpace.ObjectChanged += new EventHandler<ObjectChangedEventArgs>(ObjectSpace_ObjectChanged);
		}
		protected override void OnDeactivated() {
			View.ObjectSpace.ObjectChanged -= new EventHandler<ObjectChangedEventArgs>(ObjectSpace_ObjectChanged);
			base.OnDeactivated();
		}
		public KpiDefinitionPreviewController() {
			TargetObjectType = typeof(IKpiDefinition);
		}
	}
}
