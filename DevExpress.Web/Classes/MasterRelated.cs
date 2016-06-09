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

using System.Collections.Generic;
using System.Text;
using System;
using System.Collections;
#if ASP
using System.Web.UI;
using System.Web.UI.WebControls;
#else
using DevExpress.vNext.Internal;
#endif
namespace DevExpress.Web.Internal {
	#region ISupportsCallbackResult
	public interface ISupportsCallbackResult {
		CallbackResult CalcCallbackResult();
	}
	#endregion
	#region IRelatedControl
	public interface IRelatedControl : ISupportsCallbackResult {
		IMasterControl MasterControl { get; }
		string MasterControlID { get; set; }
		string ClientObjectId { get; }
		bool SuppressCallbackResult { get; set; }
		bool IsExternal { get; set; }
	}
	#endregion
	#region IMasterControl
	public interface IMasterControl {
		void RegisterRelatedControl(IRelatedControl control);
		string CalcRelatedControlsCallbackResult();
	}
	#endregion
	public class FakeIRelatedControl : IRelatedControl {
		#region IRelatedControl Members
		public IMasterControl MasterControl { get { return null; } }
		public string MasterControlID { get { return String.Empty; } set { } }
		public string ClientObjectId { get { return String.Empty; } }
		public bool SuppressCallbackResult { get { return false; } set { } }
		public bool IsExternal { get { return true; } set { } }
		#endregion
		#region ISupportsCallbackResult Members
		public CallbackResult CalcCallbackResult() {
			return new CallbackResult();
		}
		#endregion
	}
	#region CallbackResult
	public class CallbackResult {
		const string UsePrevResultParameters = "#UsePrevResult";
		public static readonly CallbackResult Empty = new CallbackResult();
		public static CallbackResult CreateUsePrevResult(string elementId, string clientObjectId) {
			CallbackResult result = new CallbackResult();
			result.Parameters = UsePrevResultParameters;
			result.elementId = elementId;
			result.clientObjectId = clientObjectId;
			return result;
		}
		#region Fields
		string clientObjectId = String.Empty;
		string elementId = String.Empty;
		string innerHtml = String.Empty;
		string parameters = String.Empty;
		#endregion
		#region Properties
		public bool IsEmpty { get { return this == CallbackResult.Empty; } }
		public string ClientObjectId { get { return clientObjectId; } set { clientObjectId = value; } }
		public string ElementId { get { return elementId; } set { elementId = value; } }
		public string InnerHtml { get { return innerHtml; } set { innerHtml = value; } }
		public string Parameters { get { return parameters; } set { parameters = value; } }
		#endregion
	}
	#endregion
	#region CalcCallbackResultHelper
	public class CalcCallbackResultHelper {
		#region Fields
		WebControl control;
		Control replaceableControl;
		#endregion
		public CalcCallbackResultHelper(WebControl control, Control replaceableControl) {
			if (control == null)
				throw new ArgumentNullException("control");
			if (replaceableControl == null)
				throw new ArgumentNullException("replaceableControl");
			this.control = control;
			this.replaceableControl = replaceableControl;
		}
		#region Properties
		protected internal WebControl Control { get { return control; } }
		protected internal Control ReplaceableControl { get { return replaceableControl; } }
		#endregion
		public CallbackResult CalcCallbackResult() {
			CallbackResult result = new CallbackResult();
			result.InnerHtml = RenderUtils.GetControlChildrenRenderResult(replaceableControl);
			result.ClientObjectId = control.ClientID;
			result.ElementId = replaceableControl.ClientID;
			result.Parameters = String.Empty;
			return result;
		}
	}
	#endregion
	#region FindControlHelper
	public class FindControlHelper {
		static Control LookupControlUpward(Control control, string controlId) {
			if (control == null)
				return null;
			Control namingContainer = control.NamingContainer;
			while (namingContainer != null) {
				Control result = namingContainer.FindControl(controlId);
				if (result != null)
					return result;
				namingContainer = namingContainer.NamingContainer;
			}
			return null;
		}
		public static Control LookupControlRecursive(Control control, string controlId) {
			if (control == null)
				return null;
			if (control is INamingContainer) {
				Control result = control.FindControl(controlId);
				if(result != null && result.ID.Equals(controlId, StringComparison.InvariantCultureIgnoreCase)) {
					return result;
				}
			}
			ControlCollection controls = control.Controls;
			int count = controls.Count;
			for (int i = 0; i < count; i++) {
				Control result = LookupControlRecursive(controls[i], controlId);
				if (result != null)
					return result;
			}
			return null;
		}
		public static Control LookupControl(Control control, string controlId) {
			if (string.IsNullOrEmpty(controlId))
				return null;
			Control result = LookupControlUpward(control, controlId);
			if (result == null)
				result = LookupControlRecursive(control.Page, controlId);
			return result;
		}
	}
	#endregion
	#region RelatedControlDefaultImplementation
	public class RelatedControlDefaultImplementation : IRelatedControl {
		#region Fields
		WebControl control;
		IRelatedControl owner;
		IMasterControl masterControl;
		bool suppressCallbackResult;
		bool isExternal = true;
		#endregion
		public RelatedControlDefaultImplementation(WebControl control, IRelatedControl owner) {
			if (control == null)
				throw new ArgumentNullException("control");
			if (owner == null)
				throw new ArgumentNullException("owner");
			this.control = control;
			this.owner = owner;
		}
		#region Properties
		protected internal IRelatedControl Owner { get { return owner; } }
		protected internal WebControl Control { get { return control; } }
		#endregion
		protected internal virtual IMasterControl LookupMasterControl() {
			return FindControlHelper.LookupControl(control, owner.MasterControlID) as IMasterControl;
		}
		#region IRelatedControl implementation
		string IRelatedControl.MasterControlID { get { return owner.MasterControlID; } set { } }
		string IRelatedControl.ClientObjectId { get { return String.Empty; } }
		public bool SuppressCallbackResult { get { return suppressCallbackResult; } set { suppressCallbackResult = value; } }
		public bool IsExternal { get { return isExternal; } set { isExternal = value; } }
		public IMasterControl MasterControl {
			get {
				if (this.masterControl == null)
					this.masterControl = LookupMasterControl();
				return masterControl;
			}
		}
		public virtual CallbackResult CalcCallbackResult() {
			if (suppressCallbackResult)
				return CallbackResult.Empty;
			CallbackResult result = new CallbackResult();
			result.InnerHtml = GenerateInnerHtml();
			result.ClientObjectId = control.ClientID;
			result.ElementId = String.Empty;
			result.Parameters = String.Empty;
			return result;
		}
		#endregion
		public virtual void OnLoad() {
			IMasterControl masterControl = MasterControl;
			if (masterControl == null)
				return;
			masterControl.RegisterRelatedControl(owner);
		}
		protected internal virtual string GenerateInnerHtml() {
			ICallbackEventHandler handler = control as ICallbackEventHandler;
			if (handler != null)
				return handler.GetCallbackResult();
			else
				return String.Empty;
		}
	}
	#endregion
	#region MasterControlDefaultImplementation
	public class MasterControlDefaultImplementation : IMasterControl {
		List<IRelatedControl> relatedControls;
		List<string> relatedControlsClientIds;
		public MasterControlDefaultImplementation() {
			this.relatedControls = new List<IRelatedControl>();
			this.relatedControlsClientIds = new List<string>();
		}
		public List<IRelatedControl> RelatedControls { get { return relatedControls; } }
		internal List<string> RelatedControlsClientIds { get { return relatedControlsClientIds; } }
		public virtual void RegisterRelatedControl(IRelatedControl control) {
			if (control == null)
				throw new ArgumentNullException("control");
			this.relatedControls.Add(control);
		}
		public virtual void AddRelatedControlsRegistrationScript(StringBuilder script, string clientName) {
			ObtainRelatedControlsClientIds();
			script.AppendFormat("ASPx.RelatedControlManager.RegisterRelatedControls({0}, {1});\n",
				HtmlConvertor.ToScript(clientName), HtmlConvertor.ToJSON(RelatedControlsClientIds));
		}
		protected internal virtual void ObtainRelatedControlsClientIds() {
			relatedControlsClientIds.Clear();
			int count = relatedControls.Count;
			for (int i = 0; i < count; i++)
				relatedControlsClientIds.Add(relatedControls[i].ClientObjectId);
		}
		public virtual string CalcRelatedControlsCallbackResult() {
			return CalcRelatedControlsCallbackResult(relatedControls);
		}
		public virtual string CalcRelatedControlsCallbackResult(ICollection controls) {
			StringBuilder sb = new StringBuilder();
			foreach (ISupportsCallbackResult control in controls) {
				if (ShouldCalcCallbackResult(control)) {
					CallbackResult result = control.CalcCallbackResult();
					sb.Append(FormatCallbackResult(result));
				}
			}
			return sb.ToString();
		}
		protected internal virtual bool ShouldCalcCallbackResult(ISupportsCallbackResult control) {
			return true;
		}
		protected internal virtual string FormatCallbackResult(CallbackResult callbackResult) {
			if (callbackResult.IsEmpty)
				return String.Empty;
			int innerHtmlLength = callbackResult.InnerHtml.Length;
			int parametersLength = callbackResult.Parameters.Length;
			int elementIdLength = callbackResult.ElementId.Length;
			int clientIdLength = callbackResult.ClientObjectId.Length;
			string result = String.Format("{0},{1},{2},{3}|{4}", clientIdLength, elementIdLength, innerHtmlLength, parametersLength,
				callbackResult.ClientObjectId + callbackResult.ElementId + callbackResult.InnerHtml + callbackResult.Parameters);
			System.Diagnostics.Debug.WriteLine(String.Format("FormatCallbackResult for '{0}' is {1} chars long", callbackResult.ElementId, result.Length));
			return result;
		}
		public virtual void RemoveInnerRelatedControls() {
			int count = RelatedControls.Count;
			for (int i = count - 1; i >= 0; i--)
				if (!RelatedControls[i].IsExternal)
					RelatedControls.RemoveAt(i);
		}
	}
	#endregion
}
