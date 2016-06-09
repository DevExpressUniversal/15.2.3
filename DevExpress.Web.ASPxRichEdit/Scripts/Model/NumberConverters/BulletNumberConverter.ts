module __aspxRichEdit {
    export class BulletNumberConverter extends OrdinalBasedNumberConverter {
        constructor() {
            super();
            this.type = NumberingFormat.Bullet;
        }

        convertNumberCore(value: number) {
            return "•";
        }
    }
} 