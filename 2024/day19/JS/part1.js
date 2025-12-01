const fs = require("fs");
const path = require("path");
const INPUT_FILE = path.join(__dirname, "../input.txt");

fs.readFile(INPUT_FILE, "utf-8", (err, input) => {
    if (err) {
        console.log("Error reading file:", err);
        process.exit(1);
    }

    // Разделяем вход: до первой пустой строки — полотенца, потом — дизайны
    const [patternLine, ...rest] = input.trim().split("\n");
    const splitIndex = rest.findIndex(line => line.trim() === "");
    const patterns = patternLine.split(",").map(p => p.trim());

    const designs = rest.slice(splitIndex + 1).map(line => line.trim()).filter(Boolean);

    // Функция: можно ли собрать дизайн из полотенец
    function canMakeDesign(design, patterns) {
        const memo = new Map();

        function dfs(remaining) {
            if (remaining === "") return true;
            if (memo.has(remaining)) return memo.get(remaining);

            for (const pat of patterns) {
                if (remaining.startsWith(pat)) {
                    if (dfs(remaining.slice(pat.length))) {
                        memo.set(remaining, true);
                        return true;
                    }
                }
            }

            memo.set(remaining, false);
            return false;
        }

        return dfs(design);
    }

    // Подсчёт количества возможных дизайнов
    let count = 0;
    for (const design of designs) {
        if (canMakeDesign(design, patterns)) {
            count++;
        }
    }

    console.log("Possible designs:", count);
});