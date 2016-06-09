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
using System.Globalization;
using System.Runtime.InteropServices;
using DevExpress.Utils;
using DevExpress.Office;
using DevExpress.Office.History;
using DevExpress.Office.Utils;
using DevExpress.XtraSpreadsheet.Model.History;
using DevExpress.XtraSpreadsheet.Utils;
using DevExpress.XtraSpreadsheet.Drawing;
using DevExpress.Office.Drawing;
namespace DevExpress.XtraSpreadsheet.Model {
	public class TextProperties : ISupportsCopyFrom<TextProperties>, IEquatable<TextProperties>, ISupportsInvalidateNotify {
		#region Fields
		readonly InvalidateProxy innerParent;
		readonly DocumentModel documentModel;
		readonly DrawingTextBodyProperties bodyProperties;
		readonly DrawingTextListStyles listStyles;
		readonly DrawingTextParagraphCollection paragraphs;
		#endregion
		public TextProperties(DocumentModel documentModel) {
			Guard.ArgumentNotNull(documentModel, "documentModel");
			this.innerParent = new InvalidateProxy();
			this.innerParent.NotifyTarget = this;
			this.documentModel = documentModel;
			this.bodyProperties = new DrawingTextBodyProperties(documentModel) { Parent = this.innerParent };
			this.listStyles = new DrawingTextListStyles(documentModel) { Parent = this.innerParent };
			this.paragraphs = new DrawingTextParagraphCollection(documentModel) { Parent = this.innerParent };
		}
		protected internal event EventHandler Changed;
		#region Properties
		protected internal ISupportsInvalidate Parent { get { return innerParent.Target; } set { innerParent.Target = value; } }
		protected internal DocumentModel DocumentModel { get { return documentModel; } }
		public DrawingTextBodyProperties BodyProperties { get { return bodyProperties; } }
		public DrawingTextListStyles ListStyles { get { return listStyles; } }
		public DrawingTextParagraphCollection Paragraphs { get { return paragraphs; } }
		public bool IsDefault { get { return bodyProperties.IsDefault && listStyles.IsDefault && paragraphs.Count == 0; } }
		#endregion
		#region ISupportsCopyFrom<TextProperties> Members
		public void CopyFrom(TextProperties value) {
			Guard.ArgumentNotNull(value, "value");
			this.bodyProperties.CopyFrom(value.bodyProperties);
			this.listStyles.CopyFrom(value.listStyles);
			this.paragraphs.CopyFrom(value.paragraphs);
		}
		#endregion
		#region IEquatable<TextProperties> Members
		public bool Equals(TextProperties other) {
			if (other == null)
				return false;
			if (!BodyProperties.Equals(other.BodyProperties))
				return false;
			if (!ListStyles.Equals(other.ListStyles))
				return false;
			if (!Paragraphs.Equals(other.Paragraphs))
				return false;
			return true;
		}
		#endregion
		public void ResetToStyle() {
			if (IsDefault)
				return;
			this.bodyProperties.ResetToStyle();
			this.listStyles.ResetToStyle();
			this.paragraphs.Clear();
		}
		#region ISupportsInvalidateNotify Members
		public void InvalidateNotify() {
			if (Changed != null)
				Changed(this, EventArgs.Empty);
		}
		#endregion
	}
}
