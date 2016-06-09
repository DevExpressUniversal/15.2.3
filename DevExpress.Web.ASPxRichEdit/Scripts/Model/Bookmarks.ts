module __aspxRichEdit {
    export class Bookmark {
        public name: string = "";
        public start: Position;
        public end: Position;

        constructor() {
        }

        isHidden(): boolean {
            return this.name.length > 0 && this.name[0] == "_";
        }
    }
} 