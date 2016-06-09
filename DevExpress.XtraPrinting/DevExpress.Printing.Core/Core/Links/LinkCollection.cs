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
using System.IO;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Collections;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using System.Runtime.InteropServices;
using DevExpress.XtraPrinting.Preview;
using System.Drawing;
using System.Drawing.Printing;
using DevExpress.XtraEditors;
using DevExpress.XtraPrinting.Native;
using System.Reflection;
namespace DevExpress.XtraPrinting {
	[
	ListBindable(BindableSupport.No),
	]
	public class LinkCollection : CollectionBase {
		protected PrintingSystemBase ps;
#if !SL
	[DevExpressPrintingCoreLocalizedDescription("LinkCollectionItem")]
#endif
		public LinkBase this[int index] {
			get { return (LinkBase)InnerList[index]; }
			set { InnerList[index] = value; }
		}
		internal LinkCollection() {
		}
		internal LinkCollection(PrintingSystemBase ps) {
			this.ps = ps;
		}
		public void AddRange(object[] items) {
			foreach(LinkBase item in items)
				Add(item);
		}
		public void CopyFrom(ArrayList array) {
			Clear();
			AddRange(array.ToArray());
		}
		public int Add(LinkBase val) {
			return List.Add(val);
		}
		protected override void OnInsertComplete(int index, object val) {
			base.OnInsertComplete(index, val);
			((LinkBase)val).PrintingSystemBase = ps;
			((LinkBase)val).Disposed += new EventHandler(link_Disposed);
		}
		protected override void OnRemoveComplete(int index, object value) {
			base.OnRemoveComplete(index, value);
			((LinkBase)value).Disposed -= new EventHandler(link_Disposed);
		}
		protected override void OnClear() {
			base.OnClear();
			foreach(LinkBase val in this)
				val.Disposed -= new EventHandler(link_Disposed);
		}
		public bool Contains(LinkBase val) {
			return List.Contains(val);
		}
		public int IndexOf(LinkBase val) {
			return List.IndexOf(val);
		}
		public void Insert(int index, LinkBase val) {
			List.Insert(index, val);
		}
		public virtual void Remove(LinkBase val) {
			List.Remove(val);
		}
		void link_Disposed(object sender, EventArgs e) {
			Remove((LinkBase)sender);
		}
	}
}
