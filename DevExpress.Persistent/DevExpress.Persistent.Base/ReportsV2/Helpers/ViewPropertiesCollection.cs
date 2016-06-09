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

using System.Collections;
using System.ComponentModel;
namespace DevExpress.Persistent.Base.ReportsV2 {
	[ListBindable(BindableSupport.No)]
	public sealed class ViewPropertiesCollection : CollectionBase {
		ViewDataSource owner = null;
		public ViewPropertiesCollection(ViewDataSource owner)
			: base() {
			this.owner = owner;
		}
		public void Add(ViewProperty sortProperty) {
			((IList)this).Add(sortProperty);
		}
		public void AddRange(ViewProperty[] sortProperties) {
			foreach(ViewProperty sp in sortProperties)
				Add(sp);
		}
		public void Add(ViewPropertiesCollection sortProperties) {
			foreach(ViewProperty sp in sortProperties)
				Add(sp);
		}
		public ViewProperty this[int index] { get { return (ViewProperty)List[index]; } }
		public ViewProperty this[string name] {
			get {
				foreach(ViewProperty prop in this) {
					if(Equals(name, prop.DisplayName))
						return prop;
				}
				return null;
			}
		}
		protected override void OnInsertComplete(int index, object value) {
			if(string.IsNullOrEmpty(((ViewProperty)value).DisplayName)) {
				((ViewProperty)value).DisplayName = NewPropertyName;
			}
			((ViewProperty)value).SetOwner(owner);
			if(owner != null)
				owner.RefreshProperties();
		}
		protected override void OnRemoveComplete(int index, object value) {
			((ViewProperty)value).SetOwner(null);
			if(owner != null)
				owner.RefreshProperties();
		}
		protected override void OnClear() {
			foreach(ViewProperty item in this) {
				item.SetOwner(null);
			}
			base.OnClear();
		}
		protected override void OnClearComplete() {
			base.OnClearComplete();
			if(owner != null)
				owner.RefreshProperties();
		}
		private string NewPropertyName {
			get {
				int propertyNamePostFix = 0;
				string result = "";
				do {
					propertyNamePostFix++;
					result = "viewProperty" + propertyNamePostFix.ToString();
				}
				while(!CheckName(result));
				return result;
			}
		}
		private bool CheckName(string newName) {
			foreach(ViewProperty prop in List) {
				if(prop.DisplayName == newName) {
					return false;
				};
			}
			return true;
		}
	}
}
