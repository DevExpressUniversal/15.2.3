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
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.ComponentModel;
using DevExpress.Web;
using DevExpress.Web.Internal;
using DevExpress.Data.Filtering.Helpers;
using DevExpress.Data.Filtering;
namespace DevExpress.Web.FilterControl {
	public interface IFilterControlOperationsOwner {
		WebFilterOperationsBase CreateOperations(string filterValue);
		void OnAfterFilterApply(bool isClosing);
		void ApplyFilter();
		void ResetFilter();
		void ClearFilter();
		string FilterValue { get; set; }
	}
	public class WebFilterControlOperations {
		IFilterControlOperationsOwner operationOwner;
		Dictionary<string, DoOperation> operations;		
		public WebFilterControlOperations(IFilterControlOperationsOwner operationOwner) {
			this.operationOwner = operationOwner;
			this.operations = new Dictionary<string, DoOperation>();
		}
		public delegate void DoOperation(string[] args);
		protected virtual IFilterControlOperationsOwner OperationOwner { get { return operationOwner; } }
		public virtual void Perform(string arguments) {
			RegisterOperations();
			string[] args = CommonUtils.DeserializeStringArray(arguments).ToArray();
			if(args.Length < 1) return;
			if(this.operations.ContainsKey(args[0])) {
				this.operations[args[0]](args);
			} else {
				PerformStandardOperations(args);
			}
		}
		protected virtual void PerformStandardOperations(string[] opArgs) {
			WebFilterOperationsBase operations = OperationOwner.CreateOperations(OperationOwner.FilterValue);
			operations.Perform(opArgs);
			OperationOwner.FilterValue = operations.ToString();
		}
		protected virtual void RegisterOperations() {
			AddOperation(FilterControlCallbackCommand.Apply, Apply);
			AddOperation(FilterControlCallbackCommand.Reset, Reset);
			AddOperation(FilterControlCallbackCommand.Clear, Clear);
		}
		protected virtual void AddOperation(string opName, DoOperation method) {
			this.operations[opName] = method;
		}
		protected virtual void Apply(string[] args) {
			OperationOwner.ApplyFilter();
			if(args.Length >= 3) {
				OperationOwner.OnAfterFilterApply(args[2] == "T");
			}
		}
		protected virtual void Reset(string[] args) {
			OperationOwner.ResetFilter();
		}
		protected virtual void Clear(string[] args) {
			OperationOwner.ClearFilter();
		}
	}
}
namespace DevExpress.Web.Internal {
	public static class FilterControlCallbackCommand {
		public const string Apply = "Apply";
		public const string Reset = "Reset";
		public const string Clear = "Clear";
	}
}
