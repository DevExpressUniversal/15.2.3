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
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
namespace DevExpress.Utils.IoC {
	public class IntegrityContainer : IServiceProvider {
		readonly Dictionary<Type, Registration> registry = new Dictionary<Type, Registration>();
		readonly ConcurrentDictionary<Type, object> resolveLocks = new ConcurrentDictionary<Type, object>();
		protected IEnumerable<Type> RegisteredTypes {
			get { return registry.Keys; }
		}
		public TypeRegistration RegisterType<TConcreteType>() {
			return RegisterType<TConcreteType, TConcreteType>();
		}
		public TypeRegistration RegisterType<TServiceType, TConcreteType>() where TConcreteType : TServiceType {
			Type concreteType = typeof(TConcreteType);
			if(concreteType.IsAbstract())
				throw new RegistrationFailedException("Concrete type can not be abstract.");
			ConstructorInfo[] constructorInfoArray = SelectMostGreedyConstructors(concreteType);
			if(constructorInfoArray.Length == 0)
				throw new RegistrationFailedException("Concrete type does not have public instance constructor.");
			if(constructorInfoArray.Length > 1)
				throw new RegistrationFailedException("Can not resolve ambiguity and choose most greedy constructor."); 
			return RegisterCore(typeof(TServiceType), new TypeRegistration(concreteType, constructorInfoArray[0]));
		}
		public InstanceRegistration RegisterInstance<TServiceType>(TServiceType instance) {
			return RegisterCore(typeof(TServiceType), new InstanceRegistration(instance));
		}
		protected TRegistration RegisterCore<TRegistration>(Type serviceType, TRegistration registration) where TRegistration : Registration {
			registry[serviceType] = registration;
			return registration;
		}
		public TServiceType Resolve<TServiceType>() {
			return (TServiceType)Resolve(typeof(TServiceType));
		}
		public object Resolve(Type serviceType) {
			Registration registration;
			if(!registry.TryGetValue(serviceType, out registration) && !TryResolveUnregistered(serviceType, out registration))
				throw new ResolutionFailedException(string.Format("Can not resolve type '{0}'", serviceType));
			InstanceRegistration instanceRegistration = registration as InstanceRegistration;
			if(instanceRegistration != null) {
				return instanceRegistration.Instance;
			}
			TypeRegistration typeRegistration = (TypeRegistration)registration;
			if(typeRegistration.Instance != null)
				return typeRegistration.Instance;
			if(typeRegistration.Transient) {
				return CreateInstance(typeRegistration);
			}
			lock(resolveLocks.GetOrAdd(serviceType, _ => new object())) {
				if(typeRegistration.Instance != null) {
					return typeRegistration.Instance;
				}
				var instance = CreateInstance(typeRegistration);
				typeRegistration.Instance = instance;
				return instance;
			}
		}
		object CreateInstance(TypeRegistration typeRegistration) {
			ParameterInfo[] parameterInfoArray = typeRegistration.ConstructorInfo.GetParameters();
			object[] parameters = new object[parameterInfoArray.Length];
			for(int i = 0; i < parameterInfoArray.Length; i++) {
				var parameterInfo = parameterInfoArray[i];
				if(!typeRegistration.TryGetParameterValue(parameterInfo.Name, out parameters[i])) {
					try {
						parameters[i] = Resolve(parameterInfo.ParameterType);
					} catch(ResolutionFailedException exception) {
						throw new ResolutionFailedException(string.Format("Missing parameter '{0}' for the requested type '{1}'", parameterInfo.Name, typeRegistration.ConcreteType), exception);
					}
				}
			}
			return Activator.CreateInstance(typeRegistration.ConcreteType, parameters);
		}
		#region IServiceProvider
		object IServiceProvider.GetService(Type serviceType) {
			return Resolve(serviceType);
		}
		#endregion
		protected virtual bool TryResolveUnregistered(Type serviceType, out Registration registration) {
			if(serviceType == typeof(IServiceProvider)) {
				registration = new InstanceRegistration(this);
				return true;
			}
			registration = null;
			return false;
		}
		protected static ConstructorInfo[] SelectMostGreedyConstructors(Type type) {
			ConstructorInfo[] constructorInfoArray = type.GetConstructors(BindingFlags.Instance | BindingFlags.Public);
			if(constructorInfoArray.Length == 0)
				return constructorInfoArray;
			int maximumNumberOfParameters = constructorInfoArray.Max(arg => arg.GetParameters().Length);
			return constructorInfoArray.Where(arg => arg.GetParameters().Length == maximumNumberOfParameters).ToArray();
		}
	}
}
