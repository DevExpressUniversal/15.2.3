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
using System.CodeDom.Compiler;
using System.Collections;
namespace DevExpress.API.Mso {
	#region IMsoObject
	[GeneratedCode("Suppress FxCop check", "")]
	public interface IMsoObject {
		object Application { get; }
		int Creator { get; }
		object Parent { get; }
	}
	#endregion
	#region MsoTriState
	[GeneratedCode("Suppress FxCop check", "")]
	public enum MsoTriState {
		msoCTrue = 1,
		msoFalse = 0,
		msoTriStateMixed = -2,
		msoTriStateToggle = -3,
		msoTrue = -1
	}
	#endregion
	#region MsoLanguageID
	[GeneratedCode("Suppress FxCop check", "")]
	public enum MsoLanguageID {
		msoLanguageIDAfrikaans = 0x436,
		msoLanguageIDAlbanian = 0x41c,
		msoLanguageIDAmharic = 0x45e,
		msoLanguageIDArabic = 0x401,
		msoLanguageIDArabicAlgeria = 0x1401,
		msoLanguageIDArabicBahrain = 0x3c01,
		msoLanguageIDArabicEgypt = 0xc01,
		msoLanguageIDArabicIraq = 0x801,
		msoLanguageIDArabicJordan = 0x2c01,
		msoLanguageIDArabicKuwait = 0x3401,
		msoLanguageIDArabicLebanon = 0x3001,
		msoLanguageIDArabicLibya = 0x1001,
		msoLanguageIDArabicMorocco = 0x1801,
		msoLanguageIDArabicOman = 0x2001,
		msoLanguageIDArabicQatar = 0x4001,
		msoLanguageIDArabicSyria = 0x2801,
		msoLanguageIDArabicTunisia = 0x1c01,
		msoLanguageIDArabicUAE = 0x3801,
		msoLanguageIDArabicYemen = 0x2401,
		msoLanguageIDArmenian = 0x42b,
		msoLanguageIDAssamese = 0x44d,
		msoLanguageIDAzeriCyrillic = 0x82c,
		msoLanguageIDAzeriLatin = 0x42c,
		msoLanguageIDBasque = 0x42d,
		msoLanguageIDBelgianDutch = 0x813,
		msoLanguageIDBelgianFrench = 0x80c,
		msoLanguageIDBengali = 0x445,
		msoLanguageIDBosnian = 0x101a,
		msoLanguageIDBosnianBosniaHerzegovinaCyrillic = 0x201a,
		msoLanguageIDBosnianBosniaHerzegovinaLatin = 0x141a,
		msoLanguageIDBrazilianPortuguese = 0x416,
		msoLanguageIDBulgarian = 0x402,
		msoLanguageIDBurmese = 0x455,
		msoLanguageIDByelorussian = 0x423,
		msoLanguageIDCatalan = 0x403,
		msoLanguageIDCherokee = 0x45c,
		msoLanguageIDChineseHongKongSAR = 0xc04,
		msoLanguageIDChineseMacaoSAR = 0x1404,
		msoLanguageIDChineseSingapore = 0x1004,
		msoLanguageIDCroatian = 0x41a,
		msoLanguageIDCzech = 0x405,
		msoLanguageIDDanish = 0x406,
		msoLanguageIDDivehi = 0x465,
		msoLanguageIDDutch = 0x413,
		msoLanguageIDDzongkhaBhutan = 0x851,
		msoLanguageIDEdo = 0x466,
		msoLanguageIDEnglishAUS = 0xc09,
		msoLanguageIDEnglishBelize = 0x2809,
		msoLanguageIDEnglishCanadian = 0x1009,
		msoLanguageIDEnglishCaribbean = 0x2409,
		msoLanguageIDEnglishIndonesia = 0x3809,
		msoLanguageIDEnglishIreland = 0x1809,
		msoLanguageIDEnglishJamaica = 0x2009,
		msoLanguageIDEnglishNewZealand = 0x1409,
		msoLanguageIDEnglishPhilippines = 0x3409,
		msoLanguageIDEnglishSouthAfrica = 0x1c09,
		msoLanguageIDEnglishTrinidadTobago = 0x2c09,
		msoLanguageIDEnglishUK = 0x809,
		msoLanguageIDEnglishUS = 0x409,
		msoLanguageIDEnglishZimbabwe = 0x3009,
		msoLanguageIDEstonian = 0x425,
		msoLanguageIDFaeroese = 0x438,
		msoLanguageIDFarsi = 0x429,
		msoLanguageIDFilipino = 0x464,
		msoLanguageIDFinnish = 0x40b,
		msoLanguageIDFrench = 0x40c,
		msoLanguageIDFrenchCameroon = 0x2c0c,
		msoLanguageIDFrenchCanadian = 0xc0c,
		msoLanguageIDFrenchCongoDRC = 0x240c,
		msoLanguageIDFrenchCotedIvoire = 0x300c,
		msoLanguageIDFrenchHaiti = 0x3c0c,
		msoLanguageIDFrenchLuxembourg = 0x140c,
		msoLanguageIDFrenchMali = 0x340c,
		msoLanguageIDFrenchMonaco = 0x180c,
		msoLanguageIDFrenchMorocco = 0x380c,
		msoLanguageIDFrenchReunion = 0x200c,
		msoLanguageIDFrenchSenegal = 0x280c,
		msoLanguageIDFrenchWestIndies = 0x1c0c,
		msoLanguageIDFrenchZaire = 0x240c,
		msoLanguageIDFrisianNetherlands = 0x462,
		msoLanguageIDFulfulde = 0x467,
		msoLanguageIDGaelicIreland = 0x83c,
		msoLanguageIDGaelicScotland = 0x43c,
		msoLanguageIDGalician = 0x456,
		msoLanguageIDGeorgian = 0x437,
		msoLanguageIDGerman = 0x407,
		msoLanguageIDGermanAustria = 0xc07,
		msoLanguageIDGermanLiechtenstein = 0x1407,
		msoLanguageIDGermanLuxembourg = 0x1007,
		msoLanguageIDGreek = 0x408,
		msoLanguageIDGuarani = 0x474,
		msoLanguageIDGujarati = 0x447,
		msoLanguageIDHausa = 0x468,
		msoLanguageIDHawaiian = 0x475,
		msoLanguageIDHebrew = 0x40d,
		msoLanguageIDHindi = 0x439,
		msoLanguageIDHungarian = 0x40e,
		msoLanguageIDIbibio = 0x469,
		msoLanguageIDIcelandic = 0x40f,
		msoLanguageIDIgbo = 0x470,
		msoLanguageIDIndonesian = 0x421,
		msoLanguageIDInuktitut = 0x45d,
		msoLanguageIDItalian = 0x410,
		msoLanguageIDJapanese = 0x411,
		msoLanguageIDKannada = 0x44b,
		msoLanguageIDKanuri = 0x471,
		msoLanguageIDKashmiri = 0x460,
		msoLanguageIDKashmiriDevanagari = 0x860,
		msoLanguageIDKazakh = 0x43f,
		msoLanguageIDKhmer = 0x453,
		msoLanguageIDKirghiz = 0x440,
		msoLanguageIDKonkani = 0x457,
		msoLanguageIDKorean = 0x412,
		msoLanguageIDKyrgyz = 0x440,
		msoLanguageIDLao = 0x454,
		msoLanguageIDLatin = 0x476,
		msoLanguageIDLatvian = 0x426,
		msoLanguageIDLithuanian = 0x427,
		msoLanguageIDMacedonian = 0x42f,
		msoLanguageIDMacedonianFYROM = 0x42f,
		msoLanguageIDMalayalam = 0x44c,
		msoLanguageIDMalayBruneiDarussalam = 0x83e,
		msoLanguageIDMalaysian = 0x43e,
		msoLanguageIDMaltese = 0x43a,
		msoLanguageIDManipuri = 0x458,
		msoLanguageIDMaori = 0x481,
		msoLanguageIDMarathi = 0x44e,
		msoLanguageIDMexicanSpanish = 0x80a,
		msoLanguageIDMixed = -2,
		msoLanguageIDMongolian = 0x450,
		msoLanguageIDNepali = 0x461,
		msoLanguageIDNone = 0,
		msoLanguageIDNoProofing = 0x400,
		msoLanguageIDNorwegianBokmol = 0x414,
		msoLanguageIDNorwegianNynorsk = 0x814,
		msoLanguageIDOriya = 0x448,
		msoLanguageIDOromo = 0x472,
		msoLanguageIDPashto = 0x463,
		msoLanguageIDPolish = 0x415,
		msoLanguageIDPortuguese = 0x816,
		msoLanguageIDPunjabi = 0x446,
		msoLanguageIDQuechuaBolivia = 0x46b,
		msoLanguageIDQuechuaEcuador = 0x86b,
		msoLanguageIDQuechuaPeru = 0xc6b,
		msoLanguageIDRhaetoRomanic = 0x417,
		msoLanguageIDRomanian = 0x418,
		msoLanguageIDRomanianMoldova = 0x818,
		msoLanguageIDRussian = 0x419,
		msoLanguageIDRussianMoldova = 0x819,
		msoLanguageIDSamiLappish = 0x43b,
		msoLanguageIDSanskrit = 0x44f,
		msoLanguageIDSepedi = 0x46c,
		msoLanguageIDSerbianBosniaHerzegovinaCyrillic = 0x1c1a,
		msoLanguageIDSerbianBosniaHerzegovinaLatin = 0x181a,
		msoLanguageIDSerbianCyrillic = 0xc1a,
		msoLanguageIDSerbianLatin = 0x81a,
		msoLanguageIDSesotho = 0x430,
		msoLanguageIDSimplifiedChinese = 0x804,
		msoLanguageIDSindhi = 0x459,
		msoLanguageIDSindhiPakistan = 0x859,
		msoLanguageIDSinhalese = 0x45b,
		msoLanguageIDSlovak = 0x41b,
		msoLanguageIDSlovenian = 0x424,
		msoLanguageIDSomali = 0x477,
		msoLanguageIDSorbian = 0x42e,
		msoLanguageIDSpanish = 0x40a,
		msoLanguageIDSpanishArgentina = 0x2c0a,
		msoLanguageIDSpanishBolivia = 0x400a,
		msoLanguageIDSpanishChile = 0x340a,
		msoLanguageIDSpanishColombia = 0x240a,
		msoLanguageIDSpanishCostaRica = 0x140a,
		msoLanguageIDSpanishDominicanRepublic = 0x1c0a,
		msoLanguageIDSpanishEcuador = 0x300a,
		msoLanguageIDSpanishElSalvador = 0x440a,
		msoLanguageIDSpanishGuatemala = 0x100a,
		msoLanguageIDSpanishHonduras = 0x480a,
		msoLanguageIDSpanishModernSort = 0xc0a,
		msoLanguageIDSpanishNicaragua = 0x4c0a,
		msoLanguageIDSpanishPanama = 0x180a,
		msoLanguageIDSpanishParaguay = 0x3c0a,
		msoLanguageIDSpanishPeru = 0x280a,
		msoLanguageIDSpanishPuertoRico = 0x500a,
		msoLanguageIDSpanishUruguay = 0x380a,
		msoLanguageIDSpanishVenezuela = 0x200a,
		msoLanguageIDSutu = 0x430,
		msoLanguageIDSwahili = 0x441,
		msoLanguageIDSwedish = 0x41d,
		msoLanguageIDSwedishFinland = 0x81d,
		msoLanguageIDSwissFrench = 0x100c,
		msoLanguageIDSwissGerman = 0x807,
		msoLanguageIDSwissItalian = 0x810,
		msoLanguageIDSyriac = 0x45a,
		msoLanguageIDTajik = 0x428,
		msoLanguageIDTamazight = 0x45f,
		msoLanguageIDTamazightLatin = 0x85f,
		msoLanguageIDTamil = 0x449,
		msoLanguageIDTatar = 0x444,
		msoLanguageIDTelugu = 0x44a,
		msoLanguageIDThai = 0x41e,
		msoLanguageIDTibetan = 0x451,
		msoLanguageIDTigrignaEritrea = 0x873,
		msoLanguageIDTigrignaEthiopic = 0x473,
		msoLanguageIDTraditionalChinese = 0x404,
		msoLanguageIDTsonga = 0x431,
		msoLanguageIDTswana = 0x432,
		msoLanguageIDTurkish = 0x41f,
		msoLanguageIDTurkmen = 0x442,
		msoLanguageIDUkrainian = 0x422,
		msoLanguageIDUrdu = 0x420,
		msoLanguageIDUzbekCyrillic = 0x843,
		msoLanguageIDUzbekLatin = 0x443,
		msoLanguageIDVenda = 0x433,
		msoLanguageIDVietnamese = 0x42a,
		msoLanguageIDWelsh = 0x452,
		msoLanguageIDXhosa = 0x434,
		msoLanguageIDYi = 0x478,
		msoLanguageIDYiddish = 0x43d,
		msoLanguageIDYoruba = 0x46a,
		msoLanguageIDZulu = 0x435
	}
	#endregion
	#region MsoFeatureInstall
	[GeneratedCode("Suppress FxCop check", "")]
	public enum MsoFeatureInstall {
		msoFeatureInstallNone,
		msoFeatureInstallOnDemand,
		msoFeatureInstallOnDemandWithUI
	}
	#endregion
	#region MsoAutomationSecurity
	[GeneratedCode("Suppress FxCop check", "")]
	public enum MsoAutomationSecurity {
		msoAutomationSecurityByUI = 2,
		msoAutomationSecurityForceDisable = 3,
		msoAutomationSecurityLow = 1
	}
	#endregion
	#region MsoFileDialogType
	[GeneratedCode("Suppress FxCop check", "")]
	public enum MsoFileDialogType {
		msoFileDialogFilePicker = 3,
		msoFileDialogFolderPicker = 4,
		msoFileDialogOpen = 1,
		msoFileDialogSaveAs = 2
	}
	#endregion
	#region MsoFlipCmd
	[GeneratedCode("Suppress FxCop check", "")]
	public enum MsoFlipCmd {
		msoFlipHorizontal,
		msoFlipVertical
	}
	#endregion
	#region MsoScaleFrom
	[GeneratedCode("Suppress FxCop check", "")]
	public enum MsoScaleFrom {
		msoScaleFromTopLeft,
		msoScaleFromMiddle,
		msoScaleFromBottomRight
	}
	#endregion
	#region MsoZOrderCmd
	[GeneratedCode("Suppress FxCop check", "")]
	public enum MsoZOrderCmd {
		msoBringToFront,
		msoSendToBack,
		msoBringForward,
		msoSendBackward,
		msoBringInFrontOfText,
		msoSendBehindText
	}
	#endregion
	#region MsoShapeType
	[GeneratedCode("Suppress FxCop check", "")]
	public enum MsoShapeType {
		msoAutoShape = 1,
		msoCallout = 2,
		msoCanvas = 20,
		msoChart = 3,
		msoComment = 4,
		msoDiagram = 0x15,
		msoEmbeddedOLEObject = 7,
		msoFormControl = 8,
		msoFreeform = 5,
		msoGroup = 6,
		msoInk = 0x16,
		msoInkComment = 0x17,
		msoLine = 9,
		msoLinkedOLEObject = 10,
		msoLinkedPicture = 11,
		msoMedia = 0x10,
		msoOLEControlObject = 12,
		msoPicture = 13,
		msoPlaceholder = 14,
		msoScriptAnchor = 0x12,
		msoShapeTypeMixed = -2,
		msoSmartArt = 0x18,
		msoTable = 0x13,
		msoTextBox = 0x11,
		msoTextEffect = 15
	}
	#endregion
	#region MsoAutoShapeType
	[GeneratedCode("Suppress FxCop check", "")]
	public enum MsoAutoShapeType {
		msoShape10pointStar = 0x95,
		msoShape12pointStar = 150,
		msoShape16pointStar = 0x5e,
		msoShape24pointStar = 0x5f,
		msoShape32pointStar = 0x60,
		msoShape4pointStar = 0x5b,
		msoShape5pointStar = 0x5c,
		msoShape6pointStar = 0x93,
		msoShape7pointStar = 0x94,
		msoShape8pointStar = 0x5d,
		msoShapeActionButtonBackorPrevious = 0x81,
		msoShapeActionButtonBeginning = 0x83,
		msoShapeActionButtonCustom = 0x7d,
		msoShapeActionButtonDocument = 0x86,
		msoShapeActionButtonEnd = 0x84,
		msoShapeActionButtonForwardorNext = 130,
		msoShapeActionButtonHelp = 0x7f,
		msoShapeActionButtonHome = 0x7e,
		msoShapeActionButtonInformation = 0x80,
		msoShapeActionButtonMovie = 0x88,
		msoShapeActionButtonReturn = 0x85,
		msoShapeActionButtonSound = 0x87,
		msoShapeArc = 0x19,
		msoShapeBalloon = 0x89,
		msoShapeBentArrow = 0x29,
		msoShapeBentUpArrow = 0x2c,
		msoShapeBevel = 15,
		msoShapeBlockArc = 20,
		msoShapeCan = 13,
		msoShapeChartPlus = 0xb6,
		msoShapeChartStar = 0xb5,
		msoShapeChartX = 180,
		msoShapeChevron = 0x34,
		msoShapeChord = 0xa1,
		msoShapeCircularArrow = 60,
		msoShapeCloud = 0xb3,
		msoShapeCloudCallout = 0x6c,
		msoShapeCorner = 0xa2,
		msoShapeCornerTabs = 0xa9,
		msoShapeCross = 11,
		msoShapeCube = 14,
		msoShapeCurvedDownArrow = 0x30,
		msoShapeCurvedDownRibbon = 100,
		msoShapeCurvedLeftArrow = 0x2e,
		msoShapeCurvedRightArrow = 0x2d,
		msoShapeCurvedUpArrow = 0x2f,
		msoShapeCurvedUpRibbon = 0x63,
		msoShapeDecagon = 0x90,
		msoShapeDiagonalStripe = 0x8d,
		msoShapeDiamond = 4,
		msoShapeDodecagon = 0x92,
		msoShapeDonut = 0x12,
		msoShapeDoubleBrace = 0x1b,
		msoShapeDoubleBracket = 0x1a,
		msoShapeDoubleWave = 0x68,
		msoShapeDownArrow = 0x24,
		msoShapeDownArrowCallout = 0x38,
		msoShapeDownRibbon = 0x62,
		msoShapeExplosion1 = 0x59,
		msoShapeExplosion2 = 90,
		msoShapeFlowchartAlternateProcess = 0x3e,
		msoShapeFlowchartCard = 0x4b,
		msoShapeFlowchartCollate = 0x4f,
		msoShapeFlowchartConnector = 0x49,
		msoShapeFlowchartData = 0x40,
		msoShapeFlowchartDecision = 0x3f,
		msoShapeFlowchartDelay = 0x54,
		msoShapeFlowchartDirectAccessStorage = 0x57,
		msoShapeFlowchartDisplay = 0x58,
		msoShapeFlowchartDocument = 0x43,
		msoShapeFlowchartExtract = 0x51,
		msoShapeFlowchartInternalStorage = 0x42,
		msoShapeFlowchartMagneticDisk = 0x56,
		msoShapeFlowchartManualInput = 0x47,
		msoShapeFlowchartManualOperation = 0x48,
		msoShapeFlowchartMerge = 0x52,
		msoShapeFlowchartMultidocument = 0x44,
		msoShapeFlowchartOfflineStorage = 0x8b,
		msoShapeFlowchartOffpageConnector = 0x4a,
		msoShapeFlowchartOr = 0x4e,
		msoShapeFlowchartPredefinedProcess = 0x41,
		msoShapeFlowchartPreparation = 70,
		msoShapeFlowchartProcess = 0x3d,
		msoShapeFlowchartPunchedTape = 0x4c,
		msoShapeFlowchartSequentialAccessStorage = 0x55,
		msoShapeFlowchartSort = 80,
		msoShapeFlowchartStoredData = 0x53,
		msoShapeFlowchartSummingJunction = 0x4d,
		msoShapeFlowchartTerminator = 0x45,
		msoShapeFoldedCorner = 0x10,
		msoShapeFrame = 0x9e,
		msoShapeFunnel = 0xae,
		msoShapeGear6 = 0xac,
		msoShapeGear9 = 0xad,
		msoShapeHalfFrame = 0x9f,
		msoShapeHeart = 0x15,
		msoShapeHeptagon = 0x91,
		msoShapeHexagon = 10,
		msoShapeHorizontalScroll = 0x66,
		msoShapeIsoscelesTriangle = 7,
		msoShapeLeftArrow = 0x22,
		msoShapeLeftArrowCallout = 0x36,
		msoShapeLeftBrace = 0x1f,
		msoShapeLeftBracket = 0x1d,
		msoShapeLeftCircularArrow = 0xb0,
		msoShapeLeftRightArrow = 0x25,
		msoShapeLeftRightArrowCallout = 0x39,
		msoShapeLeftRightCircularArrow = 0xb1,
		msoShapeLeftRightRibbon = 140,
		msoShapeLeftRightUpArrow = 40,
		msoShapeLeftUpArrow = 0x2b,
		msoShapeLightningBolt = 0x16,
		msoShapeLineCallout1 = 0x6d,
		msoShapeLineCallout1AccentBar = 0x71,
		msoShapeLineCallout1BorderandAccentBar = 0x79,
		msoShapeLineCallout1NoBorder = 0x75,
		msoShapeLineCallout2 = 110,
		msoShapeLineCallout2AccentBar = 0x72,
		msoShapeLineCallout2BorderandAccentBar = 0x7a,
		msoShapeLineCallout2NoBorder = 0x76,
		msoShapeLineCallout3 = 0x6f,
		msoShapeLineCallout3AccentBar = 0x73,
		msoShapeLineCallout3BorderandAccentBar = 0x7b,
		msoShapeLineCallout3NoBorder = 0x77,
		msoShapeLineCallout4 = 0x70,
		msoShapeLineCallout4AccentBar = 0x74,
		msoShapeLineCallout4BorderandAccentBar = 0x7c,
		msoShapeLineCallout4NoBorder = 120,
		msoShapeLineInverse = 0xb7,
		msoShapeMathDivide = 0xa6,
		msoShapeMathEqual = 0xa7,
		msoShapeMathMinus = 0xa4,
		msoShapeMathMultiply = 0xa5,
		msoShapeMathNotEqual = 0xa8,
		msoShapeMathPlus = 0xa3,
		msoShapeMixed = -2,
		msoShapeMoon = 0x18,
		msoShapeNonIsoscelesTrapezoid = 0x8f,
		msoShapeNoSymbol = 0x13,
		msoShapeNotchedRightArrow = 50,
		msoShapeNotPrimitive = 0x8a,
		msoShapeOctagon = 6,
		msoShapeOval = 9,
		msoShapeOvalCallout = 0x6b,
		msoShapeParallelogram = 2,
		msoShapePentagon = 0x33,
		msoShapePie = 0x8e,
		msoShapePieWedge = 0xaf,
		msoShapePlaque = 0x1c,
		msoShapePlaqueTabs = 0xab,
		msoShapeQuadArrow = 0x27,
		msoShapeQuadArrowCallout = 0x3b,
		msoShapeRectangle = 1,
		msoShapeRectangularCallout = 0x69,
		msoShapeRegularPentagon = 12,
		msoShapeRightArrow = 0x21,
		msoShapeRightArrowCallout = 0x35,
		msoShapeRightBrace = 0x20,
		msoShapeRightBracket = 30,
		msoShapeRightTriangle = 8,
		msoShapeRound1Rectangle = 0x97,
		msoShapeRound2DiagRectangle = 0x99,
		msoShapeRound2SameRectangle = 0x98,
		msoShapeRoundedRectangle = 5,
		msoShapeRoundedRectangularCallout = 0x6a,
		msoShapeSmileyFace = 0x11,
		msoShapeSnip1Rectangle = 0x9b,
		msoShapeSnip2DiagRectangle = 0x9d,
		msoShapeSnip2SameRectangle = 0x9c,
		msoShapeSnipRoundRectangle = 0x9a,
		msoShapeSquareTabs = 170,
		msoShapeStripedRightArrow = 0x31,
		msoShapeSun = 0x17,
		msoShapeSwooshArrow = 0xb2,
		msoShapeTear = 160,
		msoShapeTrapezoid = 3,
		msoShapeUpArrow = 0x23,
		msoShapeUpArrowCallout = 0x37,
		msoShapeUpDownArrow = 0x26,
		msoShapeUpDownArrowCallout = 0x3a,
		msoShapeUpRibbon = 0x61,
		msoShapeUTurnArrow = 0x2a,
		msoShapeVerticalScroll = 0x65,
		msoShapeWave = 0x67
	}
	#endregion
	#region MsoConnectorType
	[GeneratedCode("Suppress FxCop check", "")]
	public enum MsoConnectorType {
		msoConnectorCurve = 3,
		msoConnectorElbow = 2,
		msoConnectorStraight = 1,
		msoConnectorTypeMixed = -2
	}
	#endregion
	#region MsoCalloutType
	[GeneratedCode("Suppress FxCop check", "")]
	public enum MsoCalloutType {
		msoCalloutFour = 4,
		msoCalloutMixed = -2,
		msoCalloutOne = 1,
		msoCalloutThree = 3,
		msoCalloutTwo = 2
	}
	#endregion
	#region MsoPresetTextEffect
	[GeneratedCode("Suppress FxCop check", "")]
	public enum MsoPresetTextEffect {
		msoTextEffect1 = 0,
		msoTextEffect10 = 9,
		msoTextEffect11 = 10,
		msoTextEffect12 = 11,
		msoTextEffect13 = 12,
		msoTextEffect14 = 13,
		msoTextEffect15 = 14,
		msoTextEffect16 = 15,
		msoTextEffect17 = 0x10,
		msoTextEffect18 = 0x11,
		msoTextEffect19 = 0x12,
		msoTextEffect2 = 1,
		msoTextEffect20 = 0x13,
		msoTextEffect21 = 20,
		msoTextEffect22 = 0x15,
		msoTextEffect23 = 0x16,
		msoTextEffect24 = 0x17,
		msoTextEffect25 = 0x18,
		msoTextEffect26 = 0x19,
		msoTextEffect27 = 0x1a,
		msoTextEffect28 = 0x1b,
		msoTextEffect29 = 0x1c,
		msoTextEffect3 = 2,
		msoTextEffect30 = 0x1d,
		msoTextEffect4 = 3,
		msoTextEffect5 = 4,
		msoTextEffect6 = 5,
		msoTextEffect7 = 6,
		msoTextEffect8 = 7,
		msoTextEffect9 = 8,
		msoTextEffectMixed = -2
	}
	#endregion
	#region MsoTextOrientation
	[GeneratedCode("Suppress FxCop check", "")]
	public enum MsoTextOrientation {
		msoTextOrientationDownward = 3,
		msoTextOrientationHorizontal = 1,
		msoTextOrientationHorizontalRotatedFarEast = 6,
		msoTextOrientationMixed = -2,
		msoTextOrientationUpward = 2,
		msoTextOrientationVertical = 5,
		msoTextOrientationVerticalFarEast = 4
	}
	#endregion
	#region MsoEditingType
	[GeneratedCode("Suppress FxCop check", "")]
	public enum MsoEditingType {
		msoEditingAuto,
		msoEditingCorner,
		msoEditingSmooth,
		msoEditingSymmetric
	}
	#endregion
	#region MsoDiagramType
	[GeneratedCode("Suppress FxCop check", "")]
	public enum MsoDiagramType {
		msoDiagramCycle = 2,
		msoDiagramMixed = -2,
		msoDiagramOrgChart = 1,
		msoDiagramPyramid = 4,
		msoDiagramRadial = 3,
		msoDiagramTarget = 6,
		msoDiagramVenn = 5
	}
	#endregion
	#region MsoSyncEventType
	[GeneratedCode("Suppress FxCop check", "")]
	public enum MsoSyncEventType {
		msoSyncEventDownloadInitiated,
		msoSyncEventDownloadSucceeded,
		msoSyncEventDownloadFailed,
		msoSyncEventUploadInitiated,
		msoSyncEventUploadSucceeded,
		msoSyncEventUploadFailed,
		msoSyncEventDownloadNoChange,
		msoSyncEventOffline
	}
	#endregion
	#region MsoEncoding
	[GeneratedCode("Suppress FxCop check", "")]
	public enum MsoEncoding {
		msoEncodingArabic = 0x4e8,
		msoEncodingArabicASMO = 0x2c4,
		msoEncodingArabicAutoDetect = 0xc838,
		msoEncodingArabicTransparentASMO = 720,
		msoEncodingAutoDetect = 0xc351,
		msoEncodingBaltic = 0x4e9,
		msoEncodingCentralEuropean = 0x4e2,
		msoEncodingCyrillic = 0x4e3,
		msoEncodingCyrillicAutoDetect = 0xc833,
		msoEncodingEBCDICArabic = 0x4fc4,
		msoEncodingEBCDICDenmarkNorway = 0x4f35,
		msoEncodingEBCDICFinlandSweden = 0x4f36,
		msoEncodingEBCDICFrance = 0x4f49,
		msoEncodingEBCDICGermany = 0x4f31,
		msoEncodingEBCDICGreek = 0x4fc7,
		msoEncodingEBCDICGreekModern = 0x36b,
		msoEncodingEBCDICHebrew = 0x4fc8,
		msoEncodingEBCDICIcelandic = 0x5187,
		msoEncodingEBCDICInternational = 500,
		msoEncodingEBCDICItaly = 0x4f38,
		msoEncodingEBCDICJapaneseKatakanaExtended = 0x4f42,
		msoEncodingEBCDICJapaneseKatakanaExtendedAndJapanese = 0xc6f2,
		msoEncodingEBCDICJapaneseLatinExtendedAndJapanese = 0xc6fb,
		msoEncodingEBCDICKoreanExtended = 0x5161,
		msoEncodingEBCDICKoreanExtendedAndKorean = 0xc6f5,
		msoEncodingEBCDICLatinAmericaSpain = 0x4f3c,
		msoEncodingEBCDICMultilingualROECELatin2 = 870,
		msoEncodingEBCDICRussian = 0x5190,
		msoEncodingEBCDICSerbianBulgarian = 0x5221,
		msoEncodingEBCDICSimplifiedChineseExtendedAndSimplifiedChinese = 0xc6f7,
		msoEncodingEBCDICThai = 0x5166,
		msoEncodingEBCDICTurkish = 0x51a9,
		msoEncodingEBCDICTurkishLatin5 = 0x402,
		msoEncodingEBCDICUnitedKingdom = 0x4f3d,
		msoEncodingEBCDICUSCanada = 0x25,
		msoEncodingEBCDICUSCanadaAndJapanese = 0xc6f3,
		msoEncodingEBCDICUSCanadaAndTraditionalChinese = 0xc6f9,
		msoEncodingEUCChineseSimplifiedChinese = 0xcae0,
		msoEncodingEUCJapanese = 0xcadc,
		msoEncodingEUCKorean = 0xcaed,
		msoEncodingEUCTaiwaneseTraditionalChinese = 0xcaee,
		msoEncodingEuropa3 = 0x7149,
		msoEncodingExtAlphaLowercase = 0x5223,
		msoEncodingGreek = 0x4e5,
		msoEncodingGreekAutoDetect = 0xc835,
		msoEncodingHebrew = 0x4e7,
		msoEncodingHZGBSimplifiedChinese = 0xcec8,
		msoEncodingIA5German = 0x4e8a,
		msoEncodingIA5IRV = 0x4e89,
		msoEncodingIA5Norwegian = 0x4e8c,
		msoEncodingIA5Swedish = 0x4e8b,
		msoEncodingISCIIAssamese = 0xdeae,
		msoEncodingISCIIBengali = 0xdeab,
		msoEncodingISCIIDevanagari = 0xdeaa,
		msoEncodingISCIIGujarati = 0xdeb2,
		msoEncodingISCIIKannada = 0xdeb0,
		msoEncodingISCIIMalayalam = 0xdeb1,
		msoEncodingISCIIOriya = 0xdeaf,
		msoEncodingISCIIPunjabi = 0xdeb3,
		msoEncodingISCIITamil = 0xdeac,
		msoEncodingISCIITelugu = 0xdead,
		msoEncodingISO2022CNSimplifiedChinese = 0xc435,
		msoEncodingISO2022CNTraditionalChinese = 0xc433,
		msoEncodingISO2022JPJISX02011989 = 0xc42e,
		msoEncodingISO2022JPJISX02021984 = 0xc42d,
		msoEncodingISO2022JPNoHalfwidthKatakana = 0xc42c,
		msoEncodingISO2022KR = 0xc431,
		msoEncodingISO6937NonSpacingAccent = 0x4f2d,
		msoEncodingISO885915Latin9 = 0x6fbd,
		msoEncodingISO88591Latin1 = 0x6faf,
		msoEncodingISO88592CentralEurope = 0x6fb0,
		msoEncodingISO88593Latin3 = 0x6fb1,
		msoEncodingISO88594Baltic = 0x6fb2,
		msoEncodingISO88595Cyrillic = 0x6fb3,
		msoEncodingISO88596Arabic = 0x6fb4,
		msoEncodingISO88597Greek = 0x6fb5,
		msoEncodingISO88598Hebrew = 0x6fb6,
		msoEncodingISO88598HebrewLogical = 0x96c6,
		msoEncodingISO88599Turkish = 0x6fb7,
		msoEncodingJapaneseAutoDetect = 0xc6f4,
		msoEncodingJapaneseShiftJIS = 0x3a4,
		msoEncodingKOI8R = 0x5182,
		msoEncodingKOI8U = 0x556a,
		msoEncodingKorean = 0x3b5,
		msoEncodingKoreanAutoDetect = 0xc705,
		msoEncodingKoreanJohab = 0x551,
		msoEncodingMacArabic = 0x2714,
		msoEncodingMacCroatia = 0x2762,
		msoEncodingMacCyrillic = 0x2717,
		msoEncodingMacGreek1 = 0x2716,
		msoEncodingMacHebrew = 0x2715,
		msoEncodingMacIcelandic = 0x275f,
		msoEncodingMacJapanese = 0x2711,
		msoEncodingMacKorean = 0x2713,
		msoEncodingMacLatin2 = 0x272d,
		msoEncodingMacRoman = 0x2710,
		msoEncodingMacRomania = 0x271a,
		msoEncodingMacSimplifiedChineseGB2312 = 0x2718,
		msoEncodingMacTraditionalChineseBig5 = 0x2712,
		msoEncodingMacTurkish = 0x2761,
		msoEncodingMacUkraine = 0x2721,
		msoEncodingOEMArabic = 0x360,
		msoEncodingOEMBaltic = 0x307,
		msoEncodingOEMCanadianFrench = 0x35f,
		msoEncodingOEMCyrillic = 0x357,
		msoEncodingOEMCyrillicII = 0x362,
		msoEncodingOEMGreek437G = 0x2e1,
		msoEncodingOEMHebrew = 0x35e,
		msoEncodingOEMIcelandic = 0x35d,
		msoEncodingOEMModernGreek = 0x365,
		msoEncodingOEMMultilingualLatinI = 850,
		msoEncodingOEMMultilingualLatinII = 0x354,
		msoEncodingOEMNordic = 0x361,
		msoEncodingOEMPortuguese = 860,
		msoEncodingOEMTurkish = 0x359,
		msoEncodingOEMUnitedStates = 0x1b5,
		msoEncodingSimplifiedChineseAutoDetect = 0xc6f8,
		msoEncodingSimplifiedChineseGB18030 = 0xd698,
		msoEncodingSimplifiedChineseGBK = 0x3a8,
		msoEncodingT61 = 0x4f25,
		msoEncodingTaiwanCNS = 0x4e20,
		msoEncodingTaiwanEten = 0x4e22,
		msoEncodingTaiwanIBM5550 = 0x4e23,
		msoEncodingTaiwanTCA = 0x4e21,
		msoEncodingTaiwanTeleText = 0x4e24,
		msoEncodingTaiwanWang = 0x4e25,
		msoEncodingThai = 0x36a,
		msoEncodingTraditionalChineseAutoDetect = 0xc706,
		msoEncodingTraditionalChineseBig5 = 950,
		msoEncodingTurkish = 0x4e6,
		msoEncodingUnicodeBigEndian = 0x4b1,
		msoEncodingUnicodeLittleEndian = 0x4b0,
		msoEncodingUSASCII = 0x4e9f,
		msoEncodingUTF7 = 0xfde8,
		msoEncodingUTF8 = 0xfde9,
		msoEncodingVietnamese = 0x4ea,
		msoEncodingWestern = 0x4e4
	}
	#endregion
	#region MsoAlignCmd
	[GeneratedCode("Suppress FxCop check", "")]
	public enum MsoAlignCmd {
		msoAlignLefts,
		msoAlignCenters,
		msoAlignRights,
		msoAlignTops,
		msoAlignMiddles,
		msoAlignBottoms
	}
	#endregion
	#region MsoDistributeCmd
	[GeneratedCode("Suppress FxCop check", "")]
	public enum MsoDistributeCmd {
		msoDistributeHorizontally,
		msoDistributeVertically
	}
	#endregion
	#region MsoArrowheadLength
	[GeneratedCode("Suppress FxCop check", "")]
	public enum MsoArrowheadLength {
		msoArrowheadLengthMedium = 2,
		msoArrowheadLengthMixed = -2,
		msoArrowheadLong = 3,
		msoArrowheadShort = 1
	}
	#endregion
	#region MsoArrowheadStyle
	[GeneratedCode("Suppress FxCop check", "")]
	public enum MsoArrowheadStyle {
		msoArrowheadDiamond = 5,
		msoArrowheadNone = 1,
		msoArrowheadOpen = 3,
		msoArrowheadOval = 6,
		msoArrowheadStealth = 4,
		msoArrowheadStyleMixed = -2,
		msoArrowheadTriangle = 2
	}
	#endregion
	#region MsoArrowheadWidth
	[GeneratedCode("Suppress FxCop check", "")]
	public enum MsoArrowheadWidth {
		msoArrowheadNarrow = 1,
		msoArrowheadWide = 3,
		msoArrowheadWidthMedium = 2,
		msoArrowheadWidthMixed = -2
	}
	#endregion
	#region MsoLineDashStyle
	[GeneratedCode("Suppress FxCop check", "")]
	public enum MsoLineDashStyle {
		msoLineDash = 4,
		msoLineDashDot = 5,
		msoLineDashDotDot = 6,
		msoLineDashStyleMixed = -2,
		msoLineLongDash = 7,
		msoLineLongDashDot = 8,
		msoLineLongDashDotDot = 9,
		msoLineRoundDot = 3,
		msoLineSolid = 1,
		msoLineSquareDot = 2,
		msoLineSysDash = 10,
		msoLineSysDashDot = 12,
		msoLineSysDot = 11
	}
	#endregion
	#region MsoPatternType
	[GeneratedCode("Suppress FxCop check", "")]
	public enum MsoPatternType {
		msoPattern10Percent = 2,
		msoPattern20Percent = 3,
		msoPattern25Percent = 4,
		msoPattern30Percent = 5,
		msoPattern40Percent = 6,
		msoPattern50Percent = 7,
		msoPattern5Percent = 1,
		msoPattern60Percent = 8,
		msoPattern70Percent = 9,
		msoPattern75Percent = 10,
		msoPattern80Percent = 11,
		msoPattern90Percent = 12,
		msoPatternCross = 0x33,
		msoPatternDarkDownwardDiagonal = 15,
		msoPatternDarkHorizontal = 13,
		msoPatternDarkUpwardDiagonal = 0x10,
		msoPatternDarkVertical = 14,
		msoPatternDashedDownwardDiagonal = 0x1c,
		msoPatternDashedHorizontal = 0x20,
		msoPatternDashedUpwardDiagonal = 0x1b,
		msoPatternDashedVertical = 0x1f,
		msoPatternDiagonalBrick = 40,
		msoPatternDiagonalCross = 0x36,
		msoPatternDivot = 0x2e,
		msoPatternDottedDiamond = 0x18,
		msoPatternDottedGrid = 0x2d,
		msoPatternDownwardDiagonal = 0x34,
		msoPatternHorizontal = 0x31,
		msoPatternHorizontalBrick = 0x23,
		msoPatternLargeCheckerBoard = 0x24,
		msoPatternLargeConfetti = 0x21,
		msoPatternLargeGrid = 0x22,
		msoPatternLightDownwardDiagonal = 0x15,
		msoPatternLightHorizontal = 0x13,
		msoPatternLightUpwardDiagonal = 0x16,
		msoPatternLightVertical = 20,
		msoPatternMixed = -2,
		msoPatternNarrowHorizontal = 30,
		msoPatternNarrowVertical = 0x1d,
		msoPatternOutlinedDiamond = 0x29,
		msoPatternPlaid = 0x2a,
		msoPatternShingle = 0x2f,
		msoPatternSmallCheckerBoard = 0x11,
		msoPatternSmallConfetti = 0x25,
		msoPatternSmallGrid = 0x17,
		msoPatternSolidDiamond = 0x27,
		msoPatternSphere = 0x2b,
		msoPatternTrellis = 0x12,
		msoPatternUpwardDiagonal = 0x35,
		msoPatternVertical = 50,
		msoPatternWave = 0x30,
		msoPatternWeave = 0x2c,
		msoPatternWideDownwardDiagonal = 0x19,
		msoPatternWideUpwardDiagonal = 0x1a,
		msoPatternZigZag = 0x26
	}
	#endregion
	#region MsoLineStyle
	[GeneratedCode("Suppress FxCop check", "")]
	public enum MsoLineStyle {
		msoLineSingle = 1,
		msoLineStyleMixed = -2,
		msoLineThickBetweenThin = 5,
		msoLineThickThin = 4,
		msoLineThinThick = 3,
		msoLineThinThin = 2
	}
	#endregion
	#region MsoColorType
	[GeneratedCode("Suppress FxCop check", "")]
	public enum MsoColorType {
		msoColorTypeCMS = 4,
		msoColorTypeCMYK = 3,
		msoColorTypeInk = 5,
		msoColorTypeMixed = -2,
		msoColorTypeRGB = 1,
		msoColorTypeScheme = 2
	}
	#endregion
	#region MsoSoftEdgeType
	[GeneratedCode("Suppress FxCop check", "")]
	public enum MsoSoftEdgeType {
		msoSoftEdgeType1 = 1,
		msoSoftEdgeType2 = 2,
		msoSoftEdgeType3 = 3,
		msoSoftEdgeType4 = 4,
		msoSoftEdgeType5 = 5,
		msoSoftEdgeType6 = 6,
		msoSoftEdgeTypeMixed = -2,
		msoSoftEdgeTypeNone = 0
	}
	#endregion
	#region MsoReflectionType
	[GeneratedCode("Suppress FxCop check", "")]
	public enum MsoReflectionType {
		msoReflectionType1 = 1,
		msoReflectionType2 = 2,
		msoReflectionType3 = 3,
		msoReflectionType4 = 4,
		msoReflectionType5 = 5,
		msoReflectionType6 = 6,
		msoReflectionType7 = 7,
		msoReflectionType8 = 8,
		msoReflectionType9 = 9,
		msoReflectionTypeMixed = -2,
		msoReflectionTypeNone = 0
	}
	#endregion
	#region MsoShadowStyle
	[GeneratedCode("Suppress FxCop check", "")]
	public enum MsoShadowStyle {
		msoShadowStyleInnerShadow = 1,
		msoShadowStyleMixed = -2,
		msoShadowStyleOuterShadow = 2
	}
	#endregion
	#region MsoShadowType
	[GeneratedCode("Suppress FxCop check", "")]
	public enum MsoShadowType {
		msoShadow1 = 1,
		msoShadow10 = 10,
		msoShadow11 = 11,
		msoShadow12 = 12,
		msoShadow13 = 13,
		msoShadow14 = 14,
		msoShadow15 = 15,
		msoShadow16 = 0x10,
		msoShadow17 = 0x11,
		msoShadow18 = 0x12,
		msoShadow19 = 0x13,
		msoShadow2 = 2,
		msoShadow20 = 20,
		msoShadow3 = 3,
		msoShadow4 = 4,
		msoShadow5 = 5,
		msoShadow6 = 6,
		msoShadow7 = 7,
		msoShadow8 = 8,
		msoShadow9 = 9,
		msoShadowMixed = -2
	}
	#endregion
	#region MsoTextEffectAlignment
	[GeneratedCode("Suppress FxCop check", "")]
	public enum MsoTextEffectAlignment {
		msoTextEffectAlignmentCentered = 2,
		msoTextEffectAlignmentLeft = 1,
		msoTextEffectAlignmentLetterJustify = 4,
		msoTextEffectAlignmentMixed = -2,
		msoTextEffectAlignmentRight = 3,
		msoTextEffectAlignmentStretchJustify = 6,
		msoTextEffectAlignmentWordJustify = 5
	}
	#endregion
	#region MsoPresetTextEffectShape
	[GeneratedCode("Suppress FxCop check", "")]
	public enum MsoPresetTextEffectShape {
		msoTextEffectShapeArchDownCurve = 10,
		msoTextEffectShapeArchDownPour = 14,
		msoTextEffectShapeArchUpCurve = 9,
		msoTextEffectShapeArchUpPour = 13,
		msoTextEffectShapeButtonCurve = 12,
		msoTextEffectShapeButtonPour = 0x10,
		msoTextEffectShapeCanDown = 20,
		msoTextEffectShapeCanUp = 0x13,
		msoTextEffectShapeCascadeDown = 40,
		msoTextEffectShapeCascadeUp = 0x27,
		msoTextEffectShapeChevronDown = 6,
		msoTextEffectShapeChevronUp = 5,
		msoTextEffectShapeCircleCurve = 11,
		msoTextEffectShapeCirclePour = 15,
		msoTextEffectShapeCurveDown = 0x12,
		msoTextEffectShapeCurveUp = 0x11,
		msoTextEffectShapeDeflate = 0x1a,
		msoTextEffectShapeDeflateBottom = 0x1c,
		msoTextEffectShapeDeflateInflate = 0x1f,
		msoTextEffectShapeDeflateInflateDeflate = 0x20,
		msoTextEffectShapeDeflateTop = 30,
		msoTextEffectShapeDoubleWave1 = 0x17,
		msoTextEffectShapeDoubleWave2 = 0x18,
		msoTextEffectShapeFadeDown = 0x24,
		msoTextEffectShapeFadeLeft = 0x22,
		msoTextEffectShapeFadeRight = 0x21,
		msoTextEffectShapeFadeUp = 0x23,
		msoTextEffectShapeInflate = 0x19,
		msoTextEffectShapeInflateBottom = 0x1b,
		msoTextEffectShapeInflateTop = 0x1d,
		msoTextEffectShapeMixed = -2,
		msoTextEffectShapePlainText = 1,
		msoTextEffectShapeRingInside = 7,
		msoTextEffectShapeRingOutside = 8,
		msoTextEffectShapeSlantDown = 0x26,
		msoTextEffectShapeSlantUp = 0x25,
		msoTextEffectShapeStop = 2,
		msoTextEffectShapeTriangleDown = 4,
		msoTextEffectShapeTriangleUp = 3,
		msoTextEffectShapeWave1 = 0x15,
		msoTextEffectShapeWave2 = 0x16
	}
	#endregion
	#region MsoPictureColorType
	[GeneratedCode("Suppress FxCop check", "")]
	public enum MsoPictureColorType {
		msoPictureAutomatic = 1,
		msoPictureBlackAndWhite = 3,
		msoPictureGrayscale = 2,
		msoPictureMixed = -2,
		msoPictureWatermark = 4
	}
	#endregion
	#region MsoPresetGradientType
	[GeneratedCode("Suppress FxCop check", "")]
	public enum MsoPresetGradientType {
		msoGradientBrass = 20,
		msoGradientCalmWater = 8,
		msoGradientChrome = 0x15,
		msoGradientChromeII = 0x16,
		msoGradientDaybreak = 4,
		msoGradientDesert = 6,
		msoGradientEarlySunset = 1,
		msoGradientFire = 9,
		msoGradientFog = 10,
		msoGradientGold = 0x12,
		msoGradientGoldII = 0x13,
		msoGradientHorizon = 5,
		msoGradientLateSunset = 2,
		msoGradientMahogany = 15,
		msoGradientMoss = 11,
		msoGradientNightfall = 3,
		msoGradientOcean = 7,
		msoGradientParchment = 14,
		msoGradientPeacock = 12,
		msoGradientRainbow = 0x10,
		msoGradientRainbowII = 0x11,
		msoGradientSapphire = 0x18,
		msoGradientSilver = 0x17,
		msoGradientWheat = 13,
		msoPresetGradientMixed = -2
	}
	#endregion
	#region MsoFillType
	[GeneratedCode("Suppress FxCop check", "")]
	public enum MsoFillType {
		msoFillBackground = 5,
		msoFillGradient = 3,
		msoFillMixed = -2,
		msoFillPatterned = 2,
		msoFillPicture = 6,
		msoFillSolid = 1,
		msoFillTextured = 4
	}
	#endregion
	#region MsoTextureType
	[GeneratedCode("Suppress FxCop check", "")]
	public enum MsoTextureType {
		msoTexturePreset = 1,
		msoTextureTypeMixed = -2,
		msoTextureUserDefined = 2
	}
	#endregion
	#region MsoPresetTexture
	[GeneratedCode("Suppress FxCop check", "")]
	public enum MsoPresetTexture {
		msoPresetTextureMixed = -2,
		msoTextureBlueTissuePaper = 0x11,
		msoTextureBouquet = 20,
		msoTextureBrownMarble = 11,
		msoTextureCanvas = 2,
		msoTextureCork = 0x15,
		msoTextureDenim = 3,
		msoTextureFishFossil = 7,
		msoTextureGranite = 12,
		msoTextureGreenMarble = 9,
		msoTextureMediumWood = 0x18,
		msoTextureNewsprint = 13,
		msoTextureOak = 0x17,
		msoTexturePaperBag = 6,
		msoTexturePapyrus = 1,
		msoTextureParchment = 15,
		msoTexturePinkTissuePaper = 0x12,
		msoTexturePurpleMesh = 0x13,
		msoTextureRecycledPaper = 14,
		msoTextureSand = 8,
		msoTextureStationery = 0x10,
		msoTextureWalnut = 0x16,
		msoTextureWaterDroplets = 5,
		msoTextureWhiteMarble = 10,
		msoTextureWovenMat = 4
	}
	#endregion
	#region MsoGradientStyle
	[GeneratedCode("Suppress FxCop check", "")]
	public enum MsoGradientStyle {
		msoGradientDiagonalDown = 4,
		msoGradientDiagonalUp = 3,
		msoGradientFromCenter = 7,
		msoGradientFromCorner = 5,
		msoGradientFromTitle = 6,
		msoGradientHorizontal = 1,
		msoGradientMixed = -2,
		msoGradientVertical = 2
	}
	#endregion
	#region MsoGradientColorType
	[GeneratedCode("Suppress FxCop check", "")]
	public enum MsoGradientColorType {
		msoGradientColorMixed = -2,
		msoGradientMultiColor = 4,
		msoGradientOneColor = 1,
		msoGradientPresetColors = 3,
		msoGradientTwoColors = 2
	}
	#endregion
	#region MsoScriptLanguage
	[GeneratedCode("Suppress FxCop check", "")]
	public enum MsoScriptLanguage {
		msoScriptLanguageASP = 3,
		msoScriptLanguageJava = 1,
		msoScriptLanguageOther = 4,
		msoScriptLanguageVisualBasic = 2
	}
	#endregion
	#region MsoScriptLocation
	[GeneratedCode("Suppress FxCop check", "")]
	public enum MsoScriptLocation {
		msoScriptLocationInBody = 2,
		msoScriptLocationInHead = 1
	}
	#endregion
	#region MsoHyperlinkType
	[GeneratedCode("Suppress FxCop check", "")]
	public enum MsoHyperlinkType {
		msoHyperlinkRange,
		msoHyperlinkShape,
		msoHyperlinkInlineShape
	}
	#endregion
	#region XlChartType
	[GeneratedCode("Suppress FxCop check", "")]
	public enum XlChartType {
		xl3DArea = -4098,
		xl3DAreaStacked = 0x4e,
		xl3DAreaStacked100 = 0x4f,
		xl3DBarClustered = 60,
		xl3DBarStacked = 0x3d,
		xl3DBarStacked100 = 0x3e,
		xl3DColumn = -4100,
		xl3DColumnClustered = 0x36,
		xl3DColumnStacked = 0x37,
		xl3DColumnStacked100 = 0x38,
		xl3DLine = -4101,
		xl3DPie = -4102,
		xl3DPieExploded = 70,
		xlArea = 1,
		xlAreaStacked = 0x4c,
		xlAreaStacked100 = 0x4d,
		xlBarClustered = 0x39,
		xlBarOfPie = 0x47,
		xlBarStacked = 0x3a,
		xlBarStacked100 = 0x3b,
		xlBubble = 15,
		xlBubble3DEffect = 0x57,
		xlColumnClustered = 0x33,
		xlColumnStacked = 0x34,
		xlColumnStacked100 = 0x35,
		xlConeBarClustered = 0x66,
		xlConeBarStacked = 0x67,
		xlConeBarStacked100 = 0x68,
		xlConeCol = 0x69,
		xlConeColClustered = 0x63,
		xlConeColStacked = 100,
		xlConeColStacked100 = 0x65,
		xlCylinderBarClustered = 0x5f,
		xlCylinderBarStacked = 0x60,
		xlCylinderBarStacked100 = 0x61,
		xlCylinderCol = 0x62,
		xlCylinderColClustered = 0x5c,
		xlCylinderColStacked = 0x5d,
		xlCylinderColStacked100 = 0x5e,
		xlDoughnut = -4120,
		xlDoughnutExploded = 80,
		xlLine = 4,
		xlLineMarkers = 0x41,
		xlLineMarkersStacked = 0x42,
		xlLineMarkersStacked100 = 0x43,
		xlLineStacked = 0x3f,
		xlLineStacked100 = 0x40,
		xlPie = 5,
		xlPieExploded = 0x45,
		xlPieOfPie = 0x44,
		xlPyramidBarClustered = 0x6d,
		xlPyramidBarStacked = 110,
		xlPyramidBarStacked100 = 0x6f,
		xlPyramidCol = 0x70,
		xlPyramidColClustered = 0x6a,
		xlPyramidColStacked = 0x6b,
		xlPyramidColStacked100 = 0x6c,
		xlRadar = -4151,
		xlRadarFilled = 0x52,
		xlRadarMarkers = 0x51,
		xlStockHLC = 0x58,
		xlStockOHLC = 0x59,
		xlStockVHLC = 90,
		xlStockVOHLC = 0x5b,
		xlSurface = 0x53,
		xlSurfaceTopView = 0x55,
		xlSurfaceTopViewWireframe = 0x56,
		xlSurfaceWireframe = 0x54,
		xlXYScatter = -4169,
		xlXYScatterLines = 0x4a,
		xlXYScatterLinesNoMarkers = 0x4b,
		xlXYScatterSmooth = 0x48,
		xlXYScatterSmoothNoMarkers = 0x49
	}
	#endregion
	#region MsoSegmentType
	[GeneratedCode("Suppress FxCop check", "")]
	public enum MsoSegmentType {
		msoSegmentLine,
		msoSegmentCurve
	}
	#endregion
	#region MsoCalloutAngleType
	[GeneratedCode("Suppress FxCop check", "")]
	public enum MsoCalloutAngleType {
		msoCalloutAngle30 = 2,
		msoCalloutAngle45 = 3,
		msoCalloutAngle60 = 4,
		msoCalloutAngle90 = 5,
		msoCalloutAngleAutomatic = 1,
		msoCalloutAngleMixed = -2
	}
	#endregion
	#region MsoCalloutDropType
	[GeneratedCode("Suppress FxCop check", "")]
	public enum MsoCalloutDropType {
		msoCalloutDropBottom = 4,
		msoCalloutDropCenter = 3,
		msoCalloutDropCustom = 1,
		msoCalloutDropMixed = -2,
		msoCalloutDropTop = 2
	}
	#endregion
	#region MsoExtrusionColorType
	[GeneratedCode("Suppress FxCop check", "")]
	public enum MsoExtrusionColorType {
		msoExtrusionColorAutomatic = 1,
		msoExtrusionColorCustom = 2,
		msoExtrusionColorTypeMixed = -2
	}
	#endregion
	#region MsoPresetExtrusionDirection
	[GeneratedCode("Suppress FxCop check", "")]
	public enum MsoPresetExtrusionDirection {
		msoExtrusionBottom = 2,
		msoExtrusionBottomLeft = 3,
		msoExtrusionBottomRight = 1,
		msoExtrusionLeft = 6,
		msoExtrusionNone = 5,
		msoExtrusionRight = 4,
		msoExtrusionTop = 8,
		msoExtrusionTopLeft = 9,
		msoExtrusionTopRight = 7,
		msoPresetExtrusionDirectionMixed = -2
	}
	#endregion
	#region MsoLightRigType
	[GeneratedCode("Suppress FxCop check", "")]
	public enum MsoLightRigType {
		msoLightRigBalanced = 14,
		msoLightRigBrightRoom = 0x1b,
		msoLightRigChilly = 0x16,
		msoLightRigContrasting = 0x12,
		msoLightRigFlat = 0x18,
		msoLightRigFlood = 0x11,
		msoLightRigFreezing = 0x17,
		msoLightRigGlow = 0x1a,
		msoLightRigHarsh = 0x10,
		msoLightRigLegacyFlat1 = 1,
		msoLightRigLegacyFlat2 = 2,
		msoLightRigLegacyFlat3 = 3,
		msoLightRigLegacyFlat4 = 4,
		msoLightRigLegacyHarsh1 = 9,
		msoLightRigLegacyHarsh2 = 10,
		msoLightRigLegacyHarsh3 = 11,
		msoLightRigLegacyHarsh4 = 12,
		msoLightRigLegacyNormal1 = 5,
		msoLightRigLegacyNormal2 = 6,
		msoLightRigLegacyNormal3 = 7,
		msoLightRigLegacyNormal4 = 8,
		msoLightRigMixed = -2,
		msoLightRigMorning = 0x13,
		msoLightRigSoft = 15,
		msoLightRigSunrise = 20,
		msoLightRigSunset = 0x15,
		msoLightRigThreePoint = 13,
		msoLightRigTwoPoint = 0x19
	}
	#endregion
	#region MsoBevelType
	[GeneratedCode("Suppress FxCop check", "")]
	public enum MsoBevelType {
		msoBevelAngle = 6,
		msoBevelArtDeco = 13,
		msoBevelCircle = 3,
		msoBevelConvex = 8,
		msoBevelCoolSlant = 9,
		msoBevelCross = 5,
		msoBevelDivot = 10,
		msoBevelHardEdge = 12,
		msoBevelNone = 1,
		msoBevelRelaxedInset = 2,
		msoBevelRiblet = 11,
		msoBevelSlope = 4,
		msoBevelSoftRound = 7,
		msoBevelTypeMixed = -2
	}
	#endregion
	#region MsoPresetLightingDirection
	[GeneratedCode("Suppress FxCop check", "")]
	public enum MsoPresetLightingDirection {
		msoLightingBottom = 8,
		msoLightingBottomLeft = 7,
		msoLightingBottomRight = 9,
		msoLightingLeft = 4,
		msoLightingNone = 5,
		msoLightingRight = 6,
		msoLightingTop = 2,
		msoLightingTopLeft = 1,
		msoLightingTopRight = 3,
		msoPresetLightingDirectionMixed = -2
	}
	#endregion
	#region MsoPresetLightingSoftness
	[GeneratedCode("Suppress FxCop check", "")]
	public enum MsoPresetLightingSoftness {
		msoLightingBright = 3,
		msoLightingDim = 1,
		msoLightingNormal = 2,
		msoPresetLightingSoftnessMixed = -2
	}
	#endregion
	#region MsoPresetMaterial
	[GeneratedCode("Suppress FxCop check", "")]
	public enum MsoPresetMaterial {
		msoMaterialClear = 13,
		msoMaterialDarkEdge = 11,
		msoMaterialFlat = 14,
		msoMaterialMatte = 1,
		msoMaterialMatte2 = 5,
		msoMaterialMetal = 3,
		msoMaterialMetal2 = 7,
		msoMaterialPlastic = 2,
		msoMaterialPlastic2 = 6,
		msoMaterialPowder = 10,
		msoMaterialSoftEdge = 12,
		msoMaterialSoftMetal = 15,
		msoMaterialTranslucentPowder = 9,
		msoMaterialWarmMatte = 8,
		msoMaterialWireFrame = 4,
		msoPresetMaterialMixed = -2
	}
	#endregion
	#region MsoPresetThreeDFormat
	[GeneratedCode("Suppress FxCop check", "")]
	public enum MsoPresetThreeDFormat {
		msoPresetThreeDFormatMixed = -2,
		msoThreeD1 = 1,
		msoThreeD10 = 10,
		msoThreeD11 = 11,
		msoThreeD12 = 12,
		msoThreeD13 = 13,
		msoThreeD14 = 14,
		msoThreeD15 = 15,
		msoThreeD16 = 0x10,
		msoThreeD17 = 0x11,
		msoThreeD18 = 0x12,
		msoThreeD19 = 0x13,
		msoThreeD2 = 2,
		msoThreeD20 = 20,
		msoThreeD3 = 3,
		msoThreeD4 = 4,
		msoThreeD5 = 5,
		msoThreeD6 = 6,
		msoThreeD7 = 7,
		msoThreeD8 = 8,
		msoThreeD9 = 9
	}
	#endregion
	#region MsoPresetCamera
	[GeneratedCode("Suppress FxCop check", "")]
	public enum MsoPresetCamera {
		msoCameraIsometricBottomDown = 0x17,
		msoCameraIsometricBottomUp = 0x16,
		msoCameraIsometricLeftDown = 0x19,
		msoCameraIsometricLeftUp = 0x18,
		msoCameraIsometricOffAxis1Left = 0x1c,
		msoCameraIsometricOffAxis1Right = 0x1d,
		msoCameraIsometricOffAxis1Top = 30,
		msoCameraIsometricOffAxis2Left = 0x1f,
		msoCameraIsometricOffAxis2Right = 0x20,
		msoCameraIsometricOffAxis2Top = 0x21,
		msoCameraIsometricOffAxis3Bottom = 0x24,
		msoCameraIsometricOffAxis3Left = 0x22,
		msoCameraIsometricOffAxis3Right = 0x23,
		msoCameraIsometricOffAxis4Bottom = 0x27,
		msoCameraIsometricOffAxis4Left = 0x25,
		msoCameraIsometricOffAxis4Right = 0x26,
		msoCameraIsometricRightDown = 0x1b,
		msoCameraIsometricRightUp = 0x1a,
		msoCameraIsometricTopDown = 0x15,
		msoCameraIsometricTopUp = 20,
		msoCameraLegacyObliqueBottom = 8,
		msoCameraLegacyObliqueBottomLeft = 7,
		msoCameraLegacyObliqueBottomRight = 9,
		msoCameraLegacyObliqueFront = 5,
		msoCameraLegacyObliqueLeft = 4,
		msoCameraLegacyObliqueRight = 6,
		msoCameraLegacyObliqueTop = 2,
		msoCameraLegacyObliqueTopLeft = 1,
		msoCameraLegacyObliqueTopRight = 3,
		msoCameraLegacyPerspectiveBottom = 0x11,
		msoCameraLegacyPerspectiveBottomLeft = 0x10,
		msoCameraLegacyPerspectiveBottomRight = 0x12,
		msoCameraLegacyPerspectiveFront = 14,
		msoCameraLegacyPerspectiveLeft = 13,
		msoCameraLegacyPerspectiveRight = 15,
		msoCameraLegacyPerspectiveTop = 11,
		msoCameraLegacyPerspectiveTopLeft = 10,
		msoCameraLegacyPerspectiveTopRight = 12,
		msoCameraObliqueBottom = 0x2e,
		msoCameraObliqueBottomLeft = 0x2d,
		msoCameraObliqueBottomRight = 0x2f,
		msoCameraObliqueLeft = 0x2b,
		msoCameraObliqueRight = 0x2c,
		msoCameraObliqueTop = 0x29,
		msoCameraObliqueTopLeft = 40,
		msoCameraObliqueTopRight = 0x2a,
		msoCameraOrthographicFront = 0x13,
		msoCameraPerspectiveAbove = 0x33,
		msoCameraPerspectiveAboveLeftFacing = 0x35,
		msoCameraPerspectiveAboveRightFacing = 0x36,
		msoCameraPerspectiveBelow = 0x34,
		msoCameraPerspectiveContrastingLeftFacing = 0x37,
		msoCameraPerspectiveContrastingRightFacing = 0x38,
		msoCameraPerspectiveFront = 0x30,
		msoCameraPerspectiveHeroicExtremeLeftFacing = 0x3b,
		msoCameraPerspectiveHeroicExtremeRightFacing = 60,
		msoCameraPerspectiveHeroicLeftFacing = 0x39,
		msoCameraPerspectiveHeroicRightFacing = 0x3a,
		msoCameraPerspectiveLeft = 0x31,
		msoCameraPerspectiveRelaxed = 0x3d,
		msoCameraPerspectiveRelaxedModerately = 0x3e,
		msoCameraPerspectiveRight = 50,
		msoPresetCameraMixed = -2
	}
	#endregion
	#region MsoHorizontalAnchor
	[GeneratedCode("Suppress FxCop check", "")]
	public enum MsoHorizontalAnchor {
		msoAnchorCenter = 2,
		msoAnchorNone = 1,
		msoHorizontalAnchorMixed = -2
	}
	#endregion
	#region MsoVerticalAnchor
	[GeneratedCode("Suppress FxCop check", "")]
	public enum MsoVerticalAnchor {
		msoAnchorBottom = 4,
		msoAnchorBottomBaseLine = 5,
		msoAnchorMiddle = 3,
		msoAnchorTop = 1,
		msoAnchorTopBaseline = 2,
		msoVerticalAnchorMixed = -2
	}
	#endregion
	#region MsoPathFormat
	[GeneratedCode("Suppress FxCop check", "")]
	public enum MsoPathFormat {
		msoPathType1 = 1,
		msoPathType2 = 2,
		msoPathType3 = 3,
		msoPathType4 = 4,
		msoPathTypeMixed = -2,
		msoPathTypeNone = 0
	}
	#endregion
	#region MsoWarpFormat
	[GeneratedCode("Suppress FxCop check", "")]
	public enum MsoWarpFormat {
		msoWarpFormat1 = 0,
		msoWarpFormat10 = 9,
		msoWarpFormat11 = 10,
		msoWarpFormat12 = 11,
		msoWarpFormat13 = 12,
		msoWarpFormat14 = 13,
		msoWarpFormat15 = 14,
		msoWarpFormat16 = 15,
		msoWarpFormat17 = 0x10,
		msoWarpFormat18 = 0x11,
		msoWarpFormat19 = 0x12,
		msoWarpFormat2 = 1,
		msoWarpFormat20 = 0x13,
		msoWarpFormat21 = 20,
		msoWarpFormat22 = 0x15,
		msoWarpFormat23 = 0x16,
		msoWarpFormat24 = 0x17,
		msoWarpFormat25 = 0x18,
		msoWarpFormat26 = 0x19,
		msoWarpFormat27 = 0x1a,
		msoWarpFormat28 = 0x1b,
		msoWarpFormat29 = 0x1c,
		msoWarpFormat3 = 2,
		msoWarpFormat30 = 0x1d,
		msoWarpFormat31 = 30,
		msoWarpFormat32 = 0x1f,
		msoWarpFormat33 = 0x20,
		msoWarpFormat34 = 0x21,
		msoWarpFormat35 = 0x22,
		msoWarpFormat36 = 0x23,
		msoWarpFormat4 = 3,
		msoWarpFormat5 = 4,
		msoWarpFormat6 = 5,
		msoWarpFormat7 = 6,
		msoWarpFormat8 = 7,
		msoWarpFormat9 = 8,
		msoWarpFormatMixed = -2
	}
	#endregion
	#region MsoAutoSize
	[GeneratedCode("Suppress FxCop check", "")]
	public enum MsoAutoSize {
		msoAutoSizeMixed = -2,
		msoAutoSizeNone = 0,
		msoAutoSizeShapeToFitText = 1,
		msoAutoSizeTextToFitShape = 2
	}
	#endregion
	#region MsoOrgChartLayoutType
	[GeneratedCode("Suppress FxCop check", "")]
	public enum MsoOrgChartLayoutType {
		msoOrgChartLayoutBothHanging = 2,
		msoOrgChartLayoutLeftHanging = 3,
		msoOrgChartLayoutMixed = -2,
		msoOrgChartLayoutRightHanging = 4,
		msoOrgChartLayoutStandard = 1
	}
	#endregion
	#region MsoDiagramNodeType
	[GeneratedCode("Suppress FxCop check", "")]
	public enum MsoDiagramNodeType {
		msoDiagramAssistant = 2,
		msoDiagramNode = 1
	}
	#endregion
	#region MsoRelativeNodePosition
	[GeneratedCode("Suppress FxCop check", "")]
	public enum MsoRelativeNodePosition {
		msoAfterLastSibling = 4,
		msoAfterNode = 2,
		msoBeforeFirstSibling = 3,
		msoBeforeNode = 1
	}
	#endregion
}
