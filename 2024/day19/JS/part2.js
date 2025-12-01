const fs = require("fs");
const path = require("path");
const INPUT_FILE = path.join(__dirname, "../input.txt");

fs.readFile(INPUT_FILE, "utf-8", (err, input) => {
    if (err) {
        console.log("Error reading file:", err);
        process.exit(1);
    }

    const [patternLine, ...rest] = input.trim().split("\n");
    const splitIndex = rest.findIndex(line => line.trim() === "");
    const patterns = patternLine.split(",").map(p => p.trim());
    const designs = rest.slice(splitIndex + 1).map(line => line.trim()).filter(Boolean);

    function countWays(design, patterns) {
        const memo = new Map();

        function dfs(remaining) {
            if (remaining === "") return 1;
            if (memo.has(remaining)) return memo.get(remaining);

            let totalWays = 0;

            for (const pat of patterns) {
                if (remaining.startsWith(pat)) {
                    totalWays += dfs(remaining.slice(pat.length));
                }
            }

            memo.set(remaining, totalWays);
            return totalWays;
        }

        return dfs(design);
    }

    let total = 0;
    for (const design of designs) {
        const ways = countWays(design, patterns);
        console.log(`${design}: ${ways} way(s)`);
        total += ways;
    }

    console.log("Total different ways:", total);
});