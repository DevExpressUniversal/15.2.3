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
using System.Reflection;
using DevExpress.ExpressApp.Core;
using DevExpress.ExpressApp.Utils;
namespace DevExpress.ExpressApp {
	public class RequiredModuleEntry {
		public RequiredModuleEntry(string reason, ModuleBase module) {
			this.Reason = reason;
			this.Module = module;
		}
		public string Reason;
		public ModuleBase Module;
	}
	public class ModuleList : BindingListBase<ModuleBase> {
		private readonly XafApplication application;
		private readonly List<RequiredModuleEntry> requiredModules;
		private IList<ModuleBase> Clone() {
			return new List<ModuleBase>(this);
		}
		private ModuleBase Find(String moduleTypeName, IList<ModuleBase> moduleList) {
			foreach(ModuleBase module in moduleList) {
				if(module.GetType().FullName == moduleTypeName) {
					return module;
				}
			}
			return null;
		}
		private IList<ModuleBase> GetDependentModules(ModuleBase module) {
			List<ModuleBase> dependentModules = new List<ModuleBase>();
			foreach(ModuleBase currentModule in InnerList) {
				if(currentModule != module) {
					IList<ModuleBase> requiredModules = GetRequiredModules(currentModule);
					if(requiredModules.Contains(module)) {
						dependentModules.Add(currentModule);
					}
				}
			}
			return dependentModules;
		}
		private IList<ModuleBase> GetRequiredModules(ModuleBase module) {
			return ApplicationModulesManager.AddModuleIntoList(module, Clone());
		}
		private void SetupModule(ModuleBase module) {
			if(application != null) {
				module.Setup(application);
			}
		}
		private void CheckValueOnInsert(object value) {
			Guard.ArgumentNotNull(value, "value");
			if(FindModule(value.GetType()) != null) {
				throw new ArgumentException(String.Format("The {0} module has already been added.", value.GetType().FullName), "value");
			}
		}
		protected override void OnInsert(int index, object value) {
			CheckValueOnInsert(value);
			base.OnInsert(index, value);
			ModuleBase newModule = (ModuleBase)value;
			SetupModule(newModule);
			if(!IsInitializing) {
				foreach(ModuleBase requiredModule in GetRequiredModules(newModule)) {
					if(newModule != requiredModule && !InnerList.Contains(requiredModule)) {
						InnerList.Add(requiredModule);
						SetupModule(requiredModule);
					}
				}
			}
		}
		protected override void OnRemove(int index, object value) {
			base.OnRemove(index, value);
			foreach(RequiredModuleEntry entry in requiredModules) {
				if(entry.Module == value) {
					throw new CannotRemoveModule((ModuleBase)value, entry.Reason);
				}
			}
			IList<ModuleBase> dependentModules = GetDependentModules((ModuleBase)value);
			if(dependentModules.Count > 0) {
				throw new CannotRemoveModule((ModuleBase)value, dependentModules);
			}
		}
		protected override void OnSet(int index, object oldValue, object newValue) {
			throw new NotSupportedException();
		}
		protected override void OnInitializeComplete() {
			base.OnInitializeComplete();
			RefreshRequiredModules();
		}
		public ModuleList() : this(null) { }
		public ModuleList(XafApplication application) {
			this.application = application;
			requiredModules = new List<RequiredModuleEntry>();
		}
		public void AddRange(IEnumerable<ModuleBase> modules) {
			Guard.ArgumentNotNull(modules, "modules");
			BeginInit();
			foreach(ModuleBase module in modules) {
				Add(module);
			}
			EndInit();
		}
		public void RefreshRequiredModules() {
			List<ModuleBase> result = new List<ModuleBase>();
			foreach(ModuleBase module in Clone()) {
				if(!result.Contains(module)) {
					IList<ModuleBase> requiredModules = ApplicationModulesManager.AddModuleIntoList(module, this);
					foreach(ModuleBase requiredModule in requiredModules) {
						if(Find(requiredModule.GetType().FullName, result) == null) {
							result.Add(requiredModule);
						}
					}
					if(!result.Contains(module)) {
						result.Add(module);
					}
				}
			}
			InnerList.Clear();
			InnerList.AddRange(result);
		}
		public ModuleBase FindModule(Type moduleType) {
			for(int i = 0; i < Count; i++) {
				ModuleBase module = this[i];
				if(module.GetType() == moduleType) {
					return module;
				}
			}
			return null;
		}
		public ModuleType FindModule<ModuleType>() where ModuleType : ModuleBase {
			return (ModuleType)FindModule(typeof(ModuleType));
		} 
		public void AddRequiredModule<ModuleType>(string reason) where ModuleType : ModuleBase {
			ModuleType module = FindModule<ModuleType>();
			if(module == null) {
				module = (ModuleType)ModuleFactory.WithResourcesDiffs.CreateModule(typeof(ModuleType));
				Add(module);
			}
			bool isFound = false;
			foreach(RequiredModuleEntry entry in requiredModules) {
				if(entry.Module.GetType() == typeof(ModuleType)) {
					isFound = true;
					break;
				}
			}
			if(!isFound) {
				requiredModules.Add(new RequiredModuleEntry(reason, module));
			}
		}
	}
}
