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
using DevExpress.ExpressApp.Utils;
namespace DevExpress.ExpressApp.ViewVariantsModule {
	public interface IFrameVariantsEngine {
		VariantsInfo GetVariants(View view);
		void ChangeFrameViewToVariant(Frame frame, VariantsInfo variantsInfo, VariantInfo variantInfo);
	}
	public class FrameVariantsEngine : IFrameVariantsEngine, IDisposable {
		private IViewsFactory viewsFactory;
		private ReplaceViewIdHelper replaceViewIdHelper;
		protected virtual void RecreateFrameViewCore(IViewsFactory viewsFactory, Frame frame, string rootViewId) {
			RecreateFrameView(viewsFactory, frame, rootViewId);
		}
		public FrameVariantsEngine(IVariantsProvider variantsProvider, IViewsFactory viewsFactory) {
			Guard.ArgumentNotNull(variantsProvider, "variantsProvider");
			Guard.ArgumentNotNull(viewsFactory, "viewsFactory");
			this.IsDisposed = false;
			this.VariantsProvider = variantsProvider;
			this.viewsFactory = viewsFactory;
			replaceViewIdHelper = new ReplaceViewIdHelper(VariantsProvider);
			replaceViewIdHelper.Attach(viewsFactory);
		}
		public void Dispose() {
			if(replaceViewIdHelper != null) {
				replaceViewIdHelper.Dispose();
				replaceViewIdHelper = null;
			}
			IsDisposed = true;
		}
		public VariantsInfo GetVariants(View view) {
			Guard.ArgumentNotNull(view, "view");
			string viewId;
			if(!ReplaceViewIdHelper.TryGetRegisteredRootViewId(view, out viewId)) {
				viewId = view.Id;
			}
			return VariantsProvider.GetVariants(viewId);
		}
		public void ChangeFrameViewToVariant(Frame frame, VariantsInfo variantsInfo, VariantInfo variantInfo) {
			variantsInfo.CurrentVariantId = variantInfo.Id;
			VariantsProvider.SaveCurrentVariantId(variantsInfo.RootViewId, variantInfo.Id);
			RecreateFrameViewCore(viewsFactory, frame, variantsInfo.RootViewId);
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public static void RecreateFrameView(IViewsFactory viewsFactory, Frame frame, string rootViewId) {
			Guard.ArgumentNotNull(viewsFactory, "viewsFactory");
			Guard.ArgumentNotNull(frame, "frame");
			if((frame is NestedFrame) && (((NestedFrame)frame).ViewItem is ListPropertyEditor)) {
				ListPropertyEditor listPropertyEditor = (ListPropertyEditor)((NestedFrame)frame).ViewItem;
				listPropertyEditor.RecreateView();
			}
			else {
				Guard.ArgumentNotNull(frame.View, "Frame.View");
				View currentView = frame.View;
				currentView.SkipQueryCanClose = true;
				if(currentView is CompositeView) {
					((CompositeView)currentView).SkipObjectSpaceDisposing = true;
				}
				try {
					View result;
					if(currentView is DetailView) {
						result = viewsFactory.CreateDetailView(currentView.ObjectSpace, rootViewId, false, ((DetailView)currentView).CurrentObject);
					}
					else if(currentView is ListView) {
						ListView listView = (ListView)currentView;
						if(listView.CollectionSource is PropertyCollectionSource) {
							PropertyCollectionSource propertyCS = (PropertyCollectionSource)listView.CollectionSource;
							PropertyCollectionSource newPropertyCollectionSource = viewsFactory.CreatePropertyCollectionSource(
								propertyCS.ObjectSpace, propertyCS.MasterObjectType, propertyCS.MasterObject, propertyCS.MemberInfo, rootViewId, propertyCS.Mode);
							result = viewsFactory.CreateListView(rootViewId, newPropertyCollectionSource, false);
						}
						else {
							result = viewsFactory.CreateListView(rootViewId, viewsFactory.CreateCollectionSource(currentView.ObjectSpace, listView.ObjectTypeInfo.Type, rootViewId), false);
						}
					}
					else if(currentView is DashboardView) {
						result = viewsFactory.CreateDashboardView(currentView.ObjectSpace, rootViewId, false);
					}
					else {
						throw new NotImplementedException();
					}
					result.IsRoot = currentView.IsRoot;
					bool setObjectSpaceOwner = result.ObjectSpace != null && result.ObjectSpace.Owner == currentView;
					if(frame.SetView(result) && setObjectSpaceOwner) {
						result.ObjectSpace.Owner = result;
					}
				}
				finally {
					currentView.SkipQueryCanClose = false;
					if(currentView is CompositeView) {
						((CompositeView)currentView).SkipObjectSpaceDisposing = false;
					}
				}
			}
		}
		[DefaultValue(false)]
		public bool IsDisposed { get; private set; }
		public IVariantsProvider VariantsProvider { get; private set; }
	}
}
