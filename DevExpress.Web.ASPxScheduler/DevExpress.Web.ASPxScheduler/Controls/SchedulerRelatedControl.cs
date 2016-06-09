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
using System.Web.UI;
using System.Web.UI.WebControls;
using DevExpress.Web;
using DevExpress.Web.Internal;
using DevExpress.Web.ASPxScheduler.Internal;
using System.Collections.Generic;
namespace DevExpress.Web.ASPxScheduler {
	[Designer("DevExpress.Web.ASPxScheduler.Design.ASPxSchedulerRelatedControlDesigner, " + AssemblyInfo.SRAssemblySchedulerWebDesignFull)]
	public abstract class ASPxSchedulerRelatedControlBase : ASPxWebControl, IRelatedControl, ISchedulerRelatedControl {
		bool contentVisible = true;
		IMasterControl savedMasterControl;
		#region Properties
		protected internal ASPxScheduler SchedulerControl { get { return MasterControl as ASPxScheduler; } }
		internal bool ContentVisible { get { return contentVisible; } set { contentVisible = value; } }
		protected internal abstract ASPxSchedulerChangeAction RenderActions { get; }
		#endregion
		protected ASPxSchedulerRelatedControlBase() {
		}
		RelatedControlDefaultImplementation defaultRelatedControlImpl;
		RelatedControlDefaultImplementation DefaultRelatedControlImpl {
			get {
				if (defaultRelatedControlImpl == null)
					defaultRelatedControlImpl = CreateDefaultRelatedControl();
				return defaultRelatedControlImpl;
			}
		}
		protected virtual RelatedControlDefaultImplementation CreateDefaultRelatedControl(){
			return new RelatedControlDefaultImplementation(this, this);
		}
		protected override void OnLoad(EventArgs e) {
			base.OnLoad(e);
			DefaultRelatedControlImpl.OnLoad();
		}
		#region IRelatedControl implementation
		[
#if !SL
	DevExpressWebASPxSchedulerLocalizedDescription("ASPxSchedulerRelatedControlBaseMasterControlID"),
#endif
		DefaultValue(""), Themeable(false), AutoFormatDisable(),
		IDReferenceProperty(typeof(ASPxScheduler)),
		TypeConverter("DevExpress.Web.ASPxScheduler.Design.ASPxSchedulerIDConverter, " + AssemblyInfo.SRAssemblySchedulerWebDesignFull)
		]
		public virtual string MasterControlID
		{
			get { return GetStringProperty("MasterCtrlId", String.Empty); }
			set
			{
				SetStringProperty("MasterCtrlId", String.Empty, value);
				LayoutChanged();
			}
		}
		string IRelatedControl.ClientObjectId { get { return this.ClientID; } }
#if !SL
	[DevExpressWebASPxSchedulerLocalizedDescription("ASPxSchedulerRelatedControlBaseMasterControl")]
#endif
		public IMasterControl MasterControl { get { return DefaultRelatedControlImpl.MasterControl; } }
		bool IRelatedControl.SuppressCallbackResult { get { return DefaultRelatedControlImpl.SuppressCallbackResult; } set { DefaultRelatedControlImpl.SuppressCallbackResult = value; } }
		bool IRelatedControl.IsExternal { get { return DefaultRelatedControlImpl.IsExternal; } set { DefaultRelatedControlImpl.IsExternal = value; } }
		public virtual CallbackResult CalcCallbackResult() {
			if (DefaultRelatedControlImpl.SuppressCallbackResult)
				return CallbackResult.Empty;
			EnsureChildControls();
			FinalizeCreateControlHierarchy();
			PrepareControlHierarchy();
			return CalcCallbackResultCore();
		}
		protected internal abstract CallbackResult CalcCallbackResultCore();
		#endregion
		#region ISchedulerRelatedControl implementation
		ASPxSchedulerChangeAction ISchedulerRelatedControl.RenderActions { get { return this.RenderActions; } }
		#endregion
		protected override bool HasContent() {
			return MasterControl != null;
		}
		protected override void CreateControlHierarchy() {
			ParentSkinOwner = MasterControl as ISkinOwner;
			CreateOuterControlHierarchy();
			if (ContentVisible)
				CreateControlContentHierarchy();
		}
		protected override void PrepareControlHierarchy() {
			if(HasContent()) {
				PrepareOuterControlHierarchy();
				if(ContentVisible)
					PrepareControlContentHierarchy();
			}
		}
		protected override void CreateChildControls() {
			this.savedMasterControl = MasterControl;
			base.CreateChildControls();
		}
		protected override void EnsurePreRender() {
			ValidateMasterCoontrol();
			base.EnsurePreRender();
		}
		void ValidateMasterCoontrol() {
			if (MasterControl != this.savedMasterControl) {
				this.savedMasterControl = MasterControl;
				ResetControlHierarchy();
			}
		}
		protected internal abstract void CreateOuterControlHierarchy();
		protected internal abstract void PrepareOuterControlHierarchy();
		protected internal abstract void CreateControlContentHierarchy();
		protected internal abstract void PrepareControlContentHierarchy();
		protected override void PrepareControlStyle(AppearanceStyleBase style) {
			MergeParentSkinOwnerControlStyle(style);
			style.CopyFrom(ControlStyle);
		}
		protected override void GetCreateClientObjectScript(System.Text.StringBuilder stb, string localVarName, string clientName) {
			base.GetCreateClientObjectScript(stb, localVarName, clientName);
			if (!DefaultRelatedControlImpl.IsExternal)
				stb.AppendFormat("{0}.standalone = false;\n", localVarName);
		}
		public override void RegisterStyleSheets() {
			if(MasterControl == null)
				base.RegisterStyleSheets();
		}
	}
	public abstract class ASPxSchedulerRelatedControl : ASPxSchedulerRelatedControlBase {
		#region Fields
		Table mainTable;
		TableCell mainCell;
		#endregion
		#region Properties
		internal Table MainTable { get { return mainTable; } }
		protected internal TableCell MainCell { get { return mainCell; } }
		#endregion
		protected internal override void CreateOuterControlHierarchy() {
			this.mainTable = RenderUtils.CreateTable();
			Controls.Add(mainTable);
			TableRow row = RenderUtils.CreateTableRow();
			mainTable.Rows.Add(row);
			this.mainCell = RenderUtils.CreateTableCell();
			row.Cells.Add(mainCell);
		}
		protected internal override void PrepareOuterControlHierarchy() {
			if(MainTable != null) {
				RenderUtils.AssignAttributes(this, MainTable);
				RenderUtils.SetStyleStringAttribute(MainTable, "display", ContentVisible ? "" : "none");
			}
			if(MainCell != null)
				MainCell.ID = "mainCell";			
		}
		protected internal override CallbackResult CalcCallbackResultCore() {
			CalcCallbackResultHelper helper = new CalcCallbackResultHelper(this, MainCell);
			CallbackResult result = helper.CalcCallbackResult();
			if (!ContentVisible)
				result.InnerHtml = String.Empty;
			return result;
		}
		protected override string GetSkinControlName() {
			return "Scheduler";
		}
		protected override string[] GetChildControlNames() {
			return new string[] { "Editors" };
		}
		protected override Style CreateControlStyle() {
			return new AppearanceStyleBase();
		}
		protected bool IsAlive() {
			return IsEnabled() && SchedulerControl != null;
		}
		protected override void RegisterDefaultRenderCssFile() {
			ResourceManager.RegisterCssResource(Page, typeof(ASPxScheduler), ASPxScheduler.SchedulerDefaultCssResourceName);
		}
	}
}
namespace DevExpress.Web.ASPxScheduler.Internal {
	public interface ISchedulerRelatedControl {
		ASPxSchedulerChangeAction RenderActions { get; }
	}
}
