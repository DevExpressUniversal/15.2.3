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
using System.Drawing;
using DevExpress.XtraGauges.Base;
using DevExpress.XtraGauges.Core.Base;
using DevExpress.XtraGauges.Core.Customization;
using DevExpress.XtraGauges.Core.Drawing;
using DevExpress.XtraGauges.Core.Primitive;
using DevExpress.XtraGauges.Core.Resources;
using DevExpress.Compatibility.System.Drawing;
namespace DevExpress.XtraGauges.Core.Model {
	public abstract class BaseGaugeModel : BaseCompositePrimitive, ICustomizationFrameClient {
		IGauge ownerCore;
		BoxShape backGroundShape1;
		BoxShape backGroundShape2;
		BoxShape boundsShape;
		protected readonly CustomizeActionInfo defaultInitialization = new CustomizeActionInfo("AddDefaultElements", "Add default Elements", "Add Default Elements", null);
		public IGauge Owner {
			get { return ownerCore; }
		}
		public BaseGaugeModel(IGauge owner)
			: base() {
			this.ownerCore = owner;
			ZOrder = 100;
		}
		public Rectangle Bounds {
			get { return Owner != null ? Owner.Bounds : Rectangle.Empty; }
			set { if(Owner != null) Owner.Bounds = value; }
		}
		public virtual SizeF ContentSize {
			get { return SizeF.Empty; }
		}
		public virtual bool SmartLayout {
			get { return false; }
		}
		public virtual void Calc(IGauge owner, RectangleF bounds) {
			RaiseChanged(EventArgs.Empty);
		}
		public CustomizationFrameBase[] CreateCustomizeFrames() {
			CustomizationFrameBase[] customizeFrames = new CustomizationFrameBase[] { 
				new MoveFrame(this),
				new SimpleResizeFrame(this), 
				new DiagonalResizeFrame(this)
			};
			if(Owner.Container != null)
				customizeFrames = Owner.Container.OnCreateCustomizeFrames(Owner, customizeFrames);
			return customizeFrames;
		}
		CustomizeActionInfo[] ISupportCustomizeAction.GetActions() {
			return GetActionsCore();
		}
		void ICustomizationFrameClient.ResetAutoLayout() {
			(Owner as IGauge).Container.AutoLayout = false;
		}
		protected virtual CustomizeActionInfo[] GetActionsCore() {
			return new CustomizeActionInfo[]{
					 new CustomizeActionInfo("AddLabel", "Add default Label", "Add Label", UIHelper.UIOtherImages[1],"Labels"),
					 new CustomizeActionInfo("AddImageIndicator", "Add default Image", "Add Image", UIHelper.UIOtherImages[2],"Images")
				};
		}
		PenBrushObject brushObject1;
		PenBrushObject brushObject2;
		protected override void OnCreate() {
			base.OnCreate();
			brushObject1 = new PenBrushObject();
			brushObject2 = new PenBrushObject();
			brushObject1.Color = Color.White;
			brushObject2.Color = Color.Black;
			brushObject2.PenPattern = new float[] { 1.5f, 1.5f };
			backGroundShape1 = new BoxShape();
			backGroundShape2 = new BoxShape();
			backGroundShape1.BeginUpdate();
			backGroundShape1.Name = "DesignTimeBackgroundShape1";
			backGroundShape1.AcceptOrder = int.MinValue + 1;
			backGroundShape1.Appearance.BorderBrush = brushObject1;
			backGroundShape1.EndUpdate();
			backGroundShape2.BeginUpdate();
			backGroundShape2.Name = "DesignTimeBackgroundShape2";
			backGroundShape2.AcceptOrder = int.MinValue + 1;
			backGroundShape2.Appearance.BorderBrush = brushObject2;
			backGroundShape2.EndUpdate();
			boundsShape = new BoxShape();
			boundsShape.Name = "BoundsShape";
			boundsShape.AcceptOrder = int.MinValue + 1;
		}
		protected override void OnDispose() {
			Ref.Dispose(ref backGroundShape1);
			Ref.Dispose(ref backGroundShape2);
			Ref.Dispose(ref brushObject2);
			Ref.Dispose(ref brushObject1);
			Ref.Dispose(ref boundsShape);
			base.OnDispose();
		}
		protected virtual bool ShowBackgroundShape {
			get {
				return (Owner != null) && ((Owner.Site != null) || (Owner.Container != null && Owner.Container.EnableCustomizationMode));
			}
		}
		protected override void OnLoadShapes() {
			base.OnLoadShapes();
			if(ShowBackgroundShape)
				AddBGShapes();
			Shapes.Add(boundsShape);
		}
		protected void AddBGShapes() {
			Shapes.Add(backGroundShape1);
			Shapes.Add(backGroundShape2);
		}
		protected internal bool IsDesignTimeHelperShape(BaseShape shape) {
			return (shape == backGroundShape1) || (shape == backGroundShape2);
		}
		public void UpdateBackgroundShape() {
			if(!Shapes.Contains(backGroundShape1) && ShowBackgroundShape) AddBGShapes();
		}
		public void CalculateBackgroundShape(Rectangle bounds) {
			bool useEmptyShapes = !ShowBackgroundShape || Owner.SuppressDrawBorder;
			if(Owner != null) {
				backGroundShape1.Appearance.ReplaceBorderBrush(useEmptyShapes ? BrushObject.Empty : brushObject1);
				backGroundShape2.Appearance.ReplaceBorderBrush(useEmptyShapes ? BrushObject.Empty : brushObject2);
			}
			Size sz = bounds.Size;
			sz.Width = (int)((float)sz.Width / Transform.Elements[0] + 1f);
			sz.Height = (int)((float)sz.Height / Transform.Elements[3] + 1f);
			PointF topLeft = new PointF((bounds.Left - Transform.Elements[4]) / Transform.Elements[0], (bounds.Top - Transform.Elements[5]) / Transform.Elements[3]);
			backGroundShape1.BeginUpdate();
			backGroundShape1.Box = new RectangleF(topLeft, sz);
			backGroundShape1.Bounds = new RectangleF(topLeft, sz);
			backGroundShape1.EndUpdate();
			backGroundShape2.BeginUpdate();
			backGroundShape2.Box = new RectangleF(topLeft, sz);
			backGroundShape2.Bounds = new RectangleF(topLeft, sz);
			backGroundShape2.EndUpdate();
			boundsShape.BeginUpdate();
			boundsShape.Box = new RectangleF(topLeft, sz);
			boundsShape.Bounds = new RectangleF(topLeft, sz);
			boundsShape.EndUpdate();
		}
		protected override bool RaiseCustomDrawElement(IRenderingContext context) {
			BaseGauge gauge = Owner as BaseGauge;
			if(gauge != null)
				return gauge.RaiseCustomDrawElement(context, Painter, Info, Shapes);
			return false;
		}
		public static BaseGaugeModel Find(object sourceObject) {
			if(sourceObject is IGauge) 
				return ((IGauge)sourceObject).Model;
			IElement<IRenderableElement> element = sourceObject as IElement<IRenderableElement>;
			BaseGaugeModel result = element as BaseGaugeModel;
			IElement<IRenderableElement> nextElement = element;
			while(result == null && nextElement != null) {
				nextElement = nextElement.Parent;
				result = nextElement as BaseGaugeModel;
			}
			return result;
		}
	}
}
