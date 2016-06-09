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

using DevExpress.Snap.Core.Native;
using DevExpress.Office;
using DevExpress.Utils;
using DevExpress.XtraPrinting;
using DevExpress.XtraPrinting.BarCode;
using DevExpress.XtraRichEdit.Import.OpenXml;
using DevExpress.XtraRichEdit.Model;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;
using System.Xml;
namespace DevExpress.Snap.Core.Import {
	#region SnapCustomRunDestination (abstract class)
	public abstract class SnapCustomRunDestination : ElementDestination {
		ICustomRunObject customRunObject;
		protected SnapCustomRunDestination(SnapImporter importer)
			: base(importer) {
		}
		protected internal ICustomRunObject CustomRunObject { get { return customRunObject; } }
		public override void ProcessElementOpen(XmlReader reader) {
			customRunObject = CreateCustomRunObject();
			Importer.PieceTable.AppendCustomRun(Importer.Position, customRunObject);
		}
		protected abstract ICustomRunObject CreateCustomRunObject();
		protected static ICustomRunObject GetCustomRunObject(WordProcessingMLBaseImporter importer) {
			SnapCustomRunDestination thisObject = (SnapCustomRunDestination)importer.PeekDestination();
			return thisObject.CustomRunObject;
		}
	}
	#endregion
	#region CustomRunLeafElementDestination (abstract class)
	public abstract class CustomRunLeafElementDestination : SnapLeafElementDestination {
		readonly ICustomRunObject customRunObject;
		protected CustomRunLeafElementDestination(SnapImporter importer, ICustomRunObject customRunObject)
			: base(importer) {
			Guard.ArgumentNotNull(customRunObject, "customRunObject");
			this.customRunObject = customRunObject;
		}
		protected ICustomRunObject CustomRunObject { get { return customRunObject; } }
	}
	#endregion
	#region ActualSizeDestination<T>
	public class ActualSizeDestination<TRectangularRunObject> : CustomRunLeafElementDestination where TRectangularRunObject : ICustomRunObject, IRectangularObject {
		public ActualSizeDestination(SnapImporter importer, ICustomRunObject customRunObject)
			: base(importer, customRunObject) {
		}
		public override void ProcessElementOpen(XmlReader reader) {
			TRectangularRunObject run = (TRectangularRunObject)CustomRunObject;
			string height = Importer.ReadDxStringAttr("height", reader);
			string width = Importer.ReadDxStringAttr("width", reader);
			if(!String.IsNullOrEmpty(height) && !String.IsNullOrEmpty(width))
				run.ActualSize = new Size(Convert.ToInt32(width, NumberFormatInfo.InvariantInfo), Convert.ToInt32(height, NumberFormatInfo.InvariantInfo));
		}
	}
	#endregion
	#region BarCodeElementDestination
	public class BarCodeDestination : SnapCustomRunDestination {
		static readonly ElementHandlerTable handlerTable = CreateElementHadlerTable();
		static ElementHandlerTable CreateElementHadlerTable() {
			ElementHandlerTable result = new ElementHandlerTable();
			result.Add("actualSize", OnActualSize);
			result.Add("barCodeGenerator", OnBarCodeGenerator);
			result.Add("alignment", OnAlignment);
			result.Add("textAlignment", OnTextAlignment);
			result.Add("orientation", OnOrientation);
			result.Add("text", OnText);
			result.Add("showText", OnShowText);
			result.Add("autoModule", OnAutoModule);
			result.Add("module", OnModule);
			return result;
		}
		public BarCodeDestination(SnapImporter importer)
			: base(importer) {
		}
		protected internal new SnapImporter Importer { get { return (SnapImporter)base.Importer; } }
		protected internal override ElementHandlerTable ElementHandlerTable { get { return handlerTable; } }
		protected override ICustomRunObject CreateCustomRunObject() {
			return new BarCodeRunObject();
		}
		static Destination OnActualSize(WordProcessingMLBaseImporter importer, XmlReader reader) {
			return new ActualSizeDestination<BarCodeRunObject>((SnapImporter)importer, GetCustomRunObject(importer));
		}
		static Destination OnBarCodeGenerator(WordProcessingMLBaseImporter importer, XmlReader reader) {
			return new BarCodeGeneratorDestination((SnapImporter)importer, GetCustomRunObject(importer));
		}
		static Destination OnText(WordProcessingMLBaseImporter importer, XmlReader reader) {
			return new TextDestination((SnapImporter)importer, GetCustomRunObject(importer));
		}
		static Destination OnShowText(WordProcessingMLBaseImporter importer, XmlReader reader) {
			return new ShowTextDestination((SnapImporter)importer, GetCustomRunObject(importer));
		}
		static Destination OnAlignment(WordProcessingMLBaseImporter importer, XmlReader reader) {
			return new AlignmentDestination((SnapImporter)importer, GetCustomRunObject(importer));
		}
		static Destination OnOrientation(WordProcessingMLBaseImporter importer, XmlReader reader) {
			return new OrientationDestination((SnapImporter)importer, GetCustomRunObject(importer));
		}
		static Destination OnTextAlignment(WordProcessingMLBaseImporter importer, XmlReader reader) {
			return new TextAlignmentDestination((SnapImporter)importer, GetCustomRunObject(importer));
		}
		static Destination OnAutoModule(WordProcessingMLBaseImporter importer, XmlReader reader) {
			return new AutoModuleDestination((SnapImporter)importer, GetCustomRunObject(importer));
		}
		static Destination OnModule(WordProcessingMLBaseImporter importer, XmlReader reader) {
			return new ModuleDestination((SnapImporter)importer, GetCustomRunObject(importer));
		}
	}
	#endregion
	#region BarCodeGeneratorDestination
	public class BarCodeGeneratorDestination : CustomRunLeafElementDestination {
		public BarCodeGeneratorDestination(SnapImporter importer, ICustomRunObject customRunObject)
			: base(importer, customRunObject) {
		}
		public override void ProcessElementOpen(XmlReader reader) {
			string generatorString = Importer.ReadDxStringAttr("val", reader);
			if (String.IsNullOrEmpty(generatorString))
				return;
			try {
				((BarCodeRunObject)CustomRunObject).BarCodeGenerator = SNBarCodeHelper.GetBarCodeGenerator(generatorString);
			} catch { }
		}
	}
	#endregion
	#region AlignmentDestination
	public class AlignmentDestination : CustomRunLeafElementDestination {
		public AlignmentDestination(SnapImporter importer, ICustomRunObject customRunObject)
			: base(importer, customRunObject) {
		}
		public override void ProcessElementOpen(XmlReader reader) {
			string alignment = Importer.ReadDxStringAttr("val", reader);
			if (!String.IsNullOrEmpty(alignment))
				((BarCodeRunObject)CustomRunObject).Alignment = (TextAlignment)Enum.Parse(typeof(TextAlignment), alignment);
		}
	}
	#endregion
	#region TextAlignmentDestination
	public class TextAlignmentDestination : CustomRunLeafElementDestination {
		public TextAlignmentDestination(SnapImporter importer, ICustomRunObject customRunObject)
			: base(importer, customRunObject) {
		}
		public override void ProcessElementOpen(XmlReader reader) {
			string textAlignment = Importer.ReadDxStringAttr("val", reader);
			if (!String.IsNullOrEmpty(textAlignment))
				((BarCodeRunObject)CustomRunObject).TextAlignment = (TextAlignment)Enum.Parse(typeof(TextAlignment), textAlignment);
		}
	}
	#endregion
	#region OrientationDestination
	public class OrientationDestination : CustomRunLeafElementDestination {
		public OrientationDestination(SnapImporter importer, ICustomRunObject customRunObject)
			: base(importer, customRunObject) {
		}
		public override void ProcessElementOpen(XmlReader reader) {
			string orientation = Importer.ReadDxStringAttr("val", reader);
			if (!String.IsNullOrEmpty(orientation))
				((BarCodeRunObject)CustomRunObject).Orientation = (BarCodeOrientation)Enum.Parse(typeof(BarCodeOrientation), orientation);
		}
	}
	#endregion
	#region TextDestination
	public class TextDestination : CustomRunLeafElementDestination {
		public TextDestination(SnapImporter importer, ICustomRunObject customRunObject)
			: base(importer, customRunObject) {
		}
		public override void ProcessElementOpen(XmlReader reader) {
			string text = Importer.ReadDxStringAttr("val", reader);
			if (!String.IsNullOrEmpty(text))
				((BarCodeRunObject)CustomRunObject).Text = text;
		}
	}
	#endregion
	#region ShowTextDestination
	public class ShowTextDestination : CustomRunLeafElementDestination {
		public ShowTextDestination(SnapImporter importer, ICustomRunObject customRunObject)
			: base(importer, customRunObject) {
		}
		public override void ProcessElementOpen(XmlReader reader) {
			((BarCodeRunObject)CustomRunObject).ShowText = Importer.ReadDxBoolAttr("val", reader);
		}
	}
	#endregion
	#region AutoModuleDestination
	public class AutoModuleDestination : CustomRunLeafElementDestination {
		public AutoModuleDestination(SnapImporter importer, ICustomRunObject customRunObject)
			: base(importer, customRunObject) {
		}
		public override void ProcessElementOpen(XmlReader reader) {
			((BarCodeRunObject)CustomRunObject).AutoModule = Importer.ReadDxBoolAttr("val", reader);
		}
	}
	#endregion
	#region ModuleDestination
	public class ModuleDestination : CustomRunLeafElementDestination {
		public ModuleDestination(SnapImporter importer, ICustomRunObject customRunObject)
			: base(importer, customRunObject) {
		}
		public override void ProcessElementOpen(XmlReader reader) {
			double module = Importer.ReadDxIntAttr("val", reader) / 1000.0;
			if (module != 0.0)
				((BarCodeRunObject)CustomRunObject).Module = module;
		}
	}
	#endregion
	#region CheckBoxDestination
	public class CheckBoxDestination : SnapCustomRunDestination {
		static readonly ElementHandlerTable handlerTable = CreateElementHadlerTable();
		static ElementHandlerTable CreateElementHadlerTable() {
			ElementHandlerTable result = new ElementHandlerTable();
			result.Add("checkState", OnCheckState);
			return result;
		}
		public CheckBoxDestination(SnapImporter importer)
			: base(importer) {
		}
		protected internal new SnapImporter Importer { get { return (SnapImporter)base.Importer; } }
		protected internal override ElementHandlerTable ElementHandlerTable { get { return handlerTable; } }
		protected override ICustomRunObject CreateCustomRunObject() {
			return new CheckBoxRunObject();
		}
		static Destination OnCheckState(WordProcessingMLBaseImporter importer, XmlReader reader) {
			return new CheckStateDestination((SnapImporter)importer, GetCustomRunObject(importer));
		}
	}
	#endregion
	#region CheckStateDestination
	public class CheckStateDestination : CustomRunLeafElementDestination {
		public CheckStateDestination(SnapImporter importer, ICustomRunObject customRunObject)
			: base(importer, customRunObject) {
		}
		public override void ProcessElementOpen(XmlReader reader) {
			((CheckBoxRunObject)CustomRunObject).CheckState = (CheckState)Enum.Parse(typeof(CheckState), Importer.ReadDxStringAttr("val", reader), true);
		}
	}
	#endregion
}
