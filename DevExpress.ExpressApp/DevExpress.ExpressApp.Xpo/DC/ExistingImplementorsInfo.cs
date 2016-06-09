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
namespace DevExpress.ExpressApp.DC {
	sealed class ExistingImplementorsInfo : DCInterfaceWithUniqueValue<Type> {
		public ExistingImplementorsInfo() { }
		private void CheckAddParameters(Type interfaceType, Type implementorType) {
			if(implementorType == null) throw new ArgumentNullException("implementorType");
			if(!implementorType.IsClass) throw new ArgumentException(String.Format("The '{0}' type is not a class.", implementorType.FullName));
			if(!interfaceType.IsAssignableFrom(implementorType)) throw new ArgumentException(String.Format("The '{0}' class does not implement the '{1}' interface.", implementorType.FullName, interfaceType.FullName));
		}
		protected override void OnAdd(Type interfaceType, Type implementorType) {
			base.OnAdd(interfaceType, implementorType);
			CheckAddParameters(interfaceType, implementorType);
		}
		public void AddImplementor(Type implementorType, Type interfaceType) {
			AddInternal(interfaceType, implementorType);
		}
		public Type[] GetImplementors() {
			return GetValuesInternal();
		}
		public Boolean ContainsImplementor(Type implementorType) {
			return ContainsValueInternal(implementorType);
		}
		public Type GetImplementor(Type interfaceType) {
			return GetValueInternal(interfaceType);
		}
		public Type[] GetInterfaces() {
			return GetInterfacesInternal();
		}
		public Boolean ContainsInterface(Type interfaceType) {
			return ContainsInterfaceInternal(interfaceType);
		}
		public Type GetInterface(Type implementorType) {
			return GetInterfaceInternal(implementorType);
		}
	}
}
