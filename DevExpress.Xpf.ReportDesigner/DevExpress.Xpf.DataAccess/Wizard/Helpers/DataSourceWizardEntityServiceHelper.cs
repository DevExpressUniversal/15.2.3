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
using System.Reflection;
using System.Text;
using DevExpress.Entity.Model;
namespace DevExpress.Xpf.DataAccess.DataSourceWizard {
	public static class DataSourceWizardEntityServiceHelper {
		static readonly string[] ExcludedAssemblyPrefixes = {
			"mscorlib,",
			"System.",
			"System,",
			"Microsoft.",
			"PresentationCore",
			"PresentationFramework",
		};
		static readonly string[] MustIncludeAssemblyPrefixes = {
			Constants.EntityFrameworkSqliteAssemblyName,
			Constants.SqliteAssemblyName
		};
		static bool ShouldIncludeTypesFromAssembly(string assemblyName) {
			return ExcludedAssemblyPrefixes.All(t => !assemblyName.StartsWith(t)) || MustIncludeAssemblyPrefixes.Any(t => assemblyName.StartsWith(t));
		}
		public static IEnumerable<Type> GetTypes(Assembly assembly, byte[] key) {
			foreach(Type type in assembly.GetTypes())
				yield return type;
			foreach(AssemblyName name in assembly.GetReferencedAssemblies()) {
				if(key.SequenceEqual<byte>(name.GetPublicKeyToken()))
					continue;
				if(ShouldIncludeTypesFromAssembly(name.FullName)) {
					Assembly asm = Assembly.Load(name);
					Type[] types;
					try {
						types = asm.GetTypes();
					} catch(ReflectionTypeLoadException exception) {
						types = exception.Types;
					}
					if(types != null) {
						foreach(Type type in types)
							if(type != null)
								yield return type;
					}
				}
			}
		}
	}
}
