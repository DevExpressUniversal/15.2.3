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
namespace DevExpress.Office.Utils {
	#region SmartStringBuilder
	public class SmartStringBuilder : IStringBuilder {
		StringBuilderWrapper stringBuilder;
		ChunkedStringBuilder stringBuilderEx;
		IStringBuilder currentStringBuilder;
		public SmartStringBuilder() {
			this.stringBuilder = new StringBuilderWrapper();
			this.currentStringBuilder = this.stringBuilder;
		}
		void ChangeActiveStringBuilder(int deltaLength) {
			if (currentStringBuilder != stringBuilder)
				return;
			if (Length + deltaLength > ChunkedStringBuilder.DefaultMaxBufferSize) {
				this.stringBuilderEx = new ChunkedStringBuilder(stringBuilder.ToString());
				this.stringBuilder = null;
				this.currentStringBuilder = stringBuilderEx;
			}
		}
		#region IStringBuilder Members
		public int Length { get { return currentStringBuilder.Length; } }
		public IStringBuilder Append(char ch) {
			ChangeActiveStringBuilder(1);
			currentStringBuilder.Append(ch);
			return this;
		}
		public void Clear() {
			if (currentStringBuilder == stringBuilder)
				currentStringBuilder.Clear();
			else {
				this.stringBuilder = new StringBuilderWrapper();
				this.currentStringBuilder = this.stringBuilder;
				this.stringBuilderEx = null;
			}
		}
		public override string ToString() {
			return currentStringBuilder.ToString();
		}
		#endregion
	}
	#endregion
}
