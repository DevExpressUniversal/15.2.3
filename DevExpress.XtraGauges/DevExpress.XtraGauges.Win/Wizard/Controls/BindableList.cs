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
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraGauges.Core.Primitive;
using System.Drawing;
using DevExpress.XtraGauges.Core.Base;
using DevExpress.LookAndFeel;
using System.Drawing.Drawing2D;
using DevExpress.XtraEditors.Drawing;
using DevExpress.Utils.Drawing;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Container;
using DevExpress.XtraEditors;
using DevExpress.Utils;
using DevExpress.XtraGauges.Win.Base;
namespace DevExpress.XtraGauges.Win.Wizard {
	[System.ComponentModel.ToolboxItem(false)]
	public class DesignerBindableItemsList<T> : ListBoxControl
		where T : class, IBindableComponent, INamed {
		T[] primitivesCore;
		public DesignerBindableItemsList() {
			this.SetStyle(
				ControlStyles.OptimizedDoubleBuffer |
				ControlStyles.SupportsTransparentBackColor,
				true);
			OnCreate();
		}
		protected override void Dispose(bool disposing) {
			if(disposing) {
				OnDispose();
			}
			base.Dispose(disposing);
		}
		protected virtual void OnCreate() {
			this.BorderStyle = BorderStyles.NoBorder;
		}
		protected virtual void OnDispose() {
			if(Primitives != null) {
				primitivesCore = null;
			}
		}
		public T[] Primitives {
			get { return primitivesCore; }
			set {
				if(Primitives == value) return;
				SetPrimitivesCore(value);
			}
		}
		public T SelectedPrimitive {
			get { return SelectedIndex != -1 ? Primitives[SelectedIndex] : null; }
		}
		protected void SetPrimitivesCore(T[] value) {
			this.primitivesCore = value;
			string[] names = new string[Primitives.Length];
			for(int i = 0; i < Primitives.Length; i++) {
				names[i] = (Primitives[i].Site != null) ? Primitives[i].Site.Name : Primitives[i].Name;
			}
			Items.Clear();
			Items.AddRange(names);
			SelectedIndex = 0;
		}
		protected override void OnSelectionChanged() {
			base.OnSelectionChanged();
			if(SelectedItemChanged != null) {
				SelectedItemChanged(SelectedPrimitive);
			}
		}
		public event SelectedItemChangedHandler SelectedItemChanged;
	}
	public delegate void SelectedItemChangedHandler(object item);
}
