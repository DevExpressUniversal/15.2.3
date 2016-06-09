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
using System.Linq;
using System.Text;
namespace DevExpress.Utils.StoredObjects {
	class RepositoryProvider : IRepositoryProvider, IDisposable {
		Dictionary<Type, object> repositories = new Dictionary<Type, object>();
		TypeProvider typeProvider = new TypeProvider();
		public void RegisterRepository<T>(IObjectRepository<T> repository) {
			repositories[typeof(T)] = repository;
		}
		public virtual bool TryGetRepository<T>(out IObjectRepository<T> repository) {
			object value;
			if(repositories.TryGetValue(typeof(T), out value)) {
				repository = (IObjectRepository<T>)value;
				return true;
			}
			repository = null;
			return false;
		}
		public virtual object GetService(Type serviceType) {
			if(serviceType == typeof(ITypeProvider))
				return typeProvider;
			return null;
		}
		public void Dispose() {
			foreach(var pair in repositories)
				if(pair.Value is IDisposable)
					((IDisposable)pair.Value).Dispose();
			repositories.Clear();
		}
	}
}
