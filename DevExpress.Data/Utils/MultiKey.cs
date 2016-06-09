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
namespace DevExpress.Utils {
	public class MultiKey {
		protected object[] keyParts;
		int hashCode;
		public MultiKey(params object[] keyParts) {
			if(keyParts == null)
				throw new ArgumentNullException("keyParts");
			this.keyParts = keyParts;
			int count = keyParts.Length;
			int[] hashCodes = new int[count];
			for(int i = 0; i < count; i++) {
				if(keyParts[i] != null) {
					hashCodes[i] = keyParts[i].GetHashCode();
				}
			}
			hashCode = HashCodeHelper.CalcHashCode(hashCodes);
		}
		public override int GetHashCode() {
			return hashCode;
		}
		public override bool Equals(object obj) {
			MultiKey other = obj as MultiKey;
			if(other == null || GetType() != other.GetType() || keyParts.Length != other.keyParts.Length)
				return false;
			int count = keyParts.Length;
			for(int i = 0; i < count; i++) {
				if(!object.Equals(keyParts[i], other.keyParts[i]))
					return false;
			}
			return true;
		}
	}
}
