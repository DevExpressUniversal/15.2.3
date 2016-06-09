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
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Utils;
using DevExpress.Persistent.Base;
using System.ComponentModel;
namespace DevExpress.ExpressApp.ViewVariantsModule {
	public class ChangeVariantController : Controller {
		public const string ChangeVariantActionId = "ChangeVariant";
		public const string NotModifiedEnabledKey = "NotModified";
		public const string HasVariantsManagerChangeVariantActionActiveKey = "HasVariantsManager";
		private const bool IsCurrentFrameViewVariantsManagerOwnerDefaultValue = true;
		private SingleChoiceAction changeVariantAction;
		private VariantsInfo variantsInfo;
		private ICurrentFrameViewVariantsManager currentFrameViewVariantsManager;
		private void ObjectSpace_ModifiedChanged(object sender, EventArgs e) {
			changeVariantAction.Enabled[NotModifiedEnabledKey] = !((IObjectSpace)sender).IsModified;
		}
		private void changeVariantAction_OnExecute(Object sender, SingleChoiceActionExecuteEventArgs args) {
			Guard.ArgumentNotNull(args, "args");
			Guard.ArgumentNotNull(args.SelectedChoiceActionItem, "args.SelectedChoiceActionItem");
			if(CurrentFrameViewVariantsManager == null) {
				throw new InvalidOperationException("VariantsManager is null");
			}
			CurrentFrameViewVariantsManager.ChangeToVariant((VariantInfo)args.SelectedChoiceActionItem.Data);
		}
		private void Frame_ViewChanging(object sender, ViewChangingEventArgs e) {
			if((Frame.View != null) && (Frame.View.ObjectSpace != null)) {
				Frame.View.ObjectSpace.ModifiedChanged -= new EventHandler(ObjectSpace_ModifiedChanged);
			}
			changeVariantAction.Enabled.RemoveItem(NotModifiedEnabledKey);
		}
		private void Frame_ViewChanged(object sender, ViewChangedEventArgs e) {
			if(Frame.View != null) {
				if(AllowChangeVariantWhenObjectSpaceIsModified.HasValue && !AllowChangeVariantWhenObjectSpaceIsModified.Value && Frame.View.ObjectSpace != null) {
					Frame.View.ObjectSpace.ModifiedChanged += new EventHandler(ObjectSpace_ModifiedChanged);
					changeVariantAction.Enabled[NotModifiedEnabledKey] = !Frame.View.ObjectSpace.IsModified;
				}
			}
		}
		private void CurrentViewVariantsChanged_VariantsChanged(object sender, EventArgs e) {
			RefreshChangeVariantAction();
		}
		private void OnChangeVariantActionRefreshed() {
			if(ChangeVariantActionRefreshed != null) {
				ChangeVariantActionRefreshed(this, EventArgs.Empty);
			}
		}
		protected override void OnFrameAssigned() {
			base.OnFrameAssigned();
			if((Frame != null) && (Frame.Application != null) && !AllowChangeVariantWhenObjectSpaceIsModified.HasValue) {
				RefreshAllowChangeVariantWhenObjectSpaceIsModified(Frame.Application.GetType());
			}
		}
		protected override void OnActivated() {
			base.OnActivated();
			if((CurrentFrameViewVariantsManager == null) && (Frame != null) && (Application != null)) {
				ViewVariantsModule module = Application.Modules.FindModule<ViewVariantsModule>();
				if((module != null) && (module.FrameVariantsEngine != null)) {
					CurrentFrameViewVariantsManager = new CurrentFrameViewVariantsManager(Frame, module.FrameVariantsEngine);
				}
			}
			Frame.ViewChanged += new EventHandler<ViewChangedEventArgs>(Frame_ViewChanged);
			Frame.ViewChanging += new EventHandler<ViewChangingEventArgs>(Frame_ViewChanging);
		}
		protected override void OnDeactivated() {
			Frame.ViewChanged -= new EventHandler<ViewChangedEventArgs>(Frame_ViewChanged);
			Frame.ViewChanging -= new EventHandler<ViewChangingEventArgs>(Frame_ViewChanging);
			CurrentFrameViewVariantsManager = null;
			base.OnDeactivated();
		}
		static ChangeVariantController() {
			IsCurrentFrameViewVariantsManagerOwner = IsCurrentFrameViewVariantsManagerOwnerDefaultValue;
		}
		public ChangeVariantController()
			: base() {
			changeVariantAction = new SingleChoiceAction(this, ChangeVariantActionId, PredefinedCategory.View);
			changeVariantAction.Caption = "View";
			changeVariantAction.ToolTip = "Switch current view representation";
			changeVariantAction.Execute += new SingleChoiceActionExecuteEventHandler(changeVariantAction_OnExecute);
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public void RefreshAllowChangeVariantWhenObjectSpaceIsModified(Type applicationType) {
			AllowChangeVariantWhenObjectSpaceIsModified = true;
			if(applicationType != null) {
				while(applicationType != typeof(object)) {
					if(applicationType.Name == "WebApplication") {
						AllowChangeVariantWhenObjectSpaceIsModified = false;
						break;
					}
					applicationType = applicationType.BaseType;
				}
			}
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public void RefreshChangeVariantAction() {
			if(changeVariantAction != null) {
				changeVariantAction.BeginUpdate();
				try {
					changeVariantAction.Active.RemoveItem(HasVariantsManagerChangeVariantActionActiveKey);
					changeVariantAction.SelectedItem = null;
					changeVariantAction.Items.Clear();
					variantsInfo = null;
					if(CurrentFrameViewVariantsManager == null) {
						changeVariantAction.Active[HasVariantsManagerChangeVariantActionActiveKey] = false;
					}
					else {
						variantsInfo = CurrentFrameViewVariantsManager.Variants;
						if(variantsInfo != null) {
							foreach(VariantInfo variantInfo in variantsInfo.Items) {
								changeVariantAction.Items.Add(new ChoiceActionItem(variantInfo.Id, variantInfo.Caption, variantInfo));
							}
							changeVariantAction.SelectedItem = changeVariantAction.Items.FindItemByID(variantsInfo.GetCurrentVariantInfo().Id);
						}
					}
					OnChangeVariantActionRefreshed();
				}
				finally {
					changeVariantAction.EndUpdate();
				}
			}
		}
		public SingleChoiceAction ChangeVariantAction { get { return changeVariantAction; } }
		public bool? AllowChangeVariantWhenObjectSpaceIsModified { get; set; }
		[Browsable(false)]
		public ICurrentFrameViewVariantsManager CurrentFrameViewVariantsManager {
			get { return currentFrameViewVariantsManager; }
			set {
				if(currentFrameViewVariantsManager != null) {
					currentFrameViewVariantsManager.VariantsChanged -= new EventHandler<EventArgs>(CurrentViewVariantsChanged_VariantsChanged);
					if(IsCurrentFrameViewVariantsManagerOwner) {
						currentFrameViewVariantsManager.Dispose();
					}
				}
				currentFrameViewVariantsManager = value;
				if(currentFrameViewVariantsManager != null) {
					currentFrameViewVariantsManager.VariantsChanged += new EventHandler<EventArgs>(CurrentViewVariantsChanged_VariantsChanged);
					RefreshChangeVariantAction();
				}
			}
		}
		public event EventHandler<EventArgs> ChangeVariantActionRefreshed;
		[DefaultValue(IsCurrentFrameViewVariantsManagerOwnerDefaultValue)]
		public static bool IsCurrentFrameViewVariantsManagerOwner { get; set; }
		#region Obsoleted since 15.1
		[Obsolete(ModelVariantsProvider.DefaultVariantObsoleteText)] 
		public const string DefaultVariantId = "Default";
		#endregion
	}
}
