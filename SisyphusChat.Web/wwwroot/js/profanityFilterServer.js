const ProfanityFilterServer = {
  words: new Set([
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
  ]),

  patterns: [
    /\bn[i1l|]+g+[e3]+r+s?\b/i,
    /\bn[i1l|]+g+[a@]+s?\b/i,
    /\bn[i1l|]+g+[e3]+r+\w*/i,
    /\bn[i1l|]+g+[a@]+s?\w*/i,
    /\bn[i1l|]+g+[z@s]+\b/i,
    /\bf+u+c+k+\w*\b/i,
    /\bb+i+t+c+h+\w*\b/i,
    /\bc+u+n+t+\w*\b/i,
    /\bp+u+s+s+y+\w*\b/i,
  ],

  checkWord(word) {
    const cleanWord = word.toLowerCase().trim();
    if (this.words.has(cleanWord)) return true;
    return this.patterns.some((pattern) => pattern.test(cleanWord));
  },
};

if (typeof module !== "undefined" && module.exports) {
  module.exports = ProfanityFilterServer;
}
