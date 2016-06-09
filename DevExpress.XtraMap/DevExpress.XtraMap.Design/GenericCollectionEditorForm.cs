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

using DevExpress.Utils;
using DevExpress.XtraMap.Native;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
namespace DevExpress.XtraMap.Design {
	public abstract partial class GenericCollectionEditorForm<T> : CollectionEditorForm where T : class {
		DXCollection<T> editValue;
		protected override object[] EditValueArray { get { return EditValue != null ? Array.ConvertAll(editValue.ToArray(), el => (object)el) : base.EditValueArray; } }
		protected DXCollection<T> EditValue {
			get { return editValue; }
			set {
				editValue = value;
				OnEditValueChanged();
			}
		}
		public GenericCollectionEditorForm() {
		}
		protected virtual Type GetBaseType() {
			return typeof(T);
		}
		protected override Type[] GetItemTypes() {
			return MapUtils.GetTypeDescendants(MapUtils.XtraMapAssembly, GetBaseType(), DesignHelper.IgnoredTypes);
		}
		protected override void AddNewItemToCollection(object item) {
			T typedItem = item as T;
			if (typedItem != null)
				EditValue.Add(typedItem);
		}
		protected override void RemoveItemFromCollection(object item) {
			T legendItem = item as T;
			if (legendItem != null)
				EditValue.Remove(legendItem);
		}
		protected override void SwapItems(int index1, int index2) {
			ISupportSwapItems swappable = EditValue as ISupportSwapItems;
			if (swappable != null)
				swappable.Swap(index1, index2);
		}
	}
}
