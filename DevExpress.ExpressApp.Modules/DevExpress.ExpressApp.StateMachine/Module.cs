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
using System.Reflection;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Design;
using DevExpress.ExpressApp.StateMachine.Dc;
using DevExpress.ExpressApp.StateMachine.Xpo;
using DevExpress.ExpressApp.Updating;
using DevExpress.Persistent.Base;
using DevExpress.Utils;
using DevExpress.ExpressApp.StateMachine.Resources;
namespace DevExpress.ExpressApp.StateMachine {
	[DXToolboxItem(true)]
	[ToolboxTabName(XafAssemblyInfo.DXTabXafModules)]
	[ToolboxBitmap(typeof(StateMachineModule), "Resources.Toolbox_Module_StateMachine.ico")]
	[Description("Provides State Machine functionality in XAF applications.")]
	public sealed class StateMachineModule : ModuleBase {
		private static Type stateMachineStorageType = typeof(XpoStateMachine);
		protected override ModuleTypeList GetRequiredModuleTypesCore() {
			ModuleTypeList result = base.GetRequiredModuleTypesCore();
			result.Add(typeof(DevExpress.ExpressApp.ConditionalAppearance.ConditionalAppearanceModule));
			result.Add(typeof(DevExpress.ExpressApp.Validation.ValidationModule));
			return result;
		}
		protected override IEnumerable<Type> GetRegularTypes() {
			return new Type[]{
			typeof(DCTransitionDomainLogic),
			typeof(DCStateMachineDomainLogic),
			typeof(DCStateAppearanceDomainLogic),
			typeof(DCStateDomainLogic)
			};
		}
		protected override IEnumerable<Type> GetDeclaredExportedTypes() {
			if(stateMachineStorageType != null) {
				return new Type[] { stateMachineStorageType };
			}
			else {
				return Type.EmptyTypes;
			}
		}
		protected override IEnumerable<Type> GetDeclaredControllerTypes() {
			return new Type[] {
				typeof(DevExpress.ExpressApp.StateMachine.DisableStatePropertyController),
				typeof(DevExpress.ExpressApp.StateMachine.StateMachineAppearanceController),
				typeof(DevExpress.ExpressApp.StateMachine.StateMachineController),
				typeof(DevExpress.ExpressApp.StateMachine.StateMachineRefreshStatePropertyNameController),
				typeof(DevExpress.ExpressApp.StateMachine.StateMachineCacheController),
				typeof(DevExpress.ExpressApp.StateMachine.StateMasterObjectInitializingController),
			};
		}
		public override IEnumerable<ModuleUpdater> GetModuleUpdaters(IObjectSpace objectSpace, Version versionFromDB) {
			return ModuleUpdater.EmptyModuleUpdaters;
		}
		public override ICollection<Type> GetXafResourceLocalizerTypes() {
			ICollection<Type> result = new List<Type>();
			result.Add(typeof(StateMachineLocalizer));
			return result;
		}
		public static void RegisterDomainComponentEntities(ITypesInfo typesInfo, string stateMachineEntityName, string stateEntityName, string transitionEntityName, string stateAppearanceEntityName) {
			typesInfo.RegisterEntity(stateMachineEntityName, typeof(IDCStateMachine));
			typesInfo.RegisterEntity(stateEntityName, typeof(IDCState));
			typesInfo.RegisterEntity(transitionEntityName, typeof(IDCTransition));
			typesInfo.RegisterEntity(stateAppearanceEntityName, typeof(IDCStateAppearance));
		}
#if !SL
	[DevExpressExpressAppStateMachineLocalizedDescription("StateMachineModuleStateMachineStorageType")]
#endif
		[TypeConverter(typeof(BusinessClassTypeConverter<IStateMachine>))]
		public Type StateMachineStorageType {
			get { return stateMachineStorageType; }
			set { stateMachineStorageType = value; }
		}
	}
	public class StateMachineTypeConverter : LocalizedClassInfoTypeConverter {
		public override List<Type> GetSourceCollection(ITypeDescriptorContext context) {
			List<Type> source = base.GetSourceCollection(context);
			List<Type> result = new List<Type>();
			foreach(Type type in source) {
				if(type != null) {
					foreach(PropertyInfo propertyInfo in type.GetProperties()) {
						DevExpress.ExpressApp.DC.ITypeInfo typeInfo = XafTypesInfo.Instance.FindTypeInfo(propertyInfo.PropertyType);
						if(propertyInfo.PropertyType.IsEnum || (typeInfo != null && typeInfo.IsPersistent)) {
							result.Add(type);
							break;
						}
					}
				}
			}
			return result;
		}
		public override void AddCustomItems(List<Type> list) { }
	}
}
