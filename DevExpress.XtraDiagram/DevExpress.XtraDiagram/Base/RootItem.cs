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
using System.Linq;
using System.Text;
using DevExpress.Diagram.Core;
using DevExpress.XtraDiagram.Extensions;
using DevExpress.XtraDiagram.Utils;
using DevExpress.Utils.Serializing;
using DevExpress.Utils;
using PlatformSize = System.Windows.Size;
using PlatformPoint = System.Windows.Point;
using DevExpress.XtraDiagram.Layout;
namespace DevExpress.XtraDiagram.Base {
	public class DiagramRoot : DiagramContainer, IXtraDiagramRoot, IXtraSerializable, IXtraSerializableLayout, IXtraSerializableLayoutEx {
		DiagramControl diagram;
		public DiagramRoot(DiagramControl diagram) {
			this.diagram = diagram;
		}
		protected sealed override DiagramContainerController CreateContainerController() {
			return CreateRootItemController();
		}
		protected virtual DiagramRootController CreateRootItemController() {
			return new XtraDiagramRootController(this);
		}
		public override int Width {
			get { return Diagram.OptionsView.PageSize.ToSize().Width; }
			set { Diagram.OptionsView.PageSize = new SizeF(value, Height); }
		}
		public override int Height {
			get { return Diagram.OptionsView.PageSize.ToSize().Height; }
			set { Diagram.OptionsView.PageSize = new SizeF(Width, value); }
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never)]
		public override Size Size {
			get { return Diagram.OptionsView.PageSize.ToSize(); }
			set { Diagram.OptionsView.PageSize = value; }
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never)]
		public override DiagramItemCollection Items { get { return base.Items; } }
		protected override IEnumerable<PropertyDescriptor> GetEditableProperties() {
			return ReflectionUtils.GetDiagramPublicProperties(this);
		}
		protected override void SetItemSize(PlatformSize value) {
			Diagram.OptionsView.PageSize = value.ToWinSize();
		}
		protected override PlatformSize GetItemSize() {
			return Diagram.OptionsView.PageSize.ToPlatformSize();
		}
		protected override PlatformPoint GetPosition() {
			return default(PlatformPoint);
		}
		protected override void SetPosition(PlatformPoint value) {
		}
		protected override IXtraDiagramRoot GetRootItem() {
			return this;
		}
		[Browsable(false)]
		public override DiagramAppearanceObject Appearance { get { return base.Appearance; } }
		[Browsable(false)]
		public override bool CanSnapToOtherItems {
			get { return base.CanSnapToOtherItems; }
			set { base.CanSnapToOtherItems = value; }
		}
		[Browsable(false)]
		public override bool CanSnapToThisItem {
			get { return base.CanSnapToThisItem; }
			set { base.CanSnapToThisItem = value; }
		}
		[Browsable(false)]
		public override bool IsSnapScope {
			get { return base.IsSnapScope; }
			set { base.IsSnapScope = value; }
		}
		[Browsable(false)]
		public override bool TabStop {
			get { return base.TabStop; }
			set { base.TabStop = value; }
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never)]
		public override PointFloat Position {
			get { return base.Position; }
			set { base.Position = value; }
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never)]
		public override Size MinSize {
			get { return base.MinSize; }
			set { base.MinSize = value; }
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never)]
		public override Sides Anchors {
			get { return base.Anchors; }
			set { base.Anchors = value; }
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never)]
		public override bool CanCopy {
			get { return base.CanCopy; }
			set { base.CanCopy = value; }
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never)]
		public override bool CanSelect {
			get { return false; }
			set {  }
		}
		protected override bool ShouldSerializeCanSelect() { return false; }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never)]
		public override bool CanDelete {
			get { return false; }
			set { }
		}
		protected override bool ShouldSerializeCanDelete() { return false; }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never)]
		public override bool CanResize {
			get { return false; }
			set { }
		}
		protected override bool ShouldSerializeCanResize() { return false; }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never)]
		public override bool CanMove {
			get { return false; }
			set { }
		}
		protected override bool ShouldSerializeCanMove() { return false; }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never)]
		public override AdjustBoundaryBehavior AdjustBoundsBehavior {
			get { return AdjustBoundaryBehavior.None; }
			set { }
		}
		protected override bool ShouldSerializeAdjustBoundsBehavior() {
			return false;
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never)]
		public override double Angle {
			get { return 0; }
			set { }
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never)]
		public override bool CanRotate {
			get { return false; }
			set { }
		}
		protected override bool ShouldSerializeCanRotate() { return false; }
		#region IXtraSerializable
		void IXtraSerializable.OnStartSerializing() {
			OnStartSerializing();
		}
		void IXtraSerializable.OnEndSerializing() {
			OnEndSerializing();
		}
		void IXtraSerializable.OnStartDeserializing(LayoutAllowEventArgs e) {
			OnStartDeserializing(e);
		}
		void IXtraSerializable.OnEndDeserializing(string restoredVersion) {
			OnEndDeserializing(restoredVersion);
		}
		#endregion
		#region IXtraSerializableLayout
		string IXtraSerializableLayout.LayoutVersion {
			get { return Diagram.OptionsLayout.LayoutVersion; }
		}
		#endregion
		#region IXtraSerializableLayoutEx
		bool IXtraSerializableLayoutEx.AllowProperty(OptionsLayoutBase options, string propertyName, int id) {
			return OnAllowSerializationProperty(options, propertyName, id);
		}
		void IXtraSerializableLayoutEx.ResetProperties(OptionsLayoutBase options) {
			OnResetSerializationProperties(options);
		}
		#endregion
		protected virtual void OnStartSerializing() {
			SerializationController.OnStartSerializing();
		}
		protected virtual void OnEndSerializing() {
			SerializationController.OnEndSerializing();
		}
		protected virtual void OnStartDeserializing(LayoutAllowEventArgs e) {
			SerializationController.OnStartDeserializing(e);
		}
		protected virtual void OnEndDeserializing(string restoredVersion) {
			SerializationController.OnEndDeserializing(restoredVersion);
		}
		protected virtual bool OnAllowSerializationProperty(OptionsLayoutBase options, string propertyName, int id) {
			return SerializationController.OnAllowSerializationProperty(options, propertyName, id);
		}
		protected virtual void OnResetSerializationProperties(OptionsLayoutBase options) {
			SerializationController.OnResetSerializationProperties(options);
		}
		protected DiagramSerializationController SerializationController { get { return Diagram.SerializationController; } }
		#region IDiagramRoot
		void IXtraDiagramRoot.LayoutChanged(bool appearanceChanged) {
			GetXtraDiagram().LayoutChanged(appearanceChanged);
		}
		void IXtraDiagramRoot.AddIntoContainer(DiagramItem item) {
			if(Diagram.Container == null || item.Site != null) return;
			try {
				if(!Diagram.IsLoading)
					Diagram.Container.Add(item, DiagramUtils.GenerateDiagramItemName(Diagram, item));
				else
					Diagram.Container.Add(item);
			}
			catch {
				Diagram.Container.Add(item);
			}
		}
		void IXtraDiagramRoot.RemoveFromContainer(DiagramItem item) {
			if(Diagram.Container != null) Diagram.Container.Remove(item);
		}
		IDiagramControl IDiagramRoot.Diagram { get { return Diagram; } }
		#endregion
		protected IXtraDiagramControl GetXtraDiagram() {
			return (IXtraDiagramControl)Diagram;
		}
		protected DiagramControl Diagram { get { return diagram; } }
	}
}
