// Create a profanity filter module
const ProfanityFilter = {
  // Core word lists
  words: new Set([
    // English words
    "anal",
    "anus",
    "arse",
    "ass",
    "ballsack",
    "balls",
    "bastard",
    "bitch",
    "biatch",
    "bloody",
    "blowjob",
    "bollock",
    "bollok",
    "boner",
    "boob",
    "bugger",
    "bum",
    "butt",
    "buttplug",
    "cock",
    "coon",
    "crap",
    "cunt",
    "damn",
    "dick",
    "dildo",
    "dyke",
    "fag",
    "feck",
    "fellate",
    "fellatio",
    "felching",
    "fuck",
    "fucking",
    "fucked",
    "fuckhead",
    "fudgepacker",
    "fudger",
    "faggot",
    "flange",
    "goddamn",
    "hell",
    "homo",
    "jerk",
    "jizz",
    "knobend",
    "labia",
    "muff",
    "nigger",
    "nigga",
    "penis",
    "piss",
    "poop",
    "prick",
    "pube",
    "pussy",
    "queer",
    "scrotum",
    "sex",
    "shit",
    "slut",
    "smegma",
    "spunk",
    "tit",
    "tosser",
    "turd",
    "twat",
    "vagina",
    "wank",
    "whore",
    "wtf",

    // Russian words
    "блядь",
    "blyad",
    "говно",
    "govno",
    "ебать",
    "yebat",
    "ёб",
    "yob",
    "заебать",
    "zayebat",
    "заебись",
    "zayebis",
    "жопа",
    "zhopa",
    "козёл",
    "kozyol",
    "мудила",
    "mudila",
    "мудак",
    "mudak",
    "нахуй",
    "nakhuy",
    "пидорас",
    "pidoras",
    "пизда",
    "pizda",
    "пиздец",
    "pizdets",
    "срать",
    "srat",
    "сука",
    "suka",
    "хер",
    "kher",
    "хуй",
    "khuy",
    "хрен",
    "khren",
    "шлюха",
    "shlyukha",
    "шалава",
    "shalava",
    "урод",
    "urod",
    "выебываться",
    "vyeobyvatsya",
    "ебанутый",
    "yebanutyi",
    "обосранец",
    "obosranets",
    "мразь",
    "mraz",
    "дрянь",
    "dryan",
    "подонок",
    "podonok",
    "дебил",
    "debil",
    "сукин сын",
    "sukin syn",
    "проститутка",
    "prostitutka",
    "зануда",
    "zanuda",
    "мерзавец",
    "merzavets",
    "скотина",
    "skotina",
    "баран",
    "baran",
    "дерьмо",
    "dermo",
    "сволочь",
    "svoloch",
    "гадина",
    "gadina",
    "чмо",
    "chmo",
    "сосунок",
    "sosunok",

    // Latvian words
    "pimpis",
    "pidars",
    "dirsa",
    "sūds",
    "nolādēts",
    "idiņš",
    "pamuļķis",
    "mīkstais",
    "kretīns",
    "muļķis",
    "blēdis",
    "nelga",
    "sīkstulis",
    "kuce",
    "kuces dēls",
    "maita",
    "mērgli",
    "sātans",
    "naba",
    "plikpauris",
    "truls",
    "stulbenis",
    "nelietis",
    "pļāpa",
    "mūdzis",
    "nelaime",
    "vārgulis",
    "lāsts",
    "riebeklis",
    "skauģis",
    "pakaļa",
    "tizlenis",
    "āksts",
    "mēms",
    "stulbs",
    "prātiņš",
    "pļumpis",
    "gausis",
    "plānprātiņš",
  ]),

  // Special patterns that need more complex matching
  patterns: [
    // N-word variations
    /\bn[i1l|]+g+[e3]+r+s?\b/i, // Catches variations like n1gger, nigger, etc.
    /\bn[i1l|]+g+[a@]+s?\b/i, // Catches variations like n1gga, nigga, etc.
    /\bn[i1l|]+g+[e3]+r+\w*/i, // Captures variations like n1ggerdog, niggers
    /\bn[i1l|]+g+[a@]+s?\w*/i, // Captures variations like n1gga, niggadog
    /\bn[i1l|]+g+[z@s]+\b/i, // Catches variations like n1gz, niggs, etc.

    // Common letter substitutions for offensive words
    /\bf+u+c+k+\w*\b/i, // Catches fuck variations
    /\bb+i+t+c+h+\w*\b/i, // Catches bitch variations
    /\bc+u+n+t+\w*\b/i, // Catches cunt variations
    /\bp+u+s+s+y+\w*\b/i, // Catches pussy variations

    // Russian word patterns (for Latin alphabet attempts)
    /\bsuk[aа]+\b/i,
    /\bb+l+[yi]+[aа]+[dt]+\b/i,
  ].map((p) => new RegExp(p, "i")),

  messageCache: new Map(),
  wordCache: new Map(),

  // Check if a single word contains profanity
  checkWord(word) {
    const cleanWord = word.toLowerCase().trim();
    if (this.wordCache.has(cleanWord)) return this.wordCache.get(cleanWord);

    const isProfane =
      this.words.has(cleanWord) || this.patterns.some((p) => p.test(cleanWord));
    this.wordCache.set(cleanWord, isProfane);
    return isProfane;
  },

  // Filter a message
  filterMessage(message, messageId) {
    if (messageId && this.messageCache.has(messageId))
      return this.messageCache.get(messageId);

    const words = message.split(/\s+/);
    const profaneIndices = [];

    for (let i = 0; i < words.length; i++) {
      if (this.checkWord(words[i])) profaneIndices.push(i);
    }

    if (!profaneIndices.length) {
      messageId && this.messageCache.set(messageId, message);
      return message;
    }

    const filteredWords = [...words];
    profaneIndices.forEach((i) => {
      filteredWords[i] = "*".repeat(words[i].length);
    });

    const filtered = filteredWords.join(" ");
    messageId && this.messageCache.set(messageId, filtered);
    return filtered;
  },

  // Add custom words to the filter
  addWords(newWords) {
    newWords.forEach((word) => this.words.add(word.toLowerCase().trim()));
  },

  clearCache() {
    this.messageCache.clear();
    this.wordCache.clear();
  },

  limitCacheSize(maxSize = 1000) {
    if (this.messageCache.size > maxSize) {
      const entries = Array.from(this.messageCache.entries());
      const toRemove = entries.slice(0, entries.length - maxSize);
      toRemove.forEach(([key]) => this.messageCache.delete(key));
    }
    if (this.wordCache.size > maxSize) {
      const entries = Array.from(this.wordCache.entries());
      const toRemove = entries.slice(0, entries.length - maxSize);
      toRemove.forEach(([key]) => this.wordCache.delete(key));
    }
  },
};
