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
using DevExpress.Office;
namespace DevExpress.XtraSpreadsheet.Model {
	public class MailOptions : ICloneable<MailOptions> , ISupportsCopyFrom<MailOptions> {
		#region Fields
		bool hasEnvelope;
		bool envelopeVisible;
		bool envelopeInitDone;
		#endregion
		#region Properties
		public bool HasEnvelope { get { return hasEnvelope; } set { hasEnvelope = value; } }
		public bool EnvelopeVisible { get { return envelopeVisible; } set { envelopeVisible = value; } }
		public bool EnvelopeInitDone { get { return envelopeInitDone; } set { envelopeInitDone = value; } }
		#endregion
		public MailOptions Clone() {
			MailOptions result = new MailOptions();
			result.CopyFrom(this);
			return result;
		}
		public void CopyFrom(MailOptions value) {
			this.envelopeInitDone = value.envelopeInitDone;
			this.envelopeVisible = value.envelopeVisible;
			this.hasEnvelope = value.hasEnvelope;
		}
	}
}
