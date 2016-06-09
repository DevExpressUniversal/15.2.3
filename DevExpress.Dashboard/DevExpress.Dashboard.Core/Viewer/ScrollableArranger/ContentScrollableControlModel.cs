#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       Dashboard                                                   }
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
using System.Drawing;
using DevExpress.DashboardCommon.Native;
using DevExpress.DashboardCommon.ViewModel;
namespace DevExpress.DashboardCommon.Viewer {
	public enum ScrollMode { None, Vertical, Horizontal, Both }
	public enum ScrollType { ByPixel, ByItem }
	public class RequestClientSizeEventArgs : EventArgs {
		public Size ClientSize { get; set; }
	}
	public class ContentScrollableControlModel {
		readonly ContentArranger arranger;
		readonly IContentProvider contentProvider;
		Size clientSize;
		int lockUpdateCount = 0;
		Size virtualSize = new Size();
		ScrollType scrollStep = ScrollType.ByItem;
		ScrollMode scrollMode = ScrollMode.None;
		Point offset = Point.Empty;
		PointF offsetPercent = PointF.Empty;
		IScrollBar hScrollBar;
		IScrollBar vScrollBar;
		internal Size ClientSize { get { return clientSize; } }
		public event EventHandler<RequestClientSizeEventArgs> RequestClientSize;
		public event EventHandler<EventArgs> ContentChanged;
		public ScrollMode ScrollMode { get { return scrollMode; } }
		public ContentArranger Arranger { get { return arranger; } }
		public ContentArrangementMode ContentArrangementMode {
			get { return arranger.ContentArrangementMode; }
			set {
				if(arranger.ContentArrangementMode != value) {
					arranger.ContentArrangementMode = value;
					Arrange();
				}
			}
		}
		public int ContentLineCount {
			get { return arranger.ContentLineCount; }
			set {
				if(arranger.ContentLineCount != value) {
					arranger.ContentLineCount = value;
					Arrange();
				}
			}
		}
		public ContentArrangementOptions ContentArrangementOptions {
			get { return arranger.ContentArrangementOptions; }
			set {
				if(arranger.ContentArrangementOptions != value) {
					arranger.ContentArrangementOptions = value;
					Arrange();
				}
			}
		}
		public int OffsetX {
			get { return offset.X; }
			set {
				if(offset.X != value) {
					offset.X = value;
					offsetPercent.X = (float)offset.X / hScrollBar.Maximum;
					ApplyOffset();
				}
			}
		}
		public int OffsetY {
			get { return offset.Y; }
			set {
				if(offset.Y != value) {
					offset.Y = value;
					offsetPercent.Y = (float)offset.Y / vScrollBar.Maximum;
					ApplyOffset();
				}
			}
		}
		public double HPositionRatio { get { return (double)hScrollBar.Value / (double)virtualSize.Width; } }
		public double VPositionRatio { get { return (double)vScrollBar.Value / (double)virtualSize.Height; } }
		public Size VirtualSize { get { return virtualSize; } }
		public Size ItemProportions { get { return arranger.ItemProportions; } }
		public int HScrollSize { get { return hScrollBar.Height; } }
		public int VScrollSize { get { return vScrollBar.Width; } }
		public ContentScrollableControlModel(IContentProvider contentProvider) {
			this.contentProvider = contentProvider;
			this.contentProvider.Changed += OnContentChanged;
			this.arranger = new ContentArranger(contentProvider.Items, contentProvider.ItemProportions, contentProvider.ItemMargin, contentProvider.ItemMinWidth, contentProvider.BorderProportion);
			ContentArrangementOptions = ContentArrangementOptions.Default;
		}
		public void InitializeContent(ContentDescriptionViewModel contentDescription) {
			if(contentDescription != null) {
				ContentArrangementMode = contentDescription.ArrangementMode;
				ContentLineCount = contentDescription.LineCount;
			}
		}
		public void InitializeScrollBars(IScrollBar hScrollBar, IScrollBar vScrollBar) {
			this.hScrollBar = hScrollBar;
			this.vScrollBar = vScrollBar;
			UpdateHScroll(false);
			UpdateVScroll(false);
		}
		public void Arrange() {
			if(contentProvider == null) return;
			BeginUpdate();
			try {
				PerformRequestClientSize();
				if(contentProvider.Items.Count != 0) {
					virtualSize = arranger.CalculateSize(clientSize);
					if(hScrollBar.TouchMode || vScrollBar.TouchMode)
						ApplyTouchScroll(clientSize);
					else
						ApplyScrollByVirtualSize(clientSize.Width, clientSize.Height);
					Size itemSize = arranger.ApplySizeToItems();
					ApplyOffset();
					switch(scrollStep) {
						case (ScrollType.ByItem):
							hScrollBar.SmallChange = itemSize.Width;
							hScrollBar.LargeChange = clientSize.Width;
							vScrollBar.SmallChange = itemSize.Height;
							vScrollBar.LargeChange = clientSize.Height;
							break;
						case (ScrollType.ByPixel):	
							hScrollBar.SmallChange = 1;
							hScrollBar.LargeChange = clientSize.Width;
							vScrollBar.SmallChange = 1;
							vScrollBar.LargeChange = clientSize.Height;
							break;
					}
				}
				contentProvider.SetSize(clientSize);
			}
			finally {
				EndUpdate();
			}
		}
		void PerformRequestClientSize() {
			if(RequestClientSize != null) {
				var e = new RequestClientSizeEventArgs();
				RequestClientSize(this, e);
				this.clientSize = e.ClientSize;
			}
		}
		void ApplyScrollByVirtualSize(int width, int height) {
			bool lessThanVirtualWidth = virtualSize.Width > width;
			bool lessThanVirtualHeight = virtualSize.Height > height;
			if(lessThanVirtualWidth && !lessThanVirtualHeight) {
				int heightWithScroll = height - hScrollBar.Height;
				virtualSize = arranger.CalculateSize(new Size(clientSize.Width, heightWithScroll));
				if(virtualSize.Height > heightWithScroll) 
					ChangeScrollMode(ScrollMode.Both);
				else {
					int newVirtualHeight = arranger.GetHeightByWidth(width);
					if(virtualSize.Width < width && newVirtualHeight <= height) {
						virtualSize = arranger.CalculateSize(new Size(width, newVirtualHeight));
						ChangeScrollMode(ScrollMode.None);
					} else
						ChangeScrollMode(ScrollMode.Horizontal);
				}
			} else if(!lessThanVirtualWidth && lessThanVirtualHeight) {
				int widthWithScroll = width - vScrollBar.Width;
				virtualSize = arranger.CalculateSize(new Size(widthWithScroll, height));
				if(virtualSize.Width > widthWithScroll)
					ChangeScrollMode(ScrollMode.Both);
				else {
					int newVirtualWidth = arranger.GetWidthByHeight(height);
					if(virtualSize.Height < clientSize.Height && newVirtualWidth <= width) { 
						virtualSize = arranger.CalculateSize(new Size(newVirtualWidth, height));
						ChangeScrollMode(ScrollMode.None);
					} else
						ChangeScrollMode(ScrollMode.Vertical);
				}
			}
			else if(lessThanVirtualWidth && lessThanVirtualHeight)
				ChangeScrollMode(ScrollMode.Both);
			else
				ChangeScrollMode(ScrollMode.None);
		}
		void ApplyTouchScroll(Size clientSize) {
			bool lessThanVirtualWidth = virtualSize.Width > clientSize.Width;
			bool lessThanVirtualHeight = virtualSize.Height > clientSize.Height;
			if(lessThanVirtualWidth && !lessThanVirtualHeight) {
				ChangeScrollMode(ScrollMode.Horizontal);
			} else if(!lessThanVirtualWidth && lessThanVirtualHeight) {
				ChangeScrollMode(ScrollMode.Vertical);
			} else if(lessThanVirtualWidth && lessThanVirtualHeight)
				ChangeScrollMode(ScrollMode.Both);
			else
				ChangeScrollMode(ScrollMode.None);
		}
		void ChangeScrollMode(ScrollMode newScrollMode) {
			scrollMode = newScrollMode;
			offset = Point.Empty;
			switch(scrollMode) {
				case (ScrollMode.Both):
					UpdateHScroll(true);
					UpdateVScroll(true);
					hScrollBar.Width = clientSize.Width;
					vScrollBar.Height = clientSize.Height;
					if(vScrollBar.TouchMode || hScrollBar.TouchMode) {
						vScrollBar.Height -= hScrollBar.Height;
						hScrollBar.Width -= vScrollBar.Width;
					}
					break;
				case (ScrollMode.Vertical):
					UpdateHScroll(false);
					UpdateVScroll(true);
					break;
				case (ScrollMode.Horizontal):
					UpdateHScroll(true);
					UpdateVScroll(false);
					break;
				default:
					offsetPercent = Point.Empty;
					UpdateHScroll(false);
					UpdateVScroll(false);
					break;
			}
		}
		void OnContentChanged(object sender, ContentProviderEventArgs e) {
			switch(e.Reason) {
				case ContentProviderChangeReason.ItemProperties:
					arranger.Initialize(contentProvider.ItemProportions, contentProvider.ItemMargin, contentProvider.ItemMinWidth, contentProvider.BorderProportion);
					break;
				case ContentProviderChangeReason.Data:
					arranger.Initialize(contentProvider.Items);
					break;
				case ContentProviderChangeReason.ItemPropertiesAndData:
					arranger.Initialize(contentProvider.Items, contentProvider.ItemProportions, contentProvider.ItemMargin, contentProvider.ItemMinWidth, contentProvider.BorderProportion);
					break;
			}
			Arrange();
			if(ContentChanged != null)
				ContentChanged(this, new EventArgs());
		}
		public void InitializeArranger() {
			arranger.Initialize(contentProvider.Items, contentProvider.ItemProportions, contentProvider.ItemMargin, contentProvider.ItemMinWidth, contentProvider.BorderProportion);
		}
		public IList<ISizable> GetItemsByValues(IList values) {
			IList<ISizable> items = new List<ISizable>();
			foreach(object item in contentProvider.Items) {
				IValuesProvider valueProvider = item as IValuesProvider;
				foreach(object valueObject in values) {
					IList value = valueObject as IList;
					if(value != null && DataUtils.AreListsEqual(value, valueProvider.SelectionValues)) {
						items.Add(item as ISizable);
					}
				}
			}
			return items;
		}
		public IValuesProvider GetHitItem(Point location) {
			foreach(ISizable sizableItem in contentProvider.Items) {
				Rectangle bounds = new Rectangle(sizableItem.Left, sizableItem.Top, sizableItem.Width, sizableItem.Height);
				if(bounds.Contains(location))
					return sizableItem as IValuesProvider;
			}
			return null;
		}
		void UpdateHScroll(bool enabled) {
			hScrollBar.Enabled = enabled;
			hScrollBar.Visible = enabled;
			if(enabled) {
				hScrollBar.Location = new Point(0, clientSize.Height - hScrollBar.Height);
				if(!hScrollBar.TouchMode)
					clientSize.Height -= hScrollBar.Height;
				hScrollBar.Width = clientSize.Width;
				hScrollBar.Minimum = 0;
				hScrollBar.Maximum = virtualSize.Width;
				hScrollBar.Value = Convert.ToInt32(offsetPercent.X * hScrollBar.Maximum);
				offset.X = hScrollBar.Value;
				if(offset.X > hScrollBar.Maximum - hScrollBar.LargeChange && offset.X != 0) {
					offset.X = hScrollBar.Maximum - hScrollBar.LargeChange;
					offsetPercent.X = (float)offset.X / hScrollBar.Maximum;
				}
			}
		}
		void UpdateVScroll(bool enabled) {
			vScrollBar.Enabled = enabled;
			vScrollBar.Visible = enabled;
			if(enabled) {
				vScrollBar.Location = new Point(clientSize.Width - vScrollBar.Width, 0);
				if(!vScrollBar.TouchMode)
					clientSize.Width -= vScrollBar.Width;
				vScrollBar.Height = clientSize.Height;
				vScrollBar.Minimum = 0;
				vScrollBar.Maximum = virtualSize.Height;
				vScrollBar.Value = Convert.ToInt32(offsetPercent.Y * vScrollBar.Maximum);
				offset.Y = vScrollBar.Value;
				if(offset.Y > vScrollBar.Maximum - vScrollBar.LargeChange && offset.Y != 0) {
					offset.Y = vScrollBar.Maximum - vScrollBar.LargeChange;
					offsetPercent.Y = (float)offset.Y / vScrollBar.Maximum;
				}
			}
		}
		void ApplyOffset() {
			BeginUpdate();
			foreach(ISizable sizableItem in contentProvider.Items)
				sizableItem.SetOffset(offset);
			EndUpdate();
		}
		void BeginUpdate() {
			if(lockUpdateCount++ == 0)
				contentProvider.BeginUpdate();
		}
		void EndUpdate() {
			if(--lockUpdateCount <= 0) {
				contentProvider.EndUpdate();
				lockUpdateCount = 0;
			}
		}
	}
}
