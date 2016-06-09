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
using System.Windows.Forms;
using DevExpress.Utils.Controls;
using DevExpress.Utils.Drawing.Animation;
using DevExpress.Utils.Serializing;
namespace DevExpress.XtraBars.Docking2010.Views.Widget {
	public class WidgetsWobbleAnimationInfo : BaseAnimationInfo {
		protected Dictionary<Control, Rectangle> controlsBounds;
		protected WidgetsHost hostCore;
		public WidgetsWobbleAnimationInfo(WidgetsHost host, ISupportXtraAnimation obj, object animationId, int deltaTick, int frameCount)
			: base(obj, animationId, deltaTick, frameCount) {
			hostCore = host;
			controlsBounds = new Dictionary<Control, Rectangle>();
			foreach(KeyValuePair<Point, List<Point>> items in hostCore.View.TableGroup.Info.DropList) {
				foreach(var point in items.Value) {
					var item = host.View.TableGroup.GetDocument(point.X, point.Y);
					if((host.Handler as WidgetsHostHandler).DragItem != item) {
						if(item == null || !item.IsVisible) continue;
						if(controlsBounds.ContainsKey(item.ContainerControl)) continue;
						controlsBounds.Add(item.ContainerControl, Rectangle.Inflate(item.ContainerControl.Bounds, -5, -5));
						item.ContainerControl.Bounds = Rectangle.Inflate(item.ContainerControl.Bounds, -5, -5);
					}
				}
			}
			AnimationType = DevExpress.Utils.Drawing.Animation.AnimationType.Cycle;
		}
		public override void Dispose() {
			base.Dispose();
			foreach(KeyValuePair<Control, Rectangle> item in controlsBounds) {
				if((item.Key as WidgetContainer).Document.Info != null)
					item.Key.Bounds = (item.Key as WidgetContainer).Document.Info.Bounds;
			}
		}
		int i = 0;
		const int offset = 2;
		int[] delta = new int[] { -1, -1, -1, 0, 1, 1, 1, 0 };
		public override void FrameStep() {
			foreach(KeyValuePair<Control, Rectangle> item in controlsBounds) {
				Point p = item.Value.Location;
				int containerIndex = Array.IndexOf(hostCore.Containers, item.Key);
				p.X += delta[(i + offset + containerIndex * 5) % 8];
				p.Y += delta[(i + containerIndex * 5) % 8];
				item.Key.Location = p;
			}
			i++;
		}
	}
	public class FlowLayoutWidgetsWobbleAnimationInfo : WidgetsWobbleAnimationInfo {
		public FlowLayoutWidgetsWobbleAnimationInfo(WidgetsHost host, ISupportXtraAnimation obj, object animationId, int deltaTick, int frameCount)
			: base(host, obj, animationId, deltaTick,frameCount) {
				foreach(var item in host.View.FlowLayoutGroup.Items) {
					if((host.Handler as WidgetsHostHandler).DragItem != item) {
						if(item == null || item is EmptyDocument) continue;
						if(controlsBounds.ContainsKey(item.ContainerControl)) continue;
						controlsBounds.Add(item.ContainerControl, Rectangle.Inflate(item.ContainerControl.Bounds, -5, -5));
						item.ContainerControl.Bounds = Rectangle.Inflate(item.ContainerControl.Bounds, -5, -5);
					}	
				}
			AnimationType = DevExpress.Utils.Drawing.Animation.AnimationType.Cycle;
		}
	}
	public class WidgetsMovingAnimationInfo : BaseAnimationInfo {
		IDocumentGroup groupCore;
		Dictionary<Control, Rectangle> controlsBounds;
		WidgetsHost hostCore;
		public WidgetsMovingAnimationInfo(IDocumentGroup group, ISupportXtraAnimation obj, object animationId, DocumentAnimationProperties properties)
			: base(obj, animationId, properties.FrameInterval, properties.FrameCount) {
			groupCore = group;
			controlsBounds = new Dictionary<Control, Rectangle>();
			hostCore = obj as WidgetsHost;
			foreach(var item in groupCore.Items) {
				if(item.Control == null || item.Control.Parent == null || !item.IsVisible) continue;
				controlsBounds.Add(item.Control.Parent, item.Control.Parent.Bounds);
			}
		}
		public WidgetsHost Host {
			get { return hostCore; }
		}
		public override void FrameStep() {
			double delta = (double)CurrentFrame / (double)FrameCount;
			Rectangle originalBounds;
			Rectangle targetBounds;
			if(controlsBounds.Count != 0) {
				for(int i = 0; i < groupCore.Items.Count; i++) {
					var item = groupCore.Items[i];
					if(item.Control == null) continue;
					if(!controlsBounds.ContainsKey(item.Control.Parent)) continue;
					originalBounds = controlsBounds[item.Control.Parent];
					targetBounds = item.Info.Bounds;
					if(originalBounds == targetBounds) continue;
					Point newLocation = new Point(originalBounds.X + (int)((targetBounds.X - originalBounds.X) * delta), originalBounds.Y + (int)((targetBounds.Y - originalBounds.Y) * delta));
					Size newSize = new Size(originalBounds.Width + (int)((targetBounds.Width - originalBounds.Width) * delta), originalBounds.Height + (int)((targetBounds.Height - originalBounds.Height) * delta));
					item.Control.Parent.Bounds = new Rectangle(newLocation, newSize);
				}
			}
			if(IsFinalFrame) {
				groupCore.LayoutChanged();
				if(XtraAnimator.Current.Animations.GetAnimationsCountByObject(Host) == 1) {
					XtraAnimator.RemoveObject(Host, groupCore);
					hostCore.UnlockUpdateLayout();
				}
			}
		}
	}
	[TypeConverter(typeof(ExpandableObjectConverter))]
	public class DocumentAnimationProperties {
		const int defaultFrameInterval = 20000;
		const int defaultFrameCount = 100;
		int frameIntervalCore;
		int frameCountCore;
		public DocumentAnimationProperties() {
			frameCountCore = defaultFrameCount;
			frameIntervalCore = defaultFrameInterval;
		}
		[DefaultValue(defaultFrameCount), XtraSerializableProperty(XtraSerializationFlags.DefaultValue)]
		public int FrameCount {
			get { return frameCountCore; }
			set { frameCountCore = value; }
		}
		[DefaultValue(defaultFrameInterval), XtraSerializableProperty(XtraSerializationFlags.DefaultValue)]
		public int FrameInterval {
			get { return frameIntervalCore; }
			set { frameIntervalCore = value; }
		}
		public bool ShouldSerialize() {
			return frameIntervalCore != defaultFrameInterval || frameCountCore != defaultFrameCount;
		}
		public void Reset() {
			frameCountCore = defaultFrameCount;
			frameIntervalCore = defaultFrameInterval;
		}
		public override string ToString() {
			return OptionsHelper.GetObjectText(this);
		}
	}
}
