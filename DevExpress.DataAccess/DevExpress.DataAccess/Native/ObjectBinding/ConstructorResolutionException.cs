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
using DevExpress.DataAccess.ObjectBinding;
namespace DevExpress.DataAccess.Native.ObjectBinding {
	public class ConstructorResolutionException : Exception {
		public ConstructorResolutionException(Type type, ParameterList ctorParameters, string message) : base(message) {
			Type = type;
			CtorParameters = ctorParameters;
		}
		public Type Type { get; private set; }
		public ParameterList CtorParameters { get; private set; }
	}
	public class NoDefaultConstructorException : ConstructorResolutionException {
		public NoDefaultConstructorException(Type type) : base(type, new ParameterList(), string.Format("Cannot find default constructor for type {0}", type)) { }
	}
	public class ConstructorNotFoundException : ConstructorResolutionException {
		public ConstructorNotFoundException(Type type, ParameterList ctorParameters)
			: base(type, ctorParameters, string.Format("Cannot find constructor with signature ({0}) for type {1}", string.Join(", ", ctorParameters), type)) { }
	}
	public class AmbigousConstructorException : ConstructorResolutionException {
		public AmbigousConstructorException(Type type, ParameterList ctorParameters)
			: base(type, ctorParameters, string.Format("Found several constructors whith signature ({0}) for type {1}", string.Join(", ", ctorParameters), type)) { }
	}
}
