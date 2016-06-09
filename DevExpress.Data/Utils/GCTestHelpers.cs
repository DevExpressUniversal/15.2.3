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
using System.Linq;
using System.Collections.Generic;
namespace DevExpress {
	public static class GCTestHelper {
		static WeakReference Obtain(Func<object> obtainer) {
			return new WeakReference(obtainer());
		}
		static WeakReference[] Obtain(Func<object[]> obtainer) {
			return obtainer().Select(o => new WeakReference(o)).ToArray();
		}
		public static void EnsureCollected(Func<object> obtainer) {
			EnsureCollected(Obtain(obtainer));
		}
		public static void EnsureCollected(Func<object[]> obtainer) {
			EnsureCollected(Obtain(obtainer));
		}
		public static void EnsureCollected(params WeakReference[] references) {
			EnsureCollected(references.AsEnumerable());
		}
		public static void EnsureCollected(IEnumerable<WeakReference> references) {
			AssertCollectedCore(references, -1);
		}
#if SILVERLIGHT || NETFX_CORE
		static void AssertCollectedCore(IEnumerable<WeakReference> references, int alreadyCollectedGen) {
			SlowButSureAssertCollected(references.ToArray());
		}
#else
		static void AssertCollectedCore(IEnumerable<WeakReference> references, int alreadyCollectedGen) {
			int maxGeneration;
			List<WeakReference> nextIterationHolder = CollectExistingData(references, out maxGeneration);
			if(nextIterationHolder.Count == 0)
				return;
			if(maxGeneration <= alreadyCollectedGen) {
				SlowButSureAssertCollected(nextIterationHolder);
			} else {
				GC.Collect(maxGeneration, GCCollectionMode.Forced);
				AssertCollectedCore(nextIterationHolder, maxGeneration);
			}
		}
		static List<WeakReference> CollectExistingData(IEnumerable<WeakReference> references, out int maxGeneration) {
			maxGeneration = -1;
			var nextIterationHolder = new List<WeakReference>();
			foreach(var wr in references) {
				object t = wr.Target;
				if(t == null)
					continue;
				nextIterationHolder.Add(wr);
				int gen = GC.GetGeneration(t);
				if(gen > maxGeneration)
					maxGeneration = gen;
			}
			return nextIterationHolder;
		}
#endif
		static void SlowButSureAssertCollected(IList<WeakReference> nextIterationHolder) {
			GC.GetTotalMemory(true);
			if(nextIterationHolder.All(wr => !wr.IsAlive))
				return;
			GC.Collect();
			if(nextIterationHolder.All(wr => !wr.IsAlive))
				return;
			GC.GetTotalMemory(true);
			var notCollected = nextIterationHolder.Select(wr => wr.Target).Where(t => t != null).ToArray();
			if(notCollected.Length == 0)
				return;
			var objectsReport = string.Join("\n", notCollected.GroupBy(o => o.GetType()).OrderBy(gr => gr.Key.FullName)
				.Select(gr => string.Format("\t{0} object(s) of type {1}:\n{2}", gr.Count(), gr.Key.FullName
					, string.Join("\n", gr.Select(o => o.ToString()).OrderBy(s => s).Select(s => string.Format("\t\t{0}", s)))
					)));
			throw new Exception(string.Format("{0} garbage object(s) not collected:\n{1}", notCollected.Length, objectsReport));
		}
		public static void CollectOptional(params WeakReference[] references) {
			CollectOptional(references.AsEnumerable());
		}
		public static bool? HardOptional;
		static Random rnd = new Random();
		static bool IsHardOptional() {
			if(HardOptional.HasValue)
				return HardOptional.Value;
			lock(rnd) {
				return rnd.Next(100) < 5;
			}
		}
		public static void CollectOptional(IEnumerable<WeakReference> references) {
#if SILVERLIGHT || NETFX_CORE
			GC.Collect();
			GC.GetTotalMemory(true);
#else
			if(IsHardOptional()) {
				GC.Collect();
				GC.GetTotalMemory(true);
			} else {
				int? maxGeneration = references.Select(wr => wr.Target).Where(t => t != null).Select(t => GC.GetGeneration(t)).Max(gen => (int?)gen);
				if(maxGeneration.HasValue) {
					GC.Collect(maxGeneration.Value, GCCollectionMode.Forced);
				}
			}
#endif
		}
	}
}
