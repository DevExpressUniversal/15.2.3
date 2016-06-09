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

using DevExpress.Office;
using DevExpress.Office.Design.Internal;
using DevExpress.XtraEditors;
using DevExpress.XtraPrinting.Native;
using DevExpress.XtraPrinting.Native.Lines;
using DevExpress.XtraReports.Design;
using DevExpress.XtraRichEdit;
using DevExpress.XtraRichEdit.Design;
using DevExpress.XtraRichEdit.Design.Internal;
using System;
using System.ComponentModel;
using System.Windows.Forms;
namespace DevExpress.Snap.Extensions.Native {
	#region SizePropertyLineController
	class SizePropertyLineController : EditorPropertyLineBaseController {
		public SizePropertyLineController(IDesignerActionPropertyItem actionItem, IDesignerActionList actionList)
			: base(actionItem, actionList) {
		}
		protected override ILine CreateLine(LineFactoryBase lineFactory) {
			return new SizePropertyLine(CreateStringConverter(), Property, actionList.PropertiesContainer);
		}
	}
	#endregion
	#region SizePropertyLine
	public class SizePropertyLine : EditorPropertyLineBase {
		SnapSizeEdit TextIndentEdit { get { return (SnapSizeEdit)baseEdit; } }
		protected override bool ShouldUpdateValue { get { return true; } }
		protected internal bool IsValidating { get; set; }
		public SizePropertyLine(IStringConverter converter, PropertyDescriptor property, object obj)
			: base(converter, property, obj) {
		}
		protected override BaseEdit CreateEditor() {
			return new SnapSizeEdit();
		}
		protected override void IntializeEditor() {
			base.IntializeEditor();
			TextIndentEdit.ValueUnitConverter = new DocumentModelUnitTwipsConverter();
			TextIndentEdit.Properties.MinValue = 0;
			TextIndentEdit.Properties.ValidateOnEnterKey = true;
			TextIndentEdit.Validating += new CancelEventHandler(OnBaseEditValidating);
			TextIndentEdit.ValueChanged += new EventHandler(TextIndentEditValueChanged);
		}
		void TextIndentEditValueChanged(object sender, EventArgs e) {
			if (!Object.Equals(Value, TextIndentEdit.Value))
				Value = TextIndentEdit.Value;
		}
		protected override void OnBaseEditValidating(object sender, CancelEventArgs e) {
			IsValidating = true;
			try {
				base.OnBaseEditValidating(sender, e);
			} finally {
				IsValidating = false;
			}
		}
		protected override void SetEditText(object val) {
			TextIndentEdit.Value = Convert.ToInt32(val);
		}
		protected override void UpdateValueCore(XtraEditors.BaseEdit edit) {
			if (!IsValidating)
				TextIndentEdit.DoValidate();
		}
	}
	#endregion
	#region SnapSizeEdit
	[DXToolboxItem(false)]
	public class SnapSizeEdit : RichTextIndentEdit {
		protected internal override UnitPrecisionDictionary GetUnitPrecisions() {
			return SnapUnitPrecisionDictionary.BarCodePrecisions;
		}
	}
	#endregion
	#region SnapUnitPrecisionDictionary
	public class SnapUnitPrecisionDictionary : UnitPrecisionDictionary {
		static readonly SnapUnitPrecisionDictionary barCodePrecisions;
		static SnapUnitPrecisionDictionary() {
			barCodePrecisions = new SnapUnitPrecisionDictionary();
			barCodePrecisions.Add(DocumentUnit.Centimeter, 1);
			barCodePrecisions.Add(DocumentUnit.Inch, 2);
			barCodePrecisions.Add(DocumentUnit.Millimeter, 0);
			barCodePrecisions.Add(DocumentUnit.Point, 0);
		}
		public static SnapUnitPrecisionDictionary BarCodePrecisions { get { return barCodePrecisions; } }
	}
	#endregion
}
