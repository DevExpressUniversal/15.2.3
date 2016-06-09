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
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Reflection;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Utils;
using DevExpress.ExpressApp.Utils.Reflection;
using DevExpress.Persistent.Base;
namespace DevExpress.ExpressApp.Core {
	public class ControllersManager {
		private readonly LightDictionary<Type, Controller> registeredControllers;
		private readonly LightDictionary<Type, Controller> actualControllers;
		private static Boolean IsValidControllerType(Type type) {
			if(!typeof(Controller).IsAssignableFrom(type) || !TypeHelper.CanCreateInstance(type)) {
				return false;
			}
#if DebugTest
			if(type.Namespace.Contains(".Tests")) {
				return false;
			}
#endif
			return true;
		}
		private Controller CreateController(Type controllerType) {
			if(!IsValidControllerType(controllerType)) {
				return null;
			}
			try {
				return Controller.Create(controllerType);
			}
			catch(Exception e) {
				throw new InvalidOperationException(String.Format("Exception occurs while creating {0}\r\n{1}", controllerType.FullName, e.Message), e);
			}
		}
		private Boolean ContainsInheritedControllers(Type controllerType) {
			foreach(Type actualControllerType in actualControllers.GetKeys()) {
				if(controllerType.IsAssignableFrom(actualControllerType)) {
					return true;
				}
			}
			return false;
		}
		private void RemoveBaseController(Type controllerType) {
			IEnumerable<Type> actualControllerTypes = actualControllers.GetKeys();
			foreach(Type typeToCheck in actualControllerTypes) {
				if(typeToCheck.IsAssignableFrom(controllerType)) {
					actualControllers.Remove(typeToCheck);
					break;
				}
			}
		}
		protected internal ReadOnlyCollection<Controller> Controllers {
			get { return registeredControllers.GetValues().AsReadOnly(); }
		}
		protected ReadOnlyCollection<Controller> ActualControllers { 
			get { return actualControllers.GetValues().AsReadOnly(); }
		}
		public ControllersManager() {
			registeredControllers = new LightDictionary<Type, Controller>();
			actualControllers = new LightDictionary<Type, Controller>();
		}
		public static Type[] CollectControllerTypesFromAssembly(Assembly assembly) {
			Guard.ArgumentNotNull(assembly, "assembly");
			return ((TypesInfo)XafTypesInfo.Instance).GetAssemblyTypes(assembly, type => IsValidControllerType(type) && !TypeHelper.IsObsolete(type));
		}
		public void RegisterController(params Controller[] controllers) {
			foreach(Controller controller in controllers) {
				Type controllerType = controller.GetType();
				if(!registeredControllers.ContainsKey(controllerType)) {
					if(!ContainsInheritedControllers(controllerType)) {
						RemoveBaseController(controllerType);
						actualControllers.Add(controllerType, controller);
					}
					registeredControllers.Add(controllerType, controller);
				}
			}
		}
		public void RegisterControllerTypes(params Type[] controllerTypes) {
			foreach(Type controllerType in controllerTypes) {
				Controller controller = CreateController(controllerType);
				if(controller != null) {
					RegisterController(controller);
				}
			}
		}
		public List<Controller> CreateControllers(Type baseType, IModelApplication modelApplication) {
			List<Controller> result = new List<Controller>();
			foreach(Controller controller in actualControllers.GetValues()) {
				if(baseType.IsAssignableFrom(controller.GetType())) {
					result.Add(controller.Clone(modelApplication));
				}
			}
			return result;
		}
		public ControllerType CreateController<ControllerType>(IModelApplication modelApplication) where ControllerType : Controller, new() {
			ControllerType result;
			Controller sourceController = registeredControllers[typeof(ControllerType)];
			if(sourceController != null) {
				result = sourceController.Clone(modelApplication) as ControllerType;
			}
			else {
				result = new ControllerType();
			}
			return result;
		}
	}
}
