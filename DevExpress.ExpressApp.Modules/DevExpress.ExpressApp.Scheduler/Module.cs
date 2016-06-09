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
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Updating;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.Base.General;
using DevExpress.XtraScheduler;
namespace DevExpress.ExpressApp.Scheduler {
	[Description("Provides the capability to present and manage scheduling information in an efficient manner.")]
	[ToolboxBitmap(typeof(SchedulerModuleBase), "Resources.Toolbox_Module_Scheduler.ico")]
	public partial class SchedulerModuleBase : ModuleBase, IModelXmlConverter {
		private System.ComponentModel.IContainer components = null;
		public const string VisibleResourcesCountAttribute = "VisibleResourcesCount";
		private void InitializeComponent() {
		}
		protected void RegisterNotificationsProvider() {
			INotificationsServiceOwner storage = Application.Modules.FirstOrDefault(m => m is INotificationsServiceOwner) as INotificationsServiceOwner;
			if(storage != null) {
				storage.NotificationsService.RegisterNotificationsProvider(NotificationsProvider);
			}
		}
		private HashSet<ITypeInfo> CollectReminderEventTypes() {
			HashSet<ITypeInfo> eventsTypes = new HashSet<ITypeInfo>();
			foreach(ITypeInfo typeInfo in XafTypesInfo.Instance.FindTypeInfo(typeof(IEvent)).Implementors) {
				if(typeInfo.IsInterface || !((TypesInfo)XafTypesInfo.Instance).IsRegisteredEntity(typeInfo.Type)) {
					continue;
				}
				if(typeInfo.Implements<IReminderEvent>() && ((!typeInfo.HasDescendants))) {
					eventsTypes.Add(typeInfo);
				}
			}
			NotificationEventTypesEventArgs args = new NotificationEventTypesEventArgs(eventsTypes);
			if(CustomizeReminderEventTypes != null) {
				CustomizeReminderEventTypes(this, args);
			}
			return args.EventsTypes;
		}
		private NotificationsProvider CreateNotificationsProvider(XafApplication xafApplication) {
			INotificationsServiceOwner storage = xafApplication.Modules.FirstOrDefault(m => m is INotificationsServiceOwner) as INotificationsServiceOwner;
			if(storage != null) {
				HashSet<ITypeInfo> eventsTypes = CollectReminderEventTypes();
				return new NotificationsProvider(xafApplication, eventsTypes);
			}
			return null;
		}
		protected override IEnumerable<Type> GetRegularTypes() {
			return new Type[]{
			typeof(IModelListViewScheduler),
			typeof(ResourceClassDomainLogic)
			};
		}
		protected override IEnumerable<Type> GetDeclaredExportedTypes() {
			return Type.EmptyTypes;
		}
		protected override IEnumerable<Type> GetDeclaredControllerTypes() {
			return new Type[] {
				typeof(DevExpress.ExpressApp.Scheduler.SchedulerChangedOcurrenceController),
				typeof(DevExpress.ExpressApp.Scheduler.SchedulerResourceController),
				typeof(DevExpress.ExpressApp.Scheduler.NotificationsEventController)
			};
		}
		protected override void RegisterEditorDescriptors(List<EditorDescriptor> editorDescriptors) {
			editorDescriptors.Add(new ListEditorDescriptor(new AliasRegistration(EditorAliases.SchedulerListEditor, typeof(IEvent), true)));
			editorDescriptors.Add(new PropertyEditorDescriptor(new AliasRegistration(EditorAliases.SchedulerRecurrenceInfoPropertyEditor, typeof(String), false)));
			editorDescriptors.Add(new PropertyEditorDescriptor(new AliasRegistration(EditorAliases.SchedulerLabelPropertyEditor, typeof(int), false)));
			editorDescriptors.Add(new PropertyEditorDescriptor(new AliasRegistration(EditorAliases.SchedulerStatusPropertyEditor, typeof(int), false)));
		}
		public SchedulerModuleBase() {
			InitializeComponent();
		}
		public override ICollection<Type> GetXafResourceLocalizerTypes() {
			ICollection<Type> result = new List<Type>();
			result.Add(typeof(SchedulerModuleBaseLocalizer));
			return result;
		}
		public override void ExtendModelInterfaces(ModelInterfaceExtenders extenders) {
			base.ExtendModelInterfaces(extenders);
			extenders.Add<IModelListView, IModelListViewScheduler>();
		}
		public override IEnumerable<ModuleUpdater> GetModuleUpdaters(IObjectSpace objectSpace, Version versionFromDB) {
			return ModuleUpdater.EmptyModuleUpdaters;
		}
		protected override void Dispose(bool disposing) {
			if(disposing && (components != null)) {
				components.Dispose();
			}
			NotificationsProvider schedulerProvider = ValueManager.GetValueManager<NotificationsProvider>("NotificationsProvider").Value;
			if(schedulerProvider != null) {
				schedulerProvider.Dispose();
			}
			base.Dispose(disposing);
		}
		#region IModelXmlConverter Members
		public void ConvertXml(ConvertXmlParameters convertXmlArgs) {
			const string ResourceClassNameAttr = "ResourceClassName";
			const string ResourceClassAttr = "ResourceClass";
			if(convertXmlArgs.Node is IModelViews && convertXmlArgs.XmlNodeName == "ListView") {
				if(convertXmlArgs.Values.ContainsKey(ResourceClassNameAttr) &&
					!convertXmlArgs.Values.ContainsKey(ResourceClassAttr)) {
					convertXmlArgs.Values.Add(ResourceClassAttr, convertXmlArgs.Values[ResourceClassNameAttr]);
				}
			}
		}
		#endregion
		[Browsable(false)]
		public NotificationsProvider NotificationsProvider {
			get {
				NotificationsProvider schedulerProvider = ValueManager.GetValueManager<NotificationsProvider>("NotificationsProvider").Value;
				if(schedulerProvider == null) {
					schedulerProvider = CreateNotificationsProvider(Application);
					ValueManager.GetValueManager<NotificationsProvider>("NotificationsProvider").Value = schedulerProvider;
				}
				return schedulerProvider;
			}
		}
		public event EventHandler<NotificationEventTypesEventArgs> CustomizeReminderEventTypes;
	}
	public class NotificationEventTypesEventArgs : HandledEventArgs {
		public NotificationEventTypesEventArgs(HashSet<ITypeInfo> eventsTypes) {
			this.EventsTypes = eventsTypes;
		}
		public HashSet<ITypeInfo> EventsTypes {
			get;
			set;
		}
	}
	[DomainLogic(typeof(IModelListViewScheduler))]
	public static class ResourceClassDomainLogic {
		public static IModelClass Get_ResourceClass(IModelListView modelListView) {
			IList<IModelClass> modelClass = Get_ResourceClasses(modelListView);
			if(modelClass.Count > 0) {
				return modelClass[0];
			}
			return null;
		}
		public static IModelList<IModelClass> Get_ResourceClasses(IModelListView modelListView) {
			CalculatedModelNodeList<IModelClass> list = new CalculatedModelNodeList<IModelClass>();
			if(modelListView.Application != null) {
				foreach(IModelClass modelClass in modelListView.Application.BOModel) {
					if(typeof(DevExpress.Persistent.Base.General.IResource).IsAssignableFrom(modelClass.TypeInfo.Type) && modelClass.TypeInfo.IsDomainComponent) {
						list.Add(modelClass);
					}
				}
			}
			return list;
		}
	}
	public class ModelListViewSchedulerVisibilityCalculator : IModelIsVisible {
		public bool IsVisible(IModelNode node, String propertyName) {
			IModelListView modelListView = node as IModelListView;
			if(modelListView != null && modelListView.EditorType != null) {
				return typeof(SchedulerListEditorBase).IsAssignableFrom(modelListView.EditorType);
			}
			else return false;
		}
	}
	public interface IModelListViewScheduler {
		[Browsable(false)]
		IModelList<IModelClass> ResourceClasses { get; }
		[DataSourceProperty("ResourceClasses")]
		[ModelBrowsable(typeof(ModelListViewSchedulerVisibilityCalculator))]
		[
#if !SL
	DevExpressExpressAppSchedulerLocalizedDescription("IModelListViewSchedulerResourceClass"),
#endif
 Category("Data")]
		IModelClass ResourceClass { get; set; }
		[ModelBrowsable(typeof(ModelListViewSchedulerVisibilityCalculator))]
		[
#if !SL
	DevExpressExpressAppSchedulerLocalizedDescription("IModelListViewSchedulerSchedulerViewType"),
#endif
 Category("Appearance")]
		SchedulerViewType SchedulerViewType { get; set; }
		[DefaultValue(5)]
		[ModelBrowsable(typeof(ModelListViewSchedulerVisibilityCalculator))]
		[
#if !SL
	DevExpressExpressAppSchedulerLocalizedDescription("IModelListViewSchedulerVisibleResourcesCount"),
#endif
 Category("Appearance")]
		int VisibleResourcesCount { get; set; }
		[ModelBrowsable(typeof(ModelListViewSchedulerVisibilityCalculator))]
		[
#if !SL
	DevExpressExpressAppSchedulerLocalizedDescription("IModelListViewSchedulerSelectedIntervalStart"),
#endif
 Category("Appearance")]
		DateTime SelectedIntervalStart { get; set; }
		[ModelBrowsable(typeof(ModelListViewSchedulerVisibilityCalculator))]
		[
#if !SL
	DevExpressExpressAppSchedulerLocalizedDescription("IModelListViewSchedulerSelectedIntervalEnd"),
#endif
 Category("Appearance")]
		DateTime SelectedIntervalEnd { get; set; }
		[Category("Appearance")]
		[ModelBrowsable(typeof(ModelListViewSchedulerVisibilityCalculator))]
		TimeSpan TimeScale { get; set; }
	}
}
