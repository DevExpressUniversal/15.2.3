ASPxClientSpreadsheet.Functions = [
	{
		name: "AANTAL",
		description: "Telt het aantal cellen in een bereik dat getallen bevat.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "waarde1",
				description: "zijn maximaal 255 argumenten die verschillende typen gegevens kunnen bevatten of ernaar verwijzen. Alleen de getallen worden geteld"
			},
			{
				name: "waarde2",
				description: "zijn maximaal 255 argumenten die verschillende typen gegevens kunnen bevatten of ernaar verwijzen. Alleen de getallen worden geteld"
			}
		]
	},
	{
		name: "AANTAL.ALS",
		description: "Telt het aantal niet-lege cellen in een bereik die voldoen aan het opgegeven criterium.",
		arguments: [
			{
				name: "bereik",
				description: "is het celbereik waarin u wilt tellen hoeveel niet-lege cellen voldoen aan het criterium"
			},
			{
				name: "criterium",
				description: "is de voorwaarde die bepaalt welke cellen worden geteld. Dit kan een getal, een expressie of tekst zijn"
			}
		]
	},
	{
		name: "AANTAL.LEGE.CELLEN",
		description: "Telt het aantal lege cellen in een bereik.",
		arguments: [
			{
				name: "bereik",
				description: "is het bereik waarin u de lege cellen wilt tellen"
			}
		]
	},
	{
		name: "AANTALARG",
		description: "Telt het aantal niet-lege cellen in een bereik.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "waarde1",
				description: "zijn maximaal 255 argumenten met de waarden en cellen die u wilt tellen. Waarden kunnen van elk type informatie zijn"
			},
			{
				name: "waarde2",
				description: "zijn maximaal 255 argumenten met de waarden en cellen die u wilt tellen. Waarden kunnen van elk type informatie zijn"
			}
		]
	},
	{
		name: "AANTALLEN.ALS",
		description: "Telt het aantal cellen dat wordt gespecificeerd door een gegeven set voorwaarden of criteria.",
		arguments: [
			{
				name: "criteriumbereik",
				description: "is het celbereik dat u wilt evalueren voor de voorwaarde in kwestie"
			},
			{
				name: "criteria",
				description: "is de voorwaarde in de vorm van een getal, expressie of tekst waarmee wordt aangegeven welke cellen worden geteld"
			}
		]
	},
	{
		name: "ABS",
		description: "Geeft als resultaat de absolute waarde van een getal. Dit is het getal zonder het teken.",
		arguments: [
			{
				name: "getal",
				description: "is het reële getal waarvan u de absolute waarde wilt weten"
			}
		]
	},
	{
		name: "ADRES",
		description: "Geeft als resultaat een celverwijzing, in de vorm van tekst, gegeven bepaalde rij- en kolomnummers.",
		arguments: [
			{
				name: "rij_getal",
				description: "is het rijnummer voor de celverwijzing. Rij_nummer = 1 voor rij 1"
			},
			{
				name: "kolom_getal",
				description: "is het kolomnummer voor de celverwijzing. Bijvoorbeeld, Kolom_nummer = 4 voor kolom D"
			},
			{
				name: "abs_getal",
				description: "geeft aan welk type verwijzing als resultaat moet worden gegeven: absoluut = 1, absolute rij/relatieve kolom = 2, relatieve rij/absolute kolom = 3, relatief = 4"
			},
			{
				name: "A1",
				description: "is een logische waarde die aangeeft welk verwijzingstype wordt gebruikt: A1 = 1 of WAAR, R1K1 = 0 of ONWAAR"
			},
			{
				name: "blad_tekst",
				description: "is de tekst die de naam aangeeft van het werkblad of macroblad dat moet worden gebruikt als externe verwijzing"
			}
		]
	},
	{
		name: "AFRONDEN",
		description: "Rondt een getal af op het opgegeven aantal decimalen.",
		arguments: [
			{
				name: "getal",
				description: "is het getal dat u wilt afronden"
			},
			{
				name: "aantal-decimalen",
				description: "is het aantal decimalen waarop u wilt afronden. Negatieve waarden ronden links van de komma af, nul rondt af op het dichtstbijzijnde gehele getal"
			}
		]
	},
	{
		name: "AFRONDEN.BENEDEN",
		description: "Rondt een getal naar beneden af op het dichtstbijzijnde significante veelvoud.",
		arguments: [
			{
				name: "getal",
				description: "is de numerieke waarde die u wilt afronden"
			},
			{
				name: "significantie",
				description: "is het veelvoud waarop u wilt afronden. Zowel Getal als Significantie moeten beide positief of beide negatief zijn"
			}
		]
	},
	{
		name: "AFRONDEN.BENEDEN.NAUWKEURIG",
		description: "Rondt een getal naar beneden af op het dichtstbijzijnde gehele getal of het dichtstbijzijnde significante veelvoud.",
		arguments: [
			{
				name: "getal",
				description: "is de numerieke waarde die u wilt afronden"
			},
			{
				name: "significantie",
				description: "is het veelvoud waarop u wilt afronden. "
			}
		]
	},
	{
		name: "AFRONDEN.BENEDEN.WISK",
		description: "Rondt het getal naar beneden af op het dichtstbijzijnde gehele getal of op het dichtstbijzijnde veelvoud van significantie.",
		arguments: [
			{
				name: "getal",
				description: "is de waarde die u wilt afronden"
			},
			{
				name: "significantie",
				description: "is het veelvoud waarop u wilt afronden"
			},
			{
				name: "modus",
				description: "wanneer deze functie is opgegeven en niet nul is, wordt er naar beneden afgerond"
			}
		]
	},
	{
		name: "AFRONDEN.BOVEN",
		description: "Rondt een getal naar boven af op het dichtstbijzijnde significante veelvoud.",
		arguments: [
			{
				name: "getal",
				description: "is de waarde die u wilt afronden"
			},
			{
				name: "significantie",
				description: "is het veelvoud waarop u wilt afronden"
			}
		]
	},
	{
		name: "AFRONDEN.BOVEN.NAUWKEURIG",
		description: "Rondt een getal naar boven af op het dichtstbijzijnde gehele getal of het dichtstbijzijnde significante veelvoud.",
		arguments: [
			{
				name: "getal",
				description: "is de waarde die u wilt afronden"
			},
			{
				name: "significantie",
				description: "is het veelvoud waarop u wilt afronden"
			}
		]
	},
	{
		name: "AFRONDEN.BOVEN.WISK",
		description: "Rondt een getal naar boven af tot op het dichtstbijzijnde gehele getal of op het dichtstbijzijnde veelvoud van significantie.",
		arguments: [
			{
				name: "getal",
				description: "is de waarde die u wilt afronden"
			},
			{
				name: "significantie",
				description: "is het veelvoud waarop u wilt afronden"
			},
			{
				name: "modus",
				description: "wanneer deze functie is opgegeven en niet nul is, wordt er naar boven afgerond"
			}
		]
	},
	{
		name: "AFRONDEN.N.VEELVOUD",
		description: "Geeft een getal dat is afgerond op het gewenste veelvoud.",
		arguments: [
			{
				name: "getal",
				description: "is de waarde die moet worden afgerond"
			},
			{
				name: "veelvoud",
				description: "is het veelvoud waarop u het getal wilt afronden"
			}
		]
	},
	{
		name: "AFRONDEN.NAAR.BENEDEN",
		description: "Rondt de absolute waarde van een getal naar beneden af.",
		arguments: [
			{
				name: "getal",
				description: "is een reëel getal dat u naar beneden wilt afronden"
			},
			{
				name: "aantal-decimalen",
				description: "is het aantal decimalen waarop u het getal wilt afronden. Negatieve waarden ronden links van de komma af, nul of weggelaten rondt af op het dichtstbijzijnde gehele getal"
			}
		]
	},
	{
		name: "AFRONDEN.NAAR.BOVEN",
		description: "Rondt de absolute waarde van een getal naar boven af.",
		arguments: [
			{
				name: "getal",
				description: "is een reëel getal dat u naar boven wilt afronden"
			},
			{
				name: "aantal-decimalen",
				description: "is het aantal decimalen waarop u het getal wilt afronden. Negatieve waarden ronden links van de komma af, nul of weggelaten rondt af op het dichtstbijzijnde gehele getal"
			}
		]
	},
	{
		name: "ALS",
		description: "Controleert of er aan een voorwaarde is voldaan. Geeft een bepaalde waarde als resultaat als de opgegeven voorwaarde WAAR is en een andere waarde als deze ONWAAR is.",
		arguments: [
			{
				name: "logische-test",
				description: "is een waarde of expressie die WAAR of ONWAAR als resultaat kan geven"
			},
			{
				name: "waarde-als-waar",
				description: "is de waarde die als resultaat wordt gegeven als logische-test WAAR is. Als dit wordt weggelaten, wordt als resultaat WAAR gegeven. U kunt maximaal zeven ALS-functies nesten"
			},
			{
				name: "waarde-als-onwaar",
				description: "is de waarde die als resultaat wordt gegeven als logische-test ONWAAR is. Als dit wordt weggelaten, wordt als resultaat ONWAAR gegeven"
			}
		]
	},
	{
		name: "ALS.FOUT",
		description: "Geeft als resultaat de waarde_indien_fout als de expressie een fout is en anders de waarde van de expressie zelf.",
		arguments: [
			{
				name: "waarde",
				description: "is een willekeurige waarde of expressie of verwijzing"
			},
			{
				name: "waarde_indien_fout",
				description: "is een willekeurige waarde of expressie of verwijzing"
			}
		]
	},
	{
		name: "ALS.NB",
		description: "Geeft als resultaat de waarde die u opgeeft als de expressie wordt omgezet in #N.v.t, anders wordt het resultaat van de expressie geretourneerd.",
		arguments: [
			{
				name: "waarde",
				description: "is een waarde of expressie of verwijzing"
			},
			{
				name: "waarde_als_nvt",
				description: "is een waarde of expressie of verwijzing"
			}
		]
	},
	{
		name: "ARABISCH",
		description: "Converteert een Romeins cijfer naar een Arabisch cijfer.",
		arguments: [
			{
				name: "tekst",
				description: "is het Romeinse cijfer dat u wilt converteren"
			}
		]
	},
	{
		name: "ASELECT",
		description: "Geeft als resultaat een willekeurig getal, gelijkmatig verdeeld, dat groter is dan of gelijk is aan 0 en kleiner is dan 1 (wijzigt bij herberekening).",
		arguments: [
		]
	},
	{
		name: "ASELECTTUSSEN",
		description: "Geeft een willekeurig getal tussen de getallen die u hebt opgegeven.",
		arguments: [
			{
				name: "laagst",
				description: "is het kleinste gehele getal dat ASELECTTUSSEN als resultaat geeft"
			},
			{
				name: "hoogst",
				description: "is het grootste gehele getal dat ASELECTTUSSEN als resultaat geeft"
			}
		]
	},
	{
		name: "BAHT.TEKST",
		description: "Converteert een getal naar tekst (baht).",
		arguments: [
			{
				name: "getal",
				description: "is een getal dat u wilt converteren"
			}
		]
	},
	{
		name: "BASIS",
		description: "Converteert een getal in een tekstweergave met het opgegeven grondtal (basis).",
		arguments: [
			{
				name: "getal",
				description: "is het getal dat u wilt converteren"
			},
			{
				name: "grondtal",
				description: "is het basisgrondtal waarnaar u het getal wilt converteren"
			},
			{
				name: "min_lengte",
				description: "is de minimale lengte van de geretourneerde tekenreeks. Als weggelaten voorloopnullen niet worden toegevoegd"
			}
		]
	},
	{
		name: "BEGINLETTERS",
		description: "Zet de eerste letter van een tekenreeks om in een hoofdletter en converteert alle andere letters naar kleine letters.",
		arguments: [
			{
				name: "tekst",
				description: "is tekst tussen aanhalingstekens, een formule die tekst als resultaat geeft of een verwijzing naar een cel met tekst die u gedeeltelijk wilt omzetten in hoofdletters"
			}
		]
	},
	{
		name: "BEREIK",
		description: "",
		arguments: [
			{
				name: "",
				description: ""
			}
		]
	},
	{
		name: "BEREIKEN",
		description: "Geeft als resultaat het aantal bereiken in een verwijzing. Een bereik is een reeks aaneengesloten cellen of een enkele cel.",
		arguments: [
			{
				name: "verw",
				description: "is een verwijzing naar een cel, een celbereik of meerdere celbereiken"
			}
		]
	},
	{
		name: "BESSEL.I",
		description: "Berekent de gemodificeerde i-functie van Bessel In(x).",
		arguments: [
			{
				name: "x",
				description: "is de waarde waarop de functie moet worden geëvalueerd"
			},
			{
				name: "n",
				description: "is de volgorde waarin de Bessel-functie wordt uitgevoerd"
			}
		]
	},
	{
		name: "BESSEL.J",
		description: "Berekent de gemodificeerde j-functie van Bessel Jn(x).",
		arguments: [
			{
				name: "x",
				description: "is de waarde waarop de functie moet worden geëvalueerd"
			},
			{
				name: "n",
				description: "is de volgorde waarin de Bessel-functie wordt uitgevoerd"
			}
		]
	},
	{
		name: "BESSEL.K",
		description: "Berekent de gemodificeerde k-functie van Bessel Kn(x).",
		arguments: [
			{
				name: "x",
				description: "is de waarde waarop de functie moet worden geëvalueerd"
			},
			{
				name: "n",
				description: "is de volgorde waarin de functie wordt uitgevoerd"
			}
		]
	},
	{
		name: "BESSEL.Y",
		description: "Berekent de gemodificeerde y-functie van Bessel Yn(x).",
		arguments: [
			{
				name: "x",
				description: "is de waarde waarop de functie moet worden geëvalueerd"
			},
			{
				name: "n",
				description: "is de volgorde waarin de functie wordt uitgevoerd"
			}
		]
	},
	{
		name: "BET",
		description: "Berekent de periodieke betaling voor een lening op basis van constante betalingen en een constant rentepercentage.",
		arguments: [
			{
				name: "rente",
				description: "is het rentepercentage per termijn voor de lening. Gebruik bijvoorbeeld 6%/4 voor kwartaalbetalingen met een rentepercentage van 6%"
			},
			{
				name: "aantal-termijnen",
				description: "is het totale aantal betalingstermijnen van een lening"
			},
			{
				name: "hw",
				description: "is de huidige waarde: het totaalbedrag dat een reeks toekomstige waarden op dit moment waard is"
			},
			{
				name: "tw",
				description: "is de toekomstige waarde of het kassaldo waarover u wilt beschikken als de laatste betaling is gedaan. Als dit wordt weggelaten, wordt uitgegaan van 0 (nul)"
			},
			{
				name: "type_getal",
				description: "is een logische waarde: 1 = betaling aan het begin van de periode, 0 of weggelaten = betaling aan het einde van de periode"
			}
		]
	},
	{
		name: "BETA.INV",
		description: "Berekent de inverse van de cumulatieve bètakansdichtheidsfunctie (BETA.VERD).",
		arguments: [
			{
				name: "kans",
				description: "is een kans die samenhangt met de bètaverdeling"
			},
			{
				name: "alfa",
				description: "is een parameter voor de verdeling en moet groter zijn dan 0"
			},
			{
				name: "beta",
				description: "is een parameter voor de verdeling en moet groter zijn dan 0"
			},
			{
				name: "A",
				description: "is een optionele ondergrens voor het interval van x. Als dit wordt weggelaten, wordt uitgegaan van A = 0"
			},
			{
				name: "B",
				description: "is een optionele bovengrens voor het interval van x. Als dit wordt weggelaten, wordt uitgegaan van B = 1"
			}
		]
	},
	{
		name: "BETA.VERD",
		description: "Berekent de bètakansverdelingsfunctie.",
		arguments: [
			{
				name: "x",
				description: "is de waarde tussen A en B waarvoor de functie wordt geëvalueerd"
			},
			{
				name: "alfa",
				description: "is een parameter voor de verdeling en moet groter zijn dan 0"
			},
			{
				name: "beta",
				description: "is een parameter voor de verdeling en moet groter zijn dan 0"
			},
			{
				name: "cumulatief",
				description: "is een logische waarde: gebruik WAAR voor de cumulatieve verdelingsfunctie en gebruik ONWAAR voor de kansdichtheidsfunctie"
			},
			{
				name: "A",
				description: "is een optionele ondergrens voor het interval van x. Als dit wordt weggelaten, wordt uitgegaan van A = 0"
			},
			{
				name: "B",
				description: "is een optionele bovengrens voor het interval van x. Als dit wordt weggelaten, wordt uitgegaan van B = 1"
			}
		]
	},
	{
		name: "BETAINV",
		description: "Berekent de inverse van de cumulatieve bèta-kansdichtheidsfunctie (BETAVERD).",
		arguments: [
			{
				name: "kans",
				description: "is een kans die samenhangt met de bèta-verdeling"
			},
			{
				name: "alfa",
				description: "is een parameter voor de verdeling. De parameter moet groter zijn dan 0"
			},
			{
				name: "beta",
				description: "is een parameter voor de verdeling. De parameter moet groter zijn dan 0"
			},
			{
				name: "A",
				description: "is een optionele ondergrens voor het interval van x. Als dit wordt weggelaten, wordt uitgegaan van A = 0"
			},
			{
				name: "B",
				description: "is een optionele bovengrens voor het interval van x. Als deze waarde wordt weggelaten, wordt uitgegaan van B = 1"
			}
		]
	},
	{
		name: "BETAVERD",
		description: "Berekent de cumulatieve bèta-kansdichtheidsfunctie.",
		arguments: [
			{
				name: "x",
				description: "is de waarde tussen A en B waarop de functie geëvalueerd moet worden"
			},
			{
				name: "alfa",
				description: "is een parameter voor de verdeling. De parameter moet groter zijn dan 0"
			},
			{
				name: "bèta",
				description: "is een parameter voor de verdeling. De parameter moet groter zijn dan 0"
			},
			{
				name: "A",
				description: "is een optionele ondergrens voor het interval van x. Als dit wordt weggelaten, wordt uitgegaan van A = 0"
			},
			{
				name: "B",
				description: "is een optionele bovengrens voor het interval van x. Als deze waarde wordt weggelaten, wordt uitgegaan van B = 1"
			}
		]
	},
	{
		name: "BETROUWBAARHEID",
		description: "Berekent de betrouwbaarheidsinterval van een gemiddelde waarde voor de elementen van een populatie, met een normale verdeling.",
		arguments: [
			{
				name: "alfa",
				description: "is het significantieniveau op basis waarvan de betrouwbaarheid wordt berekend. Dit is een getal groter dan 0 en kleiner dan 1"
			},
			{
				name: "standaarddev",
				description: "is de standaarddeviatie voor het gegevensbereik binnen de populatie. Deze wordt verondersteld bekend te zijn. Standaarddev moet groter dan 0 zijn"
			},
			{
				name: "grootte",
				description: "is de grootte van de steekproef"
			}
		]
	},
	{
		name: "BIN.N.DEC",
		description: "Converteert een binair getal naar een decimaal getal.",
		arguments: [
			{
				name: "getal",
				description: "is het binaire getal dat u wilt converteren"
			}
		]
	},
	{
		name: "BIN.N.HEX",
		description: "Converteert een binair getal naar een hexadecimaal getal.",
		arguments: [
			{
				name: "getal",
				description: "is het binaire getal dat u wilt converteren"
			},
			{
				name: "aantal_tekens",
				description: "is het aantal tekens dat u wilt gebruiken"
			}
		]
	},
	{
		name: "BIN.N.OCT",
		description: "Converteert een binair getal naar een octaal getal.",
		arguments: [
			{
				name: "getal",
				description: "is het binaire getal dat u wilt converteren"
			},
			{
				name: "aantal_tekens",
				description: "is het aantal tekens dat u wilt gebruiken"
			}
		]
	},
	{
		name: "BINOM.VERD",
		description: "Geeft als resultaat de binomiale verdeling.",
		arguments: [
			{
				name: "aantal-gunstig",
				description: "is het aantal gunstige uitkomsten in een experiment"
			},
			{
				name: "experimenten",
				description: "is het aantal onafhankelijke experimenten"
			},
			{
				name: "kans-gunstig",
				description: "is de kans op een gunstige uitkomst bij elk experiment"
			},
			{
				name: "cumulatief",
				description: "is een logische waarde. Gebruik WAAR voor de cumulatieve verdelingsfunctie en ONWAAR voor de kansdichtheidsfunctie"
			}
		]
	},
	{
		name: "BINOM.VERD.BEREIK",
		description: "Geeft als resultaat de kans van een proefresultaat waarvoor een binomiale verdeling wordt gebruikt.",
		arguments: [
			{
				name: "proeven",
				description: "is het aantal onafhankelijke proeven"
			},
			{
				name: "kans_succes",
				description: "is de kans op succes voor elke proef"
			},
			{
				name: "getal_succes",
				description: "is het aantal successen in proeven"
			},
			{
				name: "getal_succes2",
				description: "als dit is opgegeven, wordt met deze functie de kans geretourneerd dat het aantal geslaagde proeven tussen getal_succes en getal_succes2 ligt"
			}
		]
	},
	{
		name: "BINOMIALE.INV",
		description: "Berekent de kleinste waarde waarvoor de cumulatieve binomiale verdeling kleiner is dan of gelijk aan een criteriumwaarde.",
		arguments: [
			{
				name: "experimenten",
				description: "is het aantal Bernoulli-experimenten"
			},
			{
				name: "kans-gunstig",
				description: "is de kans op een gunstige uitkomst bij elk experiment, een getal van 0 tot en met 1"
			},
			{
				name: "alfa",
				description: "is de waarde van het criterium, een getal van 0 tot en met 1"
			}
		]
	},
	{
		name: "BINOMIALE.VERD",
		description: "Geeft als resultaat de binomiale verdeling.",
		arguments: [
			{
				name: "aantal-gunstig",
				description: "is het aantal gunstige uitkomsten in een experiment"
			},
			{
				name: "experimenten",
				description: "is het aantal onafhankelijke experimenten"
			},
			{
				name: "kans-gunstig",
				description: "is de kans op een gunstige uitkomst bij elk experiment"
			},
			{
				name: "cumulatief",
				description: "is een logische waarde. Gebruik WAAR voor de cumulatieve verdelingsfunctie en ONWAAR voor de kansdichtheidsfunctie"
			}
		]
	},
	{
		name: "BIT.EN",
		description: "Geeft als resultaat een bitsgewijze 'En' van twee getallen.",
		arguments: [
			{
				name: "getal1",
				description: "is de decimale weergave van het binaire getal dat u wilt evalueren"
			},
			{
				name: "getal2",
				description: "is de decimale weergave van het binaire getal dat u wilt evalueren"
			}
		]
	},
	{
		name: "BIT.EX.OF",
		description: "Geeft als resultaat een bitsgewijze 'Exclusieve of' van twee getallen.",
		arguments: [
			{
				name: "getal1",
				description: "is de decimale weergave van het binaire getal dat u wilt evalueren"
			},
			{
				name: "getal2",
				description: "is de decimale weergave van het binaire getal dat u wilt evalueren"
			}
		]
	},
	{
		name: "BIT.OF",
		description: "Geeft als resultaat een bitsgewijze 'Of' van twee getallen.",
		arguments: [
			{
				name: "getal1",
				description: "is de decimale weergave van het binaire getal dat u wilt evalueren"
			},
			{
				name: "getal2",
				description: "is de decimale weergave van het binaire getal dat u wilt evalueren"
			}
		]
	},
	{
		name: "BIT.VERSCHUIF.LINKS",
		description: "Geeft als resultaat een getal dat naar links is verschoven met <verschuivingsaantal> bits.",
		arguments: [
			{
				name: "getal",
				description: "is de decimale weergave van het binaire getal dat u wilt evalueren"
			},
			{
				name: "verschuivingsaantal",
				description: "is het aantal bits waarmee u het getal naar links wilt verschuiven"
			}
		]
	},
	{
		name: "BIT.VERSCHUIF.RECHTS",
		description: "Geeft als resultaat een getal dat naar rechts is verschoven met <verschuivingsaantal> bits.",
		arguments: [
			{
				name: "getal",
				description: "is de decimale weergave van het binaire getal dat u wilt evalueren"
			},
			{
				name: "verschuivingsaantal",
				description: "is het aantal bits waarmee u het getal naar rechts wilt verschuiven"
			}
		]
	},
	{
		name: "BLAD",
		description: "Geeft als resultaat het bladnummer van het werkblad waarnaar wordt verwezen.",
		arguments: [
			{
				name: "waarde",
				description: "is de naam van een blad of een verwijzing waarvoor u het bladnummer wilt weten. Als dit wordt weggelaten, wordt het nummer van het blad met de functie geretourneerd."
			}
		]
	},
	{
		name: "BLADEN",
		description: "Geeft als resultaat het aantal bladen in een verwijzing.",
		arguments: [
			{
				name: "verwijzing",
				description: "is een verwijzing waarvoor u wilt weten hoeveel bladen deze bevat. Als dit wordt weggelaten, wordt het aantal bladen in de werkmap met de functie geretourneerd."
			}
		]
	},
	{
		name: "BOOGCOS",
		description: "Geeft als resultaat de boogcosinus van een getal in radialen, in het bereik 0 tot pi. De boogcosinus is de hoek waarvan de cosinus Getal is.",
		arguments: [
			{
				name: "getal",
				description: "is de cosinus waarvan u de hoek wilt weten. Het argument moet een waarde zijn tussen -1 en 1"
			}
		]
	},
	{
		name: "BOOGCOSH",
		description: "Berekent de inverse cosinus hyperbolicus van een getal.",
		arguments: [
			{
				name: "getal",
				description: "is een reëel getal dat groter is dan of gelijk is aan 1"
			}
		]
	},
	{
		name: "BOOGCOT",
		description: "Geeft als resultaat de boogcotangens van een getal in het bereik 0 tot Pi radialen.",
		arguments: [
			{
				name: "getal",
				description: "is de cotangens van de gewenste hoek"
			}
		]
	},
	{
		name: "BOOGCOTH",
		description: "Geeft als resultaat de inverse cotangens hyperbolicus van een getal.",
		arguments: [
			{
				name: "getal",
				description: "is de cotangens hyperbolicus van de gewenste hoek"
			}
		]
	},
	{
		name: "BOOGSIN",
		description: "Berekent de boogsinus van een getal, in het bereik -pi/2 tot pi/2.",
		arguments: [
			{
				name: "getal",
				description: "is de sinus waarvan u de hoek wilt berekenen. Het argument moet een getal tussen -1 en 1 zijn"
			}
		]
	},
	{
		name: "BOOGSINH",
		description: "Berekent de inverse sinus hyperbolicus van een getal.",
		arguments: [
			{
				name: "getal",
				description: "is een reëel getal dat groter is dan of gelijk is aan 1"
			}
		]
	},
	{
		name: "BOOGTAN",
		description: "Berekent de boogtangens van een getal, in het bereik -pi/2 tot pi/2.",
		arguments: [
			{
				name: "getal",
				description: "is de tangens waarvan u de hoek wilt berekenen"
			}
		]
	},
	{
		name: "BOOGTAN2",
		description: "Berekent de boogtangens van de x- en y-coördinaten in radialen, tussen -pi en pi, met -pi uitgesloten.",
		arguments: [
			{
				name: "x_getal",
				description: "is de x-coördinaat van het punt"
			},
			{
				name: "y_getal",
				description: "is de y-coördinaat van het punt"
			}
		]
	},
	{
		name: "BOOGTANH",
		description: "Berekent de inverse tangens hyperbolicus van een getal.",
		arguments: [
			{
				name: "getal",
				description: "is een reëel getal tussen -1 en 1"
			}
		]
	},
	{
		name: "C.ABS",
		description: "Geeft de absolute waarde van een complex getal.",
		arguments: [
			{
				name: "complex_getal",
				description: "is een complex getal waarvan u de absolute waarde wilt berekenen"
			}
		]
	},
	{
		name: "C.ARGUMENT",
		description: "Berekent het argument theta, een hoek uitgedrukt in radialen.",
		arguments: [
			{
				name: "complex_getal",
				description: "is een complex getal waarvan u het argument theta wilt berekenen"
			}
		]
	},
	{
		name: "C.COS",
		description: "Berekent de cosinus van een complex getal.",
		arguments: [
			{
				name: "complex_getal",
				description: "is een complex getal waarvan u de cosinus wilt berekenen"
			}
		]
	},
	{
		name: "C.COSEC",
		description: "Geeft als resultaat de cosecans van een complex getal.",
		arguments: [
			{
				name: "igetal",
				description: "is een complex getal waarvoor u de cosecans wilt berekenen"
			}
		]
	},
	{
		name: "C.COSECH",
		description: "Geeft als resultaat de cosecans hyperbolicus van een complex getal.",
		arguments: [
			{
				name: "igetal",
				description: "is een complex getal waarvoor u de cosecans hyperbolicus wilt berekenen"
			}
		]
	},
	{
		name: "C.COSH",
		description: "Geeft als resultaat de cosinus hyperbolicus van een complex getal.",
		arguments: [
			{
				name: "igetal",
				description: "is een complex getal waarvoor u de cosinus hyperbolicus wilt berekenen"
			}
		]
	},
	{
		name: "C.COT",
		description: "Geeft als resultaat de cotangens van een complex getal.",
		arguments: [
			{
				name: "igetal",
				description: "is een complex getal waarvoor u de cotangens wilt berekenen"
			}
		]
	},
	{
		name: "C.EXP",
		description: "Berekent de exponentiële waarde van een complex getal.",
		arguments: [
			{
				name: "complex_getal",
				description: "is een complex getal waarvan u de exponent wilt berekenen"
			}
		]
	},
	{
		name: "C.IM.DEEL",
		description: "Berekent de imaginaire coëfficiënt van een complex getal.",
		arguments: [
			{
				name: "complex_getal",
				description: "is een complex getal waarvan u de imaginaire coëfficiënt wilt berekenen"
			}
		]
	},
	{
		name: "C.LN",
		description: "Berekent de natuurlijke logaritme van een complex getal.",
		arguments: [
			{
				name: "complex_getal",
				description: "is een complex getal waarvan u de natuurlijke logaritme wilt berekenen"
			}
		]
	},
	{
		name: "C.LOG10",
		description: "Berekent de logaritme met grondtal 10 van een complex getal.",
		arguments: [
			{
				name: "complex_getal",
				description: "is een complex getal waarvan u de logaritme met het grondtal 10 wilt berekenen"
			}
		]
	},
	{
		name: "C.LOG2",
		description: "Berekent de logaritme met grondtal 2 van een complex getal.",
		arguments: [
			{
				name: "complex_getal",
				description: "is een complex getal waarvan u de logaritme met het grondtal 2 wilt berekenen"
			}
		]
	},
	{
		name: "C.MACHT",
		description: "Verheft een complex getal tot een hele macht.",
		arguments: [
			{
				name: "complex_getal",
				description: "is een complex getal dat u tot een bepaalde macht wilt verheffen"
			},
			{
				name: "getal",
				description: "is de macht waartoe u het complexe getal wilt verheffen"
			}
		]
	},
	{
		name: "C.PRODUCT",
		description: "Berekent het product van 1 tot 255 complexe getallen.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "complex_getal1",
				description: "complex_getal1, complex_getal,... zijn van 1 tot 255 complexe getallen die worden vermenigvuldigd."
			},
			{
				name: "complex_getal2",
				description: "complex_getal1, complex_getal,... zijn van 1 tot 255 complexe getallen die worden vermenigvuldigd."
			}
		]
	},
	{
		name: "C.QUOTIENT",
		description: "Berekent het quotiënt van twee complexe getallen.",
		arguments: [
			{
				name: "complex_getal1",
				description: "is de complexe teller of het complexe deeltal"
			},
			{
				name: "complex_getal2",
				description: "is de complexe noemer of de complexe deler"
			}
		]
	},
	{
		name: "C.REEEL.DEEL",
		description: "Bepaalt de reële coëfficiënt van een complex getal.",
		arguments: [
			{
				name: "complex_getal",
				description: "is een complex getal waarvan u de reële coëfficiënt wilt berekenen"
			}
		]
	},
	{
		name: "C.SEC",
		description: "Geeft als resultaat de secans van een complex getal.",
		arguments: [
			{
				name: "igetal",
				description: "is een complex getal waarvoor u de secans wilt berekenen"
			}
		]
	},
	{
		name: "C.SECH",
		description: "Geeft als resultaat de secans hyperbolicus van een complex getal.",
		arguments: [
			{
				name: "igetal",
				description: "is een complex getal waarvoor u de secans hyperbolicus wilt berekenen"
			}
		]
	},
	{
		name: "C.SIN",
		description: "Berekent de sinus van een complex getal.",
		arguments: [
			{
				name: "complex_getal",
				description: "is een complex getal waarvan u de sinus wilt berekenen"
			}
		]
	},
	{
		name: "C.SINH",
		description: "Geeft als resultaat de sinus hyperbolicus van een complex getal.",
		arguments: [
			{
				name: "igetal",
				description: "is een complex getal waarvoor u de sinus hyperbolicus wilt berekenen"
			}
		]
	},
	{
		name: "C.SOM",
		description: "Geeft als resultaat de som van complexe getallen.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "complex_getal1",
				description: "zijn van 1 tot 255 complexe getallen die worden toegevoegd"
			},
			{
				name: "complex_getal2",
				description: "zijn van 1 tot 255 complexe getallen die worden toegevoegd"
			}
		]
	},
	{
		name: "C.TAN",
		description: "Geeft als resultaat de tangens van een complex getal.",
		arguments: [
			{
				name: "igetal",
				description: "is een complex getal waarvoor u de tangens wilt berekenen"
			}
		]
	},
	{
		name: "C.TOEGEVOEGD",
		description: "Berekent de complex toegevoegde van een complex getal.",
		arguments: [
			{
				name: "complex_getal",
				description: "is een complex getal waarvan u de complex toegevoegde wilt berekenen"
			}
		]
	},
	{
		name: "C.VERSCHIL",
		description: "Berekent het verschil tussen twee complexe getallen.",
		arguments: [
			{
				name: "complex_getal1",
				description: "is het complexe getal waarvan u complex_getal2 wilt aftrekken"
			},
			{
				name: "complex_getal2",
				description: "is het complexe getal dat u van complex_getal1 wilt aftrekken"
			}
		]
	},
	{
		name: "C.WORTEL",
		description: "Berekent de vierkantswortel van een complex getal.",
		arguments: [
			{
				name: "complex_getal",
				description: "is een complex getal waarvan u de vierkantswortel wilt berekenen"
			}
		]
	},
	{
		name: "CEL",
		description: "Geeft als resultaat informatie over de opmaak, locatie of inhoud van de eerste cel (volgens de leesrichting van het werkblad) in een verwijzing.",
		arguments: [
			{
				name: "infotype",
				description: "is een tekstwaarde waarmee u aangeeft welk type celinformatie u wilt hebben."
			},
			{
				name: "verw",
				description: "is de cel waarover u de informatie wilt hebben"
			}
		]
	},
	{
		name: "CHI.KWADRAAT",
		description: "Berekent de rechtszijdige kans van de chi-kwadraatverdeling.",
		arguments: [
			{
				name: "x",
				description: "is de waarde waarop u de verdeling wilt evalueren. Dit is een niet-negatief getal"
			},
			{
				name: "vrijheidsgraden",
				description: "is het aantal vrijheidsgraden. Dit is een getal tussen 1 en 10^10, met 10^10 uitgesloten"
			}
		]
	},
	{
		name: "CHI.KWADRAAT.INV",
		description: "Berekent de inverse van de rechtszijdige kans van de chi-kwadraatverdeling.",
		arguments: [
			{
				name: "kans",
				description: "is een kans die samenhangt met de chi-kwadraatverdeling. Dit is een waarde van 0 tot en met 1"
			},
			{
				name: "vrijheidsgraden",
				description: "is het aantal vrijheidsgraden. Dit is een getal tussen 1 en 10^10, met 10^10 uitgesloten"
			}
		]
	},
	{
		name: "CHI.TOETS",
		description: "Geeft het resultaat van de onafhankelijkheidstoets: de waarde van de chi-kwadraatverdeling voor de toetsingsgrootheid en de ingestelde vrijheidsgraden.",
		arguments: [
			{
				name: "waarnemingen",
				description: "is het gegevensbereik met de waarnemingen die u aan de hand van de verwachte waarden wilt toetsen"
			},
			{
				name: "verwacht",
				description: "is het gegevensbereik met de verhouding tussen enerzijds het product van de rijtotalen en kolomtotalen en anderzijds de eindtotalen"
			}
		]
	},
	{
		name: "CHIKW.INV",
		description: "Berekent de inverse van de linkszijdige kans van de chi-kwadraatverdeling.",
		arguments: [
			{
				name: "kans",
				description: "is een kans die met de chi-kwadraatverdeling samenhangt. Dit is een waarde tussen 0 en 1 inclusief"
			},
			{
				name: "vrijheidsgraden",
				description: "is het aantal vrijheidsgraden. Dit is een getal tussen 1 en 10^10, exclusief 10^10"
			}
		]
	},
	{
		name: "CHIKW.INV.RECHTS",
		description: "Berekent de inverse van de rechtszijdige kans van de chi-kwadraatverdeling.",
		arguments: [
			{
				name: "kans",
				description: "is een kans die met de chi-kwadraatverdeling samenhangt. Dit is een waarde tussen 0 en 1 inclusief"
			},
			{
				name: "vrijheidsgraden",
				description: "is het aantal vrijheidsgraden. Dit is een getal tussen 1 en 10^10, exclusief 10^10"
			}
		]
	},
	{
		name: "CHIKW.TEST",
		description: "Geeft het resultaat van de onafhankelijkheidstoets: de waarde van de chi-kwadraatverdeling voor de toetsingsgrootheid en de ingestelde vrijheidsgraden.",
		arguments: [
			{
				name: "waarnemingen",
				description: "is het gegevensbereik met de waarnemingen die u aan de hand van de verwachte waarden wilt toetsen"
			},
			{
				name: "verwacht",
				description: "is het gegevensbereik met de verhouding tussen enerzijds het product van de rijtotalen en kolomtotalen en anderzijds de eindtotalen"
			}
		]
	},
	{
		name: "CHIKW.VERD",
		description: "Berekent de linkszijdige kans van de chi-kwadraatverdeling.",
		arguments: [
			{
				name: "x",
				description: "is de waarde waarvoor u de verdeling wilt evalueren. Dit is een niet-negatief getal"
			},
			{
				name: "vrijheidsgraden",
				description: "is het aantal vrijheidsgraden. Dit is een getal tussen 1 en 10^10, exclusief 10^10"
			},
			{
				name: "cumulatief",
				description: "is een logische waarde die door de functie moet worden geretourneerd: de cumulatieve verdelingsfunctie = WAAR; de kansdichtheidsfunctie = ONWAAR"
			}
		]
	},
	{
		name: "CHIKW.VERD.RECHTS",
		description: "Berekent de rechtszijdige kans van de chi-kwadraatverdeling.",
		arguments: [
			{
				name: "x",
				description: "is de waarde waarvoor u de verdeling wilt evalueren. Dit is een niet-negatief getal,"
			},
			{
				name: "vrijheidsgraden",
				description: "is het aantal vrijheidsgraden. Dit is een getal tussen 1 en 10^10, exclusief 10^10"
			}
		]
	},
	{
		name: "CODE",
		description: "Geeft als resultaat de numerieke code voor het eerste teken in een tekenreeks voor de tekenset die door uw computer wordt gebruikt.",
		arguments: [
			{
				name: "tekst",
				description: "is de tekst waarvan u de numerieke code voor het eerste teken wilt weten"
			}
		]
	},
	{
		name: "COMBIN.A",
		description: "Geeft als resultaat het aantal combinaties met herhalingen voor een opgegeven aantal items.",
		arguments: [
			{
				name: "getal",
				description: "is het totale aantal items"
			},
			{
				name: "aantal_gekozen",
				description: "is het aantal items in elke combinatie"
			}
		]
	},
	{
		name: "COMBINATIES",
		description: "Geeft als resultaat het aantal combinaties voor een gegeven aantal objecten.",
		arguments: [
			{
				name: "getal",
				description: "is het aantal objecten"
			},
			{
				name: "aantal-gekozen",
				description: "is het aantal objecten in elke combinatie"
			}
		]
	},
	{
		name: "COMPLEX",
		description: "Converteert reële en imaginaire coëfficiënten naar complexe getallen.",
		arguments: [
			{
				name: "reëel_deel",
				description: "is de reële coëfficiënt van een complex getal"
			},
			{
				name: "imaginair_deel",
				description: "is de imaginaire coëfficiënt van het complexe getal"
			},
			{
				name: "achtervoegsel",
				description: "is het achtervoegsel voor het imaginaire deel van het complexe getal"
			}
		]
	},
	{
		name: "CONVERTEREN",
		description: "Converteert een getal in de ene maateenheid naar een getal in een andere maateenheid.",
		arguments: [
			{
				name: "getal",
				description: "is de waarde in van_eenheid die u wilt converteren"
			},
			{
				name: "van_eenheid",
				description: "is de eenheid die bij getal wordt gebruikt"
			},
			{
				name: "naar_eenheid",
				description: "is de eenheid die voor het resultaat wordt gebruikt"
			}
		]
	},
	{
		name: "CORRELATIE",
		description: "Berekent de correlatiecoëfficiënt van twee gegevensverzamelingen.",
		arguments: [
			{
				name: "matrix1",
				description: "is een celbereik met waarden. Dit kunnen getallen zijn of namen, matrices of verwijzingen die getallen bevatten"
			},
			{
				name: "matrix2",
				description: "is een tweede celbereik met waarden. Dit kunnen getallen zijn of namen, matrices of verwijzingen die getallen bevatten"
			}
		]
	},
	{
		name: "COS",
		description: "Berekent de cosinus van een getal.",
		arguments: [
			{
				name: "getal",
				description: "is de hoek in radialen waarvan u de cosinus wilt berekenen"
			}
		]
	},
	{
		name: "COSEC",
		description: "Geeft als resultaat de cosecans van een hoek.",
		arguments: [
			{
				name: "getal",
				description: "is de hoek in radialen waarvoor u de cosecans wilt berekenen"
			}
		]
	},
	{
		name: "COSECH",
		description: "Geeft als resultaat de cosecans hyperbolicus van een hoek.",
		arguments: [
			{
				name: "getal",
				description: "is de hoek in radialen waarvoor u de cosecans hyperbolicus wilt berekenen"
			}
		]
	},
	{
		name: "COSH",
		description: "Berekent de cosinus hyperbolicus van een getal.",
		arguments: [
			{
				name: "getal",
				description: "is een reëel getal"
			}
		]
	},
	{
		name: "COT",
		description: "Geeft als resultaat de cotangens van een hoek.",
		arguments: [
			{
				name: "getal",
				description: "is de hoek in radialen waarvoor u de cotangens wilt berekenen"
			}
		]
	},
	{
		name: "COTH",
		description: "Geeft als resultaat de cotangens hyperbolicus van een getal.",
		arguments: [
			{
				name: "getal",
				description: "is de hoek in radialen waarvan u de cotangens hyperbolicus wilt berekenen"
			}
		]
	},
	{
		name: "COUP.AANTAL",
		description: "Berekent het aantal coupons dat uitbetaald moet worden tussen de stortingsdatum en de vervaldatum.",
		arguments: [
			{
				name: "stortingsdatum",
				description: "is de stortingsdatum van het waardepapier, uitgedrukt in een serieel getal"
			},
			{
				name: "vervaldatum",
				description: "is de vervaldatum van het waardepapier, uitgedrukt in een serieel getal"
			},
			{
				name: "frequentie",
				description: "is het aantal couponuitbetalingen per jaar"
			},
			{
				name: "soort_jaar",
				description: "is het soort jaar waarop de berekening is gebaseerd"
			}
		]
	},
	{
		name: "COUP.DAGEN.BB",
		description: "Berekent het aantal dagen vanaf het begin van de couponperiode tot de stortingsdatum.",
		arguments: [
			{
				name: "stortingsdatum",
				description: "is de stortingsdatum van het waardepapier, uitgedrukt in een serieel getal"
			},
			{
				name: "vervaldatum",
				description: "is de vervaldatum van het waardepapier, uitgedrukt in een serieel getal"
			},
			{
				name: "frequentie",
				description: "is het aantal couponuitbetalingen per jaar"
			},
			{
				name: "soort_jaar",
				description: "is het soort jaar waarop de berekening is gebaseerd"
			}
		]
	},
	{
		name: "COUP.DATUM.NB",
		description: "Bepaalt de volgende coupondatum na de stortingsdatum.",
		arguments: [
			{
				name: "stortingsdatum",
				description: "is de stortingsdatum van het waardepapier, uitgedrukt in een serieel getal"
			},
			{
				name: "vervaldatum",
				description: "is de vervaldatum van het waardepapier, uitgedrukt in een serieel getal"
			},
			{
				name: "frequentie",
				description: "is het aantal couponuitbetalingen per jaar"
			},
			{
				name: "soort_jaar",
				description: "is het soort jaar waarop de berekening is gebaseerd"
			}
		]
	},
	{
		name: "COUP.DATUM.VB",
		description: "Berekent de laatste coupondatum voor de stortingsdatum.",
		arguments: [
			{
				name: "stortingsdatum",
				description: "is de stortingsdatum van het waardepapier, uitgedrukt in een serieel getal"
			},
			{
				name: "vervaldatum",
				description: "is de vervaldatum van het waardepapier, uitgedrukt in een serieel getal"
			},
			{
				name: "frequentie",
				description: "is het aantal couponuitbetalingen per jaar"
			},
			{
				name: "soort_jaar",
				description: "is het soort jaar waarop de berekening is gebaseerd"
			}
		]
	},
	{
		name: "COVARIANTIE",
		description: "Berekent de covariantie, het gemiddelde van de producten van deviaties voor ieder paar gegevenspunten in twee gegevenssets.",
		arguments: [
			{
				name: "matrix1",
				description: "is het eerste celbereik met gehele getallen. Dit moeten getallen zijn of namen, matrices of verwijzingen die getallen bevatten"
			},
			{
				name: "matrix2",
				description: "is het tweede celbereik met gehele getallen. Dit moeten getallen zijn of matrices of verwijzingen die getallen bevatten"
			}
		]
	},
	{
		name: "COVARIANTIE.P",
		description: "Berekent de covariantie van de populatie, het gemiddelde van de producten van deviaties voor ieder paar gegevenspunten in twee gegevenssets.",
		arguments: [
			{
				name: "matrix1",
				description: "is het eerste celbereik met gehele getallen. Dit kunnen getallen zijn of matrices of verwijzingen die getallen bevatten"
			},
			{
				name: "matrix2",
				description: "is het tweede celbereik met gehele getallen. Dit kunnen getallen zijn of matrices of verwijzingen die getallen bevatten"
			}
		]
	},
	{
		name: "COVARIANTIE.S",
		description: "Berekent de covariantie voor een steekproef, het gemiddelde van de producten van deviaties voor ieder paar gegevenspunten in twee gegevenssets.",
		arguments: [
			{
				name: "matrix1",
				description: "is het eerste celbereik met gehele getallen. Dit kunnen getallen zijn of namen, matrices of verwijzingen die getallen bevatten"
			},
			{
				name: "matrix2",
				description: "is het tweede celbereik met gehele getallen. Dit kunnen getallen zijn of matrices of verwijzingen die getallen bevatten"
			}
		]
	},
	{
		name: "CRIT.BINOM",
		description: "Berekent de kleinste waarde waarvoor de cumulatieve binomiale verdeling kleiner is dan of gelijk aan een criteriumwaarde.",
		arguments: [
			{
				name: "experimenten",
				description: "is het aantal Bernoulli-experimenten"
			},
			{
				name: "kans-gunstig",
				description: "is de kans op een gunstige uitkomst bij elk experiment, een getal van 0 tot en met 1"
			},
			{
				name: "alfa",
				description: "is de waarde van het criterium, een getal van 0 tot en met 1"
			}
		]
	},
	{
		name: "CUM.HOOFDSOM",
		description: "Berekent de cumulatieve terugbetaalde hoofdsom voor een lening over een bepaalde periode.",
		arguments: [
			{
				name: "rente",
				description: "is het rentepercentage"
			},
			{
				name: "aantal_termijnen",
				description: "is het totale aantal betalingstermijnen"
			},
			{
				name: "hw",
				description: "is de huidige waarde"
			},
			{
				name: "begin_periode",
				description: "is de eerste termijn in de periode waarover u de cumulatieve terugbetaalde hoofdsom wilt berekenen"
			},
			{
				name: "einde_periode",
				description: "is de laatste termijn in de periode waarover u de cumulatieve terugbetaalde hoofdsom wilt berekenen"
			},
			{
				name: "type_getal",
				description: "is het tijdstip van de betaling"
			}
		]
	},
	{
		name: "CUM.RENTE",
		description: "Berekent de cumulatieve rente over een bepaalde periode.",
		arguments: [
			{
				name: "rente",
				description: "is het rentepercentage"
			},
			{
				name: "aantal_termijnen",
				description: "is het totale aantal betalingstermijnen"
			},
			{
				name: "hw",
				description: "is de huidige waarde"
			},
			{
				name: "begin_periode",
				description: "is de eerste termijn in de periode waarover u de cumulatieve rente wilt berekenen"
			},
			{
				name: "einde_periode",
				description: "is de laatste termijn in de periode waarover u de cumulatieve rente wilt berekenen"
			},
			{
				name: "type_getal",
				description: "is het tijdstip van de betaling"
			}
		]
	},
	{
		name: "DAG",
		description: "Geeft als resultaat de dag van de maand, een getal tussen 1 en 31.",
		arguments: [
			{
				name: "serieel-getal",
				description: "is een getal in het systeem dat in Spreadsheet wordt gebruikt voor datumberekeningen"
			}
		]
	},
	{
		name: "DAGEN",
		description: "Geeft als resultaat het aantal dagen tussen twee datums.",
		arguments: [
			{
				name: "einddatum",
				description: "begindatum en einddatum zijn de twee datums waartussen u het aantal dagen wilt weten"
			},
			{
				name: "begindatum",
				description: "begindatum en einddatum zijn de twee datums waartussen u het aantal dagen wilt weten"
			}
		]
	},
	{
		name: "DAGEN360",
		description: "Berekent het aantal dagen tussen twee datums op basis van een jaar met 360 dagen (12 maanden van 30 dagen).",
		arguments: [
			{
				name: "begindatum",
				description: "begindatum en einddatum zijn de twee datums waartussen u het aantal dagen wilt bepalen"
			},
			{
				name: "einddatum",
				description: "begindatum en einddatum zijn de twee datums waartussen u het aantal dagen wilt bepalen"
			},
			{
				name: "methode",
				description: "is een logische waarde die de methode bepaalt die u in de berekening wilt gebruiken: V.S. (NASD) = ONWAAR of leeg, Europees = WAAR."
			}
		]
	},
	{
		name: "DATUM",
		description: "Zet de opgegeven datum om in de Spreadsheet-code voor de datum/tijd.",
		arguments: [
			{
				name: "jaar",
				description: "is een getal tussen 1900 en 9999 in Spreadsheet voor Windows of tussen 1904 en 9999 in Spreadsheet voor de Apple Macintosh"
			},
			{
				name: "maand",
				description: "is een getal van 1 tot 12 dat de maand van het jaar aangeeft"
			},
			{
				name: "dag",
				description: "is een getal van 1 tot 31 dat de dag van de maand aangeeft"
			}
		]
	},
	{
		name: "DATUMVERSCHIL",
		description: "",
		arguments: [
		]
	},
	{
		name: "DATUMWAARDE",
		description: "Zet de opgegeven datum in de vorm van tekst om in de Spreadsheet-code voor de datum/tijd.",
		arguments: [
			{
				name: "datum_tekst",
				description: "is tekst die overeenkomt met een datum in een van de Spreadsheet-datumnotaties, tussen 1-1-1900 (Windows) of 1-1-1904 (Macintosh) en 31-12-9999"
			}
		]
	},
	{
		name: "DB",
		description: "Berekent de afschrijving van activa over een opgegeven termijn, met behulp van de 'fixed declining balance'-methode.",
		arguments: [
			{
				name: "kosten",
				description: "zijn de aanschafkosten van de activa"
			},
			{
				name: "restwaarde",
				description: "is de resterende waarde van de activa aan het einde van de afschrijving"
			},
			{
				name: "duur",
				description: "is het aantal termijnen waarin de activa worden afgeschreven (ook wel levensduur van de activa genoemd)"
			},
			{
				name: "termijn",
				description: "is de termijn waarvoor u de afschrijving wilt berekenen. Termijn moet in dezelfde eenheden worden opgegeven als duur"
			},
			{
				name: "maand",
				description: "is het aantal maanden in het eerste jaar. Als maand wordt weggelaten, wordt uitgegaan van 12"
			}
		]
	},
	{
		name: "DBAANTAL",
		description: "Telt de cellen in de database die getallen bevatten in het veld (kolom) met records die voldoen aan de opgegeven voorwaarden.",
		arguments: [
			{
				name: "database",
				description: "is het celbereik waaruit de lijst of database bestaat. Een database is een lijst met verwante gegevens"
			},
			{
				name: "veld",
				description: "is het label van de kolom tussen dubbele aanhalingstekens of een getal dat overeenkomt met de positie van de kolom in de lijst"
			},
			{
				name: "criteria",
				description: "is het celbereik dat de opgegeven voorwaarden bevat. Het bereik bevat een kolomlabel en een cel onder het label met een voorwaarde"
			}
		]
	},
	{
		name: "DBAANTALC",
		description: "Telt in de database de niet-lege cellen in het veld (kolom) met records die overeenkomen met de opgegeven voorwaarden.",
		arguments: [
			{
				name: "database",
				description: "is het celbereik dat de database omvat. Een database is een lijst met verwante gegevens"
			},
			{
				name: "veld",
				description: "is het label van de kolom tussen dubbele aanhalingstekens of een getal dat overeenkomt met de positie van de kolom in de lijst"
			},
			{
				name: "criteria",
				description: "is het celbereik dat de opgegeven voorwaarden bevat. Het bereik bevat een kolomlabel en een cel onder het label met een voorwaarde"
			}
		]
	},
	{
		name: "DBGEMIDDELDE",
		description: "Berekent het gemiddelde van de waarden in een kolom of database die voldoen aan de opgegeven voorwaarden.",
		arguments: [
			{
				name: "database",
				description: "is het celbereik waaruit de lijst of database bestaat. Een database is een lijst met verwante gegevens"
			},
			{
				name: "veld",
				description: "is het label van de kolom tussen dubbele aanhalingstekens of een getal dat overeenkomt met de positie van de kolom in de lijst"
			},
			{
				name: "criteria",
				description: "is het celbereik dat de opgegeven voorwaarden bevat. Het bereik bevat een kolomlabel en een cel onder het label met een voorwaarde"
			}
		]
	},
	{
		name: "DBLEZEN",
		description: "Haalt één record op uit een database dat voldoet aan de gespecificeerde criteria.",
		arguments: [
			{
				name: "database",
				description: "is het celbereik dat als database is gedefinieerd. Een database is een lijst met verwante gegevens"
			},
			{
				name: "veld",
				description: "is het label van de kolom tussen dubbele aanhalingstekens of een getal dat overeenkomt met de positie van de kolom in de lijst"
			},
			{
				name: "criteria",
				description: "is het celbereik dat de opgegeven voorwaarden bevat. Het bereik bevat een kolomlabel en een cel onder het label met een voorwaarde"
			}
		]
	},
	{
		name: "DBMAX",
		description: "Geeft als resultaat de maximumwaarde in het veld (kolom) met records in de database die overeenkomen met de opgegeven voorwaarden.",
		arguments: [
			{
				name: "database",
				description: "is het celbereik dat als database is gedefinieerd. Een database is een lijst met verwante gegevens"
			},
			{
				name: "veld",
				description: "is het label van de kolom tussen dubbele aanhalingstekens of een getal dat overeenkomt met de positie van de kolom in de lijst"
			},
			{
				name: "criteria",
				description: "is het celbereik dat de opgegeven voorwaarden bevat. Het bereik bevat een kolomlabel en een cel onder het label met een voorwaarde"
			}
		]
	},
	{
		name: "DBMIN",
		description: "Geeft als resultaat de minimumwaarde in het veld (kolom) met records in de database die overeenkomen met de opgegeven voorwaarden.",
		arguments: [
			{
				name: "database",
				description: "is het celbereik dat als database is gedefinieerd. Een database is een lijst met verwante gegevens"
			},
			{
				name: "veld",
				description: "is het label van de kolom tussen dubbele aanhalingstekens of een getal dat overeenkomt met de positie van de kolom in de lijst"
			},
			{
				name: "criteria",
				description: "is het celbereik dat de opgegeven voorwaarden bevat. Het bereik bevat een kolomlabel en een cel onder het label met een voorwaarde"
			}
		]
	},
	{
		name: "DBPRODUCT",
		description: "Vermenigvuldigt de waarden in het veld (kolom) met records in de database die voldoen aan de opgegeven voorwaarden.",
		arguments: [
			{
				name: "database",
				description: "is het celbereik dat als database is gedefinieerd. Een database is een lijst met verwante gegevens"
			},
			{
				name: "veld",
				description: "is het label van de kolom tussen dubbele aanhalingstekens of een getal dat overeenkomt met de positie van de kolom in de lijst"
			},
			{
				name: "criteria",
				description: "is het celbereik dat de opgegeven voorwaarden bevat. Het bereik bevat een kolomlabel en een cel onder het label met een voorwaarde"
			}
		]
	},
	{
		name: "DBSOM",
		description: "Telt de getallen op in het veld (kolom) met records in de database die voldoen aan de opgegeven voorwaarden.",
		arguments: [
			{
				name: "database",
				description: "is het celbereik dat als database is gedefinieerd. Een database is een lijst met verwante gegevens"
			},
			{
				name: "veld",
				description: "is het label van de kolom tussen dubbele aanhalingstekens of een getal dat overeenkomt met de positie van de kolom in de lijst"
			},
			{
				name: "criteria",
				description: "is het celbereik dat de opgegeven voorwaarden bevat. Het bereik bevat een kolomlabel en een cel onder het label met een voorwaarde"
			}
		]
	},
	{
		name: "DBSTDEV",
		description: "Maakt een schatting van de standaarddeviatie die is gebaseerd op een steekproef onder geselecteerde databasegegevens.",
		arguments: [
			{
				name: "database",
				description: "is het celbereik dat als database is gedefinieerd. Een database is een lijst met verwante gegevens"
			},
			{
				name: "veld",
				description: "is het label van de kolom tussen dubbele aanhalingstekens of een getal dat overeenkomt met de positie van de kolom in de lijst"
			},
			{
				name: "criteria",
				description: "is het celbereik dat de opgegeven voorwaarden bevat. Het bereik bevat een kolomlabel en een cel onder het label met een voorwaarde"
			}
		]
	},
	{
		name: "DBSTDEVP",
		description: "Berekent de standaarddeviatie die is gebaseerd op de hele populatie van geselecteerde databasegegevens.",
		arguments: [
			{
				name: "database",
				description: "is het celbereik dat als database is gedefinieerd. Een database is een lijst met verwante gegevens"
			},
			{
				name: "veld",
				description: "is het label van de kolom tussen dubbele aanhalingstekens of een getal dat overeenkomt met de positie van de kolom in de lijst"
			},
			{
				name: "criteria",
				description: "is het celbereik dat de opgegeven voorwaarden bevat. Het bereik bevat een kolomlabel en een cel onder het label met een voorwaarde"
			}
		]
	},
	{
		name: "DBVAR",
		description: "Maakt een schatting van de variantie die is gebaseerd op een steekproef onder geselecteerde databasegegevens.",
		arguments: [
			{
				name: "database",
				description: "is het celbereik dat als database is gedefinieerd. Een database is een lijst met verwante gegevens"
			},
			{
				name: "veld",
				description: "is het label van de kolom tussen dubbele aanhalingstekens of een getal dat overeenkomt met de positie van de kolom in de lijst"
			},
			{
				name: "criteria",
				description: "is het celbereik dat de opgegeven voorwaarden bevat. Het bereik bevat een kolomlabel en een cel onder het label met een voorwaarde"
			}
		]
	},
	{
		name: "DBVARP",
		description: "Berekent de variantie die is gebaseerd op de hele populatie van geselecteerde databasegegevens.",
		arguments: [
			{
				name: "database",
				description: "is het celbereik dat als database is gedefinieerd. Een database is een lijst met verwante gegevens"
			},
			{
				name: "veld",
				description: "is het label van de kolom tussen dubbele aanhalingstekens of een getal dat overeenkomt met de positie van de kolom in de lijst"
			},
			{
				name: "criteria",
				description: "is het celbereik dat de opgegeven voorwaarden bevat. Het bereik bevat een kolomlabel en een cel onder het label met een voorwaarde"
			}
		]
	},
	{
		name: "DDB",
		description: "Berekent de afschrijving van activa over een opgegeven termijn met de 'double declining balance'-methode of met een methode die u zelf bepaalt.",
		arguments: [
			{
				name: "kosten",
				description: "zijn de aanschafkosten van de activa"
			},
			{
				name: "restwaarde",
				description: "is de resterende waarde van de activa aan het einde van de afschrijving"
			},
			{
				name: "duur",
				description: "is het aantal termijnen waarin de activa worden afgeschreven (ook wel levensduur van de activa genoemd)"
			},
			{
				name: "termijn",
				description: "is de termijn waarvoor u de afschrijving wilt berekenen. Termijn moet in dezelfde eenheden worden opgegeven als duur"
			},
			{
				name: "factor",
				description: "is de snelheid waarmee wordt afgeschreven. Als u de factor weglaat, wordt uitgegaan van de waarde 2 ('double declining balance'-methode)"
			}
		]
	},
	{
		name: "DEC.N.BIN",
		description: "Converteert een decimaal getal naar een binair getal.",
		arguments: [
			{
				name: "getal",
				description: "is het decimale gehele getal dat u wilt converteren"
			},
			{
				name: "aantal_tekens",
				description: "is het aantal tekens dat u wilt gebruiken"
			}
		]
	},
	{
		name: "DEC.N.HEX",
		description: "Converteert een decimaal getal naar een hexadecimaal getal.",
		arguments: [
			{
				name: "getal",
				description: "is het decimale gehele getal dat u wilt converteren"
			},
			{
				name: "aantal_tekens",
				description: "is het aantal tekens dat u wilt gebruiken"
			}
		]
	},
	{
		name: "DEC.N.OCT",
		description: "Converteert een decimaal getal naar een octaal getal.",
		arguments: [
			{
				name: "getal",
				description: "is het decimale gehele getal dat u wilt converteren"
			},
			{
				name: "aantal_tekens",
				description: "is het aantal tekens dat u wilt gebruiken"
			}
		]
	},
	{
		name: "DECIMAAL",
		description: "Converteert een tekstweergave van de van een getal in een bepaalde basis naar een decimaal getal.",
		arguments: [
			{
				name: "getal",
				description: "is het getal dat u wilt converteren"
			},
			{
				name: "grondtal",
				description: "is het basisgrondtal van het getal dat u converteert"
			}
		]
	},
	{
		name: "DEEL",
		description: "Geeft als resultaat het aantal tekens in het midden van een tekenreeks, beginnend op een opgegeven positie en met een opgegeven lengte.",
		arguments: [
			{
				name: "tekst",
				description: "is de tekenreeks waaruit u de tekens wilt ophalen"
			},
			{
				name: "begin_getal",
				description: "is de positie van het eerste teken dat u wilt ophalen uit de tekenreeks. Het eerste teken in Tekst is 1"
			},
			{
				name: "aantal-tekens",
				description: "geeft aan hoeveel tekens u wilt ophalen uit Tekst"
			}
		]
	},
	{
		name: "DELTA",
		description: "Toetst of twee getallen gelijk zijn.",
		arguments: [
			{
				name: "getal1",
				description: "is het eerste getal"
			},
			{
				name: "getal2",
				description: "is het tweede getal"
			}
		]
	},
	{
		name: "DETERMINANTMAT",
		description: "Berekent de determinant van een matrix.",
		arguments: [
			{
				name: "matrix",
				description: "is een numerieke matrix met een gelijk aantal rijen en kolommen. Dit is een celbereik of een matrixconstante"
			}
		]
	},
	{
		name: "DEV.KWAD",
		description: "Berekent de som van de kwadraten van de deviaties van gegevenspunten ten opzichte van het gemiddelde van de steekproef.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "getal1",
				description: "zijn 1 tot 255 argumenten of een matrix of een verwijzing naar een matrix met getallen die u wilt gebruiken in de berekening met DEV.KWAD"
			},
			{
				name: "getal2",
				description: "zijn 1 tot 255 argumenten of een matrix of een verwijzing naar een matrix met getallen die u wilt gebruiken in de berekening met DEV.KWAD"
			}
		]
	},
	{
		name: "DISCONTO",
		description: "Berekent het discontopercentage voor waardepapier.",
		arguments: [
			{
				name: "stortingsdatum",
				description: "is de stortingsdatum van het waardepapier, uitgedrukt in een serieel getal"
			},
			{
				name: "vervaldatum",
				description: "is de vervaldatum van het waardepapier, uitgedrukt in een serieel getal"
			},
			{
				name: "prijs",
				description: "is de prijs van het waardepapier per 100 euro nominale waarde"
			},
			{
				name: "aflossingsprijs",
				description: "is de aflossingsprijs van het waardepapier per 100 euro nominale waarde"
			},
			{
				name: "soort_jaar",
				description: "is het soort jaar waarop de berekening is gebaseerd"
			}
		]
	},
	{
		name: "DRAAITABEL.OPHALEN",
		description: "Haalt gegevens op uit een draaitabel.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "gegevensveld",
				description: "is de naam van het gegevensveld waaruit gegevens moeten worden opgehaald"
			},
			{
				name: "draaitabel",
				description: "is een verwijzing naar een cel of celbereik in de draaitabel met de gegevens die u wilt ophalen"
			},
			{
				name: "veld",
				description: "veld waarnaar wordt verwezen"
			},
			{
				name: "item",
				description: "velditem waarnaar wordt verwezen"
			}
		]
	},
	{
		name: "DUBBELE.FACULTEIT",
		description: "Berekent de dubbele faculteit van een getal.",
		arguments: [
			{
				name: "getal",
				description: "is de waarde waarvoor u de dubbele faculteit wilt bepalen"
			}
		]
	},
	{
		name: "EENHEIDMAT",
		description: "Geeft als resultaat een eenheidmatrix van de opgegeven afmeting.",
		arguments: [
			{
				name: "afmeting",
				description: "is een geheel getal waarmee de afmeting wordt opgegeven van de eenheidmatrix die u wilt retourneren"
			}
		]
	},
	{
		name: "EFFECT.RENTE",
		description: "Berekent het jaarlijkse effectieve rentepercentage.",
		arguments: [
			{
				name: "nominale_rente",
				description: "is het nominale rentepercentage"
			},
			{
				name: "termijnen_per_jaar",
				description: "is het aantal termijnen per jaar waarover de samengestelde rente wordt berekend"
			}
		]
	},
	{
		name: "EN",
		description: "Controleert of alle argumenten WAAR zijn. Als dit het geval is, wordt als resultaat WAAR gegeven.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "logisch1",
				description: "zijn maximaal 255 voorwaarden waarvan u wilt testen of ze WAAR of ONWAAR zijn. De voorwaarden kunnen logische waarden, matrices of verwijzingen zijn"
			},
			{
				name: "logisch2",
				description: "zijn maximaal 255 voorwaarden waarvan u wilt testen of ze WAAR of ONWAAR zijn. De voorwaarden kunnen logische waarden, matrices of verwijzingen zijn"
			}
		]
	},
	{
		name: "EURO",
		description: "Converteert een getal naar tekst op basis van de valutanotatie.",
		arguments: [
			{
				name: "getal",
				description: "is een getal, een verwijzing naar een cel met een getal of een formule die resulteert in een getal"
			},
			{
				name: "decimalen",
				description: "is het aantal decimalen rechts van de decimale komma. Indien noodzakelijk wordt het getal afgerond. Als dit wordt weggelaten, wordt uitgegaan van Decimalen = 2"
			}
		]
	},
	{
		name: "EURO.BR",
		description: "Converteert een prijs in euro's, uitgedrukt in een decimaal getal, naar een prijs in euro's, uitgedrukt in een breuk.",
		arguments: [
			{
				name: "decimaal",
				description: "is een decimaal getal"
			},
			{
				name: "noemer",
				description: "is het gehele getal dat in de noemer van de breuk wordt gebruikt"
			}
		]
	},
	{
		name: "EURO.DE",
		description: "Converteert een prijs in euro's, uitgedrukt in een breuk, naar een prijs in euro's, uitgedrukt in een decimaal getal.",
		arguments: [
			{
				name: "breuk",
				description: "is een getal uitgedrukt in een breuk"
			},
			{
				name: "noemer",
				description: "is het gehele getal dat in de noemer van de breuk wordt gebruikt"
			}
		]
	},
	{
		name: "EVEN",
		description: "Rondt een positief getal naar boven af, en een negatief getal naar beneden, op het dichtstbijzijnde gehele even getal.",
		arguments: [
			{
				name: "getal",
				description: "is de waarde die moet worden afgerond"
			}
		]
	},
	{
		name: "EX.OF",
		description: "Geeft als resultaat een logische 'Exclusieve of' van alle argumenten.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "logisch1",
				description: "zijn maximaal 254 voorwaarden waarvan u wilt testen of ze WAAR of ONWAAR zijn en kunnen logische waarden, matrices of verwijzingen zijn"
			},
			{
				name: "logisch2",
				description: "zijn maximaal 254 voorwaarden waarvan u wilt testen of ze WAAR of ONWAAR zijn en kunnen logische waarden, matrices of verwijzingen zijn"
			}
		]
	},
	{
		name: "EXP",
		description: "Verheft e tot de macht van het gegeven getal.",
		arguments: [
			{
				name: "getal",
				description: "is de exponent die wordt toegepast op het grondtal e. De constante e is gelijk aan 2,71828182845904, de basis van de natuurlijke logaritme"
			}
		]
	},
	{
		name: "EXPON.VERD",
		description: "Geeft als resultaat de exponentiële verdeling.",
		arguments: [
			{
				name: "x",
				description: "is de waarde van de functie. Dit is een niet-negatief getal"
			},
			{
				name: "lambda",
				description: "is de waarde van de parameter. Dit is een positief getal"
			},
			{
				name: "cumulatief",
				description: "is een logische waarde die bepaalt welke functie als resultaat wordt gegeven: WAAR = de cumulatieve verdelingsfunctie, ONWAAR = de kansdichtheidsfunctie"
			}
		]
	},
	{
		name: "EXPON.VERD.N",
		description: "Geeft als resultaat de exponentiële verdeling.",
		arguments: [
			{
				name: "x",
				description: "is de waarde van de functie. Dit is een niet-negatief getal"
			},
			{
				name: "lambda",
				description: "is de waarde van de parameter. Dit is een positief getal"
			},
			{
				name: "cumulatief",
				description: "is een logische waarde die bepaalt welke functie als resultaat wordt gegeven: WAAR = de cumulatieve verdelingsfunctie, ONWAAR = de kansdichtheidsfunctie"
			}
		]
	},
	{
		name: "F.INV",
		description: "Berekent de inverse van de (linkszijdige) F-kansverdeling: als p = F.VERDELING(x,...), dan F.INVERSE(p,...) = x.",
		arguments: [
			{
				name: "kans",
				description: "is een kans die samenhangt met de cumulatieve F-verdeling. Dit is een getal tussen 0 en 1 inclusief"
			},
			{
				name: "vrijheidsgraden1",
				description: "is het aantal vrijheidsgraden van de teller. Dit is een getal tussen 1 en 10^10, exclusief 10^10"
			},
			{
				name: "vrijheidsgraden2",
				description: "is het aantal vrijheidsgraden van de noemer. Dit is een getal tussen 1 en 10^10, exclusief 10^10"
			}
		]
	},
	{
		name: "F.INV.RECHTS",
		description: "Berekent de inverse van de (rechtszijdige) F-kansverdeling: als p = F.VERD.RECHTS(x,...), dan F.INV.RECHTS(p,...) = x.",
		arguments: [
			{
				name: "kans",
				description: "is een kans die samenhangt met de cumulatieve F-verdeling. Dit is een getal tussen 0 en 1 inclusief"
			},
			{
				name: "vrijheidsgraden1",
				description: "is het aantal vrijheidsgraden van de teller. Dit is een getal tussen 1 en 10^10, exclusief 10^10"
			},
			{
				name: "vrijheidsgraden2",
				description: "is het aantal vrijheidsgraden van de noemer. Dit is een getal tussen 1 en 10^10, exclusief 10^10"
			}
		]
	},
	{
		name: "F.INVERSE",
		description: "Berekent de inverse van de (rechtszijdige) F-verdeling: als p = F.VERDELING(x,...), is F.INVERSE(p,...) = x.",
		arguments: [
			{
				name: "kans",
				description: "is de kans die samenhangt met de cumulatieve F-verdeling. Dit is een getal van 0 tot en met 1"
			},
			{
				name: "vrijheidsgraden1",
				description: "is het aantal vrijheidsgraden van de teller. Dit is een getal tussen 1 en 10^10, met 10^10 uitgesloten"
			},
			{
				name: "vrijheidsgraden2",
				description: "is het aantal vrijheidsgraden van de noemer. Dit is een getal tussen 1 en 10^10, met 10^10 uitgesloten"
			}
		]
	},
	{
		name: "F.TEST",
		description: "Geeft het resultaat van een F-toets, de tweezijdige kans dat de varianties in matrix1 en matrix2 niet significant verschillen.",
		arguments: [
			{
				name: "matrix1",
				description: "is de eerste matrix of het eerste gegevensbereik. Dit kunnen getallen zijn of namen, matrices of verwijzingen die getallen bevatten (lege cellen worden genegeerd)"
			},
			{
				name: "matrix2",
				description: "is de tweede matrix of het tweede gegevensbereik. Dit kunnen getallen zijn of namen, matrices of verwijzingen die getallen bevatten (lege cellen worden genegeerd)"
			}
		]
	},
	{
		name: "F.TOETS",
		description: "Geeft het resultaat van een F-toets, de tweezijdige kans dat de varianties in matrix1 en matrix2 niet significant verschillen.",
		arguments: [
			{
				name: "matrix1",
				description: "is de eerste matrix of het eerste gegevensbereik. Dit kunnen getallen zijn of namen, matrices of verwijzingen die getallen bevatten (lege cellen worden genegeerd)"
			},
			{
				name: "matrix2",
				description: "is de tweede matrix of het tweede gegevensbereik. Dit kunnen getallen zijn of namen, matrices of verwijzingen die getallen bevatten (lege cellen worden genegeerd)"
			}
		]
	},
	{
		name: "F.VERD",
		description: "Geeft als resultaat de (linkszijdige) F-kansverdeling (de graad van verscheidenheid) voor twee gegevenssets.",
		arguments: [
			{
				name: "x",
				description: "is de waarde waarvoor de functie moet worden geëvalueerd. Dit is een niet-negatief getal"
			},
			{
				name: "vrijheidsgraden1",
				description: "is het aantal vrijheidsgraden van de teller. Dit is een getal tussen 1 en 10^10, exclusief 10^10"
			},
			{
				name: "vrijheidsgraden2",
				description: "is het aantal vrijheidsgraden van de noemer. Dit is een getal tussen 1 en 10^10, exclusief 10^10"
			},
			{
				name: "cumulatief",
				description: "is een logische waarde die door de functie moet worden geretourneerd: de cumulatieve verdelingsfunctie = WAAR; de kansdichtheidsfunctie = ONWAAR"
			}
		]
	},
	{
		name: "F.VERD.RECHTS",
		description: "Geeft als resultaat de (rechtszijdige) F-kansverdeling (de graad van verscheidenheid) voor twee gegevenssets.",
		arguments: [
			{
				name: "x",
				description: "is de waarde waarvoor de functie moet worden geëvalueerd. Dit is een niet-negatief getal"
			},
			{
				name: "vrijheidsgraden1",
				description: "is het aantal vrijheidsgraden van de teller. Dit is een getal tussen 1 en 10^10, exclusief 10^10"
			},
			{
				name: "vrijheidsgraden2",
				description: "is het aantal vrijheidsgraden van de noemer. Dit is een getal tussen 1 en 10^10, exclusief 10^10"
			}
		]
	},
	{
		name: "F.VERDELING",
		description: "Geeft als resultaat de (rechtszijdige) F-verdeling (de graad van verscheidenheid) voor twee gegevensverzamelingen.",
		arguments: [
			{
				name: "x",
				description: "is de waarde waarop de functie geëvalueerd moet worden. Dit is een niet-negatief getal"
			},
			{
				name: "vrijheidsgraden1",
				description: "is het aantal vrijheidsgraden van de teller. Dit is een getal tussen 1 en 10^10, met 10^10 uitgesloten"
			},
			{
				name: "vrijheidsgraden2",
				description: "is het aantal vrijheidsgraden van de noemer. Dit is een getal tussen 1 en 10^10, met 10^10 uitgesloten"
			}
		]
	},
	{
		name: "FACULTEIT",
		description: "Berekent de faculteit van een getal. Dit is gelijk aan 1*2*3*...*Getal.",
		arguments: [
			{
				name: "getal",
				description: "is het niet-negatieve getal waarvan u de faculteit wilt bepalen"
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
		name: "FISHER",
		description: "Berekent de Fisher-transformatie.",
		arguments: [
			{
				name: "x",
				description: "is het getal waarvoor u de Fisher-transformatie wilt berekenen. Dit is een getal tussen -1 en 1, met -1 en 1 uitgesloten"
			}
		]
	},
	{
		name: "FISHER.INV",
		description: "Berekent de inverse van de Fisher-transformatie: als y=FISHER(x), is FISHER.INV(y) = x.",
		arguments: [
			{
				name: "y",
				description: "is de waarde waarop u de inverse van de transformatie wilt uitvoeren"
			}
		]
	},
	{
		name: "FORMULETEKST",
		description: "Geeft als resultaat een formule als tekenreeks.",
		arguments: [
			{
				name: "verwijzing",
				description: "is een verwijzing naar een formule"
			}
		]
	},
	{
		name: "FOUT.COMPLEMENT",
		description: "Geeft de bijbehorende foutfunctie.",
		arguments: [
			{
				name: "x",
				description: "is de ondergrens voor FOUTFUNCTIE"
			}
		]
	},
	{
		name: "FOUT.COMPLEMENT.NAUWKEURIG",
		description: "Geeft de bijbehorende foutfunctie als resultaat.",
		arguments: [
			{
				name: "X",
				description: "is de ondergrens voor FOUT.COMPLEMENT.NAUWKEURIG"
			}
		]
	},
	{
		name: "FOUTFUNCTIE",
		description: "Geeft de foutfunctie weer.",
		arguments: [
			{
				name: "ondergrens",
				description: "is de ondergrens voor FOUTFUNCTIE"
			},
			{
				name: "bovengrens",
				description: "is de bovengrens voor FOUTFUNCTIE"
			}
		]
	},
	{
		name: "FOUTFUNCTIE.NAUWKEURIG",
		description: "Geeft de foutfunctie als resultaat.",
		arguments: [
			{
				name: "X",
				description: "is de ondergrens voor FOUTFUNCTIE.NAUWKEURIG"
			}
		]
	},
	{
		name: "GAMMA",
		description: "Geeft als resultaat de waarde van de functie Gamma.",
		arguments: [
			{
				name: "x",
				description: "is de waarde waarvoor u Gamma wilt berekenen"
			}
		]
	},
	{
		name: "GAMMA.INV",
		description: "Berekent de inverse van de cumulatieve gamma-verdeling: als p = GAMMA.VERD(x,...), is GAMMA.INV(p,...) = x.",
		arguments: [
			{
				name: "kans",
				description: "is de kans die samenhangt met een gamma-verdeling. Dit is een getal tussen 0 en 1, met 0 en 1 ingesloten"
			},
			{
				name: "alfa",
				description: "is een parameter voor de verdeling. Dit is een positief getal"
			},
			{
				name: "bèta",
				description: "is een parameter voor de verdeling. Dit is een positief getal. Als bèta 1 is, geeft GAMMA.INV als resultaat de inverse van de standaard gamma-verdeling"
			}
		]
	},
	{
		name: "GAMMA.INV.N",
		description: "Berekent de inverse van de cumulatieve gamma-verdeling: als p = GAMMA.VERD.N(x,...), is GAMMA.INV.N(p,...) = x.",
		arguments: [
			{
				name: "kans",
				description: "is de kans die samenhangt met een gamma-verdeling. Dit is een getal tussen 0 en 1, met 0 en 1 ingesloten"
			},
			{
				name: "alfa",
				description: "is een parameter voor de verdeling. Dit is een positief getal"
			},
			{
				name: "beta",
				description: "is een parameter voor de verdeling. Dit is een positief getal. Als beta 1 is, geeft GAMMA.INV als resultaat de inverse van de standaard gamma-verdeling"
			}
		]
	},
	{
		name: "GAMMA.LN",
		description: "Berekent de natuurlijke logaritme van de gamma-functie.",
		arguments: [
			{
				name: "x",
				description: "is de waarde waarvoor u GAMMA.LN wilt berekenen. Dit is een positief getal"
			}
		]
	},
	{
		name: "GAMMA.LN.NAUWKEURIG",
		description: "Berekent de natuurlijke logaritme van de gammafunctie.",
		arguments: [
			{
				name: "x",
				description: "is de waarde waarvoor u GAMMA.LN.NAUWKEURIG wilt berekenen. Dit is een positief getal"
			}
		]
	},
	{
		name: "GAMMA.VERD",
		description: "Geeft als resultaat de gamma-verdeling.",
		arguments: [
			{
				name: "x",
				description: "is de waarde waarop u de verdeling wilt evalueren. Dit is een niet-negatief getal"
			},
			{
				name: "alfa",
				description: "is een parameter voor de verdeling. Dit is een positief getal"
			},
			{
				name: "bèta",
				description: "is een parameter voor de verdeling. Dit is een positief getal. Als bèta 1 is, geeft GAMMA.VERD als resultaat de standaard-gamma-verdeling"
			},
			{
				name: "cumulatief",
				description: "is een logische waarde. Gebruik WAAR voor de cumulatieve verdelingsfunctie en ONWAAR voor de kansdichtheidsfunctie"
			}
		]
	},
	{
		name: "GAMMA.VERD.N",
		description: "Geeft als resultaat de gamma-verdeling.",
		arguments: [
			{
				name: "x",
				description: "is de waarde waarop u de verdeling wilt evalueren. Dit is een niet-negatief getal"
			},
			{
				name: "alfa",
				description: "is een parameter voor de verdeling. Dit is een positief getal"
			},
			{
				name: "beta",
				description: "is een parameter voor de verdeling. Dit is een positief getal. Als beta 1 is, geeft GAMMA.VERD.N als resultaat de standaard-gamma-verdeling"
			},
			{
				name: "cumulatief",
				description: "is een logische waarde. Gebruik WAAR voor de cumulatieve verdelingsfunctie en ONWAAR voor de kansdichtheidsfunctie"
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
		name: "GEHEEL",
		description: "Kapt een getal af tot een geheel getal door het decimale (of gebroken) gedeelte van het getal te verwijderen.",
		arguments: [
			{
				name: "getal",
				description: "is het getal dat u wilt afkappen"
			},
			{
				name: "aantal-decimalen",
				description: "is een getal dat de precisie van de afkapping aangeeft. Als dit wordt weggelaten, wordt uitgegaan van 0 (nul)"
			}
		]
	},
	{
		name: "GELIJK",
		description: "Controleert of twee tekenreeksen identiek zijn en geeft als resultaat WAAR of ONWAAR. Er wordt verschil gemaakt tussen hoofdletters en kleine letters.",
		arguments: [
			{
				name: "tekst1",
				description: "is de eerste tekenreeks"
			},
			{
				name: "tekst2",
				description: "is de tweede tekenreeks"
			}
		]
	},
	{
		name: "GEM.DEVIATIE",
		description: "Berekent het gemiddelde van de absolute deviaties van gegevenspunten ten opzichte van hun gemiddelde waarde. De argumenten kunnen getallen zijn of namen, matrices of verwijzingen die getallen bevatten.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "getal1",
				description: "zijn maximaal 255 argumenten waarvoor u het gemiddelde van de absolute deviatie wilt bepalen"
			},
			{
				name: "getal2",
				description: "zijn maximaal 255 argumenten waarvoor u het gemiddelde van de absolute deviatie wilt bepalen"
			}
		]
	},
	{
		name: "GEMIDDELDE",
		description: "Berekent het (rekenkundig) gemiddelde van de argumenten. De argumenten kunnen getallen zijn of namen, matrices of verwijzingen die getallen bevatten.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "getal1",
				description: "zijn maximaal 255 argumenten waarvan u het gemiddelde wilt berekenen"
			},
			{
				name: "getal2",
				description: "zijn maximaal 255 argumenten waarvan u het gemiddelde wilt berekenen"
			}
		]
	},
	{
		name: "GEMIDDELDE.ALS",
		description: "Zoekt het (rekenkundige) gemiddelde voor de cellen die worden gespecificeerd door een gegeven voorwaarde of criterium.",
		arguments: [
			{
				name: "bereik",
				description: "is het celbereik dat u wilt evalueren"
			},
			{
				name: "criteria",
				description: "is de voorwaarde of het criterium in de vorm van een getal, expressie of tekst waarmee wordt aangegeven welke cellen worden gebruikt om het gemiddelde te zoeken"
			},
			{
				name: "gemiddelde_bereik",
				description: "zijn de feitelijke cellen die moeten worden gebruikt om het gemiddelde te bepalen. Indien weggelaten worden alle cellen in het bereik gebruikt "
			}
		]
	},
	{
		name: "GEMIDDELDEA",
		description: "Berekent het (meetkundige) gemiddelde van de argumenten. Tekst en ONWAAR krijgen de waarde 0, WAAR krijgt de waarde 1. Argumenten kunnen getallen, namen, matrices en verwijzingen zijn.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "waarde1",
				description: "zijn 1 tot 255 argumenten waarvan u het gemiddelde wilt bepalen"
			},
			{
				name: "waarde2",
				description: "zijn 1 tot 255 argumenten waarvan u het gemiddelde wilt bepalen"
			}
		]
	},
	{
		name: "GEMIDDELDEN.ALS",
		description: "Zoekt het (rekenkundige) gemiddelde voor de cellen die worden gespecificeerd door een gegeven set voorwaarden of criteria.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "gemiddelde_bereik",
				description: "is het werkelijke aantal cellen dat wordt gebruikt om het gemiddelde te zoeken"
			},
			{
				name: "criteriumbereik",
				description: "is het celbereik dat u wilt evalueren voor de voorwaarde in kwestie"
			},
			{
				name: "criteria",
				description: "is de voorwaarde of het criterium in de vorm van een getal, expressie of tekst waarmee wordt aangegeven welke cellen worden gebruikt om het gemiddelde te zoeken"
			}
		]
	},
	{
		name: "GETRIMD.GEM",
		description: "Berekent het gemiddelde van waarden in een gegevensverzameling, waarbij de extreme waarden in de berekening worden uitgesloten.",
		arguments: [
			{
				name: "matrix",
				description: "is de matrix of het bereik met waarden waaruit u extreme waarden wilt verwijderen om vervolgens het gemiddelde te berekenen"
			},
			{
				name: "percentage",
				description: "is het percentage van de gegevenspunten dat aan de boven- en onderzijde van de gegevensverzameling uitgesloten moet worden"
			}
		]
	},
	{
		name: "GGD",
		description: "Geeft als resultaat de grootste algemene deler.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "complex_getal1",
				description: "zijn 1 tot 255 waarden"
			},
			{
				name: "complex_getal2",
				description: "zijn 1 tot 255 waarden"
			}
		]
	},
	{
		name: "GIR",
		description: "Berekent de interne rentabiliteit voor een serie periodieke cashflows, rekening houdend met zowel beleggingskosten als de rente op het herinvesteren van geld.",
		arguments: [
			{
				name: "waarden",
				description: "is een matrix of verwijzing naar cellen die getallen bevatten die overeenkomen met een reeks betalingen (negatief) en inkomsten (positief) met regelmatige tussenperioden"
			},
			{
				name: "financieringsrente",
				description: "is het rentepercentage dat u betaalt over het in de cashflows gebruikte bedrag"
			},
			{
				name: "herinvesteringsrente",
				description: "is het rentepercentage dat u ontvangt door de cashflows te herinvesteren"
			}
		]
	},
	{
		name: "GRADEN",
		description: "Converteert radialen naar graden.",
		arguments: [
			{
				name: "hoek",
				description: "is de hoek in radialen die u wilt converteren"
			}
		]
	},
	{
		name: "GROEI",
		description: "Geeft als resultaat getallen in een exponentiële groeitrend die overeenkomen met bekende gegevenspunten.",
		arguments: [
			{
				name: "y-bekend",
				description: "is de verzameling Y-waarden die al bekend zijn uit y = b*m^x. Dit is een matrix of bereik met positieve getallen"
			},
			{
				name: "x-bekend",
				description: "is een optionele verzameling X-waarden die wellicht al bekend zijn uit y = b*m^x. Dit is een matrix of bereik met dezelfde grootte als y-bekend"
			},
			{
				name: "x-nieuw",
				description: "zijn de nieuwe X-waarden waarvoor GROEI als resultaat de bijbehorende Y-waarden moet geven"
			},
			{
				name: "const",
				description: "is een logische waarde. Als Const = WAAR, wordt de constante b normaal berekend. Als Const = ONWAAR of wordt weggelaten, moet b gelijk zijn aan 1"
			}
		]
	},
	{
		name: "GROOTSTE",
		description: "Berekent de op k-1 na grootste waarde in een gegevensbereik, bijvoorbeeld het vijfde grootste getal.",
		arguments: [
			{
				name: "matrix",
				description: "is de matrix of het gegevensbereik waarvoor u de op k-1 na grootste waarde wilt bepalen"
			},
			{
				name: "k",
				description: "is de positie (vanaf grootste waarde geteld) in de matrix die of het celbereik met gegevens dat als resultaat moet worden gegeven"
			}
		]
	},
	{
		name: "GROTER.DAN",
		description: "Toetst of een getal groter is dan de drempelwaarde.",
		arguments: [
			{
				name: "getal",
				description: "is de waarde die u wilt vergelijken met drempelwaarde"
			},
			{
				name: "drempelwaarde",
				description: "is de drempelwaarde"
			}
		]
	},
	{
		name: "HARM.GEM",
		description: "Berekent het harmonische gemiddelde van een gegevensverzameling met positieve getallen: de reciproque waarde van het meetkundige gemiddelde van reciproque waarden.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "getal1",
				description: "zijn 1 tot 255 getallen of namen, matrices of verwijzingen die getallen bevatten waarvan u het harmonische gemiddelde wilt berekenen"
			},
			{
				name: "getal2",
				description: "zijn 1 tot 255 getallen of namen, matrices of verwijzingen die getallen bevatten waarvan u het harmonische gemiddelde wilt berekenen"
			}
		]
	},
	{
		name: "HERHALING",
		description: "Herhaalt een tekst een aantal malen. Gebruik HERHALING om een cel een aantal keren te vullen met een tekenreeks.",
		arguments: [
			{
				name: "tekst",
				description: "is de tekst die u wilt herhalen"
			},
			{
				name: "aantal-malen",
				description: "is een positief getal dat aangeeft hoe vaak u een tekst wilt herhalen"
			}
		]
	},
	{
		name: "HEX.N.BIN",
		description: "Converteert een hexadecimaal getal naar een binair getal.",
		arguments: [
			{
				name: "getal",
				description: "is het hexadecimale getal dat u wilt converteren"
			},
			{
				name: "aantal_tekens",
				description: "is het aantal tekens dat u wilt gebruiken"
			}
		]
	},
	{
		name: "HEX.N.DEC",
		description: "Converteert een hexadecimaal getal naar een decimaal getal.",
		arguments: [
			{
				name: "getal",
				description: "is het hexadecimale getal dat u wilt converteren"
			}
		]
	},
	{
		name: "HEX.N.OCT",
		description: "Converteert een hexadecimaal getal naar een octaal getal.",
		arguments: [
			{
				name: "getal",
				description: "is het hexadecimale getal dat u wilt omzetten in een octaal getal"
			},
			{
				name: "aantal_tekens",
				description: "is het aantal tekens dat u wilt gebruiken"
			}
		]
	},
	{
		name: "HOOFDLETTERS",
		description: "Zet een tekenreeks om in hoofdletters.",
		arguments: [
			{
				name: "tekst",
				description: "is de tekst die u wilt omzetten in hoofdletters. Dit is een verwijzing of een tekenreeks"
			}
		]
	},
	{
		name: "HORIZ.ZOEKEN",
		description: "Zoekt in de bovenste rij van een tabel of matrix met waarden naar waarden en geeft als resultaat de waarde in dezelfde kolom uit een opgegeven kolom.",
		arguments: [
			{
				name: "zoekwaarde",
				description: "is de waarde in de eerste rij van de tabel en kan een waarde, een verwijzing, of een tekenreeks zijn"
			},
			{
				name: "tabelmatrix",
				description: "is een tabel met tekst,getallen, of logische waarden waarin u naar gegevens zoekt. Tabelmatrix kan een referentie zijn naar een bereik of de naam van een bereik"
			},
			{
				name: "rij-index_getal",
				description: "is het rijnummer in de opgegeven tabelmatrix waaruit u de overeenkomende waarde wilt halen. De eerste waarderij in de tabel is rij 1"
			},
			{
				name: "bereik",
				description: "is een logische waarde: voor de beste overeenkomst in de bovenste rij (in oplopende volgorde gesorteerd) = WAAR of weggelaten; voor een exacte overeenkomst = ONWAAR"
			}
		]
	},
	{
		name: "HW",
		description: "Berekent de huidige waarde van een investering: het totale bedrag dat een reeks toekomstige betalingen momenteel waard is.",
		arguments: [
			{
				name: "rente",
				description: "is het rentepercentage per termijn. Gebruik bijvoorbeeld 6%/4 voor kwartaalbetalingen met een rentepercentage van 6%"
			},
			{
				name: "aantal-termijnen",
				description: "is het totale aantal betalingstermijnen van een investering"
			},
			{
				name: "bet",
				description: "is de betaling die iedere termijn wordt verricht. Dit bedrag kan gedurende de looptijd van de investering niet worden gewijzigd"
			},
			{
				name: "tw",
				description: "is de toekomstige waarde of het kassaldo waarover u wilt beschikken als de laatste betaling is gedaan"
			},
			{
				name: "type_getal",
				description: "is een logische waarde: 1 = betaling aan het begin van de periode, 0 of weggelaten = betaling aan het einde van de periode"
			}
		]
	},
	{
		name: "HYPERGEO.VERD",
		description: "Geeft als resultaat de hypergeometrische verdeling.",
		arguments: [
			{
				name: "steekproef-gunstig",
				description: "Is het aantal gunstige uitkomsten in de steekproef"
			},
			{
				name: "grootte-steekproef",
				description: "Is de grootte van de steekproef"
			},
			{
				name: "populatie-gunstig",
				description: "Is het aantal gunstige uitkomsten in de populatie"
			},
			{
				name: "grootte-populatie",
				description: "Is de grootte van de populatie"
			}
		]
	},
	{
		name: "HYPERLINK",
		description: "Maakt een snelkoppeling of sprong die een document opent dat is opgeslagen op de harde schijf, op een netwerkserver of op internet.",
		arguments: [
			{
				name: "locatie_link",
				description: "is de tekst met het volledige pad en de volledige bestandsnaam van het bestand dat moet worden geopend, de locatie van een harde schijf, een UNC-adres of een URL-pad"
			},
			{
				name: "makkelijke_naam",
				description: "is het getal, de tekst of de functie die in de cel wordt weergegeven. Als dit wordt weggelaten, geeft de cel de tekst van de Locatie_koppeling weer"
			}
		]
	},
	{
		name: "HYPGEOM.VERD",
		description: "Geeft als resultaat de hypergeometrische verdeling.",
		arguments: [
			{
				name: "steekproef-gunstig",
				description: "is het aantal gunstige uitkomsten in de steekproef"
			},
			{
				name: "grootte-steekproef",
				description: "is de grootte van de steekproef"
			},
			{
				name: "populatie-gunstig",
				description: "is het aantal gunstige uitkomsten in de populatie"
			},
			{
				name: "grootte-populatie",
				description: "is de grootte van de populatie"
			},
			{
				name: "cumulatief",
				description: "is een logische waarde: gebruik WAAR voor de cumulatieve verdelingsfunctie en gebruik ONWAAR voor de kansdichtheidsfunctie"
			}
		]
	},
	{
		name: "IBET",
		description: "Berekent de te betalen rente voor een investering over een bepaalde termijn, op basis van periodieke, constante betalingen en een constant rentepercentage.",
		arguments: [
			{
				name: "rente",
				description: "is het rentepercentage per termijn. Gebruik bijvoorbeeld 6%/4 voor kwartaalbetalingen met een rentepercentage van 6%"
			},
			{
				name: "termijn",
				description: "is de termijn waarover u het rentepercentage wilt berekenen. Dit argument moet liggen tussen 1 en aantal-termijnen"
			},
			{
				name: "aantal-termijnen",
				description: "is het totale aantal betalingstermijnen van een investering"
			},
			{
				name: "hw",
				description: "is de huidige waarde of het totaalbedrag dat een reeks toekomstige waarden op dit moment waard is"
			},
			{
				name: "tw",
				description: "is de toekomstige waarde of het kassaldo waarover u wilt beschikken als de laatste betaling is gedaan. Als dit wordt weggelaten, wordt uitgegaan van TW = 0"
			},
			{
				name: "type_getal",
				description: "is een logische waarde die aangeeft wanneer de betalingen voldaan moeten worden: 0 of weggelaten = aan het begin van de periode, 1 = aan het einde van de periode"
			}
		]
	},
	{
		name: "INDEX",
		description: "Geeft als resultaat een waarde of verwijzing van de cel op het snijpunt van een bepaalde rij en kolom in een opgegeven bereik.",
		arguments: [
			{
				name: "matrix",
				description: "is een celbereik of een matrixconstante."
			},
			{
				name: "rij_getal",
				description: "selecteert de rij in de opgegeven matrix waaruit een waarde moet worden opgehaald. Als dit wordt weggelaten, moet Kolom_getal worden opgegeven"
			},
			{
				name: "kolom_getal",
				description: "selecteert de kolom in de opgegeven matrix waaruit u een waarde wilt ophalen. Als dit wordt weggelaten, moet Rij_getal worden opgegeven"
			}
		]
	},
	{
		name: "INDIRECT",
		description: "Geeft als resultaat een verwijzing aangegeven door een tekstwaarde.",
		arguments: [
			{
				name: "verw_tekst",
				description: "is een verwijzing naar een cel die een A1- of een R1K1-verwijzing bevat, een naam die is gedefinieerd als een verwijzing of een verwijzing naar een cel in de vorm van een tekenreeks"
			},
			{
				name: "A1",
				description: "is een logische waarde die aangeeft welk type verwijzing de cel in verw_tekst bevat: ONWAAR = R1K1, WAAR of weggelaten = A1"
			}
		]
	},
	{
		name: "INFO",
		description: "Geeft informatie over de huidige besturingsomgeving.",
		arguments: [
			{
				name: "type_tekst",
				description: "is tekst die aangeeft welk type informatie u als resultaat wilt hebben."
			}
		]
	},
	{
		name: "INTEGER",
		description: "Rondt een getal naar beneden af op het dichtstbijzijnde gehele getal.",
		arguments: [
			{
				name: "getal",
				description: "is het reële getal dat u naar beneden wilt afronden op een geheel getal"
			}
		]
	},
	{
		name: "INTERVAL",
		description: "Berekent hoe vaak waarden voorkomen in een waardebereik en geeft als resultaat een verticale matrix met getallen met een element meer dan interval_verw.",
		arguments: [
			{
				name: "gegevensmatrix",
				description: "is een matrix of een verwijzing naar een verzameling waarden waarvan u de frequentie wilt berekenen (lege cellen en tekst worden genegeerd)"
			},
			{
				name: "interval_verw",
				description: "is een matrix of een verwijzing naar de intervallen waarin u de waarden uit de gegevensmatrix wilt verdelen"
			}
		]
	},
	{
		name: "INVERSEMAT",
		description: "Berekent de inverse van een matrix die is opgeslagen in een matrix.",
		arguments: [
			{
				name: "matrix",
				description: "is een numerieke matrix met een gelijk aantal rijen en kolommen. Dit is een celbereik of een matrixconstante"
			}
		]
	},
	{
		name: "IR",
		description: "Berekent de interne rentabiliteit voor een reeks cashflows.",
		arguments: [
			{
				name: "waarden",
				description: "is een matrix of verwijzing naar cellen die getallen bevatten waarvoor u de interne rentabiliteit wilt berekenen"
			},
			{
				name: "schatting",
				description: "is een getal waarvan u denkt dat het in de buurt komt van het resultaat van IR. Als dit wordt weggelaten, wordt uitgegaan van 0,1 (10 procent)"
			}
		]
	},
	{
		name: "IR.SCHEMA",
		description: "Berekent de interne rentabiliteit voor een geplande serie van cashflows.",
		arguments: [
			{
				name: "waarden",
				description: "is een reeks cashflows die correspondeert met een geplande serie betalingsdatums"
			},
			{
				name: "datums",
				description: "is een schema van betalingsdatums die correspondeert met de geplande cashflowbetalingen"
			},
			{
				name: "schatting",
				description: "is een getal waarvan u denkt dat het in de buurt komt van het resultaat van IR2"
			}
		]
	},
	{
		name: "IS.EVEN",
		description: "Resulteert in WAAR als het getal even is.",
		arguments: [
			{
				name: "getal",
				description: "is de waarde die u wilt testen"
			}
		]
	},
	{
		name: "IS.ONEVEN",
		description: "Resulteert in WAAR als het getal oneven is.",
		arguments: [
			{
				name: "getal",
				description: "is de waarde die u wilt testen"
			}
		]
	},
	{
		name: "ISBET",
		description: "Berekent de betaalde rente voor een bepaalde termijn van een investering.",
		arguments: [
			{
				name: "rente",
				description: "Is het rentepercentage per termijn. Gebruik bijvoorbeeld 6%/4 voor kwartaalbetalingen met een rentepercentage van 6%"
			},
			{
				name: "termijn",
				description: "is de termijn waarvoor u de rente wilt berekenen"
			},
			{
				name: "aantal-termijnen",
				description: "is het aantal betalingstermijnen van een investering"
			},
			{
				name: "hw",
				description: "is de huidige waarde of het totaalbedrag dat een reeks toekomstige waarden op dit moment waard is"
			}
		]
	},
	{
		name: "ISFORMULE",
		description: "Controleert of een verwijzing verwijst naar een cel die een formule bevat en geeft WAAR of ONWAAR als resultaat.",
		arguments: [
			{
				name: "verwijzing",
				description: "is een verwijzing naar de cel die u wilt testen. Verwijzing kan een celverwijzing, formule of naam zijn die naar een cel verwijst"
			}
		]
	},
	{
		name: "ISFOUT",
		description: "Controleert of een waarde een fout is (#N/B, #WAARDE!, #VERW!, #DEEL/0!, #GETAL!, #NAAM? of #LEEG!) en geeft als resultaat WAAR of ONWAAR.",
		arguments: [
			{
				name: "waarde",
				description: "is de waarde die u wilt testen. Waarde kan verwijzen naar een cel, een formule of een naam die verwijst naar een cel, formule of waarde"
			}
		]
	},
	{
		name: "ISFOUT2",
		description: "Controleert of een waarde een fout is (#WAARDE!, #VERW!, #DEEL/0!, #GETAL!, #NAAM? of #LEEG!), behalve #N/B, en geeft als resultaat WAAR of ONWAAR.",
		arguments: [
			{
				name: "waarde",
				description: "is de waarde die u wilt testen. Waarde kan verwijzen naar een cel, een formule of een naam die verwijst naar een cel, formule of waarde"
			}
		]
	},
	{
		name: "ISGEENTEKST",
		description: "Controleert of een waarde geen tekst is (lege cellen zijn geen tekst), en geeft WAAR of ONWAAR als resultaat.",
		arguments: [
			{
				name: "waarde",
				description: "is de waarde die u wilt testen: een cel, een formule of een naam die verwijst naar een cel, formule of waarde"
			}
		]
	},
	{
		name: "ISGETAL",
		description: "Controleert of een waarde een getal is en geeft als resultaat WAAR of ONWAAR.",
		arguments: [
			{
				name: "waarde",
				description: "is de waarde die u wilt testen. Waarde kan verwijzen naar een cel, een formule of een naam die verwijst naar een cel, formule of waarde"
			}
		]
	},
	{
		name: "ISLEEG",
		description: "Controleert of een verwijzing naar een lege cel verwijst en geeft als resultaat WAAR of ONWAAR.",
		arguments: [
			{
				name: "waarde",
				description: "is de cel of de naam die verwijst naar de cel die u wilt testen"
			}
		]
	},
	{
		name: "ISLOGISCH",
		description: "Controleert of een waarde een logische waarde is (WAAR of ONWAAR), en geeft vervolgens WAAR of ONWAAR als resultaat.",
		arguments: [
			{
				name: "waarde",
				description: "is de waarde die u wilt testen. Waarde kan verwijzen naar een cel, een formule of een naam die verwijst naar een cel, formule of waarde"
			}
		]
	},
	{
		name: "ISNB",
		description: "Controleert of een waarde #N/B is en geeft als resultaat WAAR of ONWAAR.",
		arguments: [
			{
				name: "waarde",
				description: "is de waarde die u wilt testen. Waarde kan verwijzen naar een cel, een formule of een naam die verwijst naar een cel, formule of waarde"
			}
		]
	},
	{
		name: "ISO.AFRONDEN.BOVEN",
		description: "Rondt een getal naar boven af op het dichtstbijzijnde gehele getal of het dichtstbijzijnde significante veelvoud.",
		arguments: [
			{
				name: "getal",
				description: "is de waarde die u wilt afronden"
			},
			{
				name: "significantie",
				description: "is het optionele veelvoud waarop u wilt afronden"
			}
		]
	},
	{
		name: "ISO.WEEKNUMMER",
		description: "Geeft als resultaat het ISO-weeknummer van het jaar voor een bepaalde datum.",
		arguments: [
			{
				name: "datum",
				description: "is de datum-tijdcode die door Spreadsheet wordt gebruikt voor de berekening van de datum en tijd"
			}
		]
	},
	{
		name: "ISTEKST",
		description: "Controleert of een waarde tekst is en geeft als resultaat WAAR of ONWAAR.",
		arguments: [
			{
				name: "waarde",
				description: "is de waarde die u wilt testen. Waarde kan verwijzen naar een cel, een formule of een naam die verwijst naar een cel, formule of waarde"
			}
		]
	},
	{
		name: "ISVERWIJZING",
		description: "Controleert of een waarde een verwijzing is en geeft als resultaat WAAR of ONWAAR.",
		arguments: [
			{
				name: "waarde",
				description: "is de waarde die u wilt testen. Waarde kan verwijzen naar een cel, een formule of een naam die verwijst naar een cel, formule of waarde"
			}
		]
	},
	{
		name: "JAAR",
		description: "Geeft als resultaat het jaar van een datum, een geheel getal in het bereik 1900 - 9999.",
		arguments: [
			{
				name: "serieel-getal",
				description: "is een getal in het systeem dat in Spreadsheet wordt gebruikt voor datumberekeningen"
			}
		]
	},
	{
		name: "JAAR.DEEL",
		description: "Berekent het gedeelte van het jaar, uitgedrukt in het aantal dagen tussen begindatum en einddatum.",
		arguments: [
			{
				name: "begindatum",
				description: "is een serieel getal dat de begindatum aangeeft"
			},
			{
				name: "einddatum",
				description: "is een serieel getal dat de einddatum aangeeft"
			},
			{
				name: "soort_jaar",
				description: "is het soort jaar waarop de berekening is gebaseerd"
			}
		]
	},
	{
		name: "KANS",
		description: "Berekent de kans dat waarden zich tussen twee grenzen bevinden of gelijk zijn aan een onderlimiet.",
		arguments: [
			{
				name: "x-bereik",
				description: "is het bereik met de numerieke waarden voor x waarvoor bijbehorende kansen bestaan"
			},
			{
				name: "kansbereik",
				description: "is een verzameling kansen die betrekking hebben op de waarden in het X-bereik, waarbij de waarden tussen 0 en 1 liggen en niet gelijk zijn aan 0"
			},
			{
				name: "ondergrens",
				description: "is de ondergrens voor een waarde waarvan u een kans wilt vaststellen"
			},
			{
				name: "bovengrens",
				description: "is de optionele bovengrens voor een waarde waarvan u een kans wilt vaststellen. Zonder deze parameter geeft KANS als resultaat de kans dat de waarden in het X-bereik gelijk zijn aan Ondergrens"
			}
		]
	},
	{
		name: "KGV",
		description: "Berekent het kleinste gemene veelvoud.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "complex_getal1",
				description: "zijn 1 tot 255 waarden waarvoor u het kleinste gemene veelvoud wilt berekenen"
			},
			{
				name: "complex_getal2",
				description: "zijn 1 tot 255 waarden waarvoor u het kleinste gemene veelvoud wilt berekenen"
			}
		]
	},
	{
		name: "KIEZEN",
		description: "Kiest een waarde uit de lijst met waarden op basis van een indexnummer.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "index_getal",
				description: "geeft aan welke waarde geselecteerd is. Index_getal moet een getal zijn tussen 1 en 254 of een formule of een verwijzing naar een getal tussen 1 en 254"
			},
			{
				name: "waarde1",
				description: "zijn maximaal 254 getallen, celverwijzingen, gedefinieerde namen, formules, functies of tekst waaruit de functie KIEZEN kiest"
			},
			{
				name: "waarde2",
				description: "zijn maximaal 254 getallen, celverwijzingen, gedefinieerde namen, formules, functies of tekst waaruit de functie KIEZEN kiest"
			}
		]
	},
	{
		name: "KLEINE.LETTERS",
		description: "Zet alle letters in een tekenreeks om in kleine letters.",
		arguments: [
			{
				name: "tekst",
				description: "is de tekst die u wilt omzetten in kleine letters. Tekens in Tekst die geen letters zijn, worden niet gewijzigd"
			}
		]
	},
	{
		name: "KLEINSTE",
		description: "Geeft als resultaat de op k-1 na kleinste waarde in een gegevensbereik, bijvoorbeeld het vijfde kleinste getal.",
		arguments: [
			{
				name: "matrix",
				description: "is de matrix of het bereik met numerieke gegevens waarvoor u de op k-1 na kleinste waarde wilt bepalen"
			},
			{
				name: "k",
				description: "is de positie (vanaf de kleinste waarde geteld) in de resulterende matrix of het resulterende celbereik met gegevens"
			}
		]
	},
	{
		name: "KOLOM",
		description: "Geeft als resultaat het kolomnummer van een verwijzing.",
		arguments: [
			{
				name: "verw",
				description: "is de cel of de reeks aaneengesloten cellen waarvan u het kolomnummer wilt weten. Als dit wordt weggelaten, wordt de cel gebruikt die de functie KOLOM bevat"
			}
		]
	},
	{
		name: "KOLOMMEN",
		description: "Geeft als resultaat het aantal kolommen in een matrix of een verwijzing.",
		arguments: [
			{
				name: "matrix",
				description: "is een matrix, een matrixformule of een verwijzing naar een celbereik waarvan u het aantal kolommen wilt weten"
			}
		]
	},
	{
		name: "KURTOSIS",
		description: "Berekent de kurtosis van een gegevensverzameling.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "getal1",
				description: "zijn 1 tot 255 getallen of namen, matrices of verwijzingen die getallen bevatten waarvoor u de kurtosis wilt berekenen"
			},
			{
				name: "getal2",
				description: "zijn 1 tot 255 getallen of namen, matrices of verwijzingen die getallen bevatten waarvoor u de kurtosis wilt berekenen"
			}
		]
	},
	{
		name: "KWADRATENSOM",
		description: "Berekent de som van de kwadraten van de argumenten. Dit kunnen getallen zijn of namen, matrices of verwijzingen naar cellen die getallen bevatten.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "getal1",
				description: "zijn 1 tot 255 getallen, matrices, namen of matrixverwijzingen waarvan u de som van de kwadraten wilt bepalen"
			},
			{
				name: "getal2",
				description: "zijn 1 tot 255 getallen, matrices, namen of matrixverwijzingen waarvan u de som van de kwadraten wilt bepalen"
			}
		]
	},
	{
		name: "KWARTIEL",
		description: "Berekent het kwartiel van een gegevensverzameling.",
		arguments: [
			{
				name: "matrix",
				description: "is de matrix of het celbereik met de numerieke waarden waarvoor u het kwartiel wilt berekenen"
			},
			{
				name: "kwartiel",
				description: "is een getal: minimumwaarde = 0; 1e kwartiel = 1; middenwaarde = 2; 3e kwartiel - 3; maximumwaarde = 4"
			}
		]
	},
	{
		name: "KWARTIEL.EXC",
		description: "Bepaalt het kwartiel van een gegevensset op basis van percentiele waarden van 0..1, exclusief.",
		arguments: [
			{
				name: "matrix",
				description: "is de matrix of het celbereik met numerieke waarden waarvoor u het kwartiel wilt weten"
			},
			{
				name: "kwartiel",
				description: "is een getal: minimale waarde = 0; eerste kwartiel = 1; gemiddelde waarde = 2; derde kwartiel = 3; maximale waarde = 4"
			}
		]
	},
	{
		name: "KWARTIEL.INC",
		description: "Bepaalt het kwartiel van een gegevensset op basis van percentiele waarden van 0..1, inclusief.",
		arguments: [
			{
				name: "matrix",
				description: "is de matrix of het celbereik met numerieke waarden waarvoor u het kwartiel wilt weten"
			},
			{
				name: "kwartiel",
				description: "is een getal: minimale waarde = 0; eerste kwartiel = 1; gemiddelde waarde = 2; derde kwartiel = 3; maximale waarde = 4"
			}
		]
	},
	{
		name: "LAATSTE.DAG",
		description: "Zet de laatste dag van de maand die een opgegeven aantal maanden voor of na de begindatum ligt, om in een serieel getal.",
		arguments: [
			{
				name: "begindatum",
				description: "is een serieel getal dat de begindatum aangeeft"
			},
			{
				name: "aantal_maanden",
				description: "is het aantal maanden voor of na de begindatum"
			}
		]
	},
	{
		name: "LENGTE",
		description: "Geeft als resultaat het aantal tekens in een tekenreeks.",
		arguments: [
			{
				name: "tekst",
				description: "is de tekst waarvan u de lengte wilt bepalen. Spaties tellen mee als tekens"
			}
		]
	},
	{
		name: "LIJNSCH",
		description: "Geeft als resultaat statistieken die een lineaire trend beschrijven en overeenkomen met bekende gegevenspunten. De lijn wordt berekend met de kleinste-kwadratenmethode.",
		arguments: [
			{
				name: "y-bekend",
				description: "is de verzameling Y-waarden die u al bekend is uit y = mx + b"
			},
			{
				name: "x-bekend",
				description: "is een optionele verzameling van X-waarden die wellicht al bekend is uit y = mx + b"
			},
			{
				name: "const",
				description: "is een logische waarde. Als Const = WAAR of wordt weggelaten, wordt de constante b normaal berekend. Als Const = ONWAAR, moet b gelijk zijn aan 0"
			},
			{
				name: "stat",
				description: "is een logische waarde: WAAR = geef als resultaat aanvullende regressiegrootheden, ONWAAR of weggelaten = geef als resultaat m-coëfficiënten en de constante b"
			}
		]
	},
	{
		name: "LIN.AFSCHR",
		description: "Berekent de lineaire afschrijving van activa over één bepaalde termijn.",
		arguments: [
			{
				name: "kosten",
				description: "zijn de aanschafkosten van de activa"
			},
			{
				name: "restwaarde",
				description: "is de resterende waarde van de activa aan het einde van de levensduur van de activa"
			},
			{
				name: "duur",
				description: "is het aantal termijnen waarover de activa worden afgeschreven (ook wel levensduur van de activa genoemd)"
			}
		]
	},
	{
		name: "LINKS",
		description: "Geeft als resultaat het aantal tekens vanaf het begin van een tekenreeks.",
		arguments: [
			{
				name: "tekst",
				description: "is de tekenreeks met de tekens die u wilt ophalen"
			},
			{
				name: "aantal-tekens",
				description: "geeft aan hoeveel tekens LINKS moet ophalen. Als dit wordt weggelaten, wordt uitgegaan van 1"
			}
		]
	},
	{
		name: "LN",
		description: "Berekent de natuurlijke logaritme van een getal.",
		arguments: [
			{
				name: "getal",
				description: "is het positieve reële getal waarvan u de natuurlijke logaritme wilt berekenen"
			}
		]
	},
	{
		name: "LOG",
		description: "Berekent de logaritme van een getal met het door u opgegeven grondtal.",
		arguments: [
			{
				name: "getal",
				description: "is het positieve reële getal waarvan u de logaritme wilt berekenen"
			},
			{
				name: "grondtal",
				description: "is het grondtal van de logaritme. Als dit wordt weggelaten, wordt uitgegaan van 10"
			}
		]
	},
	{
		name: "LOG.NORM.INV",
		description: "Berekent de inverse van de logaritmische normale verdeling van x, waarbij ln(x) normaal wordt verdeeld met de parameters Gemiddelde en Standaarddev.",
		arguments: [
			{
				name: "kans",
				description: "is een getal van 0 tot en met 1 voor de kans die samenhangt met de logaritmische normale verdeling"
			},
			{
				name: "gemiddelde",
				description: "is de gemiddelde ln(x)"
			},
			{
				name: "standaarddev",
				description: "is de standaarddeviatie van ln(x). Dit is een positief getal"
			}
		]
	},
	{
		name: "LOG.NORM.VERD",
		description: "Geeft als resultaat de logaritmische normale verdeling van x, waarbij ln(x) normaal wordt verdeeld met de parameters Gemiddelde en Standaarddev.",
		arguments: [
			{
				name: "x",
				description: "is de waarde waarop de functie geëvalueerd moet worden. Dit is een positief getal."
			},
			{
				name: "gemiddelde",
				description: "is de gemiddelde ln(x)"
			},
			{
				name: "standaarddev",
				description: "is de standaarddeviatie van ln(x). Dit is een positief getal"
			}
		]
	},
	{
		name: "LOG10",
		description: "Berekent de logaritme met grondtal 10 van een getal.",
		arguments: [
			{
				name: "getal",
				description: "is het positieve reële getal waarvan u de logaritme met grondtal 10 wilt berekenen"
			}
		]
	},
	{
		name: "LOGNORM.INV",
		description: "Berekent de inverse van de logaritmische normale verdeling van x, waarbij ln(x) normaal wordt verdeeld met de parameters Gemiddelde en Standaarddev.",
		arguments: [
			{
				name: "kans",
				description: "is een getal van 0 tot en met 1 voor de kans die samenhangt met de logaritmische normale verdeling"
			},
			{
				name: "gemiddelde",
				description: "is de gemiddelde ln(x)"
			},
			{
				name: "standaarddev",
				description: "is de standaarddeviatie van ln(x). Dit is een positief getal."
			}
		]
	},
	{
		name: "LOGNORM.VERD",
		description: "Geeft als resultaat de logaritmische normale verdeling van x, waarbij ln(x) normaal is verdeeld met de parameters Gemiddelde en Standaarddev.",
		arguments: [
			{
				name: "x",
				description: "is de waarde waarvoor de functie moet worden geëvalueerd. Dit is een positief getal"
			},
			{
				name: "gemiddelde",
				description: "is het gemiddelde van ln(x)"
			},
			{
				name: "standaarddev",
				description: "is de standaarddeviatie van ln(x). Dit is een positief getal"
			},
			{
				name: "cumulatief",
				description: "is een logische waarde: gebruik WAAR voor de cumulatieve verdelingsfunctie en gebruik ONWAAR voor de kansdichtheidsfunctie"
			}
		]
	},
	{
		name: "LOGSCH",
		description: "Geeft als resultaat een matrix met waarden die een exponentiële curve beschrijven die past bij de gegevens en die is berekend in regressieanalyse.",
		arguments: [
			{
				name: "y-bekend",
				description: "is de verzameling Y-waarden die al bekend is uit y = b*m^x"
			},
			{
				name: "x-bekend",
				description: "is een optionele verzameling X-waarden die wellicht al bekend is uit y = b*m^x"
			},
			{
				name: "const",
				description: "is een logische waarde. Als Const = WAAR of wordt weggelaten, wordt de constante b normaal berekend, als Const = ONWAAR, is de constante b gelijk aan 1"
			},
			{
				name: "stat",
				description: "is een logische waarde: WAAR = geef als resultaat aanvullende regressiegrootheden, ONWAAR of weggelaten = geef als resultaat m-coëfficiënten en de constante b"
			}
		]
	},
	{
		name: "MAAND",
		description: "Geeft als resultaat de maand, een getal van 1 (januari) tot en met 12 (december).",
		arguments: [
			{
				name: "serieel-getal",
				description: "is een getal in het systeem dat in Spreadsheet wordt gebruikt voor datumberekeningen"
			}
		]
	},
	{
		name: "MACHT",
		description: "Geeft als resultaat een getal verheven tot een macht.",
		arguments: [
			{
				name: "getal",
				description: "is een reëel getal dat u tot een bepaalde macht wilt verheffen"
			},
			{
				name: "macht",
				description: "is de exponent van de machtsverheffing"
			}
		]
	},
	{
		name: "MAX",
		description: "Geeft als resultaat de grootste waarde in een lijst met argumenten. Logische waarden en tekst worden genegeerd.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "getal1",
				description: "zijn maximaal 255 getallen, lege cellen, logische waarden of tekstgetallen waarvan u het maximum wilt zoeken"
			},
			{
				name: "getal2",
				description: "zijn maximaal 255 getallen, lege cellen, logische waarden of tekstgetallen waarvan u het maximum wilt zoeken"
			}
		]
	},
	{
		name: "MAXA",
		description: "Geeft als resultaat de grootste waarde in een verzameling waarden. Logische waarden en tekst worden niet genegeerd.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "waarde1",
				description: "zijn 1 tot 255 getallen, lege cellen, logische waarden of getallen met een tekstindeling waarvan u de grootste waarde wilt bepalen"
			},
			{
				name: "waarde2",
				description: "zijn 1 tot 255 getallen, lege cellen, logische waarden of getallen met een tekstindeling waarvan u de grootste waarde wilt bepalen"
			}
		]
	},
	{
		name: "MEDIAAN",
		description: "Berekent de mediaan (het getal in het midden van een set) van de gegeven getallen.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "getal1",
				description: "zijn maximaal 255 getallen of namen, matrices of verwijzingen die getallen bevatten waarvan u de mediaan wilt bepalen"
			},
			{
				name: "getal2",
				description: "zijn maximaal 255 getallen of namen, matrices of verwijzingen die getallen bevatten waarvan u de mediaan wilt bepalen"
			}
		]
	},
	{
		name: "MEETK.GEM",
		description: "Berekent het meetkundige gemiddelde van positieve numerieke gegevens in een matrix of bereik.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "getal1",
				description: "zijn 1 tot 255 getallen of namen, matrices of verwijzingen die getallen bevatten waarvan u het meetkundige gemiddelde wilt berekenen"
			},
			{
				name: "getal2",
				description: "zijn 1 tot 255 getallen of namen, matrices of verwijzingen die getallen bevatten waarvan u het meetkundige gemiddelde wilt berekenen"
			}
		]
	},
	{
		name: "MIN",
		description: "Geeft als resultaat het kleinste getal in een lijst met waarden. Logische waarden en tekst worden genegeerd.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "getal1",
				description: "zijn maximaal 255 getallen, lege cellen, logische waarden of tekstgetallen waarvan u het minimum wilt zoeken"
			},
			{
				name: "getal2",
				description: "zijn maximaal 255 getallen, lege cellen, logische waarden of tekstgetallen waarvan u het minimum wilt zoeken"
			}
		]
	},
	{
		name: "MINA",
		description: "Geeft als resultaat de kleinste waarde in een lijst met argumenten. Logische waarden en tekst worden niet genegeerd.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "waarde1",
				description: "zijn 1 tot 255 getallen, lege cellen, logische waarden of getallen met een tekstindeling waarvan u de kleinste waarde wilt bepalen"
			},
			{
				name: "waarde2",
				description: "zijn 1 tot 255 getallen, lege cellen, logische waarden of getallen met een tekstindeling waarvan u de kleinste waarde wilt bepalen"
			}
		]
	},
	{
		name: "MINUUT",
		description: "Geeft als resultaat het aantal minuten (een getal van 0 tot en met 59).",
		arguments: [
			{
				name: "serieel-getal",
				description: "is een getal dat een dag of tijd aangeeft in het systeem dat in Spreadsheet wordt gebruikt voor datum- en tijdberekeningen of tekst in tijdopmaak, zoals 16:48:00 of 4:48:00 PM"
			}
		]
	},
	{
		name: "MODUS",
		description: "Geeft als resultaat de meest voorkomende (repeterende) waarde in een matrix of bereik met gegevens.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "getal1",
				description: "zijn 1 tot 255 getallen of namen, matrices of verwijzingen die getallen bevatten waarvan u de modus wilt bepalen"
			},
			{
				name: "getal2",
				description: "zijn 1 tot 255 getallen of namen, matrices of verwijzingen die getallen bevatten waarvan u de modus wilt bepalen"
			}
		]
	},
	{
		name: "MODUS.ENKELV",
		description: "Geeft als resultaat de meest voorkomende (repeterende) waarde in een matrix of bereik met gegevens.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "getal1",
				description: "zijn 1 tot 255 getallen of namen, matrices of verwijzingen die getallen bevatten waarvan u de modus wilt bepalen"
			},
			{
				name: "getal2",
				description: "zijn 1 tot 255 getallen of namen, matrices of verwijzingen die getallen bevatten waarvan u de modus wilt bepalen"
			}
		]
	},
	{
		name: "MODUS.MEERV",
		description: "Berekent een verticale matrix van de vaakst voorkomende, of herhaalde waarden in een matrix of gegevensbereik. Voor een horizontale matrix gebruikt u =TRANSPONEREN(MODUS.VERM(getal1,getal2,...)).",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "getal1",
				description: "zijn 1 tot 255 getallen of namen, matrices of verwijzingen die getallen bevatten waarvoor u de modus wilt berekenen"
			},
			{
				name: "getal2",
				description: "zijn 1 tot 255 getallen of namen, matrices of verwijzingen die getallen bevatten waarvoor u de modus wilt berekenen"
			}
		]
	},
	{
		name: "MULTINOMIAAL",
		description: "Berekent de multinomiaal van een set getallen.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "complex_getal1",
				description: "zijn 1 tot 255 waarden waarvoor u de multinomiaal wilt berekenen"
			},
			{
				name: "complex_getal2",
				description: "zijn 1 tot 255 waarden waarvoor u de multinomiaal wilt berekenen"
			}
		]
	},
	{
		name: "N",
		description: "Converteert een waarde die geen getal is naar een getal, datums naar seriële getallen, WAAR naar 1 en overige waarden naar 0 (nul).",
		arguments: [
			{
				name: "waarde",
				description: "is de waarde die u wilt converteren"
			}
		]
	},
	{
		name: "NB",
		description: "Geeft als resultaat de foutwaarde #N/B (waarde niet beschikbaar).",
		arguments: [
		]
	},
	{
		name: "NEG.BINOM.VERD",
		description: "Geeft als resultaat de negatieve binomiaalverdeling, de kans dat er Aantal-ongunstig ongunstige uitkomsten zijn voor Aantal-gunstig gunstige uitkomsten, met een kans van Kans-gunstig op een gunstige uitkomst.",
		arguments: [
			{
				name: "aantal-ongunstig",
				description: "is het aantal ongunstige uitkomsten"
			},
			{
				name: "aantal-gunstig",
				description: "is het minimum aantal gunstige uitkomsten"
			},
			{
				name: "kans-gunstig",
				description: "is een getal van 0 tot en met 1 voor de kans op een gunstige uitkomst"
			}
		]
	},
	{
		name: "NEGBINOM.VERD",
		description: "Geeft als resultaat de negatieve binomiaalverdeling, de kans dat er Aantal-ongunstig ongunstige uitkomsten zijn voor Aantal-gunstig gunstige uitkomsten, met een kans van Kans-gunstig op een gunstige uitkomst.",
		arguments: [
			{
				name: "aantal-ongunstig",
				description: "is het aantal ongunstige uitkomsten"
			},
			{
				name: "aantal-gunstig",
				description: "is het minimum aantal gunstige uitkomsten"
			},
			{
				name: "kans-gunstig",
				description: "is een getal tussen 0 en 1 voor de kans op een gunstige uitkomst"
			},
			{
				name: "cumulatief",
				description: "is een logische waarde: gebruik WAAR voor de cumulatieve verdelingsfunctie en gebruik ONWAAR voor de kansdichtheidsfunctie"
			}
		]
	},
	{
		name: "NETTO.WERKDAGEN",
		description: "Geeft het aantal volledige werkdagen tussen twee datums.",
		arguments: [
			{
				name: "begindatum",
				description: "is een serieel getal dat de begindatum aangeeft"
			},
			{
				name: "einddatum",
				description: "is een serieel getal dat de einddatum aangeeft"
			},
			{
				name: "vakantiedagen",
				description: "is een optionele verzameling met een of meer seriële getallen die de datums aangeven waarop niet wordt gewerkt, zoals nationale feestdagen en vakantiedagen"
			}
		]
	},
	{
		name: "NETWERKDAGEN.INTL",
		description: "Geeft het aantal volledige werkdagen tussen twee datums met aangepaste weekendparameters.",
		arguments: [
			{
				name: "begindatum",
				description: "is een serieel getal dat de begindatum aangeeft"
			},
			{
				name: "einddatum",
				description: "is een serieel getal dat de einddatum aangeeft"
			},
			{
				name: "weekend",
				description: "is een getal of tekenreeks waarmee weekends worden aangegeven"
			},
			{
				name: "vakantiedagen",
				description: "is een optionele verzameling met een of meer seriële getallen die de datums aangeven waarop niet wordt gewerkt, zoals nationale feestdagen en vakantiedagen"
			}
		]
	},
	{
		name: "NHW",
		description: "Berekent de netto huidige waarde van een investering op basis van een discontopercentage en een reeks periodieke betalingen (negatieve waarden) en inkomsten (positieve waarden).",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "rente",
				description: "is het discontopercentage over één termijn"
			},
			{
				name: "waarde1",
				description: "zijn maximaal 254 betalingen en inkomsten, met gelijke tussenperioden, die aan het einde van elke periode optreden"
			},
			{
				name: "waarde2",
				description: "zijn maximaal 254 betalingen en inkomsten, met gelijke tussenperioden, die aan het einde van elke periode optreden"
			}
		]
	},
	{
		name: "NHW2",
		description: "Berekent de netto huidige waarde van een geplande serie cashflows.",
		arguments: [
			{
				name: "rente",
				description: "is het discontopercentage dat moet worden toegepast op de cashflows"
			},
			{
				name: "waarden",
				description: "is een reeks cashflows die correspondeert met een geplande serie betalingsdatums"
			},
			{
				name: "datums",
				description: "is een schema van betalingsdatums die correspondeert met de geplande cashflowbetalingen"
			}
		]
	},
	{
		name: "NIET",
		description: "Wijzigt de waarde ONWAAR in WAAR, of WAAR in ONWAAR.",
		arguments: [
			{
				name: "logisch",
				description: "is een waarde of expressie die als resultaat WAAR of ONWAAR kan geven"
			}
		]
	},
	{
		name: "NOMINALE.RENTE",
		description: "Berekent de jaarlijkse nominale rente.",
		arguments: [
			{
				name: "effectieve_rente",
				description: "is het effectieve rentepercentage"
			},
			{
				name: "termijnen_per_jaar",
				description: "is het aantal termijnen per jaar waarover de samengestelde rente wordt berekend"
			}
		]
	},
	{
		name: "NORM.INV",
		description: "Berekent de inverse van de cumulatieve normale verdeling voor het gemiddelde en de standaarddeviatie die u hebt opgegeven.",
		arguments: [
			{
				name: "kans",
				description: "is een getal van 0 tot en met 1 voor de kans die overeenkomt met een normale verdeling"
			},
			{
				name: "gemiddelde",
				description: "is het rekenkundige gemiddelde van de verdeling"
			},
			{
				name: "standaarddev",
				description: "is de standaarddeviatie van de verdeling. Dit is een positief getal"
			}
		]
	},
	{
		name: "NORM.INV.N",
		description: "Berekent de inverse van de cumulatieve normale verdeling voor het gemiddelde en de standaarddeviatie die u hebt opgegeven.",
		arguments: [
			{
				name: "kans",
				description: "is een getal van 0 tot en met 1 voor de kans die overeenkomt met een normale verdeling"
			},
			{
				name: "gemiddelde",
				description: "is het rekenkundige gemiddelde van de verdeling"
			},
			{
				name: "standaarddev",
				description: "is de standaarddeviatie van de verdeling. Dit is een positief getal"
			}
		]
	},
	{
		name: "NORM.S.INV",
		description: "Berekent de inverse van de cumulatieve normale standaardverdeling (met een gemiddelde nul en een standaarddeviatie één).",
		arguments: [
			{
				name: "kans",
				description: "is een getal van 0 tot en met 1 voor de kans die overeenkomt met een normale verdeling"
			}
		]
	},
	{
		name: "NORM.S.VERD",
		description: "Geeft als resultaat de normale standaardverdeling (heeft een gemiddelde van nul en een standaarddeviatie van één).",
		arguments: [
			{
				name: "z",
				description: "is de waarde waarvoor u de verdeling wilt weten"
			},
			{
				name: "cumulatief",
				description: "is een logische waarde die door de functie moet worden geretourneerd: de cumulatieve verdelingsfunctie = WAAR; de kansdichtheidsfunctie = ONWAAR"
			}
		]
	},
	{
		name: "NORM.VERD",
		description: "Geeft als resultaat de cumulatieve normale verdeling van het opgegeven gemiddelde en de standaarddeviatie.",
		arguments: [
			{
				name: "x",
				description: "is de waarde waarvoor u de verdeling wilt bepalen"
			},
			{
				name: "gemiddelde",
				description: "is het rekenkundige gemiddelde van de verdeling"
			},
			{
				name: "standaarddev",
				description: "is de standaarddeviatie van de verdeling. Dit is een positief getal."
			},
			{
				name: "cumulatief",
				description: "is een logische waarde. Gebruik WAAR voor de cumulatieve verdelingsfunctie en ONWAAR voor de kansdichtheidsfunctie"
			}
		]
	},
	{
		name: "NORM.VERD.N",
		description: "Resulteert in de normale verdeling voor het opgegeven gemiddelde en de standaarddeviatie.",
		arguments: [
			{
				name: "x",
				description: "is de waarde waarvoor u de verdeling wilt berekenen"
			},
			{
				name: "gemiddelde",
				description: "is het rekenkundige gemiddelde van de verdeling"
			},
			{
				name: "standaarddev",
				description: "is de standaarddeviatie van de verdeling, een positief getal"
			},
			{
				name: "cumulatieve",
				description: "is een logische waarde. Gebruik WAAR voor de cumulatieve verdelingsfunctie en gebruik ONWAAR voor de kansdichtheidsfunctie"
			}
		]
	},
	{
		name: "NORMALISEREN",
		description: "Berekent een genormaliseerde waarde uit een verdeling die wordt gekenmerkt door een gemiddelde en standaarddeviatie.",
		arguments: [
			{
				name: "x",
				description: "is de waarde die u wilt normaliseren"
			},
			{
				name: "gemiddelde",
				description: "is het rekenkundige gemiddelde van de verdeling"
			},
			{
				name: "standaarddev",
				description: "is de standaarddeviatie van de verdeling. Dit is een positief getal."
			}
		]
	},
	{
		name: "NPER",
		description: "Berekent het aantal termijnen van een investering, op basis van periodieke, constante betalingen en een constant rentepercentage.",
		arguments: [
			{
				name: "rente",
				description: "is het rentepercentage per termijn. Gebruik bijvoorbeeld 6%/4 voor kwartaalbetalingen met een rentepercentage van 6%"
			},
			{
				name: "bet",
				description: "is de betaling die iedere termijn wordt verricht. Dit bedrag kan gedurende de looptijd van de investering niet worden gewijzigd"
			},
			{
				name: "hw",
				description: "is de huidige waarde of het totaalbedrag dat een reeks toekomstige waarden op dit moment waard is"
			},
			{
				name: "tw",
				description: "is de toekomstige waarde of het kassaldo waarover u wilt beschikken als de laatste betaling is gedaan. Als dit wordt weggelaten, wordt nul gebruikt"
			},
			{
				name: "type_getal",
				description: "is een logische waarde: 1 = betaling aan het begin van de periode, 0 of weggelaten = betaling aan het einde van de periode"
			}
		]
	},
	{
		name: "NU",
		description: "Geeft als resultaat de huidige datum en tijd in de datum- en tijdnotatie.",
		arguments: [
		]
	},
	{
		name: "NUMERIEKE.WAARDE",
		description: "Converteert tekst naar getal, onafhankelijk van landinstellingen.",
		arguments: [
			{
				name: "tekst",
				description: "is de tekenreeks voor het getal dat u wilt converteren"
			},
			{
				name: "decimaal_scheidingsteken",
				description: "is het teken dat wordt gebruikt als decimaal scheidingsteken in de tekenreeks"
			},
			{
				name: "groep_scheidingsteken",
				description: "is het teken dat wordt gebruikt als groepsscheidingsteken in de tekenreeks"
			}
		]
	},
	{
		name: "OCT.N.BIN",
		description: "Converteert een octaal getal naar een binair getal.",
		arguments: [
			{
				name: "getal",
				description: "is het octale getal dat u wilt converteren"
			},
			{
				name: "aantal_tekens",
				description: "is het aantal tekens dat u wilt gebruiken"
			}
		]
	},
	{
		name: "OCT.N.DEC",
		description: "Converteert een octaal getal naar een decimaal getal.",
		arguments: [
			{
				name: "getal",
				description: "is het octale getal dat u wilt converteren"
			}
		]
	},
	{
		name: "OCT.N.HEX",
		description: "Converteert een octaal getal naar een hexadecimaal getal.",
		arguments: [
			{
				name: "getal",
				description: "is het octale getal dat u wilt converteren"
			},
			{
				name: "aantal_tekens",
				description: "is het aantal tekens dat u wilt gebruiken"
			}
		]
	},
	{
		name: "OF",
		description: "Controleert of een van de argumenten WAAR is, en geeft als resultaat WAAR of ONWAAR. Geeft alleen ONWAAR als resultaat als alle argumenten ONWAAR zijn.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "logisch1",
				description: "zijn maximaal 255 voorwaarden die u wilt testen, die WAAR of ONWAAR kunnen zijn"
			},
			{
				name: "logisch2",
				description: "zijn maximaal 255 voorwaarden die u wilt testen, die WAAR of ONWAAR kunnen zijn"
			}
		]
	},
	{
		name: "ONEVEN",
		description: "Rondt de absolute waarde van een positief getal naar boven af, en een negatief getal naar beneden, op het dichtstbijzijnde oneven gehele getal.",
		arguments: [
			{
				name: "getal",
				description: "is de waarde die moet worden afgerond"
			}
		]
	},
	{
		name: "ONWAAR",
		description: "Geeft als resultaat de logische waarde ONWAAR.",
		arguments: [
		]
	},
	{
		name: "OPBRENGST",
		description: "Berekent het bedrag dat op de vervaldatum wordt uitgekeerd voor volgestort waardepapier.",
		arguments: [
			{
				name: "stortingsdatum",
				description: "is de stortingsdatum van het waardepapier, uitgedrukt in een serieel getal"
			},
			{
				name: "vervaldatum",
				description: "is de vervaldatum van het waardepapier, uitgedrukt in een serieel getal"
			},
			{
				name: "investering",
				description: "is het bedrag dat in het waardepapier is geïnvesteerd"
			},
			{
				name: "disc",
				description: "is het discontopercentage van het waardepapier"
			},
			{
				name: "soort_jaar",
				description: "is het soort jaar waarop de berekening is gebaseerd"
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
		name: "PBET",
		description: "Berekent de afbetaling op de hoofdsom voor een gegeven investering, op basis van constante betalingen en een constant rentepercentage.",
		arguments: [
			{
				name: "rente",
				description: "is het rentepercentage per termijn. Gebruik bijvoorbeeld 6%/4 voor kwartaalbetalingen met een rentepercentage van 6%"
			},
			{
				name: "termijn",
				description: "geeft de termijn aan en moet een getal zijn tussen 1 en aantal-termijnen"
			},
			{
				name: "aantal-termijnen",
				description: "is het totale aantal betalingstermijnen van een investering"
			},
			{
				name: "hw",
				description: "is de huidige waarde: het totaalbedrag dat een reeks toekomstige waarden op dit moment waard is"
			},
			{
				name: "tw",
				description: "is de toekomstige waarde of het kassaldo waarover u wilt beschikken als de laatste betaling is gedaan"
			},
			{
				name: "type_getal",
				description: "is een logische waarde: 1 = betaling aan het begin van de periode, 0 of weggelaten = betaling aan het einde van de periode"
			}
		]
	},
	{
		name: "PDUUR",
		description: "Geeft als resultaat het aantal perioden dat is vereist voor een investering om een opgegeven waarde te bereiken.",
		arguments: [
			{
				name: "percentage",
				description: "is het rentepercentage per periode."
			},
			{
				name: "hw",
				description: "is de huidige waarde van de investering"
			},
			{
				name: "tw",
				description: "is de gewenste toekomstige waarde van de investering"
			}
		]
	},
	{
		name: "PEARSON",
		description: "Berekent de correlatiecoëfficiënt r van Pearson.",
		arguments: [
			{
				name: "matrix1",
				description: "is een verzameling onafhankelijke waarden"
			},
			{
				name: "matrix2",
				description: "is een verzameling afhankelijke waarden"
			}
		]
	},
	{
		name: "PERCENT.RANG",
		description: "Geeft als resultaat de positie, in procenten uitgedrukt, van een waarde in de rangorde van een gegevensbereik.",
		arguments: [
			{
				name: "matrix",
				description: "is de matrix of het bereik met numerieke gegevens waarin u de relatieve positie van waarde x wilt bepalen"
			},
			{
				name: "x",
				description: "is de waarde waarvan u de positie wilt weten"
			},
			{
				name: "significantie",
				description: "is een optionele waarde die het aantal significante cijfers van het resulterende percentage aangeeft. Als u niets opgeeft, worden drie cijfers weergegeven (0,xxx%)"
			}
		]
	},
	{
		name: "PERCENTIEL",
		description: "Berekent het k-percentiel van waarden in een bereik.",
		arguments: [
			{
				name: "matrix",
				description: "is de matrix of het gegevensbereik waarin u de relatieve positie van waarde k wilt bepalen"
			},
			{
				name: "k",
				description: "is de percentielwaarde in het bereik van 0 tot en met 1"
			}
		]
	},
	{
		name: "PERCENTIEL.EXC",
		description: "Geeft als resultaat het k-percentiel van waarden in een bereik, waarbij k zich in het bereik 0..1, exclusief bevindt.",
		arguments: [
			{
				name: "matrix",
				description: "is de matrix of het bereik van gegevens die de relatieve positie bepaalt"
			},
			{
				name: "k",
				description: "is de percentiele waarde tussen 0 en 1, inclusief"
			}
		]
	},
	{
		name: "PERCENTIEL.INC",
		description: "Geeft als resultaat het k-percentiel van waarden in een bereik, waarbij k zich in het bereik 0..1, inclusief bevindt.",
		arguments: [
			{
				name: "matrix",
				description: "is de matrix of het bereik van gegevens die de relatieve positie bepaalt"
			},
			{
				name: "k",
				description: "is de percentiele waarde tussen 0 en 1, inclusief"
			}
		]
	},
	{
		name: "PERMUTATIE.A",
		description: "Geeft als resultaat het aantal permutaties voor een opgegeven aantal objecten (met herhalingen) dat kan worden geselecteerd in het totale aantal objecten.",
		arguments: [
			{
				name: "getal",
				description: "is het totale aantal objecten"
			},
			{
				name: "aantal_gekozen",
				description: "is het aantal objecten in elke permutatie"
			}
		]
	},
	{
		name: "PERMUTATIES",
		description: "Berekent het aantal permutaties voor een gegeven aantal objecten dat uit het totale aantal objecten geselecteerd kan worden.",
		arguments: [
			{
				name: "getal",
				description: "is het totale aantal objecten"
			},
			{
				name: "aantal-gekozen",
				description: "is het aantal objecten in elke permutatie"
			}
		]
	},
	{
		name: "PHI",
		description: "Geeft als resultaat de waarde van de dichtheidsfunctie voor de normale standaardverdeling.",
		arguments: [
			{
				name: "x",
				description: "is het getal waarvoor u de dichtheid van de normale standaardverdeling wilt berekenen"
			}
		]
	},
	{
		name: "PI",
		description: "Geeft als resultaat de waarde van de rekenkundige constante pi (3,14159265358979), nauwkeurig tot op 15 cijfers.",
		arguments: [
		]
	},
	{
		name: "POISSON",
		description: "Geeft als resultaat de Poisson-verdeling.",
		arguments: [
			{
				name: "x",
				description: "is het aantal gebeurtenissen"
			},
			{
				name: "gemiddelde",
				description: "is de verwachte numerieke waarde. Dit is een positief getal."
			},
			{
				name: "cumulatief",
				description: "is een logische waarde. Gebruik WAAR voor de cumulatieve Poisson-kans en ONWAAR voor de Poisson-kansdichtheidsfunctie"
			}
		]
	},
	{
		name: "POISSON.VERD",
		description: "Geeft als resultaat de Poisson-verdeling.",
		arguments: [
			{
				name: "x",
				description: "is het aantal gebeurtenissen"
			},
			{
				name: "gemiddelde",
				description: "is de verwachte numerieke waarde. Dit is een positief getal."
			},
			{
				name: "cumulatief",
				description: "is een logische waarde. Gebruik WAAR voor de cumulatieve Poisson-kans en ONWAAR voor de Poisson-kansdichtheidsfunctie"
			}
		]
	},
	{
		name: "POS.NEG",
		description: "Geeft als resultaat het teken van een getal: 1 als het getal positief is, nul als het getal nul is en -1 als het getal negatief is.",
		arguments: [
			{
				name: "getal",
				description: "is een reëel getal"
			}
		]
	},
	{
		name: "PRIJS.DISCONTO",
		description: "Bepaalt de prijs per 100 euro nominale waarde voor een verdisconteerd waardepapier.",
		arguments: [
			{
				name: "stortingsdatum",
				description: "is de stortingsdatum van het waardepapier, uitgedrukt in een serieel getal"
			},
			{
				name: "vervaldatum",
				description: "is de vervaldatum van het waardepapier, uitgedrukt in een serieel getal"
			},
			{
				name: "disc",
				description: "is het jaarlijkse discontopercentage van het waardepapier"
			},
			{
				name: "aflossingsprijs",
				description: "is de aflossingsprijs van het waardepapier per 100 euro nominale waarde"
			},
			{
				name: "soort_jaar",
				description: "is het soort jaar waarop de berekening is gebaseerd"
			}
		]
	},
	{
		name: "PROCENTRANG.EXC",
		description: "Bepaalt de positie van een waarde in een gegevensset als een percentage van de gegevensset als een percentage (0..1, exclusief) van de gegevensset.",
		arguments: [
			{
				name: "matrix",
				description: "is de matrix of het gegevensbereik met numerieke waarden die de relatieve positie bepaalt"
			},
			{
				name: "x",
				description: "is de waarde waarvoor u de positie wilt weten"
			},
			{
				name: "significantie",
				description: "is een optionele waarde die het aantal significante cijfers voor het resulterende percentage identificeert. Als u niets opgeeft, worden drie cijfers weergegeven (0,xxx%)"
			}
		]
	},
	{
		name: "PROCENTRANG.INC",
		description: "Bepaalt de positie van een waarde in een gegevensset als een percentage van de gegevensset als een percentage (0..1, inclusief) van de gegevensset.",
		arguments: [
			{
				name: "matrix",
				description: "is de matrix of het gegevensbereik met numerieke waarden die de relatieve positie bepaalt"
			},
			{
				name: "x",
				description: "is de waarde waarvoor u de positie wilt weten"
			},
			{
				name: "significantie",
				description: "is een optionele waarde die het aantal significante cijfers voor het resulterende percentage identificeert. Als u niets opgeeft, worden drie cijfers weergegeven (0,xxx%)"
			}
		]
	},
	{
		name: "PRODUCT",
		description: "Vermenigvuldigt de getallen die zijn opgegeven als argumenten met elkaar.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "getal1",
				description: "zijn maximaal 255 getallen, logische waarden of tekstgetallen van getallen die u wilt vermenigvuldigen"
			},
			{
				name: "getal2",
				description: "zijn maximaal 255 getallen, logische waarden of tekstgetallen van getallen die u wilt vermenigvuldigen"
			}
		]
	},
	{
		name: "PRODUCTMAT",
		description: "Berekent het product van twee matrices, een matrix met hetzelfde aantal rijen als matrix1 en hetzelfde aantal kolommen als matrix2.",
		arguments: [
			{
				name: "matrix1",
				description: "is de eerste matrix met getallen die u wilt vermenigvuldigen. Deze matrix moet hetzelfde aantal kolommen hebben als het aantal rijen in matrix2"
			},
			{
				name: "matrix2",
				description: "is de eerste matrix met getallen die u wilt vermenigvuldigen. Deze matrix moet hetzelfde aantal kolommen hebben als het aantal rijen in matrix2"
			}
		]
	},
	{
		name: "QUOTIENT",
		description: "Geeft de uitkomst van een deling in gehele getallen.",
		arguments: [
			{
				name: "teller",
				description: "is de teller of het deeltal"
			},
			{
				name: "noemer",
				description: "is de noemer of deler"
			}
		]
	},
	{
		name: "R.KWADRAAT",
		description: "Berekent R-kwadraat van een lineaire regressielijn door de ingevoerde gegevenspunten.",
		arguments: [
			{
				name: "y-bekend",
				description: "is een matrix of een bereik met gegevenspunten. Dit kunnen getallen zijn of namen, matrices of verwijzingen die getallen bevatten"
			},
			{
				name: "x-bekend",
				description: "is een matrix of een bereik met gegevenspunten. Dit kunnen getallen zijn of namen, matrices of verwijzingen die getallen bevatten"
			}
		]
	},
	{
		name: "RADIALEN",
		description: "Converteert graden naar radialen.",
		arguments: [
			{
				name: "hoek",
				description: "is een hoek in graden die u wilt converteren"
			}
		]
	},
	{
		name: "RANG",
		description: "Berekent de rang van een getal in een lijst getallen: de grootte ten opzichte van andere waarden in de lijst.",
		arguments: [
			{
				name: "getal",
				description: "is het getal waarvan u de rang wilt bepalen"
			},
			{
				name: "verw",
				description: "is een matrix of een verwijzing naar een lijst met getallen. Niet-numerieke waarden worden genegeerd"
			},
			{
				name: "volgorde",
				description: "is een getal: 0 of weggelaten = rang in de aflopend gesorteerde lijst, een waarde ongelijk aan nul = rang in de oplopend gesorteerde lijst"
			}
		]
	},
	{
		name: "RANG.GELIJK",
		description: "Berekent de rang van een getal in een lijst getallen: de grootte ten opzichte van andere waarden in de lijst; als meerdere waarden dezelfde rang hebben, wordt de bovenste rang van die set met waarden geretourneerd.",
		arguments: [
			{
				name: "getal",
				description: "is het getal waarvan u de rang wilt bepalen"
			},
			{
				name: "verw",
				description: "is een matrix of een verwijzing naar een lijst met getallen. Niet-numerieke waarden worden genegeerd"
			},
			{
				name: "volgorde",
				description: "is een getal: 0 of weggelaten = rang in de aflopend gesorteerde lijst, een waarde ongelijk aan nul = rang in de oplopend gesorteerde lijst"
			}
		]
	},
	{
		name: "RANG.GEMIDDELDE",
		description: "Berekent de rang van een getal in een lijst getallen: de grootte ten opzichte van andere waarden in de lijst; als meer dan één waarde dezelfde rang heeft, wordt de gemiddelde rang geretourneerd.",
		arguments: [
			{
				name: "getal",
				description: "is het getal waarvan u de rang wilt bepalen"
			},
			{
				name: "verw",
				description: "is een matrix of een verwijzing naar een lijst met getallen. Niet-numerieke waarden worden genegeerd"
			},
			{
				name: "volgorde",
				description: "is een getal: 0 of weggelaten = rang in de aflopend gesorteerde lijst, een waarde ongelijk aan nul = rang in de oplopend gesorteerde lijst"
			}
		]
	},
	{
		name: "RECHTS",
		description: "Geeft als resultaat het opgegeven aantal tekens vanaf het einde van een tekenreeks.",
		arguments: [
			{
				name: "tekst",
				description: "is de tekenreeks met de tekens die u wilt ophalen"
			},
			{
				name: "aantal-tekens",
				description: "geeft aan hoeveel tekens u wilt ophalen. Als dit wordt weggelaten, wordt uitgegaan van 1"
			}
		]
	},
	{
		name: "REND.DISCONTO",
		description: "Berekent het jaarlijkse rendement van verdisconteerd waardepapier.",
		arguments: [
			{
				name: "stortingsdatum",
				description: "is de stortingsdatum van het waardepapier, uitgedrukt in een serieel getal"
			},
			{
				name: "vervaldatum",
				description: "is de vervaldatum van een waardepapier, uitgedrukt in een serieel getal"
			},
			{
				name: "prijs",
				description: "is de prijs van het waardepapier per 100 euro nominale waarde"
			},
			{
				name: "aflossingsprijs",
				description: "is de aflossingsprijs van het waardepapier per 100 euro nominale waarde"
			},
			{
				name: "soort_jaar",
				description: "is het soort jaar waarop de berekening is gebaseerd"
			}
		]
	},
	{
		name: "RENTE",
		description: "Berekent het periodieke rentepercentage voor een lening of een investering. Gebruik bijvoorbeeld 6%/4 voor kwartaalbetalingen met een rentepercentage van 6%.",
		arguments: [
			{
				name: "aantal-termijnen",
				description: "is het totale aantal betalingstermijnen voor de lening of de investering"
			},
			{
				name: "bet",
				description: "is de betaling die iedere termijn wordt verricht. Dit bedrag kan gedurende de looptijd van de investering niet worden gewijzigd"
			},
			{
				name: "hw",
				description: "is de huidige waarde: het totaalbedrag dat een reeks toekomstige betalingen op dit moment waard is"
			},
			{
				name: "tw",
				description: "is de toekomstige waarde of het kassaldo waarover u wilt beschikken als de laatste betaling is gedaan. Als dit wordt weggelaten, wordt Tw = 0 gebruikt"
			},
			{
				name: "type_getal",
				description: "is een logische waarde: betaling aan het begin van de termijn = 1; betaling aan het eind van de termijn = 0 of weggelaten"
			},
			{
				name: "schatting",
				description: "is uw schatting voor het rentepercentage. Als dit wordt weggelaten, wordt uitgegaan van Schatting = 0,1 (10 procent)"
			}
		]
	},
	{
		name: "RENTEPERCENTAGE",
		description: "Berekent het rentepercentage voor een volgestort waardepapier.",
		arguments: [
			{
				name: "stortingsdatum",
				description: "is de stortingsdatum van het waardepapier, uitgedrukt in een serieel getal"
			},
			{
				name: "vervaldatum",
				description: "is de vervaldatum van het waardepapier, uitgedrukt in een serieel getal"
			},
			{
				name: "investering",
				description: "is het bedrag dat in het waardepapier is geïnvesteerd"
			},
			{
				name: "aflossingsprijs",
				description: "is het bedrag dat op de vervaldatum wordt uitgekeerd"
			},
			{
				name: "soort_jaar",
				description: "is het soort jaar waarop de berekening is gebaseerd"
			}
		]
	},
	{
		name: "REST",
		description: "Geeft als resultaat het restgetal bij de deling van een getal door een deler.",
		arguments: [
			{
				name: "getal",
				description: "is het getal waarvan u na deling het restgetal wilt bepalen"
			},
			{
				name: "deler",
				description: "is het getal waardoor u Getal wilt delen"
			}
		]
	},
	{
		name: "RICHTING",
		description: "Berekent de richtingscoëfficiënt van een lineaire regressielijn door de ingevoerde gegevenspunten.",
		arguments: [
			{
				name: "y-bekend",
				description: "is een matrix of celbereik met afhankelijke numerieke gegevenspunten. Dit kunnen getallen zijn of namen, matrices of verwijzingen die getallen bevatten"
			},
			{
				name: "x-bekend",
				description: "is een verzameling onafhankelijke gegevenspunten. Dit kunnen getallen zijn of namen, matrices of verwijzingen die getallen bevatten"
			}
		]
	},
	{
		name: "RIJ",
		description: "Geeft als resultaat het rijnummer van een verwijzing.",
		arguments: [
			{
				name: "verw",
				description: "is de cel of een enkel celbereik waarvan u het rijnummer wilt weten. Als dit wordt weggelaten, is het resultaat de cel die de functie RIJ bevat"
			}
		]
	},
	{
		name: "RIJEN",
		description: "Geeft als resultaat het aantal rijen in een verwijzing of matrix.",
		arguments: [
			{
				name: "matrix",
				description: "is een matrix, een matrixformule of een verwijzing naar een celbereik waarvan u het aantal rijen wilt weten"
			}
		]
	},
	{
		name: "ROMEINS",
		description: "Converteert Arabische cijfers naar Romeinse cijfers, als tekst.",
		arguments: [
			{
				name: "getal",
				description: "is het getal met Arabische cijfers dat u wilt converteren"
			},
			{
				name: "type_getal",
				description: "is een getal waarmee u het type Romeinse cijfers kiest dat u wilt gebruiken."
			}
		]
	},
	{
		name: "RRI",
		description: "Geeft als resultaat een equivalent rentepercentage voor de groei van een investering.",
		arguments: [
			{
				name: "perioden",
				description: "is het aantal perioden voor de investering"
			},
			{
				name: "hw",
				description: "is de huidige waarde van de investering"
			},
			{
				name: "tw",
				description: "is de toekomstige waarde van de investering"
			}
		]
	},
	{
		name: "RTG",
		description: "Haalt realtimegegevens op uit een programma dat COM-automatisering ondersteunt.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "prog-id",
				description: "is de naam van de ProgID van een geregistreerd COM-automatiseringsprogramma. Plaats de naam tussen aanhalingstekens"
			},
			{
				name: "server",
				description: "is de naam van de server waarop de invoegtoepassing moet worden uitgevoerd. Plaats de naam tussen aanhalingstekens. Als de invoegtoepassing lokaal wordt uitgevoerd, moet u een lege tekenreeks gebruiken"
			},
			{
				name: "onderwerp1",
				description: "zijn 1 tot 38 parameters die een gedeelte van de gegevens aangeven"
			},
			{
				name: "onderwerp2",
				description: "zijn 1 tot 38 parameters die een gedeelte van de gegevens aangeven"
			}
		]
	},
	{
		name: "SAMENG.RENTE.V",
		description: "Berekent de samengestelde rente voor een waardepapier waarvan de rente op de vervaldatum wordt uitgekeerd.",
		arguments: [
			{
				name: "uitgifte",
				description: "is de uitgiftedatum van het waardepapier, uitgedrukt in een serieel getal"
			},
			{
				name: "vervaldatum",
				description: "is de vervaldatum van het waardepapier, uitgedrukt in een serieel getal"
			},
			{
				name: "rente",
				description: "is het jaarlijkse rentepercentage van het waardepapier"
			},
			{
				name: "nominale_waarde",
				description: "is de nominale waarde van het waardepapier"
			},
			{
				name: "soort_jaar",
				description: "is het soort jaar waarop de berekening is gebaseerd"
			}
		]
	},
	{
		name: "SCHATK.OBL",
		description: "Berekent het rendement op schatkistpapier op dezelfde manier waarop het rendement op obligaties wordt berekend.",
		arguments: [
			{
				name: "stortingsdatum",
				description: "is de stortingsdatum van het schatkistpapier, uitgedrukt in een serieel getal"
			},
			{
				name: "vervaldatum",
				description: "is de vervaldatum van het schatkistpapier, uitgedrukt in een serieel getal"
			},
			{
				name: "disc",
				description: "is het discontopercentage van het schatkistpapier"
			}
		]
	},
	{
		name: "SCHATK.PRIJS",
		description: "Bepaalt de prijs per 100 euro nominale waarde voor schatkistpapier.",
		arguments: [
			{
				name: "stortingsdatum",
				description: "is de stortingsdatum van het schatkistpapier, uitgedrukt in een serieel getal"
			},
			{
				name: "vervaldatum",
				description: "is de vervaldatum van het schatkistpapier, uitgedrukt in een serieel getal"
			},
			{
				name: "disc",
				description: "is het discontopercentage van het schatkistpapier"
			}
		]
	},
	{
		name: "SCHATK.REND",
		description: "Berekent het rendement van schatkistpapier.",
		arguments: [
			{
				name: "stortingsdatum",
				description: "is de stortingsdatum van het schatkistpapier, uitgedrukt in een serieel getal"
			},
			{
				name: "vervaldatum",
				description: "is de vervaldatum van het schatkistpapier, uitgedrukt in een serieel getal"
			},
			{
				name: "prijs",
				description: "is de prijs van het schatkistpapier per 100 euro nominale waarde"
			}
		]
	},
	{
		name: "SCHEEFHEID",
		description: "Berekent de mate van asymmetrie van een verdeling: een aanduiding van de mate van asymmetrie van een verdeling rond het gemiddelde.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "getal1",
				description: "zijn 1 tot 255 getallen of namen, matrices of verwijzingen die getallen bevatten waarvoor u de scheefheid wilt berekenen"
			},
			{
				name: "getal2",
				description: "zijn 1 tot 255 getallen of namen, matrices of verwijzingen die getallen bevatten waarvoor u de scheefheid wilt berekenen"
			}
		]
	},
	{
		name: "SCHEEFHEID.P",
		description: "Geeft als resultaat de scheefheid van een verdeling op basis van een populatie: een kenmerk van de mate van asymmetrie van een verdeling rondom het gemiddelde.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "getal1",
				description: "zijn maximaal 254 getallen of namen, matrices of verwijzingen die getallen bevatten waarvoor u de populatiescheefheid wilt berekenen"
			},
			{
				name: "getal2",
				description: "zijn maximaal 254 getallen of namen, matrices of verwijzingen die getallen bevatten waarvoor u de populatiescheefheid wilt berekenen"
			}
		]
	},
	{
		name: "SEC",
		description: "Geeft als resultaat de secans van een hoek.",
		arguments: [
			{
				name: "getal",
				description: "is de hoek in radialen waarvoor u de secans wilt berekenen"
			}
		]
	},
	{
		name: "SECH",
		description: "Geeft als resultaat de secans hyperbolicus van een hoek.",
		arguments: [
			{
				name: "getal",
				description: "is de hoek in radialen waarvoor u de secans hyperbolicus wilt berekenen"
			}
		]
	},
	{
		name: "SECONDE",
		description: "Geeft als resultaat het aantal seconden (een getal van 0 tot en met 59).",
		arguments: [
			{
				name: "serieel-getal",
				description: "is het seriële getal dat een dag of tijd aangeeft in het systeem dat in Spreadsheet wordt gebruikt voor datum- en tijdberekeningen of een tekst in tijdopmaak, zoals 16:48:23 of 4:48:23 PM"
			}
		]
	},
	{
		name: "SIN",
		description: "Berekent de sinus van de opgegeven hoek.",
		arguments: [
			{
				name: "getal",
				description: "is de hoek in radialen waarvan u de sinus wilt berekenen. Graden * PI()/180 = radialen"
			}
		]
	},
	{
		name: "SINH",
		description: "Berekent de sinus hyperbolicus van een getal.",
		arguments: [
			{
				name: "getal",
				description: "is een reëel getal"
			}
		]
	},
	{
		name: "SNIJPUNT",
		description: "Berekent het snijpunt van een lijn met de y-as aan de hand van een optimale regressielijn die wordt getrokken door de bekende x-waarden en y-waarden.",
		arguments: [
			{
				name: "y-bekend",
				description: "is de afhankelijke verzameling van waarnemingen of gegevens. Dit kunnen getallen zijn of namen, matrices of verwijzingen die getallen bevatten"
			},
			{
				name: "x-bekend",
				description: "is de onafhankelijke verzameling van waarnemingen of gegevens. Dit kunnen getallen zijn of namen, matrices of verwijzingen die getallen bevatten"
			}
		]
	},
	{
		name: "SOM",
		description: "Telt de getallen in een celbereik op.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "getal1",
				description: "zijn maximaal 255 getallen die u wilt optellen. Logische waarden en tekst in cellen worden genegeerd, behalve als deze als argumenten worden opgegeven"
			},
			{
				name: "getal2",
				description: "zijn maximaal 255 getallen die u wilt optellen. Logische waarden en tekst in cellen worden genegeerd, behalve als deze als argumenten worden opgegeven"
			}
		]
	},
	{
		name: "SOM.ALS",
		description: "Telt de cellen bij elkaar op die voldoen aan het criterium dat of de voorwaarde die u hebt ingesteld.",
		arguments: [
			{
				name: "bereik",
				description: "is het celbereik waarop u de bewerking wilt uitvoeren"
			},
			{
				name: "criterium",
				description: "is de voorwaarde of het criterium in de vorm van een getal, een expressie of tekst met behulp waarvan de cellen voor de optelsom worden geselecteerd"
			},
			{
				name: "optelbereik",
				description: "zijn de werkelijke cellen waarop u de bewerking wilt uitvoeren. Als dit wordt weggelaten, worden de cellen in het bereik gebruikt"
			}
		]
	},
	{
		name: "SOM.MACHTREEKS",
		description: "Berekent de som van een machtreeks die is gebaseerd op de formule.",
		arguments: [
			{
				name: "x",
				description: "is de invoerwaarde voor de machtreeks"
			},
			{
				name: "n",
				description: "is de macht tot waartoe u de eerste x-waarde in de reeks wilt verheffen"
			},
			{
				name: "m",
				description: "is de stap waarmee n wordt verhoogd in elke term van de reeks"
			},
			{
				name: "coëfficienten",
				description: "is een verzameling coëfficiënten waarmee elke opeenvolgende macht van x wordt vermenigvuldigd"
			}
		]
	},
	{
		name: "SOM.X2MINY2",
		description: "Telt de verschillen tussen de kwadraten van twee corresponderende bereiken of matrices op.",
		arguments: [
			{
				name: "x-matrix",
				description: "is de eerste matrix of het eerste gegevensbereik. Dit kan een getal zijn of een naam, matrix of verwijzing die getallen bevat"
			},
			{
				name: "y-matrix",
				description: "is de tweede matrix of het tweede gegevensbereik. Dit kan een getal zijn of een naam, matrix of verwijzing die getallen bevat"
			}
		]
	},
	{
		name: "SOM.X2PLUSY2",
		description: "Geeft het somtotaal van de som van de kwadraten van waarden in twee corresponderende bereiken of matrices als resultaat.",
		arguments: [
			{
				name: "x-matrix",
				description: "is de eerste matrix of het eerste gegevensbereik. Dit kan een getal zijn of een naam, matrix of verwijzing die getallen bevat"
			},
			{
				name: "y-matrix",
				description: "is de tweede matrix of het tweede gegevensbereik. Dit kan een getal zijn of een naam, matrix of verwijzing die getallen bevat"
			}
		]
	},
	{
		name: "SOM.XMINY.2",
		description: "Telt de kwadraten van de verschillen tussen twee corresponderende bereiken of matrices op.",
		arguments: [
			{
				name: "x-matrix",
				description: "is de eerste matrix of het eerste gegevensbereik. Dit kan een getal zijn of een naam, matrix of verwijzing die getallen bevat"
			},
			{
				name: "y-matrix",
				description: "is de tweede matrix of het tweede gegevensbereik. Dit kan een getal zijn of een naam, matrix of verwijzing die getallen bevat"
			}
		]
	},
	{
		name: "SOMMEN.ALS",
		description: "Telt het aantal cellen op dat wordt gespecificeerd door een gegeven set voorwaarden of criteria.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "optelbereik",
				description: "is het werkelijke aantal cellen dat wordt opgeteld."
			},
			{
				name: "criteriumbereik",
				description: "is het celbereik dat u wilt evalueren voor de voorwaarde in kwestie"
			},
			{
				name: "criteria",
				description: "is de voorwaarde of het criterium in de vorm van een getal, expressie of tekst waarmee wordt aangegeven welke cellen worden opgeteld"
			}
		]
	},
	{
		name: "SOMPRODUCT",
		description: "Geeft als resultaat de som van de producten van corresponderende bereiken of matrices.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "matrix1",
				description: "zijn minimaal 2 en maximaal 255 matrices waarvan u de elementen wilt vermenigvuldigen en vervolgens optellen. Alle matrices moeten dezelfde afmetingen hebben"
			},
			{
				name: "matrix2",
				description: "zijn minimaal 2 en maximaal 255 matrices waarvan u de elementen wilt vermenigvuldigen en vervolgens optellen. Alle matrices moeten dezelfde afmetingen hebben"
			},
			{
				name: "matrix3",
				description: "zijn minimaal 2 en maximaal 255 matrices waarvan u de elementen wilt vermenigvuldigen en vervolgens optellen. Alle matrices moeten dezelfde afmetingen hebben"
			}
		]
	},
	{
		name: "SPATIES.WISSEN",
		description: "Verwijdert de spaties uit een tekst, behalve de enkele spaties tussen woorden.",
		arguments: [
			{
				name: "tekst",
				description: "is de tekst waaruit u de spaties wilt verwijderen"
			}
		]
	},
	{
		name: "STAND.FOUT.YX",
		description: "Berekent de standaardfout in de voorspelde y-waarde voor elke x in een regressie.",
		arguments: [
			{
				name: "y-bekend",
				description: "is een matrix of een bereik met afhankelijke gegevenspunten. Dit kunnen getallen zijn of namen, matrices of verwijzingen die getallen bevatten"
			},
			{
				name: "x-bekend",
				description: "is een matrix of een bereik met onafhankelijke gegevenspunten. Dit kunnen getallen zijn of namen, matrices of verwijzingen die getallen bevatten"
			}
		]
	},
	{
		name: "STAND.NORM.INV",
		description: "Berekent de inverse van de cumulatieve normale standaardverdeling (met een gemiddelde nul en een standaarddeviatie één).",
		arguments: [
			{
				name: "kans",
				description: "is een getal van 0 tot en met 1 voor de kans die overeenkomt met een normale verdeling"
			}
		]
	},
	{
		name: "STAND.NORM.VERD",
		description: "Geeft als resultaat de cumulatieve normale standaardverdeling (met een gemiddelde nul en een standaarddeviatie één).",
		arguments: [
			{
				name: "z",
				description: "is de waarde waarvoor u de verdeling wilt bepalen"
			}
		]
	},
	{
		name: "STDEV",
		description: "Maakt een schatting van de standaarddeviatie op basis van een steekproef (logische waarden en tekst in de steekproef worden genegeerd).",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "getal1",
				description: "zijn maximaal 255 getallen die resulteren uit een steekproef onder een populatie. Dit kunnen getallen zijn of verwijzingen die getallen bevatten"
			},
			{
				name: "getal2",
				description: "zijn maximaal 255 getallen die resulteren uit een steekproef onder een populatie. Dit kunnen getallen zijn of verwijzingen die getallen bevatten"
			}
		]
	},
	{
		name: "STDEV.P",
		description: "Berekent de standaarddeviatie op basis van de volledige populatie die als argumenten wordt gegeven (logische waarden en tekst worden genegeerd).",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "getal1",
				description: "zijn maximaal 255 getallen die betrekking hebben op een populatie. Dit kunnen getallen zijn of verwijzingen die getallen bevatten"
			},
			{
				name: "getal2",
				description: "zijn maximaal 255 getallen die betrekking hebben op een populatie. Dit kunnen getallen zijn of verwijzingen die getallen bevatten"
			}
		]
	},
	{
		name: "STDEV.S",
		description: "Maakt een schatting van de standaarddeviatie op basis van een steekproef (logische waarden en tekst in de steekproef worden genegeerd).",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "getal1",
				description: "zijn maximaal 255 getallen die resulteren uit een steekproef onder een populatie. Dit kunnen getallen zijn of verwijzingen die getallen bevatten"
			},
			{
				name: "getal2",
				description: "zijn maximaal 255 getallen die resulteren uit een steekproef onder een populatie. Dit kunnen getallen zijn of verwijzingen die getallen bevatten"
			}
		]
	},
	{
		name: "STDEVA",
		description: "Maakt een schatting van de standaarddeviatie op basis van een steekproef, met inbegrip van logische waarden en tekst. Tekst en de logische waarde ONWAAR krijgen de waarde 0, de logische waarde WAAR krijgt de waarde 1.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "waarde1",
				description: "zijn 1 tot 255 waarden die resulteren uit een steekproef onder een populatie. Dit kunnen waarden zijn of namen of verwijzingen voor cellen die waarden bevatten"
			},
			{
				name: "waarde2",
				description: "zijn 1 tot 255 waarden die resulteren uit een steekproef onder een populatie. Dit kunnen waarden zijn of namen of verwijzingen voor cellen die waarden bevatten"
			}
		]
	},
	{
		name: "STDEVP",
		description: "Berekent de standaarddeviatie op basis van de volledige populatie die als argumenten worden gegeven (logische waarden en tekst worden genegeerd).",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "getal1",
				description: "zijn maximaal 255 getallen die betrekking hebben op een populatie. Dit kunnen getallen zijn of verwijzingen die getallen bevatten"
			},
			{
				name: "getal2",
				description: "zijn maximaal 255 getallen die betrekking hebben op een populatie. Dit kunnen getallen zijn of verwijzingen die getallen bevatten"
			}
		]
	},
	{
		name: "STDEVPA",
		description: "Berekent de standaarddeviatie op basis van de volledige populatie, inclusief logische waarden en tekst. Tekst en de logische waarde ONWAAR krijgen de waarde 0, de logische waarde WAAR krijgt de waarde 1.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "waarde1",
				description: "zijn 1 tot 255 waarden die betrekking hebben op een populatie. Dit kunnen waarden zijn of namen, matrices of verwijzingen voor cellen die waarden bevatten"
			},
			{
				name: "waarde2",
				description: "zijn 1 tot 255 waarden die betrekking hebben op een populatie. Dit kunnen waarden zijn of namen, matrices of verwijzingen voor cellen die waarden bevatten"
			}
		]
	},
	{
		name: "SUBSTITUEREN",
		description: "Vervangt bestaande tekst door nieuwe tekst in een tekenreeks.",
		arguments: [
			{
				name: "tekst",
				description: "is de tekst of een verwijzing naar de cel met de tekst waarin u een aantal tekens wilt vervangen"
			},
			{
				name: "oud_tekst",
				description: "is de tekst die u wilt vervangen. Als de hoofdletters en kleine letters in oud_tekst niet overeenkomt met die in tekst, wordt de tekst niet vervangen door SUBSTITUEREN"
			},
			{
				name: "nieuw_tekst",
				description: "is de tekst waardoor u oud_tekst wilt vervangen"
			},
			{
				name: "rang_getal",
				description: "geeft aan welke oud_tekst u wilt vervangen door nieuw_tekst. Als dit wordt weggelaten, wordt elke oud_tekst vervangen"
			}
		]
	},
	{
		name: "SUBTOTAAL",
		description: "Berekent een subtotaal in een lijst of database.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "functie_getal",
				description: "is een getal van 1 tot 11 waarmee u bepaalt welke samenvattingsfunctie u wilt gebruiken voor de berekening van het subtotaal."
			},
			{
				name: "verw1",
				description: "zijn maximaal 254 bereiken of verwijzingen waarvoor u een subtotaal wilt berekenen."
			}
		]
	},
	{
		name: "SYD",
		description: "Berekent de afschrijving van activa over een bepaalde termijn met behulp van de 'Sum-Of-The-Years-Digit'-methode.",
		arguments: [
			{
				name: "kosten",
				description: "zijn de aanschafkosten voor de activa"
			},
			{
				name: "restwaarde",
				description: "is de resterende waarde van de activa aan het einde van de levensduur van de activa"
			},
			{
				name: "duur",
				description: "is het aantal termijnen waarover de activa worden afgeschreven (ook wel levensduur van de activa genoemd)"
			},
			{
				name: "termijn",
				description: "is de termijn. Termijn moet in dezelfde eenheden worden opgegeven als duur"
			}
		]
	},
	{
		name: "T",
		description: "Controleert of een waarde tekst is. Als dit het geval is, wordt de tekst als resultaat gegeven. Als dit niet het geval is, worden er dubbele aanhalingstekens (lege tekst) als resultaat gegeven.",
		arguments: [
			{
				name: "waarde",
				description: "is de waarde die u wilt testen"
			}
		]
	},
	{
		name: "T.DIST",
		description: "Deze eigenschap retourneert de linkszijdige Student T-verdeling.",
		arguments: [
			{
				name: "x",
				description: "is de numerieke waarde waarop u de verdeling wilt evalueren"
			},
			{
				name: "vrijheidsgraden",
				description: "is een geheel getal voor het aantal vrijheidsgraden dat de verdeling kenmerkt"
			},
			{
				name: "cumulatief",
				description: "is een logische waarde die bepaalt welke functie als resultaat wordt gegeven: WAAR = de cumulatieve verdelingsfunctie, ONWAAR = de kansdichtheidsfunctie"
			}
		]
	},
	{
		name: "T.INV",
		description: "Berekent de linkszijdige inverse van de Student T-verdeling.",
		arguments: [
			{
				name: "kans",
				description: "is een getal van 0 tot en met 1 voor de kans die samenhangt met de tweezijdige Student T-verdeling"
			},
			{
				name: "vrijheidsgraden",
				description: "is een positief geheel getal voor het aantal vrijheidsgraden van de verdeling"
			}
		]
	},
	{
		name: "T.INV.2T",
		description: "Berekent de tweezijdige inverse van de Student T-verdeling.",
		arguments: [
			{
				name: "kans",
				description: "is een getal van 0 tot en met 1 voor de kans die samenhangt met de tweezijdige Student T-verdeling"
			},
			{
				name: "vrijheidsgraden",
				description: "is een positief geheel getal voor het aantal vrijheidsgraden van de verdeling"
			}
		]
	},
	{
		name: "T.TEST",
		description: "Berekent de kans met behulp van de Student T-toets.",
		arguments: [
			{
				name: "matrix1",
				description: "is de eerste gegevensverzameling"
			},
			{
				name: "matrix2",
				description: "is de tweede gegevensverzameling"
			},
			{
				name: "zijden",
				description: "geeft de verdelingstoets aan waarvan u uitgaat: eenzijdige toets = 1, tweezijdige toets = 2"
			},
			{
				name: "type_getal",
				description: "is het type T-toets dat u wilt uitvoeren: gepaard = 1, twee steekproeven met gelijke varianties = 2, twee steekproeven met ongelijke varianties = 3"
			}
		]
	},
	{
		name: "T.TOETS",
		description: "Berekent de kans met behulp van de Student T-toets.",
		arguments: [
			{
				name: "matrix1",
				description: "is de eerste gegevensverzameling"
			},
			{
				name: "matrix2",
				description: "is de tweede gegevensverzameling"
			},
			{
				name: "zijden",
				description: "geeft de verdelingstoets aan waarvan u uitgaat: eenzijdige toets = 1, tweezijdige toets = 2"
			},
			{
				name: "type_getal",
				description: "is het type T-toets dat u wilt uitvoeren: gepaard = 1, twee steekproeven met gelijke varianties = 2, twee steekproeven met ongelijke varianties = 3"
			}
		]
	},
	{
		name: "T.VERD",
		description: "Berekent de Student T-verdeling.",
		arguments: [
			{
				name: "x",
				description: "is de numerieke waarde waarop u de verdeling wilt evalueren"
			},
			{
				name: "vrijheidsgraden",
				description: "is een geheel getal voor het aantal vrijheidsgraden dat de verdeling kenmerkt"
			},
			{
				name: "zijden",
				description: "geeft de verdelingstoets aan waarvan u uitgaat: eenzijdige toets = 1, tweezijdige toets = 2"
			}
		]
	},
	{
		name: "T.VERD.2T",
		description: "Berekent de tweezijdige Student T-verdeling.",
		arguments: [
			{
				name: "x",
				description: "is de numerieke waarde waarvoor u de verdeling wilt evalueren"
			},
			{
				name: "vrijheidsgraden",
				description: "is een geheel getal dat het aantal vrijheidsgraden aangeeft dat de verdeling kenmerkt"
			}
		]
	},
	{
		name: "T.VERD.RECHTS",
		description: "Berekent de rechtszijdige Student T-verdeling.",
		arguments: [
			{
				name: "x",
				description: "is de numerieke waarde waarvoor u de verdeling wilt evalueren"
			},
			{
				name: "vrijheidsgraden",
				description: "is een geheel getal dat het aantal vrijheidsgraden aangeeft dat de verdeling kenmerkt"
			}
		]
	},
	{
		name: "TAN",
		description: "Berekent de tangens van een getal.",
		arguments: [
			{
				name: "getal",
				description: "is de hoek in radialen waarvan u de tangens wilt berekenen. Graden * PI()/180 = radialen"
			}
		]
	},
	{
		name: "TANH",
		description: "Berekent de tangens hyperbolicus van een getal.",
		arguments: [
			{
				name: "getal",
				description: "is een reëel getal"
			}
		]
	},
	{
		name: "TEKEN",
		description: "Geeft als resultaat het teken dat hoort bij de opgegeven code voor de tekenset van uw computer.",
		arguments: [
			{
				name: "getal",
				description: "is een getal tussen 1 en 255 dat aangeeft welk teken u wilt gebruiken"
			}
		]
	},
	{
		name: "TEKST",
		description: "Converteert een waarde naar tekst in een specifieke getalnotatie.",
		arguments: [
			{
				name: "waarde",
				description: "is een getal, een formule die resulteert in een getal of een verwijzing naar een cel die een getal bevat"
			},
			{
				name: "notatie_tekst",
				description: "is een getalnotatie in de vorm van tekst uit het vak Categorie op het tabblad Getal in het dialoogvenster Celeigenschappen (niet de notatie Standaard)"
			}
		]
	},
	{
		name: "TEKST.SAMENVOEGEN",
		description: "Voegt verschillende tekenreeksen samen tot één tekenreeks.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "tekst1",
				description: "zijn 1 tot 255 tekenreeksen die u wilt samenvoegen tot één tekenreeks. Dit kunnen tekenreeksen, getallen of verwijzingen naar één cel zijn"
			},
			{
				name: "tekst2",
				description: "zijn 1 tot 255 tekenreeksen die u wilt samenvoegen tot één tekenreeks. Dit kunnen tekenreeksen, getallen of verwijzingen naar één cel zijn"
			}
		]
	},
	{
		name: "TIJD",
		description: "Converteert uren, minuten en seconden die als getallen zijn opgegeven naar seriële getallen in de tijdnotatie.",
		arguments: [
			{
				name: "uur",
				description: "is een getal tussen 0 en 23 dat het uur aangeeft"
			},
			{
				name: "minuut",
				description: "is een getal tussen 0 en 59 dat de minuut aangeeft"
			},
			{
				name: "seconde",
				description: "is een getal tussen 0 en 59 dat de seconde aangeeft"
			}
		]
	},
	{
		name: "TIJDWAARDE",
		description: "Converteert een tijd in tekstnotatie naar een serieel getal voor tijd, een getal van 0 (12:00:00) tot 0,999988426 (23:59:59). Pas na het opgeven van de formule een tijdnotatie toe op het getal.",
		arguments: [
			{
				name: "tijd_tekst",
				description: "is een tekenreeks die een tijd aangeeft in een van de tijdnotaties van Spreadsheet (datuminformatie in de reeks wordt genegeerd)"
			}
		]
	},
	{
		name: "TINV",
		description: "Berekent de tweezijdige inverse van de Student T-verdeling.",
		arguments: [
			{
				name: "kans",
				description: "is een getal van 0 tot en met 1 voor de kans die samenhangt met de tweezijdige Student T-verdeling"
			},
			{
				name: "vrijheidsgraden",
				description: "is een positief geheel getal voor het aantal vrijheidsgraden van de verdeling"
			}
		]
	},
	{
		name: "TOEK.WAARDE2",
		description: "Berekent de toekomstige waarde van een aanvangshoofdsom nadat de samengestelde rente eraan is toegevoegd.",
		arguments: [
			{
				name: "hoofdsom",
				description: "is de huidige waarde"
			},
			{
				name: "rente_waarden",
				description: "is een matrix met de rentepercentages die moeten worden toegepast"
			}
		]
	},
	{
		name: "TRANSPONEREN",
		description: "Converteert een verticaal celbereik naar een horizontaal bereik en omgekeerd.",
		arguments: [
			{
				name: "matrix",
				description: "is een celbereik op een werkblad of een matrix met waarden die u wilt transponeren"
			}
		]
	},
	{
		name: "TREND",
		description: "Geeft als resultaat getallen in een lineaire trend die overeenkomen met bekende gegevenspunten, berekend met de kleinste-kwadratenmethode.",
		arguments: [
			{
				name: "y-bekend",
				description: "is een bereik of matrix met Y-waarden die al bekend is uit de relatie y = mx + b"
			},
			{
				name: "x-bekend",
				description: "is het optionele bereik of de optionele matrix X-waarden die al bekend is uit de relatie y = mx + b, een matrix van dezelfde grootte als y_bekend"
			},
			{
				name: "x-nieuw",
				description: "is een bereik of matrix met de nieuwe X-waarden waarvoor TREND als resultaat de bijbehorende Y-waarde moet geven"
			},
			{
				name: "const",
				description: "is een logische waarde. Als Const = WAAR of wordt weggelaten, wordt de constante b normaal berekend. Als Const = ONWAAR, moet b gelijk zijn aan 0"
			}
		]
	},
	{
		name: "TW",
		description: "Berekent de toekomstige waarde van een investering, gebaseerd op periodieke, constante betalingen en een constant rentepercentage.",
		arguments: [
			{
				name: "rente",
				description: "is het rentepercentage per termijn. Gebruik bijvoorbeeld 6%/4 voor kwartaalbetalingen met een rentepercentage van 6%"
			},
			{
				name: "aantal-termijnen",
				description: "is het totale aantal betalingstermijnen van een investering"
			},
			{
				name: "bet",
				description: "is de betaling die elke termijn wordt verricht. Dit bedrag kan gedurende de looptijd van de investering niet worden gewijzigd"
			},
			{
				name: "hw",
				description: "is de huidige waarde of het totaalbedrag dat een reeks toekomstige waarden op dit moment waard is. Als dit wordt weggelaten, wordt uitgegaan van Hw = 0"
			},
			{
				name: "type_getal",
				description: "is een waarde die aangeeft wanneer de betalingen voldaan moeten worden. 1 = betaling aan het begin van de periode. 0 of weggelaten = betaling aan het einde van de periode"
			}
		]
	},
	{
		name: "TYPE",
		description: "Geeft als resultaat een geheel getal dat het gegevenstype van de waarde aangeeft: getal = 1; tekst = 2; logische waarde = 4, formule = 8; foutwaarde = 16; matrix = 64.",
		arguments: [
			{
				name: "waarde",
				description: "kan elke waarde zijn"
			}
		]
	},
	{
		name: "TYPE.FOUT",
		description: "Geeft als resultaat een nummer dat overeenkomt met een foutwaarde.",
		arguments: [
			{
				name: "foutwaarde",
				description: "is de foutwaarde waarvan u het identificatienummer wilt weten. Dit kan een werkelijke foutwaarde zijn of een verwijzing naar een cel die een foutwaarde bevat"
			}
		]
	},
	{
		name: "UNICODE",
		description: "Geeft als resultaat het getal (codepunt) dat overeenkomt met het eerste teken van de tekst.",
		arguments: [
			{
				name: "tekst",
				description: "is het teken waarvoor u de Unicode-waarde wilt weten"
			}
		]
	},
	{
		name: "URL.CODEREN",
		description: "Geeft als resultaat een tekenreeks met URL-codering.",
		arguments: [
			{
				name: "tekst",
				description: "is een tekenreeks die URL-codering moet bevatten"
			}
		]
	},
	{
		name: "UUR",
		description: "Geeft als resultaat het aantal uren als een getal van 0 (00:00) tot 23 (23:00).",
		arguments: [
			{
				name: "serieel-getal",
				description: "is een getal dat een dag of tijd aangeeft in het systeem dat in Spreadsheet wordt gebruikt voor datum- en tijdberekeningen of tekst in tijdnotatie, zoals 16:48:00 of 4:48:00 PM"
			}
		]
	},
	{
		name: "VANDAAG",
		description: "Geeft als resultaat de huidige datum in de datumnotatie.",
		arguments: [
		]
	},
	{
		name: "VAR",
		description: "Maakt een schatting van de variantie op basis van een steekproef (logische waarden en tekst in de steekproef worden genegeerd).",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "getal1",
				description: "zijn maximaal 255 numerieke argumenten die resulteren uit een steekproef onder een populatie"
			},
			{
				name: "getal2",
				description: "zijn maximaal 255 numerieke argumenten die resulteren uit een steekproef onder een populatie"
			}
		]
	},
	{
		name: "VAR.P",
		description: "Berekent de variantie op basis van de volledige populatie (logische waarden en tekst in de populatie worden genegeerd).",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "getal1",
				description: "zijn maximaal 255 numerieke argumenten die betrekking hebben op een populatie"
			},
			{
				name: "getal2",
				description: "zijn maximaal 255 numerieke argumenten die betrekking hebben op een populatie"
			}
		]
	},
	{
		name: "VAR.S",
		description: "Maakt een schatting van de variantie op basis van een steekproef (logische waarden en tekst in de steekproef worden genegeerd).",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "getal1",
				description: "zijn maximaal 255 numerieke argumenten die resulteren uit een steekproef onder een populatie"
			},
			{
				name: "getal2",
				description: "zijn maximaal 255 numerieke argumenten die resulteren uit een steekproef onder een populatie"
			}
		]
	},
	{
		name: "VARA",
		description: "Maakt een schatting van de variantie op basis van een steekproef, inclusief logische waarden en tekst. Tekst en de logische waarde ONWAAR krijgen de waarde 0, de logische waarde WAAR krijgt de waarde 1.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "waarde1",
				description: "zijn 1 tot 255 numerieke argumenten die resulteren uit een steekproef onder een populatie"
			},
			{
				name: "waarde2",
				description: "zijn 1 tot 255 numerieke argumenten die resulteren uit een steekproef onder een populatie"
			}
		]
	},
	{
		name: "VARP",
		description: "Berekent de variantie op basis van de volledige populatie (logische waarden en tekst in de populatie worden genegeerd).",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "getal1",
				description: "zijn maximaal 255 numerieke argumenten die betrekking hebben op een populatie"
			},
			{
				name: "getal2",
				description: "zijn maximaal 255 numerieke argumenten die betrekking hebben op een populatie"
			}
		]
	},
	{
		name: "VARPA",
		description: "Berekent de variantie op basis van de volledige populatie, inclusief logische waarden en tekst. Tekst en de logische waarde ONWAAR krijgen de waarde 0, de logische waarde WAAR krijgt de waarde 1.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "waarde1",
				description: "zijn 1 tot 255 numerieke argumenten die betrekking hebben op een populatie"
			},
			{
				name: "waarde2",
				description: "zijn 1 tot 255 numerieke argumenten die betrekking hebben op een populatie"
			}
		]
	},
	{
		name: "VAST",
		description: "Rondt een getal af op het opgegeven aantal decimalen en geeft het resultaat weer als tekst met of zonder komma's.",
		arguments: [
			{
				name: "getal",
				description: "is het getal dat u wilt afronden en converteren naar tekst"
			},
			{
				name: "decimalen",
				description: "is het aantal decimalen rechts van de decimale komma. Als u dit weglaat, is Decimalen = 2"
			},
			{
				name: "geen-punten",
				description: "is een logische waarde. Geen punten weergeven in de resulterende tekst = WAAR. Punten weergeven in de resulterende tekst = ONWAAR of weggelaten"
			}
		]
	},
	{
		name: "VDB",
		description: "Berekent de afschrijving van activa over een opgegeven periode, ook delen van perioden, met behulp van de 'variable declining balance'-methode of met een andere methode die u opgeeft.",
		arguments: [
			{
				name: "kosten",
				description: "zijn de aanschafkosten van de activa"
			},
			{
				name: "restwaarde",
				description: "is de resterende waarde van de activa aan het einde van de levensduur van de activa"
			},
			{
				name: "duur",
				description: "is het aantal termijnen waarover de activa worden afgeschreven (ook wel levensduur van de activa genoemd)"
			},
			{
				name: "begin-periode",
				description: "is het begin van een periode waarvoor u de afschrijving wilt berekenen. Dit moet in dezelfde eenheden zijn als Duur"
			},
			{
				name: "einde-periode",
				description: "is het einde van een periode waarvoor u de afschrijving wilt berekenen. Dit moet in dezelfde eenheden zijn als Duur"
			},
			{
				name: "factor",
				description: "is de snelheid waarmee wordt afgeschreven. Als dit wordt weggelaten, wordt uitgegaan van 2 ('double declining balance'-methode)"
			},
			{
				name: "geen-omschakeling",
				description: "ONWAAR of weggelaten = overschakelen op een lineaire afschrijving wanneer deze afschrijving groter is dan de afschrijving volgens de Declining Balance-methode, WAAR = niet overschakelen"
			}
		]
	},
	{
		name: "VERGELIJKEN",
		description: "Geeft als resultaat de relatieve positie in een matrix van een item dat overeenkomt met een opgegeven waarde in een opgegeven volgorde.",
		arguments: [
			{
				name: "zoekwaarde",
				description: "is de waarde die u gebruikt om een waarde in de matrix te zoeken. De waarde kan een getal zijn, tekst, een logische waarde of een verwijzing naar een van deze typen"
			},
			{
				name: "zoeken-matrix",
				description: "is een aaneengesloten celbereik met mogelijke zoekwaarden. Dit kunnen matrices met waarden zijn of verwijzingen naar een matrix"
			},
			{
				name: "criteriumtype_getal",
				description: "is het getal 1, 0 of -1 dat aangeeft welke waarde het resultaat moet hebben."
			}
		]
	},
	{
		name: "VERSCHUIVING",
		description: "Geeft als resultaat een verwijzing naar een bereik dat een opgegeven aantal rijen en kolommen van een opgegeven verwijzing is.",
		arguments: [
			{
				name: "verw",
				description: "is de verwijzing waarop u de verschuiving wilt baseren. Dit is een verwijzing naar een cel of een bereik aaneengesloten cellen"
			},
			{
				name: "rijen",
				description: "is het aantal rijen, omhoog of omlaag, waarnaar u de cel in de linkerbovenhoek van het resultaat wilt laten verwijzen"
			},
			{
				name: "kolommen",
				description: "is het aantal kolommen, naar links of naar rechts, waarnaar u de cel in de linkerbovenhoek van het resultaat wilt laten verwijzen"
			},
			{
				name: "hoogte",
				description: "is het aantal rijen dat u wilt dat het resultaat hoog is. Als dit wordt weggelaten, wordt uitgegaan van dezelfde hoogte als Verw"
			},
			{
				name: "breedte",
				description: "is het aantal kolommen dat u wilt dat het resultaat breed is. Als dit wordt weggelaten, wordt uitgegaan van dezelfde breedte als Verw"
			}
		]
	},
	{
		name: "VERT.ZOEKEN",
		description: "Zoekt in de meest linkse kolom van een matrix naar een bepaalde waarde en geeft als resultaat de waarde uit dezelfde rij in een opgegeven kolom. Standaard moet de tabel in oplopende volgorde worden gesorteerd.",
		arguments: [
			{
				name: "zoekwaarde",
				description: "is de waarde die u wilt zoeken in de eerste kolom van de matrix. Dit kan een waarde zijn, een verwijzing of een tekenreeks"
			},
			{
				name: "tabelmatrix",
				description: "is een tabel met tekst, getallen of logische waarden, waarin u naar gegevens zoekt. Tabelmatrix kan een verwijzing zijn naar een bereik of de naam van een bereik"
			},
			{
				name: "kolomindex_getal",
				description: "is het nummer van de kolom in tabelmatrix waaruit u de waarde wilt ophalen. De eerste waardekolom in de tabel is kolom 1"
			},
			{
				name: "benaderen",
				description: "is een logische waarde: WAAR of weggelaten = zoek de best mogelijke waarde in de eerste kolom (gesorteerd in oplopende volgorde), ONWAAR = de gevonden waarde moet exact overeenkomen"
			}
		]
	},
	{
		name: "VERTROUWELIJKHEID.NORM",
		description: "Berekent het betrouwbaarheidsinterval van een gemiddelde waarde voor de elementen van een populatie met een normale verdeling.",
		arguments: [
			{
				name: "alfa",
				description: "is het significantieniveau op basis waarvan de betrouwbaarheid wordt berekend. Dit is een getal groter dan 0 en kleiner dan 1"
			},
			{
				name: "standaarddev",
				description: "is de standaarddeviatie voor het gegevensbereik binnen de populatie. Deze wordt verondersteld bekend te zijn. Standaarddev moet groter dan 0 zijn"
			},
			{
				name: "grootte",
				description: "is de grootte van de steekproef"
			}
		]
	},
	{
		name: "VERTROUWELIJKHEID.T",
		description: "Berekent de betrouwbaarheidsinterval van een gemiddelde waarde voor de elementen van een populatie, met behulp van een Student T-verdeling.",
		arguments: [
			{
				name: "alfa",
				description: "is het significantieniveau op basis waarvan de betrouwbaarheid wordt berekend. Dit is een getal groter dan 0 en kleiner dan 1"
			},
			{
				name: "standaarddev",
				description: "is de standaarddeviatie voor het gegevensbereik binnen de populatie. Deze wordt verondersteld bekend te zijn. Standaarddev moet groter dan 0 zijn"
			},
			{
				name: "grootte",
				description: "is de grootte van de steekproef"
			}
		]
	},
	{
		name: "VERVANGEN",
		description: "Vervangt een deel van een tekenreeks door een andere tekenreeks.",
		arguments: [
			{
				name: "oud_tekst",
				description: "is tekst waarvan u een aantal tekens wilt vervangen"
			},
			{
				name: "begin_getal",
				description: "is de positie van het teken in oud_tekst dat u door nieuw_tekst wilt vervangen"
			},
			{
				name: "aantal-tekens",
				description: "is het aantal tekens in oud_tekst die u wilt vervangen"
			},
			{
				name: "nieuw_tekst",
				description: "is de tekst waardoor u tekens in oud_tekst wilt vervangen"
			}
		]
	},
	{
		name: "VIND.ALLES",
		description: "Geeft als resultaat de beginpositie van een tekenreeks binnen een andere tekenreeks (er wordt onderscheid gemaakt tussen hoofdletters en kleine letters).",
		arguments: [
			{
				name: "zoeken_tekst",
				description: "is de tekst die u zoekt. Gebruik dubbele aanhalingstekens (een lege tekenreeks) om het eerste teken in In_tekst te zoeken. Jokertekens zijn niet toegestaan"
			},
			{
				name: "in_tekst",
				description: "is de tekst die de door u gezochte tekenreeks bevat"
			},
			{
				name: "begin_getal",
				description: "geeft de positie aan van het teken waar u wilt beginnen met zoeken. Het eerste teken in In_tekst is teken nummer 1. Als u dit weglaat, is Begin_getal = 1"
			}
		]
	},
	{
		name: "VIND.SPEC",
		description: "Geeft als resultaat de positie van het teken, lezend van links naar rechts, waar een bepaald teken of een bepaalde tekenreeks de eerste keer wordt gevonden (zonder onderscheid tussen hoofdletters en kleine letters).",
		arguments: [
			{
				name: "zoeken_tekst",
				description: "is de tekenreeks die u zoekt. U kunt de jokertekens ? en * gebruiken. Gebruik ~? en ~* om de tekens ? en * te zoeken"
			},
			{
				name: "in_tekst",
				description: "is de tekst die de door u gezochte tekenreeks bevat"
			},
			{
				name: "begin_getal",
				description: "geeft de positie, tellend van links naar rechts, aan in in_tekst van het teken waar u wilt beginnen met zoeken. Als dit wordt weggelaten, wordt 1 gebruikt"
			}
		]
	},
	{
		name: "VOORSPELLEN",
		description: "Berekent of voorspelt aan de hand van bestaande waarden een toekomstige waarde volgens een lineaire trend.",
		arguments: [
			{
				name: "x",
				description: "is de numerieke waarde voor het gegevenspunt waarvan u de waarde wilt voorspellen"
			},
			{
				name: "y-bekend",
				description: "is de afhankelijke matrix of het afhankelijke bereik met numerieke gegevens"
			},
			{
				name: "x-bekend",
				description: "is de onafhankelijke matrix of het onafhankelijke bereik met numerieke gegevens. De variantie van waarden voor X-bekend mag niet nul zijn"
			}
		]
	},
	{
		name: "WAAR",
		description: "Geeft als resultaat de logische waarde WAAR.",
		arguments: [
		]
	},
	{
		name: "WAARDE",
		description: "Converteert een tekenreeks die overeenkomt met een getal naar een getal.",
		arguments: [
			{
				name: "tekst",
				description: "is de tekst tussen aanhalingstekens of een verwijzing naar een cel met de tekst die u wilt converteren"
			}
		]
	},
	{
		name: "WEEKDAG",
		description: "Geeft als resultaat een getal van 1 tot 7 dat de dag van de week van een datum aangeeft.",
		arguments: [
			{
				name: "serieel-getal",
				description: "is het seriële getal dat een datum aangeeft"
			},
			{
				name: "type_getal",
				description: "is een getal: 1 = zondag is 1 tot en met zaterdag is 7, 2 = maandag is 1 tot en met zondag is 7, 3 = maandag is 0 tot en met zondag is 6"
			}
		]
	},
	{
		name: "WEEKNUMMER",
		description: "Zet een serieel getal om in een weeknummer.",
		arguments: [
			{
				name: "serieel_getal",
				description: "is het seriële getal dat een dag of tijd aangeeft in het systeem dat Spreadsheet gebruikt voor datum- en tijdberekeningen"
			},
			{
				name: "type_resultaat",
				description: "is een getal (1 of 2) waarmee u het type waarde bepaalt dat u als resultaat wilt hebben"
			}
		]
	},
	{
		name: "WEIBULL",
		description: "Geeft als resultaat de Weibull-verdeling.",
		arguments: [
			{
				name: "x",
				description: "is een niet-negatief getal voor de waarde waarop de functie geëvalueerd moet worden"
			},
			{
				name: "alfa",
				description: "is de parameter voor de verdeling. Dit is een positief getal."
			},
			{
				name: "bèta",
				description: "is de parameter voor de verdeling. Dit is een positief getal."
			},
			{
				name: "cumulatief",
				description: "is een logische waarde. Gebruik WAAR voor de cumulatieve verdelingsfunctie en ONWAAR voor de kansdichtsheidsfunctie"
			}
		]
	},
	{
		name: "WEIBULL.VERD",
		description: "Geeft als resultaat de Weibull-verdeling.",
		arguments: [
			{
				name: "x",
				description: "is een niet-negatief getal voor de waarde waarop de functie geëvalueerd moet worden"
			},
			{
				name: "alfa",
				description: "is de parameter voor de verdeling. Dit is een positief getal."
			},
			{
				name: "beta",
				description: "is de parameter voor de verdeling. Dit is een positief getal."
			},
			{
				name: "cumulatief",
				description: "is een logische waarde. Gebruik WAAR voor de cumulatieve verdelingsfunctie en ONWAAR voor de kansdichtsheidsfunctie"
			}
		]
	},
	{
		name: "WERKDAG",
		description: "Geeft het seriële getal van de datum voor of na een opgegeven aantal werkdagen.",
		arguments: [
			{
				name: "begindatum",
				description: "is een serieel getal dat de begindatum aangeeft"
			},
			{
				name: "aantal_dagen",
				description: "is het aantal werkdagen voor of na de begindatum"
			},
			{
				name: "vakantiedagen",
				description: "is een optionele matrix met een of meer seriële getallen die de datums aangeven waarop niet wordt gewerkt, zoals nationale feestdagen en vakantiedagen"
			}
		]
	},
	{
		name: "WERKDAG.INTL",
		description: "Geeft het seriële getal van de datum voor of na een opgegeven aantal werkdagen, met aangepaste weekendparameters.",
		arguments: [
			{
				name: "begindatum",
				description: "is een serieel getal dat de begindatum aangeeft"
			},
			{
				name: "dagen",
				description: "is het aantal werkdagen voor of na de begindatum"
			},
			{
				name: "weekend",
				description: "is een getal of tekenreeks waarmee weekends worden aangegeven"
			},
			{
				name: "vakantiedagen",
				description: "is een optionele matrix van een of meer seriële getallen die de datums aangeven waarop niet wordt gewerkt, zoals nationale feestdagen en vakantiedagen"
			}
		]
	},
	{
		name: "WISSEN.CONTROL",
		description: "Verwijdert alle niet-afdrukbare tekens uit een tekst.",
		arguments: [
			{
				name: "tekst",
				description: "is de werkbladinformatie waaruit u niet-afdrukbare tekens wilt verwijderen"
			}
		]
	},
	{
		name: "WORTEL",
		description: "Berekent de vierkantswortel van een getal.",
		arguments: [
			{
				name: "getal",
				description: "is het getal waarvan u de vierkantswortel wilt berekenen"
			}
		]
	},
	{
		name: "WORTEL.PI",
		description: "Berekent de vierkantswortel van (getal * pi).",
		arguments: [
			{
				name: "getal",
				description: "is het getal waarmee pi wordt vermenigvuldigd"
			}
		]
	},
	{
		name: "Z.TEST",
		description: "Berekent de eenzijdige P-waarde voor een Z-toets.",
		arguments: [
			{
				name: "matrix",
				description: "is de matrix of het gegevensbereik waartegen u x wilt testen"
			},
			{
				name: "x",
				description: "is de waarde die u wilt testen"
			},
			{
				name: "sigma",
				description: "is de (al bekende) standaarddeviatie voor de hele populatie. Als u deze niet opgeeft, wordt de standaarddeviatie voor de steekproef gebruikt"
			}
		]
	},
	{
		name: "Z.TOETS",
		description: "Berekent de eenzijdige P-waarde voor een Z-toets.",
		arguments: [
			{
				name: "matrix",
				description: "is de matrix of het gegevensbereik waartegen u x wilt testen"
			},
			{
				name: "x",
				description: "is de waarde die u wilt testen"
			},
			{
				name: "sigma",
				description: "is de (al bekende) standaarddeviatie voor de hele populatie. Als u deze niet opgeeft, wordt de standaarddeviatie voor de steekproef gebruikt"
			}
		]
	},
	{
		name: "ZELFDE.DAG",
		description: "Zet de datum die het opgegeven aantal maanden voor of na de begindatum ligt, om in een serieel getal.",
		arguments: [
			{
				name: "begindatum",
				description: "is een serieel getal dat de begindatum aangeeft"
			},
			{
				name: "aantal_maanden",
				description: "is het aantal maanden voor of na de begindatum"
			}
		]
	},
	{
		name: "ZOEKEN",
		description: "Zoekt een waarde uit een bereik met één rij of één kolom of uit een matrix. De functie is achterwaarts compatibel.",
		arguments: [
			{
				name: "zoekwaarde",
				description: "is de waarde die ZOEKEN in Zoekvector moet zoeken. Dit kan een getal zijn, tekst, een logische waarde of een naam of een verwijzing naar een waarde"
			},
			{
				name: "zoekvector",
				description: "is een celbereik dat slechts één rij of één kolom bevat met tekst, getallen of logische waarden, geplaatst in oplopende volgorde"
			},
			{
				name: "resultaatvector",
				description: "is een celbereik dat slechts één rij of één kolom bevat van dezelfde grootte als Zoekvector"
			}
		]
	}
];