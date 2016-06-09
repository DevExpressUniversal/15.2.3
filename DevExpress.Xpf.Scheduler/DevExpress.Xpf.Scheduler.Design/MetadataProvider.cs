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
using Microsoft.Windows.Design.Interaction;
using Microsoft.Windows.Design.Metadata;
using Microsoft.Windows.Design.Policies;
using Microsoft.Windows.Design.Services;
using Microsoft.Windows.Design.Model;
using Microsoft.Windows.Design.Features;
using Microsoft.Windows.Design;
using Microsoft.Windows.Design.PropertyEditing;
using Platform::DevExpress.Xpf.Core.Design;
using DevExpress.Internal;
#if !SL
using DevExpress.Design.SmartTags;
#endif
using System;
using DevExpress.Xpf.Scheduler;
[assembly: ProvideMetadata(typeof(DevExpress.Xpf.Scheduler.Design.RegisterMetadata))]
namespace DevExpress.Xpf.Scheduler.Design {
	internal class RegisterMetadata : MetadataProviderBase {
		protected override System.Reflection.Assembly RuntimeAssembly { get { return typeof(Platform::DevExpress.Xpf.Scheduler.SchedulerControl).Assembly; } }
		protected override string ToolboxCategoryPath {
			get {
#if SILVERLIGHT
				return Platform::AssemblyInfo.DXTabNameScheduling;
#else
				return AssemblyInfo.DXTabWpfScheduling;
#endif
			}
		}
		protected override void PrepareAttributeTable(AttributeTableBuilder builder) {
			base.PrepareAttributeTable(builder);
			FrameworkElementSmartTagPropertiesViewModel.RegisterPropertyLineProvider(new SchedulerControlPropertyLinesProvider());
			RegisterNewItemTypes(builder);
			RegisterFeatures(builder);
#if !DEBUG && SL
#endif
		}
		void RegisterFeatures(AttributeTableBuilder builder) {
			builder.AddAttributes(typeof(Platform::DevExpress.Xpf.Scheduler.SchedulerControl), new FeatureAttribute(typeof(SchedulerContextMenuProvider)));
			builder.AddAttributes(typeof(Platform::DevExpress.Xpf.Scheduler.SchedulerControl), new FeatureAttribute(typeof(SchedulerControlDefaultInitializer)));
#if DEBUG
			builder.AddCustomAttributes(typeof(Platform::DevExpress.Xpf.Scheduler.SchedulerControl), new FeatureAttribute(typeof(SchedulerAdornerProvider)));
#endif
		}
		void RegisterNewItemTypes(AttributeTableBuilder builder) {
			builder.AddCustomAttributes(typeof(Platform::DevExpress.Xpf.Editors.DateNavigator.DateNavigator), "StyleSettings", new NewItemTypesAttribute(typeof(Platform::DevExpress.Xpf.Scheduler.SchedulerDateNavigatorStyleSettings)));
			builder.AddCustomAttributes(typeof(Platform::DevExpress.Xpf.Scheduler.SchedulerControl), "DayView", new NewItemTypesAttribute(typeof(Platform::DevExpress.Xpf.Scheduler.DayView)));
			builder.AddCustomAttributes(typeof(Platform::DevExpress.Xpf.Scheduler.SchedulerControl), "WorkWeekView", new NewItemTypesAttribute(typeof(Platform::DevExpress.Xpf.Scheduler.WorkWeekView)));
			builder.AddCustomAttributes(typeof(Platform::DevExpress.Xpf.Scheduler.SchedulerControl), "WeekView", new NewItemTypesAttribute(typeof(Platform::DevExpress.Xpf.Scheduler.WeekView)));
			builder.AddCustomAttributes(typeof(Platform::DevExpress.Xpf.Scheduler.SchedulerControl), "MonthView", new NewItemTypesAttribute(typeof(Platform::DevExpress.Xpf.Scheduler.MonthView)));
			builder.AddCustomAttributes(typeof(Platform::DevExpress.Xpf.Scheduler.SchedulerControl), "TimelineView", new NewItemTypesAttribute(typeof(Platform::DevExpress.Xpf.Scheduler.TimelineView)));
			builder.AddCustomAttributes(typeof(Platform::DevExpress.Xpf.Scheduler.SchedulerControl), "Storage", new NewItemTypesAttribute(typeof(Platform::DevExpress.Xpf.Scheduler.SchedulerStorage)));
		}
	}
}
