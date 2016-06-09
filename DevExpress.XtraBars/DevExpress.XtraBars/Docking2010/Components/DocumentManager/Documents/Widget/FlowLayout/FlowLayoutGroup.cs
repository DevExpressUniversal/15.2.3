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

using System.ComponentModel;
using System.Windows.Forms;
using DevExpress.Utils;
using DevExpress.Utils.Controls;
using DevExpress.Utils.Serializing;
using DevExpress.XtraBars.Docking2010.Base;
namespace DevExpress.XtraBars.Docking2010.Views.Widget {
	public interface IDocumentGroup {
		IBaseElementInfo Info { get; }
		BaseMutableList<Document> Items { get; }
		void LayoutChanged();
	}
	public enum ItemDragMode { Move, Swap }
	public class FlowLayoutGroup : BaseComponent, IDocumentGroup {
		protected internal const string Name = "FlowLayoutGroup";
		FlowLayoutProperties optionsCore;
		FlowDocumentCollection itemsCore;
		FlowLayoutGroupInfo infoCore;
		WidgetView viewCore;
		ItemDragMode positionChangeModeCore;
		FlowDirection flowDirectionCore;
		public FlowLayoutGroup(WidgetView view)
			: base(null) {
			viewCore = view;
			infoCore = CreateInfo();
			optionsCore = CreateOptions();
			itemsCore = CreateFlowDocumentCollection();
			itemsCore.CollectionChanged += OnItemsCollectionChanged;
			flowDirectionCore = System.Windows.Forms.FlowDirection.LeftToRight;
			positionChangeModeCore = Widget.ItemDragMode.Move;
			wrapContentCore = DefaultBoolean.Default;
		}
		protected override void OnDispose() {
			base.OnDispose();
			itemsCore.CollectionChanged -= OnItemsCollectionChanged;
			itemsCore = null;
		}
		public ItemDragMode ItemDragMode {
			get { return positionChangeModeCore; }
			set { SetValue(ref positionChangeModeCore, value, LayoutChanged); }
		}
		public FlowDirection FlowDirection {
			get { return flowDirectionCore; }
			set { SetValue(ref flowDirectionCore, value, LayoutChanged); }
		}
		DefaultBoolean wrapContentCore;
		public DefaultBoolean WrapContent {
			get { return wrapContentCore; }
			set { SetValue(ref wrapContentCore, value, LayoutChanged); }
		}
		public WidgetView WidgetView {
			get { return viewCore; }
		}
		public FlowDocumentCollection Items {
			get { return itemsCore; }
			set {
				if(value == itemsCore) return;
				itemsCore = value;
			}
		}
		protected virtual FlowDocumentCollection CreateFlowDocumentCollection() {
			return new FlowDocumentCollection(this);
		}
		protected virtual FlowLayoutProperties CreateOptions() {
			return new FlowLayoutProperties(this);
		}
		public FlowLayoutGroupInfo Info { get { return infoCore; } }
		virtual protected FlowLayoutGroupInfo CreateInfo() {
			return new FlowLayoutGroupInfo(viewCore, this);
		}
		void OnItemsCollectionChanged(DevExpress.XtraBars.Docking2010.Base.CollectionChangedEventArgs<Document> ea) {
			if(ea.ChangedType == Base.CollectionChangedType.ElementAdded) {
				if(ea.Element.Info == null)
					ea.Element.SetInfo(new DocumentInfo(viewCore, ea.Element));
			}
			SetLayoutModified();
			LayoutChanged();
		}
		void SetLayoutModified() {
			if(WidgetView != null) {
				WidgetView.SetLayoutModified();
			}
		}
		protected override void OnLayoutChanged() {
			base.OnLayoutChanged();
			if(viewCore.Manager != null) {
				var widgetHost = viewCore.Manager.GetOwnerControl() as WidgetsHost;
				if(widgetHost == null) return;
				widgetHost.Invalidate();
				widgetHost.LayoutChanged();
			}
		}
		#region IDocumentGroup Members
		IBaseElementInfo IDocumentGroup.Info {
			get { return Info; }
		}
		BaseMutableList<Document> IDocumentGroup.Items {
			get { return Items; }
		}
		#endregion
	}
	public class FlowDocumentCollection : BaseDocumentCollection<Document, FlowLayoutGroup> {
		public FlowDocumentCollection(FlowLayoutGroup owner)
			: base(owner) {
		}
		protected override void NotifyOwnerOnInsert(int index) { }
	}
	[TypeConverter(typeof(ExpandableObjectConverter))]
	public class FlowLayoutProperties {
		FlowLayoutGroup ownerCore;
		public FlowLayoutProperties(FlowLayoutGroup owner) {
			ownerCore = owner;
		}
		[DefaultValue(FlowDirection.LeftToRight), XtraSerializableProperty(XtraSerializationFlags.DefaultValue)]
		public FlowDirection FlowDirection {
			get { return ownerCore.FlowDirection; }
			set { ownerCore.FlowDirection = value; }
		}
		[DefaultValue(DefaultBoolean.Default), XtraSerializableProperty(XtraSerializationFlags.DefaultValue)]
		public DefaultBoolean WrapContent {
			get { return ownerCore.WrapContent; }
			set { ownerCore.WrapContent = value; }
		}
		[DefaultValue(ItemDragMode.Move), XtraSerializableProperty(XtraSerializationFlags.DefaultValue)]
		public ItemDragMode ItemDragMode {
			get { return ownerCore.ItemDragMode; }
			set { ownerCore.ItemDragMode = value; }
		}
		[ListBindable(false), TypeConverter(typeof(DevExpress.Utils.Design.UniversalCollectionTypeConverter)),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		XtraSerializableProperty(XtraSerializationVisibility.Collection, true, true, true, 1),
		Browsable(false)]
		public FlowDocumentCollection FlowLayoutItems {
			get { return ownerCore.Items; }
		}
		public bool ShouldSerialize() {
			return WrapContent != DefaultBoolean.Default || FlowDirection != System.Windows.Forms.FlowDirection.LeftToRight || ItemDragMode != Widget.ItemDragMode.Move || FlowLayoutItems.Count != 0;
		}
		public void Reset() {
			WrapContent = DefaultBoolean.Default;
			FlowDirection = System.Windows.Forms.FlowDirection.LeftToRight;
			ItemDragMode = Widget.ItemDragMode.Move;
		}
		public override string ToString() {
			return OptionsHelper.GetObjectText(this);
		}
	}
}
