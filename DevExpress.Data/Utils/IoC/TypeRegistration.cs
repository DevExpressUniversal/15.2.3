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
namespace DevExpress.Utils.IoC {
	public class TypeRegistration : Registration {
		private readonly Dictionary<string, object> arguments = new Dictionary<string, object>();
		private ParameterInfo[] parameterInfoArray;
		public Type ConcreteType { get; private set; }
		public ConstructorInfo ConstructorInfo { get; private set; }
		public object Instance { get; internal set; }
		public bool Transient { get; private set; }
		public TypeRegistration(Type concreteType, ConstructorInfo constructorInfo) {
			if(concreteType == null)
				throw new ArgumentNullException("concreteType");
			if(constructorInfo == null)
				throw new ArgumentNullException("constructorInfo");
			ConcreteType = concreteType;
			ConstructorInfo = constructorInfo;
		}
		public TypeRegistration WithCtorArgument(string name, object value) {
			if(parameterInfoArray == null) {
				parameterInfoArray = ConstructorInfo.GetParameters();
			}
			if(parameterInfoArray.FirstOrDefault(x => x.Name == name) == null)
				throw new RegistrationFailedException("Cannot find constructor parameter " + name);
			arguments[name] = value;
			return this;
		}
		public bool TryGetParameterValue(string name, out object value) {
			return arguments.TryGetValue(name, out value);
		}
		public void AsTransient() {
			Transient = true;
		}
	}
}
