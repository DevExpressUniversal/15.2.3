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
using System.Collections.Generic;
namespace DevExpress.ExpressApp.Model.Core {
	sealed class ModelValueFactory {
		private readonly Dictionary<Type, IModelValue> modelValuesSamples;
		private readonly Dictionary<Type, IModelValue> localizableModelValuesSamples;
		private readonly object locker;
		internal ModelValueFactory() {
			modelValuesSamples = new Dictionary<Type, IModelValue>();
			localizableModelValuesSamples = new Dictionary<Type, IModelValue>();
			locker = new object();
		}
		internal IModelValue CreateModelValue(Type type, bool localizable) {
			IModelValue sample = null;
			Dictionary<Type, IModelValue> cache = localizable ? localizableModelValuesSamples : modelValuesSamples;
			if(!cache.TryGetValue(type, out sample)) {
				lock(locker) {
					if(!cache.TryGetValue(type, out sample)) {
						Type modelValueType = localizable ? typeof(ModelLocalizableValue<>) : typeof(ModelValue<>);
						modelValueType = modelValueType.MakeGenericType(type);
						sample = (IModelValue)Activator.CreateInstance(modelValueType);
						cache.Add(type, sample);
					}
				}
			}
			return (IModelValue)sample.Clone();
		}
	}
}
