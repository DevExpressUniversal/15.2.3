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
using System.ComponentModel;
using System.Linq;
using System.Collections.Specialized;
using System.Reflection;
using System.Collections;
using DevExpress.Utils;
using DevExpress.Data.Browsing;
#if SL
using PropertyDescriptor = DevExpress.Data.Browsing.PropertyDescriptor;
#endif
namespace DevExpress.Mvvm.UI.Native.ViewGenerator.Model {
	public abstract class ModelPropertyCollectionBase : IModelPropertyCollection {
		protected readonly object obj;
		protected readonly EditingContextBase context;
		protected readonly IModelItem parent;
		public ModelPropertyCollectionBase(EditingContextBase context, object obj, IModelItem parent) {
			Guard.ArgumentNotNull(context, "context");
			Guard.ArgumentNotNull(parent, "parent");
			this.obj = obj;
			this.context = context;
			this.parent = parent;
		}
		IModelProperty IModelPropertyCollection.this[string propertyName] {
			get {
				var prop = FindCore(propertyName, null);
				if(prop == null) throw new ArgumentException();
				return prop;
			}
		}
		IModelProperty IModelPropertyCollection.this[DXPropertyIdentifier propertyIdentifier] {
			get {
				var prop = FindCore(propertyIdentifier.Name, propertyIdentifier.DeclaringType);
				if(prop == null) throw new ArgumentException();
				return prop;
			}
		}
		public override string ToString() {
			string res = string.Empty;
			foreach(PropertyDescriptor prop in TypeDescriptor.GetProperties(obj)) {
				res += prop.Name + ",";
			}
			return res;
		}
		IModelProperty IModelPropertyCollection.Find(string propertyName) {
			return FindCore(propertyName, null);
		}
		IModelProperty IModelPropertyCollection.Find(Type propertyType, string propertyName) {
			if(propertyType == null)
				throw new ArgumentNullException(); 
			return FindCore(propertyName, propertyType);
		}
		protected abstract IModelProperty FindCore(string propertyName, Type type);
		public abstract IEnumerator<IModelProperty> GetEnumerator();
		IEnumerator IEnumerable.GetEnumerator() {
			return this.GetEnumerator();
		}
	}
}
