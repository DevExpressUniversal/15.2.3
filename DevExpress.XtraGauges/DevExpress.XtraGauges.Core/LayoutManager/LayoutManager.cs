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
using System.ComponentModel;
using System.Drawing;
using DevExpress.XtraGauges.Core.Base;
using DevExpress.Compatibility.System.Drawing;
namespace DevExpress.XtraGauges.Core.Layout {
	public enum PreferredLayoutType {
		Top,
		Bottom,
		Left,
		Right,
		Fill
	}
	public interface ILayoutManagerClient {
		Rectangle Bounds { get; set; }
		PreferredLayoutType PreferredLayoutType { get;}
		ILayoutManagerContainer LayoutContainer { get;}
	}
	public interface ILayoutManagerContainer {
		Rectangle Bounds { get; }
		IThickness LayoutPadding { get;set; }
		int LayoutInterval { get;set; }
		List<ILayoutManagerClient> Clients { get;}
		void DoLayout();
	}
	public class LayoutManager {
		ILayoutManagerContainer containerCore = null;
		IDictionary<PreferredLayoutType, IList<ILayoutManagerClient>> clients;
		IDictionary<PreferredLayoutType, Rectangle> rects;
		bool StretchClientsByContainerArea;
		public LayoutManager(ILayoutManagerContainer container)
			: this(container, false) {
		}
		public LayoutManager(ILayoutManagerContainer container, bool stretchClients) {
			this.containerCore = container;
			clients = new Dictionary<PreferredLayoutType, IList<ILayoutManagerClient>>();
			rects = new Dictionary<PreferredLayoutType, Rectangle>();
			StretchClientsByContainerArea = stretchClients;
		}
		public ILayoutManagerContainer Container {
			get { return containerCore; }
		}
		protected virtual void ClassifyContainerClients() {
			Array values = Enum.GetValues(typeof(PreferredLayoutType));
			foreach (PreferredLayoutType plt in values) {
				if (!clients.ContainsKey(plt)) continue;
				clients[plt].Clear();
			}
			foreach(ILayoutManagerClient client in Container.Clients) {
				if (!clients.ContainsKey(client.PreferredLayoutType)) 
					clients[client.PreferredLayoutType] = new List<ILayoutManagerClient>();
				clients[client.PreferredLayoutType].Add(client);
			}
		}
		protected virtual Rectangle CutRect(ref Rectangle target, int cutSize, PreferredLayoutType plt) {
			Rectangle result = target;
			if (plt == PreferredLayoutType.Fill) return result;
			switch (plt) {
				case PreferredLayoutType.Top:
					target.Y += cutSize;
					target.Height -= cutSize;
					result.Height = cutSize;
					break;
				case PreferredLayoutType.Bottom:
					result.Y = target.Bottom - cutSize;
					result.Height = cutSize;
					target.Height -= cutSize;
					break;
				case PreferredLayoutType.Left:
					target.X += cutSize;
					target.Width -= cutSize;
					result.Width = cutSize;
					break;
				case PreferredLayoutType.Right:
					result.X = target.Right - cutSize;
					result.Width = cutSize;
					target.Width -= cutSize;
					break;
			}
			return result;
		}
		protected virtual int GetClientSize() {
			int count = Container.Clients.Count;
			bool canStretch = StretchClientsByContainerArea && (count > 1) &&
				((clients.Count == 1) && !clients.ContainsKey(PreferredLayoutType.Fill));
			if(canStretch) {
				PreferredLayoutType type = Container.Clients[0].PreferredLayoutType;
				bool horz = (type == PreferredLayoutType.Left) || (type == PreferredLayoutType.Right);
				return ((horz ? Container.Bounds.Width - Container.LayoutPadding.Width : 
					Container.Bounds.Height - Container.LayoutPadding.Height) -
					Container.LayoutInterval * (count - 1)) / count;
			}
			return 100;
		}
		protected virtual void CalcRects() {
			Array values = Enum.GetValues(typeof(PreferredLayoutType));
			Rectangle restRect = CalcContent(Container);
			int space = GetClientSize() + Container.LayoutInterval;
			foreach (PreferredLayoutType plt in values) {
				if (!clients.ContainsKey(plt)) continue;
				else {
					int cutSize = clients[plt].Count * space;
					rects[plt] = CutRect(ref restRect, cutSize, plt);
				}
			}
		}
		static Rectangle CalcContent(ILayoutManagerContainer container) {
			Rectangle bounds = container.Bounds;
			IThickness thickness = container.LayoutPadding;
			return new Rectangle(
				bounds.Left + thickness.Left,
				bounds.Top + thickness.Top,
				bounds.Width - thickness.Width,
				bounds.Height - thickness.Height);
		}
		protected virtual void PlaceBorderClient(ILayoutManagerClient client, Rectangle targetRect, ref int offset, PreferredLayoutType plt) {
			Rectangle clientRect = targetRect;
			int clientSize = GetClientSize();
			switch (plt) {
				case PreferredLayoutType.Top:
					clientRect.Height = clientSize;
					clientRect.Y = targetRect.Y + offset;
					break;
				case PreferredLayoutType.Bottom:
					clientRect.Height = clientSize;
					clientRect.Y = targetRect.Y + Container.LayoutInterval + offset;
					break;
				case PreferredLayoutType.Left:
					clientRect.Width = clientSize;
					clientRect.X = targetRect.X + offset;
					break;
				case PreferredLayoutType.Right:
					clientRect.X = targetRect.X + Container.LayoutInterval + offset;
					clientRect.Width = clientSize;
					break;
			}
			offset += clientSize + Container.LayoutInterval;
			client.Bounds = clientRect;
		}
		protected virtual int[] CalcDivizors(ref int number) {
			List<int> result = new List<int>();
			for (int i = 1; i <= number; i++) {
				if (number % i == 0) result.Add(i);
			}
			if (result.Count == 2 && number > 2) {
				number++;
				return CalcDivizors(ref number);
			}
			return result.ToArray();
		}
		protected virtual int ProcessLayoutVariant(int wCount, int hCount, Rectangle rect, out int itemSize, int itemCount) {
			int rez1 = -1, rez2 = -1;
			int w = rect.Width - GetSpacing(Container.LayoutInterval, wCount);
			int h = rect.Height - GetSpacing(Container.LayoutInterval, hCount);
			int itemWidth = w / wCount;
			if(itemWidth * hCount <= h)
				rez1 = itemCount * itemWidth * itemWidth;
			int itemHeight = h / hCount;
			if(itemHeight * wCount <= w)
				rez2 = itemCount * itemHeight * itemHeight;
			itemSize = (rez1 > rez2) ? itemWidth : itemHeight;
			return Math.Max(rez1, rez2);
		}
		static int GetSpacing(int interval, int count) {
			return count > 1 ? (count - 1) * interval : 0;
		}
		protected virtual void PlaceFillClients(IList<ILayoutManagerClient> list, Rectangle rect, int columnCount, int rowCount, int itemSize) {
			int space = itemSize;
			int counter = 0; int offsetX = 0, offsetY = 0;
			for (int row = 0; row < rowCount; row++) {
				offsetX = 0;
				for (int column = 0; column < columnCount; column++) {
					if(counter < list.Count) {
						ILayoutManagerClient client = list[counter];
						client.Bounds = new Rectangle(rect.X + offsetX, rect.Y + offsetY, space, space);
					}
					counter++;
					offsetX += space + Container.LayoutInterval;
				}
				offsetY += space + Container.LayoutInterval;
			}
		}
		protected virtual void CalcFillClients(IList<ILayoutManagerClient> list, Rectangle rect) {
			int count = list.Count;
			int bestW = 1; int bestH = 1;
			int bestSquare = -1;
			int bestItemSize = 10;
			int[] divizors = CalcDivizors(ref count);
			int[] correctDivisor;
			if(list.Count % 2 != 0) {
				correctDivisor = new int[divizors.Length + 1];
				for(int i = 0; i < divizors.Length; i++) {
					correctDivisor[i] = divizors[i];
				}
				correctDivisor[divizors.Length] = list.Count;
			}
			else correctDivisor = divizors;
			for(int i = 0; i < correctDivisor.Length; i++) {
				for(int j = 0; j < correctDivisor.Length; j++) {
					if(correctDivisor[i] * correctDivisor[j] >= list.Count) {
						int tempItemSize = 0;
						int tempSquare = ProcessLayoutVariant(correctDivisor[i], correctDivisor[j], rect, out tempItemSize, list.Count);
						if (bestSquare <= tempSquare) {
							bestSquare = tempSquare;
							bestW = correctDivisor[i];
							bestH = correctDivisor[j];
							bestItemSize = tempItemSize;
						}
					}
				}
			}
			PlaceFillClients(list, rect, bestW, bestH, bestItemSize);
		}
		protected virtual void CalcClients() {
			Array values = Enum.GetValues(typeof(PreferredLayoutType));
			foreach(PreferredLayoutType plt in values) {
				if(!clients.ContainsKey(plt)) continue;
				IList<ILayoutManagerClient> list = clients[plt];
				int offset = 0;
				if(plt != PreferredLayoutType.Fill) {
					foreach(ILayoutManagerClient client in list) {
						PlaceBorderClient(client, rects[plt], ref offset, plt);
					}
				}
				else CalcFillClients(list, rects[plt]);
			}
		}
		public void Layout() {
			IList<ILayoutManagerClient> clients = Container.Clients;
			if(clients.Count == 0) return;
			if(clients.Count == 1) {
				clients[0].Bounds = CalcContent(Container);
				return;
			}
			ClassifyContainerClients();
			CalcRects();
			CalcClients();
		}
	}
}
