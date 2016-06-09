﻿#region Copyright (c) 2000-2015 Developer Express Inc.
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
using DevExpress.Data.Entity;
using DevExpress.Entity.ProjectModel;
namespace DevExpress.DataAccess.Native.EntityFramework {
	public static class EntityServiceHelper {
		public static ISolutionTypesProvider GetRuntimeSolutionProvider(Assembly assembly) {
			if(assembly == null)
				return new RuntimeSolutionTypesProvider(() => Enumerable.Empty<Type>());
			return new RuntimeSolutionTypesProvider(() => GetTypes(assembly, _ => true));			
		}
		public static IEnumerable<Type> GetTypes(Assembly assembly, Predicate<Type> predicate) {
			foreach(Type type in assembly.GetTypes())
				yield return type;
			foreach(AssemblyName name in assembly.GetReferencedAssemblies())
				if(TypesFilterHelper.ShouldIncludeTypesFromAssembly(name.FullName)) {
					Assembly asm = Assembly.Load(name);
					Type[] types;
					try {
						types = asm.GetTypes();
					}
					catch(ReflectionTypeLoadException exception) {
						types = exception.Types;
					}
					if(types != null)
						foreach(Type type in types)
							if(type != null && predicate(type))
								yield return type;
				}
		}
	}
}
