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

using DevExpress.Xpf.Core.Native;
using System;
using System.Collections.Generic;
using System.Windows;
namespace DevExpress.Xpf.Core {
	public static class MemoryLeaksHelper {
		public static void CollectReferences(FrameworkElement rootElement, IList<WeakReference> referencesList) {
			VisualTreeEnumerator en = new VisualTreeEnumerator(rootElement);
			while(en.MoveNext()) {
				referencesList.Add(new WeakReference(en.Current));
			}
		}
		public static List<WeakReference> CollectReferences(FrameworkElement rootElement) {
			var rv = new List<WeakReference>();
			CollectReferences(rootElement, rv);
			return rv;
		}
		public static void EnsureCollected(IEnumerable<WeakReference> references) {
			GCTestHelper.EnsureCollected(references);
		}
		public static void EnsureCollected(params WeakReference[] references) {
			GCTestHelper.EnsureCollected(references);
		}
		public static void CollectOptional(params WeakReference[] references) {
			GCTestHelper.CollectOptional(references);
		}
		public static void GarbageCollect() {
#if !SILVERLIGHT
			DispatcherHelper.DoEvents();
#endif
			GC.Collect();
			GC.GetTotalMemory(true);
		}
	}
}
