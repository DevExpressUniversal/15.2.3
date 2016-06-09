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
namespace DevExpress.XtraRichEdit.Native {
	public abstract class ObjectFactoryBase<TBase, TValue> {
		Dictionary<Type, ConstructorInfo> constructors = new Dictionary<Type, ConstructorInfo>();
		public void Add<TKey, T>()
			where T : TValue
			where TKey : TBase {
			ConstructorInfo constructor = typeof(T).GetConstructor(GetConstructorParameters<TKey, T>());
			constructors.Add(typeof(TKey), constructor);
		}
		protected abstract Type[] GetConstructorParameters<TKey, T>()
			where T : TValue
			where TKey : TBase;
		public void Remove<TKey>() where TKey : TBase {
			constructors.Remove(typeof(TKey));
		}
		public bool Contains<TKey>() where TKey : TBase {
			return constructors.ContainsKey(typeof(TKey));
		}
		public void Clear() {
			constructors.Clear();
		}
		protected TValue Get(Type type, params object[] args) {
			ConstructorInfo constructor;
			if(!constructors.TryGetValue(type, out constructor))
				return default(TValue);
			return (TValue)constructor.Invoke(args);
		}
	}
}
