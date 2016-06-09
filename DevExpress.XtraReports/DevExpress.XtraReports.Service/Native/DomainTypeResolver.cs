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
namespace DevExpress.XtraReports.Service.Native {
	sealed class DomainTypeResolver : ITypeResolver, IDisposable {
		readonly BlockingCollection<Assembly> cachedAssemblies = new BlockingCollection<Assembly>();
		public Assembly[] GetCachedAssemblies() {
			return cachedAssemblies.ToArray();
		}
		IEnumerable<Assembly> DomainAssemblies {
			get {
				foreach(var assembly in cachedAssemblies) {
					yield return assembly;
				}
				foreach(var assembly in AppDomain.CurrentDomain.GetAssemblies()) {
					Func<string, bool> match = asm => assembly.FullName.StartsWith(asm, StringComparison.OrdinalIgnoreCase);
					if(!Helper.ExcludedAssemblyPrefixes.Any(match)) {
						yield return assembly;
					}
				}
			}
		}
		#region ITypeResolver Members
		public Type Resolve(string typeName) {
			foreach(var assembly in DomainAssemblies) {
				var result = assembly.GetType(typeName, false, true);
				if(result != null) {
					cachedAssemblies.TryAdd(assembly);
					return result;
				}
			}
			return null;
		}
		#endregion
#region IDisposable
		public void Dispose() {
			cachedAssemblies.Dispose();
		}
#endregion
	}
}
