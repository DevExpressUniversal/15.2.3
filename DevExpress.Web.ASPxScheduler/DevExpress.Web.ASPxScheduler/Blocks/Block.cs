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
using System.Collections.Generic;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
using DevExpress.Web;
using DevExpress.Web.Internal;
using DevExpress.XtraScheduler.Native;
namespace DevExpress.Web.ASPxScheduler.Internal {
	public interface IControlBlock : ISupportsCallbackResult {
		WebControl ContentControl { get; }
		void RenderPostbackScriptBegin(StringBuilder sb, string localVarName, string clientName);
		void RenderPostbackScriptEnd(StringBuilder sb, string localVarName, string clientName);
		void RenderCommonScript(StringBuilder sb, string localVarName, string clientName);
		void RenderCallbackScriptBegin(StringBuilder sb, string localVarName, string clientName);
		void RenderCallbackScriptEnd(StringBuilder sb, string localVarName, string clientName);
	}
	public class ControlBlockCollection : List<IControlBlock> {
		public string GetHtmlCallbackResult(ASPxScheduler control) {
			MasterControlDefaultImplementation impl = new SchedulerControlMasterControlDefaultImplementation(control);
			return impl.CalcRelatedControlsCallbackResult(this);
		}
	}
	public interface IScriptBlockOwner {
		List<IControlBlock> GetScriptBuilderOrderedList();
	}
	public static class HtmlCallbackResultHelper {
		public static string CreateHtml(IControlBlock block) {
			return RenderUtils.GetControlChildrenRenderResult(block.ContentControl);
		}
	}
	[ToolboxItem(false)]
	public abstract class ControlBlock<T> : ASPxWebControlBase, INamingContainer, IControlBlock where T : ASPxWebControl {
		T owner;
		WebControl contentControl;
		bool visible = true;
		protected ControlBlock(T owner) {
			if(owner == null)
				Exceptions.ThrowArgumentNullException("owner");
			this.owner = owner;
		}
		public T Owner { get { return owner; } }
		public WebControl ContentControl { get { return contentControl; } }
		public bool IsBlockVisible { get { return visible; } set { visible = value; } }
		public abstract string ContentControlID { get; }
		protected internal abstract void CreateControlHierarchyCore(Control parent);
		protected internal abstract void FinalizeCreateControlHierarchyCore(Control parent);
		protected internal abstract void PrepareControlHierarchyCore();
		protected virtual bool IsCollapsedToZeroSize() {
			return false;
		}
		protected virtual bool UndoCollapsedToZeroSize() {
			return false;
		}
		protected virtual bool IsHiddenInitially() {
			return false;
		}
		protected override void CreateControlHierarchy() {
			HtmlTextWriterTag contentControlTag = HtmlTextWriterTag.Div;
			this.contentControl = RenderUtils.CreateWebControl(contentControlTag);
			this.contentControl.ID = "innerContent";
			this.ID = ContentControlID;
			Controls.Add(contentControl);
			CreateControlHierarchyCore(contentControl);
		}
		protected override void FinalizeCreateControlHierarchy() {
			FinalizeCreateControlHierarchyCore(contentControl);
			if(this.contentControl != null && this.contentControl.Controls.Count == 0) {
				WebControl emptyDiv = CreateEmptyDiv();
				this.contentControl.Controls.Add(emptyDiv);
			}
		}
		protected internal virtual WebControl CreateEmptyDiv() {
			WebControl div = RenderUtils.CreateDiv();
			div.Style.Add(HtmlTextWriterStyle.Position, "absolute");
			div.Style.Add(HtmlTextWriterStyle.Height, "0");
			return div;
		}
		protected override void PrepareControlHierarchy() {
			PrepareControlHierarchyCore();
			if(IsCollapsedToZeroSize()) 
				SetCollapsedToZeroSizeBlockStyles();
			if(IsHiddenInitially())
				this.contentControl.Style.Add(HtmlTextWriterStyle.Display, "none");
		}
		public virtual void RenderPostbackScriptBegin(StringBuilder sb, string localVarName, string clientName) {
		}
		public virtual void RenderPostbackScriptEnd(StringBuilder sb, string localVarName, string clientName) {
		}
		public virtual void RenderCommonScript(StringBuilder sb, string localVarName, string clientName) {
			PrepareBlockProperties(sb, localVarName);
		}
		public virtual void RenderCallbackScriptBegin(StringBuilder sb, string localVarName, string clientName) {
		}
		public virtual void RenderCallbackScriptEnd(StringBuilder sb, string localVarName, string clientName) {
		}
		BlockRelatedControlDefaultImplementation defaultRelatedControlImplementation;
		BlockRelatedControlDefaultImplementation DefaultRelatedControlImplementation {
			get {
				if (defaultRelatedControlImplementation == null)
					defaultRelatedControlImplementation = CreateDefaultRelatedControlImplementation();
				return defaultRelatedControlImplementation;
			}
		}
		protected virtual BlockRelatedControlDefaultImplementation CreateDefaultRelatedControlImplementation() {
			return new BlockRelatedControlDefaultImplementation(owner, this);
		}
		#region ISupportsCallbackResult implementation
		public virtual CallbackResult CalcCallbackResult() {
			FinalizeCreateControlHierarchy();
			PrepareControlHierarchy();
			CallbackResult result = DefaultRelatedControlImplementation.CalcCallbackResult();
			result.ElementId = ContentControl.ClientID;
			return result;
		}
		#endregion
		void PrepareBlockProperties(StringBuilder sb, string localVarName) {
			Dictionary<String, String> updateProperties = GetPrepareBlockProperties();
			if(updateProperties == null || updateProperties.Count < 1)
				return;
			string jsonString = HtmlConvertor.ToJSON(updateProperties);
			sb.AppendFormat("{0}.PrepareBlockProperties('{1}', {2});", localVarName, ClientID, jsonString);
		}
		protected virtual Dictionary<String, String> GetPrepareBlockProperties() {
			if(IsCollapsedToZeroSize() && UndoCollapsedToZeroSize()) {
				return GetUndoCollapsedToZeroSizeBlockProperties();
			}
			return new Dictionary<String, String>();
		}
		void SetCollapsedToZeroSizeBlockStyles() {
			this.contentControl.Style.Add(HtmlTextWriterStyle.Height, "0px");
			this.contentControl.Style.Add(HtmlTextWriterStyle.FontSize, "0px");
		}
		Dictionary<String, String> GetUndoCollapsedToZeroSizeBlockProperties() {
			Dictionary<String, String> result = new Dictionary<string, string>();
			result.Add("style.height", "");
			return result;
		}
	}
	public class BlockRelatedControlDefaultImplementation : RelatedControlDefaultImplementation {
		IControlBlock block;
		public BlockRelatedControlDefaultImplementation(ASPxWebControl control, IControlBlock block) : base(control, new FakeIRelatedControl()) {
			this.block = block;
		}
		protected override string GenerateInnerHtml() {
			return HtmlCallbackResultHelper.CreateHtml(block);
		}
	}
	public abstract class ASPxSchedulerControlBlock : ControlBlock<ASPxScheduler>, ISchedulerRelatedControl {
		protected ASPxSchedulerControlBlock(ASPxScheduler owner) : base(owner) {
		}
		public abstract ASPxSchedulerChangeAction RenderActions { get; }
		protected void SetupRelatedControl(ASPxSchedulerRelatedControl relatedControl) {
			IRelatedControl relControl = relatedControl as IRelatedControl;
			relControl.SuppressCallbackResult = true;
			relControl.IsExternal = false;
			relatedControl.MasterControlID = Owner.ID;
			relatedControl.ContentVisible = IsBlockVisible;
		}
		public override CallbackResult CalcCallbackResult() {
			CallbackResult result = base.CalcCallbackResult();
			HashValueBase checkSum = HashValueBase.CalcHashValue(result.InnerHtml);
			HashValueBase prevCheckSum;
			Owner.BlockCheckSums.TryGetValue(result.ElementId, out prevCheckSum);
			if(checkSum != prevCheckSum) {
				Owner.BlockCheckSums[result.ElementId] = checkSum;
				return result; 
			}
			else {
				return CreateEmptyCallbackResult(result);
			}
		}
		protected virtual CallbackResult CreateEmptyCallbackResult(CallbackResult originalResult) {
			return CallbackResult.Empty;
		}
	}   
}
