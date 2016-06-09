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
using System.Collections.Generic;
using DevExpress.Utils.IoC;
namespace DevExpress.XtraReports.Service.Native.ServiceContainer {
	public class ServiceIntegrityContainer : IntegrityContainer, IIntegrityContainer {
#if DEBUGTEST
		public IEnumerable<Type> TEST_RegisteredTypes {
			get { return base.RegisteredTypes; }
		}
#endif
		#region IIntegrityContainer
		void IIntegrityContainer.RegisterInstance<TServiceType>(TServiceType instance) {
			RegisterInstance(instance);
		}
		void IIntegrityContainer.RegisterType<TServiceType, TConcreteType>(ContainerRegistrationKind registrationKind) {
			var registration = RegisterType<TServiceType, TConcreteType>();
			if(registrationKind == ContainerRegistrationKind.Transient) {
				registration.AsTransient();
			}
		}
		object IIntegrityContainer.Resolve(Type serviceType) {
			return Resolve(serviceType);
		}
		#endregion
		protected sealed override bool TryResolveUnregistered(Type serviceType, out Registration registration) {
			if(serviceType == typeof(IIntegrityContainer)) {
				registration = new InstanceRegistration(this);
				return true;
			}
			return base.TryResolveUnregistered(serviceType, out registration)
				|| TryResolveUnregisteredCore(serviceType, out registration);
		}
		protected virtual bool TryResolveUnregisteredCore(Type serviceType, out Registration registration) {
			registration = null;
			if(serviceType.IsAbstract) {
				return false;
			}
			var ctorInfos = SelectMostGreedyConstructors(serviceType);
			if(ctorInfos.Length == 0) {
				return false;
			}
			var ctorInfo = ctorInfos[0];
			registration = new TypeRegistration(serviceType, ctorInfo);
			return true;
		}
	}
}
