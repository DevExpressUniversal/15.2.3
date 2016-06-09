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

namespace DevExpress.Utils.Filtering.Internal {
	using System;
	abstract class StatelessObject : IServiceProvider {
		Func<IServiceProvider> getServiceProvider;
		protected StatelessObject(Func<IServiceProvider> getServiceProvider) {
			this.getServiceProvider = getServiceProvider;
		}
		protected TValue GetMetadata<TValue>(Func<IMetadataProvider, Func<string, TValue>> accessor) {
			return GetValue(accessor);
		}
		protected TValue GetBehavior<TValue>(Func<IBehaviorProvider, Func<string, TValue>> accessor) {
			return GetValue(accessor);
		}
		protected TValue GetValue<TService, TValue>(Func<TService, Func<string, TValue>> accessor) where TService : class {
			return GetService<TService>().@Get(x => accessor(x)(GetId()));
		}
		protected TService GetService<TService>() where TService : class {
			return getServiceProvider().@Get(x => x.GetService(typeof(TService)) as TService);
		}
		protected abstract string GetId();
		public override string ToString() {
			return GetId();
		}
		public sealed override int GetHashCode() {
			return GetId().GetHashCode();
		}
		public sealed override bool Equals(object obj) {
			if(ReferenceEquals(obj, this)) return true;
			StatelessObject stateless = obj as StatelessObject;
			return !ReferenceEquals(stateless, null) && stateless.GetId() == GetId();
		}
		#region IServiceProvider
		object IServiceProvider.GetService(Type serviceType) {
			return getServiceProvider().@Get(x => x.GetService(serviceType));
		}
		#endregion
	}
}
