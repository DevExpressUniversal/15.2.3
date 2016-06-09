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
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using DevExpress.XtraGauges.Core.Base;
using DevExpress.XtraGauges.Core.Customization;
using DevExpress.XtraGauges.Core.Model;
using DevExpress.XtraGauges.Core.Resources;
using DevExpress.XtraGauges.Win.Base;
using DevExpress.XtraGauges.Win.Customization;
using DevExpress.XtraGauges.Win.Data;
using DevExpress.XtraGauges.Win.Wizard;
namespace DevExpress.XtraGauges.Win.Gauges.Digital {
	[ToolboxItem(false), DesignTimeVisibleAttribute(false)]
	[Designer("DevExpress.XtraGauges.Win.Design.DigitalBackgroundLayerComponentDesigner, " + AssemblyInfo.SRAssemblyGaugesDesignWin, typeof(System.ComponentModel.Design.IDesigner))]
	public class DigitalBackgroundLayerComponent : DigitalBackgroundLayer, ISupportInitialize, IBindableComponent,
		ICustomizationFrameClient, ISupportVisualDesigning, ISupportAssign<DigitalBackgroundLayerComponent> {
		BaseBindableProvider bindableProviderCore;
		public DigitalBackgroundLayerComponent()
			: base() {
		}
		public DigitalBackgroundLayerComponent(string name)
			: base(name) {
		}
		protected override void OnCreate() {
			base.OnCreate();
			this.bindableProviderCore = CreateBindableProvider();
		}
		protected override void OnDispose() {
			if(BindableProvider != null) {
				BindableProvider.Dispose();
				bindableProviderCore = null;
			}
			base.OnDispose();
		}
		public void BeginInit() {
			BeginUpdate();
		}
		public void EndInit() {
			EndUpdate();
		}
		CustomizationFrameBase[] ICustomizationFrameClient.CreateCustomizeFrames() {
			CustomizationFrameBase[] customizeFrames = new CustomizationFrameBase[] { 
				new SelectionFrame(this),
				new ActionListFrame(this)
			};
			return customizeFrames;
		}
		void ICustomizationFrameClient.ResetAutoLayout() { }
		Rectangle ICustomizationFrameClient.Bounds {
			get {
				Rectangle res = Rectangle.Round(Info.RelativeBoundBox);
				if(res.Width <= 0 | res.Height <= 0 | res.Width > 6000 | res.Height > 6000) {
					res = new Rectangle(0, 0, 100, 100);
				}
				return res;
			}
			set { }
		}
		CustomizeActionInfo[] ISupportCustomizeAction.GetActions() {
			return new CustomizeActionInfo[]{
				new CustomizeActionInfo("RunDesigner", "Open Background Layer Designer Page", "Run Designer", UIHelper.LinearGaugeElementImages[1])
			};
		}
		Type ISupportPropertyGridWrapper.PropertyGridWrapperType {
			get { return typeof(DigitalBackgroundLayerComponentWrapper); }
		}
		void ISupportVisualDesigning.OnInitDesigner() { }
		void ISupportVisualDesigning.OnCloseDesigner() { }
		void ISupportVisualDesigning.RenderDesignerElements(Graphics g) {
			DesignerElementVisualizerHelpers.DrawDesignerElements(g, this);
		}
		void ISupportAssign<DigitalBackgroundLayerComponent>.Assign(DigitalBackgroundLayerComponent layer) {
			Assign(layer);
		}
		bool ISupportAssign<DigitalBackgroundLayerComponent>.IsDifferFrom(DigitalBackgroundLayerComponent layer) {
			return IsDifferFrom(layer);
		}
		protected void RunDesigner() {
			BaseGaugeModel model = BaseGaugeModel.Find(this);
			if(model == null) return;
			using(GaugeDesignerForm designerform = new GaugeDesignerForm(model.Owner)) {
				designerform.Pages = new BaseGaugeDesignerPage[] { 
					new PrimitiveCustomizationDesignerPage<DigitalBackgroundLayerComponent>(2,"Background Layers",UIHelper.LinearGaugeElementImages[1], new DigitalBackgroundLayerComponent[]{this},model.Owner)  
				};
				designerform.ShowDialog();
			}
		}
		protected virtual BaseBindableProvider CreateBindableProvider() {
			return new BaseBindableProvider(this);
		}
		protected BaseBindableProvider BindableProvider {
			get { return bindableProviderCore; }
		}
		[
#if !SL
	DevExpressXtraGaugesWinLocalizedDescription("DigitalBackgroundLayerComponentDataBindings"),
#endif
ParenthesizePropertyName(true), RefreshProperties(RefreshProperties.All)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content), Category("Data")]
		public ControlBindingsCollection DataBindings {
			get { return BindableProvider.DataBindings; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Advanced), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public BindingContext BindingContext {
			get { return BindableProvider.BindingContext; }
			set { BindableProvider.BindingContext = value; }
		}
	}
	[ToolboxItem(false), DesignTimeVisibleAttribute(false)]
	[Designer("DevExpress.XtraGauges.Win.Design.DigitalEffectLayerComponentDesigner, " + AssemblyInfo.SRAssemblyGaugesDesignWin, typeof(System.ComponentModel.Design.IDesigner))]
	public class DigitalEffectLayerComponent : DigitalEffectLayer, ISupportInitialize, IBindableComponent,
		ICustomizationFrameClient, ISupportVisualDesigning, ISupportAssign<DigitalEffectLayerComponent> {
		BaseBindableProvider bindableProviderCore;
		public DigitalEffectLayerComponent()
			: base() {
		}
		public DigitalEffectLayerComponent(string name)
			: base(name) {
		}
		protected override void OnCreate() {
			base.OnCreate();
			this.bindableProviderCore = CreateBindableProvider();
		}
		protected override void OnDispose() {
			if(BindableProvider != null) {
				BindableProvider.Dispose();
				bindableProviderCore = null;
			}
			base.OnDispose();
		}
		public void BeginInit() {
			BeginUpdate();
		}
		public void EndInit() {
			EndUpdate();
		}
		CustomizationFrameBase[] ICustomizationFrameClient.CreateCustomizeFrames() {
			CustomizationFrameBase[] customizeFrames = new CustomizationFrameBase[] { 
				new SelectionFrame(this),
				new ActionListFrame(this)
			};
			return customizeFrames;
		}
		void ICustomizationFrameClient.ResetAutoLayout() { }
		Rectangle ICustomizationFrameClient.Bounds {
			get { return Rectangle.Round(Info.RelativeBoundBox); }
			set { }
		}
		CustomizeActionInfo[] ISupportCustomizeAction.GetActions() {
			return new CustomizeActionInfo[]{
				new CustomizeActionInfo("RunDesigner", "Open Effect Layer Designer Page", "Run Designer", UIHelper.LinearGaugeElementImages[5])
			};
		}
		Type ISupportPropertyGridWrapper.PropertyGridWrapperType {
			get { return typeof(DigitalEffectLayerComponentWrapper); }
		}
		void ISupportVisualDesigning.OnInitDesigner() { }
		void ISupportVisualDesigning.OnCloseDesigner() { }
		void ISupportVisualDesigning.RenderDesignerElements(Graphics g) {
			DesignerElementVisualizerHelpers.DrawDesignerElements(g, this);
		}
		void ISupportAssign<DigitalEffectLayerComponent>.Assign(DigitalEffectLayerComponent layer) {
			Assign(layer);
		}
		bool ISupportAssign<DigitalEffectLayerComponent>.IsDifferFrom(DigitalEffectLayerComponent layer) {
			return IsDifferFrom(layer);
		}
		protected void RunDesigner() {
			BaseGaugeModel model = BaseGaugeModel.Find(this);
			if(model == null) return;
			using(GaugeDesignerForm designerform = new GaugeDesignerForm(model.Owner)) {
				designerform.Pages = new BaseGaugeDesignerPage[] { 
					new PrimitiveCustomizationDesignerPage<DigitalEffectLayerComponent>(3,"Effect Layers",UIHelper.LinearGaugeElementImages[5],new DigitalEffectLayerComponent[]{this},model.Owner) 
				};
				designerform.ShowDialog();
			}
		}
		protected virtual BaseBindableProvider CreateBindableProvider() {
			return new BaseBindableProvider(this);
		}
		protected BaseBindableProvider BindableProvider {
			get { return bindableProviderCore; }
		}
		[
#if !SL
	DevExpressXtraGaugesWinLocalizedDescription("DigitalEffectLayerComponentDataBindings"),
#endif
ParenthesizePropertyName(true), RefreshProperties(RefreshProperties.All)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content), Category("Data")]
		public ControlBindingsCollection DataBindings {
			get { return BindableProvider.DataBindings; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Advanced), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public BindingContext BindingContext {
			get { return BindableProvider.BindingContext; }
			set { BindableProvider.BindingContext = value; }
		}
	}
}
