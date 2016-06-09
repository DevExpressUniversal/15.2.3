module __aspxRichEdit {
    export class BorderInfo implements IEquatable<BorderInfo>, ISupportCopyFrom<BorderInfo>, ICloneable<BorderInfo> {
        style: BorderLineStyle = BorderLineStyle.None;
        color: number = -1;
        width: number = 0; // border width. Example: case "thin line, then thick, then thin" actualWidth = coef * width. Coef in table TableBorderCalculator. Case straight line coef = 1
        offset: number = 0; // think dont use in win rich
        frame: boolean = false; // think dont use in win rich
        shadow: boolean = false; // think dont use in win rich

        equals(obj: BorderInfo): boolean {
            return obj && this.style == obj.style &&
                this.color == obj.color &&
                this.width == obj.width &&
                this.offset == obj.offset &&
                this.frame == obj.frame &&
                this.shadow == obj.shadow;
        }
        static equalsBinary(borderInfoA: BorderInfo, borderInfoB: BorderInfo): boolean {
            return borderInfoA && borderInfoB &&
                borderInfoA.style == borderInfoB.style &&
                borderInfoA.color == borderInfoB.color &&
                borderInfoA.width == borderInfoB.width &&
                borderInfoA.offset == borderInfoB.offset &&
                borderInfoA.frame == borderInfoB.frame &&
                borderInfoA.shadow == borderInfoB.shadow;
        }

        copyFrom(obj: BorderInfo) {
            this.style = obj.style;
            this.color = obj.color;
            this.width = obj.width;
            this.offset = obj.offset;
            this.frame = obj.frame;
            this.shadow = obj.shadow;
        }

        reset() {
            this.style = BorderLineStyle.None;
            this.color = -1;
            this.width = 0;
            this.offset = 0;
            this.frame = false;
            this.shadow = false;
        }

        public clone(): BorderInfo {
            var result = new BorderInfo();
            result.copyFrom(this);
            return result;
        }
    }

    export class TableBordersBase implements IEquatable<TableBordersBase> {
        bottomBorder: BorderInfo;
        leftBorder: BorderInfo;
        rightBorder: BorderInfo;
        topBorder: BorderInfo;

        public equals(obj: TableBordersBase): boolean {
            if (!obj)
                return false;
            return this.bottomBorder.equals(obj.bottomBorder) &&
                this.leftBorder.equals(obj.leftBorder) &&
                this.rightBorder.equals(obj.rightBorder) &&
                this.topBorder.equals(obj.topBorder);
        }

        public copyFrom(obj: TableBordersBase) {
            this.bottomBorder = obj.bottomBorder.clone();
            this.leftBorder = obj.leftBorder.clone();
            this.rightBorder = obj.rightBorder.clone();
            this.topBorder = obj.topBorder.clone();
        }
    }

    export class TableBorders extends TableBordersBase implements IEquatable<TableBorders>, ICloneable<TableBorders> {
        insideHorizontalBorder: BorderInfo;
        insideVerticalBorder: BorderInfo;

        public equals(obj: TableBorders): boolean {
            return super.equals(obj) &&
                this.insideHorizontalBorder.equals(obj.insideHorizontalBorder) &&
                this.insideVerticalBorder.equals(obj.insideVerticalBorder);
        }

        public copyFrom(obj: TableBorders) {
            super.copyFrom(obj);
            this.insideHorizontalBorder = obj.insideHorizontalBorder.clone();
            this.insideVerticalBorder = obj.insideVerticalBorder.clone();
        }

        public clone(): TableBorders {
            var result = new TableBorders();
            result.copyFrom(this);
            return result;
        }

        static create(top: BorderInfo, right: BorderInfo, bottom: BorderInfo, left: BorderInfo, insideHorizontal: BorderInfo, insideVertical: BorderInfo): TableBorders {
            let result = new TableBorders();
            result.topBorder = top;
            result.rightBorder = right;
            result.bottomBorder = bottom;
            result.leftBorder = left;
            result.insideHorizontalBorder = insideHorizontal;
            result.insideVerticalBorder = insideVertical;
            return result;
        }
    }

    export class TableCellBorders extends TableBordersBase implements IEquatable<TableCellBorders>, ICloneable<TableCellBorders> {
        topLeftDiagonalBorder: BorderInfo;
        topRightDiagonalBorder: BorderInfo;

        public equals(obj: TableCellBorders): boolean {
            return super.equals(obj) &&
                this.topLeftDiagonalBorder.equals(obj.topLeftDiagonalBorder) &&
                this.topRightDiagonalBorder.equals(obj.topRightDiagonalBorder);
        }

        public copyFrom(obj: TableCellBorders) {
            super.copyFrom(obj);
            this.topLeftDiagonalBorder = obj.topLeftDiagonalBorder.clone();
            this.topRightDiagonalBorder = obj.topRightDiagonalBorder.clone();
        }

        public clone(): TableCellBorders {
            var result = new TableCellBorders();
            result.copyFrom(this);
            return result;
        }

        static create(top: BorderInfo, right: BorderInfo, bottom: BorderInfo, left: BorderInfo, topLeftDiagonal: BorderInfo, topRightDiagonal: BorderInfo): TableCellBorders {
            let result = new TableCellBorders();
            result.topBorder = top;
            result.rightBorder = right;
            result.bottomBorder = bottom;
            result.leftBorder = left;
            result.topLeftDiagonalBorder = topLeftDiagonal;
            result.topRightDiagonalBorder = topRightDiagonal;
            return result;
        }
    }

    export enum BorderLineStyle {
        Nil = -1,
        None = 0,
        Single = 1,
        Thick = 2,
        Double = 3,
        Dotted = 4,
        Dashed = 5,
        DotDash = 6,
        DotDotDash = 7,
        Triple = 8,
        ThinThickSmallGap = 9,
        ThickThinSmallGap = 10,
        ThinThickThinSmallGap = 11,
        ThinThickMediumGap = 12,
        ThickThinMediumGap = 13,
        ThinThickThinMediumGap = 14,
        ThinThickLargeGap = 15,
        ThickThinLargeGap = 16,
        ThinThickThinLargeGap = 17,
        Wave = 18,
        DoubleWave = 19,
        DashSmallGap = 20,
        DashDotStroked = 21,
        ThreeDEmboss = 22,
        ThreeDEngrave = 23,
        Outset = 24,
        Inset = 25,
        //art borders
        Apples,
        ArchedScallops,
        BabyPacifier,
        BabyRattle,
        Balloons3Colors,
        BalloonsHotAir,
        BasicBlackDashes,
        BasicBlackDots,
        BasicBlackSquares,
        BasicThinLines,
        BasicWhiteDashes,
        BasicWhiteDots,
        BasicWhiteSquares,
        BasicWideInline,
        BasicWideMidline,
        BasicWideOutline,
        Bats,
        Birds,
        BirdsFlight,
        Cabins,
        CakeSlice,
        CandyCorn,
        CelticKnotwork,
        CertificateBanner,
        ChainLink,
        ChampagneBottle,
        CheckedBarBlack,
        CheckedBarColor,
        Checkered,
        ChristmasTree,
        CirclesLines,
        CirclesRectangles,
        ClassicalWave,
        Clocks,
        Compass,
        Confetti,
        ConfettiGrays,
        ConfettiOutline,
        ConfettiStreamers,
        ConfettiWhite,
        CornerTriangles,
        CouponCutoutDashes,
        CouponCutoutDots,
        CrazyMaze,
        CreaturesButterfly,
        CreaturesFish,
        CreaturesInsects,
        CreaturesLadyBug,
        CrossStitch,
        Cup,
        DecoArch,
        DecoArchColor,
        DecoBlocks,
        DiamondsGray,
        DoubleD,
        DoubleDiamonds,
        Earth1,
        Earth2,
        EclipsingSquares1,
        EclipsingSquares2,
        EggsBlack,
        Fans,
        Film,
        Firecrackers,
        FlowersBlockPrint,
        FlowersDaisies,
        FlowersModern1,
        FlowersModern2,
        FlowersPansy,
        FlowersRedRose,
        FlowersRoses,
        FlowersTeacup,
        FlowersTiny,
        Gems,
        GingerbreadMan,
        Gradient,
        Handmade1,
        Handmade2,
        HeartBalloon,
        HeartGray,
        Hearts,
        HeebieJeebies,
        Holly,
        HouseFunky,
        Hypnotic,
        IceCreamCones,
        LightBulb,
        Lightning1,
        Lightning2,
        MapleLeaf,
        MapleMuffins,
        MapPins,
        Marquee,
        MarqueeToothed,
        Moons,
        Mosaic,
        MusicNotes,
        Northwest,
        Ovals,
        Packages,
        PalmsBlack,
        PalmsColor,
        PaperClips,
        Papyrus,
        PartyFavor,
        PartyGlass,
        Pencils,
        People,
        PeopleHats,
        PeopleWaving,
        Poinsettias,
        PostageStamp,
        Pumpkin1,
        PushPinNote1,
        PushPinNote2,
        Pyramids,
        PyramidsAbove,
        Quadrants,
        Rings,
        Safari,
        Sawtooth,
        SawtoothGray,
        ScaredCat,
        Seattle,
        ShadowedSquares,
        SharksTeeth,
        ShorebirdTracks,
        Skyrocket,
        SnowflakeFancy,
        Snowflakes,
        Sombrero,
        Southwest,
        Stars,
        Stars3d,
        StarsBlack,
        StarsShadowed,
        StarsTop,
        Sun,
        Swirligig,
        TornPaper,
        TornPaperBlack,
        Trees,
        TriangleParty,
        Triangles,
        Tribal1,
        Tribal2,
        Tribal3,
        Tribal4,
        Tribal5,
        Tribal6,
        TwistedLines1,
        TwistedLines2,
        Vine,
        Waveline,
        WeavingAngles,
        WeavingBraid,
        WeavingRibbon,
        WeavingStrips,
        WhiteFlowers,
        Woodwork,
        XIllusions,
        ZanyTriangles,
        ZigZag,
        ZigZagStitch,
        Disabled = 0x7FFFFFFF//don't use in rich, for flow reports only!!!
    }
}