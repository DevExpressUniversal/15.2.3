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
using System.ComponentModel.Design;
using System.Drawing;
using DevExpress.ExpressApp.Design;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model.NodeGenerators;
using DevExpress.ExpressApp.Updating;
using DevExpress.ExpressApp.Utils;
using DevExpress.ExpressApp.Utils.Reflection;
using DevExpress.ExpressApp.Xpo;
using DevExpress.Persistent.AuditTrail;
using DevExpress.Persistent.Base;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.DC.Xpo;
using DevExpress.Xpo.Metadata;
namespace DevExpress.ExpressApp.AuditTrail {
	[DXToolboxItem(true)]
	[DevExpress.Utils.ToolboxTabName(XafAssemblyInfo.DXTabXafModules)]
	[Description("Includes a Controller that audits and logs information on changes made to persistent objects when the application is running. This module is based on services from the Business Class Library's DevExpress.Persistent.AuditTrail assembly.")]
	[ToolboxBitmap(typeof(AuditTrailModule), "Resources.Toolbox_Module_AuditTrail.ico")]
	[Designer("DevExpress.ExpressApp.AuditTrail.Design.AuditTrailDesigner, DevExpress.ExpressApp.AuditTrail.Design" + XafAssemblyInfo.VersionSuffix + XafAssemblyInfo.AssemblyNamePostfix, typeof(IDesigner))]
	public sealed class AuditTrailModule : ModuleBase {
		private const string LocalizationGroup = "AuditTrail";
		private static object setupLockObject = new object();
		private bool isAuditDataItemPersistentTypeInitialized = false;
		private Type auditDataItemPersistentType = null;
		private EnumDescriptor enumDescriptor;
		protected override IEnumerable<Type> GetRegularTypes() {
			return null;
		}
		protected override IEnumerable<Type> GetDeclaredExportedTypes() {
			return AuditDataItemPersistentType == null ? Type.EmptyTypes : new Type[] { AuditDataItemPersistentType };
		}
		protected override IEnumerable<Type> GetDeclaredControllerTypes() {
			return new Type[] {
				typeof(AuditInformationListViewRefreshController),
				typeof(AuditInformationReadonlyViewController),
				typeof(AuditTrailListViewController),
				typeof(AuditTrailViewController)
			};
		}
		static AuditTrailModule() {
			ResourceLocalizationNodeGenerator.CustomizeCompoundNameConvertStyle += new EventHandler<CustomizeCompoundNameConvertStyleEventArgs>(ResourceLocalizationNodeGenerator_CustomizeCompoundNameConvertStyle);
		}
		public AuditTrailModule() {
		}
		public override void Setup(XafApplication application) {
			base.Setup(application);
			AuditTrailService.Instance.QueryCurrentUserName += new QueryCurrentUserNameEventHandler(Instance_QueryCurrentUserName);
			application.Disposed += new EventHandler(application_Disposed);
			application.SetupComplete += new EventHandler<EventArgs>(application_SetupComplete);
		}
		public override IEnumerable<ModuleUpdater> GetModuleUpdaters(IObjectSpace objectSpace, Version versionFromDB) {
			return ModuleUpdater.EmptyModuleUpdaters;
		}
		[TypeConverter(typeof(BusinessClassTypeConverter<IBaseAuditDataItemPersistent>))]
		public Type AuditDataItemPersistentType {
			get {
				if(!isAuditDataItemPersistentTypeInitialized) {
					isAuditDataItemPersistentTypeInitialized = true;
					auditDataItemPersistentType = FindDefaultAuditDataItemPersistentType();
				}
				return auditDataItemPersistentType;
			}
			set {
				isAuditDataItemPersistentTypeInitialized = true;
				auditDataItemPersistentType = value;
			}
		}
		private Type FindDefaultAuditDataItemPersistentType() {
			Type result = null;
			AssemblyHelper.TryGetType("DevExpress.Persistent.BaseImpl" + XafAssemblyInfo.VersionSuffix, "DevExpress.Persistent.BaseImpl.AuditDataItemPersistent", out result);
			return result;
		}
		protected override void RegisterEditorDescriptors(List<EditorDescriptor> editorDescriptors) { }
		private void Instance_QueryCurrentUserName(object sender, QueryCurrentUserNameEventArgs e) {
			if(!string.IsNullOrEmpty(SecuritySystem.CurrentUserName)) {
				e.CurrentUserName = SecuritySystem.CurrentUserName;
			}
		}
		private void AuditDataStore_ConvertObjectToString(object sender, ConvertObjectToStringEventArgs e) {
			if(e.Value is AuditOperationType) {
				e.StringRepresentation = enumDescriptor.GetCaption(e.Value);
			}
		}
		private void SetXPDictionaries() {
			List<XPDictionary> dictionaries = new List<XPDictionary>();
			foreach(IEntityStore entityStore in ((TypesInfo)XafTypesInfo.Instance).EntityStores) {
				if(entityStore is XpoTypeInfoSource) {
					dictionaries.Add(((XpoTypeInfoSource)entityStore).XPDictionary);
				}
			}
			AuditTrailService.Instance.SetXPDictionaries(dictionaries);
		}
		private void application_SetupComplete(object sender, EventArgs e) {
			lock(setupLockObject) {
				enumDescriptor = new EnumDescriptor(typeof(AuditOperationType));
				SetXPDictionaries();
				if(AuditTrailService.Instance.AuditDataStore == null) {
					if(auditDataItemPersistentType != null) {
						Type auditedObjectWeakReferenceType = ReflectionHelper.GetInterfaceImplementation(auditDataItemPersistentType, typeof(IAuditDataItemPersistent<>));
						Type auditDataStoreType = (typeof(AuditDataStore<,>).GetGenericTypeDefinition()).MakeGenericType(
							auditDataItemPersistentType,
							auditedObjectWeakReferenceType.GetGenericArguments()[0]);
						AuditTrailService.Instance.AuditDataStore = TypeHelper.CreateInstance(auditDataStoreType) as BaseAuditDataStore;
					}
					else {
						throw new NullReferenceException(@"Unable to create an AuditDataStore object because the AuditDataItemPersistentType property of the AuditTrailModule component is not set. To avoid this error, invoke the Application Designer, locate and select the AuditTrailModule component and then set its AuditDataItemPersistentType property as required.");
					}
				}
				if(AuditTrailService.Instance.AuditDataStore != null) {
					AuditTrailService.Instance.AuditDataStore.ConvertObjectToString += new EventHandler<ConvertObjectToStringEventArgs>(AuditDataStore_ConvertObjectToString);
				}
				AuditTrailService.Instance.AuditDataStore.NullValueString = CaptionHelper.GetLocalizedText(LocalizationGroup, "NullValueString", AuditTrailService.Instance.AuditDataStore.NullValueString);
				AuditTrailService.Instance.AuditDataStore.BlobDataString = CaptionHelper.GetLocalizedText(LocalizationGroup, "BlobDataString", AuditTrailService.Instance.AuditDataStore.BlobDataString);
			}
			((XafApplication)sender).ObjectSpaceCreated += new EventHandler<ObjectSpaceCreatedEventArgs>(application_ObjectSpaceCreated);
		}
		private void application_Disposed(object sender, EventArgs e) {
			((XafApplication)sender).Disposed -= new EventHandler(application_Disposed);
			if(AuditTrailService.Instance.AuditDataStore != null) {
				AuditTrailService.Instance.AuditDataStore.ConvertObjectToString -= new EventHandler<ConvertObjectToStringEventArgs>(AuditDataStore_ConvertObjectToString);
			}
			AuditTrailService.Instance.QueryCurrentUserName -= new QueryCurrentUserNameEventHandler(Instance_QueryCurrentUserName);
			XafApplication application = sender as XafApplication;
			if(application != null) {
				application.ObjectSpaceCreated -= new EventHandler<ObjectSpaceCreatedEventArgs>(application_ObjectSpaceCreated);
			}
		}
		private void application_ObjectSpaceCreated(object sender, ObjectSpaceCreatedEventArgs e) {
			if((e.ObjectSpace is XPObjectSpace) && !(e.ObjectSpace is INestedObjectSpace)) {
				AuditTrailService.Instance.BeginSessionAudit(((XPObjectSpace)e.ObjectSpace).Session, AuditTrailStrategy.OnObjectChanged);
				e.ObjectSpace.Committed += new EventHandler(ObjectSpace_Committed);
				e.ObjectSpace.Reloaded += new EventHandler(ObjectSpace_Reloaded);
			}
		}
		private void ObjectSpace_Reloaded(object sender, EventArgs e) {
			AuditTrailService.Instance.EndSessionAudit(((XPObjectSpace)sender).Session);
			AuditTrailService.Instance.BeginSessionAudit(((XPObjectSpace)sender).Session, AuditTrailStrategy.OnObjectChanged);
		}
		private void ObjectSpace_Committed(object sender, EventArgs e) {
			AuditTrailService.Instance.SaveAuditData(((XPObjectSpace)sender).Session);
			AuditTrailService.Instance.BeginSessionAudit(((XPObjectSpace)sender).Session, AuditTrailStrategy.OnObjectChanged);
		}
		private static void ResourceLocalizationNodeGenerator_CustomizeCompoundNameConvertStyle(object sender, CustomizeCompoundNameConvertStyleEventArgs e) {
			if(e.EnumType == typeof(AuditOperationType)) {
				e.CompoundNameConvertStyle = CompoundNameConvertStyle.None;
			}
		}
		protected override void Dispose(bool disposing) {
			if(disposing) {
				ResourceLocalizationNodeGenerator.CustomizeCompoundNameConvertStyle -= new EventHandler<CustomizeCompoundNameConvertStyleEventArgs>(ResourceLocalizationNodeGenerator_CustomizeCompoundNameConvertStyle);
			}
			base.Dispose(disposing);
		}
	}
}
