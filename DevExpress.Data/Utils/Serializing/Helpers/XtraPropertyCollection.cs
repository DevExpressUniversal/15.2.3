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
using System.Collections;
using System.Collections.Generic;
namespace DevExpress.Utils.Serializing.Helpers {
	public class XtraPropertyCollection : DXCollection<XtraPropertyInfo>, IXtraPropertyCollection {
		public XtraPropertyCollection() {
			UniquenessProviderType = DXCollectionUniquenessProviderType.None;
		}
		Dictionary<string, XtraPropertyInfo> hash = new Dictionary<string, XtraPropertyInfo>();
		protected override void OnRemoveComplete(int index, XtraPropertyInfo value) {
			base.OnRemoveComplete(index, value);
			this.hash.Remove(value.Name);
		}
		protected override void OnClearComplete() {
			base.OnClearComplete();
			this.hash.Clear();
		}
		protected override void OnInsertComplete(int index, XtraPropertyInfo value) {
			base.OnInsertComplete(index, value);
			this.hash[value.Name] = value; 
		}
		#region IXtraPropertyCollection Members
		public XtraPropertyInfo this[string name] {
			get {
				XtraPropertyInfo res;
				if(hash.TryGetValue(name, out res))
					return res;
				else
					return null;				
			}
		}
		public bool IsSinglePass { get { return false; } }
		void IXtraPropertyCollection.Add(XtraPropertyInfo prop) {
			this.Add(prop);
		}
		#endregion
	}
}
