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
using System.Text;
using System.Drawing;
using DevExpress.XtraGauges.Core.Primitive;
using DevExpress.XtraGauges.Core.Model;
using System.Drawing.Drawing2D;
using DevExpress.Utils.Menu;
using System.Windows.Forms;
using System.Reflection;
using System.Linq;
using DevExpress.XtraGauges.Win.Base;
using DevExpress.XtraGauges.Core.Base;
using DevExpress.LookAndFeel;
using DevExpress.XtraGauges.Base;
using DevExpress.XtraGauges.Core.Customization;
namespace DevExpress.XtraGauges.Win.Customization {
	public class MenuFrameItem : CustomizationFrameItemBase {
		DXPopupMenu menuCore;
		public MenuFrameItem(CustomizationFrameBase owner)
			: base(owner) {
			Name = "MenuFrameItem";
		}
		protected override void OnCreate() {
			base.OnCreate();
			itemFrameSize = new SizeF(14, 14);
			this.menuCore = new DXPopupMenu();
		}
		protected override void OnDispose() {
			if(Menu != null) {
				Menu.Dispose();
				menuCore = null;
			}
			base.OnDispose();
		}
		protected DXPopupMenu Menu {
			get { return menuCore; }
		}
		protected override void OnDelayedCalculation() {
			Rectangle clientBounds = Owner.Client.Bounds;
			Rectangle container = GetContainerBounds(Size.Round(itemFrameSize));
			if(!container.IsEmpty) {
				int left = Math.Max(clientBounds.Left, container.Left);
				int top = Math.Max(clientBounds.Top, container.Top);
				RectangleF bounds = new RectangleF(left, top,
					Math.Min(clientBounds.Right, container.Right) - left,
					Math.Min(clientBounds.Bottom, container.Bottom) - top);
				if(!bounds.IsEmpty) {
					float x = Math.Max(bounds.Left, Math.Min(bounds.Right, clientBounds.Right - (float)itemFrameSize.Width * 2.5f));
					Bounds = new RectangleF(x - itemFrameSize.Width / 2, bounds.Top - itemFrameSize.Height / 2, 
						itemFrameSize.Width, itemFrameSize.Height);
					return;
				}
			}
			Bounds = new RectangleF(clientBounds.Right - (float)itemFrameSize.Width * 3.0f, clientBounds.Top - itemFrameSize.Height / 2,
				itemFrameSize.Width, itemFrameSize.Height);
		}
		Rectangle GetContainerBounds(Size margins) {
			BaseGaugeModel gaugeModel = BaseGaugeModel.Find(Owner.Client);
			if(gaugeModel != null)
				return new Rectangle(new Point(margins.Width / 2, margins.Height / 2),
					gaugeModel.Owner.Container.Bounds.Size - margins);
			return Rectangle.Empty;
		}
		public override void CalcLayout() {
			SetCalculationDelayed();
			base.CalcLayout();
		}
		protected override void LoadModelShapes() {
			Shapes.Add(UniversalShapesFactory.GetShape(PredefinedShapeNames.MenuItemFrame));
		}
		protected override CursorInfo GetCursor() {
			return CursorInfo.Hand;
		}
		public override void OnLeftClick() {
			ShowMenu();
		}
		public override void OnDoubleClick() {
			if(Owner == null) return;
			object instance = (Owner.Client as BaseGaugeModel != null) ? (object)(Owner.Client as BaseGaugeModel).Owner : (object)Owner.Client;
			ISupportCustomizeAction client = instance as ISupportCustomizeAction;
			if(client != null) {
				CustomizeActionInfo[] actions = client.GetActions();
				if(actions.Length > 0) InvokeMenuAction(instance, actions[0]);
			}
		}
		protected virtual void ShowMenu() {
			if(Owner == null || Owner.Client == null) return;
			CustomizeActionInfo[] info = Owner.Client.GetActions();
			BaseGaugeModel model = BaseGaugeModel.Find(Owner.Client);
			IPlatformTypeProvider form = ((model.Owner as IGauge).Container as Control).FindForm() as IPlatformTypeProvider;
			if(form != null) {
				if(!form.IsWin) {
					List<CustomizeActionInfo> infoList = info.ToList();
					infoList.RemoveAll(p => p.ActionName.Equals("Add Image") || p.ActionName.Equals("Add State Image Indicator"));
					info = infoList.ToArray();
				}
			}
			Menu.Items.Clear();
			foreach(CustomizeActionInfo tInfo in info) {
				DXMenuItem mItem = new DXMenuItem(tInfo.ActionName, new EventHandler(OnMenuItemClick), tInfo.Image);
				mItem.Tag = tInfo;
				Menu.Items.Add(mItem);
			}
			Control control = (model != null) ? (model.Owner as IGauge).Container as Control : null;
			if(control != null) {
				MenuManagerHelper.ShowMenu(Menu, UserLookAndFeel.Default, null, control,
					new Point((int)(Bounds.Right - itemFrameSize.Width * 0.05f), (int)(Bounds.Top + itemFrameSize.Height * 0.05f)));
			}
		}
		protected void OnMenuItemClick(object sender, EventArgs e) {
			DXMenuItem mItem = sender as DXMenuItem;
			if(mItem == null) return;
			object instance = (Owner.Client as BaseGaugeModel != null) ? (object)(Owner.Client as BaseGaugeModel).Owner : (object)Owner.Client;
			InvokeMenuAction(instance, mItem.Tag as CustomizeActionInfo);
		}
		void InvokeMenuAction(object instance, CustomizeActionInfo cInfo) {
			if(cInfo == null) return;
			MethodInfo mi = instance.GetType().GetMethod(cInfo.MethodName, BindingFlags.Instance | BindingFlags.NonPublic);
			IPlatformTypeProvider form = null;
			var gauge = instance as IGauge;
			if(gauge != null) {
				var control = gauge.Container as Control;
				form = control != null ? control.FindForm() as IPlatformTypeProvider : null;
			}
			if(mi == null) mi = instance.GetType().GetMethod(cInfo.MethodName, BindingFlags.Instance | BindingFlags.Public);
			if(mi != null) {
				if(mi.Name.Equals("RunDesigner") && gauge != null) {
					if(form != null)
						mi.Invoke(instance, new object[] { form.IsWin });
					else
						mi.Invoke(instance, new object[] { true });
				}
				else
					mi.Invoke(instance, new object[] { });
			}
		}
	}
	public class ActionListFrame : CustomizationFrameBase {
		public ActionListFrame(ICustomizationFrameClient client)
			: base(client) {
			Name = "ActionListFrame";
		}
		protected override void CreateDesignerFrameItems() {
			Composite.Add(new MenuFrameItem(this));
		}
	}
}
