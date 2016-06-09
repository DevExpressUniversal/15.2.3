#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       eXpressApp Framework                                        }
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
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Model.NodeGenerators;
namespace DevExpress.ExpressApp.Layout {
	public enum FlowDirection { Vertical, Horizontal }
	public enum XafSizeConstraintsType { Default, Custom }
	public class MarkRequiredFieldCaptionEventArgs : EventArgs {
		private string requiredFieldMark;
		private bool needMarkRequiredField = false;
		private ViewItem viewItem;
		public MarkRequiredFieldCaptionEventArgs(ViewItem viewItem, bool needMarkRequiredField) {
			this.needMarkRequiredField = needMarkRequiredField;
			this.requiredFieldMark = "";
			this.viewItem = viewItem;
		}		
		public ViewItem ViewItem {
			get { return viewItem; }
			set { viewItem = value; }
		}
		public string RequiredFieldMark {
			get { return requiredFieldMark; }
			set { requiredFieldMark = value; }
		}
		public bool NeedMarkRequiredField {
			get { return needMarkRequiredField; }
			set { needMarkRequiredField = value; }
		}
	}
	public abstract class LayoutManager : ISupportAppearanceCustomization, IDisposable {
		public const string DefaultCaptionColon = ":";
		private EventHandler<CustomizeAppearanceEventArgs> customizeAppearanceEvent;
		private IModelNode layoutModel;
		private Int32 suffix;
		private string captionColon = DefaultCaptionColon;
		protected String AddUniqueSuffix(String id) {
			suffix++;
			return id + "(" + suffix.ToString() + ")";
		}
		protected internal static String RemoveUniqueSuffix(String id) {
			Int32 suffixStartPosition = id.LastIndexOf("(");
			Int32 suffixEndPosition = id.LastIndexOf(")");
			if((suffixStartPosition >= 0) && (suffixEndPosition == id.Length - 1)) {
				return id.Remove(suffixStartPosition);
			}
			else {
				return id;
			}
		}
		protected abstract Object GetContainerCore();
		protected IModelNode LayoutModel {
			get { return layoutModel; }
		}
		protected virtual void OnCustomizeAppearance(CustomizeAppearanceEventArgs args) {
			if(customizeAppearanceEvent != null) {
				customizeAppearanceEvent(this, args);
			}
		}
		protected virtual void OnLayoutCreated() {
			if(LayoutCreated != null) {
				LayoutCreated(this, EventArgs.Empty);
			}
		}
		protected LayoutManager() { }
		public virtual Object LayoutControls(IModelNode layoutModel, ViewItemsCollection detailViewItems) {
			this.layoutModel = layoutModel;
			if(layoutModel is IModelViewLayout) {
				EnsureLayoutInfo((IModelViewLayout)LayoutModel, detailViewItems); 
			}
			return Container;
		}
		public virtual void BreakLinksToControls() { }
		public virtual void Dispose() { }
		public virtual void SaveModel() { }
		public virtual void ClearLayoutItems() { }
		public virtual void ReplaceControl(String controlID, Object control) { }
		public virtual void UpdateViewItem(ViewItem viewItem) {
			ReplaceControl(viewItem.Id, viewItem.Control);
		}
		public virtual Boolean CustomizationEnabled {
			get { return false; }
			set { ; }
		}
		public Object Container {
			get { return GetContainerCore(); }
		}
		public Boolean DelayedItemsInitialization { get; set; }
		public string CaptionColon {
			get {
				if(ReferenceEquals(captionColon, null)) {
					captionColon = string.Empty;
				}
				return captionColon;
			}
			set { captionColon = value; }
		}
		public bool EnableCaptionColon { get; set; }
		protected virtual void InitializeLayoutOptions(IModelViewLayout layoutModel) {
			IModelLayoutManagerOptions layoutOptions = layoutModel.Parent as IModelLayoutManagerOptions;
			if(layoutOptions != null) {
				CaptionColon = layoutOptions.CaptionColon;
				EnableCaptionColon = layoutOptions.EnableCaptionColon;
			}
		}
		protected virtual void OnMarkRequiredFieldCaption(MarkRequiredFieldCaptionEventArgs args) {
			if(MarkRequiredFieldCaption != null) {
				MarkRequiredFieldCaption(this, args);
			}
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public virtual string BuildItemCaption(string caption, bool needMarkRequiredField, string requiredFieldLabel) {
			string resultCaption = EnsureCaptionColon(caption);
			if(needMarkRequiredField) {
				resultCaption += requiredFieldLabel;
			}
			return resultCaption;
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public virtual string EnsureCaptionColon(string caption) {
			if(EnableCaptionColon && !string.IsNullOrWhiteSpace(caption) && !caption.EndsWith(CaptionColon)) {
				return caption + CaptionColon;
			}
			return caption;
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public virtual string RemoveCaptionColon(string caption) {
			if(EnableCaptionColon && !string.IsNullOrWhiteSpace(caption) && caption.EndsWith(CaptionColon)) {
				return caption.Substring(0, caption.Length - CaptionColon.Length);
			}
			return caption;
		}
		public event EventHandler LayoutCreated;
		public event EventHandler<MarkRequiredFieldCaptionEventArgs> MarkRequiredFieldCaption;
		event EventHandler<CustomizeAppearanceEventArgs> ISupportAppearanceCustomization.CustomizeAppearance {
			add { customizeAppearanceEvent += value; }
			remove { customizeAppearanceEvent -= value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public static void EnsureLayoutInfo(IModelViewLayout layoutInfo, ViewItemsCollection detailViewItems) {
			if((layoutInfo != null) && (layoutInfo.Count == 0) && (detailViewItems.Count > 0)) {
				IModelCompositeView modelView = layoutInfo.Parent as IModelCompositeView;
				IModelLayoutGroup main = layoutInfo.AddNode<IModelLayoutGroup>(ModelDetailViewLayoutNodesGenerator.MainLayoutGroupName);
				int i = 0;
				foreach(ViewItem detailViewItem in detailViewItems) {
					IModelMember modelModelMember = null;
					if(detailViewItem is PropertyEditor) {
						modelModelMember = ((PropertyEditor)detailViewItem).Model.ModelMember;
					}
					if(modelModelMember == null || modelModelMember.IsVisibleInDetailView.GetValueOrDefault(true)) {
						IModelLayoutViewItem itemInfo = main.AddNode<IModelLayoutViewItem>(detailViewItem.Id);
						if(modelView != null) {
							itemInfo.ViewItem = modelView.Items[detailViewItem.Id];
						}
						itemInfo.Index = i++;
					}
				}
			}
		}
	}
}
