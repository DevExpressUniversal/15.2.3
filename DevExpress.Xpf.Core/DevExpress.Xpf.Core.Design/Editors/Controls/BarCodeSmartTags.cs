﻿#region Copyright (c) 2000-2015 Developer Express Inc.
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

using DevExpress.Design.SmartTags;
using DevExpress.Xpf.Core.Design;
using System;
using System.Collections.Generic;
using System.Linq;
namespace DevExpress.Xpf.Editors.Design {
	sealed class BarCodeControlPropertyLineProvider : PropertyLinesProviderBase {
		public BarCodeControlPropertyLineProvider() : base(typeof(BarCodeEdit)) { }
		protected override SmartTagLineViewModelFactoryList GetPropertiesImpl(FrameworkElementSmartTagPropertiesViewModel viewModel) {
			SmartTagLineViewModelFactoryList lines = base.GetPropertiesImpl(viewModel);
			lines.Add(() => new BooleanPropertyLineViewModel(viewModel, DependencyPropertyHelper.GetPropertyName(() => BarCodeEdit.AutoModuleProperty)));
			lines.Add(() => new BooleanPropertyLineViewModel(viewModel, DependencyPropertyHelper.GetPropertyName(() => BarCodeEdit.ShowTextProperty)));
			lines.Add(() => new ObjectPropertyLineViewModel(viewModel, DependencyPropertyHelper.GetPropertyName(() => BarCodeEdit.ModuleProperty)));
			lines.Add(() => new ObjectPropertyLineViewModel(viewModel, DependencyPropertyHelper.GetPropertyName(() => BarCodeEdit.EditValueProperty)));
			lines.Add(() => new ItemListPropertyLineViewModel(viewModel, DependencyPropertyHelper.GetPropertyName(() => BarCodeEdit.StyleSettingsProperty), typeof(BarCodeStyleSettings), BarCodeStyleSettingsDXTypeInfoInstanceSource.FromTypeList(BarCodeStyleSettingsStorage.GetSymbologyTypes())));
			lines.Add(() => new NestedPropertyLinesViewModel(viewModel, DependencyPropertyHelper.GetPropertyName(() => BarCodeEdit.StyleSettingsProperty)));
			return lines;
		}
	}
	class BarCodeStyleSettingsDXTypeInfoInstanceSource : DXTypeInfoInstanceSource {
		public new static IEnumerable<InstanceSourceBase> FromTypeList(IEnumerable<Type> types) {
			return types.Select(t => DXTypeInfo.FromType(t)).Select(dxt => new BarCodeStyleSettingsDXTypeInfoInstanceSource(dxt));
		}
		public BarCodeStyleSettingsDXTypeInfoInstanceSource(DXTypeInfo typeInfo) : base(typeInfo) {}
		public override string Name { get { return base.Name.Replace("StyleSettings", ""); } }
	}
	#region following propertylineproviders has been generated by DevExpress.Xpf.Editors.Tests.BarCodeStyleSettingsPropertyLineProvideGenerator.Generate()
	sealed class CodabarStyleSettingsPropertyLineProvider : PropertyLinesProviderBase {
		public CodabarStyleSettingsPropertyLineProvider() : base(typeof(DevExpress.Xpf.Editors.CodabarStyleSettings)) { }
		protected override SmartTagLineViewModelFactoryList GetPropertiesImpl(FrameworkElementSmartTagPropertiesViewModel viewModel) {
			SmartTagLineViewModelFactoryList lines = base.GetPropertiesImpl(viewModel);
			lines.Add(() => new EnumPropertyLineViewModel(viewModel, DependencyPropertyHelper.GetPropertyName(() => DevExpress.Xpf.Editors.CodabarStyleSettings.StartStopPairProperty), typeof(DevExpress.XtraPrinting.BarCode.CodabarStartStopPair)));
			lines.Add(() => new ObjectPropertyLineViewModel(viewModel, DependencyPropertyHelper.GetPropertyName(() => DevExpress.Xpf.Editors.CodabarStyleSettings.WideNarrowRatioProperty)));
			return lines;
		}
	}
	sealed class Code11StyleSettingsPropertyLineProvider : PropertyLinesProviderBase {
		public Code11StyleSettingsPropertyLineProvider() : base(typeof(DevExpress.Xpf.Editors.Code11StyleSettings)) { }
		protected override SmartTagLineViewModelFactoryList GetPropertiesImpl(FrameworkElementSmartTagPropertiesViewModel viewModel) {
			SmartTagLineViewModelFactoryList lines = base.GetPropertiesImpl(viewModel);
			return lines;
		}
	}
	sealed class Code128StyleSettingsPropertyLineProvider : PropertyLinesProviderBase {
		public Code128StyleSettingsPropertyLineProvider() : base(typeof(DevExpress.Xpf.Editors.Code128StyleSettings)) { }
		protected override SmartTagLineViewModelFactoryList GetPropertiesImpl(FrameworkElementSmartTagPropertiesViewModel viewModel) {
			SmartTagLineViewModelFactoryList lines = base.GetPropertiesImpl(viewModel);
			return lines;
		}
	}
	sealed class Code39ExtendedStyleSettingsPropertyLineProvider : PropertyLinesProviderBase {
		public Code39ExtendedStyleSettingsPropertyLineProvider() : base(typeof(DevExpress.Xpf.Editors.Code39ExtendedStyleSettings)) { }
		protected override SmartTagLineViewModelFactoryList GetPropertiesImpl(FrameworkElementSmartTagPropertiesViewModel viewModel) {
			SmartTagLineViewModelFactoryList lines = base.GetPropertiesImpl(viewModel);
			return lines;
		}
	}
	sealed class Code39StyleSettingsPropertyLineProvider : PropertyLinesProviderBase {
		public Code39StyleSettingsPropertyLineProvider() : base(typeof(DevExpress.Xpf.Editors.Code39StyleSettings)) { }
		protected override SmartTagLineViewModelFactoryList GetPropertiesImpl(FrameworkElementSmartTagPropertiesViewModel viewModel) {
			SmartTagLineViewModelFactoryList lines = base.GetPropertiesImpl(viewModel);
			return lines;
		}
	}
	sealed class Code93ExtendedStyleSettingsPropertyLineProvider : PropertyLinesProviderBase {
		public Code93ExtendedStyleSettingsPropertyLineProvider() : base(typeof(DevExpress.Xpf.Editors.Code93ExtendedStyleSettings)) { }
		protected override SmartTagLineViewModelFactoryList GetPropertiesImpl(FrameworkElementSmartTagPropertiesViewModel viewModel) {
			SmartTagLineViewModelFactoryList lines = base.GetPropertiesImpl(viewModel);
			return lines;
		}
	}
	sealed class Code93StyleSettingsPropertyLineProvider : PropertyLinesProviderBase {
		public Code93StyleSettingsPropertyLineProvider() : base(typeof(DevExpress.Xpf.Editors.Code93StyleSettings)) { }
		protected override SmartTagLineViewModelFactoryList GetPropertiesImpl(FrameworkElementSmartTagPropertiesViewModel viewModel) {
			SmartTagLineViewModelFactoryList lines = base.GetPropertiesImpl(viewModel);
			return lines;
		}
	}
	sealed class CodeMSIStyleSettingsPropertyLineProvider : PropertyLinesProviderBase {
		public CodeMSIStyleSettingsPropertyLineProvider() : base(typeof(DevExpress.Xpf.Editors.CodeMSIStyleSettings)) { }
		protected override SmartTagLineViewModelFactoryList GetPropertiesImpl(FrameworkElementSmartTagPropertiesViewModel viewModel) {
			SmartTagLineViewModelFactoryList lines = base.GetPropertiesImpl(viewModel);
			lines.Add(() => new EnumPropertyLineViewModel(viewModel, DependencyPropertyHelper.GetPropertyName(() => DevExpress.Xpf.Editors.CodeMSIStyleSettings.MSICheckSumProperty), typeof(DevExpress.XtraPrinting.BarCode.MSICheckSum)));
			return lines;
		}
	}
	sealed class DataBarStyleSettingsPropertyLineProvider : PropertyLinesProviderBase {
		public DataBarStyleSettingsPropertyLineProvider() : base(typeof(DevExpress.Xpf.Editors.DataBarStyleSettings)) { }
		protected override SmartTagLineViewModelFactoryList GetPropertiesImpl(FrameworkElementSmartTagPropertiesViewModel viewModel) {
			SmartTagLineViewModelFactoryList lines = base.GetPropertiesImpl(viewModel);
			lines.Add(() => new ObjectPropertyLineViewModel(viewModel, DependencyPropertyHelper.GetPropertyName(() => DevExpress.Xpf.Editors.DataBarStyleSettings.FNC1SubstituteProperty)));
			lines.Add(() => new ObjectPropertyLineViewModel(viewModel, DependencyPropertyHelper.GetPropertyName(() => DevExpress.Xpf.Editors.DataBarStyleSettings.SegmentsInRowProperty)));
			lines.Add(() => new EnumPropertyLineViewModel(viewModel, DependencyPropertyHelper.GetPropertyName(() => DevExpress.Xpf.Editors.DataBarStyleSettings.TypeProperty), typeof(DevExpress.XtraPrinting.BarCode.DataBarType)));
			return lines;
		}
	}
	sealed class DataMatrixGS1StyleSettingsPropertyLineProvider : PropertyLinesProviderBase {
		public DataMatrixGS1StyleSettingsPropertyLineProvider() : base(typeof(DevExpress.Xpf.Editors.DataMatrixGS1StyleSettings)) { }
		protected override SmartTagLineViewModelFactoryList GetPropertiesImpl(FrameworkElementSmartTagPropertiesViewModel viewModel) {
			SmartTagLineViewModelFactoryList lines = base.GetPropertiesImpl(viewModel);
			lines.Add(() => new ObjectPropertyLineViewModel(viewModel, DependencyPropertyHelper.GetPropertyName(() => DevExpress.Xpf.Editors.DataMatrixGS1StyleSettings.FNC1SubstituteProperty)));
			lines.Add(() => new BooleanPropertyLineViewModel(viewModel, DependencyPropertyHelper.GetPropertyName(() => DevExpress.Xpf.Editors.DataMatrixGS1StyleSettings.HumanReadableTextProperty)));
			lines.Add(() => new EnumPropertyLineViewModel(viewModel, DependencyPropertyHelper.GetPropertyName(() => DevExpress.Xpf.Editors.DataMatrixGS1StyleSettings.MatrixSizeProperty), typeof(DevExpress.XtraPrinting.BarCode.DataMatrixSize)));
			return lines;
		}
	}
	sealed class DataMatrixStyleSettingsPropertyLineProvider : PropertyLinesProviderBase {
		public DataMatrixStyleSettingsPropertyLineProvider() : base(typeof(DevExpress.Xpf.Editors.DataMatrixStyleSettings)) { }
		protected override SmartTagLineViewModelFactoryList GetPropertiesImpl(FrameworkElementSmartTagPropertiesViewModel viewModel) {
			SmartTagLineViewModelFactoryList lines = base.GetPropertiesImpl(viewModel);
			lines.Add(() => new EnumPropertyLineViewModel(viewModel, DependencyPropertyHelper.GetPropertyName(() => DevExpress.Xpf.Editors.DataMatrixStyleSettings.CompactionModeProperty), typeof(DevExpress.XtraPrinting.BarCode.DataMatrixCompactionMode)));
			lines.Add(() => new EnumPropertyLineViewModel(viewModel, DependencyPropertyHelper.GetPropertyName(() => DevExpress.Xpf.Editors.DataMatrixStyleSettings.MatrixSizeProperty), typeof(DevExpress.XtraPrinting.BarCode.DataMatrixSize)));
			return lines;
		}
	}
	sealed class EAN128StyleSettingsPropertyLineProvider : PropertyLinesProviderBase {
		public EAN128StyleSettingsPropertyLineProvider() : base(typeof(DevExpress.Xpf.Editors.EAN128StyleSettings)) { }
		protected override SmartTagLineViewModelFactoryList GetPropertiesImpl(FrameworkElementSmartTagPropertiesViewModel viewModel) {
			SmartTagLineViewModelFactoryList lines = base.GetPropertiesImpl(viewModel);
			lines.Add(() => new ObjectPropertyLineViewModel(viewModel, DependencyPropertyHelper.GetPropertyName(() => DevExpress.Xpf.Editors.EAN128StyleSettings.FNC1SubstituteProperty)));
			lines.Add(() => new BooleanPropertyLineViewModel(viewModel, DependencyPropertyHelper.GetPropertyName(() => DevExpress.Xpf.Editors.EAN128StyleSettings.HumanReadableTextProperty)));
			return lines;
		}
	}
	sealed class EAN13StyleSettingsPropertyLineProvider : PropertyLinesProviderBase {
		public EAN13StyleSettingsPropertyLineProvider() : base(typeof(DevExpress.Xpf.Editors.EAN13StyleSettings)) { }
		protected override SmartTagLineViewModelFactoryList GetPropertiesImpl(FrameworkElementSmartTagPropertiesViewModel viewModel) {
			SmartTagLineViewModelFactoryList lines = base.GetPropertiesImpl(viewModel);
			return lines;
		}
	}
	sealed class EAN8StyleSettingsPropertyLineProvider : PropertyLinesProviderBase {
		public EAN8StyleSettingsPropertyLineProvider() : base(typeof(DevExpress.Xpf.Editors.EAN8StyleSettings)) { }
		protected override SmartTagLineViewModelFactoryList GetPropertiesImpl(FrameworkElementSmartTagPropertiesViewModel viewModel) {
			SmartTagLineViewModelFactoryList lines = base.GetPropertiesImpl(viewModel);
			return lines;
		}
	}
	sealed class Industrial2of5StyleSettingsPropertyLineProvider : PropertyLinesProviderBase {
		public Industrial2of5StyleSettingsPropertyLineProvider() : base(typeof(DevExpress.Xpf.Editors.Industrial2of5StyleSettings)) { }
		protected override SmartTagLineViewModelFactoryList GetPropertiesImpl(FrameworkElementSmartTagPropertiesViewModel viewModel) {
			SmartTagLineViewModelFactoryList lines = base.GetPropertiesImpl(viewModel);
			return lines;
		}
	}
	sealed class IntelligentMailStyleSettingsPropertyLineProvider : PropertyLinesProviderBase {
		public IntelligentMailStyleSettingsPropertyLineProvider() : base(typeof(DevExpress.Xpf.Editors.IntelligentMailStyleSettings)) { }
		protected override SmartTagLineViewModelFactoryList GetPropertiesImpl(FrameworkElementSmartTagPropertiesViewModel viewModel) {
			SmartTagLineViewModelFactoryList lines = base.GetPropertiesImpl(viewModel);
			return lines;
		}
	}
	sealed class Interleaved2of5StyleSettingsPropertyLineProvider : PropertyLinesProviderBase {
		public Interleaved2of5StyleSettingsPropertyLineProvider() : base(typeof(DevExpress.Xpf.Editors.Interleaved2of5StyleSettings)) { }
		protected override SmartTagLineViewModelFactoryList GetPropertiesImpl(FrameworkElementSmartTagPropertiesViewModel viewModel) {
			SmartTagLineViewModelFactoryList lines = base.GetPropertiesImpl(viewModel);
			return lines;
		}
	}
	sealed class ITF14StyleSettingsPropertyLineProvider : PropertyLinesProviderBase {
		public ITF14StyleSettingsPropertyLineProvider() : base(typeof(DevExpress.Xpf.Editors.ITF14StyleSettings)) { }
		protected override SmartTagLineViewModelFactoryList GetPropertiesImpl(FrameworkElementSmartTagPropertiesViewModel viewModel) {
			SmartTagLineViewModelFactoryList lines = base.GetPropertiesImpl(viewModel);
			return lines;
		}
	}
	sealed class Matrix2of5StyleSettingsPropertyLineProvider : PropertyLinesProviderBase {
		public Matrix2of5StyleSettingsPropertyLineProvider() : base(typeof(DevExpress.Xpf.Editors.Matrix2of5StyleSettings)) { }
		protected override SmartTagLineViewModelFactoryList GetPropertiesImpl(FrameworkElementSmartTagPropertiesViewModel viewModel) {
			SmartTagLineViewModelFactoryList lines = base.GetPropertiesImpl(viewModel);
			return lines;
		}
	}
	sealed class PDF417StyleSettingsPropertyLineProvider : PropertyLinesProviderBase {
		public PDF417StyleSettingsPropertyLineProvider() : base(typeof(DevExpress.Xpf.Editors.PDF417StyleSettings)) { }
		protected override SmartTagLineViewModelFactoryList GetPropertiesImpl(FrameworkElementSmartTagPropertiesViewModel viewModel) {
			SmartTagLineViewModelFactoryList lines = base.GetPropertiesImpl(viewModel);
			lines.Add(() => new ObjectPropertyLineViewModel(viewModel, DependencyPropertyHelper.GetPropertyName(() => DevExpress.Xpf.Editors.PDF417StyleSettings.ColumnsProperty)));
			lines.Add(() => new EnumPropertyLineViewModel(viewModel, DependencyPropertyHelper.GetPropertyName(() => DevExpress.Xpf.Editors.PDF417StyleSettings.CompactionModeProperty), typeof(DevExpress.XtraPrinting.BarCode.PDF417CompactionMode)));
			lines.Add(() => new EnumPropertyLineViewModel(viewModel, DependencyPropertyHelper.GetPropertyName(() => DevExpress.Xpf.Editors.PDF417StyleSettings.ErrorCorrectionLevelProperty), typeof(DevExpress.XtraPrinting.BarCode.ErrorCorrectionLevel)));
			lines.Add(() => new ObjectPropertyLineViewModel(viewModel, DependencyPropertyHelper.GetPropertyName(() => DevExpress.Xpf.Editors.PDF417StyleSettings.RowsProperty)));
			lines.Add(() => new BooleanPropertyLineViewModel(viewModel, DependencyPropertyHelper.GetPropertyName(() => DevExpress.Xpf.Editors.PDF417StyleSettings.TruncateSymbolProperty)));
			lines.Add(() => new ObjectPropertyLineViewModel(viewModel, DependencyPropertyHelper.GetPropertyName(() => DevExpress.Xpf.Editors.PDF417StyleSettings.YToXRatioProperty)));
			return lines;
		}
	}
	sealed class PostNetStyleSettingsPropertyLineProvider : PropertyLinesProviderBase {
		public PostNetStyleSettingsPropertyLineProvider() : base(typeof(DevExpress.Xpf.Editors.PostNetStyleSettings)) { }
		protected override SmartTagLineViewModelFactoryList GetPropertiesImpl(FrameworkElementSmartTagPropertiesViewModel viewModel) {
			SmartTagLineViewModelFactoryList lines = base.GetPropertiesImpl(viewModel);
			return lines;
		}
	}
	sealed class QRCodeStyleSettingsPropertyLineProvider : PropertyLinesProviderBase {
		public QRCodeStyleSettingsPropertyLineProvider() : base(typeof(DevExpress.Xpf.Editors.QRCodeStyleSettings)) { }
		protected override SmartTagLineViewModelFactoryList GetPropertiesImpl(FrameworkElementSmartTagPropertiesViewModel viewModel) {
			SmartTagLineViewModelFactoryList lines = base.GetPropertiesImpl(viewModel);
			lines.Add(() => new EnumPropertyLineViewModel(viewModel, DependencyPropertyHelper.GetPropertyName(() => DevExpress.Xpf.Editors.QRCodeStyleSettings.CompactionModeProperty), typeof(DevExpress.XtraPrinting.BarCode.QRCodeCompactionMode)));
			lines.Add(() => new EnumPropertyLineViewModel(viewModel, DependencyPropertyHelper.GetPropertyName(() => DevExpress.Xpf.Editors.QRCodeStyleSettings.ErrorCorrectionLevelProperty), typeof(DevExpress.XtraPrinting.BarCode.QRCodeErrorCorrectionLevel)));
			lines.Add(() => new EnumPropertyLineViewModel(viewModel, DependencyPropertyHelper.GetPropertyName(() => DevExpress.Xpf.Editors.QRCodeStyleSettings.VersionProperty), typeof(DevExpress.XtraPrinting.BarCode.QRCodeVersion)));
			return lines;
		}
	}
	sealed class UPCAStyleSettingsPropertyLineProvider : PropertyLinesProviderBase {
		public UPCAStyleSettingsPropertyLineProvider() : base(typeof(DevExpress.Xpf.Editors.UPCAStyleSettings)) { }
		protected override SmartTagLineViewModelFactoryList GetPropertiesImpl(FrameworkElementSmartTagPropertiesViewModel viewModel) {
			SmartTagLineViewModelFactoryList lines = base.GetPropertiesImpl(viewModel);
			return lines;
		}
	}
	sealed class UPCE0StyleSettingsPropertyLineProvider : PropertyLinesProviderBase {
		public UPCE0StyleSettingsPropertyLineProvider() : base(typeof(DevExpress.Xpf.Editors.UPCE0StyleSettings)) { }
		protected override SmartTagLineViewModelFactoryList GetPropertiesImpl(FrameworkElementSmartTagPropertiesViewModel viewModel) {
			SmartTagLineViewModelFactoryList lines = base.GetPropertiesImpl(viewModel);
			return lines;
		}
	}
	sealed class UPCE1StyleSettingsPropertyLineProvider : PropertyLinesProviderBase {
		public UPCE1StyleSettingsPropertyLineProvider() : base(typeof(DevExpress.Xpf.Editors.UPCE1StyleSettings)) { }
		protected override SmartTagLineViewModelFactoryList GetPropertiesImpl(FrameworkElementSmartTagPropertiesViewModel viewModel) {
			SmartTagLineViewModelFactoryList lines = base.GetPropertiesImpl(viewModel);
			return lines;
		}
	}
	sealed class UPCSupplemental2StyleSettingsPropertyLineProvider : PropertyLinesProviderBase {
		public UPCSupplemental2StyleSettingsPropertyLineProvider() : base(typeof(DevExpress.Xpf.Editors.UPCSupplemental2StyleSettings)) { }
		protected override SmartTagLineViewModelFactoryList GetPropertiesImpl(FrameworkElementSmartTagPropertiesViewModel viewModel) {
			SmartTagLineViewModelFactoryList lines = base.GetPropertiesImpl(viewModel);
			return lines;
		}
	}
	sealed class UPCSupplemental5StyleSettingsPropertyLineProvider : PropertyLinesProviderBase {
		public UPCSupplemental5StyleSettingsPropertyLineProvider() : base(typeof(DevExpress.Xpf.Editors.UPCSupplemental5StyleSettings)) { }
		protected override SmartTagLineViewModelFactoryList GetPropertiesImpl(FrameworkElementSmartTagPropertiesViewModel viewModel) {
			SmartTagLineViewModelFactoryList lines = base.GetPropertiesImpl(viewModel);
			return lines;
		}
	}
	sealed class Code128StyleSettingsBaseCode128GeneratorPropertyLineProvider : PropertyLinesProviderBase {
		public Code128StyleSettingsBaseCode128GeneratorPropertyLineProvider() : base(typeof(DevExpress.Xpf.Editors.Code128BarCodeStyleSettings<DevExpress.XtraPrinting.BarCode.Code128Generator>)) { }
		protected override SmartTagLineViewModelFactoryList GetPropertiesImpl(FrameworkElementSmartTagPropertiesViewModel viewModel) {
			SmartTagLineViewModelFactoryList lines = base.GetPropertiesImpl(viewModel);
			lines.Add(() => new EnumPropertyLineViewModel(viewModel, DependencyPropertyHelper.GetPropertyName(() => DevExpress.Xpf.Editors.Code128BarCodeStyleSettings<DevExpress.XtraPrinting.BarCode.Code128Generator>.CharacterSetProperty), typeof(DevExpress.XtraPrinting.BarCode.Code128Charset)));
			return lines;
		}
	}
	sealed class Code39StyleSettingsBaseCode39ExtendedGeneratorPropertyLineProvider : PropertyLinesProviderBase {
		public Code39StyleSettingsBaseCode39ExtendedGeneratorPropertyLineProvider() : base(typeof(DevExpress.Xpf.Editors.Code39BarCodeStyleSettings<DevExpress.XtraPrinting.BarCode.Code39ExtendedGenerator>)) { }
		protected override SmartTagLineViewModelFactoryList GetPropertiesImpl(FrameworkElementSmartTagPropertiesViewModel viewModel) {
			SmartTagLineViewModelFactoryList lines = base.GetPropertiesImpl(viewModel);
			lines.Add(() => new ObjectPropertyLineViewModel(viewModel, DependencyPropertyHelper.GetPropertyName(() => DevExpress.Xpf.Editors.Code39BarCodeStyleSettings<DevExpress.XtraPrinting.BarCode.Code39ExtendedGenerator>.WideNarrowRatioProperty)));
			return lines;
		}
	}
	sealed class StyleSettingsCheckSumBaseCode39ExtendedGeneratorPropertyLineProvider : PropertyLinesProviderBase {
		public StyleSettingsCheckSumBaseCode39ExtendedGeneratorPropertyLineProvider() : base(typeof(DevExpress.Xpf.Editors.CheckSumStyleSettingsBase<DevExpress.XtraPrinting.BarCode.Code39ExtendedGenerator>)) { }
		protected override SmartTagLineViewModelFactoryList GetPropertiesImpl(FrameworkElementSmartTagPropertiesViewModel viewModel) {
			SmartTagLineViewModelFactoryList lines = base.GetPropertiesImpl(viewModel);
			lines.Add(() => new BooleanPropertyLineViewModel(viewModel, DependencyPropertyHelper.GetPropertyName(() => DevExpress.Xpf.Editors.CheckSumStyleSettingsBase<DevExpress.XtraPrinting.BarCode.Code39ExtendedGenerator>.CalcCheckSumProperty)));
			return lines;
		}
	}
	sealed class Code39StyleSettingsBaseCode39GeneratorPropertyLineProvider : PropertyLinesProviderBase {
		public Code39StyleSettingsBaseCode39GeneratorPropertyLineProvider() : base(typeof(DevExpress.Xpf.Editors.Code39BarCodeStyleSettings<DevExpress.XtraPrinting.BarCode.Code39Generator>)) { }
		protected override SmartTagLineViewModelFactoryList GetPropertiesImpl(FrameworkElementSmartTagPropertiesViewModel viewModel) {
			SmartTagLineViewModelFactoryList lines = base.GetPropertiesImpl(viewModel);
			lines.Add(() => new ObjectPropertyLineViewModel(viewModel, DependencyPropertyHelper.GetPropertyName(() => DevExpress.Xpf.Editors.Code39BarCodeStyleSettings<DevExpress.XtraPrinting.BarCode.Code39Generator>.WideNarrowRatioProperty)));
			return lines;
		}
	}
	sealed class StyleSettingsCheckSumBaseCode39GeneratorPropertyLineProvider : PropertyLinesProviderBase {
		public StyleSettingsCheckSumBaseCode39GeneratorPropertyLineProvider() : base(typeof(DevExpress.Xpf.Editors.CheckSumStyleSettingsBase<DevExpress.XtraPrinting.BarCode.Code39Generator>)) { }
		protected override SmartTagLineViewModelFactoryList GetPropertiesImpl(FrameworkElementSmartTagPropertiesViewModel viewModel) {
			SmartTagLineViewModelFactoryList lines = base.GetPropertiesImpl(viewModel);
			lines.Add(() => new BooleanPropertyLineViewModel(viewModel, DependencyPropertyHelper.GetPropertyName(() => DevExpress.Xpf.Editors.CheckSumStyleSettingsBase<DevExpress.XtraPrinting.BarCode.Code39Generator>.CalcCheckSumProperty)));
			return lines;
		}
	}
	sealed class StyleSettingsCheckSumBaseCode93ExtendedGeneratorPropertyLineProvider : PropertyLinesProviderBase {
		public StyleSettingsCheckSumBaseCode93ExtendedGeneratorPropertyLineProvider() : base(typeof(DevExpress.Xpf.Editors.CheckSumStyleSettingsBase<DevExpress.XtraPrinting.BarCode.Code93ExtendedGenerator>)) { }
		protected override SmartTagLineViewModelFactoryList GetPropertiesImpl(FrameworkElementSmartTagPropertiesViewModel viewModel) {
			SmartTagLineViewModelFactoryList lines = base.GetPropertiesImpl(viewModel);
			lines.Add(() => new BooleanPropertyLineViewModel(viewModel, DependencyPropertyHelper.GetPropertyName(() => DevExpress.Xpf.Editors.CheckSumStyleSettingsBase<DevExpress.XtraPrinting.BarCode.Code93ExtendedGenerator>.CalcCheckSumProperty)));
			return lines;
		}
	}
	sealed class StyleSettingsCheckSumBaseCode93GeneratorPropertyLineProvider : PropertyLinesProviderBase {
		public StyleSettingsCheckSumBaseCode93GeneratorPropertyLineProvider() : base(typeof(DevExpress.Xpf.Editors.CheckSumStyleSettingsBase<DevExpress.XtraPrinting.BarCode.Code93Generator>)) { }
		protected override SmartTagLineViewModelFactoryList GetPropertiesImpl(FrameworkElementSmartTagPropertiesViewModel viewModel) {
			SmartTagLineViewModelFactoryList lines = base.GetPropertiesImpl(viewModel);
			lines.Add(() => new BooleanPropertyLineViewModel(viewModel, DependencyPropertyHelper.GetPropertyName(() => DevExpress.Xpf.Editors.CheckSumStyleSettingsBase<DevExpress.XtraPrinting.BarCode.Code93Generator>.CalcCheckSumProperty)));
			return lines;
		}
	}
	sealed class Code128StyleSettingsBaseEAN128GeneratorPropertyLineProvider : PropertyLinesProviderBase {
		public Code128StyleSettingsBaseEAN128GeneratorPropertyLineProvider() : base(typeof(DevExpress.Xpf.Editors.Code128BarCodeStyleSettings<DevExpress.XtraPrinting.BarCode.EAN128Generator>)) { }
		protected override SmartTagLineViewModelFactoryList GetPropertiesImpl(FrameworkElementSmartTagPropertiesViewModel viewModel) {
			SmartTagLineViewModelFactoryList lines = base.GetPropertiesImpl(viewModel);
			lines.Add(() => new EnumPropertyLineViewModel(viewModel, DependencyPropertyHelper.GetPropertyName(() => DevExpress.Xpf.Editors.Code128BarCodeStyleSettings<DevExpress.XtraPrinting.BarCode.EAN128Generator>.CharacterSetProperty), typeof(DevExpress.XtraPrinting.BarCode.Code128Charset)));
			return lines;
		}
	}
	sealed class Industrial2of5StyleSettingsBaseIndustrial2of5GeneratorPropertyLineProvider : PropertyLinesProviderBase {
		public Industrial2of5StyleSettingsBaseIndustrial2of5GeneratorPropertyLineProvider() : base(typeof(DevExpress.Xpf.Editors.Industrial2of5BarCodeStyleSettings<DevExpress.XtraPrinting.BarCode.Industrial2of5Generator>)) { }
		protected override SmartTagLineViewModelFactoryList GetPropertiesImpl(FrameworkElementSmartTagPropertiesViewModel viewModel) {
			SmartTagLineViewModelFactoryList lines = base.GetPropertiesImpl(viewModel);
			lines.Add(() => new ObjectPropertyLineViewModel(viewModel, DependencyPropertyHelper.GetPropertyName(() => DevExpress.Xpf.Editors.Industrial2of5BarCodeStyleSettings<DevExpress.XtraPrinting.BarCode.Industrial2of5Generator>.WideNarrowRatioProperty)));
			return lines;
		}
	}
	sealed class StyleSettingsCheckSumBaseIndustrial2of5GeneratorPropertyLineProvider : PropertyLinesProviderBase {
		public StyleSettingsCheckSumBaseIndustrial2of5GeneratorPropertyLineProvider() : base(typeof(DevExpress.Xpf.Editors.CheckSumStyleSettingsBase<DevExpress.XtraPrinting.BarCode.Industrial2of5Generator>)) { }
		protected override SmartTagLineViewModelFactoryList GetPropertiesImpl(FrameworkElementSmartTagPropertiesViewModel viewModel) {
			SmartTagLineViewModelFactoryList lines = base.GetPropertiesImpl(viewModel);
			lines.Add(() => new BooleanPropertyLineViewModel(viewModel, DependencyPropertyHelper.GetPropertyName(() => DevExpress.Xpf.Editors.CheckSumStyleSettingsBase<DevExpress.XtraPrinting.BarCode.Industrial2of5Generator>.CalcCheckSumProperty)));
			return lines;
		}
	}
	sealed class Interleaved2of5StyleSettingsBaseInterleaved2of5GeneratorPropertyLineProvider : PropertyLinesProviderBase {
		public Interleaved2of5StyleSettingsBaseInterleaved2of5GeneratorPropertyLineProvider() : base(typeof(DevExpress.Xpf.Editors.Interleaved2of5BarCodeStyleSettings<DevExpress.XtraPrinting.BarCode.Interleaved2of5Generator>)) { }
		protected override SmartTagLineViewModelFactoryList GetPropertiesImpl(FrameworkElementSmartTagPropertiesViewModel viewModel) {
			SmartTagLineViewModelFactoryList lines = base.GetPropertiesImpl(viewModel);
			lines.Add(() => new ObjectPropertyLineViewModel(viewModel, DependencyPropertyHelper.GetPropertyName(() => DevExpress.Xpf.Editors.Interleaved2of5BarCodeStyleSettings<DevExpress.XtraPrinting.BarCode.Interleaved2of5Generator>.WideNarrowRatioProperty)));
			return lines;
		}
	}
	sealed class Interleaved2of5StyleSettingsBaseITF14GeneratorPropertyLineProvider : PropertyLinesProviderBase {
		public Interleaved2of5StyleSettingsBaseITF14GeneratorPropertyLineProvider() : base(typeof(DevExpress.Xpf.Editors.Interleaved2of5BarCodeStyleSettings<DevExpress.XtraPrinting.BarCode.ITF14Generator>)) { }
		protected override SmartTagLineViewModelFactoryList GetPropertiesImpl(FrameworkElementSmartTagPropertiesViewModel viewModel) {
			SmartTagLineViewModelFactoryList lines = base.GetPropertiesImpl(viewModel);
			lines.Add(() => new ObjectPropertyLineViewModel(viewModel, DependencyPropertyHelper.GetPropertyName(() => DevExpress.Xpf.Editors.Interleaved2of5BarCodeStyleSettings<DevExpress.XtraPrinting.BarCode.ITF14Generator>.WideNarrowRatioProperty)));
			return lines;
		}
	}
	sealed class Industrial2of5StyleSettingsBaseMatrix2of5GeneratorPropertyLineProvider : PropertyLinesProviderBase {
		public Industrial2of5StyleSettingsBaseMatrix2of5GeneratorPropertyLineProvider() : base(typeof(DevExpress.Xpf.Editors.Industrial2of5BarCodeStyleSettings<DevExpress.XtraPrinting.BarCode.Matrix2of5Generator>)) { }
		protected override SmartTagLineViewModelFactoryList GetPropertiesImpl(FrameworkElementSmartTagPropertiesViewModel viewModel) {
			SmartTagLineViewModelFactoryList lines = base.GetPropertiesImpl(viewModel);
			lines.Add(() => new ObjectPropertyLineViewModel(viewModel, DependencyPropertyHelper.GetPropertyName(() => DevExpress.Xpf.Editors.Industrial2of5BarCodeStyleSettings<DevExpress.XtraPrinting.BarCode.Matrix2of5Generator>.WideNarrowRatioProperty)));
			return lines;
		}
	}
	sealed class StyleSettingsCheckSumBaseMatrix2of5GeneratorPropertyLineProvider : PropertyLinesProviderBase {
		public StyleSettingsCheckSumBaseMatrix2of5GeneratorPropertyLineProvider() : base(typeof(DevExpress.Xpf.Editors.CheckSumStyleSettingsBase<DevExpress.XtraPrinting.BarCode.Matrix2of5Generator>)) { }
		protected override SmartTagLineViewModelFactoryList GetPropertiesImpl(FrameworkElementSmartTagPropertiesViewModel viewModel) {
			SmartTagLineViewModelFactoryList lines = base.GetPropertiesImpl(viewModel);
			lines.Add(() => new BooleanPropertyLineViewModel(viewModel, DependencyPropertyHelper.GetPropertyName(() => DevExpress.Xpf.Editors.CheckSumStyleSettingsBase<DevExpress.XtraPrinting.BarCode.Matrix2of5Generator>.CalcCheckSumProperty)));
			return lines;
		}
	}
	#endregion
}
