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
using System.Windows;
namespace DevExpress.Xpf.Docking {
	public delegate void RequestUniqueNameEventHandler(
		object sender, RequestUniqueNameEventArgs ea
	);
	public class RequestUniqueNameEventArgs : RoutedEventArgs {
		public RequestUniqueNameEventArgs(ISerializableItem item, ICollection<string> existingNames) :
			base(DockLayoutManager.RequestUniqueNameEvent) {
			Item = item;
			ExistingNames = existingNames;
		}
		public ISerializableItem Item { get; private set; }
		public ICollection<string> ExistingNames { get; private set; }
	}
	public static class UniqueNameHelper {
		public static string GetUniqueName(string prefix, ICollection<string> names, int initialValue) {
			int count = initialValue;
			while(true) {
				string name = prefix + count++.ToString();
				if(!names.Contains(name)) return name;
			}
		}
		static readonly string mdiBarManagerName = "internalMDIBarManager";
		static readonly string mdiBarName = "MDIMenuBar";
		static int mdiBarManagerCount = 0;
		static int mdiBarCount;
		public static string GetMDIBarManagerName() {
			return mdiBarManagerName + ++mdiBarManagerCount;
		}
		public static string GetMDIBarName() {
			return mdiBarName + ++mdiBarCount;
		}
	}
}
