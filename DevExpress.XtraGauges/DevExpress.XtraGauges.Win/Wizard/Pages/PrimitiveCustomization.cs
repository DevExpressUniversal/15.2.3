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
using System.Drawing;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using DevExpress.XtraGauges.Base;
using DevExpress.XtraGauges.Core.Base;
using DevExpress.XtraGauges.Core.Drawing;
using DevExpress.XtraGauges.Core.Primitive;
using DevExpress.XtraGauges.Win.Base;
namespace DevExpress.XtraGauges.Win.Wizard {
	public class PrimitiveCustomizationDesignerPage<T> : BaseGaugeDesignerPage
		where T : BaseElement<IRenderableElement>, ISupportVisualDesigning, ISupportAssign<T>, new() {
		SplitContainerControl splitContainer;
		DesignerPreviewControl previewControlCore;
		DesignerPropertyGrid propertyGridCore;
		IGauge gaugeCore;
		T[] sourcePrimitivesCore;
		T[] designedPrimitivesCore;
		public PrimitiveCustomizationDesignerPage(int index, string caption, Image img, T[] primitives, IGauge gauge)
			: base(1, index, caption, img) {
			this.gaugeCore = gauge;
			this.sourcePrimitivesCore = primitives;
			this.designedPrimitivesCore = new T[SourcePrimitives.Length];
			for(int i = 0; i < SourcePrimitives.Length; i++) {
				DesignedPrimitives[i] = CreateDesignedPrimitive(SourcePrimitives[i]);
			}
			Preview.SelectedItemChanged += OnPreviewSelectedItemChanged;
			Preview.Primitives = DesignedPrimitives;
			designerRootCore = new DesignerRoot(gauge);
			DesignerRootElement.Composite.AddRange(DesignedPrimitives);
		}
		DesignerRoot designerRootCore;
		DesignerRoot DesignerRootElement { get { return designerRootCore; } }
		public PrimitiveCustomizationDesignerPage(int index, string caption, T[] primitives, BaseGaugeWin gauge)
			: this(index, caption, null, primitives, gauge) {
		}
		T CreateDesignedPrimitive(T sourcePrimitive) {
			T designerPrimitive = new T();
			designerPrimitive.Name = (sourcePrimitive.Site == null) ? sourcePrimitive.Name : sourcePrimitive.Site.Name;
			designerPrimitive.Assign(sourcePrimitive);
			designerPrimitive.OnInitDesigner();
			return designerPrimitive;
		}
		protected override void OnCreate() {
			this.splitContainer = new SplitContainerControl();
			this.previewControlCore = new DesignerPreviewControl();
			this.propertyGridCore = new DesignerPropertyGrid(DesignerPropertyGridMode.PropertyView);
			splitContainer.Parent = this;
			splitContainer.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			splitContainer.Dock = DockStyle.Fill;
			splitContainer.Panel2.MinSize = 240;
			splitContainer.FixedPanel = SplitFixedPanel.Panel2;
			splitContainer.Panel1.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			splitContainer.Panel2.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			splitContainer.Panel2.Padding = new Padding(2);
			Preview.Parent = splitContainer.Panel1;
			Preview.Dock = DockStyle.Fill;
			PropertyGrid.Parent = splitContainer.Panel2;
			PropertyGrid.Dock = DockStyle.Fill;
		}
		protected override void OnDispose() {
			this.designerRootCore = null;
			this.designedPrimitivesCore = null;
			this.sourcePrimitivesCore = null;
			this.gaugeCore = null;
			this.settingsCore = SaveSettingsCore();
			if(Preview != null) {
				Preview.SelectedItemChanged -= OnPreviewSelectedItemChanged;
				Preview.Parent = null;
				Preview.Dispose();
				previewControlCore = null;
			}
			if(PropertyGrid != null) {
				PropertyGrid.Parent = null;
				PropertyGrid.Dispose();
				propertyGridCore = null;
			}
		}
		protected internal override void OnDesignerClosing() {
			if(DesignedPrimitives == null) return;
			for(int i = 0; i < DesignedPrimitives.Length; i++) DesignedPrimitives[i].OnCloseDesigner();
		}
		protected internal override BaseElement<IRenderableElement> GetElementByDesignedClone(BaseElement<IRenderableElement> designedClone) {
			BaseElement<IRenderableElement> result = null;
			for(int i = 0; i < DesignedPrimitives.Length; i++) {
				if(DesignedPrimitives[i] == designedClone) {
					result = SourcePrimitives[i] as BaseElement<IRenderableElement>;
					break;
				}
			}
			return result;
		}
		protected internal override BaseElement<IRenderableElement> GetDesignedCloneByElement(BaseElement<IRenderableElement> element) {
			BaseElement<IRenderableElement> result = null;
			for(int i = 0; i < SourcePrimitives.Length; i++) {
				if(SourcePrimitives[i] == element) {
					result = DesignedPrimitives[i] as BaseElement<IRenderableElement>;
					break;
				}
			}
			return result;
		}
		protected internal override bool ProcessElementDuplicateCommand(BaseElement<IRenderableElement> designedPrototype, out BaseElement<IRenderableElement> dupElement) {
#if DEBUGTEST
			bool result = true;
#else
			string msg = "The element [" + designedPrototype.Name + "] will be duplicated.\r\nDo you want to continue?";
			bool result = XtraMessageBox.Show(this, msg, "Gauges Designer", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.Yes;
#endif
			dupElement = result ? Gauge.DuplicateElement(designedPrototype) : null;
			if(result) {
				BaseElement<IRenderableElement> designedClone = CreateDesignedPrimitive(dupElement as T);
				List<T> sElements = new List<T>(SourcePrimitives);
				List<T> dElements = new List<T>(DesignedPrimitives);
				sElements.Add(dupElement as T);
				dElements.Add(designedClone as T);
				sourcePrimitivesCore = sElements.ToArray();
				designedPrimitivesCore = dElements.ToArray();
				DesignerRootElement.Composite.Add(designedClone);
			}
			return result;
		}
		protected internal override bool ProcessElementAddNewCommand(out BaseElement<IRenderableElement> newElement) {
			string type = typeof(T).Name;
#if DEBUGTEST
			bool result = true;
#else
			string msg = "The new " + type + " instance will be created.\r\nDo you want to continue?";
			bool result = XtraMessageBox.Show(this, msg, "Gauges Designer", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.Yes;
#endif
			newElement = result ? WinGaugeExtention.AddGaugeElement(Gauge, typeof(T)) : null;
			if(result) {
				BaseElement<IRenderableElement> designedClone = CreateDesignedPrimitive(newElement as T);
				List<T> sElements = new List<T>(SourcePrimitives);
				List<T> dElements = new List<T>(DesignedPrimitives);
				sElements.Add(newElement as T);
				dElements.Add(designedClone as T);
				sourcePrimitivesCore = sElements.ToArray();
				designedPrimitivesCore = dElements.ToArray();
				DesignerRootElement.Composite.Add(designedClone);
			}
			return result;
		}
		protected internal override void UpdateContent() {
			CheckNames();
			Preview.Primitives = DesignedPrimitives;
		}
		void CheckNames() {
			for(int i = 0; i < SourcePrimitives.Length; i++) {
				if(DesignedPrimitives[i].Name != SourcePrimitives[i].Name)
					DesignedPrimitives[i].Name = SourcePrimitives[i].Name;
			}
		}
		protected internal override bool ProcessElementRemoveCommand(BaseElement<IRenderableElement> designedClone, BaseElement<IRenderableElement> element) {
#if DEBUGTEST
			bool result = true;
#else
			string msg = "The element [" + designedClone.Name + "] will be deleted.\r\nDo you want to continue?";
			bool result = XtraMessageBox.Show(this, msg, "Gauges Designer", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.Yes;
#endif
			if(result) {
				List<T> sElements = new List<T>(SourcePrimitives);
				List<T> dElements = new List<T>(DesignedPrimitives);
				sElements.Remove(element as T);
				dElements.Remove(designedClone as T);
				sourcePrimitivesCore = sElements.ToArray();
				designedPrimitivesCore = dElements.ToArray();
				DesignerRootElement.Composite.Remove(designedClone);
			}
			return result;
		}
		protected internal override void ApplyChanges() {
			for(int i = 0; i < SourcePrimitives.Length; i++) {
				SourcePrimitives[i].Assign(DesignedPrimitives[i]);
			}
		}
		void OnPreviewSelectedItemChanged(PrimitiveChangedEventArgs ea) {
			PropertyGrid.SetSelectedPrimitive(ea.Primitive);
		}
		protected override void OnSetDesignerControl(GaugeDesignerControl designer) {
			Preview.SetDesignerControl(designer);
			PropertyGrid.SetDesignerControl(designer);
		}
		protected internal override bool IsHidden {
			get { return false; }
		}
		protected internal override bool IsAllowed {
			get { return DesignedPrimitives.Length > 0; }
		}
		protected internal override bool IsModified {
			get {
				bool result = false;
				for(int i = 0; i < SourcePrimitives.Length; i++) {
					result |= DesignedPrimitives[i].IsDifferFrom(SourcePrimitives[i]);
				}
				return result;
			}
		}
		protected IGauge Gauge {
			get { return gaugeCore; }
		}
		protected DesignerPreviewControl Preview {
			get { return previewControlCore; }
		}
		protected internal DesignerPropertyGrid PropertyGrid {
			get { return propertyGridCore; }
		}
		protected internal T[] SourcePrimitives {
			get { return sourcePrimitivesCore; }
		}
		protected internal T[] DesignedPrimitives {
			get { return designedPrimitivesCore; }
		}
		protected internal override void LayoutChanged() {
			Invalidate();
			if(!string.IsNullOrEmpty(settingsCore)) {
				string[] collectionProperty = settingsCore.Split(',');
				splitContainer.SplitterPosition = Int32.Parse(collectionProperty[0]);
				Preview.ZoomValue = Int32.Parse(collectionProperty[1]);
				Preview.ShowBackgroundElements = Boolean.Parse(collectionProperty[2]);
				Preview.ShowForegroundElements = Boolean.Parse(collectionProperty[3]);
				Preview.PageBackColor = ARGBColorTranslator.FromHtml(collectionProperty[4]);
				settingsCore = null;
			}
		}
		string settingsCore;
		public override string SaveSettings() {
			if(IsDisposed)
				return settingsCore;
			if(!string.IsNullOrEmpty(settingsCore))
				return settingsCore;
			return SaveSettingsCore();
		}
		public override void LoadSettings(string property) {
			settingsCore = property;
		}
		string SaveSettingsCore() {
			return string.Format("{0},{1},{2},{3},{4}",
				splitContainer.SplitterPosition,
				Preview.ZoomValue,
				Preview.ShowBackgroundElements,
				Preview.ShowForegroundElements,
				ARGBColorTranslator.ToHtml(Preview.PageBackColor));
		}
		class DesignerRoot : BaseCompositePrimitive {
			DevExpress.XtraGauges.Core.Model.BaseGaugeModel parentCore;
			public DesignerRoot(IGauge gauge)
				: base() {
				parentCore = gauge.Model;
			}
			public override IComposite<IRenderableElement> Parent { get { return parentCore; } }
		}
	}
}
