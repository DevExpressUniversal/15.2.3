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
namespace DevExpress.Web.Captcha {
	public class RuntimeCodeGenerator : ICodeGenerator {
		IRandomNumberGenerator randomGenerator;
		ASPxCaptcha owner;
		public RuntimeCodeGenerator(ASPxCaptcha owner, IRandomNumberGenerator randomGenerator) {
			this.randomGenerator = randomGenerator;
			this.owner = owner;
		}
		string ICodeGenerator.GetCode() {
			StringBuilder codeBuilder = new StringBuilder();
			int randomIndex = 0;
			for (int i = 0; i < this.owner.CodeLength; i++) {
				randomIndex = randomGenerator.Next(0, this.owner.CharacterSet.Length);
				codeBuilder.Append(this.owner.CharacterSet[randomIndex]);
			}
			return codeBuilder.ToString();
		}
	}
	public class DesignModeCodeGenerator : ICodeGenerator {
		const string CodeToken = "DevExpress";
		ASPxCaptcha owner;
		public DesignModeCodeGenerator(ASPxCaptcha owner) {
			this.owner = owner;
		}
		string ICodeGenerator.GetCode() {
			StringBuilder codeBuilder = new StringBuilder();
			int tokenEntryCount = this.owner.CodeLength / CodeToken.Length;
			for (int i = 0; i < tokenEntryCount; i++)
				codeBuilder.Append(CodeToken);
			for (int i = 0; codeBuilder.Length != this.owner.CodeLength; i++)
				codeBuilder.Append(CodeToken[i]);
			return codeBuilder.ToString();
		}
	}
}
