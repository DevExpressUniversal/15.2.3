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
using System.Windows.Forms;
using DevExpress.Compatibility.System.Drawing;
namespace DevExpress.XtraGauges.Core.Customization {
	public partial class CustomizeActionInfo {
#if DXPORTABLE
		public CustomizeActionInfo(string methodName, string description, string descriptionShort, Image image) {
		}
		public CustomizeActionInfo(string methodName, string description, string descriptionShort, Image image, string groupIdAction) {
		}
#endif
		}
	public interface ISupportCustomizeAction {
		CustomizeActionInfo[] GetActions();
	}
	public interface ICustomizationFrameClient : ISupportCustomizeAction {
		event EventHandler Changed;
		Rectangle Bounds { get; set;}
		void ResetAutoLayout();
		CustomizationFrameBase[] CreateCustomizeFrames();
	}
	public enum CursorInfo { Normal, HSizing, VSizing, Rotate, Move, NWSESizing, NESWSizing, Hand }
	public class CustomizationFrameBase : BaseCompositePrimitive {
		ICustomizationFrameClient clientCore;
		public CustomizationFrameBase(ICustomizationFrameClient client) {
			Client = client;
			CreateDesignerFrameItems();
			OnGaugeLayout();
		}
		public ICustomizationFrameClient Client {
			get { return clientCore; }
			set {
				if (Client == value) return;
				SetClientCore(value);
			}
		}
		protected void SetClientCore(ICustomizationFrameClient value) {
			if (Client != null) Client.Changed -= OnGaugeChanged;
			this.clientCore = value;
			if (Client != null) Client.Changed += OnGaugeChanged;
		}
		protected override void OnDispose() {
			SetClientCore(null);
			base.OnDispose();
		}
		void OnGaugeChanged(object sender, EventArgs e) {
			OnGaugeLayout();
		}
		protected virtual void CreateDesignerFrameItems() { }
		protected virtual void OnGaugeLayout() {
			foreach (CustomizationFrameItemBase item in Composite.Elements) {
				item.CalcLayout();
			}
		}
		public RectangleF CorrectRenderRect(RectangleF proposedRect) {
			if (proposedRect == RectangleF.Empty) return proposedRect;
			foreach (CustomizationFrameItemBase item in Composite.Elements) { proposedRect = item.CorrectRenderRect(proposedRect); }
			return proposedRect;
		}
		public virtual CustomizationFrameItemBase CalcHitTest(Point p) {
			return CalcHitInfo(p).Element as CustomizationFrameItemBase;
		}
	}
	public class SimpleResizeFrame : CustomizationFrameBase {
		public SimpleResizeFrame(ICustomizationFrameClient client) : base(client) { Name = "SimpleResizeFrame"; }
		protected override void CreateDesignerFrameItems() {
			ZOrder = 47;
			Composite.Add(new ResizeHRFrameItem(this));
			Composite.Add(new ResizeHLFrameItem(this));
			Composite.Add(new ResizeVTFrameItem(this));
			Composite.Add(new ResizeVBFrameItem(this));
		}
	}
	public class DiagonalResizeFrame : CustomizationFrameBase {
		public DiagonalResizeFrame(ICustomizationFrameClient client) : base(client) { Name = "DiagonalResizeFrame"; }
		protected override void CreateDesignerFrameItems() {
			ZOrder = 48;
			Composite.Add(new ResizeTLFrameItem(this));
			Composite.Add(new ResizeTRFrameItem(this));
			Composite.Add(new ResizeBLFrameItem(this));
			Composite.Add(new ResizeBRFrameItem(this));
		}
	}
	public class SelectionFrame : CustomizationFrameBase {
		public SelectionFrame(ICustomizationFrameClient client)
			: base(client) {
			Name = "SelectionFrame";
			ZOrder = 50;
		}
		protected override void CreateDesignerFrameItems() {
			Composite.Add(new SelectionFrameItem(this));
		}
	}
	public class MoveFrame : CustomizationFrameBase {
		public MoveFrame(ICustomizationFrameClient client)
			: base(client) {
			Name = "BorderFrame";
			ZOrder = 50;
		}
		protected override void CreateDesignerFrameItems() {
			Composite.Add(new MoveFrameItemRenderable(this));
		}
	}
}
