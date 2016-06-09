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
using System.Linq;
using System.Text;
using System.Windows;
using DevExpress.Internal;
namespace DevExpress.Diagram.Core {
	public interface IAdorner {
		Rect Bounds { get; set; }
		double Angle { get; set; }
		void Destroy();
		void MakeTopmost();
	}
	public interface IAdorner<T> : IAdorner where T : class {
		T Model { get; }
	}
	public interface IUpdatableAdorner {
		IAdorner Adorner { get; }
		void Update();
	}
	class CompositeUpdatableAdorner : IUpdatableAdorner {
		readonly IUpdatableAdorner[] adorners;
		readonly IAdorner compositeAdorner;
		public CompositeUpdatableAdorner(params IUpdatableAdorner[] adorners) {
			this.adorners = adorners;
			this.compositeAdorner = new CompositeAdorner(adorners.Select(x => x.Adorner).ToArray());
		}
		IAdorner IUpdatableAdorner.Adorner { get { return compositeAdorner; } }
		void IUpdatableAdorner.Update() {
			adorners.ForEach(x => x.Update());
		}
#if DEBUGTEST
		public IUpdatableAdorner[] AdornersForTests { get { return adorners; } }
#endif
	}
	public static class AdornerExtensions {
		class UpdatableAdorner<T> : IUpdatableAdorner where T : class {
			readonly IAdorner<T> adorner;
			readonly Action<T> update;
			public UpdatableAdorner(IAdorner<T> adorner, Action<T> update) {
				this.adorner = adorner;
				this.update = update;
			}
			IAdorner IUpdatableAdorner.Adorner { get { return adorner; } }
			void IUpdatableAdorner.Update() {
				update(adorner.Model);
			}
		}
		public static void SetBounds(this IAdorner adorner, Point p1, Point p2) {
			adorner.Bounds = MathHelper.RectFromPoints(p1, p2);
		}
		public static void SetBounds(this IAdorner adorner, IDiagramItem item) {
			var rotatedDiagramBounds = item.RotatedDiagramBounds();
			adorner.Bounds = rotatedDiagramBounds.Rect;
			adorner.Angle = rotatedDiagramBounds.Angle;
		}
		public static void Destroy(this IEnumerable<IAdorner> adorners) {
			adorners.ForEach(x => x.Destroy());
		}
		public static IUpdatableAdorner AsUpdatableAdorner<T>(this IAdorner<T> adorner, Action<T> update) where T : class {
			return new UpdatableAdorner<T>(adorner, update);
		}
		public static T MakeTopmostEx<T>(this T adorner) where T : IAdorner {
			adorner.MakeTopmost();
			return adorner;
		}
		public static Rect_Angle RotatedBounds(this IAdorner adorner) {
			return new Rect_Angle(adorner.Bounds, adorner.Angle);
		}
	}
	public sealed class CompositeAdorner : IAdorner {
		readonly IAdorner[] adorners;
		public CompositeAdorner(params IAdorner[] adorners) {
			this.adorners = adorners;
		}
		Rect IAdorner.Bounds {
			get { return adorners.First().Bounds; }
			set { adorners.ForEach(x => x.Bounds = value); }
		}
		double IAdorner.Angle {
			get { return adorners.First().Angle; }
			set { adorners.ForEach(x => x.Angle = value); }
		}
		void IAdorner.Destroy() {
			adorners.Destroy();
		}
		void IAdorner.MakeTopmost() {
			adorners.ForEach(x => x.MakeTopmost());
		}
	}
	public interface ISharedAdoners<TData> {
		void Update(params TData[] items);
		void Clear();
	}
	public static class SharedAdornersHelper {
		public static ISharedAdoners<TData> Create<TAdorner, TData>(Func<TAdorner> createAdorner, Action<TAdorner, TData> update) 
			where TAdorner : IAdorner {
			return new SharedAdornersHelperCore<TAdorner, TData>(createAdorner, update);
		}
		public static ISharedAdoners<IDiagramItem> Create<TAdorner>(Func<TAdorner> createAdorner, Action<TAdorner, IDiagramItem> update = null)
			where TAdorner : IAdorner {
			return new SharedAdornersHelperCore<TAdorner, IDiagramItem>(createAdorner, (adorner, item) => {
				adorner.SetBounds(item);
				update.Do(x => x(adorner, item));
			});
		}
		class SharedAdornersHelperCore<TAdorner, TData> : ISharedAdoners<TData> where TAdorner : IAdorner {
			readonly Func<TAdorner> createAdorner;
			readonly Action<TAdorner, TData> update;
			List<TAdorner> adorners = new List<TAdorner>();
			public SharedAdornersHelperCore(Func<TAdorner> createAdorner, Action<TAdorner, TData> update) {
				this.createAdorner = createAdorner;
				this.update = update;
			}
			public void Update(params TData[] items) {
				int difference = items.Length - adorners.Count;
				if(difference >= 0) {
					adorners.AddRange(Enumerable.Range(0, difference).Select(x => createAdorner()));
				} else {
					DestroyCore(-difference);
				}
				items.ForEach(adorners, (item, adorner) => {
					update(adorner, item);
				});
			}
			public void Clear() {
				DestroyCore(adorners.Count);
			}
			void DestroyCore(int count) {
				for(int i = 0; i < count; i++) {
					adorners.Last().Destroy();
					adorners.RemoveAt(adorners.Count - 1);
				}
			}
		}
	}
}
