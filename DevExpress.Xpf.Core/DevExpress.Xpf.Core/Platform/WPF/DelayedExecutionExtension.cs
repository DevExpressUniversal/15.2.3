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
using System.Collections.Specialized;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Threading;
#if DXWINDOW
namespace DevExpress.Internal.DXWindow {
#else
namespace DevExpress.Data {
#endif
	public static class DelayedExecutionExtension {
		static readonly Dictionary<object, DispatcherTimer> ElementDictionary = new Dictionary<object, DispatcherTimer>();
		static readonly Dictionary<object, List<DispatcherTimer>> ElementDictionaryList = new Dictionary<object, List<DispatcherTimer>>();
		public static void DelayedExecute(this object scroll, Action method) {
			scroll.DelayedExecute(0, method);
		}
		public static void DelayedExecute(this object scroll, int milliseconds, Action method) {
			lock (ElementDictionary)
			{
				if(ElementDictionary.ContainsKey(scroll)) {
					ElementDictionary[scroll].Stop();
					ElementDictionary[scroll] = CreateNewTimer(method, scroll, milliseconds);
				} else {
					ElementDictionary.Add(scroll, CreateNewTimer(method, scroll,  milliseconds));
				}
			}
		}
		public static void DelayedExecuteEnqueue(this object scroll, Action method) {
			scroll.DelayedExecuteEnqueue(0, method);
		}
		public static void DelayedExecuteEnqueue(this object scroll, int milliseconds, Action method) {
			lock (ElementDictionary)
			{
				lock (ElementDictionaryList)
				{
					if(ElementDictionary.ContainsKey(scroll)) {
						if(ElementDictionaryList.ContainsKey(scroll))
							AddNewTimer(scroll, milliseconds, method);
						else {
							lock (ElementDictionary) ElementDictionaryList.Add(scroll, new List<DispatcherTimer>() { ElementDictionary[scroll] });
							AddNewTimer(scroll, milliseconds, method);
						}
						return;
					}
				}
			}
			DelayedExecute(scroll, milliseconds, method);
		}
		private static void AddNewTimer(object scroll, int milliseconds, Action method) {
			DispatcherTimer newTimer = CreateNewTimer(method, scroll, 0);
			newTimer.Interval = TimeSpan.FromMilliseconds(milliseconds);
			lock (ElementDictionaryList) ElementDictionaryList[scroll].Add(newTimer);
		}
		static DispatcherTimer CreateNewTimer(Action method, object elem, int milliseconds) {
			DispatcherTimer timer = new DispatcherTimer();
			timer = new DispatcherTimer();
			if(milliseconds > 0) timer.Interval = TimeSpan.FromMilliseconds(milliseconds);
			timer.Tick += (d, e) =>
			{
				method();
				timer.Stop();
				lock (ElementDictionary) ElementDictionary.Remove(elem);
				lock (ElementDictionaryList)
				{
					if(ElementDictionaryList.ContainsKey(elem)) {
						ElementDictionaryList[elem].Remove(timer);
						if(ElementDictionaryList[elem].Count == 0)
							ElementDictionaryList.Remove(elem);
					}
				}
			};
			timer.Start();
			return timer;
		}
		public static void RemoveDelayedExecute(this object scroll) {
			DispatcherTimer timer;
			lock (ElementDictionary)
			{
				if(ElementDictionary.TryGetValue(scroll, out timer)) {
					timer.Stop();
					ElementDictionary.Remove(scroll);
				}
			}
			RemoveDelayedExecuteQueue(scroll);
		}
		private static void RemoveDelayedExecuteQueue(object scroll) {
			List<DispatcherTimer> timers;
			lock (ElementDictionaryList)
			{
				if(ElementDictionaryList.TryGetValue(scroll, out timers)) {
					foreach(DispatcherTimer timer in timers) {
						timer.Stop();
					}
					ElementDictionaryList.Remove(scroll);
				}
			}
		}
	}
}
