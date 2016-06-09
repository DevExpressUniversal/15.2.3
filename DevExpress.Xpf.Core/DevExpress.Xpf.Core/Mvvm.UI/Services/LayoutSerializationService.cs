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

using DevExpress.Mvvm;
using DevExpress.Mvvm.UI;
using DevExpress.Xpf.Core.Serialization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
namespace DevExpress.Mvvm.UI {
	public class LayoutSerializationService : ServiceBase, ILayoutSerializationService {
		protected override void OnAttached() {
			base.OnAttached();
			EventHandler handler = null;
			handler = (s, e) => {
				InitialState = Serialize();
				AssociatedObject.Initialized -= handler;
			};
			AssociatedObject.Initialized += handler;
		}
		protected virtual FrameworkElement GetSerializationTarget() {
			return AssociatedObject;
		}
		public void Deserialize(string state) {
			state = state ?? InitialState;
			if(string.IsNullOrWhiteSpace(state))
				return;
			using(var ms = new MemoryStream(Encoding.UTF8.GetBytes(state))) {
				DXSerializer.Deserialize(GetSerializationTarget(), ms, string.Empty,
					new DXOptionsLayout { AcceptNestedObjects = AcceptNestedObjects.IgnoreChildrenOfDisabledObjects });
			}
		}
		public string Serialize() {
			using(var ms = new MemoryStream()) {
				DXSerializer.Serialize(GetSerializationTarget(), ms, string.Empty,
					new DXOptionsLayout { AcceptNestedObjects = AcceptNestedObjects.IgnoreChildrenOfDisabledObjects });
				ms.Seek(0, SeekOrigin.Begin);
				using(var reader = new StreamReader(ms)) {
					return reader.ReadToEnd();
				}
			}
		}
		public string InitialState { get; private set; }
	}
}
