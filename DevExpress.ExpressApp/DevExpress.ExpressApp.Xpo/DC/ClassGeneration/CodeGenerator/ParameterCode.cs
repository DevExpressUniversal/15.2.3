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

namespace DevExpress.ExpressApp.DC.ClassGeneration {
	internal class ParameterCode : CodeProvider {
		private readonly string name;
		private readonly string typeFullName;
		private bool isOut;
		private bool isByRef;
		public ParameterCode(string name, string typeFullName) {
			this.name = name;
			this.typeFullName = typeFullName;
		}
		public bool IsOut {
			get { return isOut; }
			set { isOut = value; }
		}
		public bool IsByRef {
			get { return isByRef; }
			set { isByRef = value; }
		}
		protected override void GetCodeCore(CodeBuilder builder) {
			builder.Append(GetCode(true));
		}
		public string GetCode(bool isAppendType) {
			string result = string.Empty;
			if(isOut) {
				result += "out ";
			}
			else if(isByRef) {
				result += "ref ";
			}
			if(isAppendType) {
				result += string.Format("{0} {1}", typeFullName, name);
			}
			else if(!isOut && !isByRef) { 
				result += string.Format("({0}){1}", typeFullName, name);
			}
			else {
				result += name;
			}
			return result;
		}
	}
}
