const fs = require("fs");
const path = require("path");

const INPUT_FILE = path.join(__dirname, "../input.txt");

const operators = ["+", "*", "|"];

function checkCombinationsForTarget(numbers, target) {
    let operatorCombinations = operators.map((op) => [op]);
    for (let i = 0; i < numbers.length - 2; i++) {
        operatorCombinations = operatorCombinations.flatMap((combination) =>
            operators.map((op) => [...combination, op])
        );
    }

    for (let combination of operatorCombinations) {
        // Evaluate result for this operator combination
        let result = numbers[0];
        for (let i = 0; i < combination.length; i++) {
            if (combination[i] === "+") {
                result += numbers[i + 1];
            } else if (combination[i] === "*") {
                result *= numbers[i + 1];
            } else if (combination[i] === "|"){
                result = Number(result + "" + numbers[i+1]);
            }
        }
        if (result === target) {
            return true;
        }
    }
    return false;
}

fs.readFile(INPUT_FILE, "utf-8", (err, data) => {
    if (err) {
        console.error("Error reading the file:", err);
        return;
    }

    const list = data.split("\n")
        .filter(row => row.trim() !== "")
        .map((row) => row.replace(":", ""))
        .map((row) => row.split(" ").map(Number));

    let result = 0;
    list.forEach((row) => {
        const target = row[0];
        const numbers = row.slice(1);

        if (checkCombinationsForTarget(numbers, target)) {
            result += target;
        }
    });
    console.log({result});
});
