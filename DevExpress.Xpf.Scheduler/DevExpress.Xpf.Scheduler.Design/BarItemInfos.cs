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

extern alias Platform;
using System.Text;
using Microsoft.Windows.Design;
using Microsoft.Windows.Design.Model;
using Platform::System.Windows;
using Platform::System.Windows.Controls;
using Platform::System.Windows.Data;
using Platform::DevExpress.Xpf.Bars;
using Platform::DevExpress.Xpf.Core;
using Platform::DevExpress.Xpf.Core.Design;
using Platform::DevExpress.Xpf.Editors;
using Platform::DevExpress.Xpf.Core.Native;
using System;
using Platform::DevExpress.Xpf.Editors.Settings;
using Platform::DevExpress.Xpf.Scheduler.UI;
namespace DevExpress.Xpf.Scheduler.Design {
	static class BarItemInfos {
		static readonly BarItemInfo button = new BarButtonItemInfo();
		static readonly BarItemInfo check = new BarCheckItemInfo();
		static readonly SpinEditBarItemInfo spin = new SpinEditBarItemInfo();
		public static BarItemInfo Button { get { return button; } }
		public static BarItemInfo Check { get { return check; } }
		public static SpinEditBarItemInfo Spin { get { return spin; } }
	}
	public class SchedulerEditAppointmentSubItemInfo : SchedulerSubItemInfo {
		public SchedulerEditAppointmentSubItemInfo(Type barSubItemType)
			: base(barSubItemType) {
		}
	}
	public class ColorablePopupMenuSubItemInfo : SchedulerSubItemInfo {
		public ColorablePopupMenuSubItemInfo(Type barSubItemType)
			: base(barSubItemType) {
		}
		public override string XamlItemLinkTag { get { return typeof(ColorablePopupMenuBasedBarItemLink).Name; } }
		public override string XamlLinkPrefix { get { return XmlNamespaceConstants.SchedulerPrefix; } }
	}
	public class SchedulerSubItemInfo : BarSubItemInfo {
		readonly Type barSubItemType;
		public SchedulerSubItemInfo(Type barSubItemType) :
			base(new BarInfoItems(new string[] { }, new BarItemInfo[] { })) {
			this.barSubItemType = barSubItemType;
		}
		public override string XamlPrefix { get { return XmlNamespaceConstants.SchedulerPrefix; } }
		public override string XamlItemTag { get { return barSubItemType.Name; } }
		public override string XamlLinkPrefix { get { return XmlNamespaceConstants.BarsPrefix; } }
		public override string XamlItemLinkTag { get { return "BarSubItemLink"; } }
		public override ModelItem CreateItem(EditingContext context) {
			return ModelFactory.CreateItem(context, barSubItemType);
		}
		public override void SetupItem(CommandBarCreator creator, ModelItem barManager, ModelItem item, ModelItem schedulerControl) {
			ModelItem binding = ModelFactory.CreateItem(item.Context, typeof(Binding));
			binding.Properties["ElementName"].SetValue(schedulerControl.Name);
			item.Properties["SchedulerControl"].SetValue(binding);
		}
		public override void SetupItem(CommandBarXamlCreator creator, StringBuilder writer, string command, string masterControlName) {
			creator.WriteAttributeString(writer, "SchedulerControl", "{Binding ElementName=" + masterControlName + "}");
		}
	}
	public class SpinEditBarItemInfo : BarItemInfo {
		static decimal? MaxEditValue = 15;
		static decimal? MinEditValue = 1;
		public override string XamlPrefix { get { return XmlNamespaceConstants.BarsPrefix; } }
		public override string XamlItemTag { get { return "BarEditItem"; } }
		public override string XamlItemLinkTag { get { return "BarEditItemLink"; } }
		public override ModelItem CreateItem(EditingContext context) {
			return ModelFactory.CreateItem(context, typeof(BarEditItem));
		}
		public override void SetupItem(CommandBarCreator creator, ModelItem barManager, ModelItem item, ModelItem masterControl) {
			item.Properties["EditWidth"].SetValue(50.0);
			ModelItem settings = ModelFactory.CreateItem(item.Context, typeof(SpinEditSettings));
			item.Properties["EditSettings"].SetValue(settings);
			settings.Properties["MinValue"].SetValue(MinEditValue);
			settings.Properties["MaxValue"].SetValue(MaxEditValue);
			ModelItem binding = ModelFactory.CreateItem(item.Context, typeof(Binding));
			binding.Properties["ElementName"].SetValue(masterControl.Name);
		}
		public override void SetupItem(CommandBarXamlCreator creator, StringBuilder writer, string command, string masterControlName) {
		}
	}
}
