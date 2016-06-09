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

using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using DevExpress.ExpressApp.Templates.ActionControls;
using DevExpress.XtraBars;
using DevExpress.XtraBars.Painters;
using DevExpress.XtraBars.Styles;
using DevExpress.XtraBars.ViewInfo;
namespace DevExpress.ExpressApp.Win.Templates.Bars {
	[ToolboxItem(false)]
	[Designer("DevExpress.ExpressApp.Win.Design.Bars.XafBarManagerV2Designer, DevExpress.ExpressApp.Win.Design" + XafAssemblyInfo.VersionSuffix + XafAssemblyInfo.AssemblyNamePostfix, typeof(IDesigner))]
	[DesignerCategory("Component")]
	public class XafBarManagerV2 : BarManager {
		private List<IActionControl> actionControls = new List<IActionControl>();
		private List<IActionControlContainer> actionContainers = new List<IActionControlContainer>();
		protected override void FillAdditionalBarItemInfoCollection(BarItemInfoCollection coll) {
			base.FillAdditionalBarItemInfoCollection(coll);
			coll.Add(new BarItemInfo("BarLinkContainerExItem", "Inplace Link Container", 4, typeof(BarLinkContainerExItem), typeof(BarLinkContainerExItemLink), typeof(BarLinkContainerLinkViewInfo), new BarCustomContainerLinkPainter(PaintStyle), true, false));
		}
		protected override bool AllowHotCustomization {
			get { return false; }
		}
		public XafBarManagerV2() { }
		public XafBarManagerV2(IContainer container) : base(container) { }
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		[ReadOnly(false), TypeConverter(typeof(CollectionConverter))]
		public List<IActionControl> ActionControls {
			get { return actionControls; }
			set { actionControls = value; }
		}
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		[ReadOnly(false), TypeConverter(typeof(CollectionConverter))]
		public List<IActionControlContainer> ActionContainers {
			get { return actionContainers; }
			set { actionContainers = value; }
		}
	}
}
