ASPxClientSpreadsheet.Functions = [
	{
		name: "ABRUNDEN",
		description: "Rundet die Zahl auf Anzahl_Stellen ab.",
		arguments: [
			{
				name: "Zahl",
				description: "ist eine reelle Zahl, die Sie abrunden wollen"
			},
			{
				name: "Anzahl_Stellen",
				description: "legt die Anzahl der Dezimalstellen fest, auf die Sie die Zahl abrunden wollen. Negative Werte runden auf ganze Zehnerpotenzen: RUNDEN(225;-2) ergibt 200. 0 rundet auf die nächste Ganzzahl"
			}
		]
	},
	{
		name: "ABS",
		description: "Gibt den Absolutwert einer Zahl zurück.",
		arguments: [
			{
				name: "Zahl",
				description: "ist die reelle Zahl, deren Absolutwert Sie ermitteln möchten"
			}
		]
	},
	{
		name: "ACHSENABSCHNITT",
		description: "Gibt den Schnittpunkt der Regressionsgeraden zurück.",
		arguments: [
			{
				name: "Y_Werte",
				description: "ist eine abhängige Datengruppe oder Reihe von Beobachtungen, Argumente können Zahlen, Namen, Arrays oder Bezüge sein"
			},
			{
				name: "X_Werte",
				description: "ist eine unabhängige Datengruppe oder Reihe von Beobachtungen, Argumente können Zahlen, Namen, Arrays oder Bezüge sein"
			}
		]
	},
	{
		name: "ADRESSE",
		description: "Gibt einen Verweis auf eine Zelle einer Tabelle als Text zurück.",
		arguments: [
			{
				name: "Zeile",
				description: "ist die Zeilennummer, die für den Zellbezug verwendet werden soll"
			},
			{
				name: "Spalte",
				description: "ist die Spaltennummer, die für den Zellbezug verwendet werden soll"
			},
			{
				name: "Abs",
				description: "gibt an, welcher Bezugstyp zurückgegeben werden soll: absolut = 1; absolute Zeile/relative Spalte = 2; relative Zeile/absolute Spalte = 3; relative = 4"
			},
			{
				name: "A1",
				description: "ist ein Wahrheitswert, der angibt, ob Bezüge in der A1- oder Z1S1-Schreibweise ausgegeben werden sollen: 1 oder WAHR = A1-Bezug; 0 oder FALSCH = Z1S1-Bezug"
			},
			{
				name: "Tabellenname",
				description: "ist der Text, der den Tabellennamen angibt, der als externer Bezug verwendet werden soll"
			}
		]
	},
	{
		name: "ANZAHL",
		description: "Berechnet, wie viele Zellen in einem Bereich Zahlen enthalten.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "Wert1",
				description: "sind 1 bis 255 Argumente, die unterschiedliche Datentypen beinhalten können oder sich auf solche beziehen können, wobei nur Zahlen in die Berechnung eingehen"
			},
			{
				name: "Wert2",
				description: "sind 1 bis 255 Argumente, die unterschiedliche Datentypen beinhalten können oder sich auf solche beziehen können, wobei nur Zahlen in die Berechnung eingehen"
			}
		]
	},
	{
		name: "ANZAHL2",
		description: "Zählt die Anzahl nicht leerer Zellen in einem Bereich.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "Wert1",
				description: "sind 1 bis 255 Argumente, die zu zählende Werte und Zellen darstellen. Werte können beliebige Informationen sein"
			},
			{
				name: "Wert2",
				description: "sind 1 bis 255 Argumente, die zu zählende Werte und Zellen darstellen. Werte können beliebige Informationen sein"
			}
		]
	},
	{
		name: "ANZAHLLEEREZELLEN",
		description: "Zählt die leeren Zellen in einem Zellbereich.",
		arguments: [
			{
				name: "Bereich",
				description: "ist der Bereich, von dem Sie anfangen wollen, die leeren Zellen zu zählen"
			}
		]
	},
	{
		name: "ARABISCH",
		description: "Konvertiert eine römische Zahl in eine arabische Zahl.",
		arguments: [
			{
				name: "Text",
				description: "ist die zu konvertierende römische Zahl"
			}
		]
	},
	{
		name: "ARBEITSTAG",
		description: "Gibt die fortlaufende Zahl des Datums zurück, vor oder nach einer bestimmten Anzahl von Arbeitstagen.",
		arguments: [
			{
				name: "Ausgangsdatum",
				description: "ist die fortlaufende Zahl, die Ausgangsdatum repräsentiert"
			},
			{
				name: "Tage",
				description: "ist die Anzahl der Arbeitstage vor oder nach Ausgangsdatum"
			},
			{
				name: "Freie_Tage",
				description: "ist eine optionale Matrix von ein oder mehreren fortlaufenden Zahlen, die alle Arten von arbeitsfreien Tagen (Feiertage, etc.) repräsentieren"
			}
		]
	},
	{
		name: "ARBEITSTAG.INTL",
		description: "Gibt die fortlaufende Nummer des Datums vor oder nach einer angegebenen Anzahl von Arbeitstagen mit benutzerdefinierten Wochenendparametern zurück.",
		arguments: [
			{
				name: "Ausgangsdatum",
				description: "ist eine fortlaufende Datumsnummer, die das Ausgangsdatum darstellt"
			},
			{
				name: "Tage",
				description: "ist die Anzahl der nicht am Wochenende liegenden und nicht freien Tage vor oder nach dem Ausgangsdatum"
			},
			{
				name: "Wochenende",
				description: "ist eine Zahl oder Zeichenfolge, die das Auftreten von Wochenenden angibt"
			},
			{
				name: "Freie_Tage",
				description: "ist ein optionales Array aus einer oder mehreren fortlaufenden Datumsnummern, die aus dem Arbeitskalender ausgeschlossen werden sollen, wie etwa Bundes- oder Landesfeiertage und bewegliche Feiertage"
			}
		]
	},
	{
		name: "ARCCOS",
		description: "Gibt den Arkuskosinus einer Zahl im Bereich von 0 bis Pi zurück.",
		arguments: [
			{
				name: "Zahl",
				description: "ist der Kosinus des Winkels, den Sie berechnen wollen, und liegt zwischen -1 und 1"
			}
		]
	},
	{
		name: "ARCCOSHYP",
		description: "Gibt den umgekehrten hyperbolischen Kosinus einer Zahl zurück.",
		arguments: [
			{
				name: "Zahl",
				description: "ist eine reelle Zahl größer oder gleich 1 zurück"
			}
		]
	},
	{
		name: "ARCCOT",
		description: "Gibt den Arkuskotangens einer Zahl als Bogenmaß im Bereich 0 bis Pi zurück.",
		arguments: [
			{
				name: "Zahl",
				description: "ist der Kotangens des gewünschten Winkels"
			}
		]
	},
	{
		name: "ARCCOTHYP",
		description: "Gibt den umgekehrten hyperbolischen Kotangens einer Zahl zurück.",
		arguments: [
			{
				name: "Zahl",
				description: "ist der hyperbolische Kotangens des gewünschten Winkels"
			}
		]
	},
	{
		name: "ARCSIN",
		description: "Gibt den Arkussinus einer Zahl im Bereich von -Pi/2 bis Pi/2 zurück.",
		arguments: [
			{
				name: "Zahl",
				description: "ist der Sinus des Winkels, den Sie berechnen wollen, und liegt zwischen -1 und 1"
			}
		]
	},
	{
		name: "ARCSINHYP",
		description: "Gibt den umgekehrten hyperbolischen Sinus einer Zahl zurück.",
		arguments: [
			{
				name: "Zahl",
				description: "ist eine reelle Zahl größer oder gleich 1"
			}
		]
	},
	{
		name: "ARCTAN",
		description: "Gibt den Arkustangens einer Zahl in RAD in einem Bereich von -Pi/2 bis Pi/2 zurück.",
		arguments: [
			{
				name: "Zahl",
				description: "ist der Tangens des Winkels, den Sie berechnen wollen"
			}
		]
	},
	{
		name: "ARCTAN2",
		description: "Gibt den Arkustangens ausgehend von einer x- und einer y-Koordinate zurück in RAD von -Pi bis Pi (ohne -Pi selbst).",
		arguments: [
			{
				name: "x_Koordinate",
				description: "ist die x-Koordinate des Punktes"
			},
			{
				name: "y_Koordinate",
				description: "ist die y-Koordinate des Punktes"
			}
		]
	},
	{
		name: "ARCTANHYP",
		description: "Gibt den umgekehrten hyperbolischen Tangens einer Zahl zurück.",
		arguments: [
			{
				name: "Zahl",
				description: "ist eine reelle Zahl zwischen -1 und 1 (-1 und 1 ausgeschlossen)"
			}
		]
	},
	{
		name: "AUFGELZINSF",
		description: "Gibt die aufgelaufenen Zinsen (Stückzinsen) eines Wertpapiers zurück, die bei Fälligkeit ausgezahlt werden.",
		arguments: [
			{
				name: "Emission",
				description: "ist das Datum der Wertpapieremission, als fortlaufende Zahl angegeben"
			},
			{
				name: "Abrechnung",
				description: "ist der Fälligkeitstermin des Wertpapiers, als fortlaufende Zahl angegeben"
			},
			{
				name: "Nominalzins",
				description: "ist der jährliche Nominalzins (Kuponzinssatz) des Wertpapiers"
			},
			{
				name: "Nennwert",
				description: "ist der Nennwert des Wertpapiers"
			},
			{
				name: "Basis",
				description: "gibt an, auf welcher Basis die Zinstage gezählt werden"
			}
		]
	},
	{
		name: "AUFRUNDEN",
		description: "Rundet die Zahl auf Anzahl_Stellen auf.",
		arguments: [
			{
				name: "Zahl",
				description: "ist eine reelle Zahl, die Sie aufrunden wollen"
			},
			{
				name: "Anzahl_Stellen",
				description: "legt die Anzahl der Dezimalstellen fest, auf die Sie die Zahl aufrunden wollen"
			}
		]
	},
	{
		name: "AUSZAHLUNG",
		description: "Gibt den Auszahlungsbetrag eines voll investierten Wertpapiers am Fälligkeitstermin zurück.",
		arguments: [
			{
				name: "Abrechnung",
				description: "ist der Abrechnungstermin des Wertpapierkaufs, als fortlaufende Zahl angegeben"
			},
			{
				name: "Fälligkeit",
				description: "ist der Fälligkeitstermin des Wertpapiers, als fortlaufende Zahl angegeben"
			},
			{
				name: "Anlage",
				description: "ist der Betrag, der in dem Wertpapier angelegt werden soll"
			},
			{
				name: "Disagio",
				description: "ist der in Prozent ausgedrückte Abschlag (Disagio) des Wertpapiers"
			},
			{
				name: "Basis",
				description: "gibt an, auf welcher Basis die Zinstage gezählt werden"
			}
		]
	},
	{
		name: "BAHTTEXT",
		description: "Wandelt eine Zahl in Text um (Baht).",
		arguments: [
			{
				name: "Zahl",
				description: "die Zahl, die Sie umwandeln möchten"
			}
		]
	},
	{
		name: "BASIS",
		description: "Konvertiert eine Zahl in eine Textdarstellung mit der angegebenen Basis.",
		arguments: [
			{
				name: "Zahl",
				description: "ist die zu konvertierende Zahl"
			},
			{
				name: "Basis",
				description: "ist die Basis, in die die Zahl konvertiert werden soll"
			},
			{
				name: "Mindestlänge",
				description: "ist die Mindestlänge der zurückgegebenen Zeichenfolge. Ohne Angabe werden keine führenden Nullen hinzugefügt"
			}
		]
	},
	{
		name: "BEREICH",
		description: "",
		arguments: [
			{
				name: "",
				description: ""
			}
		]
	},
	{
		name: "BEREICH.VERSCHIEBEN",
		description: "Gibt einen Bezug zurück, der gegenüber dem angegebenen Bezug versetzt ist.",
		arguments: [
			{
				name: "Bezug",
				description: "ist der Bezug, der als Ausgangspunkt des Verschiebevorgangs dienen soll"
			},
			{
				name: "Zeilen",
				description: "ist die Anzahl der Zeilen, um die Sie die obere linke Eckzelle des Bereiches nach oben oder nach unten verschieben wollen"
			},
			{
				name: "Spalten",
				description: "ist die Anzahl der Spalten, um die Sie die obere linke Eckzelle des Bereiches nach links oder nach rechts verschieben wollen"
			},
			{
				name: "Höhe",
				description: "ist die Höhe des neuen Bezuges in Zeilen. Wenn der Parameter fehlt, wird die selbe Höhe wie beim Ursprungsbezug angenommen"
			},
			{
				name: "Breite",
				description: "ist die Breite des neuen Bezuges in Spalten. Wenn der Parameter fehlt, wird die selbe Breite wie beim Ursprungsbezug angenommen"
			}
		]
	},
	{
		name: "BEREICHE",
		description: "Gibt die Anzahl der innerhalb eines Bezuges aufgeführten Bereiche zurück.",
		arguments: [
			{
				name: "Bezug",
				description: "ist ein Bezug auf eine Zelle oder einen Zellbereich und kann sich auf mehrere Bereiche gleichzeitig beziehen"
			}
		]
	},
	{
		name: "BESSELI",
		description: "Gibt die geänderte Besselfunktion In(x) zurück.",
		arguments: [
			{
				name: "x",
				description: "ist der Wert, für den die Funktion ausgewertet werden soll"
			},
			{
				name: "n",
				description: "ist die Ordnung der Besselfunktion"
			}
		]
	},
	{
		name: "BESSELJ",
		description: "Gibt die Besselfunktion Jn(x) zurück.",
		arguments: [
			{
				name: "x",
				description: "ist der Wert, für den die Funktion ausgewertet werden soll"
			},
			{
				name: "n",
				description: "ist die Ordnung der Besselfunktion"
			}
		]
	},
	{
		name: "BESSELK",
		description: "Gibt die modifizierte Besselfunktion Kn(x) zurück.",
		arguments: [
			{
				name: "x",
				description: "ist der Wert, für den die Funktion ausgewertet werden soll"
			},
			{
				name: "n",
				description: "ist die Ordnung der Besselfunktion"
			}
		]
	},
	{
		name: "BESSELY",
		description: "Gibt die Besselfunktion Yn(x) zurück.",
		arguments: [
			{
				name: "x",
				description: "ist der Wert, für den die Funktion ausgewertet werden soll"
			},
			{
				name: "n",
				description: "ist die Ordnung der Besselfunktion"
			}
		]
	},
	{
		name: "BESTIMMTHEITSMASS",
		description: "Gibt das Quadrat des Pearsonschen Korrelationskoeffizienten zurück.",
		arguments: [
			{
				name: "Y_Werte",
				description: "ist eine Matrix oder ein Bereich von Datenpunkten, Argumente können Zahlen, Namen, Arrays oder Bezüge sein"
			},
			{
				name: "X_Werte",
				description: "ist eine Matrix oder ein Bereich von Datenpunkten, Argumente können Zahlen, Namen, Arrays oder Bezüge sein"
			}
		]
	},
	{
		name: "BETA.INV",
		description: "Gibt Perzentile der Betaverteilung (BETA.VERT) zurück.",
		arguments: [
			{
				name: "Wahrsch",
				description: "ist die zur Betaverteilung gehörige Wahrscheinlichkeit"
			},
			{
				name: "Alpha",
				description: "ist ein Parameter der Verteilung und muss größer als 0 sein"
			},
			{
				name: "Beta",
				description: "ist ein Parameter der Verteilung und muss größer als 0 sein"
			},
			{
				name: "A",
				description: "ist eine optionale Untergrenze des Intervalls für x. Wenn der Parameter fehlt, dann ist A = 0"
			},
			{
				name: "B",
				description: "ist eine optionale Obergrenze des Intervalls für x. Wenn der Parameter fehlt, dann ist B = 1"
			}
		]
	},
	{
		name: "BETA.VERT",
		description: "Gibt Werte der Verteilungsfunktion einer betaverteilten Zufallsvariablen zurück.",
		arguments: [
			{
				name: "x",
				description: "ist der Wert, an dem die Funktion über dem Intervall A bis B ausgewertet werden soll"
			},
			{
				name: "Alpha",
				description: "ist ein Parameter der Verteilung und muss größer als 0 sein"
			},
			{
				name: "Beta",
				description: "ist ein Parameter der Verteilung und muss größer als 0 sein"
			},
			{
				name: "kumuliert",
				description: "ist ein Wahrheitswert: für die kumulierte Verteilungsfunktion WAHR; für die Wahrscheinlichkeitsdichtefunktion FALSCH"
			},
			{
				name: "A",
				description: "ist eine optionale untere Begrenzung des Intervalls für x. Wenn der Parameter fehlt, dann ist A = 0"
			},
			{
				name: "B",
				description: "ist eine optionale obere Begrenzung des Intervalls für x. Wenn der Parameter fehlt, dann ist B = 1"
			}
		]
	},
	{
		name: "BETAINV",
		description: "Gibt Perzentile der Betaverteilung zurück.",
		arguments: [
			{
				name: "Wahrsch",
				description: "ist die zur Betaverteilung gehörige Wahrscheinlichkeit"
			},
			{
				name: "Alpha",
				description: "ist ein Parameter der Verteilung und muss größer als 0 sein"
			},
			{
				name: "Beta",
				description: "ist ein Parameter der Verteilung und muss größer als 0 sein"
			},
			{
				name: "A",
				description: "ist eine optionale Untergrenze des Intervalls für x. Wenn der Parameter fehlt, dann ist A = 0"
			},
			{
				name: "B",
				description: "ist eine optionale Obergrenze des Intervalls für x. Wenn der Parameter fehlt, dann ist B = 1"
			}
		]
	},
	{
		name: "BETAVERT",
		description: "Gibt Werte der Verteilungsfunktion einer betaverteilten Zufallsvariablen zurück.",
		arguments: [
			{
				name: "x",
				description: "ist der Wert, an dem die Funktion über dem Intervall A bis B ausgewertet werden soll"
			},
			{
				name: "Alpha",
				description: "ist ein Parameter der Verteilung und muss größer als 0 sein"
			},
			{
				name: "Beta",
				description: "ist ein Parameter der Verteilung und muss größer als 0 sein"
			},
			{
				name: "A",
				description: "ist eine optionale untere Begrenzung des Intervalls für x. Wenn der Parameter fehlt, dann ist A = 0"
			},
			{
				name: "B",
				description: "ist eine optionale obere Begrenzung des Intervalls für x. Wenn der Parameter fehlt, dann ist A = 1"
			}
		]
	},
	{
		name: "BININDEZ",
		description: "Wandelt eine binäre Zahl (Dualzahl) in eine dezimale Zahl um.",
		arguments: [
			{
				name: "Zahl",
				description: "ist die binäre Zahl, die Sie umwandeln möchten"
			}
		]
	},
	{
		name: "BININHEX",
		description: "Wandelt eine binäre Zahl (Dualzahl) in eine hexadezimale Zahl um.",
		arguments: [
			{
				name: "Zahl",
				description: "ist die binäre Zahl, die Sie umwandeln möchten"
			},
			{
				name: "Stellen",
				description: "ist die Zahl der verwendeten Stellen"
			}
		]
	},
	{
		name: "BININOKT",
		description: "Wandel eine binäre Zahl (Dualzahl) in eine oktale Zahl um.",
		arguments: [
			{
				name: "Zahl",
				description: "ist die binäre Zahl, die Sie umwandeln möchten"
			},
			{
				name: "Stellen",
				description: "ist die Zahl der verwendeten Stellen"
			}
		]
	},
	{
		name: "BINOM.INV",
		description: "Gibt den kleinsten Wert, für den die kumulierten Wahrscheinlichkeiten der Binomialverteilung größer oder gleich einer Grenzwahrscheinlichkeit sind zurück.",
		arguments: [
			{
				name: "Versuche",
				description: "ist die Zahl der Bernoulliexperimente"
			},
			{
				name: "Erfolgswahrsch",
				description: "ist die Wahrscheinlichkeit für den günstigen Ausgang des Experiments, eine Zahl von einschließlich 0 bis einschließlich 1"
			},
			{
				name: "Alpha",
				description: "ist die Grenzwahrscheinlichkeit, eine Zahl von einschließlich 0 bis einschließlich 1"
			}
		]
	},
	{
		name: "BINOM.VERT",
		description: "Gibt Wahrscheinlichkeiten einer binomialverteilten Zufallsvariablen zurück.",
		arguments: [
			{
				name: "Zahl_Erfolge",
				description: "ist die Zahl der günstigen Ereignisse der Experimente"
			},
			{
				name: "Versuche",
				description: "ist die Zahl der unabhängigen Zufallsexperimente"
			},
			{
				name: "Erfolgswahrsch",
				description: "ist die Wahrscheinlichkeit für den günstigen Ausgang des Experiments"
			},
			{
				name: "Kumuliert",
				description: "ist der Wahrheitswert, der den Typ der Funktion bestimmt"
			}
		]
	},
	{
		name: "BINOM.VERT.BEREICH",
		description: "Gibt die Erfolgswahrscheinlichkeit eines Versuchsergebnisses als Binomialverteilung zurück.",
		arguments: [
			{
				name: "Versuche",
				description: "ist die Anzahl der unabhängigen Versuche"
			},
			{
				name: "Erfolgswahrscheinlichkeit",
				description: "ist die Erfolgswahrscheinlichkeit jedes Einzelversuchs"
			},
			{
				name: "Zahl_Erfolge",
				description: "ist die Anzahl der Erfolge bei den Versuchen"
			},
			{
				name: "Zahl2_Erfolge",
				description: "sofern angegeben gibt diese Funktion die Wahrscheinlichkeit zurück, dass die Anzahl der erfolgreichen Versuche zwischen Zahl_Erfolge und Zahl2_Erfolge liegt"
			}
		]
	},
	{
		name: "BINOMVERT",
		description: "Gibt Wahrscheinlichkeiten einer binomialverteilten Zufallsvariablen zurück.",
		arguments: [
			{
				name: "Zahl_Erfolge",
				description: "ist die Zahl der günstigen Ereignisse der Experimente"
			},
			{
				name: "Versuche",
				description: "ist die Zahl der unabhängigen Zufallsexperimente"
			},
			{
				name: "Erfolgswahrsch",
				description: "ist die Wahrscheinlichkeit für den günstigen Ausgang des Experiments"
			},
			{
				name: "Kumuliert",
				description: "ist der Wahrheitswert, der den Typ der Funktion bestimmt"
			}
		]
	},
	{
		name: "BITLVERSCHIEB",
		description: "Gibt eine Zahl zurück, der um Verschiebebetrag Bits nach links verschoben ist.",
		arguments: [
			{
				name: "Zahl",
				description: "ist die dezimale Darstellung der auszuwertenden Binärzahl"
			},
			{
				name: "Verschiebebetrag",
				description: "ist die Anzahl der Bits, um die Zahl nach links verschoben werden soll"
			}
		]
	},
	{
		name: "BITODER",
		description: "Gibt ein bitweises 'Oder' zweier Zahlen zurück.",
		arguments: [
			{
				name: "Zahl1",
				description: "ist die dezimale Darstellung der auszuwertenden Binärzahl"
			},
			{
				name: "Zahl2",
				description: "ist die dezimale Darstellung der auszuwertenden Binärzahl"
			}
		]
	},
	{
		name: "BITRVERSCHIEB",
		description: "Gibt ein Zahl zurück, der um Verschiebebetrag Bits nach rechts verschoben ist.",
		arguments: [
			{
				name: "Zahl",
				description: "ist die dezimale Darstellung der auszuwertenden Binärzahl"
			},
			{
				name: "Verschiebebetrag",
				description: "ist die Anzahl der Bits, um die Zahl nach rechts verschoben werden soll"
			}
		]
	},
	{
		name: "BITUND",
		description: "Gibt ein bitweises 'Und' zweier Zahlen zurück.",
		arguments: [
			{
				name: "Zahl1",
				description: "ist die dezimale Darstellung der auszuwertenden Binärzahl"
			},
			{
				name: "Zahl2",
				description: "ist die dezimale Darstellung der auszuwertenden Binärzahl"
			}
		]
	},
	{
		name: "BITXODER",
		description: "Gibt ein bitweises 'Ausschließliches Oder' zweier Zahlen zurück.",
		arguments: [
			{
				name: "Zahl1",
				description: "ist die dezimale Darstellung der auszuwertenden Binärzahl"
			},
			{
				name: "Zahl2",
				description: "ist die dezimale Darstellung der auszuwertenden Binärzahl"
			}
		]
	},
	{
		name: "BLATT",
		description: "Gibt die Blattnummer des Blatts zurück, auf das verwiesen wird.",
		arguments: [
			{
				name: "Wert",
				description: "ist der Name eines Blatts oder Bezugs, dessen Blattnummer zurückgegeben werden soll. Ohne Parameter wird die Nummer des Blatts, das die Funktion enthält, zurückgegeben"
			}
		]
	},
	{
		name: "BLÄTTER",
		description: "Gibt die Anzahl der Blätter in einem Bezug zurück.",
		arguments: [
			{
				name: "Bezug",
				description: "ist ein Bezug, für den die Anzahl der enthaltenen Blätter zurückgegeben werden soll. Ohne Parameter wird die Anzahl der Blätter in der Arbeitsmappe, die die Funktion enthält, zurückgegeben"
			}
		]
	},
	{
		name: "BOGENMASS",
		description: "Wandelt Grad in Bogenmaß (Radiant) um.",
		arguments: [
			{
				name: "Winkel",
				description: "ist ein in Grad gegebener Winkel, den Sie umwandeln möchten"
			}
		]
	},
	{
		name: "BRTEILJAHRE",
		description: "Wandelt die Anzahl der ganzen Tage zwischen Ausgangsdatum und Enddatum in Bruchteile von Jahren um.",
		arguments: [
			{
				name: "Ausgangsdatum",
				description: "ist die fortlaufende Zahl, die das Ausgangsdatum angibt"
			},
			{
				name: "Enddatum",
				description: "ist die fortlaufende Zahl, die das Enddatum angibt"
			},
			{
				name: "Basis",
				description: "gibt an, auf welcher Basis die Zinstage gezählt werden"
			}
		]
	},
	{
		name: "BW",
		description: "Gibt den Barwert einer Investition zurück: den heutigen Gesamtwert einer Reihe zukünftiger Zahlungen.",
		arguments: [
			{
				name: "Zins",
				description: "ist der Zinssatz pro Periode (Zahlungszeitraum). Verwenden Sie z.B. 6%/4 für Quartalszahlungen mit einem Zinssatz von 6%"
			},
			{
				name: "Zzr",
				description: "gibt an, über wie viele Perioden die jeweilige Annuität (Rente) gezahlt wird"
			},
			{
				name: "Rmz",
				description: "ist der Betrag (Annuität), der in jeder Periode gezahlt wird. Dieser Betrag bleibt während der Laufzeit konstant"
			},
			{
				name: "Zw",
				description: "ist der zukünftige Wert (Endwert) oder der Kassenbestand, den Sie nach der letzten Zahlung erreicht haben möchten"
			},
			{
				name: "F",
				description: "kann den Wert 0 oder 1 annehmen und gibt an, wann Zahlungen fällig sind (Fälligkeit): 1 = Zahlungen am Anfang einer Periode; 0 = Zahlungen am Ende einer Periode"
			}
		]
	},
	{
		name: "CHIINV",
		description: "Gibt Perzentile der Chi-Quadrat-Verteilung zurück, eine Zahl von einschließlich 0 bis einschließlich 1.",
		arguments: [
			{
				name: "Wahrsch",
				description: "ist die zur Chi-Quadrat-Verteilung gehörige Wahrscheinlichkeit"
			},
			{
				name: "Freiheitsgrade",
				description: "ist die Anzahl der Freiheitsgrade, eine Zahl größer oder gleich 1 und kleiner als 10^10"
			}
		]
	},
	{
		name: "CHIQU.INV",
		description: "Gibt Perzentile der linksseitigen Chi-Quadrat-Verteilung zurück, eine Zahl von einschließlich 0 bis einschließlich 1.",
		arguments: [
			{
				name: "Wahrsch",
				description: "ist die zur Chi-Quadrat-Verteilung gehörige Wahrscheinlichkeit"
			},
			{
				name: "Freiheitsgrade",
				description: "ist die Anzahl der Freiheitsgrade, eine Zahl größer oder gleich 1 und kleiner als 10^10"
			}
		]
	},
	{
		name: "CHIQU.INV.RE",
		description: "Gibt Perzentile der rechtsseitigen Chi-Quadrat-Verteilung zurück, eine Zahl von einschließlich 0 bis einschließlich 1.",
		arguments: [
			{
				name: "Wahrsch",
				description: "ist die zur Chi-Quadrat-Verteilung gehörige Wahrscheinlichkeit"
			},
			{
				name: "Freiheitsgrade",
				description: "ist die Anzahl der Freiheitsgrade, eine Zahl größer oder gleich 1 und kleiner als 10^10"
			}
		]
	},
	{
		name: "CHIQU.TEST",
		description: "Gibt die Teststatistik eines Chi-Quadrat-Unabhängigkeitstests zurück.",
		arguments: [
			{
				name: "Beob_Messwerte",
				description: "ist der Bereich beobachteter Daten, den Sie gegen die erwarteten Werte testen möchten"
			},
			{
				name: "Erwart_Werte",
				description: "ist der Bereich erwarteter Beobachtungen, die sich aus der Division der miteinander multiplizierten Rangsummen und der Gesamtsumme berechnen"
			}
		]
	},
	{
		name: "CHIQU.VERT",
		description: "Gibt Werte der linksseitigen Verteilungsfunktion (1-Alpha) einer Chi-Quadrat-verteilten Zufallsgröße zurück.",
		arguments: [
			{
				name: "x",
				description: "ist der Wert der Verteilung (Perzentil), ein nicht-negativer Wert, dessen Wahrscheinlichkeit Sie berechnen möchten"
			},
			{
				name: "Freiheitsgrade",
				description: "ist die Anzahl der Freiheitsgrade, eine Zahl größer oder gleich 1 und kleiner als 10^10"
			},
			{
				name: "kumuliert",
				description: "ist ein Wahrheitswert, den die Funktion zurückgeben soll: die kumulative Verteilungsfunktion = WAHR; die Wahrscheinlichkeitsmassenfunktion = FALSCH"
			}
		]
	},
	{
		name: "CHIQU.VERT.RE",
		description: "Gibt Werte der rechtsseitigen Verteilungsfunktion (1-Alpha) einer Chi-Quadrat-verteilten Zufallsgröße zurück.",
		arguments: [
			{
				name: "x",
				description: "ist der Wert der Verteilung (Perzentil), ein nicht-negativer Wert, dessen Wahrscheinlichkeit Sie berechnen möchten"
			},
			{
				name: "Freiheitsgrade",
				description: "ist die Anzahl der Freiheitsgrade, eine Zahl größer oder gleich 1 und kleiner als 10^10"
			}
		]
	},
	{
		name: "CHITEST",
		description: "Gibt die Teststatistik eines Chi-Quadrat-Unabhängigkeitstests zurück.",
		arguments: [
			{
				name: "Beob_Meßwerte",
				description: "ist der Bereich beobachteter Daten, den Sie gegen die erwarteten Werte testen möchten"
			},
			{
				name: "Erwart_Werte",
				description: "ist der Bereich erwarteter Beobachtungen, die sich aus der Division der miteinander multiplizierten Rangsummen und der Gesamtsumme berechnen"
			}
		]
	},
	{
		name: "CHIVERT",
		description: "Gibt Werte der Verteilungsfunktion (1-Alpha) einer Chi-Quadrat-verteilten Zufallsgröße zurück.",
		arguments: [
			{
				name: "x",
				description: "ist der Wert der Verteilung (Perzentil), ein nicht-negativer Wert, dessen Wahrscheinlichkeit Sie berechnen möchten"
			},
			{
				name: "Freiheitsgrade",
				description: "ist die Anzahl der Freiheitsgrade, eine Zahl größer oder gleich 1 und kleiner als 10^10"
			}
		]
	},
	{
		name: "CODE",
		description: "Gibt die Codezahl des ersten Zeichens in einem Text zurück (abhängig von Zeichensatz, der auf Ihrem Computer eingestellt ist).",
		arguments: [
			{
				name: "Text",
				description: "ist der Text, für den Sie die Codezahl des ersten Zeichens bestimmen möchten"
			}
		]
	},
	{
		name: "COS",
		description: "Gibt den Kosinus einer Zahl zurück.",
		arguments: [
			{
				name: "Zahl",
				description: "ist ein im Bogenmaß (Radiant) gegebener Winkel, dessen Kosinus Sie berechnen möchten"
			}
		]
	},
	{
		name: "COSEC",
		description: "Gibt den Kosekans eines Winkels zurück.",
		arguments: [
			{
				name: "Zahl",
				description: "ist der im Bogenmaß angegebene Winkel, dessen Kosekans Sie berechnen möchten"
			}
		]
	},
	{
		name: "COSECHYP",
		description: "Gibt den hyperbolischen Kosekans eines Winkels zurück.",
		arguments: [
			{
				name: "Zahl",
				description: "ist der im Bogenmaß angegebene Winkel, dessen hyperbolischen Kosekans Sie berechnen möchten"
			}
		]
	},
	{
		name: "COSHYP",
		description: "Gibt den hyperbolischen Kosinus einer Zahl zurück. .",
		arguments: [
			{
				name: "Zahl",
				description: "ist eine reelle Zahl"
			}
		]
	},
	{
		name: "COT",
		description: "Gibt den Kotangens eines Winkels zurück.",
		arguments: [
			{
				name: "Zahl",
				description: "ist der im Bogenmaß angegebene Winkel, dessen Kotangens Sie berechnen möchten"
			}
		]
	},
	{
		name: "COTHYP",
		description: "Gibt den hyperbolischen Kotangens einer Zahl zurück.",
		arguments: [
			{
				name: "Zahl",
				description: "ist der im Bogenmaß angegebene Winkel, dessen hyperbolischen Kotangens Sie berechnen möchten"
			}
		]
	},
	{
		name: "DATEDIF",
		description: "",
		arguments: [
		]
	},
	{
		name: "DATUM",
		description: "Gibt die fortlaufende Zahl des jeweils angegebenen Datums zurück.",
		arguments: [
			{
				name: "Jahr",
				description: "ist eine ganze Zahl zwischen 1900 und 2078 in Spreadsheet für Windows und zwischen 1904 und 2078 in Spreadsheet für den Macintosh"
			},
			{
				name: "Monat",
				description: "ist eine ganze Zahl zwischen 1 und 12, die für den Monat des Jahres steht"
			},
			{
				name: "Tag",
				description: "ist eine ganze Zahl zwischen 1 und 31, die für den Tag des Monats steht"
			}
		]
	},
	{
		name: "DATWERT",
		description: "Wandelt ein als Text vorliegendes Datum in eine fortlaufende Zahl um.",
		arguments: [
			{
				name: "Datumstext",
				description: "ist ein in Anführungszeichen eingeschlossener Text, der ein Datum in einem Spreadsheet-Datumsformat wiedergibt (zwischen 1.1.1900 (Windows) oder 1.1.1904 (Macintosh) and 31.12./9999)"
			}
		]
	},
	{
		name: "DBANZAHL",
		description: "Zählt die Zellen in einem Feld (Spalte) einer Datenbank, deren Inhalte mit den Suchkriterien übereinstimmen.",
		arguments: [
			{
				name: "Datenbank",
				description: "ist der Zellbereich, der die Datenbank bestimmt"
			},
			{
				name: "Datenbankfeld",
				description: "gibt an, welches Datenbankfeld in der Funktion benutzt wird"
			},
			{
				name: "Suchkriterien",
				description: "ist der Zellbereich, der die Suchkriterien enthält. Der Bereich besteht aus der Spaltenbeschriftung und der Zelle darunter, in der sich die Bedingung befindet"
			}
		]
	},
	{
		name: "DBANZAHL2",
		description: "Zählt die Zellen einer Datenbank, deren Inhalte mit den Suchkriterien übereinstimmen und die nicht leer sind.",
		arguments: [
			{
				name: "Datenbank",
				description: "ist der Zellbereich, der die Datenbank bestimmt"
			},
			{
				name: "Datenbankfeld",
				description: "gibt an, welches Datenbankfeld in der Funktion benutzt wird"
			},
			{
				name: "Suchkriterien",
				description: "ist der Zellbereich, der die Suchkriterien enthält. Der Bereich besteht aus der Spaltenbeschriftung und der Zelle darunter, in der sich die Bedingung befindet"
			}
		]
	},
	{
		name: "DBAUSZUG",
		description: "Gibt den Datensatz in einer Datenbank zurück, der mit den angegebenen Suchkriterien übereinstimmt.",
		arguments: [
			{
				name: "Datenbank",
				description: "ist der Zellbereich, aus dem die Datenbank besteht"
			},
			{
				name: "Datenbankfeld",
				description: "gibt an, welches Datenbankfeld in der Funktion benutzt wird"
			},
			{
				name: "Suchkriterien",
				description: "ist der Zellbereich, der die Suchkriterien enthält. Der Bereich besteht aus der Spaltenbeschriftung und der Zelle darunter, in der sich die Bedingung befindet"
			}
		]
	},
	{
		name: "DBMAX",
		description: "Gibt den größten Wert aus den ausgewählten Datenbankeinträgen zurück.",
		arguments: [
			{
				name: "Datenbank",
				description: "ist der Zellbereich, aus dem die Datenbank besteht"
			},
			{
				name: "Datenbankfeld",
				description: "gibt an, welches Datenbankfeld in der Funktion benutzt wird"
			},
			{
				name: "Suchkriterien",
				description: "ist der Zellbereich, der die Suchkriterien enthält. Der Bereich besteht aus der Spaltenbeschriftung und der Zelle darunter, in der sich die Bedingung befindet"
			}
		]
	},
	{
		name: "DBMIN",
		description: "Gibt den kleinsten Wert aus den ausgewählten Datenbankeinträgen zurück.",
		arguments: [
			{
				name: "Datenbank",
				description: "ist der Zellbereich, aus dem die Datenbank besteht"
			},
			{
				name: "Datenbankfeld",
				description: "gibt an, welches Datenbankfeld in der Funktion benutzt wird"
			},
			{
				name: "Suchkriterien",
				description: "ist der Zellbereich, der die Suchkriterien enthält. Der Bereich besteht aus der Spaltenbeschriftung und der Zelle darunter, in der sich die Bedingung befindet"
			}
		]
	},
	{
		name: "DBMITTELWERT",
		description: "Gibt den Mittelwert aus den ausgewählten Datenbankeinträgen zurück.",
		arguments: [
			{
				name: "Datenbank",
				description: "ist der Zellbereich, aus dem sich die Datenbank zusammensetzt"
			},
			{
				name: "Datenbankfeld",
				description: "gibt an, welches Datenbankfeld in der Funktion benutzt wird"
			},
			{
				name: "Suchkriterien",
				description: "ist der Zellbereich, der die Suchkriterien enthält. Der Bereich besteht aus der Spaltenbeschriftung und der Zelle darunter, in der sich die Bedingung befindet"
			}
		]
	},
	{
		name: "DBPRODUKT",
		description: "Multipliziert die Werte eines bestimmten Felds der Datensätze, die innerhalb einer Datenbank mit den Suchkriterien übereinstimmen.",
		arguments: [
			{
				name: "Datenbank",
				description: "ist der Zellbereich, aus dem die Datenbank besteht"
			},
			{
				name: "Datenbankfeld",
				description: "gibt an, welches Datenbankfeld in der Funktion benutzt wird"
			},
			{
				name: "Suchkriterien",
				description: "ist der Zellbereich, der die Suchkriterien enthält. Der Bereich besteht aus der Spaltenbeschriftung und der Zelle darunter, in der sich die Bedingung befindet"
			}
		]
	},
	{
		name: "DBSTDABW",
		description: "Schätzt die Standardabweichung, ausgehend von einer Stichprobe aus bestimmten Datenbankeinträgen.",
		arguments: [
			{
				name: "Datenbank",
				description: "ist der Zellbereich, aus dem die Datenbank besteht"
			},
			{
				name: "Datenbankfeld",
				description: "gibt an, welches Datenbankfeld in der Funktion benutzt wird"
			},
			{
				name: "Suchkriterien",
				description: "ist der Zellbereich, der die Suchkriterien enthält. Der Bereich besteht aus der Spaltenbeschriftung und der Zelle darunter, in der sich die Bedingung befindet"
			}
		]
	},
	{
		name: "DBSTDABWN",
		description: "Berechnet die Standardabweichung, ausgehend von der Grundgesamtheit aus bestimmten Datenbankeinträgen.",
		arguments: [
			{
				name: "Datenbank",
				description: "ist der Zellbereich, aus dem die Datenbank besteht"
			},
			{
				name: "Datenbankfeld",
				description: "gibt an, welches Datenbankfeld in der Funktion benutzt wird"
			},
			{
				name: "Suchkriterien",
				description: "ist der Zellbereich, der die Suchkriterien enthält. Der Bereich besteht aus der Spaltenbeschriftung und der Zelle darunter, in der sich die Bedingung befindet"
			}
		]
	},
	{
		name: "DBSUMME",
		description: "Summiert Zahlen, die in einer Datenbank abgelegt sind.",
		arguments: [
			{
				name: "Datenbank",
				description: "ist der Zellbereich, aus dem die Datenbank besteht"
			},
			{
				name: "Datenbankfeld",
				description: "gibt an, welches Datenbankfeld in der Funktion benutzt wird"
			},
			{
				name: "Suchkriterien",
				description: "ist der Zellbereich, der die Suchkriterien enthält. Der Bereich besteht aus der Spaltenbeschriftung und der Zelle darunter, in der sich die Bedingung befindet"
			}
		]
	},
	{
		name: "DBVARIANZ",
		description: "Schätzt die Varianz, ausgehend von einer Stichprobe aus bestimmten Datenbankeinträgen.",
		arguments: [
			{
				name: "Datenbank",
				description: "ist der Zellbereich, aus dem die Datenbank besteht"
			},
			{
				name: "Datenbankfeld",
				description: "gibt an, welches Datenbankfeld in der Funktion benutzt wird"
			},
			{
				name: "Suchkriterien",
				description: "ist der Zellbereich, der die Suchkriterien enthält. Der Bereich besteht aus der Spaltenbeschriftung und der Zelle darunter, in der sich die Bedingung befindet"
			}
		]
	},
	{
		name: "DBVARIANZEN",
		description: "Berechnet die Varianz, ausgehend von der Grundgesamtheit aus bestimmten Datenbankeinträgen.",
		arguments: [
			{
				name: "Datenbank",
				description: "ist der Zellbereich, aus dem die Datenbank besteht"
			},
			{
				name: "Datenbankfeld",
				description: "gibt an, welches Datenbankfeld in der Funktion benutzt wird"
			},
			{
				name: "Suchkriterien",
				description: "ist der Zellbereich, der die Suchkriterien enthält. Der Bereich besteht aus der Spaltenbeschriftung und der Zelle darunter, in der sich die Bedingung befindet"
			}
		]
	},
	{
		name: "DELTA",
		description: "Überprüft, ob zwei Werte gleich sind.",
		arguments: [
			{
				name: "Zahl1",
				description: "ist die erste Zahl"
			},
			{
				name: "Zahl2",
				description: "ist die zweite Zahl"
			}
		]
	},
	{
		name: "DEZIMAL",
		description: "Konvertiert eine Textdarstellung einer Zahl mit einer angegebenen Basis in eine Dezimalzahl.",
		arguments: [
			{
				name: "Zahl",
				description: "ist die zu konvertierende Zahl"
			},
			{
				name: "Basis",
				description: "ist die Basis der zu konvertierenden Zahl"
			}
		]
	},
	{
		name: "DEZINBIN",
		description: "Wandelt eine dezimale Zahl in eine binäre Zahl (Dualzahl) um.",
		arguments: [
			{
				name: "Zahl",
				description: "ist die dezimale ganze Zahl, die Sie umwandeln möchten"
			},
			{
				name: "Stellen",
				description: "ist die Zahl der verwendeten Stellen"
			}
		]
	},
	{
		name: "DEZINHEX",
		description: "Wandelt eine dezimale Zahl in eine hexadezimale Zahl um.",
		arguments: [
			{
				name: "Zahl",
				description: "ist die dezimale ganze Zahl, die Sie umwandeln möchten"
			},
			{
				name: "Stellen",
				description: "ist die Zahl der verwendeten Stellen"
			}
		]
	},
	{
		name: "DEZINOKT",
		description: "Wandelt eine dezimale Zahl in eine oktale Zahl um.",
		arguments: [
			{
				name: "Zahl",
				description: "ist die dezimale ganze Zahl, die Sie umwandeln möchten"
			},
			{
				name: "Stellen",
				description: "ist die Zahl der verwendeten Stellen"
			}
		]
	},
	{
		name: "DIA",
		description: "Gibt die arithmetisch-degressive Abschreibung eines Wirtschaftsguts für eine bestimmte Periode zurück.",
		arguments: [
			{
				name: "Ansch_Wert",
				description: "sind die Anschaffungskosten eines Wirtschaftsguts"
			},
			{
				name: "Restwert",
				description: "ist der Restwert am Ende der Nutzungsdauer (wird häufig auch als Schrottwert bezeichnet)"
			},
			{
				name: "Nutzungsdauer",
				description: "ist die Anzahl der Perioden, über die das Wirtschaftsgut abgeschrieben wird (auch als Nutzungsdauer bezeichnet)"
			},
			{
				name: "Zr",
				description: "ist die Periode und muss dieselbe Zeiteinheit verwenden wie die Nutzungsdauer"
			}
		]
	},
	{
		name: "DISAGIO",
		description: "Gibt den in Prozent ausgedrückten Abschlag (Disagio) eines Wertpapiers zurück.",
		arguments: [
			{
				name: "Abrechnung",
				description: "ist der Abrechnungstermin des Wertpapierkaufs, als fortlaufende Zahl angegeben"
			},
			{
				name: "Fälligkeit",
				description: "ist der Fälligkeitstermin des Wertpapiers, als fortlaufende Zahl angegeben"
			},
			{
				name: "Kurs",
				description: "ist der Kurs des Wertpapiers pro 100 EUR Nennwert"
			},
			{
				name: "Rückzahlung",
				description: "ist der Rückzahlungswert des Wertpapiers pro 100 EUR Nennwert"
			},
			{
				name: "Basis",
				description: "gibt an, auf welcher Basis die Zinstage gezählt werden"
			}
		]
	},
	{
		name: "DM",
		description: "Wandelt eine Zahl in einen Text im Währungsformat um.",
		arguments: [
			{
				name: "Zahl",
				description: "ist eine Zahl, ein Bezug auf eine Zelle, die eine Zahl enthält, oder eine Formel, die eine Zahl berechnet"
			},
			{
				name: "Dezimalstellen",
				description: "ist die Anzahl der Ziffern rechts vom Dezimalkomma. Wenn nötig, wird die Zahl gerundet. Wenn der Parameter fehlt, werden 2 Dezimalstellen zurückgegeben"
			}
		]
	},
	{
		name: "EDATUM",
		description: "Gibt die fortlaufende Zahl des Datums zurück, das eine bestimmte Anzahl von Monaten vor bzw. nach dem Ausgangsdatum liegt.",
		arguments: [
			{
				name: "Ausgangsdatum",
				description: "ist die fortlaufende Zahl, die das Ausgangsdatum darstellt"
			},
			{
				name: "Monate",
				description: "ist die Anzahl der Monate vor oder nach Ausgangsdatum"
			}
		]
	},
	{
		name: "EFFEKTIV",
		description: "Gibt die jährliche Effektivverzinsung zurück.",
		arguments: [
			{
				name: "Nominalzins",
				description: "ist die Nominalverzinsung"
			},
			{
				name: "Perioden",
				description: "ist die Anzahl der Zinszahlungen pro Jahr"
			}
		]
	},
	{
		name: "ERSETZEN",
		description: "Ersetzt eine bestimmte Anzahl Zeichen ab einer bestimmten Stelle innerhalb eines Textes.",
		arguments: [
			{
				name: "Alter_Text",
				description: "ist der Text, in dem Sie eine Anzahl von Zeichen ersetzen möchten"
			},
			{
				name: "Erstes_Zeichen",
				description: "ist die Position des Zeichens in Alter_Text, an der mit dem Ersetzen durch Neuer_Text begonnen werden soll"
			},
			{
				name: "Anzahl_Zeichen",
				description: "ist die Anzahl der Zeichen in Alter_Text, angefangen mit Beginn, die Sie durch Neuer_Text ersetzen wollen"
			},
			{
				name: "Neuer_Text",
				description: "ist der Text, mit dem Sie Anzahl_Zeichen in Alter_Text ersetzen wollen"
			}
		]
	},
	{
		name: "EXP",
		description: "Potenziert die Basis e mit der als Argument angegebenen Zahl.",
		arguments: [
			{
				name: "Zahl",
				description: "ist der Exponent zur Basis e. Die Konstante e entspricht 2.71828182845904, der Basis der natürlichen Logarithmen"
			}
		]
	},
	{
		name: "EXPON.VERT",
		description: "Gibt Wahrscheinlichkeiten einer exponentialverteilten Zufallsvariablen zurück.",
		arguments: [
			{
				name: "x",
				description: "ist der Wert der Verteilung (Perzentil), dessen Wahrscheinlichkeit Sie berechnen möchten"
			},
			{
				name: "Lambda",
				description: "ist der Parameter der Verteilung"
			},
			{
				name: "Kumuliert",
				description: "ist der Wahrheitswert, der den Typ der Funktion bestimmt"
			}
		]
	},
	{
		name: "EXPONVERT",
		description: "Gibt Wahrscheinlichkeiten einer exponentialverteilten Zufallsvariablen zurück.",
		arguments: [
			{
				name: "x",
				description: "ist der Wert der Verteilung (Perzentil), dessen Wahrscheinlichkeit Sie berechnen möchten"
			},
			{
				name: "Lambda",
				description: "ist der Parameter der Verteilung"
			},
			{
				name: "Kumuliert",
				description: "ist der Wahrheitswert, der den Typ der Funktion bestimmt"
			}
		]
	},
	{
		name: "F.INV",
		description: "Gibt Perzentile der (linksseitigen) F-Verteilung zurück: wenn p = F.VERT(x,...), dann FINV(p,...) = x.",
		arguments: [
			{
				name: "Wahrsch",
				description: "ist eine der kumulierten F-Verteilung zugeordnete Wahrscheinlichkeit, eine Zahl zwischen 0 und 1 einschließlich"
			},
			{
				name: "Freiheitsgrade1",
				description: "ist die Anzahl der Freiheitsgrade im Zähler, eine Zahl größer oder gleich 1 und kleiner als 10^10"
			},
			{
				name: "Freiheitsgrade2",
				description: "ist die Anzahl der Freiheitsgrade im Nenner, eine Zahl größer als oder gleich 1 und kleiner als 10^10"
			}
		]
	},
	{
		name: "F.INV.RE",
		description: "Gibt Perzentile der (rechtsseitigen) F-Verteilung zurück: wenn p = F.VERT.RE(x,...), dann F.INV.RE(p,...) = x.",
		arguments: [
			{
				name: "Wahrsch",
				description: "ist eine der kumulierten F-Verteilung zugeordnete Wahrscheinlichkeit, eine Zahl zwischen 0 und 1 einschließlich"
			},
			{
				name: "Freiheitsgrade1",
				description: "ist die Anzahl der Freiheitsgrade im Zähler, eine Zahl größer oder gleich 1 und kleiner als 10^10"
			},
			{
				name: "Freiheitsgrade2",
				description: "ist die Anzahl der Freiheitsgrade im Nenner, eine Zahl größer als oder gleich 1 und kleiner als 10^10"
			}
		]
	},
	{
		name: "F.TEST",
		description: "Gibt die Teststatistik eines F-Tests zurück, die zweiseitige Wahrscheinlichkeit darstellt, dass sich die Varianzen in Matrix1 und Matrix2 nicht signifikant unterscheiden.",
		arguments: [
			{
				name: "Matrix1",
				description: "ist die erste Matrix oder der erste Wertebereich, Argumente können Zahlen, Namen, Arrays oder Bezüge sein (Leerzellen werden ignoriert)"
			},
			{
				name: "Matrix2",
				description: "ist die zweite Matrix oder der zweite Wertebereich, Argumente können Zahlen, Namen, Arrays oder Bezüge sein (Leerzellen werden ignoriert)"
			}
		]
	},
	{
		name: "F.VERT",
		description: "Gibt Werte der Verteilungsfunktion (1-Alpha) einer (linksseitigen) F-verteilten Zufallsvariablen zurück.",
		arguments: [
			{
				name: "x",
				description: "ist der Wert der Verteilung (Perzentil), dessen Wahrscheinlichkeit Sie berechnen möchten, eine nicht-negative Zahl"
			},
			{
				name: "Freiheitsgrade1",
				description: "ist die Anzahl der Freiheitsgrade im Zähler , eine Zahl größer oder gleich 1 und kleiner als 10^10"
			},
			{
				name: "Freiheitsgrade2",
				description: "ist die Anzahl der Freiheitsgrade im Nenner, eine Zahl größer oder gleich 1 und kleiner als 10^10"
			},
			{
				name: "kumuliert",
				description: "ist ein Wahrheitswert, den die Funktion zurückgeben soll: die kumulative Verteilungsfunktion = WAHR; die Wahrscheinlichkeitsmassenfunktion = FALSCH"
			}
		]
	},
	{
		name: "F.VERT.RE",
		description: "Gibt Werte der Verteilungsfunktion (1-Alpha) einer (rechtsseitigen) F-verteilten Zufallsvariablen zurück.",
		arguments: [
			{
				name: "x",
				description: "ist der Wert der Verteilung (Perzentil), dessen Wahrscheinlichkeit Sie berechnen möchten, eine nicht-negative Zahl"
			},
			{
				name: "Freiheitsgrade1",
				description: "ist die Anzahl der Freiheitsgrade im Zähler , eine Zahl größer oder gleich 1 und kleiner als 10^10"
			},
			{
				name: "Freiheitsgrade2",
				description: "ist die Anzahl der Freiheitsgrade im Nenner, , eine Zahl größer oder gleich 1 und kleiner als 10^10"
			}
		]
	},
	{
		name: "FAKULTÄT",
		description: "Gibt die Fakultät einer Zahl zurück (Fakultät n = 1*2*3...*n).",
		arguments: [
			{
				name: "Zahl",
				description: "ist eine nicht negative Zahl, deren Fakultät Sie berechnen wollen"
			}
		]
	},
	{
		name: "FALSCH",
		description: "Gibt den Wahrheitswert FALSCH zurück.",
		arguments: [
		]
	},
	{
		name: "FEHLER.TYP",
		description: "Gibt eine Zahl entsprechend dem vorliegenden Fehlerwert zurück.",
		arguments: [
			{
				name: "Fehlerwert",
				description: "ist der Fehlerwert, dessen Kennummer Sie finden möchten"
			}
		]
	},
	{
		name: "FEST",
		description: "Formatiert eine Zahl als Text mit einer festen Anzahl an Nachkommastellen.",
		arguments: [
			{
				name: "Zahl",
				description: "ist die Zahl, die Sie auf- oder abrunden und in Text umwandeln möchten"
			},
			{
				name: "Dezimalstellen",
				description: "ist die Anzahl der Ziffern rechts vom Dezimalkomma. Wenn der Parameter fehlt, werden 2 Dezimalstellen zurückgegeben"
			},
			{
				name: "Keine_Punkte",
				description: "ist ein Wahrheitswert, der wenn WAHR, die Funktion FEST daran hindert, Punkte (1.000er-Trennzeichen) bei der Wiedergabe des Textes zu benutzen"
			}
		]
	},
	{
		name: "FIELD",
		description: "",
		arguments: [
			{
				name: "",
				description: ""
			}
		]
	},
	{
		name: "FIELDPICTURE",
		description: "",
		arguments: [
			{
				name: "",
				description: ""
			},
			{
				name: "",
				description: ""
			},
			{
				name: "",
				description: ""
			},
			{
				name: "",
				description: ""
			},
			{
				name: "",
				description: ""
			},
			{
				name: "",
				description: ""
			},
			{
				name: "",
				description: ""
			},
			{
				name: "",
				description: ""
			}
		]
	},
	{
		name: "FINDEN",
		description: "Sucht eine Zeichenfolge innerhalb einer anderen (Groß-/Kleinschreibung wird beachtet).",
		arguments: [
			{
				name: "Suchtext",
				description: "ist der Text, den Sie finden wollen. Verwenden Sie zwei Anführungszeichen (leerer Text), um nach dem ersten Zeichen im Suchtext zu suchen. Stellvertreterzeichen sind nicht zulässig"
			},
			{
				name: "Text",
				description: "ist der Text, in dem Suchtext gesucht werden soll"
			},
			{
				name: "Erstes_Zeichen",
				description: "gibt an, bei welchem Zeichen die Suche begonnen werden soll. Wenn der Parameter fehlt, wird 1 angenommen"
			}
		]
	},
	{
		name: "FINV",
		description: "Gibt Perzentile der rechtsseitigen F-Verteilung zurück.",
		arguments: [
			{
				name: "Wahrsch",
				description: "ist die zur F-Verteilung gehörige Wahrscheinlichkeit, eine Zahl von einschließlich 0 bis einschließlich 1"
			},
			{
				name: "Freiheitsgrade1",
				description: "ist die Anzahl der Freiheitsgrade im Zähler, eine Zahl größer oder gleich 1 und kleiner als 10^10"
			},
			{
				name: "Freiheitsgrade2",
				description: "ist die Anzahl der Freiheitsgrade im Nenner, eine Zahl größer oder gleich 1 und kleiner als 10^10"
			}
		]
	},
	{
		name: "FISHER",
		description: "Gibt die Fisher-Transformation zurück.",
		arguments: [
			{
				name: "x",
				description: "ist der numerische Wert, grösser als -1 und kleiner als 1, für den Sie die Transformation durchführen möchten"
			}
		]
	},
	{
		name: "FISHERINV",
		description: "Gibt die Umkehrung der Fisher-Transformation zurück.",
		arguments: [
			{
				name: "y",
				description: "ist der Wert, dessen Transformation Sie umkehren möchten"
			}
		]
	},
	{
		name: "FORMELTEXT",
		description: "Gibt eine Formel als Zeichenfolge zurück.",
		arguments: [
			{
				name: "Bezug",
				description: "ist ein Bezug auf eine Formel"
			}
		]
	},
	{
		name: "FTEST",
		description: "Gibt die Teststatistik eines F-Tests zurück, die zweiseitige Wahrscheinlichkeit darstellt, dass sich die Varianzen in Matrix1 und Matrix2 nicht signifikant unterscheiden.",
		arguments: [
			{
				name: "Matrix1",
				description: "ist die erste Matrix oder der erste Wertebereich, Argumente können Zahlen, Namen, Arrays oder Bezüge sein (Leerzellen werden ignoriert)"
			},
			{
				name: "Matrix2",
				description: "ist die zweite Matrix oder der zweite Wertebereich, Argumente können Zahlen, Namen, Arrays oder Bezüge sein (Leerzellen werden ignoriert)"
			}
		]
	},
	{
		name: "FVERT",
		description: "Gibt Werte der Verteilungsfunktion (1-Alpha) einer rechtsseitigen F-verteilten Zufallsvariablen zurück.",
		arguments: [
			{
				name: "x",
				description: "ist der Wert der Verteilung (Perzentil), dessen Wahrscheinlichkeit Sie berechnen möchten, eine nicht-negative Zahl"
			},
			{
				name: "Freiheitsgrade1",
				description: "ist die Anzahl der Freiheitsgrade im Zähler , eine Zahl größer oder gleich 1 und kleiner als 10^10"
			},
			{
				name: "Freiheitsgrade2",
				description: "ist die Anzahl der Freiheitsgrade im Nenner, , eine Zahl größer oder gleich 1 und kleiner als 10^10"
			}
		]
	},
	{
		name: "G.TEST",
		description: "Gibt die einseitige Prüfstatistik für einen Gaußtest (Normalverteilung) zurück.",
		arguments: [
			{
				name: "Matrix",
				description: "ist die Matrix oder der Datenbereich, gegen die/den Sie x testen möchten"
			},
			{
				name: "x",
				description: "ist der zu testende Wert"
			},
			{
				name: "Sigma",
				description: "ist die bekannte Standardabweichung der Grundgesamtheit. Ohne Angabe wird die Beispielstandardabweichung verwendet"
			}
		]
	},
	{
		name: "GAMMA",
		description: "Gibt den Wert der Gammafunktion zurück.",
		arguments: [
			{
				name: "x",
				description: "ist der Wert, für den Gamma berechnet werden soll"
			}
		]
	},
	{
		name: "GAMMA.INV",
		description: "Gibt den Kehrwert der kumulierten Gammaverteilung zurück: wenn p = GAMMA.VERT(x,...), dann GAMMA.INV(p,...) = x.",
		arguments: [
			{
				name: "Wahrsch",
				description: "ist die der Gammaverteilung zugeordnete Wahrscheinlichkeit, eine Zahl zwischen 0 und 1, einschließlich"
			},
			{
				name: "Alpha",
				description: "ist ein Parameter für die Verteilung, eine positive Zahl"
			},
			{
				name: "Beta",
				description: "ist ein Parameter für die Verteilung, eine positive Zahl. Wenn Beta = 1, gibt GAMMA.INV den Kehrwert der Gammanormalverteilung zurück"
			}
		]
	},
	{
		name: "GAMMA.VERT",
		description: "Gibt Wahrscheinlichkeiten einer gammaverteilten Zufallsvariablen zurück.",
		arguments: [
			{
				name: "x",
				description: "ist der Wert, dessen Wahrscheinlichkeit berechnet werden soll, eine nicht negative Zahl"
			},
			{
				name: "Alpha",
				description: "ist ein Parameter der Verteilung, eine positive Zahl"
			},
			{
				name: "Beta",
				description: "ist ein Parameter der Verteilung, eine positive Zahl. Wenn Beta = 1, liefert GAMMA.VERT die Standard-Gammaverteilung"
			},
			{
				name: "Kumuliert",
				description: "ist ein Wahrheitswert: kumulierte Verteilungsfunktion zurückgeben = WAHR; Wahrscheinlichkeits-Mengenfunktion = FALSCH oder auslassen"
			}
		]
	},
	{
		name: "GAMMAINV",
		description: "Gibt Perzentile der Gammaverteilung zurück.",
		arguments: [
			{
				name: "Wahrsch",
				description: "ist die zur Gamma-Verteilung gehörige Wahrscheinlichkeit, eine Zahl von einschließlich 0 bis einschließlich 1"
			},
			{
				name: "Alpha",
				description: "ist ein Parameter der Verteilung, eine positive Zahl"
			},
			{
				name: "Beta",
				description: "ist ein Parameter der Verteilung, eine positive Zahl. Wenn Beta = 1, liefert GAMMAINV die Standard-Gammaverteilung"
			}
		]
	},
	{
		name: "GAMMALN",
		description: "Gibt den natürlichen Logarithmus der Gammafunktion zurück.",
		arguments: [
			{
				name: "x",
				description: "ist der Wert, für den Sie GAMMALN berechnen möchten"
			}
		]
	},
	{
		name: "GAMMALN.GENAU",
		description: "Gibt den natürlichen Logarithmus der Gammafunktion zurück.",
		arguments: [
			{
				name: "x",
				description: "ist der Wert, für den Sie GAMMALN.GENAU berechnen möchten"
			}
		]
	},
	{
		name: "GAMMAVERT",
		description: "Gibt Wahrscheinlichkeiten einer gammaverteilten Zufallsvariablen zurück.",
		arguments: [
			{
				name: "x",
				description: "ist der Wert (Perzentil), dessen Wahrscheinlichkeit (1-Alpha) Sie berechnen wollen"
			},
			{
				name: "Alpha",
				description: "ist ein Parameter der Verteilung"
			},
			{
				name: "Beta",
				description: "ist ein Parameter der Verteilung. Wenn Beta = 1, liefert GAMMAVERT die Standard-Gammaverteilung"
			},
			{
				name: "Kumuliert",
				description: "ist der Wahrheitswert, der den Typ der Funktion bestimmt"
			}
		]
	},
	{
		name: "GANZZAHL",
		description: "Rundet eine Zahl auf die nächstkleinere ganze Zahl ab.",
		arguments: [
			{
				name: "Zahl",
				description: "ist die reelle Zahl, die Sie zur nächsten ganzen Zahl abrunden möchten"
			}
		]
	},
	{
		name: "GAUSS",
		description: "",
		arguments: [
		]
	},
	{
		name: "GAUSSF.GENAU",
		description: "Gibt die Gauss'sche Fehlerfunktion zurück.",
		arguments: [
			{
				name: "X",
				description: "ist die untere Grenze für die Integration von GAUSSF.GENAU"
			}
		]
	},
	{
		name: "GAUSSFEHLER",
		description: "Gibt die Gauss'sche Fehlerfunktion zurück.",
		arguments: [
			{
				name: "Untere_Grenze",
				description: "ist die untere Grenze für die Integration von GAUSSFEHLER"
			},
			{
				name: "Obere_Grenze",
				description: "ist die obere Grenze für die Integration von GAUSSFEHLER"
			}
		]
	},
	{
		name: "GAUSSFKOMPL",
		description: "Gibt das Komplement zur Gauss'schen Fehlerfunktion zurück.",
		arguments: [
			{
				name: "Untere_Grenze",
				description: "ist die untere Grenze für die Integration von GAUSSFEHLER"
			}
		]
	},
	{
		name: "GAUSSFKOMPL.GENAU",
		description: "Gibt das Komplement zur Gauss'schen Fehlerfunktion zurück.",
		arguments: [
			{
				name: "X",
				description: "ist die untere Grenze für die Integration von GAUSSFKOMPL.GENAU"
			}
		]
	},
	{
		name: "GDA",
		description: "Gibt die degressive Doppelraten-Abschreibung eines Wirtschaftsguts für eine bestimmte Periode zurück.",
		arguments: [
			{
				name: "Ansch_Wert",
				description: "sind die Anschaffungskosten eines Wirtschaftsguts"
			},
			{
				name: "Restwert",
				description: "ist der Restwert am Ende der Nutzungsdauer (wird häufig auch als Schrottwert bezeichnet)"
			},
			{
				name: "Nutzungsdauer",
				description: "ist die Anzahl der Perioden, über die das Wirtschaftsgut abgeschrieben wird (auch als Nutzungsdauer bezeichnet)"
			},
			{
				name: "Periode",
				description: "ist die Periode, deren Abschreibungsbetrag Sie berechnen möchten. Für das Argument Periode muss dieselbe Zeiteinheit wie für die Nutzungsdauer verwendet werden"
			},
			{
				name: "Faktor",
				description: "ist die Rate, um die der Restbuchwert abnimmt ( Faktor steht für Faktor * 100 % / Nutzungsdauer)"
			}
		]
	},
	{
		name: "GDA2",
		description: "Gibt die geometrisch-degressive Abschreibung eines Wirtschaftsguts für eine bestimmte Periode zurück.",
		arguments: [
			{
				name: "Ansch_Wert",
				description: "sind die Anschaffungskosten eines Wirtschaftsguts"
			},
			{
				name: "Restwert",
				description: "ist der Restwert am Ende der Nutzungsdauer (wird häufig auch als Schrottwert bezeichnet)"
			},
			{
				name: "Nutzungsdauer",
				description: "ist die Anzahl der Perioden, über die das Wirtschaftsgut abgeschrieben wird (auch als Nutzungsdauer bezeichnet)"
			},
			{
				name: "Periode",
				description: "ist die Periode, deren Abschreibungsbetrag Sie berechnen möchten. Für das Argument Periode muss dieselbe Zeiteinheit wie für die Nutzungsdauer verwendet werden"
			},
			{
				name: "Monate",
				description: "ist die Anzahl der Monate im ersten Jahr. Fehlt das Argument Monate, wird 12 vorausgesetzt"
			}
		]
	},
	{
		name: "GEOMITTEL",
		description: "Gibt das geometrische Mittel eines Arrays oder Bereichs von positiven numerischen Daten zurück.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "Zahl1",
				description: "sind 1 bis 255 Zahlen oder Namen, Arrays oder Bezüge, die Zahlen enthalten, deren Mittel Sie berechnen möchten"
			},
			{
				name: "Zahl2",
				description: "sind 1 bis 255 Zahlen oder Namen, Arrays oder Bezüge, die Zahlen enthalten, deren Mittel Sie berechnen möchten"
			}
		]
	},
	{
		name: "GERADE",
		description: "Rundet eine positive Zahl auf die nächste gerade ganze Zahl auf und eine negative Zahl auf die nächste gerade ganze Zahl ab.",
		arguments: [
			{
				name: "Zahl",
				description: "ist der Wert, den Sie runden möchten"
			}
		]
	},
	{
		name: "GESTUTZTMITTEL",
		description: "Gibt den Mittelwert einer Datengruppe, ohne seine Werte an den Rändern zurück.",
		arguments: [
			{
				name: "Matrix",
				description: "ist eine Matrix oder Gruppe von Werten, die ohne ihre Ausreißer gemittelt wird"
			},
			{
				name: "Prozent",
				description: "ist der Prozentsatz der Datenpunkte, die nicht in die Bewertung eingehen sollen"
			}
		]
	},
	{
		name: "GGANZZAHL",
		description: "Überprüft, ob eine Zahl größer als ein gegebener Schwellenwert ist.",
		arguments: [
			{
				name: "Zahl",
				description: "ist der an Schritt zu überprüfende Wert"
			},
			{
				name: "Schritt",
				description: "ist der Schwellenwert"
			}
		]
	},
	{
		name: "GGT",
		description: "Gibt den größten gemeinsamen Teiler zurück.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "Zahl1",
				description: "sind 1 bis 255 Werte"
			},
			{
				name: "Zahl2",
				description: "sind 1 bis 255 Werte"
			}
		]
	},
	{
		name: "GLÄTTEN",
		description: "Löscht Leerzeichen in einem Text.",
		arguments: [
			{
				name: "Text",
				description: "ist der Text, aus dem Sie Leerzeichen entfernen wollen"
			}
		]
	},
	{
		name: "GRAD",
		description: "Wandelt Bogenmaß (Radiant) in Grad um.",
		arguments: [
			{
				name: "Winkel",
				description: "ist ein in Bogenmaß (Radiant) gegebener Winkel, den Sie umwandeln möchten"
			}
		]
	},
	{
		name: "GROSS",
		description: "Wandelt einen Text in Großbuchstaben um.",
		arguments: [
			{
				name: "Text",
				description: "ist der Text, der in Großbuchstaben umgewandelt werden soll"
			}
		]
	},
	{
		name: "GROSS2",
		description: "Wandelt den ersten Buchstaben aller Wörter einer Zeichenfolge in Großbuchstaben um.",
		arguments: [
			{
				name: "Text",
				description: "ist ein in Anführungszeichen eingeschlossener Text, eine Formel, die einen Text zurückgibt, oder ein Bezug auf eine Zelle, die den Text enthält, den Sie teilweise großschreiben wollen"
			}
		]
	},
	{
		name: "GTEST",
		description: "Gibt die einseitige Prüfstatistik für einen Gaußtest (Normalverteilung) zurück.",
		arguments: [
			{
				name: "Matrix",
				description: "ist die Matrix oder der Datenbereich, gegen die/den Sie x testen möchten"
			},
			{
				name: "x",
				description: "ist der zu testende Wert"
			},
			{
				name: "Sigma",
				description: "ist die bekannte Standardabweichung der Grundgesamtheit. Ohne Angabe wird die Beispielstandardabweichung verwendet"
			}
		]
	},
	{
		name: "HARMITTEL",
		description: "Gibt das harmonische Mittel eines Datensatzes mit positiven Zahlen zurück: den Umkehrwert des arithmetischen Mittels der Umkehrwerte.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "Zahl1",
				description: "sind 1 bis 255 Zahlen, Namen, Arrays oder Bezüge, die Zahlen enthalten, deren harmonisches Mittel Sie berechnen möchten"
			},
			{
				name: "Zahl2",
				description: "sind 1 bis 255 Zahlen, Namen, Arrays oder Bezüge, die Zahlen enthalten, deren harmonisches Mittel Sie berechnen möchten"
			}
		]
	},
	{
		name: "HÄUFIGKEIT",
		description: "Gibt eine Häufigkeitsverteilung als einspaltige Matrix zurück.",
		arguments: [
			{
				name: "Daten",
				description: "entspricht einer Matrix von oder einem Bezug auf eine Wertemenge, deren Häufigkeiten Sie zählen möchten"
			},
			{
				name: "Klassen",
				description: "sind die als Matrix oder Bezug auf einen Zellbereich eingegebenen Intervallgrenzen, nach denen Sie die in Daten befindlichen Werte einordnen möchten"
			}
		]
	},
	{
		name: "HEUTE",
		description: "Gibt die fortlaufende Zahl des heutigen Datums zurück.",
		arguments: [
		]
	},
	{
		name: "HEXINBIN",
		description: "Wandelt eine hexadezimale Zahl in eine Binärzahl um.",
		arguments: [
			{
				name: "Zahl",
				description: "ist die hexadezimale Zahl, die Sie umwandeln möchten"
			},
			{
				name: "Stellen",
				description: "ist die Anzahl der zu verwendenden Zeichen"
			}
		]
	},
	{
		name: "HEXINDEZ",
		description: "Wandelt eine hexadezimale Zahl in eine dezimale Zahl um.",
		arguments: [
			{
				name: "Zahl",
				description: "ist die hexadezimale Zahl, die Sie umwandeln möchten"
			}
		]
	},
	{
		name: "HEXINOKT",
		description: "Wandelt eine hexadezimale Zahl in eine oktale Zahl um.",
		arguments: [
			{
				name: "Zahl",
				description: "ist die hexadezimale Zahl, die Sie umwandeln möchten"
			},
			{
				name: "Stellen",
				description: "ist die Zahl der verwendeten Stellen"
			}
		]
	},
	{
		name: "HYPERLINK",
		description: "Erstellt eine Verknüpfung, die zur Hyperlink_Adresse verbindet.",
		arguments: [
			{
				name: "Hyperlink_Adresse",
				description: "ist der gesamte Pfad und Dateiname, UNC-Pfad oder Internet-URL zu dem Dokument, dass beim Klicken auf die Zelle geöffnet wird"
			},
			{
				name: "Freundlicher_Name",
				description: "ist die Zahl, Text oder Funktion, die in der Zelle angezeigt werden soll"
			}
		]
	},
	{
		name: "HYPGEOM.VERT",
		description: "Gibt Wahrscheinlichkeiten einer hypergeometrisch-verteilten Zufallsvariablen zurück.",
		arguments: [
			{
				name: "Erfolge_S",
				description: "ist die Anzahl der in der Stichprobe erzielten Erfolge"
			},
			{
				name: "Umfang_S",
				description: "ist der Umfang (Größe) der Stichprobe"
			},
			{
				name: "Erfolge_G",
				description: "ist die Anzahl der in der Grundgesamtheit möglichen Erfolge"
			},
			{
				name: "Umfang_G",
				description: "ist der Umfang (Größe) der Grundgesamtheit"
			},
			{
				name: "kumuliert",
				description: "ist ein Wahrheitswert: für die kumulierte Verteilungsfunktion WAHR; für die Wahrscheinlichkeitsdichtefunktion FALSCH"
			}
		]
	},
	{
		name: "HYPGEOMVERT",
		description: "Gibt Wahrscheinlichkeiten einer hypergeometrisch-verteilten Zufallsvariablen zurück.",
		arguments: [
			{
				name: "Erfolge_S",
				description: "ist die Anzahl der in der Stichprobe erzielten Erfolge"
			},
			{
				name: "Umfang_S",
				description: "ist der Umfang (Größe) der Stichprobe"
			},
			{
				name: "Erfolge_G",
				description: "ist die Anzahl der in der Grundgesamtheit möglichen Erfolge"
			},
			{
				name: "Umfang_G",
				description: "ist der Umfang (Größe) der Grundgesamtheit"
			}
		]
	},
	{
		name: "IDENTISCH",
		description: "Prüft, ob zwei Zeichenfolgen identisch sind.",
		arguments: [
			{
				name: "Text1",
				description: "ist die erste Zeichenfolge, eingeschlossen in Anführungszeichen"
			},
			{
				name: "Text2",
				description: "ist die zweite Zeichenfolge, eingeschlossen in Anführungszeichen"
			}
		]
	},
	{
		name: "IKV",
		description: "Gibt den internen Zinsfuß einer Investition ohne Finanzierungskosten oder Reinvestitionsgewinne zurück.",
		arguments: [
			{
				name: "Werte",
				description: "ist eine Matrix von Zellen oder ein Bezug auf Zellen, in denen die Zahlen stehen, für die Sie den internen Zinsfuß berechnen möchten"
			},
			{
				name: "Schätzwert",
				description: "ist eine Zahl, von der Sie annehmen, dass sie dem Ergebnis der Funktion nahe kommt. Wenn der Parameter fehlt, wird 0,1 (10 Prozent) angenommen"
			}
		]
	},
	{
		name: "IMABS",
		description: "Gibt den Absolutbetrag (Modulo) einer komplexen Zahl zuück.",
		arguments: [
			{
				name: "Komplexe_Zahl",
				description: "ist die komplexe Zahl, deren absoluten Wert Sie bestimmen möchten"
			}
		]
	},
	{
		name: "IMAGINÄRTEIL",
		description: "Gibt den Imaginärteil einer komplexen Zahl zurück.",
		arguments: [
			{
				name: "Komplexe_Zahl",
				description: "ist die komplexe Zahl, deren Imaginärteil Sie bestimmen möchten"
			}
		]
	},
	{
		name: "IMAPOTENZ",
		description: "Potenziert eine komplexe Zahl mit einer ganzen Zahl.",
		arguments: [
			{
				name: "Komplexe_Zahl",
				description: "ist die komplexe Zahl, die Sie mit dem Exponenten potenzieren möchten"
			},
			{
				name: "Potenz",
				description: "ist der Exponent, mit dem Sie die komplexe Zahl potenzieren möchten"
			}
		]
	},
	{
		name: "IMARGUMENT",
		description: "Gibt den Winkel im Bogenmaß zur Darstellung der komplexen Zahl in trigonometrischer Schreibweise zurück.",
		arguments: [
			{
				name: "Komplexe_Zahl",
				description: "ist die komplexe Zahl, deren Argument Sie bestimmen möchten"
			}
		]
	},
	{
		name: "IMCOS",
		description: "Gibt den Kosinus einer komplexen Zahl zurück.",
		arguments: [
			{
				name: "Komplexe_Zahl",
				description: "ist die komplexe Zahl, deren Kosinus Sie bestimmen möchten"
			}
		]
	},
	{
		name: "IMCOSEC",
		description: "Gibt den Kosekans einer komplexen Zahl zurück.",
		arguments: [
			{
				name: "Zahl",
				description: "ist eine komplexe Zahl, deren Kosekans Sie bestimmen möchten"
			}
		]
	},
	{
		name: "IMCOSECHYP",
		description: "Gibt den hyperbolischen Kosekans einer komplexen Zahl zurück.",
		arguments: [
			{
				name: "Zahl",
				description: "ist eine komplexe Zahl, deren hyperbolischen Kosekans Sie bestimmen möchten"
			}
		]
	},
	{
		name: "IMCOSHYP",
		description: "Gibt den hyperbolischen Kosinus einer komplexen Zahl zurück.",
		arguments: [
			{
				name: "Zahl",
				description: "ist eine komplexe Zahl, deren hyperbolischen Kosinus Sie bestimmen möchten"
			}
		]
	},
	{
		name: "IMCOT",
		description: "Gibt den Kotangens einer komplexen Zahl zurück.",
		arguments: [
			{
				name: "Zahl",
				description: "ist eine komplexe Zahl, deren Kotangens Sie bestimmen möchten"
			}
		]
	},
	{
		name: "IMDIV",
		description: "Gibt den Quotient zweier komplexer Zahlen zuück.",
		arguments: [
			{
				name: "Komplexe_Zahl1",
				description: "ist der komplexe Zähler bzw. Dividend"
			},
			{
				name: "Komplexe_Zahl2",
				description: "ist der komplexe Nenner bzw. Divisor"
			}
		]
	},
	{
		name: "IMEXP",
		description: "Gibt die algebraische Form einer in exponentieller Schreibweise vorliegenden komplexen Zahl zurück.",
		arguments: [
			{
				name: "Komplexe_Zahl",
				description: "ist die komplexe Zahl, deren exponentielle Schreibweise Sie bestimmen möchten"
			}
		]
	},
	{
		name: "IMKONJUGIERTE",
		description: "Gibt die konjugiert komplexe Zahl zu einer komplexen Zahl zurück.",
		arguments: [
			{
				name: "Komplexe_Zahl",
				description: "ist die komplexe Zahl, deren Konjugierte Sie bestimmen möchten"
			}
		]
	},
	{
		name: "IMLN",
		description: "Gibt den natürlichen Logarithmus einer komplexen Zahl zurück.",
		arguments: [
			{
				name: "Komplexe_Zahl",
				description: "ist die komplexe Zahl, deren natürlichen Logarithmus Sie bestimmen möchten"
			}
		]
	},
	{
		name: "IMLOG10",
		description: "Gibt den Logarithmus einer komplexen Zahl zur Basis 10 zurück.",
		arguments: [
			{
				name: "Komplexe_Zahl",
				description: "ist die komplexe Zahl, deren Logarithmus zur Basis 10 Sie bestimmen möchten"
			}
		]
	},
	{
		name: "IMLOG2",
		description: "Gibt den Logarithmus einer komplexen Zahl zur Basis 2 zurück.",
		arguments: [
			{
				name: "Komplexe_Zahl",
				description: "ist die komplexe Zahl, deren Logarithmus zur Basis 2 Sie bestimmen möchten"
			}
		]
	},
	{
		name: "IMPRODUKT",
		description: "Gibt das Produkt von 1 bis 255 komplexen Zahlen zurück.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "Komplexe_Zahl1",
				description: "Komplexe_Zahl1, Komplexe_Zahl2,... sind von 1 bis 255 komplexe Zahlen, die multipliziert werden können."
			},
			{
				name: "Komplexe_Zahl2",
				description: "Komplexe_Zahl1, Komplexe_Zahl2,... sind von 1 bis 255 komplexe Zahlen, die multipliziert werden können."
			}
		]
	},
	{
		name: "IMREALTEIL",
		description: "Gibt den Realteil einer komplexen Zahl zurück.",
		arguments: [
			{
				name: "Komplexe_Zahl",
				description: "ist die komplexe Zahl, deren Realteil Sie bestimmen möchten"
			}
		]
	},
	{
		name: "IMSEC",
		description: "Gibt den Sekans einer komplexen Zahl zurück.",
		arguments: [
			{
				name: "Zahl",
				description: "ist eine komplexe Zahl, deren Sekans Sie bestimmen möchten"
			}
		]
	},
	{
		name: "IMSECHYP",
		description: "Gibt den hyperbolischen Sekans einer komplexen Zahl zurück.",
		arguments: [
			{
				name: "Zahl",
				description: "ist eine komplexe Zahl, deren hyperbolischen Sekans Sie bestimmen möchten"
			}
		]
	},
	{
		name: "IMSIN",
		description: "Gibt den Sinus einer komplexen Zahl zurück.",
		arguments: [
			{
				name: "Komplexe_Zahl",
				description: "ist die komplexe Zahl, deren Sinus Sie bestimmen möchten"
			}
		]
	},
	{
		name: "IMSINHYP",
		description: "Gibt den hyperbolischen Sinus einer komplexen Zahl zurück.",
		arguments: [
			{
				name: "Zahl",
				description: "ist eine komplexe Zahl, deren hyperbolischen Sinus Sie bestimmen möchten"
			}
		]
	},
	{
		name: "IMSUB",
		description: "Gibt die Differenz zweier komplexer Zahlen.",
		arguments: [
			{
				name: "Komplexe_Zahl1",
				description: "ist die komplexe Zahl zurück, von der Komplexe_Zahl2 subtrahiert werden soll"
			},
			{
				name: "Komplexe_Zahl2",
				description: "ist die komplexe Zahl, die von Komplexe_Zahl1 subtrahiert werden soll"
			}
		]
	},
	{
		name: "IMSUMME",
		description: "Gibt die Summe von komplexen Zahlen zurück.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "Komplexe_Zahl1",
				description: "sind 1 bis 255 komplexe Zahlen, die addiert werden können"
			},
			{
				name: "Komplexe_Zahl2",
				description: "sind 1 bis 255 komplexe Zahlen, die addiert werden können"
			}
		]
	},
	{
		name: "IMTAN",
		description: "Gibt den Tangens einer komplexen Zahl zurück.",
		arguments: [
			{
				name: "Zahl",
				description: "ist eine komplexe Zahl, deren Tangens Sie bestimmen möchten"
			}
		]
	},
	{
		name: "IMWURZEL",
		description: "Gibt die Quadratwurzel einer komplexen Zahl zurück.",
		arguments: [
			{
				name: "Komplexe_Zahl",
				description: "ist die komplexe Zahle, deren Quadratwurzel Sie bestimmen möchten"
			}
		]
	},
	{
		name: "INDEX",
		description: "Verwendet einen Index, um aus einem Bezug oder einer Matrix einen Wert zu wählen.",
		arguments: [
			{
				name: "Matrix",
				description: "ist ein als Matrix eingegebener Zellbereich"
			},
			{
				name: "Zeile",
				description: "markiert die Zeile in der Matrix, aus der ein Wert zurückgegeben werden soll"
			},
			{
				name: "Spalte",
				description: "markiert die Spalte in der Matrix, aus der ein Wert zurückgegeben werden soll"
			}
		]
	},
	{
		name: "INDIREKT",
		description: "Gibt den Bezug eines Textwertes zurück.",
		arguments: [
			{
				name: "Bezug",
				description: "ist der Bezug auf eine Zelle, die einen Bezug in der A1-Schreibweise, einen Bezug in der Z1S1-Schreibweise oder einen definierten Namen als Bezug enthält"
			},
			{
				name: "A1",
				description: "ist ein Wahrheitswert, der angibt, welche Art von Bezug in der Zelle enthalten ist: FALSCH = Z1S1-Bezüge; WAHR oder fehlt = A1-Bezüge"
			}
		]
	},
	{
		name: "INFO",
		description: "Gibt Informationen zu der aktuellen Betriebssystemumgebung zurück.",
		arguments: [
			{
				name: "Typ",
				description: "ist ein Text, der bestimmt, welche Art von Informationen Sie erhalten wollen"
			}
		]
	},
	{
		name: "ISO.OBERGRENZE",
		description: "Rundet eine Zahl betragsmäßig auf das kleinste Vielfache von Schritt auf.",
		arguments: [
			{
				name: "Zahl",
				description: "ist der Wert, den Sie runden möchten"
			},
			{
				name: "Schritt",
				description: "ist der Wert, auf dessen optionales Vielfaches Sie runden möchten"
			}
		]
	},
	{
		name: "ISOKALENDERWOCHE",
		description: "Gibt die Zahl der ISO-Wochenzahl des Jahres für ein angegebenes Datum zurück.",
		arguments: [
			{
				name: "Datum",
				description: "ist der von Spreadsheet für die Datums- und Uhrzeitberechnung verwendete Datums-Uhrzeitcode"
			}
		]
	},
	{
		name: "ISPMT",
		description: "Gibt den Zinssatz für gleichgroße Rückzahlungsraten zurück.",
		arguments: [
			{
				name: "rate",
				description: "Zinssatz pro Rate. Z.B. verwenden Sie 6%/4 für Quartalszahlungen von 6%."
			},
			{
				name: "per",
				description: "Zeitraum, für den der Zinssatz ermittelt werden soll"
			},
			{
				name: "nper",
				description: "Anzahl der Zahlungen in einer Annuität"
			},
			{
				name: "pv",
				description: "Pauschalbetrag der die Anzahl der zukünftigen Zahlungen im heutigen Wert angibt"
			}
		]
	},
	{
		name: "ISTBEZUG",
		description: "Gibt WAHR zurück, wenn der Wert ein Bezug ist.",
		arguments: [
			{
				name: "Wert",
				description: "ist der Wert, der geprüft werden soll"
			}
		]
	},
	{
		name: "ISTFEHL",
		description: "Überprüft, ob ein Wert ein Fehlerwert ist (#WERT!, #BEZUG!, #DIV/0!, #ZAHL!, #NAME? oder #NULL!), ungleich #NV, und gibt WAHR oder FALSCH zurück.",
		arguments: [
			{
				name: "Wert",
				description: "ist der Wert, der geprüft werden soll. Der Wert kann eine Zelle sein, eine Formel oder ein Name, der sich auf eine Zelle, Formel oder einen Wert bezieht"
			}
		]
	},
	{
		name: "ISTFEHLER",
		description: "Überprüft, ob ein Wert ein Fehlerwert ist (#NV, #WERT!, #BEZUG!, #DIV/0!, #ZAHL!, #NAME? oder #NULL!) und gibt WAHR oder FALSCH zurück.",
		arguments: [
			{
				name: "Wert",
				description: "ist der Wert, der geprüft werden soll. Der Wert kann eine Zelle sein, eine Formel oder ein Name, der sich auf eine Zelle, Formel oder einen Wert bezieht"
			}
		]
	},
	{
		name: "ISTFORMEL",
		description: "Überprüft, ob ein Bezug auf eine Zelle verweist, die eine Formel enthält, und gibt WAHR oder FALSCH zurück.",
		arguments: [
			{
				name: "Bezug",
				description: "ist ein Bezug auf die zu prüfende Zelle. Der Bezug kann ein Zellbezug, eine Formel oder ein Name, der auf eine Zelle verweist, sein"
			}
		]
	},
	{
		name: "ISTGERADE",
		description: "Gibt WAHR zurück, wenn es sich um eine gerade Zahl handelt.",
		arguments: [
			{
				name: "Zahl",
				description: "ist der zu prüfende Wert"
			}
		]
	},
	{
		name: "ISTKTEXT",
		description: "Gibt WAHR zurück, wenn der Wert ein Element ist, das keinen Text enthält.",
		arguments: [
			{
				name: "Wert",
				description: "ist der Wert, der geprüft werden soll, eine Zelle, eine Formel oder ein Name, der ich auf eine Zelle, eine Formel oder einen Wert bezieht"
			}
		]
	},
	{
		name: "ISTLEER",
		description: "Gibt WAHR zurück, wenn der Wert eine leere Zelle ist.",
		arguments: [
			{
				name: "Wert",
				description: "ist der Wert, der geprüft werden soll"
			}
		]
	},
	{
		name: "ISTLOG",
		description: "Gibt WAHR, wenn der Wert ein Wahrheitswert ist.",
		arguments: [
			{
				name: "Wert",
				description: "ist der Wert, der geprüft werden soll, eine Zelle, eine Formel oder ein Name, der ich auf eine Zelle, eine Formel oder einen Wert bezieht"
			}
		]
	},
	{
		name: "ISTNV",
		description: "Prüft, ob ein Wert #NV ist und gibt WAHR oder FALSCH zurück.",
		arguments: [
			{
				name: "Wert",
				description: "ist der Wert, der geprüft werden soll. Der Wert kann eine Zelle sein, eine Formel oder eine Name, der sich auf eine Zelle, Formel oder einen Wert bezieht"
			}
		]
	},
	{
		name: "ISTTEXT",
		description: "Gibt WAHR zurück, wenn der Wert ein Text ist.",
		arguments: [
			{
				name: "Wert",
				description: "ist der Wert oder der Name, der darauf geprüft werden soll, ob es sich um eine leere Zelle, einen Fehlerwert, einen Wahrheitswert, Text, eine Zahl oder einen Bezug handelt"
			}
		]
	},
	{
		name: "ISTUNGERADE",
		description: "Gibt WAHR zurück, wenn es sich um eine ungerade Zahl handelt.",
		arguments: [
			{
				name: "Zahl",
				description: "ist der zu prüfende Wert"
			}
		]
	},
	{
		name: "ISTZAHL",
		description: "Gibt WAHR zurück, wenn der Wert eine Zahl ist.",
		arguments: [
			{
				name: "Wert",
				description: "ist der Wert, der geprüft werden soll"
			}
		]
	},
	{
		name: "JAHR",
		description: "Wandelt eine fortlaufende Zahl im Bereich von 1900 - 9999 in eine Jahreszahl um.",
		arguments: [
			{
				name: "Zahl",
				description: "ist der Code für Datum und Zeit, den Spreadsheet für Datums- und Zeitberechnungen verwendet"
			}
		]
	},
	{
		name: "JETZT",
		description: "Gibt die fortlaufende Zahl des aktuellen Datums und der aktuellen Uhrzeit zurück.",
		arguments: [
		]
	},
	{
		name: "KALENDERWOCHE",
		description: "Gibt die Wochenzahl für das Jahr zurück.",
		arguments: [
			{
				name: "Fortlaufende_Zahl",
				description: "ist der von Spreadsheet für Datums- und Uhrzeitberechnungen verwendete Code für Datum und Uhrzeit"
			},
			{
				name: "Zahl_Typ",
				description: "ist eine Zahl (1 oder 2), die den Typ des Rückgabewertes bestimmt"
			}
		]
	},
	{
		name: "KAPZ",
		description: "Gibt die Kapitalrückzahlung einer Investition für die angegebene Periode zurück.",
		arguments: [
			{
				name: "Zins",
				description: "ist der Zinssatz pro Periode (Zahlungszeitraum) Z.B. verwenden Sie 6%/4 für Quartalszahlungen von 6%"
			},
			{
				name: "Zr",
				description: "gibt die Periode an und muss zwischen 1 und Zr liegen"
			},
			{
				name: "Zzr",
				description: "gibt an, über wie viele Perioden die jeweilige Annuität (Rente) gezahlt wird"
			},
			{
				name: "Bw",
				description: "ist der Barwert: der Gesamtbetrag, den eine Reihe zukünftiger Zahlungen zum gegenwärtigen Zeitpunkt wert ist"
			},
			{
				name: "Zw",
				description: "ist der zukünftige Wert (Endwert) oder der Kassenbestand, den Sie nach der letzten Zahlung erreicht haben möchten"
			},
			{
				name: "F",
				description: "kann den Wert 0 oder 1 annehmen und gibt an, wann Zahlungen fällig sind (Fälligkeit): 1 = Zahlung an Beginn der Periode, 0 = Zahlung am Ende der Periode"
			}
		]
	},
	{
		name: "KGRÖSSTE",
		description: "Gibt den k-größten Wert einer Datengruppe zurück.",
		arguments: [
			{
				name: "Matrix",
				description: "ist die Matrix oder der Datenbereich, deren k-größten Wert Sie bestimmen möchten"
			},
			{
				name: "k",
				description: "ist der Rang des Elementes einer Matrix oder eines Zellbereiches, dessen Wert zurückgegeben werden soll"
			}
		]
	},
	{
		name: "KGV",
		description: "Gibt das kleinste gemeinsame Vielfache zurück.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "Zahl1",
				description: "sind 1 bis 255 Werte, für die Sie das kleinste gemeinsame Vielfache berechnen möchten"
			},
			{
				name: "Zahl2",
				description: "sind 1 bis 255 Werte, für die Sie das kleinste gemeinsame Vielfache berechnen möchten"
			}
		]
	},
	{
		name: "KKLEINSTE",
		description: "Gibt den k-kleinsten Wert einer Datengruppe zurück.",
		arguments: [
			{
				name: "Matrix",
				description: "ist eine Matrix oder ein Bereich von numerischen Daten, deren k-kleinsten Wert Sie bestimmen möchten"
			},
			{
				name: "k",
				description: "ist der Rang des Elementes einer Matrix oder eines Zellbereiches, dessen Wert zurückgegeben werden soll"
			}
		]
	},
	{
		name: "KLEIN",
		description: "Wandelt einen Text in Kleinbuchstaben um.",
		arguments: [
			{
				name: "Text",
				description: "ist der Text, dessen Großbuchstaben in Kleinbuchstaben umgewandelt werden sollen"
			}
		]
	},
	{
		name: "KOMBINATIONEN",
		description: "Gibt die Anzahl der Kombinationen ohne Wiederholung von k Elementen aus einer Menge von n Elementen zurück.",
		arguments: [
			{
				name: "n",
				description: "ist die Anzahl aller Elemente"
			},
			{
				name: "k",
				description: "gibt an, aus wie vielen Elementen jede Kombinationsmöglichkeit bestehen soll"
			}
		]
	},
	{
		name: "KOMBINATIONEN2",
		description: "Gibt die Anzahl der Kombinationen mit Wiederholung für eine angegebene Anzahl von Elementen zurück.",
		arguments: [
			{
				name: "Zahl",
				description: "ist die Gesamtzahl der Elemente"
			},
			{
				name: "gewählte_Zahl",
				description: "ist die Anzahl der Elemente in jeder Kombination"
			}
		]
	},
	{
		name: "KOMPLEXE",
		description: "Wandelt den Real- und Imaginärteil in eine komplexe Zahl um.",
		arguments: [
			{
				name: "Realteil",
				description: "ist der Realteil der komplexen Zahl"
			},
			{
				name: "Imaginärteil",
				description: "ist der Imaginärteil der komplexen Zahl"
			},
			{
				name: "Suffix",
				description: "ist der Buchstabe, der als imaginäre Einheit der komplexen Zahl verwendet werden soll."
			}
		]
	},
	{
		name: "KONFIDENZ",
		description: "Ermöglicht die Berechnung des 1-Alpha Konfidenzintervalls für den Erwartungswert einer Zufallsvariablen und verwendet dazu die Normalverteilung.",
		arguments: [
			{
				name: "Alpha",
				description: "ist die Irrtumswahrscheinlichkeit Alpha, zur Berechnung des 1- Alpha Konfidenzintervalls, eine Zahl größer als 0 und kleiner als 1"
			},
			{
				name: "Standabwn",
				description: "ist die als bekannt angenommene Standardabweichung der Grundgesamtheit, eine Zahl, die größer als 1 sein muss"
			},
			{
				name: "Umfang_S",
				description: "ist die Größe der Stichprobe"
			}
		]
	},
	{
		name: "KONFIDENZ.NORM",
		description: "Gibt das Konfidenzintervall für den Erwartungswert einer Zufallsvariablen zurück.",
		arguments: [
			{
				name: "Alpha",
				description: "ist die Irrtumswahrscheinlichkeit Alpha zur Berechnung des 1-Alpha-Konfidenzintervalls, eine Zahl größer als 0 und kleiner als 1"
			},
			{
				name: "Standardabwn",
				description: "ist die Standardabweichung der Grundgesamtheit für den Datenbereich, die als bekannt angenommen wird. Standardabwn muss größer als 0 sein"
			},
			{
				name: "Umfang",
				description: "ist die Größe der Stichprobe"
			}
		]
	},
	{
		name: "KONFIDENZ.T",
		description: "Gibt das Konfidenzintervall für den Erwartungswert einer (Student) t-verteilten Zufallsvariablen zurück.",
		arguments: [
			{
				name: "Alpha",
				description: "ist die Irrtumswahrscheinlichkeit Alpha zur Berechnung des 1-Alpha-Konfidenzintervalls, eine Zahl größer als 0 und kleiner als 1"
			},
			{
				name: "Standardabwn",
				description: "ist die Standardabweichung der Grundgesamtheit für den Datenbereich, die als bekannt angenommen wird. Standardabwn muss größer als 0 sein"
			},
			{
				name: "Umfang",
				description: "ist die Größe der Stichprobe"
			}
		]
	},
	{
		name: "KORREL",
		description: "Gibt den Korrelationskoeffizient zweier Reihen von Merkmalsausprägungen zurück.",
		arguments: [
			{
				name: "Matrix1",
				description: "ist der mit Werten belegte Zellbereich, Argumente können Zahlen, Namen, Arrays oder Bezüge sein"
			},
			{
				name: "Matrix2",
				description: "ist ein zweiter mit Werten belegter Zellbereich, Argumente können Zahlen, Namen, Arrays oder Bezüge sein"
			}
		]
	},
	{
		name: "KOVAR",
		description: "Gibt die Kovarianz, den Mittelwert der für alle Datenpunktpaare gebildeten Produkte der Abweichungen zurück.",
		arguments: [
			{
				name: "Matrix1",
				description: "ist der erste mit ganzen Zahlen belegte Zellbereich, Argumente können Zahlen, Namen, Arrays oder Bezüge sein"
			},
			{
				name: "Matrix2",
				description: "ist der zweite mit ganzen Zahlen belegte Zellbereich, Argumente können Zahlen, Namen, Arrays oder Bezüge sein"
			}
		]
	},
	{
		name: "KOVARIANZ.P",
		description: "Gibt die Kovarianz einer Grundgesamtheit, den Mittelwert der für alle Datenpunktpaare gebildeten Produkte der Abweichungen zurück.",
		arguments: [
			{
				name: "Array1",
				description: "ist der erste mit ganzen Zahlen belegte Zellbereich, Argumente können Zahlen, Namen, Arrays oder Bezüge sein"
			},
			{
				name: "Array2",
				description: "ist der zweite mit ganzen Zahlen belegte Zellbereich, Argumente können Zahlen, Namen, Arrays oder Bezüge sein"
			}
		]
	},
	{
		name: "KOVARIANZ.S",
		description: "Gibt die Kovarianz einer Stichprobe, den Mittelwert der für alle Datenpunktpaare gebildeten Produkte der Abweichungen zurück.",
		arguments: [
			{
				name: "Array1",
				description: "ist der erste mit ganzen Zahlen belegte Zellbereich, Argumente können Zahlen, Namen, Arrays oder Bezüge sein"
			},
			{
				name: "Array2",
				description: "ist der zweite mit ganzen Zahlen belegte Zellbereich, Argumente können Zahlen, Namen, Arrays oder Bezüge sein"
			}
		]
	},
	{
		name: "KRITBINOM",
		description: "Gibt den kleinsten Wert, für den die kumulierten Wahrscheinlichkeiten der Binomialverteilung größer oder gleich einer Grenzwahrscheinlichkeit sind zurück.",
		arguments: [
			{
				name: "Versuche",
				description: "ist die Zahl der Bernoulliexperimente"
			},
			{
				name: "Erfolgswahrsch",
				description: "ist die Wahrscheinlichkeit für den günstigen Ausgang des Experiments, eine Zahl von einschließlich 0 bis einschließlich 1"
			},
			{
				name: "Alpha",
				description: "ist die Grenzwahrscheinlichkeit, eine Zahl von einschließlich 0 bis einschließlich 1"
			}
		]
	},
	{
		name: "KUMKAPITAL",
		description: "Berechnet die aufgelaufene Tilgung eines Darlehens, die zwischen zwei Perioden zu zahlen ist.",
		arguments: [
			{
				name: "Zins",
				description: "ist der Zinssatz"
			},
			{
				name: "Zzr",
				description: "ist die Anzahl aller Zahlungsperioden"
			},
			{
				name: "Bw",
				description: "ist der Gegenwartswert"
			},
			{
				name: "Zeitraum_Anfang",
				description: "ist die erste in die Berechnung einfließende Periode"
			},
			{
				name: "Zeitraum_Ende",
				description: "ist die letzte in die Berechnung einfließende Periode"
			},
			{
				name: "F",
				description: "gibt an, zu welchem Zeitpunkt einer Periode jeweils eine Zahlung fällig ist"
			}
		]
	},
	{
		name: "KUMZINSZ",
		description: "Berechnet die kumulierten Zinsen, die zwischen zwei Perioden zu zahlen sind.",
		arguments: [
			{
				name: "Zins",
				description: "ist der Zinssatz"
			},
			{
				name: "Zzr",
				description: "ist die Anzahl aller Zahlungsperioden"
			},
			{
				name: "Bw",
				description: "ist der Gegenwartswert"
			},
			{
				name: "Zeitraum_Anfang",
				description: "ist die erste in die Berechnung einfließende Periode"
			},
			{
				name: "Zeitraum_Ende",
				description: "ist die letzte in die Berechnung einfließende Periode"
			},
			{
				name: "F",
				description: "gibt an, zu welchem Zeitpunkt einer Periode jeweils eine Zahlung fällig ist"
			}
		]
	},
	{
		name: "KURSDISAGIO",
		description: "Gibt den Kurs pro 100 EUR Nennwert eines unverzinslichen Wertpapiers zurück.",
		arguments: [
			{
				name: "Abrechnung",
				description: "ist der Abrechnungstermin des Wertpapierkaufs, als fortlaufende Zahl angegeben"
			},
			{
				name: "Fälligkeit",
				description: "ist der Fälligkeitstermin des Wertpapiers, als fortlaufende Zahl angegeben"
			},
			{
				name: "Disagio",
				description: "ist der in Prozent ausgedrückte Abschlag (Disagio) des Wertpapiers"
			},
			{
				name: "Rückzahlung",
				description: "ist der Rückzahlungswert des Wertpapiers pro 100 EUR Nennwert"
			},
			{
				name: "Basis",
				description: "gibt an, auf welcher Basis die Zinstage gezählt werden"
			}
		]
	},
	{
		name: "KURT",
		description: "Gibt die Kurtosis (Exzess) einer Datengruppe zurück.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "Zahl1",
				description: "sind 1 bis 255 Argumente, deren Kurtosis (Exzess) Sie berechnen möchten"
			},
			{
				name: "Zahl2",
				description: "sind 1 bis 255 Argumente, deren Kurtosis (Exzess) Sie berechnen möchten"
			}
		]
	},
	{
		name: "KÜRZEN",
		description: "Schneidet die Kommastellen der Zahl ab und gibt als Ergebnis eine ganze Zahl zurück.",
		arguments: [
			{
				name: "Zahl",
				description: "ist die Zahl, deren Stellen Sie abschneiden wollen"
			},
			{
				name: "Anzahl_Stellen",
				description: "ist eine Zahl, die angibt, wie viele Nachkommastellen erhalten bleiben sollen (0, wenn ausgelassen)"
			}
		]
	},
	{
		name: "LÄNGE",
		description: "Gibt die Anzahl der Zeichen einer Zeichenfolge zurück.",
		arguments: [
			{
				name: "Text",
				description: "ist der Text, dessen Länge Sie bestimmen wollen"
			}
		]
	},
	{
		name: "LIA",
		description: "Gibt die lineare Abschreibung eines Wirtschaftsguts pro Periode zurück.",
		arguments: [
			{
				name: "Ansch_Wert",
				description: "sind die Anschaffungskosten eines Wirtschaftsguts"
			},
			{
				name: "Restwert",
				description: "ist der Restwert am Ende der Nutzungsdauer (wird häufig auch als Schrottwert bezeichnet)"
			},
			{
				name: "Nutzungsdauer",
				description: "ist die Anzahl der Perioden, über die das Wirtschaftsgut abgeschrieben wird (auch als Nutzungsdauer bezeichnet)"
			}
		]
	},
	{
		name: "LINKS",
		description: "Gibt das erste oder die ersten Zeichen einer Zeichenfolge zurück.",
		arguments: [
			{
				name: "Text",
				description: "ist die Zeichenfolge mit den Zeichen, die Sie kopieren wollen"
			},
			{
				name: "Anzahl_Zeichen",
				description: "gibt an, wie viele Zeichen LINKS zurückgeben soll, 1 wenn nicht angegeben"
			}
		]
	},
	{
		name: "LN",
		description: "Gibt den natürlichen Logarithmus einer Zahl zurück.",
		arguments: [
			{
				name: "Zahl",
				description: "ist die positive reelle Zahl, deren natürlichen Logarithmus Sie berechnen möchten"
			}
		]
	},
	{
		name: "LOG",
		description: "Gibt den Logarithmus einer Zahl zu der angegebenen Basis zurück.",
		arguments: [
			{
				name: "Zahl",
				description: "ist die positive reelle Zahl, deren Logarithmus Sie berechnen möchten"
			},
			{
				name: "Basis",
				description: "ist die Basis des Logarithmus. Wenn der Parameter fehlt, wird 10 angenommen"
			}
		]
	},
	{
		name: "LOG10",
		description: "Gibt den Logarithmus einer Zahl zur Basis 10 zurück.",
		arguments: [
			{
				name: "Zahl",
				description: "ist die positive reelle Zahl, deren Logarithmus zur Basis 10 Sie berechnen möchten"
			}
		]
	},
	{
		name: "LOGINV",
		description: "Gibt Perzentile der Lognormalverteilung zurück.",
		arguments: [
			{
				name: "Wahrsch",
				description: "ist die zur Lognormalverteilung gehörige Wahrscheinlichkeit, eine Zahl von einschließlich 0 bis einschließlich 1"
			},
			{
				name: "Mittelwert",
				description: "ist das Mittel der Lognormalverteilung"
			},
			{
				name: "Standabwn",
				description: "ist die Standardabweichung der Lognormalverteilung, eine positive Zahl"
			}
		]
	},
	{
		name: "LOGNORM.INV",
		description: "Gibt Perzentile der Lognormalverteilung zurück.",
		arguments: [
			{
				name: "Wahrsch",
				description: "ist die zur Lognormalverteilung gehörige Wahrscheinlichkeit, eine Zahl von einschließlich 0 bis einschließlich 1"
			},
			{
				name: "Mittelwert",
				description: "ist das Mittel der Lognormalverteilung"
			},
			{
				name: "Standabwn",
				description: "ist die Standardabweichung der Lognormalverteilung, eine positive Zahl"
			}
		]
	},
	{
		name: "LOGNORM.VERT",
		description: "Gibt Werte der Verteilungsfunktion einer lognormalverteilten Zufallsvariablen zurück, wobei ln(x) mit den Parametern Mittelwert und Standabwn normalverteilt ist.",
		arguments: [
			{
				name: "x",
				description: "ist der Wert der Verteilung (Perzentil), dessen Wahrscheinlichkeit Sie berechnen möchten, eine positive Zahl"
			},
			{
				name: "Mittelwert",
				description: "ist der Mittelwert der Lognormalverteilung"
			},
			{
				name: "Standabwn",
				description: "ist die Standardabweichung der Lognormalverteilung, eine positive Zahl"
			},
			{
				name: "kumuliert",
				description: "ist ein Wahrheitswert: für die kumulierte Verteilungsfunktion WAHR; für die Wahrscheinlichkeitsdichtefunktion FALSCH"
			}
		]
	},
	{
		name: "LOGNORMVERT",
		description: "Gibt Werte der Verteilungsfunktion einer lognormalverteilten Zufallsvariablen zurück.",
		arguments: [
			{
				name: "x",
				description: "ist der Wert der Verteilung (Perzentil), dessen Wahrscheinlichkeit Sie berechnen möchten, eine positive Zahl"
			},
			{
				name: "Mittelwert",
				description: "ist der Mittelwert der Lognormalverteilung"
			},
			{
				name: "Standabwn",
				description: "ist die Standardabweichung der Lognormalverteilung, eine positive Zahl"
			}
		]
	},
	{
		name: "MAX",
		description: "Gibt den größten Wert innerhalb einer Wertemenge zurück. Logische Werte und Textwerte werden ignoriert.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "Zahl1",
				description: "sind 1 bis 255 Zahlen, leere Zellen, logische Werte oder Textzahlen, deren größte Zahl Sie berechnen möchten"
			},
			{
				name: "Zahl2",
				description: "sind 1 bis 255 Zahlen, leere Zellen, logische Werte oder Textzahlen, deren größte Zahl Sie berechnen möchten"
			}
		]
	},
	{
		name: "MAXA",
		description: "Gibt den größten Wert innerhalb einer Wertemenge zurück. Logische Werte und Text werden nicht ignoriert.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "Wert1",
				description: "sind 1 bis 255 Zahlen, leere Zellen, logische Werte oder Textwerte, in denen der größte Wert gefunden werden soll"
			},
			{
				name: "Wert2",
				description: "sind 1 bis 255 Zahlen, leere Zellen, logische Werte oder Textwerte, in denen der größte Wert gefunden werden soll"
			}
		]
	},
	{
		name: "MDET",
		description: "Gibt die Determinante einer Matrix zurück.",
		arguments: [
			{
				name: "Matrix",
				description: "ist eine quadratische Matrix (die Anzahl der Zeilen und Spalten ist identisch)"
			}
		]
	},
	{
		name: "MEDIAN",
		description: "Gibt den Median bzw. die Zahl in der Mitte der Menge von angegebenen Zahlen zurück.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "Zahl1",
				description: "sind 1 bis 255 Zahlen oder Namen, Arrays oder Bezüge, die Zahlen enthalten, deren Median Sie berechnen möchten"
			},
			{
				name: "Zahl2",
				description: "sind 1 bis 255 Zahlen oder Namen, Arrays oder Bezüge, die Zahlen enthalten, deren Median Sie berechnen möchten"
			}
		]
	},
	{
		name: "MEINHEIT",
		description: "Gibt die Einheitsmatrix für die angegebene Größe zurück.",
		arguments: [
			{
				name: "Größe",
				description: "ist eine ganze Zahl zur Angabe der Größe der Einheitsmatrix, die zurückgegeben werden soll"
			}
		]
	},
	{
		name: "MIN",
		description: "Gibt den kleinsten Wert innerhalb einer Wertemenge zurück. Logische Werte und Text werden ignoriert.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "Zahl1",
				description: "sind 1 bis 255 Zahlen, leere Zellen, logische Werte oder Textzahlen, deren kleinste Zahl Sie berechnen möchten"
			},
			{
				name: "Zahl2",
				description: "sind 1 bis 255 Zahlen, leere Zellen, logische Werte oder Textzahlen, deren kleinste Zahl Sie berechnen möchten"
			}
		]
	},
	{
		name: "MINA",
		description: "Gibt den kleinsten Wert innerhalb einer Wertemenge zurück. Logische Werte und Text werden nicht ignoriert.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "Wert1",
				description: "sind 1 bis 255 Zahlen, leere Zellen, logische Werte und Textwerte, in denen der kleinste Wert gefunden werden soll"
			},
			{
				name: "Wert2",
				description: "sind 1 bis 255 Zahlen, leere Zellen, logische Werte und Textwerte, in denen der kleinste Wert gefunden werden soll"
			}
		]
	},
	{
		name: "MINUTE",
		description: "Wandelt eine fortlaufende Zahl in einen Wert zwischen 0 und 59 für Minuten um.",
		arguments: [
			{
				name: "Zahl",
				description: "ist der Code für Datum und Zeit, den Spreadsheet für Datums- und Zeitberechnungen verwendet"
			}
		]
	},
	{
		name: "MINV",
		description: "Gibt die Inverse einer Matrix (die zu einer Matrix gehörende Kehrmatrix) zurück.",
		arguments: [
			{
				name: "Matrix",
				description: "ist eine quadratische Matrix (die Anzahl der Zeilen und Spalten ist identisch)"
			}
		]
	},
	{
		name: "MITTELABW",
		description: "Gibt die durchschnittliche absolute Abweichung von Datenpunkten von ihrem Mittelwert zurück. Die Argumente können Zahlen, Namen, Arrays oder Bezüge sein, die Zahlen enthalten.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "Zahl1",
				description: "sind 1 bis 255 Argumente, deren durchschnittliche absolute Abweichung Sie berechnen möchten"
			},
			{
				name: "Zahl2",
				description: "sind 1 bis 255 Argumente, deren durchschnittliche absolute Abweichung Sie berechnen möchten"
			}
		]
	},
	{
		name: "MITTELWERT",
		description: "Gibt den Mittelwert (arithmetisches Mittel) der Argumente zurück, bei denen es sich um Zahlen oder Namen, Arrays oder Bezüge handeln kann, die Zahlen enthalten.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "Zahl1",
				description: "sind 1 bis 255 numerische Argumente, deren Mittelwert Sie berechnen möchten"
			},
			{
				name: "Zahl2",
				description: "sind 1 bis 255 numerische Argumente, deren Mittelwert Sie berechnen möchten"
			}
		]
	},
	{
		name: "MITTELWERTA",
		description: "Gibt den Mittelwert der Argumente zurück. Dabei werden Text und FALSCH als 0 interpretiert, WAHR wird als 1 interpretiert. Argumente können Zahlen, Namen, Arrays oder Bezüge sein.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "Wert1",
				description: "sind 1 bis 255 Argumente, für die der Mittelwert berechnet werden soll"
			},
			{
				name: "Wert2",
				description: "sind 1 bis 255 Argumente, für die der Mittelwert berechnet werden soll"
			}
		]
	},
	{
		name: "MITTELWERTWENN",
		description: "Sucht den Mittelwert (arithmetisches Mittel) für die von einer bestimmten Bedingung oder bestimmten Kriterien festgelegten Zellen.",
		arguments: [
			{
				name: "Bereich",
				description: "ist der Bereich der Zellen, der ausgewertet werden soll"
			},
			{
				name: "Kriterien",
				description: "ist die Bedingung oder sind die Kriterien in Form einer Zahl, eines Ausdrucks oder Textes, der definiert, welche Zellen verwendet werden, um den Mittelwert zu suchen"
			},
			{
				name: "Mittelwert_Bereich",
				description: "sind die tatsächlichen Zellen, die verwendet werden, um den Mittelwert zu suchen. Ohne diesen Wert werden die Zellen im Bereich verwendet"
			}
		]
	},
	{
		name: "MITTELWERTWENNS",
		description: "Sucht den Mittelwert (arithmetisches Mittel) für die Zellen, die durch bestimmte Bedingungen oder Kriterien angegeben sind.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "Mittelwert_Bereich",
				description: "sind die tatsächlichen Zellen, die zum Suchen des Mittelwerts verwendet werden sollen."
			},
			{
				name: "Kriterien_Bereich",
				description: "ist der Zellenbereich, der für eine bestimmte Bedingung ausgewertet werden soll"
			},
			{
				name: "Kriterien",
				description: "ist die Bedingung oder sind die Kriterien in Form einer Zahl, eines Ausdrucks oder Texts, der definiert, welche Zellen zum Suchen des Mittelwerts verwendet werden sollen"
			}
		]
	},
	{
		name: "MMULT",
		description: "Gibt das Produkt von zwei Arrays zurück, ein Array mit der gleichen Anzahl von Zeilen wie Array1 und Spalten wie Array2.",
		arguments: [
			{
				name: "Array1",
				description: "ist das erste Array zu multiplizierender Zahlen und muss die gleiche Anzahl von Spalten aufweisen, wie Array2 Zeilen hat"
			},
			{
				name: "Array2",
				description: "ist das erste Array zu multiplizierender Zahlen und muss die gleiche Anzahl von Spalten aufweisen, wie Array2 Zeilen hat"
			}
		]
	},
	{
		name: "MODALWERT",
		description: "Gibt den häufigsten Wert in einem Array oder einer Datengruppe zurück.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "Zahl1",
				description: "sind 1 bis 255 Zahlen oder Namen, Arrays oder Bezüge, die Zahlen enthalten, deren Modalwert (Modus) Sie berechnen möchten"
			},
			{
				name: "Zahl2",
				description: "sind 1 bis 255 Zahlen oder Namen, Arrays oder Bezüge, die Zahlen enthalten, deren Modalwert (Modus) Sie berechnen möchten"
			}
		]
	},
	{
		name: "MODUS.EINF",
		description: "Gibt den häufigsten Wert in einem Array oder einer Datengruppe zurück.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "Zahl1",
				description: "sind 1 bis 255 Zahlen oder Namen, Arrays oder Bezüge, die Zahlen enthalten, deren Modalwert (Modus) Sie berechnen möchten"
			},
			{
				name: "Zahl2",
				description: "sind 1 bis 255 Zahlen oder Namen, Arrays oder Bezüge, die Zahlen enthalten, deren Modalwert (Modus) Sie berechnen möchten"
			}
		]
	},
	{
		name: "MODUS.VIELF",
		description: "Gibt ein vertikales Array der am häufigsten vorkommenden oder wiederholten Werte in einem Array oder einem Datenbereich zurück. Verwenden Sie für ein horizontales Array =MTRANS(MODUSPOLY(Zahl1,Zahl2,...)).",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "Zahl1",
				description: "sind 1 bis 255 Zahlen oder Namen, Arrays oder Bezüge, die Zahlen enthalten, für die der Modus angewendet werden soll"
			},
			{
				name: "Zahl2",
				description: "sind 1 bis 255 Zahlen oder Namen, Arrays oder Bezüge, die Zahlen enthalten, für die der Modus angewendet werden soll"
			}
		]
	},
	{
		name: "MONAT",
		description: "Gibt eine Zahl von 1 (Januar) bis 12 (Dezember) für den Monat zurück.",
		arguments: [
			{
				name: "Zahl",
				description: "ist der Code für Datum und Zeit, den Spreadsheet für Datums- und Zeitberechnungen verwendet"
			}
		]
	},
	{
		name: "MONATSENDE",
		description: "Gibt die fortlaufende Zahl des letzten Tags des Monats vor oder nach einer bestimmten Anzahl von Monaten.",
		arguments: [
			{
				name: "Ausgangsdatum",
				description: "ist eine fortlaufende Zahl zurück, die das Ausgangsdatum darstellt"
			},
			{
				name: "Monate",
				description: "ist die Anzahl der Monate vor oder nach dem Ausgangsdatum"
			}
		]
	},
	{
		name: "MTRANS",
		description: "Gibt die transponierte Matrix der angegebenen Matrix zurück.",
		arguments: [
			{
				name: "Matrix",
				description: "ist eine Matrix in einem Arbeitsblatt oder einer Makrovorlage, die Sie transponieren möchten"
			}
		]
	},
	{
		name: "N",
		description: "Wandelt einen nicht-numerischen Wert in eine Zahl, ein Datum in eine serielle Zahl, und WAHR in die Zahl 1 um. Alle anderen Werte werden in die Zahl 0 umgewandelt.",
		arguments: [
			{
				name: "Wert",
				description: "ist der Wert, den Sie in eine Zahl umwandeln möchten"
			}
		]
	},
	{
		name: "NBW",
		description: "Gibt den Nettobarwert  einer Investition auf Basis eines Abzinsungsfaktors für eine Reihe periodischer Zahlungen (negative Werte) und Erträge (positive Werte) zurück.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "Zins",
				description: "ist der Abzinsungssatz für die Dauer einer Periode"
			},
			{
				name: "Wert1",
				description: "sind 1 bis 254 Zahlungen und Einzahlungen, die gleichmäßig über die Zeit verteilt sind und am Ende jeder Periode stattfinden"
			},
			{
				name: "Wert2",
				description: "sind 1 bis 254 Zahlungen und Einzahlungen, die gleichmäßig über die Zeit verteilt sind und am Ende jeder Periode stattfinden"
			}
		]
	},
	{
		name: "NEGBINOM.VERT",
		description: "Gibt Wahrscheinlichkeiten einer negativen, binomial verteilten Zufallsvariablen zurück.",
		arguments: [
			{
				name: "Zahl_Mißerfolge",
				description: "ist die Zahl der ungünstigen Ereignisse."
			},
			{
				name: "Zahl_Erfolge",
				description: "ist die Zahl der günstigen Ereignisse"
			},
			{
				name: "Erfolgswahrsch",
				description: "ist die Wahrscheinlichkeit für den günstigen Ausgang des Experiments, eine Zahl zwischen 0 und 1"
			},
			{
				name: "kumuliert",
				description: "ist ein Wahrheitswert: für die kumulierte Verteilungsfunktion WAHR; für die Wahrscheinlichkeitsmengenfunktion FALSCH"
			}
		]
	},
	{
		name: "NEGBINOMVERT",
		description: "Gibt Wahrscheinlichkeiten einer negativen, binomial verteilten Zufallsvariablen zurück.",
		arguments: [
			{
				name: "Zahl_Mißerfolge",
				description: "ist die Zahl der ungünstigen Ereignisse."
			},
			{
				name: "Zahl_Erfolge",
				description: "ist die Zahl der günstigen Ereignisse"
			},
			{
				name: "Erfolgswahrsch",
				description: "ist die Wahrscheinlichkeit für den günstigen Ausgang des Experiments, eine Zahl zwischen 0 und 1"
			}
		]
	},
	{
		name: "NETTOARBEITSTAGE",
		description: "Gibt die Anzahl der Arbeitstage in einem Zeitintervall zurück.",
		arguments: [
			{
				name: "Ausgangsdatum",
				description: "ist die fortlaufende Zahl, die das Ausgangsdatum repräsentiert"
			},
			{
				name: "Enddatum",
				description: "ist die fortlaufende Zahl, die Enddatum repräsentiert"
			},
			{
				name: "Freie_Tage",
				description: "ist eine optionale Gruppe von ein oder mehreren fortlaufenden Zahlen, die alle Arten von arbeitsfreien Tagen (Feiertage, Freischichten, etc.) repräsentieren"
			}
		]
	},
	{
		name: "NETTOARBEITSTAGE.INTL",
		description: "Gibt die Anzahl der vollständigen Arbeitstage zwischen zwei Daten mit benutzerdefinierten Wochenendparametern zurück.",
		arguments: [
			{
				name: "Ausgangsdatum",
				description: "ist die fortlaufende Zahl, die das Ausgangsdatum repräsentiert"
			},
			{
				name: "Enddatum",
				description: "ist die fortlaufende Zahl, die Enddatum repräsentiert"
			},
			{
				name: "Wochenende",
				description: "ist eine Zahl oder eine Zeichenfolge, die den Fall von Wochenenden angibt"
			},
			{
				name: "Freie_Tage",
				description: "ist eine optionale Gruppe von ein oder mehreren fortlaufenden Zahlen, die alle Arten von arbeitsfreien Tagen (Feiertage, Freischichten, usw.) darstellen"
			}
		]
	},
	{
		name: "NICHT",
		description: "Kehrt den Wert ihres Argumentes um.",
		arguments: [
			{
				name: "Wahrheitswert",
				description: "ist ein Wert oder Ausdruck, der WAHR oder FALSCH annehmen kann"
			}
		]
	},
	{
		name: "NOMINAL",
		description: "Gibt die jährliche Nominalverzinsung zurück.",
		arguments: [
			{
				name: "Effektiver_Zins",
				description: "ist die Effektivverzinsung"
			},
			{
				name: "Perioden",
				description: "ist die Anzahl der Zinszahlungen pro Jahr"
			}
		]
	},
	{
		name: "NORM.INV",
		description: "Gibt Perzentile der Normalverteilung zurück.",
		arguments: [
			{
				name: "Wahrsch",
				description: "ist die zur Normalverteilung gehörige Wahrscheinlichkeit, eine Zahl von einschließlich 0 bis einschließlich 1"
			},
			{
				name: "Mittelwert",
				description: "ist das arithmetische Mittel der Verteilung"
			},
			{
				name: "Standabwn",
				description: "ist die Standardabweichung der Verteilung, eine positive Zahl"
			}
		]
	},
	{
		name: "NORM.S.INV",
		description: "Gibt Perzentile der Standardnormalverteilung zurück.",
		arguments: [
			{
				name: "Wahrsch",
				description: "ist die zur Standardnormalverteilung gehörige Wahrscheinlichkeit, eine Zahl von einschließlich 0 bis einschließlich 1"
			}
		]
	},
	{
		name: "NORM.S.VERT",
		description: "Gibt Werte der Verteilungsfunktion standardmäßigen, normal verteilten Zufallsvariablen zurück.",
		arguments: [
			{
				name: "z",
				description: "ist der Wert der Verteilung (Perzentil), dessen Wahrscheinlichkeit Sie berechnen möchten"
			},
			{
				name: "kumuliert",
				description: "ist ein Wahrheitswert, den die Funktion zurückgeben soll: die kumulative Verteilungsfunktion = WAHR; die Wahrscheinlichkeitsmassenfunktion = FALSCH"
			}
		]
	},
	{
		name: "NORM.VERT",
		description: "Gibt Wahrscheinlichkeiten einer normal verteilten Zufallsvariablen zurück.",
		arguments: [
			{
				name: "x",
				description: "ist der Wert der Verteilung (Perzentil), dessen Wahrscheinlichkeit Sie berechnen möchten"
			},
			{
				name: "Mittelwert",
				description: "ist das arithmetische Mittel der Verteilung, eine positive Zahl"
			},
			{
				name: "Standabwn",
				description: "ist die Standardabweichung der Verteilung"
			},
			{
				name: "Kumuliert",
				description: "ist ein Wahrheitswert: für die kumulierte Verteilungsfunktion WAHR; für die Wahrscheinlichkeitsdichtefunktion FALSCH"
			}
		]
	},
	{
		name: "NORMINV",
		description: "Gibt Perzentile der Normalverteilung zurück.",
		arguments: [
			{
				name: "Wahrsch",
				description: "ist die zur Normalverteilung gehörige Wahrscheinlichkeit, eine Zahl von einschließlich 0 bis einschließlich 1"
			},
			{
				name: "Mittelwert",
				description: "ist das arithmetische Mittel der Verteilung"
			},
			{
				name: "Standabwn",
				description: "ist die Standardabweichung der Verteilung, eine positive Zahl"
			}
		]
	},
	{
		name: "NORMVERT",
		description: "Gibt Wahrscheinlichkeiten einer normal verteilten Zufallsvariablen zurück.",
		arguments: [
			{
				name: "x",
				description: "ist der Wert der Verteilung (Perzentil), dessen Wahrscheinlichkeit Sie berechnen möchten"
			},
			{
				name: "Mittelwert",
				description: "ist das arithmetische Mittel der Verteilung, eine positive Zahl"
			},
			{
				name: "Standabwn",
				description: "ist die Standardabweichung der Verteilung"
			},
			{
				name: "Kumuliert",
				description: "ist der Wahrheitswert, der den Typ der Funktion bestimmt"
			}
		]
	},
	{
		name: "NOTIERUNGBRU",
		description: "Konvertiert eine Notierung in dezimaler Schreibweise in einen gemischten Dezimalbruch.",
		arguments: [
			{
				name: "Zahl",
				description: "ist eine Dezimalzahl"
			},
			{
				name: "Teiler",
				description: "ist eine ganze Zahl, die als Nenner des Dezimalbruchs verwendet wird"
			}
		]
	},
	{
		name: "NOTIERUNGDEZ",
		description: "Konvertiert eine Notierung, die als Dezimalbruch ausgedrückt wurde, in eine Dezimalzahl.",
		arguments: [
			{
				name: "Zahl",
				description: "ist eine als Dezimalbruch ausgedrückte Zahl"
			},
			{
				name: "Teiler",
				description: "ist eine ganze Zahl, die als Nenner des Dezimalbruchs verwendet wird"
			}
		]
	},
	{
		name: "NV",
		description: "Gibt den Fehlerwert #NV (Wert nicht verfügbar) zurück.",
		arguments: [
		]
	},
	{
		name: "OBERGRENZE",
		description: "Rundet eine Zahl betragsmäßig auf das kleinste Vielfache von Schritt auf.",
		arguments: [
			{
				name: "Zahl",
				description: "ist der Wert, den Sie runden möchten"
			},
			{
				name: "Schritt",
				description: "ist der Wert, auf dessen Vielfaches Sie runden möchten"
			}
		]
	},
	{
		name: "OBERGRENZE.GENAU",
		description: "Rundet eine Zahl betragsmäßig auf das kleinste Vielfache von Schritt auf.",
		arguments: [
			{
				name: "Zahl",
				description: "ist der Wert, den Sie runden möchten"
			},
			{
				name: "Schritt",
				description: "ist der Wert, auf dessen Vielfaches Sie runden möchten"
			}
		]
	},
	{
		name: "OBERGRENZE.MATHEMATIK",
		description: "Rundet eine Zahl betragsmäßig auf das kleinste Vielfache von Schritt auf.",
		arguments: [
			{
				name: "Zahl",
				description: "ist der Wert, den Sie runden möchten"
			},
			{
				name: "Schritt",
				description: "ist der Wert, auf dessen Vielfaches Sie runden möchten"
			},
			{
				name: "Modus",
				description: "sofern angegeben und ungleich Null rundet diese Funktion von Null weg"
			}
		]
	},
	{
		name: "ODER",
		description: "Prüft, ob eines der Argumente WAHR ist und gibt WAHR oder FALSCH zurück. Nur wenn alle Argumente FALSCH sind, wird FALSCH zurückgegeben.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "Wahrheitswert1",
				description: "sind 1 bis 255 Bedingungen, die Sie überprüfen möchten und die jeweils entweder WAHR oder FALSCH sein können"
			},
			{
				name: "Wahrheitswert2",
				description: "sind 1 bis 255 Bedingungen, die Sie überprüfen möchten und die jeweils entweder WAHR oder FALSCH sein können"
			}
		]
	},
	{
		name: "OKTINBIN",
		description: "Wandelt eine oktale Zahl in eine binäre Zahl (Dualzahl) um.",
		arguments: [
			{
				name: "Zahl",
				description: "ist die oktale Zahl, die Sie umwandeln möchten"
			},
			{
				name: "Stellen",
				description: "ist die Zahl der verwendeten Stellen"
			}
		]
	},
	{
		name: "OKTINDEZ",
		description: "Wandelt eine oktale Zahl in eine dezimale Zahl um.",
		arguments: [
			{
				name: "Zahl",
				description: "ist die oktale Zahl, die Sie umwandeln möchten"
			}
		]
	},
	{
		name: "OKTINHEX",
		description: "Wandelt eine oktale Zahl in eine hexadezimale Zahl um.",
		arguments: [
			{
				name: "Zahl",
				description: "ist die oktale Zahl, die Sie umwandlen möchten"
			},
			{
				name: "Stellen",
				description: "ist die Zahl der verwendeten Stellen"
			}
		]
	},
	{
		name: "PARAMETER",
		description: "",
		arguments: [
			{
				name: "",
				description: ""
			}
		]
	},
	{
		name: "PDURATION",
		description: "Gibt die Anzahl der Zahlungsperioden zurück, die eine Investition zum Erreichen eines angegebenen Werts benötigt.",
		arguments: [
			{
				name: "Zins",
				description: "ist der Zinssatz pro Zahlungsperiode."
			},
			{
				name: "Bw",
				description: "ist der aktuelle Wert der Investition"
			},
			{
				name: "Zw",
				description: "ist der gewünschte zukünftige Wert der Investition"
			}
		]
	},
	{
		name: "PEARSON",
		description: "Gibt den Pearsonschen Korrelationskoeffizienten zurück.",
		arguments: [
			{
				name: "Matrix1",
				description: "ist eine Reihe von unabhängigen Werten"
			},
			{
				name: "Matrix2",
				description: "ist eine Reihe von abhängigen Werten"
			}
		]
	},
	{
		name: "PHI",
		description: "Gibt den Wert der Dichtefunktion für eine Standardnormalverteilung zurück.",
		arguments: [
			{
				name: "x",
				description: "ist die Zahl, für die Sie die Dichte der Standardnormalverteilung ermitteln möchten"
			}
		]
	},
	{
		name: "PI",
		description: "Gibt den Wert PI, 3.14159265358979, mit 15 Stellen Genauigkeit zurück.",
		arguments: [
		]
	},
	{
		name: "PIVOTDATENZUORDNEN",
		description: "Extrahiert Daten, die in einer PivotTable gespeichert sind.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "Datenfeld",
				description: "ist der Name des Datenfeldes, aus dem Daten extrahiert werden sollen"
			},
			{
				name: "PivotTable",
				description: "ist ein Bezug auf eine Zelle oder einen Zellenbereich, in der PivotTable, der die Daten enthält die Sie abrufen möchten"
			},
			{
				name: "Feld",
				description: "Feld für den Verweis"
			},
			{
				name: "Element",
				description: "Feldelement für den Verweis"
			}
		]
	},
	{
		name: "POISSON",
		description: "Gibt Wahrscheinlichkeiten einer Poisson-verteilten Zufallsvariablen zurück. (Die verwendete Gleichung ist in der Hilfe genauer beschrieben.).",
		arguments: [
			{
				name: "x",
				description: "ist die Zahl der Fälle"
			},
			{
				name: "Mittelwert",
				description: "ist der erwartete Zahlenwert"
			},
			{
				name: "Kumuliert",
				description: "ist der Wahrheitswert, der den Typ der Funktion bestimmt"
			}
		]
	},
	{
		name: "POISSON.VERT",
		description: "Gibt Wahrscheinlichkeiten einer Poisson-verteilten Zufallsvariablen zurück. (Die verwendete Gleichung ist in der Hilfe genauer beschrieben.).",
		arguments: [
			{
				name: "x",
				description: "ist die Zahl der Fälle"
			},
			{
				name: "Mittelwert",
				description: "ist der erwartete Zahlenwert"
			},
			{
				name: "Kumuliert",
				description: "ist der Wahrheitswert, der den Typ der Funktion bestimmt"
			}
		]
	},
	{
		name: "POLYNOMIAL",
		description: "Gibt den Polynomialkoeffizienten einer Gruppe von Zahlen.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "Zahl1",
				description: "sind 1 bis 255 Werte zurück, für die Sie den Polynomialkoeffizienten berechnen möchten"
			},
			{
				name: "Zahl2",
				description: "sind 1 bis 255 Werte zurück, für die Sie den Polynomialkoeffizienten berechnen möchten"
			}
		]
	},
	{
		name: "POTENZ",
		description: "Gibt als Ergebnis eine potenzierte Zahl zurück.",
		arguments: [
			{
				name: "Zahl",
				description: "ist die Zahl, die Sie mit dem Exponenten potenzieren möchten"
			},
			{
				name: "Potenz",
				description: "ist der Exponent, mit dem Sie die Zahl potenzieren möchten"
			}
		]
	},
	{
		name: "POTENZREIHE",
		description: "Gibt die Summe von Potenzen (zur Berechnung von Potenzreihen und dichotomen Wahrscheinlichkeiten) zurück.",
		arguments: [
			{
				name: "x",
				description: "ist der Wert der unabhängigen Variablen der Potenzreihe"
			},
			{
				name: "n",
				description: "ist die Anfangspotenz, in die Sie x erheben möchten"
			},
			{
				name: "m",
				description: "ist das Inkrement, um das Sie n in jedem Glied der Reihe vergrößern möchten"
			},
			{
				name: "Koeffizienten",
				description: "ist eine Gruppe von Koeffizienten, mit denen die aufeinanderfolgenden Potenzen der Variablen x multipliziert werden"
			}
		]
	},
	{
		name: "PRODUKT",
		description: "Multipliziert alle als Argumente angegebenen Zahlen.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "Zahl1",
				description: "sind 1 bis 255 Zahlen, logische Werte oder Textdarstellungen von Zahlen, die Sie multiplizieren möchten"
			},
			{
				name: "Zahl2",
				description: "sind 1 bis 255 Zahlen, logische Werte oder Textdarstellungen von Zahlen, die Sie multiplizieren möchten"
			}
		]
	},
	{
		name: "QIKV",
		description: "Gibt einen modifizierten internen Zinsfuß zurück, bei dem positive und negative Cashflows mit unterschiedlichen Zinssätzen finanziert werden.",
		arguments: [
			{
				name: "Werte",
				description: "ist eine Matrix von oder ein Bezug auf Zellen, die Zahlen enthalten"
			},
			{
				name: "Investition",
				description: "ist der Zinssatz, den Sie für das Geld der Cashflows berücksichtigen müssen"
			},
			{
				name: "Reinvestition",
				description: "ist der Zinssatz, den Sie für neu investierte Cashflows erzielen"
			}
		]
	},
	{
		name: "QUADRATESUMME",
		description: "Gibt die Summe der quadrierten Argumente zurück. Die Argumente können Zahlen, Arrays, Namen oder Bezüge auf Zellen sein, die Zahlen enthalten.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "Zahl1",
				description: "sind 1 bis 255 Zahlen, Arrays, Namen oder Bezüge auf Arrays, deren Summe der Quadrate Sie berechnen möchten"
			},
			{
				name: "Zahl2",
				description: "sind 1 bis 255 Zahlen, Arrays, Namen oder Bezüge auf Arrays, deren Summe der Quadrate Sie berechnen möchten"
			}
		]
	},
	{
		name: "QUANTIL",
		description: "Gibt das Alphaquantil einer Gruppe von Daten zurück.",
		arguments: [
			{
				name: "Matrix",
				description: "ist eine Matrix oder ein Datenbereich, die/der die relative Lage der Daten beschreibt"
			},
			{
				name: "Alpha",
				description: "ist der Prozentwert im geschlossenen Intervall von 0 bis 1"
			}
		]
	},
	{
		name: "QUANTIL.EXKL",
		description: "Gibt das k-Quantil von Werten in einem Bereich zurück, wobei k im Bereich von 0..1 ausschließlich liegt.",
		arguments: [
			{
				name: "Array",
				description: "ist ein Array oder ein Datenbereich, die/der die relative Lage der Daten beschreibt"
			},
			{
				name: "k",
				description: "ist der Prozentwert im geschlossenen Intervall von 0 bis 1"
			}
		]
	},
	{
		name: "QUANTIL.INKL",
		description: "Gibt das k-Quantil von Werten in einem Bereich zurück, wobei k im Bereich von 0..1 einschließlich liegt.",
		arguments: [
			{
				name: "Array",
				description: "ist ein Array oder ein Datenbereich, die/der die relative Lage der Daten beschreibt"
			},
			{
				name: "k",
				description: "ist der Prozentwert im geschlossenen Intervall von 0 bis 1"
			}
		]
	},
	{
		name: "QUANTILSRANG",
		description: "Gibt den prozentualen Rang (Alpha) eines Wertes zurück.",
		arguments: [
			{
				name: "Matrix",
				description: "ist die Matrix oder der Bereich numerischer Daten, die/der die relative Lage der Daten beschreibt"
			},
			{
				name: "x",
				description: "ist der Wert, dessen Rang Sie bestimmen möchten"
			},
			{
				name: "Genauigkeit",
				description: "ist ein optionaler Wert, der die Anzahl der Nachkommastellen des zurückgegebenen Perzentilrangs festlegt"
			}
		]
	},
	{
		name: "QUANTILSRANG.EXKL",
		description: "Gibt den prozentualen Rang (Alpha) eines Werts in einem Dataset als Prozentsatz des Datasets (0..1 ausschließlich) zurück.",
		arguments: [
			{
				name: "Array",
				description: "ist das Array oder der Bereich numerischer Daten, die/der die relative Lage der Daten beschreibt"
			},
			{
				name: "x",
				description: "ist der Wert, dessen Rang Sie bestimmen möchten"
			},
			{
				name: "Genauigkeit",
				description: "ist ein optionaler Wert, der die Anzahl der Nachkommastellen des zurückgegebenen Perzentilrangs festlegt, drei Stellen bei fehlender Angabe (0,xxx%)"
			}
		]
	},
	{
		name: "QUANTILSRANG.INKL",
		description: "Gibt den prozentualen Rang (Alpha) eines Wertes in einem Dataset als Prozentsatz des Datasets (0..1 einschließlich) zurück.",
		arguments: [
			{
				name: "Array",
				description: "ist das Array oder der Bereich numerischer Daten, die/der die relative Lage der Daten beschreibt"
			},
			{
				name: "x",
				description: "ist der Wert, dessen Rang Sie bestimmen möchten"
			},
			{
				name: "Genauigkeit",
				description: "ist ein optionaler Wert, der die Anzahl der Nachkommastellen des zurückgegebenen Perzentilrangs festlegt, drei Stellen bei fehlender Angabe (0,xxx%)"
			}
		]
	},
	{
		name: "QUARTILE",
		description: "Gibt die Quartile der Datengruppe zurück.",
		arguments: [
			{
				name: "Matrix",
				description: "ist eine Matrix oder ein Zellbereich numerischer Werte, deren Quartile Sie bestimmen möchten"
			},
			{
				name: "Quartile",
				description: "gibt an, welches Quartil berechnet werden soll"
			}
		]
	},
	{
		name: "QUARTILE.EXKL",
		description: "Gibt die Quartile eines Datasets zurück, basierend auf Perzentilwerten von 0..1 ausschließlich.",
		arguments: [
			{
				name: "Array",
				description: "ist ein Array oder ein Zellbereich numerischer Werte, deren Quartile Sie bestimmen möchten"
			},
			{
				name: "Quartile",
				description: "ist eine Zahl: Minimalwert = 0; 1. Quartil = 1; Medianwert = 2; 3. Quartil = 3; Maximalwert = 4"
			}
		]
	},
	{
		name: "QUARTILE.INKL",
		description: "Gibt die Quartile eines Datasets zurück, basierend auf Perzentilwerten von 0..1 einschließlich.",
		arguments: [
			{
				name: "Array",
				description: "ist ein Array oder ein Zellbereich numerischer Werte, deren Quartile Sie bestimmen möchten"
			},
			{
				name: "Quartile",
				description: "ist eine Zahl: Minimalwert = 0; 1. Quartil = 1; Medianwert = 2; 3. Quartil = 3; Maximalwert = 4"
			}
		]
	},
	{
		name: "QUOTIENT",
		description: "Gibt den ganzzahligen Anteil einer Division zurück.",
		arguments: [
			{
				name: "Zähler",
				description: "ist der Dividend"
			},
			{
				name: "Nenner",
				description: "ist der Divisor"
			}
		]
	},
	{
		name: "RANG",
		description: "Gibt den Rang, den eine Zahl innerhalb einer Liste von Zahlen einnimmt zurück.",
		arguments: [
			{
				name: "Zahl",
				description: "ist die Zahl, deren Rangzahl Sie bestimmen möchten"
			},
			{
				name: "Bezug",
				description: "ist eine Matrix mit Zahlen oder ein Bezug auf eine Liste von Zahlen. Nichtnumerische Werte im Bezug werden ignoriert"
			},
			{
				name: "Reihenfolge",
				description: "ist eine Zahl, die angibt, wie der Rang von Zahl bestimmt werden soll; 0 = Rang in absteigend sortierter Liste; alle anderen Werte = Rang in aufsteigend sortierter Liste"
			}
		]
	},
	{
		name: "RANG.GLEICH",
		description: "Gibt den Rang, den eine Zahl innerhalb einer Liste von Zahlen einnimmt, zurück: die Größe relativ zu anderen Werten in der Liste; wenn mehrere Werte die gleiche Rangzahl aufweisen, wird der oberste Rang dieser Gruppe von Werten zurückgegeben.",
		arguments: [
			{
				name: "Zahl",
				description: "ist die Zahl, für die der Rang ermittelt werden soll"
			},
			{
				name: "Bezug",
				description: "ist ein Array von oder ein Bezug auf eine Liste mit Zahlen. Nicht numerische Werte werden ignoriert"
			},
			{
				name: "Reihenfolge",
				description: "ist eine Zahl: Rang in der absteigend sortierten Liste = 0 oder ohne Angabe; Rang in der aufsteigend sortierten Liste = jeder Wert ungleich Null"
			}
		]
	},
	{
		name: "RANG.MITTELW",
		description: "Gibt den Rang, den eine Zahl innerhalb einer Liste von Zahlen einnimmt, zurück: die Größe relativ zu anderen Werten in der Liste; wenn mehrere Werte die gleiche Rangzahl aufweisen, wird die durchschnittliche Rangzahl zurückgegeben.",
		arguments: [
			{
				name: "Zahl",
				description: "ist die Zahl, für die der Rang ermittelt werden soll"
			},
			{
				name: "Bezug",
				description: "ist ein Array von oder ein Bezug auf eine Liste mit Zahlen. Nicht numerische Werte werden ignoriert"
			},
			{
				name: "Reihenfolge",
				description: "ist eine Zahl: Rang in der absteigend sortierten Liste = 0 oder ohne Angabe; Rang in der aufsteigend sortierten Liste = jeder Wert ungleich Null"
			}
		]
	},
	{
		name: "RECHTS",
		description: "Gibt das letzte oder die letzten Zeichen einer Zeichenfolge zurück.",
		arguments: [
			{
				name: "Text",
				description: "ist die Zeichenfolge mit den Zeichen, die Sie kopieren wollen"
			},
			{
				name: "Anzahl_Zeichen",
				description: "gibt an, wie viele Zeichen kopiert werden sollen, 1 wenn nicht angegeben"
			}
		]
	},
	{
		name: "RENDITEDIS",
		description: "Gibt die jährliche Rendite eines unverzinslichen Wertpapiers zurück.",
		arguments: [
			{
				name: "Abrechnung",
				description: "ist der Abrechnungstermin des Wertpapierkaufs, als fortlaufende Zahl angegeben"
			},
			{
				name: "Fälligkeit",
				description: "ist der Fälligkeitstermin des Wertpapiers, als fortlaufende Zahl angegeben"
			},
			{
				name: "Kurs",
				description: "ist der Kurs des Wertpapiers pro 100 EUR Nennwert"
			},
			{
				name: "Rückzahlung",
				description: "ist der Rückzahlungswert des Wertpapiers pro 100 EUR Nennwert"
			},
			{
				name: "Basis",
				description: "gibt an, auf welcher Basis die Zinstage gezählt werden"
			}
		]
	},
	{
		name: "REST",
		description: "Gibt den Rest einer Division zurück.",
		arguments: [
			{
				name: "Zahl",
				description: "ist die Zahl, deren Rest aus einer Division Sie wissen möchten"
			},
			{
				name: "Divisor",
				description: "ist die Zahl, durch die Zahl dividiert werden soll"
			}
		]
	},
	{
		name: "RGP",
		description: "Gibt die Parameter eines linearen Trends zurück.",
		arguments: [
			{
				name: "Y_Werte",
				description: "sind die y-Werte, die Ihnen bereits aus der Beziehung y = mx + b bekannt sind zurück"
			},
			{
				name: "X_Werte",
				description: "sind optionale x-Werte, die Ihnen eventuell bereits aus der Beziehung y = mx + b bekannt sind"
			},
			{
				name: "Konstante",
				description: "ist ein Wahrheitswert, der angibt, ob die Konstante b den Wert 0 annehmen soll"
			},
			{
				name: "Stats",
				description: "ist ein Wahrheitswert, der angibt, ob weitere Regressionskenngrößen ausgegeben werden sollen: WAHR = weitere Regressionskenngrößen sollen ausgegeben werden; FALSCH oder fehlt = der m-Koeffizient und die Konstante b werden zurückgegeben"
			}
		]
	},
	{
		name: "RKP",
		description: "Gibt die Parameter eines exponentiellen Trends zurück.",
		arguments: [
			{
				name: "Y_Werte",
				description: "sind die y-Werte, die Ihnen bereits aus der Beziehung y = b*m^x bekannt sind"
			},
			{
				name: "X_Werte",
				description: "sind optionale x-Werte, die Ihnen eventuell bereits aus der Beziehung y = b*m^x bekannt sind"
			},
			{
				name: "Konstante",
				description: "ist ein Wahrheitswert, der angibt, ob die Konstante b den Wert 1 annehmen soll"
			},
			{
				name: "Stats",
				description: "ist ein Wahrheitswert, der angibt, ob weitere Regressionskenngrößen ausgegeben werden sollen: WAHR = weitere Regressionskenngrößen sollen ausgegeben werden; FALSCH oder fehlt = der m-Koeffizient und die Konstante b werden zurückgegeben"
			}
		]
	},
	{
		name: "RMZ",
		description: "Gibt die konstante Zahlung einer Annuität pro Periode zurück.",
		arguments: [
			{
				name: "Zins",
				description: "ist der Zinssatz pro Periode (Zahlungszeitraum) Z.B. verwenden Sie 6%/4 für Quartalszahlungen von 6%"
			},
			{
				name: "Zzr",
				description: "gibt an, über wie viele Perioden die jeweilige Annuität (Rente) gezahlt wird"
			},
			{
				name: "Bw",
				description: "ist der Barwert: Der Gesamtbetrag, den eine Reihe zukünftiger Zahlungen zum gegenwärtigen Zeitpunkt wert ist"
			},
			{
				name: "Zw",
				description: "ist der zukünftige Wert (Endwert) oder der Kassenbestand, den Sie nach der letzten Zahlung erreicht haben möchten"
			},
			{
				name: "F",
				description: "kann den Wert 0 oder 1 annehmen und gibt an, wann Zahlungen fällig sind (Fälligkeit): 1 = Zahlung an Beginn der Periode, 0 = Zahlung am Ende der Periode"
			}
		]
	},
	{
		name: "RÖMISCH",
		description: "Wandelt eine arabische Zahl in eine römische Zahl als Text um.",
		arguments: [
			{
				name: "Zahl",
				description: "ist die arabische Zahl, die Sie umwandeln wollen"
			},
			{
				name: "Typ",
				description: "ist eine Zahl, die den Typ der römischen Zahl festlegt"
			}
		]
	},
	{
		name: "RTD",
		description: "Empfängt Echtzeitdaten von einem Programm, das COM-Automatisierung unterstützt.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "ProgID",
				description: "der Name der ProgID eines registrierten COM Auomatisierungs-Add-Ins. Schließen Sie den Namen in Anführungszeichen ein"
			},
			{
				name: "Server",
				description: "der Name des Servers, auf dem das Add-In ausgeführt werden soll. Schließen Sie den Namen in Anführungszeichen ein. Wenn das Add-In lokal ausgeführt werden soll, verwenden Sie eine leere Zeichenfolge"
			},
			{
				name: "Topic1",
				description: "1 bis 38 Parameter, die Bestandteile der Daten enthalten"
			},
			{
				name: "Topic2",
				description: "1 bis 38 Parameter, die Bestandteile der Daten enthalten"
			}
		]
	},
	{
		name: "RUNDEN",
		description: "Rundet eine Zahl auf eine bestimmte Anzahl an Dezimalstellen.",
		arguments: [
			{
				name: "Zahl",
				description: "ist die Zahl, die Sie auf- oder abrunden möchten"
			},
			{
				name: "Anzahl_Stellen",
				description: "gibt an, auf wie viele Dezimalstellen Sie die Zahl auf- oder abrunden möchten. Negative Werte runden auf ganze Zehnerpotenzen: RUNDEN(225;-2) ergibt 200. 0 rundet auf die nächste Ganzzahl"
			}
		]
	},
	{
		name: "SÄUBERN",
		description: "Löscht alle nicht druckbaren Zeichen aus einem Text.",
		arguments: [
			{
				name: "Text",
				description: "ist jede beliebige Arbeitsblattinformation, aus der Sie die nicht druckbaren Zeichen entfernen möchten"
			}
		]
	},
	{
		name: "SCHÄTZER",
		description: "Gibt den Schätzwert für einen linearen Trend zurück.",
		arguments: [
			{
				name: "x",
				description: "ist der Datenpunkt, dessen Wert Sie schätzen möchten."
			},
			{
				name: "Y_Werte",
				description: "ist eine Matrix oder ein abhängiger Datenbereich"
			},
			{
				name: "X_Werte",
				description: "ist eine Matrix oder ein unabhängiger Datenbereich"
			}
		]
	},
	{
		name: "SCHIEFE",
		description: "Gibt die Schiefe einer Verteilung zurück: eine Charakterisierung des Assymmetriegrads einer Verteilung um seinen Mittelwert.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "Zahl1",
				description: "sind 1 bis 255 Zahlen oder Namen, Arrays oder Bezüge, die Zahlen enthalten, deren Schiefe Sie berechnen möchten"
			},
			{
				name: "Zahl2",
				description: "sind 1 bis 255 Zahlen oder Namen, Arrays oder Bezüge, die Zahlen enthalten, deren Schiefe Sie berechnen möchten"
			}
		]
	},
	{
		name: "SCHIEFE.P",
		description: "Gibt die Schiefe einer Verteilung auf der Basis einer Grundgesamtheit zurück: eine Charakterisierung des Asymmetriegrads einer Verteilung um ihren Mittelwert.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "Zahl1",
				description: "sind 1 bis 254 Zahlen oder Namen, Matrizen oder Bezüge, die Zahlen enthalten, deren Schiefe bezogen auf die Grundgesamtheit Sie berechnen möchten"
			},
			{
				name: "Zahl2",
				description: "sind 1 bis 254 Zahlen oder Namen, Matrizen oder Bezüge, die Zahlen enthalten, deren Schiefe bezogen auf die Grundgesamtheit Sie berechnen möchten"
			}
		]
	},
	{
		name: "SEC",
		description: "Gibt den Sekans eines Winkels zurück.",
		arguments: [
			{
				name: "Zahl",
				description: "ist der im Bogenmaß angegebene Winkel, dessen Sekans Sie berechnen möchten"
			}
		]
	},
	{
		name: "SECHYP",
		description: "Gibt den hyperbolischen Sekans eines Winkels zurück.",
		arguments: [
			{
				name: "Zahl",
				description: "ist der im Bogenmaß angegebene Winkel, dessen hyperbolischen Sekans Sie berechnen möchten"
			}
		]
	},
	{
		name: "SEKUNDE",
		description: "Wandelt eine fortlaufende Zahl in einen Wert von 0 bis 59 für die Sekunde um.",
		arguments: [
			{
				name: "Zahl",
				description: "ist der Code für Datum und Zeit, den Spreadsheet für Datums- und Zeitberechnungen verwendet"
			}
		]
	},
	{
		name: "SIN",
		description: "Gibt den Sinus einer Zahl zurück.",
		arguments: [
			{
				name: "Zahl",
				description: "ist der Winkel im Bogenmaß, für den Sie den Sinus berechnen wollen. Grad * PI()/180 = RAD"
			}
		]
	},
	{
		name: "SINHYP",
		description: "Gibt den hyperbolischen Sinus einer Zahl zurück. (Die verwendete Gleichung ist in der Hilfe genauer beschrieben.).",
		arguments: [
			{
				name: "Zahl",
				description: "ist eine beliebige reelle Zahl"
			}
		]
	},
	{
		name: "SPALTE",
		description: "Gibt die Spaltennummer eines Bezuges zurück.",
		arguments: [
			{
				name: "Bezug",
				description: "ist die Zelle oder der Zellbereich, deren bzw. dessen Spaltennummer Sie ermitteln möchten. Wenn der Parameter fehlt, wird die Zelle zurückgegeben, die die Funktion enthält"
			}
		]
	},
	{
		name: "SPALTEN",
		description: "Gibt die Anzahl der Spalten eines Bezuges zurück.",
		arguments: [
			{
				name: "Matrix",
				description: "ist eine Matrix, eine Matrixformel oder ein Bezug auf einen Zellbereich, dessen Spaltenanzahl Sie abfragen möchten"
			}
		]
	},
	{
		name: "STABW",
		description: "Schätzt die Standardabweichung ausgehend von einer Stichprobe (logische Werte und Text werden im Beispiel ignoriert).",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "Zahl1",
				description: "sind 1 bis 255 Zahlen, die einer Stichprobe einer Grundgesamtheit entsprechen und können Zahlen oder Bezüge darstellen, die Zahlen enthalten"
			},
			{
				name: "Zahl2",
				description: "sind 1 bis 255 Zahlen, die einer Stichprobe einer Grundgesamtheit entsprechen und können Zahlen oder Bezüge darstellen, die Zahlen enthalten"
			}
		]
	},
	{
		name: "STABW.N",
		description: "Berechnet die Standardabweichung, ausgehend von der Grundgesamtheit angegeben als Argumente (logische Werte und Text werden ignoriert).",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "Zahl1",
				description: " sind 1 bis 255 einer Grundgesamtheit entsprechende Zahlen und können Zahlen oder Bezüge sein, die Zahlen enthalten"
			},
			{
				name: "Zahl2",
				description: " sind 1 bis 255 einer Grundgesamtheit entsprechende Zahlen und können Zahlen oder Bezüge sein, die Zahlen enthalten"
			}
		]
	},
	{
		name: "STABW.S",
		description: "Schätzt die Standardabweichung ausgehend von einer Stichprobe (logische Werte und Text werden im Beispiel ignoriert).",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "Zahl1",
				description: "sind 1 bis 255 Zahlen, die einer Stichprobe einer Grundgesamtheit entsprechen und können Zahlen oder Bezüge darstellen, die Zahlen enthalten"
			},
			{
				name: "Zahl2",
				description: "sind 1 bis 255 Zahlen, die einer Stichprobe einer Grundgesamtheit entsprechen und können Zahlen oder Bezüge darstellen, die Zahlen enthalten"
			}
		]
	},
	{
		name: "STABWA",
		description: "Schätzt die Standardabweichung, ausgehend von einer Stichprobe, einschließlich logischer Werte und Text. Dabei werden Text und FALSCH als 0 interpretiert, WAHR wird als 1 interpretiert.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "Wert1",
				description: "sind 1 bis 255 Werte, die zu einer Stichprobe einer Grundgesamtheit gehören und können Zahlen, Namen, Arrays oder Bezüge sein"
			},
			{
				name: "Wert2",
				description: "sind 1 bis 255 Werte, die zu einer Stichprobe einer Grundgesamtheit gehören und können Zahlen, Namen, Arrays oder Bezüge sein"
			}
		]
	},
	{
		name: "STABWN",
		description: "Berechnet die Standardabweichung, ausgehend von der Grundgesamtheit angegeben als Argumente (logische Werte und Text werden ignoriert).",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "Zahl1",
				description: " sind 1 bis 255 einer Grundgesamtheit entsprechende Zahlen und können Zahlen oder Bezüge sein, die Zahlen enthalten"
			},
			{
				name: "Zahl2",
				description: " sind 1 bis 255 einer Grundgesamtheit entsprechende Zahlen und können Zahlen oder Bezüge sein, die Zahlen enthalten"
			}
		]
	},
	{
		name: "STABWNA",
		description: "Berechnet die Standardabweichung ausgehend von der Grundgesamtheit, einschließlich logischer Werte und Text. Dabei werden Text und FALSCH als 0 interpretiert, WAHR wird als 1 interpretiert.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "Wert1",
				description: "sind 1 bis 255 Werte, die zu einer Grundgesamtheit gehören, sie können Zahlen, Namen, Arrays oder Bezüge sein, die Werte enthalten"
			},
			{
				name: "Wert2",
				description: "sind 1 bis 255 Werte, die zu einer Grundgesamtheit gehören, sie können Zahlen, Namen, Arrays oder Bezüge sein, die Werte enthalten"
			}
		]
	},
	{
		name: "STANDARDISIERUNG",
		description: "Gibt den standardisierten Wert zurück.",
		arguments: [
			{
				name: "x",
				description: "ist der Wert, den Sie standardisieren möchten"
			},
			{
				name: "Mittelwert",
				description: "ist das arithmetische Mittel der Verteilung"
			},
			{
				name: "Standabwn",
				description: "ist die Standardabweichung der Verteilung, eine positive Zahl"
			}
		]
	},
	{
		name: "STANDNORMINV",
		description: "Gibt Perzentile der Standardnormalverteilung zurück.",
		arguments: [
			{
				name: "Wahrsch",
				description: "ist die zur Standardnormalverteilung gehörige Wahrscheinlichkeit, eine Zahl von einschließlich 0 bis einschließlich 1"
			}
		]
	},
	{
		name: "STANDNORMVERT",
		description: "Gibt Werte der Verteilungsfunktion standardmäßigen, normal verteilten Zufallsvariablen zurück.",
		arguments: [
			{
				name: "z",
				description: "ist der Wert der Verteilung (Perzentil), dessen Wahrscheinlichkeit Sie berechnen möchten"
			}
		]
	},
	{
		name: "STEIGUNG",
		description: "Gibt die Steigung der Regressionsgeraden zurück.",
		arguments: [
			{
				name: "Y_Werte",
				description: "ist eine Matrix oder ein Zellbereich von numerisch abhängigen Datenpunkten, Argumente können Zahlen, Namen, Arrays oder Bezüge sein"
			},
			{
				name: "X_Werte",
				description: "ist eine Reihe von unabhängigen Datenpunkten, Argumente können Zahlen, Namen, Arrays oder Bezüge sein"
			}
		]
	},
	{
		name: "STFEHLERYX",
		description: "Gibt den Standardfehler der geschätzten y-Werte für alle x-Werte der Regression zurück.",
		arguments: [
			{
				name: "Y_Werte",
				description: "ist eine Matrix oder ein Bereich von abhängigen Datenpunkten, Argumente können Zahlen, Namen, Arrays oder Bezüge sein"
			},
			{
				name: "X_Werte",
				description: "ist eine Matrix oder ein Bereich von unabhängigen Datenpunkten, Argumente können Zahlen, Namen, Arrays oder Bezüge sein"
			}
		]
	},
	{
		name: "STUNDE",
		description: "Liefert den Wert für die Stunde von 0 (00:00:00 Uhr) bis 23 (23:00:00 Uhr) zurück.",
		arguments: [
			{
				name: "Zahl",
				description: "ist der Code für Datum und Zeit, den Spreadsheet für Datums- und Zeitberechnungen verwendet"
			}
		]
	},
	{
		name: "SUCHEN",
		description: "Sucht eine Zeichenfolge innerhalb einer anderen (Groß-/Kleinschreibung wird nicht beachtet).",
		arguments: [
			{
				name: "Suchtext",
				description: "ist der Text, den Sie finden wollen. Sie können Stellvertreterzeichen verwenden (* und ?); verwenden Sie ~? Und ~*, um die Zeichen ? Und * selber zu suchen"
			},
			{
				name: "Text",
				description: "ist der Text, in dem Sie nach Suchtext suchen wollen"
			},
			{
				name: "Erstes_Zeichen",
				description: "ist die Nummer des Zeichens in Text (von links nach rechts), ab der Sie mit der Suche beginnen wollen"
			}
		]
	},
	{
		name: "SUMME",
		description: "Summiert die Zahlen in einem Zellenbereich.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "Zahl1",
				description: "sind 1 bis 255 Zahlen, deren Summe Sie berechnen möchten. Logische Werte und Text werden in Zellen ignoriert, jedoch berücksichtigt, wenn sie als Argumente eingegeben werden"
			},
			{
				name: "Zahl2",
				description: "sind 1 bis 255 Zahlen, deren Summe Sie berechnen möchten. Logische Werte und Text werden in Zellen ignoriert, jedoch berücksichtigt, wenn sie als Argumente eingegeben werden"
			}
		]
	},
	{
		name: "SUMMENPRODUKT",
		description: "Gibt die Summe der Produkte der entsprechenden Bereiche und Arrays zurück.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "Array1",
				description: "sind 2 bis 255 Arrays, deren Komponenten Sie zunächst multiplizieren und anschließend addieren möchten"
			},
			{
				name: "Array2",
				description: "sind 2 bis 255 Arrays, deren Komponenten Sie zunächst multiplizieren und anschließend addieren möchten"
			},
			{
				name: "Array3",
				description: "sind 2 bis 255 Arrays, deren Komponenten Sie zunächst multiplizieren und anschließend addieren möchten"
			}
		]
	},
	{
		name: "SUMMEWENN",
		description: "Addiert Zahlen, die mit den Suchkriterien übereinstimmen.",
		arguments: [
			{
				name: "Bereich",
				description: "ist der Zellbereich, den Sie berechnen wollen"
			},
			{
				name: "Suchkriterien",
				description: "ist das Suchkriterium als Zahl, Formel, oder Text, das festlegt, welche Zellen addiert werden"
			},
			{
				name: "Summe_Bereich",
				description: "sind die Zellen, die Sie summieren wollen"
			}
		]
	},
	{
		name: "SUMMEWENNS",
		description: "Addiert die Zellen, die von einer bestimmten Gruppe von Bedingungen oder Kriterien angegeben sind.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "Summe_Bereich",
				description: "sind die tatsächlich zu addierenden Zellen."
			},
			{
				name: "Kriterien_Bereich",
				description: "ist der Bereich von Zellen, die für die jeweiligen Bedingungen ausgewertet werden sollen"
			},
			{
				name: "Kriterien",
				description: "ist die Bedingung oder sind die Kriterien in Form einer Zahl, eines Ausdrucks oder Texts, die definieren, welche Zellen addiert werden"
			}
		]
	},
	{
		name: "SUMMEX2MY2",
		description: "Summiert für zusammengehörige Komponenten zweier Matrizen die Differenzen der Quadrate.",
		arguments: [
			{
				name: "Matrix_x",
				description: "ist die erste Matrix oder der erste Wertebereich, Argumente können Zahlen, Namen, Arrays oder Bezüge sein"
			},
			{
				name: "Matrix_y",
				description: "ist die zweite Matrix oder der zweite Wertebereich, Argumente können Zahlen, Namen, Arrays oder Bezüge sein"
			}
		]
	},
	{
		name: "SUMMEX2PY2",
		description: "Summiert für zusammengehörige Komponenten zweier Matrizen die Summen der Quadrate.",
		arguments: [
			{
				name: "Matrix_x",
				description: "ist die erste Matrix oder der erste Wertebereich, Argumente können Zahlen, Namen, Arrays oder Bezüge sein"
			},
			{
				name: "Matrix_y",
				description: "ist die zweite Matrix oder der zweite Wertebereich, Argumente können Zahlen, Namen, Arrays oder Bezüge sein"
			}
		]
	},
	{
		name: "SUMMEXMY2",
		description: "Summiert für zusammengehörige Komponenten zweier Matrizen die quadrierten Differenzen.",
		arguments: [
			{
				name: "Matrix_x",
				description: "ist die erste Matrix oder der erste Wertebereich, Argumente können Zahlen, Namen, Arrays oder Bezüge sein"
			},
			{
				name: "Matrix_y",
				description: "ist die zweite Matrix oder der zweite Wertebereich, Argumente können Zahlen, Namen, Arrays oder Bezüge sein"
			}
		]
	},
	{
		name: "SUMQUADABW",
		description: "Gibt die Summe der quadrierten Abweichungen der Datenpunkte von ihrem Stichprobenmittelwert zurück.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "Zahl1",
				description: "sind 1 bis 255 Argumente bzw. ein Array oder Arraybezug, deren quadratische Abweichungen Sie berechnen möchten"
			},
			{
				name: "Zahl2",
				description: "sind 1 bis 255 Argumente bzw. ein Array oder Arraybezug, deren quadratische Abweichungen Sie berechnen möchten"
			}
		]
	},
	{
		name: "SVERWEIS",
		description: "Durchsucht die erste Spalte einer Matrix und durchläuft die Zeile nach rechts, um den Wert einer Zelle zurückzugeben.",
		arguments: [
			{
				name: "Suchkriterium",
				description: "ist der Wert, nach dem Sie in der ersten Spalte der Matrix suchen"
			},
			{
				name: "Matrix",
				description: "ist die Informationstabelle, in der Daten gesucht werden"
			},
			{
				name: "Spaltenindex",
				description: "ist die Nummer der Spalte in der Mehrfachoperationsmatrix, aus der der übereinstimmende Wert  zurückgegeben werden soll"
			},
			{
				name: "Bereich_Verweis",
				description: "gibt an, ob eine genaue Übereinstimmung gefunden werden soll: WAHR = aus der aufsteigend sortierten Reihenfolge der Werte wird der Wert zurückgegeben, der am dichtesten am gesuchten Wert liegt; FALSCH = es wird eine genaue Übereinstimmung gesucht"
			}
		]
	},
	{
		name: "T",
		description: "Wandelt die Argumente in Text um.",
		arguments: [
			{
				name: "Wert",
				description: "ist der Wert, den Sie überprüfen wollen. Wenn der Wert kein Text ist, werden zwei Anführungsstriche (leerer Text) zurück gegeben."
			}
		]
	},
	{
		name: "T.INV",
		description: "Gibt linksseitige Quantile der (Student) t-Verteilung zurück.",
		arguments: [
			{
				name: "Wahrsch",
				description: "ist die zur zweiseitigen (Student) t-Verteilung gehörige Wahrscheinlichkeit, eine Zahl von einschließlich 0 bis einschließlich 1"
			},
			{
				name: "Freiheitsgrade",
				description: "ist eine positive Ganzzahl, die die Anzahl der Freiheitsgrade angibt, durch die die Verteilung bestimmt ist"
			}
		]
	},
	{
		name: "T.INV.2S",
		description: "Gibt zweiseitige Quantile der (Student) t-Verteilung zurück.",
		arguments: [
			{
				name: "Wahrsch",
				description: "ist die zur zweiseitigen (Student) t-Verteilung gehörige Wahrscheinlichkeit, eine Zahl von einschließlich 0 bis einschließlich 1"
			},
			{
				name: "Freiheitsgrade",
				description: "ist eine positive Ganzzahl, die die Anzahl der Freiheitsgrade angibt, durch die die Verteilung bestimmt ist"
			}
		]
	},
	{
		name: "T.TEST",
		description: "Gibt die Teststatistik eines Studentschen t-Tests zurück.",
		arguments: [
			{
				name: "Matrix1",
				description: "ist die erste Datengruppe"
			},
			{
				name: "Matrix2",
				description: "ist die zweite Datengruppe"
			},
			{
				name: "Seiten",
				description: "bestimmt die Anzahl der Endflächen"
			},
			{
				name: "Typ",
				description: "bestimmt die Form des durchzuführenden t-Tests"
			}
		]
	},
	{
		name: "T.VERT",
		description: "Liefert die (Student) t-Verteilung der linken Endfläche zurück.",
		arguments: [
			{
				name: "x",
				description: "ist der numerische Wert, für den die Verteilung ausgewertet werden soll"
			},
			{
				name: "Freiheitsgrade",
				description: "ist eine Ganzzahl, die die Anzahl der Freiheitsgrade angibt, durch die die Verteilung bestimmt ist"
			},
			{
				name: "kumuliert",
				description: "ist ein logischer Wert: für die kumulierte Verteilungsfunktion WAHR; für die Wahrscheinlichkeitsdichtefunktion FALSCH"
			}
		]
	},
	{
		name: "T.VERT.2S",
		description: "Liefert die (Student) t-Verteilung für zwei Endflächen zurück.",
		arguments: [
			{
				name: "x",
				description: "ist der numerische Wert, für den die Verteilung ausgewertet werden soll"
			},
			{
				name: "Freiheitsgrade",
				description: "ist eine Ganzzahl, die die Anzahl der Freiheitsgrade angibt, durch die die Verteilung bestimmt ist"
			}
		]
	},
	{
		name: "T.VERT.RE",
		description: "Liefert die (Student) t-Verteilung für die rechte Endfläche zurück.",
		arguments: [
			{
				name: "x",
				description: "ist der numerische Wert, für den die Verteilung ausgewertet werden soll"
			},
			{
				name: "Freiheitsgrade",
				description: "ist eine Ganzzahl, die die Anzahl der Freiheitsgrade angibt, durch die die Verteilung bestimmt ist"
			}
		]
	},
	{
		name: "TAG",
		description: "Wandelt eine fortlaufende Zahl in eine Zahl von 1 bis 31 für den Tag des Monats um.",
		arguments: [
			{
				name: "Zahl",
				description: "ist der Code für Datum und Zeit, den Spreadsheet für Datums- und Zeitberechnungen verwendet"
			}
		]
	},
	{
		name: "TAGE",
		description: "Gibt die Anzahl der Tage zwischen den beiden Datumswerten zurück.",
		arguments: [
			{
				name: "Zieldatum",
				description: "Ausgangsdatum und Zieldatum sind die beiden Datumswerte, für die die Anzahl der dazwischen liegenden Tage ermittelt werden soll"
			},
			{
				name: "Ausgangsdatum",
				description: "Ausgangsdatum und Zieldatum sind die beiden Datumswerte, für die die Anzahl der dazwischen liegenden Tage ermittelt werden soll"
			}
		]
	},
	{
		name: "TAGE360",
		description: "Berechnet, ausgehend von einem Jahr, das 360 (12 Monate mit je 30 Tagen)Tage umfasst, die Anzahl der zwischen zwei Tagesdaten liegenden Tage.",
		arguments: [
			{
				name: "Ausgangsdatum",
				description: "ist das Datum des ersten Tages der Zeitperiode, die Sie berechnen möchten"
			},
			{
				name: "Enddatum",
				description: "ist das Datum des letzten Tages der Zeitperiode, die Sie berechnen möchten"
			},
			{
				name: "Methode",
				description: "ist ein Wahrheitswert, der die europäische Berechnungsmethode bestimmt: FALSCH oder fehlt = U.S. (NASD); WAHR = Europäisch."
			}
		]
	},
	{
		name: "TAN",
		description: "Gibt den Tangens einer Zahl zurück.",
		arguments: [
			{
				name: "Zahl",
				description: "ist der Winkel im Bogenmaß, für den Sie den Tangens ermitteln wollen. Grad * PI()/180 = RAD"
			}
		]
	},
	{
		name: "TANHYP",
		description: "Gibt den hyperbolischen Tangens einer Zahl zurück.",
		arguments: [
			{
				name: "Zahl",
				description: "ist eine reelle Zahl"
			}
		]
	},
	{
		name: "TBILLÄQUIV",
		description: "Gibt die Rendite eines Wertpapiers zurück.",
		arguments: [
			{
				name: "Abrechnung",
				description: "ist der Abrechnungstermin des Wertpapierkaufs, angegeben als fortlaufende Zahl"
			},
			{
				name: "Fälligkeit",
				description: "ist das Fälligkeitsdatum des Wertpapiers, angegeben als fortlaufende Zahl"
			},
			{
				name: "Abzinsungssatz",
				description: "ist der Abzinsungssatz des Wertpapiers"
			}
		]
	},
	{
		name: "TBILLKURS",
		description: "Gibt den Kurs pro 100 EUR Nennwert eines Wertpapiers.",
		arguments: [
			{
				name: "Abrechnung",
				description: "ist der Abrechnungstermin des Wertpapierkaufs, angegeben als fortlaufende Zahl"
			},
			{
				name: "Fälligkeit",
				description: "ist der Fälligkeitstermin des Wertpapiers, angegeben als fortlaufende Zahl"
			},
			{
				name: "Abzinsungssatz",
				description: "ist der Abzinsungssatz des Wertpapiers"
			}
		]
	},
	{
		name: "TBILLRENDITE",
		description: "Gibt die Rendite eines Wertpapiers zurück.",
		arguments: [
			{
				name: "Abrechnung",
				description: "ist der Abrechnungstermin des Wertpapierkaufs, angegeben als fortlaufende Zahl"
			},
			{
				name: "Fälligkeit",
				description: "ist der Fälligkeitstermin des Wertpapiers, angegeben als fortlaufende Zahl"
			},
			{
				name: "pr",
				description: "ist der Kurs des Wertpapiers pro 100 EUR Nennwert"
			}
		]
	},
	{
		name: "TEIL",
		description: "Gibt eine bestimmte Anzahl Zeichen einer Zeichenfolge ab der von Ihnen bestimmten Stelle zurück.",
		arguments: [
			{
				name: "Text",
				description: "ist die Zeichenfolge mit den Zeichen, die Sie kopieren wollen"
			},
			{
				name: "Erstes_Zeichen",
				description: "ist die Position des ersten Zeichens, das Sie aus Text kopieren möchten"
			},
			{
				name: "Anzahl_Zeichen",
				description: "gibt an, wie viele Zeichen aus Text zurückgegeben werden sollen"
			}
		]
	},
	{
		name: "TEILERGEBNIS",
		description: "Gibt ein Teilergebnis in einer Liste oder Datenbank zurück.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "Funktion",
				description: "ist eine Zahl (1 bis 11), die festlegt, welche Funktion in der Berechnung des Teilergebnisses verwendet werden soll"
			},
			{
				name: "Bezug1",
				description: "ist 1 bis 254 Bereiche oder Bezüge, zu denen Sie ein Teilergebnis berechnen möchten"
			}
		]
	},
	{
		name: "TEXT",
		description: "Formatiert eine Zahl und wandelt sie in einen Text um.",
		arguments: [
			{
				name: "Wert",
				description: "ist ein numerischer Wert, eine Formel, die einen numerischen Wert berechnet, oder ein Bezug auf eine Zelle, die einen numerischen Wert enthält"
			},
			{
				name: "Textformat",
				description: "ist ein Zahlenformat aus dem Dialogfeld Zahlenformat, in Textformat"
			}
		]
	},
	{
		name: "TINV",
		description: "Gibt Perzentile der zweiseitigen t-Verteilung zurück.",
		arguments: [
			{
				name: "Wahrsch",
				description: "ist die zur t-Verteilung gehörige Wahrscheinlichkeit (zweiseitig), eine Zahl von einschließlich 0 bis einschließlich 1"
			},
			{
				name: "Freiheitsgrade",
				description: "ist die Anzahl der Freiheitsgrade"
			}
		]
	},
	{
		name: "TREND",
		description: "Gibt Werte zurück, die sich aus einem linearen Trend ergeben.",
		arguments: [
			{
				name: "Y_Werte",
				description: "sind die y-Werte, die Ihnen bereits aus der Beziehung y = mx + b bekannt sind"
			},
			{
				name: "X_Werte",
				description: "sind optionale x-Werte, die Ihnen eventuell bereits aus der Beziehung y = mx + b bekannt sind"
			},
			{
				name: "Neue_x_Werte",
				description: "sind die neuen x-Werte, für die die Funktion TREND die zugehörigen y-Werte liefern soll"
			},
			{
				name: "Konstante",
				description: "ist ein Wahrheitswert, der angibt, ob die Konstante b den Wert 0 annehmen soll: WAHR = die Konstante b wird normal berechnet; FALSCH oder fehlt = die Konstante b wird auf 0 gesetzt"
			}
		]
	},
	{
		name: "TTEST",
		description: "Gibt die Teststatistik eines Studentschen t-Tests zurück.",
		arguments: [
			{
				name: "Matrix1",
				description: "ist die erste Datengruppe"
			},
			{
				name: "Matrix2",
				description: "ist die zweite Datengruppe"
			},
			{
				name: "Seiten",
				description: "bestimmt die Anzahl der Endflächen"
			},
			{
				name: "Typ",
				description: "bestimmt die Form des durchzuführenden t-Tests"
			}
		]
	},
	{
		name: "TVERT",
		description: "Gibt Werte der Verteilungsfunktion (1-Alpha) einer (Student) t-verteilten Zufallsvariablen zurück.",
		arguments: [
			{
				name: "x",
				description: "ist der Wert der Verteilung (Perzentil), dessen Wahrscheinlichkeit Sie berechnen möchten"
			},
			{
				name: "Freiheitsgrade",
				description: "ist eine ganze Zahl, die die Anzahl der Freiheitsgrade bestimmt"
			},
			{
				name: "Seiten",
				description: "bestimmt die Anzahl der Endflächen (1 oder 2)"
			}
		]
	},
	{
		name: "TYP",
		description: "Gibt eine Zahl zurück, die den Datentyp des angegebenen Wertes anzeigt: Zahl = 1; Text = 2; Logischer Wert = 4; Fehlerwert = 16; Matrix = 64.",
		arguments: [
			{
				name: "Wert",
				description: "kann ein beliebiger Spreadsheet-Wert"
			}
		]
	},
	{
		name: "UMWANDELN",
		description: "Wandelt eine Zahl von einem Maßsystem in ein anderes um.",
		arguments: [
			{
				name: "Zahl",
				description: "ist der Wert in Von_Maßeinheit, der umgewandelt werden soll"
			},
			{
				name: "Von_Maßeinheit",
				description: "ist die Maßeinheit von Zahl"
			},
			{
				name: "In_Maßeinheit",
				description: "ist die Maßeinheit des Ergebnisses"
			}
		]
	},
	{
		name: "UND",
		description: "Prüft, ob alle Argumente WAHR sind; gibt WAHR zurück, wenn alle Argumente WAHR sind.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "Wahrheitswert1",
				description: "sind 1 bis 255 Bedingungen, die Sie überprüfen möchten und die jeweils entweder WAHR oder FALSCH sein können. Übergeben werden können logische Werte, Arrays oder Bezüge"
			},
			{
				name: "Wahrheitswert2",
				description: "sind 1 bis 255 Bedingungen, die Sie überprüfen möchten und die jeweils entweder WAHR oder FALSCH sein können. Übergeben werden können logische Werte, Arrays oder Bezüge"
			}
		]
	},
	{
		name: "UNGERADE",
		description: "Rundet eine positive Zahl auf die nächste ungerade ganze Zahl auf und eine negative Zahl auf die nächste ungerade genaue Zahl ab.",
		arguments: [
			{
				name: "Zahl",
				description: "ist der Wert, den Sie runden möchten"
			}
		]
	},
	{
		name: "UNICODE",
		description: "Gibt die Zahl (Codepoint) zurück, die dem ersten Zeichen des Texts entspricht.",
		arguments: [
			{
				name: "Text",
				description: "ist das Zeichen, dessen Unicode-Wert Sie bestimmen möchten"
			}
		]
	},
	{
		name: "UNTERGRENZE",
		description: "Rundet eine Zahl auf das nächstliegende Vielfache von Schritt ab.",
		arguments: [
			{
				name: "Zahl",
				description: "ist der Wert, den Sie runden möchten"
			},
			{
				name: "Schritt",
				description: "ist das Vielfache, auf das Sie runden möchten. Die zu rundende Zahl und das Vielfachen müssen beide entweder positiv oder negativ sein"
			}
		]
	},
	{
		name: "UNTERGRENZE.GENAU",
		description: "Rundet eine Zahl auf die nächste Ganzzahl oder auf das nächstliegende Vielfache von Schritt ab.",
		arguments: [
			{
				name: "Zahl",
				description: "ist der Wert, den Sie runden möchten"
			},
			{
				name: "Schritt",
				description: "ist das Vielfache, auf das Sie runden möchten. "
			}
		]
	},
	{
		name: "UNTERGRENZE.MATHEMATIK",
		description: "Rundet eine Zahl betragsmäßig auf das kleinste Vielfache von Schritt ab.",
		arguments: [
			{
				name: "Zahl",
				description: "ist der Wert, den Sie runden möchten"
			},
			{
				name: "Schritt",
				description: "ist der Wert, auf dessen Vielfaches Sie runden möchten"
			},
			{
				name: "Modus",
				description: "sofern angegeben und ungleich Null rundet diese Funktion Richtung Null"
			}
		]
	},
	{
		name: "URLCODIEREN",
		description: "Gibt eine URL-codierte Zeichenfolge zurück.",
		arguments: [
			{
				name: "text",
				description: "ist eine als URL zu codierende Zeichenfolge"
			}
		]
	},
	{
		name: "VAR.P",
		description: "Berechnet die Varianz, ausgehend von der Grundgesamtheit. Logische Werte und Text werden ignoriert.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "Zahl1",
				description: "sind 1 bis 255 numerische Argumente, die einer Grundgesamtheit entsprechen"
			},
			{
				name: "Zahl2",
				description: "sind 1 bis 255 numerische Argumente, die einer Grundgesamtheit entsprechen"
			}
		]
	},
	{
		name: "VAR.S",
		description: "Schätzt die Varianz, ausgehend von einer Stichprobe (logische Werte und Text werden in der Stichprobe ignoriert).",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "Zahl1",
				description: "sind 1 bis 255 numerische Argumente, die eine, aus einer Grundgesamtheit gezogene Stichprobe darstellen"
			},
			{
				name: "Zahl2",
				description: "sind 1 bis 255 numerische Argumente, die eine, aus einer Grundgesamtheit gezogene Stichprobe darstellen"
			}
		]
	},
	{
		name: "VARIANZ",
		description: "Schätzt die Varianz, ausgehend von einer Stichprobe (logische Werte und Text werden in der Stichprobe ignoriert).",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "Zahl1",
				description: "sind 1 bis 255 numerische Argumente, die eine, aus einer Grundgesamtheit gezogene Stichprobe darstellen"
			},
			{
				name: "Zahl2",
				description: "sind 1 bis 255 numerische Argumente, die eine, aus einer Grundgesamtheit gezogene Stichprobe darstellen"
			}
		]
	},
	{
		name: "VARIANZA",
		description: "Schätzt die Varianz, ausgehend von einer Stichprobe, einschließlich logischer Werte und Text. Dabei werden Text und FALSCH als 0 interpretiert, WAHR wird als 1 interpretiert.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "Wert1",
				description: "sind 1 bis 255 Werte, die zu einer Stichprobe einer Grundgesamtheit gehören"
			},
			{
				name: "Wert2",
				description: "sind 1 bis 255 Werte, die zu einer Stichprobe einer Grundgesamtheit gehören"
			}
		]
	},
	{
		name: "VARIANZEN",
		description: "Berechnet die Varianz, ausgehend von der Grundgesamtheit. Logische Werte und Text werden ignoriert.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "Zahl1",
				description: "sind 1 bis 255 numerische Argumente, die einer Grundgesamtheit entsprechen"
			},
			{
				name: "Zahl2",
				description: "sind 1 bis 255 numerische Argumente, die einer Grundgesamtheit entsprechen"
			}
		]
	},
	{
		name: "VARIANZENA",
		description: "Berechnet die Varianz, ausgehend von der Grundgesamtheit, einschließlich logischer Werte und Text. Dabei werden Text und FALSCH als 0 interpretiert, WAHR wird als 1 interpretiert.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "Wert1",
				description: "sind 1 bis 255 Argumente, die zu einer Grundgesamtheit gehören"
			},
			{
				name: "Wert2",
				description: "sind 1 bis 255 Argumente, die zu einer Grundgesamtheit gehören"
			}
		]
	},
	{
		name: "VARIATION",
		description: "Gibt Werte zurück, die sich aus einem exponentiellen Trend ergeben.",
		arguments: [
			{
				name: "Y_Werte",
				description: "sind die y-Werte, die Ihnen bereits aus der Beziehung y = b*m^x bekannt sind"
			},
			{
				name: "X_Werte",
				description: "sind optionale x-Werte, die Ihnen eventuell bereits aus der Beziehung y = b*m^x bekannt sind"
			},
			{
				name: "Neue_x_Werte",
				description: "sind die neuen x-Werte, für die die Funktion VARIATION die zugehörigen y-Werte zurückgeben soll"
			},
			{
				name: "Konstante",
				description: "ist ein Wahrheitswert, der angibt, ob die Konstante b den Wert 0 annehmen soll"
			}
		]
	},
	{
		name: "VARIATIONEN",
		description: "Gibt die Anzahl der Möglichkeiten zurück, um k Elemente aus einer Menge von n Elementen ohne Zurücklegen zu ziehen.",
		arguments: [
			{
				name: "n",
				description: "ist die Anzahl aller Elemente"
			},
			{
				name: "k",
				description: "gibt an, aus wie vielen Elementen jede Variationsmöglichkeit bestehen soll"
			}
		]
	},
	{
		name: "VARIATIONEN2",
		description: "Gibt die Anzahl der Permutationen für eine angegebene Anzahl von Objekten zurück (mit Wiederholungen), die aus der Gesamtmenge der Objekte ausgewählt werden können.",
		arguments: [
			{
				name: "Zahl",
				description: "ist die Gesamtzahl der Objekte"
			},
			{
				name: "gewählte_Zahl",
				description: "ist die Anzahl der Objekte in jeder Permutation"
			}
		]
	},
	{
		name: "VDB",
		description: "Gibt die degressive Doppelraten-Abschreibung eines Wirtschaftsguts für eine bestimmte Periode oder Teilperiode zurück.",
		arguments: [
			{
				name: "Ansch_Wert",
				description: "sind die Anschaffungskosten eines Wirtschaftsguts"
			},
			{
				name: "Restwert",
				description: "ist der Wert am Ende des Abschreibungszeitraums (auch Restwert des Anlageobjekts genannt)"
			},
			{
				name: "Nutzungsdauer",
				description: "ist die Anzahl der Perioden, über die das Wirtschaftsgut abgeschrieben wird (auch als Nutzungsdauer bezeichnet)"
			},
			{
				name: "Anfang",
				description: "ist der Anfangszeitraum, für den Sie die Abschreibung berechnen wollen"
			},
			{
				name: "Ende",
				description: "ist der Endzeitraum, für den Sie die Abschreibung berechnen wollen"
			},
			{
				name: "Faktor",
				description: "ist das Maß, in welchem die Abschreibung abnimmt"
			},
			{
				name: "Nicht_wechseln",
				description: "ist ein Wahrheitswert, der angibt, ob zur linearen Abschreibung gewechselt werden soll, wenn diese höhere jährliche Abschreibungen zurückgibt"
			}
		]
	},
	{
		name: "VERGLEICH",
		description: "Sucht Werte innerhalb eines Bezuges oder einer Matrix.",
		arguments: [
			{
				name: "Suchkriterium",
				description: "ist der Wert, aufgrund dessen der gewünschte Wert in einer Tabelle gesucht wird"
			},
			{
				name: "Suchmatrix",
				description: "ist ein zusammenhängender Zellbereich mit möglichen Vergleichskriterien"
			},
			{
				name: "Vergleichstyp",
				description: "ist die Zahl 1, 0 oder -1, die angibt, welcher Wert zurückgegeben werden soll."
			}
		]
	},
	{
		name: "VERKETTEN",
		description: "Verknüpft mehrere Zeichenfolgen zu einer Zeichenfolge.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "Text1",
				description: "sind 1 bis 255 Zeichenfolgen, die Sie zu einer Zeichenfolge verketten möchten und können Zeichenfolgen, Zahlen oder Bezüge einer Zelle sein"
			},
			{
				name: "Text2",
				description: "sind 1 bis 255 Zeichenfolgen, die Sie zu einer Zeichenfolge verketten möchten und können Zeichenfolgen, Zahlen oder Bezüge einer Zelle sein"
			}
		]
	},
	{
		name: "VERWEIS",
		description: "Durchsucht die Werte eines Vektors oder einer Matrix.",
		arguments: [
			{
				name: "Suchkriterium",
				description: "ist ein Wert, nach dem VERWEIS im ersten Vektor sucht. Wird für Abwärtskompatibilität unterstützt."
			},
			{
				name: "Suchvektor",
				description: "ist ein Bereich, der nur eine Zeile oder Spalte enthält"
			},
			{
				name: "Ergebnisvektor",
				description: "ist ein Bereich, der nur eine Zeile oder Spalte enthält"
			}
		]
	},
	{
		name: "VORZEICHEN",
		description: "Gibt das Vorzeichen einer Zahl zurück: 1 wenn die Zahl positiv ist, 0 wenn die Zahl 0 ist, -1 wenn die Zahl negativ ist.",
		arguments: [
			{
				name: "Zahl",
				description: "ist eine beliebige reelle Zahl"
			}
		]
	},
	{
		name: "VRUNDEN",
		description: "Gibt eine auf das gewünschte Vielfache gerundete Zahl.",
		arguments: [
			{
				name: "Zahl",
				description: "ist die zu rundende Zahl zurück"
			},
			{
				name: "Vielfaches",
				description: "ist der Wert auf dessen Vielfaches Sie Zahl aufrunden möchten"
			}
		]
	},
	{
		name: "WAHL",
		description: "Wählt einen Wert aus einer Liste von Werten.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "Index",
				description: "gibt an, welcher Argumentwert gewählt ist. Der Wert muss zwischen 1 und 254 liegen, oder eine Formel oder ein Bezug zu einem Wert von 1 bis 254 sein"
			},
			{
				name: "Wert1",
				description: "sind 1 bis 254 Argumente, aus denen WAHL je nach angegebenem Index einen Wert oder eine Anweisung wählt"
			},
			{
				name: "Wert2",
				description: "sind 1 bis 254 Argumente, aus denen WAHL je nach angegebenem Index einen Wert oder eine Anweisung wählt"
			}
		]
	},
	{
		name: "WAHR",
		description: "Gibt den Wahrheitswert WAHR zurück.",
		arguments: [
		]
	},
	{
		name: "WAHRSCHBEREICH",
		description: "Gibt die Wahrscheinlichkeit für ein von zwei Werten eingeschlossenes Intervall zurück.",
		arguments: [
			{
				name: "Beob_Werte",
				description: "ist ein Bereich von Realisationen der Zufallsvariablen, denen Wahrscheinlichkeiten zugeordnet sind"
			},
			{
				name: "Beob_Wahrsch",
				description: "sind die Wahrscheinlichkeiten zu den beobachteten Werten, eine Zahl gleich oder größer als 0 und kleiner als 1"
			},
			{
				name: "Untergrenze",
				description: "ist die untere Grenze der Werte, deren Wahrscheinlichkeit berechnet werden soll"
			},
			{
				name: "Obergrenze",
				description: "ist die optionale obere Grenze der Werte, deren Wahrscheinlichkeit berechnet werden soll"
			}
		]
	},
	{
		name: "WECHSELN",
		description: "Tauscht einen alten Text durch einen neuen Text in einer Zeichenfolge aus.",
		arguments: [
			{
				name: "Text",
				description: "ist der in Anführungszeichen gesetzte Text oder der Bezug auf eine Zelle, die den Text enthält, in dem Zeichen ausgetauscht werden sollen"
			},
			{
				name: "Alter_Text",
				description: "ist der Text, den Sie ersetzen wollen.  Wenn die Groß-/Kleinschreibung nicht übereinstimmt, ersetzt die Funktion den Text nicht"
			},
			{
				name: "Neuer_Text",
				description: "ist der Text, mit dem Sie Alter_Text ersetzen wollen"
			},
			{
				name: "ntes_Auftreten",
				description: "gibt an, wo Alter_Text durch Neuer_Text ersetzt werden soll"
			}
		]
	},
	{
		name: "WEIBULL",
		description: "Gibt Wahrscheinlichkeiten einer Weibull-verteilten Zufallsvariablen zurück.",
		arguments: [
			{
				name: "x",
				description: "ist der Wert der Verteilung (Perzentil), dessen Wahrscheinlichkeit Sie berechnen möchten, eine nicht-negative Zahl"
			},
			{
				name: "Alpha",
				description: "ist ein Parameter der Verteilung, eine positive Zahl"
			},
			{
				name: "Beta",
				description: "ist ein Parameter der Verteilung, eine positive Zahl"
			},
			{
				name: "Kumuliert",
				description: "ist der Wahrheitswert, der den Typ der Funktion bestimmt"
			}
		]
	},
	{
		name: "WEIBULL.VERT",
		description: "Gibt Wahrscheinlichkeiten einer Weibull-verteilten Zufallsvariablen zurück.",
		arguments: [
			{
				name: "x",
				description: "ist der Wert der Verteilung (Perzentil), dessen Wahrscheinlichkeit Sie berechnen möchten, eine nicht-negative Zahl"
			},
			{
				name: "Alpha",
				description: "ist ein Parameter der Verteilung, eine positive Zahl"
			},
			{
				name: "Beta",
				description: "ist ein Parameter der Verteilung, eine positive Zahl"
			},
			{
				name: "Kumuliert",
				description: "ist der Wahrheitswert, der den Typ der Funktion bestimmt"
			}
		]
	},
	{
		name: "WENN",
		description: "Gibt eine Wahrheitsprüfung an, die durchgeführt werden soll.",
		arguments: [
			{
				name: "Prüfung",
				description: "ist ein beliebiger Wert oder Ausdruck, der WAHR oder FALSCH sein kann"
			},
			{
				name: "Dann_Wert",
				description: "ist das Resultat der Funktion, wenn die Wahrheitsprüfung WAHR ergibt. Wenn der Parameter nicht angegeben wird, wird WAHR zurückgegeben."
			},
			{
				name: "Sonst_Wert",
				description: "ist das Resultat der Funktion, wenn die Wahrheitsprüfung FALSCH ergibt Wenn der Parameter nicht angegeben wird, wird FALSCH zurückgegeben"
			}
		]
	},
	{
		name: "WENNFEHLER",
		description: "Gibt einen Wert_falls_Fehler aus, falls es sich bei dem Ausdruck um einen Fehler handelt, und anderenfalls den Wert des Ausdrucks selbst.",
		arguments: [
			{
				name: "Wert",
				description: "ist jeder Wert oder Audruck oder Bezug"
			},
			{
				name: "Wert_falls_Fehler",
				description: "ist jeder Wert oder Ausdruck oder Bezug"
			}
		]
	},
	{
		name: "WENNNV",
		description: "Gibt den Wert zurück, den Sie angeben, wenn der Ausdruck zu '#N/V' ausgewertet wird, gibt andernfalls das Ergebnis des Ausdrucks zurück.",
		arguments: [
			{
				name: "Wert",
				description: "ist ein beliebiger Wert, Ausdruck oder Bezug"
			},
			{
				name: "Wert_bei_NV",
				description: "ist ein beliebiger Wert, Ausdruck oder Bezug"
			}
		]
	},
	{
		name: "WERT",
		description: "Wandelt ein als Text angegebenes Argument in eine Zahl um.",
		arguments: [
			{
				name: "Text",
				description: "ist der in Anführungszeichen gesetzte Text oder der Bezug auf eine Zelle, die den Text enthält, der umgewandelt werden soll"
			}
		]
	},
	{
		name: "WIEDERHOLEN",
		description: "Wiederholt einen Text so oft wie angegeben.",
		arguments: [
			{
				name: "Text",
				description: "ist der Text, den Sie wiederholen wollen"
			},
			{
				name: "Multiplikator",
				description: "ist eine positive Zahl, die die Anzahl der Wiederholungen von Text festlegt"
			}
		]
	},
	{
		name: "WOCHENTAG",
		description: "Wandelt eine fortlaufende Zahl in einen Wochentag um. .",
		arguments: [
			{
				name: "Zahl",
				description: "ist der Code für Datum und Zeit, den Spreadsheet für Datums- und Zeitberechnungen verwendet"
			},
			{
				name: "Typ",
				description: "legt den Rückgabewert-Typ fest: 1 für Sonntag = 1 bis Samstag = 7;  2 für Montag = 1 bis  Sonntag =7;  3 für Montag = 0 bis Sonntag= 6"
			}
		]
	},
	{
		name: "WURZEL",
		description: "Gibt die Quadratwurzel einer Zahl.",
		arguments: [
			{
				name: "Zahl",
				description: "ist die Zahl, deren Quadratwurzel Sie berechnen möchten zurück"
			}
		]
	},
	{
		name: "WURZELPI",
		description: "Gibt die Wurzel aus der mit Pi multiplizierten Zahl zurück.",
		arguments: [
			{
				name: "Zahl",
				description: "ist die Zahl, die mit Pi multipliziert wird"
			}
		]
	},
	{
		name: "WVERWEIS",
		description: "Durchsucht die erste Zeile einer Matrix und durchläuft die Spalte nach unten, um den Wert einer Zelle zurückzugeben.",
		arguments: [
			{
				name: "Suchkriterium",
				description: "ist der Wert, nach dem Sie in der ersten Zeile der Matrix suchen. Suchkriterium kann ein Wert, ein Bezug oder eine Zeichenfolge in Anführungszeichen sein"
			},
			{
				name: "Matrix",
				description: "ist eine Informationstabelle, in der Daten gesucht werden"
			},
			{
				name: "Zeilenindex",
				description: "ist die Nummer der Zeile in der Mehrfachoperationsmatrix, aus der der übereinstimmende Wert zurückgegeben werden soll"
			},
			{
				name: "Bereich_Verweis",
				description: "gibt an, ob eine genaue Übereinstimmung gefunden werden soll: WAHR = aus der aufsteigend sortierten Reihenfolge der Werte wird der Wert zurückgegeben, der am dichtesten am gesuchten Wert liegt; FALSCH = es wird eine genaue Übereinstimmung gesucht"
			}
		]
	},
	{
		name: "XINTZINSFUSS",
		description: "Gibt den internen Zinsfuß einer Reihe nicht periodisch anfallender Zahlungen zurück.",
		arguments: [
			{
				name: "Werte",
				description: "ist eine Reihe von nicht periodisch anfallenden Zahlungen, die sich auf die Zeitpunkte des Zahlungsplans beziehen"
			},
			{
				name: "Zeitpkte",
				description: "sind die Zeitpunkte im Zahlungsplan der nicht periodisch anfallenden Zahlungen"
			},
			{
				name: "Schätzwert",
				description: "ist eine Zahl, von der Sie annehmen, dass sie dem Ergebnis der Funktion nahekommt"
			}
		]
	},
	{
		name: "XKAPITALWERT",
		description: "Gibt den Nettobarwert (Kapitalwert) einer Reihe nicht periodisch anfallender Zahlungen zurück.",
		arguments: [
			{
				name: "Zins",
				description: "ist der Kalkulationszinsfuß, der für die Zahlungen zu berücksichtigen ist"
			},
			{
				name: "Werte",
				description: "ist eine Reihe von nicht periodisch anfallenden Zahlungen, die sich auf die Zeitpunkte des Zahlungsplans beziehen"
			},
			{
				name: "Zeitpkte",
				description: "sind die Zeitpunkte im Zahlungsplan der nicht periodisch anfallenden Zahlungen"
			}
		]
	},
	{
		name: "XODER",
		description: "Gibt ein logisches 'Ausschließliches Oder' aller Argumente zurück.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "Wahrheitswert1",
				description: "sind 1 bis 254 zu prüfende Bedingungen, die entweder WAHR oder FALSCH sein können und logische Werte, Matrizen oder Bezüge darstellen können"
			},
			{
				name: "Wahrheitswert2",
				description: "sind 1 bis 254 zu prüfende Bedingungen, die entweder WAHR oder FALSCH sein können und logische Werte, Matrizen oder Bezüge darstellen können"
			}
		]
	},
	{
		name: "ZÄHLENWENN",
		description: "Zählt die nichtleeren Zellen eines Bereichs, deren Inhalte mit den Suchkriterien übereinstimmen.",
		arguments: [
			{
				name: "Bereich",
				description: "ist der Bereich, von dem Sie anfangen wollen, die nichtleeren Zellen zu zählen"
			},
			{
				name: "Suchkriterien",
				description: "ist das Suchkriterium als Zahl, Formel oder Text, das festlegt, welche Zellen gezählt werden"
			}
		]
	},
	{
		name: "ZÄHLENWENNS",
		description: "Zählt die Anzahl der Zellen, die durch eine bestimmte Menge von Bedingungen oder Kriterien festgelegt ist.",
		arguments: [
			{
				name: "Kriterienbereich",
				description: "ist der Zellenbereich, der für eine bestimmte Bedingung ausgewertet werden soll"
			},
			{
				name: "Kriterien",
				description: "ist die Bedingung in Form einer Zahl, eines Ausdrucks oder Texts, der definiert, welche Zellen gezählt werden sollen"
			}
		]
	},
	{
		name: "ZAHLENWERT",
		description: "Konvertiert auf vom Gebietsschema unabhängige Weise Text in Zahlen.",
		arguments: [
			{
				name: "Text",
				description: "ist die Zeichenfolge, die die zu konvertierende Zahl darstellt"
			},
			{
				name: "Dezimaltrennzeichen",
				description: "ist das als Dezimaltrennzeichen in der Zeichenfolge verwendete Zeichen"
			},
			{
				name: "Gruppentrennzeichen",
				description: "ist das als Gruppentrennzeichen in der Zeichenfolge verwendete Zeichen"
			}
		]
	},
	{
		name: "ZEICHEN",
		description: "Gibt das der Codezahl entsprechende Zeichen zurück.",
		arguments: [
			{
				name: "Zahl",
				description: "ist eine Zahl zwischen 1 und 255, die das gewünschte Zeichen entspricht"
			}
		]
	},
	{
		name: "ZEILE",
		description: "Gibt die Zeilennummer eines Bezuges zurück.",
		arguments: [
			{
				name: "Bezug",
				description: "ist die Zelle oder der Zellbereich, deren bzw. dessen Zeilennummer Sie ermitteln möchten. Wenn der Parameter fehlt, wird die Zelle zurückgegeben, die die Funktion enthält"
			}
		]
	},
	{
		name: "ZEILEN",
		description: "Gibt die Anzahl der Zeilen eines Bezuges zurück.",
		arguments: [
			{
				name: "Matrix",
				description: "ist eine Matrix, eine Matrixformel oder ein Bezug auf einen Zellbereich, dessen Zeilenanzahl Sie abfragen möchten"
			}
		]
	},
	{
		name: "ZEIT",
		description: "Gibt die fortlaufende Zahl einer bestimmten Uhrzeit zurück.",
		arguments: [
			{
				name: "Stunde",
				description: "ist eine Zahl von 0 bis 23, die für die Stunde steht"
			},
			{
				name: "Minute",
				description: "ist eine Zahl von 0 bis 59, die für die Minute steht"
			},
			{
				name: "Sekunde",
				description: "ist eine Zahl von 0 bis 59, die für die Sekunde steht"
			}
		]
	},
	{
		name: "ZEITWERT",
		description: "Wandelt eine als Text vorliegende Zeitangabe in eine fortlaufende Zahl von 0 (00:00:00) bis 0.999988426 (23:59:59) um. Formatieren Sie die Zahl nach Eingabe der Formel mit einem Zeitformat.",
		arguments: [
			{
				name: "Zeit",
				description: "ist die Zeichenfolge einer Zeitangabe in einem gültigen Spreadsheet-Zeitformat (der Datumsteil der Zeichenfolge wird ignoriert.)"
			}
		]
	},
	{
		name: "ZELLE",
		description: "Gibt Informationen zu der Formatierung, der Position oder dem Inhalt der ersten Zelle gemäß der Lesereihenfolge für das Blatt zurück.",
		arguments: [
			{
				name: "Infotyp",
				description: "ist ein Textwert, mit dem Sie die gewünschte Art von Zelleninformationen festlegen."
			},
			{
				name: "Bezug",
				description: "ist die Zelle, zu der Sie Informationen abrufen möchten"
			}
		]
	},
	{
		name: "ZINS",
		description: "Gibt den Zinssatz einer Annuität pro Periode zurück. Z.B. verwenden Sie 6%/4 für Quartalszahlungen von 6%.",
		arguments: [
			{
				name: "Zzr",
				description: "ist die Anzahl der Zahlungsperioden"
			},
			{
				name: "Rmz",
				description: "ist der Betrag (Annuität), der in jeder Periode gezahlt wird. Dieser Betrag bleibt während der Laufzeit konstant."
			},
			{
				name: "Bw",
				description: "ist der Barwert: der Gesamtbetrag, den eine Reihe zukünftiger Zahlungen zum gegenwärtigen Zeitpunkt wert ist"
			},
			{
				name: "Zw",
				description: "ist der zukünftige Wert (Endwert) oder der Kassenbestand, den Sie nach der letzten Zahlung erreicht haben möchten"
			},
			{
				name: "F",
				description: "kann den Wert 0 oder 1 annehmen und gibt an, wann Zahlungen fällig sind (Fälligkeit): 1 = Zahlung an Beginn der Periode, 0 = Zahlung am Ende der Periode"
			},
			{
				name: "Schätzwert",
				description: "entspricht Ihrer Schätzung, wie hoch der Zinssatz sein wird. Wenn der Parameter fehlt, wird 0,1 (10 Prozent) angenommen"
			}
		]
	},
	{
		name: "ZINSSATZ",
		description: "Gibt den Zinssatz eines voll investierten Wertpapiers zurück.",
		arguments: [
			{
				name: "Abrechnung",
				description: "ist der Abrechnungstermin des Wertpapierkaufs, als fortlaufende Zahl angegeben"
			},
			{
				name: "Fälligkeit",
				description: "ist der Fälligkeitstermin des Wertpapiers, als fortlaufende Zahl angegeben"
			},
			{
				name: "Anlage",
				description: "ist der Betrag, der in dem Wertpapier angelegt werden soll"
			},
			{
				name: "Rückzahlung",
				description: "ist der Betrag, der bei Fälligkeit zu erwarten ist"
			},
			{
				name: "Basis",
				description: "gibt an, auf welcher Basis die Zinstage gezählt werden"
			}
		]
	},
	{
		name: "ZINSTERMNZ",
		description: "Gibt das Datum des ersten Zinstermins nach dem Abrechnungstermin zurück.",
		arguments: [
			{
				name: "Abrechnung",
				description: "ist der Abrechnungstermin des Wertpapierkaufs, als fortlaufende Zahl angegeben"
			},
			{
				name: "Fälligkeit",
				description: "ist der Fälligkeitstermin des Wertpapiers, als fortlaufende Zahl angegeben"
			},
			{
				name: "Häufigkeit",
				description: "ist die Anzahl der Zinszahlungen pro Jahr"
			},
			{
				name: "Basis",
				description: "gibt an, auf welcher Basis die Zinstage gezählt werden"
			}
		]
	},
	{
		name: "ZINSTERMTAGVA",
		description: "Gibt die Anzahl der Tage vom Anfang des Zinstermins bis zum Abrechnungstermin zurück.",
		arguments: [
			{
				name: "Abrechnung",
				description: "ist der Abrechnungstermin des Wertpapierkaufs, als fortlaufende Zahl angegeben"
			},
			{
				name: "Fälligkeit",
				description: "ist der Fälligkeitstermin des Wertpapiers, als fortlaufende Zahl angegeben"
			},
			{
				name: "Häufigkeit",
				description: "ist die Anzahl der Zinszahlungen pro Jahr"
			},
			{
				name: "Basis",
				description: "gibt an, auf welcher Basis die Zinstage gezählt werden"
			}
		]
	},
	{
		name: "ZINSTERMVZ",
		description: "Gibt das Datum des letzten Zinstermins vor dem Abrechnungstermin zurück.",
		arguments: [
			{
				name: "Abrechnung",
				description: "ist der Abrechnungstermin des Wertpapierkaufs, als fortlaufende Zahl angegeben"
			},
			{
				name: "Fälligkeit",
				description: "ist der Fälligkeitstermin des Wertpapiers, als fortlaufende Zahl angegeben"
			},
			{
				name: "Häufigkeit",
				description: "ist die Anzahl der Zinszahlungen pro Jahr"
			},
			{
				name: "Basis",
				description: "gibt an, auf welcher Basis die Zinstage gezählt werden"
			}
		]
	},
	{
		name: "ZINSTERMZAHL",
		description: "Gibt die Anzahl der Zinstermine zwischen Abrechnungs- und Fälligkeitdatum zurück.",
		arguments: [
			{
				name: "Abrechnung",
				description: "ist der Abrechnungstermin des Wertpapierkaufs, als fortlaufende Zahl angegeben"
			},
			{
				name: "Fälligkeit",
				description: "ist der Fälligkeitstermin des Wertpapiers, als fortlaufende Zahl angegeben"
			},
			{
				name: "Häufigkeit",
				description: "ist die Anzahl der Zinszahlungen pro Jahr"
			},
			{
				name: "Basis",
				description: "gibt an, auf welcher Basis die Zinstage gezählt werden"
			}
		]
	},
	{
		name: "ZINSZ",
		description: "Gibt die Zinszahlung einer Investition für die angegebene Periode.",
		arguments: [
			{
				name: "Zins",
				description: "ist der Zinssatz pro Periode (Zahlungszeitraum) Z.B. verwenden Sie 6%/4 für Quartalszahlungen von 6%"
			},
			{
				name: "Zr",
				description: "ist die Periode, für die Sie den Zinsbetrag berechnen möchten"
			},
			{
				name: "Zzr",
				description: "gibt an, über wie viele Perioden die jeweilige Annuität (Rente) gezahlt wird"
			},
			{
				name: "Bw",
				description: "ist der Barwert oder der heutige Gesamtwert einer Reihe zukünftiger Zahlungen"
			},
			{
				name: "Zw",
				description: "ist der zukünftige Wert (Endwert) oder der Kassenbestand, den Sie nach der letzten Zahlung erreicht haben möchten"
			},
			{
				name: "F",
				description: "kann den Wert 0 oder 1 annehmen und gibt an, wann Zahlungen fällig sind (Fälligkeit): 1 = Zahlung an Beginn der Periode, 0 = Zahlung am Ende der Periode"
			}
		]
	},
	{
		name: "ZSATZINVEST",
		description: "Gibt den effektiven Jahreszins für den Wertzuwachs einer Investition zurück.",
		arguments: [
			{
				name: "Zzr",
				description: "ist die Anzahl der Perioden für die Investition"
			},
			{
				name: "Bw",
				description: "ist der aktuelle Wert der Investition"
			},
			{
				name: "Zw",
				description: "ist der zukünftige Wert der Investition"
			}
		]
	},
	{
		name: "ZUFALLSBEREICH",
		description: "Gibt eine ganze Zufallszahl aus dem festgelegten Bereich zurück.",
		arguments: [
			{
				name: "Untere_Zahl",
				description: "ist die kleinste ganze Zahl, die ZUFALLSBEREICH zurückgebenn kann"
			},
			{
				name: "Obere_Zahl",
				description: "ist die größte ganze Zahl, die ZUFALLSBEREICH zurückgeben kann"
			}
		]
	},
	{
		name: "ZUFALLSZAHL",
		description: "Gibt eine Zufallszahl gleichmässig zwischen 0 und 1 verteilt zurück. Das Ergebnis ändert sich bei jeder Neuberechnung.",
		arguments: [
		]
	},
	{
		name: "ZW",
		description: "Gibt den zukünftigen Wert (Endwert) einer Investition zurück.",
		arguments: [
			{
				name: "Zins",
				description: "ist der Zinssatz pro Periode (Zahlungszeitraum) Z.B. verwenden Sie 6%/4 für Quartalszahlungen von 6%."
			},
			{
				name: "Zzr",
				description: "gibt an, über wie viele Perioden die jeweilige Annuität (Rente) gezahlt wird"
			},
			{
				name: "Rmz",
				description: "ist der Betrag (Annuität), der in jeder Periode gezahlt wird. Dieser Betrag bleibt während der Laufzeit konstant"
			},
			{
				name: "Bw",
				description: "ist der Barwert oder der heutige Gesamtwert einer Reihe zukünftiger Zahlungen"
			},
			{
				name: "F",
				description: "kann den Wert 0 oder 1 annehmen und gibt an, wann Zahlungen fällig sind (Fälligkeit): 1 = Zahlung an Beginn der Periode, 0 = Zahlung am Ende der Periode"
			}
		]
	},
	{
		name: "ZW2",
		description: "Gibt den aufgezinsten Wert des Anfangskapitals für eine Reihe periodisch unterschiedlicher Zinssätze zurück.",
		arguments: [
			{
				name: "Kapital",
				description: "ist der Gegenwartswert"
			},
			{
				name: "Zinsen",
				description: "ist eine Reihe von Zinssätzen, als Matrix eingegeben"
			}
		]
	},
	{
		name: "ZWEIFAKULTÄT",
		description: "Gibt die Fakultät zu Zahl mit Schrittlänge 2 zurück.",
		arguments: [
			{
				name: "Zahl",
				description: "ist der Wert, für den die Fakultät mit Schrittlänge 2 berechnet werden soll"
			}
		]
	},
	{
		name: "ZZR",
		description: "Gibt die Anzahl der Zahlungsperioden einer Investition zurück.",
		arguments: [
			{
				name: "Zins",
				description: "ist der Zinssatz pro Periode (Zahlungszeitraum) Z.B. verwenden Sie 6%/4 für Quartalszahlungen von 6%"
			},
			{
				name: "Rmz",
				description: "ist der Betrag (Annuität), der in jeder Periode gezahlt wird"
			},
			{
				name: "Bw",
				description: "ist der Barwert oder der heutige Gesamtwert einer Reihe zukünftiger Zahlungen"
			},
			{
				name: "Zw",
				description: "ist der zukünftige Wert (Endwert) oder der Kassenbestand, den Sie nach der letzten Zahlung erreicht haben möchten"
			},
			{
				name: "F",
				description: "kann den Wert 0 oder 1 annehmen und gibt an, wann Zahlungen fällig sind (Fälligkeit): 1 = Zahlung an Beginn der Periode, 0 = Zahlung am Ende der Periode"
			}
		]
	}
];