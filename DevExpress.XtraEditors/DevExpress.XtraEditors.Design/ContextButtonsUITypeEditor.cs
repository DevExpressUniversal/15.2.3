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
using System.Drawing.Design;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using DevExpress.Utils;
using DevExpress.Utils.Design;
using System.Drawing;
using DevExpress.Utils.Controls;
using DevExpress.Utils.Design.Internal;
namespace DevExpress.XtraEditors.Design {
	public class ContextItemCollectionUITypeEditor : DXCollectionEditorBase {
		public ContextItemCollectionUITypeEditor(Type type) : base(type) { }
		ContextItemCollection editCollection = null;
		ContextItemEditPreviewControl previewControl;
		protected override Type[] CreateNewItemTypes() {
			return new Type[]{
				typeof(ContextButton),
				typeof(CheckContextButton),
				typeof(RatingContextButton),
				typeof(TrackBarContextButton)
			};
		}
		public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value) {
			editCollection = value as ContextItemCollection;
			using(previewControl = CreatePreviewControl(editCollection)) {
				return base.EditValue(context, provider, value);
			}
		}
		ContextItemEditPreviewControl CreatePreviewControl(ContextItemCollection collection) {
			return new ContextItemEditPreviewControl(CreatePreviewContainer(collection));
		}
		ContextButtonsPreviewControl CreatePreviewContainer(ContextItemCollection collection) {
			ContextButtonsPreviewControl container = null;
			if(collection != null) {
				container = InitPreviewContainer(collection);
				previewControl = new ContextItemEditPreviewControl(container);
			}
			return container;
		}
		ContextButtonsPreviewControl InitPreviewContainer(ContextItemCollection collection) {
			ContextButtonsPreviewControl container = new ContextButtonsPreviewControl();
			if(container.Image == null) 
				container.Image = DevExpress.Utils.ResourceImageHelper.CreateImageFromResources("DevExpress.XtraEditors.Design.Images.ContextButtonsSampleImage.jpg", typeof(ContextButtonsPreviewControl).Assembly);
			container.ContextItems = new ContextItemCollection();
			container.Options.Assign(collection.Options);
			container.UseRightToLeft = collection.Owner.IsRightToLeft;
			container.BorderStyle = Controls.BorderStyles.NoBorder;
			return container;
		}
		protected override Utils.Design.DXCollectionEditorBase.UISettings GetCollectionEditorUISettings() {
			return new UISettings()
			{
				AllowReordering = true,
				ShowPreviewControlBorder = false,
				PreviewControl = previewControl
			};
		}
		protected override object CreateCustomInstance(Type itemType) {
			ContextItem item = null;
			if(editCollection != null) {
				IContextItemProvider provider = editCollection.Owner as IContextItemProvider;
				if(provider != null) {
					item = provider.CreateContextItem(itemType);
					if(item != null) return item;
				}
			}
			ConstructorInfo ci = itemType.GetConstructor(new Type[] { });
			item = (ContextItem)ci.Invoke(new object[] { });
			item.Name = itemType.Name;
			return item;
		}
		public class ContextItemEditPreviewControl : IDXCollectionEditorPreviewControl, IXtraResizableControl, IDisposable {
			public ContextItemEditPreviewControl(ContextButtonsPreviewControl previewControl) {
				this.previewPanel = new PanelControl();
				this.checkPanel = new XtraUserControl();
				this.previewPanel.Controls.Add(this.checkPanel);
				this.previewControl = previewControl;
				this.previewPanel.Controls.Add(this.previewControl);
				this.checkPanel.Dock = DockStyle.Bottom;
				this.chShowAll = new CheckEdit();
				this.checkPanel.Controls.Add(chShowAll);
				this.checkPanel.BackColor = Color.Transparent;
				InitCheckEdit();
				this.previewControl.Dock = DockStyle.Fill;
				this.previewPanel.SizeChanged += panel_SizeChanged;
			}
			void panel_SizeChanged(object sender, EventArgs e) {
				this.chShowAll.Location = new System.Drawing.Point((previewPanel.Width - this.chShowAll.Size.Width) / 2, this.chShowAll.Location.Y);
			}
			void InitCheckEdit() {
				this.chShowAll.Properties.Caption = "Show Items";
				this.chShowAll.Properties.AutoHeight = true;
				this.chShowAll.Properties.AutoWidth = true;
				this.checkPanel.Height = chShowAll.Size.Height;
				this.chShowAll.CheckedChanged += chShowAllCheckedChanged;
			}
			void chShowAllCheckedChanged(object sender, EventArgs e) {
				foreach(ContextItem item in this.previewControl.ContextItems) {
					SetItemForcedVisibility(item);
				}
			}
			void SetItemForcedVisibility(ContextItem item) {
				if(this.chShowAll.Checked)
					item.SetForcedVisibility(ContextItemVisibility.Visible);
				else
					item.SetForcedVisibility(null);
				this.previewControl.Refresh();
			}
			XtraUserControl checkPanel;
			PanelControl previewPanel;
			CheckEdit chShowAll;
			ContextButtonsPreviewControl previewControl;
			public Control Control {
				get { return previewPanel; }
			}
			ContextItemCollection ContextItems { get { return previewControl.ContextItems; } }
			public virtual void OnCollectionChanged(Utils.Design.Internal.CollectionChangedEventArgs args) {
				ContextItem item = args.Item as ContextItem;
				if(item == null) return;
				switch(args.Action) {
					case Utils.Design.Internal.CollectionAction.Add: 
						if(!previewControl.ContextItems.Contains(item))previewControl.ContextItems.Add(item);
						previewControl.Refresh();
						break;
					case Utils.Design.Internal.CollectionAction.Remove:
						if(previewControl.ContextItems.Contains(item)) previewControl.ContextItems.Remove(item);
						previewControl.Refresh();
						break;
					case Utils.Design.Internal.CollectionAction.Reorder:
						ContextItem target = args.TargetItem as ContextItem;
						if(target != null) {
							int targetIndex = previewControl.ContextItems.IndexOf(target);
							previewControl.ContextItems.Remove(item);
							previewControl.ContextItems.Insert(targetIndex, item);
							previewControl.Refresh();
						}
						break;
				}
			}
			public void OnCollectionChanging(CollectionChangingEventArgs args) { }
			public void OnItemChanged(Utils.Design.Internal.PropertyItemChangedEventArgs args) { 
			}
			public void OnSelectedItemChanged(Utils.Design.Internal.SelectedItemChangedEventArgs args) { 
			}
			public void Dispose() {
				if(previewPanel != null) this.previewPanel.SizeChanged -= panel_SizeChanged;
				if(chShowAll != null) this.chShowAll.CheckedChanged += chShowAllCheckedChanged;
				if(previewControl != null)
					previewControl.Dispose();
			}
			event EventHandler IXtraResizableControl.Changed { add { } remove { } }
			bool IXtraResizableControl.IsCaptionVisible {
				get { return true; }
			}
			Size IXtraResizableControl.MaxSize {
				get { return new Size(0,0); }
			}
			Size IXtraResizableControl.MinSize {
				get { return new Size(Math.Max(248, chShowAll.Width), 120 + chShowAll.Size.Height); }
			}
		}
	}
	public class SimpleContextItemCollectionUITypeEditor : ContextItemCollectionUITypeEditor {
		public SimpleContextItemCollectionUITypeEditor(Type type) : base(type) { }
		protected override Type[] CreateNewItemTypes() {
			return new Type[] { typeof(ContextButton) };
		}
	}
}
