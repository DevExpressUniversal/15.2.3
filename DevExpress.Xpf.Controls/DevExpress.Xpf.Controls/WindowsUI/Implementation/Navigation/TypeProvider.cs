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
using System.Reflection;
using System.Linq;
namespace DevExpress.Xpf.WindowsUI.Navigation {
	public class TypeProvider {
		public static TypeProvider Current = new TypeProvider();
		public virtual IEnumerable<Assembly> Assemblies { get { return AppDomain.CurrentDomain.GetAssemblies(); } }
		protected static readonly Dictionary<string,Type> TypesCache = new Dictionary<string,Type>();
		public virtual Type GetTypeByName(string typeName) {
			if(Assemblies == null)
				return null;
			Type type = null;
			if (TypesCache.TryGetValue(typeName, out type)) {
				return type;
			}
			Assembly executing = Assembly.GetExecutingAssembly();
			type = executing.GetTypes().FirstOrDefault(t => t.Name == typeName);
			if (type != null) {
				TypesCache[typeName] = type;
				return type;
			}
			foreach(Assembly asm in Assemblies) {
				if (asm == executing) {
					continue;
				}
				try {
					type = asm.GetTypes().FirstOrDefault(t => t.Name == typeName);
					if (type != null) {
						TypesCache[typeName] = type;
						return type;
					}
				}
				catch (ReflectionTypeLoadException) {}
			}
			return null;
		}
	}
}
